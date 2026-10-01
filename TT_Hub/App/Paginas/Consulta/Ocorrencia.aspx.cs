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
    public partial class Ocorrencia : System.Web.UI.Page
    {
        string sTituloPagina = "Ocorrências";
        protected void Page_Load(object sender, EventArgs e)
        {
            string idAgrupador = "0";
            FUNCOES.ValidarPermissaoAcesso();
            if (!IsPostBack)
            {
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                pnMensagem.Visible = false;

                if (Request["idAgrupador"] != null)
                {
                    idAgrupador = Request["idAgrupador"];
                    DIV_Filtro.Visible = false;
                }
                else
                {
                    PopularCombos();
                }


                PesquisarOcorrencia(idAgrupador);
            }
            //txtPesquisa.Focus();

        }

        void PesquisarOcorrencia(string idAgrupador)
        {
            string sFuncao = "PESQUISA";
            pnResultado.Visible = false;
            pnMensagem.Visible = false;


            DataTable tb;
            string sSql = "sp_HUB_Consulta_Ocorrencia";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);

            if (idAgrupador == "0")
            {
                vParametros.Add("@dtInicio", txtdtInicio.Text);
                vParametros.Add("@dtFinal", txtdtFinal.Text);
                vParametros.Add("@idCliente", ddlCliente.SelectedValue.ToString());
                vParametros.Add("@idEquipamento", ddlEquipamento.SelectedValue.ToString());
                vParametros.Add("@idTipoStatus", ddlTIPO_STATUS.SelectedValue.ToString());
                vParametros.Add("@sStatusOcorrencia", ddlStatusOcorrencia.SelectedValue.ToString());
            }
            else
            {
                vParametros.Add("@idAgrupador", idAgrupador);
                vParametros.Add("@sStatusOcorrencia", "EM ABERTO");
            }
            vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());


            tb = BD.ExecutarDataTable(sSql, vParametros, false);
            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(gvOcorrencias, tb), true);
                //gvOcorrencias.DataSource = tb;
                //gvOcorrencias.DataBind();

                if (idAgrupador != "0")
                {
                    lblTituloPagina.Text = "Ocorrências - " + tb.Rows[0]["sDscAgrupador_Linha1"].ToString() + " " + tb.Rows[0]["sDscAgrupador_Linha2"].ToString();
                }
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

            FUNCOES.Popula_Combo(ddlTIPO_STATUS, "sp_Select 'TIPO_STATUS'" , "idTipoStatus", "sDscTipoStatus", false, "Todos os Tipos de Ocorrências", "0");



        }

        protected void ddlCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            FUNCOES.Popula_Combo(ddlEquipamento, "sp_Select 'EQUIPAMENTOS', " + ddlCliente.SelectedValue , "idEquipamento", "sDescricao", false, "Todos os Equipamentos", "0");

        }

        protected void gvOcorrencias_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
            }

        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            PesquisarOcorrencia("0");
        }
    }
}