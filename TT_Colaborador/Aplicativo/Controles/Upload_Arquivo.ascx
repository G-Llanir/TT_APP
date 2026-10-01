<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Upload_Arquivo.ascx.cs" Inherits="TT_Colaborador.Aplicativo.Controles.Upload_Arquivo" %>

<style>
    .upload-container {
        max-width: 100%;
        width: fit-content;
        height: fit-content;
        display: flex;
        align-items: center;
        justify-content: center;
        margin: 0 auto;
    }

    .file-upload-label p {
        margin-bottom: .5em;
    }

    .file-upload-label input {
        display: none;
    }

    .file-upload-label {
        cursor: pointer;
        background-color: #dff0d8;
        padding: 1.75em 3.5em;
        border-radius: 40px;
        border: 2px dashed rgb(0, 125, 0);
        box-shadow: 0px 0px 20px -50px rgba(0, 0, 0, 0.719);
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        gap: 5px;
        text-align: center;
        color: rgb(0, 125, 0);
    }

        .file-upload-label.dragover {
            border-color: #000;
            background-color: #e6f7ff;
        }

    .file-name {
        margin-top: .5em;
        font-size: 1em;
        color: rgb(0, 75, 0);
    }

    @media only screen and (max-device-width: 768px) {
        .file-name {
            font-size: 2em;
        }
    }
</style>

<asp:HiddenField runat="server" ID="hddFileUpload_ID" Value="FileUpload" />

<div class="col-12 padd-0">
    <div class="upload-container">
        <div runat="server" id="uploadContainer" class="file-upload-label">
            <h1><i class="fa fa-2x fa-cloud-arrow-up"></i></h1>
            <h4>Arraste e solte aqui o Arquivo</h4>
            <h4>ou</h4>
            <h2><span class="browse-button badge rounded-pill text-bg-success">Escolher Arquivo</span></h2>
        </div>
    </div>
    <div runat="server" id="fileName" class="file-name text-center"></div>
</div>