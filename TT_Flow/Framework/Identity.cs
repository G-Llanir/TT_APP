using System;
using System.Web;
using static TT.FrameWork.BD;
using TT.FrameWork;
using System.Data;

namespace TT_Flow.FrameWork
{
    public class Identity
    {
        public static void CarregarVariaveis_Sistema()
        {

            string sRetorno;
            try
            {
                DataSet dsCarregarParametros = BD.ExecutarDataSet("sp_CarregaParametros");
                if (BD.ValidarDataSet(dsCarregarParametros, out sRetorno))
                {
                   Identity.sSistema_EnderecoLogin =   BD.Retorno.DATASET(dsCarregarParametros, 0, "sSistema_EnderecoLogin");

                }
            }
            catch (Exception ex)
            {
                sRetorno = "Erro ao Carregar os Parâmetros do Sistema: " + ex.Message;
            }


        }

        public static string sNomeSistem_BancoConectado
        {
            get
            {
                string sRetorno = _sNomeSistema;

                if (TT.FrameWork.Identity.Variaveis.idParceiro() != "0") sRetorno += " - Parceiro: " + TT.FrameWork.Identity.Variaveis.sDscParceiro();
                else if (TT.FrameWork.Identity.bDev || TT.FrameWork.Identity.idBanco > 0)
                {
                    if (TT.FrameWork.Identity.bDev) sRetorno += " - Ambiente: DESENVOLVIMENTO";
                    sRetorno += $" - Banco: {TT.FrameWork.Identity.BancoAtual.sNome}";
                }

                return sRetorno;
            }
        }

        static string _sNomeSistema = "T-Flow";
        static string _sVersao = "1.00.0";
        static string _nQtdMensagens = "0";
        static string _nQtdMensagens_NaoLidas = "0";
        static string _nQtdMensagens_Lidas = "0";

        static string _sSistema_EnderecoLogin = "";
        static string _sSistema_Cor = "";
        static string _sSistema_sNome = "";



        public static string sNomeSistema { get { return _sNomeSistema; } set { _sNomeSistema = value; } }
        public static string sVersao { get { return "V " + _sVersao; } set { _sVersao = value; } }
        public static string nQtdMensagens { get { return _nQtdMensagens; } set { _nQtdMensagens = value; } }
        public static string nQtdMensagens_NaoLidas { get { return _nQtdMensagens_NaoLidas; } set { _nQtdMensagens_NaoLidas = value; } }
        public static string nQtdMensagens_Lidas { get { return _nQtdMensagens_Lidas; } set { _nQtdMensagens_Lidas = value; } }
        public static string sSistema_EnderecoLogin { get { return _sSistema_EnderecoLogin; } set { _sSistema_EnderecoLogin = value; } }
        public static string sSistema_Cor { get { return _sSistema_Cor; } set { _sSistema_Cor = value; } }

        public class Variaveis
        {
            public static string idUsuario() { try { return HttpContext.Current.Session["idUsuario"].ToString(); } catch { } return "0"; }
            public static string sUsuarioLogado() { try { return HttpContext.Current.Session["sUsuarioLogado"].ToString(); } catch { } return "N/D"; }
            public static string idCliente() { try { return HttpContext.Current.Session["idCliente_TFlow"].ToString(); } catch { } return "0"; }
            public static string sTipo() { try { return HttpContext.Current.Session["sTipo"].ToString(); } catch { } return "C"; }
            public static string nQtdMensagens() { try { return Identity.nQtdMensagens; } catch { } return "0"; }
            public static string nQtdMensagens_NaoLidas() { try { return Identity.nQtdMensagens_NaoLidas; } catch { } return "0"; }
            public static string nQtdAlertas() { try { return HttpContext.Current.Session["nQtdAlertas"].ToString(); } catch { } return "0"; }
            public static string sRecursos() { try { return HttpContext.Current.Session["sRecursos"].ToString(); } catch { } return ""; }
        }
    }
}