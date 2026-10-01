<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Kanban.aspx.cs" Inherits="TT_Flow.App.Paginas.Kanban" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    

    <title></title>
    <link href="https://login.tecandtec.com.br/app/css/bootstrap.css" rel="stylesheet"/>
    <link href="https://login.tecandtec.com.br/app/css/font-awesome/css/font-awesome.css" rel="stylesheet"/>
    <link href="https://login.tecandtec.com.br/app/css/TT.css" rel="stylesheet" />
    <link href="../css/TT_Kanban.css" rel="stylesheet" />
    

    <style>


        table 
        {
            text-align: center;
            padding: 10px;
            border-collapse: collapse;
            font-size: 11px;
            font-weight: bold;

            /*margin-left: auto;*/
            margin-right: auto;
           
        }
        tr 
        { 
            border: 1px solid #ddd;
            border-spacing: 5px;
            padding: 10px
        }
        td 
        {
            padding: 1px;
            max-width:130px;
            
        }




    </style>
    

</head>


    <body>
      <asp:Literal ID="ltrBotoes" runat="server"></asp:Literal>
    </body>


</html>


<%--TESTE TABELA--%>
    
<%--    <table width="500" border="1">
        <!--linha 1 -->
        <tr>
            <td>1</td>
        </tr>
        <!--linha 2-->
        <tr>
            <td>2</td>
        </tr>
        <!--linha 3-->
        <tr>
            <td>3</td>
        </tr>
    </table>--%>




       


       
       
        

