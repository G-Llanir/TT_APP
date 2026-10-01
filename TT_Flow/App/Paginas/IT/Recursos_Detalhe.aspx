<%@ Page Title="" Language="C#" ValidateRequest="false" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Recursos_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.IT.Recursos_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <asp:UpdatePanel runat="server" ID="Upd_Recursos">
        <ContentTemplate>

            <style>
                .div_EditarMenu {
                    display: flex;
                    flex-wrap: wrap;
                }

                .linksMenu {
                    text-align: left;
                    max-width: 10%;
                    width: 3em;
                    min-width: 2em;
                }

                    .linksMenu tr {
                        height: 40.3px;
                    }

                .editarMenu {
                    text-align: left;
                    max-width: 50%;
                    width: 35em;
                    min-width: 20em;
                }

                    .editarMenu ul {
                        border: 1px solid lightgray;
                        border-width: 0.5px 1px;
                        border-radius: 5px;
                    }

                        .editarMenu ul.nav li a {
                            height: 40.25px;
                        }

                span.fa {
                    float: right;
                }

                .recursoAtual {
                    border: 1px solid red;
                    border-radius: 5px;
                }

                .recursoPai {
                    border: 1px dashed red;
                    border-radius: 5px;
                }

                .cmdMenu {
                    cursor: pointer;
                }
            </style>

            <asp:HiddenField runat="server" ID="hddIcones" Value="|" />

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Permissão"></asp:Label></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>
            </div>

            <div class="panel panel-default" id="pnPermissao">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Permissão</b></h3>
                </div>
                <div class="panel-body">
                    <div class="form-stacked row">

                        <div class="col-lg-12">
                            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                        </div>

                        <div class="col-lg-12" runat="server" id="div_idRecurso">
                            <div class="form-group row">
                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <label>ID</label>
                                        <asp:TextBox ID="txtidRecurso" runat="server" class="form-control CaixaTextoMini" disabled=""></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-12" runat="server" id="div_idSistema">
                            <div class="form-group row">
                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Sistema</label>
                                        <asp:DropDownList ID="ddlidSistema" runat="server" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlidSistema_SelectedIndexChanged">
                                            <asp:ListItem Value="0" Text="Selecione um Sistema"></asp:ListItem>
                                            <asp:ListItem Value="1" Text="HUB"></asp:ListItem>
                                            <asp:ListItem Value="2" Text="T-Flow"></asp:ListItem>
                                            <asp:ListItem Value="3" Text="Cloud-TTG"></asp:ListItem>
                                            <asp:ListItem Value="4" Text="Webmail"></asp:ListItem>
                                            <asp:ListItem Value="5" Text="Usuário"></asp:ListItem>
                                            <asp:ListItem Value="6" Text="Logoff"></asp:ListItem>
                                            <asp:ListItem Value="7" Text="Help Desk"></asp:ListItem>
                                            <asp:ListItem Value="8" Text="Área Colaborador"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-12" runat="server" id="div_sDscRecurso">
                            <div class="form-group row">
                                <div class="col-lg-7">
                                    <div class="form-group">
                                        <label>Descrição</label>
                                        <asp:TextBox ID="txtsDscRecurso" runat="server" class="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-12">
                            <div class="form-group row">
                                <div class="col-lg-6" runat="server" id="div_idRecursoPai">
                                    <div class="form-group">
                                        <label>Permissão Pai</label>
                                        <asp:DropDownList ID="ddlidRecursoPai" runat="server" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlidRecursoPai_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-12">
                            <div class="form-group row">
                                <div class="col-lg-1" runat="server" id="div_sMenu">
                                    <div class="form-group">
                                        <uc1:SwitchAtivo runat="server" ID="SwitchMenu" />
                                    </div>
                                </div>
                                <div class="col-lg-1" runat="server" id="div_sPaginaInicial">
                                    <div class="form-group">
                                        <uc1:SwitchAtivo runat="server" ID="SwitchPaginaInicial" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-12">
                            <div class="form-group row">
                                <div class="col-lg-4" runat="server" id="div_sURL">
                                    <div class="form-group">
                                        <label>URL</label><small> (Opcional)</small>
                                        <asp:TextBox ID="txtsURL" runat="server" class="form-control" placeholder="/App/Paginas/.../Exemplo.aspx"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-3" runat="server" id="div_sIcone">
                                    <div class="form-group">
                                        <label>Ícone</label><small> (Opcional)</small>
                                        <asp:TextBox ID="txtsIcone" runat="server" class="form-control" placeholder="fa fa-exemplo fa-fw" AutoPostBack="true" OnTextChanged="txtsIcone_TextChanged"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-2" runat="server" id="div_PreviaIcone">
                                    <label>Prévia do Ícone</label>
                                    <div style="width: 100%;">
                                        <asp:Literal ID="ltrPreviaIcone" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-12">
                            <div class="form-stacked row" runat="server" id="divAtivo">
                                <div class="col-lg-1">
                                    <uc1:SwitchAtivo runat="server" ID="SwitchAtivo" />
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-12">
                            <div runat="server" id="pnMenu" class="panel panel-default" style="margin: 0;">
                                <div class="panel-heading cmdMenu" style="display: flex; justify-content: space-between;">
                                    <h3 class="panel-title"><b>Posição no Menu</b></h3>
                                    <span class="fa fa-chevron-up"></span>
                                </div>
                                <div class="panel-body">
                                    <div class="form-stacked row">

                                        <div class="col-lg-2 form-group">
                                            <label>Ordem <i runat="server" id="icon_nOrdem" class="fa fa-question-circle" data-toggle="tooltip" data-html="true" title=""></i></label>
                                            <asp:TextBox ID="txtnOrdem" runat="server" class="form-control"></asp:TextBox>
                                        </div>

                                        <div runat="server" id="div_txtnOrdem_Menu" class="col-lg-2 form-group">
                                            <label>Andar no Menu <i runat="server" id="icon_nOrdem_Menu" class="fa fa-question-circle" data-toggle="tooltip" data-html="true" title=""></i></label>
                                            <asp:TextBox ID="txtnOrdem_Menu" runat="server" class="form-control"></asp:TextBox>
                                        </div>

                                        <div runat="server" id="div_PosicaoMenu" class="col-lg-12 div_EditarMenu">
                                            <label style="width: 100%;">Visualização da Posição no Menu</label>
                                            <asp:Literal ID="ltrPosicaoMenu" runat="server"></asp:Literal>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                <input type="submit" name="cancel" class="btn btn-lg btn-warning" id="field_cancel" value="Voltar" title="Voltar" onclick="history.go(-1)" runat="server" />
            </fieldset>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>