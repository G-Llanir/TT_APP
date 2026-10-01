<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Monitoramento_Conectividade.aspx.cs" Inherits="TT_Hub.App.Paginas.Monitoramento_Conectividade" %>
<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script src="https://login.tecandtec.com.br/app/js/jquery-1.9.2.min.js" type="text/javascript"></script>
    <script> $v192 = jQuery.noConflict();</script>

    <script type="text/javascript">

        function MudarBotao() {
            document.getElementById("ContentPlaceHolder1_cmdPesquisar").value = "Pesquisando..";
        }
                
    </script>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1><asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label><small> Consulta</small></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina"  NivelPagina="2" TitulodaPagina=""/>
                </div>
                <div class="col-lg-12" runat="server" id="DIV_Filtro">
                
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa </h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group">
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                    <ContentTemplate>
                                        <div class="col-lg-4">
								            <div class="form-group">
									            <asp:DropDownList ID="ddlCliente" runat="server" AutoPostBack="true" class="form-control yes_no select"  OnSelectedIndexChanged="ddlCliente_SelectedIndexChanged" > </asp:DropDownList>
								            </div>
							            </div>

                                        <div class="col-lg-4">
								            <div class="form-group">
									            <asp:DropDownList ID="ddlUnidade" runat="server" AutoPostBack="false" class="form-control yes_no select" > </asp:DropDownList>
								            </div>
							            </div>

                                         <div class="col-lg-4">
                                            <div class="form-group">
                                                <asp:DropDownList ID="ddlidTipoEquipamento" class="form-control " placeholder="Tipo de Equipamento" runat="server" attrname="Tipo de Equipamento"></asp:DropDownList>
                                            </div>
                                        </div>

                                    </ContentTemplate>            
                                </asp:UpdatePanel>
                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar"   onclientclick="MudarBotao()" OnClick="cmdPesquisar_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                
                
                <div class="col-lg-12">
                
                <asp:Panel ID="pnResultado" class="" runat="server">
                    <div class="panel panel-primary">
                        <div class="panel-body">
                        <div class="table-responsive">
                            <asp:GridView ID="gvOcorrencias" runat="server" AutoGenerateColumns="false" CellPadding="0" CellSpacing="0" 
                                class="table table-striped table-bordered table-hover table-condensed table-responsive tablesorter" GridLines="None" 
                                  ShowFooter="False"  Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvOcorrencias_RowDataBound">
                            <Columns>
                            <asp:HyperLinkField DataNavigateUrlFields="idEquipamento" 
                                DataTextField="idEquipamento" HeaderText="ID" 
                                DataNavigateUrlFormatString="../Manutencao/Equipamentos_Detalhe.aspx?id={0}&pag=db">
                                <HeaderStyle  Width="5%" />
                                <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                            </asp:HyperLinkField>

                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="dtAtualizacao"  HeaderText="Dt Atualização" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="12%" />
                            </asp:BoundField>

                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscAgrupador"  HeaderText="Cliente/Unidade" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="18%" />
                            </asp:BoundField>
                            <asp:HyperLinkField DataNavigateUrlFields="idEquipamento" 
                                DataTextField="sDescricao" HeaderText="Equipamento" 
                                DataNavigateUrlFormatString="../Manutencao/Equipamentos_Detalhe.aspx?id={0}&pag=db">
                                <HeaderStyle  Width="20%" />
                                <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                            </asp:HyperLinkField>

                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscTipoEquipamento_Reduzido"  HeaderText="Equipamento" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="10%" />
                            </asp:BoundField>
                                
                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="sEnderecoIP"  HeaderText="Endereço" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="10%" />
                            </asp:BoundField>
                                
                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="sStatusPing"  HeaderText="Status" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="8%" />
                            </asp:BoundField>
                            
                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="nTempoPing"  HeaderText="Tempo" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="8%" />
                            </asp:BoundField>
                            
          

                        </Columns>

                        </asp:GridView>
                        </div>
                        </div>
                    </div>
                    
                </asp:Panel>
                <asp:Panel ID="pnMensagem" CssClass="row" runat="server">
                     <div class="col-lg-12">
                    <asp:Label ID="lblMensagem" class="warning" runat="server" Text="Label"></asp:Label>
                         </div>
                </asp:Panel>
                </div>
            </div>
    



</asp:Content>
