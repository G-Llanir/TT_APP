<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Parceiros_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.Parceiros_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/MultiSelecao.ascx" TagPrefix="uc1" TagName="MultiSelecao" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">

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

        .card-container {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            padding: 20px;
            background-color: #f5f5f5;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            margin: 20px;
        }

        .card {
            flex: 1;
            min-width: 150px;
            max-width: 200px;
            padding: 10px;
            border-radius: 6px;
            text-align: center;
            font-size: 14px;
            transition: transform 0.3s ease;
            position: relative;
            overflow: hidden;
            cursor: pointer;
        }

            .card:hover {
                transform: translateY(-2px);
                box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            }

        .card-header {
            font-weight: bold;
            color: #333;
            margin-bottom: 5px;
            font-size: 12px;
        }

        .card-value {
            font-size: 16px;
            font-weight: bold;
            color: #333;
        }

        .card-label {
            font-size: 12px;
            color: #444444;
            margin-top: 5px;
        }

        .cardVerde {
            background: linear-gradient(135deg, #dcedc8 0%, #c5e1a5 100%); 
            border-left: 5px solid #b1ca94;
        }

        .cardAmareloClaro { 
            background: linear-gradient(135deg, #fff9c4 0%, #fff176 100%);  
            border-left: 5px solid #e5d86a;
        }

        .cardAmareloEscuro {
            background: linear-gradient(135deg, #fff176 0%, #f9a825 100%);
            border-left: 5px solid #e5c164;
        }

        .cardLaranjaClaro {
            background: linear-gradient(135deg, #ffe0b2 0%, #ff9800 100%); 
            border-left: 5px solid #e58800;
        }

        .cardLaranjaEscuro {
            background: linear-gradient(135deg, #ff9800 0%, #e65100 100%);
            border-left: 5px solid #cf4800;
        }

        .cardVermelho {
            background: linear-gradient(135deg, #ffab91 0%, #d32f2f 100%);; 
            border-left: 5px solid #bd2a2a;
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

        select[disabled] {
            background-color: #f5f5f5 !important;
            color: #333 !important;
            opacity: 1;
        }
    </style>

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Detalhe" />
        </div>
    </div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <uc1:MensagemPagina runat="server" ID="MensagemPagina1" />
        </div>
    </div>

    <ul id="tab_Parceiros" class="nav nav-tabs" role="tablist">
        <li role="presentation" class="tabpanel active" runat="server" id="aba_Parceiros">
            <a href="#parceiros" id="aba-parceiros" role="tabpanel" data-toggle="tab" aria-controls="parceiros" aria-expanded="false"><b>Parceiros</b></a>
        </li>
        <li role="presentation" id="aba_Arquivos" runat="server">
            <a href="#arquivos" role="tab" id="arquivos-tab" data-toggle="tab" aria-controls="Arquivos"><b>Arquivos</b></a>
        </li>
        <li role="presentation" id="aba_STSO" runat="server">
            <a href="#STSO" role="tab" id="STSO-tab" data-toggle="tab" aria-controls="STSO"><b>STSO</b></a>
        </li>
        <li role="presentation" id="aba_Financeiro" runat="server">
            <a href="#financeiro" role="tab" id="financeiro-tab" data-toggle="tab" aria-controls="financeiro"><b>Financeiro</b></a>
        </li>
        <li role="presentation" id="aba_Historico" runat="server">
            <a href="#historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="historico"><b>Histórico</b></a>
        </li>
    </ul>

    <br />

    <div id="tab" class="tab-content">

        <div role="tabpanel" class="tab-pane fade in active" id="parceiros" aria-labelledby="parceiros-tab">
            <asp:UpdatePanel ID="updDetalhe" runat="server">
                <ContentTemplate>

                    <div class="panel panel-default" runat="server" id="Div1">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Dados do Parceiro</b></h3>
                        </div>
                        <div class="panel-body">

                            <div class="form-stacked row">

                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                                </div>

                                <%--<div class="col-lg-6">
                                    <div class="form-group">
                                        <label>ID </label>
                                        <asp:TextBox ID="txtidCliente" class="form-control CaixaTextoMini" runat="server" disabled></asp:TextBox>
                                    </div>
                                </div>--%>

                                <div class="col-lg-12">
                                    <div class="row">
                                        <div class="col-lg-1">
                                            <div class="form-group">
                                                <label>ID</label>

                                                <div class="form-group input-group CaixaTextoPequeno">

                                                    <asp:LinkButton ID="cmRetornar" class="btn btn-sm btn-info input-group-addon colorLink" runat="server" OnClick="cmdRetornar_click"><b><</b></asp:LinkButton>

                                                    <asp:TextBox ID="txtidCliente" class="form-control" runat="server" disabled="0"></asp:TextBox>

                                                    <asp:LinkButton ID="cmAvancar" class="btn btn-sm btn-info input-group-addon colorLink" runat="server" OnClick="cmdAvancar_click"><b>></b></asp:LinkButton>

                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-12">
                                            <div class="form-group">
                                                <asp:CheckBoxList ID="cblsParceiros" runat="server" RepeatColumns="10" RepeatLayout="Table" RepeatDirection="Vertical" AutoPostBack="true" OnSelectedIndexChanged="cblsParceiros_SelectedIndexChanged">
                                                    <asp:ListItem Value="N">&nbsp;Nacional&nbsp;&nbsp;&nbsp;</asp:ListItem>
                                                    <asp:ListItem Value="E">&nbsp;Estrangeiro&nbsp;&nbsp;&nbsp;&nbsp; </asp:ListItem>
                                                    <asp:ListItem Value="U">&nbsp;USA&nbsp;&nbsp;&nbsp;&nbsp; </asp:ListItem>
                                                </asp:CheckBoxList>
                                            </div>
                                        </div>

                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>Tipo</label>
                                                <asp:DropDownList ID="ddlsTipo" runat="server" class="form-control yes_no select " AutoPostBack="True" OnSelectedIndexChanged="ddlsTipo_SelectedIndexChanged">
                                                    <asp:ListItem Value="">Selecione</asp:ListItem>
                                                    <asp:ListItem Value="F">Física</asp:ListItem>
                                                    <asp:ListItem Value="J">Juridica</asp:ListItem>
                                                    <asp:ListItem Value="O">Outros</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="DIV_CNPJ">
                                            <div class="form-group">
                                                <label>
                                                    <asp:Label ID="lblsCPF_CNPJ" runat="server" Text="CNPJ"></asp:Label></label>
                                                <asp:TextBox ID="txtsCNPJ" class="form-control" runat="server" MaxLength="20"> </asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="DIV_CPF">
                                            <div class="form-group">
                                                <label>
                                                    <asp:Label ID="Label1" runat="server" Text="CPF"></asp:Label></label>
                                                <asp:TextBox ID="txtsCPF" class="form-control" runat="server" MaxLength="20"> </asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="DIV_VAT">
                                            <div class="form-group">
                                                <label>
                                                    <asp:Label ID="Label2" runat="server" Text="VAT"></asp:Label></label>
                                                <asp:TextBox ID="txtsVAT" class="form-control uppercase" runat="server" MaxLength="20"> </asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="DIV_FEIN">
                                            <div class="form-group">
                                                <label>
                                                    <asp:Label ID="Label3" runat="server" Text="FEIN"></asp:Label></label>
                                                <asp:TextBox ID="txtsFEIN" class="form-control" runat="server" MaxLength="20"> </asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="DIV_RNE">
                                            <div class="form-group">
                                                <label>
                                                    <asp:Label ID="Label4" runat="server" Text="RNE"></asp:Label></label>
                                                <asp:TextBox ID="txtsRNE" class="form-control" runat="server" MaxLength="20"> </asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="DIV_IE">
                                            <div class="form-group">
                                                <label>
                                                    <asp:Label ID="lbl_sRG_IE" runat="server" Text="Label"></asp:Label></label>
                                                <asp:TextBox ID="txtsRG_IE" class="form-control" runat="server" MaxLength="20"> </asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="DIV_IM">
                                            <div class="form-group">
                                                <label>
                                                    <asp:Label ID="Label5" runat="server" Text="Inscrição Municipal"></asp:Label></label>
                                                <asp:TextBox ID="txtsInscricaoMunicipal" class="form-control" runat="server" MaxLength="15"> </asp:TextBox>
                                            </div>
                                        </div>


                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <asp:Image ID="imgParceiroPrincipal" runat="server" class="text-center" Height="150" Width="250" Style="display: inline-block; vertical-align: middle; margin-right: 10px;" />
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="row">
                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>
                                            <asp:Label ID="lblsRazaoSocial" runat="server" Text="Label"></asp:Label></label>
                                        <asp:TextBox ID="txtsRazaoSocial" class="form-control" runat="server" MaxLength="200"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>
                                            <asp:Label ID="lblsNomeFantasia" runat="server" Text="Label"></asp:Label></label>
                                        <asp:TextBox ID="txtsNomeFantasia" class="form-control " runat="server" MaxLength="200"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Pais Origem</label>
                                        <asp:DropDownList ID="ddlidPais" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-2" id="DIV_Vendedor" runat="server">
                                    <div class="form-group">
                                        <label>Vendedor</label>
                                        <asp:DropDownList ID="ddlidVendedor" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Tabela</label>
                                        <asp:ListBox ID="lstidTabela" runat="server" class="form-control Caixa_Selecao" SelectionMode="Multiple"></asp:ListBox>
                                    </div>
                                </div>








                                <div class="col-lg-2" id="DIV_Comprador" runat="server">
                                    <div class="form-group">
                                        <label>Comprador</label>
                                        <asp:DropDownList ID="ddlidComprador" runat="server" class="form-control yes_no select "></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="DIV_SUFRAMA">
                                    <div class="form-group">
											   
                                        <label>Inscrição SUFRAMA</label>
                                        <asp:TextBox ID="txtsSuframa" class="form-control" runat="server" MaxLength="20"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-12 row">
                                    <div class="col-lg-2 form-group">
                                        <uc1:SwitchAtivo runat="server" ID="SwitchAtivo_VendaIndividual" />
                                    </div>

                                    <div class="col-lg-2 form-group">
                                        <uc1:SwitchAtivo runat="server" ID="SwitchAtivo_Drawback" />
                                    </div>

                                    <div class="col-lg-2 form-group">
                                        <uc1:SwitchAtivo runat="server" ID="SwitchAtivo_Contribuinte" />
                                    </div>
                                </div>


                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Tipo de Parceiro</label>
                                        <asp:CheckBoxList ID="cblidTipoParceiro" runat="server" CssClass="tipos" ToolTip="Tipo de Parceiro" OnSelectedIndexChanged="cblidTipoParceiro_SelectedIndexChanged" AutoPostBack="true"></asp:CheckBoxList>
                                    </div>
                                </div>

                                <div runat="server" id="divTabelaVinculada" class="col-lg-3">
                                    <div class="form-group">
                                        <label>Tabela de Preço Vinculada</label>
                                        <asp:TextBox runat="server" ID="txtTabelaVinculada" class="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

                    <div class="panel panel-default" runat="server" id="Div_Contatos">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Contatos</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <div class="row" runat="server" id="div_Contato_Selecao">
                                            <div class="col-lg-4">
                                                <asp:LinkButton ID="cmdContato_Incluir" runat="server" CssClass="btn btn-info" OnClick="cmdContato_Incluir_Click"><i class="icon-bar"></i>&nbsp;Adicionar</asp:LinkButton>
                                            </div>
                                        </div>
                                        <div id="div_InputContato" class="row" runat="server">
                                            <div class="col-lg-11">
                                                <div class="panel panel-primary">
                                                    <div class="panel-heading">
                                                        <h3 class="panel-title">
                                                            <asp:Label ID="lblContato_Titulo" runat="server" Text="Label"></asp:Label></h3>
                                                    </div>
                                                    <div class="panel-body">

                                                        <div class="col-lg-12">
                                                            <uc1:MensagemPagina runat="server" ID="msgContato" />
                                                        </div>
                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <label>Tipo Contato</label>
                                                                <asp:TextBox ID="txtContato_sTipoContato" class="form-control CaixaTextoPequeno" runat="server" Visible="false"></asp:TextBox>
                                                                <asp:DropDownList CssClass="form-control yes_no select Caixa_Selecao" ID="ddlContato_sTipoContato" runat="server"
                                                                    AutoPostBack="true" OnSelectedIndexChanged="ddlContato_sTipoContato_SelectedIndexChanged">
                                                                    <asp:ListItem Value="" Text="Selecione uma opção" />
                                                                    <asp:ListItem Value="Vendas" Text="Vendas" />
                                                                    <asp:ListItem Value="Compras" Text="Compras" />
                                                                    <asp:ListItem Value="Financeiro" Text="Financeiro" />
                                                                    <asp:ListItem Value="Principal" Text="Principal" />
                                                                    <asp:ListItem Value="Telefone" Text="Telefone" />
                                                                    <asp:ListItem Value="Principal" Text="Principal" />
                                                                    <asp:ListItem Value="Outro" Text="Outro" />
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-5">
                                                            <div class="form-group">
                                                                <label>Nome e Sobrenome</label>
                                                                <asp:TextBox ID="txtContato_sNome" class="form-control CaixaTextoGrande" runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-4">
                                                            <div class="form-group">
                                                                <label>Telefone</label>
                                                                <asp:TextBox ID="txtContato_sTelefone" class="form-control CaixaTextoMedio" runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <label>e-mail</label>
                                                                <asp:TextBox ID="txtContato_sEmail" class="form-control" runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <br />
                                                                <asp:CheckBox ID="EMAIL" Text=" Receber Info. Financeira (Via E-mail)" runat="server" />
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-4">
                                                            <div class="form-group">
                                                                <br />
                                                                <asp:Button ID="cmdContato_Confirmar" class="btn btn-sm btn-primary" runat="server" OnClick="cmdContato_Confirmar_Click" Text="Ok" />
                                                                <asp:Button ID="cmdContato_Cancelar" class="btn btn-sm btn-danger" runat="server" Text="Cancelar" OnClick="cmdContato_Cancelar_Click" />
                                                            </div>
                                                        </div>
                                                        <asp:HiddenField ID="hddContato_index" runat="server" />

                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-12 table-responsive">
                                                <asp:GridView ID="gvContato" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvContato_RowDataBound" OnSelectedIndexChanging="gvContato_SelectedIndexChanging">
                                                    <Columns>
                                                        <asp:BoundField DataField="idContato" HeaderText="idContato">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sTipoContato" HeaderText="Tipo">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sNome" HeaderText="Nome e Sobrenome">
                                                            <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sTelefone" HeaderText="Telefone">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sEmail" HeaderText="e-mail">
                                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sRecebeEmail" HeaderText="Receber Email">
                                                            <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkContato_Editar" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Select"><i class="fa-pencil fa"></i></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkContato_Excluir" runat="server" CssClass="btn btn-small" TabIndex="100"><i class="fa-eraser fa"></i></asp:LinkButton>
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

                    <div class="panel panel-default" runat="server" id="Div_Enderecos">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Endereços</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <div class="col-lg-12">
                                            <div class="row">
                                                <uc1:MensagemPagina runat="server" ID="MensagemPagina2" />
                                            </div>
                                        </div>
                                        <div class="row" runat="server" id="divEndereco_Selecao">
                                            <div class="col-lg-4">
                                                <asp:LinkButton ID="cmdEndereco_Incluir" runat="server" CssClass="btn btn-info" OnClick="cmdEndereco_Incluir_Click"><i class="icon-bar"></i>&nbsp;Adicionar</asp:LinkButton>
                                            </div>
                                        </div>
                                        <asp:UpdatePanel ID="upd_Endereco" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="False">
                                            <ContentTemplate>
                                                <div id="divEndereco_Input" class="row" runat="server">
                                                    <div class="col-lg-11">
                                                        <div class="panel panel-primary">
                                                            <div class="panel-heading">
                                                                <h3 class="panel-title">
                                                                    <asp:Label ID="lblEndereco_Titulo" runat="server" Text="Label"></asp:Label></h3>
                                                            </div>
                                                            <div class="panel-body">
                                                                <div class="col-lg-12">
                                                                    <uc1:MensagemPagina runat="server" ID="msgEndereco" />
                                                                </div>

                                                                <div class="col-lg-6">
                                                                    <div class="form-group">
                                                                        <label>Tipo</label>
                                                                        <asp:DropDownList ID="ddlEndereco_TipoEndereco" runat="server" class="form-control CaixaTextoMedio"></asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-6">
                                                                    <div class="row">
                                                                        <div class="col-lg-4">
                                                                            <div class="form-group">
                                                                                <label>CEP</label>
                                                                                <asp:TextBox ID="txtEndereco_sCEP" class="form-control" runat="server" MaxLength="9" Style="width: 150px;" OnTextChanged="txtEndereco_sCEP_TextChanged" AutoPostBack="True"></asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                        <div class="col-lg-4">
                                                                            <div class="form-group">
                                                                                <asp:UpdateProgress ID="updCliente" runat="server" OnDataBinding="txtEndereco_sCEP_TextChanged">
                                                                                    <ProgressTemplate>
                                                                                        Aguarde
                                                                                    </ProgressTemplate>
                                                                                </asp:UpdateProgress>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>



                                                                <div class="col-lg-6">
                                                                    <div class="form-group">
                                                                        <label>Endereço</label>
                                                                        <asp:TextBox ID="txtEndereco_sLogradouro" class="form-control uppercase CaixaTextoGrande" MaxLength="200" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-6">
                                                                    <div class="form-group">
                                                                        <label>Número</label>
                                                                        <asp:TextBox ID="txtEndereco_sNumero" class="form-control CaixaTextoPequeno" runat="server" MaxLength="10"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-6">
                                                                    <div class="form-group">
                                                                        <label>Complemento&nbsp;&nbsp; Ex: AP XX, BL ZZ, CS 00, CJ 00, SL 00</label>
                                                                        &nbsp;<asp:TextBox ID="txtEndereco_sComplemento" class="form-control uppercase CaixaTextoGrande" MaxLength="100" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-6">
                                                                    <div class="form-group">
                                                                        <label>Bairro</label>
                                                                        <asp:TextBox ID="txtEndereco_sBairro" class="form-control  CaixaTextoGrande" runat="server" MaxLength="100"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-6">
                                                                    <div class="form-group">
                                                                        <label>
                                                                            <asp:Label ID="lbl_Cidade" runat="server" Text="Cidade"></asp:Label>
                                                                        </label>
                                                                        <asp:TextBox ID="txtEndereco_sCidade" class="form-control CaixaTextoGrande" runat="server" MaxLength="200"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-6" runat="server" id="DIV_ddlEstado">
                                                                    <div class="form-group">
                                                                        <label>Estado</label>
                                                                        <asp:DropDownList ID="ddlEndereco_sEstado" runat="server" class="form-control CaixaTextoPequeno"></asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-6" runat="server" id="DIV_txtEstado">
                                                                    <div class="form-group">
                                                                        <label>Estado Ex: SP, RJ ... </label>
                                                                        <asp:TextBox ID="txtEstrangeiro_sEstado" class="form-control CaixaTextoPequeno" runat="server" MaxLength="10"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-6">
                                                                    <div class="form-group">
                                                                        <label>País</label>
                                                                        <asp:TextBox ID="txtEndereco_sPais" class="form-control CaixaTextoGrande" runat="server" MaxLength="200"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-6">
                                                                    <div class="form-group">
                                                                        <br />
                                                                        <asp:Button ID="cmdEndereco_Confirma" class="btn btn-sm btn-primary" runat="server" OnClick="cmdEndereco_Confirmar_Click" Text="Ok" />
                                                                        <asp:Button ID="cmdEndereco_Cancela" class="btn btn-sm btn-danger" runat="server" OnClick="cmdEndereco_Cancelar_Click" Text="Cancelar" />
                                                                    </div>
                                                                </div>
                                                                <asp:HiddenField ID="hddEndereco_Index" runat="server" />


                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="txtEndereco_sCEP" EventName="TextChanged" />
                                            </Triggers>
                                        </asp:UpdatePanel>

                                        <div class="row">
                                            <div class="col-lg-12 table-responsive">
                                                <asp:GridView ID="gvEndereco" class="table table-striped table-bordered table-hover table-condensed"
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvEndereco_RowDataBound" OnSelectedIndexChanging="gvEndereco_SelectedIndexChanging">
                                                    <Columns>
                                                        <asp:BoundField DataField="idEndereco" HeaderText="idEndereco"></asp:BoundField>

                                                        <asp:BoundField DataField="sDscTipoEndereco" HeaderText="Tipo">
                                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sEnderecoCompleto" HeaderText="Endereço">
                                                            <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sEnderecoEstrangeiro" HeaderText="Endereço">
                                                            <ItemStyle Width="35%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkContato_Editar" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Select"><i class="fa-pencil fa"></i></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkContato_Excluir" runat="server" CssClass="btn btn-small" TabIndex="100"><i class="fa-eraser fa"></i></asp:LinkButton>
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

                    <div class="panel panel-default" runat="server" id="Div_CanalBancario">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Canal Bancário</b></h3>
                        </div>
                        <div class="panel-body">

                            <div class="row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="msgBancario" />
                                </div>
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <div class="row" runat="server" id="div_cmdBancario">
                                            <div class="col-lg-4">
                                                <asp:LinkButton ID="cmdBancario" runat="server" CssClass="btn btn-info" OnClick="cmdBancario_Click"><i class="icon-bar"></i>&nbsp;Adicionar</asp:LinkButton>
                                            </div>
                                        </div>
                                        <div id="div_imputBancario" class="row" runat="server">
                                            <div class="col-lg-11">
                                                <div class="panel panel-primary">
                                                    <div class="panel-heading">
                                                        <h3 class="panel-title">
                                                            <asp:Label ID="lblBancario" runat="server" Text="Label"></asp:Label></h3>
                                                    </div>
                                                    <div class="panel-body">


                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <label>Company</label>
                                                                <asp:TextBox ID="txtCompany" class="form-control" runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <label>Country</label>
                                                                <asp:TextBox ID="txtCoutry" class="form-control" runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <label>Bank</label>
                                                                <asp:TextBox ID="txtBank" class="form-control " runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <label>Swift / Aba</label>
                                                                <asp:TextBox ID="txtSwift" class="form-control " runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <label>Routing Number</label>
                                                                <asp:TextBox ID="txtAba" class="form-control " runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <label>Moeda</label>
                                                                <asp:DropDownList ID="ddlidMoeda" runat="server" class="form-control"></asp:DropDownList>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <label>Account</label>
                                                                <asp:TextBox ID="txtAccount" class="form-control " runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-4">
                                                            <div class="form-group">
                                                                <label>Endereço</label>
                                                                <asp:TextBox ID="txtEnderecoBancario" class="form-control" runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2">
                                                            <div class="form-group">
                                                                <br />
                                                                <asp:Button ID="cmdBancario_Confirmar" class="btn btn-sm btn-primary" runat="server" OnClick="cmdBancario_Confirmar_Click" Text="Ok" />
                                                                <asp:Button ID="cmdBancario_Cancelar" class="btn btn-sm btn-danger" runat="server" Text="Cancelar" OnClick="cmdBancario_Cancelar_Click" />
                                                            </div>
                                                        </div>


                                                        <asp:HiddenField ID="hddBancario_index" runat="server" />

                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-12 table-responsive">
                                                <asp:GridView ID="gv_Bancario" class="table table-striped table-bordered table-hover table-condensed table-responsive" DataKeyNames="idBancario"
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small"
                                                    OnRowDataBound="gv_Bancario_RowDataBound" OnSelectedIndexChanging="gv_Bancario_SelectedIndexChanging">
                                                    <Columns>
                                                        <asp:BoundField DataField="idBancario" HeaderText="ID">
                                                            <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sCompany" HeaderText="Company">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sCoutry" HeaderText="Country">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sBank" HeaderText="Bank">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sSwift" HeaderText="Swift / Aba">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sAba" HeaderText="Routing Number">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sAccount" HeaderText="Account">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sEndereco" HeaderText="Endereco">
                                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:TemplateField HeaderText="Moeda">
                                                            <ItemTemplate>
                                                                <asp:HiddenField ID="hddidMoeda" runat="server" Value='<%# Eval("idMoeda") %>' />
                                                                <label><%# Eval("sDscTipoMoeda") %></label>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkBancario_Editar" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Select"><i class="fa-pencil fa"></i></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkBancario_Excluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandArgument='<%# Eval("idBancario") %>'><i class="fa-eraser fa"></i></asp:LinkButton>
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

                    <div class="panel panel-default" runat="server" id="div_Usuarios">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Usuários</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <div class="row" runat="server" id="div3">
                                            <div class="col-lg-4">
                                                <asp:LinkButton ID="lnkUsuarios_Adicionar" runat="server" CssClass="btn btn-info" OnClick="lnkUsuarios_Adicionar_Click"><i class="icon-bar"></i>&nbsp;Adicionar</asp:LinkButton>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12 table-responsive">
                                                <asp:GridView ID="gvUsuario" class="table table-striped table-bordered table-hover table-condensed"
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvUsuario_RowDataBound" OnSelectedIndexChanging="gvUsuario_SelectedIndexChanging">
                                                    <Columns>
                                                        <asp:BoundField DataField="idUsuario" HeaderText="idUsuario"></asp:BoundField>

                                                        <asp:BoundField DataField="sLogin" HeaderText="Login">
                                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário">
                                                            <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sEmail" HeaderText="e-mail">
                                                            <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sSituacao_Completa" HeaderText="Ativo">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>



                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkContato_Editar" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Select"><i class="fa-pencil fa"></i></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkContato_Excluir" runat="server" CssClass="btn btn-small" TabIndex="100"><i class="fa-eraser fa"></i></asp:LinkButton>
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

                    <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="updFamilia">
                        <ContentTemplate>
                            <div class="panel panel-default" runat="server" id="div2">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>Família de Produtos (Liberada para Cotação)</b></h3>
                                </div>
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-lg-12">
                                            <div class="form-group">
                                                <div class="col-lg-12">
                                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina4" />
                                                </div>
                                                <div class="row" runat="server" id="div4">
                                                    <div class="col-lg-4">
                                                        <asp:LinkButton ID="lnkFamilias" runat="server" CssClass="btn btn-info" OnClick="lnkFamilias_Click"><i class="icon-bar"></i>&nbsp;Adicionar</asp:LinkButton>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <asp:MultiView runat="server" ID="BaseMultiview_Familia">
                                                        <asp:View runat="server" ID="view_Familia_Adicionar">
                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Selecione a Família de Produtos</label>
                                                                    <asp:DropDownList runat="server" ID="ddlFamilias" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-4">
                                                                <div class="form-group">
                                                                    <label for="cmdNovo" style="display: block; margin-bottom: 5px;">&nbsp;</label>
                                                                    <asp:Button ID="cmdIncluirFamilia" CssClass="btn btn-primary btn-review" runat="server" Text='Incluir' OnClick="cmdIncluirFamilia_Click" />
                                                                </div>
                                                            </div>
                                                        </asp:View>
                                                    </asp:MultiView>
                                                    <div class="col-lg-12 table-responsive">
                                                        <asp:GridView ID="gvFamilias" class="table table-striped table-bordered table-hover table-condensed"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvFamilias_RowDataBound" OnRowDeleting="gvFamilias_RowDeleting">
                                                            <Columns>
                                                                <asp:BoundField DataField="idContador" HeaderText="idContador"></asp:BoundField>
                                                                <asp:BoundField DataField="idFamilia" HeaderText="idFamilia"></asp:BoundField>
                                                                <asp:BoundField DataField="idFamilia" HeaderText="ID">
                                                                    <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sDscFamilia" HeaderText="Família Liberada">
                                                                    <ItemStyle Width="80%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkFamilia" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete">
                                                                            <i class="fa-eraser fa"></i>
                                                                        </asp:LinkButton>
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
                    </asp:UpdatePanel>

                    <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="updCondicaoPagamento">
                        <ContentTemplate>
                            <div class="panel panel-default" runat="server" id="div5">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>Condições de Pagamento (Liberada para Cotação) </b></h3>
                                </div>
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-lg-12">
                                            <div class="form-group">
                                                <div class="col-lg-12">
                                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina3" />
                                                </div>
                                                <div class="row" runat="server" id="div6">
                                                    <div class="col-lg-4">
                                                        <asp:LinkButton ID="lnkCondicaoPagamento" runat="server" CssClass="btn btn-info" OnClick="lnkPagamento_Click"><i class="icon-bar"></i>&nbsp;Adicionar</asp:LinkButton>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <asp:MultiView runat="server" ID="BaseMultiview_CondicaoPagamento">
                                                        <asp:View runat="server" ID="view_CondicaoPagamento_Adicionar">
                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Selecione a Condição de Pagamento</label>
                                                                    <asp:DropDownList runat="server" ID="ddlCondicaoPagamento" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-4">
                                                                <div class="form-group">
                                                                    <label for="cmdNovo" style="display: block; margin-bottom: 5px;">&nbsp;</label>
                                                                    <asp:Button ID="cmdIncluirCondicaoPagamento" CssClass="btn btn-primary btn-review" runat="server" Text='Incluir' OnClick="cmdIncluirCondicaoPagamento_Click" />
                                                                </div>
                                                            </div>
                                                        </asp:View>
                                                    </asp:MultiView>
                                                    <div class="col-lg-12 table-responsive">
                                                        <asp:GridView ID="gvCondicaoPagamento" class="table table-striped table-bordered table-hover table-condensed"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvCondicaoPagamento_RowDataBound" OnRowDeleting="gvCondicaoPagamento_RowDeleting">
                                                            <Columns>
                                                                <asp:BoundField DataField="idContador" HeaderText="idContador"></asp:BoundField>
                                                                <asp:BoundField DataField="idCondicaoPagamento" HeaderText="Condição de Pagamento"></asp:BoundField>

                                                                <asp:BoundField DataField="idCondicaoPagamento" HeaderText="ID">
                                                                    <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sDscCondicaoPagamento" HeaderText="Condições Liberadas">
                                                                    <ItemStyle Width="80%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkCondicaoPagamento" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete">
                                                                            <i class="fa-eraser fa"></i>
                                                                        </asp:LinkButton>
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
                    </asp:UpdatePanel>

                    <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="updCondicaoCompra">
                        <ContentTemplate>
                            <div class="panel panel-default" runat="server" id="div7">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>Condições de Pagamento (Compras) </b></h3>
                                </div>
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-lg-12">
                                            <div class="form-group">
                                                <div class="col-lg-12">
                                                    <uc1:MensagemPagina runat="server" ID="MensagemCondiçãoCompra" />
                                                </div>
                                                <div class="row" runat="server" id="div8">
                                                    <div class="col-lg-4">
                                                        <asp:LinkButton ID="lnkCondicaoCompra" runat="server" CssClass="btn btn-info" OnClick="lnkCondicaoCompra_Click"><i class="icon-bar"></i>&nbsp;Adicionar</asp:LinkButton>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <asp:MultiView runat="server" ID="BaseMultiview_CondicaoCompra">
                                                        <asp:View runat="server" ID="view1">
                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Selecione a Condição de Pagamento</label>
                                                                    <asp:DropDownList runat="server" ID="ddlCondicaoCompra" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-4">
                                                                <div class="form-group">
                                                                    <label for="cmdNovo" style="display: block; margin-bottom: 5px;">&nbsp;</label>
                                                                    <asp:Button ID="cmdIncluirCondicaoCompra" CssClass="btn btn-primary btn-review" runat="server" Text='Incluir' OnClick="cmdIncluirCondicaoCompra_Click" />
                                                                </div>
                                                            </div>
                                                        </asp:View>
                                                    </asp:MultiView>
                                                    <div class="col-lg-12 table-responsive">
                                                        <asp:GridView ID="gvCondicaoCompras" class="table table-striped table-bordered table-hover table-condensed"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvCondicaoCompras_RowDataBound" OnRowDeleting="gvCondicaoCompras_RowDeleting">
                                                            <Columns>
                                                                <asp:BoundField DataField="idContador" HeaderText="idContador"></asp:BoundField>
                                                                <asp:BoundField DataField="idCondicaoPagamento" HeaderText="Condição de Pagamento"></asp:BoundField>

                                                                <asp:BoundField DataField="idCondicaoPagamento" HeaderText="ID">
                                                                    <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sDscCondicaoPagamento" HeaderText="Condições Liberadas">
                                                                    <ItemStyle Width="80%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkCondicaoExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
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
                    </asp:UpdatePanel>

                    <div class="form-stacked row">

                        <div class="col-lg-12">
                            <div class="form-group">
                                <label>Observações Gerais</label>
                                <asp:TextBox ID="txtsObservacao" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="150px"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-lg-12">
                            <div class="form-group">
                                <label>Observações Financeiras</label>
                                <asp:TextBox ID="txtsObservacaoFinanceira" class="form-control" runat="server" MaxLength="200" TextMode="MultiLine" Height="100px"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
                        </div>
                    </div>

                    <asp:HiddenField ID="hddidParceiro" runat="server" />
                    <asp:HiddenField ID="hdd_modalExcluir" runat="server" />
                    <asp:HiddenField ID="hddidTipoPagamentoCompra" runat="server" />

                    <div id="dialog-Excluir" class="modal" style="padding: 10px;" title="Excluir">
                        <p>
                            <label id="lblTituloExcluir">Deseja excluir este item?</label>
                        </p>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="arquivos" aria-labelledby="arquivos-tab">
            <div class=" embed-responsive embed-responsive-16by9" style="min-height: 800px" runat="server" id="upArquivos">
                <embed type="text/html" runat="server" id="frmArquivos" width="900" height="500" />
            </div>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="STSO" aria-labelledby="STSO-tab">
            <asp:UpdatePanel ID="updSTSO" runat="server" UpdateMode="Conditional">
                <ContentTemplate>

                    <div class="panel panel-default" runat="server" id="DIV9">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Dados para acesso a Sistema STSO</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12 table-responsive">
                                    <div class="row">
                                        <div class="col-lg-12">
                                            <uc1:MensagemPagina runat="server" ID="MensagemPagina5" />
                                        </div>
                                        <div class="col-lg-3">
                                            <div class="form-group">
                                                <label>Tipo de Envio</label>
                                                <asp:DropDownList ID="ddlidTipoEnvio" runat="server" class="form-control yes_no select">
                                                    <asp:ListItem Value="0" Text="Selecione de Envio" />
                                                    <asp:ListItem Value="1" Text="Email" />
                                                    <asp:ListItem Value="2" Text="Aplicativo" />
                                                    <%--<asp:ListItem Value="3" Text=""/>--%>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col-lg-3" runat="server" id="Div10">
                                            <div class="form-group">
                                                <label>Endereço</label>
                                                <asp:TextBox ID="txtsEndereco" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-3" runat="server" id="Div11">
                                            <div class="form-group">
                                                <label>Usuário</label>
                                                <asp:TextBox ID="txtsUsuario" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-2" runat="server" id="Div12" style="padding-right: 0;">
                                            <div class="form-group">
                                                <label>Senha</label>
                                                <asp:TextBox ID="txtsSenha" class="form-control" runat="server" TextMode="Password"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-1" style="padding-left: 0;">
                                            <div class="form-group">
                                                <label>&nbsp;</label>
                                                <br />
                                                <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-small fa fa-eye" TabIndex="100" ToolTip="Visualizar Senha" OnClick="LinkButton1_Click"></asp:LinkButton>
                                                <asp:LinkButton ID="LinkButton3" runat="server" CssClass="btn btn-small fa fa-eye-slash" TabIndex="100" ToolTip="Esconder Senha" OnClick="LinkButton2_Click"></asp:LinkButton>
                                            </div>
                                        </div>
                                        <div class="col-lg-5" runat="server" id="Div13">
                                            <div class="form-group">
                                                <label>Observação</label>
                                                <asp:TextBox ID="txtsObservacaoSTSO" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-1" runat="server" id="Div14">
                                            <div class="form-group">
                                                <label>&nbsp;</label>
                                                <br />
                                                <asp:LinkButton ID="lnkAdicionar" runat="server" CssClass="btn btn-info" OnClick="lnkAdicionar_Click"><i class="icon-bar"></i>&nbsp;Adicionar</asp:LinkButton>
                                            </div>
                                        </div>
                                        <div class="col-lg-12 table-responsive">
                                            <asp:GridView ID="gvSTSO" class="table table-striped table-bordered table-hover table-condensed"
                                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvSTSO_RowDataBound" OnRowDeleting="gvSTSO_RowDeleting">
                                                <Columns>
                                                    <asp:BoundField DataField="idContador" HeaderText="idContador"></asp:BoundField>

                                                    <asp:BoundField DataField="idUsuario" HeaderText="ID"></asp:BoundField>

                                                    <asp:BoundField DataField="sDscTipo" HeaderText="Tipo de Envio">
                                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sEndereco" HeaderText="Endereço">
                                                        <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sUsuario" HeaderText="Usuário">
                                                        <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:TemplateField HeaderText="Valor Unitário">
                                                        <HeaderTemplate>
                                                            <div style="display: flex; align-items: center;">
                                                                <span>Senha</span>
                                                                <div style="margin-left: 10px;">
                                                                    <asp:LinkButton ID="TodoslnkVisualizarSenha" runat="server" ClientIDMode="Static" OnClick="TodoslnkVisualizarSenha_Click" data-toggle="tooltip" title="Visualizar Senha" Style="margin-left: 5px;" CssClass="fa fa-eye"></asp:LinkButton>
                                                                </div>
                                                            </div>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <div style="display: flex; align-items: center;">
                                                                <div class="form-group">
                                                                    <asp:Label ID="lblsSenha" runat="server" ClientIDMode="Static" Text='<%# Bind("sSenha") %>'></asp:Label>
                                                                </div>
                                                            </div>
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="15%" />
                                                    </asp:TemplateField>

                                                    <%--<asp:BoundField DataField="sSenha" HeaderText="Senha">
                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>--%>

                                                    <asp:BoundField DataField="sObservacao" HeaderText="Observação">
                                                        <ItemStyle Width="40%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:TemplateField HeaderText="">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkSTSO" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
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

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>


        <div role="tabpanel" class="tab-pane fade" id="financeiro" aria-labelledby="financeiro-tab">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>

                    <div class="panel panel-default" runat="server" id="DIV16">

                        <div class="panel-body">

                            <ul id="tab_DadosFinanceiros" class="nav nav-tabs" role="tablist">
                                <li role="presentation" class="tabpanel active" runat="server" id="aba_DadosFinanceiros">
                                    <a href="#dadosFinanceiros" id="aba-dadosFinanceiros" role="tabpanel" data-toggle="tab" aria-controls="dadosFinanceiros" aria-expanded="false"><b>Dados Financeiros</b></a>
                                </li>
                                <li role="presentation" id="aba_titulos" runat="server">
                                    <a href="#titulos" role="tab" id="titulos-tab" data-toggle="tab" aria-controls="titulos"><b>Títulos</b></a>
                                </li>
                                <li role="presentation" id="aba_grafico" runat="server">
                                    <a href="#grafico" role="tab" id="grafico-tab" data-toggle="tab" aria-controls="grafico"><b>Gráficos</b></a>
                                </li>
                            </ul>

                            <div id="tabFinanceiro" class="tab-content">

                                <div role="tabpanel" class="tab-pane fade in active" id="dadosFinanceiros" aria-labelledby="dadosFinanceiros-tab">
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>

                                            <div class="panel panel-default" runat="server" id="DIV17">
                                                <%--<div class="panel-heading">
                                                    <h3 class="panel-title"><b></b>Dados Financeiros</h3>
                                                </div>--%>
                                                <div class="panel-body">

                                                    <div class="col-lg-12">
                                                        <uc1:MensagemPagina runat="server" ID="MensagemPaginaDadosFinanceiros" />
                                                    </div>

                                                    <div class="col-lg-12">
                                                        <div class="row">
                                                            <div class="col-lg-2">
                                                                <div class="form-group">
                                                                    <label>Status Administrativo</label>
                                                                    <asp:DropDownList ID="ddlidTipoSituacaoCliente" runat="server" class="form-control yes_no select"></asp:DropDownList>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Limite de Crédito</label>
                                                                    <asp:TextBox ID="txtnLimiteCredito" class="form-control" runat="server"></asp:TextBox>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Crédito Disponível</label>
                                                                    <asp:TextBox ID="txtnCreditoDisponivel" class="form-control" runat="server" ReadOnly="true" Text='<%# Bind("nCreditoDisponivel", "{0:N2}") %>'></asp:TextBox>
                                                                </div>
                                                            </div>

                                                            <div runat="server" id="div_bloquearEdicao" class="col-lg-4" style="display: flex; justify-content: right;">
                                                                <div class="form-group">
                                                                    <input type="checkbox" id="lock" <%= hddsCadeado.Value == "N" ? "checked='checked'" : "" %> />
                                                                    <label for="lock" class="<%= hddsPermissaoCadeado.Value == "0" ? "lock-label lock-label-closed" : "lock-label" %>">
                                                                        <span class="lock-wrapper">
                                                                            <span class="<%= hddsCadeado.Value == "N" ? "shackle shackle-open" : "shackle shackle-closed" %>"></span>
                                                                            <svg
                                                                                class="lock-body"
                                                                                width="15"
                                                                                height="15"
                                                                                viewBox="0 0 28 28"
                                                                                fill="none"
                                                                                xmlns="http://www.w3.org/2000/svg">
                                                                                <path
                                                                                    fill-rule="evenodd"
                                                                                    clip-rule="evenodd"
                                                                                    d="M0 5C0 2.23858 2.23858 0 5 0H23C25.7614 0 28 2.23858 28 5V23C28 25.7614 25.7614 28 23 28H5C2.23858 28 0 25.7614 0 23V5ZM16 13.2361C16.6137 12.6868 17 11.8885 17 11C17 9.34315 15.6569 8 14 8C12.3431 8 11 9.34315 11 11C11 11.8885 11.3863 12.6868 12 13.2361V18C12 19.1046 12.8954 20 14 20C15.1046 20 16 19.1046 16 18V13.2361Z"
                                                                                    fill="black">
                                                                                </path>
                                                                            </svg>
                                                                        </span>
                                                                    </label>
                                                                </div>
                                                            </div>

                                                        </div>
                                                    </div>

                                                    <div class="col-lg-4 table-responsive">
                                                        <asp:GridView ID="dtgvDadosFinanceiros" class="table table-striped table-bordered table-hover table-condensed"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                                            <Columns>
                                                                <asp:BoundField DataField="Descricao" HeaderText="">
                                                                    <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" Font-Bold="true" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="Data" HeaderText="Data">
                                                                    <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="Valor" HeaderText="Valor" DataFormatString="{0:C2}">
                                                                    <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>

                                                </div>
                                            </div>

                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>

                                <div role="tabpanel" class="tab-pane fade" id="titulos" aria-labelledby="titulos-tab">
                                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                        <ContentTemplate>
                                            <div class="panel panel-primary" runat="server" id="DIV18">
                                                <div class="panel-heading">
                                                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;Filtro para pesquisa</h3>
                                                </div>
                                                <div class="panel-body">
                                                    <div class="col-lg-12">
                                                        <div class="form-group row">

                                                            <div class="col-lg-2">
                                                                <div class="form-group">
                                                                    <label>Tipo Data Filtro</label>
                                                                    <asp:DropDownList ID="ddlFinanceiroFiltroData" runat="server" class="form-control">
                                                                        <asp:ListItem Text="Emissão" Value="E" Selected="True"></asp:ListItem>
                                                                        <asp:ListItem Text="Vencimento" Value="V"></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-1">
                                                                <div class="form-group">
                                                                    <label>Data Inicial</label>
                                                                    <asp:TextBox ID="txtdtInicial" class="form-control" runat="server" type="date"></asp:TextBox>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-1">
                                                                <div class="form-group">
                                                                    <label>Data Final</label>
                                                                    <asp:TextBox ID="txtdtFinal" class="form-control" runat="server" type="date"></asp:TextBox>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-2">
                                                                <div class="form-group">
                                                                    <label>Status</label>
                                                                    <asp:DropDownList ID="ddlFinanceiroStatus" runat="server" class="form-control">
                                                                        <asp:ListItem Text="Em Aberto" Value="Em Aberto" Selected="True"></asp:ListItem>
                                                                        <asp:ListItem Text="Em Atraso" Value="Em Atraso"></asp:ListItem>
                                                                        <asp:ListItem Text="Liquidado" Value="Liquidado"></asp:ListItem>
                                                                        <asp:ListItem Text="Todos os Status" Value=""></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-2">
                                                                <div class="form-group">
                                                                    <asp:Button runat="server" ID="btnBuscaFinanceiro" CssClass="btn btn-primary" OnClick="btnBuscaFinanceiro_Click" Text="Buscar" Style="margin-top: 24px;" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="panel panel-primary">
                                                <div class="panel-body">
                                                    <div class="col-lg-12 table-responsive">
                                                        <asp:GridView ID="dtgvFinanceiroTitulo" class="table table-striped table-bordered table-hover table-condensed"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="idContasReceber"
                                                            ShowFooter="true" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvFinanceiroTitulo_RowDataBound">
                                                            <Columns>
                                                                <asp:BoundField DataField="idContasReceber" HeaderText="ID">
                                                                    <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="dtEmissao" HeaderText="Data Emissão">
                                                                    <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="dtVencimento" HeaderText="Data Vencimento">
                                                                    <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="Pedido">
                                                                    <HeaderStyle Width="10%" />
                                                                    <ItemTemplate>
                                                                        <asp:HyperLink ID="hlsDocumento" runat="server" Text='<%# Eval("sDocumento") %>'></asp:HyperLink>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>

                                                                <asp:BoundField DataField="nParcela" HeaderText="Parcela">
                                                                    <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="Nota Fiscal">
                                                                    <HeaderStyle Width="10%" />
                                                                    <ItemTemplate>
                                                                        <asp:HyperLink ID="hlsCodigo" runat="server" Text='<%# Eval("sCodigo") %>'></asp:HyperLink>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>

                                                                <asp:BoundField DataField="nValorBruto" HeaderText="Valor Bruto" DataFormatString="{0:C2}">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="nValorOriginal" HeaderText="Valor Líquido" DataFormatString="{0:C2}">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="nTotal" HeaderText="Valor Recebido" DataFormatString="{0:C2}">
                                                                    <HeaderStyle Width="10%" />
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="nSaldo" HeaderText="Saldo Aberto" DataFormatString="{0:C2}">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sDscCategoriaReceber" HeaderText="Categoria">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sStatus" HeaderText="Status">
                                                                    <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="dtLiquidado" HeaderText="Data Liquidação">
                                                                    <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sDscEmpresa" HeaderText="Empresa">
                                                                    <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>

                                <div role="tabpanel" class="tab-pane fade" id="grafico" aria-labelledby="grafico-tab">
                                    <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                        <ContentTemplate>
                                            <div class="panel panel-default" runat="server" id="DIV19">
                                                <%--<div class="panel-heading">
                                                    <h3 class="panel-title"><b></b>Gráficos</h3>
                                                </div>--%>
                                                <div class="panel-body">
                                                    <div class="form-stacked row">

                                                        <div class="col-lg-12">
                                                            <div class="row">
                                                                <%--<uc1:MensagemPagina runat="server" ID="MensagemPaginaGrafico" />--%>
                                                                <h3 class="panel-title" style="text-align: end;" id="tituloAtualizacao">
                                                                    <b>Informações atualizadas
                                                                        <asp:Literal ID="litDataAtualizacao" runat="server"></asp:Literal></b>
                                                                </h3>
                                                                <asp:Panel runat="server" ID="pnMediaCompraMensal" Style="border: ridge">
                                                                    <div class="panel-heading" style="display: flex; justify-content: space-between; align-items: center; width: 100%;">
                                                                        <h3 class="panel-title" style="margin: 0;"><b>Média de Compras Mensais (últimos 12 meses)</b></h3>
                                                                    </div>
                                                                    <asp:Panel ID="pnCardMediaCompraMensal" runat="server" Style="display: flex; gap: 40px; flex-wrap: wrap; margin-left: 50px;">
                                                                        <asp:Repeater ID="rptCardsMediaCompraMensal" runat="server">
                                                                            <ItemTemplate>
                                                                                <div class="card <%# GetCardClass(Eval("Categoria").ToString()) %>" onclick="redirecionarCard('<%# GetCardUrl(Eval("Categoria").ToString()) %>')">
                                                                                    <div class="card-header"><%# GetCardHeaderText(Eval("Categoria").ToString()) %></div>
                                                                                    <div class="card-value"><%# Eval("mediaCompraMensal") %></div>
                                                                                    <div class="card-label">Quantidade</div>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                        </asp:Repeater>
                                                                    </asp:Panel>
                                                                    <br />
                                                                </asp:Panel>

                                                                <asp:Panel runat="server" ID="pnMediaPagamento" Style="border: ridge">
                                                                    <div class="panel-heading" style="display: flex; justify-content: space-between; align-items: center; width: 100%;">
                                                                        <h3 class="panel-title" style="margin: 0;"><b>Média de Pagamentos</b></h3>
                                                                    </div>
                                                                    <asp:Panel ID="pnCardMediaPagamento" runat="server" Style="display: flex; gap: 40px; flex-wrap: wrap; margin-left: 50px;">
                                                                        <asp:Repeater ID="rptCardsPagamentoTitulos" runat="server">
                                                                            <ItemTemplate>
                                                                                <div class="card <%# GetCardClass(Eval("Categoria").ToString()) %>" onclick="redirecionarCard('<%# GetCardUrl(Eval("Categoria").ToString()) %>')">
                                                                                    <div class="card-header"><%# GetCardHeaderText(Eval("Categoria").ToString()) %></div>
                                                                                    <div class="card-value"><%# Eval("Quantidade") %></div>
                                                                                    <div class="card-label">Quantidade</div>
                                                                                    <div class="card-value"><%# Eval("Valor_Medio", "{0:C}") %></div>
                                                                                    <div class="card-label">Valor Total</div>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                        </asp:Repeater>
                                                                    </asp:Panel>
                                                                    <br />
                                                                </asp:Panel>

                                                                <asp:Panel runat="server" ID="pnMedioTitulosAberto" Style="border: ridge">
                                                                    <div class="panel-heading" style="display: flex; justify-content: space-between; align-items: center; width: 100%;">
                                                                        <h3 class="panel-title" style="margin: 0;"><b>Média de Títulos em Aberto</b></h3>
                                                                    </div>
                                                                    <asp:Panel ID="pnCardMediaTitulosAberto" runat="server" Style="display: flex; gap: 40px; flex-wrap: wrap; margin-left: 50px;">
                                                                        <asp:Repeater ID="rptMediaTitulosAberto" runat="server">
                                                                            <ItemTemplate>
                                                                                <div class="card <%# GetCardClass(Eval("Categoria").ToString()) %>" onclick="redirecionarCard('<%# GetCardUrl(Eval("Categoria").ToString()) %>')">
                                                                                    <div class="card-header"><%# GetCardHeaderText(Eval("Categoria").ToString()) %></div>
                                                                                    <div class="card-value"><%# Eval("Quantidade") %></div>
                                                                                    <div class="card-label">Quantidade</div>
                                                                                    <div class="card-value"><%# Eval("Valor_Medio", "{0:C}") %></div>
                                                                                    <div class="card-label">Valor Total</div>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                        </asp:Repeater>
                                                                    </asp:Panel>
                                                                    <br />
                                                                </asp:Panel>
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
                    </div>


                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <style>
            /* Estilos para a Grid de Histórico */
            .gvHeader th {
                padding: 8px;
                background-color: #1A4C1A; /* Verde escuro igual ao seu padrão */
                color: #fff;
                border: 1px solid #bbb;
                font-weight: bold;
                text-align: center;
            }

            .gvRow td {
                padding: 8px;
                background-color: #ffffff;
                border: 1px solid #bbb;
                vertical-align: middle;
            }

            .gvAltRow td {
                padding: 8px;
                background-color: #f9f9f9;
                border: 1px solid #bbb;
                vertical-align: middle;
            }

            /* Hover para dar feedback visual */
            .gvRow:hover td, .gvAltRow:hover td {
                background-color: #efefef !important;
                cursor: default;
            }

            .gvChildHeader th {
                padding: 5px;
                background-color: #666;
                color: #fff;
                border: 1px solid #bbb;
            }

            /* Importante para o efeito de expandir */
            .collapsed-row {
                display: none;
                background-color: #fff;
            }
        </style>

        <div role="tabpanel" class="tab-pane fade" id="historico" aria-labelledby="historico-tab">
            <asp:UpdatePanel ID="updHistorico" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="panel panel-default">
                        <div class="panel-body">
                            <uc1:MensagemPagina runat="server" ID="msgHistorico" />
                            <div class="table-responsive">
                                <asp:GridView ID="gvHistorico" runat="server" class="table table-striped table-bordered table-hover table-condensed"
                                    Width="100%" GridLines="None" AutoGenerateColumns="False"
                                    HeaderStyle-CssClass="gvHeader" CssClass="gvRow" AlternatingRowStyle-CssClass="gvAltRow"
                                    DataKeyNames="idHistorico" OnRowDataBound="gvHistorico_RowDataBound">
                                    <Columns>
                                        <asp:TemplateField ItemStyle-Width="2%">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnToggle" runat="server" CssClass="toggle-icon" data-div-id='<%# "trHist" + Eval("idHistorico") %>'>
                                                    <i class="fa fa-plus-square-o" style="cursor: pointer; font-size: 16px; color: #1A4C1A;"></i>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Ação/Alterações" ItemStyle-Width="50%">
                                            <ItemTemplate>
                                                <asp:Literal ID="litAcao" runat="server" Text='<%# Eval("sDscAcao") %>'></asp:Literal>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="dtAtualizacao" HeaderText="Data/Hora" ItemStyle-Width="15%" />
                                        <asp:BoundField DataField="sUsuario" HeaderText="Responsável" ItemStyle-Width="15%" />

                                        <asp:TemplateField HeaderText="Detalhes de Itens" Visible="true">
                                            <ItemTemplate>
                                                <%-- Esta função NovaLinha é essencial para o efeito de expansão --%>
                                                <%# NovaLinha(Eval("idHistorico"), "Hist") %>

                                                <asp:GridView ID="gvHistoricoItens" runat="server" Width="100%"
                                                    GridLines="None" AutoGenerateColumns="false"
                                                    HeaderStyle-CssClass="gvChildHeader" CssClass="gvRow"
                                                    AlternatingRowStyle-CssClass="gvAltRow">
                                                    <Columns>
                                                        <asp:BoundField DataField="sTipoEntidade" HeaderText="Tipo" ItemStyle-Width="20%" />
                                                        <asp:TemplateField HeaderText="Alteração Detalhada">
                                                            <ItemTemplate>
                                                                <asp:Literal ID="litAcaoItem" runat="server" Text='<%# Eval("sDscAcao") %>'></asp:Literal>
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
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div id="dialog_Aceitar" class="modal" title="Status Edição">
            <p>
                <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                <asp:Label ID="lblMensagemDialogCadeado" runat="server" ClientIDMode="Static" Text=""></asp:Label>
            </p>
        </div>

        <asp:HiddenField ID="hddsCadeado" runat="server" Value="N" />
        <asp:HiddenField ID="hddsPermissaoCadeado" runat="server" Value="0" />

        <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

        <fieldset class="form-stacked actions">
            <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
            &nbsp;
            <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" id="field-cancel" title="Voltar" onclick="history.go(-1)">
        </fieldset>

    </div>

    <script>
        function redirecionarCard(url) {
            if (url && url !== '#') {
                window.open(url, '_blank');
            }
        }

        document.addEventListener('DOMContentLoaded', function () {
            document.body.addEventListener('change', function (event) {
                if (event.target && event.target.id === 'lock') {
                    var isChecked = event.target.checked;

                    var mensagem;
                    if (isChecked) {
                        mensagem = "Deseja habilitar a edição?"
                    } else {
                        mensagem = "Deseja desabilitar a edição?"
                    }

                    alterarEdicao(mensagem, event);                    
                }
            });

        });

        window.addEventListener('load', function () {

            var permissao = document.getElementById('<%= hddsPermissaoCadeado.ClientID %>').value;
            var lockCheckbox = document.getElementById('lock');
            var hddidParceiro = document.getElementById('<%= hddidParceiro.ClientID %>').value;

            if (lockCheckbox) {

                var hddsCadeado = document.getElementById('<%= hddsCadeado.ClientID %>').value;
                var isChecked = (hddsCadeado === "N");
                var txtLimite = document.getElementById('<%= txtnLimiteCredito.ClientID %>');
                var ddlSituaacao = document.getElementById('<%= ddlidTipoSituacaoCliente.ClientID %>');                

                if (hddidParceiro === '0') {
                    isChecked = true;

                }

                if (isChecked) {
                    lockCheckbox.checked = isChecked;
                    txtLimite.disabled = false;
                    ddlSituaacao.disabled = false;                    
                }
                else {                   
                    txtLimite.disabled = true;
                    ddlSituaacao.disabled = true;
                    lockCheckbox.checked = isChecked;
                    var shackle = document.querySelector('.shackle');
                    shackle.style.transform = 'rotateY(0deg)';

                }
            }

        });

        $v192(function () {
            $v192("#dialog_Aceitar").dialog({
                resizable: false,
                height: "auto",
                width: 400,
                modal: true,
                autoOpen: false
            });
        });

        function alterarEdicao(mensagem, event) {
            document.getElementById('lblMensagemDialogCadeado').innerText = mensagem;
            
            var txtLimite = document.getElementById('<%= txtnLimiteCredito.ClientID %>');
            var ddlSituaacao = document.getElementById('<%= ddlidTipoSituacaoCliente.ClientID %>');
            var hddidParceiro = document.getElementById('<%= hddidParceiro.ClientID %>');
            var lockCheckbox = document.getElementById('lock');
            var isChecked = event.target.checked;

            $v192('#dialog_Aceitar').dialog('option', 'buttons', {
                "Sim": function () {
                    var idParceiro = hddidParceiro.value;

                    $.ajax({
                        url: '/app/Paginas/Manutencao/Parceiros_Detalhe.aspx/SalvaEstadoCadeado',
                        data: JSON.stringify({
                            isLocked: isChecked,
                            idParceiro: idParceiro
                        }),
                        contentType: 'application/json; charset=utf-8',
                        type: 'POST',
                        dataType: 'json',
                        success: function (data) {
                            var shackle = document.querySelector('.shackle');
                            if (isChecked) {                                
                                txtLimite.disabled = false;
                                ddlSituaacao.disabled = false;
                                shackle.style.transform = 'rotateY(150deg) translateX(3px)';
                                shackle.style.transformOrigin = 'right';
                            } else {                                
                                txtLimite.disabled = true;
                                ddlSituaacao.disabled = true;
                                shackle.style.transform = 'rotateY(0deg)';
                            }
                            //window.location.href = location.href;
                            $('#financeiro-tab').tab('show');
                        },
                        error: function (response) {
                            alert(response.responseText);
                        },
                        failure: function (response) {
                            alert(response.responseText);
                        }
                    });

                    $v192(this).dialog("close");
                },
                "Não": function () {
                    lockCheckbox.checked = !isChecked;

                    var shackle = document.querySelector('.shackle');
                    if (!isChecked) {                        
                        txtLimite.disabled = false;
                        ddlSituaacao.disabled = false;
                        shackle.style.transform = 'rotateY(150deg) translateX(3px)';
                        shackle.style.transformOrigin = 'right';
                    } else {                        
                        txtLimite.disabled = true;
                        ddlSituaacao.disabled = true;
                        shackle.style.transform = 'rotateY(0deg)';
                    }

                    $('#financeiro-tab').tab('show');

                    $v192(this).dialog("close");
                }
            });

            $v192('#dialog_Aceitar').dialog('open');
            
        }
    </script>

    <script type="text/javascript">
        function RegistrarColapsoHistorico() {
            $(document).ready(function () {
                // Remove eventos antigos para não duplicar
                $('.toggle-icon').off('click').on('click', function () {
                    var icon = $(this).find('i');
                    var divId = $(this).data('div-id');
                    var targetRow = $('#' + divId);

                    if (targetRow.is(':visible')) {
                        targetRow.hide();
                        icon.removeClass('fa-minus-square-o').addClass('fa-plus-square-o');
                    } else {
                        targetRow.show();
                        icon.removeClass('fa-plus-square-o').addClass('fa-minus-square-o');
                    }
                    return false; // Impede postback do LinkButton
                });
            });
        }

        // Registra para carregamento normal e para UpdatePanel
        $(document).ready(function () {
            RegistrarColapsoHistorico();
        });

        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function () {
            RegistrarColapsoHistorico();
        });
</script>

</asp:Content>
