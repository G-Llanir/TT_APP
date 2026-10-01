<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Usuario_Colaborador.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.Usuario_Colaborador" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>
    <script type="text/javascript">
        function MudarBotao() {
            document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
        }
    </script>
    <div class="form-stacked">
        <div class="col-lg-12 row">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Associar Usuário"></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
        </div>

        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />


        <div>
            <ul id="tab_usuarioColaborador" class="nav nav-tabs" role="tablist">
                <li role="presentation" class="tabpanel active">
                    <a href="#usuarioColaborador" id="aba_usuarioColaborador" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Associar Usuário</b></a>
                </li>
            </ul>
        </div>

        <div id="tab" class="tab-content">
            <div role="tabpanel" class="tab-pane fade in active" id="usuarioColaborador" aria-labelledby="usuarioColaborador-tab">

                <asp:UpdatePanel ID="updDetalhe" runat="server">
                    <ContentTemplate>
                        <br />
                        <div class="panel panel-default" runat="server" id="div_usuarioColaborador">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Associar Usuário</b></h3>
                            </div>
                            <div class="panel-body">

                                <div class="form-stacked row">
                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>Nome do Colaborador</label>
                                            <asp:DropDownList ID="ddlidColaboradores" class="form-control Caixa_Selecao" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidColaboradores_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>Nome do Usuário Associado</label>
                                            <asp:DropDownList ID="ddlidUsuarioAssociado" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>

                           <%--         <div class="col-lg-4">
                                        <div class="form-group">
                                            <br />
                                            <asp:Button class="btn  btn-primary" Text="Incluir" runat="server" />
                                        </div>
                                    </div>--%>
                                </div>
                        <%--        <div class="form-stacked-row">
                                    TABELA DE INCLUSÃO AQUI
                                </div>--%>

                            </div>
                        </div>

                        <div class="panel panel-default" runat="server" id="Div_UsuariosAssociados">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Usuários Associados</b></h3>
                            </div>
                            <div class="panel-body">
                                <div class="table-responsive">
                                    <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">


                                        <Columns>

                                            <asp:HyperLinkField DataNavigateUrlFields="idColaborador"
                                                DataTextField="sDscColaborador" HeaderText="Colaborador"
                                                DataNavigateUrlFormatString="Usuario_Colaborador.aspx?id={0}">
                                                <HeaderStyle Width="20%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idColaborador"
                                                DataTextField="sDscUsuarioIntegrado" HeaderText="Usuário Associado"
                                                DataNavigateUrlFormatString="Usuario_Colaborador.aspx?id={0}">
                                                <HeaderStyle Width="20%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>

                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlidColaboradores" EventName="SelectedIndexChanged" />
                    </Triggers>
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

    </div>


    <asp:HiddenField ID="hddidColaborador" runat="server" />
</asp:Content>
