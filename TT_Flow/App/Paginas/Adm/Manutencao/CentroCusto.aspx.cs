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
using RETORNO = TT.FrameWork.BD.Retorno;
using static TT.FrameWork.BD;
using TT_Flow.FrameWork;

namespace TT_Hub.App.Paginas.Adm.Manutencao
{
    public partial class CentroCusto : System.Web.UI.Page
    {
        string sTituloPagina = "Centro de Custo";
        string sPagina_NovoRegistro = "app/Paginas/Adm/Manutencao/CentroCusto_Detalhe.aspx";
        protected void Page_Load(object sender, EventArgs e)
        {
            Timer1.Enabled = false;
            FUNCOES.ValidaPermissao(Permissao.Administracao.CentrodeCustos.Consultar, true); ;
            if (!FUNCOES.ValidaPermissao(Permissao.Administracao.CentrodeCustos.Incluir))
            {
                cmdNovo.Visible = false;
            }
            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                //RegistraScript();
                Pesquisar();
                FUNCOES.Popula_Combo(ddlsTipo, "sp_Manipula_tbl_Flow_Adm_TipoCentroCusto 'Flow_TiposCC' ", "idTipoCentroCusto", "sDscTipo", false, "Selecione o Tipo", "0");
            }
            txtPesquisa.Focus();
        }


        protected void Pesquisar()
        {
            pnResultado.Visible = false;

            DataTable tb;
            string sSql = "sp_Manipula_tbl_Flow_Adm_CentroDeCusto";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR");
            vParametros.Add("@idTipoCentroDeCusto", ddlsTipo.SelectedValue);
            vParametros.Add("@sSituacao", ddlsSituacao.SelectedValue);
            vParametros.Add("@sDescricao", txtPesquisa.Text);

            tb = BD.ExecutarDataTable(sSql, vParametros, false);

            if (tb.Rows.Count > 0)
            {

                string script = @"
                    $(document).ready(function() {
                        var table = $('#" + dtgvConsulta.ClientID + @"').DataTable();
                        if (table) {
                            table.destroy();
                        }
                        $('#" + dtgvConsulta.ClientID + @"').DataTable({
                            responsive: true,
                            paging: true,
                            scrollCollapse: true,
                            scrollX: false,
                            scrollY: '',
                            pageLength: 50,
                            searching: true,
                            info: false,
                            bLengthChange: true,
                            order: [[0, 'desc']],
                            language: { url: 'https://cdn.datatables.net/plug-ins/1.11.5/i18n/pt-BR.json' }
                        });
                    });
                ";
                dtgvConsulta.DataSource = tb;
                dtgvConsulta.DataBind();
                if (dtgvConsulta.Rows.Count > 0)
                {
                    //Criando tags theader, tbody, tfoot
                    dtgvConsulta.HeaderRow.TableSection = TableRowSection.TableHeader;
                    dtgvConsulta.UseAccessibleHeader = true;
                    dtgvConsulta.FooterRow.TableSection = TableRowSection.TableFooter;
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables",script , true);
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
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            //Pesquisar();
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            e.Row.Cells[4].Style.Add("border-left", "2px solid #000000 !important;");
            e.Row.Cells[5].Style.Add("border-right", "2px solid #000000 !important;");
            e.Row.Cells[9].Style.Add("border-right", "2px solid #000000 !important;");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //if (DataBinder.Eval(e.Row.DataItem, "idStatus").ToString() != "4")
                //{
                //    e.Row.Cells[7].Font.Bold = false;
                //    e.Row.Cells[7].Text = "-";

                //}

                //if (DataBinder.Eval(e.Row.DataItem, "sDscTipo").ToString().Trim() == "S")
                //{
                //    e.Row.Cells[4].ForeColor = Color.Red;

                //}

                if (Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "nTetoGasto").ToString()) == 0)
                {
                    //e.Row.Cells[4].ForeColor = System.Drawing.Color.Red;
                    e.Row.Cells[4].CssClass = "info";
                    e.Row.Cells[4].ToolTip = "Teto de Gasto não configurado";
                }


                if (Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "nSaldoGasto").ToString()) < 0)
                {
                    e.Row.Cells[5].ForeColor = System.Drawing.Color.Red;
                    e.Row.Cells[5].CssClass = "warning";
                    e.Row.Cells[5].ToolTip = "Despesa acima do teto!";
                }



                if (Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "nReceitaLiquida").ToString()) < 0)
                {
                    e.Row.Cells[8].ForeColor = System.Drawing.Color.Red;
                    e.Row.Cells[8].CssClass = "danger";
                    e.Row.Cells[9].ForeColor = System.Drawing.Color.Red;
                    e.Row.Cells[9].CssClass = "danger";

                }

                if (Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "nReceitaLiquida").ToString()) > 0)
                {
                    e.Row.Cells[8].CssClass = "success";
                    e.Row.Cells[9].CssClass = "success";

                }

                //e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
            }
        }
    }
}