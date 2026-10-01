<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Referencia_MesAno.ascx.cs" Inherits="TT_Flow.App.Controles.Referencia_MesAno" %>

<div class="form-group">
    <asp:DropDownList ID="ddlMes" runat="server" class="form-control CaixaTextoPequeno Caixa_Selecao">
        <asp:ListItem Value="0">Mês</asp:ListItem>
        <asp:ListItem Value="1">1 - Janeiro</asp:ListItem>
        <asp:ListItem Value="2">2 - Fevereiro</asp:ListItem>
        <asp:ListItem Value="3">3 - Março</asp:ListItem>
        <asp:ListItem Value="4">4 - Abril</asp:ListItem>
        <asp:ListItem Value="5">5 - Maio</asp:ListItem>
        <asp:ListItem Value="6">6 - Junho</asp:ListItem>
        <asp:ListItem Value="7">7 - Julho</asp:ListItem>
        <asp:ListItem Value="8">8 - Agosto</asp:ListItem>
        <asp:ListItem Value="9">9 - Setembro</asp:ListItem>
        <asp:ListItem Value="10">10 - Outubro</asp:ListItem>
        <asp:ListItem Value="11">11 - Novembro</asp:ListItem>
        <asp:ListItem Value="12">12 - Dezembro</asp:ListItem>
    </asp:DropDownList>

    <asp:DropDownList ID="ddlAno" runat="server" class="form-control CaixaTextoPequeno Caixa_Selecao"></asp:DropDownList>
</div>
