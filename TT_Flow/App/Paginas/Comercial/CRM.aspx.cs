using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Controles;
using static TT.FrameWork.BD;
using Funcoes = TT.FrameWork.Funcoes;
using Identity = TT.FrameWork.Identity;

namespace TT_Flow.App.Paginas.Comercial
{
    public partial class CRM : Page
    {
        string sProcedure = "sp_Manipula_tbl_Flow_Comercial_CRM";
        int nTipoStatus = 5;
        string confidencial = "";
        static int identificador = 1;

        protected void Page_Load(object sender, EventArgs e)
        {

            Funcoes.ValidaPermissao(Permissao.Comercial.CRM.Consultar, true);
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();

            //Kanban Script
            string script = File.ReadAllText(Server.MapPath("~/App/JS/kanbanCRM.js"));
            ScriptManager.RegisterStartupScript(this, this.GetType(), "kanbanCRM_js", script, true);

            //Kanban CSS
            string cssPath = "~/App/css/styleCRM.css?v=" + DateTime.Now.Ticks.ToString();
            HtmlLink cssLink = new HtmlLink();
            cssLink.Href = ResolveUrl(cssPath);
            cssLink.Attributes.Add("rel", "stylesheet");
            cssLink.Attributes.Add("type", "text/css");
            Page.Header.Controls.Add(cssLink);

            //Manual Usuario
            manual.sNomeArquivo = "ManualDoUsuário_CRM.pdf";

            if (!IsPostBack)
            {
                string id = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(id))
                {
                    PopulaCombos();
                    ModoEdicao(id);
                    div_Consulta.Visible = false;
                    hddStatusModal.Value = "true1";
                    hddidRegistroCRM.Value = id;
                    DetalheNegocio_MensagemPagina.MostraMensagem("Selecione um Status!", "Info", false);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalLink", "$('#modalCRM').modal('show');", true);
                }
                ddlsStatus_SelectedIndexChanged(objSender, objEventArgs);
                ddlsVendedor_SelectedIndexChanged(objSender, objEventArgs);
                ddlsTipo_SelectedIndexChanged(objSender, objEventArgs);
                ddlsCLiente_SelectedIndexChanged(objSender, objEventArgs);
                txtdtFinal_TextChanged(objSender, objEventArgs);
                txtdtInicial_TextChanged(objSender, objEventArgs);

                Funcoes.Popula_Combo(ddlsCLiente, "sp_Select 'tbl_Flow_Clientes'", "idCliente", "sRazaoSocial", false, "Todos os Clientes", "0");
                Funcoes.Popula_Combo(ddlsTipo, "sp_Select 'tbl_Flow_Comercial_Orcamento_Tipo'", "idTipoOrcamento", "sDscTipoOrcamento", false, "Todos os Orçamentos", "0");
                Funcoes.Popula_Combo(ddlsVendedor, "sp_Select 'FLOW_Vendedores'", "idVendedor", "sDscUsuario", false, "Todos os Vendedores", ddlsVendedor.SelectedValue);
                Funcoes.Popula_Combo(ddlsStatus, "sp_Select 'Status CRM', @idPesquisa=" + nTipoStatus + "", "idStatus", "sDscStatus", false, "Todos os Status", ddlsStatus.SelectedValue);


                if (!Funcoes.ValidaPermissao(Permissao.Comercial.CRM.Visualizar_Vendedores))
                {
                    ddlsVendedor.SelectedValue = ConsultaVendedor();
                    ddlsVendedor.Attributes.Add("disabled", "disabled");
                }

                string view = Request.QueryString["view"];
                if (!string.IsNullOrEmpty(view))
                {
                    if (view == "grid")
                    {
                        pnGrid.Visible = true;
                        pnKanban.Visible = false;
                        idGrid.Checked = true;
                        idKanban.Checked = false;
                    }
                    else if (view == "kanban")
                    {
                        pnGrid.Visible = false;
                        pnKanban.Visible = true;
                        idGrid.Checked = false;
                        idKanban.Checked = true;

                        CarregaBoard(hddFiltroStatus.Value);
                        var confidencial = TipoPesquisa();
                        CarregaCard(confidencial);
                    }
                }

            }
            else
            {
                RestaurarSelecoes();

                if (!Funcoes.ValidaPermissao(Permissao.Comercial.CRM.Visualizar_Vendedores))
                {
                    ddlsVendedor.SelectedValue = ConsultaVendedor();
                    ddlsVendedor.Attributes.Add("disabled", "disabled");
                }

            }

            if (Session["ItemId"] != null)
            {
                hddidRegistroCRM.Value = Session["ItemId"].ToString();
                hddStatusModal.Value = Session["StatusModal"].ToString();

                if (Session["OpenModal"] as string == "modalCRM")
                {
                    PopulaCombos();
                    ModoEdicao(hddidRegistroCRM.Value);
                    ScriptsPagina(txtsCliente.ClientID, "ScriptCompletar", new List<string> { txtsCliente.ClientID }, Page);
                    string scriptModal = "window.onload = function() { $('#modalCRM').modal('show'); };";
                    ScriptManager.RegisterStartupScript(this, GetType(), "openModalCRM", scriptModal, true);
                    Session.Remove("OpenModal");
                }
                Session.Remove("ItemId");
                Session.Remove("StatusModal");
            }

            if (hddStatusModal.Value == "false")
            {
                List<int> indexesToIgnore = new List<int> { };
                if (!Funcoes.ValidaPermissao(Permissao.Comercial.CRM.Confidencial))
                {
                    confidencial = "N";
                    Pesquisar(confidencial);
                }
                else
                {
                    confidencial = "";
                    Pesquisar(confidencial);
                }
                Grid.BotoesOcultarColuna(placeholderButtons, dtgvConsulta, this, indexesToIgnore);
            }


            pnGrid.Visible = idGrid.Checked;
            pnKanban.Visible = idKanban.Checked;

            if (idKanban.Checked)
            {
                CarregaBoard(hddFiltroStatus.Value);
                confidencial = TipoPesquisa();
                CarregaCard(confidencial);

            }



        }

        #region | Consulta
        protected void Pesquisar(string confidencial)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@dtInclusao", txtdtInicial.Text },
                { "@dtUltimoContato", txtdtFinal.Text },
                { "@idCliente", ddlsCLiente.SelectedValue },
                { "@idTipoCotacao", ddlsTipo.SelectedValue },
                { "@idVendedor", ddlsVendedor.SelectedValue },
                { "@idStatus", ddlsStatus.SelectedValue },
                { "@sConfidencial", confidencial }
            };

            DataTable tb = ExecutarDataTable(sProcedure, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                pnGrid.Visible = idGrid.Checked;
                pnKanban.Visible = idKanban.Checked;
            }

                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", Grid.DataBindComScriptData(dtgvConsulta, tb, new int[2] { 0, 10 }, "desc", "false", "''"), true);
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                var sStatusCancelamento = DataBinder.Eval(e.Row.DataItem, "sStatusCancelamento").ToString();
                if (sStatusCancelamento == "S")
                {
                    e.Row.CssClass = "danger";
                    e.Row.Cells[11].Text = string.Empty;
                }

                // Verifica o status de finalização
                var sStatusFinalizado = DataBinder.Eval(e.Row.DataItem, "sStatusFinalizado").ToString();
                if (sStatusFinalizado == "S")
                {
                    e.Row.CssClass = "success";
                    e.Row.Cells[11].Text = string.Empty;
                }

                var dataProximoContato = e.Row.Cells[11].Text;
                DateTime proximoContato;

                if (DateTime.TryParse(dataProximoContato, out proximoContato))
                {
                    var diferenca = proximoContato - DateTime.Now;
                    if (diferenca.TotalHours < 0)
                    {
                        e.Row.Cells[11].CssClass = "danger";
                    }
                    else if (diferenca.TotalHours < 4)
                    {
                        e.Row.Cells[11].CssClass = "warning";
                    }
                    else
                    {
                        e.Row.Cells[11].CssClass = "success";
                    }
                }


                var dtPrevisao = e.Row.Cells[8].Text;
                DateTime previsao;

                if (DateTime.TryParse(dtPrevisao, out previsao))
                {

                    if (dtPrevisao == "01/01/1900")
                    {
                        int columnIndex = 8;
                        e.Row.Cells[columnIndex].Text = "";
                    }

                }
            }
        }

        private void RestaurarSelecoes()
        {
            ddlsCLiente.SelectedValue = Request.Form[ddlsCLiente.UniqueID];
            ddlsTipo.SelectedValue = Request.Form[ddlsTipo.UniqueID];
            ddlsVendedor.SelectedValue = Request.Form[ddlsVendedor.UniqueID];
            ddlsStatus.SelectedValue = Request.Form[ddlsStatus.UniqueID];
        }

        private string ConsultaVendedor()
        {
            string sErro = "";
            DataSet dsPesquisa;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR_VENDEDOR");
            vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                string sIdVendedor = Retorno.DATASET(dsPesquisa, "idVendedor");
                return sIdVendedor;
            }
            return "";
        }

        private int ConsultaOrcamento()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA_ORCAMENTO" },
                { "@idRegistroCRM", hddidRegistroCRM.Value }
            };
            DataSet ds = ExecutarDataSet(sProcedure, vParametros);

            if (ValidarDataSet(ds, out string sErro))
            {
                hddidOrcamento.Value = Retorno.DATASET(ds, 0, "idPedido");
                string nNumeroPedido = Retorno.DATASET(ds, 0, "idOrcamento");

                Funcoes.Popula_Combo(ddlRevOrcamento, "sp_select 'Flow_Consultar_Orcamento', @idPesquisa = " + nNumeroPedido + "", "idPedido", "sReferenciaCompleta", false, "Selecione a Revisão", "0");
            }

            return ddlRevOrcamento.Items.Count;
        }

        private string TipoPesquisa()
        {
            if (!Funcoes.ValidaPermissao(Permissao.Comercial.CRM.Confidencial))
            {
                return "N";
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region | Popula Combos

        void PopulaCombos()
        {
            Funcoes.Popula_Combo(ddlModalsTipo, "sp_Select 'tbl_Flow_Comercial_Orcamento_Tipo'", "idTipoOrcamento", "sDscTipoOrcamento", false, "Selecione um Tipo", "0");
            Funcoes.Popula_Combo(ddlModalsVendedor, "sp_Select 'FLOW_Vendedores_Ativos'", "idVendedor", "sDscUsuario", false, "Selecione um Vendedor", "0");
            Funcoes.Popula_Combo(ddlsMeioContato, "sp_select 'Flow_Comercial_CRM_Followup_MeioContato'", "idMeioContato", "sDscMeioContato", false, "Selecione um Meio de Contato", "0");
            Funcoes.Popula_Combo(ddlsModalStatus, "sp_Select 'Status CRM', @idPesquisa=" + nTipoStatus + ", @idFiltro = 1", "idStatus", "sDscStatus", false, "Selecione um Status", "0");
            Funcoes.Popula_Combo(ddlsMeioContato, "sp_select 'Flow_Comercial_CRM_Followup_MeioContato'", "idMeioContato", "sDscMeioContato", false, "Selecione o Meio de Contato", "0");
        }

        #endregion

        #region | Botões
        protected void LinkButton_Command(object sender, CommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();
            PopulaCombos();
            ScriptsPagina(txtsCliente.ClientID, "ScriptCompletar", new List<string> { txtsCliente.ClientID }, Page);
            ModoEdicao(id);
            hddidRegistroCRM.Value = id;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalEdicao", "$('#modalCRM').modal('show');", true);
        }

        protected void cmdNovoNegocio_Click(object sender, EventArgs e)
        {

            lblModalTitulo.Text = "Novo Negócio";

            aba_FollowUp.Visible = false;
            div_followUp.Visible = false;
            aba_Arquivo.Visible = false;
            DIV_Arquivos.Visible = false;
            aba_Historico.Visible = false;
            DIV_historico.Visible = false;
            hddidRegistroCRM.Value = "0";
            ddlModalsTipo.Attributes.Remove("disabled");
            txtsCliente.ReadOnly = false;
            div_Acao_FollowUp.Visible = false;
            btnAcao.Visible = false;
            txtnValor.ReadOnly = true;
            div_btnNovoOrcamento.Visible = false;
            div_btnOrcamentoDetalhe.Visible = false;

            //-------------------------------
            //Agnes Partal - 01/07/2024
            div_btnExcluir.Visible = false;
            //-------------------------------

            PopulaCombos();

            if (!Funcoes.ValidaPermissao(Permissao.Comercial.CRM.Visualizar_Vendedores))
            {
                ddlModalsVendedor.SelectedValue = ConsultaVendedor();
                ddlModalsVendedor.Attributes.Add("disabled", "disabled");
            }
            if (!Funcoes.ValidaPermissao(Permissao.Comercial.CRM.Confidencial))
            {
                div_confidencial.Visible = false;
            }

            ScriptsPagina(txtsCliente.ClientID, "ScriptCompletar", new List<string> { txtsCliente.ClientID }, Page);

            LimparCampos();

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalNovo", "$('#modalCRM').modal('show');", true);
            RegistraScriptModalNegocio();
            txtdtInclusao.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
            hddStatusModal.Value = "true2";
            ddlsModalStatus.SelectedIndex = 1;
            ddlsModalStatus.Attributes.Add("disabled", "disabled");


        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {

            string sErro = "";
            if (hddidRegistroCRM.Value == "0")
            {
                if (ValidaNovoNegocio(1))
                {
                    try
                    {
                        DataSet dsSalvar;
                        Dictionary<String, String> vParametros = new Dictionary<string, string>();

                        //Cadastro
                        vParametros.Add("@sFuncao", "SALVAR");
                        vParametros.Add("@idRegistroCRM", hddidRegistroCRM.Value);
                        vParametros.Add("@dtInclusao", DateTime.Parse(txtdtInclusao.Text).ToString());
                        vParametros.Add("@idVendedor", ddlModalsVendedor.SelectedValue);
                        vParametros.Add("@idTipoCotacao", ddlModalsTipo.SelectedValue);
                        vParametros.Add("@idStatus", ddlsModalStatus.SelectedValue);
                        vParametros.Add("@dtPrevisao", txtdtPrevisao.Text);
                        vParametros.Add("@sDscParceiro", txtsCliente.Text);
                        vParametros.Add("@sContato", txtsContato.Text);
                        vParametros.Add("@sTelefone", txtsTelefone.Text);
                        vParametros.Add("@sEmail", txtsEmail.Text.Trim());
                        vParametros.Add("@sConfidencial", cbConfidencial.Checked ? "S" : "N");
                        vParametros.Add("@sChance", ddlChance.SelectedValue);
                        vParametros.Add("@nMaterial", txtnMaterial.Text == "" ? "0.0" : Convert.ToDecimal(txtnMaterial.Text).ToString().Replace(",", "."));
                        vParametros.Add("@nServico", txtnServico.Text == "" ? "0.0" : Convert.ToDecimal(txtnServico.Text).ToString().Replace(",", "."));

                        //decimal valorServico = txtnServico.Text == "" ? 0 : Convert.ToDecimal((txtnServico.Text));
                        //decimal valorMaterial = txtnMaterial.Text == "" ? 0 : Convert.ToDecimal((txtnMaterial.Text));
                        //decimal valorTotal = valorServico + valorMaterial;

                        //vParametros.Add("@nValor", valorTotal.ToString().Replace(",", "."));

                        vParametros.Add("@sReferencia", txtsReferencia.Text);
                        vParametros.Add("@sObservacao", txtsObservacao.Text);
                        vParametros.Add("@nControle", txtnControle.Text);

                        vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
                        dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                        if (BD.ValidarDataSet(dsSalvar, out sErro))
                        {
                            DetalheNegocio_MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                            hddidRegistroCRM.Value = Retorno.DATASET(dsSalvar, "idRegistroCRM");
                            ModoEdicao(hddidRegistroCRM.Value);

                        }
                        else
                        {
                            throw new Exception("BD: " + sErro.ToString());
                        }


                    }
                    catch (Exception ex)
                    {
                        MensagemPagina.MostraMensagem_Erro(ex.Message);
                    }
                }

            }
            else if (hddidRegistroCRM.Value != "0")
            {
                if (ValidaNovoNegocio(1))
                {
                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    //Cadastro
                    vParametros.Add("@sFuncao", "EDITAR");
                    vParametros.Add("@idRegistroCRM", hddidRegistroCRM.Value);
                    vParametros.Add("@dtInclusao", DateTime.Parse(txtdtInclusao.Text).ToString());
                    vParametros.Add("@idVendedor", ddlModalsVendedor.SelectedValue);
                    vParametros.Add("@idTipoCotacao", ddlModalsTipo.SelectedValue);
                    vParametros.Add("@idStatus", ddlsModalStatus.SelectedValue);
                    vParametros.Add("@dtPrevisao", txtdtPrevisao.Text);
                    vParametros.Add("@sDscParceiro", txtsCliente.Text);
                    vParametros.Add("@sContato", txtsContato.Text);
                    vParametros.Add("@sTelefone", txtsTelefone.Text);
                    vParametros.Add("@sEmail", txtsEmail.Text.Trim());
                    vParametros.Add("@sConfidencial", cbConfidencial.Checked ? "S" : "N");
                    vParametros.Add("@sChance", ddlChance.SelectedValue);
                    vParametros.Add("@nMaterial", txtnMaterial.Text == "" ? "0.0" : Convert.ToDecimal(txtnMaterial.Text).ToString().Replace(",", "."));
                    vParametros.Add("@nServico", txtnServico.Text == "" ? "0.0" : Convert.ToDecimal(txtnServico.Text).ToString().Replace(",", "."));

                    //decimal valorServico = txtnServico.Text == "" ? 0 : Convert.ToDecimal((txtnServico.Text));
                    //decimal valorMaterial = txtnMaterial.Text == "" ? 0 : Convert.ToDecimal((txtnMaterial.Text));
                    //decimal valorTotal = valorServico + valorMaterial;

                    //vParametros.Add("@nValor", valorTotal.ToString().Replace(",", "."));
                    vParametros.Add("@sReferencia", txtsReferencia.Text);
                    vParametros.Add("@sObservacao", txtsObservacao.Text);
                    vParametros.Add("@nControle", txtnControle.Text);

                    vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        DetalheNegocio_MensagemPagina.MostraMensagem_Sucesso("Registro editado com sucesso");
                        hddidRegistroCRM.Value = Retorno.DATASET(dsSalvar, "idRegistroCRM");
                        ModoEdicao(hddidRegistroCRM.Value);
                    }
                    else
                    {
                        throw new Exception("BD: " + sErro.ToString());
                    }
                }
            }
            RegistraScriptModalNegocio();
            ScriptsPagina(txtsCliente.ClientID, "ScriptCompletar", new List<string> { txtsCliente.ClientID }, Page);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalNovo", "$('#modalCRM').modal('show');", true);
        }

        protected void btnIncluir_Click(object sender, EventArgs e)
        {
            string sErro = "";
            if (ValidaNovoNegocio(2))
            {
                DataSet ds;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "SALVAR_FOLLOWUP");
                vParametros.Add("@idRegistroCRM", hddidRegistroCRM.Value);
                vParametros.Add("@dtContato", DateTime.Parse(txtdtDataContato.Text).ToString());
                vParametros.Add("@idMeioContato", ddlsMeioContato.SelectedValue);
                vParametros.Add("@sContatoCom", txtsContatoCom.Text);
                vParametros.Add("@sObservacaoFollowUP", txtsObservacaoFollowUp.Text);
                vParametros.Add("@dtProximoContato", DateTime.Parse(txtdtProximoContato.Text).ToString());
                vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
                ds = BD.ExecutarDataSet(sProcedure, vParametros);
                if (BD.ValidarDataSet(ds, out sErro))
                {
                    FollowUPNegocio_MensagemPagina.MostraMensagem_Sucesso("Contato inserido com sucesso!");
                    hddidRegistroCRM.Value = Retorno.DATASET(ds, "idRegistroCRM");
                }

                ModoEdicao(hddidRegistroCRM.Value);

                ddlsMeioContato.SelectedValue = "0";
                txtdtDataContato.Text = "";
                txtsContatoCom.Text = "";
                txtsObservacaoFollowUp.Text = "";
                txtdtProximoContato.Text = "";
            }


            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Followup", "$('#followUp-tab').tab('show');", true);
        }

        protected void btnNegocioFechado_Click(object sender, EventArgs e)
        {
            div_Acao_FollowUp.Visible = true;
            ddlsMotivo.Visible = false;
            hddStatusNegocio.Value = "S";
            lblNegocios_Acao_Titulo.Text = "Negócio Fechado";
            txtdtFinal.Focus();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao", "$('#modalAcao').modal('show');", true);
        }

        protected void btnNegocioPerdido_Click(object sender, EventArgs e)
        {
            div_Acao_FollowUp.Visible = true;
            Funcoes.Popula_Combo(ddlsMotivo, "sp_Select 'tbl_Flow_Comercial_CRM_Followup_Motivo'", "idMotivo", "sDscMotivo", false, "Selecione um Motivo", "0");
            ddlsMotivo.Visible = true;
            hddStatusNegocio.Value = "N";
            lblNegocios_Acao_Titulo.Text = "Negócio Perdido";
            txtdtFinal.Focus();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao", "$('#modalAcao').modal('show');", true);
        }

        protected void btnNegocioExcluido_Click(object sender, EventArgs e)
        {
            div_Acao_FollowUp.Visible = true;
            ddlsMotivo.Visible = false;
            hddStatusNegocio.Value = "E";
            lblNegocios_Acao_Titulo.Text = "Excluir Negócio";
            txtdtFinal.Focus();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao", "$('#modalAcao').modal('show');", true);
        }

        protected void btnNegocioOK_Click(object sender, EventArgs e)
        {
            string sErro = "";
            DataSet ds;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "EDITAR_FOLLOWUP");
            vParametros.Add("@idRegistroCRM", hddidRegistroCRM.Value);
            vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
            if (hddStatusNegocio.Value == "S" && ValidaNovoNegocio(3))
            {

                vParametros.Add("@dtFinalizacao", DateTime.Parse(txtdtFinalizacao.Text).ToString());
                vParametros.Add("@sMotivoFinalizacao", txtsMotivoObservacao.Text);
                vParametros.Add("@sFiltro", hddStatusNegocio.Value);
                vParametros.Add("@sAcao", "Negócio Fechado");
                vParametros.Add("@sObservacao", "Negócio Fechado");

                ds = BD.ExecutarDataSet(sProcedure, vParametros);
                if (BD.ValidarDataSet(ds, out sErro))
                {
                    if (Retorno.DATASET(ds, "nRet") == "0")
                    {
                        lblsDscTipoStatus.Text = Retorno.DATASET(ds, "sDscStatus");
                    }
                    else
                    {
                        DetalheNegocio_MensagemPagina.MostraMensagem_Aviso("Não foi encontrado o status de finalização cadastrado, favor cadastrar! Para mais dúvidas consulte o Manual");
                    }
                }
                ModoEdicao(hddidRegistroCRM.Value);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_detalhe", "$('#detalhe-tab').tab('show');", true);

            }
            else if (hddStatusNegocio.Value == "N" && ValidaNovoNegocio(4))
            {

                vParametros.Add("@dtFinalizacao", DateTime.Parse(txtdtFinalizacao.Text).ToString());
                vParametros.Add("@idMotivo", ddlsMotivo.SelectedValue);
                vParametros.Add("@sMotivoFinalizacao", txtsMotivoObservacao.Text);
                vParametros.Add("@sFiltro", hddStatusNegocio.Value);
                vParametros.Add("@sAcao", "Negócio Perdido");
                vParametros.Add("@sObservacao", "Negócio Perdido");

                ds = BD.ExecutarDataSet(sProcedure, vParametros);
                if (BD.ValidarDataSet(ds, out sErro))
                {
                    if (Retorno.DATASET(ds, "nRet") == "0")
                    {
                        lblsDscTipoStatus.Text = Retorno.DATASET(ds, "sDscStatus");
                    }
                    else
                    {
                        DetalheNegocio_MensagemPagina.MostraMensagem_Aviso("Não foi encontrado o status de cancelamento cadastrado, favor cadastrar! Para mais dúvidas consulte o Manual");
                    }
                }

                ModoEdicao(hddidRegistroCRM.Value);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_detalhe", "$('#detalhe-tab').tab('show');", true);

            }
            else if (hddStatusNegocio.Value == "E" && ValidaNovoNegocio(3))
            {
                vParametros.Add("@dtFinalizacao", DateTime.Parse(txtdtFinalizacao.Text).ToString());
                vParametros.Add("@sMotivoFinalizacao", txtsMotivoObservacao.Text);
                vParametros.Add("@sFiltro", hddStatusNegocio.Value);
                vParametros.Add("@sAcao", "Negócio Excluído");
                vParametros.Add("@sObservacao", "Negócio Excluído");

                ds = BD.ExecutarDataSet(sProcedure, vParametros);
                if (BD.ValidarDataSet(ds, out sErro))
                {
                    if (Retorno.DATASET(ds, "nRet") == "0")
                    {
                        lblsDscTipoStatus.Text = Retorno.DATASET(ds, "sDscStatus");
                    }
                    else
                    {
                        DetalheNegocio_MensagemPagina.MostraMensagem_Aviso("Não foi encontrado o status de excluído cadastrado, favor cadastrar! Para mais dúvidas consulte o Manual");
                    }
                }
                ModoEdicao(hddidRegistroCRM.Value);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_detalhe", "$('#detalhe-tab').tab('show');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao", "$('#modalAcao').modal('show');", true);
            }
            RegistraScriptModalNegocio();
            ScriptsPagina(txtsCliente.ClientID, "ScriptCompletar", new List<string> { txtsCliente.ClientID }, Page);
        }

        protected void btnNegocioCancelar_Click(object sender, EventArgs e)
        {
            btnAcao.Visible = true;
            Session["ItemId"] = hddidRegistroCRM.Value;
            Session["OpenModal"] = "modalCRM";
            Session["StatusModal"] = "true";
            Response.Redirect(Request.RawUrl);
        }

        //Agnes Partal - 28/06/2024 -----------------------------------------------------------------------------------
        protected void btnOrcamentoDetalhe_Click(object sender, EventArgs e)
        {
            int qtdOrcamento = ConsultaOrcamento();

            if (qtdOrcamento > 2)
            {
                ddlRevOrcamento.Visible = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Orcamento", "$('#modalConsultaOrcamento').modal('show');", true);
            }
            else
            {
                string url = "Orcamento_Detalhe.aspx?id=" + hddidOrcamento.Value;
                Response.Redirect(url);
            }

        }

        protected void cmdOrcamento_Click(object sender, EventArgs e)
        {
            if (hddVisualizaPDF.Value == "N")
            {
                if (ddlRevOrcamento.SelectedValue != "0")
                {
                    string url = "/App/Paginas/Comercial/Orcamento_Detalhe.aspx?id=" + ddlRevOrcamento.SelectedValue;
                    Response.Redirect(url);
                }
                else
                {
                    MensagemPaginaModalVincula.MostraMensagem_Erro("Selecione uma Revisão");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Orcamento", "$('#modalConsultaOrcamento').modal('show');", true);
                }

            }
            else
            {

                if (ddlRevOrcamento.SelectedValue != "0")
                {
                    string url = "/App/Paginas/Comercial/Orcamento_Detalhe.aspx?id=" + ddlRevOrcamento.SelectedValue + "&PDF=true";
                    string script = $"window.open('{url}', '_blank');";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "openNewTab", script, true);
                }
                else
                {
                    MensagemPaginaModalVincula.MostraMensagem_Erro("Selecione uma Revisão");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Orcamento", "$('#modalConsultaOrcamento').modal('show');", true);
                }

            }

        }
        //-------------------------------------------------------------------------------------------------------------

        protected void cmdFecharModalOrcamento_Click(object sender, EventArgs e)
        {
            Session["ItemId"] = hddidRegistroCRM.Value;
            Session["OpenModal"] = "modalCRM";
            Session["StatusModal"] = "true";
            Response.Redirect(Request.RawUrl);

        }

        protected void btnVisualizaPDF_Click(object sender, EventArgs e)
        {
            int qtdOrcamento = ConsultaOrcamento();
            hddVisualizaPDF.Value = "S";

            if (qtdOrcamento > 2)
            {
                ddlRevOrcamento.Visible = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Orcamento", "$('#modalConsultaOrcamento').modal('show');", true);
            }
            else
            {
                string url = "Orcamento_Detalhe.aspx?id=" + hddidOrcamento.Value + "&PDF=true";
                string script = $"window.open('{url}', '_blank');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "openNewTab", script, true);
            }
        }

        #endregion

        #region | Modal Edição e Consulta
        private void ModoEdicao(string idRegistroCRM)
        {

            lblModalTitulo.Text = "Alterar Negócio";
            aba_Arquivo.Visible = true;
            DIV_Arquivos.Visible = true;
            div_followUp.Visible = true;
            aba_FollowUp.Visible = true;
            aba_Historico.Visible = true;
            DIV_historico.Visible = true;
            div_Acao_FollowUp.Visible = false;
            btnAcao.Visible = true;


            LimparCampos();
            try
            {
                string sErro = "";
                DataSet ds;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                vParametros.Add("@idRegistroCRM", idRegistroCRM);
                ds = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(ds, out sErro))
                {

                    txtdtInclusao.Text = Retorno.DATASET(ds, 0, "dtInclusao");
                    ddlModalsVendedor.SelectedValue = Retorno.DATASET(ds, 0, "idVendedor");
                    ddlModalsTipo.SelectedValue = Retorno.DATASET(ds, 0, "idTipoCotacao");
                    string idParceiro = Retorno.DATASET(ds, 0, "idParceiro");
                    ddlChance.SelectedValue = Retorno.DATASET(ds, 0, "sChance");
                    txtsCliente.Text = Retorno.DATASET(ds, 0, "sDscParceiro");
                    txtsContato.Text = Retorno.DATASET(ds, 0, "sContato");
                    txtsTelefone.Text = Retorno.DATASET(ds, 0, "sTelefoneContato");
                    txtsEmail.Text = Retorno.DATASET(ds, 0, "sEmail");
                    txtnValor.Text = Retorno.DATASET(ds, 0, "nNumeroOrcamento");
                    txtsReferencia.Text = Retorno.DATASET(ds, 0, "sReferencia");
                    txtsObservacao.Text = Retorno.DATASET(ds, 0, "sObservacao");
                    txtdtPrevisao.Text = Retorno.DATASET(ds, 0, "dtPrevisao") == "1900-01-01" ? "" : Retorno.DATASET(ds, 0, "dtPrevisao");
                    txtnServico.Text = Retorno.DATASET(ds, 0, "nServico");
                    txtnMaterial.Text = Retorno.DATASET(ds, 0, "nMaterial");
                    txtnControle.Text = Retorno.DATASET(ds, 0, "nControle");

                    string confidencial = Retorno.DATASET(ds, 0, "sConfidencial");
                    if (confidencial.Equals("S")) cbConfidencial.Checked = true;

                    string sDtFinalizacao = Retorno.DATASET(ds, 0, "dtFinalizacao");
                    string sDtProximoContato = Retorno.DATASET(ds, 0, "dtProximoContato");
                    string sStatus = Retorno.DATASET(ds, 0, "sDscStatus");
                    string idOrcamento = Retorno.DATASET(ds, 0, "idOrcamento");



                    if (sDtProximoContato == "")
                    {
                        lblsDscTipoStatus.Text = sStatus;
                        lblsDscTipoStatus.CssClass = string.Format("label label-{0}", Retorno.DATASET(ds, 0, "sCor"));
                    }
                    else
                    {
                        lblsDscTipoStatus.Text = sStatus;
                        lbldtProximoContato.Text = sDtProximoContato;
                        lblsDscTipoStatus.CssClass = string.Format("label label-{0}", Retorno.DATASET(ds, 0, "sCor"));
                        lbldtProximoContato.CssClass = string.Format("label label-{0}", Retorno.DATASET(ds, 0, "sCor"));
                    }

                    if (idParceiro == "0")
                    {
                        txtsCliente.ReadOnly = false;
                    }
                    else
                    {
                        txtsCliente.ReadOnly = true;
                    }

                    if (!Funcoes.ValidaPermissao(Permissao.Comercial.CRM.Visualizar_Vendedores))
                    {
                        ddlModalsVendedor.SelectedValue = ConsultaVendedor();
                        ddlModalsVendedor.Attributes.Add("disabled", "disabled");
                    }

                    if (!Funcoes.ValidaPermissao(Permissao.Comercial.CRM.Confidencial))
                    {
                        div_confidencial.Visible = false;
                    }
                    if (Funcoes.ValidaPermissao(Permissao.Comercial.CRM.Excluir))
                    {
                        div_btnExcluir.Visible = true;
                    }

                    if (Funcoes.ValidaPermissao(Permissao.Comercial.Orcamento.Consultar))
                    {
                        if (idOrcamento == "" || idOrcamento == "0")
                        {
                            div_btnNovoOrcamento.Visible = true;
                            div_btnOrcamentoDetalhe.Visible = false;
                        }
                        else
                        {

                            div_btnNovoOrcamento.Visible = false;
                            div_btnOrcamentoDetalhe.Visible = true;
                            div_btnVisualizaPDF.Visible = true;
                        }
                    }

                    ddlModalsTipo.Attributes.Add("disabled", "disabled");
                    ddlsModalStatus.Attributes.Remove("disabled");
                    txtnValor.ReadOnly = true;

                    Popular_Aba_Historico(ds);
                    Popular_HistoricoContato(ds, 2);
                    Popular_Documentos(idRegistroCRM);

                    if (sDtFinalizacao != "")
                    {
                        Funcoes.Popula_Combo(ddlsModalStatus, "sp_Select 'Status CRM', @idPesquisa=" + nTipoStatus + "", "idStatus", "sDscStatus", false, "Todos os Status", ddlsStatus.SelectedValue);
                        ddlsModalStatus.SelectedValue = Retorno.DATASET(ds, 0, "idStatus");
                        ModoConsulta();
                        lbldtProximoContato.Visible = false;
                    }
                    else
                    {
                        ddlsModalStatus.SelectedValue = Retorno.DATASET(ds, 0, "idStatus");
                    }
                }
            }
            catch { }

            hddStatusModal.Value = "true3";
            RegistraScriptModalNegocio();
        }

        private void ModoConsulta()
        {
            btnIncluir.Visible = false;
            txtdtDataContato.ReadOnly = true;
            txtsContatoCom.ReadOnly = true;
            txtsObservacaoFollowUp.ReadOnly = true;
            txtdtProximoContato.ReadOnly = true;
            txtdtInclusao.ReadOnly = true;
            txtsCliente.ReadOnly = true;
            txtsContato.ReadOnly = true;
            txtsTelefone.ReadOnly = true;
            txtsEmail.ReadOnly = true;
            txtnValor.ReadOnly = true;
            txtsReferencia.ReadOnly = true;
            txtsObservacao.ReadOnly = true;
            txtdtPrevisao.ReadOnly = true;
            txtnMaterial.ReadOnly = true;
            txtnServico.ReadOnly = true;
            ddlChance.Attributes.Add("disabled", "disabled");
            ddlsMeioContato.Attributes.Add("disabled", "disabled");
            ddlModalsVendedor.Attributes.Add("disabled", "disabled");
            ddlModalsTipo.Attributes.Add("disabled", "disabled");
            ddlsModalStatus.Attributes.Add("disabled", "disabled");
            cbConfidencial.Attributes.Add("disabled", "disabled");
            btnAcao.Visible = false;
            pnBtnSalvar.Visible = false;
            btnAcao.Visible = false;
            div_btnNovoOrcamento.Visible = false;
        }
        #endregion

        #region | Validação

        private bool ValidaNovoNegocio(int nTipoValidacao)
        {
            bool bRetorno = true;
            string sMensagemErro = "";
            if (nTipoValidacao == 1)
            {
                if (!Validacoes.ValidarData(txtdtInclusao))
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data inválida!";
                }

                if (ddlModalsTipo.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Tipo!";
                }

                //if (txtsContato.Text.Length < 4)
                //{
                //    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Contato inválido!";
                //}
                //if (!Validacoes.ValidarEmail(txtsEmail.Text))
                //{
                //    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "E-mail inválido!";
                //}

                //if (!Validacoes.ValidarTelefone(txtsTelefone.Text))
                //{
                //    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Telefone inválido";
                //}

                if (ddlModalsVendedor.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Vendedor!";
                }

                //if (ddlChance.SelectedValue == "")
                //{
                //    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Chance!";
                //}

                //if (!Validacoes.ValidarData(txtdtPrevisao))
                //{
                //    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Previsão inválida!";
                //}

                if (sMensagemErro != "")
                {
                    bRetorno = false;
                    DetalheNegocio_MensagemPagina.MostraMensagem_Erro(sMensagemErro);
                }

                return bRetorno;
            }
            else if (nTipoValidacao == 2)
            {
                if (!Validacoes.ValidarData(txtdtDataContato))
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data do Contato inválida!";
                }
                if (ddlsMeioContato.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Meio de Contato!";
                }
                if (txtsContatoCom.Text.Length < 3)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Contato inválido!";
                }
                if (!Validacoes.ValidarData(txtdtProximoContato))
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data do Próximo Contato inválida!";
                }
                if (sMensagemErro != "")
                {
                    bRetorno = false;
                    FollowUPNegocio_MensagemPagina.MostraMensagem_Erro(sMensagemErro);
                }

                return bRetorno;
            }
            else if (nTipoValidacao == 3)
            {
                if (!Validacoes.ValidarData(txtdtFinalizacao))
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data de Finalização inválida!";
                }
                if (txtsMotivoObservacao.Text.Length < 5)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Observação está muito curta! Digite ao menos 5 caracteres";
                }
                if (sMensagemErro != "")
                {
                    bRetorno = false;
                    AcaoFollowUp_MensagemPagina.MostraMensagem_Erro(sMensagemErro);
                }

                return bRetorno;

            }
            else if (nTipoValidacao == 4)
            {
                if (!Validacoes.ValidarData(txtdtFinalizacao))
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data de Finalização inválida!";
                }
                if (txtsMotivoObservacao.Text.Length < 5)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Observação está muito curta! Digite ao menos 5 caracteres";
                }
                if (ddlsMotivo.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Motivo!";
                }
                if (sMensagemErro != "")
                {
                    bRetorno = false;
                    AcaoFollowUp_MensagemPagina.MostraMensagem_Erro(sMensagemErro);
                }

                return bRetorno;
            }

            return bRetorno;
        }
        #endregion

        #region | Limpa Campos
        private void LimparCampos()
        {
            txtsCliente.Text = "";
            txtsContato.Text = "";
            txtsTelefone.Text = "";
            txtsEmail.Text = "";
            txtnValor.Text = "0";
            txtsReferencia.Text = "";
            txtsObservacao.Text = "";
            ddlModalsTipo.SelectedValue = "0";
            txtnMaterial.Text = "0";
            txtnServico.Text = "0";
            txtdtPrevisao.Text = "";
            ddlChance.SelectedValue = "";
        }
        #endregion

        #region | Script
        void RegistraScriptModalNegocio()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("$(document).ready(function () {");

            //Mascaras
            sb.AppendLine("    $('[id*=txtsTelefone]').mask('(99) 99999-9999',{");
            sb.AppendLine("         onKeyPress: function(val, e, field, options) {");
            sb.AppendLine("             field.mask(val.length > 14 ? '(00) 00000-0009' : '(00) 0000-00009', options);}");
            sb.AppendLine("    });");

            sb.AppendLine("    $('.valor-input').mask('000.000.000.000.000,00', {reverse: true});");
            sb.AppendLine("");
            sb.AppendLine("    function calcularTotal() {");
            sb.AppendLine("        var valor1 = $('#" + txtnMaterial.ClientID + "').val().replace(/\\./g, '').replace(',', '.');");
            sb.AppendLine("        var valor2 = $('#" + txtnServico.ClientID + "').val().replace(/\\./g, '').replace(',', '.');");
            sb.AppendLine("");
            sb.AppendLine("        valor1 = parseFloat(valor1) || 0;");
            sb.AppendLine("        valor2 = parseFloat(valor2) || 0;");
            sb.AppendLine("");
            sb.AppendLine("        var total = valor1 + valor2;");
            sb.AppendLine("        var totalFormatado = total.toFixed(2).replace('.', ',').replace(/(\\d)(?=(\\d{3})+(?!\\d))/g, \"$1.\"); ");
            sb.AppendLine("        $('#" + txtnValor.ClientID + "').val(totalFormatado);");
            sb.AppendLine("        $('#" + txtnValor.ClientID + "').mask('000.000.000.000.000,00', { reverse: true });");
            sb.AppendLine("    }");
            sb.AppendLine("");
            sb.AppendLine("    $('.valor-input').on('keyup change', function() {");
            sb.AppendLine("        calcularTotal();");
            sb.AppendLine("    });");

            //sb.AppendLine("    function showLoader() {");
            //sb.AppendLine("         $('#pnKanban .loader-container').show();");
            //sb.AppendLine("        }");
            //sb.AppendLine("          function hideLoader() {");
            //sb.AppendLine("             $('#pnKanban .loader-container').hide();");
            //sb.AppendLine("        }");
            //sb.AppendLine("    });");
            //sb.AppendLine("    $('[id*=txtnMaterial],[id*=txtnServico]').on('blur', function () {");
            //sb.AppendLine("        let valor = $(this).val().replace('R$', '').trim();");
            //sb.AppendLine("        if (valor) {");
            //sb.AppendLine("            valor = parseFloat(valor.replace(/\\./g, '').replace(',', '.'));");
            //sb.AppendLine("            $(this).val('R$ ' + valor.toLocaleString('pt-BR', { minimumFractionDigits: 2 }));");
            //sb.AppendLine("        }");
            //sb.AppendLine("        calculateTotal();");
            //sb.AppendLine("    });");
            //sb.AppendLine("    function calculateTotal() {");
            //sb.AppendLine("        let total = 0;");
            //sb.AppendLine("        $('[id*=txtnMaterial],[id*=txtnServico]').each(function() {");
            //sb.AppendLine("            let valor = $(this).val().replace('R$', '').trim();");
            //sb.AppendLine("            if (valor) {");
            //sb.AppendLine("                valor = parseFloat(valor.replace(/\\./g, '').replace(',', '.'));");
            //sb.AppendLine("                if (!isNaN(valor)) total += valor;");
            //sb.AppendLine("            }");
            //sb.AppendLine("        });");
            //sb.AppendLine("        $('#txtnValor').val('R$ ' + total.toLocaleString('pt-BR', { minimumFractionDigits: 2 }));");
            //sb.AppendLine("    }");
            //sb.AppendLine("    calculateTotal();");
            sb.AppendLine("    calcularTotal();");
            sb.AppendLine("});");


            ScriptManager.RegisterStartupScript(this, this.GetType(), "ModalMascaraCRMScript", sb.ToString(), true);


        }

        public string ScriptsPagina(string dgPagina, string tipoScript, List<string> elementos, Page pg)
        {
            string script = "";
            Dictionary<string, string> vParametrosAjax = new Dictionary<string, string>();
            vParametrosAjax.Add("sRazaoSocial", "JSON.stringify(request.term)");
            vParametrosAjax.Add("sidTipoParceiro", "'0'");
            //vParametrosAjax.Add("idCliente", "$('[id*=ddlTipoProduto]').val() || 'S'");

            if (tipoScript == "ScriptCompletar")
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("$v192(function() {");
                sb.Append("$v192(\"#" + dgPagina + "\").autocomplete({");
                sb.Append("source: function(request, response) {");
                sb.Append("$v192.ajax({");
                sb.Append("url:'/API/Pagina_Ajax.aspx/GetParceiros',");
                sb.Append("data: JSON.stringify({");

                foreach (KeyValuePair<string, string> item in vParametrosAjax)
                {
                    sb.Append("'" + item.Key + "': " + item.Value + ", ");
                }

                if (vParametrosAjax.Count > 0)
                {
                    sb.Length -= 2; // Remove a vírgula extra
                }

                sb.Append("}),"); // Adicione uma vírgula após a chave 'data'

                sb.Append("dataType: \"json\",");
                sb.Append("type: \"POST\",");
                sb.Append("contentType: \"application/json; charset=utf-8\",");
                sb.Append("success: function(data) {");
                sb.Append("response($v192.map(data.d, function(item) {");
                sb.Append("return {");

                sb.Append("label: item.split('|')[0],");

                int index = 0;
                foreach (string elementoID in elementos)
                {
                    sb.Append(elementoID + ": item.split('|')[" + (index) + "],");
                    index++;
                }

                sb.Remove(sb.Length - 1, 1);
                sb.Append("};");
                sb.Append("}));"); // Adicione parênteses de fechamento para a função 'map'
                sb.Append("},");
                sb.Append("error: function(response) {");
                sb.Append("alert(response.responseText);");
                sb.Append("},");
                sb.Append("failure: function(response) {");
                sb.Append("alert(response.responseText);");
                sb.Append("}");
                sb.Append("});");
                sb.Append("},");
                sb.Append("select: function(e, i) {");

                foreach (string elementoID in elementos)
                {
                    sb.Append("$(\"#" + elementoID + "\").val(i.item." + elementoID + ");");
                }


                sb.Append("},");
                sb.Append("minLength: 3");
                sb.Append("});");
                sb.Append("});");

                ScriptManager.RegisterStartupScript(pg, pg.GetType(), "js_PesquisaParceiros" + Guid.NewGuid(), sb.ToString(), true);
            }
            else
            {
                //string codigoJavaScript = System.IO.Path.Combine("~/App/JS/TabelaConsulta.js");
                //string scriptPagina = File.ReadAllText(codigoJavaScript);
                //ScriptManager.RegisterStartupScript(pg, pg.GetType(), "js_ScriptPagina_" + tipoScript, scriptPagina, true);
            }

            return script;
        }
        #endregion

        #region | Popula Historico e Arquivo

        void Popular_HistoricoContato(DataSet ds, int nTabela)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(gvFollowUp, ds.Tables[nTabela], 1, "desc"), true);
        }

        void Popular_Aba_Historico(DataSet ds)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(gv_Historico, ds.Tables[1], 1, "desc"), true);
        }

        void Popular_Documentos(string idRegistroCRM)
        {
            frmArquivos.Attributes.Add("src", string.Format("~/App/Paginas/Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}", idRegistroCRM, "CRM"));
            frmArquivos.Visible = true;
            DIV_Arquivos.Visible = true;
            aba_Arquivo.Visible = true;
        }
        #endregion

        #region | Metodos Changed
        protected void ddlsStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            confidencial = TipoPesquisa();
            Pesquisar(confidencial);
            if (ddlsStatus.SelectedValue != "" && idKanban.Checked)
            {
                hddFiltroStatus.Value = "S";
                CarregaBoard(hddFiltroStatus.Value);
                CarregaCard(confidencial);
                hddFiltroStatus.Value = "N";
            }

        }

        protected void ddlsVendedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            confidencial = TipoPesquisa();
            Pesquisar(confidencial);
        }

        protected void ddlsTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            confidencial = TipoPesquisa();
            Pesquisar(confidencial);
        }

        protected void ddlsCLiente_SelectedIndexChanged(object sender, EventArgs e)
        {
            confidencial = TipoPesquisa();
            Pesquisar(confidencial);
        }

        protected void txtdtFinal_TextChanged(object sender, EventArgs e)
        {
            confidencial = TipoPesquisa();
            Pesquisar(confidencial);
        }

        protected void txtdtInicial_TextChanged(object sender, EventArgs e)
        {
            confidencial = TipoPesquisa();
            Pesquisar(confidencial);
        }

        protected void txtnMaterial_TextChanged(object sender, EventArgs e)
        {

        }

        protected void txtnServico_TextChanged(object sender, EventArgs e)
        {

        }

        protected void idGrid_CheckedChanged(object sender, EventArgs e)
        {
            idKanban.Checked = !idGrid.Checked;
            pnKanban.Visible = idKanban.Checked;
            pnGrid.Visible = idGrid.Checked;

        }

        protected void idKanban_CheckedChanged(object sender, EventArgs e)
        {

            idGrid.Checked = !idKanban.Checked;
            pnGrid.Visible = idGrid.Checked;
            pnKanban.Visible = idKanban.Checked;

            if (idKanban.Checked)
            {
                CarregaBoard(hddFiltroStatus.Value);
                confidencial = TipoPesquisa();
                CarregaCard(confidencial);

            }

        }

        #endregion

        #region| Kanban
        public void CarregaBoard(String filtro)
        {
            var boards = new List<Board>();

            DataSet ds;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "BOARD_STATUS");
            vParametros.Add("@sFiltro", filtro);


            if (hddFiltroStatus.Value == "N")
            {
                ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_CRM", vParametros);
                foreach (DataRow status in ds.Tables[0].Rows)
                {
                    boards.Add(new Board
                    {
                        color = "_" + status["sCor"].ToString(),
                        title = status["sDscStatus"].ToString(),
                        id = status["idStatus"].ToString(),
                    });
                }

                string boardJson = new JavaScriptSerializer().Serialize(boards);
                RegistraBoardScript(boardJson);
            }
            else if (hddFiltroStatus.Value == "S")
            {
                vParametros.Add("@idStatus", ddlsStatus.SelectedValue);
                ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_CRM", vParametros);
                foreach (DataRow status in ds.Tables[0].Rows)
                {
                    boards.Add(new Board
                    {
                        color = "_" + status["sCor"].ToString(),
                        title = status["sDscStatus"].ToString(),
                        id = status["idStatus"].ToString(),
                    });
                }

                string boardJson = new JavaScriptSerializer().Serialize(boards);
                RegistraBoardScriptFiltro(boardJson);
            }

        }

        void CarregaCard(string confidencial)
        {
            List<object> cards = new List<object>();
            DataSet ds;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR");
            vParametros.Add("@dtInclusao", txtdtInicial.Text);
            vParametros.Add("@dtUltimoContato", txtdtFinal.Text);
            vParametros.Add("@idCliente", ddlsCLiente.SelectedValue);
            vParametros.Add("@idTipoCotacao", ddlsTipo.SelectedValue);
            vParametros.Add("@idVendedor", ddlsVendedor.SelectedValue);
            vParametros.Add("@idStatus", ddlsStatus.SelectedValue);
            vParametros.Add("@sConfidencial", confidencial);
            ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_CRM", vParametros);

            foreach (DataRow negocio in ds.Tables[0].Rows)
            {
                string id = negocio["idRegistroCRM"].ToString();

                foreach (DataRow outraLinha in ds.Tables[0].Rows)
                {
                    if (outraLinha == negocio) continue;

                    string outroId = outraLinha["idRegistroCRM"].ToString();
                    if (id == outroId)
                    {
                        id = id + "[" + identificador + "]";
                        identificador++;
                        break;
                    }
                }
                string sChance = "";
                if (negocio["sChance"].ToString() != "") sChance = negocio["sChance"] + "%";

                var card = new
                {
                    id = id,
                    title = negocio["sReferencia"],
                    proximoContato = negocio["dtProximoContato"],
                    chance = sChance,
                    cliente = negocio["sCliente"],
                    valor = Convert.ToDecimal(negocio["nValor"]).ToString("C"),
                    color = "_" + negocio["sCor"].ToString(),
                    position = negocio["idStatus"],
                };
                cards.Add(card);
            }
            if (hddFiltroStatus.Value == "N")
            {
                string cardsJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { cards = cards });
                RegistraCardScript(cardsJson);
            }
            else if (hddFiltroStatus.Value == "S")
            {
                string cardsJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { cards = cards });
                RegistraCardScriptFiltro(cardsJson);
            }


        }
        protected void RegistraCardScript(string cardsJson)
        {
            string scriptCard = $@"                            
                            <script>
                            $(document).ready(function() {{
                            let card = {cardsJson};
                            console.log(card);
                            initializeComponents(card);
                            }});
                            </script>                            
                            ";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "appendCardScript", scriptCard, false);
        }

        protected void RegistraCardScriptFiltro(string cardsJson)
        {
            string scriptCard = $@"                            
                            <script>
                            $(document).ready(function() {{
                            let newCard = {cardsJson};
                            console.log(newCard);
                            initializeComponents(newCard);
                            }});
                            </script>                            
                            ";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "CardFiltro", scriptCard, false);
        }

        protected void RegistraBoardScript(string boardJson)
        {
            string scriptBoard = $@"                            
                            $(document).ready(function() {{
                            let dataColors = {boardJson};                            
                            initializeBoards(dataColors);
                            }});                            
                            ";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "appendBoardScript", scriptBoard, true);
        }

        protected void RegistraBoardScriptFiltro(string boardJson)
        {
            string scriptBoard = $@"                            
                            $(document).ready(function() {{
                            let newDataColors = {boardJson};
                            console.log(newDataColors);
                            initializeBoards(newDataColors);
                            }});                            
                            ";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "boardFiltro", scriptBoard, true);
        }

        [System.Web.Services.WebMethod]
        public static string AlterarStatusKanban(string sCardId, string sPosicaoInicial, string sPosicaoFinal)
        {
            string id = Regex.Match(sCardId, @"^\d+").Value;

            DataTable dt;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "ALTERAR_STATUS");
            vParametros.Add("@idStatus", sPosicaoFinal);
            vParametros.Add("@idRegistroCRM", id);
            vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
            dt = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Comercial_CRM", vParametros);


            return $"Card: {sCardId} moveu de {sPosicaoInicial} para {sPosicaoFinal}";
        }

        protected void btnTriggerModalKanban_Click(object sender, EventArgs e)
        {
            PopulaCombos();
            ModoEdicao(hddIdCard.Value);
            hddidRegistroCRM.Value = hddIdCard.Value;
            hddStatusModal.Value = "true4";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalKanban", "$('#modalCRM').modal('show');", true);
        }
    }

    public class Board
    {
        public string color { get; set; }
        public string title { get; set; }
        public string id { get; set; }
    }
    #endregion 
}

