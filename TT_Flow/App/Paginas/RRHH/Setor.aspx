<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Setor.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.Setor" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <script type="text/javascript">
                function MudarBotao() {
                    document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
                }
            </script>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                    <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label><small> Manutenção</small></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa </h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group">
                                <div class="col-lg-4">
                                    <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-lg-5">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                    <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" OnClick="cmdNovo_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>


                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    <asp:Panel ID="pnResultado" class="" runat="server">
                        <div id="resultado">
                            <div class="panel panel-primary">
                                <div class="panel-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" DataKeyNames="idSetor" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound" OnRowCommand="dtgvConsulta_RowCommand" >
                                            <Columns>

                                                    <asp:ButtonField DataTextField="idSetor" HeaderText="ID" ItemStyle-CssClass="Setor" CommandName="Consulta" ItemStyle-Width="8%" />
                                                    <asp:ButtonField DataTextField="sDscSetor" HeaderText="Setor Principal" ItemStyle-CssClass="Setor" CommandName="Consulta" ItemStyle-Width="25%" />
                                                    <asp:HyperLinkField DataNavigateUrlFields="sDscSetorFilhos"
                                                        DataTextField="sDscSetorFilhos" HeaderText="Setores Secundários"
                                                        DataNavigateUrlFormatString="GHE_Detalhe.aspx?id={0}">
                                                        <HeaderStyle Width="35%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:HyperLinkField>                                        

                                                    <asp:ButtonField DataTextField="dtAtualizacao" HeaderText="Data Atualizacação" ItemStyle-CssClass="Setor" CommandName="Consulta" ItemStyle-Width="12%" />
                                                    <asp:ButtonField DataTextField="sDscUsuario" HeaderText="Por" ItemStyle-CssClass="Setor" CommandName="Consulta" ItemStyle-Width="12%" />
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
    
    <div class="modal fade" id="modalForm" tabindex="-2" role="dialog" aria-labelledby="modalFormLabel" aria-hidden="true" data-backdrop="static">
        <div class="modal-dialog modal-sm" role="document" style="width: 60%">

        <div class="modal-content">
             <asp:UpdatePanel ID="updModal" runat="server" >

                <ContentTemplate>
                <asp:HiddenField runat="server" ID="hddidSetor" />
                    <div class="modal-header">
                        <button type="button" id="btnHeadFecharDetalhe" class="close" data-dismiss="modal" aria-label="Fechar">
                            <span aria-hidden="true">&times;</span>
                        </button>
                        <div class="modal-header-content">
                            <div class="modal-logo">
                                <asp:Image ID="Image2" runat="server" ImageUrl="~/App/img/LogoTT.png" AlternateText="LogoTipo" />
                            </div>

                            <div class="modal-title-container">
                                 <asp:Label runat="server" class="modal-title" ID="lbltituloModal" Text="Detalhe" Font-Bold="true"></asp:Label>
                            
                            </div>
                        </div>
                    </div>
                <div class="modal-body">
                <div class="panel-body">

                    <div class="col-lg-12">
                        <uc1:MensagemPagina runat="server" ID="MensagemPaginaModalDetalhe" />
                    </div>
                    <div class="col-lg-12">
                        <div class="form-group">
                            <label>ID</label>
                            <asp:TextBox ID="txtidSetor" runat="server" class="form-control CaixaTextoMini" disabled=""></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-lg-12">
                        <div class="form-group">
                            <label>Setor Principal</label>
                            <asp:TextBox ID="txtsDscSetor" class="form-control"  MaxLength="200"  runat="server"></asp:TextBox>
                        </div>
                    </div>

                    <div class="col-lg-10 form-group" runat="server" id="DIV_Setores">  
                                                   
                    <br />
                        <div class="panel panel-default" >
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Setores Secundários</b></h3>
                            </div>
                            <div class="panel-body">
                                <div id="DIV_Setor_filhos" runat="server">
                                <div class="form-group row">
                                    <div class="col-lg-12">
                                        <asp:Button ID="cmdIncluirSetor" class="btn btn-info" runat="server" Text="Adicionar Setor" OnClick="cmdIncluirSetor_Click" />
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <div class="col-lg-12">
                                        <asp:GridView ID="gvSetor" class="table table-striped table-bordered table-hover table-condensed"
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" DataKeyNames="idRegistro" 
                                            OnRowDeleting="gvSetor_RowDeleting" OnRowDataBound="gvSetor_RowDataBound">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Ordem">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtnOrdem" runat="server" class="form-control" Text='<%# Eval("nOrdem") %>' ClientIDMode="Static"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Opção">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtsDscSetor" runat="server" class="form-control" Text='<%# Eval("sDscSetor") %>' MaxLength="200" ClientIDMode="Static"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="80%" />
                                                </asp:TemplateField>

                                            <%--     <asp:TemplateField HeaderText="Justificativa">
                                                        <ItemTemplate>
                                                            <asp:DropDownList runat="server" ID="ddlsJustificativa" class="form-control Caixa_Selecao">
                                                                <asp:ListItem Text="Não" Value="N"></asp:ListItem>
                                                                <asp:ListItem Text="Sim" Value="S"></asp:ListItem>
                                                            </asp:DropDownList>    
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="80%" />
                                                    </asp:TemplateField>
            --%>
                                                <asp:TemplateField HeaderText="">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="cmdOpcoes_Excluir" runat="server" CssClass="btn btn-small" TabIndex="100" ToolTip="Apagar Registro" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-12 form-group">
                        <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                    </div>

                </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="cmdSalvar" Text="Salvar" runat="server" CssClass="btn btn-success" OnClick="cmdSalvar_Click" />
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Fechar</button>
                </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="cmdIncluirSetor" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        </div>
    </div>


    
     <div id="dialog-Salvar" class="modal modal-salvar" title="Salvar" >
     <p>
         <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
         <asp:Label ID="lblTituloSalvar"  runat="server" Text="Deseja Salvar?"></asp:Label>
     </p>
    </div>

</asp:Content>
