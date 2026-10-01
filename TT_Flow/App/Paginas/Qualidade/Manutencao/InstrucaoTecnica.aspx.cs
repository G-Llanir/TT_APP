using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;

namespace TT_Hub.App.Paginas.Qualidade.Manutencao
{
    public partial class InstrucaoTecnica : Page
    {
        string sTituloPagina = "Instrução Técnica";
        string sPagina_NovoRegistro = "app/Paginas/Qualidade/Manutencao/InstrucaoTecnica_Detalhe.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Qualidade.Manutencao.InstrucaoTecnica.Consultar, true);
            if (!FUNCOES.ValidaPermissao(Permissao.Qualidade.Manutencao.InstrucaoTecnica.Incluir))
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

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sPesquisa", txtPesquisa.Text.Trim() }
            };
            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Qualidade_IT", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, tb, 3, new int[2] { 1, 4 }, "asc", "false", "''"), true);

                pnResultado.Visible = true;
            }
            else
                MensagemPagina.MostraMensagem_Erro("Nenhum Lançamento Localizado");
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        => Pesquisar();

        protected void cmdNovo_Click(object sender, EventArgs e)
        => FUNCOES.DirecionaPagina(sPagina_NovoRegistro);

    }
}