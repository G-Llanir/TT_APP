    <%@ Page Title="" Language="C#" MasterPageFile="~/app/site.master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Dashboard.aspx.cs" Inherits="TT_Colaborador.Dashboard" %>
<%@ Register Src="~/Aplicativo/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb_Pagina" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" Runat="Server">
    
         <script type="text/javascript">
         $(function () {
             $('[id*=lstidFluxo]').multiselect({
                 buttonWidth: '195px',
                 includeSelectAllOption: true,
                 maxHeight: 300,
                 dropRight: true,
                 nSelectedText: ' - Fluxos Selecionados!',
                 allSelectedText: 'Todos os Fluxos',
                 enableFiltering: false
             });
         });
         </script>

    <div class="row">
        <div class="col-lg-9">
            <h1>Dashboard <small> T-Flow - Painel Administrativo</small> </h1>
        </div>
        <div class="col-lg-3">
            <div class="form-inline">
                <br />
                <div class="form-group">
                <asp:ListBox ID="lstidFluxo" runat="server" SelectionMode="Multiple" > </asp:ListBox>
                <asp:LinkButton ID="lnkAtualizar" CssClass="btn btn-small btn-circle" runat="server" OnClick="cmdAtualizar_Click"><i class="fa-refresh fa"></i></asp:LinkButton>
                </div>
            </div>
        </div> 
        <div class="col-lg-12">
            <uc1:BreadCrumb_Pagina runat="server" ID="BreadCrumb_Pagina"  NivelPagina="1" TitulodaPagina="DashBoard"/>
        </div>
    </div><!-- /.row -->

        <asp:UpdatePanel ID="upDashboard" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
           <div class="row">
          <div class="col-lg-12">
            <div class="panel panel-primary">
              <div class="panel-heading">
                    <div class="row">
                        <div class="col-lg-10">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Resumo de Pedidos</h3>
                        </div>
                        <div class="col-lg-2">
                           	<asp:DropDownList ID="ddlsClienteEspecial" runat="server" AutoPostBack="true" class="form-control yes_no select" OnSelectedIndexChanged="ddlsClienteEspecial_SelectedIndexChanged"  > 
                                <asp:ListItem Selected="True" Value="T">Todos os Clientes</asp:ListItem>
                                <asp:ListItem Value="N">Excluir Clientes Visualização Diferenciada</asp:ListItem>
                                <asp:ListItem Value="S">Apenas Clientes Visualização Diferenciada</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                  
                  </div>
              <div class="panel-body">
        <div class="row">
            <div class="col-lg-3">
                <asp:HyperLink ID="lnkTotalPedidos" runat="server">
                <div class="panel panel-success ">
                  <div class="panel-heading">
                  
                    <div class="row">
                      <div class="col-xs-6">
                        <i class="fa fa-comments fa-5x"></i>
                      </div>
                      <div class="col-xs-6 text-right">
                        <p class="announcement-heading"><asp:Label ID="lblTotalPedidos" runat="server" Text=""></asp:Label></p>
                        <p class="announcement-text"> Total de Pedidos</p>
                      </div>
                    </div>
                  </div>
              
                    <div class="panel-footer announcement-bottom">
                      <div class="row">
                        <div class="col-xs-6">
                          Ver 
                        </div>
                        <div class="col-xs-6 text-right">
                          <i class="fa fa-arrow-circle-right"></i>
                        </div>
                      </div>
                    </div>
             
                </div>
                
                </asp:HyperLink>
              </div>

            <div class="col-lg-3">
                <asp:HyperLink ID="lnktotalSeuDepartamento" runat="server">
                <div class="panel panel-warning">
                    <div class="panel-heading">
                        <div class="row">
                            <div class="col-xs-6">
                                <i class="fa fa-tasks fa-5x"></i>
                            </div>
                            <div class="col-xs-6 text-right">
                                <p class="announcement-heading"><asp:Label ID="lbltotalSeuDepartamento" runat="server" Text=""></asp:Label></p>
                                <p class="announcement-text">Pedidos em seu Depto</p>
                            </div>
                        </div>
                    </div>
                    <div class="panel-footer announcement-bottom">
                        <div class="row">
                            <div class="col-xs-6">
                                Ver
                            </div>
                            <div class="col-xs-6 text-right">
                                <i class="fa fa-arrow-circle-right"></i>
                            </div>
                        </div>
                    </div>
                </div>
                </asp:HyperLink>
            </div>

            <div class="col-lg-3">
               <asp:HyperLink ID="lnkTotalVencerSeteDias" runat="server">
                <div class="panel panel-info">
                    <div class="panel-heading">
                        <div class="row">
                            <div class="col-xs-6">
                                <i class="fa fa-comments fa-5x"></i>
                            </div>
                            <div class="col-xs-6 text-right">
                                <p class="announcement-heading"><asp:Label ID="lblTotalVencerSeteDias" runat="server" Text=""></asp:Label></p>
                                <p class="announcement-text">Vencendo em 7 Dias</p>
                            </div>
                        </div>
                    </div>
                    <div class="panel-footer announcement-bottom">
                        <div class="row">
                            <div class="col-xs-6">
                                Ver 
                            </div>
                            <div class="col-xs-6 text-right">
                                <i class="fa fa-arrow-circle-right"></i>
                            </div>
                        </div>
                    </div>
                </div>
                </asp:HyperLink>
            </div>
            
            <div class="col-lg-3">
               <asp:HyperLink ID="lnkTotalemAtraso" runat="server">
                <div class="panel panel-danger">
                    <div class="panel-heading">
                        <div class="row">
                            <div class="col-xs-6">
                                <i class="fa fa-check fa-5x"></i>
                            </div>
                            <div class="col-xs-6 text-right">
                                <p class="announcement-heading"><asp:Label ID="lblTotalemAtraso" runat="server" Text=""></asp:Label></p>
                                <p class="announcement-text">Pedidos em Atraso</p>
                            </div>
                        </div>
                    </div>
                    <div class="panel-footer announcement-bottom">
                        <div class="row">
                            <div class="col-xs-6">
                                Ver 
                            </div>
                            <div class="col-xs-6 text-right">
                                <i class="fa fa-arrow-circle-right"></i>
                            </div>
                        </div>
                    </div>
                </div>
                </asp:HyperLink>
            </div>
        </div><!-- /.row -->
                             </div>
            </div>
            </div>
        </div>
   <div class="row">
          <div class="col-lg-12">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Tarefas</h3>
              </div>
              <div class="panel-body">
        <div class="row">
            <div class="col-lg-3">
                <asp:HyperLink ID="lnkTarefas_TotalUsuario" runat="server">
                <div class="panel panel-success ">
                  <div class="panel-heading">
                  
                    <div class="row">
                      <div class="col-xs-6">
                        <i class="fa fa-comments fa-5x"></i>
                      </div>
                      <div class="col-xs-6 text-right">
                        <p class="announcement-heading"><asp:Label ID="lblTarefas_TotalUsuario" runat="server" Text=""></asp:Label></p>
                        <p class="announcement-text"> Suas Tarefas</p>
                      </div>
                    </div>
                  </div>
              
                    <div class="panel-footer announcement-bottom">
                      <div class="row">
                        <div class="col-xs-6">
                          Ver 
                        </div>
                        <div class="col-xs-6 text-right">
                          <i class="fa fa-arrow-circle-right"></i>
                        </div>
                      </div>
                    </div>
             
                </div>
                
                </asp:HyperLink>
              </div>

            <div class="col-lg-3">
                <asp:HyperLink ID="lnkTarefas_TotalDepartamento" runat="server">
                <div class="panel panel-warning">
                    <div class="panel-heading">
                        <div class="row">
                            <div class="col-xs-6">
                                <i class="fa fa-tasks fa-5x"></i>
                            </div>
                            <div class="col-xs-6 text-right">
                                <p class="announcement-heading"><asp:Label ID="lblTarefas_TotalDepartamento" runat="server" Text=""></asp:Label></p>
                                <p class="announcement-text">Tarefas seu Depto</p>
                            </div>
                        </div>
                    </div>
                    <div class="panel-footer announcement-bottom">
                        <div class="row">
                            <div class="col-xs-6">
                                Ver
                            </div>
                            <div class="col-xs-6 text-right">
                                <i class="fa fa-arrow-circle-right"></i>
                            </div>
                        </div>
                    </div>
                </div>
                </asp:HyperLink>
            </div>
        </div><!-- /.row -->
            </div>
            </div>
            </div>
        </div>

        <div class="row">
          <div class="col-lg-12">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Pedidos</h3>
              </div>
              <div class="panel-body">
                <div id="UltimosEventos" class="table-responsive">
        
                    <asp:GridView ID="gvUltimosPedidos" runat="server" AutoGenerateColumns="false" CellPadding="0" CellSpacing="1" 
                        class="table table-striped table-bordered table-hover table-condensed table-responsive" GridLines="None" 
                          ShowFooter="False"  Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvUltimasOcorrencias_RowDataBound">
                        <Columns>

<%--                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="nDias"  HeaderText="dtPrevisaoEntrega_Original" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="10%" />
                            </asp:BoundField>--%>

                            <asp:HyperLinkField DataNavigateUrlFields="idPedido" 
                                DataTextField="idPedido" HeaderText="Pedido" 
                                DataNavigateUrlFormatString="Paginas/Pedidos_Detalhe.aspx?id={0}">
                                <HeaderStyle  Width="8%" />
                                <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                            </asp:HyperLinkField>

                            <asp:HyperLinkField DataNavigateUrlFields="idPedido" 
                                DataTextField="sReferencia" HeaderText="Referência" 
                                DataNavigateUrlFormatString="Paginas/Pedidos_Detalhe.aspx?id={0}">
                                <HeaderStyle  Width="20%" />
                                <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                            </asp:HyperLinkField>

                                                        
                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscCliente"  HeaderText="Cliente" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Left"  VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="Left" Width="18%" />
                            </asp:BoundField>

                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="dtPedido"  HeaderText="Data Pedido" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="10%" />
                            </asp:BoundField>

                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscFluxo"  HeaderText="Fluxo" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Left"  VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="Left" Width="15%" />
                            </asp:BoundField>

                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscStatus"  HeaderText="Status" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="12%" />
                            </asp:BoundField>

                            
                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscDepartamento"  HeaderText="Departamento" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="12%" />
                            </asp:BoundField>

                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="dtEstimativaEntrega"  HeaderText="Estimativa Entrega" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="Center" Width="10%" />
                            </asp:BoundField>

                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="dtPrevisaoEntregaFormatada"  HeaderText="Previsão Entrega" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="Center" Width="12%" />
                            </asp:BoundField>
                        </Columns>
                    </asp:GridView>
                </div>
              </div>
            </div>
          </div>
        </div><!-- /.row -->
        <div class="row">
       <%-- 
          <div class="col-lg-4">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h3 class="panel-title"><i class="fa fa-long-arrow-right"></i> Traffic Sources: October 1, 2013 - October 31, 2013</h3>
              </div>
              <div class="panel-body">
                <div id="morris-chart-donut"></div>
                <div class="text-right">
                  <a href="#">View Details <i class="fa fa-arrow-circle-right"></i></a>
                </div>
              </div>
            </div>
          </div>
          <div class="col-lg-4">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h3 class="panel-title"><i class="fa fa-clock-o"></i> Recent Activity</h3>
              </div>
              <div class="panel-body">
                <div class="list-group">
                  <a href="#" class="list-group-item">
                    <span class="badge">just now</span>
                    <i class="fa fa-calendar"></i> Calendar updated
                  </a>
                  <a href="#" class="list-group-item">
                    <span class="badge">4 minutes ago</span>
                    <i class="fa fa-comment"></i> Commented on a post
                  </a>
                  <a href="#" class="list-group-item">
                    <span class="badge">23 minutes ago</span>
                    <i class="fa fa-truck"></i> Order 392 shipped
                  </a>
                  <a href="#" class="list-group-item">
                    <span class="badge">46 minutes ago</span>
                    <i class="fa fa-money"></i> Invoice 653 has been paid
                  </a>
                  <a href="#" class="list-group-item">
                    <span class="badge">1 hour ago</span>
                    <i class="fa fa-user"></i> A new user has been added
                  </a>
                  <a href="#" class="list-group-item">
                    <span class="badge">2 hours ago</span>
                    <i class="fa fa-check"></i> Completed task: "pick up dry cleaning"
                  </a>
                  <a href="#" class="list-group-item">
                    <span class="badge">yesterday</span>
                    <i class="fa fa-globe"></i> Saved the world
                  </a>
                  <a href="#" class="list-group-item">
                    <span class="badge">two days ago</span>
                    <i class="fa fa-check"></i> Completed task: "fix error on sales page"
                  </a>
                </div>
                <div class="text-right">
                  <a href="#">View All Activity <i class="fa fa-arrow-circle-right"></i></a>
                </div>
              </div>
            </div>
          </div>--%>
		  
          <div class="col-lg-4">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h3 class="panel-title"><i class="fa fa-long-arrow-right"></i> Pedidos por Departamento</h3>
              </div>
              <div class="panel-body">
                <div id="GraficoDepartamento"></div>
                <div class="text-right">
                  
                </div>
              </div>
            </div>
          </div>

		  <div class="col-lg-4">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h3 class="panel-title"><i class="fa fa-long-arrow-right"></i> Pedidos por Status</h3>
              </div>
              <div class="panel-body">
                <div id="GraficosStatus">
                </div>
                <div class="text-right">
                </div>
              </div>
            </div>
          </div>
		 
          
           <div class="col-lg-4">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h3 class="panel-title"><i class="fa fa-long-arrow-right"></i> Outros Avisos</h3>
              </div>
              <div class="panel-body">
                <div id="Grafico_Origem">
                  </div>
                <div class="text-right">
                  
                </div>
              </div>
            </div>
          </div>
        </div>
       
     
        </ContentTemplate>            
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="timer_Atualizar" EventName="Tick" />
            </Triggers>
    </asp:UpdatePanel>
    <asp:Timer ID="timer_Atualizar" runat="server" Interval="60000" 
        ontick="timer_Atualizar_Tick">
    </asp:Timer>

<%--<script>
    $(document).ready(function() {
        $('#cphCorpo_gvUltimosPedidos').DataTable({
            paging: false,
            info: false,
            columnDefs: [
            {
                target: 0,
                visible: false,
                searchable: true,
            },]
        });
    });
</script>--%>

</asp:Content>

