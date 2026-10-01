<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Ato_Concessorio_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.COMEX.Manutencao.Ato_Concessorio_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register
    Assembly="AjaxControlToolkit"
    Namespace="AjaxControlToolkit.HtmlEditor"
    TagPrefix="HTMLEditor" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        .embed-responsive-16by9 {
            padding-bottom: 200px;
        }
    </style>

    <script>

        $(function () {
            $('[id*=txtdtEmissao]').datepicker({
                autoclose: true,
                format: 'dd/mm/yyyy',
                language: 'pt-BR'
            });

            $('[id*=txtdtEmissao]').mask('99/99/9999');

        });

        $(function () {
            $('[id*=txtdtDeferimento]').datepicker({
                autoclose: true,
                format: 'dd/mm/yyyy',
                language: 'pt-BR'
            });

            $('[id*=txtdtDeferimento]').mask('99/99/9999');

        });


        $(function () {
            $('[id*=txtdtVencimento]').datepicker({
                autoclose: true,
                format: 'dd/mm/yyyy',
                language: 'pt-BR'
            });

            $('[id*=txtdtVencimento]').mask('99/99/9999');

        });


    </script>

    <asp:UpdatePanel ID="UpdHeading" runat="server">
        <ContentTemplate>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text=""></asp:Label><small> Detalhe</small>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>
                <div class="col-lg-12">
                    <div class="col-lg-12">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <div>
        <ul id="tab_Principal" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="tabpanel active" id="aba_Principal">
                <a href="#principal" id="aba-principal" role="tab" data-toggle="tab" aria-controls="principal" aria-expanded="false"><b>Ato Concessório</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_Arquivos">
                <a href="#arquivos" role="tab" id="aba_arquivos-tab" data-toggle="tab" aria-controls="arquivos"><b>Arquivos</b></a>
            </li>
        </ul>
    </div>
    <br />

    <div id="tab" class="tab-content">

        <div role="tabpanel" class="tab-pane fade in active" id="principal" aria-labelledby="aba_Principal">
            <asp:UpdatePanel ID="UpdDetalhe" runat="server">
                <ContentTemplate>
                    <br />
                    <div class="panel panel-default">

                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Detalhe</b></h3>
                        </div>
                        <div class="panel-body">
                            <br />
                            <div class="form-stacked row">

                                <div class="col-lg-12">
                                    <div class="form-group" style="float: left">
                                        <label>ID</label>
                                        <asp:TextBox ID="txtidAtoConcessorio" runat="server" class="form-control" disabled=""></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Código</label>
                                        <asp:TextBox ID="txtsCódigo" runat="server" class="form-control" MaxLength="50"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Data de Emissão</label>
                                        <asp:TextBox ID="txtdtEmissao" class="form-control" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Data de Deferimento</label>
                                        <asp:TextBox ID="txtdtDeferimento" class="form-control" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Data de Vencimento</label>
                                        <asp:TextBox ID="txtdtVencimento" class="form-control" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <uc1:ComboAtivo runat="server" ID="sAtivo"/>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <br />
                                        <label>Descrição</label>
                                        <asp:TextBox ID="txtsCorpo" class="form-control" runat="server" MaxLength="200"  />
                                    </div>
                                </div>
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Observação</label>
                                        <asp:TextBox ID="txtsDscObservacao" class="form-control" runat="server" TextMode="MultiLine" Height="150px"></asp:TextBox>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                    <br />

                    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacaoDetalhe" />
                    <fieldset class="form-stacked actions">
                        <asp:Button ID="cmdSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                        <input type="submit" name="cancel" class="btn btn-lg btn-danger" id="field_cancelDetalhe" value="Cancelar" title="Voltar" onclick="history.go(-1)" runat="server" />
                    </fieldset>
                    <asp:HiddenField ID="hddidNCM" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="arquivos" aria-labelledby="aba_arquivos">
            <div class=" embed-responsive embed-responsive-16by9" style="min-height: 800px">
                <embed type="text/html" runat="server" id="eArquivos" width="800" height="500" />
            </div>
        </div>

    </div>



    <br />
</asp:Content>
