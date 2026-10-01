<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="cftv.aspx.cs" Inherits="TT_Flow.App.Paginas.cftv" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>


    <style>

      .main {

          margin-top: 50px;

       }

 

      .mg {

          margin-top: 20px;

       }

  </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>

            <div class="col">

                  <asp:FileUpload ID="FileUpload01" ClientIDMode="Static" onchange="this.form.submit()" accept="image/*;capture=camera" runat="server" />

              </div>

            <asp:Button ID="cmdCapturar" runat="server" Text="Capturar Imagem" OnClick="cmdCapturar_Click" />
            <asp:Image ID="Image1" runat="server" />
        </div>


        <br />
        <br />
<br />
<br />
<br />
<br />
<br />


    <div id="resultado"></div>
    <div id="camera"></div>

    <script src="quagga.min.js"></script>

    <script>

        Quagga.init({
            inputStream: {
                name: "Live",
                type: "LiveStream",
                target: document.querySelector('#camera')    // Or '#yourElement' (optional)
            },
            decoder: {
                readers: ["code_128_reader"]
            }
        }, function (err) {
            if (err) {
                console.log(err);
                return
            }
            console.log("Initialization finished. Ready to start");
            Quagga.start();
        });

        Quagga.onDetected(function (data) {
            console.log(data.codeResult.code);
            document.querySelector('#resultado').innerText = data.codeResult.code;
        });

    </script>

    </form>
</body>
</html>
