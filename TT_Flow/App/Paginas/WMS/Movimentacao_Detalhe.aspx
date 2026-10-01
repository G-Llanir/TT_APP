<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Movimentacao_Detalhe.aspx.cs" EnableViewState="true" EnableEventValidation="false" Inherits="TT_Flow.App.Paginas.WMS.Movimentacao.Movimentacao_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/FiltroPesquisa.ascx" TagPrefix="uc1" TagName="FiltroPesquisa" %>
<%@ Register Src="~/App/Controles/Pesquisa_Parceiros.ascx" TagPrefix="uc1" TagName="Pesquisa_Parceiros" %>
<%@ Register Src="~/App/Controles/ImportadorItensModal.ascx" TagPrefix="uc1" TagName="ImportadorItensModal" %>
<%@ Register Src="~/App/Controles/LeitorQuagga.ascx" TagPrefix="uc1" TagName="LeitorQuagga" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        .chosen-container .chosen-drop {
            z-index: 99999 !important;
        }

        .Calendario .ajax__calendar_container {
            border: 1px solid #646464;
            background-color: lemonchiffon;
            color: red;
        }

        .text-navy {
            color: #1ab394;
        }

        .cart-product-imitation {
            text-align: center;
            background-color: #f8f8f9;
            display: flex;
            justify-content: center;
            align-items: center;
        }

            .cart-product-imitation img {
                max-width: 100%;
                max-height: 100%;
            }

        .product-imitation.xl {
            padding: 120px 0;
        }

        .product-desc {
            padding: 20px;
            position: relative;
        }

        table.shoping-cart-table {
            margin-bottom: 0;
        }

            table.shoping-cart-table tr td {
                border: none;
                text-align: right;
            }

                table.shoping-cart-table tr td.desc,
                table.shoping-cart-table tr td:first-child {
                    text-align: left;
                }

                table.shoping-cart-table tr td:last-child {
                    width: 80px;
                }

        .table.shoping-cart-table tr td {
            padding: 10px; /* ou qualquer valor de espaçamento que desejar */
        }

            /* Se desejar espaçamento específico para uma coluna em particular */
            .table.shoping-cart-table tr td:nth-child(3) {
                padding-left: 20px; /* Especifica o espaçamento apenas para a terceira coluna */
            }

        .ibox {
            clear: both;
            margin-bottom: 25px;
            margin-top: 0;
            padding: 0;
        }

            .ibox.collapsed .ibox-content {
                display: none;
            }

            .ibox:after,
            .ibox:before {
                display: table;
            }

        .ibox-title {
            -moz-border-bottom-colors: none;
            -moz-border-left-colors: none;
            -moz-border-right-colors: none;
            -moz-border-top-colors: none;
            background-color: #ffffff;
            border-color: #e7eaec;
            border-image: none;
            border-style: solid solid none;
            border-width: 3px 0 0;
            color: inherit;
            margin-bottom: 0;
            padding: 14px 15px 7px;
            min-height: 48px;
        }

            .ibox-title h5 {
                font-size: 18px;
                font-weight: bold;
                color: #1ab394; /* Tom de verde para combinar com o tema navy */
                text-transform: uppercase;
            }

        .ibox-content {
            border: 1px solid #e7eaec;
            background-color: #f9f9f9;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0px 2px 4px rgba(0, 0, 0, 0.1);
        }

        .table.shoping-cart-table tbody tr {
            border-bottom: 1px solid #ddd;
        }

        .text-muted {
            color: #888;
            font-size: 14px;
            text-align: center;
            margin-top: 15px;
        }

        .removed-item {
            background-color: #f8d7da;
        }

        .ibox-content {
            border: 1px solid #ddd;
            padding: 10px;
        }

        /*modal*/
        .modal-header-content {
            display: flex;
            align-items: center;
        }

        .modal-logo img {
            max-width: 50px;
            margin-right: 10px;
        }

        .modal-title-container {
            flex-grow: 1;
        }

        .modal-title {
            color: #009A22;
            text-shadow: 2px 2px 2px rgba(0, 0, 0, 0.2);
            font-size: 24px;
            font-weight: bold;
            margin: 0;
        }

        .dropdown-menu {
            z-index: 1050 !important;
            position: absolute !important;
        }

        .table-custom {
            width: 100%;
            border-collapse: collapse;
            background-color: #f9f9f9;
        }

            .table-custom th, .table-custom td {
                padding: 10px;
                text-align: left;
                border: 1px solid #ddd;
            }

        .th-custom {
            background-color: #024e0a;
            color: white;
            font-weight: bold;
            text-align: center;
        }

        .td-custom label {
            display: block; /* Garante que o label ocupe o espaço inteiro da célula */
            overflow: hidden; /* Impede que o texto transborde */
            text-overflow: ellipsis; /* Adiciona "..." caso o texto seja muito grande */
            white-space: nowrap; /* Impede a quebra de linha automática */
            max-width: 100%; /* Mantém dentro do limite da célula */
        }

        .td-custom {
            background-color: #ffffff;
            color: #333;
        }

            .td-custom:nth-child(even) {
                background-color: #f2f2f2;
            }

            .td-custom:hover {
                background-color: #f9f9f9;
                transition: background-color 0.3s ease;
            }

        @media (max-width: 768px) {
            .table-responsive-custom {
                font-size: 12px;
            }

            .table-custom th, .table-custom td {
                padding: 8px;
            }
        }

        .modal-content {
            overflow: visible !important; /* Não permite scroll e o conteúdo se expande normalmente */
        }

        .modal-xl {
            max-width: 90%;
            width: 90%;
        }

        .ui-autocomplete {
            z-index: 999999 !important;
            max-height: 300px;
            overflow-y: auto;
            overflow-x: hidden;
            background-color: #fff;
        }

        /* ==========================================
           OTIMIZAÇÕES PARA CELULAR (MOBILE)
           ========================================== */
        @media (max-width: 768px) {
            /* Força a modal a caber na tela do celular e permite scroll interno */
            .modal-dialog {
                margin: 10px !important;
                width: auto !important;
            }

            .modal-xl {
                max-width: 100% !important;
                width: auto !important;
            }

            .modal-body {
                padding: 10px;
            }

            /* Habilita o arrastar horizontal suave em QUALQUER grid que estourar */
            .table-responsive-mobile {
                display: block !important;
                width: 100% !important;
                overflow-x: auto !important;
                -webkit-overflow-scrolling: touch; /* Rolagem suave no iOS */
            }

            /* Quebra palavras gigantes (como código de barras) para não empurrar a tela */
            .table-custom td, .table td {
                word-break: break-all !important;
                white-space: normal !important;
            }

            /* Empilha os botões de ação para não ficarem espremidos ou fora da tela */
            .acoes-mobile {
                flex-direction: column !important;
                align-items: stretch !important;
            }

                .acoes-mobile .btn {
                    margin-bottom: 5px !important;
                    width: 100% !important;
                    padding: 10px 5px !important;
                    font-size: 12px !important;
                }
        }
    </style>

    <div class="form-stacked row" style="">
        <div class="col-lg-12">
            <h1>
                <asp:Label CssClass="resumir-label" ID="lblTituloPagina" runat="server" Text="Movimentação"></asp:Label>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="3" TitulodaPagina="Detalhe" />

            <div class="row">
                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaInfos" />
                </div>
            </div>
        </div>
    </div>

    <asp:UpdatePanel ID="UpdDetalhe" runat="server">
        <ContentTemplate>

            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Movimentação de Produtos</b></h3>
                </div>
                <div class="panel-body">

                    <div class="row">
                        <uc1:MensagemPagina runat="server" ID="MensagemPaginaErro" />
                    </div>

                    <div class="row">
                        <div class="col-lg-2 form-group">
                            <label>ID</label>
                            <asp:TextBox ID="txtidMovimentacao" runat="server" class="form-control" disabled="" placeholder="Novo"></asp:TextBox>
                        </div>

                        <div class="col-lg-4">
                            <h3><span runat="server" id="spanStatus" class="label label-info">Nova Movimentação</span></h3>
                        </div>
                    </div>

                    <div class="row">

                        <div class="col-lg-2">
                            <div class="form-group">
                                <label>Data Movimentação</label>
                                <asp:TextBox ID="txtdtMovimentacao" runat="server" type="date" class="form-control" disabled="true"></asp:TextBox>
                            </div>
                        </div>

                        <asp:UpdatePanel UpdateMode="Conditional" ID="updp_TipoMovimentacao_x_Motivo" runat="server">
                            <ContentTemplate>
                                <div class="col-lg-3 form-group">
                                    <label>Tipo da Movimentação </label>
                                    <asp:DropDownList ID="ddlsTipoMovimentacao" class="form-control" runat="server" ValidationGroup="item" AutoPostBack="true" OnSelectedIndexChanged="ddlsTipoMovimentacao_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                                <div class="col-lg-3 form-group" id="div_Motivo" runat="server" visible="false">
                                    <label>Motivo </label>
                                    <asp:DropDownList ID="ddlMotivo" class="form-control" runat="server" ValidationGroup="item" AutoPostBack="true" OnSelectedIndexChanged="ddlMotivo_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                                <div class="col-lg-2 form-group" id="div_Lote" runat="server" visible="false">
                                    <asp:CheckBox ID="chkAlternar" runat="server" Text=" " CssClass="form-check-input" OnCheckedChanged="chkAlternar_CheckedChanged"
                                        AutoPostBack="true" />
                                    <label>
                                        Importar PO
                                    </label>
                                    <asp:TextBox ID="txtsPedidoCompra" runat="server" class="form-control" MaxLength="10"></asp:TextBox>
                                    <asp:DropDownList ID="ddlsPO" runat="server" AutoPostBack="True" class="form-control yes_no select Caixa_Selecao" Visible="false" OnSelectedIndexChanged="ddlsPO_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlsTipoMovimentacao" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>

                        <div id="div_Form" runat="server">
                            <div class="col-lg-2" id="div_Referencia" runat="server">
                                <div class="form-group">
                                    <label>Nota Fiscal</label>
                                    <asp:TextBox ID="txtsNotaFiscal" runat="server" class="form-control" MaxLength="20" placeholder=""></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-2" id="div_ImportComex" runat="server" visible="false">
                                <div class="form-group">
                                    <label>&nbsp</label>
                                    <br />
                                    <asp:Button ID="cmdImportar" runat="server" Text="Importar" class="btn btn-sm btn-primary" OnClick="cmdImportar_Click" />
                                </div>
                            </div>

                            <div class="col-lg-4" id="DIV_Fornecedor" runat="server">
                                <div class="form-group">
                                    <label id="lblFornecedor" runat="server">Fornecedor</label>
                                    <asp:DropDownList ID="ddlFornecedor" runat="server" AutoPostBack="True" class="form-control yes_no select Caixa_Selecao" OnSelectedIndexChanged="ddlFornecedor_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>
                            <div id="div_pesquisaParceiros" runat="server" class="col-lg-10" visible="false">
                                <uc1:Pesquisa_Parceiros runat="server" ID="Pesquisa_Parceiros" />
                            </div>
                            <div id="div_Impressora" runat="server" class="col-lg-2" visible="false">
                                <div class="form-group">
                                    <label id="lblImpressora" runat="server">Etiqueta</label>
                                    <asp:DropDownList ID="ddlImpressora" runat="server" AutoPostBack="False" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <div id="div_Descricao" runat="server" class="col-lg-12">
                            <label>Descrição</label>
                            <asp:TextBox ID="txtsDscObservacao" class="form-control" runat="server" MaxLength="400" TextMode="MultiLine" Rows="5"></asp:TextBox>
                        </div>

                    </div>

                </div>
            </div>

            <asp:UpdatePanel ID="UpdImportadorItensModal" runat="server">
                <ContentTemplate>
                    <uc1:ImportadorItensModal runat="server" ID="ImportadorItensModal" OnItensImportados="ImportadorItensModal_ItensImportados" OpcoesVisiveis="COMEX" />
                </ContentTemplate>
            </asp:UpdatePanel>

            <div class="panel panel-default" runat="server" id="divImportacaoXML" visible="false">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-upload"></i><b>Importação de XML (Nota Fiscal)</b></h3>
                </div>
                <div class="panel-body">

                    <div class="row" id="divUploadArea" runat="server">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>Selecione o arquivo XML da NFe</label>
                                <asp:FileUpload ID="fupArquivoXML" runat="server" CssClass="form-control" />
                            </div>
                        </div>

                        <div class="col-lg-1">
                            <div class="form-group">
                                <label>&nbsp;</label>
                                <asp:Button ID="btnProcessarXML" runat="server" Text="Importar" class="btn btn-primary form-control" OnClick="btnProcessarXML_Click" />
                            </div>
                        </div>

                    </div>

                    <br />

                    <div class="row" runat="server" id="Div_painel_parceiro_xml" visible="false">
                        <div class="col-lg-12">
                            <div class="alert alert-info" role="alert" style="margin-bottom: 15px;">
                                <div class="row">
                                    <div class="col-md-3">
                                        <strong>CNPJ Emitente (XML):</strong><br />
                                        <asp:Label ID="lblCNPJEmit" runat="server" Text="-" Font-Bold="true"></asp:Label>
                                    </div>
                                    <div class="col-md-5">
                                        <strong>Razão Social (XML):</strong><br />
                                        <asp:Label ID="lblNomeEmit" runat="server" Text="-" Font-Bold="true"></asp:Label>
                                    </div>
                                    <div class="col-md-2">
                                        <strong>Nº Nota:</strong><br />
                                        <asp:Label ID="lblNumNFe" runat="server" Text="-" Font-Bold="true"></asp:Label>
                                    </div>
                                    <div class="col-md-2 text-right">
                                        <small>Se o fornecedor abaixo estiver errado, altere-o manualmente.</small>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-12">
                            <div class="">
                                <asp:GridView ID="dtgVinculoXML" runat="server"
                                    CssClass="table table-striped table-bordered table-hover"
                                    AutoGenerateColumns="false"
                                    DataKeyNames="cProd"
                                    OnRowDataBound="dtgVinculoXML_RowDataBound">
                                    <Columns>

                                        <%-- A GRANDE MÁGICA ACONTECE AQUI: O COMBO PARA VINCULAR --%>
                                        <%--<asp:TemplateField HeaderText="Produto Interno (Tec And Tec)" HeaderStyle-CssClass="th-custom" ItemStyle-BackColor="#fff3cd">
                                            <ItemTemplate>

                                                <asp:TextBox ID="txtProdutoNfe" runat="server"
                                                    CssClass="form-control css-autocomplete"
                                                    placeholder="Digite para buscar...">
                                                </asp:TextBox>

                                                <asp:HiddenField ID="hddIdProdutoVinculado" runat="server"
                                                    Value='<%# Eval("idProdutoSistema") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>

                                        <asp:TemplateField HeaderText="Produto Interno (Tec And Tec)" HeaderStyle-CssClass="th-custom" ItemStyle-BackColor="#fff3cd">
                                            <ItemTemplate>
                                                <%-- MODO EDIÇÃO: Aparece se não tiver vínculo --%>
                                                <asp:Panel ID="pnlBuscaProduto" runat="server">
                                                    <asp:TextBox ID="txtProdutoNfe" runat="server"
                                                        CssClass="form-control css-autocomplete"
                                                        placeholder="Digite para buscar...">
            </asp:TextBox>
                                                </asp:Panel>

                                                <%-- MODO VISUALIZAÇÃO: Aparece se JÁ tiver vínculo --%>
                                                <asp:Panel ID="pnlProdutoVinculado" runat="server" Visible="false">
                                                    <asp:Label ID="lblProdutoVinculado" runat="server" Font-Bold="true" ForeColor="#006400"></asp:Label>
                                                    <asp:LinkButton ID="lnkRemoverVinculo" runat="server" ToolTip="Alterar Vínculo" OnClick="lnkRemoverVinculo_Click" Style="margin-left: 5px; color: red;">
                <i class="fa fa-times"></i>
            </asp:LinkButton>
                                                </asp:Panel>

                                                <%-- Campo Oculto com o ID do Produto --%>
                                                <asp:HiddenField ID="hddIdProdutoVinculado" runat="server" Value='<%# Eval("idProdutoSistema") %>' ClientIDMode="Static" />
                                                <%-- Campo Oculto com a Descrição do Produto (para preencher o label se já vier do banco) --%>
                                                <asp:HiddenField ID="hddDescProdutoVinculado" runat="server" Value='<%# Eval("xProdSistema") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="cProd" HeaderText="Cód. Fornecedor" HeaderStyle-CssClass="th-custom" />
                                        <asp:BoundField DataField="xProd" HeaderText="Descrição na NFe" HeaderStyle-CssClass="th-custom" />
                                        <asp:BoundField DataField="NCM" HeaderText="NCM" HeaderStyle-CssClass="th-custom" />
                                        <asp:BoundField DataField="uCom" HeaderText="UN" HeaderStyle-CssClass="th-custom" />

                                        <asp:TemplateField HeaderText="Qtd" HeaderStyle-CssClass="th-custom">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtdXML" runat="server" Text='<%# Bind("qCom", "{0:N4}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Valor Unit." HeaderStyle-CssClass="th-custom">
                                            <ItemTemplate>
                                                <asp:Label ID="lblVlrXML" runat="server" Text='<%# Bind("vUnCom", "{0:N4}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Total" HeaderStyle-CssClass="th-custom">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTotalXML" runat="server" Text='<%# Bind("vProd", "{0:N2}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>

                </div>
            </div>

            <div class="panel panel-default" runat="server" id="divItens">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Itens</b></h3>
                </div>
                <div class="panel-body">

                    <div class="row">
                        <div class="form-group">
                            <div class="col-lg-12">
                                <uc1:MensagemPagina runat="server" ID="MensagemPaginaItens" />
                            </div>
                            <asp:UpdatePanel runat="server">
                                <ContentTemplate>

                                    <div class="col-lg-12" id="divVisibleItens" runat="server">
                                        <div class="col-lg-10 padd-0">
                                            <uc1:FiltroPesquisa runat="server" ID="FiltroPesquisa" sRegistraUnidade="S" />
                                        </div>

                                        <div id="divPedidoCompra" runat="server" class="col-lg-1" visible="false">
                                            <br />
                                            <div class="form-group">
                                                <label>Lote(Global)</label>
                                                <asp:TextBox ID="txtsLote" runat="server" class="form-control"></asp:TextBox>
                                            </div>
                                        </div>

                                        <br />

                                        <div id="divPedidoCompra2" runat="server" class="col-lg-1">
                                            <label>&nbsp;</label>
                                            <asp:Button ID="cmdIncluir" runat="server" class="btn btn-info form-control" Text="Incluir" OnClick="cmdIncluir_Click" />
                                        </div>
                                    </div>

                                    <div id="divSerie" class="col-lg-2" runat="server" visible="false">
                                        <div class="form-group">
                                            <label>Nº Série</label>
                                            <asp:TextBox ID="txtnSerie" runat="server" class="form-control"></asp:TextBox>
                                        </div>
                                    </div>

                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>

                    <br />

                    <asp:UpdatePanel ID="updDtgItens" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="table-responsive table-responsive-mobile">
                                <asp:GridView ID="dtgItens" class="table table-striped table-bordered table-hover table-condensed m-0"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="true" Font-Names="Tahoma" Font-Overline="False" Font-Size="12px" DataKeyNames="idItem"
                                    OnRowDataBound="dtgItens_RowDataBound" OnRowDeleting="dtgItens_RowDeleting">
                                    <Columns>

                                        <asp:BoundField DataField="idProduto" HeaderText="ID">
                                            <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sCodigo" HeaderText="Código" ItemStyle-Width="10%" />

                                        <asp:TemplateField HeaderText="Descrição">
                                            <ItemTemplate>
                                                <div>
                                                    <asp:LinkButton ID="lnkProdutoDetalhe" runat="server" Text='<%# Eval("sDscProduto") %>'
                                                        OnClientClick='<%# "openProductDetail(\"" + Eval("idProduto") + "\"); return false;" %>'
                                                        OnMouseOver='<%# "mostraCard(this, \"" + Eval("idProduto") + "\", \"ProdutoModal\");" %>'
                                                        OnMouseOut='<%# "escondeCard( \"" + Eval("idProduto") + "\", \"ProdutoModal\");" %>'
                                                        CssClass="produto-detalhe-link sDscProduto"
                                                        ClientIDMode="Static"
                                                        data-idproduto='<%# Eval("idProduto") %>'
                                                        data-tabela="ProdutoModal" />

                                                    <div id='<%# Eval("idProduto") + "_ProdutoModal" %>' class="product-card" style="display: none;">
                                                        <!--conteudo via script -->
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="TipoProduto" HeaderText="Tipo de Produto">
                                            <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="SUnidade" HeaderText="Unidade">
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:TemplateField HeaderText="Quantidade">
                                            <ItemTemplate>
                                                <asp:TextBox ID="Itens_txtnQuantidade" oninput="formatNumber(this)" runat="server" class="form-control" MaxLength="20" Text='<%# Bind("NQuantidade","{0:N4}") %>'></asp:TextBox>
                                                <asp:TextBox ID="txtIdProduto" runat="server" class="form-control" MaxLength="20" Text='<%# Bind("idProduto") %>' Visible="false"></asp:TextBox>
                                                <asp:TextBox ID="txt1" runat="server" class="form-control" MaxLength="20" Text='<%# Bind("SLote") %>' Visible="false"></asp:TextBox>
                                                <asp:TextBox ID="txt2" runat="server" class="form-control" MaxLength="20" Text='<%# Bind("SDscProduto") %>' Visible="false"></asp:TextBox>
                                                <asp:TextBox ID="txt3" runat="server" class="form-control" MaxLength="20" Text='<%# Bind("SUnidade") %>' Visible="false"></asp:TextBox>
                                                <asp:TextBox ID="txt4" runat="server" class="form-control" MaxLength="20" Text='<%# Bind("SCodigo") %>' Visible="false"></asp:TextBox>
                                                <asp:TextBox ID="txtsGarantia" runat="server" class="form-control" MaxLength="20" Text='<%# Bind("SGarantia") %>' Visible="false"></asp:TextBox>
                                                <asp:TextBox ID="txtsTipoMov" runat="server" class="form-control" MaxLength="20" Text='<%# Bind("STipoMov") %>' Visible="false"></asp:TextBox>
                                                <asp:TextBox ID="txtidMovimentacao" runat="server" class="form-control" MaxLength="20" Text='<%# Bind("IdMovimentacao") %>' Visible="false"></asp:TextBox>
                                                <asp:TextBox ID="txtLocal" runat="server" class="form-control" MaxLength="20" Text='<%# Bind("IdLocalArmazenamento") %>' Visible="false"></asp:TextBox>
                                                <asp:TextBox ID="txtPosicao" runat="server" class="form-control" MaxLength="20" Text='<%# Bind("IdPosicao") %>' Visible="false"></asp:TextBox>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="9%" Font-Size="11px" />
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Valor Unitário">
                                            <ItemTemplate>
                                                <asp:TextBox ID="Itens_txtnValorUnitario" oninput="formatNumber(this)" runat="server" class="form-control" MaxLength="20" Text='<%# Bind("nValorUnitario","{0:N4}") %>'></asp:TextBox>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="9%" Font-Size="11px" />
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="% IPI">
                                            <ItemTemplate>
                                                <asp:TextBox ID="Itens_txtnPercIPI" oninput="formatNumber(this)" runat="server" class="form-control" MaxLength="6" Text='<%# Bind("nPercIPI","{0:N2}") %>'></asp:TextBox>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="6%" Font-Size="11px" />
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="nValorTotal" HeaderText="Total" ItemStyle-CssClass="_nValorTotal" DataFormatString="{0:N2}">
                                            <ItemStyle Width="9%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                        
                                        </asp:BoundField>

<%--                                        <asp:BoundField DataField="SLote" HeaderText="Lote">
                                            <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sTipoMov" HeaderText="Movimentação">
                                            <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>--%>

                                        <asp:TemplateField HeaderText="">
                                            <ItemTemplate>
                                                <div class="acoes-mobile" style="display: flex; gap: 5px; justify-content: center;">
                                                    <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-link" TabIndex="100" CommandName="Delete" data-toggle="tooltip" title="Excluir"><i class="fa fa-eraser"></i></asp:LinkButton>

                                                    <asp:LinkButton runat="server" ID="cmdAdicionarSerie" CssClass="btn btn-primary btn-sm" Visible="false" CommandArgument='<%# Eval("idProduto") + "|" + Eval("sTipoMov") %>' OnClick="cmdAdicionarSerie_Click">
                                                             <i class="fa fa-barcode"></i> Validar
                                                        </asp:LinkButton>

                                                    <asp:LinkButton runat="server" ID="cmdVisualizarSerie" CssClass="btn btn-success btn-sm" Visible="false" CommandArgument='<%# Eval("idProduto") + "|" + Eval("sTipoMov") %>' OnClick="cmdAdicionarSerie_Click">
                                                            <i class="fa-check fa"></i> Validado
                                                    </asp:LinkButton>
                                                </div>
                                            </ItemTemplate>
                                            <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:TemplateField>

                                    </Columns>

                                </asp:GridView>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
            </div>

            <div class="panel panel-default" runat="server" id="divItensDestino">
                <div class="panel-heading">
                    <h3 class="panel-title">
                        <label runat="server" id="lblDePara" text="Transformação de Unidade"></label>
                    </h3>
                </div>
                <div class="panel-body">

                    <div class="row">
                        <div class="form-group">
                            <div class="col-lg-12">
                                <uc1:MensagemPagina runat="server" ID="MensagemPaginaDePara" />
                            </div>
                            <div class="col-lg-12">
                                <div class="col-lg-12">
                                    <uc1:FiltroPesquisa runat="server" ID="FiltroPesquisa1" sRegistraUnidade="S" />
                                </div>

                                <div id="div_movDePara" runat="server">
                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Local(Atual):</label>
                                            <asp:DropDownList ID="ddlLocal" runat="server" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlLocalMov_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Posição(Atual):</label>
                                            <asp:DropDownList ID="ddlPosicaoPai" AutoPostBack="false" class="form-control yes_no select Caixa_Selecao" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>

                                <div id="div_botoaoMov" runat="server" class="col-lg-2" visible="false">
                                    <br />
                                    <asp:Button ID="cmdIncluirMov" runat="server" class="btn btn-info form-control" Text="Incluir" OnClick="cmdIncluirMov_Click" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <br />

                    <div class="row">
                        <!-- DE -->
                        <div class="col-lg-6">
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    <h4><b>De:</b></h4>
                                </div>
                                <div class="panel-body">
                                    <div class="row">
                                        <div id="div_botoaoRetirada" runat="server" class="col-lg-2">
                                            <asp:Button ID="cmdIncluirRetirada" runat="server" class="btn btn-info form-control" Text="Incluir" OnClick="cmdIncluirRetirada_Click" />
                                        </div>
                                        <br />
                                        <br />
                                        <div class="col-lg-12">
                                            <asp:UpdatePanel runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <asp:GridView ID="dtgItensSaida" class="table table-striped table-bordered table-hover table-condensed"
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                        ShowFooter="False" Font-Names="Tahoma" DataKeyNames="IdProduto" Font-Overline="False" Font-Size="11px" OnRowDataBound="dtgItensSaida_RowDataBound" OnRowDeleting="dtgItensSaida_RowDeleting">
                                                        <Columns>
                                                            <asp:BoundField DataField="SDscProduto" HeaderText="Produto">
                                                                <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="SCodigo" HeaderText="Código">
                                                                <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="SUnidade" HeaderText="Unidade">
                                                                <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="NQuantidade" HeaderText="Quantidade">
                                                                <ItemStyle Width="20%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:TemplateField HeaderText="Local">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlLocal" AutoPostBack="true" OnSelectedIndexChanged="ddlLocal_Mov_SelectedIndexChanged" runat="server" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                                                    <asp:HiddenField ID="hddidProdutoGvSaida" runat="server" Value='<%# Bind("IdProduto") %>' />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Posição">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlPosicaoPai" class="form-control yes_no select Caixa_Selecao" runat="server"></asp:DropDownList>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Excluir">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="lnkExcluirEntrada" runat="server" CssClass="btn btn-danger btn-sm" TabIndex="100" CommandName="Delete">
                                                                        <i class="fa-eraser fa"></i>
                                                                    </asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- PARA -->
                        <div class="col-lg-6">
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    <h4><b>Para:</b></h4>
                                </div>
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-lg-12">
                                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaItensPara" />
                                        </div>

                                        <div id="div_botoaoEntrada" runat="server" class="col-lg-2">
                                            <asp:Button ID="cmdIncluirEntrada" runat="server" class="btn btn-info form-control" Text="Incluir" OnClick="cmdIncluirEntrada_Click" />
                                        </div>
                                        <br />
                                        <br />
                                        <div class="col-lg-12">
                                            <asp:UpdatePanel runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <asp:GridView ID="dtgItensEntrada" class="table table-striped table-bordered table-hover table-condensed"
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                        ShowFooter="False" Font-Names="Tahoma" DataKeyNames="IdProduto" Font-Overline="False" Font-Size="11px" OnRowDataBound="dtgItensEntrada_RowDataBound" OnRowDeleting="dtgItensEntrada_RowDeleting">
                                                        <Columns>
                                                            <asp:BoundField DataField="SDscProduto" HeaderText="Produto">
                                                                <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="SCodigo" HeaderText="Código">
                                                                <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="SUnidade" HeaderText="Unidade">
                                                                <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="NQuantidade" HeaderText="Quantidade">
                                                                <ItemStyle Width="20%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:TemplateField HeaderText="Local">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlLocal" AutoPostBack="true" OnSelectedIndexChanged="ddlLocal_Mov_SelectedIndexChanged" runat="server" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Posição">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlPosicaoPai" class="form-control yes_no select Caixa_Selecao" runat="server"></asp:DropDownList>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>


                                                            <asp:TemplateField HeaderText="Excluir">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="lnkExcluirSaida" runat="server" CssClass="btn btn-danger btn-sm" TabIndex="100" CommandName="Delete">
                                                                        <i class="fa-eraser fa"></i>
                                                                    </asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </div>

            <div class="panel panel-default" runat="server" id="divMudancaPosicao" visible="false">
                <div class="panel-heading">
                    <h3 class="panel-title">
                        <b><i class="fa fa-exchange"></i>&nbsp Mudança de Posicionamento</b>
                    </h3>
                </div>
                <div class="panel-body">

                    <div class="row" runat="server" id="divFpMudanca">
                        <div class="col-lg-12">
                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaMudanca" />
                        </div>
                        <div class="col-lg-10">
                            <uc1:FiltroPesquisa runat="server" ID="FiltroPesquisaMudanca" sRegistraUnidade="S" />
                        </div>
                        <div class="col-lg-2">
                            <br />
                            <asp:Button ID="cmdIncluirMudanca" runat="server" class="btn btn-info form-control" Text="Incluir" OnClick="cmdIncluirMudanca_Click" />
                        </div>
                    </div>

                    <br />

                    <div class="row">
                        <div class="col-lg-12">
                            <asp:UpdatePanel runat="server" ID="updDtgMudanca" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgMudancaPosicao" class="table table-striped table-bordered table-hover"
                                            runat="server" Width="100%" AutoGenerateColumns="False" DataKeyNames="IdProduto"
                                            OnRowDataBound="dtgMudancaPosicao_RowDataBound" OnRowDeleting="dtgMudancaPosicao_RowDeleting">
                                            <Columns>
                                                <asp:BoundField DataField="SDscProduto" HeaderText="Produto" ItemStyle-Width="25%" />

                                                <asp:BoundField DataField="SCodigo" HeaderText="Código" ItemStyle-Width="10%" />

                                                <asp:TemplateField HeaderText="Qtd" ItemStyle-Width="8%" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtQtdMudanca" oninput="formatNumber(this)" runat="server" CssClass="form-control input-sm text-center" Text='<%# Bind("NQuantidade","{0:N4}") %>'></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Local (Origem)" ItemStyle-Width="13%" HeaderStyle-CssClass="text-center">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlLocalOrigem" AutoPostBack="true" OnSelectedIndexChanged="ddlLocalOrigem_SelectedIndexChanged" runat="server" CssClass="form-control input-sm"></asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Posição (Origem)" ItemStyle-Width="12%" HeaderStyle-CssClass="text-center">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlPosicaoOrigem" runat="server" CssClass="form-control input-sm"></asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Local (Destino)" ItemStyle-Width="13%" HeaderStyle-CssClass="text-center">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlLocalDestino" AutoPostBack="true" OnSelectedIndexChanged="ddlLocalDestino_SelectedIndexChanged" runat="server" CssClass="form-control input-sm"></asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Posição (Destino)" ItemStyle-Width="12%" HeaderStyle-CssClass="text-center">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlPosicaoDestino" runat="server" CssClass="form-control input-sm"></asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Ação" ItemStyle-Width="15%" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center">
                                                    <ItemTemplate>
                                                        <div class="acoes-mobile" style="display: flex; gap: 5px; justify-content: center;">
                                                            <asp:LinkButton ID="lnkExcluirMudanca" runat="server" CssClass="btn btn-danger btn-sm" CommandName="Delete" ToolTip="Excluir"><i class="fa fa-eraser"></i></asp:LinkButton>

                                                            <asp:LinkButton ID="cmdValidarMudanca" runat="server" CssClass="btn btn-primary btn-sm"
                                                                Visible="false" CommandArgument='<%# Eval("IdProduto") + "|M" %>' OnClick="cmdValidarMudanca_Click">
                <i class="fa fa-barcode"></i> Validar
            </asp:LinkButton>

                                                            <asp:LinkButton ID="cmdVisualizarMudanca" runat="server" CssClass="btn btn-success btn-sm"
                                                                Visible="false" CommandArgument='<%# Eval("IdProduto") + "|M" %>' OnClick="cmdValidarMudanca_Click">
                <i class="fa fa-check"></i> Validado
            </asp:LinkButton>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>

                </div>
            </div>

            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" ValidationGroup="DETALHE" />

                <%-- NOVO BOTÃO AQUI --%>
                <asp:Button ID="cmdValidarTodos" runat="server" Text="Validar Etiquetas" CssClass="btn btn-lg btn-primary" OnClick="cmdValidarTodos_Click" Visible="false" />

                <%-- Remover depois--%>
                <asp:Button ID="cmdEfetivar" class="btn btn-lg btn-info" runat="server" Text="Efetivar Estoque" Visible="false" OnClick="cmdEfetivar_Click" />
                <asp:Button ID="cmdImprimirEtiquetas" runat="server" Text="Imprimir Etiquetas" CssClass="btn btn-lg btn-primary" OnClick="cmdImprimirEtiquetas_Click" Visible="false" />
                <asp:Button ID="cmdExcluir" class="btn btn-lg btn-danger" runat="server" Text="Excluir" OnClick="cmdExcluir_Click" Visible="false"
                    OnClientClick="return confirm('Deseja excluir esta movimentação?');" CausesValidation="false" />
                <asp:Button ID="cmdVoltar" class="btn btn-lg btn-warning" runat="server" Text="Voltar" OnClick="cmdVoltar_Click" />
            </fieldset>

            <div id="dialog-SalvarPrincipal" class="modal" title="Salvar">
                <p>
                    <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                    <asp:Label ID="lblTituloSalvar" runat="server" Text="Deseja Executar a Ação?"></asp:Label>
                </p>
            </div>

            <div id="hdd">
                <asp:HiddenField ID="hddidLocal" runat="server" />
                <asp:HiddenField ID="hfModalAberta" runat="server" />
                <asp:HiddenField ID="hddidMovimentacao" runat="server" />
                <asp:HiddenField ID="hddidStatus" runat="server" />
                <asp:HiddenField ID="hdddtEfetivado" runat="server" />
                <asp:HiddenField ID="hddQtdTotal" runat="server" />
                <asp:HiddenField ID="hddidFornecedor" runat="server" />
                <asp:HiddenField ID="hddIdProduto" runat="server" />
                <asp:HiddenField ID="hddsLiberaSerie" runat="server" />
                <asp:HiddenField ID="hddsLiberarEfetivacao" runat="server" />
                <asp:HiddenField ID="hddsLiberaImpressao" runat="server" />
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnProcessarXML" />
        </Triggers>
    </asp:UpdatePanel>

    <div class="modal fade" id="modalItens" data-backdrop="static">
        <div class="modal-dialog modal-xl">
            <asp:UpdatePanel ID="updModalItens" runat="server" UpdateMode="Conditional">
                <ContentTemplate>

                    <div class="modal-content">

                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <h1 class="modal-title" id="lblModalEnvio">
                                <asp:Label ID="lbltituloModal" Text="Validação de Séries - Etiquetas" runat="server" />
                            </h1>
                        </div>

                        <div class="modal-body">
                            <div class="col-lg-12">
                                <uc1:MensagemPagina runat="server" ID="MensagemPaginaItensModal" />
                            </div>

                            <div id="div_SeriesMain" runat="server">

                                <div class="col-lg-5 border-right padd-l-0">
                                    <div class="panel panel-default m-0">
                                        <div class="panel-heading">
                                            <b><i class="fa fa-barcode"></i>&nbsp Validação com Leitor</b>
                                        </div>
                                        <div class="panel-body text-center">

                                            <div class="form-group">
                                                <asp:Button ID="cmdAbrirCameraValidacao" OnClick="AbrirCameraValidacao_Click" CssClass="btn btn-lg btn-primary btn-block" Text="Abrir Leitor / Câmera" runat="server" />
                                            </div>

                                            <div id="DivBipadorValidacao" runat="server" class="mb-3 text-center" visible="false">
                                                <uc1:LeitorQuagga runat="server" ID="LeitorQuaggaMovimentacao" STemParametros="S" />
                                            </div>

                                            <hr />

                                            <div class="form-group text-left" id="divCodigoManualValidacao" runat="server">
                                                <label>Leitura Manual / Código Lido</label>
                                                <div class="input-group">
                                                    <span class="input-group-addon"><i class="fa fa-qrcode"></i></span>
                                                    <asp:TextBox ID="txtCodigoBarrasValidacao" CssClass="form-control" runat="server" placeholder="PR000000E000000"></asp:TextBox>

                                                    <span class="input-group-btn">
                                                        <asp:Button ID="cmdProcessarLeituraValidacao" runat="server" CssClass="btn btn-success" Text="Validar" OnClick="ProcessarLeituraValidacao_Click" />
                                                    </span>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-7 padd-r-0">
                                    <div class="panel panel-default m-0">
                                        <div class="panel-heading">
                                            <b><i class="fa fa-list"></i>&nbsp Produtos a Validar</b>

                                            <asp:LinkButton ID="cmdReimprimirTodas" runat="server" CssClass="btn btn-warning btn-xs" Style="margin-left: 15px;" OnClick="cmdReimprimirTodas_Click"><i class="fa fa-print"></i> Reimprimir Todas</asp:LinkButton>

                                            <span class="pull-right">Validadas:
                                                        <b>
                                                            <asp:Literal ID="spanQtdPreparados" runat="server"></asp:Literal></b>
                                                de
                                                       <b>
                                                           <asp:Literal ID="spanQtdtotal" runat="server"></asp:Literal></b>
                                            </span>
                                        </div>
                                        <div class="panel-body">
                                            <div class="table-responsive-custom table-responsive-mobile" style="display: block; width: 100%; min-height: 250px; max-height: 400px; overflow-y: auto; overflow-x: auto;">
                                                <table class="table-custom table-bordered-custom" style="width: 100%;">
                                                    <thead>
                                                        <tr>
                                                            <th class="th-custom">Cód. Barras (Etiqueta)</th>
                                                            <th class="th-custom">Lote</th>
                                                            <th class="th-custom">Série</th>
                                                            <th class="th-custom text-center">Status</th>
                                                            <th class="th-custom text-center">Ação</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:Repeater ID="rptItensSalvosValidacao" runat="server">
                                                            <ItemTemplate>
                                                                <tr class='<%# Eval("SConfirmado").ToString() == "S" ? "success" : "" %>'>
                                                                    <td class="td-custom">
                                                                        <strong><%# Eval("SCodigoBarras") %></strong>
                                                                    </td>
                                                                    <td class="td-custom">
                                                                        <%# Eval("SLote") %>
                                                                    </td>
                                                                    <td class="td-custom">
                                                                        <%# Eval("NSerie") %>
                                                                    </td>
                                                                    <td class="td-custom text-center">
                                                                        <asp:Label ID="lblStatusValidacao" runat="server"
                                                                            Text='<%# Eval("SConfirmado").ToString() == "S" ? "Validado" : "Pendente" %>'
                                                                            CssClass='<%# Eval("SConfirmado").ToString() == "S" ? "label label-success" : "label label-warning" %>'>
                                                                        </asp:Label>
                                                                    </td>
                                                                    <td class="td-custom text-center">
                                                                        <%-- NOVO BOTÃO: REIMPRIMIR INDIVIDUAL --%>
                                                                        <asp:LinkButton ID="lnkReimprimirIndividual" runat="server" CssClass="btn btn-warning btn-xs" ToolTip="Reimprimir esta etiqueta" CommandArgument='<%# Eval("SCodigoBarras") %>' OnClick="lnkReimprimirIndividual_Click">
                                                                                    <i class="fa fa-print"></i>
                                                                                </asp:LinkButton>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>

                        <div class="modal-footer">
                            <asp:Button ID="cmdFechar" class="btn btn-secondary" runat="server" Text="Fechar" OnClick="cmdFecharSerie_Click" />
                        </div>

                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <div class="modal" id="modalValidarTodos" tabindex="-1" role="dialog" aria-hidden="true" data-backdrop="static">
        <div class="modal-dialog modal-xl" role="document">
            <div class="modal-content">
                <asp:UpdatePanel ID="updModalValidarTodos" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <h1 class="modal-title">
                                <asp:Label ID="lblTituloModalTodos" Text="Validação Geral de Séries" runat="server" />
                            </h1>
                        </div>
                        <div class="modal-body">
                            <div class="col-lg-12">
                                <uc1:MensagemPagina runat="server" ID="MensagemPaginaTodosModal" />
                                <div class="row">
                                    <div class="col-lg-5 border-right">
                                        <div class="panel panel-default">
                                            <div class="panel-heading bg-tt" style="color: white; background-color: #1ab394;">
                                                <b><i class="fa fa-barcode"></i>&nbsp Validação com Leitor (Todos)</b>
                                            </div>
                                            <div class="panel-body text-center">
                                                <div class="form-group">
                                                    <asp:Button ID="cmdAbrirCameraTodos" OnClick="cmdAbrirCameraTodos_Click" CssClass="btn btn-lg btn-primary btn-block" Text="Abrir Leitor / Câmera" runat="server" />
                                                </div>
                                                <div id="DivBipadorTodos" runat="server" class="mb-3 text-center" visible="false">
                                                    <uc1:LeitorQuagga runat="server" ID="LeitorQuaggaTodos" STemParametros="S" />
                                                </div>
                                                <hr />
                                                <div class="form-group text-left" id="divCodigoManualTodos" runat="server">
                                                    <label>Leitura Manual / Código Lido</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon"><i class="fa fa-qrcode"></i></span>
                                                        <asp:TextBox ID="txtCodigoBarrasTodos" CssClass="form-control" runat="server" placeholder="PR000000E000000"></asp:TextBox>
                                                        <span class="input-group-btn">
                                                            <asp:Button ID="cmdProcessarLeituraTodos" runat="server" CssClass="btn btn-success" Text="Validar" OnClick="cmdProcessarLeituraTodos_Click" />
                                                        </span>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-7">
                                        <div class="panel panel-default">
                                            <div class="panel-heading bg-tt" style="color: white; background-color: #1ab394;">
                                                <b><i class="fa fa-check-circle"></i>&nbsp Itens Validados</b>
                                                <span class="pull-right">Total: 
                                               
                                                    <asp:Literal ID="spanContadorTodos" runat="server"></asp:Literal>
                                                </span>
                                            </div>
                                            <div class="panel-body">
                                                <div class="table-responsive-custom table-responsive-mobile" style="display: block; width: 100%; min-height: 250px; max-height: 400px; overflow-y: auto;">
                                                    <asp:GridView ID="dtgValidadosTodos" runat="server" CssClass="table-custom table-bordered-custom table-striped" Width="100%"
                                                        AutoGenerateColumns="False" AllowPaging="true" PageSize="20" OnPageIndexChanging="dtgValidadosTodos_PageIndexChanging">
                                                        <EmptyDataTemplate>
                                                            <div class="text-center" style="padding: 20px; color: #888;">
                                                                <i class="fa fa-inbox fa-3x"></i>
                                                                <br />
                                                                Nenhum item validado ainda.
                                                       
                                                            </div>
                                                        </EmptyDataTemplate>
                                                        <Columns>
                                                            <asp:BoundField DataField="sCodigoBarras" HeaderText="Cód. Barras" ItemStyle-CssClass="td-custom text-bold" HeaderStyle-CssClass="th-custom" />
                                                            <asp:BoundField DataField="sDscProduto" HeaderText="Produto" ItemStyle-CssClass="td-custom" HeaderStyle-CssClass="th-custom" />
                                                            <asp:BoundField DataField="sLoteSerie" HeaderText="Lote/Série" ItemStyle-CssClass="td-custom" HeaderStyle-CssClass="th-custom" />
                                                            <%--                    <asp:BoundField DataField="sLote" HeaderText="Lote" ItemStyle-CssClass="td-custom" HeaderStyle-CssClass="th-custom" />
                                                            <asp:BoundField DataField="nSerie" HeaderText="Série" ItemStyle-CssClass="td-custom" HeaderStyle-CssClass="th-custom" />--%>
                                                        </Columns>
                                                        <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Fechar</button>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <div class="modal" id="modalFracionamento" tabindex="-1" role="dialog" aria-hidden="true" data-backdrop="static">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <asp:UpdatePanel ID="updModalFracionamento" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <h2 class="modal-title" style="color: #009A22; font-weight: bold;">
                                <i class="fa fa-print"></i>Configuração de Etiquetas
                        </h2>
                        </div>
                        <div class="modal-body">
                            <div class="alert alert-info">
                                Defina a quantidade de etiquetas que deseja imprimir para cada produto e o sistema calculará o fracionamento automaticamente.
                       
                            </div>
                            <div class="table-responsive-custom" style="max-height: 400px; overflow-y: auto;">
                                <asp:GridView ID="dtgFracionamento" runat="server" CssClass="table-custom table-bordered-custom table-striped" Width="100%"
                                    AutoGenerateColumns="False" DataKeyNames="IdProduto" OnRowDataBound="dtgFracionamento_RowDataBound">
                                    <Columns>
                                        <asp:BoundField DataField="SCodigoComDescricao" HeaderText="Produto" ItemStyle-CssClass="td-custom" HeaderStyle-CssClass="th-custom" />

                                        <asp:TemplateField HeaderText="Qtd Total" HeaderStyle-CssClass="th-custom text-center" ItemStyle-CssClass="td-custom text-center">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtdTotal" runat="server" Text='<%# Eval("NQuantidade", "{0:N4}") %>' CssClass="label-qtd-total" Font-Bold="true"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Nº de Etiquetas" HeaderStyle-CssClass="th-custom text-center" ItemStyle-CssClass="td-custom text-center">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtQtdEtiquetas" runat="server" CssClass="form-control text-center input-qtd-etiquetas" Text="1" type="number" min="1" Width="80px" Style="margin: 0 auto;"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Qtd por Etiqueta" HeaderStyle-CssClass="th-custom text-center" ItemStyle-CssClass="td-custom text-center">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtQtdFracionada" runat="server" CssClass="form-control text-center input-qtd-fracionada" Text='<%# Eval("NQuantidade", "{0:N4}") %>' Width="100px" Style="margin: 0 auto;"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancelar</button>
                            <asp:Button ID="cmdConfirmarImpressao" runat="server" Text="Imprimir" CssClass="btn btn-primary" OnClick="cmdConfirmarImpressao_Click" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalMudanca" tabindex="-1" role="dialog" aria-hidden="true" data-backdrop="static">
        <div class="modal-dialog modal-xl" role="document">
            <div class="modal-content">
                <asp:UpdatePanel ID="updModalMudanca" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <h1 class="modal-title">
                                <asp:Label ID="lblTituloModalMudanca" Text="Bipar Itens - Mudança de Posição" runat="server" />
                            </h1>
                        </div>
                        <div class="modal-body">
                            <div class="col-lg-12">
                                <uc1:MensagemPagina runat="server" ID="MensagemPaginaMudancaModal" />

                                <div class="row">
                                    <div class="col-lg-5 border-right">
                                        <div class="panel panel-default">
                                            <div class="panel-heading bg-tt" style="color: white; background-color: #1ab394;">
                                                <b><i class="fa fa-barcode"></i>&nbsp Validação com Leitor</b>
                                            </div>
                                            <div class="panel-body text-center">
                                                <div class="form-group">
                                                    <asp:Button ID="cmdAbrirCameraMudanca" OnClick="cmdAbrirCameraMudanca_Click" CssClass="btn btn-lg btn-primary btn-block" Text="Abrir Leitor / Câmera" runat="server" />
                                                </div>
                                                <div id="DivBipadorMudanca" runat="server" class="mb-3 text-center" visible="false">
                                                    <uc1:LeitorQuagga runat="server" ID="LeitorQuaggaMudanca" STemParametros="S" />
                                                </div>
                                                <hr />
                                                <div class="form-group text-left" id="divCodigoManualMudanca" runat="server">
                                                    <label>Leitura Manual / Código Lido</label>

                                                    <asp:HiddenField ID="hddIdLocalOrigemMudanca" runat="server" ClientIDMode="Static" />

                                                    <div class="input-group">
                                                        <span class="input-group-addon"><i class="fa fa-qrcode"></i></span>

                                                        <asp:TextBox ID="txtCodigoBarrasMudanca" CssClass="form-control css-autocomplete-etiquetas" runat="server" placeholder="PR000000E000000"></asp:TextBox>

                                                        <span class="input-group-btn">
                                                            <asp:Button ID="cmdProcessarLeituraMudanca" runat="server" CssClass="btn btn-success" Text="Validar" OnClick="cmdProcessarLeituraMudanca_Click" />
                                                        </span>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-7">
                                        <div class="panel panel-default">
                                            <div class="panel-heading bg-tt" style="color: white; background-color: #1ab394;">
                                                <b><i class="fa fa-list"></i>&nbsp Etiquetas </b>
                                                <span class="pull-right">Validadas:
                                               
                                                    <b>
                                                        <asp:Literal ID="spanValidadasMudanca" runat="server" Text="0"></asp:Literal></b>
                                                    de
                                               
                                                    <b>
                                                        <asp:Literal ID="spanTotalMudanca" runat="server" Text="0"></asp:Literal></b>
                                                </span>
                                            </div>
                                            <div class="panel-body">
                                                <div class="table-responsive-custom table-responsive-mobile" style="display: block; width: 100%; min-height: 250px; max-height: 400px; overflow-y: auto;">
                                                    <asp:GridView ID="dtgMudancaValidadas" runat="server" CssClass="table-custom table-bordered-custom table-striped" Width="100%" AutoGenerateColumns="False">
                                                        <EmptyDataTemplate>
                                                            <div class="text-center" style="padding: 20px; color: #888;">
                                                                <i class="fa fa-inbox fa-3x"></i>
                                                                <br />
                                                                Nenhuma etiqueta bipada para transferência ainda.
                                                       
                                                            </div>
                                                        </EmptyDataTemplate>
                                                        <Columns>
                                                            <asp:BoundField DataField="sCodigoBarras" HeaderText="Cód. Barras" ItemStyle-CssClass="td-custom text-bold" HeaderStyle-CssClass="th-custom" />
                                                            <asp:BoundField DataField="sDscProduto" HeaderText="Produto" ItemStyle-CssClass="td-custom" HeaderStyle-CssClass="th-custom" />
                                                            <asp:TemplateField HeaderText="Status" ItemStyle-CssClass="text-center">
                                                                <ItemTemplate>
                                                                    <span class="label label-info">Pronto para Mover</span>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Fechar</button>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <script>
        function resetAndOpenModalItens() {
            $('.modal-backdrop').remove();
            $('body').removeClass('modal-open');

            // Garante que o modal seja redefinido corretamente
            $('#modalItens').off('hidden.bs.modal').on('hidden.bs.modal', function () {
                $(this).find('.modal-content').html('');
            });

            // Certifica que o modal será exibido no topo
            $('#modalItens').css('z-index', 1050);

            $('#modalItens').modal({ backdrop: 'static', keyboard: false, show: true });
        }

        function formatNumber(input) {
            var value = input.value;

            // Remove qualquer caractere não numérico, exceto números e vírgula
            value = value.replace(/[^\d,]/g, '');

            // Se houver mais de uma vírgula, mantemos apenas a última
            var parts = value.split(',');
            if (parts.length > 2) {
                parts = [parts[0], parts.slice(1).join('')]; // Junta o que estava após as outras vírgulas
            }

            // Limita as casas decimais após a vírgula a 4
            if (parts[1]) {
                parts[1] = parts[1].substring(0, 4); // Limita a 4 casas decimais
            }

            // Junta novamente as partes (inteiro e decimal)
            value = parts.join(',');

            // Atualiza o valor no campo de entrada
            input.value = value;
        }

        function bindFracionamentoEvents() {
            // 1. Quando altera o NÚMERO DE ETIQUETAS
            $('.input-qtd-etiquetas').off('input').on('input', function () {
                var row = $(this).closest('tr');
                if (row.attr('data-unitario') === 'S') return; // Se for unitário, ignora o JS

                var qtdTotalStr = row.find('.label-qtd-total').text().replace(/\./g, '').replace(',', '.');
                var qtdTotal = parseFloat(qtdTotalStr);
                var numEtiquetas = parseInt($(this).val());

                if (numEtiquetas > 0 && !isNaN(qtdTotal)) {
                    var fracao = qtdTotal / numEtiquetas;
                    // Joga o resultado calculado para a fração com formatação
                    row.find('.input-qtd-fracionada').val(fracao.toFixed(4).replace('.', ','));
                }
            });

            // 2. Quando altera a FRAÇÃO (Matemática silenciosa, sem arrancar a vírgula)
            $('.input-qtd-fracionada').off('input').on('input', function () {
                var row = $(this).closest('tr');
                if (row.attr('data-unitario') === 'S') return; // Se for unitário, ignora o JS

                var qtdTotalStr = row.find('.label-qtd-total').text().replace(/\./g, '').replace(',', '.');
                var qtdTotal = parseFloat(qtdTotalStr);

                // Converte o que o cara digitou pra número de verdade
                var fracaoStr = $(this).val().replace(/\./g, '').replace(',', '.');
                var fracao = parseFloat(fracaoStr);

                // Apenas calcula a qtd de etiquetas. Não joga formatação de volta na caixa de fração aqui!
                if (fracao > 0 && !isNaN(qtdTotal)) {
                    var numEtiquetas = Math.ceil(qtdTotal / fracao);
                    row.find('.input-qtd-etiquetas').val(numEtiquetas);
                }
            });

            // 3. Quando SAI DO CAMPO da fração (Blur) -> Formata com ,0000
            $('.input-qtd-fracionada').off('blur').on('blur', function () {
                var row = $(this).closest('tr');
                if (row.attr('data-unitario') === 'S') return;

                var qtdTotalStr = row.find('.label-qtd-total').text().replace(/\./g, '').replace(',', '.');
                var qtdTotal = parseFloat(qtdTotalStr);

                var fracaoStr = $(this).val().replace(/\./g, '').replace(',', '.');
                var fracao = parseFloat(fracaoStr);

                if (fracao > 0 && !isNaN(fracao)) {
                    // Quando ele tira o mouse do campo, a gente força a máscara de 4 casas decimais
                    $(this).val(fracao.toFixed(4).replace('.', ','));
                } else {
                    // Se o cara apagar tudo e sair, reseta pro padrão
                    $(this).val(qtdTotal.toFixed(4).replace('.', ','));
                    row.find('.input-qtd-etiquetas').val(1);
                }
            });
        }

        function RegistrarAutocompleteMudanca() {
            var $j = window.jQuery || window.$;
            var inputMudanca = $j('.css-autocomplete-etiquetas');

            if (inputMudanca.length > 0) {
                // Destrói a instância anterior para não duplicar se a tela piscar
                if (inputMudanca.hasClass('ui-autocomplete-input')) {
                    inputMudanca.autocomplete('destroy');
                }

                inputMudanca.autocomplete({
                    minLength: 0,
                    source: function (request, response) {
                        // Pega o ID com segurança usando o ClientID do WebForms
                        var idProd = $j('#<%= hddIdProduto.ClientID %>').val() || "0";
                        var idLoc = $j('#hddIdLocalOrigemMudanca').val() || "0";

                        $j.ajax({
                            url: 'Movimentacao_Detalhe.aspx/GetEtiquetasMudancaAutocomplete',
                            data: JSON.stringify({ term: request.term, idProduto: idProd, idLocal: idLoc }),
                            dataType: 'json',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            success: function (data) {
                                if (data.d && data.d.length > 0) {
                                    response($j.map(data.d, function (item) {
                                        var parts = item.split('|');
                                        return { label: parts[0], value: parts[1] };
                                    }));
                                } else {
                                    response([{ label: '⚠️ Nenhuma etiqueta disponível neste local!', value: '' }]);
                                }
                            },
                            error: function (xhr, textStatus, errorThrown) {
                                console.error("Erro no Autocomplete:", xhr.responseText);
                            }
                        });
                    },
                    select: function (event, ui) {
                        if (ui.item.value === '') {
                            event.preventDefault();
                            $j(this).val('');
                            return false;
                        }
                        $j(this).val(ui.item.value);
                        $j('#<%= cmdProcessarLeituraMudanca.ClientID %>').click();
                        return false;
                    }
                }).focus(function () {
                    $j(this).autocomplete("search", $j(this).val());
                });
            }
        }

        // =========================================================================
        // PROTEÇÃO CONTRA POSTBACK (UpdatePanel)
        // =========================================================================
        var $j = window.jQuery || window.$;
        $j(document).ready(function () {
            // 1. Executa na primeira vez que a página carrega
            RegistrarAutocompleteMudanca();

            // 2. Re-executa sempre que um UpdatePanel fizer Postback Assíncrono
            if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (sender, args) {
                    RegistrarAutocompleteMudanca();
                });
            }
        });
    </script>

</asp:Content>
