<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Veiculos.aspx.cs" Inherits="TT_Colaborador.Aplicativo.Paginas.Veiculos.Veiculos" %>

<%@ Register Src="~/Aplicativo/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/Aplicativo/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/Aplicativo/Controles/Upload_Arquivo.ascx" TagPrefix="uc1" TagName="Upload_Arquivo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:UpdatePanel ID="updDetalhe" runat="server">
        <ContentTemplate>

            <style>
                .invisivel {
                    display: none !important;
                }

                .efeito {
                    cursor: pointer;
                    text-decoration: none !important;
                    color: black !important;
                    border: none;
                    border-radius: 10px;
                    box-shadow: -7.5px 7.5px 10px rgba(0, 0, 0, .5);
                }

                    .efeito:hover {
                        box-shadow: -7.5px 7.5px 10px rgba(0, 150, 0, 1);
                    }

                    .efeito .card-header {
                        align-self: center;
                        background-color: white;
                        margin-bottom: 0 !important;
                        border: none;
                    }

                    .efeito .card-body .card-title {
                        margin-bottom: 1em;
                    }

                    .efeito .card-footer {
                        align-self: center;
                        background-color: white;
                        gap: .5em;
                        border: none;
                    }

                .altura-scroll {
                    max-height: 600px;
                    overflow-y: scroll;
                }

                .tabelas > div {
                    width: 100%;
                }

                    .tabelas > div > table {
                        width: 100%;
                    }

                .tabelas td, .tabelas th {
                    vertical-align: middle;
                    text-align: center;
                    padding: 1rem;
                }

                .tabelas th {
                    font-weight: bold;
                }

                .btn-outline-success {
                    width: max-content;
                    margin: 0;
                }

                .checklist input {
                    display: none;
                }

                .checklist i {
                    margin: auto;
                }

                .checklist td:has(input) {
                    cursor: pointer;
                    color: black;
                }

                .checklist td:has(input:checked) i {
                    display: block;
                }

                .checklist td:has(input:not(:checked)) i {
                    display: none;
                }

                .checklist td:has(input:not(:checked)) {
                    background-color: white;
                }

                .card-img-top {
                    cursor: pointer;
                }

                .btnCheckList {
                    padding: .5rem 1rem;
                }

                    .btnCheckList ul {
                        padding-left: 1rem;
                        margin: 0;
                    }

                @media only screen and (max-device-width: 768px) {
                    .checklist {
                        border: .5px solid black;
                    }

                        .checklist th, .checklist tr {
                            font-size: 2rem;
                            border: .5px solid black;
                        }

                        .checklist td:has(input) {
                            color: white !important;
                        }

                        .checklist td:has(span[data-situacao="0"] input:checked) {
                            background-color: slategray;
                        }

                        .checklist td:has(span[data-situacao="N"] input:checked) {
                            background-color: royalblue;
                        }

                        .checklist td:has(span[data-situacao="B"] input:checked) {
                            background-color: forestgreen;
                        }

                        .checklist td:has(span[data-situacao="R"] input:checked) {
                            background-color: darkred;
                        }

                    .btnCheckList {
                        width: 50%;
                    }

                        .btnCheckList ul {
                            padding-left: 2rem;
                        }
                }
            </style>

            <div id="hdd">
                <asp:HiddenField ID="hddidMovimentacao" runat="server" Value="0" />
                <asp:HiddenField ID="hddEtapaAtual" runat="server" Value="1" />
                <asp:HiddenField ID="hddLatitude" runat="server" />
                <asp:HiddenField ID="hddLongitude" runat="server" />
            </div>

            <div runat="server" id="div_filtros" class="card form-group">

                <div class="card-header bg-primary text-white">
                    <h3 class="card-title"><i class="fa fa-bar-chart"></i>Filtro para pesquisa</h3>
                </div>

                <div class="card-body">
                    <div class="row">

                        <div class="col-6 form-group">
                            <asp:TextBox ID="txtPesquisa" CssClass="form-control" runat="server" placeholder="Pesquisar" MaxLength="200"></asp:TextBox>
                        </div>

                        <div class="col-6">
                            <asp:DropDownList ID="ddlVeiculo_Filtro" CssClass="form-select" runat="server"></asp:DropDownList>
                        </div>

                        <div class="col-12">
                            <asp:Button ID="cmdPesquisar" CssClass="btn btn-md btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao_Consulta(this)" />
                            <asp:Button ID="cmdNovo" CssClass="btn btn-md btn-success" runat="server" Text="Novo" OnClick="cmdNovo_Click" />
                            <asp:Button ID="cmdLegenda" CssClass="btn btn-md btn-info" runat="server" Text="Legenda" OnClientClick="$('#divLegenda').toggle('fast'); return false;" />
                            <div class="card" style="width: max-content; margin-top: 10px;">
                                <div id="divLegenda" class="card-body" style="display: none;">
                                    <p class="btn btn-outline-success"><i class="fa fa-tag"></i>Placa</p>
                                    <p class="btn btn-outline-success"><i class="fa fa-store"></i>Locadora</p>
                                    <p class="btn btn-outline-success"><i class="fa fa-calendar-days"></i>Rodízio</p>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>

            </div>

            <div class="card">

                <div class="card-header card-tt">
                    <h3 class="card-title">
                        <b>
                            <label id="lblTituloPagina"><i class="fa fa-car-side"></i>Movimentação - Veículos</label>
                        </b>
                    </h3>
                </div>

                <div class="col-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                </div>

                <div id="div_consulta" runat="server" class="card card-body" style="border: none; gap: 2em;">

                    <asp:Repeater ID="rptConsulta" runat="server" OnItemCommand="rptConsulta_ItemCommand">
                        <ItemTemplate>

                            <asp:LinkButton runat="server" ID="lnkMovimentacao" CommandArgument='<%# Eval("idMovimentacao") %>' CssClass="efeito" ClientIDMode="Static">
                                    <div class="card text-center">
                                        <div class="card-header">
                                            <h2 class="card-title"><%# Eval("sDscVeiculo") %></h2>
                                        </div>
                                        <div class="card-body">
                                            <p class="btn btn-outline-success"><i class="fa fa-tag"></i> <%# Eval("sPlaca") %></p>
                                            <p class="btn btn-outline-success"><i class="fa fa-store"></i> <%# Eval("sLocadora") %></p>
                                            <p class="btn btn-outline-success"><i class="fa fa-calendar-days"></i> <%# Eval("sRodizio") %></p>
                                        </div>
                                        <div class="card-footer div-flex justify-content-center text-center">
                                            <%# $"<h4><span class='badge rounded-pill {(Eval("idVeiculo").Equals(-1) ? "text-bg-success'><i class='fa fa-check'></i>" : "text-bg-danger'><i class='fa fa-xmark'></i>")} Veículo Alugado</span></h4>" %>
                                            <%# $"<h4><span class='badge rounded-pill {(Eval("sSeguro").Equals("S") ? "text-bg-success'><i class='fa fa-check'></i>" : "text-bg-danger'><i class='fa fa-xmark'></i>")} Seguro</span></h4>" %>
                                            <%# $"<h4><span class='badge rounded-pill {(Eval("sRastreador").Equals("S") ? "text-bg-success'><i class='fa fa-check'></i>" : "text-bg-danger'><i class='fa fa-xmark'></i>")} Rasterador</span></h4>" %>
                                        </div>
                                    </div>
                            </asp:LinkButton>

                        </ItemTemplate>
                    </asp:Repeater>

                </div>

                <div id="div_detalhe" runat="server" class="col-12">
                    <div class="row">

                        <div class="col-12 form-group padd-0">
                            <div class="col-2">
                                <label class="form-label">ID</label>
                                <asp:TextBox ID="txtID" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                            </div>
                        </div>

                        <div runat="server" id="div_Veiculo" class="col-12 form-group">
                            <label class="form-label">Veículo</label>
                            <asp:DropDownList ID="ddlVeiculo" CssClass="form-select" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlVeiculo_SelectedIndexChanged"></asp:DropDownList>
                        </div>

                        <div runat="server" id="div_VeiculoAlugado" class="col-12 form-group div-flex padd-0">

                            <div class="col-6">
                                <label class="form-label">Marca</label>
                                <asp:TextBox ID="txtMarca" CssClass="form-control" runat="server" MaxLength="50"></asp:TextBox>
                            </div>

                            <div class="col-6">
                                <label class="form-label">Modelo</label>
                                <asp:TextBox ID="txtModelo" CssClass="form-control" runat="server" MaxLength="100"></asp:TextBox>
                            </div>

                            <div class="col-4">
                                <label class="form-label">Placa</label>
                                <asp:TextBox ID="txtPlaca" CssClass="form-control" runat="server" MaxLength="20"></asp:TextBox>
                            </div>

                            <div class="col-4">
                                <label class="form-label">Ano</label>
                                <asp:TextBox ID="txtAno" CssClass="form-control" runat="server" MaxLength="20" placeholder="Ex: 2020/2021"></asp:TextBox>
                            </div>

                            <div class="col-4">
                                <label class="form-label">Cor</label>
                                <asp:TextBox ID="txtCor" CssClass="form-control" runat="server" MaxLength="20"></asp:TextBox>
                            </div>

                            <div class="col-3 form-check form-switch">
                                <label class="form-check-label" for="switchCheckDefault">Seguro</label>
                                <input runat="server" id="swSeguro" type="checkbox" role="switch" class="form-check-input form-check-switch">
                            </div>

                            <div class="col-3 form-check form-switch">
                                <label class="form-check-label" for="switchCheckDefault">Rastreador</label>
                                <input runat="server" id="swRastreador" type="checkbox" role="switch" class="form-check-input form-check-switch">
                            </div>

                            <div class="col-6">
                                <label class="form-label">Locadora</label>
                                <asp:TextBox ID="txtLocadora" CssClass="form-control" runat="server" MaxLength="100"></asp:TextBox>
                            </div>

                            <div class="col-4">
                                <label class="form-label">Retirada</label>
                                <asp:TextBox ID="txtRetirada" CssClass="form-control" runat="server" TextMode="Date"></asp:TextBox>
                            </div>

                            <div class="col-4">
                                <label class="form-label">Devolução</label>
                                <asp:TextBox ID="txtDevolucao" CssClass="form-control" runat="server" TextMode="Date"></asp:TextBox>
                            </div>

                            <div class="col-4">
                                <label class="form-label">Rodízio</label>
                                <asp:TextBox ID="txtRodizio" CssClass="form-control" runat="server" MaxLength="20" placeholder="Ex: Segunda-Feira"></asp:TextBox>
                            </div>

                        </div>

                        <div runat="server" id="div_Itens" class="col-12 form-group">
                            <div class="card">
                                <div class="card-header card-tt margin-0">
                                    <h3 class="card-title">Registro de Viagens</h3>
                                </div>
                                <div class="card-body">

                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Itens" />

                                    <div class="row">

                                        <div runat="server" id="div_IncluirItens" class="col-12">
                                            <asp:LinkButton runat="server" ID="cmdIncluirItem_modal" class="btn btn-lg btn-primary" data-bs-toggle="modal" data-bs-target="#modal_IncluirItem">Nova Viagem</asp:LinkButton>
                                        </div>

                                        <div runat="server" id="div_rptViagens" class="col-12">
                                            <div class="card" style="padding: 1em; gap: 1.5rem;">
                                                <asp:Repeater runat="server" ID="rptViagens">
                                                    <ItemTemplate>

                                                        <div class="card">
                                                            <div class="card-header">
                                                                <h2 class="card-title"><%# Eval("sDscTipoMovimentacao") %></h2>
                                                            </div>
                                                            <div class="card-body">
                                                                <p class="btn btn-success margin-0"><i class="fa fa-arrows-left-right"></i> <%# decimal.Parse(Eval("nKilometros").ToString()).ToString("N0") + " km" %></p>
                                                                <p class="btn btn-success margin-0"><%# Eval("idSituacao_Tanque").ToString() == "1" ? "<i class='fa fa-battery-quarter'></i> 1/4 de Tanque" : Eval("idSituacao_Tanque").ToString() == "2" ? "<i class='fa fa-battery-half'></i> Meio Tanque" : Eval("idSituacao_Tanque").ToString() == "3" ? "<i class='fa fa-battery-three-quarters'></i> 3/4 de Tanque" : Eval("idSituacao_Tanque").ToString() == "4" ? "<i class='fa fa-battery-full'></i> Tanque Cheio" : "<i class='fa fa-battery-empty'></i> Tanque Vazio" %></p>

                                                                <%# string.IsNullOrEmpty(Convert.ToString(Eval("sObservacao"))) ? "" : "<p class=\"btn btn-outline-success\" onclick=\"$(this).siblings('#divObs').toggle('normal');\"><i class=\"fa fa-comment\"></i> Observação</p>" %>
                                                                <%# Eval("idTipoViagem").ToString() == "1" || Eval("idTipoViagem").ToString() == "5" ? "<p class=\"btn btn-outline-success\" onclick=\"$(this).siblings('#divCheckList').toggleClass('invisivel');\"><i class=\"fa fa-list-check\"></i> Inspeção</p>" : "" %>
                                                                <p class="btn btn-outline-success" onclick="$(this).siblings('#divFotos').toggle('normal');"><i class="fa fa-images"></i> Fotos</p>

                                                                <div id="divObs" class="font-18" style="display: none; border: 1px solid black; border-radius: 10px; padding: .5em; margin-top: 1em;"><%# Eval("sObservacao") %></div>
                                                                <div id="divCheckList" class="invisivel font-18" style="display: flex; flex-wrap: wrap; border: 1px solid black; border-radius: 10px; padding: .5em; margin-top: 1em;">
                                                                    <%# ValidaStatus_CheckList(Eval("idRegistro").ToString(), 'N', out string Novos) ? $"<div class=\"btn btn-success btnCheckList margin-0\"><b>Novo</b> <ul style='text-align: start;'>{Novos}</ul></div>" : "" %>
                                                                    <%# ValidaStatus_CheckList(Eval("idRegistro").ToString(), 'B', out string Bons) ? $"<div class=\"btn btn-primary btnCheckList margin-0\"><b>Bom Estado</b> <ul style='text-align: start;'>{Bons}</ul></div>" : "" %>
                                                                    <%# ValidaStatus_CheckList(Eval("idRegistro").ToString(), 'R', out string Ruins) ? $"<div class=\"btn btn-danger btnCheckList margin-0\"><b>Mal Estado</b> <ul style='text-align: start;'>{Ruins}</ul></div>" : "" %>
                                                                    <%# ValidaStatus_CheckList(Eval("idRegistro").ToString(), '0', out string NaoPossui) ? $"<div class=\"btn btn-secondary btnCheckList margin-0\"><b>Não Possui</b> <ul style='text-align: start;'>{NaoPossui}</ul></div>" : "" %>
                                                                </div>
                                                                <div id="divFotos" class="font-18" style="display: none; border: 1px solid black; border-radius: 10px; padding: .5em; margin-top: 1em;">
                                                                    <img src='<%# "data:image/png;base64," + Eval("vbArquivo_Painel") %>' alt="Foto do Painel" width="120" height="120" data-bs-toggle="tooltip_bottom" data-bs-title="Foto do Painel" onclick="$('[id*=imgArquivo]').attr('src', $(this).attr('src')); $('#lblTitulo_modalImagem').text($(this).attr('alt')); $('#modal_Imagem').modal('show');" />
                                                                    <%# string.IsNullOrEmpty(Convert.ToString(Eval("vbArquivo_Frente"))) ? "" : $"<img src='data:image/png;base64,{Eval("vbArquivo_Frente")}' alt='Foto da Frente' width='120' height='120' data-bs-toggle=\"tooltip_bottom\" data-bs-title=\"Foto da Frente\" onclick=\"$('[id*=imgArquivo]').attr('src', $(this).attr('src')); $('#lblTitulo_modalImagem').text($(this).attr('alt')); $('#modal_Imagem').modal('show');\" />" %>
                                                                    <%# string.IsNullOrEmpty(Convert.ToString(Eval("vbArquivo_Traseira"))) ? "" : $"<img src='data:image/png;base64,{Eval("vbArquivo_Traseira")}' alt='Foto da Traseira' width='120' height='120' data-bs-toggle=\"tooltip_bottom\" data-bs-title=\"Foto da Traseira\" onclick=\"$('[id*=imgArquivo]').attr('src', $(this).attr('src')); $('#lblTitulo_modalImagem').text($(this).attr('alt')); $('#modal_Imagem').modal('show');\" />" %>
                                                                    <%# string.IsNullOrEmpty(Convert.ToString(Eval("vbArquivo_Lat_Direita"))) ? "" : $"<img src='data:image/png;base64,{Eval("vbArquivo_Lat_Direita")}' alt='Foto da Lateral Direita' width='120' height='120' data-bs-toggle=\"tooltip_bottom\" data-bs-title=\"Foto da Lateral Direita\" onclick=\"$('[id*=imgArquivo]').attr('src', $(this).attr('src')); $('#lblTitulo_modalImagem').text($(this).attr('alt')); $('#modal_Imagem').modal('show');\" />" %>
                                                                    <%# string.IsNullOrEmpty(Convert.ToString(Eval("vbArquivo_Lat_Esquerda"))) ? "" : $"<img src='data:image/png;base64,{Eval("vbArquivo_Lat_Esquerda")}' alt='Foto da Lateral Esquerda' width='120' height='120' data-bs-toggle=\"tooltip_bottom\" data-bs-title=\"Foto da Lateral Esquerda\" onclick=\"$('[id*=imgArquivo]').attr('src', $(this).attr('src')); $('#lblTitulo_modalImagem').text($(this).attr('alt')); $('#modal_Imagem').modal('show');\" />" %>
                                                                </div>
                                                            </div>
                                                            <div class="card-footer">
                                                                <%# $"<h4><span class='badge rounded-pill text-bg-secondary font-16'>Cadastrado em <b>{Eval("dtMovimentacao")}</b> por <b>{Eval("sDscUsuario")}</b></span></h4>" %>
                                                            </div>
                                                        </div>

                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>

                        <div id="div_botoes_painelAtualizacao" class="col-12">
                            <fieldset class="form-stacked actions">
                                <asp:Button ID="cmdSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                                <asp:Button ID="cmdVoltar" class="btn btn-lg btn-warning" runat="server" Text="Voltar" OnClick="cmdVoltar_Click" />
                            </fieldset>

                            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                        </div>

                    </div>
                </div>

            </div>

            <div class="modal fade" id="modal_IncluirItem" tabindex="-1">
                <div class="modal-dialog modal-max">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h1 runat="server" class="modal-title" id="lblTitulo_Modal_IncluirItem">Nova Viagem</h1>
                            <button type="button" class="btn btn-close" data-bs-dismiss="modal" aria-label="Fechar"></button>
                        </div>
                        <div class="modal-body padd-0">

                            <br />

                            <div class="col-12">
                                <uc1:MensagemPagina runat="server" ID="MensagemPagina_Modal" />
                            </div>

                            <div class="div-flex">

                                <div id="divEtapa_1" class="col-12 div-flex padd-0">

                                    <div class="col-12 form-group">
                                        <label class="form-label">Destino</label>
                                        <asp:DropDownList runat="server" ID="ddlTipoMovimentacao_IncluirItem" CssClass="form-select"></asp:DropDownList>
                                    </div>

                                    <div class="col-12 form-group">
                                        <label class="form-label">Centro de Custo</label>
                                        <asp:DropDownList runat="server" ID="ddlCentroCusto_IncluirItem" CssClass="form-select" required></asp:DropDownList>
                                        <div class="invalid-feedback">É necessário selecionar um Centro de Custo!</div>
                                    </div>

                                    <div class="col-6 form-group">
                                        <label class="form-label">Kilometragem</label>
                                        <div class="input-group">
                                            <asp:TextBox runat="server" ID="txtKilometros_IncluirItem" CssClass="form-control mascara-int" placeholder="Ex: 10.000"></asp:TextBox>
                                            <span class="input-group-text">km</span>
                                            <div class="invalid-feedback">É necessário preencher a Kilometragem do veículo!</div>
                                        </div>
                                    </div>

                                    <div class="col-12 form-group">
                                        <label class="form-label">Nível do Tanque</label>
                                        <div class="form-label" style="display: flex; justify-content: space-between; width: 100%; font-weight: normal;"><small>Vazio</small><small>1/4</small><small>Metade</small><small>3/4</small><small>Cheio</small></div>
                                        <input runat="server" type="range" id="rnTanque_IncluirItem" class="form-range" min="0" max="4" value="0" step="1" />
                                    </div>

                                    <div class="col-12 form-group">
                                        <label class="form-label">Observação</label>
                                        <asp:TextBox runat="server" ID="txtObservacao_IncluirItem" CssClass="form-control" TextMode="MultiLine" Rows="5" MaxLength="400"></asp:TextBox>
                                    </div>

                                </div>

                                <div id="div_CheckLists_IncluirItem" class="col-12 div-flex tabelas" style="justify-content: space-between;">

                                    <div id="divEtapa_2" class="form-group">
                                        <asp:Table runat="server" ID="tbPneus_IncluirItem" CssClass="table table-bordered table-hover dataTable table-striped checklist">

                                            <asp:TableHeaderRow>
                                                <asp:TableHeaderCell RowSpan="2">RODAS E PNEUS</asp:TableHeaderCell>
                                                <asp:TableHeaderCell ColumnSpan="3">Situação</asp:TableHeaderCell>
                                            </asp:TableHeaderRow>
                                            <asp:TableHeaderRow>
                                                <asp:TableHeaderCell>Novos</asp:TableHeaderCell>
                                                <asp:TableHeaderCell>Bons</asp:TableHeaderCell>
                                                <asp:TableHeaderCell>Ruins</asp:TableHeaderCell>
                                            </asp:TableHeaderRow>

                                            <asp:TableRow>
                                                <asp:TableCell>Pneus Dianteiros</asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbPneusDianteiros_Novo" data-situacao="N" GroupName="PneusDianteiros" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbPneusDianteiros_Bom" data-situacao="B" Checked="true" GroupName="PneusDianteiros" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbPneusDianteiros_Ruim" data-situacao="R" GroupName="PneusDianteiros" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell>Pneus Traseiros</asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbPneusTraseiros_Novo" data-situacao="N" GroupName="PneusTraseiros" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbPneusTraseiros_Bom" data-situacao="B" Checked="true" GroupName="PneusTraseiros" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbPneusTraseiros_Ruim" data-situacao="R" GroupName="PneusTraseiros" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell>Rodas Dianteiras</asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbRodasDianteiras_Novo" data-situacao="N" GroupName="RodasDianteiras" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbRodasDianteiras_Bom" data-situacao="B" Checked="true" GroupName="RodasDianteiras" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbRodasDianteiras_Ruim" data-situacao="R" GroupName="RodasDianteiras" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell>Rodas Traseiras</asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbRodasTraseiras_Novo" data-situacao="N" GroupName="RodasTraseiras" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbRodasTraseiras_Bom" data-situacao="B" Checked="true" GroupName="RodasTraseiras" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbRodasTraseiras_Ruim" data-situacao="R" GroupName="RodasTraseiras" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                            </asp:TableRow>

                                        </asp:Table>
                                        <br />
                                        <asp:Table runat="server" ID="tbAcessorios_IncluirItem" CssClass="table table-bordered table-hover dataTable table-striped checklist">

                                            <asp:TableHeaderRow>
                                                <asp:TableHeaderCell RowSpan="2">ACESSÓRIOS</asp:TableHeaderCell>
                                                <asp:TableHeaderCell ColumnSpan="3">Situação</asp:TableHeaderCell>
                                            </asp:TableHeaderRow>
                                            <asp:TableHeaderRow>
                                                <asp:TableHeaderCell>Não Possui</asp:TableHeaderCell>
                                                <asp:TableHeaderCell>Bons</asp:TableHeaderCell>
                                                <asp:TableHeaderCell>Ruins</asp:TableHeaderCell>
                                            </asp:TableHeaderRow>

                                            <asp:TableRow>
                                                <asp:TableCell>Calotas</asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbCalotas_NaoPossui" data-situacao="0" GroupName="Calotas" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbCalotas_Bom" data-situacao="B" Checked="true" GroupName="Calotas" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbCalotas_Ruim" data-situacao="R" GroupName="Calotas" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell>Retrovisores</asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbRetrovisores_NaoPossui" data-situacao="0" GroupName="Retrovisores" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbRetrovisores_Bom" data-situacao="B" Checked="true" GroupName="Retrovisores" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbRetrovisores_Ruim" data-situacao="R" GroupName="Retrovisores" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell>Palhetas</asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbPalhetas_NaoPossui" data-situacao="0" GroupName="Palhetas" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbPalhetas_Bom" data-situacao="B" Checked="true" GroupName="Palhetas" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbPalhetas_Ruim" data-situacao="R" GroupName="Palhetas" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell>Triângulo</asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbTriangulo_NaoPossui" data-situacao="0" GroupName="Triangulo" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbTriangulo_Bom" data-situacao="B" Checked="true" GroupName="Triangulo" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbTriangulo_Ruim" data-situacao="R" GroupName="Triangulo" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell>Macaco / Chave de Roda</asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbMacaco_NaoPossui" data-situacao="0" GroupName="Macaco" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbMacaco_Bom" data-situacao="B" Checked="true" GroupName="Macaco" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbMacaco_Ruim" data-situacao="R" GroupName="Macaco" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell>Estepe</asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbEstepe_NaoPossui" data-situacao="0" GroupName="Estepe" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbEstepe_Bom" data-situacao="B" Checked="true" GroupName="Estepe" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbEstepe_Ruim" data-situacao="R" GroupName="Estepe" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                            </asp:TableRow>

                                        </asp:Table>
                                    </div>

                                    <div id="divEtapa_3" class="form-group">
                                        <asp:Table runat="server" ID="tbInterior_IncluirItem" CssClass="table table-bordered table-hover dataTable table-striped checklist">

                                            <asp:TableHeaderRow>
                                                <asp:TableHeaderCell RowSpan="2">INTERIOR</asp:TableHeaderCell>
                                                <asp:TableHeaderCell ColumnSpan="3">Situação</asp:TableHeaderCell>
                                            </asp:TableHeaderRow>
                                            <asp:TableHeaderRow>
                                                <asp:TableHeaderCell>Não Possui</asp:TableHeaderCell>
                                                <asp:TableHeaderCell>Bons</asp:TableHeaderCell>
                                                <asp:TableHeaderCell>Ruins</asp:TableHeaderCell>
                                            </asp:TableHeaderRow>

                                            <asp:TableRow>
                                                <asp:TableCell>Bancos</asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbBancos_NaoPossui" data-situacao="0" GroupName="Bancos" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbBancos_Bom" data-situacao="B" Checked="true" GroupName="Bancos" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbBancos_Ruim" data-situacao="R" GroupName="Bancos" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell>Painel</asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbPainel_NaoPossui" data-situacao="0" GroupName="Painel" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbPainel_Bom" data-situacao="B" Checked="true" GroupName="Painel" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbPainel_Ruim" data-situacao="R" GroupName="Painel" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell>Consoles</asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbConsoles_NaoPossui" data-situacao="0" GroupName="Consoles" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbConsoles_Bom" data-situacao="B" Checked="true" GroupName="Consoles" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbConsoles_Ruim" data-situacao="R" GroupName="Consoles" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell>Forração</asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbForro_NaoPossui" data-situacao="0" GroupName="Forro" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbForro_Bom" data-situacao="B" Checked="true" GroupName="Forro" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbForro_Ruim" data-situacao="R" GroupName="Forro" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell>Tapetes</asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbTapetes_NaoPossui" data-situacao="0" GroupName="Tapetes" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbTapetes_Bom" data-situacao="B" Checked="true" GroupName="Tapetes" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbTapetes_Ruim" data-situacao="R" GroupName="Tapetes" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                            </asp:TableRow>

                                        </asp:Table>
                                        <br />
                                        <asp:Table runat="server" ID="tbOutros_IncluirItem" CssClass="table table-bordered table-hover dataTable table-striped checklist">

                                            <asp:TableHeaderRow>
                                                <asp:TableHeaderCell RowSpan="2">OUTROS</asp:TableHeaderCell>
                                                <asp:TableHeaderCell ColumnSpan="3">Situação</asp:TableHeaderCell>
                                            </asp:TableHeaderRow>
                                            <asp:TableHeaderRow>
                                                <asp:TableHeaderCell>Não Possui</asp:TableHeaderCell>
                                                <asp:TableHeaderCell>Bons</asp:TableHeaderCell>
                                                <asp:TableHeaderCell>Ruins</asp:TableHeaderCell>
                                            </asp:TableHeaderRow>

                                            <asp:TableRow>
                                                <asp:TableCell>Bateria</asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbBateria_NaoPossui" data-situacao="0" GroupName="Bateria" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbBateria_Bom" data-situacao="B" Checked="true" GroupName="Bateria" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbBateria_Ruim" data-situacao="R" GroupName="Bateria" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell>Chaves</asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbChaves_NaoPossui" data-situacao="0" GroupName="Chaves" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbChaves_Bom" data-situacao="B" Checked="true" GroupName="Chaves" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbChaves_Ruim" data-situacao="R" GroupName="Chaves" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell>Documentos</asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbDocumentos_NaoPossui" data-situacao="0" GroupName="Documentos" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbDocumentos_Bom" data-situacao="B" Checked="true" GroupName="Documentos" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbDocumentos_Ruim" data-situacao="R" GroupName="Documentos" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell>Som</asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbSom_NaoPossui" data-situacao="0" GroupName="Som" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbSom_Bom" data-situacao="B" Checked="true" GroupName="Som" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbSom_Ruim" data-situacao="R" GroupName="Som" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell>Caixa Selada</asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbCaixaSelada_NaoPossui" data-situacao="0" GroupName="CaixaSelada" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbCaixaSelada_Bom" data-situacao="B" Checked="true" GroupName="CaixaSelada" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                                <asp:TableCell>
                                                    <asp:RadioButton runat="server" ID="rbCaixaSelada_Ruim" data-situacao="R" GroupName="CaixaSelada" />
                                                    <i class="fa fa-check"></i>
                                                </asp:TableCell>
                                            </asp:TableRow>

                                        </asp:Table>
                                    </div>

                                </div>

                                <div id="divEtapa_4" class="col-12">

                                    <div class="row">
                                        <div class="col-12 div-flex padd-0">

                                            <div runat="server" id="div_FotoFrente" class="col-6 form-group">
                                                <div class="card text-center">

                                                    <h3 class="card-title">Foto da Frente</h3>

                                                    <div id="divAdicionar_FotoFrente">
                                                        <uc1:Upload_Arquivo runat="server" ID="Upload_Arquivo_FotoFrente" />
                                                        <asp:FileUpload runat="server" ID="fuAdicionarArquivo_FotoFrente" CssClass="invisivel" accept=".jpg,.jpeg,.png,.webp" />
                                                    </div>

                                                    <div id="divVisualizar_FotoFrente">
                                                        <asp:Image runat="server" ID="imgFotoFrente" AlternateText="Foto da Frente" CssClass="card-img-top" />

                                                        <div class="card-footer">
                                                            <asp:LinkButton runat="server" ID="cmdAlterar_FotoFrente" CssClass="btn btn-lg btn-warning">Alterar Foto</asp:LinkButton>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>

                                            <div runat="server" id="div_FotoTraseira" class="col-6 form-group">
                                                <div class="card text-center">

                                                    <h3 class="card-title">Foto da traseira</h3>

                                                    <div id="divAdicionar_FotoTraseira">
                                                        <uc1:Upload_Arquivo runat="server" ID="Upload_Arquivo_FotoTraseira" />
                                                        <asp:FileUpload runat="server" ID="fuAdicionarArquivo_FotoTraseira" CssClass="invisivel" accept=".jpg,.jpeg,.png,.webp" />
                                                    </div>

                                                    <div id="divVisualizar_FotoTraseira">
                                                        <asp:Image runat="server" ID="imgFotoTraseira" AlternateText="Foto da Traseira" CssClass="card-img-top" />

                                                        <div class="card-footer">
                                                            <asp:LinkButton runat="server" ID="cmdAlterar_FotoTraseira" CssClass="btn btn-lg btn-warning">Alterar Foto</asp:LinkButton>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>

                                            <div runat="server" id="div_FotoLatDireita" class="col-6 form-group">
                                                <div class="card text-center">

                                                    <h3 class="card-title">Foto da Lateral Direita</h3>

                                                    <div id="divAdicionar_FotoLatDireita">
                                                        <uc1:Upload_Arquivo runat="server" ID="Upload_Arquivo_FotoLatDireita" />
                                                        <asp:FileUpload runat="server" ID="fuAdicionarArquivo_FotoLatDireita" CssClass="invisivel" accept=".jpg,.jpeg,.png,.webp" />
                                                    </div>

                                                    <div id="divVisualizar_FotoLatDireita">
                                                        <asp:Image runat="server" ID="imgFotoLatDireita" AlternateText="Foto da Lateral Direita" CssClass="card-img-top" />

                                                        <div class="card-footer">
                                                            <asp:LinkButton runat="server" ID="cmdAlterar_FotoLatDireita" CssClass="btn btn-lg btn-warning">Alterar Foto</asp:LinkButton>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>

                                            <div runat="server" id="div_FotoLatEsquerda" class="col-6 form-group">
                                                <div class="card text-center">

                                                    <h3 class="card-title">Foto da Lateral Esquerda</h3>

                                                    <div id="divAdicionar_FotoLatEsquerda">
                                                        <uc1:Upload_Arquivo runat="server" ID="Upload_Arquivo_FotoLatEsquerda" />
                                                        <asp:FileUpload runat="server" ID="fuAdicionarArquivo_FotoLatEsquerda" CssClass="invisivel" accept=".jpg,.jpeg,.png,.webp" />
                                                    </div>

                                                    <div id="divVisualizar_FotoLatEsquerda">
                                                        <asp:Image runat="server" ID="imgFotoLatEsquerda" AlternateText="Foto da Lateral Esquerda" CssClass="card-img-top" />

                                                        <div class="card-footer">
                                                            <asp:LinkButton runat="server" ID="cmdAlterar_FotoLatEsquerda" CssClass="btn btn-lg btn-warning">Alterar Foto</asp:LinkButton>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>

                                            <div runat="server" id="div_FotoPainel" class="col-6 form-group">
                                                <div class="card text-center">

                                                    <h3 class="card-title">Foto do Painel</h3>

                                                    <div id="divAdicionar_FotoPainel">
                                                        <uc1:Upload_Arquivo runat="server" ID="Upload_Arquivo_FotoPainel" />
                                                        <asp:FileUpload runat="server" ID="fuAdicionarArquivo_FotoPainel" CssClass="invisivel" accept=".jpg,.jpeg,.png,.webp" />
                                                    </div>

                                                    <div id="divVisualizar_FotoPainel">
                                                        <asp:Image runat="server" ID="imgFotoPainel" AlternateText="Foto do Painel" CssClass="card-img-top" />

                                                        <div class="card-footer">
                                                            <asp:LinkButton runat="server" ID="cmdAlterar_FotoPainel" CssClass="btn btn-lg btn-warning">Alterar Foto</asp:LinkButton>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>

                                        </div>
                                    </div>

                                    <div class="col-12">
                                        <div class='mensagemErro invisivel alert alert-danger alert-dismissible fade show' role='alert'>
                                            <p class="msg"></p>
                                            <button type="button" class="btn btn-close" onclick="$(this).closest('.mensagemErro').addClass('invisivel');" aria-label="Close"></button>
                                        </div>
                                    </div>

                                </div>

                            </div>

                        </div>
                        <div class="modal-footer d-flex justify-content-between">
                            <div>
                                <asp:LinkButton runat="server" ID="cmdAnterior" CssClass="btn btn-lg btn-secondary"><i class="fa fa-arrow-left"></i> Anterior</asp:LinkButton>
                            </div>

                            <div>
                                <asp:LinkButton runat="server" ID="cmdProximo" CssClass="btn btn-lg btn-success">Próximo <i class="fa fa-arrow-right"></i></asp:LinkButton>
                                <asp:Button runat="server" ID="cmdIncluirItem" CssClass="btn btn-lg btn-success" Text="Incluir Viagem" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="modal fade" id="modal_Imagem" tabindex="-1">
                <div class="modal-dialog modal-dialog-scrollable" style="min-width: fit-content; width: fit-content; max-width: 95vw;">
                    <div class="modal-content" style="width: fit-content; max-width: 95vw;">
                        <div class="modal-header">
                            <h2 class="modal-title" id="lblTitulo_modalImagem"></h2>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Fechar"></button>
                        </div>
                        <div class="modal-body">
                            <asp:Image runat="server" ID="imgArquivo" />
                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>