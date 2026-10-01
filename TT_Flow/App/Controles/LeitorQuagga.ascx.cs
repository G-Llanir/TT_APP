using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TT_Flow.App.Controles
{
    public partial class LeitorQuagga : System.Web.UI.UserControl
    {
        public string txtClient { get; set; }
        public string click { get; set; }
        public string STemParametros { get; set; } = "N";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //if (txtClient != null && click != null)
                //{
                //    Session["CameraAberta"] = txtClient;
                //    Session["click"] = click;
                //}
                //if (Session["CameraAberta"] != null)
                //{
                //        RegistraQuaggaScript(Session["CameraAberta"].ToString(), Session["click"].ToString());
                //}
                //else

                if (STemParametros != "S")
                    RegistraQuaggaScript();
                else
                   RegistraFuncaoGenerica();

                RegisterQuaggaLibrary();
            }
            else
            {

            }
        }
        public void RegisterQuaggaLibrary()
        {
            string scriptQuagga = @"<script src='/app/js/quagga.min.js'></script>";
            Page.ClientScript.RegisterStartupScript(GetType(), "ValidarQuaggaScript", scriptQuagga, false);
        }
        public void RegistraQuaggaScript()
        {
            // Obter o ClientID do controle no code-behind
            string textBoxClientId = txtsCodigoControle.ClientID;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<script>");
            sb.AppendLine("function iniciarQuagga() {");
            sb.AppendLine("    var cameraElement = document.querySelector('#camera');");
            sb.AppendLine("    if (!cameraElement) {");
            sb.AppendLine("        console.error('Elemento #camera não encontrado.');");
            sb.AppendLine("        return;");
            sb.AppendLine("    }");
            sb.AppendLine("");
            sb.AppendLine("    Quagga.init({");
            sb.AppendLine("        inputStream: {");
            sb.AppendLine("            name: 'Live',");
            sb.AppendLine("            type: 'LiveStream',");
            sb.AppendLine("            target: cameraElement");  // Elemento onde o vídeo será exibido
            sb.AppendLine("        },");
            sb.AppendLine("        decoder: {");
            sb.AppendLine("            readers: ['code_128_reader', 'ean_reader']  // Tipos de código de barras que o Quagga deve reconhecer");
            sb.AppendLine("        }");
            sb.AppendLine("    }, function (err) {");
            sb.AppendLine("        if (err) {");
            sb.AppendLine("            console.error('Erro na inicialização do Quagga:', err);");
            sb.AppendLine("            return;");
            sb.AppendLine("        }");
            sb.AppendLine("        console.log('Inicialização concluída. Pronto para começar');");
            sb.AppendLine("        Quagga.start();");
            sb.AppendLine("");
            sb.AppendLine("        // Ajustar dimensões e estilos após a inicialização do Quagga");
            sb.AppendLine("        var videoElement = cameraElement.querySelector('video');");
            sb.AppendLine("        if (videoElement) {");
            sb.AppendLine("            videoElement.style.width = '98%';");
            sb.AppendLine("        }");
            sb.AppendLine("");
            sb.AppendLine("        var canvasElement = cameraElement.querySelector('canvas');");
            sb.AppendLine("        if (canvasElement) {");
            sb.AppendLine("            canvasElement.removeAttribute('width');");
            sb.AppendLine("            canvasElement.style.height = '1px';");
            sb.AppendLine("            // Outros estilos podem ser aplicados aqui");
            sb.AppendLine("        }");
            sb.AppendLine("    });");
            sb.AppendLine("");
            //sb.AppendLine("    Quagga.onDetected(function (data) {");
            //sb.AppendLine("        console.log('Código escaneado:', data.codeResult.code);");
            //sb.AppendLine("        document.querySelector('#resultado').innerText = data.codeResult.code;");
            //sb.AppendLine("");
            //sb.AppendLine("        // Enviar o valor para o TextBox");
            //sb.AppendLine("        var codigoBarras = data.codeResult.code;");
            //sb.AppendLine("        var textBox = document.getElementById('" + textBoxClientId + "');");
            //sb.AppendLine("        if (textBox) {");
            //sb.AppendLine("            textBox.value = codigoBarras;");
            //sb.AppendLine("        } else {");
            //sb.AppendLine("            console.error('TextBox não encontrado.');");
            //sb.AppendLine("        }");
            //sb.AppendLine("    });");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("// Adicionar a função ao evento EndRequest do PageRequestManager");
            sb.AppendLine("Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {");
            //sb.AppendLine("    iniciarQuagga();");
            sb.AppendLine("});");
            sb.AppendLine("</script>");

            // Registrar o script no cliente
            Page.ClientScript.RegisterStartupScript(GetType(), "IniciarQuaggaScript", sb.ToString(), false);
        }

        //Basta Passar os tipos de Decoder separando por vírgulas
        public void RegistraQuaggaScript(List<string> decoders)
        {
            // Obter o ClientID do controle no code-behind
            string textBoxClientId = txtsCodigoControle.ClientID;

            if (decoders == null || decoders.Count == 0)
            {
                decoders = new List<string> { "code_128_reader", "ean_reader" };
            }

            // Criar a string formatada para os decoders em formato JSON válido
            string decodersJson = string.Join(", ", decoders.Select(d => $"'{d}'"));

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<script>");
            sb.AppendLine("function iniciarQuagga() {");
            sb.AppendLine("    var cameraElement = document.querySelector('#camera');");
            sb.AppendLine("    if (!cameraElement) {");
            sb.AppendLine("        console.error('Elemento #camera não encontrado.');");
            sb.AppendLine("        return;");
            sb.AppendLine("    }");
            sb.AppendLine("");
            sb.AppendLine("    Quagga.init({");
            sb.AppendLine("        inputStream: {");
            sb.AppendLine("            name: 'Live',");
            sb.AppendLine("            type: 'LiveStream',");
            sb.AppendLine("            target: cameraElement");  // Elemento onde o vídeo será exibido
            sb.AppendLine("        },");
            sb.AppendLine("        decoder: {");
            sb.AppendLine($"            readers: [['code_128_reader', 'ean_reader', {decodersJson}]  // Tipos de código de barras que o Quagga deve reconhecer");
            sb.AppendLine("        }");
            sb.AppendLine("    }, function (err) {");
            sb.AppendLine("        if (err) {");
            sb.AppendLine("            console.error('Erro na inicialização do Quagga:', err);");
            sb.AppendLine("            return;");
            sb.AppendLine("        }");
            sb.AppendLine("        console.log('Inicialização concluída. Pronto para começar');");
            sb.AppendLine("        Quagga.start(); ");
            sb.AppendLine("        Quagga.initialized = true;                                  ");
            sb.AppendLine("");
            sb.AppendLine("        // Ajustar dimensões e estilos após a inicialização do Quagga");
            sb.AppendLine("        var videoElement = cameraElement.querySelector('video');");
            sb.AppendLine("        if (videoElement) {");
            sb.AppendLine("            videoElement.style.width = '98%';");
            sb.AppendLine("        }");
            sb.AppendLine("");
            sb.AppendLine("        var canvasElement = cameraElement.querySelector('canvas');");
            sb.AppendLine("        if (canvasElement) {");
            sb.AppendLine("            canvasElement.removeAttribute('width');");
            sb.AppendLine("            canvasElement.style.height = '1px';");
            sb.AppendLine("            // Outros estilos podem ser aplicados aqui");
            sb.AppendLine("        }");
            sb.AppendLine("    });");
            sb.AppendLine("");
            sb.AppendLine("    Quagga.onDetected(function (data) {");
            sb.AppendLine("        console.log('Código escaneado:', data.codeResult.code);");
            sb.AppendLine("        document.querySelector('#resultado').innerText = data.codeResult.code;");
            sb.AppendLine("");
            sb.AppendLine("        // Enviar o valor para o TextBox");
            sb.AppendLine("        var codigoBarras = data.codeResult.code;");
            sb.AppendLine($"        var textBox = document.getElementById('{textBoxClientId}');");
            sb.AppendLine("        if (textBox) {");
            sb.AppendLine("            textBox.value = codigoBarras;");
            sb.AppendLine("        } else {");
            sb.AppendLine("            console.error('TextBox não encontrado.');");
            sb.AppendLine("        }");
            sb.AppendLine("    });");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("// Adicionar a função ao evento EndRequest do PageRequestManager");
            sb.AppendLine("Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {");
            sb.AppendLine("    iniciarQuagga();");
            sb.AppendLine("});");
            sb.AppendLine("</script>");

            // Registrar o script no cliente
            Page.ClientScript.RegisterStartupScript(GetType(), "IniciarQuaggaScript", sb.ToString(), false);
        }
        public void RegistraQuaggaScript(string textBoxClientId)
        {
            //// Obter o ClientID do controle no code-behind
            //string textBoxClientId = txtsCodigoControle.ClientID;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<script>");
            sb.AppendLine("function iniciarQuagga() {");
            sb.AppendLine("    var cameraElement = document.querySelector('#camera');");
            sb.AppendLine("    if (!cameraElement) {");
            sb.AppendLine("        console.error('Elemento #camera não encontrado.');");
            sb.AppendLine("        return;");
            sb.AppendLine("    }");
            sb.AppendLine("");
            sb.AppendLine("    Quagga.init({");
            sb.AppendLine("        inputStream: {");
            sb.AppendLine("            name: 'Live',");
            sb.AppendLine("            type: 'LiveStream',");
            sb.AppendLine("            target: cameraElement");  // Elemento onde o vídeo será exibido
            sb.AppendLine("        },");
            sb.AppendLine("        decoder: {");
            sb.AppendLine("            readers: ['code_128_reader', 'ean_reader']  // Tipos de código de barras que o Quagga deve reconhecer");
            sb.AppendLine("        }");
            sb.AppendLine("    }, function (err) {");
            sb.AppendLine("        if (err) {");
            sb.AppendLine("            console.error('Erro na inicialização do Quagga:', err);");
            sb.AppendLine("            return;");
            sb.AppendLine("        }");
            sb.AppendLine("        console.log('Inicialização concluída. Pronto para começar');");
            sb.AppendLine("        Quagga.start();");
            sb.AppendLine("");
            sb.AppendLine("        // Ajustar dimensões e estilos após a inicialização do Quagga");
            sb.AppendLine("        var videoElement = cameraElement.querySelector('video');");
            sb.AppendLine("        if (videoElement) {");
            sb.AppendLine("            videoElement.style.width = '98%';");
            sb.AppendLine("        }");
            sb.AppendLine("");
            sb.AppendLine("        var canvasElement = cameraElement.querySelector('canvas');");
            sb.AppendLine("        if (canvasElement) {");
            sb.AppendLine("            canvasElement.removeAttribute('width');");
            sb.AppendLine("            canvasElement.style.height = '1px';");
            sb.AppendLine("            // Outros estilos podem ser aplicados aqui");
            sb.AppendLine("        }");
            sb.AppendLine("    });");
            sb.AppendLine("");
            sb.AppendLine("    Quagga.onDetected(function (data) {");
            sb.AppendLine("        console.log('Código escaneado:', data.codeResult.code);");
            sb.AppendLine("        document.querySelector('#resultado').innerText = data.codeResult.code;");
            sb.AppendLine("");
            sb.AppendLine("        // Enviar o valor para o TextBox");
            sb.AppendLine("        var codigoBarras = data.codeResult.code;");
            sb.AppendLine("        var textBox = document.getElementById('" + textBoxClientId + "');");
            sb.AppendLine("        if (textBox) {");
            sb.AppendLine("            textBox.value = codigoBarras;");
            sb.AppendLine("        } else {");
            sb.AppendLine("            console.error('TextBox não encontrado.');");
            sb.AppendLine("        }");
            sb.AppendLine("    });");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("// Adicionar a função ao evento EndRequest do PageRequestManager");
            sb.AppendLine("Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {");
            sb.AppendLine("    iniciarQuagga();");
            sb.AppendLine("});");
            sb.AppendLine("</script>");

            // Registrar o script no cliente
            Page.ClientScript.RegisterStartupScript(GetType(), "IniciarQuaggaScript", sb.ToString(), false);
        }

        public void RegistraQuaggaScript(string textBoxClientId, string click)
        {
            DesligarCam();
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<script>");
            sb.AppendLine("function iniciarQuagga() {");
            sb.AppendLine("    var cameraElement = document.querySelector('#camera');");
            sb.AppendLine("    if (!cameraElement) {");
            sb.AppendLine("        console.error('Elemento #camera não encontrado.');");
            sb.AppendLine("        return;");
            sb.AppendLine("    }");
            sb.AppendLine("");
            sb.AppendLine("    Quagga.init({");
            sb.AppendLine("        inputStream: {");
            sb.AppendLine("            name: 'Live',");
            sb.AppendLine("            type: 'LiveStream',");
            sb.AppendLine("            target: cameraElement");  // Elemento onde o vídeo será exibido
            sb.AppendLine("        },");
            sb.AppendLine("        decoder: {");
            sb.AppendLine("            readers: ['code_128_reader', 'ean_reader']  // Tipos de código de barras que o Quagga deve reconhecer");
            sb.AppendLine("        }");
            sb.AppendLine("    }, function (err) {");
            sb.AppendLine("        if (err) {");
            sb.AppendLine("            console.error('Erro na inicialização do Quagga:', err);");
            sb.AppendLine("            return;");
            sb.AppendLine("        }");
            sb.AppendLine("        console.log('Inicialização concluída. Pronto para começar');");
            sb.AppendLine("        Quagga.start();");
            sb.AppendLine("");
            sb.AppendLine("        // Ajustar dimensões e estilos após a inicialização do Quagga");
            sb.AppendLine("        var videoElement = cameraElement.querySelector('video');");
            sb.AppendLine("        if (videoElement) {");
            sb.AppendLine("            videoElement.style.width = '98%';");
            sb.AppendLine("        }");
            sb.AppendLine("");
            sb.AppendLine("        var canvasElement = cameraElement.querySelector('canvas');");
            sb.AppendLine("        if (canvasElement) {");
            sb.AppendLine("            canvasElement.removeAttribute('width');");
            sb.AppendLine("            canvasElement.style.height = '1px';");
            sb.AppendLine("            // Outros estilos podem ser aplicados aqui");
            sb.AppendLine("        }");
            sb.AppendLine("    });");
            sb.AppendLine("");
            sb.AppendLine("    Quagga.onDetected(function (data) {");
            sb.AppendLine("        console.log('Código escaneado:', data.codeResult.code);");
            sb.AppendLine("        document.querySelector('#resultado').innerText = data.codeResult.code;");
            sb.AppendLine("");
            sb.AppendLine("        // Enviar o valor para o TextBox");
            sb.AppendLine("        var codigoBarras = data.codeResult.code;");
            sb.AppendLine("        var textBox = document.getElementById('" + textBoxClientId + "');");
            sb.AppendLine("        if (textBox) {");
            sb.AppendLine("            textBox.value = codigoBarras;");
            sb.AppendLine("            document.querySelector('#" + click + "').click();");
            sb.AppendLine("        } else {");
            sb.AppendLine("            console.error('TextBox não encontrado.');");
            sb.AppendLine("        }");
            sb.AppendLine("    });");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("// Adicionar a função ao evento EndRequest do PageRequestManager");
            sb.AppendLine("Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {");
            sb.AppendLine("});");
            sb.AppendLine("</script>");

            // Registrar o script no cliente
            //string scriptKey = "IniciarQuaggaScript_" + Guid.NewGuid().ToString();
            Page.ClientScript.RegisterStartupScript(GetType(), "IniciarQuaggaScript", sb.ToString(), false);
        }

        private void RegistraFuncaoGenerica()
        {
            // Verificamos se o script já foi registrado para não duplicar na página
            if (Page.ClientScript.IsStartupScriptRegistered("FuncaoQuaggaGlobal")) return;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<script>");

            // A função agora aceita QUEM deve receber o valor (idInput) e QUEM deve ser clicado (idBotao)
            sb.AppendLine("function iniciarQuaggaDinamico(idInput, idBotao) {");

            sb.AppendLine("    var cameraElement = document.querySelector('#camera');");
            sb.AppendLine("    if (!cameraElement) { console.error('Div #camera não encontrada'); return; }");

            sb.AppendLine("    Quagga.init({");
            sb.AppendLine("        inputStream: { name: 'Live', type: 'LiveStream', target: cameraElement },");
            sb.AppendLine("        decoder: { readers: ['code_128_reader', 'ean_reader'] }");
            sb.AppendLine("    }, function (err) {");
            sb.AppendLine("        if (err) { console.log(err); return; }");
            sb.AppendLine("        Quagga.start();");
            sb.AppendLine("        Quagga.initialized = true;");

            // Ajustes visuais
            sb.AppendLine("        var video = cameraElement.querySelector('video');");
            sb.AppendLine("        if(video) video.style.width = '100%';");
            sb.AppendLine("        var canvas = cameraElement.querySelector('canvas');");
            sb.AppendLine("        if(canvas) canvas.style.display = 'none';");
            sb.AppendLine("    });");

            sb.AppendLine("    Quagga.onDetected(function (data) {");
            sb.AppendLine("        var code = data.codeResult.code;");

            // Lógica Dinâmica: Usa os IDs passados por parâmetro
            sb.AppendLine("        if(idInput) {");
            sb.AppendLine("            var txt = document.getElementById(idInput);");
            sb.AppendLine("            if(txt) txt.value = code;");
            sb.AppendLine("        }");

            sb.AppendLine("        if(idBotao) {");
            sb.AppendLine("            var btn = document.getElementById(idBotao);");
            sb.AppendLine("            if(btn) {");
            sb.AppendLine("                Quagga.stop();"); // Para antes de clicar
            sb.AppendLine("                btn.click();");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine("    });");
            sb.AppendLine("}"); // Fim da função iniciar

            // Função de Parar
            // Função de Parar CORRIGIDA
            sb.AppendLine("function pararQuagga() {");
            sb.AppendLine("    if (typeof Quagga !== 'undefined') {");
            sb.AppendLine("        Quagga.stop();");
            sb.AppendLine("    }");
            sb.AppendLine("    ");
            sb.AppendLine("    // Acessa o elemento de vídeo criado pelo Quagga");
            sb.AppendLine("    var video = document.querySelector('#camera video');");
            sb.AppendLine("    if (video && video.srcObject) {");
            sb.AppendLine("        var stream = video.srcObject;");
            sb.AppendLine("        var tracks = stream.getTracks();");
            sb.AppendLine("        ");
            sb.AppendLine("        // Varre todas as tracks (vídeo e lanterna) e as encerra forçadamente");
            sb.AppendLine("        tracks.forEach(function(track) {");
            sb.AppendLine("            track.stop(); ");
            sb.AppendLine("        });");
            sb.AppendLine("        video.srcObject = null;");
            sb.AppendLine("    }");
            sb.AppendLine("    ");
            sb.AppendLine("    var cam = document.querySelector('#camera');");
            sb.AppendLine("    if(cam) cam.innerHTML = '';");
            sb.AppendLine("}");


            sb.AppendLine("</script>");

            // Registra como ClientScriptBlock para ficar no topo ou body, acessível globalmente
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "FuncaoQuaggaGlobal", sb.ToString(), false);
        }
        public string GetCodigoBarras(TextBox txtCodigo)
        {
            if (!string.IsNullOrEmpty(txtsCodigoControle.Text))
                txtCodigo.Text = txtsCodigoControle.Text;

            return txtCodigo.Text;
        }
        public void AbrirCamera()
        {
            if (!string.IsNullOrEmpty(txtClient) && !string.IsNullOrEmpty(click))
            {
                // AQUI é o lugar certo. Chamamos o método que cria o Javascript 
                // usando as variáveis públicas que a página OPI preencheu.
                RegistraQuaggaScript(this.txtClient, this.click);
            }

            string script = "iniciarQuagga();";
            ScriptManager.RegisterStartupScript(this, GetType(), "AbrirCameraScript", script, true);
        }
        public void FecharCamera()
        {
            string script = "desiniciarQuagga();";
            ScriptManager.RegisterStartupScript(this, GetType(), "FecharCameraScript", script, true);
        }

        public void DesligarCam()
        {
            string script = @"
                function desiniciarQuagga() {
                    if (typeof Quagga !== 'undefined' && Quagga.initialized) {
                        Quagga.stop(); // Para o scanner e a câmera
                        console.log('Entrei no método desiniciarQuagga');

                        // Remover o vídeo e o canvas
                        var cameraDiv = document.querySelector('#camera');
                        if (cameraDiv) {
                            var videoElement = cameraDiv.querySelector('video');
                            if (videoElement) {
                                var stream = videoElement.srcObject;
                                if (stream) {
                                    var tracks = stream.getTracks();
                                    tracks.forEach(function(track) {
                                        track.stop(); // Para todas as tracks de mídia
                                    });
                                }
                                videoElement.srcObject = null; // Desvincula o stream do vídeo
                            }
                            cameraDiv.innerHTML = ''; // Limpa o conteúdo do div da câmera
                        }

                        // Remover todos os callbacks do Quagga
                        Quagga.offDetected();
                        Quagga.offProcessed();
                        Quagga.offDecoded();
                        Quagga.offFailed();

                        console.log('Quagga desinicializado e câmera desligada.');
                    } else {
                        console.log('Quagga não está inicializado ou já foi parado.');
                    }
                    Quagga.initialized = false;
                    removeScript('DesiniciarQuaggaScript');
                }

                function removeScript(scriptId) {
                    var scriptElement = document.getElementById(scriptId);
                    if (scriptElement) {
                        scriptElement.parentNode.removeChild(scriptElement);
                    }
                }
            ";

            // Registrar o script corretamente
            ScriptManager.RegisterStartupScript(this, GetType(), "DesiniciarQuaggaScript", script, true);
        }

        public void AbrirCameraVariante()
        {
            if (string.IsNullOrEmpty(txtClient) || string.IsNullOrEmpty(click)) return;

            string script = $"iniciarQuaggaDinamico('{this.txtClient}', '{this.click}');";

            ScriptManager.RegisterStartupScript(this, GetType(), "StartQuagga_" + this.ClientID, script, true);
        }

        public void DesligarCamVariante()
        {
            string script = "pararQuagga();";
            ScriptManager.RegisterStartupScript(this, GetType(), "StopQuagga", script, true);
        }
    }
}