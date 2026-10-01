using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using TT.FrameWork;

namespace TT_Colaborador.FrameWork
{
    public class Identity
    {
        public static string sNomeSistem_BancoConectado
        {
            get
            {
                string sRetorno = _sNomeSistema;
                if (TT.FrameWork.Identity.bDev)
                {
                    sRetorno += " - Area Colaborador - DESENVOLVIMENTO";
                }

                return sRetorno;
            }
            
        }


        static string _sNomeSistema = "T-Colaborador";
        public static string sNomeSistema
        {
            get
            {
                return _sNomeSistema;
            }
            set
            {
                _sNomeSistema = value;
            }
        }



        static string _sVersao = "1.00.0";
        public static string sVersao
        {
            get
            {
                return "V " + _sVersao;
            }
            set
            {
                _sVersao = value;
            }
        }

        static string _nQtdMensagens = "0";
        public static string nQtdMensagens
        {

            get
            {
                return _nQtdMensagens;
            }
            set
            {
                _nQtdMensagens = value;
            }
        }

        static string _nQtdMensagens_NaoLidas = "0";
        public static string nQtdMensagens_NaoLidas
        {

            get
            {
                return _nQtdMensagens_NaoLidas;
            }
            set
            {
                _nQtdMensagens_NaoLidas = value;
            }
        }

        static string _nQtdMensagens_Lidas = "0";
        public static string nQtdMensagens_Lidas
        {

            get
            {
                return _nQtdMensagens_Lidas;
            }
            set
            {
                _nQtdMensagens_Lidas = value;
            }
        }

        public class Variaveis
        {
            public static string idUsuario()
            {
                string idUsuario = "";
                try
                {
                    idUsuario = HttpContext.Current.Session["idUsuario"].ToString();
                }
                catch (Exception)
                {

                    idUsuario = "0";
                }
                return idUsuario;
            }
            public static string sUsuarioLogado()
            {
                string sUsuarioLogado = "";
                try
                {
                    sUsuarioLogado = HttpContext.Current.Session["sUsuarioLogado"].ToString();
                }
                catch (Exception)
                {

                    sUsuarioLogado = "N/D";
                }
                return sUsuarioLogado;
            }
            public static string idCliente()
            {
                string idCliente = "";
                try
                {
                    idCliente = HttpContext.Current.Session["idCliente_TFlow"].ToString();
                }
                catch (Exception)
                {

                    idCliente = "0";
                }
                return idCliente;
            }


            public static string sTipo()
            {
                string sTipo = "C";
                try
                {
                    sTipo = HttpContext.Current.Session["sTipo"].ToString();
                }
                catch (Exception)
                {

                    sTipo = "C";
                }
                return sTipo;
            }



            public static string nQtdMensagens()
            {
                string nQtdMensagens = "";
                try
                {
                    nQtdMensagens = Identity.nQtdMensagens;
                }
                catch (Exception)
                {
                    nQtdMensagens = "0";
                }
                return nQtdMensagens;
            }

            public static string nQtdMensagens_NaoLidas()
            {
                string nQtdMensagens = "";
                try
                {
                    nQtdMensagens = Identity.nQtdMensagens_NaoLidas;
                }
                catch (Exception)
                {
                    nQtdMensagens = "0";
                }
                return nQtdMensagens;
            }

            public static string nQtdAlertas()
            {
                string nQtdAlertas = "";
                try
                {
                    nQtdAlertas = HttpContext.Current.Session["nQtdAlertas"].ToString();
                }
                catch (Exception)
                {
                    nQtdAlertas = "0";
                }
                return nQtdAlertas;
            }

            public static string sRecursos()
            {
                string sRecursos = "";
                try
                {
                    sRecursos = HttpContext.Current.Session["sRecursos"].ToString();
                }
                catch (Exception)
                {

                    sRecursos = "";
                }
                return sRecursos;
            }


        }


    }
}