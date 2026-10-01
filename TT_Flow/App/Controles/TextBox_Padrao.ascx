<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TextBox_Padrao.ascx.cs" Inherits="TT_Flow.App.Controles.TextBox_Padrao" %>

<div runat="server" id="divPadrao" class="col-lg-12 padd-0">
    <asp:Label runat="server" ID="lblPadrao" Font-Bold="true" style="display: inline-block; margin-bottom: 5px;"></asp:Label>
    <i runat="server" id="iconPadrao" class="fa fa-asterisk" data-toggle="tooltip_top" title="Obrigatório" style="position: relative; bottom: .3rem; font-size: 1rem; color: red;"></i>
    <div runat="server" id="div_InputGroupPadrao">
        <asp:Literal runat="server" ID="ltr_InputGroupPadrao_1"></asp:Literal>
        <asp:TextBox ID="txtPadrao" class="form-control" runat="server"></asp:TextBox>
        <asp:Literal runat="server" ID="ltr_InputGroupPadrao_2"></asp:Literal>
    </div>
</div>