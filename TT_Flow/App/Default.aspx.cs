using System;
using System.Web.UI;
using TT.FrameWork;

namespace TT_Flow.App
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e) => Funcoes.DirecionaPagina("App/Login.aspx");
    }
}