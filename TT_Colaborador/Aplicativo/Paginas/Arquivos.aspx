<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Arquivos.aspx.cs" Inherits="TT_Colaborador.Aplicativo.Paginas.Arquivos" %>

<%@ Register Src="~/Aplicativo/Controles/MensagemPagina.ascx" TagPrefix="uc2" TagName="MensagemPagina" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" />
    <link href="https://cdn.jsdelivr.net/npm/summernote@0.8.18/dist/summernote-bs4.min.css" rel="stylesheet" />
    <link href="/aplicativo/css/font-awesome/css/font-awesome.min.css" rel="stylesheet" />
    <link href="/aplicativo/css/bootdey-template.css" rel="stylesheet" />
    <link href="https://login.tecandtec.com.br/app/css/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="https://cdn.datatables.net/1.12.1/css/dataTables.bootstrap5.min.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/chosen/1.8.7/chosen.min.css" />

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script type="text/javascript" src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script type="text/javascript" src="https://code.jquery.com/ui/1.13.3/jquery-ui.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/summernote@0.8.18/dist/summernote-bs4.min.js"></script>
    <script src="https://cdn.datatables.net/1.12.1/js/jquery.dataTables.min.js"></script>
    <script src="https://cdn.datatables.net/1.12.1/js/dataTables.bootstrap5.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/chosen/1.8.7/chosen.jquery.min.js"></script>
    <script> $ = jQuery.noConflict();</script>

    <script>
        $(document).ready(function () {
            $('#gv_Arquivo').DataTable({
                "pageLength": 100,
                order: [[0, 'desc']],
                language: {
                    url: 'https://cdn.datatables.net/plug-ins/1.11.5/i18n/pt-BR.json',
                },
                "LengthChange": false
            });
        });
    </script>
    <style>
        body {
            background: none;
            padding: 0;
        }

        .form-group {
            margin-bottom: 15px;
        }

        .form-control, form-select {
            border-color: #ccc;
        }

            .form-control[readonly] {
                background-color: #eee;
                border-color: #ccc;
            }

        .col-lg-1, .col-lg-2, .col-lg-3, .col-lg-4, .col-lg-5, .col-lg-6,
        .col-lg-7, .col-lg-8, .col-lg-9, .col-lg-10, .col-lg-11, .col-lg-12 {
            padding: 0 20px 10px 20px;
        }

        /*----------------------------------------------------------------*/

        /* Estilos para Dispositivos Móveis */

        @media only screen and (max-device-width: 768px) {

            .menu-colaborador {
                width: 45vw !important;
                height: 100% !important;
            }

            .font-13,
            .ribbon,
            h6 {
                font-size: 1.5rem !important;
            }

            .form-control,
            .alert,
            .btn,
            input,
            label b,
            b label {
                font-size: 1.75rem !important;
            }

            h2 {
                font-size: 2.5rem !important;
            }

            h3 {
                font-size: 2.15rem !important;
            }

            table {
                font-size: x-large;
            }
        }

        /* --- CSS DO LOADING --- */
        #loadingOverlay {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(255, 255, 255, 0.85); /* Fundo branco transparente */
            z-index: 9999;
            display: none; /* Oculto por padrão */
            align-items: center;
            justify-content: center;
            flex-direction: column;
        }
        
        .spinner-custom {
            width: 3rem;
            height: 3rem;
            border-width: 0.25em;
        }
    </style>
</head>

<body>
    <form id="frmArquivos" runat="server">

        <div id="loadingOverlay">
            <div class="spinner-border text-primary spinner-custom" role="status">
                <span class="visually-hidden">Carregando...</span>
            </div>
            <h4 style="margin-top: 15px; color: #555; font-weight: 300;">Processando, aguarde...</h4>
        </div>

        <div id="DIV3" runat="server">
            <div class="panel panel-default" runat="server">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Arquivos</b></h3>
                </div>

                <div class="panel-body">
                    <div class="form-stacked row">
                        <div class="col-lg-12 table-responsive">

    <asp:GridView ID="gv_Arquivo" 
        runat="server" 
        Width="100%" 
        CellSpacing="1" 
        CellPadding="1" 
        AutoGenerateColumns="False" 
        GridLines="None"
        ShowFooter="False" 
        DataKeyNames="idArquivo" 
        
        ClientIDMode="Static" 
        OnPreRender="gv_Arquivo_PreRender"
        
        OnRowCommand="gv_Arquivo_RowCommand" 
        OnRowDataBound="gv_Arquivo_RowDataBound"
        ShowHeaderWhenEmpty="true"
        CssClass="table table-striped table-bordered table-hover table-condensed">
        
        <Columns>
            <%-- Suas colunas aqui (mantive igual) --%>
            <asp:BoundField DataField="idArquivo" HeaderText="ID">
                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
            </asp:BoundField>

            <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Inclusão">
                <ItemStyle Width="15%" HorizontalAlign="left" VerticalAlign="Middle" />
            </asp:BoundField>

            <asp:ButtonField DataTextField="sNomeArquivo" HeaderText="Nome do Arquivo" CommandName="Download" ItemStyle-Width="30%" />

            <asp:BoundField DataField="sDscTipoArquivo" HeaderText="Tipo">
                <ItemStyle Width="15%" HorizontalAlign="left" VerticalAlign="Middle" />
            </asp:BoundField>

            <asp:BoundField DataField="sDscArquivo" HeaderText="Descrição">
                <ItemStyle Width="25%" HorizontalAlign="left" VerticalAlign="Middle" />
            </asp:BoundField>

            <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Usuário">
                <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
            </asp:BoundField>

     <%--       <asp:TemplateField HeaderText="Deletar" Visible="false">
                <ItemTemplate>
                    <asp:LinkButton ID="lnkDocumento_Excluir" runat="server" CssClass="btn btn-sm btn-danger ExcluirArquivo" TabIndex="100">
                 deletar
                    </asp:LinkButton>
                </ItemTemplate>
                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
            </asp:TemplateField>--%>
        </Columns>

        <%-- Isso garante que algo apareça se não houver dados --%>
        <EmptyDataTemplate>
            <div class="alert alert-warning text-center" style="margin-top: 10px;">
                <i class="fa fa-exclamation-triangle"></i> Nenhum arquivo encontrado.
            </div>
        </EmptyDataTemplate>

    </asp:GridView>

</div>
                    </div>
                    <div id="div_EnviarArquivos" class="form-stacked " runat="server">
                        <div class="panel panel-default" runat="server">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Enviar Arquivos</b></h3>
                            </div>
                            <uc2:MensagemPagina runat="server" ID="MensagemPagina" />

                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-lg-12" style="padding: 0;">
                                        <div class="col-lg-6">
                                            <div class="row">
                                                <div class="form-stacked row">
                                                    <div class="col-lg-12">
                                                        <div class="form-group">
                                                            <b><label>Tipo</label></b>
                                                            <asp:DropDownList ID="ddlidTipoArquivo" runat="server" class="form-control yes_no select CaixaTextoGrande" ClientIDMode="Static"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-6" id="div4" runat="server">
                                                        <div class="form-group">
                                                            <b><label>Selecione o Arquivo</label></b>
                                                            <asp:FileUpload ID="fu_Arquivo" class="form-control-file" runat="server" />
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-12">
                                                        <div class="form-group">
                                                            <b><label><asp:Label ID="lblEnviarArquivo_sDscArquivo" runat="server" Text="Descrição"></asp:Label></label></b>
                                                            <asp:TextBox ID="txtEnviarArquivo_sDscArquivo" class="form-control CaixaTextoGigante" runat="server" MaxLength="300"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-12">
                                                        <div class="form-group">
                                                            <asp:Button ID="cmdEnviarArquivos" CssClass="btn-success btn" runat="server" Text="Enviar" OnClick="cmdEnviarArquivos_Click" OnClientClick="return ShowLoader();" />
                                                        </div>
                                                    </div>
                                                </div>

                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <asp:Image ID="img_Produto" runat="server" class="img-responsive pull-right" ImageUrl="~/App/img/photo-camera.png" Height="300" Width="300" Visible="false" Style="display: none" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>

        <div id="dialog_Apagar" class="modal" title="Excluir Arquivo">
            <p>
                <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                <asp:Label ID="LM_lblTituloDialog" runat="server" Text="Deseja Excluir o Arquivo?"></asp:Label>
            </p>
        </div>

        <asp:HiddenField ID="hddidObjeto" runat="server" />
        <asp:HiddenField ID="hddsTipoObjeto" runat="server" />
        <asp:HiddenField ID="hddExcluirArquivo" runat="server" />

    </form>
    <div id="dialog" title="Adicione ou Altere a Imagem do Produto"></div>

    <script>
        // Função para mostrar o loading
        function ShowLoader() {
            var ddl = document.getElementById('ddlidTipoArquivo');
            var upload = document.getElementById('fu_Arquivo');

            $('#loadingOverlay').css('display', 'flex');

            return true;
        }

        $(document).ready(function () {
            var tabela = $('#gv_Arquivo');

            if (tabela.length > 0) {
                tabela.DataTable({
                    "destroy": true, // <--- ISSO CORRIGE O ERRO "Cannot reinitialise"
                    "pageLength": 100,
                    "order": [[0, 'desc']],
                    "language": {
                        url: 'https://cdn.datatables.net/plug-ins/1.11.5/i18n/pt-BR.json',
                    },
                    "LengthChange": false
                });
            }
        });
    </script>
</body>
</html>
