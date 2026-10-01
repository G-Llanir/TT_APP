<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="TipoProduto_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Almoxarifado.TipoProduto_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        .tipos td {
            display: flex;
            align-items: center;
            margin: 5px 0;
            white-space: nowrap;
        }

        /* Estilo padrão dos checkboxes/radios */
        .tipos input[type="checkbox"],
        .tipos input[type="radio"] {
            cursor: pointer;
            -webkit-appearance: none;
            appearance: none;
            background-color: #f0f0f0;
            border: 2px solid #d1d1d1;
            padding: 10px;
            border-radius: 3px;
            transition: box-shadow 0.3s;
            margin-right: 10px;
            width: 20px;
            height: 20px;
            display: inline-block;
        }

            .tipos input[type="checkbox"]:hover,
            .tipos input[type="radio"]:hover {
                box-shadow: 0 0 8px green;
            }

            .tipos input[type="checkbox"]:checked,
            .tipos input[type="radio"]:checked {
                background-color: #4CAF50;
                border-color: #4CAF50;
            }

        /* Remove input do título e mostra texto como cabeçalho */
        .tipos input[disabled] {
            display: none;
        }

        .tipos label[for*="cblPrincipal"]
        .tipos label[for*="cblTipo"] {
            font-weight: normal;
        }

        /* Destaca os "títulos" baseados no texto */
        .tipos td:has(input[disabled]) label {
            font-weight: bold;
            color: #2c3e50;
            margin-top: 10px;
            margin-bottom: 5px;
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
                        <asp:TextBox ID="txtidTipoProduto" runat="server" class="form-control CaixaTextoMini" disabled=""></asp:TextBox>
                    </div>
                </div>

                <div class="col-lg-3">
                    <div class="form-group">
                        <label>Descrição</label>
                        <asp:TextBox ID="txtsDscTipoProduto" runat="server" class="form-control" MaxLength="50"></asp:TextBox>
                    </div>
                </div>

                <div class="col-lg-12 row">
                    <div class="col-lg-12 form-group">
                        <label>Tipos</label>
                        <asp:RadioButtonList runat="server" ID="cblTipo" CssClass="tipos" RepeatDirection="Vertical"
                            AutoPostBack="true" OnSelectedIndexChanged="cblTipo_SelectedIndexChanged">
                            <asp:ListItem Value="PROD" Text="É Produto?"></asp:ListItem>
                            <asp:ListItem Value="SER" Text="É Serviço?"></asp:ListItem>
                            <asp:ListItem Value="SUB_SER" Text="É Sub-Serviço?"></asp:ListItem>
                            <asp:ListItem Value="REC" Text="É Recurso?"></asp:ListItem>
                            <asp:ListItem Value="FAB" Text="É Fabricado?"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>


                    <br />

                    <div class="col-lg-12 form-group">
                        <asp:CheckBoxList runat="server" ID="cblPrincipal" CssClass="tipos" RepeatDirection="Vertical">
                           <%-- <asp:ListItem Text="Exibição" Enabled="False"></asp:ListItem>
                            <asp:ListItem Value="COMP" Text="Exibe Composição"></asp:ListItem>
                            <asp:ListItem Value="COMER" Text="Exibe em Comercial"></asp:ListItem>

                            <asp:ListItem Text="Categorias" Enabled="False"></asp:ListItem>
                            <asp:ListItem Value="EMB" Text="Embalagem"></asp:ListItem>
                            <asp:ListItem Value="EPI" Text="EPI"></asp:ListItem>
                            <asp:ListItem Value="IND" Text="Industrializado"></asp:ListItem>
                            <asp:ListItem Value="SIS" Text="Sistema"></asp:ListItem>--%>
                        </asp:CheckBoxList>
                    </div>
                </div>

                <div id="divRegraFiscal" runat="server" class="col-lg-2" visible="false">
                    <div class="form-group">
                        <label>Tipo de Regra Fiscal</label>
                        <asp:DropDownList ID="ddlTipoRegra" runat="server" class="form-control"></asp:DropDownList>
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
            <asp:HiddenField ID="hddidTipoProduto" runat="server" />

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
