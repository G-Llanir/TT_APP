<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Pesquisa_NFe.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Manutencao.Pesquisa_NFe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <script type="text/javascript">
        function MudarBotao() {
            document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
        }

        $(function () {
            $('[id*=txtDataInicial]').datepicker({
                autoclose: true,
                format: 'dd/mm/yyyy',
                language: 'pt-BR'
            });
            $('[id*=txtDataFinal]').datepicker({
                autoclose: true,
                format: 'dd/mm/yyyy',
                language: 'pt-BR'
            });

            $('[id*=txtDataInicial]').mask('99/99/9999');
            $('[id*=txtDataFinal]').mask('99/99/9999');

        });
    </script>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Consulta NF-e"></asp:Label><small> Manutenção</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="Consulta" />
        </div>

        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa</h3>
                </div>
                <div class="panel-body">
                    <div class="col-lg-4">
                        <div class="form-group">
                            <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server" MaxLength="50"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-lg-2">
                        <div class="form-group">
                            <asp:TextBox ID="txtDataInicial" class="form-control" runat="server" placeholder="Data Inicial" data-mask="00/00/0000" MaxLength="10"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-lg-2">
                        <div class="form-group">
                            <asp:TextBox ID="txtDataFinal" class="form-control" runat="server" placeholder="Data Final" data-mask="00/00/0000" MaxLength="10"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-lg-2">
                        <div class="form-group">
                            <asp:DropDownList ID="ddlTipoNFe" runat="server" class="form-control"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-lg-2">
                        <div class="form-group">
                            <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-lg-12">
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>

    </div>


    <asp:Panel ID="pnResultado" runat="server">
        <div id="resultado">
            <div class="panel panel-primary">
                <div class="panel-body">
                    <div class="table-responsive">
                        <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                            <Columns>

                                <asp:BoundField DataField="idXML" HeaderText="ID">
                                    <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:BoundField DataField="dtImportacao" HeaderText="Data Importado">
                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:BoundField DataField="nNumeroNF" HeaderText="Número">
                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:HyperLinkField DataNavigateUrlFields="chNFe"
                                    DataTextField="chNFe" HeaderText="Chave NFe"
                                    DataNavigateUrlFormatString="https://consultadanfe.com/?chave={0}">
                                    <HeaderStyle Width="50%" />
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:BoundField DataField="sCNPJ_Emitente" HeaderText="Parceiro">
                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:BoundField DataField="sDscTipoObjeto" HeaderText="Tipo">
                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:TemplateField HeaderText="Download">
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server" ID="btnXML" class="btn btn-sm btn-primary" Text="XML" OnClick="btnXML_Click"></asp:LinkButton>
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

</asp:Content>
