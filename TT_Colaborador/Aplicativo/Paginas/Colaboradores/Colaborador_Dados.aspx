<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Colaborador_Dados.aspx.cs" Inherits="TT_Colaborador.Aplicativo.Paginas.Colaboradores.Colaborador_Dados" %>

<%@ Register Src="~/Aplicativo/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/Aplicativo/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="MainContent" runat="server">
    <div id="DIV_ESPACO" class="h-100" runat="server" style="display: none;"></div>

    <style>
        .assinatura {
            width: 100%;
            height: 100%;
            position: absolute;
        }
    </style>

    <asp:UpdatePanel ID="updDetalhe" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <div id="hdd">
                <asp:HiddenField ID="hddidColaborador" runat="server" />
                <asp:HiddenField runat="server" ID="hddAssinatura" />
            </div>

            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

            <div class="card">
                <div class="card-header card-tt">
                    <h3 class="card-title"><i class="fa fa-signature"></i><b> Assinatura</b></h3>
                </div>
                <div class="card-body">

                    <div runat="server" id="div_AssinaturaEdicao">

                        <div class="row form-group">
                            <div class="col-12 form-group">
                                <div id="div_Assinatura" runat="server" style="border: 1px solid #000; width: 100%; height: 500px; position: relative; border: 1px solid black; border-radius: 10px; cursor: crosshair;">
                                    <canvas id="canvaAssinatura" class="assinatura"></canvas>
                                </div>
                            </div>

                            <div class="col-12">
                                <asp:Button ID="cmdSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar Assinatura" OnClick="cmdSalvar_Click" />
                                <asp:LinkButton runat="server" ID="cmdLimpaAssinatura" class="btn btn-lg btn-secondary"><i class="fa fa-trash-can"></i> Limpar</asp:LinkButton>
                                <asp:LinkButton runat="server" ID="cmdCancelar" class="btn btn-lg btn-danger" OnClick="cmdCancelar_Click"><i class="fa fa-xmark"></i> Cancelar</asp:LinkButton>
                            </div>
                        </div>
                    </div>

                    <div runat="server" id="div_AssinaturaVisualizacao" class="row form-group">
                        <div class="col-12 form-group">
                            <img runat="server" id="imgAssinatura" class="assinatura" src="" alt="Assinatura do Colaborador" style="position: relative;" />
                        </div>
                    </div>

                </div>
            </div>

            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
            <br />

            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdNovaAssinatura" class="btn btn-lg btn-primary" runat="server" Text="Nova Assinatura" OnClick="cmdNovaAssinatura_Click" />
                <input type="button" class="btn btn-lg btn-warning" id="cmdVoltar" value="Voltar" name="Voltar" onclick="history.go(-1);" />
            </fieldset>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
