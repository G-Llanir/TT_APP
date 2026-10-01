<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="CFOP.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Manutencao.CFOP" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>

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

        .modal-content {
            overflow-y: inherit !important;
        }
    </style>

    <asp:UpdatePanel runat="server">
        <ContentTemplate>
    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="CFOP"></asp:Label>
                <small class="text-muted"></small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="CFOP" />
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>
        <div class="col-lg-12">

            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa </h3>
                </div>
                <div class="panel-body">
                    <div class="form-group">
                        <div class="col-lg-4 row">
                            <div class="form-group">
                                <asp:TextBox ID="txtsPesquisa" class="form-control" runat="server" placeholder="Pesquisar"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-lg-2">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlTipo" runat="server" class="form-control yes_no select Caixa_Selecao">
                                    <asp:ListItem Value="-1" Selected="True">Todos os Tipos</asp:ListItem>
                                    <asp:ListItem Value="0">Entrada</asp:ListItem>
                                    <asp:ListItem Value="1">Saida</asp:ListItem>
                                </asp:DropDownList>
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
                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:Button ID="cmdPesquisar" class="btn btn-md btn-primary" runat="server" Text="Pesquisar" />
                                <asp:Button ID="cmdNovo" class="btn btn-md btn-success" runat="server" Text="Novo" OnClick="cmdNovo_Click" />
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

                                        <asp:BoundField DataField="idCFOP" HeaderText="ID">
                                            <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:TemplateField HeaderText="CFOP">
                                            <HeaderStyle Width="4%" />
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkDescricao" runat="server" CommandName="Pesquisar" CommandArgument='<%# Eval("idCFOP") %>' Text='<%# Eval("sCFOP") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Descrição">
                                            <HeaderStyle Width="30%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkCategoria" runat="server" CommandName="Pesquisar" CommandArgument='<%# Eval("idCFOP") %>' Text='<%# Eval("sDescricao") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="sTpNF" HeaderText="Tipo">
                                            <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sCSTICMS" HeaderText="CST ICMS">
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="cBenef" HeaderText="Benef Fiscal">
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sCSTIPI" HeaderText="CST IPI">
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sCSTPIS" HeaderText="CST PIS">
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sCSTCOFINS" HeaderText="CST COFINS">
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sTotal" HeaderText="Total">
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sMovimentacao" HeaderText="Mov">
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sPagamento" HeaderText="Pagamento">
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sRecebimento" HeaderText="Recebimento">
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDevolucao" HeaderText="Devolução">
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sAtivo" HeaderText="Ativo">
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
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


        </ContentTemplate>
    </asp:UpdatePanel>


        <div class="modal fade" id="modal_Tipos" tabindex="-2" role="dialog" data-backdrop="static" aria-labelledby="modalFollowUpLabel" aria-hidden="true" style="height: 100%;">
        <div class="modal-dialog modal-sm modal-Medio" role="document" style="width: 45%;">
            <div class="modal-content">
                 <div class="modal-header">
                     <button type="button" id="btnFechar" class="close" data-dismiss="modal" aria-label="Fechar">
                         <span aria-hidden="true">&times;</span>
                     </button>
                     <div class="modal-header-content">
                         <div class="modal-logo">
                             <asp:Image ID="imgLogo" runat="server" ImageUrl="~/App/img/LogoTT.png" AlternateText="LogoTipo" />
                         </div>
                         <div class="modal-title-container">
                             <asp:Label runat="server" class="modal-title" ID="lblModal" Text="CFOP" Font-Bold="true"></asp:Label>
                         </div>
                     </div>
                 </div>
                
                 <div class="modal-body">
                    <asp:UpdatePanel runat="server"  >
                    <ContentTemplate>
    
                    <div class="form-group row">
                        <div class="col-lg-12">
                            <uc1:MensagemPagina runat="server" ID="MensagemPagina1" />
                        </div>

                        <div class="col-lg-12 row">
                            <div class="col-lg-2">
                                <div class="form-group">
                                    <label>ID</label>
                                    <asp:TextBox ID="txtidCFOP" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-12 row">

                            <div class="col-lg-3">
                                <div class="form-group">
                                    <label>Tipo</label>
                                    <asp:DropDownList ID="ddltpNF" runat="server" class="form-control Caixa_Selecao">
                                        <asp:ListItem Value="">Selecione o Tipo</asp:ListItem>
                                        <asp:ListItem Value="0">Entrada</asp:ListItem>
                                        <asp:ListItem Value="1">Saída</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-2">
                                <div class="form-group">
                                    <label>CFOP</label>
                                    <asp:TextBox ID="txtsCFOP" class="form-control" MaxLength="4" runat="server"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-7">
                                <div class="form-group">
                                    <label>Descrição</label>
                                    <asp:TextBox ID="txtsDescricao" class="form-control" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-12 row">
                            <div class="col-lg-3">
                                <div class="form-group">
                                    <label>CST ICMS</label>
                                    <asp:DropDownList ID="ddlidCSTICMS" runat="server" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlidCSTICMS_SelectedIndexChanged" > </asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-3" id="div_cBenef" runat="server">
                                <div class="form-group">
                                    <label>Código Benefício</label>
                                    <asp:DropDownList ID="ddlidcBenef" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>
                            </div>


                            <div class="col-lg-3">
                                <div class="form-group">
                                    <label>CST IPI</label>
                                    <asp:DropDownList ID="ddlidCSTIPI" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-3">
                                <div class="form-group">
                                    <label>CST PIS</label>
                                    <asp:DropDownList ID="ddlidCSTPIS" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-3">
                                <div class="form-group">
                                    <label>CST COFINS</label>
                                    <asp:DropDownList ID="ddlidCSTCOFINS" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-2" runat="server" id="DIV_Total">
                            <uc1:SwitchAtivo runat="server" ID="sTotal" />
                        </div>

                        <div class="col-lg-2" runat="server" id="DIV_sMovimentacao">
                            <uc1:SwitchAtivo runat="server" ID="sMovimentacao" />
                        </div>

                        <div class="col-lg-2" runat="server" id="DIV_sPagamento">
                            <uc1:SwitchAtivo runat="server" ID="sPagamento" />
                        </div>

                        <div class="col-lg-2" runat="server" id="DIV_sRecebimento">
                            <uc1:SwitchAtivo runat="server" ID="sRecebimento" />
                        </div>

                        <div class="col-lg-2" runat="server" id="DIV_sDevolucao">
                            <uc1:SwitchAtivo runat="server" ID="sDevolucao" />
                        </div>

                        <div class="col-lg-2" runat="server" id="DIV_Faturamento">
                            <uc1:SwitchAtivo runat="server" ID="sFaturamento" />
                        </div>

                        <div class="col-lg-2" runat="server" id="DIV_sAtivo">
                            <uc1:SwitchAtivo runat="server" ID="sAtivo" />
                        </div>

                        <div class="col-lg-12">
                            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                        </div>
                       
                    </div>
                    
                    
                    </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer text-left">
                    <asp:LinkButton ID="cmdSalvar" CssClass="btn btn-success" Text="Salvar" runat="server" OnClick="cmdSalvar_Click" />
                    <asp:LinkButton CssClass="btn btn-danger" Text="Cancelar" runat="server" data-dismiss="modal" />
                </div>
            </div>
        </div>
    </div>


    <div>
        <asp:HiddenField ID="hddidCFOP" runat="server" />
    </div>
</asp:Content>
