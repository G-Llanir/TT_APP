<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="SeparacaoPedido_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.SeparacaoPedido_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/LeitorQuagga.ascx" TagPrefix="uc1" TagName="LeitorQuagga" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        .visible {
            display: none;
        }

        .gvHeader th {
            padding: 7px;
            background-color: #1A4C1A;
            color: #fff;
            border: 1px solid #bbb;
            font-weight: normal;
        }

        .gvChildHeader th {
            padding: 4px;
            background-color: #999966;
            color: #fff;
            border: 1px solid #bbb;
            font-weight: normal;
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

        .linha-excluida {
            background-color: #ffcccc; /* Ou qualquer outra cor vermelha desejada */
        }

        /*  Grid Itens Modal*/
        .truncate-text {
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        /*modal*/
        .modal-header-content {
            display: flex;
            align-items: center;
        }

        .modal-xxl {
            max-width: 85%; /* Ajuste conforme necessário */
            width: 66%;
        }

        .modal-xl {
            max-width: 65%; /* Ajuste conforme necessário */
            width: 46%;
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

        .mini-menu {
            background-color: #009a22;
            border-radius: 5px;
            padding: 10px;
        }

            .mini-menu ul {
                list-style-type: none;
                padding: 0;
                display: flex; /* Exibe os itens horizontalmente */
            }

            .mini-menu li {
                margin-right: 10px; /* Espaçamento entre os itens */
            }

                .mini-menu li:last-child {
                    margin-right: 0; /* Remove o espaçamento do último item */
                }

                .mini-menu li a {
                    color: #ffffff; /* Texto branco para contraste */
                    text-decoration: none;
                    padding: 5px 10px;
                    display: flex; /* Para alinhar o texto e a seta verticalmente */
                    align-items: center; /* Alinha verticalmente */
                    transition: background-color 0.3s, color 0.3s; /* Suaviza a transição */
                }

                    .mini-menu li a:hover {
                        background-color: #005a12; /* Fundo mais escuro no hover */
                        color: #ffffff; /* Mantém o texto branco no hover */
                    }
    </style>

    <div id="DIV3" runat="server">
        <script src="https://login.tecandtec.com.br/app/js/jquery-1.9.2.min.js" type="text/javascript"></script>
        <script> $v192 = jQuery.noConflict();</script>

        <div class="form-stacked row">
            <div class="col-lg-12">
                <h1>
                    <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA">

                    </asp:Label></h1>
                <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Detalhe" />
            </div>
        </div>

        <div>
            <ul id="tab_Separacao" class="nav nav-tabs" role="tablist">
                <li role="presentation" class="tabpanel active">
                    <a href="#ide" id="ide-tab" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Separação</b></a>
                </li>

                <li role="presentation" runat="server" id="aba_Historico">
                    <a href="#Historico" role="tab" id="Historico-tab" data-toggle="tab" aria-controls="Historico"><b>Histórico</b></a>
                </li>
            </ul>
        </div>

        <div id="tab" class="tab-content">

            <div role="tabpanel" class="tab-pane fade in active" id="ide" aria-labelledby="ide-tab">
                <br />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>

                        <div class="panel panel-default" runat="server">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Detalhe</b></h3>
                            </div>

                            <div class="panel-body">
                                <div class="form-stacked row">

                                    <div class="col-lg-12">
                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                                    </div>

                                    <div class="col-lg-2" runat="server" id="DIV4">
                                        <div class="form-group">
                                            <label>Data do Pedido</label>
                                            <asp:TextBox ID="txtdtPedido" class="form-control uppercase" runat="server" MaxLength="50" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2" runat="server" id="DIV1">
                                        <div class="form-group">
                                            <label>N° Pedido (Cliente)</label>
                                            <asp:TextBox ID="txtnNumeroPedido" class="form-control uppercase" runat="server" MaxLength="50" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2" runat="server" id="DIV_nControleTT">
                                        <div class="form-group">
                                            <label>N° Controle TT</label>
                                            <asp:TextBox ID="txtnControleTT" class="form-control uppercase" runat="server" MaxLength="50" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-4" runat="server" id="DIV_Referencia">
                                        <div class="form-group">
                                            <label>Referência</label>
                                            <asp:TextBox ID="txtsReferencia" class="form-control uppercase" runat="server" MaxLength="50" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <h3>
                                                <span class="status" title="Status">
                                                    <asp:Label ID="lblsDscTipoStatus" runat="server" TabIndex="100"></asp:Label>
                                                </span>
                                            </h3>
                                        </div>
                                    </div>

                                    <div class="col-lg-6" runat="server" id="DIV_DADOS_Cliente">
                                        <div class="form-group">
                                            <label>Cliente </label>
                                            <asp:TextBox ID="txtsRazaoSocial" class="form-control uppercase" runat="server" MaxLength="200" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-6" runat="server" id="DIV2">
                                        <div class="form-group">
                                            <label>Empresa </label>
                                            <asp:TextBox ID="txtsEmpresa" class="form-control uppercase" runat="server" MaxLength="200" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2" id="div_EstimativaEntrega" runat="server">
                                        <div class="form-group">
                                            <label>Estimativa Entrega</label>
                                            <asp:TextBox ID="txtdtEstimativaEntrega" class="form-control uppercase" runat="server" MaxLength="10" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-4" id="Div_FormadeEnvio" runat="server">
                                        <div class="form-group">
                                            <label>Forma de Envio</label>
                                            <asp:TextBox ID="txtsDscTipoEnvio" class="form-control uppercase " runat="server" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-6" runat="server" id="DIV_EnderecoEntrega">
                                        <div class="form-group">
                                            <label>Endereço de Entrega</label>
                                            <asp:TextBox ID="txtsEnderecoEntrega" class="form-control uppercase " runat="server" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-12 table-responsive">
                                        <asp:GridView ID="gv_Arquivo" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" OnRowDataBound="gv_Arquivo_RowDataBound">
                                            <Columns>

                                                <asp:BoundField DataField="idProduto" HeaderText="ID">
                                                    <ItemStyle Width="5%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sCodigo" HeaderText="Código">
                                                    <ItemStyle Width="12%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDscProduto" HeaderText="Descrição">
                                                    <ItemStyle Width="33%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDscLocalArmazenamento" HeaderText="Local Armazenamento">
                                                    <ItemStyle Width="15%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sUnidade" HeaderText="UN">
                                                    <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade">
                                                    <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="nQtdSeparado" HeaderText="Quantidade Separado">
                                                    <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Falta Separar">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="FaltaSeparar" Text=' <%# Convert.ToDecimal(Eval("nQuantidade")) - Convert.ToDecimal(Eval("nQtdSeparado")) %>' ClientIDMode="Static"></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>
                                    </div>

                                    <div class="col-lg-12" style="margin-top: 6px;" id="DIV17" runat="server">
                                        <div class="form-group">
                                            <br />
                                            <asp:Button ID="btnVisualizarEnvios" OnClick="btnVisualizarEnvios_Click" CssClass="btn btn-sm btn-primary mb-2" Text="Visualizar Envios" runat="server" />
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>

                        <div class="panel panel-default" runat="server" id="div23">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Separação</b></h3>
                            </div>
                            <div class="panel-body row">
                                <div class="form-stacked">
                                    <div class="col-lg-12" runat="server" id="DIV6">
                                        <div class="row">

                                            <div class="col-lg-12">
                                                <uc1:MensagemPagina runat="server" ID="MensagemPagina1" />
                                            </div>

                                            <div class="col-lg-2" id="DIV_sCodigo" runat="server">
                                                <div class="form-group">
                                                    <label>Código Barras</label>
                                                    <asp:TextBox ID="txtsCodigo_EAN" CssClass="form-control" runat="server" placeholder="Código Barras"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-1" style="margin-top: 6px;" id="DIV_IncluirEAN" runat="server">
                                                <div class="form-group">
                                                    <br />
                                                    <asp:Button ID="btnIncluirEAN" OnClick="btnIncluirEAN_Click" CssClass="btn btn-sm btn-primary" Text="Incluir" runat="server" />
                                                </div>
                                            </div>

                                            <div class="col-lg-9" style="margin-top: 6px;" id="DIV_AbrirLeitor" runat="server">
                                                <div class="form-group">
                                                    <br />
                                                    <asp:Button ID="cmdAbrirCâmera" OnClick="AbrirCamera_Click" CssClass="btn btn-sm btn-primary mb-2" Text="Abrir Leitor" runat="server" Style="float: right;" />
                                                </div>
                                            </div>

                                            <div class="col-lg-12 " runat="server" id="DIV_Incluir">
                                                <div id="div5" class="table-responsive" runat="server">
                                                    <asp:GridView ID="gv_IncluirItens" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed"
                                                        Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                        ShowFooter="False" DataKeyNames="idItem" OnRowDeleting="gv_IncluirItens_RowDeleting" OnRowDataBound="gv_IncluirItens_RowDataBound">
                                                        <Columns>
                                                            <asp:BoundField DataField="nOrdem" HeaderText="Ordem" />
                                                            <asp:BoundField DataField="SCodigo" HeaderText="Código" />
                                                            <asp:BoundField DataField="SdscProduto" HeaderText="Produto" />
                                                            <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade" />
                                                            <asp:BoundField DataField="sCodigoBarras" HeaderText="Código de Barras" />
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" CssClass="btn btn-small"><i class='fa-eraser fa'></i></asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>

                                            <div class="col-lg-9" style="margin-top: 6px;" id="DIV13" runat="server">
                                                <div class="form-group">
                                                    <br />
                                                    <asp:Button ID="btnEnvios" OnClick="btnEnvios_Click" CssClass="btn btn-sm btn-primary mb-2" Text="Efetuar Envios" runat="server" />
                                                </div>
                                            </div>

                                            <div class="col-lg-12 " runat="server" id="DIV18">
                                                <div id="div19" class="table-responsive" runat="server">
                                                    <asp:GridView ID="gv_Separados" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed"
                                                        Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                        ShowFooter="False" DataKeyNames="idProdutos" OnRowDataBound="gv_Separados_RowDataBound">
                                                        <Columns>
                                                            <asp:BoundField DataField="idProdutos" HeaderText="ID" />
                                                            <asp:BoundField DataField="sCodigo" HeaderText="Código" />
                                                            <asp:BoundField DataField="sDscProduto" HeaderText="Produto" />
                                                            <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade" />
                                                            <asp:BoundField DataField="sCodigoBarras" HeaderText="Código de Barras" />
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="panel panel-default" runat="server" id="div20">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Unitizados</b></h3>
                            </div>
                            <div class="panel-body row">
                                <div class="form-stacked">
                                    <div class="col-lg-12" runat="server" id="DIV21">
                                        <div class="row">

                                            <div class="col-lg-12 row" runat="server" id="DIV_Separacao">

                                                <div class="col-lg-12" runat="server" id="DIV_separacaoitens">
                                                    <div id="div_Unitizados" class="table-responsive" runat="server">
                                                        <asp:GridView ID="dtgUNItens" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed"
                                                            Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            ShowFooter="False" DataKeyNames="idItem" OnRowDeleting="dtgUNItens_RowDeleting" OnRowDataBound="dtgUNItens_RowDataBound">
                                                            <Columns>
                                                                <asp:BoundField DataField="nOrdem" HeaderText="Ordem" />
                                                                <asp:BoundField DataField="SCodigo" HeaderText="Código" />
                                                                <asp:BoundField DataField="SdscProduto" HeaderText="Produto" />
                                                                <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade" />
                                                                <asp:BoundField DataField="sCodigoBarras" HeaderText="Código Barras" />
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" CssClass="btn btn-small"><i class='fa-eraser fa'></i></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-lg-12 " runat="server" id="DIV_UNITIZADOS_Modal">
                                                <div class="row">
                                                    <div class="col-lg-4" id="DIV_IdEmbalagem" runat="server">
                                                        <div class="form-group">
                                                            <label for="txtIdEmbalagem">Tipo de Embalagem:</label>
                                                            <asp:DropDownList ID="ddlEmbalagemUnitizado" runat="server" class="form-control"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-6" id="DIV_sDscUnitizado" runat="server">
                                                        <div class="form-group">
                                                            <label for="txtIdEmbalagem">Descrição do Unitizado:</label>
                                                            <asp:TextBox ID="txtsDscUnitizado" class="form-control " runat="server" placeholder="Descrição Unitizado"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-2" style="margin-top: 25px;" id="DIV_ProximoUnitizado" runat="server">
                                                        <asp:Button ID="cmdProximoUnitizado" runat="server" Text="Adicionar Produtos" OnClick="ProximoUnitizado_Click" CssClass="btn btn-sm btn-success" />
                                                    </div>

                                                    <div class="col-lg-12 " id="DIV_UnitizadosItens" runat="server">
                                                        <div class="row">
                                                            <div class="col-lg-4">
                                                                <label>Código de Barras</label>
                                                                <asp:TextBox ID="txtsCodigoBarras" CssClass="form-control" runat="server" placeholder="Código de Barras"></asp:TextBox>
                                                            </div>
                                                            <div class="col-xs-6" style="margin-top: 25px;">
                                                                <asp:Button ID="IncluirUnitizadosItens" OnClick="IncluirUnitizadosItens_Click" CssClass="btn btn-sm btn-primary" Text="Incluir" runat="server" />
                                                            </div>

                                                            <div class="col-lg-2" style="margin-top: 6px;" id="DIV7" runat="server">
                                                                <div class="form-group">
                                                                    <br />
                                                                    <asp:Button ID="btnAbrirUnit" OnClick="btnAbrirUnit_Click" CssClass="btn btn-sm btn-primary mb-2" Text="Abrir Leitor" runat="server" Style="float: right;" />
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-12 " runat="server" id="DIV_gv_UnitizadosItens">
                                                                <br />
                                                                <div id="div9" class="table-responsive" runat="server">
                                                                    <asp:GridView ID="gv_UnitizadosItens" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed"
                                                                        Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" ShowFooter="False"
                                                                        DataKeyNames="idItem" OnRowDeleting="gv_UnitizadosItens_RowDeleting" OnRowDataBound="gv_UnitizadosItens_RowDataBound">
                                                                        <Columns>
                                                                            <asp:BoundField DataField="nOrdem" HeaderText="Ordem" />
                                                                            <asp:BoundField DataField="idUnitizadoItem" HeaderText="idUnitizadoItem" />
                                                                            <asp:BoundField DataField="SCodigo" HeaderText="Código" />
                                                                            <asp:BoundField DataField="SdscProduto" HeaderText="Produto" />
                                                                            <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade" />
                                                                            <asp:BoundField DataField="sCodigoBarras" HeaderText="Código Barras" />
                                                                            <asp:TemplateField>
                                                                                <ItemTemplate>
                                                                                    <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" CssClass="btn btn-small"><i class='fa-eraser fa'></i></asp:LinkButton>
                                                                                </ItemTemplate>
                                                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </div>
                                                            </div>

                                                            <div class="col-xs-6" style="margin-top: 25px;" runat="server" id="DIV_SalvarUnitizado">
                                                                <asp:Button ID="btnSalvarUnitizado" OnClick="btnSalvarUnitizado_Click" CssClass="btn btn-sm btn-primary" Text="Salvar Unitizado" runat="server" />
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-12 table-responsive" runat="server" id="aba_Unitizados">
                                                        <br />
                                                        <asp:GridView ID="gv_Unitizados" class="table table-striped table-bordered table-hover "
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            ShowFooter="False" Font-Names="Tahoma" DataKeyNames="idUnitizado" Font-Overline="False" Font-Size="Small"
                                                            OnRowDataBound="gv_Unitizados_RowDataBound" OnRowCommand="gv_Unitizados_RowCommand">
                                                            <Columns>

                                                                <asp:TemplateField HeaderText="ID">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="btnToggle" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-div-id='<%# "tr_Fatura" + Eval("idUnitizado") %>'></asp:LinkButton>
                                                                        <asp:Label runat="server" ID="idUnitizado" Text='<%# Bind("idUnitizado") %>' ClientIDMode="Static"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="left" VerticalAlign="Middle" Width="5%" />
                                                                </asp:TemplateField>

                                                                <asp:BoundField DataField="sCodigoBarras" HeaderText="Código Barras">
                                                                    <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sDscProdutoEmbalagem" HeaderText="Embalagem">
                                                                    <ItemStyle Width="35%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sFinalizado" HeaderText="sFinalizado">
                                                                    <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="Descrição">
                                                                    <ItemTemplate>
                                                                        <asp:Label runat="server" ID="sDscUnitizado" Text='<%# Bind("sDscUnitizado") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="left" VerticalAlign="Middle" Width="40%" />
                                                                </asp:TemplateField>

                                                                <asp:BoundField DataField="sStatus" HeaderText="Status">
                                                                    <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkEditar" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' runat="server" ToolTip="Editar" CssClass="btn btn-small" TabIndex="100" CommandName="Editar"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                                        <asp:LinkButton runat="server" ID="btnFecharCaixa" class="btn btn-sm btn-success" Text="Fechar Caixa" OnClick="btnFecharCaixa_Click"></asp:LinkButton>

                                                                        <%# NovaLinha(Eval("idUnitizado"), "_Fatura") %>

                                                                        <asp:GridView ID="gv_abaUnitizadosItens" class="table table-striped table-bordered table-hover table-condensed"
                                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                            HeaderStyle-CssClass="gvChildHeader" CssClass="gvRow" Style="padding: 0; margin: 0" AlternatingRowStyle-CssClass="gvAltRow"
                                                                            Font-Names="Tahoma" Font-Overline="False" DataKeyNames="idUnitizado, idUnitizadoItem" OnRowDataBound="gv_abaUnitizadosItens_RowDataBound">
                                                                            <Columns>

                                                                                <asp:BoundField DataField="idUnitizado" HeaderText="ID">
                                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="idUnitizado" />
                                                                                </asp:BoundField>

                                                                                <asp:BoundField DataField="idUnitizadoItem" HeaderText="ID">
                                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="idUnitizado" />
                                                                                </asp:BoundField>

                                                                                <asp:BoundField DataField="idProduto" HeaderText="ID">
                                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="idUnitizado" />
                                                                                </asp:BoundField>

                                                                                <asp:BoundField DataField="sDscProduto" HeaderText="Produto">
                                                                                    <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:BoundField>

                                                                                <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade">
                                                                                    <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:BoundField>

                                                                                <asp:BoundField DataField="sCodigoBarras" HeaderText="Código Barras">
                                                                                    <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:BoundField>

                                                                            </Columns>
                                                                        </asp:GridView>

                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
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
                        </div>

                        <div class="panel panel-default" runat="server" id="div22">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Envios</b></h3>
                            </div>
                            <div class="panel-body row">
                                <div class="form-stacked">
                                    <div class="col-lg-12" runat="server" id="DIV24">
                                        <div class="row">

                                            <div class="col-lg-12 " runat="server" id="DIV_ENVIOS">
                                                <div class="row">
                                                    <div class="col-lg-12" runat="server" id="DIV_dtgItens">
                                                        <div class="table table-responsive">
                                                            <asp:GridView ID="dtgItens" runat="server" class="table table-striped table-bordered table-hover table-condensed"
                                                                Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                ShowFooter="False" DataKeyNames="idProduto">
                                                                <Columns>
                                                                    <asp:BoundField DataField="sCodigo" HeaderText="Código" ItemStyle-Width="10%" />
                                                                    <asp:BoundField DataField="sDscProduto" HeaderText="Descrição" ItemStyle-Width="15%" />
                                                                    <asp:BoundField DataField="sUnidade" HeaderText="UN" ItemStyle-Width="2%" />
                                                                    <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade Separado" ItemStyle-Width="8%" />
                                                                    <asp:BoundField DataField="nQtdEnviado" HeaderText="Quantidade Enviada" ItemStyle-Width="8%" />
                                                                    <asp:BoundField DataField="nEnviar" HeaderText="Saldo" ItemStyle-Width="5%" />
                                                                </Columns>
                                                            </asp:GridView>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-12" style="margin-top: 6px;" id="DIV10" runat="server">
                                                        <div class="form-group">
                                                            <asp:Button ID="btnEnvio" OnClick="btnEnvio_Click" CssClass="btn btn-sm btn-primary mb-2" Text="Novo Envio" runat="server" />
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-12 row" id="DIV8" runat="server">
                                                        <div class="col-lg-4" runat="server" id="divCodigoB">
                                                            <div class="form-group">
                                                                <asp:TextBox ID="txtsCodigoBarrasVolume" CssClass="form-control" runat="server" placeholder="Código de Barras"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-4">
                                                            <div class="form-group">
                                                                <asp:DropDownList ID="ddlEmbalagem" runat="server" class="form-control"></asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-2">
                                                            <asp:Button ID="cmdIncluirVolume" OnClick="cmdIncluirVolume_Click" CssClass="btn btn-sm btn-primary" Text="Incluir" runat="server" />
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-12">
                                                        <div id="div_volumeItens" class="table-responsive" runat="server">
                                                            <asp:GridView ID="dtgVolumesItens" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed"
                                                                Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                ShowFooter="False" DataKeyNames="IdVolumeItem">
                                                                <Columns>
                                                                    <asp:BoundField DataField="SDscObjeto" HeaderText="Volume" />
                                                                    <asp:BoundField DataField="SCodigoBarras" HeaderText="Código de Barras" />
                                                                    <asp:TemplateField HeaderText="Ordem">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtnVolumeGV" CssClass="form-control" runat="server" Text='<%# Bind("nVolume") %>' TextMode="Number" min="0" step="1" Style="width: 80px;" />
                                                                        </ItemTemplate>
                                                                        <HeaderStyle Width="10%" />
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                            </asp:GridView>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-12" id="div_VolumesSalvos" runat="server">
                                                        <h4>Volumes - Salvos</h4>
                                                        <div class="table table-responsive">
                                                            <asp:GridView ID="dtgVolumes" runat="server" class="table table-striped table-bordered table-hover table-condensed"
                                                                Width="100%" GridLines="None"
                                                                HeaderStyle-CssClass="gvHeader" CssClass="gvRow" AlternatingRowStyle-CssClass="gvAltRow"
                                                                AutoGenerateColumns="False" OnRowDataBound="dtgVolumes_RowDataBound"
                                                                ShowFooter="False" DataKeyNames="idVolume">
                                                                <Columns>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <asp:LinkButton ID="LinkButton1" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-div-id='<%# "tr" + Eval("idVolume") %>'></asp:LinkButton>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <asp:CheckBox ID="chkOpcaoItemVolume" runat="server" OnCheckedChanged="chkOpcaoItemVolume_CheckedChanged" AutoPostBack="true" />
                                                                        </ItemTemplate>
                                                                        <HeaderStyle Width="10%" />
                                                                    </asp:TemplateField>
                                                                    <asp:BoundField DataField="idVolume" HeaderText="ID" />
                                                                    <asp:BoundField DataField="sDscVolume" HeaderText="Volume" />
                                                                    <asp:BoundField DataField="sEmbalagem" HeaderText="Embalagem" />
                                                                    <asp:TemplateField HeaderText="Ação">
                                                                        <ItemTemplate>
                                                                            <asp:LinkButton ID="cmdAbrirArquivos" runat="server" OnClick="cmdAbrirArquivos_Click" CommandArgument='<%# Eval("idVolume") %>'><i class="fa fa-upload"></i>
                                                                            </asp:LinkButton>
                                                                            <%# NovaLinha(Eval("idVolume"),"") %>

                                                                            <asp:GridView ID="dtgVolumesItensGV" runat="server" class="table table-striped table-bordered table-hover table-condensed"
                                                                                Width="100%" GridLines="None"
                                                                                HeaderStyle-CssClass="gvHeader" CssClass="gvRow" AlternatingRowStyle-CssClass="gvAltRow"
                                                                                AutoGenerateColumns="False" OnRowDataBound="dtgVolumesItensGV_RowDataBound"
                                                                                ShowFooter="False" DataKeyNames="idObjeto">
                                                                                <Columns>
                                                                                    <asp:TemplateField>
                                                                                        <ItemTemplate>
                                                                                            <asp:LinkButton ID="btnToggle" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-div-id='<%# "trChild" + Eval("idVolumeItem") %>'></asp:LinkButton>

                                                                                        </ItemTemplate>
                                                                                    </asp:TemplateField>
                                                                                    <asp:BoundField DataField="SCodigoProduto" HeaderText="Código Produto" />
                                                                                    <asp:BoundField DataField="SDscObjeto" HeaderText="Produto" />
                                                                                    <asp:BoundField DataField="NQuantidade" HeaderText="Quantidade" />
                                                                                    <asp:BoundField DataField="SControlaGarantia" HeaderText="Garantia" />
                                                                                    <asp:TemplateField HeaderText="Código Barras">
                                                                                        <ItemTemplate>
                                                                                            <%# Eval("sCodigoBarras") %>
                                                                                            <%# NovaLinha(Eval("idVolumeItem"), "Child") %>

                                                                                            <asp:GridView ID="dtgItensVL" runat="server" Width="100%"
                                                                                                GridLines="None" AutoGenerateColumns="false" DataKeyNames="idUnitizadoItem"
                                                                                                HeaderStyle-CssClass="gvChildHeader" CssClass="gvRow" Style="padding: 0; margin: 0"
                                                                                                AlternatingRowStyle-CssClass="gvAltRow">
                                                                                                <Columns>
                                                                                                    <asp:BoundField DataField="SCodigo" HeaderText="Código Produto" />
                                                                                                    <asp:BoundField DataField="SdscProduto" HeaderText="Produto" />
                                                                                                    <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade" />
                                                                                                    <asp:BoundField DataField="sControlaGarantiaLote" HeaderText="Garantia" />
                                                                                                    <asp:BoundField DataField="sCodigoBarras" HeaderText="Código Barras" />
                                                                                                </Columns>

                                                                                            </asp:GridView>
                                                                                        </ItemTemplate>
                                                                                    </asp:TemplateField>
                                                                                </Columns>
                                                                            </asp:GridView>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                            </asp:GridView>
                                                        </div>
                                                    </div>
                                                    <div id="Div_FormEnvios" class="col-lg-12" runat="server">
                                                        <div class="row">
                                                            <div class="col-md-4">
                                                                <div class="form-group">
                                                                    <label for="txtDtEnvio">Data do Envio:</label>
                                                                    <asp:TextBox ID="txtDtEnvio" runat="server" CssClass="form-control col-lg-2" type="date"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <br />
                                                        <div class="row">
                                                            <div class="col-md-4">
                                                                <div class="form-group">
                                                                    <label for="txtNPesoLiquido">Peso Líquido:</label>
                                                                    <asp:TextBox ID="txtNPesoLiquido" runat="server" CssClass="form-control" placeholder="Peso Líquido"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="col-md-4">
                                                                <div class="form-group">
                                                                    <label for="txtNPesoBruto">Peso Bruto:</label>
                                                                    <asp:TextBox ID="txtNPesoBruto" runat="server" CssClass="form-control" placeholder="Peso Bruto"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row">

                                                            <div class="col-md-4">
                                                                <div class="form-group">
                                                                    <label for="txtNLargura">Largura:</label>
                                                                    <asp:TextBox ID="txtNLargura" runat="server" CssClass="form-control" placeholder="Largura" onchange="calcularPesoTotal()"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="col-md-4">
                                                                <div class="form-group">
                                                                    <label for="txtNAltura">Altura:</label>
                                                                    <asp:TextBox ID="txtNAltura" runat="server" CssClass="form-control" placeholder="Altura" onchange="calcularPesoTotal()"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="col-md-4">
                                                                <div class="form-group">
                                                                    <label for="txtNComprimento">Comprimento:</label>
                                                                    <asp:TextBox ID="txtNComprimento" runat="server" CssClass="form-control" placeholder="Comprimento" onchange="calcularPesoTotal()"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="col-md-4">
                                                                <div class="form-group">
                                                                    <label id="lblVolumeCubico" runat="server"></label>
                                                                </div>
                                                            </div>
                                                        </div>

                                                    </div>

                                                    <div id="div_botoesVolumes" runat="server" class="col-lg-12">
                                                        <asp:Button ID="cmdGerarVolume" OnClick="cmdGerarVolume_Click" CssClass="btn btn-sm btn-primary" Text="Criar Volume" runat="server" />
                                                        <asp:Button ID="cmdEfetuarEnvio" runat="server" Text="Efetuar Envio" Visible="false" CssClass="btn btn-success" OnClick="cmdEfetuarEnvio_Click" />
                                                    </div>

                                                    <div id="divTableEnvios" class="col-lg-12" runat="server">
                                                        <br />
                                                        <div class="table table-responsive">
                                                            <asp:GridView ID="dtgvEnvios" runat="server" class="table table-striped table-bordered table-hover table-condensed"
                                                                Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" DataKeyNames="idOPI" OnRowDataBound="dtgvEnvios_RowDataBound">
                                                                <Columns>
                                                                    <asp:HyperLinkField DataNavigateUrlFields="idEnvioOPI"
                                                                        DataTextField="idEnvioOPI" HeaderText="Nº Envio"
                                                                        DataNavigateUrlFormatString="Envio_Detalhe.aspx?id={0}">
                                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        <HeaderStyle Width="4%" />
                                                                    </asp:HyperLinkField>

                                                                    <asp:BoundField DataField="dtEnvio" HeaderText="Data do Envio" />
                                                                    <asp:BoundField DataField="nQtdVolumes" HeaderText="Qtd de Volumes" />
                                                                    <asp:BoundField DataField="sDscUsuarioEnvio" HeaderText="Usuário do Envio" />
                                                                    <asp:TemplateField HeaderText="Gerar">
                                                                        <ItemTemplate>
                                                                            <asp:LinkButton ID="cmdGerarExcelEnvio" CssClass="btn btn-sm btn-success" runat="server" OnClick="GerarExcelEnvio_Click" CommandArgument='<%# Eval("idEnvioOPI") %>'><i class="fa fa-table"></i> Excel Envio</asp:LinkButton>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle Width="4%" />
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                            </asp:GridView>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>

                                            <div id="ViewArquivos2" runat="server">
                                                <h3>Adicione as Imagens Abaixo:</h3>
                                                <div class="col-lg-12" runat="server" id="div_arquivos2">
                                                    <div class=" embed-responsive embed-responsive-16by9" style="min-height: 200px">
                                                        <embed type="text/html" runat="server" id="frmArquivos2" width="800" height="500" />
                                                    </div>
                                                </div>
                                                <div class="col-lg-12" runat="server" id="div14">
                                                    <asp:Button ID="cmdVoltarModal" runat="server" Text="Voltar" OnClick="cmdVoltarModal_Click" CssClass="btn btn-warning" />
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </ContentTemplate>
                </asp:UpdatePanel>

                <fieldset class="form-stacked actions">
                    <asp:Button ID="btnSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" ValidationGroup="DETALHE" OnClick="btnSalvar_Click" />
                    <a href="javascript:history.back()" class="btn btn-lg btn-warning" title="Voltar">Voltar</a>
                    <asp:Button ID="ExportarExcel" CssClass="btn btn-lg  btn-success" runat="server" Text="Excel PL" OnClick="ExportarExcel_Click" />
                </fieldset>

            </div>

            <div role="tabpanel" class="tab-pane fade" id="Historico" aria-labelledby="Historico-tab">
                <br />
                <div class="panel panel-default" runat="server" id="DIV16">
                    <div class="panel-heading">
                        <h3 class="panel-title"><b>Histórico</b></h3>
                    </div>
                    <div class="panel-body">
                        <div class="form-stacked row">
                            <div class="col-lg-12">
                                <uc1:MensagemPagina runat="server" ID="MensagemPagina2" />
                            </div>
                            <div class="col-lg-12 table-responsive">
                                <asp:GridView ID="gvHistorico" class="table table-striped table-bordered table-hover "
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False" Font-Names="Tahoma" DataKeyNames="idSeparacao" Font-Overline="False" Font-Size="Small"
                                    OnRowDataBound="gvHistorico_RowDataBound">
                                    <Columns>

                                        <asp:BoundField DataField="idSeparacao" HeaderText="ID">
                                            <ItemStyle Width="2%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sAcao" HeaderText="Ação">
                                            <ItemStyle Width="7%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscAcao" HeaderText="Descrição">
                                            <ItemStyle Width="15%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade">
                                            <ItemStyle Width="5%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sCodigoBarras" HeaderText="Codigo de Barras">
                                            <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário">
                                            <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="dtatualizacao" HeaderText="Data">
                                            <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
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

    <div class="modal fade" id="modalEnvio" tabindex="-1" role="dialog" aria-labelledby="modalEnvioLabel" aria-hidden="true" data-backdrop="static">
        <asp:UpdatePanel ID="updModal" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="modal-dialog modal-xl" role="document">
                    <div class="modal-content ">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <h1 class="modal-title" id="lblModalEnvio">Envios</h1>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaUnitizados" />
                                </div>
                                <div id="divMultiView" runat="server" class="col-lg-12">
                                    <div id="divFormEnvios" class="col-lg-12" runat="server">
                                        <asp:MultiView ID="MultiViewUnitizados" runat="server" ActiveViewIndex="0">
                                            <asp:View ID="ViewMenu" runat="server">
                                                <div class="form-stacked row">
                                                    <div id="div15" class="col-lg-12" runat="server">
                                                        <div class="table table-responsive">
                                                            <asp:GridView ID="gv_EnviosVi" runat="server" class="table table-striped table-bordered table-hover table-condensed"
                                                                Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" DataKeyNames="idOPI" OnRowDataBound="dtgvEnvios_RowDataBound">
                                                                <Columns>
                                                                    <asp:HyperLinkField DataNavigateUrlFields="idEnvioOPI"
                                                                        DataTextField="idEnvioOPI" HeaderText="Nº Envio"
                                                                        DataNavigateUrlFormatString="Envio_Detalhe.aspx?id={0}">
                                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        <HeaderStyle Width="4%" />
                                                                    </asp:HyperLinkField>

                                                                    <asp:BoundField DataField="dtEnvio" HeaderText="Data do Envio" />
                                                                    <asp:BoundField DataField="nQtdVolumes" HeaderText="Qtd de Volumes" />
                                                                    <asp:BoundField DataField="sDscUsuarioEnvio" HeaderText="Usuário do Envio" />
                                                                    <asp:TemplateField HeaderText="Gerar">
                                                                        <ItemTemplate>
                                                                            <asp:LinkButton ID="cmdGerarExcelEnvio" CssClass="btn btn-sm btn-success" runat="server" OnClick="GerarExcelEnvio_Click" CommandArgument='<%# Eval("idEnvioOPI") %>'><i class="fa fa-table"></i> Excel Envio</asp:LinkButton>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle Width="4%" />
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                            </asp:GridView>
                                                        </div>
                                                    </div>
                                                </div>
                                            </asp:View>
                                        </asp:MultiView>
                                        <div>
                                            <div class="col-lg-12 row">
                                                <button id="Button2" runat="server" type="button" class="btn btn-lg btn-warning" data-dismiss="modal" aria-label="Fechar">Fechar</button>
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

    <div class="modal fade" id="modalUnitizado" tabindex="-1" role="dialog" aria-labelledby="modalUnitizadoLabel" aria-hidden="true" data-backdrop="static">
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="modal-dialog modal-xl" role="document">
                    <div class="modal-content ">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <h1 class="modal-title" id="lblmodalUnitizado" runat="server">Leitor</h1>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">

                                <div class="row">
                                    <div class="col-xs-12" id="DivBipadorVolume" runat="server">
                                        <uc1:LeitorQuagga runat="server" ID="LeitorQuagga1" txtClient="cphCorpo_txtsCodigoEAN" click="cphCorpo_cmdIncluirUnitizadoItem" />
                                    </div>
                                </div>

                                <div class="row" runat="server" id="DIV_CodigoEAN">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaUnItens" />
                                    <div class="col-lg-12" id="DivManual" runat="server">
                                        <div class="col-lg-4">
                                            <asp:TextBox ID="txtsCodigoEAN" CssClass="form-control" runat="server" placeholder="Código Barras"></asp:TextBox>
                                        </div>
                                        <div class="col-lg-6">
                                            <asp:Button ID="cmdIncluirUnitizadoItem" OnClick="cmdIncluirUnitizadoItem_Click" CssClass="btn btn-sm btn-primary" Text="Incluir" runat="server" />
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

    <asp:HiddenField ID="hddidObjeto" runat="server" />
    <asp:HiddenField ID="hddsTipoObjeto" runat="server" />
    <asp:HiddenField ID="hfScrollPosition" runat="server" />
    <asp:HiddenField ID="hddidOPI" runat="server" />
    <asp:HiddenField ID="hddidProduto" runat="server" />
    <asp:HiddenField ID="hddidPedido" runat="server" />
    <asp:HiddenField ID="hddidUnitizado" runat="server" />
    <asp:HiddenField ID="hddidVolume" runat="server" />
    <asp:HiddenField ID="hddidEnvioOPI" runat="server" />
    <asp:HiddenField ID="hddidStatus" runat="server" />
    <asp:HiddenField ID="hddsDscOPI" runat="server" />
    <asp:HiddenField ID="hddnPedido" runat="server" />
    <asp:HiddenField ID="hddleitura" runat="server" />
    <asp:HiddenField ID="hddultimaleitura" runat="server" />
    <asp:HiddenField ID="hddIncluirItem" runat="server" />

</asp:Content>
