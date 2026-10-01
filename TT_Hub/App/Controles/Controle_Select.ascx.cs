using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;

namespace TT_Hub.App.Controles
{
    public partial class Controle_Select : System.Web.UI.UserControl
    {
        public bool ReadOnly = false;
        //string Tabela = "";
        //string CampoCodigo = "";
        //string CampoDescricao = "";
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void CarregarDados(string sTabela, string sCampoCodigo, string sCampoDescricao)
        {
            FUNCOES.Popula_Combo(ddl, "sp_Select ' " + sTabela + "', " + IDENTITY.Variaveis.idCliente(), sCampoCodigo, sCampoDescricao, false, "Selecione ", "0");
        }

        public string Retorna_Codigo()
        {
            string sCodigo = "0";
            sCodigo = ddl.SelectedItem.Value.ToString();
            return sCodigo;
        }

        public string Retorna_Descricao()
        {
            return ddl.SelectedItem.Text.ToString();
        }
    }
}