<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Embalagem_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.Embalagem_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmEmbalagem_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked">
        <div class="col-lg-12 row">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Embalagem"></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
        </div>

        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />


        <div>
            <ul id="tab_Embalagem" class="nav nav-tabs" role="tablist">
                <li role="presentation" class="tabpanel active">
                    <a href="#Embalagem" id="aba_Embalagem" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Embalagem</b></a>
                </li>
            </ul>
        </div>

        <div id="tab" class="tab-content">
            <div role="tabpanel" class="tab-pane fade in active" id="Embalagem" aria-labelledby="Embalagem-tab">

                
                        <br />


                        <div class="panel panel-default" runat="server" id="div_Embalagem">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Dados da Embalagem</b></h3>
                            </div>
                            <div class="panel-body">

                                <div class="form-stacked row">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaUnitizados" />
                                    <div class="row">
                                        <div class="col-lg-12" id="DivBipador" runat="server">
                                            <div class="col-lg-12">
                                                <div class="table table-responsive" style="border: 2px solid #ccc; border-radius: 5px; margin-bottom: 10px; width: 100%; height: auto;">
                                                    <div id="camera"></div>
                                                    <div id="resultado" style="font-size: 18px;"></div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-12" id="DivManual" runat="server">
                                            <div class="form-group">
                                         <%--       <div class="col-lg-6">
                                                    <asp:Button ID="cmdAbrirCâmera" OnClick="AbrirCamera_Click" CssClass="btn btn-sm btn-primary" Text="Incluir" runat="server" />
                                                </div>--%>
                                                <div class="col-lg-6">
                                                    <asp:TextBox ID="txtsCodigoBarras" class="form-control " runat="server" placeholder="Código de Barras"></asp:TextBox>
                                                </div>
                                                <div class="col-lg-6">
                                                    <asp:Button ID="cmdIncluirUnitizadoItem" OnClick="IncluirUnitizadosItens_Click" CssClass="btn btn-sm btn-primary" Text="Incluir" runat="server" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <br />
                                    <div class="row">
                                        <div class="col-lg-12">
                                            <div id="div_UnitizadosItens" class="table table-responsive" runat="server">
                                                <asp:GridView ID="dtgUNItens" runat="server" class="table table-striped table-bordered table-hover table-condensed"
                                                    Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    ShowFooter="False" DataKeyNames="idItem">
                                                    <Columns>
                                                        <asp:BoundField DataField="SCodigo" HeaderText="Código Produto" />
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>

                                 <%--   <div class="row">
                                        <div class="col-lg-12">
                                            <asp:Button ID="cmdAbrirArquivos" runat="server" Text="Próximo" OnClick="AbrirArquivos_Click" CssClass="btn btn-primary" />
                                        </div>
                                    </div>--%>
                                </div>
                            </div>
                        </div>
                        </div>
               

                <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                <br />
            </div>

        </div>

        <%----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>


        <fieldset class="form-stacked actions">
            <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-info" runat="server" Text="Salvar" ValidationGroup="DETALHE" />
            <asp:Button ID="Button1" class="btn  btn-lg btn-info" runat="server" Text="Salvar" ValidationGroup="DETALHE" />
            <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field-cancel" title="Voltar" onclick="history.go(-2)">
        </fieldset>
        <div id="dialog-Salvar" class="modal" title="Salvar">
            <p>
                <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
            </p>
        </div>

    </div>


    <asp:HiddenField ID="hddidEmbalagem" runat="server" />
</asp:Content>
