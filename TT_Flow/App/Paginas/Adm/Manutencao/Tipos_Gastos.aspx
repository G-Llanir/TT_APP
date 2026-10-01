<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Tipos_Gastos.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Manutencao.Tipos_Gastos" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        .modal-title {
            color: #009A22;
            text-shadow: 2px 2px 2px rgba(0, 0, 0, 0.2);
            font-size: 24px;
            font-weight: bold;
            margin: 0;
        }

        .modal-largo {
            width: 80% !important;
            max-width: none !important;
        }

        .modal-Medio {
            width: 60% !important;
            max-width: none !important;
        }

        .modal-Pequeno {
            width: 35% !important;
            max-width: none !important;
        }

        .modal-logo img {
            max-width: 50px;
            margin-right: 10px;
        }

        .modal-header-content {
            display: flex;
            align-items: center;
        }

        #btnFechar {
            position: absolute;
            top: 20px;
            right: 20px;
        }

        .modal-footer {
            text-align: left !important;
        }

        .modal-backdrop.show {
            z-index: 1060;
        }

        .modal.show {
            z-index: 1070;
        }

        .modal-backdrop.fade.show {
            display: none;
        }
    </style>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Tipos de Despesas"></asp:Label>
                <small class="text-muted"></small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="Tipos de Despesas" />
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>
        <div class="col-lg-12">

            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa </h3>
                </div>
                <div class="panel-body">
                    <div class="form-group">

                        <div class="row">

                            <div class="col-lg-3">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlidCategoriaFiltro" runat="server" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-3">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlidRecursoFiltro" runat="server" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-lg-3">
                                <div class="form-group">
                                    <asp:TextBox ID="txtStatus" class="form-control" runat="server" placeholder="Pesquisar"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-1">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlsAtivo" runat="server" class="form-control yes_no select Caixa_Selecao">
                                        <asp:ListItem Value="">Selecione</asp:ListItem>
                                        <asp:ListItem Selected="True" Value="S">Ativo</asp:ListItem>
                                        <asp:ListItem Value="N">Inativo</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        

                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-md btn-primary" runat="server" Text="Pesquisar" />
                                    <asp:Button ID="cmdNovo" class="btn btn-md btn-success" runat="server" Text="Novo" OnClick="cmdNovo_Click" />
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>

        <div class="col-lg-12">
            <asp:Panel ID="div_gvRelatorio" class="" runat="server">
                <div id="resultado">
                    <div class="panel panel-primary">
                        <div class="panel-body">
                            <div class="table-responsive">
                                <asp:GridView ID="gvRelatorio" class="table table-striped table-bordered table-hover"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    Font-Names="Tahoma" ShowFooter="False" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvRelatorio_RowDataBound"
                                    OnRowCommand="gvRelatorio_RowCommand">
                                    <Columns>

                                        <asp:BoundField DataField="idTipoGastos" HeaderText="ID">
                                            <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscRecurso" HeaderText="Recurso de Vendas">
                                            <ItemStyle Width="18%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:TemplateField HeaderText="Descrição">
                                            <HeaderStyle Width="20%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkDescricao" runat="server" CommandName="Pesquisar" CommandArgument='<%# Eval("idTipoGastos") %>' Text='<%# Eval("sDscGasto") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Categoria Contas a Pagar">
                                            <HeaderStyle Width="18%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkCategoria" runat="server" CommandName="Pesquisar" CommandArgument='<%# Eval("idTipoGastos") %>' Text='<%# Eval("sDscCategoriaPagar") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="vlrMaximo" HeaderText="Valor Diário" DataFormatString="{0:C2}">
                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sAtivo" HeaderText="Ativo">
                                            <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Atualização">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário Atualização">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>

    <div class="modal fade" id="modal_Tipos" tabindex="-2" role="dialog" data-backdrop="static" aria-labelledby="modalFollowUpLabel" aria-hidden="true" style="height: 100%;">
        <div class="modal-dialog modal-sm modal-Medio" role="document" style="width: 45%;">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="modalFollowUpLabel">Tipos de Despesa</h5>
                </div>
                <div class="modal-body">

                    <div class="col-lg-12">
                        <div class="form-group">

                            <div class="col-lg-12 row">
                                <uc1:MensagemPagina runat="server" ID="MensagemPagina1" />
                            </div>

                            <div class="col-lg-12 row">
                                <div class="col-lg-2 row">
                                    <div class="form-group">
                                        <label>ID</label>
                                        <asp:TextBox ID="txtidTipoGastos" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-12 row">
                                <div class="col-lg-6 row">
                                    <div class="form-group">
                                        <label>Recurso de Vendas</label>
                                        <asp:DropDownList ID="ddlidRecurso" runat="server" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-12 row">
                                <div class="col-lg-6 row">
                                    <div class="form-group">
                                        <label>Descrição</label>
                                        <asp:TextBox ID="txtsDscGasto" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-12 row">
                                <div class="col-lg-6 row">
                                    <div class="form-group">
                                        <label>Categoria Contas a Pagar</label>
                                        <asp:DropDownList ID="ddlidCategoriaPagar" runat="server" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-12 row">
                                <div class="col-lg-2 row">
                                    <div class="form-group">
                                        <label>Valor Diário (R$)</label>
                                        <asp:TextBox ID="txtvlrMaximo" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-12 row">
                                <div class="col-lg-1 row">
                                    <div class="form-group">
                                        <uc1:ComboAtivo runat="server" ID="ddlComboAtivo" />
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-12 row">
                                <div class="form-group">
                                    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                                </div>
                            </div>

                            <div class="col-lg-12 row">
                                <div class="col-lg-4 row">
                                    <div class="form-group">
                                        <br />
                                        <asp:LinkButton ID="cmdSalvar" CssClass="btn btn-success" Text="Salvar" runat="server" OnClick="cmdSalvar_Click" />
                                        <asp:LinkButton CssClass="btn btn-danger" Text="Cancelar" runat="server" data-dismiss="modal" />
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>


    <div>
        <asp:HiddenField ID="hddidTipoGastos" runat="server" />
           <asp:HiddenField ID="hddidRecursoFiltro" runat="server" />
           <asp:HiddenField ID="hddidCategoriaPagarfiltro" runat="server" />
    </div>
</asp:Content>
