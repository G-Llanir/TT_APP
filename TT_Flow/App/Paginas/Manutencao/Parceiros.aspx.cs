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

namespace TT_Flow.App.Paginas.Manutencao
{
    public partial class Parceiros : System.Web.UI.Page
    {
        string sTituloPagina = "Parceiros";
        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Parceiros.Consultar, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.Parceiros.Incluir);

            if (!IsPostBack)
            {
                lblSubTituloPagina.Text = "Manutenção";
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                FUNCOES.Popula_Combo(ddlTipoParceiro, "sp_Select 'Flow_Tipo_Parceiro'", "idTipoParceiro", "sDscTipoParceiro", false, "Selecione o Tipo de Parceiro", "0");


                if (IDENTITY.Variaveis.idCliente() != "0")
                {
                    Pesquisar(IDENTITY.Variaveis.idCliente());
                }
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
            vParametros.Add("@idTipo", ddlTipoParceiro.SelectedValue);

            tb = BD.ExecutarDataTable(sSql, vParametros, false);


            foreach (DataRow row in tb.Rows)
            {
                string cpfCnpj = row["sCPF_CNPJ"].ToString();
                if (long.TryParse(cpfCnpj, out long numero))
                {
                    if (row["sTipoCliente"].ToString() == "N")
                    {
                        if (cpfCnpj.Length == 14)
                        {
                            string cnpjMascara = Convert.ToUInt64(cpfCnpj).ToString(@"00\.000\.000\/0000\-00");
                            row["sCPF_CNPJ"] = cnpjMascara;
                        }
                    }
                }
            }

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb), true);
                pnResultado.Visible = true;
            }
            else
            {
                pnMensagem.Visible = true;
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar(IDENTITY.Variaveis.idCliente());
        }
        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina("app/Paginas/Manutencao/Parceiros_Detalhe.aspx?id=0");

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