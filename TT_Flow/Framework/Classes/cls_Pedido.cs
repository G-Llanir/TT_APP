using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using TT.FrameWork;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Flow.FrameWork
{
    [Serializable]
    public class cls_Pedidos_Itens
    {
        #region | Construtor 

        public cls_Pedidos_Itens()
        {
            //
            // TODO: Add constructor logic here
            //


        }

        public cls_Pedidos_Itens
        (
            int idContador
            , int idItem
            , int nOrdem
            , int idProduto
            , int idPedido
            , string sDscCategoriaVendas
            , string sCodigoProduto
            , string sDscProduto
            , string sUnidade
            , double nQuantidade
            , double nValorUnitario
            , double nValorTotal
            , string sObservacao
            , string dtPrevisaoEntrega
            , string sFuncao
            , string sEscopo
            , double nValorReal
            , decimal nValorRealTotal
            , decimal nResultado
            , double nQuantidadeE
            , double nQuantidadeO
            , double nQuantidadeRO
            , double nQuantidadeCO
            , double nTotal
            , bool EnvioResultado
            , string sTipoProduto_Servico
            , string sDscProdutoIdioma
            , string sTipo
            , decimal nICMS
            , decimal nIPI
            , decimal nValorTblPreco
            , string sUnidadeEntrega
            , string sMoedaVenda
            , int idTipo
            , int idDepartamento
            , string sTipoFaturamento
            , string sDscDepartamento
            , string sEmitirNFe
        )
        {
            _idContador = idContador;
            _idItem = idItem;
            _nOrdem = nOrdem;
            _idProduto = idProduto;
            _idPedido = idPedido;
            _sDscCategoriaVendas = sDscCategoriaVendas;
            _sCodigoProduto = sCodigoProduto;
            _sDscProduto = sDscProduto;
            _sUnidade = sUnidade;
            _nQuantidade = nQuantidade;
            _nValorUnitario = nValorUnitario;
            _nValorTotal = nValorTotal;
            _sObservacao = sObservacao;
            _dtPrevisaoEntrega = dtPrevisaoEntrega;
            _sFuncao = sFuncao;
            _sEscopo = sEscopo;
            _nValorReal = nValorReal;
            _nValorRealTotal = nValorRealTotal;
            _nResultado = nResultado;
            _nQuantidadeE = nQuantidadeE;
            _nQuantidadeO = nQuantidadeO;
            _nQuantidadeRO = nQuantidadeRO;
            _nQuantidadeCO = nQuantidadeCO;
            _nTotal = nTotal;
            _EnvioResultado = EnvioResultado;
            _sTipoProduto_Servico = sTipoProduto_Servico;
            _sDscProdutoIdioma = sDscProdutoIdioma;
            _sTipo = sTipo;
            _nICMS = nICMS;
            _nIPI = nIPI;
            _nValorTblPreco = nValorTblPreco;
            _sUnidadeEntrega = sUnidadeEntrega;
            _sMoedaVenda = sMoedaVenda;
            _idTipo = idTipo;
            _idDepartamento = idDepartamento;
            _sTipoFaturamento = sTipoFaturamento;
            _sDscDepartamento = sDscDepartamento;
            _sEmitirNFe = sEmitirNFe;
        }

        #endregion

        #region | Membros Privados

        private int _idContador;
        private int _idItem;
        private int _nOrdem;
        private int _idProduto;
        private int _idPedido;
        private string _sDscCategoriaVendas;
        private string _sCodigoProduto;
        private string _sDscProduto;
        private string _sUnidade;
        private double _nQuantidade;
        private double _nValorUnitario;
        private double _nValorTotal;
        private string _sObservacao;
        private string _dtPrevisaoEntrega;
        private string _sFuncao;
        private string _sEscopo;
        private double _nValorReal;
        private decimal _nValorRealTotal;
        private decimal _nResultado;
        private int _idNCM;
        private string _sCodigoNCM;
        private string _sCodigoCEST;
        private int _Importacao_idDestino;
        private string _Importacao_sDscDestino;
        private int _Importacao_idAtoConcessorio;
        private string _Importacao_sCodigoAtoConcessorio;
        private double _Importacao_nValorUnitario_Origem;
        private double _Importacao_nValorTotal_Origem;
        private string _Importacao_dtPO;
        private string _Importacao_dtETD;
        private string _Importacao_dtETA;
        private double _nQuantidadeE;
        private double _nQuantidadeO;
        private double _nQuantidadeRO;
        private double _nQuantidadeCO;
        private double _nTotal;
        private bool _EnvioResultado;
        private string _sTipoProduto_Servico;
        private string _sDscProdutoIdioma;
        private string _sTipo;
        private decimal _nICMS;
        private decimal _nIPI;
        private decimal _nValorTblPreco;
        private string _sUnidadeEntrega;
        private string _sMoedaVenda;
        private int _idTipo;
        private int _idDepartamento;
        private string _sTipoFaturamento;
        private string _sDscDepartamento;
        private string _sEmitirNFe;
        private decimal _nMVA;
        private decimal _nICMS_Interno_Destino;
        private decimal _nVlrIPI;
        private decimal _nVlrICMS;
        private decimal _nVlrST;
        private decimal _nVlrDIFAL;
        private decimal _nDesconto;
        private bool _bProdutoPai;

        private int _idCST_ICMS;
        private int _idCST_cBenef;
        private decimal _nPercReducaoBC;
        private string _sPedidoCliente;

        #endregion

        #region | Propriedades

        //resultado
        public double nValorReal
        {
            get { return _nValorReal; }
            set { _nValorReal = value; }
        }

        public decimal nValorRealTotal
        {
            get { return _nValorRealTotal; }
            set { _nValorRealTotal = value; }
        }

        public decimal nResultado
        {
            get { return _nResultado; }
            set { _nResultado = value; }
        }

        public int idContador
        {
            get { return _idContador; }
            set { _idContador = value; }
        }

        public int idItem
        {
            get { return _idItem; }
            set { _idItem = value; }
        }

        public int nOrdem
        {
            get { return _nOrdem; }
            set { _nOrdem = value; }
        }

        public int idProduto
        {
            get { return _idProduto; }
            set { _idProduto = value; }
        }

        public int idPedido
        {
            get { return _idPedido; }
            set { _idPedido = value; }
        }

        public string sDscCategoriaVendas
        {
            get { return _sDscCategoriaVendas; }
            set { _sDscCategoriaVendas = value; }
        }

        public string sCodigoProduto
        {
            get { return _sCodigoProduto; }
            set { _sCodigoProduto = value; }
        }

        public string sDscProduto
        {
            get { return _sDscProduto; }
            set { _sDscProduto = value; }
        }

        public string sUnidade
        {
            get { return _sUnidade; }
            set { _sUnidade = value; }
        }

        public double nQuantidade
        {
            get { return _nQuantidade; }
            set { _nQuantidade = value; }
        }

        public double nValorUnitario
        {
            get { return _nValorUnitario; }
            set { _nValorUnitario = value; }
        }

        public double nValorTotal
        {
            get { return _nValorTotal; }
            set { _nValorTotal = value; }
        }

        public string sObservacao
        {
            get { return _sObservacao; }
            set { _sObservacao = value; }
        }

        public string dtPrevisaoEntrega
        {
            get { return _dtPrevisaoEntrega; }
            set { _dtPrevisaoEntrega = value; }
        }

        public string sFuncao
        {
            get { return _sFuncao; }
            set { _sFuncao = value; }

        }

        public string sEscopo
        {
            get { return _sEscopo; }
            set { _sEscopo = value; }

        }

        public int idCNM
        {
            get { return _idNCM; }
            set { _idNCM = value; }
        }

        public string sCodigoNCM
        {
            get { return _sCodigoNCM; }
            set { _sCodigoNCM = value; }

        }

        public string sCodigoCEST
        {
            get { return _sCodigoCEST; }
            set { _sCodigoCEST = value; }

        }

        public int Importacao_idDestino
        {
            get { return _Importacao_idDestino; }
            set { _Importacao_idDestino = value; }
        }

        public string Importacao_sDscDestino
        {
            get { return _Importacao_sDscDestino; }
            set { _Importacao_sDscDestino = value; }

        }

        public int Importacao_idAtoConcessorio
        {
            get { return _Importacao_idAtoConcessorio; }
            set { _Importacao_idAtoConcessorio = value; }
        }

        public string Importacao_sCodigoAtoConcessorio
        {
            get { return _Importacao_sCodigoAtoConcessorio; }
            set { _Importacao_sCodigoAtoConcessorio = value; }

        }

        public double Importacao_nValorUnitario_Origem
        {
            get { return _Importacao_nValorUnitario_Origem; }
            set { _Importacao_nValorUnitario_Origem = value; }
        }

        public double Importacao_nValorTotal_Origem
        {
            get { return _Importacao_nValorTotal_Origem; }
            set { _Importacao_nValorTotal_Origem = value; }
        }

        public string Importacao_dtPO
        {
            get { return _Importacao_dtPO; }
            set { _Importacao_dtPO = value; }

        }

        public string Importacao_dtETD
        {
            get { return _Importacao_dtETD; }
            set { _Importacao_dtETD = value; }

        }

        public string Importacao_dtETA
        {
            get { return _Importacao_dtETA; }
            set { _Importacao_dtETA = value; }

        }//adicionado por welligton 20/12/2022

        public double nQuantidadeE
        {
            get { return _nQuantidadeE; }
            set { _nQuantidadeE = value; }
        }

        public double nQuantidadeO
        {
            get { return _nQuantidadeO; }
            set { _nQuantidadeO = value; }
        }

        public double nQuantidadeRO
        {
            get { return _nQuantidadeRO; }
            set { _nQuantidadeRO = value; }
        }

        public double nQuantidadeCO
        {
            get { return _nQuantidadeCO; }
            set { _nQuantidadeCO = value; }
        }

        public double nTotal
        {
            get { return _nTotal; }
            set { _nTotal = value; }
        }

        public bool EnvioResultado
        {
            get { return _EnvioResultado; }
            set { _EnvioResultado = value; }
        }

        public int IdCondicaoPagamento { get; set; }

        public int IdStatus { get; set; }

        public int IdEndereco { get; set; }

        public int IdFormaPagamento { get; set; }

        public int IdCliente { get; set; }

        public DateTime DtInclusao { get; set; }

        public int IdUsuario { get; set; }

        public string sDscUsuario { get; set; }

        public int idCotacao { get; set; }

        public string sDscParceiro { get; set; }

        public string sTipoProduto_Servico { get; set; }

        public string sDscProdutoIdioma
        {
            get { return _sDscProdutoIdioma; }
            set { _sDscProdutoIdioma = value; }
        }

        public string sTipo
        {
            get { return _sTipo; }
            set { _sTipo = value; }
        }

        public decimal nICMS
        {
            get { return _nICMS; }
            set { _nICMS = value; }
        }

        public decimal nIPI
        {
            get { return _nIPI; }
            set { _nIPI = value; }
        }

        public decimal nValorTblPreco
        {
            get { return _nValorTblPreco; }
            set { _nValorTblPreco = value; }
        }

        public string sUnidadeEntrega
        {
            get { return _sUnidadeEntrega; }
            set { _sUnidadeEntrega = value; }
        }

        public string sMoedaVenda
        {
            get { return _sMoedaVenda; }
            set { _sMoedaVenda = value; }
        }

        public int idTipo
        {
            get { return _idTipo; }
            set { _idTipo = value; }
        }

        public int idDepartamento
        {
            get { return _idDepartamento; }
            set { _idDepartamento = value; }
        }

        public string sTipoFaturamento
        {
            get { return _sTipoFaturamento; }
            set { _sTipoFaturamento = value; }
        }

        public string sDscDepartamento
        {
            get { return _sDscDepartamento; }
            set { _sDscDepartamento = value; }
        }

        public string sEmitirNFe
        {
            get { return _sEmitirNFe; }
            set { _sEmitirNFe = value; }
        }

        public decimal nMVA
        {
            get { return _nMVA; }
            set { _nMVA = value; }
        }

        public decimal nICMS_Interno_Destino
        {
            get { return _nICMS_Interno_Destino; }
            set { _nICMS_Interno_Destino = value; }
        }

        public decimal nVlrIPI
        {
            get { return _nVlrIPI; }
            set { _nVlrIPI = value; }
        }

        public decimal nVlrICMS
        {
            get { return _nVlrICMS; }
            set { _nVlrICMS = value; }
        }

        public decimal nVlrST
        {
            get { return _nVlrST; }
            set { _nVlrST = value; }
        }

        public decimal nVlrDIFAL
        {
            get { return _nVlrDIFAL; }
            set { _nVlrDIFAL = value; }
        }

        public decimal nDesconto
        {
            get { return _nDesconto; }
            set { _nDesconto = value; }
        }

        public bool bProdutoPai
        {
            get { return _bProdutoPai; }
            set { _bProdutoPai = value; }
        }
        public string dtChegada { get; set; }

        public int idCST_ICMS
        {
            get { return _idCST_ICMS; }
            set { _idCST_ICMS = value; }
        }
        public int idCST_cBenef
        {
            get { return _idCST_cBenef; }
            set { _idCST_cBenef = value; }
        }
        public decimal nPercReducaoBC
        {
            get { return _nPercReducaoBC; }
            set { _nPercReducaoBC = value; }
        }
        public string sPedidoCliente
        {
            get { return _sPedidoCliente; }
            set { _sPedidoCliente = value; }
        }


        #endregion
    }

    [Serializable]
    public class cls_Pedidos_Faturamento
    {
        #region | Construtor 

        public cls_Pedidos_Faturamento() { }

        #endregion


        #region | Propriedades

        public int idContador { get; set ; }
        public int idFaturamento { get; set; }
        public int idServico { get; set; }
        public int idTipo { get; set; }
        public int idDepartamento { get; set; }
        public int idEmpresa { get; set; }

        public string sFuncao { get; set ; }
        public string sDscFaturamento { get; set ; }
        public string sDscServico { get; set ; }
        public string sFaturado { get; set ; }
        public string sCor { get; set ; }
        public string sDscTipo { get; set ; }
        public string sDscDepartamento { get; set ; }
        public string sDscEmpresa { get; set ; }
        public string dtFaturamento { get; set; }
        public string sBase_Porcentagem { get; set; }

        public decimal nPorcentagem { get; set; }
        public decimal nValor { get; set; }

        #endregion
    }

    [Serializable]
    public class cls_Fluxo
    {

        #region | Construtor 
        public cls_Fluxo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public cls_Fluxo
        (
              int idFluxo
            , string sDscFluxo
            , int nTempoTotalHoras
            , int nTempoDias
        )
        {
            _idFluxo = idFluxo;
            _sDscFluxo = sDscFluxo;
            _nTempoTotalHoras = nTempoTotalHoras;
            _nTempoDias = nTempoDias;

        }

        #endregion

        #region | Membros Privados 

        private int _idFluxo;
        private string _sDscFluxo;
        private int _nTempoTotalHoras;
        private int _nTempoDias;
        private string _sFuncao;

        #endregion

        #region | Propriedades

        public int idFluxo
        {
            get { return _idFluxo; }
            set { _idFluxo = value; }
        }

        public string sDscFluxo
        {
            get { return _sDscFluxo; }
            set { _sDscFluxo = value; }
        }

        public int nTempoTotalHoras
        {
            get { return _nTempoTotalHoras; }
            set { _nTempoTotalHoras = value; }
        }

        public int nTempoDias
        {
            get { return _nTempoDias; }
            set { _nTempoDias = value; }
        }

        public string sFuncao
        {
            get { return _sFuncao; }
            set { _sFuncao = value; }
        }

        #endregion

    }

    [Serializable]
    public class cls_Pedidos_Envios : cls_Ocorrencias
    {

        #region | Construtor 
        public cls_Pedidos_Envios()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public cls_Pedidos_Envios
        (
              int idRegistro
            , int idPedido
            , int idTipoEnvio
            , int idParceiro
            , string sCodigo
            , int idMetodoEnvio
            , int idMeioEnvio
            , decimal nPesoEnvio
            , decimal nLarguraEnvio
            , decimal nComprimentoEnvio
            , decimal nAlturaEnvio
            , decimal nVolume
        //, int idArquivo
        )
        {
            _idRegistro = idRegistro;
            _idPedido = idPedido;
            _idPedido = idPedido;
            _idTipoEnvio = idTipoEnvio;
            _idParceiro = idParceiro;
            _sCodigo = sCodigo;
            _idMetodoEnvio = idMetodoEnvio;
            _idMeioEnvio = idMeioEnvio;
            _nPesoEnvio = nPesoEnvio;
            _nLarguraEnvio = nLarguraEnvio;
            _nComprimentoEnvio = nComprimentoEnvio;
            _nAlturaEnvio = nAlturaEnvio;
            _nVolume = nVolume;
            //_idArquivo = idArquivo;
        }


        #endregion

        #region | Membros Privados 

        private int _idRegistro;
        private int _idContador;
        private int _idPedido;
        private int _idTipoEnvio;
        private string _sDscTipoEnvio;
        private int _idParceiro;
        private string _sDscParceiro;
        private string _sCodigo;
        private int _idMetodoEnvio;
        private string _sDscMetodoEnvio;
        private int _idMeioEnvio;
        private string _sDscMeioEnvio;
        private string _sFuncao;
        private decimal _nPesoEnvio;
        private decimal _nLarguraEnvio;
        private decimal _nComprimentoEnvio;
        private decimal _nAlturaEnvio;
        private decimal _nVolume;
        //private int _idArquivo;

        #endregion

        #region | Propriedades

        public int idRegistro
        {
            get { return _idRegistro; }
            set { _idRegistro = value; }
        }
        public int idContador
        {
            get { return _idContador; }
            set { _idContador = value; }
        }

        public int idPedido
        {
            get { return _idPedido; }
            set { _idPedido = value; }
        }

        public int idTipoEnvio
        {
            get { return _idTipoEnvio; }
            set { _idTipoEnvio = value; }
        }

        public string sDscTipoEnvio
        {
            get { return _sDscTipoEnvio; }
            set { _sDscTipoEnvio = value; }
        }

        public int idParceiro
        {
            get { return _idParceiro; }
            set { _idParceiro = value; }
        }
        public string sDscParceiro
        {
            get { return _sDscParceiro; }
            set { _sDscParceiro = value; }
        }

        public string sCodigo
        {
            get { return _sCodigo; }
            set { _sCodigo = value; }
        }

        public int idMetodoEnvio
        {
            get { return _idMetodoEnvio; }
            set { _idMetodoEnvio = value; }
        }

        public string sDscMetodoEnvio
        {
            get { return _sDscMetodoEnvio; }
            set { _sDscMetodoEnvio = value; }
        }

        public int idMeioEnvio
        {
            get { return _idMeioEnvio; }
            set { _idMeioEnvio = value; }
        }
        public string sDscMeioEnvio
        {
            get { return _sDscMeioEnvio; }
            set { _sDscMeioEnvio = value; }
        }
        public string sFuncao
        {
            get { return _sFuncao; }
            set { _sFuncao = value; }
        }

        public decimal nPesoEnvio
        {
            get { return _nPesoEnvio; }
            set { _nPesoEnvio = value; }
        }

        public decimal nLarguraEnvio
        {
            get { return _nLarguraEnvio; }
            set { _nLarguraEnvio = value; }
        }

        public decimal nComprimentoEnvio
        {
            get { return _nComprimentoEnvio; }
            set { _nComprimentoEnvio = value; }
        }

        public decimal nAlturaEnvio
        {
            get { return _nAlturaEnvio; }
            set { _nAlturaEnvio = value; }
        }

        public decimal nVolume
        {
            get { return _nVolume; }
            set { _nVolume = value; }
        }

        //public int idArquivo
        //{
        //    get { return _idArquivo;}
        //    set { _idArquivo = value;}
        //}
        #endregion

    }

    [Serializable]
    public class cls_Pedidos_Garantia
    {
        #region | Construtor
        public cls_Pedidos_Garantia()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public cls_Pedidos_Garantia
        (
              int idLinha
            , string sDscGarantia
            , string dtGarantia
            , string sGarantia
            , decimal nValorGarantia
            , string sFuncao
            , int idRegistro
            , string sEnvioRemesa
        //, int idArquivo
        )
        {
            _idLinha = idLinha;
            _sDscGarantia = sDscGarantia;
            _dtGarantia = dtGarantia;
            _sGarantia = sGarantia;
            _nValorGarantia = nValorGarantia;
            _sFuncao = sFuncao;
            _idRegistro = idRegistro;
            _sEnvioRemesa = sEnvioRemesa;
            //_idArquivo = idArquivo;
        }
        #endregion

        #region | Membros Privados
        private int _idLinha;
        private string _sDscGarantia;
        private string _dtGarantia;
        private string _sGarantia;
        private decimal _nValorGarantia;
        private string _sFuncao;
        private int _idRegistro;
        private string _sEnvioRemesa;
        private int _idArquivo;
        private string _sNomeArquivo;
        private string _sObservacaoArquivo;
        private Byte[] _objArquivo;
        #endregion

        #region | Propriedades
        public int idLinha
        {
            get { return _idLinha; }
            set { _idLinha = value; }
        }

        public string sDscGarantia
        {
            get { return _sDscGarantia; }
            set { _sDscGarantia = value; }
        }

        public string dtGarantia
        {
            get { return _dtGarantia; }
            set { _dtGarantia = value; }
        }

        public string sGarantia
        {
            get { return _sGarantia; }
            set { _sGarantia = value; }
        }

        public decimal nValorGarantia
        {
            get { return _nValorGarantia; }
            set { _nValorGarantia = value; }
        }
        public string sFuncao
        {
            get { return _sFuncao; }
            set { _sFuncao = value; }
        }

        public int idRegistro
        {
            get { return _idRegistro; }
            set { _idRegistro = value; }
        }

        public string sEnvioRemesa
        {
            get { return _sEnvioRemesa; }
            set { _sEnvioRemesa = value; }
        }

        public int idArquivo { get => _idArquivo; set => _idArquivo = value; }
        public string sNomeArquivo { get => _sNomeArquivo; set => _sNomeArquivo = value; }
        public string sObservacaoArquivo { get => _sObservacaoArquivo; set => _sObservacaoArquivo = value; }
        public byte[] objArquivo { get => _objArquivo; set => _objArquivo = value; }


        #endregion
    }

    public class cls_Variaveis
    {
        static int _nView_Voltar = 0;
        public static int nView_Voltar
        {
            get
            {
                return _nView_Voltar;
            }
            set
            {
                _nView_Voltar = value;
            }
        }

    }

    [Serializable]
    public class cls_Itens_Tabela
    {
        #region | Construtor 
        public cls_Itens_Tabela()
        {

        }

        public cls_Itens_Tabela
        (
            int idItem
            , int idNCM
            , string sCodigo
            , string sDscProduto
            , string sUnidade
            , decimal nTotal
            , string sUnidadeEntrega
            , string dtPrevisao
            , decimal nQuantidade
            , decimal nICMS
            , decimal nIPI
            , decimal nValorUnitario
            , decimal nPorcentagem
        )
        {
            _idItem = idItem;
            _idNCM = idNCM;
            _sCodigo = sCodigo;
            _sDscProduto = sDscProduto;
            _sUnidade = sUnidade;
            _nTotal = nTotal;
            _sUnidadeEntrega = sUnidadeEntrega;
            _dtPrevisao = dtPrevisao;
            _nQuantidade = nQuantidade;
            _nICMS = nICMS;
            _nIPI = nIPI;
            _nValorUnitario = nValorUnitario;
            _nPorcentagem = nPorcentagem;
        }
        #endregion

        #region | Membros Privados 
        private int _idItem;
        private int _idNCM;
        private string _sCodigo;
        private string _sDscProduto;
        private string _sUnidade;
        private decimal _nTotal;
        private string _sUnidadeEntrega;
        private string _dtPrevisao;
        private decimal _nQuantidade;
        private decimal _nICMS;
        private decimal _nIPI;
        private decimal _nValorUnitario;
        private decimal _nPorcentagem;
        #endregion

        #region | Propriedades
        public int idItem
        {
            get { return _idItem; }
            set { _idItem = value; }
        }

        public int idNCM
        {
            get { return _idNCM; }
            set { _idNCM = value; }
        }

        public string sCodigo
        {
            get { return _sCodigo; }
            set { _sCodigo = value; }
        }

        public string sDscProduto
        {
            get { return _sDscProduto; }
            set { _sDscProduto = value; }
        }

        public string sUnidade
        {
            get { return _sUnidade; }
            set { _sUnidade = value; }
        }

        public decimal nTotal
        {
            get { return _nTotal; }
            set { _nTotal = value; }
        }

        public string sUnidadeEntrega
        {
            get { return _sUnidadeEntrega; }
            set { _sUnidadeEntrega = value; }
        }

        public string dtPrevisao
        {
            get { return _dtPrevisao; }
            set { _dtPrevisao = value; }
        }

        public decimal nQuantidade
        {
            get { return _nQuantidade; }
            set { _nQuantidade = value; }
        }

        public decimal nICMS
        {
            get { return _nICMS; }
            set { _nICMS = value; }
        }

        public decimal nIPI
        {
            get { return _nIPI; }
            set { _nIPI = value; }
        }

        public decimal nValorUnitario
        {
            get { return _nValorUnitario; }
            set { _nValorUnitario = value; }
        }

        public decimal nPorcentagem
        {
            get { return _nPorcentagem; }
            set { _nPorcentagem = value; }
        }
        #endregion
    }

    [Serializable]
    public class cls_Arquivos_STSO
    {
        #region | Construtor 
        public cls_Arquivos_STSO()
        {

        }

        public cls_Arquivos_STSO
        (
            int idContador
            , int idArquivo
            , string sDscsArquivo
            , string sFuncao
            , int idPedido
            , int idArquvoSTSO
            , string sNome
            , string sTipo
            , string sDscsNomeArquivo
            , string sDescricao
            , string sEmpresaxColaborador
            , string dtAtualizacao
            , string sAprovacao
            , int idObjeto
        )
        {
            _idContador = idContador;
            _idArquivo = idArquivo;
            _sDscsArquivo = sDscsArquivo;
            _sFuncao = sFuncao;
            _idPedido = idPedido;
            _idArquvoSTSO = idArquvoSTSO;
            _sNome = sNome;
            _sTipo = sTipo;
            _sDscsNomeArquivo = sDscsNomeArquivo;
            _sDescricao = sDescricao;
            _sEmpresaxColaborador = sEmpresaxColaborador;
            _dtAtualizacao = dtAtualizacao;
            _sAprovacao = sAprovacao;
            _idObjeto = idObjeto;
        }
        #endregion

        #region | Membros Privados 
        private int _idContador;
        private int _idArquivo;
        private string _sDscsArquivo;
        private string _sFuncao;
        private int _idPedido;
        private int _idArquvoSTSO;
        private string _sNome;
        private string _sTipo;
        private string _sDscsNomeArquivo;
        private string _sDescricao;
        private string _sEmpresaxColaborador;
        private string _dtAtualizacao;
        private string _sAprovacao;
        private int _idObjeto;
        #endregion

        #region | Propriedades
        public int idContador
        {
            get { return _idContador; }
            set { _idContador = value; }
        }

        public int idArquivo
        {
            get { return _idArquivo; }
            set { _idArquivo = value; }
        }

        public string sDscsArquivo
        {
            get { return _sDscsArquivo; }
            set { _sDscsArquivo = value; }
        }

        public string sFuncao
        {
            get { return _sFuncao; }
            set { _sFuncao = value; }
        }

        public int idPedido
        {
            get { return _idPedido; }
            set { _idPedido = value; }
        }

        public int idArquvoSTSO
        {
            get { return _idArquvoSTSO; }
            set { _idArquvoSTSO = value; }
        }

        public string sNome
        {
            get { return _sNome; }
            set { _sNome = value; }
        }

        public string sTipo
        {
            get { return _sTipo; }
            set { _sTipo = value; }
        }

        public string sDscsNomeArquivo
        {
            get { return _sDscsNomeArquivo; }
            set { _sDscsNomeArquivo = value; }
        }

        public string sDescricao
        {
            get { return _sDescricao; }
            set { _sDescricao = value; }
        }

        public string sEmpresaxColaborador
        {
            get { return _sEmpresaxColaborador; }
            set { _sEmpresaxColaborador = value; }
        }

        public string dtAtualizacao
        {
            get { return _dtAtualizacao; }
            set { _dtAtualizacao = value; }
        }

        public string sAprovacao
        {
            get { return _sAprovacao; }
            set { _sAprovacao = value; }
        }

        public int idObjeto
        {
            get { return _idObjeto; }
            set { _idObjeto = value; }
        }
        #endregion
    }

    [Serializable]
    public class cls_Download_STSO
    {
        #region | Construtor 
        public cls_Download_STSO()
        {

        }

        public cls_Download_STSO
        (
            int idLink
            , string sChave
            , string sSenha
            , string dtCriacao
            , string sFuncao
        )
        {
            _idLink = idLink;
            _sChave = sChave;
            _sSenha = sSenha;
            _dtCriacao = dtCriacao;
            _sFuncao = sFuncao;
        }
        #endregion

        #region | Membros Privados 
        private int _idLink;
        private string _sChave;
        private string _sFuncao;
        private string _sSenha;
        private string _dtCriacao;
        #endregion

        #region | Propriedades
        public int idLink
        {
            get { return _idLink; }
            set { _idLink = value; }
        }

        public string sChave
        {
            get { return _sChave; }
            set { _sChave = value; }
        }

        public string sFuncao
        {
            get { return _sFuncao; }
            set { _sFuncao = value; }
        }

        public string sSenha
        {
            get { return _sSenha; }
            set { _sSenha = value; }
        }

        public string dtCriacao
        {
            get { return _dtCriacao; }
            set { _dtCriacao = value; }
        }
        #endregion
    }

    #region | Cotação
    [Serializable]
    public class cls_Pedido : cls_Pedidos_Itens
    {
        public cls_Pedido()
        {
        }
        public string sStatus { get; set; }
    }

    [Serializable]
    public class cls_Pedido_Cotacao : cls_Pedidos_Itens
    {
        private double nQuantidadeItens = 0;
        private double nEstoque = 0;


        #region | Construtor
        public cls_Pedido_Cotacao()
        {

        }
        #endregion

        #region | Membros públicos
        public double NQuantidadeItens { get => nQuantidadeItens; set => nQuantidadeItens = value; }
        public double NEstoque { get => nEstoque; set => nEstoque = value; }
        public string SDscVendaPT { get; set; }
        public string SDscVendaEN { get; set; }
        public string SDscVendaES { get; set; }

        #endregion

        #region | Métodos
        public static DataSet Pedido_ConsultarCotacao(string idParceiro, string idPedido = "", string idUsuario = "")
        {
            string sProcedure = "sp_Manipula_tbl_Flow_Pedidos_Cotacao";

            string sErro = "";
            DataSet dsPesquisa;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTA");
            vParametros.Add("@idParceiro", idParceiro);
            //vParametros.Add("@idUsuario", idUsuario);

            if (idPedido != "")
            {
                vParametros.Add("@idPedido", idPedido);
            }

            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                if (sErro != "")
                    throw new Exception("Erro ao salvar " + sErro);
                else
                {
                }
            }
            return dsPesquisa;
        }
        public static DataSet Consultar_Cotacao_Itens(string idParceiro, string idPedido = "")
        {
            string sProcedure = "sp_Manipula_tbl_Flow_Pedidos_Cotacao";

            string sErro = "";
            DataSet dsPesquisa;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@idParceiro", idParceiro);
            vParametros.Add("@sFuncao", "CONSULTA_ITENS_COTACAO");

            if (idPedido != "")
            {
                vParametros.Add("@idPedido", idPedido);
            }

            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                if (sErro != "")
                    throw new Exception("Erro ao salvar " + sErro);
                else
                {

                }
            }
            return dsPesquisa;
        }
        public static void DesativaCotacao(string idPedido, string idParceiro)
        {
            string sErro = "";

            DataSet dsPesquisa;
            string sProcedure_DesativarCotacao = "sp_Manipula_tbl_Flow_Pedidos_Cotacao";
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "DESATIVAR_COTACAO");
            vParametros.Add("@idParceiro", idParceiro);
            vParametros.Add("@sAtivo", "N");
            vParametros.Add("@idPedido", idPedido);

            dsPesquisa = BD.ExecutarDataSet(sProcedure_DesativarCotacao, vParametros);
            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                if (sErro != "")
                {
                    throw new Exception("Erro ao salvar " + sErro);
                }
            }


        }
        public static DataSet Pedido_ConsultarCotacao_Detalhe(string idPedido, string idTipo)
        {
            string sProcedure = "sp_Manipula_tbl_Flow_Pedidos_Cotacao";
            string sFuncao = "CONSULTA_DETALHE";
            string sErro = "";
            DataSet dsPesquisa;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFucao", sFuncao);
            vParametros.Add("@idPedido", idPedido);
            vParametros.Add("@idTipo", idTipo);

            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                if (sErro != "")
                    throw new Exception("Erro ao salvar " + sErro);
            }
            return dsPesquisa;
        }
        public static DataSet Pedido_ConsultarCotacao_Log(string idPedido)
        {
            string sProcedure = "sp_Manipula_tbl_Flow_Pedidos_Cotacao";
            string sFuncao = "CONSULTA_LOG";

            string sErro = "";
            DataSet dsPesquisa;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idPedido", idPedido);

            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                if (sErro != "")
                    throw new Exception("Erro ao salvar " + sErro);
            }
            return dsPesquisa;
        }
        public static void Cotacao_ConverterDS_BD<T>(DataSet ds, List<T> lista) where T : cls_Pedidos_Itens
        {

            lista.Clear();
            foreach (DataTable dt in ds.Tables)
            {
                if (!dt.Columns.Contains("idPedido"))
                    return;
                foreach (DataRow row in dt.Rows)
                {
                    try
                    {
                        //cabeçalho
                        if (typeof(T) == typeof(cls_Pedido))
                        {
                            cls_Pedido item = new cls_Pedido();

                            double nValorTotal;
                            bool conversaoBemSucedida = double.TryParse(row["nValorTotal"].ToString(), out nValorTotal);
                            item.nValorTotal = conversaoBemSucedida ? nValorTotal : 0.0;
                            lista.Add((T)(object)item);
                            item.sStatus = row["sStatus"].ToString();
                            item.idPedido = Convert.ToInt32(row["idPedido"].ToString());
                            //item.DtInclusao = Convert.ToDateTime(row["dtInclusao"].ToString());

                            DateTime dtInclusao;
                            conversaoBemSucedida = DateTime.TryParse(row["dtInclusao"].ToString(), out dtInclusao);
                            item.DtInclusao = conversaoBemSucedida ? dtInclusao : DateTime.Today;


                            int idCondicaoPagamento;
                            conversaoBemSucedida = Int32.TryParse(row["idCondicaoPagamento"].ToString(), out idCondicaoPagamento);
                            item.IdCondicaoPagamento = conversaoBemSucedida ? idCondicaoPagamento : 0;

                            int idEnderecoEntrega;
                            conversaoBemSucedida = Int32.TryParse(row["idEnderecoEntrega"].ToString(), out idEnderecoEntrega);
                            item.IdEndereco = conversaoBemSucedida ? idEnderecoEntrega : 0;

                            int idUsuario;
                            conversaoBemSucedida = Int32.TryParse(row["idUsuario"].ToString(), out idUsuario);
                            item.IdUsuario = idUsuario;

                            //Antonio
                            item.sDscParceiro = row["sDscParceiro"].ToString();
                            item.IdCliente = Convert.ToInt32(row["idParceiro"].ToString());
                            item.sDscUsuario = row["sDscUsuario"].ToString();
                            item.idCotacao = Convert.ToInt32(row["idCotacao"].ToString());

                        }//itens
                        else if (typeof(T) == typeof(cls_Pedido_Cotacao))
                        {
                            cls_Pedido_Cotacao item = new cls_Pedido_Cotacao();
                            item.idItem = Convert.ToInt32(row["idItem"]);
                            item.idPedido = Convert.ToInt32(row["idPedido"]);
                            item.idProduto = Convert.ToInt32(row["idProduto"]);
                            double nQuantidade;
                            bool conversaoBemSucedida = double.TryParse(row["nQuantidade"].ToString(), out nQuantidade);
                            item.nQuantidadeItens = conversaoBemSucedida ? nQuantidade : 0.0;

                            double nValorUnitario;
                            conversaoBemSucedida = double.TryParse(row["nValorUnitario"].ToString(), out nValorUnitario);
                            item.nValorUnitario = conversaoBemSucedida ? nValorUnitario : 0.0;

                            item.nValorTotal = item.nValorUnitario * item.nQuantidadeItens;

                            item.sCodigoProduto = row["sCodigo"].ToString();
                            item.sDscProduto = row["sDscProduto"].ToString();
                            lista.Add((T)(object)item);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
        }
        public static decimal ConverterStringDecimal(string dado)
        {
            if (decimal.TryParse(dado, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal converter))
            {
                return converter;
            }
            return decimal.Zero;
        }
        //public static void Pedido_Consultar_Parceiro_x_Usuario(string idParceiro, string idUsuario = "0")
        //{
        //    DataSet ds;
        //    Dictionary<string,string> vParametros = new Dictionary<string,string>();
        //    vParametros.Add("@sFuncao", "CONSULTA_USUARIOS_X_PARCEIROS");
        //    vParametros.Add("@idCliente", idUsuario);

        //}
        //apenas para forçar, estamos considerando a mesma seja do tipo 4 --> cotação (foi editado na tabela pedidos_tipo = 4)
        public static void Salvar(List<cls_Pedido> Cotacao, List<cls_Pedido_Cotacao> Itens, int idParceiro, int idUsuario = 0)
        {
            int idCotacao = 0;
            string sErro = "";
            string sProcedure_Salvar = "sp_Manipula_tbl_Flow_Pedidos_Cotacao";
            DataSet ds;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();

            vParametros.Add("@sFuncao", "INCLUIR_COTACAO");
            vParametros.Add("@idPedido", "");
            vParametros.Add("@idParceiro", "");
            vParametros.Add("@idCliente", "");
            vParametros.Add("@idCondicaoPagamento", "");
            vParametros.Add("@idEnderecoEntrega", "");
            vParametros.Add("@idStatus", "");
            vParametros.Add("@idTipo", "4");
            vParametros.Add("@idUsuario", idUsuario.ToString());

            Dictionary<string, string> vParametros_Itens = new Dictionary<string, string>();
            vParametros_Itens.Add("@sFuncao", "INCLUIR_ITEM");
            vParametros_Itens.Add("@idProduto", "");
            vParametros_Itens.Add("@nQuantidade", "");
            vParametros_Itens.Add("@nValorUnitario", "");
            vParametros_Itens.Add("@idParceiro", "");
            vParametros_Itens.Add("@idPedido", "");
            vParametros_Itens.Add("@idItem", "");
            vParametros_Itens.Add("@idUsuarioInclusao", idUsuario.ToString());

            Cotacao.ForEach(cot =>
            {
                vParametros["@idPedido"] = cot.idPedido.ToString();
                vParametros["@idParceiro"] = idParceiro.ToString();
                vParametros["@idCondicaoPagamento"] = cot.IdCondicaoPagamento.ToString();
                vParametros["@idStatus"] = cot.IdStatus.ToString();
                vParametros["@idEnderecoEntrega"] = cot.IdEndereco.ToString();


                ds = BD.ExecutarDataSet(sProcedure_Salvar, vParametros);
                if (BD.ValidarDataSet(ds, out sErro))
                {
                    if (sErro != "")
                        throw new Exception("Erro ao salvar " + sErro);
                    else //salva os itens a partir do número do ID da cotação
                    {
                        idCotacao = Convert.ToInt32(RETORNO.DATASET(ds, 0, "idPedido"));
                        if (cot.idPedido == 0)
                        {
                            cot.sFuncao = "INCLUIR_COTACAO";
                        }
                        else
                            cot.sFuncao = "";
                        cot.idPedido = idCotacao;

                    }
                }

                Itens.ForEach(item =>
                {
                    if (cot.idPedido == item.idPedido && sErro == "" || cot.sFuncao == "INCLUIR_COTACAO")
                    {
                        vParametros_Itens["@sFuncao"] = item.sFuncao;
                        vParametros_Itens["@idProduto"] = item.idProduto.ToString();
                        vParametros_Itens["@nQuantidade"] = item.NQuantidadeItens.ToString();
                        vParametros_Itens["@nValorUnitario"] = item.nValorUnitario.ToString().Replace(",", ".");
                        vParametros_Itens["@idParceiro"] = idParceiro.ToString();
                        vParametros_Itens["@idPedido"] = cot.idPedido.ToString();
                        vParametros_Itens["@idUsuarioInclusao"] = idUsuario.ToString();

                        if (item.sFuncao == "EXCLUIR_ITEM")
                        {
                            vParametros_Itens["@idItem"] = item.idItem.ToString();
                        }

                        if (!string.IsNullOrEmpty(item.sFuncao))
                        {
                            try
                            {
                                ds = BD.ExecutarDataSet(sProcedure_Salvar, vParametros_Itens);

                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message, ex);
                            }

                        }

                    }
                });

            });


        }


        #endregion

        #region | Membros Privados
        #endregion
    }
    #endregion
}