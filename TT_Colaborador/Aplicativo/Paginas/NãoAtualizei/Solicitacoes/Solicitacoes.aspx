<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Solicitacoes.aspx.cs" Inherits="TT_Colaborador.Aplicativo.Paginas.Solicitacoes.Solicitacoes" %>

<%@ Register Src="~/Aplicativo/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/Aplicativo/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmSolicitacoes" ContentPlaceHolderID="MainContent" runat="server">

    <div id="DIV_ESPACO" class="h-100" runat="server" style="display: none;"></div>

    <asp:UpdatePanel ID="updDetalhe" runat="server">
        <ContentTemplate>

            <style>
                .altura-scroll {
                    max-height: 600px;
                    overflow-y: scroll;
                }

                .efeito {
                    margin: 5px 0;
                    height: auto;
                    width: 100%;
                }

                    .efeito:hover {
                        background-color: #f0f0f0;
                    }

                #div_torta {
                    width: 66.5%;
                }

                /*----------------------------------------------------------------*/

                /* Estilos para Dispositivos Móveis */

                @media only screen and (max-device-width: 768px) {

                    .altura-scroll {
                        max-height: 1200px;
                    }

                    #div_torta {
                        width: 100% !important;
                    }

                    .message-content p {
                        font-size: 2rem !important;
                    }
                }

                /*----------------------------------------------------------------*/
            </style>

            <div id="hdd">
                <asp:HiddenField ID="hddidColaborador" runat="server" />
                <asp:HiddenField ID="hddidSolicitacao" runat="server" />
            </div>

            <div class="card">

                <div class="card-header card-tt">
                    <h3 class="card-title" style="color: white"><b>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Solicitações"></asp:Label></b>
                        <small class="text-muted"></small></h1>
                    </h3>
                </div>

                <div id="div_solicitacoes" runat="server" class="card-body">
                    <div class="row">

                        <div class="col-lg-4">
                            <div class="card">
                                <div class="card-header bg-primary text-white">
                                    <h3 class="card-title"><i class="fa fa-bar-chart me-2"></i>Filtro para pesquisa </h3>
                                </div>

                                <div class="card-body">

                                    <div class="row">
                                        <div class="col-lg-12">
                                            <asp:TextBox ID="txtdtInicial" type="date" placeholder="dd/MM/yyyy" CssClass="form-control CaixaTextoGrande" runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <hr />

                                    <div class="row">
                                        <div class="col-lg-12">
                                            <asp:TextBox ID="txtdtFinal" type="date" placeholder="dd/MM/yyyy" CssClass="form-control CaixaTextoGrande" runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <hr />

                                    <div class="row">
                                        <div class="col-lg-12">
                                            <asp:DropDownList ID="ddlidTipoSolicitacao" CssClass="form-select Caixa_Selecao" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <hr />

                                    <div class="row">
                                        <div class="col-lg-12">
                                            <asp:TextBox ID="txtPesquisa" CssClass="form-control CaixaTextoGrande" placeholder="Pesquisar Avisos" runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <hr />

                                    <div class="row">
                                        <div class="col-lg-12">
                                            <asp:Button ID="cmdPesquisar" CssClass="btn btn-md btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" />
                                            <asp:Button ID="cmdNovaSolicitacao" class="btn btn-md btn-success" runat="server" Text="Nova Solicitação" OnClick="cmdNovaSolicitacao_Click" />
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>

                        <div id="div_torta" class="col-lg-8">
                            <div class="card card-body altura-scroll">

                                <uc1:MensagemPagina runat="server" ID="MensagemPagina_Solicitacoes" />

                                <asp:Repeater ID="rptSolicitacoes" runat="server" OnItemDataBound="rptSolicitacoes_ItemDataBound">
                                    <ItemTemplate>

                                        <div runat="server" id="div_Efeito" class="card card-body efeito">

                                            <div class="message-box">
                                                <div runat="server" id="div_message" class="message-content">

                                                    <div runat="server" id="div_Solicitacao">

                                                        <p>
                                                            <asp:Literal runat="server" ID="lt_ID" Text='<%# string.Format("<b>ID:</b> {0}", Eval("idSolicitacao")) %>' />
                                                            &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
                                                            <asp:Literal runat="server" ID="lt_Tipo" Text='<%# string.Format("<b> Tipo:</b> {0}", Eval("sTipo")) %>' />
                                                        </p>

                                                        <p>
                                                            <asp:Literal runat="server" ID="lt_Data" Text='<%# Eval("dtSolicitacao") %>' />
                                                        </p>

                                                        <p>
                                                            <asp:Literal runat="server" ID="lt_Status" Text='<%# string.Format("<b>Status:</b> {0}", Eval("sStatus")) %>' />
                                                        </p>

                                                        <p>
                                                            <asp:Literal runat="server" ID="lt_Solicitacao" Text='<%# string.Format("<b>Solicitação:</b> <br />{0}", Eval("sDscSolicitacao")) %>' />
                                                        </p>

                                                    </div>

                                                    <asp:LinkButton runat="server" ID="lnk_Solicitacao" CommandArgument='<%# Eval("idSolicitacao") %>' OnClick="AbrirSolicitacao_Click" style="text-decoration: none; color: black;">

                                                        <p>
                                                            <asp:Literal runat="server" ID="lt_ID__lnk" Text='<%# string.Format("<b>ID:</b> {0}", Eval("idSolicitacao")) %>' />
                                                            &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
                                                            <asp:Literal runat="server" ID="lt_Tipo__lnk" Text='<%# string.Format("<b> Tipo:</b> {0}", Eval("sTipo")) %>' />
                                                        </p>

                                                        <p>
                                                            <asp:Literal runat="server" ID="lt_Data__lnk" Text='<%# Eval("dtSolicitacao") %>' />
                                                        </p>

                                                        <p>
                                                            <asp:Literal runat="server" ID="lt_Status__lnk" Text='<%# string.Format("<b>Status:</b> {0}", Eval("sStatus")) %>' />
                                                        </p>

                                                        <p>
                                                            <asp:Literal runat="server" ID="lt_Solicitacao__lnk" Text='<%# string.Format("<b>Solicitação:</b> <br />{0}", Eval("sDscSolicitacao")) %>' />
                                                        </p>

                                                    </asp:LinkButton>
                                                    <asp:HiddenField runat="server" ID="hddsLiberadoColaborador" Value='<%# Eval("sLiberaColaborador") %>' />

                                                </div>
                                            </div>

                                        </div>

                                    </ItemTemplate>
                                </asp:Repeater>

                                <fieldset class="form-stacked actions">
                                    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                                </fieldset>

                            </div>
                        </div>

                    </div>
                </div>

                <div id="div_dados" runat="server" class="card-body">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Principal" />

                    <div class="card">
                        <div class="card-body">
                            <div class="row">

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label><b>Tipo de Solicitação</b></label>
                                        <asp:DropDownList ID="ddlidTipo" class="form-select" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidTipo_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>

                                <div runat="server" id="divFormulario_Solicitacao">

                                    <div runat="server" id="div_Solicitacao" class="row">
                                        <div class="col-lg-12" style="padding: 0;">
                                            <div class="col-lg-2" runat="server" id="div_dtInicio">
                                                <div class="form-group">
                                                    <label><b runat="server" id="lblDtInicial">Data Inicial</b></label>
                                                    <asp:TextBox ID="txtsdtInicio" class="form-control" type="date" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2" runat="server" id="div_dtFinal">
                                                <div class="form-group">
                                                    <label><b runat="server" id="lblDtFim">Data Final Férias</b></label>
                                                    <asp:TextBox ID="txtsdtFinal" class="form-control" type="date" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-12" runat="server" id="div_sDscSolicitacao">
                                            <div class="form-group">
                                                <label><b>Motivo da Solicitação</b></label>
                                                <asp:TextBox ID="txtsDscSolicitacao" TextMode="multiline" Height="250px" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-12">
                                    <fieldset class="form-stacked actions">
                                        <asp:Button ID="cmdSalvar" class="btn btn-md btn-success" runat="server" Text="Salvar" ValidationGroup="DETALHE" />
                                        <asp:Button ID="btnFechar" class="btn btn-md btn-warning" runat="server" Text="Voltar" OnClick="fechar_click" />
                                    </fieldset>

                                    <div id="dialog-Salvar" class="modal" title="Salvar">
                                        <p>
                                            <asp:Label ID="lblTituloSalvar" runat="server" Text="Deseja salvar a Nova Solicitação?"></asp:Label>
                                        </p>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>

                    <br />

                    <div runat="server" id="divFormArquivo" class="card">
                        <div class="card-body">
                            <div id="divArquivos" runat="server">
                                <div runat="server" id="div_arquivos">
                                    <div class=" embed-responsive embed-responsive-16by9" style="min-height: 200px">
                                        <embed type="text/html" runat="server" id="frmArquivos" width="100%" height="550" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>

            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
