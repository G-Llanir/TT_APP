<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExcelImportar.ascx.cs" Inherits="TT_Flow.App.Controles.ExcelImportar" %>

<style>
    .upload-container {
        width: fit-content;
        height: fit-content;
        display: flex;
        align-items: center;
        justify-content: center;
        margin: 0 auto;
    }

    .file-upload-label input {
        display: none;
    }

    .file-upload-label svg {
        height: 50px;
        fill: rgb(0, 125, 0);
        margin-bottom: 20px;
    }

    .file-upload-label {
        cursor: pointer;
        background-color: #dff0d8;
        padding: 30px 70px;
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

    .browse-button {
        background-color: rgb(0, 125, 0);
        padding: 5px 15px;
        border-radius: 10px;
        color: white;
        transition: all 0.3s;
    }

        .browse-button:hover {
            background-color: rgb(0, 75, 0);
        }

    .file-name {
        margin-top: 10px;
        font-size: 14px;
        color: rgb(0, 75, 0);
    }
</style>

<asp:HiddenField runat="server" ID="hddFileUpload_ID" Value="FileUpload" />

<div class="col-lg-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            <h3 class="panel-title"><b>Upload de Arquivo</b></h3>
        </div>
        <div class="panel-body">
            <div class="form-stacked row">

                <div class="row">
                    <div class="col-lg-12">
                        <div class="form-stacked">
                            <div class="col-lg-12" id="div1" runat="server" style="text-align: center;">
                                <div class="form-group" style="display: inline-block">
                                    <div class="upload-container">
                                        <div class="file-upload-label" id="uploadContainer">
                                            <svg viewBox="0 0 640 512" height="1em">
                                                <path
                                                    d="M144 480C64.5 480 0 415.5 0 336c0-62.8 40.2-116.2 96.2-135.9c-.1-2.7-.2-5.4-.2-8.1c0-88.4 71.6-160 160-160c59.3 0 111 32.2 138.7 80.2C409.9 102 428.3 96 448 96c53 0 96 43 96 96c0 12.2-2.3 23.8-6.4 34.6C596 238.4 640 290.1 640 352c0 70.7-57.3 128-128 128H144zm79-217c-9.4 9.4-9.4 24.6 0 33.9s24.6 9.4 33.9 0l39-39V392c0 13.3 10.7 24 24 24s24-10.7 24-24V257.9l39 39c9.4 9.4 24.6 9.4 33.9 0s9.4-24.6 0-33.9l-80-80c-9.4-9.4-24.6-9.4-33.9 0l-80 80z">
                                                </path>
                                            </svg>
                                            <p>Arraste e solte aqui o Arquivo</p>
                                            <p>ou</p>
                                            <span class="browse-button">Escolher Arquivo</span>
                                            <%--<asp:FileUpload ID="fu_EnviarArquivo" runat="server" Style="display: none;" />--%>
                                        </div>
                                    </div>
                                    <div class="file-name" id="fileName"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>
</div>
