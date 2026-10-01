using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using TT.FrameWork;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT_Flow.FrameWork.Identity;

namespace TT_Flow.App.Paginas.Manutencao
{
    public partial class Usuarios : Page
    {
        string sTituloPagina = "Usuários";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(55, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(56, false);

            if (!IsPostBack)
            {
                div_Selecao.Visible = true;
                FUNCOES.Popula_Combo(ddlPerfil, "sp_Select 'Flow_Usuarios_Perfil_Consulta'", "idPerfil", "sDscPerfil");

                if (Request["idPerfil"] != null)
                {
                    ddlPerfil.SelectedValue = Request["idPerfil"].ToString();
                    ddlsTipo.SelectedValue = "T";
                    div_Selecao.Visible = false;
                }

                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                pnMensagem.Visible = false;
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
                { "@sFuncao", "CONSULTAR" },
                { "@sDsUsuario", txtPesquisa.Text.Trim() },
                { "@idCliente", IDENTITY.Variaveis.idCliente() },
                { "@sTipo", ddlsTipo.SelectedValue },
                { "@idPerfil", ddlPerfil.SelectedValue },
                { "@sSituacao", ddlSituacao.SelectedValue }
            };
            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Usuarios", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", Grid.DataBindComScriptData(dtgvConsulta, tb, 1, new int[1] { 6 }, "asc", "false", "''"), true);
                pnResultado.Visible = true;
            }
            else
            {
                pnMensagem.Visible = true;
                lblMensagem.Text = "Nenhum registro localizado para sua pesquisa!";
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void cmdNovo_Click(object sender, EventArgs e) => FUNCOES.DirecionaPagina("app/Paginas/Manutencao/Usuarios_Detalhe.aspx?id=0");
    }
}