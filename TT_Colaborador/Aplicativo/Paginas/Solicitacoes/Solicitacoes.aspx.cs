using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using System.Globalization;
using System.Text;
using System.Web.UI.WebControls;
using static TT.FrameWork.Identity;
using System.Web.UI.HtmlControls;

namespace TT_Colaborador.Aplicativo.Paginas.Solicitacoes
{
    // CLASSES DE APOIO PARA ORGANIZAR OS DADOS
    public class HistoricoItem
    {
        public string Status { get; set; }
        public string Motivo { get; set; }
        public string Usuario { get; set; }
        public string DataAtualizacao { get; set; }
        public string Cor { get; set; }
    }

    public class MinhaSolicitacao
    {
        public int IdSolicitacao { get; set; }
        public string Tipo { get; set; }
        public string Descricao { get; set; }
        public string DataSolicitacao { get; set; }
        public string StatusAtual { get; set; }
        public string CorStatusAtual { get; set; }
        public bool PodeEditar { get; set; }
        public string Observacao { get; set; }
        public List<HistoricoItem> Historico { get; set; } = new List<HistoricoItem>();
    }
    public enum Status
    {
        EmAnalise = 1,
        AprovadoSupervisor = 2,
        Finalizado = 3,
        RejeitadoSupervisor = 4,
        Cancelado = 5,
        AprovadoRH = 6,
        RejeitadoRH = 7,
        PendenteDocumentacao = 8,
        AnaliseRH = 9,
        AprovadoDiretor = 10,
        RejeitadoDiretor = 11,
        AprovadoTerceiros = 12,
        RejeitadoTerceiros = 13,
        AguardandoNfe = 14,
        AnaliseTerceiros = 15
    }

    [Serializable]
    public class OpcaoDataItem
    {
        public DateTime DtInicio { get; set; }
        public DateTime DtFinal { get; set; }
    }
    public partial class Solicitacoes : Page
    {
        #region | Variáveis Globais

        string sProcedure = "sp_Manipula_tbl_Flow_Solicitacoes";
        string sistema = "TCOLABORADOR";
        bool edicao = false;
        int idStatusAlterar = 1;

        public List<OpcaoDataItem> ListaOpcoesDatas
        {
            get
            {
                if (ViewState["ListaOpcoesDatas"] == null)
                    return new List<OpcaoDataItem>();
                return (List<OpcaoDataItem>)ViewState["ListaOpcoesDatas"];
            }
            set
            {
                ViewState["ListaOpcoesDatas"] = value;
            }
        }

        #endregion

        #region | Eventos da Página

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Solicitações.Consultar, true, true);
            cmdNovaSolicitacao.Visible = FUNCOES.ValidaPermissao(Permissao.Solicitações.Incluir, false, true);

            if (!IsPostBack)
            {
                PopularCombos();

                string idUrl = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(idUrl))
                {
                    Pesquisar(idUrl);
                    ddlidTipo_SelectedIndexChanged(sender, e);
                }
                else if (hddidSolicitacao.Value != "0" && !string.IsNullOrEmpty(hddidSolicitacao.Value))
                {
                    Pesquisar(hddidSolicitacao.Value);
                    ddlidTipo_SelectedIndexChanged(sender, e);
                }
                else if (hddidSolicitacao.Value == "0")
                {

                }
                else
                {
                    Pesquisar();
                }   // Carrega a nova lista de solicitações
            }
            else
            {
                var requestTarget = this.Request["__EVENTTARGET"];

                if (requestTarget == "funcao_SAIR")
                    FUNCOES.DirecionaPagina("/Aplicativo/MenuColaborador.aspx");
                else if (requestTarget == "funcao_SALVAR")
                    Salvar();
                else if (requestTarget == "funcao_Editar")
                {
                    if (hddidSolicitacao.Value != "0" && !string.IsNullOrEmpty(hddidSolicitacao.Value))
                    {
                        Pesquisar(hddidSolicitacao.Value);
                        ddlidTipo_SelectedIndexChanged(sender, e);
                    }
                    else if (hddidSolicitacao.Value == "0")
                    {

                    }
                    else
                    {
                        Pesquisar();
                    }
                }

                if (hddidSolicitacao.Value != "0" && !string.IsNullOrEmpty(hddidSolicitacao.Value))
                {
                    Pesquisar(hddidSolicitacao.Value);
                    ddlidTipo_SelectedIndexChanged(sender, e);
                }
                else if (hddidSolicitacao.Value == "0")
                {

                }
                else
                {
                    Pesquisar();
                }
            }

            RegistraScript();
        }

        #endregion

        #region | Métodos de Banco de Dados

        protected void Pesquisar()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sSistema", sistema },
                { "@sDscSolicitacao", txtPesquisa.Text.Trim() },
                { "@idTipoSolicitacao", ddlidTipoSolicitacao.SelectedValue },
                { "@dtInicio", string.IsNullOrEmpty(txtdtInicial.Text.Trim()) ? "" : DateTime.Parse(txtdtInicial.Text.Trim()).ToString("dd/MM/yyyy") },
                { "@dtFinal", string.IsNullOrEmpty(txtdtFinal.Text.Trim()) ? "" : DateTime.Parse(txtdtFinal.Text.Trim()).ToString("dd/MM/yyyy")},
                { "@idUsuarioLogado", Variaveis.idUsuario() }
            };
            DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            div_dados.Visible = false;
            div_solicitacoes.Visible = true;
            lblTituloPagina.Text = "Minhas Solicitações";
            PainelAtualizacao.Visible = false;

            if (BD.ValidarDataSet(dsPesquisa, out string sErro))
            {
                var listaDeSolicitacoes = new List<MinhaSolicitacao>();

                foreach (DataRow row in dsPesquisa.Tables[0].Rows)
                {
                    var solicitacao = new MinhaSolicitacao
                    {
                        IdSolicitacao = Convert.ToInt32(row["idSolicitacao"]),
                        Tipo = row["sTipo"].ToString(),
                        Descricao = row["sDscSolicitacao"].ToString(),
                        DataSolicitacao = Convert.ToDateTime(row["dtSolicitacao"]).ToString("dd/MM/yyyy 'às' HH:mm"),
                        StatusAtual = row["sStatus"].ToString(),
                        CorStatusAtual = row["sCor"].ToString(),
                        PodeEditar = row["sLiberaColaborador"].ToString() == "S" && FUNCOES.ValidaPermissao(Permissao.Solicitações.Editar, false, true),
                        Observacao = row["sObservacao"].ToString()
                    };

                    Dictionary<string, string> vParamsHist = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_HISTORICO" },
                        { "@idSolicitacao", solicitacao.IdSolicitacao.ToString() }
                    };
                    DataSet dsHistorico = BD.ExecutarDataSet(sProcedure, vParamsHist);

                    if (BD.ValidarDataSet(dsHistorico, out _))
                    {
                        foreach (DataRow histRow in dsHistorico.Tables[0].Rows)
                        {
                            solicitacao.Historico.Add(new HistoricoItem
                            {
                                Status = histRow["sStatus"].ToString(),
                                Motivo = histRow["sMotivo"].ToString(),
                                Usuario = histRow["sUsuario"].ToString(),
                                DataAtualizacao = Convert.ToDateTime(histRow["dtAtualizacao"]).ToString("dd/MM/yyyy 'às' HH:mm"),
                                Cor = histRow["sCor"].ToString()
                            });
                        }
                    }
                    listaDeSolicitacoes.Add(solicitacao);
                }

                rptSolicitacoes.DataSource = listaDeSolicitacoes;
                rptSolicitacoes.DataBind();
            }
            else
            {
                rptSolicitacoes.DataSource = null;
                rptSolicitacoes.DataBind();
                MensagemPagina_Solicitacoes.MostraMensagem("Nenhuma solicitação encontrada para os filtros informados.", "info", false);
            }
        }

        void Pesquisar(string idSolicitacao)
        {
            PopularCombos();
            edicao = true;

            try
            {
                LimpaCampos();

                div_dados.Visible = true;
                div_solicitacoes.Visible = false;
                cmdSalvar.Visible = false;
                btnFechar.Visible = true;
                divFormArquivo.Visible = false;

                if (idSolicitacao != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR" },
                        { "@idSolicitacao", idSolicitacao },
                        { "@sSistema", sistema },
                        { "@idUsuarioLogado", Variaveis.idUsuario() }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        lblTituloPagina.Text = $"Detalhes da Solicitação: {RETORNO.DATASET(dsPesquisa, 0, "sDscSolicitacao")}";

                        txtsDscSolicitacao.Attributes.Add("disabled", "disabled");
                        ddlidTipo.Attributes.Add("disabled", "disabled");
                        txtsdtFinal.Attributes.Add("disabled", "disabled");
                        txtsdtInicio.Attributes.Add("disabled", "disabled");
                        //divFormArquivo.Visible = true;

                        hddidSolicitacao.Value = RETORNO.DATASET(dsPesquisa, 0, "idSolicitacao");
                        txtsDscSolicitacao.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscSolicitacao");
                        string statusId = RETORNO.DATASET(dsPesquisa, 0, "idStatus");
                        ddlidTipo.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idTipo");

                        //txtsObservacao.Text = RETORNO.DATASET(dsPesquisa, 0, "sObservacao");

                        string observacao = RETORNO.DATASET(dsPesquisa, 0, "sObservacao");
                        txtsObservacao.Text = Server.HtmlDecode(observacao);
                        txtsObservacao.Attributes.Add("readonly", "readonly");

                        hddidStatus.Value = statusId;

                        ConsultarOpcoesDatas(idSolicitacao);

                        lblsStatus.Text = RETORNO.DATASET(dsPesquisa, 0, "sStatus");
                        lblsStatus.CssClass = $"badge bg-{RETORNO.DATASET(dsPesquisa, 0, "sCor")}";

                        // --- ALTERAÇÃO 1: ESCONDER O BOTÃO DE ADICIONAR DATAS ---
                        btnOpenModalDatas.Visible = false;

                        //Lógica DivArquivos
                        switch (statusId)
                        {
                            case "1":  // Em Análise
                            case "4":  //Rejeitado Sup
                            case "14": //Aguardando NFE
                            case "8":  // Documento Pendente
                            case "7":  // Rejeitado RH
                            case "11":  // Rejeitado Diretor
                            case "13":  // Rejeitado Terceiros
                                divFormArquivo.Visible = true;
                                break;

                            default:
                                divFormArquivo.Visible = false;
                                break;
                        }


                        // --- LÓGICA DAS LABELS DINÂMICAS ---
                        hddsStatus.Value = "Desejada";
                        string labelStatus = hddsStatus.Value; // Padrão
                        switch (statusId)
                        {
                            case "2":
                            case "4": // Status do Supervisor
                                labelStatus = "Sugerida (Supervisor)";
                                hddsStatus.Value = labelStatus;
                                break;
                            case "6":
                                // Status do RH
                                labelStatus = "Aprovada (RH)";
                                hddsStatus.Value = labelStatus;
                                break;
                            case "7": // Status do RH
                                labelStatus = "Rejeitada (RH)";
                                hddsStatus.Value = labelStatus;
                                break;
                            case "8": // Status do RH
                                labelStatus = "Pendente Documentação";
                                hddsStatus.Value = labelStatus;
                                idStatusAlterar = 9; // Analise do RH
                                break;
                        }
                        lblDtInicial.InnerText = $"Data Inicial {labelStatus}";
                        lblDtFim.InnerText = $"Data Final {labelStatus}";
                        // --- FIM DA LÓGICA ---

                        if (RETORNO.DATASET(dsPesquisa, 0, "dtInicio").ToString() == "01/01/1900 00:00:00") txtsdtInicio.Text = null;
                        else
                        {
                            var dt = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtInicio").ToString());
                            txtsdtInicio.Text = dt.ToString(ddlidTipo.SelectedValue != "1" && ddlidTipo.SelectedValue != "3" ? "yyyy-MM-ddTHH:mm:ss" : "yyyy-MM-dd");
                        }

                        if (RETORNO.DATASET(dsPesquisa, 0, "dtFinal").ToString() == "01/01/1900 00:00:00") txtsdtFinal.Text = null;
                        else
                        {
                            var dt = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtFinal").ToString());
                            txtsdtFinal.Text = dt.ToString("yyyy-MM-dd");
                        }

                        if (RETORNO.DATASET(dsPesquisa, 0, "sFluxoTerceiro") == "S")
                        {
                            if (statusId == ((int)Status.EmAnalise).ToString())
                            {
                                frmArquivos.Attributes.Add("src", $"~/Aplicativo/Paginas/Arquivos.aspx?idObjeto={idSolicitacao}&sTipoObjeto=Solicitação&idStatus={statusId}&idUsuario={Variaveis.idUsuario()}&statusAtual={statusId}&sExibicao=false");
                            }
                            else
                            {
                                frmArquivos.Attributes.Add("src", $"~/Aplicativo/Paginas/Arquivos.aspx?idObjeto={idSolicitacao}&sTipoObjeto=Solicitação&idStatus={(int)Status.AnaliseTerceiros}&idUsuario={Variaveis.idUsuario()}&statusAtual={statusId}&sExibicao=false");
                            }
                        }
                        else
                        {
                            frmArquivos.Attributes.Add("src", $"~/Aplicativo/Paginas/Arquivos.aspx?idObjeto={idSolicitacao}&sTipoObjeto=Solicitação&idStatus={idStatusAlterar}&idUsuario={Variaveis.idUsuario()}&statusAtual={statusId}&sExibicao=false");
                        }

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                    }
                    else
                    {
                        MensagemPagina_Principal.MostraMensagem_Erro("Erro: " + sErro);
                        PainelAtualizacao.Visible = false;
                    }
                }
                else // Modo de "Nova Solicitação"
                {
                    hddsStatus.Value = "Desejada";
                    txtsDscSolicitacao.Attributes.Remove("disabled");
                    ddlidTipo.Attributes.Remove("disabled");
                    txtsdtFinal.Attributes.Remove("disabled");
                    txtsdtInicio.Attributes.Remove("disabled");
                    txtsObservacao.Attributes.Remove("readonly");
                    lblTituloPagina.Text = "Nova Solicitação";
                    ListItem itemRemover = ddlidTipo.Items.FindByValue("3");
                    if (itemRemover != null)
                    {
                        ddlidTipo.Items.Remove(itemRemover);
                    }

                    lblsStatus.Text = "Nova Solicitação";
                    lblsStatus.CssClass = $"badge bg-warning";

                    // --- ALTERAÇÃO 2: MOSTRAR O BOTÃO SE FOR NOVA SOLICITAÇÃO ---
                    btnOpenModalDatas.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_Principal.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }


        void Salvar()
        {
            if (ValidarDados())
            {
                try
                {
                    string[] vidSolicitacao = hddidSolicitacao.Value.Split(',');
                    string idSolicitacao = vidSolicitacao[0].ToString();

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idSolicitacao", idSolicitacao },
                        { "@sDscSolicitacao", txtsDscSolicitacao.Text },
                        { "@idTipo", ddlidTipo.SelectedValue },
                        { "@idStatus", "1" }, // Status inicial
                        { "@idUsuarioSolicitacao", Variaveis.idUsuario() },
                        { "@idUsuarioAtualizacao", Variaveis.idUsuario() },
                        { "@sObservacaoSolicitacao", txtsObservacao.Text }
                    };

                    if (!string.IsNullOrEmpty(txtsdtInicio.Text))
                        vParametros.Add("@dtInicio", DateTime.Parse(txtsdtInicio.Text).ToString());

                    if (ddlidTipo.SelectedValue == "1" && !string.IsNullOrEmpty(txtsdtFinal.Text))
                        vParametros.Add("@dtFinal", DateTime.Parse(txtsdtFinal.Text).ToString());

                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out string sErro))
                    {
                        AdicionaDatas(dsSalvar, idSolicitacao);

                        Pesquisar(); // Volta para a lista
                        MensagemPagina_Solicitacoes.MostraMensagem_Sucesso("Solicitação efetuada com sucesso!");
                    }
                    else
                        MensagemPagina_Principal.MostraMensagem_Erro("BD: " + sErro.ToString());
                }
                catch (Exception ex)
                {
                    MensagemPagina_Principal.MostraMensagem_Erro(ex.Message);
                }
            }
        }

        string ConverterData(string dt)
        {
            return DateTime.Parse(dt).ToString();
        }

        string ConverterDataRetorno(string date)
        {
            if (date == "01/01/1900 00:00:00") return null;
            else
            {
                var dt = Convert.ToDateTime(date);
                return dt.ToString("yyyy-MM-dd");
            }
        }
        #endregion

        #region | Métodos Auxiliares

        void LimpaCampos()
        {
            ddlidTipoSolicitacao.SelectedValue = "0";
            txtdtInicial.Text = "";
            txtdtFinal.Text = "";
            txtPesquisa.Text = "";
            ddlidTipo.SelectedValue = "0";
            txtsDscSolicitacao.Text = "";
            txtsdtFinal.Text = null;
            txtsdtInicio.Text = null;
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            hddidSolicitacao.Value = "0";

            txtsObservacao.Text = "";
            txtsObservacao.Attributes.Remove("readonly");
            txtsObservacao.ReadOnly = false;

            divFormulario_Solicitacao.Visible = false;

            ListaOpcoesDatas = new List<OpcaoDataItem>(); // Zera ViewState
            AtualizarRepeaterDatas();
            div_OpcoesDatas.Visible = false;
        }

        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (ddlidTipo.SelectedValue == "0")
            {
                sMensagemErro += "Tipo de Solicitação é obrigatório!<br/>";
            }
            else if (ddlidTipo.SelectedValue == "1") // Férias
            {
                if (string.IsNullOrEmpty(txtsDscSolicitacao.Text)) sMensagemErro += "Descrição é obrigatória!<br/>";
                if (txtsDscSolicitacao.Text.Length > 100) sMensagemErro += "Descrição excede o número de caracteres máximo!<br/>";
                if (string.IsNullOrEmpty(txtsdtFinal.Text) || string.IsNullOrEmpty(txtsdtInicio.Text)) sMensagemErro += "Datas são Obrigatórias!<br/>";
                if (string.IsNullOrEmpty(txtsObservacao.Text)) sMensagemErro += "Observação é obrigatória!<br/>";
                if (DateTime.TryParse(txtsdtInicio.Text, out DateTime dataInicio) && DateTime.TryParse(txtsdtFinal.Text, out DateTime dataFinal))
                {
                    if (dataInicio > dataFinal) sMensagemErro += "A data de início não pode ser posterior à data final.<br/>";
                    if (dataInicio.Date < DateTime.Now.Date) sMensagemErro += "A data de início não pode ser no passado.<br/>";
                }
            }
            else if (ddlidTipo.SelectedValue == "2") // Especial
            {
                if (txtsDscSolicitacao.Text.Length < 3) sMensagemErro = "A descrição deve possuir ao menos 3 caracteres!<br/>";
                if (!DateTime.TryParse(txtsdtInicio.Text, out _)) sMensagemErro = "A data informada é inválida.<br/>";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPagina_Principal.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlidTipo, "sp_Select 'Flow_Solicitacao_Tipo'", "idTipo", "sDscTipo", false, "Selecione um Tipo", "0");
            FUNCOES.Popula_Combo(ddlidTipoSolicitacao, "sp_Select 'Flow_Solicitacao_Tipo'", "idTipo", "sDscTipo", false, "Todos os Tipos", "0");
        }

        void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("$(document).ready(function() {");
            sb.Append(" $('#dialog-Salvar').dialog({ resizable: false, height: 'auto', width: 400, modal: true, autoOpen: false, buttons: { 'Sim': function() { __doPostBack('funcao_SALVAR', ''); $(this).dialog('close'); }, 'Não': function() { $(this).dialog('close'); } } });");
            sb.Append(" $('[id*=cmdSalvar]').click(function(e) { e.preventDefault(); $('#dialog-Salvar').dialog('open'); });");
            sb.Append("});");
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ScriptPagina_Modal", sb.ToString(), true);
        }

        #endregion

        #region | Eventos de Controles

        protected void cmdNovaSolicitacao_Click(object sender, EventArgs e) => Pesquisar("0");
        protected void fechar_click(object sender, EventArgs e) => Pesquisar();
        protected void AbrirSolicitacao_Click(object sender, EventArgs e)
        {
            Pesquisar((sender as LinkButton).CommandArgument);
            ddlidTipo_SelectedIndexChanged(sender, e);
        }
        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void ddlidTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string lblStatus = hddsStatus.Value;
            divFormulario_Solicitacao.Visible = ddlidTipo.SelectedValue != "0";
            if (!edicao) cmdSalvar.Visible = ddlidTipo.SelectedValue != "0";

            switch (ddlidTipo.SelectedValue)
            {
                case "1": // Férias
                    div_dtFinal_Form.Visible = true;
                    div_OpcoesDatas.Visible = true;
                    lblDtInicial.InnerText = $"Data de Início {lblStatus}";
                    lblDtFim.InnerText = $"Data Final {lblStatus}";
                    div_dtInicio.Attributes["class"] = "col-lg-3";
                    txtsdtInicio.Attributes["type"] = "date";
                    break;
                case "3":
                    div_dtFinal_Form.Visible = true;
                    div_OpcoesDatas.Visible = false;
                    lblDtInicial.InnerText = "Período: ";
                    div_dtInicio.Attributes["class"] = "col-lg-3";
                    txtsdtInicio.Attributes["type"] = "date";
                    lblDtFim.InnerText = "";
                    lblDtFim.InnerHtml = "&nbsp;";
                    txtsdtFinal.Attributes["type"] = "date";
                    break;
                case "2": // Especial
                    div_dtFinal_Form.Visible = false;
                    div_OpcoesDatas.Visible = false;
                    lblDtInicial.InnerText = "Data e Hora";
                    div_dtInicio.Attributes["class"] = "col-lg-3";
                    txtsdtInicio.Attributes["type"] = "datetime-local";
                    break;
                default:
                    div_dtFinal_Form.Visible = false;
                    div_OpcoesDatas.Visible = false;
                    divFormulario_Solicitacao.Visible = false;
                    break;
            }
        }

        protected void btnAtualizarViaUpload_Click(object sender, EventArgs e)
        {
            // Recarrega a solicitação atual para atualizar status, logs, etc.
            if (!string.IsNullOrEmpty(hddidSolicitacao.Value) && hddidSolicitacao.Value != "0")
            {
                Pesquisar(hddidSolicitacao.Value);
                ddlidTipo_SelectedIndexChanged(sender, e);
                MensagemPagina_Principal.MostraMensagem_Sucesso("Arquivo recebido e solicitação atualizada!");
            }
        }

        #endregion

        #region | Opçoes de Datas

        protected void btnOpenModalDatas_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtsdtInicio.Text) && !string.IsNullOrEmpty(txtsdtFinal.Text))
            {
                // Limpa campos do modal e abre
                txtModalDtInicio.Text = string.Empty;
                txtModalDtFim.Text = string.Empty;
                litErroModalData.Text = string.Empty;

                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModalDatas", "openModalDatas();", true);
            }
            else
            {
                MensagemPagina_Principal.MostraMensagem_Erro("Adicione uma Data Inicial e Final Padrão primeiro!");
            }
        }

        protected void btnAdicionarDataLista_Click(object sender, EventArgs e)
        {
            DateTime dtIni, dtFim;

            // Validação básica
            if (!DateTime.TryParse(txtModalDtInicio.Text, out dtIni) || !DateTime.TryParse(txtModalDtFim.Text, out dtFim))
            {
                litErroModalData.Text = "Datas inválidas.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "CloseModalDatas", "closeModalDatas();", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepModalOpen", "openModalDatas();", true);
                return;
            }

            if (dtIni > dtFim)
            {
                litErroModalData.Text = "Data inicial maior que final.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "CloseModalDatas", "closeModalDatas();", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepModalOpen", "openModalDatas();", true);
                return;
            }

            if (txtModalDtInicio.Text == txtsdtInicio.Text && txtModalDtFim.Text == txtsdtFinal.Text)
            {
                litErroModalData.Text = "Esta opção de datas já foram adicionadas nos campos principais.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "CloseModalDatas", "closeModalDatas();", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepModalOpen", "openModalDatas();", true);
                return;
            }

            // Adiciona na lista
            var lista = ListaOpcoesDatas;

            foreach (var item in lista)
            {
                // Compara apenas a parte da data (.Date) para ignorar horas, se houver
                if (item.DtInicio.Date == dtIni.Date && item.DtFinal.Date == dtFim.Date)
                {
                    litErroModalData.Text = "Esta opção de datas já foi adicionada na lista.";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "CloseModalDatas", "closeModalDatas();", true);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepModalOpen", "openModalDatas();", true);
                    return;
                }
            }

        

            lista.Add(new OpcaoDataItem { DtInicio = dtIni, DtFinal = dtFim });
            ListaOpcoesDatas = lista; // Atualiza ViewState

            AtualizarRepeaterDatas();

            // Fecha modal
            ScriptManager.RegisterStartupScript(this, this.GetType(), "CloseModalDatas", "closeModalDatas();", true);
        }


        protected void rptOpcoesDatas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Remover")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                var lista = ListaOpcoesDatas;

                if (index >= 0 && index < lista.Count)
                {
                    lista.RemoveAt(index);
                    ListaOpcoesDatas = lista;
                    AtualizarRepeaterDatas();
                }
            }
        }

        private void AtualizarRepeaterDatas()
        {
            var lista = ListaOpcoesDatas;
            rptOpcoesDatas.DataSource = lista;
            rptOpcoesDatas.DataBind();

            div_OpcoesDatas.Visible = true; // Mostra a div se houver itens ou se estiver no modo de edição
            divSemDatas.Visible = lista.Count == 0;
        }

        void AdicionaDatas(DataSet dsSalvar, string idSolicitacao)
        {
            var idSolicitacaoSalva = idSolicitacao;

            // Tenta pegar o ID retornado caso seja uma inserção nova
            if (idSolicitacao == "0" && dsSalvar.Tables[0].Columns.Contains("idSolicitacao"))
            {
                idSolicitacaoSalva = dsSalvar.Tables[0].Rows[0]["idSolicitacao"].ToString();
            }

            // 1. Limpa as datas antigas
            Dictionary<string, string> pDeleta = new Dictionary<string, string>
    {
        { "@sFuncao", "DELETA-OPCOES-DATAS" },
        { "@idSolicitacao", idSolicitacaoSalva }
    };
            BD.ExecutarDataSet(sProcedure, pDeleta);

            // 2. Insere as datas da Lista
            var listaDatas = ListaOpcoesDatas;
            foreach (var item in listaDatas)
            {
                // VERIFICAÇÃO DE SEGURANÇA:
                // O SQL Server (DATETIME) só aceita datas a partir de 1753. 
                // Se o C# mandar 0001-01-01, o banco explode.
                if (item.DtInicio.Year < 1900) item.DtInicio = DateTime.Now;
                if (item.DtFinal.Year < 1900) item.DtFinal = item.DtInicio;

                Dictionary<string, string> pInsere = new Dictionary<string, string>
        {
            { "@sFuncao", "INSERE-OPCOES-DATAS" },
            { "@idSolicitacao", idSolicitacaoSalva },
            
            // ALTERAÇÃO AQUI: Mudado para dd/MM/yyyy para agradar o SQL Server em PT-BR
            { "@dtInicio", item.DtInicio.ToString("dd/MM/yyyy") },
            { "@dtFinal", item.DtFinal.ToString("dd/MM/yyyy") }
        };
                BD.ExecutarDataSet(sProcedure, pInsere);
            }

            // Limpa a lista da memória
            ListaOpcoesDatas = new List<OpcaoDataItem>();
        }

        void ConsultarOpcoesDatas(string idSolicitacao)
        {
            Dictionary<string, string> vParamsDatas = new Dictionary<string, string>
{
    { "@sFuncao", "CONSULTAR-OPCOES-DATAS" }, // Você precisaria criar essa função na PROC
    { "@idSolicitacao", idSolicitacao }
};
            DataSet dsDatas = BD.ExecutarDataSet(sProcedure, vParamsDatas);
            var listaBanco = new List<OpcaoDataItem>();
            if (BD.ValidarDataSet(dsDatas, out _))
            {
                foreach (DataRow row in dsDatas.Tables[0].Rows)
                {
                    listaBanco.Add(new OpcaoDataItem
                    {
                        DtInicio = Convert.ToDateTime(row["dtInicio"]),
                        DtFinal = Convert.ToDateTime(row["dtFinal"])
                    });
                }
            }
            ListaOpcoesDatas = listaBanco;
            AtualizarRepeaterDatas();

        }

        protected void rptOpcoesDatas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            // Verifica se é um item de dados (Item ou AlternatingItem)
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // Encontra o botão de remover dentro do repeater
                LinkButton btnRemover = (LinkButton)e.Item.FindControl("btnRemoverData");

                if (btnRemover != null)
                {
                    // Se houver um ID de solicitação carregado e não for "0", estamos em modo de visualização/edição
                    if (!string.IsNullOrEmpty(hddidSolicitacao.Value) && hddidSolicitacao.Value != "0")
                    {
                        // Esconde o botão "X"
                        btnRemover.Visible = false;
                    }
                    else
                    {
                        // Mostra o botão "X" (Nova Solicitação)
                        btnRemover.Visible = true;
                    }
                }
            }
        }

        #endregion
    }
}