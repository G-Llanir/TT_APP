<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DropDownList_Padrao.ascx.cs" Inherits="TT_Flow.App.Controles.DropDownList_Padrao" %>

<div runat="server" id="divPadrao" class="col-lg-12 padd-0">
    <asp:Label runat="server" ID="lblPadrao" Font-Bold="true" style="display: inline-block; margin-bottom: 5px;"></asp:Label>
    <i runat="server" id="iconPadrao" class="fa fa-asterisk" data-toggle="tooltip_top" title="Obrigatório" style="position: relative; bottom: .3rem; font-size: 1rem; color: red;"></i>
    <div runat="server" id="div_InputGroupPadrao">
        <asp:ListBox ID="ddlPadrao" class="form-control Caixa_Selecao" runat="server"></asp:ListBox>
        <asp:HyperLink runat="server" ID="lnkPadrao" CssClass="lnkPadrao input-group-addon" Target="_blank" data-toggle="tooltip_top"></asp:HyperLink>
    </div>
</div>