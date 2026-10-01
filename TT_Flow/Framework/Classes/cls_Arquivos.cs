using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static TT.FrameWork.BD;

namespace TT_Flow.FrameWork
{
    public class cls_Arquivos
    {
        #region | Construtor 

        public cls_Arquivos() { }

        #endregion

        #region | Membros Privados 

        private int? _idTipoArquivo = null;
        private int? _idObjeto = null;
        private int _idTipoObjeto;
        private string _sNomeArquivo = string.Empty;
        private string _sDscArquivo = string.Empty;
        private string _sObservacao = string.Empty;
        private Byte[] _vbArquivo = null;
        private int? _idUsuario = null;
        private string _dtExpiracaoDoc = string.Empty;
        private string _dtRegistroDoc = string.Empty;
        #endregion

        #region | Propriedades 

        public int? idTipoArquivo
        {
            get { return _idTipoArquivo; }
            set { _idTipoArquivo = value; }
        }
        public int idTipoObjeto
        {
            get { return _idTipoObjeto; }
            set { _idTipoObjeto = value; }
        }
        public int? idObjeto
        {
            get { return _idObjeto; }
            set { _idObjeto = value; }
        }
        public string sNomeArquivo
        {
            get { return _sNomeArquivo; }
            set { _sNomeArquivo = value; }
        }
        public string sDscArquivo
        {
            get { return _sDscArquivo; }
            set { _sDscArquivo = value; }
        }
        public string sObservacao
        {
            get { return _sObservacao; }
            set { _sObservacao = value; }
        }
        public Byte[] vbArquivo
        {
            get { return _vbArquivo; }
            set { _vbArquivo = value; }
        }

        public int? idUsuario
        {
            get { return _idUsuario; }
            set { _idUsuario = value; }
        }

        public string dtExpiracaoDoc
        {
            get { return _dtExpiracaoDoc; }
            set { _dtExpiracaoDoc = value; }
        }

        public string dtRegistroDoc
        {
            get { return _dtRegistroDoc; }
            set { _dtRegistroDoc = value; }
        }

        #endregion

        public enum HistoricoArquivoPedidoAcao
        {
            Arquivado,
            Enviado
        }

        public DataSet ConsultarArquivos(int idObjeto, string sTipoObjeto)
        {
            SqlDataAdapter da = new SqlDataAdapter("sp_Manipula_tbl_Flow_Arquivos", StringDeConexao);
            da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "CONSULTAR";
            da.SelectCommand.Parameters.Add("@idObjeto", SqlDbType.Int).Value = idObjeto;
            da.SelectCommand.Parameters.Add("@sTipoObjeto", SqlDbType.VarChar).Value = sTipoObjeto;
            da.SelectCommand.Parameters.Add("@idUsuario", SqlDbType.VarChar).Value = Identity.Variaveis.idUsuario();

            DataSet tabela = new DataSet();
            try
            {
                da.Fill(tabela);
                return tabela;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Erro BD-DS: {0}", ex.Message));
            }
        }

        public DataSet EnviarArquivo(cls_Arquivos Arquivo)
        {
            SqlDataAdapter da = new SqlDataAdapter("sp_Manipula_tbl_Flow_Arquivos", StringDeConexao);
            da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "INCLUIR";
            da.SelectCommand.Parameters.Add("@idTipoArquivo", SqlDbType.Int).Value = Arquivo.idTipoArquivo;
            da.SelectCommand.Parameters.Add("@idObjeto", SqlDbType.Int).Value = Arquivo.idObjeto;
            da.SelectCommand.Parameters.Add("@sNomeArquivo", SqlDbType.VarChar).Value = Arquivo.sNomeArquivo;
            da.SelectCommand.Parameters.Add("@sDscArquivo", SqlDbType.VarChar).Value = Arquivo.sDscArquivo;
            da.SelectCommand.Parameters.Add("@sObservacao", SqlDbType.VarChar).Value = Arquivo.sObservacao;
            da.SelectCommand.Parameters.Add("@vbArquivo", SqlDbType.VarBinary).Value = Arquivo.vbArquivo;
            da.SelectCommand.Parameters.Add("@idUsuario", SqlDbType.Int).Value = Arquivo.idUsuario;
            da.SelectCommand.Parameters.Add("@dtExpiracaoDoc", SqlDbType.VarChar).Value = Arquivo.dtExpiracaoDoc;
            da.SelectCommand.Parameters.Add("@dtRegistroDoc", SqlDbType.VarChar).Value = Arquivo.dtRegistroDoc;

            DataSet tabela = new DataSet();
            try
            {
                da.Fill(tabela);
                return tabela;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro BD-DS: {ex.Message}");
            }
        }

        public DataSet ArquivarArquivo(int idArquivo, int idUsuario)
        {
            SqlDataAdapter da = new SqlDataAdapter("sp_Manipula_tbl_Flow_Arquivos", StringDeConexao);
            da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "ARQUIVAR_DOC";
            da.SelectCommand.Parameters.Add("@idArquivo", SqlDbType.Int).Value = idArquivo;
            da.SelectCommand.Parameters.Add("@sArquivado", SqlDbType.VarChar).Value = "S";
            da.SelectCommand.Parameters.Add("@idUsuarioArquivado", SqlDbType.Int).Value = idUsuario;

            DataSet tabela = new DataSet();
            try
            {
                da.Fill(tabela);
                return tabela;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro BD-DS: {ex.Message}");
            }
        }

        public string ConsultarNomeArquivo(int idArquivo)
        {
            SqlDataAdapter da = new SqlDataAdapter("sp_Manipula_tbl_Flow_Arquivos", StringDeConexao);
            da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "CONSULTAR_DETALHE";
            da.SelectCommand.Parameters.Add("@idArquivo", SqlDbType.Int).Value = idArquivo;

            DataSet tabela = new DataSet();
            da.Fill(tabela);

            if (tabela.Tables.Count > 0 && tabela.Tables[0].Rows.Count > 0 && tabela.Tables[0].Columns.Contains("sNomeArquivo"))
                return tabela.Tables[0].Rows[0]["sNomeArquivo"].ToString();

            return string.Empty;
        }

        public static bool EhTipoObjetoPedido(string sTipoObjeto)
        {
            if (string.IsNullOrWhiteSpace(sTipoObjeto))
                return false;

            return sTipoObjeto.StartsWith("Pedido", StringComparison.OrdinalIgnoreCase);
        }

        public static string ObterTipoObjetoArquivoPedido(string idTipo)
        {
            if (idTipo == "3" || idTipo == "6")
                return "Pedido-Comex";
            if (idTipo == "7")
                return "PedidoCompras";

            return "Pedido";
        }

        public DataSet ConsultarArquivados(int idObjeto, string sTipoObjeto)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_ARQUIVADOS" },
                { "@idObjeto", idObjeto.ToString() },
                { "@sTipoObjeto", sTipoObjeto },
                { "@idUsuario", Identity.Variaveis.idUsuario() }
            };
            return ExecutarDataSet("sp_Manipula_tbl_Flow_Arquivos", vParametros);
        }

        public DataSet RegistrarHistoricoArquivoPedido(int idPedido, int idUsuario, string sNomeArquivo) => RegistrarHistoricoArquivoPedido(idPedido, idUsuario, sNomeArquivo, HistoricoArquivoPedidoAcao.Arquivado);

        public DataSet RegistrarHistoricoArquivoPedido(int idPedido, int idUsuario, string sNomeArquivo, HistoricoArquivoPedidoAcao eAcao)
        {
            string sNome = string.IsNullOrWhiteSpace(sNomeArquivo) ? "arquivo" : sNomeArquivo.Trim();
            if (sNome.Length > 500)
                sNome = sNome.Substring(0, 500);

            string sFuncao = eAcao == HistoricoArquivoPedidoAcao.Arquivado ? "ARQUIVAR_ARQUIVO_PEDIDO" : "INCLUIR_ARQUIVO_PEDIDO";
            string sPrefixoAcao = eAcao == HistoricoArquivoPedidoAcao.Arquivado ? "Arquivo arquivado: " : "Arquivo enviado: ";

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", sFuncao },
                { "@idPedido", idPedido.ToString() },
                { "@idUsuario", idUsuario.ToString() },
                { "@sObservacao", sNome }
            };
            DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametros);

            if (ds != null && ds.Tables.Count > 0)
                return ds;

            try
            {
                InserirPedidosLogArquivo(idPedido, idUsuario, sPrefixoAcao + sNome);
                return CriarDataSetRetorno(0, "Histórico registrado com sucesso!");
            }
            catch (Exception ex)
            {
                return CriarDataSetRetorno(1, ex.Message);
            }
        }

        static void InserirPedidosLogArquivo(int idPedido, int idUsuario, string sAcao)
        {
            if (sAcao.Length > 500)
                sAcao = sAcao.Substring(0, 500);

            using (SqlConnection conn = new SqlConnection(StringDeConexao))
            using (SqlCommand cmd = new SqlCommand(@"INSERT INTO tbl_Flow_Pedidos_Log (idPedido, dtLog, idUsuario, sTipoAcao, sAcao) VALUES (@idPedido, GETDATE(), @idUsuario, N'Alteração', @sAcao)", conn))
            {
                cmd.Parameters.Add("@idPedido", SqlDbType.Int).Value = idPedido;
                cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = idUsuario;
                cmd.Parameters.Add("@sAcao", SqlDbType.NVarChar, 500).Value = sAcao;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        static DataSet CriarDataSetRetorno(int ret, string msg)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("ret", typeof(int));
            dt.Columns.Add("msg", typeof(string));
            dt.Rows.Add(ret, msg);
            ds.Tables.Add(dt);
            return ds;
        }
    }
}
