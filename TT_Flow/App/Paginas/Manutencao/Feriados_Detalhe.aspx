<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Feriados_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.Feriado.Feriados_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmFeriados_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <style>
        .colorLink {
            background-color: aqua;
        }
    </style>
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>
    <script src="/app/js/vanilla-masker.js"></script>

    <div class="form-stacked">
        <div class="col-lg-12 row">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Feriados"></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
        </div>

        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

        <div>
            <ul id="tab_Feriado" class="nav nav-tabs" role="tablist">
                <li role="presentation" class="tabpanel active">
                    <a href="#feriado" id="aba_feriado" role="tabpanel" data-toggle="tab" aria-controls="feriado" aria-expanded="false"><b>Feriado</b></a>
                </li>
            </ul>
        </div>

        <br />
        <div id="tab" class="tab-content">

            <div role="tabpanel" class="tab-pane fade in active" id="feriado" aria-labelledby="aba_feriado">
                <asp:UpdatePanel ID="updDetalhe" runat="server">
                    <ContentTemplate>
                        <div class="panel panel-default" runat="server" id="div_feriado">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Dados do Feriado</b></h3>
                            </div>
                            <div class="panel-body">

                                <div class="form-stacked row">

                                    <div class="col-lg-2">
                                        <div class="form-group">

                                            <label>ID</label>

                                            <div class="form-group input-group">

                                                <asp:LinkButton ID="cmRetornar" class="btn btn-sm btn-info input-group-addon colorLink" runat="server" OnClick="cmdRetornar_click"><b><</b></asp:LinkButton>

                                                <asp:TextBox ID="txtidFeriado" class="form-control" runat="server" disabled></asp:TextBox>

                                                <asp:LinkButton ID="cmAvancar" class="btn btn-sm btn-info input-group-addon colorLink" runat="server" OnClick="cmdAvancar_click"><b>></b></asp:LinkButton>

                                            </div>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-lg-12">
                                            
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Data do Feriado</label>
                                                    <asp:TextBox ID="txtDtFeriado" class="form-control" type="date" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-4">
                                                <div class="form-group">
                                                    <label>Tipo de Feriado</label>
                                                    <asp:DropDownList ID="ddlidTipo" class="form-control" runat="server"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>


                                    <div class="row">

                                        <div class="col-lg-12">

                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>Descrição do Feriado</label>
                                                    <asp:TextBox ID="txtsDscFeriado" class="form-control " runat="server"></asp:TextBox>
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
            <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field-cancel" title="Voltar" onclick="history.go(-2)">
        </fieldset>
        <div id="dialog-Salvar" class="modal" title="Salvar">
            <p>
                <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
            </p>
        </div>

    </div>


    <asp:HiddenField ID="hddidFeriado" runat="server" />
</asp:Content>
