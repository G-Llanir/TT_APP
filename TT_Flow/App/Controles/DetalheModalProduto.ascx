<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetalheModalProduto.ascx.cs" Inherits="TT_Flow.App.Controles.DetalheModalProduto" %>

<div class="form-group">
    <!-- Modal Sugestão-->
    <style>
        /*CSS - Sugestão*/

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

        .ibox-content {
            background-color: #ffffff;
            color: inherit;
            padding: 15px 20px 20px 20px;
            border-color: #e7eaec;
            border-image: none;
            border-style: solid solid none;
            border-width: 1px 0;
        }

        .ibox-footer {
            color: inherit;
            border-top: 1px solid #e7eaec;
            font-size: 90%;
            background: #ffffff;
            padding: 10px 15px;
        }

        .removed-item {
            background-color: #f8d7da; /* cor de fundo para indicar remoção */
            /* Outros estilos de formatação ou indicadores visuais, se necessário */
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
    </style>
    <div class="modal fade" id="modalProduto" tabindex="-1" role="dialog" aria-labelledby="modalProdutoLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="modalProdutoLabel"><b>Detalhes do Produto</b></h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">

                    <asp:Repeater ID="rptItensDetalhes" runat="server" OnItemDataBound="rptItemSugerido_ItemDataBound">
                        <ItemTemplate>
                            <div class="ibox-content">
                                <div class="table-responsive">
                                    <table class="table shoping-cart-table">
                                        <tbody>
                                            <tr>
                                                <td width="90">
                                                    <div class="cart-product-imitation">
                                                        <asp:Image ID="imgProdutoPrincipal" runat="server" class="cart-product-imitation img" />
                                                    </div>

                                                    <asp:TextBox runat="server" ID="txtIdProdutoSugestao" Visible="false" Text='<%#Eval("idItem") %>'></asp:TextBox>

                                                </td>
                                                <td class="desc">
                                                    <h3>
                                                        <a class="text-navy"><%#Eval( "sDscProduto") %>
                                                        </a>
                                                    </h3>

                                                    <p class="small">
                                                        <%#Eval( "sDscProduto") %>
                                                    </p>
                                                </td>
                                                <td class="col-lg-3">
                                                    <asp:HiddenField ID="hddsCodigo" runat="server" Value='<%# Eval("sCodigo") %>' />
                                                    <asp:HiddenField ID="hddsUnidade" runat="server" Value='<%# Eval("sUnidade") %>' />
                                                    <asp:HiddenField ID="hddsDscProduto" runat="server" Value='<%# Eval("sDscProduto") %>' />
                                                    <%--  <asp:LinkButton CssClass="btn btn-danger" Text="Remover" runat="server" OnClick="Remover_Click" CommandArgument='<%# Eval("idProdutoSugestao") %>'/>--%>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>

</div>
