<%@ Page Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Seguro.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.Seguro" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .tag-list {
            width: 100%;
            flex-wrap: wrap;
            display: flex;
            overflow: hidden;
        }

        .tag-item {
            background: lightgray;
            padding: 2px 4px;
            font-size: 13px;
            text-transform: uppercase;
            margin: 2px;
            display: inline-flex;
            flex-wrap: nowrap;
            border-radius: 5px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Cadastro de Seguros"></asp:Label>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">
            <asp:Panel ID="pnSeguroConsulta" class="" runat="server">
                <div class="panel panel-primary" id="div_filtroPesquisa" runat="server">
                    <div class="panel-heading">
                        <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;Filtro para pesquisa </h3>

                    </div>
                    <div class="panel-body ">
                        <div class="form-group">

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:TextBox ID="txtPesquisa" class="form-control " placeholder="Pesquisar Seguro" runat="server"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" />
                                    <asp:Button ID="btnNovoSeguro" class="btn btn-success" runat="server" Text="Novo Seguro" OnClick="btnNovoSeguro_Click" />
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
                                    <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover " DataKeyNames="idSeguro"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                        <Columns>

                                            <asp:BoundField DataField="idSeguro" HeaderText="ID">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Seguradora">
                                                <HeaderStyle Width="20%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbSeguroDetalhe_Parceiro" runat="server"
                                                        Text='<%# Eval("sRazaoSocial") %>'
                                                        CommandArgument='<%# Eval("idSeguro") %>'
                                                        OnCommand="lbSeguroDetalhe_Command">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="CNPJ">
                                                <HeaderStyle Width="30%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbSeguroDetalhe_CNPJ" runat="server"
                                                        Text='<%# Eval("sCNPJ") %>'
                                                        CommandArgument='<%# Eval("idSeguro") %>'
                                                        OnCommand="lbSeguroDetalhe_Command">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Planos">
                                                <HeaderStyle Width="15%" />
                                                <ItemTemplate>
                                                    <asp:Literal ID="litPlanos" runat="server" Text='<%# Eval("sPlano") %>' Mode="PassThrough" />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Usuário Atualização">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Atualização">
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


            <asp:Panel ID="pnSeguroDetalhe" runat="server" Style="margin-left: -40px">

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
                                            <asp:TextBox ID="txtIdSeguro" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
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
                                            <label>Seguradora</label>
                                            <asp:DropDownList ID="ddlsParceiroSeguro" class="form-control Caixa_Selecao" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlsSeguroParceiro_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>

                        <div class="form-group" id="div_detalheParceiroSeguro">

                            <div id="div_dadosSeguro" runat="server">
                                <div class="row">

                                    <div class="col-lg-12">
                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>CNPJ</label>
                                                <asp:TextBox ID="txtsCNPJ" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label>Endereço</label>
                                                <asp:TextBox ID="txtsEndereco" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-12">
                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>Nº Apólice V.G.</label>
                                                <asp:TextBox ID="txtnApoliceVG" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>Nº Apólice APC</label>
                                                <asp:TextBox ID="txtnApoliceAPC" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>Telefone de Apoio</label>
                                                <asp:TextBox ID="txtsTelefoneApoio" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </div>

                            <div class="panel panel-primary" runat="server" id="div_IncluirPlanosSeguro" style="display: flow-root; margin-left: 15px;">
                                <div class="panel-heading" style="display: flex;">
                                    <h3 class="panel-title">Planos</h3>
                                </div>

                                <div class="panel-body">

                                    <div class="form-group">
                                        <div class="col-lg-12">
                                            <div class="row">

                                                <div class="col-lg-3">
                                                    <div class="form-group">
                                                        <label>Tipo Plano</label>
                                                        <asp:TextBox ID="txtsTipoPlano" class="form-control" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2">
                                                    <div class="form-group">
                                                        <label>Valor Natural</label>
                                                        <asp:TextBox ID="txtnValorNatural" class="form-control" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2">
                                                    <div class="form-group">
                                                        <label>Valor Acidental</label>
                                                        <asp:TextBox ID="txtnValorAcidental" class="form-control" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2">
                                                    <div class="form-group">
                                                        <label>Valor Invalidez</label>
                                                        <asp:TextBox ID="txtnValorInvalidez" class="form-control" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>


                                                <div class="col-lg-1">
                                                    <div class="form-group">
                                                        <label>V.G.</label>
                                                        <asp:TextBox runat="server" ID="txtnVGSeguro" class="form-control"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-1">
                                                    <div class="form-group">
                                                        <label>APC</label>
                                                        <asp:TextBox runat="server" ID="txtnAPCSeguro" class="form-control"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-1">
                                                    <div class="form-group">
                                                        <asp:Button ID="btnIncluirPlano" runat="server" class="btn btn-primary" Text="Incluir" OnClick="btnIncluirPlano_Click" Style="margin-top: 24px;" />
                                                    </div>
                                                </div>

                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-12 form-group table-responsive" style="margin-top: 20px;">
                                        <asp:GridView ID="gvPlanoSeguro" class="table table-striped table-bordered table-hover " DataKeyNames="idPlanoSeguro, sTemRelatorio"
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDeleting="gvPlanoSeguro_RowDeleting" OnRowCommand="gvPlanoSeguro_RowCommand" OnRowDataBound="gvPlanoSeguro_RowDataBound">

                                            <Columns>

                                                <asp:BoundField DataField="idPlanoSeguro" HeaderText="ID">
                                                    <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sTipoPlano" HeaderText="Tipo Plano">
                                                    <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="nValorNatural" HeaderText="Valor Natural" DataFormatString="{0:N2}">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="nValorAcidental" HeaderText="Valor Acidental" DataFormatString="{0:N2}">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="nValorInvalidez" HeaderText="Valor Invalidez" DataFormatString="{0:N2}">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="nVG" HeaderText="V.G." DataFormatString="{0:N5}">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="nAPC" HeaderText="APC" DataFormatString="{0:N5}">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>                                              
                                               
                                                <asp:TemplateField HeaderText="">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkEditar" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Editar" CommandArgument='<%# Eval("idPlanoSeguro") %>' ><i class="fa fa-pencil"></i></asp:LinkButton>
                                                        <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>
                                    </div>

                                </div>
                            </div>
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

        <asp:HiddenField ID="hddidSeguro" Value="" runat="server" />
        <asp:HiddenField ID="hddidPlanoSeguro" Value="0" runat="server" />

    </div>
</asp:Content>
