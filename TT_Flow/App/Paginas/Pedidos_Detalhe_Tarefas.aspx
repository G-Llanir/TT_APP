<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Pedidos_Detalhe_Tarefas.aspx.cs" Inherits="TT_Flow.App.Paginas.Pedidos_Detalhe_Tarefas" %>

<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <title></title>
    <link href="https://login.tecandtec.com.br/app/css/bootstrap.css" rel="stylesheet" />
    <link href="https://login.tecandtec.com.br/app/css/font-awesome/css/font-awesome.css" rel="stylesheet" />
    <link href="https://login.tecandtec.com.br/app/css/TT.css" rel="stylesheet" />
    <link href="https://login.tecandtec.com.br/app/css/plugins/datepicker/css/datepicker.css" rel="stylesheet" />
    <link href="https://login.tecandtec.com.br/app/css/jquery-ui.css" rel="stylesheet" type="text/css" />

    <script src="https://login.tecandtec.com.br/app/js/jquery-3.2.0.min.js"></script>
    <script src="https://login.tecandtec.com.br/plugins/Chosen/docsupport/jquery-3.2.1.min.js" type="text/javascript"></script>
    <script src="https://login.tecandtec.com.br/plugins/Chosen/chosen.jquery.js" type="text/javascript"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-multiselect/0.9.13/js/bootstrap-multiselect.js"></script>
    <script src="https://login.tecandtec.com.br/app/js/bootstrap.min.js"></script>
    <script src="https://login.tecandtec.com.br/app/js/plugins/metisMenu/jquery.metisMenu.js"></script>
    <script src="https://login.tecandtec.com.br/app/js/sb-admin.js"></script>
    <script src="https://login.tecandtec.com.br/app/js/plugins/morris/morris.js"></script>
    <script src="https://login.tecandtec.com.br/app/js/plugins/morris/raphael-2.1.0.min.js"></script>
    <script src="https://login.tecandtec.com.br/app/js/plugins/dataTables/datatables.min.js"></script>
    <script src="https://login.tecandtec.com.br/app/js/plugins/jquery.mask.js"></script>
    <script src="https://login.tecandtec.com.br/app/js/plugins/datepicker/bootstrap-datepicker.js"></script>
    <script src="https://login.tecandtec.com.br/app/js/jquery-1.10.0.min.js" type="text/javascript"></script>
    <script src="https://login.tecandtec.com.br/app/js/jquery-1.9.2.min.js" type="text/javascript"></script>
    <script> $v192 = jQuery.noConflict();</script>

</head>
<body>
    <form id="frmTarefas" runat="server">
        <asp:ScriptManager ID="sm_PedidosTarefas" runat="server"></asp:ScriptManager>
        <br />
        <div class="col-lg-12">
            <div class="form-stacked row">
                <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                <asp:MultiView ID="BaseMultiView" runat="server">

                    <asp:View ID="view_Tarefas" runat="server">
                        <div class="panel panel-default" runat="server">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Tarefas</b></h3>
                            </div>
                            <div class="panel-body">

                                <div class="row">
                                    <div class="col-lg-3">
                                        <div class="form-group">
                                            <asp:DropDownList ID="ddlDepartamento" runat="server" AutoPostBack="true" class="form-control yes_no select CaixaTextoMedio" OnSelectedIndexChanged="ddlDepartamento_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-lg-3">
                                        <div class="form-group">
                                            <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="true" class="form-control yes_no select CaixaTextoMedio" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>

                                <div class="table-responsive">
                                    <asp:GridView ID="gv_Tarefas" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" OnRowDataBound="gv_Tarefas_RowDataBound">
                                        <Columns>

                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <div style="text-align: center;">
                                                        <asp:CheckBox ID="chkTarefas_Seleciona_Todos" runat="server" AutoPostBack="true" OnCheckedChanged="chkTarefas_Seleciona_Todos_CheckedChanged" />
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkTarefas_Seleciona" runat="server" />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="4%" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="idRegistroTarefa" HeaderText="ID">
                                                <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idPedido,idRegistroTarefa"
                                                DataTextField="dtInclusao" HeaderText="Data"
                                                DataNavigateUrlFormatString="Pedidos_Detalhe_Tarefas.aspx?id={0}&idrt={1}">
                                                <HeaderStyle Width="8%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idPedido,idRegistroTarefa"
                                                DataTextField="sDscDepartamento" HeaderText="Departamento"
                                                DataNavigateUrlFormatString="Pedidos_Detalhe_Tarefas.aspx?id={0}&idrt={1}">
                                                <HeaderStyle Width="15%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idPedido,idRegistroTarefa"
                                                DataTextField="sDscUsuarioResponsavel" HeaderText="Responsável"
                                                DataNavigateUrlFormatString="Pedidos_Detalhe_Tarefas.aspx?id={0}&idrt={1}">
                                                <HeaderStyle Width="15%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idPedido,idRegistroTarefa"
                                                DataTextField="sDscTarefa" HeaderText="Tarefa"
                                                DataNavigateUrlFormatString="Pedidos_Detalhe_Tarefas.aspx?id={0}&idrt={1}">
                                                <HeaderStyle Width="20%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idPedido,idRegistroTarefa"
                                                DataTextField="dtPrevisaoConclusao" HeaderText="Previsão"
                                                DataNavigateUrlFormatString="Pedidos_Detalhe_Tarefas.aspx?id={0}&idrt={1}">
                                                <HeaderStyle Width="10%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:BoundField DataField="sDscStatusTarefa" HeaderText="Status">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                        </Columns>
                                    </asp:GridView>
                                    <fieldset class="form-stacked actions">
                                        <asp:Button ID="cmdTarefas_Excluir" class="btn  btn-sm btn-danger" runat="server" Text="Excluir Tarefas Selecionadas" OnClick="cmdTarefas_Excluir_Click" />
                                        <asp:Button ID="cmdTarefas_Finalizar" class="btn  btn-sm btn-success" runat="server" Text="Finalizar Múltiplas Tarefas" OnClick="cmdTarefas_Finalizar_Click" />
                                    </fieldset>
                                    <div id="div_Tarefas_Acoes" class="row">
                                        <div class="col-lg-11">
                                            <div class="panel panel-primary">
                                                <div class="panel-heading">
                                                    <h3 class="panel-title">
                                                        <asp:Label ID="lblTarefas_Acao_Titulo" runat="server" Text="Label"></asp:Label></h3>
                                                </div>
                                                <div class="panel-body">
                                                    <div class="col-lg-3">
                                                        <div class="form-group">
                                                            <label>Motivo/Observação</label>
                                                            <asp:TextBox ID="txtTarefas_Observacao" class="form-control" runat="server" TextMode="MultiLine" Height="80px" MaxLength="300"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-4">
                                                        <div class="form-group">
                                                            <br />
                                                            <asp:Button ID="cmdTarefas_OK" class="btn btn-sm btn-primary" runat="server" Text="Ok" OnClick="cmdTarefas_OK_Click" />
                                                            <asp:Button ID="cmdTarefas_Cancelar" class="btn btn-sm btn-danger" runat="server" Text="Cancelar" OnClick="cmdTarefas_Cancelar_Click" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>

                            </div>
                        </div>
                    </asp:View>

                    <asp:View ID="view_Tarefa_Detalhe" runat="server">

                        <div class="panel panel-default" runat="server">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Detalhe da Tarefa</b></h3>
                            </div>
                            <div class="panel-body">

                                <fieldset class="form-stacked actions">

                                    <div class="btn-group">
                                        <button id="cmdIniciar" runat="server" type="button" class="btn btn-sm btn-success dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Iniciar  <span class="caret"></span></button>
                                        <ul class="dropdown-menu">
                                            <li>
                                                <asp:LinkButton ID="lnkIniciarTarefa" runat="server" OnClick="lnkIniciarTarefa_Click">Iniciar Tarefa</asp:LinkButton></li>
                                            <li role="separator" class="divider"></li>
                                            <li>
                                                <asp:LinkButton ID="lnkIniciarTarefaSemAlterarUsuario" runat="server" OnClick="lnkIniciarTarefaSemAlterarUsuario_Click">Iniciar Tarefa sem alterar Responsável</asp:LinkButton></li>

                                        </ul>
                                    </div>

                                    <asp:Button ID="cmdFinalizarTarefa" class="btn  btn-sm btn-success" runat="server" Text="Finalizar" OnClick="cmdFinalizarTarefa_Click" />
                                    <asp:Button ID="cmdRejeitarTarefa" class="btn  btn-sm btn-default" runat="server" Text="Rejeitar" OnClick="cmdRejeitarTarefa_Click" />
                                    <asp:Button ID="cmdObservacao" class="btn  btn-sm btn-info" runat="server" Text="Inserir Observação" OnClick="cmdObservacao_Click" />
                                    <asp:Button ID="cmdExcluirTarefa" class="btn  btn-sm btn-danger" runat="server" Text="Excluir" OnClick="cmdExcluirTarefa_Click" />
                                    <asp:Button ID="cmdVoltar" class="btn  btn-sm  btn-warning" runat="server" Text="Voltar" OnClick="cmdVoltar_Click" />
                                    <asp:Button ID="cmdProximaTarefa" class="btn  btn-sm  btn-success" runat="server" Text="Próxima Tarefa" OnClick="cmdProximaTarefa_Click" />
                                </fieldset>

                                <div id="div_Acoes" class="row">
                                    <div class="col-lg-11">
                                        <div class="panel panel-primary">
                                            <div class="panel-heading">
                                                <h3 class="panel-title">
                                                    <asp:Label ID="lblAcao_Titulo" runat="server" Text="Label"></asp:Label></h3>
                                            </div>
                                            <div class="panel-body">
                                                <div class="col-lg-3">
                                                    <div class="form-group">
                                                        <label>Motivo/Observação</label>
                                                        <asp:TextBox ID="txtAcao_Observacao" class="form-control" runat="server" TextMode="MultiLine" Height="80px" MaxLength="300"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-lg-4">
                                                    <div class="form-group">
                                                        <br />
                                                        <asp:Button ID="cmdAcao_Confirmar" class="btn btn-sm btn-primary AcaoConfirmar" runat="server" Text="Ok" />
                                                        <asp:Button ID="cmdAcao_Cancelar" class="btn btn-sm btn-danger" runat="server" Text="Cancelar" OnClick="cmdAcao_Cancelar_Click" />
                                                    </div>
                                                </div>
                                                <asp:HiddenField ID="hddacao_sFuncao" runat="server" />

                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-stacked row">

                                    <div class="col-lg-6">
                                        <div class="row">
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>ID </label>
                                                    <asp:TextBox ID="txtidRegistroTarefa" class="form-control CaixaTextoMini" runat="server" disabled=""></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-lg-3">
                                                <div class="form-group">
                                                    <label>Degrau</label>
                                                    <asp:TextBox ID="txtnDegrauTarefa" class="form-control CaixaTextoMini" runat="server" disabled=""></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>Data Inclusão</label>
                                                    <asp:TextBox ID="txtdtInclusao" class="form-control CaixaTextoMedio" runat="server" MaxLength="20"> </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-6">
                                        <div class="form-group">
                                            <label>Departamento</label>
                                            <asp:TextBox ID="txtsDscDepartamento" class="form-control CaixaTextoGigante" runat="server"> </asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-6">
                                        <div class="form-group">
                                            <label>Responsável</label>
                                            <asp:DropDownList ID="ddlResponsavel" runat="server" class="form-control yes_no select CaixaTextoGrande"></asp:DropDownList>

                                        </div>
                                    </div>

                                    <div class="col-lg-6">
                                        <div class="form-group">
                                            <label>Tarefa</label>
                                            <asp:TextBox ID="txtsDscTarefa" class="form-control  CaixaTextoGigante" runat="server" MaxLength="200"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-6">
                                        <div class="row">
                                            <div class="col-lg-5">
                                                <div class="form-group">
                                                    <label>Previsão de Conclusão</label>
                                                    <asp:TextBox ID="txtdtPrevisaoConclusao" class="form-control CaixaTextoPequeno" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>Status</label>
                                                    <asp:TextBox ID="txtsDscStatusTarefa" class="form-control CaixaTextoGrande" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-12">
                                        <div class="form-group">
                                            <label>Descrição da Tarefa/Observações</label>
                                            <asp:TextBox ID="txtsObservacaoTarefa" class="form-control" runat="server" TextMode="MultiLine" Height="80px"></asp:TextBox>
                                        </div>
                                    </div>

                                </div>

                                <fieldset class="form-stacked actions" runat="server" id="div_botoes_Alteracao">
                                    <asp:Button ID="cmdSalvarAlteracoes" class="btn  btn-sm btn-info" runat="server" Text="Salvar Alterações" OnClick="cmdSalvarAlteracoes_Click" />
                                    <asp:Button ID="cmdVoltar_Alteracoes" class="btn  btn-sm  btn-warning" runat="server" Text="Voltar" OnClick="cmdVoltar_Click" />
                                </fieldset>

                            </div>
                        </div>

                        <div class="panel panel-default" id="DIV_Arquivos" runat="server">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Arquivos</b></h3>
                            </div>

                            <div class="panel-body">
                                <div class="form-stacked row">
                                    <div class="col-lg-12 table-responsive">
                                        <asp:GridView ID="gv_Arquivo" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" OnRowCommand="gv_Arquivo_RowCommand" OnRowDataBound="gv_Arquivo_RowDataBound">
                                            <Columns>
                                                <asp:BoundField DataField="idArquivo" HeaderText="ID">
                                                    <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Inclusão">
                                                    <ItemStyle Width="15%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:ButtonField DataTextField="sNomeArquivo" HeaderText="Nome do Arquivo" CommandName="Download" ItemStyle-Width="20%" />

                                                <asp:BoundField DataField="sDscArquivo" HeaderText="Descrição">
                                                    <ItemStyle Width="40%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Usuário">
                                                    <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>
                                            </Columns>
                                        </asp:GridView>

                                    </div>
                                </div>
                                <div id="div_EnviarArquivos" class="form-stacked " runat="server">
                                    <div class="panel panel-default" runat="server">
                                        <div class="panel-heading">
                                            <h3 class="panel-title"><b>Enviar Arquivos</b></h3>
                                        </div>
                                        <uc1:MensagemPagina runat="server" ID="MensagemArquivo" />
                                        <div class="panel-body">
                                            <div class="form-stacked row">

                                                <div class="col-lg-6" id="div_EnviarArquivos_Selecao" runat="server">
                                                    <div class="form-group">
                                                        <label>Selecione o Arquivo</label>
                                                        <asp:FileUpload ID="fu_Arquivo" class="form-control-file" runat="server" Width="400px" />
                                                    </div>
                                                </div>
                                                <div class="col-lg-12">
                                                    <div class="form-group">
                                                        <label>
                                                            <asp:Label ID="lblEnviarArquivo_sDscArquivo" runat="server" Text="Descrição"></asp:Label></label>
                                                        <asp:TextBox ID="txtEnviarArquivo_sDscArquivo" class="form-control CaixaTextoGigante" runat="server" MaxLength="300"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-lg-12">
                                                    <div class="form-group">
                                                        <asp:Button ID="cmdEnviarArquivos" CssClass="btn-success btn" runat="server" Text="Enviar" OnClick="cmdEnviarArquivos_Click" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>

                        <div class="panel panel-default" runat="server" id="Div_Historico">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Histórico</b></h3>
                            </div>
                            <div class="panel-body">
                                <div class="form-stacked row">
                                    <div class="col-lg-12 table-responsive">
                                        <asp:GridView
                                            ID="gv_Tarefas_Historico" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False">
                                            <Columns>
                                                <asp:BoundField DataField="dtHistorico" HeaderText="Data">
                                                    <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário">
                                                    <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="sDscHistorico" HeaderText="Ação">
                                                    <ItemStyle HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>
                                            </Columns>
                                        </asp:GridView>

                                    </div>
                                </div>

                            </div>
                        </div>

                        <asp:HiddenField ID="hddidRegistroTarefa" runat="server" />
                    </asp:View>

                </asp:MultiView>
            </div>
        </div>

        <div id="dialog_OK" class="modal" title="Alterar data de Previsão ?">
            <p>
                <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                <asp:Label ID="Label2" runat="server" Text="Confirmar Alteração da Data de Previsão ?"></asp:Label>
            </p>
        </div>


        <asp:HiddenField ID="hddidPedido" runat="server" />
        <asp:HiddenField ID="hddsAtualizaPagina" runat="server" />
        <asp:HiddenField ID="hddidTipo" runat="server" />
        <asp:HiddenField ID="hddidRegistroTarefas" runat="server" />
        <asp:HiddenField ID="hddMaximoidRegistro" runat="server" />
        <asp:HiddenField ID="hddFinalizarTarefas" runat="server" Value="N" />
        <asp:HiddenField ID="hddExcluirTarefas" runat="server" Value="N" />
        <asp:HiddenField ID="hddDataconcluir" runat="server" />
        <asp:HiddenField ID="hddAlterarData" runat="server" />
    </form>
</body>
</html>
