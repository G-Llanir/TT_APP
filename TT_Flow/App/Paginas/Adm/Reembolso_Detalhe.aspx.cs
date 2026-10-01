using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using System.IO;
using System.Text;
using Microsoft.Reporting.WebForms;
using System.Globalization;
using TT_Flow.FrameWork;

namespace TT_Flow.App.Paginas.Manutencao
{
    public partial class Reembolso_Detalhe : System.Web.UI.Page
    {
        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Reembolso";
        public static decimal vlrMaximo = 0;
        public List<TT_Flow.FrameWork.cls_Despesas> Base_Despesas
        {
            get
            {
                if (ViewState["Base_Despesas"] == null)
                {
                    ViewState["Base_Despesas"] = new List<FrameWork.cls_Despesas>();
                }
                return (List<FrameWork.cls_Despesas>)ViewState["Base_Despesas"];
            }

            set
            {
                ViewState["Base_Despesas"] = value;
            }
        }

        public List<Cls_TipoDespesa> Base_TipoDespesa
        {
            get
            {
                if (ViewState["Base_TipoDespesa"] == null)
                {
                    ViewState["Base_TipoDespesa"] = new List<Cls_TipoDespesa>();
                }
                return (List<Cls_TipoDespesa>)ViewState["Base_TipoDespesa"];
            }

            set
            {
                ViewState["Base_TipoDespesa"] = value;
            }
        }

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                var requestTarget = this.Request["__EVENTTARGET"];
                if (requestTarget == "funcao_Finalizar")
                    Finalizar();
            }
            else
            {
                PopularTiposDespesas();
                ConfigurarParaReembolso();
                if (Request["id"] != null)
                {
                    hddidDespesas.Value = Request["id"].ToString();
                    Pesquisar(Request["id"].ToString());
                }
                if (Request["sMsg"] != null)
                {
                    MensagemPagina_Entregas.MostraMensagem_Sucesso("Relatório Salvo Com Sucesso!");
                }
            }
            if (Request.Browser.IsMobileDevice)
            {
                ddlsParticipantes.Visible = true;
                chkParticipantes.Visible = false;
            }
            else
            {
                ddlsParticipantes.Visible = true;
                chkParticipantes.Visible = false;
            }

            RegistraScript();
        }
        #endregion

        #region | Metodos Banco de Dados
        void Pesquisar(string idDespesas)
        {
            try
            {
                DIV_Valor.Visible = false;
                DIV_Local.Visible = false;
                DIV_Participantes.Visible = false;
                div4.Visible = false;
                DIV_Pagamento.Visible = false;
                btnExcluir.Visible = false;
                lblTituloPagina.Text = "Relatório de Reembolso";
                PopulaCombo();
                LimparDespesas();

                if (idDespesas != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idDespesas", idDespesas);

                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (dsPesquisa.Tables[1].Rows.Count > 0)
                    {
                        txtsDscMotivo.Text = RETORNO.DATASET(dsPesquisa, 1, 0, "sDscMotivo");
                        ddlCentroCusto.SelectedValue = RETORNO.DATASET(dsPesquisa, 1, 0, "idPedido");
                        hddidCentrodeCusto.Value = RETORNO.DATASET(dsPesquisa, 1, 0, "idPedido");
                        txtsObservacao.Text = RETORNO.DATASET(dsPesquisa, 1, 0, "sDscObservacao");
                        lblsDscTipoStatus.Text = RETORNO.DATASET(dsPesquisa, 1, 0, "sStatus");
                        ddlsTipoCompra.SelectedValue = RETORNO.DATASET(dsPesquisa, 1, 0, "idTipo");

                        lblsDscTipoStatus.Text = RETORNO.DATASET(dsPesquisa, 1, 0, "sStatus");
                        ConfigurarParaReembolso();
                        lblsDscTipoStatus.CssClass = string.Format("label label-{0}", RETORNO.DATASET(dsPesquisa, 1, 0, "sCor"));

                        ddlCentroCusto.Attributes.Add("disabled", "disabled");
                        ddlsTipoCompra.Attributes.Add("disabled", "disabled");
                        this.gv_Despesas.Columns[8].Visible = true;
                        this.gv_Despesas.Columns[9].Visible = false;

                        if (RETORNO.DATASET(dsPesquisa, 1, 0, "idStatus") == "2" || RETORNO.DATASET(dsPesquisa, 1, 0, "idStatus") == "3")
                        {
                            txtsDscMotivo.ReadOnly = true;
                            txtsObservacao.ReadOnly = true;
                            ddlCentroCusto.Attributes.Add("disabled", "disabled");
                            ddlsTipoCompra.Attributes.Add("disabled", "disabled");
                            DIV_Categoria.Visible = false;
                            DIV_Pagamento.Visible = false;
                            DIV_dtDespesas.Visible = false;
                            DIV_Valor.Visible = false;
                            DIV_Local.Visible = false;
                            DIV_Inserir.Visible = false;
                            Div_NovoGasto.Visible = false;

                            DIV_Participantes.Visible = false;
                            cmdSalvar.Visible = false;
                            btnFinalizar.Visible = false;
                            this.gv_Despesas.Columns[8].Visible = false;
                            this.gv_Despesas.Columns[9].Visible = true;
                            Button1.Visible = true;

                            if (RETORNO.DATASET(dsPesquisa, 1, 0, "sGestor") == "S")
                            {
                                if (RETORNO.DATASET(dsPesquisa, 1, 0, "sDscUsuario") != IDENTITY.Variaveis.sUsuarioLogado())
                                {
                                    cmdAprovar.Visible = true;
                                    cmdRejeitar.Visible = true;
                                }
                                else
                                {
                                    cmdAprovar.Visible = false;
                                    cmdRejeitar.Visible = false;
                                }
                            }
                            else
                            {
                                cmdAprovar.Visible = false;
                                cmdRejeitar.Visible = false;
                            }

                            if (RETORNO.DATASET(dsPesquisa, 1, 0, "idStatus") == "3")
                            {
                                cmdAprovar.Visible = false;
                                cmdRejeitar.Visible = false;
                            }
                        }
                        else if (RETORNO.DATASET(dsPesquisa, 1, 0, "idStatus") == "4")
                        {
                            cmdSalvar.Visible = true;
                            btnFinalizar.Visible = false;
                            Button1.Visible = false;
                            cmdAprovar.Visible = false;
                            cmdRejeitar.Visible = false;
                            btnExcluir.Visible = true;

                            if (RETORNO.DATASET(dsPesquisa, 1, 0, "sDscUsuario") != IDENTITY.Variaveis.sUsuarioLogado())
                            {
                                cmdSalvar.Visible = false;
                                btnFinalizar.Visible = false;
                                txtsDscMotivo.ReadOnly = true;
                                txtsObservacao.ReadOnly = true;
                                ddlCentroCusto.Attributes.Add("disabled", "disabled");
                                ddlsTipoCompra.Attributes.Add("disabled", "disabled");
                                DIV_Categoria.Visible = false;
                                DIV_Pagamento.Visible = false;
                                DIV_dtDespesas.Visible = false;
                                DIV_dtDespesas.Visible = false;
                                DIV_Valor.Visible = false;
                                DIV_Local.Visible = false;
                                DIV_Inserir.Visible = false;
                                Div_NovoGasto.Visible = false;

                                DIV_Participantes.Visible = false;
                                this.gv_Despesas.Columns[8].Visible = false;
                                this.gv_Despesas.Columns[9].Visible = true;
                            }
                        }
                        else if (RETORNO.DATASET(dsPesquisa, 1, 0, "idStatus") == "5")
                        {
                            txtsDscMotivo.ReadOnly = true;
                            txtsObservacao.ReadOnly = true;
                            ddlCentroCusto.Attributes.Add("disabled", "disabled");
                            ddlsTipoCompra.Attributes.Add("disabled", "disabled");
                            DIV_Categoria.Visible = false;
                            DIV_Pagamento.Visible = false;
                            DIV_dtDespesas.Visible = false;
                            DIV_Valor.Visible = false;
                            DIV_Local.Visible = false;
                            DIV_Inserir.Visible = false;
                            Div_NovoGasto.Visible = false; //Thiago - 07/11/2025

                            DIV_Participantes.Visible = false;
                            cmdSalvar.Visible = false;
                            gv_Despesas.Columns[8].Visible = false;
                            gv_Despesas.Columns[9].Visible = true;
                            Button1.Visible = false;
                            btnExcluir.Visible = false;
                            btnFinalizar.Visible = false;
                            cmdRejeitar.Visible = false;
                            cmdAprovar.Visible = false;
                        }
                        else
                        {
                            btnFinalizar.Visible = true;
                            Div_NovoGasto.Visible = true;
                            Button1.Visible = false;
                            cmdAprovar.Visible = false;
                            cmdRejeitar.Visible = false;
                            btnExcluir.Visible = true;
                        }

                        PopularDespesas(dsPesquisa);
                        PopularHistorico(idDespesas);

                        if (dsPesquisa.Tables[2].Rows.Count > 0)
                        {
                            btnFinalizar.Visible = true;
                            if (RETORNO.DATASET(dsPesquisa, 1, 0, "idStatus") == "2" || RETORNO.DATASET(dsPesquisa, 1, 0, "idStatus") == "3" || RETORNO.DATASET(dsPesquisa, 1, 0, "idStatus") == "4" || RETORNO.DATASET(dsPesquisa, 1, 0, "idStatus") == "5")
                                btnFinalizar.Visible = false;
                        }
                        else
                        {
                            btnFinalizar.Visible = false;
                        }
                    }
                    else
                    {

                    }
                }
                else
                {
                    txtdtDespesas.Text = ConverterDateType(DateTime.Now);
                    Button1.Visible = false;
                    cmdAprovar.Visible = false;
                    cmdRejeitar.Visible = false;
                    btnFinalizar.Visible = false;                  
                    this.gv_Despesas.Columns[9].Visible = false;
                    Div_NovoGasto.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_Entregas.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }
        protected string GetParticipanteTags(object participantes, string separador)
        {
            if (participantes == null || string.IsNullOrWhiteSpace(participantes.ToString()))
            {
                return "";
            }

            string participantesStr = participantes.ToString();
            string[] listaParticipantes = participantesStr.Split(new string[] { separador }, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder sb = new StringBuilder();

            foreach (string participante in listaParticipantes)
            {
                string nome = participante.Trim();
                if (!string.IsNullOrEmpty(nome))
                {
                    sb.AppendFormat("<span class='label label-info' style='margin-right: 5px;'>{0}</span>", nome);
                }
            }

            return sb.ToString();
        }
        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            string sErro = "";
            string idDespesas = "";
            if (hddidCentrodeCusto.Value != "")
                ddlCentroCusto.SelectedValue = hddidCentrodeCusto.Value;
            if (Validar())
            {
                DataSet dsGravar;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "SALVAR");
                vParametros.Add("@sDscMotivo", txtsDscMotivo.Text);
                vParametros.Add("@idPedido", ddlCentroCusto.SelectedValue);
                vParametros.Add("@sDscObservacao", txtsObservacao.Text);
                vParametros.Add("@idTipo", "2");
                if (hddidDespesas.Value != "0")
                {
                    vParametros.Add("@idDespesas", hddidDespesas.Value);

                }
                vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());
                vParametros.Add("@idStatus", "1");


                dsGravar = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsGravar, out sErro))
                {
                    if (hddidDespesas.Value == "0")
                    {
                        idDespesas = RETORNO.DATASET(dsGravar, 0, 0, "idDespesas");
                        SalvarDespesas(idDespesas);
                        Pesquisar(idDespesas);

                    }
                    else
                    {
                        SalvarDespesas(hddidDespesas.Value);
                        Pesquisar(hddidDespesas.Value);
                    }
                    MensagemPagina_Entregas.MostraMensagem_Sucesso("Relatório Salvo Com Sucesso!");
                    if (hddidDespesas.Value == "0")
                    {
                        hddidDespesas.Value = idDespesas;
                    }
                    //FUNCOES.DirecionaPagina("App/Paginas/Manutencao/Relatorio_Despesas_Detalhe.aspx?id=" + hddidDespesas.Value + "&sMsg=1");
                }
                else
                {
                    MensagemPagina_Entregas.MostraMensagem_Erro("Erro ao Salvar o Relatório!");
                }
            }
        }

        void Finalizar()
        {
            string sErro = "";
            if (Base_Despesas.Count() > 0)
            {
                try
                {
                    DataSet dsGravar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "Finalizar");
                    vParametros.Add("@idStatus", "2");
                    vParametros.Add("@idDespesas", hddidDespesas.Value); 
                    vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());

                    dsGravar = BD.ExecutarDataSet(sProcedure, vParametros);

                    Pesquisar(hddidDespesas.Value);
                }
                catch
                {
                    MensagemPagina_Entregas.MostraMensagem_Erro("Erro ao Finalizar Relatório");
                }
            }
            else
            {
                MensagemPagina_Entregas.MostraMensagem_Erro("Adicione itens a Despesa!");
            }
            ddlCentroCusto.SelectedValue = hddidCentrodeCusto.Value;
        }
        #endregion

        #region | Configurações de Campos
        void PopulaCombo()
        {
            PopulaListBox();

            FUNCOES.Popula_Combo(ddlCentroCusto, "sp_Select 'Flow_Adm_CentroDeCusto', @idPesquisa=1, @sPesquisa=C", "idCentroDeCusto", "sDescricao", false, "Selecione um Centro de Custo", "0");
            FUNCOES.Popula_Combo(ddlidCategoria, "sp_Select 'Produtos_Flow', @sPesquisa=S", "idItem", "sCodProd", false, "Selecione o Recurso", "0");
            FUNCOES.Popula_Combo(ddlTipoDespesa, "sp_Manipula_tbl_Flow_Colaboradores_Relatorio_Despesas 'FLOW-TIPOS'", "idTipoGastos", "sDscGasto", false, "Selecione o Tipo", "0");
        }

        void PopulaListBox()
        {
            FUNCOES.Popula_Combo(ddlsParticipantes, $"sp_Select 'Flow_Colaboradores-REL', @idFiltro={IDENTITY.Variaveis.idUsuario()}", "idColaborador", "sDscColaborador", false);
            DataTable dsPesquisa;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sTabela", "Flow_Colaboradores-REL");
            vParametros.Add("@idFiltro", IDENTITY.Variaveis.idUsuario());

            dsPesquisa = BD.ExecutarDataTable("sp_Select", vParametros);
            chkParticipantes.DataSource = dsPesquisa;
            chkParticipantes.DataTextField = "sDscColaborador";
            chkParticipantes.DataValueField = "idColaborador";
            chkParticipantes.DataBind();
        }

        void LimparDespesas()
        {
            ddlidCategoria.SelectedValue = "0";
            ddlidFormaPagamento.SelectedValue = "0";
            ddlTipoDespesa.SelectedValue = "0";
            txtdtDespesas.Text = ConverterDateType(DateTime.Now);
            txtnValor.Text = "";
            txtsLocal.Text = "";

            PopulaListBox();

            chkParticipantes.DataBind();
            DIV_Valor.Visible = false;
            DIV_Local.Visible = false;
            DIV_Participantes.Visible = false;
            div4.Visible = false;
        }
        #endregion

        #region | Script
        void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Sys.Application.add_load(function() {");
            sb.Append("$v192('[id*=txtnValor]').mask('0.000.000.009,99', { reverse: true });");

            sb.Append("$v192('#dialog-Finalizar').dialog({");
            sb.Append("    resizable: false,");
            sb.Append("    height: 'auto',");
            sb.Append("    width: 400,");
            sb.Append("    modal: true,");
            sb.Append("    autoOpen: false,");
            sb.Append("    buttons: {");
            sb.Append("        'Sim': function() {");
            sb.Append("            __doPostBack('funcao_Finalizar', '');");
            sb.Append("          $v192(this).dialog(\"close\");");
            sb.Append("        },");
            sb.Append("        'Não': function() {");
            sb.Append("     $v192(this).dialog(\"close\");");
            sb.Append("        }");
            sb.Append("    }");
            sb.Append("});");

            sb.Append("$('[id*=btnFinalizar]').click(function(e) {");
            sb.Append("    e.preventDefault();");
            sb.Append("    $v192('#dialog-Finalizar').dialog('open');");
            sb.Append("});");

            sb.Append("$('[id*=lnkEnvio_UpLoad]').click(function(e) {");
            sb.Append("    e.preventDefault();");
            sb.Append("    $('[id*=hddidItens]').val($(this).closest('tr').find('.idContador').text());");
            sb.Append("    $('#dialog-Arquivo').modal('show');");
            sb.Append("});");

            sb.Append("$('#btnAbreModalGasto').click(function(e) {");
            sb.Append("   e.preventDefault();");
            sb.Append("   $('#modalNovoGasto').modal('show');");
            sb.Append("});");

            sb.Append("});");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ScriptPagina", sb.ToString(), true);
        }
        #endregion

        #region | Despesas
        private bool Validar()
        {
            string sMensagem = "";
            if (ddlCentroCusto.SelectedValue == "0")
            {
                sMensagem += "Selecione o Centro de Custo!";
            }
            //if (ddlsTipoCompra.SelectedValue == "0")
            //    sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Tipo!";
            if (txtsDscMotivo.Text == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Descreva o Motivo!";
            }

            if (Base_Despesas.Count < 0 && Base_Despesas == null)
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Adicione pelo menos um Gasto Para Continuar!";
            }

            if (sMensagem != "")
            {
                MensagemPagina_Entregas.MostraMensagem_Erro(sMensagem);
                return false;
            }
            return true;
        }

        protected void btnInserir_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidarDespesas())
                {
                    FrameWork.cls_Despesas objItem = new FrameWork.cls_Despesas();

                    string sidParticipantes = string.Empty;
                    string sParticipantes = string.Empty;

                    if (Request.Browser.IsMobileDevice)
                    {
                        foreach (ListItem item in chkParticipantes.Items)
                        {
                            if (item.Selected)
                            {
                                if (!string.IsNullOrEmpty(sidParticipantes))
                                {
                                    sidParticipantes += ",";
                                    sParticipantes += ", ";
                                }
                                sidParticipantes += item.Value;
                                sParticipantes += item.Text;
                            }
                        }
                    }
                    else
                    {
                        foreach (ListItem item in ddlsParticipantes.Items)
                        {
                            if (item.Selected)
                            {
                                if (!string.IsNullOrEmpty(sidParticipantes))
                                {
                                    sidParticipantes += ",";
                                    sParticipantes += ", ";
                                }
                                sidParticipantes += item.Value;
                                sParticipantes += item.Text;
                            }
                        }
                    }

                    objItem.idCategoriaPagar = int.Parse(ddlidCategoria.SelectedValue);
                    objItem.sDscCategoriaPagar = ddlidCategoria.SelectedItem.ToString();
                    objItem.sDscFormaPagamento = ddlidFormaPagamento.SelectedItem.ToString();
                    objItem.idFormaPagamento = int.Parse(ddlidFormaPagamento.SelectedValue);
                    DateTime dt = DateTime.Parse(txtdtDespesas.Text);
                    objItem.dtDespesa = dt.ToString("dd/MM/yyyy HH:mm");
                    objItem.nValor = decimal.Parse(txtnValor.Text);
                    objItem.sFuncao = "INSERIR_DESPESAS";
                    objItem.sDscObservacao = "";
                    objItem.sLocal = txtsLocal.Text;
                    objItem.idContador = Base_Despesas.Count + 1;
                    objItem.sParticipantes = sParticipantes;
                    objItem.sidParticipantes = sidParticipantes;

                    objItem.idTipoDespesa = int.Parse(ddlTipoDespesa.SelectedValue);

                    converterNomeTipoGasto(objItem);

                    if (ImportarArquivo.HasFiles)
                    {
                        int numeroDeArquivos = ImportarArquivo.PostedFiles.Count;
                        if (numeroDeArquivos < 3)
                        {
                            int arquivo = 0;
                            eBloco bloco = (eBloco)Enum.Parse(typeof(eBloco), "Despesas");
                            foreach (HttpPostedFile postedFile in ImportarArquivo.PostedFiles)
                            {
                                arquivo += 1;

                                TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                                Byte[] lObjArquivo = objArquivo.TransformaArquivoEmArrayBytes(postedFile.FileName, postedFile.InputStream);

                                if (arquivo == 1)
                                {
                                    switch (bloco)
                                    {
                                        case eBloco.Despesas:

                                            objItem.idArquivo = 0;
                                            objItem.sNomeArquivo = postedFile.FileName;
                                            objItem.objArquivo = lObjArquivo;
                                            objItem.sFuncao = "INSERIR_DESPESAS";
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (bloco)
                                    {
                                        case eBloco.Despesas:

                                            objItem.idArquivo2 = 0;
                                            objItem.sNomeArquivo2 = postedFile.FileName;
                                            objItem.objArquivo2 = lObjArquivo;
                                            objItem.sFuncao = "INSERIR_DESPESAS";
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            MensagemPagina_Entregas.MostraMensagem_Erro("Permitido Selecionar Apenas 2 Arquivos.");
                        }
                    }
                    Base_Despesas.Add(objItem);

                    DataBind_DocumentosSTSO();
                    LimparDespesas();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_AbreModalGasto", "$('#modalNovoGasto').modal('show');", true);
                }
            }
            catch (Exception ex)
            {
                //Thiago - Rodrigues 06/11/2025

                //MensagemPagina_Entregas.MostraMensagem_Erro(ex.Message);
                MensagemPagina_Modal.MostraMensagem_Erro(ex.Message);

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_AbreModalGasto", "$('#modalNovoGasto').modal('show');", true);
            }
        }

        void DataBind_DocumentosSTSO()
        {
            gv_Despesas.DataSource = Base_Despesas.Where(c => c.sFuncao.ToString() != "EXCLUIR_DESPESAS").OrderBy(c => c.sDscCategoriaPagar);
            gv_Despesas.DataBind();
            //div2.Visible = true;
        }

        protected void gv_Despesas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int nColunaBotoes = e.Row.Cells.Count - 2;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sNomeArquivo = (e.Row.FindControl("lnkEnvio_Download") as LinkButton).ToolTip;

                foreach (LinkButton lnk in e.Row.Cells[nColunaBotoes].Controls.OfType<LinkButton>())
                {
                    if ((lnk.ID == "lnkEnvio_Download" && sNomeArquivo == "") || (lnk.ID == "lnkEnvio_UpLoad" && sNomeArquivo != ""))
                    {
                        lnk.Visible = false;
                    }
                    if ((lnk.ID == "lnkEnvio_UpLoad" && sNomeArquivo == "Download do Arquivo: "))
                    {
                        lnk.Visible = true;
                    }
                    else if ((lnk.ID == "lnkEnvio_Download" && sNomeArquivo == "Download do Arquivo: "))
                    {
                        lnk.Visible = false;
                    }
                }

                int nColunaBotao = e.Row.Cells.Count - 1;
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string sNomeArquivos = (e.Row.FindControl("Envio_Download") as LinkButton).ToolTip;

                    foreach (LinkButton lnk in e.Row.Cells[nColunaBotao].Controls.OfType<LinkButton>())
                    {
                        if (sNomeArquivos == "Download do Arquivo: ")
                        {
                            lnk.Visible = false;
                        }
                    }
                }

                decimal valorDecimal = decimal.Parse(e.Row.Cells[7].Text);
                e.Row.Cells[7].Text = valorDecimal.ToString("N2", new CultureInfo("pt-BR"));
            }
        }

        private bool ValidarDespesas()
        {
            string sMensagem = "";
            if (ddlidCategoria.SelectedValue == "0")
            {
                sMensagem += "Selecione a Categoria!";
            }
            if (DIV_Pagamento.Visible == false && ddlidFormaPagamento.SelectedValue == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Para selecionar a forma de pagamento, primeiro escolha o tipo!.";
            }
            if (ddlidFormaPagamento.SelectedValue == "0" || ddlidFormaPagamento.SelectedValue == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione a Forma de Pagamento!";
            }
            if (txtdtDespesas.Text == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Descreva a Data!";
            }
            if (txtnValor.Text == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Descreva o Valor!";
            }
            if (txtsLocal.Text == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Descreva o Local!";
            }
            if (Request.Browser.IsMobileDevice)
            {
                bool msg = false;
                foreach (ListItem item in chkParticipantes.Items)
                {
                    if (item.Selected)
                        msg = true;
                }
                if (msg == false)
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Participante!";
            }
            else
            {
                if (ddlsParticipantes.SelectedValue == "")
                {
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Participante!";
                }
            }

            if (!ImportarArquivo.HasFiles)
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Arquivo!";

            if (sMensagem != "")
            {
                //MensagemPagina_Entregas.MostraMensagem_Erro(sMensagem);
                MensagemPagina_Modal.MostraMensagem_Erro(sMensagem);
                return false;
            }
            return true;
        }

        void SalvarDespesas(string idDespesas)
        {
            foreach (var Linha in Base_Despesas)
            {
                int idArquivo = Linha.idArquivo;
                int idItens = 0;

                if (Linha.sFuncao != "CONSULTAR")
                {
                    DataSet dsGravar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", Linha.sFuncao);
                    vParametros.Add("@idDespesas", idDespesas);
                    vParametros.Add("@dtDespesa", Convert.ToDateTime(Linha.dtDespesa).ToString());
                    vParametros.Add("@nValor", BD.Conversoes.Numerico(Linha.nValor));
                    vParametros.Add("@idFormaPagamento", Linha.idFormaPagamento.ToString());
                    vParametros.Add("@idCategoriaPagar", Linha.idCategoriaPagar.ToString());
                    vParametros.Add("@idItens", Linha.idItens.ToString());
                    vParametros.Add("@sLocal", Linha.sLocal.ToString());
                    vParametros.Add("@idArquivo", Linha.idArquivo.ToString());
                    vParametros.Add("@sidParticipantes", Linha.sidParticipantes.ToString());
                    vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());

                    vParametros.Add("@idTipoGastos", Linha.idTipoDespesa.ToString());

                    dsGravar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (Linha.sFuncao != "EXCLUIR_DESPESAS")
                    {
                        idItens = int.Parse(RETORNO.DATASET(dsGravar, 0, 0, "idItens"));

                        if (Linha.sNomeArquivo != "" && Linha.sNomeArquivo != null)
                        {
                            TT_Flow.FrameWork.cls_Arquivos Arquivo = new FrameWork.cls_Arquivos();
                            Arquivo.idTipoArquivo = 8888;
                            Arquivo.idObjeto = idItens;
                            Arquivo.sNomeArquivo = Linha.sNomeArquivo;
                            Arquivo.sDscArquivo = Linha.sObservacaoArquivo;
                            Arquivo.sObservacao = "";
                            Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                            Arquivo.vbArquivo = Linha.objArquivo;
                            Arquivo.dtExpiracaoDoc = "";
                            Linha.idArquivo = Convert.ToInt32(BD.Retorno.DATASET(Arquivo.EnviarArquivo(Arquivo), "idArquivo"));
                        }
                        if (Linha.sNomeArquivo2 != null && Linha.sNomeArquivo2 != "")
                        {
                            TT_Flow.FrameWork.cls_Arquivos Arquivo = new FrameWork.cls_Arquivos();
                            Arquivo.idTipoArquivo = 8888;
                            Arquivo.idObjeto = idItens;
                            Arquivo.sNomeArquivo = Linha.sNomeArquivo2;
                            Arquivo.sDscArquivo = Linha.sObservacaoArquivo2;
                            Arquivo.sObservacao = "";
                            Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                            Arquivo.vbArquivo = Linha.objArquivo2;
                            Arquivo.dtExpiracaoDoc = "";
                            Linha.idArquivo2 = Convert.ToInt32(BD.Retorno.DATASET(Arquivo.EnviarArquivo(Arquivo), "idArquivo"));
                        }
                    }
                }
            }
        }

        void PopularTiposDespesas()
        {
            var vParametros = new Dictionary<string, string> { { "@sFuncao", "FLOW-TIPOS" } };

            Base_TipoDespesa = BD.ExecutarLista<Cls_TipoDespesa>("sp_Manipula_tbl_Flow_Colaboradores_Relatorio_Despesas", vParametros, false);
        }

        void PopularDespesas(DataSet dsPesquisa)
        {
            if (dsPesquisa.Tables.Count > 2 && dsPesquisa.Tables[2] != null && dsPesquisa.Tables[2].Rows.Count > 0)
            {
                DIV_Grid_Gastos.Visible = true;
            }
            else
            {
                DIV_Grid_Gastos.Visible = false;
            }

            PopularTiposDespesas();

            Base_Despesas.Clear();
            foreach (DataRow row in dsPesquisa.Tables[2].Rows)
            {
                FrameWork.cls_Despesas objItem = new FrameWork.cls_Despesas();

                objItem.sFuncao = "CONSULTAR";
                objItem.idCategoriaPagar = int.Parse(row["idCategoriaPagar"].ToString());
                objItem.sDscCategoriaPagar = row["sDscGasto"].ToString();
                objItem.sDscFormaPagamento = row["sDscFormaPagamento"].ToString();
                objItem.idFormaPagamento = int.Parse(row["idFormaPagamento"].ToString());
                objItem.dtDespesa = row["dtDespesa"].ToString();
                objItem.nValor = decimal.Parse(row["nValor"].ToString());
                objItem.sDscObservacao = "";
                objItem.idItens = int.Parse(row["idItens"].ToString());
                objItem.sLocal = row["sLocal"].ToString();
                objItem.idContador = Base_Despesas.Count + 1;
                objItem.sidArquivo = row["sidArquivo"].ToString();
                objItem.sNomeArquivo = row["sNomeArquivo"].ToString();
                objItem.sParticipantes = row["sParticipantes"].ToString();
                objItem.sidParticipantes = row["sidParticipantes"].ToString();

                objItem.idTipoDespesa = int.Parse(row["idTipoDespesa"].ToString());


                converterNomeTipoGasto(objItem);


                Base_Despesas.Add(objItem);
            }
            DataBind_DocumentosSTSO();
        }

        protected void converterNomeTipoGasto(cls_Despesas objItem)
        {
            var tipoDespesa = Base_TipoDespesa.FirstOrDefault(x => x.idTipoGastos == objItem.idTipoDespesa);

            if (tipoDespesa != null)
            {
                objItem.sDscTipoDespesa = tipoDespesa.sDscGasto;
            }
            else
            {
                objItem.sDscTipoDespesa = "Sem Tipo";
            }
        }

        protected void gv_Despesas_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (hddidCentrodeCusto.Value != "")
                ddlCentroCusto.SelectedValue = hddidCentrodeCusto.Value;
            int idItens = Convert.ToInt32(e.Keys[1].ToString());
            Base_Despesas[Base_Despesas.FindIndex(x => x.idContador.Equals(idItens))].sFuncao = "EXCLUIR_DESPESAS";
            DataBind_DocumentosSTSO();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            bool bPDF = true;
            if (hddidCentrodeCusto.Value != "")
                ddlCentroCusto.SelectedValue = hddidCentrodeCusto.Value;
            try
            {
                ReportViewer rv = new ReportViewer();

                rv.ProcessingMode = ProcessingMode.Local;
                rv.LocalReport.EnableExternalImages = true;
                rv.LocalReport.EnableHyperlinks = true;
                rv.LocalReport.ReportPath = "App\\Reports\\" + "Relatorio_Despesas.rdlc";

                Dictionary<string, string> vParametrosProduct = new Dictionary<string, string>();
                vParametrosProduct.Add("@idDespesas", hddidDespesas.Value);
                DataSet dtProduct1;
                dtProduct1 = BD.ExecutarDataSet("sp_Manipula_tbl_Relatorio_Despesas_PDF", vParametrosProduct);

                DateTime dtInclusao = DateTime.Parse(dtProduct1.Tables[0].Rows[0]["dtInclusao"].ToString());
                DateTime dtFinalizado = DateTime.Parse(dtProduct1.Tables[0].Rows[0]["dtFinalizado"].ToString());
                TimeSpan diferenca = dtFinalizado - dtInclusao;
                int diasDeDiferenca = (int)Math.Ceiling(diferenca.TotalDays);
                var Ano = dtInclusao.Year.ToString();
                var DataMes = dtInclusao.Month.ToString("D2");
                var Mes = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dtInclusao.ToString("MMMM", new CultureInfo("pt-BR")));
                decimal Total = 0;
                decimal vlrPessoa = 0;
                decimal vlr = 0;
                decimal Saldo = 0;
                string Motivo = dtProduct1.Tables[0].Rows[0]["sDscMotivo"].ToString();
                string idDespesa = dtProduct1.Tables[0].Rows[0]["idDespesas"].ToString();
                string sDscMotivo = dtProduct1.Tables[0].Rows[0]["sDscMotivo"].ToString();
                int QuantidadePessoas = 0;
                decimal vlrTotal = 0;
                string sNomeArquivo = "Relatorio_Despesa" + FUNCOES.CarimboDataHora() + ".pdf";

                foreach (DataRow row in dtProduct1.Tables[1].Rows)
                {
                    Total += decimal.Parse(row["nValor"].ToString());

                    if (row["sDscCategoriaPagar"].ToString() != "")
                    {
                        vlrPessoa = decimal.Parse(row["vlrMaximo"].ToString());
                        QuantidadePessoas = int.Parse(row["Quantidade"].ToString());
                        vlr = vlrPessoa * QuantidadePessoas;
                        vlrTotal += vlr;
                    }

                    if (dtProduct1.Tables[1].Rows.IndexOf(row) == 0 || dtProduct1.Tables[1].Rows[dtProduct1.Tables[1].Rows.IndexOf(row) - 1].Field<int>("idCategoriaPagar") != row.Field<int>("idCategoriaPagar"))
                        row.SetField<string>(dtProduct1.Tables[1].Columns.Count - 4, "1");

                    if (row["Arquivo"].ToString() != "")
                    {
                        string Arquivos = row["Arquivo"].ToString();
                        string idItens = row["idItens"].ToString();

                        row.SetField<string>(dtProduct1.Tables[1].Columns.Count - 3, HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + string.Format("/Aplicativo/Download.aspx?idArquivo={0}&idItens={1}&sPage={2}", Arquivos, idItens, sNomeArquivo));
                    }
                }

                if (diasDeDiferenca != 0)
                    vlrTotal = vlrTotal * diasDeDiferenca;

                Saldo = vlrTotal - Total;

                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtProduct1.Tables[0]));
                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", dtProduct1.Tables[1]));

                ReportParameter[] rp = new ReportParameter[8];

                rp[0] = new ReportParameter("Ano", Ano.ToString());
                rp[1] = new ReportParameter("Mes", Mes.ToString());
                rp[2] = new ReportParameter("Motivo", Motivo.ToString());
                rp[3] = new ReportParameter("Total", Total.ToString("N2"));
                rp[4] = new ReportParameter("idDespesa", idDespesa.ToString());
                rp[5] = new ReportParameter("vlrPessoa", vlrTotal.ToString("N2"));
                rp[6] = new ReportParameter("Saldo", Saldo.ToString("N2"));
                rp[7] = new ReportParameter("sDscMotivo", sDscMotivo.ToString());

                rv.LocalReport.SetParameters(rp);
                rv.LocalReport.Refresh();

                byte[] bytesProforma = bPDF ? rv.LocalReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings) : rv.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);


                File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivo, bytesProforma);
                FUNCOES.DownloadArquivo(Page, sNomeArquivo);
            }
            catch (Exception ex)
            {
                MensagemPagina_Entregas.MostraMensagem_Erro("Erro ao gerar Relatório! </br>" + ex.Message + (ex.InnerException == null ? "" : "<br />" + ex.InnerException.Message));
            }
        }

        protected void gv_Despesas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Upload_Arquivo")
            {

            }
            else if (e.CommandName == "Download_Arquivo")
            {
                GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                string idContador = gv_Despesas.DataKeys[row.RowIndex].Values["idContador"].ToString();

                DownloadArquivo(e.CommandArgument.ToString(), idContador);
            }
        }

        private enum eBloco
        {
            Despesas = 1
        }

        void DownloadArquivo(string idItens, string idContador)
        {
            string sNomeArquivo = "";
            byte[] bObjArquivo = null;
            string urlAtualPagina = Request.UrlReferrer.ToString().Replace(Request.RawUrl, "/Download/");
            if (idItens != "")
            {
                Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();
                DataTable dtArquivo;
                vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao",                  "CONSULTAR_ARQUIVOS" },
                        {"@idItens",                  idItens.ToString()}

                    };

                dtArquivo = BD.ExecutarDataTable(sProcedure, vParametrosItem);
                if (dtArquivo.Rows.Count > 0)
                {
                    foreach (DataRow item in dtArquivo.Rows)
                    {
                        try
                        {
                            sNomeArquivo = item["sNomeArquivo"].ToString();
                            bObjArquivo = (byte[])item["vbArquivo"];
                        }
                        catch
                        {

                        }

                        //Gera o Arquivo
                        TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                        FileStream lObjFile = objArquivo.TransformarArrayBytesEmArquivo(bObjArquivo, Server.MapPath("~/Download/" + sNomeArquivo));
                        lObjFile.Close();
                        lObjFile.Dispose();

                        FUNCOES.DownloadArquivo(Page, sNomeArquivo);
                    }
                }
                if (dtArquivo.Rows.Count == 0)
                {
                    string sNomeArquivo2 = "";
                    byte[] bObjArquivo2 = null;
                    int idArquivo = 0;
                    int idArquivo2 = 0;
                    var resultado = Base_Despesas.FirstOrDefault(x => x.idContador.Equals(int.Parse(idContador)));

                    if (resultado != null)
                    {
                        sNomeArquivo = resultado.sNomeArquivo;
                        bObjArquivo = resultado.objArquivo;
                        idArquivo = resultado.idArquivo;

                        if (resultado.sNomeArquivo2 != null)
                        {
                            sNomeArquivo2 = resultado.sNomeArquivo2;
                            bObjArquivo2 = resultado.objArquivo2;
                            idArquivo2 = resultado.idArquivo2;
                        }
                    }

                    TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                    FileStream lObjFile = objArquivo.TransformarArrayBytesEmArquivo(bObjArquivo, Server.MapPath("~/Download/" + sNomeArquivo));
                    lObjFile.Close();
                    lObjFile.Dispose();

                    FUNCOES.DownloadArquivo(Page, sNomeArquivo);

                    if (sNomeArquivo2 != "")
                    {
                        TT.FrameWork.Arquivo objArquivo2 = new TT.FrameWork.Arquivo();
                        FileStream lObjFile2 = objArquivo2.TransformarArrayBytesEmArquivo(bObjArquivo2, Server.MapPath("~/Download/" + sNomeArquivo2));
                        lObjFile.Close();
                        lObjFile.Dispose();

                        FUNCOES.DownloadArquivo(Page, sNomeArquivo2);
                    }
                }
            }
            else
            {

            }
        }

        protected void cmdAprovar_Click(object sender, EventArgs e)
        {
            string sErro = "";
            try
            {
                DataSet dsGravar;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "Finalizar");
                vParametros.Add("@idStatus", "3");
                vParametros.Add("@idDespesas", hddidDespesas.Value);
                vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());

                dsGravar = BD.ExecutarDataSet(sProcedure, vParametros);

                Pesquisar(hddidDespesas.Value);
            }
            catch
            {
                MensagemPagina_Entregas.MostraMensagem_Erro("Erro ao Finalizar Relatório");
            }
        }

        protected void cmdRejeitar_Click(object sender, EventArgs e)
        {
            string sErro = "";
            try
            {
                DataSet dsGravar;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "Finalizar");
                vParametros.Add("@idStatus", "4");
                vParametros.Add("@idDespesas", hddidDespesas.Value);
                vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());

                dsGravar = BD.ExecutarDataSet(sProcedure, vParametros);

                Pesquisar(hddidDespesas.Value);
            }
            catch
            {
                MensagemPagina_Entregas.MostraMensagem_Erro("Erro ao Finalizar Relatório");
            }
        }

        protected void ddlidCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hddidCentrodeCusto.Value != "")
                ddlCentroCusto.SelectedValue = hddidCentrodeCusto.Value;
            int Participantes = 0;
            foreach (ListItem item in ddlsParticipantes.Items)
            {
                if (item.Selected)
                {
                    Participantes++;
                }
            }
            if (ddlidCategoria.SelectedValue != "0")
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_VLRMAXIMO" },
                        { "@idTipoGastos", ddlidCategoria.SelectedValue }
                    };
                DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                decimal valorMaximo = 0;
                if (Participantes != 0)
                    valorMaximo = decimal.Parse(RETORNO.DATASET(dsPesquisa, 0, "vlrMaximo")) * Participantes;
                else
                    valorMaximo = decimal.Parse(RETORNO.DATASET(dsPesquisa, 0, "vlrMaximo"));

                vlrMaximo = valorMaximo;
                lblValor.InnerText = $"Valor (Sugerido R${valorMaximo})";

                DIV_Local.Visible = true;
                DIV_Participantes.Visible = true;
                div4.Visible = true;
            }
            else
            {
                lblValor.InnerText = "Valor";
                txtnValor.Text = "";
                DIV_Valor.Visible = false;
                DIV_Local.Visible = false;
                DIV_Participantes.Visible = false;
                div4.Visible = false;
            }
        }

        protected void ddlTipoDespesa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hddidCentrodeCusto.Value != "")
                ddlCentroCusto.SelectedValue = hddidCentrodeCusto.Value;
            int Participantes = 0;
            foreach (ListItem item in ddlsParticipantes.Items)
            {
                if (item.Selected)
                {
                    Participantes++;
                }
            }

            DataSet dsRecurso = new DataSet();

            if (ddlTipoDespesa.SelectedValue != "0" || !string.IsNullOrEmpty(ddlTipoDespesa.SelectedValue))
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_RECURSO" },
                        { "@idTipoGastos", ddlTipoDespesa.SelectedValue }
                    };

                dsRecurso = BD.ExecutarDataSet(sProcedure, vParametros);
            }


            if (RETORNO.DATASET(dsRecurso, "idRecurso") != "0" && !string.IsNullOrEmpty(RETORNO.DATASET(dsRecurso, "idRecurso")))
            {
                ddlidCategoria.SelectedValue = RETORNO.DATASET(dsRecurso, "idRecurso");

                decimal valorMaximo = 0;
                if (Participantes != 0)
                    valorMaximo = decimal.Parse(RETORNO.DATASET(dsRecurso, 0, "vlrMaximo")) * Participantes;
                else
                    valorMaximo = decimal.Parse(RETORNO.DATASET(dsRecurso, 0, "vlrMaximo"));

                vlrMaximo = valorMaximo;
                lblValor.InnerText = $"Valor (Max. Sugerido R${valorMaximo})";

                DIV_Local.Visible = true;
                DIV_Participantes.Visible = true;
                div4.Visible = true;
                DIV_Pagamento.Visible = true;
            }
            else
            {
                lblValor.InnerText = "Valor";
                txtnValor.Text = "";
                DIV_Valor.Visible = false;
                DIV_Local.Visible = false;
                DIV_Participantes.Visible = false;
                div4.Visible = false;
            }
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_AbreModalGasto", "$('#modalNovoGasto').modal('show');", true);
        }

        protected void ddlsParticipantes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hddidCentrodeCusto.Value != "")
                ddlCentroCusto.SelectedValue = hddidCentrodeCusto.Value;
            int Participantes = 0;
            foreach (ListItem item in ddlsParticipantes.Items)
            {
                if (item.Selected)
                {
                    Participantes++;
                }
            }
            decimal Maximo = vlrMaximo * Participantes;
            lblValor.InnerText = $"Valor (Max. Sugerido R${Maximo})";
            DIV_Valor.Visible = true;

            AbrirModal();
        }
        #endregion

        protected void chkParticipantes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hddidCentrodeCusto.Value != "")
                ddlCentroCusto.SelectedValue = hddidCentrodeCusto.Value;
            int Participantes = 0;
            foreach (ListItem item in chkParticipantes.Items)
            {
                if (item.Selected)
                {
                    Participantes++;
                }
            }
            decimal Maximo = vlrMaximo * Participantes;
            lblValor.InnerText = $"Valor (Max. Sugerido R${Maximo})";
            DIV_Valor.Visible = true;
            //FUNCOES.Scripts.FocusScript(Page, chkParticipantes.ClientID);
            DIV1.Focus();

            AbrirModal();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "Finalizar" },
                    { "@idStatus", "5" },
                    { "@idDespesas", hddidDespesas.Value },
                    { "@idUsuario", IDENTITY.Variaveis.idUsuario() }
                };

                DataSet dsGravar = BD.ExecutarDataSet(sProcedure, vParametros);

                Pesquisar(hddidDespesas.Value);
            }
            catch
            {
                MensagemPagina_Entregas.MostraMensagem_Erro("Erro ao Excluir Relatório");
            }
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina("App/Paginas/Adm/Reembolso_Consulta.aspx");
        }

        private string ConverterDateType(DateTime dt)
        {
            return dt.ToString("yyyy-MM-ddTHH:mm");
        }

        protected void btnFechar_Click(object sender, EventArgs e)
        {
            LimparDespesas();

            string script = @"
        $('#modalNovoGasto').one('hidden.bs.modal', function () {
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
        }).modal('hide');";

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_FechaModalGasto", script, true);
        }

        protected void AbrirModal()
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_AbreModalGasto", "$('#modalNovoGasto').modal('show');", true);
        }


        #region | Métodos do Histórico

        /// <summary>
        /// Busca o histórico na procedure e popula o Repeater
        /// </summary>
        void PopularHistorico(string idDespesas)
        {
            try
            {
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR-HISTORICO");
                vParametros.Add("@idDespesas", idDespesas);

                // Reutiliza sua classe de BD para buscar os dados
                DataSet dsHistorico = BD.ExecutarDataSet(sProcedure, vParametros);

                // A mágica acontece aqui:
                if (dsHistorico.Tables.Count > 0 && dsHistorico.Tables[0].Rows.Count > 0)
                {
                    pnlHistorico.Visible = true; // SÓ MOSTRA O PAINEL SE TIVER DADOS
                    rptHistorico.DataSource = dsHistorico.Tables[0];
                    rptHistorico.DataBind();
                }
                else
                {
                    pnlHistorico.Visible = false; // Garante que está oculto se não tiver dados
                }
            }
            catch (Exception ex)
            {
                // Não quebra a página se o histórico falhar, apenas oculta o painel
                pnlHistorico.Visible = false;
                System.Diagnostics.Debug.WriteLine("Erro ao carregar histórico: " + ex.Message);
            }
        }

        /// <summary>
        /// Método auxiliar para o Repeater definir a cor da bolinha da timeline
        /// </summary>
        protected string GetStatusClass(object sCor)
        {
            if (sCor == null || sCor == DBNull.Value) return "status-primary";
            return "status-" + sCor.ToString();
        }

        /// <summary>
        /// Método auxiliar para formatar o motivo (para o Literal)
        /// </summary>
        protected string GetMotivo(object sDscMotivo)
        {
            if (sDscMotivo == null || sDscMotivo == DBNull.Value) return "";

            string motivo = sDscMotivo.ToString();

            // Se o motivo *não* for o log de HTML, apenas o exibe.
            // Se for o log de HTML, já vem formatado.
            if (!motivo.StartsWith("<b>"))
            {
                // Formata como um parágrafo simples se for texto puro (ex: rejeição)
                return "<br/><p>" + HttpUtility.HtmlEncode(motivo) + "</p>";
            }

            // Se for HTML, retorna como está (confia na sua lógica da SP)
            return motivo;
        }

        #endregion

        private void ConfigurarParaReembolso()
        {
            // Força o tipo para Reembolso (2)
            ddlsTipoCompra.SelectedValue = "2";

            // Configura as formas de pagamento exclusivas de reembolso
            ddlidFormaPagamento.Items.Clear();
            ddlidFormaPagamento.Items.Add(new ListItem("Selecione a Forma de Pagamento", "0"));
            ddlidFormaPagamento.Items.Add(new ListItem("PIX", "999"));
            ddlidFormaPagamento.Items.Add(new ListItem("Transferência", "998"));
            ddlidFormaPagamento.Items.Add(new ListItem("Cartão crédito pessoal", "997"));
            ddlidFormaPagamento.Items.Add(new ListItem("Cartão debito pessoal", "996"));
            ddlidFormaPagamento.Items.Add(new ListItem("Dinheiro", "995"));

            // Exibe os painéis necessários
            DIV_Pagamento.Visible = true;
        }
    }

    [Serializable]
    public class Cls_TipoDespesa
    {
        public int idTipoGastos { get; set; }
        public string sDscGasto { get; set; }
    }
}