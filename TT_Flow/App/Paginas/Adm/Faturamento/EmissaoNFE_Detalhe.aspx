<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="EmissaoNFE_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Faturamento.EmissaoNFE_Detalhe" Async="true" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/FiltroPesquisa.ascx" TagPrefix="uc1" TagName="FiltroPesquisa" %>
<%@ Register Src="~/App/Controles/DropDownList_Padrao.ascx" TagPrefix="uc1" TagName="DropDownList_Padrao" %>
<%@ Register Src="~/App/Controles/TextBox_Padrao.ascx" TagPrefix="uc1" TagName="TextBox_Padrao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        td .chosen-container-single {
            max-width: 400px !important;
        }

        td.Amarelo .chosen-single {
            background-color: #fff3cd !important;
        }

        .modal-title {
            color: #009A22;
            text-shadow: 2px 2px 2px rgba(0, 0, 0, 0.2);
            font-size: 24px;
            font-weight: bold;
            margin: 0;
        }

        .modal-Titulo {
            color: white;
            text-shadow: 2px 2px 2px rgba(0, 0, 0, 0.2);
            font-size: 20px;
            font-weight: bold;
            margin: 0;
            background: #d9534f;
            padding: 10px;
            border-radius: 6px;
            margin: -10px;
        }

        .modal-largo {
            width: 80% !important;
            max-width: none !important;
        }


        .modal-Medio {
            width: 60% !important;
            max-width: none !important;
        }

        .modal-Pequeno {
            width: 35% !important;
            max-width: none !important;
        }

        .modal-logo img {
            max-width: 50px;
            margin-right: 10px;
        }

        .modal-header-content {
            display: flex;
            align-items: center;
        }

        .modal-footer {
            text-align: left !important;
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

        .modal, 
        .modal-dialog, 
        .modal-content, 
        .modal-body,
        .panel-body,
        .form-stacked {
            overflow: visible !important;
        }

        /* Garante que o container do Chosen não crie barras de rolagem internas */
        .chosen-container {
            overflow: visible !important;
        }

        /* Mantém a lista suspensa por cima de qualquer elemento inferior do modal */
        .chosen-container .chosen-drop {
            z-index: 99999 !important;
            display: block;
        }

    </style>

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Emissão NF-e"></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>
    </div>

    <div>
        <ul id="tab_EmissorNFe" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="tabpanel active">
                <a href="#ide" id="ide-tab" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Detalhe</b></a>
            </li>

            <li role="presentation" runat="server" id="aba_Configuracoes">
                <a href="#Configuracoes" role="tab" id="Configuracoes-tab" data-toggle="tab" aria-controls="Configuracoes"><b>Configurações</b></a>
            </li>

            <li role="presentation" runat="server" id="aba_NFe">
                <a href="#NFe" role="tab" id="NFe-tab" data-toggle="tab" aria-controls="Documentos"><b>Documentos Fiscais</b></a>
            </li>

        </ul>
    </div>

    <div id="tab" class="tab-content">

        <div role="tabpanel" class="tab-pane fade in active" id="ide" aria-labelledby="ide-tab">
            <br />
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="panel panel-default" runat="server" id="DIV_DADOS">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Detalhe</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">

                                <div class="col-lg-12" runat="server" id="div_ide">
                                    <div class="row">
                                        <div class="col-lg-1">
                                            <div class="form-group">
                                                <label>
                                                    <asp:Label ID="DADOS_lblData" runat="server" Text="Data"></asp:Label>
                                                </label>
                                                <asp:TextBox ID="txtdtPedido" class="form-control " runat="server" MaxLength="10" ReadOnly="true">
                                                </asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-4 form-group" runat="server" id="Div_ddlEmpresa">
                                            <uc1:DropDownList_Padrao runat="server" ID="ddlidEmpresa" />
                                        </div>
                                 
                                        <div class="col-lg-2" runat="server" id="DIV_Referencia">
                                            <div class="form-group">
                                                <label>Referência</label>
                                                <asp:TextBox ID="txtsReferencia" class="form-control uppercase" runat="server" MaxLength="50"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="DIV_ControleTT">
                                            <div class="form-group">
                                                <uc1:TextBox_Padrao ID="txtsControleTT" runat="server" MaxLength="50" Titulo="Nº Controle TT"/>
                                            </div>
                                        </div>


                                        <div class="col-lg-3" id="div9" runat="server">
                                            <div class="form-group">
                                                <label>Status</label>
                                                <asp:TextBox ID="txtsStatus" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" id="DIV18" runat="server">
                                            <div class="form-group">
                                                <label>Tipo</label>
                                                <asp:DropDownList ID="ddlsTipo" runat="server" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlsTipo_SelectedIndexChanged">
                                                    <asp:ListItem Value="">Selecione o Tipo</asp:ListItem>
                                                    <asp:ListItem Value="0">Entrada</asp:ListItem>
                                                    <asp:ListItem Value="1">Saída</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>



                                        <div class="col-lg-5" id="DIV4" runat="server">
                                            <div class="form-group">
                                                <label>CFOP</label>
                                                <asp:DropDownList ID="ddlidCFOP" runat="server" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-5" id="DIV_NaturezaOperacao" runat="server">
                                            <div class="form-group">
                                                <label>Natureza da Operação</label>
                                                <asp:TextBox ID="txtsNaturezadaOperacao" class="form-control" runat="server" MaxLength="60"></asp:TextBox>
                                            </div>
                                        </div>



                                    </div>
                                </div>

																						   
													 
										  

										  


                                <div class="col-lg-12">
                                    <div class="row">
                                        <div class="col-lg-6" id="DIV_Cliente" runat="server">
                                            <div class="form-group">
                                                <label>Destinatário</label>
                                                <asp:DropDownList ID="ddlCliente" runat="server" class="form-control Caixa_Selecao" AutoPostBack="True" OnSelectedIndexChanged="ddlCliente_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-6" runat="server" id="DIV_EnderecoEntrega">
                                            <div class="form-group">
                                                <label>Endereço de Entrega</label>
                                                <asp:DropDownList ID="ddlsEnderecoEntrega" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="row">

                                        <div class="col-lg-2" runat="server" id="div_txtIE">
                                            <div class="form-group">
                                                <label>IE</label>
                                                <asp:TextBox ID="txtIE" runat="server" class="form-control" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="div_txtContribuinteICMS">
                                            <div class="form-group">
                                                <label>Contribuinte do ICMS?</label>
                                                <asp:TextBox ID="txtContribuinteICMS" runat="server" class="form-control" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>Tipo de Emissão</label>
                                                <asp:DropDownList ID="ddlsDestinoVenda" class="form-control Caixa_Selecao" runat="server" MaxLength="10">
                                                    <asp:ListItem Value="C" Text="Consumo" />
                                                    <asp:ListItem Value="R" Text="Revenda" Selected="True" />
                                                    <asp:ListItem Value="I" Text="Industrialização" />
                                                </asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" id="div_nVlrFrete" runat="server">
                                            <div class="form-group">
                                                <label>Valor Frete</label>
                                                <div class="input-group">
                                                    <asp:TextBox ID="txtnFrete" class="form-control" runat="server" MaxLength="10" AutoPostBack="True" OnTextChanged="txtnFrete_TextChanged" ToolTip="Valor do Frete"></asp:TextBox>
                                                    <span class="input-group-addon" style="font-weight: bold;">R$</i></span>
                                               </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" id="div2" runat="server">
                                            <div class="form-group">
                                                <label>Valor Produtos</label>
                                                <div class="input-group">
                                                    <asp:TextBox ID="txtnVlrProdutos" class="form-control" runat="server" MaxLength="10" ReadOnly="true" ToolTip="Valor Produtos"></asp:TextBox>
                                                    <span class="input-group-addon" style="font-weight: bold;">R$</i></span>
                                               </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" id="div3" runat="server">
                                            <div class="form-group">
                                                <label>Valor Total</label>
                                                <div class="input-group">
                                                    <asp:TextBox ID="txtnVlrTotal" class="form-control negrito" runat="server" MaxLength="10" ReadOnly="true" ToolTip="Valor Total"></asp:TextBox>
                                                    <span class="input-group-addon" style="font-weight: bold;">R$</i></span>
                                               </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="row">

                                        <div class="col-lg-2 form-group">
                                            <uc1:TextBox_Padrao runat="server" ID="txtdtEntrada_Saida" Titulo="Data Entrada/Saída" Modo="DataHora" />
                                        </div>

                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>NFe Ref</label>
                                                <asp:DropDownList ID="ddlsChaveNFe_Referencia" runat="server" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlsChaveNFe_Referencia_SelectedIndexChanged">
                                                    <asp:ListItem Value="N">Não</asp:ListItem>
                                                    <asp:ListItem Value="S">Sim</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-4" id="Div_sChaveNFe_Referencia" runat="server">
                                            <div class="form-group">
                                                <label>Chave NFe Referenciada</label>
                                                <asp:TextBox ID="txtsChaveNFe_Referencia" class="form-control" runat="server" MaxLength="44" ToolTip="Informe a ChaveNFe da NFe referenciada"></asp:TextBox>
                                            </div>
                                        </div>


                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="panel panel-default" runat="server" id="DIV5">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Transporte</b></h3>
                        </div>
                        <div class="panel-body row">
                            <div class="form-stacked">
                                <div class="col-lg-12" runat="server" id="div6">
                                    <div class="row">
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <label>Modo Frete</label>
                                                <asp:DropDownList ID="ddltranporte" runat="server" class="form-control yes_no select Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddltranporte_SelectedIndexChanged">
                                                    <asp:ListItem Value="">Selecione o Modo Frete</asp:ListItem>
                                                    <asp:ListItem Value="0">0 - Contrat. conta do Remetente (CIF)</asp:ListItem>
                                                    <asp:ListItem Value="1">1 - Contrat. conta do Destinatário (FOB)</asp:ListItem>
                                                    <asp:ListItem Value="2">2 - Contrat. conta de Terceiros</asp:ListItem>
                                                    <asp:ListItem Value="3">3 - Transp. próp. conta de Remetente</asp:ListItem>
                                                    <asp:ListItem Value="4">4 - Transp. próp. conta de Destinatário</asp:ListItem>
                                                    <asp:ListItem Value="9">9 - Sem ocorrência de transporte</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-6" runat="server" id="DIV_Transportadora">
                                            <div class="form-group">
                                                <label>Transportadora</label>
                                                <asp:DropDownList ID="ddlidTranportadora" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="DIV_Volumes">
                                            <div class="form-group">
                                                <label>Qtd Volumes</label>
                                                <asp:TextBox ID="txtVolumes" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="DIV_Especie">
                                            <div class="form-group">
                                                <label>Espécie</label>
                                                <asp:TextBox ID="txtEspecie" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="DIV_Liquido">
                                            <div class="form-group">
                                                <label>Peso Líquido</label>
                                                <asp:TextBox ID="txtPesoLiquido" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="DIV_Bruto">
                                            <div class="form-group">
                                                <label>Peso Bruto</label>
                                                <asp:TextBox ID="txtPesoBruto" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="panel panel-default" runat="server" id="DIV7">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Informações NF-e</b></h3>
                        </div>
                        <div class="panel-body row">
                            <div class="form-stacked">
                                <div class="col-lg-12" runat="server" id="div8">
                                    <div class="row">
                                        <div class="col-lg-12">
                                            <div class="form-group">
                                                <label>Informações Adicionais ao Fisco</label>
                                                <asp:TextBox ID="txtInfoFisco" class="form-control" runat="server" TextMode="MultiLine" Height="50px"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-12">
                                            <div class="form-group">
                                                <label>Informações Complementares</label>
                                                <asp:TextBox ID="txtInfoComplementares" class="form-control" runat="server" TextMode="MultiLine" Height="50px"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="panel panel-default" runat="server" id="DIV1">
                        <div class="panel-heading">
                            <div class="row">
                                <div class="col-lg-11">
                                    <h3 class="panel-title"><b>Itens</b></h3>
                                </div>
                                <div class="col-lg-1 padd-0" runat="server" id="div_Ordem">
                                    <div class="">
                                        <asp:Button ID="cmdImportarItens" class="btn btn-sm btn-warning" runat="server" Text="Importar Itens" ValidationGroup="DETALHE" OnClick="cmdImportarItens_Click" />
                                    <asp:DropDownList ID="BaseddlsUnidade" Visible="false" runat="server" class="form-control yes_no select"></asp:DropDownList>
           
                                        </div>
                                </div>
                            </div>
                        </div>
                        <div class="panel-body row">
                            <div class="col-lg-12">
                                <uc1:MensagemPagina runat="server" ID="MensagemPagina_Itens" />
                            </div>
                            <div class="form-stacked">
                                <div class="col-lg-11" runat="server" id="DIV_FiltroPesquisaProdutos">
                                    <uc1:FiltroPesquisa runat="server" ID="FiltroPesquisaProdutos" />
                                </div>
                                <br />
                                <div class="col-lg-1" runat="server" id="DIV_IncluirItem">
                                    <label>&nbsp;</label>
					                <asp:Button ID="Item_cmdIncluirItem" class="btn btn-info form-control" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="Item_cmdIncluirItem_Click" />
            
                                </div>
                    
                                <div class="col-lg-12">
                                    <asp:GridView ID="gvItens" class="table table-striped table-bordered table-hover table-responsive"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" DataKeyNames="IdItem, nOrdem" Font-Size="Small" OnRowDeleting="gvItens_RowDeleting" OnRowCommand="gvItens_RowCommand"
                                        OnRowDataBound="gvItens_RowDataBound">
                                        <Columns>
                                            <asp:BoundField DataField="nOrdem" HeaderText="Ordem">
                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="idItem" HeaderText="idItem">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="idProduto" HeaderText="ID">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sCodigoProduto" HeaderText="Código">
                                                <ItemStyle Width="16%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscProduto" HeaderText="Descrição">
                                                <ItemStyle Width="37%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="UN">
                                                <ItemTemplate>
                                                    <asp:DropDownList ID="ddlsUnidade" runat="server" class="form-control yes_no select"></asp:DropDownList>
                                                </ItemTemplate>
                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="sUnidade" HeaderText="UN">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Qtd">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtnQuantidade" runat="server" OnTextChanged="txtnValorUnitario_TextChanged" class="form-control" AutoPostBack="true" MaxLength="20" Text='<%# Bind("nQuantidade", "{0:N4}") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                            </asp:TemplateField>


                                            <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade" DataFormatString="{0:N4}">
                                                <ItemStyle Width="10%" HorizontalAlign="Right" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Unitário">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtnValorUnitario" runat="server" OnTextChanged="txtnValorUnitario_TextChanged" class="form-control" AutoPostBack="true" MaxLength="20" Text='<%# Bind("nValorUnitario", "{0:N4}") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="nValorUnitario" HeaderText="Unitário" DataFormatString="{0:N4}">
                                                <ItemStyle Width="10%" HorizontalAlign="Right" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>


                                            <asp:BoundField DataField="nValorTotal" HeaderText="Valor Total" DataFormatString="{0:N2}">
                                                <ItemStyle Width="10%" HorizontalAlign="Right" Font-Bold="true" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkReducaoBC" runat="server" CommandName="RedBC" CommandArgument='<%# Eval("nOrdem") %>' ToolTip="Redução Base de Calculo" CssClass="btn btn-small"><i class='fa-plus-square fa'></i></asp:LinkButton>
                                                    <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" ToolTip="Excluir Item" CssClass="btn btn-small"><i class='fa-eraser fa'></i></asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"  />
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-12">
                            <div class="form-group">
                                <label>Observações</label>
                                <asp:TextBox ID="txtsObservacao" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="150px"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

                    <asp:HiddenField ID="hddidImportarItem" runat="server" Value="0" />
                    <asp:HiddenField ID="hddImportarTipo" runat="server" Value="" />

                </ContentTemplate>
            </asp:UpdatePanel>

            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdEditar" class="btn  btn-lg btn-info" runat="server" Text="Editar" ValidationGroup="DETALHE" OnClick="cmdEditar_Click" />
                <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" ValidationGroup="DETALHE" OnClick="cmdSalvar_Click" />
                <asp:Button ID="cmdGerarPiloto" class="btn  btn-lg btn-danger" runat="server" Text="Gerar Espelho" ValidationGroup="DETALHE" OnClick="cmdGerarPiloto_Click" />
                <a href="javascript:history.back()" class="btn btn-lg btn-warning" title="Voltar">Voltar</a>
            </fieldset>

        </div>

        <div role="tabpanel" class="tab-pane fade" id="NFe" aria-labelledby="NFe-tab">
            <asp:UpdatePanel ID="updPanel_DocumentoFiscais" runat="server">
                <ContentTemplate>
                    <br />
                    <div class="panel panel-default" runat="server" id="DIV19">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Documentos Fiscais</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_DocumentosFiscais" />
                                </div>
                                <div class="col-lg-12 table-responsive">
                                    <asp:GridView ID="gvDocumentosFiscais" class="table table-striped table-bordered table-hover "
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="chNFe, idObjeto, sXML_Autorizado, idXML"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvDocumentosFiscais_RowDataBound">
                                        <Columns>


                                            <asp:BoundField DataField="idXML" HeaderText="ID">
                                                <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="idObjeto" HeaderText="ID Pedido">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sdtEvento_Formatada" HeaderText="Data ">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sTipo" HeaderText="Tipo">
                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nSerieNF" HeaderText="Série">
                                                <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nNumeroNF" HeaderText="Número">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="chNFe" HeaderText="Chave NFe">
                                                <ItemStyle Width="16%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sStatus" HeaderText="Status">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="xMotivo" HeaderText="Observação">
                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Ações">
                                                <ItemTemplate>
                                                    <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdGerarNFe" class="btn btn-sm btn-success" Text="Enviar Sefaz" OnClick="DocumentosFiscais_cmdGerarNFe_Click"></asp:LinkButton>
                                                    <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdDownloadXML" class="btn btn-sm btn-primary" Text="XML" OnClick="DocumentosFiscais_cmdDownloadXML_Click"></asp:LinkButton>
                                                    <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdDownloadDANFE" class="btn btn-sm btn-warning" Text="DANFE" OnClick="DocumentosFiscais_cmdDownloadDANFE_Click"></asp:LinkButton>
                                                    <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdCartaCorrecao" class="btn btn-sm btn-success" ToolTip="Carta de Correção" Text="CC-e" OnClick="DocumentosFiscais_cmdCartaCorrecao_Click"></asp:LinkButton>
                                                    <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdDownloadCCeXML" class="btn btn-sm btn-primary" Text="XML" OnClick="DocumentosFiscais_cmdDownloadCCeXML_Click"></asp:LinkButton>
                                                    <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdDownloadCCePDF" class="btn btn-sm btn-danger" Text="PDF" OnClick="DocumentosFiscais_cmdDownloadCCePDF_Click"></asp:LinkButton>
                                                    <asp:LinkButton runat="server" ID="DocumentosFiscais_cmdCancelarNFE" class="btn btn-sm btn-danger" ToolTip="Cancelar NF-e" Text="Cancelar NF-e" OnClick="DocumentosFiscais_cmdCancelarNFE_Click"></asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="nNumeroCartaCorrecao" HeaderText="Qtd Cartas" />
                                            <asp:BoundField DataField="sCancelamento" HeaderText="Tem Cancelamento" />
                                            <asp:BoundField DataField="sChaveNFE" HeaderText="ChaveNFe" />



                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>


        <div class="modal fade" id="Modal_AcoesNFe" tabindex="-1" data-backdrop="static" role="dialog">
            <div class="modal-dialog modal-dialog-centered" role="document" id="div_Modal" runat="server">
                <div class="modal-content">
                    <asp:UpdatePanel ID="updAcoesNFe" runat="server">
                        <ContentTemplate>
                            <div class="modal-header">
                                <button type="button" id="cmdFecharModalAcoesNFe" class="close" data-dismiss="modal" aria-label="Fechar">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                                <div class="modal-header-content">
                                    <div class="modal-logo">
                                        <asp:Image ID="Image1" runat="server" ImageUrl="~/App/img/LogoTT.png" AlternateText="LogoTipo" />
                                    </div>
                                    <div class="modal-title-container">
                                        <asp:Label runat="server" class="modal-title" ID="lblTitulos_AcoesNFe" Text="" Font-Bold="true"></asp:Label>
                                    </div>
                                </div>
                            </div>

                            <div class="modal-body">
                                <div class="panel-body">
                                    <div class="form-stacked row">
                                        <div class="col-lg-12">
                                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaAcoesNFe" />
                                        </div>

                                        <div class="col-lg-12">
                                            <div class="form-group row">
                                                <label>
                                                    <asp:Label runat="server" ID="lblTituloJustificativa_AcoesNFe" Text="Informe uma justificativa"></asp:Label></label>
                                                <asp:TextBox ID="txtsJustificativa_AcoesNFe" class="form-control" runat="server" MaxLength="255" TextMode="MultiLine" Height="95px"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <asp:HiddenField ID="hddsAcaoNFe" runat="server" />
                            <asp:HiddenField ID="hddidXML" runat="server" />

                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div class="modal-footer text-left">
                        <asp:LinkButton ID="cmdConfirmar_AcoesNFe" CssClass="btn btn-success" Text="OK" OnClick="cmdConfirmar_AcoesNFe_Click" runat="server" />
                        <asp:LinkButton CssClass="btn btn-danger" Text="Cancelar" runat="server" data-dismiss="modal" />
                    </div>
                </div>
            </div>
        </div>

        <div class="modal fade" id="Modal_ImportarItens" tabindex="-999" data-backdrop="static" role="dialog">
            <div class="modal-dialog modal-sm modal-dialog-centered modal-largo" role="document">
                <div class="modal-content">
                    <asp:UpdatePanel ID="UpdateImportarItens" runat="server">
                        <ContentTemplate>

                            <div class="modal-header">
                                <button type="button" id="cmdFecharModal" class="close" data-dismiss="modal" aria-label="Fechar">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                                <div class="modal-header-content">
                                    <div class="modal-logo">
                                        <asp:Image ID="imgLogo" runat="server" ImageUrl="~/App/img/LogoTT.png" AlternateText="LogoTipo" />
                                    </div>
                                    <div class="modal-title-container">
                                        <asp:Label runat="server" class="modal-title" ID="lblTituloImportItens" Text="Importar Itens" Font-Bold="true"></asp:Label>
                                    </div>
                                </div>
                            </div>

                            <div class="modal-body">
                                <div class="panel-body">
                                    <div class="form-stacked row">
                                        <div class="col-lg-12 row">
                                            <uc1:MensagemPagina runat="server" ID="MensagemPagina_ImportacaoItens" />
                                        </div>

                                        <div class="col-lg-12">
                                            <div class="form-group">
                                                <div id="tiposView" class="row">
                                                    <div runat="server" id="div_TipoImportacao" class="col-lg-12 btn-group tiposView">

                                                        <asp:RadioButtonList ID="rb_Resposta" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="cbTipoImportacao_CheckedChanged" AutoPostBack="true">
                                                            <asp:ListItem class="btn btn-primary" Value="PEDIDO">&nbsp;Pedido &nbsp;&nbsp;</asp:ListItem>
                                                            <asp:ListItem class="btn btn-primary" Value="OPI">&nbsp;OPI&nbsp;&nbsp;</asp:ListItem>
                                                            <asp:ListItem class="btn btn-primary" Value="LME">&nbsp;LME&nbsp;&nbsp;</asp:ListItem>
                                                            <asp:ListItem class="btn btn-primary" Value="LMCO">&nbsp;LMCO&nbsp;&nbsp;</asp:ListItem>
                                                        </asp:RadioButtonList>

                                                    </div>
                                                </div>

                                            </div>
                                        </div>
                                        <asp:Panel ID="pnResultado" class="" runat="server">
                                            <div class="col-lg-12">


                                                <div class="panel panel-primary">
                                                    <div class="panel-body">

                                                        <div class=" row form-group">

                                                            <div class="col-lg-12 table-responsive">
                                                                <asp:GridView ID="gvImportacaoItens" class="table table-striped table-bordered table-hover "
                                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="id, sTipo"
                                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowCommand="gvImportacaoItens_RowCommand" OnRowDataBound="gvImportacaoItens_RowDataBound">
                                                                    <Columns>
                                                                        <asp:ButtonField DataTextField="id" HeaderText="ID" ItemStyle-CssClass="Importacao" CommandName="Importacao" ItemStyle-Width="2%" />
                                                                        <asp:ButtonField DataTextField="idObjeto" HeaderText="ID" ItemStyle-CssClass="Importacao" CommandName="Importacao" ItemStyle-Width="6%" />
                                                                        <asp:ButtonField DataTextField="sTipo" HeaderText="Tipo" ItemStyle-CssClass="Importacao" CommandName="Importacao" ItemStyle-Width="8%" />
                                                                        <asp:ButtonField DataTextField="sDscCliente" HeaderText="Cliente" ItemStyle-CssClass="Importacao" CommandName="Importacao" ItemStyle-Width="20%" />
                                                                        <asp:ButtonField DataTextField="sControleTT" HeaderText="Controle TT" ItemStyle-CssClass="Importacao" CommandName="Importacao" ItemStyle-Width="10%" />
                                                                        <asp:ButtonField DataTextField="sReferencia" HeaderText="Referência" ItemStyle-CssClass="Importacao" CommandName="Importacao" ItemStyle-Width="20%" />
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </div>

                                                    </div>
                                                </div>


                                            </div>
                                        </asp:Panel>
                                    </div>
                                </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div class="modal-footer text-left">
                    </div>
                </div>
            </div>
        </div>

    </div>


    <div class="modal fade" id="Modal_ReducaoBC" tabindex="-999" data-backdrop="static" role="dialog">
        <div class="modal-dialog modal-lg modal-Comprido" role="document" id="div10"  runat="server">
            <div class="modal-content" >
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" id="cmdFecharModal_ReducaoBC" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <div class="modal-header-content">
                                <div class="modal-logo">
                                    <asp:Image ID="ImgLogo_ReducaoBC" runat="server" ImageUrl="~/App/img/LogoTT.png" AlternateText="LogoTipo" />
                                </div>
                                <div class="modal-title-container">
                                    <asp:Label runat="server" class="modal-title" ID="lblReducaoBC_Titulo" Text="Redução Base de Calculo" Font-Bold="true"></asp:Label>
                                </div>
                            </div>
                        </div>

                        <div class="modal-body" style="overflow: visible !important;">
                            <div class="panel-body">
                                <div class="form-stacked row">
                                    <div class="col-lg-12">
                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina1" />
                                    </div>

                                    <div class="col-lg-12">
                                        <div class="form-group row">
                                            <div class="col-lg-2" >
                                                <div class="form-group">
                                                    <label>Código</label>
                                                    <asp:TextBox ID="txtReducaoBC_sCodigoProduto" class="form-control" ReadOnly="true" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                             <div class="col-lg-10" >
                                                 <div class="form-group">
                                                     <label>Produto</label>
                                                     <asp:TextBox ID="txtReducaoBC_sDscProduto" class="form-control" ReadOnly="true" runat="server"></asp:TextBox>
                                                 </div>
                                             </div>
                                        </div>
                                    </div>

                                    
                                    <div class="col-lg-12 row">
                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label>CST ICMS</label>
                                                <asp:DropDownList ID="ddlReducaoBC_idCST_ICMS" runat="server" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlReducaoBC_idCSTICMS_SelectedIndexChanged" > </asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-4" id="div_ReducaoBC_idCST_cBenef" runat="server">
                                            <div class="form-group">
                                                <label>Código Benefício</label>
                                                <asp:DropDownList ID="ddlReducaoBC_idCST_cBenef" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                            </div>
                                        </div>

                                         <div class="col-lg-4">
                                             <div class="input-group">
                                                <label>Redução BC</label>
                                                <div class="input-group">
                                                    <asp:TextBox ID="txtReducaoBC_nPercReducaoBC" class="form-control" runat="server"></asp:TextBox>
                                                    <span class="input-group-addon" style="font-weight: bold;"><i class="fa fa-percent"></i></span>
                                                </div>
                                             </div>
                                        </div>
                                        <div class="col-lg-12 row">
                                            <div class="col-lg-4">
                                                <div class="form-group">
                                                    <label>Pedido Cliente</label>
                                                    <asp:TextBox ID="txtReducaoBC_sPedidoCliente" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <asp:HiddenField ID="hddReducaoBC_nOrdem" runat="server" />

                        <div class="modal-footer text-left">
                            <asp:Button ID="cmdReducaoBC_Confirmar" class="btn  btn-success" runat="server" Text="Salvar" ValidationGroup="ReducaoBC" OnClick="cmdReducaoBC_Confirmar_Click" />
                            <asp:LinkButton CssClass="btn btn-danger" Text="Cancelar" runat="server" data-dismiss="modal" />
                        </div>
                     </ContentTemplate>
                </asp:UpdatePanel>
          
            </div>
        </div>
    </div>





    <asp:HiddenField ID="hddidPedido" runat="server" />
    <asp:HiddenField ID="hddidArquivo" runat="server" />

    <asp:HiddenField ID="hddnNumeroPedido" runat="server" />
    <asp:HiddenField ID="hddidEmpresa" runat="server" />
    <asp:HiddenField ID="hddsEmissaoNfe" runat="server" />
    <asp:HiddenField ID="hddsCaminho_UniNFe" runat="server" />
    <asp:HiddenField ID="hddchNFe" runat="server" />
    <asp:HiddenField ID="hddsCPF_CNPJ" runat="server" />
    <asp:HiddenField ID="hddstatusNota" runat="server" />
    <asp:HiddenField ID="hddnCartaCorrecao" runat="server" />
    <asp:HiddenField ID="hddsCartaCorrecao" runat="server" />
    <asp:HiddenField ID="hddsFuncaoPagina" runat="server" />



    <%----------------------------------------------------------------------------------------------------------------------------------------------%>
</asp:Content>
