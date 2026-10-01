<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Colaboradores_Exibicao.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.Colaboradores_Exibicao" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <asp:UpdatePanel ID="UpdDetalhe" runat="server">
        <ContentTemplate>
            <br />

            <style>
                .invisivel {
                    display: none;
                }

                .senha {
                    display: flex;
                    flex-wrap: wrap;
                }

                    .senha label {
                        width: 100%;
                    }

                    .senha input {
                        width: 80%;
                    }

                    .senha a {
                        width: 20%;
                    }

                .btn-link:hover {
                    text-decoration: none;
                }
            </style>

            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

            <div class="panel panel-default">
                <div class="panel-heading">
                    <h1 class="panel-title">
                        <i class="fa fa-user"></i>
                        <b>
                            <asp:Label runat="server" ID="lblTituloPagina"></asp:Label></b>
                    </h1>
                </div>
                <div class="panel-body">

                    <div class="form-stacked row">
                        <div class="row">

                            <div class="col-lg-3 form-group" style="text-align: center;">
                                <asp:Image ID="imgColaborador" runat="server" AlternateText="Imagem do Colaborador" Style="min-width: 250px; min-height: 150px; max-width: 300px; max-height: 300px;" />
                            </div>

                            <div class="col-lg-9">

                                <div class="col-lg-6 form-group">
                                    <label>Empresa</label>
                                    <asp:TextBox ID="txtEmpresa" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-lg-6 form-group">
                                    <label>E-mail</label>
                                    <asp:TextBox ID="txtsEmail" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-lg-6 form-group">
                                    <label>Departamento</label>
                                    <asp:TextBox ID="txtDepartamento" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-lg-3 form-group">
                                    <label>Ramal</label>
                                    <asp:TextBox ID="txtsRamal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-lg-3 form-group">
                                    <label>Celular Empresa</label>
                                    <asp:TextBox ID="txtsTelCelular" class="form-control" runat="server" TextMode="Phone" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div runat="server" id="div_UsuarioLogado">

                                    <div class="col-lg-2 form-group">
                                        <label>Login</label>
                                        <asp:TextBox ID="txtLogin" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                    </div>

                                    <div class="col-lg-4 form-group">
                                        <label>Dashboard Inicial <i class="fa fa-info-circle" data-toggle="tooltip" title="Define a Página Inicial ao realizar o Login"></i></label>
                                        <asp:DropDownList ID="ddlDashboard" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                    </div>

                                    <div class="col-lg-3 form-group senha">
                                        <label>Senha</label>
                                        <asp:TextBox ID="txtSenha" class="form-control" runat="server" TextMode="Password"></asp:TextBox>
                                        <asp:LinkButton ID="cmdExibe_Senha" runat="server" class="btn btn-lg btn-link fa fa-eye"></asp:LinkButton>
                                        <asp:LinkButton ID="cmdEsconde_Senha" runat="server" class="btn btn-lg btn-link fa fa-eye-slash invisivel"></asp:LinkButton>
                                    </div>

                                    <div class="col-lg-3 form-group senha">
                                        <label>Confirmação de Senha</label>
                                        <asp:TextBox ID="txtConfirmaSenha" class="form-control" runat="server" TextMode="Password"></asp:TextBox>
                                        <asp:LinkButton ID="cmdExibe_ConfirmaSenha" runat="server" class="btn btn-lg btn-link fa fa-eye"></asp:LinkButton>
                                        <asp:LinkButton ID="cmdEsconde_ConfirmaSenha" runat="server" class="btn btn-lg btn-link fa fa-eye-slash invisivel"></asp:LinkButton>
                                        <asp:CompareValidator runat="server" CssClass="erro_validador" ControlToValidate="txtConfirmaSenha" ControlToCompare="txtSenha" Display="Dynamic" ErrorMessage="As senhas não batem!" Type="String" />
                                    </div>

                                </div>

                            </div>

                        </div>
                    </div>

                    <fieldset class="form-stacked actions" style="text-align: right;">
                        <asp:Button runat="server" ID="cmdSalvar" class="btn btn-lg btn-success" Text="Salvar" OnClick="cmdSalvar_Click" OnClientClick="MudarBotao_Salvar(this)" />
                        <input type="submit" name="voltar" class="btn btn-lg btn-warning" id="field_cancel" value="Voltar" title="Voltar" onclick="history.go(-1)" runat="server" />
                    </fieldset>

                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>