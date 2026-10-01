<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Avaliacao_Resultado.aspx.cs" Inherits="TT_Hub.App.Paginas.RRHH.Avaliacao_Resultado" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Import Namespace="System.Data" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .gvChildHeader2 {
            padding: 4px;
            background-color: rgba(0, 180, 0) !important;
            color: white !important;
            border: 1px solid #bbb;
            font-weight: bold;
        }

        .collapsed-row {
            display: none;
            padding: 1px;
            margin: 1px;
        }

        .gvMainTh {
            padding: 4px;
            background-color: rgba(0, 125, 0) !important;
            color: white !important;
            border: 1px solid #bbb;
            font-weight: bold;
        }

        .gvMainTd {
            padding: 7px;
            border: 1px solid #bbb;
            background-color: #ffffff;
        }

            .gvMainTd tbody tr:hover td {
                background-color: #e0e0e0;
            }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">


    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <script type="text/javascript">
                function MudarBotao() {
                    document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
                }
            </script>

            <style>
                modal {
                    position: fixed;
                    top: 0;
                    right: 0;
                    bottom: 0;
                    left: 0;
                    z-index: 2;
                    display: none;
                    overflow: auto;
                    overflow-y: scroll;
                    -webkit-overflow-scrolling: touch;
                    outline: 0;
                }
            </style>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>
                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Top" />
                </div>
                <div class="col-lg-12" id="DIV_PESQUISA" runat="server">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa </h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group">

                                
                                    <div class="col-lg-1">
                                        <label>&nbsp;</label>
                                        <asp:TextBox ID="txtsReferencia" class="form-control" runat="server" MaxLength="7" placeholder="MM/AAAA"></asp:TextBox>
                                    </div>
                                    <div class="col-lg-11" style="padding: 0px;">
                                        <div class="col-lg-3" id="div_Selecao_Empresa" runat="server">
                                            <div class="form-group">
                                                <label>Empresa</label>
                                                <asp:ListBox ID="lstEmpresa" class="form-control Caixa_Selecao" SelectionMode="Multiple" runat="server"></asp:ListBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-3" id="div_Selecao_TipoContrato" runat="server">
                                            <div class="form-group">
                                                <label>Tipo Contrato</label>
                                                <asp:ListBox ID="lstTipoContrato" class="form-control Caixa_Selecao" SelectionMode="Multiple" runat="server"></asp:ListBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-3" id="div_Selecao_Cargo" runat="server">
                                            <div class="form-group">
                                                <label>Cargo</label>
                                                <asp:ListBox ID="lstCargo" class="form-control Caixa_Selecao" SelectionMode="Multiple" runat="server"></asp:ListBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-3" id="div_Selecao_Departamento" runat="server">
                                            <div class="form-group">
                                                <label>Departamento</label>
                                                <asp:ListBox ID="lstDepartamento" class="form-control Caixa_Selecao" SelectionMode="Multiple" runat="server"></asp:ListBox>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-2" id="div_Selecao_TempoContrato" runat="server">
                                        <label>&nbsp;</label>
                                        <asp:DropDownList ID="ddlnTipo_dtInicioContrato" runat="server" class="form-control Caixa_Selecao">
                                            <asp:ListItem Value="0" Selected="True">Todos</asp:ListItem>
                                            <asp:ListItem Value="1">Mais de um ano de contrato</asp:ListItem>
                                            <asp:ListItem Value="2">Menos de um ano de contrato</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>

                                    <div class="col-lg-3" id="div_Selecao_Supervisores" runat="server">
                                        <label>Supervisor Direto</label>
                                        <asp:ListBox ID="lstSupervisorDireto" class="form-control Caixa_Selecao" SelectionMode="Multiple" runat="server"></asp:ListBox>
                                    </div>

                                    <div class="col-lg-3" id="div_Selecao_Status" runat="server">
                                        <label>Status</label>
                                        <asp:ListBox ID="lstStatus" class="form-control Caixa_Selecao" SelectionMode="Multiple" runat="server"></asp:ListBox>
                                    </div>

                                    <div class="col-lg-4" id="div_Selecao_Pesquisa" runat="server">
                                        <label>&nbsp;</label>
                                        <asp:TextBox ID="txtPesquisa" class="form-control " placeholder="Pesquisar" runat="server" MaxLength="200"></asp:TextBox>
                                    </div>

                                    <div class="col-lg-4" style="margin-top: 24px">
                                        <div class="form-group">
                                            <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Consultar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                            <asp:Button ID="cmdVoltar" class="btn btn-info" runat="server" Text="Voltar" OnClick="cmdVoltar_Click" />
                                            <asp:Button ID="cmdSelecao_Gerar" class="btn btn-success" runat="server" Text="Gerar Avaliações" OnClick="cmdSelecao_Gerar_Click" />
                                            <asp:Button ID="cmdGerarAvaliacoes" class="btn btn-success" runat="server" Text="Gerar Avaliações" />
                                        </div>
                                    </div>
                                
                            </div>

                        </div>
                    </div>
                </div>
            </div>


            <asp:Panel ID="pnResultado_Grupo" class="" runat="server">
                <div class="col-lg-2"></div>
                <div class="col-lg-8" id="Resultado_Grupo" align="center">
                    <div class="panel panel-primary">
                        <div class="panel-body">
                            <div class="table-responsive">

                                <asp:GridView ID="gvResultado_Grupo" class="table table-striped table-bordered table-hover" DataKeyNames="sIdReferencia, sReferencia"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" OnRowCommand="dtgvConsulta_RowCommand"
                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvResultado_Grupo_RowDataBound">
                                    <Columns>

                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnToggle" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-div-id='<%# "tr_1_grupo" + string.Format("{0}", Eval("sIdReferencia")) %>'></asp:LinkButton>
                                            </ItemTemplate>
                                            <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="sDscGrupo" HeaderText="Grupo">
                                            <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sReferencia" HeaderText="Referência">
                                            <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:TemplateField HeaderText="">
                                            <ItemTemplate>
                                                <%# NovaLinha(string.Format("{0}", Eval("sIdReferencia")), "_1_grupo") %>

                                                <asp:GridView ID="gvResultado_Grupo_Detalhe" runat="server" Width="100%" class="table table-striped table-bordered table-hover tablePai"
                                                    GridLines="None" AutoGenerateColumns="false" Style="padding: 0; margin: 0" DataKeyNames="sIdReferencia, sReferencia" OnRowCommand="dtgvConsulta_RowCommand"
                                                    AlternatingRowStyle-CssClass="gvAltRow" OnRowDataBound="gvResultado_Grupo_Detalhe_RowDataBound">
                                                    <Columns>

                                                        <asp:TemplateField HeaderText="Grupo" ItemStyle-Width="40%">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="btnGrupo" runat="server"
                                                                    Text='<%# Eval("sDscGrupo") %>'
                                                                    CommandName="CONSULTAR_AVALIACAO_GRUPO"
                                                                    CommandArgument='<%# Eval("sDscGrupo") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Quantidade" ItemStyle-Width="18%">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="btnQuantidade" runat="server"
                                                                    Text='<%# Eval("nQuantidade") %>'
                                                                    CommandName="CONSULTAR_AVALIACAO_GRUPO"
                                                                    CommandArgument='<%# Eval("sDscGrupo") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                    </Columns>
                                                </asp:GridView>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>

                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-lg-2"></div>
            </asp:Panel>

            <asp:Panel ID="pnResultado_Detalhe" class="" runat="server">
                <div id="resultado_Detalhe">
                    <div class="panel panel-primary">
                        <div class="panel-body">
                            <div class="table-responsive">

                                <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" OnRowCommand="dtgvConsulta_RowCommand"
                                    Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                    <Columns>

                                        <asp:BoundField DataField="idRegistro" HeaderText="ID">
                                            <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:ButtonField DataTextField="idRegistro" HeaderText="ID" CommandName="CONSULTAR_AVALIACAO_APLICADA" ItemStyle-Width="5%" />

                                        <asp:ButtonField DataTextField="sReferencia" HeaderText="Referência" CommandName="CONSULTAR_AVALIACAO_APLICADA" ItemStyle-Width="5%" />

                                        <asp:ButtonField DataTextField="sDscAvaliacao" HeaderText="Avaliação Utilizada" CommandName="CONSULTAR_AVALIACAO_APLICADA" ItemStyle-Width="15%" />

                                        <asp:ButtonField DataTextField="sDscColaborador" HeaderText="Colaborador" CommandName="CONSULTAR_AVALIACAO_APLICADA" ItemStyle-Width="18%" />

                                        <asp:ButtonField DataTextField="sDscDepartamento" HeaderText="Departamento" CommandName="CONSULTAR_AVALIACAO_APLICADA" ItemStyle-Width="14%" />

                                        <asp:ButtonField DataTextField="sDscSupervisorDireto" HeaderText="Supervisor" CommandName="CONSULTAR_AVALIACAO_APLICADA" ItemStyle-Width="12%" />

                                        <asp:BoundField DataField="sDscStatus" HeaderText="Status">
                                            <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="dtInicio" HeaderText="Inicio">
                                            <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="dtFim" HeaderText="Fim">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                    </Columns>
                                </asp:GridView>

                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnResultado_Gerar" class="" runat="server">
                <div class="panel panel-primary">
                    <div class="panel-body">

                        <div class="table-responsive">

                            <asp:GridView ID="gvResultado_Gerar" class="table table-striped table-bordered table-hover" DataKeyNames="idColaborador"
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                <Columns>

                                    <asp:TemplateField HeaderText="">
                                        <HeaderTemplate>
                                            <div style="text-align: center;">
                                                <asp:CheckBox ID="chkSelectAll" runat="server" AutoPostBack="false" Checked="true" />
                                            </div>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSelecionado" runat="server" Checked="true" />
                                        </ItemTemplate>
                                        <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="sDscDepartamento" HeaderText="Departamento">
                                        <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscColaborador" HeaderText="Colaborador">
                                        <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sEmailPessoal" HeaderText="E-mail">
                                        <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDtInicioContrato" HeaderText="Inicio">
                                        <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscEmpresa" HeaderText="Empresa">
                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscCargo" HeaderText="Cargo">
                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscTipoContrato" HeaderText="Tipo Contrato">
                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>


                                </Columns>
                            </asp:GridView>

                        </div>
                    </div>
                </div>
                </div>
            </asp:Panel>


            <asp:Panel ID="pnMensagem" runat="server">
            </asp:Panel>


            <%--------------------------------------MODAL AVALIACAO DETALHE---------------------------------------------------------------------------------------%>

            <asp:HiddenField ID="hdd_sDscGrupo" runat="server" />
            <asp:HiddenField ID="hdd_sFuncao" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="modal fade" id="modalAvaliacaoDetalhe" tabindex="-2" role="dialog" data-backdrop="static" aria-labelledby="modalAvaliacaoDetalhe" aria-hidden="true">
        <div class="modal-dialog modal-sm" role="document" style="width: 60%">
            <div class="modal-content">
                <asp:UpdatePanel ID="updModalDetalhe" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" id="btnHeadFecharDetalhe" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <div class="modal-header-content">
                                <div class="modal-logo">
                                    <asp:Image ID="imgLogoModal" runat="server" ImageUrl="~/App/img/instrucaoTecnica_pdf.jpg" AlternateText="Logo TT" />
                                </div>

                                <div class="modal-title-container">
                                    <h4 class="modal-title">
                                        <asp:Label ID="Detalhe_lblTitulo" runat="server" Text=""></asp:Label>
                                    </h4>
                                </div>
                            </div>

                            <div class="modal-body">
                                <uc1:MensagemPagina runat="server" ID="MensagemPagina_Modal" />

                                <div>
                                    <ul id="tab_Avaliacao_Detalhe" class="nav nav-tabs" role="tablist">
                                        <li role="presentation" class="tabpanel active">
                                            <a href="#Avaliacao_Dados" id="detalhe-tab" role="tab" data-toggle="tab" aria-controls="Avaliacao_Dados" aria-expanded="false"><b>Dados</b></a>
                                        </li>

                                        <li role="presentation" runat="server" id="aba_Avaliacao_AAP">
                                            <a href="#Avaliacao_AAP" role="tab" id="Avaliacao_AAP-tab" data-toggle="tab" aria-controls="Avaliacao_AAP"><b>AAP</b></a>
                                        </li>

                                        <li role="presentation" runat="server" id="aba_Avaliacao_ASU">
                                            <a href="#Avaliacao_ASU" role="tab" id="Avaliacao_ASU-tab" data-toggle="tab" aria-controls="Avaliacao_ASU"><b>ASU</b></a>
                                        </li>

                                        <li role="presentation" runat="server" id="aba_Avaliacao_ASD">
                                            <a href="#Avaliacao_ASD" role="tab" id="Avaliacao_ASD-tab" data-toggle="tab" aria-controls="Avaliacao_ASD"><b>ASD</b></a>
                                        </li>

                                        <li role="presentation" runat="server" id="aba_Resultado">
                                            <a href="#Resultado" role="tab" id="Resultado-tab" data-toggle="tab" aria-controls="Resultado"><b>Resultado</b></a>
                                        </li>
                                    </ul>
                                </div>


                                <div id="tab" class="tab-content">

                                    <div role="tabpanel" class="tab-pane fade in active" id="Avaliacao_Dados" aria-labelledby="Detalhe">
                                        <br />
                                        <div class="panel panel-default" runat="server" id="div_Detalhe1">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>Detalhe</b></h3>
                                            </div>
                                            <div class="panel-body ">
                                                <div class="form-stacked row">
                                                    <div class="form-group">
                                                        <div class="col-lg-2 form-group">
                                                            <label>ID</label>
                                                            <asp:TextBox ID="Detalhe_txtidRegistro" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                        <div class="col-lg-3 form-group">
                                                            <label>Referência</label>
                                                            <asp:TextBox ID="Detalhe_txtsReferencia" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                        <div class="col-lg-4 form-group">
                                                            <label>Status</label>
                                                            <asp:TextBox ID="Detalhe_txtsDscStatus" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                        <div class="form-group col-lg-12">
                                                            <label>Colaborador</label>
                                                            <asp:TextBox ID="Detalhe_txtsDscColaborador" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                        <div class="form-group col-lg-6">
                                                            <label>Departamento</label>
                                                            <asp:TextBox ID="Detalhe_txtsDscDepartamento" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                        <div class="form-group col-lg-6">
                                                            <label>Supervisor</label>
                                                            <asp:TextBox ID="Detalhe_txtsDscSupervisor" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="panel panel-default" runat="server" id="Detalhe_divObservacao">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>Observações</b></h3>
                                            </div>
                                            <div class="panel-body ">
                                                <div class="form-stacked row">
                                                    <div class="form-group">
                                                        <div class="col-lg-12 form-group">
                                                            <asp:TextBox ID="Detalhe_txtsObservacao" class="form-control" TextMode="MultiLine" MaxLength="2000" Height="200px" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div role="tabpanel" class="tab-pane fade" id="Avaliacao_AAP" aria-labelledby="AAP">
                                        <br />
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>AAP</b></h3>
                                            </div>
                                            <div class="panel-body">
                                                <div class="form-group col-lg-7">
                                                    <label>Colaborador</label>
                                                    <asp:TextBox ID="AAP_txtsDscColaborador" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                                <div class="form-group col-lg-5">
                                                    <label>Departamento</label>
                                                    <asp:TextBox ID="AAP_txtsDscDepartamento" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                                <div class="col-lg-6 form-group">
                                                    <label>Avaliação utilizada</label>
                                                    <asp:TextBox ID="AAP_txtsDscAvaliacao" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                                <div class="form-group col-lg-3">
                                                    <label>Inicio</label>
                                                    <asp:TextBox ID="AAP_txtInicio" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                                <div class="form-group col-lg-3">
                                                    <label>Término</label>
                                                    <asp:TextBox ID="AAP_txtFim" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                                <div class="form-group col-lg-12">
                                                    <asp:Repeater ID="AAP_rptAvaliacao_Grupo" runat="server">
                                                        <ItemTemplate>
                                                            <div class="panel panel-primary">
                                                                <div class="panel-body">
                                                                    <div class="well bg-danger" runat="server" id="caixaTitulo">
                                                                        <h4><b>
                                                                            <asp:Label runat="server" Text='<%# Eval("sGrupo") %>'></asp:Label>
                                                                        </b></h4>
                                                                    </div>


                                                                    <asp:Repeater ID="AAP_rptAvaliacao_Respostas" DataSource='<%# ((DataRowView)Container.DataItem).Row.GetChildRows("AAP_Relacionamento") %>' runat="server">
                                                                        <ItemTemplate>

                                                                            <div class="panel panel-default">
                                                                                <div class="panel-heading">
                                                                                    <h4 class="panel-title"><b>
                                                                                        <asp:Label runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "[\"sDscPergunta\"]")%>'></asp:Label>
                                                                                    </b></h4>
                                                                                </div>
                                                                                <div class="panel-body" style="min-height: 50px">
                                                                                    <div class="form-stacked">
                                                                                        <label>
                                                                                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "[\"sResposta\"]")%>'></asp:Label>
                                                                                        </label>
                                                                                        </br>
                                                                        <asp:Label runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "[\"sJustificativa\"]")%>'></asp:Label>
                                                                                    </div>
                                                                                </div>
                                                                            </div>

                                                                        </ItemTemplate>
                                                                    </asp:Repeater>

                                                                </div>
                                                            </div>


                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>

                                            </div>
                                        </div>

                                    </div>

                                    <div role="tabpanel" class="tab-pane fade" id="Avaliacao_ASU" aria-labelledby="ASU">
                                        <br />
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>ASU</b></h3>
                                            </div>
                                            <div class="panel-body">
                                                <div id="ASU_DIV_DADOS" runat="server">
                                                    <div class="form-group col-lg-7">
                                                        <label>Colaborador</label>
                                                        <asp:TextBox ID="ASU_txtsDscColaborador" class="form-control" runat="server"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group col-lg-5">
                                                        <label>Departamento</label>
                                                        <asp:TextBox ID="ASU_txtsDscDepartamento" class="form-control" runat="server"></asp:TextBox>
                                                    </div>
                                                    <div class="col-lg-6 form-group">
                                                        <label>Supervisor</label>
                                                        <asp:TextBox ID="ASU_txtsDscSupervisorDireto" class="form-control" runat="server"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group col-lg-3">
                                                        <label>Inicio</label>
                                                        <asp:TextBox ID="ASU_txtdtInicio" class="form-control" runat="server"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group col-lg-3">
                                                        <label>Término</label>
                                                        <asp:TextBox ID="ASU_txtdtFim" class="form-control" runat="server"></asp:TextBox>
                                                    </div>

                                                    <div class="form-group col-lg-12" id="ASU_DivBotaoResponde" runat="server">
                                                        <asp:Button ID="ASU_cmdResponder" CssClass="btn-success btn" runat="server" OnClick="ASU_cmdResponder_Click" Text="Responder" />
                                                    </div>
                                                </div>

                                                <div id="ASU_DIV_RESPONDER" runat="server">
                                                    <asp:Button ID="ASU_cmdFechar" CssClass="btn-info btn" runat="server" OnClick="ASU_cmdFechar_Click" Text="Encerrar Avaliação" />
                                                    <div class="col-lg-12  embed-responsive embed-responsive-16by9" style="min-height: 1000px" runat="server" id="ASU_Div_BlocoResposta">
                                                        <embed type="text/html" runat="server" id="ASU_frmResponder" />
                                                    </div>
                                                </div>
                                                <div class="form-group col-lg-12">
                                                    <asp:Repeater ID="ASU_rptAvaliacao" runat="server">
                                                        <ItemTemplate>
                                                            <div class="panel panel-primary">
                                                                <div class="panel-body">
                                                                    <div class="well bg-danger">
                                                                        <b>
                                                                            <asp:Label runat="server" Text='<%# Eval("sGrupo") %>'></asp:Label>
                                                                        </b>
                                                                    </div>


                                                                    <asp:Repeater ID="ASU_rptAvaliacao_Respostas" DataSource='<%# ((DataRowView)Container.DataItem).Row.GetChildRows("ASU_Relacionamento") %>' runat="server">
                                                                        <ItemTemplate>

                                                                            <div class="panel panel-default">
                                                                                <div class="panel-heading">
                                                                                    <h3 class="panel-title"><b>
                                                                                        <asp:Label runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "[\"sDscPergunta\"]")%>'></asp:Label>
                                                                                    </b></h3>
                                                                                </div>
                                                                                <div class="panel-body" style="min-height: 50px">
                                                                                    <div class="form-stacked">
                                                                                        <label>
                                                                                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "[\"sResposta\"]")%>'></asp:Label>
                                                                                        </label>
                                                                                        </br>
                                                                        <asp:Label runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "[\"sJustificativa\"]")%>'></asp:Label>
                                                                                    </div>
                                                                                </div>
                                                                            </div>

                                                                        </ItemTemplate>
                                                                    </asp:Repeater>

                                                                </div>
                                                            </div>


                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>

                                            </div>
                                        </div>

                                    </div>


                                    <div role="tabpanel" class="tab-pane fade" id="Avaliacao_ASD" aria-labelledby="ASD">
                                        <br />
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>ASD</b></h3>
                                            </div>
                                            <div class="panel-body">
                                                <div id="ASD_DIV_DADOS" runat="server">
                                                    <div class="form-group col-lg-7">
                                                        <label>Colaborador</label>
                                                        <asp:TextBox ID="ASD_txtsDscColaborador" class="form-control" runat="server"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group col-lg-5">
                                                        <label>Departamento</label>
                                                        <asp:TextBox ID="ASD_txtsDscDepartamento" class="form-control" runat="server"></asp:TextBox>
                                                    </div>
                                                    <div class="col-lg-6 form-group">
                                                        <label>Supervisor</label>
                                                        <asp:TextBox ID="ASD_txtsDscSupervisorDireto" class="form-control" runat="server"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group col-lg-3">
                                                        <label>Inicio</label>
                                                        <asp:TextBox ID="ASD_txtdtInicio" class="form-control" runat="server"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group col-lg-3">
                                                        <label>Término</label>
                                                        <asp:TextBox ID="ASD_txtdtFim" class="form-control" runat="server"></asp:TextBox>
                                                    </div>

                                                    <div class="form-group col-lg-12" id="ASD_DivBotaoResponde" runat="server">
                                                        <asp:Button ID="ASD_cmdResponder" CssClass="btn-success btn" runat="server" OnClick="ASD_cmdResponder_Click" Text="Responder" />
                                                    </div>
                                                </div>
                                                <div id="ASD_DIV_RESPONDER" runat="server">
                                                    <asp:Button ID="ASD_cmdFechar" CssClass="btn-info btn" runat="server" OnClick="ASD_cmdFechar_Click" Text="Encerrar Avaliação" />
                                                    <div class="col-lg-12  embed-responsive embed-responsive-16by9" style="min-height: 1000px" runat="server" id="ASD_Div_BlocoResposta">
                                                        <embed type="text/html" runat="server" id="ASD_frmResponder" />
                                                    </div>
                                                </div>

                                                <div class="form-group col-lg-12">
                                                    <asp:Repeater ID="ASD_rptAvaliacao" runat="server">
                                                        <ItemTemplate>
                                                            <div class="panel panel-primary">
                                                                <div class="panel-body">
                                                                    <div class="well bg-danger">
                                                                        <b>
                                                                            <asp:Label runat="server" Text='<%# Eval("sGrupo") %>'></asp:Label>
                                                                        </b>
                                                                    </div>



                                                                    <asp:Repeater ID="ASD_rptAvaliacao_Respostas" DataSource='<%# ((DataRowView)Container.DataItem).Row.GetChildRows("ASD_Relacionamento") %>' runat="server">
                                                                        <ItemTemplate>

                                                                            <div class="panel panel-default">
                                                                                <div class="panel-heading">
                                                                                    <h3 class="panel-title"><b>
                                                                                        <asp:Label runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "[\"sDscPergunta\"]")%>'></asp:Label>
                                                                                    </b></h3>
                                                                                </div>
                                                                                <div class="panel-body" style="min-height: 50px">
                                                                                    <div class="form-stacked">
                                                                                        <label>
                                                                                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "[\"sResposta\"]")%>'></asp:Label>
                                                                                        </label>
                                                                                        </br>
                                                                       <asp:Label runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "[\"sJustificativa\"]")%>'></asp:Label>
                                                                                    </div>
                                                                                </div>
                                                                            </div>

                                                                        </ItemTemplate>
                                                                    </asp:Repeater>

                                                                </div>
                                                            </div>


                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>

                                            </div>
                                        </div>

                                    </div>


                                    <div role="tabpanel" class="tab-pane fade" id="Resultado" aria-labelledby="Resultado">
                                        DEV
                                    </div>
                                </div>

                            </div>

                            <div class="modal-footer">
                                <div class="col-lg-11" align="left">
                                    <asp:Button ID="cmdSalvar" CssClass="btn-success btn" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                                    <asp:Button ID="cmdExcluir" CssClass="btn-danger btn" runat="server" Text="Excluir" />
                                </div>
                                <div class="col-lg-1">
                                    <button id="btnFecharFooterDetalhe" type="button" class="btn btn-secondary" data-dismiss="modal">Fechar</button>
                                </div>
                            </div>
                        </div>


                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>


    <div id="dialog-Gerar" class="modal" title="Gerar Avaliações">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloGerar" runat="server" Text=""></asp:Label>
            <br />
            <label>Informe uma Descrição para o Grupo</label>
        </p>
        <asp:TextBox ID="txtsTextoGerar" class="form-control" runat="server"></asp:TextBox>


    </div>

    <div id="dialog-Excluir" class="modal" title="Excluir Avaliação">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloExcluir" runat="server" Text=""></asp:Label>
        </p>
    </div>


    <script>
        function initSelectAllCheckbox() {
            var grid = $('#<%= gvResultado_Gerar.ClientID %>');

            if (grid.length > 0) {
                var chkSelectAll = grid.find('th input[type="checkbox"]').first();

                if (chkSelectAll.length > 0) {
                    chkSelectAll.off('change.selectall').on('change.selectall', function () {
                        var isChecked = $(this).is(':checked');
                        grid.find('td input[type="checkbox"][id*="chkSelecionado"]').prop('checked', isChecked);
                    });
                }
            }
        }

        $(document).ready(function () {
            initSelectAllCheckbox();
        });

        if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(function () {
                initSelectAllCheckbox();
            });
        }
    </script>
</asp:Content>
