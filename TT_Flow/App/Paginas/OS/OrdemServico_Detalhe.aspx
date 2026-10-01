<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="OrdemServico_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.OS.OrdemServico_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%--<%@ Register Src="~/App/Controles/TimerPicker.ascx" TagPrefix="uc1" TagName="TimerPicker" %>--%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false">||</div>
    <script type="text/javascript">

        $(function () {


        });
    </script>
    <asp:UpdatePanel ID="updDetalhe" runat="server">
        <ContentTemplate>

            <div class="form-stacked" style="">
                <div class="col-lg-12 row">
                    <br />
                    <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    <div class="well-lg" runat="server" id="caixaTitulo">
                        <div class="row">
                            <div class="col-lg-8">
                                <h4>
                                    <span id="MainContent_lblTituloPagina">
                                        <asp:Label ID="lblTituloPagina" runat="server" Text="Ordem Serviço"></asp:Label>
                                    </span>

                                </h4>
                            </div>
                            <div class="col-lg-4 text-right">
                                <span>
                                    <asp:Label ID="lblTituloStatus" class="label" runat="server" Text="TITULO_PAGINA"></asp:Label>
                                </span>
                            </div>
                        </div>
                    </div>



                <div>
                    <ul id="tab_OrdemServico" class="nav nav-tabs" role="tablist">
                        <li role="presentation" class="tabpanel active">
                            <a href="#ordemservico" id="aba_OrdemServico" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Ordem de Serviço</b></a>
                        </li>
                        <li role="presentation" runat="server" id="aba_Procedimentos">
                            <a href="#procedimentos" role="tab" id="procedimentos-tab" data-toggle="tab" aria-controls="Procedimentos"><b>Procedimentos</b></a>
                        </li>
                        <li role="presentation" runat="server" id="aba_Arquivos">
                            <a href="#arquivos" role="tab" id="arquivo-tab" data-toggle="tab" aria-controls="Historico"><b>Arquivo</b></a>
                        </li>
                        <li role="presentation" runat="server" id="aba_Historico">
                            <a href="#historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="Historico"><b>Histórico</b></a>
                        </li>
                    </ul>
                </div>

                <div id="tab" class="tab-content">
                    <div role="tabpanel" class="tab-pane fade in active" id="ordemservico" aria-labelledby="pedido-tab">

                        <asp:UpdatePanel ID="updCabecalho" runat="server">
                            <ContentTemplate>
                                <br />
                                <div class="panel panel-default" runat="server" id="div_Cabecalho">
                                    <div class="panel-heading">
                                        <h3 class="panel-title"><b>Cabeçalho</b></h3>
                                    </div>
                                    <div class="panel-body">
                                        <div class="form-stacked row">

                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>ID </label>
                                                    <asp:TextBox ID="txtidOrdemServico" class="form-control CaixaTextoMini" runat="server" disabled="0"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>Número OS</label>
                                                    <asp:TextBox ID="txtsNumeroOS" class="form-control" runat="server" disabled="0"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>Tipo</label>
                                                    <asp:DropDownList ID="ddlidTipoOrdemServico" class="form-control yes_no select" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlTipoOrdemServico_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-lg-6">
                                                <div class="row">
                                                    <div class="col-lg-3">
                                                        <div class="form-group">
                                                            <label>Data da OS</label>
                                                            <asp:TextBox ID="txtdtOrdemServico" class="form-control CaixaTextoData" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-4">
                                                        <div class="form-group">
                                                            <label>Centro de Custo</label>
                                                            <asp:TextBox ID="txtsCentroCusto" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>Departamento </label>
                                                    <asp:DropDownList ID="ddlidDepartamento" class="form-control" placeholder="Departamento" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidDepartamento_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>Solicitante </label>
                                                    <asp:DropDownList ID="ddlidSolicitante" class="form-control" runat="server" attrname="sSolicitante"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div runat="server" id="DIV_CLIENTE">
                                                <div class="col-lg-6">
                                                    <div class="form-group">
                                                        <label>Cliente</label>
                                                        <asp:DropDownList ID="ddlidCliente" runat="server" class="form-control yes_no select" AutoPostBack="true" OnSelectedIndexChanged="ddlCliente_SelectedIndexChanged"></asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="col-lg-3">
                                                    <div class="form-group">
                                                        <label>Referência</label>
                                                        <asp:DropDownList ID="ddlsReferencia" runat="server" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlReferencia_SelectedIndexChanged"></asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="col-lg-3">
                                                    <div class="form-group">
                                                        <label>Pedido</label>
                                                        <asp:TextBox ID="txtidPedido" runat="server" class="form-control" disabled="0"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                            <div runat="server" id="DIV_CONTATO">
                                                <div class="col-lg-6">
                                                    <div class="form-group">
                                                        <label>Endereço </label>
                                                        <asp:DropDownList ID="ddlidEndereco" class="form-control" runat="server"></asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="col-lg-6">
                                                    <div class="form-group">
                                                        <label>Contato</label>
                                                        <asp:DropDownList ID="ddlidContato" class="form-control" runat="server"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>


                                        </div>
                                    </div>
                                </div>

                                <asp:UpdatePanel ID="updPanel_Servico" runat="server">
                                    <ContentTemplate>
                                        <br />
                                        <div class="panel panel-default" runat="server" id="div1">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>Serviço</b></h3>
                                            </div>
                                            <div class="panel-body">

                                                <div class="col-lg-12">
                                                    <div class="row">
                                                        <div class="col-lg-6">
                                                            <div class="form-group">
                                                                <label>Motivo / Descrição do Serviço </label>
                                                                <asp:TextBox ID="txtsDscMotivo" class="form-control" runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div runat="server" id="DIV_INTERNO">
                                                            <div class="col-lg-6">
                                                                <div class="form-group">
                                                                    <label>Produto </label>
                                                                    <asp:TextBox ID="txtsProduto" class="form-control" runat="server"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-lg-12">

                                                    <div class="form-group">
                                                        <label>Observação </label>
                                                        <asp:TextBox ID="txtsObservacao" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="120px"></asp:TextBox>

                                                    </div>
                                                </div>
                                                <div class="col-lg-12">
                                                    <div class="row">
                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <label>Cronograma Macro de Início </label>
                                                                <asp:TextBox ID="txtdtDataInicio" class="form-control CaixaTextoData" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <label>Cronograma Macro de Término </label>
                                                                <asp:TextBox ID="txtdtDataTermino" class="form-control CaixaTextoData" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                                            </div>
                                                        </div>


                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <label>Horário de Início</label>
                                                                <asp:TextBox ID="txtdtHorarioInicio" class="form-control CaixaTextoData" runat="server" MaxLength="10"></asp:TextBox>
                                                                <%--<uc1:TimerPicker ID="txtdtHorarioInicio" runat="server" />--%>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <label>Horário de Término</label>
                                                                <asp:TextBox ID="txtdtHorarioTermino" class="form-control CaixaTextoData" runat="server" MaxLength="10"></asp:TextBox>
                                                                <%--<uc1:TimerPicker ID="txtdtHorarioTermino" runat="server" />--%>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-lg-12">
                                                    <div class="row">
                                                        <div class="col-lg-6">
                                                            <div class="form-group">
                                                                <label>Permitido Hora Extra </label>
                                                                <asp:DropDownList ID="ddlsHoraExtra" class="form-control" placeholder="HoraExtra" runat="server" attrname="sHoraExtra">
                                                                    <asp:ListItem Value="N">Não</asp:ListItem>
                                                                    <asp:ListItem Value="S">Sim</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>



                                                        <div runat="server" id="DIV_EXTERNO">
                                                            <div class="col-lg-6">
                                                                <div class="form-group">
                                                                    <label>Há Periculosidade </label>
                                                                    <asp:DropDownList ID="ddlsPericulosidade" class="form-control" placeholder="Periculosidade" runat="server">
                                                                        <asp:ListItem Value="N">Não</asp:ListItem>
                                                                        <asp:ListItem Value="S">Sim</asp:ListItem>
                                                                        <asp:ListItem Value="V">A verificar</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-lg-12">
                                                    <div class="row">
                                                        <div runat="server" id="DIV_LST_EXTERNA">
                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Tipo de Atividade </label>
                                                                    <asp:ListBox ID="lstidTipoAtividadeExterna" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Documentos a Elaborar </label>

                                                                    <asp:ListBox ID="lstidDocumentosExterna" runat="server" SelectionMode="Multiple"></asp:ListBox>

                                                                </div>
                                                            </div>

                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Exigências </label>
                                                                     <%--<a data-toggle="collapse" href="#idFiltroCollapseExigencias" role="button" aria-expanded="false" aria-controls="idFiltroCollapseExigencias"><i class="glyphicon glyphicon-chevron-down"></i><span></span></a>
                                                                    <div class="collapse" id="idFiltroCollapseExigencias">--%>
                                                                    <asp:ListBox ID="lstsExigencias" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                                                    <%-- <asp:CheckBoxList ID="cblsExigencias" class="form-control" runat="server">
                                                                        <asp:ListItem Value="1"> Bagagem extra inclusa na ida</asp:ListItem>
                                                                        <asp:ListItem Value="2"> Horário Noturno</asp:ListItem>
                                                                        <asp:ListItem Value="3"> Sábados</asp:ListItem>
                                                                        <asp:ListItem Value="4"> Domingo ou Feriados</asp:ListItem>
                                                                    </asp:CheckBoxList>--%>
                                                                </div>
                                                            </div>

                                                        </div>

                                                        <div runat="server" id="DIV_LST_INTERNA">
                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Tipo de Atividade </label>
                                                                    <asp:ListBox ID="lstidTipoAtividadeInterna" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Documentos a Elaborar </label>
                                                                    <asp:ListBox ID="lstidDocumentosInterna" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <label>Jornada de Trabalho </label>
                                                                <%--  <a data-toggle="collapse" href="#idFiltroCollapseJornadaTrabalho" role="button" aria-expanded="false" aria-controls="idFiltroCollapseJornadaTrabalho"><i class="glyphicon glyphicon-chevron-down"></i><span></span></a>
                                                                <div class="collapse" id="idFiltroCollapseJornadaTrabalho">--%>
                                                                <asp:ListBox ID="lstidJornadaTrabalho" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                                                <%--<asp:CheckBoxList ID="cblidJornadaTrabalho" runat="server">
                                                                         <asp:ListItem Value="1"> Horário Comercial</asp:ListItem>
                                                                        <asp:ListItem Value="2"> Horário Noturno</asp:ListItem>
                                                                        <asp:ListItem Value="3"> Sábados</asp:ListItem>
                                                                        <asp:ListItem Value="4"> Domingo ou Feriados</asp:ListItem>
                                                                    </asp:CheckBoxList>--%>
                                                            </div>
                                                        </div>

                                                      
                                                    


                                                    </div>
                                                </div>


                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>

                                <asp:UpdatePanel ID="updPanel_Colaborador" runat="server">
                                    <ContentTemplate>
                                        <div class="panel panel-default" runat="server" id="Div_Colaboradores">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>Colaboradores</b></h3>
                                            </div>
                                            <div class="panel-body">
                                                <uc1:MensagemPagina runat="server" ID="MensagemColaboradores" />
                                                <div class="row">
                                                    <div class="col-lg-12">
                                                        <div class="form-group">
                                                            <div class="row" runat="server" id="div_SelecaoItens">

                                                                <div class="col-lg-6">
                                                                    <div class="form-group">
                                                                        <label>Colaborador: *</label>
                                                                        <asp:DropDownList ID="ddlColaborador" class="form-control" runat="server"></asp:DropDownList>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2">
                                                                    <div class="form-group">
                                                                        <label>Previsão de Início </label>
                                                                        <asp:TextBox ID="txtdtPrevisaoInicio" class="form-control CaixaTextoData" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2">
                                                                    <div class="form-group">
                                                                        <label>Previsão de Término </label>
                                                                        <asp:TextBox ID="txtdtPrevisaoTermino" class="form-control CaixaTextoData" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2">
                                                                    <br />
                                                                    <asp:Button ID="cmdIncluirColaborador" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdIncluirColaborador_Click" />
                                                                </div>

                                                                <div class="col-lg-3">
                                                                    <br />

                                                                    <asp:Label ID="lblMensagem_Item" runat="server" Text="" Visible="false" CssClass="label-danger"></asp:Label>
                                                                </div>
                                                            </div>

                                                            <div class="row">
                                                                <div class="col-lg-12">
                                                                    <asp:GridView ID="dtgOSColaboradores" class="table table-striped table-bordered table-hover table-condensed"
                                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgOSColaboradores_RowDataBound" OnRowDeleting="dtgOSColaboradores_RowDeleting">
                                                                        <Columns>



                                                                            <asp:BoundField DataField="idColaborador" HeaderText="Id do Colaborador">
                                                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="sDscColaborador" HeaderText="Colaborador">
                                                                                <ItemStyle Width="40%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="dtPrevisaoInicio" HeaderText="Previsão de Inicio">
                                                                                <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="dtPrevisaoTermino" HeaderText="Previsão de Termino">
                                                                                <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                            </asp:BoundField>


                                                                            <asp:TemplateField HeaderText="">
                                                                                <ItemTemplate>
                                                                                    <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                                                </ItemTemplate>
                                                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </div>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>

                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cmdIncluirColaborador" EventName="Click" />
                                    </Triggers>
                                </asp:UpdatePanel>

                                <asp:UpdatePanel ID="updPanel_Seguranca" runat="server">
                                    <ContentTemplate>
                                        <div class="panel panel-default" runat="server" id="Div2">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>Segurança</b></h3>
                                            </div>
                                            <div class="panel-body">
                                                <uc1:MensagemPagina runat="server" ID="MensagemEPI" />
                                                <div class="row">
                                                    <div class="col-lg-12">
                                                        <div class="form-group">
                                                            <div class="row" runat="server" id="div3">

                                                                <div class="col-lg-4">
                                                                    <div class="form-group">
                                                                        <label>Nome do EPI</label>
                                                                        <asp:DropDownList ID="ddlEPI" class="form-control" runat="server"></asp:DropDownList>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2">
                                                                    <div class="form-group">
                                                                        <label>Quantidade </label>
                                                                        <asp:TextBox ID="txtnQuantidadeEPI" class="form-control" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2">
                                                                    <br />
                                                                    <asp:Button ID="cmdIncluirEPI" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdIncluirEPI_Click" />

                                                                </div>

                                                                <div class="col-lg-3">
                                                                    <br />

                                                                    <asp:Label ID="Label1" runat="server" Text="" Visible="false" CssClass="label-danger"></asp:Label>
                                                                </div>
                                                            </div>

                                                            <div class="row">
                                                                <div class="col-lg-12">
                                                                    <asp:GridView ID="dtgOSEPI" class="table table-striped table-bordered table-hover table-condensed"
                                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgOSEPI_RowDataBound" OnRowDeleting="dtgOSEPI_RowDeleting">
                                                                        <Columns>



                                                                            <asp:BoundField DataField="idEPI" HeaderText="Id do EPI">
                                                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="sDscEPI" HeaderText="EPI">
                                                                                <ItemStyle Width="40%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="nQuantidadeEPI" HeaderText="Quantidade">
                                                                                <ItemStyle Width="30%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                            </asp:BoundField>

                                                                            <asp:TemplateField HeaderText="">
                                                                                <ItemTemplate>
                                                                                    <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                                                </ItemTemplate>
                                                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </div>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>

                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cmdIncluirEPI" EventName="Click" />
                                    </Triggers>
                                </asp:UpdatePanel>

                                <asp:UpdatePanel ID="updPanel_Acomodacao" runat="server">
                                    <ContentTemplate>
                                        <br />
                                        <div class="panel panel-default" runat="server" id="div4">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>Acomodação</b></h3>
                                            </div>
                                            <div class="panel-body">
                                                <uc1:MensagemPagina runat="server" ID="MensagemAcomodacao" />
                                                <div class="form-stacked row">
                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label>Tipo de Acomodação </label>
                                                            <asp:DropDownList ID="ddlTipoAcomodacao" class="form-control" placeholder="Jornada" runat="server" attrname="idTipoAcomodacao"></asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label>Nome Acomodação </label>
                                                            <asp:TextBox ID="txtsNomeAcomodacao" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-12">
                                                        <div class="form-group">
                                                            <label>Endereco Acomodação </label>
                                                            <asp:TextBox ID="txtsEnderecoAcomodacao" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label>Nome Contato Acomodação </label>
                                                            <asp:TextBox ID="txtsNomeContatoAcomodacao" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label>Contato Acomodação </label>
                                                            <asp:TextBox ID="txtsContatoAcomodacao" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-2">
                                                        <br />
                                                        <asp:Button ID="cmdIncluirAcomodacao" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdIncluirAcomodacao_Click" />
                                                    </div>

                                                    <div class="col-lg-3">
                                                        <br />

                                                        <asp:Label ID="Label3" runat="server" Text="" Visible="false" CssClass="label-danger"></asp:Label>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-lg-12">
                                                        <asp:GridView ID="dtgOSAcomodacao" class="table table-striped table-bordered table-hover table-condensed"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgOSAcomodacao_RowDataBound" OnRowDeleting="dtgOSAcomodacao_RowDeleting">
                                                            <Columns>



                                                                <asp:BoundField DataField="idTipoAcomodacao" HeaderText="Id do Tipo da Acomodacao">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sDscTipoAcomodacao" HeaderText="Tipo da Acomodacao">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sNomeAcomodacao" HeaderText="Nome da Acomodacao">
                                                                    <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sEnderecoAcomodacao" HeaderText="Endereco da Acomodacao">
                                                                    <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sNomeContatoAcomodacao" HeaderText="Nome do Contato">
                                                                    <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sContatoAcomodacao" HeaderText="Contato da Acomodacao">
                                                                    <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>

                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cmdIncluirAcomodacao" EventName="Click" />
                                    </Triggers>
                                </asp:UpdatePanel>

                                <asp:UpdatePanel ID="updPanel_Transporte" runat="server">
                                    <ContentTemplate>
                                        <div class="panel panel-default" runat="server" id="Div5">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>Transporte</b></h3>
                                            </div>
                                            <div class="panel-body">
                                                <uc1:MensagemPagina runat="server" ID="MensagemTransporte" />
                                                <div class="form-stacked row">
                                                    <div class="col-lg-12">
                                                        <div class="form-group">
                                                            <label>Tipo de Transporte </label>
                                                            <asp:DropDownList ID="ddlTipoTransporte" class="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlTipoTransporte_SelectedIndexChanged"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">

                                                    <div class="col-lg-12">
                                                        <div class="form-group">
                                                            <div class="row" runat="server" id="DIV_ALUGADO">

                                                                <div class="col-lg-2">
                                                                    <div class="form-group">
                                                                        <label>Marca </label>
                                                                        <asp:TextBox ID="txtsMarca" class="form-control CaixaTextoData" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-4">
                                                                    <div class="form-group">
                                                                        <label>Modelo</label>
                                                                        <asp:TextBox ID="txtsModelo" class="form-control  " runat="server" MaxLength="20" ValidationGroup="Item" AutoPostBack="True" b></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2">
                                                                    <div class="form-group">
                                                                        <label>Placa</label>
                                                                        <asp:TextBox ID="txtsPlaca" class="form-control  " runat="server" MaxLength="20" ValidationGroup="Item" AutoPostBack="True" b></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-2">
                                                                    <div class="form-group">
                                                                        <label>Cor </label>
                                                                        <asp:TextBox ID="txtsCor" class="form-control CaixaTextoData" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-12">
                                                        <div class="form-group">
                                                            <div class="row" runat="server" id="DIV_FROTA">
                                                                <div class="col-lg-12">
                                                                    <div class="form-group">
                                                                        <label>Frota</label>
                                                                        <asp:DropDownList ID="ddlsFrota" class="form-control" runat="server"></asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-12">
                                                        <div class="form-group">
                                                            <div class="row" runat="server" id="DIV_TRANSPORTE">
                                                                <div class="col-lg-12">
                                                                    <div class="form-group">
                                                                        <label>Itinerário</label>
                                                                        <asp:TextBox ID="txtsItinerario" class="form-control" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2">
                                                        <br />
                                                        <asp:Button ID="cmdIncluirTransporte" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdIncluirTransporte_Click" />
                                                    </div>

                                                    <div class="col-lg-3">
                                                        <br />
                                                        <asp:Label ID="Label2" runat="server" Text="" Visible="false" CssClass="label-danger"></asp:Label>
                                                    </div>

                                                </div>

                                                <div class="row">
                                                    <div class="col-lg-12">
                                                        <asp:GridView ID="dtgOSTransporte" class="table table-striped table-bordered table-hover table-condensed"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgOSTransporte_RowDataBound" OnRowDeleting="dtgOSTransporte_RowDeleting">
                                                            <Columns>


                                                                <asp:BoundField DataField="idTipoTransporte" HeaderText="Id do Tipo do Transporte">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sDscTipoTransporte" HeaderText="Tipo de Transporte">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sMarca" HeaderText="Marca">
                                                                    <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sModelo" HeaderText="Modelo">
                                                                    <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sPlaca" HeaderText="Placa">
                                                                    <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sCor" HeaderText="Cor">
                                                                    <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sFrota" HeaderText="Frota">
                                                                    <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sItinerario" HeaderText="Itinerario">
                                                                    <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>
                                                </div>

                                                <triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="cmdIncluirTransporte" EventName="Click" />
                                                </triggers>
                                            </div>
                                        </div>
                                    </ContentTemplate>


                                </asp:UpdatePanel>

                            </ContentTemplate>
                        </asp:UpdatePanel>

                         </asp:UpdatePanel>                   
        

                    <div id="div_OrdemServico_Acoes" class="row">
                        <div class="col-lg-12">
                            <div class="panel panel-primary">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><asp:Label ID="lblOrdemServico_Acao_Titulo" runat="server" Text="Label"></asp:Label></h3>
                                </div>
                                <div class="panel-body">
                                    <div class="col-lg-10" runat="server" id="div_AlterarStatus">
								    <div class="form-group">
                                        <label>Status</label>
                                        <asp:DropDownList ID="ddlAlterarStatus" class="form-control CaixaTextoGrande" runat="server" ></asp:DropDownList>
                                    </div>
                                    </div>
                                    
                                    <div class="col-lg-10">
								    <div class="form-group">
                                        <label>Motivo/Observação</label>
                                        <asp:TextBox ID="txtOrdemServico_ObservacaoStatus" class="form-control" runat="server" TextMode="MultiLine" MaxLength="300" ></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <br />
                                        <asp:Button ID="cmdOrdemServico_Acao_OK" class="btn btn-sm btn-primary"  runat="server" Text="Ok" OnClick="cmdOrdemServico_Acao_OK_Click"   />
                                        <asp:Button ID="cmdOrdemServico_Acao_Cancelar"  class="btn btn-sm btn-danger" runat="server" Text="Cancelar" OnClick="cmdOrdemServico_Acao_Cancelar_Click"  />
                                    </div>
                                </div>
                                
                            
                                </div>
                            </div>
                        </div>
                    </div>

                        <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                                                                        
                        <fieldset class="form-stacked actions">
                            <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click"/>
                            <asp:Button ID="cmdEditar" class="btn  btn-lg  btn-danger" runat="server" Text="Editar" OnClick="cmdEditar_Click"/>
                            <asp:Button ID="cmdAlterarStatus" class="btn  btn-lg  btn-danger" runat="server" Text="Alterar Status" OnClick="cmdAlterarStatus_Click" />

                            <div class="btn-group dropup" runat="server" id="Botoes_Acao">
                                <button id="cmdAcao" runat="server" type="button" class="btn btn-lg btn-success dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><span>Ações</span> <span class="caret"></span></button>
                                <ul class="dropdown-menu">
                                    <li>
                                        <asp:LinkButton ID="lnkIniciarOrdemServico" runat="server" OnClick="lnkIniciarOrdemServico_Click">Iniciar Ordem de Serviço</asp:LinkButton></li>
                                    <li role="separator" class="divider"></li>
                                    <li>
                                        <asp:LinkButton ID="lnkFinalizarOrdemServico" runat="server" OnClick="lnkFinalizarOrdemServico_Click">Finalizar Ordem de Serviço</asp:LinkButton></li>
                                    <li role="separator" class="divider"></li>
                                    <li>
                                        <asp:LinkButton ID="lnkRejeitarOrdemServico" runat="server" OnClick="lnkRejeitarOrdemServico_Click">Rejeitar Ordem de Serviço</asp:LinkButton></li>
                                </ul>
                            </div>

                            <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field-cancel" title="Voltar" onclick="history.go(-1)">
                        </fieldset>

                    </div>

                    <div role="tabpanel" class="tab-pane fade" id="procedimentos" aria-labelledby="procedimentos-tab">
                        <br />
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Procedimentos</b></h3>
                            <div class=" embed-responsive embed-responsive-16by9" style="min-height: 800px">
                                <embed type="text/html" runat="server" id="frmProcedimentos" width="800" height="500" />
                            </div>
                        </div>

                    </div>

                    <div role="tabpanel" class="tab-pane fade" id="arquivos" aria-labelledby="arquivos-tab">
                        <br />
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Arquivos</b></h3>
                            <div class=" embed-responsive embed-responsive-16by9" style="min-height: 800px">
                                <embed type="text/html" runat="server" id="frmArquivos" width="800" height="500" />
                            </div>
                        </div>

                    </div>

                    <div role="tabpanel" class="tab-pane fade" id="historico" aria-labelledby="historico-tab">
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
                                                <asp:BoundField DataField="dtLog" HeaderText="Data">
                                                    <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário">
                                                    <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="sTipoAcao" HeaderText="Tipo">
                                                    <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="sAcao" HeaderText="Ação">
                                                    <ItemStyle HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
            <%--
            <div id="dialog-Salvar" class="modal" title="Salvar">
                <p><span class="ui-icon ui-icon-alert" style="float:left; margin:12px 12px 20px 0;"></span><asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label></p>
            </div>
            <div id="dialog-Editar"  class="modal" title="Editar Pedido">
                <p><span class="ui-icon ui-icon-alert" style="float:left; margin:12px 12px 20px 0;"></span><asp:Label ID="lblTituloEdiar" runat="server" Text="Confirma a Edição da Requisição?"></asp:Label></p>
            </div>
               --%>
            <asp:HiddenField ID="hddidOrdemServico" runat="server" />
            <asp:HiddenField ID="hddidCliente" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
