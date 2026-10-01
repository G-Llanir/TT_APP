using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT_Flow.App.Controles;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas
{
    public partial class ChangeLog : Page
    {
        private static string sProcedure = "sp_Manipula_tbl_Flow_ChangeLog";
        private static string sTituloPagina = "Change Log";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.TI.Change_Log.Consultar, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.TI.Change_Log.Incluir);

            if (!IsPostBack)
            {
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;

                FUNCOES.Popula_Combo(ddlUsuario, "sp_Select 'Usuarios_ChangeLog'", "idUsuario", "sDscUsuario", false, "Todos os Usuários", "0");

                Pesquisar();
            }

            var requestTarget = Request["__EVENTTARGET"];
            if (!string.IsNullOrEmpty(requestTarget) && requestTarget.Contains("Manual_"))
                BaixarManual(int.Parse(requestTarget.Split('_')[1]));

            txtPesquisa.Focus();
            RegistraScript();
        }

        protected void Pesquisar()
        {
            pnResultado.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscAlteracao", txtPesquisa.Text.Trim() },
                { "@idStatus", ddlStatus.SelectedValue },
                { "@idUsuario", ddlUsuario.SelectedValue },
                { "@idFiltro", ddlFiltro.SelectedValue }
            };
            DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);
            DataTable tb = ds.Tables[0];

            if (tb.Rows.Count > 0)
            {
                if (ds.Tables.Count > 1)
                {
                    string manuais = string.Empty;

                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        string dt = string.IsNullOrEmpty(row["dtUpload"].ToString()) ? string.Empty : DateTime.Parse(row["dtUpload"].ToString()).ToString("dd/MM/yyyy");

                        manuais += $@"
                            <tr>
                                <td style='text-align: center;'><a target='_blank' href='/App/Paginas/IT/ChangeLog_Detalhe.aspx?id={row["idObjeto"]}' data-toggle='tooltip' title='Change Log'>{row["idObjeto"]}</a></td>
                                <td style='text-align: left;'>{row["sNomeArquivo"]}</td>
                                <td style='text-align: left;'>{row["sDscArquivo"]}</td>
                                <td style='text-align: center;'>{dt}</td>
                                <td style='text-align: center;'><a class='btn btn-primary' data-id='{row["idArquivo"]}' data-toggle='tooltip' title='Baixar Manual' onclick='__doPostBack(""Manual_{row["idArquivo"]}"", """");'><i class='fa fa-download'></i></a></td>
                            </tr>";
                    }

                    string tabela = $@"
                        <table class='table' style='margin: 0;'>
                            <thead>
                                <tr>
                                    <th style='width: 5%; text-align: center;'>ID</th>
                                    <th style='width: 35%; text-align: left;'>Nome do Arquivo</th>
                                    <th style='width: 40%; text-align: left;'>Descrição</th>
                                    <th style='width: 15%; text-align: center;'>Data de Upload</th>
                                    <th style='width: 5%; text-align: center;'></th>
                                </tr>
                            </thead>
                            <tbody>
                                {manuais}
                            </tbody>
                        </table>";

                    ltrTabela_Manuais.Text = tabela;
                }

                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, tb, 2, new int[1] { 8 }, "desc", "false", "''"), true);
                pnResultado.Visible = true;
            }
            else
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[6].Text.Equals("S"))
                    e.Row.Cells[6].Text = "✔";

                if (e.Row.Cells[7].Text.Equals("S"))
                    e.Row.Cells[7].Text = "✔";

                e.Row.Cells[6].Text = HttpUtility.HtmlDecode(e.Row.Cells[6].Text);
                e.Row.Cells[7].Text = HttpUtility.HtmlDecode(e.Row.Cells[7].Text);
            }
        }

        protected void BaixarManual(int idArquivo)
        {
            if (idArquivo > 0)
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE" },
                    { "@idArquivo", idArquivo.ToString() }
                };
                DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametros);

                string sNomeArquivo = $"Manual_de_Uso_{FUNCOES.CarimboDataHora()}.pdf";

                File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivo, (byte[])tb.Rows[0]["vbArquivo"]);

                FUNCOES.DownloadArquivo(Page, sNomeArquivo);
                FUNCOES.Scripts.RemoverBackdrop_Modal(Page);
                FUNCOES.Scripts.AbrirModal(Page, "modalManuais");
            }
        }

        protected void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("$(document).ready(function () {");
            sb.AppendLine("");
            sb.AppendLine("     $('.cmdManuais').off('click').on('click', function (e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         $('#modalManuais').modal('show');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("});");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RegistraScript", sb.ToString(), true);
        }
    }
}