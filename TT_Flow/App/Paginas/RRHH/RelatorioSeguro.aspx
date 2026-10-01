<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/App/main.master" CodeBehind="RelatorioSeguro.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.RelatorioSeguro" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="content_frmRelatorioSeguro" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Relatório Seguro"></asp:Label>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">
            <div id="div_RelatorioSeguroConsulta" runat="server">
                <div class="panel panel-primary" id="div_filtroPesquisa" runat="server">
                    <div class="panel-heading">
                        <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;Filtro para pesquisa/Gerar Novo Relatório </h3>

                    </div>
                    <div class="panel-body ">

                        <div class="col-lg-12">
                            <div class="form-group">
                                <uc1:MensagemPagina runat="server" ID="MensagemPagina_Filtro" />
                            </div>
                        </div>

                        <div class="row form-group col-lg-12">


                            <div class="col-lg-1">
                                <div class="form-group">
                                    <label>Data Início</label>
                                    <asp:TextBox ID="txtdtInicio" class="form-control" runat="server" TextMode="Date" Style="inline-size: 120px;"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-1">
                                <div class="form-group">
                                    <label>Data Final</label>
                                    <asp:TextBox ID="txtdtFinal" class="form-control" runat="server" TextMode="Date" Style="inline-size: 120px;"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <label>Seguradora</label>
                                    <asp:DropDownList runat="server" ID="ddlsSeguradora" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-3">
                                <div class="form-group">
                                    <label>Empresa</label>
                                    <asp:DropDownList runat="server" ID="ddlsEmpresa" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-3">
                                <div class="form-group" style="margin-top: 24px;">
                                    <asp:Button ID="btnPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="btnPesquisar_Click" />
                                    <asp:Button ID="btnNovoRelatorio" class="btn btn-success" runat="server" Text="Novo Relatório Seguro" OnClick="btnNovoRelatorio_Click" />
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
                                    <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover " DataKeyNames="idRelatorioSeguro"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">

                                        <Columns>

                                            <asp:BoundField DataField="idRelatorioSeguro" HeaderText="ID">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtInicio" HeaderText="Data Início">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtFinal" HeaderText="Data Final">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Seguradora">
                                                <HeaderStyle Width="20%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnSeguradora" runat="server"
                                                        Text='<%# Eval("sSeguradora") %>'
                                                        CommandArgument='<%# Eval("idRelatorioSeguro") %>'
                                                        OnCommand="lbDetalheRelatorio_Command">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Empresa">
                                                <HeaderStyle Width="20%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnEmpresa" runat="server"
                                                        Text='<%# Eval("sEmpresa") %>'
                                                        CommandArgument='<%# Eval("idRelatorioSeguro") %>'
                                                        OnCommand="lbDetalheRelatorio_Command">
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="sStatus" HeaderText="Status">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscUsuario" HeaderText="Por">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtCriaRelatorio" HeaderText="Em">
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

            <div id="div_RelatorioSeguroDetalhe" runat="server">
                <asp:UpdatePanel runat="server" ID="upd_DetalheRelatorioSeguro">
                    <ContentTemplate>
                        <div>
                            <ul id="tab_RelatorioDetalhe" class="nav nav-tabs" role="tablist">
                                <li role="presentation" class="tabpanel active">
                                    <a href="#detalheRelatorioSeguro" id="relatorioDetalhe-tab" role="tab" data-toggle="tab" aria-controls="Relatorio" aria-expanded="false"><b>Relatório</b></a>
                                </li>

                                <li role="presentation" runat="server" id="aba_Arquivo">
                                    <a href="#arquivoRelatorioSeguro" role="tab" id="arquivo-tab" data-toggle="tab" aria-controls="Arquivo"><b>Documentos</b></a>
                                </li>

                            </ul>
                        </div>
                        <div id="tab" class="tab-content">
                            <div role="tabpanel" class="tab-pane fade in active" id="detalheRelatorioSeguro" aria-labelledby="Relatorio">
                                <br />

                                <uc1:MensagemPagina runat="server" ID="MensagemPaginaDetalhe" />

                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        <h3 class="panel-title"><b>&nbsp;Dados Seguradora</b></h3>
                                    </div>
                                    <div class="panel-body">

                                        <div class="form-group">
                                            <div class="col-lg-12">
                                                <div class="row">
                                                    <div class="col-lg-1">
                                                        <div class="form-group">
                                                            <label>ID</label>
                                                            <asp:TextBox ID="txtIdRelatorioSeguro" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
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
                                                            <label>Período</label>
                                                            <asp:TextBox runat="server" ID="txtsPeriodo" class="form-control" ReadOnly="true"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-3">
                                                        <div class="form-group">
                                                            <label>Empresa</label>
                                                            <asp:TextBox runat="server" ID="txtsEmpresa" class="form-control" ReadOnly="true"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2">
                                                        <div class="form-group">
                                                            <label>Nº Apólice V.G.</label>
                                                            <asp:TextBox ID="txtnApoliceVG" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2">
                                                        <div class="form-group">
                                                            <label>Nº Apólice APC</label>
                                                            <asp:TextBox ID="txtnApoliceAPC" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <div class="col-lg-12">
                                                <div class="row">

                                                    <div class="col-lg-3">
                                                        <div class="form-group">
                                                            <label>Seguradora</label>
                                                            <asp:TextBox runat="server" ID="txtsSeguradora" class="form-control" ReadOnly="true"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-4">
                                                        <div class="form-group">
                                                            <label>Endereço</label>
                                                            <asp:TextBox ID="txtsEndereco" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2">
                                                        <div class="form-group">
                                                            <label>Contato</label>
                                                            <asp:TextBox ID="txtsContato" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>

                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        <h3 class="panel-title"><b>&nbsp;Tabela Relatório Seguro</b></h3>
                                    </div>
                                    <div class="panel-body">

                                        <div class="col-lg-12 form-group table-responsive" style="margin-top: 30px">
                                            <asp:GridView ID="gvRelatorioSeguro" class="table table-striped table-bordered table-hover"
                                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">

                                                <Columns>

                                                    <asp:BoundField DataField="idColaborador" HeaderText="ID">
                                                        <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sDscColaborador" HeaderText="Colaborador">
                                                        <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sNomeSocial" HeaderText="Nome Social">
                                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    
                                                    <asp:BoundField DataField="sCPF" HeaderText="CPF">
                                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    
                                                    <asp:BoundField DataField="dtNascimento" HeaderText="Data Nascimento">
                                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    
                                                    <asp:BoundField DataField="sFuncaoCarteira" HeaderText="Função Carteira">
                                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    
                                                    <asp:BoundField DataField="nSalario" HeaderText="Salário" DataFormatString="{0:N2}">
                                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    
                                                    <asp:BoundField DataField="sSexo" HeaderText="Sexo">
                                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    
                                                    <asp:BoundField DataField="sEstadoCivil" HeaderText="Estado Civil">
                                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sEmpresa" HeaderText="Empresa">
                                                        <ItemStyle Width="4%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="dtInicioSeguro" HeaderText="Início Contrato">
                                                        <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sTipoPlano" HeaderText="Tipo">
                                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="nValorNatural" HeaderText="Valor Natural" DataFormatString="{0:N2}">
                                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="nValorAcidental" HeaderText="Valor Acidental" DataFormatString="{0:N2}">
                                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="nValorInvalidez" HeaderText="Valor Invalidez" DataFormatString="{0:N2}">
                                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="nVG" HeaderText="V.G." DataFormatString="{0:N5}">
                                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="nAPC" HeaderText="APC" DataFormatString="{0:N5}">
                                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="nVG_APC" HeaderText="V.G. + APC" DataFormatString="{0:N5}">
                                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                </Columns>
                                            </asp:GridView>
                                        </div>

                                        <div class="form-group">
                                            <div class="col-lg-12">
                                                <div class="row">

                                                    <div class="col-lg-2">
                                                        <div class="form-group">
                                                            <label>Total V.G.</label>
                                                            <div class="input-group">
                                                                <span class="input-group-addon">R$</span>
                                                                <asp:TextBox runat="server" ID="txtnTotalVG" class="form-control" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2">
                                                        <div class="form-group">
                                                            <label>Total APC</label>
                                                            <div class="input-group">
                                                                <span class="input-group-addon">R$</span>
                                                                <asp:TextBox runat="server" ID="txtnTotalAPC" class="form-control" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2">
                                                        <div class="form-group">
                                                            <label>Total V.G. + APC</label>
                                                            <div class="input-group">
                                                                <span class="input-group-addon">R$</span>
                                                                <asp:TextBox ID="txtnTotalVgApc" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                                <div class="col-lg-12" style="margin-bottom: 15px; padding: 0px">
                                    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" Visible="false" />
                                </div>

                                <fieldset class="form-stacked actions">
                                    <div class="col-lg-12">
                                        <div class="form-group">
                                            <asp:Button ID="btnGerarRelatorio" class="btn btn-lg btn-success" runat="server" Text="Gerar Relatório" OnClick="btnGerarRelatorio_Click" />
                                            <asp:Button ID="btnGerarContasPagar" class="btn btn-lg btn-info" runat="server" Text="Gerar Contas a Pagar" OnClick="btnGerarContasPagar_Click" Visible="false" />
                                            <asp:Button ID="btnExcluirRelatorio" class="btn btn-lg btn-danger" runat="server" Text="Excluir" />
                                            <asp:HyperLink ID="lnkExcelRelatorio" runat="server" CssClass="btn btn-lg btn-primary" Target="_blank" NavigateUrl="~/App/Paginas/RRHH/RelatorioSeguro.aspx?action=export" Visible="false"><i class="fa fa-file-excel-o"></i> Relatório Excel</asp:HyperLink>
                                            <asp:Button ID="btnVoltar" CssClass="btn-warning btn-lg btn" runat="server" Text="Voltar" OnClick="btnVoltar_Click" />
                                        </div>
                                    </div>
                                </fieldset>

                            </div>

                            <%--------------------------------------Tab-Arquivo----------------------------------------------------------------------------------------------------------------------------------------------------------------%>
                            <div role="tabpanel" class="tab-pane fade" id="arquivoRelatorioSeguro" aria-labelledby="Arquivo">
                                <div class=" embed-responsive embed-responsive-16by9" style="min-height: 800px" runat="server" id="DIV_Arquivos">
                                    <embed type="text/html" runat="server" id="frmArquivos" width="800" height="500" />
                                </div>
                            </div>
                            <%--------------------------------------------Fim-Tab-Arquivo--------------------------------------------------------------------------------------------------------------------------------------------------------------%>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <asp:HiddenField ID="hddidRelatorioSeguro" Value="" runat="server" />

        </div>
    </div>
    

    <%--------------------------------------MODAL GERAR CONTAS A PAGAR---------------------------------------------------------------------------------------%>
    <div class="modal fade" id="modalContasPagarDetalhe" tabindex="-2" role="dialog" data-backdrop="static" aria-labelledby="modalLancamentoDetalhe" aria-hidden="true">
        <div class="modal-dialog modal-sm" role="document" style="width: 60%">
            <div class="modal-content" style="overflow-y: visible !important;">
                <asp:UpdatePanel ID="updModalDetalhe" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" id="btnHeadFecharDetalhe" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <div class="modal-header-content">
                                <div class="modal-logo">
                                    <asp:Image ID="Image2" runat="server" ImageUrl="~/App/img/instrucaoTecnica_pdf.jpg" AlternateText="Instrução Técnica" />
                                </div>

                                <div class="modal-title-container">
                                    <h4 class="modal-title">Criar Contas a Pagar</h4>
                                </div>
                            </div>
                            <div class="modal-body">
                                <div class="panel panel-default" runat="server" id="div_NovaConciliacao">
                                    <div class="panel-body ">

                                        <div class="col-lg-12 row" style="width: 100%; margin-left: 0px; padding: 0px;">
                                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                <ContentTemplate>
                                                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaModalDetalhe" style="margin-left: 0px !important" />
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>

                                        <div class="form-group">
                                            <div class="col-lg-12">
                                                <div class="row">
                                                    <div class="col-lg-6" id="div_ddlCategoriaPagar" runat="server">
                                                        <div class="form-group">
                                                            <label>Categoria</label>
                                                            <asp:DropDownList ID="ddlidCategoriaPagar" class="form-control Caixa_Selecao" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidCategoriaPagar_OnSelectedIndexChanged"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="div_camposDetalhe" runat="server">
                                            <div class="form-group">
                                                <div class="col-lg-12">
                                                    <div class="row">

                                                        <div class="col-lg-3" id="div_ddlidParceiro" runat="server">
                                                            <div class="form-group">
                                                                <label>Credor</label>
                                                                <asp:DropDownList ID="ddlidParceiro" class="form-control Caixa_Selecao" runat="server" AutoPostBack="true"></asp:DropDownList>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3" id="div_ddlEmpresa" runat="server">
                                                            <div class="form-group">
                                                                <label>Empresa</label>
                                                                <asp:DropDownList ID="ddlidEmpresa" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <label>Valor Bruto</label>
                                                                <div class="input-group">
                                                                    <span class="input-group-addon" id="Span4" runat="server">R$</span>
                                                                    <asp:TextBox ID="txtnValorBruto" runat="server" CssClass="valor-input form-control" aria-describedby="sMoedaOrigem" Style="z-index: 0;"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <label>Valor Líquido</label>
                                                                <div class="input-group">
                                                                    <span class="input-group-addon" id="Span5" runat="server">R$</span>
                                                                    <asp:TextBox ID="txtnValorLiquido" runat="server" CssClass="valor-input form-control" aria-describedby="sMoedaOrigem" Style="z-index: 0;"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>

                                                    </div>
                                                </div>
                                            </div>

                                            <div class="form-group">
                                                <div class="col-lg-12" id="div_TipoCategoriaPagar" runat="server">
                                                    <div class="row">

                                                        <div class="col-lg-3" id="div_dtEmissao" runat="server">
                                                            <div class="form-group">
                                                                <label>Data Emissão</label>
                                                                <asp:TextBox ID="txtdtEmissao" type="date" class="form-control" runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3" id="div_txtdtApuracao" runat="server">
                                                            <div class="form-group">
                                                                <label>Data Apuração</label>
                                                                <asp:TextBox ID="txtdtApuracao" type="date" class="form-control" runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <label>Data Vencimento</label>
                                                                <asp:TextBox runat="server" type="date" class="form-control" ID="txtdtVencimento"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <asp:Label runat="server" ID="lblFormaPagamento" Text="Forma de Pagamento" Font-Bold="true"></asp:Label>
                                                                <label>&nbsp;</label>
                                                                <asp:DropDownList ID="ddlidFormaPagamento" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                                            </div>
                                                        </div>

                                                    </div>
                                                </div>
                                            </div>

                                            <div class="form-group">
                                                <div class="col-lg-12">
                                                    <div class="row">

                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <label>Referência</label>
                                                                <asp:TextBox ID="txtsDocumento" class="form-control" runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3" id="div_ddlidCentroCusto" runat="server">
                                                            <div class="form-group">
                                                                <label>Centro de Custo</label>
                                                                <asp:DropDownList ID="ddlidCentroDeCusto" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <asp:Label runat="server" ID="lblsCodigo" Text="Nota Fiscal" Font-Bold="true"></asp:Label>
                                                                <label>&nbsp;</label>
                                                                <asp:TextBox ID="txtsCodigo" class="form-control" runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <asp:Label runat="server" ID="lblMeioPagamento" Text="Tipo de Pagamento" Font-Bold="true"></asp:Label>
                                                                <label>&nbsp;</label>
                                                                <asp:DropDownList ID="ddlidMeioPagamento" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                                            </div>
                                                        </div>

                                                    </div>
                                                </div>
                                            </div>


                                            <div class="form-group">
                                                <div class="col-lg-12">
                                                    <div class="row">

                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <label>Código Contábil</label>
                                                                <asp:DropDownList ID="ddlidContabil" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-9">
                                                            <div class="form-group">
                                                                <label>Observação</label>
                                                                <asp:TextBox ID="txtsObservacao" class="form-control" runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <asp:Button ID="btnSalvarModal" CssClass="btn-success btn" runat="server" Text="Salvar" OnClick="btnSalvarModal_Click" />
                                <asp:Button ID="btnFecharFooterDetalhe" runat="server" class="btn btn-secondary" Text="Fechar" OnClick="btnFecharFooterDetalhe_Click"></asp:Button>
                            </div>

                            <asp:HiddenField runat="server" ID="hddTipoCategoria" Value="" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <asp:HiddenField runat="server" ID="hddidTitulo" Value="" />

    <div id="dialog-Excluir" class="modal" title="Excluir">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloExcluir" runat="server" Text=""></asp:Label>
        </p>
    </div>
</asp:Content>
