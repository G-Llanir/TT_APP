<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Download.aspx.cs" Inherits="TT_Flow.App.Download" %>

<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <title></title>
    <link href="https://login.tecandtec.com.br/app/css/bootstrap.css" rel="stylesheet" />
    <link href="https://login.tecandtec.com.br/app/css/font-awesome/css/font-awesome.css" rel="stylesheet" />
    <link href="https://login.tecandtec.com.br/app/css/TT.css" rel="stylesheet" />
    <link href="https://login.tecandtec.com.br/app/css/plugins/datepicker/css/datepicker.css" rel="stylesheet" />


    <script src="https://login.tecandtec.com.br/app/js/jquery-3.2.0.min.js"></script>
    <script src="https://login.tecandtec.com.br/app/js/bootstrap.min.js"></script>
    <script src="https://login.tecandtec.com.br/app/js/plugins/datepicker/bootstrap-datepicker.js"></script>

    <link href="/app/css/font-awesome/css/font-awesome.min.css" rel="stylesheet" />

    <style>
        .center-screen {
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            position: relative;
            z-index: 2;
        }

        .login-box {
            background-color: #f8f9fa;
            padding: 20px;
            border: 1px solid #dee2e6;
            border-radius: 10px;
            box-shadow: 0 0 15px rgba(0, 0, 0, 0.1);
            z-index: 0;
        }

        .login-header h3 {
            margin-top: 0;
        }

        body::before {
            content: "";
            background-image: url('https://acdn.mitiendanube.com/stores/001/247/779/themes/common/logo-1495870104-1701438222-73f84be58ad71c04e9e375749f7466d41701438222.png?0'); /* Substitua pela sua imagem */
            background-repeat: no-repeat;
            background-position: center;
            background-size: 85% 55%;
            transform: rotate(-25deg);
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            opacity: 0.1;
            z-index: 3;
            pointer-events: none;
        }

        .visible {
            display: none;
        }
    </style>
</head>
<body>
    <form id="frmDownload" runat="server">
        <div class="center-screen">
            <div class="login-box" runat="server" id="DIV_Login">
                <div class="login-header">
                    <h3>Senha de Acesso</h3>
                </div>
                <div class="form-inline">
                    <asp:TextBox ID="txtSenha" runat="server" TextMode="Password" CssClass="form-control" placeholder="Senha" Style="width: 200px;" />
                    <asp:Button ID="btnAcessar" runat="server" CssClass="btn btn-primary" Text="Acessar" Style="margin-left: 10px;" OnClick="btnAcessar_Click" />
                </div>
                <div class="">
                    <br />
                    <uc1:MensagemPagina runat="server" ID="Mensagem" />
                </div>
            </div>

            <div class="login-box" runat="server" id="DIV1">
                <div class="login-header">
                    <h3>Login</h3>
                </div>

                <div class="form-group">
                    <div class="input-group" runat="server" id="DIV4">
                        <span class="input-group-addon" id="Span1" runat="server" style="font-weight: bold;"></span>
                        <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" placeholder="Usuário" Style="width: 160px; z-index: 0;" aria-describedby="Span1" />
                    </div>
                </div>
                <div class="form-group">
                    <asp:TextBox ID="txtsSenha" runat="server" TextMode="Password" CssClass="form-control" placeholder="Senha" Style="width: 230px; margin-bottom: 10px;" />
                </div>
                <div class="form-group">
                    <asp:TextBox ID="txtsChave" runat="server" CssClass="form-control" placeholder="Chave de Acesso" Style="width: 230px; margin-bottom: 10px;" />
                </div>
                <div class="form-group">
                    <asp:Button ID="btnLogin" runat="server" CssClass="btn btn-primary" Text="Acessar" OnClick="btnLogin_Click" Style="width: 230px;" />
                </div>
                <div class="form-group" runat="server" id="btnCadastre">
                    <asp:Button ID="Cadastre" runat="server" CssClass="btn btn-warning" Text="Cadastre-se" OnClick="Cadastre_Click" Style="width: 230px;" />
                </div>
                <div class="form-group">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina1" Style="width: 200px;" />
                </div>
            </div>

            <div class="login-box" runat="server" id="DIV2">
                <div class="login-header">
                    <h3>Cadastre-se</h3>
                </div>
                <div class="form-group">
                    <asp:TextBox ID="txtsNome" runat="server" CssClass="form-control" placeholder="Nome e Sobrenome" Style="width: 230px; margin-bottom: 10px;" />
                </div>
                <div class="form-group">
                    <asp:TextBox ID="txtsEmail" runat="server" CssClass="form-control" placeholder="Email" Style="width: 230px; margin-bottom: 10px;" />
                </div>
                <div class="form-group">
                    <div class="input-group" runat="server" id="DIV_Produtos">
                        <span class="input-group-addon" id="lblsPrefixoLogin" runat="server" style="font-weight: bold;"></span>
                        <asp:TextBox ID="txtsUsuario" runat="server" CssClass="form-control" placeholder="Usuário" Style="width: 160px; z-index: 0;" aria-describedby="lblsPrefixoLogin" />
                    </div>
                </div>

                <div class="form-group" style="display: flex; align-items: center;">
                    <asp:TextBox ID="txtsSenhaCadastro" runat="server" TextMode="Password" CssClass="form-control" placeholder="Senha" Style="width: 200px; margin-right: 1px;" />

                    <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-small fa fa-eye" TabIndex="100" ToolTip="Visualizar Senha" OnClick="LinkButton1_Click" Style="width: 1px; height: 25px;"></asp:LinkButton>
                    <asp:LinkButton ID="LinkButton3" runat="server" CssClass="btn btn-small fa fa-eye-slash" TabIndex="100" ToolTip="Esconder Senha" OnClick="LinkButton3_Click" Style="width: 1px; height: 25px;"></asp:LinkButton>
                </div>

                <div class="form-group">
                    <asp:Button ID="btnCadastro" runat="server" CssClass="btn btn-primary" Text="Cadastre-se" OnClick="btnCadastro_Click" Style="width: 230px;" />
                </div>
                <div class="form-group">
                    <asp:Button ID="btnVoltar" runat="server" CssClass="btn btn-warning" Text="Voltar" OnClick="btnVoltar_Click" Style="width: 230px;" />
                </div>
                <div class="">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina2" />
                </div>
            </div>

            <div class="login-box" runat="server" id="DIV3">
                <div class="login-header">
                    <h3>Pedidos - Arquivos STSO</h3>
                </div>

                <div class="">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina4" />
                </div>

                <div class="form-group">

                    <asp:GridView ID="gv_Pedidos" class="table table-striped table-bordered table-hover table-condensed"
                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                        Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" Style="width: 1000px;" DataKeyNames="idPedido"
                        OnRowDataBound="gv_Pedidos_RowDataBound" OnRowCommand="gv_Pedidos_RowCommand">
                        <Columns>

                            <asp:BoundField DataField="idPedido" HeaderText="ID">
                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                            </asp:BoundField>

                            <asp:BoundField DataField="sPedidoCliente" HeaderText="N° Pedido">
                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                            </asp:BoundField>

                            <asp:BoundField DataField="sReferencia" HeaderText="Referência">
                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                            </asp:BoundField>

                            <asp:BoundField DataField="sDscCliente" HeaderText="Cliente">
                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                            </asp:BoundField>

                            <asp:BoundField DataField="dtPedido" HeaderText="Data Pedido">
                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                            </asp:BoundField>

                            <asp:BoundField DataField="sChave" HeaderText="sChave">
                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="visible" />
                                <HeaderStyle CssClass="visible" />
                            </asp:BoundField>

                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkDownload" runat="server" CommandName="Download" CssClass="btn btn-primary" CommandArgument='<%# Eval("idPedido") %>'>Download</asp:LinkButton>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="4%" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                </div>
            </div>

            <div class="login-box" runat="server" id="DIV5">
                <div class="login-header">
                    <h3>Alterar Senha</h3>
                </div>

                <div class="form-group">
                    <div class="input-group" runat="server" id="DIV6">
                        <span class="input-group-addon" id="Span2" runat="server" style="font-weight: bold;"></span>
                        <asp:TextBox ID="txtsUsuário_AlterarSenha" runat="server" CssClass="form-control" placeholder="Usuário" Style="width: 160px; z-index: 0;" aria-describedby="Span1" />
                    </div>
                </div>
                <div class="form-group">
                    <asp:TextBox ID="txtsSenha_AlterarSenha" runat="server" TextMode="Password" CssClass="form-control" placeholder="Senha" Style="width: 230px; margin-bottom: 10px;" />
                </div>
                <div class="form-group">
                    <asp:TextBox ID="txtsEmail_AlterarSenha" runat="server" CssClass="form-control" placeholder="Email" Style="width: 230px; margin-bottom: 10px;" />
                </div>
                <div class="form-group">
                    <asp:TextBox ID="txtsNome_AlterarSenha" runat="server" CssClass="form-control" placeholder="Nome" Style="width: 230px; margin-bottom: 10px;" />
                </div>
                <div class="form-group">
                    <asp:Button ID="btnSalvar_AlterarSenha" runat="server" CssClass="btn btn-primary" Text="Salvar" OnClick="btnSalvar_AlterarSenha_Click" Style="width: 230px;" />
                </div>
                <div class="form-group">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina3" Style="width: 200px;" />
                </div>
            </div>

        </div>
    </form>
</body>
</html>
