<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProdutosImportador.ascx.cs" Inherits="TT_Flow.App.Controles.ProdutosImportador" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


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
                fill: rgb(82, 82, 82);
                margin-bottom: 20px;
           
    }

        .file-upload-label {
                cursor: pointer;
                background-color: #ddd;
                padding: 30px 70px;
                border-radius: 40px;
                border: 2px dashed rgb(82, 82, 82);
                box-shadow: 0px 0px 20px -50px rgba(0, 0, 0, 0.719);
                display: flex;
                flex-direction: column;
                align-items: center;
                justify-content: center;
                gap: 5px;
                text-align: center;
           
    }

     



      
        .file-upload-label.dragover {
                border-color: #000;
                background-color: #e6f7ff;
           
    }

        .browse-button {
                background-color: rgb(82, 82, 82);
                padding: 5px 15px;
                border-radius: 10px;
                color: white;
                transition: all 0.3s;
           
    }

     



      
        .browse-button:hover {
                background-color: rgb(14, 14, 14);
           
    }

        .file-name {
                margin-top: 10px;
                font-size: 14px;
                color: rgb(82, 82, 82);
           
    }

    /* Estilos para mensagens */
    .message-container {
        margin-top: 20px;
        text-align: center;
    }

    .message-success {
        color: green;
        font-weight: bold;
    }

    .message-error {
        color: red;
        font-weight: bold;
    }
</style>

<div class="col-lg-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            <h3 class="panel-title"><b>Importar EAN de Produtos (Excel)</b></h3>
        </div>
        <div class="panel-body" id="divPnBody" runat="server">
            <div class="form-stacked row">
                <div class="row">
                    <div class="col-lg-12">
                        <div class="form-stacked">
                            <div class="col-lg-12">
                                <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                            </div>
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

                                            <%-- O FileUpload foi movido PARA DENTRO do controle --%>
                                            <asp:FileUpload ID="fuEANExcel" runat="server" />
                                        </div>
                                    </div>
                                    <div class="file-name" id="fileName"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <%-- Botão de importação movido PARA DENTRO do controle --%>
                <div class="col-lg-12 text-center mt-3" style="margin-top: 20px;">
                    <asp:Button ID="btnIniciarImportacao" runat="server" Text="Iniciar Importação" OnClick="btnIniciarImportacao_Click" CssClass="btn btn-primary" />
                </div>

                <hr style="margin-top: 20px; margin-bottom: 20px; border-top: 1px solid #eee;" />

                <%-- Mensagens e GridView para exibir resultados movidos PARA DENTRO do controle --%>
                <div class="col-lg-12 message-container">
                    <asp:Label ID="lblMensagemSucesso" runat="server" Text="" CssClass="message-success"></asp:Label><br />
                    <asp:Label ID="lblMensagemErro" runat="server" Text="" CssClass="message-error"></asp:Label>
                </div>

                <div class="col-lg-12">
                    <asp:GridView ID="gvResultadosImportacao" runat="server" CssClass="table table-bordered table-striped mt-4" AutoGenerateColumns="True" EmptyDataText="Nenhum resultado para exibir."></asp:GridView>
                </div>

            </div>
        </div>
    </div>
</div>
