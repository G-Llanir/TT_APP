using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TT_Hub.App.Controles
{
    public partial class ComboAtivo : System.Web.UI.UserControl
    {
        public bool ReadOnly = false;
        public string Text = "Ativo";
        protected void Page_Load(object sender, EventArgs e)
        {
            txtAtivo.Visible = false;
            ddlAtivo.Visible = false;
            txtAtivo.ReadOnly = ReadOnly;
            if (ReadOnly)
            {
                txtAtivo.Visible = true;
            }
            else
            {
                ddlAtivo.Visible = true;
            }
            lblTitulo.Text = Text;
        }
        public void Situacao_Definir(string sSituacao)
        {
            ddlAtivo.SelectedValue = sSituacao;
            txtAtivo.Text = ddlAtivo.SelectedItem.Text ;
        }

        public string Situacao_Recuperar()
        {
            string sRetorno = "";

            sRetorno = ddlAtivo.SelectedValue.ToString();

            return sRetorno;

        }
        
    }
}