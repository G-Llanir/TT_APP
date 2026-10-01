using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using TT.FrameWork;
using TT_Flow.FrameWork;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using GRID = TT.FrameWork.Grid;

namespace TT_Hub.App.Paginas.RRHH
{
    public partial class PlanosSSTT : Page
    {
        string sTituloPagina = "Planos SSTT";
        string sCaminho = "App/Paginas/RRHH/";
        string sPaginaDetalhe = "PlanosSSTT_Detalhe.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.RRHH.Planos.Consultar, true);

            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.Planos.Incluir);

            if (!IsPostBack)
            {
                Pesquisar();
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
            }
            FUNCOES.Scripts.FocusScript(Page, txtsPesquisa.ClientID);
        }

        protected void Pesquisar()
        {
            pnResultado.Visible = false;
            pnMensagem.Visible = false;
            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR" },
                    { "@sDscPlano", txtsPesquisa.Text.Trim() }
                };
                DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Colaboradores_GHE_Plano_SSTT", vParametros);

                if (tb.Rows.Count > 0)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", Grid.DataBindComScriptData(dtgvConsulta, tb, 1, new int[1] { 4  }, "asc", "false", "''"), true);
                    pnResultado.Visible = true;
                }
                else
                {
                    pnMensagem.Visible = true;
                    MensagemPagina.MostraMensagem_Erro("Nenhum plano localizado para sua pesquisa!");
                }
            }
            catch (Exception ex)
            {
                pnMensagem.Visible = true;
                MensagemPagina.MostraMensagem_Erro("Erro ao Carregar Dados: " + ex.Message);
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }

        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina(string.Format("{0}{1}?id={2}", sCaminho, sPaginaDetalhe, "0"));
        }
    }
}

