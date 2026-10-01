<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="EntregaEPI.aspx.cs" Inherits="TT_Colaborador.Aplicativo.Paginas.EPI.EntregaEPI" %>

<%@ Register Src="~/Aplicativo/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/Aplicativo/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmEntregaEPI" ContentPlaceHolderID="MainContent" runat="server">

    <div id="DIV_ESPACO" class="h-100" runat="server" style="display: none;"></div>

    <asp:UpdatePanel ID="updDetalhe" runat="server">
        <ContentTemplate>

            <style>
                .id {
                    display: none;
                }

                .efeito {
                    cursor: pointer;
                    border: 3px solid lawngreen;
                }

                    .efeito:hover {
                        background-color: #f0f0f0;
                    }

                .a {
                    text-decoration: none !important;
                    color: black !important;
                }

                .altura-scroll {
                    max-height: 600px;
                    overflow-y: scroll;
                }

                /*----------------------------------------------------------------*/

                /* Estilos para Dispositivos Móveis */

                @media only screen and (max-device-width: 768px) {

                    .altura-scroll {
                        max-height: 1200px;
                    }

                    .message-content p {
                        font-size: 2rem !important;
                    }
                }

                /*----------------------------------------------------------------*/
            </style>

            <div id="hdd">
                <asp:HiddenField ID="hddidColaborador" runat="server" />
                <asp:HiddenField ID="hddidEntregaEPI" runat="server" />
            </div>

            <div class="card">

                <div class="card-header card-tt">
                    <h3 class="card-title" style="color: white"><b>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Entrega de EPI"></asp:Label></b>
                    </h3>
                </div>

                <div id="div_entregas" runat="server" class="card-body">

                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Entregas" />

                    <div class="card card-body altura-scroll">

                        <asp:Repeater ID="rptEntregas" runat="server" OnItemCommand="rptEntregas_ItemCommand">
                            <ItemTemplate>

                                <div class="card card-body efeito" style="margin: 5px 0; height: auto; width: 100%;">

                                    <div class="message-box">
                                        <div class="message-content">
                                            <asp:LinkButton runat="server" ID="lnkEntrega" CommandArgument='<%# Eval("idEntregaEPI") %>' CssClass="a">

                                                <p>
                                                    <asp:Literal runat="server" ID="lt_ID" Text='<%# string.Format("<b>ID:</b> {0}", Eval("idEntregaEPI")) %>' />
                                                </p>

                                                <p>
                                                    <asp:Literal runat="server" ID="lt_Data" Text='<%# Eval("dtEntrega") %>' />
                                                </p>

                                                <p>
                                                    <asp:HiddenField runat="server" ID="hddidStatus" Value='<%# Eval("idStatus") %>' />
                                                    <asp:Literal runat="server" ID="lt_Status" Text='<%# string.Format("<b>Status:</b> {0}", Eval("sDscStatus")) %>' />
                                                </p>

                                                <p>
                                                    <asp:Literal runat="server" ID="lt_Solicitacao" Text='<%# string.Format("<b>Obs:</b> <br />{0}", Eval("sDscObservacao")) %>' />
                                                </p>

                                            </asp:LinkButton>
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

                <div id="div_dados" runat="server" class="col-lg-12">

                    <div class="card">
                        <div class="card-body">

                            <div class="row">

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <b>
                                            <label>Status</label></b>
                                        <asp:TextBox ID="txtStatus" class="form-control" runat="server" disabled=""></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <b>
                                            <label>Tipo de Entrega</label></b>
                                        <asp:TextBox ID="txtEntregaPontual" class="form-control" runat="server" disabled=""></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <b>
                                            <label>Observação</label></b>
                                        <asp:TextBox ID="txtObs" class="form-control" runat="server" TextMode="MultiLine" Height="150px" disabled=""></asp:TextBox>
                                    </div>
                                </div>

                                <div runat="server" id="div_gvEPI" class="col-lg-12">
                                    <asp:GridView ID="gvEPI" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                        <Columns>

                                            <asp:BoundField DataField="sCodigoEPI" HeaderText="Código do EPI">
                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscEPI" HeaderText="Descrição do EPI">
                                                <ItemStyle Width="40%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nCA" HeaderText="CA">
                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nQuantidadeEPI" HeaderText="Quantidade">
                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscPeriodicidade" HeaderText="Periodicidade">
                                                <ItemStyle Width="20%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                        </Columns>
                                    </asp:GridView>
                                </div>

                                <div class="row">
                                    <div class="col-lg-12">
                                        <fieldset class="form-stacked actions">
                                            <asp:Button ID="cmdConfirmar" class="btn btn-md btn-success" runat="server" Text="Confirmar" />
                                            <asp:Button ID="cmdVoltar" class="btn btn-md btn-warning" runat="server" Text="Voltar" OnClick="fechar_click" />
                                        </fieldset>

                                        <div id="dialog-Confirmar" class="modal" title="Confirmar">
                                            <p>
                                                <asp:Label ID="lblTituloConfirmar" runat="server" Text="Deseja Confirmar a Entrega de EPI?"></asp:Label>
                                            </p>
                                        </div>
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
