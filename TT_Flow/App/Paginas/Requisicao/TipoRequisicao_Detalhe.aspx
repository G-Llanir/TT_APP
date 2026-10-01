<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="TipoRequisicao_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Requisicao.TipoRequisicao_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>





<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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


                <div class="col-lg12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                </div>

                <div class="col-lg-12">
                    <div class="form-stacked row">
                        <div class="col-lg-12">
                            <div class="form-group">
                                <label>ID </label>
                                <asp:TextBox ID="txtidTipoRequisicao" class="form-control CaixaTextoMini" runat="server" disabled="true"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-5">
                            <div class="form-group">
                                <label>Descrição</label>
                                <asp:TextBox ID="txtsDscTipoRequisicao" class="form-control" runat="server" MaxLength="50"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-3">
                            <div class="form-group">
                                <label>Tipo</label>
                                <asp:TextBox ID="txtsTipoRequisicao" class="form-control" runat="server" MaxLength="20"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-lg-12 row">
                        
                            <div class="col-lg-4">
                                <div class="form-group">
                                    <label>Departamento Responsável pela segunda aprovação</label>
                                    <asp:DropDownList ID="ddlidDepartamento" class="form-control Caixa_Selecao" runat="server" ></asp:DropDownList>
                                </div>
                            </div>
                        </div> 
                        <div class="col-lg-12">
                             <uc1:SwitchAtivo runat="server" ID="SwitchAtivo" ClientIDMode="Static" OnClientStateChanged="toggleGarantiaDiv"/>
                         </div>

                         <div class="col-lg-12">
                             <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
                         </div>
                       
                    </div>
                </div>
            </div>
            <br />

            <div class="col-lg-12">
                <div class="form-stacked row">
                    <asp:UpdatePanel ID="updPanel_Acoes" runat="server">
                        <ContentTemplate>
                            <div class="panel panel-default col-lg-8" runat="server" id="Div_Acoes">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>Membros</b></h3>
                                </div>
                                <div class="panel-body">
                                    <uc1:MensagemPagina runat="server" ID="MensagemAcoes" />
                                    <div class="row">
                                        <div class="col-lg-12">
                                            <div class="form-group">
                                                <div class="row" runat="server" id="Div_Selecao">
                                                    <div class="col-lg-10">
                                                        <div class="form-group">
                                                            <label>Usuário </label>
                                                            <asp:DropDownList ID="ddlUsuarios" runat="server" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-10">
                                                       <div class="form-group">
                                                           <label>Tipo de Usuário </label>
                                                           <asp:DropDownList ID="ddlTipoMembros" runat="server" class="form-control yes_no select">
                                                                <asp:ListItem Value="0" Text="Selecione um Tipo" />
                                                               <asp:ListItem Value="1" Text="Representantes - Adms" />
                                                               <asp:ListItem Value="2" Text="Executores - Membros" />       
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
                                                        <asp:GridView ID="dtgSelecao" class="table table-striped table-bordered table-hover table-condensed"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgSelecao_RowDataBound" OnRowDeleting="dtgSelecao_RowDeleting">
                                                            <Columns>
                                                                <asp:BoundField DataField="idUsuario" HeaderText="idUsuario">
                                                                    <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário">
                                                                    <ItemStyle Width="50%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>
                                                                 <asp:BoundField DataField="IdTipoMembro" HeaderText="Tipo Membro" Visible="false">
                                                                    <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                 <asp:TemplateField HeaderText="Tipo">
                                                                   <ItemTemplate>
                                                                      <asp:DropDownList ID="ddlTipoMembros" runat="server" CssClass="form-control yes_no select"
                                                                          SelectedValue='<%# Bind("IdTipoMembro") %>'>                                                               
                                                                          <asp:ListItem Value="0" Text="Selecione um Tipo" />
                                                                          <asp:ListItem Value="1" Text="Representantes - Adms" />
                                                                          <asp:ListItem Value="2" Text="Executores - Membros" />
                                                                      </asp:DropDownList>
                                                                   </ItemTemplate>                                                        
                                                                   <ItemStyle Width="50%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                               </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="20%" HorizontalAlign="Center" VerticalAlign="Middle" />
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
            <br />

            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />


            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />

                <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" " id="field-cancel" title="Voltar" onclick="history.go(-1)" ></input>
            </fieldset>

            <asp:HiddenField ID="hddidTipoRequisicao" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
