<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ModalUsuario.ascx.cs" Inherits="TT_Flow.App.Controles.ModalUsuario" %>

<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:UpdatePanel ID="UpdModal" runat="server">
    <ContentTemplate>
        <br />

        <style>
            .invisivel {
                display: none;
            }

            .senha {
                display: flex;
                flex-wrap: wrap;
            }

                .senha label {
                    width: 100%;
                }

                .senha input {
                    width: 80%;
                }

                .senha a {
                    width: 20%;
                }

                    .senha a:hover {
                        text-decoration: none;
                    }

            .erro_validador {
                background-color: antiquewhite;
                border: .5px dashed red;
                border-radius: 5px;
                font-weight: bold;
                padding: 5px;
                color: darkred;
            }

            .div_Ponto {
                display: flex;
                flex-wrap: wrap;
                column-gap: 10px;
            }

                .div_Ponto span {
                    display: flex;
                }

                    .div_Ponto span label {
                        margin: 0 0 0 5px;
                    }

                    .div_Ponto span input {
                        margin: 0;
                    }
        </style>

        <div class="modal fade" id="modalUsuario">
            <div class="modal-dialog" style="width: 80%;">
                <div class="modal-content" style="min-height: 300px;">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                        <h3 class="modal-title">
                            <label runat="server" id="lblUsuario"></label>
                        </h3>
                    </div>
                    <div class="modal-body">

                        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_Script" />

                        <div class="row">

                            <div class="col-lg-3 form-group" style="text-align: center;">
                                <asp:Image ID="imgColaborador" runat="server" AlternateText="Imagem do Colaborador" Style="width: 100%; height: 100%; object-fit: contain;" />
                            </div>

                            <div class="col-lg-9" style="padding: 0;">

                                <div class="col-lg-6 form-group">
                                    <label>Empresa</label>
                                    <asp:TextBox ID="txtEmpresa" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-lg-6 form-group">
                                    <label>E-mail</label>
                                    <asp:TextBox ID="txtsEmail" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-lg-6 form-group">
                                    <label>Departamento</label>
                                    <asp:TextBox ID="txtDepartamento" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-lg-3 form-group">
                                    <label>Ramal</label>
                                    <asp:TextBox ID="txtsRamal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-lg-3 form-group">
                                    <label>Celular Empresa</label>
                                    <asp:TextBox ID="txtsTelCelular" class="form-control" runat="server" TextMode="Phone" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div runat="server" id="div_UsuarioLogado">

                                    <div class="col-lg-2 form-group">
                                        <label>Login</label>
                                        <asp:TextBox ID="txtLogin" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                    </div>

                                    <div class="col-lg-4 form-group">
                                        <label>Dashboard Inicial <i class="fa fa-info-circle" data-toggle="tooltip" title="Define a Página Inicial ao realizar o Login"></i></label>
                                        <asp:DropDownList ID="ddlDashboard" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                    </div>

                                    <div class="col-lg-3 form-group senha">
                                        <label>Senha</label>
                                        <asp:TextBox ID="txtSenha" class="form-control" runat="server" TextMode="Password"></asp:TextBox>
                                        <asp:LinkButton ID="cmdExibe_Senha" runat="server" class="btn btn-lg btn-link fa fa-eye"></asp:LinkButton>
                                        <asp:LinkButton ID="cmdEsconde_Senha" runat="server" class="btn btn-lg btn-link fa fa-eye-slash invisivel"></asp:LinkButton>
                                    </div>

                                    <div class="col-lg-3 form-group senha">
                                        <label>Confirmação de Senha</label>
                                        <asp:TextBox ID="txtConfirmaSenha" class="form-control" runat="server" TextMode="Password"></asp:TextBox>
                                        <asp:LinkButton ID="cmdExibe_ConfirmaSenha" runat="server" class="btn btn-lg btn-link fa fa-eye"></asp:LinkButton>
                                        <asp:LinkButton ID="cmdEsconde_ConfirmaSenha" runat="server" class="btn btn-lg btn-link fa fa-eye-slash invisivel"></asp:LinkButton>
                                        <asp:CompareValidator runat="server" CssClass="erro_validador" ControlToValidate="txtConfirmaSenha" ControlToCompare="txtSenha" Display="Dynamic" ErrorMessage="As senhas não batem!" Type="String" />
                                    </div>

                                </div>

                                <div runat="server" id="div_Ponto" class="col-lg-12 div_Ponto">
                                    <label style="width: 100%;">Ponto</label>
                                    <asp:RadioButton runat="server" ID="rbEmExpediente" Text="Em Expediente" CssClass="btn btn-primary ponto pontoEx" GroupName="Ponto"></asp:RadioButton>
                                    <asp:RadioButton runat="server" ID="rbEmDescanso" Text="Em Descanso" CssClass="btn btn-success ponto pontoDe" GroupName="Ponto"></asp:RadioButton>
                                    <asp:RadioButton runat="server" ID="rbEmAlmoco" Text="Em Almoço" CssClass="btn btn-warning ponto pontoAl" GroupName="Ponto"></asp:RadioButton>
                                    <asp:RadioButton runat="server" ID="rbForaExpediente" Text="Fora de Expediente" CssClass="btn btn-danger ponto pontoFo" GroupName="Ponto"></asp:RadioButton>
                                </div>

                            </div>

                        </div>

                    </div>
                    <div class="modal-footer">
                        <asp:Button runat="server" ID="cmdSalvar" class="btn btn-md btn-success" Text="Salvar" />
                        <input type="submit" class="btn btn-md btn-danger" value="Fechar" data-dismiss="modal" />
                    </div>
                </div>
            </div>
        </div>

    </ContentTemplate>
</asp:UpdatePanel>
