<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LeitorAtividade.aspx.cs" Inherits="TT_Flow.App.Paginas.LeitorAtividade" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Apontamento de Atividade</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />

    <script src="https://unpkg.com/html5-qrcode@2.3.8/html5-qrcode.min.js"></script>

    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f0f2f5;
            margin: 0;
            padding: 0;
            display: flex;
            flex-direction: column;
            align-items: center;
            min-height: 100vh;
            color: #333;
        }

        .container {
            background-color: #ffffff;
            margin: 20px;
            padding: 20px;
            border-radius: 12px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
            width: 90%;
            max-width: 500px;
            text-align: center;
            box-sizing: border-box;
        }

        h1 {
            color: #333;
            margin-bottom: 25px;
            font-size: 24px;
            font-weight: 600;
        }

        h2 {
            color: #555;
            margin-top: 30px;
            margin-bottom: 20px;
            font-size: 20px;
            font-weight: 500;
        }

        .button-scanner {
            background-color: #007bff;
            color: white;
            border: none;
            padding: 12px 25px;
            border-radius: 8px;
            font-size: 18px;
            cursor: pointer;
            transition: background-color 0.3s ease;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 10px;
            width: 100%;
            box-sizing: border-box;
            font-weight: 500;
        }

            .button-scanner:hover {
                background-color: #0056b3;
            }

        #reader {
            width: 100%;
            max-width: 300px;
            margin: 25px auto;
            border: 2px solid #ddd;
            border-radius: 8px;
            overflow: hidden;
        }

        /* Estilo para o qrCodeInput e outros campos de formulário */
        .form-control {
            width: calc(100% - 20px); /* Ajuste para padding */
            padding: 12px;
            margin-top: 20px;
            border: 1px solid #ccc;
            border-radius: 8px;
            font-size: 16px;
            text-align: center; /* Manter centralizado para o QR input */
            background-color: #ffffff;
            box-sizing: border-box;
        }

        .input-group {
            margin-bottom: 15px;
            text-align: left;
        }

            .input-group label {
                display: block;
                margin-bottom: 5px;
                font-weight: 500;
                color: #555;
            }

            .input-group .form-control {
                width: 100%; /* Overwrite for general form controls */
                padding: 10px;
                border: 1px solid #ccc;
                border-radius: 8px;
                font-size: 16px;
                box-sizing: border-box;
                text-align: left; /* Default text alignment for form inputs */
            }

            .input-group textarea.form-control {
                min-height: 80px;
                resize: vertical;
            }

        .activity-details {
            margin-top: 30px;
            padding: 20px;
            background-color: #e9f7ef;
            border: 1px solid #c3e6cb;
            border-radius: 8px;
            text-align: left;
        }

            .activity-details h3 {
                color: #28a745;
                margin-bottom: 15px;
                font-size: 20px;
                font-weight: 600;
            }

        .activity-detail-item {
            margin-bottom: 8px;
            color: #495057;
            line-height: 1.5;
            font-size: 15px;
            display: grid;
            gap: 5px;
            align-items: start;
        }

            .activity-detail-item strong {
                color: #333;
                font-weight: 600;
                white-space: nowrap;
            }

        .error-message {
            color: #dc3545;
            background-color: #f8d7da;
            border: 1px solid #f5c6cb;
            padding: 10px;
            border-radius: 8px;
            margin-top: 15px;
            font-weight: bold;
        }

        .success-message {
            color: #28a745;
            background-color: #d8f8d7;
            border: 1px solid #c6f5c7;
            padding: 10px;
            border-radius: 8px;
            margin-top: 15px;
            font-weight: bold;
        }

        .loading-message {
            color: #007bff;
            background-color: #e0f2f7;
            border: 1px solid #b3e0ff;
            padding: 10px;
            border-radius: 8px;
            margin-top: 15px;
            font-weight: bold;
        }

        .button-submit {
            background-color: #28a745;
            color: white;
            border: none;
            padding: 12px 25px;
            border-radius: 8px;
            font-size: 18px;
            cursor: pointer;
            transition: background-color 0.3s ease;
            margin-top: 20px;
            width: 100%;
            box-sizing: border-box;
            font-weight: 500;
        }

            .button-submit:hover {
                background-color: #218838;
            }

        @media (max-width: 600px) {
            .container {
                width: 95%;
                margin: 15px;
                padding: 15px;
            }

            h1 {
                font-size: 22px;
            }

            h2 {
                font-size: 18px;
            }

            .button-scanner {
                font-size: 16px;
                padding: 10px 20px;
            }

            .form-control,
            .input-group .form-control {
                padding: 10px;
                font-size: 14px;
            }

            .activity-details h3 {
                font-size: 18px;
            }

            .activity-detail-item {
                font-size: 14px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:HiddenField ID="hdnQRCode" runat="server" OnValueChanged="hdnQRCode_ValueChanged" />
        <asp:HiddenField ID="hdnActivityId" runat="server" />
        <asp:HiddenField ID="hdnActivityUuid" runat="server" />

        <div class="container">
            <h1>Apontamento de Atividade via QR Code</h1>

            <asp:UpdatePanel ID="updContent" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="pnlLoadingMessage" runat="server" CssClass="loading-message" Visible="false">
                        Carregando informações da atividade...
                    </asp:Panel>
                    <asp:Panel ID="pnlErrorMessage" runat="server" CssClass="error-message" Visible="false">
                        <asp:Literal ID="litErrorMessage" runat="server"></asp:Literal>
                    </asp:Panel>

                    <div id="activityDetails" runat="server" class="activity-details">
                        <h3>Detalhes da Atividade</h3>

                        <asp:Panel ID="pnlProjectTitle" runat="server" CssClass="activity-detail-item" Visible="false">
                            <strong>Projeto:</strong>
                            <asp:Literal ID="litProjectTitle" runat="server"></asp:Literal>
                        </asp:Panel>

                        <asp:Panel ID="pnlRequestNumber" runat="server" CssClass="activity-detail-item" Visible="false">
                            <strong>Nº Requisição:</strong>
                            <asp:Literal ID="litRequestNumber" runat="server"></asp:Literal>
                        </asp:Panel>

                        <asp:Panel ID="pnlParentActivityTitle" runat="server" CssClass="activity-detail-item" Visible="false">
                            <strong>Tarefa:</strong>
                            <asp:Literal ID="litParentActivityTitle" runat="server"></asp:Literal>
                        </asp:Panel>

                        <asp:Panel ID="pnlActivityTitle" runat="server" CssClass="activity-detail-item" Visible="false">
                            <strong>Descrição:</strong>
                            <asp:Literal ID="litActivityTitle" runat="server"></asp:Literal>
                        </asp:Panel>

                        <asp:Panel ID="pnlActivityStatus" runat="server" CssClass="activity-detail-item" Visible="false">
                            <strong>Status:</strong>
                            <asp:Literal ID="litActivityStatus" runat="server"></asp:Literal>
                        </asp:Panel>

                    </div>


                    <asp:Panel ID="pnFinalizar" runat="server" Visible="false">
                        <hr style="margin: 30px 0; border: 0; border-top: 1px solid #eee;" />
                        <asp:CheckBox ID="chkFinalizar" Text="Finalizar Atividade?" runat="server" />
                    </asp:Panel>

                    <hr style="margin: 30px 0; border: 0; border-top: 1px solid #eee;" />
                    
                    <asp:Panel ID="divApontador" runat="server">
                        <h2>Leitor de QR Code de Usuário</h2>
                        <button type="button" class="button-scanner" id="btnScannerAction" onclick="startScanner()">
                            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" fill="currentColor" class="bi bi-qr-code-scan" viewBox="0 0 16 16">
                                <path d="M0 .5A.5.5 0 0 1 .5 0h3a.5.5 0 0 1 0 1H1v2.5a.5.5 0 0 1-1 0zm12 0a.5.5 0 0 1 .5-.5h3a.5.5 0 0 1 0 1V3h-2.5a.5.5 0 0 1-.5-.5M.5 16a.5.5 0 0 1-.5-.5v-3a.5.5 0 0 1 1 0V15h2.5a.5.5 0 0 1 0 1zm15 .5a.5.5 0 0 1-.5.5h-3a.5.5 0 0 1 0-1H15v-2.5a.5.5 0 0 1 1 0zM11.5 4a.5.5 0 0 0-1 0v.5H10a.5.5 0 0 0-.5.5V6h-.5a.5.5 0 0 0-.5.5v.5H8a.5.5 0 0 0-.5.5v.5H7a.5.5 0 0 0-.5.5V9h-.5a.5.5 0 0 0-.5.5v.5H4a.5.5 0 0 0-.5.5V11a.5.5 0 0 0 .5.5h1.5a.5.5 0 0 0 .5-.5v-1.5h.5a.5.5 0 0 0 .5-.5V9h.5a.5.5 0 0 0 .5-.5V8h.5a.5.5 0 0 0 .5-.5V7h.5a.5.5 0 0 0 .5-.5V6h.5a.5.5 0 0 0 .5-.5V5h.5a.5.5 0 0 0 .5-.5zm0 1h-1.5a.5.5 0 0 0-.5.5v1.5h-.5a.5.5 0 0 0-.5.5V10h-.5a.5.5 0 0 0-.5.5v1.5H5v-.5a.5.5 0 0 0-.5-.5H3V7.5a.5.5 0 0 0 .5-.5H5V5.5a.5.5 0 0 0 .5-.5h1.5a.5.5 0 0 0 .5-.5V4h1.5a.5.5 0 0 0 .5-.5V3h-.5a.5.5 0 0 0-.5-.5H8a.5.5 0 0 0-.5.5V4h-.5a.5.5 0 0 0-.5.5V5h-.5a.5.5 0 0 0-.5.5V6h-.5a.5.5 0 0 0-.5.5V7H4v-.5a.5.5 0 0 0-.5-.5H2.5a.5.5 0 0 0-.5.5v2.5a.5.5 0 0 0 .5.5H4v.5a.5.5 0 0 0 .5.5H7v.5a.5.5 0 0 0 .5.5h1.5a.5.5 0 0 0 .5-.5V12h1.5a.5.5 0 0 0 .5-.5V9.5a.5.5 0 0 0-.5-.5H11V7.5a.5.5 0 0 0 .5-.5h-.5V5.5a.5.5 0 0 0 .5-.5V4z" />
                            </svg>
                            <span runat="server" id="scannerButtonText">Ler QR Code</span>
                        </button>

                        <div id="reader"></div>

                        <asp:TextBox ID="qrCodeInput" runat="server" CssClass="form-control" placeholder="QR Code de Usuário (escaneie)" ReadOnly="true"></asp:TextBox>
                    </asp:Panel>





                    <asp:Panel ID="pnlApontamentoFields" runat="server" Visible="true" Style="display: none;">
                        <hr style="margin: 30px 0; border: 0; border-top: 1px solid #eee;" />
                        <h2>Apontar Horas</h2>

                        <div class="input-group">
                            <label for="<%= txtHoras.ClientID %>">Horas (Ex: 2,5):</label>
                            <asp:TextBox ID="txtHoras" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                        </div>

                        <div class="input-group">
                            <label for="<%= txtObservacao.ClientID %>">Observação (Opcional):</label>
                            <asp:TextBox ID="txtObservacao" runat="server" CssClass="form-control" TextMode="MultiLine"></asp:TextBox>
                        </div>

                        <asp:Button ID="cmdApontarHora" runat="server" Text="Apontar Horas" OnClick="cmdApontarHora_Click" CssClass="button-submit" />
                    </asp:Panel>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="hdnQRCode" EventName="ValueChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </form>

    <script>

        function initializePageScripts() {
            const qrCodeInput = document.getElementById('<%= qrCodeInput.ClientID %>');
            const pnlLoadingMessageJs = document.getElementById('<%= pnlLoadingMessage.ClientID %>');
            const pnlErrorMessageJs = document.getElementById('<%= pnlErrorMessage.ClientID %>');
            const litErrorMessageJs = document.getElementById('<%= litErrorMessage.ClientID %>');
            const pnlApontamentoFieldsJs = document.getElementById('<%= pnlApontamentoFields.ClientID %>');
            const hdnQRCodeJs = document.getElementById('<%= hdnQRCode.ClientID %>');
            const scannerButtonText = document.getElementById('scannerButtonText');;

            const cmdApontarHoraBtn = document.getElementById('<%= cmdApontarHora.ClientID %>');

            let html5QrCode = null;

            window.toggleApontamentoFieldsClient = function (show) {
                if (pnlApontamentoFieldsJs) {
                    pnlApontamentoFieldsJs.style.display = show ? 'block' : 'none';
                }
            };

            window.updateScannerButtonText = function (text) {
                if (scannerButtonText) {
                    scannerButtonText.innerText = text;
                }
            };

            window.startScanner = async function () {
                if (pnlErrorMessageJs) pnlErrorMessageJs.style.display = 'none';
                if (pnlLoadingMessageJs) pnlLoadingMessageJs.style.display = 'none';

                if (html5QrCode && html5QrCode.isScanning) {
                    try {
                        await html5QrCode.stop();
                        console.log("Scanner existente parado.");
                    } catch (err) {
                        console.error("Erro ao parar scanner existente:", err);
                    }
                }

                const readerDiv = document.getElementById('reader');
                if (readerDiv) {
                    readerDiv.innerHTML = '';
                }

                html5QrCode = new Html5Qrcode("reader");

                const qrCodeSuccessCallback = (decodedText, decodedResult) => {
                    qrCodeInput.value = decodedText;
                    hdnQRCodeJs.value = decodedText;

                    if (pnlLoadingMessageJs) pnlLoadingMessageJs.style.display = 'block';

                    if (html5QrCode && html5QrCode.isScanning) {
                        html5QrCode.stop()
                            .then(() => {
                                console.log("Scanner parado após leitura do QR Code de usuário.");
                                if (cmdApontarHoraBtn) {
                                    cmdApontarHoraBtn.click();
                                } else {
                                    console.error("Botão 'Apontar Horas' não encontrado para simular clique.");
                                    if (pnlLoadingMessageJs) pnlLoadingMessageJs.style.display = 'none';
                                    window.showFrontendError("Erro interno: Botão de ação não encontrado.");
                                }
                            })
                            .catch(err => {
                                console.error("Erro ao parar scanner após leitura do usuário:", err);
                                if (pnlLoadingMessageJs) pnlLoadingMessageJs.style.display = 'none';
                                window.showFrontendError("Erro ao processar leitura do QR Code de usuário. Tente novamente.");
                                if (cmdApontarHoraBtn) {
                                    cmdApontarHoraBtn.click();
                                }
                            });
                    } else {
                        if (cmdApontarHoraBtn) {
                            cmdApontarHoraBtn.click();
                        } else {
                            console.error("Botão 'Apontar Horas' não encontrado para simular clique.");
                            if (pnlLoadingMessageJs) pnlLoadingMessageJs.style.display = 'none';
                            window.showFrontendError("Erro interno: Botão de ação não encontrado.");
                        }
                    }
                };

                const config = { fps: 10, qrbox: { width: 250, height: 250 } };

                try {
                    const devices = await Html5Qrcode.getCameras();
                    if (devices && devices.length) {
                        await html5QrCode.start(
                            { facingMode: "environment" },
                            config,
                            qrCodeSuccessCallback,
                            (error) => {
                            }
                        );
                        console.log("Scanner iniciado.");
                    } else {
                        window.showFrontendError("Nenhuma câmera encontrada. Certifique-se de que o dispositivo possui uma câmera e as permissões foram concedidas.");
                        console.error("Nenhuma câmera encontrada para Html5Qrcode.");
                    }
                } catch (err) {
                    console.error("Erro ao iniciar leitor ou acessar câmera:", err);
                    window.showFrontendError("Não foi possível iniciar a câmera para ler QR de usuário. Verifique as permissões do navegador e do sistema.");
                }
            };

            window.showLoadingMessageJs = function (message = "Carregando...") {
                if (pnlErrorMessageJs) pnlErrorMessageJs.style.display = 'none';
                if (pnlLoadingMessageJs) {
                    pnlLoadingMessageJs.innerText = message;
                    pnlLoadingMessageJs.style.display = 'block';
                }
            };

            window.hideLoadingMessageJs = function () {
                if (pnlLoadingMessageJs) pnlLoadingMessageJs.style.display = 'none';
            };

            window.showFrontendError = function (message) {
                if (pnlLoadingMessageJs) pnlLoadingMessageJs.style.display = 'none';
                if (pnlErrorMessageJs && litErrorMessageJs) {
                    litErrorMessageJs.innerText = message;
                    pnlErrorMessageJs.style.display = 'block';
                }
            };

            const urlParams = new URLSearchParams(window.location.search);
            const errorParam = urlParams.get('error');
            if (errorParam) {
                window.showFrontendError(decodeURIComponent(errorParam));
            }
        }

        window.onload = initializePageScripts;

        if (Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                initializePageScripts();
            });
        }
</script>
</body>
</html>
