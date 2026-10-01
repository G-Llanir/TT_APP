using System;
using System.Collections.Generic;

namespace TT_Flow.FrameWork
{
    #region | EPI

    [Serializable]
    public class cls_EPI_Itens
    {
        #region | Construtor 
        public cls_EPI_Itens()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_EPI_Itens
        (
                int idColaborador
            , int idRegistroEPI
            , int idEPI
            , string sEPI
            , string sTamanhoEPI
            , string dtRecebimentoEPI
            , string dtVencimentoEPI
            , string sObservacaoEPI
            , string sQuantidadeEPI
            , string sCA

            , int idArquivo
            , int idLinha
            , string sFuncao

        )
        {

            _idColaborador = idColaborador;
            _idEPI = idEPI;
            _sEPI = sEPI;
            _sTamanhoEPI = sTamanhoEPI;
            _dtRecebimentoEPI = dtRecebimentoEPI;
            _dtVencimentoEPI = dtVencimentoEPI;
            _sObservacaoEPI = sObservacaoEPI;
            _sQuantidadeEPI = sQuantidadeEPI;
            _sCA = sCA;

            _idArquivo = idArquivo;
            _idLinha = idLinha;
            _sFuncao = sFuncao;
            _idRegistroEPI = idRegistroEPI;
        }

        #endregion

        #region | Membros Privados 

        private int _idColaborador;
        private int _idEPI;
        private string _sEPI;
        private string _sTamanhoEPI;
        private string _dtRecebimentoEPI;
        private string _dtVencimentoEPI;
        private string _sObservacaoEPI;
        private string _sQuantidadeEPI;
        private string _sCA;

        private int _idArquivo;
        private string _sNomeArquivo;
        private string _sObservacaoArquivo;
        private Byte[] _objArquivo;
        private string _sFuncao;
        private int _idLinha;
        private int _idRegistroEPI;


        #endregion

        #region | Propriedades


        public int idColaborador
        {
            get { return _idColaborador; }
            set { _idColaborador = value; }
        }
        public int idEPI
        {
            get { return _idEPI; }
            set { _idEPI = value; }
        }

        public string sEPI
        {
            get { return _sEPI; }
            set { _sEPI = value; }
        }

        public string sTamanhoEPI
        {
            get { return _sTamanhoEPI; }
            set { _sTamanhoEPI = value; }
        }

        public string dtRecebimentoEPI
        {
            get { return _dtRecebimentoEPI; }
            set { _dtRecebimentoEPI = value; }
        }


        public string dtVencimentoEPI
        {
            get { return _dtVencimentoEPI; }
            set { _dtVencimentoEPI = value; }
        }


        public string sObservacaoEPI
        {
            get { return _sObservacaoEPI; }
            set { _sObservacaoEPI = value; }
        }

        public string sQuantidadeEPI
        {
            get { return _sQuantidadeEPI; }
            set { _sQuantidadeEPI = value; }
        }


        public string sCA
        {
            get { return _sCA; }
            set { _sCA = value; }
        }

        public int idArquivo { get => _idArquivo; set => _idArquivo = value; }
        public string sNomeArquivo { get => _sNomeArquivo; set => _sNomeArquivo = value; }
        public string sObservacaoArquivo { get => _sObservacaoArquivo; set => _sObservacaoArquivo = value; }
        public byte[] objArquivo { get => _objArquivo; set => _objArquivo = value; }
        public string sFuncao { get => _sFuncao; set => _sFuncao = value; }
        public int idLinha { get => _idLinha; set => _idLinha = value; }
        public int idRegistroEPI { get => _idRegistroEPI; set => _idRegistroEPI = value; }


        #endregion
    }

    [Serializable]
    public class cls_ItemEPI
     {
        public int idItem { get; set; }
        public int nOrdem { get; set; }
        public string sCodigo { get; set; }
        public string sDscProduto { get; set; }
        public int nCA { get; set; }
        public int nQuantidadeEPI { get; set; }
        public string sPeriodo { get; set; }
        public int idEntregaEPI { get; set; }
        public string dtVencimento { get; set; }
        public bool bMaisRecente { get; set; }

    }

    [Serializable]
    public class cls_EntregaEPI
    {
        public int idEntregaEPI { get; set; }
        public int idArquivo { get; set; }
        public string sPeriodicidade { get; set; }
        public string sTipoEntrega { get; set; }
        public string sDscStatus { get; set; }
        public string dtSolicitacao { get; set; }
        public string dtConfirmacao { get; set; }
        public string dtEntrega { get; set; }
        public DateTime dtEntrega_Date { get { if (DateTime.TryParse(dtEntrega, out DateTime data)) return data; else return DateTime.Parse("1900-01-01"); } }
        public List<cls_ItemEPI> lsItensEPI { get; set; } 
        public int nQtd { get { return lsItensEPI.Count; } }
    }

    #endregion

    #region | NR

    [Serializable]
    public class cls_NR_Itens
    {
        #region | Construtor 
        public cls_NR_Itens()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public cls_NR_Itens
        (
             int idLinha
            , string sFuncao
            , int idColaborador
            , int idRegistroNR
            , int idArquivo
            , int idNR
            , string sTipoNR
            , string sDscNR
            , string dtEmissaoNR
            , string dtVencimentoNR
            , int idTipoNr
            , string sDscArquivo
        )
        {
            _idLinha = idLinha;
            _sFuncao = sFuncao;
            _idColaborador = idColaborador;
            _idRegistroNR = idRegistroNR;
            _idArquivo = idArquivo;
            _idNR = idNR;
            _sTipoNR = sTipoNR;
            _sDscNR = sDscNR;
            _dtEmissaoNR = dtEmissaoNR;
            _dtVencimentoNR = dtVencimentoNR;
            _idTipoNr = idTipoNr;
            _sDscArquivo = sDscArquivo;
        }

        #endregion

        #region | Membros Privados 

        private int _idLinha;
        private string _sFuncao;
        private int _idColaborador;
        private int _idRegistroNR;
        private int _idArquivo;
        private string _sNomeArquivo;
        private string _sObservacaoArquivo;
        private Byte[] _objArquivo;
        private int _idNR;
        private string _sTipoNR;
        private string _sDscNR;
        private string _dtEmissaoNR;
        private string _dtVencimentoNR;
        private int _idTipoNr;
        private string _sDscArquivo;


        #endregion

        #region | Propriedades

        public int idLinha
        {
            get { return _idLinha; }
            set { _idLinha = value; }
        }

        public string sFuncao
        {
            get { return _sFuncao; }
            set { _sFuncao = value; }
        }
        public int idColaborador
        {
            get { return _idColaborador; }
            set { _idColaborador = value; }
        }
        public int idRegistroNR
        {
            get { return _idRegistroNR; }
            set { _idRegistroNR = value; }
        }

        public int idArquivo
        {
            get { return _idArquivo; }
            set { _idArquivo = value; }
        }
        public string sNomeArquivo
        {
            get { return _sNomeArquivo; }
            set { _sNomeArquivo = value; }
        }

        public string sObservacaoArquivo
        {
            get { return _sObservacaoArquivo; }
            set { _sObservacaoArquivo = value; }
        }


        public Byte[] objArquivo
        {
            get { return _objArquivo; }
            set { _objArquivo = value; }

        }


        public int idNR
        {
            get { return _idNR; }
            set { _idNR = value; }
        }

        public string sTipoNR
        {
            get { return _sTipoNR; }
            set { _sTipoNR = value; }
        }

        public string sDscNR
        {
            get { return _sDscNR; }
            set { _sDscNR = value; }
        }


        public string dtEmissaoNR
        {
            get { return _dtEmissaoNR; }
            set { _dtEmissaoNR = value; }
        }


        public string dtVencimentoNR
        {
            get { return _dtVencimentoNR; }
            set { _dtVencimentoNR = value; }
        }

        public int idTipoNr { get => _idTipoNr; set => _idTipoNr = value; }

        public string sDscArquivo
        {
            get { return _sDscArquivo; }
            set { _sDscArquivo = value; }
        }

        #endregion
    }

    #endregion

    #region | Evento

    [Serializable]
    public class cls_Evento
    {
        #region | Construtor 
        public cls_Evento()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_Evento
        (
              int idRegistroEvento
            , int idColaborador
            , int idEvento
            , string sDscEvento
            , string sReincidencia
            , string sObservacaoEvento
            , string sGravidadeEvento
            , string sTipoEvento
            , string dtEvento
            , string sCustoOcorrencia
            , decimal nValorOcorrencia
            , string sResponsavelOcorrencia
            , string sAfastamentoEvento
            , string dtInicioAfastamento
            , string dtRetornoAfastamento
            , string sObservacaoOcorrencia
            , string sCodReferencia
            , int idGravidade
            , string sDscGravidade

            , int idReincidencia
            , int idTipoEvento
            , int idCustoOcorrencia
            , int idResponsavelOcorrencia
            , int idAfastamentoEvento


            , int idArquivo
            , int idLinha
            , string sFuncao

        )
        {
            _idRegistroEvento = idRegistroEvento;
            _idColaborador = idColaborador;
            _idEvento = idEvento;
            _sDscEvento = sDscEvento;
            _sReincidencia = sReincidencia;
            _sObservacaoEvento = sObservacaoEvento;
            _sGravidadeEvento = sGravidadeEvento;
            _sTipoEvento = sTipoEvento;
            _dtEvento = dtEvento;
            _sCustoOcorrencia = sCustoOcorrencia;
            _nValorOcorrencia = nValorOcorrencia;
            _sResponsavelOcorrencia = sResponsavelOcorrencia;
            _sAfastamentoEvento = sAfastamentoEvento;
            _dtInicioAfastamento = dtInicioAfastamento;
            _dtRetornoAfastamento = dtRetornoAfastamento;
            _sObservacaoOcorrencia = sObservacaoOcorrencia;
            _sCodReferencia = sCodReferencia;
            _idGravidade = idGravidade;
            _sDscGravidade = sDscGravidade;
            _idReincidencia = idReincidencia;
            _idTipoEvento = idTipoEvento;
            _idCustoOcorrencia = idCustoOcorrencia;
            _idResponsavelOcorrencia = idResponsavelOcorrencia;
            _idAfastamentoEvento = idAfastamentoEvento;

            _idArquivo = idArquivo;
            _idLinha = idLinha;
            _sFuncao = sFuncao;
        }

        #endregion

        #region | Membros Privados 

        private int _idRegistroEvento;
        private int _idColaborador;
        private int _idEvento;
        private string _sDscEvento;
        private string _sTamanhoEPI;
        private string _sReincidencia;
        private string _sObservacaoEvento;
        private string _sGravidadeEvento;
        private string _sTipoEvento;
        private string _dtEvento;
        private string _sCustoOcorrencia;
        private decimal _nValorOcorrencia;
        private string _sResponsavelOcorrencia;
        private string _sAfastamentoEvento;
        private string _dtInicioAfastamento;
        private string _dtRetornoAfastamento;
        private string _sObservacaoOcorrencia;
        private string _sCodReferencia;
        private int _idGravidade;
        private string _sDscGravidade;
        private int _idReincidencia;
        private int _idTipoEvento;
        private int _idCustoOcorrencia;
        private int _idResponsavelOcorrencia;
        private int _idAfastamentoEvento;

        private int _idArquivo;
        private string _sNomeArquivo;
        private string _sObservacaoArquivo;
        private Byte[] _objArquivo;
        private string _sFuncao;
        private int _idLinha;

        #endregion

        #region | Propriedades


        public int idColaborador
        {
            get { return _idColaborador; }
            set { _idColaborador = value; }
        }
        public int idEvento
        {
            get { return _idEvento; }
            set { _idEvento = value; }
        }

        public string sDscEvento
        {
            get { return _sDscEvento; }
            set { _sDscEvento = value; }
        }

        public string sTamanhoEPI
        {
            get { return _sTamanhoEPI; }
            set { _sTamanhoEPI = value; }
        }

        public string sReincidencia
        {
            get { return _sReincidencia; }
            set { _sReincidencia = value; }
        }


        public string sObservacaoEvento
        {
            get { return _sObservacaoEvento; }
            set { _sObservacaoEvento = value; }
        }

        public string sGravidadeEvento { get => _sGravidadeEvento; set => _sGravidadeEvento = value; }
        public string sTipoEvento { get => _sTipoEvento; set => _sTipoEvento = value; }
        public string dtEvento { get => _dtEvento; set => _dtEvento = value; }
        public string sCustoOcorrencia { get => _sCustoOcorrencia; set => _sCustoOcorrencia = value; }
        public decimal nValorOcorrencia { get => _nValorOcorrencia; set => _nValorOcorrencia = value; }
        public string sResponsavelOcorrencia { get => _sResponsavelOcorrencia; set => _sResponsavelOcorrencia = value; }
        public string sAfastamentoEvento { get => _sAfastamentoEvento; set => _sAfastamentoEvento = value; }
        public string dtInicioAfastamento { get => _dtInicioAfastamento; set => _dtInicioAfastamento = value; }
        public string dtRetornoAfastamento { get => _dtRetornoAfastamento; set => _dtRetornoAfastamento = value; }
        public string sObservacaoOcorrencia { get => _sObservacaoOcorrencia; set => _sObservacaoOcorrencia = value; }
        public string sCodReferencia { get => _sCodReferencia; set => _sCodReferencia = value; }
        public int idRegistroEvento { get => _idRegistroEvento; set => _idRegistroEvento = value; }
        public int idArquivo { get => _idArquivo; set => _idArquivo = value; }
        public string sNomeArquivo { get => _sNomeArquivo; set => _sNomeArquivo = value; }
        public string sObservacaoArquivo { get => _sObservacaoArquivo; set => _sObservacaoArquivo = value; }
        public byte[] objArquivo { get => _objArquivo; set => _objArquivo = value; }
        public string sFuncao { get => _sFuncao; set => _sFuncao = value; }
        public int idLinha { get => _idLinha; set => _idLinha = value; }
        public int idGravidade { get => _idGravidade; set => _idGravidade = value; }
        public string sDscGravidade { get => _sDscGravidade; set => _sDscGravidade = value; }
        public int idReincidencia { get => _idReincidencia; set => _idReincidencia = value; }
        public int idTipoEvento { get => _idTipoEvento; set => _idTipoEvento = value; }
        public int idCustoOcorrencia { get => _idCustoOcorrencia; set => _idCustoOcorrencia = value; }
        public int idResponsavelOcorrencia { get => _idResponsavelOcorrencia; set => _idResponsavelOcorrencia = value; }
        public int idAfastamentoEvento { get => _idAfastamentoEvento; set => _idAfastamentoEvento = value; }

        #endregion
    }

    #endregion

    #region | Ausencia

    [Serializable]
    public class cls_Ausencia
    {
        #region | Construtor 
        public cls_Ausencia()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_Ausencia
        (
            int idRegistroAtestado
            , int idColaborador
            , int idAtestado
            , string sDscAtestado
            , DateTime dtInicioAtestado
            , DateTime dtRetornoAtestado
            , string sObservacaoAtestado
            , double nHorasAtestado

            , int idTipoAtestado
            , decimal nQuantidadeHoras
            , string sTipoAtestado

            , int idArquivo
            , int idLinha
            , string sFuncao
            , string sCID
        )
        {
            _idRegistroAtestado = idRegistroAtestado;
            _idColaborador = idColaborador;
            _idAtestado = idAtestado;
            _sDscAtestado = sDscAtestado;
            _dtInicioAtestado = dtInicioAtestado;
            _dtRetornoAtestado = dtRetornoAtestado;
            _sObservacaoAtestado = sObservacaoAtestado;
            _nHorasAtestado = nHorasAtestado;

            _idTipoAtestado = idTipoAtestado;
            _nQuantidadeHoras = nQuantidadeHoras;
            _sTipoAtestado = sTipoAtestado;

            _idArquivo = idArquivo;
            _idLinha = idLinha;
            _sFuncao = sFuncao;
            _sCID = sCID;

        }

        #endregion

        #region | Membros Privados 

        private int _idRegistroAtestado;
        private int _idColaborador;
        private int _idAtestado;
        private string _sDscAtestado;
        private string _sTamanhoEPI;
        private DateTime _dtInicioAtestado;
        private DateTime _dtRetornoAtestado;
        private string _sObservacaoAtestado;
        private double _nHorasAtestado;

        private int _idTipoAtestado;
        private decimal _nQuantidadeHoras;
        private string _sTipoAtestado;
        private string _sTipoAusencia;
        private string _sMotivoAtestado;

        private int _idArquivo;
        private string _sNomeArquivo;
        private string _sObservacaoArquivo;
        private Byte[] _objArquivo;
        private string _sFuncao;
        private int _idLinha;
        private string _sDescontoVT;
        private string _sDescontoVR;
        private string _sDescontoDSR;

        private string _sCID; 

        #endregion

        #region | Propriedades


        public int idColaborador
        {
            get { return _idColaborador; }
            set { _idColaborador = value; }
        }
        public int idAtestado
        {
            get { return _idAtestado; }
            set { _idAtestado = value; }
        }

        public string sDscAtestado
        {
            get { return _sDscAtestado; }
            set { _sDscAtestado = value; }
        }

        public string sTamanhoEPI
        {
            get { return _sTamanhoEPI; }
            set { _sTamanhoEPI = value; }
        }

        public DateTime dtInicioAtestado
        {
            get { return _dtInicioAtestado; }
            set { _dtInicioAtestado = value; }
        }


        public DateTime dtRetornoAtestado
        {
            get { return _dtRetornoAtestado; }
            set { _dtRetornoAtestado = value; }
        }


        public string sObservacaoAtestado
        {
            get { return _sObservacaoAtestado; }
            set { _sObservacaoAtestado = value; }
        }

        public double nHorasAtestado { get => _nHorasAtestado; set => _nHorasAtestado = value; }
        public int idArquivo { get => _idArquivo; set => _idArquivo = value; }
        public string sNomeArquivo { get => _sNomeArquivo; set => _sNomeArquivo = value; }
        public string sObservacaoArquivo { get => _sObservacaoArquivo; set => _sObservacaoArquivo = value; }
        public byte[] objArquivo { get => _objArquivo; set => _objArquivo = value; }
        public string sFuncao { get => _sFuncao; set => _sFuncao = value; }
        public int idLinha { get => _idLinha; set => _idLinha = value; }
        public int idRegistroAtestado { get => _idRegistroAtestado; set => _idRegistroAtestado = value; }
        public int idTipoAtestado { get => _idTipoAtestado; set => _idTipoAtestado = value; }
        public string sTipoAusencia { get => _sTipoAusencia; set => _sTipoAusencia = value; }
        public string sTipoAtestado { get => _sTipoAtestado; set => _sTipoAtestado = value; }
        public string sMotivoAtestado { get => _sMotivoAtestado; set => _sMotivoAtestado = value; }
        public string sDescontoVT { get => _sDescontoVT; set => _sDescontoVT = value; }
        public string sDescontoVR { get => _sDescontoVR; set => _sDescontoVR = value; }
        public string sDescontoDSR { get => _sDescontoDSR; set => _sDescontoDSR = value; }
        public string sCID { get => _sCID; set => _sCID = value; }

        #endregion
    }

    #endregion

    #region | Credito

    [Serializable]
    public class cls_Credito
    {
        #region | Construtor 
        public cls_Credito()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_Credito
        (
            int idRegistro
            , int idColaborador
            , int idCredito
            , string sDscJustificativa
            , DateTime dtInicioCredito
            , DateTime dtRetornoCredito
            , string sObservacaoCredito
            , double nHorasCredito

            , int idTipoCredito
            , decimal nQuantidadeHoras
            , string sTipoCredito
            , string sCreditaVT
            , string sCreditaVR

            , int idArquivo
            , int idLinha
            , string sFuncao
        )
        {
            _idRegistro = idRegistro;
            _idColaborador = idColaborador;
            _idCredito = idCredito;
            _sDscJustificativa = sDscJustificativa;
            _dtInicioCredito = dtInicioCredito;
            _dtRetornoCredito = dtRetornoCredito;
            _sObservacaoCredito = sObservacaoCredito;
            _nHorasCredito = nHorasCredito;

            _idTipoCredito = idTipoCredito;
            _nQuantidadeHoras = nQuantidadeHoras;
            _sTipoCredito = sTipoCredito;

            _idArquivo = idArquivo;
            _idLinha = idLinha;
            _sFuncao = sFuncao;
            _sCreditaVR = sCreditaVR;
            _sCreditaVT = sCreditaVT;
        }

        #endregion

        #region | Membros Privados 

        private int _idRegistro;
        private int _idColaborador;
        private int _idCredito;
        private string _sDscJustificativa;
        private DateTime _dtInicioCredito;
        private DateTime _dtRetornoCredito;
        private string _sObservacaoCredito;
        private double _nHorasCredito;

        private int _idTipoCredito;
        private decimal _nQuantidadeHoras;
        private string _sTipoCredito;

        private string _sCreditaVR;
        private string _sCreditaVT;

        private int _idArquivo;
        private string _sNomeArquivo;
        private string _sObservacaoArquivo;
        private Byte[] _objArquivo;
        private string _sFuncao;
        private int _idLinha;

        #endregion

        #region | Propriedades


        public int idColaborador
        {
            get { return _idColaborador; }
            set { _idColaborador = value; }
        }
        public int idCredito
        {
            get { return _idCredito; }
            set { _idCredito = value; }
        }

        public string sDscJustificativa
        {
            get { return _sDscJustificativa; }
            set { _sDscJustificativa = value; }
        }

        public string sCreditaVR
        {
            get { return _sCreditaVR; }
            set { _sCreditaVR = value; }
        }

        public string sCreditaVT
        {
            get { return _sCreditaVT; }
            set { _sCreditaVT = value; }
        }

        public DateTime dtInicioCredito
        {
            get { return _dtInicioCredito; }
            set { _dtInicioCredito = value; }
        }


        public DateTime dtRetornoCredito
        {
            get { return _dtRetornoCredito; }
            set { _dtRetornoCredito = value; }
        }


        public string sObservacaoCredito
        {
            get { return _sObservacaoCredito; }
            set { _sObservacaoCredito = value; }
        }

        public double nHorasCredito { get => _nHorasCredito; set => _nHorasCredito = value; }
        public int idArquivo { get => _idArquivo; set => _idArquivo = value; }
        public string sNomeArquivo { get => _sNomeArquivo; set => _sNomeArquivo = value; }
        public string sObservacaoArquivo { get => _sObservacaoArquivo; set => _sObservacaoArquivo = value; }
        public byte[] objArquivo { get => _objArquivo; set => _objArquivo = value; }
        public string sFuncao { get => _sFuncao; set => _sFuncao = value; }
        public int idLinha { get => _idLinha; set => _idLinha = value; }
        public int idRegistro { get => _idRegistro; set => _idRegistro = value; }
        public int idTipoCredito { get => _idTipoCredito; set => _idTipoCredito = value; }
        public decimal nQuantidadeHoras { get => _nQuantidadeHoras; set => _nQuantidadeHoras = value; }
        public string sTipoCredito { get => _sTipoCredito; set => _sTipoCredito = value; }

        #endregion
    }

    #endregion

    #region | Veículos

    [Serializable]
    public class cls_Ocorrencias
    {
        #region | Construtor 
        public cls_Ocorrencias(string sVeiculoOcorrencia = null)
        {
            _sVeiculoOcorrencia = sVeiculoOcorrencia;
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_Ocorrencias
        (
            int idRegistroOcorrencia
            , int idColaborador
            , int idOcorrencia
            , string sDscInfracao
            , string dtOcorrencia
            , string dtLimiteIndicarCondutor
            , string dtLimitePagamento
            , decimal nValor
            , string sPontos
            , string sPlaca
            , string sMarca
            , string sModelo
            , string sStatus
            , string sAIT
            , string sObservacaoInfracao
            , string sResponsavel
            , string sTipoOcorrencia
            , string dtInfracao
            , string sVeiculoOcorrencia

            , int idArquivo
            , int idLinha
            , string sFuncao

            , int idResponsavelVeiculo
            , int idVeiculoOcorrencia
            , int idStatusOcorrencia
            , int idTipoOcorrencia
        )
        {

            _idColaborador = idColaborador;
            _idRegistroOcorrencia = idRegistroOcorrencia;
            _idOcorrencia = idOcorrencia;
            _sDscInfracao = sDscInfracao;
            _dtOcorrencia = dtOcorrencia;
            _dtLimiteIndicarCondutor = dtLimiteIndicarCondutor;
            _dtLimitePagamento = dtLimitePagamento;
            _nValor = nValor;
            _sPontos = sPontos;
            _sPlaca = sPlaca;
            _sMarca = sMarca;
            _sModelo = sModelo;
            _sStatus = sStatus;
            _sAIT = sAIT;
            _sObservacaoInfracao = sObservacaoInfracao;
            _sResponsavel = sResponsavel;
            _sTipoOcorrencia = sTipoOcorrencia;
            _dtInfracao = dtInfracao;
            _sVeiculoOcorrencia = sVeiculoOcorrencia;

            _idArquivo = idArquivo;
            _idLinha = idLinha;
            _sFuncao = sFuncao;

            _idResponsavelVeiculo = idResponsavelVeiculo;
            _idVeiculoOcorrencia = idVeiculoOcorrencia;
            _idStatusOcorrencia = idStatusOcorrencia;
            _idTipoOcorrencia = idTipoOcorrencia;
        }

        #endregion

        #region | Membros Privados 

        private int _idRegistroOcorrencia;
        private int _idOcorrencia;
        private int _idColaborador;
        private string _sDscInfracao;
        private string _dtOcorrencia;
        private string _dtLimiteIndicarCondutor;
        private string _dtLimitePagamento;
        private decimal _nValor;
        private string _sPontos;
        private string _sPlaca;
        private string _sMarca;
        private string _sModelo;
        private string _sStatus;
        private string _sAIT;
        private string _sObservacaoInfracao;
        private string _sResponsavel;
        private string _sTipoOcorrencia;
        private string _dtInfracao;
        private string _sVeiculoOcorrencia;

        private int _idArquivo;
        private string _sNomeArquivo;
        private string _sObservacaoArquivo;
        private Byte[] _objArquivo;
        private string _sFuncao;
        private int _idLinha;

        private int _idResponsavelVeiculo;
        private int _idVeiculoOcorrencia;
        private int _idStatusOcorrencia;
        private int _idTipoOcorrencia;

        #endregion

        #region | Propriedades


        public int idColaborador
        {
            get { return _idColaborador; }
            set { _idColaborador = value; }
        }
        public int idOcorrencia
        {
            get { return _idOcorrencia; }
            set { _idOcorrencia = value; }
        }

        public string sDscInfracao
        {
            get { return _sDscInfracao; }
            set { _sDscInfracao = value; }
        }

        public string dtOcorrencia
        {
            get { return _dtOcorrencia; }
            set { _dtOcorrencia = value; }
        }

        public string dtLimiteIndicarCondutor
        {
            get { return _dtLimiteIndicarCondutor; }
            set { _dtLimiteIndicarCondutor = value; }
        }


        public string dtLimitePagamento
        {
            get { return _dtLimitePagamento; }
            set { _dtLimitePagamento = value; }
        }

        public decimal nValor
        {
            get { return _nValor; }
            set { _nValor = value; }
        }


        public string sPontos
        {
            get { return _sPontos; }
            set { _sPontos = value; }
        }

        public string sPlaca
        {
            get { return _sPlaca; }
            set { _sPlaca = value; }
        }

        public string sMarca
        {
            get { return _sMarca; }
            set { _sMarca = value; }
        }


        public string sModelo
        {
            get { return _sModelo; }
            set { _sModelo = value; }
        }

        public string sStatus
        {
            get { return _sStatus; }
            set { _sStatus = value; }
        }

        public string sAIT
        {
            get { return _sAIT; }
            set { _sAIT = value; }
        }


        public string sObservacaoInfracao
        {
            get { return _sObservacaoInfracao; }
            set { _sObservacaoInfracao = value; }
        }

        public string sResponsavel
        {
            get { return _sResponsavel; }
            set { _sResponsavel = value; }
        }

        public string sTipoOcorrencia { get => _sTipoOcorrencia; set => _sTipoOcorrencia = value; }
        public string dtInfracao { get => _dtInfracao; set => _dtInfracao = value; }
        public string sVeiculoOcorrencia { get => _sVeiculoOcorrencia; set => _sVeiculoOcorrencia = value; }
        public int idArquivo { get => _idArquivo; set => _idArquivo = value; }
        public string sNomeArquivo { get => _sNomeArquivo; set => _sNomeArquivo = value; }
        public string sObservacaoArquivo { get => _sObservacaoArquivo; set => _sObservacaoArquivo = value; }
        public byte[] objArquivo { get => _objArquivo; set => _objArquivo = value; }
        public string sFuncao { get => _sFuncao; set => _sFuncao = value; }
        public int idLinha { get => _idLinha; set => _idLinha = value; }
        public int idRegistroOcorrencia { get => _idRegistroOcorrencia; set => _idRegistroOcorrencia = value; }
        public int idResponsavelVeiculo { get => _idResponsavelVeiculo; set => _idResponsavelVeiculo = value; }
        public int idVeiculoOcorrencia { get => _idVeiculoOcorrencia; set => _idVeiculoOcorrencia = value; }
        public int idStatusOcorrencia { get => _idStatusOcorrencia; set => _idStatusOcorrencia = value; }
        public int idTipoOcorrencia { get => _idTipoOcorrencia; set => _idTipoOcorrencia = value; }

        #endregion
    }

    #endregion

    #region | Equipamentos

    [Serializable]
    public class cls_Equipamentos
    {
        #region | Construtor 
        public cls_Equipamentos()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_Equipamentos
        (
             int idRegistroEquipamento
            , int idColaborador
            , int idEquipamento
            , string sDcsTipoEquipamento
            , string sModeloEquipamento
            , string sCodTTEquipamento
            , string dtRecebimentoEquipamento
            , string dtDevolucaoEquipamento
            , string sObservacaoEquipamento
            , string sVeiculoEquipamentos
            , int idVeiculo

            , int idArquivo
            , int idLinha
            , string sFuncao


        )
        {
            _idRegistroEquipamento = idRegistroEquipamento;
            _idColaborador = idColaborador;
            _idEquipamento = idEquipamento;
            _sDcsTipoEquipamento = sDcsTipoEquipamento;
            _sModeloEquipamento = sModeloEquipamento;
            _sCodTTEquipamento = sCodTTEquipamento;
            _dtRecebimentoEquipamento = dtRecebimentoEquipamento;
            _dtDevolucaoEquipamento = dtDevolucaoEquipamento;
            _sObservacaoEquipamento = sObservacaoEquipamento;
            _sVeiculoEquipamentos = sVeiculoEquipamentos;
            _idVeiculo = idVeiculo;

            _idArquivo = idArquivo;
            _idLinha = idLinha;
            _sFuncao = sFuncao;
        }

        #endregion

        #region | Membros Privados 

        private int _idRegistroEquipamento;
        private int _idColaborador;
        private int _idEquipamento;
        private string _sDcsTipoEquipamento;
        private string _sModeloEquipamento;
        private string _sCodTTEquipamento;
        private string _dtRecebimentoEquipamento;
        private string _dtDevolucaoEquipamento;
        private string _sObservacaoEquipamento;
        private string _sVeiculoEquipamentos;
        private int _idVeiculo;

        private int _idArquivo;
        private string _sNomeArquivo;
        private string _sObservacaoArquivo;
        private Byte[] _objArquivo;
        private string _sFuncao;
        private int _idLinha;


        #endregion

        #region | Propriedades


        public int idColaborador
        {
            get { return _idColaborador; }
            set { _idColaborador = value; }
        }
        public int idEquipamento
        {
            get { return _idEquipamento; }
            set { _idEquipamento = value; }
        }

        public string sDcsTipoEquipamento
        {
            get { return _sDcsTipoEquipamento; }
            set { _sDcsTipoEquipamento = value; }
        }

        public string sModeloEquipamento
        {
            get { return _sModeloEquipamento; }
            set { _sModeloEquipamento = value; }
        }

        public string sCodTTEquipamento
        {
            get { return _sCodTTEquipamento; }
            set { _sCodTTEquipamento = value; }
        }


        public string dtRecebimentoEquipamento
        {
            get { return _dtRecebimentoEquipamento; }
            set { _dtRecebimentoEquipamento = value; }
        }


        public string dtDevolucaoEquipamento
        {
            get { return _dtDevolucaoEquipamento; }
            set { _dtDevolucaoEquipamento = value; }
        }

        public string sObservacaoEquipamento
        {
            get { return _sObservacaoEquipamento; }
            set { _sObservacaoEquipamento = value; }
        }

        public string sVeiculoEquipamentos { get => _sVeiculoEquipamentos; set => _sVeiculoEquipamentos = value; }
        public int idArquivo { get => _idArquivo; set => _idArquivo = value; }
        public string sNomeArquivo { get => _sNomeArquivo; set => _sNomeArquivo = value; }
        public string sObservacaoArquivo { get => _sObservacaoArquivo; set => _sObservacaoArquivo = value; }
        public byte[] objArquivo { get => _objArquivo; set => _objArquivo = value; }
        public string sFuncao { get => _sFuncao; set => _sFuncao = value; }
        public int idLinha { get => _idLinha; set => _idLinha = value; }
        public int idRegistroEquipamento { get => _idRegistroEquipamento; set => _idRegistroEquipamento = value; }
        public int idVeiculo { get => _idVeiculo; set => _idVeiculo = value; }


        #endregion
    }

    #endregion

    #region | Dependentes

    [Serializable]
    public class cls_Dependentes
    {
        #region | Construtor 
        public cls_Dependentes()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public cls_Dependentes
        (
              int idRegistroDependentes
            , int idColaborador
            , int idDependente
            , string sNomeDependente
            , string sCPFDependente
            , string dtNascDependente
            , string sPensaoDependente
            , string sPlanoSaudeDependente
            , string sDscGrauParentesco
            , decimal nValorPensaoDependente
            , decimal nValorPlanoDependente
            , string sObservacaoDependente

            , int idArquivo
            , int idLinha
            , string sFuncao

            , int idPensaoDependente
            , int idPlanoSaudeDependente
            , int idGrauParentesco

            , string sBancoDependente
            , string sContaDependente
            , string sAgencia
            , int idTipoContaDepentente
            , string sDscTipoContaDepentente

        )
        {
            _idRegistroDependentes = idRegistroDependentes;
            _idColaborador = idColaborador;
            _idDependente = idDependente;
            _sNomeDependente = sNomeDependente;
            _sCPFDependente = sCPFDependente;
            _dtNascDependente = dtNascDependente;
            _sPensaoDependente = sPensaoDependente;
            _sPlanoSaudeDependente = sPlanoSaudeDependente;
            _sDscGrauParentesco = sDscGrauParentesco;
            _nValorPensaoDependente = nValorPensaoDependente;
            _nValorPlanoDependente = nValorPlanoDependente;
            _sObservacaoDependente = sObservacaoDependente;

            _idArquivo = idArquivo;
            _idLinha = idLinha;
            _sFuncao = sFuncao;

            _idPensaoDependente = idPensaoDependente;
            _idPlanoSaudeDependente = idPlanoSaudeDependente;
            _idGrauParentesco = idGrauParentesco;

            _sBancoDependente = sBancoDependente;
            _sContaDependente = sContaDependente;
            _sAgencia = sAgencia;
            _idTipoContaDepentente = idTipoContaDepentente;
            _sDscTipoContaDepentente = sDscTipoContaDepentente;

        }

        #endregion

        #region | Membros Privados 

        private int _idRegistroDependentes;
        private int _idColaborador;
        private int _idDependente;
        private string _sNomeDependente;
        private string _sCPFDependente;
        private string _dtNascDependente;
        private string _sPensaoDependente;
        private string _sPlanoSaudeDependente;
        private string _sDscGrauParentesco;
        private decimal _nValorPensaoDependente;
        private decimal _nValorPlanoDependente;
        private string _sObservacaoDependente;

        private int _idArquivo;
        private string _sNomeArquivo;
        private string _sObservacaoArquivo;
        private Byte[] _objArquivo;
        private string _sFuncao;
        private int _idLinha;

        private int _idPensaoDependente;
        private int _idPlanoSaudeDependente;
        private int _idGrauParentesco;

        private string _sBancoDependente;
        private string _sContaDependente;
        private string _sAgencia;
        private int _idTipoContaDepentente;
        private string _sDscTipoContaDepentente;

        #endregion

        #region | Propriedades


        public int idColaborador
        {
            get { return _idColaborador; }
            set { _idColaborador = value; }
        }
        public int idDependente
        {
            get { return _idDependente; }
            set { _idDependente = value; }
        }

        public string sNomeDependente
        {
            get { return _sNomeDependente; }
            set { _sNomeDependente = value; }
        }

        public string sCPFDependente
        {
            get { return _sCPFDependente; }
            set { _sCPFDependente = value; }
        }


        public string dtNascDependente
        {
            get { return _dtNascDependente; }
            set { _dtNascDependente = value; }
        }


        public string sPensaoDependente
        {
            get { return _sPensaoDependente; }
            set { _sPensaoDependente = value; }
        }

        public string sPlanoSaudeDependente
        {
            get { return _sPlanoSaudeDependente; }
            set { _sPlanoSaudeDependente = value; }
        }


        public string sDscGrauParentesco
        {
            get { return _sDscGrauParentesco; }
            set { _sDscGrauParentesco = value; }
        }

        public decimal nValorPensaoDependente
        {
            get { return _nValorPensaoDependente; }
            set { _nValorPensaoDependente = value; }
        }

        public decimal nValorPlanoDependente
        {
            get { return _nValorPlanoDependente; }
            set { _nValorPlanoDependente = value; }
        }

        public string sObservacaoDependente
        {
            get { return _sObservacaoDependente; }
            set { _sObservacaoDependente = value; }
        }

        public int idArquivo { get => _idArquivo; set => _idArquivo = value; }
        public string sNomeArquivo { get => _sNomeArquivo; set => _sNomeArquivo = value; }
        public string sObservacaoArquivo { get => _sObservacaoArquivo; set => _sObservacaoArquivo = value; }
        public byte[] objArquivo { get => _objArquivo; set => _objArquivo = value; }
        public string sFuncao { get => _sFuncao; set => _sFuncao = value; }
        public int idLinha { get => _idLinha; set => _idLinha = value; }
        public int idRegistroDependentes { get => _idRegistroDependentes; set => _idRegistroDependentes = value; }
        public int idPensaoDependente { get => _idPensaoDependente; set => _idPensaoDependente = value; }
        public int idPlanoSaudeDependente { get => _idPlanoSaudeDependente; set => _idPlanoSaudeDependente = value; }
        public int idGrauParentesco { get => _idGrauParentesco; set => _idGrauParentesco = value; }
        public string sBancoDependente { get => _sBancoDependente; set => _sBancoDependente = value; }
        public string sContaDependente { get => _sContaDependente; set => _sContaDependente = value; }
        public string sAgencia { get => _sAgencia; set => _sAgencia = value; }
        public int idTipoContaDepentente { get => _idTipoContaDepentente; set => _idTipoContaDepentente = value; }
        public string sDscTipoContaDepentente { get => _sDscTipoContaDepentente; set => _sDscTipoContaDepentente = value; }


        #endregion
    }

    #endregion

    #region | Elogios

    [Serializable]
    public class cls_Elogios
    {

        #region | Construtor 
        public cls_Elogios()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_Elogios
        (
              int idColaborador
            , int idElogio
            , string sDscElogio
            , string dtElogio
            , string sObservacaoElogio

        )
        {

            _idColaborador = idColaborador;
            _idElogio = idElogio;
            _sDscElogio = sDscElogio;
            _dtElogio = dtElogio;
            _sObservacaoElogio = sObservacaoElogio;
        }

        #endregion

        #region | Membros Privados 

        private int _idColaborador;
        private int _idElogio;
        private string _sDscElogio;
        private string _dtElogio;
        private string _sObservacaoElogio;


        #endregion

        #region | Propriedades


        public int idColaborador
        {
            get { return _idColaborador; }
            set { _idColaborador = value; }
        }
        public int idElogio
        {
            get { return _idElogio; }
            set { _idElogio = value; }
        }

        public string sDscElogio
        {
            get { return _sDscElogio; }
            set { _sDscElogio = value; }
        }

        public string dtElogio
        {
            get { return _dtElogio; }
            set { _dtElogio = value; }
        }

        public string sObservacaoElogio
        {
            get { return _sObservacaoElogio; }
            set { _sObservacaoElogio = value; }
        }


        #endregion
    }

    #endregion

    #region | Conversas

    [Serializable]
    public class cls_Conversas
    {
        #region | Construtor 
        public cls_Conversas()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_Conversas
        (

             int idRegistroConvesa
            , int idConversa
            , int idColaborador
            , string sDscConversa
            , string dtEventoConversa
            , string dtResolucaoConversa
            , string dtRepostaConversa
            , string sObservacaoConversa
            , string dtProximaConversa

            , int idTopico
            , string sDscTopico
            , int idTopicoPai
            , string sDscTopicoPai

            , int idArquivo
            , int idLinha
            , string sFuncao

            , string sDscAcao
            , string sDscMeio
            , string sDscStatusConversa
            , string sDscTipoEvento

            , int idAcao
            , int idMeio
            , int idStatusConversa
            , int idTipoEvento

            , string sDscEventoConversa

        )
        {
            _idRegistroConvesa = idRegistroConvesa;
            _idConversa = idConversa;
            _idColaborador = idColaborador;
            _sDscConversa = sDscConversa;
            _dtEventoConversa = dtEventoConversa;
            _dtResolucaoConversa = dtResolucaoConversa;
            _dtRepostaConversa = dtRepostaConversa;
            _sObservacaoConversa = sObservacaoConversa;
            _dtProximaConversa = dtProximaConversa;

            _idArquivo = idArquivo;
            _idLinha = idLinha;
            _sFuncao = sFuncao;

            _idTopico = idTopico;
            _sDscTopico = sDscTopico;
            _idTopicoPai = idTopicoPai;
            _sDscTopicoPai = sDscTopicoPai;

            _sDscAcao = sDscAcao;
            _sDscMeio = sDscMeio;
            _sDscStatusConversa = sDscStatusConversa;
            _sDscTipoEvento = sDscTipoEvento;

            _idAcao = idAcao;
            _idMeio = idMeio;
            _idStatusConversa = idStatusConversa;
            _idTipoEvento = idTipoEvento;

            _sDscEventoConversa = sDscEventoConversa;
        }

        #endregion

        #region | Membros Privados 

        private int _idRegistroConvesa;
        private int _idConversa;
        private int _idColaborador;
        private string _sDscConversa;
        private string _dtEventoConversa;
        private string _dtResolucaoConversa;
        private string _dtRepostaConversa;
        private string _sObservacaoConversa;
        private string _dtProximaConversa;

        private int _idArquivo;
        private string _sNomeArquivo;
        private string _sObservacaoArquivo;
        private Byte[] _objArquivo;
        private string _sFuncao;
        private int _idLinha;

        private int _idTopico;
        private string _sDscTopico;
        private int _idTopicoPai;
        private string _sDscTopicoPai;


        private string _sDscAcao;
        private string _sDscMeio;
        private string _sDscStatusConversa;
        private string _sDscTipoEvento;

        private int _idAcao;
        private int _idMeio;
        private int _idStatusConversa;
        private int _idTipoEvento;

        private string _sDscEventoConversa;

        public int idRegistroConvesa { get => _idRegistroConvesa; set => _idRegistroConvesa = value; }
        public int idConversa { get => _idConversa; set => _idConversa = value; }
        public int idColaborador { get => _idColaborador; set => _idColaborador = value; }
        public string sDscConversa { get => _sDscConversa; set => _sDscConversa = value; }
        public string dtEventoConversa { get => _dtEventoConversa; set => _dtEventoConversa = value; }
        public string dtResolucaoConversa { get => _dtResolucaoConversa; set => _dtResolucaoConversa = value; }
        public string dtRepostaConversa { get => _dtRepostaConversa; set => _dtRepostaConversa = value; }
        public string sObservacaoConversa { get => _sObservacaoConversa; set => _sObservacaoConversa = value; }
        public string dtProximaConversa { get => _dtProximaConversa; set => _dtProximaConversa = value; }
        public int idArquivo { get => _idArquivo; set => _idArquivo = value; }
        public string sNomeArquivo { get => _sNomeArquivo; set => _sNomeArquivo = value; }
        public string sObservacaoArquivo { get => _sObservacaoArquivo; set => _sObservacaoArquivo = value; }
        public byte[] objArquivo { get => _objArquivo; set => _objArquivo = value; }
        public string sFuncao { get => _sFuncao; set => _sFuncao = value; }
        public int idLinha { get => _idLinha; set => _idLinha = value; }
        public string sDscAcao { get => _sDscAcao; set => _sDscAcao = value; }
        public string sDscMeio { get => _sDscMeio; set => _sDscMeio = value; }
        public string sDscStatusConversa { get => _sDscStatusConversa; set => _sDscStatusConversa = value; }
        public string sDscTipoEvento { get => _sDscTipoEvento; set => _sDscTipoEvento = value; }
        public string sDscEventoConversa { get => _sDscEventoConversa; set => _sDscEventoConversa = value; }
        public int idAcao { get => _idAcao; set => _idAcao = value; }
        public int idMeio { get => _idMeio; set => _idMeio = value; }
        public int idStatusConversa { get => _idStatusConversa; set => _idStatusConversa = value; }
        public int idTipoEvento { get => _idTipoEvento; set => _idTipoEvento = value; }
        public int idTopico { get => _idTopico; set => _idTopico = value; }
        public string sDscTopico { get => _sDscTopico; set => _sDscTopico = value; }
        public int idTopicoPai { get => _idTopicoPai; set => _idTopicoPai = value; }
        public string sDscTopicoPai { get => _sDscTopicoPai; set => _sDscTopicoPai = value; }


        #endregion

        #region | Propriedades



        #endregion
    }

    #endregion

    #region | VT

    [Serializable]
    public class cls_VT
    {
        #region | Construtor 
        public cls_VT()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_VT
        (

              int idRegistroVT
            , int idColaborador
            , int idVT
            , string sTipoVt
            , string sNumeroVT
            , decimal nValorVT
            , string sVTDefinitivo
            , string sOpcaoVT
            , string sDscMotivo
            , string dtVT

            , int idVTDefinitivo
            , int idOpcaoVT
            , int idTipoVT

            , int idArquivo
            , int idLinha
            , string sFuncao
            , string sTipoVtOutros


        )
        {
            _idRegistroVT = idRegistroVT;
            _idColaborador = idColaborador;
            _idVT = idVT;
            _sTipoVt = sTipoVt;
            _sNumeroVT = sNumeroVT;
            _nValorVT = nValorVT;
            _sVTDefinitivo = sVTDefinitivo;
            _sOpcaoVT = sOpcaoVT;
            _sDscMotivo = sDscMotivo;
            _sTipoVtOutros = sTipoVtOutros;


            _idArquivo = idArquivo;
            _idLinha = idLinha;
            _sFuncao = sFuncao;

            _dtVT = dtVT;
        }

        #endregion

        #region | Membros Privados 


        private int _idColaborador;
        private int _idVT;
        private string _sTipoVt;
        private string _sNumeroVT;
        private decimal _nValorVT;
        private string _sVTDefinitivo;
        private string _sOpcaoVT;
        private string _sDscMotivo;
        private int _idArquivo;
        private string _sNomeArquivo;
        private string _sObservacaoArquivo;
        private Byte[] _objArquivo;
        private int _idRegistroVT;
        private int _idLinha;
        private string _dtVT;
        private string _sFuncao;

        private int _idVTDefinitivo;
        private int _idOpcaoVT;
        private int _idTipoVT;
        private string _sTipoVtOutros;

        #endregion

        #region | Propriedades


        public int idColaborador
        {
            get { return _idColaborador; }
            set { _idColaborador = value; }
        }
        public int idVT
        {
            get { return _idVT; }
            set { _idVT = value; }
        }

        public string sTipoVt
        {
            get { return _sTipoVt; }
            set { _sTipoVt = value; }
        }

        public string sNumeroVT
        {
            get { return _sNumeroVT; }
            set { _sNumeroVT = value; }
        }

        public decimal nValorVT
        {
            get { return _nValorVT; }
            set { _nValorVT = value; }
        }


        public string sVTDefinitivo
        {
            get { return _sVTDefinitivo; }
            set { _sVTDefinitivo = value; }
        }

        public string sOpcaoVT { get => _sOpcaoVT; set => _sOpcaoVT = value; }
        public int idArquivo { get => _idArquivo; set => _idArquivo = value; }
        public string sFuncao { get => _sFuncao; set => _sFuncao = value; }
        public string sNomeArquivo { get => _sNomeArquivo; set => _sNomeArquivo = value; }
        public string sObservacaoArquivo { get => _sObservacaoArquivo; set => _sObservacaoArquivo = value; }
        public byte[] objArquivo { get => _objArquivo; set => _objArquivo = value; }
        public int idRegistroVT { get => _idRegistroVT; set => _idRegistroVT = value; }
        public int idLinha { get => _idLinha; set => _idLinha = value; }
        public string sDscMotivo { get => _sDscMotivo; set => _sDscMotivo = value; }
        public string dtVT { get => _dtVT; set => _dtVT = value; }
        public int idVTDefinitivo { get => _idVTDefinitivo; set => _idVTDefinitivo = value; }
        public int idOpcaoVT { get => _idOpcaoVT; set => _idOpcaoVT = value; }
        public int idTipoVT { get => _idTipoVT; set => _idTipoVT = value; }
        public string sTipoVtOutros { get => _sTipoVtOutros; set => _sTipoVtOutros = value; }



        #endregion
    }

    #endregion

    #region | Roupas

    [Serializable]
    public class cls_Roupas
    {
        public cls_Roupas()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public cls_Roupas
        (
              int idColaborador
            , int idRegistroRoupa
            , int idTipoRoupa
            , string sDscTipoRoupa
            , int idTamanho
            , string sDscTamanho
            , string sCorRoupa
            , int nQuantidade
            , string dtEntregaRoupa
            , string sObservacao

            , int idArquivo
            , int idLinha
            , string sFuncao

        )
        {

        }
        private int idColaborador;
        private int idRegistroRoupa;
        private int idTipoRoupa;
        private string sDscTipoRoupa;
        private int idTamanho;
        private string sDscTamanho;
        private string sCorRoupa;
        private int nQuantidade;
        private string dtEntregaRoupa;
        private string sObservacao;

        private int idArquivo;
        private string sNomeArquivo;
        private string sObservacaoArquivo;
        private Byte[] objArquivo;
        private string sFuncao;
        private int idLinha;

        public int IdColaborador { get => idColaborador; set => idColaborador = value; }
        public int IdRegistroRoupa { get => idRegistroRoupa; set => idRegistroRoupa = value; }
        public int IdTipoRoupa { get => idTipoRoupa; set => idTipoRoupa = value; }
        public string SDscTipoRoupa { get => sDscTipoRoupa; set => sDscTipoRoupa = value; }
        public int IdTamanho { get => idTamanho; set => idTamanho = value; }
        public string SDscTamanho { get => sDscTamanho; set => sDscTamanho = value; }
        public string SCorRoupa { get => sCorRoupa; set => sCorRoupa = value; }
        public int NQuantidade { get => nQuantidade; set => nQuantidade = value; }
        public string DtEntregaRoupa { get => dtEntregaRoupa; set => dtEntregaRoupa = value; }
        public string SObservacao { get => sObservacao; set => sObservacao = value; }
        public int IdArquivo { get => idArquivo; set => idArquivo = value; }
        public string SNomeArquivo { get => sNomeArquivo; set => sNomeArquivo = value; }
        public string SObservacaoArquivo { get => sObservacaoArquivo; set => sObservacaoArquivo = value; }
        public byte[] ObjArquivo { get => objArquivo; set => objArquivo = value; }
        public string SFuncao { get => sFuncao; set => sFuncao = value; }
        public int IdLinha { get => idLinha; set => idLinha = value; }
    }

    #endregion

    #region | Avaliacao

    [Serializable]
    public class cls_Avaliacao
    {

        #region | Construtor 
        public cls_Avaliacao()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public cls_Avaliacao
        (
             int idLinha
            , string sFuncao
            , int idColaborador
            , int idRegistroAvaliacao
            , int idArquivo
            , string sDscAvaliacao
            , string dtAvaliacao
            , int idTipoAvaliacao
            , string sObservacaoAvaliacao


        )
        {
            _idLinha = idLinha;
            _sFuncao = sFuncao;
            _idColaborador = idColaborador;
            _idRegistroAvaliacao = idRegistroAvaliacao;
            _idArquivo = idArquivo;

            _sDscAvaliacao = sDscAvaliacao;
            _dtAvaliacao = dtAvaliacao;
            _idTipoAvaliacao = idTipoAvaliacao;

            _sObservacaoAvaliacao = sObservacaoAvaliacao;
        }

        #endregion

        #region | Membros Privados 

        private int _idLinha;
        private string _sFuncao;
        private int _idColaborador;
        private int _idRegistroAvaliacao;

        private int _idArquivo;
        private string _sNomeArquivo;
        private string _sObservacaoArquivo;
        private Byte[] _objArquivo;


        private string _sDscAvaliacao;
        private string _dtAvaliacao;
        private int _idTipoAvaliacao;
        private string _sObservacaoAvaliacao;



        public int idLinha { get => _idLinha; set => _idLinha = value; }
        public string sFuncao { get => _sFuncao; set => _sFuncao = value; }
        public int idColaborador { get => _idColaborador; set => _idColaborador = value; }
        public int idRegistroAvaliacao { get => _idRegistroAvaliacao; set => _idRegistroAvaliacao = value; }
        public int idArquivo { get => _idArquivo; set => _idArquivo = value; }
        public string sNomeArquivo { get => _sNomeArquivo; set => _sNomeArquivo = value; }
        public string sObservacaoArquivo { get => _sObservacaoArquivo; set => _sObservacaoArquivo = value; }
        public byte[] objArquivo { get => _objArquivo; set => _objArquivo = value; }
        public string sDscAvaliacao { get => _sDscAvaliacao; set => _sDscAvaliacao = value; }
        public string dtAvaliacao { get => _dtAvaliacao; set => _dtAvaliacao = value; }
        public int idTipoAvaliacao { get => _idTipoAvaliacao; set => _idTipoAvaliacao = value; }
        public string sObservacaoAvaliacao { get => _sObservacaoAvaliacao; set => _sObservacaoAvaliacao = value; }




        #endregion

        #region | Propriedades




        #endregion
    }

    #endregion

    #region | Arquivo morto

    [Serializable]
    public class cls_ArquivoMorto
    {
        #region | Construtor 
        public cls_ArquivoMorto()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public cls_ArquivoMorto
        (
             int idLinha
            , string sDescricao
            , int idColaborador            
            , int idArquivo
            , string sNomeArquivo
            , string dtExclusao
            , int idUsuarioAtualizacao
            , string sDscUsuario

        )
        {
            _idLinha = idLinha;
            _sDescricao = sDescricao;
            _idColaborador = idColaborador;
            _idArquivo = idArquivo;

            _sNomeArquivo = sNomeArquivo;
            _dtExclusao = dtExclusao;
            _idUsuarioAtualizacao = idUsuarioAtualizacao;
            _sDscUsuario = sDscUsuario;
        }

        #endregion

        #region | Membros Privados 

        private int _idLinha;
        private string _sDescricao;
        private int _idColaborador;
        private int _idArquivo;
        private string _sNomeArquivo;
        private Byte[] _objArquivo;

        private string _dtExclusao;
        private int _idUsuarioAtualizacao;
        private string _sDscUsuario;



        public int idLinha { get => _idLinha; set => _idLinha = value; }
        public string sDescricao { get => _sDescricao; set => _sDescricao = value; }
        public int idColaborador { get => _idColaborador; set => _idColaborador = value; }
        public int idArquivo { get => _idArquivo; set => _idArquivo = value; }
        public string sNomeArquivo { get => _sNomeArquivo; set => _sNomeArquivo = value; }
        public byte[] objArquivo { get => _objArquivo; set => _objArquivo = value; }
        public string dtExclusao { get => _dtExclusao; set => _dtExclusao = value; }
        public int idUsuarioAtualizacao { get => _idUsuarioAtualizacao; set => _idUsuarioAtualizacao = value; }
        public string sDscUsuario { get => _sDscUsuario; set => _sDscUsuario = value; }

        #endregion

        #region | Propriedades




        #endregion
    }

    #endregion

    #region | EPI_x_Funcao

    [Serializable]
    public class cls_EPI_x_Funcao
    {
        #region | Construtor

        public cls_EPI_x_Funcao() { }

        public cls_EPI_x_Funcao
        (
             int idRegistroEPI
            , int idFuncao
            , int idItem
            , int idTipoProduto
            , string sCodigoEPI
            , string sDscEPI
            , int nCA

            , int nQuantidateEPI
            , int nQuantidadeTempo

            , string dtAtualizacao
            , int idUsuarioAtualizacao
            , string sTipoPeriodo
            , string sDscTipoPeriodo

            , int idLinha
            , string sFuncao
        )
        {
            _idRegistroEPI = idRegistroEPI;
            _idFuncao = idFuncao;
            _idItem = idItem;
            _idTipoProduto = idTipoProduto;
            _sCodigoEPI = sCodigoEPI;
            _sDscEPI = sDscEPI;
            _nCA = nCA;
            _nQuantidateEPI = nQuantidateEPI;
            _nQuantidadeTempo = nQuantidadeTempo;
            _dtAtualizacao = dtAtualizacao;
            _idUsuarioAtualizacao = idUsuarioAtualizacao;
            _sTipoPeriodo = sTipoPeriodo;
            _sDscTipoPeriodo = sDscTipoPeriodo;
            _idLinha = idLinha;
            _sFuncao = sFuncao;
        }

        #endregion

        #region | Membros Privados 

        private int _idRegistroEPI;
        private int _idFuncao;
        private int _idItem;
        private int _idTipoProduto;
        private int _nOrdem;
        private int _idEstado;
        private string _sCodigoEPI;
        private string _sDscEPI;
        private int _nCA;
        private string _sCA;
        private int _nQuantidateEPI;
        private int _nQuantidadeTempo;
        private string _sTipoPeriodo;
        private string _sPeriodicidade;
        private string _dtAtualizacao;
        private int _idUsuarioAtualizacao;
        private string _sDscTipoPeriodo;
        private DateTime _dtVencimento;

        private int _idLinha;
        private string _sFuncao;

        private object _vbArquivo;
        private string _sTipoArquivo;

        #endregion

        #region | Propriedades

        public int idRegistroEPI { get => _idRegistroEPI; set => _idRegistroEPI = value; }
        public int idFuncao { get => _idFuncao; set => _idFuncao = value; }
        public int idItem { get => _idItem; set => _idItem = value; }
        public int idTipoProduto { get => _idTipoProduto; set => _idTipoProduto = value; }
        public int nOrdem { get => _nOrdem; set => _nOrdem = value; }
        public int idEstado { get => _idEstado; set => _idEstado = value; }
        public string sCodigoEPI { get => _sCodigoEPI; set => _sCodigoEPI = value; }
        public string sDscEPI { get => _sDscEPI; set => _sDscEPI = value; }
        public int nCA { get => _nCA; set => _nCA = value; }
        public string sCA { get => _sCA; set => _sCA = value; }
        public int nQuantidateEPI { get => _nQuantidateEPI; set => _nQuantidateEPI = value; }
        public string dtAtualizacao { get => _dtAtualizacao; set => _dtAtualizacao = value; }
        public int idUsuarioAtualizacao { get => _idUsuarioAtualizacao; set => _idUsuarioAtualizacao = value; }
        public int idLinha { get => _idLinha; set => _idLinha = value; }
        public string sFuncao { get => _sFuncao; set => _sFuncao = value; }
        public string sTipoPeriodo { get => _sTipoPeriodo; set => _sTipoPeriodo = value; }
        public string sPeriodicidade { get => _sPeriodicidade; set => _sPeriodicidade = value; }
        public string sDscTipoPeriodo { get => _sDscTipoPeriodo; set => _sDscTipoPeriodo = value; }
        public int nQuantidadeTempo { get => _nQuantidadeTempo; set => _nQuantidadeTempo = value; }
        public DateTime dtVencimento { get => _dtVencimento; set => _dtVencimento = value; }
        public object vbArquivo { get => _vbArquivo; set => _vbArquivo = value; }
        public string sTipoArquivo { get => _sTipoArquivo; set => _sTipoArquivo = value; }


        #endregion
    }

    #endregion

    #region | Setor

    [Serializable]
    public class cls_Setor
    {
        public cls_Setor()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public cls_Setor
        (
            int idRegistro
            ,  int idSetor
            , int idEmpresa
            , int nOrdem
            , int idSetorPai
            , int idUsuarioAtualizacao
            , string sDscSetor
            , string sFuncao
     
        )
        {

        }
        private int _idRegistro;
        private int _idSetor;
        private int _idEmpresa;
        private int _nOrdem;
        private int _idSetorPai;
        private int _idUsuarioAtualizacao;
        private string _sDscSetor;
        private string _sFuncao;
         
        public int idRegistro { get => _idRegistro; set => _idRegistro = _idSetor = value; }
        public int idSetor { get => _idSetor; set => _idSetor = value; }
        public int idEmpresa { get => _idEmpresa; set => _idEmpresa = value; }
        public int nOrdem { get => _nOrdem; set => _nOrdem = value; }
        public int idSetorPai { get => _idSetorPai; set => _idSetorPai = value; }
        public int idUsuarioAtualizacao { get => _idUsuarioAtualizacao; set => _idUsuarioAtualizacao = value; }
        public string sDscSetor { get => _sDscSetor; set => _sDscSetor = value; }
        public string sFuncao { get => _sFuncao; set => _sFuncao = value; }
    }

    #endregion

    #region | Plano Saude


    [Serializable]
    public class cls_PlanoSaude_Itens
    {
        #region | Construtor 
        
        public cls_PlanoSaude_Itens
        (
             int idLinha
            , string sFuncao
            , int idColaborador
            , string sDscColaborador
            , int idDependente
            , string sNomeDependente
            , int idPlanoSaude
            , string sDscPlanoSaude
            , int idArquivo
            , string dtInclusao
            , string dtFimCarencia
            , decimal nValorPlano
            , string sDscBeneficiario
            , string sTipo
            , string sTipoBeneficiario
        )
        {
            _idLinha = idLinha;
            _sFuncao = sFuncao;
            _idColaborador = idColaborador;
            _sDscColaborador = sDscColaborador;
            _idDependente = idDependente;
            _sNomeDependente = sNomeDependente;
            _idPlanoSaude = idPlanoSaude;
            _sDscPlanoSaude = sDscPlanoSaude;
            _idArquivo = idArquivo;
            _dtInclusao = dtInclusao;
            _dtFimCarencia = dtFimCarencia;
            _nValorPlano = nValorPlano;
            _sDscBeneficiario = sDscBeneficiario;
            _sTipo = sTipo;
            _sTipoBeneficiario = sTipoBeneficiario;
        }

        public cls_PlanoSaude_Itens()
        {
        }

        #endregion

        #region | Membros Privados 

        private int _idLinha;
        private string _sFuncao;
        private int _idColaborador;
        private int _idPlanoSaude;
        private int _idArquivo;
        private int _idDependente;
        private string _sNomeArquivo;
        private string _sObservacaoArquivo;
        private Byte[] _objArquivo;
        private string _dtInclusao;
        private string _dtFimCarencia;
        private decimal _nValorPlano;     
        private string _sDscColaborador;     
        private string _sNomeDependente;     
        private string _sDscPlanoSaude;
        private string _sDscBeneficiario;
        private string _sTipo;
        private string _sTipoBeneficiario;

        #endregion

        #region | Propriedades

        public int idLinha
        {
            get { return _idLinha; }
            set { _idLinha = value; }
        }

        public string sFuncao
        {
            get { return _sFuncao; }
            set { _sFuncao = value; }
        }
        public int idColaborador
        {
            get { return _idColaborador; }
            set { _idColaborador = value; }
        }

        public int idPlanoSaude
        {
            get { return _idPlanoSaude; }
            set { _idPlanoSaude = value; }
        }
        
        public int idDependente
        {
            get { return _idDependente; }
            set { _idDependente = value; }
        }

        public int idArquivo
        {
            get { return _idArquivo; }
            set { _idArquivo = value; }
        }
        public string sNomeArquivo
        {
            get { return _sNomeArquivo; }
            set { _sNomeArquivo = value; }
        }

        public string sObservacaoArquivo
        {
            get { return _sObservacaoArquivo; }
            set { _sObservacaoArquivo = value; }
        }

        public Byte[] objArquivo
        {
            get { return _objArquivo; }
            set { _objArquivo = value; }
        }

        public string dtInclusao
        {
            get { return _dtInclusao; }
            set { _dtInclusao = value; }
        }

        public string dtFimCarencia
        {
            get { return _dtFimCarencia; }
            set { _dtFimCarencia = value; }
        }

        public decimal nValorPlano
        {
            get { return _nValorPlano; }
            set { _nValorPlano = value; }
        }
        
        public string sDscColaborador
        {
            get { return _sDscColaborador; }
            set { _sDscColaborador = value; }
        }
        
        public string sNomeDependente
        {
            get { return _sNomeDependente; }
            set { _sNomeDependente = value; }
        }
        
        public string sDscPlanoSaude
        {
            get { return _sDscPlanoSaude; }
            set { _sDscPlanoSaude = value; }
        }

        public string sDscBeneficiario
        {
            get { return _sDscBeneficiario; }
            set { _sDscBeneficiario = value; }
        }
        
        public string sTipo
        {
            get { return _sTipo; }
            set { _sTipo = value; }
        }
        
        public string sTipoBeneficiario
        {
            get { return _sTipoBeneficiario; }
            set { _sTipoBeneficiario = value; }
        }


        #endregion
    }
   
    #endregion

}
