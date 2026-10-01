<%@ Page Title="Detalhe do Plano SSTT" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="PlanosSSTT_Detalhe.aspx.cs" Inherits="TT_Hub.App.Paginas.RRHH.PlanosSSTT_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>
            </div>

            <div>
                <ul id="tab_Planos" class="nav nav-tabs" role="tablist">
                    <li role="presentation" class="active">
                        <a href="#Planos" role="tab" data-toggle="tab" aria-controls="Planos" aria-expanded="true"><b>Planos SSTT</b></a>
                    </li>
                    <li role="presentation">
                        <a href="#Logs" role="tab" data-toggle="tab" aria-controls="Logs" aria-expanded="false"><b>Logs</b></a>
                    </li>
                </ul>
            </div>
            <br />

            <div id="DIV_Body" runat="server">

                <div id="tab" class="tab-content">

                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                    <div role="tabpanel" class="tab-pane fade in active" id="Planos" aria-labelledby="Planos-tab">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h3 class="panel-title"><i class="fa fa-pencil-square-o"></i>Dados do Plano</h3>
                            </div>
                            <div class="panel-body">
                                <div class="form-stacked">
                                    <div class="row">
                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>ID</label>
                                                <asp:TextBox ID="txtidPlano" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-lg-5">
                                            <div class="form-group">
                                                <label>Descrição do Plano</label>
                                                <asp:TextBox ID="txtsDscPlano" runat="server" CssClass="form-control" MaxLength="200"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>Valor Mensal (R$)</label>
                                                <asp:TextBox ID="txtnValor" runat="server" CssClass="form-control" placeholder="0,00"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>


                                    <div class="row">
                                        <div class="col-lg-7">
                                            <div class="panel panel-default">
                                                <div class="panel-heading">
                                                    <h3 class="panel-title"></i>Coberturas do Plano</h3>
                                                </div>
                                                <div class="panel-body">
                                                    <%-- ALTERAÇÃO: Adicionado painel para controlar visibilidade da adição de coberturas --%>
                                                    <asp:Panel ID="pnlAdicionarCobertura" runat="server">
                                                        <div class="row">
                                                            <div class="col-lg-6">
                                                                <div class="form-group">
                                                                    <label>Nova Cobertura</label>
                                                                    <asp:TextBox ID="txtNovaCobertura" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-2">
                                                                <div class="form-group">
                                                                    <label>&nbsp;</label>
                                                                    <asp:Button ID="btnAdicionarCobertura" runat="server" Text="incluir" CssClass="btn btn-block btn-sm btn-primary" OnClick="btnAdicionarCobertura_Click" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </asp:Panel>
                                                    <div class="table-responsive">
                                                        <asp:GridView ID="gvCoberturas" runat="server"
                                                            AutoGenerateColumns="False"
                                                            CssClass="table table-striped table-bordered table-hover"
                                                            OnRowDeleting="gvCoberturas_RowDeleting"
                                                            ShowHeaderWhenEmpty="true" GridLines="None">
                                                            <Columns>
                                                                <asp:BoundField DataField="Descricao" HeaderText="Cobertura" />

                                                                <asp:TemplateField HeaderText="Ação" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="btnRemoverCobertura" runat="server" CommandName="Delete" CssClass="btn btn-xs btn-danger"><i class="fa fa-trash-o"></i></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                <div class="alert alert-warning text-center">Nenhuma cobertura adicionada a este plano.</div>
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-lg-12">
                                            <div class="form-group">
                                                <label>Observações</label>
                                                <asp:TextBox ID="txtsObservacao" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>

                    <div role="tabpanel" class="tab-pane fade" id="Logs" aria-labelledby="Logs-tab">

                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h3 class="panel-title"></i>Logs e Alterações</h3>
                            </div>
                            <div class="panel-body">
                                <uc1:MensagemPagina runat="server" ID="MensagemPaginaLOGS" />

                                <asp:GridView ID="gvLogs" class="table table-striped table-bordered table-hover"
                                    runat="server" Width="100%" CellPadding="1" AutoGenerateColumns="true" GridLines="None"
                                    Font-Size="Small">

                                    <EmptyDataTemplate>
                                        <div class="alert alert-warning text-center">Nenhum Log Registrado.</div>
                                    </EmptyDataTemplate>

                                </asp:GridView>
                            </div>
                        </div>

                    </div>

                    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />


                    <fieldset class="col-lg-12 form-stacked actions">
                        <asp:Button ID="cmdSalvar" runat="server" class="btn btn-lg btn-success" Text="Salvar" />
                        <asp:Button ID="cmdExcluir" class="btn btn-lg btn-danger" runat="server" Text="Excluir" />
                        <a href="PlanosSSTT.aspx" class="btn btn-lg btn-warning">Voltar</a>
                    </fieldset>
                </div>

            </div>
            <asp:HiddenField ID="hddidPlano" runat="server" Value="0" />
        </ContentTemplate>
    </asp:UpdatePanel>

    <div id="dialog-Salvar" class="modal" title="Salvar Plano">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span><b>
                <asp:Label ID="lblTituloSalvar" runat="server"></asp:Label></b>
        </p>
    </div>
    <div id="dialog-Excluir" class="modal" title="Excluir Plano">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span><b>
                <asp:Label ID="lblTitulosExcluir" runat="server"></asp:Label></b>
        </p>
    </div>
</asp:Content>
