<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="TipoArquivo_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.TipoArquivo_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>


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
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Tipo de Arquivo"></asp:Label></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>
            </div>

            <div class="panel panel-default" runat="server" id="Div1">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Dados do Tipo de Arquivo</b></h3>
                </div>
                <div class="panel-body">

                    <div class="form-stacked row">
                        <div class="col-lg-12">
                            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                        </div>
                    </div>

                    <div class="form-stacked row">
                        <div class="col-lg-12">
                            <div class="form-group">
                                <label>ID </label>
                                <asp:TextBox ID="txtidControle" class="form-control CaixaTextoMini" runat="server" disabled="true"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="form-stacked row">

                        <div class="col-lg-2">
                            <div class="form-group">
                                <label>Tipo de Arquivo</label>
                                <asp:TextBox ID="txtsTipoObjeto" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-5">
                            <div class="form-group">
                                <label>Descrição</label>
                                <asp:TextBox ID="txtsDscTipoArquivo" runat="server" class="form-control"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-5">
                            <div class="form-group">
                                <label>Recurso</label>
                                <asp:DropDownList ID="ddlidRecurso" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-12 row">
                            <div class="col-lg-3">
                                <div class="form-group">
                                    <label>Extensões Arquivo (Opcional)</label>
                                    <asp:ListBox ID="ddlsExtensoes" runat="server" class="form-control yes_no select Caixa_Selecao" SelectionMode="Multiple">
                                        <asp:ListItem Text=".doc (Documento Word)" Value=".doc" />
                                        <asp:ListItem Text=".docx (Documento Word)" Value=".docx" />
                                        <asp:ListItem Text=".pdf (Documento PDF)" Value=".pdf" />
                                        <asp:ListItem Text=".txt (Texto simples)" Value=".txt" />
                                        <asp:ListItem Text=".xls (Planilha Excel)" Value=".xls" />
                                        <asp:ListItem Text=".xlsx (Planilha Excel)" Value=".xlsx" />
                                        <asp:ListItem Text=".ppt (Apresentação PowerPoint)" Value=".ppt" />
                                        <asp:ListItem Text=".pptx (Apresentação PowerPoint)" Value=".pptx" />

                                        <asp:ListItem Text=".jpg (Imagem JPEG)" Value=".jpg" />
                                        <asp:ListItem Text=".jpeg (Imagem JPEG)" Value=".jpeg" />
                                        <asp:ListItem Text=".png (Imagem PNG)" Value=".png" />
                                        <asp:ListItem Text=".gif (Imagem GIF)" Value=".gif" />
                                        <asp:ListItem Text=".bmp (Imagem Bitmap)" Value=".bmp" />
                                        <asp:ListItem Text=".svg (Imagem SVG)" Value=".svg" />

                                        <asp:ListItem Text=".mp3 (Áudio MP3)" Value=".mp3" />

                                        <asp:ListItem Text=".mp4 (Vídeo MP4)" Value=".mp4" />
                                        <asp:ListItem Text=".avi (Vídeo AVI)" Value=".avi" />
                                        <asp:ListItem Text=".mkv (Vídeo MKV)" Value=".mkv" />
                                        <asp:ListItem Text=".mov (Vídeo MOV)" Value=".mov" />
                                        <asp:ListItem Text=".wmv (Vídeo WMV)" Value=".wmv" />

                                        <asp:ListItem Text=".zip (Arquivo Compactado ZIP)" Value=".zip" />
                                        <asp:ListItem Text=".rar (Arquivo Compactado RAR)" Value=".rar" />
                                        <asp:ListItem Text=".7z (Arquivo Compactado 7-Zip)" Value=".7z" />

                                        <asp:ListItem Text=".xml (Arquivo XML)" Value=".xml" />
                                    </asp:ListBox>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-12" id="div_swtsPermiteSobrescrever" runat="server">
                            <uc1:SwitchAtivo ID="swtsPermiteSobrescrever" runat="server" />
                        </div>

                        <div class="col-lg-12" id="div2" runat="server">
                            <uc1:SwitchAtivo ID="swtsValidacaoObrigatario" runat="server" />
                        </div>

                    </div>
                </div>
            </div>

            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                <a href="javascript:history.back()" class="btn btn-lg btn-warning" title="Voltar">Voltar</a>
            </fieldset>

            <asp:HiddenField ID="hddidControle" runat="server" />

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
