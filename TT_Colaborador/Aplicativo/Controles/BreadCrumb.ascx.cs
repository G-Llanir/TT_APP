using System;
using System.Collections;
using System.Web.UI;
using System.Text;

namespace TT_Colaborador.Aplicativo.Controles
{
    public partial class BreadCrumb : UserControl
    {
        #region | Propriedades

        private Colecao.ColecaodeCrumbs _pageCrumb = new Colecao.ColecaodeCrumbs();
        private SortedList _CrumbList;

        private string _TitulodaPagina;
        private short _NivelPagina;

        public short NivelPagina
        {
            get { return _NivelPagina; }
            set { _NivelPagina = value; }
        }

        public string TitulodaPagina
        {
            get { return _TitulodaPagina; }
            set { _TitulodaPagina = value; }
        }

        #endregion

        private void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (_NivelPagina <= 0)
                    _NivelPagina = 1;

                if ((string.IsNullOrEmpty(_TitulodaPagina)))
                    _TitulodaPagina = "Sem Título";

                _pageCrumb = new Colecao.ColecaodeCrumbs(_NivelPagina, Request.RawUrl, _TitulodaPagina);

                if (Session["HASH_OF_CRUMPS"] == null)
                {
                    _CrumbList = new SortedList();
                    Session.Add("HASH_OF_CRUMPS", _CrumbList);
                }
                else
                    _CrumbList = (SortedList)Session["HASH_OF_CRUMPS"];

                ModificarLista();
                PrintarBreadCrumb();
            }
        }

        #region | Funções para geração do BreadCrumb

        private void ModificarLista()
        {
            RemoverNivelAnterior();

            if (_pageCrumb.Nivel == 1)
            {
                _CrumbList.Clear();
                _CrumbList.Add(Convert.ToInt16(1), new Colecao.ColecaodeCrumbs(1, "/Default.aspx", "DashBoard"));
            }
            else
            {
                if (_CrumbList.Count == 0)
                    _CrumbList.Add(Convert.ToInt16(1), new Colecao.ColecaodeCrumbs(1, "/Default.aspx", "DashBoard"));

                _CrumbList.Add(_NivelPagina, _pageCrumb);
            }
        }

        private void RemoverNivelAnterior()
        {
            short Nivel = 0;
            ArrayList lstRemover = new ArrayList(_CrumbList.Count);

            foreach (short Nivel_Loop in _CrumbList.Keys)
            {
                Nivel = Nivel_Loop;

                if (Nivel >= _NivelPagina)
                    lstRemover.Add(Nivel);
            }

            foreach (short Nivel_Loop in lstRemover)
            {
                Nivel = Nivel_Loop;
                _CrumbList.Remove(Nivel);
            }
        }

        private void PrintarBreadCrumb()
        {
            StringBuilder linkString = new StringBuilder();
            Colecao.ColecaodeCrumbs pageCrumb;
            int nContador;

            for (nContador = 0; nContador <= _CrumbList.Count - 2; nContador++)
            {
                pageCrumb = (Colecao.ColecaodeCrumbs)_CrumbList.GetByIndex(nContador);
                linkString.Append(string.Format("<li><a href = {0} >{1} </a></li> &nbsp;/&nbsp;", pageCrumb.urlPagina, pageCrumb.TituloPagina));
            }

            pageCrumb = (Colecao.ColecaodeCrumbs)_CrumbList.GetByIndex(nContador);
            linkString.Append(string.Format("<li class=\"active\">{1}</li>", pageCrumb.urlPagina, pageCrumb.TituloPagina));

            ltrBreadCrumb.Text = linkString.ToString();
        }

        #endregion
    }

    #region | Coletação de BreadCrumbs

    public class Colecao
    {
        public struct ColecaodeCrumbs
        {
            private short _NivelPagina;
            private string _urlPagina;
            private string _TituloPagina;

            public ColecaodeCrumbs(short NivelPagina, string urlPagina, string TituloPagina)
            {
                _NivelPagina = NivelPagina;
                _urlPagina = urlPagina;
                _TituloPagina = TituloPagina;
            }

            public short Nivel
            {
                get { return _NivelPagina; }
            }
            public string urlPagina
            {
                get { return _urlPagina; }
            }
            public string TituloPagina
            {
                get { return _TituloPagina; }
            }

        }
    }

    #endregion
}