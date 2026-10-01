using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Funcoes = TT.FrameWork.Funcoes;

namespace TT_Hub.App
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            Funcoes.DirecionaPagina("app/dashboard.aspx");
        }
    }
}