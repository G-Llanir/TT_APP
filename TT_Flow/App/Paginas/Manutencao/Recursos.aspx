<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Recursos.aspx.cs" Inherits="TT_Hub.App.Paginas.Manutencao.Recursos" %>
<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
        <div id="DIV_ESPACO" style="height:100px" runat ="server" visible="false"></div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <script type="text/javascript">
                function MudarBotao() {
                    document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
                }
            </script>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1><asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label><small> Manutenção</small></h1>
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
                                    <asp:TextBox ID="txtPesquisa" class="form-control"  placeholder="Pesquisar" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-lg-3">
                                    <asp:DropDownList ID="ddlidTipoRecurso" runat="server" AutoPostBack="false" class="form-control yes_no select" > </asp:DropDownList>
                                </div>
                                <div class="col-lg-5">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click"  onclientclick="MudarBotao()" />
                                    <asp:Button ID="cmdNovo"      class="btn btn-success" runat="server" Text="Novo"  OnClick="cmdNovo_Click"/>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            
            
                <div class="col-lg-12">
                <uc1:MensagemPagina runat="server" id="MensagemPagina" />
                <asp:Panel ID="pnResultado" class="" runat="server">
                    <div id="resultado" >
                        <div class="panel panel-primary">
                        <div class="panel-body">
                            <div class="table-responsive">
                            <asp:GridView   ID="dtgvConsulta" class="table table-striped table-bordered table-hover " 
                                runat="server" Width="100%" CellSpacing="1"  CellPadding="1"   AutoGenerateColumns="False" GridLines="None"  
                                 ShowFooter="False"  Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                            <Columns>

                                <asp:HyperLinkField DataNavigateUrlFields="idRecurso" 
                                    DataTextField="idRecurso" HeaderText="ID" 
                                    DataNavigateUrlFormatString="Recursos_Detalhe.aspx?id={0}">
                                    <HeaderStyle  Width="8%" />
                                    <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>
                               
                                <asp:HyperLinkField DataNavigateUrlFields="idRecurso" 
                                    DataTextField="sDscRecurso" HeaderText="Recurso" 
                                    DataNavigateUrlFormatString="Recursos_Detalhe.aspx?id={0}">
                                    <HeaderStyle  Width="35%" />
                                    <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>
                                
                                <asp:BoundField DataField="sDscTipoRecuro" HeaderText="Tipo Recurso" >
                                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    <ItemStyle Width="20%"  HorizontalAlign="left" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:BoundField DataField="sSituacao_Completa" HeaderText="Ativo" >
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                    <ItemStyle Width="10%"  HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Atualização" >
                                    <ItemStyle Width="15%"  HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por" >
                                    <ItemStyle Width="18%"  HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:BoundField>
                        
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
