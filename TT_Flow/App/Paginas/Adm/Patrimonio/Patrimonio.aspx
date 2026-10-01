<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Patrimonio.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Patrimonio.Patrimonio" %>

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
                <asp:Label ID="lblTituloPagina" runat="server" Text="Patrimônio"></asp:Label><small> Patrimônio</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa </h3>
                </div>
                <div class="panel-body ">
                    <div class="form-group">

                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:TextBox ID="txtPesquisa" class="form-control " placeholder="Pesquisar" runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlGrupo" runat="server" class="form-control" attrname="Tipo"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlCategoria" runat="server" class="form-control" attrname="Tipo"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlLocal" runat="server" class="form-control" attrname="Tipo"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlEstadoConservacao" runat="server" class="form-control" attrname="Tipo"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlParceiro" runat="server" class="form-control" attrname="Tipo"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlDepartamento" runat="server" class="form-control" attrname="Tipo"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-6">
                            <div class="form-group">
                                <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo Patrimônio" OnClick="cmdNovo_Click" />
                                <asp:Button ID="cmdExportar" class="btn btn-info" runat="server" Text="Exportar Excel" OnClick="cmdExportar_Click" />
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
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowCommand="dtgvConsulta_RowCommand">
                                            <Columns>

                                                <asp:HyperLinkField DataNavigateUrlFields="idPatrimonio"
                                                    DataTextField="idPatrimonio" HeaderText="ID"
                                                    DataNavigateUrlFormatString="Patrimonio_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="3%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idPatrimonio"
                                                    DataTextField="sEtiquetaPatrimonio" HeaderText="TAG"
                                                    DataNavigateUrlFormatString="Patrimonio_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="7%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>
												
                                                <asp:HyperLinkField DataNavigateUrlFields="idPatrimonio"
                                                    DataTextField="dtInclusao" HeaderText="Data de Inclusão"
                                                    DataNavigateUrlFormatString="Patrimonio_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="10%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idPatrimonio"
                                                    DataTextField="sDscPatrimonio" HeaderText="Descrição"
                                                    DataNavigateUrlFormatString="Patrimonio_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="18%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idPatrimonio"
                                                    DataTextField="sDscCategoria" HeaderText="Categoria"
                                                    DataNavigateUrlFormatString="Patrimonio_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="17%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idPatrimonio"
                                                    DataTextField="sDscParceiro" HeaderText="Parceiro"
                                                    DataNavigateUrlFormatString="Patrimonio_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="15%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idPatrimonio"
                                                    DataTextField="sDscLocal" HeaderText="Local"
                                                    DataNavigateUrlFormatString="Patrimonio_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="10%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idPatrimonio"
                                                    DataTextField="sDscResponsavel" HeaderText="Responsável"
                                                    DataNavigateUrlFormatString="Patrimonio_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="10%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idPatrimonio"
                                                    DataTextField="sDscEstadoConservacao" HeaderText="Estado de Consevação"
                                                    DataNavigateUrlFormatString="Patrimonio_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="5%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>


                                                <asp:TemplateField HeaderText="Duplicar">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkDuplicar" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Duplicar" CommandArgument='<%# Eval("idPatrimonio") %>'><i class="fa fa-files-o"></i></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:TemplateField>
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
