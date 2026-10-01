<%@ Page Language="C#" MasterPageFile="~/app/main.master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="CRM.aspx.cs" Inherits="TT_Flow.App.Paginas.Comercial.CRM" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Pesquisa_Parceiros.ascx" TagPrefix="uc1" TagName="Pesquisa_Parceiros" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link rel="stylesheet" href="/app/css/styleCRM.css">
    <link rel="stylesheet" href="https://fonts.googleapis.com/icon?family=Material+Icons" />
    <link href="https://fonts.googleapis.com/css?family=Nunito:400,500,700,800,900" rel="stylesheet">
    <%--<script src="/app/js/kanbanCRM.js"></script>--%>

    <style>
        .custom-margin {
            margin-right: 2%;
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

        .status {
            position: absolute;
            right: 50px;
            top: 10px;
        }

        .btn-custom {
            padding: 5px 10px;
            font-size: 14px;
        }

        .panel-body .form-control {
            width: 100%;
        }

        .ui-autocomplete {
            max-height: 200px;
            overflow-y: auto;
            overflow-x: hidden;
            width: 30%;
        }

        #btnAcao .dropdown-menu li {
            border: 0;
        }

        #btnAcao .dropdown-menu button {
            width: 100%;
            border-radius: 0;
        }

        #btnAcao .dropdown-menu .divider {
            margin: 0;
        }

        .modal-backdrop.show {
            z-index: 1060;
        }

        .modal.show {
            z-index: 1070;
        }

        .modal-backdrop.fade.show {
            display: none;
        }

        .align-right {
            float: right;
            margin-right: 15px;
        }

        .alert {
            margin-left: 15px;
        }

        .dropdown-menu {
            padding: 0.1px 0;
        }

        .btn-block + .btn-block {
            margin-top: 1.5px;
        }

        .chosen-container {
            width: 100% !important
        }

        .form-group.d-flex.align-items-center.h-100 {
            margin-top: 20px;
        }

        span.material-icons {
            font-size: 30px;
            color: green;
            padding: revert;
        }

        button#btnNovoOrcamento {
            border: 0.5px solid;
            border-color: lightgray;
            padding: revert;
            display: inline-flex;
        }

        .chosen-container {
            margin-left: 5px !important;
        }

        input#cphCorpo_btnNegocioPerdido {
            margin: 0rem 0rem 0rem 0rem !important;
        }

        input#cphCorpo_btnNegocioFechado {
            margin: 0rem 0rem 0rem 0rem !important;
        }

        input#cphCorpo_btnNegocioExcluido {
            margin: 0rem 0rem 0rem 0rem !important;
        }

        button#cphCorpo_btnOrcamentoDetalhe {
            border: 0.5px solid;
            border-color: lightgray;
            padding: revert;
            display: inline-flex;
        }

        button#cphCorpo_btnVisualizaPDF {
            border: 0.5px solid;
            border-color: lightgray;
            padding: revert;
            display: inline-flex;
        }

        .deleteButton {
            width: 40px;
            height: 40px;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            gap: 3px;
            background-color: transparent;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            transition: all 0.2s;
            position: relative;
            overflow: hidden;
            margin-right: 15px;
        }

            .deleteButton svg {
                width: 55%;
            }

            .deleteButton:hover {
                background-color: rgb(237, 56, 56);
                overflow: visible;
            }

        .bin path {
            transition: all 0.2s;
        }

        .deleteButton:hover .bin path {
            fill: #fff;
        }

        .deleteButton:active {
            transform: scale(0.98);
        }

        .tooltip {
            --tooltip-color: rgb(41, 41, 41);
            position: absolute;
            top: -40px;
            background-color: var(--tooltip-color);
            color: white;
            border-radius: 5px;
            font-size: 12px;
            padding: 8px 12px;
            font-weight: 600;
            box-shadow: 0px 10px 10px rgba(0, 0, 0, 0.105);
            display: flex;
            align-items: center;
            justify-content: center;
            opacity: 0;
            transition: all 0.5s;
        }

            .tooltip::before {
                position: absolute;
                width: 10px;
                height: 10px;
                transform: rotate(45deg);
                content: "";
                background-color: var(--tooltip-color);
                bottom: -10%;
            }

        .deleteButton:hover .tooltip {
            opacity: 1;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="Server">

    <uc1:Manual runat="server" ID="manual" />

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked row" id="div_Consulta" runat="server">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="CRM"></asp:Label>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="CRM" />

            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>

        <div class="col-lg-12">

            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;Funil</h3>
                </div>

                <div class="panel-body ">
                    <div class="form-group">
                        <div class="col-lg-12">
                            <div class="row">
                                <div class="col-lg-1 custom-margin">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtdtInicial" class="form-control CaixaTextoData" placeholder="Data Inicial" runat="server" MaxLength="10" data-mask="00/00/0000" AutoPostBack="true" OnTextChanged="txtdtInicial_TextChanged"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-1 custom-margin">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtdtFinal" class="form-control CaixaTextoData" placeholder="Data Final" runat="server" MaxLength="10" data-mask="00/00/0000" AutoPostBack="true" OnTextChanged="txtdtFinal_TextChanged"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlsCLiente" class="form-control  Caixa_Selecao" runat="server" attrname="idRegistroCRM" AutoPostBack="true" EnableViewState="true" OnSelectedIndexChanged="ddlsCLiente_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlsTipo" class="form-control  Caixa_Selecao" runat="server" attrname="idRegistroCRM" AutoPostBack="true" EnableViewState="true" OnSelectedIndexChanged="ddlsTipo_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlsVendedor" class="form-control  Caixa_Selecao" runat="server" attrname="idRegistroCRM" AutoPostBack="true" EnableViewState="true" OnSelectedIndexChanged="ddlsVendedor_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlsStatus" class="form-control  Caixa_Selecao" runat="server" attrname="idRegistroCRM" AutoPostBack="true" EnableViewState="true" OnSelectedIndexChanged="ddlsStatus_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-1">
                                    <div class="btn-group" style="display: flex">
                                        <asp:CheckBox ID="idGrid" Text="Grid" runat="server" CssClass="btn btn-primary" AutoPostBack="true" Checked="True" OnCheckedChanged="idGrid_CheckedChanged" />
                                        <asp:CheckBox ID="idKanban" Text="Kanban" runat="server" CssClass="btn btn-primary" AutoPostBack="true" OnCheckedChanged="idKanban_CheckedChanged" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-12">
                        <div class="form-group">
                            <asp:Button ID="cmdNovoNegocio" class="btn btn-success" runat="server" Text="Novo Negócio" OnClick="cmdNovoNegocio_Click" />
                        </div>
                    </div>
                </div>
            </div>


            <div class="row">
                <div class="col-lg-12">
                    <asp:Panel ID="pnGrid" runat="server">

                        <div id="gridCRM">
                            <div class="panel panel-primary">
                                <div class="panel-body">
                                    <button type="button" class="btn btn-toggle-col" data-toggle="collapse" data-target="#toggleButtons">
                                        Mostrar/Ocultar Colunas
                                    </button>
                                    <div id="toggleButtons" class="collapse">
                                        <br />
                                        <asp:PlaceHolder ID="placeholderButtons" runat="server"></asp:PlaceHolder>
                                    </div>
                                    <br />
                                    <br />
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">

                                            <Columns>

                                                <asp:BoundField DataField="dtInclusao" HeaderText="Data">
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Cliente">
                                                    <HeaderStyle Width="15%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnsCliente" runat="server"
                                                            Text='<%# Eval("sCliente") %>'
                                                            CommandArgument='<%# Eval("idRegistroCRM") %>'
                                                            OnCommand="LinkButton_Command">
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Referência">
                                                    <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnsReferencia" runat="server"
                                                            Text='<%# Eval("sReferencia") %>'
                                                            CommandArgument='<%# Eval("idRegistroCRM") %>'
                                                            OnCommand="LinkButton_Command">
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="sVendedor" HeaderText="Vendedor">
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sStatus" HeaderText="Status">
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="nMaterial" HeaderText="Valor Material" DataFormatString="R$ {0:N2}">
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="nServico" HeaderText="Valor Serviços" DataFormatString="R$ {0:N2}">
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="nValor" HeaderText="Valor Total" DataFormatString="R$ {0:N2}">
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="dtPrevisao" HeaderText="Previsão Fechamento">
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sChance" HeaderText="Chance" DataFormatString="{0}%">
                                                    <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="dtUltimoContato" HeaderText="Data Último Contato">
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="dtProximoContato" HeaderText="Data Próximo Contato">
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="ID" Visible="false">
                                                    <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <%--<ItemTemplate>
                                                         <asp:LinkButton ID="btnIdRegistroCRM" runat="server"
                                                             Text='<%# Eval("idRegistroCRM") %>'
                                                             CommandArgument='<%# Eval("idRegistroCRM") %>'
                                                             OnCommand="LinkButton_Command">
                                                         </asp:LinkButton>
                                                     </ItemTemplate>--%>
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="sStatusCancelamento" HeaderText="Status Cancelado" Visible="false">
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sStatusFinalizado" HeaderText="Status Finalizado" Visible="false">
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sStatusExcluido" HeaderText="Status Excluido" Visible="false">
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
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
        <asp:Panel ID="pnKanban" runat="server" Visible="false">
            <div class="boards overflow-auto p-0" id="boardsContainer">
            </div>
        </asp:Panel>
    </div>

    <!------------------- Modal -------------------------------------------------------------------------------------------------------------------------------------------------->
    <div class="modal fade" id="modalCRM" tabindex="-1" data-backdrop="static" role="dialog">
        <div class="modal-dialog modal-dialog-centered modal-largo" role="document">
            <div class="modal-content">
                <asp:UpdatePanel ID="updTituloModal" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" id="btnModalFechar" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <div class="modal-header-content">
                                <div class="modal-logo">
                                    <asp:Image ID="Image1" runat="server" ImageUrl="~/App/img/instrucaoTecnica_pdf.jpg" AlternateText="Instrução Técnica" />
                                </div>


                                <div class="modal-title-container">
                                    <asp:Label runat="server" class="modal-title" ID="lblModalTitulo" Text="Alterar Negócio" Font-Bold="true"></asp:Label>
                                </div>


                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <h3>
                                            <span class="status" title="Status">
                                                <asp:Label ID="lblsDscTipoStatus" runat="server" TabIndex="100"></asp:Label>
                                                <label>&nbsp;</label>
                                                <asp:Label ID="lbldtProximoContato" runat="server" TabIndex="100"></asp:Label>
                                            </span>

                                        </h3>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-body">

                                <div>
                                    <ul id="tab_ModalFollowUp" class="nav nav-tabs" role="tablist">
                                        <li role="presentation" class="tabpanel active">
                                            <a href="#detalheNegocio" id="detalhe-tab" role="tab" data-toggle="tab" aria-controls="Detalhe" aria-expanded="false"><b>Detalhe</b></a>
                                        </li>

                                        <li role="presentation" runat="server" id="aba_FollowUp">
                                            <a href="#followUpNegocio" role="tab" id="followUp-tab" data-toggle="tab" aria-controls="FollowUp"><b>Follow Up</b></a>
                                        </li>

                                        <li role="presentation" runat="server" id="aba_Arquivo">
                                            <a href="#arquivoNegocio" role="tab" id="arquivo-tab" data-toggle="tab" aria-controls="Arquivo"><b>Documentos</b></a>
                                        </li>

                                        <li role="presentation" runat="server" id="aba_Historico">
                                            <a href="#historicoNegocio" role="tab" id="historico-tab" data-toggle="tab" aria-controls="Historico"><b>Histórico</b></a>
                                        </li>

                                    </ul>
                                </div>
                                <div id="tab" class="tab-content">

                                    <%--TAB DETALHE--%>

                                    <div role="tabpanel" class="tab-pane fade in active" id="detalheNegocio" aria-labelledby="Detalhe">
                                        <br />
                                        <%--                                <asp:UpdatePanel ID="updDetalhe" runat="server">
                                <ContentTemplate>--%>

                                        <div class="panel panel-default" runat="server" id="div_DetalheNegocio">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>Detalhe</b></h3>
                                            </div>

                                            <div class="panel-body ">
                                                <div class="form-stacked row">
                                                    <div class="col-lg-12 row">
                                                        <asp:UpdatePanel ID="idUpdPanelMsgDetalhe" runat="server">
                                                            <ContentTemplate>
                                                                <uc1:MensagemPagina runat="server" ID="DetalheNegocio_MensagemPagina" />
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </div>

                                                    <div class="form-group">
                                                        <div class="col-lg-12">
                                                            <div class="row">
                                                                <div class="col-lg-3">
                                                                    <div class="form-group">
                                                                        <label>Data</label>
                                                                        <asp:TextBox ID="txtdtInclusao" type="datetime-local" class="form-control CaixaTextoMedio" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-3">
                                                                    <div class="form-group">
                                                                        <label>Vendedor</label>
                                                                        <asp:DropDownList ID="ddlModalsVendedor" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-3">
                                                                    <div class="form-group">
                                                                        <label>Tipo</label>
                                                                        <asp:DropDownList ID="ddlModalsTipo" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-3" runat="server" id="div_ddlsModalStatus">
                                                                    <div class="form-group">
                                                                        <label>Status</label>
                                                                        <asp:DropDownList ID="ddlsModalStatus" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="form-group">
                                                        <div class="col-lg-12">
                                                            <div class="row">
                                                                <div class="col-lg-3">
                                                                    <div class="form-group">
                                                                        <label>Cliente</label>
                                                                        <asp:TextBox ID="txtsCliente" class="form-control" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-3">
                                                                    <div class="form-group">
                                                                        <label>Contato</label>
                                                                        <asp:TextBox ID="txtsContato" class="form-control CaixaTextoMedio" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-3">
                                                                    <div class="form-group">
                                                                        <label>Telefone</label>
                                                                        <asp:TextBox ID="txtsTelefone" class="form-control CaixaTextoMedio" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-3">
                                                                    <div class="form-group">
                                                                        <label>E-mail</label>
                                                                        <asp:TextBox ID="txtsEmail" class="form-control CaixaTextoMedio" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="form-group">
                                                        <div class="col-lg-12">
                                                            <div class="row">
                                                                <div class="col-lg-3">
                                                                    <div class="form-group">
                                                                        <label>Chance de Fechamento</label>
                                                                        <asp:DropDownList ID="ddlChance" class="form-control Caixa_Selecao" runat="server">
                                                                            <asp:ListItem Text="Selecione a Chance" Value=""></asp:ListItem>
                                                                            <asp:ListItem Text="0%" Value="0"></asp:ListItem>
                                                                            <asp:ListItem Text="25%" Value="25"></asp:ListItem>
                                                                            <asp:ListItem Text="50%" Value="50"></asp:ListItem>
                                                                            <asp:ListItem Text="75%" Value="75"></asp:ListItem>
                                                                            <asp:ListItem Text="100%" Value="100"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-3">
                                                                    <div class="form-group">
                                                                        <label>Previsão Fechamento</label>
                                                                        <asp:TextBox ID="txtdtPrevisao" type="date" class="form-control CaixaTextoMedio" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2">
                                                                    <label>Valor Material</label>
                                                                    <div class="input-group">
                                                                        <asp:TextBox ID="txtnMaterial" runat="server" CssClass="valor-input form-control" aria-describedby="sMoedaOrigem" Style="z-index: 0;" OnTextChanged="txtnMaterial_TextChanged"></asp:TextBox>
                                                                        <span class="input-group-addon" id="sMoedaOrigem" runat="server">R$</span>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-2">
                                                                    <label>Valor Serviço</label>
                                                                    <div class="input-group">
                                                                        <asp:TextBox ID="txtnServico" runat="server" CssClass="valor-input form-control" aria-describedby="sMoedaOrigem" Style="z-index: 0;" OnTextChanged="txtnServico_TextChanged"></asp:TextBox>
                                                                        <span class="input-group-addon" id="Span1" runat="server">R$</span>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-2">
                                                                    <label>Total</label>
                                                                    <div class="input-group">
                                                                        <asp:TextBox ID="txtnValor" CssClass="valor-input form-control" aria-describedby="sMoedaOrigem" Style="z-index: 0;" runat="server"></asp:TextBox>
                                                                        <span class="input-group-addon" id="Span2" runat="server">R$</span>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="form-group">
                                                        <div class="col-lg-12">
                                                            <div class="row">
                                                                <div class="col-lg-3">
                                                                    <div class="form-group">
                                                                        <label>N° Controle TT</label>
                                                                        <asp:TextBox ID="txtnControle" class="form-control" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-4">
                                                                    <div class="form-group">
                                                                        <label>Referência</label>
                                                                        <asp:TextBox ID="txtsReferencia" class="form-control" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-1" runat="server" id="div_confidencial">
                                                                    <div class="form-group">
                                                                        <label>Confidencial</label>
                                                                        <asp:CheckBox ID="cbConfidencial" runat="server" class="form-control" Style="width: fit-content; padding: 4px; display: flex"></asp:CheckBox>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-2" runat="server" id="div_btnNovoOrcamento" style="text-align: center" visible="true">
                                                                    <label>Novo Orçamento</label>
                                                                    <div class="form-group" style="text-align: center">
                                                                        <button class="btn" id="btnNovoOrcamento">
                                                                            <span class="material-icons" onclick="orcamentoNovo()">add_box</span>
                                                                        </button>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-2" runat="server" id="div_btnOrcamentoDetalhe" style="text-align: center" visible="false">
                                                                    <div class="form-group" style="text-align: center">
                                                                        <label>Orçamento Detalhe</label>
                                                                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                                            <ContentTemplate>
                                                                                <button id="btnOrcamentoDetalhe" class="btn" runat="server" onserverclick="btnOrcamentoDetalhe_Click">
                                                                                    <span class="material-icons">view_list</span>
                                                                                </button>
                                                                            </ContentTemplate>
                                                                        </asp:UpdatePanel>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-2" runat="server" id="div_btnVisualizaPDF" style="text-align: center" visible="false">
                                                                    <label>PDF Orçamento</label>
                                                                    <div class="form-group" style="text-align: center">
                                                                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                                                            <ContentTemplate>
                                                                                <button class="btn" id="btnVisualizaPDF" runat="server" onserverclick="btnVisualizaPDF_Click">
                                                                                    <span class="material-icons" onclick="visualizaPDF()">document_scanner</span>
                                                                                </button>
                                                                            </ContentTemplate>
                                                                        </asp:UpdatePanel>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                    </div>
                                                </div>

                                                <div class="col-lg-12">
                                                    <div class="form-group">
                                                        <label>Observação</label>
                                                        <asp:TextBox ID="txtsObservacao" class="form-control" runat="server" TextMode="MultiLine" Height="100px" Style="resize: none;"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <asp:Panel ID="pnBtnSalvar" runat="server">

                                            <div class="btn-lg">
                                                <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                                                <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field_cancel" title="Voltar" data-dismiss="modal">

                                                <div class="align-right">

                                                    <div class="btn" runat="server" id="div_btnExcluir" style="padding: 0px">
                                                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                            <ContentTemplate>
                                                                <button id="btnNegocioExcluido" class="deleteButton" runat="server" onserverclick="btnNegocioExcluido_Click">
                                                                    <svg
                                                                        xmlns="http://www.w3.org/2000/svg"
                                                                        fill="none"
                                                                        viewBox="0 0 50 59"
                                                                        class="bin">
                                                                        <path
                                                                            fill="#B5BAC1"
                                                                            d="M0 7.5C0 5.01472 2.01472 3 4.5 3H45.5C47.9853 3 50 5.01472 50 7.5V7.5C50 8.32843 49.3284 9 48.5 9H1.5C0.671571 9 0 8.32843 0 7.5V7.5Z">
                                                                        </path>
                                                                        <path
                                                                            fill="#B5BAC1"
                                                                            d="M17 3C17 1.34315 18.3431 0 20 0H29.3125C30.9694 0 32.3125 1.34315 32.3125 3V3H17V3Z">
                                                                        </path>
                                                                        <path
                                                                            fill="#B5BAC1"
                                                                            d="M2.18565 18.0974C2.08466 15.821 3.903 13.9202 6.18172 13.9202H43.8189C46.0976 13.9202 47.916 15.821 47.815 18.0975L46.1699 55.1775C46.0751 57.3155 44.314 59.0002 42.1739 59.0002H7.8268C5.68661 59.0002 3.92559 57.3155 3.83073 55.1775L2.18565 18.0974ZM18.0003 49.5402C16.6196 49.5402 15.5003 48.4209 15.5003 47.0402V24.9602C15.5003 23.5795 16.6196 22.4602 18.0003 22.4602C19.381 22.4602 20.5003 23.5795 20.5003 24.9602V47.0402C20.5003 48.4209 19.381 49.5402 18.0003 49.5402ZM29.5003 47.0402C29.5003 48.4209 30.6196 49.5402 32.0003 49.5402C33.381 49.5402 34.5003 48.4209 34.5003 47.0402V24.9602C34.5003 23.5795 33.381 22.4602 32.0003 22.4602C30.6196 22.4602 29.5003 23.5795 29.5003 24.9602V47.0402Z"
                                                                            clip-rule="evenodd"
                                                                            fill-rule="evenodd">
                                                                        </path>
                                                                        <path fill="#B5BAC1" d="M2 13H48L47.6742 21.28H2.32031L2 13Z"></path>
                                                                    </svg>

                                                                    <span class="tooltip">Excluir</span>
                                                                </button>

                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </div>

                                                    <div class="btn-group dropup" id="btnAcao" runat="server">
                                                        <button id="cmdFinalizaNegocio" runat="server" type="button" class="btn btn-lg btn-primary dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Fechar Negócio <span class="caret"></span></button>
                                                        <ul class="dropdown-menu">
                                                            <asp:Button ID="btnNegocioPerdido" class="btn btn-danger btn-block" runat="server" Text="Negócio Perdido" OnClick="btnNegocioPerdido_Click" />
                                                            <li role="separator" class="divider" runat="server" id="btnDivisor3" style="margin: 2.5px 0px"></li>
                                                            <li>
                                                                <asp:Button ID="btnNegocioFechado" class="btn btn-success btn-block" runat="server" Text="Negócio Fechado" OnClick="btnNegocioFechado_Click" />
                                                        </ul>

                                                    </div>

                                                </div>
                                            </div>

                                            <%--------------------------------------MODAL AÇAO---------------------------------------------------------------------------------------%>
                                            <div class="modal fade" id="modalAcao" tabindex="-2" role="dialog" data-backdrop="static" aria-labelledby="modalFollowUpLabel" aria-hidden="true" style="height: 100%;">
                                                <div class="modal-dialog modal-sm" role="document" style="width: 45%;">
                                                    <div class="modal-content">
                                                        <div class="modal-header">
                                                            <h5 class="modal-title" id="modalFollowUpLabel">Ação</h5>
                                                        </div>
                                                        <div class="modal-body">
                                                            <div class="row" runat="server" id="div_Acao_FollowUp">
                                                                <div class="col-lg-12">
                                                                    <div class="panel panel-primary">
                                                                        <div class="panel-heading">
                                                                            <h3 class="panel-title">
                                                                                <asp:Label ID="lblNegocios_Acao_Titulo" runat="server" Text="Ação"></asp:Label></h3>
                                                                        </div>
                                                                        <div class="panel-body">
                                                                            <div class="col-lg-12 row">
                                                                                <asp:UpdatePanel ID="uptPanelAcao" runat="server">
                                                                                    <ContentTemplate>
                                                                                        <uc1:MensagemPagina runat="server" ID="AcaoFollowUp_MensagemPagina" />
                                                                                    </ContentTemplate>
                                                                                </asp:UpdatePanel>
                                                                            </div>

                                                                            <div class="col-xs-12">
                                                                                <div class="form-group">
                                                                                    <label>Data Finalização</label>
                                                                                    <asp:TextBox ID="txtdtFinalizacao" CssClass="form-control" runat="server" type="datetime-local"></asp:TextBox>
                                                                                </div>
                                                                            </div>


                                                                            <div class="col-xs-12">
                                                                                <div class="form-group">
                                                                                    <label>Motivo/Observação</label>
                                                                                    <asp:DropDownList ID="ddlsMotivo" class="form-control CaixaSelecao" runat="server"></asp:DropDownList>
                                                                                    <asp:TextBox ID="txtsMotivoObservacao" class="form-control" runat="server" TextMode="MultiLine" Height="80px" MaxLength="300"></asp:TextBox>
                                                                                </div>
                                                                            </div>


                                                                            <div class="col-lg-6">
                                                                                <div class="form-group">
                                                                                    <asp:Button ID="btnNegocioOK" class="btn btn-sm btn-primary" runat="server" Text="OK" OnClick="btnNegocioOK_Click" />
                                                                                    <asp:Button ID="btnNegocioCancelar" class="btn btn-sm btn-danger" runat="server" Text="Cancelar" OnClick="btnNegocioCancelar_Click" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <asp:HiddenField runat="server" ID="hddStatusNegocio" Value="N" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </div>
                                    <asp:HiddenField ID="hddidRegistroCRM" runat="server" />
                                </div>
                                <%-- TAB FOLLOW UP--%>

                                <div role="tabpanel" class="tab-pane fade" id="followUpNegocio" aria-labelledby="Follow Up">
                                    <br />
                                    <%--<asp:UpdatePanel ID="updFollowUp" runat="server">
                                <ContentTemplate>--%>
                                    <div class="panel panel-default" runat="server" id="div_followUp">
                                        <div class="panel-heading">
                                            <h3 class="panel-title"><b>Follow UP</b></h3>
                                        </div>
                                        <div class="panel-body">
                                            <div class="form-stacked row">
                                                <div class="col-lg-12 row">
                                                    <asp:UpdatePanel ID="idUpdPanelMsgFollowUp" runat="server">
                                                        <ContentTemplate>
                                                            <uc1:MensagemPagina runat="server" ID="FollowUPNegocio_MensagemPagina" />
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </div>

                                                <div class="form-group">
                                                    <div class="col-lg-12">
                                                        <div class="row">
                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Meio de Contato</label>
                                                                    <asp:DropDownList ID="ddlsMeioContato" class="form-control CaixaSelecao" runat="server"></asp:DropDownList>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Data/Hora Contato</label>
                                                                    <asp:TextBox ID="txtdtDataContato" type="datetime-local" class="form-control CaixaTextoMedio" runat="server"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Contato com</label>
                                                                    <asp:TextBox ID="txtsContatoCom" class="form-control CaixaTextoMedio" runat="server"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Data/Hora Próximo Contato</label>
                                                                    <asp:TextBox ID="txtdtProximoContato" type="datetime-local" class="form-control CaixaTextoMedio" runat="server"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <div class="col-lg-12">
                                                        <div class="row align-items-center">
                                                            <div class="col-lg-5">
                                                                <label>Observação</label>
                                                                <div class="form-group">
                                                                    <asp:TextBox ID="txtsObservacaoFollowUp" class="form-control CaixaSelecao" runat="server"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-7">
                                                                <div class="form-group d-flex align-items-center h-100">
                                                                    <asp:Button ID="btnIncluir" class="btn btn-lg btn-primary btn-custom ml-3" runat="server" Text="Incluir" OnClick="btnIncluir_Click" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="panel panel-default" runat="server">
                                                <div class="panel-heading">
                                                    <h4 class="panel-title"><b>Histórico de Contatos</b></h4>
                                                </div>
                                                <div class="panel-body ">
                                                    <%--GRID com HISTORICO DE CONTATOS --%>

                                                    <div class="row">
                                                        <div class="col-lg-12">
                                                            <asp:GridView ID="gvFollowUp" class="table table-striped table-bordered table-hover table-condensed"
                                                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                                                <Columns>
                                                                    <asp:BoundField DataField="idRegistroCRM" HeaderText="ID" Visible="false">
                                                                        <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    </asp:BoundField>

                                                                    <asp:BoundField DataField="dtContato" HeaderText="Data do Contato">
                                                                        <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    </asp:BoundField>

                                                                    <asp:BoundField DataField="sDscMeioContato" HeaderText="Meio de Contato">
                                                                        <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    </asp:BoundField>

                                                                    <asp:BoundField DataField="sContato" HeaderText="Contato">
                                                                        <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    </asp:BoundField>

                                                                    <asp:BoundField DataField="sObservacao" HeaderText="Observação">
                                                                        <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    </asp:BoundField>

                                                                    <asp:BoundField DataField="dtProximoContato" HeaderText="Data do Próximo Contato">
                                                                        <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    </asp:BoundField>
                                                                </Columns>
                                                                <PagerStyle BackColor="Black" ForeColor="Black" HorizontalAlign="Center" />
                                                            </asp:GridView>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>



                                    </div>
                                    <%-- </ContentTemplate>
                            </asp:UpdatePanel>--%>
                                </div>
                                <%--------------------------------------Tab-Arquivo----------------------------------------------------------------------------------------------------------------------------------------------------------------%>
                                <div role="tabpanel" class="tab-pane fade" id="arquivoNegocio" aria-labelledby="Arquivo">
                                    <div class=" embed-responsive embed-responsive-16by9" style="min-height: 800px" runat="server" id="DIV_Arquivos">
                                        <embed type="text/html" runat="server" id="frmArquivos" width="800" height="500" />
                                    </div>
                                </div>
                                <%--------------------------------------------Fim-Tab-Arquivo--------------------------------------------------------------------------------------------------------------------------------------------------------------%>


                                <%--TAB HISTORICO--%>

                                <div role="tabpanel" class="tab-pane fade" id="historicoNegocio" aria-labelledby="Historico">
                                    <br />
                                    <div class="panel panel-default" runat="server" id="DIV_historico">
                                        <div class="panel-heading">

                                            <h3 class="panel-title"><b>Histórico</b></h3>
                                        </div>
                                        <div class="panel-body">
                                            <div class="form-stacked row">
                                                <div class="col-lg-12 table-responsive">
                                                    <asp:GridView
                                                        ID="gv_Historico" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                        ShowFooter="False">
                                                        <Columns>
                                                            <asp:BoundField DataField="dtAcao" HeaderText="Data Ação">
                                                                <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário">
                                                                <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="sAcao" HeaderText="Ação">
                                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="sObservacao" HeaderText="Observação">
                                                                <ItemStyle Width="70%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>
                                                        </Columns>
                                                    </asp:GridView>

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <asp:HiddenField runat="server" ID="hddVisualizaPDF" Value="N" />
                                <asp:HiddenField runat="server" ID="hddStatusModal" Value="false" />
                                <%-- <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Fechar</button>
                        </div>--%>
                            </div>
                            <%-- FIM Tab Content--%>
                        </div>
                        </div>

                        <%--------------------------------------MODAL ORCAMENTO---------------------------------------------------------------------------------------%>
                        <div class="modal fade" id="modalConsultaOrcamento" tabindex="-2" role="dialog" data-backdrop="static" aria-labelledby="modalConsultaOrcamentoLabel" aria-hidden="true">
                            <div class="modal-dialog modal-sm" role="document" style="width: 40%;">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h4 class="modal-title">Orçamento</h4>
                                    </div>
                                    <div class="modal-body">
                                        <div class="col-lg-12 row" style="width: 100%; padding: 0px">
                                            <asp:UpdatePanel ID="uptPanelOrcamento" runat="server">
                                                <ContentTemplate>
                                                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaModalVincula" style="margin-left: 0px !important" />
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12 form-group">
                                                <label>Revisões do Orçamento</label>
                                                <asp:DropDownList runat="server" ID="ddlRevOrcamento" class="form-control Caixa_Selecao"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="modal-footer">
                                            <asp:Button ID="cmdOrcamento" class="btn btn-success" runat="server" Text="Ir para Orçamento" OnClick="cmdOrcamento_Click" />
                                            <asp:Button runat="server" ID="cmdFecharModalOrcamento" class="btn btn-danger" Text="Cancelar" OnClick="cmdFecharModalOrcamento_Click" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>


                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>

    </div>
    <asp:Button ID="btnTriggerModalKanban" runat="server" Style="display: none" OnClick="btnTriggerModalKanban_Click" />
    <asp:HiddenField runat="server" ID="hddIdCard" Value="" />
    <asp:HiddenField runat="server" ID="hddFiltroStatus" Value="N" />
    <asp:HiddenField runat="server" ID="hddidOrcamento" Value="" />



    <%--<script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>--%>
    <%--<script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/js/bootstrap.min.js" integrity="sha384-OgVRvuATP1z7JjHLkuOU7Xw704+h835Lr+6QL9UvYjZE3Ipu6Tp75j7Bh/kR0JKI" crossorigin="anonymous"></script>--%>
    <script src="https://unpkg.com/material-components-web@latest/dist/material-components-web.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/popper.js@1.16.0/dist/umd/popper.min.js" integrity="sha384-Q6E9RHvbIyZFJoft+2mJbHaEWldlvI9IOYy5n3zV9zzTtmI3UksdQRVvoxMfooAo" crossorigin="anonymous"></script>
    <%--<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>--%>


    <script>
        $(function () {
            var $dtInicial = $('[id*=txtdtInicial]');
            var $dtFinal = $('[id*=txtdtFinal]');

            $dtInicial.datepicker({
                autoclose: true,
                format: 'dd/mm/yyyy',
                language: 'pt-BR'
            }).on('changeDate', function (e) {
                if ($(this).data('previous') !== $(this).val()) {
                    $(this).data('previous', $(this).val());
                    __doPostBack($dtInicial.attr('name'), '');
                }
            }).mask('99/99/9999');

            $dtFinal.datepicker({
                autoclose: true,
                format: 'dd/mm/yyyy',
                language: 'pt-BR'
            }).on('changeDate', function (e) {
                if ($(this).data('previous') !== $(this).val()) {
                    $(this).data('previous', $(this).val());
                    __doPostBack($dtFinal.attr('name'), '');
                }
            }).mask('99/99/9999');

            $dtInicial.data('previous', $dtInicial.val());
            $dtFinal.data('previous', $dtFinal.val());
        });


        $(document).ready(function () {
            console.log(document.getElementById('<%= hddStatusModal.ClientID %>').value);
            recarregaEvento();

            var isGridChecked = $('#<%= idGrid.ClientID %>').prop('checked');
            var isKanbanChecked = $('#<%= idKanban.ClientID %>').prop('checked');

            var url = new URL(window.location.href);
            var params = new URLSearchParams(url.search);

            if (!params.has('view')) {
                if (isGridChecked) {
                    params.set('view', 'grid');
                }
                if (isKanbanChecked) {
                    params.set('view', 'kanban');
                }

                if (params.has('view')) {
                    window.history.replaceState({}, '', `${url.pathname}?${params.toString()}`);
                }
            }
            else {
                params.delete('view');

                if (isGridChecked) {
                    params.set('view', 'grid');
                }
                if (isKanbanChecked) {
                    params.set('view', 'kanban');
                }

                if (params.has('view')) {
                    window.history.replaceState({}, '', `${url.pathname}?${params.toString()}`);
                }
            }

        });

        function recarregaEvento() {
            $('#btnModalFechar, #field_cancel').click(function () {
                document.getElementById('<%= hddStatusModal.ClientID %>').value = 'false';

                var isGridChecked = $('#<%= idGrid.ClientID %>').prop('checked');
                var isKanbanChecked = $('#<%= idKanban.ClientID %>').prop('checked');

                window.location.href = isGridChecked ? '/App/Paginas/Comercial/CRM.aspx?view=grid' : '/App/Paginas/Comercial/CRM.aspx?view=kanban';
            });

            $('#modalCRM').keypress(function (event) {
                if (event.keyCode === 13) {
                    event.preventDefault();
                    return false;
                }
            });
        }

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (sender, args) {
            recarregaEvento();
        });

        function openModal(button) {
            const cardId = button.getAttribute('data-card-id');

            var hddIdCard = document.getElementById('<%= hddIdCard.ClientID %>');

            hddIdCard.value = cardId;
            __doPostBack('<%= btnTriggerModalKanban.UniqueID %>', '');

        }

        document.addEventListener("DOMContentLoaded", function () {
            var btnTriggerKanban = document.getElementById('<%= btnTriggerModalKanban.ClientID %>');
            if (btnTriggerKanban) {
                btnTriggerKanban.addEventListener('click', function () {
                });
            }
        });

        function orcamentoNovo() {
            var idCRM = document.getElementById('<%= hddidRegistroCRM.ClientID %>').value;
            var url = '/App/Paginas/Comercial/Orcamento_Detalhe.aspx?id=0&crm=' + idCRM;
            window.location.href = url;
        }

    </script>

</asp:Content>
