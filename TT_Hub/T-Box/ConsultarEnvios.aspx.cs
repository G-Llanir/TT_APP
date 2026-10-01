using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BD = TT.FrameWork.BD;
using System.Data;



namespace TT_Hub.T_Box
{
    public partial class ConsultarEnvios : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Consultar();
            }

        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "SALVAR_PARAMETRO");
            vParametros.Add("@sStringRecebida", txtsRetorno.Text);
            DataSet dsRegistro = BD.ExecutarDataSet("sp_Manipula_HUB_Teste_TBox", vParametros);

        }

        protected void cmdLimparEnvios_Click(object sender, EventArgs e)
        {
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "LIMPAR");
            DataSet dsRegistro = BD.ExecutarDataSet("sp_Manipula_HUB_Teste_TBox", vParametros);
            Consultar();
        }

        void Consultar()
        {
            try
            {
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR");
                DataSet dsRegistro = BD.ExecutarDataSet("sp_Manipula_HUB_Teste_TBox", vParametros);

                gvResultado.DataSource = dsRegistro.Tables[0];
                gvResultado.DataBind();

                txtsRetorno.Text = Server.HtmlDecode(BD.Retorno.DATASET(dsRegistro, 1, 0, "sRetorno"));

            }
            catch (Exception ex)
            {
                Response.Write("Erro ao consultar BD: " + ex.Message);
            }

        }
    }
}