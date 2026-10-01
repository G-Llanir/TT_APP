<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Controle_Metas.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Manutencao.Controle_Metas" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>

<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>




<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .gvChildHeader {
            padding: 4px;
            background-color: rgba(0, 155, 0) !important;
            color: white !important;
            border: 1px solid #bbb;
            font-weight: bold;
        }

        .gvChildHeader2 {
            padding: 4px;
            background-color: rgba(0, 180, 0) !important;
            color: white !important;
            border: 1px solid #bbb;
            font-weight: bold;
        }

        .collapsed-row {
            display: none;
            padding: 1px;
            margin: 1px;
        }

        .gvMainTh {
            padding: 4px;
            background-color: rgba(0, 125, 0) !important;
            color: white !important;
            border: 1px solid #bbb;
            font-weight: bold;
        }

        .gvMainTd {
            padding: 7px;
            border: 1px solid #bbb;
            background-color: #ffffff;
        }

            .gvMainTd tbody tr:hover td {
                background-color: #e0e0e0;
            }

        .table {
            margin-bottom: 0px !important;
        }

        .modal-dialog {
            width: 60%;
        }

        .modal-content {
            display: flex;
            flex-direction: column;
            height: auto;
            overflow-y: inherit !important;
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
    </style>

</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>


    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Controle Metas"></asp:Label>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="2" TitulodaPagina="Controle de Metas" />
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>
    </div>

    <div>
        <ul id="tab_Metas" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="tabpanel active" runat="server" id="aba_Meta">
                <a href="#Metas" id="Meta-tab" role="tab" data-toggle="tab" aria-controls="home" aria-expanded="false" data-show-button="true"><b>Financeiro</b></a>
            </li>

            <li role="presentation" runat="server" id="aba_Comercial">
                <a href="#comercial" role="tab" id="comercial-tab" data-toggle="tab" aria-controls="comercial" data-show-button="false"><b>Comercial</b></a>
            </li>

        </ul>
    </div>

    <div id="tab" class="tab-content">

        <div role="tabpanel" class="tab-pane fade in active" id="Metas" aria-labelledby="Metas">
            <br />
            <div class="panel panel-default" runat="server" id="div_Metas">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Meta Financeiro</b></h3>
                </div>
                <div class="panel-body table-responsive">
                    <asp:UpdatePanel ID="updDepartamentos" runat="server">
                        <ContentTemplate>


                            <uc1:MensagemPagina runat="server" ID="MensagemAcoes" />
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <div class="row" runat="server" id="Div_Selecao">

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Ano </label>
                                                    <asp:TextBox ID="txtnAno" runat="server" class=" form-control" ValidationGroup="Inclusao">  </asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-4">
                                                <div class="form-group">
                                                    <label>Mês </label>
                                                    <asp:DropDownList ID="ddlidMes" runat="server" class="form-control" ValidationGroup="Inclusao"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Valor </label>
                                                    <asp:TextBox ID="txtnValor" runat="server" class=" form-control" ValidationGroup="Inclusao">  </asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <br />
                                                <asp:Button ID="cmdMeta_Incluir" class="btn  btn-sm btn-success" runat="server" Text="Incluir" OnClick="cmdMeta_Incluir_Click" ValidationGroup="Inclusao" Style="margin-top: 5px;" />
                                            </div>

                                        </div>

                                        <div class="row table-responsive">
                                            <div class="col-lg-12">
                                                <asp:GridView ID="dtgMeta" runat="server" Width="100%" class="table table-striped table-bordered table-hover tablePai"
                                                    AutoGenerateColumns="False" GridLines="None" ShowFooter="False"
                                                    Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDeleting="dtgMeta_RowDeleting" OnRowDataBound="dtgMeta_RowDataBound">
                                                    <Columns>

                                                        <asp:BoundField DataField="idMeta" HeaderText="idMeta">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="idMes" HeaderText="idMes">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="nAno" HeaderText="Ano">
                                                            <ItemStyle Width="16%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sDscMes" HeaderText="Mês">
                                                            <ItemStyle Width="16%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:TemplateField HeaderText="Valor Meta" runat="server">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="txtnValor" class="form-control " Text='<%# Bind("nValor","{0:N2}") %>' runat="server" ValidationGroup="Salvar"></asp:TextBox>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkMeta_Excluir" runat="server" CssClass="btn btn-small" TabIndex="50" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                    </Columns>
                                                    <PagerStyle BackColor="Black" ForeColor="Black" HorizontalAlign="Center" />
                                                    <EditRowStyle BackColor="#2461BF" />
                                                </asp:GridView>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>


                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cmdMeta_Incluir" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>

        </div>

        <div role="tabpanel" class="tab-pane fade" id="comercial" aria-labelledby="comercial">
            <br />
            <div class="panel panel-default" runat="server" id="DIV_Vendedores">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Meta Vendedores</b></h3>
                </div>

                <div class="panel-body">

                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <uc1:MensagemPagina runat="server" ID="MensagemPagina_AbaComercial" />
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <div class="row">

                                            <div class="col-lg-3">
                                                <div class="form-group">
                                                    <label>Empresa</label>
                                                    <asp:DropDownList ID="ddlidEmpresaFiltro" runat="server" class="form-control  Caixa_Selecao"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Ano</label>
                                                    <asp:TextBox ID="txtnAnoComercialFiltro" runat="server" class="form-control"></asp:TextBox>
                                                </div>
                                            </div>                                            

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Vendedor</label>
                                                    <asp:DropDownList ID="ddlidVendedorFiltro" runat="server" class="form-control  Caixa_Selecao"></asp:DropDownList>
                                                </div>
                                            </div>                                            

                                            <div class="col-lg-2">
                                                <div class="form-group">                                                    
                                                    <asp:Button ID="btnMetaComercialPesquisa" class="btn btn-primary" runat="server" Text="Pesquisar" OnClick="btnMetaComercialPesquisa_Click" style="margin-top: 25px;" />
                                                    <asp:Button ID="btnNovaMetaVendedor" class="btn btn-success" runat="server" Text="Nova Meta" OnClick="btnNovaMetaVendedor_Click" style="margin-top: 25px;" />
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row table-responsive">
                                            <div class="col-lg-12">
                                                <asp:GridView ID="gv_MetaComercial" runat="server" class="table table-striped table-bordered table-hover tablePai"
                                                    AutoGenerateColumns="False" GridLines="None" ShowFooter="False"
                                                    Font-Names="Tahoma" Font-Overline="False" Font-Size="Small"
                                                    OnRowDataBound="gv_MetaComercial_RowDataBound">
                                                    <Columns>
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="btnToggle" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-div-id='<%# "tr_1_Meta" + string.Format("{0}_{1}_{2}",Eval("idEmpresa"), Eval("idMes"), Eval("nAno")) %>'></asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                        <asp:BoundField DataField="sEmpresa" HeaderText="Empresa">
                                                            <ItemStyle Width="30%" />
                                                        </asp:BoundField>

                                                        <asp:TemplateField HeaderText="Total da Meta">
                                                            <ItemTemplate>
                                                                <asp:Label ID="nTotalEmpresa" runat="server" Text='<%# Eval("nTotalEmpresa", "{0:C}") %>'></asp:Label>
                                                                <%# NovaLinha(string.Format("{0}_{1}_{2}",Eval("idEmpresa"), Eval("idMes"), Eval("nAno")), "_1_Meta") %>
                                                                <asp:GridView ID="gv_VendedorMensal" class="table table-striped table-bordered table-hover tablePai" AlternatingRowStyle-CssClass="gvAltRow"
                                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="idEmpresa, idMes, nAno"
                                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gv_VendedorMensal_RowDataBound">
                                                                    <Columns>
                                                                        <asp:TemplateField>
                                                                            <ItemTemplate>
                                                                                <asp:LinkButton ID="btnToggle2" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-div-id='<%# "tr_2_Meta" + string.Format("{0}_{1}_{2}",Eval("idEmpresa"), Eval("idMes"), Eval("nAno")) %>'></asp:LinkButton>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:BoundField DataField="MesAno" HeaderText="Mês/Ano" />
                                                                        <asp:TemplateField HeaderText="Total da Meta Mensal">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="nTotalMetaMensal" runat="server" Text='<%# Eval("nTotalMetaMensal", "{0:C}") %>'></asp:Label>
                                                                                <%# NovaLinha(string.Format("{0}_{1}_{2}",Eval("idEmpresa"), Eval("idMes"), Eval("nAno")), "_2_Meta") %>

                                                                                <asp:GridView ID="gv_VendedorDetalhe" runat="server" Width="100%"
                                                                                    GridLines="None" AutoGenerateColumns="false" class="table table-striped table-bordered table-hover tablePai"
                                                                                    Style="padding: 0; margin: 0" AlternatingRowStyle-CssClass="gvAltRow" OnRowCommand="gv_VendedorDetalhe_RowCommand" OnRowDataBound="gv_VendedorDetalhe_RowDataBound">
                                                                                    <Columns>

                                                                                        <asp:BoundField DataField="sDscVendedor" HeaderText="Vendedor">
                                                                                            <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                        </asp:BoundField>

                                                                                        <asp:BoundField DataField="nValor" HeaderText="Valor" DataFormatString="R$ {0:N2}">
                                                                                            <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                        </asp:BoundField>

                                                                                        <asp:TemplateField HeaderText="">
                                                                                            <ItemTemplate>
                                                                                                <asp:LinkButton ID="lnkEditar" runat="server" CssClass="btn btn-small" ToolTip="Editar" TabIndex="99" CommandName="EditarVendedor" CommandArgument='<%# Eval("idMeta") %>'><i class="fa fa-pencil"></i></asp:LinkButton>
                                                                                                <asp:LinkButton ID="lnkExcluir" runat="server" ToolTip="Excluir" CssClass="btn btn-small" TabIndex="100" CommandName="ExcluirVendedor" CommandArgument='<%# Eval("idMeta") %>'><i class="fa fa-trash-o" aria-hidden="true"></i></asp:LinkButton>
                                                                                            </ItemTemplate>
                                                                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                        </asp:TemplateField>

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

                                    </div>
                                </div>
                            </div>


                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
        <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
        <br />
    </div>
    <fieldset class="form-stacked actions">
        <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" ValidationGroup="Salvar" />
        <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field-cancel" title="Voltar" onclick="history.go(-2)">
    </fieldset>

    <div id="dialog-Salvar" class="modal" title="Salvar">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
        </p>
    </div>

    <asp:HiddenField ID="hddidMeta" runat="server" />
    <asp:HiddenField ID="hddAbaComercial" runat="server" Value="N" />

    <%--------------------------------------MODAL INCLUIR META COMERCIAL---------------------------------------------------------------------------------------%>
    <div class="modal fade" id="modalMetaVendedor" tabindex="-2" role="dialog" data-backdrop="static" aria-labelledby="modalMetaVendedor" aria-hidden="true">
        <div class="modal-dialog modal-sm" role="document" style="width: 60%">
            <div class="modal-content">
                <asp:UpdatePanel ID="updModalMetaVendedor" runat="server">
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
                                    <h4 class="modal-title">Meta Comercial</h4>
                                </div>
                            </div>
                        </div>
                        <div class="modal-body">
                            <div class="panel panel-default" runat="server" id="div_detalhe">

                                <div class="panel-body ">
                                    <div class="col-lg-12 row" style="width: 100%; margin-left: 0px; padding: 0px;">
                                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                            <ContentTemplate>
                                                <uc1:MensagemPagina runat="server" ID="MensagemPaginaModal" style="margin-left: 0px !important" />
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Ano</label>
                                            <asp:TextBox ID="txtnAnoComercial" runat="server" class="form-control"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>Empresa</label>
                                            <asp:DropDownList ID="ddlidEmpresa" runat="server" class="form-control  Caixa_Selecao"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>Vendedor</label>
                                            <asp:DropDownList ID="ddlidVendedor" runat="server" class="form-control  Caixa_Selecao"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-1">
                                        <div class="form-group">
                                            <asp:Button ID="btnIncluir" class="btn btn-success" runat="server" Text="Incluir" OnClick="btnIncluir_Click" style="margin-top: 25px;"/>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="panel panel-default" runat="server" id="div_gvMetaVendedor">
                                <div class="panel-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvMetaVendedor" runat="server" class="table table-striped table-bordered table-hover"
                                            Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="idMeta, idEmpresa, idVendedor, idMes"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvMetaVendedor_RowDataBound">

                                            <Columns>
                                                <asp:BoundField DataField="idMeta" HeaderText="Meta" visible="false"/>

                                                <asp:BoundField DataField="idEmpresa" HeaderText="id Empresa" visible="false"/>

                                                <asp:BoundField DataField="sDscEmpresa" HeaderText="Empresa" />

                                                <asp:BoundField DataField="idVendedor" HeaderText="id Vendedor" visible="false"/>

                                                <asp:BoundField DataField="sDscVendedor" HeaderText="Vendedor" />

                                                <asp:BoundField DataField="nAno" HeaderText="Ano" />

                                                <asp:BoundField DataField="sDscMes" HeaderText="Mês" />

                                                <asp:BoundField DataField="idMes" HeaderText="idMes" visible="false"/>

                                                <asp:TemplateField HeaderText="Valor">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtnValorComercial" runat="server"  class="form-control" Text='<%# Bind("nValor") %>'></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>


                        </div>

                        <div class="modal-footer">
                            <asp:Button ID="btnSalvar" CssClass="btn-success btn" runat="server" Text="Salvar" OnClick="btnSalvar_Click" />
                            <button id="btnFecharFooterDetalhe" type="button" class="btn btn-secondary" data-dismiss="modal">Fechar</button>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <script type="text/javascript">        
        $(document).ready(function () {
            function updateButtonVisibility() {
                var activeTab = $('ul#tab_Metas li.active a');
                var showButton = activeTab.data("show-button");

                if (showButton) {
                    $('#<%=cmdSalvar.ClientID %>').show();
                } else {
                    $('#<%=cmdSalvar.ClientID %>').hide();
                }
            }

            $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                updateButtonVisibility();
            });

            updateButtonVisibility();

            $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                var abaAtiva = $(e.target).attr('href');

                var parametroAba = '';
                if (abaAtiva === '#comercial') {
                    parametroAba = 'comercial';
                } else if (abaAtiva === '#Metas') {
                    parametroAba = 'financeiro';
                }

                if (parametroAba) {
                    var novaUrl = window.location.href.split('?')[0] + '?aba=' + parametroAba;
                    history.replaceState(null, '', novaUrl);
                }
            });

            const urlParams = new URLSearchParams(window.location.search);
            const aba = urlParams.get('aba');

            if (aba === 'comercial') {
                $('#comercial-tab').tab('show'); 
            } else if (aba === 'financeiro') {
                $('#Meta-tab').tab('show'); 
            }
            
        });


        recarregaEvento();

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            recarregaEvento();
        });

        function recarregaEvento() {

            document.getElementById('btnFecharFooterDetalhe').addEventListener('click', function () {
                refreshPagina('#modalMetaVendedor');
            });

            document.getElementById('btnHeadFecharDetalhe').addEventListener('click', function () {
                refreshPagina('#modalMetaVendedor');
            });
        }

        function refreshPagina(idModal) {          

            $(idModal).on('hidden.bs.modal', function () {
                var url = '/App/Paginas/Adm/Manutencao/Controle_Metas.aspx?aba=comercial';               
                window.location.href = url;

            });
            $(idModal).modal('hide');

        }
    </script>

</asp:Content>
