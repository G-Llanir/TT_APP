<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Envio_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.Envio_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmEnvio_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>
    <style>
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
        /* CSS TXTBOXTOGETHER*/
        .input-with-symbol {
            position: relative;
        }

            .input-with-symbol .symbol {
                position: absolute;
                right: 10px;
                top: 50%;
                transform: translateY(-50%);
                font-weight: bold;
                color: #333;
            }

            .input-with-symbol input {
                padding-right: 40px; /* Espaço para o símbolo à direita */
            }
    </style>
    <div class="form-stacked">
        <div class="col-lg-12 row">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Envio"></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
        </div>

        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />


        <div>
            <ul id="tab_Envio" class="nav nav-tabs" role="tablist">
                <li role="presentation" class="tabpanel active">
                    <a href="#Envio" id="aba_Envio" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Envio</b></a>
                </li>
            </ul>
        </div>

        <div id="tab" class="tab-content">
            <div role="tabpanel" class="tab-pane fade in active" id="Envio" aria-labelledby="Envio-tab">

                <asp:UpdatePanel ID="updDetalhe" runat="server">
                    <ContentTemplate>
                        <br />


                        <div class="panel panel-default" runat="server" id="div_Envio">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Dados do Envio</b></h3>
                            </div>
                            <div class="panel-body">
                                <div class="col-lg-12">
                                    <div class="form-stacked row">
                                        <div class="row">
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>ID</label>
                                                    <asp:TextBox ID="txtidEnvio" class="form-control CaixaTextoMini" runat="server" disabled="true"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label for="txtDtEnvio">Data do Envio:</label>
                                                    <asp:TextBox ID="txtDtEnvio" runat="server" CssClass="form-control" type="date" disabled="true"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="form-stacked row">
                                        <div class="row">
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Cliente</label>
                                                    <asp:TextBox ID="txtsCliente" runat="server" CssClass="form-control" disabled="true"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-4">
                                                <div class="form-group">
                                                     <label>Descrição da OPI</label>
                                                    <asp:TextBox ID="txtsDscOPI" runat="server" CssClass="form-control" disabled="true"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="form-stacked row">
                                        <div class="row">
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label for="txtNQtdVolumes">Quantidade de Volumes:</label>
                                                    <div class="input-with-symbol">
                                                        <asp:TextBox ID="txtNQtdVolumes" runat="server" CssClass="form-control" placeholder="Quantidade de Volumes" disabled="true"></asp:TextBox>
                                                        <span class="symbol">qtd</span>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label for="txtNPesoLiquido">Peso Líquido:</label>
                                                    <div class="input-with-symbol">
                                                        <asp:TextBox ID="txtNPesoLiquido" runat="server" CssClass="form-control" placeholder="Peso Líquido" disabled="true"></asp:TextBox>
                                                        <span class="symbol">kg</span>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label for="txtNPesoBruto">Peso Bruto:</label>
                                                    <div class="input-with-symbol">
                                                        <asp:TextBox ID="txtNPesoBruto" runat="server" CssClass="form-control" placeholder="Peso Bruto" disabled="true"></asp:TextBox>
                                                        <span class="symbol">kg</span>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label for="txtNComprimento">Comprimento:</label>
                                                    <div class="input-with-symbol">
                                                        <asp:TextBox ID="txtNComprimento" runat="server" CssClass="form-control" placeholder="Comprimento" disabled="true"></asp:TextBox>
                                                        <span class="symbol">cm</span>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label for="txtNLargura">Largura:</label>
                                                    <div class="input-with-symbol">
                                                        <asp:TextBox ID="txtNLargura" runat="server" CssClass="form-control" placeholder="Largura" disabled="true"></asp:TextBox>
                                                        <span class="symbol">cm</span>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label for="txtNAltura">Altura:</label>
                                                    <div class="input-with-symbol">
                                                        <asp:TextBox ID="txtNAltura" runat="server" CssClass="form-control" placeholder="Altura" disabled="true"></asp:TextBox>
                                                        <span class="symbol">cm</span>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label for="txtNDimensoes">Volume m³:</label>
                                                    <div class="input-with-symbol">
                                                        <asp:TextBox ID="txtNDimensoes" runat="server" CssClass="form-control" placeholder="Volume³" disabled="true"></asp:TextBox>
                                                        <span class="symbol">m³</span>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>


                            </div>
                        </div>

                        <div class="panel panel-default" runat="server" id="Div_Envios">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Itens Enviados</b></h3>
                            </div>
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <uc1:MensagemPagina runat="server" ID="MensagemPaginaAbaEnvios" />
                                    </div>
                                </div>

                                <br />
                                <div class="col-lg-12">
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
                                                        <itemstyle width="10%" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="idVolume" HeaderText="ID">
                                                    <ItemStyle Width="10%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="sDscVolume" HeaderText="Volume">
                                                    <ItemStyle Width="50%" />
                                                </asp:BoundField>
                                              
                                                <asp:BoundField DataField="sEmbalagem" HeaderText="Embalagem">
                                                    <ItemStyle Width="20%" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Ação">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="cmdAbrirArquivos" runat="server" OnClick="AbrirArquivosVolumes_Click" CommandArgument='<%# Eval("idVolume") %>'>
                                                           <i class="fa fa-upload"></i>
                                                        </asp:LinkButton>

                                                        <%-- <%# Eval("sEmbalagem") %>--%>
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
                                                                <asp:BoundField DataField="IdVolumeItem" HeaderText="Nº Volume" />
                                                                <asp:BoundField DataField="SDscObjeto" HeaderText="Descrição" />
                                                                 <asp:BoundField DataField="SControlaGarantia" HeaderText="Garantia" />
                                                                 <asp:BoundField DataField="NQuantidade" HeaderText="Quantidade" />
                                                                <asp:TemplateField HeaderText="Código">
                                                                    <ItemTemplate>
                                                                        <%# Eval("sCodigoBarras") %>
                                                                        <%# NovaLinha(Eval("idVolumeItem"), "Child") %>

                                                                        <asp:GridView ID="dtgItensVL" runat="server" Width="100%"
                                                                            GridLines="None" AutoGenerateColumns="false" DataKeyNames="idUnitizadoItem"
                                                                            HeaderStyle-CssClass="gvChildHeader" CssClass="gvRow" Style="padding: 0; margin: 0"
                                                                            AlternatingRowStyle-CssClass="gvAltRow">
                                                                            <Columns>
                                                                                <%--<asp:BoundField DataField="idProdutoEmbalagem" HeaderText="Embalagem" />--%>
                                                                                <asp:BoundField DataField="SCodigo" HeaderText="Código Produto" />
                                                                                <asp:BoundField DataField="SdscProduto" HeaderText="Produto" />
                                                                                <asp:BoundField DataField="NQuantidade" HeaderText="Quantidade" />
                                                                                <asp:BoundField DataField="SControlaGarantiaLote" HeaderText="Garantia" />
                                                                                <asp:BoundField DataField="SCodigoBarras" HeaderText="Código Barras" />
                                                                            </Columns>

                                                                        </asp:GridView>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="10%" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>

                            </div>
                        </div>

                        <div class="modal fade" id="modalEnvio" tabindex="-1" role="dialog" aria-labelledby="modalEnvioLabel" aria-hidden="true" data-backdrop="static">
                            <asp:UpdatePanel ID="updModal" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="modal-dialog modal-lg" role="document">
                                        <div class="modal-content">
                                            <div class="modal-header">
                                                <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                                                    <span aria-hidden="true">&times;</span>
                                                </button>
                                                <h1 class="modal-title" id="lblModalEnvio">
                                                    <asp:Label ID="lbltituloModal" Text="Arquivos de Volumes" runat="server" />
                                                </h1>
                                            </div>
                                            <div class="modal-body">
                                                <div class=" embed-responsive embed-responsive-16by9" style="min-height: 200px">
                                                    <embed type="text/html" runat="server" id="frmArquivos2" width="800" height="500" />
                                                </div>
                                            </div>
                                            <div class="modal-footer">
                                                <button type="button" class="btn btn-secondary" data-dismiss="modal">Fechar</button>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                <br />
            </div>
            <script type="text/javascript">
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


                function formatNumberWithSeparators() {
                    // IDs dos controles ASP.NET com a quantidade de casas decimais que cada um deve ter
                    var campos = [
                        { id: '<%= txtNComprimento.ClientID %>', casas: 2 },
                { id: '<%= txtNLargura.ClientID %>', casas: 2 },
                { id: '<%= txtNAltura.ClientID %>', casas: 2 },
                        { id: '<%= txtNDimensoes.ClientID %>', casas: 4 } // Adicionei o Volume (m³) com 4 casas!
                    ];

                    campos.forEach(function (campo) {
                        var input = document.getElementById(campo.id);
                        if (input && input.value !== "") {

                            // Se o valor já tiver vírgula (padrão BR), removemos os pontos de milhar e trocamos a vírgula por ponto pro JS entender.
                            // Se não tiver vírgula, assumimos que já veio do banco no formato americano com ponto (ex: 2.32)
                            var valorString = input.value.indexOf(',') > -1
                                ? input.value.replace(/\./g, '').replace(',', '.')
                                : input.value;

                            var value = parseFloat(valorString);

                            if (!isNaN(value)) {
                                // MUDANÇA AQUI: Removido o Math.floor()
                                // Formata para o padrão Brasileiro (pt-BR) travando a quantidade de casas decimais
                                input.value = value.toLocaleString('pt-BR', {
                                    minimumFractionDigits: campo.casas,
                                    maximumFractionDigits: campo.casas
                                });
                            }
                        }
                    });
                }

                function initializeScripts() {
                    RegistrarColapsoScript();
                    formatNumberWithSeparators(); // Chama a função para formatar números com separadores
                }

                // Executa ao carregar a página
                document.addEventListener('DOMContentLoaded', function () {
                    initializeScripts(); // Inicializa scripts ao carregar a página
                });

                // Executa após o postback via UpdatePanel
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    initializeScripts(); // Inicializa scripts após o postback
                });
            </script>

        </div>

        <%----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>


        <fieldset class="form-stacked actions">
            <asp:Button ID="ExportarExcel" CssClass="btn btn-lg  btn-success" runat="server" Text="Excel Envio" OnClick="GerarExcelEnvio_Click" />
            <asp:Button ID="cmdVoltar" class="btn  btn-lg btn-warning" runat="server" Text="Voltar" OnClick="Voltar_Click" />
        </fieldset>

    </div>


    <asp:HiddenField ID="hddidEnvio" runat="server" />
    <asp:HiddenField ID="hddidOPI" runat="server" />
</asp:Content>
