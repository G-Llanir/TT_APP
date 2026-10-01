<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Tarefas_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.Tarefas_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>




<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_Tarefas_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>
    <asp:UpdatePanel ID="updDetalhe" runat="server">
        <ContentTemplate>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>
            </div>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                </div>

                <div class="col-lg-12">
                    <div class="form-group">
                        <label>ID </label>
                        <asp:TextBox ID="txtidTarefa" class="form-control CaixaTextoMini" runat="server" disabled="0"></asp:TextBox>
                    </div>
                </div>
                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Departamento </label>
                        <asp:DropDownList ID="ddlDepartamento" runat="server" class="form-control CaixaTextoGigante" attrname="Departamento">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Nome da Tarefa</label>
                        <asp:TextBox ID="txtsDscTarefa" class="form-control CaixaTextoGigante" runat="server" MaxLength="50"></asp:TextBox>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Descrição da Tarefa (Memorando)</label>
                        <asp:TextBox ID="txtsObservacaoTarefa" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="150px"></asp:TextBox>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Obrigatório Conclusão para executar a próxima tarefa </label>
                        <asp:DropDownList ID="ddlsObrigatorioConclusao" runat="server" class="form-control CaixaTextoMedio" attrname="Obrigatório Conslusão para executar a próxima tarefa">
                            <asp:ListItem Selected="True" Value="S">Sim</asp:ListItem>
                            <asp:ListItem Value="N">Não</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="row ">

                        <div class="col-lg-4">
                            <div class="form-group">
                                <label>Tempo Execução</label>
                                <div class="form-inline">
                                    <asp:TextBox ID="txtnTempo" class="form-control CaixaTextoMicro" MaxLength="2" runat="server" Text=""></asp:TextBox>
                                    <asp:DropDownList ID="ddlTipoTempo" runat="server" class="form-control yes_no select CaixaTextoMini">
                                        <asp:ListItem class="danger" Value="" Selected="True">Selecione </asp:ListItem>
                                        <asp:ListItem Value="h">Hora</asp:ListItem>
                                        <asp:ListItem Value="d">Dia</asp:ListItem>
                                        <asp:ListItem Value="s">Semana</asp:ListItem>
                                        <asp:ListItem Value="m">Mês</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>

                <div class="col-lg-12">
                    <uc1:SwitchAtivo runat="server" ID="ComboAtivo" />
                </div>
                <div class="col-lg-12" runat="server" id="DIV_STSO">
                    <div class="btn-group">
                        <asp:CheckBox ID="STSO" Text="Habilita aba STSO" runat="server" CssClass="btn btn-primary" />
                        <asp:CheckBox ID="ART" Text="Habilita aba ART" runat="server" CssClass="btn btn-primary" />
                    </div>
                </div>
            </div>

            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />


            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                &nbsp;
            <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" " id="field-cancel" title="Voltar" onclick="history.go(-1)" >&nbsp;
            </fieldset>

            <asp:HiddenField ID="hddidTarefa" runat="server" />

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
