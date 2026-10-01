using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;


namespace TT_Colaborador.FrameWork
{
    public class cls_Arquivos
    {
        #region | Construtor 

        public cls_Arquivos()
        { }

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

        #endregion

        public DataSet ConsultarArquivos(int idObjeto, string sTipoObjeto)
        {
            DataSet tabela = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter("sp_Manipula_tbl_Flow_Arquivos", TT.FrameWork.BD.StringDeConexao);
            da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            SqlCommand lObjCommand = new SqlCommand();

            da.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "CONSULTAR";
            da.SelectCommand.Parameters.Add("@idObjeto", SqlDbType.Int).Value = idObjeto;
            da.SelectCommand.Parameters.Add("@sTipoObjeto", SqlDbType.VarChar).Value = sTipoObjeto;
            da.SelectCommand.Parameters.Add("@idUsuario", SqlDbType.VarChar).Value = Identity.Variaveis.idUsuario();

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

            SqlDataAdapter da = new SqlDataAdapter("sp_Manipula_tbl_Flow_Arquivos", TT.FrameWork.BD.StringDeConexao);

            DataSet tabela = new DataSet();
            da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            SqlCommand lObjCommand = new SqlCommand();

            string tipo = "7101";

            da.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "INCLUIR";
            da.SelectCommand.Parameters.Add("@idTipoArquivo", SqlDbType.Int).Value = tipo;
            da.SelectCommand.Parameters.Add("@idObjeto", SqlDbType.Int).Value = Arquivo.idObjeto;
            da.SelectCommand.Parameters.Add("@sNomeArquivo", SqlDbType.VarChar).Value = Arquivo.sNomeArquivo;
            da.SelectCommand.Parameters.Add("@sDscArquivo", SqlDbType.VarChar).Value = Arquivo.sDscArquivo;
            da.SelectCommand.Parameters.Add("@sObservacao", SqlDbType.VarChar).Value = Arquivo.sObservacao;
            da.SelectCommand.Parameters.Add("@vbArquivo", SqlDbType.VarBinary).Value = Arquivo.vbArquivo;
            da.SelectCommand.Parameters.Add("@idUsuario", SqlDbType.Int).Value = Arquivo.idUsuario;
            da.SelectCommand.Parameters.Add("@dtExpiracaoDoc", SqlDbType.VarChar).Value = Arquivo.dtExpiracaoDoc;

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

        public DataSet EnviarArquivoDespesas(cls_Arquivos Arquivo)
        {

            SqlDataAdapter da = new SqlDataAdapter("sp_Manipula_tbl_Flow_Arquivos", TT.FrameWork.BD.StringDeConexao);

            DataSet tabela = new DataSet();
            da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            SqlCommand lObjCommand = new SqlCommand();

            string tipo = "8888";

            da.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "INCLUIR";
            da.SelectCommand.Parameters.Add("@idTipoArquivo", SqlDbType.Int).Value = tipo;
            da.SelectCommand.Parameters.Add("@idObjeto", SqlDbType.Int).Value = Arquivo.idObjeto;
            da.SelectCommand.Parameters.Add("@sNomeArquivo", SqlDbType.VarChar).Value = Arquivo.sNomeArquivo;
            da.SelectCommand.Parameters.Add("@sDscArquivo", SqlDbType.VarChar).Value = Arquivo.sDscArquivo;
            da.SelectCommand.Parameters.Add("@sObservacao", SqlDbType.VarChar).Value = Arquivo.sObservacao;
            da.SelectCommand.Parameters.Add("@vbArquivo", SqlDbType.VarBinary).Value = Arquivo.vbArquivo;
            da.SelectCommand.Parameters.Add("@idUsuario", SqlDbType.Int).Value = Arquivo.idUsuario;
            da.SelectCommand.Parameters.Add("@dtExpiracaoDoc", SqlDbType.VarChar).Value = Arquivo.dtExpiracaoDoc;

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
    }
}