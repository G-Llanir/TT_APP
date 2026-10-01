<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="ContasReceber.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Financeiro.ContasReceber" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <style>
        .dataTables_scrollFootInner {
            padding-right: 0px !important;
        }

        div#cphCorpo_dtgvConsulta_filter {
            margin-right: 25px;
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

        body {
            font-family: Arial, sans-serif;
        }

        h2 {
            color: #333;
        }

        .modal-content {
            overflow-y: inherit !important;
        }

        .gridViewStyle {
            border-collapse: collapse;
            margin: 20px 0;
            width: 100%;
            box-shadow: 0 2px 15px rgba(0, 0, 0, 0.15);
        }

            .gridViewStyle th, .gridViewStyle td {
                border: 1px solid #ddd;
                padding: 12px 15px;
                text-align: left;
            }

            .gridViewStyle th {
                background-color: #009A22;
                color: #ffffff;
            }

            .gridViewStyle tr:nth-child(even) {
                background-color: #f2f2f2;
            }

            .gridViewStyle tr:hover {
                background-color: #ddd;
            }

            .gridViewStyle a {
                color: #007bff;
                text-decoration: none;
            }

                .gridViewStyle a:hover {
                    text-decoration: underline;
                }

        .scrollable-grid {
            overflow-x: auto;
        }
       
    </style>

    <div class="form-stacked row">

        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Contas a Receber"></asp:Label><small> Consulta</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>

        <div class="col-lg-12" runat="server" id="DIV_Filtro">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa</h3>
                </div>
                <div class="panel-body">
                    <div class="form-group">
                        <div class="row">

                            <div class="col-lg-2">
                                <div class="form-group row">
                                    <div class="col-lg-6">
                                        <asp:TextBox ID="txtdtInicio" class="form-control CaixaTextoData" placeholder="Emissão" runat="server" MaxLength="10" type="date"></asp:TextBox>
                                    </div>
                                    <div class="col-lg-6">
                                        <asp:TextBox ID="txtdtFinal" class="form-control CaixaTextoData" runat="server" placeholder="Vencimento" MaxLength="10" type="date"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlidDataPesquisa" runat="server" class="form-control yes_no select Caixa_Selecao" attrname="idContasPagar">
                                        <asp:ListItem Selected="True" Value="0">Vencimento</asp:ListItem>
                                        <asp:ListItem Value="1">Emissão</asp:ListItem>
                                        <asp:ListItem Value="2">Liquidação</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlidContabil" class="form-control Caixa_Selecao" runat="server" attrname="idContasReceber"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlidCategoriaReceber" class="form-control Caixa_Selecao" runat="server" attrname="idContasReceber"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlidFormaRecebimento" class="form-control Caixa_Selecao" runat="server" attrname="idContasReceber"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlidCentroDeCusto" class="form-control Caixa_Selecao" runat="server" attrname="idContasReceber"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlidEmpresa" class="form-control Caixa_Selecao" runat="server" attrname="idContasReceber"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlsStatus" runat="server" class="form-control yes_no select Caixa_Selecao Caixa_Selecao" attrname="idContasReceber">
                                        <asp:ListItem Selected="True" Value="">Todos os Status</asp:ListItem>
                                        <asp:ListItem Value="Liquidado">Liquidado</asp:ListItem>
                                        <asp:ListItem Value="Em Aberto">Em Aberto</asp:ListItem>
                                        <asp:ListItem Value="Em Atraso">Em Atraso</asp:ListItem>
                                        <asp:ListItem Value="Vencendo Hoje">Vencendo Hoje</asp:ListItem>
                                        <asp:ListItem Value="A vencer em 7 dias">A vencer em 7 dias</asp:ListItem>
                                        <asp:ListItem Value="Protestado">Protestado</asp:ListItem>
                                        <asp:ListItem Value="Cobrança Judicial">Cobrança Judicial</asp:ListItem>
                                        <asp:ListItem Value="Inadimplente">Inadimplente</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlsTituloAdiantado" runat="server" class="form-control yes_no select Caixa_Selecao" attrname="idContasReceber">
                                        <asp:ListItem Selected="True" Value="T">Todos os Títulos</asp:ListItem>
                                        <asp:ListItem Value="S">Adiantados</asp:ListItem>
                                        <asp:ListItem Value="N">Não Adiantados</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlidConta_Info_Rec" class="form-control Caixa_Selecao" runat="server" attrname="idContasReceber"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlsConciliado" class="form-control Caixa_Selecao" runat="server" attrname="idContasPagar">
                                        <asp:ListItem Selected="True" Value="T">Conciliados e Não Conciliados</asp:ListItem>
                                        <asp:ListItem Value="S">Conciliados</asp:ListItem>
                                        <asp:ListItem Value="N">Não Conciliado</asp:ListItem>
                                        <asp:ListItem Value="P">Parcialmente Conciliado</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlsCLiente" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao_Pesquisa()" />
                                    <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Nova Conta a Receber" OnClick="cmdNovo_Click" />
                                    <asp:Button ID="cmdAdiantar" class="btn btn-info" runat="server" Text="Adiantar Títulos" OnClick="cmdAdiantar_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-lg-12" runat="server" id="DIV_Adiantamento">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Selecione Títulos para Adiantar</h3>
                </div>
                <div class="panel-body ">
                    <div class="form-group">

                        <div class="col-lg-6">
                            <div class="form-group">
                                <asp:Button ID="cmdVoltarAdiantamento" class="btn btn-success" runat="server" Text="Voltar a Conta a Receber" OnClick="cmdVoltarAdiantamento_Click" />
                                <asp:Button ID="cmdAdiantamento" class="btn btn-info" runat="server" OnClientClick="prepararEabrirModalAdiantamento(); return false;" Text="Adiantar selecionados" Style="display: none;" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>

        <div class="col-lg-12">

            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

            <asp:Panel ID="pnResultado" class="" runat="server">

                <div id="resultado">
                    <div class="panel panel-primary">
                        <div class="panel-body">

                            <%----------------------------------------------------%>
                            <button type="button" class="btn" data-toggle="collapse" data-target="#toggleButtons" style="background-color: #009a22; color: white;">Mostrar/Ocultar Colunas</button>

                            <div id="toggleButtons" class="collapse">
                                <br />
                                <asp:PlaceHolder ID="placeholderButtons" runat="server"></asp:PlaceHolder>
                            </div>

                            <br />
                            <br />
                            <%----------------------------------------------------%>

                            <div class="table-responsive">
                                <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                    DataKeyNames="idContasReceber" runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="true" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound" UseAccessibleHeader="true">
                                    <Columns>

                                        <%--==============================
                                        === N° Célula ------- Info ===
                                        ==============================
                                        0 --------------- CheckBox
                                        1 --------------- ID
                                        2 --------------- Cliente
                                        3 --------------- Data Emissão
                                        4 --------------- Data Vencimento
                                        5 --------------- Dias Até Vencimento
                                        6 --------------- Pedido
                                        7 --------------- Parcela
                                        8 --------------- Nota Fiscal
                                        9 --------------- Valor Bruto
                                        10 -------------- Valor Liquido
                                        11 -------------- Valor Pago
                                        12 -------------- Saldo em Aberto
                                        13 -------------- Categoria
                                        14 -------------- Status
                                        15 -------------- Conciliação
                                        16 -------------- Forma de Recebimento
                                        17 -------------- Data Liquidação
                                        18 -------------- Empresa
                                        19 -------------- idEmpresa
                                        20 -------------- Titulo Adiantado
                                        ==============================--%>

                                        <asp:TemplateField HeaderText="">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkTitulo_Selecionado" runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="4%" />
                                        </asp:TemplateField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                            DataTextField="idContasReceber" HeaderText="ID"
                                            DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <HeaderStyle Width="4%" />
                                        </asp:HyperLinkField>

                                        <asp:BoundField DataField="idContasReceber" HeaderText="ID" SortExpression="idContasReceber" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="4%" />
                                        <asp:BoundField DataField="SRazaoSocial" HeaderText="Cliente" SortExpression="SRazaoSocial" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="33%" />
                                        <asp:BoundField DataField="dtEmissao_ordem" HeaderText="Data Emissão" DataFormatString="{0:u}" SortExpression="dtEmissao_ordem" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="8%" />
                                        <asp:BoundField DataField="dtVencimento_ordem" HeaderText="Data Vencimento" DataFormatString="{0:u}" SortExpression="dtVencimento_ordem" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="8%" />

                                        <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                            DataTextField="sDiasParaVencer" HeaderText="Dias Até Venc."
                                            DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <HeaderStyle Width="9%" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                            DataTextField="sDscEmpresa" HeaderText="Empresa"
                                            DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <HeaderStyle Width="5%" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                            DataTextField="sDocumento" HeaderText="Pedido"
                                            DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <HeaderStyle Width="8%" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                            DataTextField="sQuantidadeParcela" HeaderText="Parcela"
                                            DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <HeaderStyle Width="4%" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                            DataTextField="sCodigo" HeaderText="Nota Fiscal"
                                            DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <HeaderStyle Width="9%" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idContasReceber" DataTextFormatString="{0:C2}"
                                            DataTextField="nValorBruto" HeaderText="Valor Bruto"
                                            DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <HeaderStyle Width="11%" />
                                        </asp:HyperLinkField>

                                        <asp:BoundField DataField="nValorOriginal" HeaderText="Valor Líquido" DataFormatString="{0:C2}" SortExpression="nValorOriginal" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="12%" />

                                        <asp:BoundField DataField="nTotal" HeaderText="Valor Recebido" DataFormatString="{0:C2}">
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <HeaderStyle Width="11%" />
                                        </asp:BoundField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idContasReceber" DataTextFormatString="{0:C2}"
                                            DataTextField="nSaldo" HeaderText="Saldo em Aberto"
                                            DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <HeaderStyle Width="12%" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                            DataTextField="sDscCategoriaReceber" HeaderText="Categoria"
                                            DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <HeaderStyle Width="10%" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                            DataTextField="sStatus" HeaderText="Status"
                                            DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <HeaderStyle Width="10%" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                            DataTextField="sDscFormaRecebimento" HeaderText="Forma de Recebimento"
                                            DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <HeaderStyle Width="10%" />
                                        </asp:HyperLinkField>

                                        <asp:BoundField DataField="dtLiquidado" HeaderText="Data Liquidação">
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <HeaderStyle Width="10%" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="dtConciliado" HeaderText="Data Conciliação">
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <HeaderStyle Width="10%" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="idEmpresa" HeaderText="idEmpresa">
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <HeaderStyle Width="1%" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sTituloAdiantado" HeaderText="Titulo Adiantado">
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <HeaderStyle Width="1%" />
                                        </asp:BoundField>

                                    </Columns>
                                </asp:GridView>
                            </div>

                        </div>
                    </div>
                </div>
            </asp:Panel>

        </div>

    </div>

    <div class="modal fade" id="Modal_Adiantamento" tabindex="-1" role="dialog" aria-labelledby="ModalAdiantamento_Label" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog modal-xl" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="modal-header-content">
                        <div class="modal-logo">
                            <asp:Image ID="Image1" runat="server" ImageUrl="~/App/img/instrucaoTecnica_pdf.jpg" AlternateText="Instrução Técnica" />
                        </div>
                        <div class="modal-title-container">
                            <h4 class="modal-title" id="ModalAdiantamento_Label">Adiantar Títulos</h4>
                        </div>
                    </div>

                </div>
                <div class="modal-body">

                    <div class="col-lg-12 row">
                        <asp:UpdatePanel ID="UpdatePanelMensagem" runat="server">
                            <ContentTemplate>
                                <uc1:MensagemPagina runat="server" ID="Adiantamento_MensagemPagina" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-12">

                            <asp:UpdatePanel ID="UpdatePanel_Limpacampos" runat="server">
                                <ContentTemplate>

                                    <div class="row">
                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label>Banco Credor:</label>
                                                <asp:DropDownList ID="ddlidConta_Info_Pag" CssClass="form-control Caixa_Selecao trigger-hide-button" runat="server"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-5">
                                            <div class="form-group">
                                                <label>Categoria Contas Pagar:</label>
                                                <asp:DropDownList ID="ddlidCategoriaPagar" CssClass="form-control Caixa_Selecao trigger-hide-button" runat="server"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>Metódo de Cálculo</label>
                                                <asp:DropDownList ID="ddlsTipoCalculo" CssClass="form-control Caixa_Selecao" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlsTipoCalculo_SelectedIndexChanged">
                                                    <asp:ListItem Text="Selecione um Metódo" Value="0"></asp:ListItem>
                                                    <asp:ListItem Text="Manual" Value="M"></asp:ListItem>
                                                    <asp:ListItem Text="Proporcional" Value="P"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>


                                    </div>
                                    <div class="row">
                                        <div class="col-lg-3">
                                            <div class="form-group">
                                                <label>Data do Adiantamento</label>
                                                <asp:TextBox ID="txtdtEmissaoAdiantamento" class="form-control " runat="server" placeholder="Vencimento" MaxLength="10" type="date"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div runat="server" id="div_valoresAdiantamento" visible="false">
                                            <div class="col-lg-2" runat="server" id="div_valorLiberado">
                                                <div class="form-group">
                                                    <label>Valor Liberado</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon">R$</span>
                                                        <asp:TextBox ID="txtnValorLiberado" CssClass="form-control trigger-hide-button" Style="z-index: 0;" placeholder="R$ 0.000,00" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Juros ao mês %:</label>
                                                    <asp:TextBox ID="txtnTaxaJuros" CssClass="form-control trigger-hide-button" placeholder="00,00%" MaxLength="20" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2" runat="server" id="div_totalJuros">
                                                <div class="form-group">
                                                    <label>Total Juros</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon">R$</span>
                                                        <asp:TextBox ID="txtnTotalJurosValor" CssClass="form-control trigger-hide-button" Style="z-index: 0;" placeholder="R$ 0.000,00" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>IOF Diário %</label>
                                                    <asp:TextBox ID="txtnIOF" CssClass="form-control trigger-hide-button" placeholder="00,0000000%" MaxLength="20" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>IOF Adicional %</label>
                                                    <asp:TextBox ID="txtnIOFAdicional" CssClass="form-control trigger-hide-button" placeholder="00,0000000%" MaxLength="20" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2" runat="server" id="div_totalIOF">
                                                <div class="form-group">
                                                    <label>Total IOF</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon">R$</span>
                                                        <asp:TextBox ID="txtnTotalIOFValor" CssClass="form-control trigger-hide-button" Style="z-index: 0;" placeholder="R$ 0.000,00" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Tarifas:</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon">R$</span>
                                                        <asp:TextBox ID="txtnTarifas" CssClass="form-control trigger-hide-button" Style="z-index: 0;" placeholder="R$ 0.000,00" MaxLength="20" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>


                                    <div class="col-lg-12 text-right">
                                        <asp:Button ID="btnSimularAdiantamento" runat="server" CssClass="btn btn-info" OnClick="btnSimularAdiantamento_Click" Text="Simular" />
                                    </div>


                                    <asp:Button ID="btnLimpaCampos" runat="server" Text="Limpar Campos" OnClick="btnLimpaCampos_Click" Style="display: none;" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>

                    <div id="simulacaoGrid_Detalhe" runat="server">
                        <div class="row">
                            <div class="col-lg-12">
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                    <ContentTemplate>
                                        <asp:GridView ID="gvSimulacaoAdiantamento_Detalhe" runat="server" AutoGenerateColumns="False" CssClass="gridViewStyle"
                                            OnRowDataBound="gvSimulacaoAdiantamento_Detalhe_RowDataBound" Font-Size="11px">
                                            <Columns>
                                                <asp:BoundField DataField="idContasReceber_Detalhe" HeaderText="ID" SortExpression="idContasReceber" />
                                                <asp:BoundField DataField="sDscCliente_Detalhe" HeaderText="Cliente" SortExpression="sDscCliente" />
                                                <asp:BoundField DataField="dtVencimento_Detalhe" HeaderText="Vencimento" SortExpression="dtVencimento_Detalhe" DataFormatString="{0:dd-MM-yyyy}" ItemStyle-Width="8%" />
                                                <asp:BoundField DataField="nDias_Detalhe" HeaderText="N° Dias" SortExpression="nDias_Detalhe" ItemStyle-Width="5%" />
                                                <asp:BoundField DataField="nValorTitulo" HeaderText="Valor do Título" SortExpression="valorOriginal" DataFormatString="{0:C2}" />
                                                <asp:BoundField DataField="nJurosTotal_Detalhe" HeaderText="Valor Juros" SortExpression="nJurosTotal" DataFormatString="{0:C2}" />
                                                <asp:BoundField DataField="nSomaIOF_Detalhe" HeaderText="Valor IOF" SortExpression="nSomaIOF_Detalhe" DataFormatString="{0:C2}" />
                                                <asp:BoundField DataField="nDivisaoTarifa" HeaderText="Tarifa" SortExpression="nDivisaoTarifa" DataFormatString="{0:C2}" />
                                                <asp:BoundField DataField="nValorLiberado_Detalhe" HeaderText="Valor Liberado" SortExpression="nValorLiberado_Detalhe" DataFormatString="{0:C2}" />
                                                <asp:BoundField DataField="nTotalDespesas" HeaderText="Total Despesas" SortExpression="nTotalDespesas" DataFormatString="{0:C2}" />
                                            </Columns>
                                        </asp:GridView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>

                    <div id="simulacaoGrid" runat="server">
                        <div class="row">
                            <div class="col-lg-12">
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                    <ContentTemplate>
                                        <h3 id="tituloResumo" runat="server" style="text-align: left;" visible="false">Resumo</h3>
                                        <asp:GridView ID="gvSimulacaoAdiantamento" runat="server" AutoGenerateColumns="False" CssClass="gridViewStyle" OnRowDataBound="gvSimulacaoAdiantamento_RowDataBound" Font-Size="11px">
                                            <Columns>
                                                <asp:BoundField DataField="nSomaValorOriginal" HeaderText="Valor do Título" SortExpression="nSomaValorOriginal" DataFormatString="{0:C2}" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="nJurosTotal" HeaderText="Valor Juros" SortExpression="nJurosTotal" DataFormatString="{0:C2}" ItemStyle-Width="11%" />
                                                <asp:BoundField DataField="nSomaIOF" HeaderText="Valor IOF" SortExpression="nSomaIOF" DataFormatString="{0:C2}" ItemStyle-Width="11%" />
                                                <asp:BoundField DataField="nTarifas" HeaderText="Tarifa" SortExpression="nTarifas" DataFormatString="{0:C2}" ItemStyle-Width="11%" />
                                                <asp:BoundField DataField="nValorLiberado" HeaderText="Total Liberado" SortExpression="nValorLiberado" DataFormatString="{0:C2}" ItemStyle-Width="18%" />
                                                <asp:BoundField DataField="nDespesas" HeaderText="Total Despesas" SortExpression="nDespesas" DataFormatString="{0:C2}" ItemStyle-Width="13%" />
                                            </Columns>
                                        </asp:GridView>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btnSimularAdiantamento" EventName="Click" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>

                </div>
                <div class="modal-footer">
                    <div class="col-lg-12 text-right">
                        <asp:UpdatePanel ID="UpdatePanelBotao" runat="server">
                            <ContentTemplate>
                                <asp:Button ID="btnConfirmarAdiantamento" runat="server" CssClass="btn btn-success" OnClick="btnConfirmarAdiantamento_Click" Text="Confirmar Adiantamento" Visible="true" Style="margin-right: 10px;" />
                                <button type="button" class="btn btn-danger" data-dismiss="modal">Cancelar</button>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>

    </div>

    <script type="text/javascript">
        $(document).ready(function () {
            $('#<%=cmdAdiantamento.ClientID%>').hide();

            $(document).on('click', '[id*=chkTitulo_Selecionado]', function () {
                if ($('[id*=chkTitulo_Selecionado]:checked').length > 0) $('#<%=cmdAdiantamento.ClientID%>').show();
                else $('#<%=cmdAdiantamento.ClientID%>').hide();
            });
        });

        $(document).ready(function () {
            $('#Modal_Adiantamento').on('keypress', function (e) {
                if (e.which == 13) { // Enter
                    if ($('#<%= btnConfirmarAdiantamento.ClientID %>').is(':visible')) {
                        e.preventDefault();
                        $('#<%= btnConfirmarAdiantamento.ClientID %>').click();
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $(document).on('shown.bs.modal', '#Modal_Adiantamento', function () {
                $(this).focus();
            });
        });

        function toggleVisibility(show) {
            var grid = document.getElementById('simulacaoGrid');
            var title = document.getElementById('tituloResumo');
            if (show) {
                grid.style.display = 'block';  // Mostra o grid e o título
                title.style.display = 'block'; // Certifique-se de que o título também seja mostrado
            } else {
                grid.style.display = 'none';   // Oculta o grid e o título
                title.style.display = 'none';  // Certifique-se de que o título também seja ocultado
            }
        }

        $v192 = jQuery.noConflict();
    </script>

</asp:Content>


