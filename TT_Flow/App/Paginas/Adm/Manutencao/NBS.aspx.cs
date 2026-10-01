using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT_Flow.App.Controles;
using TT.FrameWork;
using static TT.FrameWork.BD;
using static TT.FrameWork.Funcoes;
using static TT.FrameWork.Grid;
using static TT.FrameWork.Identity;

namespace TT_Flow.App.Paginas.Adm.Manutencao
{
    public partial class NBS : Page
    {
        #region | Propriedades

        string sProcedure = "sp_Manipula_tbl_Flow_Adm_NBS";
        int colunaData = 3;
        int colunaExcluir = 5;

        #endregion

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            ValidaPermissao(Permissao.Administracao.NBS.Consultar, true);
            cmdNovo.Visible = ValidaPermissao(Permissao.Administracao.NBS.Incluir);

            if (!IsPostBack)
                Pesquisar(Request.GetValue("id"));

            RegistraScript();
        }

        protected void Pesquisar(string sidNBS = "")
        {
            ltrOcultarColunas.Visible = false;
            pnResultado.Visible = false;

            hddidNBS.Value = sidNBS;

            Dictionary<string, string> vParam = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDescricao", txtPesquisa.Text },
                { "@sStatus", ddlStatus.SelectedValue }
            };
            DataSet ds = ExecutarDataSet(sProcedure, vParam);

            if (ValidarDataSet(ds))
            {
                ltrOcultarColunas.Visible = true;
                pnResultado.Visible = true;

                bool bAltera = ValidaPermissao(Permissao.Administracao.NBS.Alterar);
                cmdAtivar.Visible = ddlStatus.SelectedValue == "N" && bAltera;
                cmdExcluir.Visible = ddlStatus.SelectedValue == "S" && bAltera;

                List<int> ocultar = new List<int> { 0, 1, 2, 3, 4, 5 };
                List<int> ocultosInicial = null;

                if (!ValidaPermissao(Permissao.Administracao.NBS.Alterar))
                {
                    ocultar.Remove(colunaExcluir);
                    ocultosInicial = new List<int> { colunaExcluir };
                }

                ScriptManager.RegisterStartupScript(Page, GetType(), "js_DataTable", DataBindComScriptData(gvNBS, ds.Tables[0], 1, new int[1] { colunaData }, new int[1] { colunaExcluir }, "asc", ltrOcultarColunas, ocultar, ocultosInicial), true);
            }
            else MensagemPagina.MostraMensagem_Erro("Nenhum registro encontrado!");
        }

        protected void Salvar()
        {
            try
            {
                if (txtCodigo.Text.Length != 12 || string.IsNullOrEmpty(txtsDescricao.Text))
                    throw new Exception("É obrigatório preencher o Código (9 dígitos) e a Descrição!");
                else
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idNBS", hddidNBS.Value },
                        { "@nCodigo", txtCodigo.Text.Replace(".", "") },
                        { "@sDescricao", txtsDescricao.Text },
                        { "@idUsuario", Variaveis.idUsuario() }
                    };
                    ExecutarDataSet(sProcedure, vParametros);

                    Pesquisar();
                    PainelAtualizacao.Visible = true;

                    MensagemPagina.MostraMensagem_Sucesso("NBS salvo com sucesso!");
                }
            }
            catch (Exception ex)
            {
                txtidNBS.Text = hddidNBS.Value;
                PainelAtualizacao.Visible = false;

                MensagemPagina_ModalNBS.MostraMensagem_Erro(ex.Message);
                Scripts.AbrirModal(Page, "modalNBS");
            }
        }

        protected void Excluir()
        {
            try
            {
                foreach (GridViewRow row in gvNBS.Rows)
                {
                    if (row.FindControl("chkExcluir") is CheckBox chkExcluir && chkExcluir.Checked)
                    {
                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "EXCLUIR" },
                            { "@idNBS", (row.FindControl("lnkID") as LinkButton).Text },
                            { "@idUsuario", Variaveis.idUsuario() }
                        };
                        ExecutarDataSet(sProcedure, vParametros);
                    }
                }

                MensagemPagina.MostraMensagem_Sucesso("NBS excluídos com sucesso!");
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
                foreach (GridViewRow row in gvNBS.Rows)
                {
                    if (row.FindControl("chkExcluir") is CheckBox chkExcluir && chkExcluir.Checked)
                    {
                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "ATIVAR" },
                            { "@idNBS", (row.FindControl("lnkID") as LinkButton).Text },
                            { "@idUsuario", Variaveis.idUsuario() }
                        };
                        ExecutarDataSet(sProcedure, vParametros);
                    }
                }

                MensagemPagina.MostraMensagem_Sucesso("NBS ativados com sucesso!");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }

            Pesquisar();
        }

        #endregion

        #region | Eventos

        protected void gvNBS_RowDataBound(object sender, GridViewRowEventArgs e)
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

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void cmdNovo_Click(object sender, EventArgs e) => Pesquisar("0");

        protected void cmdSalvar_Click(object sender, EventArgs e) => Salvar();

        protected void cmdExcluir_Click(object sender, EventArgs e) => Excluir();

        protected void cmdAtivar_Click(object sender, EventArgs e) => Ativar();

        #endregion

        #region | Script

        protected void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("function AbrirModal_Detalhe($this) {");
            sb.AppendLine("     if ($this) {");
            sb.AppendLine("         const row = $this.closest('tr');");
            sb.AppendLine("         $('.sTituloNBS').text('NBS - ' + row.find('.lnkNBS').text());");
            sb.AppendLine($"        $('#{hddidNBS.ClientID}').val(row.find('.lnkID').text());");
            sb.AppendLine("         $('.sIdNBS').val(row.find('.lnkID').text());");
            sb.AppendLine("         $('.sCodigoNBS').val(row.find('.lnkNBS').text());");
            sb.AppendLine("         $('.sDescricaoNBS').val(row.find('.lnkDescricao').text());");
            sb.AppendLine("         $('#div_PainelAtualizacao').css('display', 'block');");
            sb.AppendLine("         $('#div_PainelAtualizacao').find('[id*=lbldtAtualizacao]').text(row.find('.lblData').text());");
            sb.AppendLine("         $('#div_PainelAtualizacao').find('[id*=lblsDscUsuarioAtualizacao]').text(row.find('.lblUsuario').text());");
            sb.AppendLine("     } else {");
            sb.AppendLine("         $('.sTituloNBS').text('Novo NBS');");
            sb.AppendLine("         $('.sIdNBS').val('Novo');");
            sb.AppendLine($"        $('#{hddidNBS.ClientID}').val('0');");
            sb.AppendLine("         $('.sCodigoNBS').val('');");
            sb.AppendLine("         $('.sDescricaoNBS').val('');");
            sb.AppendLine("         $('#div_PainelAtualizacao').css('display', 'none');");
            sb.AppendLine("     }");
            sb.AppendLine("     $('#modalNBS').modal('show');");
            sb.AppendLine("}");

            ScriptManager.RegisterStartupScript(Page, GetType(), "js_RegistraScript", sb.ToString(), true);
        }

        #endregion
    }
}