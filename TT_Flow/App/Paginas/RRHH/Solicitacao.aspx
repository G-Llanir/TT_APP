<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Solicitacao.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.Solicitacoes.Solicitacao" %>

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
                <asp:Label ID="lblTituloPagina" runat="server" Text="Solicitação"></asp:Label><small> Solicitações do Colaborador </small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa </h3>
                </div>
                <div class="panel-body ">
                    <div class="form-group">

                        <div class="row">
                       <%--     <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:TextBox ID="txtdtSolicitacao" class="form-control" runat="server" placeholder="Data da Solicitação" MaxLength="10" type="date"></asp:TextBox>
                                </div>
                            </div>--%>
                           
                        </div>
                        <div class="row">
                             <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlidDepartamento" runat="server" class="form-control yes_no select Caixa_Selecao" attrname="Tipo"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-lg-4">
                                <asp:DropDownList ID="ddlidUsuario" runat="server" class="form-control yes_no select Caixa_Selecao" attrname="idUsuario"></asp:DropDownList>
                            </div>
                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlidStatus" runat="server" class="form-control" attrname="Tipo">
                                   <%--     <asp:ListItem Selected="True" Value="0">Todos Status</asp:ListItem>
                                        <asp:ListItem Value="1"> Em Análise</asp:ListItem>
                                        <asp:ListItem Value="2"> Aprovado pelo Supervisor</asp:ListItem>
                                        <asp:ListItem Value="4"> Rejeitado pelo Supervisor</asp:ListItem>
                                        <asp:ListItem Value="6"> Aprovado pelo RH</asp:ListItem>
                                        <asp:ListItem Value="7"> Rejeitado pelo RH</asp:ListItem>
                                        <asp:ListItem Value="3"> Finalizado</asp:ListItem>
                                        <asp:ListItem Value="5"> Cancelado</asp:ListItem>--%>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlidTipo" runat="server" class="form-control" attrname="Tipo">
                               <%--         <asp:ListItem Selected="True" Value="0">Todos Tipos</asp:ListItem>
                                        <asp:ListItem Value="1">Férias</asp:ListItem>
                                        <asp:ListItem Value="2">Especiais</asp:ListItem>--%>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-6">
                                <div class="form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                </div>
                            </div>
                        </div>
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

                                                <asp:HyperLinkField DataNavigateUrlFields="idSolicitacao"
                                                    DataTextField="idSolicitacao" HeaderText="ID"
                                                    DataNavigateUrlFormatString="Solicitacao_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="2%" />
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idSolicitacao"
                                                    DataTextField="dtSolicitacao" HeaderText="Data de Abertura"
                                                    DataNavigateUrlFormatString="Solicitacao_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="10%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                  <asp:HyperLinkField DataNavigateUrlFields="idSolicitacao"
                                                    DataTextField="sTipo" HeaderText="Tipo de Solicitação"
                                                    DataNavigateUrlFormatString="Solicitacao_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="10%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idSolicitacao"
                                                    DataTextField="sDscSolicitacao" HeaderText="Descrição"
                                                    DataNavigateUrlFormatString="Solicitacao_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="40%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idSolicitacao"
                                                    DataTextField="sSolicitante" HeaderText="Solicitante"
                                                    DataNavigateUrlFormatString="Solicitacao_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="10%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>                                                                                                                                

                                                <asp:HyperLinkField DataNavigateUrlFields="idSolicitacao"
                                                    DataTextField="sDepartamento" HeaderText="Departamento"
                                                    DataNavigateUrlFormatString="Solicitacao_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="8%" />
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idSolicitacao"
                                                    DataTextField="sStatus" HeaderText="Status da Solicitação"
                                                    DataNavigateUrlFormatString="Solicitacao_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="10%" />
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

</asp:Content>
