using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using TT_Flow.FrameWork;
using TT.FrameWork;

namespace TT_Flow.App.Paginas.WMS
{
    public partial class Embalagem : System.Web.UI.Page
    {
        string sTituloPagina = "Embalagem";
        string sPagina_NovoRegistro = "app/Paginas/WMS/Manutencao/Embalagem_Detalhe.aspx";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                Pesquisar();
            }
            txtPesquisa.Focus();
        }

        protected void cmdExportar_Click(object sender, EventArgs e)
        {
            string NomePlanilha = "TabelaExportada" + Funcoes.CarimboDataHora();
            ExcelApp.ExportarArquivoExcel(dtgvConsulta, NomePlanilha, Response, Server.MapPath("~/Download/" + "DadosExportados"), Page);
        }

        protected void Pesquisar()
        {
            pnResultado.Visible = false;
 

            string sFuncao = "CONSULTAR";
            string sDscPesquisa = "";

            sDscPesquisa = txtPesquisa.Text.Trim();
            DataTable tb;
            string sSql = "sp_Manipula_tbl_Flow_WMS_OPI_Embalagem";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);

            tb = BD.ExecutarDataTable(sSql, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb), true);

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

    }
}