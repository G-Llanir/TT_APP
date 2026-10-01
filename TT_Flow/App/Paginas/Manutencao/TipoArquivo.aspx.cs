using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using System.Web.UI.WebControls;
using TT_Flow.App.Controles;

namespace TT_Hub.App.Paginas.Manutencao
{
    public partial class TipoArquivo : Page
    {
        string sTituloPagina = "Tipo de Arquivo";
        string sPagina_NovoRegistro = "app/Paginas/Manutencao/TipoArquivo_Detalhe.aspx?id=0";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.TipoArquivo.Consultar, true);
            manual.sNomeArquivo = "Manual-TipoArquivo.pdf";

            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.TipoArquivo.Incluir);

            if (!IsPostBack)
            {
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                Pesquisar();
            }

            txtPesquisa.Focus();
        }

        protected void Pesquisar()
        {
            pnResultado.Visible = false;
            pnMensagem.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA_TIPO_ARQUIVO" },
                { "@sDscArquivo", txtPesquisa.Text }
            };
            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                string sScript = TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb, 0, "desc");
                hddScript.Value = sScript;
                ScriptManager.RegisterClientScriptBlock(this.updConsulta, typeof(string), "DataTable_" + Guid.NewGuid().ToString(), sScript, true);
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
            Pesquisar();
        }

        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina(sPagina_NovoRegistro);
        }

        protected void dtgvConsulta_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[3].Text == " - " && e.Row.Cells[4].Text != "&nbsp;")
                {
                    e.Row.Cells[3].Text = "";
                }
                if (e.Row.Cells[4].Text == "S" && e.Row.Cells[4].Text != "&nbsp;")
                {
                    e.Row.Cells[4].Text = "Sim";
                }
                else if (e.Row.Cells[4].Text == "N" && e.Row.Cells[4].Text != "&nbsp;")
                {
                    e.Row.Cells[4].Text = "Não";
                }
                if(e.Row.Cells[5].Text == "S" && e.Row.Cells[5].Text != "&nbsp;")
                {
                    e.Row.Cells[5].Text = "Sim";
                }
                else if (e.Row.Cells[5].Text == "N" && e.Row.Cells[5].Text != "&nbsp;")
                {
                    e.Row.Cells[5].Text = "Não";
                }
            }
        }
    }
}