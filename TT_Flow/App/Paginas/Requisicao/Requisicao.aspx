<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Requisicao.aspx.cs" Inherits="TT_Hub.App.Paginas.Requisicao.Requisicao" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <asp:UpdatePanel ID="updPanelGeral" runat="server">
        <ContentTemplate>

            <script type="text/javascript">

                function MudarBotao() {
                    document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
                }

                $(function () {
                    $('[id*=txtdtInicial]').datepicker({
                        autoclose: true,
                        format: 'dd/mm/yyyy',
                        language: 'pt-BR'
                    });

                    $('[id*=txtdtFinal]').datepicker({
                        autoclose: true,
                        format: 'dd/mm/yyyy',
                        language: 'pt-BR'
                    });

                    $('[id*=txtdtInicial]').mask('99/99/9999');
                    $('[id*=txtdtFinal]').mask('99/99/9999');

                });

            </script>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Requisições"></asp:Label><small> Consulta</small></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa </h3>
                        </div>
                        <div class="panel-body ">
                            <div class="form-group">
                                <asp:UpdatePanel ID="updPagina" runat="server">
                                    <ContentTemplate>

                                        <div class="col-lg-1">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtdtInicial" class="form-control" placeholder="Data Inicial" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-1">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtdtFinal" class="form-control" placeholder="Data Final" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <asp:DropDownList ID="ddlidTipoRequisicao" class="form-control Caixa_Selecao" placeholder="Tipo de Requisição" runat="server" attrname="Tipo de Requisição"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <asp:DropDownList ID="ddlidDepartamento" class="form-control Caixa_Selecao" placeholder="Departamento Solicitante" runat="server" attrname="idDepartamento"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <asp:DropDownList ID="ddlidStatus" class="form-control Caixa_Selecao" runat="server" placeholder="Status" attrname="idStatus"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                                <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Nova Requisição" OnClick="cmdNovo_Click" />
                                            </div>
                                        </div>

                                    </ContentTemplate>
                                </asp:UpdatePanel>

                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-12">
                            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                            <asp:Panel ID="pnResultado" class="" runat="server">

                                <div id="resultado">
                                    <div class="panel panel-primary">
                                        <div class="panel-body">
                                            <div class="table-responsive">
                                                <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">


                                                    <Columns>

                                                        <asp:HyperLinkField DataNavigateUrlFields="idRequisicao"
                                                            DataTextField="idRequisicao" HeaderText="ID"
                                                            DataNavigateUrlFormatString="Requisicao_Detalhe.aspx?id={0}">
                                                            <HeaderStyle Width="5%" />
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:HyperLinkField>

                                                        <asp:HyperLinkField DataNavigateUrlFields="idRequisicao"
                                                            DataTextField="dtRequisicao" HeaderText="Data Requisição"
                                                            DataNavigateUrlFormatString="Requisicao_Detalhe.aspx?id={0}">
                                                            <HeaderStyle Width="10%" />
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:HyperLinkField>

                                                        <asp:HyperLinkField DataNavigateUrlFields="idRequisicao"
                                                            DataTextField="sDscTipoRequisicao" HeaderText="Tipo"
                                                            DataNavigateUrlFormatString="Requisicao_Detalhe.aspx?id={0}">
                                                            <HeaderStyle Width="15%" />
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:HyperLinkField>

                                                        <asp:HyperLinkField DataNavigateUrlFields="idRequisicao"
                                                            DataTextField="sReferencia" HeaderText="Referência"
                                                            DataNavigateUrlFormatString="Requisicao_Detalhe.aspx?id={0}">
                                                            <HeaderStyle Width="10%" />
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:HyperLinkField>

                                                        <asp:HyperLinkField DataNavigateUrlFields="idRequisicao"
                                                            DataTextField="sDscUsuarioRequisicao" HeaderText="Solicitante"
                                                            DataNavigateUrlFormatString="Requisicao_Detalhe.aspx?id={0}">
                                                            <HeaderStyle Width="15%" />
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:HyperLinkField>


                                                        <asp:HyperLinkField DataNavigateUrlFields="idRequisicao"
                                                            DataTextField="sDscDepartamento" HeaderText="Departamento Solicitante"
                                                            DataNavigateUrlFormatString="Requisicao_Detalhe.aspx?id={0}">
                                                            <HeaderStyle Width="12%" />
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:HyperLinkField>

                                                        <asp:HyperLinkField DataNavigateUrlFields="idRequisicao"
                                                            DataTextField="sDscMotivoRequisicao" HeaderText="Motivo Requisição"
                                                            DataNavigateUrlFormatString="Requisicao_Detalhe.aspx?id={0}">
                                                            <HeaderStyle Width="25%" />
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:HyperLinkField>

                                                        <asp:HyperLinkField DataNavigateUrlFields="idRequisicao"
                                                            DataTextField="sDscStatus" HeaderText="Status Atual"
                                                            DataNavigateUrlFormatString="Requisicao_Detalhe.aspx?id={0}">
                                                            <HeaderStyle Width="20%" />
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
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
                </div>

            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
