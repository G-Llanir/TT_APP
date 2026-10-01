<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContadorMensagem.ascx.cs" Inherits="TT_Flow.App.Controles.ContadorMensagem" %>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:Timer ID="tmrAtualiza" runat="server" OnTick="tmrAtualiza_Tick">
        </asp:Timer>
        <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
    </ContentTemplate>
</asp:UpdatePanel>

