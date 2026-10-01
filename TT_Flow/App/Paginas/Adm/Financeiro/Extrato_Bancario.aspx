<%@ Page Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Extrato_Bancario.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Financeiro.Extrato_Bancario" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>
<%@ Register Src="~/App/Controles/Pesquisa_Parceiros.ascx" TagPrefix="uc1" TagName="Pesquisa_Parceiros" %>
<%@ Register Src="~/App/Controles/Pesquisa_Colaborador.ascx" TagPrefix="uc1" TagName="Pesquisa_Colaborador" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link rel="stylesheet" href="https://fonts.googleapis.com/icon?family=Material+Icons" />

    <style>
        .modal-dialog {
            width: 60%;
        }

        .modal-content {
            display: flex;
            flex-direction: column;
            height: auto;
            overflow-y: auto;
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
            fill: rgb(82, 82, 82);
            margin-bottom: 20px;
        }

        .file-upload-label {
            cursor: pointer;
            background-color: #ddd;
            padding: 30px 70px;
            border-radius: 40px;
            border: 2px dashed rgb(82, 82, 82);
            box-shadow: 0px 0px 20px -50px rgba(0, 0, 0, 0.719);
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            gap: 5px;
            text-align: center;
        }

            .file-upload-label.dragover {
                border-color: #000;
                background-color: #e6f7ff;
            }

        .browse-button {
            background-color: rgb(82, 82, 82);
            padding: 5px 15px;
            border-radius: 10px;
            color: white;
            transition: all 0.3s;
        }

            .browse-button:hover {
                background-color: rgb(14, 14, 14);
            }

        .file-name {
            margin-top: 10px;
            font-size: 14px;
            color: rgb(82, 82, 82);
        }

        .material-icons {
            font-size: 27px;
        }

        .tall {
            height: 90%;
        }

        .scrollable-grid {
            overflow-x: auto;
        }

        div#cphCorpo_gv_ConciliacaoComposta_filter {
            margin-right: 20px;
        }

        #cphCorpo_dtgvConsultaExtrato tr.linha-conciliada-destaque > td {
            background-color: #dff0d8 !important;
        }

        #cphCorpo_dtgvConsultaExtrato tr.linha-conciliada-destaque:focus {
            outline: 2px solid #3c763d;
            outline-offset: 2px;
        }
    </style>

    <script>
        function disableButton(button) {
            button.disabled = true;
        }

        function iniciarExportacaoExtrato() {
            document.cookie = 'DownloadExtratoConcluido=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/';

            var downloadMonitor = window.setInterval(function () {
                if (document.cookie.indexOf('DownloadExtratoConcluido=1') !== -1) {
                    window.clearInterval(downloadMonitor);
                    document.cookie = 'DownloadExtratoConcluido=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/';
                    esconderLoadingExportacao();
                }
            }, 250);

            window.setTimeout(function () {
                window.clearInterval(downloadMonitor);
                esconderLoadingExportacao();
            }, 60000);
        }

        function esconderLoadingExportacao() {
            $('#div_Loading').hide();
            $('#div_img').hide();
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Extrato Bancário"></asp:Label>
                <small>Consulta</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />

            <uc1:MensagemPagina runat="server" ID="MensagemPaginaConsulta" />
        </div>
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;Filtro para pesquisa </h3>

                </div>
                <div class="panel-body ">
                    <div class="form-group">
                        <div class="col-lg-12">
                            <div class="form-group row">
                                
                                <div class="col-lg-1" runat="server" id="div_Selecao_Tipo">
                                   <div class="form-group">
                                       <asp:DropDownList ID="ddlTipoConsulta" class="form-control Caixa_Selecao" runat="server" OnSelectedIndexChanged="ddlTipoConsulta_SelectedIndexChanged" AutoPostBack="true">
                                            <asp:ListItem Selected="True" Text="Arquivo" Value="2"></asp:ListItem> 
                                            <asp:ListItem   Text="Lançamento" Value="1"></asp:ListItem>  
                                        </asp:DropDownList>
                                   </div>
                               </div>

                                <div runat="server" id="div_Selecao_Periodo" visible="false">
                                    <div class="col-lg-1">
                                        <div class="form-group">
                                            <asp:TextBox ID="txtdtInicial" class="form-control CaixaTextoData" placeholder="Data Inicial" runat="server" MaxLength="10" type="date" data-mask="00/00/0000"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-1">
                                        <div class="form-group">
                                            <asp:TextBox ID="txtdtFinal" class="form-control CaixaTextoData" placeholder="Data Final" runat="server" MaxLength="10" type="date" data-mask="00/00/0000"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div runat="server" id="div_Selecao_PeriodoMes">
                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <asp:DropDownList ID="ddlMesAnoExtrato" class="form-control Caixa_Selecao" OnSelectedIndexChanged="ddl_SelectedIndexChanged" AutoPostBack="true" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="div_Selecao_Conta">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlConta" class="form-control Caixa_Selecao" OnSelectedIndexChanged="ddl_SelectedIndexChanged" AutoPostBack="true" runat="server"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-1" runat="server" id="Div_Selecao_TipoLancamento">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlTipoLancamento" class="form-control Caixa_Selecao" runat="server" OnSelectedIndexChanged="ddl_SelectedIndexChanged" AutoPostBack="true">
                                            <asp:ListItem Text="Todos" Value="T"></asp:ListItem>
                                            <asp:ListItem Text="Recebimento" Value="R"></asp:ListItem>
                                            <asp:ListItem Text="Pagamento" Value="P"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="div_Selecao_Conciliado">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlConciliado" class="form-control Caixa_Selecao" runat="server" OnSelectedIndexChanged="ddl_SelectedIndexChanged" AutoPostBack="true">
                                            <asp:ListItem Text="Conciliados e Não Conciliados" Value="T"></asp:ListItem>
                                            <asp:ListItem Text="Conciliados" Value="S"></asp:ListItem>
                                            <asp:ListItem Text="Não Conciliados" Value="N"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
<%--                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtsDescricao" class="form-control" placeholder="Lançamento" runat="server"></asp:TextBox>
                                    </div>
                                </div>--%>



                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <asp:Button ID="btnPesquisar" class="btn btn-primary" runat="server" Text="Pesquisar" OnClick="btnPesquisar_Click" />
                                        <asp:Button ID="btnImportar" class="btn btn-success" Style="margin-left: 10px" runat="server" Text="Importar" OnClick="btnImportar_Click" />
                                        <asp:Button ID="btnExportarExcel" class="btn btn-info" runat="server" Text="Exportar Excel" OnClick="btnExportarExcel_Click" OnClientClick="iniciarExportacaoExtrato();" />
                                        <div runat="server" id="div_btnVoltar" visible="false" style="display: inline; margin-left: 5px;">
                                            <asp:Button ID="btnVoltarFiltro" CssClass="btn-warning btn" runat="server" Text="Voltar" OnClick="btnVoltarGrid_Click" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
 

            <div class="row">
                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    <asp:Panel ID="pnResultado" class="" runat="server">

                        <div id="div_detalheExtrato" runat="server" visible="false">
                            <div class="panel panel-primary">
                                <div class="panel-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgvConsultaExtrato" class="table table-striped table-bordered table-hover "
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsultaExtrato_RowDataBound">

                                            <Columns>
<%--                                                <asp:BoundField DataField="Extrato" HeaderText="Extrato" ItemStyle-Width="20%" />--%>
                                                
												<asp:BoundField DataField="dtLancamento" HeaderText="Data" ItemStyle-Width="6%" ItemStyle-HorizontalAlign="Center" />
                                                
                                                <asp:TemplateField HeaderText="Conta" >
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="6%" />
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnsDscConta" runat="server"
                                                            Text='<%# Eval("sDscConta") %>'
                                                            CommandArgument='<%# Eval("idLancamento") %>'
                                                            OnCommand="LinkButton_Command">
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="sCodigoContabil" HeaderText="Acento" ItemStyle-Width="12%" />
                                                
                                                <asp:TemplateField HeaderText="Lançamento">
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="20%" />
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnsDscLancamento" runat="server"
                                                            Text='<%# Eval("sDscLancamento") %>'
                                                            CommandArgument='<%# Eval("idLancamento") %>'
                                                            OnCommand="LinkButton_Command">
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="sTipo" HeaderText="Tipo" ItemStyle-Width="8%" />
                                                <asp:BoundField DataField="nCredito" HeaderText="Crédito" DataFormatString="R$ {0:N2}" ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Right" ItemStyle-ForeColor="Blue" />
                                                <asp:BoundField DataField="nDebito" HeaderText="Débito" DataFormatString="R$ {0:N2}"   ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Right" ItemStyle-ForeColor="Red"/>
                                                <asp:BoundField DataField="sInformacaoContabilidade" HeaderText="Contexto" ItemStyle-Width="28%" />
                                                <asp:TemplateField HeaderText="Conciliado">
                                                    <HeaderStyle Width="4%" />
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="coluna-conciliado" />
                                                    <ItemTemplate>
                                                        <asp:Literal ID="litConciliacao" runat="server"></asp:Literal>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="idLancamento" HeaderText="ID" Visible="false" />
                                                <asp:BoundField DataField="idExtrato" HeaderText="ExtratoID" Visible="false" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                    <div class="form-group">
                                        <asp:Button ID="btnVoltarGrid" CssClass="btn-warning btn" runat="server" Text="Voltar" OnClick="btnVoltarGrid_Click" Visible="false" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div id="div_Extrato" runat="server">
                            <div class="panel panel-primary">
                                <div class="panel-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="gv_Extrato" class="table table-striped table-bordered table-hover "
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">

                                            <Columns>
                                                <asp:BoundField DataField="idExtrato" HeaderText="ID">
                                                    <ItemStyle Width="4%" />
                                                </asp:BoundField>
                                                
                                                <asp:BoundField DataField="dtLancamento" HeaderText="Mês do Extrato">
                                                    <ItemStyle Width="10%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="dtRegistroDoc" HeaderText="Data Importação">
                                                    <ItemStyle Width="10%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="sDscEmpresaReduzida" HeaderText="Empresa">
                                                    <ItemStyle Width="10%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="sDscConta" HeaderText="Conta">
                                                    <ItemStyle Width="10%" />
                                                </asp:BoundField>
                                                <asp:TemplateField HeaderText="Extrato">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnsExtrato" runat="server"
                                                            Text='<%# Eval("Extrato") %>'
                                                            CommandArgument='<%# Eval("idExtrato") %>'
                                                            OnCommand="cmd_Extrato">
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>



                                                <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário Importação">
                                                    <ItemStyle Width="15%" />
                                                </asp:BoundField>


                                                <asp:BoundField DataField="idExtrato" HeaderText="ExtratoID" Visible="false" />

                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>
    

    <asp:HiddenField runat="server" ID="hddidExtrato" Value="" />
    <asp:HiddenField runat="server" ID="hddValido" Value="1" />
    <asp:HiddenField runat="server" ID="hddMudaGrid" Value="0" />

    <%--------------------------------------MODAL IMPORTAR EXTRATO---------------------------------------------------------------------------------------%>
    <div class="modal fade" id="modalImportarExtrato" tabindex="-2" role="dialog" data-backdrop="static" aria-labelledby="modalImportarExtrato" aria-hidden="true">
        <div class="modal-dialog modal-sm" role="document" style="width: 30%">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" id="btnHeadFecharImportar" class="close" data-dismiss="modal" aria-label="Fechar">
                        <span aria-hidden="true">&times;</span>
                    </button>
                    <div class="modal-header-content">
                        <div class="modal-logo">
                            <asp:Image ID="Image1" runat="server" ImageUrl="~/App/img/instrucaoTecnica_pdf.jpg" AlternateText="Instrução Técnica" />
                        </div>

                        <div class="modal-title-container">
                            <h4 class="modal-title">Importar Extrato</h4>
                        </div>
                    </div>
                    <div class="modal-body">
                        <div class="panel panel-default" runat="server" id="div_DetalheNegocio">

                            <div class="panel-body ">
                                <div class="col-lg-12 row" style="width: 100%; margin-left: 0px; padding: 0px;">
                                    <asp:UpdatePanel ID="uptPanelImportar" runat="server">
                                        <ContentTemplate>
                                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaModalImportar" style="margin-left: 0px !important" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>

                                <div class="row" runat="server" id="div_BancoImportar">
                                    <div class="col-lg-12 form-group" style="padding: 0px 30px;" >
                                        <label>Banco</label>
                                        <asp:DropDownList runat="server" ID="ddlBanco" class="form-control Caixa_Selecao">
                                            <asp:ListItem Text="Selecione um Banco" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Banco do Brasil" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Bradesco" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="Itaú" Value="3"></asp:ListItem>
                                            <asp:ListItem Text="Santander" Value="4"></asp:ListItem>
                                            <asp:ListItem Text="Daycoval" Value="5"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row" runat="server" id="div_envioArquivo">
                                    <div class="col-lg-12">
                                        <div class="form-stacked">
                                            <div class="col-lg-12" id="div1" runat="server" style="text-align: center;">
                                                <div class="form-group" style="display: inline-block">
                                                    <div class="upload-container">
                                                        <div class="file-upload-label" id="uploadContainer">
                                                            <svg viewBox="0 0 640 512" height="1em">
                                                                <path
                                                                    d="M144 480C64.5 480 0 415.5 0 336c0-62.8 40.2-116.2 96.2-135.9c-.1-2.7-.2-5.4-.2-8.1c0-88.4 71.6-160 160-160c59.3 0 111 32.2 138.7 80.2C409.9 102 428.3 96 448 96c53 0 96 43 96 96c0 12.2-2.3 23.8-6.4 34.6C596 238.4 640 290.1 640 352c0 70.7-57.3 128-128 128H144zm79-217c-9.4 9.4-9.4 24.6 0 33.9s24.6 9.4 33.9 0l39-39V392c0 13.3 10.7 24 24 24s24-10.7 24-24V257.9l39 39c9.4 9.4 24.6 9.4 33.9 0s9.4-24.6 0-33.9l-80-80c-9.4-9.4-24.6-9.4-33.9 0l-80 80z">
                                                                </path>
                                                            </svg>
                                                            <p>Arraste e solte aqui o Arquivo</p>
                                                            <p>ou</p>
                                                            <span class="browse-button">Escolher Arquivo</span>
                                                            <asp:FileUpload ID="fu_EnviarArquivo" runat="server" Style="display: none;" />
                                                        </div>
                                                    </div>
                                                    <div class="file-name" id="fileName"></div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="row" runat="server" id="div_importarConta" visible="false">
                                <div class="col-lg-12 form-group" >
                                    <div class="col-lg-8">
                                        <div class="form-group">
                                            <label>Conta Bancária</label>
                                            <asp:DropDownList ID="ddlImportarConta" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <asp:Button ID="btnImportarConta" CssClass="btn-success btn" runat="server" Text="Confirmar Conta" OnClick="btnImportarConta_Click" Style="margin-top: 24px;"></asp:Button>
                                        </div>
                                    </div>
                                </div>
                                    </div>


                                <div class="modal-footer" style="text-align: left;">
                                    <div class="form-group">
                                        <div class="col-lg-12">
                                            <div class="row">
                                                <div class="col-lg-6" style="padding: 0px;">
                                                    <div class="form-group">
                                                        <asp:Button ID="cmdEnviarArquivos" CssClass="btn-success btn" runat="server" Text="Enviar" OnClick="cmdEnviarArquivos_Click" UseSubmitBehavior="False" />
                                                        <button type="button" id="btnFecharFooterImportar" class="btn btn-secondary" data-dismiss="modal">Fechar</button>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>


    <%--------------------------------------MODAL LANCAMENTO DETALHE---------------------------------------------------------------------------------------%>
    <div class="modal fade" id="modalLancamentoDetalhe" tabindex="-999" role="dialog" data-backdrop="static" aria-labelledby="modalLancamentoDetalhe" aria-hidden="true">
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
                                    <asp:Image ID="Image2" runat="server" ImageUrl="~/App/img/instrucaoTecnica_pdf.jpg" AlternateText="Instrução Técnica" />
                                </div>

                                <div class="modal-title-container">
                                    <h4 class="modal-title">Lançamento Detalhe</h4>
                                </div>
                            </div>
                            <div class="modal-body">
                                <div class="panel panel-default" runat="server" id="div_detalhe">

                                    <div class="panel-body ">
                                        <div class="col-lg-12 row" style="width: 100%; margin-left: 0px; padding: 0px;">
                                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                <ContentTemplate>
                                                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaModalDetalhe" style="margin-left: 0px !important" />
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>

                                        <div class="form-group">
                                            <div class="col-lg-12">
                                                <div class="row">
                                                    <div class="col-lg-3">
                                                        <div class="form-group">
                                                            <label>Data</label>
                                                            <asp:TextBox ID="txtdtLancamento" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-3">
                                                        <div class="form-group">
                                                            <label>Banco</label>
                                                            <asp:TextBox ID="txtsDscBanco" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label>Descrição do Lançamento</label>
                                                            <asp:TextBox ID="txtsLancamento" class="form-control" runat="server"></asp:TextBox>
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
                                                            <label>Número do Documento</label>
                                                            <asp:TextBox ID="txtsNumeroDocumento" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-3">
                                                        <label>Crédito</label>
                                                        <div class="input-group">
                                                            <span class="input-group-addon" id="sMoedaOrigem" runat="server">R$</span>
                                                            <asp:TextBox ID="txtnCredito" runat="server" CssClass="valor-input form-control" aria-describedby="sMoedaOrigem" Style="z-index: 0;"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-3">
                                                        <label>Débito</label>
                                                        <div class="input-group">
                                                            <span class="input-group-addon" id="Span1" runat="server">R$</span>
                                                            <asp:TextBox ID="txtnDebito" runat="server" CssClass="valor-input form-control" aria-describedby="sMoedaOrigem" Style="z-index: 0;"></asp:TextBox>
                                                        </div>
                                                    </div>


                                                    <div class="col-lg-1" runat="server" id="div_btnExtrato" style="text-align: center">
                                                        <div class="form-group" style="text-align: center">
                                                            <label>Extrato</label>
                                                            <button id="btnExtrato" class="btn" runat="server" onserverclick="btnExtrato_Click" style="padding: revert; color: green;">
                                                                <span class="material-icons">cloud_download</span>
                                                            </button>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2" runat="server" id="div_btnNovoConciliar" style="text-align: center" visible="false">
                                                        <div class="form-group" style="text-align: center">
                                                            <label>Nova Conciliação</label>
                                                            <button id="btnNovoConciliar" class="btn" onserverclick="btnNovoConciliar_Click" runat="server" onclick="disableButton(this);" style="padding: revert; color: green; display: flex; margin-left: 60px;">
                                                                <span class="material-icons">add_circle</span>
                                                            </button>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2" runat="server" id="div_DetalheConciliar" style="text-align: center" visible="false">
                                                        <div class="form-group" style="text-align: center">
                                                            <label>Detalhe Conciliação</label>
                                                            <button id="btnDetalheConciliar" class="btn" onserverclick="btnDetalheConciliar_Click" runat="server" onclick="disableButton(this);" style="padding: revert; color: green; display: flex; margin-left: 60px;">
                                                                <span class="material-icons">list</span>
                                                            </button>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <div class="col-lg-12">
                                                <div class="row">

                                                    <div class="col-lg-6" id="div_txtObservacao" runat="server">
                                                        <div class="form-group">
                                                            <label>Observação</label>
                                                            <asp:TextBox ID="txtsObservacao" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-4">
                                                        <div class="form-group">
                                                            <label>&nbsp;</label>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2" runat="server" id="div_transferencia" style="text-align: center" visible="false">
                                                        <div class="form-group" style="text-align: center">
                                                            <label>Transferência</label>
                                                            <button id="btnTransferencia" class="btn" onserverclick="btnTransferencia_Click" runat="server" onclick="disableButton(this);" style="padding: revert; color: green; display: flex; margin-left: 60px;">
                                                                <span class="material-icons">sync_alt</span>
                                                            </button>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>

                                <div class="panel panel-default" runat="server" id="div_btnTipoConciliacao" visible="false">
                                    <div class="panel-body">
                                        <div class="form-group">
                                            <div class="col-lg-12" style="display: flex; justify-content: center;">
                                                <div class="row">

                                                    <div class="col-lg-6" style="text-align: center">
                                                        <div class="form-group" style="text-align: center">
                                                            <label>Conciliação Única</label>
                                                            <button id="btnConciliacaoUnica" class="btn" onserverclick="btnConciliacaoUnica_Click" runat="server" onclick="disableButton(this);" style="color: green; display: flex; margin-left: 60px;">
                                                                <span class="material-icons">add_box</span>
                                                            </button>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6" style="text-align: center">
                                                        <div class="form-group" style="text-align: center">
                                                            <label>Conciliação Composta</label>
                                                            <button id="btnConciliacaoComposta" type="button" class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" style="height: 41px; color: green;"><span class="material-icons">library_add</span></button>
                                                            <ul class="dropdown-menu" style="margin-top: -10px; margin-left: 70px;">
                                                                <asp:Button ID="btnConciliacaoCompostaNova" class="btn btn-primary btn-block" runat="server" Text="Novos Lançamentos" OnClick="btnConciliacaoCompostaNova_Click" Style="background: darkgreen; border: black;" />
                                                                <li role="separator" class="divider" runat="server" id="btnDivisor3" style="margin: 2.5px 0px"></li>
                                                                <li>
                                                                    <asp:Button ID="btnConciliacaoCompostaExistente" class="btn btn-info btn-block" runat="server" Text="Lançamentos Existentes" OnClick="btnConciliacaoCompostaExistente_Click" Style="background: darkgreen; border: black;" />
                                                            </ul>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>


                                <div class="panel panel-default" runat="server" id="div_NovaConciliacao">
                                    <div class="panel-body ">

                                        <div class="form-group">
                                            <div class="col-lg-12">
                                                <div class="row">
                                                    <div class="col-lg-6" id="div_ddlCategoriaPagar" runat="server">
                                                        <div class="form-group">
                                                            <label>Categoria</label>
                                                            <asp:DropDownList ID="ddlidCategoriaPagar" class="form-control Caixa_Selecao" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidCategoriaPagar_OnSelectedIndexChanged"></asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6" id="div_ddlCategoriaReceber" runat="server">
                                                        <div class="form-group">
                                                            <label>Categoria</label>
                                                            <asp:DropDownList ID="ddlidCategoriaReceber" class="form-control Caixa_Selecao" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidCategoriaReceber_SelectedIndexChanged"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <div class="col-lg-12">
                                                <div class="row">
                                                    <uc1:Pesquisa_Parceiros runat="server" AutoPostBack="true" ID="Pesquisa_Parceiros" SsTipoParceiro="0;" />
                                                    <uc1:Pesquisa_Colaborador runat="server" AutoPostBack="true" ID="Pesquisa_Colaborador" />

                                                    <div class="col-lg-3" id="div_ddlidParceiro" runat="server">
                                                        <div class="form-group">
                                                            <label>Credor</label>
                                                            <asp:DropDownList ID="ddlidParceiro" class="form-control Caixa_Selecao" runat="server" AutoPostBack="true"></asp:DropDownList>
                                                        </div>
                                                    </div>


                                                    <div class="col-lg-3" id="div_ddlEmpresa" runat="server">
                                                        <div class="form-group">
                                                            <label>Empresa</label>
                                                            <asp:DropDownList ID="ddlidEmpresa" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <div class="col-lg-12" id="div_TipoCategoriaPagar" runat="server">
                                                <div class="row">

                                                    <div class="col-lg-3" id="div_dtEmissao" runat="server">
                                                        <div class="form-group">
                                                            <label>Data Emissão</label>
                                                            <asp:TextBox ID="txtdtEmissao" type="date" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-3" id="div_txtdtApuracao" runat="server">
                                                        <div class="form-group">
                                                            <label>Data Apuração</label>
                                                            <asp:TextBox ID="txtdtApuracao" type="date" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-3">
                                                        <div class="form-group">
                                                            <asp:Label runat="server" ID="lblFormaPagamento" Text="Forma de Pagamento" Font-Bold="true"></asp:Label>
                                                            <label>&nbsp;</label>
                                                            <asp:DropDownList ID="ddlidFormaPagamento" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-3" id="div_ddlidCentroCusto" runat="server">
                                                        <div class="form-group">
                                                            <label>Centro de Custo</label>
                                                            <asp:DropDownList ID="ddlidCentroDeCusto" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <div class="col-lg-12" id="div_ddlDisabled" runat="server">
                                                <div class="row">

                                                    <div class="col-lg-3">
                                                        <div class="form-group">
                                                            <asp:Label runat="server" ID="lblsCodigo" Text="Nota Fiscal" Font-Bold="true"></asp:Label>
                                                            <label>&nbsp;</label>
                                                            <asp:TextBox ID="txtsCodigo" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-3">
                                                        <div class="form-group">
                                                            <asp:Label runat="server" ID="lblMeioPagamento" Text="Tipo de Pagamento" Font-Bold="true"></asp:Label>
                                                            <label>&nbsp;</label>
                                                            <asp:DropDownList ID="ddlidMeioPagamento" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label>Código Contábil</label>
                                                            <asp:DropDownList ID="ddlidContabil" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="panel panel-default" runat="server" id="div_gvConciliar">
                                    <div class="panel-body">
                                        <div class="table-responsive">
                                            <asp:GridView ID="gv_conciliar" class="table table-striped table-bordered table-hover "
                                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gv_conciliar_RowDataBound" OnRowCommand="gv_conciliar_RowCommand">

                                                <Columns>
                                                    <asp:TemplateField HeaderText="ID">
                                                        <HeaderStyle Width="5%" />
                                                        <ItemTemplate>
                                                            <asp:HyperLink ID="hlTitulo" runat="server" Text='<%# Eval("idTitulo") %>'></asp:HyperLink>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:BoundField DataField="sRazaoSocial" HeaderText="Parceiro">
                                                        <HeaderStyle Width="30%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sDscCategoria" HeaderText="Categoria">
                                                        <HeaderStyle Width="15%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="dtVencimento" HeaderText="Data Vencimento">
                                                        <HeaderStyle Width="10%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sCodigo" HeaderText="Nota Fiscal">
                                                        <HeaderStyle Width="20%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:TemplateField HeaderText="Conciliar">
                                                        <HeaderStyle Width="15%" />
                                                        <ItemTemplate>
                                                            <asp:Button ID="btnConciliar" runat="server" Text="Confirmar" CommandName="conciliar" CommandArgument='<%# Eval("idTitulo") + ";" + Eval("idRegistro") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:BoundField DataField="idTitulo" HeaderText="ID Titulo" Visible="false">
                                                        <HeaderStyle Width="1%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="idRegistro" HeaderText="ID Registro" Visible="false">
                                                        <HeaderStyle Width="1%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>

                                <div class="panel panel-default" runat="server" id="div_ConciliacaoCompostaNova" visible="false">
                                    <div class="panel-body">
                                        <div class="form-group">
                                            <div class="col-lg-12">
                                                <div class="row">

                                                    <div class="col-lg-3">
                                                        <div class="form-group">
                                                            <label>Data Vencimento</label>
                                                            <asp:TextBox runat="server" type="date" class="form-control" ID="txtdtVencimento"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-3">
                                                        <div class="form-group">
                                                            <label>Valor Bruto</label>
                                                            <div class="input-group">
                                                                <span class="input-group-addon" id="Span4" runat="server">R$</span>
                                                                <asp:TextBox ID="txtnValorBruto" runat="server" CssClass="valor-input form-control" aria-describedby="sMoedaOrigem" Style="z-index: 0;"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-3">
                                                        <div class="form-group">
                                                            <label>Valor Líquido</label>
                                                            <div class="input-group">
                                                                <span class="input-group-addon" id="Span5" runat="server">R$</span>
                                                                <asp:TextBox ID="txtnValorLiquido" runat="server" CssClass="valor-input form-control" aria-describedby="sMoedaOrigem" Style="z-index: 0;"></asp:TextBox>
                                                            </div>
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
                                                            <asp:Label runat="server" ID="lbldtTransacao" Text="Data Pagamento" Font-Bold="true"></asp:Label>
                                                            <label>&nbsp;</label>
                                                            <asp:TextBox runat="server" type="date" class="form-control" ID="txtdtTransacao"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-3">
                                                        <div class="form-group">
                                                            <asp:Label runat="server" ID="lblValorTransacao" Text="Valor Pagamento" Font-Bold="true"></asp:Label>
                                                            <label>&nbsp;</label>
                                                            <div class="input-group">
                                                                <span class="input-group-addon" id="Span6" runat="server">R$</span>
                                                                <asp:TextBox ID="txtnValorTransacao" runat="server" CssClass="valor-input form-control" aria-describedby="sMoedaOrigem" Style="z-index: 0;"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <%--                                                 <div class="col-lg-3">
                                                        <div class="form-group">
                                                            <label>Valor Líquido</label>
                                                            <asp:TextBox runat="server" class="form-control" ID="TextBox3"></asp:TextBox>
                                                        </div>
                                                    </div>--%>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <div class="col-lg-12">
                                                <div class="row">
                                                    <div class="col-lg-6" style="margin-top: 25px;">
                                                        <div class="form-group">
                                                            <asp:Button runat="server" ID="btnProximoTitulo" CssClass="btn btn-primary" Text="Próximo" OnClick="btnProximoTitulo_Click" />
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6" style="display: flex; justify-content: end;">
                                                        <div class="form-group">
                                                            <label>Total dos Títulos</label>
                                                            <asp:TextBox runat="server" class="form-control" ID="txtnValorTotalTitulo"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="panel panel-default" runat="server" id="div_gvConciliacaoComposta" visible="false">
                                    <div class="panel-body">
                                        <div class="table-responsive">
                                            <asp:GridView ID="gv_ConciliacaoComposta" class="table table-striped table-bordered table-hover "
                                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gv_ConciliacaoComposta_RowDataBound" DataKeyNames="idTitulo,idRegistro">

                                                <Columns>

                                                    <asp:TemplateField HeaderText="">
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="cbConciliacaoComposta" runat="server"
                                                                AutoPostBack="False"
                                                                CssClass="chkConciliacao"
                                                                onchange="calculateTotal()" />
                                                        </ItemTemplate>
                                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="ID">
                                                        <HeaderStyle Width="5%" />
                                                        <ItemTemplate>
                                                            <asp:HyperLink ID="hlTitulo" runat="server" Text='<%# Eval("idTitulo") %>'></asp:HyperLink>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:BoundField DataField="dtVencimento" HeaderText="Data Vencimento">
                                                        <HeaderStyle Width="10%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:TemplateField HeaderText="Parceiro">
                                                        <HeaderStyle Width="25%" />
                                                        <ItemTemplate>
                                                            <asp:HyperLink ID="hlParceiro" runat="server" Text='<%# Eval("sRazaoSocial") %>'></asp:HyperLink>
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:TemplateField>

                                                    <asp:BoundField DataField="sDscCategoria" HeaderText="Categoria">
                                                        <HeaderStyle Width="15%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sCodigo" HeaderText="Nota Fiscal">
                                                        <HeaderStyle Width="20%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="dtTransacao" HeaderText="Data Transação">
                                                        <HeaderStyle Width="10%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:TemplateField HeaderText="Valor">
                                                        <HeaderStyle Width="10%" />
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblValor"
                                                                Text='<%# String.Format(System.Globalization.CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", Eval("nValor")) %>'>
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle CssClass="valor" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:TemplateField>

                                                    <asp:BoundField DataField="idTitulo" HeaderText="ID Titulo" Visible="false">
                                                        <HeaderStyle Width="1%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="idRegistro" HeaderText="ID Registro" Visible="false">
                                                        <HeaderStyle Width="1%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                        <br />
                                        <div class="form-group">
                                            <div class="col-lg-12">
                                                <div class="row">

                                                    <div class="col-lg-6" style="padding: 0px; margin-top: 25px;">
                                                        <div class="form-group">
                                                            <asp:Button runat="server" ID="btnConfirmar" CssClass="btn btn-success" Text="Confirmar" OnClick="btnConfirmar_Click" />
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6" style="display: flex; justify-content: end;">
                                                        <div class="form-group">
                                                            <label>Total dos Títulos Selecionados</label>
                                                            <asp:TextBox runat="server" class="form-control" ID="txtnValorTotal"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>


                                <div class="panel panel-default" runat="server" id="div_gvTransferencia">
                                    <div class="panel-body">
                                        <div class="table-responsive">
                                            <asp:GridView ID="gv_transferencia" class="table table-striped table-bordered table-hover "
                                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowCommand="gv_transferencia_RowCommand">

                                                <Columns>
                                                    <asp:BoundField DataField="idLancamento" HeaderText="ID">
                                                        <HeaderStyle Width="5%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sDscConta" HeaderText="Conta">
                                                        <HeaderStyle Width="5%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sDscLancamento" HeaderText="Lançamento">
                                                        <HeaderStyle Width="15%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sNumeroDocumento" HeaderText="Número Documento">
                                                        <HeaderStyle Width="10%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:TemplateField HeaderText="Vínculo">
                                                        <HeaderStyle Width="15%" />
                                                        <ItemTemplate>
                                                            <asp:Button ID="btnVinculoTransferencia" runat="server" Text="Confirmar" CommandName="transferencia" CommandArgument='<%# Eval("idLancamento") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>

                                <div class="panel panel-default" runat="server" id="div_detalheTransferencia">
                                    <div class="panel-heading">
                                        <h3 class="panel-title"><b>Detalhe Transferência</b></h3>
                                    </div>
                                    <div class="panel-body ">

                                        <div class="form-group">
                                            <div class="col-lg-12">
                                                <div class="row">
                                                    <div class="col-lg-3">
                                                        <div class="form-group">
                                                            <label>Data</label>
                                                            <asp:TextBox ID="txtTransferenciaData" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-3">
                                                        <div class="form-group">
                                                            <label>Banco</label>
                                                            <asp:TextBox ID="txtTransferenciaBanco" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label>Descrição do Lançamento</label>
                                                            <asp:TextBox ID="txtTransferenciaDscLancamento" class="form-control" runat="server"></asp:TextBox>
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
                                                            <label>Número do Documento</label>
                                                            <asp:TextBox ID="txtTransferenciaNumeroDocumento" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-3">
                                                        <label>Crédito</label>
                                                        <div class="input-group">
                                                            <span class="input-group-addon" id="Span2" runat="server">R$</span>
                                                            <asp:TextBox ID="txtTransferenciaCredito" runat="server" CssClass="valor-input form-control" aria-describedby="sMoedaOrigem" Style="z-index: 0;"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-3">
                                                        <label>Débito</label>
                                                        <div class="input-group">
                                                            <span class="input-group-addon" id="Span3" runat="server">R$</span>
                                                            <asp:TextBox ID="txtTransferenciaDebito" runat="server" CssClass="valor-input form-control" aria-describedby="sMoedaOrigem" Style="z-index: 0;"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-3" id="div_ObservacaoTransferencia" runat="server">
                                                        <div class="form-group">
                                                            <label>Observação</label>
                                                            <asp:TextBox ID="txtTransferenciaObservacao" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="modal-footer">
                                <asp:Button ID="btnSalvar" CssClass="btn-success btn" runat="server" Text="Salvar" OnClick="btnSalvar_Click" />
                                <button id="btnFecharFooterDetalhe" type="button" class="btn btn-secondary" data-dismiss="modal">Fechar</button>
                            </div>

                            <asp:HiddenField runat="server" ID="hddidLancamento" Value="" />
                            <asp:HiddenField runat="server" ID="hddTipoCategoria" Value="" />
                            <asp:HiddenField runat="server" ID="hddidTitulo" Value="" />
                            <asp:HiddenField runat="server" ID="hddTipoTitulo" Value="" />
                            <asp:HiddenField runat="server" ID="hddidEmpresa" Value="" />
                            <asp:HiddenField runat="server" ID="hddConciliacaoComposta" Value="" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>



    <script>        

        function localizarLinhaLancamento(idLancamento) {
            var grid = document.getElementById('<%= dtgvConsultaExtrato.ClientID %>');
            if (!grid) {
                return null;
            }

            var rows = grid.getElementsByTagName('tr');
            for (var index = 0; index < rows.length; index++) {
                if (rows[index].getAttribute('data-id-lancamento') === idLancamento) {
                    return rows[index];
                }
            }

            return null;
        }

        function focarLinhaLancamento(idLancamento) {
            var targetRow = localizarLinhaLancamento(idLancamento);
            if (!targetRow) {
                return;
            }

            targetRow.setAttribute('tabindex', '-1');
            targetRow.classList.add('linha-conciliada-destaque');
            targetRow.scrollIntoView({ behavior: 'smooth', block: 'center' });
            targetRow.focus({ preventScroll: true });

            window.setTimeout(function () {
                targetRow.classList.remove('linha-conciliada-destaque');
            }, 5000);
        }

        function atualizarLinhaConciliada(idLancamento) {
            var targetRow = localizarLinhaLancamento(idLancamento);
            if (!targetRow) {
                return;
            }

            var reconciliationCell = targetRow.querySelector('.coluna-conciliado');
            if (reconciliationCell) {
                reconciliationCell.innerHTML = "<i class='fa fa-check'></i>";
            }

            if (!$('#modalLancamentoDetalhe').is(':visible')) {
                window.setTimeout(function () {
                    focarLinhaLancamento(idLancamento);
                }, 100);
            }
        }
        function refreshPagina(idModal) {
            var hiddenField = document.getElementById('<%= hddMudaGrid.ClientID %>');
            var hiddenField2 = document.getElementById('<%= hddidExtrato.ClientID %>');

            console.log("idextrato: " + hiddenField2.value);
            console.log("grid" + hiddenField.value);

            $(idModal).on('hidden.bs.modal', function () {
                var url = '/App/Paginas/Adm/Financeiro/Extrato_Bancario.aspx';
                url += '?grid=' + encodeURIComponent(hiddenField.value);
                url += '&extrato=' + encodeURIComponent(hiddenField2.value);
                window.location.href = url;

            });
            $(idModal).modal('hide');

        }

        function recarregaEvento() {
            document.getElementById('btnHeadFecharImportar').addEventListener('click', function () {
                refreshPagina('#modalImportarExtrato');
            });

            document.getElementById('btnFecharFooterImportar').addEventListener('click', function () {
                refreshPagina('#modalImportarExtrato');
            });

            $('#modalLancamentoDetalhe')
                .off('hidden.bs.modal.focoLancamento')
                .on('hidden.bs.modal.focoLancamento', function () {
                    var idLancamento = document.getElementById('<%= hddidLancamento.ClientID %>').value;
                    if (idLancamento) {
                        window.setTimeout(function () {
                            focarLinhaLancamento(idLancamento);
                        }, 100);
                    }
                });
        }

        recarregaEvento();

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            recarregaEvento();
        });

        document.addEventListener('DOMContentLoaded', function () {
            var uploadContainer = document.getElementById('uploadContainer');
            var fileInput = document.getElementById('<%= fu_EnviarArquivo.ClientID %>');
            var fileNameDisplay = document.getElementById('fileName');

            uploadContainer.addEventListener('click', function () {
                fileInput.click();
            });

            uploadContainer.addEventListener('dragover', function (e) {
                e.preventDefault();
                e.stopPropagation();
                uploadContainer.classList.add('dragover');
            });

            uploadContainer.addEventListener('dragleave', function (e) {
                e.preventDefault();
                e.stopPropagation();
                uploadContainer.classList.remove('dragover');
            });

            uploadContainer.addEventListener('drop', function (e) {
                e.preventDefault();
                e.stopPropagation();
                uploadContainer.classList.remove('dragover');
                fileInput.files = e.dataTransfer.files;
                displayFileName(fileInput.files[0].name);
            });

            fileInput.addEventListener('change', function () {
                if (fileInput.files.length > 0) {
                    displayFileName(fileInput.files[0].name);
                }
            });

            function displayFileName(name) {
                fileNameDisplay.textContent = name;
            }

        });

        //$(function () {
        //    var $dtInicial = $('[id*=txtdtInicial]');
        //    var $dtFinal = $('[id*=txtdtFinal]');

        //    $dtInicial.datepicker({
        //        autoclose: true,
        //        format: 'dd/mm/yyyy',
        //        language: 'pt-BR'
        //    }).mask('99/99/9999');

        //    $dtFinal.datepicker({
        //        autoclose: true,
        //        format: 'dd/mm/yyyy',
        //        language: 'pt-BR'
        //    }).mask('99/99/9999');

        //    $dtInicial.data('previous', $dtInicial.val());
        //    $dtFinal.data('previous', $dtFinal.val());
        //});

        function updateTotal() {
            var checkboxes = document.querySelectorAll('.chkConciliacao');
            var total = 0;

            checkboxes.forEach(function (checkbox) {
                if (checkbox.checked) {
                    var row = checkbox.closest('tr');
                    var valor = row.querySelector('.valor').innerText.replace(/[^\d,]/g, '').replace(',', '.');
                    total += parseFloat(valor);
                }
            });

            document.getElementById('<%= txtnValorTotal.ClientID %>').value = 'R$ ' + total.toFixed(2).replace('.', ',');
        }

        function calculateTotal() {
            var total = 0;

            var rows = document.querySelectorAll("#<%= gv_ConciliacaoComposta.ClientID %> tr");

            rows.forEach(function (row) {
                var checkbox = row.querySelector("input[type='checkbox']");
                var lblValor = row.querySelector(".valor");

                if (checkbox && checkbox.checked && lblValor) {
                    var valor = parseFloat(lblValor.innerText.replace(/[^\d,]/g, '').replace(",", "."));
                    if (!isNaN(valor)) {
                        total += valor;
                    }
                }
            });

            document.getElementById('<%= txtnValorTotal.ClientID %>').value = total.toLocaleString("pt-BR", { style: "currency", currency: "BRL", minimumFractionDigits: 2, maximumFractionDigits: 2 });

            validateValorTotal();
        }

        function validateValorTotal() {
            var valorDebito = parseFloat(document.getElementById('<%= txtnDebito.ClientID %>').value.replace(/[^\d,]/g, '').replace(",", "."));
            var valorCredito = parseFloat(document.getElementById('<%= txtnCredito.ClientID %>').value.replace(/[^\d,]/g, '').replace(",", "."));
            var valorTotal = parseFloat(document.getElementById('<%= txtnValorTotal.ClientID %>').value.replace(/[^\d,]/g, '').replace(",", "."));

            var tipoTitulo = document.getElementById('<%= hddTipoTitulo.ClientID %>').value;
            var btnConfirmar = document.getElementById('<%= btnConfirmar.ClientID %>');

            if (tipoTitulo === "D") {
                if (valorDebito < valorTotal) {
                    btnConfirmar.disabled = true;
                    document.getElementById('<%= txtnValorTotal.ClientID %>').style.backgroundColor = "#e83838";
                } else if (valorDebito === valorTotal) {
                    btnConfirmar.disabled = false;
                    document.getElementById('<%= txtnValorTotal.ClientID %>').style.backgroundColor = "#6dd540";
                } else {
                    btnConfirmar.disabled = true;
                    document.getElementById('<%= txtnValorTotal.ClientID %>').style.backgroundColor = "#f5f259";
                }
            } else if (tipoTitulo === "C") {
                if (valorCredito < valorTotal) {
                    btnConfirmar.disabled = true;
                    document.getElementById('<%= txtnValorTotal.ClientID %>').style.backgroundColor = "#e83838"; //red
                } else if (valorCredito === valorTotal) {
                    btnConfirmar.disabled = false;
                    document.getElementById('<%= txtnValorTotal.ClientID %>').style.backgroundColor = "#6dd540"; //green
                } else {
                    btnConfirmar.disabled = true;
                    document.getElementById('<%= txtnValorTotal.ClientID %>').style.backgroundColor = "#f5f259"; //yellow
                }
            }
        }

       <%-- $(document).ready(function () {
            $('#<%= btnConciliacaoCompostaExistente.ClientID %>').click(function () {
                $('[id*=gv_ConciliacaoComposta]').DataTable({
                    paging: false,
                    order: [[1, 'asc']],
                    scrollCollapse: true,
                    scrollY: '300vh',
                    language: {
                        url: 'https://cdn.datatables.net/plug-ins/1.11.5/i18n/pt-BR.json',
                    },
                    lengthChange: false
                });
            });
        });--%>

    </script>
</asp:Content>

