<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Requisicao_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Requisicao.Requisicao_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/GerenciadorMembrosModal.ascx" TagPrefix="uc1" TagName="GerenciadorMembrosModal" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>
    <script src="/app/js/vanilla-masker.js"></script>


    <div class="form-stacked row" style="">
        <div class="col-lg-12">
            <br />
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
            <div class="well-lg" runat="server" id="caixaTitulo">
                <div class="row">
                    <div class="col-lg-8">
                        <h4><span id="MainContent_lblTituloPagina">
                            <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label>

                        </span></h4>
                    </div>
                    <div class="col-lg-4 text-right">
                        <span>
                            <asp:Label ID="lblTituloStatus" class="label" runat="server" Text="TITULO_PAGINA"></asp:Label></span>
                    </div>
                </div>
            </div>
            <br />

        </div>
    </div>


    <div>
        <ul id="tab_Requisicao" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="tabpanel active">
                <a href="#requisicao" id="aba_Requisicao" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Requisição</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_Arquivos">
                <a href="#arquivos" role="tab" id="arquivos-tab" data-toggle="tab" aria-controls="Arquivos"><b>Arquivos</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_producao">
                <a href="#producao" role="tab" id="producao-tab" data-toggle="tab" aria-controls="Arquivos"><b>Produção</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_cotacao">
                <a href="#cotacao" role="tab" id="cotacao-tab" data-toggle="tab" aria-controls="Tarefas"><b>Cotações</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_Historico">
                <a href="#historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="Historico"><b>Histórico</b></a>
            </li>

        </ul>

    </div>
    <div id="tab" class="tab-content">
        <div role="tabpanel" class="tab-pane fade in active" id="requisicao" aria-labelledby="pedido-tab">

            <asp:UpdatePanel ID="updDetalhe" runat="server">
                <ContentTemplate>
                    <br />

                    <div class="panel panel-default" runat="server" id="div_Cabecalho">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Detalhe</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Tipo de Requisição </label>
                                        <asp:DropDownList ID="ddlidTipoRequisicao" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <div class="row">
                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label>Data da Requisição </label>
                                                <asp:TextBox ID="txtdtRequisicao" class="form-control CaixaTextoData" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label>Referência  </label>
                                                <asp:TextBox ID="txtsReferencia" class="form-control CaixaTextoMedio" runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Solicitante </label>
                                        <asp:DropDownList ID="ddlsSolicitante" class="form-control Caixa_Selecao" runat="server" attrname="sSolicitante"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Departamento </label>
                                        <asp:DropDownList ID="ddlidDepartamento" class="form-control Caixa_Selecao" placeholder="Departamento" runat="server" attrname="idDepartamento"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Centro de Custo </label>
                                        <asp:TextBox ID="txtsCentroCusto" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Motivo da Requisição </label>
                                        <asp:TextBox ID="txtsDscMotivoRequisicao" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Observação </label>
                                        <asp:TextBox ID="txtsObservacao" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="120px"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div id="div_projetos" runat="server" class="form-stacked row" visible="false">

                                <div class="col-lg-12">
                                    <div class="panel panel-default" runat="server" id="div_ProdutosFabricacao">
                                        <div class="panel-heading">
                                            <h3 class="panel-title"><b>Fabricações</b></h3>
                                        </div>
                                        <div class="panel-body">
                                            <asp:GridView ID="gvProdutosFabricacao" runat="server" CssClass="table table-striped table-bordered"
                                                AutoGenerateColumns="False" DataKeyNames="idFabricacao" AllowPaging="true" PageSize="10" OnRowCommand="gvProdutosFabricacao_RowCommand">
                                                <Columns>
                                                    <%-- <asp:TemplateField HeaderText="+">
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chkSelecionar" runat="server" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>--%>

                                                    <asp:BoundField DataField="idFabricacao" HeaderText="Nº Fabricação" />
                                                    <asp:BoundField DataField="sDscFabricacao" HeaderText="Fabricação" />
                                                    <asp:BoundField DataField="sDscProduto" HeaderText="Produto" />
                                                    <asp:BoundField DataField="dtAtualizacao" HeaderText="Atualização" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                                                    <asp:TemplateField HeaderText="Ação">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="cmdGerenciar" Text="Gerenciar" CssClass="btn btn-sm btn-warning" runat="server" CommandName="Gerenciar"
                                                                CommandArgument='<%# Eval("idProjeto") %>' Visible='<%#TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Fabricacao.Gerenciar, false)%>' />

                                                            <asp:LinkButton ID="LinkButton1" Text="Gerenciar" CssClass="btn btn-sm btn-warning disabled" runat="server" CommandName="Gerenciar"
                                                                CommandArgument='<%# Eval("idProjeto") %>' Visible='<%#!TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Fabricacao.Gerenciar, false)%>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

                    <asp:UpdatePanel ID="updPanel_Itens" runat="server">
                        <ContentTemplate>
                            <div class="panel panel-default" runat="server" id="Div_Itens">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>Itens</b></h3>
                                </div>
                                <div class="panel-body">
                                    <uc1:MensagemPagina runat="server" ID="MensagemItem" />
                                    <div class="row">
                                        <div class="col-lg-12">
                                            <div class="form-group">
                                                <div class="row" runat="server" id="div_SelecaoItens">

                                                    <div class="col-lg-2">
                                                        <div class="form-group">
                                                            <label>Código Produto</label>
                                                            <asp:TextBox ID="txtsCodigoProduto" class="form-control  " runat="server" MaxLength="50" ValidationGroup="Item" AutoPostBack="True" OnTextChanged="txtsCodigoProduto_TextChanged"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-4">
                                                        <div class="form-group">
                                                            <label>Descrição Produto </label>
                                                            <asp:TextBox ID="txtsDscProduto" class="form-control " runat="server" MaxLength="200" ValidationGroup="Item"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2">
                                                        <div class="row">
                                                            <div class="col-lg-5">
                                                                <div class="form-group">
                                                                    <label>Quantidade </label>
                                                                    <asp:TextBox ID="txtnQuantidade" class="form-control uppercase " runat="server" MaxLength="10" ValidationGroup="Item"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-7">
                                                                <div class="form-group">
                                                                    <label>Unidade</label>
                                                                    <asp:DropDownList ID="ddlsUnidade" runat="server" class="form-control Caixa_Selecao" attrname="Unidade" ValidationGroup="Item"></asp:DropDownList>
                                                                </div>
                                                            </div>

                                                        </div>
                                                    </div>

                                                    <div class="col-lg-1">
                                                        <div class="form-group">
                                                            <label>Previsão de Uso </label>
                                                            <asp:TextBox ID="txtdtPrevisaoUso" class="form-control CaixaTextoData" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-1">
                                                        <br />
                                                        <asp:Button ID="cmdIncluirItem" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdIncluirItem_Click" style="margin-top: 4px;" />
                                                    </div>

                                                    <div class="col-lg-3">
                                                        <br />

                                                        <asp:Label ID="lblMensagem_Item" runat="server" Text="" Visible="false" CssClass="label-danger"></asp:Label>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-lg-12">
                                                        <asp:GridView ID="dtgItens" class="table table-striped table-bordered table-hover table-condensed" DataKeyNames="sCodigo"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDeleting="dtgItens_RowDeleting" OnRowDataBound="dtgItens_RowDataBound1">
                                                            <Columns>


                                                                <asp:BoundField DataField="idRequisicao" HeaderText="idRequisicao">
                                                                    <ItemStyle Width="1%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:HyperLinkField DataNavigateUrlFields="idProduto"
                                                                    DataTextField="sCodigo" HeaderText="Código" Target="_blank"
                                                                    DataNavigateUrlFormatString="/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:HyperLinkField>

                                                                <asp:TemplateField HeaderText="Produto">
                                                                    <ItemTemplate>
                                                                        <div>
                                                                            <asp:LinkButton ID="lnkProdutoDetalhe" runat="server" Text='<%# Eval("sDscProduto") %>'
                                                                                OnClientClick='<%# "openModal(\"" + Eval("idProduto") + "\"); return false;" %>'
                                                                                OnMouseOver='<%# "mostraCard(this, \"" + Eval("idProduto") + "\", \"produto\");" %>'
                                                                                OnMouseOut='<%# "escondeCard( \"" + Eval("idProduto") + "\", \"produto\");" %>'
                                                                                CssClass="produto-detalhe-link"
                                                                                ClientIDMode="Static"
                                                                                data-idproduto='<%# Eval("idProduto") %>'
                                                                                data-tabela="produto" />

                                                                            <div id='<%# Eval("idProduto") + "_produto" %>' class="product-card" style="display: none;">
                                                                                <!--conteudo via script -->
                                                                            </div>
                                                                        </div>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="45%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:BoundField DataField="sUnidade" HeaderText="Unidade">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="Quantidade">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="txtQuantidade" runat="server"
                                                                            Text='<%# Eval("nQuantidade") %>'
                                                                            CssClass="form-control"
                                                                            Width="90px"
                                                                            MaxLength="10"
                                                                            onkeypress="return SomenteNumero(event);" />

                                                                        <asp:RequiredFieldValidator ID="rfvQuantidade" runat="server"
                                                                            ControlToValidate="txtQuantidade"
                                                                            ErrorMessage="*"
                                                                            ForeColor="Red"
                                                                            Display="Dynamic"
                                                                            ValidationGroup="vgItens" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Previsão de Uso">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="txtPrevisaoUso" runat="server"
                                                                            Text='<%# Eval("dtPrevisaoUso", "{0:dd/MM/yyyy}") %>'
                                                                            CssClass="form-control input-sm datepicker"
                                                                            Width="120px"
                                                                            MaxLength="10"
                                                                            placeholder="dd/mm/aaaa"
                                                                            onfocus="(this.type='date')"
                                                                            onblur="if(!this.value) this.type='text';" />

                                                                        <asp:RequiredFieldValidator ID="rfvPrevisaoUso" runat="server"
                                                                            ControlToValidate="txtPrevisaoUso"
                                                                            ErrorMessage="*"
                                                                            ForeColor="Red"
                                                                            Display="Dynamic"
                                                                            ValidationGroup="vgItens" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:TemplateField>


                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </div>

                            <asp:HiddenField ID="hddidProduto" runat="server" Value="0" />

                        </ContentTemplate>

                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cmdIncluirItem" EventName="Click" />
                        </Triggers>

                    </asp:UpdatePanel>

                    <uc1:GerenciadorMembrosModal runat="server" ID="GerenciadorMembrosModal" />
                </ContentTemplate>


            </asp:UpdatePanel>


            <div id="div_Requisicao_Acoes" class="row">
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title">
                                <asp:Label ID="lblRequisicao_Acao_Titulo" runat="server" Text="Label"></asp:Label></h3>
                        </div>
                        <div class="panel-body">
                            <div class="col-lg-10" runat="server" id="div_AlterarStatus">
                                <div class="form-group">
                                    <label>Status</label>
                                    <asp:DropDownList ID="ddlAlterarStatus" class="form-control CaixaTextoGrande" runat="server"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-10">
                                <div class="form-group">
                                    <label>Motivo/Observação</label>
                                    <asp:TextBox ID="txtRequisicao_Observacao" class="form-control" runat="server" TextMode="MultiLine" MaxLength="300"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-lg-2">
                                <div class="form-group">
                                    <br />
                                    <asp:Button ID="cmdRequisicao_Acao_OK" class="btn btn-sm btn-primary" runat="server" Text="Ok" OnClick="cmdRequisicao_Acao_OK_Click" />
                                    <asp:Button ID="cmdRequisicao_Acao_Cancelar" class="btn btn-sm btn-danger" runat="server" Text="Cancelar" OnClick="cmdRequisicao_Acao_Cancelar_Click" />
                                </div>
                            </div>


                        </div>
                    </div>
                </div>
            </div>

            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />


            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-info" runat="server" Text="Salvar" />
                <asp:Button ID="cmdEditar" class="btn  btn-lg  btn-danger" runat="server" Text="Editar" />

                <%--Thiago Rodrigues 25/10/2024--%>
                <asp:Button ID="cmdCotar" class="btn  btn-lg  btn-success" runat="server" Text="Gerar Cotação" OnClick="cmdCotar_Click" />

                <asp:Button ID="cmdAlterarStatus" class="btn  btn-lg  btn-danger" runat="server" Text="Alterar Status" OnClick="cmdAlterarStatus_Click" />
                <div class="btn-group dropup" runat="server" id="Botoes_Acao">
                    <button id="cmdAcao" runat="server" type="button" class="btn btn-lg btn-success dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><span>Ações</span> <span class="caret"></span></button>
                    <ul class="dropdown-menu">
                        <li>
                            <asp:LinkButton ID="lnkIniciarRequisicao" runat="server" OnClick="lnkIniciarRequisicao_Click">Aprovar Requisição</asp:LinkButton></li>
                        <li role="separator" class="divider"></li>
                        <li>
                            <asp:LinkButton ID="lnkRejeitarRequisicao" runat="server" OnClick="lnkRejeitarRequisicao_Click">Rejeitar Requisição</asp:LinkButton></li>
                    </ul>
                </div>


                &nbsp;
                    <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field-cancel" title="Voltar" onclick="history.go(-1)">&nbsp;
            </fieldset>


        </div>

        <div role="tabpanel" class="tab-pane fade" id="arquivos" aria-labelledby="arquivos-tab">
            <div class=" embed-responsive embed-responsive-16by9" style="min-height: 800px">
                <embed type="text/html" runat="server" id="frmArquivos" width="800" height="500" />
            </div>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="historico" aria-labelledby="historico-tab">
            <br />
            <div class="panel panel-default" runat="server" id="DIV_historico">
                <div class="panel-heading">

                    <h3 class="panel-title"><b>Histórico</b></h3>
                </div>
                <div class="panel-body">
                    <div class="form-stacked row">
                        <div class="col-lg-12 table-responsive">
                            <asp:GridView
                                ID="gv_Historico" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                ShowFooter="False">
                                <Columns>
                                    <asp:BoundField DataField="dtLog" HeaderText="Data">
                                        <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário">
                                        <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sTipoAcao" HeaderText="Tipo">
                                        <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sAcao" HeaderText="Ação">
                                        <ItemStyle HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>

                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="cotacao" aria-labelledby="cotacao-tab">
            <br />
            <div class="panel panel-default" runat="server" id="DIV1">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Cotações</b></h3>
                </div>
                <div class="panel-body">
                    <div class="form-stacked row">

                        <fieldset class="form-stacked actions">

                            <asp:Button ID="cmdNovaCotacao" class="btn  btn-sm btn-success" runat="server" Text="Nova Cotação" />
                            <asp:Button ID="cmdAprovarCotacao" class="btn  btn-sm btn-default" runat="server" Text="Aprovar Cotação" />
                            <asp:Button ID="cmdRejeitarCotacao" class="btn  btn-sm btn-danger" runat="server" Text="Rejeitar Cotação" />
                            <div id="div_GerarPDF" runat="server" visible="false">
                                <asp:LinkButton ID="cmdGerarPDF" CssClass="btn btn-sm btn-success" runat="server" OnClick="cmdGerarPDF_Click"><i class="fa fa-print"></i> PDF</asp:LinkButton>
                            </div>
                        </fieldset>

                        <div class="col-lg-12 table-responsive">
                            <asp:GridView
                                ID="gvCotacaoo" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                ShowFooter="False">
                                <Columns>
                                    <asp:BoundField DataField="dtLog" HeaderText="Data">
                                        <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário">
                                        <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sTipoAcao" HeaderText="Tipo">
                                        <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sAcao" HeaderText="Ação">
                                        <ItemStyle HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>

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

    <div id="dialog-Salvar" class="modal" title="Salvar">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
        </p>
    </div>
    <div id="dialog-Editar" class="modal" title="Editar Pedido">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloEdiar" runat="server" Text="Confirma a Edição da Requisição?"></asp:Label>
        </p>
    </div>

    <asp:HiddenField ID="hddIdRequisicao" runat="server" />
    <asp:HiddenField ID="hddsTipoRequisicao" runat="server" />
    <asp:HiddenField ID="hddsFabricacao" runat="server" />
    <asp:HiddenField ID="hddidCotacao" runat="server" />
    <asp:HiddenField ID="hddidTipoRequisicao" runat="server" />
    <asp:HiddenField ID="hddidDepartamento" runat="server" />
    <asp:HiddenField ID="hddidSolicitante" runat="server" />
    <asp:HiddenField ID="hddidTipReqAux" runat="server" />
    
</asp:Content>
