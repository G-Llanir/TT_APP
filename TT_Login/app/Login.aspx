<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="TT_Login.Login" %>

<!DOCTYPE html>
<html lang="pt-br">

    <head>
        <meta charset="utf-8">
        <meta http-equiv="X-UA-Compatible" content="IE=edge">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <!-- Just for debugging purposes. Don't actually copy this line! -->
        <!--[if lt IE 9]><script src="../../assets/js/ie8-responsive-file-warning.js"></script><![endif]-->
        <!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries -->
        <!--[if lt IE 9]>
            <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
            <script src="https://oss.maxcdn.com/libs/respond.js/1.4.2/respond.min.js"></script>
        <![endif]-->
        <title><%=TT.FrameWork.Identity.Variaveis.sNomeSistema() %> </title>
        <link href="/app/css/bootstrap.css" rel="stylesheet">
        <link href="/app/css/font-awesome/css/font-awesome.css" rel="stylesheet">
        <link href="/app/css/sb-admin.css" rel="stylesheet">
        
    </head>
    <script type="text/javascript">
        function MudarBotao() {

            document.getElementById("cmdLogin").value = "Aguarde..";
       }

    </script>

    <body>
    <div class="container">
        <div class="row">
            <div class="col-md-4 col-md-offset-4" >
                <div class="login-panel panel panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title"> <i class="fa fa-bar-chart-o"></i> <%=TT.FrameWork.Identity.Variaveis.sNomeSistema() %> Login</h3>
                    </div>
                    <div class="panel-body">
                        <form id="form1" runat="server" role="form">
                            <fieldset>
                                <div class="form-group">
                                    <asp:TextBox ID="txtUsuario" runat="server" class="form-control" placeholder="Usuário" MaxLength="30" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="vldtxtUsuario" runat="server" ErrorMessage="Informe o Usuário" ControlToValidate="txtUsuario" Display="Dynamic" SetFocusOnError="True" ValidationGroup="Login"></asp:RequiredFieldValidator>
                                </div>
                                <div class="form-group">
                                    <asp:TextBox ID="txtSenha" runat="server" class="form-control" placeholder="Senha" value="" MaxLength="30" TextMode="Password" ValidationGroup="LOGIN" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="vldtxtSenha" runat="server" ErrorMessage="Informe a Senha" ControlToValidate="txtSenha" Display="Dynamic" SetFocusOnError="True" ValidationGroup="Login"></asp:RequiredFieldValidator>
                                </div>
                                <asp:Label ID="lblresultado" runat="server" ForeColor="Red"></asp:Label>
                                <br />
                               <asp:Button ID="cmdLogin" runat="server" class="btn btn-lg btn-success btn-block " text="Login" OnClick="cmdLogin_Click" ValidationGroup="Login"  onclientclick="MudarBotao()" />
                            </fieldset>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script src="/app/js/jquery-1.10.2.js"></script>
    <script src="/app/js/bootstrap.min.js"></script>
    <script src="/app/js/plugins/metisMenu/jquery.metisMenu.js"></script>
    <script src="/app/js/sb-admin.js"></script>

</body>

</html>
