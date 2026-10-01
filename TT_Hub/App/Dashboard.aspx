<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="TT_Hub.App.Dashboard" %>
<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
        <style>
           .caixaBotao {
  
            } 
           .caixaBotao:hover
           {
                box-shadow: 3px 3px 7px #337ab7, -3px -3px 7px #337ab7;
                -webkit-transition : box-shadow ease-out 0.1s;
                transition : box-shadow ease-out 0.1s;        
           }

        </style>
                

   
     <script type="text/javascript">
         $(function () {
             $('[id*=lstidCliente]').multiselect({
                 buttonWidth: '195px',
                 includeSelectAllOption: true,
                 maxHeight: 300,
                 dropRight: true,
                 nSelectedText: ' - Clientes Selecionados!',
                 allSelectedText: 'Todos os Clientes',
                 enableFiltering: false
             });
         });
     </script>


         <div class="row">
            <div class="col-lg-9">
                <h1>Dashboard <small> Painel Administrativo</small> </h1>
            </div>
            <div class="col-lg-3">
                <div class="form-inline">
                    <br />
                    <div class="form-group">
                    <asp:ListBox ID="lstidCliente" runat="server" SelectionMode="Multiple" > </asp:ListBox>
                    <asp:LinkButton ID="lnkAtualizar" CssClass="btn btn-small btn-circle" runat="server" OnClick="cmdAtualizar_Click"><i class="fa-refresh fa"></i></asp:LinkButton>
                    </div>
                </div>
            </div>
       
             
        </div><!-- /.row -->
    
        <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina"  NivelPagina="1" TitulodaPagina="DashBoard"/>
        
    <asp:UpdatePanel ID="upDashboard" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
        <div class="row">
            <div id="div_Resumo" runat="server">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bell-o"></i> Resumo</h3>
                </div>
                <div class="panel-body">
                <div class="row">
                <asp:Repeater ID="rptBotoes" runat="server">
                <ItemTemplate>
                    <div class="col-lg-<%#DataBinder.Eval(Container.DataItem, "nQtdColunas") %>  ">
                        <a href="<%#DataBinder.Eval(Container.DataItem, "sLink") %>">
                            <div class="panel panel-<%#DataBinder.Eval(Container.DataItem, "sCor") %> caixaBotao">
                              <div class="panel-heading">
                                <div class="row">
                                  <div class="col-xs-12 text-center">
                                    <p class="announcement-heading"><%#DataBinder.Eval(Container.DataItem, "nQuantidade") %></p>
                                    <p class="announcement-text"><%#DataBinder.Eval(Container.DataItem, "sDscAgrupador_Linha1") %></p>
                                    <p class="announcement-text"><%#DataBinder.Eval(Container.DataItem, "sDscAgrupador_Linha2") %></p>
                                  </div>
                                </div>
                              </div>
                            </div>
                        </a>
                    </div>


                </ItemTemplate>
            </asp:Repeater>
                </div><!-- /.row -->
                </div>
            </div>
            </div>

            <div id="div_Monitoramento" runat="server" class="col-lg-4">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bell-o"></i> Monitoramento Conectividade</h3>
                </div>
                <div class="panel-body">
                <div id="div_gvMonitoramento" class="table-responsive">
        
                    <asp:GridView ID="gvMonitoramento" runat="server" AutoGenerateColumns="false" CellPadding="0" CellSpacing="0" 
                        class="table table-striped table-bordered table-hover table-condensed table-responsive" GridLines="None" 
                          ShowFooter="False"  Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvMonitoramento_RowDataBound" >
                            <Columns>
                            <asp:HyperLinkField DataNavigateUrlFields="idCliente, idUnidade" 
                                DataTextField="sDscAgrupador" HeaderText="Unidade" 
                                DataNavigateUrlFormatString="./Paginas/Consulta/Monitoramento_Conectividade.aspx?idCliente={0}&idUnidade={1}&sFuncao=DashBoard">
                                <HeaderStyle  Width="40%" />
                                <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                            </asp:HyperLinkField>

                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="nTotalEquipamentos"  HeaderText="Qtd" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="center" />
                                <ItemStyle HorizontalAlign="center" Width="10%" />
                            </asp:BoundField>

                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="nTotalOK"  HeaderText="OK" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="center" />
                                <ItemStyle HorizontalAlign="center" Width="10%" />
                            </asp:BoundField>
                            
                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="nTotalNOK"  HeaderText="Err" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" Width="10%" />
                            </asp:BoundField>
                        </Columns>
                    </asp:GridView>
                </div>
              </div>
                </div>
            </div>
           

        </div>
        <div class="row">


          <div class="col-lg-12">


            <div class="panel panel-primary">
              <div class="panel-heading">
                <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Últimos Eventos</h3>
              </div>
              <div class="panel-body">
                <div id="UltimosEventos" class="table-responsive">
        
                    <asp:GridView ID="gvUltimasOcorrencias" runat="server" AutoGenerateColumns="false" CellPadding="0" CellSpacing="1" 
                        class="table table-striped table-bordered table-hover table-condensed table-responsive" GridLines="None" 
                          ShowFooter="False"  Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvUltimasOcorrencias_RowDataBound">
                      <%--  <FooterStyle CssClass="TAB_Fundo_Azul" />
                        <RowStyle CssClass="texto_padrao_preto" />--%>
                        <Columns>
                            <asp:HyperLinkField DataNavigateUrlFields="idOcorrencia" 
                                DataTextField="idoCorrencia" HeaderText="ID" 
                                DataNavigateUrlFormatString="./Paginas/Consulta/Ocorrencia_Detalhe.aspx?id={0}">
                                <HeaderStyle  Width="4%" />
                                <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                            </asp:HyperLinkField>

                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="dtEvento"  HeaderText="Dt Ocorrência" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="14%" />
                            </asp:BoundField>

                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscTipoReduzido"  HeaderText="Ocorrência" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="6%" />
                            </asp:BoundField>
                            
                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscCliente"  HeaderText="Cliente" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="18%" />
                            </asp:BoundField>
                            
                            <asp:HyperLinkField DataNavigateUrlFields="idEquipamento" 
                                                DataTextField="sDescricao" HeaderText="Equipamento" 
                                                DataNavigateUrlFormatString="./Paginas/Manutencao/Equipamentos_Detalhe.aspx?id={0}&pag=db">
                                <HeaderStyle  Width="18%" />
                                <ItemStyle  HorizontalAlign="Left" VerticalAlign="Middle" />
                            </asp:HyperLinkField>

                            <asp:BoundField  DataField="sDscEventoCompleto"  HeaderText="Evento" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>


                            <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscTipoEquipamento"  HeaderText="Tipo Equipamento" HtmlEncode="False" HtmlEncodeFormatString="False">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="10%" />
                            </asp:BoundField>
                            
                        </Columns>
                <%--        <FooterStyle CssClass="TAB_Fundo_Azul" />
                        <RowStyle CssClass="texto_padrao_preto" />
                        <PagerStyle BackColor="Black" ForeColor="Black" HorizontalAlign="Center" />
                        <HeaderStyle CssClass="TAB_Fundo_Azul" />
                        <EditRowStyle BackColor="#2461BF" />--%>
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
                <h3 class="panel-title"><i class="fa fa-long-arrow-right"></i> Tipos de Equipametos</h3>
              </div>
              <div class="panel-body">
                <div id="GraficoEquipamentos"></div>
                <div class="text-right">
                  
                </div>
              </div>
            </div>
          </div>

		  <div class="col-lg-4">
            <div class="panel panel-primary">
              <div class="panel-heading">
                <h3 class="panel-title"><i class="fa fa-long-arrow-right"></i> Resumo Eventos</h3>
              </div>
              <div class="panel-body">
                <div id="ResumoEventos">
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






</asp:Content>

