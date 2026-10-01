<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="CentroCusto.aspx.cs" Inherits="TT_Hub.App.Paginas.Adm.Manutencao.CentroCusto" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <script type="text/javascript">

        function MudarBotao() {
            document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
        }

    </script>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Centro de Custo"></asp:Label><small> Consulta</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;Filtro para pesquisa </h3>
                </div>
                <div class="panel-body ">
                    <div class="form-group">

                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-2">
                            <div class="form-group">
                                <asp:DropDownList runat="server" ID="ddlsTipo" class="form-control Caixa_Selecao">
                       <%--             <asp:ListItem Text="Todos os Tipos" Value=""></asp:ListItem>
                                    <asp:ListItem Text="Interno" Value="I"></asp:ListItem>
                                    <asp:ListItem Text="Clientes" Value="C"></asp:ListItem>--%>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-2">
                            <div class="form-group">
                                <asp:DropDownList runat="server" ID="ddlsSituacao" class="form-control">
                                    <asp:ListItem Text="Ativos e Inativos" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Ativos" Value="S" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Inativos" Value="N"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-3">
                            <div class="form-group">
                                <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo Centro de Custo" OnClick="cmdNovo_Click" Style="margin-left: 5px;" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    <asp:Panel ID="pnResultado" class="" runat="server">
                        <asp:Timer ID="Timer1" runat="server" Interval="60000" OnTick="Timer1_Tick"></asp:Timer>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <div id="resultado">
                                    <div class="panel panel-primary">
                                        <div class="panel-body">
                                            <div class="table-responsive">
                                                <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">

                                                    <Columns>

                                                        <asp:HyperLinkField DataNavigateUrlFields="idCentroDeCusto"
                                                            DataTextField="idCentroDeCusto" HeaderText="ID"
                                                            DataNavigateUrlFormatString="CentroCusto_Detalhe.aspx?id={0}">
                                                            <HeaderStyle Width="1%" />
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:HyperLinkField>

                                                        <asp:HyperLinkField DataNavigateUrlFields="idCentroDeCusto"
                                                            DataTextField="sTipoCompleto" HeaderText="Tipo"
                                                            DataNavigateUrlFormatString="CentroCusto_Detalhe.aspx?id={0}">
                                                            <HeaderStyle Width="5%" />
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:HyperLinkField>

                                                        <asp:HyperLinkField DataNavigateUrlFields="idCentroDeCusto"
                                                            DataTextField="sCodCC" HeaderText="Código"
                                                            DataNavigateUrlFormatString="CentroCusto_Detalhe.aspx?id={0}">
                                                            <HeaderStyle Width="8%" />
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:HyperLinkField>

                                                        <asp:HyperLinkField DataNavigateUrlFields="idCentroDeCusto"
                                                            DataTextField="sDescricao" HeaderText="Descrição"
                                                            DataNavigateUrlFormatString="CentroCusto_Detalhe.aspx?id={0}">
                                                            <HeaderStyle Width="20%" />
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:HyperLinkField>

                                                        <asp:BoundField DataField="nTetoGasto" HeaderText="Teto" DataFormatString="{0:C2}">
                                                            <ItemStyle Width="8%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="nSaldoGasto" HeaderText="Disponivel">
                                                            <ItemStyle Width="8%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                         <asp:BoundField DataField="nReceitas" HeaderText="Total Receita">
                                                             <ItemStyle Width="8%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                         </asp:BoundField>
                                                      
                                                        <asp:BoundField DataField="nTotalDespesas" HeaderText="Total Despesa">
                                                            <ItemStyle Width="8%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                      
                                                        <asp:BoundField DataField="nReceitaLiquida" HeaderText="Total Líquido">
                                                            <ItemStyle Width="8%" HorizontalAlign="Right" VerticalAlign="Middle" Font-Bold="true" />
                                                        </asp:BoundField>
                                                        
                                                        <asp:BoundField DataField="nPorcentagem" HeaderText="%">
                                                            <ItemStyle Width="5%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        


                                                        <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Atualização">
                                                            <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                    </Columns>

                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="Timer1" EventName="Tick" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </asp:Panel>

                </div>
            </div>

        </div>

    </div>

    <script>
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            var table = $('#' + '<%= dtgvConsulta.ClientID %>').DataTable();
            if (table) {
                table.destroy();
            }
            $('#' + '<%= dtgvConsulta.ClientID %>').DataTable({
                responsive: true,
                paging: true,
                scrollCollapse: true,
                scrollX: false,
                scrollY: '',
                pageLength: 50,
                searching: true,
                info: false,
                bLengthChange: true,
                order: [[0, 'asc']],
                language: { url: 'https://cdn.datatables.net/plug-ins/1.11.5/i18n/pt-BR.json' }
            });
        });

    </script>

</asp:Content>
