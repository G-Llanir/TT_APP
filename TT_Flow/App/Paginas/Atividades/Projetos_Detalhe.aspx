<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Projetos_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Atividades.Projetos_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src='<%= ResolveUrl("~/App/JS/Projetos.min.js?v=1.0.00") %>'></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <style>
        /*-------------------------------------------------------------*/
        /* Estilos para manipulações Gerais da Página */

        .invisivel {
            display: none !important;
        }

        .visivel {
            display: block !important;
        }

        .alert {
            display: none;
        }

        /*-------------------------------------------------------------*/
        /* Estilos para Composição de Atividades */

        .tabelaComposicao {
            margin: 0 !important;
        }

            .tabelaComposicao tr td img {
                max-width: 100%;
                height: auto;
            }

        .collapsed-row {
            display: none;
            padding: 1px;
            margin: 1px;
        }

        .cabecalho_Composicao_0 thead tr {
            background-color: rgba(0, 100, 0) !important;
            color: white !important;
        }

        .cabecalho_Composicao_1 thead tr {
            background-color: rgba(0, 140, 0) !important;
            color: white !important;
        }

        .cabecalho_Composicao_2 thead tr {
            background-color: rgba(0, 180, 0) !important;
            color: white !important;
        }

        /*-------------------------------------------------------------*/
        /* Estilos para manipulações do .DataTables */

        .dataTables_scrollBody {
            overflow: visible !important;
        }

        .dataTables_scrollHeadInner {
            max-width: 100%;
        }

        .composicaoLinha {
            display: grid;
        }

        a.composicaoLinha:hover, a.editarAtividade:hover, a.novoApontamento:hover, a.editarApontamento:hover {
            cursor: pointer;
            text-decoration: none;
        }

        /*-------------------------------------------------------------*/
        /* Estilos para o Tooltip Personalizado de Erro */

        .fechar {
            cursor: pointer;
            text-decoration: none;
        }

        .input-container {
            display: inline-block;
            position: relative;
        }

        .tooltip-erro {
            display: none;
            position: absolute;
            bottom: 100%;
            left: 0;
            transform: translateY(-5px);
            background-color: #cc3024;
            color: #fff;
            padding: 6px 10px;
            border-radius: 4px;
            font-size: 12.5px;
            font-weight: bold;
            white-space: normal;
            z-index: 10;
        }

            .tooltip-erro::after {
                content: "";
                position: absolute;
                top: 100%;
                left: 10px;
                margin-left: -5px;
                border-width: 5px;
                border-style: solid;
                border-color: #cc3024 transparent transparent transparent;
            }

        /*-------------------------------------------------------------*/
        /* Estilos para os Apontamentos gerados automaticamente */

        .infoObs {
            text-decoration: none;
            color: blue;
        }

        .infoAlterada {
            text-decoration: underline;
            color: red;
        }

        .infoNova {
            text-decoration: underline;
            color: green;
        }

        ul.infoAlterada {
            text-decoration: none;
        }

            ul.infoAlterada li {
                color: black;
            }

                ul.infoAlterada li::marker {
                    color: red;
                }

        ul.infoNova {
            text-decoration: none;
        }

            ul.infoNova li {
                color: black;
            }

                ul.infoNova li::marker {
                    color: green;
                }

        /*-------------------------------------------------------------*/
        /* Estilos para evitar erros nas Grids */

        .table tbody td {
            word-break: break-word;
        }

        /*-------------------------------------------------------------*/
        /* Estilos para o Histórico de Apontamentos */

        div.tableClean:has(table) {
            border: solid 2px gray;
            border-radius: 7.5px;
        }
    </style>

    <asp:UpdatePanel ID="UpdatePanel" runat="server">
        <ContentTemplate>

            <div id="hdd">
                <asp:HiddenField ID="hddidProjeto" runat="server" Value="0" />
                <asp:HiddenField ID="hddidUsuario" runat="server" Value="0" />
                <asp:HiddenField ID="hddsDscUsuario" runat="server" />
            </div>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>
            </div>

            <div id="abas">
                <ul id="tab_Projetos" class="nav nav-tabs" role="tablist">
                    <li role="presentation" class="tabpanel active">
                        <a href="#projetos" id="aba_Projetos" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Projetos</b></a>
                    </li>
                    <li role="presentation" runat="server" id="aba_Arquivos">
                        <a href="#arquivos" role="tab" id="arquivos-tab" data-toggle="tab" aria-controls="Arquivos"><b>Arquivos</b></a>
                    </li>
                    <li role="presentation" runat="server" id="aba_Historico">
                        <a href="#historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="Historico"><b>Histórico</b></a>
                    </li>
                </ul>
            </div>

            <div id="tab" class="tab-content">

                <div role="tabpanel" class="tab-pane fade in active" id="projetos" aria-labelledby="projetos-tab">
                    <br />
                    <div class="panel panel-default" runat="server" id="div_Projetos">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Projetos</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group row">

                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                                </div>

                                <div id="projeto">

                                    <div id="cadastroInicial" class="col-lg-12">

                                        <div class="row">
                                            <div class="col-lg-1 form-group">
                                                <label>ID</label>
                                                <asp:TextBox runat="server" ID="txtidProjeto" class="form-control" disabled="disabled"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-6 form-group">
                                                <label>Título do Projeto</label>
                                                <asp:TextBox runat="server" ID="txtsTitulo" class="form-control"></asp:TextBox>
                                            </div>

                                            <div class="col-lg-2 form-group">
                                                <label>Horas Previstas</label>
                                                <asp:TextBox runat="server" ID="txtnHoras_Previsao" class="form-control nHoras"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-6 form-group">
                                                <label>Empresa do Projeto</label>
                                                <asp:DropDownList runat="server" ID="ddlEmpresa" class="form-control Caixa_Selecao"></asp:DropDownList>
                                            </div>

                                            <div class="col-lg-6 form-group">
                                                <label>Departamento do Projeto</label>
                                                <asp:ListBox runat="server" ID="ddlDepartamento" SelectionMode="Multiple" class="form-control Caixa_Selecao"></asp:ListBox>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div id="div_lstAdm" class="col-lg-6 form-group">
                                                <label>Criadores / Administradores do Projeto</label>
                                                <asp:ListBox runat="server" ID="ddlAdm" class="form-control Caixa_Selecao usuariosProjeto" SelectionMode="Multiple"></asp:ListBox>
                                            </div>

                                            <div id="div_lstMembros" class="col-lg-6 form-group input-container">
                                                <label>Membros do Projeto</label>
                                                <asp:ListBox runat="server" ID="ddlMembros" class="form-control Caixa_Selecao usuariosProjeto" SelectionMode="Multiple"></asp:ListBox>
                                                <div style="left: 15px;" class="tooltip-erro"></div>
                                            </div>
                                        </div>

                                    </div>

                                    <div class="col-lg-12 form-group">
                                        <label>Descrição do Projeto</label>
                                        <textarea runat="server" id="txtsDescricao" class="htmlEditor"></textarea>
                                    </div>

                                </div>

                                <div runat="server" id="div_Atividades" class="col-lg-12">
                                    <div class="panel panel-default">
                                        <div class="panel-heading">
                                            <h3 class="panel-title"><b>Atividades</b></h3>
                                        </div>
                                        <div class="panel-body">
                                            <div class="form-group row">

                                                <div class="col-lg-12">
                                                    <uc1:MensagemPagina runat="server" ID="Mensagem_IncluirAtividade" />
                                                </div>

                                                <div class="col-lg-3">
                                                    <div class="form-group">
                                                        <asp:Button ID="cmdIncluir_Atividade" class="form-control btn-info incluirAtividade" runat="server" Text="Incluir Atividade" />
                                                    </div>
                                                </div>

                                                <div class="col-lg-12">
                                                    <uc1:MensagemPagina runat="server" ID="Mensagem_gvAtividades" />
                                                </div>

                                                <div runat="server" id="div_gvAtividades" class="col-lg-12">
                                                    <asp:GridView ID="gvAtividades_Master" class="table table-striped table-bordered table-hover table-condensed cabecalho_Composicao_0"
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px">
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

                <div role="tabpanel" class="tab-pane fade" id="arquivos" aria-labelledby="arquivos-tab">
                    <div class=" embed-responsive embed-responsive-16by9" style="min-height: 200px">
                        <embed type="text/html" runat="server" id="frmArquivos" width="800" height="500" />
                    </div>
                </div>

                <div role="tabpanel" class="tab-pane fade" id="historico" aria-labelledby="historico-tab">
                    <br />
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Histórico</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12 table-responsive">
                                    <asp:GridView
                                        ID="gv_Historico" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" OnRowDataBound="gv_Historico_RowDataBound">
                                        <Columns>

                                            <asp:BoundField DataField="idHistorico" HeaderText="ID">
                                                <HeaderStyle CssClass="invisivel" />
                                                <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="invisivel" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscAlteracao" HeaderText="Alteração">
                                                <ItemStyle Width="80%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscUsuarioAlteracao" HeaderText="Alterado Por">
                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtAlteracao" HeaderText="Em">
                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                        </Columns>
                                    </asp:GridView>

                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>

            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" TabIndex="1000" />
                <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" id="field-cancel" title="Voltar" onclick="history.go(-1)">
            </fieldset>

            <div id="dialog-Salvar" class="modal" title="Salvar">
                <p>
                    <label>Deseja Salvar o Projeto?</label>
                </p>
            </div>

            <div class="modal fade" id="modalIncluirAtividade" data-backdrop="static">
                <div class="modal-dialog" style="width: 65%;">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h3 class="modal-title">Incluir Atividade</h3>
                        </div>
                        <div class="modal-body">

                            <uc1:MensagemPagina runat="server" ID="Mensagem_Modal_IncluirAtividade" />

                            <div class="row div_atividadePai_IncluirAtividade">

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Atividade Pai</label>
                                        <asp:DropDownList ID="ddlAtividadePai_IncluirAtividade" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                    </div>
                                </div>

                            </div>

                            <div class="row">

                                <div class="col-lg-8">
                                    <div class="form-group">
                                        <label>Título</label>
                                        <asp:TextBox ID="txtTitulo_IncluirAtividade" class="form-control" runat="server" MaxLength="400"></asp:TextBox>
                                    </div>
                                </div>

                            </div>

                            <div class="div_usuarios_IncluirAtividade">
                                <div class="row">

                                    <div class="col-lg-12">
                                        <div class="form-group">
                                            <label>Usuários Associados</label>
                                            <asp:ListBox ID="lstUsuarios_IncluirAtividade" class="form-control Caixa_Selecao" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                        </div>
                                    </div>

                                </div>
                            </div>

                            <div class="row">

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Descrição</label>
                                        <asp:TextBox ID="txtDescricao_IncluirAtividade" class="form-control" runat="server" TextMode="MultiLine" Height="150px"></asp:TextBox>
                                    </div>
                                </div>

                            </div>

                            <div id="div_DataHora_Previstas_IncluirAtividade" class="row">

                                <div class="col-lg-2">
                                    <div class="form-group input-container">
                                        <label>Horas Previstas</label>
                                        <asp:TextBox ID="txtHoras_IncluirAtividade" class="form-control nHoras horasPrevisao" runat="server" MaxLength="6"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group input-container">
                                        <label>Data Inicial Prevista</label>
                                        <asp:TextBox ID="txtdtInicial_IncluirAtividade" class="form-control dtInicial" TextMode="DateTimeLocal" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group input-container">
                                        <label>Data Final Prevista</label>
                                        <asp:TextBox ID="txtdtFinal_IncluirAtividade" class="form-control dtFinal" TextMode="DateTimeLocal" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                            </div>

                            <div class="row modal-footer">
                                <asp:Button ID="cmdIncluirAtividade" class="btn btn-success" runat="server" Text="Incluir" />
                                <button id="cmdCancelar_IncluirAtividade" type="button" class="btn btn-warning" data-dismiss="modal">Cancelar</button>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <div class="modal fade" id="modalEditarAtividade" data-backdrop="static">
                <div class="modal-dialog" style="width: 65%;">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h3 class="modal-title">Editar Atividade</h3>
                        </div>
                        <div class="modal-body">

                            <uc1:MensagemPagina runat="server" ID="Mensagem_Modal_EditarAtividade" />

                            <div class="row" id="div_atividadePai_EditarAtividade">

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Atividade Pai</label>
                                        <asp:TextBox ID="txtAtividadePai_EditarAtividade" class="form-control" runat="server" MaxLength="400" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>

                            </div>

                            <div class="row">

                                <div class="col-lg-8">
                                    <div class="form-group">
                                        <label>Título</label>
                                        <asp:TextBox ID="txtTitulo_EditarAtividade" class="form-control" runat="server" MaxLength="400"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-4" id="div_Status_EditarAtividade">
                                    <div class="form-group">
                                        <label>Status</label>
                                        <asp:TextBox ID="txtStatus_EditarAtividade" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>

                            </div>

                            <div class="div_usuarios_EditarAtividade">
                                <div class="row">

                                    <div class="col-lg-12">
                                        <div class="form-group">
                                            <label>Usuários Associados</label>
                                            <asp:ListBox ID="lstUsuarios_EditarAtividade" class="form-control Caixa_Selecao" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                        </div>
                                    </div>

                                </div>
                            </div>

                            <div class="row">

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Descrição</label>
                                        <asp:TextBox ID="txtDescricao_EditarAtividade" class="form-control" runat="server" TextMode="MultiLine" Height="150px"></asp:TextBox>
                                    </div>
                                </div>

                            </div>

                            <div class="row">

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Data Inicial</label>
                                        <asp:TextBox ID="txtdtInicial_EditarAtividade" class="form-control" TextMode="DateTimeLocal" runat="server" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Data Final</label>
                                        <asp:TextBox ID="txtdtFinal_EditarAtividade" class="form-control" TextMode="DateTimeLocal" runat="server" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>

                            </div>

                            <div class="row">

                                <div class="col-lg-3">
                                    <div class="form-group input-container">
                                        <label>Horas Previstas</label>
                                        <asp:TextBox ID="txtHoras_EditarAtividade" class="form-control nHoras horasPrevisao" runat="server" MaxLength="6"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group input-container">
                                        <label>Data Inicial Prevista</label>
                                        <asp:TextBox ID="txtdtInicial_Prevista_EditarAtividade" class="form-control dtInicial" TextMode="DateTimeLocal" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group input-container">
                                        <label>Data Final Prevista</label>
                                        <asp:TextBox ID="txtdtFinal_Prevista_EditarAtividade" class="form-control dtFinal" TextMode="DateTimeLocal" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                            </div>

                            <div class="row modal-footer">
                                <asp:Button ID="cmdEditarAtividade" class="btn btn-success" runat="server" Text="Salvar Alterações" />
                                <div class="btn-group dropup">
                                    <button id="cmdAlterarStatus" runat="server" type="button" class="btn btn-primary dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Ação  <span class="caret"></span></button>
                                    <ul class="dropdown-menu dropdown-menu-right">
                                        <li>
                                            <asp:LinkButton ID="cmdIniciarAtividade" class="btn btn-success" runat="server" Text="Iniciar Atividade" data-status="2"></asp:LinkButton>
                                        </li>
                                        <li>
                                            <asp:LinkButton ID="cmdPausarAtividade" class="btn btn-warning" runat="server" Text="Pausar Atividade" data-status="3"></asp:LinkButton>
                                        </li>
                                        <li>
                                            <asp:LinkButton ID="cmdExcluirAtividade" class="btn btn-danger" runat="server" Text="Excluir Atividade" data-status="4"></asp:LinkButton>
                                        </li>
                                        <li>
                                            <asp:LinkButton ID="cmdFinalizarAtividade" class="btn btn-success" runat="server" Text="Finalizar Atividade" data-status="5"></asp:LinkButton>
                                        </li>
                                        <li>
                                            <asp:LinkButton ID="cmdAtividadeImpedida" class="btn btn-danger" runat="server" Text="Pausar Atividade por Impeditivo" data-status="6"></asp:LinkButton>
                                        </li>
                                    </ul>
                                </div>
                                <button id="cmdCancelar_EditarAtividade" type="button" class="btn btn-warning" data-dismiss="modal">Cancelar</button>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <div class="modal fade" id="modalApontamentos" data-backdrop="static">
                <div class="modal-dialog" style="width: 60%;">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h3 class="modal-title">Novo Apontamento de Horas</h3>
                        </div>
                        <div class="modal-body">

                            <uc1:MensagemPagina runat="server" ID="Mensagem_Modal_Apontamento" />

                            <div id="abas_Apontamentos">
                                <ul id="tab_Apontamentos" class="nav nav-tabs" role="tablist">
                                    <li role="presentation" class="tabpanel active">
                                        <a href="#apontamento" id="aba_Apontamento" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Apontamento</b></a>
                                    </li>
                                    <li role="presentation" runat="server" id="aba_HistoricoApontamento">
                                        <a href="#historicoApontamento" role="tab" id="historicoApontamento-tab" data-toggle="tab" aria-controls="Historico"><b>Histórico</b></a>
                                    </li>
                                </ul>
                            </div>

                            <div id="tab_A" class="tab-content">

                                <div role="tabpanel" class="tab-pane fade in active" id="apontamento" aria-labelledby="apontamento-tab">
                                    <br />

                                    <div class="row">
                                        <div class="col-lg-12 form-group">
                                            <label>Apontamento de Horas para a Atividade:</label>
                                            <asp:TextBox runat="server" ID="txtTituloAtividade_Apontamento" class="form-control" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-lg-4 form-group">
                                            <label>Usuário Associado</label>
                                            <asp:DropDownList runat="server" ID="ddlUsuario_Apontamento" class="form-control Caixa_Selecao"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-lg-4 form-group">
                                            <label>Horas Realizadas</label>
                                            <asp:TextBox runat="server" ID="txtHoras_Apontamento" class="form-control nHoras"></asp:TextBox>
                                        </div>

                                        <div id="divData_Apontamento" class="col-lg-4 form-group">
                                            <label>Data do Apontamento</label>
                                            <asp:TextBox runat="server" ID="txtData_Apontamento" class="form-control" TextMode="DateTimeLocal"></asp:TextBox>
                                        </div>

                                        <div id="divMotivo_Apontamento" class="col-lg-12 form-group">
                                            <label>Motivo da Alteração</label>
                                            <asp:TextBox runat="server" ID="txtMotivo_Apontamento" class="form-control" MaxLength="200"></asp:TextBox>
                                        </div>

                                        <div class="col-lg-12 form-group">
                                            <label>Observação</label>
                                            <textarea runat="server" id="txtObservacao_Apontamento" class="htmlEditor"></textarea>
                                        </div>
                                    </div>

                                    <div class="row modal-footer">
                                        <asp:Button ID="cmdEditarApontamentos" class="btn btn-success" runat="server" Text="Salvar" />
                                        <asp:Button ID="cmdNovoApontamento" class="btn btn-success" runat="server" Text="Novo Apontamento" />
                                        <button type="button" class="btn btn-warning" data-dismiss="modal">Cancelar</button>
                                    </div>

                                </div>

                                <div role="tabpanel" class="tab-pane fade" id="historicoApontamento" aria-labelledby="historicoApontamento-tab">
                                    <br />

                                    <div runat="server" id="ltrHistoricoApontamento" class="tableClean">
                                    </div>

                                </div>

                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <div class="modal fade" id="modalStatusAtividade" data-backdrop="static" style="z-index: 3100;">
                <div class="modal-dialog" style="width: 40%;">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h3 class="modal-title">Confirmar Alteração de Status</h3>
                        </div>
                        <div class="modal-body">

                            <uc1:MensagemPagina runat="server" ID="Mensagem_Modal_StatusAtividade" />

                            <div class="row">

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Observação / Motivo</label>
                                        <asp:TextBox ID="txtObservacao_StatusAtividade" class="form-control" runat="server" TextMode="MultiLine" Height="200px"></asp:TextBox>
                                    </div>
                                </div>

                            </div>

                            <div class="row modal-footer">
                                <asp:Button ID="cmdConfirmarStatus_Atividade" class="btn btn-success" runat="server" Text="Confirmar" />
                                <button type="button" class="btn btn-warning" data-dismiss="modal">Cancelar</button>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
