<%@ Page Language="C#" MasterPageFile="~/site.master" AutoEventWireup="true" CodeBehind="MenuColaborador.aspx.cs" Inherits="TT_Colaborador.MenuColaborador" %>

<%@ Register Src="~/Aplicativo/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/Aplicativo/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="content_frmMenu_Detalhe" ContentPlaceHolderID="MainContent" runat="server">

    <asp:UpdatePanel ID="updDetalhe" runat="server">
        <ContentTemplate>
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
            <br />

            <div class="card">
                <div class="card-header card-tt">
                    <h3 class="card-title"><i class="fa fa-address-card"></i><b>Perfil</b></h3>
                </div>
                <div class="card-body">
                    <div class="row g-4">
                        <div class="col-lg-12 text-center">
                            <div class="d-flex flex-column align-items-center">
                                <asp:Image ID="imgColaborador" runat="server" CssClass="img-fluid rounded-circle" Height="300" Width="300" style="border: 2px solid green;" />
                                <br />
                                <h2><b id="lblNomeColaborador" runat="server"></b></h2>
                                <h4><label ID="lblDepartamento" runat="server"></label></h4>
                            </div>
                        </div>
                    </div>

                    <div class="container">
                        <div class="row g-4">

                            <div class="col-lg-3">
                                <div class="d-flex flex-column align-items-center">
                                    <label><b>Email</b></label>
                                    <asp:TextBox ID="txtsEmail" CssClass="form-control text-center" runat="server" disabled=""></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-3">
                                <div class="d-flex flex-column align-items-center">
                                    <label><b>Celular</b></label>
                                    <asp:TextBox ID="txtsTelCelular" CssClass="form-control text-center" runat="server" disabled=""></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-3">
                                <div class="d-flex flex-column align-items-center">
                                    <label><b>Ramal</b></label>
                                    <asp:TextBox ID="txtsRamal" CssClass="form-control text-center" runat="server" disabled=""></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-3">
                                <div class="d-flex flex-column align-items-center">
                                    <label><b>Data de Nascimento</b></label>
                                    <asp:TextBox ID="txtdtNascimento" CssClass="form-control text-center" runat="server" disabled=""></asp:TextBox>
                                </div>
                            </div>

                        </div>
                    </div>

                    <div class="row g-4 mt-2">
                        <div class="col-lg-12">
                            <div class="d-flex justify-content-center">
                                <asp:HyperLink ID="hplAssinatura" runat="server" NavigateUrl="/Aplicativo/Paginas/Colaboradores/Colaborador_Dados.aspx" CssClass="btn btn-md btn-primary"><i class="fa fa-pen-clip"></i> Assinatura</asp:HyperLink>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
