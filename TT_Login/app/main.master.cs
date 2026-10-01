using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using Funcoes = TT.FrameWork.Funcoes;

namespace TT_Login.App
{
    public partial class MainMaster : System.Web.UI.MasterPage
    {

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/app/login.aspx");
        }





    }
}