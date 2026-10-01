<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="LocalArmazenamento_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.Manutencao.LocalArmazenamento_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <asp:UpdatePanel ID="UpdDetalhe" runat="server">
        <ContentTemplate>
            <style>
                /* GridColapsada*/
                /*CSS Colapso da Tabela*/
                .gvHeader th {
                    padding: 7px;
                    background-color: #1A4C1A;
                    color: #fff;
                    border: 1px solid #bbb;
                    font-weight: normal;
                }

                .gvChildHeader th {
                    padding: 4px;
                    background-color: #999966;
                    color: #fff;
                    border: 1px solid #bbb;
                    font-weight: normal;
                }

                .gvRow td {
                    padding: 7px;
                    background-color: #ffffff;
                    border: 1px solid #bbb;
                }

                .gvAltRow td {
                    padding: 7px;
                    background-color: #f1f1f1;
                    border: 1px solid #bbb;
                }

                .collapsed-row {
                    display: none;
                    padding: 1px;
                    margin: 1px;
                }
            </style>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>


                <div>
                    <ul id="tab_armazenamento" class="nav nav-tabs" role="tablist">
                        <li role="presentation" class="tabpanel active">
                            <a href="#armazenamento" id="aba_armazenamento" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Armazenamento</b></a>
                        </li>
                        <%--      <li role="presentation" id="li_posicao" runat="server">
                            <a href="#posicao" id="aba_posicao" role="tabpanel" data-toggle="tab" aria-controls="Posicao" aria-expanded="false"><b>Posicionamento</b></a>
                        </li>--%>
                    </ul>
                </div>
                <br />

                <div id="tab" class="tab-content">
                    <div role="tabpanel" class="tab-pane fade in active" id="armazenamento" aria-labelledby="armazenamento-tab">
                        <div class="panel panel-default" runat="server" id="div_armazenamento">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Local de Armazenamento</b></h3>
                            </div>
                            <div class="panel-body">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                                </div>
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>ID</label>
                                        <asp:TextBox ID="txtidLocalArmazenamento" runat="server" class="form-control CaixaTextoMini" disabled=""></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Descrição</label>
                                        <asp:TextBox ID="txtsDscLocalArmazenamento" runat="server" class="form-control CaixaTextoGigante" MaxLength="50"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <uc1:SwitchAtivo runat="server" ID="SwitchAtivoPadrao" />
                                        <%--Estoque Padrão--%>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <uc1:SwitchAtivo runat="server" ID="SwitchAtivo" />
                                        <%--Computa Estoque--%>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <uc1:SwitchAtivo runat="server" ID="SwitchAtivoOPI" />
                                        <%--Exibe em OPI--%>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="panel panel-default" runat="server" id="div_posicionamento">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Posicionamento</b></h3>
                            </div>
                            <div class="panel-body">

                                <uc1:MensagemPagina runat="server" ID="MensagemPaginaPosicao" />

                                <div class="form-stacked row">

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Código da Posição</label>
                                            <asp:TextBox ID="txtsCodigoPosicao" class="form-control" runat="server" oninput="mascaraCodigoPosicao(this)" />
                                            <small class="form-text text-muted">Use R, N ou E, seguido do número da posição (ex: R01, N10, E05).</small>
                                        </div>
                                    </div>

                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>Descrição da Posição </label>
                                            <asp:TextBox ID="txtsDscPosicao" class="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Posição Pai</label>
                                            <asp:DropDownList ID="ddlidPosiçãoPai" AutoPostBack="false" class="form-control yes_no select Caixa_Selecao" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <uc1:ComboAtivo runat="server" ID="ComboAtivo1" Visible="false" />
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <br />
                                            <asp:LinkButton class="btn btn-sm btn-primary" Text="Incluir" runat="server" OnClick="IncluirPosicoes_Click" />
                                        </div>
                                    </div>
                                </div>

                                <div id="div_gvPosicao" runat="server" class="tabel-responsive">
                                    <asp:GridView ID="dtgPosicao" runat="server" AutoGenerateColumns="False" DataKeyNames="idPosicao" class="table table-striped table-bordered table-hover table-condensed"
                                        Width="100%" GridLines="None"
                                        HeaderStyle-CssClass="gvHeader" CssClass="gvRow" AlternatingRowStyle-CssClass="gvAltRow"
                                        OnRowDataBound="dtgPosicao_RowDataBound">
                                        <Columns>
                                            <asp:BoundField DataField="IdPosicao" HeaderText="ID" ReadOnly="True" />
                                            <asp:BoundField DataField="SDscPosicao" HeaderText="Descrição" />
                                            <asp:BoundField DataField="SCodigoLocal" HeaderText="Código Local" />
                                            <asp:BoundField DataField="SDscLocal" HeaderText="Local" />
                                            <asp:BoundField DataField="SDscPai" HeaderText="Posição Pai" />
                                            <asp:BoundField DataField="SSituacao" HeaderText="Situação" />
                                            <asp:TemplateField HeaderText="Ação">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="cmdExcluirEtiqueta" CssClass="btn btn-sm btn-danger" runat="server" OnClick="ExcluirPosicoes_Click" CommandArgument='<%# Eval("idPosicao") %>'><i class="fa fa-trash-o"></i></asp:LinkButton>
                                                </ItemTemplate>
                                                <HeaderStyle Width="10%" />
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>

                                <fieldset id="fdBotoes" runat="server" class="form-stacked actions">
                                    <asp:Button ID="cmdSalvarPosicoes" OnClick="SalvarPosicoes_Click" CssClass="btn btn-sm btn-success" Text="Salvar" runat="server" />
                                </fieldset>

                            </div>
                        </div>
                    </div>

                    <%--<div role="tabpanel" class="tab-pane fade" id="posicao" aria-labelledby="posicao-tab">--%>

                    <%-- </div>--%>
                </div>


                <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

                <fieldset class="form-stacked actions">
                    <asp:Button ID="cmdSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                    <input type="submit" name="cancel" class="btn btn-lg btn-warning" id="field-cancel" value="Cancelar" title="Voltar" onclick="history.go(-1)" />

                </fieldset>
                <asp:HiddenField ID="hddidLocalArmazenamento" runat="server" />
                <asp:HiddenField ID="hddidPosicao" runat="server" />
            </div>


            <script>
                document.addEventListener("DOMContentLoaded", function () {
                    let campo = document.getElementById("<%= txtsCodigoPosicao.ClientID %>");
                    let prefixoFixo = campo.value.substring(0, 7); // Pegando os 7 primeiros caracteres

                    // Função para garantir que o prefixo não seja modificado
                    function mascaraCodigoPosicao() {
                        let valor = campo.value;

                        // Garante que o valor sempre começa com o prefixo fixo
                        if (!valor.startsWith(prefixoFixo)) {
                            campo.value = prefixoFixo;
                        }

                        let sufixo = valor.substring(prefixoFixo.length).toUpperCase();
                        let regex = /^[REN]?\d{0,2}$/;

                        if (!regex.test(sufixo)) {
                            campo.value = prefixoFixo + sufixo.substring(0, sufixo.length - 1);
                        }
                    }

                    campo.addEventListener("input", mascaraCodigoPosicao);

                    // Evita que o usuário apague ou edite o prefixo
                    campo.addEventListener("keydown", function (e) {
                        if (campo.selectionStart < prefixoFixo.length && e.key !== "ArrowRight" && e.key !== "ArrowLeft") {
                            e.preventDefault();
                        }
                    });

                    // Reexecuta o script após o UpdatePanel ser atualizado
                    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                        mascaraCodigoPosicao();
                    });
                });
            </script>
        </ContentTemplate>
    </asp:UpdatePanel>


</asp:Content>
