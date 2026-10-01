<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Ocorrencia_Detalhe.aspx.cs" Inherits="TT_Hub.App.Paginas.Ocorrencia_Detalhe" %>
<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%--<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>--%>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>




<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div id="DIV_ESPACO" style="height:600px" runat ="server" visible="false"> </div>
<script src="https://login.tecandtec.com.br/app/js/jquery-1.9.2.min.js" type="text/javascript"></script>
<script> $v192 = jQuery.noConflict();</script>

  
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <div class="form-stacked row">
            <div class="col-lg-12">
                <br />
                <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
                <div class="well bg-danger" runat="server" id="caixaTitulo">
                    <h4><span id="MainContent_lblTituloPagina">
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label>
                          <span class="status" title="Status">
                                <asp:Label ID="lblsDscTipoStatus" runat="server" TabIndex="100"></asp:Label>
                        </span>
                        </span></h4>
                </div>
                <uc1:MensagemPagina runat="server" id="MensagemPagina" />
            </div>
        </div>
        <div class="panel panel-default" runat="server" id="div_Ocorrencia">
            <div class="panel-heading">
                <h3 class="panel-title">Ocorrência</h3>
            </div>
            <div class="panel-body">
                <div class="form-stacked row">
                    <div class="col-lg-6">
                    <div class="row">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>Data da Ocorrência </label>
                                <asp:TextBox ID="txtdtOcorrencia" class="form-control uppercase CaixaTextoMedio" runat="server" MaxLength="10"></asp:TextBox>
                            </div>
                        </div>
               
                        <div class="col-lg-6">
                            <div class="form-group">
                                <label>Status</label>
                                <asp:TextBox ID="txtsDscTipoStatus" class="form-control uppercase CaixaTextoMedio " runat="server" MaxLength="200"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    </div>

                     <div class="col-lg-6">
                        <div class="form-group">
                            <label>Evento</label>
                            <asp:TextBox ID="txtsDscEventoCompleto" class="form-control" runat="server" MaxLength="200"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-lg-6">
                         <div class="row">
                            <div class="col-lg-6">
                                <div class="form-group">
                                    <label>Equipamento</label>
                                    <asp:TextBox ID="txtsDscTipoEquipamento" class="form-control uppercase  " runat="server" MaxLength="200"></asp:TextBox>
                                </div>
                            </div>
					        <div class="col-lg-6">
                                <div class="form-group">
                                  <label>ID Equipamento</label>
                                  <asp:TextBox ID="txtsID" class="form-control uppercase " runat="server" MaxLength="200"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                    </div>

                    <div class="col-lg-6">
                        <div class="form-group">
                            <label>Cliente </label>
                            <asp:TextBox ID="txtsDscCliente" class="form-control uppercase CaixaTextoGigante" runat="server" MaxLength="200"></asp:TextBox>
                        </div>
                    </div>

                    <div class="col-lg-6">
                        <div class="row">
                            <div class="col-lg-6">
                                <div class="form-group">
                                    <label>Contato Técnico</label>
                                    <asp:TextBox ID="txtsContatoTecnico" class="form-control CaixaTextoMedio"  runat="server"  MaxLength="50"    ></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-lg-6">
                                <div class="form-group">
                                    <label>Telefone</label>
                                    <asp:TextBox ID="txtsTelefoneTecnico" class="form-control CaixaTextoMedio"  runat="server"  MaxLength="50"    ></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-6">
                        <div class="form-group">
                            <label>Arquivo Recebido</label>
                            <asp:TextBox ID="txtsNomeArquivo" class="form-control uppercase CaixaTextoGigante " runat="server" MaxLength="200"></asp:TextBox>
                        </div>
                    </div>

                    <div class="col-lg-6">
                        <div class="form-group">
                            <label>Evento Recebido</label>
                            <asp:TextBox ID="txtsRegistroCompleto" class="form-control CaixaTextoGrande " runat="server" MaxLength="200"></asp:TextBox>
                        </div>
                    </div>

                    <div class="col-lg-6">
                        <div class="row">
                            <div class="col-lg-3">
                                    <div class="form-group">
                                    <label>Segware</label>
                                    <asp:TextBox ID="txtsEnviarSWCompleto" class="form-control CaixaTextoMini " runat="server"  MaxLength="16" ></asp:TextBox>
                                </div>
                            </div>
				            <div class="col-lg-3" runat="server" id="dv_CodigoSW">
                                <div class="form-group">
                                    <label>Código SW  </label>
                                    <asp:TextBox ID="txtsCodigoSW" class="form-control CaixaTextoMini " runat="server"  MaxLength="4" ></asp:TextBox>
                                </div>
                            </div>
                                <div class="col-lg-4" runat="server" id="dv_dtEnvioSW">
                                <div class="form-group">
                                    <label>dtEnvio SW  </label>
                                    <asp:TextBox ID="txtdtEnvioSW" class="form-control CaixaTextoMedio " runat="server"  ></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
        <asp:UpdatePanel ID="updPanel_Acoes" runat="server">
            <ContentTemplate>
            <div class="panel panel-default" runat="server" id="Div_Acoes">
            <div class="panel-heading">
                <h3 class="panel-title">Ações</h3>
            </div>
            <div class="panel-body">
				<div class="row">
					<div class="col-lg-12">
						<div class="form-group">
							<div class="row" runat="server"  id="div_SelecaoAcao">
								<div class="col-lg-3">
									<div class="form-group">
										<label>Tipo </label>
										<asp:DropDownList ID="ddlAccao" runat="server" class="form-control" Width="250px" attrname=txtsAcao > </asp:DropDownList>
									</div>
								</div>
								<div class="col-lg-3">
									<div class="form-group">
										<label>Observação</label>
										<asp:TextBox ID="txtsObservacaoAcao" class="form-control"  runat="server"  style="width:250px;" MaxLength="150" attrname="txtsObservacao" ></asp:TextBox>
									</div>
								</div>
								<div class="col-lg-2">
									<br />
									<asp:LinkButton ID="cmdIncluirAcao" runat="server" CssClass="btn btn-info" OnClick="cmdIncluirAcao_Click" ><i class="icon-bar"></i>&nbsp;Adicionar</asp:LinkButton>
								</div>
								<div class="col-lg-3">
									<br />
									&nbsp;&nbsp;
									<asp:Label ID="lblMensagem_Acao" runat="server" Text="" Visible ="false" CssClass="label-danger"></asp:Label>
								</div>
							</div>

							<div class="row">
							<div class="col-lg-10">
								<asp:GridView  ID="dtgAcoes" class="table table-striped table-bordered table-hover table-condensed" 
								runat="server" Width="100%" CellSpacing="1"  CellPadding="1"   AutoGenerateColumns="False" GridLines="None"  
								ShowFooter="False"  Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgAcoes_RowDataBound"> 
 								<FooterStyle CssClass="TAB_Fundo_Azul" />
								<RowStyle CssClass="texto_padrao_preto" />
								<Columns>
									<asp:BoundField DataField="idRegistroAcao" HeaderText="idRegistroAcao" >
										<ItemStyle Width="20%"  HorizontalAlign="Left" VerticalAlign="Middle" />
									</asp:BoundField>
		                            <asp:BoundField DataField="idAcao" HeaderText="idAcao" >
										<ItemStyle Width="20%"  HorizontalAlign="Left" VerticalAlign="Middle" />
									</asp:BoundField>
									<asp:BoundField DataField="idUsuarioAcao" HeaderText="idUsuarioAcao" >
										<ItemStyle Width="20%"  HorizontalAlign="Left" VerticalAlign="Middle" />
									</asp:BoundField>
                					<asp:BoundField DataField="sDscAcao" HeaderText="Ação" >
										<ItemStyle Width="25%"  HorizontalAlign="Left" VerticalAlign="Middle" />
									</asp:BoundField>
									<asp:BoundField DataField="sObservacao" HeaderText="Observação" >
										<ItemStyle Width="20%"  HorizontalAlign="Left" VerticalAlign="Middle" />
									</asp:BoundField>
									<asp:BoundField DataField="sDscUsuarioAcao" HeaderText="Por" >
										<ItemStyle Width="20%"  HorizontalAlign="Left" VerticalAlign="Middle" />
									</asp:BoundField>
                                    <asp:BoundField DataField="dtEnvioEquipamento" HeaderText="Data Envio Equipamento" >
										<ItemStyle Width="20%"  HorizontalAlign="Left" VerticalAlign="Middle" />
									</asp:BoundField>
								</Columns>
								<FooterStyle CssClass="TAB_Fundo_Azul" />
								<RowStyle CssClass="texto_padrao_preto" />
								<PagerStyle BackColor="Black" ForeColor="Black" HorizontalAlign="Center" />
								<HeaderStyle CssClass="TAB_Fundo_Azul" />
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
                <asp:AsyncPostBackTrigger ControlID="cmdIncluirAcao" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
        <div class="panel panel-default" runat="server" id="div_Resolucao">
            <div class="panel-heading">
                <h3 class="panel-title">Resolução</h3>
            </div>
            <div class="panel-body">
                <div class="form-stacked row">
                    <div class="col-lg-6">
                        <div class="form-group">
                            <label>Data Resolução</label>
                            <asp:TextBox ID="txtdtResolucao" class="form-control uppercase CaixaTextoGigante " runat="server" MaxLength="200"></asp:TextBox>
                        </div>
                    </div>
					<div class="col-lg-6">
                        <div class="form-group">
                            <label>Usuário Resolução</label>
                            <asp:TextBox ID="txtsDscUsuarioResolucao" class="form-control uppercase CaixaTextoGigante " runat="server" MaxLength="200"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-lg-6" runat="server" id="div_SW_Resolucao">
                        <div class="row">
                            <div class="col-lg-3" >
                                <div class="form-group">
                                    <label>Código SW  </label>
                                    <asp:TextBox ID="txtsCodigoSW_Resolucao" class="form-control CaixaTextoMini " runat="server"  MaxLength="4" ></asp:TextBox>
                                </div>
                            </div>
                                <div class="col-lg-4" runat="server" id="Div3">
                                <div class="form-group">
                                    <label>dtEnvio SW  </label>
                                    <asp:TextBox ID="txtdtEnvioSW_Resolucao" class="form-control CaixaTextoMedio " runat="server"  ></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    
        <fieldset class="form-stacked actions">
            <asp:Button ID="cmdEnviar" class="btn  btn-lg btn-success" runat="server" Text="Enviar" />
            <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" " id="field-cancel" title="Voltar" onclick="history.go(-1)" >&nbsp;
        </fieldset>


        <div id="dialog-Salvar" title="Salvar">
            <p><span class="ui-icon ui-icon-alert" style="float:left; margin:12px 12px 20px 0;"></span><asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label></p>
        </div>
           

        <asp:HiddenField ID="hddidOcorrencia" runat="server" />

    </ContentTemplate>            
</asp:UpdatePanel>

</asp:Content>
