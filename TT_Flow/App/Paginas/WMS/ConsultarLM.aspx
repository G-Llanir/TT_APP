<%@ Page Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="ConsultarLM.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.ConsultarLM" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="LM Pendente de OPI"></asp:Label>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">
            <asp:Panel ID="pnLMConsulta" class="" runat="server">
                <div class="panel panel-primary" id="div_filtroPesquisa" runat="server">
                    <div class="panel-heading">
                        <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;Filtro para pesquisa </h3>

                    </div>
                    <div class="panel-body ">
                        <div class="form-group">

                            <div class="col-lg-1">
                                <div class="form-group">
                                    <label>Data Início</label>
                                    <asp:TextBox ID="txtdtInicio" class="form-control" runat="server" TextMode="Date" Style="inline-size: 120px;"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-1">
                                <div class="form-group">
                                    <label>Data Final</label>
                                    <asp:TextBox ID="txtdtFinal" class="form-control" runat="server" TextMode="Date" Style="inline-size: 120px;"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-2">
                                <div class="form-group">
                                    <label>Tipo LM</label>
                                    <asp:DropDownList runat="server" ID="ddlTipoLM" class="form-control Caixa_Selecao">
                                        <asp:ListItem Text="Todas as LMs" Value="0" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="LME"  Value="1"></asp:ListItem>
                                        <asp:ListItem Text="LMO"  Value="2"></asp:ListItem>
                                        <asp:ListItem Text="LMRO" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="LMCO" Value="4"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

<%--                            <div class="col-lg-4">
                                <div class="form-group">
                                    <label>Responsável</label>
                                    <asp:DropDownList runat="server" ID="ddlUsuarioResponsavel" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>
                            </div>--%>

                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" Style="margin-top: 24px;" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-lg-12">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                        <div class="panel panel-primary" id="div_gvConsulta" runat="server">
                            <div class="panel-body">
                                <div class="table-responsive">
                                    <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover table-responsive table-condensed" DataKeyNames="idLM"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                        <Columns>


                                            <asp:HyperLinkField DataNavigateUrlFields="idPedido, idLM, sTipoLM"
                                                DataTextField="idLM" HeaderText="LM" Target="_blank"
                                                DataNavigateUrlFormatString="/App/Paginas/Pedidos_Detalhe.aspx?id={0}&sTp=2&sSession=S&idLM={1}&sTipoLM={2}">
                                                <HeaderStyle Width="6%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:BoundField DataField="dtLM" HeaderText="Data">
                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sTipoLM" HeaderText="Tipo">
                                                <ItemStyle Width="4%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nControleTT" HeaderText="Controle TT">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sRazaoSocial" HeaderText="Cliente">
                                                <ItemStyle Width="14%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idPedido, idLM, sTipoLM"
                                                DataTextField="sDscLM" HeaderText="Descrição da LM" Target="_blank"
                                                DataNavigateUrlFormatString="/App/Paginas/Pedidos_Detalhe.aspx?id={0}&sTp=2&sSession=S&idLM={1}&sTipoLM={2}">
                                                <HeaderStyle Width="18%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

<%--                                            <asp:BoundField DataField="sReferencia" HeaderText="Referência Pedido">
                                                <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>--%>
                                            <asp:BoundField DataField="sDscStatus" HeaderText="Status Pedido">
                                                <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle"  />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="dtPrevisaoEntrega" HeaderText="Prev Entrega" DataFormatString="{0:dd/MM/yyyy}">
                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle"  />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="sDscUsuario" HeaderText="Autor LM" >
                                                <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle"  />
                                            </asp:BoundField>



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
</asp:Content>
