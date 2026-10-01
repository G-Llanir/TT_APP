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

namespace TT_Hub.App.Paginas.Cadastros
{
    public partial class Equipamentos : System.Web.UI.Page
    {
        string sTituloPagina = "Equipamentos";
        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidarPermissaoAcesso();

            
          
            if (!IsPostBack)
            {
                PopularCombo();
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                pnMensagem.Visible = false;

                if (Request["idC"] != null)
                {
                    DIV_Filtro.Visible = false;
                    Pesquisar(Request["idC"].ToString(), IDENTITY.Variaveis.idCliente());
                    
                }
                else
                {
                    Pesquisar("0", IDENTITY.Variaveis.idCliente());
                }

            }
            txtPesquisa.Focus();
        }

        protected void PopularCombo()
        {
            FUNCOES.Popula_Combo(ddlCliente, "sp_Select 'CLIENTE', " + IDENTITY.Variaveis.idCliente(), "idCliente", "sDscCliente", false, "Todos os Clientes", "0");
            if (IDENTITY.Variaveis.idCliente() != "0")
            {
                ddlCliente.SelectedValue = IDENTITY.Variaveis.idCliente();
                ddlCliente.Attributes.Add("disabled", "disabled");
            }

            FUNCOES.Popula_Combo(ddlidTipoEquipamento, "sp_Select 'TIPO_EQUIPAMENTO', " + IDENTITY.Variaveis.idCliente(), "idTipoEquipamento", "sDscTipoEquipamento", false, "Todos os Tipos de Equipamentos", "0");
            FUNCOES.Popula_Combo(ddlidTipoMonitoramento, "sp_Select 'HUB_TipoMonitoramento'", "idTipoMonitoramento", "sDscTipoMonitoramento", false, "Todos os Tipos de Monitoramento", "0");
            FUNCOES.Popula_Combo(ddlUnidade, "sp_Manipula_tbl_HUB_Cliente_Unidade 'CONSULTAR', " + ddlCliente.SelectedValue, "idUnidade", "sDScUnidade", false, "Todas as Unidades", "0");

        }

        protected void Pesquisar(string idClientePesquisa, string idCliente)
        {
            pnResultado.Visible = false;
            pnMensagem.Visible = false;


            string sFuncao = "PESQUISA";
            string sDscPesquisa = "";
            

            if (idCliente == "0" || idClientePesquisa != "0")
            {
                idCliente = idClientePesquisa;
            }

            if (ddlCliente.SelectedValue != "0")
            {
                idCliente = ddlCliente.SelectedValue;
            }


            sDscPesquisa = txtPesquisa.Text.Trim();
            DataTable tb;
            string sSql = "sp_HUB_Manipula_tbl_Equipamentos";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@sObjPesquisa", sDscPesquisa);
            vParametros.Add("@idCliente", idCliente);
            vParametros.Add("@idTipoEquipamento", ddlidTipoEquipamento.SelectedValue);
            vParametros.Add("@idTipoMonitoramento", ddlidTipoMonitoramento.SelectedValue);
            vParametros.Add("@idUnidade", ddlUnidade.SelectedValue);


            // vParametros.Add()
            tb = BD.ExecutarDataTable(sSql, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb), true);
                pnResultado.Visible = true;
            }
            else
            {
                pnMensagem.Visible = true;
                lblMensagem.Text = "Nenhum registro localizado para sua pesquisa!";
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar("0", IDENTITY.Variaveis.idCliente());
        }
        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina("app/Paginas/Manutencao/Equipamentos_Detalhe.aspx?id=0");
        
        }

        protected void ddlCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            FUNCOES.Popula_Combo(ddlUnidade, "sp_Manipula_tbl_HUB_Cliente_Unidade 'CONSULTAR', " + ddlCliente.SelectedValue, "idUnidade", "sDScUnidade", false, "Todas as Unidades", "0");

        }
    }
}