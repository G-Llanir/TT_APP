<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Motivos_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.Motivos_Detalhe" %>
<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>




<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
   <div id="DIV_ESPACO" style="height:600px" runat ="server" visible="false"></div>
   <asp:UpdatePanel ID="updDetalhe" runat="server">
        <ContentTemplate>
        <div class="form-stacked row">
            <div class="col-lg-12">
                <h1><asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label></h1>
                <uc1:BreadCrumb runat="server" ID="BreadCrumb"  NivelPagina="3" TitulodaPagina="Detalhe"/>
            </div>
        </div>
                   
        <div class="form-stacked row">
 
            <div class="col-lg-12">
                <uc1:MensagemPagina runat="server" id="MensagemPagina" />
            </div>
 
            <div class="col-lg-12">
                <div class="form-group">
                    <label>ID</label>
                    <asp:TextBox ID="txtidMotivo" runat="server" class="form-control CaixaTextoMini" attrname="idMotivo" disabled="0">    
                    </asp:TextBox>
                </div>
            </div> 

            <div class="col-lg-12">
                <div class="form-group">
                  <label> Tipo Motivo </label>
                    <asp:DropDownList ID="ddlsTipoMotivo" runat="server" class="form-control CaixaTextoMedio"  attrname="Tipo Motivo" > 
                        <asp:ListItem Value="CancelarPedido"> Cancelamento de Pedido </asp:ListItem>
                        <asp:ListItem Value="AlterarDepto"> Alteração de Departamento </asp:ListItem>
                    </asp:DropDownList>
                 </div>
            </div> 

            <div class="col-lg-12">
                <div class="form-group">
                  <label> Descrição Motivo </label>
                    <asp:TextBox ID="txtsDscMotivo" runat="server" class="form-control CaixaTextoGigante" attrname="sDscMotivo" >    
                    </asp:TextBox>
                </div>
            </div> 

            <div class="col-lg-12">
                <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
            </div> 

        </div>
        <uc1:PainelAtualizacao runat="server" id="PainelAtualizacao" />
                         
        <fieldset class="form-stacked actions">
            <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
            &nbsp;
            <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" " id="field-cancel" title="Voltar" onclick="history.go(-1)" >&nbsp;
        </fieldset>

        <asp:HiddenField ID="hddidMotivo" runat="server" />

    </ContentTemplate>            
    </asp:UpdatePanel>
     
</asp:Content>
