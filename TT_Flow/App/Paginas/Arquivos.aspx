<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Arquivos.aspx.cs" Inherits="TT_Flow.App.Paginas.Arquivos" %>

<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <script src="https://login.tecandtec.com.br/app/js/jquery-3.2.0.min.js"></script>
    <link rel="stylesheet" href="https://login.tecandtec.com.br/plugins/Chosen/chosen.css">
    <script src="https://login.tecandtec.com.br/plugins/Chosen/docsupport/jquery-3.2.1.min.js" type="text/javascript"></script>
    <script src="https://login.tecandtec.com.br/plugins/Chosen/chosen.jquery.js" type="text/javascript"></script>


    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-multiselect/0.9.13/css/bootstrap-multiselect.css">
    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-multiselect/0.9.13/js/bootstrap-multiselect.js"></script>

    <script src="https://login.tecandtec.com.br/app/js/bootstrap.min.js"></script>
    <script src="https://login.tecandtec.com.br/app/js/sb-admin.js"></script>
    <script src="https://login.tecandtec.com.br/app/js/plugins/morris/morris.js"></script>
    <script src="https://login.tecandtec.com.br/app/js/plugins/morris/raphael-2.1.0.min.js"></script>
    <script src="https://login.tecandtec.com.br/app/js/plugins/jquery.mask.js"></script>

    <script src="https://login.tecandtec.com.br/app/js/plugins/datepicker/bootstrap-datepicker.js"></script>
    <script src="https://login.tecandtec.com.br/app/js/jquery-1.10.0.min.js" type="text/javascript"></script>
    <script src="https://login.tecandtec.com.br/app/js/jquery-1.9.2.min.js" type="text/javascript"></script>
    <script> $v192 = jQuery.noConflict();</script>

    <link href="https://login.tecandtec.com.br/app/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://login.tecandtec.com.br/app/css/sb-admin.css" rel="stylesheet" />
    <link href="https://login.tecandtec.com.br/app/css/TT.css" rel="stylesheet" />
    <link href="https://login.tecandtec.com.br/app/css/plugins/dataTables/datatables.min.css" rel="stylesheet" />
    <link href="https://login.tecandtec.com.br/app/css/plugins/morris/morris-0.4.3.min.css" rel="stylesheet" />
    <link href="https://login.tecandtec.com.br/app/css/plugins/datepicker/css/datepicker.css" rel="stylesheet" />
    <link href="https://login.tecandtec.com.br/app/css/jquery-ui.css" rel="stylesheet" type="text/css" />

    <link href="/app/css/font-awesome/css/font-awesome.min.css" rel="stylesheet" />

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
        /*Thiago Rodrigues - 14 / 06 / 2024*/
        body {
            background: none;
            padding: 0px 0px 0px;
        }
    </style>
</head>
<body>
    <form id="frmArquivos" runat="server">
        <br />

        <div id="DIV3" runat="server">
            <div class="panel panel-default" runat="server">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Arquivos</b></h3>
                </div>
                <div class="col-lg-12">
                    <br />
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina1" />
                </div>
                <div class="panel-body">
                    <div class="form-stacked row">
                        <%------------------ Higor Maestrello 17-06-2024 -------------------------------%>
                        <div class="col-lg-10" runat="server" id="btDownload">
                            <div class="form-group">
                                <asp:Button ID="btDownloadArquivo" CssClass="btn-success btn btDownload" runat="server" Text="Download" />
                                <asp:Button ID="btArquivar" CssClass="btn-info btn ArquivarDoc" runat="server" Text="Arquivar" />
                            </div>
                        </div>

                        <div class="col-lg-2" runat="server" id="div_Ordem">
                            <div class="pull-right">
                                <asp:DropDownList ID="ddlOrdem" runat="server" class="form-control yes_no select" AutoPostBack="true" OnSelectedIndexChanged="ddlOrdem_SelectedIndexChanged">
                                    <asp:ListItem Value="0" Text="Ordenar Arquivos" />
                                    <asp:ListItem Value="1" Text="Data Inclusão" />
                                    <asp:ListItem Value="2" Text="Tipo" />
                                    <asp:ListItem Value="3" Text="Nome do Arquivo" />
                                    <asp:ListItem Value="4" Text="Descrição" />
                                    <asp:ListItem Value="5" Text="Data Expiração" />
                                    <asp:ListItem Value="6" Text="Data Registro" />
                                    <asp:ListItem Value="7" Text="Aceitar Documento" />
                                    <asp:ListItem Value="8" Text="Usuário" />
                                </asp:DropDownList>
                            </div>
                        </div>
                        <%-------------------------------------------------------------------------------------%>
                        <div class="col-lg-12 table-responsive">
                            <asp:GridView ID="gv_Arquivo" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                ShowFooter="False" DataKeyNames="idArquivo, sAceitarDoc" OnRowDataBound="gv_Arquivo_RowDataBound" OnRowDeleting="gv_Arquivo_RowDeleting" OnSorting="gv_Arquivo_Sorting">
                                <Columns>
                                    <%------------------ Higor Maestrello 17-06-2024 -------------------------------%>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <div style="text-align: center;">
                                                <asp:CheckBox ID="chkTarefas_Seleciona_Todos" runat="server" CssClass="Todos" />
                                            </div>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkTarefas_Seleciona" runat="server" CssClass="Individual" />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="2%" />
                                    </asp:TemplateField>
                                    <%-------------------------------------------------------------------------------------%>
                                    <asp:BoundField DataField="sDscCategoria" HeaderText="Categoria">
                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="idArquivo" HeaderText="ID">
                                        <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Inclusão">
                                        <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sDscTipoArquivo" HeaderText="Tipo">
                                        <ItemStyle Width="15%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:ButtonField DataTextField="sNomeArquivo" HeaderText="Nome do Arquivo" ItemStyle-CssClass="Arquivo" CommandName="Download" ItemStyle-Width="18%" />

                                    <asp:BoundField DataField="sDscArquivo" HeaderText="Descrição">
                                        <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <%------------------ Higor Maestrello 17-06-2024 -------------------------------%>
                                    <asp:BoundField DataField="dtExpiracaoDoc" HeaderText="Data Expiração">
                                        <ItemStyle Width="8%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="dtRegistroDoc" HeaderText="Data Registro">
                                        <ItemStyle Width="8%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:TemplateField HeaderText="Aceitar Documento">
                                        <ItemTemplate>
                                            <asp:Label ID="Observacao" runat="server" Text='<%# Bind("sObservacao") %>'></asp:Label>
                                            <asp:Button ID="btnAceitar" runat="server" Text="Aceitar Data Expiração" CommandName="Aceitar" CssClass="AceitarDoc" />
                                        </ItemTemplate>
                                        <ItemStyle Width="12%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:TemplateField>
                                    <%-------------------------------------------------------------------------------------%>
                                    <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Usuário">
                                        <ItemStyle Width="15%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <%--<asp:TemplateField HeaderText="Arquivar Documento">
                                        <ItemTemplate>
                                            <asp:Button ID="btnArquivar" runat="server" Text="Arquivar" CommandName="Arquivar" CssClass="ArquivarDoc" />
                                        </ItemTemplate>
                                        <ItemStyle Width="9%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:TemplateField>--%>

                                    <asp:TemplateField HeaderText="Deletar" Visible="false">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkDocumento_Excluir" runat="server" CssClass="btn btn-small ExcluirArquivo" TabIndex="100"><i class="fa fa-eraser"></i></asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>

                        </div>
                    </div>
                    <div id="div_EnviarArquivos" class="form-stacked " runat="server">
                        <div class="panel panel-default" runat="server">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Enviar Arquivos</b></h3>
                            </div>
                            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <div class="col-lg-6">
                                            <div class="row">
                                                <div class="form-stacked row">
                                                    <div class="col-lg-12">
                                                        <div class="form-group">
                                                            <label>Tipo</label>
                                                            <asp:DropDownList ID="ddlidTipoArquivo" runat="server" class="form-control yes_no select CaixaTextoGrande" ClientIDMode="Static" OnSelectedIndexChanged="ddlidTipoArquivo_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-12" id="div_subtipo" runat="server">
                                                        <div class="form-group">
                                                            <label>Subtipo</label>
                                                            <asp:DropDownList ID="ddlSubtipoArquivo" runat="server" class="form-control yes_no select CaixaTextoGrande" ClientIDMode="Static"></asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-3" id="div_dtExpiracaoDoc" runat="server">
                                                        <div class="form-group">
                                                            <label>
                                                                <asp:Label ID="dtExpiracaoDoc" runat="server" Text="Data Expiração"></asp:Label></label>
                                                            <asp:TextBox ID="txtdtExpiracaoDoc" class="form-control CaixaTextoPequeno" placeholder="Opcional" runat="server" MaxLength="10" data-mask="00/00/0000" type="date"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-9" id="div_dtRegistroDoc" runat="server">
                                                        <div class="form-group">
                                                            <label>
                                                                <asp:Label ID="dtRegistroDoc" runat="server" Text="Data Registro"></asp:Label></label>
                                                            <asp:TextBox ID="txtdtRegistroDoc" class="form-control CaixaTextoPequeno" placeholder="Opcional" runat="server" MaxLength="10" data-mask="00/00/0000" type="date"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6" id="div4" runat="server">
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

        <div id="dialog_Aceitar" class="modal" title="Aceitar Arquivo">
            <p>
                <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                <asp:Label ID="Label1" runat="server" Text="Deseja Aceitar a Data de Expiração do Documento?"></asp:Label>
            </p>
        </div>

        <div id="dialog_Arquivar" class="modal" title="Arquivar Documento">
            <p>
                <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                <asp:Label ID="Label2" runat="server" Text="Deseja Arquivar o Documento?"></asp:Label>
            </p>
        </div>

        <asp:HiddenField ID="hddidObjeto" runat="server" />
        <asp:HiddenField ID="hddsTipoObjeto" runat="server" />
        <asp:HiddenField ID="hddExcluirArquivo" runat="server" />
        <asp:HiddenField ID="hddSubTipo" runat="server" />
        <asp:HiddenField ID="hddsExtensoes" runat="server" />
        <%------ Gabriel Llanir 12/09/2024 ------%>
        <%------------------ Higor Maestrello 17-06-2024 -------------------------------%>
        <asp:HiddenField ID="hddChecked" runat="server" />
        <asp:HiddenField ID="hddArquivo" runat="server" />
        <asp:HiddenField ID="hddAceitarDoc" runat="server" />
        <asp:HiddenField ID="hddArquivarDoc" runat="server" />
        <asp:HiddenField ID="hddsEmpresaxColaborador" runat="server" />
        <asp:HiddenField ID="hddsidPedido" runat="server" />
        <asp:HiddenField ID="hddidOrdem" runat="server" Value="0"/>
        <%-------------------------------------------------------------------------------------%>
    </form>
    <div id="dialog" title="Adicione ou Altere a Imagem do Produto"></div>
</body>
</html>
