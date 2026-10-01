using iTextSharp.text;
using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.FrameWork;
using si = System.Diagnostics;

namespace TT_Flow.App
{


    public partial class botpress : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["Consulta"] != null)
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", Request["Consulta"].ToString() }
                    };
                    dtgvConsulta.DataSource = BD.ExecutarDataSet("sp_Select_IA", vParametros);
                    dtgvConsulta.DataBind();
                }
            }
        }
    }

}