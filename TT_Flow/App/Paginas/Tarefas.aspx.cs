using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;

namespace TT_Flow.App
{
    public partial class Tarefas : System.Web.UI.Page
    {
        string sTituloPagina = "Tarefas";
        protected void Page_Load(object sender, EventArgs e)

        {
            string DashBoard = "";
            FUNCOES.ValidarPermissaoAcesso();
            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlDepartamento, "sp_Select 'Departamentos_x_Usuarios', " + IDENTITY.Variaveis.idUsuario(), "idDepartamento", "sDscDepartamento", false, "Todos Departamentos", "0");
                FUNCOES.Popula_Combo(ddlStatus, "sp_Select 'Flow_Tarefas_Status'", "idStatusTarefa", "sDscStatusTarefa", false, "Todos os Status", "0");

                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;

                if (Request["DashBoard"] != null)
                {
                    DashBoard = Request["DashBoard"];
                }

                if (DashBoard != "")
                {
                    lblTituloPagina.Text = sTituloPagina + " - " + DashBoard;
                    BreadCrumb_Pagina.TitulodaPagina = sTituloPagina + " - " + DashBoard;
                    PesquisarTarefas(DashBoard);
                }

            }

        }





        protected void PesquisarTarefas( string sDashBoard)
        {
         
            string sErro = "";
            DataSet dsTarefas;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();

            try
            {

                vParametros.Add("@sFuncao", "CONSULTAR_TAREFAS_PEDIDO");
                vParametros.Add("@idDepartamento", ddlDepartamento.SelectedValue);
                vParametros.Add("@idStatusTarefa", ddlStatus.SelectedValue);
                vParametros.Add("@sDashBoard", sDashBoard);
                vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());

                if (sDashBoard == "SuasTarefas")
                {
                    vParametros.Add("@idUsuarioResponsavel", IDENTITY.Variaveis.idUsuario());
                }
                dsTarefas = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos_x_Tarefas", vParametros);

                if (BD.ValidarDataSet(dsTarefas, out sErro))
                {
                    //dtgvConsulta.DataSource = dsTarefas.Tables[0];
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, dsTarefas), true);
                    pnResultado.Visible = true;

                    //dtgvConsulta.DataBind();

                }
                else
                {
                    pnResultado.Visible = false;
                    MensagemPagina.MostraMensagem_Erro(sErro);
                }
            
            }
            catch (Exception ex)
            {

                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
            lblTituloPagina.Text = sTituloPagina + " - " + sDashBoard + " - " + ddlDepartamento.SelectedItem;
            BreadCrumb_Pagina.TitulodaPagina = sTituloPagina + " - " + sDashBoard;


        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
            }
        }

        protected void ddlDepartamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            string DashBoard = "";
            if (Request["DashBoard"] != null)
            {
                DashBoard = Request["DashBoard"];
            }

            if (DashBoard != "")
            {
                PesquisarTarefas(DashBoard);
            }
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            string DashBoard = "";
            if (Request["DashBoard"] != null)
            {
                DashBoard = Request["DashBoard"];
            }

            if (DashBoard != "")
            {
                PesquisarTarefas(DashBoard);
            }

        }
    }
}