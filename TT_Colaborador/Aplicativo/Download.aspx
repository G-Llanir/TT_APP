<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Download.aspx.cs" Inherits="TT_Colaborador.Download" %>

<%@ Register Src="~/Aplicativo/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/Aplicativo/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>

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

</head>
<body>
    <form id="frmDownload" runat="server">
        <br />
        <div class="col-lg-12">
            <div class="form-stacked row">
                <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                <asp:MultiView ID="BaseMultiView" runat="server">
                    <asp:View ID="view_Download" runat="server">
                        <div class="panel panel-default" runat="server">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Download</b></h3>
                            </div>
                        </div>
                    </asp:View>
                </asp:MultiView>
            </div>
        </div>
    </form>
</body>
</html>
