<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Pesquisa_Parceiros.ascx.cs" Inherits="TT_Flow.App.Controles.Pesquisa_Parceiros" ViewStateMode="Enabled"%>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:UpdatePanel ID="updpClientesParceiros" runat="server" UpdateMode="Conditional">

   

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


<%--<div runat="server" id="div_BuscarParceiros">

     <div class="col-lg-12">
         <div class="row">
             <div class="col-lg-2">
                 <div class="form-group">
                     <label>CNPJ / CPF</label>
                     <asp:TextBox ID="TextBox1" class="form-control" runat="server" MaxLength="25" OnTextChanged="txtParceiro_sCPF_CNPJ_TextChanged" AutoPostBack="true"></asp:TextBox>
                 </div>
             </div>
         </div>
     </div>
     <div class="col-lg-12">
         <div class="row">
             <div class="col-lg-4">
                 <div class="form-group">
                     <label>Razão social </label>
                     <asp:TextBox ID="TextBox2" class="form-control" runat="server" MaxLength="100" OnTextChanged="txtParceiro_sRazaoSocial_TextChanged" AutoPostBack="true"></asp:TextBox>
                 </div>
             </div>
         </div>
     </div>
 </div>--%>



        <div  runat="server" id="div_BuscarParceiros" >
            <div class="col-lg-3" id="divCnpjCpf" runat="server">
                <div class="form-group">
                    <label>CNPJ / CPF</label>
                    <asp:TextBox ID="txtParceiro_sCPF_CNPJ" class="form-control" runat="server" MaxLength="25" placeholder="CNPJ ou CPF" OnTextChanged="txtParceiro_sCPF_CNPJ_TextChanged" AutoPostBack="true"></asp:TextBox>
                </div>
            </div>

            <div class="col-lg-4" id="divRazao" runat="server">
                <div class="form-group">
                    <label>Razão social </label>
                    <asp:TextBox ID="txtParceiro_sRazaoSocial" class="form-control" runat="server" placeholder="Razão Social" MaxLength="100" OnTextChanged="txtParceiro_sRazaoSocial_TextChanged" AutoPostBack="true"></asp:TextBox>
                </div>
            </div>
        </div>

        <asp:HiddenField ID="hddClientes_idParceiro" runat="server" />
        <asp:HiddenField ID="hddClientes_sNomeFantasia" runat="server" />
        <asp:HiddenField ID="hddClientes_sRazaoSocial" runat="server" />
        <asp:HiddenField ID="hddClientes_sTipoParceiro" runat="server" />

    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="txtParceiro_sCPF_CNPJ" EventName="TextChanged" />
    </Triggers>
</asp:UpdatePanel>
