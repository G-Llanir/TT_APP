using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TT_Hub
{
    public partial class Commbox : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TT_Hub.API.Commbox commbox = new API.Commbox();

            //commbox.EnviarAcaoEquipamento();
            //commbox.ConsultarStatusEquipamentos();
            
        }
    }
}