<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Moedas.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Manutencao.Moedas" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>

<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>




<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>


    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Moedas"></asp:Label></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="2" TitulodaPagina="Detalhe" />
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>
    </div>

    <div>
        <ul id="tab_Moeda" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="tabpanel active">
                <a href="#moeda" id="aba_Moeda" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Moedas</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_Historico">
                <a href="#historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="Historioco"><b>Histórico</b></a>
            </li>
        </ul>
    </div>

    <div id="tab" class="tab-content">

        <div role="tabpanel" class="tab-pane fade in active" id="moeda" aria-labelledby="moeda-tab">
            <br />


            <div class="panel panel-default" runat="server" id="div_Moeda">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Detalhes</b></h3>
                </div>
                <div class="panel-body table-responsive">

                    <asp:UpdatePanel ID="updDepartamentos" runat="server">
                        <ContentTemplate>
                            <uc1:MensagemPagina runat="server" ID="MensagemAcoes" />
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <div class="row" runat="server" id="Div_Selecao">

                                            <div class="col-lg-4">
                                                <div class="form-group">
                                                    <label>Moeda </label>
                                                    <asp:DropDownList ID="ddlMoeda_idTipoMoeda" runat="server" class="form-control Caixa_Selecao" ValidationGroup="Inclusao"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <%--<div class="col-lg-4">
                                                        <div class="form-group">
                                                            <label>País </label>
                                                            <asp:DropDownList ID="ddlMoeda_idPais" runat="server" class="form-control" ValidationGroup="Inclusao"></asp:DropDownList>
                                                        </div>
                                                    </div>--%>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Valor </label>
                                                    <asp:TextBox ID="txtMoeda_nValorCambio" runat="server" class=" form-control" ValidationGroup="Inclusao">  </asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <label>&nbsp;</label>
                                                <br />
                                                <asp:Button ID="cmdMoeda_Incluir" class="btn  btn-sm btn-success" runat="server" Text="Incluir" OnClick="cmdMoeda_Incluir_Click" ValidationGroup="Inclusao" />
                                            </div>

                                        </div>

                                        <div class="row table-responsive">
                                            <div class="col-lg-12">
                                                <asp:GridView ID="dtgMoeda" class="table table-striped table-bordered table-hover table-condensed"
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDeleting="dtgMoeda_RowDeleting" OnRowDataBound="dtgMoeda_RowDataBound">
                                                    <Columns>

                                                        <asp:BoundField DataField="idMoeda" HeaderText="idMoeda">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="idTipoMoeda" HeaderText="idTipoMoeda">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sDscTipoMoeda" HeaderText="Moeda">
                                                            <ItemStyle Width="16%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <%--<asp:BoundField DataField="sDscPais" HeaderText="País">
                                                                    <ItemStyle Width="16%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>--%>

                                                        <asp:TemplateField HeaderText="Valor Câmbio" runat="server">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="txtnValorCambio" class="form-control " Text='<%# Bind("nValorCambio","{0:N2}") %>' runat="server" ValidationGroup="Salvar"></asp:TextBox>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                        <asp:BoundField DataField="UltimonValorCambio" HeaderText="Último Valor" >
                                                            <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="UltimodtAtualizacao" HeaderText="Última Atualização">
                                                            <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkMoeda_Excluir" runat="server" CssClass="btn btn-small" TabIndex="50" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                    </Columns>
                                                    <PagerStyle BackColor="Black" ForeColor="Black" HorizontalAlign="Center" />
                                                    <EditRowStyle BackColor="#2461BF" />
                                                </asp:GridView>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                            </div>

                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cmdMoeda_Incluir" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>

            </div>

            <div role="tabpanel" class="tab-pane fade" id="historico" aria-labelledby="historico-tab">
                <br />
                <div class="panel panel-default" runat="server" id="DIV_historico">
                    <div class="panel-heading">

                        <h3 class="panel-title"><b>Histórico</b></h3>
                    </div>

                    <div class="panel-body">
                        <div class="form-stacked row">
                            <div class="col-lg-12 table-responsive">

                                <asp:GridView
                                    ID="gv_Historico" class="table table-striped table-bordered table-hover table-condensed "
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False">
                                    <Columns>
                                        <asp:BoundField DataField="sDscTipoMoeda" HeaderText="Moeda">
                                            <ItemStyle Width="35%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <%--<asp:BoundField DataField="sDscPais" HeaderText="País">
                                        <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>--%>
                                        <asp:BoundField DataField="nValorCambio" HeaderText="Vlr Câmbio" DataFormatString="{0:N2}">
                                            <ItemStyle Width="10%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="dtAtualizacao" HeaderText="Data de Atualização">
                                            <ItemStyle Width="15%" HorizontalAlign="center" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                            <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                    </Columns>
                                </asp:GridView>

                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>

        <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
        <br />
    </div>
    <fieldset class="form-stacked actions">
        <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-info" runat="server" Text="Salvar" ValidationGroup="Salvar" />
        <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field-cancel" title="Voltar" onclick="history.go(-2)">
    </fieldset>

    <div id="dialog-Salvar" class="modal" title="Salvar">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
        </p>
    </div>

    <asp:HiddenField ID="hddidMoeda" runat="server" />
</asp:Content>
