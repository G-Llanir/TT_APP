<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FiltroPesquisa.ascx.cs" Inherits="TT_Flow.App.Controles.FiltroPesquisa" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<uc1:MensagemPagina runat="server" ID="MensagemPagina" />

<div id="hdd">

    <asp:HiddenField runat="server" ID="hddTipoFiltro" Value="0" />

    <asp:HiddenField ID="FT_hddComposicao_idItem" runat="server" />
    <asp:HiddenField ID="FT_hddComposicao_idTipoProduto" runat="server" />
    <asp:HiddenField ID="FT_hddComposicao_sDscTipoProduto" runat="server" />
    <asp:HiddenField ID="FT_hddComposicao_nPreco" runat="server" />
    <asp:HiddenField ID="FT_hddComposicao_sUnidade" runat="server" />

    <asp:HiddenField ID="FT_hddComposicao_idServico_Recurso" runat="server" />
    <asp:HiddenField ID="FT_hddComposicao_idTipoServico_Recurso" runat="server" />
    <asp:HiddenField ID="FT_hddComposicao_sDscTipoServico_Recurso" runat="server" />
    <asp:HiddenField ID="FT_hddComposicao_sUnidadeServico_Recurso" runat="server" />

    <asp:HiddenField ID="FT_hddComposicao_idParceiro_Colaborador" runat="server" />
    <asp:HiddenField ID="FT_hddidParceiro_Produtos" runat="server" />
</div>

<div runat="server" id="div_FiltroProdutos">

    <a data-toggle="collapse" runat="server" id="filtroCollapseIcone" href="#idFiltroCollapse" role="button" aria-expanded="false" aria-controls="idFiltroCollapse1"><i class="fa fa-chevron-down"></i><span></span></a>

    <div class="collapse" runat="server" id="idFiltroCollapse">
        <div class="row">
            <div class="col-lg-4">
                <div class="form-group">
                    <label runat="server" id="lblTipo">Tipo de Produto</label>
                    <asp:DropDownList ID="FT_ddlidTipoProduto" class="form-control" runat="server"></asp:DropDownList>
                </div>
            </div>
            <div runat="server" id="div_Familia" class="col-lg-4">
                <div class="form-group">
                    <label>Familia</label>
                    <asp:DropDownList ID="FT_ddlidFamilia" class="form-control" runat="server"></asp:DropDownList>
                </div>
            </div>
            <div runat="server" id="div_Grupo" class="col-lg-2">
                <div class="form-group">
                    <label>Grupo</label>
                    <asp:DropDownList ID="FT_ddlidGrupo" class="form-control" runat="server"></asp:DropDownList>
                </div>
            </div>
            <div runat="server" id="divPais" class="col-lg-2">
                <div class="form-group">
                    <label>País de Origem</label>
                    <asp:DropDownList ID="FT_ddlidPaisOrigem" class="form-control" runat="server"></asp:DropDownList>
                </div>
            </div>
        </div>
    </div>

</div>

<asp:UpdatePanel ID="updpItensProduto" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="form-group row" runat="server" id="div_BuscarItens">
            <div class="col-lg-2" id="div_sCodigoProduto" runat="server">
                <div class="form-group">
                    <label runat="server" id="lblCodigo">Código Produto</label>
                    <asp:TextBox ID="FT_txtComposicao_sCodigoProduto" class="form-control" runat="server" MaxLength="50" oninput="this.value = this.value.toUpperCase();"></asp:TextBox>
                </div>
            </div>

            <div class="col-lg-4" id="div_sDscProduto" runat="server">
                <div class="form-group">
                    <label runat="server" id="lblDesc">Descrição Produto </label>
                    <asp:TextBox ID="FT_txtComposicao_sDscProduto" class="form-control" runat="server" MaxLength="200" ValidationGroup="Produto"></asp:TextBox>
                </div>
            </div>
            <div class="col-lg-2" id="div_sUnidade" runat="server">
                <div class="form-group">
                    <label>Unidade</label>
                    <asp:DropDownList ID="FT_ddlComposicao_sUnidade" runat="server" class="form-control" attrname="Unidade" ValidationGroup="Item"></asp:DropDownList>
                </div>
            </div>
            <div class="col-lg-2" id="div_Qtde" runat="server">
                <div class="form-group">
                    <label>Quantidade</label>
                    <asp:TextBox ID="FT_txtnQuantidade" class="form-control" runat="server" MaxLength="20"></asp:TextBox>
                </div>
            </div>
            <div class="col-lg-2" id="div_nValor" runat="server">
                <div class="form-group">
                    <label runat="server" id="lblValor">Valor Unitário</label>
                    <asp:TextBox ID="FT_txtnValor" class="form-control" runat="server" MaxLength="20"></asp:TextBox>
                </div>
            </div>
        </div>

    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="FT_txtComposicao_sCodigoProduto" />
    </Triggers>
</asp:UpdatePanel>

<asp:UpdatePanel ID="UpdServico_Recurso" runat="server">
    <ContentTemplate>
        <div class="form-group row" runat="server" id="div_BuscarServicos_Recurso">
            <div class="col-lg-3" id="div_sCodigoServico_Recurso" runat="server">
                <div class="form-group">
                    <label runat="server" id="lblCodigoServico_Recurso">Código</label>
                    <asp:TextBox ID="FT_txtComposicao_sCodigoServico_Recurso" class="form-control" runat="server" MaxLength="50"></asp:TextBox>
                </div>
            </div>

            <div class="col-lg-5" id="div_sDscServico_Recurso" runat="server">
                <div class="form-group">
                    <label runat="server" id="lblDescServico_Recurso">Descrição</label>
                    <asp:TextBox ID="FT_txtComposicao_sDscServico_Recurso" class="form-control" runat="server" MaxLength="200" ValidationGroup="Servico_Recurso"></asp:TextBox>
                </div>
            </div>
            <div class="col-lg-2" id="div_QtdeServico_Recurso" runat="server">
                <div class="form-group">
                    <label>Quantidade</label>
                    <asp:TextBox ID="FT_txtnQuantidadeServico_Recurso" class="form-control" runat="server" MaxLength="20"></asp:TextBox>
                </div>
            </div>
            <div class="col-lg-2" id="div_nValorServico_Recurso" runat="server">
                <div class="form-group">
                    <label runat="server" id="lblValorServico_Recurso">Valor Unitário</label>
                    <asp:TextBox ID="FT_txtnValorServico_Recurso" class="form-control" runat="server" MaxLength="20"></asp:TextBox>
                </div>
            </div>
        </div>

    </ContentTemplate>
</asp:UpdatePanel>

<asp:UpdatePanel ID="UpdParceiro_Colaborador" runat="server">
    <ContentTemplate>

        <div class="form-group row" runat="server" id="div_BuscarParceiro_Colaborador">
            <div class="col-lg-3" id="div_sCNPJParceiro_sCPFColaborador" runat="server">
                <div class="form-group">
                    <label runat="server" id="lblCNPJ_CPFColaborador">CNPJ Parceiro</label>
                    <asp:TextBox ID="FT_txtComposicao_sCNPJarceiro_sCPFColaborador" class="form-control" runat="server" MaxLength="50"></asp:TextBox>
                </div>
            </div>

            <div class="col-lg-9" id="div_sDscParceiro_Colaborador" runat="server">
                <div class="form-group">
                    <label runat="server" id="lblDescParceiro_Colaborador">Razão Social Parceiro</label>
                    <asp:TextBox ID="FT_txtComposicao_sDscParceiro_Colaborador" class="form-control" runat="server" MaxLength="200" ValidationGroup="Parceiro_Colaborador"></asp:TextBox>
                </div>
            </div>
        </div>

    </ContentTemplate>
</asp:UpdatePanel>
