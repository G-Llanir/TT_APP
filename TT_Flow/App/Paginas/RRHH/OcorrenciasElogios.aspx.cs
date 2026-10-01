using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;

namespace TT_Hub.App.Paginas.RRHH
{
    public partial class OcorrenciasElogios : Page
    {
        string sTituloPagina = "Ocorrências e Elogios";
        string sPagina_NovoRegistro = "app/Paginas/RRHH/OcorrenciasElogios_Detalhe.aspx?id=0";
        int idColuna_Salario = 6;        

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.RRHH.OcorrenciasElogios.Consultar, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.OcorrenciasElogios.Incluir);
            
            if (!IsPostBack)
            {
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;                                

                PopularCombo();
                Pesquisar();
            }
            
            txtPesquisa.Focus();
        }

        protected void PopularCombo()
        {
            FUNCOES.Popula_Combo(ddlidColaborador, "sp_Select 'Flow_Colaboradores_Ocorrencia', @idUsuario=" + IDENTITY.Variaveis.idUsuario(), "idColaborador", "sDscColaborador", false, "Todos os Colaboradores", "0");
        }

        protected void Pesquisar()
        {
            pnResultado.Visible = false;
            //pnMensagem.Visible = false;           

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE_Ocorrencias_Elogios" },
                { "@sPesquisa", txtPesquisa.Text },
                { "@dtEventoInicio", txtdtEventoInicio.Text },
                { "@dtEventoFinal", txtdtEventoFinal.Text },
                { "@idUsuario", IDENTITY.Variaveis.idUsuario() },
                { "@idColaborador", ddlidColaborador.SelectedValue },
                { "@sAdvertencia", ddlAdvertencia.SelectedValue }
       
            };
                DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Colaboradores", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                pnResultado.Visible = true;

                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, tb, 2, new int[1] { 1 }, "desc", "false", "''"), true);


                //if (FUNCOES.ValidaPermissao(Permissao.RRHH.FuncoesCarteira.ConsultarDadosNívelII))
                //    dtgvConsulta.Columns[idColuna_Salario].Visible = true;
                //else
                //    dtgvConsulta.Columns[idColuna_Salario].Visible = false;
            }
            else
            {
                //pnMensagem.Visible = true;
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void cmdNovo_Click(object sender, EventArgs e) => FUNCOES.DirecionaPagina(sPagina_NovoRegistro);

        protected void dtgvConsulta_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    if (e.Row.Cells[5].Text == "A")
            //        e.Row.Cells[5].Text = "Advertência";
            //    else if (e.Row.Cells[5].Text == "O")
            //        e.Row.Cells[5].Text = "Ocorrência";
            //    else
            //        e.Row.Cells[5].Text = "Elogio";

            //    if (e.Row.Cells[4].Text == "Grave")
            //    {
            //        e.Row.CssClass = "danger";
            //    }
            //    else if (e.Row.Cells[4].Text == "Elogio")
            //    {
            //        e.Row.CssClass = "success";
            //    }
            //}
        }
    }
}