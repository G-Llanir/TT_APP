<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="ControleDadosAcesso.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.ControleAcesso" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        #lock {
            display: none;
        }

        .lock-label {
            position: relative;
            width: 45px;
            height: 45px;
            display: flex;
            align-items: center;
            justify-content: center;
            background-color: transparent;
            border-radius: 15px;
            cursor: pointer;
            transition: all 0.3s;
        }

            .lock-label:hover::after {
                content: "Clique para habilitar a edição";
                position: absolute;
                bottom: 100%;
                left: 10%;
                transform: translateX(-50%);
                background-color: #333;
                color: #fff;
                padding: 5px 8px;
                border-radius: 4px;
                white-space: nowrap;
                font-size: 12px;
                pointer-events: none;
                opacity: 0;
                transition: opacity 0.2s ease-in-out;
                z-index: 1;
            }

            .lock-label:hover::after {
                opacity: 1;
            }

        #lock:checked + .lock-label:hover::after {
            content: "Clique para bloquear a edição";
        }

        .lock-wrapper {
            width: fit-content;
            height: fit-content;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
        }

        .shackle {
            background-color: transparent;
            height: 9px;
            width: 14px;
            border-top-right-radius: 10px;
            border-top-left-radius: 10px;
            border-top: 3px solid black;
            border-left: 3px solid black;
            border-right: 3px solid black;
            transform: rotateY(150deg) translateX(3px);
            transform-origin: right;
            transition: all 0.3s;
        }

        .lock-body {
            width: 15px;
        }

        .shackle-closed {
            transform: rotateY(0deg);
        }

        .shackle-open {
            transform: rotateY(150deg) translateX(3px);
            transform-origin: right;
        }

        .lock-label-closed {
            pointer-events: none;
        }

        .lock-label:active {
            transform: scale(0.9);
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Controle Dados de Acesso"></asp:Label>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">
            <asp:Panel ID="pnControleAcessoConsulta" class="" runat="server">
                <div class="panel panel-primary" id="div_filtroPesquisa" runat="server">
                    <div class="panel-heading">
                        <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;Filtro para pesquisa </h3>

                    </div>
                    <div class="panel-body ">
                        <div class="form-group">

                            <div class="col-lg-2">
                                <div class="form-group">
                                    <label>Tipo</label>
                                    <asp:DropDownList ID="ddlsTipoPesquisa" class="form-control Caixa_Selecao" runat="server">
                                        <asp:ListItem Text="Selecione um Tipo" Value=""></asp:ListItem>
                                        <asp:ListItem Text="Site" Value="Site"></asp:ListItem>
                                        <asp:ListItem Text="Sistema" Value="Sistema"></asp:ListItem>
                                        <asp:ListItem Text="E-mail" Value="E-mail"></asp:ListItem>
                                        <asp:ListItem Text="Outros" Value="Outros"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-3">
                                <div class="form-group">
                                    <label>Departamento</label>
                                    <asp:DropDownList ID="ddlidDepartamentoPesquisa" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-3">
                                <div class="form-group">
                                    <label>Cliente/Fornecedor</label>
                                    <asp:DropDownList ID="ddlidParceiroPesquisa" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <label>&nbsp; </label>
                                    <asp:TextBox ID="txtPesquisa" class="form-control " placeholder="Pesquisar Endereço" runat="server"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-3">
                                <div class="form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" />
                                    <asp:Button ID="btnNovoControleAcesso" class="btn btn-success" runat="server" Text="Novo" OnClick="btnNovoControleAcesso_Click" />
                                    <asp:HyperLink ID="lnkExportar" runat="server" CssClass="btn btn-warning" Target="_blank" NavigateUrl="~/App/Paginas/Manutencao/ControleDadosAcesso.aspx?action=export"><i class="fa fa-file-excel-o"></i> Exportar para Excel</asp:HyperLink>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-lg-12">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />


                        <div class="panel panel-primary" id="div_gvConsulta" runat="server">
                            <div class="panel-body">
                                <div class="table-responsive">
                                    <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover " DataKeyNames="idDadoAcesso"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                        <Columns>

                                            <asp:BoundField DataField="idDadoAcesso" HeaderText="ID">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Tipo">
                                                <HeaderStyle Width="8%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbDadoAcessoDetalhe_Tipo" runat="server"
                                                        Text='<%# Eval("sTipo") %>'
                                                        CommandArgument='<%# Eval("idDadoAcesso") %>'
                                                        OnCommand="lbDadoAcessoDetalhe_Command">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Departamento">
                                                <HeaderStyle Width="10%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbDadoAcessoDetalhe_Departamento" runat="server"
                                                        Text='<%# Eval("sDscDepartamento") %>'
                                                        CommandArgument='<%# Eval("idDadoAcesso") %>'
                                                        OnCommand="lbDadoAcessoDetalhe_Command">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Cliente/Fornecedor">
                                                <HeaderStyle Width="15%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbDadoAcessoDetalhe_Parceiro" runat="server"
                                                        Text='<%# Eval("sRazaoSocial") %>'
                                                        CommandArgument='<%# Eval("idDadoAcesso") %>'
                                                        OnCommand="lbDadoAcessoDetalhe_Command">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Endereço">
                                                <HeaderStyle Width="50%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbDadoAcessoDetalhe_Endereco" runat="server"
                                                        Text='<%# Eval("sEndereco") %>'
                                                        CommandArgument='<%# Eval("idDadoAcesso") %>'
                                                        OnCommand="lbDadoAcessoDetalhe_Command">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Usuário Atualização">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Atualização">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>


            <asp:Panel ID="pnControleAcessoDetalhe" runat="server">

                <div>
                    <ul id="tab_dadoAcesso" class="nav nav-tabs" role="tablist">
                        <li role="presentation" class="tabpanel active">
                            <a href="#dadoAcesso" id="aba-dadoAcesso" role="tab" data-toggle="tab" aria-controls="dadoAcesso" aria-expanded="false"><b>Dados Acesso</b></a>
                        </li>

                        <li role="presentation" runat="server" id="aba_Historico">
                            <a href="#historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="Historico"><b>Histórico</b></a>
                        </li>

                    </ul>
                </div>

                <div id="tab" class="tab-content">

                    <div role="tabpanel" class="tab-pane fade in active" id="dadoAcesso" aria-labelledby="dadoAcesso">
                        <br />
                        <asp:UpdatePanel ID="updDetalhe" runat="server">
                            <ContentTemplate>

                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        <h3 class="panel-title"><b>Dados de Acesso</b></h3>
                                    </div>
                                    <div class="panel-body">

                                        <div class="col-lg-12">
                                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaDetalhe" />
                                        </div>


                                        <div class="form-group">
                                            <div class="col-lg-12">
                                                <div class="row">
                                                    <div class="col-lg-1">
                                                        <div class="form-group">
                                                            <label>ID</label>
                                                            <asp:TextBox ID="txtIdControleAcesso" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div runat="server" id="div_bloquearEdicao" class="col-lg-11" style="display: flex; justify-content: right;">
                                                        <div class="form-group">
                                                            <input type="checkbox" id="lock" <%= hddsCadeado.Value == "N" ? "checked='checked'" : "" %> />
                                                            <label for="lock" class="<%= hddsPermissaoCadeado.Value == "0" ? "lock-label lock-label-closed" : "lock-label" %>">
                                                                <span class="lock-wrapper">
                                                                    <span class="<%= hddsCadeado.Value == "N" ? "shackle shackle-open" : "shackle shackle-closed" %>"></span>
                                                                    <svg
                                                                        class="lock-body"
                                                                        width="15"
                                                                        height="15"
                                                                        viewBox="0 0 28 28"
                                                                        fill="none"
                                                                        xmlns="http://www.w3.org/2000/svg">
                                                                        <path
                                                                            fill-rule="evenodd"
                                                                            clip-rule="evenodd"
                                                                            d="M0 5C0 2.23858 2.23858 0 5 0H23C25.7614 0 28 2.23858 28 5V23C28 25.7614 25.7614 28 23 28H5C2.23858 28 0 25.7614 0 23V5ZM16 13.2361C16.6137 12.6868 17 11.8885 17 11C17 9.34315 15.6569 8 14 8C12.3431 8 11 9.34315 11 11C11 11.8885 11.3863 12.6868 12 13.2361V18C12 19.1046 12.8954 20 14 20C15.1046 20 16 19.1046 16 18V13.2361Z"
                                                                            fill="black">
                                                                        </path>
                                                                    </svg>
                                                                </span>
                                                            </label>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <div class="col-lg-12">
                                                <div class="row">

                                                    <div class="col-lg-2">
                                                        <div class="form-group">
                                                            <label>Tipo</label>
                                                            <asp:DropDownList ID="ddlsTipo" class="form-control Caixa_Selecao" runat="server">
                                                                <asp:ListItem Text="Selecione um Tipo" Value=""></asp:ListItem>
                                                                <asp:ListItem Text="Site" Value="Site"></asp:ListItem>
                                                                <asp:ListItem Text="Sistema" Value="Sistema"></asp:ListItem>
                                                                <asp:ListItem Text="E-mail" Value="E-mail"></asp:ListItem>
                                                                <asp:ListItem Text="Outros" Value="Outros"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-3">
                                                        <div class="form-group">
                                                            <label>Departamento</label>
                                                            <asp:DropDownList ID="ddlidDepartamento" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-3">
                                                        <div class="form-group">
                                                            <label>Cliente/Fornecedor</label>
                                                            <asp:DropDownList ID="ddlidParceiro" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <div class="col-lg-12">
                                                <div class="row">

                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label>Endereço</label>
                                                            <asp:TextBox ID="txtsEndereco" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-12 form-group">
                                            <div class="row">

                                                <div class="col-lg-3">
                                                    <div class="form-group">
                                                        <label>Usuário</label>
                                                        <asp:TextBox ID="txtsUsuario" class="form-control" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2">
                                                    <div class="form-group">
                                                        <label>Senha</label>
                                                        <asp:TextBox ID="txtsSenha" class="form-control" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2">
                                                    <div class="form-group">
                                                        <label>Token</label>
                                                        <asp:TextBox ID="txtsToken" class="form-control" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2">
                                                    <div class="form-group">
                                                        <label>Celular Recuperação</label>
                                                        <asp:TextBox ID="txtsCelularRecuperacao" class="form-control" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-3">
                                                    <div class="form-group">
                                                        <label>E-mail Recuperação</label>
                                                        <asp:TextBox ID="txtsEmailRecuperacao" class="form-control" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-12">
                                                    <div class="form-group">
                                                        <label>Outros Dados</label>
                                                        <asp:TextBox ID="txtsOutrosDados" class="form-control" runat="server" TextMode="MultiLine"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-12">
                                                    <div class="form-group">
                                                        <label>Observações</label>
                                                        <asp:TextBox ID="txtsObservacao" class="form-control" runat="server" TextMode="MultiLine" Height="200px"></asp:TextBox>
                                                    </div>
                                                </div>

                                            </div>


                                            <div class="panel panel-primary" runat="server" id="div_UsuarioLiberado">
                                                <div class="panel-heading" style="display: flex;">
                                                    <h3 class="panel-title">Liberado Para</h3>
                                                </div>

                                                <div class="panel-body">

                                                    <div class="form-group" id="div_incluirUsuario" runat="server">
                                                        <div class="col-lg-12">
                                                            <div class="row">

                                                                <div class="col-lg-3">
                                                                    <div class="form-group">
                                                                        <label>Usuário</label>
                                                                        <asp:DropDownList ID="ddlidUsuario" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                                                    </div>
                                                                </div>

                                                                <div class="col-lg-1">
                                                                    <div class="form-group">
                                                                        <asp:Button ID="btnIncluirUsuario" runat="server" class="btn btn-primary" Text="Incluir" Style="margin-top: 24px;" OnClick="btnIncluirUsuario_Click" />
                                                                    </div>
                                                                </div>

                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-12 form-group table-responsive" style="margin-top: 20px;">
                                                        <asp:GridView ID="gvUsuarioLiberado" class="table table-striped table-bordered table-hover " DataKeyNames="idUsuario"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDeleting="gvUsuarioLiberado_RowDeleting" OnRowDataBound="gvUsuarioLiberado_RowDataBound">

                                                            <Columns>

                                                                <asp:BoundField DataField="idUsuario" HeaderText="ID">
                                                                    <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário">
                                                                    <ItemStyle Width="80%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="Permissão">
                                                                    <ItemTemplate>
                                                                        <asp:DropDownList ID="ddlPermissao" runat="server" CssClass="form-control">
                                                                            <asp:ListItem Text="Visualização" Value="V"></asp:ListItem>
                                                                            <asp:ListItem Text="Edição" Value="E"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>

                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                    <div role="tabpanel" class="tab-pane fade" id="historico" aria-labelledby="Historico">
                        <br />

                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
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
                                                    ShowFooter="False" OnRowDataBound="gv_Historico_RowDataBound">
                                                    <Columns>
                                                        <asp:BoundField DataField="dtAcao" HeaderText="Data da Ação">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sDscUsuarioAcao" HeaderText="Usuário">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sTipoAcao" HeaderText="Tipo">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="sAcao" HeaderText="Ação">
                                                            <ItemStyle Width="70%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                    </Columns>
                                                </asp:GridView>

                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                    <fieldset class="form-stacked actions">
                        <div class="col-lg-12">
                            <div class="form-group">
                                <asp:Button ID="btnSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="btnSalvar_Click" />
                                <asp:Button ID="btnVoltar" CssClass="btn btn-lg btn-warning" runat="server" Text="Voltar" OnClick="btnVoltar_Click" />
                            </div>
                        </div>
                    </fieldset>

                </div>

            </asp:Panel>
        </div>

        <div id="dialog_Aceitar" class="modal" title="Status Edição">
            <p>
                <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                <asp:Label ID="Label3" runat="server" ClientIDMode="Static" Text=""></asp:Label>
            </p>
        </div>

        <asp:HiddenField ID="hddidDadoAcesso" Value="0" runat="server" />
        <asp:HiddenField ID="hddsCadeado" runat="server" Value="N" />
        <asp:HiddenField ID="hddsPermissaoCadeado" runat="server" Value="0" />
        <asp:HiddenField ID="hddsPermissaoAcesso" runat="server" Value="E" />

    </div> 

</asp:Content>
