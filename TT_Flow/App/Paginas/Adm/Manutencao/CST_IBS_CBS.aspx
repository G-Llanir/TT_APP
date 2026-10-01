<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="CST_IBS_CBS.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Manutencao.CST_IBS_CBS" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>
<%@ Register Src="~/App/Controles/TextBox_Padrao.ascx" TagPrefix="uc1" TagName="TextBox_Padrao" %>
<%@ Register Src="~/App/Controles/DropDownList_Padrao.ascx" TagPrefix="uc1" TagName="DropDownList_Padrao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        .form-check input {
            margin: 0;
        }
    </style>

    <asp:UpdatePanel runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <ContentTemplate>

            <asp:HiddenField runat="server" ID="hddidCST" Value="0" />

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1><span id="lblTituloPagina">CST IBS/CBS</span>&nbsp;<small>Consulta</small></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="CST IBS/CBS" />
                </div>

                <asp:Panel runat="server" ID="pnConsulta" class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;Filtro para pesquisa</h3>
                        </div>
                        <div class="panel-body">

                            <div class="col-lg-4 padd-l-0">
                                <uc1:TextBox_Padrao runat="server" ID="txtPesquisa" Modo="Pesquisa" Placeholder="Pesquisar" />
                            </div>
                            <div class="col-lg-6 padd-r-0">
                                <asp:Button ID="cmdPesquisar" class="btn btn-md btn-primary" runat="server" Text="Pesquisar" OnClientClick="MudarBotao_Pesquisa(this)" OnClick="cmdPesquisar_Click" />
                                <asp:Button ID="cmdNovo" class="btn btn-md btn-success" runat="server" Text="Novo" OnClick="cmdNovo_Click" />
                            </div>

                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel runat="server" ID="pnResultado" class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-body">

                            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                            <div runat="server" id="div_gvCST" class="table-responsive">
                                <asp:GridView ID="gvCST" class="table table-striped table-bordered table-hover table-condesed m-0"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    Font-Names="Tahoma" ShowFooter="False" Font-Overline="False" Font-Size="14px" OnRowCommand="gvCST_RowCommand">
                                    <Columns>

                                        <asp:TemplateField HeaderText="CST">
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" ID="lnkCodigo" Text='<%# Eval("sCodigo") %>' CommandName="Detalhe" CommandArgument='<%# Eval("idCST") %>' />
                                            </ItemTemplate>
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Descrição">
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" ID="lnkDescricao" Text='<%# Eval("sDescricao") %>' CommandName="Detalhe" CommandArgument='<%# Eval("idCST") %>' />
                                            </ItemTemplate>
                                            <ItemStyle Width="40%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Classificações (cClassTrib)">
                                            <ItemTemplate>
                                                <div class="tagList">
                                                    <asp:Literal runat="server" Text='<%# Eval("sFilhos") %>'></asp:Literal>
                                                </div>
                                            </ItemTemplate>
                                            <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="dtAtualizacao" HeaderText="Atualizado Em" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}">
                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sUsuarioAtualizacao" HeaderText="Por">
                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                    </Columns>
                                </asp:GridView>
                            </div>

                        </div>
                    </div>
                </asp:Panel>
            </div>

            <div class="modal fade" id="modalCST">
                <div class="modal-dialog modal-largo">
                    <div class="modal-content">

                        <div class="modal-header">
                            <div class="modal-title">
                                <img class="modal-logo" src="../../../img/instrucaoTecnica_pdf.jpg" alt="Logo TT" />
                                <label runat="server" id="lblTitulo_ModalCST"></label>
                                <button type="button" id="btnFechar" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span></button>
                            </div>
                        </div>
                        <div class="modal-body">

                            <asp:UpdatePanel runat="server">
                                <ContentTemplate>

                                    <div id="hdd">
                                        <asp:HiddenField runat="server" ID="hddidFilho" Value="0" />
                                        <asp:HiddenField runat="server" ID="hdd_divIncluirFilho" Value="N" />
                                    </div>

                                    <div class="row">
                                        <div class="col-lg-12">
                                            <uc1:MensagemPagina runat="server" ID="MensagemPagina_ModalCST" />
                                        </div>
                                    </div>

                                    <div id="divCST_Pai">
                                        <div class="row">
                                            <div class="col-lg-1 form-group">
                                                <uc1:TextBox_Padrao runat="server" ID="txtidCST" Titulo="ID" ReadOnly="true" />
                                            </div>
                                        </div>

                                        <div class="row">

                                            <div class="col-lg-1 form-group padd-r-0">
                                                <uc1:TextBox_Padrao runat="server" ID="txtCodigo" Titulo="CST" Obrigatorio="true" Validacao="true" MaxLength="3" Modo="Inteiro" />
                                            </div>

                                            <div class="col-lg-5 form-group">
                                                <uc1:TextBox_Padrao runat="server" ID="txtsDescricao" Titulo="Descrição" Obrigatorio="true" Validacao="true" />
                                            </div>

                                            <div class="col-lg-2">
                                                <uc1:SwitchAtivo runat="server" ID="sExigeTrib" />
                                            </div>
                                            <div class="col-lg-2">
                                                <uc1:SwitchAtivo runat="server" ID="sReducaoBC" />
                                            </div>
                                            <div class="col-lg-2">
                                                <uc1:SwitchAtivo runat="server" ID="sReducaoAliq" />
                                            </div>

                                        </div>
                                    </div>

                                    <div id="divCST_Filho" class="row">
                                        <div class="col-lg-12">

                                            <div id="div_IncluirFilhos" class="panel panel-default">
                                                <div class="panel-heading cursor-pointer" data-toggle="collapse" data-target="#divIncluirFilho">
                                                    <div class="d-flex space-b">
                                                        <h3 class="panel-title"><b>Nova Classificação</b></h3>
                                                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-default" OnClientClick="return false;"><i class="fa fa-plus"></i></asp:LinkButton>
                                                    </div>
                                                </div>
                                                <div id="divIncluirFilho" class="panel-collapse collapse">
                                                    <div class="panel-body">

                                                        <div class="row">
                                                            <div class="col-lg-3 form-group">
                                                                <uc1:TextBox_Padrao runat="server" ID="txtnCodigo_Filho" Titulo="Código" Obrigatorio="true" Validacao="true" MaxLength="6" Modo="Inteiro" />
                                                            </div>

                                                            <div class="col-lg-3 form-group">
                                                                <uc1:DropDownList_Padrao runat="server" ID="ddlidTipoAliq" Titulo="Tipo de Alíquota" Obrigatorio="true" Validacao="true">
                                                                    <asp:ListItem Value="0" Text="Teste" />
                                                                    <asp:ListItem Value="1" Text="Fixa" />
                                                                    <asp:ListItem Value="2" Text="Padrão" Selected="True" />
                                                                    <asp:ListItem Value="3" Text="Sem Alíquota" />
                                                                    <asp:ListItem Value="4" Text="Uniforme Nacional" />
                                                                    <asp:ListItem Value="5" Text="Uniforme Setorial" />
                                                                </uc1:DropDownList_Padrao>
                                                            </div>

                                                            <div class="col-lg-3 form-group">
                                                                <uc1:TextBox_Padrao runat="server" ID="txtRedIBS" Titulo="Redução IBS" Modo="Decimal" TamanhoTotal="5" SeparadorMilhar="false" Grupo_Simbolo="<span class='symbol'><i class='fa fa-percent'></i></span>" />
                                                            </div>

                                                            <div class="col-lg-3 form-group">
                                                                <uc1:TextBox_Padrao runat="server" ID="txtRedCBS" Titulo="Redução CBS" Modo="Decimal" TamanhoTotal="5" SeparadorMilhar="false" Grupo_Simbolo="<span class='symbol'><i class='fa fa-percent'></i></span>" />
                                                            </div>
                                                        </div>

                                                        <div class="row">
                                                            <div class="col-lg-12 form-group">
                                                                <uc1:TextBox_Padrao runat="server" ID="txtsDescricao_Filho" Titulo="Descrição" Modo="Multilinha" Obrigatorio="true" Validacao="true" Rows="3" />
                                                            </div>
                                                        </div>

                                                        <div class="row padd-l">
                                                            <asp:LinkButton runat="server" ID="lnkIncluirFilho" CssClass="btn btn-md btn-info" Text="Incluir" OnClick="lnkIncluirFilho_Click"></asp:LinkButton>
                                                        </div>

                                                    </div>
                                                </div>
                                            </div>

                                            <div runat="server" id="div_gvFilhos" class="panel panel-default m-0">
                                                <div class="panel-heading">
                                                    <div class="d-flex space-b">
                                                        <h3 class="panel-title"><b>Classificações (cClassTrib)</b></h3>
                                                        <asp:LinkButton runat="server" ID="lnkExcluir_Filhos" CssClass="btn btn-md btn-danger" OnClick="lnkExcluir_Filhos_Click" data-toggle="tooltip_top" title="Excluir Itens selecionados" OnClientClick="$('#cphCorpo_hdd_divIncluirFilho').val($('#divIncluirFilho').hasClass('in') ? 'S' : 'N');">
                                                            <i class="fa fa-trash"></i>
                                                        </asp:LinkButton>
                                                    </div>
                                                </div>
                                                <div class="panel-body">
                                                    <div class="table-responsive">
                                                        <asp:GridView ID="gvFilhos" class="table table-striped table-bordered table-hover table-condesed m-0"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            Font-Names="Tahoma" ShowFooter="False" Font-Overline="False" Font-Size="Small" OnRowCommand="gvFilhos_RowCommand">
                                                            <Columns>

                                                                <asp:BoundField DataField="idCST_Filho" HeaderText="ID">
                                                                    <HeaderStyle CssClass="invisivel" />
                                                                    <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="invisivel" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sCodigo" HeaderText="Código">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sDescricao" HeaderText="Descrição">
                                                                    <ItemStyle Width="45%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sTipoAliq" HeaderText="Tipo">
                                                                    <ItemStyle Width="17%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="nRedIBS" HeaderText="Redução IBS" DataFormatString="{0:N2}">
                                                                    <ItemStyle Width="13%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="nRedCBS" HeaderText="Redução CBS" DataFormatString="{0:N2}">
                                                                    <ItemStyle Width="13%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton runat="server" ID="lnkEditar" CommandName="Editar" CommandArgument='<%# Eval("idCST_Filho") %>' data-toggle="tooltip_top" title="Editar"><i class="fa fa-2x fa-pencil"></i></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="1%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="content-center" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField>
                                                                    <HeaderTemplate>
                                                                        <asp:CheckBox runat="server" ID="chkExcluir_Todos" ClientIDMode="Static" data-check-todos="Excluir" CssClass="form-check form-check-danger form-check-x form-check-todos" />
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox runat="server" ID="chkExcluir" ClientIDMode="Static" data-check-todos="Excluir" CssClass="form-check form-check-danger form-check-x form-check-unico" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="1%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row">
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
            <asp:AsyncPostBackTrigger ControlID="cmdNovo" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="gvCST" EventName="RowCommand" />
            <asp:PostBackTrigger ControlID="cmdSalvar" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>