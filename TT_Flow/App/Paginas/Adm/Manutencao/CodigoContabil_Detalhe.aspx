<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="CodigoContabil_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Manutencao.LancamentoContabil_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>


    <script type="text/javascript">

        $(function () {


        });

    </script>


    <div class="form-stacked" style="">
        <div class="col-lg-12 row">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text=""></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
        </div>

        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />


        <div>
            <ul id="tab_Contabil" class="nav nav-tabs" role="tablist">
                <li role="presentation" class="tabpanel active">
                    <a href="#contabil" id="aba_Contabil" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Código Contábil</b></a>
                </li>

                <li role="presentation" runat="server" id="aba_Historico">
                    <a href="#historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="Historioco"><b>Histórico</b></a>
                </li>
            </ul>

        </div>
        <div id="tab" class="tab-content">
            <div role="tabpanel" class="tab-pane fade in active" id="contabil" aria-labelledby="contabil-tab">

                <asp:UpdatePanel ID="updDetalhe" runat="server">
                    <ContentTemplate>
                        <br />


                        <div class="panel panel-default" runat="server" id="div_Contabil">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Dados do Código Contábil</b></h3>
                            </div>
                            <div class="panel-body">

                                <div class="form-stacked row">
                                    <div class="col-lg-12">
                                        <div class="form-group">
                                            <label>ID </label>
                                            <asp:TextBox ID="txtidContabil" class="form-control CaixaTextoMini" runat="server" disabled></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-12">
                                        <div class="form-group">
                                            <label>Código Contábil </label>
                                            <asp:TextBox ID="txtsCodContabil" class="form-control CaixaTextoPequeno" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-lg-12">
                                        <div class="form-group row">

                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>Código Contábil Pai</label>
                                                    <asp:DropDownList ID="ddlidContabil" class="form-control " runat="server"></asp:DropDownList>
                                                </div>
                                            </div>

                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Exibe em Patrimônio</label>
                                            <asp:DropDownList ID="ddlsExibirPatrimonio" runat="server" class="form-control CaixaTextoMini yes_no select ">
                                                <asp:ListItem Value="S">Sim</asp:ListItem>
                                                <asp:ListItem Selected="True" Value="N">Não</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Exibe em Financeiro</label>
                                            <asp:DropDownList ID="ddlsExibirFinanceiro" runat="server" class="form-control CaixaTextoMini yes_no select ">
                                                <asp:ListItem Value="S">Sim</asp:ListItem>
                                                <asp:ListItem Selected="True" Value="N">Não</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-lg-12">
                                        <div class="form-group row">

                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>Descrição Código </label>
                                                    <asp:TextBox ID="txtsDscCodContabil" class="form-control " runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                        </div>
                                    </div>                                        

                                </div>
                            </div>
                        </div>
                        </div>

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
                                        <asp:BoundField DataField="sCodContabil" HeaderText="Código Contábil">
                                            <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="sDscCodContabil" HeaderText="Descrição Código">
                                            <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="sDscCodContabilPai" HeaderText="Código Contábil Pai">
                                            <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="dtAtualizacao" HeaderText="Data de Atualização">
                                            <ItemStyle Width="8%" HorizontalAlign="center" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                    </Columns>
                                </asp:GridView>

                            </div>
                        </div>
                    </div>
                </div>
            </div>

                       
                    </ContentTemplate>
                </asp:UpdatePanel>

                <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                <br />
                 </div>

            </div>

            <%----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>
       

        <fieldset class="form-stacked actions">
            <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-info" runat="server" Text="Salvar" ValidationGroup="DETALHE" />
            <asp:Button ID="cmdEditar" class="btn  btn-lg  btn-danger" runat="server" Text="Editar" />

            <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field-cancel" title="Voltar" onclick="history.go(-2)">
        </fieldset>
        <div id="dialog-Salvar" class="modal" title="Salvar">
            <p>
                <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
            </p>
        </div>
        <div id="dialog-Editar" class="modal" title="Editar Lançamento">
            <p>
                <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                <asp:Label ID="lblTituloEdiar" runat="server" Text="Confirma a Edição do Lançamento?"></asp:Label>
            </p>
        </div>
    </div>


    <asp:HiddenField ID="hddidContabil" runat="server" />
</asp:Content>
