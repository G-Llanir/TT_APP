<%@ Page MasterPageFile="~/App/main.master" Language="C#" AutoEventWireup="true" CodeBehind="RelatorioMovimentacao.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.RelatorioMovimentacao" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>





<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <script type="text/javascript">
        function MudarBotao() {
            document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
        }
    </script>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Movimentação"></asp:Label><small> Relatório de Movimentação</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="Relatório de Movimentação" />
        </div>
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa </h3>
                </div>
                <div class="panel-body ">
                    <div class="form-group">
                        <div class="row">
                            <div class="col-lg-2">
                                <div class="form-group">
                                    <label>Inicial</label>
                                    <asp:TextBox ID="txtdtFiltro" class="form-control " type="date" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-2">
                                <label>Final</label>
                                <div class="form-group">
                                    <asp:TextBox ID="txtdtFinal" class="form-control " type="date" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-2">
                                <label>Familia</label>
                                <asp:DropDownList ID="ddlFamilia" runat="server" class="form-control  yes_no select Caixa_Selecao">
                                </asp:DropDownList>
                            </div>

                            <div class="col-lg-2">
                                <label>Tipo do Produto</label>
                                <asp:DropDownList ID="ddlTipo" runat="server" class="form-control  yes_no select Caixa_Selecao">
                                </asp:DropDownList>
                            </div>

                            <div class="col-lg-2">
                                <label>Grupo</label>
                                <asp:DropDownList ID="ddlGrupo" runat="server" class="form-control  yes_no select Caixa_Selecao">
                                </asp:DropDownList>
                            </div>
                            <div class="col-lg-2">
                                <label>Local</label>
                                <asp:DropDownList ID="ddlLocal" runat="server" class="form-control  yes_no select Caixa_Selecao">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-4">
                                <div class="form-group">
                                    <label>Produto</label>
                                    <asp:TextBox ID="txtPesquisa" class="form-control " placeholder="Pesquisar" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                    <asp:Button ID="cmdExcel" class="btn btn-info" runat="server" Text="Exportar Excel" OnClick="ExportarExcel_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    <asp:Panel ID="pnResultado" class="" runat="server">

                        <div id="resultado">
                            <div class="panel panel-primary">
                                <div class="panel-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                            <%--OnRowDataBound="dtgItens_RowDataBound"--%>

                                            <Columns>
                                                <asp:BoundField DataField="dtMovimentacao" HeaderText="Data Movimentação" />
                                                <asp:BoundField DataField="sCodigo" HeaderText="Código" />
                                                <asp:TemplateField HeaderText="Descrição">
                                                    <ItemTemplate>
                                                        <div>

                                                            <%--Thiago * 07/02/2025---------%>
                                                            <asp:LinkButton ID="lnkProdutoDetalhe" runat="server" Text='<%# Eval("sDscProduto") %>'
                                                                OnClientClick='<%# "openProductDetail(\"" + Eval("idItem") + "\"); return false;" %>'
                                                                OnMouseOver='<%# "mostraCard(this, \"" + Eval("idItem") + "\", \"ProdutoModal\");" %>'
                                                                OnMouseOut='<%# "escondeCard( \"" + Eval("idItem") + "\", \"ProdutoModal\");" %>'
                                                                CssClass="produto-detalhe-link sDscProduto"
                                                                ClientIDMode="Static"
                                                                data-idproduto='<%# Eval("idItem") %>'
                                                                data-tabela="ProdutoModal" />

                                                            <div id='<%# Eval("idItem") + "_ProdutoModal" %>' class="product-card" style="display: none;">
                                                                <!--conteudo via script -->
                                                            </div>
                                                            <%------------------------------------%>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="sUnidade" HeaderText="Unidade" />
                                                <asp:BoundField DataField="sDscFamilia" HeaderText="Família" />

                                                <asp:BoundField DataField="sLocal" HeaderText="Local" />

                                                <asp:BoundField DataField="nEstoqueAnterior" HeaderText="Estoque Anterior" />
                                                <asp:BoundField DataField="nEstoqueAtual" HeaderText="Estoque Atual" />
                                                <asp:BoundField DataField="nCompras" HeaderText="Compras" />
                                                <asp:BoundField DataField="nVendas" HeaderText="Vendas" />


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

</asp:Content>
