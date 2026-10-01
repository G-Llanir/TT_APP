using Dapper;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using static TT.FrameWork.BD;

namespace TT.FrameWork
{
    public class BD
    {
        public static string StringDeConexao
        {
            get
            {
                try
                {
                    if (Identity.Bancos.Count <= 0) Funcoes.CarregarInfo_BD(0);
                    return string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3}", Identity.BancoAtual.sIP, Identity.BancoAtual.sBanco, Identity.BancoAtual.sUsuario, Identity.BancoAtual.sSenha);
                }
                catch { }

                return "";
            }
        }

        public static DataSet ExecutarDataSet(string sProcedure) => ExecutarDataSet(sProcedure, new Dictionary<string, string>());
        public static DataSet ExecutarDataSet(string sProcedure, Dictionary<string, string> vParametros) => ExecutarDataSet(sProcedure, vParametros, out _);
        public static DataSet ExecutarDataSet(string sProcedure, Dictionary<string, string> vParametros, bool bSQL) => ExecutarDataSet(sProcedure, vParametros, out _);
        public static DataSet ExecutarDataSet(string sProcedure, Dictionary<string, string> vParametros, out string sSQL)
        {
            SqlDataAdapter da = new SqlDataAdapter(sProcedure, StringDeConexao);

            da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
            da.SelectCommand.CommandType = CommandType.StoredProcedure;

            if (vParametros != null)
            {
                foreach (var n in vParametros.Keys)
                {
                    object obj = vParametros[n];

                    if (obj == null || obj == DBNull.Value)
                        obj = DBNull.Value;
                    else
                        obj = obj.ToString().Trim();

                    da.SelectCommand.Parameters.AddWithValue(n.ToString(), obj);
                }
            }

            sSQL = sProcedure + " ";
            for (int n = 0; n < da.SelectCommand.Parameters.Count; n++)
            {
                sSQL += da.SelectCommand.Parameters[n].ToString() + "='" + da.SelectCommand.Parameters[n].Value.ToString() + "',</br>";
            }
            sSQL = sSQL.Substring(0, sSQL.Length - 6);
            try
            {
                DataSet tabela = new DataSet();
                da.Fill(tabela);
                return tabela;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Erro BD-DS: {0}", ex.Message));
            }
            finally
            {
                da.Dispose();
            }
        }
       public static DataSet ExecutarDataSet(string sProcedure, Dictionary<string, string> vParametros, out string sSQL, out List<string> prints)
        {
            using (var conn = new SqlConnection(StringDeConexao))
            {
                List<string> info = new List<string>();
                conn.InfoMessage += (sender, e) =>
                {
                    foreach (SqlError err in e.Errors)
                        info.Add(err.Message);
                };
                prints = info;

                SqlDataAdapter da = new SqlDataAdapter(sProcedure, conn);

                da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                if (vParametros != null)
                {
                    foreach (var n in vParametros.Keys)
                    {
                        object obj = vParametros[n];
                        if (obj == null || obj == DBNull.Value)
                            obj = DBNull.Value;
                        else obj = obj.ToString().Trim();

                        da.SelectCommand.Parameters.AddWithValue(n.ToString(), obj);
                    }
                }

                sSQL = sProcedure + " ";
                for (int n = 0; n < da.SelectCommand.Parameters.Count; n++)
                {
                    sSQL += da.SelectCommand.Parameters[n].ToString() + "='" + da.SelectCommand.Parameters[n].Value.ToString() + "',</br>";
                }
                sSQL = sSQL.Substring(0, sSQL.Length - 6);

                try
                {
                    DataSet tabela = new DataSet();
                    da.Fill(tabela);
                    return tabela;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Erro BD-DS: {0}", ex.Message));
                }
                finally
                {
                    da.Dispose();
                }
            }
        }
        public static bool ValidarDataSet(DataSet ds) => ValidarDataSet(ds, out string sMensagemErro);
        public static bool ValidarDataSet(DataSet ds, out string sMensagemErro)
        {
            string sNome_Coluna;
            string sErro = "";
            bool bRetorno = false;

            try
            {
                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        sNome_Coluna = ds.Tables[0].Columns[0].ColumnName.ToString().Trim().ToUpper();

                        if (sNome_Coluna == "RET" || sNome_Coluna == "NRET")
                        {
                            if (ds.Tables[0].Rows[0][sNome_Coluna].ToString() != "0")
                                sErro = ds.Tables[0].Rows[0][1].ToString();
                            else
                                sErro = "";
                        }
                    }
                    else
                        sErro = "Nenhum Registro Encontrado";
                }
                else
                    sErro = "#Erro no Retorno dos dados";
            }
            catch (Exception ex)
            {
                sErro = "Erro Geral: " + ex.Message.ToString();
            }

            if (sErro != "")
                bRetorno = false;
            else
                bRetorno = true;

            sMensagemErro = sErro;
            return bRetorno;

        }

        /// <summary>
        /// Retorna um DataTable para manipulaçao das páginas
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable ExecutarDataTable(string sql)
        {
            SqlDataAdapter da = new SqlDataAdapter(sql, StringDeConexao);
            DataTable tabela = new DataTable();
            da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;

            da.Fill(tabela);
            return tabela;
        }

        public static DataTable ExecutarDataTable(string sProcedure, Dictionary<string, string> vParametros) => ExecutarDataTable(sProcedure, vParametros, false);
        public static DataTable ExecutarDataTable(string sProcedure, Dictionary<string, string> vParametros, bool bPrint_sSql)
        {
            SqlDataAdapter da = new SqlDataAdapter(sProcedure, StringDeConexao);
            da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
            da.SelectCommand.CommandType = CommandType.StoredProcedure;

            if (vParametros != null)
            {
                foreach (var n in vParametros.Keys)
                {
                    da.SelectCommand.Parameters.AddWithValue(n.ToString(), vParametros[n].ToString().Trim());
                }
                    
                string sMSG_sSQL = "";
                {
                    sMSG_sSQL = sProcedure + " ";
                    for (int n = 0; n < da.SelectCommand.Parameters.Count; n++)
                    {
                        sMSG_sSQL += da.SelectCommand.Parameters[n].ToString() + "='" + da.SelectCommand.Parameters[n].Value.ToString() + "',</br>";

                    }
                }
                sMSG_sSQL = sMSG_sSQL.Substring(0, sMSG_sSQL.Length - 6);
            }

            try
            {
                DataTable tabela = new DataTable();
                da.Fill(tabela);
                return tabela;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable ExecutarDataTable2(string sql)
        {
            SqlDataAdapter da = new SqlDataAdapter(sql, StringDeConexao);
            DataTable tabela = new DataTable();
            da.Fill(tabela);
            return tabela;
        }

        /// <summary>
        /// Retorna um DataReader para manipulação das páginas acesso rápido
        /// </summary>
        /// <param name="sql">SP para execução</param>
        /// <returns></returns>
        public static SqlDataReader ExecutarDataReader(string sql)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = StringDeConexao;

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = sql;

            try
            {
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // Apos a execução do Data Reader a conexao é fechada
                return dr;
            }
            catch
            {
                return null;
            }
        }

        public static List<T> ExecutarLista<T>(string sProcedure, Dictionary<string, string> vParametros, bool bGeraPrintSQL) where T : class, new()
        {
            using (SqlConnection conn = new SqlConnection(StringDeConexao))
            {
                conn.Open();
                DynamicParameters parametros = new DynamicParameters();

                if (vParametros != null)
                {
                    foreach (var param in vParametros)
                    {
                        parametros.Add(param.Key, param.Value?.Trim());
                    }
                }

                if (bGeraPrintSQL)
                {
                    string sMSG_sSQL = sProcedure + " ";
                    foreach (var param in parametros.ParameterNames)
                    {
                        sMSG_sSQL += $"{param} = '{parametros.Get<dynamic>(param)}',<br/>";
                    }
                }

                try
                {
                    return conn.Query<T>(sProcedure, parametros, commandType: CommandType.StoredProcedure).ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro BD-Lista: {ex.Message}");
                }
            }
        }

        public static void ExecutarComandoLista(string sProcedure, Dictionary<string, string> vParametros, bool bGeraPrintSQL)
        {
            using (SqlConnection conn = new SqlConnection(StringDeConexao))
            {
                conn.Open();
                DynamicParameters parametros = new DynamicParameters();

                if (vParametros != null)
                {
                    foreach (var param in vParametros)
                    {
                        parametros.Add(param.Key, param.Value?.Trim());
                    }
                }

                if (bGeraPrintSQL)
                {
                    string sMSG_sSQL = sProcedure + " ";
                    foreach (var param in parametros.ParameterNames)
                    {
                        sMSG_sSQL += $"{param} = '{parametros.Get<dynamic>(param)}',<br/>";
                    }
                }

                try
                {
                    conn.Execute(sProcedure, parametros, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro BD-Comando: {ex.Message}");
                }
            }
        }

        public static string CarregarParametro(Parametro eParametro)
        {
            string sRetorno = "";
            string sCaminho_TFlow = "C:\\+Desenvolvimento\\TT_APP\\TT_Flow\\";
            try
            {
                if (!System.Diagnostics.Debugger.IsAttached)
                {
                    using (DataSet ds = ExecutarDataSet("sp_Select", new Dictionary<string, string> { { "@sTabela", "Parametros" } }))
                    {
                        if (ValidarDataSet(ds))
                        {
                            sCaminho_TFlow = Retorno.DATASET(ds, "sCaminho_TFlow");
                        }
                    }
                }

                switch (eParametro)
                {
                    case Parametro.Caminho_TFlow:
                        sRetorno = sCaminho_TFlow;
                        break;


                }

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar parametro: " + ex.Message);

            }


            return sRetorno;
        }

        public enum Parametro
        {
            Caminho_TFlow = 1,
        }

        public static void GravarEvento(int @idEquipamento, int @idConta, string @sTpEvento, string @sEvento, string @sDscEvento, string sSqlUtilizada)
        {
            try
            {
                Dictionary<string, string> dValores = new Dictionary<string, string>
                {
                    { "@sFuncao", "INCLUIR" },
                    { "@sEvento", @sEvento },
                    { "@idConta", @idConta.ToString() },
                    { "@idEquipamento", @idEquipamento.ToString() },
                    { "@sDscEvento", @sDscEvento.ToString() },
                    { "@sTpEvento", @sTpEvento.ToString() }
                };
                ExecutarDataSet("sp_Manipula_Eventos", dValores);
            }
            catch { }
        }

        public static void GravaLogArquivoTexto(string sTipo, string sProcesso, string sDscLog, string sSqlUtilizada)
        {
            StreamWriter lObjEscreveTexto = new StreamWriter(@"c:\Temp\Log.txt", true);
            string lStrAtencao;

            if (sTipo == "E")
                lStrAtencao = "Erro: ";
            else
                lStrAtencao = "Informação: ";

            lObjEscreveTexto.WriteLine(string.Concat(lStrAtencao, sTipo, " - ", sProcesso, " - ", sDscLog, " - ", sSqlUtilizada, Environment.NewLine));
            lObjEscreveTexto.Flush();
            lObjEscreveTexto.Close();
            lObjEscreveTexto.Dispose();
        }

        public class Retorno
        {
            public static string DATASET(DataSet ds, string sCampo) => DATASET(ds, 0, 0, sCampo, false);
            public static string DATASET(DataSet ds, int nLinha, string sCampo) => DATASET(ds, 0, nLinha, sCampo, false);
            public static string DATASET(DataSet ds, int nLinha, string sCampo, bool bRetiraFormatacaoBanco) => DATASET(ds, 0, nLinha, sCampo, bRetiraFormatacaoBanco);
            public static string DATASET(DataSet ds, int nTabela, int nLinha, string sCampo) => DATASET(ds, nTabela, nLinha, sCampo, false);
            public static string DATASET(DataSet ds, int nTabela, int nLinha, string sCampo, bool bRetiraFormatacaoBanco)
            {
                string sRetorno = null;

                try
                {
                    sRetorno = ds.Tables[nTabela].Rows[nLinha][sCampo].ToString().Trim();

                    if (bRetiraFormatacaoBanco)
                        sRetorno = sRetorno.Replace("R$", "").Trim();
                }
                catch { sRetorno = null; }
                return sRetorno;
            }

            public static int nDR(SqlDataReader dr, string sCampo)
            {
                string sRetorno = DR(dr, sCampo, false);
                if (sRetorno == "")
                    sRetorno = null;

                return Convert.ToInt32(sRetorno);
            }
            public static string DR(SqlDataReader dr, string sCampo) => DR(dr, sCampo, false);
            public static string DR(SqlDataReader dr, string sCampo, bool bRetiraFormatacaoBanco)
            {
                string sRetorno = null;
                try
                {
                    sRetorno = dr[sCampo].ToString().Trim();
                    if (bRetiraFormatacaoBanco)
                        sRetorno = sRetorno.Replace("R$", "").Trim();
                }
                catch { sRetorno = null; }
                return sRetorno;
            }

            public static string DadosXML(XmlDocument XML, string sCampo)
            {
                string sRetorno = "";
                try
                {
                    XmlNodeList XMLNODE = XML.GetElementsByTagName(sCampo);
                    sRetorno = XMLNODE[0].InnerText.ToString();
                }
                catch
                {
                    sRetorno = "";
                }

                return sRetorno;
            }
            public static string DadosXML(XmlNode XML, string sCampo)
            {
                string sRetorno = "";
                try
                {
                    sRetorno = XML[sCampo].InnerText.ToString();
                }
                catch
                {
                    sRetorno = "";
                }

                return sRetorno;
            }
            public static string DadosXML(XDocument xml, string tag)
            {
                try
                {
                    return xml.Descendants(tag).FirstOrDefault()?.Value ?? "";
                }
                catch
                {
                    return "";
                }
            }

            public static string DadosVetor(string[] sVetor, string sCampo) => DadosVetor(sVetor, sCampo, ":");

            public static string DadosVetor(string[] sVetor, string sCampo, string sSeparador)
            {
                string sRetorno = "";

                try
                {
                    for (int nVetor = 0; nVetor < sVetor.Length; nVetor++)
                    {
                        char sSeparador1;
                        sSeparador1 = Convert.ToChar(sSeparador);
                        string[] sValor = sVetor[nVetor].Split(sSeparador1);
                        int nTamanho_sCampo_Vetor = sVetor[nVetor].IndexOf(sSeparador1);

                        if (nTamanho_sCampo_Vetor > 0)
                        {
                            string sCampo_Vetor = sVetor[nVetor].Substring(0, nTamanho_sCampo_Vetor);
                            string sDados_Vetor = sVetor[nVetor].Substring(sCampo_Vetor.Length + 1);

                            if (sCampo_Vetor.Trim() == sCampo)
                            {
                                sRetorno = sDados_Vetor.Trim();
                                break;
                            }
                        }
                    }
                }
                catch
                {
                    sRetorno = "";
                }

                return sRetorno;
            }
        }

        public static class Conversoes
        {
            public static string Numerico(TextBox txt)
            {
                Double nValorRetorno = 0;

                try
                {
                    if (txt.Text != "")
                        nValorRetorno = Convert.ToDouble(txt.Text.Replace("R$", ""));
                    else
                        nValorRetorno = 0;
                }
                catch
                {
                    throw new Exception(string.Format("Erro ao Converter o valor do campo {0} - {1}", txt.ID, txt.Text));
                }

                return nValorRetorno.ToString().Replace(",", ".");
            }

            public static Double Numerico(string String)
            {
                Double nValorRetorno = 0;

                try
                {
                    if (String != "")
                        nValorRetorno = Convert.ToDouble(String.Replace("R$", ""));
                    else
                        nValorRetorno = 0;
                }
                catch
                {
                    throw new Exception(string.Format("Erro ao Converter o valor {0}", String));
                }

                return nValorRetorno;
            }

            public static Decimal Numerico_Decimal(string String)
            {
                Decimal nValorRetorno = 0;

                try
                {
                    if (String != "")
                        nValorRetorno = Convert.ToDecimal(String.Replace("R$", ""));
                    else
                        nValorRetorno = 0;
                }
                catch
                {
                    throw new Exception(string.Format("Erro ao Converter o valor {0}", String));
                }

                return nValorRetorno;
            }

            public static string Numerico(double Obj_double) => Obj_double.ToString().Replace(",", ".");
            public static string Numerico(decimal Obj_decimal) => Obj_decimal.ToString().Replace(",", ".");
        }
    }
}
