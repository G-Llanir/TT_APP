<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddLoteSerie.ascx.cs" Inherits="TT_Flow.App.Controles.AddLoteSerie" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<style>
    .bg-tt {
        background-color: #009a22 !important;
    }
</style>

<div id="divPrincipal" runat="server" class="col-lg-12">
    <div class="panel panel-default" runat="server">
        <div class="panel-heading bg-tt text-white">
            <h3 class="panel-title"><b style="color: white">Produto: </b>
                <asp:Label Style="color: white" ID="lblTituloProduto" Text="" runat="server" /></h3>
        </div>
        <div class="panel-body">
            <div class="col-lg-12">
                <uc1:MensagemPagina runat="server" ID="MensagemPaginaControle" />
            </div>
            <div id="div_Controle" runat="server" class="row">
                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaSeries" />
                </div>
                <div class="col-lg-12">
                    <div class="col-lg-4">
                        <div class="form-group">
                            <label for="ddlLocal">Armazenamento:</label>
                            <asp:DropDownList ID="ddlLocal" runat="server" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlLocal_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-lg-4">
                        <div class="form-group">
                            <label>Posição:</label>
                            <asp:DropDownList ID="ddlPosicaoPai" AutoPostBack="false" class="form-control yes_no select Caixa_Selecao" runat="server"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-lg-2">
                        <div class="form-group">
                            <label for="txtsLote">Lote:</label>
                            <asp:TextBox ID="txtsLoteEtiquetas" class="form-control " runat="server" placeholder="Lote"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-lg-2">
                        <div class="form-group">
                            <label for="txtnSerie">Série:</label>
                            <asp:TextBox ID="txtnSerie" class="form-control " runat="server" placeholder="Nº de Série"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-lg-2">
                        <br />
                        <asp:LinkButton ID="cmdIncluirSerie" CssClass="btn btn-primary btn-sm" runat="server" OnClick="IncluirSerie_Click"><b>Incluir</b></asp:LinkButton>
                        <asp:LinkButton ID="cmdSalvarSeries" CssClass="btn btn-success btn-sm" Text="Salvar" runat="server" Visible="false" OnClick="SalvarSeries_Click" />
                    </div>
                </div>
                <br />
                <div id="div_gvSerie" runat="server" class="col-lg-12" style="margin-top: 10px;">
                    <asp:GridView ID="dtgSerie" runat="server" class="table table-striped table-bordered table-hover table-condensed"
                        Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                        ShowFooter="False" DataKeyNames="idRegistro" OnRowDataBound="dtgSerie_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="SdscProduto" HeaderText="Produto" />

                            <asp:TemplateField HeaderText="Local">
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlArmazenamento" runat="server" class="form-control yes_no select Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlLocal_Salvos_SelectedIndexChanged"></asp:DropDownList>
                                </ItemTemplate>
                                <HeaderStyle Width="15%" />
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Posição">
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlPosicaoPai" AutoPostBack="false" class="form-control yes_no select Caixa_Selecao" runat="server"></asp:DropDownList>
                                </ItemTemplate>
                                <HeaderStyle Width="15%" />
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Lote">
                                <ItemTemplate>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="txtsLote"
                                        Text='<%#Eval("SLote") %>'
                                        Style="width: 100px; padding: 0; font-size: 12px;" placeholder="Lote">
                                    </asp:TextBox>
                                </ItemTemplate>
                                <HeaderStyle Width="15%" />
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="NºSérie">
                                <ItemTemplate>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="txtNSerie"
                                        Text='<%#Eval("NSerie") %>'
                                        Style="width: 100px; padding: 0; font-size: 12px;" placeholder="NºSérie">
                                    </asp:TextBox>
                                </ItemTemplate>
                                <HeaderStyle Width="15%" />
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Ação">
                                <ItemTemplate>
                                    <asp:LinkButton ID="cmdExcluirSerie" CssClass="btn btn-sm btn-danger" runat="server" OnClick="ExcluirSerie_Click" CommandArgument='<%# Eval("IdEtiquetaSerie") %>'><i class="fa fa-trash-o"></i></asp:LinkButton>

                                    <asp:HiddenField ID="hddidRegistro" runat="server" Value='<%# Eval("idRegistro") %>' />
                                    <asp:HiddenField ID="hddidProdutoSerie" runat="server" Value='<%# Eval("idProduto") %>' />
                                    <asp:HiddenField ID="hddidEtiqueta" runat="server" Value='<%# Eval("idEtiqueta") %>' />
                                    <asp:HiddenField ID="hddsProdutoSerie" runat="server" Value='<%# Eval("sDscProduto") %>' />
                                    <asp:HiddenField ID="hddsCodigo" runat="server" Value='<%# Eval("sCodigo") %>' />
                                    <asp:HiddenField ID="hddsCodigoBarras" runat="server" Value='<%# Eval("sCodigoBarras") %>' />
                                    <asp:HiddenField ID="hddsExclusao" runat="server" Value='<%# Eval("sExclusao") %>' />
                                    <asp:HiddenField ID="hddidMovimentacao" runat="server" Value='<%# Eval("idMovimentacao") %>' />
                                    <asp:HiddenField ID="hddidObjeto" runat="server" Value='<%# Eval("idObjeto") %>' />
                                    <asp:HiddenField ID="hddsTipoObjeto" runat="server" Value='<%# Eval("sTipoObjeto") %>' />
                                    <asp:HiddenField ID="hddsTipoMov" runat="server" Value='<%# Eval("sTipoMov") %>' />
                                </ItemTemplate>
                                <HeaderStyle Width="10%" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
        <asp:HiddenField ID="hddidProdutoSerie" runat="server" />
        <asp:HiddenField ID="hddnQtdSerie" runat="server" />
        <asp:HiddenField ID="hddsProdutoSerie" runat="server" />
    </div>
</div>

