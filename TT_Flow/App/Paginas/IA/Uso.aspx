<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Uso.aspx.cs" Inherits="TT_Flow.App.Paginas.IA.Uso" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <style>
        /* Unica regra propria: o Morris precisa de altura no container para desenhar.
           Mesmo padrao do Dashboard.aspx, que tambem declara o tamanho dos divs de grafico. */
        #GraficoPerguntas, #GraficoTokens {
            width: 100%;
            height: 240px;
        }
    </style>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="IA - Uso"></asp:Label><small> Dashboards</small>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="IA - Uso" />
        </div>

        <div class="col-lg-12">
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>

        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart"></i> Uso do assistente IA</h3>
                </div>
                <div class="panel-body">
                    <div class="row d-flex fw-w">
                        <div class="col-lg-2 col-md-3 col-sm-6">
                            <div class="form-group">
                                <label>Período</label>
                                <asp:DropDownList ID="ddlPeriodo" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="7">Últimos 7 dias</asp:ListItem>
                                    <asp:ListItem Value="30" Selected="True">Últimos 30 dias</asp:ListItem>
                                    <asp:ListItem Value="90">Últimos 90 dias</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-lg-2 col-md-3 col-sm-6">
                            <div class="form-group">
                                <label>&nbsp;</label>
                                <div>
                                    <asp:Button ID="cmdAtualizar" runat="server" Text="Atualizar" CssClass="btn btn-primary" OnClick="cmdAtualizar_Click" OnClientClick="MudarBotao_Pesquisa(this)" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <%-- Panel + panel-body: bootstrap puro. O card do Dashboard nao serve aqui - o
                         .announcement-heading dele e 50px, feito para 2 tiles largos, nao para 6 KPIs. --%>
                    <div class="row">
                        <div class="col-lg-2 col-md-4 col-sm-6">
                            <div class="panel panel-primary">
                                <div class="panel-heading"><i class="fa fa-comments"></i> Conversas</div>
                                <div class="panel-body text-center">
                                    <h3 class="m-0"><asp:Label ID="lblCardConversas" runat="server" Text="-" /></h3>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-2 col-md-4 col-sm-6">
                            <div class="panel panel-primary">
                                <div class="panel-heading"><i class="fa fa-question"></i> Perguntas</div>
                                <div class="panel-body text-center">
                                    <h3 class="m-0"><asp:Label ID="lblCardPerguntas" runat="server" Text="-" /></h3>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-2 col-md-4 col-sm-6">
                            <div class="panel panel-primary">
                                <div class="panel-heading"><i class="fa fa-reply"></i> Respostas</div>
                                <div class="panel-body text-center">
                                    <h3 class="m-0"><asp:Label ID="lblCardRespostas" runat="server" Text="-" /></h3>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-2 col-md-4 col-sm-6">
                            <div class="panel panel-primary">
                                <div class="panel-heading"><i class="fa fa-sign-in"></i> Tokens entrada</div>
                                <div class="panel-body text-center">
                                    <h3 class="m-0"><asp:Label ID="lblCardTokensEntrada" runat="server" Text="-" /></h3>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-2 col-md-4 col-sm-6">
                            <div class="panel panel-primary">
                                <div class="panel-heading"><i class="fa fa-sign-out"></i> Tokens saída</div>
                                <div class="panel-body text-center">
                                    <h3 class="m-0"><asp:Label ID="lblCardTokensSaida" runat="server" Text="-" /></h3>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-2 col-md-4 col-sm-6">
                            <div class="panel panel-primary">
                                <div class="panel-heading"><i class="fa fa-money"></i> Custo est. (US$)</div>
                                <div class="panel-body text-center">
                                    <h3 class="m-0"><asp:Label ID="lblCardCusto" runat="server" Text="-" /></h3>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-6 col-md-6 col-sm-12">
                            <h5><strong>Perguntas por dia</strong></h5>
                            <asp:Literal ID="ltrResumoPerguntas" runat="server" />
                            <div id="GraficoPerguntas"></div>
                        </div>
                        <div class="col-lg-6 col-md-6 col-sm-12">
                            <h5><strong>Tokens de saída por dia</strong></h5>
                            <asp:Literal ID="ltrResumoTokens" runat="server" />
                            <div id="GraficoTokens"></div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-6 col-md-6 col-sm-12">
                            <h5><strong>Top usuários</strong></h5>
                            <div class="table-responsive">
                                <asp:GridView ID="dtgvUsoUsuarios" CssClass="table table-striped table-bordered table-condensed"
                                    runat="server" Width="100%" AutoGenerateColumns="True" GridLines="None" />
                            </div>
                        </div>
                        <div class="col-lg-6 col-md-6 col-sm-12">
                            <h5><strong>Por modelo</strong></h5>
                            <div class="table-responsive">
                                <asp:GridView ID="dtgvUsoModelos" CssClass="table table-striped table-bordered table-condensed"
                                    runat="server" Width="100%" AutoGenerateColumns="True" GridLines="None" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-lg-12">
            <asp:Panel ID="pnPrecos" runat="server">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h3 class="panel-title"><i class="fa fa-money"></i> Preços (US$) para estimativa de custo
                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Valores em dólares por 1 milhão de tokens, conforme a tabela oficial do provedor. O custo soma cada modelo pelo seu próprio preço; sem preço específico, usa o padrão."></i>
                        </h3>
                    </div>
                    <div class="panel-body">
                        <h5><strong>Preços por modelo</strong> <small>US$ por 1 milhão de tokens</small></h5>
                        <div class="table-responsive">
                            <asp:GridView ID="dtgvPrecosModelos" runat="server" CssClass="table table-striped table-bordered table-condensed" Width="100%"
                                AutoGenerateColumns="False" GridLines="None" DataKeyNames="Modelo" OnRowCommand="dtgvPrecosModelos_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField="Modelo" HeaderText="Modelo" />
                                    <asp:BoundField DataField="Entrada" HeaderText="Entrada (US$/1M)" />
                                    <asp:BoundField DataField="Saida" HeaderText="Saída (US$/1M)" />
                                    <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" CssClass="btn btn-danger btn-xs" ToolTip="Remover preço"
                                                CommandName="RemoverPreco" CommandArgument='<%# Eval("Modelo") %>'><i class="fa fa-eraser"></i></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                        <asp:Label ID="lblPrecosVazio" runat="server" CssClass="help-block" Text="Nenhum preço por modelo cadastrado ainda. Cadastre abaixo os modelos que você usa." Visible="false" />

                        <div class="row d-flex fw-w">
                            <div class="col-lg-4 col-md-6 col-sm-12">
                                <div class="form-group">
                                    <label>Modelo</label>
                                    <asp:DropDownList ID="ddlModeloPreco" CssClass="form-control" runat="server" />
                                </div>
                            </div>
                            <div class="col-lg-2 col-md-3 col-sm-6">
                                <div class="form-group">
                                    <label>Entrada (US$/1M)</label>
                                    <asp:TextBox ID="txtPrecoModeloEntrada" CssClass="form-control" runat="server" MaxLength="12" />
                                </div>
                            </div>
                            <div class="col-lg-2 col-md-3 col-sm-6">
                                <div class="form-group">
                                    <label>Saída (US$/1M)</label>
                                    <asp:TextBox ID="txtPrecoModeloSaida" CssClass="form-control" runat="server" MaxLength="12" />
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-6 col-sm-12">
                                <div class="form-group">
                                    <label>&nbsp;</label>
                                    <div>
                                        <asp:Button ID="cmdSalvarPrecoModelo" runat="server" Text="Salvar preço do modelo" CssClass="btn btn-success" OnClick="cmdSalvarPrecoModelo_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <hr />

                        <h5><strong>Preço padrão (fallback)</strong> <small>usado quando o modelo não tem preço específico</small></h5>
                        <div class="row d-flex fw-w">
                            <div class="col-lg-2 col-md-3 col-sm-6">
                                <div class="form-group">
                                    <label>Entrada (US$/1M)</label>
                                    <asp:TextBox ID="txtPrecoEntrada" CssClass="form-control" runat="server" MaxLength="12" />
                                </div>
                            </div>
                            <div class="col-lg-2 col-md-3 col-sm-6">
                                <div class="form-group">
                                    <label>Saída (US$/1M)</label>
                                    <asp:TextBox ID="txtPrecoSaida" CssClass="form-control" runat="server" MaxLength="12" />
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-6 col-sm-12">
                                <div class="form-group">
                                    <label>&nbsp;</label>
                                    <div>
                                        <asp:Button ID="cmdSalvarPrecos" runat="server" Text="Salvar preço padrão" CssClass="btn btn-success" OnClick="cmdSalvarPrecos_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <span class="help-block">Sem nenhum preço configurado, o custo estimado não é calculado.</span>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
