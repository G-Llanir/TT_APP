<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Grupo_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.Manutencao.Grupo_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <div id="DIV_ESPACO" style="height: 100px" runat="server" visible="false"></div>
    <script src="/app/js/vanilla-masker.js"></script>

    <style>
        .tipos td {
            display: flex;
            align-items: center;
            margin: 5px;
            white-space: nowrap;
        }

        .tipos input[type="checkbox"] {
            cursor: pointer;
            -webkit-appearance: none;
            appearance: none;
            background-color: #f0f0f0;
            border: 2px solid #d1d1d1;
            padding: 10px;
            border-radius: 3px;
            transition: box-shadow 0.3s;
            margin-right: 10px;
        }

            .tipos input[type="checkbox"]:hover {
                box-shadow: 0 0 8px green;
            }

            .tipos input[type="checkbox"]:checked {
                background-color: #4CAF50;
                border-color: #4CAF50;
            }
    </style>

    <asp:UpdatePanel ID="UpdDetalhe" runat="server">
        <ContentTemplate>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>
            </div>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                </div>
                <div class="col-lg-12">
                    <div class="form-group">
                        <label>ID</label>
                        <asp:TextBox ID="txtidGrupo" runat="server" class="form-control CaixaTextoMini" disabled=""></asp:TextBox>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Descrição</label>
                        <asp:TextBox ID="txtsDscGrupo" runat="server" class="form-control CaixaTextoGigante" MaxLength="100"></asp:TextBox>
                    </div>
                </div>
                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Tipo de Grupo</label>
                        <asp:CheckBoxList runat="server" ID="cblTipo" class="tipos">
                            <asp:ListItem Value="PROD" Text="Grupo de Produtos"></asp:ListItem>
                            <asp:ListItem Value="SER" Text="Grupo de Serviços"></asp:ListItem>
                            <asp:ListItem Value="SUB_SER" Text="Grupo de Sub-Serviços"></asp:ListItem>
                            <asp:ListItem Value="REC" Text="Grupo de Recursos"></asp:ListItem>
                            <asp:ListItem Value="PROJ" Text="Grupo de Projetos"></asp:ListItem>
                        </asp:CheckBoxList>
                    </div>
                </div>
                <div class="col-lg-12">
                    <div class="form-group">
                        <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
                    </div>
                </div>

            </div>

            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                <input type="submit" name="cancel" class="btn btn-lg btn-warning" id="field-cancel" value="Cancelar" title="Voltar" onclick="history.go(-1)" />
            </fieldset>
            <asp:HiddenField ID="hddidGrupo" runat="server" />

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
