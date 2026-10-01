<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Funcoes_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.Funcoes_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/ExcelImportar.ascx" TagPrefix="uc1" TagName="ExcelImportar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%-- Estilos para o Switch foram movidos para cá para ficarem no head da página --%>
    <style>
        /*      .switch-container .form-group {
            display: flex;
            align-items: center;
            margin-bottom: 0; 
        }

        .switch-container label {
            margin-right: 10px;
            margin-bottom: 0;
            white-space: nowrap;
        }*/

        .switch {
            position: relative;
            display: inline-block;
            width: 60px;
            height: 25px;
        }

            .switch input {
                opacity: 0;
                width: 0;
                height: 0;
            }

        .slider {
            position: absolute;
            cursor: pointer;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: red; /* Cor para 'Não' */
            transition: .4s;
            border-radius: 34px;
        }

            .slider:before {
                content: "Não";
                position: absolute;
                left: 70%;
                top: 50%;
                transform: translate(-50%, -50%);
                color: white; /* Cor do texto para 'Não' */
                font-size: 1.4rem;
                font-weight: bold;
                transition: .4s;
            }

        input:checked + .slider {
            background-color: green; /* Cor para 'Sim' */
        }

            input:checked + .slider:before {
                content: "Sim";
                color: white; /* Cor do texto para 'Sim' */
                left: 30%;
            }

        .slider:after {
            content: "";
            position: absolute;
            height: 20px;
            width: 20px;
            left: 3px;
            bottom: 2.25px;
            background-color: white;
            transition: .4s;
            border-radius: 15px;
        }

        input:checked + .slider:after {
            transform: translateX(34px);
        }

        .switch-bloquear {
            pointer-events: none;
        }
    </style>
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <style>
        .id {
            display: none;
        }

        #lock {
            display: none;
        }

        .lock-label {
            position: relative;
            width: 45px;
            height: 45px;
            display: flex;
            align-items: center;
            justify-content: center;
            background-color: transparent;
            border-radius: 15px;
            cursor: pointer;
            transition: all 0.3s;
        }

            .lock-label:hover::after {
                content: "Clique para habilitar a edição";
                position: absolute;
                bottom: 100%;
                left: 10%;
                transform: translateX(-50%);
                background-color: #333;
                color: #fff;
                padding: 5px 8px;
                border-radius: 4px;
                white-space: nowrap;
                font-size: 12px;
                pointer-events: none;
                opacity: 0;
                transition: opacity 0.2s ease-in-out;
                z-index: 1;
            }

            .lock-label:hover::after {
                opacity: 1;
            }

        #lock:checked + .lock-label:hover::after {
            content: "Clique para bloquear a edição";
        }

        .lock-wrapper {
            width: fit-content;
            height: fit-content;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
        }

        .shackle {
            background-color: transparent;
            height: 9px;
            width: 14px;
            border-top-right-radius: 10px;
            border-top-left-radius: 10px;
            border-top: 3px solid black;
            border-left: 3px solid black;
            border-right: 3px solid black;
            transform: rotateY(150deg) translateX(3px);
            transform-origin: right;
            transition: all 0.3s;
        }

        .lock-body {
            width: 15px;
        }

        .shackle-closed {
            transform: rotateY(0deg);
        }

        .shackle-open {
            transform: rotateY(150deg) translateX(3px);
            transform-origin: right;
        }

        .lock-label-closed {
            pointer-events: none;
        }

        .lock-label:active {
            transform: scale(0.9);
        }
    </style>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <div id="hdd">
                <asp:HiddenField ID="hddidFuncao" runat="server" />
                <asp:HiddenField ID="hddEPIs" runat="server" Value="[]" />
                <asp:HiddenField ID="hddIncluir_idEPI" runat="server" Value="0" />

                <asp:HiddenField ID="hddsCadeado" runat="server" Value="N" />
                <asp:HiddenField ID="hddsPermissaoCadeado" runat="server" Value="0" />
            </div>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>
            </div>

            <div id="abas">
                <ul id="tab_Funcao" class="nav nav-tabs" role="tablist">
                    <li role="presentation" class="tabpanel active">
                        <a href="#funcao" id="aba_Funcao" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Funções</b></a>
                    </li>
                    <li role="presentation" runat="server" id="aba_Descritivo">
                        <a href="#descritivo" role="tab" id="descritivo-tab" data-toggle="tab" aria-controls="Descritivo"><b>Descritivo</b></a>
                    </li>
                    <li role="presentation" runat="server" id="aba_EPI">
                        <a href="#epi" role="tab" id="epi-tab" data-toggle="tab" aria-controls="EPIUtilizada"><b>EPIs</b></a>
                    </li>
                    <li role="presentation" runat="server" id="aba_Historico">
                        <a href="#historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="Historico"><b>Histórico</b></a>
                    </li>
                </ul>
            </div>

            <div id="tab" class="tab-content">

                <div role="tabpanel" class="tab-pane fade in active" id="funcao" aria-labelledby="funcao-tab">
                    <br />
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                    <div class="panel panel-default">

                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Detalhe</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">

                                <div class="row col-lg-12">
                                    <div id="id" class="col-lg-10">
                                        <div class="form-group">
                                            <label>ID </label>
                                            <asp:TextBox ID="txtidFuncao" class="form-control CaixaTextoMini" runat="server" disabled="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div runat="server" id="div_bloquearEdicao" class="col-lg-2" style="display: flex; justify-content: right; padding-right: 0;">
                                        <div class="form-group">
                                            <input type="checkbox" id="lock" <%= hddsCadeado.Value == "N" ? "checked='checked'" : "" %> />
                                            <label for="lock" class="<%= hddsPermissaoCadeado.Value == "0" ? "lock-label lock-label-closed" : "lock-label" %>">
                                                <span class="lock-wrapper">
                                                    <span class="<%= hddsCadeado.Value == "N" ? "shackle shackle-open" : "shackle shackle-closed" %>"></span>
                                                    <svg class="lock-body" width="15" height="15" viewBox="0 0 28 28" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                        <path fill-rule="evenodd" clip-rule="evenodd" d="M0 5C0 2.23858 2.23858 0 5 0H23C25.7614 0 28 2.23858 28 5V23C28 25.7614 25.7614 28 23 28H5C2.23858 28 0 25.7614 0 23V5ZM16 13.2361C16.6137 12.6868 17 11.8885 17 11C17 9.34315 15.6569 8 14 8C12.3431 8 11 9.34315 11 11C11 11.8885 11.3863 12.6868 12 13.2361V18C12 19.1046 12.8954 20 14 20C15.1046 20 16 19.1046 16 18V13.2361Z" fill="black"></path>
                                                    </svg>
                                                </span>
                                            </label>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <label>Empresa</label>
                                        <asp:DropDownList runat="server" ID="ddlidEmpresa" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlidEmpresa_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Tipo Contrato</label>
                                        <asp:DropDownList runat="server" ID="ddlidTipoContrato" class="form-control Caixa_Selecao"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <label>Departamento</label>
                                        <asp:DropDownList runat="server" ID="ddlidDepartamento" class="form-control Caixa_Selecao"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Tipo de Função</label>
                                        <asp:DropDownList runat="server" ID="ddlTipoFuncao" class="form-control Caixa_Selecao">
                                            <asp:ListItem Value="0">Selecione o Tipo</asp:ListItem>
                                            <asp:ListItem Value="1">Administrativo</asp:ListItem>
                                            <asp:ListItem Value="2">Operacional</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Descrição da Função</label>
                                        <asp:TextBox ID="txtsDscFuncao" class="form-control" runat="server" MaxLength="200"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>CBO</label>
                                        <asp:TextBox ID="sCBO" class="form-control " runat="server" MaxLength="10"></asp:TextBox>
                                    </div>
                                </div>
                                <div runat="server" id="DIV_Avaliacao">
                                    <div class="form-group">
                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>Tipo de Cargo (Avaliação)</label>
                                                <asp:DropDownList ID="ddlidTipoAvaliacao_Funcao" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-12 form-group row">
                                    <div class="col-lg-6">
                                        <div class="">
                                            <label>GHE</label>
                                            <asp:DropDownList ID="ddlidGHE" runat="server" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlidGHE_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="col-lg-12">
                                            <div class="form-group">
                                                <label>Setor</label>
                                                <asp:DropDownList ID="ddlidSetor" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>





                                <div class="col-lg-12 row">
                                    <!-- Bloco de Remuneração e Adicionais -->
                                    <div class="col-lg-6">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                <h3 class="panel-title" style="font-size: 14px;"><b>Remuneração e Adicionais</b></h3>
                                            </div>
                                            <div class="panel-body">
                                                <div class="row">
                                                    <div class="col-lg-5" runat="server" id="DIV_Remuneracao">
                                                        <div class="form-group">
                                                            <label>Remuneração Base</label>
                                                            <asp:TextBox ID="txtnRemuneracaoBase" class="form-control" runat="server" MaxLength="12"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-3" runat="server" id="DIV_TipoRemuneracao">
                                                        <div class="form-group">
                                                            <label>Por</label>
                                                            <asp:DropDownList runat="server" ID="ddlsTipoRemuneracao" class="form-control Caixa_Selecao">
                                                                <asp:ListItem>Mês</asp:ListItem>
                                                                <asp:ListItem Value="Diario">Dia</asp:ListItem>
                                                                <asp:ListItem>Quinzena</asp:ListItem>
                                                                <asp:ListItem>Hora</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-4" runat="server" id="DIV_Comissão">
                                                        <div class="form-group">
                                                            <label>% Comissão</label>
                                                            <asp:TextBox ID="txtnPercComissaoVendas" class="form-control" runat="server" MaxLength="12"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-lg-2">
                                                        <div class="form-group">
                                                            <label>Dupla Função</label>
                                                            <div>
                                                                <label class="switch">
                                                                    <asp:CheckBox ID="chkDuplaFuncao" runat="server" onclick="toggleDuplaFuncao(this);" />
                                                                    <span class="slider"></span>
                                                                </label>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-4" id="div_DuplaFuncao" runat="server" style="display: none;">
                                                        <div class="form-group">
                                                            <label>% Valor</label>
                                                            <asp:TextBox ID="txtnValorDuplaFuncao" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2">
                                                        <div class="form-group">
                                                            <label>Periculosidade</label>
                                                            <div>
                                                                <label class="switch">
                                                                    <asp:CheckBox ID="chkPericulosidade" runat="server" onclick="togglePericulosidade(this);" />
                                                                    <span class="slider"></span>
                                                                </label>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-4" id="div_Periculosidade" runat="server" style="display: none;">
                                                        <div class="form-group">
                                                            <label>% Periculosidade</label>
                                                            <asp:TextBox ID="txtnValorPericulosidade" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>



                                                </div>

                                                <div class="row">

                                                    <div class="col-lg-2">
                                                        <div class="form-group">
                                                            <label>Noturnidade</label>
                                                            <div>
                                                                <label class="switch">
                                                                    <asp:CheckBox ID="chkNoturnidade" runat="server" onclick="toggleNoturnidade(this);" />
                                                                    <span class="slider"></span>
                                                                </label>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-4" id="div_Noturnidade" runat="server" style="display: none;">
                                                        <div class="form-group">
                                                            <label>% Noturnidade</label>
                                                            <asp:TextBox ID="txtnValorNoturnidade" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                </div>

                                            </div>

                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-6">
                                    <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
                                </div>

                            </div>
                        </div>
                    </div>
                </div>



                <div role="tabpanel" class="tab-pane fade" id="descritivo" aria-labelledby="descritivo-tab">
                    <br />
                    <div class="panel panel-default" runat="server" id="Div_descritivo">
                        <div class="panel-heading">

                            <h3 class="panel-title"><b>Descritivo</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">

                                <div class="col-lg-12">
                                    <div class="form-group row">
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <label>Tarefas </label>
                                                <asp:TextBox ID="txtsTarefas" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="200px"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <label>Competências </label>
                                                <asp:TextBox ID="txtsCompetencias" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="200px"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group row">
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <label>Formação acadêmica </label>
                                                <asp:TextBox ID="txtsFormacaoAcademica" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="200px"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <label>Experiencia profissional </label>
                                                <asp:TextBox ID="txtsExperienciaProfissional" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="200px"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>

                <div role="tabpanel" class="tab-pane fade" id="epi" aria-labelledby="epi-tab">
                    <br />
                    <div class="panel panel-default" runat="server" id="Div_EPIs">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>EPI Utilizada</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group row">

                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_EPI" />
                                </div>

                                <div runat="server" id="div_EPI">
                                    <div class="col-lg-12">
                                        <div class="form-stacled row">

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Código</label>
                                                    <asp:TextBox ID="txtCodigo_EPI" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-4">
                                                <div class="form-group">
                                                    <label>Descrição</label>
                                                    <asp:TextBox ID="txtDesc_EPI" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-1">
                                                <div class="form-group">
                                                    <label>Qtd do EPI</label>
                                                    <asp:TextBox ID="txtIncluirEPI_Qtd" class="form-control qtd" runat="server" MaxLength="3" Text="1"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2" style="padding: 0;">
                                                <label>Periodicidade</label>
                                                <div class="input-group" style="width: 100%;">
                                                    <asp:TextBox ID="txtIncluirEPI_nTempo" class="form-control qtd" runat="server" MaxLength="3" Text="1" aria-label="..." Style="width: 40%;"></asp:TextBox>

                                                    <asp:DropDownList ID="ddlIncluirEPI_sTipoPeriodo" runat="server" class="form-control input-group-addon" aria-label="..." Style="width: 60%; z-index: 1; background-color: white; text-align: start;">
                                                        <asp:ListItem Value="H">Hora</asp:ListItem>
                                                        <asp:ListItem Value="D">Dia</asp:ListItem>
                                                        <asp:ListItem Value="S">Semana</asp:ListItem>
                                                        <asp:ListItem Value="M">Mês</asp:ListItem>
                                                        <asp:ListItem Value="A">Ano</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-1">
                                                <div class="form-group">
                                                    <label>&nbsp;</label>
                                                    <asp:Button ID="cmdEPI_Incluir" class="form-control btn-info" runat="server" Text="Incluir" OnClick="cmdEPI_Incluir_Click" />
                                                </div>
                                            </div>

                                            <div class="col-lg-1" style="padding-left: 0;">
                                                <div class="form-group">
                                                    <label>&nbsp;</label>
                                                    <asp:Button ID="cmdEPI_Importar" class="form-control btn-primary" runat="server" Text="Importar EPIs" OnClick="cmdEPI_Importar_Click" />
                                                </div>
                                            </div>

                                            <div class="col-lg-1" style="padding-left: 0;">
                                                <div class="form-group">
                                                    <label>&nbsp;</label>
                                                    <asp:Button ID="cmdEPI_Exportar" class="form-control btn-danger" runat="server" Text="Exportar EPIs" OnClick="cmdEPI_Exportar_Click" />
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <asp:GridView ID="dtgEPI" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small"
                                        OnRowDataBound="dtgEPI_RowDataBound" OnRowDeleting="dtgEPI_RowDeleting">
                                        <Columns>

                                            <asp:BoundField DataField="idlinha" HeaderText="idLinha">
                                                <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="id" />
                                                <HeaderStyle CssClass="id" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Ordem">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtnOrdem" class="form-control qtd" MaxLength="3" runat="server" Text='<%# Bind("nOrdem") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Código">
                                                <ItemTemplate>
                                                    <%# MostrarLink(Eval("idItem"), Eval("sCodigoEPI"))%>
                                                </ItemTemplate>
                                                <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Descrição">
                                                <ItemTemplate>
                                                    <%# MostrarLink(Eval("idItem"), Eval("sDscEPI"))%>
                                                </ItemTemplate>
                                                <ItemStyle Width="50%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Quantidade">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtnQuantidateEPI" class="form-control qtd" MaxLength="3" runat="server" Text='<%# Bind("nQuantidateEPI") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Periodicidade">
                                                <ItemTemplate>
                                                    <div style="width: 100%; display: flex;">

                                                        <div style="width: 45%;">
                                                            <asp:TextBox ID="txtnQuantidadeTempo" class="form-control qtd" Style="width: 100%" MaxLength="3" runat="server" Text='<%# Bind("nQuantidadeTempo") %>'></asp:TextBox>
                                                        </div>

                                                        <div style="width: 55%; margin-left: 5px;">
                                                            <asp:DropDownList ID="ddlsTipoPeriodo" runat="server" class="form-control" Style="width: 100%">
                                                                <asp:ListItem Value="H">Hora</asp:ListItem>
                                                                <asp:ListItem Value="D">Dia</asp:ListItem>
                                                                <asp:ListItem Value="S">Semana</asp:ListItem>
                                                                <asp:ListItem Value="M">Mês</asp:ListItem>
                                                                <asp:ListItem Value="A">Ano</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>

                                                    </div>
                                                </ItemTemplate>
                                                <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Excluir">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkEPI_Excluir" CommandArgument='<%# Bind("idLinha") %>' runat="server" CssClass="btn btn-small" TabIndex="100" data-toggle="tooltip" title="Excluir" CommandName="Delete"><i class="fa fa-eraser"></i></asp:LinkButton>
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

                <div role="tabpanel" class="tab-pane fade" id="historico" aria-labelledby="historico-tab">
                    <br />
                    <div class="panel panel-default" runat="server" id="DIV_historico">
                        <div class="panel-heading">

                            <h3 class="panel-title"><b>Histórico</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12 table-responsive">

                                    <asp:GridView ID="gv_Historico" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" OnRowDataBound="gv_Historico_RowDataBound">
                                        <Columns>

                                            <asp:BoundField DataField="idLog" HeaderText="ID" Visible="False">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtAcao" HeaderText="Data">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sAcao" HeaderText="Ação">
                                                <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sObservacao" HeaderText="Detalhe">
                                                <ItemStyle Width="55%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                        </Columns>
                                    </asp:GridView>

                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>

            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                &nbsp;
                <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" id="field-cancel" title="Voltar" onclick="history.go(-1)">&nbsp;
            </fieldset>

            <div class="modal fade" id="modalImportar_EPI" data-backdrop="static">
                <div class="modal-dialog" style="width: 45%;">
                    <div class="modal-content" style="overflow: visible;">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h4 class="modal-title">Importar EPIs</h4>
                        </div>
                        <div class="modal-body">
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="col-lg-12">

                                        <div class="row">
                                            <div class="col-lg-12">
                                                <uc1:MensagemPagina runat="server" ID="MensagemPagina_Modal_ImportarEPI" />
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="form-group">
                                                <label>Função</label>
                                                <asp:DropDownList runat="server" ID="ddlFuncao_ImportarEPI" class="form-control Caixa_Selecao"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="modal-footer" style="padding: 15px 0;">
                                                <asp:Button ID="cmdFuncao_ImportarEPI" class="btn btn-success" runat="server" Text="Importar EPIs" OnClick="cmdFuncao_ImportarEPI_Click" />
                                                <button type="button" class="btn btn-danger" data-dismiss="modal">Fechar</button>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="dialog_Aceitar" class="modal" title="Status Edição">
                <p>
                    <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                    <asp:Label ID="Label3" runat="server" ClientIDMode="Static" Text=""></asp:Label>
                </p>
            </div>

            <!-- ** INÍCIO DO NOVO SCRIPT PARA OS SWITCHES ** -->
            <script type="text/javascript">
                // 'pageLoad' é uma função especial do ASP.NET AJAX que executa sempre
                // que a página carrega, incluindo após um postback parcial do UpdatePanel.
                function pageLoad(sender, args) {
                    // Após cada postback parcial, garantimos que o estado visual dos divs
                    // corresponda ao estado atual dos checkboxes.
                    toggleDuplaFuncao(document.getElementById('<%= chkDuplaFuncao.ClientID %>'));
                    togglePericulosidade(document.getElementById('<%= chkPericulosidade.ClientID %>'));
                    toggleNoturnidade(document.getElementById('<%= chkNoturnidade.ClientID %>'));
                }

                // Função chamada pelo evento onclick do checkbox 'Dupla Função' e pelo pageLoad.
                function toggleDuplaFuncao(checkbox) {
                    // Verifica se o checkbox foi encontrado antes de tentar usá-lo.
                    if (checkbox) {
                        toggleDivVisibilidade(checkbox.checked, '<%= div_DuplaFuncao.ClientID %>');
                    }
                }

                // Função chamada pelo evento onclick do checkbox 'Periculosidade' e pelo pageLoad.
                function togglePericulosidade(checkbox) {
                    if (checkbox) {
                        toggleDivVisibilidade(checkbox.checked, '<%= div_Periculosidade.ClientID %>');
                    }
                }

                function toggleNoturnidade(checkbox) {
                    if (checkbox) {
                        toggleDivVisibilidade(checkbox.checked, '<%= div_Noturnidade.ClientID %>');
                    }
                }

                // Função central que efetivamente mostra ou esconde o DIV.
                function toggleDivVisibilidade(isVisivel, divId) {
                    const divElemento = document.getElementById(divId);
                    if (divElemento) {
                        // Define o estilo 'display' como 'block' (visível) ou 'none' (oculto).
                        divElemento.style.display = isVisivel ? 'block' : 'none';
                    }
                }
            </script>
            <!-- ** FIM DO NOVO SCRIPT ** -->

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

