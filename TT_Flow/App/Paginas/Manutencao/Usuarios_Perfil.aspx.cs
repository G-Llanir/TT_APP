using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI;
using TT.FrameWork;
using BD = TT.FrameWork.BD;
using RETORNO = TT.FrameWork.BD.Retorno;
using FUNCOES = TT.FrameWork.Funcoes;

namespace TT_Hub.App.Paginas.Manutencao
{
    public partial class Usuarios_Perfil : Page
    {
        string sTituloPagina = "Perfil de Acesso";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(59, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(60, false);

            if (!IsPostBack)
            {
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;

                Pesquisar();
            }

            txtPesquisa.Focus();
        }

        protected void Pesquisar(string idPerfil = "", string sDscPerfil = "")
        {
            pnResultado.Visible = false;
            pnMensagem.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@idPerfil_Usuario", idPerfil ?? "0" },
                { "@sDscPerfil", txtPesquisa.Text.Trim() }
            };
            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Usuarios_Perfil", vParametros);

            if (BD.ValidarDataSet(ds))
            {
                pnResultado.Visible = true;
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", Grid.DataBindComScriptData(dtgvConsulta, ds.Tables[0], 1, new int[1] { 4 }, "asc", "false", "''"), true);

                try
                {

                    if (!string.IsNullOrEmpty(idPerfil) && ds.Tables[1].Rows.Count > 0)
                    {
                        modalTitle.InnerText = sDscPerfil;
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables_modal", Grid.DataBindComScriptData(gvUsuarios, ds.Tables[1], 1, "asc", "false", "''") + "$('#modal').modal('show');", true);
                    }
                }
                catch { }
            }
            else
            {
                pnMensagem.Visible = true;
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void dtgvConsulta_RowCommand(object sender, GridViewCommandEventArgs e) => Pesquisar(e.CommandName.ToString(), e.CommandArgument.ToString());
    }
}