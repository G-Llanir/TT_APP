<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="ControleFerias.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.ControleFerias" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .gvChildHeader2 {
            padding: 4px;
            background-color: rgba(0, 180, 0) !important;
            color: white !important;
            border: 1px solid #bbb;
            font-weight: bold;
        }

        .collapsed-row {
            display: none;
            padding: 1px;
            margin: 1px;
        }

        .gvMainTh {
            padding: 4px;
            background-color: rgba(0, 125, 0) !important;
            color: white !important;
            border: 1px solid #bbb;
            font-weight: bold;
        }

        .gvMainTd {
            padding: 7px;
            border: 1px solid #bbb;
            background-color: #ffffff;
        }

            .gvMainTd tbody tr:hover td {
                background-color: #e0e0e0;
            }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Controle de Férias"></asp:Label>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">
            <asp:Panel ID="pnFeriasConsulta" class="" runat="server">
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
                                    <label>Tipo</label>
                                    <asp:DropDownList ID="ddlidTipoPesquisa" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-3">
                                <div class="form-group">
                                    <label>Colaborador</label>
                                    <asp:DropDownList ID="ddlidColaboradorPesquisa" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-4" style="margin-top: 24px;">
                                <div class="form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" />
                                    <asp:Button ID="btnNovoFerias" class="btn btn-success" runat="server" Text="Registro Férias" OnClick="btnNovoFerias_Click" />
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
                                    <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover " DataKeyNames="idColaborador"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">
                                        <Columns>

                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnToggle" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-div-id='<%# "tr_1_Ferias" + string.Format("{0}", Eval("idColaborador")) %>'></asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="sDscColaborador" HeaderText="Colaborador">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Saldo (Dias)">
                                                <ItemTemplate>
                                                    <asp:Label ID="nSaldo" runat="server" Text='<%# Eval("nSaldo") %>'></asp:Label>
                                                    <%# NovaLinha(string.Format("{0}", Eval("idColaborador")), "_1_Ferias") %>

                                                    <asp:GridView ID="gv_FeriasDetalhe" runat="server" Width="100%" class="table table-striped table-bordered table-hover tablePai"
                                                        GridLines="None" AutoGenerateColumns="false" Style="padding: 0; margin: 0" DataKeyNames="idControleFerias"
                                                        AlternatingRowStyle-CssClass="gvAltRow" OnRowDataBound="gv_FeriasDetalhe_RowDataBound">
                                                        <Columns>

                                                            <asp:BoundField DataField="dtEvento" HeaderText="Data">
                                                                <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:TemplateField HeaderText="Tipo">
                                                                <HeaderStyle Width="15%" />
                                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="lbFeriasDetalhe_Tipo" runat="server"
                                                                        Text='<%# Eval("sDscEvento") %>'
                                                                        CommandArgument='<%# Eval("idControleFerias") %>'
                                                                        OnCommand="lbFeriasDetalhe_Command">
                                                                    </asp:LinkButton>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                            <asp:BoundField DataField="nDias" HeaderText="Dias">
                                                                <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                        </Columns>
                                                    </asp:GridView>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnFeriasDetalhe" runat="server">
                <div>
                    <ul id="tab_controleFerias" class="nav nav-tabs" role="tablist">
                        <li role="presentation" class="tabpanel active">
                            <a href="#controleFerias" id="aba-controleFerias" role="tab" data-toggle="tab" aria-controls="controleFerias" aria-expanded="false"><b>Controle Férias</b></a>
                        </li>

                        <li role="presentation" runat="server" id="aba_historico">
                            <a href="#historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="Historico"><b>Histórico</b></a>
                        </li>

                    </ul>
                </div>
                <div id="tab" class="tab-content">

                    <div role="tabpanel" class="tab-pane fade in active" id="controleFerias" aria-labelledby="controleFerias">
                        <br />
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>

                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaDetalhe" />
                                </div>

                                <div class="form-group">
                                    <div class="row">

                                        <div class="col-lg-12 form-group">

                                            <div class="col-lg-1">
                                                <label>ID</label>
                                                <asp:TextBox ID="txtidControleFerias" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>

                                        </div>

                                        <div class="col-lg-12 form-group">

                                            <div class="col-lg-3">
                                                <div class="form-group">
                                                    <label>Colaborador</label>
                                                    <asp:DropDownList ID="ddlidColaborador" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Tipo</label>
                                                    <asp:DropDownList ID="ddlidTipo" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Data</label>
                                                    <asp:TextBox ID="txtdtEvento" class="form-control" runat="server" TextMode="Date"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-1">
                                                <div class="form-group">
                                                    <label>Quantidade Dias</label>
                                                    <asp:TextBox ID="txtnQtdFerias" class="form-control" runat="server" TextMode="Number"></asp:TextBox>
                                                </div>
                                            </div>

                                        </div>

                                        <div class="col-lg-12 form-group">

                                            <div class="col-lg-10">
                                                <div class="form-group">
                                                    <label>Descrição</label>
                                                    <asp:TextBox ID="txtsDescricao" class="form-control" runat="server" TextMode="MultiLine" MaxLength="200"></asp:TextBox>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>

                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>

                    <div role="tabpanel" class="tab-pane fade" id="historico" aria-labelledby="Historico">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <div class="panel" runat="server" id="DIV_historico">

                                    <div class="panel-body">
                                        <div class="form-stacked row">
                                            <div class="col-lg-12 form-group table-responsive" style="margin-top: 20px;">
                                                <asp:GridView
                                                    ID="gv_Historico" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    ShowFooter="False" OnRowDataBound="gv_Historico_RowDataBound">
                                                    <Columns>
                                                        <asp:BoundField DataField="dtAcao" HeaderText="Data da Ação">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sDscUsuarioAcao" HeaderText="Usuário">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sTipoAcao" HeaderText="Tipo">
                                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sAcao" HeaderText="Ação">
                                                            <ItemStyle Width="70%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                    <fieldset class="form-stacked actions">
                        <div class="col-lg-12">
                            <div class="form-group">
                                <asp:Button ID="btnSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="btnSalvar_Click" />
                                <asp:Button ID="btnVoltar" CssClass="btn btn-lg btn-warning" runat="server" Text="Voltar" OnClick="btnVoltar_Click" />
                            </div>
                        </div>
                    </fieldset>

                </div>
            </asp:Panel>

        </div>

        <asp:HiddenField ID="hddidControleFerias" Value="0" runat="server" />
        <asp:HiddenField ID="hddidColaborador" Value="0" runat="server" />

    </div>

</asp:Content>
