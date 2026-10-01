<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Faturamento_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Faturamento.Faturamento_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>
<%@ Register Src="~/App/Controles/TextBox_Padrao.ascx" TagPrefix="uc1" TagName="TextBox_Padrao" %>
<%@ Register Src="~/App/Controles/DropDownList_Padrao.ascx" TagPrefix="uc1" TagName="DropDownList_Padrao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        .gvChildHeader th {
            padding: 4px;
            background-color: rgba(50, 150, 50);
            color: #fff;
            border: 1px solid #bbb;
            font-weight: bold;
        }

        .gvRow td {
            padding: 7px;
            background-color: #ffffff;
            border: 1px solid #bbb;
        }

        .gvAltRow td {
            padding: 7px;
            background-color: #f1f1f1;
            border: 1px solid #bbb;
        }

        .collapsed-row {
            display: none;
            padding: 1px;
            margin: 1px;
        }

        .checkbox-spacing {
            margin-right: 10px;
        }

        .modal-content {
            display: flex;
            flex-direction: column;
            height: auto;
        }

        .modal-body {
            max-height: 100vh;
            flex: 1 1 auto;
            overflow-y: visible;
        }

        .upload-container {
            width: fit-content;
            height: fit-content;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto;
        }

        .file-upload-label input {
            display: none;
        }

        .file-upload-label svg {
            height: 50px;
            fill: green;
            margin-bottom: 20px;
        }

        .file-upload-label {
            cursor: pointer;
            background-color: #d0f5da;
            padding: 30px 70px;
            border-radius: 40px;
            border: 2px dashed green;
            box-shadow: 0px 0px 20px -50px rgba(0, 0, 0, 0.719);
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            gap: 5px;
            text-align: center;
        }

            .file-upload-label.dragover {
                border-color: lightgreen;
                background-color: lightgreen;
            }

        .browse-button {
            background-color: green;
            padding: 5px 15px;
            border-radius: 10px;
            color: white;
            transition: all 0.3s;
        }

            .browse-button:hover {
                background-color: darkgreen;
            }

        .file-name {
            text-align: center;
            margin-top: 10px;
            font-size: 14px;
            color: green;
        }

        .tabelaImpostos tr td {
            border-color: black !important;
        }
    </style>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Faturamento"></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>
    </div>

    <div id="abas">
        <ul id="tab_Faturamento" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="tabpanel active">
                <a href="#Faturamento" id="aba_Faturamento" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Faturamento</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_PDF">
                <a href="#PDF" role="tab" id="PDF-tab" data-toggle="tab" aria-controls="PDF"><b>NF-e</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_ContasReceber">
                <a href="#ContasReceber" role="tab" id="ContasReceber-tab" data-toggle="tab" aria-controls="ContasReceber"><b>Contas Receber</b></a>
            </li>
        </ul>
    </div>

    <div id="tab" class="tab-content">

        <div role="tabpanel" class="tab-pane fade in active" id="Faturamento" aria-labelledby="Faturamento-tab">
            <br />
            <asp:UpdatePanel ID="UpdFaturamento" runat="server">
                <ContentTemplate>

                    <div class="panel panel-default" runat="server" id="div_Faturamento">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Detalhe</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">

                                <div id="div_FaturamentoDetalhe">

                                    <div class="col-lg-12 padd-0">

                                        <div class="col-lg-1">
                                            <label>Pedido</label>
                                            <h4 class="m-0">
                                                <asp:HyperLink ID="hplPedido" Target="_blank" runat="server" CssClass="form-control label label-primary"></asp:HyperLink></h4>
                                        </div>

                                        <div class="col-lg-2 form-group">
                                            <uc1:TextBox_Padrao runat="server" ID="txtdtPedido" Titulo="Data Pedido" ReadOnly="true" />
                                        </div>

                                        <div class="col-lg-3 form-group">
                                            <uc1:TextBox_Padrao runat="server" ID="txtnNumeroPedidoCliente" Titulo="N° Pedido (Cliente)" ReadOnly="true" />
                                        </div>

                                        <div class="col-lg-2 form-group">
                                            <uc1:TextBox_Padrao runat="server" ID="txtnControle" Titulo="N° Controle TT" ReadOnly="true" />
                                        </div>

                                        <div class="col-lg-4 form-group">
                                            <uc1:TextBox_Padrao runat="server" ID="txtsReferencia" Titulo="Referência" ReadOnly="true" />
                                        </div>

                                    </div>

                                    <div class="col-lg-12 padd-0">

                                        <div class="col-lg-6 form-group">
                                            <uc1:DropDownList_Padrao runat="server" ID="ddlidCliente" Titulo="Cliente" ReadOnly="true" Link="/App/Paginas/Manutencao/Parceiros_Detalhe.aspx?id=" />
                                        </div>

                                        <div class="col-lg-6 form-group">
                                            <uc1:TextBox_Padrao runat="server" ID="txtsEnderecoEntrega" Titulo="Endereço de Entrega" ReadOnly="true" />
                                        </div>

                                    </div>

                                    <div class="col-lg-12 padd-0">

                                        <div class="col-lg-4 form-group">
                                            <uc1:DropDownList_Padrao runat="server" ID="ddlEmpresa" Titulo="Empresa" ReadOnly="true" Link="/App/Paginas/Manutencao/Empresas_Detalhe.aspx?id=" />
                                        </div>

                                        <div class="col-lg-2 form-group">
                                            <uc1:DropDownList_Padrao runat="server" ID="ddlidCondicaoPagamento" Titulo="Condição Pagamento" ReadOnly="true" Link="/App/Paginas/Manutencao/CondicaodePagamento_Detalhe.aspx?id=" />
                                        </div>

                                        <div class="col-lg-2 form-group">
                                            <uc1:TextBox_Padrao runat="server" ID="txtsDscTipoFaturamento" Titulo="Tipo de Faturamento" ReadOnly="true" />
                                        </div>

                                        <div class="col-lg-2 form-group" runat="server" id="DIV_Status">
                                            <uc1:TextBox_Padrao runat="server" ID="txtsDscStatus" Titulo="Status OPI" ReadOnly="true" />
                                        </div>

                                        <div class="col-lg-2 form-group">
                                            <uc1:DropDownList_Padrao runat="server" ID="ddlidFluxo" Titulo="Fluxo" ReadOnly="true" Link="/App/Paginas/Manutencao/Fluxo_Detalhe.aspx?id=" />
                                        </div>

                                    </div>

                                    <div class="col-lg-12 padd-0">

                                        <div class="col-lg-2 form-group">
                                            <uc1:TextBox_Padrao runat="server" ID="txtnVlrProdutos" Titulo="Valor Produtos" ReadOnly="true" />
                                        </div>

                                        <div class="col-lg-2 form-group">
                                            <uc1:TextBox_Padrao runat="server" ID="txtnVlrServico" Titulo="Valor Serviços" ReadOnly="true" />
                                        </div>

                                        <div class="col-lg-2 form-group">
                                            <uc1:TextBox_Padrao runat="server" ID="txtnVlrTotal" Titulo="Valor Total" ReadOnly="true" />
                                        </div>

                                        <div class="col-lg-2 form-group">
                                            <uc1:TextBox_Padrao runat="server" ID="txtvlrFaturado" Titulo="Valor Faturado" ReadOnly="true" />
                                        </div>

                                        <div class="col-lg-2 form-group">
                                            <uc1:TextBox_Padrao runat="server" ID="txtvlrAFaturar" Titulo="Valor à Faturar" ReadOnly="true" />
                                        </div>

                                    </div>

                                </div>

                                <div class="col-lg-12" runat="server" id="DIV_Envios">
                                    <div class="panel panel-default m-0">
                                        <div class="panel-heading">
                                            <label class="panel-title">Envios</label>
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">

                                                <div class="col-lg-12 table-responsive">
                                                    <asp:GridView ID="gv_Envios" class="table table-striped table-bordered table-hover m-0" DataKeyNames="idEnvioOPI, sFaturamento"
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gv_Envios_RowDataBound">
                                                        <Columns>

                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:RadioButton ID="rbEnvio" GroupName="Envios" runat="server" CssClass="Individual" />
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="2%" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="ID">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="btnToggle" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-div-id='<%# "tr_Fatura" + Eval("idEnvioOPI") %>'></asp:LinkButton>
                                                                    <asp:Label runat="server" ID="idEnvioOPI" Text='<%# Bind("idEnvioOPI") %>' ClientIDMode="Static"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="3%" />
                                                            </asp:TemplateField>

                                                            <asp:BoundField DataField="dtEnvio" HeaderText="Data Envio">
                                                                <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="sDscOPI" HeaderText="Descrição">
                                                                <ItemStyle Width="24%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="nPesoLiquido" HeaderText="Peso Líquido">
                                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="nPesoBruto" HeaderText="Peso Bruto">
                                                                <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="nComprimento" HeaderText="Comprimento">
                                                                <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="nLargura" HeaderText="Largura">
                                                                <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="nAltura" HeaderText="Altura">
                                                                <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="nDimensoes" HeaderText="Volume m³">
                                                                <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="sFaturamento" HeaderText="Faturamento">
                                                                <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="sChave" HeaderText="Chave">
                                                                <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:TemplateField HeaderText="Qtd Volumes">
                                                                <ItemTemplate>
                                                                    <asp:Label runat="server" ID="nQtdVolumes" Text='<%# Bind("nQtdVolumes") %>'></asp:Label>

                                                                    <%# NovaLinha(Eval("idEnvioOPI"), "_Fatura") %>

                                                                    <asp:GridView ID="gvItensEnvio" class="table table-striped table-bordered table-hover table-condensed padd-0 m-0"
                                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                        HeaderStyle-CssClass="gvChildHeader" CssClass="gvRow" AlternatingRowStyle-CssClass="gvAltRow"
                                                                        Font-Names="Tahoma" Font-Overline="False" DataKeyNames="idEnvioOPI, idProduto" OnRowDataBound="gvItensEnvio_RowDataBound">
                                                                        <Columns>

                                                                            <asp:BoundField DataField="idProduto" HeaderText="ID">
                                                                                <HeaderStyle CssClass="invisivel" />
                                                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="invisivel idProduto" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="Ordem" HeaderText="Ordem">
                                                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="idProduto" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="sCodigo" HeaderText="Código">
                                                                                <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="sDscProduto" HeaderText="Descrição">
                                                                                <ItemStyle Width="24%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="sUnidade" HeaderText="Unidade">
                                                                                <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade">
                                                                                <ItemStyle Width="6%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="sCodigoBarras" HeaderText="sCodigoBarras">
                                                                                <HeaderStyle CssClass="invisivel" />
                                                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="invisivel" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="nValorUnitario" HeaderText="Valor Unitário">
                                                                                <ItemStyle Width="7%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="nVlrTotal" HeaderText="Valor Total">
                                                                                <ItemStyle Width="7%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                            </asp:BoundField>

                                                                            <asp:TemplateField HeaderText="Número Pedido Cliente">
                                                                                <ItemTemplate>
                                                                                    <asp:TextBox ID="txtnPedidoCliente" class="form-control" runat="server" MaxLength="15" Text='<%# Bind("nPedidoCliente") %>'></asp:TextBox>
                                                                                </ItemTemplate>
                                                                                <ItemStyle Width="11%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                            </asp:TemplateField>

                                                                            <asp:TemplateField HeaderText="CST ICMS">
                                                                                <ItemTemplate>
                                                                                    <asp:DropDownList ID="ddlidCSTICMS" runat="server" class="form-control Caixa_Selecao ddlidCSTICMS">
                                                                                        <asp:ListItem Value="0" Text="Selecione um CST"></asp:ListItem>
                                                                                    </asp:DropDownList>
                                                                                </ItemTemplate>
                                                                                <ItemStyle Width="16%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                            </asp:TemplateField>

                                                                            <asp:TemplateField HeaderText="Redução">
                                                                                <ItemTemplate>
                                                                                    <asp:TextBox ID="txtpReducao" class="form-control pReducao" runat="server" Text='<%# Bind("pRedBC") %>'></asp:TextBox>
                                                                                </ItemTemplate>
                                                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                            </asp:TemplateField>

                                                                            <asp:BoundField DataField="sFaturamento" HeaderText="sFaturamento">
                                                                                <HeaderStyle CssClass="invisivel" />
                                                                                <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="invisivel sFaturamento" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="sServico" HeaderText="sServico">
                                                                                <HeaderStyle CssClass="invisivel" />
                                                                                <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="invisivel" />
                                                                            </asp:BoundField>

                                                                        </Columns>
                                                                    </asp:GridView>

                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="6%" />
                                                            </asp:TemplateField>

                                                            <asp:BoundField DataField="idArquivo" HeaderText="Arquivo">
                                                                <HeaderStyle CssClass="invisivel" />
                                                                <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper invisivel" />
                                                            </asp:BoundField>

                                                        </Columns>
                                                    </asp:GridView>
                                                </div>

                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-12" runat="server" id="DIV_vlrFaturamento">
                                    <div class="panel panel-default m-0">
                                        <div class="panel-heading">
                                            <h3 class="panel-title"><b>Faturamento</b></h3>
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">
                                                <div class="col-lg-12 table-responsive">
                                                    <asp:GridView ID="gv_Faturamento" class="table table-striped table-bordered table-hover m-0" ShowFooter="true"
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                        Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gv_Faturamento_RowDataBound">
                                                        <Columns>

                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:RadioButton ID="rbFaturamento" runat="server" ValidationGroup="Faturamento" />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:BoundField DataField="idFaturamento" HeaderText="ID">
                                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="sDscFaturamento" HeaderText="Descrição" HtmlEncode="false">
                                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idServico"
                                                                DataTextField="sDscServico" HeaderText="Serviço" Target="_blank"
                                                                DataNavigateUrlFormatString="/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="48%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:BoundField DataField="vlrFaturamento" HeaderText="Valor" DataFormatString="{0:N2}">
                                                                <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="dtFaturamento" HeaderText="Data">
                                                                <ItemStyle Width="13%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="sFaturado" HeaderText="sFaturado">
                                                                <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="ContasReceber" aria-labelledby="ContasReceber-tab">
            <br />
            <div class="panel panel-default" runat="server" id="DIV_11">
                <div class="panel-heading">

                    <h3 class="panel-title"><b>Contas Receber</b></h3>
                </div>
                <div class="panel-body">
                    <div class="form-stacked row">
                        <div class="col-lg-12 table-responsive">
                            <asp:GridView ID="gv_ContasReceber" class="table table-striped table-bordered table-hover m-0" ShowFooter="true" DataKeyNames="idContasReceber"
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                <Columns>

                                    <asp:HyperLinkField DataNavigateUrlFields="idContasReceber" DataTextField="idContasReceber" HeaderText="ID" Target="_blank"
                                        DataNavigateUrlFormatString="/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}">
                                        <ItemStyle Width="4%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:BoundField DataField="dtEmissao_ordem" HeaderText="Emissão" DataFormatString="{0:u}" SortExpression="dtEmissao_ordem">
                                        <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="dtVencimento_ordem" HeaderText="Vencimento" DataFormatString="{0:u}" SortExpression="dtVencimento_ordem">
                                        <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idContasReceber" DataTextField="sQuantidadeParcela" HeaderText="Parcela" Target="_blank"
                                        DataNavigateUrlFormatString="/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}">
                                        <ItemStyle Width="4%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idContasReceber" DataTextField="sDocumento" HeaderText="Pedido" Target="_blank"
                                        DataNavigateUrlFormatString="/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}">
                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idContasReceber" DataTextField="sCodigo" HeaderText="Nota Fiscal" Target="_blank"
                                        DataNavigateUrlFormatString="/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idContasReceber" DataTextFormatString="{0:C2}" DataTextField="nValorBruto" HeaderText="Valor Bruto" Target="_blank"
                                        DataNavigateUrlFormatString="/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}">
                                        <ItemStyle Width="11%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idContasReceber" DataTextFormatString="{0:C2}" DataTextField="nValorOriginal" HeaderText="Valor Líquido" Target="_blank"
                                        DataNavigateUrlFormatString="/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}">
                                        <ItemStyle Width="12%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idContasReceber" DataTextField="sDscCategoriaReceber" HeaderText="Categoria" Target="_blank"
                                        DataNavigateUrlFormatString="/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}">
                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idContasReceber" DataTextField="sDscFormaRecebimento" HeaderText="Forma de Recebimento" Target="_blank"
                                        DataNavigateUrlFormatString="/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}">
                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="PDF" aria-labelledby="PDF-tab">
            <br />
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Documentos Fiscais</b></h3>
                </div>
                <div class="panel-body">
                    <div class="form-stacked row">

                        <div class="col-lg-12">
                            <uc1:MensagemPagina runat="server" ID="MensagemPagina_AbaPDF" />
                        </div>

                        <div class="col-lg-12 table-responsive">
                            <asp:GridView ID="gvDocumentosFiscais" class="table table-striped table-bordered table-hover m-0"
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="chNFe, idEnvioOPI, sXML_Autorizado, idXML"
                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvDocumentosFiscais_RowDataBound">
                                <Columns>

                                    <asp:BoundField DataField="idXML" HeaderText="ID">
                                        <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="idEnvioOPI" HeaderText="ID Envio">
                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sdtEvento_Formatada" HeaderText="Data">
                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sTipo" HeaderText="Tipo">
                                        <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="nSerieNF" HeaderText="Série">
                                        <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="toUpper" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="nNumeroNF" HeaderText="Número">
                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="toUpper" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="chNFe" HeaderText="Chave NFe">
                                        <ItemStyle Width="16%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sStatus" HeaderText="Status">
                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="xMotivo" HeaderText="Observação">
                                        <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                    </asp:BoundField>

                                    <asp:TemplateField HeaderText="Ações">
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdGerarNFe" class="btn btn-sm btn-success" Text="Enviar Sefaz" OnClick="DocumentosFiscais_cmdGerarNFe_Click"></asp:LinkButton>
                                            <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdDownloadXML" class="btn btn-sm btn-primary" Text="XML" OnClick="DocumentosFiscais_cmdDownloadXML_Click"></asp:LinkButton>
                                            <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdDownloadDANFE" class="btn btn-sm btn-warning" Text="DANFE" OnClick="DocumentosFiscais_cmdDownloadDANFE_Click"></asp:LinkButton>
                                            <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdEnviarEmail" CssClass="btn btn-sm btn-info" ClientIDMode="Static" ToolTip="Enviar Email" Text="Email" OnClick="DocumentosFiscais_cmdEnviarEmail_Click"></asp:LinkButton>

                                            <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdEditarXML" CssClass="btn btn-sm btn-danger" ClientIDMode="Static" Text="Editar XML" OnClick="DocumentosFiscais_cmdEditarXML_Click"></asp:LinkButton>
                                            <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdCartaCorrecao" class="btn btn-sm btn-success" ToolTip="Carta de Correção" Text="CC-e" OnClick="DocumentosFiscais_cmdCartaCorrecao_Click"></asp:LinkButton>

                                            <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdDownloadCCeXML" class="btn btn-sm btn-primary" Text="XML" OnClick="DocumentosFiscais_cmdDownloadCCeXML_Click"></asp:LinkButton>
                                            <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdDownloadCCePDF" class="btn btn-sm btn-danger" Text="PDF" OnClick="DocumentosFiscais_cmdDownloadCCePDF_Click"></asp:LinkButton>

                                            <asp:LinkButton runat="server" ID="NFS_btnImportarRetorno" class="btn btn-sm btn-success" Text="Retorno NFS-e" data-toggle="modal" data-target="#modalImportarRetorno_NFS"></asp:LinkButton>
                                            <asp:LinkButton runat="server" ID="NFS_btnGerarPDFEspelho" class="btn btn-sm btn-primary" Text="NFS-e Espelho" OnClick="btnGerarPDFEspelho_Click"></asp:LinkButton>
                                            <asp:LinkButton runat="server" ID="NFS_btnGerarTXT" class="btn btn-sm btn-info" Text="TXT" OnClick="btnGerarTXT_Click"></asp:LinkButton>

                                            <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdCancelarNFE" class="btn btn-sm btn-danger" ToolTip="Cancelar NF-e" Text="Cancelar NF-e" OnClick="cmdCancelarNFE_Click"></asp:LinkButton>
                                            <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdDevolucao" class="btn btn-sm btn-info" ToolTip="Devolução" Text="Devolução" OnClick="DocumentosFiscais_cmdDevolucao_Click"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="idFaturamento" HeaderText="Faturamento" />
                                    <asp:BoundField DataField="nNumeroCartaCorrecao" HeaderText="Qtd Cartas" />
                                    <asp:BoundField DataField="sCancelamento" HeaderText="Tem Cancelamento" />
                                    <asp:BoundField DataField="sChaveNFE" HeaderText="ChaveNFe" />

                                </Columns>
                            </asp:GridView>
                        </div>

                        <div class="col-lg-12" id="div_Email" runat="server">
                            <div class="row">

                                <div class="col-lg-4 form-group" runat="server" id="DIV34">
                                    <uc1:DropDownList_Padrao runat="server" ID="ddlsEmailCliente" Titulo="Email do Destinatário" Validacao="true" Obrigatorio="true" />
                                </div>

                                <div class="col-lg-5 form-group" runat="server" id="DIV44">
                                    <uc1:TextBox_Padrao runat="server" ID="txtsEmailCopia" Titulo="Email em Copia" Validacao="true" Obrigatorio="true" Placeholder="ex: tt@tecandtec.com.br;tt@tecandtec.com.br" />
                                </div>

                                <div class="col-lg-1" runat="server" id="DIV35">
                                    <div class="form-group">
                                        <br />
                                        <label>&nbsp;</label>
                                        <asp:Button ID="cmdEnviarEmailSTSO" class="btn btn-lg btn-info" runat="server" Text="Enviar" OnClick="cmdEnviarEmailSTSO_Click" />
                                    </div>
                                </div>

                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>

    </div>

    <fieldset class="form-stacked actions">
        <asp:Button ID="cmdSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
        <asp:Button ID="cmdFaturar" class="btn btn-lg btn-info" runat="server" Text="Faturar" OnClick="cmdFaturar_Click" />
        <a href="javascript:history.back()" class="btn btn-lg btn-warning" title="Voltar">Voltar</a>
        <asp:HyperLink runat="server" ID="cmdPedido" NavigateUrl="/App/Paginas/Pedidos_Detalhe.aspx?id=" Target="_blank" data-toggle="tooltip_top" title="Ir para o Pedido" CssClass="btn btn-lg btn-primary" Style="float: right;">Pedido <i class="fa fa-arrow-right"></i></asp:HyperLink>
        <asp:HyperLink runat="server" ID="cmdOPI" NavigateUrl="/App/Paginas/WMS/OPI_Detalhe.aspx?id=" Target="_blank" data-toggle="tooltip_top" title="Ir para a OPI" CssClass="btn btn-lg btn-primary" Style="float: right;">OPI <i class="fa fa-arrow-right"></i></asp:HyperLink>
    </fieldset>

    <div class="modal fade" id="Modal_Receber" data-backdrop="static">
        <div class="modal-dialog modal-largo">

            <asp:UpdatePanel ID="UpdModal_Receber" runat="server">
                <ContentTemplate>

                    <div class="modal-content">
                        <div class="modal-header">
                            <div class="modal-title">
                                <img class="modal-logo" src="../../../img/instrucaoTecnica_pdf.jpg" alt="Logo TT" />
                                <label runat="server" id="lblTitulo_Modal_Receber"></label>
                                <button type="button" id="btnFechar" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span></button>
                            </div>
                        </div>
                        <div class="modal-body">

                            <uc1:MensagemPagina runat="server" ID="MensagemPagina_Modal_Receber" />

                            <div id="abas_Modal_Receber">
                                <ul class="nav nav-tabs" role="tablist">
                                    <li runat="server" id="aba_NF" role="presentation" class="active">
                                        <a href="#NF" id="NF-tab" role="tab" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>NF</b></a>
                                    </li>
                                    <li runat="server" id="aba_Transportadora" role="presentation">
                                        <a href="#Transportadora" role="tab" id="Transportadora-tab" data-toggle="tab" aria-controls="Transportadora"><b>Transportadora</b></a>
                                    </li>
                                    <li runat="server" id="aba_ItensDevolucao" role="presentation">
                                        <a href="#ItensDevolucao" id="ItensDevolucao-tab" role="tab" data-toggle="tab" aria-controls="Itens_Devolucao" aria-expanded="false"><b>Itens Devolução</b></a>
                                    </li>
                                    <li runat="server" id="aba_Informacoes" role="presentation">
                                        <a href="#Informacoes" role="tab" id="Informacoes-tab" data-toggle="tab" aria-controls="Informacoes"><b>Informações</b></a>
                                    </li>
                                    <li runat="server" id="aba_Impostos" role="presentation">
                                        <a href="#Impostos" role="tab" id="Impostos-tab" data-toggle="tab" aria-controls="Itens/Impostos"><b>Impostos</b></a>
                                    </li>
                                </ul>
                                <br />
                            </div>

                            <div id="tab_Modal_Receber" class="tab-content">

                                <div role="tabpanel" class="tab-pane fade in active" id="NF" aria-labelledby="NF-tab">
                                    <div id="div_Fatumento_NF" runat="server">

                                        <div class="panel panel-default" runat="server" id="div_NF">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>Dados NFe</b></h3>
                                            </div>
                                            <div class="panel-body">
                                                <div class="form-stacked row">

                                                    <div class="col-lg-2 form-group">
                                                        <uc1:TextBox_Padrao runat="server" ID="txtdtEmissao" Titulo="Data de Emissão" ReadOnly="true" Modo="Data" />
                                                    </div>

                                                    <div class="col-lg-5 form-group">
                                                        <uc1:DropDownList_Padrao runat="server" ID="ddlidParceiro" Titulo="Parceiro" ReadOnly="true" Link="/App/Paginas/Manutencao/Parceiros_Detalhe.aspx?id=" />
                                                    </div>

                                                    <div class="col-lg-5 form-group">
                                                        <uc1:DropDownList_Padrao runat="server" ID="ddlidEmpresa" Titulo="Empresa" ReadOnly="true" Link="/App/Paginas/Manutencao/Empresas_Detalhe.aspx?id=" />
                                                    </div>

                                                    <div class="col-lg-12">
                                                        <div class="row">

                                                            <div class="col-lg-2 form-group" runat="server" id="Div_nValorBruto">
                                                                <uc1:TextBox_Padrao runat="server" ID="txtnValorBruto" Titulo="Valor Bruto" ReadOnly="true" Grupo="true" Grupo_Simbolo="<span class='input-group-addon'>R$</span>" />
                                                            </div>

                                                            <div class="col-lg-2 form-group" runat="server" id="Div_nValorOriginal">
                                                                <uc1:TextBox_Padrao runat="server" ID="txtnValorOriginal" Titulo="Valor Líquido" ReadOnly="true" Grupo="true" Grupo_Simbolo="<span class='input-group-addon'>R$</span>" />
                                                            </div>

                                                            <div class="col-lg-2 form-group">
                                                                <uc1:TextBox_Padrao runat="server" ID="txtvlrFaturadoOriginal" Titulo="Valor Faturado" ReadOnly="true" Grupo="true" Grupo_Simbolo="<span class='input-group-addon'>R$</span>" />
                                                            </div>

                                                            <div class="col-lg-2 form-group">
                                                                <uc1:TextBox_Padrao runat="server" ID="txtvlrAFaturarOriginal" Titulo="Valor à Faturar" ReadOnly="true" Grupo="true" Grupo_Simbolo="<span class='input-group-addon'>R$</span>" />
                                                            </div>

                                                            <div class="col-lg-4 form-group">
                                                                <uc1:TextBox_Padrao runat="server" ID="txtsCodigo" Titulo="Nota Fiscal" ReadOnly="true" />
                                                            </div>

                                                            <div class="row">
                                                                <div class="col-lg-12">

                                                                    <div class="col-lg-4 form-group">
                                                                        <uc1:TextBox_Padrao runat="server" ID="txtsDocumento" Titulo="Número do Pedido" ReadOnly="true" />
                                                                    </div>

                                                                    <div class="col-lg-4 form-group">
                                                                        <uc1:DropDownList_Padrao runat="server" ID="ddlidCategoriaReceber" Titulo="Categoria" Validacao="true" Obrigatorio="true" Link="/App/Paginas/Adm/Manutencao/CategoriaContasReceber_Detalhe.aspx?id=" AutoPostBack="true" OnSelectedIndexChanged="ddlidCategoriaReceber_SelectedIndexChanged" />
                                                                    </div>

                                                                    <div class="col-lg-4 form-group">
                                                                        <uc1:DropDownList_Padrao runat="server" ID="ddlidContabil" Titulo="Código Contábil" ReadOnly="true" Link="/App/Paginas/Adm/Manutencao/CodigoContabil_Detalhe.aspx?id=" />
                                                                    </div>

                                                                </div>
                                                            </div>

                                                            <div id="div_linhaTipoRecebimento" runat="server">

                                                                <div class="col-lg-4 form-group">
                                                                    <uc1:DropDownList_Padrao runat="server" ID="ddlidCentroDeCusto" Titulo="Centro de Custo" Validacao="true" Obrigatorio="true" Link="/App/Paginas/Adm/Manutencao/CentroCusto_Detalhe.aspx?id=" />
                                                                </div>

                                                                <div class="col-lg-4 form-group">
                                                                    <uc1:DropDownList_Padrao runat="server" ID="ddlidMeioRecebimento" Titulo="Tipo de Recebimento" Validacao="true" Obrigatorio="true" AutoPostBack="true" OnSelectedIndexChanged="ddlidMeioRecebimento_SelectedIndexChanged" />
                                                                </div>

                                                                <div class="col-lg-4 form-group" runat="server" id="Div_nParcelas">
                                                                    <uc1:TextBox_Padrao runat="server" ID="txtnParcelas" Titulo="N° Parcelas" Validacao="true" Obrigatorio="true" Modo="Inteiro" AutoPostBack="true" OnTextChanged="txtnParcelas_TextChanged" />
                                                                </div>

                                                            </div>

                                                        </div>
                                                    </div>

                                                    <div class="col-lg-12">
                                                        <uc1:TextBox_Padrao runat="server" ID="txtsObservacaoGeral" Titulo="Observações Gerais" Modo="Multilinha" Rows="5" />
                                                    </div>

                                                </div>
                                            </div>
                                        </div>

                                        <div class="panel panel-default" runat="server" id="DIV_Lancamentos">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>Lançamentos Contas a Receber</b></h3>
                                            </div>
                                            <div class="panel-body">
                                                <div class="form-group row">

                                                    <div class="col-lg-12">
                                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina1" />
                                                    </div>

                                                    <div class="col-lg-12">
                                                        <asp:GridView ID="dtgLancamento" class="table table-striped table-bordered table-hover table-condensed m-0"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" ShowFooter="true"
                                                            OnRowDeleting="dtgLancamento_RowDeleting" OnRowDataBound="dtgLancamento_RowDataBound">
                                                            <Columns>

                                                                <asp:BoundField DataField="idlinha" HeaderText="idLinha">
                                                                    <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="nParcelaLancamentoRec" HeaderText="Nº Parcela">
                                                                    <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="Valor Bruto">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="txtnValorBrutoLancamentoRec" class="form-control valorBruto_Lancamento" runat="server" Text='<%# Bind("nValorBrutoLancamentoRec", "{0:N2}") %>'></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Valor Líquido">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="txtnValorLancamentoRec" class="form-control valorLiquido_Lancamento" runat="server" Text='<%# Bind("nValorLancamentoRec", "{0:N2}") %>'></asp:TextBox>
                                                                        <asp:HiddenField ID="hddValorLiquido" runat="server" Value="0" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Data Vencimento">
                                                                    <ItemTemplate>
                                                                        <div class="d-flex fw-no">
                                                                            <asp:TextBox ID="txtdtLancamentoRec" class="form-control aplicaTodos" data-aplicatodos="data" runat="server" type="date" Text='<%# Bind("dtLancamentoRec") %>'></asp:TextBox>
                                                                            <asp:LinkButton ID="cmdAplicaTodos_data" class="btn btn-link" runat="server"><i class="fa fa-retweet" data-toggle="tooltip_top" title="Aplicar em todos"></i></asp:LinkButton>
                                                                        </div>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Forma de Recebimento">
                                                                    <ItemTemplate>
                                                                        <div class="d-flex fw-no">
                                                                            <asp:DropDownList ID="ddlidFormaPagamentoLancamentoRec" runat="server" class="form-control Caixa_Selecao aplicaTodos" data-aplicatodos="id" AutoPostBack="true" OnSelectedIndexChanged="ddlidFormaPagamentoLancamentoRec_SelectedIndexChanged"></asp:DropDownList>
                                                                            <asp:LinkButton ID="cmdAplicaTodos_idForma" class="btn btn-link" runat="server"><i class="fa fa-retweet" data-toggle="tooltip_top" title="Aplicar em todos"></i></asp:LinkButton>
                                                                        </div>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="25%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Conta Bancária">
                                                                    <ItemTemplate>
                                                                        <div class="d-flex fw-no">
                                                                            <asp:DropDownList ID="ddlidContaBancaria" runat="server" class="form-control Caixa_Selecao aplicaTodos" data-aplicatodos="id"></asp:DropDownList>
                                                                            <asp:LinkButton ID="cmdAplicaTodos_idConta" class="btn btn-link" runat="server"><i class="fa fa-retweet" data-toggle="tooltip_top" title="Aplicar em todos"></i></asp:LinkButton>
                                                                        </div>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="25%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>

                                                </div>
                                            </div>
                                        </div>

                                        <div class="panel panel-default" runat="server" id="div_Servicos">
                                            <div class="panel-heading">
                                                <div class="d-flex fw-no space-b">
                                                    <h3 class="panel-title"><b>Serviços</b></h3>
                                                    <div class="btn-group dropup">
                                                        <a id="cmdImpostos" class="btn btn-info dropdown-toggle" data-toggle="dropdown"><i class="fa fa-plus"></i>&nbsp;Impostos</a>
                                                        <div class="dropdown-menu dropdown-menu-right m-0 padd-0">
                                                            <asp:Table runat="server" ID="tbImpostos" CssClass="table table-bordered table-condensed table-hover tabelaImpostos m-0" Style="width: 500px;">
                                                                <asp:TableRow>
                                                                    <asp:TableCell HorizontalAlign="Right" CssClass="td_IR danger"><b>IR</b></asp:TableCell>
                                                                    <asp:TableCell HorizontalAlign="Right" CssClass="td_ISS danger"><b>ISS</b></asp:TableCell>
                                                                    <asp:TableCell HorizontalAlign="Right" CssClass="td_INSS danger"><b>INSS</b></asp:TableCell>
                                                                    <asp:TableCell HorizontalAlign="Right" CssClass="td_CSLL danger"><b>CSLL</b></asp:TableCell>
                                                                    <asp:TableCell HorizontalAlign="Right" CssClass="td_PIS danger"><b>PIS</b></asp:TableCell>
                                                                    <asp:TableCell HorizontalAlign="Right" CssClass="td_COFINS danger"><b>COFINS</b></asp:TableCell>
                                                                </asp:TableRow>
                                                                <asp:TableRow>
                                                                    <asp:TableCell runat="server" ID="txtImposto_IR" HorizontalAlign="Right" CssClass="td_IR danger"></asp:TableCell>
                                                                    <asp:TableCell runat="server" ID="txtImposto_ISS" HorizontalAlign="Right" CssClass="td_ISS danger"></asp:TableCell>
                                                                    <asp:TableCell runat="server" ID="txtImposto_INSS" HorizontalAlign="Right" CssClass="td_INSS danger"></asp:TableCell>
                                                                    <asp:TableCell runat="server" ID="txtImposto_CSLL" HorizontalAlign="Right" CssClass="td_CSLL danger"></asp:TableCell>
                                                                    <asp:TableCell runat="server" ID="txtImposto_PIS" HorizontalAlign="Right" CssClass="td_PIS danger"></asp:TableCell>
                                                                    <asp:TableCell runat="server" ID="txtImposto_COFINS" HorizontalAlign="Right" CssClass="td_COFINS danger"></asp:TableCell>
                                                                </asp:TableRow>
                                                            </asp:Table>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="panel-body">
                                                <div class="form-group row">

                                                    <div class="col-lg-2 form-group">
                                                        <uc1:SwitchAtivo runat="server" ID="SwitchAtivo_Retencao_IR" sFunctionScript_Switch="Calcula_ValorLiquido();" />
                                                    </div>
                                                    <div class="col-lg-2 form-group">
                                                        <uc1:SwitchAtivo runat="server" ID="SwitchAtivo_Retencao_ISS" sFunctionScript_Switch="Calcula_ValorLiquido();" />
                                                    </div>
                                                    <div class="col-lg-2 form-group">
                                                        <uc1:SwitchAtivo runat="server" ID="SwitchAtivo_Retencao_INSS" sFunctionScript_Switch="Calcula_ValorLiquido();" />
                                                    </div>
                                                    <div class="col-lg-2 form-group">
                                                        <uc1:SwitchAtivo runat="server" ID="SwitchAtivo_Retencao_CSLL" sFunctionScript_Switch="Calcula_ValorLiquido();" />
                                                    </div>
                                                    <div class="col-lg-2 form-group">
                                                        <uc1:SwitchAtivo runat="server" ID="SwitchAtivo_Retencao_PIS" sFunctionScript_Switch="Calcula_ValorLiquido();" />
                                                    </div>
                                                    <div class="col-lg-2 form-group">
                                                        <uc1:SwitchAtivo runat="server" ID="SwitchAtivo_Retencao_COFINS" sFunctionScript_Switch="Calcula_ValorLiquido();" />
                                                    </div>

                                                    <div class="col-lg-6 form-group">
                                                        <uc1:DropDownList_Padrao runat="server" ID="ddlRPS" Titulo="Situação do RPS" Validacao="true" Obrigatorio="true">
                                                            <asp:ListItem Value="T" Text="Tributado em São Paulo" Selected="true" />
                                                            <asp:ListItem Value="F" Text="Tributado Fora de São Paulo" />
                                                            <asp:ListItem Value="A" Text="Tributado em São Paulo, porém Isento" />
                                                            <asp:ListItem Value="B" Text="Tributado Fora de São Paulo, porém Isento" />
                                                            <asp:ListItem Value="D" Text="Tributado em São Paulo com isenção parcial" />
                                                            <asp:ListItem Value="M" Text="Tributado em São Paulo, porém com indicação de imunidade subjetiva" />
                                                            <asp:ListItem Value="N" Text="Tributado Fora de São Paulo, porém com indicação de imunidade subjetiva" />
                                                            <asp:ListItem Value="R" Text="Tributado em São Paulo, porém com indicação de imunidade objetiva" />
                                                            <asp:ListItem Value="S" Text="Tributado Fora de São Paulo, porém com indicação de imunidade objetiva" />
                                                            <asp:ListItem Value="X" Text="Tributado em São Paulo, porém Exgibilidade Suspensa" />
                                                            <asp:ListItem Value="V" Text="Tributado Fora de São Paulo, porém Exgibilidade Suspensa" />
                                                            <asp:ListItem Value="P" Text="Exportação de Serviços" />
                                                            <asp:ListItem Value="C" Text="Cancelado" />
                                                        </uc1:DropDownList_Padrao>
                                                    </div>

                                                    <div runat="server" id="div_txtnISS" class="col-lg-2 form-group">
                                                        <uc1:TextBox_Padrao runat="server" ID="txtnISS" Titulo="ISS" Validacao="true" Obrigatorio="true" Modo="Decimal" TamanhoTotal="5" SeparadorMilhar="false" Grupo_Simbolo="<b class='symbol'><i class='fa fa-percent'></i></b>" />
                                                    </div>

                                                    <div class="col-lg-12 form-group">
                                                        <uc1:TextBox_Padrao runat="server" ID="txtsDscContrib" Titulo="Descrição das Contribuições Sociais" />
                                                    </div>

                                                    <div class="col-lg-12">
                                                        <uc1:TextBox_Padrao runat="server" ID="txtsDscServico" Titulo="Corpo da NFS-e (Discriminação dos Serviços)" Validacao="true" Obrigatorio="true" Modo="Multilinha" Rows="10" />
                                                    </div>

                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>

                                <div role="tabpanel" class="tab-pane fade" id="Transportadora" aria-labelledby="Transportadora-tab">

                                    <div class="panel panel-default" runat="server" id="div5">
                                        <div class="panel-heading">
                                            <h3 class="panel-title"><b>Detalhe</b></h3>
                                        </div>
                                        <div class="panel-body">
                                            <div class="form-stacked row">

                                                <div class="col-lg-4 form-group">
                                                    <uc1:DropDownList_Padrao runat="server" ID="ddltranporte" Titulo="Modo Frete" Validacao="true" Obrigatorio="true">
                                                        <asp:ListItem Value="" Text="Selecione o Modo Frete" />
                                                        <asp:ListItem Value="0" Text="0 - Contrat. conta do Remetente (CIF)" Selected="true" />
                                                        <asp:ListItem Value="1" Text="1 - Contrat. conta do Destinatário (FOB)" />
                                                        <asp:ListItem Value="2" Text="2 - Contrat. conta de Terceiros" />
                                                        <asp:ListItem Value="3" Text="3 - Transp. próp. conta de Remetente" />
                                                        <asp:ListItem Value="4" Text="4 - Transp. próp. conta de Destinatário" />
                                                        <asp:ListItem Value="9" Text="9 - Sem ocorrência de transporte" />
                                                    </uc1:DropDownList_Padrao>
                                                </div>

                                                <div class="col-lg-12 row">

                                                    <div class="col-lg-6 form-group">
                                                        <uc1:DropDownList_Padrao runat="server" ID="ddlidTranportadora" Titulo="Transportadora" Validacao="true" Obrigatorio="true" Link="/App/Paginas/Manutencao/Parceiros_Detalhe.aspx?id=" AutoPostBack="true" OnSelectedIndexChanged="ddlidTranportadora_SelectedIndexChanged" />
                                                    </div>

                                                    <div class="col-lg-12 row" runat="server" id="div_InfoTransportadora">
                                                        <div class="col-lg-3 form-group">
                                                            <uc1:TextBox_Padrao runat="server" ID="txtCNPJTrans" Titulo="CNPJ" ReadOnly="true" />
                                                        </div>

                                                        <div class="col-lg-6 form-group">
                                                            <uc1:TextBox_Padrao runat="server" ID="txtNomeTrans" Titulo="Nome" ReadOnly="true" />
                                                        </div>

                                                        <div class="col-lg-2 form-group" runat="server" id="DIV_IETrans">
                                                            <uc1:TextBox_Padrao runat="server" ID="txtIETrans" Titulo="IE" ReadOnly="true" />
                                                        </div>

                                                        <div class="col-lg-6 form-group">
                                                            <uc1:TextBox_Padrao runat="server" ID="txtEnderecoTrans" Titulo="Endereço" ReadOnly="true" />
                                                        </div>

                                                        <div class="col-lg-2 form-group">
                                                            <uc1:TextBox_Padrao runat="server" ID="txtMunicipiotrans" Titulo="Município" ReadOnly="true" />
                                                        </div>

                                                        <div class="col-lg-1 form-group">
                                                            <uc1:TextBox_Padrao runat="server" ID="txtUFTrans" Titulo="UF" ReadOnly="true" />
                                                        </div>
                                                    </div>

                                                </div>

                                                <div class="col-lg-12 row">
                                                    <div class="col-lg-2">
                                                        <uc1:TextBox_Padrao runat="server" ID="txtVolumes" Titulo="Qtd Volumes" ReadOnly="true" />
                                                    </div>

                                                    <div class="col-lg-2">
                                                        <uc1:TextBox_Padrao runat="server" ID="txtEspecie" Titulo="Espécie" Validacao="true" Obrigatorio="true" />
                                                    </div>

                                                    <div class="col-lg-2">
                                                        <uc1:TextBox_Padrao runat="server" ID="txtPesoLiquido" Titulo="Peso Líquido" ReadOnly="true" />
                                                    </div>

                                                    <div class="col-lg-2">
                                                        <uc1:TextBox_Padrao runat="server" ID="txtPesoBruto" Titulo="Peso Bruto" ReadOnly="true" />
                                                    </div>
                                                </div>

                                            </div>
                                        </div>
                                    </div>

                                </div>

                                <div role="tabpanel" class="tab-pane fade" id="Informacoes" aria-labelledby="Informacoes-tab">

                                    <div class="panel panel-default" runat="server" id="div6">
                                        <div class="panel-heading">
                                            <h3 class="panel-title"><b>Detalhe</b></h3>
                                        </div>
                                        <div class="panel-body">
                                            <div class="form-stacked row">

                                                <div class="col-lg-12 form-group">
                                                    <div class="btn-group">
                                                        <asp:CheckBox runat="server" ID="chkExibirFabricante" Text="&nbsp;Exibir Fabricante" CssClass="btn btn-primary" />
                                                        <asp:CheckBox runat="server" ID="chksUsarProdutosCliente" Text="&nbsp;NFe com Produtos do Cliente" CssClass="btn btn-primary" AutoPostBack="true" OnCheckedChanged="chkExibirProdutosCliente_CheckedChanged" />
                                                    </div>
                                                </div>

                                                <div runat="server" id="DIV_SalvarCFOP">
                                                    <div class="col-lg-5 form-group" runat="server" id="DIV_CFOP">
                                                        <uc1:DropDownList_Padrao runat="server" ID="ddlidCFOP" Titulo="CFOP" Validacao="true" Obrigatorio="true" AutoPostBack="true" OnSelectedIndexChanged="ddlidCFOP_SelectedIndexChanged" />
                                                    </div>

                                                    <div class="col-lg-2 form-group row" runat="server" id="DIV_btnSalvarCFOP">
                                                        <label>&nbsp;</label>
                                                        <asp:LinkButton ID="btnSalvarCFOP" CssClass="form-control btn-success" Text="Salvar" runat="server" OnClick="btnSalvarCFOP_Click" />
                                                    </div>
                                                </div>

                                                <div class="col-lg-12 form-group">
                                                    <uc1:TextBox_Padrao runat="server" ID="txtInfoFisco" Titulo="Informações Adicionais ao Fisco" Modo="Multilinha" Rows="3" />
                                                </div>

                                                <div class="col-lg-12 form-group">
                                                    <uc1:TextBox_Padrao runat="server" ID="txtInfoComplementares" Titulo="Informações Complementares" Modo="Multilinha" Rows="3" />
                                                </div>

                                                <div class="col-lg-2">
                                                    <uc1:TextBox_Padrao runat="server" ID="txtxPed" Titulo="xPed" Validacao="true" Obrigatorio="true" />
                                                </div>

                                            </div>
                                        </div>

                                    </div>

                                </div>

                                <div role="tabpanel" class="tab-pane fade" id="ItensDevolucao" aria-labelledby="ItensDevolucao">

                                    <div class="panel panel-default">
                                        <div class="panel-heading">
                                            <h3 class="panel-title"><b>Itens Devolução</b></h3>
                                        </div>
                                        <div class="panel-body">
                                            <div class="form-stacked row">
                                                <div class="col-lg-12 form-group" runat="server" id="DIV_ItensDevolucao">
                                                    <asp:GridView ID="gvItensDevolucao" class="table table-striped table-bordered table-hover table-condensed"
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                        HeaderStyle-CssClass="gvChildHeader" CssClass="gvRow" Style="padding: 0; margin: 0" AlternatingRowStyle-CssClass="gvAltRow"
                                                        Font-Names="Tahoma" Font-Overline="False" DataKeyNames="idEnvioOPI" OnRowDataBound="gvItensDevolucao_RowDataBound">
                                                        <Columns>

                                                            <asp:BoundField DataField="idProduto" HeaderText="ID">
                                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="idProduto" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="Ordem" HeaderText="Ordem">
                                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="idProduto" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="sCodigo" HeaderText="Código">
                                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="sDscProduto" HeaderText="Descrição">
                                                                <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="sUnidade" HeaderText="Unidade">
                                                                <ItemStyle Width="4%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade">
                                                                <ItemStyle Width="7%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:TemplateField HeaderText="Qtd devolução">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtnQuantidadeEnvio" CssClass="form-control" runat="server" TextMode="Number" min="0" step="1" />
                                                                </ItemTemplate>
                                                                <HeaderStyle Width="10%" />
                                                            </asp:TemplateField>

                                                            <asp:BoundField DataField="sCodigoBarras" HeaderText="sCodigoBarras">
                                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="nValorUnitario" HeaderText="Valor Unitário">
                                                                <ItemStyle Width="10%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="nVlrTotal" HeaderText="Valor Total">
                                                                <ItemStyle Width="10%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:TemplateField HeaderText="Número Pedido Cliente">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtnPedidoCliente" class="form-control" runat="server" MaxLength="60" Text='<%# Bind("nPedidoCliente") %>'></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>

                                <div role="tabpanel" class="tab-pane fade" id="Impostos" aria-labelledby="Impostos-tab">

                                    <div class="row">
                                        <div class="col-lg-12">
                                            <asp:GridView ID="gvItensImpostos" class="table table-striped table-bordered table-hover "
                                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvItensImpostos_RowDataBound">
                                                <Columns>

                                                    <asp:BoundField DataField="ID" HeaderText="ID">
                                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="idProduto" />
                                                    </asp:BoundField>

                                                    <asp:TemplateField HeaderText="Impostos">
                                                        <ItemTemplate>
                                                            <div class="btn-group">
                                                                <button type="button" class="btn btn-sm btn-light dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" title="Ver impostos">
                                                                    <i class="fa fa-chevron-down"></i>
                                                                </button>
                                                                <ul class="dropdown-menu p-2" style="width: 1000px;">
                                                                    <table class="table table-striped table-condensed table-bordered">
                                                                        <tbody>
                                                                            <tr>
                                                                                <td><b>ICMS</b></td>
                                                                                <td><b>orig:</b> <%# Eval("orig") %></td>
                                                                                <td><b>CST:</b>
                                                                                    <asp:Label ID="lblCST" runat="server" Text='<%# Eval("CST") %>' />
                                                                                </td>
                                                                                <td><b>modBC:</b> <%# Eval("modBC") %></td>
                                                                                <td id="pRedBC" runat="server"><b>pRedBC:</b> <%# Eval("pRedBC") %></td>
                                                                                <td><b>vBC:</b> <%# Eval("vBC") %></td>
                                                                                <td><b>pICMS:</b> <%# Eval("pICMS") %> %</td>
                                                                                <td><b>vICMS:</b> <%# Eval("vICMS") %></td>
                                                                                <td id="modBCST" runat="server"><b>modBCST:</b> <%# Eval("modBCST") %></td>
                                                                                <td id="pMVAST" runat="server"><b>pMVAST:</b> <%# Eval("pMVAST") %></td>
                                                                                <td id="vBCSTICMS" runat="server"><b>vBCST:</b> <%# Eval("vBCSTICMS") %></td>
                                                                                <td id="pICMSST" runat="server"><b>pICMSST:</b> <%# Eval("pICMSST") %></td>
                                                                                <td id="vICMSST" runat="server"><b>vICMSST:</b> <%# Eval("vICMSST") %></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td><b>IPI</b></td>
                                                                                <td><b>cEnq:</b> <%# Eval("cEnq") %></td>
                                                                                <td><b>CST:</b>
                                                                                    <asp:Label ID="lblCSTIPI" runat="server" Text='<%# Eval("CSTIPI") %>' />
                                                                                </td>
                                                                                <td><b>vBC:</b> <%# Eval("vBCIPI") %></td>
                                                                                <td><b>pIPI:</b> <%# Eval("pIPI") %> %</td>
                                                                                <td><b>vIPI:</b> <%# Eval("vIPI") %></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td><b>PIS</b></td>
                                                                                <td><b>CST:</b>
                                                                                    <asp:Label ID="lblCSTPIS" runat="server" Text='<%# Eval("CSTPIS") %>' />
                                                                                </td>
                                                                                <td><b>vBC:</b> <%# Eval("vBCPIS") %></td>
                                                                                <td><b>pPIS:</b> <%# Eval("pPIS") %> %</td>
                                                                                <td><b>vPIS:</b> <%# Eval("vPIS") %> </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td><b>COFINS</b></td>
                                                                                <td><b>CST:</b>
                                                                                    <asp:Label ID="lblCSTCOFINS" runat="server" Text='<%# Eval("CSTCOFINS") %>' />
                                                                                </td>
                                                                                <td><b>vBC:</b> <%# Eval("vBCCOFINS") %></td>
                                                                                <td><b>vBC:</b> <%# Eval("pCOFINS") %> %</td>
                                                                                <td><b>pIPI:</b> <%# Eval("vCOFINS") %></td>
                                                                            </tr>
                                                                            <tr id="trDIFAL" runat="server">
                                                                                <td><b>DIFAL</b></td>
                                                                                <td id="vBCUFDest" runat="server"><b>vBCUFDest:</b> <%# Eval("vBCUFDest") %></td>
                                                                                <td id="pFCPUFDest" runat="server"><b>pFCPUFDest:</b> <%# Eval("pFCPUFDest") %></td>
                                                                                <td id="pICMSUFDest" runat="server"><b>pICMSUFDest:</b> <%# Eval("pICMSUFDest") %>%</td>
                                                                                <td id="pICMSInter" runat="server"><b>pICMSInter:</b><%# Eval("pICMSInter") %></td>
                                                                                <td id="pICMSInterPart" runat="server"><b>pICMSInterPart:</b><%# Eval("pICMSInterPart") %></td>
                                                                                <td id="vFCPUFDest" runat="server"><b>vFCPUFDest:</b><%# Eval("vFCPUFDest") %></td>
                                                                                <td id="vICMSUFDest" runat="server"><b>vICMSUFDest:</b><%# Eval("vICMSUFDest") %></td>
                                                                                <td id="vICMSUFRemet" runat="server"><b>vICMSUFRemet:</b><%# Eval("vICMSUFRemet") %></td>
                                                                                <td id="sCalculoDIFAL" runat="server"><b>sCalculoDIFAL:</b><%# Eval("sCalculoDIFAL") %></td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>
                                                                </ul>
                                                            </div>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:TemplateField>

                                                    <asp:BoundField DataField="cProd" HeaderText="Código">
                                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="idProduto" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="xProd" HeaderText="Descrição">
                                                        <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="idProduto" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="cEAN" HeaderText="EAN">
                                                        <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="NCM" HeaderText="NCM">
                                                        <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="CFOP" HeaderText="CFOP">
                                                        <ItemStyle Width="4%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="uCom" HeaderText="UN">
                                                        <ItemStyle Width="4%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="qCom" HeaderText="Quantidade">
                                                        <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="vUnCom" HeaderText="Vlr Unitário">
                                                        <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="vProd" HeaderText="Vlr Total">
                                                        <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="vDesc" HeaderText="Vlr Desconto">
                                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-lg-12">
                                            <div class="panel panel-default">
                                                <div class="panel-heading">
                                                    <h3 class="panel-title"><b>Total</b></h3>
                                                </div>
                                                <div class="panel-body">
                                                    <div class="form-stacked row">
                                                        <div class="panel-body">

                                                            <div class="form-group col-lg-2">
                                                                <label>vBC</label>
                                                                <asp:TextBox ID="txtvBCtotal" runat="server" CssClass="form-control" ReadOnly="true" />
                                                            </div>
                                                            <div class="form-group col-lg-2">
                                                                <label>vICMS</label>
                                                                <asp:TextBox ID="txtvICMStotal" runat="server" CssClass="form-control" ReadOnly="true" />
                                                            </div>
                                                            <div class="form-group col-lg-2">
                                                                <label>vICMSDeson</label>
                                                                <asp:TextBox ID="txtvICMSDesontotal" runat="server" CssClass="form-control" ReadOnly="true" />
                                                            </div>
                                                            <div class="form-group col-lg-2">
                                                                <label>vFCP</label>
                                                                <asp:TextBox ID="txtvFCPtotal" runat="server" CssClass="form-control" ReadOnly="true" />
                                                            </div>
                                                            <div class="form-group col-lg-2">
                                                                <label>vBCST</label>
                                                                <asp:TextBox ID="txtvBCSTtotal" runat="server" CssClass="form-control" ReadOnly="true" />
                                                            </div>
                                                            <div class="form-group col-lg-2">
                                                                <label>vST</label>
                                                                <asp:TextBox ID="txtvSTtotal" runat="server" CssClass="form-control" ReadOnly="true" />
                                                            </div>
                                                            <div class="form-group col-lg-2">
                                                                <label>vFCPST</label>
                                                                <asp:TextBox ID="txtvFCPSTtotal" runat="server" CssClass="form-control" ReadOnly="true" />
                                                            </div>
                                                            <div class="form-group col-lg-2">
                                                                <label>vFCPSTRet</label>
                                                                <asp:TextBox ID="txtvFCPSTRettotal" runat="server" CssClass="form-control" ReadOnly="true" />
                                                            </div>
                                                            <div class="form-group col-lg-2">
                                                                <label>vProd</label>
                                                                <asp:TextBox ID="txtvProdtotal" runat="server" CssClass="form-control" ReadOnly="true" />
                                                            </div>
                                                            <div class="form-group col-lg-2">
                                                                <label>vFrete</label>
                                                                <asp:TextBox ID="txtvFretetotal" runat="server" CssClass="form-control" ReadOnly="true" />
                                                            </div>
                                                            <div class="form-group col-lg-2">
                                                                <label>vSeg</label>
                                                                <asp:TextBox ID="txtvSegtotal" runat="server" CssClass="form-control" ReadOnly="true" />
                                                            </div>
                                                            <div class="form-group col-lg-2">
                                                                <label>vDesc</label>
                                                                <asp:TextBox ID="txtvDesctotal" runat="server" CssClass="form-control" ReadOnly="true" />
                                                            </div>
                                                            <div class="form-group col-lg-2">
                                                                <label>vII</label>
                                                                <asp:TextBox ID="txtvIItotal" runat="server" CssClass="form-control" ReadOnly="true" />
                                                            </div>
                                                            <div class="form-group col-lg-2">
                                                                <label>vIPI</label>
                                                                <asp:TextBox ID="txtvIPItotal" runat="server" CssClass="form-control" ReadOnly="true" />
                                                            </div>
                                                            <div class="form-group col-lg-2">
                                                                <label>vIPIDevol</label>
                                                                <asp:TextBox ID="txtvIPIDevoltotal" runat="server" CssClass="form-control" ReadOnly="true" />
                                                            </div>
                                                            <div class="form-group col-lg-2">
                                                                <label>vPIS</label>
                                                                <asp:TextBox ID="txtvPIStotal" runat="server" CssClass="form-control" ReadOnly="true" />
                                                            </div>
                                                            <div class="form-group col-lg-2">
                                                                <label>vCOFINS</label>
                                                                <asp:TextBox ID="txtvCOFINStotal" runat="server" CssClass="form-control" ReadOnly="true" />
                                                            </div>
                                                            <div class="form-group col-lg-2">
                                                                <label>vOutro</label>
                                                                <asp:TextBox ID="txtvOutrototal" runat="server" CssClass="form-control" ReadOnly="true" />
                                                            </div>
                                                            <div class="col-lg-2">
                                                                <label>vNF</label>
                                                                <asp:TextBox ID="txtvNFtotal" runat="server" CssClass="form-control" ReadOnly="true" />
                                                            </div>
                                                            <div class="col-lg-2">
                                                                <label>vTotTrib</label>
                                                                <asp:TextBox ID="txtvTotTribtotal" runat="server" CssClass="form-control" ReadOnly="true" />
                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>

                                <div class="modal fade" id="modal_SalvarFaturamento">
                                    <div class="modal-dialog modal-pequeno">
                                        <div class="modal-content">
                                            <div class="modal-header">
                                                <h5 class="modal-title">Gerar Faturamento</h5>
                                            </div>
                                            <div class="modal-body">
                                                <asp:Label ID="lblTitulo_SalvarFaturamento" runat="server" Text="" Font-Bold="true"></asp:Label>
                                            </div>
                                            <div class="modal-footer">
                                                <asp:LinkButton ID="cmdSalvarFaturamento" CssClass="btn btn-success" Text="Sim" runat="server" OnClick="cmdSalvarFaturamento_Click" />
                                                <asp:LinkButton CssClass="btn btn-danger" Text="Não" runat="server" OnClientClick="$('#modal_SalvarFaturamento').modal('hide'); return false;" />
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>

                            <div class="modal-footer">
                                <asp:LinkButton ID="cmdGerarFaturamento" CssClass="btn btn-success" Text="Gerar Faturamento" runat="server" OnClientClick="$('#modal_SalvarFaturamento').modal('show'); return false;" />
                                <asp:LinkButton ID="cmdSalvarEdicao" CssClass="btn btn-success" Text="Salvar" runat="server" OnClick="cmdSalvarEdicao_Click" />
                                <asp:LinkButton ID="btnSalvarDevolucao" CssClass="btn btn-success" Text="Efetuar Devolução" runat="server" OnClick="btnSalvarDevolucao_Click" />
                                <asp:LinkButton ID="cmdEmitirNFS" CssClass="btn btn-success" Text="Emitir NFs" runat="server" OnClick="cmdEmitirNFS_Click" />
                                <asp:LinkButton CssClass="btn btn-danger" Text="Cancelar" runat="server" data-dismiss="modal" />
                            </div>

                        </div>
                    </div>

                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="cmdSalvarFaturamento" />
                    <asp:PostBackTrigger ControlID="cmdSalvarEdicao" />
                    <asp:PostBackTrigger ControlID="btnSalvarDevolucao" />
                    <asp:PostBackTrigger ControlID="cmdEmitirNFS" />
                </Triggers>
            </asp:UpdatePanel>

        </div>
    </div>

    <div class="modal fade" id="modal_EnviarSefaz" tabindex="-1" data-backdrop="static" role="dialog">
        <div class="modal-dialog modal-pequeno">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Enviar para Sefaz</h5>
                </div>
                <div class="modal-body">
                    <asp:Label ID="lblTitulo_EnviarSefaz" runat="server" Text="" Font-Bold="true"></asp:Label>
                </div>
                <div class="modal-footer">
                    <asp:LinkButton ID="cmdEnviarSefaz" CssClass="btn btn-success" Text="Sim" runat="server" OnClick="DocumentosFiscais_cmdEnviarSefaz_Click" />
                    <asp:LinkButton CssClass="btn btn-danger" Text="Não" runat="server" OnClientClick="$('#modal_EnviarSefaz').modal('hide'); return false;" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="Modal_Cartacorrecao" tabindex="-1" data-backdrop="static" role="dialog">
        <div class="modal-dialog modal-dialog-centered modal-largo" role="document">
            <div class="modal-content">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <div class="modal-header-content">
                            <div class="modal-logo">
                                <asp:Image ID="Image2" runat="server" ImageUrl="~/App/img/instrucaoTecnica_pdf.jpg" AlternateText="Instrução Técnica" />
                            </div>

                            <div class="modal-title-container">
                                <asp:Label runat="server" class="modal-title" ID="Label1" Text="Carta de Correção" Font-Bold="true"></asp:Label>
                            </div>

                            <button type="button" id="btnFecharCartacorrecao" class="close" data-dismiss="modal" aria-label="Fechar" style="margin-right: 10px;">
                                <span aria-hidden="true">&times;</span>
                            </button>

                            <br />
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina3" />
                                </div>

                                <div class="col-lg-12 form-group">
                                    <uc1:TextBox_Padrao runat="server" ID="txtCorrecao" Titulo="Correção" Validacao="true" Obrigatorio="true" Modo="Multilinha" MaxLength="1000" Rows="5" />
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div class="modal-footer text-left">
                    <asp:LinkButton ID="btnSalvaCorrecao" CssClass="btn btn-success" Text="Salvar" runat="server" OnClick="btnSalvaCorrecao_Click" />
                    <asp:LinkButton CssClass="btn btn-danger" Text="Cancelar" runat="server" data-dismiss="modal" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="Modal_Cancelamento">
        <div class="modal-dialog modal-largo">
            <div class="modal-content">

                <asp:UpdatePanel ID="UpdModal_Cancelamento" runat="server">
                    <ContentTemplate>

                        <div class="modal-header">
                            <div class="modal-title">
                                <img src="../../../img/instrucaoTecnica_pdf.jpg" alternatetext="Logo TT" class="modal-logo" />
                                <label>Cancelar NF</label>
                                <button type="button" id="btnFecharCancelamento" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span></button>
                            </div>
                        </div>

                        <div class="modal-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Modal_Cancelamento" />
                                </div>

                                <div class="col-lg-12">
                                    <uc1:TextBox_Padrao runat="server" ID="txtsJustificativa" Titulo="Justificativa de Cancelamento" Validacao="true" Obrigatorio="true" Modo="Multilinha" MaxLength="255" Rows="5" />
                                </div>
                            </div>
                        </div>

                        <div class="modal-footer">
                            <asp:LinkButton runat="server" ID="btnSalvarCancelamento" CssClass="btn btn-success" Text="Salvar" OnClick="btnSalvarCancelamento_Click" />
                            <asp:LinkButton runat="server" CssClass="btn btn-danger" Text="Cancelar" data-dismiss="modal" />
                        </div>

                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btnSalvarCancelamento" />
                    </Triggers>
                </asp:UpdatePanel>

            </div>
        </div>
    </div>

    <div class="modal fade" id="modalImportarRetorno_NFS">
        <div class="modal-dialog modal-pequeno">
            <div class="modal-content">
                <div class="modal-header">
                    <h3 class="modal-title">Importar Retorno NFS-e</h3>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">

                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_ModalImportarRetorno_NFS" />

                    <div class="row">
                        <div class="col-lg-12">
                            <div class="upload-container">
                                <div class="file-upload-label" id="uploadContainer">
                                    <svg viewBox="0 0 640 512" height="1em">
                                        <path d="M144 480C64.5 480 0 415.5 0 336c0-62.8 40.2-116.2 96.2-135.9c-.1-2.7-.2-5.4-.2-8.1c0-88.4 71.6-160 160-160c59.3 0 111 32.2 138.7 80.2C409.9 102 428.3 96 448 96c53 0 96 43 96 96c0 12.2-2.3 23.8-6.4 34.6C596 238.4 640 290.1 640 352c0 70.7-57.3 128-128 128H144zm79-217c-9.4 9.4-9.4 24.6 0 33.9s24.6 9.4 33.9 0l39-39V392c0 13.3 10.7 24 24 24s24-10.7 24-24V257.9l39 39c9.4 9.4 24.6 9.4 33.9 0s9.4-24.6 0-33.9l-80-80c-9.4-9.4-24.6-9.4-33.9 0l-80 80z"></path>
                                    </svg>
                                    <p>Arraste e solte aqui o Arquivo</p>
                                    <p>ou</p>
                                    <span class="browse-button">Escolher Arquivo</span>
                                    <asp:FileUpload ID="fu_ImportarRetorno_NFS" runat="server" class="invisivel" accept=".txt" />
                                </div>
                            </div>
                            <div class="file-name" id="fileName"></div>
                        </div>
                    </div>

                </div>
                <div class="modal-footer">
                    <asp:LinkButton ID="btnImportarRetorno" CssClass="btn btn-md btn-success" runat="server" Text="Importar" OnClick="btnImportarRetorno_Click" />
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Fechar</button>
                </div>
            </div>
        </div>
    </div>

    <div id="hdd">
        <asp:HiddenField ID="hddidFaturamento" runat="server" />
        <asp:HiddenField ID="hddidEmpresa" runat="server" />
        <asp:HiddenField ID="hddnNumeroPedido" runat="server" />

        <asp:HiddenField ID="hddidXML" runat="server" />
        <asp:HiddenField ID="hddidCondicaoPagamento" runat="server" />
        <asp:HiddenField ID="hddidCliente" runat="server" />
        <asp:HiddenField ID="hddidMeioRecebimento" runat="server" />
        <asp:HiddenField ID="hddidContabil" runat="server" />
        <asp:HiddenField ID="hddidOPI" runat="server" />
        <asp:HiddenField ID="hddidPedido" runat="server" />
        <asp:HiddenField ID="hddidEnvioOPI" runat="server" />
        <asp:HiddenField ID="hddStatusModal" runat="server" />
        <asp:HiddenField ID="hddsFaturamento" runat="server" Value="N" />
        <asp:HiddenField ID="hddsEmpreitada" runat="server" Value="N" />
        <asp:HiddenField ID="hddsGerarNFe" runat="server" Value="N" />
        <asp:HiddenField ID="hddidArquivo" runat="server" />
        <asp:HiddenField ID="hddsCaminho_UniNFe" runat="server" />
        <asp:HiddenField ID="hddsChave" runat="server" />
        <asp:HiddenField ID="hddidEnvioEmail" runat="server" />
        <asp:HiddenField ID="hddsCaminho" runat="server" />
        <asp:HiddenField ID="hddchaveNFe" runat="server" />
        <asp:HiddenField ID="hddmod" runat="server" />
        <asp:HiddenField ID="hddserie" runat="server" />
        <asp:HiddenField ID="hddnNF" runat="server" />
        <asp:HiddenField ID="hddnCartaCorrecao" runat="server" />
        <asp:HiddenField ID="hddidEnvioOPI_idFaturamento" runat="server" />
        <asp:HiddenField ID="hddvlrFaturamento" runat="server" />
        <asp:HiddenField ID="hddidFaturamento_Cancela" runat="server" />
        <asp:HiddenField ID="hddServico" runat="server" />

        <asp:HiddenField ID="hddValor_Faturar" runat="server" Value="0" />
        <asp:HiddenField ID="hddFator_IR" runat="server" Value="0" />
        <asp:HiddenField ID="hddFator_ISS" runat="server" Value="0" />
        <asp:HiddenField ID="hddFator_INSS" runat="server" Value="0" />
        <asp:HiddenField ID="hddFator_CSLL" runat="server" Value="0" />
        <asp:HiddenField ID="hddFator_PIS" runat="server" Value="0" />
        <asp:HiddenField ID="hddFator_COFINS" runat="server" Value="0" />
    </div>

</asp:Content>