<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="CentroCusto_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Manutencao.CentroCusto_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>
<%@ Register Src="~/App/Controles/Controle_CategoriasCC.ascx" TagPrefix="uc1" TagName="Controle_CategoriasCC" %>
<%@ Register Src="~/App/Controles/ChartControl.ascx" TagPrefix="uc1" TagName="ChartControl" %>





<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <style>
        .total-label {
            font-size: 1.5em;
            width: auto;
            height: auto;
        }
    </style>
    <script>

        function MudarBotao() {
            document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
        }

    </script>
    <div class="form-stacked" style="">
        <div class="col-lg-12 row">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Centro Custo"></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
        </div>

        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

        <div>
            <ul id="tab_CentroCusto" class="nav nav-tabs" role="tablist">
                <li role="presentation" class="tabpanel active">
                    <a href="#centroCusto" id="aba_CentroCusto" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Centro de Custo</b></a>
                </li>

                <li role="presentation" runat="server" id="aba_Categoria">
                    <a href="#categoria" role="tab" id="categoria-tab" data-toggle="tab" aria-controls="Categoria"><b>Categorias</b></a>
                </li>

                <li role="presentation" runat="server" id="aba_Lancamento">
                    <a href="#lancamento" role="tab" id="lancamento-tab" data-toggle="tab" aria-controls="Lancamento"><b>Lançamentos</b></a>
                </li>

                <li role="presentation" runat="server" id="aba_Historico">
                    <a href="#historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="Historico"><b>Histórico</b></a>
                </li>


            </ul>

        </div>
        <br />
        <div id="tab" class="tab-content">
            <div role="tabpanel" class="tab-pane fade in active" id="centroCusto" aria-labelledby="centroCusto-tab">
                <asp:UpdatePanel ID="updDetalhe" runat="server">
                    <ContentTemplate>
                        <asp:Button ID="btnGlobalRefresh" runat="server" OnClick="btnGlobalRefresh_Click" Style="display: none;" />
                        
                        <div class="panel panel-primary" runat="server" id="div1">
                             <div class="panel-heading">
                                 <h3 class="panel-title"><b>Resumo</b></h3>
                             </div>
                             <div class="panel-body text-center">
                                <div id="resumo_receitasDespesas">
                                   <div class="col-lg-5">
 
                                    <asp:GridView ID="gvResumoFinanceiros" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" ShowHeader="false" OnRowDataBound="gvResumoFinanceiros_RowDataBound">
                                        <Columns>
                                            <asp:BoundField DataField="sDscTitulo" HeaderText="">
                                                <ItemStyle Width="60%" HorizontalAlign="Left" VerticalAlign="Middle" Font-Bold="true" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="nValor" HeaderText="">
                                                <ItemStyle Width="20%" HorizontalAlign="Right" VerticalAlign="Middle"  />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nTotal" HeaderText="">
                                                <ItemStyle Width="20%" HorizontalAlign="Right" VerticalAlign="Middle" Font-Bold="true" />
                                            </asp:BoundField>

                                        </Columns>
                                    </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="panel panel-primary" runat="server" style="cursor: pointer;" id="togglePanelExibeGrafico">
                            <div class="panel-heading">
                                <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;<b>Gráficos</b>
                                    <span class="pull-right">
                                        <i runat="server" class="fa fa-chevron-down" id="iconDownExibe"></i>
                                    </span>
                                </h3>
                            </div>
                            <div id="graficoPanelBody" runat="server" class="panel-body" style="display: none">
                                <div id="div_grfCategorias" runat="server" class="row">
                                    <div id="div_grfTetoCategoria" runat="server" class="col-lg-7">
                                        <h4><b>Teto de Gastos x Despesas </b></h4>
                                        <hr />
                                        <uc1:ChartControl runat="server" ID="ChartControl"
                                            Type="Bar"
                                            BarMode="Stacked" Width="80%" Height="600px" />
                                    </div>
                                    <div id="div_grfPizzaCategorias" runat="server" class="col-lg-5">
                                        <h4><b>Despesas por Categoria </b></h4>
                                        <hr />
                                        <uc1:ChartControl runat="server" ID="ChartControlPizza"
                                            Type="Doughnut"
                                            DataLabelField="sDscCategoria" DataValueField="nTotalGastos" Height="400px" />


                                    </div>

                                </div>
                            </div>
                        </div>

                        <div class="panel panel-primary" runat="server" id="div_CentroCusto">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Dados do Centro de Custo</b></h3>
                            </div>
                            <div class="panel-body">

                                <uc1:MensagemPagina runat="server" ID="MensagemPaginaCC" />

                                <div class="form-stacked row">
                                    <div class="col-lg-12">
                                        <div class="form-group">
                                            <label>ID</label>
                                            <asp:TextBox ID="txtidCentroDeCusto" class="form-control CaixaTextoMini" runat="server" disabled=""></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="form-stacked row">
                                        <div class="col-lg-12">

                                            <div class="col-lg-1" style="padding-right: 0px;">
                                                <div class="form-group">
                                                    <label>Tipo</label>
                                                    <asp:DropDownList runat="server" ID="ddlsTipo" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlsTipo_SelectedIndexChanged">
                                                        <%--                                                        <asp:ListItem Text="Selecione o Tipo" Value=""></asp:ListItem>
                                                        <asp:ListItem Text="Interno" Value="I"></asp:ListItem>
                                                        <asp:ListItem Text="Clientes" Value="C"></asp:ListItem>--%>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Código</label>
                                                    <asp:TextBox ID="txtsCodCC" class="form-control " runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-3">
                                                <div class="form-group">
                                                    <label>Descrição</label>
                                                    <asp:TextBox ID="txtsDescricao" class="form-control " runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                        </div>
                                    </div>

                                    <div class="form-stacked row">
                                        <div class="col-lg-12">

                                            <div class="col-lg-3">
                                                <div class="form-group">
                                                    <label>Teto de Gastos</label>
                                                    <asp:TextBox ID="txtnTetoGasto" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-3">
                                                <div class="form-group">
                                                    <label>Saldo</label>
                                                    <asp:TextBox ID="txtnSaldoGasto" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Última Atualização do Saldo</label>
                                                    <asp:TextBox ID="txtdtUltimaAtualizacao" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-3">
                                                <div class="form-group">
                                                    <%--                  <asp:LinkButton runat="server" ID="lnkAtualiza_Saldo" OnClick="lnkAtualiza_Saldo_Click" class="btn btn-info" data-toggle="tooltip" title="Atualizar Saldo" Style="margin-top: 24px;" Visible="false"><i class="fa fa-refresh"></i></asp:LinkButton>--%>
                                                </div>
                                            </div>

                                        </div>
                                    </div>

                                    <div class="form-stacked row">
                                        <div class="col-lg-12">


                                            <div class="col-lg-12">
                                                <div class="form-group">
                                                    <label>Observações</label>
                                                    <asp:TextBox ID="txtsObservacao" class="form-control" runat="server" TextMode="MultiLine" Height="100px" Style="resize: none;"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-1">
                                                <div class="form-group">
                                                    <label>Ativo</label>
                                                    <asp:DropDownList runat="server" ID="ddlsSituacao" class="form-control">
                                                        <asp:ListItem Text="Sim" Value="S"></asp:ListItem>
                                                        <asp:ListItem Text="Não" Value="N"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>



                    </ContentTemplate>
                </asp:UpdatePanel>

                <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                <br />

            </div>
            <div role="tabpanel" class="tab-pane fade" id="historico" aria-labelledby="historico-tab">
                <asp:UpdatePanel ID="updHistorico" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Button ID="btnRefreshHistorico" runat="server" OnClick="btnRefreshHistorico_Click" Style="display: none;" />
                        <br />
                        <div class="panel panel-default" runat="server" id="DIV_historico">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Histórico</b></h3>
                            </div>
                            <div class="panel-body">

                                <ul class="nav nav-pills" style="margin-bottom: 15px;">
                                    <li role="presentation" id="liHistoricoCC" runat="server" class="active">
                                        <asp:LinkButton ID="btnToggleHistoricoCC" runat="server" OnClick="btnToggleHistorico_Click">Centro de Custo</asp:LinkButton>
                                    </li>
                                    <li role="presentation" id="liHistoricoLancamentos" runat="server">
                                        <asp:LinkButton ID="btnToggleHistoricoLancamentos" runat="server" OnClick="btnToggleHistorico_Click">Categorias & Lançamentos</asp:LinkButton>
                                    </li>
                                </ul>

                                <div id="divHistoricoCC" runat="server" visible="true">
                                    <div class="form-stacked row">
                                        <div class="col-lg-12 table-responsive">
                                            <asp:GridView
                                                ID="gv_Historico" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                ShowFooter="False">
                                                <Columns>
                                                    <asp:BoundField DataField="dtAtualizacao" HeaderText="Data da Atualização">
                                                        <ItemStyle Width="10%" HorizontalAlign="center" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="sTipo" HeaderText="Tipo">
                                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="sCodCC" HeaderText="Código">
                                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="sDescricao" HeaderText="Descrição">
                                                        <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="nTetoGasto" HeaderText="Teto de Gasto" DataFormatString="{0:N2}">
                                                        <ItemStyle Width="8%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="nSaldoGasto" HeaderText="Saldo Gasto" DataFormatString="{0:N2}" Visible="false">
                                                        <ItemStyle Width="8%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="sObservacao" HeaderText="Observação">
                                                        <ItemStyle Width="28%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>

                                <div id="divHistoricoLancamentos" runat="server" visible="false">
                                    <div class="form-stacked row">
                                        <div class="col-lg-12 table-responsive">
                                            <asp:GridView
                                                ID="gv_Historico_Lancamentos" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                ShowFooter="False">
                                                <Columns>
                                                    <asp:BoundField DataField="dtAtualizacao" HeaderText="Data da Atualização">
                                                        <ItemStyle Width="15%" HorizontalAlign="center" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                                        <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="sTipoAlteracao" HeaderText="Tipo de Alteração">
                                                        <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="Descrição da Ação">
                                                        <ItemTemplate>
                                                            <%# Eval("sDscAcao") %>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="55%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:TemplateField>
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    <div class="alert alert-info">Nenhum histórico de alteração de categoria ou lançamento encontrado.</div>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <div role="tabpanel" class="tab-pane fade" id="lancamento" aria-labelledby="lancamento-tab">
                <br />
                <div class="panel panel-default" runat="server" id="div_Lancamento">
                    <div class="panel-heading">
                        <h3 class="panel-title"><b>Lançamentos</b></h3>
                    </div>
                    <div class="panel-body">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_AbaLancamento" />
                        <%-- Controle oculto para a lógica de cálculo de saldo que será chamada pelo WebMethod --%>
                        <uc1:Controle_CategoriasCC runat="server" ID="cc" Visible="false" />

                        <%-- Filtros (Estrutura mantida, mas interações via JS) --%>
                        <div class="col-lg-12" style="padding: 0px;" runat="server" id="divGridFiltros">
                            <div class="panel panel-primary">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;Filtro para pesquisa</h3>
                                </div>
                                <div class="panel-body">
                                    <div class="row align-items-center">
                                        <div class="col-lg-4 col-md-6 col-sm-12 mb-2">
                                            <asp:DropDownList runat="server" ID="ddlFiltroCategoria" class="form-control Caixa_Selecao">
                                                <%-- Populado via C# no Page_Load inicial --%>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-lg-4 col-md-6 col-sm-12 mb-2">
                                            <asp:DropDownList runat="server" ID="ddlTipo" class="form-control Caixa_Selecao">
                                                <asp:ListItem Text="Todos Tipos" Value="" />
                                                <asp:ListItem Text="Contas Receber" Value="R" />
                                                <asp:ListItem Text="Contas Pagar" Value="P" />
                                                <asp:ListItem Text="Cartões" Value="C" />
                                                <asp:ListItem Text="Despesas" Value="D" />
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-lg-2 col-md-4 col-sm-6 mb-2">
                                            <asp:DropDownList runat="server" ID="ddlPageSize" class="form-control Caixa_Selecao">
                                                <asp:ListItem Text="10 Itens" Value="10"></asp:ListItem>
                                                <asp:ListItem Text="30 Itens" Value="30" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="50 Itens" Value="50"></asp:ListItem>
                                                <asp:ListItem Text="Todos" Value="-1"></asp:ListItem>
                                                <%-- JS tratará -1 como um PageSize muito grande --%>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-lg-2 col-md-3 col-sm-6 mb-2">
                                            <%-- Botão de Limpar agora aciona JS --%>
                                            <button type="button" id="btnLimparFiltrosJs" class="btn btn-warning btn-block">Limpar</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <%-- Removido o UpdatePanel updCorpo e o Timer --%>
                        <div class="form-stacked row" id="div_gridLancamento" runat="server">
                            <div class="col-lg-12">
                                <h3 class="panel-title">
                                    <%-- Saldo Total será atualizado via JS --%>
<%--                                    <span id="spanSaldoTotalCabecalho"><b>Saldo Total:</b>
                                        <asp:Literal ID="litSaldoTotalInicial" runat="server">0,00</asp:Literal></span>--%>
                                </h3>
                            </div>

                            <div class="col-lg-12 ">
                                <div style="padding: 5px; text-align: right; font-weight: bold;">
                                    <%-- Info de Paginação será atualizada via JS --%>
                                    <span id="spanInfoPaginacao"></span>
                                </div>

                                <%-- Tabela HTML Simples (ou GridView com AutoGenerateColumns=false e sem ItemTemplate) --%>
                                <table id="tabelaLancamentos" class="table table-striped table-bordered table-hover table-condensed table-responsive" style="width: 100%;">
                                    <thead>
                                        <%-- Colunas definidas como no seu GridView original --%>
                                        <tr>
                                            <th style="width: 2%; text-align: center;">ID</th>
                                            <th style="width: 5%; text-align: center;">Data</th>
                                            <th style="width: 8%;">Origem</th>
                                            <th style="width: 20%;">Referência</th>
                                            <th style="width: 20%;">Descrição</th>
                                            <th style="width: 12%;">Categoria C.C.</th>
                                            <th style="width: 14%;">Categoria Título</th>
                                            <th style="width: 9%; text-align: left;">Valor</th>
                                            <th style="width: 1%; text-align: left;">Tipo</th>
                                            <th style="width: 9%; text-align: left;">Saldo</th>
                                            <th style="width: auto;">Ação</th>
                                            <%-- Coluna para o botão editar --%>
                                        </tr>
                                    </thead>
                                    <tbody id="tbodyLancamentos">
                                        <%-- Linhas serão adicionadas aqui via JavaScript --%>
                                    </tbody>
                                    <tfoot id="tfootLancamentos">
                                        <tr>
                                            <td colspan="9" style="text-align: right;"><strong>Saldo:</strong></td>
                                           <%-- <td id="tdTotalValorRodape" style="text-align: right; font-weight: bold;">R$ 0,00</td>
                                           --%>
                                            <%-- Célula para o SALDO FINAL (do seu antigo txtnSaldoGasto) --%>
                                            <td id="tdSaldoFinalRodape" style="text-align: right; font-weight: bold;">R$ 0,00</td>
                                            <td></td>
                                            <%-- Célula Ação vazia --%>
                                        </tr>
                                    </tfoot>
                                </table>
                            </div>

                            <%-- Controles de Paginação (acionarão funções JS) --%>
                            <div class="pagination-container col-lg-12 text-center" style="margin-top: 15px;">
                                <button type="button" id="btnPrimeiraJs" class="btn btn-default btn-sm" disabled>&laquo; Primeira</button>
                                <button type="button" id="btnAnteriorJs" class="btn btn-default btn-sm" disabled>&lsaquo; Anterior</button>
                                <span style="margin: 0 10px;">Página
                       
                                    <span id="spanPaginaAtual">1</span> de <span id="spanTotalPaginas">1</span>
                                </span>
                                <button type="button" id="btnProximoJs" class="btn btn-default btn-sm" disabled>Próxima &rsaquo;</button>
                                <button type="button" id="btnUltimaJs" class="btn btn-default btn-sm" disabled>Última &raquo;</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div role="tabpanel" class="tab-pane fade" id="categoria" aria-labelledby="categoria-tab">
                <br />
                <asp:UpdatePanel ID="updCategorias" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Button ID="btnRefreshCategorias" runat="server" OnClick="btnRefreshCategorias_Click" Style="display: none;" />
                        <div class="panel panel-default" runat="server" id="divCategoria">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Categorias</b></h3>
                            </div>
                            <div class="panel-body">

                                <uc1:MensagemPagina runat="server" ID="MensagemPaginaCategoria" />

                                <div class="form-stacked row" id="divFormInclusao" runat="server">
                                    <div class="col-lg-12">
                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label>Descrição da Categoria</label>
                                                <asp:TextBox ID="txtsDscCategoria" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>Teto de Gastos</label>
                                                <asp:TextBox ID="txtnTetoCategoria" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-2" runat="server" visible="false">
                                            <div class="form-group">
                                                <label>Saldo</label>
                                                <asp:TextBox ID="txtnSaldoCategoria" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                        <%--<div class="col-lg-2">
                                            <div class="form-group">
                                                <uc1:SwitchAtivo runat="server" ID="SwitchAtivoPadrao" />
                                            </div>
                                        </div>--%>
                                        <div class="col-lg-2">
                                            <div class="form-group" style="margin-top: 24px;">
                                                <asp:Button ID="cmdIncluirCategoriaCC" runat="server" Text="Incluir" CssClass="btn btn-info" OnClick="cmdIncluirCategoriaCC_Click" />
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <hr />

                                <div class="form-stacked" id="divGridCategoria" runat="server">
                                    <div class="col-lg-9">
                                        <asp:GridView ID="gridCategorias" runat="server" AutoGenerateColumns="false"
                                            CssClass="table table-striped table-bordered table-responsive table-hover table-condensed"
                                            DataKeyNames="GridKey"
                                            OnRowEditing="gridCategorias_RowEditing"
                                            OnRowCancelingEdit="gridCategorias_RowCancelingEdit"
                                            OnRowUpdating="gridCategorias_RowUpdating"
                                            OnRowDataBound="gridCategorias_RowDataBound"
                                            OnRowDeleting="gridCategorias_RowDeleting"
                                            OnRowCommand="gridCategorias_RowCommand">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Descrição da Categoria" ItemStyle-Width="47%" ControlStyle-Width="100%">
                                                    <ItemTemplate><%# Eval("sDscCategoria") %></ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtGridDscCategoria" runat="server" Text='<%# Bind("sDscCategoria") %>' CssClass="form-control"></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Teto" ControlStyle-Width="100%" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Right" >
                                                    <ItemTemplate><%# Eval("nTeto", "{0:C2}") %></ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtGridTeto" runat="server" Text='<%# Bind("nTeto","{0:C2}") %>' CssClass="form-control"></asp:TextBox>
                                                        <asp:HiddenField ID="hddGridTetoValue" runat="server" />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Total Despesas"  ItemStyle-Width="15%"  ItemStyle-HorizontalAlign="Right">
                                                    <ItemTemplate><%# Eval("nTotalGastos", "{0:C2}") %></ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtGridGastos" disabled="disabled" runat="server" Text='<%# Bind("nTotalGastos","{0:C2}") %>' CssClass="form-control"></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>


                                                <asp:TemplateField HeaderText="Saldo Disponivel"  ItemStyle-Width="15%"  ItemStyle-HorizontalAlign="Right">
                                                    <ItemTemplate><%# Eval("nSaldo", "{0:C2}") %></ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtGridSaldo" disabled="disabled" runat="server" Text='<%# Bind("nSaldo","{0:C2}") %>' CssClass="form-control"></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>

                                                <%--  <asp:TemplateField HeaderText="Padrão">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkPadrao" runat="server" Checked='<%# Eval("bPadrao") %>' Enabled="false" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>--%>

                                                <asp:TemplateField ItemStyle-Width="8%">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkEditar" runat="server" CommandName="Edit"
                                                            CssClass="btn btn-default btn-xs" ToolTip="Editar">
                                                            <i class='fa fa-pencil'></i>
                                                        </asp:LinkButton>

                                                        <asp:LinkButton ID="lnkExcluir" runat="server" CommandName="Delete"
                                                            CssClass="btn btn-danger btn-xs" ToolTip="Excluir">
                                                            <i class="fa fa-trash-o"></i> 
                                                        </asp:LinkButton>
                                                    </ItemTemplate>

                                                    <EditItemTemplate>
                                                        <asp:LinkButton ID="lnkSalvar" runat="server" CommandName="Update"
                                                            CssClass="btn btn-success btn-xs" ToolTip="Salvar">
                                                            <i class='fa fa-check'></i>
                                                        </asp:LinkButton>
                                                        <asp:LinkButton ID="lnkCancelar" runat="server" CommandName="Cancel"
                                                            CssClass="btn btn-danger btn-xs" ToolTip="Cancelar">
                                                            <i class='fa fa-times'></i>
                                                        </asp:LinkButton>
                                                        <asp:LinkButton ID="lnkConfirmarExclusao" runat="server" CommandName="ConfirmDelete"
                                                            CssClass="btn btn-success btn-xs" ToolTip="Confirmar Exclusão"
                                                            CommandArgument='<%# Container.DataItemIndex %>'>
                                                            <i class='fa fa-check'></i>
                                                        </asp:LinkButton>
                                                        <asp:LinkButton ID="lnkCancelarExclusao" runat="server" CommandName="Cancel"
                                                            CssClass="btn btn-danger btn-xs" ToolTip="Cancelar">
                                                            <i class='fa fa-times'></i> 
                                                        </asp:LinkButton>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>

                                <div class="row" runat="server" visible="false">
                                    <div class="col-lg-12 text-right">
                                        <asp:Button ID="btnSalvarCategorias" runat="server" Text="Salvar Todas as Alterações"
                                            CssClass="btn btn-primary" OnClick="btnSalvarCategorias_Click" />
                                    </div>
                                </div>

                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <%----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>


            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" ValidationGroup="DETALHE" />
                <asp:Button ID="cmdEditar" class="btn  btn-lg  btn-danger" runat="server" Text="Editar" />
                <asp:Button ID="btnPedido" class="btn  btn-lg  btn-primary" runat="server" Text="Pedido" Visible="false" OnClick="btnPedido_Click" />

                <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field-cancel" title="Voltar" onclick="history.go(-2)">
            </fieldset>
            <div id="dialog-Salvar" class="modal" title="Salvar">
                <p>
                    <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                    <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
                </p>
            </div>
            <div id="dialog-Editar" class="modal" title="Editar Lançamento">
                <p>
                    <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                    <asp:Label ID="lblTituloEdiar" runat="server" Text="Confirma a Edição do Lançamento?"></asp:Label>
                </p>
            </div>
        </div>

        <%--    <asp:UpdatePanel ID="updModal" runat="server" UpdateMode="Conditional">
    <ContentTemplate>--%>
        <div class="modal fade" id="edicaoLancamentoModal" tabindex="-1" role="dialog" aria-labelledby="modalLabel">
            <div class="modal-dialog modal-lg" role="document">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #f5f5f5; border-bottom: 1px solid #ddd;">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" id="modalLabel"><i class="fa fa-pencil-square-o"></i>Editar Lançamento</h4>
                    </div>
                    <div class="modal-body">
                        <%-- Conteúdo será populado via JS --%>
                        <input type="hidden" id="hddModalLancamentoId" />
                        <input type="hidden" id="hddModalLancamentoTipo" />
                        <div class="row">
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label><i class="fa fa-calendar"></i>Data</label><p class="form-control-static" id="pModalData"></p>
                                </div>
                            </div>
                            <div class="col-sm-8">
                                <div class="form-group">
                                    <label><i class="fa fa-file-text-o"></i>Referência</label><p class="form-control-static" id="pModalReferencia"></p>
                                </div>
                            </div>
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label><i class="fa fa-money"></i>Valor</label><p class="form-control-static" id="pModalValor"></p>
                                </div>
                            </div>
                        </div>
                        <hr />
                        <div class="alert alert-info" style="margin-bottom: 15px;">
                            <strong>Categoria Atual:</strong> <span id="spanModalCategoriaAtual" style="font-weight: bold;"></span>
                        </div>
                        <div class="form-group">
                            <label><strong><i class="fa fa-tags"></i>Selecione a Nova Categoria</strong></label>
                            <%-- Dropdown será populado via C# no PageLoad inicial e usado aqui --%>
                            <asp:DropDownList runat="server" ID="ddlCategoriaModal" CssClass="form-control"></asp:DropDownList>
                        </div>
                        <%-- Mensagens de erro/sucesso do modal --%>
                        <div id="divModalMensagem" style="display: none;" class="alert"></div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">Cancelar</button>
                        <button type="button" id="btnSalvarModalJs" class="btn btn-success"><i class='fa fa-check'></i>Salvar Alteração</button>
                    </div>
                </div>
            </div>
        </div>
        <%--    </ContentTemplate>
</asp:UpdatePanel>--%>

        <asp:HiddenField ID="hddActiveTab" runat="server" Value="#centroCusto" />
        <asp:HiddenField ID="hddidCentroDeCusto" runat="server" Value="0" />
        <asp:HiddenField ID="hddidPedido" runat="server" Value="0" />
        <asp:HiddenField ID="hddidCategoriaFiltro" runat="server" Value="0" />
        <asp:HiddenField ID="hddsTipo" runat="server" Value="" />
        <asp:HiddenField ID="hddGraficoState" runat="server" Value="open" />
        <asp:HiddenField ID="hddnTotalRegistros" runat="server" Value="0" />

        <script type="text/javascript">
            // Função para restaurar o estado do painel de gráficos (sem alteração)
            function restaurarEstadoPainelGraficoVisual() {
                var panelBody = $('[id$=graficoPanelBody]');
                if (panelBody.length === 0) return;
                var icon = $('[id$=iconDownExibe]');
                var storageKey = 'graficoCollapseState';
                var savedState = sessionStorage.getItem(storageKey);
                if (savedState === 'open') {
                    panelBody.show();
                    icon.removeClass('fa-chevron-down').addClass('fa-chevron-up');
                } else {
                    panelBody.hide();
                    icon.removeClass('fa-chevron-up').addClass('fa-chevron-down');
                }
            }

            // ==========================================================
            // INÍCIO DA LÓGICA DE LANÇAMENTOS (AJAX/WEB METHODS)
            // ==========================================================

            let currentPage = 1;
            let currentFilters = {
                categoriaId: null, // null para "Todas"
                tipo: '',
                pageSize: 30,
                centroCustoId: 0
            };

            let refreshIntervalId = null;
            const refreshTime = 600000; // 1 minuto

            function startAutoRefresh() {
                stopAutoRefresh();
                refreshIntervalId = setInterval(function () {
                    console.log("Timer: Atualizando TUDO...");

                    // 1. Atualiza a grid de lançamentos (JS)
                    // (Isso também atualiza o saldo no DB via SP)
                    fetchAndDisplayLancamentos(currentPage, true);

                    // 2. Clica no botão gatilho mestre para atualizar
                    //    todos os painéis de servidor (Saldos, Gráficos, Grids Categoria, Grids Histórico)
                    try {
                        __doPostBack('<%= btnGlobalRefresh.UniqueID %>', '');
                    } catch (e) { console.error("Erro ao disparar refresh global do timer:", e); }

                }, refreshTime);
                // console.log("Timer de atualização global iniciado.");
            }

            function stopAutoRefresh() {
                if (refreshIntervalId) {
                    clearInterval(refreshIntervalId);
                    refreshIntervalId = null;
                    // console.log("Timer de atualização automática parado."); // Opcional: remover para menos log
                }
            }

            // Função Principal para buscar e exibir dados
            function fetchAndDisplayLancamentos(page = 1, isSilentRefresh = false) {

                stopAutoRefresh();

                currentPage = page;
                // Lê o valor. Se for "" (Todas), manda null. Se for "0" (Sem Categoria), manda 0.
                let categoriaFiltroVal = $('#<%= ddlFiltroCategoria.ClientID %>').val();
                currentFilters.categoriaId = (categoriaFiltroVal === "") ? null : parseInt(categoriaFiltroVal, 10);

                currentFilters.tipo = $('#<%= ddlTipo.ClientID %>').val() || '';
                currentFilters.pageSize = parseInt($('#<%= ddlPageSize.ClientID %>').val() || '30', 10);
                currentFilters.centroCustoId = parseInt($('#<%= hddidCentroDeCusto.ClientID %>').val() || '0', 10);

                if (!isSilentRefresh) {
                    $('#tbodyLancamentos').html('<tr><td colspan="11" class="text-center"><i class="fa fa-spinner fa-spin"></i> Carregando...</td></tr>');
                }

                // Retorna a Promise do AJAX
                return $.ajax({
                    type: "POST",
                    url: "CentroCusto_Detalhe.aspx/GetLancamentos",
                    data: JSON.stringify({
                        idCentroDeCusto: currentFilters.centroCustoId,
                        idRegistroCategoria: currentFilters.categoriaId, // Envia null se for "Todas"
                        sTipo: currentFilters.tipo,
                        pageNumber: currentPage,
                        pageSize: currentFilters.pageSize
                    }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        const result = response.d;
                        populateGrid(result.Lancamentos);
                        updatePaginationControls(result.PaginaAtual, result.TotalPaginas, result.TotalRegistros);
                        //$('#spanSaldoTotalCabecalho').html(`<b>Saldo Total:</b> ${result.SaldoTotalFormatado || 'R$ 0,00'}`);

                        // *** ATUALIZADO: Passa o SaldoTotalFormatado para o rodapé ***
                        updateFooter(result.Lancamentos, result.SaldoTotalFormatado);
                    },
                    error: function (xhr, status, error) {
                        console.error("Erro ao buscar lançamentos:", status, error, xhr.responseText);
                        $('#tbodyLancamentos').html('<tr><td colspan="11" class="text-center text-danger">Erro ao carregar dados. Tente novamente.</td></tr>');
                        updatePaginationControls(1, 1, 0);
                       /* $('#spanSaldoTotalCabecalho').html(`<b>Saldo Total:</b> Erro`);*/
                        updateFooter(null, "Erro");
                    },
                    complete: function () {
                        startAutoRefresh(); // Reinicia o timer após a chamada
                    }
                });
            }

            // Função para preencher a tabela
            function populateGrid(lancamentos) {
                const tbody = $('#tbodyLancamentos');
                tbody.empty();

                if (!lancamentos || lancamentos.length === 0) {
                    tbody.html('<tr><td colspan="10" class="text-center">Nenhum lançamento encontrado.</td></tr>');
                    return;
                }

                lancamentos.forEach(item => {
                    let refLink = item.sReferencia || '';
                    let dscLink = item.sDscGeral || '';
                    let url = '#';
                    switch (item.sTipo) {
                        case 'P': url = `/App/Paginas/Adm/Financeiro/ContasPagar_Detalhe.aspx?id=${item.idLink}`; break;
                        case 'R': url = `/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id=${item.id}`; break;
                        case 'C': url = `/App/Paginas/Adm/Financeiro/Cartoes.aspx?id=${item.id}&cc=true`; break;
                        case 'D': url = `/App/Paginas/Adm/Relatorio_Despesas_Detalhe.aspx?id=${item.id}`; break;
                    }
                    if (url !== '#') {
                        refLink = `<a href="${url}" target="_blank">${item.sReferencia || ''}</a>`;
                        dscLink = `<a href="${url}" target="_blank">${item.sDscGeral || ''}</a>`;
                    }

                    let editButton = '';
                    if (item.sTipo !== 'R' && item.sTipo !== 'D') {
                        editButton = `<button type="button" class="btn btn-default btn-xs btn-edit" title="Editar" data-id="${item.id}" data-tipo="${item.sTipo}"><i class="fa fa-pencil"></i></button>`;
                    }

                    // **CORREÇÃO AQUI**: data-categoria-id usa "0" para "Sem Categoria" (vindo da SP)
                    const rowHtml = `
                <tr data-id="${item.id}" data-tipo="${item.sTipo}">
                    <td style="text-align:center;">${item.id || ''}</td>
                    <td style="text-align:center;">${item.dtLancamento || ''}</td>
                    <td>${getTipoTexto(item.sTipo)}</td>
                    <td>${refLink}</td>
                    <td>${dscLink}</td>
                    <td class="categoria-cc-cell" data-categoria-id="${item.idRegistroCategoria || '0'}">${item.sDscCategoriaCC || 'Sem Categoria'}</td>
                    <td>${item.sDscCategoria || ''}</td>
                    <td style="text-align:right;">R$ ${item.nValor || '0,00'}</td>
                    <td style="text-align:center;">${item.sCreditoDebito || ''}</td>
                    <td style="text-align:right;">R$ ${item.nSaldo || '0,00'}</td>
                    <td>${editButton}</td>
                </tr>
            `;
                    tbody.append(rowHtml);
                });
            }

            // Função auxiliar para texto do tipo
            function getTipoTexto(sTipo) {
                switch (sTipo) {
                    case 'P': return 'Contas a Pagar';
                    case 'R': return 'Contas a Receber';
                    case 'C': return 'Cartão';
                    case 'D': return 'Relatório de Gastos';
                    default: return sTipo || '';
                }
            }

            // *** ATUALIZADO: Função para atualizar o rodapé (Total da Página e Saldo Final) ***
            function updateFooter(lancamentos, saldoTotalFormatado) {
                let totalValorPagina = 0;

                if (lancamentos && lancamentos.length > 0) {
                    lancamentos.forEach(item => {
                        try {
                            // Remove R$, pontos de milhar, e troca vírgula por ponto para somar
                            const valorStrLimpo = (item.nValor || '0').toString()
                                .replace("R$", "").trim()
                                .replace(/\./g, '')
                                .replace(',', '.');
                            totalValorPagina += parseFloat(valorStrLimpo);
                        } catch (e) { console.error("Erro ao converter valor do rodapé:", item.nValor, e); }
                    });
                }

                const formatoBR = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' });

                // Atualiza a célula de SOMA DE VALOR (antigo GRID.SomarColunas)
                $('#tdTotalValorRodape').text(formatoBR.format(totalValorPagina));

                // Atualiza a célula de SALDO FINAL (antigo txtnSaldoGasto)
                $('#tdSaldoFinalRodape').text('R$ ' + saldoTotalFormatado || 'R$ 0,00');
            }

            // Função para atualizar controles de paginação
            function updatePaginationControls(paginaAtual, totalPaginas, totalRegistros) {
                $('#spanPaginaAtual').text(paginaAtual);
                $('#spanTotalPaginas').text(totalPaginas);

                $('#btnPrimeiraJs').prop('disabled', paginaAtual <= 1);
                $('#btnAnteriorJs').prop('disabled', paginaAtual <= 1);
                $('#btnProximoJs').prop('disabled', paginaAtual >= totalPaginas);
                $('#btnUltimaJs').prop('disabled', paginaAtual >= totalPaginas);

                if (totalRegistros > 0) {
                    const pageSize = parseInt($('#<%= ddlPageSize.ClientID %>').val() || '30', 10);
                    const effectivePageSize = (pageSize === -1 || pageSize === 0) ? totalRegistros : pageSize;
                    if (effectivePageSize > 0 && paginaAtual > 0) {
                        const registroInicial = ((paginaAtual - 1) * effectivePageSize) + 1;
                        const registroFinal = Math.min((paginaAtual * effectivePageSize), totalRegistros);
                        $('#spanInfoPaginacao').text(`Mostrando registros ${registroInicial} a ${registroFinal} de ${totalRegistros}`);
                    } else {
                        $('#spanInfoPaginacao').text(`Mostrando 0 a 0 de ${totalRegistros}`);
                    }
                } else {
                    $('#spanInfoPaginacao').text('');
                }
            }

            // Função para abrir e popular o modal
            function openEditModal(lancamentoId, tipoLancamento) {
                stopAutoRefresh();
                const linha = $(`#tbodyLancamentos tr[data-id='${lancamentoId}'][data-tipo='${tipoLancamento}']`);
                if (linha.length === 0) {
                    alert('Erro: Lançamento não encontrado na tabela.');
                    startAuto
                        ();
                    return;
                }

                const data = linha.find('td:eq(1)').text();
                const referencia = linha.find('td:eq(3)').html();
                const valor = linha.find('td:eq(7)').text();
                const categoriaCell = linha.find('td:eq(5)');
                const categoriaAtualTexto = categoriaCell.text();
                // **CORREÇÃO AQUI**: Pega o ID e trata NULL/undefined como '0'
                const categoriaAtualId = categoriaCell.data('categoria-id') || '0';

                $('#hddModalLancamentoId').val(lancamentoId);
                $('#hddModalLancamentoTipo').val(tipoLancamento);
                $('#pModalData').text(data);
                $('#pModalReferencia').html(referencia);
                $('#pModalValor').text(valor);
                $('#spanModalCategoriaAtual').text(categoriaAtualTexto);

                // **CORREÇÃO AQUI**: Define o valor do dropdown para "0" se for "Sem Categoria"
                $('#<%= ddlCategoriaModal.ClientID %>').val(categoriaAtualId.toString());

                $('#divModalMensagem').hide().text('');
                $('#edicaoLancamentoModal').modal('show');
            }

            // Função para salvar a categoria do modal
            function saveLancamentoCategory() {
                const lancamentoId = parseInt($('#hddModalLancamentoId').val() || '0', 10);
                const tipoLancamento = $('#hddModalLancamentoTipo').val();
                const novaCategoriaId = parseInt($('#<%= ddlCategoriaModal.ClientID %>').val() || '0', 10);
                const centroCustoId = parseInt($('#<%= hddidCentroDeCusto.ClientID %>').val() || '0', 10);
                const modalMensagemDiv = $('#divModalMensagem');

                if (lancamentoId === 0 || !tipoLancamento) {
                    modalMensagemDiv.removeClass('alert-success alert-danger').addClass('alert alert-warning').text('Erro: ID ou Tipo do lançamento não encontrados no modal.').show();
                    return;
                }

                $('#btnSalvarModalJs').prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Salvando...');
                modalMensagemDiv.hide().text('');

                $.ajax({
                    type: "POST",
                    url: "CentroCusto_Detalhe.aspx/SalvarCategoriaLancamento",
                    data: JSON.stringify({
                        idCentroDeCusto: centroCustoId,
                        idLancamento: lancamentoId,
                        sTipo: tipoLancamento,
                        idNovaCategoria: novaCategoriaId // Envia 0 se for "Sem Categoria"
                    }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        const result = response.d;
                        if (result.success) {
                            const linha = $(`#tbodyLancamentos tr[data-id='${lancamentoId}'][data-tipo='${tipoLancamento}']`);
                            if (linha.length > 0) {
                                const novaCategoriaTexto = $('#<%= ddlCategoriaModal.ClientID %> option:selected').text();
                                const categoriaCell = linha.find('td:eq(5)');
                                categoriaCell.text(novaCategoriaTexto);
                                categoriaCell.attr('data-categoria-id', novaCategoriaId); // Atualiza o data attribute
                            }

                            //if (result.novoSaldo) {
                            //    $('#spanSaldoTotalCabecalho').html(`<b>Saldo Total:</b> ${result.novoSaldo}`);
                            //}

                            modalMensagemDiv.removeClass('alert-danger alert-warning').addClass('alert alert-success').html(result.message || 'Salvo com sucesso!').show();
                            setTimeout(function () {
                                $('#edicaoLancamentoModal').modal('hide');
                            }, 1500);

                            // *** ATUALIZAÇÃO IMPORTANTE ***
                            // Força o postback do UpdatePanel das Categorias
                            // para atualizar a grid e os gráficos.
                            console.log('Forçando refresh das categorias...');
                            __doPostBack('<%= btnRefreshCategorias.UniqueID %>', '');
                            __doPostBack('<%= btnRefreshHistorico.UniqueID %>', '');

                        } else {
                            modalMensagemDiv.removeClass('alert-success alert-warning').addClass('alert alert-danger').html(result.message || 'Erro desconhecido ao salvar.').show();
                        }
                    },
                    error: function (xhr, status, error) {
                        console.error("Erro ao salvar categoria:", status, error, xhr.responseText);
                        let errorMsg = 'Erro de comunicação com o servidor.';
                        if (xhr.responseJSON && xhr.responseJSON.Message) {
                            errorMsg = xhr.responseJSON.Message;
                        }
                        modalMensagemDiv.removeClass('alert-success alert-warning').addClass('alert alert-danger').text(errorMsg).show();
                    },
                    complete: function () {
                        $('#btnSalvarModalJs').prop('disabled', false).html("<i class='fa fa-check'></i> Salvar Alteração");
                        // O timer será reiniciado pelo evento 'hidden.bs.modal'
                    }
                });
            }

            // Flag para garantir que os handlers só sejam anexados uma vez
            let lancamentoHandlersAnexados = false;

            // --- CORREÇÃO: Handlers movidos para pageLoad() ---
            function pageLoad(sender, args) {

                // Lógica das abas
                $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                    var activeTab = $(e.target).attr('href');
                    $('#<%= hddActiveTab.ClientID %>').val(activeTab);
                });
                var lastActiveTab = $('#<%= hddActiveTab.ClientID %>').val();
                if (lastActiveTab) {
                    $('a[href="' + lastActiveTab + '"]').tab('show');
                }

                // Lógica do painel de gráficos
                var panelId = '#<%= togglePanelExibeGrafico.ClientID %>';
                var storageKey = 'graficoCollapseState';
                var hddState = $('#<%= hddGraficoState.ClientID %>');

                // Anexa handlers do gráfico SOMENTE UMA VEZ
                if (!$(panelId).data('handler-anexado')) {
                    $(document).on('click', panelId, function (e) {
                        if ($(e.target).closest('.panel-body').length > 0) { return; }
                        var panelBody = $(this).find('[id$=graficoPanelBody]');
                        var icon = $(this).find('[id$=iconDownExibe]');
                        var isCurrentlyVisible = panelBody.is(':visible');
                        var newState = isCurrentlyVisible ? 'closed' : 'open';
                        sessionStorage.setItem(storageKey, newState);
                        hddState.val(newState);
                        panelBody.slideToggle();
                        icon.toggleClass('fa-chevron-down fa-chevron-up');
                        return false;
                    }).on('click', panelId + ' .panel-body', function () {
                        setTimeout(function () { restaurarEstadoPainelGraficoVisual(); }, 10);
                    });
                    $(panelId).data('handler-anexado', true);
                }

                if (sessionStorage.getItem(storageKey) === null) {
                    sessionStorage.setItem(storageKey, hddState.val());
                }
                restaurarEstadoPainelGraficoVisual();
                // Fim lógica gráficos


                // --- ANEXA OS HANDLERS DA ABA LANÇAMENTO ---
                // Garante que só rode UMA VEZ
                if (!lancamentoHandlersAnexados) {
                    // Carrega dados iniciais SE houver um Centro de Custo válido
                    if (parseInt($('#<%= hddidCentroDeCusto.ClientID %>').val() || '0', 10) > 0) {
                        // Atraso de 100ms para garantir que a aba JS carregue
                        setTimeout(function () { fetchAndDisplayLancamentos(1); }, 100);
                    }

                    // Filtros e PageSize - Disparam busca
                    $('#<%= ddlFiltroCategoria.ClientID %>, #<%= ddlTipo.ClientID %>, #<%= ddlPageSize.ClientID %>').on('change', function () {
                        fetchAndDisplayLancamentos(1);
                    });

                    // Botão Limpar Filtros
                    $('#btnLimparFiltrosJs').on('click', function () {
                        console.log("Botão Limpar Clicado!"); // DEBUG
                        // **CORREÇÃO AQUI**: Define "Todas Categorias" para "" (vazio)
                        $('#<%= ddlFiltroCategoria.ClientID %>').val('0');
                        $('#<%= ddlTipo.ClientID %>').val('0');
                        $('#<%= ddlPageSize.ClientID %>').val('30');
                        fetchAndDisplayLancamentos(1);
                    });

                    // Botões de Paginação
                    $('#btnPrimeiraJs').on('click', function () { fetchAndDisplayLancamentos(1); });
                    $('#btnAnteriorJs').on('click', function () { if (currentPage > 1) fetchAndDisplayLancamentos(currentPage - 1); });
                    $('#btnProximoJs').on('click', function () {
                        const totalPaginas = parseInt($('#spanTotalPaginas').text() || '1', 10);
                        if (currentPage < totalPaginas) fetchAndDisplayLancamentos(currentPage + 1);
                    });
                    $('#btnUltimaJs').on('click', function () {
                        const totalPaginas = parseInt($('#spanTotalPaginas').text() || '1', 10);
                        fetchAndDisplayLancamentos(totalPaginas);
                    });

                    // Abrir Modal (usando delegação de eventos)
                    $('#tbodyLancamentos').on('click', '.btn-edit', function () {
                        stopAutoRefresh();
                        const id = $(this).data('id');
                        const tipo = $(this).data('tipo');
                        openEditModal(id, tipo);
                    });

                    // Salvar Modal
                    $('#btnSalvarModalJs').on('click', saveLancamentoCategory);

                    // Reiniciar o timer quando o modal for fechado
                    $('#edicaoLancamentoModal').on('hidden.bs.modal', function () {
                        startAutoRefresh();
                    });

                    // Parar o timer ao sair da página
                    $(window).on('beforeunload', function () {
                        stopAutoRefresh();
                    });

                    lancamentoHandlersAnexados = true; // Marca como anexado
                }
            }
        </script>
</asp:Content>

