using System;
using System.Web;
using System.Web.UI;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using TT.FrameWork;

using System.Web.UI.WebControls;



/// <summary>
/// Summary description for Identity
/// </summary>
/// 

namespace TT_Hub.FrameWork
{
    public class Identity
    {

        public static string _sNomeSistema = "TT - HUB";
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


        static string _sVersao = "V 1.02.6";
        public static string sVersao
        {
            get
            {
                return _sVersao;
            }
            set
            {
                _sVersao = value;
            }
        }



        public class Variaveis
        {

            public static string sNomeSistemaCliente()
            {
                

                string _sNomeSistema = "";
                try
                {
                    _sNomeSistema = sNomeSistema + " - " + HttpContext.Current.Session["sDsCliente"].ToString();
                }
                catch (Exception)
                {

                    _sNomeSistema = sNomeSistema;
                }
                return _sNomeSistema;

            }



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
                    idCliente = HttpContext.Current.Session["idCliente"].ToString();
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
                    nQtdMensagens = HttpContext.Current.Session["nQtdMensagens"].ToString();
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


        }



    }

}