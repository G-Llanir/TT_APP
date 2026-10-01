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


namespace TT_Flow.App.Paginas
{
    public partial class Clientes : System.Web.UI.Page
    {
        string sTituloPagina = "Clientes";
        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Pedidos.Incluir, true);

            if (!IsPostBack)
            {

                if (Request["tp"] == "np") //Novo Pedido
                {
                    sTituloPagina = "Selecione o cliente";
                    lblSubTituloPagina.Text = "Novo Pedido";
                }
                else if (Request["tp"] == "cl") //Cliente
                {
                    lblSubTituloPagina.Text = "Manutenção";
                }
                else
                {
                    DIV_PESQUISA.Visible = false;
                    return;
                }

                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                pnMensagem.Visible = false;
 
                //Pesquisar(IDENTITY.Variaveis.idCliente());
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
            string sSql = "sp_Manipula_tbl_Flow_Clientes";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@sRazaoSocial", sDscPesquisa);
            



            // vParametros.Add()
            tb = BD.ExecutarDataTable(sSql, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                GRID.DataBind(dtgvConsulta, tb);
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

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
            }
        }
    }
}