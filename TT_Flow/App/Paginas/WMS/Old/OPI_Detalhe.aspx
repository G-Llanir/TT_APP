<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="OPI_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.OPI_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/LeitorQuagga.ascx" TagPrefix="uc1" TagName="LeitorQuagga" %>




<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmOPI_Detalhe_Old" ContentPlaceHolderID="cphCorpo" runat="server">
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
                <div class="modal-dialog modal-xl" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <h1 class="modal-title" id="lblModalEnvio">Novo Envio -
                        <asp:Label ID="lbltituloModal" Text="Etiqueta" runat="server" />
                            </h1>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <div class="col-lg-12">
                                    <div class="mini-menu col-lg-12">
                                        <ul>
                                            <li>
                                                <asp:LinkButton ID="cmdEtiquetas" runat="server" OnClick="AbrirEtiquetas_Click"><b>Etiquetas</b><span class="arrow">&rsaquo;</span></asp:LinkButton></li>
                                            <li>
                                                <asp:LinkButton ID="cmdUnitizados" runat="server" OnClick="AbrirUnitizados_Click"><b>Unitizados</b> <span class="arrow">&rsaquo;</span></asp:LinkButton>
                                            </li>
                                            <li>
                                                <asp:LinkButton ID="cmdVolumes" runat="server" OnClick="AbrirVolumes_Click"><b>Volumes</b> <span class="arrow">&rsaquo;</span></asp:LinkButton></li>
                                        </ul>
                                    </div>
                                </div>
                                <br />
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaEnvios" />
                                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaVolume" />
                                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaUnitizados" />
                                </div>
                                <div id="divMultiView" runat="server" class="col-lg-12">
                                    <div class="border p-4">
                                        <div id="divFormEnvios" class="col-lg-12" runat="server">
                                            <asp:MultiView ID="MultiViewFormularios" runat="server" ActiveViewIndex="0">

                                                <%--View Gerenciador de Etiquetas--%>
                                                <asp:View ID="ViewGerenciador" runat="server">
                                                    <br />
                                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                        <ContentTemplate>

                                                            <div class="form-stacked row">
                                                                <div id="div_allView" class="col-lg-12" runat="server" style="max-height: 750px; overflow-x: auto;">
                                                                    <div class="col-lg-12">
                                                                        <uc1:MensagemPagina runat="server" ID="MensagemPaginaEtiqueta" />

                                                                        <div class="row mb-3" id="div_formEtiqueta" runat="server">
                                                                            <div class="col-lg-4">
                                                                                <div class="form-group">
                                                                                    <label>Impressora</label>
                                                                                    <asp:DropDownList ID="ddlImpressora" AutoPostBack="false" class="form-control yes_no select Caixa_Selecao" runat="server"></asp:DropDownList>
                                                                                </div>
                                                                            </div>
                                                                        </div>

                                                                    </div>
                                                                    <div id="div_ItensEtiquetas" runat="server" class="col-lg-12">
                                                                        <div id="div_scroll" runat="server" class="table table-responsive" style="max-height: 400px; overflow-y: auto;">
                                                                            <asp:GridView ID="dtgItensEtiquetas" runat="server" class="table table-striped table-bordered table-hover table-condensed"
                                                                                Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                                ShowFooter="False" DataKeyNames="idItem" OnRowDataBound="dtgItensEtiquetas_RowDataBound">
                                                                                <%--AllowPaging="true" PageSize="10" OnPageIndexChanging="dtgItensEtiquetas_PageIndexChanging"--%>
                                                                                <Columns>
                                                                                    <asp:TemplateField HeaderText="+">
                                                                                        <ItemTemplate>
                                                                                            <asp:CheckBox ID="chkOpcaoItemEtiqueta" runat="server" OnCheckedChanged="chkOpcaoItemEtiqueta_CheckedChanged" AutoPostBack="true" />
                                                                                        </ItemTemplate>
                                                                                        <HeaderStyle Width="4%" />
                                                                                    </asp:TemplateField>
                                                                                    <asp:BoundField DataField="sCodigo" HeaderText="Código" ItemStyle-Width="1%" />
                                                                                    <asp:BoundField DataField="sDscProduto" HeaderText="Produto">
                                                                                        <ItemStyle Width="2%" />
                                                                                    </asp:BoundField>
                                                                                    <asp:BoundField DataField="sUnidade" HeaderText="UN" />
                                                                                    <asp:BoundField DataField="nEnviar" HeaderText="Saldo a Enviar" ItemStyle-CssClass="quantidade-pendente" />
                                                                                    <asp:BoundField DataField="nPesoLiquido" HeaderText="Peso Líquido" DataFormatString="{0:F2}" />
                                                                                    <asp:TemplateField HeaderText="Qtd">
                                                                                        <ItemTemplate>
                                                                                            <asp:TextBox ID="txtsDscEtiqueta" runat="server" Text='<%# Bind("sDscProduto") %>' Visible="false" />
                                                                                            <asp:TextBox ID="txtnQuantidadeEnvio" placeholder="0,0000" CssClass="form-control mask-money" runat="server" Text='<%# Bind("nQuantidade") %>'  Style="width: 80px;" /> <%--TextMode="Number" min="0" step="1"--%>
                                                                                            <asp:TextBox ID="txtnQtdPendente" runat="server" Text='<%# Bind("nEnviar") %>' Visible="false" />
                                                                                        </ItemTemplate>
                                                                                        <HeaderStyle Width="10%" />
                                                                                    </asp:TemplateField>
                                                                                    <asp:TemplateField HeaderText="Observação">
                                                                                        <ItemTemplate>
                                                                                            <asp:TextBox ID="txtObservacao" placeholder="Observação" CssClass="form-control" runat="server" />
                                                                                        </ItemTemplate>
                                                                                        <HeaderStyle Width="15%" />
                                                                                    </asp:TemplateField>
                                                                                    <%--    <asp:TemplateField HeaderText="Lote">
                                                                                        <ItemTemplate>
                                                                                            <asp:TextBox ID="txtsLote" placeholder="Lote" CssClass="form-control" runat="server" />
                                                                                        </ItemTemplate>
                                                                                        <HeaderStyle Width="10%" />
                                                                                    </asp:TemplateField>--%>
                                                                                    <asp:TemplateField HeaderText="Imagem">
                                                                                        <ItemTemplate>
                                                                                            <asp:TextBox ID="txtidItem" runat="server" Text='<%# Eval("idItem") %>' Visible="false" />
                                                                                            <asp:Image ID="imgProduto" runat="server" CssClass="img" Style="max-width: 100%; max-height: 100%; border-top-left-radius: 5px; border-bottom-left-radius: 5px;" />
                                                                                        </ItemTemplate>
                                                                                        <HeaderStyle Width="10%" />
                                                                                    </asp:TemplateField>
                                                                                </Columns>
                                                                            </asp:GridView>
                                                                        </div>
                                                                    </div>

                                                                    <div id="div_Etiquetas" runat="server" class="col-lg-12">
                                                                        <div class="table table-responsive">
                                                                            <%--style="max-height: 400px; overflow-y: auto;"--%>
                                                                            <asp:GridView ID="dtgEtiquetas" runat="server" class="table table-striped table-bordered table-hover table-condensed"
                                                                                Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                                ShowFooter="False" DataKeyNames="idOPI,idProduto" OnRowDataBound="dtgEtiquetas_RowDataBound">
                                                                                <Columns>
                                                                                    <asp:BoundField DataField="SCodigoBarras" HeaderText="Código de Barras" />
                                                                                    <asp:BoundField DataField="SdscEtiqueta" HeaderText="Etiqueta" />
                                                                                    <asp:BoundField DataField="NQuantidade" HeaderText="Quantidade" />
                                                                                    <asp:BoundField DataField="Sobservacao" HeaderText="Observação" />
                                                                                    <%-- <asp:BoundField DataField="SLote" HeaderText="Lote" />--%>
                                                                                    <asp:BoundField DataField="DtImpressao" HeaderText="Data de Impressão" />
                                                                                    <asp:TemplateField HeaderText="Ação">
                                                                                        <ItemTemplate>
                                                                                            <asp:LinkButton ID="cmdExcluirEtiqueta" CssClass="btn btn-sm btn-danger" runat="server" OnClick="ExcluirEtiquetas_Click" CommandArgument='<%# Eval("idEtiqueta") %>'><i class="fa fa-trash-o"></i></asp:LinkButton>
                                                                                            <asp:LinkButton ID="cmdAdicionarSerie" runat="server" CssClass="btn btn-primary btn-sm" Visible="false" CommandArgument='<%# Eval("idEtiqueta") %>' OnClick="cmdAdicionarSerie_Click">
                                                                                              <i class="fa-plus fa"></i> 
                                                                                            </asp:LinkButton>
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

                                                                    <div class="col-lg-12">

                                                                        <asp:Button ID="cmdIncluirEtiqueta" OnClick="IncluirEtiquetas_Click" CssClass="btn btn-sm btn-primary" Text="Incluir" runat="server" />
                                                                        <asp:Button ID="cmdSalvarEtiquetas" OnClick="SalvarEtiquetas_Click" CssClass="btn btn-sm btn-success" Text="Salvar" runat="server" />

                                                                    </div>

                                                                </div>
                                                                <div id="div_seriesEtiquetas" visible="false" runat="server">
                                                                    <div class="panel panel-default" runat="server">
                                                                        <div class="panel-heading bg-tt text-white">
                                                                            <h3 class="panel-title"><b style="color: white">Produto: </b>
                                                                                <asp:Label Style="color: white" ID="lblTituloProduto" Text="" runat="server" /></h3>
                                                                        </div>
                                                                        <div class="panel-body">
                                                                            <div class="row">
                                                                                <div class="col-lg-12">
                                                                                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaSeries" />
                                                                                </div>
                                                                                <div class="col-lg-12">
                                                                                    <div class="col-lg-4">
                                                                                        <div class="form-group">
                                                                                            <label for="txtIdEmbalagem">Armazenamento:</label>
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
                                                                                        <asp:LinkButton ID="cmdVoltarEtiquetas" CssClass="btn btn-warning btn-sm" runat="server" OnClick="VoltarEtiquetas_Click"><b>Voltar</b></asp:LinkButton>
                                                                                    </div>
                                                                                </div>
                                                                                <br />
                                                                                <div id="div_gvSerie" runat="server" class="col-lg-12" style="margin-top: 10px;">
                                                                                    <asp:GridView ID="dtgEtiquetasSerie" runat="server" class="table table-striped table-bordered table-hover table-condensed"
                                                                                        Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                                        ShowFooter="False" DataKeyNames="idOPI, idEtiqueta, idEtiquetaSerie" OnRowDataBound="dtgEtiquetasSeries_RowDataBound">
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

                                                                                                    <asp:HiddenField ID="hddidEtiquetaSerie" runat="server" Value='<%# Eval("idEtiquetaSerie") %>' />
                                                                                                    <asp:HiddenField ID="hddidProdutoSerie" runat="server" Value='<%# Eval("idProduto") %>' />
                                                                                                    <asp:HiddenField ID="hddidEtiqueta" runat="server" Value='<%# Eval("idEtiqueta") %>' />
                                                                                                    <asp:HiddenField ID="hddsProdutoSerie" runat="server" Value='<%# Eval("sDscProduto") %>' />
                                                                                                    <asp:HiddenField ID="hddsCodigo" runat="server" Value='<%# Eval("sCodigo") %>' />
                                                                                                    <asp:HiddenField ID="hddsCodigoBarras" runat="server" Value='<%# Eval("sCodigoBarras") %>' />
                                                                                                    <asp:HiddenField ID="hddsExclusao" runat="server" Value='<%# Eval("sExclusao") %>' />
                                                                                                    <asp:HiddenField ID="hddidOPI" runat="server" Value='<%# Eval("idOPI") %>' />
                                                                                                </ItemTemplate>
                                                                                                <HeaderStyle Width="10%" />
                                                                                            </asp:TemplateField>
                                                                                        </Columns>
                                                                                    </asp:GridView>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </asp:View>

                                                <!-- Formulário de Unitizados e Itens -->
                                                <asp:View ID="ViewUnitzadosItens" runat="server">
                                                    <br />
                                                    <asp:MultiView ID="MultiViewUnitizados" runat="server" ActiveViewIndex="0">
                                                        <asp:View ID="ViewFormUnitizado" runat="server">
                                                            <div class="form-stacked row">
                                                                <div class="row">
                                                                    <div class="col-lg-12">
                                                                        <div class="col-md-4">
                                                                            <div class="form-group">
                                                                                <label for="txtIdEmbalagem">Tipo de Embalagem:</label>
                                                                                <asp:DropDownList ID="ddlEmbalagemUnitizado" runat="server" class="form-control"></asp:DropDownList>
                                                                            </div>
                                                                        </div>
                                                                        <div class="col-lg-6">
                                                                            <div class="form-group">
                                                                                <label for="txtIdEmbalagem">Descrição do Unitizado:</label>
                                                                                <asp:TextBox ID="txtsDscUnitizado" class="form-control " runat="server" placeholder="Descrição Unitizado"></asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <br />
                                                                <div class="row">
                                                                    <div id="div_UnitizadoGV" runat="server" class="col-lg-12">
                                                                        <%--botão vai ser parte da grid--%>
                                                                        <div class="table table-responsive">
                                                                            <asp:GridView ID="dtgUnitizado" runat="server" class="table table-striped table-bordered table-hover table-condensed"
                                                                                Width="100%" GridLines="None"
                                                                                HeaderStyle-CssClass="gvHeader" CssClass="gvRow" AlternatingRowStyle-CssClass="gvAltRow"
                                                                                AutoGenerateColumns="False" OnRowDataBound="dtgUnitizado_RowDataBound"
                                                                                ShowFooter="False" DataKeyNames="idUnitizado">
                                                                                <Columns>
                                                                                    <asp:TemplateField>
                                                                                        <ItemTemplate>
                                                                                            <asp:LinkButton ID="btnToggle" runat="server" CssClass="toggle-icon" data-div-id='<%# "tr" + Eval("idUnitizado") %>'>
                                                                           <i style="cursor: pointer;"></i>
                                                                                            </asp:LinkButton>
                                                                                        </ItemTemplate>
                                                                                    </asp:TemplateField>
                                                                                    <asp:BoundField DataField="sDscUnitizado" HeaderText="Unitizado" />
                                                                                    <asp:BoundField DataField="idOPI" HeaderText="Nº OPI" />
                                                                                    <asp:BoundField DataField="sEmbalagem" HeaderText="Embalagem" />
                                                                                    <asp:TemplateField HeaderText="Ação">
                                                                                        <ItemTemplate>
                                                                                            <asp:LinkButton ID="cmdAbrirArquivos" runat="server" OnClick="AbrirArquivos_Click" CommandArgument='<%# Eval("idUnitizado") %>'>
                                                                                          <i class="fa fa-upload"></i>
                                                                                            </asp:LinkButton>
                                                                                            <asp:LinkButton ID="cmdEditarUnitizado" runat="server" OnClick="EditarUnitizado_Click" CommandArgument='<%# Eval("idUnitizado") %>'>
                                                                                          <i class="fa fa-pencil"></i>
                                                                                            </asp:LinkButton>
                                                                                            <%# NovaLinha(Eval("idUnitizado"),"") %>

                                                                                            <asp:GridView ID="dtgUnitizadosItens" runat="server" Width="100%"
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
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <div class="row">
                                                                    <div class="col-lg-12" style="margin-left: 15px;">
                                                                        <asp:Button ID="cmdProximoUnitizado" runat="server" Text="Adicionar Produtos" OnClick="ProximoUnitizado_Click" CssClass="btn btn-sm btn-success" />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </asp:View>
                                                        <asp:View ID="ViewMenu" runat="server">
                                                            <div class="form-stacked row">
                                                                <div class="row">
                                                                    <div class="col-lg-12">
                                                                        <div class="row">
                                                                            <div class="col-lg-12">
                                                                                <div class="col-xs-12">
                                                                                    <asp:Button ID="cmdAbrirCâmera" OnClick="AbrirCamera_Click" CssClass="btn btn-sm btn-primary mb-2" Text="Abrir Leitor" runat="server" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                        <div class="row">
                                                                            <div class="col-xs-12" id="DivBipador" runat="server">
                                                                                <div class="col-lg-12">
                                                                                    <div class="table-responsive" style="border: 2px solid #ccc; border-radius: 5px; margin-bottom: 10px;">
                                                                                        <%--    <div id="camera"></div>
                                                                                        <div id="resultado" style="font-size: 18px;"></div>--%>
                                                                                        <uc1:LeitorQuagga runat="server" ID="LeitorQuagga1" />
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                        <br />
                                                                        <div class="row">
                                                                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaUnItens" />
                                                                            <div class="col-lg-12" id="DivManual" runat="server">
                                                                                <div class="col-xs-6">
                                                                                    <asp:TextBox ID="txtsCodigoBarras" CssClass="form-control" runat="server" placeholder="Código de Barras"></asp:TextBox><%--OnTextChanged="txtsCodigoBarras_TextChanged" AutoPostBack="true" EnableViewState="true"--%>
                                                                                </div>
                                                                                <div class="col-xs-6">
                                                                                    <asp:Button ID="cmdIncluirUnitizadoItem" OnClick="IncluirUnitizadosItens_Click" CssClass="btn btn-sm btn-primary" Text="Incluir" runat="server" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                        <br />
                                                                    </div>
                                                                </div>
                                                                <div class="row">
                                                                    <div class="col-lg-12">
                                                                        <div id="div_UnitizadosItens" class="table-responsive" runat="server">
                                                                            <asp:GridView ID="dtgUNItens" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed"
                                                                                Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                                ShowFooter="False" DataKeyNames="idItem">
                                                                                <Columns>
                                                                                    <asp:BoundField DataField="SCodigo" HeaderText="Código Produto" />
                                                                                    <asp:BoundField DataField="SdscProduto" HeaderText="Produto" />
                                                                                    <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade" />
                                                                                </Columns>
                                                                            </asp:GridView>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </asp:View>

                                                        <asp:View ID="viewArquivos" runat="server">
                                                            <h3>Adicione as Imagens Abaixo:</h3>
                                                            <div class="col-lg-12" runat="server" id="div_arquivo">
                                                                <div class=" embed-responsive embed-responsive-16by9" style="min-height: 200px">
                                                                    <embed type="text/html" runat="server" id="frmArquivos" width="800" height="500" />
                                                                </div>
                                                            </div>
                                                        </asp:View>
                                                    </asp:MultiView>
                                                </asp:View>

                                                <%--View Volumes--%>
                                                <asp:View ID="ViewVolumes" runat="server">
                                                    <br />

                                                    <div class="form-stacked row">
                                                        <div class="row">
                                                            <div class="col-lg-12">
                                                                <div class="row">
                                                                    <div class="col-lg-12">
                                                                        <div class="col-xs-12">
                                                                            <asp:Button ID="cmdAbrirCamVolume" OnClick="AbrirCameraVolume_Click" CssClass="btn btn-sm btn-primary mb-2" Text="Abrir Leitor" runat="server" />
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <div class="row">
                                                                    <div class="col-xs-12" id="DivBipadorVolume" runat="server">
                                                                        <%--<div class="col-lg-12">
                                                                            <div class="table-responsive" style="border: 2px solid #ccc; border-radius: 5px; margin-bottom: 10px;">
                                                                                <div id="cameraVolume"></div>
                                                                                <div id="resultadoVolume" style="font-size: 18px;"></div>
                                                                            </div>
                                                                        </div>--%>
                                                                        <uc1:LeitorQuagga runat="server" ID="LeitorQuagga" />
                                                                    </div>
                                                                </div>
                                                                <br />
                                                                <div class="row">
                                                                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaVolumeBipador" />
                                                                    <div class="col-lg-12" id="Div_formVolume" runat="server">
                                                                        <div class="col-md-6" runat="server" id="divCodigoB">
                                                                            <div class="form-group">
                                                                                <asp:TextBox ID="txtsCodigoBarrasVolume" CssClass="form-control" runat="server" placeholder="Código de Barras"></asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                        <div class="col-md-4">
                                                                            <div class="form-group">
                                                                                <asp:DropDownList ID="ddlEmbalagem" runat="server" class="form-control"></asp:DropDownList>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-lg-12">
                                                                        <div class="col-xs-6">
                                                                            <asp:Button ID="cmdIncluirVolume" OnClick="IncluirVolumesItens_Click" CssClass="btn btn-sm btn-primary" Text="Incluir" runat="server" />
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <br />
                                                            </div>
                                                        </div>
                                                        <div class="row">
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
                                                        </div>
                                                        <br />

                                                        <div id="div_VolumesSalvos" class="row" runat="server">
                                                            <div class="col-lg-12">
                                                                <h4>Volumes - Salvos</h4>
                                                                <%--botão vai ser parte da grid--%>
                                                                <div class="table table-responsive">
                                                                    <asp:GridView ID="dtgVolumes" runat="server" class="table table-striped table-bordered table-hover table-condensed"
                                                                        Width="100%" GridLines="None"
                                                                        HeaderStyle-CssClass="gvHeader" CssClass="gvRow" AlternatingRowStyle-CssClass="gvAltRow"
                                                                        AutoGenerateColumns="False" OnRowDataBound="dtgVolume_RowDataBound"
                                                                        ShowFooter="False" DataKeyNames="idVolume">
                                                                        <Columns>
                                                                            <asp:TemplateField>
                                                                                <ItemTemplate>
                                                                                    <asp:LinkButton ID="btnToggle" runat="server" CssClass="toggle-icon" data-div-id='<%# "tr" + Eval("idVolume") %>'>
                                                                           <i style="cursor: pointer;"></i>
                                                                                    </asp:LinkButton>
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
                                                                                    <asp:LinkButton ID="cmdAbrirArquivos" runat="server" OnClick="AbrirArquivosVolumes_Click" CommandArgument='<%# Eval("idVolume") %>'>
                                                                                          <i class="fa fa-upload"></i>
                                                                                    </asp:LinkButton>
                                                                                    <%# NovaLinha(Eval("idVolume"),"") %>

                                                                                    <asp:GridView ID="dtgVolumesItensGV" runat="server" class="table table-striped table-bordered table-hover table-condensed"
                                                                                        Width="100%" GridLines="None"
                                                                                        HeaderStyle-CssClass="gvHeader" CssClass="gvRow" AlternatingRowStyle-CssClass="gvAltRow"
                                                                                        AutoGenerateColumns="False" OnRowDataBound="dtgVolumeItem_RowDataBound"
                                                                                        ShowFooter="False" DataKeyNames="idObjeto">
                                                                                        <Columns>
                                                                                            <asp:TemplateField>
                                                                                                <ItemTemplate>
                                                                                                    <asp:LinkButton ID="btnToggle" runat="server" CssClass="toggle-icon" data-div-id='<%# "trChild" + Eval("idVolumeItem") %>'>
                                                                           <i style="cursor: pointer;"></i>
                                                                                                    </asp:LinkButton>
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
                                                        </div>

                                                    </div>
                                                </asp:View>
                                                <asp:View ID="ViewArquivos2" runat="server">
                                                    <h3>Adicione as Imagens Abaixo:</h3>
                                                    <div class="col-lg-12" runat="server" id="div_arquivos2">
                                                        <div class=" embed-responsive embed-responsive-16by9" style="min-height: 200px">
                                                            <embed type="text/html" runat="server" id="frmArquivos2" width="800" height="500" />
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-12" runat="server" id="div2">
                                                        <asp:Button ID="cmdVoltarModal" runat="server" Text="Voltar" OnClick="VoltarModal_Click" CssClass="btn btn-warning" />
                                                    </div>
                                                </asp:View>
                                            </asp:MultiView>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="modal-footer">
                            <div class="row">
                                <div class="col-lg-6">
                                    <!-- Barra de Progresso-->
                                    <div class="progress">
                                        <div id="divProgresso" runat="server" class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="25" aria-valuemin="0" aria-valuemax="100" style="width: 25%"></div>
                                    </div>
                                </div>
                                <div class="col-lg-6">
                                    <div id="divBotoes" runat="server" class="footer">
                                        <div id="div_botoesUnitizado" runat="server">
                                            <asp:Button ID="cmdAnteriorUnitizado" runat="server" Text="Voltar" OnClick="AnteriorUnitizado_Click" CssClass="btn btn-warning" />
                                            <asp:Button ID="cmdSalvarUnitizado" runat="server" Text="Salvar" CssClass="btn btn-success" OnClick="SalvarUnitizado_Click" />
                                        </div>

                                        <div id="div_botoesVolumes" runat="server">
                                            <asp:Button ID="cmdGerarVolume" OnClick="GerarVolume_Click" CssClass="btn btn-sm btn-primary" Text="Criar Volume" runat="server" />
                                            <asp:Button ID="cmdEfetuarEnvio" runat="server" Text="Efetuar Envio" Visible="false" CssClass="btn btn-success" OnClick="btnEfetuarEnvio_Click" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <script type="text/javascript">
                    // Captura a posição do scroll antes do PostBack
                    Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function () {
                        var scrollPosition = $('#<%= div_scroll.ClientID %>').scrollTop(); // Posição atual
                        console.log("Saving scroll position:", scrollPosition); // Debug
                        $('#<%= hfScrollPosition.ClientID %>').val(scrollPosition); // Salva no HiddenField
                    });

                    // Restaura a posição do scroll após o PostBack
                    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                        var scrollPosition = $('#<%= hfScrollPosition.ClientID %>').val(); // Pega do HiddenField
                        if (scrollPosition) {
                            $('#<%= div_scroll.ClientID %>').scrollTop(scrollPosition); // Restaura
                            console.log("Restored scroll position:", scrollPosition); // Debug
                        }
                    });
                </script>

                <%--script Para Leitor--%>
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

            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <uc1:MensagemPagina runat="server" ID="MensagemPaginaExclusao" />
    <asp:HiddenField ID="hddidOPI" runat="server" />
    <asp:HiddenField ID="hddidEnvioOPI" runat="server" />
    <asp:HiddenField ID="hddidProduto" runat="server" />
    <asp:HiddenField ID="hddidUnitizado" runat="server" />
    <asp:HiddenField ID="hddidUnitizadoUpdt" runat="server" />
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

