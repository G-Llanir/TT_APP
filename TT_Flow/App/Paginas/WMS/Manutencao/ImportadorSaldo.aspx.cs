using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using System.IO;
using System.Text;
using System.Net.NetworkInformation;
using System.Web.Services;

namespace TT_Flow.App.Paginas.WMS.Manutencao
{
    public partial class ImportadorArquivo : System.Web.UI.Page
    {
        string sTituloPagina = "Importador de Saldo/Produtos";
        string sPagina_NovoRegistro = "app/Paginas/WMS/Manutencao/ImportadorSaldo_Detalhe.aspx?id=0";


        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Produtos.Consultar, true);
            //cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.Produtos.Incluir);

            if (!IsPostBack)
            {
                
                Pesquisar();
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
            }
            else
            {

            }
            txtPesquisa.Focus();
        }
        protected void LimpaCampos()
        {
            ddlTipoProduto.SelectedValue = "0";

        }
        protected void popularCombo()
        {
            FUNCOES.Popula_Combo(ddlTipoProduto, "sp_Select 'Flow_Produtos_Tipo'", "idTipoProduto", "sDscTipoProduto", false, "Todos os Tipos", "0");
        }


        protected void Pesquisar()
        {
            LimpaCampos();
            popularCombo();

            pnResultado.Visible = false;
            pnMensagem.Visible = false;


            string sFuncao = "CONSULTAR";
            string sDscPesquisa = "";

            sDscPesquisa = txtPesquisa.Text.Trim();
            DataTable tb;
            string sSql = "sp_Manipula_tbl_Flow_Produtos";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@sDscProduto", sDscPesquisa);
            vParametros.Add("@idTipoProduto", ddlTipoProduto.SelectedValue);

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
            Pesquisar();
            pnResultado.Style["Display"] = "block";

        }

        protected void cmdNovoCadastro_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina(sPagina_NovoRegistro);
        }
    }
}