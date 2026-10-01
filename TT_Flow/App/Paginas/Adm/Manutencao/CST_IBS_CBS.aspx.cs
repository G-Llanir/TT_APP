using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Controles;
using TT_Flow.FrameWork;
using static TT.FrameWork.BD;
using static TT.FrameWork.BD.Retorno;
using static TT.FrameWork.Funcoes;
using static TT.FrameWork.Grid;
using static TT.FrameWork.Identity;

namespace TT_Flow.App.Paginas.Adm.Manutencao
{
    public partial class CST_IBS_CBS : Page
    {
        #region | Propriedades

        string sProcedure = "sp_Manipula_tbl_Flow_Adm_CST_IBS_CBS";
        int colunaEditar = 6;
        int colunaExcluir = 7;

        public List<cls_CST_IBS_CBS_Filho> lstFilhos
        {
            get
            {
                if (ViewState["lstFilhos"] == null) ViewState["lstFilhos"] = new List<cls_CST_IBS_CBS_Filho>();
                return (List<cls_CST_IBS_CBS_Filho>)ViewState["lstFilhos"];
            }
            set => ViewState["lstFilhos"] = value;
        }

        #endregion

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            ValidaPermissao(Permissao.Administracao.CST_IBS_CBS.Consultar, true);
            cmdNovo.Visible = ValidaPermissao(Permissao.Administracao.CST_IBS_CBS.Incluir);

            if (!IsPostBack) Pesquisar(Request.GetValue("id"));            
        }

        protected void Pesquisar(string sidCST = "")
        {
            div_gvCST.Visible = false;
            hddidCST.Value = sidCST;

            Dictionary<string, string> vParam = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sPesquisa", txtPesquisa.Text }
            };
            DataSet ds = ExecutarDataSet(sProcedure, vParam);

            if (ValidarDataSet(ds))
            {
                div_gvCST.Visible = true;
                ScriptManager.RegisterStartupScript(Page, GetType(), "js_DataTables", DataBindComScriptData(gvCST, ds.Tables[0], 0, new int[3], "asc", "''", "''"), true);
            }
            else MensagemPagina.MostraMensagem_Erro("Nenhum registro encontrado!");

            Scripts.FecharModal(Page, "modalCST");

            if (int.TryParse(sidCST, out int idCST))
            {
                if (idCST > 0) // Detalhe
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idCST", sidCST }
                    };
                    DataSet dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

                    if (ValidarDataSet(dsPesquisa, out _))
                    {
                        string sCodigo = DATASET(dsPesquisa, "sCodigo"), sDescricao = DATASET(dsPesquisa, "sDescricao");
                        lblTitulo_ModalCST.InnerText = $"{sCodigo} - {sDescricao}";

                        txtidCST.Text = DATASET(dsPesquisa, "idCST");
                        txtCodigo.Text = sCodigo;
                        txtsDescricao.Text = sDescricao;
                        sExigeTrib.Definir(DATASET(dsPesquisa, "sExigeTrib"), "Exige Tributação?", "N");
                        sReducaoBC.Definir(DATASET(dsPesquisa, "sReducaoBC"), "Redução BC?", "N");
                        sReducaoAliq.Definir(DATASET(dsPesquisa, "sReducaoAliq"), "Redução Alíquota?", "N");

                        PopulaFilhos(dsPesquisa.Tables[1]);

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(DATASET(dsPesquisa, "dtAtualizacao"), DATASET(dsPesquisa, "sUsuarioAtualizacao"));

                        Scripts.AbrirModal(Page, "modalCST");
                    }
                }
                else // Novo
                {
                    lblTitulo_ModalCST.InnerText = "Novo CST";

                    txtidCST.Text = "Novo";
                    txtCodigo.Text = "";
                    txtsDescricao.Text = "";
                    sExigeTrib.Definir("N", "Exige Tributação?", "N");
                    sReducaoBC.Definir("N", "Redução BC?", "N");
                    sReducaoAliq.Definir("N", "Redução Alíquota?", "N");

                    lstFilhos.Clear();
                    gvFilhos_DataBind();

                    PainelAtualizacao.Visible = false;

                    Scripts.AbrirModal(Page, "modalCST");
                }
            }
        }

        protected void Salvar()
        {
            try
            {
                if (txtCodigo.Text.Length != 3 || string.IsNullOrEmpty(txtsDescricao.Text))
                    MensagemPagina_ModalCST.MostraMensagem_Erro("É obrigatório preencher o Código (xxx) e a Descrição!");
                else
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idCST", hddidCST.Value },
                        { "@nCodigo", txtCodigo.Text },
                        { "@sDescricao", txtsDescricao.Text },
                        { "@sExigeTrib", sExigeTrib.Recuperar() },
                        { "@sReducaoBC", sReducaoBC.Recuperar() },
                        { "@sReducaoAliq", sReducaoAliq.Recuperar() },
                        { "@idUsuario", Variaveis.idUsuario() }
                    };
                    DataSet dsPesquisa = ExecutarDataSet(sProcedure, vParametros);

                    if (ValidarDataSet(dsPesquisa, out _) && SalvarFilhos())
                    {
                        Pesquisar();
                        MensagemPagina.MostraMensagem_Sucesso("CST salvo com sucesso!");
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_ModalCST.MostraMensagem_Erro(ex.Message);
            }
        }

        protected bool SalvarFilhos()
        {
            try
            {
                foreach (var filho in lstFilhos)
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", filho.bExcluir ? "EXCLUIR_FILHO" : "SALVAR_FILHO" },
                        { "@idCST_Filho", filho.idCST_Filho.ToString() },
                        { "@idCST", hddidCST.Value },
                        { "@nCodigo", filho.nCodigo.ToString() },
                        { "@sDescricao", filho.sDescricao },
                        { "@idTipo", filho.idTipoAliq.ToString() },
                        { "@nRedIBS", filho.nRedIBS.ToString().StringToDecimalString() },
                        { "@nRedCBS", filho.nRedCBS.ToString().StringToDecimalString() }
                    };
                    ExecutarDataSet(sProcedure, vParametros);
                }

                return true;
            }
            catch
            {
                MensagemPagina_ModalCST.MostraMensagem_Erro("Ocorreu um erro ao salvar as Classificações!");
                return false;
            }
        }

        #endregion

        #region | Filhos

        protected void PopulaFilhos(DataTable tb)
        {
            lstFilhos.Clear();

            foreach (DataRow row in tb.Rows)
            {
                lstFilhos.Add(new cls_CST_IBS_CBS_Filho
                {
                    idCST_Filho = Convert.ToInt32(row["idCST_Filho"]),
                    nCodigo = Convert.ToInt32(row["nCodigo"]),
                    sDescricao = row["sDescricao"].ToString(),
                    idTipoAliq = Convert.ToInt32(row["idTipoAliq"]),
                    sTipoAliq = row["sTipoAliq"].ToString(),
                    nRedIBS = Convert.ToDecimal(row["nRedIBS"]),
                    nRedCBS = Convert.ToDecimal(row["nRedCBS"]),
                    bEditar = false,
                    bExcluir = false
                });
            }

            gvFilhos_DataBind();
        }

        protected void gvFilhos_DataBind()
        {
            div_gvFilhos.Visible = lstFilhos.Any(f => !f.bEditar && !f.bExcluir);
            ScriptManager.RegisterStartupScript(Page, GetType(), "js_DataTables_Filhos", DataBindComScript(gvFilhos, lstFilhos.Where(f => !f.bEditar && !f.bExcluir), 1, new int[] { colunaEditar, colunaExcluir }), true);
        }

        protected int NovoID() => lstFilhos.Any() ? lstFilhos.Max(f => f.idCST_Filho) + 1 : 1;

        protected void gvFilhos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var filho = lstFilhos.FirstOrDefault(f => f.idCST_Filho.ToString() == e.CommandArgument.ToString());

            if (filho != null)
            {
                hddidFilho.Value = filho.idCST_Filho.ToString();

                txtnCodigo_Filho.Text = filho.nCodigo.ToString().PadLeft(6, '0');
                txtsDescricao_Filho.Text = filho.sDescricao;
                ddlidTipoAliq.SelectedValue = filho.idTipoAliq.ToString();
                txtRedIBS.Text = filho.nRedIBS.ToString("N2");
                txtRedCBS.Text = filho.nRedCBS.ToString("N2");

                filho.bEditar = true;
            }
            else MensagemPagina_ModalCST.MostraMensagem_Erro("Não foi possível encontrar a Classificação para Editar!");

            gvFilhos_DataBind();

            ScriptManager.RegisterStartupScript(Page, GetType(), "js_AjustaDIV", "$('#divIncluirFilho').addClass('in');", true);
        }

        protected void lnkIncluirFilho_Click(object sender, EventArgs e)
        {
            decimal.TryParse(txtRedIBS.Text, out decimal nRedIBS);
            decimal.TryParse(txtRedCBS.Text, out decimal nRedCBS);

            if (txtnCodigo_Filho.Text.Length != 6 || string.IsNullOrEmpty(txtsDescricao_Filho.Text))
                MensagemPagina_ModalCST.MostraMensagem_Erro("É obrigatório preencher o Código (xxxxxx) e a Descrição da Classificação!");
            else if (!int.TryParse(hddidFilho.Value, out int idFilho) || idFilho == 0)
            {
                string sTipoAliq = "Fixa";

                switch (ddlidTipoAliq.SelectedValue)
                {
                    case "2": sTipoAliq = "Padrão"; break;
                    case "3": sTipoAliq = "Sem Alíquota"; break;
                    case "4": sTipoAliq = "Uniforme Nacional"; break;
                    case "5": sTipoAliq = "Uniforme Setorial"; break;
                }

                lstFilhos.Add(new cls_CST_IBS_CBS_Filho
                {
                    idCST_Filho = NovoID(),
                    nCodigo = Convert.ToInt32(txtnCodigo_Filho.Text),
                    sDescricao = txtsDescricao_Filho.Text,
                    idTipoAliq = Convert.ToInt32(ddlidTipoAliq.SelectedValue),
                    sTipoAliq = sTipoAliq,
                    nRedIBS = nRedIBS,
                    nRedCBS = nRedCBS,
                    bEditar = false,
                    bExcluir = false
                });

                txtnCodigo_Filho.Text = "";
                txtsDescricao_Filho.Text = "";
                ddlidTipoAliq.SelectedValue = "2";
                txtRedIBS.Text = "";
                txtRedCBS.Text = "";
            }
            else
            {
                var filho = lstFilhos.FirstOrDefault(f => f.idCST_Filho == idFilho);

                if (filho != null)
                {
                    filho.nCodigo = Convert.ToInt32(txtnCodigo_Filho.Text);
                    filho.sDescricao = txtsDescricao_Filho.Text;
                    filho.idTipoAliq = Convert.ToInt32(ddlidTipoAliq.SelectedValue);
                    filho.nRedIBS = nRedIBS;
                    filho.nRedCBS = nRedCBS;
                    filho.bEditar = false;

                    txtnCodigo_Filho.Text = "";
                    txtsDescricao_Filho.Text = "";
                    ddlidTipoAliq.SelectedValue = "2";
                    txtRedIBS.Text = "";
                    txtRedCBS.Text = "";
                }
                else MensagemPagina_ModalCST.MostraMensagem_Erro("Ocorreu um erro ao editar a Classificação!");

                hddidFilho.Value = "0";
            }

            gvFilhos_DataBind();

            ScriptManager.RegisterStartupScript(Page, GetType(), "js_AjustaDIV", "$('#divIncluirFilho').addClass('in');", true);
        }

        protected void lnkExcluir_Filhos_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (GridViewRow row in gvFilhos.Rows)
                {
                    if ((row.FindControl("chkExcluir") as CheckBox).Checked)
                        lstFilhos.First(f => f.idCST_Filho.ToString() == row.Cells[0].Text).bExcluir = true;
                }
            }
            catch
            {
                MensagemPagina_ModalCST.MostraMensagem_Erro("Ocorreu um erro ao excluir as Classificações!");
            }

            gvFilhos_DataBind();

            if (hdd_divIncluirFilho.Value == "S")
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "js_AjustaDIV", "$('#divIncluirFilho').addClass('in');", true);
                hdd_divIncluirFilho.Value = "N";
            }
        }

        #endregion

        #region | Eventos

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void cmdNovo_Click(object sender, EventArgs e) => Pesquisar("0");

        protected void gvCST_RowCommand(object sender, GridViewCommandEventArgs e) => Pesquisar(e.CommandArgument.ToString());

        protected void cmdSalvar_Click(object sender, EventArgs e) => Salvar();

        protected void cmdCancelar_Click(object sender, EventArgs e) => Pesquisar();

        #endregion
    }
}