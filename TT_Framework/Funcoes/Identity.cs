using System;
using System.Web;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace TT.FrameWork
{
    public class Identity
    {
        public static List<Banco_de_Dados> Bancos { get; set; } = new List<Banco_de_Dados>();
        public static string _sNomeSistema = "TecandTec";
        public static string _sVersao = "V 1.00.2";
        private static string _PathAplicacao = @"C:\TT\";

        public static bool bDev { get { return System.Diagnostics.Debugger.IsAttached; } }
        public static int idBanco { get { int.TryParse(Variaveis.idBanco(), out int _idBanco); return _idBanco; } }
        public static Banco_de_Dados BancoAtual { get { return Bancos[idBanco]; } }
        public static string PathAplicacao { get { return _PathAplicacao; } set { _PathAplicacao = value; } }
        public static string sNomeSistema { get { return _sNomeSistema; } set { _sNomeSistema = value; } }
        public static string sVersao { get { return _sVersao; } set { _sVersao = value; } }

        public static string CarregarParametros(string sCampo)
        {
            string sRetorno = "";

            try
            {
                DataSet dsCarregarParametros = BD.ExecutarDataSet("sp_CarregaParametros");
                if (BD.ValidarDataSet(dsCarregarParametros, out sRetorno)) sRetorno = BD.Retorno.DATASET(dsCarregarParametros, 0, sCampo);
            }
            catch (Exception ex)
            {
                sRetorno = "Erro ao Carregar os Parâmetros do Sistema: " + ex.Message;
            }

            return sRetorno;
        }

        public class Banco_de_Dados
        {
            private string _sIP = "";
            private string _sUsuario = "";
            private string _sSenha = "";
            private string _sBanco = "";
            private string _sNome = "";
			private string _sChaveIA = "";							  

            public string sIP { get { return _sIP; } set { _sIP = value; } }
            public string sUsuario { get { return _sUsuario; } set { _sUsuario = value; } }
            public string sSenha { get { return _sSenha; } set { _sSenha = value; } }
            public string sBanco { get { return _sBanco; } set { _sBanco = value; } }
            public string sNome { get { return _sNome; } set { _sNome = value; } }

            /// <summary>
            /// Tag opcional CHAVE_IA do XML: segredo que decifra as chaves de API da IA
            /// (tbl_Flow_IA_Config). Fica aqui porque este arquivo ja e o mecanismo de
            /// distribuicao de segredo da plataforma — quem o le ja tem a senha do banco,
            /// entao guardar a chave junto nao abre exposicao nova. E, ao contrario de
            /// guardar no proprio banco, um backup vazado continua sem servir para decifrar.
            /// Ausente = vazio; nesse caso a criptografia procura em appSettings/ambiente.
            /// </summary>
            public string sChaveIA { get { return _sChaveIA; } set { _sChaveIA = value; } }  
		}

        public class Usuario
        {
            string _idUsuario = "0";
            string _nQtdMensagens = "0";
            string _nQtdAlertas = "0";
            string _sUsuarioLogado = "N/D";

            public string idUsuario { get { return _idUsuario; } set { _idUsuario = value; } }
            public string nQtdMensagens { get { return _nQtdMensagens; } set { _nQtdMensagens = value; } }
            public string nQtdAlertas { get { return _nQtdAlertas; } set { _nQtdAlertas = value; } }
            public string sUsuarioLogado { get { return _sUsuarioLogado; } set { _sUsuarioLogado = value; } }

            public static bool Login_Sessao(string sChaveSessao, string sPagina = null, bool bResetaBanco = true)
            {
                bool bLogin = false;

                try
                {
                    if (bResetaBanco) HttpContext.Current.Session["idBanco"] = "0";
                    HttpContext.Current.Session["sLogin"] = "";
                    HttpContext.Current.Session["sDScCliente"] = "";
                    HttpContext.Current.Session["sChaveSessao"] = "";
                    HttpContext.Current.Session["sPermissao"] = "";
                    HttpContext.Current.Session["sURLChamada"] = "";

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR SESSAO" },
                        { "@sChaveSessao", string.IsNullOrEmpty(sChaveSessao) ? null : sChaveSessao },
                        { "@sDEV", bDev ? "S" : "N" }
                    };
                    DataSet dsLogin = BD.ExecutarDataSet("sp_Manipula_tbl_Usuarios_Sessao", vParametros);

                    string sdtValidade = BD.Retorno.DATASET(dsLogin, "dtValidade");
                    int dtresultado = 1;

                    if (sdtValidade != "")
                    {
                        DateTime dtValidade = Convert.ToDateTime(sdtValidade);
                        DateTime dtHoje = DateTime.Today;
                        dtresultado = DateTime.Compare(dtValidade, dtHoje);
                    }

                    if (dtresultado > 0)
                    {
                        if (BD.ValidarDataSet(dsLogin))
                        {
                            HttpContext.Current.Session["sLogin"] = BD.Retorno.DATASET(dsLogin, "sLogin");
                            HttpContext.Current.Session["sUsuarioLogado"] = BD.Retorno.DATASET(dsLogin, "sDscUsuario");
                            HttpContext.Current.Session["idCliente"] = BD.Retorno.DATASET(dsLogin, "idCliente");
                            HttpContext.Current.Session["idParceiro"] = BD.Retorno.DATASET(dsLogin, "idParceiro");
                            HttpContext.Current.Session["sDscParceiro"] = BD.Retorno.DATASET(dsLogin, "sDscParceiro");
                            HttpContext.Current.Session["idUsuario"] = BD.Retorno.DATASET(dsLogin, "idUsuario");
                            HttpContext.Current.Session["idColaborador"] = BD.Retorno.DATASET(dsLogin, "idColaborador");
                            HttpContext.Current.Session["sDsCliente"] = BD.Retorno.DATASET(dsLogin, "sDsCliente");
                            HttpContext.Current.Session["sTipo"] = BD.Retorno.DATASET(dsLogin, "sTipo");
                            HttpContext.Current.Session["sChaveSessao"] = BD.Retorno.DATASET(dsLogin, "sChaveSessao");
                            HttpContext.Current.Session["sPermissao"] = BD.Retorno.DATASET(dsLogin, "sPermissao");
                            HttpContext.Current.Session["sVersao"] = BD.Retorno.DATASET(dsLogin, "sVersao");
                            HttpContext.Current.Session["sLoginVia"] = "CHAVE";

                            bLogin = true;

                            if (string.IsNullOrEmpty(sPagina)) try { sPagina = BD.Retorno.DATASET(dsLogin, "sPagina"); } catch { }
                            if (string.IsNullOrEmpty(sPagina)) sPagina = "App/Dashboard.aspx";

                            Funcoes.DirecionaPagina(sPagina.TrimStart('/'));
                        }
                    }
                    else
                        throw new Exception("Usuário expirado!");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                return bLogin;
            }

            public static bool Login_Usuario(string sLogin, string sSenha, out string sMensagem) => Login_Usuario(sLogin, sSenha, out sMensagem, "");
            public static bool Login_Usuario(string sLogin, string sSenha, out string sMensagem, string sPagina)
            {
                bool bLoginEfetuado = false;

                sMensagem = "";
                HttpContext.Current.Session["sLogin"] = "";
                HttpContext.Current.Session["sDScCliente"] = "";
                HttpContext.Current.Session["sChaveSessao"] = "";
                try
                {
                    sLogin = sLogin.Trim().Replace("'", "").Replace("*", "").Replace("%", "");
                    int dtresultado = 1;

                    DataSet dsLogin = BD.ExecutarDataSet("sp_Login_ValidaUsuario", new Dictionary<string, string> { { "@sLogin", sLogin } });
                    string sdtValidade = BD.Retorno.DATASET(dsLogin, "dtValidade");

                    if (sdtValidade != "")
                    {
                        DateTime dtValidade = Convert.ToDateTime(sdtValidade);
                        DateTime dtHoje = DateTime.Today;
                        dtresultado = DateTime.Compare(dtValidade, dtHoje);
                    }

                    if (dtresultado > 0)
                    {
                        if (BD.ValidarDataSet(dsLogin))
                        {
                            if (sSenha.Trim().Equals(BD.Retorno.DATASET(dsLogin, "sSenha")))
                            {
                                HttpContext.Current.Session["sLogin"] = BD.Retorno.DATASET(dsLogin, "sLogin");
                                HttpContext.Current.Session["dtValidade"] = BD.Retorno.DATASET(dsLogin, "dtValidade");
                                HttpContext.Current.Session["sUsuarioLogado"] = BD.Retorno.DATASET(dsLogin, "sDscUsuario");
                                HttpContext.Current.Session["idCliente"] = BD.Retorno.DATASET(dsLogin, "idCliente");
                                HttpContext.Current.Session["idUsuario"] = BD.Retorno.DATASET(dsLogin, "idUsuario");
                                HttpContext.Current.Session["idColaborador"] = BD.Retorno.DATASET(dsLogin, "idColaborador");
                                HttpContext.Current.Session["sDsCliente"] = BD.Retorno.DATASET(dsLogin, "sDsCliente");
                                HttpContext.Current.Session["idParceiro"] = BD.Retorno.DATASET(dsLogin, 0, "idParceiro");
                                HttpContext.Current.Session["sDscParceiro"] = BD.Retorno.DATASET(dsLogin, 0, "sDscParceiro");
                                HttpContext.Current.Session["sTipo"] = BD.Retorno.DATASET(dsLogin, "sTipo");
                                HttpContext.Current.Session["sPermissao"] = BD.Retorno.DATASET(dsLogin, "sPermissao");

                                Dictionary<string, string> vParametrosSessao = new Dictionary<string, string>
                                {
                                    { "@sFuncao", "GERAR SESSAO" },
                                    { "@idUsuario", BD.Retorno.DATASET(dsLogin, "idUsuario")}
                                };
                                DataSet dsSessao = BD.ExecutarDataSet("sp_Manipula_tbl_Usuarios_Sessao", vParametrosSessao);
                                HttpContext.Current.Session["sChaveSessao"] = BD.Retorno.DATASET(dsSessao, "sChavesessao");
                                HttpContext.Current.Session["sLoginVia"] = "LOGIN";
                                bLoginEfetuado = true;

                                if (sPagina != "")
                                    Funcoes.DirecionaPagina(sPagina);
                            }
                            else
                                throw new Exception("Usuário e senha inválidos!");
                        }
                    }
                    else
                        throw new Exception("Usuário expirado!");
                }
                catch (Exception ex)
                {
                    sMensagem = string.Format("Erro: {0}", ex.Message);
                }

                return bLoginEfetuado;
            }

            public static void Logoff() => Logoff("");
            public static void Logoff(string sChaveSessao)
            {
                HttpContext.Current.Session["sLogin"] = "";
                HttpContext.Current.Session["sDScCliente"] = "";
                HttpContext.Current.Session["sChaveSessao"] = "";
                HttpContext.Current.Session["sPermissao"] = "";
                HttpContext.Current.Session["SalvaMenu"] = null;
                HttpContext.Current.Session["idEmpresa"] = null;

                if (sChaveSessao != "") BD.ExecutarDataSet("sp_Manipula_tbl_Usuarios_Sessao", new Dictionary<string, string> { { "@sFuncao", "ENCERRAR SESSAO" }, { "@sChaveSessao", sChaveSessao } });

                string sLoginVia = "";
                try
                {
                    if (HttpContext.Current.Session["sLoginVia"] != null)
                        sLoginVia = HttpContext.Current.Session["sLoginVia"].ToString();
                    HttpContext.Current.Session["sLoginVia"] = "";
                }
                catch
                {
                    sLoginVia = "LOGIN";
                }

                if (sLoginVia == "CHAVE") HttpContext.Current.Response.Redirect("https://login.tecandtec.com.br");
                //if (sLoginVia == "CHAVE") HttpContext.Current.Response.Redirect("http://localhost:7006/"); // URL para o Projeto -> TT_Login
            }
        }

        public class DADOS
        {
            public static string sDscParceiro(int idParceiro)
            {
                string sDscParceiro = "";
                string sErro = "";

                if (idParceiro != 0)
                {
                    SqlDataReader sdr = null;
                    try
                    {
                        sdr = BD.ExecutarDataReader("EXEC sp_Manipula_tbl_Flow_Clientes @sFuncao = 'CONSULTAR', @idParceiro=" + idParceiro.ToString() + ",@sSituacao='T'");
                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                sDscParceiro = sdr["sRazaoSocial"].ToString();
                            }
                        }
                        else
                        {
                            sDscParceiro = string.Format("Parceiro (ID: {0}) Não localizado!", idParceiro.ToString());
                            sErro = sDscParceiro;
                        }
                    }
                    catch { sErro = "BD: Erro ao Consultar Parceiro!"; }
                    finally { if (sdr != null) sdr.Close(); }
                }

                if (sErro != "") throw new Exception(sErro);

                return sDscParceiro;
            }
        }

        public class Variaveis
        {
            public static string idBanco() { try { return HttpContext.Current.Session["idBanco"].ToString(); } catch { } return "0"; }
            public static string sNomeSistema() { try { return Identity.sNomeSistema; } catch { } return "TecandTec - Login"; }
            public static string sNomeSistemaCliente() { try { return Identity.sNomeSistema + " - " + HttpContext.Current.Session["sDsCliente"].ToString(); } catch { } return Identity.sNomeSistema; }
            public static string idEmpresa() { try { return HttpContext.Current.Session["idEmpresa"].ToString(); } catch { } return "0"; }
            public static string idUsuario() { try { return HttpContext.Current.Session["idUsuario"].ToString(); } catch { } return "0"; }
            public static string idColaborador() { try { return HttpContext.Current.Session["idColaborador"].ToString(); } catch { } return "0"; }
            public static string sUsuarioLogado() { try { return HttpContext.Current.Session["sUsuarioLogado"].ToString(); } catch { } return "N/D"; }
            public static string idCliente() { try { return HttpContext.Current.Session["idCliente"].ToString(); } catch { } return "0"; }
            public static string idParceiro() { try { return HttpContext.Current.Session["idParceiro"].ToString(); } catch { } return "0"; }
            public static string sDscParceiro() { try { HttpContext.Current.Session["sDscParceiro"].ToString(); } catch { } return ""; }
            public static string sTipo() { try { return HttpContext.Current.Session["sTipo"].ToString(); } catch { } return "C"; }
            public static string sPermissao() { try { return HttpContext.Current.Session["sPermissao"].ToString(); } catch { } return ""; }
            public static string nQtdMensagens_NaoLidas() { try { return HttpContext.Current.Session["nQtdMensagens_NaoLidas"].ToString(); } catch { } return "0"; }
            public static string nQtdMensagens_Lidas() { try { return HttpContext.Current.Session["nQtdMensagens_Lidas"].ToString(); } catch { } return "0"; }
            public static string nQtdMensagens() { try { return HttpContext.Current.Session["nQtdMensagens"].ToString(); } catch { } return "0"; }
            public static string nQtdAlertas() { try { return HttpContext.Current.Session["nQtdAlertas"].ToString(); } catch { } return "0"; }
        }
    }
}