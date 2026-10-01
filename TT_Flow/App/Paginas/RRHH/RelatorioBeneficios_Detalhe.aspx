<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="RelatorioBeneficios_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.RelatorioBeneficios_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Referencia_MesAno.ascx" TagPrefix="uc1" TagName="Referencia_MesAno" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="content_frmRelatorioBeneficios_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        .invisivel {
            display: none;
        }

        .linkModal a {
            cursor: pointer;
        }

        .relatorio {
            margin-bottom: 5px;
        }

        .mes-ano-label {
            font-size: 1.5em;
            width: 110px;
            height: auto;
        }

        .tabela {
            border: solid 2px gray;
            border-radius: 7.5px;
        }

            .tabela thead tr {
                border-bottom: solid 3px darkgray;
            }

            .tabela tbody tr:last-child {
                border-top: solid 3px darkgray;
            }

        .tag-list {
            width: 100%;
            flex-wrap: wrap;
            display: flex;
            overflow: hidden;
        }

        .tag-item {
            background: lightgray;
            padding: 2px 4px;
            font-size: 13px;
            text-transform: uppercase;
            margin: 2px;
            display: inline-flex;
            flex-wrap: nowrap;
            border-radius: 5px;
        }
    </style>

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div id="hdd">
        <asp:HiddenField ID="hddidColaborador" runat="server" />
        <asp:HiddenField ID="hddidRelatorio" runat="server" />
    </div>

    <asp:UpdatePanel ID="updDetalhe" runat="server">
        <ContentTemplate>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Relatórios"></asp:Label>
                    </h1>
                    <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>
            </div>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa</h3>
                        </div>
                        <div class="panel-body">

                            <div class="col-lg-2">
                                <div class="form-group">
                                    <label>Mês/Ano de Referência</label>
                                    <uc1:Referencia_MesAno runat="server" ID="Referencia_MesAno" />
                                </div>
                            </div>

                            <div class="col-lg-3">
                                <div class="form-group">
                                    <label>Empresa</label>
                                    <asp:DropDownList runat="server" ID="ddlidEmpresa" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-3">
                                <div class="form-group">
                                    <label>Tipo de Contrato </label>
                                    <asp:ListBox ID="lstTipoContrato" class="form-control Caixa_Selecao" SelectionMode="Multiple" runat="server"></asp:ListBox>
                                </div>
                            </div>

                            <div class="form-group">
                                <div class="col-lg-3">
                                    <div class="form-group" style="margin-top: 4px;">
                                        <br />
                                        <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao" />
                                        <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo Relatório" OnClick="cmdNovo_Click" />
                                        <asp:HyperLink ID="lnkExportar" runat="server" CssClass="btn btn-warning" Target="_blank" NavigateUrl="~/App/Paginas/RRHH/RelatorioBeneficios_Detalhe.aspx?action=export" Visible="false"><i class="fa fa-file-excel-o"></i> Exportar para Excel</asp:HyperLink>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-1 form-group" style="display: flow; text-align: -webkit-right; /*padding: 0px 0px 0px 15px; */">
                                <label>Valor diário - VR</label>
                                <label runat="server" id="lblVlr_VR" class="form-control mes-ano-label" disabled="" />
                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <div class="form-stacked row">
                <div class="col-lg-12">

                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                    <asp:Panel ID="pnResultado" class="" runat="server">

                        <div id="resultado" runat="server">
                            <div class="panel panel-primary">
                                <div class="panel-heading">
                                    <h3 class="panel-title">
                                        <asp:Label runat="server" ID="lblCabecalho_Novo"></asp:Label></h3>
                                </div>
                                <div class="panel-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover" runat="server" Width="100%"
                                            CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" ShowFooter="true" Font-Names="Tahoma"
                                            Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">
                                            <Columns>

                                                <asp:HyperLinkField DataNavigateUrlFields="idColaborador"
                                                    DataTextField="idColaborador" HeaderText="ID"
                                                    DataNavigateUrlFormatString="Colaboradores_Detalhe.aspx?id={0}">
                                                    <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idColaborador"
                                                    DataTextField="sDscColaborador" HeaderText="Colaborador"
                                                    DataNavigateUrlFormatString="Colaboradores_Detalhe.aspx?id={0}">
                                                    <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="sDsc" />
                                                </asp:HyperLinkField>

                                                <asp:BoundField DataField="sDscTipoContrato" HeaderText="Tipo Contrato">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDscEmpresa" HeaderText="Empresa">
                                                    <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="nDiasUteis" HeaderText="Úteis">
                                                    <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="nDiasDescontar_Feriado" HeaderText="Feriados">
                                                    <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="nDiasUteis" HeaderText="A Trabalhar">
                                                    <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:HyperLinkField DataTextField="nDiasDescontar_Atestado" HeaderText="Ausências">
                                                    <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="linkModal ausencias" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataTextField="nDiasCredito" HeaderText="Adicionais">
                                                    <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="linkModal adicionais" />
                                                </asp:HyperLinkField>

                                                <asp:BoundField DataField="ValorVT" HeaderText="Total VT" DataFormatString="{0:N2}">
                                                    <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="ValorVR" HeaderText="Total VR" DataFormatString="{0:N2}">
                                                    <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                            <fieldset class="form-stacked actions">
                                <asp:Button ID="cmdSalvar" runat="server" class="btn btn-lg btn-success" Text="Salvar Novo Relatório"></asp:Button>
                                <asp:Button ID="cmdVoltar" runat="server" class="btn btn-lg btn-warning" Text="Cancelar" OnClick="cmdVoltar_Click"></asp:Button>
                            </fieldset>
                        </div>

                        <div id="resultadoSalvo" runat="server">
                            <div class="panel panel-primary">
                                <div class="panel-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgRelatorioSalvo" class="table table-striped table-bordered table-hover "
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="true" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" DataKeyNames="idRelatorio" OnRowDataBound="dtgRelatorioSalvo_RowDataBound">
                                            <Columns>

                                                <asp:HyperLinkField DataNavigateUrlFields="idColaborador"
                                                    DataTextField="idColaborador" HeaderText="ID"
                                                    DataNavigateUrlFormatString="Colaboradores_Detalhe.aspx?id={0}">
                                                    <ItemStyle Width="4%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idColaborador"
                                                    DataTextField="sDscColaborador" HeaderText="Colaborador"
                                                    DataNavigateUrlFormatString="Colaboradores_Detalhe.aspx?id={0}">
                                                    <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="sDsc" />
                                                </asp:HyperLinkField>

                                                <asp:BoundField DataField="sDscTipoContrato" HeaderText="Tipo Contrato">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDscEmpresa" HeaderText="Empresa">
                                                    <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="dtReferencia" HeaderText="Referência" DataFormatString="{0:dd/MM/yyyy}">
                                                    <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="nDiasUteis" HeaderText="Úteis">
                                                    <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="nDiasDescontar_Feriado" HeaderText="Feriados">
                                                    <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:HyperLinkField DataTextField="nDiasDescontar_Atestado" HeaderText="Ausências">
                                                    <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="linkModal ausencias" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataTextField="nDiasCreditar" HeaderText="Adicionais">
                                                    <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="linkModal adicionais" />
                                                </asp:HyperLinkField>

                                                <asp:BoundField DataField="ValorVT" HeaderText="Total VT" DataFormatString="{0:N2}">
                                                    <ItemStyle Width="9%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="ValorVR" HeaderText="Total VR" DataFormatString="{0:N2}">
                                                    <ItemStyle Width="9%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                            <fieldset class="form-stacked actions">
                                <asp:Button ID="btnVoltar" runat="server" class="btn btn-lg btn-warning" Text="Voltar" OnClick="btnVoltar_Click"></asp:Button>
                            </fieldset>
                        </div>

                        <div id="resultadoResumo" runat="server">
                            <div class="panel panel-primary">
                                <div class="panel-heading">
                                    <h3 class="panel-title">
                                        <asp:Label runat="server" Text="Relatórios já gerados"></asp:Label></h3>
                                </div>
                                <div class="panel-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgConsultaResumo" class="table table-striped table-bordered table-hover "
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowCommand="dtgConsultaResumo_RowCommand">
                                            <Columns>

                                                <asp:HyperLinkField DataNavigateUrlFields="idRegistro"
                                                    DataTextField="idRegistro" HeaderText="ID"
                                                    DataNavigateUrlFormatString="RelatorioBeneficios_Detalhe.aspx?id={0}">
                                                    <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idRegistro"
                                                    DataTextField="sMes_Ano" HeaderText="Mês/Ano de Referência"
                                                    DataNavigateUrlFormatString="RelatorioBeneficios_Detalhe.aspx?id={0}">
                                                    <ItemStyle Width="35%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:TemplateField HeaderText="Tipo Contrato">
                                                    <HeaderStyle Width="25%" />
                                                    <ItemTemplate>
                                                        <asp:Literal ID="litTipoContrato" runat="server" Text='<%# Eval("sDscTipoContrato") %>' Mode="PassThrough" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idRegistro"
                                                    DataTextField="sDscEmpresa" HeaderText="Empresa"
                                                    DataNavigateUrlFormatString="RelatorioBeneficios_Detalhe.aspx?id={0}">
                                                    <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idRegistro"
                                                    DataTextField="sDscUsuario" HeaderText="Por"
                                                    DataNavigateUrlFormatString="RelatorioBeneficios_Detalhe.aspx?id={0}">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idRegistro"
                                                    DataTextField="dtAtualizacao" HeaderText="Em"
                                                    DataNavigateUrlFormatString="RelatorioBeneficios_Detalhe.aspx?id={0}">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:TemplateField HeaderText="Excluir">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="cmdExcluir" runat="server" CommandName='<%# Eval("idRegistro") %>' data-toggle="tooltip" title="Excluir Relatório"><i class="fa fa-eraser"></i></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
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

            <asp:HiddenField ID="hddidEmpresa" Value="0" runat="server" />
            <asp:HiddenField ID="hddsTipoContrato" Value="" runat="server" />

            <div class="row col-lg-12">
                <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                <br />
            </div>

            <div id="dialog-Salvar" class="modal" title="Salvar">
                <p>
                    <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                    <asp:Label ID="lblTituloSalvar" runat="server" Text="">Confirma o Salvamento do Controle?</asp:Label>
                </p>
            </div>

            <div class="modal fade" id="mdoalDias">
                <div class="modal-dialog" style="width: 65%;">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h3 class="modal-title"></h3>
                        </div>
                        <div class="modal-body">

                            <label class="relatorio invisivel">Relatório de <u id="relatorio"></u></label>
                            <label class="pai"></label>

                            <div style="padding-bottom: 10px;">
                                <div class="tabela form-group"></div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
