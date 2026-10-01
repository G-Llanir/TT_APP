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
using TT_Hub.App.Paginas.RRHH;

namespace TT_Hub.App.Paginas.Adm.Manutencao
{
    public partial class ContasBancarias : System.Web.UI.Page
    {
        string sTituloPagina = "Contas Bancárias";
        string sPagina_NovoRegistro = "app/Paginas/Adm/Manutencao/ContasBancarias_Detalhe.aspx";
        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Administracao.ContasBancarias.Consultar, true) ;
            if (!FUNCOES.ValidaPermissao(Permissao.Administracao.ContasBancarias.Incluir))
            {
                cmdNovo.Visible = false;
            }
            FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa', @idUsuario=" + IDENTITY.Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");
            if (!IsPostBack)
            {

                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                Pesquisar();
            }
            txtPesquisa.Focus();
        }


        protected void Pesquisar()
        {
            pnResultado.Visible = false;
 

            string sFuncao = "CONSULTAR";
            string sDscPesquisa = "";

            sDscPesquisa = txtPesquisa.Text.Trim();
            DataTable tb;
            string sSql = "sp_Manipula_tbl_Flow_Adm_ContasBancarias";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idEmpresa", ddlidEmpresa.SelectedValue);

            tb = BD.ExecutarDataTable(sSql, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb), true);
                pnResultado.Visible = true;
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Nenhuma conta Localizada");
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
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
            }
        }
    }
}