<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Ocorrencia.aspx.cs" Inherits="TT_Hub.App.Paginas.Ocorrencia" %>
<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

        <script src="https://login.tecandtec.com.br/app/js/jquery-1.9.2.min.js" type="text/javascript"></script>
        <script> $v192 = jQuery.noConflict();</script>
    <script src="/app/js/vanilla-masker.js"></script>

            <script type="text/javascript">

                function() {
                    VMasker(document.getElementById("ContentPlaceHolder1_txtdtInicio")).maskPattern('99/99/9999');
                }

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
                                     <div class="row">
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtdtInicio" class="form-control CaixaTextoData" placeholder="Data Inicial" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                            </div>
                                        </div>
					                    <div class="col-lg-6">
                                            <div class="form-group">
                                              <asp:TextBox ID="txtdtFinal" class="form-control CaixaTextoData " runat="server" placeholder="Data Final" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>

                                </div>

                                <div class="col-lg-4">
								    <div class="form-group">
									    <asp:DropDownList ID="ddlCliente" runat="server" AutoPostBack="true" class="form-control yes_no select"  OnSelectedIndexChanged="ddlCliente_SelectedIndexChanged" > </asp:DropDownList>
								    </div>
							    </div>

                                <div class="col-lg-4">
								    <div class="form-group">
									    <asp:DropDownList ID="ddlEquipamento" runat="server" AutoPostBack="false" class="form-control yes_no select"  > </asp:DropDownList>
								    </div>
							    </div>

                                <div class="col-lg-4">
								    <div class="form-group">
									    <asp:DropDownList ID="ddlTIPO_STATUS" runat="server" AutoPostBack="false" class="form-control yes_no select" > </asp:DropDownList>
								    </div>
							    </div>

                                <div class="col-lg-4">
								    <div class="form-group">
                                	    <asp:DropDownList ID="ddlStatusOcorrencia" runat="server" AutoPostBack="false" class="form-control yes_no select"  > 
                                            <asp:ListItem Value="TODOS">Todas as Ocorrências</asp:ListItem>
                                            <asp:ListItem Value="EM ABERTO" Selected="True">Em Aberto</asp:ListItem>
                                            <asp:ListItem Value="FINALIZADA">Ecerradas</asp:ListItem>
                                        </asp:DropDownList>
							        </div>
							    </div>
                                </ContentTemplate>            
                            </asp:UpdatePanel>
                              
                          
                            <div class="col-lg-12">
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
                            <asp:HyperLinkField DataNavigateUrlFields="idOcorrencia" 
                                DataTextField="idoCorrencia" HeaderText="ID" 
                                DataNavigateUrlFormatString="/app/Paginas/Consulta/Ocorrencia_Detalhe.aspx?id={0}">
                                <HeaderStyle  Width="6%" />
                                <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                            </asp:HyperLinkField>

                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="dtEvento"  HeaderText="Dt Ocorrência" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="15%" />
                            </asp:BoundField>

                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscTipoReduzido"  HeaderText="Ocorrência" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="8%" />
                            </asp:BoundField>

                            <asp:BoundField  DataField="sDscEventoCompleto"  HeaderText="Evento" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            
                                <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscCliente"  HeaderText="Cliente" HtmlEncode="False" HtmlEncodeFormatString="False">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" Width="18%" />
                                </asp:BoundField>
                            
                                <asp:HyperLinkField DataNavigateUrlFields="idEquipamento" 
                                    DataTextField="sDescricao" HeaderText="Equipamento" 
                                    DataNavigateUrlFormatString="/app/Paginas/Manutencao/Equipamentos_Detalhe.aspx?id={0}&pag=db">
                                    <HeaderStyle  Width="18%" />
                                    <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>
                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="dtResolucao"  HeaderText="Dt Resolução" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="14%" />
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
