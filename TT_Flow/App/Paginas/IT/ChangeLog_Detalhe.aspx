<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="ChangeLog_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.IT.ChangeLog_Detalhe" ValidateRequestMode="Disabled" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/UploadArquivo_Padrao.ascx" TagPrefix="uc1" TagName="UploadArquivo_Padrao" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HtmlEditor" TagPrefix="HTMLEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        .embed-responsive-16by9 {
            padding-bottom: 200px;
        }

        .editor {
            padding: 3px;
            background-color: white;
            border: solid 1px;
            border-color: lightgray;
            border-radius: 8px;
        }

            .editor table tbody tr td div {
                border-top: solid 1px;
                border-color: lightgray;
                margin: 2px 0px 2px 0px;
            }

                .editor table tbody tr td div #cphCorpo_txtsDscAlteracao_ctl01_FixedForeColor {
                    margin: 0px;
                    border: none;
                }

                .editor table tbody tr td div #cphCorpo_txtsDscAlteracao_ctl01_FixedBackColor {
                    margin: 0px;
                    border: none;
                }

                .editor table tbody tr td div #cphCorpo_txtsDscDocumentacao_ctl01_FixedForeColor {
                    margin: 0px;
                    border: none;
                }

                .editor table tbody tr td div #cphCorpo_txtsDscDocumentacao_ctl01_FixedBackColor {
                    margin: 0px;
                    border: none;
                }

            .editor table tbody tr td #cphCorpo_txtsDscAlteracao_ctl01 {
                width: 100%;
                max-height: 25px;
                border: none;
            }

            .editor table tbody tr td #cphCorpo_txtsDscDocumentacao_ctl01 {
                width: 100%;
                max-height: 25px;
                border: none;
            }

            .editor table tbody tr td #cphCorpo_txtsDscAlteracao_ctl03 {
                padding: 5px;
            }

            .editor table tbody tr td #cphCorpo_txtsDscDocumentacao_ctl03 {
                padding: 5px;
            }

        .modal-dialog {
            width: 60%;
        }

        .modal-content {
            display: flex;
            flex-direction: column;
            height: auto;
        }

        .modal-body {
            max-height: 850px;
            flex: 1 1 auto;
            overflow-y: visible;
        }

        .modal-largo {
            width: 80% !important;
            max-width: none !important;
        }

        .modal-header-content {
            display: flex;
            align-items: center;
        }

        .modal-logo img {
            max-width: 50px;
            margin-right: 10px;
        }

        .modal-title-container {
            flex-grow: 1;
        }

        .modal-title {
            color: #009A22;
            text-shadow: 2px 2px 2px rgba(0, 0, 0, 0.2);
            font-size: 24px;
            font-weight: bold;
            margin: 0;
        }

        .upload-container {
            width: fit-content;
            height: fit-content;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto;
        }

        .file-upload-label input {
            display: none;
        }

        .file-upload-label svg {
            height: 50px;
            fill: rgb(82, 82, 82);
            margin-bottom: 20px;
        }

        .file-upload-label {
            cursor: pointer;
            background-color: #ddd;
            padding: 30px 70px;
            border-radius: 40px;
            border: 2px dashed rgb(82, 82, 82);
            box-shadow: 0px 0px 20px -50px rgba(0, 0, 0, 0.719);
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            gap: 5px;
            text-align: center;
        }

            .file-upload-label.dragover {
                border-color: #000;
                background-color: #e6f7ff;
            }

        .browse-button {
            background-color: rgb(82, 82, 82);
            padding: 5px 15px;
            border-radius: 10px;
            color: white;
            transition: all 0.3s;
        }

            .browse-button:hover {
                background-color: rgb(14, 14, 14);
            }

        .file-name {
            margin-top: 10px;
            font-size: 14px;
            color: rgb(82, 82, 82);
        }

        .id {
            display: none;
        }
    </style>

    <div id="hdd">
        <asp:HiddenField ID="hddidItem" runat="server" />
        <asp:HiddenField ID="hddUsuario" runat="server" Value="0" />
        <asp:HiddenField ID="hddidStatus" runat="server" Value="1" />
        <asp:HiddenField ID="hddAlteracao" runat="server" />
        <asp:HiddenField ID="hddDocumentacao" runat="server" />
        <asp:HiddenField ID="hddTeste" runat="server" />
        <asp:HiddenField ID="hddTreinamento" runat="server" />
    </div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="3" TitulodaPagina="Detalhe" />
        </div>
    </div>

    <asp:UpdatePanel runat="server" ID="UpdAbas" UpdateMode="Conditional">
        <ContentTemplate>

            <div id="abas">
                <ul id="tab_Principal" class="nav nav-tabs" role="tablist">
                    <li role="presentation" class="tabpanel active" id="aba_Principal">
                        <a href="#principal" id="aba-principal" role="tab" data-toggle="tab" aria-controls="principal" aria-expanded="false"><b>Change log</b></a>
                    </li>
                    <li role="presentation" runat="server" id="aba_documentacao">
                        <a href="#documentacao" role="tab" id="aba_documentacao-tab" data-toggle="tab" aria-controls="documentacao"><b>Documentação</b></a>
                    </li>
                    <li role="presentation" runat="server" id="aba_Arquivos">
                        <a href="#arquivos" role="tab" id="aba_arquivos-tab" data-toggle="tab" aria-controls="arquivos"><b>Arquivos</b></a>
                    </li>
                    <li role="presentation" runat="server" id="aba_videos">
                        <a href="#videos" role="tab" id="aba_videos-tab" data-toggle="tab" aria-controls="videos"><b>Vídeos</b></a>
                    </li>
                    <li role="presentation" runat="server" id="aba_Testes">
                        <a href="#testes" role="tab" id="aba_testes-tab" data-toggle="tab" aria-controls="testes"><b>Teste</b></a>
                    </li>
                    <li role="presentation" runat="server" id="aba_Treinamento">
                        <a href="#Treinamento" role="tab" id="aba_Treinamento-tab" data-toggle="tab" aria-controls="Treinamento"><b>Treinamento</b></a>
                    </li>
                    <li role="presentation" runat="server" id="aba_Historico">
                        <a href="#historico" role="tab" id="aba_historico-tab" data-toggle="tab" aria-controls="historico"><b>Histórico</b></a>
                    </li>
                </ul>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <br />

    <div id="tab" class="tab-content">

        <div role="tabpanel" class="tab-pane fade in active" id="principal" aria-labelledby="aba_Principal">

            <asp:UpdatePanel runat="server" ID="UpdPrincipal" UpdateMode="Conditional">
                <ContentTemplate>

                    <div class="form-stacked row">
                        <div class="col-lg-12">
                            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                        </div>

                        <div class="col-lg-12">
                            <div class="form-group col-lg-1 row">
                                <label>ID</label>
                                <div class="form-group input-group CaixaTextoPequeno">
                                    <asp:LinkButton ID="cmdRetornar" class="btn btn-sm btn-info input-group-addon colorLink" runat="server" OnClick="cmdRetornar_click"><b><</b></asp:LinkButton>
                                    <asp:TextBox ID="txtidItem" class="form-control" runat="server" disabled="0"></asp:TextBox>
                                    <asp:LinkButton ID="cmdAvancar" class="btn btn-sm btn-info input-group-addon colorLink" runat="server" OnClick="cmdAvancar_click"><b>></b></asp:LinkButton>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-12">
                            <div class="form-group row" runat="server" id="divStatus">
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Status Atual</label>
                                        <asp:TextBox ID="txtStatus" runat="server" class="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-12">
                            <div class="form-group row" runat="server" id="divUsuarioTeste">
                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <label>Usuário de Teste</label>
                                        <asp:DropDownList ID="ddlidUsuarioTeste" runat="server" class="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-12">
                            <div class="form-group" runat="server" id="divVersao">
                                <label>Versão</label>
                                <asp:TextBox ID="txtsCodigoVersao" runat="server" class="form-control CaixaTextoPequeno" MaxLength="10" disabled=""></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-12">
                            <div class="form-group row">
                                <div class="col-lg-4" style="padding-right: 0;">
                                    <div class="form-group">
                                        <label>Tipo da Alteração</label>
                                        <asp:DropDownList ID="ddlTipoAlteracao" runat="server" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlTipoAlteracao_SelectedIndexChanged">
                                            <asp:ListItem Value="0">Selecione tipo da Atualização</asp:ListItem>
                                            <asp:ListItem Value="3">Breaking Change</asp:ListItem>
                                            <asp:ListItem Value="2">Feat</asp:ListItem>
                                            <asp:ListItem Value="4">Small Change</asp:ListItem>
                                            <asp:ListItem Value="1">Fix</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-1" style="padding: 0;">
                                    <label style="width: 100%;">&nbsp;</label>
                                    <asp:LinkButton runat="server" ID="cmdAlterna_Fix_x_SmallChange" class="btn btn-link CaixaTextoMicro" OnClick="cmdAlterna_Fix_x_SmallChange_Click" data-toggle="tooltip" title="Alternar <br />Fix e Small Change"><i class="fa fa-exchange"></i></asp:LinkButton>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-12">
                            <div class="form-group row">
                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Título da alteração</label>
                                        <asp:TextBox ID="txtsDscTituloAlteracao" runat="server" class="form-control" MaxLength="50"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-12">
                            <div class="form-group row">
                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Possui documentação</label>
                                        <asp:DropDownList ID="ddlDocumentacao" class="form-control" runat="server" OnSelectedIndexChanged="ddlDocumentacao_SelectedIndexChanged" AutoPostBack="true">
                                            <asp:ListItem Value="S">SIM</asp:ListItem>
                                            <asp:ListItem Value="N">NÃO </asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>

                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlTipoAlteracao" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="ddlDocumentacao" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="cmdAlterna_Fix_x_SmallChange" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>

            <div class="form-stacked row">

                <div class="col-lg-12">
                    <div class="form-group" runat="server" id="divAlteracao">
                        <label>Descrição</label>
                          <textarea runat="server" id="txtsDscAlteracao" class="htmlEditor" ></textarea> 
                        <%--<HTMLEditor:Editor runat="server" Height="800px" Width="100%" ID="txtsDscAlteracao" HtmlPanelCssClass="editor" CssClass="editor" DocumentCssPath="App\css\HtmlEditor.css" />--%>
                    </div>
                </div>

                <asp:UpdatePanel runat="server" ID="UpdLocalArquivo" UpdateMode="Conditional">
                    <ContentTemplate>

                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>Local do Arquivo</label>
                                <asp:TextBox ID="txtsLocalArquivo" runat="server" class="form-control" ToolTip="Local onde está os arquivos de atualização" MaxLength="200"></asp:TextBox>
                            </div>
                        </div>

                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>

        </div>

        <div role="tabpanel" class="tab-pane fade" id="documentacao" aria-labelledby="aba_documentacao">
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Descrição</label>
                        <textarea runat="server" id="txtsDscDocumentacao" class="htmlEditor" ></textarea> 
                        <%--<HTMLEditor:Editor runat="server" Height="1200px" Width="100%" ID="txtsDscDocumentacao" HtmlPanelCssClass="editor" CssClass="editor" DocumentCssPath="App\css\HtmlEditor.css" />--%>
                    </div>
                </div>
            </div>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="arquivos" aria-labelledby="aba_arquivos">
            <div class=" embed-responsive embed-responsive-16by9" style="min-height: 800px">
                <embed type="text/html" runat="server" id="eArquivos" width="800" height="500" />
            </div>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="videos" aria-labelledby="aba_videos">
            <div class="form-stacked row">

                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Videos" />
                </div>

                <div class="col-lg-12">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Vídeos</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">

                                <div runat="server" id="div_gvVideos" class="col-lg-12 form-group" style="padding: 0;">

                                    <div class="col-lg-12 table-responsive">
                                        <asp:GridView ID="gvVideos" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" DataKeyNames="idArquivo">
                                            <Columns>

                                                <asp:BoundField DataField="idArquivo" HeaderText="">
                                                    <ItemStyle Width="0%" HorizontalAlign="left" VerticalAlign="Middle" CssClass="id idArquivo" />
                                                    <HeaderStyle CssClass="id" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Nome do Arquivo">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkAbrir_Video" ClientIDMode="Static" runat="server" CssClass="Download" data-obs='<%# Eval("sObservacao") %>' Text='<%# Eval("sNomeArquivo") %>'></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="70%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Usuário">
                                                    <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Inclusão">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkDocumento_Download" runat="server" CssClass="btn btn-small DownloadArquivo" TabIndex="100" ToolTip='<%# Bind("sNomeArquivo", "Download do Arquivo: {0}") %>'><i class="fa-file fa"></i></asp:LinkButton>
                                                        <asp:LinkButton ID="lnkDocumento_Excluir" runat="server" CssClass="btn btn-small ExcluirArquivo" TabIndex="100"><i class="fa fa-eraser"></i></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>
                                    </div>

                                </div>

                                <div class="col-lg-12">
                                    <fieldset class="col-lg-12 form-stacked actions">
                                        <asp:Button ID="cmdUpload" class="btn btn-lg btn-success" runat="server" Text="Subir Arquivo" />
                                    </fieldset>
                                </div>

                                <div class="modal fade" id="modalUploadVideo" tabindex="-2" role="dialog" data-backdrop="static" aria-labelledby="modalUploadVideo" aria-hidden="true">
                                    <div class="modal-dialog modal-sm" role="document" style="width: 35%">
                                        <div class="modal-content">
                                            <div class="modal-header">
                                                <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                                                    <span aria-hidden="true">&times;</span>
                                                </button>
                                                <div class="modal-header-content">
                                                    <div class="modal-title-container">
                                                        <h4 class="modal-title">Upload de Vídeos</h4>
                                                    </div>
                                                </div>
                                                <div class="modal-body">
                                                    <div class="panel panel-default">

                                                        <div class="panel-body ">
                                                            <div class="col-lg-12 row" style="width: 100%; margin-left: 0px; padding: 0px;">
                                                                <uc1:MensagemPagina runat="server" ID="MensagemPagina_ModalVideos" style="margin-left: 0 !important" />
                                                            </div>

                                                            <div class="row">
                                                                <div class="col-lg-12">
                                                                    <div class="form-stacked">
                                                                        <div class="col-lg-12" id="div1" runat="server" style="text-align: center;">
                                                                            <div class="form-group" style="display: inline-block">
                                                                                <div class="upload-container">
                                                                                    <div class="file-upload-label" id="uploadContainer">
                                                                                        <svg viewBox="0 0 640 512" height="1em">
                                                                                            <path
                                                                                                d="M144 480C64.5 480 0 415.5 0 336c0-62.8 40.2-116.2 96.2-135.9c-.1-2.7-.2-5.4-.2-8.1c0-88.4 71.6-160 160-160c59.3 0 111 32.2 138.7 80.2C409.9 102 428.3 96 448 96c53 0 96 43 96 96c0 12.2-2.3 23.8-6.4 34.6C596 238.4 640 290.1 640 352c0 70.7-57.3 128-128 128H144zm79-217c-9.4 9.4-9.4 24.6 0 33.9s24.6 9.4 33.9 0l39-39V392c0 13.3 10.7 24 24 24s24-10.7 24-24V257.9l39 39c9.4 9.4 24.6 9.4 33.9 0s9.4-24.6 0-33.9l-80-80c-9.4-9.4-24.6-9.4-33.9 0l-80 80z">
                                                                                            </path>
                                                                                        </svg>
                                                                                        <p>Arraste e solte aqui o Arquivo</p>
                                                                                        <p>ou</p>
                                                                                        <span class="browse-button">Escolher Arquivo</span>
                                                                                        <asp:FileUpload ID="fu_EnviarArquivo" runat="server" Style="display: none;" accept=".mp4" />
                                                                                    </div>
                                                                                </div>
                                                                                <div class="file-name" id="fileName"></div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                            <div class="modal-footer" style="text-align: left;">
                                                                <div class="form-group">
                                                                    <div class="col-lg-12">
                                                                        <div class="row">
                                                                            <div class="col-lg-6" style="padding: 0px;">
                                                                                <div class="form-group">
                                                                                    <asp:Button ID="cmdEnviarArquivos" CssClass="btn btn-md btn-success" runat="server" Text="Enviar" OnClick="cmdEnviarArquivos_Click" />
                                                                                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Fechar</button>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="modal fade" id="modalVideoPlayer" tabindex="-2" role="dialog" aria-labelledby="modalVideoPlayer">
                                    <div class="modal-dialog modal-sm" role="document" style="width: 70%">
                                        <div class="modal-content">

                                            <div class="modal-header">
                                                <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                                                    <span aria-hidden="true">&times;</span>
                                                </button>
                                                <div class="modal-header-content">
                                                    <div class="modal-title-container">
                                                        <h4 class="modal-title">Player de Vídeo</h4>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="modal-body">
                                                <div class="panel panel-default">

                                                    <div class="panel-body ">
                                                        <div class="col-lg-12 row" style="width: 100%; margin-left: 0px; padding: 0px;">
                                                            <uc1:MensagemPagina runat="server" ID="MensagemPagina_ModalVideoPlayer" style="margin-left: 0 !important" />
                                                        </div>

                                                        <div class="col-lg-12 row" style="width: 100%; margin-left: 0px; padding: 0px;">
                                                            <video class="videoPlayer" controls style="width: 100%;"></video>
                                                        </div>

                                                    </div>
                                                </div>
                                            </div>

                                            <div class="modal-footer">
                                                <button type="button" class="btn btn-md btn-dark" data-dismiss="modal">Fechar</button>
                                            </div>

                                        </div>
                                    </div>
                                </div>

                                <div id="dialog_Apagar" class="modal" title="Excluir Arquivo">
                                    <p>
                                        <asp:Label ID="LM_lblTituloDialog" runat="server" Text="Deseja Excluir o Arquivo?"></asp:Label>
                                    </p>
                                </div>

                                <asp:HiddenField ID="hddidObjeto" runat="server" />
                                <asp:HiddenField ID="hddsTipoObjeto" runat="server" />
                                <asp:HiddenField ID="hddExcluirArquivo" runat="server" />
                                <asp:HiddenField ID="hddDownloadArquivo" runat="server" />
                                <asp:HiddenField ID="hddSubTipo" runat="server" />
                                <asp:HiddenField ID="hddChecked" runat="server" />
                                <asp:HiddenField ID="hddArquivo" runat="server" />

                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="testes" aria-labelledby="aba_testes">
            <div class="form-stacked row">

                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Evidências do Teste</label>
                        <textarea runat="server" id="txtsDscTeste" class="htmlEditor" ></textarea> 

                     <%--   <HTMLEditor:Editor runat="server"
                            Height="1200px"
                            Width="100%" ID="txtsDscTeste" HtmlPanelCssClass="editor" CssClass="editor" DocumentCssPath="App\css\HtmlEditor.css" />--%>
                    </div>
                </div>
            </div>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="Treinamento" aria-labelledby="aba_Treinamento">
            <div class="form-stacked row">

                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Evidências do Treinamento</label>
                          <textarea runat="server" id="txtsDscTreinamento" class="htmlEditor" ></textarea> 
                        <%--<HTMLEditor:Editor runat="server"
                            Height="1200px"
                            Width="100%" ID="txtsDscTreinamento" HtmlPanelCssClass="editor" CssClass="editor" DocumentCssPath="App\css\HtmlEditor.css" />--%>
                    </div>
                </div>
            </div>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="historico" aria-labelledby="aba_historico">
            <uc1:MensagemPagina runat="server" ID="MensagemPagina1" />
            <asp:Panel ID="pnResultado" class="" runat="server">
                <div id="resultado">
                    <div class="panel panel-primary">
                        <div class="panel-body">
                            <div class="table-responsive">
                                <asp:GridView ID="dtgvHistorico" class="table table-striped table-bordered table-hover "
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvHistorico_RowDataBound">
                                    <Columns>

                                        <asp:BoundField DataField="idHistorico" HeaderText="ID">
                                            <HeaderStyle HorizontalAlign="left" VerticalAlign="Middle" />
                                            <ItemStyle Width="5%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sStatus" HeaderText="Status">
                                            <HeaderStyle HorizontalAlign="left" VerticalAlign="Middle" />
                                            <ItemStyle Width="15%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sUsuario" HeaderText="Usuário Atualização">
                                            <HeaderStyle HorizontalAlign="left" VerticalAlign="Middle" />
                                            <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDescricao" HeaderText="Descrição">
                                            <HeaderStyle HorizontalAlign="left" VerticalAlign="Middle" />
                                            <ItemStyle Width="60%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="dtHistorico" HeaderText="Data Atualização">
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

    <asp:UpdatePanel runat="server" ID="UpdAcao" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="div_Acao" runat="server">
                <div class="form-stacked row">
                    <div class="col-lg-8">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Observação</b></h3>
                            </div>
                            <div class="panel-body">
                                <div id="div_Observacao" runat="server" class="form-group">
                                    <asp:HiddenField runat="server" ID="hddObservacao" Value="1" />
                                    <div class="col-lg-12">
                                        <label runat="server" id="lblObservacao"></label>
                                        <asp:TextBox runat="server" ID="txtsDscObservacao" class="form-control" TextMode="MultiLine"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-6">
                                    <br />
                                    <fieldset>
                                        <asp:Button ID="cmdAcaoOk" class="btn btn-sm btn-success" runat="server" Text="Ok" OnClick="cmdAcaoOk_Click" />
                                        <asp:Button ID="cmdAcaoCancelar" class="btn btn-sm btn-warning" runat="server" Text="Cancelar" OnClick="cmdAcaoCancelar_Click" />
                                    </fieldset>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <fieldset class="form-stacked actions">
        <asp:Button ID="cmdSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />

        <div class="btn-group dropup">
            <button id="cmdListaAprovar" runat="server" type="button" class="btn btn-lg btn-primary dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Ação  <span class="caret"></span></button>
            <ul class="dropdown-menu">

                <asp:UpdatePanel runat="server" ID="UpdBotoes" UpdateMode="Conditional">
                    <ContentTemplate>
                        <li>
                            <asp:LinkButton ID="cmdAprovar" class="btn btn-lg btn-primary" runat="server" Text="Aprovar" OnClick="cmdAprovar_Click" Style="width: 100%;" /></li>

                        <li role="separator" class="divider" runat="server" id="btnDivisor1"></li>

                        <li>
                            <asp:LinkButton ID="cmdRejeitar" class="btn btn-lg btn-danger" runat="server" Text="Rejeitar" OnClick="cmdRejeitar_Click" Style="width: 100%;" /></li>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <li>
                    <asp:LinkButton ID="cmdAprovarTeste" class="btn btn-lg btn-success" runat="server" Text="Aprovar Teste" OnClick="cmdAprovarTeste_Click" /></li>

                <li role="separator" class="divider" runat="server" id="btnDivisor2"></li>

                <li>
                    <asp:LinkButton ID="cmdPausarTeste" class="btn btn-lg btn-warning" runat="server" Text="Pausar Teste" OnClick="cmdPausarTeste_Click" /></li>

                <li role="separator" class="divider" runat="server" id="btnDivisor3"></li>

                <li>
                    <asp:LinkButton ID="cmdRejeitarTeste" class="btn btn-lg btn-danger" runat="server" Text="Rejeitar Teste" OnClick="cmdRejeitarTeste_Click" /></li>

                <li>
                    <asp:LinkButton ID="cmdEmTeste" class="btn btn-lg btn-primary" runat="server" Text="Em Teste" OnClick="cmdEmTeste_Click" /></li>

                <li>
                    <asp:LinkButton ID="cmdProducao" class="btn btn-lg btn-primary" runat="server" Text="Em Produção" OnClick="cmdProducao_Click" /></li>

                <li>
                    <asp:LinkButton ID="cmdAprovarTreinamento" class="btn btn-lg btn-primary" runat="server" Text="Implantação / Treinamento" OnClick="cmdAprovarTreinamento_Click" /></li>

            </ul>
        </div>

        <input type="submit" name="cancel" class="btn btn-lg btn-warning" id="field_cancel" value="Cancelar" title="Voltar" onclick="history.go(-1)" runat="server" />
    </fieldset>

</asp:Content>
