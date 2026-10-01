<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Colaboradores_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.Colaboradores_Detalhe" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <asp:UpdatePanel ID="updDetalhe" runat="server">
        <ContentTemplate>


            <div class="form-stacked">

                <style>
                    .chatpanel-header{
                        display: flex;
                        align-items: center;
                        justify-content: space-between;
                    }
                    .hdChat{
                        display:flex;
                        align-items:center;
                    }
                    .tt-switch {
                        display: inline-flex;
                        align-items: center;
                        min-height: 34px;
                    }
                    #cphCorpo_cmdConversas{
                        padding: 6px 12px;
                        font-size: 14px;
                    }
                    .tt-switch input[type="checkbox"] {
                        position: absolute;
                        opacity: 0;
                        width: 0;
                        height: 0;
                    }

                    .tt-switch label {
                        position: relative;
                        display: inline-flex;
                        align-items: center;
                        margin: 0;
                        padding-left: 58px;
                        min-height: 30px;
                        cursor: pointer;
                        font-weight: normal;
                    }

                    .tt-switch label:before {
                        content: "";
                        position: absolute;
                        left: 0;
                        width: 48px;
                        height: 26px;
                        border-radius: 999px;
                        background: #d5dbe3;
                        transition: background .2s ease;
                    }

                    .tt-switch label:after {
                        content: "";
                        position: absolute;
                        left: 3px;
                        width: 20px;
                        height: 20px;
                        border-radius: 50%;
                        background: #fff;
                        box-shadow: 0 1px 4px rgba(0,0,0,.25);
                        transition: transform .2s ease;
                    }

                    .tt-switch input[type="checkbox"]:checked + label:before {
                        background: #337ab7;
                    }

                    .tt-switch input[type="checkbox"]:checked + label:after {
                        transform: translateX(22px);
                    }

                    .sender {
                        background-color: #e2e2e2;
                    }

                    .badgeControl-danger, .badgeControl-success, .badgeControl-warning, .badgeControl-info {
                        display: inline-block;
                        width: 10px;
                        height: 10px;
                        border-radius: 50%;
                        text-align: center;
                        line-height: 20px;
                        padding: 3px;
                        font-size: 12px;
                        font-weight: bold;
                    }

                    .badgeControl-danger {
                        background-color: red;
                        color: white;
                    }

                    .badgeControl-success {
                        background-color: green;
                        color: white;
                    }

                    .badgeControl-warning {
                        background-color: yellow;
                        color: black;
                    }

                    .badgeControl-info {
                        background-color: blue;
                        color: white;
                    }

                    .texto-resumido {
                        white-space: nowrap;
                        overflow: hidden;
                        text-overflow: ellipsis;
                        max-width: 80%;
                    }

                    .collapsed-row {
                        display: none;
                        padding: 1px;
                        margin: 1px;
                    }

                    .gvMainTh {
                        padding: 4px;
                        border: 1px solid #bbb;
                        font-weight: bold;
                    }

                    .gvMainTd {
                        padding: 7px;
                        border: 1px solid #bbb;
                    }

                        .gvMainTd tbody tr:hover td {
                            /* background-color: #e0e0e0*/
                        }

                    .table {
                        margin-bottom: 0px !important;
                    }

                    .todosEPI {
                        text-align: center !important;
                    }

                    .modal-content {
                        overflow-y: inherit !important;
                    }

                    .panel-body .tipoBeneficio-container,
                    .panel-body #div_tipoBeneficio h3 {
                        display: flex;
                        flex-wrap: wrap;
                        gap: 8px;
                        margin: 0;
                        padding: 0;
                    }

                        .panel-body .tipoBeneficio-container .status,
                        .panel-body #div_tipoBeneficio h3 .status {
                            display: inline-block;
                        }

                    .datepicker {
                        z-index: 3000 !important;
                    }
                </style>

                <div class="col-lg-12 row">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Colaboradores"></asp:Label>
                    </h1>
                    <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>

                <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                <div id="abas">
                    <ul id="tab_Colaborador" class="nav nav-tabs" role="tablist">
                        <li role="presentation" class="tabpanel active">
                            <a href="#colaborador" id="aba_Colaborador" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Colaborador</b></a>
                        </li>
                        <li role="presentation" runat="server" id="aba_Documentos">
                            <a href="#documentos" role="tab" id="documentos-tab" data-toggle="tab" aria-controls="Documentos"><b>Documentos</b></a>
                        </li>
                        <li role="presentation" runat="server" id="aba_SSTT">
                            <a href="#SSTT" role="tab" id="SSTT-tab" data-toggle="tab" aria-controls="SSTT"><b>SSTT</b></a>
                        </li>
                        <li role="presentation" runat="server" id="aba_Evento">
                            <a href="#Evento" role="tab" id="Evento-tab" data-toggle="tab" aria-controls="Ausencia"><b>Ocorrências </b></a>
                        </li>
                        <li role="presentation" runat="server" id="aba_Beneficios">
                            <a href="#beneficios" role="tab" id="beneficios-tab" data-toggle="tab" aria-controls="Beneficios"><b>Benefícios</b></a>
                        </li>
                        <li role="presentation" runat="server" id="aba_Conversas">
                            <a href="#conversas" role="tab" id="conversas-tab" data-toggle="tab" aria-controls="Conversas"><b>Conversas</b></a>
                        </li>
                        <li role="presentation" runat="server" id="aba_Avaliacao">
                            <a href="#avaliacao" role="tab" id="avaliacao-tab" data-toggle="tab" aria-controls="Avaliacao"><b>Avaliação</b></a>
                        </li>
                        <li role="presentation" runat="server" id="aba_ArquivoMorto">
                            <a href="#arquivoMorto" role="tab" id="arquivoMorto-tab" data-toggle="tab" aria-controls="ArquivoMorto"><b>Arquivo Morto</b></a>
                        </li>
                    </ul>
                </div>

            </div>

            <div id="tab" class="tab-content">

                <div role="tabpanel" class="tab-pane fade in active" id="colaborador" aria-labelledby="colaborador-tab">
                    <br />

                    <div class="panel panel-default" runat="server" id="Div_Beneficios">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Benefícios</b></h3>
                        </div>

                        <div class="panel-body">
                            <div class="form-group" id="div_tipoBeneficio" runat="server">
                                <h3></h3>
                            </div>
                        </div>
                    </div>

                    <div class="panel panel-default" runat="server" id="div_Cabecalho">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Dados do Colaborador</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">

                                <div class="col-lg-6">

                                    <div class="col-lg-6">
                                        <div class="form-group">
                                            <br />
                                            <br />
                                            <asp:Image ID="imgColaborador" runat="server" class="text-center form-control" Height="320" Width="250" Style="display: block; margin-left: auto; margin-right: auto;" />
                                            <br />
                                            <br />

                                        </div>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="form-group">
                                            <br />
                                            <br />
                                            <br />
                                            <br />
                                            <br />
                                            <br />
                                            <asp:Image ID="imgQr" runat="server" class="text-center form-control" Height="120" Width="120" Style="display: block; margin-left: auto; margin-right: auto;" />
                                        </div>

                                    </div>

                                </div>

                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Nome Colaborador  </label>
                                        <asp:TextBox ID="txtsDscColaborador" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Nome Social  </label>
                                        <asp:TextBox ID="txtsNomeSocial" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <div class="form-group row">
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <label>Empresa </label>
                                                <asp:DropDownList ID="ddlidEmpresa" class="form-control " runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlidEmpresa_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <label>Tipo de Contrato </label>
                                                <asp:DropDownList ID="ddlidTipoContrato" class="form-control yes_no select" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlidTipoContrato_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <div class="form-group row">

                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <label>Departamento </label>
                                                <asp:DropDownList ID="ddlidDepartamento" class="form-control " runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidDepartamento_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <label>Cargo </label>
                                                <asp:DropDownList ID="ddlidCargo" class="form-control " runat="server"></asp:DropDownList>
                                            </div>
                                        </div>

                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Supervisor Direto</label>
                                        <asp:DropDownList ID="ddlidSupervisorDireto" class="form-control " runat="server"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>
                                            <asp:Label ID="lblsFuncao" runat="server" Text="Função Carteira"></asp:Label></label>
                                        <asp:DropDownList ID="ddlidFuncao" class="form-control " runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlidFuncao_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>GHE</label>
                                        <asp:TextBox ID="txtsDscGHE" class="form-control " runat="server" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Setor</label>
                                        <asp:TextBox ID="txtsDscGHESetor" class="form-control " runat="server" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>


                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>CBO</label>
                                        <asp:TextBox ID="sCBO" class="form-control " runat="server" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <label>E-mail </label>
                                        <asp:TextBox ID="txtsEmail" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <div class="form-group row">

                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label>Data Início Contrato </label>
                                                <asp:TextBox ID="txtdtInicioContrato" class="form-control " runat="server" AutoPostBack="True" Type="date" OnTextChanged="txtdtInicioContrato_TextChanged"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div runat="server" id="div_ddlsLimiteContrato" class="col-lg-2">
                                            <div class="form-group">
                                                <label>Contrato Limite? </label>
                                                <asp:DropDownList ID="ddlsLimiteContrato" runat="server" class="form-control yes_no select" AutoPostBack="True" OnSelectedIndexChanged="ddlsLimiteContrato_SelectedIndexChanged">
                                                    <asp:ListItem Value="S">Sim</asp:ListItem>
                                                    <asp:ListItem Value="N">Não</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="DIV_sTerminoContrato">
                                            <div class="form-group">
                                                <label>
                                                    <asp:Label ID="lblsTerminoContrato" runat="server" Text="Término Contrato"></asp:Label></label>
                                                <asp:TextBox ID="txtsTerminoContrato" class="form-control " runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-4" runat="server" id="div_txtRegistro">
                                            <div class="form-group">
                                                <label>Registro</label>
                                                <asp:TextBox ID="txtRegistro" class="form-control" runat="server" MaxLength="20"></asp:TextBox>
                                            </div>
                                        </div>

                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <div class="form-group row">
                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label>Celular Empresa</label>
                                                <asp:TextBox ID="txtsTelCelular" class="form-control " runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-3">
                                            <div class="form-group">
                                                <label>Ramal </label>
                                                <asp:TextBox ID="txtsRamal" class="form-control " runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server">
                                            <div class="form-group">
                                                <label>Dependentes? </label>
                                                <asp:DropDownList ID="ddlsDependentesConvenio" runat="server" class="form-control yes_no select" AutoPostBack="True" OnSelectedIndexChanged="ddlsDependentesConvenio_SelectedIndexChanged">
                                                    <asp:ListItem Value="N">Não</asp:ListItem>
                                                    <asp:ListItem Value="S">Sim</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-3" runat="server" id="DIV3">
                                            <div class="form-group">
                                                <asp:Label ID="Label1" runat="server" Text="Data Prev. Plano Saúde" Style="font-weight: bold;"></asp:Label>
                                                <label>&nbsp;</label>
                                                <asp:TextBox ID="txtdtPrevPlanoSaude" class="form-control " runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="Div_ddlsTipoCTPS">
                                    <div class="form-group">
                                        <label>Tipo CTPS </label>
                                        <asp:DropDownList ID="ddlsTipoCTPS" runat="server" class="form-control yes_no select" AutoPostBack="True" OnSelectedIndexChanged="ddlsDependentesConvenio_SelectedIndexChanged">
                                            <asp:ListItem Value="0">Selecione</asp:ListItem>
                                            <asp:ListItem Value="N">Não Necessário</asp:ListItem>
                                            <asp:ListItem Value="A">Entrega Pendente</asp:ListItem>
                                            <asp:ListItem Value="F">Física</asp:ListItem>
                                            <asp:ListItem Value="D">Digital</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-1">
                                    <div class="form-group">
                                        <label>Status </label>
                                        <asp:DropDownList ID="ddlsSituacao" runat="server" class="form-control yes_no select" AutoPostBack="True" OnSelectedIndexChanged="ddlsSituacao_SelectedIndexChanged">
                                            <asp:ListItem Value="S">Ativo</asp:ListItem>
                                            <asp:ListItem Value="N">Inativo</asp:ListItem>
                                            <asp:ListItem Value="F">Fora do Quadro</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Liberado para fazer relatório de gastos</label>
                                        <asp:DropDownList ID="ddlsRelatorioGastos" runat="server" class="form-control yes_no select">
                                            <asp:ListItem Value="S">Sim</asp:ListItem>
                                            <asp:ListItem Selected="True" Value="N">Não</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="DIV_dtDesligamento">
                                    <div class="form-group">
                                        <asp:Label ID="lbldtDesligamnto" runat="server" Text="Data Desligamento" Style="font-weight: bold;"></asp:Label>
                                        <label>&nbsp;</label>
                                        <asp:TextBox ID="txtdtDesligamento" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

                    <div class="panel panel-default" runat="server" id="Div_Dependentes">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Dependentes</b></h3>
                        </div>

                        <div class="panel-body">
                            <div class="form-stacked row">

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <div class="row" runat="server" id="div9">

                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>Nome dependente</label>
                                                    <asp:TextBox ID="txtsNomeDependente" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Grau de Parentesco</label>
                                                    <asp:DropDownList ID="ddlidGrauParentesco" runat="server" class="form-control yes_no select" RepeatLayout="Table" AutoPostBack="True"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>CPF Dependente</label>
                                                    <asp:TextBox ID="txtsCPFDependente" class="form-control " runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Data Nascimento</label>
                                                    <asp:TextBox ID="txtdtNascDependente" class="form-control " runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-6">
                                                <div class="form-group row">
                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label>Possui Pensão?</label>
                                                            <asp:DropDownList ID="ddlsPensaoDependente" runat="server" class="form-control yes_no select" RepeatLayout="Table" AutoPostBack="True" OnSelectedIndexChanged="ddlsPensaoDependente_SelectedIndexChanged">
                                                                <asp:ListItem Value="N">Não </asp:ListItem>
                                                                <asp:ListItem Value="S">Sim</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6" runat="server" id="Div_ValorPensaoDependente">
                                                        <div class="form-group">
                                                            <label>Valor Pensão</label>
                                                            <asp:TextBox ID="txtnValorPensaoDependente" class="form-control " runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>

                                            <div class="col-lg-3">
                                                <div class="form-group">
                                                    <label>Tipo de Conta</label>
                                                    <asp:DropDownList ID="ddlidTipoContaDepentente" runat="server" class="form-control yes_no select" RepeatLayout="Table"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-3">
                                                <div class="form-group">
                                                    <label>Banco</label>
                                                    <asp:TextBox ID="txtsBancoDependente" class="form-control " runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-3">
                                                <div class="form-group">
                                                    <label>Agência</label>
                                                    <asp:TextBox ID="txtsAgencia" class="form-control " runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-3">
                                                <div class="form-group">
                                                    <label>Numero Conta</label>
                                                    <asp:TextBox ID="txtsContaDependente" class="form-control " runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-12">
                                                <div class="form-group">
                                                    <label>Observação do Dependente </label>
                                                    <asp:TextBox ID="txtsObservacaoDependente" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="120px"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <br />
                                                <asp:Button ID="cmdDependente" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdIncluirDependentes_Click" />

                                                <asp:Button ID="BtnSalvarDependente" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Salvar" OnClick="BtnSalvarDependente_Click1" />
                                            </div>

                                            <div class="col-lg-12">
                                                <br />

                                                <uc1:MensagemPagina runat="server" ID="MensagemPagina_Dependentes" />
                                            </div>

                                        </div>

                                        <div class="row">
                                            <div class="col-lg-12">
                                                <asp:GridView ID="dtgDependentes" class="table table-striped table-bordered table-hover table-condensed"
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" ClientIDMode="Static"
                                                    OnRowDataBound="dtgDependentes_RowDataBound" OnRowDeleting="dtgDependentes_RowDeleting" OnRowCommand="dtgDependentes_RowCommand">
                                                    <Columns>

                                                        <asp:BoundField DataField="idLinha" HeaderText="idLinha">
                                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sNomeDependente" HeaderText="Nome">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sCPFDependente" HeaderText="CPF">
                                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="dtNascDependente" HeaderText="Data Nascimento">
                                                            <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sPensaoDependente" HeaderText="Possui pensão?">
                                                            <ItemStyle Width="2%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="nValorPensaoDependente" HeaderText="Valor Pensão">
                                                            <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sDscGrauParentesco" HeaderText="Parentesco">
                                                            <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sObservacaoDependente" HeaderText="Observação">
                                                            <ItemStyle Width="16%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkDependenteEditar" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Editar" CssClass="btn btn-small" TabIndex="100" CommandName="Editar"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkDependente_UpLoad" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Upload do Arquivo" CssClass="btn     all" TabIndex="100" CommandName="Upload_Arquivo"><i class="fa-upload fa"></i></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkDependente_Download" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Download do Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkDependente_Excluir" CommandArgument='<%# Bind("idLinha") %>' runat="server" CssClass="btn btn-small" TabIndex="100" data-toggle="tooltip" title="Excluir" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
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

                    <div class="panel panel-default" runat="server" id="DIV_DadosPessoais">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Dados Pessoais</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>CEP</label>
                                        <asp:TextBox ID="txtEndereco_sCEP" class="form-control " runat="server" AutoPostBack="True" OnTextChanged="txtEndereco_sCEP_TextChanged"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-8">
                                    <div class="form-group">
                                        <label>Logradouro </label>
                                        <asp:TextBox ID="txtEndereco_sLogradouro" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Número </label>
                                        <asp:TextBox ID="txtEndereco_sNumero" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Complemento</label>
                                        <asp:TextBox ID="txtEndereco_sComplemento" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Bairro</label>
                                        <asp:TextBox ID="txtEndereco_sBairro" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-12 ">
                                    <div class="form-group row">
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <label>Cidade</label>
                                                <asp:TextBox ID="txtEndereco_sCidade" class="form-control " runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>Estado</label>
                                                <asp:DropDownList ID="ddlEndereco_sUF" runat="server" class="form-control CaixaTextoPequeno"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Data nascimento </label>
                                        <asp:TextBox ID="txtdtNascimento" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Telefone </label>
                                        <asp:TextBox ID="txtsTelResidencial" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>


                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Celular </label>
                                        <asp:TextBox ID="txtsCelularPessoal" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>E-mail Pessoal</label>
                                        <asp:TextBox ID="txtsEmailPessoal" class="form-control " MaxLength="100" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Estado Civil</label>
                                        <asp:DropDownList ID="ddlsEstadoCivil" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-1">
                                    <div class="form-group">
                                        <label>Sexo</label>
                                        <asp:DropDownList ID="ddlsSexo" class="form-control" runat="server">
                                            <asp:ListItem Text="Selecione o Sexo" Value=""></asp:ListItem>
                                            <asp:ListItem Text="Feminino" Value="F"></asp:ListItem>
                                            <asp:ListItem Text="Masculino" Value="M"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Pessoa Exposta Politicamente</label>
                                        <asp:DropDownList ID="ddlsPolitico" class="form-control" runat="server">
                                            <asp:ListItem Text="Não" Value="N" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="Sim" Value="S"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

                    <div class="panel panel-default" runat="server" id="Div_Documentos">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Documentos</b></h3>
                        </div>
                        <div class="panel-body row">

                            <div class="col-lg-12 ">
                                <div class="form-group row">
                                    <div class="col-lg-3">
                                        <div class="form-group">
                                            <asp:Label ID="lblsCPF" runat="server" Text="CPF" Style="font-weight: bold;"></asp:Label>
                                            <asp:TextBox ID="txtsCPF" class="form-control " runat="server" Style="margin-top: 5px;"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-3">
                                        <div class="form-group">
                                            <asp:Label ID="lbltxtsRG" runat="server" Text="RG/RNE" Style="font-weight: bold;"></asp:Label>
                                            <asp:TextBox ID="txtsRG" class="form-control " runat="server" Style="margin-top: 5px;"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-3" runat="server" id="DIV_sCNPJ">
                                        <div class="form-group">
                                            <label>CNPJ</label>
                                            <asp:TextBox ID="txtsCNPJ" class="form-control " runat="server"></asp:TextBox>
                                        </div>
                                    </div>


                                    <div class="col-lg-3" runat="server" id="DIV_sPIS">
                                        <div class="form-group">
                                            <label>PIS</label>
                                            <asp:TextBox ID="txtsPIS" class="form-control " runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-3" runat="server" id="DIV_sIE">
                                        <div class="form-group">
                                            <asp:Label ID="lblsIE" runat="server" Text="Inscrição Estadual" Style="font-weight: bold;"></asp:Label>
                                            <asp:TextBox ID="txtsIE" class="form-control " runat="server" Style="margin-top: 5px;"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-3" runat="server" id="DIV7">
                                        <div class="form-group">
                                            <asp:Label ID="lblsNomeMae" runat="server" Text="Nome da Mãe" Style="font-weight: bold;"></asp:Label>
                                            <asp:TextBox ID="txtsNomeMae" class="form-control " runat="server" Style="margin-top: 5px;"></asp:TextBox>
                                        </div>
                                    </div>

                                </div>
                            </div>

                            <div class="col-lg-3" runat="server" id="DIV_sIM">
                                <div class="form-group">
                                    <asp:Label ID="lblsIM" runat="server" Text="Inscrição Municipal " Style="font-weight: bold;"></asp:Label>
                                    <asp:TextBox ID="txtsIM" class="form-control " runat="server" Style="margin-top: 5px;"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-3" runat="server" id="DIV_sNIRE">
                                <div class="form-group">
                                    <asp:Label ID="lblsNIRE" runat="server" Text="NIRE" Style="font-weight: bold;"></asp:Label>
                                    <asp:TextBox ID="txtsNIRE" class="form-control " runat="server" Style="margin-top: 5px;"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-3" runat="server" id="DIV_dtRegistro">
                                <div class="form-group">
                                    <asp:Label ID="lbldtRegistro" runat="server" Text="Data Registro NIRE" Style="font-weight: bold;"></asp:Label>
                                    <asp:TextBox ID="txtdtRegistro" class="form-control " runat="server" Style="margin-top: 5px;"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-12 ">
                                <div class="form-group row">
                                    <div class="col-lg-3">
                                        <div class="form-group">
                                            <label>Escolaridade</label>
                                            <asp:DropDownList runat="server" ID="ddlsEscolaridade" class="form-control Caixa_Selecao"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>CNH</label>
                                            <asp:TextBox ID="txtsCNH" class="form-control " runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Vencimento CNH</label>
                                            <asp:TextBox ID="txtdtVencCNH" class="form-control " runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-3">
                                        <div class="form-group">
                                            <label>Categoria CNH</label>
                                            <asp:CheckBoxList ID="cblsTipoCNH" runat="server" RepeatColumns="5" RepeatLayout="Table" RepeatDirection="Vertical">
                                                <asp:ListItem Value="A">A&nbsp;</asp:ListItem>
                                                <asp:ListItem Value="B">B&nbsp; </asp:ListItem>
                                                <asp:ListItem Value="C">C&nbsp; </asp:ListItem>
                                                <asp:ListItem Value="D">D&nbsp; </asp:ListItem>
                                                <asp:ListItem Value="E">E&nbsp; </asp:ListItem>
                                            </asp:CheckBoxList>
                                        </div>
                                    </div>

                                </div>
                            </div>


                            <div class="col-lg-3">
                                <div class="form-group">
                                    <label>Tipo de Conta </label>
                                    <asp:DropDownList ID="ddlsTipoConta" runat="server" class="form-control yes_no select">
                                        <asp:ListItem Value="C">Conta Corrente</asp:ListItem>
                                        <asp:ListItem Value="P">Poupança</asp:ListItem>
                                        <asp:ListItem Value="S">Salário</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-3">
                                <div class="form-group">
                                    <label>Banco</label>
                                    <asp:TextBox ID="txtsBanco" class="form-control " runat="server"></asp:TextBox>
                                </div>
                            </div>


                            <div class="col-lg-3">
                                <div class="form-group">
                                    <label>Agência</label>
                                    <asp:TextBox ID="txtsAgenciaBancaria" class="form-control " runat="server"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-3">
                                <div class="form-group">
                                    <label>Conta</label>
                                    <asp:TextBox ID="txtsContaBancaria" class="form-control " runat="server"></asp:TextBox>
                                </div>
                            </div>

                            <%--                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Possui Pensão? </label>
                                        <asp:DropDownList ID="ddlsPensao" runat="server" class="form-control yes_no select" AutoPostBack="True" OnSelectedIndexChanged="ddlsLimiteContrato_SelectedIndexChanged">
                                            <asp:ListItem Value="N">Não</asp:ListItem>
                                            <asp:ListItem Value="S">Sim</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>



                                <div class="col-lg-12 ">
                                    <div class="form-group row">


                                        <div class="col-lg-3" runat="server" id="DIV_sNomePensao">
                                            <div class="form-group">
                                                <label>Nome</label>
                                                <asp:TextBox ID="txtsNomePensao" class="form-control " runat="server"></asp:TextBox>
                                            </div>

                                        </div>
                                        <div class="col-lg-3" runat="server" id="DIV_sSobrenomePensao">
                                            <div class="form-group">
                                                <label>Sobrenome</label>
                                                <asp:TextBox ID="txtsSobrenomePensao" class="form-control " runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-3" runat="server" id="DIV_sCPFPensao">
                                            <div class="form-group">
                                                <label>CPF</label>
                                                <asp:TextBox ID="txtsCPFPensao" class="form-control " runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                    </div>
                                </div>

                                <div class="col-lg-12 ">
                                    <div class="form-group row">
                                        <div class="col-lg-3" runat="server" id="DIV_sBancoPensao">
                                            <div class="form-group">
                                                <label>Banco</label>
                                                <asp:TextBox ID="txtsBancoPensao" class="form-control " runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-3" runat="server" id="DIV_sTipoContaPensao">
                                            <div class="form-group">
                                                <label>Tipo de Conta </label>
                                                <asp:DropDownList ID="ddlsTipoContaPensao" runat="server" class="form-control yes_no select" AutoPostBack="True" OnSelectedIndexChanged="ddlsLimiteContrato_SelectedIndexChanged">
                                                    <asp:ListItem Value="C">Conta Corrente</asp:ListItem>
                                                    <asp:ListItem Value="P">Poupança</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-3" runat="server" id="DIV_sAgenciaPensao">
                                            <div class="form-group">
                                                <label>Agência Bancaria</label>
                                                <asp:TextBox ID="txtsAgenciaPensao" class="form-control " runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-3" runat="server" id="DIV_sContaPensao">
                                            <div class="form-group">
                                                <label>Conta Bancaria</label>
                                                <asp:TextBox ID="txtsContaPensao" class="form-control " runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                    </div>
                                </div>--%>

                            <div class="col-lg-12">
                                <div class="form-group">
                                    <label>Observação </label>
                                    <asp:TextBox ID="txtsObservacao" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="120px"></asp:TextBox>
                                </div>
                            </div>


                        </div>
                    </div>

                    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

                </div>

                <div role="tabpanel" class="tab-pane fade" id="documentos" aria-labelledby="documentos-tab">
                    <div class=" embed-responsive embed-responsive-16by9" style="min-height: 200px">
                        <embed type="text/html" runat="server" id="frmDocumentos" width="800" height="500" />
                    </div>
                </div>

                <div role="tabpanel" class="tab-pane fade" id="SSTT" aria-labelledby="SSTT-tab">
                    <br />

                    <div class="panel panel-default" runat="server" id="Div_SSTT">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Saúde</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Tipo Sanguíneo </label>
                                        <asp:DropDownList ID="ddlidTipoSanguineo" class="form-control" runat="server"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Contado de Emergência </label>
                                        <asp:TextBox ID="txtsContatoEmergencia" class="form-control CaixaTextoMedia" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Número do Contato</label>
                                        <asp:TextBox ID="txtsDscContatoEmergencia" class="form-control CaixaTextoMedia" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

                    <div class="panel panel-default" runat="server" id="Div_nr">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Certificação NR e Outros</b></h3>
                        </div>

                        <%------------------ Higor Maestrello 18-06-2024 -------------------------------%>
                        <div class="col-lg-12">
                            <br />
                            <uc1:MensagemPagina runat="server" ID="MensagemPagina1" />
                        </div>
                        <%-------------------------------------------------------------------------------%>

                        <div class="panel-body">

                            <div class="col-lg-12 row">
                                <div class="form-group  row">
                                    <div runat="server" id="div_SelecaoNR">

                                        <div class="col-lg-2 form-group">
                                            <asp:Button ID="cmdIncluirItem" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdIncluirItem_Click" />
                                        </div>

                                    </div>
                                </div>
                            </div>

                            <uc1:MensagemPagina runat="server" ID="MensagemPagina_NR" />

                            <div class="row">
                                <div class="col-lg-12">
                                    <asp:GridView ID="dtgNR" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="idRegistroNR, idArquivo" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small"
                                        OnRowDataBound="dtgNR_RowDataBound" OnRowDeleting="dtgNR_RowDeleting" OnRowCommand="dtgNR_RowCommand">
                                        <Columns>

                                            <%------------------ Higor Maestrello 19-06-2024 -------------------------------%>

                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <div style="text-align: center;">
                                                        <asp:CheckBox ID="chkTarefas_Seleciona_Todos" runat="server" CssClass="Todos" />
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkTarefas_Seleciona" runat="server" CssClass="Individual" />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="2%" />
                                            </asp:TemplateField>

                                            <%-------------------------------------------------------------------------------%>

                                            <asp:BoundField DataField="idLinha" HeaderText="idLinha">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscNR" HeaderText="Tipo NR">
                                                <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <%------------------ Higor Maestrello 19-06-2024 -------------------------------%>

                                            <asp:TemplateField HeaderText="Nome do Documento">
                                                <ItemTemplate>
                                                    <asp:LinkButton runat="server" Text='<%# Eval("sNomeArquivo") %>' CommandName="Download" CommandArgument='<%# Eval("idLinha") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="sObservacaoArquivo" HeaderText="Observação">
                                                <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <%-------------------------------------------------------------------------------%>

                                            <asp:BoundField DataField="dtEmissaoNR" HeaderText="Data de Emissão">
                                                <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtVencimentoNR" HeaderText="Data de Vencimento">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkEditar" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Editar" CssClass="btn btn-small" TabIndex="100" CommandName="Editar"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="lnkUpLoad" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Upload do Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Upload_Arquivo"><i class="fa-upload fa"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="lnkDownload" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Download do Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="lnkExcluir" CommandArgument='<%# Bind("idLinha") %>' runat="server" CssClass="btn btn-small" TabIndex="100" data-toggle="tooltip" title="Excluir" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>

                            <%------------------ Higor Maestrello 18-06-2024 -------------------------------%>

                            <div class="row">
                                <div class="col-lg-12" runat="server" id="btDownload">
                                    <div class="form-group">
                                        <asp:Button ID="btDownloadArquivo" CssClass="btn-success btn btDownload" runat="server" Text="Download" CommandName="Download_Arquivo_Checked" />
                                    </div>
                                </div>
                            </div>

                            <%-------------------------------------------------------------------------------%>
                        </div>

                    </div>

                    <div class="panel panel-default" runat="server" id="Div_EPI">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Recebimento de EPI</b></h3>
                        </div>

                        <div class="panel-body">

                            <div class="form-stacked row">
                                <div class="col-lg-12 form-group table-responsive" style="overflow: visible;">
                                    <asp:GridView ID="gv_EPI" runat="server" class="table table-striped table-condensed table-bordered table-hover"
                                        AutoGenerateColumns="False" GridLines="None" ShowFooter="False"
                                        Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gv_EPI_RowDataBound" OnRowCommand="gv_EPI_RowCommand">
                                        <Columns>

                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:LinkButton ID="btnToggle_Todos" ClientIDMode="Static" runat="server" CssClass="composicaoLinha fa fa-plus"></asp:LinkButton>
                                                </HeaderTemplate>
                                                <HeaderStyle CssClass="todosEPI" />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnToggle" ClientIDMode="Static" runat="server" CssClass="composicaoLinha fa fa-plus" data-div-id='<%# "tr_1_EPI" + string.Format("{0}",Eval("idEntregaEPI")) %>'></asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idEntregaEPI"
                                                DataTextField="idEntregaEPI" HeaderText="ID" Target="_blank"
                                                DataNavigateUrlFormatString="/App/Paginas/RRHH/EntregaEPI_Detalhe.aspx?id={0}">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idEntregaEPI"
                                                DataTextField="sTipoEntrega" HeaderText="Tipo de Entrega" Target="_blank"
                                                DataNavigateUrlFormatString="/App/Paginas/RRHH/EntregaEPI_Detalhe.aspx?id={0}">
                                                <ItemStyle Width="17%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idEntregaEPI"
                                                DataTextField="dtSolicitacao" HeaderText="Data Solicitação" Target="_blank"
                                                DataNavigateUrlFormatString="/App/Paginas/RRHH/EntregaEPI_Detalhe.aspx?id={0}">
                                                <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idEntregaEPI"
                                                DataTextField="nQtd" HeaderText="Qtd EPIs" Target="_blank"
                                                DataNavigateUrlFormatString="/App/Paginas/RRHH/EntregaEPI_Detalhe.aspx?id={0}">
                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idEntregaEPI"
                                                DataTextField="sDscStatus" HeaderText="Status" Target="_blank"
                                                DataNavigateUrlFormatString="/App/Paginas/RRHH/EntregaEPI_Detalhe.aspx?id={0}">
                                                <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idEntregaEPI"
                                                DataTextField="dtEntrega" HeaderText="Data Entrega" Target="_blank"
                                                DataNavigateUrlFormatString="/App/Paginas/RRHH/EntregaEPI_Detalhe.aspx?id={0}">
                                                <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idEntregaEPI"
                                                DataTextField="dtConfirmacao" HeaderText="Data Confirmação" Target="_blank"
                                                DataNavigateUrlFormatString="/App/Paginas/RRHH/EntregaEPI_Detalhe.aspx?id={0}">
                                                <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:TemplateField HeaderText="">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkEPI_Download" CommandArgument='<%# Bind("idArquivo") %>' runat="server" data-toggle="tooltip" title="Download da Entrega de EPI" CssClass="btn btn-small" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>

                                                    <%# NovaLinha(string.Format("{0}",Eval("idEntregaEPI")), "_1_EPI") %>
                                                    <asp:GridView ID="gv_EPI_itens" class="table table-striped table-bordered table-hover" AlternatingRowStyle-CssClass="gvAltRow"
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="idEntregaEPI"
                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gv_EPI_itens_RowDataBound">
                                                        <Columns>

                                                            <asp:BoundField DataField="sCodigo" HeaderText="Código" />
                                                            <asp:BoundField DataField="sDscProduto" HeaderText="Descrição" />
                                                            <asp:BoundField DataField="nQuantidadeEPI" HeaderText="Quantidade" />
                                                            <asp:BoundField DataField="nCA" HeaderText="CA" />
                                                            <asp:BoundField DataField="sPeriodo" HeaderText="Periodicidade" />
                                                            <asp:BoundField DataField="dtVencimento" HeaderText="Data Vencimento" />

                                                        </Columns>
                                                    </asp:GridView>

                                                </ItemTemplate>
                                                <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                        </Columns>
                                    </asp:GridView>
                                </div>

                                <div class="col-lg-12 row form-group">
                                    <div class="col-lg-3 form-group">
                                        <label>&nbsp;</label>
                                        <asp:LinkButton runat="server" ID="cmdConsulta_EPIsAntigos" class="btn btn-info form-control" Text="Consultar EPIs Antigos" OnClick="cmdConsulta_EPIsAntigos_Click"></asp:LinkButton>
                                    </div>
                                </div>
                            </div>

                            <div runat="server" id="div_EPIs_Antigos" class="panel panel-default">
                                <div class="panel-heading">
                                    <div style="display: flex; flex-wrap: nowrap; justify-content: space-between;">
                                        <h3 class="panel-title"><b>EPIs Antigos</b></h3>
                                        <asp:LinkButton runat="server" ID="cmdFechar_EPIsAntigos" class="btn-default" Style="background: none; border: none;" data-toggle="tooltip" title="Fechar EPIs Antigos" OnClick="cmdFechar_EPIsAntigos_Click"><i class="fa fa-times"></i></asp:LinkButton>
                                    </div>
                                </div>
                                <div class="panel-body">
                                    <div class="form-stacked row">
                                        <div class="col-lg-12 form-group">
                                            <asp:GridView ID="dtgEPI" class="table table-striped table-bordered table-hover table-condensed"
                                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small"
                                                OnRowDataBound="dtgEPI_RowDataBound" OnRowCommand="dtgEPI_RowCommand">
                                                <Columns>

                                                    <asp:BoundField DataField="idLinha" HeaderText="idLinha">
                                                        <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sEPI" HeaderText="Descrição">
                                                        <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sTamanhoEPI" HeaderText="Tamanho">
                                                        <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sQuantidadeEPI" HeaderText="Quantidade">
                                                        <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="dtRecebimentoEPI" HeaderText="Data de Recebimento">
                                                        <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="dtVencimentoEPI" HeaderText="Data de Vencimento">
                                                        <ItemStyle Width="12%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sCA" HeaderText="CA">
                                                        <ItemStyle Width="8%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sObservacaoEPI" HeaderText="Observação">
                                                        <ItemStyle Width="25%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:TemplateField HeaderText="">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkEPI_Download" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Download do Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>
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
                    </div>

                    <div class="panel panel-default" runat="server" id="Div_Roupas">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Roupas</b></h3>
                        </div>

                        <div class="panel-body">
                            <div class="form-stacked row">

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <div class="row" runat="server" id="Div_Roupas_Selecao">

                                            <uc1:MensagemPagina runat="server" ID="Roupas_Mensagem" />

                                            <div class="col-lg-4">
                                                <div class="form-group">
                                                    <label>Tipo</label>
                                                    <asp:DropDownList ID="Roupas_ddlidTipoRoupa" class="form-control" runat="server"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Tamanho</label>
                                                    <asp:DropDownList ID="Roupas_ddlidTamanho" class="form-control" runat="server"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Quantidade </label>
                                                    <asp:TextBox ID="Roupas_txtQuantidade" class="form-control" runat="server" MaxLength="2"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Cor</label>
                                                    <asp:TextBox ID="Roupas_txtsCor" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Data Entrega</label>
                                                    <asp:TextBox ID="Roupas_txtdtEntrega" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-12">
                                                <div class="form-group">
                                                    <label>Observação</label>
                                                    <asp:TextBox ID="Roupas_txtsObservacao" class="form-control" runat="server" MaxLength="200" TextMode="MultiLine" Height="100px"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <asp:Button ID="Roupas_cmdIncluir" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="Roupas_cmdIncluir_Click" />
                                                <asp:Button ID="Roupas_BtnSalvar" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Salvar" OnClick="Roupas_BtnSalvar_Click" />
                                            </div>

                                        </div>

                                        <br />

                                        <div class="row">
                                            <div class="col-lg-12">
                                                <asp:GridView ID="Roupas_GV" runat="server" AutoGenerateColumns="False" CellPadding="1" CellSpacing="1"
                                                    class="table table-striped table-bordered table-hover table-condensed" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" GridLines="None" ShowFooter="False" Width="100%"
                                                    OnRowDeleting="Roupas_GV_RowDeleting" OnRowDataBound="Roupas_GV_RowDataBound" OnRowCommand="Roupas_GV_RowCommand">
                                                    <Columns>

                                                        <asp:BoundField DataField="idlinha" HeaderText="idLinha">
                                                            <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="idRegistroRoupa" HeaderText="idRegistroRoupa">
                                                            <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sDscTipoRoupa" HeaderText="Tipo">
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="15%" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sDscTamanho" HeaderText="Tamanho">
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="8%" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade">
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="6%" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sCorRoupa" HeaderText="Cor">
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="6%" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="DtEntregaRoupa" HeaderText="Data Entrega">
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="10%" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sObservacao" HeaderText="Observações">
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="25%" />
                                                        </asp:BoundField>

                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkRoupasEditar" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Editar" CssClass="btn btn-small" TabIndex="100" CommandName="Editar"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkRoupas_UpLoad" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Upload do Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Upload_Arquivo"><i class="fa-upload fa"></i></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkRoupas_Download" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Download do Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkRoupas_Excluir" CommandArgument='<%# Bind("idLinha") %>' runat="server" CssClass="btn btn-small" TabIndex="100" data-toggle="tooltip" title="Excluir" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="12%" />
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

                </div>

                <div role="tabpanel" class="tab-pane fade" id="Evento" aria-labelledby="Evento-tab">
                    <br />
                    <%--  -------------------------------------------------%>
                    <div class="panel panel-default" runat="server" id="Div_atestado">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Ausência/Faltas</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked">

                                 <div class="form-group row">
                                     <div class="col-lg-8 row">
         
                                             <div class="col-lg-2">
                                                 <div class="form-group">
                                                     <asp:TextBox ID="txtdtInicioAtestado_Pesquisa" class="form-control CaixaTextoData" placeholder="Inicio" runat="server" MaxLength="10"></asp:TextBox>
                                                 </div>
                                             </div>
                                             <div class="col-lg-2">
                                                 <div class="form-group">
                                                     <asp:TextBox ID="txtdtFinalAtestado_Pesquisa" class="form-control CaixaTextoData " runat="server" placeholder="Final" MaxLength="10"></asp:TextBox>
                                                 </div>
                                             </div>


                                             <div class="col-lg-4">
                                                 <div class="form-group">
                                                     <asp:Button ID="cmdPesquisarAtestado" class="btn btn-info" runat="server" Text="Buscar" OnClick="cmdPesquisarAtestado_Click" OnClientClick="MudarBotao()" />
                                                    <asp:Button ID="cmdIncluirAusencias" class="btn btn-success" runat="server" ValidationGroup="Item" Text="Nova Ausência" OnClick="cmdIncluirAusencia_Click" />

                                                 </div>
                                             </div>

                                            
                                     </div>
                                 </div>


                                                       

                                 <div class="row">
                                                    <div class="col-lg-12">
                                                        <asp:GridView ID="dtg_Ausencia" class="table table-striped table-bordered table-hover table-condensed table-responsive table-hover"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            Font-Names="Tahoma" ShowFooter="False" Font-Overline="False" Font-Size="Small" ClientIDMode="Static"
                                                            OnRowDataBound="dtgAtestado_RowDataBound" OnRowDeleting="dtg_Ausencia_RowDeleting" OnRowCommand="dtg_Ausencia_RowCommand">
                                                            <Columns>

                                                                <asp:BoundField DataField="idLinha" HeaderText="idLinha">
                                                                    <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>
                                                                
                                                                <asp:BoundField DataField="sTipoAusencia" HeaderText="Tipo">
                                                                    <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sMotivoAtestado" HeaderText="Motivo">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sDscAtestado" HeaderText="Descrição">
                                                                    <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sCid" HeaderText="CID">
                                                                    <ItemStyle Width="4%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sTipoAtestado" HeaderText="Período">
                                                                    <ItemStyle Width="4%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="nHorasAtestado" HeaderText="Horas Ausência" DataFormatString="{0:N2}">
                                                                    <ItemStyle Width="6%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="dtInicioAtestado" HeaderText="Dt Saída">
                                                                    <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="dtRetornoAtestado" HeaderText="Dt Retorno">
                                                                    <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sDescontoVT" HeaderText="Desc VT">
                                                                    <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sDescontoVR" HeaderText="Desc VR">
                                                                    <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="sDescontoDSR" HeaderText="Desc DSR">
                                                                    <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:BoundField>


                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkAtestadoEditar" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Editar" CssClass="btn btn-small" TabIndex="100" CommandName="Editar"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="lnkAtestado_UpLoad" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Upload do Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Upload_Arquivo"><i class="fa-upload fa"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="lnkAtestado_Download" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Download do Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="lnkAtestado_Excluir" CommandArgument='<%# Bind("idLinha") %>' runat="server" CssClass="btn btn-small" TabIndex="100" data-toggle="tooltip" title="Excluir" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
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

                    <%-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>

                    <div class="panel panel-default" runat="server" id="Div_Credito">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Adicionais de (VT e VR)</b></h3>
                        </div>

                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaCredito" />
                                </div>
                                <div class="col-lg-12">
                                    <div class="form-group">

                                        <div class="row" runat="server" id="div4">

                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>Justificativa do Crédito</label>
                                                    <asp:TextBox ID="sDscJustificativa" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-4">
                                                <div class="form-group">
                                                    <label>Tipo Adicional: </label>
                                                    <asp:DropDownList ID="ddlsTipoCredito" runat="server" class="form-control yes_no select" disabled="">
                                                        <asp:ListItem Value="Dia">Dia</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Data Início </label>
                                                    <asp:TextBox ID="txtCredito_dtInicioCredito" class="form-control " runat="server" type="date"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Data Final</label>
                                                    <asp:TextBox ID="txtCredito_dtFinalCredito" class="form-control " runat="server" type="date"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-4">
                                                <div class="form-group">
                                                    <label>Adicional no VR? </label>
                                                    <asp:DropDownList ID="ddlCreditaVR" runat="server" class="form-control yes_no select">
                                                        <asp:ListItem Value="">Selecione uma opção</asp:ListItem>
                                                        <asp:ListItem Value="S">Sim</asp:ListItem>
                                                        <asp:ListItem Value="N">Não</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-4">
                                                <div class="form-group">
                                                    <label>Adicional no VT? </label>
                                                    <asp:DropDownList ID="ddlCreditaVT" runat="server" class="form-control yes_no select">
                                                        <asp:ListItem Value="">Selecione uma opção</asp:ListItem>
                                                        <asp:ListItem Value="S">Sim</asp:ListItem>
                                                        <asp:ListItem Value="N">Não</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-12">
                                                <div class="form-group">
                                                    <label>Observação</label>
                                                    <asp:TextBox ID="txtCredito_sObservacaoCredito" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="120px"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <asp:Button ID="btnCredito_Incluir" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdIncluirCredito_Click" />
                                                <asp:Button ID="btnCredito_Salvar" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Salvar" OnClick="BtnSalvarCredito_Click" />
                                            </div>

                                        </div>

                                        <br />

                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                <h3 class="panel-title">Filtro para pesquisa </h3>
                                            </div>
                                            <div class="panel-body ">

                                                <div class="form-group">
                                                    <div class="col-lg-4">
                                                        <div class="row">

                                                            <div class="col-lg-2">
                                                                <div class="form-group">
                                                                    <asp:Button ID="btnCredito_btnBuscar" class="btn btn-info" runat="server" Text="Exibir Adicionais" OnClick="cmdPesquisarCredito_Click" OnClientClick="MudarBotao()" />
                                                                </div>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-lg-12">
                                                        <asp:GridView ID="dtgCredito" class="table table-striped table-bordered table-hover table-condensed"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            Font-Names="Tahoma" ShowFooter="False" Font-Overline="False" Font-Size="Small"
                                                            OnRowDataBound="dtgCredito_RowDataBound" OnRowDeleting="dtgCredito_RowDeleting" OnRowCommand="dtgCredito_RowCommand">
                                                            <Columns>

                                                                <asp:BoundField DataField="idLinha" HeaderText="idLinha">
                                                                    <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sDscJustificativa" HeaderText="Descrição">
                                                                    <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="nHorasCredito" HeaderText="Horas">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sTipoCredito" HeaderText="Período">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="dtInicioCredito" HeaderText="Data de Início ">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="dtRetornoCredito" HeaderText="Data Final">
                                                                    <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sObservacaoCredito" HeaderText="Observação">
                                                                    <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkCreditoEditar" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Editar" CssClass="btn btn-small" TabIndex="100" CommandName="Editar"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="lnkCredito_UpLoad" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Upload do Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Upload_Arquivo"><i class="fa-upload fa"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="lnkCredito_Download" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Download do Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>
                                                                        <asp:LinkButton ID="lnkCredito_Excluir" CommandArgument='<%# Bind("idLinha") %>' runat="server" CssClass="btn btn-small" TabIndex="100" data-toggle="tooltip" title="Excluir" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
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
                        </div>
                    </div>

                    <%------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>

                    <div class="panel panel-default" runat="server" id="Div_Evento">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Eventos</b></h3>
                        </div>

                        <div class="panel-body">
                            <div class="form-stacked row">

                                <div class="col-lg-12">
                                    <div class="form-group">

                                        <br />

                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                <h3 class="panel-title">Filtro para pesquisa </h3>
                                            </div>
                                            <div class="panel-body ">

                                                <div class="form-group">
                                                    <div class="col-lg-4">
                                                        <div class="row">
                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <asp:TextBox ID="txtdtdtInicioAfastamento_Pesquisa" class="form-control CaixaTextoData" placeholder="Inicio" runat="server" MaxLength="10"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <asp:TextBox ID="txtdtdtFinalAfastamento_Pesquisa" class="form-control CaixaTextoData " runat="server" placeholder="Final" MaxLength="10"></asp:TextBox>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-2">
                                                                <div class="form-group">
                                                                    <asp:Button ID="PesquisarAfastamento" class="btn btn-info" runat="server" OnClick='cmdPesquisarAfastamento_Click' Text="Buscar" OnClientClick="MudarBotao()" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-lg-12 form-group table-responsive">
                                                        <asp:GridView ID="dtgEvento" class="table table-striped table-bordered table-hover table-condensed"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" ClientIDMode="Static"
                                                            OnRowCommand="dtgEvento_RowCommand">
                                                            <Columns>

                                                                <asp:BoundField DataField="idRegistroEvento" HeaderText="ID">
                                                                    <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sDscEvento" HeaderText="Descrição">
                                                                    <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sReincidencia" HeaderText="Reincidência ">
                                                                    <ItemStyle Width="4%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sDscGravidade" HeaderText="Gravidade">
                                                                    <ItemStyle Width="6%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sTipoEvento" HeaderText="Tipo">
                                                                    <ItemStyle Width="6%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sCodReferencia" HeaderText="Referência">
                                                                    <ItemStyle Width="6%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="dtEvento" HeaderText="Data Evento">
                                                                    <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sCustoOcorrencia" HeaderText="Custo">
                                                                    <ItemStyle Width="4%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="nValorOcorrencia" HeaderText="Valor">
                                                                    <ItemStyle Width="6%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sResponsavelOcorrencia" HeaderText="Responsável">
                                                                    <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sAfastamentoEvento" HeaderText="Afastamento">
                                                                    <ItemStyle Width="4%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="dtInicioAfastamento" HeaderText="Data Início ">
                                                                    <ItemStyle Width="6%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="dtRetornoAfastamento" HeaderText="Data Retorno">
                                                                    <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sObservacaoEvento" HeaderText="Obs Evento">
                                                                    <ItemStyle Width="15%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkEventoEditar" CommandArgument='<%# Bind("idRegistroEvento") %>' runat="server" data-toggle="tooltip" title="Editar" CssClass="btn btn-small" TabIndex="100" CommandName="Editar"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
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
                        </div>

                    </div>

                    <%----------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>

                    <div class="panel panel-default" runat="server" id="Div_Veiculos">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Veículos / Infrações / Ocorrências</b></h3>
                        </div>

                        <div class="panel-body">
                            <div class="form-stacked row">

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <asp:Button ID="cmdIncluirOcorrencias" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Nova Infração" OnClick="cmdIncluirOcorrencias_Click" />
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">

                                        <div class="row table-responsive">
                                            <div class="col-lg-12">

                                                <asp:GridView ID="dtgOcorrencias" class="table table-striped table-bordered table-hover table-condensed"
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" ClientIDMode="Static"
                                                    OnRowDataBound="dtgOcorrencias_RowDataBound" OnRowDeleting="dtgOcorrencias_RowDeleting" OnRowCommand="dtgOcorrencias_RowCommand">
                                                    <Columns>

                                                        <asp:BoundField DataField="idLinha" HeaderText="idLinha">
                                                            <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sTipoOcorrencia" HeaderText="Tipo da Ocorrência">
                                                            <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="dtOcorrencia" HeaderText="Data Ocorrência">
                                                            <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sDscInfracao" HeaderText="Descrição">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sVeiculoOcorrencia" HeaderText="Veículo">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="dtInfracao" HeaderText="Data Infração">
                                                            <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="dtLimiteIndicarCondutor" HeaderText="Data Indicar Condutor">
                                                            <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="dtLimitePagamento" HeaderText="Data Limite Pagamento">
                                                            <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="nValor" HeaderText="Valor">
                                                            <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sPontos" HeaderText="Pontos">
                                                            <ItemStyle Width="4%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sStatus" HeaderText="Status">
                                                            <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sAIT" HeaderText="AIT">
                                                            <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sResponsavel" HeaderText="Responsável">
                                                            <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:TemplateField HeaderText="Observação">
                                                            <ItemTemplate>
                                                                <div data-toggle="tooltip" title='<%# Eval("sObservacaoInfracao").ToString() %>'>
                                                                    <%#Eval("sObservacaoInfracao") %>
                                                                </div>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="11%" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkOcorrenciaEditar" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Editar" CssClass="btn btn-small" TabIndex="100" CommandName="Editar"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkOcorrencia_UpLoad" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Upload do Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Upload_Arquivo"><i class="fa-upload fa"></i></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkOcorrencia_Download" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Download do Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkOcorrencia_Excluir" CommandArgument='<%# Bind("idLinha") %>' runat="server" CssClass="btn btn-small" TabIndex="100" data-toggle="tooltip" title="Excluir" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
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

                    <%------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>

                    <div class=" embed-responsive embed-responsive-16by9" style="min-height: 200px" runat="server" id="DIV_Documentos_Evento">
                        <embed type="text/html" runat="server" id="frmEvento" width="800" height="500" />
                    </div>

                </div>

                <div role="tabpanel" class="tab-pane fade" id="beneficios" aria-labelledby="beneficios-tab">
                    <br />

                    <div class="panel panel-default" runat="server" id="Div_VR">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>VR</b></h3>
                        </div>

                        <div class="panel-body">
                            <div class="form-stacked row">

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>N° Cartão VR</label>
                                        <asp:TextBox ID="txtsVR" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>VR Definitivo? </label>
                                        <asp:DropDownList ID="ddlsVRDefinitivo" runat="server" class="form-control yes_no select">
                                            <asp:ListItem Value="S">Sim</asp:ListItem>
                                            <asp:ListItem Value="N">Não</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Valor/Dia VR</label>
                                        <asp:TextBox ID="txtnValorDiaVR" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

                    <div class="panel panel-default" runat="server" id="Div_VT">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>VT</b></h3>
                        </div>

                        <div class="panel-body">

                            <div class="form-group row">

                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemVT" />
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Optou pelo VT? </label>
                                        <asp:DropDownList ID="ddlVT_idOpcaoVT" runat="server" class="form-control yes_no select" AutoPostBack="True" OnSelectedIndexChanged="ddlVT_sOpcaoVT_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-8" runat="server" id="Div_txtVT_sDscMotivo">
                                    <div class="form-group">
                                        <label>Motivo </label>
                                        <asp:TextBox ID="txtVT_sDscMotivo" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="Div_ddlVT_sTipo">
                                    <div class="form-group">
                                        <label>Tipo VT </label>
                                        <asp:DropDownList ID="ddlVT_idTipoVT" runat="server" class="form-control yes_no select" OnSelectedIndexChanged="ddlVT_idTipoVT_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="div_tipoVT_outros" visible="false">
                                    <div class="form-group">
                                        <label>Outros </label>
                                        <asp:TextBox ID="txtVtTipoOutro" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="Div_txtVT_sNumeroVT">
                                    <div class="form-group">
                                        <label>Número Cartão </label>
                                        <asp:TextBox ID="txtVT_sNumeroVT" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="Div_nValorVT">
                                    <div class="form-group">
                                        <label>Valor </label>
                                        <asp:TextBox ID="txtVT_nValorVT" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="Div_ddlVT_sVTDefinitivo">
                                    <div class="form-group">
                                        <label>Definitivo </label>
                                        <asp:DropDownList ID="ddlVT_idVTDefinitivo" runat="server" class="form-control yes_no select"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Data </label>
                                        <asp:TextBox ID="txtVT_dtVT" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <br />
                                        <asp:Button ID="cmdVT_Incluir" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdVT_Incluir_Click" />
                                        <asp:Button ID="BtnSalvarVT" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Salvar" OnClick="BtnSalvarVT_Click" />
                                    </div>
                                </div>

                                <br />

                                <asp:HiddenField ID="hddVT_idLinha" runat="server" />

                                <div class="col-lg-12">
                                    <asp:GridView ID="dtgVT" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="True" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" ClientIDMode="Static"
                                        OnRowDataBound="dtgVT_RowDataBound" OnRowCommand="dtgVT_RowCommand" OnRowDeleting="dtgVT_RowDeleting1">
                                        <Columns>

                                            <asp:BoundField DataField="idlinha" HeaderText="idLinha">
                                                <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="idRegistroVT" HeaderText="idLinha">
                                                <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sTipoVt" HeaderText="Tipo VT">
                                                <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sNumeroVT" HeaderText="Número Cartão">
                                                <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nValorVT" HeaderText="Valor">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscMotivo" HeaderText="Motivo">
                                                <ItemStyle Width="18%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sVTDefinitivo" HeaderText="Definitivo">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtVT" HeaderText="Data">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkVTEditar" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Editar" CssClass="btn btn-small" TabIndex="100" CommandName="Editar"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="lnkVT_UpLoad" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Upload do Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Upload_Arquivo"><i class="fa-upload fa"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="lnkVT_Download" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Download do Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="lnkVT_Excluir" CommandArgument='<%# Bind("idLinha") %>' runat="server" CssClass="btn btn-small" TabIndex="100" data-toggle="tooltip" title="Excluir" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                        </Columns>
                                    </asp:GridView>
                                </div>

                            </div>
                        </div>
                    </div>

                    <div class="panel panel-default" runat="server" id="Div_PlanoSaude">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Plano de Saúde</b></h3>
                        </div>

                        <div class="panel-body">

                            <div class="form-group row">

                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_PlanoSaude" />
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Tem Plano de Saúde?</label>
                                        <asp:DropDownList ID="ddlTemPlanoSaude" runat="server" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlTemPlanoSaude_SelectedIndexChanged">
                                            <asp:ListItem Text="Sim" Value="S"></asp:ListItem>
                                            <asp:ListItem Text="Não Tem Interesse" Value="N"></asp:ListItem>
                                            <asp:ListItem Text="Não Definido" Value="I" Selected="True"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div runat="server" id="div_arquivoPlanoSaude" visible="false">

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <asp:Button runat="server" ID="btnArquivoPlanoSaude" Text="Carta Desinteresse" CssClass="btn btn-primary" OnClick="btnArquivoPlanoSaude_Click" Style="margin-top: 24px" />
                                        </div>
                                    </div>

                                </div>

                                <div class="col-lg-2 form-group" runat="server" id="div_btnIncluirPlanoSaude">
                                    <asp:Button ID="btnIncluirPlanoSaude" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="btnIncluirPlanoSaude_Click" Style="margin-top: 24px" />
                                </div>

                                <div class="col-lg-12">
                                    <asp:GridView ID="gv_PlanoSaude" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="idLinha, idArquivo" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small"
                                        OnRowDataBound="gv_PlanoSaude_RowDataBound" OnRowDeleting="gv_PlanoSaude_RowDeleting" OnRowCommand="gv_PlanoSaude_RowCommand">
                                        <Columns>

                                            <asp:BoundField DataField="idLinha" HeaderText="idLinha">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Beneficiário">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblBeneficiario" runat="server" Text='<%# Eval("sDscBeneficiario") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="sTipoBeneficiario" HeaderText="Tipo Beneficiário">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sTipo" HeaderText="Tipo Plano">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Descrição">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbsDscPlanoSaude" runat="server" Text='<%# Eval("sDscPlanoSaude") %>' />
                                                </ItemTemplate>
                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="dtInclusao" HeaderText="Data Inclusão">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtFimCarencia" HeaderText="Data Fim Carência">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nValorPlano" HeaderText="Valor Plano" DataFormatString="{0:N2}">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkEditar" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Editar" CssClass="btn btn-small" TabIndex="100" CommandName="Editar"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="lnkExcluir" CommandArgument='<%# Bind("idLinha") %>' runat="server" CssClass="btn btn-small" TabIndex="100" data-toggle="tooltip" title="Excluir" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                        </Columns>
                                    </asp:GridView>
                                </div>

                            </div>
                        </div>
                    </div>

                    <div class="panel panel-default" runat="server" id="DivSeguro">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Seguro de Vida</b></h3>
                        </div>
                        <div class="panel-body row">

                            <div class="col-lg-2">
                                <div class="form-group">
                                    <label>Seguro de Vida</label>
                                    <asp:DropDownList ID="ddlSeguroVida" runat="server" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlSeguroVida_SelectedIndexChanged">
                                        <asp:ListItem Value="N">Não</asp:ListItem>
                                        <asp:ListItem Value="S">Sim</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div id="div_tipoSeguro" runat="server" visible="false">
                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <label>Tipo Seguro</label>
                                        <asp:DropDownList ID="ddlsTipoSeguro" runat="server" class="form-control"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-1">
                                    <div class="form-group">
                                        <label>Data Início</label>
                                        <asp:TextBox runat="server" ID="txtdtInicioSeguro" class="form-control" TextMode="Date"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>

                    <div class="panel panel-default" runat="server" id="Div_Equipamentos">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Equipamentos/Veículo</b></h3>
                        </div>

                        <div class="panel-body">
                            <div class="col-lg-12 row">
                                <uc1:MensagemPagina runat="server" ID="MensagemEquipamentos" />
                            </div>

                            <div class="form-group row">
                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Descrição do Equipamento</label>
                                        <asp:TextBox ID="txtsDcsTipoEquipamento" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <label>Veículo</label>
                                        <asp:DropDownList ID="ddlidVeiculoEquipamentos" runat="server" class="form-control yes_no select"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Modelo Equipamento</label>
                                        <asp:TextBox ID="txtsModeloEquipamento" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>TAG Patrimônio</label>
                                        <asp:TextBox ID="txtsCodTTEquipamento" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Data Entrega</label>
                                        <asp:TextBox ID="txtdtRecebimentoEquipamento" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Data Devolução</label>
                                        <asp:TextBox ID="txtdtDevolucaoEquipamento" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Observação</label>
                                        <asp:TextBox ID="txtsObservacaoEquipamento" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="120px"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <asp:Button ID="cmdEquipamentos_Incluir" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdIncluirEquipamentos_Click" />
                                    <asp:Button ID="BtnSalvarEquipamento" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Salvar" OnClick="BtnSalvarEquipamento_Click" />
                                </div>

                            </div>

                            <asp:HiddenField ID="hddEquipamento_idLinha" runat="server" />
                            <br />

                            <div class="row">
                                <div class="col-lg-12">
                                    <asp:GridView ID="dtgEquipamentos" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small"
                                        OnRowDataBound="dtgEquipamentos_RowDataBound" OnRowDeleting="dtgEquipamentos_RowDeleting" OnRowCommand="dtgEquipamentos_RowCommand">
                                        <Columns>

                                            <asp:BoundField DataField="idLinha" HeaderText="idLinha">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDcsTipoEquipamento" HeaderText="Descrição">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sModeloEquipamento" HeaderText="Modelo">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sCodTTEquipamento" HeaderText="Código TT">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtRecebimentoEquipamento" HeaderText="Data Recebimento">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtDevolucaoEquipamento" HeaderText="Data Devolução">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sObservacaoEquipamento" HeaderText="Observação">
                                                <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkEquipamentoEditar" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Editar" CssClass="btn btn-small" TabIndex="100" CommandName="Editar"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="lnkEquipamento_UpLoad" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Upload do Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Upload_Arquivo"><i class="fa-upload fa"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="lnkEquipamento_Download" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Download do Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="lnkEquipamento_Excluir" CommandArgument='<%# Bind("idLinha") %>' runat="server" CssClass="btn btn-small" TabIndex="100" data-toggle="tooltip" title="Excluir" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>

                        </div>

                    </div>

                    <div class="panel panel-default" runat="server" id="div_ControleFerias">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Controle Férias</b></h3>
                        </div>

                        <div class="panel-body row">
                            <div class="col-lg-12 row">
                                <uc1:MensagemPagina runat="server" ID="MensagemPaginaControleFerias" />
                            </div>

                            <div class="col-lg-12 form-group">
                                <div class="col-lg-1">
                                    <div class="form-group">
                                        <label>Data Início</label>
                                        <asp:TextBox ID="txtdtInicioFerias" class="form-control" runat="server" TextMode="Date" Style="inline-size: 120px;"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-1">
                                    <div class="form-group">
                                        <label>Data Final</label>
                                        <asp:TextBox ID="txtdtFinalFerias" class="form-control" runat="server" TextMode="Date" Style="inline-size: 120px;"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group" style="margin-top: 24px;">
                                        <asp:Button ID="btnFiltroFerias" class="btn btn-primary" runat="server" Text="Buscar" OnClick="btnFiltroFerias_Click" />
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-12">
                                <asp:GridView ID="dtgv_ControleFerias" class="table table-striped table-bordered table-hover table-condensed"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="idControleFerias"
                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                    <Columns>

                                        <asp:HyperLinkField DataNavigateUrlFields="idControleFerias"
                                            DataTextField="dtEvento" HeaderText="Data Início"
                                            DataNavigateUrlFormatString="ControleFerias.aspx?id={0}" Target="_blank">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>
                                        
                                        <asp:HyperLinkField DataNavigateUrlFields="idControleFerias"
                                            DataTextField="dtFinal" HeaderText="Data Final"
                                            DataNavigateUrlFormatString="ControleFerias.aspx?id={0}" Target="_blank">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>
                                        
                                        <asp:HyperLinkField DataNavigateUrlFields="idControleFerias"
                                            DataTextField="sDscEvento" HeaderText="Tipo"
                                            DataNavigateUrlFormatString="ControleFerias.aspx?id={0}" Target="_blank">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:BoundField DataField="sDescricao" HeaderText="Descrição">
                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="nDias" HeaderText="Dias">
                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="nSaldo" HeaderText="Saldo">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                    </Columns>
                                </asp:GridView>
                            </div>

                        </div>
                    </div>

                    <div class="panel panel-default" runat="server" id="Div_Outros">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Outros</b></h3>
                        </div>

                        <div class="panel-body row">

                            <div class="col-lg-12 ">
                                <div class="form-group">
                                    <label>Observação </label>
                                    <asp:TextBox ID="txtsObsBeneficios" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="120px"></asp:TextBox>
                                </div>
                            </div>

                        </div>
                    </div>

                </div>

                <div role="tabpanel" class="tab-pane fade" id="conversas" aria-labelledby="conversas-tab">
                    <br />

                    <div class="panel panel-default" runat="server" id="Div_Conversas">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Conversas</b></h3>
                        </div>

                        <uc1:MensagemPagina runat="server" ID="MensagemPaginaConversas2" />

                        <div runat="server" class="panel-body" id="DIV_NOVOASSUNTO">
                            <asp:Button ID="btnNovaConversa" class="btn btn-success" runat="server" ValidationGroup="Item" Text="Nova Conversa" OnClick="cmdConversas_Novo_Click" />
                        </div>

                        <div id="DIV_ASSUNTO" runat="server" class="panel-body text-left " style="text-align: left">

                            <div class="row">
                                <asp:Repeater ID="rptConversas" runat="server" OnItemDataBound="rptConversas_ItemDataBound">
                                    <ItemTemplate>
                                        <!-- Início do Card da Conversa -->
                                        <div class="col-md-4">
                                            <div class="panel panel-default">
                                                <div class="panel-heading">
                                                    <h3 class="panel-title">
                                                        <div class="chatpanel-header">
                                                            <div>
                                                        <span id="sCor" runat="server"></span>
                                                        <b>Assunto:</b>
                                                            </div>
                                                        </div>

                                                        <p class="texto-resumido">
                                                            <asp:Literal runat="server" Text='<%# Eval("sDscConversa") %>'></asp:Literal>
                                                        </p>
                                                    </h3>
                                                </div>
                                                <div class="panel-body" style="min-height: 150px">
                                                    <p>
                                                        <b>Status da Solicitação:</b>
                                                        <asp:Literal runat="server" Text='<%# Eval("sDscStatusConversa") %>'></asp:Literal>
                                                    </p>
                                                    <p>
                                                        <b>Data da solicitação:</b>
                                                        <asp:Literal runat="server" Text='<%# Eval("dtEventoConversa") %>'></asp:Literal>
                                                    </p>
                                                    <div>
                                                        <p id="pDtProxConv" runat="server">
                                                            <b>Data da Próxima Conversa</b>
                                                            <asp:Literal ID="litDtProximaConversa" runat="server" Text='<%# Eval("dtProximaConversa") %>'></asp:Literal>
                                                        </p>
                                                    </div>
                                                    <p>
                                                        <b>Tipo de Solicitação:</b>
                                                        <asp:Literal runat="server" Text='<%# Eval("sDscTipoEvento") %>'></asp:Literal>
                                                    </p>

                                                </div>
                                                <div class="panel-footer text-center">
                                                    <asp:LinkButton ID="lnkConversasEditar" runat="server" data-toggle="tooltip" title="Ver Detalhes" CssClass="btn btn-primary" TabIndex="100" CommandName="VerDetalhes" OnClick="lnkConversasEditar_Click" CommandArgument='<%# Eval("idRegistroConvesa") %>'>
                                                            <i class="fa fa-eye"></i> Ver Detalhes <span class="badge badge-danger"><%# Eval("nQtdConversasPendentes") %></span>
                                                            <span class="sr-only">Não Lidas</span>
                                                    </asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                        <!-- Fim do Card da Conversa -->
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>


                        </div>

                        <div class="panel-body" runat="server" id="DIV_CONVERSA">

                            <div class="form-group row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaConversas" />
                                </div>

                                <div runat="server" id="Div_Inclusao">
                                    <div class="form-group row">
                                        <div class="col-lg-12">

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Tipo de Solicitação </label>
                                                    <asp:DropDownList ID="ddlConversas_idTipoEvento" class="form-control" runat="server"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>Assunto </label>
                                                    <asp:TextBox ID="txtConversas_sDscConversa" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Meio </label>
                                                    <asp:DropDownList ID="ddlConversas_idMeio" class="form-control" runat="server"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Ação </label>
                                                    <asp:DropDownList ID="ddlConversas_idAcao" class="form-control" runat="server"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-2" runat="server" id="Div_resolucao">
                                                <div class="form-group">
                                                    <label>Data de Resolução </label>
                                                    <asp:TextBox ID="txtConversas_dtResolucaoConversa" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>

                                <asp:HiddenField ID="hddConversa_idRegistroConvesa" runat="server" />
                            </div>


                            <div runat="server" id="DIV_Historico" class="historico">
                                <div class="chat-panel panel panel-default">
                                    <div class="panel-heading">
                                        <i class="fa fa-comments fa-fw"></i>
                                        <b>
                                            <asp:Label ID="lblAssunto" runat="server"></asp:Label>
                                        </b>

                                        <div class="row">
                                            <div class="col-lg-12">
                                                <div class="form-inline float-right">

                                                    <div id="div_statusConv" runat="server" class="col-lg-2">
                                                        <label>Status</label>
                                                        <br />
                                                        <asp:DropDownList ID="ddlConversas_idStatusConversa" class="form-control mr-2" runat="server"></asp:DropDownList>
                                                    </div>

                                                    <div id="div_dtProxConv" runat="server" class="col-lg-2">
                                                        <b>Próxima Conversa</b>
                                                        <asp:TextBox ID="txtConversas_dtProximaConversa" class="form-control mr-2" runat="server" type="date"></asp:TextBox>
                                                    </div>
                                                    <div class="col-lg-4">
                                                        <br />
                                                        <asp:Button class="btn btn-success" runat="server" ValidationGroup="Item" ID="btnSalvarStatus" Text="Salvar" OnClick="cmdConversas_SalvarDtStt_Click" />
                                                        <asp:Button class="btn btn-primary" runat="server" ValidationGroup="Item" ID="btnArquivoChat" Text="Adicionar Arquivo" OnClick="btnArquivoConversa_Click"/>
                                                        <asp:Button ID="cmdConversas" class="btn btn-warning btn-sm" Style="margin-left: 2px" runat="server" ValidationGroup="Item" Text="Voltar" OnClick="cmdConversas_Cancelar_Click" />
                                                        <asp:Button class="btn btn-danger" runat="server" ValidationGroup="Item" ID="btnExcluir" Text="Excluir" OnClick="cmdConversas_Excluir_Click" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                    </div>

                                    <!-- /.panel-heading -->
                                    <div class="panel-body" id="conversasContainer">
                                        <ul class="chat">
                                            <asp:Repeater ID="rptHistorico" runat="server">
                                                <ItemTemplate>
                                                    <li class=" <%# GetLiClass(Eval("idUsuarioAtualizacao")) %> clearfix">
                                                        <span class="chat-img pull-<%# GetLiClass(Eval("idUsuarioAtualizacao")) %>">
                                                            <img src="<%# GetLiImagemColaborador(Eval("imgColaborador")) %>" width="50" height="50" class="img-circle" />
                                                        </span>
                                                        <div class="chat-body clearfix">
                                                            <div class="header hdChat">
                                                                <strong style="font-size: 85%;  " class='<%# string.Format("{0} primary-font", GetLiClassStrong(Eval("idUsuarioAtualizacao"))) %>'><%# Eval("sDscUsuario") %></strong>
                                                                <small class='<%# string.Format("{0} text-muted", GetLiClassSmall(Eval("idUsuarioAtualizacao"))) %>'>
                                                                    <i class="fa fa-clock-o fa-fw"></i><span class="message-date"><%# Eval("dtAtualizacao", "{0:dd/MM/yyyy HH:mm:ss}") %></span>
                                                                </small>
                                                            </div>
                                                            <p>
                                                                <pre style="white-space: pre-line; font-family: Helvetica Neue, Helvetica, Arial, sans-serif;">
                                                                     <%# Eval("sObservacaoConversa") %>
                                                                 </pre>
                                                            </p>
                                                        </div>
                                                    </li>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </ul>
                                    </div>
                                    <!-- /.panel-body -->
                                    <div class="panel-footer">
                                        <div class="input-group">
                                            <asp:TextBox ID="txtConversas_sObservacaoConversa" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="80px"></asp:TextBox>
                                            <span class="input-group-btn">
                                                <asp:Button ID="BtnSalvarConversas" class="btn btn-success btn-sm" Style="margin-left: 2px" runat="server" ValidationGroup="Item" Text="Enviar" OnClick="cmdConversas_Salvar_Click" />
                                                <br />
                                            </span>
                                        </div>
                                    </div>
                                    <!-- /.panel-footer -->
                                </div>
                            </div>
                            <asp:HiddenField ID="hddidUsuarioAtualizacao" runat="server" />
                        </div>

                    </div>

                </div>

                <div role="tabpanel" class="tab-pane fade" id="avaliacao" aria-labelledby="avaliacao-tab">
                    <br />

                    <div class="panel panel-default" runat="server" id="Div_Avaliacao">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Avaliação</b></h3>
                        </div>

                        <div class="panel-body">

                            <div class="form-group row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Avaliacao" />
                                </div>
                                <div>
                                    <div class="form-group row">
                                        <div class="col-lg-12">

                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>Tipo de Avaliação </label>
                                                    <asp:DropDownList ID="ddlidTipoAvaliacao" class="form-control" runat="server"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Data da Avaliação </label>
                                                    <asp:TextBox ID="txtdtAvaliacao" class="form-control " runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-12">
                                                <div class="form-group">
                                                    <label>Observação </label>
                                                    <asp:TextBox ID="txtsObservacaoAvaliacao" class="form-control" runat="server" MaxLength="400" TextMode="MultiLine" Height="120px"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-12">
                                                <div class="form-group">
                                                    <br />
                                                    <asp:Button ID="cmdAvaliacao" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdAvaliacao_Incluir_Click" />
                                                    <asp:Button ID="BtnAvaliacao" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Salvar" OnClick="BtnAvaliacao_Click" />
                                                </div>
                                            </div>

                                            <br />

                                            <asp:HiddenField ID="hddAvalicao_idLinha" runat="server" />

                                            <div class="col-lg-12">
                                                <asp:GridView ID="dtgAvaliacao" class="table table-striped table-bordered table-hover table-condensed"
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    ShowFooter="True" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgAvaliacao_RowDataBound" OnRowCommand="dtgAvaliacao_RowCommand" OnRowDeleting="dtgAvaliacao_RowDeleting">
                                                    <Columns>

                                                        <asp:BoundField DataField="idlinha" HeaderText="idLinha">
                                                            <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="idRegistroAvaliacao" HeaderText="idRegistroAvaliacao">
                                                            <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sDscAvaliacao" HeaderText="Tipo Avaliação">
                                                            <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="dtAvaliacao" HeaderText="Data da Avaliação">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sObservacaoAvaliacao" HeaderText="Observação">
                                                            <ItemStyle Width="35%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkAvaliacaoEditar" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Editar" CssClass="btn btn-small" TabIndex="100" CommandName="Editar"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkAvaliacao_UpLoad" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Upload do Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Upload_Arquivo"><i class="fa-upload fa"></i></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkAvaliacao_Download" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Download do Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkAvaliacao_Excluir" CommandArgument='<%# Bind("idLinha") %>' runat="server" CssClass="btn btn-small" TabIndex="100" data-toggle="tooltip" title="Excluir" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
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

                </div>

                <div role="tabpanel" class="tab-pane fade" id="arquivoMorto" aria-labelledby="arquivoMorto-tab">
                    <br />

                    <div class="panel panel-default" runat="server" id="Div8">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Arquivados</b></h3>
                        </div>

                        <div class="panel-body">
                            <div class="col-lg-12">
                                <asp:GridView ID="gv_ArquivoMorto" class="table table-striped table-bordered table-hover table-condensed"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="True" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gv_ArquivoMorto_RowDataBound" OnRowCommand="gv_ArquivoMorto_RowCommand">
                                    <Columns>

                                        <asp:BoundField DataField="idlinha" HeaderText="idLinha">
                                            <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="sDescricao" HeaderText="Descrição">
                                            <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="sNomeArquivo" HeaderText="Nome Arquivo">
                                            <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="dtExclusao" HeaderText="Data da Exclusão">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkArquivoMorto_Download" CommandArgument='<%# Bind("idLinha") %>' runat="server" data-toggle="tooltip" title="Download do Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>
                                            </ItemTemplate>
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>

                            </div>
                        </div>
                    </div>

                </div>

                <div id="DIV_Salvar" runat="server">
                    <fieldset class="form-stacked actions">
                        <asp:Button ID="Principal_cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" ValidationGroup="DETALHE" />
                        <asp:Button ID="Principal_cmdEditar" class="btn  btn-lg  btn-danger" runat="server" Text="Editar" />

                        <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field-cancel" title="Voltar" onclick="history.go(-2)">
                    </fieldset>
                </div>

            </div>

            <div id="dialog-Salvar" class="modal" title="Salvar">
                <p>
                    <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                    <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
                </p>
            </div>

            <div id="dialog-Editar" class="modal" title="Editar Colaborador">
                <p>
                    <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                    <asp:Label ID="lblTituloEdiar" runat="server" Text="Confirma a Edição da Requisição?"></asp:Label>
                </p>
            </div>

            <div class="modal fade" id="modal_Infracoes" tabindex="-1" role="dialog" data-backdrop="static" aria-labelledby="modal_Infracoes_Label" aria-hidden="true">
                <div class="modal-dialog modal-lg" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title" id="modal_Infracoes_Label"><b>
                                <asp:Label ID="Infracoes_lblTitulo" runat="server" Text="Infrações / Ocorrências"></asp:Label></b>
                            </h4>
                        </div>
                        <div class="modal-body">

                            <div class="col-lg-12 row">
                                <uc1:MensagemPagina runat="server" ID="Infracoes_MensagemPagina" />
                            </div>

                            <div class="row">

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Tipo</label>
                                        <asp:DropDownList ID="Infracoes_ddlidTipoOcorrencia" class="form-control" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlsTipoOcorrencia_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-6" runat="server" id="div_VeiculoOcorrencia">
                                    <div class="form-group">
                                        <label>Veículo</label>
                                        <asp:DropDownList ID="ddlidVeiculoOcorrencia" runat="server" class="form-control yes_no select"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-3" runat="server" id="div_dtOcorrencia">
                                    <div class="form-group">
                                        <label>Data Ocorrência</label>
                                        <asp:TextBox ID="txtdtOcorrencia" class="form-control " runat="server" TextMode="Date"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-12" runat="server" id="div_sDscInfracao">
                                    <div class="form-group">
                                        <label>Descrição</label>
                                        <asp:TextBox ID="txtsDscInfracao" class="form-control" MaxLength="100" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3" runat="server" id="div_Custo">
                                    <div class="form-group">
                                        <label>Custo</label>
                                        <asp:TextBox ID="txtnValor" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3" runat="server" id="div_Responsavel">
                                    <div class="form-group">
                                        <label>Responsável</label>
                                        <asp:DropDownList ID="ddlidResponsavelVeiculo" runat="server" class="form-control yes_no select">
                                            <asp:ListItem Value="C">Colaborador</asp:ListItem>
                                            <asp:ListItem Value="E">Empresa</asp:ListItem>
                                            <asp:ListItem Value="EC">Empresa/Colaborador</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-3" runat="server" id="div_status">
                                    <div class="form-group">
                                        <label>Status</label>
                                        <asp:DropDownList ID="ddlidStatusOcorrencia" runat="server" class="form-control yes_no select">
                                            <asp:ListItem Value="0">Selecione</asp:ListItem>
                                            <asp:ListItem Value="P">Paga</asp:ListItem>
                                            <asp:ListItem Value="Av">A vencer</asp:ListItem>
                                            <asp:ListItem Value="V">Vencida</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="div_Potnos">
                                    <div class="form-group">
                                        <label>Pontos</label>
                                        <asp:TextBox ID="txtsPontos" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3" runat="server" id="div_dtInfração">
                                    <div class="form-group">
                                        <label>Data Infração</label>
                                        <asp:TextBox ID="txtdtInfracao" class="form-control " runat="server" TextMode="Date"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3" runat="server" id="div_dtIndicarCondutor">
                                    <div class="form-group">
                                        <label>Data Indicar Condutor</label>
                                        <asp:TextBox ID="txtdtLimiteIndicarCondutor" class="form-control " runat="server" TextMode="Date"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3" runat="server" id="div_dtLimitePagamento">
                                    <div class="form-group">
                                        <label>Data Limite Pagamento</label>
                                        <asp:TextBox ID="txtdtLimitePagamento" class="form-control " runat="server" TextMode="Date"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3" runat="server" id="div_AIT">
                                    <div class="form-group">
                                        <label>AIT</label>
                                        <asp:TextBox ID="txtsAIT" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-12" runat="server" id="div_ObservacaoInfraca">
                                    <div class="form-group">
                                        <label>Observações </label>
                                        <asp:TextBox ID="txtsObservacaoInfracao" class="form-control" runat="server" MaxLength="6000" TextMode="MultiLine" Height="120px"></asp:TextBox>
                                    </div>
                                </div>

                            </div>

                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton CssClass="btn btn-success" ID="cmdInfracao_Salvar" Text="Salvar" runat="server" OnClick="cmdInfracao_Salvar_Click" />
                            <asp:LinkButton CssClass="btn btn-danger" ID="cmdInfracao_Cancelar" Text="Cancelar" runat="server" OnClick="Modal_Fechar" />
                        </div>
                    </div>
                </div>
            </div>



            <div id="hdd">
                <asp:HiddenField ID="hddidArquivoConversa" runat="server" />
                <asp:HiddenField ID="hddDependente_idLinha" runat="server" />
                <asp:HiddenField ID="hddNR_idLinha" runat="server" Value="" />
                <asp:HiddenField ID="hddEPI_idLinha" runat="server" />
                <asp:HiddenField ID="hddRoupas_idLinha" runat="server" />
                <asp:HiddenField ID="hddAusencia_idLinha" runat="server" />
                <asp:HiddenField ID="hddCredito_idLinha" runat="server" />
                <asp:HiddenField ID="hddEvento_idLinha" runat="server" />
                <asp:HiddenField ID="hddOcorrencias_idLinha" runat="server" />
                <asp:HiddenField ID="hddidColaborador" runat="server" />
                <asp:HiddenField ID="hddChecked" runat="server" />
                <asp:HiddenField ID="hddIdLinha" runat="server" />
                <asp:HiddenField ID="hddsBloco" runat="server" />
                <asp:HiddenField ID="hddidArquivoPlanoSaude" runat="server" />
                <asp:HiddenField ID="hddPlanoSaude_idLinha" runat="server" Value="0" />
                <asp:HiddenField ID="hddPlanoSaude_Carencia" runat="server" Value="0" />
                <asp:HiddenField ID="hddPlanoSaude_sTipo" runat="server" />
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    
    <div class="modal fade" id="modal_Ausencia" tabindex="-1" role="dialog" data-backdrop="static" aria-labelledby="modal_Ausencia_Label" aria-hidden="true">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="modal-dialog modal-lg" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title" id="modal_Ausencia_Label"><b>
                                <asp:Label ID="Ausencia_lblTitulo" runat="server" Text="Ausências/Faltas"></asp:Label></b>
                            </h4>
                        </div>
                        <div class="modal-body">

                            <div class="col-lg-12 row">
                                <uc1:MensagemPagina runat="server" ID="Ausencia_MensagemPagina" />
                            </div>

                            <div class="row" runat="server" id="div6">
                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <label>Tipo</label>
                                        <asp:DropDownList ID="Ausencia_ddlsTipoAusencia" runat="server" class="form-control yes_no select">
                                             <asp:ListItem Value="">Selecione o Tipo</asp:ListItem>
                                            <asp:ListItem Value="Falta Justificada">Falta Justificada</asp:ListItem>
                                            <asp:ListItem Value="Falta Não Justificada">Falta Não Justificada</asp:ListItem>
                                            <asp:ListItem Value="Ausência">Ausência </asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <label>Motivo</label>
                                        <asp:DropDownList ID="Ausencia_ddlsMotivo" runat="server" class="form-control yes_no select">
                                            <asp:ListItem Value="">Selecione o Motivo</asp:ListItem>
                                            <asp:ListItem Value="Afastamento Médico">Afastamento Médico</asp:ListItem>
                                            <asp:ListItem Value="Alistamento (Militar/Eleitoral">Alistamento (Militar/Eleitoral)</asp:ListItem>
                                            <asp:ListItem Value="Atraso">Atraso</asp:ListItem>
                                            <asp:ListItem Value="Audiência / Comparecimento legal">Audiência / Comparecimento legal.</asp:ListItem>
                                            <asp:ListItem Value="Banco de Horas">Banco de Horas</asp:ListItem>
                                            <asp:ListItem Value="Casamento">Casamento</asp:ListItem>
                                            <asp:ListItem Value="Consulta/Exames Médicos">Consulta/Exames Médicos</asp:ListItem>
                                            <asp:ListItem Value="Doação de Sangue">Doação de Sangue</asp:ListItem>
                                            <asp:ListItem Value="Folga compensatória">Folga compensatória</asp:ListItem>
                                            <asp:ListItem Value="Licenças Especiais">Licenças Especiais</asp:ListItem>
                                            <asp:ListItem Value="Nascimento">Nascimento</asp:ListItem>
                                            <asp:ListItem Value="Saída Antecipada">Saída Antecipada</asp:ListItem>
                                            <asp:ListItem Value="Óbito parente">Óbito parente</asp:ListItem>
                                            <asp:ListItem Value="Outros">Outros</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Período</label>
                                        <asp:DropDownList ID="Ausencia_ddlsTipoPeriodo" runat="server" class="form-control yes_no select" AutoPostBack="True" OnSelectedIndexChanged="Ausencia_ddlsPeriodo_SelectedIndexChanged">
                                            <asp:ListItem Value="Dia">Dia</asp:ListItem>
                                            <asp:ListItem Value="Hora">Hora</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <label>Data Início </label>
                                        <asp:TextBox ID="Ausencia_txtdtInicio" class="form-control " runat="server" type="datetime-local" AutoPostBack="True" OnTextChanged="Ausencia_txtdt_TextChanged"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <label>Data Retorno</label>
                                        <asp:TextBox ID="Ausencia_txtdtRetorno" class="form-control " runat="server" type="datetime-local" AutoPostBack="True" OnTextChanged="Ausencia_txtdt_TextChanged"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="Ausencia_div_Horas">
                                    <div class="form-group">
                                        <label>Qtd Horas</label>
                                        <asp:TextBox ID="Ausencia_txtnHoras" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="Ausencia_div_HorasCalculadas">
                                    <div class="form-group">
                                        <label>Horas Calc</label>
                                        <asp:TextBox ID="Ausencia_txtnQuantidadeHoras" class="form-control " runat="server" disabled></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Descrição</label>
                                        <asp:TextBox ID="Ausencia_txtsDscAtestado" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>CID</label>
                                        <asp:TextBox ID="Ausencia_txtsCID" class="form-control" MaxLength="10" runat="server"></asp:TextBox>
                                    </div>
                                </div>


                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Descontar VT?</label>
                                        <div class="tt-switch">
                                            <asp:CheckBox ID="Ausencia_cbDescontoVT" runat="server" Text="Sim" onclick="Mensagens_AtualizarAvisoDashboard();" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Descontar VR?</label>
                                        <div class="tt-switch">
                                            <asp:CheckBox ID="Ausencia_cbDescontoVR" runat="server" Text="Sim" onclick="Mensagens_AtualizarAvisoDashboard();" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Descontar Salário/DSR?</label>
                                        <div class="tt-switch">
                                            <asp:CheckBox ID="Ausencia_cbDescontoDSR" runat="server" Text="Sim" onclick="Mensagens_AtualizarAvisoDashboard();" />
                                        </div>
                                    </div>
                                </div>


                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Observação</label>
                                        <asp:TextBox ID="Ausencia_txtsObservacao" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="120px"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton CssClass="btn btn-success" ID="Ausencia_cmdIncluir" Text="Salvar" runat="server" OnClick="Ausencia_cmdIncluir_Click" />
                            <asp:LinkButton CssClass="btn btn-success" ID="Ausencia_cmdSalvar" Text="Salvar" runat="server" OnClick="Ausencia_cmdSalvar_Click" />
                            <asp:LinkButton CssClass="btn btn-danger" ID="Ausencia_cmdCancelar" Text="Cancelar" runat="server" OnClick="Modal_Fechar" />
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>


    <div class="modal fade" id="modal_NR" tabindex="-1" role="dialog" data-backdrop="static" aria-labelledby="modal_NR_Label" aria-hidden="true">
        <asp:UpdatePanel ID="Upd_Modal_NR" runat="server">
            <ContentTemplate>

                <div class="modal-dialog modal-lg" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title" id="modal_NR_Label"><b>
                                <asp:Label ID="NR_lblTitulo" runat="server" Text="Certificação NR e Outros"></asp:Label></b>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <asp:UpdatePanel runat="server" ID="up_modalNr_mensagem">
                                <ContentTemplate>
                                    <div class="col-lg-12 row">
                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_ModalNR" />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="row">

                                <div class="col-lg-6" runat="server" id="div_ddlTipoNr">
                                    <div class="form-group">
                                        <label>NR </label>
                                        <asp:DropDownList ID="ddlidTipoNr" class="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidTipoNr_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-3" runat="server" id="div_TipoNrOutros" visible="false">
                                    <div class="form-group">
                                        <label>Outros</label>
                                        <asp:TextBox ID="txtTipoNrOutros" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Data Emissão</label>
                                        <asp:TextBox ID="txtdtEmissaoNR" class="form-control CaixaTextoMedia" runat="server" AutoPostBack="true" OnTextChanged="txtdtEmissaoNR_TextChanged"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Data Vencimento</label>
                                        <asp:TextBox ID="txtdtVencimentoNR" class="form-control CaixaTextoMedia" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Selecione o Arquivo</label>
                                        <asp:FileUpload ID="fu_EnviarArquivo_NR" runat="server" />
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>
                                            <asp:Label ID="lblObservacao" runat="server" Text="Observação"></asp:Label></label>
                                        <asp:TextBox ID="txtObservacaoNR" class="form-control" runat="server" MaxLength="300" TextMode="MultiLine" Height="50px"></asp:TextBox>
                                    </div>
                                </div>

                            </div>

                        </div>
                        <asp:UpdatePanel runat="server" ID="up_modalNR_botoes">
                            <ContentTemplate>
                                <div class="modal-footer">
                                    <asp:Button ID="BtnSalvarNR" class="btn btn-success" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="BtnSalvarNR_Click" />
                                    <asp:LinkButton CssClass="btn btn-danger" ID="cmdNR_Cancelar" Text="Cancelar" runat="server" OnClick="cmdNR_Cancelar_Click" />
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:PostBackTrigger ControlID="BtnSalvarNR" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <div class="modal fade" id="UploadArquivos_Modal" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="exampleModalLongTitle">
                        <asp:UpdatePanel runat="server" ID="UpdTitulo_UploadArquivos_Modal" UpdateMode="Conditional">
                            <ContentTemplate>
                                <b>
                                    <asp:Label ID="lblEnviarArquivos_Titulo" runat="server" Text="TITULO"></asp:Label></b>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </h5>
                </div>
                <div class="modal-body">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_EnviarArquivo" />
                    <div class="row">
                        <div class="col-lg-12">
                            <div class="form-stacked">
                                <div class="col-lg-12" id="div1" runat="server">
                                    <div class="form-group">
                                        <label>Selecione o Arquivo</label>
                                        <asp:FileUpload ID="fu_EnviarArquivo" runat="server" />
                                    </div>
                                </div>
                                <asp:UpdatePanel runat="server" ID="UpdObs_UploadArquivos_Modal" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="col-lg-12" runat="server" id="div_observacaoEnvioArquivo">
                                            <div class="form-group">
                                                <label>
                                                    <asp:Label ID="lblEnviarArquivo_sDscArquivo" runat="server" Text="Observação"></asp:Label></label>
                                                <asp:TextBox ID="txtEnviarArquivo_sDscArquivo" class="form-control" runat="server" MaxLength="300"></asp:TextBox>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="cmdEnviarArquivos" CssClass="btn btn-success" runat="server" Text="Enviar" OnClick="cmdEnviarArquivos_Click" />
                    <button type="button" class="btn btn-danger" data-dismiss="modal">Fechar</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modal_PlanoSaude" tabindex="-1" role="dialog" data-backdrop="static" aria-labelledby="modal_PlanoSaude_Label" aria-hidden="true">
        <asp:UpdatePanel ID="up_PlanoSaude" runat="server">
            <ContentTemplate>

                <div class="modal-dialog modal-lg" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title" id="modal_PlanoSaude_Label"><b>
                                <asp:Label ID="lblTituloPlanoSaude" runat="server" Text="Plano de Saúde"></asp:Label></b>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <asp:UpdatePanel runat="server" ID="UpdatePanel2">
                                <ContentTemplate>
                                    <div class="col-lg-12 row">
                                        <uc1:MensagemPagina runat="server" ID="MensagemPaginaPlanoSaude" />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="row">

                                <div runat="server" id="div_dadosPlanoSaude">

                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>Convênio / Plano</label>
                                            <asp:DropDownList ID="ddlidPlanoSaude" runat="server" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlidPlanoSaude_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Beneficiário</label>
                                            <asp:DropDownList ID="ddlsBeneficiario" runat="server" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlsBeneficiario_SelectedIndexChanged">
                                                <asp:ListItem Text="Colaborador" Value="0" Selected="true"></asp:ListItem>
                                                <asp:ListItem Text="Dependente" Value="1"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-4" runat="server" id="div_ddlDependente">
                                        <div class="form-group">
                                            <label>Dependente</label>
                                            <asp:DropDownList ID="ddlidDependente" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Data Inclusão</label>
                                            <asp:TextBox ID="txtdtInclusaoConvenio" class="form-control CaixaTextoMedia" runat="server" AutoPostBack="true" OnTextChanged="txtdtInclusaoConvenio_TextChanged"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Fim da Carência</label>
                                            <asp:TextBox ID="txtdtCarenciaConvenio" class="form-control CaixaTextoMedia" runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Valor Plano</label>
                                            <asp:TextBox ID="txtnValorPlano" class="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                </div>

                            </div>

                        </div>
                        <asp:UpdatePanel runat="server" ID="UpdatePanel3">
                            <ContentTemplate>
                                <div class="modal-footer">
                                    <asp:Button ID="btnSalvarPlanoSaude" class="btn btn-success" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="btnSalvarPlanoSaude_Click" />
                                    <asp:LinkButton CssClass="btn btn-danger" ID="lbCancelaPlanoSaude" Text="Cancelar" runat="server" OnClick="lbCancelaPlanoSaude_Click" />
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:PostBackTrigger ControlID="btnSalvarPlanoSaude" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <script>
        var conversasContainer = document.getElementById("conversasContainer");
        if (conversasContainer) conversasContainer.scrollTop = conversasContainer.scrollHeight;
    </script>

</asp:Content>
