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
    public partial class Cliente : System.Web.UI.Page
    {
        string sTituloPagina = "Clientes";
        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidarPermissaoAcesso();
            if (!IsPostBack)
            {
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                pnMensagem.Visible = false;
                if (IDENTITY.Variaveis.idCliente() != "0")
                {
                    cmdNovo.Visible = false;
                }
                Pesquisar(IDENTITY.Variaveis.idCliente());
            }
            txtPesquisa.Focus();
        }

        protected void Pesquisar(string idCliente)
        {
            pnResultado.Visible = false;
            pnMensagem.Visible = false;


            string sFuncao = "CONSULTAR";
            string sDscPesquisa = "";

            sDscPesquisa = txtPesquisa.Text.Trim();
            DataTable tb;
            string sSql = "sp_HUB_Manipula_tbl_Cliente";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@sDscCliente", sDscPesquisa);
            vParametros.Add("@idCliente", idCliente);



            // vParametros.Add()
            tb = BD.ExecutarDataTable(sSql, vParametros, false);

            if (tb.Rows.Count > 0)
            {

                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb), true);

                //dtgvConsulta.DataSource = tb;
                // dtgvConsulta.DataBind();
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
            Pesquisar(IDENTITY.Variaveis.idCliente());
        }
        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina("app/Paginas/Manutencao/Cliente_Detalhe.aspx?id=0");
        
        }

    }
}