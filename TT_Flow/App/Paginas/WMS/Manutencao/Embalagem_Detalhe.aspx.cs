using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.FrameWork;
using System.Linq;

namespace TT_Flow.App.Paginas.WMS
{
    public partial class Embalagem_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Embalagem";
        string sProcedure = "sp_Manipula_tbl_Flow_WMS_OPI_Embalagem";
        public List<FrameWork.cls_WMS_Produtos> ls_unitizadosItens
        {
            get
            {
                if (ViewState["ls_unitizadosItens"] == null)
                {
                    ViewState["ls_unitizadosItens"] = new List<FrameWork.cls_WMS_Produtos>();
                }
                return (List<FrameWork.cls_WMS_Produtos>)ViewState["ls_unitizadosItens"];
            }
            set
            {
                ViewState["ls_unitizadosItens"] = value;
            }
        }
        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();

            if (!IsPostBack)
            {
                RegistraQuaggaScript();
            }
            else
            {
                RegistraQuaggaScript();
            }

            cmdSalvar.Text = "BBBBBBBBaBBaaaazzz";
            //cmdSalvar.Enabled = false;
            MensagemPagina.MostraMensagem_Sucesso("TESTE");
        }
        #endregion

        #region |Metodos Banco de Dados
        protected void IncluirUnitizadosItens_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(txtsCodigoBarras.Text))
            {
                string codigoBarras = txtsCodigoBarras.Text; 

              
                cls_WMS_Produtos novoItem = new cls_WMS_Produtos();

                
                novoItem.SCodigo = codigoBarras; 

                ls_unitizadosItens.Add(novoItem);

                RegistraQuaggaScript();
            }
            dtgUNItensDataBind();     
        }

        void dtgUNItensDataBind()
        {
            dtgUNItens.DataSource = ls_unitizadosItens;
            dtgUNItens.DataBind();

            RegistraQuaggaScript();
        }
        #endregion

        #region | Script 
   
        void RegistraQuaggaScript()
        {
            string scriptQuagga = @"
    <script src='/app/js/quagga.min.js'></script>";

            Page.ClientScript.RegisterStartupScript(GetType(), "QuaggaScript", scriptQuagga, false);
            string script = @"
    <script>
        function iniciarQuagga() {
            Quagga.init({
                inputStream: {
                    name: 'Live',
                    type: 'LiveStream',
                    target: document.querySelector('#camera')    // Ou '#seuElemento' (opcional)
                },
                decoder: {
                    readers: ['code_128_reader','ean_reader']
                }
            }, function (err) {
                if (err) {
                    console.log(err);
                    return;
                }
                console.log('Inicialização concluída. Pronto para começar');
                Quagga.start();

            // Definir as dimensões do vídeo após a inicialização do Quagga
            var videoElement = document.querySelector('#camera video');
            if (videoElement) {
                videoElement.style.width = '98%';
            }
                // Aplicar estilos ao elemento <canvas> após a inicialização do Quagga
                var canvasElement = document.querySelector('#camera canvas');
                if (canvasElement) {
                canvasElement.removeAttribute('width');
                canvasElement.style.height = '1px'
                    // Outros estilos que você deseja aplicar ;
                }
            });

            Quagga.onDetected(function (data) {
                console.log('escaneado!');
                console.log(data.codeResult.code);
                document.querySelector('#resultado').innerText = data.codeResult.code;

              // Enviar o valor para o TextBox
            var codigoBarras = data.codeResult.code;
            var textBox = document.getElementById('cphCorpo_txtsCodigoBarras');
            if (textBox) {
                textBox.value = codigoBarras;
            } else {
                console.log('TextBox não encontrado.');
            }
            });
        }

        // Chamar a função de inicialização no carregamento da página
        window.onload = function () {
            iniciarQuagga();
        };
    </script>";

            // Registrar o script no cliente
            Page.ClientScript.RegisterStartupScript(GetType(), "IniciarQuaggaScript", script, false);
        }

        #endregion

    }
}