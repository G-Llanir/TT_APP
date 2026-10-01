using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using TT_Flow.FrameWork;
using TT.FrameWork;
using static TT.FrameWork.BD;
using TT_Hub.App.Paginas.Adm.Manutencao;
using System.Text;
using System.IO;
using TT_Hub.App.Paginas.Adm.Financeiro;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using static Permissao.Financeiro;

namespace TT_Flow.App.Paginas.Adm.Financeiro
{
    public partial class Financeiro_Cliente_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Financeiro";
        string sProcedure = "sp_Manipula_tbl_Flow_Financeiro_Cliente";

        #region | Classes
        public List<FrameWork.cls_Recebimento> bs_Recebimento
        {

            get
            {
                if (ViewState["bs_Recebimento"] == null)
                {
                    ViewState["bs_Recebimento"] = new List<FrameWork.cls_Recebimento>();
                }
                return (List<cls_Recebimento>)ViewState["bs_Recebimento"];
            }

            set
            {
                ViewState["bs_Recebimento"] = value;
            }



        }
        #endregion

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "ManualdoUsuarioContasReceber.pdf";
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();

            txtidContasReceber.ReadOnly = true;
            txtdtVencimentoOriginal.ReadOnly = true;

            if (!IsPostBack)
            {

                PopularCombos();
                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Financeiro.Financeiro_Cliente.Consultar, true);
                    Pesquisar(Request["id"].ToString(), false);
                }

                if (Session["SalvoComSucesso"] != null && (bool)Session["SalvoComSucesso"])
                {
                    MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                    Session["SalvoComSucesso"] = false;
                }
            }
            else
            {
                var requestTarget = this.Request["__EVENTTARGET"];
                var requestArgs = this.Request["__EVENTARGUMENT"];

                if (requestTarget == "funcao_SAIR")
                {
                    FUNCOES.DirecionaPagina("/app/dashboard.aspx");
                }
                else if (requestTarget == "funcao_Editar")
                {
                    Pesquisar(hddidContasReceber.Value, true);
                }
            }
            RegistraScript("");

        }
        #endregion

        #region |Metodos Banco de Dados
        protected void Pesquisar(string idContasReceber, bool bEdicao)
        {
            Div_dtVencimento.Visible = false;
            Div_nValorOriginal.Visible = true;
            Div_VencimentoOriginal.Visible = false;
            Div_dtVencimento.Visible = false;
            txtnTotal.ReadOnly = true;
            Div_Saldo.Visible = false;
            DivTotal.Visible = false;

            string sErro = "";

            try
            {
                LimpaCampos();

                if (idContasReceber != "0")
                {

                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());
                    vParametros.Add("@idContasReceber", idContasReceber);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        StatusDosCampos(true);
                        string sDscTipoStatus = RETORNO.DATASET(dsPesquisa, 0, "sStatus");
                        lblsDscTipoStatus.Text = sDscTipoStatus;
                        lblsDscTipoStatus.CssClass = string.Format("label label-{0}", RETORNO.DATASET(dsPesquisa, 0, "sCor"));

                        hddidContasReceber.Value = RETORNO.DATASET(dsPesquisa, 0, "idContasReceber");
                        txtidContasReceber.Text = RETORNO.DATASET(dsPesquisa, 0, "idContasReceber");

                        var dt = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtEmissao").ToString());
                        txtdtEmissao.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');

                        var dt2 = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtVencimento").ToString());
                        txtdtVencimento.Text = dt2.ToString(@"yyyy/MM/dd").Replace('/', '-');
                        txtdtVencimentoOriginal.Text = dt2.ToString(@"yyyy/MM/dd").Replace('/', '-');

                        txtsDocumento.Text = RETORNO.DATASET(dsPesquisa, 0, "sDocumento");

                        txtnValorOriginal.Text = RETORNO.DATASET(dsPesquisa, 0, "nValorOriginal");
                        txtnSaldo.Text = RETORNO.DATASET(dsPesquisa, 0, "nSaldo");
                        txtsObservacaoGeral.Text = RETORNO.DATASET(dsPesquisa, 0, "sObservacaoGeral");
                        txtsCodigo.Text = RETORNO.DATASET(dsPesquisa, 0, "sCodigo");
                        //ddlidFormaRecebimento.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idFormaRecebimento");
                        //ddlidCentroDeCusto.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idCentroDeCusto");
                        Pesquisa_Parceiros.idParceiro = Convert.ToInt32(RETORNO.DATASET(dsPesquisa, 0, "idParceiro"));

                        txtsQuantidadeParcela.Text = RETORNO.DATASET(dsPesquisa, 0, "sQuantidadeParcela");
                        txtnTotal.Text = RETORNO.DATASET(dsPesquisa, 0, "nValorPago");


                        Popular_dtgRecebimento(dsPesquisa);

                        lblTituloPagina.Text = string.Format("Financeiro - {0} - Vencimento: {1}", RETORNO.DATASET(dsPesquisa, 0, "sRazaoSocial"), RETORNO.DATASET(dsPesquisa, 0, "dtVencimento"));
                        BreadCrumb.TitulodaPagina = string.Format("Financeiro Detalhe - {0}", RETORNO.DATASET(dsPesquisa, 0, "idContasReceber"));

                        Pesquisa_Parceiros.ConfigurarControles(false);
                        Div_VencimentoOriginal.Visible = true;
                        Div_dtVencimento.Visible = true;
                        Div_nValorOriginal.Visible = true;
                        Div_sQuantidadeParcela.Visible = true;
                        Div_dtVencimento.Visible = true;

                        Div_Saldo.Visible = true;
                        DivTotal.Visible = true;

                        if (dsPesquisa.Tables[1].Rows.Count == 0)
                        {
                            Div_Recebimento.Visible = false;
                        }
                        else
                        {
                            Div_Recebimento.Visible = true;
                        }

                        DateTime dataVencimento;
                        bool conversaoSucesso = DateTime.TryParse(txtdtVencimento.Text, out dataVencimento);

                        if (txtnSaldo.Text == "0,00")
                        {
                            txtsObservacaoGeral.ReadOnly = false;
                            txtsObservacaoGeral.ReadOnly = true;
                        }

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtdtRecebimento_Info_Rec]').focus();", true);
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }
                }
                else
                {
                    BreadCrumb.TitulodaPagina = string.Format("Nova {0}", sTituloPagina);
                    lblTituloPagina.Text = string.Format("Nova {0}", sTituloPagina);
                    txtidContasReceber.Text = "Novo";
                    Div_Recebimento.Visible = false;
                    DivTotal.Visible = false;
                    Div_Saldo.Visible = false;
                    Div_dtVencimento.Visible = false;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtdtEmissao]').focus();", true);

                }
                RegistraScript("");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }

        void StatusDosCampos(bool bStatus)
        {
            txtsCodigo.ReadOnly = bStatus;
            txtdtEmissao.ReadOnly = bStatus;
            txtdtVencimento.ReadOnly = bStatus;
            txtsDocumento.ReadOnly = bStatus;
            txtnValorOriginal.ReadOnly = bStatus;
            txtnSaldo.ReadOnly = bStatus;
            txtsObservacaoGeral.ReadOnly = bStatus;
            txtsQuantidadeParcela.ReadOnly = bStatus;


            string sStatusCombo = "disabled";
            if (!bStatus)
            {
                sStatusCombo = "enabled";
            }
        }
        #endregion

        #region | Limpar Campos
        void LimpaCampos()
        {
            hddidContasReceber.Value = "0";
            txtidContasReceber.Text = "Novo";
            txtsCodigo.Text = "";
            txtdtEmissao.Text = DateTime.Today.ToString("u").Substring(0, 10);
            txtdtVencimento.Text = "";
            txtsDocumento.Text = "";
            txtnValorOriginal.Text = "";
            txtnSaldo.Text = "";
            hddidEmpresa.Value = "0";
            txtsObservacaoGeral.Text = "";

            bs_Recebimento.Clear();

            lblTituloPagina.Text = sTituloPagina;

        }
        #endregion

        #region | Script 
        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            //Mensagens de Confirmação
            sb.Append("$v192(function() {");

            sb.Append("$v192(\"#dialog-Salvar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_SALVAR\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=cmdSalvar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Salvar').dialog('open');");
            sb.Append("});");

            sb.Append("$v192(\"#dialog-Excluir\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_EXCLUIR\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=BtnExcluirParcela]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Excluir').dialog('open');");
            sb.Append("});");

            sb.Append("$v192(\"#dialog-Reabrir\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_Reabrir\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=BtnReabrirTitulo]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Reabrir').dialog('open');");
            sb.Append("});");


            sb.Append("$('[id*=txtnParcela]').mask('00#');");

            sb.Append("$('[id*=txtnValorOriginal]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnSaldo]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnValorPago]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=nValorRecebimento]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=nValorLancamentoRec]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnValorBruto]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnMulta]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnJuros]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnDesconto]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnValorTotalRec]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnTotal]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnValorRecebimento_Info_Rec]').mask('000.000.000.000.000,00', { reverse: true });");




            sb.Append("});");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);

        }
        #endregion

        #region | Combos/DDL
        void PopularCombos()
        {

        }
        #endregion

        #region | Recebimento
        void Popular_dtgRecebimento(DataSet dsPesquisa)
        {
            foreach (DataRow row in dsPesquisa.Tables[1].Rows)
            {
                FrameWork.cls_Recebimento objItem = new FrameWork.cls_Recebimento();

                objItem.idContasReceber = Convert.ToInt32(row["idContasReceber"].ToString());
                objItem.idRegistroRecebimento = Convert.ToInt32(row["idRegistroRecebimento"].ToString());

                objItem.idLinha = bs_Recebimento.Count() + 1;
                objItem.sFuncao = "SEM ALTERAÇÃO";

                objItem.nValorRecebimento_Info_Rec = ConverterStringDecimal(row["nValorRecebimento_Info_Rec"].ToString());
                objItem.dtRecebimento_Info_Rec = row["dtRecebimento_Info_Rec"].ToString();
                objItem.nNumeroParcela_Info_Rec = Convert.ToInt32(row["nNumeroParcela_Info_Rec"].ToString());
                objItem.idConta_Info_Rec = Convert.ToInt32(row["idConta_Info_Rec"].ToString());
                objItem.sDscConta_Info_Rec = row["sDscConta_Info_Rec"].ToString();
                objItem.dtAtualizacao = row["dtAtualizacao"].ToString();
                objItem.idFormaRecebimento_Info_Rec = Convert.ToInt32(row["idFormaRecebimento_Info_Rec"].ToString());
                objItem.sDscFormaRecebimento_Info_Rec = row["sDscFormaRecebimento_Info_Rec"].ToString();
                objItem.idArquivo = Convert.ToInt32(row["idArquivo"].ToString());
                objItem.sNomeArquivo = row["sNomeArquivo"].ToString();

                objItem.nDesconto = ConverterStringDecimal(row["nDesconto"].ToString());
                objItem.nJuros = ConverterStringDecimal(row["nJuros"].ToString());
                objItem.nMulta = ConverterStringDecimal(row["nMulta"].ToString());
                objItem.nValorTotalRec = ConverterStringDecimal(row["nValorTotalRec"].ToString());


                bs_Recebimento.Add(objItem);
            }
            dtgRecebimento_DataBind();
        }

        protected void dtgRecebimento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0);

            if (e.Row.Cells[2].Text != "Data do Recebimento" && e.Row.Cells[2].Text != "&nbsp;")
                e.Row.Cells[2].Text = DateTime.Parse(e.Row.Cells[2].Text).ToString("dd/MM/yyyy");

            decimal Recebido = 0;
            decimal Multa = 0;
            decimal Juros = 0;
            decimal Desconto = 0;

            if (e.Row.Cells[3].Text != "Valor Recebido" && e.Row.Cells[3].Text != "&nbsp;")
            {
                Recebido = Convert.ToDecimal(e.Row.Cells[3].Text.Replace("R$ ", ""));
            }

            if (e.Row.Cells[4].Text != "Valor Multa" && e.Row.Cells[4].Text != "&nbsp;")
            {
                Multa = Convert.ToDecimal(e.Row.Cells[4].Text.Replace("R$ ", ""));
            }
                
            if (e.Row.Cells[5].Text != "Valor Juros" && e.Row.Cells[5].Text != "&nbsp;")
            {
                Juros = Convert.ToDecimal(e.Row.Cells[5].Text.Replace("R$ ", ""));
            }
               
            if (e.Row.Cells[6].Text != "Valor Desconto" && e.Row.Cells[6].Text != "&nbsp;")
            {
                Desconto = Convert.ToDecimal(e.Row.Cells[6].Text.Replace("R$ ", ""));
            }

            decimal Total = Recebido + Multa + Juros - Desconto;
            if (e.Row.Cells[7].Text != "Valor Total" && e.Row.Cells[7].Text != "&nbsp;")
            {
                e.Row.Cells[7].Text = Total.ToString();
            }
                
        }

        void dtgRecebimento_DataBind()
        {
            try
            {
                dtgRecebimento.DataSource = bs_Recebimento.Where(c => c.sFuncao.ToString() != "EXCLUIR_Recebimento");
                dtgRecebimento.DataBind();
                RegistraScript("");
                GRID.SomarColunas(dtgRecebimento, true, GRID.Formatação.Moeda, 7);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao Carregar Grid de Recebimento: " + ex.Message);
            }
        }
        #endregion

        #region | Eventos 
        decimal ConverterStringDecimal(string dado)
        {
            decimal converter = 0;
            if (decimal.TryParse(dado, out converter))
            {
                converter = Convert.ToDecimal(dado);
                return converter;
            }
            return decimal.Zero;
        }
        #endregion
    }
}
