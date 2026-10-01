<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="RelatorioFinanceiro_Parceiros.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Financeiro.RelatorioFinanceiro_Parceiros" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Relatório Financeiro"></asp:Label>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">

            <div class="panel panel-primary" id="div_filtroPesquisa" runat="server">

                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;Filtro para pesquisa </h3>
                </div>

                <div class="panel-body ">

                    <div class="form-group">
                        <div class="col-lg-12">
                            <div class="row">
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

                                <div class="col-lg-1">
                                    <div class="form-group">
                                        <label>Tipo</label>
                                        <asp:DropDownList ID="ddlsTipo" class="form-control" runat="server">
                                            <asp:ListItem Text="Emissão" Value="E"></asp:ListItem>
                                            <asp:ListItem Text="Vencimento" Value="V"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Local</label>
                                        <asp:DropDownList ID="ddlsLocal" class="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlsLocal_SelectedIndexChanged">
                                            <asp:ListItem Text="Contas a Receber" Value="Receber"></asp:ListItem>
                                            <asp:ListItem Text="Contas a Pagar" Value="Pagar"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-1">
                                    <div class="form-group">
                                        <label>Status</label>
                                        <asp:DropDownList ID="ddlsStatus" class="form-control" runat="server">
                                            <asp:ListItem Text="Todos" Value=""></asp:ListItem>
                                            <asp:ListItem Text="Em Aberto" Value="Em Aberto"></asp:ListItem>
                                            <asp:ListItem Text="Liquidado" Value="Liquidado"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Parceiro</label>
                                        <asp:DropDownList ID="ddlidParceiro" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Empresa</label>
                                        <asp:ListBox ID="lstEmpresa" class="form-control Caixa_Selecao" SelectionMode="Multiple" runat="server"></asp:ListBox>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="col-lg-12">
                            <div class="row">

                                <div class="col-lg-4" style="padding: 0px;">
                                    <div class="col-lg-6" style="display: flex;">
                                        <div class="form-group">
                                            <label></label>
                                            <asp:CheckBox ID="cbAgruparCNPJ" Text="&nbspAgrupar por Cabeça de CNPJ" runat="server" class="form-control" Style="width: fit-content; padding: revert-layer; display: flow; justify-self: center;" AutoPostBack="true" OnCheckedChanged="cbAgruparCNPJ_CheckedChanged"></asp:CheckBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-6" style="display: flex;">
                                        <div class="form-group">
                                            <label></label>
                                            <asp:CheckBox ID="cbItemFinanceiro" Text="&nbspExcluir Itens Financeiros" runat="server" class="form-control" Style="width: fit-content; padding: revert-layer; display: flow; justify-self: center;" Checked="true"></asp:CheckBox>
                                        </div>
                                    </div>

                                </div>


                                <div class="col-lg-1" style="padding: 0px;">
                                    <div class="col-lg-6">
                                        <div class="form-group">
                                            <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" Style="margin-top: 24px" />
                                        </div>
                                    </div>

                                    <div class="col-lg-6">
                                        <div class="form-group">
                                            <asp:HyperLink ID="hlExcelRelatorio" runat="server" CssClass="btn btn-warning" Target="_blank" NavigateUrl="~/App/Paginas/Adm/Financeiro/RelatorioFinanceiro_Parceiros.aspx?action=export" Visible="false" Style="margin-top: 24px"><i class="fa fa-file-excel-o"></i> Relatório Excel</asp:HyperLink>
                                        </div>
                                    </div>
                                </div>
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

                            <asp:Label runat="server" ID="lblTituloTabela" Text="" Style="font-size: Large; font-weight: bold; display: flex; justify-content: center;"></asp:Label>

                            <div class="table-responsive">
                                <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover" DataKeyNames="idLinha"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">
                                    <Columns>

                                        <asp:BoundField DataField="idLinha" HeaderText="Ordem">
                                            <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sCNPJ" HeaderText="CNPJ">
                                            <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:TemplateField HeaderText="Razão Social">
                                            <ItemTemplate>
                                                <asp:HyperLink runat="server" ID="hlRazaoSocial" Text='<%# Eval("sRazaoSocial") %>' Target="_blank"></asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="nValorTotal" HeaderText="Valor Total" DataFormatString="R$ {0:N2}">
                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="nQtdTitulos" HeaderText="Quantidade de Títulos">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                    </Columns>
                                </asp:GridView>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

        </div>

    </div>

</asp:Content>
