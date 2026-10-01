<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/App/main.master" CodeBehind="AlterarDados.aspx.cs" Inherits="TT_Login.AlterarDados" %>

<%@ Register Src="~/app/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <!DOCTYPE html>

    <html>


    <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <title></title>

      
        
        
        <link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css">
        <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>
        
        <style>
            .container {
                margin-top: 100px;
            }
            .padding-top-xl { padding-top: 3em; }
        </style>

    </head>

    <body>
        <br/>
        <div class="col-lg-12" class="container">
            <div class="form-group">
                <div class="panel panel-default">

                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_AlterarDados" />

                    <div class="panel-heading">
                        <h3 class="panel-title"><b>Alterar Dados do Usuário</b></h3>
                    </div>
                    <div class="panel-body">
                        <div class="row">


                            <div class="col-lg-6">
                                <div class="form-group">
                                    <label>Login</label>
                                    <asp:TextBox ID="txtsLogin" class="form-control CaixaTextoMedio" runat="server" MaxLength="50"></asp:TextBox>
                                </div>
                            </div>


                            <div class="col-lg-6">
                                <div class="form-group">
                                    <label>Nome</label>
                                    <asp:TextBox ID="txtsDsUsuario" class="form-control  CaixaTextoGrande" Width="400px" runat="server" MaxLength="200"></asp:TextBox>
                                </div>
                            </div>


                            <div class="col-lg-6">
                                <div class="form-group">
                                    <label>Senha</label>
                                    <asp:TextBox ID="txtsSenha" class="form-control  CaixaTextoMedio" runat="server" MaxLength="50" TextMode="Password"></asp:TextBox>
                                </div>
                            </div>


                            <div class="col-lg-6">
                                <div class="form-group">
                                    <label>Confirmação de Senha</label>
                                    <asp:TextBox ID="txtsSenha_Confirmacao" class="form-control  CaixaTextoMedio" runat="server" MaxLength="50" TextMode="Password" OnTextChanged="txtsSenha_TextChanged"></asp:TextBox>
                                </div>
                            </div>


                            <div class="col-lg-6">
                                <div class="form-group">
                                    <label>e-mail</label>
                                    <asp:TextBox ID="txtsEmail" class="form-control " Width="400px" runat="server" MaxLength="200"></asp:TextBox>
                                </div>
                            </div>


                            <fieldset class="form-stacked actions">
                                <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                                <asp:Button ID="cmdCancelar" class="btn  btn-lg btn-warning" runat="server" Text="Voltar" OnClick="cmdCancelar_Click"  />
                                
                            </fieldset>


                        </div>
                    </div>
                </div>

            </div>
        </div>


    <div id="dialog-Salvar" title="Salvar">
        <p><span class="ui-icon ui-icon-alert" style="float:left; margin:12px 12px 20px 0;"></span><asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label></p>
    </div>

        <asp:HiddenField ID="hddIdUsuario" runat="server" />
        <asp:HiddenField ID="hddsSenha" runat="server" />
        <asp:HiddenField ID="hddsSenhaConfirmacao" runat="server" />

    </body>
    </html>
</asp:Content>
