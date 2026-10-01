<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ComboAtivo.ascx.cs" Inherits="TT_Hub.App.Controles.ComboAtivo" %>
<div class="form-group">
    <label><asp:Label ID="lblTitulo" runat="server" Text="Ativo"></asp:Label></label>
    <asp:DropDownList ID="ddlAtivo" runat="server"  class="form-control CaixaTextoPequeno"  >
        <asp:ListItem Selected="True" Value="S">Sim</asp:ListItem>
        <asp:ListItem Value="N">Não</asp:ListItem>
    </asp:DropDownList>
    <asp:TextBox ID="txtAtivo" class="form-control CaixaTextoPequeno " runat="server" ></asp:TextBox>


</div>