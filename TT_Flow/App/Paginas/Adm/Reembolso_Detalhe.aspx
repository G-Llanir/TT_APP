<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Reembolso_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.Reembolso_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.16/jquery.mask.min.js"></script>

    <style>
        .id {
            display: none;
        }

        .efeito {
            cursor: pointer;
            border: 3px solid lawngreen;
        }

            .efeito:hover {
                background-color: #f0f0f0;
            }

        .a {
            text-decoration: none !important;
            color: black !important;
        }

        .altura-scroll {
            max-height: 600px;
            overflow-y: scroll;
        }

        .modal-title {
            text-shadow: 1px 2px 3px rgba(0, 0, 0, 0.2);
            font-size: 20px;
            font-weight: bold;
            margin: 0;
        }

        .visible {
            display: none;
        }

        .chk-list label {
            padding-right: 10px;
        }

        /*    estilo grid col-participantes*/
        .wrap-participantes {
            word-wrap: break-word;
            word-break: break-word;
            line-height: 1.2;
            padding-top: 5px;
        }


        .label-info {
            display: inline-block;
            margin-bottom: 2px;
        }


        /*
================================================
 CSS DA TIMELINE DE HISTÓRICO
================================================
*/
        .timeline-container {
            position: relative;
            padding-left: 40px;
            list-style: none;
            margin-top: 20px;
        }

            .timeline-container::before { /* A linha vertical */
                content: '';
                position: absolute;
                left: 20px;
                top: 0;
                bottom: 0;
                width: 2px;
                background-color: #ddd;
            }

        .timeline-item {
            position: relative;
            margin-bottom: 20px;
        }

        .timeline-dot {
            position: absolute;
            left: -29px; /* Posição da "bolinha" */
            top: 5px;
            width: 18px;
            height: 18px;
            border-radius: 50%;
            background-color: #fff;
            border: 4px solid #007bff; /* Azul padrão */
        }

        /* Cores dinâmicas para a bolinha */
        .timeline-item.status-success .timeline-dot {
            border-color: #28a745;
        }

        .timeline-item.status-danger .timeline-dot {
            border-color: #dc3545;
        }

        .timeline-item.status-warning .timeline-dot {
            border-color: #ffc107;
        }

        .timeline-item.status-primary .timeline-dot {
            border-color: #007bff;
        }

        .timeline-content {
            background-color: #f9f9f9;
            padding: 15px;
            border-radius: 5px;
            border: 1px solid #eee;
        }

        .timeline-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 10px;
        }

        .timeline-user {
            font-weight: bold;
            font-size: 1.1em;
        }

        .timeline-date {
            font-size: 0.9em;
            color: #777;
        }

        .timeline-body {
            font-size: 1em;
        }

            /* Para renderizar o HTML do motivo */
            .timeline-body b {
                font-weight: bold;
            }

            .timeline-body span[style*="color:red"] {
                color: #dc3545 !important;
                text-decoration: line-through;
            }

            .timeline-body span[style*="color:green"] {
                color: #28a745 !important;
            }
    </style>

    <div id="hdd">
        <asp:HiddenField ID="hddidDespesas" runat="server" />
        <asp:HiddenField ID="hddsBloco" runat="server" />
        <asp:HiddenField ID="hddidItens" runat="server" />
        <asp:HiddenField ID="hddidCentrodeCusto" runat="server" />
        <asp:HiddenField ID="hddvalorMaximo" runat="server" />
        <asp:HiddenField ID="hddCentroCusto" runat="server" />
    </div>

    <contenttemplate>
        <div class="form-stacked row">
            <div class="col-lg-12">
                <h1>
                    <asp:Label ID="lblTituloPagina" runat="server" Text="Relatório de Reembolso"></asp:Label>
                    <small class="text-muted"></small>
                </h1>
                <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Detalhe" />
            </div>
        </div>

        <div class="panel panel-default" runat="server" id="DIV_Novo">
            <div class="panel-heading">
                <h3 class="panel-title"><b>Reembolso</b></h3>
            </div>
            <div class="panel-body">
                <div class="form-group row">

                    <div class="col-lg-12">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_Entregas" />
                    </div>


                    <div class="col-lg-12">
                        <div class="col-lg-2" runat="server" id="DivTipoCompra" visible="false">
                            <div class="form-group">
                                <b>
                                    <label>Tipo</label></b>
                                <asp:DropDownList ID="ddlsTipoCompra" runat="server" class="form-control yes_no select Caixa_Selecao">
                                    <asp:ListItem Value="2" Selected="True">Reembolso</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <label>Centro de Custo</label>
                                <asp:DropDownList ID="ddlCentroCusto" runat="server" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <label>Motivo</label>
                                <asp:TextBox ID="txtsDscMotivo" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-2">
                            <div class="form-group">
                                <h3>
                                    <span class="status" title="Status">
                                        <asp:Label ID="lblsDscTipoStatus" runat="server" TabIndex="100"></asp:Label>
                                    </span>
                                </h3>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-12">
                        <div class="col-lg-8">
                            <div class="panel panel-default" runat="server">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b></b></h3>
                                </div>
                                <div class="panel-body">
                                    <div class="form-stacked row">
                                        <div class="col-lg-12">

                                            <div class="form-group row" id="Div_NovoGasto" runat="server" visible="false">
                                                <div class="col-lg-12">
                                                    <button type="button" id="btnAbreModalGasto" class="btn btn-md btn-primary">
                                                        <i class="fa fa-plus"></i>
                                                        Nova Despesa
                                                    </button>
                                                </div>
                                            </div>
                                            <br />

                                            <div runat="server" id="DIV_Grid_Gastos" class="col-lg-12 table-responsive">
                                                <asp:GridView ID="gv_Despesas" class="table table-striped table-bordered table-hover table-condensed"
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="idItens, idContador"
                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gv_Despesas_RowDataBound"
                                                    OnRowDeleting="gv_Despesas_RowDeleting" OnRowCommand="gv_Despesas_RowCommand">
                                                    <Columns>

                                                        <asp:BoundField DataField="idItens" HeaderText="id">
                                                            <ItemStyle Width="2%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="idItens visible" />
                                                            <HeaderStyle CssClass="visible" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="idContador" HeaderText="ID">
                                                            <ItemStyle Width="2%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="idContador" />
                                                        </asp:BoundField>


                                                        <asp:BoundField DataField="sDscTipoDespesa" HeaderText="Tipo de Despesa">
                                                            <ItemStyle Width="18%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <%--                                                        <asp:BoundField DataField="sDscCategoriaPagar" HeaderText="Recurso">
                                                            <ItemStyle Width="18%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>--%>

                                                        <asp:BoundField DataField="sLocal" HeaderText="Local">
                                                            <ItemStyle Width="18%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sDscFormaPagamento" HeaderText="Forma de Pagamento">
                                                            <ItemStyle Width="18%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <%--  <asp:BoundField DataField="sParticipantes" HeaderText="Participantes">
                                                            <ItemStyle Width="18%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>--%>

                                                        <asp:TemplateField HeaderText="Participantes">
                                                            <ItemTemplate>
                                                                <%# GetParticipanteTags(Eval("sParticipantes"), ",") %>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Top" CssClass="wrap-participantes" />
                                                            <HeaderStyle Width="15%" />
                                                        </asp:TemplateField>

                                                        <asp:BoundField DataField="dtDespesa" HeaderText="Data">
                                                            <ItemStyle Width="18%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="nValor" HeaderText="Valor (R$)">
                                                            <ItemStyle Width="18%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <div style="display: flex;">
                                                                    <asp:LinkButton ID="lnkEnvio_UpLoad" CommandArgument='<%# Bind("idItens") %>' runat="server" ToolTip="Efetuar Upload de Arquivo" CssClass="btn btn-small btn-link lnkEnvio_UpLoad" TabIndex="100" CommandName="Upload_Arquivo"><i class="fa-upload fa"></i></asp:LinkButton>
                                                                    <asp:LinkButton ID="lnkEnvio_Download" CommandArgument='<%# Bind("idItens") %>' runat="server" ToolTip='<%# Bind("sNomeArquivo", "Download do Arquivo: {0}") %>' CssClass="btn btn-small btn-link" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>
                                                                    <asp:LinkButton ID="lnkEnvio_Excluir" CommandArgument='<%# Bind("idItens") %>' runat="server" CssClass="btn btn-small btn-link" TabIndex="100" ToolTip="Apagar Registro" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                                </div>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="Envio_Download" CommandArgument='<%# Bind("idItens") %>' runat="server" ToolTip='<%# Bind("sNomeArquivo", "Download do Arquivo: {0}") %>' CssClass="btn btn-small btn-link" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div>
                            </div>

                        </div>
                    </div>


                    <div class="modal fade" id="modalNovoGasto" tabindex="-1" role="dialog" aria-labelledby="lblModalGastoTitle" data-backdrop="static">
                        <div class="modal-dialog modal-lg" role="document">
                            <div class="modal-content" style="overflow: visible !important;">
                                <div class="modal-header">
                                    <h4 class="modal-title" id="lblModalGastoTitle">Adicionar Novo Gasto</h4>
                                </div>
                                <div class="modal-body">

                                    <div class="panel panel-default" runat="server" id="DIV5">
                                        <div class="panel-body">

                                            <div class="row">

                                                <div class="col-lg-12">
                                                    <div class="row">
                                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_Modal" />
                                                    </div>
                                                    <br />

                                                    <div class="row">
                                                        <div class="col-lg-2" id="DIV_dtDespesas" runat="server">
                                                            <div class="form-group">
                                                                <b>
                                                                    <label>Data</label></b>
                                                                <asp:TextBox ID="txtdtDespesas" class="form-control" runat="server" type="datetime-local"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-lg-3" id="DIV_Categoria" runat="server" visible="false">
                                                            <div class="form-group">
                                                                <b>
                                                                    <label>Recurso</label></b>
                                                                <asp:DropDownList ID="ddlidCategoria" runat="server" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                                                <%-- OnSelectedIndexChanged="ddlidCategoria_SelectedIndexChanged"--%>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3" id="DIV3" runat="server">
                                                            <div class="form-group">
                                                                <b>
                                                                    <label>Tipo de Despesa</label></b>
                                                                <asp:DropDownList ID="ddlTipoDespesa" runat="server" class="form-control yes_no select Caixa_Selecao" OnSelectedIndexChanged="ddlTipoDespesa_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3" id="DIV_Pagamento" runat="server">
                                                            <div class="form-group">
                                                                <b>
                                                                    <label>Forma de Pagamento</label></b>
                                                                <asp:DropDownList ID="ddlidFormaPagamento" runat="server" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                                            </div>
                                                        </div>

                                                    </div>
                                                    <asp:UpdatePanel runat="server" UpdateMode="Conditional">
                                                        <ContentTemplate>
                                                            <div class="row">
                                                                <div class="col-lg-6" runat="server" id="DIV_Participantes">
                                                                    <div class="form-group">
                                                                        <b>
                                                                            <label>Participantes</label></b>
                                                                        <asp:ListBox ID="ddlsParticipantes" runat="server" class="form-control yes_no select Caixa_Selecao" SelectionMode="Multiple" AutoPostBack="true" OnSelectedIndexChanged="ddlsParticipantes_SelectedIndexChanged"></asp:ListBox>
                                                                        <asp:CheckBoxList ID="chkParticipantes" CssClass="chk-list" runat="server" AutoPostBack="true" OnSelectedIndexChanged="chkParticipantes_SelectedIndexChanged" RepeatDirection="Horizontal" RepeatColumns="2"></asp:CheckBoxList>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                            <div class="row">
                                                                <div id="DIV1" runat="server">
                                                                    <div class="col-lg-4" id="DIV_Valor" runat="server">
                                                                        <div class="form-group">
                                                                            <b>
                                                                                <label runat="server" id="lblValor">Valor</label></b>
                                                                            <asp:TextBox ID="txtnValor" class="form-control" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>


                                                    <div class="row">
                                                        <div class="col-lg-4" id="DIV_Local" runat="server">
                                                            <div class="form-group">
                                                                <b>
                                                                    <label>Local</label></b>
                                                                <asp:TextBox ID="txtsLocal" class="form-control" runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-4" id="div4" runat="server">
                                                            <div class="form-group">
                                                                <b>
                                                                    <label>Selecione o Arquivo</label></b>
                                                                <br />
                                                                <asp:FileUpload ID="ImportarArquivo" runat="server" AllowMultiple="true" />
                                                            </div>
                                                        </div>
                                                    </div>

                                                </div>

                                            </div>

                                            <div class="row">
                                                <div class="col-lg-4" id="DIV_Inserir" runat="server">
                                                    <div class="form-group">
                                                        <br />
                                                        <asp:Button ID="btnInserir" class="btn btn-md btn-success" runat="server" Text="Inserir" OnClick="btnInserir_Click" />

                                                        <asp:Button ID="btnFechar" class="btn btn-md btn-danger" runat="server" Text="Cancelar" OnClick="btnFechar_Click" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>

                            </div>
                        </div>
                    </div>

                    <div class="col-lg-12">
                        <div class="col-lg-12">
                            <div class="form-group">
                                <label>Observações</label>
                                <asp:TextBox ID="txtsObservacao" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="95px"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-12">
                        <asp:Panel ID="pnlHistorico" runat="server" Visible="false">
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>Ações</b></h3>
                                </div>
                                <div class="panel-body">
                                    <ul class="timeline-container">
                                        <asp:Repeater ID="rptHistorico" runat="server">
                                            <ItemTemplate>
                                                <%-- A classe CSS "status-..." é definida no code-behind --%>
                                                <li class="timeline-item <%# GetStatusClass(Eval("sCor")) %>">
                                                    <div class="timeline-dot"></div>
                                                    <div class="timeline-content">
                                                        <div class="timeline-header">
                                                            <span class="timeline-user">
                                                                <i class="fa fa-user"></i><%# Eval("sDscUsuario") %>
                                                        </span>
                                                            <span class="timeline-date">
                                                                <i class="fa fa-calendar"></i><%# Eval("dtStatusFormatada") %>
                                                        </span>
                                                        </div>
                                                        <div class="timeline-body">
                                                            <strong>Status alterado para: <span class="label label-<%# Eval("sCor") %>"><%# Eval("sStatus") %></span></strong>

                                                            <%-- Usamos um Literal para renderizar o HTML do motivo (vermelho/verde) --%>
                                                            <asp:Literal ID="litMotivo" runat="server" Text='<%# GetMotivo(Eval("sDscMotivo")) %>' />
                                                        </div>
                                                    </div>
                                                </li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ul>
                                </div>
                            </div>
                        </asp:Panel>
                    </div>


                    <div class="col-lg-12">
                        <div class="col-lg-12">
                            <fieldset class="form-stacked actions">
                                <asp:Button ID="cmdSalvar" class="btn btn-md btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />

                                <asp:Button ID="Button1" class="btn btn-md btn-info" runat="server" Text="PDF" OnClick="Button1_Click" />
                                <asp:Button ID="btnVoltar" class="btn btn-md btn-warning" runat="server" Text="Voltar" OnClick="btnVoltar_Click" />

                                <asp:Button ID="cmdAprovar" class="btn btn-md btn-success" runat="server" Text="Aprovar" OnClick="cmdAprovar_Click" />
                                <asp:Button ID="cmdRejeitar" class="btn btn-md btn-danger" runat="server" Text="Rejeitar" OnClick="cmdRejeitar_Click" />
                                <asp:Button ID="btnExcluir" class="btn btn-md btn-danger" runat="server" Text="Excluir" OnClick="btnExcluir_Click" />
                                <asp:Button ID="btnFinalizar" class="btn btn-md btn-success" runat="server" Text="Finalizar Relatório" />

                            </fieldset>

                            <div id="dialog-Finalizar" class="modal" title="Finalizar Relatório">
                                <p>
                                    <asp:Label ID="lblTituloSalvar" runat="server" Text="Ao finalizar o relatório, ele não poderá mais ser editado. Você realmente deseja finalizar?"></asp:Label>
                                </p>
                            </div>

                        </div>
                    </div>

                </div>

            </div>
        </div>

        <%--final panel--%>
    </contenttemplate>
</asp:Content>
