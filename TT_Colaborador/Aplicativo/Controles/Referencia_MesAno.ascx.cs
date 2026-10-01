using System;
using System.Globalization;
using System.Web.UI.WebControls;

namespace TT_Colaboradores.Aplicativo.Controles
{
    public partial class Referencia_MesAno : System.Web.UI.UserControl
    {
        public string dataReferencia;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int anoMinimo = 2024;

                retornaAnosAnteriores(anoMinimo);

                // Adiciona anos de 2024 para frente
                for (int i = anoMinimo; i <= 2050; i++)
                {
                    ListItem item = new ListItem(i.ToString(), i.ToString());
                    ddlAno.Items.Add(item);
                }
            }
        }

        public void retornaAnosAnteriores(int anoMinimo)
        {
            int anoAtual = DateTime.Now.Year;

            // Começa do próximo ano após o ano mínimo até o ano atual
            for (int i = anoAtual - 1; i >= anoMinimo; i--)
            {
                ListItem item = new ListItem(i.ToString(), i.ToString());
                ddlAno.Items.Add(item);
            }
        }

        public bool ValidarDados()
        {
            if (ddlAno.SelectedValue == "")
            {
                return false;
            }
            if (ddlMes.SelectedValue == "")
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public DateTime? RetornaValor()
        {
            if (ValidarDados())
            {
                string anoSelecionado = ddlAno.SelectedValue;
                string mesSelecionado = ddlMes.SelectedValue.PadLeft(2, '0'); // Garante que o mês tenha 2 dígitos

                string dataReferencia = anoSelecionado + "-" + mesSelecionado;
                return DateTime.ParseExact(dataReferencia, "yyyy-MM", CultureInfo.InvariantCulture);
            }
            else
            {
                return null;
            }
        }
        public DateTime RetornaValor(bool dadoValidado)
        {
            string anoSelecionado = ddlAno.SelectedValue;
            string mesSelecionado = ddlMes.SelectedValue.PadLeft(2, '0'); // Garante que o mês tenha 2 dígitos

            string dataReferencia = anoSelecionado + "-" + mesSelecionado;
            return DateTime.ParseExact(dataReferencia, "yyyy-MM", CultureInfo.InvariantCulture);
        }

    }
}