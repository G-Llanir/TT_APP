<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="PatrimonioLocal_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Patrimonio.PatrimonioLocal_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked">
        <div class="col-lg-12 row">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Local de Patrimônio"></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
        </div>

        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />


        <div>
            <ul id="tab_PatrimonioLocal" class="nav nav-tabs" role="tablist">
                <li role="presentation" class="tabpanel active">
                    <a href="#PatrimonioLocal" id="aba_patrimonioLocal" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Local de Patrimônio</b></a>
                </li>
            </ul>
        </div>

        <div id="tab" class="tab-content">
            <div role="tabpanel" class="tab-pane fade in active" id="patrimonioLocal" aria-labelledby="patrimonioLocal-tab">

                <asp:UpdatePanel ID="updDetalhe" runat="server">
                    <ContentTemplate>
                        <br />


                        <div class="panel panel-default" runat="server" id="div_PatrimonioLocal">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Dados do Local de Patrimônio</b></h3>
                            </div>
                            <div class="panel-body">

                                
                                    <div class="form-stacked row">
                                        <div class="col-lg-12">
                                            <div class="form-group">
                                                <label>ID</label>
                                                <asp:TextBox ID="txtidPatrimonioLocal" class="form-control CaixaTextoMini" runat="server" disabled></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    

                                    <div class="form-stacked row">
                                            <div class="col-lg-8">
                                                <div class="form-group">
                                                    <label>Descrição do Local </label>
                                                    <asp:TextBox ID="txtsDscLocal" class="form-control " runat="server"></asp:TextBox>
                                                </div>
                                            </div>                                        
                                        </div>

                                        <asp:HiddenField ID="hddidParceiro" runat="server" />

                                        <div class="form-stacked row">
                                            <div class="col-lg-4">
                                                <div class="form-group">
                                                    <label>Departamento</label>
                                                    <asp:DropDownList ID="ddlidDepartamento" class="form-control" runat="server"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-stacked row">
                                            <div class="col-lg-12">
                                                <div class="form-group">
                                                    <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
                                                </div>
                                            </div>
                                        </div>

                                    
                                </div>
                            </div>
                        </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                <br />
            </div>

        </div>

        <%----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>


        <fieldset class="form-stacked actions">
            <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-info" runat="server" Text="Salvar" ValidationGroup="DETALHE" />
            <asp:Button ID="cmdEditar" class="btn  btn-lg  btn-danger" runat="server" Text="Editar" />

            <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field-cancel" title="Voltar" onclick="history.go(-2)">
        </fieldset>
        <div id="dialog-Salvar" class="modal" title="Salvar">
            <p>
                <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
            </p>
        </div>

    </div>


    <asp:HiddenField ID="hddidPatrimonioLocal" runat="server" />
</asp:Content>
