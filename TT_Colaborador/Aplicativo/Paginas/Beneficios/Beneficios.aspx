<%@ Page Title="" Language="C#" MasterPageFile="~/site.master" AutoEventWireup="true" CodeBehind="Beneficios.aspx.cs" Inherits="TT_Colaborador.Aplicativo.Paginas.Beneficios.Beneficios" %>

<%@ Register Src="~/Aplicativo/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/Aplicativo/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/Aplicativo/Controles/Referencia_MesAno.ascx" TagPrefix="uc1" TagName="Referencia_MesAno" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmBeneficios" ContentPlaceHolderID="MainContent" runat="server">
    <div id="DIV_ESPACO" class="h-100" runat="server" style="display: none;"></div>

    <style>
        .info-box:hover {
            background-color: #f0f0f0;
        }

        .card-header-tt {
            background-color: #4CAF50;
            color: white;
        }

        .info-box {
            background-color: #e0f2f1;
            padding: 15px;
            border-radius: 5px;
            transition: background-color 0.3s ease;
            cursor: pointer;
            margin-bottom: 15px;
        }

        .info-title {
            color: #4CAF50;
            margin-top: 0;
        }

        .literal-content {
            font-weight: bold;
        }

        .info-box:hover {
            background-color: #81C784;
        }

        .panel-box {
            background-color: #81C784;
            border-radius: 10px;
            padding: 15px;
            margin-bottom: 15px;
        }

        .mes-ano {
            font-family: Arial, sans-serif;
            font-size: 20px;
            font-weight: bold;
            text-transform: capitalize;
            color: #333;
            padding: 5px;
            display: inline-block;
        }
    </style>

    <div id="hdd">
        <asp:HiddenField ID="hddidColaborador" runat="server" />
    </div>

    <asp:UpdatePanel ID="updDetalhe" runat="server">
        <ContentTemplate>

            <div class="card">
                <div class="card-header card-tt">
                    <h3 class="card-title"><b><asp:Label ID="lblTituloPagina" runat="server"></asp:Label></b></h3>
                </div>

                <div class="card-body">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                    <div id="beneficiosDadosAnterior" runat="server">
                        <div class="row align-items-center">
                            <div class="col-md-2">
                                <label style="color: white;"><b>Mês Referência:</b></label>
                                <uc1:Referencia_MesAno runat="server" ID="Referencia_MesAno1" />
                            </div>
                            <div class="col-md-6">
                                <asp:Button ID="btnBuscarAnterior" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click1" />
                                <asp:Button ID="LimparAnterior" class="btn btn-secondary" runat="server" Text="Limpar Filtros" OnClick="cmdLimpar_Click1" />
                            </div>
                        </div>
                    </div>

                    <div id="beneficiosDados" runat="server">
                        <div class="row">
                            <div class="container">
                                <div class="card mt-4">
                                    <div class="card-body">
                                        <div class="row align-items-center">
                                            <div class="col-md-3">
                                                <div class="info-box">
                                                    <h4 class="info-title">Úteis</h4>
                                                    <div class="literal-content">
                                                        <asp:Literal ID="ltrDiasUteis" runat="server"></asp:Literal>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-3">
                                                <div class="info-box">
                                                    <h4 class="info-title">Atestado</h4>
                                                    <div class="literal-content">
                                                        <asp:Literal ID="ltrDiasAtestado" runat="server"></asp:Literal>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-3">
                                                <div class="info-box">
                                                    <h4 class="info-title">Feriados</h4>
                                                    <div class="literal-content">
                                                        <asp:Literal ID="ltrDiasFeriado" runat="server"></asp:Literal>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-3">
                                                <div class="info-box">
                                                    <h4 class="info-title">Adicionais</h4>
                                                    <div class="literal-content">
                                                        <asp:Literal ID="ltrDiasCredito" runat="server"></asp:Literal>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>

                        <br />

                        <div class="row">
                            <div class="container">
                                <div class="card mt-4">
                                    <div class="card-header card-header-tt" style="background-color: #4CAF50; color: white;">
                                        <b>
                                            <asp:Label ID="Cabecalho" CssClass="mes-ano-label" runat="server" />
                                        </b>
                                    </div>
                                    <div class="card-body">
                                        <div class="row align-items-center">
                                            <div class="col-md-6">
                                                <label style="color: white;"><b>Mês Referência:</b></label>
                                                <uc1:Referencia_MesAno runat="server" ID="Referencia_MesAno" />
                                            </div>
                                            <div class="col-md-6 text-md-end">
                                                <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" />
                                                <asp:Button ID="cmdLimpar" class="btn btn-secondary" runat="server" Text="Limpar Filtros" OnClick="cmdLimpar_Click" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="container">
                                        <hr />
                                    </div>
                                    <br />

                                    <div class="container">
                                        <div class="card-body">
                                            <div class="row align-items-center panel-box">
                                                <div class="mes-ano">
                                                    <b>
                                                        <asp:Label ID="lblTitulo" CssClass="mes-ano-label" runat="server" />
                                                    </b>
                                                </div>

                                                <div class="col-sm-4">
                                                    <div class="card-box tilebox-one">
                                                        <i class="icon-paypal float-right text-muted"></i>
                                                        <h6 class="text-muted text-uppercase mt-0">VR</h6>
                                                        <h2 style="color: black" class="">R$<span data-plugin="counterup">
                                                            <asp:Literal ID="ltrsVR" runat="server" /></span></h2>
                                                        <hr />
                                                        <span class="badge badge-custom">
                                                            <asp:Literal ID="ltrPossuiVR" runat="server" />
                                                        </span>
                                                        <span class="badge badge-custom">
                                                            <asp:Literal ID="ltrnDiaVR" runat="server" />
                                                        </span>
                                                        <span class="badge badge-custom">
                                                            <asp:Literal ID="ltrDiasDescVR" runat="server" />
                                                        </span>
                                                    </div>
                                                </div>

                                                <div class="col-sm-4">
                                                    <div class="card-box tilebox-one">
                                                        <i class="icon-rocket float-right text-muted"></i>
                                                        <h6 class="text-muted text-uppercase mt-0">Plano de Saúde</h6>
                                                        <h2 style="color: black" class=""><span data-plugin="counterup">
                                                            <asp:Literal ID="ltrsPlano" runat="server" /></span></h2>
                                                        <hr />
                                                        <span class="badge badge-custom">Nº Dependentes:
                                                <asp:Literal ID="ltrsQntDependentes" runat="server" /></span>
                                                        <span class="badge badge-custom">Valor do Plano: R$
                                                <asp:Literal ID="ltrnValorPlano" runat="server" /></span>
                                                    </div>
                                                </div>
                                                <div class="col-sm-4">
                                                    <div class="card-box tilebox-one">
                                                        <i class="icon-layers float-right text-muted"></i>
                                                        <h6 class="text-muted text-uppercase mt-0">VT</h6>
                                                        <h2 style="color: black" class="" data-plugin="counterup">R$
                                                             <asp:Literal ID="ltrnVTValor" runat="server" /></h2>
                                                        <hr />
                                                        <span class="badge badge-custom">
                                                            <asp:Literal ID="ltrnDiaVt" runat="server" />
                                                        </span>
                                                        <span class="badge badge-custom">
                                                            <asp:Literal ID="ltrDiasDescVt" runat="server" />
                                                        </span>
                                                    </div>
                                                </div>

                                                <div class="col-sm-4">
                                                    <div class="card-box tilebox-one">
                                                        <i class="icon-layers float-right text-muted"></i>
                                                        <h6 class="text-muted text-uppercase mt-0">Outros Beneficios</h6>
                                                        <h2 style="color: black" class="" data-plugin="counterup">
                                                            <asp:Literal ID="ltrsBeneficios" runat="server" /></h2>
                                                        <span class="badge badge-custom">
                                                            <asp:Literal ID="ltrPossuiOutros" runat="server" />
                                                        </span>
                                                        </h6>
                                                    </div>
                                                </div>

                                            </div>
                                        </div>
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

    <div id="dialog-Salvar" class="modal" title="Salvar">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
        </p>
    </div>

</asp:Content>
