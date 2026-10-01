<%@ Page Title="" Language="C#" MasterPageFile="~/app/main.master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Projetos.aspx.cs" Inherits="TT_Flow.App.Paginas.Atividades.Projetos" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Calendario.ascx" TagPrefix="uc1" TagName="Calendario" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <style>
                .invisivel {
                    display: none;
                }

                .toggle-panel {
                    cursor: pointer;
                }

                .btn-primary input {
                    margin-right: 2.5px;
                }

                .linkApontamentos a {
                    cursor: pointer;
                }

                .tableClean {
                    border: solid 2px gray;
                    border-radius: 7.5px;
                }

                .tabelaApontamentos thead tr {
                    border-bottom: solid 3px darkgray;
                }

                .tabelaApontamentos tbody tr:last-child {
                    border-top: solid 3px darkgray;
                }

                .sabado, .domingo {
                    background-color: rgb(255, 175, 150) !important;
                }

                .segunda {
                    background-color: rgb(200, 215, 235) !important;
                }

                .terca {
                    background-color: rgb(150, 175, 225) !important;
                }

                .quarta {
                    background-color: rgb(215, 225, 210) !important;
                }

                .quinta {
                    background-color: rgb(190, 210, 175) !important;
                }

                .sexta {
                    background-color: rgb(230, 220, 210) !important;
                }

                .divLegenda {
                    display: flex;
                    justify-content: right;
                    gap: .5rem;
                    font: 2rem bold;
                    font-family: Tahoma;
                }

                    .divLegenda > .label {
                        color: black;
                    }
            </style>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <span runat="server" id="lblTituloPagina"></span>
                        <small>Consulta</small>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>
                <div class="col-lg-12">

                    <div id="painelFiltros" class="panel panel-primary">
                        <div class="panel-heading d-flex space-b">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa</h3>
                            <div id="div_TiposView" class="btn-group checkList">
                                <asp:CheckBox runat="server" ID="cbProjetos" Text="Projetos" class="btn btn-success" data-tipo="1" ValidationGroup="tipos" AutoPostBack="true" OnCheckedChanged="cbTipoVisualizacao_CheckedChanged" />
                                <asp:CheckBox runat="server" ID="cbAtividades" Text="Atividades" class="btn btn-success" data-tipo="2" ValidationGroup="tipos" AutoPostBack="true" OnCheckedChanged="cbTipoVisualizacao_CheckedChanged" />
                                <asp:CheckBox runat="server" ID="cbApontamentos" Text="Apontamentos" class="btn btn-success" data-tipo="4" ValidationGroup="tipos" AutoPostBack="true" OnCheckedChanged="cbTipoVisualizacao_CheckedChanged" />
                                <asp:CheckBox runat="server" ID="cbCalendario" Text="Calendário" class="btn btn-success" data-tipo="3" ValidationGroup="tipos" AutoPostBack="true" OnCheckedChanged="cbTipoVisualizacao_CheckedChanged" />
                                <asp:CheckBox runat="server" ID="cbKanban" Text="Kanban" class="btn btn-success" data-tipo="5" ValidationGroup="tipos" Visible="false" AutoPostBack="true" OnCheckedChanged="cbTipoVisualizacao_CheckedChanged" />
                                <asp:CheckBox runat="server" ID="cbPonto" Text="Ponto" class="btn btn-success" data-tipo="6" ValidationGroup="tipos" AutoPostBack="true" OnCheckedChanged="cbTipoVisualizacao_CheckedChanged" />
                            </div>
                        </div>
                        <div class="panel-body">

                            <div id="filtros" class="row">

                                <div runat="server" id="div_txtPesquisa" class="col-lg-3 form-group">
                                    <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server" MaxLength="200"></asp:TextBox>
                                </div>

                                <div runat="server" id="div_ddlidDepartamento" class="col-lg-5 form-group">
                                    <asp:ListBox ID="ddlidDepartamento" runat="server" class="form-control Caixa_Selecao" SelectionMode="Multiple"></asp:ListBox>
                                </div>

                                <div runat="server" id="div_ddlidUsuario" class="col-lg-4 form-group">
                                    <asp:ListBox ID="ddlidUsuario" runat="server" class="form-control Caixa_Selecao" SelectionMode="Multiple"></asp:ListBox>
                                </div>

                            </div>

                            <div id="filtros2" class="row">

                                <div runat="server" id="div_ddlidEmpresa" class="col-lg-3">
                                    <asp:DropDownList ID="ddlidEmpresa" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>

                                <div runat="server" id="div_ddlidStatus" class="col-lg-3">
                                    <asp:ListBox ID="ddlidStatus" runat="server" class="form-control Caixa_Selecao" SelectionMode="Multiple">
                                        <asp:ListItem Value="1,2,3,6,7" Text="Em Aberto"></asp:ListItem>
                                        <asp:ListItem Value="0" Text="Todos os Status"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="Não Iniciada"></asp:ListItem>
                                        <asp:ListItem Value="2" Text="Em andamento"></asp:ListItem>
                                        <asp:ListItem Value="3" Text="Pausada"></asp:ListItem>
                                        <asp:ListItem Value="4" Text="Excluída"></asp:ListItem>
                                        <asp:ListItem Value="5" Text="Finalizada"></asp:ListItem>
                                        <asp:ListItem Value="6" Text="Impeditivo"></asp:ListItem>
                                        <asp:ListItem Value="7" Text="Pendente"></asp:ListItem>
                                    </asp:ListBox>
                                </div>

                                <div runat="server" id="div_ddlTipoPonto" class="col-lg-3">
                                    <asp:DropDownList ID="ddlTipoPonto" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>

                                <div runat="server" id="div_datas" class="col-lg-3" style="padding: 0;">
                                    <div class="col-lg-6">
                                        <asp:TextBox runat="server" ID="txtData_Inicio" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>
                                    <div class="col-lg-6">
                                        <asp:TextBox runat="server" ID="txtData_Fim" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao_Pesquisar()" />
                                    <asp:HyperLink ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" Target="_blank" NavigateUrl="~/App/Paginas/Atividades/Projetos_Detalhe.aspx?id=0"></asp:HyperLink>
                                </div>

                                <div runat="server" id="divLegenda" class="divLegenda padd-r">
                                    <span class="label label-primary" style="color: white !important;">Departamentos</span>
                                    <span class="label label-success" style="color: white !important;">Administradores</span>
                                    <span class="label label-default" style="color: white !important;">Membros</span>
                                </div>

                            </div>

                        </div>
                    </div>

                </div>
            </div>

            <asp:Panel ID="pnProjetos" runat="server">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <div class="table-responsive">

                            <asp:GridView ID="gvProjetos" class="table table-striped table-bordered table-hover "
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvProjetos_RowDataBound">
                                <Columns>

                                    <asp:HyperLinkField DataNavigateUrlFields="idProjeto"
                                        DataTextField="idProjeto" HeaderText="ID"
                                        DataNavigateUrlFormatString="Projetos_Detalhe.aspx?id={0}">
                                        <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idProjeto" HeaderText="Data Inclusão"
                                        DataTextField="dtInclusao" DataTextFormatString="{0:dd/MM/yyyy HH:mm:ss}"
                                        DataNavigateUrlFormatString="Projetos_Detalhe.aspx?id={0}">
                                        <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idProjeto"
                                        DataTextField="sDscTitulo" HeaderText="Título"
                                        DataNavigateUrlFormatString="Projetos_Detalhe.aspx?id={0}">
                                        <ItemStyle Width="18%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:BoundField DataField="idStatus" HeaderText="Status">
                                        <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:TemplateField HeaderText="Departamentos">
                                        <ItemTemplate>
                                            <div class="tagList">
                                                <asp:Literal Text='<%# Eval("sDeptos") %>' runat="server" />
                                            </div>
                                        </ItemTemplate>
                                        <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Usuários">
                                        <ItemTemplate>
                                            <div class="tagList">
                                                <asp:Literal Text='<%# Eval("sAdms") %>' runat="server" />
                                                <br />
                                                <asp:Literal Text='<%# Eval("sMembros") %>' runat="server" />
                                            </div>
                                        </ItemTemplate>
                                        <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="nHoras_Previsao" HeaderText="Horas Previstas" DataFormatString="{0:N2}">
                                        <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="nHoras_Realizadas" HeaderText="Horas Realizadas" DataFormatString="{0:N2}">
                                        <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="nHoras_Disponiveis" HeaderText="Horas Disponíveis" DataFormatString="{0:N2}">
                                        <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="dtFinal_Previsao" HeaderText="Data Final Prevista" DataFormatString="{0:dd/MM/yyyy HH:mm}">
                                        <ItemStyle Width="9%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="dtAtualizacao" HeaderText="Atualizado em" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}">
                                        <ItemStyle Width="9%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                </Columns>
                            </asp:GridView>

                        </div>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnAtividades" runat="server">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <div class="table-responsive">

                            <asp:GridView ID="gvAtividades" class="table table-striped table-bordered table-hover "
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px">
                                <Columns>

                                    <asp:HyperLinkField DataNavigateUrlFields="idProjeto, idAtividade"
                                        DataTextField="idAtividade" HeaderText="ID"
                                        DataNavigateUrlFormatString="Projetos_Detalhe.aspx?id={0}&atividade={1}">
                                        <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:TemplateField HeaderText="Projeto">
                                        <ItemTemplate>
                                            <%# Eval("sDscProjeto") %>
                                        </ItemTemplate>
                                        <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:TemplateField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idProjeto, idAtividade"
                                        DataTextField="sDscTitulo" HeaderText="Título"
                                        DataNavigateUrlFormatString="Projetos_Detalhe.aspx?id={0}&atividade={1}">
                                        <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idProjeto, idAtividade"
                                        DataTextField="sDscDescricao" HeaderText="Descrição"
                                        DataNavigateUrlFormatString="Projetos_Detalhe.aspx?id={0}&atividade={1}">
                                        <ItemStyle Width="19%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:BoundField DataField="sDscStatus" HeaderText="Status Atual">
                                        <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:TemplateField HeaderText="Usuários Associados">
                                        <ItemTemplate>
                                            <div class="usuarios">
                                                <asp:Literal Text='<%# Eval("sUsuarios") %>' runat="server" />
                                            </div>
                                        </ItemTemplate>
                                        <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="nHoras_Previsao" HeaderText="Horas Previstas" DataFormatString="{0:N2}">
                                        <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="nHoras_Realizadas" HeaderText="Horas Realizadas" DataFormatString="{0:N2}">
                                        <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="nHoras_Disponiveis" HeaderText="Horas Disponíveis" DataFormatString="{0:N2}">
                                        <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="dtInicial_Previsao" HeaderText="Data Inicial Prevista">
                                        <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="dtFinal_Previsao" HeaderText="Data Final Prevista">
                                        <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                </Columns>
                            </asp:GridView>

                        </div>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnApontamentos" runat="server">
                <div class="panel panel-primary">
                    <div class="panel-body">

                        <div class="form-group divLegenda">
                            <span class="label domingo">Fim de Semana</span>
                            <span class="label segunda">Segunda</span>
                            <span class="label terca">Terça</span>
                            <span class="label quarta">Quarta</span>
                            <span class="label quinta">Quinta</span>
                            <span class="label sexta">Sexta</span>
                        </div>

                        <div class="table-responsive">
                            <asp:GridView ID="gvApontamentos" class="table table-striped table-bordered table-hover "
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvApontamentos_RowDataBound">
                                <Columns>

                                    <asp:HyperLinkField DataNavigateUrlFields="idProjeto, idAtividade"
                                        DataTextField="idAtividade" HeaderText="ID"
                                        DataNavigateUrlFormatString="Projetos_Detalhe.aspx?id={0}&atividade={1}">
                                        <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idUsuario"
                                        DataTextField="sDscUsuario" HeaderText="Usuário" Target="_blank"
                                        DataNavigateUrlFormatString="/App/Paginas/Manutencao/Usuarios_Detalhe.aspx?idu={0}">
                                        <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idProjeto, idAtividade"
                                        DataTextField="sDscTitulo" HeaderText="Título"
                                        DataNavigateUrlFormatString="Projetos_Detalhe.aspx?id={0}&atividade={1}">
                                        <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="tituloAtividade" />
                                    </asp:HyperLinkField>

                                    <asp:BoundField DataField="sDscStatus" HeaderText="Status Atual">
                                        <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="nHoras_Previsao" HeaderText="Horas Previstas" DataFormatString="{0:N2}">
                                        <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="nHoras_Realizadas" HeaderText="Horas Realizadas" DataFormatString="{0:N2}">
                                        <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="nHoras_Disponiveis" HeaderText="Horas Disponíveis" DataFormatString="{0:N2}">
                                        <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="dtInicial_Previsao" HeaderText="Data Inicial Prevista">
                                        <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="dtFinal_Previsao" HeaderText="Data Final Prevista">
                                        <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                </Columns>
                            </asp:GridView>
                        </div>

                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnCalendario" runat="server">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <uc1:Calendario runat="server" ID="Calendario" />
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnKanban" runat="server"></asp:Panel>

            <asp:Panel ID="pnPonto" runat="server">
                <div class="panel panel-primary">
                    <div class="panel-body">

                        <div class="table-responsive">
                            <asp:GridView ID="gvPonto" class="table table-striped table-bordered table-hover "
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvPonto_RowDataBound">
                                <Columns>

                                    <asp:BoundField DataField="idPonto" HeaderText="ID">
                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sDscTipoPonto" HeaderText="Status Atual">
                                        <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sDscAlteracao_Ponto" HeaderText="Ponto">
                                        <ItemStyle Width="55%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="dtPonto" HeaderText="Batido em" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}">
                                        <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sDscUsuario" HeaderText="Por">
                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                </Columns>
                            </asp:GridView>
                        </div>

                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnMensagem" runat="server">
                <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
            </asp:Panel>

            <div class="modal fade" id="modalApontamentos">
                <div class="modal-dialog" style="width: 50%;">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h3 class="modal-title">Apontamentos</h3>
                        </div>
                        <div class="modal-body">

                            <label class="atividade_apontamentos"></label>

                            <div class="tabelaApontamentos form-group"></div>

                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>