using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TT.FrameWork;

namespace TT_Flow.FrameWork
{
    #region | Contatos
    [Serializable]
    public class cls_Contatos
    {

        #region | Construtor 
        public cls_Contatos()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_Contatos
        (
                int idConta
            , int idContatos
            , string sDscContato
            , string sTelefoneContato
            , string sEmailContato
            , string sContaCorrente


        )
        {

            _idConta = idConta;
            _idContatos = idContatos;
            _sDscContato = sDscContato;
            _sTelefoneContato = sTelefoneContato;
            _sEmailContato = sEmailContato;
            _sContaCorrente = sContaCorrente;

        }

        #endregion

        #region | Membros Privados 

        private int _idConta;
        private int _idContatos;
        private string _sDscContato;
        private string _sTelefoneContato;
        private string _sEmailContato;
        private string _sContaCorrente;

        #endregion

        #region | Propriedades


        public int idConta
        {
            get { return _idConta; }
            set { _idConta = value; }
        }
        public int idContatos
        {
            get { return _idContatos; }
            set { _idContatos = value; }
        }

        public string sDscContato
        {
            get { return _sDscContato; }
            set { _sDscContato = value; }
        }

        public string sTelefoneContato
        {
            get { return _sTelefoneContato; }
            set { _sTelefoneContato = value; }
        }

        public string sEmailContato
        {
            get { return _sEmailContato; }
            set { _sEmailContato = value; }
        }

        public string sContaCorrente { get => _sContaCorrente; set => _sContaCorrente = value; }


        #endregion


    }
    #endregion

    #region | CartãoCredito
    [Serializable]
    public class cls_CartaoCredito
    {

        #region | Construtor 
        public cls_CartaoCredito()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_CartaoCredito
        (
                int idConta
            , int idCartao
            , string nNumCartao
            , string dtVenciCartao
            , string sBandeiraCartao
            , string sTitularCartao
            , decimal nLimiteCartao

            , decimal nProvisaoGastos
            , int idUsuario
            , string sStatus
            , string dtCorteFatura
            , string dtVencimento
            , int idBandeira

            , int idArquivo
            , int idLinha
            , string sFuncao
            , string sDscBandeira
            , string sDscUsuario
            , string sStatus_Completo
            , string sLancamento
        )
        {

            _idConta = idConta;
            _idCartao = idCartao;
            _nNumCartao = nNumCartao;
            _dtVenciCartao = dtVenciCartao;
            _sBandeiraCartao = sBandeiraCartao;
            _sTitularCartao = sTitularCartao;
            _nLimiteCartao = nLimiteCartao;

            _nProvisaoGastos = nProvisaoGastos;
            _idUsuario = idUsuario;
            _sStatus = sStatus;
            _dtCorteFatura = dtCorteFatura;
            _dtVencimento = dtVencimento;
            _idBandeira = idBandeira;

            _idArquivo = idArquivo;
            _idLinha = idLinha;
            _sFuncao = sFuncao;
            _sDscBandeira = sDscBandeira;
            _sDscUsuario = sDscUsuario;
            _sStatus_Completo = sStatus_Completo;
            _sLancamento = sLancamento;
        }

        #endregion

        #region | Membros Privados 

        private int _idConta;
        private int _idCartao;
        private string _nNumCartao;
        private string _dtVenciCartao;
        private string _sBandeiraCartao;
        private string _sTitularCartao;
        private decimal _nLimiteCartao;

        private decimal _nProvisaoGastos;
        private int _idUsuario;
        private string _sStatus;
        private string _dtCorteFatura;
        private string _dtVencimento;
        private int _idBandeira;

        private int _idArquivo;
        private int _idLinha;
        private string _sFuncao;
        private string _sNomeArquivo;
        private string _sObservacaoArquivo;
        private Byte[] _objArquivo;
        private string _sDscBandeira;
        private string _sDscUsuario;
        private string _sStatus_Completo;
        private string _sLancamento;
        #endregion

        #region | Propriedades


        public int idConta { get => _idConta; set => _idConta = value; }
        public int idCartao { get => _idCartao; set => _idCartao = value; }
        public string nNumCartao { get => _nNumCartao; set => _nNumCartao = value; }
        public string dtVenciCartao { get => _dtVenciCartao; set => _dtVenciCartao = value; }
        public string sBandeiraCartao { get => _sBandeiraCartao; set => _sBandeiraCartao = value; }
        public string sTitularCartao { get => _sTitularCartao; set => _sTitularCartao = value; }
        public decimal nLimiteCartao { get => _nLimiteCartao; set => _nLimiteCartao = value; }
        public decimal nProvisaoGastos { get => _nProvisaoGastos; set => _nProvisaoGastos = value; }
        public int idUsuario { get => _idUsuario; set => _idUsuario = value; }
        public string sStatus { get => _sStatus; set => _sStatus = value; }
        public string dtCorteFatura { get => _dtCorteFatura; set => _dtCorteFatura = value; }
        public string dtVencimento { get => _dtVencimento; set => _dtVencimento = value; }
        public int idArquivo { get => _idArquivo; set => _idArquivo = value; }
        public int idLinha { get => _idLinha; set => _idLinha = value; }
        public string sFuncao { get => _sFuncao; set => _sFuncao = value; }
        public string sNomeArquivo { get => _sNomeArquivo; set => _sNomeArquivo = value; }
        public string sObservacaoArquivo { get => _sObservacaoArquivo; set => _sObservacaoArquivo = value; }
        public byte[] objArquivo { get => _objArquivo; set => _objArquivo = value; }
        public int idBandeira { get => _idBandeira; set => _idBandeira = value; }

        public string sDscBandeira { get => _sDscBandeira; set => _sDscBandeira = value; }
        public string sDscUsuario { get => _sDscUsuario; set => _sDscUsuario = value; }
        public string sStatus_Completo { get => _sStatus_Completo; set => _sStatus_Completo = value; }
        public string sLancamento { get => _sLancamento; set => _sLancamento = value; }


        #endregion


    }
    #endregion

    #region | ProdutosFinanceiros
    [Serializable]
    public class cls_ProdutosFinanceiros
    {

        #region | Construtor 
        public cls_ProdutosFinanceiros()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public cls_ProdutosFinanceiros
        (
              int idConta
            , int idProdutosFinanceiros
            , int idTipoConta
            , string sDscTipoConta
            , decimal nLimiteConta
            , string dtInicio
            , string dtVencimento
            , string sGarantias
            , decimal nPorcentagemTaxaConta
            , decimal nValorProdutosFinanceiros

        )
        {

            _idConta = idConta;
            _idProdutosFinanceiros = idProdutosFinanceiros;
            _idTipoConta = idTipoConta;
            _sDscTipoConta = sDscTipoConta;
            _nLimiteConta = nLimiteConta;
            _dtInicio = dtInicio;
            _dtVencimento = dtVencimento;
            _sGarantias = sGarantias;
            _nPorcentagemTaxaConta = nPorcentagemTaxaConta;
            _nValorProdutosFinanceiros = nValorProdutosFinanceiros;

        }

        #endregion

        #region | Membros Privados 

        private int _idConta;
        private int _idProdutosFinanceiros;
        private int _idTipoConta;
        private string _sDscTipoConta;
        private decimal _nLimiteConta;
        private string _dtInicio;
        private string _dtVencimento;
        private string _sGarantias;
        private decimal _nPorcentagemTaxaConta;
        private decimal _nValorProdutosFinanceiros;

        #endregion

        #region | Propriedades


        public int idConta { get => _idConta; set => _idConta = value; }
        public int idProdutosFinanceiros { get => _idProdutosFinanceiros; set => _idProdutosFinanceiros = value; }
        public int idTipoConta { get => _idTipoConta; set => _idTipoConta = value; }
        public string sDscTipoConta { get => _sDscTipoConta; set => _sDscTipoConta = value; }
        public decimal nLimiteConta { get => _nLimiteConta; set => _nLimiteConta = value; }
        public string itInicio { get => _dtInicio; set => _dtInicio = value; }
        public string dtVencimento { get => _dtVencimento; set => _dtVencimento = value; }
        public string sGarantias { get => _sGarantias; set => _sGarantias = value; }
        public decimal nPorcentagemTaxaConta { get => _nPorcentagemTaxaConta; set => _nPorcentagemTaxaConta = value; }
        public decimal nValorProdutosFinanceiros { get => _nValorProdutosFinanceiros; set => _nValorProdutosFinanceiros = value; }
        public string dtInicio { get; set; }


        #endregion


    }
    #endregion

    #region | Moeda
    [Serializable]
    public class cls_Moeda
    {

        #region | Construtor 
        public cls_Moeda()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public cls_Moeda
        (
            int idMoeda
            , int idTipoMoeda
            , string sDscTipoMoeda
            , int idPais
            , string sDscPais
            , double nValorCambio
            , string sDscUsuarioAtualizacao
            , string dtAtualizacao
            , string sFuncao
            , string sSimbolo
            , string UltimonValorCambio
            , string UltimodtAtualizacao

        )
        {

            _idMoeda = idMoeda;
            _idTipoMoeda = idTipoMoeda;
            _sDscTipoMoeda = sDscTipoMoeda;
            _idPais = idPais;
            _sDscPais = sDscPais;
            _nValorCambio = nValorCambio;
            _sDscUusuarioAtualizacao = sDscUsuarioAtualizacao;
            _dtAtualizacao = dtAtualizacao;
            _sFuncao = sFuncao;
            _SSimbolo = sSimbolo;
            _UltimonValorCambio = UltimonValorCambio;
            _UltimodtAtualizacao = UltimodtAtualizacao;



        }

        #endregion

        #region | Membros Privados 

        private int _idMoeda;
        private int _idTipoMoeda;
        private string _sDscTipoMoeda;
        private int _idPais;
        private string _sDscPais;
        private double _nValorCambio;
        private string _sDscUusuarioAtualizacao;
        private string _dtAtualizacao;
        private string _sFuncao;
        private string _SSimbolo;
        private string _UltimonValorCambio;
        private string _UltimodtAtualizacao;

        public int IdMoeda { get => _idMoeda; set => _idMoeda = value; }
        public int IdTipoMoeda { get => _idTipoMoeda; set => _idTipoMoeda = value; }
        public string SDscTipoMoeda { get => _sDscTipoMoeda; set => _sDscTipoMoeda = value; }
        public int IdPais { get => _idPais; set => _idPais = value; }
        public string SDscPais { get => _sDscPais; set => _sDscPais = value; }
        public double NValorCambio { get => _nValorCambio; set => _nValorCambio = value; }
        public string SDscUusuarioAtualizacao { get => _sDscUusuarioAtualizacao; set => _sDscUusuarioAtualizacao = value; }
        public string DtAtualizacao { get => _dtAtualizacao; set => _dtAtualizacao = value; }
        public string SFuncao { get => _sFuncao; set => _sFuncao = value; }
        public string SSimbolo { get => _SSimbolo; set => _SSimbolo = value; }
        public string UltimonValorCambio { get => _UltimonValorCambio; set => _UltimonValorCambio = value; }
        public string UltimodtAtualizacao { get => _UltimodtAtualizacao; set => _UltimodtAtualizacao = value; }


        #endregion

        #region | Propriedades


        #endregion

        #region | Funções

        public static DataSet Consulta()
        {
            string sErro = "";
            string sProcedure = "sp_Manipula_tbl_Flow_Adm_Moedas";

            try
            {
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR");
                DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsPesquisa, out sErro))
                {
                    return dsPesquisa;
                }
                else
                {
                    throw new Exception(sErro);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static void ConverterDatasetClasse(DataSet ds, List<cls_Moeda> BS_MOEDA)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                FrameWork.cls_Moeda objItem = new FrameWork.cls_Moeda();
                objItem.IdMoeda = Convert.ToInt32(row["idMoeda"].ToString());
                objItem.IdTipoMoeda = Convert.ToInt32(row["idTipoMoeda"].ToString());
                objItem.SDscTipoMoeda = row["sDscTipoMoeda"].ToString();
                //objItem.IdPais = Convert.ToInt32(row["idPais"].ToString());
                //objItem.SDscPais = row["sDscPais"].ToString();
                objItem.SSimbolo = row["sSimbolo"].ToString();
                objItem.NValorCambio = Convert.ToDouble(row["NValorCambio"].ToString());
                objItem.SFuncao = "ATUAL";
                BS_MOEDA.Add(objItem);
            }
        }
        public static void ObterDadosTipoMoeda()
        {

        }
        #endregion


        #region | Patrimonio
        [Serializable]
        public class cls_Patrimonio
        {

            #region | Construtor 
            public cls_Patrimonio()
            {
                //
                // TODO: Add constructor logic here
                //
            }
            public cls_Patrimonio
            (
                int idParimonioGrupo
                , string sDscPatrimonio
                , int nVidaUtil
                , decimal nTxAnualDepreciacao
                , char sSituacao
                , string sDscUsuarioAtualizacao
                , string dtAtualizacao
                , string sFuncao
                , string sDscCodContabil_Depreciacao
                , int idContabil
                , string sDscCodContabil
                , int idContabil_Depreciacao
            )
            {

                _idParimonioGrupo = idParimonioGrupo;
                _sDscPatrimonio = sDscPatrimonio;
                _nVidaUtil = nVidaUtil;
                _nTxAnualDepreciacao = nTxAnualDepreciacao;
                _sSituacao = sSituacao;
                _sDscUsuarioAtualizacao = sDscUsuarioAtualizacao;
                _dtAtualizacao = dtAtualizacao;
                _sFuncao = sFuncao;
                _sDscCodContabil_Depreciacao = sDscCodContabil_Depreciacao;
                _idContabil = idContabil;
                _sDscCodContabil = sDscCodContabil;
                _idContabil_Depreciacao = idContabil_Depreciacao;
            }

            #endregion

            #region | Membros Privados 

            private int _idParimonioGrupo;
            private string _sDscPatrimonio;
            private int _nVidaUtil;
            private decimal _nTxAnualDepreciacao;
            private int _idContabil;
            private string _sDscCodContabil;
            private int _idContabil_Depreciacao;
            private string _sDscCodContabil_Depreciacao;
            private char _sSituacao;
            private string _sDscUsuarioAtualizacao;
            private string _dtAtualizacao;
            private string _sFuncao;


            #endregion

            #region | Propriedades
            public int IdParimonioGrupo { get => _idParimonioGrupo; set => _idParimonioGrupo = value; }
            public string SDscPatrimonio { get => _sDscPatrimonio; set => _sDscPatrimonio = value; }
            public int NVidaUtil { get => _nVidaUtil; set => _nVidaUtil = value; }
            public decimal NTxAnualDepreciacao { get => _nTxAnualDepreciacao; set => _nTxAnualDepreciacao = value; }
            public int IdContabil { get => _idContabil; set => _idContabil = value; }//Campos Novos de Id Contabil
            public string SDscCodContabil { get => _sDscCodContabil; set => _sDscCodContabil = value; }////Campos Novos de Id Contabil
            public int IdContabil_Depreciacao { get => _idContabil_Depreciacao; set => _idContabil_Depreciacao = value; }////Campos Novos de Id Contabil
            public string SDscCodContabil_Depreciacao { get => _sDscCodContabil_Depreciacao; set => _sDscCodContabil_Depreciacao = value; }////Campos Novos de Id Contabil
            public char SSituacao { get => _sSituacao; set => _sSituacao = value; }
            public string SDscUusuarioAtualizacao { get => _sDscUsuarioAtualizacao; set => _sDscUsuarioAtualizacao = value; }
            public string DtAtualizacao { get => _dtAtualizacao; set => _dtAtualizacao = value; }
            public string SFuncao { get => _sFuncao; set => _sFuncao = value; }


            #endregion


        }

        #endregion


    }
    #endregion

    #region | Patrimonio_Itens
    public class cls_Patrimonio_Itens
    {

        #region | Construtor 
        public cls_Patrimonio_Itens()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public cls_Patrimonio_Itens
        (
              int idPatrimonio
            , string sCodigo
            , string sDscProduto
            , string sUnidade
            , string dtPrevisaoUso
        )
        {
            _idPatrimonio = idPatrimonio;
            _sCodigo = sCodigo;
            _sDscProduto = sDscProduto;
            _sUnidade = sUnidade;
            _dtPrevisaoUso = dtPrevisaoUso;
        }

        #endregion

        #region | Membros Privados 


        private int _idPatrimonio;
        private string _sCodigo;
        private string _sDscProduto;
        private string _sUnidade;
        private string _nQuantidade;
        private string _dtPrevisaoUso;

        #endregion

        #region | Propriedades

        public int idPatrimonio
        {
            get { return _idPatrimonio; }
            set { _idPatrimonio = value; }
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

        public string dtPrevisaoUso
        {
            get { return _dtPrevisaoUso; }
            set { _dtPrevisaoUso = value; }
        }


        #endregion

    }
    #endregion

    #region | Recebimento
    [Serializable]
    public class cls_Recebimento
    {

        #region | Construtor 
        public cls_Recebimento(string sDscFormaRecebimento_Info_Rec = null)
        {
            _sDscFormaRecebimento_Info_Rec = sDscFormaRecebimento_Info_Rec;
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_Recebimento
        (

            int idRegistroRecebimento
            , int idContasReceber
            , decimal nValorRecebimento_Info_Rec
            , int idFormaRecebimento_Info_Rec
            , string dtRecebimento_Info_Rec
            , int nNumeroParcela_Info_Rec
            , string sDscFormaRecebimento_Info_Rec
            , int idConta_Info_Rec
            , string sDscConta_Info_Rec
            , decimal nMulta
            , decimal nDesconto
            , decimal nJuros
            , decimal nValorTotalRec

            , string dtAtualizacao
            , string idUsuarioAtualizacao
            , int idCredito


            //Arquivos
            , int idLinha
            , string sFuncao
            , int idArquivo
            , string sCor
        )
        {

            _idRegistroRecebimento = idRegistroRecebimento;
            _idContasReceber = idContasReceber;
            _nValorRecebimento_Info_Rec = nValorRecebimento_Info_Rec;
            _idFormaRecebimento_Info_Rec = idFormaRecebimento_Info_Rec;
            _dtRecebimento_Info_Rec = dtRecebimento_Info_Rec;
            _nNumeroParcela_Info_Rec = nNumeroParcela_Info_Rec;
            _sDscFormaRecebimento_Info_Rec = sDscFormaRecebimento_Info_Rec;
            _idConta_Info_Rec = idConta_Info_Rec;
            _sDscConta_Info_Rec = sDscConta_Info_Rec;
            _nMulta = nMulta;
            _nDesconto = nDesconto;
            _nJuros = nJuros;
            _nValorTotalRec = nValorTotalRec;

            _dtAtualizacao = dtAtualizacao;
            _idUsuarioAtualizacao = idUsuarioAtualizacao;
            _idCredito = idCredito;

            //Arquivos
            _idLinha = idLinha;
            _sFuncao = sFuncao;
            _idArquivo = idArquivo;
            _sCor = sCor;
        }

        #endregion

        #region | Membros Privados 

        private int _idRegistroRecebimento;
        private int _idContasReceber;
        private decimal _nValorRecebimento_Info_Rec;
        private int _idFormaRecebimento_Info_Rec;
        private string _dtRecebimento_Info_Rec;
        private int _nNumeroParcela_Info_Rec;
        private string _sDscFormaRecebimento_Info_Rec;
        private int _idConta_Info_Rec;
        private string _sDscConta_Info_Rec;
        private decimal _nMulta;
        private decimal _nDesconto;
        private decimal _nJuros;
        private decimal _nValorTotalRec;
        private decimal _nTarifa;

        private string _dtAtualizacao;
        private string _idUsuarioAtualizacao;
        private int _idCredito;

        //Arquivos
        private int _idArquivo;
        private string _sNomeArquivo;
        private string _sObservacaoArquivo;
        private Byte[] _objArquivo;
        private string _sFuncao;
        private int _idLinha;
        private string _sCor;


        public int idRegistroRecebimento { get => _idRegistroRecebimento; set => _idRegistroRecebimento = value; }
        public int idContasReceber { get => _idContasReceber; set => _idContasReceber = value; }
        public decimal nValorRecebimento_Info_Rec { get => _nValorRecebimento_Info_Rec; set => _nValorRecebimento_Info_Rec = value; }
        public int idFormaRecebimento_Info_Rec { get => _idFormaRecebimento_Info_Rec; set => _idFormaRecebimento_Info_Rec = value; }
        public string dtRecebimento_Info_Rec { get => _dtRecebimento_Info_Rec; set => _dtRecebimento_Info_Rec = value; }
        public int nNumeroParcela_Info_Rec { get => _nNumeroParcela_Info_Rec; set => _nNumeroParcela_Info_Rec = value; }
        public string sDscFormaRecebimento_Info_Rec { get => _sDscFormaRecebimento_Info_Rec; set => _sDscFormaRecebimento_Info_Rec = value; }
        public int idConta_Info_Rec { get => _idConta_Info_Rec; set => _idConta_Info_Rec = value; }
        public string sDscConta_Info_Rec { get => _sDscConta_Info_Rec; set => _sDscConta_Info_Rec = value; }

        public string dtAtualizacao { get => _dtAtualizacao; set => _dtAtualizacao = value; }
        public string idUsuarioAtualizacao { get => _idUsuarioAtualizacao; set => _idUsuarioAtualizacao = value; }
        public int idCredito { get => _idCredito; set => _idCredito = value; }


        //Arquivos
        public int idArquivo { get => _idArquivo; set => _idArquivo = value; }
        public string sNomeArquivo { get => _sNomeArquivo; set => _sNomeArquivo = value; }
        public string sObservacaoArquivo { get => _sObservacaoArquivo; set => _sObservacaoArquivo = value; }
        public byte[] objArquivo { get => _objArquivo; set => _objArquivo = value; }
        public string sFuncao { get => _sFuncao; set => _sFuncao = value; }
        public int idLinha { get => _idLinha; set => _idLinha = value; }
        public decimal nMulta { get => _nMulta; set => _nMulta = value; }
        public decimal nDesconto { get => _nDesconto; set => _nDesconto = value; }
        public decimal nJuros { get => _nJuros; set => _nJuros = value; }
        public decimal nValorTotalRec { get => _nValorTotalRec; set => _nValorTotalRec = value; }
        public decimal nTarifa { get => _nTarifa; set => _nTarifa = value; }
        public string sCor { get => _sCor; set => _sCor = value; }
        #endregion

        #region | Propriedades


        #endregion


    }
    #endregion

    #region | Pagamento
    [Serializable]
    public class cls_Pagamento
    {

        #region | Construtor 
        public cls_Pagamento()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_Pagamento
        (

            int idRegistroPagamento
            , int idContasPagar
            , decimal nValorPagamento_Info_Pag
            , int idFormaPagamento_Info_Pag
            , string dtPagamento_Info_Pag
            , int nNumeroParcela_Info_Pag
            , string sDscFormaPagamento_Info_Pag
            , int idConta_Info_Pag
            , string sDscConta_Info_Pag

            , decimal nMulta
            , decimal nDesconto
            , decimal nJuros
            , decimal nValorTotalPag

            , string dtAtualizacao
            , string idUsuarioAtualizacao
            , string sContratoCambio
            , string sCorretora

            //Arquivos
            , int idLinha
            , string sFuncao
            , int idArquivo
            , int idCompensacao
             , string sCor
        )
        {

            _idRegistroPagamento = idRegistroPagamento;
            _idContasPagar = idContasPagar;
            _nValorPagamento_Info_Pag = nValorPagamento_Info_Pag;
            _idFormaPagamento_Info_Pag = idFormaPagamento_Info_Pag;
            _dtPagamento_Info_Pag = dtPagamento_Info_Pag;
            _nNumeroParcela_Info_Pag = nNumeroParcela_Info_Pag;
            _sDscFormaPagamento_Info_Pag = sDscFormaPagamento_Info_Pag;
            _idConta_Info_Pag = idConta_Info_Pag;
            _sDscConta_Info_Pag = sDscConta_Info_Pag;

            _nMulta = nMulta;
            _nDesconto = nDesconto;
            _nJuros = nJuros;
            _nValorTotalPag = nValorTotalPag;
            _sContratoCambio = sContratoCambio;
            _sCorretora = sCorretora;

            _dtAtualizacao = dtAtualizacao;
            _idUsuarioAtualizacao = idUsuarioAtualizacao;

            //Arquivos
            _idLinha = idLinha;
            _sFuncao = sFuncao;
            _idArquivo = idArquivo;
            _idCompensacao = idCompensacao;
            _sCor = sCor;
        }

        #endregion

        #region | Membros Privados 

        private int _idRegistroPagamento;
        private int _idContasPagar;
        private decimal _nValorPagamento_Info_Pag;
        private int _idFormaPagamento_Info_Pag;
        private string _dtPagamento_Info_Pag;
        private int _nNumeroParcela_Info_Pag;
        private string _sDscFormaPagamento_Info_Pag;
        private int _idConta_Info_Pag;
        private string _sDscConta_Info_Pag;

        private decimal _nMulta;
        private decimal _nDesconto;
        private decimal _nJuros;
        private decimal _nValorTotalPag;

        private string _dtAtualizacao;
        private string _idUsuarioAtualizacao;
        private string _sContratoCambio;
        private string _sCorretora;

        //Arquivos
        private int _idArquivo;
        private string _sNomeArquivo;
        private string _sObservacaoArquivo;
        private Byte[] _objArquivo;
        private string _sFuncao;
        private int _idLinha;

        private int _idCompensacao;
        private string _sCor;

        public int idRegistroPagamento { get => _idRegistroPagamento; set => _idRegistroPagamento = value; }
        public int idContasPagar { get => _idContasPagar; set => _idContasPagar = value; }
        public decimal nValorPagamento_Info_Pag { get => _nValorPagamento_Info_Pag; set => _nValorPagamento_Info_Pag = value; }
        public int idFormaPagamento_Info_Pag { get => _idFormaPagamento_Info_Pag; set => _idFormaPagamento_Info_Pag = value; }
        public string dtPagamento_Info_Pag { get => _dtPagamento_Info_Pag; set => _dtPagamento_Info_Pag = value; }
        public int nNumeroParcela_Info_Pag { get => _nNumeroParcela_Info_Pag; set => _nNumeroParcela_Info_Pag = value; }
        public string sDscFormaPagamento_Info_Pag { get => _sDscFormaPagamento_Info_Pag; set => _sDscFormaPagamento_Info_Pag = value; }
        public int idConta_Info_Pag { get => _idConta_Info_Pag; set => _idConta_Info_Pag = value; }
        public string sDscConta_Info_Pag { get => _sDscConta_Info_Pag; set => _sDscConta_Info_Pag = value; }

        public string dtAtualizacao { get => _dtAtualizacao; set => _dtAtualizacao = value; }
        public string idUsuarioAtualizacao { get => _idUsuarioAtualizacao; set => _idUsuarioAtualizacao = value; }
        public string sContratoCambio { get => _sContratoCambio; set => _sContratoCambio = value; }
        public string sCorretora { get => _sCorretora; set => _sCorretora = value; }


        //Arquivos
        public int idArquivo { get => _idArquivo; set => _idArquivo = value; }
        public string sNomeArquivo { get => _sNomeArquivo; set => _sNomeArquivo = value; }
        public string sObservacaoArquivo { get => _sObservacaoArquivo; set => _sObservacaoArquivo = value; }
        public byte[] objArquivo { get => _objArquivo; set => _objArquivo = value; }
        public string sFuncao { get => _sFuncao; set => _sFuncao = value; }
        public int idLinha { get => _idLinha; set => _idLinha = value; }
        public decimal nMulta { get => _nMulta; set => _nMulta = value; }
        public decimal nDesconto { get => _nDesconto; set => _nDesconto = value; }
        public decimal nJuros { get => _nJuros; set => _nJuros = value; }
        public decimal nValorTotalPag { get => _nValorTotalPag; set => _nValorTotalPag = value; }


        public int idCompensacao { get => _idCompensacao; set => _idCompensacao = value; }
        public string sCor { get => _sCor; set => _sCor = value; }
        #endregion

        #region | Propriedades


        #endregion


    }
    #endregion

    #region | LancamentoPag
    [Serializable]
    public class cls_LancamentoPag
    {

        #region | Construtor 
        public cls_LancamentoPag()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_LancamentoPag
        (

            int idRegistroLancamentoPag
            , int idContasPagar
            , string dtLancamentoPag
            , int nParcelaLancamentoPag
            , string dtAtualizacao
            , int idUsuarioAtualizacao
            , decimal nValorLancamentoPag
            , int idLinha
            , string sFuncao
            , int idFormaPagamentoLancamentoPag
            , string sDscFormaPagamentoLancamentoPag

            , decimal nValorBrutoLancamentoPag
        )
        {

            _idRegistroLancamentoPag = idRegistroLancamentoPag;
            _idContasPagar = idContasPagar;
            _dtLancamentoPag = dtLancamentoPag;
            _nParcelaLancamentoPag = nParcelaLancamentoPag;

            _dtAtualizacao = dtAtualizacao;
            _idUsuarioAtualizacao = idUsuarioAtualizacao;
            _nValorLancamentoPag = nValorLancamentoPag;
            _idLinha = idLinha;
            _sFuncao = sFuncao;
            _idFormaPagamentoLancamentoPag = idFormaPagamentoLancamentoPag;
            _sDscFormaPagamentoLancamentoPag = sDscFormaPagamentoLancamentoPag;

            _nValorBrutoLancamentoPag = nValorBrutoLancamentoPag;
        }

        #endregion

        #region | Membros Privados 

        private int _idRegistroLancamentoPag;
        private int _idContasPagar;
        private string _dtLancamentoPag;
        private int _nParcelaLancamentoPag;

        private string _dtAtualizacao;
        private int _idUsuarioAtualizacao;
        private decimal _nValorLancamentoPag;
        private int _idLinha;
        private string _sFuncao;
        private int _idFormaPagamentoLancamentoPag;
        private string _sDscFormaPagamentoLancamentoPag;


        private decimal _nValorBrutoLancamentoPag;

        public int idRegistroLancamentoPag { get => _idRegistroLancamentoPag; set => _idRegistroLancamentoPag = value; }
        public int idContasPagar { get => _idContasPagar; set => _idContasPagar = value; }
        public string dtLancamentoPag { get => _dtLancamentoPag; set => _dtLancamentoPag = value; }
        public int nParcelaLancamentoPag { get => _nParcelaLancamentoPag; set => _nParcelaLancamentoPag = value; }
        public string dAtualizacao { get => _dtAtualizacao; set => _dtAtualizacao = value; }
        public int idUsuarioAtualizacao { get => _idUsuarioAtualizacao; set => _idUsuarioAtualizacao = value; }
        public decimal nValorLancamentoPag { get => _nValorLancamentoPag; set => _nValorLancamentoPag = value; }
        public int idLinha { get => _idLinha; set => _idLinha = value; }
        public string sFuncao { get => _sFuncao; set => _sFuncao = value; }
        public int idFormaPagamentoLancamentoPag { get => _idFormaPagamentoLancamentoPag; set => _idFormaPagamentoLancamentoPag = value; }
        public string sDscFormaPagamentoLancamentoPag { get => _sDscFormaPagamentoLancamentoPag; set => _sDscFormaPagamentoLancamentoPag = value; }


        public decimal nValorBrutoLancamentoPag { get => _nValorBrutoLancamentoPag; set => _nValorBrutoLancamentoPag = value; }




        #endregion

        #region | Propriedades


        #endregion


    }
    #endregion

    #region | LancamentoRec
    [Serializable]
    public class cls_LancamentoRec
    {

        #region | Construtor 
        public cls_LancamentoRec()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_LancamentoRec
        (

            int idRegistroLancamentoRec
            , int idContasReceber
            , string dtLancamentoRec
            , int nParcelaLancamentoRec
            , string dtAtualizacao
            , int idUsuarioAtualizacao
            , decimal nValorLancamentoRec
            , int idLinha
            , string sFuncao
            , int idFormaPagamentoLancamentoRec
            , string sDscFormaPagamentoLancamentoRec

            , decimal nValorBrutoLancamentoRec

        )
        {

            _idRegistroLancamentoRec = idRegistroLancamentoRec;
            _idContasReceber = idContasReceber;
            _dtLancamentoRec = dtLancamentoRec;
            _nParcelaLancamentoRec = nParcelaLancamentoRec;

            _dtAtualizacao = dtAtualizacao;
            _idUsuarioAtualizacao = idUsuarioAtualizacao;
            _nValorLancamentoRec = nValorLancamentoRec;
            _idLinha = idLinha;
            _sFuncao = sFuncao;
            _idFormaPagamentoLancamentoRec = idFormaPagamentoLancamentoRec;
            _sDscFormaPagamentoLancamentoRec = sDscFormaPagamentoLancamentoRec;
            _nValorBrutoLancamentoRec = nValorBrutoLancamentoRec;
        }

        #endregion

        #region | Membros Privados 

        private int _idRegistroLancamentoRec;
        private int _idContasReceber;
        private string _dtLancamentoRec;
        private int _nParcelaLancamentoRec;

        private string _dtAtualizacao;
        private int _idUsuarioAtualizacao;
        private decimal _nValorLancamentoRec;
        private int _idLinha;
        private string _sFuncao;
        private int _idFormaPagamentoLancamentoRec;
        private int _idContaBancaria;
        private string _sDscFormaPagamentoLancamentoRec;

        private decimal _nValorBrutoLancamentoRec;

        public int idRegistroLancamentoRec { get => _idRegistroLancamentoRec; set => _idRegistroLancamentoRec = value; }
        public int idContasReceber { get => _idContasReceber; set => _idContasReceber = value; }
        public string dtLancamentoRec { get => _dtLancamentoRec; set => _dtLancamentoRec = value; }
        public int nParcelaLancamentoRec { get => _nParcelaLancamentoRec; set => _nParcelaLancamentoRec = value; }
        public string dtAtualizacao { get => _dtAtualizacao; set => _dtAtualizacao = value; }
        public int idUsuarioAtualizacao { get => _idUsuarioAtualizacao; set => _idUsuarioAtualizacao = value; }
        public decimal nValorLancamentoRec { get => _nValorLancamentoRec; set => _nValorLancamentoRec = value; }
        public int idLinha { get => _idLinha; set => _idLinha = value; }
        public string sFuncao { get => _sFuncao; set => _sFuncao = value; }
        public int idFormaPagamentoLancamentoRec { get => _idFormaPagamentoLancamentoRec; set => _idFormaPagamentoLancamentoRec = value; }
        public int idContaBancaria { get => _idContaBancaria; set => _idContaBancaria = value; }
        public string sDscFormaPagamentoLancamentoRec { get => _sDscFormaPagamentoLancamentoRec; set => _sDscFormaPagamentoLancamentoRec = value; }
        public decimal nValorBrutoLancamentoRec { get => _nValorBrutoLancamentoRec; set => _nValorBrutoLancamentoRec = value; }





        #endregion

        #region | Propriedades


        #endregion


    }
    #endregion

    #region | Meta
    [Serializable]
    public class cls_Meta
    {

        #region | Construtor 
        public cls_Meta()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public cls_Meta
        (
            int idMeta
            , decimal nValor
            , string sDscUsuarioAtualizacao
            , string dtAtualizacao
            , string sFuncao
            , int nAno
            , int idMes
            , string sDscMes
            , int idVendedor
            , string sDscVendedor

        )
        {

            _idMeta = idMeta;
            _nValor = nValor;
            _sDscUsuarioAtualizacao = sDscUsuarioAtualizacao;
            _dtAtualizacao = dtAtualizacao;
            _sFuncao = sFuncao;
            _nAno = nAno;
            _idMes = idMes;
            _sDscMes = sDscMes;
            _idVendedor = idVendedor;
            _sDscVendedor = sDscVendedor;
        }

        #endregion

        #region | Membros Privados 

        private int _idMeta;
        private decimal _nValor;
        private string _sDscUsuarioAtualizacao;
        private string _dtAtualizacao;
        private string _sFuncao;
        private int _nAno;
        private int _idMes;
        private string _sDscMes;
        private int _idVendedor;
        private string _sDscVendedor;

        public int IdMeta { get => _idMeta; set => _idMeta = value; }
        public decimal NValor { get => _nValor; set => _nValor = value; }
        public string SDscUsuarioAtualizacao { get => _sDscUsuarioAtualizacao; set => _sDscUsuarioAtualizacao = value; }
        public string DtAtualizacao { get => _dtAtualizacao; set => _dtAtualizacao = value; }
        public string SFuncao { get => _sFuncao; set => _sFuncao = value; }
        public int NAno { get => _nAno; set => _nAno = value; }
        public int IdMes { get => _idMes; set => _idMes = value; }
        public string SDscMes { get => _sDscMes; set => _sDscMes = value; }
        public int idVendedor { get => _idVendedor; set => _idVendedor = value; }
        public string sDscVendedor { get => _sDscVendedor; set => _sDscVendedor = value; }

        #endregion


        #region | Funções

        public static DataSet Consulta()
        {
            string sErro = "";
            string sProcedure = "sp_Manipula_tbl_Flow_Adm_Metas";

            try
            {
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR");
                DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsPesquisa, out sErro))
                {
                    return dsPesquisa;
                }
                else
                {
                    throw new Exception(sErro);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static void ConverterDatasetClasse(DataSet ds, List<cls_Meta> BS_META)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                FrameWork.cls_Meta objItem = new FrameWork.cls_Meta();
                objItem.IdMeta = Convert.ToInt32(row["idMeta"].ToString());
                objItem.NValor = Convert.ToDecimal(row["nValor"].ToString());
                objItem.SDscUsuarioAtualizacao = row["sDscUsuarioAtualizacao"].ToString();
                objItem.DtAtualizacao = row["dtAtualizacao"].ToString();
                objItem.NAno = Convert.ToInt32(row["nAno"].ToString());
                objItem.IdMes = Convert.ToInt32(row["idMes"].ToString());
                objItem.SDscMes = row["sDscMes"].ToString();
                objItem.SFuncao = "ATUAL";
                BS_META.Add(objItem);
            }
        }

        #endregion





    }
    #endregion

    #region | Adiantamento
    [Serializable]
    public class cls_Adiantamento
    {

        #region | Construtor 

        public cls_Adiantamento
        (

            int idConta_Info_Pag
            , int idCategoriaPagar
            , string sidsContasReceber
            , decimal nValorTotalComJuros
            , decimal nSomaValorOriginal
            , decimal nTaxaJuros
            , decimal nIOF
            , decimal nTarifas
            , string dtVencimento
            , string sFuncao
            , string dtAtualizacao
            , string idUsuarioAtualizacao
            , decimal nIOFAdicional
            , decimal nTaxaJurosNominal
            , decimal nDespesas
            , decimal nValorEmprestimo
            , decimal nCET
            , int nDias
            , decimal nIOFTotal
            , decimal nIOFAdicionalTotal
            , decimal nJurosTotal
            , decimal nSomaIOF
            , string dtEmissaoAdiantamento
            , int idEmpresa
            , int idContasPagar_Adiantado
            , decimal nValorLiberado    
            , int nContadorAdiantamento
        )
        {

            _idConta_Info_Pag = idConta_Info_Pag;
            _idCategoriaPagar = idCategoriaPagar;
            _sidsContasReceber = sidsContasReceber;
            _nValorTotalComJuros = nValorTotalComJuros;
            _nSomaValorOriginal = nSomaValorOriginal;
            _nTaxaJuros = nTaxaJuros;
            _nIOF = nIOF;
            _nTarifas = nTarifas;
            _dtVencimento = dtVencimento;
            _sFuncao = sFuncao;
            _dtAtualizacao = dtAtualizacao;
            _idUsuarioAtualizacao = idUsuarioAtualizacao;
            _nIOFAdicional = nIOFAdicional;
            _nTaxaJurosNominal = nTaxaJurosNominal;
            _nDespesas = nDespesas;
            _nValorEmprestimo = nValorEmprestimo;
            _nCET = nCET;
            _nDias = nDias;
            _nIOFTotal = nIOFTotal;
            _nIOFAdicionalTotal = nIOFAdicionalTotal;
            _nJurosTotal = nJurosTotal;
            _nSomaIOF = nSomaIOF;
            _dtEmissaoAdiantamento = dtEmissaoAdiantamento;
            _idEmpresa = idEmpresa;
            _idContasPagar_Adiantado = idContasPagar_Adiantado;
            _nValorLiberado = nValorLiberado;
            _nContadorAdiantamento = nContadorAdiantamento;
        }

        #endregion

        #region | Membros Privados 


        private int _idConta_Info_Pag;
        private int _idCategoriaPagar;
        private string _sidsContasReceber;
        private decimal _nValorTotalComJuros;
        private decimal _nSomaValorOriginal;
        private decimal _nTaxaJuros;
        private decimal _nIOF;
        private decimal _nTarifas;
        private string _dtVencimento;
        private string _sFuncao;
        private string _dtAtualizacao;
        private string _idUsuarioAtualizacao;
        private decimal _nIOFAdicional;
        private decimal _nTaxaJurosNominal;
        private decimal _nDespesas;
        private decimal _nValorEmprestimo;
        private decimal _nCET;
        private int _nDias;
        private decimal _nIOFTotal;
        private decimal _nIOFAdicionalTotal;
        private decimal _nJurosTotal;
        private decimal _nSomaIOF;
        private string _dtEmissaoAdiantamento;
        private int _idEmpresa;
        private int _idContasPagar_Adiantado;
        private decimal _nValorLiberado;
        private int _nContadorAdiantamento;

        #endregion

        #region | Propriedades

        public int idConta_Info_Pag { get => _idConta_Info_Pag; set => _idConta_Info_Pag = value; }
        public int idCategoriaPagar { get => _idCategoriaPagar; set => _idCategoriaPagar = value; }
        public string sidsContasReceber { get => _sidsContasReceber; set => _sidsContasReceber = value; }
        public decimal nValorTotalComJuros { get => _nValorTotalComJuros; set => _nValorTotalComJuros = value; }
        public decimal nSomaValorOriginal { get => _nSomaValorOriginal; set => _nSomaValorOriginal = value; }
        public decimal nTaxaJuros { get => _nTaxaJuros; set => _nTaxaJuros = value; }
        public decimal nIOF { get => _nIOF; set => _nIOF = value; }
        public decimal nTarifas { get => _nTarifas; set => _nTarifas = value; }
        public string dtVencimento { get => _dtVencimento; set => _dtVencimento = value; }
        public string sFuncao { get => _sFuncao; set => _sFuncao = value; }
        public string dtAtualizacao { get => _dtAtualizacao; set => _dtAtualizacao = value; }
        public string idUsuarioAtualizacao { get => _idUsuarioAtualizacao; set => _idUsuarioAtualizacao = value; }
        public decimal nIOFAdicional { get => _nIOFAdicional; set => _nIOFAdicional = value; }
        public decimal nTaxaJurosNominal { get => _nTaxaJurosNominal; set => _nTaxaJurosNominal = value; }
        public decimal nDespesas { get => _nDespesas; set => _nDespesas = value; }
        public decimal nValorEmprestimo { get => _nValorEmprestimo; set => _nValorEmprestimo = value; }
        public decimal nCET { get => _nCET; set => _nCET = value; }
        public int nDias { get => _nDias; set => _nDias = value; }
        public decimal nIOFTotal { get => _nIOFTotal; set => _nIOFTotal = value; }
        public decimal nIOFAdicionalTotal { get => _nIOFAdicionalTotal; set => nIOFAdicionalTotal = value; }
        public decimal nJurosTotal { get => _nJurosTotal; set => _nJurosTotal = value; }
        public decimal nSomaIOF { get => _nSomaIOF; set => _nSomaIOF = value; }
        public string dtEmissaoAdiantamento { get => _dtEmissaoAdiantamento; set => _dtEmissaoAdiantamento = value; }
        public int idEmpresa { get => _idEmpresa; set => _idEmpresa = value; }
        public int idContasPagar_Adiantado { get => _idContasPagar_Adiantado; set => _idContasPagar_Adiantado = value; }
        public decimal nValorLiberado { get => _nValorLiberado; set => _nValorLiberado = value; }     
        public int nContadorAdiantamento { get => _nContadorAdiantamento; set => _nContadorAdiantamento = value; }

        #endregion


    }
    #endregion

    #region | AdiantamentoDetalhe
    [Serializable]
    public class cls_Adiantamento_Detalhe
    {

        #region | Construtor 

        public cls_Adiantamento_Detalhe
        (

            int idContasReceber_Detalhe
            , int idContasPagar_Detalhe
            , decimal nValorTotalComJuros_Detalhe
            , decimal nSomaValorOriginal_Detalhe
            , decimal nTaxaJuros_Detalhe
            , decimal nIOF_Detalhe
            , decimal nTarifas_Detalhe
            , string dtVencimento_Detalhe
            , decimal nIOFAdicional_Detalhe
            , decimal nTaxaJurosNominal_Detalhe
            , decimal nDespesas_Detalhe
            , decimal nValorEmprestimo_Detalhe
            , decimal nCET_Detalhe
            , int nDias_Detalhe
            , decimal nIOFTotal_Detalhe
            , decimal nIOFAdicionalTotal_Detalhe
            , decimal nJurosTotal_Detalhe
            , decimal nSomaIOF_Detalhe
            , string dtEmissaoAdiantamento_Detalhe
            , int idEmpresa_Detalhe
            , string sDscCliente_Detalhe
            , decimal nValorLiberado_Detalhe
            , decimal nValorTitulo
            , decimal nTotalOperacao
            , decimal nTotalDespesas
            , decimal nDivisaoTarifa
            , string sTipoCalculo

        )
        {

            _idContasReceber_Detalhe = idContasReceber_Detalhe;
            _idContasPagar_Detalhe = idContasPagar_Detalhe;
            _nValorTotalComJuros_Detalhe = nValorTotalComJuros_Detalhe;
            _nSomaValorOriginal_Detalhe = nSomaValorOriginal_Detalhe;
            _nTaxaJuros_Detalhe = nTaxaJuros_Detalhe;
            _nIOF_Detalhe = nIOF_Detalhe;
            _nTarifas_Detalhe = nTarifas_Detalhe;
            _dtVencimento_Detalhe = dtVencimento_Detalhe;
            _nIOFAdicional_Detalhe = nIOFAdicional_Detalhe;
            _nTaxaJurosNominal_Detalhe = nTaxaJurosNominal_Detalhe;
            _nDespesas_Detalhe = nDespesas_Detalhe;
            _nValorEmprestimo_Detalhe = nValorEmprestimo_Detalhe;
            _nCET_Detalhe = nCET_Detalhe;
            _nDias_Detalhe = nDias_Detalhe;
            _nIOFTotal_Detalhe = nIOFTotal_Detalhe;
            _nIOFAdicionalTotal_Detalhe = nIOFAdicionalTotal_Detalhe;
            _nJurosTotal_Detalhe = nJurosTotal_Detalhe;
            _nSomaIOF_Detalhe = nSomaIOF_Detalhe;
            _dtEmissaoAdiantamento_Detalhe = dtEmissaoAdiantamento_Detalhe;
            _idEmpresa_Detalhe = idEmpresa_Detalhe;
            _sDscCliente_Detalhe = sDscCliente_Detalhe;
            _nValorLiberado_Detalhe = nValorLiberado_Detalhe;
            _nValorTitulo = nValorTitulo;
            _nTotalOperacao = nTotalOperacao;
            _nTotalDespesas = nTotalDespesas;
            _nDivisaoTarifa = nDivisaoTarifa;
            _sTipoCalculo = sTipoCalculo;
        }

        #endregion

        #region | Membros Privados 


        private int _idContasReceber_Detalhe;
        private int _idContasPagar_Detalhe;
        private decimal _nValorTotalComJuros_Detalhe;
        private decimal _nSomaValorOriginal_Detalhe;
        private decimal _nTaxaJuros_Detalhe;
        private decimal _nIOF_Detalhe;
        private decimal _nTarifas_Detalhe;
        private string _dtVencimento_Detalhe;
        private decimal _nIOFAdicional_Detalhe;
        private decimal _nTaxaJurosNominal_Detalhe;
        private decimal _nDespesas_Detalhe;
        private decimal _nValorEmprestimo_Detalhe;
        private decimal _nCET_Detalhe;
        private int _nDias_Detalhe;
        private decimal _nIOFTotal_Detalhe;
        private decimal _nIOFAdicionalTotal_Detalhe;
        private decimal _nJurosTotal_Detalhe;
        private decimal _nSomaIOF_Detalhe;
        private string _dtEmissaoAdiantamento_Detalhe;
        private int _idEmpresa_Detalhe;
        private string _sDscCliente_Detalhe;
        private decimal _nValorLiberado_Detalhe;
        private decimal _nValorTitulo;
        private decimal _nTotalOperacao;
        private decimal _nTotalDespesas;
        private decimal _nDivisaoTarifa;
        private string _sTipoCalculo;

        #endregion

        #region | Propriedades
        public int idContasReceber_Detalhe { get => _idContasReceber_Detalhe; set => _idContasReceber_Detalhe = value; }
        public int IdContasPagar_Detalhe { get => _idContasPagar_Detalhe; set => _idContasPagar_Detalhe = value; }
        public decimal nValorTotalComJuros_Detalhe { get => _nValorTotalComJuros_Detalhe; set => _nValorTotalComJuros_Detalhe = value; }
        public decimal nSomaValorOriginal_Detalhe { get => _nSomaValorOriginal_Detalhe; set => _nSomaValorOriginal_Detalhe = value; }
        public decimal nTaxaJuros_Detalhe { get => _nTaxaJuros_Detalhe; set => _nTaxaJuros_Detalhe = value; }
        public decimal nIOF_Detalhe { get => _nIOF_Detalhe; set => _nIOF_Detalhe = value; }
        public decimal nTarifas_Detalhe { get => _nTarifas_Detalhe; set => _nTarifas_Detalhe = value; }
        public string dtVencimento_Detalhe { get => _dtVencimento_Detalhe; set => _dtVencimento_Detalhe = value; }
        public decimal nIOFAdicional_Detalhe { get => _nIOFAdicional_Detalhe; set => _nIOFAdicional_Detalhe = value; }
        public decimal nTaxaJurosNominal_Detalhe { get => _nTaxaJurosNominal_Detalhe; set => _nTaxaJurosNominal_Detalhe = value; }
        public decimal nDespesas_Detalhe { get => _nDespesas_Detalhe; set => _nDespesas_Detalhe = value; }
        public decimal nValorEmprestimo_Detalhe { get => _nValorEmprestimo_Detalhe; set => _nValorEmprestimo_Detalhe = value; }
        public decimal nCET_Detalhe { get => _nCET_Detalhe; set => _nCET_Detalhe = value; }
        public int nDias_Detalhe { get => _nDias_Detalhe; set => _nDias_Detalhe = value; }
        public decimal nIOFTotal_Detalhe { get => _nIOFTotal_Detalhe; set => _nIOFTotal_Detalhe = value; }
        public decimal nIOFAdicionalTotal_Detalhe { get => _nIOFAdicionalTotal_Detalhe; set => nIOFAdicionalTotal_Detalhe = value; }
        public decimal nJurosTotal_Detalhe { get => _nJurosTotal_Detalhe; set => _nJurosTotal_Detalhe = value; }
        public decimal nSomaIOF_Detalhe { get => _nSomaIOF_Detalhe; set => _nSomaIOF_Detalhe = value; }
        public string dtEmissaoAdiantamento_Detalhe { get => _dtEmissaoAdiantamento_Detalhe; set => _dtEmissaoAdiantamento_Detalhe = value; }
        public int idEmpresa_Detalhe { get => _idEmpresa_Detalhe; set => _idEmpresa_Detalhe = value; }
        public string sDscCliente_Detalhe { get => _sDscCliente_Detalhe; set => _sDscCliente_Detalhe = value; }
        public decimal nValorLiberado_Detalhe { get => _nValorLiberado_Detalhe; set => _nValorLiberado_Detalhe = value; }
        public decimal nValorTitulo { get => _nValorTitulo; set => _nValorTitulo = value; }
        public decimal nTotalOperacao { get => _nTotalOperacao; set => _nTotalOperacao = value; }
        public decimal nTotalDespesas { get => _nTotalDespesas; set => _nTotalDespesas = value; }
        public decimal nDivisaoTarifa { get => _nDivisaoTarifa; set => _nDivisaoTarifa = value; }
        public string sTipoCalculo { get => _sTipoCalculo; set => _sTipoCalculo = value; }

        #endregion

    }
    #endregion

    //Agnes Partal * 21/06/2024
    #region| Extrato
    [Serializable]
    public class cls_Extrato
    {
        #region | Construtor

        public cls_Extrato() { }

        public cls_Extrato(
            string sAgencia
            , string sNumeroConta
            , string dtLancamento
            , string sDscLancamento
            , string sNumeroDocumento
            , string sCredito
            , string sDebito
            , string sSaldoAnterior
            , string sObservacao        //Agnes Partal - 25/06/2024
            , string sSaldoAtual        //Agnes Partal - 12/08/2024
            )
        {
            _sAgencia = sAgencia;
            _sNumeroConta = sNumeroConta;
            _dtLancamento = dtLancamento;
            _sDscLancamento = sDscLancamento;
            _sNumeroDocumento = sNumeroDocumento;
            _sCredito = sCredito;
            _sDebito = sDebito;
            _sSaldoAnterior = sSaldoAnterior;
            _sObservacao = sObservacao;
            _sSaldoAtual = sSaldoAtual;
        }

        #endregion

        #region | Membros Privados

        private string _sAgencia;
        private string _sNumeroConta;
        private string _dtLancamento;
        private string _sDscLancamento;
        private string _sNumeroDocumento;
        private string _sCredito;
        private string _sDebito;
        private string _sSaldoAnterior;
        private string _sObservacao;
        private string _sSaldoAtual;

        #endregion

        #region | Propriedades

        public string sAgencia { get => _sAgencia; set => _sAgencia = value; }
        public string sNumeroConta { get => _sNumeroConta; set => _sNumeroConta = value; }
        public string dtLancamento { get => _dtLancamento; set => _dtLancamento = value; }
        public string sDscLancamento { get => _sDscLancamento; set => _sDscLancamento = value; }
        public string sNumeroDocumento { get => _sNumeroDocumento; set => _sNumeroDocumento = value; }
        public string sCredito { get => _sCredito; set => _sCredito = value; }
        public string sDebito { get => _sDebito; set => _sDebito = value; }
        public string sSaldoAnterior { get => _sSaldoAnterior; set => _sSaldoAnterior = value; }
        public string sObservacao { get => _sObservacao; set => _sObservacao = value; }
        public string sSaldoAtual { get => _sSaldoAtual; set => _sSaldoAtual = value; }

        #endregion
    }

    //Agnes Partal * 23/08/2024
    [Serializable]
    public class cls_ConciliacaoComposta
    {
        #region | Construtor

        public cls_ConciliacaoComposta() { }

        public cls_ConciliacaoComposta(
            string dtVencimento
            , string sCodigo
            , string dtEmissao
            , string sDocumento
            , string idContabil
            , string idCentroDeCusto
            , string idEmpresa
            , string idParceiro
            , string sObservacaoGeral
            , string nSaldo
            , string nValorLiquido
            , string nValorBruto
            , string idFormaTransacao
            , string idMeioTransacao
            , string dtApuracao
            , string idColaborador
            , string idCategoria
            , string nValorTransacao
            , string dtTransacao
            , string nValorTotal
            )
        {
            _dtVencimento = dtVencimento;
            _sCodigo = sCodigo;
            _dtEmissao = dtEmissao;
            _sDocumento = sDocumento;
            _idContabil = idContabil;
            _idCentroDeCusto = idCentroDeCusto;
            _idEmpresa = idEmpresa;
            _idParceiro = idParceiro;
            _sObservacaoGeral = sObservacaoGeral;
            _nSaldo = nSaldo;
            _nValorLiquido = nValorLiquido;
            _nValorBruto = nValorBruto;
            _idFormaTransacao = idFormaTransacao;
            _idMeioTransacao = idMeioTransacao;
            _dtApuracao = dtApuracao;
            _idColaborador = idColaborador;
            _idCategoria = idCategoria;
            _nValorTransacao = nValorTransacao;
            _dtTransacao = dtTransacao;
            _nValorTotal = nValorTotal;
        }

        #endregion

        #region | Membros Privados

        private string _dtVencimento;
        private string _sCodigo;
        private string _dtEmissao;
        private string _sDocumento;
        private string _idContabil;
        private string _idCentroDeCusto;
        private string _idEmpresa;
        private string _idParceiro;
        private string _sObservacaoGeral;
        private string _nSaldo;
        private string _nValorLiquido;
        private string _nValorBruto;
        private string _idFormaTransacao;
        private string _idMeioTransacao;
        private string _dtApuracao;
        private string _idColaborador;
        private string _idCategoria;
        private string _nValorTransacao;
        private string _dtTransacao;
        private string _nValorTotal;

        #endregion

        #region | Propriedades

        public string dtVencimento { get => _dtVencimento; set => _dtVencimento = value; }
        public string sCodigo { get => _sCodigo; set => _sCodigo = value; }
        public string dtEmissao { get => _dtEmissao; set => _dtEmissao = value; }
        public string sDocumento { get => _sDocumento; set => _sDocumento = value; }
        public string idContabil { get => _idContabil; set => _idContabil = value; }
        public string idCentroDeCusto { get => _idCentroDeCusto; set => _idCentroDeCusto = value; }
        public string idEmpresa { get => _idEmpresa; set => _idEmpresa = value; }
        public string idParceiro { get => _idParceiro; set => _idParceiro = value; }
        public string sObservacaoGeral { get => _sObservacaoGeral; set => _sObservacaoGeral = value; }
        public string nSaldo { get => _nSaldo; set => _nSaldo = value; }
        public string nValorLiquido { get => _nValorLiquido; set => _nValorLiquido = value; }
        public string nValorBruto { get => _nValorBruto; set => _nValorBruto = value; }
        public string idFormaTransacao { get => _idFormaTransacao; set => _idFormaTransacao = value; }
        public string idMeioTransacao { get => _idMeioTransacao; set => _idMeioTransacao = value; }
        public string dtApuracao { get => _dtApuracao; set => _dtApuracao = value; }
        public string idColaborador { get => _idColaborador; set => _idColaborador = value; }
        public string idCategoria { get => _idCategoria; set => _idCategoria = value; }
        public string nValorTransacao { get => _nValorTransacao; set => _nValorTransacao = value; }
        public string dtTransacao { get => _dtTransacao; set => _dtTransacao = value; }
        public string nValorTotal { get => _nValorTotal; set => _nValorTotal = value; }

        #endregion
    }
    #endregion

    //Agnes Partal * 11/11/2024
    #region | Meta Comercial
    [Serializable]
    public class cls_Meta_Vendedor
    {

        #region | Construtor 
        public cls_Meta_Vendedor()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public cls_Meta_Vendedor
        (
            int idMeta
            , decimal nValor
            , string dtAtualizacao
            , int nAno
            , int idMes
            , string sDscMes
            , int idVendedor
            , string sDscVendedor
            , int idEmpresa
            , string sDscEmpresa

        )
        {

            _idMeta = idMeta;
            _nValor = nValor;
            _dtAtualizacao = dtAtualizacao;
            _nAno = nAno;
            _idMes = idMes;
            _sDscMes = sDscMes;
            _idVendedor = idVendedor;
            _sDscVendedor = sDscVendedor;
            _idEmpresa = idEmpresa;
            _sDscEmpresa = sDscEmpresa;
        }

        #endregion

        #region | Membros Privados 

        private int _idMeta;
        private decimal _nValor;
        private string _dtAtualizacao;
        private int _nAno;
        private int _idMes;
        private string _sDscMes;
        private int _idVendedor;
        private string _sDscVendedor;
        private int _idEmpresa;
        private string _sDscEmpresa;

        public int idMeta { get => _idMeta; set => _idMeta = value; }
        public decimal nValor { get => _nValor; set => _nValor = value; }
        public string dtAtualizacao { get => _dtAtualizacao; set => _dtAtualizacao = value; }
        public int nAno { get => _nAno; set => _nAno = value; }
        public int idMes { get => _idMes; set => _idMes = value; }
        public string sDscMes { get => _sDscMes; set => _sDscMes = value; }
        public int idVendedor { get => _idVendedor; set => _idVendedor = value; }
        public string sDscVendedor { get => _sDscVendedor; set => _sDscVendedor = value; }
        public int idEmpresa { get => _idEmpresa; set => _idEmpresa = value; }
        public string sDscEmpresa { get => _sDscEmpresa; set => _sDscEmpresa = value; }

        #endregion

    }

    [Serializable]
    public class cls_Meta_Mensal
    {

        #region | Construtor 
        public cls_Meta_Mensal()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public cls_Meta_Mensal
        (
            int idMeta
            , int nAno
            , int idMes
            , string sDscMes
            , decimal nTotalMetaMensal
            , List<cls_Meta_Vendedor> lsMetaVendedores
            , int idEmpresa

        )
        {
            _idMeta = idMeta;
            _nAno = nAno;
            _idMes = idMes;
            _sDscMes = sDscMes;
            _nTotalMetaMensal = nTotalMetaMensal;
            _lsMetaVendedores = lsMetaVendedores;
            _idEmpresa = idEmpresa;
        }

        #endregion

        #region | Membros Privados 

        private int _idMeta;
        private int _nAno;
        private int _idMes;
        private string _sDscMes;
        private decimal _nTotalMetaMensal;
        private List<cls_Meta_Vendedor> _lsMetaVendedores;
        public int _idEmpresa;

        public int idMeta { get => _idMeta; set => _idMeta = value; }
        public int nAno { get => _nAno; set => _nAno = value; }
        public int idMes { get => _idMes; set => _idMes = value; }
        public string sDscMes { get => _sDscMes; set => _sDscMes = value; }
        public decimal nTotalMetaMensal { get => _nTotalMetaMensal; set => _nTotalMetaMensal = value; }
        public List<cls_Meta_Vendedor> lsMetaVendedores { get => _lsMetaVendedores; set => _lsMetaVendedores = value; }
        public int idEmpresa { get => _idEmpresa; set => _idEmpresa = value; }

        #endregion

        #region | Funções

        public string MesAno
        {
            get { return sDscMes + "/" + nAno; }
        }

        #endregion

    }

    [Serializable]
    public class cls_EmpresaMeta
    {
        public int idEmpresa { get; set; }
        public string sEmpresa { get; set; }
        public decimal nTotalEmpresa { get; set; }
        public List<cls_Meta_Mensal> lsMetasMensais { get; set; }
        public int nAno { get; set; }
        public int idMes { get; set; }

    }

    #endregion

    #region | Fatura Cartoes

    [Serializable]
    public class cls_Fatura
    {
        #region | Construtor
        public cls_Fatura()
        {

        }

        public cls_Fatura
        (
            int idBanco
            , string sBanco
            , string sEmpresa
            , string sDscCartao
            , int idCartao
            , decimal nTotalFatura
            , List<cls_FaturaCartao> lsCartoes
            , int idFatura
            , string mesAno
            , decimal nTotalLancamento
        )
        {
            _idBanco = idBanco;
            _sBanco = sBanco;
            _sEmpresa = sEmpresa;
            _sDscCartao = sDscCartao;
            _idCartao = idCartao;
            _nTotalFatura = nTotalFatura;
            _lsCartoes = lsCartoes;
            _idFatura = idFatura;
            _mesAno = mesAno;
            _nTotalLancamento = nTotalLancamento;
        }

        #endregion

        #region | Membros Privados

        private int _idBanco;
        private string _sBanco;
        private string _sEmpresa;
        private string _sDscCartao;
        private int _idCartao;
        private decimal _nTotalFatura;
        private List<cls_FaturaCartao> _lsCartoes;
        private int _idFatura;
        private string _mesAno;
        private decimal _nTotalLancamento;

        public int idBanco { get => _idBanco; set => _idBanco = value; }
        public string sBanco { get => _sBanco; set => _sBanco = value; }
        public string sEmpresa { get => _sEmpresa; set => _sEmpresa = value; }
        public string sDscCartao { get => _sDscCartao; set => _sDscCartao = value; }
        public int idCartao { get => _idCartao; set => _idCartao = value; }
        public decimal nTotalFatura { get => _nTotalFatura; set => _nTotalFatura = value; }
        public List<cls_FaturaCartao> lsCartoes { get => _lsCartoes; set => _lsCartoes = value; }
        public int idFatura { get => _idFatura; set => _idFatura = value; }
        public string mesAno { get => _mesAno; set => _mesAno = value; }
        public decimal nTotalLancamento { get => _nTotalLancamento; set => _nTotalLancamento = value; }

        #endregion
    }

    [Serializable]
    public class cls_FaturaCartao
    {
        public int idFatura { get; set; }
        public int idCartao { get; set; }
        public decimal nTotalLancamento { get; set; }
        public string sDscCartao { get; set; }
        public List<cls_Detalhe_Fatura> lsLancamentos { get; set; }
    }

    [Serializable]
    public class cls_Detalhe_Fatura
    {

        #region | Construtor
        public cls_Detalhe_Fatura()
        {

        }

        public cls_Detalhe_Fatura
        (
            int idLancamento
            , string dtLancamento
            , string sDscLancamento
            , decimal nValor
            , string sDscUsuario
            , int idCartao
            , int idBanco
            , string sBanco
            , string sEmpresa
            , string sDscCartao
            , int idFatura
            , string mesAno
            , decimal nTotalLancamento
        )
        {
            _idLancamento = idLancamento;
            _dtLancamento = dtLancamento;
            _sDscLancamento = sDscLancamento;
            _nValor = nValor;
            _sDscUsuario = sDscUsuario;
            _idCartao = idCartao;
            _idBanco = idBanco;
            _sBanco = sBanco;
            _sEmpresa = sEmpresa;
            _sDscCartao = sDscCartao;
            _idFatura = idFatura;
            _mesAno = mesAno;
            _nTotalLancamento = nTotalLancamento;
        }

        #endregion

        #region | Membros Reservados

        private int _idLancamento;
        private string _dtLancamento;
        private string _sDscLancamento;
        private decimal _nValor;
        private string _sDscUsuario;
        private int _idCartao;
        private int _idBanco;
        private string _sBanco;
        private string _sEmpresa;
        private string _sDscCartao;
        private int _idFatura;
        private string _mesAno;
        private decimal _nTotalLancamento;

        public int idLancamento { get => _idLancamento; set => _idLancamento = value; }
        public string dtLancamento { get => _dtLancamento; set => _dtLancamento = value; }
        public string sDscLancamento { get => _sDscLancamento; set => _sDscLancamento = value; }
        public decimal nValor { get => _nValor; set => _nValor = value; }
        public string sDscUsuario { get => _sDscUsuario; set => _sDscUsuario = value; }
        public int idCartao { get => _idCartao; set => _idCartao = value; }
        public int idBanco { get => _idBanco; set => _idBanco = value; }
        public string sBanco { get => _sBanco; set => _sBanco = value; }
        public string sEmpresa { get => _sEmpresa; set => _sEmpresa = value; }
        public string sDscCartao { get => _sDscCartao; set => _sDscCartao = value; }
        public int idFatura { get => _idFatura; set => _idFatura = value; }
        public string mesAno { get => _mesAno; set => _mesAno = value; }
        public decimal nTotalLancamento { get => _nTotalLancamento; set => _nTotalLancamento = value; }

        #endregion
    }

    #endregion

    #region | Cartões

    [Serializable]
    public class cls__Cartao_Lancamento_Produtos
    {
        public int idLinha { get; set; }
        public int idLancamentoItem { get; set; }
        public string idProduto { get; set; }
        public string sCodProduto { get; set; }
        public string sDscProduto { get; set; }
        public int nQuantidade { get; set; }
        public string sUnidade { get; set; }
        public decimal nValorUnitario { get; set; }
        public decimal nValorTotal { get; set; }

    }

    #endregion

    #region | Impostos
    [Serializable]
    public class cls_Impostos
    {
        #region | Construtor 
        public cls_Impostos()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        //(@idCompra, @sCodigo, @sDscProduto, @sUnidade, @nQuantidade)
        public cls_Impostos
        (
             string ID
            , string cProd
            , string xProd
            , string NCM
            , string CFOP
            , string qCom
            , string uCom
            , string vProd
            , string cEAN
            , string vUnCom
            , string vDesc
            , string vTotTrib

            , string orig
            , string CST
            , string modBC
            , string vBC
            , string pRedBC
            , string pICMS
            , string vICMS
            , string modBCST
            , string pMVAST
            , string vBCSTICMS
            , string pICMSST
            , string vICMSST

            , string cEnq
            , string CSTIPI
            , string vBCIPI
            , string pIPI
            , string vIPI

            , string CSTPIS
            , string vBCPIS
            , string pPIS
            , string vPIS

            , string CSTCOFINS
            , string vBCCOFINS
            , string pCOFINS
            , string vCOFINS

            , string vBCUFDest
            , string pFCPUFDest
            , string pICMSUFDest
            , string pICMSInter
            , string pICMSInterPart
            , string vFCPUFDest
            , string vICMSUFDest
            , string vICMSUFRemet
            , string sCalculoDIFAL

            , string idCFOP
        )
        {
            _ID = ID;
            _cProd = cProd;
            _xProd = xProd;
            _NCM = NCM;
            _CFOP = CFOP;
            _qCom = qCom;
            _uCom = uCom;
            _vProd = vProd;
            _cEAN = cEAN;
            _vUnCom = vUnCom;
            _vDesc = vDesc;
            _vTotTrib = vTotTrib;

            _orig = orig;
            _CST = CST;
            _modBC = modBC;
            _vBC = vBC;
            _pRedBC = pRedBC;
            _pICMS = pICMS;
            _vICMS = vICMS;
            _modBCST = modBCST;
            _pMVAST = pMVAST;
            _vBCSTICMS = vBCSTICMS;
            _pICMSST = pICMSST;
            _vICMSST = vICMSST;

            _cEnq = cEnq;
            _CSTIPI = CSTIPI;
            _vBCIPI = vBCIPI;
            _pIPI = pIPI;
            _vIPI = vIPI;

            _CSTPIS = CSTPIS;
            _vBCPIS = vBCPIS;
            _pPIS = pPIS;
            _vPIS = vPIS;

            _CSTCOFINS = CSTCOFINS;
            _vBCCOFINS = vBCCOFINS;
            _pCOFINS = pCOFINS;
            _vCOFINS = vCOFINS;

            _vBCUFDest = vBCUFDest;
            _pFCPUFDest = pFCPUFDest;
            _pICMSUFDest = pICMSUFDest;
            _pICMSInter = pICMSInter;
            _pICMSInterPart = pICMSInterPart;
            _vFCPUFDest = vFCPUFDest;
            _vICMSUFDest = vICMSUFDest;
            _vICMSUFRemet = vICMSUFRemet;
            _sCalculoDIFAL = sCalculoDIFAL;

            _idCFOP = idCFOP;
        }

        #endregion

        #region | Membros Privados 
        private string _ID;
        private string _cProd;
        private string _xProd;
        private string _NCM;
        private string _CFOP;
        private string _qCom;
        private string _uCom;
        private string _vProd;
        private string _cEAN;
        private string _vUnCom;
        private string _vDesc;
        private string _vTotTrib;

        private string _orig;
        private string _CST;
        private string _modBC;
        private string _vBC;
        private string _pRedBC;
        private string _pICMS;
        private string _vICMS;
        private string _modBCST;
        private string _pMVAST;
        private string _vBCSTICMS;
        private string _pICMSST;
        private string _vICMSST;

        private string _cEnq;
        private string _CSTIPI;
        private string _vBCIPI;
        private string _pIPI;
        private string _vIPI;

        private string _CSTPIS;
        private string _vBCPIS;
        private string _pPIS;
        private string _vPIS;

        private string _CSTCOFINS;
        private string _vBCCOFINS;
        private string _pCOFINS;
        private string _vCOFINS; 

        private string _vBCUFDest; 
        private string _pFCPUFDest; 
        private string _pICMSUFDest; 
        private string _pICMSInter; 
        private string _pICMSInterPart; 
        private string _vFCPUFDest; 
        private string _vICMSUFDest; 
        private string _vICMSUFRemet; 
        private string _sCalculoDIFAL; 

        private string _idCFOP;
        #endregion

        #region | Propriedades
        public string ID { get => _ID; set => _ID = value; }
        public string cProd { get => _cProd; set => _cProd = value; }
        public string xProd { get => _xProd; set => _xProd = value; }
        public string NCM { get => _NCM; set => _NCM = value; }
        public string CFOP { get => _CFOP; set => _CFOP = value; }
        public string qCom { get => _qCom; set => _qCom = value; }
        public string uCom { get => _uCom; set => _uCom = value; }
        public string vProd { get => _vProd; set => _vProd = value; }
        public string cEAN { get => _cEAN; set => _cEAN = value; }
        public string vUnCom { get => _vUnCom; set => _vUnCom = value; }
        public string vDesc { get => _vDesc; set => _vDesc = value; }
        public string vTotTrib { get => _vTotTrib; set => _vTotTrib = value; }

        public string orig { get => _orig; set => _orig = value; }
        public string CST { get => _CST; set => _CST = value; }
        public string modBC { get => _modBC; set => _modBC = value; }
        public string vBC { get => _vBC; set => _vBC = value; }
        public string pRedBC { get => _pRedBC; set => _pRedBC = value; }
        public string pICMS { get => _pICMS; set => _pICMS = value; }
        public string vICMS { get => _vICMS; set => _vICMS = value; }
        public string modBCST { get => _modBCST; set => _modBCST = value; }
        public string pMVAST { get => _pMVAST; set => _pMVAST = value; }
        public string vBCSTICMS { get => _vBCSTICMS; set => _vBCSTICMS = value; }
        public string pICMSST { get => _pICMSST; set => _pICMSST = value; }
        public string vICMSST { get => _vICMSST; set => _vICMSST = value; }

        public string cEnq { get => _cEnq; set => _cEnq = value; }
        public string CSTIPI { get => _CSTIPI; set => _CSTIPI = value; }
        public string vBCIPI { get => _vBCIPI; set => _vBCIPI = value; }
        public string pIPI { get => _pIPI; set => _pIPI = value; }
        public string vIPI { get => _vIPI; set => _vIPI = value; }

        public string CSTPIS { get => _CSTPIS; set => _CSTPIS = value; }
        public string vBCPIS { get => _vBCPIS; set => _vBCPIS = value; }
        public string pPIS { get => _pPIS; set => _pPIS = value; }
        public string vPIS { get => _vPIS; set => _vPIS = value; }

        public string CSTCOFINS { get => _CSTCOFINS; set => _CSTCOFINS = value; }
        public string vBCCOFINS { get => _vBCCOFINS; set => _vBCCOFINS = value; }
        public string pCOFINS { get => _pCOFINS; set => _pCOFINS = value; }
        public string vCOFINS { get => _vCOFINS; set => _vCOFINS = value; }

        public string vBCUFDest { get => _vBCUFDest; set => _vBCUFDest = value; }
        public string pFCPUFDest { get => _pFCPUFDest; set => _pFCPUFDest = value; }
        public string pICMSUFDest { get => _pICMSUFDest; set => _pICMSUFDest = value; }
        public string pICMSInter { get => _pICMSInter; set => _pICMSInter = value; }
        public string pICMSInterPart { get => _pICMSInterPart; set => _pICMSInterPart = value; }
        public string vFCPUFDest { get => _vFCPUFDest; set => _vFCPUFDest = value; }
        public string vICMSUFDest { get => _vICMSUFDest; set => _vICMSUFDest = value; }
        public string vICMSUFRemet { get => _vICMSUFRemet; set => _vICMSUFRemet = value; }
        public string sCalculoDIFAL { get => _sCalculoDIFAL; set => _sCalculoDIFAL = value; }

        public string idCFOP { get => _idCFOP; set => _idCFOP = value; }
        #endregion
    }
    #endregion

    #region | Centro de Custo

    [Serializable]
    public class cls_CentroDeCusto_Lancamentos
    {
        public int id { get; set; }
        public string sReferencia { get; set; }
        public string sDscGeral { get; set; }
        public string sDscCategoria { get; set; }
        public string sTipo { get; set; }
        public string dtLancamento { get; set; }
        public decimal nSaldo { get; set; }
        public decimal nValor { get; set; }

    }

    [Serializable]
    public class cls_CentroDeCusto_Consulta
    {
        public int idCentroDeCusto { get; set; }
        public string sTipoCompleto { get; set; }
        public string sCodCC { get; set; }
        public string sDescricao { get; set; }
        public decimal nTetoGasto { get; set; }
        public decimal nSaldoGasto { get; set; }
        public string dtAtualizacao { get; set; }

    }

    #endregion

    #region | CST IBS/CBS

    [Serializable]
    public class cls_CST_IBS_CBS_Filho
    {
        public int idCST_Filho { get; set; }
        public int nCodigo { get; set; }
        public string sDescricao { get; set; }
        public int idTipoAliq { get; set; }
        public string sTipoAliq { get; set; }
        public decimal nRedIBS { get; set; }
        public decimal nRedCBS { get; set; }
        public bool bEditar { get; set; }
        public bool bExcluir { get; set; }

        public string sCodigo { get => nCodigo.ToString().PadLeft(6, '0'); }
    }

    #endregion

    #region | CST

    [Serializable]
    public class cls_CST
    {
        public int idCST { get; set; }
        public string sCST { get; set; }
        public string sDescricao { get; set; }
        public string sICMS { get; set; }
        public string sIPI { get; set; }
        public string sPIS { get; set; }
        public string sCOFINS { get; set; }
        public string sBENEF { get; set; }
        public string sExige_cBenef { get; set; }

        public static List<cls_CST> Popular()
        {
            List<cls_CST> Base_CST = new List<cls_CST>();
            //Popula a Classe CST
            SqlDataReader dr;
            dr = BD.ExecutarDataReader("sp_Manipula_tbl_Flow_Adm_CFOP 'Consulta_CFOP_CST', @sPesquisa=TODOS");

            if (dr != null)
            {
                Base_CST.Clear();


                Base_CST.Add(new cls_CST { idCST = 0, sDescricao = "Selecione CST ICMS", sICMS = "S", sIPI = "N", sPIS = "N", sCOFINS = "N", sBENEF = "N" });
                Base_CST.Add(new cls_CST { idCST = 0, sDescricao = "Selecione CST IPI", sICMS = "N", sIPI = "S", sPIS = "N", sCOFINS = "N", sBENEF = "N" });
                Base_CST.Add(new cls_CST { idCST = 0, sDescricao = "Selecione CST PIS", sICMS = "N", sIPI = "N", sPIS = "S", sCOFINS = "N", sBENEF = "N" });
                Base_CST.Add(new cls_CST { idCST = 0, sDescricao = "Selecione CST COFINS", sICMS = "N", sIPI = "N", sPIS = "N", sCOFINS = "S", sBENEF = "N" });
                Base_CST.Add(new cls_CST { idCST = 0, sDescricao = "Selecione Código do Benefício", sICMS = "N", sIPI = "N", sPIS = "N", sCOFINS = "S", sBENEF = "S" });

                while (dr.Read())
                {
                    cls_CST objItem = new cls_CST
                    {
                        idCST = Convert.ToInt32(dr["idCST"].ToString()),
                        sDescricao = dr["sCST"].ToString(),
                        sICMS = dr["sICMS"].ToString(),
                        sIPI = dr["sIPI"].ToString(),
                        sPIS = dr["sPIS"].ToString(),
                        sCOFINS = dr["sCOFINS"].ToString(),
                        sBENEF = dr["sBENEF"].ToString(),
                        sExige_cBenef = dr["sExige_cBenef"].ToString(),

                    };
                    Base_CST.Add(objItem);
                }
            }
            return Base_CST;
        }
    }

    #endregion
}