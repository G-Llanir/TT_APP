<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="NBS.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Manutencao.NBS" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/TextBox_Padrao.ascx" TagPrefix="uc1" TagName="TextBox_Padrao" %>
<%@ Register Src="~/App/Controles/DropDownList_Padrao.ascx" TagPrefix="uc1" TagName="DropDownList_Padrao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <asp:UpdatePanel runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <ContentTemplate>

            <style>
                .form-check input {
                    margin: 0;
                }
            </style>

            <div id="hdd">
                <asp:HiddenField runat="server" ID="hddidNBS" Value="0" />
            </div>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1><span id="lblTituloPagina">NBS</span>&nbsp;<small>Consulta</small></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="NBS" />
                </div>

                <asp:Panel runat="server" ID="pnConsulta" class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-filter"></i>&nbsp;Filtro para pesquisa</h3>
                        </div>
                        <div class="panel-body">

                            <div class="col-lg-6 padd-l-0">
                                <uc1:TextBox_Padrao runat="server" ID="txtPesquisa" Modo="Pesquisa" Placeholder="Pesquisar" />
                            </div>
                            <div class="col-lg-2">
                                <uc1:DropDownList_Padrao runat="server" ID="ddlStatus">
                                    <asp:ListItem Value="S" Text="Ativos" Selected="True" />
                                    <asp:ListItem Value="N" Text="Excluídos" />
                                </uc1:DropDownList_Padrao>
                            </div>
                            <div class="col-lg-4 padd-r-0">
                                <asp:LinkButton runat="server" ID="cmdPesquisar" class="btn btn-md btn-primary" Text="Pesquisar" OnClientClick="MudarBotao_Pesquisa(this)" OnClick="cmdPesquisar_Click" />
                                <asp:LinkButton runat="server" ID="cmdNovo" class="btn btn-md btn-success" Text="Novo" OnClientClick="AbrirModal_Detalhe(); return false;" />
                                <asp:LinkButton runat="server" ID="cmdAtivar" class="btn btn-md btn-success float-r" data-toggle="tooltip" title="Reativar selecionados" OnClick="cmdAtivar_Click"><i class="fa fa-refresh"></i></asp:LinkButton>
                                <asp:LinkButton runat="server" ID="cmdExcluir" class="btn btn-md btn-danger float-r" data-toggle="tooltip" title="Excluir selecionados" OnClick="cmdExcluir_Click"><i class="fa fa-trash"></i></asp:LinkButton>
                            </div>

                        </div>
                    </div>
                </asp:Panel>

                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                </div>

                <asp:Panel runat="server" ID="pnResultado" class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-body">

                            <div class="col-lg-12 padd-0">
                                <asp:Literal runat="server" ID="ltrOcultarColunas" />
                            </div>

                            <div class="col-lg-12 padd-0">
                                <div class="table-responsive">
                                    <asp:GridView ID="gvNBS" class="table table-striped table-bordered table-hover table-condesed m-0"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        Font-Names="Tahoma" ShowFooter="False" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvNBS_RowDataBound">
                                        <Columns>

                                            <asp:TemplateField HeaderText="ID">
                                                <ItemTemplate>
                                                    <asp:LinkButton runat="server" ID="lnkID" CssClass="lnkID" Text='<%# Eval("idNBS") %>' OnClientClick="AbrirModal_Detalhe($(this)); return false;" />
                                                </ItemTemplate>
                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="NBS">
                                                <ItemTemplate>
                                                    <asp:LinkButton runat="server" CssClass="lnkNBS" Text='<%# Eval("nCodigo") %>' tt-format="9.9999.99.99" OnClientClick="AbrirModal_Detalhe($(this)); return false;" />
                                                </ItemTemplate>
                                                <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Descrição">
                                                <ItemTemplate>
                                                    <asp:LinkButton runat="server" CssClass="lnkDescricao" Text='<%# Eval("sDescricao") %>' OnClientClick="AbrirModal_Detalhe($(this)); return false;" />
                                                </ItemTemplate>
                                                <ItemStyle Width="50%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="dtAtualizacao" HeaderText="Atualizado Em" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}">
                                                <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="lblData" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sUsuarioAtualizacao" HeaderText="Por">
                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="lblUsuario" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="">
                                                <HeaderTemplate>
                                                    <asp:CheckBox runat="server" ID="chkExcluir_Todos" CssClass="form-check form-check-danger form-check-x form-check-todos" />
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:CheckBox runat="server" ID="chkExcluir" CssClass="form-check form-check-danger form-check-x form-check-unico" />
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

            <div class="modal fade" id="modalNBS" x-data>
                <div class="modal-dialog modal-medio">
                    <div class="modal-content">

                        <div class="modal-header">
                            <div class="modal-title">
                                <img class="modal-logo" src="../../../img/instrucaoTecnica_pdf.jpg" alt="Logo TT" />
                                <label runat="server" id="lblTitulo_ModalNBS" class="sTituloNBS"></label>
                                <button type="button" id="btnFechar" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span></button>
                            </div>
                        </div>
                        <div class="modal-body">

                            <asp:UpdatePanel runat="server">
                                <ContentTemplate>

                                    <div class="row">
                                        <div class="col-lg-12">
                                            <uc1:MensagemPagina runat="server" ID="MensagemPagina_ModalNBS" />
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-lg-1 form-group">
                                            <uc1:TextBox_Padrao runat="server" ID="txtidNBS" Classe="sIdNBS" Titulo="ID" ReadOnly="true" />
                                        </div>
                                    </div>

                                    <div class="row">

                                        <div class="col-lg-6 form-group padd-r-0">
                                            <uc1:TextBox_Padrao runat="server" ID="txtCodigo" Classe="sCodigoNBS" Titulo="Código NBS" Obrigatorio="true" Validacao="true" Placeholder="0.0000.00.00" Mascara="9.9999.99.99" />
                                        </div>

                                        <div class="col-lg-12">
                                            <uc1:TextBox_Padrao runat="server" ID="txtsDescricao" Classe="sDescricaoNBS" Titulo="Descrição" Obrigatorio="true" Validacao="true" Modo="Multilinha" Rows="10" />
                                        </div>

                                    </div>

                                    <div id="div_PainelAtualizacao" class="row">
                                        <div class="col-lg-12">
                                            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                                        </div>
                                    </div>

                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton runat="server" ID="cmdSalvar" CssClass="btn btn-success" Text="Salvar" OnClick="cmdSalvar_Click" />
                            <asp:LinkButton runat="server" ID="cmdCancelar" CssClass="btn btn-danger" Text="Cancelar" data-dismiss="modal" />
                        </div>

                    </div>
                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="cmdPesquisar" EventName="Click" />
            <asp:PostBackTrigger ControlID="cmdSalvar" />
            <asp:PostBackTrigger ControlID="cmdAtivar" />
            <asp:PostBackTrigger ControlID="cmdExcluir" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>