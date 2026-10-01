<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Referencia_MesAno.ascx.cs" Inherits="TT_Colaboradores.Aplicativo.Controles.Referencia_MesAno" %>
<div class="form-group">



    <asp:DropDownList ID="ddlMes" runat="server" class="form-select">
        <asp:ListItem Value="">Mês</asp:ListItem>
        <asp:ListItem Value="01">Janeiro</asp:ListItem>
        <asp:ListItem Value="02">Fevereiro</asp:ListItem>
        <asp:ListItem Value="03">Março</asp:ListItem>
        <asp:ListItem Value="04">Abril</asp:ListItem>
        <asp:ListItem Value="05">Maio</asp:ListItem>
        <asp:ListItem Value="06">Junho</asp:ListItem>
        <asp:ListItem Value="07">Julho</asp:ListItem>
        <asp:ListItem Value="08">Agosto</asp:ListItem>
        <asp:ListItem Value="09">Setembro</asp:ListItem>
        <asp:ListItem Value="10">Outubro</asp:ListItem>
        <asp:ListItem Value="11">Novembro</asp:ListItem>
        <asp:ListItem Value="12">Dezembro</asp:ListItem>
    </asp:DropDownList>



    <asp:DropDownList ID="ddlAno" runat="server" class="form-select"></asp:DropDownList>


    <%--<asp:DropDownList ID="ddlMostraAnos" runat="server" class="form-control CaixaTextoPequeno" >
          <asp:ListItem Value="True"></asp:ListItem>
         <asp:ListItem Value="True">Sim</asp:ListItem>
         <asp:ListItem Value="False">Não</asp:ListItem>
    </asp:DropDownList>--%>
</div>
