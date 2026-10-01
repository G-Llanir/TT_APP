<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Certificados_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.Certificados.Certificados_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCertificados_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>
    <script src="/app/js/vanilla-masker.js"></script>

    <uc1:Manual runat="server" ID="manual" />

    <div class="form-stacked">
        <div class="col-lg-12 row">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Certificados"></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
        </div>

        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

        <div>
            <ul id="tab_Certificados" class="nav nav-tabs" role="tablist">
                <li role="presentation" class="tabpanel active">
                    <a href="#Certificados" id="aba_Certificados" role="tabpanel" data-toggle="tab" aria-controls="Certificados" aria-expanded="false"><b>Certificados</b></a>
                </li>
                <li role="presentation" id="aba_Arquivos" runat="server">
                    <a href="#arquivos" role="tab" id="arquivos-tab" data-toggle="tab" aria-controls="Arquivos"><b>Arquivos</b></a>
                </li>
            </ul>
        </div>

        <br />
        <div id="tab" class="tab-content">

            <div role="tabpanel" class="tab-pane fade in active" id="Certificados" aria-labelledby="aba_Certificados">
                <asp:UpdatePanel ID="updDetalhe" runat="server">
                    <ContentTemplate>
                        <div class="panel panel-default" runat="server" id="div_Certificados">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Dados do Certificados</b></h3>
                            </div>
                            <div class="panel-body">

                                <div class="form-stacked row">

                                    <div class="col-lg-12">
                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_certificado" />
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>ID</label>
                                            <div class="form-group input-group CaixaTextoPequeno">
                                                <asp:LinkButton ID="cmRetornar" class="btn btn-sm btn-info input-group-addon" runat="server" OnClick="cmdRetornar_click"><b><</b></asp:LinkButton>
                                                <asp:TextBox ID="txtidRegistro" class="form-control" runat="server" disabled Style="text-align: center;"></asp:TextBox>
                                                <asp:LinkButton ID="cmAvancar" class="btn btn-sm btn-info input-group-addon" runat="server" OnClick="cmdAvancar_click"><b>></b></asp:LinkButton>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-lg-12">

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Tipo</label>
                                                    <asp:DropDownList ID="ddlidTipo" runat="server" class="form-control yes_no select">
                                                        <asp:ListItem Value="0" Text="Selecione o Tipo" />
                                                        <asp:ListItem Value="1" Text="Interno" />
                                                        <asp:ListItem Value="2" Text="Externo" />
                                                    </asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-5">
                                                <div class="form-group">
                                                    <label>Nome</label>
                                                    <asp:TextBox ID="txtsNome" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-3">
                                                <div class="form-group">
                                                    <label>E-mail</label>
                                                    <asp:TextBox ID="txtsEmail" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Telefone</label>
                                                    <asp:TextBox ID="txtsTelefone" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-lg-12">

                                            <div class="col-lg-5">
                                                <div class="form-group">
                                                    <label>Parceiro</label>
                                                    <asp:DropDownList ID="ddlidCliente" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-5">
                                                <div class="form-group">
                                                    <label>Assunto</label>
                                                    <asp:TextBox ID="txtsAssunto" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <label>Carga Horária</label>
                                                <div class="input-group">
                                                    <asp:TextBox ID="txtnCargaHoraria" class="form-control" runat="server" aria-describedby="Span1" Style="z-index: 0;"></asp:TextBox>
                                                    <span class="input-group-addon" id="Span1" runat="server">H</span>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-lg-12">
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Data</label>
                                                    <asp:TextBox ID="txtdtCertificado" class="form-control" type="date" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Validade</label>
                                                    <asp:TextBox ID="txtdtValidade" class="form-control" type="date" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-4">
                                                <div class="form-group">
                                                    <label>Número Certificado</label>
                                                    <asp:TextBox ID="txtnNumero" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>


                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                <br />
            </div>

            <div role="tabpanel" class="tab-pane fade" id="arquivos" aria-labelledby="arquivos-tab">
                <div class=" embed-responsive embed-responsive-16by9" style="min-height: 800px" runat="server" id="upArquivos">
                    <embed type="text/html" runat="server" id="frmArquivos" width="500" height="300" />
                </div>
            </div>
        </div>

        <%----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>

        <fieldset class="form-stacked actions">
            <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-info" runat="server" Text="Salvar" ValidationGroup="DETALHE" />
            <a href="javascript:history.back()" class="btn btn-lg btn-warning" title="Voltar">Voltar</a>
            <%--<input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field-cancel" title="Voltar" onclick="history.go(-1)">--%>
        </fieldset>
        <div id="dialog-Salvar" class="modal" title="Salvar">
            <p>
                <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                <asp:Label ID="lblTituloSalvar" runat="server" Text="Confirma a Inclusão do Certificado?"></asp:Label>
            </p>
        </div>
    </div>

    <asp:HiddenField ID="hddidCertificados" runat="server" />
</asp:Content>
