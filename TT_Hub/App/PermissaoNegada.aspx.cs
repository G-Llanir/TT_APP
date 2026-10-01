using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TT_Flow.App
{
    public partial class PermissaoNegada : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblTituloPagina.Text = "Acesso negado! ";

            if (Request["Recurso"] != null)
            {
                lblTituloPagina.Text += "<br/>"  + Request["Recurso"];
            }
        }
    }
}