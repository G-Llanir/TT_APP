<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Usuarios_Detalhe.aspx.cs" Inherits="TT_Hub.App.Paginas.Manutencao.Usuarios_Detalhe" %>
<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="DIV_ESPACO" style="height:600px" runat ="server" visible="false"> </div>

    <link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css">
    <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>
    <script src="/app/js/vanilla-masker.js"></script>

    <asp:UpdatePanel ID="updUsuario" runat="server" ChildrenAsTriggers="True">
    <ContentTemplate>
        <div class="form-stacked row">
            <div class="col-lg-12">
                <br />
                <uc1:BreadCrumb runat="server" ID="BreadCrumb"  NivelPagina="3" TitulodaPagina="Usuários"/>
                <div class="well">
                    <h4>
                        <span id="MainContent_lblTituloPagina">
                            <asp:Label ID="lblTituloPagina" runat="server" Text="Usuário"></asp:Label>
                        </span>
                    </h4>
                </div>
                
                <div class="panel panel-default">
                    <uc1:MensagemPagina runat="server" id="MensagemPagina" />
            
                    <div class="panel-heading">
                        <h3 class="panel-title"><b>Dados do Usuário</b></h3>
                    </div>
                    <div class="panel-body">
	                    <div class="row">
                            <div class="col-lg-6">
                                <div class="form-group">
                                    <label>Login</label>
                                    <asp:TextBox ID="txtsLogin" class="form-control CaixaTextoMedio" runat="server" MaxLength="50" ></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-6">
                                <div class="form-group">
                                    <label>Nome</label>
                                    <asp:TextBox ID="txtsDsUsuario" class="form-control  CaixaTextoGrande"  Width="400px" runat="server" MaxLength="200" ></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-6">
                                <div class="form-group">
                                    <label>Senha</label>
                                    <asp:TextBox ID="txtsSenha" class="form-control  CaixaTextoMedio" runat="server" MaxLength="50" TextMode="Password" OnTextChanged="txtsSenha_TextChanged"  ></asp:TextBox>
                                </div>
                            </div>
                            
                            <div class="col-lg-6">
                                <div class="form-group">
                                    <label>Confirmação de Senha</label>
                                    <asp:TextBox ID="txtsSenha_Confirmacao" class="form-control  CaixaTextoMedio" runat="server" MaxLength="50" TextMode="Password" OnTextChanged="txtsSenha_Confirmacao_TextChanged"  ></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-6">
                                <div class="form-group">
                                    <label>e-mail</label>
                                    <asp:TextBox ID="txtsEmail" class="form-control "  Width="400px" runat="server" MaxLength="200" ></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-6">
								<div class="form-group">
									<label>Tipo Usuário</label>
									<asp:DropDownList ID="ddlTpUsuario" runat="server" AutoPostBack="true" class="form-control yes_no select" Width="400px" OnSelectedIndexChanged="ddlTpUsuario_SelectedIndexChanged" > 
                                        <asp:ListItem>Selecione o Tipo</asp:ListItem>
                                        <asp:ListItem Value="I">Usuário Interno</asp:ListItem>
                                        <asp:ListItem Value="A">Administrador</asp:ListItem>
                                        <asp:ListItem Value="C">Usuário Externo</asp:ListItem>
                                    </asp:DropDownList>
					            </div>
							</div>
		                    
					        <div class="col-lg-6" runat="server" id="DIV_Cliente">
								<div class="form-group">
									<label></label>
									<asp:DropDownList ID="ddlCliente" runat="server" AutoPostBack="false" class="form-control yes_no select" Width="400px" OnSelectedIndexChanged="ddlCliente_SelectedIndexChanged" > </asp:DropDownList>
					            </div>
							</div>
		                </div>
                        <div class="row">
                            <div class="col-lg-6">
                                <uc1:ComboAtivo runat="server" ID="ddlAtivo" />
                            </div> 
                        </div>
                    </div>
                </div>
                            
                <uc1:PainelAtualizacao runat="server" id="PainelAtualizacao" />
                
                    
                <fieldset class="form-stacked actions">
                    <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar"  OnClick="cmdSalvar_Click"  />
                    &nbsp;
                    <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" " id="field-cancel" title="Voltar" onclick="history.go(-1)" >&nbsp;
                </fieldset>
                    


          

            </div>   
        </div>

        <div id="dialog-Salvar" title="Salvar">
            <p><span class="ui-icon ui-icon-alert" style="float:left; margin:12px 12px 20px 0;"></span><asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label></p>
        </div>
        

        <asp:HiddenField ID="hddIdUsuario" runat="server" />
        <asp:HiddenField ID="hddidCliente" runat="server" />
        <asp:HiddenField ID="hddsSenha" runat="server" />
        <asp:HiddenField ID="hddsSenhaConfirmacao" runat="server" />

    </ContentTemplate>            
</asp:UpdatePanel>
</asp:Content>
