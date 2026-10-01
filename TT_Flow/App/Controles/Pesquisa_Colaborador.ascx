<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Pesquisa_Colaborador.ascx.cs" Inherits="TT_Flow.App.Controles.Pesquisa_Colaborador" ViewStateMode="Enabled"%>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:UpdatePanel ID="updpColaboradores" runat="server" UpdateMode="Conditional">

   

    <ContentTemplate>
<style>
    .centralizar-div {
        margin-right: auto;
        margin-left: auto;
        width: 95%; 
    }
</style>

<div class="centralizar-div">
    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
</div>


        <div  runat="server" id="div_BuscarColaboradores">
            <div class="col-lg-3">
                <div class="form-group">
                    <label>CPF</label>
                    <asp:TextBox ID="txtColaborador_CPF" class="form-control" runat="server" MaxLength="20"  OnTextChanged="Colaborador_CPF_TextChanged" AutoPostBack="true"></asp:TextBox>
                </div>
            </div>

            <div class="col-lg-4">
                <div class="form-group">
                    <label>Nome Colaborador </label>
                    <asp:TextBox ID="txtColaborador_Nome" class="form-control" runat="server" MaxLength="200" OnTextChanged="txtColaborador_Nome_TextChanged" AutoPostBack="true"></asp:TextBox>
                </div>
            </div>
        </div>

        <asp:HiddenField ID="hddidColaborador" runat="server" />

    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="txtColaborador_CPF" EventName="TextChanged" />
    </Triggers>
</asp:UpdatePanel>
