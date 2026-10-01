using iTextSharp.text;
using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.FrameWork;
using si = System.Diagnostics;

namespace TT_Flow.App
{


    public partial class teste_API : System.Web.UI.Page
    {

        cResultados vResultados = new cResultados();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            { 
            

                vResultados.Adicionar("1", "TESTE", "TESTE", "TESTE", 5);

                vResultados.Adicionar("2", "TESTE", "TESTE", "TESTE", 5);

                vResultados.Adicionar("3", "TESTE", "TESTE", "TESTE", 5);

                vResultados.Adicionar("4", "TESTE", "TESTE", "TESTE", 5);

                vResultados.Adicionar("5", "TESTE", "TESTE", "TESTE", 5);

                vResultados.Adicionar("6", "TESTE", "TESTE", "TESTE", 5);

                vResultados.Adicionar("7", "TESTE", "TESTE", "TESTE", 5);

                vResultados.Adicionar("8", "TESTE", "TESTE", "TESTE", 5);

            }
            //string sresposta = API.LegisWeb.Consultar_ST_Interestadual("SP", "SP", "8424.89.90", "2");
        }


        #region | Classe de Totalização
        public class cResultados : CollectionBase
        {
            public cResultados()
            {
                List.Clear();
            }

            public int Adicionar(Valores sValores)
            {
                return List.Add(sValores);
            }

            public int Adicionar(string sColuna, string sDscCategoriaTipo, string sDscCategoriaPai, string sDscCategoriaPagar_Completa, double nValor)
            {
                Valores sValores = new Valores(sColuna, sDscCategoriaTipo, sDscCategoriaPai, sDscCategoriaPagar_Completa, nValor);
                return List.Add(sValores);
            }

            public double TotalizarColuna(string sColuna)
            {
                double retorno = 0;
                for (int v = 0; v < List.Count; v++)
                {
                    Valores ListaSomar = (Valores)List[v];

                    if (string.Compare(ListaSomar.sColuna, sColuna) == 0)
                    {
                        retorno += ListaSomar.nValor;
                    }
                }
                return retorno;

            }
            public double Totalizar_sDscCategoriaPagar_Completa(string sDscCategoriaPagar_Completa)
            {
                double retorno = 0;
                for (int v = 0; v < List.Count; v++)
                {
                    Valores ListaSomar = (Valores)List[v];

                    if (string.Compare(ListaSomar.sDscCategoriaPagar_Completa, sDscCategoriaPagar_Completa) == 0)
                    {
                        retorno += ListaSomar.nValor;
                    }
                }
                return retorno;

            }

            public double TotalizarColuna_sDscCategoriaPai(string sDscCategoriaPai, string sColuna)
            {
                double retorno = 0;
                for (int v = 0; v < List.Count; v++)
                {
                    Valores ListaSomar = (Valores)List[v];

                    if (string.Compare(ListaSomar.sDscCategoriaPai, sDscCategoriaPai) == 0 && string.Compare(ListaSomar.sColuna, sColuna) == 0)
                    {
                        retorno += ListaSomar.nValor;
                    }
                }
                return retorno;

            }

            public double TotalizarColuna_sDscCategoriaTipo(string sDscCategoriaTipo, string sColuna)
            {
                double retorno = 0;
                for (int v = 0; v < List.Count; v++)
                {
                    Valores ListaSomar = (Valores)List[v];

                    if (string.Compare(ListaSomar.sDscCategoriaTipo, sDscCategoriaTipo) == 0 && string.Compare(ListaSomar.sColuna, sColuna) == 0)
                    {
                        retorno += ListaSomar.nValor;
                    }
                }
                return retorno;

            }


            public Valores this[int index]
            {
                get { return (Valores)List[index]; }
                set { List[index] = value; }
            }
        }
        public class Valores
        {
            protected string _sColuna;
            protected string _sDscCategoriaPagar_Completa;
            protected string _sDscCategoriaTipo;
            protected string _sDscCategoriaPai;
            protected double _nValor;


            public Valores()
            {
            }

            public Valores(string sColuna, string sDscCategoriaTipo, string sDscCategoriaPai, string sDscCategoriaPagar_Completa, double nValor)
            {
                this._sColuna = sColuna;
                this._sDscCategoriaTipo = sDscCategoriaTipo;
                this._sDscCategoriaPai = sDscCategoriaPai;
                this._sDscCategoriaPagar_Completa = sDscCategoriaPagar_Completa;
                this._nValor = nValor;
            }

            public string sColuna
            {
                get { return this._sColuna; }
                set { _sColuna = value; }
            }
            public string sDscCategoriaTipo
            {
                get { return this._sDscCategoriaTipo; }
                set { _sDscCategoriaTipo = value; }
            }
            public string sDscCategoriaPai
            {
                get { return this._sDscCategoriaPai; }
                set { _sDscCategoriaPai = value; }
            }
            public string sDscCategoriaPagar_Completa
            {
                get { return this._sDscCategoriaPagar_Completa; }
                set { _sDscCategoriaPagar_Completa = value; }
            }


            public double nValor
            {
                get { return this._nValor; }
                set { _nValor = value; }
            }






        }


        #endregion

        protected void Button1_Click(object sender, EventArgs e)
        {
            Label1.Text = vResultados.Totalizar_sDscCategoriaPagar_Completa("TESTE").ToString();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {


            ProcessStartInfo teste = new ProcessStartInfo("\\\\192.168.51.87\\AccessControl\\N3000.exe");
            //teste.WorkingDirectory = "C:\\SSB";
            teste.Arguments = "-USER \"abc\" -PASSWORD \"123\" -OPEN 423137148 1";
            teste.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(teste);



            //ProcessStartInfo teste = new ProcessStartInfo();
            ////teste.WorkingDirectory = "C:\\SSB";
            //teste.FileName = "C:\\Windows\\System32\\cmd.exe";
            ////teste.Arguments = "\\\\192.168.51.87\\AccessControl\\N3000.exe -USER \"abc\" -PASSWORD \"123\" -OPEN 423137148 1";
            //teste.Arguments = "\\\\192.168.51.87\\AccessControl\\N3000.exe -USER \"abc\" -PASSWORD \"123\" -OPEN 423137148 1";
            //teste.WindowStyle = ProcessWindowStyle.Normal;
            //Process.Start(teste);

            System.Diagnostics.Process.Start("cmd.exe", "\\\\192.168.51.87\\AccessControl\\N3000.exe -USER \"abc\" -PASSWORD \"123\" -OPEN 423137148 1");
        }
    }




}