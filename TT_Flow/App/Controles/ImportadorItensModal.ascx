<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImportadorItensModal.ascx.cs" Inherits="TT_Flow.App.Controles.ImportadorItensModal" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<div class="modal fade" id="Modal_ImportarItens" tabindex="-999" data-backdrop="static" role="dialog">
    <div class="modal-dialog modal-sm modal-dialog-centered modal-largo" role="document">
        <div class="modal-content">
            <asp:UpdatePanel ID="UpdateImportarItens" runat="server">
                <ContentTemplate>
                    <div class="modal-header">
                        <button type="button" id="cmdFecharModal" class="close" data-dismiss="modal" aria-label="Fechar">
                            <span aria-hidden="true">&times;</span>
                        </button>
                        <div class="modal-header-content">
                            <div class="modal-logo">
                                <asp:Image ID="imgLogo" runat="server" ImageUrl="~/App/img/LogoTT.png" AlternateText="LogoTipo" />
                            </div>
                            <div class="modal-title-container">
                                <asp:Label runat="server" class="modal-title" ID="lblTituloImportItens" Text="Importar Itens" Font-Bold="true"></asp:Label>
                            </div>
                        </div>
                    </div>

                    <div class="modal-body">
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12 row">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_ImportacaoItens" />
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <div id="tiposView" class="row">
                                            <div runat="server" id="div_TipoImportacao" class="col-lg-12 btn-group tiposView">
                                                <asp:RadioButtonList id="rb_Resposta" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="cbTipoImportacao_CheckedChanged" AutoPostBack="true">
                                           <%--         <asp:ListItem class="btn btn-primary" Value="PEDIDO">&nbsp;Pedido &nbsp;&nbsp;</asp:ListItem>
                                                    <asp:ListItem class="btn btn-primary" Value="OPI">&nbsp;OPI&nbsp;&nbsp;</asp:ListItem>
                                                    <asp:ListItem class="btn btn-primary" Value="LME">&nbsp;LME&nbsp;&nbsp;</asp:ListItem>--%>
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <asp:Panel ID="pnResultado" Visible="false" class="" runat="server">
                                    <div class="col-lg-12">
                                        <div class="panel panel-primary">
                                            <div class="panel-body">
                                                <div class="row form-group">
                                                    <div class="col-lg-12 table-responsive">
                                                        <asp:GridView ID="gvImportacaoItens" class="table table-striped table-bordered table-hover"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="id, sTipo"
                                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowCommand="gvImportacaoItens_RowCommand" OnRowDataBound="gvImportacaoItens_RowDataBound">
                                                            <Columns>
                                                                <asp:ButtonField DataTextField="id" HeaderText="ID" ItemStyle-CssClass="Importacao" CommandName="Importacao" ItemStyle-Width="2%" />
                                                                <asp:ButtonField DataTextField="idObjeto" HeaderText="ID" ItemStyle-CssClass="Importacao" CommandName="Importacao" ItemStyle-Width="6%" />
                                                                <asp:ButtonField DataTextField="sTipo" HeaderText="Tipo" ItemStyle-CssClass="Importacao" CommandName="Importacao" ItemStyle-Width="8%" />
                                                                <asp:ButtonField DataTextField="sDscCliente" HeaderText="Cliente" ItemStyle-CssClass="Importacao" CommandName="Importacao" ItemStyle-Width="20%" />
                                                                <asp:ButtonField DataTextField="sControleTT" HeaderText="Controle TT" ItemStyle-CssClass="Importacao" CommandName="Importacao" ItemStyle-Width="10%" />
                                                                <asp:ButtonField DataTextField="sReferencia" HeaderText="Referência" ItemStyle-CssClass="Importacao" CommandName="Importacao" ItemStyle-Width="20%" />
                                                            </Columns>
                                                        </asp:GridView>

                                                        <%-- 2. GRID EXCLUSIVA COMEX (NFe) --%>
                                                        <asp:GridView ID="gvImportacaoComex" class="table table-striped table-bordered table-hover"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="idImportador"
                                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowCommand="gvImportacaoComex_RowCommand" OnRowDataBound="gvImportacaoComex_RowDataBound">
                                                            <Columns>
                                                                <asp:ButtonField DataTextField="idImportador_Formatado" HeaderText="ID" ItemStyle-CssClass="Importacao" CommandName="Importacao" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center" />
                                                                <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Inclusão" ItemStyle-Width="10%" />
                                                                <asp:BoundField DataField="sEmpresa" HeaderText="Emitente" />
                                                                <asp:BoundField DataField="sCliente" HeaderText="Parceiro" />
                                                                <asp:BoundField DataField="sNomeArquivo" HeaderText="Nome do Arquivo" />
                                                                <asp:BoundField DataField="sDscStatus" HeaderText="Status" ItemStyle-Width="8%" />
                                                                <asp:BoundField DataField="nNumeroNF" HeaderText="NFe" ItemStyle-Width="8%" />
                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </asp:Panel>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div class="modal-footer text-left">
            </div>
        </div>
    </div>
</div>