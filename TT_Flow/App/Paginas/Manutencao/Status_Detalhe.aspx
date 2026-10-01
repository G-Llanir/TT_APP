<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Status_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.Status_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <link rel="stylesheet" href="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.24/themes/start/jquery-ui.css" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.24/jquery-ui.min.js"></script>

    <script type="text/javascript">
        var $j = jQuery.noConflict();

        $j(function () {
            $j("#dialog").dialog({
                autoOpen: false,
                modal: true,
                height: 530,
                width: 500,
                title: "Zoom Imagem"
            });
            $j("[id*=Image1]").click(function () {
                $j('#dialog').html('');
                $j('#dialog').append($j(this).clone());
                $j('#dialog').dialog('open');
            });
        });
    </script>

    <asp:UpdatePanel ID="updDetalhe" runat="server">
        <ContentTemplate>

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
                        <asp:TextBox ID="txtidStatus" class="form-control CaixaTextoMini" runat="server" disabled="disabled"></asp:TextBox>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="form-group">
                        <asp:Label ID="lblsDscStatus" runat="server" Text="Descrição" Font-Bold="true"></asp:Label>
                        <asp:TextBox ID="txtsDscStatus" class="form-control CaixaTextoGrande" runat="server" MaxLength="50"></asp:TextBox>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="form-group">
                        <asp:Label ID="lblsTIpo" runat="server" Text="Tipo" Font-Bold="true"></asp:Label>
                        <asp:DropDownList ID="ddlsTipo" runat="server" class="form-control yes_no select CaixaTextoMedio" AutoPostBack="true" Style="margin-top: 5px" OnSelectedIndexChanged="ddlsTipo_SelectedIndexChanged">
                            <asp:ListItem Selected="True" Value="">Selecione um Tipo</asp:ListItem>
                            <asp:ListItem Value="CRM">CRM</asp:ListItem>
                            <asp:ListItem Value="Pedido">Pedido</asp:ListItem>
                            <asp:ListItem Value="Comex">Comex</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="col-lg-12" runat="server" id="div_ddlAlteraEndereco">
                    <div class="form-group">
                        <label>Permite Alterar Endereço</label>
                        <asp:DropDownList ID="ddlAlteraEndereco" runat="server" class="form-control yes_no select CaixaTextoPequeno" Style="margin-top: 5px">
                            <asp:ListItem Value="S">Sim</asp:ListItem>
                            <asp:ListItem Value="N">Não</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="col-lg-12" runat="server" id="divExibeCliente">
                    <div class="form-group">
                        <asp:Label ID="lblExibeCliente" runat="server" Text="Exibe Status para Cliente" Font-Bold="true"></asp:Label>
                        <asp:DropDownList ID="ddlsExibeCliente" runat="server" class="form-control yes_no select CaixaTextoPequeno" Style="margin-top: 5px">
                            <asp:ListItem Value="S">Sim</asp:ListItem>
                            <asp:ListItem Selected="True" Value="N">Não</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="col-lg-12" runat="server" id="divExibeKanban">
                    <div class="form-group">
                        <asp:Label ID="lblExibeKanban" runat="server" Text="Exibe Kanban" Font-Bold="true"></asp:Label>
                        <asp:DropDownList ID="ddlsExibeKanban" runat="server" class="form-control yes_no select CaixaTextoPequeno" Style="margin-top: 5px">
                            <asp:ListItem Value="S">Sim</asp:ListItem>
                            <asp:ListItem Selected="True" Value="N">Não</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="col-lg-12" runat="server" id="divCancelado">
                    <div class="form-group">
                        <asp:Label ID="lblCancelado" runat="server" Text="Status Cancelamento" Font-Bold="true"></asp:Label>
                        <asp:DropDownList ID="ddlCancelado" runat="server" class="form-control yes_no select CaixaTextoPequeno" Style="margin-top: 5px">
                            <asp:ListItem Value="S">Sim</asp:ListItem>
                            <asp:ListItem Selected="True" Value="N">Não</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="col-lg-12" runat="server" id="divLiberadoFaturamento">
                    <div class="form-group">
                        <asp:Label ID="lblLiberadoFaturamento" runat="server" Text="Liberado para Faturamento" Font-Bold="true"></asp:Label>
                        <asp:DropDownList ID="ddlsLiberadoFaturamento" runat="server" class="form-control yes_no select CaixaTextoPequeno" Style="margin-top: 5px">
                            <asp:ListItem Value="S">Sim</asp:ListItem>
                            <asp:ListItem Selected="True" Value="N">Não</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="form-group">
                        <asp:Label ID="lblPedidoFinalizado" runat="server" Text="Pedido Finalizado" Font-Bold="true"></asp:Label>
                        <asp:DropDownList ID="ddlsPedidoFinalizado" runat="server" class="form-control yes_no select CaixaTextoPequeno" Style="margin-top: 5px">
                            <asp:ListItem Value="S">Sim</asp:ListItem>
                            <asp:ListItem Selected="True" Value="N">Não</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="form-group">
                        <asp:Label ID="lblsCor" runat="server" Text="Cor de Exibição" Font-Bold="true"></asp:Label>
                        <asp:DropDownList ID="ddlsCor" runat="server" class="form-control yes_no select CaixaTextoMedio" Style="margin-top: 5px"></asp:DropDownList>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Pedido em Logística</label>
                        <asp:DropDownList ID="ddlLogistica" runat="server" class="form-control CaixaTextoPequeno">
                            <asp:ListItem Value="S">Sim</asp:ListItem>
                            <asp:ListItem Value="N">Não</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="col-lg-12">
                    <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
                </div>

                <div class="col-lg-12 table-responsive">
                    <div id="div_EnviarArquivos" class="form-stacked " runat="server">
                        <div class="panel panel-default" runat="server">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Enviar Arquivos</b></h3>
                            </div>
                            <uc1:MensagemPagina runat="server" ID="MensagemPagina1" />


                            <div class="panel-body">
                                <div class="form-stacked row">

                                    <div class="col-lg-6" id="div4" runat="server">
                                        <div class="form-group">
                                            <label>Selecione o Arquivo</label>
                                            <asp:FileUpload ID="fu_Arquivo" class="form-control-file" runat="server" Width="400px" />
                                        </div>
                                    </div>

                                    <div class="col-lg-12">
                                        <div class="form-group">
                                            <label>Tipo de Ícone</label>
                                            <asp:DropDownList ID="ddlidTipoArquivo" runat="server" class="form-control yes_no select CaixaTextoGrande">
                                                <asp:ListItem Value="0"> Selecione o tipo </asp:ListItem>
                                                <asp:ListItem Value="1098"> Realizado </asp:ListItem>
                                                <asp:ListItem Value="1097"> Não Realizado </asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>


                                <div class="form-stacked row">

                                    <div class="col-lg-12">
                                        <div class="form-group">
                                            <asp:Button ID="cmdEnviarArquivos" CssClass="btn-success btn" runat="server" Text="Enviar" OnClick="cmdEnviarArquivos_Click" />
                                        </div>
                                    </div>
                                    <div class="col-lg-12">
                                        <div class="form-group">
                                            <asp:GridView ID="gv_Arquivo" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                ShowFooter="False" OnRowCommand="gv_Arquivo_RowCommand" OnRowDataBound="gv_Arquivo_RowDataBound">
                                                <Columns>
                                                    <asp:BoundField DataField="idArquivo" HeaderText="ID">
                                                        <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Inclusão">
                                                        <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sDscTipoArquivo" HeaderText="Tipo">
                                                        <ItemStyle Width="15%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:ButtonField DataTextField="sNomeArquivo" HeaderText="Nome do Arquivo" CommandName="Download" ItemStyle-Width="35%" />

                                                    <asp:TemplateField HeaderText="Image">
                                                        <ItemTemplate>
                                                            <asp:Image ID="Image1" runat="server" class="img-responsive" />
                                                            <itemstyle width="10%" horizontalalign="left" verticalalign="Middle" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Usuário">
                                                        <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                    </asp:BoundField>


                                                </Columns>
                                            </asp:GridView>
                                            <div id="dialog" style="display: none" />

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>

            </div>

            <br />

            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                &nbsp;
            <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" id="field-cancel" title="Voltar" onclick="history.go(-1)">&nbsp;
            </fieldset>

            <asp:HiddenField ID="hddidStatus" runat="server" />

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdEnviarArquivos" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>
