<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Tarefas.aspx.cs" Inherits="TT_Flow.App.Tarefas" %>
<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height:600px" runat ="server" visible="false"> </div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
        <div class="form-stacked row">
            <div class="col-lg-12">
                <h1><asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label><small> Consulta</small></h1>
                <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina"  NivelPagina="2" TitulodaPagina=""/>
            </div>
        </div>
        <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa </h3>
                </div>
                <div class="panel-body">
                    <div class="col-lg-4">
                        <div class="form-group">
                            <asp:DropDownList ID="ddlDepartamento" runat="server" AutoPostBack="true" class="form-control yes_no select CaixaTextoMedio"  OnSelectedIndexChanged="ddlDepartamento_SelectedIndexChanged" > </asp:DropDownList>
				        </div>
                    </div>
                    <div class="col-lg-4">
                    <div class="form-group">
 		                <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="true" class="form-control yes_no select CaixaTextoMedio" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged" > </asp:DropDownList>
					</div>
                    </div>
           
                </div>
            </div>
        <div class="form-stacked row">
            <div class="col-lg-12">
                <uc1:MensagemPagina runat="server" id="MensagemPagina" />
                <asp:Panel ID="pnResultado" class="" runat="server">
                    <div id="resultado" >
                        <div class="panel panel-primary">
                        <div class="panel-body">
                            <div class="table-responsive">
                            <asp:GridView   ID="dtgvConsulta" class="table table-striped table-bordered table-hover " 
                                runat="server" Width="100%" CellSpacing="1"  CellPadding="1"   AutoGenerateColumns="False" GridLines="None"  
                                    ShowFooter="False"  Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">
                            <Columns>
                                <asp:HyperLinkField DataNavigateUrlFields="idPedido,idRegistroTarefa, sDashBoard" 
                                    DataTextField="idPedidoFormatado" HeaderText="Pedido" 
                                    DataNavigateUrlFormatString="Pedidos_Detalhe.aspx?id={0}">
                                    <HeaderStyle  Width="8%" />
                                    <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="idPedido,idRegistroTarefa, sDashBoard" 
                                    DataTextField="sReferencia" HeaderText="Ref." 
                                    DataNavigateUrlFormatString="Pedidos_Detalhe.aspx?id={0}">
                                    <HeaderStyle  Width="8%" />
                                    <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="idPedido,idRegistroTarefa, sDashBoard" 
                                    DataTextField="sDscCliente" HeaderText="Cliente" 
                                    DataNavigateUrlFormatString="Manutencao/Parceiros_Detalhe.aspx?id={0}">
                                    <HeaderStyle  Width="20%" />
                                    <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="idPedido,idRegistroTarefa, sDashBoard" 
                                    DataTextField="sDscStatus" HeaderText="Status" 
                                    DataNavigateUrlFormatString="Pedidos_Detalhe.aspx?id={0}">
                                    <HeaderStyle  Width="15%" />
                                    <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="idPedido,idRegistroTarefa, sDashBoard" 
                                    DataTextField="sDscDepartamento" HeaderText="Departamento" 
                                    DataNavigateUrlFormatString="Pedidos_Detalhe.aspx?id={0}">
                                    <HeaderStyle  Width="15%" />
                                    <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                            
                                <asp:HyperLinkField DataNavigateUrlFields="idPedido,idRegistroTarefa, sDashBoard" 
                                    DataTextField="sDscUsuarioResponsavel" HeaderText="Responsável" 
                                    DataNavigateUrlFormatString="Pedidos_Detalhe.aspx?id={0}">
                                    <HeaderStyle  Width="15%" />
                                    <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="idPedido,idRegistroTarefa, sDashBoard" 
                                    DataTextField="sDscTarefa" HeaderText="Tarefa" 
                                    DataNavigateUrlFormatString="Pedidos_Detalhe.aspx?id={0}&idrt={1}&DashBoard={2}">
                                    <HeaderStyle  Width="20%" />
                                    <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="idPedido,idRegistroTarefa, sDashBoard" 
                                    DataTextField="dtPrevisaoConclusao" HeaderText="Previsão" 
                                    DataNavigateUrlFormatString="Pedidos_Detalhe.aspx?id={0}">
                                    <HeaderStyle  Width="12%" />
                                    <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="idPedido,idRegistroTarefa, sDashBoard" 
                                    DataTextField="sDscStatusTarefa" HeaderText="Status" 
                                    DataNavigateUrlFormatString="Pedidos_Detalhe.aspx?id={0}">
                                    <HeaderStyle  Width="10%" />
                                    <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

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
 
</asp:Content>
