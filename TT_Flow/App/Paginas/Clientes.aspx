<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Clientes.aspx.cs" Inherits="TT_Flow.App.Paginas.Clientes" %>
<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <script type="text/javascript">
                function MudarBotao() {
                    document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
                }
            </script>
            <div class="form-stacked row" id="DIV_PESQUISA" runat="server">
                <div class="col-lg-12">
                    <h1><asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label><small> <asp:Label ID="lblSubTituloPagina" runat="server" Text="SUB_TITULO"></asp:Label></small></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina"  NivelPagina="2" TitulodaPagina=""/>
                </div>
                <div class="col-lg-12">
                
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa </h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group">
                                <div class="col-lg-4">
                                    <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-lg-4">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click"  onclientclick="MudarBotao()" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            
            
            <div class="col-lg-12">
                <asp:Panel ID="pnResultado" class="" runat="server">
                    <div id="resultado" >
                        <div class="panel panel-primary">
                        <div class="panel-body">
                            <div class="table-responsive">
                            <asp:GridView   ID="dtgvConsulta" class="table table-striped table-bordered table-hover " 
                                runat="server" Width="100%" CellSpacing="1"  CellPadding="1"   AutoGenerateColumns="False" GridLines="None"  
                                 ShowFooter="False"  Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">
                            <Columns>
                                <asp:HyperLinkField DataNavigateUrlFields="sLink" 
                                    DataTextField="sRazaoSocial" HeaderText="Nome do Cliente" 
                                    DataNavigateUrlFormatString="{0}">
                                    <HeaderStyle  Width="30%" />
                                    <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="sLink" 
                                    DataTextField="sNomeFantasia" HeaderText="Fantasia" 
                                    DataNavigateUrlFormatString="{0}">
                                    <HeaderStyle  Width="25%" />
                                    <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="sLink" 
                                    DataTextField="sCPF_CNPJ" HeaderText="CPF/CNPJ" 
                                    DataNavigateUrlFormatString="{0}">
                                    <HeaderStyle  Width="15%" />
                                    <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="sLink" 
                                    DataTextField="sDscTipoSituacaoCliente" HeaderText="Situação" 
                                    DataNavigateUrlFormatString="{0}">
                                    <HeaderStyle  Width="15%" />
                                    <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                
                                <asp:HyperLinkField DataNavigateUrlFields="sLink" 
                                    DataTextField="sTipoPesquisa" HeaderText="Tipo Pesquisa" 
                                    DataNavigateUrlFormatString="{0}">
                                    <HeaderStyle  Width="20%" />
                                    <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>                                
                        
                            </Columns>
                        </asp:GridView>
                          </div>
                        </div>
                        </div>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnMensagem" CssClass="row" runat="server">
                    <asp:Label ID="lblMensagem" class="warning" runat="server" Text="Label"></asp:Label>
                </asp:Panel>
            </div>
        </div>
        </ContentTemplate>            
    </asp:UpdatePanel>

<script>
    $(document).ready(function() {
        $('#cphCorpo_dtgvConsulta').DataTable({
            paging: false,
            info: false,
        });
    });
</script>
</asp:Content>
