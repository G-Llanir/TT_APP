using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.FrameWork;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Flow.App.Paginas.RRHH
{
    public partial class Funcoes_Detalhe : Page
    {
        #region | Contrutores

        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_FuncaoCarteira";

        public List<cls_EPI_x_Funcao> bs_EPI_x_Funcao
        {

            get
            {
                if (ViewState["bs_EPI_x_Funcao"] == null)
                {
                    ViewState["bs_EPI_x_Funcao"] = new List<cls_EPI_x_Funcao>();
                }
                return (List<cls_EPI_x_Funcao>)ViewState["bs_EPI_x_Funcao"];
            }
            set
            {
                ViewState["bs_EPI_x_Funcao"] = value;
            }

        }

        #endregion

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.RRHH.FuncoesCarteira.Consultar, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.RRHH.FuncoesCarteira.Incluir, true);
                    Pesquisar("0");
                }

                if (Session["SalvoComSucesso"] != null && Convert.ToBoolean(Session["SalvoComSucesso"]))
                {
                    MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso!");
                    Session["SalvoComSucesso"] = false;
                }

                hddsPermissaoCadeado.Value = FUNCOES.ValidaPermissao(Permissao.RRHH.FuncoesCarteira.Cadeado, false) ? "1" : "0";
            }

            RegistraScript();
        }

        protected void Pesquisar(string idPesquisa)
        {
            PopulaCombos();
            aba_Historico.Visible = false;

            try
            {
                LimpaCampos();

                if (idPesquisa != "0")
                {
                    cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.FuncoesCarteira.Alterar);

                    Dictionary<string, string> vParametros = new Dictionary<string, string>()
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idFuncao", idPesquisa }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        hddidFuncao.Value = RETORNO.DATASET(dsPesquisa, "idFuncao");
                        hddsCadeado.Value = RETORNO.DATASET(dsPesquisa, "sCadeado");
                        txtidFuncao.Text = RETORNO.DATASET(dsPesquisa, "idFuncao");
                        ddlidEmpresa.SelectedValue = RETORNO.DATASET(dsPesquisa, "idEmpresa");
                        ddlidDepartamento.SelectedValue = RETORNO.DATASET(dsPesquisa, "idDepartamento");
                        ddlidTipoContrato.SelectedValue = RETORNO.DATASET(dsPesquisa, "idTipoContrato");
                        txtsDscFuncao.Text = RETORNO.DATASET(dsPesquisa, "sDscFuncao");
                        txtnRemuneracaoBase.Text = RETORNO.DATASET(dsPesquisa, 0, "nRemuneracaoBase", true);
                        ddlsTipoRemuneracao.SelectedValue = RETORNO.DATASET(dsPesquisa, "sTipoRemuneracao");
                        txtnPercComissaoVendas.Text = RETORNO.DATASET(dsPesquisa, 0, "nPercComissaoVendas", true);
                        sCBO.Text = RETORNO.DATASET(dsPesquisa, "sCBO");

                        txtsTarefas.Text = RETORNO.DATASET(dsPesquisa, "sTarefas");
                        txtsCompetencias.Text = RETORNO.DATASET(dsPesquisa, "sCompetencias");
                        txtsFormacaoAcademica.Text = RETORNO.DATASET(dsPesquisa, "sFormacaoAcademica");
                        txtsExperienciaProfissional.Text = RETORNO.DATASET(dsPesquisa, "sExperienciaProfissional");
                        ddlidTipoAvaliacao_Funcao.SelectedValue = RETORNO.DATASET(dsPesquisa, "idTipoAvaliacao_Funcao");

                        // ** Alteração: Definindo o estado dos novos checkboxes **
                        // Supondo que o banco de dados retorne 'S' para ativo e 'N' (ou qualquer outra coisa) para inativo
                        chkDuplaFuncao.Checked = RETORNO.DATASET(dsPesquisa, "sDuplaFuncaoAtiva") == "S";
                        txtnValorDuplaFuncao.Text = RETORNO.DATASET(dsPesquisa, 0, "nValorDuplaFuncao", true);
                        chkPericulosidade.Checked = RETORNO.DATASET(dsPesquisa, "sPericulosidadeAtiva") == "S";
                        txtnValorPericulosidade.Text = RETORNO.DATASET(dsPesquisa, 0, "nValorPericulosidade", true);

                        chkNoturnidade.Checked = RETORNO.DATASET(dsPesquisa, "sNoturnidadeAtiva") == "S";
                        txtnValorNoturnidade.Text = RETORNO.DATASET(dsPesquisa, 0, "nValorNoturnidade", true);

                        ddlTipoFuncao.SelectedValue = RETORNO.DATASET(dsPesquisa, "idTipoFuncao");

                        //21/08/2025 - Antonio Lemos
                        PopularCombo_GHE(ddlidEmpresa.SelectedValue);
                        ddlidGHE.SelectedValue = RETORNO.DATASET(dsPesquisa, "idGHE");

                        if (ddlidGHE.SelectedValue != "0")
                        {
                            PopularCombo_GHE_Setor(ddlidGHE.SelectedValue);
                            ddlidSetor.SelectedValue = RETORNO.DATASET(dsPesquisa, "idSetor");
                        }

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, "sDscUsuarioAtualizacao"));
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, "sAtivo"));
                        lblTituloPagina.Text = txtsDscFuncao.Text;



                        Popular_Aba_Historico(dsPesquisa);
                        Popular_dtgEPI(dsPesquisa);

                        if (FUNCOES.ValidaPermissao(Permissao.RRHH.FuncoesCarteira.ConsultarDadosNívelII))
                        {
                            DIV_Remuneracao.Visible = true;
                            DIV_TipoRemuneracao.Visible = true;
                            DIV_Comissão.Visible = true;
                            DIV_Avaliacao.Visible = true;
                        }
                        else
                        {
                            DIV_Remuneracao.Visible = false;
                            DIV_TipoRemuneracao.Visible = false;
                            DIV_Comissão.Visible = false;
                            DIV_Avaliacao.Visible = false;
                        }
                    }
                    else
                        throw new Exception(sErro);
                }
                else
                {
                    BreadCrumb.TitulodaPagina = "Incluir";
                    lblTituloPagina.Text = "Nova Função";
                    txtidFuncao.Text = "Novo";
                    cmdSalvar.Text = "Incluir";
                    div_bloquearEdicao.Visible = false;
                    aba_EPI.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        #endregion

        #region | Combos

        protected void PopulaCombos()
        {
            FUNCOES.Popula_Combo(ddlidDepartamento, "sp_Select 'Flow_Colaboradores_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Selecione o Departamento ", "0");
            FUNCOES.Popula_Combo(ddlidTipoContrato, "sp_Select 'tbl_Flow_Colaboradores_TipoContrato'", "idTipoContrato", "sDscTipoContrato", false, "Selecione o Tipo de Contratos", "0");
            FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa', @idUsuario=" + IDENTITY.Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");
            FUNCOES.Popula_Combo(ddlidTipoAvaliacao_Funcao, "sp_Select 'tbl_Flow_Colaboradores_FuncaoCarteira_TipoAvaliacao'", "idTipoAvaliacao_Funcao", "sDscTipoAvaliacao", false, "Selecione o Tipo de Avaliação", "0");
            FUNCOES.Popula_Combo(ddlFuncao_ImportarEPI, "sp_Select 'RRHH_Funcoes'", "idFuncao", "sDscFuncao", false, "Selecione a Função", "0");
            try { ddlFuncao_ImportarEPI.Items.Remove(ddlFuncao_ImportarEPI.Items.FindByValue(hddidFuncao.Value)); } catch { }
        }

        protected void PopularCombo_GHE(string idEmpresa)
        {
            if (idEmpresa != "0")
            {
                FUNCOES.Popula_Combo(ddlidGHE, "sp_Manipula_tbl_Flow_Colaboradores_GHE 'SELECT_GHE', @sidEmpresa=" + idEmpresa, "idGHE", "sDscGHE", false, "Selecione o GHE", "0");
            }
            else
            {
                ddlidGHE.Items.Clear();
            }
            RegistraScript();
        }
        protected void PopularCombo_GHE_Setor(string idGHE)
        {
            if (idGHE != "0")
            {
                FUNCOES.Popula_Combo(ddlidSetor, "sp_Manipula_tbl_Flow_Colaboradores_GHE 'SELECT_SETOR', @idGHE=" + idGHE, "idSetor", "sDscSetor", false, "Selecione o Setor", "0");
            }
            else
            {
                ddlidSetor.Items.Clear();
            }
            RegistraScript();
        }

        #endregion

        #region | Utils

        void LimpaCampos()
        {
            hddidFuncao.Value = "0";
            txtidFuncao.Text = "";
            ddlidEmpresa.SelectedValue = "0";
            ddlidDepartamento.SelectedValue = "0";
            txtsDscFuncao.Text = "";
            txtnRemuneracaoBase.Text = "";
            ddlidTipoContrato.SelectedValue = "0";
            ddlsTipoRemuneracao.SelectedValue = "";
            txtnPercComissaoVendas.Text = "";
            txtsTarefas.Text = "";
            txtsCompetencias.Text = "";
            txtsFormacaoAcademica.Text = "";
            txtsExperienciaProfissional.Text = "";
            chkDuplaFuncao.Checked = false;
            txtnValorDuplaFuncao.Text = "";
            chkPericulosidade.Checked = false;
            txtnValorPericulosidade.Text = "";

            chkNoturnidade.Checked = false;
            txtnValorNoturnidade.Text = "";

            ddlTipoFuncao.SelectedValue = "0";

            ddlidTipoAvaliacao_Funcao.SelectedValue = "0";
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = "Nova Função";
            ddlidGHE.SelectedValue = "0";
            ddlidSetor.SelectedValue = "0";
        }

        private bool ValidarDados()
        {
            string sMensagemErro = "";

            if (ddlidEmpresa.SelectedValue == "0")
                sMensagemErro = "Selecione a Empresa!";

            if (ddlidTipoContrato.SelectedValue == "0")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Tipo de Contrato";

            if (ddlidDepartamento.SelectedValue == "0")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Departamento";

            if (Validacoes.ValidarTexto(txtsDscFuncao))
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe uma Função valída";

            if (Validacoes.ValidarTexto(txtnRemuneracaoBase))
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o Valor da Remuneração";

            if (ddlidDepartamento.SelectedValue == "")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Departamento";

            if (ddlidTipoAvaliacao_Funcao.SelectedValue == "0")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Tipo de Cargo (Avaliação)";

            if (ddlTipoFuncao.SelectedValue == "0")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Tipo de Função";

            if (ddlidGHE.SelectedValue != "0")
            {
                if (ddlidSetor.SelectedValue == "0")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Setor";
            }

            if (!string.IsNullOrEmpty(sMensagemErro))
            {
                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
                return false;
            }

            return true;
        }

        #endregion

        #region | Salvar

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            if (ValidarDados())
            {
                try
                {
                    string[] vidFuncao = hddidFuncao.Value.Split(',');
                    string idFuncao = vidFuncao[0].ToString();

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idFuncao", idFuncao },
                        { "@idEmpresa", ddlidEmpresa.SelectedValue },
                        { "@idDepartamento", ddlidDepartamento.SelectedValue },
                        { "@idTipoContrato", ddlidTipoContrato.SelectedValue },
                        { "@sDscFuncao", txtsDscFuncao.Text },
                        { "@sCBO", sCBO.Text },
                        { "@nRemuneracaoBase", BD.Conversoes.Numerico(txtnRemuneracaoBase) },
                        { "@sTipoRemuneracao", ddlsTipoRemuneracao.SelectedValue },
                        { "@nPercComissaoVendas", BD.Conversoes.Numerico(txtnPercComissaoVendas) },
                        
                        // ** Alteração: Lendo o estado dos novos checkboxes **
                        { "@sDuplaFuncaoAtiva", chkDuplaFuncao.Checked ? "S" : "N" },
                        { "@nValorDuplaFuncao",  chkDuplaFuncao.Checked ? BD.Conversoes.Numerico(txtnValorDuplaFuncao) : "0" },
                        { "@sPericulosidadeAtiva", chkPericulosidade.Checked ? "S" : "N" },
                        { "@nValorPericulosidade", chkPericulosidade.Checked ? BD.Conversoes.Numerico(txtnValorPericulosidade) : "0" },
                        { "@sNoturnidadeAtiva", chkNoturnidade.Checked ? "S" : "N" },
                        { "@nValorNoturnidade", chkNoturnidade.Checked ? BD.Conversoes.Numerico(txtnValorNoturnidade) : "0" },

                        { "@idTipoFuncao", ddlTipoFuncao.SelectedValue },
                        { "@sTarefas", txtsTarefas.Text },
                        { "@sCompetencias", txtsCompetencias.Text },
                        { "@sFormacaoAcademica", txtsFormacaoAcademica.Text },
                        { "@sExperienciaProfissional", txtsExperienciaProfissional.Text },
                        { "@idTipoAvaliacao_Funcao", ddlidTipoAvaliacao_Funcao.SelectedValue },
                        { "@sAtivo", ComboAtivo.Situacao_Recuperar() },
                        { "idGHE", ddlidGHE.SelectedValue },
                        { "idSetor", ddlidSetor.SelectedValue },
                        { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                    };
                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out string sErro))
                    {
                        idFuncao = RETORNO.DATASET(dsSalvar, 0, "idFuncao");

                        if (Salvar_EPI(idFuncao))
                        {
                            Session["SalvoComSucesso"] = true;
                            Response.Redirect("/app/Paginas/RRHH/Funcoes_Detalhe.aspx?id=" + idFuncao);
                        }
                        else
                        {
                            Pesquisar(idFuncao);
                            throw new Exception("Erro ao Salvar EPI:");
                        }

                        Pesquisar(idFuncao);
                        MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                    }
                    else
                        throw new Exception("BD: " + sErro.ToString());
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }
            }
        }

        #endregion

        #region | Aba Histórico

        void Popular_Aba_Historico(DataSet ds)
        {
            gv_Historico.DataSource = ds.Tables[4];
            gv_Historico.DataBind();
            aba_Historico.Visible = true;
        }

        protected void gv_Historico_RowDataBound(object sender, GridViewRowEventArgs e) => e.Row.Cells[3].Text = HttpUtility.HtmlDecode(e.Row.Cells[3].Text);

        #endregion

        #region | Aba EPI

        protected string MostrarLink(object idProduto, object sCodigoProduto) => $"<a href='/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={idProduto.ToString()}' target='_blank'>{sCodigoProduto}</a>";

        void Popular_dtgEPI(DataSet dsPesquisa)
        {
            bs_EPI_x_Funcao.Clear();

            foreach (DataRow row in dsPesquisa.Tables[2].Rows)
            {
                cls_EPI_x_Funcao objItem = new cls_EPI_x_Funcao();

                objItem.idFuncao = Convert.ToInt32(row["idFuncao"].ToString());
                objItem.idRegistroEPI = Convert.ToInt32(row["idRegistroEPI"].ToString());
                objItem.idLinha = bs_EPI_x_Funcao.Count() + 1;
                objItem.sFuncao = "CONSULTA_EPI";

                objItem.idItem = Convert.ToInt32(row["idProduto"].ToString());
                objItem.idTipoProduto = Convert.ToInt32(row["idTipoProduto"].ToString());

                objItem.nOrdem = Convert.ToInt32(row["nOrdem"].ToString());
                objItem.sTipoPeriodo = row["sTipoPeriodo"].ToString();
                objItem.sCodigoEPI = row["sCodigoEPI"].ToString();
                objItem.sDscEPI = row["sDscEPI"].ToString();
                objItem.nQuantidateEPI = Convert.ToInt32(row["nQuantidateEPI"].ToString());
                objItem.sDscTipoPeriodo = row["sDscTipoPeriodo"].ToString();
                objItem.nQuantidadeTempo = Convert.ToInt32(row["nQuantidadeTempo"].ToString());

                bs_EPI_x_Funcao.Add(objItem);
            }

            foreach (DataRow row in dsPesquisa.Tables[3].Rows)
            {
                hddEPIs.Value += string.Format("[{0}|{1}|{2}]", row["idItem"].ToString(), row["sCodigo"].ToString(), row["sDscProduto"].ToString());
            }

            dtgEPI_DataBind();
            LimpaCampos_EPI();
        }

        void dtgEPI_DataBind()
        {
            try
            {
                dtgEPI.DataSource = bs_EPI_x_Funcao.Where(e => e.sFuncao.ToString() != "EXCLUIR_EPI").OrderBy(e => (e.nOrdem, e.sDscEPI, e.nCA));
                dtgEPI.DataBind();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao Carregar os EPIs: " + ex.Message);
            }
        }

        void LimpaCampos_EPI()
        {
            hddIncluir_idEPI.Value = "0";
            txtCodigo_EPI.Text = string.Empty;
            txtDesc_EPI.Text = string.Empty;
            txtIncluirEPI_Qtd.Text = "1";
            txtIncluirEPI_nTempo.Text = "1";
            ddlIncluirEPI_sTipoPeriodo.SelectedValue = "H";

            cmdEPI_Incluir.Visible = true;
        }

        protected void cmdEPI_Incluir_Click(object sender, EventArgs e)
        {
            AtualizaClasses_EPI();

            if (ValidarDados_EPI())
            {
                string[] vidFuncao = hddidFuncao.Value.Split(',');
                string idFuncao = vidFuncao[0].ToString().Trim();

                var item = bs_EPI_x_Funcao.Where(ep => ep.idItem.Equals(int.Parse(hddIncluir_idEPI.Value))).FirstOrDefault();

                if (item == null)
                {
                    int idLinha = bs_EPI_x_Funcao.Count() + 1;

                    cls_EPI_x_Funcao epi = new cls_EPI_x_Funcao
                    {
                        idLinha = idLinha,
                        nOrdem = idLinha * 10,
                        sFuncao = "INSERIR_EPI",
                        idFuncao = Convert.ToInt32(idFuncao),
                        idItem = Convert.ToInt32(hddIncluir_idEPI.Value),
                        sCodigoEPI = txtCodigo_EPI.Text,
                        sDscEPI = txtDesc_EPI.Text,
                        nQuantidateEPI = !string.IsNullOrEmpty(txtIncluirEPI_Qtd.Text) && !string.IsNullOrWhiteSpace(txtIncluirEPI_Qtd.Text) ? int.Parse(txtIncluirEPI_Qtd.Text) : 1,
                        nQuantidadeTempo = !string.IsNullOrEmpty(txtIncluirEPI_nTempo.Text) && !string.IsNullOrWhiteSpace(txtIncluirEPI_nTempo.Text) ? int.Parse(txtIncluirEPI_nTempo.Text) : 1,
                        sTipoPeriodo = ddlIncluirEPI_sTipoPeriodo.SelectedValue,
                        sDscTipoPeriodo = ddlIncluirEPI_sTipoPeriodo.SelectedItem.Text
                    };
                    bs_EPI_x_Funcao.Add(epi);
                }
                else if (item.sFuncao == "EXCLUIR_EPI")
                    item.sFuncao = "INSERIR_EPI";
                else
                    MensagemPagina_EPI.MostraMensagem_Erro("Não é possível adicionar um EPI já incluído!", false);

                dtgEPI_DataBind();
                LimpaCampos_EPI();

                MensagemPagina_EPI.MostraMensagem_Sucesso("EPI incluído com sucesso!", false);
            }

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_MantemAbaAtiva_EPI", "$('#epi-tab').tab('show');", true);
        }

        protected void dtgEPI_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                    (e.Row.FindControl("ddlsTipoPeriodo") as DropDownList).SelectedValue = bs_EPI_x_Funcao.Where(epi => epi.idLinha.Equals(int.Parse(e.Row.Cells[0].Text))).FirstOrDefault().sTipoPeriodo;
            }
            catch { }
        }

        protected void dtgEPI_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            AtualizaClasses_EPI();

            int idLinha = Convert.ToInt32(dtgEPI.Rows[e.RowIndex].Cells[0].Text);

            bs_EPI_x_Funcao[bs_EPI_x_Funcao.FindIndex(ep => ep.idLinha.Equals(idLinha))].sFuncao = "EXCLUIR_EPI";
            dtgEPI_DataBind();

            div_EPI.Visible = true;

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_MantemAbaAtiva_EPI", "$('#epi-tab').tab('show');", true);
        }

        void AtualizaClasses_EPI()
        {
            foreach (GridViewRow row in dtgEPI.Rows)
            {
                try
                {
                    var EPI = bs_EPI_x_Funcao[int.Parse(row.Cells[0].Text) - 1];

                    int ordem = Convert.ToInt32((row.FindControl("txtnOrdem") as TextBox).Text);
                    int qtdEPI = Convert.ToInt32((row.FindControl("txtnQuantidateEPI") as TextBox).Text);
                    int qtdTempo = Convert.ToInt32((row.FindControl("txtnQuantidadeTempo") as TextBox).Text);
                    var ddlPeriodo = row.FindControl("ddlsTipoPeriodo") as DropDownList;
                    string sPeriodo = ddlPeriodo.SelectedValue;

                    if (EPI.nOrdem != ordem)
                        EPI.sFuncao = "INSERIR_EPI";
                    else if (EPI.nQuantidateEPI != qtdEPI)
                        EPI.sFuncao = "INSERIR_EPI";
                    else if (EPI.nQuantidadeTempo != qtdTempo)
                        EPI.sFuncao = "INSERIR_EPI";
                    else if (EPI.sTipoPeriodo != sPeriodo)
                        EPI.sFuncao = "INSERIR_EPI";

                    EPI.nOrdem = ordem;
                    EPI.nQuantidateEPI = qtdEPI;
                    EPI.nQuantidadeTempo = qtdTempo;
                    EPI.sTipoPeriodo = sPeriodo;

                    try
                    {
                        EPI.sDscTipoPeriodo = qtdTempo > 1 ?
                                                    ddlPeriodo.SelectedValue == "H" ? "Horas"
                                                    : ddlPeriodo.SelectedValue == "D" ? "Dias"
                                                    : ddlPeriodo.SelectedValue == "S" ? "Semanas"
                                                    : ddlPeriodo.SelectedValue == "M" ? "Meses"
                                                    : "Anos"
                                                :
                                                    ddlPeriodo.SelectedValue == "H" ? "Hora"
                                                    : ddlPeriodo.SelectedValue == "D" ? "Dia"
                                                    : ddlPeriodo.SelectedValue == "S" ? "Semana"
                                                    : ddlPeriodo.SelectedValue == "M" ? "Mês"
                                                    : "Ano";
                    }
                    catch
                    {

                    }
                }
                catch { }
            }
        }

        private bool ValidarDados_EPI()
        {
            if (hddIncluir_idEPI.Value == "0")
            {
                MensagemPagina_EPI.MostraMensagem_Erro("É necessário selecionar um EPI já cadastrado!");
                return false;
            }
            if (txtCodigo_EPI.Text.Length < 3)
            {
                MensagemPagina_EPI.MostraMensagem_Erro("É necessário preencher o campo de Código do EPI com ao menos 3 caracteres!");
                return false;
            }
            if (txtDesc_EPI.Text.Length < 5)
            {
                MensagemPagina_EPI.MostraMensagem_Erro("É necessário preencher o campo de Descrição do EPI com ao menos 5 caracteres!");
                return false;
            }

            return true;
        }

        bool Salvar_EPI(string idFuncao)
        {
            try
            {
                AtualizaClasses_EPI();

                foreach (var EPI_Linha in bs_EPI_x_Funcao)
                {
                    if (EPI_Linha.sFuncao == "CONSULTA_EPI")
                        continue;

                    Dictionary<string, string> vParametroItensEPI = new Dictionary<string, string>()
                    {
                        { "@idRegistro", EPI_Linha.idRegistroEPI.ToString() },
                        { "@sFuncao", EPI_Linha.sFuncao },
                        { "@idFuncao", idFuncao },
                        { "@idProduto", EPI_Linha.idItem.ToString() },
                        { "@nOrdem", EPI_Linha.nOrdem.ToString() },
                        { "@nQuantidateEPI", EPI_Linha.nQuantidateEPI.ToString() },
                        { "@nQuantidadeTempo", EPI_Linha.nQuantidadeTempo.ToString() },
                        { "@sTipoPeriodo", EPI_Linha.sTipoPeriodo.ToString() },
                        { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                    };
                    BD.ExecutarDataSet(sProcedure, vParametroItensEPI);
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao Salvar os EPIs: " + ex.Message);
                return false;
            }

            return true;
        }

        #endregion

        #region | Importar / Exportar EPIs

        protected void cmdFuncao_ImportarEPI_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlFuncao_ImportarEPI.SelectedValue == "0")
                {
                    MensagemPagina_Modal_ImportarEPI.MostraMensagem_Erro("É necessário selecionar uma Função para Importar EPIs!");
                    FUNCOES.Scripts.AbrirModal(Page, "modalImportar_EPI");
                }
                else
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_EPIS" },
                        { "@idFuncao", ddlFuncao_ImportarEPI.SelectedValue }
                    };
                    DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            int idLinha = bs_EPI_x_Funcao.Count + 1;
                            var item = bs_EPI_x_Funcao.Where(ep => ep.idItem.Equals(row.Field<int>("idItem"))).FirstOrDefault();

                            if (item == null)
                            {
                                cls_EPI_x_Funcao epi = new cls_EPI_x_Funcao
                                {
                                    idLinha = idLinha,
                                    nOrdem = row.Field<int>("nOrdem"),
                                    sFuncao = "INSERIR_EPI",
                                    idFuncao = int.Parse(hddidFuncao.Value),
                                    idItem = row.Field<int>("idItem"),
                                    sCodigoEPI = row.Field<string>("sCodigo"),
                                    sDscEPI = row.Field<string>("sDscProduto"),
                                    nQuantidateEPI = row.Field<int>("nQuantidateEPI"),
                                    nQuantidadeTempo = row.Field<int>("nQuantidadeTempo"),
                                    sTipoPeriodo = row.Field<string>("sTipoPeriodo")
                                };
                                bs_EPI_x_Funcao.Add(epi);
                            }
                            else if (item.sFuncao == "EXCLUIR_EPI")
                            {
                                item.sFuncao = "INSERIR_EPI";
                                item.idLinha = idLinha;
                                item.nOrdem = row.Field<int>("nOrdem");
                                item.nQuantidateEPI = row.Field<int>("nQuantidateEPI");
                                item.nQuantidadeTempo = row.Field<int>("nQuantidadeTempo");
                                item.sTipoPeriodo = row.Field<string>("sTipoPeriodo");
                            }
                        }

                        dtgEPI_DataBind();
                        LimpaCampos_EPI();

                        MensagemPagina_EPI.MostraMensagem_Sucesso("EPIs Importados com sucesso!", false);
                    }
                    else
                        MensagemPagina_EPI.MostraMensagem_Aviso("Não existem EPIs cadastrados para a Função selecionada!", false);

                    FUNCOES.Scripts.FecharModal(Page, "modalImportar_EPI");
                    FUNCOES.Scripts.RemoverBackdrop_Modal(Page);
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_EPI.MostraMensagem_Erro("Erro ao Importar EPIs: " + ex.Message, true);
            }

            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "epi-tab");
        }

        protected void cmdEPI_Importar_Click(object sender, EventArgs e)
        {
            AtualizaClasses_EPI();
            ddlFuncao_ImportarEPI.SelectedIndex = 0;
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "epi-tab");
            FUNCOES.Scripts.AbrirModal(Page, "modalImportar_EPI");
        }

        protected void cmdEPI_Exportar_Click(object sender, EventArgs e)
        {
            AtualizaClasses_EPI();

            if (bs_EPI_x_Funcao.Count > 0)
            {
                try
                {
                    ReportViewer rv = new ReportViewer();

                    rv.LocalReport.ReportPath = @"App\Reports\Excel_Geral.rdlc";

                    List<object> list = new List<object>();

                    bs_EPI_x_Funcao.ForEach(a =>
                    {
                        list.Add(new
                        {
                            a.nOrdem,
                            sCodigo = a.sCodigoEPI,
                            sDscItem = a.sDscEPI,
                            sDsc_1 = a.nQuantidateEPI.ToString(),
                            sDsc_2 = $"{a.nQuantidadeTempo} {a.sDscTipoPeriodo}"
                        });
                    });

                    rv.LocalReport.DataSources.Add(new ReportDataSource("ds_Consulta", list));

                    ReportParameter[] rp = new ReportParameter[2];

                    rp[0] = new ReportParameter("Aparece_Tabela", "EPI_");
                    rp[1] = new ReportParameter("Titulo_Personalizado", " ");

                    rv.LocalReport.SetParameters(rp);
                    rv.LocalReport.Refresh();

                    byte[] bytes = rv.LocalReport.Render("Excel", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings);
                    string sNomeArquivo = $"{FUNCOES.NormalizarTexto(txtsDscFuncao.Text)}_EPIs_{FUNCOES.CarimboDataHora()}.xlsx";

                    File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivo, bytes);

                    FUNCOES.DownloadArquivo(Page, sNomeArquivo);

                    MensagemPagina_EPI.MostraMensagem_Sucesso("EPIs Exportados com sucesso!", true);
                }
                catch (Exception ex)
                {
                    MensagemPagina_EPI.MostraMensagem_Erro("Houve um erro ao Gerar o Excel dos EPIs!<br />" + (ex.InnerException != null ? ex.InnerException.Message : "") + "<br />" + ex.Message, true);
                }
            }

            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "epi-tab");
        }

        #endregion

        #region | Script

        protected void RegistraScript()
        {
            FUNCOES.Scripts.Aplica_TooltipPersonalizado(Page, "tooltip");

            RegistraScript_IncluirEPI(txtCodigo_EPI.ClientID, true);
            RegistraScript_IncluirEPI(txtDesc_EPI.ClientID, false);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("$('[id*=txtnRemuneracaoBase]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.AppendLine("$('[id*=txtnPercComissaoVendas]').mask('0.000,00', { reverse: true });");
            sb.AppendLine("$('[id*=nValorBonus_Avaliacao]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.AppendLine("$('[id*=sCBO]').mask('0000-00', { reverse: true });");
            sb.AppendLine("$('.qtd').mask('000', { reverse: true });");

            sb.AppendLine("$('[id*=txtnValorDuplaFuncao]').mask('##0,00', { reverse: true });");
            sb.AppendLine("$('[id*=nValorPericulosidade]').mask('##0,00', { reverse: true });");
            sb.AppendLine("$('[id*=txtnValorNoturnidade]').mask('##0,00', { reverse: true });");
            

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_AplicaMascaras", sb.ToString(), true);

            sb.Clear();

            sb.AppendLine("window.addEventListener('load', function () {");
            sb.AppendLine("");
            sb.AppendLine($"     var permissao = document.getElementById('{hddsPermissaoCadeado.ClientID}').value; ");
            sb.AppendLine("     var lockCheckbox = document.getElementById('lock'); ");
            sb.AppendLine($"     var hddidFuncao = document.getElementById('{hddidFuncao.ClientID}').value; ");
            sb.AppendLine("");
            sb.AppendLine("     if (lockCheckbox) {");
            sb.AppendLine($"         var hddsCadeado = document.getElementById('{hddsCadeado.ClientID}').value; ");
            sb.AppendLine("         var isChecked = (hddsCadeado === 'N'); ");
            sb.AppendLine($"         var btnSalvar = document.getElementById('{cmdSalvar.ClientID}'); ");
            sb.AppendLine("");
            sb.AppendLine("         if (hddidFuncao === '0') {");
            sb.AppendLine("             isChecked = true; ");
            sb.AppendLine("         }");
            sb.AppendLine("");
            sb.AppendLine("         if (isChecked) {");
            sb.AppendLine("             lockCheckbox.checked = isChecked;");
            sb.AppendLine("             btnSalvar.style.display = 'inline';");
            sb.AppendLine("         } else {");
            sb.AppendLine("             btnSalvar.style.display = 'none';");
            sb.AppendLine("             lockCheckbox.checked = isChecked;");
            sb.AppendLine("             var shackle = document.querySelector('.shackle');");
            sb.AppendLine("             shackle.style.transform = 'rotateY(0deg)';");
            sb.AppendLine("         }");
            sb.AppendLine("     }");
            sb.AppendLine("");
            sb.AppendLine("});");

            sb.AppendLine("");

            sb.AppendLine("document.addEventListener('DOMContentLoaded', function () {");
            sb.AppendLine("    document.body.addEventListener('change', function (event) {");
            sb.AppendLine("        if (event.target && event.target.id === 'lock') {");
            sb.AppendLine("            var isChecked = event.target.checked;");
            sb.AppendLine("            var mensagem;");
            sb.AppendLine("            if (isChecked) {");
            sb.AppendLine("                mensagem = 'Deseja habilitar a edição?';");
            sb.AppendLine("            } else {");
            sb.AppendLine("                mensagem = 'Deseja desabilitar a edição?';");
            sb.AppendLine("            }");
            sb.AppendLine("");
            sb.AppendLine("            alterarEdicao(mensagem, event);");
            sb.AppendLine("        }");
            sb.AppendLine("    });");
            sb.AppendLine("});");

            sb.AppendLine("");

            sb.AppendLine("$v192(function () {");
            sb.AppendLine("    $v192('#dialog_Aceitar').dialog({");
            sb.AppendLine("        resizable: false,");
            sb.AppendLine("        height: 'auto',");
            sb.AppendLine("        width: 400,");
            sb.AppendLine("        modal: true,");
            sb.AppendLine("        autoOpen: false");
            sb.AppendLine("    });");
            sb.AppendLine("});");

            sb.AppendLine("");

            sb.AppendLine("function alterarEdicao(mensagem, event) {");
            sb.AppendLine("    document.getElementById('Label3').innerText = mensagem;");
            sb.AppendLine("");
            sb.AppendLine($"    var btnSalvar = document.getElementById('{cmdSalvar.ClientID}');");
            sb.AppendLine($"    var hddidFuncao = document.getElementById('{hddidFuncao.ClientID}');");
            sb.AppendLine("    var lockCheckbox = document.getElementById('lock');");
            sb.AppendLine("    var isChecked = event.target.checked;");
            sb.AppendLine("");
            sb.AppendLine("    $v192('#dialog_Aceitar').dialog('option', 'buttons', {");
            sb.AppendLine("        'Sim': function () {");
            sb.AppendLine("            var idFuncao = hddidFuncao.value;");
            sb.AppendLine("");
            sb.AppendLine("            $.ajax({");
            sb.AppendLine("                url: '/app/Paginas/RRHH/Funcoes_Detalhe.aspx/SalvarCadeado',");
            sb.AppendLine("                data: JSON.stringify({");
            sb.AppendLine("                    isLocked: isChecked,");
            sb.AppendLine("                    idFuncao: idFuncao");
            sb.AppendLine("                }),");
            sb.AppendLine("                contentType: 'application/json; charset=utf-8',");
            sb.AppendLine("                type: 'POST',");
            sb.AppendLine("                dataType: 'json',");
            sb.AppendLine("                success: function (data) {");
            sb.AppendLine("                    var shackle = document.querySelector('.shackle');");
            sb.AppendLine("                    if (isChecked) {");
            sb.AppendLine("                        btnSalvar.style.display = 'inline';");
            sb.AppendLine("                        shackle.style.transform = 'rotateY(150deg) translateX(3px)';");
            sb.AppendLine("                        shackle.style.transformOrigin = 'right';");
            sb.AppendLine("                    } else {");
            sb.AppendLine("                        btnSalvar.style.display = 'none';");
            sb.AppendLine("                        shackle.style.transform = 'rotateY(0deg)';");
            sb.AppendLine("                    }");
            sb.AppendLine("                    window.location.href = location.href;");
            sb.AppendLine("                },");
            sb.AppendLine("                error: function (response) {");
            sb.AppendLine("                    alert(response.responseText);");
            sb.AppendLine("                },");
            sb.AppendLine("                failure: function (response) {");
            sb.AppendLine("                    alert(response.responseText);");
            sb.AppendLine("                }");
            sb.AppendLine("            });");
            sb.AppendLine("");
            sb.AppendLine("            $v192(this).dialog('close');");
            sb.AppendLine("        },");
            sb.AppendLine("        'Não': function () {");
            sb.AppendLine("            lockCheckbox.checked = !isChecked;");
            sb.AppendLine("");
            sb.AppendLine("            var shackle = document.querySelector('.shackle');");
            sb.AppendLine("            if (!isChecked) {");
            sb.AppendLine("                btnSalvar.style.display = 'inline';");
            sb.AppendLine("                shackle.style.transform = 'rotateY(150deg) translateX(3px)';");
            sb.AppendLine("                shackle.style.transformOrigin = 'right';");
            sb.AppendLine("            } else {");
            sb.AppendLine("                btnSalvar.style.display = 'none';");
            sb.AppendLine("                shackle.style.transform = 'rotateY(0deg)';");
            sb.AppendLine("            }");
            sb.AppendLine("");
            sb.AppendLine("            $v192(this).dialog('close');");
            sb.AppendLine("        }");
            sb.AppendLine("    });");
            sb.AppendLine("");
            sb.AppendLine("    $v192('#dialog_Aceitar').dialog('open');");
            sb.AppendLine("}");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Cadeado", sb.ToString(), true);
        }

        protected void RegistraScript_IncluirEPI(string clientID, bool bCodigo)
        {
            string EPIs = JsonConvert.SerializeObject(hddEPIs.Value.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries).ToList());

            StringBuilder sb = new StringBuilder();

            sb.Append("$v192(function() {\r\n");
            sb.Append("$v192(\"[id*=" + clientID + "]\").autocomplete({\r\n");
            sb.Append("source: function(request, response) {\r\n");
            sb.Append("$v192.ajax({\r\n");
            sb.Append("url: '/app/Paginas/RRHH/Funcoes_Detalhe.aspx/GetEPI',\r\n");
            sb.Append("data: JSON.stringify({\r\n");

            sb.Append("'hddEPIs': " + EPIs + ", \r\n");
            sb.Append("'sDscEPI': JSON.stringify(request.term), \r\n");
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

            sb.Append("$(\"[id$=" + hddIncluir_idEPI.ClientID + "]\").val(i.item." + hddIncluir_idEPI.ClientID + ");\r\n");
            sb.Append("$(\"[id$=" + txtCodigo_EPI.ClientID + "]\").val(i.item." + txtCodigo_EPI.ClientID + ");\r\n");
            sb.Append("$(\"[id$=" + txtDesc_EPI.ClientID + "]\").val(i.item." + txtDesc_EPI.ClientID + ");\r\n");

            sb.Append("$('[id$=" + txtIncluirEPI_Qtd.ClientID + "]').focus();");

            sb.Append("},\r\n");
            sb.Append("minLength: 3\r\n");
            sb.Append("});\r\n");
            sb.Append("});\r\n\r\n");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ScriptPagina_IncluirEPI_" + Guid.NewGuid(), sb.ToString(), true);
        }

        #endregion

        #region | WebMethod

        [WebMethod]
        public static string[] GetEPI(List<string> hddEPIs, string sCodigo, string sDscEPI)
        {
            sDscEPI = sDscEPI.Replace("\"", "").Trim();
            return hddEPIs.Where(s => Convert.ToBoolean(sCodigo) ? s.Split('|')[1].ToLower().Contains(sDscEPI.ToLower()) : s.Split('|')[2].ToLower().Contains(sDscEPI.ToLower())).ToArray();
        }

        [WebMethod]
        public static void SalvarCadeado(bool isLocked, string idFuncao)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CADEADO" },
                { "@sCadeado", isLocked ? "N" : "S" },
                { "@idFuncao", idFuncao },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
            };
            BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Colaboradores_FuncaoCarteira", vParametros);
        }

        #endregion

        protected void ddlidEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopularCombo_GHE(ddlidEmpresa.SelectedValue);
        }

        protected void ddlidGHE_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopularCombo_GHE_Setor(ddlidGHE.SelectedValue);
        }
    }
}

