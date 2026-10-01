<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Colaboradores_Pagamento.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.Colaboradores_Pagamento" ResponseEncoding="utf-8" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <style type="text/css">
        #modalGerarCP .modal-dialog {
            width: 60%;
            max-width: 920px;
        }
        #modalGerarCP .modal-content,
        #modalGerarCP .modal-body {
            overflow: visible !important;
        }
        #modalGerarCP .modal-body {
            min-height: 320px;
            padding-bottom: 30px;
        }
        #modalGerarCP .chosen-container {
            width: 100% !important;
        }
        #modalGerarCP .chosen-container .chosen-drop {
            z-index: 2000;
        }
    </style>

    <script type="text/javascript">
        function MudarBotao() {
            var btn = document.getElementById('<%= cmdPesquisar.ClientID %>');
            if (btn) btn.value = 'Pesquisando...';
        }
        function limparBackdropModal() {
            $('.modal-backdrop').remove();
            $('body').removeClass('modal-open');
            $('body').css({ overflow: '', paddingRight: '' });
        }
        function inicializarChosenModalGerarCP() {
            var $modal = $('#modalGerarCP');
            $modal.find('.Caixa_Selecao').each(function () {
                var $el = $(this);
                if ($el.data('chosen')) {
                    $el.chosen('destroy');
                }
                $el.chosen({ width: '100%', disable_search_threshold: 10 });
            });
        }
        function openModalGerarCP(idPagamento, referencia, total) {
            document.getElementById('<%= hddidPagamentoGerar.ClientID %>').value = idPagamento;
            $('#modalGerarCP').data('idPagamento', idPagamento);
            var msg = document.querySelector('#modalGerarCP .modal-message');
            if (msg) msg.innerText = 'Confirma gerar Contas a Pagar para o pagamento ' + referencia + ' no valor total de R$ ' + total + '?';
            limparBackdropModal();
            $('#modalGerarCP').modal({ backdrop: 'static', keyboard: false, show: true });
            setTimeout(inicializarChosenModalGerarCP, 200);
        }
        function prepararPostConfirmarGerarCP() {
            var hid = document.getElementById('<%= hddidPagamentoGerar.ClientID %>');
            var backup = $('#modalGerarCP').data('idPagamento');
            if (hid && (!hid.value || hid.value === '0') && backup) {
                hid.value = backup;
            }
            $('#modalGerarCP select.Caixa_Selecao').each(function () {
                var $el = $(this);
                if ($el.data('chosen')) {
                    $el.chosen('destroy');
                }
            });
            return true;
        }
        function fecharModalGerarCP() {
            $('#modalGerarCP').modal('hide');
            limparBackdropModal();
        }
        $(function () {
            $('#modalGerarCP').on('hidden.bs.modal', function () {
                limparBackdropModal();
            });
        });
    </script>

    <asp:UpdatePanel runat="server" ID="UpdGeral" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Pagamentos de Colaboradores"></asp:Label>
                        <small><asp:Label ID="lblSubTituloPagina" runat="server" Text=" Consulta"></asp:Label></small>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>

                <div class="col-lg-12">
                    <div class="panel panel-primary" runat="server" id="DIV_Filtro">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa</h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-lg-2 form-group">
                                    <label>Data Inicial</label>
                                    <asp:TextBox ID="txtdtInicio" class="form-control" runat="server" TextMode="Date"></asp:TextBox>
                                </div>
                                <div class="col-lg-2 form-group">
                                    <label>Data Final</label>
                                    <asp:TextBox ID="txtdtFinal" class="form-control" runat="server" TextMode="Date"></asp:TextBox>
                                </div>
                                <div class="col-lg-2 form-group">
                                    <label>Tipo</label>
                                    <asp:DropDownList ID="ddlidTipo" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-lg-3 form-group">
                                    <label>Empresa</label>
                                    <asp:DropDownList ID="ddlidEmpresa" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-lg-3 form-group">
                                    <label>Situa&ccedil;&atilde;o</label>
                                    <asp:DropDownList ID="ddlidSituacao" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-12 form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                    <asp:HyperLink ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" NavigateUrl="Colaboradores_Pagamento_Detalhe.aspx?id=0"></asp:HyperLink>
                                </div>
                            </div>
                        </div>
                    </div>

                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                    <asp:Panel ID="pnResultado" runat="server" Visible="false">
                        <div class="panel panel-primary">
                            <div class="panel-body table-responsive">
                                <asp:GridView ID="dtgvConsulta" runat="server" CssClass="table table-striped table-bordered table-hover"
                                    AutoGenerateColumns="False" DataKeyNames="idPagamento" Width="100%"
                                    OnRowDataBound="dtgvConsulta_RowDataBound">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Compet&ecirc;ncia">
                                            <ItemTemplate>
                                                <asp:HyperLink ID="lnkEditar" runat="server"
                                                    NavigateUrl='<%# "Colaboradores_Pagamento_Detalhe.aspx?id=" + Eval("idPagamento") %>'
                                                    Text='<%# Eval("sReferencia") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="sDscTipo" HeaderText="Tipo" />
                                        <asp:BoundField DataField="dtEmissao" HeaderText="Data Emiss&atilde;o" />
                                        <asp:BoundField DataField="dtVencimento" HeaderText="Data Vencimento" />
                                        <asp:BoundField DataField="sDscEmpresa" HeaderText="Empresa" />
                                        <asp:BoundField DataField="nQtdColaboradores" HeaderText="Qtd. Colaboradores" />
                                        <asp:BoundField DataField="nValorTotal" HeaderText="Total" DataFormatString="{0:N2}" />
                                        <asp:BoundField DataField="sDscSituacao" HeaderText="Situa&ccedil;&atilde;o" />
                                        <asp:TemplateField HeaderText="A&ccedil;&otilde;es">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnGerarCP" runat="server" CssClass="btn btn-info btn-sm"
                                                    Text="Gerar Contas a Pagar"
                                                    OnClientClick='<%# GetGerarCpScript(Eval("idPagamento"), Eval("sReferencia"), Eval("nValorTotal"), Eval("idSituacao")) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="modal fade" id="modalGerarCP" tabindex="-1" role="dialog" data-backdrop="static" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <asp:UpdatePanel runat="server" ID="updModalGerarCP" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlidCategoriaPagar" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlidMeioPagamento" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtnParcelas" EventName="TextChanged" />
                        <asp:PostBackTrigger ControlID="btnConfirmarGerarCP" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:HiddenField ID="hddidPagamentoGerar" runat="server" Value="0" />
                        <asp:HiddenField ID="hddTipoCategoria" runat="server" Value="0" />
                        <div class="modal-header">
                            <button type="button" class="close" onclick="fecharModalGerarCP(); return false;" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <h4 class="modal-title">Gerar Contas a Pagar</h4>
                        </div>
                        <div class="modal-body">
                            <p class="modal-message text-muted"></p>
                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaModal" />
                            <div class="form-group">
                                <label>Categoria (Internos)</label>
                                <asp:DropDownList ID="ddlidCategoriaPagar" class="form-control Caixa_Selecao" runat="server"
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlidCategoriaPagar_SelectedIndexChanged"></asp:DropDownList>
                            </div>
                            <div id="div_camposModal" runat="server" visible="false">
                                <div class="row">
                                    <div class="col-lg-6 form-group">
                                        <label>Forma de Pagamento</label>
                                        <asp:DropDownList ID="ddlidFormaPagamento" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                    </div>
                                    <div class="col-lg-6 form-group">
                                        <label>Tipo de Pagamento</label>
                                        <asp:DropDownList ID="ddlidMeioPagamento" class="form-control Caixa_Selecao" runat="server"
                                            AutoPostBack="true" OnSelectedIndexChanged="ddlidMeioPagamento_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row" id="Div_nParcelas" runat="server" visible="false">
                                    <div class="col-lg-4 form-group">
                                        <label>N&ordm; parcelas</label>
                                        <asp:TextBox ID="txtnParcelas" class="form-control" runat="server" AutoPostBack="true"
                                            OnTextChanged="txtnParcelas_TextChanged"></asp:TextBox>
                                    </div>
                                </div>
                                <div id="DIV_Lancamentos" runat="server" visible="false" class="form-group">
                                    <label>Datas de vencimento das parcelas</label>
                                    <asp:GridView ID="dtgLancamentoModal" runat="server" CssClass="table table-striped table-bordered table-hover table-condensed"
                                        AutoGenerateColumns="False" Width="100%">
                                        <Columns>
                                            <asp:BoundField DataField="nParcelaLancamentoPag" HeaderText="N&ordm; Parcela" />
                                            <asp:TemplateField HeaderText="Data Vencimento">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtdtLancamentoPag" runat="server" CssClass="form-control" TextMode="Date"
                                                        Text='<%# Bind("dtLancamentoPag") %>'></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <p class="text-muted small">Os valores ser&atilde;o divididos igualmente por colaborador na gera&ccedil;&atilde;o (a &uacute;ltima parcela ajusta centavos).</p>
                                </div>
                                <div class="form-group">
                                    <label>C&oacute;digo Cont&aacute;bil</label>
                                    <asp:DropDownList ID="ddlidContabil" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" onclick="fecharModalGerarCP(); return false;">Cancelar</button>
                            <asp:Button ID="btnConfirmarGerarCP" runat="server" CssClass="btn btn-primary" Text="Confirmar"
                                OnClick="btnConfirmarGerarCP_Click" OnClientClick="return prepararPostConfirmarGerarCP();" Visible="false" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
</asp:Content>
