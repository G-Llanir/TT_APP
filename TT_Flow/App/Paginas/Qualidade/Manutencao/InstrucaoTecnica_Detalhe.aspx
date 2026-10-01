<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="InstrucaoTecnica_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Qualidade.Manutencao.InstrucaoTecnica_Detalhe" %>

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
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>


    <%--<script type="text/javascript">

        $(function () {


        });

    </script>--%>


    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text=""></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />

            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>
    </div>

    <div>
        <ul id="tab_InstrucaoTecnica" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="tabpanel active">
                <a href="#instrucaoTecnica" id="aba-instrucaoTecnica" role="tab" data-toggle="tab" aria-controls="instrucaoTecnica" aria-expanded="false"><b>Instrução Técnica</b></a>
            </li>

            <li role="presentation" runat="server" id="aba_Arquivo">
                <a href="#arquivo" role="tab" id="arquivo-tab" data-toggle="tab" aria-controls="Arquivo"><b>Arquivo</b></a>
            </li>

            <li role="presentation" runat="server" id="aba_Historico">
                <a href="#historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="Historico"><b>Histórico</b></a>
            </li>

        </ul>
    </div>

    <div id="tab" class="tab-content">

        <div role="tabpanel" class="tab-pane fade in active" id="instrucaoTecnica" aria-labelledby="instrucaoTecnica">
            <br />
            <asp:UpdatePanel ID="updDetalhe" runat="server">
                <ContentTemplate>

                    <div class="panel panel-default" runat="server" id="div_InstrucaoTecnica">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Dados da Instrução Técnica</b></h3>
                        </div>
                        <div class="panel-body">

                            <div class="form-stacked row">
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>ID </label>
                                        <asp:TextBox ID="txtidIT" class="form-control CaixaTextoMini" runat="server" disabled=""></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group" runat="server" id="divdtInclusao">
                                        <label>Data Inclusão </label>
                                        <asp:TextBox ID="txtdtInclusao" class="form-control CaixaTextoMedio" runat="server" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Revisão</label>
                                        <asp:TextBox ID="txtnRevisao" class="form-control CaixaTextoMini" runat="server" Text="1" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Título da Instrução</label>
                                        <asp:TextBox ID="txtsTituloIT" class="form-control " runat="server" Width="50%" MaxLength="200" placeholder="Título"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Descrição IT </label>
                                        <HTMLEditor:Editor ID="txtDscEditor" runat="server" Width="100%" Height="1200px"/>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                </ContentTemplate>
            </asp:UpdatePanel>

        </div>

        <div role="tabpanel" class="tab-pane fade" id="historico" aria-labelledby="Historico">
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
                                ShowFooter="False" OnRowDataBound="gv_Historico_RowDataBound">
                                <Columns>
                                    <asp:BoundField DataField="dtInclusao" HeaderText="Data Inclusão">
                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="nRevisao" HeaderText="Revisão">
                                        <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sTituloIT" HeaderText="Título">
                                        <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sSituacao_Completa" HeaderText="Situação">
                                        <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscIT" HeaderText="Descrição IT">
                                        <ItemStyle Width="40%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="dtAtualizacao" HeaderText="Data de Atualização">
                                        <ItemStyle Width="10%" HorizontalAlign="center" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Usuário">
                                        <ItemStyle Width="14%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>

                        </div>
                    </div>
                </div>
            </div>
        </div>

        <%--------------------------------------Tab-Arquivo----------------------------------------------------------------------------------------------------------------------------------------------------------------%>
        <div role="tabpanel" class="tab-pane fade" id="arquivo" aria-labelledby="Arquivo">
            <div class=" embed-responsive embed-responsive-16by9" style="min-height: 800px" runat="server" id="DIV_Arquivos">
                <embed type="text/html" runat="server" id="frmArquivos" width="800" height="500" />
            </div>
        </div>
        <%----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>
    </div>


    <fieldset class="form-stacked actions">
        <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" ValidationGroup="DETALHE" />
        <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field_cancel" title="Voltar" onclick="history.go(-1)">
        <asp:LinkButton ID="cmdGerarPDF" class="btn btn-lg btn-danger" runat="server" Text="Gerar PDF" OnClick="cmdGeraPDF_Click" />

    </fieldset>

    <div id="dialog-Salvar" class="modal" title="Salvar">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
        </p>
    </div>

    <%--<div id="dialog-Editar" class="modal" title="Editar Instrução">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloEditar" runat="server" Text="Confirma a Edição da Instrução Técnica?"></asp:Label>
        </p>
    </div>--%>



    <asp:HiddenField ID="hddidIT" runat="server" />
</asp:Content>
