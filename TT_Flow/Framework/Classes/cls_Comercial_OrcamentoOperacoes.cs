using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using TT.FrameWork;

namespace TT_Flow.FrameWork
{
    public static class cls_Comercial_OrcamentoOperacoes
    {
        public static TransactionScope CriarTransacao()
        {
            TransactionOptions opcoes = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromMinutes(5)
            };
            return new TransactionScope(TransactionScopeOption.Required, opcoes);
        }

        public static void BloquearVinculoPedido(int idOrcamento, int idPedido = 0)
        {
            if (Transaction.Current == null)
                throw new InvalidOperationException("O bloqueio de efetivação exige uma transação ativa.");
            if (idOrcamento <= 0)
                throw new InvalidOperationException("O orçamento informado para efetivação é inválido.");

            ObterBloqueioTransacional("TT_ORCAMENTO_PEDIDO_ORCAMENTO_" + idOrcamento);
            if (idPedido > 0)
                ObterBloqueioTransacional("TT_ORCAMENTO_PEDIDO_PEDIDO_" + idPedido);
        }

        private static void ObterBloqueioTransacional(string recurso)
        {
            using (SqlConnection conexao = new SqlConnection(BD.StringDeConexao))
            using (SqlCommand comando = new SqlCommand("sys.sp_getapplock", conexao))
            {
                comando.CommandType = CommandType.StoredProcedure;
                comando.CommandTimeout = 15;
                comando.Parameters.Add("@Resource", SqlDbType.NVarChar, 255).Value = recurso;
                comando.Parameters.Add("@LockMode", SqlDbType.VarChar, 32).Value = "Exclusive";
                comando.Parameters.Add("@LockOwner", SqlDbType.VarChar, 32).Value = "Transaction";
                comando.Parameters.Add("@LockTimeout", SqlDbType.Int).Value = 10000;
                SqlParameter retorno = comando.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
                retorno.Direction = ParameterDirection.ReturnValue;

                conexao.Open();
                comando.ExecuteNonQuery();

                int codigo = retorno.Value == null || retorno.Value == DBNull.Value
                    ? -999
                    : Convert.ToInt32(retorno.Value);
                if (codigo < 0)
                    throw new InvalidOperationException("Não foi possível serializar a efetivação deste orçamento. Tente novamente.");
            }
        }

        public static cls_Comercial_OrcamentoDuplicacaoResultado Duplicar(
            int idOrcamentoOrigem,
            int numeroOrcamentoOrigem,
            string referencia,
            string observacao,
            string dataEstimativaEntrega,
            string chaveIdempotencia,
            int idUsuario)
        {
            cls_Comercial_OrcamentoDuplicacaoResultado resultado = new cls_Comercial_OrcamentoDuplicacaoResultado();
            try
            {
                DataSet ds = BD.ExecutarDataSet("sp_IA_Orcamento_Duplicar", new Dictionary<string, string>
                {
                    { "@idOrcamentoOrigem", idOrcamentoOrigem.ToString() },
                    { "@nNumeroOrcamentoOrigem", numeroOrcamentoOrigem.ToString() },
                    { "@sReferencia", referencia ?? string.Empty },
                    { "@sObservacao", observacao ?? string.Empty },
                    { "@dtEstimativaEntrega", dataEstimativaEntrega ?? string.Empty },
                    { "@sChaveIdempotencia", chaveIdempotencia ?? string.Empty },
                    { "@idUsuario", idUsuario.ToString() }
                });

                if (!BD.ValidarDataSet(ds))
                {
                    resultado.Erro = MensagemErro(ds, "O banco não retornou o orçamento duplicado.");
                    return resultado;
                }

                DataRow row = ds.Tables[0].Rows[0];
                int idOrcamento;
                int numeroOrcamento;
                int.TryParse(Convert.ToString(row["idOrcamento"]), out idOrcamento);
                int.TryParse(Convert.ToString(row["nNumeroOrcamento"]), out numeroOrcamento);
                if (idOrcamento <= 0)
                {
                    resultado.Erro = MensagemErro(ds, "O banco não retornou o ID do orçamento duplicado.");
                    return resultado;
                }

                resultado.Sucesso = true;
                resultado.IdOrcamento = idOrcamento;
                resultado.NumeroOrcamento = numeroOrcamento;
                resultado.Referencia = row.Table.Columns.Contains("referencia") ? Convert.ToString(row["referencia"]) : string.Empty;
                resultado.Idempotente = row.Table.Columns.Contains("idempotente") && Convert.ToBoolean(row["idempotente"]);
                return resultado;
            }
            catch (Exception ex)
            {
                resultado.Erro = "Erro ao duplicar orçamento: " + ex.Message;
                return resultado;
            }
        }

        private static string MensagemErro(DataSet ds, string padrao)
        {
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Columns.Contains("msg"))
            {
                string mensagem = Convert.ToString(ds.Tables[0].Rows[0]["msg"]);
                if (!string.IsNullOrWhiteSpace(mensagem)) return mensagem;
            }
            return padrao;
        }
    }

    public sealed class cls_Comercial_OrcamentoDuplicacaoResultado
    {
        public bool Sucesso { get; set; }
        public bool Idempotente { get; set; }
        public int IdOrcamento { get; set; }
        public int NumeroOrcamento { get; set; }
        public string Referencia { get; set; }
        public string Erro { get; set; }
    }
}
