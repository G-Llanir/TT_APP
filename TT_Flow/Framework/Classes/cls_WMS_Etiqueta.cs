using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TT.FrameWork;
using IDENTITY = TT.FrameWork.Identity;
using System.Globalization;
using RETORNO = TT.FrameWork.BD.Retorno;
using System.Web.UI;
using iTextSharp.tool.xml.html;
using NPOI.SS.Formula.Functions;

namespace TT_Flow.FrameWork
{
    [Serializable]
    public class cls_WMS_Etiquetas
    {
        #region | Construtor
        public cls_WMS_Etiquetas() { }


        #endregion

        #region | Membros Privados
        private int _idEtiqueta;
        private int _idProduto;
        private string _sDscProduto;
        private decimal _nQuantidade;
        private string _sDscEtiqueta;
        private string _sObservacao;
        private int _idOPI;
        private int _idUsuarioAtualizacao;
        private string _dtImpressao;
        private string _sTipoEtiqueta;
        private string _sCodigoBarras;
        private string _sExclusao;
        private string _sLote;
        private int _IdImpressora;
        #endregion

        #region | Propriedades
        public int IdEtiqueta { get => _idEtiqueta; set => _idEtiqueta = value; }
        public int IdProduto { get => _idProduto; set => _idProduto = value; }
        public decimal NQuantidade { get => _nQuantidade; set => _nQuantidade = value; }
        public string SdscEtiqueta { get => _sDscEtiqueta; set => _sDscEtiqueta = value; }
        public string Sobservacao { get => _sObservacao; set => _sObservacao = value; }
        public int IdOPI { get => _idOPI; set => _idOPI = value; }
        public int IdUsuarioAtualizacao { get => _idUsuarioAtualizacao; set => _idUsuarioAtualizacao = value; }
        public string SdscProduto { get => _sDscProduto; set => _sDscProduto = value; }
        public string DtImpressao { get => _dtImpressao; set => _dtImpressao = value; }
        public string StipoEtiqueta { get => _sTipoEtiqueta; set => _sTipoEtiqueta = value; }
        public string SCodigoBarras { get => _sCodigoBarras; set => _sCodigoBarras = value; }
        public string SExclusao { get => _sExclusao; set => _sExclusao = value; }
        public string SLote { get => _sLote; set => _sLote = value; }
        public string SGarantia { get; set; }
        public string NSerie { get; set; }
        public int IdEtiquetaSerie { get; set; }
        public int IdLocal { get; set; }
        public int IdPosicao { get; set; }
        public string sCodigo { get; set; }
        public int IdImpressora { get; set; }
        public string sTipoEtiqueta { get; set; }
        public int idLocalOPI { get; set; }

        #endregion

        //Implementação do Quagga Genérico
        void RegistraQuaggaScript(Page page)
        {
            string scriptQuagga = @"
    <script src='/app/js/quagga.min.js'></script>";

            page.ClientScript.RegisterStartupScript(GetType(), "QuaggaScript", scriptQuagga, false);
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
                            readers: ['code_128_reader']
                        }
                    }, function (err) {
                        if (err) {
                            console.log(err);
                            return;
                        }
                        console.log('Inicialização concluída. Pronto para começar');
                        Quagga.start();
                    });

                    Quagga.onDetected(function (data) {
                        console.log(data.codeResult.code);
                        document.querySelector('#resultado').innerText = data.codeResult.code;
                    });
                }

                // Chamar a função de inicialização no carregamento da página
                window.onload = function () {
                    iniciarQuagga();
                };
            </script>";

            // Registrar o script no cliente
            page.ClientScript.RegisterStartupScript(GetType(), "IniciarQuaggaScript", script, false);
        }
    }
}