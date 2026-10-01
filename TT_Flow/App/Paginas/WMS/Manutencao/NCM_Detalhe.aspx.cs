using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Flow.App.Paginas.WMS.Manutencao
{
    public partial class NCM_Detalhe : Page
    {
        string sTituloPagina = "NCM";
        string sProcedure = "sp_Manipula_tbl_Flow_WMS_NCM";

        #region | Page_Load + Pesquisar

        private void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["id"] != "0" && Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.NCM.Consultar, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.NCM.Incluir, true);
                    Pesquisar("0");
                }

                if (Request["msg"] != null && Request["msg"] == "1")
                    MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso!");
            }

            RegistraScript();
        }

        private void Pesquisar(string idPesquisa)
        {
            try
            {
                aba_Historico.Visible = false;
                div_cmdCancelarEdicao.Visible = false;

                LimpaCampos();
                FUNCOES.Popula_Combo(lstCEST, "sp_Select 'Flow_WMS_Produtos_CEST'", "idCEST", "sCodigoCEST", false);
                FUNCOES.Popula_Combo(ddlOrigem, "sp_Select 'UF'", "sEstado", "sDscEstado", false, "UF Origem", "");
                FUNCOES.Popula_Combo(ddlDestino, "sp_Select 'UF'", "sEstado", "sDscEstado", false, "UF Destino", "");
                ddlDestino.Items.Add(new ListItem("DIFERENTE DA ORIGEM", "DI"));

                if (idPesquisa != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idNCM", idPesquisa }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        Popula_AbaHistorico(dsPesquisa.Tables[3]);
                        Popula_Regras(dsPesquisa.Tables[4]);

                        gvHistorico_Regras.DataSource = dsPesquisa.Tables[5];
                        gvHistorico_Regras.DataBind();

                        cmdModal_Historico_Regras.Visible = false;
                        if (dsPesquisa.Tables[6].Rows.Count > 1)
                        {
                            cmdModal_Historico_Regras.Visible = true;

                            string statusAnterior = "1", grupo = "<div class='btn-group checkList' style='margin-right: 5px;'>";
                            litFiltro_Historico.Text += grupo;
                            foreach (DataRow row in dsPesquisa.Tables[6].Rows)
                            {
                                if (row["idStatus"].ToString() != statusAnterior)
                                    litFiltro_Historico.Text += $"</div>{grupo}";

                                litFiltro_Historico.Text += row["sDescricao"];

                                statusAnterior = row["idStatus"].ToString();
                            }
                            litFiltro_Historico.Text += "</div>";
                        }

                        hddidNCM.Value = RETORNO.DATASET(dsPesquisa, "idNCM");
                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.NCM.Alterar, false);

                        txtidNCM.Text = RETORNO.DATASET(dsPesquisa, "idNCM");
                        txtsCodigoNCM.Text = RETORNO.DATASET(dsPesquisa, "sCodigoNCM");
                        ddlAntidamping.SelectedValue = RETORNO.DATASET(dsPesquisa, "sAntidamping");
                        ddlLicencaImportacao.SelectedValue = RETORNO.DATASET(dsPesquisa, "sLiImportacao");
                        Switch_ReducaoBC.Definir(RETORNO.DATASET(dsPesquisa, "sRedBC"), "Redução na Base de Cálculo do ICMS?", "N");
                        txtsDscNCM.Text = RETORNO.DATASET(dsPesquisa, "sDscNCM");
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, "sSituacao"));

                        // Seleciona CESTs Salvos
                        foreach (DataRow row in dsPesquisa.Tables[2].Rows)
                        {
                            try
                            {
                                ddlCEST_Principal.Items.Add(new ListItem(row["sCodigoCEST"].ToString(), row["idCEST"].ToString()));

                                if (lstCEST.Items.FindByValue(row["idCEST"].ToString()) != null) lstCEST.Items.FindByValue(row["idCEST"].ToString()).Selected = true;
                            }
                            catch { }
                        }

                        ddlCEST_Principal.SelectedValue = RETORNO.DATASET(dsPesquisa, "idCEST");

                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsCodigoNCM.Text);
                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, "sDscUsuarioAtualizacao"));

                        txtnII.Text = RETORNO.DATASET(dsPesquisa, 1, 0, "nII");
                        txtnIPI.Text = RETORNO.DATASET(dsPesquisa, 1, 0, "nIPI");
                        txtnPIS.Text = RETORNO.DATASET(dsPesquisa, 1, 0, "nPIS");
                        txtnCOFINS.Text = RETORNO.DATASET(dsPesquisa, 1, 0, "nCOFINS");
                        txtnICMS.Text = RETORNO.DATASET(dsPesquisa, 1, 0, "nICMS");
                        txtnII_Internacional.Text = RETORNO.DATASET(dsPesquisa, 1, 0, "nII_Internacional");
                        txtnIPI_Internacional.Text = RETORNO.DATASET(dsPesquisa, 1, 0, "nIPI_Internacional");
                        txtnPIS_Internacional.Text = RETORNO.DATASET(dsPesquisa, 1, 0, "nPIS_Internacional");
                        txtnCOFINS_Internacional.Text = RETORNO.DATASET(dsPesquisa, 1, 0, "nCOFINS_Internacional");
                        txtnICMS_Internacional.Text = RETORNO.DATASET(dsPesquisa, 1, 0, "nICMS_Internacional");
                    }
                    else throw new Exception(sErro);
                }

                txtsCodigoNCM.Focus();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        #endregion

        #region | Aba Histórico

        void Popula_AbaHistorico(DataTable dt)
        {
            gv_Historico.DataSource = dt;
            gv_Historico.DataBind();

            aba_Historico.Visible = gv_Historico.Rows.Count > 0;
        }

        protected void gv_Historico_RowDataBound(object sender, GridViewRowEventArgs e) => e.Row.Cells[1].Text = HttpUtility.HtmlDecode(e.Row.Cells[1].Text);

        #endregion

        #region | Utils

        private void LimpaCampos()
        {
            txtidNCM.Text = "Novo";
            txtsDscNCM.Text = "";
            txtsCodigoNCM.Text = "";
            hddidNCM.Value = "0";
            ddlAntidamping.SelectedValue = "0";
            ddlLicencaImportacao.SelectedValue = "0";
            Switch_ReducaoBC.Definir("N", "Redução na Base de Cálculo do ICMS?", "N");

            txtnII.Text = "0,00";
            txtnIPI.Text = "0,00";
            txtnPIS.Text = "0,00";
            txtnCOFINS.Text = "0,00";
            txtnICMS.Text = "0,00";
            txtnII_Internacional.Text = "0,00";
            txtnIPI_Internacional.Text = "0,00";
            txtnPIS_Internacional.Text = "0,00";
            txtnCOFINS_Internacional.Text = "0,00";
            txtnICMS_Internacional.Text = "0,00";

            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
        }

        private bool ValidarDados()
        {
            string sMsg = "";

            if (string.IsNullOrEmpty(txtsDscNCM.Text)) sMsg += "A Descrição é obrigatória!<br />";

            if (string.IsNullOrWhiteSpace(txtsCodigoNCM.Text))
            {
                sMsg += "O Código do NCM é obrigatório!<br />";
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(
                         txtsCodigoNCM.Text.Trim(),
                         @"^\d{4}\.\d{2}\.\d{2}$"))
            {
                sMsg += "O Código do NCM deve conter 8 números no formato 0000.00.00!<br />";
            }

            if (!lstCEST.Items.Cast<ListItem>().Any(c => c.Selected)) sMsg += "É obrigatório selecionar ao menos um CEST!<br />";
            if (txtnII.Text.Length < 1) sMsg += "Informe o Imposto de Importação (II) Nacional!<br />";
            if (txtnIPI.Text.Length < 1) sMsg += "Informe o Imposto sobre Produtos Industrializados (IPI) Nacional!<br />";
            if (txtnPIS.Text.Length < 1) sMsg += "Informe o Programa de Integração Social (PIS) Nacional!<br />";
            if (txtnCOFINS.Text.Length < 1) sMsg += "Informe a Contribuição para o Financiamento da Seguridade Social (COFINS) Nacional!<br />";
            if (txtnICMS.Text.Length < 1) sMsg += "Informe o Imposto sobre Circulação de Mercadorias e Serviços (ICMS) Nacional!<br />";
            if (txtnII_Internacional.Text.Length < 1) sMsg += "Informe o Imposto de Importação (II) Internacional!<br />";
            if (txtnIPI_Internacional.Text.Length < 1) sMsg += "Informe o Imposto sobre Produtos Industrializados (IPI) Internacional!<br />";
            if (txtnPIS_Internacional.Text.Length < 1) sMsg += "Informe o Programa de Integração Social (PIS) Internacional!<br />";
            if (txtnCOFINS_Internacional.Text.Length < 1) sMsg += "Informe o Contribuição para o Financiamento da Seguridade Social (COFINS) Internacional!<br />";
            if (txtnICMS_Internacional.Text.Length < 1) sMsg += "Informe o Imposto sobre Circulação de Mercadorias e Serviços (ICMS) Internacional!";

            if (!string.IsNullOrEmpty(sMsg))
            {
                MensagemPagina.MostraMensagem_Erro(sMsg.Trim());
                return false;
            }

            return true;
        }

        private string removeCaracteres(string sObjeto)
        {
            string sObjetoSemCaracter = new string(sObjeto
                .Where(c => Char.IsDigit(c) || c == ',' || c == '.')
                .Select(c => c == ',' ? '.' : c)
                .ToArray());

            if (sObjetoSemCaracter == "")
                sObjetoSemCaracter = "0";
            return sObjetoSemCaracter;
        }

        #endregion

        #region | Regras Fiscais

        #region | Classes

        [Serializable]
        public class clsRegras
        {
            public int idRegra { get; set; }
            public int idStatus { get; set; }
            public DateTime dtInicial { get; set; }
            public DateTime dtFinal { get; set; }
            public string sEstadoOrigem { get; set; }
            public string sDscEstadoOrigem { get; set; }
            public string sEstadoDestino { get; set; }
            public string sDscEstadoDestino { get; set; }
            public decimal nICMS { get; set; }
            public string sObs { get; set; }
        }
        public List<clsRegras> listRegras { get { if (ViewState["listRegras"] == null) ViewState["listRegras"] = new List<clsRegras>(); return (List<clsRegras>)ViewState["listRegras"]; } set { ViewState["listRegras"] = value; } }

        #endregion

        protected void LimpaCampos_Regra()
        {
            txtdtInicial.Text = "";
            txtdtFinal.Text = "";
            ddlOrigem.SelectedValue = "";
            ddlDestino.SelectedValue = "";
            txtnICMS_Regra.Text = "0,00";
            txtObs.Text = "";
        }

        protected void Popula_Regras(DataTable tb)
        {
            LimpaCampos_Regra();

            foreach (DataRow row in tb.Rows)
            {
                clsRegras regra = new clsRegras
                {
                    idRegra = Convert.ToInt32(row["idRegra_NCM"]),
                    idStatus = Convert.ToInt32(row["idStatus"]),
                    dtInicial = Convert.ToDateTime(row["dtInicial"]),
                    dtFinal = Convert.ToDateTime(row["dtFinal"]),
                    sEstadoOrigem = row["sEstadoOrigem"].ToString(),
                    sDscEstadoOrigem = row["sDscEstadoOrigem"].ToString(),
                    sEstadoDestino = row["sEstadoDestino"].ToString(),
                    sDscEstadoDestino = row["sDscEstadoDestino"].ToString(),
                    nICMS = Convert.ToDecimal(row["nICMS"]),
                    sObs = row["sObs"].ToString()
                };
                listRegras.Add(regra);
            }

            gvRegrasFicais_DataBind();
            hddidRegra_Editar.Value = "0";
        }

        protected void gvRegrasFicais_DataBind()
        {
            gvRegrasFiscais.DataSource = listRegras.Where(r => r.idStatus != 3).OrderBy(r => r.idRegra);
            gvRegrasFiscais.DataBind();
        }

        protected void gvRegrasFiscais_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string idStatus = DataBinder.Eval(e.Row.DataItem, "idStatus").ToString();
                e.Row.CssClass = idStatus == "1" ? "success" : "warning";
            }
        }

        protected void gvRegrasFiscais_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var regra = listRegras.FirstOrDefault(r => r.idRegra.ToString() == e.CommandArgument.ToString());
            if (regra != null)
            {
                regra.idStatus = 3;

                if (e.CommandName == "Editar")
                {
                    if (hddidRegra_Editar.Value != "0") cmdCancelarEdicao_Click(null, null);

                    div_cmdCancelarEdicao.Visible = true;

                    txtdtInicial.Text = regra.dtInicial.ToString("yyyy-MM-ddTHH:mm:ss");
                    txtdtFinal.Text = regra.dtFinal.ToString("yyyy-MM-ddTHH:mm:ss");
                    ddlOrigem.SelectedValue = regra.sEstadoOrigem;
                    ddlDestino.SelectedValue = regra.sEstadoDestino;
                    txtnICMS_Regra.Text = regra.nICMS.ToString();
                    txtObs.Text = regra.sObs;

                    hddidRegra_Editar.Value = e.CommandArgument.ToString();
                    cmdIncluirRegra.Text = "Editar Regra";

                    FUNCOES.Scripts.AbrirCollapse(Page, "div_IncluirRegra");
                }
                else MensagemPagina_Regras.MostraMensagem_Sucesso("Regra excluída com sucesso!");

                gvRegrasFicais_DataBind();
            }
        }

        protected bool Salvar_Regras(string idNCM)
        {
            foreach (var regra in listRegras)
            {
                if (regra.idRegra.ToString() == hddidRegra_Editar.Value) continue;

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", regra.idStatus == 3 ? "EXCLUIR_REGRA_FISCAL" : "SALVAR_REGRA_FISCAL" },
                    { "@idNCM", idNCM },
                    { "@idRegra_NCM", regra.idRegra.ToString() },
                    { "@idStatus", regra.idStatus.ToString() },
                    { "@dtInicial", regra.dtInicial.ToString() },
                    { "@dtFinal", regra.dtFinal.ToString() },
                    { "@sEstadoOrigem", regra.sEstadoOrigem },
                    { "@sEstadoDestino", regra.sEstadoDestino },
                    { "@nICMS_Regra", removeCaracteres(regra.nICMS.ToString()) },
                    { "@sObs", regra.sObs }
                };
                BD.ExecutarDataSet(sProcedure, vParametros);
            }

            return true;
        }

        protected void cmdIncluirRegra_Click(object sender, EventArgs e)
        {
            string sMsg = "";
            if (string.IsNullOrEmpty(txtdtInicial.Text)) sMsg += "Preencha a Data Inicial da Regra Fiscal!<br />";
            if (string.IsNullOrEmpty(txtdtFinal.Text)) sMsg += "Preencha a Data Final da Regra Fiscal!<br />";
            if (string.IsNullOrEmpty(ddlOrigem.Text)) sMsg += "Selecione um Estado de Origem para a Regra Fiscal!<br />";
            if (string.IsNullOrEmpty(ddlDestino.Text)) sMsg += "Selecione um Estado de Destino para a Regra Fiscal!<br />";
            if (string.IsNullOrEmpty(txtnICMS_Regra.Text)) sMsg += "Preencha a porcentagem de ICMS!";

            if (string.IsNullOrEmpty(sMsg))
            {
                DateTime.TryParse(txtdtInicial.Text, out DateTime dtInicial);
                DateTime.TryParse(txtdtFinal.Text, out DateTime dtFinal);
                int idStatus = DateTime.Now.Between(dtInicial, dtFinal) ? 1 : 2;

                if (hddidRegra_Editar.Value == "0")
                {
                    clsRegras regra = new clsRegras
                    {
                        idRegra = listRegras.Count > 0 ? listRegras.Count * (-1) - 1 : -1,
                        idStatus = idStatus,
                        dtInicial = dtInicial,
                        dtFinal = dtFinal,
                        sEstadoOrigem = ddlOrigem.SelectedValue,
                        sDscEstadoOrigem = ddlOrigem.SelectedItem.Text,
                        sEstadoDestino = ddlDestino.SelectedValue,
                        sDscEstadoDestino = ddlDestino.SelectedItem.Text,
                        nICMS = decimal.Parse(txtnICMS_Regra.Text),
                        sObs = txtObs.Text
                    };
                    listRegras.Add(regra);

                    MensagemPagina_Regras.MostraMensagem_Sucesso("Nova Regra incluída com sucesso!");
                }
                else
                {
                    var regra = listRegras.FirstOrDefault(r => r.idRegra.ToString() == hddidRegra_Editar.Value);
                    if (regra != null)
                    {
                        regra.idStatus = idStatus;
                        regra.dtInicial = dtInicial;
                        regra.dtFinal = dtFinal;
                        regra.sEstadoOrigem = ddlOrigem.SelectedValue;
                        regra.sDscEstadoOrigem = ddlOrigem.SelectedItem.Text;
                        regra.sEstadoDestino = ddlDestino.SelectedValue;
                        regra.sDscEstadoDestino = ddlDestino.SelectedItem.Text;
                        regra.nICMS = decimal.Parse(txtnICMS_Regra.Text);
                        regra.sObs = txtObs.Text;

                        MensagemPagina_Regras.MostraMensagem_Sucesso("Regra editada com sucesso!");
                    }
                }

                hddidRegra_Editar.Value = "0";
                cmdIncluirRegra.Text = "Incluir Regra";
                div_cmdCancelarEdicao.Visible = false;

                LimpaCampos_Regra();
                gvRegrasFicais_DataBind();
            }
            else
            {
                MensagemPagina_Regras.MostraMensagem_Erro(sMsg);
                FUNCOES.Scripts.AbrirCollapse(Page, "div_IncluirRegra");
            }

            cmdIncluirRegra.Focus();
        }

        protected void cmdCancelarEdicao_Click(object sender, EventArgs e)
        {
            var regra = listRegras.First(r => r.idRegra.ToString() == hddidRegra_Editar.Value);
            regra.idStatus = DateTime.Now.Between(regra.dtInicial, regra.dtFinal) ? 1 : 2;

            hddidRegra_Editar.Value = "0";
            cmdIncluirRegra.Text = "Incluir Regra";
            div_cmdCancelarEdicao.Visible = false;

            LimpaCampos_Regra();
            gvRegrasFicais_DataBind();
        }

        protected void cmdFiltro_Historico_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> vParam = new Dictionary<string, string>
            {
                { "@sFuncao", "FILTRO_HISTORICO_REGRA" },
                { "@idNCM", hddidNCM.Value },
                { "@sFiltro_Historico", hddFiltro_Historico.Value.Trim('|').Trim() }
            };
            gvHistorico_Regras.DataSource = BD.ExecutarDataTable(sProcedure, vParam);
            gvHistorico_Regras.DataBind();

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_AjusteFiltro_Historico", $"$('#{hddFiltro_Historico.ClientID}').val().split('|').forEach(function(value) {{ if (value) {{ $(`.chkFiltro_Historico[data-id*=${{value}}]`).prop('checked', true); }} }});", true);
        }

        #endregion

        #region | Eventos

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            if (ValidarDados())
            {
                try
                {
                    string[] vidNCM = hddidNCM.Value.Split(',');
                    string idNCM = vidNCM[0].ToString();

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idNCM", idNCM },
                        { "@sCodigoNCM", txtsCodigoNCM.Text },
                        { "@sLiImportacao", ddlLicencaImportacao.SelectedValue },
                        { "@sRedBC", Switch_ReducaoBC.Recuperar() },
                        { "@sAntidamping", ddlAntidamping.SelectedValue },
                        { "@nIPI", removeCaracteres(txtnIPI.Text) },
                        { "@nII", removeCaracteres(txtnII.Text) },
                        { "@nPIS", removeCaracteres(txtnPIS.Text) },
                        { "@nCOFINS", removeCaracteres(txtnCOFINS.Text) },
                        { "@nICMS", removeCaracteres(txtnICMS.Text) },
                        { "@nIPI_Internacional", removeCaracteres(txtnIPI_Internacional.Text) },
                        { "@nII_Internacional", removeCaracteres(txtnII_Internacional.Text) },
                        { "@nPIS_Internacional", removeCaracteres(txtnPIS_Internacional.Text) },
                        { "@nCOFINS_Internacional", removeCaracteres(txtnCOFINS_Internacional.Text) },
                        { "@nICMS_Internacional", removeCaracteres(txtnICMS_Internacional.Text) },
                        { "@sDscNCM", txtsDscNCM.Text },
                        { "@sSituacao", ComboAtivo.Situacao_Recuperar() },
                        { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() },
                        { "@idCEST", ddlCEST_Principal.SelectedValue },
                        { "@sCEST", string.Join("|", lstCEST.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value)) }
                    };
                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out string sErro))
                    {
                        idNCM = RETORNO.DATASET(dsSalvar, "idNCM");

                        if (Salvar_Regras(idNCM))
                            FUNCOES.RecarregarPagina("&msg=1");
                    }
                    else throw new Exception("BD: " + sErro.ToString());
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }
            }
        }

        protected void lstCEST_SelectedIndexChanged(object sender, EventArgs e)
        {
            string idCEST = ddlCEST_Principal.SelectedValue;

            ddlCEST_Principal.Items.Clear();
            ddlCEST_Principal.Items.Add(new ListItem("Não Classificado", "0"));

            foreach (ListItem item in lstCEST.Items)
            {
                if (item.Selected)
                    ddlCEST_Principal.Items.Add(new ListItem(item.Text, item.Value));
            }

            try { ddlCEST_Principal.Items.FindByValue(idCEST).Selected = true; } catch { }
        }

        #endregion

        #region | Script

        protected void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("$(document).ready(function() {");
            sb.AppendLine("");
            sb.AppendLine("     $('[id*=txtsCodigoNCM]').mask('0000.00.00');");
            sb.AppendLine("");
            sb.AppendLine("     $('[id*=txtnII]').mask('999,99', { reverse: true });");
            sb.AppendLine("     $('[id*=txtnIPI]').mask('999,99', { reverse: true });");
            sb.AppendLine("     $('[id*=txtnPIS]').mask('999,99', { reverse: true });");
            sb.AppendLine("     $('[id*=txtnCOFINS]').mask('999,99', { reverse: true });");
            sb.AppendLine("     $('[id*=txtnICMS]').mask('999,99', { reverse: true });");
            sb.AppendLine("");
            sb.AppendLine("     $('.cmdHistorico_Collapse').click(function(e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         $(this).siblings('.collapse').collapse('toggle');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $('.chkLimpa_Filtro_Historico').click(function() {");
            sb.AppendLine("         $('.chkFiltro_Historico').prop('checked', false);");
            sb.AppendLine($"        $('#{hddFiltro_Historico.ClientID}').val('');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $('.spanFiltro_Historico').click(function(e) {");
            sb.AppendLine("         if ($(e.target).is('.chkFiltro_Historico')) return;");
            sb.AppendLine("         const check = $(this).find('.chkFiltro_Historico');");
            sb.AppendLine("         check.prop('checked', !check.prop('checked'));");
            sb.AppendLine("         check.trigger('change');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $('.chkFiltro_Historico').change(function(e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         console.log('teste');");
            sb.AppendLine("         const checked = $(this).prop('checked');");
            sb.AppendLine("         const filtro = $(this).data('id') + '|';");
            sb.AppendLine($"        $('#{hddFiltro_Historico.ClientID}').val($('#{hddFiltro_Historico.ClientID}').val().replace(filtro, '') + (checked ? filtro : ''));");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("});");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_RegistraScript", sb.ToString(), true);
        }

        #endregion
    }
}