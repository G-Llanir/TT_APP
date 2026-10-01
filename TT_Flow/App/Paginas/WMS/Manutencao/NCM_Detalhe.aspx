<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="NCM_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.Manutencao.NCM_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        .taglist a:hover {
            text-decoration: none;
        }
    </style>

    <div id="hdd">
        <asp:HiddenField ID="hddidNCM" runat="server" />
    </div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="3" TitulodaPagina="Detalhe" />
        </div>
    </div>

    <div id="abas">
        <ul id="tab_NCM" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="tabpanel active">
                <a href="#NCM" id="aba_NCM" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>NCM</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_Historico">
                <a href="#historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="Historico"><b>Histórico</b></a>
            </li>
        </ul>
    </div>

    <br />

    <div id="tab" class="tab-content">
        <div role="tabpanel" class="tab-pane fade in active" id="NCM" aria-labelledby="NCM-tab">

            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>NCM</b></h3>
                </div>
                <div class="panel-body">
                    <div class="row">

                        <div class="col-lg-12">
                            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                        </div>

                        <div id="div_NCM" class="col-lg-12">

                            <div class="row">
                                <div class="col-lg-1 form-group">
                                    <label>ID</label>
                                    <asp:TextBox ID="txtidNCM" runat="server" class="form-control" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-lg-2 form-group">
                                    <label>Código NCM</label>

                                    <asp:TextBox
                                        ID="txtsCodigoNCM"
                                        runat="server"
                                        class="form-control"
                                        MaxLength="10">
                                    </asp:TextBox>

                                    <asp:RegularExpressionValidator
                                        ID="revCodigoNCM"
                                        runat="server"
                                        ControlToValidate="txtsCodigoNCM"
                                        ValidationExpression="^\d{4}\.\d{2}\.\d{2}$"
                                        ErrorMessage="O código NCM deve conter 8 números no formato 0000.00.00."
                                        ForeColor="Red"
                                        Display="Dynamic"
                                        SetFocusOnError="true"
                                        ValidationGroup="SalvarNCM">
                                    </asp:RegularExpressionValidator>
                                </div>

                                <div class="col-lg-2 CaixaTextoPequeno form-group">
                                    <label>Antidamping</label>
                                    <asp:DropDownList ID="ddlAntidamping" runat="server" MaxLength="10" class="form-control">
                                        <asp:ListItem Value="S">Sim</asp:ListItem>
                                        <asp:ListItem Value="N">Não</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-lg-2 CaixaTextoMedio form-group">
                                    <label>Licença de Importação</label>
                                    <asp:DropDownList ID="ddlLicencaImportacao" runat="server" MaxLength="10" class="form-control">
                                        <asp:ListItem Value="Spre">Sim, pré embarque</asp:ListItem>
                                        <asp:ListItem Value="Spos">Sim, pós embarque</asp:ListItem>
                                        <asp:ListItem Value="N">Não</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-lg-2 form-group">
                                    <uc1:SwitchAtivo runat="server" ID="Switch_ReducaoBC" />
                                </div>

                                <div class="col-lg-12 form-group">
                                    <label>Descricao NCM</label>
                                    <asp:TextBox ID="txtsDscNCM" runat="server" class="form-control CaixaTextoObservacao" TextMode="MultiLine" Height="150px" MaxLength="800"></asp:TextBox>
                                </div>
                            </div>

                            <div id="div_CEST" class="row">
                                <asp:UpdatePanel ID="UpdCEST" runat="server">
                                    <ContentTemplate>

                                        <div class="col-lg-2 form-group">
                                            <label>CEST Principal</label>
                                            <i class="fa fa-question-circle" data-toggle="tooltip_top" title="O CEST definido neste campo, será aplicado em todos os Produtos que possuem este NCM."></i>
                                            <asp:DropDownList ID="ddlCEST_Principal" runat="server" class="form-control Caixa_Selecao">
                                                <asp:ListItem Text="Não Classificado" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-lg-10 form-group">
                                            <label>CEST</label>
                                            <asp:ListBox ID="lstCEST" runat="server" class="form-control Caixa_Selecao" SelectionMode="Multiple" AutoPostBack="true" OnSelectedIndexChanged="lstCEST_SelectedIndexChanged"></asp:ListBox>
                                        </div>

                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>

                            <div class="row">
                                <div class="col-lg-2">
                                    <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
                                </div>
                            </div>

                        </div>

                        <div id="div_Impostos" class="col-lg-12">
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>Impostos</b></h3>
                                </div>
                                <div class="panel-body">
                                    <div class="table-responsive">
                                        <asp:Table runat="server" ID="tbImpostos" CellPadding="10" GridLines="Both" HorizontalAlign="Center" Width="100%" class="table table-striped table-bordered table-hover m-0"
                                            CellSpacing="1" AutoGenerateColumns="False" ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">

                                            <asp:TableHeaderRow>
                                                <asp:TableHeaderCell></asp:TableHeaderCell>
                                                <asp:TableHeaderCell>II (%)</asp:TableHeaderCell>
                                                <asp:TableHeaderCell>IPI (%)</asp:TableHeaderCell>
                                                <asp:TableHeaderCell>PIS (%)</asp:TableHeaderCell>
                                                <asp:TableHeaderCell>COFINS (%)</asp:TableHeaderCell>
                                                <asp:TableHeaderCell>ICMS (%)</asp:TableHeaderCell>
                                            </asp:TableHeaderRow>

                                            <asp:TableRow runat="server" ID="rowNacional">
                                                <asp:TableCell><b>Nacional</b></asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:TextBox runat="server" ID="txtnII" class="form-control"></asp:TextBox>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:TextBox runat="server" ID="txtnIPI" class="form-control"></asp:TextBox>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:TextBox runat="server" ID="txtnPIS" class="form-control"></asp:TextBox>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:TextBox runat="server" ID="txtnCOFINS" class="form-control"></asp:TextBox>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:TextBox runat="server" ID="txtnICMS" class="form-control"></asp:TextBox>
                                                </asp:TableCell>
                                            </asp:TableRow>

                                            <asp:TableRow runat="server" ID="rowInternacional">
                                                <asp:TableCell><b>Internacional</b></asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:TextBox runat="server" ID="txtnII_Internacional" class="form-control"></asp:TextBox>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:TextBox runat="server" ID="txtnIPI_Internacional" class="form-control"></asp:TextBox>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:TextBox runat="server" ID="txtnPIS_Internacional" class="form-control"></asp:TextBox>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:TextBox runat="server" ID="txtnCOFINS_Internacional" class="form-control"></asp:TextBox>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:TextBox runat="server" ID="txtnICMS_Internacional" class="form-control"></asp:TextBox>
                                                </asp:TableCell>
                                            </asp:TableRow>

                                        </asp:Table>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div id="div_RegrasFiscais" class="col-lg-12">
                            <div class="panel panel-default m-0">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>Regras Fiscais</b></h3>
                                </div>
                                <div class="panel-body">

                                    <asp:UpdatePanel ID="UpdRegrasFiscais" runat="server">
                                        <ContentTemplate>

                                            <asp:HiddenField ID="hddidRegra_Editar" runat="server" Value="0" />

                                            <uc1:MensagemPagina runat="server" ID="MensagemPagina_Regras" />

                                            <asp:LinkButton runat="server" ID="cmd_div_IncluirRegra" data-toggle="collapse" href="#div_IncluirRegra" aria-expanded="false" CssClass="btn btn-info"><i class="fa fa-plus"></i> Incluir Regra Fiscal</asp:LinkButton>
                                            <asp:LinkButton runat="server" ID="cmdModal_Historico_Regras" data-toggle="modal" data-target="#Modal_Historico_Regras" CssClass="btn btn-primary float-r"><i class="fa fa-clock-o"></i> Histórico das Regras</asp:LinkButton>

                                            <div id="div_IncluirRegra" class="panel panel-default collapse" style="margin-top: 15px; margin-bottom: 0;">
                                                <div class="panel-body">
                                                    <div class="row">

                                                        <div class="col-lg-2 form-group">
                                                            <label>Data Inicial</label>
                                                            <asp:TextBox runat="server" ID="txtdtInicial" CssClass="form-control" TextMode="DateTimeLocal"></asp:TextBox>
                                                        </div>

                                                        <div class="col-lg-2 form-group">
                                                            <label>Data Final</label>
                                                            <asp:TextBox runat="server" ID="txtdtFinal" CssClass="form-control" TextMode="DateTimeLocal"></asp:TextBox>
                                                        </div>

                                                        <div class="col-lg-2 form-group">
                                                            <label>UF Origem</label>
                                                            <asp:DropDownList runat="server" ID="ddlOrigem" CssClass="form-control Caixa_Selecao"></asp:DropDownList>
                                                        </div>

                                                        <div class="col-lg-2 form-group">
                                                            <label>UF Destino</label>
                                                            <asp:DropDownList runat="server" ID="ddlDestino" CssClass="form-control Caixa_Selecao"></asp:DropDownList>
                                                        </div>

                                                        <div class="col-lg-2 form-group">
                                                            <label>% ICMS</label>
                                                            <asp:TextBox runat="server" ID="txtnICMS_Regra" CssClass="form-control"></asp:TextBox>
                                                        </div>

                                                        <div runat="server" id="div_cmdCancelarEdicao" class="col-lg-2">
                                                            <label>&nbsp;</label>
                                                            <asp:LinkButton runat="server" ID="cmdCancelarEdicao" CssClass="btn btn-danger form-control" OnClick="cmdCancelarEdicao_Click" Text="Cancelar Edição"></asp:LinkButton>
                                                        </div>

                                                        <div class="col-lg-10">
                                                            <label>Observação</label>
                                                            <asp:TextBox runat="server" ID="txtObs" CssClass="form-control"></asp:TextBox>
                                                        </div>

                                                        <div class="col-lg-2">
                                                            <label>&nbsp;</label>
                                                            <asp:LinkButton runat="server" ID="cmdIncluirRegra" CssClass="btn btn-success form-control" OnClick="cmdIncluirRegra_Click" Text="Incluir Regra"></asp:LinkButton>
                                                        </div>

                                                    </div>
                                                </div>
                                            </div>

                                            <div runat="server" id="div_gvRegrasFiscais" class="table-responsive" style="margin-top: 15px;">
                                                <asp:GridView ID="gvRegrasFiscais" class="table table-striped table-bordered table-hover table-condensed table-responsive m-0"
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" ShowFooter="False"
                                                    OnRowDataBound="gvRegrasFiscais_RowDataBound" OnRowCommand="gvRegrasFiscais_RowCommand">
                                                    <Columns>

                                                        <asp:BoundField DataField="idRegra" HeaderText="ID">
                                                            <HeaderStyle CssClass="invisivel" />
                                                            <ItemStyle Width="0%" CssClass="invisivel" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sObs" HeaderText="Observação">
                                                            <ItemStyle Width="35%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sDscEstadoOrigem" HeaderText="Estado de Origem">
                                                            <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sDscEstadoDestino" HeaderText="Estado de Destino">
                                                            <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="nICMS" HeaderText="% ICMS" DataFormatString="{0:N2} %">
                                                            <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="dtInicial" HeaderText="Data Inicial" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}">
                                                            <ItemStyle Width="13%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="dtFinal" HeaderText="Data Final" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}">
                                                            <ItemStyle Width="13%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <div class="d-flex fw-no space-e">
                                                                    <asp:LinkButton runat="server" ID="cmdEditar" CommandName="Editar" CommandArgument='<%# Eval("idRegra") %>' data-toggle="tooltip_top" title="Editar"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                                    <asp:LinkButton runat="server" ID="cmdExcluir" CommandName="Excluir" CommandArgument='<%# Eval("idRegra") %>' data-toggle="tooltip_top" title="Excluir"><i class="fa fa-eraser"></i></asp:LinkButton>
                                                                </div>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                    </Columns>
                                                </asp:GridView>
                                            </div>

                                        </ContentTemplate>
                                    </asp:UpdatePanel>

                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>

        </div>

        <div role="tabpanel" class="tab-pane fade" id="historico" aria-labelledby="historico-tab">

            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Histórico</b></h3>
                </div>
                <div class="panel-body">
                    <div class="form-stacked row">
                        <div class="col-lg-12 table-responsive">
                            <asp:GridView
                                ID="gv_Historico" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                ShowFooter="False" OnRowDataBound="gv_Historico_RowDataBound">
                                <Columns>

                                    <asp:BoundField DataField="idHistorico" HeaderText="ID">
                                        <HeaderStyle CssClass="invisivel" />
                                        <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="invisivel" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sDscAlteracao" HeaderText="Alteração">
                                        <ItemStyle Width="80%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sDscUsuarioAlteracao" HeaderText="Alterado Por">
                                        <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="dtHistorico" HeaderText="Em">
                                        <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>

    <div class="modal fade" id="Modal_Historico_Regras">
        <div class="modal-dialog modal-grande">
            <div class="modal-content">

                <div class="modal-header">
                    <div class="modal-title">
                        <img class="modal-logo" src="../../../img/instrucaoTecnica_pdf.jpg" alt="Logo TT" />
                        <label>Histórico das Regras</label>
                        <button type="button" id="btnFechar" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span></button>
                    </div>
                </div>

                <div class="modal-body">

                    <asp:UpdatePanel ID="UpdModal_Historico_Regras" runat="server">
                        <ContentTemplate>

                            <asp:HiddenField ID="hddFiltro_Historico" runat="server" />

                            <div runat="server" id="divFiltro_Historico" class="panel panel-default">
                                <div class="panel-heading">
                                    <label class="panel-title">Filtro do Histórico por Regra</label>
                                </div>
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-lg-12 form-group">
                                            <asp:Literal runat="server" ID="litFiltro_Historico" />
                                        </div>
                                        <div class="col-lg-12">
                                            <asp:LinkButton runat="server" ID="cmdFiltro_Historico" CssClass="btn btn-primary" OnClick="cmdFiltro_Historico_Click" Text="Filtrar Regras selecionadas"></asp:LinkButton>
                                            <span class='btn btn-danger cursor-pointer chkLimpa_Filtro_Historico'><i class='fa fa-ban'></i>&nbsp;Limpar Filtros</span>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <asp:GridView ID="gvHistorico_Regras" class="table table-striped table-bordered table-hover table-condensed table-responsive m-0"
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" ShowFooter="False">
                                <Columns>

                                    <asp:BoundField DataField="idRegra_NCM" HeaderText="ID">
                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sDscAlteracao" HeaderText="Alteração" HtmlEncode="false">
                                        <ItemStyle Width="75%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="dtHistorico" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}">
                                        <ItemStyle Width="20%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                </Columns>
                            </asp:GridView>

                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>

            </div>
        </div>
    </div>

    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

    <fieldset class="form-stacked actions">
        <asp:Button ID="cmdSalvar"
            class="btn btn-lg btn-success"
            runat="server"
            Text="Salvar"
            ValidationGroup="SalvarNCM"
            OnClick="cmdSalvar_Click" />

        <input type="submit" name="cancel" class="btn btn-lg btn-warning" id="field-cancel" value="Cancelar" title="Voltar" onclick="history.go(-1)" />
    </fieldset>

</asp:Content>
