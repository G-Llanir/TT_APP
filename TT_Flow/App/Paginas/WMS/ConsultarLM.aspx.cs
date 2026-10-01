using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;

namespace TT_Flow.App.Paginas.WMS
{
    public partial class ConsultarLM : System.Web.UI.Page
    {
        string sTituloPagina = "LMs Pendente de OPI";
        string sProcedure = "sp_Manipula_tbl_Flow_Pedidos_LM";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Funcoes.ValidaPermissao(Permissao.RRHH.Seguro.Consultar, true);                
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                Pesquisar();
            }
        }

        private void Pesquisar()
        {
            PopulaCombo();

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "Consultar_LM" },
                { "@dtInicio", txtdtInicio.Text },
                { "@dtFinal", txtdtFinal.Text },
                { "@idTipoLM", ddlTipoLM.Text },
            };
            DataTable tb = BD.ExecutarDataTable(sProcedure, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                div_gvConsulta.Visible = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, tb, 7, "asc", "false", "false"), true);
            }
            else
            {
                div_gvConsulta.Visible = false;
                MensagemPagina.MostraMensagem_Erro("Nenhum registro encontrado");
            }
        }

        private void PopulaCombo()
        {
            //Funcoes.Popula_Combo(ddlUsuarioResponsavel, "sp_Select 'Usuarios'", "idUsuario", "sDscUsuario", false, "Todos os Responsáveis", "0");
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }

    }
}
