<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="TipoCentroCusto_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Manutencao.TipoCentroCusto_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <asp:UpdatePanel ID="UpdDetalhe" runat="server">
        <ContentTemplate>
            <style>
                .gvHeader th {
                    padding: 7px;
                    background-color: #1A4C1A;
                    color: #fff;
                    border: 1px solid #bbb;
                    font-weight: normal;
                }

                .gvChildHeader th {
                    padding: 4px;
                    background-color: #999966;
                    color: #fff;
                    border: 1px solid #bbb;
                    font-weight: normal;
                }

                .gvRow td, .gvAltRow td {
                    padding: 7px;
                    border: 1px solid #bbb;
                }

                .gvRow td {
                    background-color: #ffffff;
                }

                .gvAltRow td {
                    background-color: #f1f1f1;
                }

                .collapsed-row {
                    display: none;
                    padding: 1px;
                    margin: 1px;
                }
            </style>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>

                <div>
                    <ul id="tab_TipoCentroCusto" class="nav nav-tabs" role="tablist">
                        <li role="presentation" class="active">
                            <a href="#TipoCentroCusto" id="aba_TipoCentroCusto" role="tab" data-toggle="tab" aria-controls="TipoCentroCusto" aria-expanded="true">
                                <b>Tipos - Centro de Custo</b>
                            </a>
                        </li>
                    </ul>
                </div>

                <br />

                <div id="tab" class="tab-content">
                    <div role="tabpanel" class="tab-pane fade in active" id="TipoCentroCusto">
                        <div class="panel panel-default" runat="server" id="div_TipoCentroCusto">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Tipos - Centro de Custo</b></h3>
                            </div>
                            <div class="panel-body">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>ID</label>
                                        <asp:TextBox ID="txtidTipoCentroCusto" runat="server" CssClass="form-control CaixaTextoMini" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Descrição</label>
                                        <asp:TextBox ID="txtsDscTipo" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
                                    </div>
                                </div>
                            </div>
                        </div>


                        <div class="panel panel-default" runat="server" id="divCategoria">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Categorias</b></h3>
                            </div>
                            <div class="panel-body">
                                <uc1:MensagemPagina runat="server" ID="MensagemPaginaCategoria" />

                                <div class="form-stacked row" id="divFormInclusao" runat="server">
                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>Descrição da Categoria</label>
                                            <asp:TextBox ID="txtsDscCategoria" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Teto de Gastos</label>
                                            <asp:TextBox ID="txtnTetoCategoria" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Saldo</label>
                                            <asp:TextBox ID="txtnSaldoCategoria" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>

                                  <%--  <div class="col-lg-2">
                                        <uc1:SwitchAtivo runat="server" ID="SwitchAtivoPadrao" />
                                    </div>--%>

                                    <div class="col-lg-2" style="margin-top: 24px;">
                                        <asp:LinkButton runat="server" ID="cmdIncluirCategoriaCC" CssClass="btn btn-info"
                                            OnClick="cmdIncluirCategoriaCC_Click" ValidationGroup="InclusaoCategoria">
                                  <i class="fa fa-plus"></i> Incluir na Lista
                                        </asp:LinkButton>
                                    </div>
                                </div>

                                <hr />

                                <!-- GRID DE CATEGORIAS -->
                                <div class="form-stacked row" id="divGridCategoria" runat="server">
                                    <div class="col-lg-12 table-responsive">
                                        <asp:GridView ID="gridCategorias" runat="server" AutoGenerateColumns="false"
                                            CssClass="table table-striped table-bordered"
                                            DataKeyNames="GridKey"
                                            OnRowEditing="gridCategorias_RowEditing"
                                            OnRowCancelingEdit="gridCategorias_RowCancelingEdit"
                                            OnRowUpdating="gridCategorias_RowUpdating"
                                            OnRowDataBound="gridCategorias_RowDataBound"
                                            OnRowDeleting="gridCategorias_RowDeleting"
                                            OnRowCommand="gridCategorias_RowCommand">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Descrição">
                                                    <ItemTemplate><%# Eval("sDscCategoria") %></ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtGridDscCategoria" runat="server" Text='<%# Bind("sDscCategoria") %>' CssClass="form-control"></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Teto">
                                                    <ItemTemplate><%# Eval("nTeto", "{0:C}") %></ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtGridTeto" runat="server" Text='<%# Bind("nTeto", "{0:C}") %>' CssClass="form-control"></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Saldo">
                                                    <ItemTemplate><%# Eval("nSaldo", "{0:C}") %></ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtGridSaldo" runat="server" Text='<%# Bind("nSaldo", "{0:C}") %>' CssClass="form-control"></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>

                                    <%--            <asp:TemplateField HeaderText="Padrão">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkPadrao" runat="server" Checked='<%# Eval("bPadrao") %>' Enabled="false" />
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:CheckBox ID="chkPadrao" runat="server" Checked='<%# Bind("bPadrao") %>' />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>--%>

                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkEditar" runat="server" CommandName="Edit" CssClass="btn btn-default btn-xs" ToolTip="Editar">
                                                  <i class='fa fa-pencil'></i>
                                                        </asp:LinkButton>

                                                        <asp:LinkButton ID="lnkExcluir" runat="server" CommandName="Delete" CssClass="btn btn-danger btn-xs" ToolTip="Excluir">
                                                  <i class="fa fa-trash-o"></i>
                                                        </asp:LinkButton>
                                                    </ItemTemplate>

                                                    <EditItemTemplate>
                                                        <asp:LinkButton ID="lnkSalvar" runat="server" CommandName="Update" CssClass="btn btn-success btn-xs" ToolTip="Salvar">
                                                  <i class='fa fa-check'></i>
                                                        </asp:LinkButton>

                                                        <asp:LinkButton ID="lnkCancelar" runat="server" CommandName="Cancel" CssClass="btn btn-danger btn-xs" ToolTip="Cancelar">
                                                  <i class='fa fa-times'></i>
                                                        </asp:LinkButton>

                                                        <asp:LinkButton ID="lnkConfirmarExclusao" runat="server" CommandName="ConfirmDelete" CssClass="btn btn-success btn-xs" ToolTip="Confirmar Exclusão" CommandArgument='<%# Container.DataItemIndex %>'>
                                                  <i class='fa fa-check'></i>
                                                        </asp:LinkButton>

                                                        <asp:LinkButton ID="lnkCancelarExclusao" runat="server" CommandName="Cancel" CssClass="btn btn-danger btn-xs" ToolTip="Cancelar">
                                                  <i class='fa fa-times'></i>
                                                        </asp:LinkButton>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-lg-12 text-right">
                                        <asp:Button ID="btnSalvarCategorias" runat="server" Text="Salvar Todas as Alterações"
                                            CssClass="btn btn-primary" OnClick="btnSalvarCategorias_Click" visible="false"/>
                                    </div>
                                </div>
                            </div>
                        </div>


                    </div>
                </div>

                <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

                <fieldset class="form-stacked actions">
                    <asp:Button ID="cmdSalvar" CssClass="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                    <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" title="Voltar" onclick="history.go(-1)" />
                </fieldset>

                <asp:HiddenField ID="hddidTipoCentroCusto" runat="server" />
                <asp:HiddenField ID="hddidCategoria" runat="server" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
