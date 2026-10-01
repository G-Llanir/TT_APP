<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" ValidateRequest="false" AutoEventWireup="true" CodeBehind="Solicitacao_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.Solicitacoes.Solicitacao_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HtmlEditor" TagPrefix="HTMLEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        .editor {
            padding: 3px;
            background-color: white;
            border: solid 1px;
            border-color: lightgray;
            border-radius: 8px;
        }

        /* --- NOVO CSS PARA O LOADER --- */
        .loader-mask {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0, 0, 0, 0.1);
            z-index: 50;
            display: flex;
            justify-content: center;
            align-items: center;
            border-radius: 4px;
        }

        .loader-spinner {
            border: 5px solid #f3f3f3;
            border-top: 5px solid #337ab7; /* Azul padrão do Bootstrap */
            border-radius: 50%;
            width: 50px;
            height: 50px;
            animation: spin 1s linear infinite;
        }

        @keyframes spin {
            0% {
                transform: rotate(0deg);
            }

            100% {
                transform: rotate(360deg);
            }
        }
    </style>

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css">
    <div class="form-stacked">
        <div class="col-lg-12 row">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Solicitação"></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
        </div>

        <div>
            <ul id="tab_Solicitacao" class="nav nav-tabs" role="tablist">
                <li role="presentation" class="tabpanel active">
                    <a href="#solicitacao" id="aba_Solicitacao" role="tab" data-toggle="tab" aria-controls="solicitacao"><b>Solicitação</b></a>
                </li>
                <li role="presentation" runat="server" id="aba_Arquivos" visible="false">
                    <a href="#arquivos" role="tab" id="arquivos-tab" data-toggle="tab" aria-controls="Arquivos"><b>Arquivos</b></a>
                </li>
            </ul>
        </div>
        <div id="tab" class="tab-content">
            <div role="tabpanel" class="tab-pane fade in active" id="solicitacao" aria-labelledby="solicitacao-tab">

                <asp:UpdatePanel ID="updDetalhe" runat="server">
                    <ContentTemplate>
                        <%-- HiddenField para armazenar a ação selecionada --%>
                        <asp:HiddenField ID="hddSelectedAction" runat="server" />
                        <asp:HiddenField ID="hddAlterarData" runat="server" Value="N" />
                        <asp:HiddenField ID="hddsFluxoTerceiro" runat="server" />
                        <br />

                        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                        <div class="panel panel-default" runat="server" id="div_Solicitacao">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Dados de Solicitação</b></h3>
                            </div>
                            <div class="panel-body">
                                <div class="form-stacked row">
                                    <div class="col-lg-8">
                                        <div class="form-group">
                                            <label>ID</label>
                                            <asp:TextBox ID="txtidSolicitacao" class="form-control CaixaTextoMini" runat="server" disabled=""></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <h3>
                                                <span class="status" title="Status">
                                                    <asp:Label ID="lblsStatus" runat="server" TabIndex="100"></asp:Label>
                                                </span>
                                            </h3>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-stacked row">
                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label runat="server" id="Label2">Data da Solicitação</label>
                                            <asp:TextBox ID="txtdtSolicitacao" type="date" class="form-control" runat="server" disabled="true"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-stacked row">
                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label runat="server" id="lblDtInicio">Data de Ínício</label>
                                            <asp:TextBox ID="txtsdtInicio" type="date" class="form-control" runat="server" disabled=""></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2" runat="server" id="div_dtFinal">
                                        <div class="form-group">
                                            <label runat="server" id="lblDtFinal">Data Final</label>

                                            <div class="input-group">
                                                <asp:TextBox ID="txtsdtFinal" type="date" class="form-control" runat="server" disabled=""></asp:TextBox>

                                                <span class="input-group-addon bg-info" id="divCalcDiasMain" style="display: none; border-left: 0; font-weight: bold; color: #31708f; background-color: #d9edf7;">
                                                    <i class="fa fa-calendar"></i> <span id="lblDiasMain">0</span> Dias
            </span>
                                            </div>

                                        </div>
                                    </div>

                                    <div class="col-lg-4" runat="server" id="div_espacoBranco">
                                        &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp
                                    </div>

                                    <div class="col-lg-4">
                                        <label>&nbsp </label>

                                        <asp:Panel ID="pnlAcoes" runat="server" Visible="false">
                                            <div id="Div_AprovacaoSupervisor" class="pull-left" runat="server" style="margin-right: 10px;">
                                                <div class="btn-group dropup">
                                                    <%-- CLASSE ADICIONADA AQUI --%>
                                                    <button id="cmdListaAprovar" runat="server" type="button" class="btn btn-sm btn-warning dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Ação <span class="caret"></span></button>
                                                    <ul class="dropdown-menu">
                                                        <asp:UpdatePanel runat="server" ID="UpdBotoes" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <li>
                                                                    <asp:LinkButton ID="cmdAprovarSupervisor" class="btn btn-primary" Style="width: 100%;" runat="server" Text="Aprovar" OnClientClick="return openActionModal('AprovarSupervisor');" />
                                                                </li>
                                                                <li role="separator" class="divider" runat="server" id="btnDivisor1"></li>
                                                                <li>
                                                                    <asp:LinkButton ID="cmdRejeitarSupervisor" class="btn btn-danger" Style="width: 100%;" runat="server" Text="Rejeitar" OnClientClick="return openActionModal('RejeitarSupervisor');" />
                                                                </li>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </ul>
                                                </div>
                                            </div>

                                            <div id="Div_AprovacaoRH" runat="server" style="display: flex; align-items: center; gap: 10px;">

                                                <div class="btn-group btn-sm dropup">
                                                    <button id="Button1" runat="server" type="button" class="btn btn-sm btn-warning dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Ação <span class="caret"></span></button>
                                                    <ul class="dropdown-menu">
                                                        <li>
                                                            <asp:LinkButton ID="cmdAprovarRH" class="btn btn-success" Style="width: 100%;" runat="server" Text="Aprovar (RH)" OnClientClick="return openActionModal('AprovarRH');" />
                                                        </li>
                                                        <li role="separator" class="divider" runat="server" id="btnDivisor2"></li>
                                                        <li>
                                                            <asp:LinkButton ID="cmdRejeitarRH" class="btn btn-danger" Style="width: 100%;" runat="server" Text="Rejeitar (RH)" OnClientClick="return openActionModal('RejeitarRH');" />
                                                        </li>
                                                        <li role="separator" class="divider" runat="server" id="Li1"></li>
                                                        <li>
                                                            <asp:LinkButton ID="cmdPendenteDoc" class="btn btn-warning" Style="width: 100%;" runat="server" Text="Pendente Documentação" OnClientClick="return openActionModal('PendenteDoc');" />
                                                        </li>
                                                        <li role="separator" class="divider" runat="server" id="btnDivisor3"></li>
                                                        <li>
                                                            <asp:LinkButton ID="cmdCancelado" class="btn btn-warning" Style="width: 100%;" runat="server" Text="Cancelar Solicitação" OnClientClick="return openActionModal('Cancelado');" />
                                                        </li>
                                                        <li>
                                                            <asp:LinkButton ID="cmdFinalizado" class="btn btn-secondary" Style="width: 100%;" runat="server" Text="Finalizar Solicitação" OnClientClick="return openActionModal('Finalizado');" />
                                                        </li>
                                                    </ul>
                                                </div>

                                                <div id="div_PDF" runat="server" visible="true">
                                                    <asp:LinkButton ID="cmdGerarPDF" class="btn btn-sm btn-info" runat="server" Text="Gerar PDF" OnClick="cmdGerarPDF_Click" />
                                                </div>

                                            </div>


                                            <div id="Div_AprovacaoDiretor" class="pull-left" runat="server" style="margin-right: 10px;">
                                                <div class="btn-group dropup">
                                                    <%-- CLASSE ADICIONADA AQUI --%>
                                                    <button id="Button2" runat="server" type="button" class="btn btn-sm btn-warning dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Ação <span class="caret"></span></button>
                                                    <ul class="dropdown-menu">
                                                        <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <li>
                                                                    <asp:LinkButton ID="cmdAprovarDiretor" class="btn btn-primary" Style="width: 100%;" runat="server" Text="Aprovar" OnClientClick="return openActionModal('AprovarDiretor');" />
                                                                </li>
                                                                <li role="separator" class="divider" runat="server" id="Li2"></li>
                                                                <li>
                                                                    <asp:LinkButton ID="RejeitarDiretor" class="btn btn-danger" Style="width: 100%;" runat="server" Text="Rejeitar" OnClientClick="return openActionModal('RejeitarDiretor');" />
                                                                </li>
                                                                <li role="separator" class="divider" runat="server" id="Li4"></li>
                                                                <li>
                                                                    <asp:LinkButton ID="cmdPendenteDocDiretor" class="btn btn-warning" Style="width: 100%;" runat="server" Text="Pendente Documentação" OnClientClick="return openActionModal('PendenteDoc');" />
                                                                </li>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </ul>
                                                </div>
                                            </div>

                                            <div id="Div_AprovacaoTerceiros" class="pull-left" runat="server" style="margin-right: 10px;">
                                                <div class="btn-group dropup">
                                                    <%-- CLASSE ADICIONADA AQUI --%>
                                                    <button id="Button3" runat="server" type="button" class="btn btn-sm btn-warning dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Ação <span class="caret"></span></button>
                                                    <ul class="dropdown-menu">
                                                        <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <li>
                                                                    <asp:LinkButton ID="cmdAprovarTerceiros" class="btn btn-primary" Style="width: 100%;" runat="server" Text="Aprovar Para Pagamento" OnClientClick="return openActionModal('AprovarTerceiros');" />
                                                                </li>
                                                                <li role="separator" class="divider" runat="server" id="Li3"></li>
                                                                <li>
                                                                    <asp:LinkButton ID="cmdRejeitarTerceiros" class="btn btn-danger" Style="width: 100%;" runat="server" Text="Rejeitar" OnClientClick="return openActionModal('RejeitarTerceiros');" />
                                                                </li>
                                                                <li role="separator" class="divider" runat="server" id="Li5"></li>
                                                                <li>
                                                                    <asp:LinkButton ID="cmdPendenteDocTerceiros" class="btn btn-warning" Style="width: 100%;" runat="server" Text="Pendente Documentação" OnClientClick="return openActionModal('PendenteDoc');" />
                                                                </li>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </ul>
                                                </div>
                                            </div>

                                        </asp:Panel>
                                    </div>

                                    <div class="col-lg-3">
                                        <div class="form-group">
                                            <asp:DropDownList ID="ddlidStatus" class="form-control" runat="server" disabled="" Visible="false"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-stacked row" runat="server" id="div_infoExtra" visible="false">
                                    <div class="col-lg-2" runat="server" id="div_Solicitante">
                                        <div class="form-group">
                                            <label>Solicitante</label>
                                            <asp:TextBox ID="txtSolicitante" class="form-control" runat="server" disabled=""></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-lg-2" runat="server" id="div_Departamento">
                                        <div class="form-group">
                                            <label>Depto. do Solicitante</label>
                                            <asp:TextBox ID="txtDeptoSolicitante" class="form-control" runat="server" disabled=""></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-lg-2" runat="server" id="div_EmailSolicitante">
                                        <div class="form-group">
                                            <label>Email do Solicitante</label>
                                            <asp:TextBox ID="txtEmailSolicitante" class="form-control" runat="server" disabled=""></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-stacked row">
                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Tipo de Solicitação</label>
                                            <asp:DropDownList ID="ddlidTipo" class="form-control" runat="server" disabled=""></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-5">
                                        <div class="form-group">
                                            <label>Título</label>
                                            <asp:TextBox ID="txtsDscSolicitacao" class="form-control" runat="server" disabled=""></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-stacked row">
                                    <div class="col-lg-12">
                                        <div class="form-group">
                                            <label>Observação</label>
                                            <HTMLEditor:Editor runat="server"
                                                ID="txtsObservacaoSolicitacao"
                                                Height="300px"
                                                Width="100%"
                                                CssClass="editor"
                                                HtmlPanelCssClass="editor" />
                                        </div>
                                    </div>
                                </div>

                                <asp:HiddenField ID="hddidSolicitacao" runat="server" />
                            </div>
                        </div>

                        <div class="panel panel-default" runat="server" id="div1">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Fluxo de Aprovação</b></h3>
                            </div>
                            <div class="panel-body">
                                <div class="form-stacked row">
                                    <div class="col-lg-12">
                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina1" />
                                        <asp:Panel ID="pnResultado" class="" runat="server">
                                            <div id="resultado">
                                                <div class="panel panel-primary">
                                                    <div class="panel-body">
                                                        <div class="table-responsive">
                                                            <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                                                <Columns>
                                                                    <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Preenchimento">
                                                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    </asp:BoundField>
                                                                    <%--  <asp:BoundField DataField="sMotivo" HeaderText="Motivo">
                                                                        <ItemStyle Width="50%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    </asp:BoundField>--%>
                                                                    <asp:TemplateField HeaderText="Motivo Literal">
                                                                        <ItemTemplate>
                                                                            <asp:Literal ID="ltlMotivo" runat="server" Text='<%# Eval("sMotivo") %>'></asp:Literal>
                                                                        </ItemTemplate>
                                                                        <ItemStyle Width="50%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    </asp:TemplateField>
                                                                    <asp:HyperLinkField DataNavigateUrlFields="idUsuario" DataTextField="sUsuario" HeaderText="Usuário" DataNavigateUrlFormatString="../Manutencao/Usuarios_Detalhe.aspx?idu={0}">
                                                                        <HeaderStyle Width="15%" />
                                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    </asp:HyperLinkField>
                                                                    <asp:HyperLinkField DataNavigateUrlFields="idDepartamento" DataTextField="sDepartamento" HeaderText="Departamento" DataNavigateUrlFormatString="../Manutencao/Departamentos_Detalhe.aspx?id={0}">
                                                                        <HeaderStyle Width="15%" />
                                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    </asp:HyperLinkField>
                                                                    <asp:BoundField DataField="sStatus" HeaderText="Status">
                                                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
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

                        <%--  <div class="panel panel-default" runat="server" id="divArquivoForm" visible="false">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Arquivos Anexados</b></h3>
                            </div>
                            <div class="panel-body">
                                <div id="loadingMaskArquivo" class="loader-mask">
                                    <div class="loader-spinner"></div>
                                    <h4 style="margin-left: 15px; color: #555;">Carregando visualização...</h4>
                                </div>

                                <div class="col-lg-12" runat="server" id="div_arquivos">
                                    <div class="embed-responsive embed-responsive-16by9" style="min-height: 200px">
                                        <embed type="text/html" runat="server" id="frmArquivos" width="800" height="500" />
                                    </div>
                                </div>
                            </div>
                        </div>--%>

                        <%-- ####### MODAL ####### --%>
                        <div class="modal fade" id="myModal" data-backdrop="static" role="dialog">
                            <div class="modal-dialog">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                                        <h4 class="modal-title">Gerenciar Solicitação</h4>
                                    </div>
                                    <div class="modal-body">
                                        <uc1:MensagemPagina runat="server" ID="MensagemModal" />
                                        <div class="col-lg-12">
                                            <div class="row">

                                                <div id="div_DataAlterar" runat="server" visible="false">

                                                    <div id="div_flagAlterar" runat="server" class="col-lg-10 form-group">
                                                        <uc1:SwitchAtivo runat="server" ID="SwitchAtivo" ClientIDMode="Static" />
                                                    </div>
                                                    <div class="col-lg-2" id="divCalcDiasModal" style="display: none; padding-top: 25px;">
                                                        <span class="label label-primary" style="font-size: 100%; padding: 8px 12px; display: inline-block;">
                                                            <i class="fa fa-calculator"></i><span id="lblDiasModal">0</span> Dias
</span>
                                                    </div>
                                                    <div id="div_Datas" runat="server" style="display: none;">

                                                        <div class="col-lg-12 form-group" style="background: #f9f9f9; padding: 10px; border-radius: 5px;"
                                                            runat="server" id="divOpc">
                                                            <uc1:SwitchAtivo runat="server" ID="SwitchOpcoesDatas" ClientIDMode="Static" />
                                                            <small class="text-muted" style="display: block; margin-top: 5px;">Ative para ver datas alternativas sugeridas pelo colaborador.</small>
                                                        </div>

                                                        <div class="col-lg-6">
                                                            <div class="form-group">
                                                                <label runat="server" id="Label1">Nova Data de Início</label>
                                                                <asp:TextBox ID="txtdtInicialSugerida" type="date" class="form-control" runat="server" onchange="calcDiasModal()" onkeyup="calcDiasModal()"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-6" runat="server" id="div2">
                                                            <div class="form-group">
                                                                <label>Nova Data Final</label>
                                                                <asp:TextBox ID="txtdtFinalSugerida" type="date" class="form-control" runat="server" onchange="calcDiasModal()" onkeyup="calcDiasModal()"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div id="div_ListaOpcoesDatas" class="col-lg-12" style="display: none; margin-bottom: 15px;">
                                                            <asp:Repeater ID="rptOpcoesDatas" runat="server">
                                                                <ItemTemplate>
                                                                    <div class="panel panel-info" style="cursor: pointer;"
                                                                        onclick="selecionarDataOpcao(this, '<%# Eval("DtInicio", "{0:yyyy-MM-dd}") %>', '<%# Eval("DtFinal", "{0:yyyy-MM-dd}") %>')">
                                                                        <div class="panel-body">
                                                                            <div class="input-group">
                                                                                <span class="input-group-addon">
                                                                                    <input type="radio" name="rdOpcaoData" />
                                                                                </span>
                                                                                <div class="form-control" style="height: auto;">
                                                                                    <strong>Opção <%# Container.ItemIndex + 1 %>:</strong><br />
                                                                                    <i class="fa fa-calendar"></i>De: <%# Eval("DtInicio", "{0:dd/MM/yyyy}") %>
                                                                                    <br />
                                                                                    <i class="fa fa-calendar"></i>Até: <%# Eval("DtFinal", "{0:dd/MM/yyyy}") %>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                            <div id="msgSemOpcoes" runat="server" visible="false" class="alert alert-warning btn-sm">
                                                                Nenhuma opção extra registrada.
                                                            </div>
                                                        </div>

                                                    </div>
                                                </div>

                                                <div id="div_Motivo" runat="server" class="col-lg-12">
                                                    <div class="form-group">
                                                        <label>Motivo</label>
                                                        <asp:TextBox ID="txtsObservacao" class="form-control" TextMode="multiline" Height="150px" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>

                                            </div>
                                        </div>
                                    </div>
                                    <div class="modal-footer">
                                        <%-- NOVO Botão de confirmar ação --%>
                                        <asp:Button ID="cmdConfirmarAcao" runat="server" Text="Confirmar Ação" OnClick="cmdConfirmarAcao_Click" CssClass="btn btn-primary" />
                                        <button type="button" class="btn btn-default" data-dismiss="modal">Cancelar</button>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </ContentTemplate>
                </asp:UpdatePanel>

                <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                <br />
            </div>

            <%--arquivos--%>
            <div role="tabpanel" class="tab-pane fade" id="arquivos" aria-labelledby="arquivos-tab">
                <br />
                <div class="panel panel-default" runat="server" id="divArquivoForm" visible="false">
                    <div class="panel-heading">
                        <h3 class="panel-title"><b>Arquivos Anexados</b></h3>
                    </div>
                    <div class="panel-body">

                        <div class="col-lg-12" runat="server" id="div_arquivos" style="position: relative; min-height: 200px;">

                            <div id="loadingMaskArquivo" class="loader-mask">
                                <div class="loader-spinner"></div>
                                <h4 style="margin-left: 15px; color: #555;">Carregando visualização...</h4>
                            </div>

                            <div class="embed-responsive embed-responsive-16by9" style="min-height: 200px">
                                <embed type="text/html" runat="server" id="frmArquivos" width="800" height="500" />
                            </div>
                        </div>

                    </div>
                </div>

            </div>
        </div>

        <%-- ####### BOTÕES DE AÇÃO NO RODAPÉ DA PÁGINA ####### --%>
        <fieldset class="form-stacked actions" runat="server" visible="false">
        </fieldset>

        <div id="dialog-Salvar" class="modal" title="Salvar">
            <p>
                <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
            </p>
        </div>
    </div>

    <style>
        /* Botão Flutuante (FAB) */
        .chat-fab {
            position: fixed;
            bottom: 30px;
            right: 30px;
            width: 60px;
            height: 60px;
            background-color: #007bff;
            color: white;
            border-radius: 50%;
            text-align: center;
            box-shadow: 2px 2px 10px rgba(0,0,0,0.3);
            cursor: pointer;
            z-index: 9999;
            transition: transform 0.3s;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 24px;
        }

            .chat-fab:hover {
                transform: scale(1.1);
                background-color: #0056b3;
            }

        /* Janela do Chat */
        .chat-window {
            position: fixed;
            bottom: 100px;
            right: 30px;
            width: 350px;
            height: 450px;
            background-color: white;
            border-radius: 10px;
            box-shadow: 0 5px 20px rgba(0,0,0,0.2);
            z-index: 99999; /* Z-INDEX ALTO PARA FICAR EM CIMA DOS PAINEIS */
            /* REMOVA display: flex daqui. Deixe o JS controlar */
            display: none;
            flex-direction: column;
            border: 1px solid #ddd;
            overflow: hidden;
        }

        .chat-header {
            background-color: #007bff;
            color: white;
            padding: 10px 15px;
            font-weight: bold;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .chat-body {
            flex: 1;
            padding: 10px;
            overflow-y: auto;
            background-color: #f4f7f6;
            font-size: 0.9em;
            height: 100%;
        }

        .chat-footer {
            padding: 10px;
            background-color: #fff;
            border-top: 1px solid #eee;
        }

        /* Balões de Mensagem */
        .msg-container {
            margin-bottom: 10px;
            clear: both;
        }

        .msg-bubble {
            padding: 8px 12px;
            border-radius: 15px;
            max-width: 80%;
            position: relative;
            word-wrap: break-word;
        }

        .msg-me {
            background-color: #dcf8c6; /* Verde claro tipo WhatsApp */
            float: right;
            border-bottom-right-radius: 0;
            color: #000;
        }

        .msg-other {
            background-color: #fff;
            float: left;
            border-bottom-left-radius: 0;
            border: 1px solid #ddd;
            color: #000;
        }

        .msg-info {
            font-size: 0.75em;
            color: #777;
            margin-top: 4px;
            display: block;
            text-align: right;
        }

        /* --- Badge de Notificação (!) --- */
        .notification-badge {
            position: absolute;
            top: -5px;
            right: -5px;
            background-color: #ff3b30; /* Vermelho alerta */
            color: white;
            border-radius: 50%;
            width: 22px;
            height: 22px;
            font-size: 14px;
            font-weight: bold;
            display: flex;
            justify-content: center;
            align-items: center;
            border: 2px solid white;
            box-shadow: 0 2px 5px rgba(0,0,0,0.2);
            z-index: 10000;
            animation: pulse 2s infinite;
        }

        @keyframes pulse {
            0% {
                transform: scale(1);
            }

            50% {
                transform: scale(1.1);
            }

            100% {
                transform: scale(1);
            }
        }
    </style>

    <div class="chat-fab" style="cursor: pointer;" onclick="return toggleChat();" id="btnFabChat" title="Observações Internas">
        <i class="fa fa-comments"></i>
        <span id="chatNotification" class="notification-badge" style="display: none;">!</span>
    </div>

    <asp:UpdatePanel ID="updChatInterno" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnEnviarReal" EventName="Click" />
        </Triggers>
        <ContentTemplate>
            <asp:Button ID="btnEnviarReal" runat="server" OnClick="btnEnviarMsgInterna_Click" Style="display: none;" />
            <asp:HiddenField ID="hddTemNotificacao" runat="server" Value="N" />

            <div class="chat-window" id="divChatWindow" style="display: none;">

                <div id="chatLoader" class="loader-mask" style="display: none; border-radius: 10px; z-index: 10000;">
                    <div class="loader-spinner" style="width: 30px; height: 30px; border-width: 3px;">
                    </div>
                </div>

                <div class="chat-header">
                    <span><i class="fa fa-lock me-1"></i> Notas Internas</span>
                    <%--<span style="cursor: pointer;" onclick="toggleChat()"><i class="fa fa-times"></i></span>--%>
                    <span style="cursor: pointer;" onclick="return toggleChat();"><i class="fa fa-times"></i></span>
                </div>

                <div class="chat-body" id="chatBodyScroll">
                    <asp:Repeater ID="rptChatInterno" runat="server">
                        <ItemTemplate>
                            <div class="msg-container">
                                <div class='msg-bubble <%# Eval("ClasseCss") %>'>
                                    <div style="white-space: pre-wrap;"><%# Eval("Motivo") %></div>
                                    <span class="msg-info">
                                        <%# Eval("Usuario") %> - <%# Eval("DataAtualizacao") %>
                                 </span>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <div id="divSemMsg" runat="server" visible="false" class="text-center text-muted" style="margin-top: 50px;">
                        <i class="fa fa-comment-slash fa-2x"></i>
                        <br />
                        Nenhuma observação interna.
                
                    </div>
                </div>

                <div class="chat-footer">
                    <div class="input-group">
                        <asp:TextBox ID="txtMsgInterna" runat="server" CssClass="form-control input-sm" placeholder="Digite uma observação..."></asp:TextBox>
                        <span class="input-group-btn">
                            <button type="button" class="btn btn-primary btn-sm" onclick="enviarMsgChat()">
                                <i class="fa fa-paper-plane"></i>
                            </button>
                        </span>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>


    <script type="text/javascript">
        // ==============================================================
        // 1. FUNÇÕES GERAIS (Chat, Modal, Datas, Loader)
        // ==============================================================

        // --- LÓGICA DO CHAT ---

        // Função que clica no botão escondido do servidor


        function toggleChat() {
            var chat = $('#divChatWindow');
            var isVisible = chat.is(':visible');

            if (isVisible) {
                // FECHAR
                chat.hide();
            } else {
                // ABRIR
                // Forçamos o display flex via CSS direto para não quebrar o layout interno
                chat.css('display', 'flex').hide().fadeIn(function () {
                    scrollToBottom(); // Rola para baixo ao terminar de abrir
                    $('#chatNotification').hide(); // Remove notificação
                });

                // Foca no input
                var txt = document.getElementById('<%= txtMsgInterna.ClientID %>');
                if (txt) txt.focus();
            }
            return false; // Impede postback do botão
        }

        function scrollToBottom() {
            var div = document.getElementById('chatBodyScroll');
            if (div) {
                // Tenta rolar imediatamente
                div.scrollTop = div.scrollHeight;
                // E garante com um delay (caso imagens ou fontes demorem)
                setTimeout(function () {
                    div.scrollTop = div.scrollHeight;
                }, 100);
            }
        }

        function verificarNotificacao() {
            // Pega valor do HiddenField preenchido pelo C#
            var temNotificacao = document.getElementById('<%= hddTemNotificacao.ClientID %>').value;
            var chatAberto = $('#divChatWindow').is(':visible');

            // Só mostra a bolinha se tiver msg nova E o chat estiver FECHADO
            if (temNotificacao === "S" && !chatAberto) {
                $('#chatNotification').show();
            } else {
                $('#chatNotification').hide();
            }
        }


        // Função global de inicialização (chama tudo que precisa rodar no load)
        function pageLoad(sender, args) {

            // SE FOR CARREGAMENTO TOTAL (F5 ou Link) -> FORÇA FECHADO
            if (!args.get_isPartialLoad()) {
                $('#divChatWindow').hide();
            }
            // SE FOR UPDATEPANEL -> Mantém como estava, mas se estiver aberto, rola o scroll
            else {
                if ($('#divChatWindow').is(':visible')) {
                    scrollToBottom();
                }
            }

            // Verifica notificação em ambos os casos
            verificarNotificacao();

            // Reatribui evento de Enter (pois o UpdatePanel pode remover)
            $('#<%= txtMsgInterna.ClientID %>').off('keypress').on('keypress', function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    enviarMsgChat();
                    return false;
                }
            });

            // Suas funções auxiliares originais
            if (typeof initSwitchOpcoesLogic === 'function') initSwitchOpcoesLogic();
            if (typeof initSwitchLogic === 'function') initSwitchLogic();
            if (typeof calcDiasMain === 'function') setTimeout(calcDiasMain, 200);
        }
        // --- LÓGICA DA MODAL DE AÇÃO ---

        function openActionModal(action) {
            // 1. Armazena a ação
            var hddAction = document.getElementById('<%= hddSelectedAction.ClientID %>');
            if (hddAction) hddAction.value = action;

            // 2. Busca containers
            const divDataAlterar = document.getElementById('<%= div_DataAlterar.ClientID %>');
            const divMotivo = document.getElementById('<%= div_Motivo.ClientID %>');

            // 3. Lógica de exibição
            if (divDataAlterar) {
                const actionsWithDateChange = ['AprovarSupervisor', 'RejeitarSupervisor', 'AprovarRH', 'RejeitarRH', 'AprovarDiretor', 'RejeitarDiretor'];

                if (actionsWithDateChange.includes(action)) {
                    divDataAlterar.style.display = 'block';
                    initSwitchLogic(); // Inicia o switch
                } else {
                    divDataAlterar.style.display = 'none';
                    if (divMotivo) divMotivo.style.display = 'block';
                }
            }

            // 4. Calcula dias (caso já tenha data preenchida)
            setTimeout(calcDiasModal, 100);

            // 5. Abre modal
            $('#myModal').modal('show');
            return false;
        }

        // --- LÓGICA DOS SWITCHES E DATAS ---

        function toggleDivs(isChecked) {
            const divDatas = document.getElementById('<%= div_Datas.ClientID %>');
            const divMotivo = document.getElementById('<%= div_Motivo.ClientID %>');
            const hfAlterarData = document.getElementById('<%= hddAlterarData.ClientID %>');

            if (divDatas && divMotivo && hfAlterarData) {
                divDatas.style.display = isChecked ? 'block' : 'none';
                divMotivo.style.display = isChecked ? 'none' : 'block';
                hfAlterarData.value = isChecked ? 'S' : 'N';
            }
        }

        function onSwitchChange() {
            toggleDivs(this.checked);
        }

        function initSwitchLogic() {
            const switchInput = document.getElementById('SwitchAtivo_idSwitch');
            if (!switchInput) return;

            // Remove listener antigo para evitar duplicação
            switchInput.removeEventListener('change', onSwitchChange);
            switchInput.addEventListener('change', onSwitchChange);

            // Estado inicial
            toggleDivs(switchInput.checked);
        }

        function initSwitchOpcoesLogic() {
            const switchOpcoes = document.getElementById('SwitchOpcoesDatas_idSwitch');
            const divLista = document.getElementById('div_ListaOpcoesDatas');

            if (!switchOpcoes || !divLista) return;

            const toggleLista = () => {
                if (switchOpcoes.checked) $(divLista).slideDown();
                else $(divLista).slideUp();
            };

            switchOpcoes.removeEventListener('change', toggleLista);
            switchOpcoes.addEventListener('change', toggleLista);
            toggleLista();
        }

        function selecionarDataOpcao(cardDiv, dtInicio, dtFim) {
            // Marca visualmente
            const radio = cardDiv.querySelector('input[type="radio"]');
            if (radio) radio.checked = true;

            // Preenche os inputs da modal
            const txtIni = document.getElementById('<%= txtdtInicialSugerida.ClientID %>');
            const txtFim = document.getElementById('<%= txtdtFinalSugerida.ClientID %>');

            if (txtIni) txtIni.value = dtInicio;
            if (txtFim) txtFim.value = dtFim;

            // Estilo dos cards
            const todosPaineis = document.querySelectorAll('#div_ListaOpcoesDatas .panel');
            todosPaineis.forEach(p => {
                p.classList.remove('panel-primary');
                p.classList.add('panel-info');
            });
            cardDiv.classList.remove('panel-info');
            cardDiv.classList.add('panel-primary');

            // Recalcula
            calcDiasModal();
        }

        // --- CÁLCULO DE DIAS ---

        function calcularDiferencaDias(idIni, idFim, idLabel, idDivContainer) {
            var elIni = document.getElementById(idIni);
            var elFim = document.getElementById(idFim);
            var elLabel = document.getElementById(idLabel);
            var elDiv = document.getElementById(idDivContainer);

            if (elIni && elFim && elLabel && elDiv) {
                if (elIni.value && elFim.value) {
                    var dt1 = new Date(elIni.value + 'T00:00:00');
                    var dt2 = new Date(elFim.value + 'T00:00:00');
                    var diffTime = dt2 - dt1;
                    var diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
                    var totalDias = diffDays + 1;

                    if (totalDias > 0) {
                        elLabel.innerText = totalDias;
                        $(elDiv).fadeIn();
                    } else {
                        $(elDiv).hide();
                    }
                } else {
                    $(elDiv).hide();
                }
            }
        }

        function calcDiasMain() {
            // Verifica se existe o campo final (só existe se for Férias)
            var elem = document.getElementById('<%= txtsdtFinal.ClientID %>');
            if (elem) {
                calcularDiferencaDias('<%= txtsdtInicio.ClientID %>', '<%= txtsdtFinal.ClientID %>', 'lblDiasMain', 'divCalcDiasMain');
            }
        }

        function calcDiasModal() {
            calcularDiferencaDias('<%= txtdtInicialSugerida.ClientID %>', '<%= txtdtFinalSugerida.ClientID %>', 'lblDiasModal', 'divCalcDiasModal');
        }

        // --- LOADER ARQUIVOS ---

        function iniciarLoaderArquivos() {
            var loader = document.getElementById('loadingMaskArquivo');
            if (loader) {
                loader.style.display = 'flex';
                $(loader).show();
                setTimeout(function () { $(loader).fadeOut('slow'); }, 5000);
            }
        }


        // ==============================================================
        // 2. INICIALIZAÇÃO E EVENTOS (DOCUMENT READY)
        // ==============================================================

        $(document).ready(function () {

            // 1. Configura Enter no Chat
            // Usamos 'on' no document para garantir que funcione mesmo após UpdatePanel
            $(document).on('keypress', '#<%= txtMsgInterna.ClientID %>', function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    enviarMsgChat();
                    return false;
                }
            });

            // 2. Listener das Abas (para o loader)
            $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                var target = $(e.target).attr("href");
                if (target === '#arquivos') {
                    iniciarLoaderArquivos();
                }
            });

            // 3. Inicializa Switches e Cálculos
            initSwitchOpcoesLogic();
            setTimeout(calcDiasMain, 200);
        });


        // ==============================================================
        // 3. GERENCIADOR DE POSTBACK (Sys.WebForms)
        // ==============================================================

        // Garante que o Sys existe antes de usar
        if (typeof Sys !== 'undefined') {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            var postBackElementId = '';

            // 1. Captura quem disparou ANTES do envio
            prm.add_beginRequest(function (sender, args) {
                var elem = args.get_postBackElement();
                if (elem) {
                    postBackElementId = elem.id;

                    // --- NOVO: Lógica de Load no Chat ---
                    // Se o botão clicado for o de enviar mensagem do chat
                    if (postBackElementId.indexOf("btnEnviarReal") > -1) {
                        // Mostra o loader APENAS dentro do chat
                        $('#chatLoader').fadeIn('fast');
                    }
                    // ------------------------------------
                }
            });

            // 2. Executa lógica APÓS o retorno
            prm.add_endRequest(function (sender, args) {

                // Esconde o loader do chat sempre que o processo terminar
                $('#chatLoader').hide();

                // A. Tratamento de Erro
                if (args.get_error()) {
                    console.error("Erro no UpdatePanel:", args.get_error().message);
                    return;
                }

                // B. Mantém o Chat aberto e rola para baixo
                if (postBackElementId && postBackElementId.indexOf("btnEnviarReal") > -1) {
                    var chat = $('#divChatWindow');
                    chat.show();
                    scrollToBottom();

                    var txt = document.getElementById('<%= txtMsgInterna.ClientID %>');
                    if (txt) txt.focus();
                }

                // C. Re-inicializa componentes visuais
                initSwitchLogic();
                initSwitchOpcoesLogic();
                setTimeout(calcDiasMain, 200);

                // Se estiver na aba arquivos, roda o loader de arquivos
                if ($('#arquivos').hasClass('active') && postBackElementId.indexOf("btnEnviarReal") === -1) {
                    iniciarLoaderArquivos();
                }

                // Limpa a variável
                postBackElementId = '';
            });
        }


        // --- LÓGICA DO CHAT COM WEBMETHOD ---

        function enviarMsgChat() {
            // 1. Pegar valores
            var txtInput = $('#<%= txtMsgInterna.ClientID %>');
            var msg = txtInput.val();
            var idSol = $('#<%= hddidSolicitacao.ClientID %>').val(); // Pegamos do HiddenField

            if (!msg || msg.trim() === "") return;

            // 2. Mostrar loader SÓ do chat (manual)
            $('#chatLoader').show();

            // 3. Chamada AJAX para o WebMethod
            $.ajax({
                type: "POST",
                url: "Solicitacao_Detalhe.aspx/EnviarMensagemChat_WebMethod",
                data: JSON.stringify({ idSolicitacao: idSol, mensagem: msg }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    // O retorno vem dentro de response.d
                    var resultado = response.d;

                    if (resultado.sucesso) {
                        // 4. Sucesso: Adiciona o balão visualmente sem recarregar nada
                        adicionarBalaoNaTela(resultado.mensagem, resultado.dataHora, resultado.usuario);

                        // Limpa o campo
                        txtInput.val('').focus();
                        scrollToBottom();
                    } else {
                        alert('Erro ao enviar: ' + resultado.erro);
                    }
                },
                error: function (xhr, status, error) {
                    console.log(xhr.responseText);
                    alert('Erro de comunicação com o servidor.');
                },
                complete: function () {
                    // 5. Esconde loader do chat
                    $('#chatLoader').hide();
                }
            });
        }

        function adicionarBalaoNaTela(msg, data, usuario) {
            // HTML template do balão "msg-me" (minha mensagem)
            // Note que estou escapando o texto para evitar XSS básico
            var safeMsg = $('<div>').text(msg).html();

            var html = `
            <div class="msg-container">
                <div class="msg-bubble msg-me" style="display:none;"> 
                    <div style="white-space: pre-wrap;">${safeMsg}</div>
                    <span class="msg-info">
                        ${usuario} - ${data}
                    </span>
                </div>
            </div>`;

            var $novoBalao = $(html);

            // Remove a div de "Nenhuma mensagem" se ela existir
            var divSemMsg = document.getElementById('<%= divSemMsg.ClientID %>');
            if (divSemMsg) divSemMsg.style.display = 'none';

            // Adiciona ao repeater (container)
            var container = $('#chatBodyScroll');

            // Se for repeater, ele pode não ter uma div container limpa, 
            // então adicionamos ao final da div chatBodyScroll
            container.append($novoBalao);

            // Efeito de fade in bonito
            $novoBalao.find('.msg-bubble').fadeIn();
        }
    </script>


</asp:Content>
