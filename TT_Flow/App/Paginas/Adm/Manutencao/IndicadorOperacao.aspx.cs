using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT_Flow.App.Controles;
using TT.FrameWork;
using static TT.FrameWork.BD;
using static TT.FrameWork.BD.Retorno;
using static TT.FrameWork.Funcoes;
using static TT.FrameWork.Grid;
using static TT.FrameWork.Identity;

namespace TT_Flow.App.Paginas.Adm.Manutencao
{
    public partial class IndicadorOperacao : Page
    {
        #region | Propriedades

        string sProcedure = "sp_Manipula_tbl_Flow_Adm_IndicadorOperacao";
        int colunaID = 0;
        int colunaData = 6;
        int colunaExcluir = 8;

        #endregion

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            ValidaPermissao(Permissao.Administracao.IndicadorOperacao.Consultar, true);
            cmdNovo.Visible = ValidaPermissao(Permissao.Administracao.IndicadorOperacao.Incluir);

            if (!IsPostBack)
                Pesquisar(Request.GetValue("id"));
        }

        protected void Pesquisar(string sidIndOp = "")
        {
            ltrOcultarColunas.Visible = false;
            pnResultado.Visible = false;

            hddidIndOp.Value = sidIndOp;

            Dictionary<string, string> vParam = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sPesquisa", txtPesquisa.Text },
                { "@sStatus", ddlStatus.SelectedValue }
            };
            DataSet ds = ExecutarDataSet(sProcedure, vParam);

            if (ValidarDataSet(ds))
            {
                ltrOcultarColunas.Visible = true;
                pnResultado.Visible = true;

                bool bAltera = ValidaPermissao(Permissao.Administracao.IndicadorOperacao.Alterar);
                cmdAtivar.Visible = ddlStatus.SelectedValue == "N" && bAltera;
                cmdExcluir.Visible = ddlStatus.SelectedValue == "S" && bAltera;

                List<int> ocultar = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
                List<int> ocultosInicial = new List<int> { colunaID };

                if (!ValidaPermissao(Permissao.Administracao.IndicadorOperacao.Alterar))
                {
                    ocultar.Remove(colunaExcluir);
                    ocultosInicial.Add(colunaExcluir);
                }

                ScriptManager.RegisterStartupScript(Page, GetType(), "js_DataTable", DataBindComScriptData(gvIndOp, ds.Tables[0], 1, new int[1] { colunaData }, new int[1] { colunaExcluir }, "asc", ltrOcultarColunas, ocultar, ocultosInicial), true);
            }
            else MensagemPagina.MostraMensagem_Erro("Nenhum registro encontrado!");

            Scripts.FecharModal(Page, "modalIndOp");

            if (int.TryParse(sidIndOp, out int idIndOp))
            {
                if (idIndOp > 0)
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idIndOp", sidIndOp }
                    };
                    DataSet dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

                    if (ValidarDataSet(dsPesquisa, out _))
                    {
                        string sCodigo = DATASET(dsPesquisa, "nCodigo");
                        lblTitulo_ModalIndOp.InnerText = $"Indicador de Operação - {sCodigo}";

                        txtidIndOp.Text = DATASET(dsPesquisa, "idIndOp");
                        txtCodigo.Text = sCodigo;
                        txtsTipo.Text = DATASET(dsPesquisa, "sDscTipo");
                        txtsLocal.Text = DATASET(dsPesquisa, "sDscLocal");
                        txtsFornecimento.Text = DATASET(dsPesquisa, "sDscFornecimento");
                        txtsLocal_DFe.Text = DATASET(dsPesquisa, "sDscLocal_DFe");

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(DATASET(dsPesquisa, "dtAtualizacao"), DATASET(dsPesquisa, "sUsuarioAtualizacao"));

                        Scripts.AbrirModal(Page, "modalIndOp");
                    }
                }
                else
                {
                    lblTitulo_ModalIndOp.InnerText = "Novo Indicador de Operação";

                    txtidIndOp.Text = "Novo";
                    txtCodigo.Text = "";
                    txtsTipo.Text = "";
                    txtsLocal.Text = "";
                    txtsFornecimento.Text = "";
                    txtsLocal_DFe.Text = "";

                    PainelAtualizacao.Visible = false;

                    Scripts.AbrirModal(Page, "modalIndOp");
                }
            }
        }

        protected void Salvar()
        {
            try
            {
                if (txtCodigo.Text.Length != 6)
                    MensagemPagina_ModalIndOp.MostraMensagem_Erro("É obrigatório preencher o Código!");
                else if (string.IsNullOrWhiteSpace(txtsTipo.Text) || string.IsNullOrWhiteSpace(txtsLocal.Text) || string.IsNullOrWhiteSpace(txtsFornecimento.Text) || string.IsNullOrWhiteSpace(txtsLocal_DFe.Text))
                    MensagemPagina_ModalIndOp.MostraMensagem_Erro("É obrigatório preencher o Tipo de Operação, o Local da Operação, a Característica do Fornecimento e o Local do fornecimento em DFe!");
                else
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idIndOp", hddidIndOp.Value },
                        { "@nCodigo", txtCodigo.Text },
                        { "@sDscTipo", txtsTipo.Text },
                        { "@sDscLocal", txtsLocal.Text },
                        { "@sDscFornecimento", txtsFornecimento.Text },
                        { "@sDscLocal_DFe", txtsLocal_DFe.Text },
                        { "@idUsuario", Variaveis.idUsuario() }
                    };
                    ExecutarDataSet(sProcedure, vParametros);

                    Pesquisar();
                    MensagemPagina.MostraMensagem_Sucesso("Indicador de Operação salvo com sucesso!");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_ModalIndOp.MostraMensagem_Erro(ex.Message);
            }
        }

        protected void Excluir()
        {
            try
            {
                foreach (GridViewRow row in gvIndOp.Rows)
                {
                    if (row.FindControl("chkExcluir") is CheckBox chkExcluir && chkExcluir.Checked)
                    {
                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "EXCLUIR" },
                            { "@idIndOp", (row.FindControl("lnkID") as LinkButton).Text },
                            { "@idUsuario", Variaveis.idUsuario() }
                        };
                        ExecutarDataSet(sProcedure, vParametros);
                    }
                }

                MensagemPagina.MostraMensagem_Sucesso("Indicadores de Operação excluídos com sucesso!");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }

            Pesquisar();
        }

        protected void Ativar()
        {
            try
            {
                foreach (GridViewRow row in gvIndOp.Rows)
                {
                    if (row.FindControl("chkExcluir") is CheckBox chkExcluir && chkExcluir.Checked)
                    {
                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "ATIVAR" },
                            { "@idIndOp", (row.FindControl("lnkID") as LinkButton).Text },
                            { "@idUsuario", Variaveis.idUsuario() }
                        };
                        ExecutarDataSet(sProcedure, vParametros);
                    }
                }

                MensagemPagina.MostraMensagem_Sucesso("Indicadores de Operação ativados com sucesso!");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }

            Pesquisar();
        }

        #endregion

        #region | Eventos

        protected void gvIndOp_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            bool bAtivos = ddlStatus.SelectedValue == "S";

            if (e.Row.RowType == DataControlRowType.Header)
            {
                if (e.Row.FindControl("chkExcluir_Todos") is CheckBox excluir)
                    excluir.CssClass = bAtivos ? excluir.CssClass.Replace("success", "danger").Replace("-v", "-x") : excluir.CssClass.Replace("danger", "success").Replace("-x", "-v");
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = bAtivos ? "" : "danger";

                if (e.Row.FindControl("chkExcluir") is CheckBox excluir)
                    excluir.CssClass = bAtivos ? excluir.CssClass.Replace("success", "danger").Replace("-v", "-x") : excluir.CssClass.Replace("danger", "success").Replace("-x", "-v");
            }
        }

        protected void gvIndOp_RowCommand(object sender, GridViewCommandEventArgs e) => Pesquisar(e.CommandArgument.ToString());

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void cmdNovo_Click(object sender, EventArgs e) => Pesquisar("0");

        protected void cmdSalvar_Click(object sender, EventArgs e) => Salvar();

        protected void cmdCancelar_Click(object sender, EventArgs e) => Pesquisar();

        protected void cmdExcluir_Click(object sender, EventArgs e) => Excluir();

        protected void cmdAtivar_Click(object sender, EventArgs e) => Ativar();

        #endregion
    }
}