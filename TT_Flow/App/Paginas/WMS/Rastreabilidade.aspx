<%@ Page MasterPageFile="~/App/main.master" Language="C#" AutoEventWireup="true" CodeBehind="Rastreabilidade.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.Rastreabilidade" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%-- Registrando o controle do Leitor Quagga --%>
<%@ Register Src="~/App/Controles/LeitorQuagga.ascx" TagPrefix="uc1" TagName="LeitorQuagga" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <script type="text/javascript">
        function MudarBotao() {
            var btn = document.getElementById("<%= cmdPesquisar.ClientID %>");
            if(btn) btn.value = "Buscando...";
        }
        
        function SetFocus() {
            var input = document.getElementById("<%= txtLeitura.ClientID %>");
            if(input) input.focus();
        }
        
        // Garante o foco no input após cada UpdatePanel refresh
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function() {
            SetFocus();
        });
        window.onload = SetFocus;
    </script>
    <style>
    /* Estilos do Autocomplete */
    .ui-autocomplete {
        z-index: 10500 !important;
        max-height: 250px;
        overflow-y: auto;
        overflow-x: hidden;
        font-size: 14px;
        background-color: white;
        border: 1px solid #ccc;
        padding: 0;
    }
    
    .item-valido {
        background-color: #d4edda !important;
        color: #155724 !important;
        border-bottom: 1px solid #c3e6cb;
        font-weight: 500;
    }

    .item-valido:hover {
        background-color: #c3e6cb !important;
        cursor: pointer;
    }

    .item-invalido {
        background-color: #f8d7da !important;
        color: #721c24 !important;
        border-bottom: 1px solid #f5c6cb;
        font-weight: bold;
        cursor: not-allowed !important;
    }
</style>
    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Rastreabilidade"></asp:Label>
                <small> Consulta</small>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="Rastreabilidade de Produto" />
        </div>

        <div class="col-lg-12">
            <asp:UpdatePanel ID="updGeral" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-barcode"></i> Leitor de Código de Barras / Pesquisa</h3>
                        </div>
                        <div class="panel-body text-center">
                            
                            <div class="form-group">
                                <asp:Button ID="cmdAbrirCamera" OnClick="cmdAbrirCamera_Click" CssClass="btn btn-lg btn-success" Text="Abrir Câmera / Leitor" runat="server" />
                            </div>

                            <div id="DivBipador" runat="server" class="mb-3 text-center" visible="false">
                                <uc1:LeitorQuagga runat="server" ID="LeitorQuaggaPesquisa" STemParametros="S" />
                            </div>
                            
                            <hr />

                            <div class="row">
                                <div class="col-lg-8 col-lg-offset-2" id="divCodigoManual" runat="server">
                                    <div class="form-group text-left">
                                        <label style="font-size: 16px;">Leitura Manual / Código Lido:</label>
                                        <div class="input-group">
                                            <span class="input-group-addon"><i class="fa fa-qrcode"></i></span>
                                            <asp:TextBox ID="txtLeitura" CssClass="form-control input-lg" placeholder="Passe o leitor ou digite aqui..." runat="server" onkeydown="return VerificarEnter(event);"></asp:TextBox>
                                            <span class="input-group-btn">
                                                <asp:Button ID="cmdPesquisar" class="btn btn-primary btn-lg" runat="server" Text="Rastrear" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>

                    <asp:Panel ID="pnResultado" runat="server" Visible="false">
                        
                        <div class="alert alert-info" style="border-left: 5px solid #31708f;">
                            <div class="row">
                                <div class="col-md-8">
                                    <h3 style="margin-top: 0;">
                                        <asp:Label ID="lblDscProduto" runat="server" Font-Bold="true"></asp:Label>
                                    </h3>
                                    <p><strong>Código Interno:</strong> <asp:Label ID="lblCodigoProduto" runat="server"></asp:Label></p>
                                    <p><strong>Família:</strong> <asp:Label ID="lblFamilia" runat="server"></asp:Label> | <strong>Unidade:</strong> <asp:Label ID="lblUnidade" runat="server"></asp:Label></p>
                                </div>
                                <div class="col-md-4 text-right">
                                    <p style="font-size: 14px; margin-bottom: 5px;">Você pesquisou por:</p>
                                    <div style="background-color: #fff; padding: 10px; border-radius: 5px; display: inline-block; border: 1px dashed #31708f;">
                                        <h4 style="margin: 0; color: #d9534f; font-weight: bold;">
                                            <i class="fa fa-crosshairs"></i> <asp:Label ID="lblCodigoBipado" runat="server"></asp:Label>
                                        </h4>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h3 class="panel-title"><i class="fa fa-history"></i><b>Movimentações do Produto</b></h3>
                            </div>
                            <div class="panel-body">
                                <div class="table-responsive">
                                    <asp:GridView ID="dtgvMovimentacao" CssClass="table table-striped table-bordered table-hover"
                                        runat="server" Width="100%" AutoGenerateColumns="False" GridLines="None" Font-Names="Tahoma" Font-Size="Small">
                                        <Columns>
                                            <asp:BoundField DataField="dtMovimentacao" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                                            <asp:BoundField DataField="sTipoMov" HeaderText="Operação" />
                                            <asp:BoundField DataField="nQuantidade" HeaderText="Qtd" />
                                            <asp:BoundField DataField="sLoteEtiqueta" HeaderText="Lote" />
                                            <asp:BoundField DataField="sLocalOrigem" HeaderText="Local" />
                                            <%--<asp:BoundField DataField="sLocalDestino" HeaderText="Destino" />--%>
                                            <asp:BoundField DataField="sNotaFiscal" HeaderText="Nota Fiscal" />
                                            <asp:BoundField DataField="sUsuario" HeaderText="Usuário" />
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>

                    </asp:Panel>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <script type="text/javascript">
        function VerificarEnter(e) {
            // Se a tecla pressionada for o Enter (código 13) enviado pelo leitor de código de barras
            if (e.keyCode === 13) {
                e.preventDefault(); // Impede o envio duplo do formulário (o Postback fantasma)

                // Simula o clique no botão de pesquisa de forma controlada
                var btn = document.getElementById("<%= cmdPesquisar.ClientID %>");
                if (btn) {
                    btn.click();
                }
                return false;
            }
            return true;
        }
</script>
    <script type="text/javascript">
        function IniciarAutocompleteRastreabilidade() {
            // Pega o campo de leitura principal
            var $txtBox = $('#<%= txtLeitura.ClientID %>');

        if ($txtBox.length > 0) {
            if ($txtBox.hasClass('ui-autocomplete-input')) {
                $txtBox.autocomplete('destroy');
            }

            $txtBox.autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: "Rastreabilidade.aspx/BuscarProdutosRastreabilidadeAutocomplete",
                        type: "POST",
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        data: JSON.stringify({ termo: request.term }),
                        success: function (data) {
                            if (!data.d || data.d.length === 0) {
                                response([{ label: '⚠️ Nenhum produto encontrado!', value: 'VAZIO', busca: '' }]);
                            } else {
                                response(data.d);
                            }
                        }
                    });
                },
                minLength: 2,
                delay: 300, // Dá um delayzinho pra não brigar com o leitor de código de barras físico
                select: function (event, ui) {
                    event.preventDefault();

                    if (ui.item.value === 'VAZIO') {
                        $txtBox.val('');
                        return false;
                    }

                    $txtBox.val(ui.item.busca);

                    // Coloque um pequeno timeout para o valor fixar na textbox antes do clique
                    setTimeout(function () {
                        $('#<%= cmdPesquisar.ClientID %>').click();
                    }, 50);
                },
                focus: function (event, ui) {
                    event.preventDefault(); // Evita que ele preencha a textbox só de passar o mouse por cima
                }
            });

                // Injeta o visual bonitão com os Emojis que você tem na OPI
                $txtBox.data("ui-autocomplete")._renderItem = function (ul, item) {
                    var cssClass = (item.value === 'VAZIO') ? "item-invalido" : "item-valido";
                    var textoVisual = item.exibicao || item.label;

                    return $("<li>")
                        .addClass("ui-menu-item")
                        .append("<div class='" + cssClass + "' style='padding: 10px;'>" + textoVisual + "</div>")
                        .appendTo(ul);
                };
            }
        }

        // Garante que vai ligar o Autocomplete quando a página carregar e após os UpdatePanels (se houver)
        $(document).ready(function () { IniciarAutocompleteRastreabilidade(); });
        if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () { IniciarAutocompleteRastreabilidade(); });
        }
</script>
</asp:Content>