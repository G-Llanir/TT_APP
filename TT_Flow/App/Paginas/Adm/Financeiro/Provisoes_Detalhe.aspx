<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Provisoes_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Financeiro.Provisoes_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>



    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Provisões"></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>
    </div>

    <div>
        <ul id="tab_Provisao" class="nav nav-tabs" role="tablist">

            <li role="presentation" class="tabpanel active">
                <a href="#provisao" id="aba_Provisaoa" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Provisão</b></a>
            </li>

            <li role="presentation" runat="server" id="aba_Historico">
                <a href="#historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="Historico"><b>Histórico</b></a>
            </li>
        </ul>
    </div>

    <div id="tab" class="tab-content">

        <div role="tabpanel" class="tab-pane fade in active" id="provisao" aria-labelledby="Provisao-tab">
            <br />
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>

                    <div class="panel panel-default" runat="server" id="div_Provisao">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Detalhe</b></h3>
                        </div>

                        <div class="panel-body">
                            <div class="form-stacked row">

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>ID</label>
                                        <asp:TextBox ID="txtidProvisao" class="form-control CaixaTextoMini" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Descrição da Provisão</label>
                                        <asp:TextBox ID="txtsDscProvisao" class="form-control " MaxLength="100" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Data ínicio</label>
                                        <asp:TextBox ID="txtdtInicioProvisao" class="form-control " runat="server" type="date"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Data vencimento</label>
                                        <asp:TextBox ID="txtdtFinalProvisao" class="form-control " runat="server" type="date"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Valor</label>
                                        <asp:TextBox ID="txtnValorProvisao" class="form-control " runat="server"></asp:TextBox>
                                    </div>
                                </div>


                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Empresa </label>
                                        <asp:DropDownList ID="ddlidEmpresa" class="form-control " runat="server" attrname="idProvisao"></asp:DropDownList>
                                    </div>
                                </div>

                                         <div class="col-lg-4">
                                             <div class="form-group">
                                                 <label>Categoria</label>
                                                 <asp:DropDownList ID="ddlidCategoriaPagar" class="form-control Caixa_Selecao" runat="server" attrname="idContasPagar"></asp:DropDownList>
                                             </div>
                                         </div>

                                         <div class="col-lg-4">
                                             <div class="form-group">
                                                 <label>Categoria</label>
                                                 <asp:DropDownList ID="ddlidCategoriaReceber" class="form-control Caixa_Selecao" runat="server" attrname="idContasReceber" ></asp:DropDownList>
                                             </div>
                                         </div>


                                <div class="col-lg-12">
                                    <div class="row">
                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>Tipo Provisão</label>
                                                <asp:DropDownList ID="ddlidTipoProvisao" runat="server" class="form-control yes_no select" AutoPostBack="true" OnSelectedIndexChanged="ddlidTipoProvisao_SelectedIndexChanged">
                                                    <asp:ListItem Selected="True" Value="1">única</asp:ListItem>
                                                    <asp:ListItem Value="2">Parcelada</asp:ListItem>
                                                    <asp:ListItem Value="3">Fixa</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" id="div_Parcelas" runat="server">
                                            <div class="form-group">
                                                <label>N° Parcelas</label>
                                                <asp:TextBox ID="txtnParcelasProvisao" class="form-control " runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" id="div_Periodicidade" runat="server">
                                            <div class="form-group">
                                                <label>Periodicidade</label>
                                                <asp:DropDownList ID="ddlidPeriodicidade" runat="server" class="form-control yes_no select" AutoPostBack="true" OnSelectedIndexChanged="ddlidPeriodicidade_SelectedIndexChanged">
                                                    <asp:ListItem Selected="True" Value="0">Selecione</asp:ListItem>
                                                    <asp:ListItem Value="1">Diaria</asp:ListItem>
                                                    <asp:ListItem Value="2">Semanal</asp:ListItem>
                                                    <asp:ListItem Value="3">Mensal</asp:ListItem>
                                                    <asp:ListItem Value="4">Anual</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" id="div_Repeticao" runat="server">
                                            <div class="form-group">
                                                <label>Repetir a cada</label>
                                                <asp:TextBox ID="txtnRepetocoes" class="form-control " runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                   </div>



                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Banco </label>
                                        <asp:DropDownList ID="ddlidBanco" class="form-control " runat="server" attrname="idProvisao"></asp:DropDownList>
                                    </div>
                                </div>


                                <div class="col-lg-12">
                                    <div class="row">
                                        <div class="col-lg-2" id="div_ComissaoPaga" runat="server">
                                            <div class="form-group">
                                                <label>Status</label>
                                                <asp:DropDownList ID="ddlsAtivo" runat="server" class="form-control yes_no select">
                                                    <asp:ListItem Selected="True" Value="S">Sim</asp:ListItem>
                                                    <asp:ListItem Value="N">Não</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>


                            </div>
                        </div>

                        </div>


                        <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                </ContentTemplate>
            </asp:UpdatePanel>

        </div>


        <div role="tabpanel" class="tab-pane fade" id="historico" aria-labelledby="historico-tab">
            <br />
            <div class="panel panel-default" runat="server" id="DIV_historico">
                <div class="panel-heading">

                    <h3 class="panel-title"><b>Histórico</b></h3>
                </div>
                <div class="panel-body">
                    <div class="form-stacked row">
                        <div class="col-lg-12 table-responsive">
                            <asp:GridView
                                ID="gv_Historico" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                ShowFooter="False">
                                <Columns>
                                    <asp:BoundField DataField="sDscProvisao" HeaderText="Descrção">
                                        <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                </Columns>
                            </asp:GridView>

                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>


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

    <asp:HiddenField ID="hddidProvisao" runat="server" />



    <%----------------------------------------------------------------------------------------------------------------------------------------------%>
</asp:Content>
