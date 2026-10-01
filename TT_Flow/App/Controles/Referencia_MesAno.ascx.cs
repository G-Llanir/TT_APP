using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

namespace TT_Flow.App.Controles
{
    public partial class Referencia_MesAno : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlAno.Items.Add(new ListItem("Ano", "0"));

                int anoAtual = DateTime.Now.Year;
                for (int i = anoAtual - 5; i <= anoAtual + 5; i++) { ddlAno.Items.Add(new ListItem(i.ToString(), i.ToString())); }
                ddlAno.Items.FindByValue(anoAtual.ToString()).Selected = true;
            }
        }

        public bool ValidarDados()
        {
            if (ddlMes.SelectedValue == "0" || ddlAno.SelectedValue == "0")
                return false;

            return true;
        }

        public DateTime? RetornaData()
        {
            if (ValidarDados())
                return DateTime.ParseExact($"{ddlAno.SelectedValue}-{ddlMes.SelectedValue.PadLeft(2, '0')}", "yyyy-MM", CultureInfo.InvariantCulture);
            else
                return null;
        }

        public void DefinirData(DateTime data) { ddlMes.SelectedValue = data.Month.ToString(); ddlAno.SelectedValue = data.Year.ToString(); }

        public void LimparCampos(bool bMes, bool bAno)
        {
            if (bMes) ddlMes.SelectedValue = "0";

            if (bAno) ddlAno.SelectedValue = "0";
        }

    }
}