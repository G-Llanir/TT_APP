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
    public partial class TipoCentroCusto : System.Web.UI.Page
    {
        string sTituloPagina = "Tipo de Centro de Custo";
        string sPagina_NovoRegistro = "app/Paginas/Adm/Manutencao/TipoCentroCusto_Detalhe.aspx";
        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Administracao.CentrodeCustos.TpCentroCusto, true);

            cmdNovo.Visible = false;
            if (FUNCOES.ValidaPermissao(Permissao.Administracao.CentrodeCustos.InserirTpCentroCusto, false))
            {
                cmdNovo.Visible = true;
            }

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;                
                //RegistraScript();
                Pesquisar();
            }
            txtPesquisa.Focus();
        }


        protected void Pesquisar()
        {
            pnResultado.Visible = false;

            DataTable tb;
            string sSql = "sp_Manipula_tbl_Flow_Adm_TipoCentroCusto";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR");
            vParametros.Add("@sDscTipo", txtPesquisa.Text);

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
                            order: [[0, 'asc']],
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
        
    }
}