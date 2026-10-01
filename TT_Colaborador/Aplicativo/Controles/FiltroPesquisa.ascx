<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FiltroPesquisa.ascx.cs" Inherits="TT_Flow.App.Controles.FiltroPesquisa" %>

<link rel="stylesheet" href="//netdna.bootstrapcdn.com/bootstrap/3.0.0/css/bootstrap-glyphicons.css">


<%--<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-chevron-down" viewBox="0 0 16 16">
  <path fill-rule="evenodd" d="M1.646 4.646a.5.5 0 0 1 .708 0L8 10.293l5.646-5.647a.5.5 0 0 1 .708.708l-6 6a.5.5 0 0 1-.708 0l-6-6a.5.5 0 0 1 0-.708z"/>
</svg>--%>




<a data-toggle="collapse" id="idFiltroPesquisa" runat="server" href="#idFiltroCollapse" role="button" aria-expanded="false" aria-controls="idFiltroCollapse">   <i class="glyphicon glyphicon-chevron-down"></i> <span></span></a>

<div class="collapse" id="idFiltroCollapse">
    <div class="row">
        <div class="col-lg-4">
            <div class="form-group">
                <label>Tipo de Produto</label>
                <asp:DropDownList ID="ddlTipoProduto" class="form-control" runat="server"></asp:DropDownList>
            </div>
        </div>
        <div class="col-lg-4">
            <div class="form-group">
                <label>Familia</label>
                <asp:DropDownList ID="ddlFamilia" class="form-control" runat="server"></asp:DropDownList>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-group">
                <label>Grupo</label>
                <asp:DropDownList ID="ddlGrupo" class="form-control" runat="server"></asp:DropDownList>
            </div>
        </div>
        <div class="col-lg-2">
            <div class="form-group">
                <label>País de Origem</label>
                <asp:DropDownList ID="ddlPaisOrigem" class="form-control" runat="server"></asp:DropDownList>
            </div>
        </div>
    </div>
</div>


