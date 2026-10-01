<%@ Page Title="" Language="C#" EnableEventValidation="false" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="CategoriaEscopo_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Comercial.Manutencao.CategoriaEscopo_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/FiltroPesquisa.ascx" TagPrefix="uc1" TagName="FiltroPesquisa" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual"/>

    <asp:UpdatePanel ID="UpdDetalhe" runat="server">
        <ContentTemplate>

            <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>
            </div>

            <br />

            <div id="hdd">
                <asp:HiddenField runat="server" ID="hddidCategoria" />
                <asp:HiddenField runat="server" ID="hddEditarPergunta" />
                <asp:HiddenField runat="server" ID="hddEditarOpcao" />
                <asp:Button runat="server" ID="cmdDetalhe" OnClick="cmdDetalhe_Click" Visible="false" />
            </div>

            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Categoria</b></h3>
                </div>
                <div class="panel-body">
                    <div class="row">

                        <div class="col-lg-12">
                            <div class="col-lg-2 row">
                                <div class="form-group">
                                    <label>ID</label>
                                    <asp:TextBox runat="server" ID="txtidCategoria" class="form-control CaixaTextoMini" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <label>Descrição</label>
                                <asp:TextBox runat="server" ID="txtsDscCategoria" class="form-control"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-12">
                            <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
                        </div>

                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-lg-6">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Perguntas</b></h3>
                        </div>
                        <div class="panel-body">
                            <div runat="server" id="div_IncluirPerguntas" class="row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_IncluirPerguntas" />

                                    <div class="col-lg-2" runat="server" id="div_OrdemPergunta">
                                        <div class="form-group">
                                            <label>Ordem</label>
                                            <asp:TextBox runat="server" ID="txtOrdemPergunta" class="form-control"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-10" runat="server" id="div_Pergunta">
                                        <div class="form-group">
                                            <label>Pergunta</label>
                                            <asp:TextBox runat="server" ID="txtPergunta" class="form-control"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2" runat="server" id="div_cmdAlterarPergunta">
                                        <div class="form-group">
                                            <label>&nbsp;</label>
                                            <asp:Button runat="server" ID="cmdAlterarPergunta" class="btn btn-info form-control" Text="Alterar" OnClick="cmdAlterarPergunta_Click"></asp:Button>
                                        </div>
                                    </div>
                                    
                                    <div class="col-lg-2" runat="server" id="div_cmdIncluirPergunta">
                                        <div class="form-group">
                                            <label>&nbsp;</label>
                                            <asp:Button runat="server" ID="cmdIncluirPergunta" class="btn btn-info form-control" Text="Incluir" OnClick="cmdIncluirPergunta_Click"></asp:Button>
                                        </div>
                                    </div>

                                </div>
                            </div>

                            <uc1:MensagemPagina runat="server" ID="MensagemPagina_gvPergunta" />

                            <div runat="server" id="div_gvPergunta" class="col-lg-12">

                                <asp:GridView ID="gvPergunta" class="table table-striped table-bordered table-hover "
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" DataKeyNames="Key" OnRowDataBound="gvPergunta_RowDataBound" OnRowDeleting="gvPergunta_RowDeleting" OnRowCommand="gvPergunta_RowCommand">
                                    <Columns>

                                        <asp:TemplateField HeaderText="Ordem">
                                            <ItemTemplate>
                                                <asp:Label ID="lblidPergunta" runat="server" Text='<%# Eval("Value") %>' ClientIDMode="Static"></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Descrição da Pergunta">
                                            <ItemTemplate>
                                                <asp:Label ID="lblsDscPergunta" runat="server" Text='<%# Eval("Value") %>' ClientIDMode="Static"></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="85%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Editar">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnEditar" runat="server" CssClass="btn btn-small" TabIndex="99" CommandName="Editar" CommandArgument='<%# Eval("Key") %>' ClientIDMode="Static"><i class="fa fa-pencil"></i></asp:LinkButton>
                                            </ItemTemplate>
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Excluir">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete" ClientIDMode="Static"><i class="fa fa-eraser"></i></asp:LinkButton>
                                            </ItemTemplate>
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>

                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-6">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Opções</b></h3>
                        </div>
                        <div class="panel-body">
                            <div runat="server" id="div_IncluirOpcoes" class="row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_IncluirOpcoes" />

                                    <div class="col-lg-2" runat="server" id="div_OrdemOpcao">
                                        <div class="form-group">
                                            <label>Ordem</label>
                                            <asp:TextBox runat="server" ID="txtOrdemOpcao" class="form-control"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-10" runat="server" id="div_Opcao">
                                        <div class="form-group">
                                            <label>Opção</label>
                                            <asp:TextBox runat="server" ID="txtOpcao" class="form-control"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2" runat="server" id="div_cmdAlterarOpcao">
                                        <div class="form-group">
                                            <label>&nbsp;</label>
                                            <asp:Button runat="server" ID="cmdAlterarOpcao" class="btn btn-info form-control" Text="Alterar" OnClick="cmdAlterarOpcao_Click"></asp:Button>
                                        </div>
                                    </div>

                                    <div class="col-lg-2" runat="server" id="div_cmdIncluirOpcao">
                                        <div class="form-group">
                                            <label>&nbsp;</label>
                                            <asp:Button runat="server" ID="cmdIncluirOpcao" class="btn btn-info form-control" Text="Incluir" OnClick="cmdIncluirOpcao_Click"></asp:Button>
                                        </div>
                                    </div>

                                </div>
                            </div>

                            <uc1:MensagemPagina runat="server" ID="MensagemPagina_gvOpcao" />

                            <div runat="server" id="div_gvOpcao" class="col-lg-12">

                                <asp:GridView ID="gvOpcao" class="table table-striped table-bordered table-hover "
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" DataKeyNames="Key" OnRowDataBound="gvOpcao_RowDataBound" OnRowDeleting="gvOpcao_RowDeleting" OnRowCommand="gvOpcao_RowCommand">
                                    <Columns>

                                        <asp:TemplateField HeaderText="Ordem">
                                            <ItemTemplate>
                                                <asp:Label ID="lblidOpcao" runat="server" Text='<%# Eval("Value") %>' ClientIDMode="Static"></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Descrição da Opção">
                                            <ItemTemplate>
                                                <asp:Label ID="lblsDscOpcao" runat="server" Text='<%#Eval("Value") %>' ClientIDMode="Static"></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="85%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Editar">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnEditar" runat="server" CssClass="btn btn-small" TabIndex="99" CommandName="Editar" CommandArgument='<%# Eval("Key") %>' ClientIDMode="Static"><i class="fa fa-pencil"></i></asp:LinkButton>
                                            </ItemTemplate>
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Excluir">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete" ClientIDMode="Static"><i class="fa fa-eraser"></i></asp:LinkButton>
                                            </ItemTemplate>
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>

                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />


            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" id="field_cancel" title="Voltar" onclick="history.go(-1)">
            </fieldset>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
