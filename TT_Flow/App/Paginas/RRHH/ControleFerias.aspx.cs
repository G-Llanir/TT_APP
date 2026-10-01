using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Paginas.Adm.Financeiro;
using TT_Flow.App.Paginas.Manutencao;
using TT_Flow.FrameWork;
using static TT.FrameWork.BD;
using Identity = TT.FrameWork.Identity;

namespace TT_Flow.App.Paginas.RRHH
{
    public partial class ControleFerias : System.Web.UI.Page
    {
        string sTituloPagina = "Controle Férias";
        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Ferias";

        public List<cls_FeriasDetalhe> bs_FeriasDetalhe
        {
            get
            {
                if (ViewState["bs_FeriasDetalhe"] == null)
                {
                    ViewState["bs_FeriasDetalhe"] = new List<cls_FeriasDetalhe>();
                }
                return (List<cls_FeriasDetalhe>)ViewState["bs_FeriasDetalhe"];
            }
            set
            {
                ViewState["bs_FeriasDetalhe"] = value;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            Funcoes.ValidaPermissao(Permissao.RRHH.ControleFerias.Consultar, true);
            if (!Funcoes.ValidaPermissao(Permissao.RRHH.ControleFerias.Incluir))
            {
                btnNovoFerias.Visible = false;
            }

            BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;


            if (!IsPostBack)
            {
                PopulaCombo();
                AlterarVisualizacao_Edicao(false);
                Pesquisar();
            }

            if (Request["id"] != null)
            {
                string id = Request["id"].ToString();
                hddidControleFerias.Value = id;
                PopulaCombo();
                FeriasDetalhe(id);
            }

            RegistraScript();
        }

        private void Pesquisar()
        {

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@dtInicio", txtdtInicio.Text.Trim() },
                { "@dtFinal", txtdtFinal.Text.Trim() },
                { "@idTipoEvento", ddlidTipoPesquisa.SelectedValue },
                { "@idColaborador", ddlidColaboradorPesquisa.SelectedValue }
            };
            DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros, false);

            if (BD.ValidarDataSet(ds, out string sErro))
            {
                div_gvConsulta.Visible = true;
                List<cls_FeriasDetalhe> detalheFerias = GetDetalheFerias(ds.Tables[0]);
                List<cls_Ferias> feriasColaborador = GetFeriasPorColaborador(detalheFerias);

                dtgvConsulta.DataSource = feriasColaborador;
                dtgvConsulta.DataBind();
            }
            else
            {
                div_gvConsulta.Visible = false;
                MensagemPagina.MostraMensagem_Erro(sErro);
            }
        }

        private List<cls_FeriasDetalhe> GetDetalheFerias(DataTable dt)
        {
            bs_FeriasDetalhe.Clear();

            foreach (DataRow row in dt.Rows)
            {
                cls_FeriasDetalhe detalheFerias = new cls_FeriasDetalhe
                {
                    idControleFerias = Convert.ToInt32(row["idControleFerias"]),
                    idColaborador = Convert.ToInt32(row["idColaborador"]),
                    sDscColaborador = row["sDscColaborador"].ToString(),
                    dtEvento = row["dtEvento"].ToString(),
                    sDescricao = row["sDescricao"].ToString(),
                    sDscEvento = row["sDscEvento"].ToString(),
                    nDias = Convert.ToInt32(row["nDias"]),
                    nSaldo = Convert.ToInt32(row["nSaldo"])
                };

                bs_FeriasDetalhe.Add(detalheFerias);
            }

            return bs_FeriasDetalhe;
        }

        private List<cls_Ferias> GetFeriasPorColaborador(List<cls_FeriasDetalhe> detalhes)
        {
            var detalhesPorColab = detalhes
                .GroupBy(d => d.idColaborador)
                .ToDictionary(g => g.Key, g => g.ToList());

            var colaboradoresUnicos = detalhes
                .GroupBy(d => d.idColaborador)
                .Select(g => new cls_Ferias
                {
                    idColaborador = g.Key,
                    sDscColaborador = g.First().sDscColaborador,
                    nSaldo = g.First().nSaldo,
                    ls_FeriasDetalhes = detalhesPorColab[g.Key]
                })
                .ToList();

            return colaboradoresUnicos;
        }

        private void AlterarVisualizacao_Edicao(bool bEdicao)
        {
            pnFeriasConsulta.Visible = !bEdicao;
            pnFeriasDetalhe.Visible = bEdicao;
        }

        private void FeriasDetalhe(string id)
        {
            try
            {
                DataSet ds;
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE" },
                    { "@idControleFerias", id }
                };
                ds = BD.ExecutarDataSet(sProcedure, vParametros);
                if (BD.ValidarDataSet(ds, out string sErro))
                {
                    txtidControleFerias.Text = id;
                    ddlidColaborador.SelectedValue = Retorno.DATASET(ds, 0, "idColaborador");
                    ddlidTipo.SelectedValue = Retorno.DATASET(ds, 0, "idTipoEvento");
                    txtdtEvento.Text = Convert.ToDateTime(Retorno.DATASET(ds, 0, "dtEvento")).ToString("yyyy-MM-dd");
                    txtnQtdFerias.Text = Retorno.DATASET(ds, 0, "nDias");
                    txtsDescricao.Text = Retorno.DATASET(ds, 0, "sDescricao");
                    hddidColaborador.Value = Retorno.DATASET(ds, 0, "idColaborador");

                    AlterarVisualizacao_Edicao(true);
                    ddlidColaborador.Attributes.Add("disabled", "disabled");

                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        aba_historico.Visible = true;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables_Historico", TT.FrameWork.Grid.DataBindComScriptData(gv_Historico, ds.Tables[1], 0, "desc", "false", "''"), true);
                    }

                    if (!Funcoes.ValidaPermissao(Permissao.RRHH.ControleFerias.Editar))
                    {
                        btnSalvar.Visible = false;
                    }
                }
                else
                {
                    throw new Exception("BD: " + sErro.ToString());
                }
            }
            catch (Exception e)
            {
                MensagemPaginaDetalhe.MostraMensagem_Erro(e.Message);
            }

        }

        protected void lbFeriasDetalhe_Command(object sender, CommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();
            hddidControleFerias.Value = id;
            PopulaCombo();
            FeriasDetalhe(id);
        }

        private void PopulaCombo()
        {
            Funcoes.Popula_Combo(ddlidTipoPesquisa, "sp_Select 'tbl_Flow_Colaboradores_Controle_Ferias_TipoEvento'", "idTipoEvento", "sDscEvento", true, "Todos os Tipos", "0");
            Funcoes.Popula_Combo(ddlidTipo, "sp_Select 'tbl_Flow_Colaboradores_Controle_Ferias_TipoEvento'", "idTipoEvento", "sDscEvento", true, "Selecione o Tipo", "0");
            Funcoes.Popula_Combo(ddlidColaborador, "sp_Select 'Flow_Colaboradores'", "idColaborador", "sDscColaborador", false, "Selecione o Colaborador", "0");
            Funcoes.Popula_Combo(ddlidColaboradorPesquisa, "sp_Select 'Flow_Colaboradores'", "idColaborador", "sDscColaborador", false, "Todos os Colaboradores", "0");
        }

        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (ddlidColaborador.SelectedValue == "0" && hddidColaborador.Value == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Colaborador";
            }

            if (ddlidTipo.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Tipo";
            }
            if (txtnQtdFerias.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite a Quantidade de Dias";
            }

            if (!Validacoes.ValidarData(txtdtEvento))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite a Data";
            }

            if (txtsDescricao.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite uma Descrição";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPaginaDetalhe.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        private void LimpaCampos()
        {
            ddlidColaborador.SelectedValue = "0";
            ddlidTipo.SelectedValue = "0";
            txtnQtdFerias.Text = "";
            txtdtEvento.Text = "";
            txtsDescricao.Text = "";
        }

        void RegistraScript()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("$('.composicaoLinha').addClass('fa fa-plus');\r\n");
            sb.Append("$('.composicaoLinha').click(function() {\r\n");
            sb.Append("     var icon = $(this);\r\n");
            sb.Append("     var divId = $(this).data('div-id');\r\n");
            sb.Append("     var current = $('#' + divId).css('display');\r\n");
            sb.Append("     if (current == 'none') {\r\n");
            sb.Append("         $('#' + divId).show('slow');\r\n");
            sb.Append("         icon.removeClass('fa fa-plus').addClass('fa fa-minus');\r\n");
            sb.Append("     } else {\r\n");
            sb.Append("         $('#' + divId).hide('slow');\r\n");
            sb.Append("         icon.removeClass('fa fa-minus').addClass('fa fa-plus');\r\n");
            sb.Append("     }\r\n");
            sb.Append("     return false;\r\n");
            sb.Append("});\r\n\r\n");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);

        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }

        protected void btnNovoFerias_Click(object sender, EventArgs e)
        {
            hddidControleFerias.Value = "0";
            hddidColaborador.Value = "0";
            AlterarVisualizacao_Edicao(true);
            LimpaCampos();
            PopulaCombo();
            txtidControleFerias.Text = "Novo";
            aba_historico.Visible = false;
        }

        protected void gv_Historico_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[3].Text = HttpUtility.HtmlDecode(e.Row.Cells[3].Text);
            }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            if (ValidarDados())
            {
                try
                {
                    DataSet ds;
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idControleFerias", hddidControleFerias.Value},
                        { "@idColaborador", hddidColaborador.Value == "0" ? ddlidColaborador.SelectedValue : hddidColaborador.Value },
                        { "@idTipoEvento", ddlidTipo.SelectedValue },
                        { "@dtEvento", txtdtEvento.Text },
                        { "@sDescricao", txtsDescricao.Text },
                        { "@nDias", txtnQtdFerias.Text },
                        { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() }
                    };
                    ds = BD.ExecutarDataSet(sProcedure, vParametros);
                    if (BD.ValidarDataSet(ds, out string sErro))
                    {
                        hddidControleFerias.Value = Retorno.DATASET(ds, 0, "idControleFerias");

                        MensagemPaginaDetalhe.MostraMensagem_Sucesso("Controle de Férias gravado com sucesso");
                        FeriasDetalhe(hddidControleFerias.Value);
                    }
                    else
                    {
                        throw new Exception("BD: " + sErro.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MensagemPaginaDetalhe.MostraMensagem_Erro(ex.Message);
                }
            }
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            AlterarVisualizacao_Edicao(false);
            Pesquisar();
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var colaborador = (cls_Ferias)e.Row.DataItem;
                var gvDetalhe = (GridView)e.Row.FindControl("gv_FeriasDetalhe");

                if (gvDetalhe != null)
                {
                    gvDetalhe.DataSource = colaborador.ls_FeriasDetalhes;
                    gvDetalhe.DataBind();
                }
            }
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "gvMainTh";
        }

        protected void gv_FeriasDetalhe_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = "gvMainTd";
            }
            else if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "gvChildHeader2";
        }

        public string NovaLinha(object id, string gridNome)
        {
            /* 
            * Passo a passo:
            * 1. Fecha a célula atual
            * 2. Fecha a linha Atual
            * 3. Cria uma nova linha com o ID e a classe <TR id='...' style='...'>
            * 4. Cria uma célula em branco: <TD></TD>
            * 5. Cria uma nova célula para conter o gridview
            ************************************************************/
            if (id != null && !string.IsNullOrEmpty(id.ToString()))
            {
                // Se houver um ID, retorna a nova linha com o ID e a classe
                return string.Format(@"</td></tr><tr id='tr{0}{1}' class='collapsed-row'>
                               <td></td><td colspan='100' style='padding:0px; margin:0px;'>", gridNome, id);
            }
            else
            {
                // Se não houver ID, retorna uma string vazia para que nada seja renderizado e o botão de colapso desapareça
                return string.Empty;
            }
        }
    }

    [Serializable]
    public class cls_FeriasDetalhe
    {
        public int idControleFerias { get; set; }
        public int idColaborador { get; set; }
        public string dtEvento { get; set; }
        public string sDescricao { get; set; }
        public string sDscColaborador { get; set; }
        public int nSaldo { get; set; }
        public int nDias { get; set; }
        public string sDscEvento { get; set; }
    }

    [Serializable]
    public class cls_Ferias
    {
        public int idColaborador { get; set; }
        public string sDscColaborador { get; set; }
        public int nSaldo { get; set; }
        public List<cls_FeriasDetalhe> ls_FeriasDetalhes { get; set; } = new List<cls_FeriasDetalhe>();
    }
}