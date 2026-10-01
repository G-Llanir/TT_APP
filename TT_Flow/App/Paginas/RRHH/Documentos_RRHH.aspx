<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Documentos_RRHH.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.Documentos_RRHH" %>

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

    <script type="text/javascript">

        $(function () {

            $('[id*=txtdtExpiracaoDoc]').datepicker({
                autoclose: true,
                format: 'dd/mm/yyyy',
                language: 'pt-BR'
            });


            $('[id*=txtdtExpiracaoDoc]').mask('99/99/9999');

        });

    </script>

</head>
<body>
    <form id="frmDocumentos" runat="server">
        <br />
        <div>
            <div id="DIV3" runat="server">
                <div class="panel panel-default" runat="server">
                    <div class="panel-heading">
                        <h3 class="panel-title"><b>Arquivos</b></h3>
                    </div>
                    <%------------------ Higor Maestrello 18-06-2024 -------------------------------%>
                    <div class="col-lg-12">
                        <br />
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina1" />
                    </div>
                    <%-------------------------------------------------------------------------------%>
                    <div class="panel-body">
                        <div class="form-stacked row">

                            <%------------------ Higor Maestrello 18-06-2024 -------------------------------%>
                            <div class="col-lg-12" runat="server" id="btDownload">
                                <div class="form-group">
                                    <asp:Button ID="btDownloadArquivo" CssClass="btn-success btn btDownload" runat="server" Text="Download" />
                                </div>
                            </div>
                            <%-------------------------------------------------------------------------------%>

                            <div class="col-lg-12 table-responsive">
                                <asp:GridView ID="gv_Arquivo" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False" DataKeyNames="idArquivo, sAceitarDoc" OnRowCommand="gv_Arquivo_RowCommand" OnRowDataBound="gv_Arquivo_RowDataBound">
                                    <Columns>
                                        <%------------------ Higor Maestrello 18-06-2024 -------------------------------%>
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
                                        <%-------------------------------------------------------------------------------%>
                                        <asp:BoundField DataField="idArquivo" HeaderText="ID">
                                            <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscTipoArquivo" HeaderText="Tipo">
                                            <ItemStyle Width="16%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:ButtonField DataTextField="sNomeArquivo" HeaderText="Nome do Documento" CommandName="Download" ItemStyle-Width="20%" />

                                        <asp:BoundField DataField="sDscArquivo" HeaderText="Descrição">
                                            <ItemStyle Width="16%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="dtExpiracaoDoc" HeaderText="Data Expiração">
                                            <ItemStyle Width="12%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:TemplateField HeaderText="Aceitar Documento">
                                            <ItemTemplate>
                                                <asp:Label ID="Observacao" runat="server" Text='<%# Bind("sObservacao") %>'></asp:Label>
                                                <asp:Button ID="btnAceitar" runat="server" Text="Aceitar Data Expiração" CommandName="Aceitar" CssClass="AceitarDoc" />
                                            </ItemTemplate>
                                            <ItemStyle Width="12%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:TemplateField>


                                        <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Inclusão">
                                            <ItemStyle Width="12%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Inserido Por:">
                                            <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:TemplateField HeaderText="">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkDocumento_Excluir" runat="server" CssClass="btn btn-small ExcluirArquivo" TabIndex="100"><i class="fa fa-eraser"></i></asp:LinkButton>
                                            </ItemTemplate>
                                            <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>

                            </div>
                        </div>

                    </div>

                </div>

                <%--                <a class="btn btn-primary" data-toggle="collapse" href="#idFiltroCollapse" role="button" aria-expanded="false" aria-controls="collapseExample">Enviar Arquivo</a>
                <div class="collapse" id="idFiltroCollapse">--%>


                <div id="div_EnviarArquivos" class="form-stacked " runat="server">
                    <div class="panel panel-default" runat="server">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Enviar Documentos</b></h3>
                        </div>
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                        <div class="panel-body">
                            <div class="form-stacked row">

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Tipo</label>
                                        <asp:DropDownList ID="ddlidTipoDocumento" runat="server" class="form-control yes_no select CaixaTextoGrande"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-6" id="div4" runat="server">
                                    <div class="form-group">
                                        <label>Selecione o Documento</label>
                                        <asp:FileUpload ID="fu_Documento" class="form-control-file" runat="server" Width="400px" />
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>
                                            <asp:Label ID="dtExpiracaoDoc" runat="server" Text="Data Expiração"></asp:Label></label>
                                        <asp:TextBox ID="txtdtExpiracaoDoc" class="form-control CaixaTextoPequeno" placeholder="Opcional" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>
                                            <asp:Label ID="lblEnviarArquivo_sDscArquivo" runat="server" Text="Descrição"></asp:Label></label>
                                        <asp:TextBox ID="txtEnviarArquivo_sDscArquivo" class="form-control CaixaTextoGigante" placeholder="Insira uma descrição" runat="server" MaxLength="300"></asp:TextBox>
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

                <%--</div>--%>
            </div>

        </div>

        <div id="dialog_Apagar" class="modal" title="Excluir Arquivo">
            <p>
                <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                <asp:Label ID="LM_lblTituloDialog" runat="server" Text="Deseja Excluir o Documento?"></asp:Label>
            </p>
        </div>

        <div id="dialog_Aceitar" class="modal" title="Aceitar Arquivo">
            <p>
                <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                <asp:Label ID="Label1" runat="server" Text="Deseja Aceitar a Data de Expiração do Documento?"></asp:Label>
            </p>
        </div>

        <asp:HiddenField ID="hddidObjeto" runat="server" />
        <asp:HiddenField ID="hddsTipoObjeto" runat="server" />
        <asp:HiddenField ID="hddExcluirArquivo" runat="server" />
        <asp:HiddenField ID="hddAceitarDoc" runat="server" />
        <asp:HiddenField ID="hddChecked" runat="server"/>
        <asp:HiddenField ID="hddArquivarDoc" runat="server"/>
    </form>
</body>
</html>
