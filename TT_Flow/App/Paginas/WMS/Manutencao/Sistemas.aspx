<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Sistemas.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.Manutencao.Sistemas" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>
<%@ Register Src="~/App/Controles/FiltroPesquisa.ascx" TagPrefix="uc1" TagName="FiltroPesquisa" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <style>
        .modal-footer {
            text-align: left !important;
        }

        .mini-menu {
            background-color: #009a22;
            border-radius: 5px;
            padding: 10px;
        }

            .mini-menu ul {
                list-style-type: none;
                padding: 0;
                display: flex; /* Exibe os itens horizontalmente */
            }

            .mini-menu li {
                margin-right: 10px; /* Espaçamento entre os itens */
            }

                .mini-menu li:last-child {
                    margin-right: 0; /* Remove o espaçamento do último item */
                }

                .mini-menu li a {
                    color: #ffffff; /* Texto branco para contraste */
                    text-decoration: none;
                    padding: 5px 10px;
                    display: flex; /* Para alinhar o texto e a seta verticalmente */
                    align-items: center; /* Alinha verticalmente */
                    transition: background-color 0.3s, color 0.3s; /* Suaviza a transição */
                }

                    .mini-menu li a:hover {
                        background-color: #005a12; /* Fundo mais escuro no hover */
                        color: #ffffff; /* Mantém o texto branco no hover */
                    }
    </style>

    <script type="text/javascript">
        function MudarBotao() {
            document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
        }
    </script>

    <uc1:Manual runat="server" ID="manual" />

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Sistemas"></asp:Label><small> Consulta</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa </h3>
                </div>
                <div class="panel-body ">
                    <div class="form-group">

                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:TextBox ID="txtPesquisa" class="form-control " placeholder="Pesquisar" runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-2" runat="server" id="DIV_sAtivo">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlsAtivo" runat="server" class="Caixa_Selecao form-control">
                                    <asp:ListItem Value="" Text="Todos os Status" />
                                    <asp:ListItem Selected="True" Value="S" Text="Ativo" />
                                    <asp:ListItem Value="N" Text="Inativo" />
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-6">
                            <div class="form-group">
                                <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo Sistema" OnClick="cmdNovo_Click" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    <asp:Panel ID="pnResultado" class="" runat="server">

                        <div id="resultado">
                            <div class="panel panel-primary">
                                <div class="panel-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                            <Columns>

                                                <asp:TemplateField HeaderText="ID">
                                                    <HeaderStyle Width="10%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnidSistema" runat="server"
                                                            Text='<%# Eval("idSistema") %>'
                                                            CommandArgument='<%# Eval("idSistema") %>'
                                                            OnCommand="LinkButton_Command">
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Nome do Sistema">
                                                    <HeaderStyle Width="20%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnsDscSistema" runat="server"
                                                            Text='<%# Eval("sDscSistema") %>'
                                                            CommandArgument='<%# Eval("idSistema") %>'
                                                            OnCommand="LinkButton_Command">
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Descrição">
                                                    <HeaderStyle Width="40%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnsObservacao" runat="server"
                                                            Text='<%# Eval("sObservacao") %>'
                                                            CommandArgument='<%# Eval("idSistema") %>'
                                                            OnCommand="LinkButton_Command">
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Data">
                                                    <HeaderStyle Width="20%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btndtSistema" runat="server"
                                                            Text='<%# Eval("dtSistema") %>'
                                                            CommandArgument='<%# Eval("idSistema") %>'
                                                            OnCommand="LinkButton_Command">
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Ativo">
                                                    <HeaderStyle Width="10%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnsAtivo" runat="server"
                                                            Text='<%# Eval("sAtivo") %>'
                                                            CommandArgument='<%# Eval("idSistema") %>'
                                                            OnCommand="LinkButton_Command">
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="Modal_Detalhe" tabindex="-1" data-backdrop="static" role="dialog">
        <div class="modal-dialog modal-dialog-centered modal-largo" role="document">
            <div class="modal-content">
                <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                    <ContentTemplate>
                        <div class="modal-header-content">
                            <div class="modal-logo">
                                <asp:Image ID="Image3" runat="server" ImageUrl="~/App/img/instrucaoTecnica_pdf.jpg" AlternateText="Instrução Técnica" />
                            </div>

                            <div class="modal-title-container">
                                <asp:Label runat="server" class="modal-title" ID="Label2" Text="Incluir Sistema" Font-Bold="true"></asp:Label>
                            </div>

                            <button type="button" id="btnFecharCancelamento" class="close" data-dismiss="modal" aria-label="Fechar" style="margin-right: 10px;">
                                <span aria-hidden="true">&times;</span>
                            </button>

                            <br />
                        </div>
                        <div class="panel-body">
                            <div id="DIV_MENU" runat="server">
                                <ul id="tab_Sistemas" class="nav nav-tabs" role="tablist">

                                    <li role="presentation" class="tabpanel active">
                                        <a href="#Sistemas" id="aba_Sistemas" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Sistema</b></a>
                                    </li>

                                    <li role="presentation" runat="server" id="aba_Arquivos">
                                        <a href="#Arquivos" role="tab" id="Arquivos-tab" data-toggle="tab" aria-controls="Arquivos"><b>Arquivos</b></a>
                                    </li>
                                </ul>
                            </div>
                            <br />
                            <div id="tab" class="tab-content">
                                <div role="tabpanel" class="tab-pane fade in active" id="Sistemas" aria-labelledby="Sistemas-tab">
                                    <div class="panel panel-default" runat="server" id="DIV_Detalhes">
                                        <div class="panel-heading">
                                            <h3 class="panel-title"><b>Detalhes</b></h3>
                                        </div>
                                        <div class="panel-body">
                                            <div class="form-stacked row">
                                                <div class="col-lg-12">
                                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina1" />
                                                </div>

                                                <div class="col-lg-12">
                                                    <div class="col-lg-1 row">
                                                        <div class="form-group">
                                                            <label>ID</label>
                                                            <asp:TextBox ID="txtidSistema" class="form-control" ReadOnly="true" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-lg-12">
                                                    <div class="col-lg-6 row">
                                                        <div class="form-group">
                                                            <label>Nome do Sistema</label>
                                                            <asp:TextBox ID="txtsDscSistema" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-12">
                                                        <div class="form-group row">
                                                            <label>Descrição</label>
                                                            <asp:TextBox ID="txtsObservacao" class="form-control" runat="server" MaxLength="255" TextMode="MultiLine" Height="95px"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-12 row" runat="server" id="DIV_Ativo">
                                                        <uc1:SwitchAtivo runat="server" ID="sAtivo" />
                                                    </div>
                                                </div>

                                                <div class="col-lg-12 ">
                                                    <div class="panel panel-default" runat="server" id="div_ContasReceber">
                                                        <div class="panel-heading">
                                                            <h3 class="panel-title"><b>Produtos Associados</b></h3>
                                                        </div>
                                                        <div class="panel-body">
                                                            <div class="form-stacked row">

                                                                <div class="col-lg-9">
                                                                    <uc1:FiltroPesquisa runat="server" ID="FiltroPesquisaProdutos" />
                                                                </div>

                                                                <div class="col-lg-2">
                                                                    <br />
                                                                    <label>&nbsp;</label>
                                                                    <asp:Button runat="server" ID="btnIncluir" class="form-control btn btn-info" Text="Incluir Produto" OnClick="btnIncluir_Click"></asp:Button>
                                                                </div>

                                                                <div class="col-lg-12">
                                                                    <asp:GridView ID="gvItens" class="table table-striped table-bordered table-hover "
                                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" DataKeyNames="IdItem" Font-Size="Small" OnRowDeleting="gvItens_RowDeleting"
                                                                        OnRowDataBound="gvItens_RowDataBound">
                                                                        <Columns>
                                                                            <asp:BoundField DataField="nOrdem" HeaderText="ID">
                                                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="IdItem" HeaderText="ID">
                                                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="SCodigo" HeaderText="Código">
                                                                                <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="SDscProduto" HeaderText="Descrição">
                                                                                <ItemStyle Width="55%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                                            </asp:BoundField>

                                                                            <asp:TemplateField>
                                                                                <ItemTemplate>
                                                                                    <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" CssClass="btn btn-small"><i class='fa-eraser fa'></i></asp:LinkButton>
                                                                                </ItemTemplate>
                                                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="5%" />
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </div>

                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>


                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div role="tabpanel" class="tab-pane fade" id="Arquivos" aria-labelledby="Arquivos-tab">
                                    <div class=" embed-responsive embed-responsive-16by9" style="min-height: 800px">
                                        <embed type="text/html" runat="server" id="frmArquivos" width="800" height="500" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div class="modal-footer">
                    <asp:LinkButton ID="btnSalvar" CssClass="btn btn-success" Text="Salvar" runat="server" OnClick="btnSalvar_Click" />
                    <asp:LinkButton CssClass="btn btn-danger" Text="Fechar" runat="server" data-dismiss="modal" />
                </div>
            </div>
        </div>
    </div>

</asp:Content>
