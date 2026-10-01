<%@ Page Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="CodigoServico.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Fiscal.CodigoServico" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">


    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Serviços Federal"></asp:Label>
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
                                    <asp:DropDownList runat="server" ID="ddlCodigoServico" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:TextBox ID="txtPesquisa" class="form-control " placeholder="Pesquisar Serviço Filho" runat="server"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" />
                                    <asp:Button ID="btnNovoCodServico" class="btn btn-success" runat="server" Text="Novo Código Serviço" OnClick="btnNovoCodServico_Click" />
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
                                    <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover " DataKeyNames="idCodigoServico"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">


                                        <Columns>

                                            <asp:BoundField DataField="idServico" HeaderText="Código Serviço">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Descrição Serviço Pai">
                                                <HeaderStyle Width="20%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnServicoPai" runat="server"
                                                        Text='<%# Eval("sDscServicoPai") %>'
                                                        CommandArgument='<%# Eval("idCodigoServico") %>'
                                                        OnCommand="lbDetalheServico_Command">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Descrição Serviço Filho">
                                                <HeaderStyle Width="20%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnServicoFilho" runat="server"
                                                        Text='<%# Eval("sDscServicoFilho") %>'
                                                        CommandArgument='<%# Eval("idCodigoServico") %>'
                                                        OnCommand="lbDetalheServico_Command">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="nIRRF" HeaderText="IRRF">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nINSS" HeaderText="INSS">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nISS" HeaderText="ISS">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nPIS" HeaderText="PIS">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nCOFINS" HeaderText="COFINS">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nCSSL" HeaderText="CSSL">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

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
                                                    <asp:TextBox ID="txtIdCodigoServico" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
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
                                                    <label>Código Serviço Pai</label>
                                                    <asp:DropDownList ID="ddlsCodigoServicoPai" class="form-control Caixa_Selecao" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlsCodigoServicoPai_SelectedIndexChanged"></asp:DropDownList>
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
                                                    <label>Código Serviço Filho</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon" id="sCodigoServicoPai" runat="server"></span>
                                                        <asp:TextBox ID="txtnCodigoServicoFilho" class="form-control" runat="server" Style="z-index: 0;" aria-describedby="sCodigoServicoPai" placeholder="00" ></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>Descrição Serviço Filho</label>
                                                    <asp:TextBox ID="txtsDscServicoFilho" class="form-control" runat="server"></asp:TextBox>
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
                                                    <label>IRRF</label>
                                                    <asp:TextBox ID="txtnIRRF" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>INSS</label>
                                                    <asp:TextBox ID="txtnINSS" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>ISS</label>
                                                    <asp:TextBox ID="txtnISS" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>PIS</label>
                                                    <asp:TextBox ID="txtnPIS" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>COFINS</label>
                                                    <asp:TextBox ID="txtnCOFINS" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>CSSL</label>
                                                    <asp:TextBox ID="txtnCSSL" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

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
