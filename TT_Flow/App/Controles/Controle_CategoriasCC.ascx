<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Controle_CategoriasCC.ascx.cs" Inherits="TT_Flow.App.Controles.Controle_CategoriasCC" %>

<div runat="server" id="AcaoEncontrada" visible="false">
    <div class="col-lg-4" runat="server" id="Div_CentroCusto">
        <div class="form-group">
            <label>Centro de custo</label>
            <asp:DropDownList ID="ddlidCentroDeCusto" class="form-control  Caixa_Selecao" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidCentroDeCusto_SelectedIndexChanged"></asp:DropDownList>
        </div>
    </div>

    <div class="col-lg-4 _danger" runat="server" id="DivCategoria" visible="false">
        <div class="form-group">
            <label>Categoria Centro de Custo</label>
            <asp:DropDownList ID="ddlCategoriaCC" class="form-control  Caixa_Selecao" runat="server" AutoPostBack="true"></asp:DropDownList>
        </div>
    </div>
</div>
<div runat="server" id="AcaoNaoEncontrada" visible="false">
    <label>Defina uma Ação Válida entre "subtrair ou somar" para usar o Controle </label>
</div>

<asp:HiddenField ID="hddidCentroCusto" runat="server" />
<asp:HiddenField ID="hddidRegistro" runat="server" />
<asp:HiddenField ID="hddidRegistroAntigo" runat="server" />

