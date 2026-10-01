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

namespace TT_Hub.App.Paginas.Adm.Manutencao
{
    public partial class CategoriaContasReceber : System.Web.UI.Page
    {
        string sTituloPagina = "Categoria Contas Receber";
        string sPagina_NovoRegistro = "app/Paginas/Adm/Manutencao/CategoriaContasReceber_Detalhe.aspx";
        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Administracao.CategoriaContasReceber.Consultar, true);
            if (!FUNCOES.ValidaPermissao(Permissao.Administracao.CategoriaContasReceber.Incluir))
            {
                cmdNovo.Visible = false;
            }

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
            string sSql = "sp_Manipula_tbl_Flow_Adm_Contas_Receber_Categoria";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@sPesquisa", txtPesquisa.Text);
            vParametros.Add("@sDataExibirCaixa", ddlsDataExibirCaixa.SelectedValue);
            vParametros.Add("@sDataExibirContabil", ddlsDataExibirContabil.SelectedValue);

            tb = BD.ExecutarDataTable(sSql, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb), true);
                string script = $@"
                    <script type='text/javascript'>
                    $(document).ready(function() {{
                        setTimeout(function() {{
                            var table = $('#{dtgvConsulta.ClientID}').DataTable();
                            table.page.len(100).draw();
                        }}, 0);
                    }});
                    </script>
                    ";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "AdjustDataTablesPageSize", script, false);


                pnResultado.Visible = true;
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Nenhum Categoria Localizado");
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
            Literal litDespesa = (Literal)e.Row.FindControl("litDespesa");
            if (litDespesa != null)
            {                
                if (DataBinder.Eval(e.Row.DataItem, "sDespesa").ToString() == "S")
                {
                    litDespesa.Text = "<i class='fa fa-check'></i>";
                }
                else
                {
                    litDespesa.Text = string.Empty;
                }
            }

            Literal litEmprestimo = (Literal)e.Row.FindControl("litEmprestimo");
            if (litEmprestimo != null)
            {
                if (DataBinder.Eval(e.Row.DataItem, "sEmprestimoRec").ToString() == "S")
                {
                    litEmprestimo.Text = "<i class='fa fa-check'></i>";
                }
                else
                {
                    litEmprestimo.Text = string.Empty;
                }
            }

            Literal litExibirDash = (Literal)e.Row.FindControl("litExibirDash");
            if (litExibirDash != null)
            {
                if (DataBinder.Eval(e.Row.DataItem, "sExibirDash").ToString() == "S")
                {
                    litExibirDash.Text = "<i class='fa fa-check'></i>";
                }
                else
                {
                    litExibirDash.Text = string.Empty;
                }
            }
           
        }
    }
}