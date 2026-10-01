<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Patrimonio_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Patrimonio.Patrimonio_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <style>
        .colorLink{
            background-color:aqua;
        }
    </style>
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>
    <script src="/app/js/vanilla-masker.js"></script>

    <div class="form-stacked">
        <div class="col-lg-12 row">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Patrimônio"></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
        </div>

        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

        <div>
            <ul id="tab_patrimonio" class="nav nav-tabs" role="tablist">
                <li role="presentation" class="tabpanel active">
                    <a href="#patrimonio" id="aba_patrimonio" role="tabpanel" data-toggle="tab" aria-controls="patrimonio" aria-expanded="false"><b>Patrimônio</b></a>
                </li>
                <li role="presentation" runat="server" id="aba_Arquivo">
                    <a href="#arquivo" role="tab" id="aba_Arquivo-tab" data-toggle="tab" aria-controls="arquivo"><b>Arquivos</b></a>
                </li>
                <li role="presentation" runat="server" id="aba_Movimentacao">
                    <a href="#movimentacao" role="tab" id="aba_Movimentacao-tab" data-toggle="tab" aria-controls="movimentacao"><b>Movimentação</b></a>
                </li>
            </ul>
        </div>

        <br />
        <div id="tab" class="tab-content">

            <div role="tabpanel" class="tab-pane fade in active" id="patrimonio" aria-labelledby="aba_patrimonio">
                <asp:UpdatePanel ID="updDetalhe" runat="server">
                    <ContentTemplate>
                        <div class="panel panel-default" runat="server" id="div_patrimonio">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Dados do Patrimônio</b></h3>
                            </div>
                            <div class="panel-body">

                                <div class="form-stacked row">

                                    <div class="col-lg-4" runat="server" id="DIV_IMG">
                                        <div class="form-group">
                                            <asp:Image ID="imgPatrimonio" runat="server" class="form-control" Height="300" Width="300" Style="display: block; margin-left: auto; margin-right: auto;" />
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">

                                            <label>ID</label>

                                            <div class="form-group input-group">

                                                <asp:LinkButton ID="cmRetornar" class="btn btn-sm btn-info input-group-addon colorLink" runat="server" OnClick="cmdRetornar_click"><b><</b></asp:LinkButton>

                                                <asp:TextBox ID="txtidPatrimonio" class="form-control" runat="server" disabled></asp:TextBox>

                                                <asp:LinkButton ID="cmAvancar" class="btn btn-sm btn-info input-group-addon colorLink" runat="server" OnClick="cmdAvancar_click"><b>></b></asp:LinkButton>
 

                                                <%--<div class="form-stacked row">
                                                    <div class="col-lg-2">
                                                    <asp:Button ID="cmdRetornar" class="btn btn-info" runat="server" Text="<" OnClick="cmdRetornar_click" />
                                                </div>

                                                <div class="col-lg-2">
                                                    <asp:TextBox ID="txtidPatrimonio" class="form-control CaixaTextoMini" runat="server" disabled Width="77px"></asp:TextBox>
                                                </div>

                                                <div class="col-lg-2"></div>

                                                <div class="col-lg-2">
                                                    <asp:Button ID="cmdAvancar" class="btn btn-info" runat="server" Text=">" OnClick="cmdAvancar_click" />
                                                </div>
                                                    </div>--%>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Etiqueta (TAG)</label>
                                            <asp:TextBox ID="txtsEtiquetaPatrimonio" class="form-control " runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>Grupo</label>
                                            <asp:DropDownList ID="ddlidGrupo" class="form-control" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>Categoria</label>
                                            <asp:DropDownList ID="ddlidCategoria" class="form-control" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-8">
                                        <div class="form-group">
                                            <label>Descrição do Patrimonio </label>
                                            <asp:TextBox ID="txtsDscPatrimonio" class="form-control " runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                </div>

                                <div class="form-stacked row">

                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>Marca</label>
                                            <asp:TextBox ID="txtsMarca" class="form-control " runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>Modelo </label>
                                            <asp:TextBox ID="txtsModelo" class="form-control " runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>Cor</label>
                                            <asp:TextBox ID="txtsCor" class="form-control " MaxLength="10" runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>N° de Série</label>
                                            <asp:TextBox ID="txtNumeroSerie" class="form-control " runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-stacked row">
                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>Estado de Conservação</label>
                                            <asp:DropDownList ID="ddlidEstadoConservacao" class="form-control" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>Status</label>
                                            <asp:DropDownList ID="ddlidStatus" class="form-control" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>

                                </div>

                                <div class="form-stacked row">
                                    <div class="col-lg-12">
                                        <div class="form-group">
                                            <label>Observação</label>
                                            <asp:TextBox ID="txtsObservacao" class="form-control" TextMode="multiline" Height="150px" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>

                        <div class="panel panel-default" runat="server" id="Div_Local_Responsável">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Localização e Responsável</b></h3>
                            </div>
                            <div class="panel-body">
                                <div class="form-stacked row">
                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>Local</label>
                                            <asp:DropDownList ID="ddlidLocal" class="form-control Caixa_Selecao" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidLocal_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>Responsável</label>
                                            <asp:DropDownList ID="ddlidResponsavel" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>

                        <div class="panel panel-default" runat="server" id="Div1">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Dados de Compra</b></h3>
                            </div>
                            <div class="panel-body">
                                <div class="form-stacked row">

                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>Fornecedor</label>
                                            <asp:DropDownList ID="ddlidParceiro" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Nota Fiscal</label>
                                            <asp:TextBox ID="txtsNotaFiscal" class="form-control " runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Data da Compra</label>
                                            <asp:TextBox ID="txtsdtCompra" class="form-control" type="date" runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Valor de Compra</label>
                                            <asp:TextBox ID="txtnValorCompra" class="form-control " runat="server"></asp:TextBox>
                                        </div>
                                    </div>


                                </div>

                            </div>
                        </div>

                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlidLocal" EventName="SelectedIndexChanged" />
                        <%--  <asp:AsyncPostBackTrigger ControlID="cmdAvancar" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="cmdRetornar" EventName="Click" />--%>
                    </Triggers>
                </asp:UpdatePanel>
                <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                <br />
            </div>


            <div role="tabpanel" class="tab-pane fade" id="arquivo" aria-labelledby="aba_Arquivo-tab">
                <div class=" embed-responsive embed-responsive-16by9" style="min-height: 200px">
                    <embed type="text/html" runat="server" id="frmArquivos" width="800" height="500" />
                </div>
            </div>


            <div role="tabpanel" class="tab-pane fade" id="movimentacao" aria-labelledby="aba_Movimentacao-tab">
                <h1>Não Implementado Ainda!</h1>
            </div>

        </div>

        <%----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>


        <fieldset class="form-stacked actions">
            <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-info" runat="server" Text="Salvar" ValidationGroup="DETALHE" />
            <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field-cancel" title="Voltar" onclick="history.go(-2)">
        </fieldset>
        <div id="dialog-Salvar" class="modal" title="Salvar">
            <p>
                <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
            </p>
        </div>

    </div>


    <asp:HiddenField ID="hddidPatrimonio" runat="server" />
    <asp:HiddenField ID="hddidProduto" runat="server" />
    <asp:HiddenField ID="hddidResponsavel" runat="server" />
</asp:Content>
