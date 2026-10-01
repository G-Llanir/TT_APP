<%@ Page Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="FaturaCartoes.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Financeiro.FaturaCartoes" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>
<%@ Register Src="~/App/Controles/Pesquisa_Parceiros.ascx" TagPrefix="uc1" TagName="Pesquisa_Parceiros" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .modal-dialog {
            width: 60%;
        }

        .modal-content {
            display: flex;
            flex-direction: column;
            height: auto;
            /*overflow-y: auto;*/
        }

        .modal-body {
            max-height: 850px;
            flex: 1 1 auto;
        }

        .modal-largo {
            width: 80% !important;
            max-width: none !important;
        }

        .modal-header-content {
            display: flex;
            align-items: center;
        }

        .modal-logo img {
            max-width: 50px;
            margin-right: 10px;
        }

        .modal-title-container {
            flex-grow: 1;
        }

        .modal-title {
            color: #009A22;
            text-shadow: 2px 2px 2px rgba(0, 0, 0, 0.2);
            font-size: 24px;
            font-weight: bold;
            margin: 0;
        }

        .gvChildHeader {
            padding: 4px;
            background-color: rgba(0, 155, 0) !important;
            color: white !important;
            border: 1px solid #bbb;
            font-weight: bold;
        }

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

        .gvMainTh{
            padding: 4px;
            background-color: rgba(0, 125, 0) !important;
            color: white !important;
            border: 1px solid #bbb;
            font-weight: bold;
        }

        .gvMainTd{
            padding: 7px;
            border: 1px solid #bbb;
            background-color: #ffffff;
        }

        .gvMainTd tbody tr:hover td {
            background-color: #e0e0e0;
        }

        /*.radioLayout[type="radio"]{
            -webkit-appearance: none; 
            -moz-appearance: none;    
            appearance: none;  
            width: 13px;
            height: 13px;
            border: 2px solid #444;   
            border-radius: 2px;       
            background-color: #fff;  
            cursor: pointer;
        }

            .radioLayout[type="radio"]:checked {
                background-color: green;
                border-color: #444;               
            } */           

    </style>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Fatura Cartões"></asp:Label><small>&nbsp;</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="Fatura Cartões" />
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;Filtro para pesquisa </h3>

                </div>
                <div class="panel-body ">
                    <div class="form-group">

                        <div class="col-lg-1">
                            <div class="form-group">
                                <asp:TextBox ID="txtsPeriodo" runat="server" CssClass="form-control"
                                    Placeholder="Mës/Ano"
                                    Pattern="^(0[1-9]|1[0-2])/([0-9]{4})$"
                                    Title="Insira o período no formato MM/AAAA"
                                    MaxLength="7"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-3">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlidEmpresaFiltro" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-3">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlidConta" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                            </div>
                        </div>

                        <%--<div class="col-lg-3">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlidCartao" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                            </div>
                        </div>--%>

                        <div class="col-lg-2">
                            <div class="form-group">
                                <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" />
                                <asp:Button ID="cmdFatura" class="btn btn-success" runat="server" Text="Gerar Fatura" OnClick="cmdFatura_Click" />

                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaGrid" />
                    <asp:Panel ID="pnResultado" class="" runat="server">

                        <div id="div_Lancamentos" runat="server">
                            <div class="panel panel-primary">
                                <div class="panel-body">
                                    <div class="row table-responsive">
                                        <div class="col-lg-12">
                                            <asp:GridView ID="gv_FaturaCartao" runat="server" class="table table-striped table-bordered table-hover tablePai" DataKeyNames="idFatura, idCartao"
                                                AutoGenerateColumns="False" GridLines="None" ShowFooter="False"
                                                Font-Names="Tahoma" Font-Overline="False" Font-Size="Small"
                                                OnRowDataBound="gv_FaturaCartao_RowDataBound">
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <input type="radio" class="radioLayout" name="FaturaGroup" value='<%# Eval("idFatura") %>' />
                                                            <asp:LinkButton ID="btnToggle" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-div-id='<%# "tr_1_Fatura" + string.Format("{0}-{1}", Eval("idFatura"),Eval("idCartao")) %>'></asp:LinkButton>
                                                        </ItemTemplate>
                                                        <ItemStyle Width= "2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:TemplateField>

                                                    <asp:BoundField DataField="MesAno" HeaderText="Mês/Ano">
                                                        <ItemStyle Width="10%" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sBanco" HeaderText="Banco">
                                                        <ItemStyle Width="20%" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sEmpresa" HeaderText="Empresa">
                                                        <ItemStyle Width="15%" />
                                                    </asp:BoundField>

                                                    <asp:TemplateField HeaderText="Total da Fatura">
                                                        <ItemTemplate>
                                                            <asp:Label ID="nTotalFatura" runat="server" Text='<%# Eval("nTotalFatura", "{0:C}") %>'></asp:Label>
                                                            <%# NovaLinha(string.Format("{0}-{1}", Eval("idFatura"), Eval("idCartao")), "_1_Fatura") %>

                                                            <asp:GridView ID="gv_FaturaDetalhe" runat="server" Width="100%" class="table table-striped table-bordered table-hover tablePai"
                                                                GridLines="None" AutoGenerateColumns="false" Style="padding: 0; margin: 0"
                                                                AlternatingRowStyle-CssClass="gvAltRow" OnRowDataBound="gv_FaturaDetalhe_RowDataBound">
                                                                <Columns>

                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <asp:LinkButton ID="btnToggle2" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-div-id='<%# "tr_2_Fatura" + string.Format("{0}-{1}", Eval("idFatura"),Eval("idCartao")) %>'></asp:LinkButton>
                                                                        </ItemTemplate>
                                                                        <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                    </asp:TemplateField>

                                                                    <asp:BoundField DataField="sDscCartao" HeaderText="Número Cartão">
                                                                        <ItemStyle Width="30%" />
                                                                    </asp:BoundField>

                                                                    <asp:TemplateField HeaderText="Total dos Lançamentos">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="nTotalLancamento" runat="server" Text='<%# Eval("nTotalLancamento", "{0:C}") %>'></asp:Label>
                                                                            <%# NovaLinha(string.Format("{0}-{1}", Eval("idFatura"), Eval("idCartao")), "_2_Fatura") %>

                                                                            <asp:GridView ID="gv_CartaoDetalhe" runat="server" Width="100%" class="table table-striped table-bordered table-hover tablePai"
                                                                                GridLines="None" AutoGenerateColumns="false" Style="padding: 0; margin: 0"
                                                                                AlternatingRowStyle-CssClass="gvAltRow" OnRowDataBound="gv_CartaoDetalhe_RowDataBound">
                                                                                <Columns>

                                                                                    <asp:BoundField DataField="dtLancamento" HeaderText="Data Lançamento">
                                                                                        <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                    </asp:BoundField>

                                                                                    <asp:BoundField DataField="sDscLancamento" HeaderText="Descrição Lançamento">
                                                                                        <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                    </asp:BoundField>

                                                                                    <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário Despesa">
                                                                                        <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                    </asp:BoundField>

                                                                                    <asp:BoundField DataField="nValor" HeaderText="Valor" DataFormatString="R$ {0:N2}">
                                                                                        <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                    </asp:BoundField>

                                                                                </Columns>
                                                                            </asp:GridView>
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
                        </div>

                    </asp:Panel>

                </div>
            </div>

        </div>

    </div>

    <asp:HiddenField runat="server" ID="hddidLancamento" Value="" />
    <asp:HiddenField runat="server" ID="hddidCartao" Value="" />

    <%--------------------------------------MODAL IMPORTAR EXTRATO---------------------------------------------------------------------------------------%>
    <div class="modal fade" id="modalGerarFatura" tabindex="-2" role="dialog" data-backdrop="static" aria-labelledby="modalGerarFatura" aria-hidden="true">
        <div class="modal-dialog modal-sm" role="document" style="width: 60%">
            <div class="modal-content">
                <asp:UpdatePanel ID="updModalDetalhe" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" id="btnHeadFechar" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <div class="modal-header-content">
                                <div class="modal-logo">
                                    <asp:Image ID="Image1" runat="server" ImageUrl="~/App/img/instrucaoTecnica_pdf.jpg" AlternateText="Instrução Técnica" />
                                </div>

                                <div class="modal-title-container">
                                    <h4 class="modal-title">Gerar Fatura</h4>
                                </div>
                            </div>
                            <div class="modal-body">
                                <div class="panel panel-default" runat="server" id="div_Detalhe">

                                    <div class="panel-body ">
                                        <div class="col-lg-12 row" style="width: 100%; margin-left: 0px; padding: 0px;">
                                            <asp:UpdatePanel ID="uptPanelImportar" runat="server">
                                                <ContentTemplate>
                                                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaModal" style="margin-left: 0px !important" />
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>

                                        <div class="panel panel-default">
                                            <div class="panel-body ">

                                                <div class="form-group">
                                                    <div class="col-lg-12">
                                                        <div class="row">
                                                            <div class="col-lg-7">
                                                                <div class="form-group">
                                                                    <label>Categoria</label>
                                                                    <asp:DropDownList ID="ddlidCategoriaPagar" class="form-control Caixa_Selecao" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidCategoriaPagar_OnSelectedIndexChanged"></asp:DropDownList>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <div class="col-lg-12">
                                                        <div class="row">

                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Empresa</label>
                                                                    <asp:DropDownList ID="ddlidEmpresa" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-4">
                                                                <div class="form-group">
                                                                    <label>Referência</label>
                                                                    <asp:TextBox ID="txtsReferencia" class="form-control" runat="server" MaxLength="100"></asp:TextBox>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-5">
                                                                <div class="form-group">
                                                                    <label>Credor</label>
                                                                    <asp:DropDownList ID="ddlidParceiro" class="form-control Caixa_Selecao" runat="server" AutoPostBack="true"></asp:DropDownList>
                                                                </div>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <div class="col-lg-12">
                                                        <div class="row">

                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Data Emissão</label>
                                                                    <asp:TextBox ID="txtdtEmissão" type="date" class="form-control" runat="server"></asp:TextBox>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-4">
                                                                <div class="form-group">
                                                                    <label>Nota Fiscal</label>
                                                                    <asp:TextBox ID="txtsCodigo" class="form-control" runat="server"></asp:TextBox>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-5">
                                                                <div class="form-group">
                                                                    <label>Centro de Custo</label>
                                                                    <asp:DropDownList ID="ddlidCentroDeCusto" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <div class="col-lg-12">
                                                        <div class="row">

                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Data Vencimento</label>
                                                                    <asp:TextBox ID="txtdtVencimento" type="date" class="form-control" runat="server"></asp:TextBox>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-4">
                                                                <div class="form-group">
                                                                    <label>Forma de Pagamento</label>
                                                                    <asp:DropDownList ID="ddlidFormaPagamento" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                                                </div>
                                                            </div>



                                                            <div class="col-lg-5">
                                                                <div class="form-group">
                                                                    <label>Código Contábil</label>
                                                                    <asp:DropDownList ID="ddlidContabil" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <div class="col-lg-12">
                                                        <div class="row">

                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Tipo de Pagamento"</label>
                                                                    <asp:DropDownList ID="ddlidMeioPagamento" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-4">
                                                                <div class="form-group">
                                                                    <label>Valor Bruto</label>
                                                                    <asp:TextBox ID="txtnValorBruto" class="form-control" runat="server"></asp:TextBox>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-5">
                                                                <div class="form-group">
                                                                    <label>Valor Líquido</label>
                                                                    <asp:TextBox ID="txtnValorLiquido" class="form-control" runat="server"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>


                                        <div class="modal-footer">
                                            <asp:Button ID="btnSalvar" CssClass="btn-success btn" runat="server" Text="Salvar" OnClick="btnSalvar_Click" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <asp:HiddenField runat="server" ID="hddidFatura" Value="0" />
                        <asp:HiddenField runat="server" ID="hddTipoCategoria" />
                        <asp:HiddenField runat="server" ID="hddUrlTitulo" />


                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>


    <script type="text/javascript">

        function selecionaCheckbox(checkboxSelecionada) {
            console.log("selecionou");
            var checkboxes = document.querySelectorAll('.ccbFatura');
            checkboxes.forEach(function (checkbox) {
                if (checkbox !== checkboxSelecionada) {
                    checkbox.checked = false;
                }
            });
        }

        document.addEventListener('DOMContentLoaded', function () {
            var checkboxes = document.querySelectorAll('.ccbFatura');
            checkboxes.forEach(function (checkbox) {
                checkbox.addEventListener('click', function () {
                    selecionaCheckbox(this);
                });
            });
        });

        function refreshPagina() {
            $('#modalGerarFatura').on('hidden.bs.modal', function () {
                var url = '/App/Paginas/Adm/Financeiro/FaturaCartoes.aspx';
                window.location.href = url;
            });
            $('#modalGerarFatura').modal('hide');

        }

        function recarregaEvento() {
            document.getElementById('btnHeadFechar').addEventListener('click', function () {
                refreshPagina();
            });
        }

        recarregaEvento();

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            recarregaEvento();
        });

    </script>
</asp:Content>
