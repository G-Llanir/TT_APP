<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Pedidos_Detalhe_Acoes.aspx.cs" Inherits="TT_Flow.app.Paginas.Pedidos_Detalhe_Acoes" %>

<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Ações</title>

    <script src="https://login.tecandtec.com.br/app/js/jquery-3.2.0.min.js"></script>
    <script src="https://login.tecandtec.com.br/app/js/plugins/datepicker/bootstrap-datepicker.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.16/jquery.mask.min.js"></script>
    <link href="https://getbootstrap.com/docs/3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://login.tecandtec.com.br/app/css/TT.css" rel="stylesheet" />
    <link href="https://login.tecandtec.com.br/app/css/plugins/datepicker/css/datepicker.css" rel="stylesheet" />
    <link href="/app/css/font-awesome/css/font-awesome.min.css" rel="stylesheet" />
</head>
<body>

    <form id="frmPedidoAcoes" runat="server">
        <asp:ScriptManager runat="server"></asp:ScriptManager>

        <script type="text/javascript">

            function MudarBotao() {
                document.getElementById("cmdConfirmar").value = "Aguarde..";
            }

            $(document).ready(function () {
                $('[id*=txtDatas]').datepicker({
                    autoclose: true,
                    format: 'dd/mm/yyyy',
                    language: 'pt-BR'
                });
            });

        </script>

        <br />

        <div class="container-fluid row">

            <asp:UpdatePanel ID="upd_Acoes" runat="server" RenderMode="Inline">
                <ContentTemplate>

                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                    <asp:MultiView ID="BaseMultiView" runat="server">

                        <asp:View ID="view_Branco" runat="server"></asp:View>

                        <asp:View ID="view_Selecao" runat="server">
                            <div class="panel panel-default" runat="server">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>Ações</b></h3>
                                </div>

                                <div class="panel-body">
                                    <div class="form-stacked row">
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <label></label>
                                                <asp:DropDownList ID="ddlAcao" runat="server" class="form-control yes_no select CaixaTextoGrande" AutoPostBack="True" OnSelectedIndexChanged="ddlAcao_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </asp:View>

                        <asp:View ID="view_Confirmacao" runat="server">

                            <div class="col-md-6 col-md-offset-3">
                                <div class="col-lg-6">
                                    <div class="panel panel-success">
                                        <div class="panel-heading">
                                            <h3 class="panel-title"><b>Confirmação</b></h3>
                                        </div>
                                        <div class="panel-body">
                                            <div class="form-stacked row">
                                                <div class="col-lg-8">
                                                    <div class="form-group text-center">
                                                        <h3><b>
                                                            <asp:Label ID="lblMensagemConfirmacao" runat="server" Text="Label" CssClass="text-warning"></asp:Label></b></h3>
                                                    </div>
                                                </div>
                                                <div class="col-lg-8">
                                                    <div class="form-group text-center">
                                                        <br />
                                                    </div>
                                                </div>
                                                <div class="col-lg-8">
                                                    <fieldset class="form-stacked actions text-center">
                                                        <asp:Button ID="cmdConfirmar" CssClass="btn btn-lg btn-primary" runat="server" Text="Sim" OnClientClick="MudarBotao()" OnClick="cmdConfirmar_Click" />
                                                        <asp:Button ID="cmdVoltar" CssClass="btn btn-lg btn-warning" runat="server" Text="Não" OnClick="cmdVoltar_Click" />
                                                    </fieldset>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </asp:View>

                        <asp:View ID="view_CancelamentoPedido" runat="server">

                            <div id="DIV_CancelarPedido" runat="server">
                                <div class="panel panel-default" runat="server">
                                    <div class="panel-heading">
                                        <h3 class="panel-title"><b>Cancelar Pedido </b></h3>
                                    </div>
                                    <div class="panel-body">
                                        <div class="form-stacked row">
                                            <div class="col-lg-8">
                                                <div class="form-group">
                                                    <label>Motivo</label>
                                                    <asp:DropDownList ID="ddlidMotivoCancelamento" runat="server" AutoPostBack="false" class="form-control yes_no selec CaixaTextoGrande"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-lg-8">
                                                <div class="form-group">
                                                    <label>Observação</label>
                                                    <asp:TextBox ID="txtMotivoCancelamento" class="form-control uppercase CaixaTextoGrande" runat="server" MaxLength="300"></asp:TextBox>
                                                </div>
                                            </div>
                                            <fieldset class="form-stacked actions">
                                                <asp:Button ID="cmdCancelamento_Executar" class="btn  btn-lg btn-success" runat="server" Text="Confirmar" OnClick="cmdAcoes_Executar_Click" />
                                                <asp:Button ID="cmdCancelamento_Voltar" class="btn  btn-lg  btn-danger" runat="server" Text="Voltar" OnClick="cmdCancelamento_Voltar_Click" />
                                            </fieldset>
                                        </div>
                                    </div>

                                </div>
                            </div>

                        </asp:View>

                        <asp:View ID="view_AlterarDepartamento" runat="server">
                            <div id="DIV1" runat="server">
                                <div class="panel panel-default" runat="server">
                                    <div class="panel-heading">
                                        <h3 class="panel-title"><b>Enviar pedido para um departamento </b></h3>
                                    </div>
                                    <div class="panel-body">
                                        <div class="form-stacked row">

                                            <div class="col-lg-12">
                                                <div class="form-group">
                                                    <label>Departamento</label>
                                                    <asp:DropDownList ID="ddlidAlterarDepartamento" runat="server" AutoPostBack="false" class="form-control yes_no selec CaixaTextoGrande"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-12">
                                                <div class="form-group">
                                                    <asp:RadioButtonList ID="rdlStatus_AlterarDepartamento" class="radio" runat="server" AutoPostBack="True" OnSelectedIndexChanged="rdlStatus_AlterarDepartamento_SelectedIndexChanged"></asp:RadioButtonList>
                                                    <asp:DropDownList ID="ddlidStatus_AlterarDepartamento" runat="server" AutoPostBack="false" class="form-control yes_no selec CaixaTextoGrande"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-12">
                                                <div class="form-group">
                                                    <label>Observação</label>
                                                    <asp:TextBox ID="txtSObservacao_AlterarDepartamento" class="form-control CaixaTextoGrande" runat="server" MaxLength="300"></asp:TextBox>
                                                </div>
                                            </div>

                                            <fieldset class="form-stacked actions">
                                                <asp:Button ID="cmdConfirmar_AlterarDepartamento" class="btn  btn-lg btn-success" runat="server" Text="Confirmar" OnClick="cmdAcoes_Executar_Click" />
                                                <asp:Button ID="cmdCancelar_AlterarDepartamento" class="btn  btn-lg  btn-danger" runat="server" Text="Voltar" OnClick="cmdCancelamento_Voltar_Click" />
                                            </fieldset>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </asp:View>

                        <asp:View ID="view_Aceitar_RejeitarPedido" runat="server">
                            <div id="DIV2" runat="server">
                                <div class="panel panel-default" runat="server">
                                    <div class="panel-heading">
                                        <h3 class="panel-title"><b>
                                            <asp:Label ID="lblAceitarRejeitar_Titulo" runat="server" Text="TITULO"></asp:Label></b></h3>
                                    </div>
                                    <div class="panel-body">
                                        <div class="form-stacked row">

                                            <div class="col-lg-12" id="divAceitarRejeitar_Departamento" runat="server">
                                                <div class="form-group">
                                                    <label>Pedido será enviado para o Departamento</label>
                                                    <asp:TextBox ID="txtAceitarRejeitar_sDscDepartamento" class="form-control CaixaTextoGrande" runat="server" disabled></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-12" id="divAceitarRejeitar_datas" runat="server">
                                                <div class="form-group">
                                                    <label>Nova Data</label>
                                                    <asp:TextBox ID="txtDatas" type="date" class="form-control CaixaTextoPequeno" runat="server"></asp:TextBox>
                                                </div>
                                            </div>


                                            <div class="col-lg-12">
                                                <div class="form-group">
                                                    <label>
                                                        <asp:Label ID="lblAceitarRejeitar_Motivo" runat="server" Text="TITULO"></asp:Label></label>
                                                    <asp:TextBox ID="txtAceitarRejeitar_Motivo" class="form-control CaixaTextoGigante" runat="server" MaxLength="300"></asp:TextBox>
                                                </div>
                                            </div>

                                            <fieldset class="form-stacked actions">
                                                <asp:Button ID="cmdAceitarRejeitar_Confirmar" class="btn  btn-lg btn-success" runat="server" Text="Confirmar" OnClick="cmdAcoes_Executar_Click" />
                                                <asp:Button ID="cmdAceitarRejeitar_Cancelar" class="btn  btn-lg  btn-danger" runat="server" Text="Voltar" OnClick="cmdCancelamento_Voltar_Click" />
                                            </fieldset>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </asp:View>

                        <asp:View ID="view_FileUpLoad" runat="server">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>

                                    <div id="DIV3" runat="server">
                                        <div class="panel panel-default" runat="server">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>
                                                    <asp:Label ID="Label1" runat="server" Text="TITULO"></asp:Label></b></h3>
                                            </div>
                                            <div class="panel-body">
                                                <div class="form-stacked row">

                                                    <div class="col-lg-12" id="div4" runat="server">
                                                        <div class="form-group">
                                                            <label>Selecione o Arquivo</label>
                                                            <asp:FileUpload ID="fu_Arquivo" runat="server" />
                                                        </div>
                                                    </div>




                                                    <div class="col-lg-12">
                                                        <div class="form-group">
                                                            <label>
                                                                <asp:Label ID="lblEnviarArquivo_sDscArquivo" runat="server" Text="TITULO"></asp:Label></label>
                                                            <asp:TextBox ID="txtEnviarArquivo_sDscArquivo" class="form-control CaixaTextoGigante" runat="server" MaxLength="300"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <fieldset class="form-stacked actions">
                                                        <asp:Button ID="cmdEnviarArquivo_OK" class="btn  btn-lg btn-success" runat="server" Text="Enviar" OnClick="cmdAcoes_Executar_Click" />
                                                        <asp:Button ID="cmdEnviarArquivo_Cancelar" class="btn  btn-lg  btn-danger" runat="server" Text="Voltar" OnClick="cmdCancelamento_Voltar_Click" />
                                                    </fieldset>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </asp:View>

                        <asp:View ID="view_SolicitarDocumentosSTSO" runat="server">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                <ContentTemplate>

                                    <div id="DIV5" runat="server">
                                        <div class="panel panel-default" runat="server">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>
                                                    <asp:Label ID="Label2" runat="server" Text="Solicitar Documentos STSO"></asp:Label></b></h3>
                                            </div>
                                            <div class="panel-body">
                                                <div class="form-stacked row">

                                                    <div class="col-lg-12">
                                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina7" />
                                                    </div>

                                                    <div class="col-lg-2">
                                                        <div class="form-group">
                                                            <label>Usuários Destinatário</label>
                                                            <asp:DropDownList ID="ddlidUsuario" runat="server" class="form-control yes_no selec Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlidUsuario_SelectedIndexChanged"></asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-4">
                                                        <div class="form-group">
                                                            <label>
                                                                <asp:Label ID="Label4" runat="server" Text="Email"></asp:Label></label>
                                                            <asp:TextBox ID="txtEmail" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-12 ">
                                                        <div class="form-group">
                                                            <label>Observações</label>
                                                            <asp:TextBox ID="txtsObservacao" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="150px"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <fieldset class="form-stacked actions">
                                                        <asp:Button ID="btnConfirmarSolicitar" class="btn  btn-lg btn-success" runat="server" Text="Confirmar" OnClick="btnConfirmarSolicitar_Click" />
                                                        <asp:Button ID="bntVoltarSolicitar" class="btn  btn-lg  btn-danger" runat="server" Text="Voltar" OnClick="bntVoltarSolicitar_Click" />
                                                    </fieldset>

                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </asp:View>

                        <asp:View ID="view_GerarCentroDeCusto" runat="server">
                            <div class="panel panel-default" runat="server">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>Gerar Centro de Custo</b></h3>
                                </div>

                                <div class="panel-body">
                                    <div class="form-stacked row">
                                        <div class="col-lg-12">
                                            <div class="col-lg-3 form-group">
                                                <label>Valor Teto</label>
                                                <asp:TextBox ID="txtnValorTeto" runat="server" class="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-12" style="padding: 0px 25px;">
                                            <div class="form-group">
                                                <label></label>
                                                <asp:Button ID="btnGerarCentroDeCusto" runat="server" class="btn btn-info" Text="Gerar Centro de Custo" OnClick="btnGerarCentroDeCusto_Click" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </asp:View>

                        <asp:View ID="view_GerarMedicao" runat="server">
                            <div class="panel panel-default" runat="server">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>Gerar Medição</b></h3>
                                </div>

                                <div class="panel-body">
                                    <div class="form-stacked row">
                                        <div class="col-lg-2" id="DIV62" runat="server">
                                            <div class="form-group">
                                                <label>Data</label>
                                                <asp:TextBox ID="txtdtMedicao" class="form-control uppercase " runat="server" type="date"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-2" id="DIV59" runat="server">
                                            <div class="form-group">
                                                <label>% Obra</label>
                                                <asp:TextBox ID="txtsPorcentagem" class="form-control uppercase " runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-2" id="DIV61" runat="server">
                                            <div class="form-group">
                                                <br />
                                                <label>&nbsp;</label>
                                                <asp:Button ID="btnIncluirMedicao" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="btnIncluirMedicao_Click" />
                                            </div>
                                        </div>

                                        <div class="col-lg-12">
                                            <div class="col-lg-6 table-responsive">
                                                <div class="row">
                                                    <div class="form-group">
                                                        <br />
                                                        <asp:GridView ID="gvMedicao" class="table table-striped table-bordered table-hover"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            ShowFooter="false" DataKeyNames="idMedicao" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small"
                                                            OnRowDeleting="gvMedicao_RowDeleting">
                                                            <Columns>

                                                                <asp:BoundField DataField="idMedicao" HeaderText="ID">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="dtMedicao" HeaderText="Data">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="nPorcentagem" HeaderText="% Obra">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                                </asp:BoundField>

                                                                <asp:CommandField ButtonType="Link" DeleteText="<i class='fa-eraser fa'></i>" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" ItemStyle-Width="5%" ShowDeleteButton="True" />

                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" id="DIV6" runat="server">
                                            <div class="form-group">
                                                <br />
                                                <asp:Button ID="btnVoltar" class="btn btn-danger" runat="server" ValidationGroup="Item" Text="Voltar" OnClick="btnVoltar_Click" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </asp:View>

                    </asp:MultiView>

                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlAcao" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="cmdCancelamento_Executar" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="cmdCancelamento_Voltar" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>

        </div>
        <asp:HiddenField ID="hddidPedido" runat="server" />
        <asp:HiddenField ID="hddidTipo" runat="server" />
        <asp:HiddenField ID="hddsDscTipo" runat="server" />

    </form>
</body>
</html>
