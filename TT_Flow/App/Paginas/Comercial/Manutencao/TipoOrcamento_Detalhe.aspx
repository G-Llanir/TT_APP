<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="TipoOrcamento_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Comercial.Manutencao.TipoOrcamento_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>
<%@ Register Src="~/App/Controles/FiltroPesquisa.ascx" TagPrefix="uc1" TagName="FiltroPesquisa" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <asp:UpdatePanel ID="UpdDetalhe" runat="server">
        <ContentTemplate>

            <uc1:Manual runat="server" ID="manual" />

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
                <asp:HiddenField runat="server" ID="hddidTipoOrcamento" />
            </div>

            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Tipo de Orçamento</b></h3>
                </div>
                <div class="panel-body">
                    <div class="row">

                        <div class="col-lg-12">
                            <div class="col-lg-2 row">
                                <div class="form-group">
                                    <label>ID</label>
                                    <asp:TextBox runat="server" ID="txtidTipoOrcamento" class="form-control CaixaTextoMini" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <label>Descrição</label>
                                <asp:TextBox runat="server" ID="txtsDscTipoOrcamento" class="form-control"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-3">
                            <div class="form-group">
                                <uc1:SwitchAtivo runat="server" ID="SwitchCalcularInstalacao" />
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
                    <h3 class="panel-title"><b>Fluxos</b></h3>
                </div>
                <div class="panel-body">
                    <div runat="server" id="div_IncluirFluxo" class="col-lg-12 row">

                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_IncluirFluxo" />

                        <div class="col-lg-2">
                            <div class="form-group">
                                <label>Fluxo</label>
                                <asp:DropDownList runat="server" ID="ddlFluxo" class="form-control"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <div class="col-lg-3">
                                    <label>&nbsp;</label>
                                    <asp:Button runat="server" ID="cmdIncluirFluxo" class="btn btn-info form-control" Text="Incluir" OnClick="cmdIncluirFluxo_Click"></asp:Button>
                                </div>
                            </div>
                        </div>

                    </div>

                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_gvFluxo" />

                    <div runat="server" id="div_gvFluxo" class="col-lg-12">

                        <asp:GridView ID="gvFluxo" class="table table-striped table-bordered table-hover "
                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvFluxo_RowDataBound" OnRowDeleting="gvFluxo_RowDeleting">
                            <Columns>

                                <asp:BoundField DataField="idFluxo" HeaderText="ID">
                                    <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:BoundField DataField="sDscFluxo" HeaderText="Descrição do Fluxo">
                                    <ItemStyle Width="90%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:TemplateField HeaderText="Excluir">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete" ClientIDMode="Static"><i class="fa fa-eraser"></i></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>

                            </Columns>
                        </asp:GridView>

                    </div>
                </div>
            </div>

            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Serviços</b></h3>
                </div>
                <div class="panel-body">

                    <div runat="server" id="div_IncluirServico_Recurso" class="col-lg-12 row">

                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_IncluirServico_Recurso" />

                        <div class="col-lg-3">
                            <div class="form-group">
                                <label>Tipo de Serviços</label>
                                <asp:DropDownList runat="server" ID="ddlTipoServico" class="form-control"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-2">
                            <div class="form-group">
                                <div class="col-lg-6">
                                    <label>&nbsp;</label>
                                    <asp:Button runat="server" ID="cmdIncluirTipoServico" class="btn btn-info form-control" Text="Incluir" OnClick="cmdIncluirTipoServico_Click"></asp:Button>
                                </div>
                            </div>
                        </div>

                    </div>

                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_gvServico_Recurso" />

                    <div runat="server" id="div_gvTiposServicos" class="col-lg-12">

                        <asp:GridView ID="gvTiposServicos" class="table table-striped table-bordered table-hover "
                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvTiposServicos_RowDataBound" OnRowDeleting="gvTiposServicos_RowDeleting">
                            <Columns>

                                <asp:BoundField DataField="idTipoServico_Recurso" HeaderText="ID">
                                    <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:BoundField DataField="sDscTipoServico_Recurso" HeaderText="Descrição do Tipo de Serviço">
                                    <ItemStyle Width="90%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:TemplateField HeaderText="Excluir">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete" ClientIDMode="Static"><i class="fa fa-eraser"></i></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>

                            </Columns>
                        </asp:GridView>

                    </div>
                </div>
            </div>

            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Escopos</b></h3>
                </div>
                <div class="panel-body">
                    <div runat="server" id="div_IncluirEscopos" class="col-lg-12 row">

                        <uc1:MensagemPagina runat="server" ID="MensagemIncluirEscopos" />

                        <div class="col-lg-2">
                            <div class="form-group">
                                <label>Escopos</label>
                                <asp:DropDownList runat="server" ID="ddlEscopo" class="form-control"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <div class="col-lg-3">
                                    <label>&nbsp;</label>
                                    <asp:Button runat="server" ID="cmdIncluirEscopo" class="btn btn-info form-control" Text="Incluir" OnClick="cmdIncluirEscopo_Click"></asp:Button>
                                </div>
                            </div>
                        </div>

                    </div>

                    <uc1:MensagemPagina runat="server" ID="MensagemEscopo" />

                    <div runat="server" id="div_gvEscopo" class="col-lg-12">

                        <asp:GridView ID="gvEscopo" class="table table-striped table-bordered table-hover "
                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvEscopo_RowDataBound" OnRowDeleting="gvEscopo_RowDeleting">
                            <Columns>

                                <asp:BoundField DataField="idEscopo" HeaderText="ID">
                                    <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:BoundField DataField="sDscEscopo" HeaderText="Descrição do Escopo">
                                    <ItemStyle Width="90%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:TemplateField HeaderText="Excluir">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete" ClientIDMode="Static"><i class="fa fa-eraser"></i></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
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
