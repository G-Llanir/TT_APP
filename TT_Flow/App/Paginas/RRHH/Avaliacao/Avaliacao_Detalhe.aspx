<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Avaliacao_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.Avaliacao_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="content_frmAvaliacao_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
        <style>
        .modal-dialog {
            width: 60%;
        }

        .modal-content {
            display: flex;
            flex-direction: column;
            height: auto;
            /*overflow-y: auto;*/
        }

        .modal-body {
            max-height: 850px;
            flex: 1 1 auto;
        }

        .modal-largo {
            width: 80% !important;
            max-width: none !important;
        }

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
    </style>
    
             <asp:UpdatePanel ID="UpdatePanel1" runat="server">
             <ContentTemplate>  

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>
            </div>
            <div class="col-lg-12 row">
                <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
            </div>
            <div id="DIV_Body" runat="server">    
                <div class="form-stacked row">
               
                
                    <div class="col-lg-12">
                        <div class="form-group">
                            <label>ID</label>
                            <asp:TextBox ID="txtidAvaliacao" runat="server" class="form-control CaixaTextoMini" disabled=""></asp:TextBox>
                        </div>
                    </div>

                    <div class="col-lg-12 row">
                        <div class="form-group">
                            <div class="col-lg-2">
                                <div class="form-group">
                                    <label>Referência</label>
                                    <asp:TextBox ID="txtsReferencia" runat="server" class="form-control CaixaTextoPequeno" MaxLength="7" placeholder="MM/AAAA"></asp:TextBox>
                                </div> 
                            </div>
                         

                        </div>
                    </div>
                    <div class="col-lg-12 form-group row">
                        <div class="col-lg-5">
                            <div class="form-group">
                                <label>Descrição</label>
                                <asp:TextBox ID="txtsDscAvaliacao" runat="server" class="form-control " MaxLength="200" ></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-12 form-group row">
                        <div class="col-lg-6 form-group">
                            <label>Aplicar a Departamento</label>
                            <asp:ListBox runat="server" ID="lstsidDepartamento" SelectionMode="Multiple" CssClass="Caixa_Selecao"></asp:ListBox>
                        </div>
                    
                    </div>
                    <div class="col-lg-12">
                        <div class="form-group">
                            <uc1:SwitchAtivo ID="SwitchsPermiteAnonimo" runat="server" />
    <%--                            <uc1:ComboAtivo runat="server" ID="ddlsPermiteAnonimo"  />--%>
                        </div>
                    </div>

                    <div class="col-lg-10">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Perguntas</b></h3>
                            </div>
                            <div class="panel-body">
                                <div runat="server" id="div_IncluirPerguntas" class="row">
                                    <div class="col-lg-12">
                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_IncluirPerguntas" />
                                    
                                        <div class="col-lg-2 row" runat="server" id="div_cmdIncluirPergunta">
                                            <div class="form-group">
                                                <asp:Button runat="server" ID="cmdIncluirPergunta" class="btn  btn-info" Text="Adicionar Pergunta" OnClick="cmdIncluirPergunta_Click"></asp:Button>
                                            </div>
                                        </div>

                                    </div>
                                </div>

                                <uc1:MensagemPagina runat="server" ID="MensagemPagina_gvPergunta" />

                                <div runat="server" id="div_gvPergunta" class="col-lg-12 row">

                                    <asp:GridView ID="gvPergunta" class="table table-striped table-bordered table-hover "
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" DataKeyNames="idContador" OnRowDataBound="gvPergunta_RowDataBound" OnRowCommand="gvPergunta_RowCommand" OnRowDeleting="gvPergunta_RowDeleting">
                                        <Columns>
                                             <asp:BoundField DataField="sRespondidaPor" HeaderText="Respondida em">
                                                 <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                             </asp:BoundField>

                                             <asp:BoundField DataField="sGrupo" HeaderText="Grupo">
                                                 <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                             </asp:BoundField>

                                            <asp:BoundField DataField="sDscPergunta" HeaderText="Pergunta">
                                                 <ItemStyle Width="55%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                             </asp:BoundField>
                                            
                                            <asp:TemplateField>
                                              <ItemTemplate>
                                                  <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Alterar" CssClass="btn btn-small" CommandArgument='<%# Eval("idContador") %>'><i class='fa fa-pencil'></i></asp:LinkButton>
                                                  <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" CssClass="btn btn-small"><i class='fa-eraser fa'></i></asp:LinkButton>
                                              </ItemTemplate>
                                              <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10  %" />
                                          </asp:TemplateField>

                                        </Columns>
                                    </asp:GridView>

                                </div>
                            </div>
                        </div>
                    </div>

                </div>

                <div class="col-lg-12 form-group">
                    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                </div>
                <fieldset class="col-lg-12 form-stacked actions">
                    <asp:Button ID="cmdSalvarAvaliacao" class="btn btn-success" runat="server" Text="Salvar" />
                    <asp:Button ID="cmdSimular" class="btn btn-info" runat="server" Text="Simular Avaliação" OnClick="cmdSimular_Click" />
                    <asp:Button ID="cmdDuplicar" class="btn btn-warning" runat="server" Text="Duplicar Avaliação" />
                    <asp:Button ID="cmdExcluir" class="btn btn-danger" runat="server" Text="Excluir" />
                    <input type="submit" name="cancel" class="btn btn" id="field-cancel" value="Cancelar" title="Voltar" onclick="history.go(-1)" />
                </fieldset>
            </div>
            <asp:HiddenField ID="hddidAvaliacao" runat="server" />
            <asp:HiddenField ID="hddsidDepartamento" runat="server" />
            <asp:HiddenField ID="hddsChaveGUI" runat="server" />
        </ContentTemplate>
        </asp:UpdatePanel>

    <%--------------------------------------MODAL Perguntas---------------------------------------------------------------------------------------%>
    <div class="modal fade" id="modal_Perguntas" tabindex="-2" role="dialog" data-backdrop="static" aria-labelledby="modal_Perguntas" aria-hidden="true">
        <div class="modal-dialog modal-sm" role="document" style="width: 60%">
     <div class="modal-content">
         <asp:UpdatePanel ID="updModalDetalhe" runat="server">
             <ContentTemplate>  
                 <div class="modal-header">
                     <button type="button" id="btnHeadFecharDetalhe" class="close" data-dismiss="modal" aria-label="Fechar">
                         <span aria-hidden="true">&times;</span>
                     </button>
                     <div class="modal-header-content">

                         <div class="modal-logo">
                             <asp:Image ID="Image2" runat="server" ImageUrl="~/App/img/instrucaoTecnica_pdf.jpg" AlternateText="LogoTipo" />
                         </div>

                         <div class="modal-title-container">
                             <asp:Label runat="server" class="modal-title" ID="lblTituloModal" Text="Detalhe" Font-Bold="true"></asp:Label>
                         </div>

                     </div>
                     <div class="modal-body">

                    <div class="panel panel-default" runat="server" id="div_detalhe">
                        <div class="panel-body ">
                            <div class="col-lg-12 row" style="width: 100%; margin-left: 0px; padding: 0px;">
                                <uc1:MensagemPagina runat="server" ID="MensagemPaginaModalDetalhe" style="margin-left: 0px !important" />
                            </div>

                            <div class="form-group row">
                            <div class="col-lg-12 form-group">
                                    <div class="col-lg-12">
                                    <label>Pergunta</label>
                                        <asp:TextBox ID="txtsDscPergunta" class="form-control" TextMode="MultiLine" MaxLength="800" Height="80px" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                  
                            <div class="col-lg-12 form-group">
                                <div class="col-lg-6">
                                    <label>Grupo</label>
                                    <asp:TextBox ID="txtsGrupo" class="form-control" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-lg-6">
                                    <label>Respondida Por</label>
                                    <asp:DropDownList runat="server" ID="ddlsRespondidaPor" class="form-control Caixa_Selecao">
                                        <asp:ListItem Text="Selecione quem responde" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="01 - Colaborador (AAP)" Value="AAP" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="02 - Supervisor (ASU)" Value="ASU"></asp:ListItem>
                                        <asp:ListItem Text="03 - Diretoria (ASD)" Value="ASD"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                                
                            <div class="col-lg-12 form-group">         
                                <div class="col-lg-6">
                                        <label>Tipo</label>
                                        <asp:DropDownList runat="server" ID="ddlsTipo" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlsTipo_SelectedIndexChanged">
                                            <asp:ListItem Text="Selecione um tipo (formato da pergunta)" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Caixa de texto" Value="T"></asp:ListItem>
                                            <asp:ListItem Text="Escolha de Sim ou Não" Value="S"></asp:ListItem>
                                            <asp:ListItem Text="Escala de números" Value="N"></asp:ListItem>
                                            <asp:ListItem Text="Lista com ÚNICA escolha" Value="E"></asp:ListItem>
                                            <asp:ListItem Text="Lista com MÚLTIPLAS escolhas" Value="M"></asp:ListItem>
                                            <asp:ListItem Text="Resposta Automática via T-Flow" Value="F"></asp:ListItem>
                                        </asp:DropDownList>
                                </div>
                                <div class="col-lg-6" runat="server" id="DIV_Perguntas_Descreva">
                                    <uc1:SwitchAtivo ID="switchsCaixadeObservacao"   runat="server" />

                                </div>
                            </div>
                                        
                        </div>
                            <div class="col-lg-6 form-group" runat="server" id="DIV_Perguntas_Opcoes">  
                                <div class="panel panel-default" >
                                    <div class="panel-heading">
                                        <h3 class="panel-title"><b>Opções de Resposta</b></h3>
                                    </div>
                                    <div class="panel-body">
                                        <div id="DIV_RESPOSTAS_OPCOES" runat="server">
                                        <div class="form-group row">
                                            <div class="col-lg-12">
                                                <asp:Button ID="cmdIncluirOpcao" class="btn btn-info" runat="server" Text="Adicionar Opção" OnClick="cmdIncluirOpcao_Click" />
                                            </div>
                                        </div>
                                        <div class="form-group row">
                                            <div class="col-lg-12">
                                                <asp:GridView ID="gvPerguntas_Opcoes" class="table table-striped table-bordered table-hover table-condensed"
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" DataKeyNames="Key" 
                                                    OnRowDeleting="gvPerguntas_Opcoes_RowDeleting" OnRowDataBound="gvPerguntas_Opcoes_RowDataBound">
                                                    <Columns>
                                                       <asp:TemplateField HeaderText="Ordem">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="txtnOrdem" runat="server" class="form-control" Text='<%# Eval("Value") %>' ClientIDMode="Static"></asp:TextBox>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Opção">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="txtsDescricao" runat="server" class="form-control" Text='<%# Eval("Value") %>' MaxLength="200" ClientIDMode="Static"></asp:TextBox>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="80%" />
                                                        </asp:TemplateField>

                                                         <asp:TemplateField HeaderText="Justificativa">
                                                             <ItemTemplate>
                                                                 <asp:DropDownList runat="server" ID="ddlsJustificativa" class="form-control Caixa_Selecao">
                                                                     <asp:ListItem Text="Não" Value="N"></asp:ListItem>
                                                                     <asp:ListItem Text="Sim" Value="S"></asp:ListItem>
                                                                 </asp:DropDownList>    
                                                             </ItemTemplate>
                                                             <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="80%" />
                                                         </asp:TemplateField>
                                                                   
                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="cmdOpcoes_Excluir" runat="server" CssClass="btn btn-small" TabIndex="100" ToolTip="Apagar Registro" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                        </div>
                                        <div id="DIV_RESPOSTAS_ESCALA" runat="server">
                                           <div class="form-group row col-lg-12">
                                                <div class="col-lg-2">
                                                    <label>De:</label>
                                                </div>
                                                    <div class="col-lg-3">
                                                    <asp:TextBox ID="txtnEscala_DE" runat="server" class="form-control"  ClientIDMode="Static"></asp:TextBox>
                                                </div>
                                                <div class="col-lg-2">
                                                     <label>Ate:</label>
                                                 </div>
                                                     <div class="col-lg-3">
                                                     <asp:TextBox ID="txtnEscala_Ate" runat="server" class="form-control"  ClientIDMode="Static"></asp:TextBox>
                                                 </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                           
                        <div class="modal-footer">
                            <asp:Button ID="cmdSalvarPergunta" CssClass="btn-success btn" runat="server" Text="Salvar" OnClick="cmdSalvarPergunta_Click"/>
                        </div>

                        <asp:HiddenField runat="server" ID="hddidContador" Value="" />
                    </div>
                 
                </div>
    
        </ContentTemplate>
        </asp:UpdatePanel>
        
        </div>
        </div>
    </div>

     <div id="dialog-Salvar" class="modal" title="Salvar">
     <p>
         <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
         <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
     </p>
    </div>
    
    <div id="dialog-Duplicar" class="modal" title="Duplicar">
     <p>
         <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
         <asp:Label ID="lblTituloDuplicar" runat="server" Text=""></asp:Label>
     </p>
    </div>

        <div id="dialog-Excluir" class="modal" title="Excluir">
     <p>
         <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
         <asp:Label ID="lblTitulosExcluir" runat="server" Text=""></asp:Label>
     </p>
    </div>

</asp:Content>
