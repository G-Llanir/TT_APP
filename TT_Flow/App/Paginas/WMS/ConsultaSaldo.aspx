<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="ConsultaSaldo.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.Manutencao.ConsultaSaldo" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/FiltroPesquisa.ascx" TagPrefix="uc1" TagName="FiltroPesquisa" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        div#cphCorpo_dtgvConsulta_filter {
            margin-right: 20px;
        }

        .card {
            box-shadow: 0 4px 8px 0 rgba(0,0,0,0.2);
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
        }

        .modal-title {
            text-shadow: 1px 2px 3px rgba(0, 0, 0, 0.2);
            font-size: 20px;
            font-weight: bold;
            margin: 0;
        }

        .modal-largo {
            width: 80% !important;
            max-width: none !important;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <script type="text/javascript">
                function MudarBotao() {
                    document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
                }
            </script>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label><small> Manutenção</small></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>
                <br />
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa </h3>
                        </div>
                        <div class="panel-body">

                            <div class="col-lg-12">
                                <div class="row form-group">
                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <asp:DropDownList ID="ddlSituacaoCadastral" runat="server" class="form-control Caixa_Selecao" OnSelectedIndexChanged="ddlSituacaoCadastral_SelectedIndexChanged" AutoPostBack="true">
                                                <asp:ListItem Text="Saldo de Produtos" Value="" Selected="True" />
                                                <asp:ListItem Text="Produtos Importados e não classificados" Value="N" />
                                                <asp:ListItem Text="Sugestão de Compras" Value="MINIMO" />
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <asp:TextBox ID="txtPesquisa" class="form-control" runat="server" placeholder="Pesquisar"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <asp:DropDownList ID="ddlTipoProduto" runat="server" class="form-control Caixa_Selecao" attrname="Tipo"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div runat="server" id="div_Familia" class="col-lg-2">
                                        <div class="form-group">
                                            <asp:DropDownList ID="ddlFamilia" runat="server" class="form-control Caixa_Selecao" attrname="Tipo"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-12">
                                <div class="row form-group">
                                    <div runat="server" id="div_Pais" class="col-lg-2">
                                        <div class="form-group">
                                            <label>&nbsp;</label>
                                            <asp:DropDownList ID="ddlPaisOrigem" runat="server" class="form-control Caixa_Selecao" attrname="Tipo"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-3">
                                        <div class="form-group">
                                            <label>Local de Armazenamento</label>
                                            <asp:ListBox ID="lstLocalArmazenamento" class="form-control Caixa_Selecao" SelectionMode="Multiple" runat="server"></asp:ListBox>
                                        </div>
                                    </div>

                                    <div runat="server" id="div_Grupo" class="col-lg-2">
                                        <div class="form-group">
                                            <label>&nbsp;</label>
                                            <asp:DropDownList ID="ddlGrupo" runat="server" class="form-control Caixa_Selecao" attrname="Tipo"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-2" style="display: flex; padding: initial;">
                                        <div class="form-group">
                                            <label>Apenas produtos com Saldo Mínimo</label>
                                            <asp:CheckBox ID="ckbSaldoMinimo" class="form-control" runat="server" Style="width: fit-content; padding: revert-layer; display: flow; justify-self: center;"></asp:CheckBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-3" style="margin-top: 24px">
                                        <div class="form-group">
                                            <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                            <asp:Button ID="cmdNovoCadastro" class="btn btn-success" runat="server" Text="Importar Saldo" OnClick="cmdNovoCadastro_Click" />
                                            <asp:Button ID="cmdEditar" class="btn btn-warning" runat="server" Text="Editar" OnClick="cmdEditar_Click" />
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
                <div class="col-lg-12">
                    <asp:Panel ID="pnMensagem" runat="server">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    </asp:Panel>
                    <asp:Panel ID="pnResultado" runat="server">
                        <div class="panel panel-primary">
                            <div class="panel-body ">
                                <asp:GridView ID="dtgvConsulta" runat="server" class="table table-condensed table-striped table-bordered table-hover" GridLines="None"
                                    CellSpacing="1" CellPadding="1" Width="100%" ShowFooter="False" Font-Names="Tahoma"
                                    Font-Overline="False" Font-Size="Small" AutoGenerateColumns="false" OnRowDataBound="dtgvConsulta_RowDataBound">
                                    <Columns>

                                        <asp:BoundField DataField="idItem" HeaderText="ID">
                                            <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                            DataTextField="sCodigo" HeaderText="Código"
                                            DataNavigateUrlFormatString="~/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}" Target="_blank">
                                            <ItemStyle Width="17%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link" />
                                        </asp:HyperLinkField>

                                        <asp:TemplateField HeaderText="Descrição do Produto">
                                            <ItemTemplate>
                                                <div>
                                                    <asp:LinkButton ID="lnkProdutoDetalhe" runat="server" Text='<%# Eval("sDscProduto") %>'
                                                        OnClientClick='<%# "openModal(\"" + Eval("idItem") + "\"); return false;" %>'
                                                        OnMouseOver='<%# "mostraCard(this, \"" + Eval("idItem") + "\", \"produto\");" %>'
                                                        OnMouseOut='<%# "escondeCard( \"" + Eval("idItem") + "\", \"produto\");" %>'
                                                        CssClass="produto-detalhe-link"
                                                        ClientIDMode="Static"
                                                        data-idproduto='<%# Eval("idItem") %>'
                                                        data-tabela="produto" />

                                                    <div id='<%# Eval("idItem") + "_produto" %>' class="product-card" style="display: none;">
                                                        <!--conteudo via script -->
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="sFabricante" HeaderText="Fabricante">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscLocalArmazenamento" HeaderText="Local de Armazenamento">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscGrupoProduto" HeaderText="Grupo">
                                            <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscFamiliaProduto" HeaderText="Família">
                                            <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="nEstoqueMinimo" HeaderText="Mínimo" DataFormatString="{0:N4}">
                                            <ItemStyle Width="8%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:TemplateField HeaderText="Atualizar Estoque">
                                            <ItemTemplate>
                                                <asp:TextBox ID="nEstoqueAtual" runat="server" class="form-control" MaxLength="20" Text='<%# Bind("NEstoqueAtual","{0:N4}") %>'></asp:TextBox>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="nEstoqueAtualVisualizacao" HeaderText="Estoque Atual" DataFormatString="{0:N4}">
                                            <ItemStyle Width="8%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sUnidade" HeaderText="Unidade">
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="nSugestaoCompra" HeaderText="Sugestão Compra" DataFormatString="{0:N4}">
                                            <ItemStyle Width="8%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </asp:Panel>

                    <fieldset class="form-stacked actions" id="idCampoSalvar" runat="server">
                        <asp:Button ID="cmdAtualizarSaldo" class="btn  btn-lg btn-success" runat="server" Text="Atualizar Estoque" OnClick="cmdAtualizarSaldo_Click" />
                        &nbsp;
                        <asp:Button ID="cmdCancelar" class="btn  btn-lg btn-warning" runat="server" Text="Cancelar" OnClick="cmdCancelar_Click" />&nbsp;
                    </fieldset>

                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script>
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
    </script>

</asp:Content>
