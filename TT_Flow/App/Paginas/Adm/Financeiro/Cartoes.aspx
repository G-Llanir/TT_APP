<%@ Page Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Cartoes.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Financeiro.Cartoes" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>
<%@ Register Src="~/App/Controles/Controle_CategoriasCC.ascx" TagPrefix="uc1" TagName="Controle_CategoriasCC" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .modal-dialog {
            width: 60%;
        }

        .modal-content {
            display: flex;
            flex-direction: column;
            height: auto;
            /*overflow-y: auto;*/
        }

        .modal-body {
            max-height: 850px;
            flex: 1 1 auto;
        }

        .modal-largo {
            width: 80% !important;
            max-width: none !important;
        }

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

        .deleteButton {
            width: 40px;
            height: 40px;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            gap: 3px;
            background-color: transparent;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            transition: all 0.2s;
            position: relative;
            overflow: hidden;
            margin-right: 15px;
        }

            .deleteButton svg {
                width: 55%;
            }

            .deleteButton:hover {
                background-color: rgb(237, 56, 56);
                overflow: visible;
            }

        .bin path {
            transition: all 0.2s;
        }

        .deleteButton:hover .bin path {
            fill: #fff;
        }

        .deleteButton:active {
            transform: scale(0.98);
        }

        .tooltip {
            --tooltip-color: rgb(41, 41, 41);
            position: absolute;
            top: -40px;
            background-color: var(--tooltip-color);
            color: white;
            border-radius: 5px;
            font-size: 12px;
            padding: 8px 12px;
            font-weight: 600;
            box-shadow: 0px 10px 10px rgba(0, 0, 0, 0.105);
            display: flex;
            align-items: center;
            justify-content: center;
            opacity: 0;
            transition: all 0.5s;
        }

            .tooltip::before {
                position: absolute;
                width: 10px;
                height: 10px;
                transform: rotate(45deg);
                content: "";
                background-color: var(--tooltip-color);
                bottom: -10%;
            }

        .deleteButton:hover .tooltip {
            opacity: 1;
        }

        #suggestionsContainerCodigo,
        #suggestionsContainerDescricao {
            width: 150%;
            max-height: 200px;
            overflow-y: auto;
            border: 1px solid #ddd;
            z-index: 1000;
            background: white;
            margin-top: -14px;
            margin-left: 15px;
        }

        .suggestion-item {
            padding: 8px 12px;
            cursor: pointer;
        }

            .suggestion-item:hover {
                background-color: #f5f5f5;
            }

        .visible {
            display: none;
        }
    </style>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Cartões"></asp:Label><small>&nbsp;Lançamentos</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>
        <div class="col-lg-12">
            <div class="panel panel-primary" runat="server" id="div_filtro">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;Filtro para pesquisa </h3>
                </div>
                <div class="panel-body ">
                    <div class="col-lg-12">
                        <div class="form-group row">

                            <div class="col-lg-1">
                                <div class="form-group">
                                    <asp:TextBox ID="txtdtInicial" class="form-control CaixaTextoData" placeholder="Data Inicial" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-1">
                                <div class="form-group">
                                    <asp:TextBox ID="txtdtFinal" class="form-control CaixaTextoData" placeholder="Data Final" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-3">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlidCategoriaConsulta" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-2" id="div_ddlsTipo" runat="server">
                                <div class="form-group">
                                    <asp:DropDownList runat="server" ID="ddlsTipoConsulta" class="form-control Caixa_Selecao">
                                        <asp:ListItem Text="Todos os Tipos" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="Crédito" Value="C"></asp:ListItem>
                                        <asp:ListItem Text="Débito" Value="D"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-3" id="div_sPesquisa" runat="server">
                                <div class="form-group">
                                    <asp:TextBox ID="txtPesquisa" class="form-control " placeholder="Lançamento" runat="server"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-2" id="div_ddlAtivo" runat="server">
                                <div class="form-group">
                                    <asp:DropDownList runat="server" ID="ddlAtivoCartao" class="form-control Caixa_Selecao">
                                        <asp:ListItem Text="Ativos e Inativos" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="Ativos" Selected="True" Value="A"></asp:ListItem>
                                        <asp:ListItem Text="Inativos" Value="I"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-2" id="div_botoesFiltro" runat="server">
                                <div class="form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" />
                                    <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo Lançamento" OnClick="cmdNovo_Click" />
                                    <asp:Button ID="btnVoltarGrid2" CssClass="btn-warning btn" runat="server" Text="Voltar" OnClick="btnVoltarGrid_Click" Visible="false" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaGrid" />
                    <asp:Panel ID="pnResultado" class="" runat="server">

                        <div id="div_Lancamentos" runat="server">
                            <div class="panel panel-primary">
                                <div class="panel-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">


                                            <Columns>
                                                <asp:BoundField DataField="sDscCartao" HeaderText="Número Cartão" />

                                                <asp:BoundField DataField="dtLancamento" HeaderText="Data Lançamento" />

                                                <asp:TemplateField HeaderText="Descrição Lançamento">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnsDscLancamento" runat="server"
                                                            Text='<%# Eval("sDscLancamento") %>'
                                                            CommandArgument='<%# Eval("idLancamento") %>'
                                                            OnCommand="cmd_LancamentosCartao">
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="sDscCategoriaPagar" HeaderText="Categoria" />

                                                <asp:BoundField DataField="sTipoLancamento" HeaderText="Tipo Lançamento" />

                                                <asp:BoundField DataField="nVlrLancamento" HeaderText="Valor" DataFormatString="R$ {0:N2}" />

                                                <asp:TemplateField HeaderText="Status">
                                                    <HeaderStyle Width="5%" />
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    <ItemTemplate>
                                                        <asp:Literal ID="litStatus" runat="server"></asp:Literal>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="idLancamento" HeaderText="ID" Visible="false" />

                                            </Columns>
                                        </asp:GridView>
                                        <div class="form-group">
                                            <asp:Button ID="btnVoltarGrid" CssClass="btn-warning btn" runat="server" Text="Voltar" OnClick="btnVoltarGrid_Click" Visible="false" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div id="div_Cartoes" runat="server">
                            <div class="panel panel-primary">
                                <div class="panel-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="gv_Cartao" class="table table-striped table-bordered table-hover "
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">

                                            <Columns>
                                                <asp:BoundField DataField="sDscConta" HeaderText="Banco">
                                                    <ItemStyle Width="10%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="sTitularCartao" HeaderText="Titular">
                                                    <ItemStyle Width="10%" />
                                                </asp:BoundField>
                                                <asp:TemplateField HeaderText="Número do Cartão">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnsCartao" runat="server"
                                                            Text='<%# Eval("sDscCartao") %>'
                                                            CommandArgument='<%# Eval("idCartao") %>'
                                                            OnCommand="cmd_Cartao">
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="sDscBandeira" HeaderText="Bandeira">
                                                    <ItemStyle Width="10%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="dtVenciCartao" HeaderText="Data Vencimento">
                                                    <ItemStyle Width="10%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="nLimiteCartao" HeaderText="Limite" DataFormatString="{0:N2}">
                                                    <ItemStyle Width="10%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="idCartao" HeaderText="CartaoID" Visible="false" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>

                </div>
            </div>

        </div>

    </div>
    <asp:HiddenField runat="server" ID="hddMudaGrid" Value="0" />
    <asp:HiddenField runat="server" ID="hddidLancamento" Value="" />
    <asp:HiddenField runat="server" ID="hddidCartao" Value="" />


    <%--------------------------------------MODAL NOVO LANCAMENTO---------------------------------------------------------------------------------------%>
    <div class="modal fade" id="modalLancamentoDetalhe" tabindex="-2" role="dialog" data-backdrop="static" aria-labelledby="modalLancamentoDetalhe" aria-hidden="true">
        <div class="modal-dialog modal-sm" role="document" style="width: 70%">
            <div class="modal-content">
                <asp:UpdatePanel ID="updModalDetalhe" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" id="btnHeadFecharDetalhe" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <div class="modal-header-content">

                                <div class="modal-logo">
                                    <asp:Image ID="Image2" runat="server" ImageUrl="~/App/img/instrucaoTecnica_pdf.jpg" AlternateText="Instrução Técnica" />
                                </div>

                                <div class="modal-title-container">
                                    <asp:Label runat="server" class="modal-title" ID="lblTituloModal" Text="Lançamento Detalhe" Font-Bold="true"></asp:Label>
                                </div>

                            </div>
                            <div class="modal-body">

                                <div>
                                    <ul id="tab_ModalLancamento" class="nav nav-tabs" role="tablist">
                                        <li role="presentation" class="tabpanel active">
                                            <a href="#detalheLancamento" id="detalhe-tab" role="tab" data-toggle="tab" aria-controls="Lancamento" aria-expanded="false"><b>Lançamento</b></a>
                                        </li>

                                        <li role="presentation" runat="server" id="aba_Historico">
                                            <a href="#historicoLancamento" role="tab" id="historico-tab" data-toggle="tab" aria-controls="Historico"><b>Histórico</b></a>
                                        </li>

                                    </ul>
                                </div>

                                <div id="tab" class="tab-content">

                                    <%--TAB DETALHE--%>

                                    <div role="tabpanel" class="tab-pane fade in active" id="detalheLancamento" aria-labelledby="Lançamento">

                                        <div class="panel panel-default" runat="server" id="div_detalhe">
                                            <div class="panel-body ">
                                                <div class="col-lg-12 row" style="width: 100%; margin-left: 0px; padding: 0px;">
                                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                        <ContentTemplate>
                                                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaModalDetalhe" style="margin-left: 0px !important" />
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </div>

                                                <div class="form-group">
                                                    <div class="col-lg-12">
                                                        <div class="row">

                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Data Lançamento</label>
                                                                    <asp:TextBox ID="txtdtLancamento" type="date" class="form-control" runat="server"></asp:TextBox>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-6">
                                                                <div class="form-group">
                                                                    <label>Cartão</label>
                                                                    <asp:DropDownList runat="server" ID="ddlidCartao" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Usuário</label>
                                                                    <asp:DropDownList runat="server" ID="ddlidUsuario" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                                </div>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <div class="col-lg-12">
                                                        <div class="row">

                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Tipo</label>
                                                                    <asp:DropDownList runat="server" ID="ddlsTipo" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlsTipo_SelectedIndexChanged">
                                                                        <asp:ListItem Text="Selecione um Tipo" Value="0"></asp:ListItem>
                                                                        <asp:ListItem Text="Crédito" Value="C"></asp:ListItem>
                                                                        <asp:ListItem Text="Débito" Value="D"></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                            </div>


                                                            <div class="col-lg-6">
                                                                <div class="form-group">
                                                                    <label>Descrição do Lançamento</label>
                                                                    <asp:TextBox ID="txtsDscLancamento" class="form-control" runat="server"></asp:TextBox>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Valor Total (R$)</label>
                                                                    <asp:TextBox ID="txtnValor" class="form-control" runat="server" AutoPostBack="true" OnTextChanged="txtnValor_TextChanged"></asp:TextBox>
                                                                </div>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="form-group" runat="server" id="div_parcelado">
                                                    <div class="col-lg-12">
                                                        <div class="row">

                                                            <div class="col-lg-1" runat="server" id="div_swtParcelado">
                                                                <div class="form-group">
                                                                    <uc1:SwitchAtivo ID="SwitchParcelado" runat="server" />
                                                                </div>
                                                            </div>
                                                            <div runat="server" id="div_qtdParcela">
                                                                <div class="col-lg-2">
                                                                    <div class="form-group">
                                                                        <label>Quantidade de Parcelas</label>
                                                                        <asp:TextBox ID="txtnQuantidade" TextMode="Number" min="1" step="1" class="form-control" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-2">
                                                                <uc1:SwitchAtivo ID="swtCompraInternacional" runat="server" />
                                                            </div>

                                                            <div runat="server" id="div_compraInternacional">

                                                                <div class="col-lg-4">
                                                                    <div class="form-group">
                                                                        <label>Moeda</label>
                                                                        <asp:DropDownList ID="ddlMoeda" class="form-control Caixa_Selecao" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlMoeda_SelectedIndexChanged"></asp:DropDownList>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-3">
                                                                    <div class="form-group">
                                                                        <label>Valor Câmbio</label>
                                                                        <asp:TextBox ID="txtValorCambio" class="form-control" runat="server" AutoPostBack="true" OnTextChanged="txtValorCambio_TextChanged"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <div class="col-lg-12">
                                                        <div class="row">

                                                            <div class="col-lg-4">
                                                                <div class="form-group">
                                                                    <label>Categoria</label>
                                                                    <asp:DropDownList ID="ddlidCategoriaPagar" class="form-control Caixa_Selecao" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidCategoriaPagar_SelectedIndexChanged"></asp:DropDownList>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-4">
                                                                <div class="form-group">
                                                                    <label>Código Contábil</label>
                                                                    <asp:DropDownList ID="ddlidContabil" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                                                </div>
                                                            </div>

                                                                  <%-- Thiago Rodrigues - 02/09/2025--%>
                                                        <%--    <div class="col-lg-4">
                                                                <div class="form-group">
                                                                    <label>Centro de Custo</label>
                                                                    <asp:DropDownList ID="ddlidCentroDeCusto" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                                                </div>
                                                            </div>--%>

                                                            <%-- Thiago Rodrigues - 02/09/2025--%>
                                                            <uc1:Controle_CategoriasCC runat="server" ID="Controle_CategoriasCC" SAcao="subtrair"  />
                                                                       

                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="panel panel-default" runat="server" id="div_gridProdutos">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>Produtos</b></h3>
                                            </div>
                                            <div class="panel-body">

                                                <div class="form-group" runat="server" id="div_InserirProdutos">
                                                    <div class="col-lg-12">
                                                        <div class="row">

                                                            <div class="col-lg-2">
                                                                <div class="form-group">
                                                                    <label>Código</label>
                                                                    <asp:TextBox ID="txtsCodigoProduto" runat="server" CssClass="form-control" placeholder="Código"></asp:TextBox>
                                                                    <div id="suggestionsContainerCodigo" class="dropdown-menu" style="display: none;"></div>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-5">
                                                                <div class="form-group">
                                                                    <label>Descrição</label>
                                                                    <asp:TextBox ID="txtsDscProduto" runat="server" CssClass="form-control" placeholder="Descrição"></asp:TextBox>
                                                                    <div id="suggestionsContainerDescricao" class="dropdown-menu" style="display: none;"></div>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Unidade</label>
                                                                    <asp:DropDownList ID="ddlsUnidadeProduto" runat="server" CssClass="form-control"></asp:DropDownList>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-2">
                                                                <div class="form-group">
                                                                    <label>Quantidade</label>
                                                                    <asp:TextBox ID="txtnQuantidadeProduto" TextMode="Number" min="1" step="1" class="form-control" runat="server"></asp:TextBox>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Valor Unitário</label>
                                                                    <asp:TextBox ID="txtnValorProduto" class="form-control" runat="server"></asp:TextBox>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-2">
                                                                <div class="form-group">
                                                                    <asp:Button ID="btnIncluirProduto" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="btnIncluirProduto_Click" Style="margin-top: 24px;" />
                                                                </div>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>


                                                <div class="form-stacked row">
                                                    <div class="col-lg-12 table-responsive" style="padding: 5px 30px;">

                                                        <asp:GridView
                                                            ID="dtgv_produtosLancamento" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="idLinha"
                                                            ShowFooter="False">
                                                            <Columns>
                                                                <asp:BoundField DataField="idLinha" HeaderText="Ordem">
                                                                    <ItemStyle Width="2%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="sCodProduto" HeaderText="Código">
                                                                    <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="sDscProduto" HeaderText="Descrição">
                                                                    <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="sUnidade" HeaderText="Unidade">
                                                                    <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade">
                                                                    <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="nValorUnitario" HeaderText="Valor Unitário" DataFormatString="{0:N2}">
                                                                    <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="nValorTotal" HeaderText="Total" DataFormatString="{0:N2}">
                                                                    <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField runat="server" ID="hddidLinha" Value='<%# Eval("idLinha") %>' />
                                                                        <asp:LinkButton ID="lnkDescricao_Editar" runat="server" CssClass="btn btn-small" ToolTip="Editar" TabIndex="99" OnClick="lnkDescricao_Editar_Click"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="lnkDescricao_Excluir" runat="server" ToolTip="Excluir" CssClass="btn btn-small" TabIndex="100" OnClick="lnkDescricao_Excluir_Click"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                            </Columns>
                                                        </asp:GridView>

                                                    </div>
                                                </div>

                                            </div>
                                        </div>
                                    </div>

                                    <%--TAB HISTORICO--%>

                                    <div role="tabpanel" class="tab-pane fade" id="historicoLancamento" aria-labelledby="Historico">
                                        <br />
                                        <div class="panel panel-default" runat="server" id="DIV_historico">
                                            <div class="panel-body">
                                                <div class="form-stacked row">
                                                    <div class="col-lg-12 table-responsive">
                                                        <asp:GridView
                                                            ID="gv_Historico" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            ShowFooter="False" OnRowDataBound="gv_Historico_RowDataBound">
                                                            <Columns>
                                                                <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Atualização">
                                                                    <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário">
                                                                    <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="sAcao" HeaderText="Tipo">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="sDscAcao" HeaderText="Ação">
                                                                    <ItemStyle Width="70%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>
                                                            </Columns>
                                                        </asp:GridView>

                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="modal-footer">

                                        <div class="btn" runat="server" id="div_btnExcluir" style="padding: 0px">
                                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                <ContentTemplate>
                                                    <button id="btExcluir" class="deleteButton" runat="server" onserverclick="btnExcluir_Click">
                                                        <svg
                                                            xmlns="http://www.w3.org/2000/svg"
                                                            fill="none"
                                                            viewBox="0 0 50 59"
                                                            class="bin">
                                                            <path
                                                                fill="#B5BAC1"
                                                                d="M0 7.5C0 5.01472 2.01472 3 4.5 3H45.5C47.9853 3 50 5.01472 50 7.5V7.5C50 8.32843 49.3284 9 48.5 9H1.5C0.671571 9 0 8.32843 0 7.5V7.5Z">
                                                            </path>
                                                            <path
                                                                fill="#B5BAC1"
                                                                d="M17 3C17 1.34315 18.3431 0 20 0H29.3125C30.9694 0 32.3125 1.34315 32.3125 3V3H17V3Z">
                                                            </path>
                                                            <path
                                                                fill="#B5BAC1"
                                                                d="M2.18565 18.0974C2.08466 15.821 3.903 13.9202 6.18172 13.9202H43.8189C46.0976 13.9202 47.916 15.821 47.815 18.0975L46.1699 55.1775C46.0751 57.3155 44.314 59.0002 42.1739 59.0002H7.8268C5.68661 59.0002 3.92559 57.3155 3.83073 55.1775L2.18565 18.0974ZM18.0003 49.5402C16.6196 49.5402 15.5003 48.4209 15.5003 47.0402V24.9602C15.5003 23.5795 16.6196 22.4602 18.0003 22.4602C19.381 22.4602 20.5003 23.5795 20.5003 24.9602V47.0402C20.5003 48.4209 19.381 49.5402 18.0003 49.5402ZM29.5003 47.0402C29.5003 48.4209 30.6196 49.5402 32.0003 49.5402C33.381 49.5402 34.5003 48.4209 34.5003 47.0402V24.9602C34.5003 23.5795 33.381 22.4602 32.0003 22.4602C30.6196 22.4602 29.5003 23.5795 29.5003 24.9602V47.0402Z"
                                                                clip-rule="evenodd"
                                                                fill-rule="evenodd">
                                                            </path>
                                                            <path fill="#B5BAC1" d="M2 13H48L47.6742 21.28H2.32031L2 13Z"></path>
                                                        </svg>

                                                        <span class="tooltip">Excluir</span>
                                                    </button>

                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>

                                        <asp:Button ID="btnSalvar" CssClass="btn-success btn" runat="server" Text="Salvar" OnClick="btnSalvar_Click" />
                                        <%--                                    <button id="btnFecharFooterDetalhe" type="button" class="btn btn-secondary" data-dismiss="modal">Fechar</button>--%>

                                        <%--------------------------------------MODAL AÇAO---------------------------------------------------------------------------------------%>
                                        <div class="modal fade" id="modalAcao" tabindex="-2" role="dialog" data-backdrop="static" aria-labelledby="modaAcaoExcluir" aria-hidden="true" style="height: 100%;">
                                            <div class="modal-dialog modal-sm" role="document" style="width: 35%;">
                                                <div class="modal-content" style="overflow-x: hidden;">
                                                    <div class="modal-body" style="padding: 0px;">
                                                        <div class="row" runat="server" id="div_Acao_Excluir">
                                                            <div class="col-lg-12">
                                                                <div class="panel panel-primary" style="margin: 0px;">
                                                                    <div class="panel-heading">
                                                                        <h3 class="panel-title">
                                                                            <asp:Label ID="lblAcao_Titulo" runat="server" Text="Excluir"></asp:Label></h3>
                                                                    </div>
                                                                    <div class="panel-body">
                                                                        <div class="col-lg-12">
                                                                            <div class="form-group">
                                                                                <label>Confirma a exclusão deste lançamento?</label>
                                                                            </div>
                                                                        </div>

                                                                        <div class="col-lg-12">
                                                                            <div class="form-group">
                                                                                <asp:Button ID="Button1" class="btn btn-sm btn-success" runat="server" Text="Sim" OnClick="btnOK_Click" />
                                                                                <asp:Button ID="Button2" class="btn btn-sm btn-danger" runat="server" Text="Não" OnClick="btnCancelar_Click" />
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <asp:HiddenField runat="server" ID="hddTipoCategoria" Value="" />
                                    <asp:HiddenField runat="server" ID="hddidLancamentoItem" Value="" />
                                    <asp:HiddenField runat="server" ID="hddidProduto" Value="0" />
                                </div>
                            </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <script>
        function refreshPagina() {
            var hiddenField = document.getElementById('<%= hddMudaGrid.ClientID %>');
            var hiddenField2 = document.getElementById('<%= hddidCartao.ClientID %>');
            $('#modalLancamentoDetalhe').on('hidden.bs.modal', function () {
                var url = '/App/Paginas/Adm/Financeiro/Cartoes.aspx';
                url += '?grid=' + encodeURIComponent(hiddenField.value);
                url += '&cartao=' + encodeURIComponent(hiddenField2.value);
                window.location.href = url;
            });
            $('#modalLancamentoDetalhe').modal('hide');

        }

        function recarregaEvento() {
            document.getElementById('btnHeadFecharDetalhe').addEventListener('click', function () {
                refreshPagina();
            });
        }

        recarregaEvento();

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            recarregaEvento();
        });

        $(function () {
            var $dtInicial = $('[id*=txtdtInicial]');
            var $dtFinal = $('[id*=txtdtFinal]');

            $dtInicial.datepicker({
                autoclose: true,
                format: 'dd/mm/yyyy',
                language: 'pt-BR'
            }).mask('99/99/9999');

            $dtFinal.datepicker({
                autoclose: true,
                format: 'dd/mm/yyyy',
                language: 'pt-BR'
            }).mask('99/99/9999');

            $dtInicial.data('previous', $dtInicial.val());
            $dtFinal.data('previous', $dtFinal.val());
        });

    </script>

</asp:Content>

