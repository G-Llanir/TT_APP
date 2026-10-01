<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Procedimento_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.OS.Manutencao.Procedimento_Detalhe" %>

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
<asp:Content ID="content_Procedimento_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>


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
                <asp:TextBox ID="txtidProcedimento" class="form-control CaixaTextoMini" runat="server" disabled="0"></asp:TextBox>
            </div>
        </div>
        <div class="col-lg-12">
            <div class="form-group">
                <label>Departamento </label>
                <asp:DropDownList ID="ddlDepartamento" runat="server" class="form-control" attrname="Departamento">
                </asp:DropDownList>
            </div>
        </div>

        <div class="col-lg-6">
            <div class="form-group">
                <label>Nome do Procedimento</label>
                <asp:TextBox ID="txtsDscProcedimento" class="form-control" runat="server" MaxLength="50"></asp:TextBox>
            </div>
        </div>
        <div class="col-lg-6">
            <div class="form-group">
                <label>Tipo Procedimento </label>
                <asp:DropDownList ID="ddlTipoProcedimento" runat="server" class="form-control" attrname="TipoProcedimento">
                </asp:DropDownList>
            </div>
        </div>

        <%--<div class="col-lg-12">
                <div class="form-group">
                    <label>Descrição do Procedimento</label>
                    <asp:TextBox ID="txtsObservacaoProcedimento" class="form-control"  runat="server" MaxLength="4000" TextMode="MultiLine" Height="150px"></asp:TextBox>
                </div>
            </div> --%>

        <div class="panel-body col-lg-12">
            <div class="form-group">
                <label>Descrição do Procedimento</label>
                <HTMLEditor:Editor ID="sObservacaoProcedimento" runat="server" Width="100%" Height="500px" />
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
                                <asp:ListItem Value="n">Minuto</asp:ListItem>
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
            <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
        </div>
    </div>

    <div class="panel panel-default" id="DIV_Arquivos" runat="server">
        <div class="panel-heading">
            <h3 class="panel-title"><b>Arquivos</b></h3>
        </div>

        <div class="panel-body">
            <div class="form-stacked row">
                <div class="col-lg-12 table-responsive">
                    <asp:GridView ID="gv_Arquivo" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                        ShowFooter="False" OnRowCommand="gv_Arquivo_RowCommand" OnRowDataBound="gv_Arquivo_RowDataBound" OnRowDeleting="gv_Arquivo_RowDeleting">
                        <Columns>
                            <asp:BoundField DataField="idArquivo" HeaderText="ID">
                                <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" />
                            </asp:BoundField>
                            <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Inclusão">
                                <ItemStyle Width="15%" HorizontalAlign="left" VerticalAlign="Middle" />
                            </asp:BoundField>

                            <asp:ButtonField DataTextField="sNomeArquivo" HeaderText="Nome do Arquivo" CommandName="Download" ItemStyle-Width="15%" />

                            <asp:BoundField DataField="sDscArquivo" HeaderText="Descrição">
                                <ItemStyle Width="40%" HorizontalAlign="left" VerticalAlign="Middle" />
                            </asp:BoundField>

                            <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Usuário">
                                <ItemStyle Width="15%" HorizontalAlign="left" VerticalAlign="Middle" />
                            </asp:BoundField>


                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                </ItemTemplate>
                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                            </asp:TemplateField>

                        </Columns>
                    </asp:GridView>

                </div>
            </div>
            <div id="div_EnviarArquivos" class="form-stacked " runat="server">
                <div class="panel panel-default" runat="server">
                    <div class="panel-heading">
                        <h3 class="panel-title"><b>Enviar Arquivos</b></h3>
                    </div>
                    <uc1:MensagemPagina runat="server" ID="MensagemArquivo" />
                    <div class="panel-body">
                        <div class="form-stacked row">

                            <div class="col-lg-6" id="div_EnviarArquivos_Selecao" runat="server">
                                <div class="form-group">
                                    <label>Selecione o Arquivo</label>
                                    <asp:FileUpload ID="fu_Arquivo" class="form-control-file" runat="server" Width="400px" />
                                </div>
                            </div>
                            <div class="col-lg-12">
                                <div class="form-group">
                                    <label>
                                        <asp:Label ID="lblEnviarArquivo_sDscArquivo" runat="server" Text="Descrição"></asp:Label></label>
                                    <asp:TextBox ID="txtEnviarArquivo_sDscArquivo" class="form-control CaixaTextoGigante" runat="server" MaxLength="300"></asp:TextBox>
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


    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />



    <fieldset class="form-stacked actions">
        <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
        <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" id="field-cancel" title="Voltar" onclick="history.go(-1)">
    </fieldset>

    <asp:HiddenField ID="hddidProcedimento" runat="server" />



</asp:Content>
