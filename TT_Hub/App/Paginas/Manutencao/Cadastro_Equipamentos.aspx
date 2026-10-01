<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Cadastro_Equipamentos.aspx.cs" Inherits="TT_Hub.App.Paginas.Manutencao.Cadastro_Equipamentos" %>
<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="DIV_ESPACO" style="height:600px" runat ="server" visible="false"> </div>

    <link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css">
    <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>
    <script src="/app/js/vanilla-masker.js"></script>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="False" UpdateMode="Conditional">
    <ContentTemplate>


         <div class="form-stacked row">
            <div class="col-lg-12">
                <br />
                <uc1:BreadCrumb runat="server" ID="BreadCrumb"  NivelPagina="2" TitulodaPagina="Cadastro de Equipamentos"/>
                <div class="well">
                    <h4>
                        <span id="MainContent_lblTituloPagina">
                            <asp:Label ID="lblTituloPagina" runat="server" Text="Cadastro de Equipamento"></asp:Label>
                        </span>
                    </h4>
                </div>
            <uc1:MensagemPagina runat="server" id="MensagemPagina" />

                        <div role="tabpanel" class="tab-pane fade in active" id="dados" aria-labelledby="dados-tab">
                            <br />
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>Dados de Equipamentos</b></h3>
                                </div>
                                <div class="panel-body">
                                    <div class="row">       
                                        

                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <label>ID</label>
                                                <asp:TextBox ID="txtidTipoEquipamento" class="form-control uppercase  CaixaTextoGrande" runat="server" MaxLength="200" ></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <label> Tipo do equipamento</label>
                                                <asp:TextBox ID="txtsDscTipoEquipamento" class="form-control uppercase  CaixaTextoGrande" runat="server" MaxLength="200" ></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-6">
                                            <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
                                        </div>
                                        </div>
                                    </div>
                                </div>

                                      
                            

                            <uc1:PainelAtualizacao runat="server" id="PainelAtualizacao" />
                            
                        <fieldset class="form-stacked actions">
                            <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar"  />
                            &nbsp;
                            <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" " id="field-cancel" title="Voltar" onclick="history.go(-1)" >&nbsp;
                        </fieldset>

                       

        <div id="dialog-Salvar" title="Salvar">
            <p><span class="ui-icon ui-icon-alert" style="float:left; margin:12px 12px 20px 0;"></span><asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label></p>
        </div>

        <asp:HiddenField ID="hddidTipoEquipamento" runat="server" />

    </ContentTemplate>            
</asp:UpdatePanel>
</asp:Content>
