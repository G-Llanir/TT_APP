<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Departamentos_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.Departamentos_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>




<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .checkbox-margin {
            margin-right: 25px;
        }

        .checkbox-label {
            font-size: 14px;
            margin-top: 10px;
            display: block;
        }
    </style>


</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>


    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
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
            </div>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <div class="form-group">
                        <label>ID </label>
                        <asp:TextBox ID="txtidDepartamento" class="form-control CaixaTextoMini" runat="server" disabled="true"></asp:TextBox>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Descrição </label>
                        <asp:TextBox ID="txtsDscDepartamento" class="form-control CaixaTextoGrande" runat="server" MaxLength="200"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Departamento Pai </label>
                        <asp:DropDownList ID="ddlidDepartamentoPai" runat="server" class="form-control CaixaTextoGrande"></asp:DropDownList>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Sigla OS</label>
                        <asp:TextBox ID="txtsSiglaOS" runat="server" class="form-control CaixaTextoGrande" MaxLength="200"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Tipo do Departamento </label>
                        <asp:DropDownList ID="ddlsTpDepartamento" runat="server" AutoPostBack="false" class="form-control yes_no select CaixaTextoGrande"></asp:DropDownList>
                    </div>
                </div>
            </div>
            
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Tipo de Departamento (Fluxo E-mail)</label>
                        <asp:DropDownList ID="ddlsFluxoEmail" runat="server" AutoPostBack="false" class="form-control yes_no select CaixaTextoGrande">
                            <asp:ListItem Text="Selecione um Fluxo E-mail" Value="" ></asp:ListItem>
                            <asp:ListItem Text="Administrativo" Value="Administrativo" ></asp:ListItem>
                            <asp:ListItem Text="Compras" Value="Compras" ></asp:ListItem>
                            <asp:ListItem Text="Contas a Pagar" Value="Pagar" ></asp:ListItem>
                            <asp:ListItem Text="Contas a Receber" Value="Receber" ></asp:ListItem>
                            <asp:ListItem Text="RRHH" Value="RRHH" ></asp:ListItem>
                            <asp:ListItem Text="STSO" Value="STSO" ></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>

            <div class="form-stacked row">

                <div class="col-lg-12">
                    <div class="form-group">
                        <div class="checkbox-label">
                            <asp:CheckBox ID="cbComercial" runat="server" Text="Exibe em Comercial" CssClass="checkbox-margin" />
                       </div>
                        <div class="checkbox-label">
                            <asp:CheckBox ID="cbRequisicoes" runat="server" Text="Exibe nas Requisições" CssClass="checkbox-margin" />
                        </div>
                        <div class="checkbox-label">
                            <asp:CheckBox ID="cbRRHH" runat="server" Text="Exibe em RRHH" CssClass="checkbox-margin" />
                        </div>
                        <div class="checkbox-label">
                            <asp:CheckBox ID="cbComex" runat="server" Text="Exibe em COMEX" CssClass="checkbox-margin" />
                        </div>
                        <div class="checkbox-label">
                            <asp:CheckBox ID="cbOrdemServico" runat="server" Text="Exibe em Ordem de Serviço" CssClass="checkbox-margin" />
                        </div>
                        <div class="checkbox-label">
                            <asp:CheckBox ID="cbPatrimonio" runat="server" Text="Exibe em Patrimônio" CssClass="checkbox-margin" />
                        </div>
                        <div class="checkbox-label">
                            <asp:CheckBox ID="cbMensagens" runat="server" Text="Exibe em Mensagens" CssClass="checkbox-margin" />
                            <i class="fa fa-info-circle" data-toggle="tooltip" title="Permite envio de mensagem para o departamento"></i>
                        </div>
                        <div class="checkbox-label">
                            <asp:CheckBox ID="cbDepartFinanceiro" runat="server" Text="Departamento Financeiro" CssClass="checkbox-margin" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
                </div>
            </div>
            
        <br />
        <div class="form-stacked row">
            <div class="col-lg-9">
                <asp:UpdatePanel ID="updPanel_Acoes" runat="server">
                    <ContentTemplate>
                        <div class="panel panel-default" runat="server" id="Div_Acoes">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Responsáveis</b></h3>
                        </div>
                        <div class="panel-body">
                        <uc1:MensagemPagina runat="server" id="MensagemAcoes" />
				        <div class="row">
					        <div class="col-lg-12" >
						        <div class="form-group">
							        <div class="row" runat="server"  id="Div_Selecao">
								        <div class="col-lg-6">
									        <div class="form-group">
										        <label>Usuário </label>
										        <asp:DropDownList ID="ddlUsuarios" runat="server" class="form-control yes_no select CaixaTextoGrande"> </asp:DropDownList>
									        </div>
								        </div>

                                         <div class="col-lg-2">
									        <div class="form-group">
										        <label>Gestor</label>
										        <asp:DropDownList ID="ddlsGestorDepartamento" runat="server" class="form-control" Width="100px" > 
                                                    <asp:ListItem Value="S">Sim</asp:ListItem>
                                                    <asp:ListItem Selected="True" Value="N">Não</asp:ListItem>
                                                </asp:DropDownList>
							                </div>
								        </div>

								        <div class="col-lg-2">
									        <div class="form-group">
										        <label>Notificação E-mail</label>
										        <asp:DropDownList ID="ddlsNotificacaoEmail" runat="server" class="form-control" Width="100px" > 
                                                    <asp:ListItem Selected="True" Value="S">Sim</asp:ListItem>
                                                    <asp:ListItem Value="N">Não</asp:ListItem>
                                                </asp:DropDownList>
							                </div>
								        </div>
								        <div class="col-lg-2">
									        <br />
									        <asp:LinkButton ID="cmdIncluirSelecao" runat="server" CssClass="btn btn-info" OnClick="cmdIncluirSelecao_Click"><i class="icon-bar"></i>&nbsp;Adicionar</asp:LinkButton>
								        </div>
							        </div>

							        <div class="row">
							        <div class="col-lg-12">
								        <asp:GridView  ID="dtgSelecao" class="table table-striped table-bordered table-hover table-condensed" 
								        runat="server" Width="100%" CellSpacing="1"  CellPadding="1"   AutoGenerateColumns="False" GridLines="None"  
								        ShowFooter="False"  Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgSelecao_RowDataBound" OnRowDeleting="dtgSelecao_RowDeleting"> 
 								        <Columns>
									        <asp:BoundField DataField="idUsuario" HeaderText="idUsuario" >
										        <ItemStyle Width="20%"  HorizontalAlign="Left" VerticalAlign="Middle" />
									        </asp:BoundField>
		                                    <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário" >
										        <ItemStyle Width="30%"  HorizontalAlign="Left" VerticalAlign="Middle" />
									        </asp:BoundField>
                                            <asp:BoundField DataField="sGestorDepartamento" HeaderText="Gestor" >
										        <ItemStyle Width="10%"  HorizontalAlign="Left" VerticalAlign="Middle" />
									        </asp:BoundField>
									        <asp:BoundField DataField="sNotificacaoEmail" HeaderText="E-mail" >
										        <ItemStyle Width="10%"  HorizontalAlign="Left" VerticalAlign="Middle" />
									        </asp:BoundField>
                		                    <asp:TemplateField HeaderText="">
										        <ItemTemplate>
											        <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="icon-remove-sign"></i>&nbsp;Excluir</asp:LinkButton>
										        </ItemTemplate>
										        <ItemStyle Width="18%"   HorizontalAlign="Center" VerticalAlign="Middle" />
									        </asp:TemplateField>
					                        </Columns>
								        <PagerStyle BackColor="Black" ForeColor="Black" HorizontalAlign="Center" />
								        <EditRowStyle BackColor="#2461BF" />
							        </asp:GridView>
							        </div>
							        </div>
						        </div>
					        </div>
				        </div>
                                
                    </div>
                </div>
                    </ContentTemplate>
                    <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="cmdIncluirSelecao" EventName="Click" />
                </Triggers>
                </asp:UpdatePanel>
             </div>
        </div>  
           
        
        <uc1:PainelAtualizacao runat="server" id="PainelAtualizacao" />
             
            
        <fieldset class="form-stacked actions">
            <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
            
            <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" " id="field-cancel" title="Voltar" onclick="history.go(-1)" ></input>
        </fieldset>

        <asp:HiddenField ID="hddidDepartamento" runat="server" />

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
