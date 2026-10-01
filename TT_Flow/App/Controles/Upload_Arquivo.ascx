<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Upload_Arquivo.ascx.cs" Inherits="TT_Flow.App.Controles.Upload_Arquivo" %>
<div class="form-group">
    <label id="container-UploadArquivo">
        <asp:FileUpload ID="FileUpload" runat="server" />
        &nbsp;
        <asp:Button ID="cmdEnviar_Arquivo" runat="server" class="btn primary" OnClick="cmdEnviar_Arquivo_Click" Text="Enviar Arquivo" title="Enviar arquivo" TabIndex="20" />
    </label>
</div>