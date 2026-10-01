<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="CondicaodePagamento_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.CondicaodePagamento_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.16/jquery.mask.min.js"></script>
    <script>
        $(document).ready(function () {
            $('[id*=txtnPorcentagemValor]').mask('000,00%', { reverse: true });
        });
    </script>

    <asp:UpdatePanel ID="updDetalhe" runat="server">
        <ContentTemplate>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>
            </div>

            <div class="form-stacked row">

                <div class="row col-lg-12">
                    <div class="col-lg-12">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    </div>

                    <div class="col-lg-1 form-group">
                        <label>ID</label>
                        <asp:TextBox ID="txtidCondicaoPagamento" class="form-control CaixaTextoMini" runat="server" disabled=""></asp:TextBox>
                    </div>

                    <div id="div_idPedido" runat="server" class="col-lg-2 form-group">
                        <label>Vinculada ao Pedido/Orçamento</label>
                        <div class="input-group">
                            <asp:TextBox ID="txtidPedido" class="form-control" runat="server" disabled=""></asp:TextBox>
                            <span class="input-group-btn">
                                <a runat="server" id="cmd_idPedido" class="btn btn-info" href="" target="_blank">Pedido</a>
                            </span>
                        </div>
                    </div>
                </div>

                <div class="col-lg-8 form-group">
                    <label>Descrição </label>
                    <asp:TextBox ID="txtsDscCondicaoPagamento" class="form-control" runat="server" MaxLength="50"></asp:TextBox>
                </div>

                <div class="col-lg-3 form-group">
                    <label>Quantidade de Parcelas </label>
                    <asp:TextBox ID="txtnQtdParcelas" class="form-control" runat="server"></asp:TextBox>
                </div>

                <div class="col-lg-12">
                    <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
                </div>

            </div>

            <div class="panel panel-default" runat="server" id="DIV_CP">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Configurações</b></h3>
                </div>
                <div class="panel-body">
                    <div class="form-group row">

                        <div>
                            <br />
                            <asp:HiddenField ID="hddCondicaoPagamento_idLinha" runat="server" />
                        </div>

                        <div class="col-lg-12" runat="server" id="DIV_GRID">
                            <asp:GridView ID="dtgCondicaoPagamento" class="table table-striped table-bordered table-hover table-condensed"
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="idRegistroCondicaoPagamento" Font-Names="Tahoma" Font-Overline="False" Font-Size="11px"
                                ShowFooter="False" OnRowDataBound="dtgCondicaoPagamento_RowDataBound" OnRowDeleting="dtgCondicaoPagamento_RowDeleting" OnRowCommand="dtgCondicaoPagamento_RowCommand">
                                <Columns>

                                    <asp:BoundField DataField="idLinha" HeaderText="idLinha">
                                        <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="idRegistroCondicaoPagamento" HeaderText="idRegistroCondicaoPagamento">
                                        <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:TemplateField HeaderText="Tipo pagamento">
                                        <ItemTemplate>
                                            <asp:DropDownList ID="ddlidTipoCondicaoPagamento" runat="server" class="form-control yes_no select " AutoPostBack="true" onselectedindexchange="ddlidTipoCondicaoPagamento_SelectedIndexChanged"></asp:DropDownList>
                                        </ItemTemplate>
                                        <ItemStyle Width="20%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Porcentagem" runat="server">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtnPorcentagemValor" class="form-control " Text='<%# Bind("nPorcentagemValor","{0:N2}") %>' runat="server" ValidationGroup="Salvar"></asp:TextBox>
                                        </ItemTemplate>
                                        <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="DDL" runat="server">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtnDDL" class="form-control " Text='<%# Bind("nDDL") %>' runat="server" AutoPostBack="true" ValidationGroup="Salvar" OnTextChanged="txtnDDL_TextChanged"></asp:TextBox>
                                        </ItemTemplate>
                                        <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:TemplateField>

                                </Columns>
                            </asp:GridView>
                        </div>

                    </div>

                </div>
            </div>

            <br />
            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click1" />
                &nbsp;
                <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" id="field-cancel" title="Voltar" onclick="history.go(-1)">&nbsp;
            </fieldset>

            <asp:HiddenField ID="hddIdLinha" runat="server" />
            <asp:HiddenField ID="hddidCondicaoPagamento" runat="server" />
            <asp:HiddenField ID="hddnQtdParcelas" runat="server" />

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
