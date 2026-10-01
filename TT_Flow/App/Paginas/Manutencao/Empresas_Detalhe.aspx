<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Empresas_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.Empresas.Empresas_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/DropDownList_Padrao.ascx" TagPrefix="uc1" TagName="DropDownList_Padrao" %>
<%@ Register Src="~/App/Controles/TextBox_Padrao.ascx" TagPrefix="uc1" TagName="TextBox_Padrao" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <asp:UpdatePanel ID="updDetalhe" runat="server">
        <ContentTemplate>

            <uc1:Manual runat="server" ID="manual" />

            <style>
                .upload-container {
                    width: fit-content;
                    height: fit-content;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    margin: 0 auto;
                }

                .file-upload-label input {
                    display: none;
                }

                .file-upload-label svg {
                    height: 50px;
                    fill: rgb(0, 125, 0);
                    margin-bottom: 20px;
                }

                .file-upload-label {
                    cursor: pointer;
                    background-color: #dff0d8;
                    padding: 30px 70px;
                    border-radius: 40px;
                    border: 2px dashed rgb(0, 125, 0);
                    box-shadow: 0px 0px 20px -50px rgba(0, 0, 0, 0.719);
                    display: flex;
                    flex-direction: column;
                    align-items: center;
                    justify-content: center;
                    gap: 5px;
                    text-align: center;
                    color: rgb(0, 125, 0);
                }

                    .file-upload-label.dragover {
                        border-color: #000;
                        background-color: #e6f7ff;
                    }

                .browse-button {
                    background-color: rgb(0, 125, 0);
                    padding: 5px 15px;
                    border-radius: 10px;
                    color: white;
                    transition: all 0.3s;
                }

                    .browse-button:hover {
                        background-color: rgb(0, 75, 0);
                    }

                .file-name {
                    margin-top: 10px;
                    font-size: 14px;
                    color: rgb(0, 75, 0);
                }

                #modalView_ImagemFornecedores img {
                    max-width: 100%;
                    height: auto;
                    display: block;
                    margin: 0 auto;
                }
            </style>

            <div id="hdd">
                <asp:HiddenField ID="hddidEmpresa" runat="server" />
            </div>

            <div class="form-stacked">

                <div class="col-lg-12 row">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Empresas"></asp:Label>
                        <small>Detalhe</small>
                    </h1>
                    <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>

                <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                <div id="abas">
                    <ul id="tab_Empresa" class="nav nav-tabs" role="tablist">
                        <li role="presentation" class="tabpanel active">
                            <a href="#Empresas" id="aba-Empresas" role="tabpanel" data-toggle="tab" aria-controls="Empresas" aria-expanded="false"><b>Empresa</b></a>
                        </li>
                        <li role="presentation" class="tabpanel">
                            <a href="#Parametros" id="aba-Parametros" role="tabpanel" data-toggle="tab" aria-controls="Parametros" aria-expanded="false"><b>Parâmetros</b></a>
                        </li>
                        <li role="presentation" id="aba_Arquivos" runat="server">
                            <a href="#arquivos" role="tab" id="arquivos-tab" data-toggle="tab" aria-controls="Arquivos"><b>Documentos</b></a>
                        </li>
                    </ul>
                </div>

                <br />

                <div id="tab" class="tab-content">

                    <div role="tabpanel" class="tab-pane fade in active" id="Empresas" aria-labelledby="Empresas">

                        <div class="panel panel-default" runat="server" id="div_Empresa">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Dados da Empresa</b></h3>
                            </div>
                            <div class="panel-body">

                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_empresa" />
                                </div>

                                <div class="row">
                                    <div class="col-lg-1 col-lg-1-5 form-group">
                                        <label>ID</label>
                                        <div class="input-group">
                                            <asp:LinkButton ID="cmRetornar" class="btn btn-sm btn-info input-group-addon" runat="server" OnClick="cmdRetornar_click"><b><</b></asp:LinkButton>
                                            <asp:TextBox ID="txtidRegistro" class="form-control" runat="server" disabled="" Style="text-align: center;"></asp:TextBox>
                                            <asp:LinkButton ID="cmAvancar" class="btn btn-sm btn-info input-group-addon" runat="server" OnClick="cmdAvancar_click"><b>></b></asp:LinkButton>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">

                                    <div class="col-lg-4 form-group">
                                        <label>Razão Social</label>
                                        <asp:TextBox ID="txtsDscEmpresa" class="form-control" runat="server" MaxLength="50"></asp:TextBox>
                                    </div>

                                    <div class="col-lg-2 form-group">
                                        <label>Código Empresa</label>
                                        <asp:TextBox ID="txtsDscEmpresaReduzida" class="form-control" runat="server" MaxLength="10"></asp:TextBox>
                                    </div>

                                    <div class="col-lg-2 form-group">
                                        <label>País de Operação</label>
                                        <asp:DropDownList ID="ddlidTipo" runat="server" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlidTipo_SelectedIndexChanged">
                                            <asp:ListItem Value="0" Text="Selecione o País" />
                                            <asp:ListItem Value="1" Text="BRASIL" />
                                            <asp:ListItem Value="6" Text="USA" />
                                        </asp:DropDownList>
                                    </div>

                                    <div class="col-lg-2 form-group" runat="server" id="div_txtEstado">
                                        <label>Estado de Domicílio</label>
                                        <asp:TextBox ID="txtsEstado" class="form-control" runat="server" MaxLength="5"></asp:TextBox>
                                    </div>

                                    <div runat="server" id="div_UF_Mun_Nacional">

                                        <div class="col-lg-1 form-group" runat="server" id="div_ddlEstado">
                                            <label>UF de Domicílio</label>
                                            <asp:DropDownList ID="ddlEstado" runat="server" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlEstado_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                        <div class="col-lg-3 form-group" runat="server" id="div_ddlMunicipio">
                                            <label>Município de Domicílio</label>
                                            <asp:DropDownList ID="ddlMunicipio" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                        </div>

                                    </div>

                                </div>

                                <div class="row">

                                    <div class="col-lg-2 form-group" runat="server" id="div_CNPJ">
                                        <asp:Label ID="lblCNPJ_VAT" runat="server" Font-Bold="true" Text="Label"></asp:Label>
                                        <asp:TextBox ID="txtCNPJ" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                    </div>

                                    <div class="col-lg-4 form-group">
                                        <label>Parceiro</label>
                                        <uc1:DropDownList_Padrao runat="server" ID="ddlidCliente" Link="/App/Paginas/Manutencao/Parceiros_Detalhe.aspx?id=" AutoPostBack="true" OnSelectedIndexChanged="ddlidCliente_SelectedIndexChanged" />
                                    </div>

                                    <div class="col-lg-2 form-group">
                                        <label>Tipo de Tributação</label>
                                        <asp:DropDownList ID="ddlidTributacao" runat="server" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlidTributacao_SelectedIndexChanged">
                                            <asp:ListItem Value="0" Text="Selecione o Tipo de Tributação" />
                                            <asp:ListItem Value="1" Text="Lucro Real" />
                                            <asp:ListItem Value="2" Text="Lucro Presumido" />
                                            <asp:ListItem Value="3" Text="Simples" />
                                            <asp:ListItem Value="4" Text="Tax Return Florida" />
                                        </asp:DropDownList>
                                    </div>

                                    <div class="col-lg-2 form-group" runat="server" id="div_TaxReturn">
                                        <label>Código Tax Return</label>
                                        <asp:TextBox ID="txtsCodigoTaxReturn" class="form-control" runat="server"></asp:TextBox>
                                    </div>

                                    <div class="col-lg-2 form-group">
                                        <label>Código de Regime Tributário</label>
                                        <asp:DropDownList ID="ddlidCodigoTributario" runat="server" class="form-control Caixa_Selecao">
                                            <asp:ListItem Value="0" Text="Selecione o Código de Regime Tributário" />
                                            <asp:ListItem Value="1" Text="1 - Simples Nacional" />
                                            <asp:ListItem Value="2" Text="2 - Simples Nacional - Excesso de sublimite da receita bruta." />
                                            <asp:ListItem Value="3" Text="3 - Regime Normal" />
                                        </asp:DropDownList>
                                    </div>

                                    <div class="col-lg-2">
                                        <uc1:ComboAtivo runat="server" ID="ddlsEfetuaCompras" />
                                    </div>

                                </div>

                                <div class="row">

                                    <div class="col-lg-6 form-group">
                                        <label>Tipos de Faturamento</label>
                                        <asp:ListBox ID="ddlidTipoFaturamento" runat="server" class="form-control Caixa_Selecao" SelectionMode="Multiple"></asp:ListBox>
                                    </div>

                                    <div class="col-lg-2 form-group">
                                        <uc1:TextBox_Padrao runat="server" ID="txtSenha_Certificado" Titulo="Senha - Certificado Digital" Modo="Senha" />
                                    </div>

                                </div>

                                <div class="row">
                                    <div id="pnNFe" class="col-lg-6">
                                        <div class="panel panel-default m-0">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>Nota Fiscal Vendas</b></h3>
                                            </div>
                                            <div class="panel-body">

                                                <div class="row">
                                                    <div class="col-lg-6">
                                                        <uc1:TextBox_Padrao runat="server" ID="txtsSerieNFE" Titulo="Série NF-e" Modo="Inteiro" />
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <uc1:TextBox_Padrao runat="server" ID="txtsNumeroNFE" Titulo="Número NF-e" Modo="Inteiro" />
                                                    </div>
                                                </div>

                                            </div>
                                        </div>
                                    </div>

                                    <div id="pnNFS" class="col-lg-6">
                                        <div class="panel panel-default m-0">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>Nota Fiscal Serviços</b></h3>
                                            </div>
                                            <div class="panel-body">

                                                <div class="row">
                                                    <div class="col-lg-4">
                                                        <uc1:TextBox_Padrao runat="server" ID="txtsLoteRPS" Titulo="Lote RPS" Modo="Inteiro" ReadOnly="true" />
                                                    </div>

                                                    <div class="col-lg-4">
                                                        <uc1:TextBox_Padrao runat="server" ID="txtsSerieRPS" Titulo="Série RPS" Modo="Inteiro" />
                                                    </div>

                                                    <div class="col-lg-4">
                                                        <uc1:TextBox_Padrao runat="server" ID="txtsNumeroRPS" Titulo="Número RPS" Modo="Inteiro" />
                                                    </div>
                                                </div>

                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>

                        <div class="panel panel-default" runat="server" id="Div_Impostos">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Impostos</b></h3>
                            </div>
                            <div class="col-lg-12">
                                <uc1:MensagemPagina runat="server" ID="MensagemPagina4" />
                            </div>
                            <div class="panel-body">
                                <div class="form-stacked row">
                                    <div class="col-lg-12 table-responsive">
                                        <div class="row" runat="server" id="Div_Imposto">
                                            <div class="col-lg-12">
                                                <div class="row">
                                                    <div class="col-lg-2">
                                                        <div class="form-group">
                                                            <label>Tipo de Impostos</label>
                                                            <asp:DropDownList ID="ddlsImpostos" runat="server" class="form-control" AutoPostBack="true">
                                                                <asp:ListItem Value="0" Text="Selecione o Tipo de Impostos" />
                                                                <asp:ListItem Value="1" Text="Serviços" />
                                                                <asp:ListItem Value="2" Text="Material" />
                                                                <asp:ListItem Value="3" Text="Apuração" />
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-2" runat="server" id="Div_PIS">
                                                        <label>PIS</label>
                                                        <div class="input-group">
                                                            <asp:TextBox ID="txtnPis" class="form-control " runat="server" aria-describedby="span1" Style="z-index: 0;"></asp:TextBox>
                                                            <span class="input-group-addon" id="span1" runat="server">%</span>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-2" runat="server" id="Div_COFINS">
                                                        <label>COFINS</label>
                                                        <div class="input-group">
                                                            <asp:TextBox ID="txtnCofins" class="form-control " runat="server" aria-describedby="span2" Style="z-index: 0;"></asp:TextBox>
                                                            <span class="input-group-addon" id="span2" runat="server">%</span>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2" runat="server" id="Div_ICMS">
                                                        <label>ICMS</label>
                                                        <div class="input-group">
                                                            <asp:TextBox ID="txtnICMS" class="form-control" runat="server" aria-describedby="span3" Style="z-index: 0;"></asp:TextBox>
                                                            <span class="input-group-addon" id="span3" runat="server">%</span>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2" runat="server" id="Div_CSSL">
                                                        <label>CSSL</label>
                                                        <div class="input-group">
                                                            <asp:TextBox ID="txtnCSSL" class="form-control" runat="server" aria-describedby="span4" Style="z-index: 0;"></asp:TextBox>
                                                            <span class="input-group-addon" id="span4" runat="server">%</span>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2" runat="server" id="Div_IRPJ">
                                                        <label>IRPJ</label>
                                                        <div class="input-group">
                                                            <asp:TextBox ID="txtnIRPJ" class="form-control" runat="server" aria-describedby="span5" Style="z-index: 0;"></asp:TextBox>
                                                            <span class="input-group-addon" id="span5" runat="server">%</span>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2" runat="server" id="Div_Ano">
                                                        <div class="form-group">
                                                            <label>Ano</label>
                                                            <asp:TextBox ID="txtnAno" class="form-control" type="number" runat="server" min="2024" max="2099" MaxLength="4"></asp:TextBox>
                                                        </div>
                                                    </div>


                                                    <div class="col-lg-1 form-group">
                                                        <label>&nbsp;</label>
                                                        <asp:Button ID="cmdImposto" runat="server" CssClass="btn btn-info form-control" OnClick="cmdImposto_Click" Text="Adicionar" />
                                                    </div>
                                                    <asp:HiddenField ID="hddGarantia_idLinha" runat="server" />
                                                    <asp:HiddenField ID="hddMoeda" runat="server" />
                                                </div>
                                            </div>
                                        </div>

                                        <div class="table-responsive col-lg-12" runat="server" id="DIV10">
                                            <div class="row">
                                                <asp:GridView ID="gv_Imposto" class="table table-striped table-bordered table-hover table-condensed"
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" DataKeyNames="idLinha" OnRowDeleting="gv_Imposto_RowDeleting">
                                                    <Columns>

                                                        <asp:BoundField DataField="idlinha" HeaderText="Ordem">
                                                            <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sImpostos" HeaderText="Tipo de Imposto">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:TemplateField HeaderText="PIS">
                                                            <ItemTemplate>
                                                                <div style="display: flex;">
                                                                    <asp:Label ID="nPis" runat="server" ClientIDMode="Static" MaxLength="20" Text='<%# Bind("nPis", "{0:N2}") %>'></asp:Label>
                                                                    <span style="margin-left: 10px;">%</span>
                                                                </div>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="COFINS">
                                                            <ItemTemplate>
                                                                <div style="display: flex;">
                                                                    <asp:Label ID="nCofins" runat="server" ClientIDMode="Static" MaxLength="20" Text='<%# Bind("nCofins", "{0:N2}") %>'></asp:Label>
                                                                    <span style="margin-left: 10px;">%</span>
                                                                </div>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="ICMS">
                                                            <ItemTemplate>
                                                                <div style="display: flex;">
                                                                    <asp:Label ID="nICMS" runat="server" ClientIDMode="Static" MaxLength="20" Text='<%# Bind("nICMS", "{0:N2}") %>'></asp:Label>
                                                                    <span style="margin-left: 10px;">%</span>
                                                                </div>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="CSSL">
                                                            <ItemTemplate>
                                                                <div style="display: flex;">
                                                                    <asp:Label ID="nCSSL" runat="server" ClientIDMode="Static" MaxLength="20" Text='<%# Bind("nCSSL", "{0:N2}") %>'></asp:Label>
                                                                    <span style="margin-left: 10px;">%</span>
                                                                </div>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="IRPJ">
                                                            <ItemTemplate>
                                                                <div style="display: flex;">
                                                                    <asp:Label ID="nIRPJ" runat="server" ClientIDMode="Static" MaxLength="20" Text='<%# Bind("nIRPJ", "{0:N2}") %>'></asp:Label>
                                                                    <span style="margin-left: 10px;">%</span>
                                                                </div>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                                        </asp:TemplateField>

                                                        <asp:BoundField DataField="nAno" HeaderText="Ano">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkEnvio_Excluir" runat="server" CssClass="btn btn-small" TabIndex="100" ToolTip="Apagar Registro" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
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

                        <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

                    </div>

                    <div role="tabpanel" class="tab-pane fade" id="Parametros" aria-labelledby="Parametros">

                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Orçamentos</b></h3>
                            </div>
                            <div class="panel-body">

                                <div class="form-group">
                                    <asp:LinkButton ID="cmdFornecedores_Modal" class="btn btn-info" runat="server" Text="Imagem de Fornecedores - PDF" data-toggle="modal" data-target="#modalUpload_ImagemFornecedores" />
                                </div>

                                <div class="panel panel-default m-0" runat="server" id="Div_Orcamento">
                                    <div class="panel-heading">
                                        <h3 class="panel-title"><b>Tipo de Orçamento</b></h3>
                                    </div>
                                    <div class="panel-body">

                                        <asp:CheckBoxList ID="cblsTipoOrcamento" runat="server" CssClass="form-check-list"></asp:CheckBoxList>

                                    </div>
                                </div>

                            </div>
                        </div>

                        <div class="panel panel-default m-0">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Faturamento</b></h3>
                            </div>
                            <div class="panel-body">

                                <div class="row">
                                    <div class="col-lg-3">
                                        <label>Conta Bancária</label>
                                        <asp:DropDownList runat="server" ID="ddlContaBancaria" class="form-control Caixa_Selecao"></asp:DropDownList>
                                    </div>
                                </div>

                            </div>
                        </div>

                    </div>

                    <div role="tabpanel" class="tab-pane fade" id="arquivos" aria-labelledby="arquivos-tab">
                        <div class=" embed-responsive embed-responsive-16by9" style="min-height: 800px" runat="server" id="upArquivos">
                            <embed type="text/html" runat="server" id="frmArquivos" width="500" height="300" />
                        </div>
                    </div>

                </div>

                <fieldset class="form-stacked actions">
                    <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" ValidationGroup="DETALHE" />
                    <a href="javascript:history.back()" class="btn btn-lg btn-warning" title="Voltar">Voltar</a>
                </fieldset>

                <div id="dialog-Salvar" class="modal" title="Salvar">
                    <p>
                        <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                        <asp:Label ID="lblTituloSalvar" runat="server" Text="Confirma a Inclusão da Empresa?"></asp:Label>
                    </p>
                </div>

                <div class="modal fade" id="modalUpload_ImagemFornecedores">
                    <div class="modal-dialog" style="width: 40%;">
                        <div class="modal-content">
                            <div class="modal-header">
                                <button type="button" class="close" data-dismiss="modal">&times;</button>
                                <h4 class="modal-title">Atualizar Imagem de Fornecedores - PDF</h4>
                            </div>
                            <div class="modal-body">
                                <div class="row">

                                    <div class="col-lg-12">
                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_ImagemFornecedores" />
                                    </div>

                                    <div runat="server" id="div_cmdVisualizar_ImagemFornecedores" class="col-lg-12 form-group text-center">
                                        <asp:LinkButton runat="server" ID="cmdVisualizar_ImagemFornecedores" class="btn btn-primary" Text="Visualizar Imagem Atual" />
                                    </div>

                                    <div class="col-lg-12 form-group" style="display: inline-block; text-align: center;">
                                        <div class="upload-container">
                                            <div class="file-upload-label" id="uploadContainer">
                                                <svg viewBox="0 0 640 512" height="1em">
                                                    <path d="M144 480C64.5 480 0 415.5 0 336c0-62.8 40.2-116.2 96.2-135.9c-.1-2.7-.2-5.4-.2-8.1c0-88.4 71.6-160 160-160c59.3 0 111 32.2 138.7 80.2C409.9 102 428.3 96 448 96c53 0 96 43 96 96c0 12.2-2.3 23.8-6.4 34.6C596 238.4 640 290.1 640 352c0 70.7-57.3 128-128 128H144zm79-217c-9.4 9.4-9.4 24.6 0 33.9s24.6 9.4 33.9 0l39-39V392c0 13.3 10.7 24 24 24s24-10.7 24-24V257.9l39 39c9.4 9.4 24.6 9.4 33.9 0s9.4-24.6 0-33.9l-80-80c-9.4-9.4-24.6-9.4-33.9 0l-80 80z"></path>
                                                </svg>
                                                <p>Arraste e solte aqui o Arquivo</p>
                                                <p>ou</p>
                                                <span class="browse-button">Escolher Arquivo</span>
                                                <asp:FileUpload ID="fu_ImagemFornecedores" runat="server" Style="display: none;" accept=".jpg,.jpeg,.png,.gif,.bmp,.webp,.ico" />
                                            </div>
                                        </div>
                                        <div class="file-name" id="fileName"></div>
                                    </div>

                                </div>
                            </div>
                            <div class="modal-footer">
                                <asp:Button ID="cmdSalva_ImagemFornecedores" class="btn btn-success" runat="server" Text="Atualizar" OnClick="cmdSalva_ImagemFornecedores_Click" />
                                <button type="button" class="btn btn-danger" data-dismiss="modal">Fechar</button>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal fade" id="modalView_ImagemFornecedores">
                    <div class="modal-dialog" style="width: 75%;">
                        <div class="modal-content">
                            <div class="modal-header">
                                <button type="button" class="close" data-dismiss="modal">&times;</button>
                                <h4 class="modal-title">Imagem Atual - Fornecedores</h4>
                            </div>
                            <div class="modal-body">
                                <div class="row">

                                    <div class="col-lg-12">
                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_View_ImagemFornecedores" />
                                        <asp:Image runat="server" ID="imgFornecedores" AlternateText="Fornecedores" ImageAlign="AbsMiddle" />
                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdSalva_ImagemFornecedores" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>