<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="GHE_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.GHE_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="content_frmGHE_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>
            </div>
            <div class="col-lg-12 row">
                <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
            </div>
            <div id="DIV_Body" runat="server">

                <div id="abas">
                    <ul id="tab_GHE" class="nav nav-tabs" role="tablist">
                        <li role="presentation" class="tabpanel active">
                            <a href="#ghe" id="aba_ghe" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Dados</b></a>
                        </li>
                        <li role="presentation" runat="server" id="aba_informacoes">
                            <a href="#informacoes" role="tab" id="informacoes-tab" data-toggle="tab" aria-controls="informacoes"><b>Informações</b></a>
                        </li>

                        <%--ocultos por não implementação--%>
                        <li role="presentation" runat="server" id="aba_riscos" style="display: none;">
                            <a href="#riscos" role="tab" id="riscos-tab" data-toggle="tab" aria-controls="riscos"><b>Riscos</b></a>
                        </li>
                        <li role="presentation" runat="server" id="aba_historico" style="display: none;">
                            <a href="#historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="riscos"><b>Histórico</b></a>
                        </li>
                    </ul>
                </div>

                <div id="tab" class="tab-content">

                    <div role="tabpanel" class="tab-pane fade in active" id="ghe" aria-labelledby="ghe-tab">
                        <br />
                        <div class="form-stacked">
                            <div class="col-lg-12">
                                <div class="form-group">
                                    <label>ID</label>
                                    <asp:TextBox ID="txtidGHE" runat="server" class="form-control CaixaTextoMini" disabled=""></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-lg-12 form-group row">
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Código</label>
                                        <asp:TextBox ID="txtsCodigoGHE" runat="server" class="form-control " MaxLength="20"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-7">
                                    <div class="form-group">
                                        <label>Empresa</label>
                                        <asp:ListBox runat="server" ID="lstidEmpresa" SelectionMode="Multiple" CssClass="Caixa_Selecao"></asp:ListBox>
                                    </div>
                                </div>
                            </div>


                            <!-- NOVO CAMPO ADICIONADO -->
                            <div class="col-lg-12 form-group row">
                            </div>
                            <!-- FIM DO NOVO CAMPO -->

                            <div class="col-lg-12 form-group row">
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Plano</label>
                                        <asp:DropDownList runat="server" ID="ddlIdPlano" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-7">
                                    <div class="form-group">
                                        <label>Descrição</label>
                                        <asp:TextBox ID="txtsDscGHE" runat="server" class="form-control " MaxLength="300"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-12 row">
                                <div class="form-group">

                                    <div class="col-lg-9 form-group">
                                        <label>Setor</label>
                                        <asp:ListBox runat="server" ID="lstidSetor" SelectionMode="Multiple" CssClass="Caixa_Selecao"></asp:ListBox>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-12 row" runat="server" id="DIV_Funcoes">
                                <div class="form-group">
                                    <div class="col-lg-9">
                                        <div class="form-group">
                                            <label>Funções Vinculadas</label>
                                            <br />
                                            <asp:Label ID="lblsFuncoes" runat="server" Text=""></asp:Label>

                                        </div>
                                    </div>
                                </div>
                            </div>
                            <br />
                            <div class="col-lg-12">
                                <div class="form-group">
                                    <uc1:SwitchAtivo ID="sAtivo" runat="server" />
                                </div>
                            </div>
                            <div class="col-lg-12 form-group">
                                <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                            </div>
                        </div>
                    </div>

                    <div role="tabpanel" class="tab-pane fade" id="informacoes" aria-labelledby="informacoes-tab">
                        <br />
                        <div class="form-stacked">

                            <div class="col-lg-12 form-group">
                                <label>Síntese das Atividades Realizadas no Ambiente</label>
                                <asp:TextBox runat="server" ID="txtsDscSintese" CssClass="form-control" MaxLength="800" TextMode="MultiLine" Height="150px"></asp:TextBox>
                            </div>

                            <div class="col-lg-12 form-group">
                                <label>Descrição do Ambiente de Trabalho</label>
                                <asp:TextBox runat="server" ID="txtsDscAmbienteTrabalho" CssClass="form-control" MaxLength="800" TextMode="MultiLine" Height="150px"></asp:TextBox>
                            </div>

                            <div class="col-lg-12 form-group">
                                <label>EPC</label>
                                <asp:TextBox runat="server" ID="txtsEPC" CssClass="form-control" MaxLength="800" TextMode="MultiLine" Height="150px"></asp:TextBox>
                            </div>

                            <div class="col-lg-12 form-group">
                                <label>Máquinas e Equipamentos</label>
                                <asp:TextBox runat="server" ID="txtsMaquinas_Equipamentos" CssClass="form-control" MaxLength="800" TextMode="MultiLine" Height="150px"></asp:TextBox>
                            </div>


                        </div>
                    </div>

                    <div role="tabpanel" class="tab-pane fade" id="riscos" aria-labelledby="riscos-tab">
                        <br />
                    </div>

                    <div role="tabpanel" class="tab-pane fade" id="historico" aria-labelledby="historico-tab">
                        <br />
                    </div>


                </div>


                <fieldset class="col-lg-12 form-stacked actions">
                    <asp:Button ID="GHEcmdSalvar" runat="server" class="btn btn-lg btn-success" Text="Salvar" />
                    <asp:Button ID="GHEcmdExcluir" class="btn btn-lg btn-danger" runat="server" Text="Excluir" />
                    <input type="submit" name="cancel" class="btn btn-lg btn-warning" id="field-cancel" value="Cancelar" title="Voltar" onclick="history.go(-1)" />
                </fieldset>
            </div>
            <asp:HiddenField ID="hddidGHE" runat="server" />
            <asp:HiddenField ID="hddsidSetor" runat="server" />
            <asp:HiddenField ID="hddsidEmpresa" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>

    <div id="dialog-Salvar" class="modal" title="Salvar">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <b>
                <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label></b>
        </p>
    </div>

    <div id="dialog-Duplicar" class="modal" title="Duplicar">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloDuplicar" runat="server" Text=""></asp:Label>
        </p>
    </div>

    <div id="dialog-Excluir" class="modal" title="Excluir">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <b>
                <asp:Label ID="lblTitulosExcluir" runat="server" Text=""></asp:Label></b>
        </p>
    </div>
</asp:Content>
