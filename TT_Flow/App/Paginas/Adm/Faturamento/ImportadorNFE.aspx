<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="ImportadorNFE.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Faturamento.ImportadorNFE" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .tag-list {
            width: 100%;
            flex-wrap: wrap;
            display: flex;
            overflow: hidden;
        }

        .tag-item {
            background: whitesmoke;
            padding: 2px 6px;
            font-size: 12px;
            text-transform: uppercase;
            border-radius: 4px;
            display: inline-block;
            margin: 2px;
            text-decoration: none;
            color: #333;
        }

        .grafico {
            border: none !important;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

    <script type="text/javascript">

        $(function () {
            $('[id*=txtdtInicio]').datepicker({
                autoclose: true,
                format: 'dd/mm/yyyy',
                language: 'pt-BR'
            });

            $('[id*=txtdtFinal]').datepicker({
                autoclose: true,
                format: 'dd/mm/yyyy',
                language: 'pt-BR'
            });

            $('[id*=txtdtInicio]').mask('99/99/9999');
            $('[id*=txtdtFinal]').mask('99/99/9999');
        });       

        function MudarBotao() {
            document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
        }
    </script>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="NFe Importação"></asp:Label><small> Consulta</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;Filtro para pesquisa </h3>
                </div>
                <div class="panel-body ">
                    <div class="form-group">

                        <div class="col-lg-3">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlsEmitente" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-3">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlidParceiro" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-2">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlsStatus" class="form-control  Caixa_Selecao" runat="server">
                                    <asp:ListItem Text="Todos os Status" Value="" Selected="True" ></asp:ListItem>
                                    <asp:ListItem Text="Pendente" Value="Pendente"></asp:ListItem>
                                    <asp:ListItem Text="Em Processamento" Value="Em processamento"></asp:ListItem>
                                    <asp:ListItem Text="Espelho" Value="Espelho"></asp:ListItem>
                                    <asp:ListItem Text="Aguardando DANFE" Value="Aguardando DANFE"></asp:ListItem>
                                    <asp:ListItem Text="Autorizada" Value="Autorizada"></asp:ListItem>
                                    <asp:ListItem Text="Cancelada" Value="Cancelada"></asp:ListItem>
                                    <asp:ListItem Text="Rejeitada" Value="Rejeitada"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-2">
                            <div class="form-group">
                                <asp:TextBox ID="txtPesquisa" class="form-control " placeholder="Pesquisar" runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-2">
                            <div class="form-group">
                                <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Importar Novo XML" OnClick="cmdNovo_Click" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <div class="panel panel-primary">
                <div class="panel-body" id="panelExibirOPI">
                    <div class="form-group">
                        <div class="col-lg-12">
                            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                            <asp:Panel ID="pnResultado" class="" runat="server">

                                <div class="table-responsive">
                                    <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">
                                        <Columns>

                                            <asp:BoundField DataField="idImportador" HeaderText="ID">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="idImportador_Formatado" HeaderText="ID">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Inclusão">
                                                <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idImportador"
                                                DataTextField="sEmpresa" HeaderText="Emitente"
                                                DataNavigateUrlFormatString="ImportadorNFE_Detalhe.aspx?id={0}">
                                                <HeaderStyle Width="5%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idImportador"
                                                DataTextField="sCliente" HeaderText="Parceiro"
                                                DataNavigateUrlFormatString="ImportadorNFE_Detalhe.aspx?id={0}">
                                                <HeaderStyle Width="20%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:TemplateField HeaderText="Referência">
                                                <ItemTemplate>
                                                    <asp:Literal ID="litReferência" runat="server" Text='<%# Eval("sReferencia") %>' Mode="PassThrough" />
                                                </ItemTemplate>
                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idImportador"
                                                DataTextField="sNomeArquivo" HeaderText="Nome do Arquivo"
                                                DataNavigateUrlFormatString="ImportadorNFE_Detalhe.aspx?id={0}">
                                                <HeaderStyle Width="20%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:BoundField DataField="sDscStatus" HeaderText="Status">
                                                <ItemStyle Width="8%" HorizontalAlign="left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nNumeroNF" HeaderText="NFe">
                                                <ItemStyle Width="8%" HorizontalAlign="left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <%--                                            <asp:BoundField DataField="xMotivo" HeaderText="Status NFe">
                                                <ItemStyle Width="15%" HorizontalAlign="left" VerticalAlign="Middle" />
                                            </asp:BoundField>--%>

                                            <%--                                            <asp:TemplateField HeaderText="Download">
                                                <ItemTemplate>
                                                    <asp:LinkButton runat="server" ID="btnXML" class="btn btn-sm btn-primary" Text="XML" OnClick="cmdXMLDownload_Click" CommandArgument='<%# Eval("idArquivo") %>'></asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>--%>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
