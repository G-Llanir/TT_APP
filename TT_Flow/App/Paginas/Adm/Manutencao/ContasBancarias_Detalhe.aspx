<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="ContasBancarias_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Manutencao.ContasBancarias_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">


    <style>
        .modal-dialog {
            width: 60%;
        }

        .modal-content {
            display: flex;
            flex-direction: column;
            height: auto;
            overflow: unset;
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
    </style>

</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>



    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Conta Bancária"></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>
    </div>
    <div>
        <ul id="tab_ContaBancaria" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="tabpanel active">
                <a href="#contaBancaria" id="aba_ContaBancaria" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Conta Bancária</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_ProdutosFinanceiros">
                <a href="#ProdutosFinanceiros" role="tab" id="ProdutosFinanceiros-tab" data-toggle="tab" aria-controls="ProdutosFinanceiros"><b>Produtos Financeiros</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_Historico">
                <a href="#historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="Historico"><b>Histórico</b></a>
            </li>
        </ul>
    </div>


    <div id="tab" class="tab-content">

        <div role="tabpanel" class="tab-pane fade in active" id="contaBancaria" aria-labelledby="contaBancaria-tab">
            <br />
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>

                    <div class="panel panel-default" runat="server" id="div_ContaBancaria">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Detalhe</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>ID</label>
                                        <asp:TextBox ID="txtidConta" class="form-control CaixaTextoMini" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Descrição da Conta</label>
                                        <asp:TextBox ID="txtsDsConta" class="form-control " MaxLength="100" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Empresa </label>
                                        <asp:DropDownList ID="ddlidEmpresa" class="form-control " runat="server" attrname="idConta"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Banco </label>
                                        <asp:DropDownList ID="ddlidBanco" class="form-control " runat="server" attrname="idConta"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Tipo de Conta </label>
                                        <asp:DropDownList ID="ddlsTipoConta" runat="server" class="form-control yes_no select">
                                            <asp:ListItem Value="Conta Corrente">Conta Corrente</asp:ListItem>
                                            <asp:ListItem Value="Poupança">Poupança</asp:ListItem>
                                             <asp:ListItem Value="Conta Salário">Conta Salário</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Número da Agência</label>
                                        <asp:TextBox ID="txtsAgencia" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Número da Conta</label>
                                        <asp:TextBox ID="txtsNumeroConta" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Chave Pix</label>
                                        <asp:TextBox ID="txtsChavePix" class="form-control " runat="server" MaxLength="30"></asp:TextBox>
                                    </div>
                                </div>


                                <div class="col-lg-6">
                                    <div class="form-stacked row">
                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label>Saldo Inicial</label>
                                                <asp:TextBox ID="txtnValorIncial" class="form-control " runat="server" MaxLength="12"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label>Saldo Atual</label>
                                                <asp:TextBox ID="txtnValorAtual" class="form-control " runat="server" MaxLength="12"></asp:TextBox>
                                            </div>
                                        </div>


                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label>Código Contabíl </label>
                                                <asp:DropDownList ID="ddlidLancamentoContabol" class="form-control " runat="server" attrname="idConta"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                    <div class="panel panel-default" runat="server" id="Div_Tarifas">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Informações sobre Tarifas</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group row">
                                <div class="col-lg-6 ">
                                    <div class="form-stacked row">
                                        <div class="col-lg-8">
                                            <div class="form-group">
                                                <label>Descrição do Pacote de Tarifas</label>
                                                <asp:TextBox ID="txtsNomePacoteTarifa" class="form-control " runat="server" MaxLength="100"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label>Tarifa Mensal</label>
                                                <asp:TextBox ID="txtnValorMensalTarifa" class="form-control" runat="server" MaxLength="12"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Observações</label>
                                        <asp:TextBox ID="txtsObservacao" class="form-control " runat="server" MaxLength="300" TextMode="MultiLine" Height="120px"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>


                    <div class="panel panel-default" runat="server" id="DIV_Contatos">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Contatos</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemContatos" />
                                </div>
                                <div>
                                    <div class="col-lg-3">
                                        <div class="form-group">
                                            <label>Nome Contato </label>
                                            <asp:TextBox ID="txtContatos_sDscContato" class="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Telefone</label>
                                            <asp:TextBox ID="txtContatos_sTelefoneContato" class="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>E-mail </label>
                                            <asp:TextBox ID="txtContatos_sEmailContato" class="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <br />
                                            <asp:Button ID="cmdContatos_Incluir" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdContatos_Incluir_Click" />
                                        </div>
                                    </div>
                                    <br />
                                </div>

                                <div class="col-lg-12">
                                    <asp:GridView ID="dtgContatos" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDeleting="dtgContatos_RowDeleting">
                                        <Columns>
                                            <asp:BoundField DataField="sDscContato" HeaderText="Nome">
                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="sTelefoneContato" HeaderText="Telefone">
                                                <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="sEmailContato" HeaderText="E-mail">
                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkProdutosFinanceiros_Excluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
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

        <div role="tabpanel" class="tab-pane fade" id="ProdutosFinanceiros" aria-labelledby="ProdutosFinanceiros-tab">
            <br />
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>

                    <div class="panel panel-default" runat="server" id="DIV_ProdutosFinanceiros">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Produtos Financeiros</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemProdutosFinanceiros" />
                                </div>
                                <div class="col-lg-7">
                                    <div class="form-group row">
                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label>Tipo Conta </label>
                                                <asp:DropDownList ID="ddlProdutosFinanceiros_idTipoConta" class="form-control " runat="server" attrname="idConta"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-3">
                                            <div class="form-group">
                                                <label>Limite Conta </label>
                                                <asp:TextBox ID="txtProdutosFinanceiros_nLimiteConta" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-5">
                                            <div class="form-group">
                                                <label>Garantia </label>
                                                <asp:TextBox ID="txtProdutosFinanceiros_sGarantias" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-5">
                                    <div class="form-group row">

                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label>Data Início </label>
                                                <asp:TextBox ID="txtProdutosFinanceiros_dtInicio" class="form-control " runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label>Data Vencimento </label>
                                                <asp:TextBox ID="txtProdutosFinanceiros_dtVencimento" class="form-control " runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label>Taxa %</label>
                                                <asp:TextBox ID="txtProdutosFinanceiros_nPorcentagemTaxaConta" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <asp:Button ID="cmdProdutosFinanceiros_Incluir" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdProdutosFinanceiros_Incluir_Click" />
                                    </div>
                                </div>
                                <br />
                            </div>

                            <div class="col-lg-12 row">
                                <asp:GridView ID="dtgProdutosFinanceiros" class="table table-striped table-bordered table-hover table-condensed"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDeleting="dtgProdutosFinanceiros_RowDeleting">
                                    <Columns>
                                        <asp:BoundField DataField="sDscTipoConta" HeaderText="Tipo">
                                            <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="nLimiteConta" DataFormatString="{0:C2}" HeaderText="Limite Conta">
                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="dtInicio" HeaderText="Inicio">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="dtVencimento" HeaderText="Vencimento">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="nPorcentagemTaxaConta" DataFormatString="{0:N2}" HeaderText="Taxa %">
                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkProdutosFinanceiros_Excluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                            </ItemTemplate>
                                            <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>

                        </div>
                    </div>


                    <div class="panel panel-default" runat="server" id="Div_Cartoes">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Cartões</b></h3>
                        </div>

                        <div class="panel-body">
                            <div class="form-stacked row">

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <asp:Button ID="cmdIncluirCartoes" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Novo Cartão" OnClick="cmdIncluirCartoes_Click" />
                                    </div>
                                </div>
                                <div class="col-lg-12">
                                    <div class="form-group">

                                        <asp:HiddenField ID="hddCartaoCredito_idLinha" runat="server" />
                                        <div class="row table-responsive">
                                            <div class="col-lg-12">

                                                <asp:GridView ID="dtgCartaoCredito" class="table table-striped table-bordered table-hover table-condensed"
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="11px"
                                                    OnRowDataBound="dtgCartaoCredito_RowDataBound" OnRowDeleting="dtgCartaoCredito_RowDeleting" OnRowCommand="dtgCartaoCredito_RowCommand">
                                                    <Columns>

                                                        <asp:BoundField DataField="idLinha" HeaderText="idLinha">
                                                            <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="nNumCartao" HeaderText="Número Cartão">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sTitularCartao" HeaderText="Titular">
                                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sDscBandeira" HeaderText="Bandeira">
                                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="nLimiteCartao" HeaderText="Limite" DataFormatString="{0:C}" HtmlEncode="False">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="dtVenciCartao" HeaderText="Validade">
                                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="dtCorteFatura" HeaderText="Corte Fatura">
                                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="nProvisaoGastos" HeaderText="Provisão Gastos" DataFormatString="{0:C}" HtmlEncode="False">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sDscUsuario" HeaderText="Responsável">
                                                            <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sStatus_Completo" HeaderText="Status">
                                                            <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkLancamento" CommandArgument='<%# Bind("idLinha") %>' runat="server" class="btn btn-sm btn-warning" TabIndex="100" CommandName="Lancamento" data-toggle="tooltip" title="Lançamentos"><i class="fa fa-list-ul"></i></asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkCartoesEditar" CommandArgument='<%# Bind("idLinha") %>' runat="server" ToolTip="Editar" CssClass="btn btn-small" TabIndex="100" CommandName="Editar"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkCartoes_UpLoad" CommandArgument='<%# Bind("idLinha") %>' runat="server" ToolTip="Efetuar Upload de Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Upload_Arquivo" Visible="false"><i class="fa-upload fa"></i></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkCartoes_Download" CommandArgument='<%# Bind("idLinha") %>' runat="server" ToolTip='<%# Bind("sNomeArquivo", "Download do Arquivo: {0}") %>' CssClass="btn btn-small" TabIndex="100" CommandName="Download_Arquivo" Visible="false"><i class="fa-file fa"></i></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkCartoes_Excluir" CommandArgument='<%# Bind("idLinha") %>' runat="server" CssClass="btn btn-small" TabIndex="100" ToolTip="Apagar Registro" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
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

                </ContentTemplate>
            </asp:UpdatePanel>

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
                                <Columns>
                                    <asp:BoundField DataField="sDscBanco" HeaderText="Banco">
                                        <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscConta" HeaderText="Descrição">
                                        <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sTipoConta" HeaderText="Tipo de Conta">
                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sAgencia" HeaderText="Agência">
                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sNumeroConta" HeaderText="Conta">
                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="nValorIncial" DataFormatString="{0:C2}" HeaderText="Inicial">
                                        <ItemStyle Width="8%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>

                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>


    <fieldset class="form-stacked actions">
        <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" ValidationGroup="DETALHE" />
        <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field-cancel" title="Voltar" onclick="history.go(-2)">
    </fieldset>


    <div id="dialog-Salvar" class="modal" title="Salvar">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
        </p>
    </div>

    <asp:HiddenField ID="hddidConta" runat="server" />



    <%----------------------------------------------------------------------------------------------------------------------------------------------%>




    <%----------------------------------------%>
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

            <!-- Modal /-->
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdEnviarArquivos" />

        </Triggers>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="Cartoes_UpdPanel" runat="server">
        <ContentTemplate>
            <div class="modal fade" id="modal_Cartoes" tabindex="-1" role="dialog" data-backdrop="static" aria-labelledby="modal_Cartoes_Label" aria-hidden="true">
                <div class="modal-dialog modal-lg" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title" id="modal_Cartoes_Label"><b>
                                <asp:Label ID="Cartoes_lblTitulo" runat="server" Text="Cartões"></asp:Label></b>                                
                            </h4>
                        </div>
                        <div class="modal-body">
                            <div class="col-lg-12 row">
                                <uc1:MensagemPagina runat="server" ID="Cartoess_MensagemPagina" />
                            </div>

                            <div class="row">
                                <div class="col-lg-3" runat="server" id="div_nNumCarta">
                                    <div class="form-group">
                                        <label>Número </label>
                                        <asp:TextBox ID="txtnNumCartao" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-4" runat="server" id="div_sTitularCartao">
                                    <div class="form-group">
                                        <label>Titular</label>
                                        <asp:TextBox ID="txtsTitularCartao" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3" runat="server" id="div_idBandeirao">
                                    <div class="form-group">
                                        <label>Bandeira</label>
                                        <asp:DropDownList ID="ddlidBandeira" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="div_nLimiteCartao">
                                    <div class="form-group">
                                        <label>Limite </label>
                                        <asp:TextBox ID="txtnLimiteCartao" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3" runat="server" id="div_dtVenciCartao">
                                    <div class="form-group">
                                        <label>Validade</label>
                                        <asp:TextBox ID="txtdtVenciCartao" class="form-control " runat="server" type="date"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3" runat="server" id="div_dtCorteFatura">
                                    <div class="form-group">
                                        <label>Data Corte Fatura</label>
                                        <asp:TextBox ID="txtdtCorteFatura" class="form-control " runat="server" type="date"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3" runat="server" id="div_dtVencimento">
                                    <div class="form-group">
                                        <label>Data Vencimento</label>
                                        <asp:TextBox ID="txtdtVencimento" class="form-control " runat="server" type="date"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="div_nProvisaoGastos">
                                    <div class="form-group">
                                        <label>Provisão Mensal</label>
                                        <asp:TextBox ID="txtnProvisaoGastos" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-6" runat="server" id="div_idUsuario">
                                    <div class="form-group">
                                        <label>Responsável</label>
                                        <asp:DropDownList ID="ddlidUsuario" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="div_sStatus">
                                    <div class="form-group">
                                        <label>Status</label>
                                        <asp:DropDownList ID="ddlsStatus" runat="server" class="form-control">
                                            <asp:ListItem Value="A" Selected="True">Ativo</asp:ListItem>
                                            <asp:ListItem Value="I">Inativo</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton CssClass="btn btn-info" ID="cmdCartao_Salvar" Text="Salvar" runat="server" OnClick="cmdCartao_Salvar_Click" />
                            <asp:LinkButton CssClass="btn btn-danger" ID="cmdCartoes_Cancelar" Text="Cancelar" runat="server" OnClick="Modal_Fechar" />
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>




</asp:Content>
