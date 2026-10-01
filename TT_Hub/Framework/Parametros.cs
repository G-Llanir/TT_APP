using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TT_Hub.FrameWork
{
    public class Parametros
    {
        static bool _bDEV = false;
        public static bool bDev
        {
            get { return _bDEV; }
            set { _bDEV = value; }
        }

        static string _sw_uniqueID = "";
        public static string sw_uniqueID
        {
            get { return _sw_uniqueID; }
            set { _sw_uniqueID = value; }
        }

        static string _sw_API_Alarm = "";
        public static string sw_API_Alarm
        {
            get { return _sw_API_Alarm; }
            set { _sw_API_Alarm = value; }
        }

        static string _sw_API_Token = "";
        public static string sw_API_Token
        {
            get { return _sw_API_Token; }
            set { _sw_API_Token = value; }
        }




    }
}