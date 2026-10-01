<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TT_Login.AutoAvaliacao.Default" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<!DOCTYPE html>

<!DOCTYPE html>
<html lang="pt-br">

    <head>
        <meta charset="utf-8">
        <meta http-equiv="X-UA-Compatible" content="IE=edge">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <link rel='icon' type='image/x-icon' href='/App/img/LogoTT.png' />
        <!-- Just for debugging purposes. Don't actually copy this line! -->
        <!--[if lt IE 9]><script src="../../assets/js/ie8-responsive-file-warning.js"></script><![endif]-->
        <!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries -->
        <!--[if lt IE 9]>
            <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
            <script src="https://oss.maxcdn.com/libs/respond.js/1.4.2/respond.min.js"></script>
        <![endif]-->
        <title><%=TT.FrameWork.Identity.Variaveis.sNomeSistema() %> - Autoavaliacao </title>
        <link href="/app/css/bootstrap.css" rel="stylesheet">
        <link href="/app/css/font-awesome/css/font-awesome.css" rel="stylesheet">
        
        <script src="/app/js/jquery-3.2.0.min.js"></script>

        <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.0/jquery.mask.js"></script>
<script>
    $(document).ready(function () {
        var $seuCampoCpf = $("#txtsCPF");
        $seuCampoCpf.mask('999.999.999-99', { reverse: false });
    });

    function getGeolocation() {
        if (navigator.geolocation && $('[id*=hddLatitude]').val().length <= 0 && $('[id*=hddLongitude]').val().length <= 0) {
            navigator.geolocation.getCurrentPosition(sendPositionToServer, showError);
        }
    };

    function sendPositionToServer(position) {
        $('[id*=hddLatitude]').val(position.coords.latitude);
        $('[id*=hddLongitude]').val(position.coords.longitude);
        __doPostBack('funcao_LOCALIZACAO','');
    };
    function showError(error) {
        switch (error.code) {
            case error.PERMISSION_DENIED:
                console.log('Usuário negou a solicitação de geolocalização!');
                break;
            case error.POSITION_UNAVAILABLE:
                console.log('Informação da localização não está disponível!');
                break;
            case error.TIMEOUT:
                console.log('A solicitação para obter localização expirou!');
                break;
            case error.UNKNOWN_ERROR:
                console.log('Ocorreu um erro desconhecido!');
                break;
        }
    };

</script>
    </head>


    <body onload="getGeolocation()">
         
        <br />
        <br />
        <br />
        <form id="frmAvaliacao" runat="server" role="form">
        <asp:ScriptManager ID="sm_main" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
        <div id="DIV_Principal" runat="server"  >
            <div class="login-panel panel panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"> <asp:Label ID="lblTitulo" Text="" runat="server"></asp:Label> </h3>
                </div>
                <div class="panel-body">
                    <div class=" col-lg-12">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    </div>
                <asp:MultiView ID="mtv_Principal" runat="server">
                        
                    <asp:View ID="view_Confirmacao" runat="server">
                        <div class="form-group">
                            <div class=" col-lg-12">
                                <label>Olá, seja bem-vindo ao sistema de Autoavaliação da <%=TT.FrameWork.Identity.Variaveis.sNomeSistema() %>, para continuar, por favor informe seu CPF</label>
                                <br />
                                <br />
                            </div>    
                            <div class="col-lg-12">
                                <div class="form-group row">
                                    <div class="col-lg-6">
                                        <asp:TextBox ID="txtsCPF" runat="server" class="form-control"  placeholder="CPF" MaxLength="14" ></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class=" col-lg-12">
                                <fieldset class="form-stacked actions">
                                    <div class="form-group">
                                        <div class="row col-lg-12">
                                            <br />
                                            <asp:Button ID="cmdIdentificar" runat="server" class="btn btn btn-success" text="Continuar" OnClick="cmdIdentificar_Click"  ValidationGroup="AutoAvaliacao"  onclientclick="MudarBotao()" />
                                        </div>
                                
                                        <div class="row col-lg-12">
                                            <br />
                                            <asp:LinkButton ID="cmdAnonimo" OnClick="cmdAnonimo_Click" runat="server">Anônimo</asp:LinkButton>
                                        </div>
                                    </div>
                                </fieldset>
                            </div>
                        </div>

                    </asp:View>

                    <asp:View ID="view_Identificacao" runat="server">
                        <div class="form-group">
                            <div class=" col-lg-12">
                                <label>Por favor, confirme seus dados e clique em Continuar.</label>
                                <br />
                                <br />

                            </div>
                            <div class="col-lg-12">
                                <div class="form-group">
                                    <label>Colaborador</label>
                                    <asp:TextBox ID="txtsNome" runat="server" class="form-control"  MaxLength="200" ></asp:TextBox>
                                </div>

                                <div class="form-group">
                                    <label>Departamento</label>
                                    <asp:DropDownList ID="ddlidDepartamento" class="form-control" runat="server"></asp:DropDownList>
                                </div>

                            </div>
                            <div class="col-lg-12">
                                <fieldset class="form-stacked actions">
                                    <div class="form-group">
                                        <div class="row col-lg-12">
                                            <br />
                                            <asp:Button ID="cmdAvancar_2" runat="server" class="btn btn btn-success" text="Continuar" OnClick="cmdAvancar_2_Click"  ValidationGroup="AutoAvaliacao"  onclientclick="MudarBotao()" />
                                        </div>
                                    </div>
                                </fieldset>
                            </div>
                            <br />
                            <br />

                        </div>
                    </asp:View>

                    <asp:View ID="view_Perguntas" runat="server">
                        <div class="form-group">
                            <div class=" col-lg-12"> 
                            <div class="well bg-danger" runat="server" id="caixaTitulo">
                                 <h5><label><asp:Label ID="lblTituloPergunta" runat="server" Text="TITULO_PERGUNTA"></asp:Label></label></h5>
                             </div>
                            </div>
                            <div class=" col-lg-12">
                            <div class=" col-lg-12">
                                <div class="form-group">
                                    <label><asp:Label ID="lblPergunta" runat="server" Text="TITULO_PERGUNTA"></asp:Label></label>
                                </div>

                                <div class="btn-group" id="DIV_sTipo_SIM_NAO" runat="server">
                                    <asp:RadioButtonList  id="rb_Resposta" Runat="server"  RepeatDirection="Horizontal" OnSelectedIndexChanged="rb_Resposta_SelectedIndexChanged" AutoPostBack="true">
                                        <asp:ListItem Value="S">&nbsp;Sim &nbsp;&nbsp;</asp:ListItem>
                                        <asp:ListItem Value="N">&nbsp;Não</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>

                                <div class="btn-group" id="DIV_sTipo_Numeral" runat="server">
                                    <asp:RadioButtonList  id="rb_Numeral" Runat="server" OnSelectedIndexChanged="rb_Numeral_SelectedIndexChanged" AutoPostBack="true" RepeatDirection="Horizontal"></asp:RadioButtonList>
                                </div>

                                <div class="btn-group" id="DIV_sTipo_Unica" runat="server">
                                    <asp:RadioButtonList  id="rb_OpcaoUnica" Runat="server" OnSelectedIndexChanged="rb_OpcaoUnica_SelectedIndexChanged" AutoPostBack="true" RepeatDirection="Vertical"></asp:RadioButtonList>
                                </div>

                                <div class="btn-group" id="DIV_sTipo_Multipla" runat="server">
                                    <asp:CheckBoxList ID="chkMultipla" runat="server" OnSelectedIndexChanged="chkMultipla_SelectedIndexChanged" AutoPostBack="true" ></asp:CheckBoxList>
                                </div>


                            </div>
                            
                            <div class="col-lg-12 form-group" id="DIV_sTipo_TEXTO" runat="server">
                                <label id="lblJustifique" runat="server">
                                    </br>
                                    <asp:Label ID="lblJustifique_Texto" runat="server" Text="Justifique"></asp:Label>
                                </label>
                                <asp:TextBox ID="txtsResposta" runat="server" class="form-control" TextMode="MultiLine" Height="250px" MaxLength="1200" ></asp:TextBox>
                            </div>
                            </div>
                        </div>
                        <div class=" col-lg-12">
                            <fieldset class="form-stacked actions">
                                <div class="form-group">
                                    <div class="row col-lg-12">
                                        <br />
                                        <asp:Button ID="cmdAvancar" runat="server" class="btn btn btn-success" text="Continuar" OnClick="cmdAvancar_2_Click"  ValidationGroup="AutoAvaliacao"  onclientclick="MudarBotao()" />
                                    </div>
                                </div>
                            </fieldset>
                        </div>
                        <br />
                        <br />
                    </asp:View>

                    <asp:View ID="view_Finalizacao" runat="server">
                        <div class="form-group">
                             <div class=" col-lg-12 text-center" align="center">
                                 <label><h3>Obrigado, sua avaliação foi finalizada e registrada com sucesso!</label></h3>
                             </div>
                            <br />
                            <br />
                            <br />
                            <br />

                        </div>
                        <div class="form-group">
                            <div class="col-lg-12 text-center" align="center">
                                <asp:Button ID="cmdFinalizar" runat="server" class="btn btn btn-success" text="Finalizar" OnClick="cmdFinalizar_Click" ValidationGroup="AutoAvaliacao"  onclientclick="MudarBotao()" />
                            </div>
                        </div>

               
                    </asp:View>



                </asp:MultiView>
                </div>
            </div>
        </div>
        <asp:HiddenField ID="hddidColaborador" runat="server" />
        <asp:HiddenField ID="hddidAvaliacao" runat="server" />
        <asp:HiddenField ID="hddsTipo" runat="server" />
        <asp:HiddenField ID="hddidRegistro" runat="server" />    
        <asp:HiddenField ID="hddidPergunta" runat="server" />  
        <asp:HiddenField ID="hddsCaixadeObservacao" runat="server" />
        <asp:HiddenField ID="hddLatitude" runat="server" />
        <asp:HiddenField ID="hddLongitude" runat="server" />
        <asp:HiddenField ID="hddsChave" runat="server" />
        <asp:HiddenField ID="hddidU" runat="server" />
        <asp:HiddenField ID="hddsRP" runat="server" />


        </ContentTemplate>
        </asp:UpdatePanel>
        </form>


    <script src="/app/js/bootstrap.min.js"></script>
    <script src="/app/js/plugins/metisMenu/jquery.metisMenu.js"></script>
    <script src="/app/js/sb-admin.js"></script>

</body>

</html>
