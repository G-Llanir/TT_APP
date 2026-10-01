<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Colaboradores_Pagamento_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.Colaboradores_Pagamento_Detalhe" ResponseEncoding="utf-8" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function ColabPag_toggleCheckboxes(gridId, headerChk) {
            var grid = document.getElementById(gridId);
            if (!grid || !headerChk) return;
            var checks = grid.querySelectorAll('input[type=checkbox][id*=Selecionado]');
            for (var i = 0; i < checks.length; i++) {
                if (!checks[i].disabled) checks[i].checked = headerChk.checked;
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <asp:UpdatePanel runat="server" ID="UpdGeral" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Pagamento de Colaboradores"></asp:Label>
                        <small><asp:Label ID="lblSubTituloPagina" runat="server" Text=""></asp:Label></small>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>

                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                    <div class="panel panel-primary" id="pnCabecalho" runat="server">
                        <div class="panel-heading"><h3 class="panel-title">Dados do Pagamento</h3></div>
                        <div class="panel-body">
                            <asp:HiddenField ID="hddidPagamento" runat="server" Value="0" />
                            <asp:HiddenField ID="hddSomenteLeitura" runat="server" Value="N" />
                            <asp:HiddenField ID="hddItensPagamento" runat="server" />
                            <asp:HiddenField ID="hddScrollY" runat="server" Value="0" />
                            <div class="row">
                                <div class="col-lg-2 form-group">
                                    <label>Tipo</label>
                                    <asp:DropDownList ID="ddlidTipo" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                    <asp:TextBox ID="txtsDscTipo" class="form-control" runat="server" ReadOnly="true" Visible="false"></asp:TextBox>
                                </div>
                                <div class="col-lg-2 form-group">
                                    <label>Data Emiss&atilde;o</label>
                                    <asp:TextBox ID="txtdtEmissao" class="form-control" runat="server" TextMode="Date"></asp:TextBox>
                                </div>
                                <div class="col-lg-2 form-group">
                                    <label>Data Vencimento</label>
                                    <asp:TextBox ID="txtdtVencimento" class="form-control" runat="server" TextMode="Date"></asp:TextBox>
                                </div>
                                <div class="col-lg-2 form-group">
                                    <label>Empresa</label>
                                    <asp:DropDownList ID="ddlidEmpresa" class="form-control Caixa_Selecao" runat="server"
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlidEmpresa_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:TextBox ID="txtsDscEmpresa" class="form-control" runat="server" ReadOnly="true" Visible="false"></asp:TextBox>
                                </div>
                                <div class="col-lg-2 form-group">
                                    <label>Situa&ccedil;&atilde;o</label>
                                    <asp:TextBox ID="txtsSituacao" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-12 form-group">
                                    <label>Observa&ccedil;&atilde;o</label>
                                    <asp:TextBox ID="txtsObservacao" class="form-control" runat="server" TextMode="MultiLine" Rows="2"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <asp:Panel ID="pnItensPagamento" runat="server" CssClass="panel panel-success">
                        <div class="panel-heading"><h3 class="panel-title">Colaboradores adicionados ao pagamento</h3></div>
                        <div class="panel-body table-responsive">
                            <asp:Label ID="lblResumoItens" runat="server" CssClass="text-muted"></asp:Label>
                            <p runat="server" id="pnRemoverSelecionados" class="form-group">
                                <asp:Button ID="cmdRemoverSelecionados" runat="server" CssClass="btn btn-danger btn-sm"
                                    Text="Remover selecionados" OnClick="cmdRemoverSelecionados_Click"
                                    OnClientClick="if(window.ColabPag_salvarScroll){ColabPag_salvarScroll();} return confirm('Remover os colaboradores selecionados do pagamento?');" />
                            </p>
                            <asp:GridView ID="gvItensPagamento" runat="server" CssClass="table table-striped table-bordered table-hover"
                                AutoGenerateColumns="False" DataKeyNames="idColaborador" Width="100%"
                                OnRowDataBound="gvItensPagamento_RowDataBound">
                                <Columns>
                                    <asp:TemplateField HeaderText="">
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkItens_SelecionarTudo" runat="server"
                                                onclick="ColabPag_toggleCheckboxes(this.closest('table').id, this);" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkItem_Selecionado" runat="server" />
                                        </ItemTemplate>
                                        <ItemStyle Width="4%" HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="idColaborador" HeaderText="ID" />
                                    <asp:BoundField DataField="sDscColaborador" HeaderText="Colaborador" />
                                    <asp:BoundField DataField="sDscEmpresa" HeaderText="Empresa" />
                                    <asp:BoundField DataField="sDscDepartamento" HeaderText="Departamento" />
                                    <asp:BoundField DataField="sDscCargo" HeaderText="Cargo" />
                                    <asp:BoundField DataField="sDscTipoContrato" HeaderText="Tipo Contrato" />
                                    <asp:TemplateField HeaderText="Valor">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtnValor" runat="server" CssClass="form-control txtnValorPagamento" Width="120px"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <p class="text-muted">Nenhum colaborador adicionado. Use a busca abaixo para incluir colaboradores.</p>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                    </asp:Panel>

                    <div class="panel panel-primary" runat="server" id="DIV_Filtro">
                        <div class="panel-heading"><h3 class="panel-title"><i class="fa fa-search"></i> Buscar colaboradores</h3></div>
                        <div class="panel-body">
                            <p class="text-muted small">A busca respeita a empresa selecionada no cabe&ccedil;alho do pagamento. Marque os colaboradores desejados e use &quot;Adicionar selecionados&quot;.</p>
                            <div class="row">
                                <div class="col-lg-4 form-group">
                                    <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-lg-3 form-group">
                                    <asp:DropDownList ID="ddlidDepartamento" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-lg-2 form-group">
                                    <asp:DropDownList ID="ddlsCBO" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-lg-3 form-group">
                                    <asp:DropDownList ID="ddlidGHE" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-2 form-group">
                                    <asp:DropDownList ID="ddlidCargo" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-lg-2 form-group">
                                    <asp:DropDownList ID="ddlidSupervisorDireto" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-lg-2 form-group">
                                    <asp:DropDownList ID="ddlidTipoContrato" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-lg-2 form-group">
                                    <asp:DropDownList ID="ddlidFuncao" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-lg-2 form-group">
                                    <asp:DropDownList ID="ddlsSituacao" class="form-control Caixa_Selecao" runat="server">
                                        <asp:ListItem Value="T">Todos</asp:ListItem>
                                        <asp:ListItem Value="S" Selected="True">Ativo</asp:ListItem>
                                        <asp:ListItem Value="N">Inativo</asp:ListItem>
                                        <asp:ListItem Value="F">Fora do Quadro</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-lg-2 form-group">
                                    <asp:Button ID="cmdBuscarColaboradores" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdBuscarColaboradores_Click" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <asp:Panel ID="pnColaboradores" runat="server" Visible="false">
                        <div class="panel panel-default">
                            <div class="panel-heading"><h3 class="panel-title">Resultado da busca</h3></div>
                            <div class="panel-body table-responsive">
                                <p class="form-group">
                                    <asp:Button ID="cmdAdicionarSelecionados" runat="server" CssClass="btn btn-success btn-sm"
                                        Text="Adicionar selecionados" OnClick="cmdAdicionarSelecionados_Click"
                                        OnClientClick="if(window.ColabPag_salvarScroll){ColabPag_salvarScroll();}" />
                                </p>
                                <asp:GridView ID="gvColaboradores" runat="server" CssClass="table table-striped table-bordered table-hover"
                                    AutoGenerateColumns="False" DataKeyNames="idColaborador" Width="100%"
                                    OnRowDataBound="gvColaboradores_RowDataBound">
                                    <Columns>
                                        <asp:TemplateField HeaderText="">
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="chkColab_SelecionarTudo" runat="server"
                                                    onclick="ColabPag_toggleCheckboxes(this.closest('table').id, this);" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkColab_Selecionado" runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle Width="4%" HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="idColaborador" HeaderText="ID" />
                                        <asp:BoundField DataField="sDscColaborador" HeaderText="Colaborador" />
                                        <asp:BoundField DataField="sDscEmpresa" HeaderText="Empresa" />
                                        <asp:BoundField DataField="sDscDepartamento" HeaderText="Departamento" />
                                        <asp:BoundField DataField="sDscCargo" HeaderText="Cargo" />
                                        <asp:BoundField DataField="sDscTipoContrato" HeaderText="Tipo Contrato" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </asp:Panel>

                    <br />
                    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" Visible="false" />

                    <fieldset class="form-stacked actions">
                        <asp:Button ID="cmdSalvar" CssClass="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                        <input type="button" class="btn btn-lg btn-warning" value="Voltar" title="Voltar" onclick="history.go(-1)" />
                    </fieldset>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
