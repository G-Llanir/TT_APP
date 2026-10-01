<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="PlanoSaude.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.PlanoSaude" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Cadastro Plano de Saúde"></asp:Label>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">
            <asp:Panel ID="pnPlanoSaudeConsulta" class="" runat="server">
                <div class="panel panel-primary" id="div_filtroPesquisa" runat="server">
                    <div class="panel-heading">
                        <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;Filtro para pesquisa </h3>

                    </div>
                    <div class="panel-body ">
                        <div class="form-group">

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:TextBox ID="txtPesquisa" class="form-control " placeholder="Pesquisar Plano" runat="server"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlsTipoPesquisa" class="form-control" runat="server">
                                        <asp:ListItem Text="Todos os Planos" Value=""></asp:ListItem>
                                        <asp:ListItem Text="Plano de Saúde" Value="PS"></asp:ListItem>
                                        <asp:ListItem Text="Plano Odontológico" Value="PO"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" />
                                    <asp:Button ID="btnNovoPlano" class="btn btn-success" runat="server" Text="Novo Plano Saúde" OnClick="btnNovoPlano_Click" />
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
                                    <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover " DataKeyNames="idPlanoSaude"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                        <Columns>

                                            <asp:BoundField DataField="idPlanoSaude" HeaderText="ID">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Tipo">
                                                <HeaderStyle Width="20%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbPlanoSaudeDetalhe_Tipo" runat="server"
                                                        Text='<%# Eval("sTipo") %>'
                                                        CommandArgument='<%# Eval("idPlanoSaude") %>'
                                                        OnCommand="lbPlanoSaudeDetalhe_Plano">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Plano">
                                                <HeaderStyle Width="30%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbPlanoSaudeDetalhe_Plano" runat="server"
                                                        Text='<%# Eval("sDscPlanoSaude") %>'
                                                        CommandArgument='<%# Eval("idPlanoSaude") %>'
                                                        OnCommand="lbPlanoSaudeDetalhe_Plano">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="nCarencia" HeaderText="Carência" DataFormatString="{0} dia(s)">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nValor" HeaderText="Valor Plano" DataFormatString="{0:N2}">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Última Atualização">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
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


            <asp:Panel ID="pnPlanoSaudeDetalhe" runat="server" Style="margin-left: -40px">

                <div class="panel-body">

                    <div class="col-lg-12 form-group">

                        <div class="col-lg-12">
                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaDetalhe" />
                        </div>

                        <div class="form-group">
                            <div class="col-lg-12">
                                <div class="row">
                                    <div class="col-lg-1">
                                        <div class="form-group">
                                            <label>ID</label>
                                            <asp:TextBox ID="txtIdPlanoSaude" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="form-group">
                            <div class="col-lg-12">
                                <div class="row">

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Tipo</label>
                                            <asp:DropDownList ID="ddlsTipoPlanoSaude" class="form-control" runat="server">
                                                <asp:ListItem Text="Selecione o Tipo do Plano" Value=""></asp:ListItem>
                                                <asp:ListItem Text="Plano de Saúde" Value="PS"></asp:ListItem>
                                                <asp:ListItem Text="Plano Odontológico" Value="PO"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>

                        <div class="form-group">
                            <div class="col-lg-12">
                                <div class="row">

                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>Descrição Plano Saúde</label>
                                            <asp:TextBox ID="txtsDscPlanoSaude" class="form-control" runat="server" MaxLength="50"></asp:TextBox>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>


                        <div class="row">
                            <div class="col-lg-12 form-group">

                                <div class="col-lg-1">
                                    <div class="form-group">
                                        <label>Carência</label>
                                        <div class="input-group">
                                            <asp:TextBox ID="txtnCarencia" class="form-control" runat="server" TextMode="Number"></asp:TextBox>
                                            <span class="input-group-addon">Dias</span>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Valor</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">R$</span>
                                            <asp:TextBox ID="txtnValorPlano" class="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>

                        <div class="col-lg-12" style="margin-bottom: 15px; padding: 0px">
                            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" Visible="false" />
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
                </div>

            </asp:Panel>

        </div>

        <asp:HiddenField ID="hddidPlanoSaude" Value="" runat="server" />

    </div>

</asp:Content>
