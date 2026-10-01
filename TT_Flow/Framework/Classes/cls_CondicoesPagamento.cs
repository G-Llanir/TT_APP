using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static TT.FrameWork.BD;
using TT.FrameWork;
using System.Runtime.InteropServices;

namespace TT_Flow.FrameWork
{

    #region | Condiçoes de Pagamento
    [Serializable]
    public class cls_CondicoesPagamento
    {

        #region | Construtor 
        public cls_CondicoesPagamento()
        {
            //
            // TODO: Add constructor logic here
            //
        }
       
        public cls_CondicoesPagamento
        (
             int idRegistroCondicaoPagamento
            , int idCondicaoPagamento
            , string sDscCondicaoPagamento
            , int nQtdParcelas
            , decimal nPorcentagemValor
            , int idTipoCondicaoPagamento
            , int nDDL

            , int idLinha
            , string sFuncao
            , int idTipo
            , int idParceiro
            , int idContador
            , int idRegistro

        )
        {
            _idRegistroCondicaoPagamento = idRegistroCondicaoPagamento;
            _idCondicaoPagamento = idCondicaoPagamento;
            _sDscCondicaoPagamento = sDscCondicaoPagamento;
            _nQtdParcelas = nQtdParcelas;
            _nPorcentagemValor = nPorcentagemValor;
            _idTipoCondicaoPagamento = idTipoCondicaoPagamento;
            _nDDL = nDDL;

            _idLinha = idLinha;
            _sFuncao = sFuncao;
            _idTipo = idTipo;
            _idParceiro = idParceiro;
            _idContador = idContador;
            _idRegistro = idRegistro;

        }

        #endregion

        #region | Membros Privados 

        private int _idRegistroCondicaoPagamento;
        private int _idCondicaoPagamento;
        private string _sDscCondicaoPagamento;
        private int _nQtdParcelas;
        private decimal _nPorcentagemValor;
        private int _idTipoCondicaoPagamento;
        private int _nDDL;
        private string _sFuncao;
        private int _idLinha;
        private int _idTipo;
        private int _idParceiro;
        private int _idContador;
        private int _idRegistro;

        public int idRegistroCondicaoPagamento { get => _idRegistroCondicaoPagamento; set => _idRegistroCondicaoPagamento = value; }
        public int idCondicaoPagamento { get => _idCondicaoPagamento; set => _idCondicaoPagamento = value; }
        public string sDscCondicaoPagamento { get => _sDscCondicaoPagamento; set => _sDscCondicaoPagamento = value; }
        public int nQtdParcelas { get => _nQtdParcelas; set => _nQtdParcelas = value; }
        public decimal nPorcentagemValor { get => _nPorcentagemValor; set => _nPorcentagemValor = value; }
        public string sFuncao { get => _sFuncao; set => _sFuncao = value; }
        public int idLinha { get => _idLinha; set => _idLinha = value; }
        public int idTipoCondicaoPagamento { get => _idTipoCondicaoPagamento; set => _idTipoCondicaoPagamento = value; }
        public int nDDL { get => _nDDL; set => _nDDL = value; }
        public int idTipo { get => _idTipo; set => _idTipo = value; }
        public int idContador { get => _idContador; set => _idContador = value; }
        public int idParceiro { get => _idParceiro; set => _idParceiro = value; }
        public int idRegistro { get => _idRegistro; set => _idRegistro = value; }

        #endregion

        #region | Propriedades




        #endregion

        #region | Funcoes



        public static DataSet CarregarCondicaoPagamentos(int idFamilia = 0)
        {
            string sErro = "";
            string vProcedureFamilia = "sp_Manipula_tbl_Flow_CondicaodePagamento";
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR");
            DataSet ds;

            ds = BD.ExecutarDataSet(vProcedureFamilia, vParametros);

            if (BD.ValidarDataSet(ds, out sErro))
            {
                if (sErro != "")
                {
                    throw new Exception("Erro ao carregar a condição de pagamento");
                }
            }
            return ds;
        }

        public static void CondicaoPagamento_Converter(DataSet ds, List<cls_CondicoesPagamento> lst, int tabela = 0)
        {
            
            try
            {
                foreach (DataRow row in ds.Tables[tabela].Rows)
                {
                    cls_CondicoesPagamento CondicaoPagamento = new cls_CondicoesPagamento();

                    bool conversaoBemSucedida = false;
                    int idCondicaoPagamento = 0;

                    conversaoBemSucedida = int.TryParse(row["idCondicaoPagamento"].ToString(), out idCondicaoPagamento);
                    CondicaoPagamento.idCondicaoPagamento = conversaoBemSucedida ? idCondicaoPagamento : 0;
                    CondicaoPagamento.sDscCondicaoPagamento = row["sDscCondicaoPagamento"].ToString();
                    lst.Add(CondicaoPagamento);
                }

                if(lst.Count == 0)
                    throw new Exception("Erro ao procurar os registros das condições de pagamento");

            }
            catch (Exception ex)
            {
                throw new Exception("Problema na conversão da família em classe" + ex.Message);
            }
        }




        #endregion
    }

    [Serializable]
    public class cls_CondicoesPagamentoCompra
    {

        #region | Construtor 
        public cls_CondicoesPagamentoCompra()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public cls_CondicoesPagamentoCompra
        (
             int idParceiro
            , int idContador
            , int idFamilia
            , int idTabela
            , int idCondicaoPagamento
            , string sFuncao
            , string sDscCondicaoPagamento
            , int idTipo
            , int idRegistro

        )
        {
            _idParceiro = idParceiro;
            _idFamilia = idFamilia;
            _idTabela = idTabela;
            _idCondicaoPagamento = idCondicaoPagamento;
            _sFuncao = sFuncao;
            _sDscCondicaoPagamento = sDscCondicaoPagamento;
            _idTipo = idTipo;
            _idContador = idContador;
            _idRegistro = idRegistro;

        }

        #endregion

        #region | Membros Privados 

        private int _idParceiro;
        private int _idFamilia;
        private int _idTabela;
        private int _idCondicaoPagamento;
        private string _sFuncao;
        private string _sDscCondicaoPagamento;
        private int _idTipo;
        private int _idContador;
        private int _idRegistro;

        public int idParceiro { get => _idParceiro; set => _idParceiro = value; }
        public int idFamilia { get => _idFamilia; set => _idFamilia = value; }
        public int idTabela { get => _idTabela; set => _idTabela = value; }
        public string sFuncao { get => _sFuncao; set => _sFuncao = value; }
        public string sDscCondicaoPagamento { get => _sDscCondicaoPagamento; set => _sDscCondicaoPagamento = value; }
        public int idCondicaoPagamento { get => _idCondicaoPagamento; set => _idCondicaoPagamento = value; }
        public int idTipo { get => _idTipo; set => _idTipo = value; }
        public int idContador { get => _idContador; set => _idContador = value; }
        public int idRegistro { get => _idRegistro; set => _idRegistro = value; }

        #endregion

        #region | Propriedades




        #endregion
    }
    #endregion




}