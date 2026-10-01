<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/App/main.master" CodeBehind="Sistemas.aspx.cs" Inherits="TT_Login.Sistemas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <!DOCTYPE html>

    <html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
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
        <link href="/app/css/TT.css" rel="stylesheet" />

        <script src="/app/js/jquery-3.2.0.min.js"></script>
        <script src="/app/js/bootstrap.min.js"></script>
        <script src="/app/js/sb-admin.js"></script>

        <style>
            *:focus, *:focus-visible {
                outline: none !important;
            }

            a {
                color: green;
            }

                a:hover {
                    text-decoration: underline;
                    color: green !important;
                }

            .caixaBotao:hover {
                box-shadow: 3px 3px 7px #009a22, -3px -3px 7px #009a22;
                -webkit-transition: box-shadow ease-out 0.1s;
                transition: box-shadow ease-out 0.1s;
            }
        </style>

    </head>
    <body>
        <div class="text-center">
            <div class="row">
                <div class="col-lg-8  col-md-offset-2">
                    <header>
                        <h1>Bem-vindo</h1>
                        <br />
                        <br />
                    </header>
                    <asp:Literal ID="ltrBotoes" runat="server"></asp:Literal>
                </div>
            </div>
        </div>
    </body>
    </html>
</asp:Content>
