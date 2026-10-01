<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="botpress.aspx.cs" Inherits="TT_Flow.App.botpress" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="frmRetorno" runat="server">
        <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="true" GridLines="None"
    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
    <Columns>
    </Columns>
</asp:GridView>
    </form>
</body>
</html>
