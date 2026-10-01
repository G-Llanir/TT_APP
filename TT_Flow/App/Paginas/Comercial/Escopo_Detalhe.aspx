<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Escopo_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Comercial.Escopo_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/FiltroPesquisa.ascx" TagPrefix="uc1" TagName="FiltroPesquisa" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <asp:UpdatePanel ID="UpdDetalhe" runat="server">
        <ContentTemplate>

            <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>
            </div>

            <br />

            <div id="hdd">
                <asp:HiddenField runat="server" ID="hddidEscopo" />
            </div>

            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Escopo</b></h3>
                </div>
                <div class="panel-body">
                    <div class="row">

                        <div class="col-lg-12">
                            <div class="col-lg-2 row">
                                <div class="form-group">
                                    <label>ID</label>
                                    <asp:TextBox runat="server" ID="txtidEscopo" class="form-control CaixaTextoMini" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <label>Descrição</label>
                                <asp:TextBox runat="server" ID="txtsDscEscopo" class="form-control"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-12">
                            <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
                        </div>

                    </div>
                </div>
            </div>

            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Categorias de Escopo</b></h3>
                </div>
                <div class="panel-body">
                    <div runat="server" id="div_IncluirCategoria" class="col-lg-12 row">

                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_IncluirCategoria" />

                        <div class="col-lg-2">
                            <div class="form-group">
                                <label>Categorias</label>
                                <asp:DropDownList runat="server" ID="ddlCategoria" class="form-control"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <div class="col-lg-3">
                                    <label>&nbsp;</label>
                                    <asp:Button runat="server" ID="cmdIncluirCategoria" class="btn btn-info form-control" Text="Incluir" OnClick="cmdIncluirCategoria_Click"></asp:Button>
                                </div>
                            </div>
                        </div>

                    </div>

                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_gvCategoria" />

                    <div runat="server" id="div_gvCategoria" class="col-lg-12">

                        <asp:GridView ID="gvCategoria" class="table table-striped table-bordered table-hover "
                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvCategoria_RowDataBound" OnRowDeleting="gvCategoria_RowDeleting">
                            <Columns>

                                <asp:BoundField DataField="idCategoria" HeaderText="ID">
                                    <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:BoundField DataField="sDscCategoria" HeaderText="Descrição do Fluxo">
                                    <ItemStyle Width="94%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:TemplateField HeaderText="Excluir">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete" ClientIDMode="Static"><i class="fa fa-eraser"></i></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>

                            </Columns>
                        </asp:GridView>

                    </div>
                </div>
            </div>

            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" id="field_cancel" title="Voltar" onclick="history.go(-1)">
            </fieldset>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
