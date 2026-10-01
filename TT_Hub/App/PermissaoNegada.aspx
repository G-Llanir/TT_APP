<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="PermissaoNegada.aspx.cs" Inherits="TT_Flow.App.PermissaoNegada" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
          <link href="https://login.tecandtec.com.br/app/css/bootstrap.min.css" rel="stylesheet"/>
        <link href="https://login.tecandtec.com.br/app/css/sb-admin.css" rel="stylesheet"/>
  
    <div id="DIV_ESPACO" style="height:600px" runat ="server" visible="false"> </div>
   
    <div class="text-center">
    <div class="form-stacked row ">
        <br />
        <br />
        <div class="col-lg-12">
            <asp:Label ID="lblTituloPagina" CssClass="text-danger" runat="server" Text="TITULO_PAGINA"></asp:Label>
        </div>      
        
        <div class="col-lg-12">
            <br />
            <br />
        
            <input type="submit" name="cmdVoltars" class="btn btn-sm btn-warning" value="Voltar" id="field-cancel" title="Voltar" onclick="history.go(-1)">
        </div>
    </div>
    </div>

</asp:Content>
