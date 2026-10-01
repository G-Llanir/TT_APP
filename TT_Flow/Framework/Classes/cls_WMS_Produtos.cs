using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using TT.FrameWork;
using IDENTITY = TT.FrameWork.Identity;
using System.Globalization;
using RETORNO = TT.FrameWork.BD.Retorno;
using TT_Flow.App.Paginas.RRHH;

namespace TT_Flow.FrameWork
{
    #region | Descrição Produto


    [Serializable]
    public class cls_Produtos_Descricao
    {

        #region | Construtor 
        public cls_Produtos_Descricao()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public cls_Produtos_Descricao
        (
             int idLinha
            , string sFuncao
            , int idRegistroDescricao
            , int idProduto
            , int idPais
            , string sDscPais
            , string sDscDescricao
            , int idTipo
            , string sDscTipo
            , string dtAtualizacao
            , int idUsuarioAtualizacao

        )
        {
            _idLinha = idLinha;
            _sFuncao = sFuncao;
            _idRegistroDescricao = idRegistroDescricao;
            _idProduto = idProduto;
            _idPais = idPais;
            _sDscPais = sDscPais;
            _sDscDescricao = sDscDescricao;
            _idTipo = idTipo;
            _sDscTipo = sDscTipo;
            _dtAtualizacao = dtAtualizacao;
            _idUsuarioAtualizacao = idUsuarioAtualizacao;


        }

        #endregion

        #region | Membros Privados 

        private int _idLinha;
        private string _sFuncao;

        private int _idRegistroDescricao;
        private int _idProduto;
        private int _idPais;
        private string _sDscPais;
        private string _sDscDescricao;
        private int _idTipo;
        private string _sDscTipo;
        private string _dtAtualizacao;
        private int _idUsuarioAtualizacao;




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

        public int idRegistroDescricao { get => _idRegistroDescricao; set => _idRegistroDescricao = value; }
        public int idProduto { get => _idProduto; set => _idProduto = value; }
        public int idPais { get => _idPais; set => _idPais = value; }
        public string sDscDescricao { get => _sDscDescricao; set => _sDscDescricao = value; }
        public int idTipo { get => _idTipo; set => _idTipo = value; }
        public string dtAtualizacao { get => _dtAtualizacao; set => _dtAtualizacao = value; }
        public int idUsuarioAtualizacao { get => _idUsuarioAtualizacao; set => _idUsuarioAtualizacao = value; }
        public string sDscPais { get => _sDscPais; set => _sDscPais = value; }
        public string sDscTipo { get => _sDscTipo; set => _sDscTipo = value; }




        #endregion


    }

    #endregion

    [Serializable]
    public class cls_WMS_Produtos
    {
        #region | Construtor
        public cls_WMS_Produtos() { }

        public cls_WMS_Produtos(

              int idItem
            , int idItemComposicao
            , string sCodigo
            , string sDscProduto
            , decimal nQuantidade
            , string sUnidade
            , string sExibePedido
            )
        {

        }
        public cls_WMS_Produtos(

            int idItem
          , int idItemComposicao
          , string sCodigo
          , string sDscProduto
          , decimal nQuantidade
          , string sUnidade
          , string sExibePedido
          , int nOrdem
          )
        {

        }
        public cls_WMS_Produtos(int idItem, string sCodigo, string sDscProduto, decimal nQuantidade
                                 , string sUnidade, decimal nEnviar, decimal qtdEnviado ){}

        #endregion

        #region | Membros Privados
        private int _idItem;
        private int _idItemComposicao;
        private string _sCodigo;
        private string _sCodigoPai;
        private string _sDscProduto;
        private string _sDscProdutoPai;
        private string _sUnidade;
        private decimal _nQuantidade;
        private string _tipoProduto;
        private decimal _nEstoqueMinimo;
        private decimal _nEstoqueAtual;
        private decimal _nEstoqueReservado;
        private string _dtAtualizacaoUsuario;
        private string _sCor;
        private decimal _nEstoqueAtualVisualizacao;
        private string _sFuncao;
        private string _sExibePedido;
        private int _nOrdem;
        private int _nOrdemPai;
        private string _sOrdem;
        private string _sOrdemPai;
        private int _IdGrupoProduto;
        private int _IdFamiliaProduto;
        private string _sDscGrupoProduto;
        private string _sDscFamiliaProduto;
        private decimal _nEnviar;
        private decimal _qtdEnviado;
        private int _idItemOPI;
        private int _idVolume;
        private int _idUnitizado;
        private int _idUnitizadoItem;
        private string _sCodigoBarras;
        private decimal _nTotalComposicao;
        private string _sIndustrializado;
        private string _sOrigem;
        private string _sNCM;
        private decimal _nPesoBruto;
        private decimal _nPesoLiquido;
        private decimal _nVolume;
        private string _sCST;
        private string _sCFOP;
        private int _idItemPai;
        private int _idItemAvo;
        private int _idItemBisavo;
        private int _idTipo;
        private bool _bProjeto;
        private string _sLink;
        private string _sLinkPai;
        private int _idPedido;
        private string _sDscLocalArmazenamento;
        private string _sFabricante;
        private decimal _nSugestaoCompra;

        #endregion

        #region | Propriedades
        public int IdItem { get => _idItem; set => _idItem = value; }
        public int IdItemComposicao { get => _idItemComposicao; set => _idItemComposicao = value; }
        public string SCodigo { get => _sCodigo; set => _sCodigo = value; }
        public string SDscProduto { get => _sDscProduto; set => _sDscProduto = value; }
        public string sCodigoPai { get => _sCodigoPai; set => _sCodigoPai = value; }
        public string sDscProdutoPai { get => _sDscProdutoPai; set => _sDscProdutoPai = value; }
        public string SUnidade { get => _sUnidade; set => _sUnidade = value; }
        public decimal NQuantidade { get => _nQuantidade; set => _nQuantidade = value; }
        public string TipoProduto { get => _tipoProduto; set => _tipoProduto = value; }
        public decimal NEstoqueMinimo { get => _nEstoqueMinimo; set => _nEstoqueMinimo = value; }
        public decimal NEstoqueAtual { get => _nEstoqueAtual; set => _nEstoqueAtual = value; }
        public decimal NEstoqueReservado { get => _nEstoqueReservado; set => _nEstoqueReservado = value; }
        public string DtAtualizacao { get => _dtAtualizacaoUsuario; set => _dtAtualizacaoUsuario = value; }
        public string SCor { get => _sCor; set => _sCor = value; }
        public string sExibePedido { get => _sExibePedido; set => _sExibePedido = value; }
        public decimal NEstoqueAtualVisualizacao { get => _nEstoqueAtualVisualizacao; set => _nEstoqueAtualVisualizacao = value; }
        public string SFuncao { get => _sFuncao; set => _sFuncao = value; }
        public string SDscVendaPT { get; set; }
        public string SDscVendasEN { get; set; }
        public string SDscVendaES { get; set; }
        public int nOrdem { get => _nOrdem; set => _nOrdem = value; }
        public int nOrdemPai { get => _nOrdemPai; set => _nOrdemPai = value; }
        public string sOrdem { get => _sOrdem; set => _sOrdem = value; }
        public string sOrdemPai { get => _sOrdemPai; set => _sOrdemPai = value; }
        public int IdGrupoProduto { get => _IdGrupoProduto; set => _IdGrupoProduto = value; }
        public int IdFamiliaProduto { get => _IdFamiliaProduto; set => _IdFamiliaProduto = value; }
        public int IdVolume { get => _idVolume; set => _idVolume = value; }
        public string sDscGrupoProduto { get => _sDscGrupoProduto; set => _sDscGrupoProduto = value; }
        public string sDscFamiliaProduto { get => _sDscFamiliaProduto; set => _sDscFamiliaProduto = value; }
        public decimal nEnviar { get => _nEnviar; set => _nEnviar = value; }
        public int IdUnitizado { get => _idUnitizado; set => _idUnitizado = value; }
        public decimal qtdEnviado { get => _qtdEnviado; set => _qtdEnviado = value; }
        public int IdItemOPI { get => _idItemOPI; set => _idItemOPI = value; }
        public int IdUnitizadoItem { get => _idUnitizadoItem; set => _idUnitizadoItem = value; }
        public string SCodigoBarras { get => _sCodigoBarras; set => _sCodigoBarras = value; }
        public decimal nTotalComposicao { get => _nTotalComposicao; set => _nTotalComposicao = value; }
        public string sIndustrializado { get => _sIndustrializado; set => _sIndustrializado = value; }
        public string sOrigem { get => _sOrigem; set => _sOrigem = value; }
        public string sNCM { get => _sNCM; set => _sNCM = value; }
        public decimal nPesoBruto { get => _nPesoBruto; set => _nPesoBruto = value; }
        public decimal nPesoLiquido { get => _nPesoLiquido; set => _nPesoLiquido = value; }
        public decimal nVolume { get => _nVolume; set => _nVolume = value; }
        public string sCST { get => _sCST; set => _sCST = value; }
        public string sCFOP { get => _sCFOP; set => _sCFOP = value; }
        public int idItemPai { get => _idItemPai; set => _idItemPai = value; }
        public int idItemAvo { get => _idItemAvo; set => _idItemAvo = value; }
        public int idItemBisavo { get => _idItemBisavo; set => _idItemBisavo = value; }
        public int idTipo { get => _idTipo; set => _idTipo = value; }
        public bool bProjeto { get => _bProjeto; set => _bProjeto = value; }
        public string sLink { get => _sLink; set => _sLink = value; }
        public string sLinkPai { get => _sLinkPai; set => _sLinkPai = value; }
        public string sControlaGarantiaLote { get; set; }
        public string SEfetiva { get; set; }
        public int idPedido { get => _idPedido; set => _idPedido = value; }
        public string sTipoEtiqueta { get; set; }
		public string sDscLocalArmazenamento { get => _sDscLocalArmazenamento; set => _sDscLocalArmazenamento = value; }
        public string sFabricante { get => _sFabricante; set => _sFabricante = value; }
        public decimal nSugestaoCompra { get => _nSugestaoCompra; set => _nSugestaoCompra = value; }

        #endregion

        #region | Funcoes

        public void ConverterTabela(DataSet ds, List<cls_WMS_Produtos> bs, string str)
        {
            try
            {
                bs.Clear();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    FrameWork.cls_WMS_Produtos objItem = new FrameWork.cls_WMS_Produtos();
                    objItem.IdItem = Convert.ToInt32(row["idItem"].ToString());
                    objItem.SCodigo = row["sCodigo"].ToString();
                    try { objItem.TipoProduto = row["sDscTipoProduto"].ToString(); } catch { objItem.TipoProduto = ""; }
                    try { objItem.NEstoqueReservado = Convert.ToDecimal(row["nEstoqueReservado"].ToString()); } catch { objItem.NEstoqueReservado = 0; }
                    try { objItem.NEstoqueMinimo = Convert.ToDecimal(row["nEstoqueMinimo"].ToString()); } catch { objItem.NEstoqueMinimo = 0; }
                    try { objItem.SCor = row["sCor"].ToString(); } catch { objItem.SCor = ""; }
                    try { objItem.SDscProduto = row["sDscProduto"].ToString(); } catch { objItem.SDscProduto = ""; }

                    objItem.SUnidade = row["sUnidade"].ToString();
                    objItem.NEstoqueAtual = Convert.ToDecimal(row["nEstoqueAtual"].ToString());
                    objItem.DtAtualizacao = row["dtAtualizacao"].ToString();
                    objItem.SFuncao = "CONSULTA";
                    objItem.sDscFamiliaProduto = row["sDscFamiliaProduto"].ToString();
                    objItem.sFabricante = row["sFabricante"].ToString();
                    objItem.sDscGrupoProduto = row["sDscGrupoProduto"].ToString();
                    objItem.sDscLocalArmazenamento = row["sDscLocalArmazenamento"].ToString();
                    objItem.nSugestaoCompra = Convert.ToDecimal(row["nSugestaoCompra"].ToString());

                    if (str == "S" || str == "H")
                    {
                        bs.Add(objItem);
                    }
                    else
                    {
                        objItem.NEstoqueAtualVisualizacao = Convert.ToDecimal(row["nEstoqueAtual"].ToString());
                        bs.Add(objItem);

                    }

                }
            }
            catch
            (Exception ex)
            { throw ex; }

        }
        public static decimal ConverterStringDecimal(string dado)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("pt-BR");
            if (decimal.TryParse(dado, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal converter))
            {
                return converter;
            }
            return decimal.Zero;
        }
        public void ConverterCamposClasse(cls_WMS_Produtos produto)
        {
            DataSet dsEditar = new DataSet();
            Dictionary<string, string> vParametrosItens = new Dictionary<string, string>();

            vParametrosItens.Add("@sFuncao", "CONSULTAR-INDICES");
            vParametrosItens.Add("@idTipoProduto", produto.TipoProduto.ToString());
            vParametrosItens.Add("@sExibePedido", produto.sExibePedido.ToString());
            dsEditar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_WMS_Composicao", vParametrosItens);


            string sExibePedidoBanco = RETORNO.DATASET(dsEditar, "sExibePedido");
            string tipoProdutoBanco = RETORNO.DATASET(dsEditar, "TipoProduto");

            // Atualizar apenas os campos necessários no objeto da classe
            produto.sExibePedido = sExibePedidoBanco;
            produto.TipoProduto = tipoProdutoBanco;

        }
        /// <summary>
        /// Remove caracteres especiais e substitui ponto por vírgula
        /// </summary>
        /// <param name="sObjeto"></param>
        /// <returns></returns>
        public static string removeCaracteres(string sObjeto)
        {
            try
            {
                string sObjetoSemCaracter = new string(sObjeto
                    .Where(c => Char.IsDigit(c) || c == ',' || c == '.')
                    .Select(c => c == ',' ? '.' : c)
                    .ToArray());

                if (sObjetoSemCaracter == "")
                {
                    sObjetoSemCaracter = "0";
                }
                return sObjetoSemCaracter;

            }
            catch
            {
                return "0";
            };
        }

        public static bool SalvarDados(List<cls_WMS_Produtos> tblAtual)
        {
            string vProcedure = "sp_Manipula_tbl_Flow_Produtos";
            string sFuncao = "INCLUIR_SALDO_ATUALIZADO";
            string sErro = "";

            DateTime dtAtual = DateTime.Now;
            string dtAtualAjustada = dtAtual.ToString("dd/MM/yyyy HH:mm:ss");

            cls_WMS_MovimentacaoRel _MV = new cls_WMS_MovimentacaoRel()
            {
                IdMotivo = 0,
                IdTipoMovimentacao = 1,
                SNotaFiscal = "",
                SObservacao = "Atualização via página de consulta de saldo",
                IdParceiro = 0,
                IDusuarioAtualizacao = int.Parse(IDENTITY.Variaveis.idUsuario()),
                DtMovimentacao = DateTime.Now,
                SEfetiva = "T",
                
            };
            int idMovimentacao = cls_WMS_MovimentacaoRel.Movimentacao_Estoque(_MV);
            _MV.IdMovimentacao = idMovimentacao;

            if (_MV.IdMovimentacao == 0)
            {
                throw new Exception("Erro ao adquirir idMovimentação");
            }
            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", sFuncao);
                vParametros.Add("@idItem", "");
                vParametros.Add("@nEstoqueAtual", "");
                vParametros.Add("@sSituacao", "S");
                vParametros.Add("@idMovimentacao", idMovimentacao.ToString());
                DataSet ds = new DataSet();

                foreach (cls_WMS_Produtos tblComercialAtual in tblAtual)
                {
                    if (tblComercialAtual is cls_WMS_Produtos)
                    {

                        if (tblComercialAtual.SFuncao == sFuncao)
                        {
                            vParametros["@idItem"] = tblComercialAtual.IdItem.ToString();
                            vParametros["@nEstoqueAtual"] = removeCaracteres((tblComercialAtual.NEstoqueAtual.ToString()));
                            vParametros["@idUsuarioAtualizacao"] = IDENTITY.Variaveis.idUsuario();
                            ds = BD.ExecutarDataSet(vProcedure, vParametros);
                            tblComercialAtual.SFuncao = "CONSULTA";
                            string sErroItens = "";
                            if (BD.ValidarDataSet(ds, out sErroItens))
                            {
                                if (sErroItens != "")
                                {
                                    throw new Exception("Erro: ao salvar os itens");
                                }
                                else
                                {
                                    _MV.IdProduto = tblComercialAtual.IdItem;
                                }
                            }

                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro: " + ex.Message);
            }

            return false;
        }

        public static DataSet Consultar_Impostos(string idItem)
        {
            DataSet ds = new DataSet();
            try
            {
                string vProcedure = "sp_Manipula_tbl_Flow_Produtos";

                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                vParametros.Add("@idItem", idItem);
                ds = BD.ExecutarDataSet(vProcedure, vParametros);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
        public static decimal Imposto_Item(string idItem)
        {
            DataSet ds = Consultar_Impostos(idItem);
            decimal ts = ConverterStringDecimal(removeCaracteres(RETORNO.DATASET(ds, 0, "nImpostos")));
            return ts;
        }


        #endregion

    }

    [Serializable]
    public class cls_WMS_CEST
    {
        #region | Construtor
        public cls_WMS_CEST() { }

        public cls_WMS_CEST(

              int idCest
            , string sCodigoCEST
            , string sDscCEST

            )
        {

        }
        #endregion

        #region | Membros Privados
        private int _idCest;
        private string _sCodigoCEST;
        private string _sDscCEST;



        public int idCest { get => _idCest; set => _idCest = value; }
        public string sCodigoCEST { get => _sCodigoCEST; set => _sCodigoCEST = value; }
        public string sDscCEST { get => _sDscCEST; set => _sDscCEST = value; }

        #endregion

        #region | Propriedades

        #endregion

        #region | Funcoes



        #endregion

    }

    [Serializable]
    public class cls_WMS_NCM
    {
        #region | Construtor
        public cls_WMS_NCM() { }


        #endregion

        #region | Membros Privados
        private int _idNCM;
        private int _idCEST;
        private string _sCodigoNCM;
        private string _sDscNCM;



        public int idNCM { get => _idNCM; set => _idNCM = value; }
        public int idCEST { get => _idCEST; set => _idCEST = value; }
        public string sCodigoNCM { get => _sCodigoNCM; set => _sCodigoNCM = value; }
        public string sDscNCM { get => _sDscNCM; set => _sDscNCM = value; }

        #endregion

        #region | Propriedades

        #endregion

        #region | Funcoes



        #endregion

    }

    [Serializable]
    public class cls_WMS_Familias
    {
        #region | Construtor
        public cls_WMS_Familias()
        {

        }
        public cls_WMS_Familias
        (
             int idFamilia
            , int idContador
            , int idRegistro
            , int idParceiro
            , string sDscFamilia
            , string sFuncao
        )
        {
            _idFamilia = idFamilia;
            _idContador = idContador;
            _sDscFamilia = sDscFamilia;
            _sFuncao = sFuncao;
            _idRegistro = idRegistro;
            _idParceiro = idParceiro;
        }
        #endregion

        #region | Propriedades
        private int _idFamilia;
        private int _idContador;
        private int _idRegistro;
        private int _idParceiro;
        private string _sDscFamilia;
        private string _sFuncao;

        public int idFamilia { get => _idFamilia; set => _idFamilia = value; }
        public int idContador { get => _idContador; set => _idContador = value; }
        public int idRegistro { get => _idRegistro; set => _idRegistro = value; }
        public int idParceiro { get => _idParceiro; set => _idParceiro = value; }
        public string sDscFamilia { get => _sDscFamilia; set => _sDscFamilia = value; }
        public string sFuncao { get => _sFuncao; set => _sFuncao = value; }
        #endregion
    }

    [Serializable]
    public class cls_WMS_ConsultaSaldo : cls_WMS_Produtos
    {
        public cls_WMS_ConsultaSaldo()
        {

        }


    }
    [Serializable]
    public class cls_CotacaoComprasItens : cls_WMS_Produtos
    {
        public cls_CotacaoComprasItens()
        {

        }
        public int IdProduto { get; set; }
        public int IdParceiro { get; set; }
        public int IdFornecedor { get; set; }
        public int IdCotacao { get; set; }
        public decimal NValorCotado { get; set; }
        public DateTime DtPrevisao { get; set; }
        public string SObservacao { get; set; }
        public decimal NIPI { get; set; }
    }

    public interface IMovimentacao
    {
        int IdMotivo { get; set; }
        int IdTipoMovimentacao { get; set; }
        string SDscTipoMovimentacao { get; set; }
        string SDscMotivo { get; set; }
    }

    [Serializable]
    public class cls_WMS_MovimentacaoRel : cls_WMS_Produtos, IMovimentacao
    {
        static string sProcedure_Consultar_Detalhe = "CONSULTAR_DETALHE";
        static string sProcedure_Consultar = "CONSULTAR_MOVIMENTACAO_PRODUTO";
        static string sProcedure = "sp_Manipula_tbl_Flow_Produtos_Movimentacao";


        #region | Membros Privados
        private int _idLinha;
        private int _idMovimentacao;
        private int _idTipoMovimentacao;
        private int _idLocalArmazenamento;
        private int _idMotivo;
        private int _idParceiro;
        private string _sNotaFiscal;
        private string _sObservacao;
        private int _idUsuario;
        private int _idRegistro;
        private decimal _nQuantidadeMovimentacao;
        private decimal _nValorUnitario;
        private decimal _nPercIPI;
        private decimal _nValorTotal;

        private DateTime _dtMovimentacao;
        private string _sCodigoComDescricao;
        private int _idProduto;
        private string _sDscMotivo;
        private string _sDscTipoMovimentacao;

        #endregion

        #region | Propriedades

        public int idLinha { get => _idLinha; set => _idLinha = value; }
        public int IdMovimentacao { get => _idMovimentacao; set => _idMovimentacao = value; }
        public int IdTipoMovimentacao { get => _idTipoMovimentacao; set => _idTipoMovimentacao = value; }
        public int IdLocalArmazenamento { get => _idLocalArmazenamento; set => _idLocalArmazenamento = value; }
        public DateTime DtMovimentacao { get => _dtMovimentacao; set => _dtMovimentacao = value; }
        public int IdMotivo { get => _idMotivo; set => _idMotivo = value; }
        public decimal nValorUnitario { get => _nValorUnitario; set => _nValorUnitario = value; }
        public decimal nPercIPI { get => _nPercIPI; set => _nPercIPI = value; }
        public decimal nValorTotal { get => _nValorTotal; set => _nValorTotal = value; }

        public string SCodigoComDescricao { get => _sCodigoComDescricao; set => _sCodigoComDescricao = value; }
        public int IdParceiro { get => _idParceiro; set => _idParceiro = value; }
        public string SNotaFiscal { get => _sNotaFiscal; set => _sNotaFiscal = value; }
        public string SObservacao { get => _sObservacao; set => _sObservacao = value; }
        public int IdProduto { get => _idProduto; set => _idProduto = value; }
        public string SDscMotivo { get => _sDscMotivo; set => _sDscMotivo = value; }
        public string SDscTipoMovimentacao { get => _sDscTipoMovimentacao; set => _sDscTipoMovimentacao = value; }
        public int IDusuarioAtualizacao { get; set; }
        public string NSerie { get; set; }
        public string SLote { get; set; }
        public string SEntrada { get; set; }
        public string SGarantia { get; set; }
        public string SSaida { get; set; }
        public string STipoMov { get; set; }
        public int IdRegistro { get; set; }
        public int NQtdPreparados { get; set; }
        public int NQtdTotal { get; set; }
        public int IdUsuario { get; set; }
        public string SControlaGarantiaLote { get; set; }
        public int IdPosicao { get; set; }
        public string SConfirmado { get; set; }
        public string SSerializavel { get; set; }
        public int IdLocalDestino { get; set; }
        public int IdPosicaoDestino { get; set; }
		#endregion

        #region | Funcoes estaticas
        public static DataSet Movimentacao_ConsultarProduto(string idMovimentacao)
        {
            string sErro = "";
            DataSet dsPesquisa;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros["@idMovimentacao"] = idMovimentacao;
            dsPesquisa = BD.ExecutarDataSet(sProcedure_Consultar, vParametros);
            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                if (sErro != "")
                    throw new Exception("Erro ao salvar " + sErro);
            }
            return dsPesquisa;

        }

        public static DataSet Movimentacao_ConsultarDetalhe(string idPesquisa)
        {

            try
            {
                string sErro = "";
                DataSet dsPesquisa;
                string sSql = "sp_Manipula_tbl_Flow_Produtos_Movimentacao";
                Dictionary<String, String> vParametros = new Dictionary<string, string>();

                vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                vParametros.Add("@idMovimentacao", idPesquisa);
                dsPesquisa = BD.ExecutarDataSet(sSql, vParametros);

                return dsPesquisa;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            #endregion

        }
 
       public static void Movimentacao_ConverterDS_BD<T>(DataSet ds, List<T> lista) where T : IMovimentacao
        {
            lista.Clear();
            foreach (DataTable dt in ds.Tables)
            {
                foreach (DataRow row in dt.Rows)
                {
                    try
                    {
                        if (typeof(T) == typeof(cls_WMS_MovimentacaoRel))
                        {
                            var tipoMovimentacao = row["sTipoMov"].ToString().Trim().ToUpper() ?? "";
                            cls_WMS_MovimentacaoRel item = new cls_WMS_MovimentacaoRel();
                            item.IdMotivo = Convert.ToInt32(row["idMotivo"]);
                            item.IdTipoMovimentacao = Convert.ToInt32(row["idTipoMovimentacao"]);
                            item.IdMovimentacao = Convert.ToInt32(row["idMovimentacao"]);
                            item.DtMovimentacao = DateTime.Parse(row["dtMovimentacao"].ToString());
                            item.IdParceiro = Convert.ToInt32(row["idParceiro"]);
                            item.SNotaFiscal = row["sNotaFiscal"].ToString();
                            item.IdProduto = string.IsNullOrEmpty(row["idProduto"].ToString()) ? 0 : Convert.ToInt32(row["idProduto"].ToString());
                            item.IdLocalArmazenamento = Convert.ToInt32(row["idLocalArmazenamento"]);
                            item.IdPosicao = string.IsNullOrEmpty(row["idPosicao"].ToString()) ? 0 : Convert.ToInt32(row["idPosicao"].ToString());
                            // ---Convert.ToDecimal direto no objeto do banco ---
                            item.NQuantidade = row["nQuantidadeMovimentacao"] != DBNull.Value ? Convert.ToDecimal(row["nQuantidadeMovimentacao"]) : 0;
                            item.nValorUnitario = row["nValorUnitario"] != DBNull.Value ? Convert.ToDecimal(row["nValorUnitario"]) : 0;
                            item.nPercIPI = row["nPercIPI"] != DBNull.Value ? Convert.ToDecimal(row["nPercIPI"]) : 0;
                            item.nValorTotal = row["nTotal"] != DBNull.Value ? Convert.ToDecimal(row["nTotal"]) : 0;
                            item.SCodigo = row["sCodigo"].ToString();
                            item.SDscProduto = row["sDscProduto"].ToString();
                            item.TipoProduto = row["sDscTipoProduto"].ToString();
                            item.SCodigoComDescricao = item.SCodigo + " - " + item.SDscProduto;
                            item.STipoMov = tipoMovimentacao == "S" ? "Saída" : tipoMovimentacao == "E" ? "Entrada" : "N/A";
                            item.SLote = row["sLote"].ToString();
                            item.SGarantia = row["sControlaGarantiaLote"].ToString();
                            item.SSerializavel = row["sSerializavel"].ToString();
                            item.SUnidade = row["sUnidade"].ToString();
                            item.IdLocalDestino = Convert.ToInt32(row["idLocalDestino"]);
                            item.IdPosicaoDestino = Convert.ToInt32(row["idPosicaoDestino"]);
                            lista.Add((T)(object)item);
                        }
                        else if (typeof(T) == typeof(cls_WMS_Movimentacao_Motivo))
                        {
                            cls_WMS_Movimentacao_Motivo item_motivo = new cls_WMS_Movimentacao_Motivo();
                            item_motivo.IdMotivo = Convert.ToInt32(row["idMotivo"]);
                            item_motivo.IdTipoMovimentacao = Convert.ToInt32(row["idTipoMovimentacao"]);
                            item_motivo.SDscMotivo = row["sDscMotivo"].ToString();
                            item_motivo.SDscTipoMovimentacao = row["sDscTipoMovimentacao"].ToString();
                            lista.Add((T)(object)item_motivo);
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }
        public static DataSet Movimentacao_Consultar_Motivo_x_Tipo()
        {
            string sErro = "";
            DataSet dsPesquisa;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "MOVIMENTACAO_MOTIVO");
            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                if (sErro != "")
                    throw new Exception("Erro ao salvar " + sErro);
            }
            return dsPesquisa;
        }
        /// <summary>
        /// Fornecer obrigatoriamente idTipoMovimentacao, 
        /// </summary>
        /// <param name="BS_MOVIMENTACAO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static int Movimentacao_Estoque(cls_WMS_MovimentacaoRel BS_MOVIMENTACAO)
        {
            string vProcedure_MovimentarEstoque = "sp_Manipula_tbl_Flow_Produtos_Movimentacao";
            DataSet ds;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFUncao", "MOVIMENTAR_ESTOQUE");
            vParametros.Add("@dtMovimentacao", BS_MOVIMENTACAO.DtMovimentacao.ToString());
            vParametros.Add("@idTipoMovimentacao", BS_MOVIMENTACAO.IdTipoMovimentacao.ToString());
            vParametros.Add("@idMotivo", BS_MOVIMENTACAO.IdMotivo.ToString());
            vParametros.Add("@idParceiro", BS_MOVIMENTACAO.IdParceiro.ToString());
            vParametros.Add("@sNotaFiscal", BS_MOVIMENTACAO.SNotaFiscal.ToString());
            vParametros.Add("@sObservacao", BS_MOVIMENTACAO.SObservacao);
            vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());
            if (BS_MOVIMENTACAO.SEfetiva == "S")
            {
                vParametros.Add("@sEfetivaEstoque", BS_MOVIMENTACAO.SEfetiva);
            }
            ds = BD.ExecutarDataSet(vProcedure_MovimentarEstoque, vParametros);
            string sErro = "";
            if (BD.ValidarDataSet(ds, out sErro))
            {
                if (sErro != "")
                {
                    throw new Exception("Erro ao salvar " + sErro);
                }
                else
                    try
                    {
                        return int.Parse(RETORNO.DATASET(ds, 0, "idMovimentacao"));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Erro ao obter o idMovimentacao " + sErro);
                    }
            }
            return 0;
        }

        public static bool Movimentacao_SalvarItens(string idMovimentacao, List<cls_WMS_MovimentacaoRel> lista, string sEfetivaEstoque, string idImpressora)
        {
            try
            {

                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "MOVIMENTAR_ESTOQUE_ITENS");
                vParametros.Add("@idMovimentacao", idMovimentacao);
                vParametros.Add("@sEfetivaEstoque", sEfetivaEstoque);
                vParametros.Add("@idProduto", "");
                vParametros.Add("@idLocalArmazenamento", "");
                vParametros.Add("@idLocalizacao", "");
                vParametros.Add("@nQtdMovimentacao", "");
                vParametros.Add("@nValorUnitario", "");
                vParametros.Add("@nPercIPI", "");
                vParametros.Add("@sUnidade", "");
                vParametros.Add("@nSerie", "");
                vParametros.Add("@sLote", "");
                vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());

                lista.ForEach(item =>
                {
                    DataSet dsItens;

                    vParametros["@idProduto"] = item.IdProduto.ToString();
                    vParametros["@idLocalArmazenamento"] = "1";
                    vParametros["@idLocalizacao"] = "0";
                    vParametros["@nValorUnitario"] = BD.Conversoes.Numerico(item.nValorUnitario);
                    vParametros["@nQtdMovimentacao"] = BD.Conversoes.Numerico(item.NQuantidade);
                    vParametros["@nPercIPI"] = BD.Conversoes.Numerico(item.nPercIPI);
                    vParametros["@sUnidade"] = item.SUnidade;
                    vParametros["@nSerie"] = item.NSerie?.ToString() ?? "";
                    vParametros["@sLote"] = item.SLote ?? "";
                    vParametros["@sCodigoBarras"] = $"PR{item.IdProduto}";
                    vParametros["@idImpressora"] = idImpressora;
                    vParametros["@idPedido"] = item.idPedido.ToString();
                    dsItens = BD.ExecutarDataSet(sProcedure, vParametros);

                });

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro: " + ex.Message);
            }

        }
    }

    [Serializable]
    public class cls_WMS_Movimentacao_Motivo : IMovimentacao
    {

        public int IdMotivo { get; set; }
        public int IdTipoMovimentacao { get; set; }
        public string SDscTipoMovimentacao { get; set; }
        public string SDscMotivo { get; set; }


        public static void Movimentacao_Motivo()
        {
            string sErro = "";
            DataSet ds;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "MOVIMENTACAO_MOTIVO");
            ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);
            if (BD.ValidarDataSet(ds))
            {

            }
        }
    }

    [Serializable]
    public class cls_Cotacao : IMovimentacao
    {
        public int IdCotacao { get; set; }
        public int IdMotivo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int IdTipoMovimentacao { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string SDscTipoMovimentacao { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string SDscMotivo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        public static void Cotacao_Consulta()
        {
            string sErro = "";
            DataSet ds;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "COTACAO");
            ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos_Movimentacao", vParametros);

        }
    }
    [Serializable]
    public class cls_WMS_Produtos_Sugestao : cls_WMS_Produtos
    {
        public int IdSugestao { get; set; }
        public int IdProduto { get; set; }
        public int IdProdutoSugestao { get; set; }


        public static void Consultar_Sugestao(string idProduto, List<cls_WMS_Produtos_Sugestao> BS_SUGESTAO)
        {
            string vProcedure = "sp_Manipula_tbl_Flow_Produtos";
            string sErro = "";
            DataSet dsConsulta;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTA_SUGESTAO_ITENS");
            vParametros.Add("idProduto", idProduto);

            dsConsulta = BD.ExecutarDataSet(vProcedure, vParametros);

            if (BD.ValidarDataSet(dsConsulta, out sErro))
            {
                if (sErro == "")
                {
                    cls_WMS_Produtos_Sugestao BS_PRODUTO_ITEM = new cls_WMS_Produtos_Sugestao();
                    foreach (DataRow row in dsConsulta.Tables[0].Rows)
                    {
                        BS_PRODUTO_ITEM.IdSugestao = Convert.ToInt32(row["idSugestao"].ToString());
                        BS_PRODUTO_ITEM.IdProduto = Convert.ToInt32(row["idProduto"].ToString());
                    }
                }
                else
                {
                    throw new Exception(sErro);
                }
            }
        }

        public static void IncluirItens_Sugestao(string idProduto, List<cls_WMS_Produtos_Sugestao> BS_SUGESTAO)
        {

            string sProcedure = "sp_Manipula_tbl_Flow_Produtos";
            string sErro = "";
            DataSet dsConsulta;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "SUGESTAO_DELETAR");
            vParametros.Add("idProduto", idProduto);

            dsConsulta = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsConsulta, out sErro))
            {
                if (sErro == "")
                {
                    vParametros["sFuncao"] = "SUGESTAO_INCLUIR_ITENS";

                    BS_SUGESTAO.ForEach(item =>
                    {
                        vParametros["@idProduto"] = item.IdProduto.ToString();
                        vParametros["@idProdutoSugestao"] = item.IdProdutoSugestao.ToString();
                        vParametros["@sUnidade"] = item.SUnidade;
                        vParametros["@sQuantidade"] = item.NQuantidade.ToString();
                        dsConsulta = BD.ExecutarDataSet(sProcedure, vParametros);
                    });
                }
            }
        }
    }

    public class cls_WMS_Volume
    {
        #region | Construtor
        public cls_WMS_Volume() { }

        #endregion

        #region | Membros Privados
        private int _idVolume;
        private int _nVolume;
        private int _idEnvioOPI;
        private int _idProdutoEmbalagem;

        #endregion

        #region | Propriedades
        public int IdVolume { get => _idVolume; set => _idVolume = value; }
        public int NVolume { get => _nVolume; set => _nVolume = value; }
        public int IdEnvioOPI { get => _idEnvioOPI; set => _idEnvioOPI = value; }
        public int IdProdutoEmbalagem { get => _idProdutoEmbalagem; set => _idProdutoEmbalagem = value; }
        #endregion


    }

    [Serializable]
    public class cls_WMS_Produtos_Fornecedores
    {
        public int IdParceiro { get; set; }
        public int IdProduto { get; set; }
        public int IdTipoFornecedor { get; set; }
        public decimal NvalorUnitario { get; set; }
        public string SCodigoFornecedor { get; set; }
        public string SDscFornecedorProduto { get; set; }
        public string sCNPJ_CPF { get; set; }
        public string SRazaoSocial { get; set; }
        public string STipoFornecedor { get; set; }
    }

    [Serializable]      //Agnes Partal * 07/08/2024
    public class cls_WMS_Produtos_Documentacao
    {
        public int idLinha { get; set; }
        public string idDocumentacao { get; set; }
        public int idProduto { get; set; }
        public string sDscTipo { get; set; }
        public string sDscIdioma { get; set; }
        public string sDscEndereco { get; set; }
        public string sListarOrcamento { get; set; }
    }

    [Serializable]       //Agnes Partal * 04/02/2025
    public class cls_WMS_Fabricacao_Produto
    {
        public int idProduto { get; set; }
        public string sCodigo { get; set; }
        public string sDescricao { get; set; }
        public decimal nQtd { get; set; }
        public int idFabricacaoProduto { get; set; }
        public string sUnidade { get; set; }
        public int nOrdem { get; set; }
        public string sTipo { get; set; }
        public int idTipo { get; set; }
        public decimal nValorUnitario { get; set; }
        public decimal nTotal { get; set; }
        public int idTabela { get; set; }
    }

    [Serializable]
    public class cls_WMS_Fabricacao_Produto_Tipo
    {
        public int idTipo { get; set; }
        public string sTipo { get; set; }
        public decimal nTotalTipo { get; set; }
        public List<cls_WMS_Fabricacao_Produto> ls_FabricacaoProduto { get; set; } = new List<cls_WMS_Fabricacao_Produto>();
    }

    [Serializable]      //Agnes Partal * 04/02/2025     
    public class cls_WMS_Fabricacao_Recurso
    {
        public int idProduto { get; set; }
        public string sCodigo { get; set; }
        public string sDescricao { get; set; }
        public decimal nQtd { get; set; }
        public int idFabricacaoRecurso { get; set; }
        public string sUnidade { get; set; }
        public int nOrdem { get; set; }
        public string sTipo { get; set; }
        public int idTipo { get; set; }
        public decimal nValorUnitario { get; set; }
        public decimal nTotal { get; set; }
        public int idTabela { get; set; }

    }

    [Serializable]      //Agnes Partal * 04/02/2025
    public class cls_WMS_Fabricacao_Processo
    {
        public int idLinha { get; set; }
        public int idProduto { get; set; }
        public int nOrdem { get; set; }
        public string sDescricao { get; set; }
        public int idFabricacaoProcesso { get; set; }
        public int idRecurso { get; set; }

    }

    [Serializable]       //Agnes Partal * 25/03/2026
    public class cls_WMS_Fabricacao_InsumoFerramenta
    {
        public int idProduto { get; set; }
        public string sCodigo { get; set; }
        public string sDescricao { get; set; }
        public decimal nQtd { get; set; }
        public int idFabricacaoInsumoFerramenta { get; set; }
        public string sUnidade { get; set; }
        public int nOrdem { get; set; }
        public string sTipo { get; set; }
        public int idTipo { get; set; }
        public decimal nValorUnitario { get; set; }
        public decimal nTotal { get; set; }
        public int idTabela { get; set; }

    }

    [Serializable]
    public class cls_Produtos_Aba_Tabelas
    {
        public int idTabela { get; set; }
        public int idTipoTabela { get; set; }
        public int idMoedaOrigem { get; set; }
        public string sDscTabela { get; set; }
        public string sObservacao { get; set; }
        public string sDscTipoTabela { get; set; }
        public string sSimbolo_MoedaOrigem { get; set; }
        public string sSimbolo_MoedaDestino { get; set; }
        public decimal nCambio { get; set; }
        public decimal nPreco { get; set; }
        public decimal nPreco_Zona_SD { get; set; }
        public decimal nPreco_Zona_ND { get; set; }
        public decimal nPreco_Zona_N { get; set; }
        public decimal nPreco_Zona_CO { get; set; }
        public decimal nPreco_Zona_S { get; set; }
        public decimal nFator { get; set; }
        public decimal nMargem { get; set; }
        public decimal nTaxaEnvio { get; set; }
        public decimal nTaxaLocal { get; set; }
        public decimal nTaxaImpostos { get; set; }
        public decimal nIPI { get; set; }
        public decimal nTotal { get; set; }
        public bool bProdutoIncluso { get; set; }
        public bool bRecurso { get; set; }
        public bool bIndustrializado { get; set; }
        public bool bLiberado { get; set; }
    }
}