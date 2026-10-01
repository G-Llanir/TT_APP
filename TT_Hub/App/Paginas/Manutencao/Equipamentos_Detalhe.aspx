<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" EnableEventValidation="true" AutoEventWireup="true" CodeBehind="Equipamentos_Detalhe.aspx.cs" Inherits="TT_Hub.App.Paginas.Manutencao.Equipamento_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style>
        .caixaBotao {
            height: 150px;
            background-color: inherit;
            border-radius: 4px;
            overflow: hidden;
            display: flex;
            flex-direction: column;
            justify-content: center;
            text-align: center;
            width: 100%;
        }

            .caixaBotao .panel-heading {
                border: none;
                padding: 0;
                height: 100%;
                display: flex;
                align-items: center;
                justify-content: center;
            }

        .announcement-heading {
            font-size: clamp(16px, 2.5vw, 30px);
            word-wrap: break-word;
            padding: 10px;
            width: 385px;
            height: 150px;
            text-align:center;
            display:contents;
        }

        .caixaBotao:hover {
            box-shadow: 3px 3px 7px #337ab7, -3px -3px 7px #337ab7;
            -webkit-transition: box-shadow ease-out 0.1s;
            transition: box-shadow ease-out 0.1s;
        }
    </style>

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>
    <script src="https://login.tecandtec.com.br/app/js/jquery-1.9.2.min.js" type="text/javascript"></script>
    <script> $v192 = jQuery.noConflict();</script>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <br />
            <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Consulta de Cadastros" />
            <div class="well">
                <h4>
                    <span id="MainContent_lblTituloPagina">
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Cadastro"></asp:Label>
                    </span>
                </h4>
            </div>

            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
            <asp:Panel ID="pnlCadastro" runat="server">
                <div class="bs-example bs-example-tabs" data-example-id="togglable-tabs">
                    <ul id="myTabs" class="nav nav-tabs" role="tablist">
                        <li role="presentation" class="active">
                            <a href="#dados" id="dados-tab" role="tab" data-toggle="tab" aria-controls="home" aria-expanded="true"><b>Dados</b></a>
                        </li>

                        <li role="presentation" runat="server" id="abaInformacoes">
                            <a href="#Informacoes" role="tab" id="informacoes-tab" data-toggle="tab" aria-controls="historico"><b>Informações</b></a>
                        </li>

                        <li role="presentation" runat="server" id="abaSigma">
                            <a href="#Sigma" role="tab" id="sigma-tab" data-toggle="tab" aria-controls="Links"><b>Configurações Sigma</b></a>
                        </li>

                        <li role="presentation" runat="server" id="abaHistorico">
                            <a href="#Historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="Links"><b>Histórico</b></a>
                        </li>
                    </ul>
                </div>

                <div id="myTabContent" class="tab-content">

                    <div role="tabpanel" class="tab-pane fade in active" id="dados" aria-labelledby="dados-tab">
                        <asp:UpdatePanel ID="updEquipamentos" runat="server" ChildrenAsTriggers="False" UpdateMode="Conditional">
                            <ContentTemplate>

                                <br />
                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        <h3 class="panel-title"><b>Dados do Equipamento</b></h3>
                                    </div>
                                    <div class="panel-body">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <div class="form-group">
                                                    <label>Descrição</label>
                                                    <asp:TextBox ID="txtsDescricao" class="form-control " runat="server" MaxLength="200"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>Cliente</label>
                                                    <asp:DropDownList ID="ddlCliente" runat="server" AutoPostBack="true" class="form-control yes_no select" OnSelectedIndexChanged="ddlCliente_SelectedIndexChanged"></asp:DropDownList>
                                                    <asp:TextBox ID="txtsDscCliente" class="form-control " runat="server" Disabled="disabled"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>Unidade</label>
                                                    <asp:DropDownList ID="ddlidUnidade" runat="server" AutoPostBack="false" class="form-control yes_no select"></asp:DropDownList>
                                                    <asp:TextBox ID="txtsDscUnidade" class="form-control " runat="server" Disabled="disabled"></asp:TextBox>

                                                </div>
                                            </div>

                                            <div class="col-lg-6">
                                                <div class="row">
                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label>Tipo de Monitoramento</label>
                                                            <asp:DropDownList ID="ddlidTipoMonitoramento" runat="server" AutoPostBack="false" class="form-control yes_no select"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label>Modelo do Equipamento</label>
                                                            <asp:DropDownList ID="ddlTipoEquipamento" runat="server" AutoPostBack="false" class="form-control yes_no select"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>ID ou IMEI</label>
                                                    <asp:TextBox ID="txtsID" class="form-control  CaixaTextoGrande" runat="server" MaxLength="200"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>HostName</label>
                                                    <asp:TextBox ID="txtsHostName" class="form-control " runat="server" MaxLength="200"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-6">
                                                <div class="row">
                                                    <div class="col-lg-4">
                                                        <div class="form-group">
                                                            <label>Cód Cliente Sigma</label>
                                                            <asp:TextBox ID="txtsw_Account" class="form-control uppercase" runat="server" MaxLength="10" Style="width: 150px;"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-8">
                                                        <div class="form-group">
                                                            <label>Token Sigma</label>
                                                            <asp:DropDownList ID="ddlidToken" runat="server" AutoPostBack="false" class="form-control yes_no select"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>Operadora (Chip)</label>
                                                    <asp:DropDownList ID="ddlOperadora" runat="server" AutoPostBack="false" class="form-control yes_no select"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-6">
                                                <div class="form-group">
                                                    <label>IccID  (Código do Chip)</label>
                                                    <asp:TextBox ID="txtsIMEI" class="form-control uppercase  CaixaTextoGrande" runat="server" MaxLength="200"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-lg-6">
                                                <div class="row">
                                                    <div class="col-lg-8">
                                                        <div class="form-group">
                                                            <label>Endereço IP Equipamento</label>
                                                            <asp:TextBox ID="txtsEnderecoIP" class="form-control" runat="server" MaxLength="20"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-4">
                                                        <div class="form-group">
                                                            <label>Porta acesso</label>
                                                            <asp:TextBox ID="txtsPorta" class="form-control uppercase  CaixaTextoPequeno" runat="server" MaxLength="4"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-lg-6">
                                                <div class="row">
                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label>Login/Usuário</label>
                                                            <asp:TextBox ID="txtsUsuario" class="form-control uppercase  CaixaTextoMedio" runat="server" MaxLength="40"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label>Senha</label>
                                                            <asp:TextBox ID="txtsSenha" class="form-control uppercase  CaixaTextoMedio" runat="server" MaxLength="40"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-lg-6">
                                                <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
                                            </div>
                                        </div>

                                    </div>

                                </div>

                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        <h3 class="panel-title"><b>Contato</b></h3>
                                    </div>
                                    <div class="panel-body">
                                        <div class="col-lg-6">
                                            <div class="row">
                                                <div class="col-lg-6">
                                                    <div class="form-group">
                                                        <label>Técnico (Nome)</label>
                                                        <asp:TextBox ID="txtsContatoTecnico" class="form-control CaixaTextoMedio" runat="server" MaxLength="50"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-lg-6">
                                                    <div class="form-group">
                                                        <label>Telefone</label>
                                                        <asp:TextBox ID="txtsTelefoneTecnico" class="form-control CaixaTextoMedio" runat="server" MaxLength="50"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-6">
                                            <div class="row">
                                                <div class="col-lg-6">
                                                    <div class="form-group">
                                                        <label>Outros</label>
                                                        <asp:TextBox ID="txtsContatoOutros" class="form-control CaixaTextoMedio" runat="server" MaxLength="50"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-lg-6">
                                                    <div class="form-group">
                                                        <label>Telefone</label>
                                                        <asp:TextBox ID="txtsTelefoneOutros" class="form-control CaixaTextoMedio" runat="server" MaxLength="50"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        <h3 class="panel-title"><b>Configuração</b></h3>
                                    </div>
                                    <div class="panel-body">
                                        <div class="col-lg-6">
                                            <uc1:ComboAtivo runat="server" ID="ddlsMonitorar_Tensao_Rede" />
                                        </div>
                                        <div class="col-lg-6">
                                            <uc1:ComboAtivo runat="server" ID="ddlsMonitorar_Tensao_Bateria" />
                                        </div>
                                    </div>
                                </div>

                                <asp:UpdatePanel ID="updPanel_Endereco" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="False">
                                    <ContentTemplate>
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>Endereço de Instalação</b></h3>
                                            </div>
                                            <div class="panel-body">
                                                <div class="row">
                                                    <div class="col-lg-6">
                                                        <div class="row">
                                                            <div class="col-lg-4">
                                                                <div class="form-group">
                                                                    <label>CEP</label>
                                                                    <asp:TextBox ID="txtsCEP" class="form-control" runat="server" MaxLength="9" Style="width: 150px;" OnTextChanged="txtsCEP_TextChanged" AutoPostBack="True"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-4">
                                                                <div class="form-group">
                                                                    <asp:UpdateProgress ID="UpdateProgress1" runat="server" OnDataBinding="txtsCEP_TextChanged">
                                                                        <ProgressTemplate>
                                                                            Aguarde
                                                                        </ProgressTemplate>
                                                                    </asp:UpdateProgress>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>


                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label>Endereco</label>
                                                            <asp:TextBox ID="txtsLogradouro" class="form-control uppercase" MaxLength="200" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label>Número</label>
                                                            <asp:TextBox ID="txtsNumero" class="form-control" runat="server" MaxLength="10" Style="width: 150px;"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label>Complemento&nbsp;&nbsp; Ex: AP XX, BL ZZ, CS 00, CJ 00, SL 00</label>
                                                            &nbsp;<asp:TextBox ID="txtsComplemento" class="form-control uppercase" MaxLength="100" runat="server" Style="width: 300px;"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label>Bairro</label>
                                                            <asp:TextBox ID="txtsBairro" class="form-control uppercase" runat="server" MaxLength="100" Style="width: 300px;"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label>Cidade</label>
                                                            <asp:TextBox ID="txtsCidade" class="form-control uppercase" runat="server" MaxLength="200" Style="width: 300px;"></asp:TextBox>

                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <div class="form-group">
                                                            <label>Estado</label>
                                                            <asp:DropDownList ID="ddlsUF" runat="server" class="form-control uppercase" Width="150px"></asp:DropDownList>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="txtsCEP" EventName="TextChanged" />

                                    </Triggers>
                                </asp:UpdatePanel>

                                <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

                                <fieldset class="form-stacked actions">
                                    <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                                    &nbsp;
                                <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" " id="field-cancel" title="Voltar" onclick="history.go(-1)" >&nbsp;
                                </fieldset>

                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlCliente" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>

                    </div>

                    <div role="tabpanel" class="tab-pane fade" id="Informacoes" aria-labelledby="informacoes-tab">
                        <asp:UpdatePanel ID="upInformacoes" runat="server">
                            <ContentTemplate>
                                <br />

                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        <h3 class="panel-title"><b>Informações do Equipamento</b></h3>
                                    </div>
                                    <div class="panel-body">
                                        <div class="form-group">
                                            <uc1:MensagemPagina runat="server" ID="msgAcoes" />
                                        </div>

                                        <asp:GridView
                                            ID="gvInformacoes" class="table table-striped table-bordered table-hover table-condensed"
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvInformacoes_RowDataBound">
                                            <FooterStyle CssClass="TAB_Fundo_Azul" />
                                            <RowStyle CssClass="texto_padrao_preto" />
                                            <Columns>
                                                <asp:BoundField DataField="sParametro" HeaderText="TAG">
                                                    <ItemStyle Width="5%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDscEventoCompleto" HeaderText="Descrição">
                                                    <ItemStyle Width="50%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sValor" HeaderText="Valor">
                                                    <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="dtUltimaAtualizacao" HeaderText="Atualizado em">
                                                    <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                            </Columns>
                                            <FooterStyle CssClass="TAB_Fundo_Azul" />
                                            <RowStyle CssClass="texto_padrao_preto" />
                                            <PagerStyle BackColor="Black" ForeColor="Black" HorizontalAlign="Center" />
                                            <HeaderStyle CssClass="TAB_Fundo_Azul" />
                                            <EditRowStyle BackColor="#2461BF" />

                                        </asp:GridView>

                                    </div>
                                </div>

                                <div class="panel panel-default" id="div_Acoes" runat="server">
                                    <div class="panel-heading">
                                        <h3 class="panel-title"><b>Executar Ações</b></h3>
                                    </div>
                                    <div class="panel-body">
                                        <div class="row">

                                            <div class="panel-body">
                                                <div class="row">
                                                    <asp:Repeater ID="rptBotoes" runat="server">
                                                        <ItemTemplate>
                                                            <div class="col-lg-<%#DataBinder.Eval(Container.DataItem, "nQtdColunas") %>  ">
                                                                <asp:LinkButton ID="cmdEquipamentos" runat="server" OnClick="cmdEquipamentos_Click"
                                                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CodigoAcao_SW") %>'>
                                                                    <div class="panel panel-<%# DataBinder.Eval(Container.DataItem, "sCor") %> caixaBotao">
                                                                        <div class="panel-heading">
                                                                            <div class="row">
                                                                                <div class="col-xs-12 text-center">
                                                                                    <p class="announcement-heading"><%# DataBinder.Eval(Container.DataItem, "sDscAcao") %></p>
                                                                                    <%--<p class="announcement-text"><%# DataBinder.Eval(Container.DataItem, "sDscAcao") %></p>--%>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </asp:LinkButton>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>

                                <asp:Timer ID="timer_AtualizarInformacoes" runat="server" Interval="5000"
                                    OnTick="timer_AtualizarInformacoes_Tick">
                                </asp:Timer>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="timer_AtualizarInformacoes" EventName="Tick" />
                            </Triggers>
                        </asp:UpdatePanel>

                    </div>

                    <div role="tabpanel" class="tab-pane fade" id="Historico" aria-labelledby="historico-tab">
                        <br />
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Histórico</b></h3>
                            </div>
                            <div class="panel-body">
                                <asp:GridView
                                    ID="dtgConsultaHistorico" class="table table-striped table-bordered table-hover table-condensed"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgConsultaHistorico_RowDataBound">
                                    <FooterStyle CssClass="TAB_Fundo_Azul" />
                                    <RowStyle CssClass="texto_padrao_preto" />
                                    <Columns>
                                        <asp:BoundField DataField="ID" HeaderText="ID Log">
                                            <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="DatadoEvento" HeaderText="Data">
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:HyperLink ID="hplDescricao" runat="server" NavigateUrl='<%# Bind("sLink") %>' Text='<%# Bind("Descricao") %>'></asp:HyperLink>
                                                <asp:Label ID="lblDescricao" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="sdscUsuario" HeaderText="Efetuada por">
                                            <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                    </Columns>
                                    <FooterStyle CssClass="TAB_Fundo_Azul" />
                                    <RowStyle CssClass="texto_padrao_preto" />
                                    <PagerStyle BackColor="Black" ForeColor="Black" HorizontalAlign="Center" />
                                    <HeaderStyle CssClass="TAB_Fundo_Azul" />
                                    <EditRowStyle BackColor="#2461BF" />

                                </asp:GridView>
                            </div>
                        </div>
                    </div>

                    <div role="tabpanel" class="tab-pane fade" id="Sigma" aria-labelledby="Sigma-tab">
                        <br />
                        <div class="panel panel-default" id="pn_Link" runat="server">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Links para API - Segware</b></h3>
                            </div>
                            <div class="panel-body">
                                <asp:GridView
                                    ID="gvLinks" class="table table-striped table-bordered table-hover table-condensed"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                    <FooterStyle CssClass="TAB_Fundo_Azul" />
                                    <RowStyle CssClass="texto_padrao_preto" />
                                    <Columns>
                                        <asp:BoundField DataField="sDscAcao" HeaderText="Ação">
                                            <ItemStyle Width="30%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="sURL" HeaderText="URL">
                                            <ItemStyle Width="70%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                    </Columns>
                                    <FooterStyle CssClass="TAB_Fundo_Azul" />
                                    <RowStyle CssClass="texto_padrao_preto" />
                                    <PagerStyle BackColor="Black" ForeColor="Black" HorizontalAlign="Center" />
                                    <HeaderStyle CssClass="TAB_Fundo_Azul" />
                                    <EditRowStyle BackColor="#2461BF" />

                                </asp:GridView>
                            </div>
                        </div>
                        <br />
                        <div class="panel panel-default" id="Div1" runat="server">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Enviar Testes para o Sigma</b></h3>
                            </div>
                            <div class="panel-body">
                            </div>
                        </div>

                    </div>

                </div>

            </asp:Panel>
        </div>

    </div>

    <div id="dialog-Salvar" title="Salvar">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
        </p>
    </div>

    <asp:HiddenField ID="hddIdEquipamento" runat="server" />
    <asp:HiddenField ID="hddidRegistroAcao" runat="server" />
    <asp:HiddenField ID="hddidCodigoAcao_SW" runat="server" />

</asp:Content>
