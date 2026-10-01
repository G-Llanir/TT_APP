<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="PosVendas.aspx.cs" Inherits="TT_Flow.App.Paginas.Comercial.PosVendas" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>
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
                    <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label>
                    <small><asp:Label ID="lsubTituloPagina" runat="server" Text=" Consulta"></asp:Label> </small></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa </h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group">
                                <div class="col-lg-3">
                                    <asp:DropDownList ID="ddlsTipoPesquisa" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>

                                <div class="col-lg-3 form-group" runat="server" id="DIV_Fluxo">
                                    <asp:ListBox ID="ddlFluxo" runat="server" class="Caixa_Selecao form-control" SelectionMode="Multiple"></asp:ListBox>
                                </div>

                                <div class="col-lg-3">
                                    <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-lg-1">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
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
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" DataKeyNames="idPedido" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound" OnRowCommand="dtgvConsulta_RowCommand" >
                                            <Columns>
                                                    <asp:ButtonField DataTextField="nNumeroPedido" HeaderText="Pedido"  CommandName="Consulta" ItemStyle-Width="6%" />
                                                    <asp:ButtonField DataTextField="dtPedido" HeaderText="Data Pedido"  CommandName="Consulta" ItemStyle-Width="6%" />
                                                    <asp:ButtonField DataTextField="nControleTT" HeaderText="Controle TT"  CommandName="Consulta" ItemStyle-Width="8%" />
                                                    <asp:ButtonField DataTextField="sReferencia" HeaderText="Referência"  CommandName="Consulta" ItemStyle-Width="18%" />
                                                    <asp:ButtonField DataTextField="sDscCliente" HeaderText="Cliente"  CommandName="Consulta" ItemStyle-Width="20%" />
                                                    <asp:ButtonField DataTextField="sDscFluxo" HeaderText="Fluxo"  CommandName="Consulta" ItemStyle-Width="15%" />
                                                    <asp:ButtonField DataTextField="dtPrevisaoEntregaFormatada" HeaderText="Prev Entrega"  CommandName="Consulta" ItemStyle-Width="6%" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
 
                    </asp:Panel>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

        <div class="modal fade" id="Modal_Detalhe" tabindex="-2" role="dialog" aria-labelledby="modalFormLabel" aria-hidden="true" data-backdrop="static">
        <div class="modal-dialog modal-largo">

            <asp:UpdatePanel ID="UpdModal_Detalhe" runat="server">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <div class="modal-logo">
                                <asp:Image ID="Image2" runat="server" ImageUrl="~/App/img/LogoTT.png" AlternateText="LogoTipo" />
                                <label runat="server" id="lblTitulo_Modal_Detalhe"></label>
                                <button type="button" id="btnFechar" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span></button>
                            </div>
                        </div>
                        <div class="modal-body">

                            <uc1:MensagemPagina runat="server" ID="MensagemPagina_Modal_Receber" />

                            <div id="abas_Modal_Detalhe">
                                <ul class="nav nav-tabs" role="tablist">
                                    <li runat="server" id="aba_Pedido" role="presentation" class="active">
                                        <a href="#Pedido" id="tab-pedido" role="tab" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Pedido</b></a>
                                    </li>
                                    <li runat="server" id="aba_Contato" role="presentation">
                                        <a href="#Contato" role="tab" id="tab-Contato" data-toggle="tab" aria-controls="Contato"><b>Contato</b></a>
                                    </li>
                                    <li runat="server" id="aba_Envios" role="presentation">
                                        <a href="#Contato" role="tab" id="tab-Envios" data-toggle="tab" aria-controls="OPI/Envios"><b>OPI/Envios</b></a>
                                    </li>
                                    <li runat="server" id="aba_NF" role="presentation">
                                        <a href="#NF" role="tab" id="tab-NF" data-toggle="tab" aria-controls="NF"><b>Nota Fiscal</b></a>
                                    </li>
                                </ul>
                                <br />
                            </div>

                            <div id="tab_Modal_Receber" class="tab-content">
                                <div role="tabpanel" class="tab-pane fade in active" id="Pedido" aria-labelledby="pedido-tab">
                                    <div class="panel panel-default" runat="server" id="div_NF">
                                        <div class="panel-heading">
                                            <h3 class="panel-title"><b>Dados</b></h3>
                                        </div>
                                        <div class="panel-body">
                                            <div class="form-stacked row">
                                                <div class="col-lg-1">
                                                    <div class="form-group">
                                                        <label>Pedido</label>
                                                        <br />
                                                        <h4>
                                                            <asp:HyperLink ID="hplPedido" Target="_blank" runat="server" CssClass="form-control label label-primary"></asp:HyperLink>
                                                        </h4>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2">
                                                    <div class="form-group">
                                                        <label>Data Pedido</label>
                                                        <asp:TextBox ID="txtdtPedido" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-3">
                                                    <div class="form-group">
                                                        <label>N° Pedido (Cliente)</label>
                                                        <asp:TextBox ID="txtnNumeroPedidoCliente" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2">
                                                    <div class="form-group">
                                                        <label>N° Controle TT</label>
                                                        <asp:TextBox ID="txtnControle" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-4">
                                                    <div class="form-group">
                                                        <label>Referência</label>
                                                        <asp:TextBox ID="txtsReferencia" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-6">
                                                    <div class="form-group">
                                                        <label>Cliente</label>
                                                        <asp:TextBox ID="txtsDscCliente" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-6">
                                                    <div class="form-group">
                                                        <label>Endereço de Entrega</label>
                                                        <asp:TextBox ID="txtsEnderecoEntrega" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-4">
                                                    <div class="form-group">
                                                        <label>Empresa</label>
                                                        <asp:TextBox ID="txtsDscEmpresa" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2">
                                                    <div class="form-group">
                                                        <label>Condição Pagamento</label>
                                                        <asp:TextBox ID="txtsDscCondicaoPagamento" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2">
                                                    <div class="form-group">
                                                        <label>Tipo de Faturamento</label>
                                                        <asp:TextBox ID="txtsDscTipoFaturamento" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2" runat="server" id="DIV_Status">
                                                    <div class="form-group">
                                                        <label>Status OPI</label>
                                                        <asp:TextBox ID="txtsDscStatus" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2">
                                                    <div class="form-group">
                                                        <label>Fluxo</label>
                                                        <asp:TextBox ID="txtsDscFluxo" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-12" runat="server">
                                                    <div class="row">
                                                        <div class="col-lg-2">
                                                            <div class="form-group">
                                                                <label>Valor Produto</label>
                                                                <asp:TextBox ID="txtnVlrProdutos" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2">
                                                            <div class="form-group">
                                                                <label>Valor Serviços</label>
                                                                <asp:TextBox ID="txtnVlrServico" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2">
                                                            <div class="form-group">
                                                                <label>Valor Total</label>
                                                                <asp:TextBox ID="txtnVlrTotal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-lg-12">
                                                    <label>Observações Gerais</label>
                                                    <asp:TextBox ID="txtsObservacaoGeral" class="form-control" runat="server" TextMode="MultiLine" Rows="5"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    
                                    </div>
                                </div>
                            


                                <div role="tabpanel" class="tab-pane fade" id="Informacoes" aria-labelledby="Informacoes-tab">

                                    <div class="panel panel-default" runat="server" id="div6">
                                        <div class="panel-heading">
                                            <h3 class="panel-title"><b>Detalhe</b></h3>
                                        </div>
                                        <div class="panel-body">
                                            <div class="form-stacked row">
                                            </div>
                                          
                                        </div>

                                    </div>

                                </div>


                                <div class="modal fade" id="modal_SalvarFaturamento">
                                    <div class="modal-dialog modal-pequeno">
                                        <div class="modal-content">
                                            <div class="modal-header">
                                                <h5 class="modal-title">Gerar Faturamento</h5>
                                            </div>
                                            <div class="modal-body">
                                                <asp:Label ID="lblTitulo_SalvarFaturamento" runat="server" Text="" Font-Bold="true"></asp:Label>
                                            </div>
                                            <div class="modal-footer">
                                       <%--         <asp:LinkButton ID="cmdSalvarFaturamento" CssClass="btn btn-success" Text="Sim" runat="server" OnClick="cmdSalvarFaturamento_Click" />
                                                <asp:LinkButton CssClass="btn btn-danger" Text="Não" runat="server" OnClientClick="$('#modal_SalvarFaturamento').modal('hide'); return false;" />--%>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>

       
                            
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="cmdSalvar" Text="Salvar" runat="server" CssClass="btn btn-success" OnClick="cmdSalvar_Click" />
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Fechar</button>
                        </div>

                    </div>

                </ContentTemplate>
                <Triggers>
                </Triggers>
            </asp:UpdatePanel>

        </div>
    </div>


   


    
     <div id="dialog-Salvar" class="modal modal-salvar" title="Salvar" >
     <p>
         <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
         <asp:Label ID="lblTituloSalvar"  runat="server" Text="Deseja Salvar?"></asp:Label>
     </p>
    </div>

</asp:Content>
