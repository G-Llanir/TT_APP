<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Colaboradores.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.Colaboradores" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        .label {
            font-size: 15px;
            font-weight: bold;
        }

        .card {
            box-shadow: 0px 2px 8px 2px rgba(0,0,0,0.3);
            transition: 0.3s;
            width: 100%;
            border-radius: 5px;
        }

        .card-body {
            padding: 10px;
            display: flex;
            align-items: flex-start;
        }

        .flex-grow-1 {
            flex-grow: 1;
            margin-left: 15px;
        }

        .img-thumbnail {
            border: none;
            width: 100px;
            height: auto;
        }

        .me-3 {
            margin-right: 1rem;
            padding: 2px;
            border-bottom: 1px dashed black;
        }

        .success {
            color: #5cb85c !important;
        }

        .warning {
            color: #f0ad4e !important;
        }

        .danger {
            color: #d9534f !important;
        }

        .default {
            color: #777 !important;
        }
    </style>

    <asp:UpdatePanel runat="server" ID="UpdGeral" UpdateMode="Conditional">
        <ContentTemplate>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Colaboradores"></asp:Label>
                        <small>
                            <asp:Label ID="lblSubTituloPagina" runat="server" Text=" Consulta"></asp:Label></small>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>
                <div class="col-lg-12">
                    <div class="panel panel-primary" runat="server" id="DIV_Filtro">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa</h3>
                        </div>
                        <div class="panel-body ">

                            <div class="row">

                                <div class="col-lg-4 form-group">
                                    <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                </div>

                                <div class="col-lg-3 form-group">
                                    <asp:DropDownList ID="ddlidEmpresa" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlidEmpresa_SelectedIndexChanged" runat="server"></asp:DropDownList>
                                </div>

                                <div class="col-lg-3 form-group">
                                    <asp:DropDownList ID="ddlidDepartamento" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-lg-2 form-group">
                                    <asp:DropDownList ID="ddlsCBO" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                                

                            </div>

                            <div class="row">
                                <div class="col-lg-3 form-group">
                                    <asp:DropDownList ID="ddlidGHE" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>

                                <div class="col-lg-2 form-group">
                                    <asp:DropDownList ID="ddlidCargo" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>

                                <div class="col-lg-2 form-group">
                                    <asp:DropDownList ID="ddlidSupervisorDireto" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>

                                <div class="col-lg-2 form-group">
                                    <asp:DropDownList ID="ddlidTipoContrato" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>

                                <div class="col-lg-2 form-group">
                                    <asp:DropDownList ID="ddlidFuncao" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-lg-1 form-group">
                                    <asp:DropDownList ID="ddlsSituacao" runat="server" class="form-control Caixa_Selecao">
                                        <asp:ListItem Value="T">Todos</asp:ListItem>
                                        <asp:ListItem Value="S" Selected="True">Ativo</asp:ListItem>
                                        <asp:ListItem Value="N">Inativo</asp:ListItem>
                                        <asp:ListItem Value="F">Fora do Quadro</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                              



                            </div>

                            <div class="row">

                                <div class="col-lg-6 form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                    <asp:HyperLink ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" Target="_blank" NavigateUrl="/App/Paginas/RRHH/Colaboradores_Detalhe.aspx?id=0"></asp:HyperLink>
                                    <asp:HyperLink ID="cmdAssociar" class="btn btn-warning" runat="server" Text="Associar Usuário" Target="_blank" NavigateUrl="/App/Paginas/RRHH/Usuario_Colaborador.aspx"></asp:HyperLink>
                                    <asp:Button ID="cmdEPIs" class="btn btn-info" runat="server" Text="Controle de EPIs" OnClick="cmdEPIs_Click" />
                                    <asp:HyperLink ID="lnkExportar" runat="server" CssClass="btn btn-warning" Target="_blank" NavigateUrl="~/App/Paginas/RRHH/Colaboradores.aspx?action=export"><i class="fa fa-file-excel-o"></i> Exportar para Excel</asp:HyperLink>
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
                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">
                                                    <Columns>

                                                        <asp:HyperLinkField DataNavigateUrlFields="idColaborador"
                                                            DataTextField="idColaborador" HeaderText="ID"
                                                            DataNavigateUrlFormatString="Colaboradores_Detalhe.aspx?id={0}">
                                                            <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:HyperLinkField>

                                                        <asp:HyperLinkField DataNavigateUrlFields="idColaborador"
                                                            DataTextField="sDscColaborador" HeaderText="Colaborador"
                                                            DataNavigateUrlFormatString="Colaboradores_Detalhe.aspx?id={0}">
                                                            <ItemStyle Width="18%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:HyperLinkField>


                                                        <asp:HyperLinkField DataNavigateUrlFields="idColaborador"
                                                            DataTextField="sDscEmpresa" HeaderText="Empresa"
                                                            DataNavigateUrlFormatString="Colaboradores_Detalhe.aspx?id={0}">
                                                            <ItemStyle Width="4%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:HyperLinkField>

                                                        <asp:HyperLinkField DataNavigateUrlFields="idColaborador"
                                                            DataTextField="sDscDepartamento" HeaderText="Departamento"
                                                            DataNavigateUrlFormatString="Colaboradores_Detalhe.aspx?id={0}">
                                                            <ItemStyle Width="16%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:HyperLinkField>

														<asp:HyperLinkField DataNavigateUrlFields="idColaborador"
                                                            DataTextField="sDscCargo" HeaderText="Cargo"
                                                            DataNavigateUrlFormatString="Colaboradores_Detalhe.aspx?id={0}">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:HyperLinkField>
														
                                                        <asp:HyperLinkField DataNavigateUrlFields="idColaborador"
                                                            DataTextField="sDscGHE_Completo" HeaderText="GHE/Setor"
                                                            DataNavigateUrlFormatString="Colaboradores_Detalhe.aspx?id={0}">
                                                            <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:HyperLinkField>

														<asp:HyperLinkField DataNavigateUrlFields="idFuncao"
                                                            DataTextField="sDscFuncao" HeaderText="Função"
                                                            DataNavigateUrlFormatString="Funcoes_Detalhe.aspx?id={0}">
                                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:HyperLinkField>
                                                        
														<asp:HyperLinkField DataNavigateUrlFields="idColaborador"
                                                            DataTextField="sDscSupervisorDireto" HeaderText="Supervisor Direto"
                                                            DataNavigateUrlFormatString="Colaboradores_Detalhe.aspx?id={0}">
                                                            <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:HyperLinkField>														
                                                        



                                                        <asp:HyperLinkField DataNavigateUrlFields="idColaborador"
                                                            DataTextField="sCBO" HeaderText="CBO"
                                                            DataNavigateUrlFormatString="Colaboradores_Detalhe.aspx?id={0}">
                                                            <ItemStyle Width="14%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:HyperLinkField>


                                                        <asp:BoundField DataField="dtAniversario" HeaderText="Aniver">
                                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sSituacao_Completa" HeaderText="Ativo">
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

                </div>

            </div>

            <div class="modal fade" id="modalEPI">
                <div class="modal-dialog" style="width: 65%;">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h3 class="modal-title">Controle de EPIs <i class="fa fa-info-circle" style="vertical-align: top; font-size: 16px;" data-toggle="tooltip" title="Apenas Entregas de EPI <u>confirmadas</u> serão utilizadas para validar o Status dos EPIs!"></i></h3>
                        </div>
                        <div class="modal-body">


                            <uc1:MensagemPagina runat="server" ID="MensagemPagina_ModalEPI" />


                            <div class="form-group">
                                <label>Colaborador</label>
                                <asp:DropDownList runat="server" ID="ddlColaboradores" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlColaboradores_SelectedIndexChanged"></asp:DropDownList>
                            </div>

                            <div runat="server" id="div_gvEPIs" class="row">
                                <div class="col-lg-12 form-group">
                                    <label style="width: 100%;">&nbsp;</label>
                                    <span class="label label-success"><i class="fa fa-check-circle"></i>Ok</span>
                                    <span class="label label-warning"><i class="fa fa-exclamation-circle"></i>Vence em 7 Dias</span>
                                    <span class="label label-danger"><i class="fa fa-times-circle"></i>Vencido</span>
                                    <span class="label label-default"><i class="fa fa-question-circle"></i>Não recebido</span>
                                </div>

                                <div class="col-lg-12">
                                    <div class="panel panel-primary" style="margin-bottom: 0;">
                                        <div class="panel-body table-responsive">
                                            <asp:GridView ID="gvEPIs" class="table table-striped table-hover dataTable no-footer"
                                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvEPIs_RowDataBound">
                                                <Columns>

                                                    <asp:HyperLinkField DataNavigateUrlFields="idEPI"
                                                        DataTextField="sCodigoEPI" HeaderText="Código" Target="_blank"
                                                        DataNavigateUrlFormatString="/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                        <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:HyperLinkField>

                                                    <asp:TemplateField HeaderText="Descrição do EPI">
                                                        <ItemTemplate>
                                                            <div>
                                                                <asp:LinkButton ID="lnkProdutoDetalhe" runat="server" Text='<%# Eval("sDscEPI") %>'
                                                                    OnClientClick='<%# "openModal(\"" + Eval("idEPI") + "\"); return false;" %>'
                                                                    OnMouseOver='<%# "mostraCard(this, \"" + Eval("idEPI") + "\", \"produto\");" %>'
                                                                    OnMouseOut='<%# "escondeCard( \"" + Eval("idEPI") + "\", \"produto\");" %>'
                                                                    CssClass="epi-detalhe-link"
                                                                    ClientIDMode="Static"
                                                                    data-idproduto='<%# Eval("idEPI") %>'
                                                                    data-tabela="epi" />

                                                                <div id='<%# Eval("idEPI") + "_produto" %>' class="product-card" style="display: none;">
                                                                    <!--conteudo via script -->
                                                                </div>
                                                            </div>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="40%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:TemplateField>

                                                    <asp:BoundField DataField="nCA" HeaderText="CA">
                                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sStatus" HeaderText="Status">
                                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade">
                                                        <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="dtVencimento" HeaderText="Data do Vencimento">
                                                        <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

                                    <fieldset class="form-stacked actions" style="margin-bottom: 0;">
                                        <asp:HyperLink runat="server" ID="cmdConferencia" class="btn btn-md btn-info" Text="Nova Conferência" Target="_blank" />
                                        <asp:Button runat="server" ID="cmdFechar_Modal" class="btn btn-md btn-warning" Text="Voltar" data-dismiss="modal" />
                                    </fieldset>
                                </div>
                            </div>

                            <div runat="server" id="div_espaco_modal" style="height: 250px;">&nbsp;</div>

                        </div>
                    </div>
                </div>
            </div>


            <div class="modal fade" id="produtoDetalheModal">
                <div class="modal-dialog" style="width: 35%;">
                    <div class="modal-content" style="display: table; width: 100%;">
                        <div class="modal-header" id="modalInfo">
                        </div>
                        <div class="modal-body" id="modalBody">
                            <!-- conteudo via javascript-->
                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="cmdPesquisar" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="cmdEPIs" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ddlColaboradores" EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>
