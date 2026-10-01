<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Regras.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Fiscal.Regras" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        .input-group {
            z-index: 0;
        }

        .CaixaTextoPequeno {
            width: 100% !important;
        }
    </style>

    <asp:UpdatePanel ID="updDetalhe" runat="server">
        <ContentTemplate>
            <div class="row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Empresas"></asp:Label><small>
                            <asp:Label ID="lblSubTituloPagina" runat="server" Text="Consulta"></asp:Label></small></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>

                <div class="col-lg-12">

                    <asp:Panel runat="server" ID="pnPesquisa">
                        <div class="panel panel-primary">
                            <div class="panel-heading">
                                <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa</h3>
                            </div>
                            <div class="panel-body">

                                <div class="row">

                                    <div class="col-lg-3 form-group">
                                        <asp:TextBox ID="txtsPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                    </div>

                                    <div class="col-lg-3">
                                        <asp:DropDownList ID="ddlEmpresa" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                    </div>

                                    <div class="col-lg-2">
                                        <asp:DropDownList ID="ddlPesquisa_sTipoRegra" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                    </div>

                                    <div class="col-lg-2">
                                        <asp:DropDownList ID="ddlPesquisa_sUFOrigem" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                    </div>

                                    <div class="col-lg-2">
                                        <asp:DropDownList ID="ddlPesquisa_sUFDestino" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                    </div>

                                </div>

                                <div class="row">

                                    <div class="col-lg-2">
                                        <asp:DropDownList ID="ddlPesquisa_sSituacao" runat="server" class="form-control Caixa_Selecao">
                                            <asp:ListItem Value="T">Todas as Situações</asp:ListItem>
                                            <asp:ListItem Value="S" Selected="True">Ativas</asp:ListItem>
                                            <asp:ListItem Value="N">Invativas</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>

                                    <div class="col-lg-10">
                                        <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao_Pesquisa(this)" />
                                        <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Nova Regra" OnClick="cmdNovo_Click" />
                                        <asp:Button ID="cmdSimular" class="btn btn-info" runat="server" Text="Simular" />
                                    </div>

                                </div>
                            </div>
                        </div>
                    </asp:Panel>

                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                    <asp:Panel ID="pnResultado" class="" runat="server">
                        <div id="resultado">
                            <div class="panel panel-primary">
                                <div class="panel-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">
                                            <Columns>

                                                <asp:HyperLinkField DataNavigateUrlFields="idRegra"
                                                    DataTextField="idRegra" HeaderText="ID"
                                                    DataNavigateUrlFormatString="Regras.aspx?id={0}">
                                                    <HeaderStyle Width="5%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idRegra"
                                                    DataTextField="nOrdem" HeaderText="Peso"
                                                    DataNavigateUrlFormatString="Regras.aspx?id={0}">
                                                    <HeaderStyle Width="5%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idRegra"
                                                    DataTextField="sDscRegra" HeaderText="Descrição"
                                                    DataNavigateUrlFormatString="Regras.aspx?id={0}">
                                                    <HeaderStyle Width="30%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idRegra"
                                                    DataTextField="dtInclusao" HeaderText="Data Inclusão"
                                                    DataNavigateUrlFormatString="Regras.aspx?id={0}">
                                                    <HeaderStyle Width="18%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:BoundField DataField="sDscTipo" HeaderText="Tipo">
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDscEmpresa" HeaderText="Empresa">
                                                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sUFOrigem" HeaderText="UF Origem">
                                                    <HeaderStyle HorizontalAlign="center" VerticalAlign="Middle" />
                                                    <ItemStyle Width="5%" HorizontalAlign="center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sUFDestino" HeaderText="UF Destino">
                                                    <HeaderStyle HorizontalAlign="center" VerticalAlign="Middle" />
                                                    <ItemStyle Width="5%" HorizontalAlign="center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDscContribuinte" HeaderText="Contribuinte">
                                                    <HeaderStyle HorizontalAlign="center" VerticalAlign="Middle" />
                                                    <ItemStyle Width="6%" HorizontalAlign="center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDscTemIE" HeaderText="Tem IE">
                                                    <HeaderStyle HorizontalAlign="center" VerticalAlign="Middle" />
                                                    <ItemStyle Width="6%" HorizontalAlign="center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDscProdutoIndustrializado" HeaderText="Industrializado">
                                                    <HeaderStyle HorizontalAlign="center" VerticalAlign="Middle" />
                                                    <ItemStyle Width="6%" HorizontalAlign="center" VerticalAlign="Middle" />
                                                </asp:BoundField>


                                                <asp:BoundField DataField="sDscsProdutoImportado" HeaderText="Importado">
                                                    <HeaderStyle HorizontalAlign="center" VerticalAlign="Middle" />
                                                    <ItemStyle Width="6%" HorizontalAlign="center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDscSituacao" HeaderText="Situação">
                                                    <HeaderStyle HorizontalAlign="center" VerticalAlign="Middle" />
                                                    <ItemStyle Width="6%" HorizontalAlign="center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sSUFRAMA" HeaderText="SUFRAMA">
                                                    <ItemStyle Width="3%" HorizontalAlign="center" VerticalAlign="Middle" />
                                                </asp:BoundField>


                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pnDetalhe" class="" runat="server">

                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_Detalhe" />

                        <div class="panel panel-default" runat="server" id="div_Parametros">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Parâmetros</b></h3>
                            </div>
                            <div class="panel-body">
                                <div class="form-stacked row">

                                    <div class="col-lg-12">
                                        <div class="col-lg-1 form-group">
                                            <label>ID</label>
                                            <asp:TextBox ID="txtidRegra" class="form-control" runat="server" disabled=""></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-12">
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <label>Descrição</label>
                                                <asp:TextBox ID="txtsDscRegra" class="form-control" runat="server" MaxLength="200"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-12">
                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label>Empresa</label>
                                                <asp:DropDownList ID="ddlidEmpresa" runat="server" class="form-control Caixa_Selecao" OnSelectedIndexChanged="ddlidEmpresa_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>Tipo Regra</label>
                                                <asp:DropDownList ID="ddlidTipo" runat="server" class="form-control Caixa_Selecao" OnSelectedIndexChanged="ddlidTipo_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>UF Origem</label>
                                                <asp:DropDownList ID="ddlsUFOrigem" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>UF Destino</label>
                                                <asp:DropDownList ID="ddlsUFDestino" runat="server" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlsUFDestino_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="DIV_idCidadeDestino">
                                            <div class="form-group">
                                                <label>Município Destino</label>
                                                <asp:DropDownList ID="ddlidCidadeDestino" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                            </div>
                                        </div>

                                    </div>

                                    <div class="col-lg-12">

                                        <div class="col-lg-2 form-group">
                                            <label>Apenas Contribuinte?</label>
                                            <asp:DropDownList ID="ddlsContribuinte" runat="server" class="form-control"></asp:DropDownList>
                                        </div>

                                        <div class="col-lg-2 form-group">
                                            <label>Apenas com IE?</label>
                                            <asp:DropDownList ID="ddlsTemIE" runat="server" class="form-control"></asp:DropDownList>
                                        </div>

                                        <div class="col-lg-2 form-group" runat="server" id="DIV_ProdutoIndustrializado">
                                            <label>Apenas Produto Industrializado?</label>
                                            <asp:DropDownList ID="ddlsProdutoIndustrializado" runat="server" class="form-control"></asp:DropDownList>
                                        </div>

                                        <div class="col-lg-2 form-group" runat="server" id="DIV_ProdutoImportado">
                                            <label>Apenas Produto Importado?</label>
                                            <asp:DropDownList ID="ddlsProdutoImportado" runat="server" class="form-control"></asp:DropDownList>
                                        </div>

                                        <div class="col-lg-2 form-group">
                                            <label>Apenas SUFRAMA?</label>
                                            <asp:DropDownList ID="ddlSuframa" runat="server" class="form-control">
                                                <asp:ListItem Value="S">Sim</asp:ListItem>
                                                <asp:ListItem Value="N">Não</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-lg-1 form-group">
                                            <label>Peso</label>
                                            <asp:DropDownList ID="ddlnOrdem" runat="server" class="form-control">
                                                <asp:ListItem Value="5">5</asp:ListItem>
                                                <asp:ListItem Value="4">4</asp:ListItem>
                                                <asp:ListItem Value="3">3</asp:ListItem>
                                                <asp:ListItem Value="2">2</asp:ListItem>
                                                <asp:ListItem Value="1">1</asp:ListItem>
                                                <asp:ListItem Value="0" Selected="True">0</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-lg-1">
                                            <uc1:ComboAtivo runat="server" ID="ddlsSituacao" />
                                        </div>

                                    </div>

                                </div>
                            </div>
                        </div>

                        <div class="panel panel-default" runat="server" id="DIV_Valores">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Valores</b></h3>
                            </div>
                            <div class="panel-body">
                                <div class="form-stacked row">
                                    <div runat="server" id="DIV_Valores_Produtos">
                                        <div class="col-lg-12">
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Tributa ICMS?</label>
                                                    <asp:DropDownList ID="ddlsTributaICMS" runat="server" class="form-control">
                                                        <asp:ListItem Value="S">Sim</asp:ListItem>
                                                        <asp:ListItem Value="N">Não</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Base de Calculo ICMS</label>
                                                    <asp:DropDownList ID="ddlsBaseCalculo" runat="server" class="form-control">
                                                        <asp:ListItem Value="P">Produto</asp:ListItem>
                                                        <asp:ListItem Value="PI">Produto + IPI</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>


                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Aplica IPI?</label>
                                                    <asp:DropDownList ID="ddlsCalculoIPI" runat="server" class="form-control">
                                                        <asp:ListItem Value="S">Sim (NCM)</asp:ListItem>
                                                        <asp:ListItem Value="N">Não</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Aplica DIFAL?</label>
                                                    <asp:DropDownList ID="ddlsCalculoDIFAL" runat="server" class="form-control">
                                                        <asp:ListItem Value="S">Sim</asp:ListItem>
                                                        <asp:ListItem Value="N">Não</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Aplica ICMS ST?</label>
                                                    <asp:DropDownList ID="ddlsICMSST" runat="server" class="form-control">
                                                        <asp:ListItem Value="S">Sim</asp:ListItem>
                                                        <asp:ListItem Value="N">Não</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                    <div class="col-lg-12">
                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>CFOP</label>
                                                <div class="input-group">
                                                    <span class="input-group-addon">Com ICMS ST</span>
                                                    <asp:TextBox ID="txtsCFOP_ST" class="form-control" runat="server" MaxLength="12"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div runat="server" id="DIV_Valores_Produtos2">
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>CFOP</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon">Sem ICMS ST</span>
                                                        <asp:TextBox ID="txtsCFOP" class="form-control" runat="server" MaxLength="12"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div runat="server" id="DIV_Valores_Servicos">
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Código Municipal</label>
                                                    <asp:TextBox ID="txtsCodigoMunicipal" class="form-control" runat="server" MaxLength="12"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Código Federal</label>
                                                    <asp:TextBox ID="txtsCodigoFederal" class="form-control" runat="server" MaxLength="12"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>ISS</label>
                                                    <div class="input-group">
                                                        <asp:TextBox ID="txtnISS" class="form-control" runat="server" MaxLength="12"></asp:TextBox>
                                                        <span class="input-group-addon">%</span>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>CSSL</label>
                                                    <div class="input-group">
                                                        <asp:TextBox ID="txtnCSSL" class="form-control" runat="server" MaxLength="12"></asp:TextBox>
                                                        <span class="input-group-addon">%</span>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>IR</label>
                                                    <div class="input-group">
                                                        <asp:TextBox ID="txtnIR" class="form-control" runat="server" MaxLength="12"></asp:TextBox>
                                                        <span class="input-group-addon">%</span>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>INSS</label>
                                                    <div class="input-group">
                                                        <asp:TextBox ID="txtnINSS" class="form-control" runat="server" MaxLength="12"></asp:TextBox>
                                                        <span class="input-group-addon">%</span>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>PIS</label>
                                                <div class="input-group">
                                                    <asp:TextBox ID="txtnPIS" class="form-control" runat="server" MaxLength="12"></asp:TextBox>
                                                    <span class="input-group-addon">%</span>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>COFINS</label>
                                                <div class="input-group">
                                                    <asp:TextBox ID="txtnCofins" class="form-control" runat="server" MaxLength="12"></asp:TextBox>
                                                    <span class="input-group-addon">%</span>
                                                </div>
                                            </div>
                                        </div>


                                    </div>
                                </div>

                            </div>

                        </div>

                        <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

                        <fieldset class="form-stacked actions">
                            <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                            <asp:Button ID="cmdVoltar" class="btn  btn-lg btn-warning" runat="server" Text="Voltar" OnClick="cmdVoltar_Click" />
                        </fieldset>

                    </asp:Panel>

                </div>

            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
