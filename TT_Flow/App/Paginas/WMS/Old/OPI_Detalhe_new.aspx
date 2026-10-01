<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="OPI_Detalhe_new.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.OPI_Detalhe_new" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/LeitorQuagga.ascx" TagPrefix="uc1" TagName="LeitorQuagga" %>




<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmOPI_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <style>
        .colorLink {
            background-color: aqua;
        }

        .resumir-label {
            display: inline-block;
            width: 1000px; /* largura máxima da label */
            white-space: nowrap; /* evita que o texto quebre em várias linhas */
            overflow: hidden; /* oculta o texto que não cabe na largura definida */
            text-overflow: ellipsis; /* exibe reticências (...) quando o texto é cortado */
        }

        /*modal*/
        .modal-header-content {
            display: flex;
            align-items: center;
        }

        .modal-xl {
            max-width: 65%; /* Ajuste conforme necessário */
            width: 65%;
        }

        .modal-xxl {
            width: 95% !important;
            max-width: 95% !important;
            margin: 30px auto;
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

        /*menu modal*/
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

        .arrow {
            margin-left: 5px; /* Espaço entre o texto e a seta */
        }

        .activeMn {
            background-color: #005a12; /* Cor mais escura para o estado ativo */
            color: #ffffff; /* Texto branco no estado ativo */
        }

        /* GridColapsada*/
        /*CSS Colapso da Tabela*/
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

        .bg-tt {
            background-color: #009a22 !important;
        }


        /* Estilo para quando a grid estiver vazia */
        .empty-data-placeholder {
            text-align: center;
            padding: 40px 20px;
            background-color: #f9f9f9;
            border: 2px dashed #ddd;
            border-radius: 8px;
            color: #888;
        }

            .empty-data-placeholder i {
                font-size: 3em;
                color: #ccc;
                margin-bottom: 15px;
                display: block;
            }

            .empty-data-placeholder h4 {
                font-weight: bold;
                margin-bottom: 5px;
            }
    </style>
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>
    <script src="/app/js/vanilla-masker.js"></script>

    <div class="form-stacked">
        <div class="col-lg-12 row">
            <h1>
                <asp:Label CssClass="resumir-label" ID="lblTituloPagina" runat="server" Text="Ordem de Produção Interna"></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
        </div>

        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

        <div>
            <ul id="tab_OPI" class="nav nav-tabs" role="tablist">
                <li role="presentation" class="tabpanel active">
                    <a href="#OPI" id="aba_OPI" role="tabpanel" data-toggle="tab" aria-controls="OPI" aria-expanded="false"><b>OPI</b></a>
                </li>
                <li role="presentation" class="tabpanel">
                    <a href="#Etiqueta" id="aba_Etiqueta" role="tabpanel" data-toggle="tab" aria-controls="Etiqueta" aria-expanded="false"><b>Etiquetas</b></a>
                </li>
            </ul>
        </div>

        <br />
        <div id="div_main" runat="server">
            <div id="tab" class="tab-content">

                <div role="tabpanel" class="tab-pane fade in active" id="OPI" aria-labelledby="aba_OPI">
                    <asp:UpdatePanel ID="updDetalhe" runat="server">
                        <ContentTemplate>
                            <div class="panel panel-default" runat="server" id="div_OPI">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>Dados da OPI</b></h3>
                                </div>
                                <div class="panel-body">

                                    <div class="form-stacked row">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <div class="col-lg-2">
                                                    <div class="form-group">

                                                        <label>Nº da OPI</label>

                                                        <div class="form-group input-group">

                                                            <asp:LinkButton ID="cmRetornar" Style="background-color: #009a22; color: white;" class="btn btn-sm btn-info input-group-addon" runat="server" OnClick="cmdRetornar_click"><b><i class="fa fa-arrow-circle-left"></i></b></asp:LinkButton>

                                                            <asp:TextBox ID="txtidOPI" class="form-control" runat="server" disabled="true"></asp:TextBox>

                                                            <asp:LinkButton ID="cmAvancar" Style="background-color: #009a22; color: white;" class="btn btn-sm btn-info input-group-addon" runat="server" OnClick="cmdAvancar_click"><b><i class="fa fa-arrow-circle-right"></i></b></asp:LinkButton>

                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-lg-2">
                                                    <div class="form-group">
                                                        <label>Data</label>
                                                        <asp:TextBox ID="txtdtOPI" class="form-control" type="date" runat="server" disabled="true"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-12">
                                                <div class="col-lg-2">
                                                    <div class="form-group">
                                                        <label>Nº Pedido</label>
                                                        <div class="form-group input-group">
                                                            <asp:TextBox ID="txtnPedido" class="form-control" runat="server" disabled="true" Style="background-color: #f0f8ff; color: #00008b;"></asp:TextBox>
                                                            <asp:LinkButton Style="background-color: #009a22; color: white;" ID="cmdVerPedido"
                                                                class="btn btn-sm btn-primary input-group-addon" runat="server" OnClick="cmdVerPedido_click" CommandArgument='<%# Eval("idPedido") %>'> <b><i class="fa fa-eye"></i></b></asp:LinkButton>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-lg-6">
                                                    <div class="form-group">
                                                        <label>Descrição da OPI</label>
                                                        <asp:TextBox ID="txtsDscOPI" class="form-control " runat="server" disabled="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2">
                                                    <label>Status</label>
                                                    <asp:DropDownList ID="ddlStatus" runat="server" class="form-control" disabled="true">
                                                        <asp:ListItem class="danger" Value="-1">Selecione </asp:ListItem>
                                                        <asp:ListItem Value="1">Pendente</asp:ListItem>
                                                        <asp:ListItem Value="2">Em Andamento</asp:ListItem>
                                                        <asp:ListItem Value="3">Envio Parcial</asp:ListItem>
                                                        <asp:ListItem Value="4">Finalizada</asp:ListItem>
                                                        <asp:ListItem Value="5">Cancelada</asp:ListItem>
                                                        <asp:ListItem Value="6">Excluída</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>

                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-12">
                                                <div class="col-lg-2">
                                                    <div class="form-group">
                                                        <label>Referência</label>
                                                        <asp:TextBox ID="txtsReferencia" class="form-control " runat="server" disabled="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-4">
                                                    <div class="form-group">
                                                        <label>Cliente</label>
                                                        <asp:TextBox ID="txtsCliente" class="form-control " runat="server" disabled="true"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="panel-footer" style="background-color: #f9f9f9; border-top: 1px solid #e7e7e7; padding: 15px;">
                                    <div class="row">
                                        <div class="col-lg-6 hidden-xs">
                                            <small class="text-muted"><i class="fa fa-info-circle"></i>&nbsp Ações disponíveis: </small>
                                        </div>

                                        <div class="col-lg-6 text-right">
                                            <asp:LinkButton ID="cmdAbrirEtiqueta" OnClick="cmdAbrirEtiqueta_Click"
                                                runat="server" CssClass="btn btn-default" Style="border-color: #ccc;"
                                                ToolTip="Imprimir etiquetas de saída">
    <i class="fa fa-barcode"></i>&nbsp; Imprimir Etiquetas
</asp:LinkButton>
                                            <asp:LinkButton ID="cmdAbrirUnitizados" OnClick="cmdAbrirUnitizados_Click"
                                                runat="server" CssClass="btn btn-primary"
                                                ToolTip="Gerenciar caixas e unitização">
                               <i class="fa fa-box"></i>&nbsp; Unitizar Itens
                           </asp:LinkButton>

                                            &nbsp;
                              

                                        </div>
                                    </div>
                                </div>
                            </div>

                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <asp:UpdatePanel ID="UpdatePanelItens" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="panel panel-default" runat="server" id="Div_Itens">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>Itens</b></h3>
                                </div>
                                <div class="panel-body">

                                    <div class="form-group row">
                                        <div class="col-lg-12">
                                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaItens" />
                                        </div>

                                        <div class="col-lg-12">
                                            <div class="table table-responsive">

                                                <asp:GridView ID="dtgItens" runat="server" class="table table-striped table-bordered table-hover table-condensed"
                                                    Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    ShowFooter="False" DataKeyNames="idProduto">
                                                    <%--OnRowDataBound="dtgItens_RowDataBound"--%>
                                                    <Columns>
                                                        <asp:BoundField DataField="sCodigo" HeaderText="Código" ItemStyle-Width="10%" />
                                                        <asp:BoundField DataField="nOrdem" HeaderText="Ordem" ItemStyle-Width="1%" />
                                                        <asp:TemplateField HeaderText="Descrição">
                                                            <ItemTemplate>
                                                                <div>

                                                                    <%--Thiago * 16/09/2024---------%>
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
                                                                    <%------------------------------------%>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="sUnidade" HeaderText="UN" ItemStyle-Width="2%" />
                                                        <asp:BoundField DataField="nQuantidade" HeaderText="Qtd Projeto" ItemStyle-Width="8%" />
                                                        <asp:BoundField DataField="nQtdEnviado" HeaderText="Qtd Enviada" ItemStyle-Width="8%" />
                                                        <asp:BoundField DataField="nEnviar" HeaderText="Saldo" ItemStyle-Width="5%" />
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <asp:UpdatePanel ID="updEnvios" runat="server">
                        <ContentTemplate>
                            <div class="panel panel-default" runat="server" id="Div_Envios">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>Envios</b></h3>
                                </div>
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-lg-12">
                                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaAbaEnvios" />
                                        </div>
                                        <div class="col-lg-12">
                                            <asp:Button ID="cmdAbrir" OnClick="AbrirModal_Click" CssClass="btn btn-sm btn-success" Text="Novo Envio" runat="server" />
                                        </div>
                                    </div>

                                    <br />

                                    <div class="row">

                                        <div id="divTableEnvios" class="col-lg-12" runat="server">
                                            <div class="table table-responsive">
                                                <asp:GridView ID="dtgvEnvios" runat="server" class="table table-striped table-bordered table-hover table-condensed"
                                                    Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" DataKeyNames="idOPI" OnRowDataBound="dtgItens_RowDataBound">
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
                            </div>

                        </ContentTemplate>
                    </asp:UpdatePanel>


                    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                    <br />
                </div>


                <div role="tabpanel" class="tab-pane" id="Etiqueta" aria-labelledby="aba_Etiqueta">

                    <asp:UpdatePanel runat="server" ID="updEtiquetas">
                        <ContentTemplate>
                            <div class="panel panel-default" runat="server" id="Div1">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>Gerenciador de Etiquetas</b></h3>
                                </div>
                                <div class="panel-body">

                                    <div class="row">
                                        <div class="col-lg-12">
                                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaEtiquetaPN" />
                                        </div>
                                    </div>

                                    <br />

                                    <div class="row">
                                        <div class="col-lg-12">
                                            <asp:GridView ID="dtgEtiquetas" runat="server" class="table table-striped table-bordered table-hover table-condensed"
                                                Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                ShowFooter="False" DataKeyNames="idOPI,idProduto" OnRowDataBound="dtgEtiquetas_RowDataBound">

                                                <EmptyDataTemplate>
                                                    <div class="alert alert-warning text-center" role="alert">
                                                        <i class="fa fa-exclamation-triangle"></i>Nenhuma etiqueta gerada ainda. Utilize o botão "Imprimir Etiquetas" para gerar.
               
                                                    </div>
                                                </EmptyDataTemplate>

                                                <Columns>
                                                    <asp:BoundField DataField="SCodigoBarras" HeaderText="Código de Barras" />
                                                    <asp:BoundField DataField="SdscEtiqueta" HeaderText="Etiqueta" />
                                                    <asp:BoundField DataField="NQuantidade" HeaderText="Quantidade" />
                                                    <asp:BoundField DataField="Sobservacao" HeaderText="Observação" />
                                                    <%-- <asp:BoundField DataField="SLote" HeaderText="Lote" />--%>
                                                    <asp:BoundField DataField="DtImpressao" HeaderText="Data de Impressão" />
                                                    <asp:TemplateField HeaderText="Ação" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="cmdExcluirEtiqueta" CssClass="btn btn-sm btn-danger" runat="server" Visible="false" OnClick="ExcluirEtiquetas_Click" CommandArgument='<%# Eval("idEtiqueta") %>'><i class="fa fa-trash-o"></i></asp:LinkButton>
                                                            <%--   <asp:LinkButton ID="cmdAdicionarSerie" runat="server" CssClass="btn btn-primary btn-sm" Visible="false" CommandArgument='<%# Eval("idEtiqueta") %>' OnClick="cmdAdicionarSerie_Click">
                                                            <i class="fa-plus fa"></i> 
                                                            </asp:LinkButton>--%>
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="10%" />
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="SGarantia" HeaderText="Possui Garantia?" Visible="false" />
                                                    <asp:BoundField DataField="idProduto" HeaderText="Id Produto" Visible="false" />
                                                    <asp:BoundField DataField="sCodigo" HeaderText="Código Produto" Visible="false" />
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
            </div>

            <%----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>


            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-info" runat="server" Text="Alterar Status" ValidationGroup="DETALHE" />
                <asp:Button ID="ExportarExcel" CssClass="btn btn-lg  btn-success" runat="server" Text="Excel PL" OnClick="ExportarExcel_Click" />
                <%----%>
                <asp:Button ID="cmdExcluir" Text="Excluir" runat="server" class="btn btn-lg btn-danger" OnClick="Excluir_Click" />
                <asp:Button ID="cmdVoltar" class="btn  btn-lg btn-warning" runat="server" Text="Voltar" OnClick="Voltar_Click" />
            </fieldset>
            <div id="dialog-Salvar" class="modal" title="Salvar">
                <p>
                    <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                    <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
                </p>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalEnvio" tabindex="-1" role="dialog" aria-labelledby="modalEnvioLabel" aria-hidden="true" data-backdrop="static">
        <asp:UpdatePanel ID="updModal" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="modal-dialog modal-xxl" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <h1 class="modal-title" id="lblModalEnvio">Novo Envio 
                          <asp:Label ID="lbltituloModal" Text="" runat="server" Visible="false" />
                            </h1>
                        </div>
                        <div class="modal-body">
                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaEnvios" />
                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaVolume" />

                            <div class="mini-menu mb-4">
                                <ul class="nav nav-pills">
                                    <li class="nav-item">
                                        <asp:LinkButton ID="btnAbaConfig" runat="server" CssClass="nav-link activeMn" OnClick="AbrirVolumes_Click">
                                        <i class="fa fa-box"></i> 1. Montagem do Volume
                                        </asp:LinkButton>
                                    </li>
                                </ul>
                            </div>
                            <br />
                            <asp:MultiView ID="MultiViewFormularios" runat="server" ActiveViewIndex="0">

                                <%-- VIEW 1: MONTAGEM E CONFIGURAÇÃO --%>
                                <asp:View ID="ViewVolumes" runat="server">
                                    <div class="row">
                                        <div class="col-lg-4 border-right">
                                            <div class="panel panel-default">
                                                <div class="panel-heading bg-tt" style="color: white;"><b><i class="fa fa-barcode"></i>Bipagem</b></div>
                                                <div class="panel-body">
                                                    <div class="form-group">
                                                        <asp:Button ID="cmdAbrirCamVolume" OnClick="AbrirCameraVolume_Click" CssClass="btn btn-block btn-primary" Text="Abrir Câmera/Leitor" runat="server" />
                                                    </div>

                                                    <div id="DivBipadorVolume" runat="server" class="mb-3 text-center" visible="false">
                                                        <uc1:LeitorQuagga runat="server" ID="LeitorQuagga" STemParametros="S" />
                                                    </div>

                                                    <div class="form-group" id="divCodigoB" runat="server">
                                                        <label>Código de Barras</label>
                                                        <div class="input-group">
                                                            <span class="input-group-addon"><i class="fa fa-qrcode"></i></span>
                                                            <asp:TextBox ID="txtsCodigoBarrasVolume" CssClass="form-control" runat="server" placeholder="PR, UN ou VOL..."></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="form-group">
                                                        <label>Embalagem</label>
                                                        <asp:DropDownList ID="ddlEmbalagem" runat="server" CssClass="form-control"></asp:DropDownList>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-md-6" style="padding-right: 2px;">
                                                            <asp:Button ID="cmdIncluirVolume" OnClick="IncluirVolumesItens_Click" CssClass="btn btn-success btn-block" Text="Adicionar Item" runat="server" />
                                                        </div>
                                                        <div class="col-md-6" style="padding-left: 2px;">
                                                            <asp:Button ID="cmdImportarEtiquetas" OnClick="ImportarEtiquetas_Click" CssClass="btn btn-info btn-block" Text="📥 Importar Pendentes" runat="server" ToolTip="Importa todas as etiquetas desta OPI que ainda não estão em nenhum volume." />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-8">
                                            <div class="panel panel-default">
                                                <div class="panel-heading bg-tt" style="color: white;"><b><i class="fa fa-list"></i>Itens no Volume Atual</b></div>

                                                <div class="table-responsive" style="min-height: 250px; max-height: 400px;">
                                                    <asp:GridView ID="dtgVolumesItens" runat="server"
                                                        CssClass="table table-bordered gvRow"
                                                        HeaderStyle-CssClass="gvHeader"
                                                        AlternatingRowStyle-CssClass="gvAltRow"
                                                        AutoGenerateColumns="False" DataKeyNames="IdVolumeItem"
                                                        OnRowCommand="dtgVolumesItens_RowCommand">
                                                        <Columns>
                                                            <asp:BoundField DataField="SCodigoProduto" HeaderText="Código" />
                                                            <asp:BoundField DataField="SDscObjeto" HeaderText="Item/Unitizado Adicionado" />
                                                            <asp:BoundField DataField="NQuantidade" HeaderText="Qtd" />
                                                            <asp:TemplateField HeaderText="Ordem" ItemStyle-Width="100px" Visible="false">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtnVolumeGV" CssClass="form-control input-sm text-center" runat="server" Text='<%# Bind("nVolume") %>' TextMode="Number" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Ação" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="btnExcluirItem" runat="server"
                                                                        CommandName="Excluir"
                                                                        CommandArgument='<%# Container.DataItemIndex %>'
                                                                        CssClass="btn btn-danger btn-xs"
                                                                        ToolTip="Remover Item da Lista">
                                                                           <i class="fa fa-trash-o"></i>
                                                                       </asp:LinkButton>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>

                                                        <%-- EMPTY STATE GRID 1 --%>
                                                        <EmptyDataTemplate>
                                                            <div class="empty-data-placeholder">
                                                                <i class="fa fa-arrow-left"></i>
                                                                <h4>Volume Vazio</h4>
                                                                <p>Utilize o leitor ou digite o código ao lado para adicionar itens.</p>
                                                            </div>
                                                        </EmptyDataTemplate>

                                                    </asp:GridView>
                                                </div>
                                                <div class="panel-footer text-right">
                                                    <asp:Button ID="Button2" OnClick="GerarVolume_Click" CssClass="btn btn-primary btn-lg" Text="📦 Fechar e Salvar Volume" runat="server" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <hr />

                                    <div id="div_VolumesSalvos" runat="server" class="mt-4">
                                        <h4 style="color: #1A4C1A;"><i class="fa fa-check-circle"></i>Volumes Prontos para Envio</h4>
                                        <div class="table-responsive">
                                            <asp:GridView ID="dtgVolumes" runat="server"
                                                CssClass="table table-bordered gvRow"
                                                HeaderStyle-CssClass="gvHeader"
                                                AlternatingRowStyle-CssClass="gvAltRow"
                                                DataKeyNames="idVolume" AutoGenerateColumns="False" OnRowDataBound="dtgVolume_RowDataBound">
                                                <Columns>
                                                    <%-- Botão de Expandir (+) só nesta grid principal --%>
                                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="40px">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="btnToggle" runat="server" CssClass="toggle-icon btn btn-default btn-xs" data-div-id='<%# "tr" + Eval("idVolume") %>'>
                                                            
                                                            </asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Enviar?" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="60px">
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chkOpcaoItemVolume" runat="server" OnCheckedChanged="chkOpcaoItemVolume_CheckedChanged" AutoPostBack="true" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:BoundField DataField="idVolume" HeaderText="ID" HeaderStyle-Width="60px" />
                                                    <asp:BoundField DataField="sDscVolume" HeaderText="Identificação" />
                                                    <asp:BoundField DataField="sEmbalagem" HeaderText="Embalagem" />

                                                    <asp:TemplateField HeaderText="Ações" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="cmdAbrirArquivos" runat="server" OnClick="AbrirArquivosVolumes_Click" CommandArgument='<%# Eval("idVolume") %>' CssClass="btn btn-default btn-xs" ToolTip="Ver Fotos">
                                                            <i class="fa fa-camera"></i>
                                                            </asp:LinkButton>

                                                            <%# NovaLinha(Eval("idVolume"),"") %>

                                                            <asp:GridView ID="dtgVolumesItensGV" runat="server"
                                                                CssClass="table table-bordered table-condensed"
                                                                HeaderStyle-CssClass="gvChildHeader"
                                                                AutoGenerateColumns="False" Style="margin: 10px 0; background-color: #fff;">
                                                                <Columns>
                                                                    <asp:BoundField DataField="SDscObjeto" HeaderText="Conteúdo do Volume" />
                                                                    <asp:BoundField DataField="NQuantidade" HeaderText="Qtd" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Center" />
                                                                </Columns>
                                                            </asp:GridView>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>

                                                <%-- EMPTY STATE GRID 2 --%>
                                                <EmptyDataTemplate>
                                                    <div class="empty-data-placeholder">
                                                        <i class="fa fa-cubes"></i>
                                                        <h4>Nenhum Volume Salvo</h4>
                                                        <p>Crie volumes utilizando o formulário acima para que eles apareçam aqui.</p>
                                                    </div>
                                                </EmptyDataTemplate>

                                            </asp:GridView>
                                        </div>
                                    </div>

                                    <div id="Div_FormEnvios" runat="server" class="panel panel-success mt-4" visible="false">
                                        <div class="panel-heading" style="background-color: #dff0d8; border-color: #d6e9c6; color: #3c763d;">
                                            <b><i class="fa fa-truck"></i>Dados de Expedição (Peso e Dimensões Totais)</b>
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">
                                                <div class="col-lg-2">
                                                    <label>Data Envio</label>
                                                    <asp:TextBox ID="txtDtEnvio" runat="server" CssClass="form-control" type="date"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-2">
                                                    <label>Peso Líq. (Kg)</label>
                                                    <asp:TextBox ID="txtNPesoLiquido" runat="server" CssClass="form-control mask-money"></asp:TextBox>
                                                </div>
                                                <div class="col-lg-2">
                                                    <label>Peso Bruto (Kg)</label>
                                                    <asp:TextBox ID="txtNPesoBruto" runat="server" CssClass="form-control mask-money"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-2">
                                                    <label>Largura (cm)</label>
                                                    <asp:TextBox ID="txtNLargura" runat="server" CssClass="form-control" onchange="calcularPesoTotal()"></asp:TextBox>
                                                </div>
                                                <div class="col-lg-2">
                                                    <label>Altura (cm)</label>
                                                    <asp:TextBox ID="txtNAltura" runat="server" CssClass="form-control" onchange="calcularPesoTotal()"></asp:TextBox>
                                                </div>
                                                <div class="col-lg-2">
                                                    <label>Comprimento (cm)</label>
                                                    <asp:TextBox ID="txtNComprimento" runat="server" CssClass="form-control" onchange="calcularPesoTotal()"></asp:TextBox>
                                                </div>
                                                <div class="col-lg-2">
                                                    <label>Cúbica Total</label>
                                                    <div class="well well-sm" style="margin-bottom: 0px; background-color: #e8f5e9;">
                                                        <asp:Label ID="lblVolumeCubico" runat="server" Text="0,00 m³" Font-Bold="true" Font-Size="14px" ForeColor="#1b5e20" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="text-right mt-3">
                                                <asp:Button ID="cmdEfetuarEnvio" runat="server" Text="🚀 FINALIZAR ENVIO" CssClass="btn btn-lg btn-success" OnClick="btnEfetuarEnvio_Click" />
                                            </div>
                                        </div>
                                    </div>
                                    <br />
                                    <div id="divBotoes" runat="server">
                                    </div>
                                </asp:View>

                                <%-- VIEW 2: MANTIDA IGUAL --%>
                                <asp:View ID="ViewArquivos2" runat="server">
                                    <div class="text-center">
                                        <h4><i class="fa fa-image"></i>Evidências do Volume</h4>
                                        <div class="embed-responsive embed-responsive-16by9 border">
                                            <embed type="text/html" runat="server" id="frmArquivos2" width="100%" height="500px" />
                                        </div>
                                        <br />
                                        <asp:Button ID="cmdVoltarModal" runat="server" Text="Voltar para Volume" OnClick="VoltarModal_Click" CssClass="btn btn-warning" />
                                    </div>
                                </asp:View>

                            </asp:MultiView>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <div class="modal fade" id="modalGerenciarEtiquetas" tabindex="-1" role="dialog" aria-labelledby="lblModalEtiquetas" aria-hidden="true" data-backdrop="static">
        <div class="modal-dialog modal-xl" role="document">
            <asp:UpdatePanel ID="updModalEtiquetas" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <h3 class="modal-title" id="lblModalEtiquetas">Gerenciador de Etiquetas</h3>
                        </div>
                        <div class="modal-body">

                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaEtiquetaNova" />

                            <div id="alertJs" class="alert alert-danger" style="display: none; margin-bottom: 10px;">
                                <strong>Atenção:</strong> <span id="msgJsContent"></span>
                            </div>

                            <div class="row">

                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <label>Impressora</label>
                                        <asp:DropDownList ID="ddlImpressora" AutoPostBack="false" class="form-control yes_no select Caixa_Selecao" runat="server"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <asp:Button ID="cmdZerar" CssClass="btn btn-primary" Text="Limpar Tudo" runat="server"
                                            OnClientClick="LimparTudo(); return false;"
                                            Style="margin-top: 25px;" />
                                    </div>
                                </div>
                            </div>


                            <div class="table-responsive" style="max-height: 500px; overflow-y: auto;">
                                <asp:GridView ID="dtgItensEtiquetas" runat="server"
                                    class="table table-striped table-bordered table-hover table-condensed"
                                    Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False" DataKeyNames="idItem"
                                    OnRowDataBound="dtgItensEtiquetas_RowDataBound">
                                    <Columns>

                                        <asp:BoundField DataField="sCodigo" HeaderText="Código" ItemStyle-Width="10%" />

                                        <asp:TemplateField HeaderText="Produto">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDscProduto" runat="server" Text='<%# Eval("sDscProduto") %>'></asp:Label>
                                                <asp:HiddenField ID="hddSDscEtiqueta" runat="server" Value='<%# Eval("sDscProduto") %>' />
                                            </ItemTemplate>
                                            <ItemStyle Width="30%" />
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="sUnidade" HeaderText="UN" ItemStyle-Width="5%" />

                                        <asp:TemplateField HeaderText="Qtd">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSaldo" runat="server" Text='<%# Eval("nEnviar") %>' CssClass="js-saldo label label-info" Font-Size="Small"></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Width="10%" />
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Qtd Etiquetas">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtQtdEtiquetas" runat="server"
                                                    CssClass="form-control js-qtd"
                                                    TextMode="Number" min="0" placeholder="0"
                                                    onkeyup="RecalcularEtiquetas(this, 'QTD');" onchange="RecalcularEtiquetas(this, 'QTD');">
                                                </asp:TextBox>
                                            </ItemTemplate>
                                            <HeaderStyle Width="10%" />
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Unid. por Etiqueta">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtnQuantidadeEnvio" runat="server"
                                                    CssClass="form-control mask-money js-unid"
                                                    placeholder="0,0000"
                                                    onkeyup="RecalcularEtiquetas(this, 'UNID');" onchange="RecalcularEtiquetas(this, 'UNID');">
                                                </asp:TextBox>
                                            </ItemTemplate>
                                            <HeaderStyle Width="10%" />
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Observação">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtObservacao" placeholder="Opcional" CssClass="form-control" runat="server" />
                                            </ItemTemplate>
                                            <HeaderStyle Width="20%" />
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>
                            </div>

                        </div>

                        <div class="modal-footer">
                            <asp:Button ID="Button1" OnClick="IncluirEtiquetas_Click" CssClass="btn btn-primary" Text="Imprimir" runat="server" />
                            <button type="button" class="btn btn-warning" data-dismiss="modal">Fechar</button>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <div class="modal fade" id="modalUnitizado" tabindex="-1" role="dialog" aria-labelledby="lblModalUnitizado" aria-hidden="true" data-backdrop="static">
        <div class="modal-dialog modal-xxl" role="document">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <h3 class="modal-title" id="lblModalUnitizado">Gerenciar Unitizados</h3>
                        </div>
                        <div class="modal-body">
                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaUnitizados" />
                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaUnItens" />

                            <div class="mini-menu mb-4">
                                <ul class="nav nav-pills">
                                    <li class="nav-item">
                                        <asp:LinkButton ID="btnAbaListaUnitizado" runat="server" CssClass="nav-link activeMn" OnClick="AnteriorUnitizado_Click">
                                        <i class="fa fa-list"></i>&nbsp Lista de Unitizados
                                        </asp:LinkButton>
                                    </li>
                                    <li class="nav-item" style="margin-left: 10px;">
                                        <asp:LinkButton ID="btnAbaNovoUnitizado" runat="server" CssClass="nav-link" OnClick="AbrirNovoUnitizado_Click">
                                        <i class="fa fa-plus"></i>&nbsp Novo / Editar
                                        </asp:LinkButton>
                                    </li>
                                </ul>
                            </div>
                            <br />

                            <asp:MultiView ID="MultiViewUnitizados" runat="server" ActiveViewIndex="1">

                                <%-- VIEW 0: LISTA DE UNITIZADOS JÁ SALVOS --%>
                                <asp:View ID="ViewListaUnitizados" runat="server">
                                    <div class="panel panel-default">
                                        <div class="panel-heading bg-tt" style="color: white;">
                                            <b><i class="fa fa-cubes"></i>Unitizados Gerados</b>
                                        </div>
                                        <div class="panel-body">
                                            <div id="div_UnitizadoGV" runat="server" class="table-responsive">
                                                <asp:GridView ID="dtgUnitizado" runat="server"
                                                    class="table table-striped table-bordered table-hover table-condensed"
                                                    Width="100%" GridLines="None"
                                                    HeaderStyle-CssClass="gvHeader" CssClass="gvRow" AlternatingRowStyle-CssClass="gvAltRow"
                                                    AutoGenerateColumns="False" OnRowDataBound="dtgUnitizado_RowDataBound"
                                                    ShowFooter="False" DataKeyNames="idUnitizado">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-Width="30px">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="btnToggle" runat="server" CssClass="toggle-icon" data-div-id='<%# "tr" + Eval("idUnitizado") %>'>
                                                                <i style="cursor: pointer;"></i>
                                                                </asp:LinkButton>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="sDscUnitizado" HeaderText="Descrição" />
                                                        <asp:BoundField DataField="idOPI" HeaderText="Nº OPI" />
                                                        <asp:BoundField DataField="sEmbalagem" HeaderText="Embalagem" />
                                                        <asp:BoundField DataField="sCodigoBarras" HeaderText="Código de Barras" />
                                                        <asp:BoundField DataField="sEnviado" HeaderText="Enviado?" />
                                                        <asp:TemplateField HeaderText="Ações" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="cmdAbrirArquivos" runat="server" OnClick="AbrirArquivos_Click" CommandArgument='<%# Eval("idUnitizado") %>' CssClass="btn btn-default btn-xs" ToolTip="Ver Fotos">
                                                                <i class="fa fa-camera"></i>
                                                                </asp:LinkButton>
                                                                <asp:LinkButton ID="cmdEditarUnitizado" runat="server" OnClick="EditarUnitizado_Click" CommandArgument='<%# Eval("idUnitizado") %>' CssClass="btn btn-default btn-xs" ToolTip="Editar">
                                                                <i class="fa fa-pencil"></i>
                                                                </asp:LinkButton>

                                                                <%# NovaLinha(Eval("idUnitizado"),"") %>

                                                                <asp:GridView ID="dtgUnitizadosItens" runat="server" Width="100%"
                                                                    GridLines="None" AutoGenerateColumns="false" DataKeyNames="idUnitizadoItem"
                                                                    HeaderStyle-CssClass="gvChildHeader" CssClass="gvRow" Style="padding: 0; margin: 0; background-color: white;"
                                                                    AlternatingRowStyle-CssClass="gvAltRow">
                                                                    <Columns>
                                                                        <asp:BoundField DataField="SCodigo" HeaderText="Cód." />
                                                                        <asp:BoundField DataField="SdscProduto" HeaderText="Produto" />
                                                                        <asp:BoundField DataField="nQuantidade" HeaderText="Qtd" />
                                                                        <asp:BoundField DataField="sCodigoBarras" HeaderText="Barras" />
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <EmptyDataTemplate>
                                                        <div class="empty-data-placeholder">
                                                            <i class="fa fa-arrow-left"></i>
                                                            <h4>Lista Vazia</h4>
                                                            <p>Nenhum Unitizado criado para esta OPI.</p>
                                                        </div>
                                                    </EmptyDataTemplate>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </asp:View>

                                <%-- VIEW 1: CADASTRO (FORMULÁRIO + BIPAGEM + GRID DE ITENS) --%>
                                <asp:View ID="ViewCadastroUnitizado" runat="server">
                                    <div class="row">
                                        <%-- COLUNA DA ESQUERDA: DADOS E BIPAGEM --%>
                                        <div class="col-lg-4 border-right">
                                            <div class="panel panel-default">
                                                <div class="panel-heading"><b>1. Dados do Unitizado</b></div>
                                                <div class="panel-body">
                                                    <div class="form-group">
                                                        <label>Descrição (Ex: Pallet 01)</label>
                                                        <asp:TextBox ID="txtsDscUnitizado" class="form-control" runat="server" placeholder="Descrição"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group">
                                                        <label>Tipo de Embalagem</label>
                                                        <asp:DropDownList ID="ddlEmbalagemUnitizado" runat="server" class="form-control"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="panel panel-default">
                                                <div class="panel-heading"><b>2. Adicionar Itens (Bipagem)</b></div>
                                                <div class="panel-body">

                                                    <div class="form-group">
                                                        <asp:Button ID="cmdAbrirCâmera" OnClick="AbrirCamera_Click" CssClass="btn btn-sm btn-primary btn-block" Text="Abrir Leitor" runat="server" />
                                                    </div>

                                                    <div id="DivBipador" runat="server" class="mb-3 text-center" visible="false">
                                                        <uc1:LeitorQuagga runat="server" ID="LeitorQuagga1" STemParametros="S" />
                                                    </div>

                                                    <label>Código de Barras</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon"><i class="fa fa-barcode"></i></span>
                                                        <asp:TextBox ID="txtsCodigoBarras" CssClass="form-control" runat="server" placeholder="Bipe aqui..."></asp:TextBox>
                                                    </div>
                                                    <br />
                                                    <div class="row">
                                                        <div class="col-md-6" style="padding-right: 2px;">
                                                            <asp:Button ID="cmdIncluirUnitizadoItem" OnClick="IncluirUnitizadosItens_Click" CssClass="btn btn-success btn-block" Text="Adicionar Produto" runat="server" />
                                                        </div>
                                                        <div class="col-md-6" style="padding-left: 2px;">
                                                            <asp:Button ID="cmdImportarPendentesUnitizado" OnClick="ImportarPendentesUnitizado_Click" CssClass="btn btn-info btn-block" Text="📥 Importar Pendentes" runat="server" ToolTip="Importa produtos da OPI que não estão em envios, volumes ou unitizados." />
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>
                                        </div>

                                        <%-- COLUNA DA DIREITA: GRID DE ITENS --%>
                                        <div class="col-lg-8">
                                            <div class="panel panel-default">
                                                <div class="panel-heading bg-tt" style="color: white;">
                                                    <b><i class="fa fa-list"></i>Itens neste Unitizado</b>
                                                </div>
                                                <div class="panel-body">
                                                    <div class="table-responsive" style="min-height: 300px; max-height: 450px; overflow-y: auto;">
                                                        <asp:GridView ID="dtgUNItens" runat="server"
                                                            CssClass="table table-striped table-bordered table-hover table-condensed"
                                                            Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            ShowFooter="False" DataKeyNames="idItem"
                                                            OnRowCommand="dtgUNItens_RowCommand">
                                                            <Columns>
                                                                <asp:BoundField DataField="SCodigo" HeaderText="Cód." ItemStyle-Width="10%" />
                                                                <asp:BoundField DataField="SdscProduto" HeaderText="Produto" />
                                                                <asp:BoundField DataField="nQuantidade" HeaderText="Qtd" ItemStyle-Width="10%" />
                                                                <asp:BoundField DataField="sCodigoBarras" HeaderText="Barras" ItemStyle-Width="20%" />
                                                                <%-- Botão Excluir --%>
                                                                <asp:TemplateField HeaderText="Ação" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="btnExcluirItemUnit" runat="server"
                                                                            CommandName="Excluir"
                                                                            CommandArgument='<%# Container.DataItemIndex %>'
                                                                            CssClass="btn btn-danger btn-xs"
                                                                            ToolTip="Remover Item">
                                                                            <i class="fa fa-trash-o"></i>
                                                                        </asp:LinkButton>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                <div class="empty-data-placeholder">
                                                                    <i class="fa fa-arrow-left"></i>
                                                                    <h4>Lista Vazia</h4>
                                                                    <p>Bipe os produtos ao lado para montar o unitizado.</p>
                                                                </div>
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                                    </div>
                                                    <div class="panel-footer text-right">
                                                        <asp:Button ID="cmdSalvarUnitizado" runat="server" Text="💾 Salvar Unitizado" CssClass="btn btn-lg btn-success" OnClick="SalvarUnitizado_Click" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </asp:View>

                                <%-- VIEW 2: ARQUIVOS (Mantida) --%>
                                <asp:View ID="viewArquivos" runat="server">
                                    <div class="text-center">
                                        <h4><i class="fa fa-image"></i>Evidências do Unitizado</h4>
                                        <div class="col-lg-12" runat="server" id="div_arquivo">
                                            <div class="embed-responsive embed-responsive-16by9 border">
                                                <embed type="text/html" runat="server" id="frmArquivos" width="100%" height="500px" />
                                            </div>
                                        </div>
                                        <br />
                                        <asp:Button ID="cmdVoltarDeArquivos" runat="server" Text="Voltar para Lista" OnClick="AnteriorUnitizado_Click" CssClass="btn btn-warning" />
                                    </div>
                                </asp:View>

                            </asp:MultiView>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>
                        </div>
                    </div>


                    <asp:HiddenField ID="hddidUnitizado" runat="server" />
                    <asp:HiddenField ID="hddidUnitizadoUpdt" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <%--  Script Etiquetas--%>
    <script type="text/javascript">

        // Garante inicialização após PostBack do UpdatePanel
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            IniciarLogicaEtiquetas();
        });

        $(document).ready(function () {
            IniciarLogicaEtiquetas();
        });

        function IniciarLogicaEtiquetas() {
            if (typeof aplicarMascaras === "function") { aplicarMascaras(); }

            // Percorre a grid para aplicar a SUGESTÃO PADRÃO INICIAL
            $('#<%= dtgItensEtiquetas.ClientID %> tr').each(function () {
                var $row = $(this);
                if ($row.attr('data-tipo')) {
                    var $txtQtd = $row.find('.js-qtd');
                    var $txtUnid = $row.find('.js-unid');

                    // Só sugere se estiver vazio
                    if ($txtQtd.val() === "" || $txtQtd.val() === "0") {
                        AplicarSugestaoPadrao($row, $txtQtd, $txtUnid);
                    }
                }
            });
        }

        // LISTA DE UNIDADES QUE ACEITAM DECIMAL
        function IsUnidadeFracionada(unidade) {
            if (!unidade) return false;
            var listaFracionada = [
                'KG', 'LB', 'LT', 'MT', 'M2', 'M3', 'MM', 'CM', 'KM', 'GA', 'L', 'VL', 'B3', 'B6', 'RL', 'TB'
            ];
            return listaFracionada.indexOf(unidade.toUpperCase().trim()) >= 0;
        }

        function AplicarSugestaoPadrao($row, $txtQtd, $txtUnid) {
            var tipoEtiqueta = $row.attr('data-tipo');
            var unidade = $row.attr('data-unidade');

            var saldoStr = $row.attr('data-saldo').replace(/\./g, '').replace(',', '.');
            var saldo = parseFloat(saldoStr) || 0;

            // Lógica Inicial (Apenas sugestão ao abrir)
            if (tipoEtiqueta === 'A' || IsUnidadeFracionada(unidade)) {
                // Agrupado/Peso: 1 etiqueta com o saldo total
                $txtQtd.val(1);
                $txtUnid.val(saldo.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 4 }));
            }
            else {
                // Unitário: N etiquetas de 1
                $txtQtd.val(Math.floor(saldo));
                $txtUnid.val('1,0000');
            }
        }

        function RecalcularEtiquetas(inputChanged, origem) {
            var $row = $(inputChanged).closest("tr");

            // Limpa avisos visuais
            var $divAlert = $("#alertJs");
            var $msgJs = $("#msgJsContent");
            $divAlert.hide().removeClass('alert-danger alert-warning');
            $row.find("input").css("border", "");

            // --- 1. LEITURA DOS DADOS (SEM ALTERAR NADA) ---
            var unidade = $row.attr('data-unidade');
            var aceitaDecimal = IsUnidadeFracionada(unidade);

            var saldoStr = $row.attr('data-saldo').replace(/\./g, '').replace(',', '.');
            var saldo = parseFloat(saldoStr) || 0;

            var $txtQtd = $row.find('.js-qtd');
            var $txtUnid = $row.find('.js-unid');

            var qtdVal = parseInt($txtQtd.val()) || 0;
            var unidValStr = $txtUnid.val().replace(/\./g, '').replace(',', '.');
            var unidVal = parseFloat(unidValStr) || 0;

            // *** REMOVIDA LÓGICA DE CÁLCULO AUTOMÁTICO AQUI *** // O usuário digita o que quiser.

            // --- 2. VALIDAÇÕES ---

            // Validação A: Unidade Inteira com Valor Quebrado
            if (!aceitaDecimal) {
                // Verifica se é inteiro (com tolerância minima para erros de ponto flutuante)
                var isInteger = Math.abs(unidVal - Math.round(unidVal)) < 0.00001;

                if (!isInteger && unidVal > 0) {
                    ExibirAviso("A unidade '" + unidade + "' aceita apenas números inteiros.", $row, "erro");
                    return;
                }
            }

            // Validação B: Total vs Saldo (Apenas informativo)
            var totalSendoGerado = qtdVal * unidVal;

            // Se ambos estiverem preenchidos, valida o saldo
            if (qtdVal > 0 && unidVal > 0) {
                var diferenca = Math.abs(totalSendoGerado - saldo);

                if (diferenca > 0.001) {
                    var msg = "";
                    var tipoAviso = "alerta"; // Amarelo por padrão

                    if (totalSendoGerado > saldo) {
                        msg = "CUIDADO: O total (" + totalSendoGerado.toLocaleString('pt-BR') + ") é MAIOR que o saldo (" + saldo.toLocaleString('pt-BR') + ").";
                        tipoAviso = "erro"; // Vermelho
                    } else {
                        msg = "Atenção: Geração Parcial. Total (" + totalSendoGerado.toLocaleString('pt-BR') + ") é menor que o saldo (" + saldo.toLocaleString('pt-BR') + ").";
                    }
                    // Chama a função de exibir aviso
                    ExibirAviso(msg, $row, tipoAviso);
                }
            }
        }

        function ExibirAviso(msg, $row, tipo) {
            var $divAlert = $("#alertJs");
            var $msgJs = $("#msgJsContent");

            $msgJs.html(msg);
            $divAlert.show();

            if (tipo === "erro") {
                $divAlert.removeClass('alert-warning').addClass('alert-danger');
                $row.find(".js-qtd, .js-unid").css("border", "2px solid red");
            } else {
                $divAlert.removeClass('alert-danger').addClass('alert-warning');
                $row.find(".js-qtd, .js-unid").css("border", "2px solid #f0ad4e"); // Amarelo
            }
        }

        function ExibirAvisoValor(msg, $row, tipo) {
            var $divAlert = $("#alertJs");
            var $msgJs = $("#msgJsContent");

            // Alterado de .text() para .html() para renderizar o negrito
            $msgJs.html(msg);
            $divAlert.show();

            if (tipo === "erro") {
                $divAlert.removeClass('alert-warning').addClass('alert-danger');

                // Só tenta pintar a borda se a linha ($row) foi passada
                if ($row) {
                    $row.find(".js-qtd, .js-unid").css("border", "2px solid red");
                }
            } else {
                $divAlert.removeClass('alert-danger').addClass('alert-warning');

                // Só tenta pintar a borda se a linha ($row) foi passada
                if ($row) {
                    $row.find(".js-qtd, .js-unid").css("border", "2px solid #f0ad4e");
                }
            }
        }

        function LimparTudo() {
            var $grid = $('#<%= dtgItensEtiquetas.ClientID %>');
            var $divAlert = $("#alertJs");

            $grid.find('.js-qtd').val('');
            $grid.find('.js-unid').val('');
            $grid.find('input').css('border', '');
            $divAlert.hide();

            return false;
        }
    </script>

    <%--script Para Colapso e Mascara--%>
    <script type="text/javascript">
        function pageLoad() {
            aplicarMascaras();
        }

        function aplicarMascaras() {
            // AQUI ESTÁ A MÁGICA PARA A GRIDVIEW
            // Como você definiu CssClass="form-control mask-money" no TextBox da Grid
            $('.mask-money').mask('000.000.000.000.000,0000', { reverse: true });
        }

        function RegistrarColapsoScript() {
            $(document).ready(function () {
                // Define o ícone inicial
                $('.toggle-icon').addClass('fa fa-plus');

                // Função para alternar ícones e mostrar/ocultar div
                $('.toggle-icon').click(function () {
                    var icon = $(this);
                    var divId = $(this).data('div-id');
                    var current = $('#' + divId).css('display');
                    if (current == 'none') {
                        $('#' + divId).show('slow');
                        icon.removeClass('fa fa-plus').addClass('fa fa-minus');
                    } else {
                        $('#' + divId).hide('slow');
                        icon.removeClass('fa fa-minus').addClass('fa fa-plus');
                    }
                    return false; // Evita o postback
                });
            });
        }

        // Adiciona o evento para garantir que o script seja executado após a atualização do UpdatePanel
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            RegistrarColapsoScript();
        });
    </script>

    <%--script de render produtos--%>
    <script>
        /*Agnes Partal * 10/07/2024--------------------------------------------*/
        var cardTimer = {};

        function mostraCard(element, idProduto, tabela) {

            cardTimer[idProduto + '_' + tabela] = setTimeout(function () {
                console.log(idProduto);
                $.ajax({
                    url: "/API/Pagina_Ajax.aspx/GetProdutoDetalhes",
                    data: JSON.stringify({ idProduto: idProduto }),
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    success: function (response) {
                        var produto = JSON.parse(response.d);
                        var cardProduto = `
                    <div class="card">
                        <div class="card-body d-flex">
                            <div class="flex-shrink-0" style="min-inline-size: fit-content;">
                                ${produto.imagem ? `<img src="${produto.imagem}" alt="Imagem do Produto" class="img-fluid img-thumbnail" style="width: 100px; height: auto;" />` : ''}
                            </div>
                            <div class="flex-grow-1 d-flex flex-column ms-3" style="min-width: 30%">
                                <div class="d-flex">                                          
                                    <div class="card-text me-3"> ${produto.sCategoriaVendas ? `<strong>Categoria Vendas: </strong>${produto.sCategoriaVendas}` : ''}</div>
                                    <div class="card-text me-3"> ${produto.sTipo ? `<strong>Tipo: </strong>${produto.sTipo}` : ''}</div>
                                    <div class="card-text me-3"> ${produto.sFabricante ? `<strong>Fabricante: </strong>${produto.sFabricante}` : ''}</div>
                                    <div class="card-text me-3"> ${produto.sGrupo ? `<strong>Grupo: </strong>${produto.sGrupo}` : ''}</div>
                                    <div class="card-text me-3"> ${produto.sFamilia ? `<strong>Família: </strong>${produto.sFamilia}` : ''}</div>
                                </div>
                            </div>
                            <div class="flex-grow-1 d-flex flex-column ms-3" style="max-width: 70%">
                                <div class="d-flex">
                                    <div class="card-text"> ${produto.sCodigoCEST ? `<strong>CEST: </strong>${produto.sCodigoCEST}` : ''}</div>
                                    <div class="card-text me-3"> ${produto.sCodigoNCM ? `<strong>NCM: </strong>${produto.sCodigoNCM}` : ''}</div>
                                    <div class="card-text me-3"> ${produto.sPaisOrigem ? `<strong>Origem: </strong>${produto.sPaisOrigem}` : ''}</div>                                                                           
                                    <div class="card-text me-3"> ${produto.sLocalArmazenamento ? `<strong>Local Armazenamento: </strong>${produto.sLocalArmazenamento}` : ''}</div>
                                </div>
                            </div>
                        </div>
                    </div>
                `;

                        var cardId = idProduto + '_' + tabela;
                        var card = document.getElementById(cardId);
                        card.innerHTML = cardProduto;

                        //Posição card                    
                        var rect = element.getBoundingClientRect();
                        var scrollTop = document.documentElement.scrollTop || document.body.scrollTop;
                        var scrollLeft = document.documentElement.scrollLeft || document.body.scrollLeft;

                        hideAllCards();

                        card.style.top = (rect.top + scrollTop - 10) + 'px';
                        card.style.left = (rect.right + scrollLeft + element.offsetWidth + 10) + 'px';
                        card.style.display = 'block';
                    },
                    error: function (error) {
                        console.error("Erro ao obter os detalhes do produto:", error);
                    }
                });
            }, 300);
        }

        function escondeCard(idProduto, tabela) {
            var cardId = idProduto + '_' + tabela;
            var card = document.getElementById(cardId);

            clearTimeout(cardTimer[idProduto + '_' + tabela]);

            card.style.display = 'none';

        }

        function hideAllCards() {
            var cards = document.querySelectorAll('.product-card');
            cards.forEach(function (card) {
                card.style.display = 'none';
            });
        }

        function openModal(idProduto) {
            $.ajax({
                url: "/API/Pagina_Ajax.aspx/GetProdutoDetalhes",
                data: JSON.stringify({ idProduto: idProduto }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (response) {
                    var produto = JSON.parse(response.d);

                    var tituloProduto = `
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                        <h5 class="modal-title" id="detailsModalLabel">${produto.sCodigo} - ${produto.sDsc}</h5>
                    `;

                    var modalHeader = document.getElementById('modalHeader');
                    modalHeader.innerHTML = tituloProduto;

                    var imagem = '';

                    if (produto.imagem) {
                        imagem += `
                                <div style="text-align: center; margin-bottom: 20px;">
                                    <img src="${produto.imagem}" alt="Imagem do Produto" class="img-fluid" style="width: 300px; height: auto;"/>
                                </div>
                        `;
                    }

                    var tabelaProduto = '<table class="table table-bordered">';

                    if (produto.sCategoriaVendas) {
                        tabelaProduto += `
                            <tr>
                                <th>Categoria Vendas</th>
                                <td>${produto.sCategoriaVendas}</td>
                            </tr>
                        `;
                    }

                    if (produto.sTipo) {
                        tabelaProduto += `
                            <tr>
                                <th>Tipo</th>
                                <td>${produto.sTipo}</td>
                            </tr>
                        `;
                    }

                    if (produto.sGrupo) {
                        tabelaProduto += `
                            <tr>
                                <th>Grupo</th>
                                <td>${produto.sGrupo}</td>
                            </tr>
                        `;
                    }

                    if (produto.sFabricante) {
                        tabelaProduto += `
                            <tr>
                                <th>Fabricante</th>
                                <td>${produto.sFabricante}</td>
                            </tr>
                        `;
                    }

                    if (produto.sLocalArmazenamento) {
                        tabelaProduto += `
                            <tr>
                                <th>Local Armazenamento</th>
                                <td>${produto.sLocalArmazenamento}</td>
                            </tr>
                        `;
                    }

                    if (produto.sFamilia) {
                        tabelaProduto += `
                            <tr>
                                <th>Família</th>
                                <td>${produto.sFamilia}</td>
                            </tr>
                        `;
                    }

                    if (produto.sCodigoCEST) {
                        tabelaProduto += `
                            <tr>
                                <th>CEST</th>
                                <td>${produto.sCodigoCEST}</td>
                            </tr>
                        `;
                    }

                    if (produto.sCodigoNCM) {
                        tabelaProduto += `
                            <tr>
                                <th>NCM</th>
                                <td>${produto.sCodigoNCM}</td>
                            </tr>
                        `;
                    }

                    if (produto.sPaisOrigem) {
                        tabelaProduto += `
                            <tr>
                                <th>Origem</th>
                                <td>${produto.sPaisOrigem}</td>
                            </tr>
                        `;
                    }

                    tabelaProduto += `</table>`;

                    var modalBody = document.getElementById('modalBody');
                    modalBody.innerHTML = imagem + tabelaProduto;

                    $('#produtoDetalheModal').modal('show');
                },
                error: function (error) {
                    console.error("Erro ao obter os detalhes do produto:", error);
                }
            });
        }

        function openProductDetail(idItem) {
            // Construa a URL
            var url = '/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id=' + idItem;
            // Abre a URL em uma nova aba
            window.open(url, '_blank');
            // Impede que o LinkButton execute o postback
            return false;
        }

        /*-----------------------------------------------------------------*/

    </script>


    <uc1:MensagemPagina runat="server" ID="MensagemPaginaExclusao" />
    <asp:HiddenField ID="hddidOPI" runat="server" />
    <asp:HiddenField ID="hddidEnvioOPI" runat="server" />
    <asp:HiddenField ID="hddidProduto" runat="server" />
    <asp:HiddenField ID="hddidResponsavel" runat="server" />
    <asp:HiddenField ID="hddidVolume" runat="server" />
    <asp:HiddenField ID="hddidMovimentacao" runat="server" />
    <asp:HiddenField ID="hfScrollPosition" runat="server" />

    <asp:HiddenField ID="hddsCodigoBarras" runat="server" />
    <asp:HiddenField ID="hddidProdutoSerie" runat="server" />
    <asp:HiddenField ID="hddnQtdSerie" runat="server" />
    <asp:HiddenField ID="hddsProdutoSerie" runat="server" />
    <asp:HiddenField ID="hddidEtiqueta" runat="server" />
</asp:Content>
