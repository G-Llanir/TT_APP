<%@ Page Title="" Language="C#" Async="true" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="GerenciadorNFe.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Faturamento.GerenciadorNFe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

    <script type="text/javascript">
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

    <div id="hdd">
        <asp:HiddenField ID="hddIdLinha" runat="server" />
        <asp:HiddenField ID="hddsBloco" runat="server" />
        <asp:HiddenField ID="hddEndereco" runat="server" />
        <asp:HiddenField ID="hddsCaminho_UniNFe" runat="server" />
    </div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Gerenciador NF-e"></asp:Label><small> Consulta</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="Gerenciador NF-e" />
        </div>
        <div class="col-lg-12">

            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-filter"></i>&nbsp;Filtro para pesquisa</h3>
                </div>
                <div class="panel-body">

                    <div class="col-lg-1 form-group">
                        <asp:TextBox ID="txtDataInicial" class="form-control" runat="server" placeholder="Data Inicial" data-mask="00/00/0000" MaxLength="10" />
                    </div>

                    <div class="col-lg-1 form-group">
                        <asp:TextBox ID="txtDataFinal" class="form-control" runat="server" placeholder="Data Final" data-mask="00/00/0000" MaxLength="10" />
                    </div>

                    <div class="col-lg-2 form-group">
                        <asp:DropDownList ID="ddlidOrigem" runat="server" class="form-control">
                            <asp:ListItem Value="0" Text="Todos"></asp:ListItem>
                            <asp:ListItem Value="1" Text="Faturamento NFe"></asp:ListItem>
                            <asp:ListItem Value="2" Text="Faturamento NFs"></asp:ListItem>
                            <asp:ListItem Value="3" Text="Importação"></asp:ListItem>
                            <asp:ListItem Value="4" Text="Devolução"></asp:ListItem>
                            <asp:ListItem Value="5" Text="Emissor"></asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="col-lg-2 form-group">
                        <asp:DropDownList ID="ddlidEmpresa" runat="server" class="form-control" />
                    </div>

                    <div class="col-lg-2 form-group">
                        <asp:DropDownList ID="ddlsStatus" runat="server" class="form-control">
                            <asp:ListItem Value="0" Text="Todos os Status"></asp:ListItem>
                            <asp:ListItem Value="Espelho" Text="Espelho"></asp:ListItem>
                            <asp:ListItem Value="Processamento" Text="Em Processamento"></asp:ListItem>
                            <asp:ListItem Value="Autorizada" Text="Autorizada"></asp:ListItem>
                            <asp:ListItem Value="Cancelada" Text="Cancelada"></asp:ListItem>
                            <asp:ListItem Value="Rejeitada" Text="Rejeitada"></asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="col-lg-3 form-group">
                        <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server" MaxLength="50" />
                    </div>

                    <div class="col-lg-1 form-group">
                        <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao_Pesquisa(this)" />
                    </div>

                </div>

                    <a data-toggle="collapse" runat="server" id="filtroCollapseIcone" href="#idFiltroCollapse" role="button" aria-expanded="false" aria-controls="idFiltroCollapse1"><i class="fa fa-chevron-down"></i><span></span></a>

                    <div class="collapse" runat="server" id="idFiltroCollapse">
                        <div class="row">
                            <div class="col-lg-4">
                                <div class="form-group">
                                    <label runat="server" id="lblTipo">Tipo de Produto</label>
                                    <asp:DropDownList ID="FT_ddlidTipoProduto" class="form-control" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                            <div runat="server" id="div_Familia" class="col-lg-4">
                                <div class="form-group">
                                    <label>Familia</label>
                                    <asp:DropDownList ID="FT_ddlidFamilia" class="form-control" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                            <div runat="server" id="div_Grupo" class="col-lg-2">
                                <div class="form-group">
                                    <label>Grupo</label>
                                    <asp:DropDownList ID="FT_ddlidGrupo" class="form-control" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                            <div runat="server" id="divPais" class="col-lg-2">
                                <div class="form-group">
                                    <label>País de Origem</label>
                                    <asp:DropDownList ID="FT_ddlidPaisOrigem" class="form-control" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </div>

            <div class="panel panel-primary">
                <div class="panel-body" id="panelExibirOPI">

                    <div class="col-lg-12">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    </div>

                    <asp:Panel ID="pnResultado" runat="server">

                        <asp:Literal runat="server" ID="ltOcultarColunas" />

                        <div class="col-lg-12 padd-0">
                            <div class="table-responsive">
                                <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover " DataKeyNames="chNFe, idArquivo, sLinkPDF, sXML_Autorizado, idXML"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">
                                    <Columns>

                                        <asp:HyperLinkField DataNavigateUrlFields="sLink"
                                            DataTextField="idXML" HeaderText="ID"
                                            DataNavigateUrlFormatString="{0}" Target="_blank">
                                            <HeaderStyle Width="4%" />
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:BoundField DataField="sDscOrigem" HeaderText="Origem">
                                            <ItemStyle Width="8%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="dtImportacao" HeaderText="Data Emissão">
                                            <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="nSerieNF" HeaderText="Série">
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:HyperLinkField DataNavigateUrlFields="chNFe"
                                            DataTextField="chNFe" HeaderText="Chave NF-e"
                                            DataNavigateUrlFormatString="https://consultadanfe.com/?chave={0}" Target="_blank">
                                            <HeaderStyle Width="10%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:BoundField DataField="nNumeroNF" HeaderText="Número">
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sNome_Emitente" HeaderText="Emitente">
                                            <ItemStyle Width="5%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="nControleTT" HeaderText="Nº Controle TT">
                                            <ItemStyle Width="8%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:HyperLinkField DataNavigateUrlFields="sLinkPedido"
                                            DataTextField="nPedido" HeaderText="Nº Pedido"
                                            DataNavigateUrlFormatString="{0}" Target="_blank">
                                            <HeaderStyle Width="8%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="sLink"
                                            DataTextField="sNome_Destinatario" HeaderText="Destinatário"
                                            DataNavigateUrlFormatString="{0}" Target="_blank">
                                            <HeaderStyle Width="30%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:BoundField DataField="nValorFaturado" HeaderText="Valor Faturado" DataFormatString="{0:N2}">
                                            <ItemStyle Width="8%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sStatus" HeaderText="Status">
                                            <ItemStyle Width="8%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="xMotivoCompleto" HeaderText="Observação">
                                            <ItemStyle Width="15%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:TemplateField HeaderText="Download">
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" ID="cmdDownloadXML" class="btn btn-sm btn-primary" Text="XML" OnClick="cmdDownloadXML_Click" />
                                                <asp:LinkButton runat="server" ID="cmdDownloadDANFE" class="btn btn-sm btn-warning" Text="DANFE" OnClick="cmdDownloadDANFE_Click" />
                                            </ItemTemplate>
                                            <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>

                    </asp:Panel>

                </div>
            </div>

        </div>
    </div>

    <div class="modal fade" id="UploadArquivos_Modal" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="exampleModalLongTitle">
                        <asp:Label ID="lblEnviarArquivos_Titulo" runat="server" Text="Upload PDF Serviços" Font-Bold="true" />
                    </h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">

                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_EnviarArquivo" />

                    <div class="row">
                        <div class="col-lg-12">
                            <div class="form-stacked">
                                <div class="col-lg-12" id="div4" runat="server">
                                    <div class="form-group">
                                        <label>Selecione o Arquivo</label>
                                        <asp:FileUpload ID="fu_EnviarArquivo" runat="server" />
                                    </div>
                                </div>
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <asp:Label ID="lblEnviarArquivo_sDscArquivo" runat="server" Text="Observação" Font-Bold="true" />
                                        <asp:TextBox ID="txtEnviarArquivo_sDscArquivo" class="form-control" runat="server" MaxLength="300" />
                                    </div>
                                </div>
                                <div class="col-lg-12">
                                    <div class="form-group"></div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
                <div class="modal-footer">
                    <asp:Button ID="cmdEnviarArquivos" CssClass="btn-success btn" runat="server" Text="Enviar" OnClick="cmdEnviarArquivos_Click" UseSubmitBehavior="False" />
                    <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
