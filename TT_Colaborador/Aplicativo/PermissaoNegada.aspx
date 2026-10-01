<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="PermissaoNegada.aspx.cs" Inherits="TT_Colaborador.Aplicativo.PermissaoNegada" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <script src="https://login.tecandtec.com.br/app/js/jquery-3.2.0.min.js"></script>
    <link rel="stylesheet" href="https://login.tecandtec.com.br/plugins/Chosen/chosen.css" />
    <script src="https://login.tecandtec.com.br/plugins/Chosen/docsupport/jquery-3.2.1.min.js" type="text/javascript"></script>
    <script src="https://login.tecandtec.com.br/plugins/Chosen/chosen.jquery.js" type="text/javascript"></script>


    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-multiselect/0.9.13/css/bootstrap-multiselect.css" />
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
</head>
<body>
    <form runat="server" id="formMain">

        <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

        <div class="text-center">
            <div class="form-stacked row ">
                <br />
                <div class="col-lg-12">
                    <asp:Label ID="lblTituloPagina" CssClass="text-danger" runat="server" Text="TITULO_PAGINA"></asp:Label>
                </div>
                <div runat="server" id="divUsuarioVinculado_Voltar" class="col-lg-12">
                    <br />
                    <asp:Button ID="cmdVoltar" CssClass="btn btn-md btn-warning" runat="server" Text="Voltar" OnClick="cmdVoltar_Click" />
                </div>

                <div runat="server" id="divVoltar" class="col-lg-12">
                    <br />
                    <input type="submit" name="cmdVoltar" class="btn btn-md btn-warning" value="Voltar" id="field-cancel" title="Voltar" onclick="history.go(-1)" />
                </div>
            </div>
        </div>

    </form>
</body>
</html>
