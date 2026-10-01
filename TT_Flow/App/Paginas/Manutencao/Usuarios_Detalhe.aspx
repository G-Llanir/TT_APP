<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Usuarios_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.Usuarios_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Recursos.ascx" TagPrefix="uc1" TagName="Recursos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <br />
            <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Usuários" />
            <div class="well">
                <h4>
                    <span id="MainContent_lblTituloPagina">
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Usuário"></asp:Label>
                    </span>
                </h4>
            </div>

            <div id="abas">
                <ul id="tab_guias" class="nav nav-tabs" role="tablist">
                    <li role="presentation" class="tabpanel active" id="tab_dados">
                        <a href="#dados" id="aba_Dados" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Dados</b></a>
                    </li>
                    <li role="presentation" runat="server" id="tab_Departamentos">
                        <a href="#Departamentos" role="tab" id="aba_departamentos" data-toggle="tab" aria-controls="departamentos"><b>Departamentos</b></a>
                    </li>
                    <li role="presentation" runat="server" id="tab_permissoes">
                        <a href="#permissoes" role="tab" id="aba_permissoes" data-toggle="tab" aria-controls="permissoes"><b>Permissões</b></a>
                    </li>
                    <li role="presentation" runat="server" id="tab_Empresa">
                        <a href="#Empresa" role="tab" id="aba_empresa" data-toggle="tab" aria-controls="empresa"><b>Empresa</b></a>
                    </li>
                </ul>
            </div>

            <div id="tab" class="tab-content">

                <div role="tabpanel" class="tab-pane fade in active" id="dados" aria-labelledby="aba_Dados">
                    <asp:UpdatePanel ID="updUsuario" runat="server" ChildrenAsTriggers="True">
                        <ContentTemplate>
                            <br />
                            <div class="panel panel-default">

                                <div class="panel-heading">
                                    <div class="d-flex fw-no space-b">
                                        <h3 class="panel-title"><b>Dados do Usuário</b></h3>
                                        <asp:LinkButton runat="server" ID="cmdLoginUsuario_DEV" CssClass="btn btn-info form-control" Text="Efetuar Login" Width="20%" OnClick="cmdLoginUsuario_DEV_Click" />
                                    </div>
                                </div>
                                <div class="panel-body">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                                    <div class="row">
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <div class="col-lg-6 row">
                                                    <label>Login</label>
                                                    <div class="input-group">
                                                        <div class="input-group-addon" runat="server" id="DIV_PrefixoLogin">
                                                            <b>
                                                                <asp:Label ID="lblsPrefixoLogin" runat="server" Text=""></asp:Label></b>
                                                        </div>
                                                        <asp:TextBox ID="txtsLogin" class="form-control" runat="server" MaxLength="50"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <label>Nome</label>
                                                <asp:TextBox ID="txtsDsUsuario" class="form-control " runat="server" MaxLength="200"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" style="padding-right: 0;">
                                            <div class="form-group">
                                                <label>Senha</label>
                                                <asp:TextBox ID="txtsSenha" class="form-control" runat="server" MaxLength="50" TextMode="Password"></asp:TextBox>
                                                <asp:TextBox ID="txtsSenha1" class="form-control" runat="server" MaxLength="50" OnTextChanged="txtsSenha_TextChanged"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-4" style="padding-left: 0;">
                                            <div class="form-group">
                                                <label>&nbsp;</label>
                                                <br />
                                                <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-small fa fa-eye" TabIndex="100" ToolTip="Visualizar Senha" OnClick="LinkButton1_Click"></asp:LinkButton>
                                                <asp:LinkButton ID="LinkButton3" runat="server" CssClass="btn btn-small fa fa-eye-slash" TabIndex="100" ToolTip="Esconder Senha" OnClick="LinkButton3_Click"></asp:LinkButton>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" style="padding-right: 0;">
                                            <div class="form-group">
                                                <label>Confirmação de Senha</label>
                                                <asp:TextBox ID="txtsSenha_Confirmacao" class="form-control" runat="server" MaxLength="50" TextMode="Password" OnTextChanged="txtsSenha_Confirmacao_TextChanged"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-4" style="padding-left: 0;">
                                            <div class="form-group">
                                                <label>&nbsp;</label>
                                                <br />
                                                <asp:LinkButton ID="LinkButton2" runat="server" CssClass="btn btn-small fa fa-eye" TabIndex="100" ToolTip="Visualizar Senha" OnClick="LinkButton2_Click"></asp:LinkButton>
                                                <asp:LinkButton ID="LinkButton4" runat="server" CssClass="btn btn-small fa fa-eye-slash" TabIndex="100" ToolTip="Esconder Senha" OnClick="LinkButton4_Click"></asp:LinkButton>
                                            </div>
                                        </div>

                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <label>e-mail</label>
                                                <asp:TextBox ID="txtsEmail" class="form-control " Width="400px" runat="server" MaxLength="200"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <label>Data de Validade</label>
                                                <asp:TextBox ID="txtdtValidade" class="form-control CaixaTextoData" runat="server" MaxLength="10" data-mask="00/00/0000" OnTextChanged="txtdtValidade_TextChanged"></asp:TextBox>
                                            </div>
                                        </div>

                                    </div>

                                    <asp:UpdatePanel ID="updPanel_TipoUsuario" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row" runat="server" id="DIV_TipoParceiro">
                                                <div class="col-lg-6">
                                                    <div class="form-group">
                                                        <label>Tipo Usuário</label>
                                                        <asp:DropDownList ID="ddlTpUsuario" runat="server" AutoPostBack="true" class="form-control yes_no select" Width="400px" OnSelectedIndexChanged="ddlTpUsuario_SelectedIndexChanged">
                                                            <asp:ListItem Value="">Selecione o Tipo</asp:ListItem>
                                                            <asp:ListItem Selected="True" Value="I">Usuário Interno</asp:ListItem>
                                                            <asp:ListItem Value="A">Administrador</asp:ListItem>
                                                            <asp:ListItem Value="C">Usuário Externo (HUB)</asp:ListItem>
                                                            <asp:ListItem Value="P" Enabled="false">Usuário Externo (Parceiros) </asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="col-lg-6" runat="server" id="DIV_Cliente">
                                                    <div class="form-group">
                                                        <label>Cliente HUB</label>
                                                        <asp:DropDownList ID="ddlCliente" runat="server" AutoPostBack="false" class="form-control yes_no select" Width="400px" OnSelectedIndexChanged="ddlCliente_SelectedIndexChanged"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="ddlTpUsuario" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>

                                    <div class="row" runat="server" id="DIV_Vendedor">
                                        <div class="col-lg-6">
                                            <uc1:ComboAtivo runat="server" ID="ddlsVendedor" />
                                        </div>
                                    </div>

                                    <div class="row" runat="server" id="DIV_Comprador">
                                        <div class="col-lg-6">
                                            <uc1:ComboAtivo runat="server" ID="ddlsComprador" />
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-lg-6">
                                            <uc1:ComboAtivo runat="server" ID="ddlAtivo" />
                                        </div>
                                    </div>
                                </div>

                            </div>
                            <div class="panel panel-default" runat="server" id="divQR" visible="true">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>QR Code Usuário</b></h3>
                                </div>
                                <div class="panel-body">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaQr" />
                                    <div class="col-lg-12">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <asp:Image ID="imgQRCode" runat="server" Visible="false" CssClass="img-thumbnail" />
                                                <asp:HyperLink ID="cmdDownload" runat="server"
                                                    Text="Download QR Code"
                                                    Target="_blank"
                                                    Visible="false"
                                                    CssClass="btn btn-primary mt-2" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <asp:Button runat="server" ID="cmdGerarQRCode" OnClick="cmdGerarQRCode_Click" CssClass="btn btn-sm btn-success" Text="Gerar QR Code" Visible="false" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

                <div role="tabpanel" class="tab-pane fade in " id="Departamentos" aria-labelledby="aba_Departamentos">
                    <br />
                    <div class="form-stacked row">
                        <div class="col-lg-9">
                            <asp:UpdatePanel ID="updPanel_Departamentos" runat="server">
                                <ContentTemplate>
                                    <div class="panel panel-default" runat="server" id="Div_Acoes">
                                        <div class="panel-heading">
                                            <h3 class="panel-title"><b>Departamentos do Usuário</b></h3>
                                        </div>
                                        <div class="panel-body">
                                            <uc1:MensagemPagina runat="server" ID="MensagemDepartamentos" />
                                            <div class="row">
                                                <div class="col-lg-12">
                                                    <div class="form-group">
                                                        <div class="row" runat="server" id="Div_Selecao">
                                                            <div class="col-lg-6">
                                                                <div class="form-group">
                                                                    <label>Departamento </label>
                                                                    <asp:DropDownList ID="ddlDepartamentos" runat="server" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-2">
                                                                <div class="form-group">
                                                                    <label>Gestor</label>
                                                                    <asp:DropDownList ID="ddlsGestorDepartamento" runat="server" class="form-control" Width="100px">
                                                                        <asp:ListItem Value="S">Sim</asp:ListItem>
                                                                        <asp:ListItem Selected="True" Value="N">Não</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-2">
                                                                <div class="form-group">
                                                                    <label>Notific e-mail</label>
                                                                    <asp:DropDownList ID="ddlsNotificacaoEmail" runat="server" class="form-control" Width="100px">
                                                                        <asp:ListItem Selected="True" Value="S">Sim</asp:ListItem>
                                                                        <asp:ListItem Value="N">Não</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-2">
                                                                <br />
                                                                <asp:LinkButton ID="cmdIncluirSelecao" runat="server" CssClass="btn btn-info" OnClick="cmdIncluirSelecao_Click"><i class="icon-bar"></i>&nbsp;Adicionar</asp:LinkButton>
                                                            </div>
                                                        </div>

                                                        <div class="row">
                                                            <div class="col-lg-12">
                                                                <asp:GridView ID="dtgDepartamentos" class="table table-striped table-bordered table-hover table-condensed"
                                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgSelecao_RowDataBound" OnRowDeleting="dtgSelecao_RowDeleting">
                                                                    <Columns>
                                                                        <asp:BoundField DataField="idUsuario" HeaderText="idDepartamento">
                                                                            <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:BoundField>
                                                                        <asp:BoundField DataField="sDscDepartamento" HeaderText="Usuário">
                                                                            <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:BoundField>
                                                                        <asp:BoundField DataField="sGestorDepartamento" HeaderText="Gestor">
                                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:BoundField>
                                                                        <asp:BoundField DataField="sNotificacaoEmail" HeaderText="email">
                                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:BoundField>
                                                                        <asp:TemplateField HeaderText="">
                                                                            <ItemTemplate>
                                                                                <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="icon-remove-sign"></i>&nbsp;Excluir</asp:LinkButton>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="18%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                    <PagerStyle BackColor="Black" ForeColor="Black" HorizontalAlign="Center" />
                                                                    <EditRowStyle BackColor="#2461BF" />
                                                                </asp:GridView>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cmdIncluirSelecao" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                    </div>


                </div>

                <div role="tabpanel" class="tab-pane fade in " id="permissoes" aria-labelledby="aba_Permissoes">

                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <br />
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>Permissões</b></h3>
                                </div>
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-lg-3 form-group">
                                            <label>Perfil</label>
                                            <asp:DropDownList ID="ddlidPerfil" runat="server" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlidPerfil_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                        <div class="col-lg-3 form-group">
                                            <label>Página Inicial</label>
                                            <asp:DropDownList ID="ddlPaginaInicial" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                        </div>
                                        <div class="col-lg-12">
                                            <uc1:Recursos runat="server" ID="ctrl_Recursos" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

                <div role="tabpanel" class="tab-pane fade in" id="Empresa" aria-labelledby="aba_empresa">
                    <br />
                    <div class="form-stacked row">
                        <div class="col-lg-9">
                            <div class="panel panel-default" runat="server" id="Div1">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>Empresa</b></h3>
                                </div>
                                <div class="panel-body">
                                    <div class="form-group row">
                                        <div class="col-lg-12">
                                            <uc1:MensagemPagina runat="server" ID="MensagemPagina1" />
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <div class="form-group">
                                                    <div class="row" runat="server" id="Div2">
                                                        <div class="col-lg-12">
                                                            <div class="col-lg-6">
                                                                <div class="form-group">
                                                                    <label>Empresas </label>
                                                                    <asp:DropDownList ID="ddlTodasEmpresa" runat="server" class="Caixa_Selecao form-control"></asp:DropDownList>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-1 form-group">
                                                                <label>&nbsp;</label>
                                                                <asp:Button ID="cmdEmpresa" runat="server" CssClass="btn btn-info form-control" OnClick="cmdEmpresa_Click" Text="Adicionar" />
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="table-responsive col-lg-12" runat="server" id="DIV_Empresa">
                                                        <asp:GridView ID="gv_Empresa" class="table table-striped table-bordered table-hover table-condensed"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" DataKeyNames="idLinha" OnRowDataBound="gv_Empresa_RowDataBound" OnRowDeleting="gv_Empresa_RowDeleting">
                                                            <Columns>
                                                                <asp:BoundField DataField="idlinha" HeaderText="idLinha">
                                                                    <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="idEmpresa" HeaderText="ID">
                                                                    <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="sDscEmpresa" HeaderText="Empresa">
                                                                    <ItemStyle Width="70%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkEnvio_Excluir" runat="server" CssClass="btn btn-small" TabIndex="100" ToolTip="Apagar Registro" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
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

                <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

                <fieldset class="form-stacked actions">
                    <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" />
                    <asp:Button ID="cmdExcluir" class="btn  btn-lg btn-danger" runat="server" Text="Excluir" />
                    <asp:Button ID="cmdForcarLogoff" class="btn  btn-lg btn-info" runat="server" Text="Forçar Logoff" />
                    <asp:Button ID="cmdVoltar" class="btn  btn-lg btn-warning" runat="server" Text="Voltar" OnClick="cmdVoltar_Click" />
                    <%--            <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" " id="field-cancel" title="Voltar" onclick="history.go(-1)" >&nbsp;--%>
                </fieldset>

            </div>

            <div id="dialog-Salvar" title="Salvar">
                <p>
                    <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                    <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
                </p>
            </div>

            <div id="dialog-Excluir" title="">
                <p>
                    <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                    <asp:Label ID="lblTituloExcluir" runat="server" Text=""></asp:Label>
                </p>
            </div>

            <div id="dialog-Logoff" title="">
                <p>
                    <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                    <asp:Label ID="lblTituloLogoff" runat="server" Text=""></asp:Label>
                </p>
            </div>

        </div>
    </div>

    <div id="hdd">
        <asp:HiddenField ID="hddIdUsuario" runat="server" />
        <asp:HiddenField ID="hddidParceiro" runat="server" />
        <asp:HiddenField ID="hddsFuncaoParceiro" runat="server" Value="" />
        <asp:HiddenField ID="hddidCliente" runat="server" />
        <asp:HiddenField ID="hddsSenha" runat="server" />
        <asp:HiddenField ID="hddsSenhaConfirmacao" runat="server" />
        <asp:HiddenField ID="hddsRecursos" runat="server" />
    </div>

</asp:Content>
