using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using System.Web.UI.WebControls;
using TT_Flow.FrameWork;
using TT.FrameWork;

namespace TT_Flow.App.Paginas.Compras
{
    public partial class CotacaoCompras : System.Web.UI.Page
    {
        string sTituloPagina = "Solicitação de Cotação";
        string sPagina_NovoRegistro = "app/Paginas/Compras/CotacaoCompras_Detalhe.aspx";
        string sProcedure = "sp_Manipula_tbl_Flow_Comercial_CotacaoCompras";
        protected void Page_Load(object sender, EventArgs e)
        {     
            if (!IsPostBack)
            {
                FUNCOES.ValidaPermissao(Permissao.Compras.CotacaoCompras.Consultar, true);

                if (!FUNCOES.ValidaPermissao(Permissao.Compras.CotacaoCompras.Incluir))
                {
                    cmdNovo.Visible = false;
                }
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                PopularCombos();
                Pesquisar();
            }
        }
        protected void PopularCombos()
        {
            //FUNCOES.Popula_Combo(ddlCliente, "sp_Select 'Flow_Clientes'", "idCliente", "sRazaoSocial", false, "Todos os Clientes", "0");
        }
        protected void Pesquisar()
        {
            pnResultado.Visible = false;
 

            string sFuncao = "CONSULTAR";
            string sDscPesquisa = "";

            //sDscPesquisa = txtPesquisa.Text.Trim();
            DataTable tb;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@txtPesquisa", txtPesquisa.Text);
            //vParametros.Add("@idParceiro", ddlCliente.SelectedValue);
            //vParametros.Add("@idStatus", ddlStatus.SelectedValue);
            tb = BD.ExecutarDataTable(sProcedure, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb, 0, "dsc"), true);

                pnResultado.Visible = true;
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Nenhum Lançamento Localizado");
            }
        }
        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }
        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina(sPagina_NovoRegistro);
        }
        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();

                string sCadastroCompleto = DataBinder.Eval(e.Row.DataItem, "sCadastroCompleto")?.ToString();
                string dtValidade = DataBinder.Eval(e.Row.DataItem, "dtValidade")?.ToString();

                if (sCadastroCompleto == "Cadastro Incompleto")
                {
                    e.Row.CssClass = "info";
                }
                

                if (dtValidade == "01/01/1900 00:00:00")
                {
                    e.Row.Cells[3].Text = "<b>Sem Data</b>"; 
                }
                else if (Convert.ToDateTime(dtValidade).Date > DateTime.Now.Date)
                {
                    e.Row.CssClass = "success";
                    e.Row.Cells[3].Text = "<b>Prazo: </b>" + Convert.ToDateTime(dtValidade).ToString("dd/MM/yyyy");
                }
                else if (Convert.ToDateTime(dtValidade).Date == DateTime.Now.Date)
                {
                    e.Row.CssClass = "warning";
                    e.Row.Cells[3].Text = "<b>Vence Hoje: </b>" + Convert.ToDateTime(dtValidade).ToString("dd/MM/yyyy");
                }
                else if (dtValidade != "01/01/1900 00:00:00" && Convert.ToDateTime(dtValidade).Date < DateTime.Now.Date)
                {
                    e.Row.CssClass = "danger";
                    e.Row.Cells[3].Text = "<b>Vencida: </b>" + Convert.ToDateTime(dtValidade).ToString("dd/MM/yyyy");
                }
            }
        }

    }
}