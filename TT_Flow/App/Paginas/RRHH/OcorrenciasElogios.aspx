<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="OcorrenciasElogios.aspx.cs" Inherits="TT_Hub.App.Paginas.RRHH.OcorrenciasElogios" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <script type="text/javascript">

                $(function () {
                    $('[id*=txtdtEvento]').datepicker({
                        autoclose: true,
                        format: 'dd/mm/yyyy',
                        language: 'pt-BR'
                    });

                    $('[id*=txtdtEvento]').mask('99/99/9999');
                });

            </script>

            <script type="text/javascript">
                function MudarBotao() {
                    document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
                }
            </script>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label>
                        <small>
                            <asp:Label ID="lblSubTituloPagina" runat="server" Text=" Consulta"></asp:Label></small>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>
                <div class="col-lg-12" runat="server" id="DIV_Filtro">

                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa</h3>
                        </div>
                        <div class="panel-body">

                            <div class="row">

                                <div class="col-lg-1">
                                    <div class="form-group row">
                                        <div class="col-lg-12">
                                            <asp:TextBox ID="txtdtEventoInicio" class="form-control CaixaTextoData" placeholder="Data Inicial" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-1">
                                    <div class="form-group row">
                                        <div class="col-lg-12">
                                            <asp:TextBox ID="txtdtEventoFinal" class="form-control CaixaTextoData" placeholder="Data Final" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlidColaborador" runat="server" class="Caixa_Selecao form-control"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2" id="div_Advertencia" runat="server">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlAdvertencia" class="form-control" runat="server">
                                            <asp:ListItem Selected="True" Value="">Todos os Tipos</asp:ListItem>
                                            <asp:ListItem Value="A">Advertência</asp:ListItem>
                                            <asp:ListItem Value="O">Ocorrência</asp:ListItem>
                                            <asp:ListItem Value="E">Elogio</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-3 form-group">
                                    <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                </div>

                                <div class="col-lg-2 form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                    <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" OnClick="cmdNovo_Click" />
                                </div>

                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <div class="col-lg-12" style="padding:0px" >
                <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
            </div>
            <asp:Panel ID="pnResultado" class="" runat="server">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <div class="table-responsive">
                            <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">
                                <Columns>

                                    <asp:HyperLinkField DataNavigateUrlFields="idRegistroEvento"
                                        DataTextField="idRegistroEvento" HeaderText="ID"
                                        DataNavigateUrlFormatString="OcorrenciasElogios_Detalhe.aspx?id={0}">
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="8%" />
                                    </asp:HyperLinkField>

                                    <asp:BoundField DataField="dtEvento" HeaderText="Data Evento">
                                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sTipo" HeaderText="Tipo">
                                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>


                                    <asp:HyperLinkField DataNavigateUrlFields="idRegistroEvento"
                                        DataTextField="sDscColaborador" HeaderText="Colaborador"
                                        DataNavigateUrlFormatString="OcorrenciasElogios_Detalhe.aspx?id={0}">
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"  Width="15%" />
                                    </asp:HyperLinkField>

                                    <asp:BoundField DataField="sDscEvento" HeaderText="Descrição">
                                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sDscGravidade" HeaderText="Gravidade">
                                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sTipoEvento" HeaderText="Local">
                                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>





                                </Columns>
                                <FooterStyle CssClass="TAB_Fundo_Azul" />
                                <RowStyle CssClass="texto_padrao_preto" />
                                <PagerStyle BackColor="Black" ForeColor="Black" HorizontalAlign="Center" />
                                <HeaderStyle CssClass="TAB_Fundo_Azul" />
                                <EditRowStyle BackColor="#2461BF" />

                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <%--<asp:Panel ID="pnMensagem" CssClass="row" runat="server">
                <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
            </asp:Panel>--%>

            </div>
        </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
