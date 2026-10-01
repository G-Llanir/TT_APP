<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Controle_Aprovacao_Pag.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Financeiro.Controle_Aprovacao_Pag" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>

<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>


    <script type="text/javascript">
        // Função chamada ao confirmar aprovação
        function confirmAprovar() {
            var obs = $('#txtObsAprovar').val();
            __doPostBack('BtnAprovar', obs);
            $('#modalAprovar').modal('hide');
        }

        // Função chamada ao confirmar rejeição
        function confirmRejeitar() {
            var obs = $('#txtObsRejeitar').val();
            __doPostBack('BtnRejeitar', obs);
            $('#modalRejeitar').modal('hide');
        }


    </script>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Controle Aprovação Pagamentos"></asp:Label><small> Consulta</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12" runat="server" id="DIV_Filtro">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa </h3>
                </div>
                <div class="panel-body ">
                    <div class="form-group">

                        <div class="col-lg-4">
                            <div class="row">
                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtdtInicio" class="form-control CaixaTextoData" placeholder="Emissão" runat="server" MaxLength="10" type="date"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtdtFinal" class="form-control CaixaTextoData " runat="server" placeholder="Vencimento" MaxLength="10" type="date"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-6">
                                    <asp:DropDownList ID="ddlidDataPesquisa" runat="server" class="form-control yes_no select Caixa_Selecao" attrname="idContasPagar">
                                        <asp:ListItem Selected="True" Value="0">Vencimento</asp:ListItem>
                                        <asp:ListItem Value="1">Emissão</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlidEmpresa" class="form-control Caixa_Selecao" runat="server" attrname="idContasPagar"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlidCredor" class="form-control Caixa_Selecao" runat="server" attrname="idContasPagar"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlidCategoriaPagar" class="form-control Caixa_Selecao" runat="server" attrname="idContasPagar"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlidFormaPagamento" class="form-control Caixa_Selecao" runat="server" attrname="idContasPagar"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlidCentroDeCusto" class="form-control Caixa_Selecao" runat="server" attrname="idContasPagar"></asp:DropDownList>
                            </div>
                        </div>


                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlsStatus" runat="server" class="form-control yes_no select Caixa_Selecao" attrname="idContasPagar">
                                    <asp:ListItem Value="">Todos os Status</asp:ListItem>
                                    <asp:ListItem Value="Liquidado">Liquidado</asp:ListItem>
                                    <asp:ListItem Selected="True" Value="Em Aberto">Em Aberto</asp:ListItem>
                                    <asp:ListItem Value="Em Atraso">Em Atraso</asp:ListItem>
                                    <asp:ListItem Value="Vencendo Hoje">Vencendo Hoje</asp:ListItem>
                                    <asp:ListItem Value="A vencer em 7 dias">A vencer em 7 dias</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>


                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlidAprovado" class="form-control Caixa_Selecao" runat="server" attrname="idContasPagar"></asp:DropDownList>
                            </div>
                        </div>


                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-2">
                            <div class="form-group">
                                <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                            </div>
                        </div>

                    </div>
                </div>

            </div>

        </div>


        <%------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>
        <div class="col-lg-12">
            <div class="panel panel-default" runat="server" id="DIV_VALORES">
                <%--            <div class="panel-heading">
                <h3 class="panel-title"><b>Valores</b></h3>
            </div>--%>
                <div class="panel-body">
                    <div class="form-group row">
                        <%--                    <div class="col-lg-12">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina1" />
                    </div>--%>
                        <div>
                            <br />
                        </div>

                        <div class="col-lg-3">
                            <div class="form-group">
                                <label>Total Aprovado</label>
                                <asp:TextBox ID="txtnTotalAprovado" class="form-control " runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-3">
                            <div class="form-group">
                                <label>Total Rejeitado</label>
                                <asp:TextBox ID="txtnTotalRejeitado" class="form-control " runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-3">
                            <div class="form-group">
                                <label>Total Pendente</label>
                                <asp:TextBox ID="txtnTotalPendente" class="form-control " runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-3">
                            <div class="form-group">
                                <label>Total Geral</label>
                                <asp:TextBox ID="txtnTotalGeral" class="form-control " runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-3">
                            <div class="form-group">
                                <label>Total Selecionado</label>
                                <asp:TextBox ID="txtTotal" runat="server" ReadOnly="true" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <%------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>


        <div class="col-lg-12">
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
            <asp:Panel ID="pnResultado" class="" runat="server">
                <div id="resultado">
                    <div class="panel panel-primary">
                        <div class="panel-body">
                            <%----------------------------------------------------%>
                            <button type="button" class="btn" data-toggle="collapse" data-target="#toggleButtons" style="background-color: #009a22; color: white;">
                                Mostrar/Ocultar Colunas
                            </button>
                            <div id="toggleButtons" class="collapse">
                                <br />
                                <asp:PlaceHolder ID="placeholderButtons" runat="server"></asp:PlaceHolder>
                            </div>
                            <br />
                            <br />

                            <%----------------------------------------------------%>
                            <asp:GridView ID="dtgAprovacao" DataKeyNames="idContasPagar" class="table table-striped table-bordered table-hover "
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                ShowFooter="true" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgAprovacao_RowDataBound">

                                <Columns>

                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkTitulo_SelecionarTudo" runat="server" style="display: flex; justify-content: flex-end; align-items: center; margin: auto; margin-right: 6px;" />                                            
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkTitulo_Selecionado" runat="server" />
                                        </ItemTemplate>                                        
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="4%" />
                                    </asp:TemplateField>


                                    <asp:HyperLinkField DataNavigateUrlFields="idContasPagar"
                                        DataTextField="idContasPagar" HeaderText="ID"
                                        DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="4%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idContasPagar"
                                        DataTextField="sRazaoSocial" HeaderText="Credor"
                                        DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="20%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idContasPagar" DataTextFormatString="{0:u}"
                                        DataTextField="dtEmissao_ordem" HeaderText="Data Emissão"
                                        DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="8%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idContasPagar" DataTextFormatString="{0:u}"
                                        DataTextField="dtVencimento_ordem" HeaderText="Data Vencimento"
                                        DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="8%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idContasPagar"
                                        DataTextField="sDocumento" HeaderText="Pedido/Documento"
                                        DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="9%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idContasPagar"
                                        DataTextField="sQuantidadeParcela" HeaderText="Parcela"
                                        DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="6%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idContasPagar"
                                        DataTextField="sCodigo" HeaderText="NF/Referência"
                                        DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="10%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idContasPagar" DataTextFormatString="{0:C2}"
                                        DataTextField="nValorOriginal" HeaderText="Valor"
                                        DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="12%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idContasPagar" DataTextFormatString="{0:C2}"
                                        DataTextField="nSaldo" HeaderText="Saldo a Pagar"
                                        DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="12%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idContasPagar"
                                        DataTextField="sDscCategoriaPagar" HeaderText="Categoria"
                                        DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="10%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idContasPagar"
                                        DataTextField="sStatus" HeaderText="Status"
                                        DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="10%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idContasPagar"
                                        DataTextField="sDscEmpresa" HeaderText="Empresa"
                                        DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="10%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idContasPagar"
                                        DataTextField="sDscAprovacao" HeaderText="Aprovação"
                                        DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="6%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idContasPagar"
                                        DataTextField="sDscColaborador" HeaderText="Colaborador"
                                        DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="10%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>

        <br />
    </div>
    <fieldset class="form-stacked actions">
        <asp:Button ID="BtnAprovar" class="btn btn-lg btn-success" runat="server" Text="Aprovar" OnClientClick="openModalAprovar(); return false;" />
        <asp:Button ID="BtnRejeitar" class="btn btn-lg btn-danger" runat="server" Text="Rejeitar" OnClientClick="openModalRejeitar(); return false;" />

       <%-- <asp:Button ID="btnSelecionarTodos" class="btn btn-lg btn-primary" runat="server" Text="Selecionar Todos" OnClientClick="selectAllCheckboxes(this); return false"/>--%>
        <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field-cancel" title="Voltar" onclick="history.go(-2)">
    </fieldset>

    <style>
        .modal-header-content {
            display: flex;
            align-items: center;
        }

        .modal-logo img {
            max-width: 50px;
            margin-right: 10px;
        }

        .modal-title-container {
            flex-grow: 1;
        }

        .modal-title {
            color: #009A22;
            text-shadow: 2px 2px 2px rgba(0, 0, 0, 0.2);
            font-size: 24px;
            font-weight: bold;
            margin: 0;
        }

        .modal-body, .modal-footer {
            padding: 20px;
        }

            .modal-footer button {
                width: auto;
                padding: 5px 15px;
            }

        .form-control {
            margin-bottom: 10px;
        }

        .modal-body {
            font-size: 18px;
            color: #333;
            font-family: 'Arial', sans-serif;
        }

        .modal-message {
            font-weight: bold;
            margin-bottom: 20px;
        }

        .dataTables_scrollFootInner {
            padding-right: 0px !important;
        }

        div#cphCorpo_dtgAprovacao_filter {
            padding-right: 15px;
        }
    </style>

    <!-- Modal para Aprovação -->
    <div class="modal fade" id="modalAprovar" tabindex="-1" role="dialog" aria-labelledby="modalAprovarLabel">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title" id="modalAprovarLabel">Confirmação de Aprovação</h4>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <p class="modal-message"></p>
                    <asp:TextBox ID="txtsObsAprovacao" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="4000" TextMode="MultiLine" Height="120px" placeholder="Opcional"></asp:TextBox>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-lg btn-success" onclick="confirmAprovar()">Sim</button>
                    <button type="button" class="btn btn-lg btn-danger" data-dismiss="modal">Não</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal para Rejeição -->
    <div class="modal fade" id="modalRejeitar" tabindex="-1" role="dialog" aria-labelledby="modalRejeitarLabel">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title" id="modalRejeitarLabel">Confirmação de Rejeição</h4>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <p class="modal-message">Tem certeza que deseja rejeitar o(s) item(ns) selecionado(s)?</p>
                    <asp:TextBox ID="txtsObsRejeicao" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="4000" TextMode="MultiLine" Height="120px" placeholder="Opcional"></asp:TextBox>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-lg btn-success" onclick="confirmRejeitar()">Sim</button>
                    <button type="button" class="btn btn-lg btn-danger" data-dismiss="modal">Não</button>
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hddidContasPagar" runat="server" />
    <asp:HiddenField ID="hddidAprovacao" runat="server" />

</asp:Content>
