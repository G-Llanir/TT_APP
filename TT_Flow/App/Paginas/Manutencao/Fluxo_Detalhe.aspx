<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Fluxo_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.Fluxo_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        span.form-control.check-custom {
            width: 78%;
        }
    </style>

</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <asp:UpdatePanel ID="updDetalhe" runat="server">
        <ContentTemplate>
            <div>
                <div class="form-stacked row">
                    <div class="col-lg-12">
                        <h1>
                            <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label></h1>
                        <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Detalhe" />
                    </div>
                </div>

                <div class="form-stacked row">

                    <div class="col-lg-12">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    </div>

                    <div class="col-lg-12">
                        <div class="form-group">
                            <label>ID </label>
                            <asp:TextBox ID="txtidFluxo" class="form-control CaixaTextoMini" runat="server" disabled=""></asp:TextBox>
                        </div>
                    </div>

                    <div class="col-lg-12">
                        <div class="form-group">
                            <div class="col-lg-4 row">
                                <div class="form-group ">
                                    <label>Descrição </label>
                                    <asp:TextBox ID="txtsDscFluxo" class="form-control" runat="server" MaxLength="200"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-12">
                        <div class="form-group">
                            <div class="col-lg-2 row">
                                <div class="form-group ">
                                    <label>Tipo </label>
                                    <asp:DropDownList ID="ddlidTipo" runat="server" class="form-control yes_no select" AutoPostBack="true" OnSelectedIndexChanged="ddlidTipo_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-12" runat="server" id="div_ddlGeraOPI">
                        <div class="form-group">
                            <div class="col-lg-4 row">
                                <div class="form-group ">
                                    <label>Gera OPI?</label>
                                    <asp:DropDownList ID="ddlGeraOPI" runat="server" class="form-control yes_no select CaixaTextoMini">
                                        <asp:ListItem class="danger" Value="">Selecione </asp:ListItem>
                                        <asp:ListItem Value="S">Sim</asp:ListItem>
                                        <asp:ListItem Value="N">Não</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-12">
                        <uc1:SwitchAtivo ID="SwitchGastos" runat="server" />
                    </div>

                    <div class="col-lg-6">
                        <uc1:SwitchAtivo runat="server" ID="ComboAtivo" />
                    </div>

                </div>
                <br />
                <div class="panel panel-default" runat="server" id="Div1">
                    <div class="panel-heading">
                        <h3 class="panel-title"><b>Sequência de Departamentos</b></h3>
                    </div>
                    <div class="panel-body table-responsive">
                        <asp:UpdatePanel ID="updDepartamentos" runat="server">
                            <ContentTemplate>
                                <uc1:MensagemPagina runat="server" ID="MensagemAcoes" />
                                <div class="row">
                                    <div class="col-lg-12">
                                        <div class="form-group">
                                            <div class="row" runat="server" id="Div_Selecao">
                                                <div class="col-lg-3">
                                                    <div class="form-group">
                                                        <label>Departamento </label>
                                                        <asp:DropDownList ID="ddlDepartamentos_Departamento" runat="server" class="form-control"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-lg-3">
                                                    <div class="form-group">
                                                        <label>&nbsp;</label>
                                                        <asp:CheckBox runat="server" ID="cbAdicionaStatus" class="form-control check-custom" Text="Adicionar Status Finalizado e Cancelado" Checked="false" AutoPostBack="true" OnCheckedChanged="cbAdicionaStatus_CheckedChanged" />
                                                    </div>
                                                </div>

                                                <div class="col-lg-3" style="margin-top: 4px">
                                                    <br />
                                                    <asp:LinkButton ID="cmdDepartamentos_Incluir" runat="server" CssClass="btn btn-info" OnClick="cmdDepartamentos_Incluir_Click"><i class="icon-bar"></i>&nbsp;Adicionar</asp:LinkButton>
                                                </div>
                                            </div>

                                            <div class="row">
                                                <div class="col-lg-12">
                                                    <asp:GridView ID="gvDepartamentos" class="table table-striped table-bordered table-hover table-condensed"
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvDepartamentos_RowDataBound" OnRowDeleting="gvDepartamentos_RowDeleting">
                                                        <Columns>
                                                            <asp:BoundField DataField="idRegistro" HeaderText="idRegistro">
                                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="idDepartamento" HeaderText="idDepartamento">
                                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="sDscDepartamento" HeaderText="Departamento">
                                                                <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>


                                                            <asp:TemplateField HeaderText="Ordem">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtDepartamentos_nOrdem" class="form-control CaixaTextoMicro" MaxLength="4" runat="server" Text='<%# Bind("nOrdem") %>' AutoPostBack="True" OnTextChanged="txtDepartamentos_nOrdem_TextChanged"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Status Interno">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlDepartamentos_StatusInterno" runat="server" class="form-control yes_no select CaixaTextoPequeno" AutoPostBack="true" OnSelectedIndexChanged="ddlDepartamentos_StatusInterno_SelectedIndexChanged"></asp:DropDownList>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Status Kanban">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlDepartamentos_StatusKanban" runat="server" class="form-control yes_no select CaixaTextoPequeno" AutoPostBack="true" OnSelectedIndexChanged="ddlDepartamentos_StatusKanban_SelectedIndexChanged"></asp:DropDownList>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Tempo">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtDepartamentos_nTempo" class="form-control CaixaTextoMicro" MaxLength="3" runat="server" Text='<%# Bind("nTempo") %>' AutoPostBack="True" OnTextChanged="txtDepartamentos_nTempo_TextChanged"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Tempo">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlDepartamentos_TipoTempo" runat="server" class="form-control yes_no select CaixaTextoMini" AutoPostBack="true" OnSelectedIndexChanged="ddlDepartamentos_TipoTempo_SelectedIndexChanged">
                                                                        <asp:ListItem class="danger" Value="" Selected="True">Selecione </asp:ListItem>
                                                                        <asp:ListItem Value="h">Hora</asp:ListItem>
                                                                        <asp:ListItem Value="d">Dia</asp:ListItem>
                                                                        <asp:ListItem Value="s">Semana</asp:ListItem>
                                                                        <asp:ListItem Value="m">Mês</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="lnkDepartamentos_Excluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
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

                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cmdDepartamentos_Incluir" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div id="Div3" runat="server" class="panel panel-default">
                    <div class="panel-heading">
                        <h3 class="panel-title"><b>Tarefas</b></h3>
                    </div>
                    <div class="panel-body table-responsive">
                        <asp:UpdatePanel ID="updTarefas" runat="server">
                            <ContentTemplate>
                                <uc1:MensagemPagina ID="MensagemTarefa" runat="server" />
                                <div class="row">
                                    <div class="col-lg-12">
                                        <div class="form-group">
                                            <div id="Div2" runat="server" class="row">
                                                <div class="col-lg-5">
                                                    <div class="form-group">
                                                        <label>
                                                            Departamento
                                                        </label>
                                                        <asp:DropDownList ID="ddlTarefas_Departamento" runat="server" AutoPostBack="True" class="form-control yes_no select " OnSelectedIndexChanged="ddlTarefas_Departamento_SelectedIndexChanged">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-lg-5">
                                                    <div class="form-group">
                                                        <label>
                                                            Tarefa</label>
                                                        <asp:DropDownList ID="ddlTarefas_Tarefa" runat="server" class="form-control yes_no select ">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-lg-2">
                                                    <br />
                                                    <asp:Button ID="cmdTarefas_Incluir" runat="server" CssClass="btn btn-info" OnClick="cmdTarefas_Incluir_Click" Text="Adicionar" />
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-12">
                                                    <asp:GridView ID="gvTarefas" runat="server" AutoGenerateColumns="False" CellPadding="1" CellSpacing="1" class="table table-striped table-bordered table-hover table-condensed" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" GridLines="None" OnRowDataBound="gvTarefas_RowDataBound" OnRowDeleting="gvTarefas_RowDeleting" ShowFooter="False" Width="100%">
                                                        <Columns>
                                                            <asp:BoundField DataField="idRegistro" HeaderText="idRegistro">
                                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="20%" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="idTarefa" HeaderText="idTarefa">
                                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="20%" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="sDscDepartamento" HeaderText="Departamento">
                                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="30%" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="sDscTarefa" HeaderText="Tarefa">
                                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="30%" />
                                                            </asp:BoundField>
                                                            <asp:TemplateField HeaderText="Ordem">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtTarefas_nOrdem" runat="server" class="form-control CaixaTextoMicro" MaxLength="4" Text='<%# Bind("nOrdem") %>'></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="8%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Degrau">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlTarefas_nDegrau" runat="server" class="form-control yes_no select CaixaTextoMini">
                                                                        <asp:ListItem Selected="True" Value="0">D0 - Inicial</asp:ListItem>
                                                                        <asp:ListItem Value="1">D1</asp:ListItem>
                                                                        <asp:ListItem Value="2">D2</asp:ListItem>
                                                                        <asp:ListItem Value="3">D3</asp:ListItem>
                                                                        <asp:ListItem Value="4">D4</asp:ListItem>
                                                                        <asp:ListItem Value="5">Finalizador</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Obrigatório">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlTarefas_sObrigatorioConclusao" runat="server" class="form-control yes_no select CaixaTextoMini" ToolTip="Obrigatório Conclusão da Tarefa anterior?">
                                                                        <asp:ListItem Selected="True" Value="S">Sim </asp:ListItem>
                                                                        <asp:ListItem Value="N">Não</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Tempo">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtTarefas_nTempo" runat="server" class="form-control CaixaTextoMicro" MaxLength="3" Text='<%# Bind("nTempo") %>'></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="8%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Tempo">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlTarefas_TipoTempo" runat="server" class="form-control yes_no select CaixaTextoMini">
                                                                        <asp:ListItem Selected="True" Value="">Selecione </asp:ListItem>
                                                                        <asp:ListItem Value="h">Hora</asp:ListItem>
                                                                        <asp:ListItem Value="d">Dia</asp:ListItem>
                                                                        <asp:ListItem Value="s">Semana</asp:ListItem>
                                                                        <asp:ListItem Value="m">Mês</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="lnkTarefas_Excluir" runat="server" CommandName="Delete" CssClass="btn btn-small" TabIndex="100"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="18%" />
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
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cmdTarefas_Incluir" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </div>

                <div id="Div4" runat="server" class="panel panel-default">
                    <div class="panel-heading">
                        <h3 class="panel-title"><b>Recursos</b></h3>
                    </div>
                    <div class="panel-body table-responsive">
                        <asp:UpdatePanel ID="updRecursos" runat="server">
                            <ContentTemplate>
                                <uc1:MensagemPagina ID="MensagemRecurso" runat="server" />
                                <div class="row">
                                    <div class="col-lg-12">
                                        <div class="form-group">
                                            <div id="Div5" runat="server" class="row">
                                                <div class="col-lg-5">
                                                    <div class="form-group">
                                                        <label>
                                                            Tipo
                                                        </label>
                                                        <asp:DropDownList ID="ddlRecursos_idTipoRecurso" runat="server" AutoPostBack="True" class="form-control yes_no select " OnSelectedIndexChanged="ddlRecursos_idTipoRecurso_SelectedIndexChanged">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-lg-5">
                                                    <div class="form-group">
                                                        <label>
                                                            Tarefa</label>
                                                        <asp:DropDownList ID="ddlRecursos_idRecurso" runat="server" class="form-control yes_no select ">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-lg-2">
                                                    <br />
                                                    <asp:Button ID="cmdRecursos_Incluir" runat="server" CssClass="btn btn-info" OnClick="cmdRecursos_Incluir_Click" Text="Adicionar" />
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-12">
                                                    <asp:GridView ID="gvRecursos" runat="server" AutoGenerateColumns="False" CellPadding="1" CellSpacing="1" class="table table-striped table-bordered table-hover table-condensed"
                                                        Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" GridLines="None"
                                                        OnRowDataBound="gvRecursos_RowDataBound" OnRowDeleting="gvRecursos_RowDeleting" ShowFooter="False" Width="100%">
                                                        <Columns>
                                                            <asp:BoundField DataField="idRegistro" HeaderText="idRegistro">
                                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="20%" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="idRecurso" HeaderText="idRecurso">
                                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="20%" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="sDscTipoRecurso" HeaderText="Tipo">
                                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="30%" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="sDscRecurso" HeaderText="Recurso">
                                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="30%" />
                                                            </asp:BoundField>
                                                            <asp:TemplateField HeaderText="Ordem">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtRecursos_nOrdem" class="form-control CaixaTextoMicro" MaxLength="4" runat="server" Text='<%# Bind("nOrdem") %>' AutoPostBack="True" OnTextChanged="txtDepartamentos_nOrdem_TextChanged"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Unidade">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlRecursos_sUnidade" runat="server" class="form-control yes_no select CaixaTextoMini">
                                                                    </asp:DropDownList>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Quantidade">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtRecursos_nQuantidade" runat="server" class="form-control CaixaTextoMini" MaxLength="6" Text='<%# Bind("nQuantidade") %>'></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="8%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="lnkRecursos_Excluir" runat="server" CommandName="Delete" CssClass="btn btn-small" TabIndex="100"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="18%" />
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
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cmdRecursos_Incluir" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </div>

                <uc1:PainelAtualizacao ID="PainelAtualizacao" runat="server" />
                <fieldset class="form-stacked actions">
                    <asp:Button ID="cmdSalvar" runat="server" class="btn  btn-lg btn-success" OnClick="cmdSalvar_Click" Text="Salvar" />
                    &nbsp;
                    <input type="submit" class="btn btn-lg btn-warning" name="cancel" onclick="history.go(-1)" title="Voltar" value="Cancelar">
                </fieldset>
                <asp:HiddenField ID="hddidFluxo" runat="server" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
