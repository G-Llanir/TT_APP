<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UploadArquivo_Padrao.ascx.cs" Inherits="TT_Flow.App.Controles.UploadArquivo_Padrao" %>
 <%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
 
    <div class="panel panel-default" id="DIV_Arquivos" runat="server">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Arquivos</b></h3>
                </div>
                    
                <div class="panel-body">
                    <div class="form-stacked row">
                    <div class="col-lg-12 table-responsive">
                    <asp:GridView ID="gv_Arquivo" class="table table-striped table-bordered table-hover table-condensed table-responsive" 
                    runat="server" Width="100%" CellSpacing="1"  CellPadding="1"   AutoGenerateColumns="False" GridLines="None"  
                        ShowFooter="False" OnRowCommand="gv_Arquivo_RowCommand" OnRowDataBound="gv_Arquivo_RowDataBound" >
                        <Columns>
                            <asp:BoundField DataField="idArquivo" HeaderText="ID" >
                                <ItemStyle Width="0%"  HorizontalAlign="Center" VerticalAlign="Middle" />
                            </asp:BoundField>
                            <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Inclusão" >
                                <ItemStyle Width="15%" HorizontalAlign="left" VerticalAlign="Middle" />
                            </asp:BoundField>

                            <asp:ButtonField DataTextField="sNomeArquivo"  HeaderText ="Nome do Arquivo" CommandName="Download" ItemStyle-Width="20%" />

                            <asp:BoundField DataField="sDscArquivo" HeaderText="Descrição" >
                                <ItemStyle Width="40%" HorizontalAlign="left" VerticalAlign="Middle" />
                            </asp:BoundField>
                                    
                            <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Usuário" >
                                <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                            </asp:BoundField>
                        </Columns>
                    </asp:GridView>
                    
                    </div>
                    </div>
                    <div id="div_EnviarArquivos" class="form-stacked " runat="server">
                        <div class="panel panel-default" runat="server">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Enviar Arquivos</b></h3>
                        </div>
                        <uc1:MensagemPagina runat="server" id="MensagemArquivo" />
                        <div class="panel-body">
                            <div class="form-stacked row">

                            <div class="col-lg-6" id="div_EnviarArquivos_Selecao" runat="server">
                                <div class="form-group">
                                    <label>Selecione o Arquivo</label>
                                    <asp:FileUpload ID="fu_Arquivo" class="form-control-file"  runat="server" Width="400px" />
                                </div>
                            </div>
                            <div class="col-lg-12">
                                <div class="form-group">
                                    <label><asp:Label ID="lblEnviarArquivo_sDscArquivo" runat="server" Text="Descrição"></asp:Label></label>
                                    <asp:TextBox ID="txtEnviarArquivo_sDscArquivo" class="form-control CaixaTextoGigante" runat="server" MaxLength="300"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-lg-12">
                                <div class="form-group">
                                    <asp:Button ID="cmdEnviarArquivos" CssClass="btn-success btn" runat="server" Text="Enviar" OnClick="cmdEnviarArquivos_Click" />
                                </div>
                            </div>
                            </div>
                        </div>
                        </div>
                    </div>
                </div>

            </div>
