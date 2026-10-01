using System;
using System.Web.UI;
using static TT.FrameWork.Funcoes;

namespace TT_Colaborador.App
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e) => DirecionaPagina("App/Login.aspx");
    }
}