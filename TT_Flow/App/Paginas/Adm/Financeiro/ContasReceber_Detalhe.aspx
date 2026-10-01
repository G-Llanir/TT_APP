<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="ContasReceber_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Financeiro.ContasReceber_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Pesquisa_Parceiros.ascx" TagPrefix="uc1" TagName="Pesquisa_Parceiros" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <uc1:Manual runat="server" ID="manual" />
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>



    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Conta a Receber"></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

        </div>
    </div>

    <div>
        <ul id="tab_ContasReceber" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="tabpanel active">
                <a href="#ContasReceber" id="aba_ContasReceber" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Conta a Receber</b></a>
            </li>

            <li role="presentation" runat="server" id="aba_Arquivo">
                <a href="#arquivo" role="tab" id="aba_Arquivo-tab" data-toggle="tab" aria-controls="arquivo"><b>Arquivos</b></a>
            </li>

            <li role="presentation" runat="server" id="Aba_Log">
                <a href="#log" role="tab" id="log-tab" data-toggle="tab" aria-controls="Log"><b>Histórico</b></a>
            </li>

            <li role="presentation" runat="server" id="aba_Historico">
                <a href="#historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="Historico"><b>Histórico</b></a>
            </li>
        </ul>
    </div>


    <div id="tab" class="tab-content">

        <div role="tabpanel" class="tab-pane fade in active" id="ContasReceber" aria-labelledby="ContasReceber-tab">
            <br />
            <asp:UpdatePanel ID="updDetalhe" runat="server">
                <ContentTemplate>
                    <div class="panel panel-default" runat="server" id="div_ContasReceber">
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

                                        <div class="col-lg-4" style="display: flex; margin-top: 5px;" <%--Agnes Partal * 01/07/2024--%>>
                                            <div class="form-group">

                                                <div class="col-lg-8">
                                                    <h3>
                                                        <span class="status" title="Status">
                                                            <asp:Label ID="lblsDscTipoStatus" runat="server" TabIndex="100"></asp:Label>
                                                        </span>
                                                    </h3>
                                                </div>

                                                <div class="col-lg-4" runat="server" id="div_modalAlteraStatus" visible="false">
                                                    <asp:Button ID="btnAbrirModalStatus" class="btn btn-info" Style="margin-top: 18px" runat="server" Text="Alterar Status" OnClick="btnAbrirModalStatus_Click" />
                                                </div>
                                            </div>

                                            <%--Agnes Partal * 01/07/2024----------------------------------------------------------------%>
                                            <div class="form-group" runat="server" id="div_sStatusConciliado" visible="false" style="margin-left: 5px;">
                                                <h3>
                                                    <span class="status" title="StatusConciliado">
                                                        <asp:Label ID="lblsStatusConciliado" runat="server" TabIndex="150"></asp:Label>
                                                    </span>
                                                </h3>
                                            </div>
                                            <%--------------------------------------------------------------------------------------------%>
                                        </div>

                                        <div class="col-lg-4" runat="server" id="Div_sStatusAdiantado">
                                            <div class="form-group">
                                                <h3>
                                                    <span class="status" title="StatusAdiantado">
                                                        <asp:Label ID="lblsStatusAdiantado" runat="server" TabIndex="150"></asp:Label>
                                                    </span>
                                                </h3>
                                            </div>
                                        </div>


                                    </div>
                                </div>



                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Data de Emissão</label>
                                        <asp:TextBox ID="txtdtEmissao" class="form-control" runat="server" type="date"></asp:TextBox>
                                    </div>
                                </div>

                                <%--<div class="col-lg-2" runat="server" id="DIV_cmdFaturamento">
                                    <div class="form-group">
                                        <label>&nbsp;</label>
                                        <asp:Button ID="cmdFaturamento" runat="server" Text="Faturamento" OnClick="cmdFaturamento_Click" CssClass="btn btn-sm btn-info" />
                                    </div>
                                </div>--%>

                                <div class="form-group">
                                    <div class="col-lg-12">
                                        <div class="row">
                                            <uc1:Pesquisa_Parceiros runat="server" ID="Pesquisa_Parceiros" SsTipoParceiro="0;" />

                                            <div class="col-lg-5">
                                                <div class="form-group">
                                                    <label>Empresa</label>
                                                    <asp:DropDownList ID="ddlidEmpresa" class="form-control  Caixa_Selecao" runat="server" attrname="idContasReceber" AutoPostBack="true" OnSelectedIndexChanged="ddlidCentroDeCusto_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>


                                <div class="form-group">
                                    <div class="col-lg-12">
                                        <div class="row">

                                            <div class="col-lg-2" runat="server" id="Div_dtVencimento">
                                                <div class="form-group">
                                                    <label>Previsão de Recebimento</label>
                                                    <asp:TextBox ID="txtdtVencimento" class="form-control" runat="server" type="date"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2" runat="server" id="Div_VencimentoOriginal">
                                                <div class="form-group">
                                                    <label>Vencimento Original</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon">R$</span>
                                                        <asp:TextBox ID="txtdtVencimentoOriginal" class="form-control" runat="server" Style="z-index: 0;" type="date"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-lg-2" runat="server" id="Div_nValorBruto">
                                                <div class="form-group">
                                                    <label>Valor Bruto</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon">R$</span>
                                                        <asp:TextBox ID="txtnValorBruto" class="form-control" Style="z-index: 0;" runat="server" AutoPostBack="true" OnTextChanged="txtnValorBruto_TextChanged"></asp:TextBox>
                                                    </div>
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

                                            <div class="col-lg-2" runat="server" id="Div_Saldo">
                                                <div class="form-group">

                                                    <label>Saldo a Receber</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon">R$</span>
                                                        <asp:TextBox ID="txtnSaldo" class="form-control " Style="z-index: 0;" runat="server"></asp:TextBox>
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

                                            <div class="col-lg-4">
                                                <div class="form-group">
                                                    <label>Nota Fiscal</label>
                                                    <asp:TextBox ID="txtsCodigo" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-4">
                                                <div class="form-group">
                                                    <label>Número do Pedido</label>
                                                    <asp:TextBox ID="txtsDocumento" class="form-control " runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-4">
                                                <div class="form-group">
                                                    <label>Categoria</label>
                                                    <asp:DropDownList ID="ddlidCategoriaReceber" class="form-control  Caixa_Selecao Caixa_Selecao" runat="server" attrname="idContasReceber" AutoPostBack="true" OnTextChanged="ddlidCategoriaReceber_TextChanged"></asp:DropDownList>
                                                </div>
                                            </div>


                                            <div id="div_linhaFormaRecebimento" runat="server">
                                                <div id="div_linhaFormaRecebimento2" runat="server">
                                                    <div id="div_linhaFormaRecebimento3" runat="server">
                                                        <div class="col-lg-4" runat="server" id="Div_idFormaRecebimento">
                                                            <div class="form-group">
                                                                <label>Forma de Recebimento </label>
                                                                <asp:DropDownList ID="ddlidFormaRecebimento" class="form-control  Caixa_Selecao" runat="server" attrname="idContasReceber" AutoPostBack="True" OnSelectedIndexChanged="ddlidFormaRecebimento_SelectedIndexChanged"></asp:DropDownList>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-4" runat="server" id="Div_idContaBancaria">
                                                            <div class="form-group">
                                                                <label>Conta Bancária </label>
                                                                <asp:DropDownList ID="ddlContaBancaria" class="form-control  Caixa_Selecao" runat="server" attrname="idContasReceber"></asp:DropDownList>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-4">
                                                            <div class="form-group">
                                                                <label>Código Contábil</label>
                                                                <asp:DropDownList ID="ddlidContabil" class="form-control Caixa_Selecao " runat="server" attrname="idContasReceber" AutoPostBack="True" OnSelectedIndexChanged="ddlidCentroDeCusto_SelectedIndexChanged"></asp:DropDownList>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-4">
                                                            <div class="form-group">
                                                                <label>Centro de custo</label>
                                                                <asp:DropDownList ID="ddlidCentroDeCusto" class="form-control  Caixa_Selecao" runat="server" attrname="idContasReceber" AutoPostBack="True" OnSelectedIndexChanged="ddlidCentroDeCusto_SelectedIndexChanged"></asp:DropDownList>
                                                            </div>
                                                        </div>

                                                    </div>
                                                </div>
                                            </div>


                                            <div id="div_linhaTipoRecebimento" runat="server">
                                                <div id="div_linhaTipoRecebimento2" runat="server">
                                                    <div id="div_linhaTipoRecebimento3" runat="server">
                                                        <div class="col-lg-4">
                                                            <div class="form-group">
                                                                <label>Tipo Recebimento </label>
                                                                <asp:DropDownList ID="ddlidMeioRecebimento" class="form-control  Caixa_Selecao" runat="server" attrname="idContasReceber" AutoPostBack="True" OnSelectedIndexChanged="ddlidMeioRecebimento_SelectedIndexChanged"></asp:DropDownList>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-4" runat="server" id="Div_nParcelas">
                                                            <div class="form-group">
                                                                <label>N° parcela</label>
                                                                <asp:TextBox ID="txtnParcelas" class="form-control " AutoPostBack="true" runat="server" OnTextChanged="txtnParcelas_TextChanged"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-4" runat="server" id="Div_sQuantidadeParcela">
                                                            <div class="form-group">
                                                                <label>Quantidade de parcelas</label>
                                                                <asp:TextBox ID="txtsQuantidadeParcela" class="form-control " AutoPostBack="true" runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
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
                                <div runat="server" id="Div_CadastroRecebimento">


                                    <div class="col-lg-12">
                                        <div class="form-stacled row">
                                            <div class="col-lg-2" runat="server" id="Div_dtRecebimento_Info_Rec">
                                                <div class="form-group">
                                                    <label>Data</label>
                                                    <asp:TextBox ID="txtdtRecebimento_Info_Rec" class="form-control" runat="server" type="date"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-3" runat="server" id="Div_idFormaRecebimento_Info_Rec">
                                                <div class="form-group">
                                                    <label>Forma de Recebimento </label>
                                                    <asp:DropDownList ID="ddlidFormaRecebimento_Info_Rec" class="form-control Caixa_Selecao " runat="server" attrname="idContasReceber" AutoPostBack="true" OnSelectedIndexChanged="ddlidFormaRecebimento_Info_Rec_SelectedIndexChanged"></asp:DropDownList>

                                                </div>
                                            </div>

                                            <div runat="server" id="div_devolucao" visible="false">
                                                <div class="col-lg-3">
                                                    <div class="form-group">
                                                        <label>Crédito Disponível</label>
                                                        <asp:DropDownList ID="ddlCreditoDevolucao" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>

                                            <div runat="server" id="div_valor_recebimento">

                                                <div class="col-lg-3" runat="server" id="Div_idConta_Info_Rec">
                                                    <div class="form-group">
                                                        <label>Conta </label>
                                                        <asp:DropDownList ID="ddlidConta_Info_Rec" class="form-control  Caixa_Selecao" runat="server" attrname="idContasReceber" AutoPostBack="true" OnSelectedIndexChanged="ddlidConta_Info_Rec_SelectedIndexChanged"></asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2" runat="server">
                                                    <div class="form-group">
                                                        <label>Valor Recebido (Título)</label>
                                                        <div class="input-group">
                                                            <span class="input-group-addon">R$</span>
                                                            <asp:TextBox ID="txtnValorRecebimento_Info_Rec" class="form-control" runat="server" AutoPostBack="true" Style="z-index: 0;" OnTextChanged="txtnValorRecebimento_Info_Rec_TextChanged"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2" runat="server" id="Div_Multa">
                                                    <div class="form-group">
                                                        <label>Valor Multa</label>
                                                        <div class="input-group">
                                                            <span class="input-group-addon">R$</span>
                                                            <asp:TextBox ID="txtnMulta" class="form-control" runat="server" AutoPostBack="true" Style="z-index: 0;" OnTextChanged="txtnValorTotalRec_TextChanged"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-lg-2" runat="server" id="Div_Juros">
                                                    <div class="form-group">
                                                        <label>Valor Juros</label>
                                                        <div class="input-group">
                                                            <span class="input-group-addon">R$</span>
                                                            <asp:TextBox ID="txtnJuros" class="form-control" runat="server" AutoPostBack="true" Style="z-index: 0;" OnTextChanged="txtnValorTotalRec_TextChanged"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2" runat="server" id="Div_Tarifa" visible="false">
                                                    <div class="form-group">
                                                        <label>Valor Tarifa</label>
                                                        <div class="input-group">
                                                            <span class="input-group-addon">R$</span>
                                                            <asp:TextBox ID="txtnTarifa" class="form-control" runat="server" AutoPostBack="true" Style="z-index: 0;" OnTextChanged="txtnValorTotalRec_TextChanged"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2" runat="server" id="Div_Desconto">
                                                    <div class="form-group">
                                                        <label>Valor Desconto</label>
                                                        <div class="input-group">
                                                            <span class="input-group-addon">R$</span>
                                                            <asp:TextBox ID="txtnDesconto" class="form-control" runat="server" AutoPostBack="true" Style="z-index: 0;" OnTextChanged="txtnValorTotalRec_TextChanged"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2" runat="server" id="Div_ValorTotal">
                                                    <div class="form-group">
                                                        <label>Total Recebido (Em Conta)</label>
                                                        <div class="input-group">
                                                            <span class="input-group-addon">R$</span>
                                                            <asp:TextBox ID="txtnValorTotalRec" class="form-control" runat="server" AutoPostBack="true" Style="z-index: 0;" OnTextChanged="txtnValorTotalRec_TextChanged"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2 form-group">
                                                    <uc1:SwitchAtivo runat="server" ID="SwitchAtivo_EmailPagoAtraso" />
                                                </div>

                                            </div>

                                            <div class="col-lg-1" runat="server" id="Div_BotaoIncluirRec">
                                                <div class="form-group">
                                                    <asp:Button ID="cmdRecebimento_Incluir" class="btn btn-info" runat="server" Text="Incluir" OnClick="cmdRecebimento_Incluir_Click" Style="margin-top: 24px" />
                                                    <asp:Button ID="BtnEdicaoRecebimento" class="btn btn-info" runat="server" Text="Salvar" OnClick="BtnEdicaoRecebimento_Click" Style="margin-top: 24px" />
                                                </div>
                                            </div>

                                        </div>
                                    </div>

                                    <asp:HiddenField ID="hddRecebimento_idLinha" runat="server" />


                                </div>

                                <div class="col-lg-12">
                                    <asp:GridView ID="dtgRecebimento" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small"
                                        OnRowDeleting="dtgRecebimento_RowDeleting" OnRowDataBound="dtgRecebimento_RowDataBound" OnRowCommand="dtgRecebimento_RowCommand">
                                        <%--
                                ========================
                                =========índice=========
                                ========================
                                N° Célula ------- Info
                                ========================
                                0 --------------- idLinha
                                1 --------------- Nº Recebimento
                                2 --------------- Data do Recebimento
                                3 --------------- Valor Recebido
                                4 --------------- Valor Multa
                                5 --------------- Valor Juros
                                6 --------------- Valor Tarifa
                                7 --------------- Valor Desconto
                                8 --------------- Valor Total
                                9 --------------- Conta
                                10 -------------- Forma de Recebimento
                                11 -------------- Botões
                                ========================
                                        --%>
                                        <Columns>

                                            <asp:BoundField DataField="idlinha" HeaderText="idLinha">
                                                <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="nNumeroParcela_Info_Rec" HeaderText="Nº Recebimento">
                                                <ItemStyle Width="4%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="dtRecebimento_Info_Rec" HeaderText="Data do Recebimento">
                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="nValorRecebimento_Info_Rec" HeaderText="Recebido (Titulo)" DataFormatString="{0:C}" HtmlEncode="False">
                                                <ItemStyle Width="5%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="nMulta" HeaderText="Multa" DataFormatString="{0:C}" HtmlEncode="False">
                                                <ItemStyle Width="5%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="nJuros" HeaderText="Juros" DataFormatString="{0:C}" HtmlEncode="False">
                                                <ItemStyle Width="5%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="nTarifa" HeaderText="Tarifas" DataFormatString="{0:C}" HtmlEncode="False">
                                                <ItemStyle Width="5%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="nDesconto" HeaderText="Desconto" DataFormatString="{0:C}" HtmlEncode="False">
                                                <ItemStyle Width="5%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="nValorTotalRec" HeaderText="Total (Em conta)" DataFormatString="{0:C}" HtmlEncode="False">
                                                <ItemStyle Width="5%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="sDscConta_Info_Rec" HeaderText="Conta">
                                                <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="sDscFormaRecebimento_Info_Rec" HeaderText="Forma de Recebimento">
                                                <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>


                                            <asp:TemplateField HeaderText="">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkRecebimento_Editar" CommandArgument='<%# Bind("idLinha") %>' runat="server" ToolTip="Editar" CssClass="btn btn-small" TabIndex="100" CommandName="Editar"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="lnkRecebimento_UpLoad" CommandArgument='<%# Bind("idLinha") %>' runat="server" ToolTip="Efetuar Upload de Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Upload_Arquivo"><i class="fa-upload fa"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="lnkRecebimento_Download" CommandArgument='<%# Bind("idLinha") %>' runat="server" ToolTip='<%# Bind("sNomeArquivo", "Download do Arquivo: {0}") %>' CssClass="btn btn-small" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="lnkRecebimento_Excluir" CommandArgument='<%# Bind("idLinha") %>' runat="server" CssClass="btn btn-small" TabIndex="100" ToolTip="Apagar Registro" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>

                                                </ItemTemplate>
                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>

                            </div>
                        </div>
                    </div>


                    <%------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>

                    <div class="panel panel-default" runat="server" id="DIV_Lancamentos">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Lançamentos</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina1" />
                                </div>
                                <div>


                                    <br />
                                </div>

                                <div class="col-lg-12">
                                    <asp:GridView ID="dtgLancamento" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="true" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDeleting="dtgLancamento_RowDeleting" OnRowDataBound="dtgLancamento_RowDataBound">

                                        <%--
                                    ========================
                                    =========índice=========
                                    ========================
                                    N° Célula ------- Info
                                    ========================
                                    0 --------------- idLinha
                                    1 --------------- Nº Parcela
                                    2 --------------- Valor Parcela Bruto
                                    3 --------------- Valor Parcela Liquido
                                    4 --------------- Data Vencimento
                                    5 --------------- Forma de Recebimento
                                    6 --------------- Botões
                                    ========================
                                        --%>
                                        <Columns>

                                            <asp:BoundField DataField="idlinha" HeaderText="idLinha">
                                                <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nParcelaLancamentoRec" HeaderText="Nº Parcela">
                                                <ItemStyle Width="2%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Valor Parcela Bruto">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtnValorBrutoLancamentoRec" class="form-control" runat="server" Text='<%# Bind("nValorBrutoLancamentoRec","{0:N2}") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle Width="14%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Valor Parcela Líquido">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtnValorLancamentoRec" class="form-control" runat="server" Text='<%# Bind("nValorLancamentoRec","{0:N2}") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle Width="14%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Data Vencimento">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtdtLancamentoRec" class="form-control" runat="server" type="date" Text='<%# Bind("dtLancamentoRec") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle Width="14%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Forma de Recebimento">
                                                <ItemTemplate>
                                                    <asp:DropDownList ID="ddlidFormaPagamentoLancamentoRec" runat="server" class="form-control yes_no select  Caixa_Selecao"></asp:DropDownList>
                                                </ItemTemplate>
                                                <ItemStyle Width="27%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkLancamentosRec_Excluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>

                            </div>
                        </div>
                    </div>


                    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div role="tabpanel" class="tab-pane fade" id="log" aria-labelledby="log-tab">
            <br />
            <div class="panel panel-default" runat="server" id="DIV_Log">
                <div class="panel-heading">

                    <h3 class="panel-title"><b>Historico</b></h3>
                </div>
                <div class="panel-body">
                    <div class="form-stacked row">
                        <div class="col-lg-12 table-responsive">
                            <asp:GridView
                                ID="gv_Log" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                ShowFooter="False" OnRowDataBound="gv_Log_RowDataBound">

                                <%--
                                       ========================
                                       =========índice=========
                                       ========================
                                       N° Célula ------- Info
                                       ========================
                                       0 --------------- Data
                                       1 --------------- Por
                                       2 --------------- Ação
                                       3 --------------- Detalhe
                                        ========================
                                --%>
                                <Columns>
                                    <asp:BoundField DataField="idLog" HeaderText="ID" Visible="False">
                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="dtAcao" HeaderText="Data">
                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sAcao" HeaderText="Ação">
                                        <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sObservacao" HeaderText="Detalhe">
                                        <ItemStyle Width="55%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>

                        </div>
                    </div>
                </div>
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
                                ID="gv_Historico" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                ShowFooter="False">

                                <%--
                                    ========================
                                    =========índice=========
                                    ========================
                                    N° Célula ------- Info
                                    ========================
                                    0 --------------- Data Emissão
                                    1 --------------- Data Vencimento
                                    2 --------------- Valor Liquido
                                    3 --------------- Saldo
                                    4 --------------- Nota Fiscal
                                    5 --------------- Número do Pedido
                                    6 --------------- Forma Recebimento
                                    7 --------------- Código Contabil
                                    8 --------------- Centro custo
                                    9 --------------- Meio Recebimento
                                    10 -------------- Por
                                    11 -------------- Data
                                    12 -------------- Reaberto
                                    ========================
                                --%>
                                <Columns>

                                    <asp:BoundField DataField="dtEmissao" HeaderText="Data Emissão">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="dtVencimento" HeaderText="Data Vencimento">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="nValorOriginal" HeaderText="Valor Líquido">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="nSaldo" HeaderText="Saldo">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sCodigo" HeaderText="Nota Fiscal">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDocumento" HeaderText="Número do Pedido">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscFormaRecebimento" HeaderText="Forma Recebimento">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscCodContabil" HeaderText="Código Contabil">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscCentroCusto" HeaderText="Centro custo">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscMeioRecebimento" HeaderText="Meio Recebimento">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="dtAtualizacao" HeaderText="Data">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sReabertoCompleto" HeaderText="Reaberto">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                </Columns>
                            </asp:GridView>

                        </div>
                    </div>
                </div>
            </div>
        </div>
        <%------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>

        <div role="tabpanel" class="tab-pane fade" id="arquivo" aria-labelledby="aba_Arquivo-tab">
            <div class=" embed-responsive embed-responsive-16by9" style="min-height: 200px">
                <embed type="text/html" runat="server" id="frmArquivos" width="800" height="500" />
            </div>
        </div>
        <%------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>
    </div>



    <fieldset class="form-stacked actions">
        <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" ValidationGroup="DETALHE" />

        <asp:Button ID="btnAlterarVencimento" class="btn  btn-lg btn-info" runat="server" Text="Editar" OnClick="btnAlterarVencimento_Click" />

        <asp:Button ID="BtnAdicionarObservacao" class="btn btn-lg btn-info" runat="server" Text="Adicionar Observação" OnClick="BtnAdicionarObservacao_Click" />

        <asp:Button ID="BtnExcluirParcela" class="btn  btn-lg btn-danger" runat="server" Text="Excluir" OnClick="BtnExcluirParcela_Click" />

        <asp:Button ID="BtnReabrirTitulo" class="btn  btn-lg btn-danger" runat="server" Text="Reabrir" OnClick="BtnReabrirTitulo_Click" />

        <asp:Button ID="cmdFaturamento" runat="server" Text="Faturamento" OnClick="cmdFaturamento_Click" CssClass="btn  btn-lg btn-primary" />

        <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field-cancel" title="Voltar" onclick="history.go(-1)">
    </fieldset>


    <div id="dialog-Salvar" class="modal" title="Salvar">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
        </p>
    </div>

    <div id="dialog-Excluir" class="modal" title="Excluir">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloExcluir" runat="server" Text=""></asp:Label>
        </p>
    </div>

    <div id="dialog-Reabrir" class="modal" title="Reabrir">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloReabrir" runat="server" Text=""></asp:Label>
        </p>
    </div>

    <asp:UpdatePanel ID="upd_EnvioArquivos" runat="server">
        <ContentTemplate>
            <div class="modal fade" id="UploadArquivos_Modal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="exampleModalLongTitle">
                                <b>
                                    <asp:Label ID="lblEnviarArquivos_Titulo" runat="server" Text="TITULO"></asp:Label></b></h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <uc1:MensagemPagina runat="server" ID="MensagemPagina_EnviarArquivo" />
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="form-stacked">
                                        <div class="col-lg-12" id="div1" runat="server">
                                            <div class="form-group">
                                                <label>Selecione o Arquivo</label>
                                                <asp:FileUpload ID="fu_EnviarArquivo" runat="server" />
                                            </div>
                                        </div>
                                        <div class="col-lg-12">
                                            <div class="form-group">
                                                <label>
                                                    <asp:Label ID="lblEnviarArquivo_sDscArquivo" runat="server" Text="Observação"></asp:Label></label>
                                                <asp:TextBox ID="txtEnviarArquivo_sDscArquivo" class="form-control" runat="server" MaxLength="300"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-12">
                                            <div class="form-group">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="cmdEnviarArquivos" CssClass="btn-success btn" runat="server" Text="Enviar" OnClick="cmdEnviarArquivos_Click" UseSubmitBehavior="False" />
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
                <asp:HiddenField ID="hddIdLinha" runat="server" />
                <asp:HiddenField ID="hddsBloco" runat="server" />
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdEnviarArquivos" />
        </Triggers>
    </asp:UpdatePanel>

    <div id="observacaoModal" class="modal">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Adicionar Anotação</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <asp:TextBox ID="txtsAnotacao" runat="server" TextMode="MultiLine" Rows="5" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="BtnSalvarObservacao" class="btn btn-success" runat="server" Text="Salvar" OnClick="BtnSalvarObservacao_Click" />
                    <button type="button" class="btn btn-danger" data-dismiss="modal">Fechar</button>
                </div>
            </div>
        </div>
    </div>


    <div class="modal fade" id="modalAlterarStatus" tabindex="-2" role="dialog" data-backdrop="static" aria-labelledby="modalAlterarStatus" aria-hidden="true">
        <div class="modal-dialog modal-sm" role="document" style="width: 50%">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Alterar Status</h5>
                </div>
                <div class="modal-body">

                    <asp:UpdatePanel ID="updModalStatus" runat="server">
                        <ContentTemplate>

                            <div class="col-lg-12 row" style="width: 100%; margin-left: 0px; padding: 0px;">
                                <asp:UpdatePanel ID="updMensagemModalAlteraStatus" runat="server">
                                    <ContentTemplate>
                                        <uc1:MensagemPagina runat="server" ID="MensagemModalAlteraStatus" style="margin-left: 0px !important" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div class="col-lg-3 form-group" runat="server" id="div_ddlNovoStatus">
                                <label>Novo Status</label>
                                <asp:DropDownList runat="server" ID="ddlsAlteraStatus" class="form-control Caixa_Selecao">
                                    <asp:ListItem Text="Selecione um Status" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Protestado" Value="Protestado"></asp:ListItem>
                                    <asp:ListItem Text="Cobrança Judicial" Value="Cobrança Judicial"></asp:ListItem>
                                    <asp:ListItem Text="Inadimplente" Value="Inadimplente"></asp:ListItem>
                                </asp:DropDownList>
                            </div>

                            <div class="col-lg-3 form-group" runat="server" id="div_txtNovoStatus">
                                <label>Novo Status</label>
                                <asp:TextBox ID="txtsNovoStatus" Text="Em Atraso" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                            </div>

                            <div class="col-lg-12 form-group">
                                <asp:Label runat="server" ID="lblsObservacao" Text="Observação" Style="font-weight: bold;"></asp:Label>
                                <label>&nbsp</label>
                                <asp:TextBox ID="txtsObservacaoStatus" runat="server" TextMode="MultiLine" Rows="5" CssClass="form-control"></asp:TextBox>
                            </div>

                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnSalvarStatus" class="btn btn-success" runat="server" Text="Salvar" OnClick="btnSalvarStatus_Click" />
                    <asp:Button ID="btnFecharModalStatus" class="btn btn-secondary" runat="server" Text="Fechar" OnClick="btnFecharModalStatus_Click" />
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hddidContasReceber" runat="server" />
    <asp:HiddenField ID="hddidEmpresa" runat="server" />
    <asp:HiddenField ID="hddidParceiro" runat="server" />
    <asp:HiddenField ID="hddidOPI" runat="server" />
    <asp:HiddenField ID="hddsTituloAdiantado" runat="server" Value="N" />

</asp:Content>
