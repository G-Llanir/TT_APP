<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Financeiro_Cliente_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Financeiro.Financeiro_Cliente_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Pesquisa_Parceiros.ascx" TagPrefix="uc1" TagName="Pesquisa_Parceiros" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <uc1:Manual runat="server" ID="manual" />
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>



    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Financeiro"></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

        </div>
    </div>

    <div>
        <ul id="tab_Financeiro" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="tabpanel active">
                <a href="#Financeiro" id="aba_Financeiro" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Financeiro</b></a>
            </li>
        </ul>
    </div>


    <div id="tab" class="tab-content">

        <div role="tabpanel" class="tab-pane fade in active" id="Financeiro" aria-labelledby="Financeiro-tab">
            <br />

            <div class="panel panel-default" runat="server" id="div_Financeiro">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Detalhe</b></h3>
                </div>
                <div class="panel-body">
                    <div class="form-stacked row">
                        <div class="col-lg-12">
                            <div class="form-stacled row">
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>ID</label>
                                        <asp:TextBox ID="txtidContasReceber" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2" style="display: ruby" <%-- * --%>>
                                    <div class="form-group">
                                        <h3>
                                            <span class="status" title="Status">
                                                <asp:Label ID="lblsDscTipoStatus" runat="server" TabIndex="100"></asp:Label>
                                            </span>
                                        </h3>
                                    </div>

                                </div>
                            </div>
                        </div>

                        <uc1:Pesquisa_Parceiros runat="server" ID="Pesquisa_Parceiros" SsTipoParceiro="0;" />

                        <div class="col-lg-2">
                            <div class="form-group">
                                <label>Data de Emissão</label>
                                <asp:TextBox ID="txtdtEmissao" class="form-control" runat="server" type="date"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-2" runat="server" id="Div_dtVencimento">
                            <div class="form-group">
                                <label>Previsão de Recebimento</label>
                                <asp:TextBox ID="txtdtVencimento" class="form-control" runat="server" type="date"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-2" runat="server" id="Div_VencimentoOriginal">
                            <div class="form-group">
                                <label>Vencimento Original</label>
                                <asp:TextBox ID="txtdtVencimentoOriginal" class="form-control" runat="server" Style="z-index: 0;" type="date"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-2" runat="server" id="Div_sQuantidadeParcela">
                            <div class="form-group">
                                <label>Quantidade de parcelas</label>
                                <asp:TextBox ID="txtsQuantidadeParcela" class="form-control " AutoPostBack="true" runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <label>Número do Pedido</label>
                                <asp:TextBox ID="txtsDocumento" class="form-control " runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-2" runat="server" id="Div_nValorOriginal">
                            <div class="form-group">
                                <label>Valor Líquido</label>
                                <div class="input-group">
                                    <span class="input-group-addon">R$</span>
                                    <asp:TextBox ID="txtnValorOriginal" class="form-control " Style="z-index: 0;" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-2" runat="server" id="DivTotal">
                            <div class="form-group">
                                <label>Total recebido</label>
                                <div class="input-group">
                                    <span class="input-group-addon">R$</span>
                                    <asp:TextBox ID="txtnTotal" class="form-control " Style="z-index: 0;" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-2" runat="server" id="Div_Saldo">
                            <div class="form-group">

                                <label>Saldo a Receber</label>
                                <div class="input-group">
                                    <span class="input-group-addon">R$</span>
                                    <asp:TextBox ID="txtnSaldo" class="form-control " Style="z-index: 0;" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <label>Nota Fiscal</label>
                                <asp:TextBox ID="txtsCodigo" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>



                        <div class="col-lg-12">
                            <div class="form-group">
                                <label>Observações Gerais</label>
                                <asp:TextBox ID="txtsObservacaoGeral" class="form-control" runat="server" TextMode="MultiLine" Height="120px"></asp:TextBox>
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <uc1:MensagemPagina runat="server" ID="MensagemPagina_Recebimento" />

            <div class="panel panel-default" runat="server" id="Div_Recebimento">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Informações sobre Recebimento</b></h3>
                </div>
                <div class="panel-body">
                    <div class="form-group row">
                        <div class="col-lg-12">
                            <uc1:MensagemPagina runat="server" ID="MensagemLancamento" />
                        </div>

                        <div class="col-lg-12">
                            <asp:GridView ID="dtgRecebimento" class="table table-striped table-bordered table-hover table-condensed"
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small"
                                OnRowDataBound="dtgRecebimento_RowDataBound">
                                <Columns>

                                    <asp:BoundField DataField="idlinha" HeaderText="idLinha">
                                        <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="nNumeroParcela_Info_Rec" HeaderText="Nº Recebimento">
                                        <ItemStyle Width="4%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="dtRecebimento_Info_Rec" HeaderText="Data do Recebimento">
                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="nValorRecebimento_Info_Rec" HeaderText="Valor Recebido" DataFormatString="{0:C}" HtmlEncode="False">
                                        <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="nMulta" HeaderText="Valor Multa" DataFormatString="{0:C}" HtmlEncode="False">
                                        <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="nJuros" HeaderText="Valor Juros" DataFormatString="{0:C}" HtmlEncode="False">
                                        <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="nDesconto" HeaderText="Valor Desconto" DataFormatString="{0:C}" HtmlEncode="False">
                                        <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="nValorTotalRec" HeaderText="Valor Total" DataFormatString="{0:C}" HtmlEncode="False">
                                        <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sDscFormaRecebimento_Info_Rec" HeaderText="Forma de Recebimento">
                                        <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>


    <fieldset class="form-stacked actions">
        <a href="javascript:history.back()" class="btn btn-lg btn-warning" title="Voltar">Voltar</a>
    </fieldset>

    <script type="text/javascript">
        function openModal() {
            $('#observacaoModal').modal('show');
        }
    </script>

    <asp:HiddenField ID="hddidContasReceber" runat="server" />
    <asp:HiddenField ID="hddidEmpresa" runat="server" />
</asp:Content>
