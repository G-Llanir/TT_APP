using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT_Flow.FrameWork;
using static TT.FrameWork.Identity;
using ARQUIVO = TT.FrameWork.Arquivo;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using CODIGOBARRAS = TT.FrameWork.GeradorCodigoBarras;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;

namespace TT_Flow.App.Paginas.RRHH
{
    public partial class EntregaEPI_Detalhe : Page
    {
        #region | Contrutores

        protected static string sTituloPagina = "Entrega de EPI";
        protected static string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Entrega_EPI";
        protected static int nColuna_Estado = 3;
        protected static int nColuna_Periodicidade = 6;

        /// <summary>
        /// Objeto utilizado para resgatar o 'sPossuiPeriodicidade' para cada Tipo de Entrega de EPI, sendo:
        /// <br />
        /// 1 - Lista Padrão -> Periodicidade <br />
        /// 2 - Entrega Pontual -> Periodicidade <br />
        /// 3 - Solicitação de Cliente -> Vencimento <br />
        /// 4 - Conferência -> Vencimento <br />
        /// 5 - Entrega Avulsa -> Vencimento <br />
        /// </summary>
        protected static Dictionary<string, bool> Periodicidade_x_Status = new Dictionary<string, bool> { { "0", true }, { "1", true }, { "2", true }, { "3", false }, { "4", false }, { "5", false } };

        protected List<cls_EPI_x_Funcao> Lista_EPIs
        {
            get
            {
                if (ViewState["Lista_EPIs"] == null) ViewState["Lista_EPIs"] = new List<cls_EPI_x_Funcao>();
                return (List<cls_EPI_x_Funcao>)ViewState["Lista_EPIs"];
            }
            set { ViewState["Lista_EPIs"] = value; }
        }

        protected Dictionary<int, DateTime> Lista_Entregas_Vinculadas
        {
            get
            {
                if (ViewState["Lista_Entregas_Vinculadas"] == null) ViewState["Lista_Entregas_Vinculadas"] = new Dictionary<int, DateTime>();
                return (Dictionary<int, DateTime>)ViewState["Lista_Entregas_Vinculadas"];
            }
            set { ViewState["Lista_Entregas_Vinculadas"] = value; }
        }

        #endregion

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.RRHH.Entrega_EPI.Consultar, true);
                    Pesquisar(Request["id"].ToString(), false);
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.RRHH.Entrega_EPI.Incluir, true);
                    Pesquisar("0", false);
                }

                if (Convert.ToBoolean(Session["SalvoComSucesso"]))
                {
                    MensagemPagina_Entrega.MostraMensagem_Sucesso(string.Format("Registro gravado com sucesso!{0}", Convert.ToBoolean(Session["LinkNovo"]) ? $"<br /><a href=\"/App/Paginas/RRHH/EntregaEPI_Detalhe.aspx?id=0\">Nova {sTituloPagina}...</a>" : string.Empty));

                    Session["SalvoComSucesso"] = false;
                    Session["LinkNovo"] = false;
                }
                else if (Convert.ToBoolean(Session["EntregarEPI"]))
                {
                    MensagemPagina_Entrega.MostraMensagem_Sucesso("EPIs Entregues com sucesso!", false);
                    MensagemPagina_Entrega.MostraMensagem("Ao definir como Entregue, o Status da Entrega é atualizado, e passa a ser 'Entregue - Aguardando Confirmação', faltando apenas a confirmação da Entrega!", "info", false);

                    Session["EntregarEPI"] = false;
                }
                else if (Convert.ToBoolean(Session["ConfirmarEntrega"]))
                {
                    MensagemPagina_Entrega.MostraMensagem_Sucesso("Entrega de EPI confirmada com sucesso!", false);
                    Session["ConfirmarEntrega"] = false;
                }
            }
            else
            {
                var requestTarget = Request["__EVENTTARGET"];
                if (requestTarget == "funcao_SALVAR")
                    SalvarEntrega();
                else if (requestTarget == "funcao_EDITAR")
                    Pesquisar(Request["id"], true);
            }

            RegistraScript();
        }

        protected void Pesquisar(string idPesquisa, bool bEdita)
        {
            aba_Historico.Visible = false;
            aba_Arquivos.Visible = false;

            div_EPIs.Visible = false;
            divIncluir_EPI.Visible = false;
            div_ddlTipo.Visible = false;
            div_dtgEPI.Visible = false;
            div_Obs.Visible = false;
            div_ComboAtivo.Visible = false;

            cmdEditar.Visible = false;
            cmdGerarPDF.Visible = false;
            cmdEntregarEPI.Visible = false;
            cmdConfirmar.Visible = false;
            cmdSubstitutivas.Visible = false;
            cmdNovaSubstitutiva.Visible = false;

            try
            {
                FUNCOES.Popula_Combo(ddlTipo, "sp_Select 'tbl_Flow_Colaboradores_Entrega_EPI_Tipo'", "idTipo", "sDscTipo", false, "Selecione um Tipo", "0");
                FUNCOES.Popula_Combo(ddlidStatus, "sp_Select 'tbl_Flow_Colaboradores_Entrega_EPI_Status'", "idStatus", "sDscStatus", false);

                LimpaCampos();

                DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_DETALHE" }, { "@idEntregaEPI", idPesquisa } });

                if (idPesquisa != "0")
                {
                    cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.Entrega_EPI.Alterar);

                    if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        div_EPIs.Visible = true;
                        div_ddlTipo.Visible = true;
                        div_dtgEPI.Visible = true;
                        div_Obs.Visible = true;
                        div_ComboAtivo.Visible = true;
                        aba_Arquivos.Visible = true;
                        PainelAtualizacao.Visible = true;

                        cmdGerarPDF.Visible = true;

                        ComboAtivo.Situacao_BloquearEdicao(false);

                        hddidEntregaEPI.Value = RETORNO.DATASET(dsPesquisa, "idEntregaEPI");
                        txtidEntrega.Text = RETORNO.DATASET(dsPesquisa, "idEntregaEPI");
                        hddidSelecionarColaborador.Value = RETORNO.DATASET(dsPesquisa, "idColaborador");
                        txtdtSolicitacao.Text = DateTime.Parse(RETORNO.DATASET(dsPesquisa, "dtSolicitacao")).ToString("yyyy-MM-dd");
                        txtdtEntrega.Text = DateTime.Parse(RETORNO.DATASET(dsPesquisa, "dtEntrega")).ToString("yyyy-MM-dd");
                        hddidStatus.Value = RETORNO.DATASET(dsPesquisa, "idStatus");
                        ddlidStatus.SelectedValue = RETORNO.DATASET(dsPesquisa, "idStatus");
                        ddlTipo.SelectedValue = RETORNO.DATASET(dsPesquisa, "idTipoEntrega");
                        txtCPF_Colaborador.Text = RETORNO.DATASET(dsPesquisa, "sCPF_Colcaborador");
                        txtNome_Colaborador.Text = RETORNO.DATASET(dsPesquisa, "sDscColaborador");
                        txtObs.Text = RETORNO.DATASET(dsPesquisa, "sDscObservacao");
                        hddsDataEntrega_Manual.Value = RETORNO.DATASET(dsPesquisa, "sDataEntrega_Manual");
                        cmdNovaSubstitutiva.NavigateUrl = $"~/App/Paginas/RRHH/EntregaEPI_Detalhe.aspx?id=0&idConferencia={idPesquisa}";
                        cmdNovaSubstitutiva_modal.NavigateUrl = $"~/App/Paginas/RRHH/EntregaEPI_Detalhe.aspx?id=0&idConferencia={idPesquisa}";

                        if (ddlTipo.SelectedValue == "4") sTituloPagina = "Conferência";
                        else sTituloPagina = "Entrega de EPI";

                        lblTituloPagina.Text = string.Format("{0} para {1}", sTituloPagina, txtNome_Colaborador.Text);
                        aba_EntregaEPI.InnerHtml = $"<b>{sTituloPagina}</b>";
                        tituloPagina.InnerHtml = $"<b>{sTituloPagina}</b>";

                        string sSolicitacao = string.Empty;
                        string sEntrega = string.Empty;
                        string sConfirmacao = string.Empty;
                        if (RETORNO.DATASET(dsPesquisa, "idUsuarioSolicitacao") != "0") sSolicitacao = string.Format("<br />Solicitado em <b>{0}</b> por <b>{1}</b>", RETORNO.DATASET(dsPesquisa, "dtSolicitacao"), RETORNO.DATASET(dsPesquisa, "sDscUsuarioSolicitacao"));
                        if (RETORNO.DATASET(dsPesquisa, "idUsuarioEntrega") != "0") sEntrega = string.Format("<br />Entregue em <b>{0}</b> por <b>{1}</b>", RETORNO.DATASET(dsPesquisa, "dtEntrega"), RETORNO.DATASET(dsPesquisa, "sDscUsuarioEntrega"));
                        if (RETORNO.DATASET(dsPesquisa, "idUsuarioConfirmacao") != "0") sConfirmacao = string.Format("<br />Confirmada em <b>{0}</b> por <b>{1}</b>", RETORNO.DATASET(dsPesquisa, "dtConfirmacao"), RETORNO.DATASET(dsPesquisa, "sDscUsuarioConfirmacao"));

                        PainelAtualizacao.Personalizar(string.Format("Última atualização em <b>{0}</b> por <b>{1}</b>{2}{3}{4}", RETORNO.DATASET(dsPesquisa, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, "sDscUsuarioAtualizacao"), sSolicitacao, sEntrega, sConfirmacao));
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, "sAtivo"));

                        Popular_Aba_Historico(dsPesquisa);
                        Popular_Entregas_Vinculadas(dsPesquisa.Tables[5]);

                        if (!bEdita)
                        {
                            hddEditar.Value = "false";

                            txtCPF_Colaborador.ReadOnly = true;
                            txtNome_Colaborador.ReadOnly = true;
                            div_cmdSelecionarColaborador.Visible = false;
                            ddlTipo.Attributes.Add("disabled", "disabled");
                            txtdtSolicitacao.ReadOnly = true;
                            txtdtEntrega.ReadOnly = true;
                            txtObs.ReadOnly = true;
                            cmdSalvar.Visible = false;
                            cmdEditar.Visible = true;

                            if (hddidStatus.Value == "2") cmdEntregarEPI.Visible = true;
                            else if (hddidStatus.Value == "3")
                            {
                                cmdEditar.Visible = false;
                                cmdGerarPDF.Visible = false;
                                cmdConfirmar.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.Entrega_EPI.ConfirmarEntrega, false);
                            }
                            else if (hddidStatus.Value == "4" || hddidStatus.Value == "5" || hddidStatus.Value == "7")
                            {
                                cmdEditar.Visible = false;
                                cmdGerarPDF.Visible = false;
                            }
                            else if (hddidStatus.Value == "6") cmdEditar.Visible = false;

                            if (RETORNO.DATASET(dsPesquisa, "sAtivo").Equals("N")) cmdGerarPDF.Visible = false;
                        }
                        else
                        {
                            hddEditar.Value = "true";

                            txtCPF_Colaborador.ReadOnly = false;
                            txtNome_Colaborador.ReadOnly = false;
                            div_cmdSelecionarColaborador.Visible = true;
                            ddlTipo.Attributes.Remove("disabled");
                            txtdtSolicitacao.ReadOnly = true;
                            txtdtEntrega.ReadOnly = true;
                            txtObs.ReadOnly = false;
                            divIncluir_EPI.Visible = true;
                            cmdSalvar.Visible = true;
                            cmdEditar.Visible = false;
                            cmdGerarPDF.Visible = false;
                            cmdEntregarEPI.Visible = false;

                            ComboAtivo.Situacao_BloquearEdicao(true);
                        }

                        if (ddlTipo.SelectedValue == "4") divIncluir_EPI.Visible = false;

                        eArquivos.Attributes.Add("src", string.Format("../Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}", idPesquisa, "EntregaEPI"));
                    }
                    else
                        throw new Exception(sErro);
                }
                else
                {
                    hddidStatus.Value = "1";

                    div_ddlStatus.Visible = false;
                    ddlidStatus.SelectedValue = "1";

                    if (Request["idColaborador"] != null) sTituloPagina = "Conferência";
                    else sTituloPagina = "Entrega de EPI";

                    lblTituloPagina.Text = string.Format("Nova {0}", sTituloPagina);
                    aba_EntregaEPI.InnerHtml = $"<b>{sTituloPagina}</b>";
                    tituloPagina.InnerHtml = $"<b>{sTituloPagina}</b>";
                    BreadCrumb.TitulodaPagina = "Incluir";

                    txtidEntrega.Text = "Novo";
                    txtdtSolicitacao.Text = DateTime.Today.ToString("yyyy-MM-dd");
                    txtdtEntrega.Text = DateTime.Today.ToString("yyyy-MM-dd");
                    cmdSalvar.Text = "Incluir";

                    if (Request["idConferencia"] != null)
                    {
                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.Entrega_EPI.Alterar);

                        idPesquisa = Request["idConferencia"].ToString();
                        hddConferencia.Value = idPesquisa;

                        dsPesquisa = BD.ExecutarDataSet(sProcedure, new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_DETALHE" }, { "@idEntregaEPI", idPesquisa }, { "@idConferencia", idPesquisa } });

                        if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                        {
                            div_EPIs.Visible = true;
                            div_ddlTipo.Visible = true;
                            div_dtgEPI.Visible = true;
                            div_Obs.Visible = true;

                            hddidSelecionarColaborador.Value = RETORNO.DATASET(dsPesquisa, "idColaborador");
                            txtCPF_Colaborador.Text = RETORNO.DATASET(dsPesquisa, "sCPF_Colcaborador");
                            txtNome_Colaborador.Text = RETORNO.DATASET(dsPesquisa, "sDscColaborador");
                            ddlTipo.SelectedValue = "2";
                            txtObs.Text = RETORNO.DATASET(dsPesquisa, "sDscObservacao");
                        }
                        else
                            throw new Exception(sErro);
                    }
                }

                Popular_dtgEPI(dsPesquisa);

                if (idPesquisa == "0" && Request["idColaborador"] != null)
                {
                    string idColaborador = Request["idColaborador"].ToString();
                    var colaborador = hddColaboradores.Value.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries).First(c => c.Split('|')[0].Equals(idColaborador));

                    txtCPF_Colaborador.Text = colaborador.Split('|')[1];
                    txtNome_Colaborador.Text = colaborador.Split('|')[2];

                    hddidSelecionarColaborador.Value = idColaborador;
                    cmdSelecionarColaborador_Click(null, null);

                    ddlTipo.SelectedValue = "4";
                    ddlTipo_SelectedIndexChanged(null, null);

                    Popular_EPIs_Colaborador();
                }

                if (ddlTipo.SelectedValue == "4")
                {
                    aba_Arquivos.Visible = false;
                    aba_Historico.Visible = false;
                    txtdtSolicitacao.ReadOnly = true;
                    div_dtEntrega.Visible = false;

                    if (idPesquisa != "0")
                    {
                        cmdSubstitutivas.Visible = Lista_EPIs.Where(e => e.idEstado >= 3).Any() && Lista_Entregas_Vinculadas.Any();
                        cmdNovaSubstitutiva.Visible = Lista_EPIs.Where(e => e.idEstado >= 3).Any() && !Lista_Entregas_Vinculadas.Any();
                    }
                }

                lbl_dtSolicitacao.InnerText = ddlTipo.SelectedValue == "4" ? "Data da Conferência" : "Data da Solicitação";

                FUNCOES.Scripts.FocusScript(Page, txtNome_Colaborador.ClientID);
            }
            catch (Exception ex)
            {
                MensagemPagina_Entrega.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        #endregion

        #region | dtgEPI

        void dtgEPI_DataBind()
        {
            try
            {
                if (ddlTipo.SelectedValue == "4") dtgEPI.Columns[dtgEPI.Columns.Count - 1].Visible = false;
                dtgEPI.Columns[nColuna_Estado].Visible = ddlTipo.SelectedValue == "4";
                dtgEPI.Columns[nColuna_Periodicidade].HeaderText = Periodicidade_x_Status[ddlTipo.SelectedValue] ? "Periodicidade" : "Vence em";
                lblPeriodicidade_Incluir_EPI.InnerText = Periodicidade_x_Status[ddlTipo.SelectedValue] ? "Periodicidade" : "Vence em";

                dtgEPI.DataSource = Lista_EPIs.Where(c => c.sFuncao.ToString() != "EXCLUIR_EPI");
                dtgEPI.DataBind();

                div_dtgEPI.Visible = dtgEPI.Rows.Count > 0;
                div_Excluir.Visible = dtgEPI.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao Carregar os EPIs: " + ex.Message);
            }
        }

        protected void dtgEPI_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string idEstado = DataBinder.Eval(e.Row.DataItem, "idEstado").ToString();
                    if (ddlTipo.SelectedValue == "4" && idEstado == "0" || string.IsNullOrEmpty(idEstado)) idEstado = "1";

                    (e.Row.FindControl("ddlsTipoPeriodo") as DropDownList).SelectedValue = DataBinder.Eval(e.Row.DataItem, "sTipoPeriodo").ToString();
                    (e.Row.FindControl("ddlEstado") as DropDownList).SelectedValue = idEstado;

                    if (!string.IsNullOrEmpty(idEstado) && idEstado != "0")
                    {
                        switch (idEstado)
                        {
                            case "1":
                                e.Row.CssClass = "success";
                                break;
                            case "2":
                                e.Row.CssClass = "info";
                                break;
                            case "3":
                                e.Row.CssClass = "warning";
                                break;
                            case "4":
                            case "5":
                                e.Row.CssClass = "danger";
                                break;
                        }
                    }

                    if (!Convert.ToBoolean(hddEditar.Value))
                    {
                        dtgEPI.Columns[dtgEPI.Columns.Count - 1].Visible = false;

                        (e.Row.FindControl("txtCA") as TextBox).ReadOnly = true;
                        (e.Row.FindControl("txtnQuantidateEPI") as TextBox).ReadOnly = true;
                        (e.Row.FindControl("txtnQuantidadeTempo") as TextBox).ReadOnly = true;
                        (e.Row.FindControl("ddlEstado") as DropDownList).Attributes.Add("disabled", "disabled");
                        (e.Row.FindControl("ddlsTipoPeriodo") as DropDownList).Attributes.Add("disabled", "disabled");

                        if (ddlidStatus.SelectedValue == "5" && FUNCOES.ValidaPermissao(Permissao.RRHH.Entrega_EPI.AlterarPeriodicidade))
                        {
                            (e.Row.FindControl("txtnQuantidadeTempo") as TextBox).ReadOnly = false;
                            (e.Row.FindControl("ddlsTipoPeriodo") as DropDownList).Attributes.Remove("disabled");
                            cmdSalvar.Visible = true;
                        }
                    }
                    else
                    {
                        dtgEPI.Columns[dtgEPI.Columns.Count - 1].Visible = true;

                        (e.Row.FindControl("txtCA") as TextBox).ReadOnly = false;
                        (e.Row.FindControl("txtnQuantidateEPI") as TextBox).ReadOnly = false;
                        (e.Row.FindControl("txtnQuantidadeTempo") as TextBox).ReadOnly = false;
                        (e.Row.FindControl("ddlEstado") as DropDownList).Attributes.Remove("disabled");
                        (e.Row.FindControl("ddlsTipoPeriodo") as DropDownList).Attributes.Remove("disabled");
                    }
                }
            }
            catch { }
        }

        #endregion

        #region | Aba Histórico

        void Popular_Aba_Historico(DataSet ds)
        {
            aba_Historico.Visible = true;
            gv_Historico.DataSource = ds.Tables[2];
            gv_Historico.DataBind();
        }

        protected void gv_Historico_RowDataBound(object sender, GridViewRowEventArgs e) => e.Row.Cells[2].Text = HttpUtility.HtmlDecode(e.Row.Cells[2].Text);

        #endregion

        #region | Salvar + Classe

        protected void SalvarEntrega()
        {
            AtualizaClasses_EPI(true);

            if (ValidarDados())
            {
                try
                {
                    string idEntrega = hddidEntregaEPI.Value;

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idEntregaEPI", idEntrega },
                        { "@idColaborador", hddidSelecionarColaborador.Value },
                        { "@sDscObservacao", txtObs.Text },
                        { "@idStatus", hddidStatus.Value },
                        { "@idTipoEntrega", ddlTipo.SelectedValue },
                        { "@dtSolicitacao", txtdtSolicitacao.Text },
                        { "@dtEntrega", txtdtEntrega.Text },
                        { "@sAtivo", ComboAtivo.Situacao_Recuperar() },
                        { "@sDataEntrega_Manual", hddsDataEntrega_Manual.Value },
                        { "@idConferencia", hddConferencia.Value },
                        { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
                    };
                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out string sErro))
                    {
                        idEntrega = RETORNO.DATASET(dsSalvar, "idEntregaEPI");

                        if (Salvar_EPI(idEntrega))
                        {
                            Session["SalvoComSucesso"] = true;
                            Session["LinkNovo"] = Request["id"] == "0";

                            FUNCOES.DirecionaPagina(string.Format("App/Paginas/RRHH/EntregaEPI_Detalhe.aspx?id={0}", idEntrega));
                        }
                        else
                        {
                            Pesquisar(idEntrega, false);
                            throw new Exception("Erro ao Salvar EPI:");
                        }
                    }
                    else
                        throw new Exception("BD: " + sErro.ToString());
                }
                catch (Exception ex)
                {
                    MensagemPagina_Entrega.MostraMensagem_Erro(ex.Message);
                }
            }
        }

        bool Salvar_EPI(string idEntrega)
        {
            try
            {
                foreach (var EPI_Linha in Lista_EPIs)
                {
                    if (EPI_Linha.sFuncao == "CONSULTA_EPI")
                        continue;

                    Dictionary<string, string> vParametroItensEPI = new Dictionary<string, string>()
                    {
                        { "@idEntregaEPI", idEntrega },
                        { "@idStatus", hddidStatus.Value },
                        { "@sFuncao", EPI_Linha.sFuncao },
                        { "@idRegistro", EPI_Linha.idRegistroEPI.ToString() },
                        { "@idProduto", EPI_Linha.idItem.ToString() },
                        { "@idEstado", EPI_Linha.idEstado.ToString() },
                        { "@sDscProduto", EPI_Linha.sDscEPI.ToString() },
                        { "@nQuantidadeEPI", EPI_Linha.nQuantidateEPI.ToString() },
                        { "@nQuantidadePeriodo", EPI_Linha.nQuantidadeTempo.ToString() },
                        { "@nCA", EPI_Linha.nCA.ToString() },
                        { "@sTipoPeriodo", EPI_Linha.sTipoPeriodo.ToString() },
                        { "@sRegistra_Log", Request["id"] == "0" ? "N" : "S" },
                        { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
                    };
                    BD.ExecutarDataSet(sProcedure, vParametroItensEPI);
                }

                return true;
            }
            catch (Exception ex)
            {
                MensagemPagina_Entrega.MostraMensagem_Erro("Erro ao Salvar os EPIs: " + ex.Message);
                return false;
            }
        }

        void AtualizaClasses_EPI(bool bValidaDuplicado)
        {
            foreach (GridViewRow row in dtgEPI.Rows)
            {
                try
                {
                    var EPI = Lista_EPIs.Where(ep => ep.idLinha.Equals(int.Parse(row.Cells[0].Text))).First();

                    int nCA = Convert.ToInt32((row.FindControl("txtCA") as TextBox).Text);
                    int qtdEPI = Convert.ToInt32((row.FindControl("txtnQuantidateEPI") as TextBox).Text);
                    int qtdTempo = Convert.ToInt32((row.FindControl("txtnQuantidadeTempo") as TextBox).Text);
                    string sPeriodo = (row.FindControl("ddlsTipoPeriodo") as DropDownList).SelectedValue;

                    if (EPI.nCA != nCA || EPI.nQuantidateEPI != qtdEPI || EPI.nQuantidadeTempo != qtdTempo || EPI.sTipoPeriodo != sPeriodo) EPI.sFuncao = "INCLUIR_EPI";

                    EPI.nCA = nCA;
                    EPI.nQuantidateEPI = qtdEPI;
                    EPI.nQuantidadeTempo = qtdTempo;
                    EPI.sTipoPeriodo = sPeriodo;

                    if (ddlTipo.SelectedValue == "4")
                    {
                        int idEstado = Convert.ToInt32((row.FindControl("ddlEstado") as DropDownList).SelectedValue);

                        if (EPI.idEstado != idEstado) EPI.sFuncao = "INCLUIR_EPI";
                        EPI.idEstado = idEstado;
                    }
                }
                catch { }
            }

            if (bValidaDuplicado && Lista_EPIs.Where(e => !e.sFuncao.Equals("EXCLUIR_EPI")).GroupBy(e => (e.idItem, e.nCA)).Where(e => e.Count() > 1).Any())
                MensagemPagina_Entrega.MostraMensagem_Erro("Não é possível que dois EPIs iguais sejam incluídos utilizando o mesmo CA!", false);
        }

        #endregion

        #region | Utils

        void Popular_dtgEPI(DataSet dsPesquisa)
        {
            Lista_EPIs.Clear();

            foreach (DataRow row in dsPesquisa.Tables[1].Rows)
            {
                cls_EPI_x_Funcao epi = new cls_EPI_x_Funcao
                {
                    idRegistroEPI = Convert.ToInt32(row["idRegistro"].ToString()),
                    idLinha = Lista_EPIs.Count + 1,
                    sFuncao = hddConferencia.Value != "0" ? "INCLUIR_EPI" : "CONSULTA_EPI",
                    idItem = Convert.ToInt32(row["idProduto"].ToString()),
                    sCodigoEPI = row["sCodigoEPI"].ToString(),
                    sDscEPI = row["sDscEPI"].ToString(),
                    nCA = Convert.ToInt32(row["nCA"].ToString()),
                    idEstado = Convert.ToInt32(row["idEstado"].ToString()),
                    nQuantidateEPI = Convert.ToInt32(row["nQuantidadeEPI"].ToString()),
                    nQuantidadeTempo = Convert.ToInt32(row["nQuantidadeTempo"].ToString()),
                    sTipoPeriodo = row["sTipoPeriodo"].ToString(),
                    vbArquivo = row["vbArquivo"]
                };
                Lista_EPIs.Add(epi);
            }

            Popula_IncluirEPIs(dsPesquisa.Tables[3]);
            PopulaColaboradores(dsPesquisa.Tables[4]);

            dtgEPI_DataBind();
            LimpaCampos_EPI();
        }

        void Popular_Entregas_Vinculadas(DataTable dt) { Lista_Entregas_Vinculadas.Clear(); foreach (DataRow row in dt.Rows) Lista_Entregas_Vinculadas.Add(Convert.ToInt32(row["idEntregaEPI"]), Convert.ToDateTime(row["dtSolicitacao"])); }

        void LimpaCampos()
        {
            hddidEntregaEPI.Value = "0";
            txtNome_Colaborador.Text = "";
            txtCPF_Colaborador.Text = "";
            txtNome_Colaborador.Text = "";
            ddlTipo.SelectedValue = "0";
            txtCodigo_EPI.Text = "";
            txtDesc_EPI.Text = "";
            txtIncluirEPI_CA.Text = "";
            txtIncluirEPI_Qtd.Text = "";
            txtIncluirEPI_nTempo.Text = "";
            ddlIncluirEPI_sTipoPeriodo.SelectedValue = "H";
            txtObs.Text = "";

            PainelAtualizacao.Visible = false;
        }

        private bool ValidarDados()
        {
            if (hddidSelecionarColaborador.Value == "0")
            {
                MensagemPagina_Entrega.MostraMensagem_Erro("É necessário selecionar um Colaborador!", false);
                return false;
            }
            else if (ddlTipo.SelectedValue == "0")
            {
                MensagemPagina_Entrega.MostraMensagem_Erro("É necessário selecionar o Tipo da Entrega!", false);
                return false;
            }
            else if (!DateTime.TryParse(txtdtSolicitacao.Text, out DateTime dtSolicitacao) || dtSolicitacao.Year <= 2000)
            {
                MensagemPagina_Entrega.MostraMensagem_Erro("É necessário definir a Data de Solicitação!", false);
                return false;
            }
            else if (!DateTime.TryParse(txtdtEntrega.Text, out DateTime dtEntrega) || dtEntrega.Year <= 2000)
            {
                MensagemPagina_Entrega.MostraMensagem_Erro("É necessário definir a Data de Entrega!", false);
                return false;
            }
            else if (!Lista_EPIs.Where(e => e.sFuncao != "EXCLUIR_EPI").Any())
            {
                MensagemPagina_Entrega.MostraMensagem_Erro("É necessário incluir ao menos um EPI!", false);
                return false;
            }
            else if (Lista_EPIs.Where(e => !e.sFuncao.Equals("EXCLUIR_EPI")).GroupBy(e => (e.idItem, e.nCA)).Where(e => e.Count() > 1).Any())
            {
                MensagemPagina_Entrega.MostraMensagem_Erro("Não é possível que dois (ou mais) EPIs iguais sejam incluídos utilizando o mesmo CA!", false);
                return false;
            }

            return true;
        }

        protected void PopulaColaboradores(DataTable dt = null) { hddColaboradores.Value = string.Empty; foreach (DataRow row in dt.Rows) hddColaboradores.Value += string.Format("[{0}|{1}|{2}]", row["idColaborador"].ToString(), row["sCPF"].ToString(), row["sDscColaborador"].ToString()); }

        protected void Popula_IncluirEPIs(DataTable dt) { hddEPIs.Value = string.Empty; foreach (DataRow row in dt.Rows) hddEPIs.Value += string.Format("[{0}|{1}|{2}]", row["idItem"].ToString(), row["sCodigo"].ToString(), row["sDscProduto"].ToString()); }

        protected void Popular_ListaPadrao()
        {
            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>()
                {
                    { "@sFuncao", "CONSULTAR_EPIs_x_COLABORADOR" },
                    { "@idColaborador", hddidSelecionarColaborador.Value }
                };
                DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_FuncaoCarteira", vParametros);

                if (BD.ValidarDataSet(ds))
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int id = int.Parse(row["idItem"].ToString());

                        var item = Lista_EPIs.Where(e => e.idItem.Equals(id)).FirstOrDefault();

                        if (item == null)
                        {
                            cls_EPI_x_Funcao epi = new cls_EPI_x_Funcao
                            {
                                sFuncao = "INCLUIR_EPI",
                                idLinha = Lista_EPIs.Count + 1,
                                idItem = id,
                                sDscEPI = row["sDscProduto"].ToString(),
                                sCodigoEPI = row["sCodigo"].ToString(),
                                nCA = 0,
                                nQuantidateEPI = int.Parse(row["nQuantidateEPI"].ToString()),
                                nQuantidadeTempo = int.Parse(row["nQuantidadeTempo"].ToString()),
                                sTipoPeriodo = row["sTipoPeriodo"].ToString(),
                                vbArquivo = row["vbArquivo"]
                            };
                            Lista_EPIs.Add(epi);
                        }
                        else
                        {
                            item.sFuncao = "INCLUIR_EPI";
                            item.nCA = 0;
                            item.nQuantidateEPI = int.Parse(row["nQuantidateEPI"].ToString());
                            item.nQuantidadeTempo = int.Parse(row["nQuantidadeTempo"].ToString());
                            item.sTipoPeriodo = row["sTipoPeriodo"].ToString();
                        }
                    }

                    dtgEPI_DataBind();
                }
                else
                    MensagemPagina_Entrega.MostraMensagem_Aviso("Não foi encontrada uma Lista Padrão de EPIs para a Função do Colaborador selecionado!", false);
            }
            catch { }
        }

        protected void Popular_EPIs_Colaborador()
        {
            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>()
                {
                    { "@sFuncao", "CONSULTAR_EPI_X_COLABORADOR" },
                    { "@idColaborador", hddidSelecionarColaborador.Value }
                };
                DataTable dt = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Colaboradores", vParametros);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        int id = int.Parse(row["idEPI"].ToString());
                        var item = Lista_EPIs.Where(e => e.idItem.Equals(id)).FirstOrDefault();

                        if (item == null)
                        {
                            cls_EPI_x_Funcao epi = new cls_EPI_x_Funcao
                            {
                                sFuncao = "INCLUIR_EPI",
                                idLinha = Lista_EPIs.Count + 1,
                                idItem = id,
                                sDscEPI = row["sDscEPI"].ToString(),
                                sCodigoEPI = row["sCodigoEPI"].ToString(),
                                nCA = 0,
                                nQuantidateEPI = int.Parse(row["nQuantidade"].ToString()),
                                nQuantidadeTempo = int.Parse(row["nQuantidadeTempo"].ToString()),
                                sTipoPeriodo = row["sTipoPeriodo"].ToString(),
                                vbArquivo = row["vbArquivo"]
                            };
                            Lista_EPIs.Add(epi);
                        }
                        else
                        {
                            item.sFuncao = "INCLUIR_EPI";
                            item.nQuantidateEPI = int.Parse(row["nQuantidateEPI"].ToString());
                        }
                    }

                    dtgEPI_DataBind();
                }
                else
                    MensagemPagina_Entrega.MostraMensagem_Aviso("Não foram encontrados os EPIs deste Colaborador!", false);
            }
            catch { }
        }

        void LimpaCampos_EPI()
        {
            hddIncluir_idEPI.Value = "0";
            txtCodigo_EPI.Text = string.Empty;
            txtDesc_EPI.Text = string.Empty;
            txtIncluirEPI_CA.Text = "0";
            txtIncluirEPI_Qtd.Text = "1";
            txtIncluirEPI_nTempo.Text = "1";
            ddlIncluirEPI_sTipoPeriodo.SelectedValue = "H";

            cmdEPI_Incluir.Visible = true;
        }

        private bool ValidarDados_EPI()
        {
            if (hddIncluir_idEPI.Value == "0")
            {
                MensagemPagina_Incluir_EPI.MostraMensagem_Erro("É necessário selecionar um EPI!");
                return false;
            }
            else if (txtCodigo_EPI.Text.Length < 3)
            {
                MensagemPagina_Incluir_EPI.MostraMensagem_Erro("É necessário preencher o campo de Código do EPI com ao menos 3 caracteres!");
                return false;
            }
            else if (txtDesc_EPI.Text.Length < 5)
            {
                MensagemPagina_Incluir_EPI.MostraMensagem_Erro("É necessário preencher o campo de Descrição do EPI com ao menos 5 caracteres!");
                return false;
            }

            return true;
        }

        protected void AvancaStatus(string idEntregaEPI) => BD.ExecutarDataSet(sProcedure, new Dictionary<string, string>() { { "@sFuncao", "AVANCA_FLUXO_STATUS" }, { "@idEntregaEPI", idEntregaEPI }, { "@idUsuarioAtualizacao", Variaveis.idUsuario() } });

        #endregion

        #region | Eventos

        protected void cmdSelecionarColaborador_Click(object sender, EventArgs e)
        {
            if (hddidSelecionarColaborador.Value != hddidSelecionarColaborador_Original.Value) Lista_EPIs.Clear();

            if (hddidSelecionarColaborador.Value != "0")
            {
                div_ddlTipo.Visible = true;

                hddidSelecionarColaborador_Original.Value = hddidSelecionarColaborador.Value;

                if (ddlTipo.SelectedValue == "1") Popular_ListaPadrao();
                else if (ddlTipo.SelectedValue == "3" || ddlTipo.SelectedValue == "4") Popular_EPIs_Colaborador();

                FUNCOES.Scripts.FocusScript(Page, ddlTipo.ClientID);
            }
            else
            {
                ddlTipo.SelectedValue = "0";
                ddlTipo_SelectedIndexChanged(null, null);

                MensagemPagina_Entrega.MostraMensagem_Erro("É necessário selecionar um Colaborador!", false);
                FUNCOES.Scripts.FocusScript(Page, txtCPF_Colaborador.ClientID);
            }

            dtgEPI_DataBind();
        }

        protected void cmdEPI_Incluir_Click(object sender, EventArgs e)
        {
            AtualizaClasses_EPI(true);

            if (ValidarDados_EPI())
            {
                string[] vidFuncao = hddidEntregaEPI.Value.Split(',');
                string idFuncao = vidFuncao[0].ToString().Trim();
                int nCA = !string.IsNullOrEmpty(txtIncluirEPI_CA.Text) && !string.IsNullOrWhiteSpace(txtIncluirEPI_CA.Text) ? int.Parse(txtIncluirEPI_CA.Text) : 0;
                var item = Lista_EPIs.Where(ep => ep.idItem.Equals(int.Parse(hddIncluir_idEPI.Value)) && ep.nCA.Equals(nCA)).FirstOrDefault();

                if (item == null)
                {
                    Dictionary<string, string> vParam = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTA_IMAGEM_PRINCIPAL" },
                        { "@idItem", hddIncluir_idEPI.Value }
                    };
                    DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos", vParam);

                    cls_EPI_x_Funcao epi = new cls_EPI_x_Funcao
                    {
                        idLinha = Lista_EPIs.Count + 1,
                        sFuncao = "INCLUIR_EPI",
                        idFuncao = Convert.ToInt32(idFuncao),
                        idItem = Convert.ToInt32(hddIncluir_idEPI.Value),
                        sCodigoEPI = txtCodigo_EPI.Text,
                        sDscEPI = txtDesc_EPI.Text,
                        nCA = nCA,
                        nQuantidateEPI = !string.IsNullOrEmpty(txtIncluirEPI_Qtd.Text) && !string.IsNullOrWhiteSpace(txtIncluirEPI_Qtd.Text) ? int.Parse(txtIncluirEPI_Qtd.Text) : 1,
                        nQuantidadeTempo = !string.IsNullOrEmpty(txtIncluirEPI_nTempo.Text) && !string.IsNullOrWhiteSpace(txtIncluirEPI_nTempo.Text) ? int.Parse(txtIncluirEPI_nTempo.Text) : 1,
                        sTipoPeriodo = ddlIncluirEPI_sTipoPeriodo.SelectedValue,
                        vbArquivo = RETORNO.DATASET(ds, "vbArquivo")
                    };
                    Lista_EPIs.Add(epi);
                }
                else if (item.sFuncao == "EXCLUIR_EPI") item.sFuncao = "INCLUIR_EPI";
                else MensagemPagina_Incluir_EPI.MostraMensagem_Erro("Não é possível adicionar um EPI já incluído com o mesmo CA!", false);

                dtgEPI_DataBind();
                LimpaCampos_EPI();
            }

            FUNCOES.Scripts.FocusScript(Page, txtCodigo_EPI.ClientID);
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "epi-tab");
        }

        protected void ddlTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlTipo.SelectedValue != "0")
            {
                Lista_EPIs.Clear();

                div_EPIs.Visible = true;
                div_dtgEPI.Visible = true;
                div_Obs.Visible = true;
                divIncluir_EPI.Visible = ddlTipo.SelectedValue != "4";
                div_ComboAtivo.Visible = hddidEntregaEPI.Value != "0";

                if (ddlTipo.SelectedValue == "4")
                {
                    sTituloPagina = "Conferência";
                    lbl_dtSolicitacao.InnerText = "Data da Conferência";
                    txtdtSolicitacao.ReadOnly = true;
                    div_dtEntrega.Visible = false;
                }
                else
                {
                    sTituloPagina = "Entrega de EPI";
                    lbl_dtSolicitacao.InnerText = "Data da Solicitação";
                    txtdtSolicitacao.ReadOnly = false;
                    div_dtEntrega.Visible = true;
                }

                lblTituloPagina.Text = string.Format(hddidEntregaEPI.Value != "0" ? "{0} para {1}" : "Nova {0}", sTituloPagina, txtNome_Colaborador.Text);
                aba_EntregaEPI.InnerHtml = $"<b>{sTituloPagina}</b>";
                tituloPagina.InnerHtml = $"<b>{sTituloPagina}</b>";

                if (ddlTipo.SelectedValue == "1") Popular_ListaPadrao();
                else if (ddlTipo.SelectedValue == "3" || ddlTipo.SelectedValue == "4") Popular_EPIs_Colaborador();

                FUNCOES.Scripts.FocusScript(Page, txtdtSolicitacao.ClientID);
            }
            else
            {
                div_EPIs.Visible = false;
                divIncluir_EPI.Visible = false;
                div_dtgEPI.Visible = false;
                div_Obs.Visible = false;
                div_ComboAtivo.Visible = false;

                MensagemPagina_Entrega.MostraMensagem_Erro("É necessário selecionar um Tipo de Entrega!", false);
                FUNCOES.Scripts.FocusScript(Page, ddlTipo.ClientID);
            }

            dtgEPI_DataBind();
        }

        protected void cmdEntregarEPI_Click(object sender, EventArgs e)
        {
            DataSet ds = BD.ExecutarDataSet(sProcedure, new Dictionary<string, string>() { { "@sFuncao", "VERIFICA_ARQUIVOS__ENTREGAR_EPI" }, { "@idEntregaEPI", hddidEntregaEPI.Value } });

            if (BD.ValidarDataSet(ds))
            {
                Session["EntregarEPI"] = true;
                AvancaStatus(hddidEntregaEPI.Value);
                FUNCOES.DirecionaPagina(string.Format("App/Paginas/RRHH/EntregaEPI_Detalhe.aspx?id={0}", hddidEntregaEPI.Value));
            }
            else
                MensagemPagina_Entrega.MostraMensagem_Erro("É necessário subir o Arquivo do Documento PDF da Entrega de EPI, já assinado, na aba Arquivos!", true);
        }

        protected void cmdConfirmar_Click(object sender, EventArgs e)
        {
            Session["ConfirmarEntrega"] = true;
            AvancaStatus(hddidEntregaEPI.Value);
            FUNCOES.DirecionaPagina(string.Format("App/Paginas/RRHH/EntregaEPI_Detalhe.aspx?id={0}", hddidEntregaEPI.Value));
        }

        protected void cmdEPI_Excluir_Click(object sender, EventArgs e)
        {
            AtualizaClasses_EPI(false);
            foreach (GridViewRow row in dtgEPI.Rows) { if ((row.FindControl("cbEPI_Excluir") as CheckBox).Checked) Lista_EPIs.FirstOrDefault(epi => epi.idLinha == int.Parse(row.Cells[0].Text)).sFuncao = "EXCLUIR_EPI"; }
            dtgEPI_DataBind();
        }

        protected void cmdSubstitutiva_Click(object sender, EventArgs e)
        {
            lstSubstituicoes.Items.Clear();
            foreach (var x in Lista_Entregas_Vinculadas) lstSubstituicoes.Items.Add(new ListItem($"<a href='/App/Paginas/RRHH/EntregaEPI_Detalhe.aspx?id={x.Key}' target='_blank'>Criada em {x.Value:dd/MM/yyyy}</a>", x.Key.ToString()));
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_LinkSubstitutivas", "linkSubstitutivas();", true);
            FUNCOES.Scripts.AbrirModal(Page, "modalSubs");
        }

        protected void ddlEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            string idEstado = (sender as DropDownList).SelectedValue;
            var row = (sender as DropDownList).NamingContainer as GridViewRow;

            if (!string.IsNullOrEmpty(idEstado) && idEstado != "0")
            {
                switch (idEstado)
                {
                    case "1":
                        row.CssClass = "success";
                        break;
                    case "2":
                        row.CssClass = "info";
                        break;
                    case "3":
                        row.CssClass = "warning";
                        break;
                    case "4":
                    case "5":
                        row.CssClass = "danger";
                        break;
                }
            }
        }

        #endregion

        #region | PDF

        protected void cmdGerarPDF_Click(object sender, EventArgs e)
        {
            try
            {
                AtualizaClasses_EPI(true);

                ReportViewer rv = new ReportViewer();

                rv.LocalReport.EnableExternalImages = true;
                rv.LocalReport.ReportPath = @"App\Reports\EntregaEPI.rdlc";

                if (!DateTime.TryParse(txtdtEntrega.Text, out DateTime dtEntrega) || dtEntrega.Year < 2000) throw new Exception("A Data de Entrega é obrigatória!");

                Dictionary<string, string> vParametrosEmpresa = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_EMPRESA_x_COLABORADOR" },
                    { "@idColaborador", hddidSelecionarColaborador.Value }
                };
                DataSet ds = BD.ExecutarDataSet(sProcedure, vParametrosEmpresa);

                if (!BD.ValidarDataSet(ds, out string sErro)) throw new Exception(sErro);

                string razaoSocial = !string.IsNullOrEmpty(RETORNO.DATASET(ds, "sRazaoSocial").Trim()) ? RETORNO.DATASET(ds, "sRazaoSocial") : throw new Exception("Não foi possível consultar o Nome da Empresa, a partir do cadastro do Colaborador!");
                string cnpj = !string.IsNullOrEmpty(RETORNO.DATASET(ds, "sCPF_CNPJ").Trim()) ? RETORNO.DATASET(ds, "sCPF_CNPJ") : throw new Exception("Não foi possível consultar o CNPJ da Empresa, a partir do cadastro do Colaborador!");
                string nome = !string.IsNullOrEmpty(RETORNO.DATASET(ds, "sDscColaborador").Trim()) ? RETORNO.DATASET(ds, "sDscColaborador") : throw new Exception("Não foi possível consultar o Nome do Colaborador, a partir do cadastro do Colaborador!");
                string funcao = !string.IsNullOrEmpty(RETORNO.DATASET(ds, "sFuncao").Trim()) ? RETORNO.DATASET(ds, "sFuncao") : throw new Exception("Não foi possível consultar a Função do Colaborador, a partir do cadastro do Colaborador!");
                string cpf = !string.IsNullOrEmpty(RETORNO.DATASET(ds, "sCPF").Trim()) ? RETORNO.DATASET(ds, "sCPF") : throw new Exception("Não foi possível consultar o CPF do Colaborador, a partir do cadastro do Colaborador!");

                List<cls_EPI_x_Funcao> list_EPI = new List<cls_EPI_x_Funcao>();

                Lista_EPIs.Where(x => x.sFuncao != "EXCLUIR_ITEM").ToList().ForEach(x =>
                {
                    string sTipoArquivo = "image/jpeg";

                    try
                    {
                        if (x.vbArquivo != null) sTipoArquivo = ARQUIVO.RetornaContentTypeArquivo(string.Empty, x.vbArquivo);

                        if (!sTipoArquivo.StartsWith("image/"))
                            sTipoArquivo = "image/jpeg";
                        else if (sTipoArquivo.Contains("webp"))
                        {
                            sTipoArquivo = "image/png";
                            x.vbArquivo = ARQUIVO.ConverterImagem_PNG(x.vbArquivo);
                        }
                    }
                    catch { }

                    cls_EPI_x_Funcao epi = new cls_EPI_x_Funcao
                    {
                        sDscEPI = $"{x.sCodigoEPI} - {x.sDscEPI}",
                        nQuantidateEPI = x.nQuantidateEPI,
                        sCA = int.Parse(x.nCA.ToString().PadLeft(5, '0')).ToString(@"00\.000"),
                        vbArquivo = x.vbArquivo,
                        sTipoArquivo = sTipoArquivo
                    };
                    list_EPI.Add(epi);
                });

                rv.LocalReport.DataSources.Add(new ReportDataSource("ds_EPIs", list_EPI));
                rv.LocalReport.DataSources.Add(new ReportDataSource("ds_Vazio", new List<string>() { "vazio" }));

                if (!int.TryParse(hddidEntregaEPI.Value, out int idEntrega) || idEntrega <= 0) throw new Exception("Não foi possível identificar a Entrega de EPI para gerar o código de barras!");

                byte[] codigoBarras = CODIGOBARRAS.GerarCodigoBarras(CODIGOBARRAS.TipoRelatorioCodigoBarras.EPI, idEntrega);

                ReportParameter[] rp = new ReportParameter[8];

                rp[0] = new ReportParameter("Empresa_RazaoSocial", razaoSocial);
                rp[1] = new ReportParameter("Empresa_CNPJ", FUNCOES.Formatar_CNPJ_CPF(cnpj));
                rp[2] = new ReportParameter("Colaborador_Nome", nome);
                rp[3] = new ReportParameter("Colaborador_CPF", FUNCOES.Formatar_CNPJ_CPF(cpf));
                rp[4] = new ReportParameter("Colaborador_Funcao", funcao);
                rp[5] = new ReportParameter("Data_EntregaEPI", dtEntrega.ToString("dd/MM/yyyy"));
                rp[6] = new ReportParameter("Logo_Empresa", Request.Url.GetLeftPart(UriPartial.Authority) + "/App/img/Logo_Empresa.png" ?? "");
                //rp[6] = new ReportParameter("Logo_Empresa", Server.MapPath("~/App/img/Logo_Empresa.png") ?? "");
                rp[7] = new ReportParameter("CodigoBarras_Base64", Convert.ToBase64String(codigoBarras));
                //rp[8] = new ReportParameter("CodigoBarras_Texto", CODIGOBARRAS.MontarIdentificador(CODIGOBARRAS.TipoRelatorioCodigoBarras.EPI, idEntrega));

                rv.LocalReport.SetParameters(rp);
                rv.LocalReport.Refresh();

                byte[] bytes = rv.LocalReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings);
                string sNomeArquivo = "Entrega_EPI_" + hddidEntregaEPI.Value.PadLeft(6, '0') + "_" + FUNCOES.CarimboDataHora() + ".pdf";
                File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivo, bytes);

                FUNCOES.DownloadArquivo(Page, sNomeArquivo);

                if (ddlidStatus.SelectedValue == "1")
                {
                    AvancaStatus(hddidEntregaEPI.Value);
                    MensagemPagina_Entrega.MostraMensagem("Ao gerar o Relatório em PDF, o Status da Entrega de EPI é atualizado, e passa a ser 'Pendente de Documentação', faltando apenas o Usuário subir o Arquivo do Relatório em PDF, já assinado, na aba Arquivos!", "info", false);
                }

                Pesquisar(hddidEntregaEPI.Value, false);
                MensagemPagina_Entrega.MostraMensagem_Sucesso("Relatório em PDF gerado com sucesso!", true);
            }
            catch (Exception ex)
            {
                MensagemPagina_Entrega.MostraMensagem_Erro($"Houve um erro ao Gerar o Relatório em PDF da Entrega de EPI!<br />Erro: {ex.Message}{(ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message) ? "<br />Erro interno: " + ex.InnerException.Message : "")}", true);
            }
        }

        #endregion

        #region | Script

        protected void RegistraScript()
        {
            FUNCOES.Scripts.Aplica_TooltipPersonalizado(Page, "tooltip");

            RegistraScript_Personalizado(txtCPF_Colaborador.ClientID, true, false);
            RegistraScript_Personalizado(txtNome_Colaborador.ClientID, false, false);

            RegistraScript_Personalizado(txtCodigo_EPI.ClientID, true, true);
            RegistraScript_Personalizado(txtDesc_EPI.ClientID, false, true);

            StringBuilder sb = new StringBuilder();

            // Evento do campo de Data de Entrega
            sb.AppendLine("$(document).ready(function() {");
            sb.AppendLine("     var entregaAlterada = false;");
            sb.AppendLine("     $('#" + txtdtEntrega.ClientID + "').on('input, change', function() {");
            sb.AppendLine("         entregaAlterada = true;");
            sb.AppendLine("     });");
            sb.AppendLine("     $('#" + txtdtEntrega.ClientID + "').on('blur', function() {");
            sb.AppendLine("         if (entregaAlterada) {");
            sb.AppendLine("             $('#" + hddsDataEntrega_Manual.ClientID + "').val('S');");
            sb.AppendLine("         }");
            sb.AppendLine("     });");
            sb.AppendLine("     $('.excluirTodos input').on('change', function() {");
            sb.AppendLine("         var excluir = $(this).prop('checked');\r\n");
            sb.AppendLine("         $('.excluir input').each(function () {\r\n");
            sb.AppendLine("             $(this).prop('checked', excluir);\r\n");
            sb.AppendLine("         });\r\n");
            sb.AppendLine("     });");
            sb.AppendLine("});");

            sb.AppendLine("");

            sb.AppendLine("function linkSubstitutivas() {");
            sb.AppendLine("     $('ul.lstSubstituicoes li').each(function() {");
            sb.AppendLine("         $(this).html($(this).text());");
            sb.AppendLine("     });");
            sb.AppendLine("}");

            sb.AppendLine(" ");

            // Máscara
            sb.Append("$('.qtd').mask('99999', { reverse: true });\r\n\r\n");

            sb.Append("$v192(function() {\r\n");

            // Modal Salvar
            sb.Append("     $v192(\"#dialog-Salvar\").dialog({\r\n");
            sb.Append("         resizable: false,\r\n");
            sb.Append("         height: \"auto\",\r\n");
            sb.Append("         width: 400,\r\n");
            sb.Append("         modal: true,\r\n");
            sb.Append("         autoOpen: false,\r\n");
            sb.Append("         buttons:\r\n");
            sb.Append("         {\r\n");
            sb.Append("             \"Sim\": function() {\r\n");
            sb.Append("                 __doPostBack(\"funcao_SALVAR\", \"\");\r\n");
            sb.Append("                 $v192(this).dialog(\"close\");\r\n");
            sb.Append("             },\r\n");
            sb.Append("             \"Não\": function() {\r\n");
            sb.Append("                 $v192(this).dialog(\"close\");\r\n");
            sb.Append("             },\r\n");
            sb.Append("         }\r\n");
            sb.Append("     });\r\n");
            sb.Append("     $v192('[id*=cmdSalvar]').click(function(e) {\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         $('[id*=hdd_ID_cmdSalvar]').val($(this).attr('id'));\r\n");
            sb.Append("         $v192('#dialog-Salvar').dialog('open');\r\n");
            sb.Append("     });\r\n\r\n");

            // Modal Editar
            sb.Append("     $v192(\"#dialog-Editar\").dialog({\r\n");
            sb.Append("         resizable: false,\r\n");
            sb.Append("         height: \"auto\",\r\n");
            sb.Append("         width: 400,\r\n");
            sb.Append("         modal: true,\r\n");
            sb.Append("         autoOpen: false,\r\n");
            sb.Append("         buttons:\r\n");
            sb.Append("             {\r\n");
            sb.Append("                 \"Sim\": function() {\r\n");
            sb.Append("                     __doPostBack(\"funcao_EDITAR\", \"\");\r\n");
            sb.Append("                     $v192(this).dialog(\"close\");\r\n");
            sb.Append("                 },\r\n");
            sb.Append("                 \"Não\": function() {\r\n");
            sb.Append("                     $v192(this).dialog(\"close\");\r\n");
            sb.Append("                 },\r\n");
            sb.Append("             }\r\n");
            sb.Append("     });\r\n");
            sb.Append("     $v192('[id*=cmdEditar]').click(function(e) {\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         $v192('#dialog-Editar').dialog('open');\r\n");
            sb.Append("     });\r\n\r\n");

            sb.Append("});\r\n\r\n");

            // Exibir Links em outra Aba do navegador
            sb.Append("$('.link').find('a').click(function() { window.open(window.location.protocol + '//' + window.location.host + '/App/Paginas' + $(this).attr('href'), '_blank'); return false; });\r\n\r\n");

            // Card de informações dos EPIs
            sb.Append("     var cardTimer = { };\r\n");
            sb.Append("     function mostraCard(element, idProduto, tabela) {\r\n");
            sb.Append("         cardTimer[idProduto + '_' + tabela] = setTimeout(function() {\r\n");
            sb.Append("             $.ajax({\r\n");
            sb.Append("                 url: \"/API/Pagina_Ajax.aspx/GetProdutoDetalhes\",\r\n");
            sb.Append("                 data: JSON.stringify({ idProduto: idProduto }),\r\n");
            sb.Append("                 type: 'POST',\r\n");
            sb.Append("                 dataType: 'json',\r\n");
            sb.Append("                 contentType: 'application/json; charset=utf-8',\r\n");
            sb.Append("                 success: function(response) {\r\n");
            sb.Append("                     var produto = JSON.parse(response.d);\r\n");
            sb.Append("                     var cardProduto = `\r\n");
            sb.Append("                         <div class=\"card\">\r\n");
            sb.Append("                             <div class=\"card-body d-flex\">\r\n");
            sb.Append("                                 <div class=\"flex-shrink-0\" style=\"min-inline-size: fit-content;\">\r\n");
            sb.Append("                                     ${produto.imagem? `<img src = \"${produto.imagem}\" alt=\"Imagem do Produto\" class=\"img-fluid img-thumbnail\" style=\"width: 100px; height: auto;\" />` : ''}\r\n");
            sb.Append("                                 </div>\r\n");
            sb.Append("                                 <div class=\"flex-grow-1 d-flex flex-column ms-3\">\r\n");
            sb.Append("                                     <div class=\"d-flex\">\r\n");
            sb.Append("                                         ${produto.sCategoriaVendas? `<div class=\"card-text me-3\"> <strong>Categoria Vendas: </strong>${produto.sCategoriaVendas\r\n}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sFabricante ? `<div class= \"card-text me-3\"> <strong > Fabricante: </strong >${ produto.sFabricante}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sTipo ? `<div class= \"card-text me-3\"> <strong > Tipo: </strong >${ produto.sTipo}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sGrupo ? `<div class= \"card-text me-3\"> <strong > Grupo: </strong >${ produto.sGrupo}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sFamilia ? `<div class= \"card-text me-3\"> <strong > Família: </strong >${ produto.sFamilia}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sPaisOrigem ? `<div class= \"card-text me-3\"> <strong > Origem: </strong >${ produto.sPaisOrigem}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sLocalArmazenamento ? `<div class= \"card-text me-3\"> <strong > Local Armazenamento: </strong >${ produto.sLocalArmazenamento}</div>` : ''}\r\n");
            sb.Append("                                     </div>\r\n");
            sb.Append("                                 </div>\r\n");
            sb.Append("                             </div>\r\n");
            sb.Append("                         </div>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                     var cardId = idProduto + '_' + tabela;\r\n");
            sb.Append("                     var card = document.getElementById(cardId);\r\n");
            sb.Append("                     card.innerHTML = cardProduto;\r\n");
            sb.Append("                     var rect = element.getBoundingClientRect();\r\n");
            sb.Append("                     var scrollTop = document.documentElement.scrollTop || document.body.scrollTop;\r\n");
            sb.Append("                     var scrollLeft = document.documentElement.scrollLeft || document.body.scrollLeft;\r\n");
            sb.Append("                     hideAllCards();\r\n");
            sb.Append("                     card.style.top = (rect.top + scrollTop - 10) + 'px';\r\n");
            sb.Append("                     card.style.left = (rect.right + scrollLeft + element.offsetWidth + 10) + 'px';\r\n");
            sb.Append("                     card.style.display = 'block';\r\n");
            sb.Append("                 },\r\n");
            sb.Append("                 error: function(error) {\r\n");
            sb.Append("                     console.error(\"Erro ao obter os detalhes do produto:\", error);\r\n");
            sb.Append("                 }\r\n");
            sb.Append("             });\r\n");
            sb.Append("         }, 300);\r\n");
            sb.Append("     }\r\n\r\n");
            sb.Append("     function escondeCard(idProduto, tabela) {\r\n");
            sb.Append("         var cardId = idProduto + '_' + tabela;\r\n");
            sb.Append("         var card = document.getElementById(cardId);\r\n");
            sb.Append("         clearTimeout(cardTimer[idProduto + '_' + tabela]);\r\n");
            sb.Append("         card.style.display = 'none';\r\n");
            sb.Append("     }\r\n\r\n");
            sb.Append("     function hideAllCards() {\r\n");
            sb.Append("         var cards = document.querySelectorAll('.product-card');\r\n");
            sb.Append("         cards.forEach(function(card) {\r\n");
            sb.Append("             card.style.display = 'none';\r\n");
            sb.Append("         });\r\n");
            sb.Append("     }\r\n\r\n");
            sb.Append("     function openModal(idProduto) {\r\n");
            sb.Append("         $.ajax({\r\n");
            sb.Append("             url: \"/API/Pagina_Ajax.aspx/GetProdutoDetalhes\",\r\n");
            sb.Append("             data: JSON.stringify({ idProduto: idProduto }),\r\n");
            sb.Append("             type: 'POST',\r\n");
            sb.Append("             dataType: 'json',\r\n");
            sb.Append("             contentType: 'application/json; charset=utf-8',\r\n");
            sb.Append("             success: function(response) {\r\n");
            sb.Append("                 var produto = JSON.parse(response.d);\r\n");
            sb.Append("                 var tituloProduto = `\r\n");
            sb.Append("                     <button type = \"button\" class= \"close\" data - dismiss = \"modal\" aria - label = \"Close\">\r\n");
            sb.Append("                         <span aria - hidden = \"true\" > &times;</span>\r\n");
            sb.Append("                     </button>\r\n");
            sb.Append("                     <h5 class= \"modal-title\" id = \"detailsModalLabel\" > ${ produto.sCodigo} - ${ produto.sDsc}</h5>\r\n");
            sb.Append("                 `;\r\n");
            sb.Append("                 var modalInfo = document.getElementById('modalInfo');\r\n");
            sb.Append("                 modalInfo.innerHTML = tituloProduto;\r\n");
            sb.Append("                 var imagem = '';\r\n");
            sb.Append("                 if (produto.imagem) {\r\n");
            sb.Append("                     imagem += `\r\n");
            sb.Append("                         <div style = \"text-align: center; margin-bottom: 20px;\">\r\n");
            sb.Append("                             <img src = \"${produto.imagem}\" alt = \"Imagem do Produto\" class= \"img-fluid\" style = \"width: 300px; height: auto;\" />\r\n");
            sb.Append("                         </div>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 var tabelaProduto = '<table class=\"table table-bordered\">';\r\n");
            sb.Append("                 if (produto.sCategoriaVendas) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Categoria Vendas </th>\r\n");
            sb.Append("                             <td>${ produto.sCategoriaVendas}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sTipo) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Tipo </th>\r\n");
            sb.Append("                             <td>${ produto.sTipo}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sGrupo) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Grupo </th>\r\n");
            sb.Append("                             <td>${ produto.sGrupo}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sFabricante) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Fabricante </th>\r\n");
            sb.Append("                             <td>${ produto.sFabricante}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sLocalArmazenamento) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Local Armazenamento </th>\r\n");
            sb.Append("                             <td>${ produto.sLocalArmazenamento}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sFamilia) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Família </th>\r\n");
            sb.Append("                             <td>${ produto.sFamilia}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sCodigoCEST) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> CEST </th>\r\n");
            sb.Append("                             <td>${ produto.sCodigoCEST}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sCodigoNCM) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> NCM </th>\r\n");
            sb.Append("                             <td>${ produto.sCodigoNCM}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sPaisOrigem) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Origem </th>\r\n");
            sb.Append("                             <td>${ produto.sPaisOrigem}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 tabelaProduto += `</table >`;\r\n");
            sb.Append("                 var modalBody = document.getElementById('modalBody');\r\n");
            sb.Append("                 modalBody.innerHTML = imagem + tabelaProduto;\r\n");
            sb.Append("                 $('#produtoDetalheModal').modal('show');\r\n");
            sb.Append("             },\r\n");
            sb.Append("             error: function(error) {\r\n");
            sb.Append("                 console.error(\"Erro ao obter os detalhes do produto:\", error);\r\n");
            sb.Append("             }\r\n");
            sb.Append("         });\r\n");
            sb.Append("     }\r\n");
            sb.Append("     function openProductDetail(idItem) {\r\n");
            sb.Append("         var url = '/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id=' + idItem;\r\n");
            sb.Append("         window.open(url, '_blank');\r\n");
            sb.Append("         return false;\r\n");
            sb.Append("     }\r\n");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_RegistraScript", sb.ToString(), true);
        }

        protected void RegistraScript_Personalizado(string clientID, bool bCodigo, bool bEPI)
        {
            string list = bEPI ? JsonConvert.SerializeObject(hddEPIs.Value.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries))
                                : JsonConvert.SerializeObject(hddColaboradores.Value.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries));

            StringBuilder sb = new StringBuilder();

            sb.Append("$v192(function() {\r\n");
            sb.Append("$v192(\"[id*=" + clientID + "]\").autocomplete({\r\n");
            sb.Append("source: function(request, response) {\r\n");
            sb.Append("$v192.ajax({\r\n");
            sb.Append("url: '/app/Paginas/RRHH/EntregaEPI_Detalhe.aspx/Get',\r\n");
            sb.Append("data: JSON.stringify({\r\n");

            sb.Append("'list': " + list + ", \r\n");
            sb.Append("'sDsc': JSON.stringify(request.term), \r\n");
            sb.Append("'sCodigo': '" + bCodigo.ToString() + "'\r\n");

            sb.Append("}),\r\n"); // Adicione uma vírgula após a chave 'data'

            sb.Append("dataType: \"json\",\r\n");
            sb.Append("type: \"POST\",\r\n");
            sb.Append("contentType: \"application/json; charset=utf-8\",\r\n");
            sb.Append("success: function(data) {\r\n");
            sb.Append("response($v192.map(data.d, function(item) {\r\n");
            sb.Append("return {\r\n");

            sb.Append("label: item.split('|')[" + (bCodigo ? 1 : 2) + "],\r\n");

            sb.Append(hddIncluir_idEPI.ClientID + ": item.split('|')[0],\r\n");
            sb.Append(txtCodigo_EPI.ClientID + ": item.split('|')[1],\r\n");
            sb.Append(txtDesc_EPI.ClientID + ": item.split('|')[2]\r\n");

            sb.Append("};\r\n");
            sb.Append("}));\r\n"); // Adicione parênteses de fechamento para a função 'map'
            sb.Append("},\r\n");
            sb.Append("error: function(response) {\r\n");
            sb.Append("console.log(response.responseText);\r\n");
            sb.Append("},\r\n");
            sb.Append("failure: function(response) {\r\n");
            sb.Append("console.log(response.responseText);\r\n");
            sb.Append("}\r\n");
            sb.Append("});\r\n");
            sb.Append("},\r\n");
            sb.Append("select: function(e, i) {\r\n");

            sb.Append("$(\"[id$=" + (bEPI ? hddIncluir_idEPI.ClientID : hddidSelecionarColaborador.ClientID) + "]\").val(i.item." + hddIncluir_idEPI.ClientID + ");\r\n");
            sb.Append("$(\"[id$=" + (bEPI ? txtCodigo_EPI.ClientID : txtCPF_Colaborador.ClientID) + "]\").val(i.item." + txtCodigo_EPI.ClientID + ");\r\n");
            sb.Append("$(\"[id$=" + (bEPI ? txtDesc_EPI.ClientID : txtNome_Colaborador.ClientID) + "]\").val(i.item." + txtDesc_EPI.ClientID + ");\r\n");

            sb.Append("$('[id$=" + (bEPI ? txtIncluirEPI_CA.ClientID : cmdSelecionarColaborador.ClientID) + "]').focus();");

            sb.Append("},\r\n");
            sb.Append("minLength: 3\r\n");
            sb.Append("});\r\n");
            sb.Append("});\r\n\r\n");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ScriptPagina_Personalizado_" + Guid.NewGuid(), sb.ToString(), true);
        }

        #endregion

        #region | WebMethod

        [WebMethod]
        public static string[] Get(List<string> list, string sDsc, string sCodigo)
        {
            sDsc = sDsc.Replace("\"", "").Trim();
            return list.Where(s => Convert.ToBoolean(sCodigo) ? s.Split('|')[1].ToLower().Contains(sDsc.ToLower()) : s.Split('|')[2].ToLower().Contains(sDsc.ToLower())).ToArray();
        }

        #endregion
    }
}