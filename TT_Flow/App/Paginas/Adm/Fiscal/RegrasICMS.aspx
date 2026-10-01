<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="RegrasICMS.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Fiscal.RegrasICMS" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/TextBox_Padrao.ascx" TagPrefix="uc1" TagName="TextBox_Padrao" %>
<%@ Register Src="~/App/Controles/DropDownList_Padrao.ascx" TagPrefix="uc1" TagName="DropDownList_Padrao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <div id="hdd">
        <asp:HiddenField ID="hddidRegraICMS" Value="" runat="server" />
    </div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="ICMS"></asp:Label>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">

            <asp:Panel ID="pnICMSConsulta" runat="server">
                <div class="panel panel-primary" id="div_filtroPesquisa" runat="server">
                    <div class="panel-heading">
                        <h3 class="panel-title"><i class="fa fa-filter"></i>&nbsp;Filtro para pesquisa </h3>
                    </div>
                    <div class="panel-body">

                        <div class="col-lg-3 padd-0">
                            <uc1:DropDownList_Padrao runat="server" ID="ddlsOrigemPesquisa" />
                        </div>

                        <div class="col-lg-3">
                            <uc1:DropDownList_Padrao runat="server" ID="ddlsDestinoPesquisa" />
                        </div>

                        <div class="col-lg-4 padd-0">
                            <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClientClick="MudarBotao_Pesquisa(this)" OnClick="cmdPesquisar_Click" />
                            <asp:Button ID="btnNovoICMS" class="btn btn-success" runat="server" Text="Novo" OnClick="btnNovoICMS_Click" />
                            <asp:HyperLink ID="hlExcelRelatorio" runat="server" CssClass="btn btn-warning" Target="_blank" NavigateUrl="~/App/Paginas/Adm/Fiscal/RegrasICMS.aspx?action=export"><i class="fa fa-file-excel-o"></i> Relatório Excel</asp:HyperLink>
                        </div>

                    </div>
                </div>

                <div class="row">
                    <div class="col-lg-12">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                        <div class="panel panel-primary" id="div_gvConsulta" runat="server">
                            <div class="panel-body">
                                <div class="table-responsive">
                                    <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                        <Columns>

                                            <asp:BoundField DataField="idRegraICMS" HeaderText="ID">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="UF Origem">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbUFOrigem" runat="server" Text='<%# Eval("sDscUFOrigem") %>' CommandArgument='<%# Eval("idRegraICMS") %>' OnCommand="lbRegrasIMCS_Command" />
                                                </ItemTemplate>
                                                <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="UF Destino">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbUFDestino" runat="server" Text='<%# Eval("sDscUFDestino") %>' CommandArgument='<%# Eval("idRegraICMS") %>' OnCommand="lbRegrasIMCS_Command" />
                                                </ItemTemplate>
                                                <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="nICMS" HeaderText="ICMS">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nICMS_DIFAL" HeaderText="ICMS DIFAL">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nICMS_Nacional" HeaderText="ICMS Nacional">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nICMS_Importado" HeaderText="ICMS Importado">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nIBS_UF" HeaderText="IBS UF">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nRedBC" HeaderText="Redução BC ICMS">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtAtualizacao" HeaderText="Última Atualização">
                                                <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnICMSDetalhe" class="" runat="server">

                <div class="panel panel-primary">
                    <div class="panel-body">
                        <uc1:MensagemPagina runat="server" ID="MensagemPaginaDetalhe" />

                        <div class="row">
                            <div class="col-lg-1 form-group">
                                <uc1:TextBox_Padrao runat="server" ID="txtIdRegraICMS" Titulo="ID" ReadOnly="true" />
                            </div>
                        </div>

                        <div class="row" runat="server" id="div_novaRegra">

                            <div class="col-lg-3 form-group">
                                <uc1:DropDownList_Padrao runat="server" ID="ddlidOrigem" Titulo="UF Origem" />
                            </div>

                            <div class="col-lg-3 form-group">
                                <uc1:DropDownList_Padrao runat="server" ID="ddlidDestino" Titulo="UF Destino" />
                            </div>

                            <div class="col-lg-1 form-group">
                                <label>&nbsp;</label>
                                <asp:Button ID="btnIncluir" class="form-control btn-primary" runat="server" Text="Incluir" OnClick="btnIncluir_Click"></asp:Button>
                            </div>

                        </div>

                        <div class="table-responsive" runat="server" id="div_gvRegrasICMS_Novo">
                            <asp:GridView ID="gvRegrasICMS_Novo" class="table table-striped table-bordered table-hover m-0" DataKeyNames="idLinha"
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                <Columns>

                                    <asp:BoundField DataField="sDscUFOrigem" HeaderText="UF Origem">
                                        <ItemStyle Width="14%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sDscUFDestino" HeaderText="UF Destino">
                                        <ItemStyle Width="14%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:TemplateField HeaderText="ICMS">
                                        <ItemTemplate>
                                            <uc1:TextBox_Padrao runat="server" ID="txtnICMS" Text='<%# Eval("nICMS") %>' Grupo_Simbolo="%" MascaraDinamica="$money($input, ',', '.')" />
                                        </ItemTemplate>
                                        <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="ICMS DIFAL">
                                        <ItemTemplate>
                                            <uc1:TextBox_Padrao runat="server" ID="txtnICMS_DIFAL" Text='<%# Eval("nICMS_DIFAL") %>' Grupo_Simbolo="%" MascaraDinamica="$money($input, ',', '.')" />
                                        </ItemTemplate>
                                        <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="ICMS Nacional">
                                        <ItemTemplate>
                                            <uc1:TextBox_Padrao runat="server" ID="txtnICMS_Nacional" Text='<%# Eval("nICMS_Nacional") %>' Grupo_Simbolo="%" MascaraDinamica="$money($input, ',', '.')" />
                                        </ItemTemplate>
                                        <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="ICMS Importado">
                                        <ItemTemplate>
                                            <uc1:TextBox_Padrao runat="server" ID="txtnICMS_Importado" Text='<%# Eval("nICMS_Importado") %>' Grupo_Simbolo="%" MascaraDinamica="$money($input, ',', '.')" />
                                        </ItemTemplate>
                                        <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="IBS UF">
                                        <ItemTemplate>
                                            <uc1:TextBox_Padrao runat="server" ID="txtnIBS_UF" Text='<%# Eval("nIBS_UF") %>' Grupo_Simbolo="%" MascaraDinamica="$money($input, ',', '.')" />
                                        </ItemTemplate>
                                        <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Redução BC ICMS">
                                        <ItemTemplate>
                                            <uc1:TextBox_Padrao runat="server" ID="txtnRedBC" Text='<%# Eval("nRedBC") %>' Grupo_Simbolo="%" MascaraDinamica="$money($input, ',', '.')" />
                                        </ItemTemplate>
                                        <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:TemplateField>

                                </Columns>
                            </asp:GridView>
                        </div>

                    </div>
                </div>

                <div class="col-lg-12" style="margin-bottom: 15px; padding: 0px">
                    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" Visible="false" />
                </div>

                <fieldset class="form-stacked actions">
                    <asp:Button ID="btnSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="btnSalvar_Click" />
                    <asp:Button ID="btnVoltar" CssClass="btn-warning btn-lg btn" runat="server" Text="Voltar" OnClick="btnVoltar_Click" />
                </fieldset>

            </asp:Panel>

        </div>
    </div>

</asp:Content>