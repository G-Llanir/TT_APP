<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="ConsultaSaldo_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.Manutencao.ConsultaSaldo_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        .linha-riscada {
            border-top: 1px dotted red;
            color: red;
            background-color: yellow;
        }
    </style>

    <div id="DIV3" runat="server">
        <script src="https://login.tecandtec.com.br/app/js/jquery-1.9.2.min.js" type="text/javascript"></script>
        <script> $v192 = jQuery.noConflict();</script>


        <div class="form-stacked row">
            <div class="col-lg-12">
                <h1>
                    <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA">

                    </asp:Label></h1>
                <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Detalhe" />
            </div>
        </div>

        <br />

        <div class="panel panel-default" runat="server">
            <div class="panel-heading">
                <h3 class="panel-title"><b>Arquivos</b></h3>
            </div>

            <div class="panel-body">

                <div id="div_EnviarArquivos" class="form-stacked " runat="server">
                    <div class="panel panel-default" runat="server">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Enviar Arquivos</b></h3>
                        </div>
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12">
                                    <div class="col-lg-6">
                                        <div class="row">
                                            <div class="col-lg-12" id="div4" runat="server">
                                                <div class="form-group">
                                                    <label>Selecione a tabela de saldo</label>
                                                    <asp:FileUpload ID="fu_Planilha" class="form-control-file" runat="server" Width="400px" accept=".xlsm,.xlsx" OnDataBinding="fu_Planilha_DataBinding" />
                                                </div>
                                            </div>
                                            <div class="col-lg-12">
                                                <div class="form-group">
                                                    <label>
                                                        <asp:Label ID="lblEnviarArquivo_sDscArquivo" runat="server" Text="Descrição"></asp:Label></label>
                                                    <asp:TextBox ID="txtEnviarArquivo_sDscArquivo" class="form-control" runat="server" MaxLength="300"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-lg-12">
                                                <div class="form-group">
                                                    <asp:Button ID="cmdEnviarArquivos" CssClass="btn-success btn" runat="server" Text="Enviar" OnClick="cmdEnviarArquivos_Click" />
                                                </div>
                                            </div>
                                     
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>


                <div class="form-stacked row">
                    <div class="col-lg-12 table-responsive">
                        <asp:GridView ID="gv_Arquivo" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                            ShowFooter="False" OnRowCommand="gv_Arquivo_RowCommand" OnRowDataBound="gv_Arquivo_RowDataBound" OnDataBinding="gv_Arquivo_DataBinding">
                            <Columns>
                                <asp:BoundField DataField="idArquivo" HeaderText="ID">
                                    <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:ButtonField DataTextField="idArquivo" HeaderText="nID" CommandName="ExibirTabela" ItemStyle-Width="5%" />

                                <asp:BoundField DataField="sObservacao" HeaderText="Observacao">
                                    <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:BoundField DataField="sDscTipoArquivo" HeaderText="Tipo">
                                    <ItemStyle Width="15%" HorizontalAlign="left" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:ButtonField DataTextField="sNomeArquivo" HeaderText="Nome do Arquivo" CommandName="Download" ItemStyle-Width="20%" />

                                <asp:BoundField DataField="sDscArquivo" HeaderText="Descrição">
                                    <ItemStyle Width="25%" HorizontalAlign="left" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Usuário">
                                    <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                </asp:BoundField>
                                <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Inclusão">
                                    <ItemStyle Width="0%" HorizontalAlign="left" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:TemplateField HeaderText="Deletar" Visible="false">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkDocumento_Excluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa fa-eraser"></i></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>

                            </Columns>
                        </asp:GridView>

                    </div>
                </div>
                <asp:UpdatePanel ID="updpTabelaSaldo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <script type="text/javascript">
                            function MudarBotao() {
                                document.getElementById("cphCorpo_cmdAtualizarBanco").value = "Atualizando...";
                                $('[cmdAtualizarBanco]').prop('disabled', true);
                                $('[id*=field-cancel]').prop('disabled', true);
                                $('[id*=txtEnviarArquivo_sDscArquivo]').prop('disabled', true);
                            }

                        </script>
                        <div id="div_ExibeTabela" class="panel panel-default" runat="server">
                            <div class="panel-heading">
                                <h3 class="panel-heading">
                                    <asp:Label ID="lblTituloTabelaSaldo" runat="server" Text="Tabela Carregada"></asp:Label></h3>
                            </div>
                            <div class="panel-body">
                                <div class="form-stacked">
                                    <asp:GridView ID="gv_TabelaSaldo" class="table table-striped table-bordered table-hover table-condended table-responsive"
                                        runat="server" CellPadding="1" AutoGenerateColumns="true" OnRowDataBound="gv_TabelaSaldo_RowDataBound" EnableViewState="true">
                                        <Columns>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                                <div>
                                    <asp:UpdateProgress ID="updtProgress" runat="server" DisplayAfter="0">
                                        <ProgressTemplate>
                                            <div id="div_Progress" class="progress">
                                                <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
                                                    Processando...
                                                </div>
                                            </div>
                                        </ProgressTemplate>
                                    </asp:UpdateProgress>
                                </div>
                                <div>
                                <fieldset class="form-stacked actions">
                                <asp:Button ID="cmdAtualizarBanco" class="btn  btn-lg btn-success" runat="server" Text="Atualizar Saldo" OnClick="cmdAtualizarBanco_Click" onclientclick="MudarBotao()"/>
                                &nbsp;
                                <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" " id="field-cancel" title="Voltar" onclick="history.go(-1)" >&nbsp;
                                </fieldset>
                                </div>
                            </div>
                        </div>

                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cmdAtualizarBanco" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>

            </div>
        </div>
    </div>



<%--    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />--%>

    <asp:HiddenField ID="hddidObjeto" runat="server" />
    <asp:HiddenField ID="hddsTipoObjeto" runat="server" />

</asp:Content>
