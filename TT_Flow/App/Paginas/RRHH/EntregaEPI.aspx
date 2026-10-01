<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="EntregaEPI.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.EntregaEPI" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        .invisivel {
            display: none;
        }

        .label {
            font-size: 15px;
            font-weight: bold;
        }

        .card {
            box-shadow: 0px 2px 8px 2px rgba(0,0,0,0.3);
            transition: 0.3s;
            width: 100%;
            border-radius: 5px;
        }

        .card-body {
            padding: 10px;
            display: flex;
            align-items: flex-start;
        }

        .flex-grow-1 {
            flex-grow: 1;
            margin-left: 15px;
        }

        .img-thumbnail {
            border: none;
            width: 100px;
            height: auto;
        }

        .me-3 {
            margin-right: 1rem;
            padding: 2px;
            border-bottom: 1px dashed black;
        }

        .success {
            color: #5cb85c !important;
        }

        .warning {
            color: #f0ad4e !important;
        }

        .danger {
            color: #d9534f !important;
        }

        .default {
            color: #777 !important;
        }
    </style>

    <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa</h3>
                        </div>
                        <div class="panel-body">

                            <div id="datas_EPIs" class="row">

                                <div class="col-lg-2 form-group">
                                    <asp:DropDownList ID="ddlFiltraDatas" runat="server" class="form-control">
                                        <asp:ListItem Value="S" Text="Data da Solicitação"></asp:ListItem>
                                        <asp:ListItem Value="E" Text="Data da Entrega"></asp:ListItem>
                                        <asp:ListItem Value="C" Text="Data da Confirmação"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-lg-4 form-group" style="display: flex; flex-wrap: nowrap; gap: 7.5px; justify-content: center; align-items: center;">
                                    <asp:TextBox ID="txtdtInicial" class="form-control " TextMode="Date" runat="server" MaxLength="10"></asp:TextBox>
                                    <asp:LinkButton runat="server" ID="cmdRepeteDatas" data-toggle="tooltip" title="Repetir Data" OnClick="cmdRepeteDatas_Click"><i class="fa fa-arrow-right"></i> </asp:LinkButton>
                                    <asp:TextBox ID="txtdtFinal" class="form-control " TextMode="Date" runat="server" MaxLength="10"></asp:TextBox>
                                </div>

                                <div class="col-lg-6 form-group">
                                    <asp:DropDownList ID="ddlEPIs" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>

                            </div>

                            <div class="row">

                                <div class="col-lg-3 form-group">
                                    <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server" MaxLength="200"></asp:TextBox>
                                </div>

                                <div class="col-lg-3 form-group">
                                    <asp:DropDownList ID="ddlColaborador" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>

                                <div class="col-lg-3 form-group">
                                    <asp:DropDownList ID="ddlStatus" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>

                                <div class="col-lg-3 form-group">
                                    <asp:DropDownList ID="ddlTipo" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>

                            </div>

                            <div class="row">

                                <div class="col-lg-2 form-group">
                                    <asp:DropDownList ID="ddlSituacao" runat="server" class="form-control">
                                        <asp:ListItem Value="" Text="Todas as Situações"></asp:ListItem>
                                        <asp:ListItem Value="S" Text="Ativas"></asp:ListItem>
                                        <asp:ListItem Value="N" Text="Inativas"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-lg-4 form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                    <asp:HyperLink ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" Target="_blank" NavigateUrl="/App/Paginas/RRHH/EntregaEPI_Detalhe.aspx?id=0"></asp:HyperLink>
                                    <asp:Button ID="cmdEPIs" class="btn btn-info" runat="server" Text="Controle de EPIs" OnClick="cmdEPIs_Click" />
                                </div>

                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <asp:Panel ID="pnResultado" runat="server">
                <div id="resultado">
                    <div class="panel panel-primary">
                        <div class="panel-body">
                            <div class="table-responsive">

                                <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">
                                    <Columns>

                                        <asp:HyperLinkField DataNavigateUrlFields="idEntregaEPI"
                                            DataTextField="idEntregaEPI" HeaderText="ID"
                                            DataNavigateUrlFormatString="EntregaEPI_Detalhe.aspx?id={0}">
                                            <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idEntregaEPI"
                                            DataTextField="dtSolicitacao" HeaderText="Solicitação"
                                            DataNavigateUrlFormatString="EntregaEPI_Detalhe.aspx?id={0}">
                                            <ItemStyle Width="9%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idEntregaEPI"
                                            DataTextField="sDscColaborador" HeaderText="Colaborador"
                                            DataNavigateUrlFormatString="EntregaEPI_Detalhe.aspx?id={0}">
                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idEntregaEPI"
                                            DataTextField="sDscTipoEntrega" HeaderText="Tipo de Entrega"
                                            DataNavigateUrlFormatString="EntregaEPI_Detalhe.aspx?id={0}">
                                            <ItemStyle Width="14%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idEntregaEPI"
                                            DataTextField="sDscStatus" HeaderText="Status"
                                            DataNavigateUrlFormatString="EntregaEPI_Detalhe.aspx?id={0}">
                                            <ItemStyle Width="16%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idEntregaEPI"
                                            DataTextField="dtEntrega" HeaderText="Entrega"
                                            DataNavigateUrlFormatString="EntregaEPI_Detalhe.aspx?id={0}">
                                            <ItemStyle Width="9%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idEntregaEPI"
                                            DataTextField="dtConfirmacao" HeaderText="Confirmação"
                                            DataNavigateUrlFormatString="EntregaEPI_Detalhe.aspx?id={0}">
                                            <ItemStyle Width="9%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:BoundField DataField="dtAtualizacao" HeaderText="Atualizado em">
                                            <ItemStyle Width="11%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                            <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="idColaborador" HeaderText="ID">
                                            <HeaderStyle CssClass="invisivel" />
                                            <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="invisivel" />
                                        </asp:BoundField>

                                    </Columns>
                                </asp:GridView>

                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnMensagem" runat="server">
                <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
            </asp:Panel>

            <div class="modal fade" id="modalEPI">
                <div class="modal-dialog" style="width: 65%;">
                    <div class="modal-content" style="overflow: visible;">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h3 class="modal-title">Controle de EPIs</h3>
                        </div>
                        <div class="modal-body">

                            <div class="col-lg-12 form-group">
                                <uc1:MensagemPagina runat="server" ID="MensagemPagina_ModalEPI" />
                            </div>

                            <div class="form-group">
                                <label>Colaborador</label>
                                <asp:DropDownList runat="server" ID="ddlColaboradores" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlColaboradores_SelectedIndexChanged"></asp:DropDownList>
                            </div>

                            <div runat="server" id="div_gvEPIs" class="row">
                                <div class="col-lg-12 form-group">
                                    <label style="width: 100%;"><i class="fa fa-info-circle" data-toggle="tooltip" title="Apenas Entregas de EPI <u>confirmadas</u> serão utilizadas para validar o Status dos EPIs!"></i> </label>
                                    <span class="label label-success"><i class="fa fa-check-circle"></i> Ok</span>
                                    <span class="label label-warning"><i class="fa fa-exclamation-circle"></i> Vence em 7 Dias</span>
                                    <span class="label label-danger"><i class="fa fa-times-circle"></i> Vencido</span>
                                    <span class="label label-default"><i class="fa fa-question-circle"></i> Não recebido</span>
                                </div>

                                <div class="col-lg-12">
                                    <div class="panel panel-primary" style="margin-bottom: 0;">
                                        <div class="panel-body table-responsive">
                                            <asp:GridView ID="gvEPIs" class="table table-striped table-hover dataTable no-footer"
                                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvEPIs_RowDataBound">
                                                <Columns>

                                                    <asp:HyperLinkField DataNavigateUrlFields="idEPI"
                                                        DataTextField="sCodigoEPI" HeaderText="Código" Target="_blank"
                                                        DataNavigateUrlFormatString="/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                        <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:HyperLinkField>

                                                    <asp:TemplateField HeaderText="Descrição do EPI">
                                                        <ItemTemplate>
                                                            <div>
                                                                <asp:LinkButton ID="lnkProdutoDetalhe" runat="server" Text='<%# Eval("sDscEPI") %>'
                                                                    OnClientClick='<%# "openModal(\"" + Eval("idEPI") + "\"); return false;" %>'
                                                                    OnMouseOver='<%# "mostraCard(this, \"" + Eval("idEPI") + "\", \"produto\");" %>'
                                                                    OnMouseOut='<%# "escondeCard( \"" + Eval("idEPI") + "\", \"produto\");" %>'
                                                                    CssClass="epi-detalhe-link"
                                                                    ClientIDMode="Static"
                                                                    data-idproduto='<%# Eval("idEPI") %>'
                                                                    data-tabela="epi" />

                                                                <div id='<%# Eval("idEPI") + "_produto" %>' class="product-card" style="display: none;">
                                                                    <!--conteudo via script -->
                                                                </div>
                                                            </div>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="40%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:TemplateField>

                                                    <asp:BoundField DataField="nCA" HeaderText="CA">
                                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sStatus" HeaderText="Status">
                                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade">
                                                        <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="dtVencimento" HeaderText="Data do Vencimento">
                                                        <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

                                    <fieldset class="form-stacked actions" style="margin-bottom: 0;">
                                        <asp:HyperLink runat="server" ID="cmdConferencia" class="btn btn-md btn-info" Text="Nova Conferência" Target="_blank" />
                                        <asp:Button runat="server" ID="cmdFechar_Modal" class="btn btn-md btn-warning" Text="Voltar" data-dismiss="modal" />
                                    </fieldset>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <div class="modal fade" id="produtoDetalheModal">
                <div class="modal-dialog" style="width: 35%;">
                    <div class="modal-content" style="display: table; width: 100%;">
                        <div class="modal-header" id="modalInfo">
                        </div>
                        <div class="modal-body" id="modalBody">
                            <!-- conteudo via javascript-->
                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="cmdPesquisar" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="cmdEPIs" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="cmdRepeteDatas" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ddlColaboradores" EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>
