using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using TT.FrameWork;
namespace TT_Hub.API
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            //https://hub.tecandtec.com.br/api/?st=3BEDEC22-C62A-447F-BCD5-41E29CDB468C&ie=4&iu=6&cs=1001&so=Silenciar

            try
            {


                string sURL = HttpContext.Current.Request.Url.AbsoluteUri;


                //Registra Eventos
                DataSet dsLog;
                Dictionary<String, String> vParametrosLog = new Dictionary<string, string>();
                vParametrosLog.Add("@sTipoLog", "API_SEGWARE");
                vParametrosLog.Add("@URL", sURL);
                vParametrosLog.Add("@sCorpo", "");
                dsLog = BD.ExecutarDataSet("sp_HUB_Manipula_Log", vParametrosLog);



                string sFuncao          = "REGISTRAR ACAO";
                string sObservacao      = Request.QueryString["so"];
                string idUsuario        = Request.QueryString["iu"];
                string idEquipamento    = Request.QueryString["ie"];
                string CodigoAcao_SW    = Request.QueryString["cs"];




                //Registra Ação
                DataSet dsAcao;
                Dictionary<String, String> vParametrosAcao = new Dictionary<string, string>();

                vParametrosAcao.Add("@sFuncao", sFuncao);
                vParametrosAcao.Add("@sObservacao", sObservacao);
                vParametrosAcao.Add("@idUsuario", idUsuario );
                vParametrosAcao.Add("@idEquipamento", idEquipamento);
                vParametrosAcao.Add("@CodigoAcao_SW", CodigoAcao_SW);
                dsAcao = BD.ExecutarDataSet("sp_HUB_Manipula_tbl_Eventos_Ocorrencia_Acao", vParametrosAcao);

                if (BD.ValidarDataSet(dsAcao))
                {
                    TT_Hub.API.Commbox commbox = new API.Commbox();
                    if (BD.Retorno.DATASET(dsAcao, "sExecutaImediato") == "S" )
                    {
                        commbox.EnviarAcaoEquipamento(BD.Retorno.DATASET(dsAcao, "idRegistroAcao"));
                    }
                }

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }



        }
    }
}