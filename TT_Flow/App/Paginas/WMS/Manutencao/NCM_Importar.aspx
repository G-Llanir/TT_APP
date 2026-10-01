<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="NCM_Importar.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.Manutencao.NCM_Importar" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">


    <script src="https://login.tecandtec.com.br/app/js/jquery-1.9.2.min.js" type="text/javascript"></script>
    <script> $v192 = jQuery.noConflict();</script>

    <div id="DIV3" runat="server">

        <div class="form-stacked row">
            <div class="col-lg-12">
                <h1>
                    <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label><small> Importar</small></h1>
                <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
            </div>
        </div>
        <div class="panel panel-default" runat="server" visible="false">
            <div class="panel-heading">
                <h3 class="panel-title"><b>Histórico</b></h3>
            </div>
            <div class="panel-body">
                

                <div class="form-stacked row">
                    <div class="col-lg-12 table-responsive">
                        <asp:GridView ID="gv_Arquivo" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                            ShowFooter="False" OnRowCommand="gv_Arquivo_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="idArquivo" HeaderText="ID">
                                    <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:ButtonField DataTextField="idArquivo" HeaderText="nID" CommandName="ExibirTabela" ItemStyle-Width="20%" />

                                <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Inclusão">
                                    <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:BoundField DataField="sDscTipoArquivo" HeaderText="Tipo">
                                    <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:ButtonField DataTextField="sNomeArquivo" HeaderText="Nome do Arquivo" CommandName="Download" ItemStyle-Width="20%" />

                                <asp:BoundField DataField="sDscArquivo" HeaderText="Descrição">
                                    <ItemStyle Width="40%" HorizontalAlign="left" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Usuário">
                                    <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
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
            </div>
        </div>
    
    <fieldset class="form-stacked actions">
    <asp:Button ID="cmdAtualizar" class="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdAtualizar_Click" />
    &nbsp;
    <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" " id="field-cancel" title="Voltar" onclick="history.go(-1)" >&nbsp;
</fieldset>

    <asp:HiddenField ID="hddidObjeto" runat="server" />
    <asp:HiddenField ID="hddsTipoObjeto" runat="server" />
</div>
</asp:Content>
