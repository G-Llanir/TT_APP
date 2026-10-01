<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="EntregaEPI_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.EntregaEPI_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="RV" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <style>
        .id {
            display: none;
        }

        .card {
            box-shadow: 0px 2px 8px 2px rgba(0,0,0,0.3);
            transition: 0.3s;
            width: 100%;
            border-radius: 5px;
        }

        .card-body {
            padding: 10px;
            display: flex;
            align-items: flex-start;
        }

        .flex-grow-1 {
            flex-grow: 1;
            margin-left: 15px;
        }

        .img-thumbnail {
            border: none;
            width: 100px;
            height: auto;
        }

        .me-3 {
            margin-right: 1rem;
            padding: 2px;
            border-bottom: 1px dashed black;
        }

        .modal-dialog {
            display: flex;
            justify-content: center;
        }

        .modal-title {
            text-shadow: 1px 2px 3px rgba(0, 0, 0, 0.2);
            font-size: 20px;
            font-weight: bold;
            margin: 0;
        }

        .infoObs {
            text-decoration: none;
            color: blue;
        }

        .infoAlterada {
            text-decoration: underline;
            color: red;
        }

        .infoNova {
            text-decoration: underline;
            color: green;
        }
    </style>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <div id="hdd">
                <asp:HiddenField ID="hddidEntregaEPI" runat="server" />
                <asp:HiddenField ID="hddColaboradores" runat="server" Value="[]" />
                <asp:HiddenField ID="hddidSelecionarColaborador" runat="server" Value="0" />
                <asp:HiddenField ID="hddidSelecionarColaborador_Original" runat="server" Value="0" />
                <asp:HiddenField ID="hddEPIs" runat="server" Value="[]" />
                <asp:HiddenField ID="hddIncluir_idEPI" runat="server" Value="0" />
                <asp:HiddenField ID="hddidStatus" runat="server" Value="0" />
                <asp:HiddenField ID="hddEditar" runat="server" Value="true" />
                <asp:HiddenField ID="hddsDataEntrega_Manual" runat="server" Value="N" />
                <asp:HiddenField ID="hddConferencia" runat="server" Value="0" />
            </div>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>
            </div>

            <div id="abas">
                <ul id="tab_EPI" class="nav nav-tabs" role="tablist">
                    <li role="presentation" class="tabpanel active">
                        <a href="#epi" id="aba_EntregaEPI" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false" runat="server"><b>Entrega de EPI</b></a>
                    </li>
                    <li role="presentation" runat="server" id="aba_Arquivos">
                        <a href="#arquivos" role="tab" id="aba_arquivos-tab" data-toggle="tab" aria-controls="arquivos"><b>Arquivos</b></a>
                    </li>
                    <li role="presentation" runat="server" id="aba_Historico">
                        <a href="#historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="Historico"><b>Histórico</b></a>
                    </li>
                </ul>
            </div>

            <div id="tab" class="tab-content">

                <div role="tabpanel" class="tab-pane fade in active" id="epi" aria-labelledby="epi-tab">
                    <br />
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 runat="server" id="tituloPagina" class="panel-title"><b>Entrega de EPI</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group row">

                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Entrega" />
                                </div>

                                <div id="div_Entrega">
                                    <div class="col-lg-12">
                                        <div class="form-stacled row">

                                            <div id="id" class="row col-lg-12">
                                                <div class="col-lg-1 form-group">
                                                    <label>ID</label>
                                                    <asp:TextBox runat="server" ID="txtidEntrega" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div id="div_ddlStatus" runat="server" class="row col-lg-12">
                                                <div class="col-lg-3 form-group">
                                                    <label>Status</label>
                                                    <asp:DropDownList runat="server" ID="ddlidStatus" CssClass="form-control Caixa_Selecao" disabled=""></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div id="colaborador" class="row col-lg-12">
                                                <div class="col-lg-3 form-group">
                                                    <label>CPF / CNPJ</label>
                                                    <asp:TextBox runat="server" ID="txtCPF_Colaborador" CssClass="form-control"></asp:TextBox>
                                                </div>

                                                <div class="col-lg-7 form-group">
                                                    <label>Nome do Colaborador</label>
                                                    <asp:TextBox runat="server" ID="txtNome_Colaborador" CssClass="form-control"></asp:TextBox>
                                                </div>

                                                <div runat="server" id="div_cmdSelecionarColaborador" class="col-lg-2 form-group">
                                                    <label>&nbsp;</label>
                                                    <asp:Button runat="server" ID="cmdSelecionarColaborador" CssClass="form-control btn-info" Text="Selecionar Colaborador" OnClick="cmdSelecionarColaborador_Click" />
                                                </div>
                                            </div>

                                            <div id="div_ddlTipo" runat="server" class="row col-lg-12">
                                                <div class="col-lg-2 form-group">
                                                    <label>Tipo de Entrega</label>
                                                    <asp:DropDownList runat="server" ID="ddlTipo" CssClass="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlTipo_SelectedIndexChanged"></asp:DropDownList>
                                                </div>

                                                <div class="col-lg-2 form-group">
                                                    <label runat="server" id="lbl_dtSolicitacao">Data da Solicitação</label>
                                                    <asp:TextBox runat="server" ID="txtdtSolicitacao" CssClass="form-control" type="date"></asp:TextBox>
                                                </div>

                                                <div runat="server" id="div_dtEntrega" class="col-lg-2 form-group">
                                                    <label>Data da Entrega</label>
                                                    <asp:TextBox runat="server" ID="txtdtEntrega" CssClass="form-control" type="date"></asp:TextBox>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Incluir_EPI" />
                                </div>

                                <div id="div_EPIs" runat="server" class="col-lg-12">
                                    <div class="panel panel-default">
                                        <div class="panel-heading">
                                            <h3 class="panel-title"><b>EPIs</b></h3>
                                        </div>
                                        <div class="panel-body">
                                            <div class="form-group row">

                                                <div runat="server" id="divIncluir_EPI" class="row">
                                                    <div class="col-lg-12">

                                                        <div class="col-lg-2 form-group">
                                                            <label>Código do EPI</label>
                                                            <asp:TextBox ID="txtCodigo_EPI" class="form-control" runat="server"></asp:TextBox>
                                                        </div>

                                                        <div class="col-lg-4 form-group">
                                                            <label>Descrição do EPI</label>
                                                            <asp:TextBox ID="txtDesc_EPI" class="form-control" runat="server"></asp:TextBox>
                                                        </div>

                                                        <div class="col-lg-1 form-group">
                                                            <label>CA do EPI</label>
                                                            <asp:TextBox ID="txtIncluirEPI_CA" class="form-control qtd" runat="server"></asp:TextBox>
                                                        </div>

                                                        <div class="col-lg-1 form-group">
                                                            <label>Qtd do EPI</label>
                                                            <asp:TextBox ID="txtIncluirEPI_Qtd" class="form-control qtd" runat="server" MaxLength="3" Text="1"></asp:TextBox>
                                                        </div>

                                                        <div class="col-lg-2" style="padding: 0;">
                                                            <label runat="server" id="lblPeriodicidade_Incluir_EPI">Periodicidade</label>
                                                            <div class="input-group" style="width: 100%;">
                                                                <asp:TextBox ID="txtIncluirEPI_nTempo" class="form-control qtd" runat="server" MaxLength="3" Text="1" aria-label="..." Style="width: 40%;"></asp:TextBox>

                                                                <asp:DropDownList ID="ddlIncluirEPI_sTipoPeriodo" runat="server" class="form-control input-group-addon" aria-label="..." Style="width: 60%; z-index: 1; background-color: white; text-align: start;">
                                                                    <asp:ListItem Value="H">Hora</asp:ListItem>
                                                                    <asp:ListItem Value="D">Dia</asp:ListItem>
                                                                    <asp:ListItem Value="S">Semana</asp:ListItem>
                                                                    <asp:ListItem Value="M">Mês</asp:ListItem>
                                                                    <asp:ListItem Value="A">Ano</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2 form-group">
                                                            <label>&nbsp;</label>
                                                            <asp:Button ID="cmdEPI_Incluir" class="form-control btn-primary" runat="server" Text="Incluir EPI" OnClick="cmdEPI_Incluir_Click" />
                                                        </div>

                                                        <div runat="server" id="div_Excluir">
                                                            <div class="col-lg-10 form-group">&nbsp;</div>

                                                            <div class="col-lg-2 form-group">
                                                                <label>&nbsp;</label>
                                                                <asp:Button ID="cmdEPI_Excluir" class="form-control btn-danger" runat="server" Text="Excluir EPI" OnClick="cmdEPI_Excluir_Click" />
                                                            </div>
                                                        </div>

                                                    </div>
                                                </div>

                                                <div runat="server" id="div_dtgEPI" class="col-lg-12">
                                                    <asp:GridView ID="dtgEPI" class="table table-striped table-bordered table-hover table-condensed"
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgEPI_RowDataBound">
                                                        <Columns>

                                                            <asp:BoundField DataField="idlinha" HeaderText="idLinha">
                                                                <HeaderStyle CssClass="id" />
                                                                <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="id" />
                                                            </asp:BoundField>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                                DataTextField="sCodigoEPI" HeaderText="Código do EPI"
                                                                DataNavigateUrlFormatString="/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="17%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link" />
                                                            </asp:HyperLinkField>

                                                            <asp:TemplateField HeaderText="Descrição do EPI">
                                                                <ItemTemplate>
                                                                    <div>
                                                                        <asp:LinkButton ID="lnkProdutoDetalhe" runat="server" Text='<%# Eval("sDscEPI") %>'
                                                                            OnClientClick='<%# "openModal(\"" + Eval("idItem") + "\"); return false;" %>'
                                                                            OnMouseOver='<%# "mostraCard(this, \"" + Eval("idItem") + "\", \"produto\");" %>'
                                                                            OnMouseOut='<%# "escondeCard( \"" + Eval("idItem") + "\", \"produto\");" %>'
                                                                            CssClass="epi-detalhe-link"
                                                                            ClientIDMode="Static"
                                                                            data-idproduto='<%# Eval("idItem") %>'
                                                                            data-tabela="epi" />

                                                                        <div id='<%# Eval("idItem") + "_produto" %>' class="product-card" style="display: none;">
                                                                            <!--conteudo via script -->
                                                                        </div>
                                                                    </div>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="35%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Estado">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList runat="server" ID="ddlEstado" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlEstado_SelectedIndexChanged">
                                                                        <asp:ListItem Value="1" Text="Ótimo"></asp:ListItem>
                                                                        <asp:ListItem Value="2" Text="Bom"></asp:ListItem>
                                                                        <asp:ListItem Value="3" Text="Regular"></asp:ListItem>
                                                                        <asp:ListItem Value="4" Text="Vencido"></asp:ListItem>
                                                                        <asp:ListItem Value="5" Text="Perdido"></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="CA">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtCA" class="form-control qtd" runat="server" Text='<%# Bind("nCA") %>'></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Quantidade">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtnQuantidateEPI" class="form-control qtd" MaxLength="3" runat="server" Text='<%# Bind("nQuantidateEPI") %>'></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Periodicidade">
                                                                <ItemTemplate>
                                                                    <div style="width: 100%; display: flex;">

                                                                        <div style="width: 45%;">
                                                                            <asp:TextBox ID="txtnQuantidadeTempo" class="form-control qtd" Style="width: 100%" MaxLength="3" runat="server" Text='<%# Bind("nQuantidadeTempo") %>'></asp:TextBox>
                                                                        </div>

                                                                        <div style="width: 55%; margin-left: 5px;">
                                                                            <asp:DropDownList ID="ddlsTipoPeriodo" runat="server" class="form-control" Style="width: 100%">
                                                                                <asp:ListItem Value="H">Hora</asp:ListItem>
                                                                                <asp:ListItem Value="D">Dia</asp:ListItem>
                                                                                <asp:ListItem Value="S">Semana</asp:ListItem>
                                                                                <asp:ListItem Value="M">Mês</asp:ListItem>
                                                                                <asp:ListItem Value="A">Ano</asp:ListItem>
                                                                            </asp:DropDownList>
                                                                        </div>

                                                                    </div>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField>
                                                                <HeaderTemplate>
                                                                    <asp:CheckBox ID="cbEPI_Excluir_Todos" runat="server" CssClass="excluirTodos"></asp:CheckBox>
                                                                </HeaderTemplate>
                                                                <HeaderStyle CssClass="text-center" />
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="cbEPI_Excluir" runat="server" CssClass="form-control excluir"></asp:CheckBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                        </Columns>
                                                    </asp:GridView>
                                                </div>

                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div runat="server" id="div_Obs" class="col-lg-12">
                                    <div class="form-stacked row">

                                        <div class="col-lg-12 form-group">
                                            <label>Observação</label>
                                            <asp:TextBox runat="server" ID="txtObs" CssClass="form-control" MaxLength="400" TextMode="MultiLine" Height="150px"></asp:TextBox>
                                        </div>

                                    </div>
                                </div>

                                <div runat="server" id="div_ComboAtivo" class="col-lg-12">
                                    <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
                                </div>

                            </div>
                        </div>
                    </div>
                </div>

                <div role="tabpanel" class="tab-pane fade" id="arquivos" aria-labelledby="aba_arquivos">
                    <div class=" embed-responsive embed-responsive-16by9" style="min-height: 800px">
                        <embed type="text/html" runat="server" id="eArquivos" width="800" height="500" />
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
                                    <asp:GridView ID="gv_Historico" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" OnRowDataBound="gv_Historico_RowDataBound">
                                        <Columns>

                                            <asp:BoundField DataField="idHistorico" HeaderText="ID">
                                                <HeaderStyle CssClass="id" />
                                                <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="id" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscStatus" HeaderText="Status">
                                                <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscAlteracao" HeaderText="Detalhe da Alteração">
                                                <ItemStyle Width="65%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtHistorico" HeaderText="Data">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
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

            </div>

            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar" />
                <asp:Button ID="cmdEditar" class="btn btn-lg btn-success" runat="server" Text="Editar" />
                <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" id="field-cancel" title="Voltar" onclick="history.go(-1)" />
                <asp:Button ID="cmdGerarPDF" class="btn btn-lg btn-danger" runat="server" Text="PDF" OnClick="cmdGerarPDF_Click" />
                <asp:Button ID="cmdEntregarEPI" class="btn btn-lg btn-primary" runat="server" Text="Definir como Entregue" OnClick="cmdEntregarEPI_Click" />
                <asp:Button ID="cmdConfirmar" class="btn btn-lg btn-primary" runat="server" Text="Confirmar Entrega" OnClick="cmdConfirmar_Click" />
                <asp:Button ID="cmdSubstitutivas" class="btn btn-lg btn-info" runat="server" Text="Reposições de EPIs" OnClick="cmdSubstitutiva_Click" />
                <asp:HyperLink ID="cmdNovaSubstitutiva" class="btn btn-lg btn-info" runat="server" Text="Nova Reposição de EPIs" Target="_blank"></asp:HyperLink>
            </fieldset>

            <div id="dialog-Salvar" class="modal" title="Salvar">
                <p>
                    <label>Deseja Salvar a Entrega de EPI?</label>
                </p>
            </div>

            <div id="dialog-Editar" class="modal" title="Editar">
                <p>
                    <label>Deseja Editar a Entrega de EPI?</label>
                </p>
            </div>

            <div class="modal fade" id="produtoDetalheModal">
                <div class="modal-dialog" style="width: 35%;">
                    <div class="modal-content" style="display: table; width: 100%;">
                        <div class="modal-header" id="modalInfo">
                        </div>
                        <div class="modal-body" id="modalBody">
                            <!-- conteudo via javascript-->
                        </div>
                    </div>
                </div>
            </div>

            <div class="modal fade" id="modalSubs">
                <div class="modal-dialog" style="width: 40%;">
                    <div class="modal-content" style="overflow: visible;">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h3 class="modal-title">Entregas de EPI para Reposição</h3>
                        </div>
                        <div class="modal-body">

                            <div class="row">
                                <div class="col-lg-12 form-group">
                                    <div class="panel panel-primary">
                                        <div class="panel-body">
                                            <asp:BulletedList runat="server" ID="lstSubstituicoes" BulletStyle="Disc" CssClass="lstSubstituicoes"></asp:BulletedList>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <fieldset class="form-stacked actions" style="margin: 0;">
                                        <asp:HyperLink ID="cmdNovaSubstitutiva_modal" class="btn btn-md btn-info" runat="server" Text="Nova Reposição de EPIs" Target="_blank"></asp:HyperLink>
                                        <asp:Button runat="server" ID="cmdFechar_Modal" class="btn btn-md btn-warning" Text="Voltar" data-dismiss="modal" />
                                    </fieldset>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>