<%@ Page Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="ServicoMunicipal.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Fiscal.ServicoMunicipal" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">


    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Serviços Municipal"></asp:Label>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">
            <asp:Panel ID="pnServicoConsulta" class="" runat="server">
                <div class="panel panel-primary" id="div_filtroPesquisa" runat="server">
                    <div class="panel-heading">
                        <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;Filtro para pesquisa </h3>

                    </div>
                    <div class="panel-body ">
                        <div class="form-group">

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:TextBox ID="txtPesquisa" class="form-control " placeholder="Pesquisar Serviço Filho" runat="server"></asp:TextBox>
                                </div>
                            </div>

                            <div runat="server" id="div_sAtivo" class="col-lg-2">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlsAtivoo" runat="server" class="form-control">
                                        <asp:ListItem Text="Ativos" Value="S" />
                                        <asp:ListItem Text="Inativos" Value="N" />
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" />
                                    <asp:Button ID="btnNovoCodServico" class="btn btn-success" runat="server" Text="Novo Serviço Municipal" OnClick="btnNovoCodServico_Click" />
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
                                    <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover " DataKeyNames="idServico"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">


                                        <Columns>

                                            <asp:BoundField DataField="idServico" HeaderText="ID">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Código Serviço">
                                                <HeaderStyle Width="7%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnServicoPai" runat="server"
                                                        Text='<%# Eval("sCodigo") %>'
                                                        CommandArgument='<%# Eval("idServico") %>'
                                                        OnCommand="lbDetalheServico_Command">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Descrição Serviço">
                                                <HeaderStyle Width="20%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnServicoFilho" runat="server"
                                                        Text='<%# Eval("sDscServico") %>'
                                                        CommandArgument='<%# Eval("idServico") %>'
                                                        OnCommand="lbDetalheServico_Command">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="nISS" HeaderText="ISS">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscServicoFilho" HeaderText="Serviço Federal">
                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Usuário Atualização">
                                                <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Atualização">
                                                <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <div class="row">
                <div class="col-lg-12">
                    <asp:Panel ID="pnServicoDetalhe" class="" runat="server">

                        <div class="panel panel-primary">
                            <div class="panel-body">
                                <uc1:MensagemPagina runat="server" ID="MensagemPaginaDetalhe" />


                                <div class="form-group">
                                    <div class="col-lg-12">
                                        <div class="row">
                                            <div class="col-lg-1">
                                                <div class="form-group">
                                                    <label>ID</label>
                                                    <asp:TextBox ID="txtidServico" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
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
                                                    <label>Código</label>
                                                    <asp:TextBox ID="txtsCodigo" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>Descrição</label>
                                                    <asp:TextBox ID="txtsDscServico" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <div class="col-lg-12">
                                        <div class="row">

                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>Serviço Federal</label>
                                                    <asp:DropDownList ID="ddlidServicoFederal" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
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
                                                    <label>ISS</label>
                                                    <asp:TextBox ID="txtnISS" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="row">
                                        <div class="col-lg-3" id="div_Ativo" runat="server">
                                            <uc1:SwitchAtivo ID="sAtivo" runat="server" />
                                        </div>
                                    </div>
                                </div>

                                <%--<asp:Panel class="" runat="server" ID="pnIncluirServicos">--%>
                                    <div class="panel panel-primary" runat="server" id="div_IncluirServicos" style="display: flow-root; margin-left: 15px;">
                                        <div class="panel-heading" style="display: flex;">
                                            <h3 class="panel-title">Serviços Vinculados</h3>
                                        </div>

                                        <div class="panel-body">

                                            <div class="form-group">
                                                <div class="col-lg-12">
                                                    <div class="row">

                                                        <div class="col-lg-4">
                                                            <div class="form-group">
                                                                <label>Serviço</label>
                                                                <asp:DropDownList runat="server" ID="ddlsServico" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2">
                                                            <div class="form-group">
                                                                <asp:Button ID="btnIncluirServico" runat="server" class="btn btn-primary" Text="Incluir" OnClick="btnIncluirServico_Click" style="margin-top: 24px;"/>
                                                            </div>
                                                        </div>

                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-lg-12 form-group table-responsive" >
                                                <asp:GridView ID="gvServicoVinculado" class="table table-striped table-bordered table-hover " DataKeyNames="idServico"
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDeleting="gvServicoVinculado_RowDeleting">

                                                    <Columns>

                                                        <asp:BoundField DataField="idServico" HeaderText="ID">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sDscServico" HeaderText="Descrição">
                                                            <ItemStyle Width="80%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                    </Columns>
                                                </asp:GridView>
                                            </div>

                                        </div>
                                    </div>

                                <%--</asp:Panel>--%>

                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <asp:Button ID="btnSalvar" class="btn btn-success" runat="server" Text="Salvar" OnClick="btnSalvar_Click" />
                                        <asp:Button ID="btnVoltar" CssClass="btn-warning btn" runat="server" Text="Voltar" OnClick="btnVoltar_Click" />
                                    </div>
                                </div>

                            </div>
                        </div>

                    </asp:Panel>

                </div>
            </div>

            <asp:HiddenField ID="hddidCodigoServico" Value="" runat="server" />
        </div>
    </div>
</asp:Content>
