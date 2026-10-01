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

namespace TT_Hub.App.Paginas
{
    public partial class Monitoramento_Conectividade : System.Web.UI.Page
    {
        string sTituloPagina = "Monitoramento Conectividade";
        protected void Page_Load(object sender, EventArgs e)
        {
            string idCliente = "0";
            string idUnidade = "0";
            string sFuncao = "CONSULTAR_DETALHE";
            FUNCOES.ValidarPermissaoAcesso();
            if (!IsPostBack)
            {
                PopularCombos();
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                pnMensagem.Visible = false;

                if (Request["idCliente"] != null)
                {
                    idCliente = Request["idCliente"];
                    ddlCliente.SelectedValue = idCliente;

                }

                if (Request["idUnidade"] != null)
                {
                    idUnidade = Request["idUnidade"];
                    ddlUnidade.SelectedValue = idUnidade;
                }

                if (Request["sFuncao"] != null)
                {
                    if (Request["sFuncao"].ToString().ToLower() == "dashboard")
                    {
                        sFuncao = "CONSULTAR_DETALHE";
                        DIV_Filtro.Visible = false;
                    }
                }
                else
                {
                    
                }
                PesquisarPing(sFuncao, idCliente, idUnidade);
            }
            //txtPesquisa.Focus();

        }

        void PesquisarPing(string sFuncao, string idCliente, string idUnidade)
        {
            
            pnResultado.Visible = false;
            pnMensagem.Visible = false;


            DataTable tb;
            string sSql = "sp_HUB_Consulta_PING";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idCliente", idCliente);
            vParametros.Add("@idUnidade", idUnidade);
            vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());


            tb = BD.ExecutarDataTable(sSql, vParametros, false);
            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(gvOcorrencias, tb), true);
                pnResultado.Visible = true;
            }
            else
            {
                pnMensagem.Visible = true;
                lblMensagem.Text = "Nenhum registro localizado para sua pesquisa!";
            }
        }

        void PopularCombos()
        {
            object sender = new object();
            EventArgs e = new EventArgs();

            FUNCOES.Popula_Combo(ddlCliente, "sp_Select 'CLIENTE', " + IDENTITY.Variaveis.idCliente(), "idCliente", "sDscCliente", false, "Todos os Clientes", "0");

            if (IDENTITY.Variaveis.idCliente() != "0")
            {
                ddlCliente.SelectedValue = IDENTITY.Variaveis.idCliente();
                ddlCliente.Attributes.Add("disabled", "disabled");
            }
            ddlCliente_SelectedIndexChanged(sender, e);

        }

        protected void ddlCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            FUNCOES.Popula_Combo(ddlUnidade, "sp_Manipula_tbl_HUB_Cliente_Unidade 'CONSULTAR', " + ddlCliente.SelectedValue, "idUnidade", "sDScUnidade", false, "Todas as Unidades", "0");
            FUNCOES.Popula_Combo(ddlidTipoEquipamento, "sp_Select 'TIPO_EQUIPAMENTO'", "idTipoEquipamento", "sDscTipoEquipamento", false, "Selecione o modelo do equipamento", "0");
        }

        protected void gvOcorrencias_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int nColunaStatus = 6;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sStatus = DataBinder.Eval(e.Row.DataItem, "sStatus").ToString();

                if (sStatus  == "OK")
                {
                    e.Row.Cells[nColunaStatus].CssClass = "success";
                }
                else if (sStatus == "ALERTA")
                {
                    e.Row.Cells[nColunaStatus].CssClass = "warning";
                }
                else
                {
                    e.Row.Cells[nColunaStatus].CssClass = "danger";
                }


                //e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
            }

        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            PesquisarPing("CONSULTAR_DETALHE", ddlCliente.SelectedValue, ddlUnidade.SelectedValue);
        }
    }
}