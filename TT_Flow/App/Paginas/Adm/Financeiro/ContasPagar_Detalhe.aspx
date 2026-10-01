<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="ContasPagar_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Financeiro.ContasPagar_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Pesquisa_Parceiros.ascx" TagPrefix="uc1" TagName="Pesquisa_Parceiros" %>
<%@ Register Src="~/App/Controles/Pesquisa_Colaborador.ascx" TagPrefix="uc1" TagName="Pesquisa_Colaborador" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>
<%@ Register Src="~/App/Controles/Controle_CategoriasCC.ascx" TagPrefix="uc1" TagName="Controle_CategoriasCC" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <uc1:Manual runat="server" ID="manual" />
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <%--    <style>
    .camera-icon {
        position: absolute; /* Ajuste conforme necessário */
        top: 20px; /* Ajuste conforme necessário */
        right: 20px; /* Ajuste conforme necessário */
        color: #000; /* A cor do ícone */
        z-index: 100; /* Garante que o ícone fique sobre outros elementos */
    }
</style>
    <script>
    function handleFiles(files) {
        if (files.length > 0) {
            var file = files[0];
            // Faça algo com o arquivo capturado, como enviar para o servidor ou mostrar numa <img> tag
            console.log(file.name); // Exemplo: logar o nome do arquivo
        }
    }
    </script>--%>
    <%--    <style>
    .Caixa_Selecao {
    height: 34px !important;

}

</style>--%>

    <style>
        .dropdown-container-modal {
            position: relative;
            z-index: 1051;
            overflow: visible;
        }


        /*        .listbox-estilizado {
            border: 2px solid #007bff;
            border-radius: 8px;
            padding: 5px;
            background-color: #f8f9fa;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            box-shadow: 2px 2px 5px rgba(0,0,0,0.1);
        }
            .listbox-estilizado option {
                padding: 5px; 
                color: #333;
            }

                .listbox-estilizado option:checked {
                    background: #007bff; 
                    color: #fff;
                }*/

        /* Contêiner para a grid com scroll */
        .mini-grid-container {
            max-height: 160px; /* Altura máxima antes de aparecer o scroll */
            overflow-y: auto; /* Habilita o scroll vertical quando necessário */
            border: 1px solid #ddd; /* Borda ao redor do contêiner */
            border-radius: 4px;
        }

        /* Ajustes na mini-grid para funcionar bem dentro do contêiner */
        .mini-grid {
            width: 100%;
            border-collapse: collapse;
            font-size: 0.9em;
        }

            .mini-grid th, .mini-grid td {
                padding: 8px 10px;
                text-align: left;
                border-bottom: 1px solid #ddd;
            }

            .mini-grid th {
                background-color: #f8f9fa;
                font-weight: bold;
                position: sticky; /* Efeito "cabeçalho fixo" ao rolar */
                top: 0;
                z-index: 1;
            }

            .mini-grid tr:last-child td {
                border-bottom: none; /* Remove a borda do último item */
            }

            .mini-grid tr:hover {
                background-color: #f5f5f5;
            }

        .espaco-linha {
            padding: 5px 10px 5px 10px;
        }
    </style>
    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Conta a Pagar"></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>
    </div>

    <div>
        <ul id="tab_ContasPagar" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="tabpanel active">
                <a href="#ContasPagar" id="aba_ContasPagar" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Conta a Pagar</b></a>
            </li>

            <li role="presentation" runat="server" id="aba_Arquivo">
                <a href="#arquivo" role="tab" id="aba_Arquivo-tab" data-toggle="tab" aria-controls="arquivo"><b>Arquivos</b></a>
            </li>
            <li role="presentation" runat="server" id="Aba_Log">
                <a href="#log" role="tab" id="log-tab" data-toggle="tab" aria-controls="Log"><b>Histórico</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_Historico">
                <a href="#historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="Historico"><b>Histórico</b></a>
            </li>
        </ul>
    </div>

    <div id="tab" class="tab-content">

        <div role="tabpanel" class="tab-pane fade in active" id="ContasPagar" aria-labelledby="ContasPagar-tab">
            <br />
            <asp:UpdatePanel ID="updDetalhe" runat="server">
                <ContentTemplate>
                    <div class="panel panel-default" runat="server" id="div_ContasPagar">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Detalhe</b></h3>
                        </div>
                        <div class="panel-body">

                            <div class="form-stacked row">
                                <div class="col-lg-12">
                                    <div class="form-stacled row">
                                        <div class="col-lg-1">
                                            <div class="form-group">
                                                <label>ID</label>
                                                <asp:TextBox ID="txtidContasPagar" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" style="display: ruby" <%--Agnes Partal * 01/07/2024--%>>
                                            <div class="form-group">
                                                <h3>
                                                    <span class="status" title="Status">
                                                        <asp:Label ID="lblsDscTipoStatus" runat="server" TabIndex="100"></asp:Label>
                                                    </span>
                                                </h3>
                                            </div>
                                            <%--Agnes Partal * 01/07/2024----------------------------------------------------------------%>
                                            <div class="form-group" runat="server" id="div_sStatusConciliado" visible="false">
                                                <h3>
                                                    <span class="status" title="StatusConciliado">
                                                        <asp:Label ID="lblsStatusConciliado" runat="server" TabIndex="150"></asp:Label>
                                                    </span>
                                                </h3>
                                            </div>
                                            <%--------------------------------------------------------------------------------------------%>
                                        </div>

                                        <div class="col-lg-4" runat="server" id="Div_sStatusAdiantado">
                                            <div class="form-group">
                                                <h3>
                                                    <span class="status" title="StatusAdiantado">
                                                        <asp:Label ID="lblsStatusAdiantado" runat="server" TabIndex="150"></asp:Label>
                                                    </span>
                                                </h3>
                                            </div>
                                        </div>

                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-stacled row">
                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <asp:Label ID="lbldtEmissao" runat="server" Text="Data de emissão" Style="font-weight: bold;"></asp:Label>
                                                <asp:TextBox ID="txtdtEmissao" class="form-control" runat="server" type="date" Style="margin-top: 5px;"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <%-- Agnes Partal * 06/08/2024 --%>
                                    <div class="col-lg-12">
                                        <div class="row">
                                            <%-------------------------------%>
                                            <div class="col-lg-4" runat="server" id="div_ddlEmpresa">
                                                <div class="form-group">
                                                    <label>Empresa</label>
                                                    <asp:DropDownList ID="ddlidEmpresa" class="form-control  Caixa_Selecao" runat="server" attrname="idContasPagar" AutoPostBack="true" OnTextChanged="ddlidCentroDeCusto_TextChanged"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-4">
                                                <div class="form-group">
                                                    <label>Categoria</label>
                                                    <asp:DropDownList ID="ddlidCategoriaPagar" class="form-control Caixa_Selecao" runat="server" attrname="idContasPagar" AutoPostBack="true" OnTextChanged="ddlidCategoriaPagar_TextChanged"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <%----------------------------------------------------------------------------------------------%>

                                            <%--Credor impostos--%>
                                            <div class="col-lg-4" runat="server" id="Div_Parceiro_Imposto">
                                                <div class="form-group">
                                                    <label>Credor</label>
                                                    <asp:DropDownList ID="ddlidParceiroImposto" class="form-control Caixa_Selecao" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidParceiroImposto_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <%--Credor Colaboradores--%>
                                            <div class="col-lg-4" runat="server" id="Div_Parceiro_Interno">
                                                <div class="form-group">
                                                    <label>Credor</label>
                                                    <asp:DropDownList ID="ddlidParceiroInterno" class="form-control Caixa_Selecao" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidParceiroInterno_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <%--Credor Cliente--%>
                                            <div class="col-lg-4" runat="server" id="Div_Parceiro_Cliente">
                                                <div class="form-group">
                                                    <label>Credor</label>
                                                    <asp:DropDownList ID="ddlidParceiroCliente" class="form-control Caixa_Selecao" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidParceiroCliente_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <%--Credor Cliente--%>
                                            <div class="col-lg-4" runat="server" id="Div_idBancoCredo">
                                                <div class="form-group">
                                                    <label>Banco Credor</label>
                                                    <asp:DropDownList ID="ddlidBancoCredor" class="form-control Caixa_Selecao" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidBancoCredor_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <%----------------------------------------------------------------------------------------------%>


                                            <script type="text/javascript">
                                                document.addEventListener('DOMContentLoaded', function () {
                                                    var dataInput = document.getElementById('<%= txtdtPrevisaoPagamento.ClientID %>');
                                                    dataInput.addEventListener('blur', function () {
                                                        __doPostBack('<%= txtdtPrevisaoPagamento.UniqueID %>', '');
                                                    });
                                                });
                                            </script>

                                        </div>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <%-- Agnes Partal * 06/08/2024 --%>
                                    <div class="col-lg-12">
                                        <div class="row">
                                            <%-------------------------------%>
                                            <div class="col-lg-2" runat="server" id="Div_dtPrevisaoPagamento">
                                                <div class="form-group">
                                                    <label>Previsão Pagamento</label>
                                                    <asp:TextBox ID="txtdtPrevisaoPagamento" class="form-control" runat="server" AutoPostBack="False" type="date" OnTextChanged="txtdtPrevisaoPagamento_TextChanged"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2" runat="server" id="Div_sDiaSemana">
                                                <div class="form-group">
                                                    <label>Dia Pagamento</label>
                                                    <asp:TextBox ID="txtsDiaSemana" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2" runat="server" id="Div_dtVencimento">
                                                <div class="form-group">
                                                    <label>Data Vencimento</label>
                                                    <asp:TextBox ID="txtdtVencimento" class="form-control" runat="server" type="date"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2" runat="server" id="Div_dtApuracao">
                                                <div class="form-group">
                                                    <label>Data Apuração</label>
                                                    <asp:TextBox ID="txtdtApuracao" class="form-control" runat="server" type="date"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2" runat="server" id="Div_nValorBruto">
                                                <div class="form-group">
                                                    <asp:Label ID="lblValorBruto" runat="server" Text="Valor Bruto" Style="font-weight: bold;"></asp:Label>
                                                    <label>&nbsp</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon">R$</span>
                                                        <asp:TextBox ID="txtnValorBruto" class="form-control " Style="z-index: 0;" runat="server" AutoPostBack="true" OnTextChanged="txtnValorBruto_TextChanged"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-lg-2" runat="server" id="Div_nValorOriginal">
                                                <div class="form-group">
                                                    <asp:Label ID="lblValorLiquido" runat="server" Text="Valor Líquido" Style="font-weight: bold;"></asp:Label>
                                                    <label>&nbsp</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon">R$</span>
                                                        <asp:TextBox ID="txtnValorOriginal" Style="z-index: 0;" class="form-control " runat="server"
                                                            onkeyup="formatarMoeda(this); atualizarDelimitadorCentroCusto(this);"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-lg-2" runat="server" id="Div_Saldo">
                                                <div class="form-group">
                                                    <label>Saldo a pagar</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon">R$</span>
                                                        <asp:TextBox ID="txtnSaldo" class="form-control " Style="z-index: 0;" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-lg-2" runat="server" id="DivTotal">
                                                <div class="form-group">
                                                    <label>Total Pago</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon">R$</span>
                                                        <asp:TextBox ID="txtnTotal" class="form-control valor-input" aria-describedby="sMoedaOrigem" Style="z-index: 0;" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-lg-4" runat="server" id="Div_sCodigo">
                                                <div class="form-group">
                                                    <asp:Label ID="lblNotaFiscal" runat="server" Text="Nota Fiscal" Style="font-weight: bold;"></asp:Label>
                                                    <asp:TextBox ID="txtsCodigo" class="form-control" runat="server" MaxLength="100" Style="margin-top: 5px;"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-4" runat="server" id="Div_sDocumento">
                                                <div class="form-group">
                                                    <asp:Label ID="lblsDocumento" runat="server" Text="Número do Pedido" Style="font-weight: bold;"></asp:Label>
                                                    <asp:TextBox ID="txtsDocumento" class="form-control" runat="server" MaxLength="100" Style="margin-top: 5px;"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-4" runat="server" id="Div_idFormaPagamento">
                                                <div class="form-group">
                                                    <label>Forma de Pagamento </label>
                                                    <asp:DropDownList ID="ddlidFormaPagamento" class="form-control  Caixa_Selecao" runat="server" attrname="idContasPagar" AutoPostBack="true" OnSelectedIndexChanged="ddlidFormaPagamento_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-4" runat="server" id="Div_ComplementoMeioPag">
                                                <div class="form-group">
                                                    <asp:Label ID="lblsComplementoFormaPag" runat="server" Text="Complemento Forma Pagamento" Style="font-weight: bold;"></asp:Label>
                                                    <asp:TextBox ID="txtsComplementoFormaPag" class="form-control " AutoPostBack="true" MaxLength="60" runat="server" Style="margin-top: 5px;"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <%-- Agnes Partal * 06/08/2024 --%>
                                    <div class="col-lg-12">
                                        <div class="row">
                                            <%-------------------------------%>
                                            <div class="col-lg-4">
                                                <div class="form-group">
                                                    <label>Código Contábil</label>
                                                    <asp:DropDownList ID="ddlidContabil" class="form-control  Caixa_Selecao" runat="server" attrname="idContasPagar" AutoPostBack="true" OnTextChanged="ddlidCentroDeCusto_TextChanged"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <%--    <div class="col-lg-4" runat="server" id="Div_CentroCusto">
                                                <div class="form-group">
                                                    <label>Centro de custo</label>
                                                    <asp:DropDownList ID="ddlidCentroDeCusto" class="form-control  Caixa_Selecao" runat="server" attrname="idContasPagar" AutoPostBack="true" OnTextChanged="ddlidCentroDeCusto_TextChanged"></asp:DropDownList>
                                                </div>
                                            --%>

                                            <div class="col-lg-4">
                                                <div class="form-group">
                                                    <label>Tipo Pagamento </label>
                                                    <asp:DropDownList ID="ddlidMeioPagamento" class="form-control  Caixa_Selecao" runat="server" attrname="idContasPagar" AutoPostBack="True" OnSelectedIndexChanged="ddlidMeioPagamento_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>


                                            <div class="col-lg-4" runat="server" id="Div_nParcelas">
                                                <div class="form-group">
                                                    <label>N° parcela</label>
                                                    <asp:TextBox ID="txtnParcelas" class="form-control " AutoPostBack="true" runat="server" OnTextChanged="txtnParcelas_TextChanged"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-4" runat="server" id="Div_sQuantidadeParcela">
                                                <div class="form-group">
                                                    <label>Quantidade de parcelas</label>
                                                    <asp:TextBox ID="txtsQuantidadeParcela" class="form-control " AutoPostBack="true" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                        </div>
                                        <div class="row" id="Div_CentroCusto" runat="server">

                                            <asp:Panel runat="server" ID="pnlCentroCustoPrincipal">
                                                <uc1:Controle_CategoriasCC runat="server" ID="Controle_CategoriasCC" SAcao="subtrair" sPlaceholder="Sem Categoria" />
                                            </asp:Panel>

                                            <%--                                            <asp:Panel runat="server" ID="pnlRateiosLista" Visible="false">
                                                <div class="col-lg-8">
                                                    <div class="form-group">
                                                        <label>Centro de Custo (Rateado)</label>
                                                        <asp:ListBox runat="server" ID="lbRateios"  CssClass="form-control listbox-estilizado" Rows="4"
                                                            SelectionMode="Multiple" Enabled="false" />                                                  
                                                    </div>
                                                </div>
                                            </asp:Panel>--%>

                                            <asp:Panel runat="server" ID="pnlRateiosLista" Visible="false">
                                                <div class="col-lg-8">
                                                    <div class="form-group">
                                                        <label>Centro de Custo (Divididos)</label>

                                                        <div class="mini-grid-container">
                                                            <asp:Repeater ID="rptRateios" runat="server">
                                                                <HeaderTemplate>
                                                                    <table class="mini-grid">
                                                                        <thead>
                                                                            <tr>
                                                                                <th>Centro de Custo</th>
                                                                                <th>Categoria</th>
                                                                                <th style="text-align: right;">Valor</th>
                                                                            </tr>
                                                                        </thead>
                                                                        <tbody>
                                                                </HeaderTemplate>

                                                                <ItemTemplate>
                                                                    <tr>
                                                                        <td><%# Eval("sDscCentroCusto") %></td>
                                                                        <td><%# Eval("sDscCategoria") %></td>
                                                                        <td style="text-align: right;"><%# Eval("nValor", "R$ {0:N2}") %></td>
                                                                    </tr>
                                                                </ItemTemplate>

                                                                <FooterTemplate>
                                                                    </tbody>
                        </table>
                   
                                                                </FooterTemplate>
                                                            </asp:Repeater>
                                                        </div>

                                                    </div>
                                                </div>
                                            </asp:Panel>

                                            <div class="col-lg-4">
                                                <div class="form-group" style="padding-top: 25px;">
                                                    <asp:LinkButton ID="btnAbrirModalCC" Visible="false" runat="server" CssClass="btn btn-info"
                                                        OnClick="btnAbrirModalCC_Click" ToolTip="Fazer divisão por múltiplos centros de custo">
                <i class="fa fa-sitemap"></i>&nbsp;<b>...</b>
            </asp:LinkButton>
                                                </div>
                                            </div>

                                        </div>

                                    </div>

                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Observações Gerais</label>
                                        <asp:TextBox ID="txtsObservacaoGeral" class="form-control" runat="server" TextMode="MultiLine" Height="120px"></asp:TextBox>
                                    </div>
                                </div>

                            </div>

                        </div>
                    </div>
                    <%------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>

                    <div class="panel panel-default" runat="server" id="DIV_DADOS_COLABORADOR">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Dados Colaborador</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_dadosColab" />
                                </div>
                                <div>
                                    <br />
                                </div>

                                <div class="col-lg-3" runat="server" id="Div_idColaborador">
                                    <div class="form-group">
                                        <label>ID</label>
                                        <asp:TextBox ID="txtidColaborador" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="Div_sAdiantado">
                                    <div class="form-group">
                                        <label>Banco Credor</label>
                                        <asp:TextBox ID="txtsAdiantado" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Tipo de Conta </label>
                                        <asp:DropDownList ID="ddlsTipoConta" runat="server" class="form-control yes_no select  Caixa_Selecao">
                                            <asp:ListItem Value="C">Conta Corrente</asp:ListItem>
                                            <asp:ListItem Value="P">Poupança</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Banco</label>
                                        <asp:TextBox ID="txtsBanco" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Agencia</label>
                                        <asp:TextBox ID="txtsAgenciaBancaria" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>N° Conta</label>
                                        <asp:TextBox ID="txtsContaBancaria" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server">
                                    <div class="form-group">
                                        <label>Dependentes? </label>
                                        <asp:DropDownList ID="ddlsDependentesConvenio" runat="server" class="form-control yes_no select  Caixa_Selecao">
                                            <asp:ListItem Value="N">Não</asp:ListItem>
                                            <asp:ListItem Value="S">Sim</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>


                            </div>
                        </div>
                    </div>
                    <%------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>
                    <div class="panel panel-default" runat="server" id="DIV_Dados_Dependentes">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Dados Dependentes</b></h3>
                        </div>

                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12 table-responsive">
                                    <asp:GridView
                                        ID="gv_Dependentes" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False">

                                        <%--
                                     ========================
                                     =========índice=========
                                     ========================
                                     N° Célula ------- Info
                                     ========================
                                     0 --------------- Nome
                                     1 --------------- CPF
                                     2 --------------- Data Nascimento
                                     3 --------------- Possui Pensão
                                     4 --------------- Valor Pensão
                                     5 --------------- Possui Plano de Saúde
                                     6 --------------- Valor Plano de Saúde
                                     7 --------------- Tipo Conta
                                     8 --------------- Banco
                                     9 --------------- Agência
                                     10 -------------- N° Conta
                                     ========================
                                        --%>

                                        <Columns>

                                            <asp:BoundField DataField="sNomeDependente" HeaderText="Nome">
                                                <ItemStyle Width="14%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sCPFDependente" HeaderText="CPF">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtNascDependente" HeaderText="Data Nascimento">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sPensaoDependente" HeaderText="Possui pensão?">
                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nValorPensaoDependente" HeaderText="Valor Pensão">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sPlanoSaudeDependente" HeaderText="Possui Plano Saúde?">
                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nValorPlanoDependente" HeaderText="Valor Plano">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="idTipoContaDepentente" HeaderText="Tipo Conta">
                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sBancoDependente" HeaderText="Banco">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sAgencia" HeaderText="Agência">
                                                <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sContaDependente" HeaderText="N° Conta">
                                                <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>


                                        </Columns>
                                    </asp:GridView>

                                </div>
                            </div>
                        </div>
                    </div>

                    <%------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>
                    <div class="panel panel-default" runat="server" id="DIV_Adiantamento">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Resumo de Títulos Adiantados</b></h3>
                        </div>

                        <div class="panel-body">
                            <div class="form-stacked row">

                                <div class="col-lg-12 form-group" style="padding: 0px">
                                    <div class="col-lg-2">
                                        <label>Taxa Juros</label>
                                        <asp:TextBox ID="txtnTaxaJuros" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                    </div>

                                    <div class="col-lg-2">
                                        <label>IOF</label>
                                        <asp:TextBox ID="txtnIOF" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                    </div>

                                    <div class="col-lg-2">
                                        <label>IOF Adicional</label>
                                        <asp:TextBox ID="txtnIOF_Adicional" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-12 table-responsive">
                                    <asp:GridView
                                        ID="gv_Adiantamento" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="True" OnRowDataBound="gv_Adiantamento_RowDataBound">

                                        <Columns>

                                            <asp:TemplateField HeaderText="Empresa">
                                                <ItemTemplate>
                                                    <asp:HyperLink ID="idContasReceber" runat="server"
                                                        NavigateUrl='<%# "/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id=" + Eval("idContasReceber") %>'
                                                        Text='<%# Eval("sRazaoSocialReceber") %>'>
													</asp:HyperLink>
                                                </ItemTemplate>
                                                <ItemStyle Width="18%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="idContasReceber" HeaderText="ID Nota"
                                                SortExpression="dtVencimento"
                                                ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="4%" />

                                            <asp:BoundField DataField="dtVencimento" HeaderText="Data Vencimento"
                                                SortExpression="dtVencimento"
                                                ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="10%" />

                                            <asp:BoundField DataField="nSomaValorOriginal" HeaderText="Valor Título"
                                                DataFormatString="{0:C2}" SortExpression="nSomaValorOriginal"
                                                ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="10%" />

                                            <asp:BoundField DataField="nDias" HeaderText="Dias até Vencimento"
                                                SortExpression="nDias"
                                                ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="10%" />

                                            <asp:BoundField DataField="sStatus" HeaderText="Status"
                                                SortExpression="sStatus"
                                                ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="10%" />

                                        </Columns>
                                    </asp:GridView>
                                </div>

                                <div class="col-lg-12 form-group" style="padding: 0px;">

                                    <div class="col-lg-2">
                                        <label>Total Juros</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">R$</span>
                                            <asp:TextBox runat="server" ID="txtnTotalJuros" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <label>Total IOF</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">R$</span>
                                            <asp:TextBox runat="server" ID="txtnTotalIOF" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <label>Tarifas</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">R$</span>
                                            <asp:TextBox runat="server" ID="txtnTarifas" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-1" style="margin-top: 24px;">
                                        <label>&nbsp;</label>
                                        <asp:Button runat="server" ID="btnSalvarValoresAdiantamento" class="btn btn-success" Text="Atualizar Valores" OnClick="btnSalvarValoresAdiantamento_Click"/>
                                    </div>

                                </div>
                                <div class="col-lg-12 form-group" style="padding: 0px;">

                                    <div class="col-lg-2">
                                        <label>Valor Operação</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">R$</span>
                                            <asp:TextBox runat="server" ID="txtnValorOperacao" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <label>Total Títulos</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">R$</span>
                                            <asp:TextBox runat="server" ID="txtnTotalTitulos" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>


                                    <div class="col-lg-2">
                                        <label>Total Liberado</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">R$</span>
                                            <asp:TextBox runat="server" ID="txtnTotalLiberado" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <label>Total Despesas</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">R$</span>
                                            <asp:TextBox runat="server" ID="txtnTotalDespesas" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <label>Títulos em Aberto</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">R$</span>
                                            <asp:TextBox runat="server" ID="txtnTitulosAberto" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>

                                </div>
                            </div>

                        </div>
                    </div>



                    <%------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>
                    <div class="panel panel-default" runat="server" id="Div_linkcompensacao">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Compensação</b></h3>
                        </div>

                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12 table-responsive">
                                    <asp:GridView
                                        ID="gv_linkcompensacao" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False">

                                        <Columns>

                                            <asp:TemplateField HeaderText="Link">
                                                <ItemTemplate>
                                                    <asp:HyperLink ID="idCompensacao" runat="server"
                                                        NavigateUrl='<%# "/App/Paginas/Adm/Financeiro/ContasPagar_Detalhe.aspx?id=" + Eval("idCompensacao") %>'
                                                        Text='<%# Eval("sDscCredorCompensacao") %>'>
                                        </asp:HyperLink>
                                                </ItemTemplate>
                                                <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>




                                        </Columns>
                                    </asp:GridView>

                                </div>
                            </div>
                        </div>
                    </div>
                    <%------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>

                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Pagamento" />

                    <div class="panel panel-default" runat="server" id="Div_Pagamento">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Informações sobre Pagamento</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemLancamento" />
                                </div>
                                <div runat="server" id="Div_CadastroPagamento">


                                    <div class="col-lg-12">
                                        <div class="form-stacled row">
                                            <div class="col-lg-2" runat="server" id="Div_dtPagamento_Info_Pag">
                                                <div class="form-group">
                                                    <label>Data</label>
                                                    <asp:TextBox ID="txtdtPagamento_Info_Pag" class="form-control" runat="server" type="date"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-3" runat="server" id="Div_idConta_Info_Pag">
                                                <div class="form-group">
                                                    <label>Conta </label>
                                                    <asp:DropDownList ID="ddlidConta_Info_Pag" class="form-control  Caixa_Selecao" runat="server" attrname="idContasPagar"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-3" runat="server" id="Div_idFormaPagamento_Info_Pag">
                                                <div class="form-group">
                                                    <label>Forma de Pagamento </label>
                                                    <asp:DropDownList ID="ddlidFormaPagamento_Info_Pag" class="form-control  Caixa_Selecao" runat="server" attrname="idContasPagar" AutoPostBack="true" OnSelectedIndexChanged="ddlidFormaPagamento_Info_Pag_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-3" runat="server" id="Div_Compensacao">
                                                <div class="form-group">
                                                    <label>Títulos </label>
                                                    <asp:DropDownList ID="ddlidCompensacao" class="form-control  Caixa_Selecao" runat="server" attrname="idContasPagar" AutoPostBack="true" OnSelectedIndexChanged="ddlidCompensacao_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-2" runat="server" id="Div_ValorPago">
                                        <div class="form-group">
                                            <label>Valor Pago </label>
                                            <div class="input-group">
                                                <span class="input-group-addon">R$</span>
                                                <asp:TextBox ID="txtnValorPagamento_Info_Pag" class="form-control" runat="server" AutoPostBack="true" Style="z-index: 0;" OnTextChanged="txtnValorPagamento_Info_Pag_TextChanged"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-2" runat="server" id="Div_Multa">
                                        <div class="form-group">
                                            <label>Valor Multa </label>
                                            <div class="input-group">
                                                <span class="input-group-addon">R$</span>
                                                <asp:TextBox ID="txtnMulta" class="form-control" runat="server" AutoPostBack="true" Style="z-index: 0;" OnTextChanged="txtnValorTotalPag_TextChanged"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-2" runat="server" id="Div_Juros">
                                        <div class="form-group">
                                            <label>Valor Juros </label>
                                            <label>&nbsp</label>
                                            <div class="input-group">
                                                <span class="input-group-addon">R$</span>
                                                <asp:TextBox ID="txtnJuros" class="form-control" runat="server" AutoPostBack="true" Style="z-index: 0;" OnTextChanged="txtnValorTotalPag_TextChanged"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-2" runat="server" id="Div_Desconto">
                                        <div class="form-group">
                                            <asp:Label ID="lblValorDesconto" runat="server" Text="Valor Desconto" Style="font-weight: bold;"></asp:Label>
                                            <label>&nbsp</label>
                                            <div class="input-group">
                                                <span class="input-group-addon">R$</span>
                                                <asp:TextBox ID="txtnDesconto" class="form-control" runat="server" AutoPostBack="true" Style="z-index: 0;" OnTextChanged="txtnValorTotalPag_TextChanged"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-2" runat="server" id="Div_ValorTotal">
                                        <div class="form-group">
                                            <label>Total Pago </label>
                                            <div class="input-group">
                                                <span class="input-group-addon">R$</span>
                                                <asp:TextBox ID="txtnValorTotalPag" class="form-control" runat="server" AutoPostBack="true" Style="z-index: 0;" OnTextChanged="txtnValorTotalPag_TextChanged"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>


                                    <div class="col-lg-2" runat="server" id="Div_BotaoIncluirPag">
                                        <div class="form-group">
                                            <br />
                                            <asp:Button ID="cmdPagamento_Incluir" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdPagamento_Incluir_Click" Style="margin-top: 5px;" />
                                            <asp:Button ID="BtnEdicaoPagamento" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Salvar" OnClick="BtnEdicaoPagamento_Click" />
                                        </div>
                                    </div>
                                    <br />
                                    <asp:HiddenField ID="hddPagamento_idLinha" runat="server" />


                                </div>

                                <div class="col-lg-12">
                                    <asp:GridView ID="dtgPagamento" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small"
                                        OnRowDeleting="dtgPagamento_RowDeleting" OnRowDataBound="dtgPagamento_RowDataBound" OnRowCommand="dtgPagamento_RowCommand">

                                        <%--
                                    ========================
                                    =========índice=========
                                    ========================
                                    N° Célula ------- Info
                                    ========================
                                    0 --------------- idLinha
                                    1 --------------- Nº Pagamento
                                    2 --------------- Data do Pagamento
                                    3 --------------- Valor Pago
                                    4 --------------- Valor Multa
                                    5 --------------- Valor Juros
                                    6 --------------- Valor Desconto
                                    7 --------------- Valor Total
                                    8 --------------- Conta
                                    9 --------------- Forma de Pagamento
                                    ========================
                                        --%>
                                        <Columns>

                                            <asp:BoundField DataField="idlinha" HeaderText="idLinha">
                                                <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="nNumeroParcela_Info_Pag" HeaderText="Nº Pagamento">
                                                <ItemStyle Width="4%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="dtPagamento_Info_Pag" HeaderText="Data do Pagamento">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="nValorPagamento_Info_Pag" HeaderText="Valor Pago" DataFormatString="{0:C}" HtmlEncode="False">
                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="nMulta" HeaderText="Valor Multa" DataFormatString="{0:C}" HtmlEncode="False">
                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="nJuros" HeaderText="Valor Juros" DataFormatString="{0:C}" HtmlEncode="False">
                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="nDesconto" HeaderText="Valor Desconto" DataFormatString="{0:C}" HtmlEncode="False">
                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="nValorTotalPag" HeaderText="Valor Total" DataFormatString="{0:C}" HtmlEncode="False">
                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="sDscConta_Info_Pag" HeaderText="Conta">
                                                <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="sDscFormaPagamento_Info_Pag" HeaderText="Forma de Pagamento">
                                                <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>


                                            <asp:TemplateField HeaderText="">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkPagamento_Editar" CommandArgument='<%# Bind("idLinha") %>' runat="server" ToolTip="Editar" CssClass="btn btn-small" TabIndex="100" CommandName="Editar"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="lnkPagamento_UpLoad" CommandArgument='<%# Bind("idLinha") %>' runat="server" ToolTip="Efetuar Upload de Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Upload_Arquivo"><i class="fa-upload fa"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="lnkPagamento_Download" CommandArgument='<%# Bind("idLinha") %>' runat="server" ToolTip='<%# Bind("sNomeArquivo", "Download do Arquivo: {0}") %>' CssClass="btn btn-small" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="lnkPagamento_Excluir" CommandArgument='<%# Bind("idLinha") %>' runat="server" CssClass="btn btn-small" TabIndex="100" ToolTip="Apagar Registro" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>

                                                </ItemTemplate>
                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>

                            </div>
                        </div>
                    </div>

                    <%------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>

                    <div class="panel panel-default" runat="server" id="DIV_Lancamentos">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Lançamentos</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina1" />
                                </div>
                                <div>


                                    <%--<div class="col-lg-2">
                                        <div class="form-group">
                                            <br />
                                            <asp:Button ID="cmdLancamento_Incluir" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdLancamento_Incluir_Click" />
                                        </div>
                                    </div>--%>
                                    <br />
                                </div>

                                <div class="col-lg-12">
                                    <asp:GridView ID="dtgLancamento" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="true" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDeleting="dtgLancamento_RowDeleting" OnRowDataBound="dtgLancamento_RowDataBound">
                                        <Columns>

                                            <asp:BoundField DataField="idlinha" HeaderText="idLinha">
                                                <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nParcelaLancamentoPag" HeaderText="Nº Parcela">
                                                <ItemStyle Width="2%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Valor Parcela Bruto">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtnValorBrutoLancamentoPag" class="form-control" runat="server" Text='<%# Bind("nValorBrutoLancamentoPag","{0:N2}") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle Width="14%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Valor Parcela Líquido">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtnValorLancamentoPag" class="form-control" runat="server" Text='<%# Bind("nValorLancamentoPag","{0:N2}") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle Width="14%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Data Vencimento">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtdtLancamentoPag" class="form-control" runat="server" type="date" Text='<%# Bind("dtLancamentoPag") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle Width="14%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Forma de Pagamento">
                                                <ItemTemplate>
                                                    <asp:DropDownList ID="ddlidFormaPagamentoLancamentoPag" runat="server" class="form-control yes_no select  Caixa_Selecao"></asp:DropDownList>
                                                </ItemTemplate>
                                                <ItemStyle Width="27%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkLancamentosPag_Excluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>

                            </div>
                        </div>
                    </div>

                    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div role="tabpanel" class="tab-pane fade" id="log" aria-labelledby="log-tab">
            <br />
            <div class="panel panel-default" runat="server" id="DIV_Log">
                <div class="panel-heading">

                    <h3 class="panel-title"><b>Historico</b></h3>
                </div>
                <div class="panel-body">
                    <div class="form-stacked row">
                        <div class="col-lg-12 table-responsive">
                            <asp:GridView
                                ID="gv_Log" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                ShowFooter="False" OnRowDataBound="gv_Log_RowDataBound">
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
                                <%--
                                    ========================
                                    =========índice=========
                                    ========================
                                    N° Célula ------- Info
                                    ========================
                                    0 --------------- Data Emissão
                                    1 --------------- Data Vencimento
                                    2 --------------- Valor Liquido
                                    3 --------------- Saldo
                                    4 --------------- Nota Fiscal
                                    5 --------------- Número do Pedido
                                    6 --------------- Forma Pagamento
                                    7 --------------- Código Contabil
                                    8 --------------- Centro Custo
                                    9 --------------- Meio Pagamento
                                    10 -------------- Por
                                    11 -------------- Data
                                    12 -------------- Reaberto
                                    ========================
                                --%>

                                <Columns>
                                    <asp:BoundField DataField="dtEmissao" HeaderText="Data Emissão">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="dtVencimento" HeaderText="Data Vencimento">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="nValorOriginal" HeaderText="Valor Líquido">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="nSaldo" HeaderText="Saldo">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sCodigo" HeaderText="Nota Fiscal">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDocumento" HeaderText="Número do Pedido">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscFormapagamento" HeaderText="Forma Pagamento">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscCodContabil" HeaderText="Código Contabil">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscCentroCusto" HeaderText="Centro Custo">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscMeiopagamento" HeaderText="Meio Pagamento">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="dtAtualizacao" HeaderText="Data">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sReabertoCompleto" HeaderText="Reaberto">
                                        <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>

                        </div>
                    </div>
                </div>
            </div>
        </div>
        <%------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>


        <div role="tabpanel" class="tab-pane fade" id="arquivo" aria-labelledby="aba_Arquivo-tab">
            <div class=" embed-responsive embed-responsive-16by9" style="min-height: 200px">
                <embed type="text/html" runat="server" id="frmArquivos" width="800" height="500" />
            </div>
        </div>
        <%------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>
    </div>




    <fieldset class="form-stacked actions">
        <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" ValidationGroup="DETALHE" />

        <asp:Button ID="btnAlterarVencimento" class="btn  btn-lg btn-info" runat="server" Text="Editar" OnClick="btnAlterarVencimento_Click" />


        <asp:Button ID="BtnAdicionarObservacao" class="btn btn-lg btn-info" runat="server" Text="Adicionar Observação" OnClick="BtnAdicionarObservacao_Click" />


        <asp:Button ID="BtnExcluirParcela" class="btn  btn-lg btn-danger" runat="server" Text="Excluir" OnClick="BtnExcluirParcela_Click" />

        <asp:Button ID="BtnReabrirTitulo" class="btn  btn-lg btn-danger" runat="server" Text="Reabrir" OnClick="BtnReabrirTitulo_Click" />

        <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field-cancel" title="Voltar" onclick="history.go(-1)">
    </fieldset>


    <%-- ======================================================================= --%>
    <%-- │ INÍCIO: MODAL DE RATEIO POR CENTRO DE CUSTO (VERSÃO FINAL)         │ --%>
    <%-- ======================================================================= --%>
    <div class="modal fade" id="modalCentroDeCustos" tabindex="-1" role="dialog" aria-labelledby="modalCentroDeCustosLabel">
        <div class="modal-dialog modal-xl" role="document">
            <div class="modal-content">
                <asp:UpdatePanel ID="updModalCC" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h4 class="modal-title" id="modalCentroDeCustosLabel">Divisão por Centro de Custo</h4>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close" style="margin-top: -25px;"><span aria-hidden="true">&times;</span></button>
                        </div>
                        <div class="modal-body">
                            <uc1:MensagemPagina runat="server" ID="MensagemModalCC" />

                            <%-- PAINEL DE INCLUSÃO --%>
                            <div class="panel-body">

                                <div class="row">

                                    <div class="dropdown-container-modal">
                                        <uc1:Controle_CategoriasCC runat="server" ID="ControleCategoriasModal" SAcao="subtrair" sPlaceholder="Sem Categoria" />
                                    </div>

                                    <div class="col-lg-4" runat="server" id="divValorModal" visible="false">
                                        <div class="form-group">
                                            <label>Valor do Rateio</label>
                                            <div class="input-group">
                                                <span class="input-group-addon">R$</span>
                                                <asp:TextBox runat="server" ID="txtModalValor" CssClass="form-control" Style="z-index: 0;" />
                                            </div>
                                        </div>
                                    </div>

                                </div>

                                <div class="row">
                                    <div class="col-lg-12">
                                        <div class="form-group">
                                            <label>Observação (Opcional)</label>
                                            <%-- TextMode="MultiLine" e Rows="2" para deixar o campo mais alto --%>
                                            <asp:TextBox runat="server" ID="txtModalObservacao" CssClass="form-control" MaxLength="200" TextMode="MultiLine" Rows="2" />
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-lg-12">
                                        <asp:LinkButton runat="server" ID="btnModalIncluir" CssClass="btn btn-primary pull-right" OnClick="btnModalIncluir_Click" ValidationGroup="ModalCC">
                <i class="fa fa-plus"></i> Incluir
            </asp:LinkButton>
                                    </div>
                                </div>

                            </div>
                            <%-- GRID DE LANÇAMENTOS --%>
                            <div>
                                <asp:GridView ID="gvLancamentosCC" runat="server"
                                    AutoGenerateColumns="false"
                                    CssClass="table table-striped table-bordered table-hover"
                                    Width="100%"
                                    DataKeyNames="GridKey"
                                    ShowFooter="true"
                                    OnRowDeleting="gvLancamentosCC_RowDeleting"
                                    OnRowDataBound="gvLancamentosCC_RowDataBound">
                                    <Columns>
                                        <asp:BoundField DataField="sDscCentroCusto" HeaderText="Centro de Custo" HeaderStyle-Width="25%" />
                                        <asp:BoundField DataField="sDscCategoria" HeaderText="Categoria" HeaderStyle-Width="25%" />

                                        <asp:TemplateField HeaderText="Valor" HeaderStyle-Width="20%">
                                            <ItemTemplate>
                                                <div class="input-group">
                                                    <span class="input-group-addon">R$</span>
                                                    <asp:TextBox runat="server" ID="txtGridValor" Text='<%# Bind("nValor", "{0:N2}") %>'
                                                        CssClass="form-control text-right" AutoPostBack="true"
                                                        OnTextChanged="txtGridValor_TextChanged" Style="z-index: 0;"></asp:TextBox>
                                                </div>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Right" />
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="sObservacao" HeaderText="Observação" />
                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="50px">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkExcluir" runat="server" CommandName="Delete" CausesValidation="false"
                                                    CssClass="btn btn-danger btn-xs" ToolTip="Excluir" OnClientClick="return confirm('Deseja realmente excluir este lançamento?');">
                                                <i class="fa fa-trash-o"></i>
                                            </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <div class="alert alert-info text-center">Nenhum Centro de Custo adicionado.</div>
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>

                            <hr />
                            <div class="text-right">
                                <button type="button" class="btn btn-success" data-dismiss="modal">Concluir</button>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
    <%-- ======================================================================= --%>
    <%-- │ FIM: MODAL DE RATEIO                                                  │ --%>
    <%-- ======================================================================= --%>

    <div id="dialog-Salvar" class="modal" title="Salvar">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
        </p>
    </div>


    <div id="dialog-Excluir" class="modal" title="Excluir">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloExcluir" runat="server" Text=""></asp:Label>
        </p>
    </div>

    <div id="dialog-Reabrir" class="modal" title="Reabrir">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloReabrir" runat="server" Text=""></asp:Label>
        </p>
    </div>

    <asp:UpdatePanel ID="upd_EnvioArquivos" runat="server">
        <ContentTemplate>
            <div class="modal fade" id="UploadArquivos_Modal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="exampleModalLongTitle">
                                <b>
                                    <asp:Label ID="lblEnviarArquivos_Titulo" runat="server" Text="TITULO"></asp:Label></b></h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
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
                                        <div class="col-lg-12">
                                            <div class="form-group">
                                                <label>
                                                    <asp:Label ID="lblEnviarArquivo_sDscArquivo" runat="server" Text="Observação"></asp:Label></label>
                                                <asp:TextBox ID="txtEnviarArquivo_sDscArquivo" class="form-control" runat="server" MaxLength="300"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-12">
                                            <div class="form-group">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="cmdEnviarArquivos" CssClass="btn-success btn" runat="server" Text="Enviar" OnClick="cmdEnviarArquivos_Click" UseSubmitBehavior="False" />
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
                <asp:HiddenField ID="hddIdLinha" runat="server" />
                <asp:HiddenField ID="hddsBloco" runat="server" />
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdEnviarArquivos" />
        </Triggers>
    </asp:UpdatePanel>

    <div id="observacaoModal" class="modal">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Adicionar Anotação</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <asp:TextBox ID="txtsAnotacao" runat="server" TextMode="MultiLine" Rows="5" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="BtnSalvarObservacao" class="btn btn-success" runat="server" Text="Salvar" OnClick="BtnSalvarObservacao_Click" />
                    <button type="button" class="btn btn-danger" data-dismiss="modal">Fechar</button>
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hddidContasPagar" runat="server" />
    <asp:HiddenField ID="hddidEmpresa" runat="server" />
    <asp:HiddenField ID="hddidAprovacao" runat="server" />
    <asp:HiddenField ID="hddlidCompensaca" runat="server" />

</asp:Content>
