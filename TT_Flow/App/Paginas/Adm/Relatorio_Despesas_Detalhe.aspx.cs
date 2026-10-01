using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT_Flow.FrameWork;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Flow.App.Paginas.Manutencao
{
    public partial class Relatorio_Despesas_Detalhe : System.Web.UI.Page
    {
        static string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Relatorio_Despesas";

        [Serializable]
        public class RegraDespesa
        {
            public string idTipo { get; set; }
            public string sDescTipo { get; set; }
            public string nValor { get; set; }
            public string sFrequencia { get; set; }
            public string nDias { get; set; }
            // NOVA PROPRIEDADE
            public string sTotal { get; set; }
        }

        public List<RegraDespesa> ListaRegras
        {
            get
            {
                if (ViewState["ListaRegras"] == null) ViewState["ListaRegras"] = new List<RegraDespesa>();
                return (List<RegraDespesa>)ViewState["ListaRegras"];
            }
            set { ViewState["ListaRegras"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.RelatorioDespesas.Consultar, true);

            ScriptManager sm = ScriptManager.GetCurrent(this);
            if (sm != null) sm.EnablePageMethods = true;

            // 1. Define se é edição olhando a URL (Isso roda SEMPRE, garantindo que as abas fiquem visíveis no PostBack)
            string idUrl = Request.QueryString["id"];
            bool isEdicao = !string.IsNullOrEmpty(idUrl) && idUrl != "0";

            // 2. Aplica a visibilidade baseada na condição, não no PostBack
            tabLancamento.Visible = isEdicao;
            tabDespesa.Visible = isEdicao;

            if (!IsPostBack)
            {
                CarregarCombosIniciais();

                if (isEdicao)
                {
                    // Se tem ID, carrega os dados
                    CarregarDados(idUrl);
                    ConfigurarBotoesAcao(idUrl);
                }
                else
                {
                    // Se é novo, apenas data padrão
                    txtDtInicio.Text = DateTime.Now.ToString("yyyy-MM-dd");
                }
            }
            AplicaPermissoes(isEdicao);
        }

        private void AplicaPermissoes(bool isEdicao)
        {
            // Salvar Despesa
            btnSalvarBase.Visible = FUNCOES.ValidaPermissao(Permissao.RelatorioDespesas.SalvarDespesa);

            // Gerenciar Previsões (Forecasts)
            btnAbrirModal.Visible = FUNCOES.ValidaPermissao(Permissao.RelatorioDespesas.GerenciarPrevisões);

            // Gerenciar Lançamentos (Realizados)
            btnNovoLancamento.Visible = FUNCOES.ValidaPermissao(Permissao.RelatorioDespesas.GerenciarLancamentos);

            // Gerenciar Status (Botão de Ações)
            // Só exibe se for edição E se tiver permissão de status
            divAcoes.Visible = isEdicao && FUNCOES.ValidaPermissao(Permissao.RelatorioDespesas.GerenciarStatus);

            // Exibir Relatório (Aba Dashboard)
            // Só exibe se for edição E se tiver permissão de relatório
            tabDespesa.Visible = isEdicao && FUNCOES.ValidaPermissao(Permissao.RelatorioDespesas.ExibirRelatorio);

            // A aba de Lançamentos depende apenas de ser edição (conforme seu código original)
            tabLancamento.Visible = isEdicao;
        }

        // --- MÉTODO NOVO: CARREGAR DADOS DO BANCO ---
        private void CarregarDados(string idDespesa)
        {
            try
            {
                Dictionary<string, string> p = new Dictionary<string, string>();
                p.Add("@sFuncao", "CONSULTAR_DETALHE");
                p.Add("@idDespesas", idDespesa);

                DataSet ds = BD.ExecutarDataSet(sProcedure, p);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];

                    // 1. Preenche Header
                    txtsDscMotivo.Text = dr["sDscMotivo"].ToString();
                    txtsObservacao.Text = dr["sDscObservacao"].ToString();

                    if (dr["idPedido"] != DBNull.Value) ddlCentroCusto.SelectedValue = dr["idPedido"].ToString();

                    if (dr["dtInicioVigencia"] != DBNull.Value)
                        txtDtInicio.Text = Convert.ToDateTime(dr["dtInicioVigencia"]).ToString("yyyy-MM-dd");

                    // 2. Preenche Participantes (ListBox)
                    string participantesCSV = dr["sListaParticipantes"].ToString(); // Vem "1,5,9"
                    if (!string.IsNullOrEmpty(participantesCSV))
                    {
                        string[] partes = participantesCSV.Split(',');
                        foreach (ListItem item in ddlColaborador.Items)
                        {
                            if (partes.Contains(item.Value)) item.Selected = true;
                        }
                    }

                    // 3. Preenche Grid de Regras (Tabela 1 do DataSet)
                    if (ds.Tables.Count > 1)
                    {
                        List<RegraDespesa> regrasCarregadas = new List<RegraDespesa>();
                        foreach (DataRow row in ds.Tables[1].Rows)
                        {
                            // Cálculos auxiliares para o Total
                            decimal vlr = Convert.ToDecimal(row["nValor"]);
                            int dias = Convert.ToInt32(row["nQtdDias"]);
                            string totalRow = (vlr * dias).ToString("N2");

                            regrasCarregadas.Add(new RegraDespesa
                            {
                                idTipo = row["idTipoDespesa"].ToString(),
                                sDescTipo = row["sDescTipo"].ToString(),
                                nValor = Convert.ToDecimal(row["nValor"]).ToString("N2"),
                                sFrequencia = row["sFrequencia"].ToString(),
                                nDias = row["nQtdDias"].ToString(),
                                // PREENCHE O TOTAL
                                sTotal = totalRow
                            });
                        }
                        ListaRegras = regrasCarregadas; // Salva no ViewState
                        gvRegras.DataSource = ListaRegras;
                        gvRegras.DataBind();
                    }

                    // --- LÓGICA DO BADGE DE STATUS (NOVO) ---
                    int idStatus = dr["idStatus"] != DBNull.Value ? Convert.ToInt32(dr["idStatus"]) : 1;
                    string textoStatus = "";
                    string classeCor = "";

                    switch (idStatus)
                    {
                        case 1:
                            textoStatus = "Aberto";
                            classeCor = "label-warning"; // Amarelo
                            break;
                        case 2:
                            textoStatus = "Finalizado";
                            classeCor = "label-primary"; // Azul Escuro
                            break;
                        case 3:
                            textoStatus = "Aprovado";
                            classeCor = "label-success"; // Verde
                            break;
                        case 4:
                            textoStatus = "Rejeitado";
                            classeCor = "label-danger"; // Vermelho
                            break;
                        case 5:
                            textoStatus = "Excluído";
                            classeCor = "label-danger";
                            break;
                        default:
                            textoStatus = "Novo";
                            classeCor = "label-default";
                            break;
                    }

                    // Monta o HTML do balãozinho
                    litStatusBadge.Text = $"<span class='label {classeCor}' style='font-size: 100%; vertical-align: middle; position: relative; top: -3px; margin-left: 5px;'>{textoStatus.ToUpper()}</span>";

                    CarregarLancamentos(idDespesa);
                    GerarRelatorioDinamico(idDespesa);
                    ConfigurarBotoesAcao(idDespesa);
                    CarregarHistorico(idDespesa);
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_Entregas.MostraMensagem_Erro("Erro ao carregar dados: " + ex.Message);
            }
        }

        private string RecuperarFlagSupervisor()
        {
            return "";
        }

        private void CarregarCombosIniciais()
        {
            FUNCOES.Popula_Combo(ddlColaborador, $"sp_Select 'Flow_Colaboradores-REL', @idFiltro={IDENTITY.Variaveis.idUsuario()}", "idColaborador", "sDscColaborador", false, null, null);
            FUNCOES.Popula_Combo(ddlCentroCusto, "sp_Select 'Flow_Adm_CentroDeCusto', @idPesquisa=1, @sPesquisa=C", "idCentroDeCusto", "sDescricao", false, "Selecione o Centro de Custo", "0");
            PopulaComboTipos();
        }

        private void PopulaComboTipos()
        {
            try
            {
                ddlsTipoCompra.Items.Clear();
                ddlsTipoCompra.Items.Add(new ListItem("Selecione...", "0"));
                Dictionary<string, string> vParametros = new Dictionary<string, string> { { "@sFuncao", "FLOW-TIPOS" } };
                DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                        ddlsTipoCompra.Items.Add(new ListItem(row["sDscGasto"].ToString(), row["idTipoGastos"].ToString()));
                }
            }
            catch { }
        }

        // --- CONTROLES MODAL ---
        protected void btnAbrirModal_Click(object sender, EventArgs e)
        {

            // 1. Primeiro, repopula o combo com todos os tipos vindos do banco
            PopulaComboTipos();

            // 2. Recupera os IDs que já estão na lista de regras
            var idsJaAdicionados = ListaRegras.Select(r => r.idTipo).ToList();

            // 3. Remove do DropDown os itens que já existem na grade
            foreach (string id in idsJaAdicionados)
            {
                ListItem itemExistente = ddlsTipoCompra.Items.FindByValue(id);
                if (itemExistente != null)
                {
                    ddlsTipoCompra.Items.Remove(itemExistente);
                }
            }

            ddlsTipoCompra.SelectedIndex = 0;
            ddlFrequencia.SelectedIndex = 0;
            txtQtdDiasRegra.Text = "";
            txtnValorLimite.Text = "";
            //gvRegras.DataBind();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "abrirModal", "abrirModalRegra();", true);
        }

        protected void btnFecharModal_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "fecharModal", "fecharModalRegra();", true);
        }

        // --- ADICIONAR REGRA ---
        protected void btnAdicionarRegra_Click(object sender, EventArgs e)
        {
            if (ddlsTipoCompra.SelectedValue == "0") { MensagemModal.MostraMensagem_Erro("Selecione o Tipo."); return; }
            if (string.IsNullOrEmpty(txtnValorLimite.Text)) { MensagemModal.MostraMensagem_Erro("Informe o Valor."); return; }
            if (string.IsNullOrEmpty(txtQtdDiasRegra.Text)) { MensagemModal.MostraMensagem_Erro("Informe os Dias."); return; }

            // --- NOVA VALIDAÇÃO: Verifica duplicidade ---
            // Verifica se na lista atual já existe um objeto com o mesmo idTipo selecionado
            if (ListaRegras.Any(x => x.idTipo == ddlsTipoCompra.SelectedValue))
            {
                MensagemModal.MostraMensagem_Erro("Este Tipo de Despesa já foi adicionado na lista.");
                return;
            }

            decimal valorDec = Convert.ToDecimal(txtnValorLimite.Text);
            int diasInt = int.Parse(txtQtdDiasRegra.Text);
            string totalCalculado = (valorDec * diasInt).ToString("N2");

            var novaRegra = new RegraDespesa
            {
                idTipo = ddlsTipoCompra.SelectedValue,
                sDescTipo = ddlsTipoCompra.SelectedItem.Text,
                nValor = txtnValorLimite.Text,
                sFrequencia = ddlFrequencia.SelectedValue,
                nDias = txtQtdDiasRegra.Text,

                // PREENCHE O TOTAL
                sTotal = totalCalculado

            };

            var lista = ListaRegras;
            lista.Add(novaRegra);
            ListaRegras = lista;

            gvRegras.DataSource = lista;
            gvRegras.DataBind();


            VerificarRegrasBloqueio();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "fecharModal", "fecharModalRegra();", true);

            MensagemPagina_Entregas.MostraMensagem_Aviso("Salve para Registrar as novas previsões.");
        }

        protected void gvRegras_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var lista = ListaRegras;
            lista.RemoveAt(e.RowIndex);
            ListaRegras = lista;
            gvRegras.DataSource = lista;
            gvRegras.DataBind();

            MensagemPagina_Entregas.MostraMensagem_Aviso("Salve para Registrar exclusões.");
        }

        // --- SALVAR BASE E REGRAS ---
        protected void btnSalvarBase_Click(object sender, EventArgs e)
        {
            try
            {
                // --- Validações ---
                if (ddlCentroCusto.SelectedValue == "0") { MensagemPagina_Entregas.MostraMensagem_Erro("Selecione o Centro de Custo."); return; }
                if (string.IsNullOrEmpty(txtDtInicio.Text)) { MensagemPagina_Entregas.MostraMensagem_Erro("Selecione a Data de Início."); return; }
                if (string.IsNullOrEmpty(txtsDscMotivo.Text)) { MensagemPagina_Entregas.MostraMensagem_Erro("Informe o Motivo Geral."); return; }

                List<string> idsColaboradores = new List<string>();
                foreach (ListItem item in ddlColaborador.Items) if (item.Selected) idsColaboradores.Add(item.Value);

                if (idsColaboradores.Count == 0) { MensagemPagina_Entregas.MostraMensagem_Erro("Selecione pelo menos um colaborador."); return; }
                if (ListaRegras.Count == 0) { MensagemPagina_Entregas.MostraMensagem_Erro("Adicione pelo menos uma Regra de Despesa."); return; }

                // Cria a string "1,2,3" para usar tanto no Header quanto nos Itens de Regra
                string strParticipantes = string.Join(",", idsColaboradores);

                int maxDias = 0;
                if (ListaRegras.Count > 0) { try { maxDias = ListaRegras.Max(r => int.Parse(r.nDias)); } catch { maxDias = 1; } }

                DateTime dtInicio = Convert.ToDateTime(txtDtInicio.Text);
                DateTime dtFim = dtInicio.AddDays(maxDias);

                // --- 1. Salvar Header (Base) ---
                Dictionary<String, String> pHeader = new Dictionary<string, string>();
                pHeader.Add("@sFuncao", "SALVAR");

                string idAtual = Request.QueryString["id"] ?? "0";
                pHeader.Add("@idDespesas", idAtual);

                pHeader.Add("@sDscMotivo", txtsDscMotivo.Text);
                pHeader.Add("@idPedido", ddlCentroCusto.SelectedValue);
                pHeader.Add("@idTipo", "1");
                pHeader.Add("@sDscObservacao", txtsObservacao.Text);
                pHeader.Add("@idUsuario", IDENTITY.Variaveis.idUsuario()); // Aqui é quem criou a base (Logado)

                pHeader.Add("@sidParticipantes", strParticipantes); // Lista no Header

                pHeader.Add("@dtDespesa", dtInicio.ToString("dd/MM/yyyy"));
                pHeader.Add("@dtFimVigencia", dtFim.ToString("dd/MM/yyyy"));
                pHeader.Add("@idStatus", "1");
                pHeader.Add("@sExcluido", "N");
                pHeader.Add("@nValorLimite", "0");

                DataSet dsHeader = BD.ExecutarDataSet(sProcedure, pHeader);
                string erro = "";

                if (BD.ValidarDataSet(dsHeader, out erro))
                {
                    string idGerado = RETORNO.DATASET(dsHeader, 0, "idDespesas");

                    // --- 2. Salvar Regras (Itens Tipo 'C') ---
                    if (ListaRegras.Count > 0)
                    {
                        foreach (var regra in ListaRegras)
                        {
                            Dictionary<String, String> pItem = new Dictionary<string, string>();
                            pItem.Add("@sFuncao", "INSERIR_DESPESAS");
                            pItem.Add("@idDespesas", idGerado);
                            pItem.Add("@idTipoGastos", regra.idTipo);
                            pItem.Add("@nValor", regra.nValor.Replace(".", "").Replace(",", "."));
                            pItem.Add("@sFrequencia", regra.sFrequencia);
                            pItem.Add("@nQtdDias", regra.nDias);
                            pItem.Add("@sTipoItem", "C");
                            pItem.Add("@dtDespesa", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                            // CORREÇÃO: Passamos quem criou no @idUsuario (apenas log)
                            // E passamos a LISTA de quem essa regra se aplica no @sidParticipantes
                            pItem.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());
                            pItem.Add("@sidParticipantes", strParticipantes);

                            pItem.Add("@idFormaPagamento", "0");
                            pItem.Add("@idCategoriaPagar", "0");
                            pItem.Add("@sLocal", "Regra Definida");

                            DataSet ds = BD.ExecutarDataSet(sProcedure, pItem);

                            // PEGA O ID GERADO
                            string idItemGerado = "";
                            if (BD.ValidarDataSet(ds) && ds.Tables[0].Columns.Contains("idItens"))
                                idItemGerado = ds.Tables[0].Rows[0]["idItens"].ToString();

                            if (!string.IsNullOrEmpty(idItemGerado))
                            {
                                // AQUI ESTÁ A MÁGICA: Salva o que estava na lista de memória vinculado ao novo ID
                                PersistirArquivosNoBanco(idItemGerado);
                            }
                        }
                    }

                    MensagemPagina_Entregas.MostraMensagem_Sucesso($"Base salva com sucesso! ID: {idGerado}");

                    GerarRelatorioDinamico(idGerado);

                    tabLancamento.Visible = true;
                    tabDespesa.Visible = true;
                    if (idAtual == "0")
                        Response.Redirect($"Relatorio_Despesas_Detalhe.aspx?id={idGerado}");
                }
                else
                {
                    MensagemPagina_Entregas.MostraMensagem_Erro("Erro ao salvar: " + erro);
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_Entregas.MostraMensagem_Erro("Erro crítico: " + ex.Message);
            }
        }
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/App/Paginas/Adm/Relatorio_Despesas.aspx");
        }

        [WebMethod]
        public static string BuscarLimitePorTipo(int idTipo)
        {
            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
        {
          { "@sFuncao", "CONSULTAR_RECURSO" },
          { "@idTipoGastos", idTipo.ToString() }
        };
                DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    decimal valor = Convert.ToDecimal(RETORNO.DATASET(ds, 0, "vlrMaximo"));
                    return valor.ToString("N2");
                }
                return "0,00";
            }
            catch { return "0,00"; }
        }

        // -----------------------------------------------------------
        // BLOCO DE LANÇAMENTOS (REALIZADO)
        // -----------------------------------------------------------

        private void CarregarLancamentos(string idDespesa)
        {
            try
            {
                Dictionary<string, string> p = new Dictionary<string, string>();
                p.Add("@sFuncao", "CONSULTAR_LANCAMENTOS"); // <--- Nova função no SQL
                p.Add("@idDespesas", idDespesa);

                DataSet ds = BD.ExecutarDataSet(sProcedure, p);

                //if (ds.Tables.Count > 0)
                //{
                //    gvLancamentos.DataSource = ds.Tables[0];
                //    gvLancamentos.DataBind();
                //}

                if (ds != null && ds.Tables.Count > 0)
                {
                    gvLancamentos.DataSource = ds.Tables[0];
                }
                else
                {
                    gvLancamentos.DataSource = null;
                }

                gvLancamentos.DataBind();

                // <--- ADICIONE ESTA CHAMADA AQUI
                VerificarRegrasBloqueio();
            }
            catch (Exception ex)
            {
                MensagemPagina_Entregas.MostraMensagem_Erro("Erro ao carregar lançamentos: " + ex.Message);
            }
        }

        protected void btnNovoLancamento_Click(object sender, EventArgs e)
        {
            pnlDadosDespesa.Visible = true;
            // 1. Limpa e Prepara campos de texto
            txtDataLancamento.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtLocalLancamento.Text = "";
            txtValorLancamento.Text = "";

            // 2. Preenche o Dropdown de Usuários (NOVO)
            // Lógica: Pega apenas os itens SELECIONADOS na ListBox principal (ddlColaborador)
            ddlUsuarioLancamento.Items.Clear();
            ddlUsuarioLancamento.Items.Add(new ListItem("Selecione o Colaborador...", "0"));

            IdItemEmEdicao = "0";
            ListaArquivosMemoria = new List<ArquivoDTO>();

            bool temSelecionado = false;
            foreach (ListItem item in ddlColaborador.Items)
            {
                if (item.Selected)
                {
                    ddlUsuarioLancamento.Items.Add(new ListItem(item.Text, item.Value));
                    temSelecionado = true;
                }
            }

            // Tenta selecionar automaticamente o usuário logado se ele estiver na lista
            try
            {
                string idLogado = IDENTITY.Variaveis.idUsuario().ToString();
                if (ddlUsuarioLancamento.Items.FindByValue(idLogado) != null)
                    ddlUsuarioLancamento.SelectedValue = idLogado;
            }
            catch { }

            // Validação simples para não abrir modal vazio se ninguém foi escolhido na base
            if (!temSelecionado)
            {
                MensagemPagina_Entregas.MostraMensagem_Erro("Selecione e Salve os Participantes na 'Definição da Base' antes de lançar despesas.");
                return;
            }

            // 3. Preenche o Dropdown de Tipos
            //if (ddlTipoLancamento.Items.Count == 0)
            //{
            //    ddlTipoLancamento.Items.Clear();
            //    ddlTipoLancamento.Items.Add(new ListItem("Selecione o Tipo...", "0"));
            //    foreach (ListItem item in ddlsTipoCompra.Items)
            //    {
            //        if (item.Value != "0") // Ignora o "Selecione" da origem para não duplicar
            //            ddlTipoLancamento.Items.Add(new ListItem(item.Text, item.Value));
            //    }
            //}
            //ddlTipoLancamento.SelectedIndex = 0;


            // 3. Preenche o Dropdown de Tipos (FILTRADO PELAS REGRAS DEFINIDAS)
            ddlTipoLancamento.Items.Clear();
            ddlTipoLancamento.Items.Add(new ListItem("Selecione o Tipo...", "0"));

            // Se houver regras cadastradas, carrega apenas elas
            if (ListaRegras != null && ListaRegras.Count > 0)
            {
                // Usa um HashSet para garantir que não haja duplicatas no combo
                HashSet<string> tiposAdicionados = new HashSet<string>();

                foreach (var regra in ListaRegras)
                {
                    if (!tiposAdicionados.Contains(regra.idTipo))
                    {
                        ddlTipoLancamento.Items.Add(new ListItem(regra.sDescTipo, regra.idTipo));
                        tiposAdicionados.Add(regra.idTipo);
                    }
                }
            }
            else
            {
                // Opcional: Se não tiver regras, exibe mensagem ou deixa vazio
                // MensagemPagina_Entregas.MostraMensagem_Aviso("Defina as regras/previsões antes de lançar.");
            }

            ddlTipoLancamento.SelectedIndex = 0;

            ConfigurarModalLancamento(false);
            CarregarGridArquivos();

            // Abre o modal
            ScriptManager.RegisterStartupScript(this, this.GetType(), "abrirModalLanc", "abrirModalLancamento();", true);
        }

        protected void btnSalvarLancamento_Click(object sender, EventArgs e)
        {
            try
            {
                string idDespesa = Request.QueryString["id"];

                // Validação da URL
                if (string.IsNullOrEmpty(idDespesa) || idDespesa == "0")
                {
                    MensagemModalLanc.MostraMensagem_Erro("Salve a base (Cabeçalho) antes de lançar despesas.");
                    // Força a modal a reabrir para mostrar o erro
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepModalOpen", "abrirModalLancamento();", true);
                    return;
                }

                // --- 1. VALIDAÇÃO DE DATA (VIGÊNCIA) ---
                DateTime dtLancamento;
                if (!DateTime.TryParse(txtDataLancamento.Text, out dtLancamento))
                {
                    MensagemModalLanc.MostraMensagem_Erro("Data do lançamento inválida.");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepModalOpen", "abrirModalLancamento();", true);
                    return;
                }

                DateTime dtInicioVigencia;
                if (!DateTime.TryParse(txtDtInicio.Text, out dtInicioVigencia))
                {
                    MensagemModalLanc.MostraMensagem_Erro("Data de Início da Vigência não definida na aba Definições.");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepModalOpen", "abrirModalLancamento();", true);
                    return;
                }

                // Calcula a data final baseada nas regras (mesma lógica do Salvar Base)
                int maxDias = 0;
                if (ListaRegras.Count > 0)
                {
                    try { maxDias = ListaRegras.Max(r => int.Parse(r.nDias)); } catch { maxDias = 0; }
                }

                // Se maxDias for 0 ou 1, consideramos o próprio dia. Se for maior, somamos.
                // Nota: O AddDays adiciona dias corridos. Se a regra é "5 dias" contando com o hoje, 
                // a data final é dtInicio + 4 dias? Ou dtInicio + 5? 
                // Vou usar a lógica padrão: DataInicio + DiasLimite.
                DateTime dtFimVigencia = dtInicioVigencia.AddDays(maxDias);

                // Verifica se está FORA do range
                // Removemos a hora (.Date) para garantir que a comparação seja apenas por dia
                if (dtLancamento.Date < dtInicioVigencia.Date || dtLancamento.Date > dtFimVigencia.Date)
                {
                    string msg = $"A data do lançamento ({dtLancamento.ToString("dd/MM/yyyy")}) está fora da vigência permitida.<br/>";
                    msg += $"Período Válido: <b>{dtInicioVigencia.ToString("dd/MM/yyyy")}</b> até <b>{dtFimVigencia.ToString("dd/MM/yyyy")}</b>.";

                    MensagemModalLanc.MostraMensagem_Erro(msg);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepModalOpen", "abrirModalLancamento();", true);
                    return;
                }

                // --- VALIDAÇÕES DE CAMPO ---
                // Em cada erro, chamamos o script 'abrirModalLancamento();' para impedir que ela suma

                if (ddlUsuarioLancamento.SelectedValue == "0")
                {
                    MensagemModalLanc.MostraMensagem_Erro("Selecione quem gastou.");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepModalOpen", "abrirModalLancamento();", true);
                    return;
                }

                if (ddlTipoLancamento.SelectedValue == "0")
                {
                    MensagemModalLanc.MostraMensagem_Erro("Selecione o Tipo.");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepModalOpen", "abrirModalLancamento();", true);
                    return;
                }

                if (string.IsNullOrEmpty(txtValorLancamento.Text))
                {
                    MensagemModalLanc.MostraMensagem_Erro("Informe o Valor.");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepModalOpen", "abrirModalLancamento();", true);
                    return;
                }

                if (string.IsNullOrEmpty(txtLocalLancamento.Text))
                {
                    MensagemModalLanc.MostraMensagem_Erro("Informe o Local.");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepModalOpen", "abrirModalLancamento();", true);
                    return;
                }

                if (!ListaArquivosMemoria.Any(x => !x.MarcadoParaExclusao))
                {
                    MensagemModalLanc.MostraMensagem_Erro("É obrigatório anexar pelo menos um comprovante.");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepModalOpen", "abrirModalLancamento();", true);
                    return;
                }

                // --- SE PASSOU TUDO, GRAVA NO BANCO ---

                Dictionary<string, string> pItem = new Dictionary<string, string>();
                pItem.Add("@sFuncao", "INSERIR_DESPESAS");
                pItem.Add("@idDespesas", idDespesa);
                pItem.Add("@sTipoItem", "L");

                pItem.Add("@idTipoGastos", ddlTipoLancamento.SelectedValue);
                pItem.Add("@nValor", txtValorLancamento.Text.Replace(".", "").Replace(",", "."));
                pItem.Add("@idColaborador", ddlUsuarioLancamento.SelectedValue);
                pItem.Add("@idUsuario", RecuperarUsuario(ddlUsuarioLancamento.SelectedValue));
                pItem.Add("@sLocal", txtLocalLancamento.Text);
                DateTime dtDespesas = Convert.ToDateTime(txtDataLancamento.Text);
                pItem.Add("@dtDespesa", dtDespesas.ToString("dd/MM/yyyy HH:mm:ss"));

                pItem.Add("@sFrequencia", "");
                pItem.Add("@nQtdDias", "0");
                pItem.Add("@idFormaPagamento", ddlidCartao.SelectedValue);
                pItem.Add("@idCategoriaPagar", "0");

                DataSet ds = BD.ExecutarDataSet(sProcedure, pItem);


                string idItemGerado = "";
                if (BD.ValidarDataSet(ds) && ds.Tables[0].Columns.Contains("idItens"))
                    idItemGerado = ds.Tables[0].Rows[0]["idItens"].ToString();

                // --- SALVA OS ARQUIVOS VINCULADOS AO NOVO ID ---
                if (!string.IsNullOrEmpty(idItemGerado))
                {
                    PersistirArquivosNoBanco(idItemGerado);
                }

                // Sucesso: Recarrega grid e FECHA a modal
                CarregarLancamentos(idDespesa);
                GerarRelatorioDinamico(idDespesa);
                CarregarDados(idDespesa);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "CloseModal", "fecharModalLancamento();", true);
                MensagemPagina_Entregas.MostraMensagem_Sucesso("Lançamento realizado com sucesso!");
            }
            catch (Exception ex)
            {
                // Se der erro de banco (SQL), mantém a modal aberta mostrando o erro
                MensagemModalLanc.MostraMensagem_Erro("Erro: " + ex.Message);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepModalOpen", "abrirModalLancamento();", true);
            }
        }

        protected string RecuperarUsuario(string idColaborador)
        {
            DataSet ds = new DataSet();
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "RECUPERAR-USUARIO");
            vParametros.Add("@sidParticipantes", idColaborador);

            ds = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(ds))
                return RETORNO.DATASET(ds, "idUsuario").ToString();
            else
                return "0";
        }

        protected void btnCancelarLanc_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "fecharModalLanc", "fecharModalLancamento();", true);
        }

        // Melhor abordagem para exclusão direta no banco:
        protected void gvLancamentos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            pnlDadosDespesa.Visible = true;
            if (e.CommandName == "ExcluirLanc")
            {
                string idItem = e.CommandArgument.ToString();

                Dictionary<string, string> p = new Dictionary<string, string>();
                p.Add("@sFuncao", "EXCLUIR_DESPESAS");
                p.Add("@idItens", idItem);
                BD.ExecutarDataSet(sProcedure, p);

                string idRelatorio = Request.QueryString["id"];
                CarregarLancamentos(idRelatorio);

                GerarRelatorioDinamico(idRelatorio);
                VerificarRegrasBloqueio();
                CarregarHistorico(idRelatorio);

                updGeral.Update();

                MensagemPagina_Entregas.MostraMensagem_Sucesso("Lançamento removido com sucesso.");
            }
            if (e.CommandName == "Arquivos") // CLICOU NO CLIPS DA GRID
            {
                string idItem = e.CommandArgument.ToString();
                IdItemEmEdicao = idItem;
                pnlDadosDespesa.Visible = false;
                // 1. Carrega os arquivos existentes desse lançamento
                CarregarArquivosDoBanco(idItem);
                CarregarGridArquivos();

                // 2. Configura tela para MODO EDIÇÃO ARQUIVOS (Campos bloqueados, botão Salvar Anexos visível)
                // Nota: Como os campos estão bloqueados, não é estritamente necessário preencher os valores dos textos, 
                // mas se quiser, teria que buscar os dados do lançamento no banco aqui.
                ConfigurarModalLancamento(true);

                // 3. Abre a MESMA modal
                ScriptManager.RegisterStartupScript(this, this.GetType(), "abrirModalLanc", "abrirModalLancamento();", true);
            }
        }

        private void GerarRelatorioDinamico(string idDespesa)
        {
            try
            {
                Dictionary<string, string> p = new Dictionary<string, string>();
                p.Add("@sFuncao", "RELATORIO_CONSOLIDADO");
                p.Add("@idDespesas", idDespesa);

                DataSet ds = BD.ExecutarDataSet(sProcedure, p);

                if (ds.Tables.Count < 2 || ds.Tables[1].Rows.Count == 0)
                {
                    litRelatorioDinamic.Text = "<div class='alert alert-info'>Nenhum dado ou regra definida para exibir.</div>";
                    return;
                }

                DataRow drHeader = ds.Tables[0].Rows[0];
                if (drHeader["dtInicio"] == DBNull.Value || drHeader["dtFim"] == DBNull.Value) return;

                DateTime dtInicio = Convert.ToDateTime(drHeader["dtInicio"]);
                DateTime dtFim = Convert.ToDateTime(drHeader["dtFim"]);
                DataTable dtDados = ds.Tables[1];

                StringBuilder html = new StringBuilder();
                html.Append("<table class='table table-bordered table-condensed tabela-relatorio'>");

                // CABEÇALHO
                html.Append("<thead><tr>");
                html.Append("<th style='width: 30px;'>#</th>");
                html.Append("<th>Tipo de Despesa</th>");
                html.Append("<th>Total Previsto</th>");
                html.Append("<th>Total Gasto</th>");
                html.Append("<th>Saldo</th>");

                List<DateTime> listaDias = new List<DateTime>();
                for (DateTime date = dtInicio; date <= dtFim; date = date.AddDays(1))
                {
                    listaDias.Add(date);
                    html.Append($"<th style='background-color: #e2e6ea;'>{date.ToString("dd/MM")}</th>");
                }
                html.Append("</tr></thead><tbody>");

                // AGRUPAMENTO DE COLABORADORES
                var colaboradores = dtDados.AsEnumerable()
                    .Select(r => r.Field<string>("sNomeColaborador"))
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                foreach (var nomeColaborador in colaboradores)
                {
                    int totalColunas = 5 + listaDias.Count;
                    html.Append($"<tr class='row-colaborador'><td colspan='{totalColunas}'><i class='fa fa-user'></i> {nomeColaborador}</td></tr>");

                    var dadosColab = dtDados.AsEnumerable().Where(r => r.Field<string>("sNomeColaborador") == nomeColaborador);

                    // Agrupa por Tipo e captura a regra de dias vinda do SQL
                    var tiposDespesa = dadosColab
                        .GroupBy(r => new
                        {
                            Tipo = r.Field<string>("sDescTipo"),
                            LimiteRule = Convert.ToDecimal(r["nValorLimite"]),
                            DiasRule = Convert.ToInt32(r["nQtdDias"]) // <--- AQUI PEGA A DURAÇÃO DA REGRA (Tabela Itens)
                        });

                    // Totais do Colaborador (Rodapé)
                    decimal sumMetaColab = 0;
                    decimal sumGastoColab = 0;
                    Dictionary<DateTime, decimal> sumDiasColab = new Dictionary<DateTime, decimal>();
                    foreach (var d in listaDias) sumDiasColab[d] = 0;

                    int contadorId = 1;

                    foreach (var grupoTipo in tiposDespesa)
                    {
                        decimal totalGastoLinha = 0;
                        decimal totalLimiteLinha = 0;
                        bool temDiaEstourado = false;
                        StringBuilder celulasDias = new StringBuilder();

                        // --- LÓGICA DE DURAÇÃO DA REGRA ---
                        // Se a regra diz 1 dia, ela vale de dtInicio até dtInicio.
                        // Se diz 2 dias, vale de dtInicio até dtInicio + 1.
                        int diasValidos = grupoTipo.Key.DiasRule > 0 ? grupoTipo.Key.DiasRule : 1;
                        DateTime dataLimiteRegra = dtInicio.AddDays(diasValidos - 1);

                        foreach (DateTime dia in listaDias)
                        {
                            var registro = grupoTipo.FirstOrDefault(r => Convert.ToDateTime(r["dtDespesa"]).Date == dia.Date);

                            decimal valorGastoDia = 0;
                            decimal valorLimiteDia = 0;

                            // Só aplica o limite se o dia estiver dentro da duração da regra
                            if (dia.Date <= dataLimiteRegra.Date)
                            {
                                valorLimiteDia = grupoTipo.Key.LimiteRule;
                            }

                            if (registro != null)
                                valorGastoDia = Convert.ToDecimal(registro["nValorGasto"]);

                            totalGastoLinha += valorGastoDia;
                            totalLimiteLinha += valorLimiteDia; // Soma o limite correto (valor ou 0)
                            sumDiasColab[dia] += valorGastoDia;

                            bool estourouHoje = valorGastoDia > valorLimiteDia;
                            // Considera estouro se gastou mais que o limite (mesmo que o limite seja 0 num dia fora da regra)
                            if (estourouHoje && (valorLimiteDia > 0 || valorGastoDia > 0)) temDiaEstourado = true;

                            string styleCell = "";
                            if (valorGastoDia > 0)
                            {
                                styleCell = estourouHoje ? "background-color: #f2dede; color: #a94442; font-weight:bold;" : "";
                                celulasDias.Append($"<td class='cell-day' style='{styleCell}'>{valorGastoDia:N2}</td>");
                            }
                            else
                            {
                                // Visual: Mostra um ponto cinza se o dia não tem regra (acabou a verba de dias)
                                string charVazio = (dia.Date <= dataLimiteRegra.Date) ? "-" : "<span style='color:#eee'>.</span>";
                                celulasDias.Append($"<td class='cell-day' style='color:#ccc;'>{charVazio}</td>");
                            }
                        }

                        sumMetaColab += totalLimiteLinha;
                        sumGastoColab += totalGastoLinha;
                        decimal saldoLinha = totalLimiteLinha - totalGastoLinha;

                        // Cores da Linha
                        string classeLinha = "success";
                        string iconeStatus = "<i class='fa fa-check-circle text-success'></i>";

                        if (saldoLinha < 0) { classeLinha = "danger"; iconeStatus = "<i class='fa fa-times-circle text-danger'></i>"; }
                        else if (temDiaEstourado) { classeLinha = "warning"; iconeStatus = "<i class='fa fa-exclamation-triangle text-warning'></i>"; }

                        html.Append($"<tr class='{classeLinha}'>");
                        html.Append($"<td class='text-center text-muted'>{contadorId}</td>");
                        html.Append($"<td class='cell-info'>{iconeStatus} {grupoTipo.Key.Tipo}</td>");

                        // Exibe o Total Previsto (Soma correta baseada nos dias)
                        html.Append($"<td class='text-right'>{totalLimiteLinha:N2}</td>");
                        html.Append($"<td class='text-right'><b>{totalGastoLinha:N2}</b></td>");

                        string corTextoSaldo = saldoLinha < 0 ? "red" : "green";
                        html.Append($"<td class='text-right' style='color:{corTextoSaldo}; font-weight:bold;'>{saldoLinha:N2}</td>");
                        html.Append(celulasDias.ToString());
                        html.Append("</tr>");
                        contadorId++;
                    }

                    // Rodapé
                    html.Append("<tr class='row-total'>");
                    html.Append("<td colspan='2' class='text-right'>Total:</td>");
                    html.Append($"<td class='text-right'>{sumMetaColab:N2}</td>");
                    html.Append($"<td class='text-right'>{sumGastoColab:N2}</td>");

                    decimal saldoColab = sumMetaColab - sumGastoColab;
                    string corSaldoColab = saldoColab < 0 ? "red" : "green";
                    html.Append($"<td class='text-right' style='color:{corSaldoColab};'>{saldoColab:N2}</td>");

                    foreach (var dia in listaDias)
                    {
                        decimal valDia = sumDiasColab[dia];
                        string styleTotalDia = valDia > 0 ? "font-weight:bold;" : "color:#ccc;";
                        html.Append($"<td class='text-right' style='{styleTotalDia}'>{valDia:N2}</td>");
                    }
                    html.Append("</tr>");
                }

                html.Append("</tbody></table>");
                litRelatorioDinamic.Text = html.ToString();
            }
            catch (Exception ex)
            {
                litRelatorioDinamic.Text = $"<div class='alert alert-danger'>Erro: {ex.Message}</div>";
            }
        }

        // Método auxiliar para verificar arquivos e desenhar o ícone na Grid
        protected string ObterIconeArquivo(object idItem)
        {
            try
            {
                if (idItem == null) return "<i class='fa fa-times text-danger'></i>";

                cls_Arquivos objArq = new cls_Arquivos();
                // Chama a consulta de arquivos para este item específico
                DataSet ds = objArq.ConsultarArquivos(int.Parse(idItem.ToString()), "Despesas");

                if (BD.ValidarDataSet(ds) && ds.Tables[0].Rows.Count > 0)
                {
                    // Tem arquivo: Retorna Check Verde
                    return "<i class='fa fa-check-circle' style='color:green; font-size:16px;' title='Comprovante anexado'></i>";
                }
                else
                {
                    // Não tem arquivo: Retorna X Vermelho
                    return "<i class='fa fa-times-circle' style='color:red; font-size:16px;' title='Sem comprovante'></i>";
                }
            }
            catch
            {
                return "-";
            }
        }

        // Método auxiliar para pegar os IDs que já possuem gastos
        private HashSet<string> ObterTiposComGasto()
        {
            HashSet<string> tipos = new HashSet<string>();
            foreach (GridViewRow row in gvLancamentos.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    var id = gvLancamentos.DataKeys[row.RowIndex]["idTipoGastos"];
                    if (id != null) tipos.Add(id.ToString());
                }
            }
            return tipos;
        }

        // Evento que roda para CADA LINHA da grid de previsões quando ela é montada
        protected void gvRegras_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton btnExcluir = (LinkButton)e.Row.FindControl("btnExcluirRegra");

                if (btnExcluir != null)
                {
                    // Regra 1: Permissão de Gerenciar
                    bool temPermissao = FUNCOES.ValidaPermissao(Permissao.RelatorioDespesas.GerenciarPrevisões);

                    // Regra 2: Não ter gasto vinculado
                    var regra = (RegraDespesa)e.Row.DataItem;
                    HashSet<string> tiposComGasto = ObterTiposComGasto();
                    bool temGasto = tiposComGasto.Contains(regra.idTipo);

                    // O botão só aparece se tiver permissão E não tiver gasto
                    btnExcluir.Visible = temPermissao && !temGasto;
                }
            }
        }

        protected void gvLancamentos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton btnArquivos = (LinkButton)e.Row.FindControl("btnArquivos");
                LinkButton btnExcluirLanc = (LinkButton)e.Row.FindControl("btnExcluirLanc");

                bool podeGerenciar = FUNCOES.ValidaPermissao(Permissao.RelatorioDespesas.GerenciarLancamentos);

                if (btnArquivos != null) btnArquivos.Visible = podeGerenciar;
                if (btnExcluirLanc != null) btnExcluirLanc.Visible = podeGerenciar;
            }
        }

        #region | Arquivos (Logica Mestre-Detalhe)

        [Serializable]
        public class ArquivoDTO
        {
            public string IdTemporario { get; set; } = Guid.NewGuid().ToString();
            public int IdBanco { get; set; } = 0;
            public string Nome { get; set; }
            public string Descricao { get; set; }
            public byte[] Bytes { get; set; }
            public bool MarcadoParaExclusao { get; set; } = false;
        }

        public List<ArquivoDTO> ListaArquivosMemoria
        {
            get
            {
                if (ViewState["ListaArquivosMemoria"] == null) ViewState["ListaArquivosMemoria"] = new List<ArquivoDTO>();
                return (List<ArquivoDTO>)ViewState["ListaArquivosMemoria"];
            }
            set { ViewState["ListaArquivosMemoria"] = value; }
        }

        public string IdItemEmEdicao
        {
            get { return ViewState["IdItemEmEdicao"] as string ?? "0"; }
            set { ViewState["IdItemEmEdicao"] = value; }
        }

        // 1. Abrir Modal pelo "Novo Lançamento" (Modo Memória)
        protected void btnAbrirAnexos_Lancamento_Click(object sender, EventArgs e)
        {
            if (IdItemEmEdicao != "0")
            {
                IdItemEmEdicao = "0";
                ListaArquivosMemoria = new List<ArquivoDTO>();
            }

            btnSalvarArquivosBanco.Visible = false;
            CarregarGridArquivos();
            AbrirModalArquivos();
        }

        // 2. Adicionar Arquivo na Lista (Memória)
        protected void btnAdicionarArquivoLista_Click(object sender, EventArgs e)
        {
            try
            {
                if (fuArquivo.HasFile)
                {
                    var lista = ListaArquivosMemoria;
                    lista.Add(new ArquivoDTO
                    {
                        Nome = fuArquivo.FileName,
                        Descricao = txtDescArquivo.Text,
                        Bytes = fuArquivo.FileBytes,
                        IdBanco = 0
                    });
                    ListaArquivosMemoria = lista;
                    txtDescArquivo.Text = "";
                    CarregarGridArquivos();
                }
            }
            catch (Exception ex)
            {
                MensagemModalArquivos.MostraMensagem_Erro("Erro: " + ex.Message);
            }
            AbrirModalArquivos();
        }

        // 3. Remover da Lista
        protected void gvArquivos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RemoverTemp")
            {
                string idTemp = e.CommandArgument.ToString();
                var lista = ListaArquivosMemoria;
                var item = lista.FirstOrDefault(x => x.IdTemporario == idTemp);
                if (item != null)
                {
                    if (item.IdBanco > 0) item.MarcadoParaExclusao = true;
                    else lista.Remove(item);
                }
                ListaArquivosMemoria = lista;
                CarregarGridArquivos();
                AbrirModalArquivos();
            }

            if (e.CommandName == "Download")
            {
                string idTemp = e.CommandArgument.ToString();

                // Busca o arquivo na lista em memória
                var arquivo = ListaArquivosMemoria.FirstOrDefault(x => x.IdTemporario == idTemp);

                if (arquivo != null && arquivo.Bytes != null)
                {
                    try
                    {
                        // Limpa a resposta
                        Response.Clear();
                        Response.ClearHeaders();
                        Response.ClearContent();

                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);

                        // Define o cabeçalho
                        Response.ContentType = "application/octet-stream";
                        Response.AddHeader("Content-Disposition", "attachment; filename=" + arquivo.Nome);

                        // Escreve os bytes
                        Response.BinaryWrite(arquivo.Bytes);

                        // Envia para o cliente
                        Response.Flush();

                        // --- O SEGREDO ESTÁ AQUI EMBAIXO ---

                        // Evita que o ASP.NET tente renderizar o resto da página HTML depois do arquivo
                        Response.SuppressContent = true;

                        // Finaliza a requisição sem "matar" a thread violentamente (sem ThreadAbortException)
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                    catch (Exception ex)
                    {
                        MensagemModalArquivos.MostraMensagem_Erro("Erro ao baixar: " + ex.Message);
                        // Se der erro real, reabre a modal
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepOpen", "abrirModalLancamento();", true);
                    }
                }
            }
        }

        // 4. Salvar Definitivo
        private void PersistirArquivosNoBanco(string idItem)
        {
            foreach (var arq in ListaArquivosMemoria)
            {
                if (arq.MarcadoParaExclusao && arq.IdBanco > 0)
                {
                    ExcluirArquivoDoBanco(arq.IdBanco);
                }
                else if (!arq.MarcadoParaExclusao && arq.IdBanco == 0)
                {
                    cls_Arquivos obj = new cls_Arquivos();
                    obj.idTipoArquivo = 8888;
                    obj.idObjeto = int.Parse(idItem);
                    obj.sNomeArquivo = arq.Nome;
                    obj.sDscArquivo = arq.Descricao;
                    obj.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                    obj.vbArquivo = arq.Bytes;
                    obj.EnviarArquivo(obj);
                }
            }
        }

        // 5. Salvar Grid
        protected void btnSalvarArquivosBanco_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ListaArquivosMemoria.Any(x => !x.MarcadoParaExclusao))
                {
                    MensagemModalLanc.MostraMensagem_Erro("A despesa deve ter pelo menos um comprovante. Adicione um antes de salvar ou cancele a exclusão.");
                    // Reabre a modal para mostrar o erro
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepOpen", "abrirModalLancamento();", true);
                    return;
                }

                PersistirArquivosNoBanco(IdItemEmEdicao);
                ListaArquivosMemoria = new List<ArquivoDTO>();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeArq", "fecharModalArquivos();", true);
                MensagemPagina_Entregas.MostraMensagem_Sucesso("Arquivos atualizados com sucesso!");
            }
            catch (Exception ex)
            {
                MensagemModalArquivos.MostraMensagem_Erro("Erro ao salvar: " + ex.Message);
                AbrirModalArquivos();
            }
        }

        private void CarregarGridArquivos()
        {
            gvArquivos.DataSource = ListaArquivosMemoria.Where(x => !x.MarcadoParaExclusao).ToList();
            gvArquivos.DataBind();
        }

        private void AbrirModalArquivos()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenLanc", "abrirModalLancamento();", true);
        }
        private void CarregarArquivosDoBanco(string idItem)
        {
            try
            {
                cls_Arquivos objArq = new cls_Arquivos();

                // Supus que 'ds' é uma variável global da sua classe, já que ela aparece no 'if' abaixo
                // Se o método retorna o DataSet, use: DataSet ds = objArq.ConsultarArquivos(...)
                DataSet ds = objArq.ConsultarArquivos(int.Parse(idItem), "Despesas");

                // 1. Criamos uma lista temporária para receber os dados
                List<ArquivoDTO> listaDoBanco = new List<ArquivoDTO>();

                if (BD.ValidarDataSet(ds))
                {
                    // 2. Percorremos cada linha retornada do banco
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        ArquivoDTO arq = new ArquivoDTO();

                        // --- ÁREA DE MAPEAMENTO (Ajuste os nomes das colunas conforme seu BD) ---

                        // Exemplo: row["NomeDaColunaNoBanco"]
                        arq.IdBanco = Convert.ToInt32(row["idArquivo"]);
                        arq.Nome = row["sNomeArquivo"].ToString();

                        // Verifica se a coluna existe para evitar erro, caso a proc mude
                        if (row.Table.Columns.Contains("sDscArquivo"))
                            arq.Descricao = row["sDscArquivo"].ToString();

                        // CUIDADO: Carregar arquivos (bytes) no ViewState pode deixar a página pesada
                        if (row.Table.Columns.Contains("vbArquivo") && row["vbArquivo"] != DBNull.Value)
                        {
                            arq.Bytes = (byte[])row["vbArquivo"];
                        }
                        // -----------------------------------------------------------------------

                        // Adiciona na lista temporária
                        listaDoBanco.Add(arq);
                    }
                }

                // 3. Atualizamos a propriedade que guarda no ViewState
                ListaArquivosMemoria = listaDoBanco;

                // 4. Agora o Grid é carregado a partir da LISTA DE MEMÓRIA, e não mais do DataSet direto
                // Isso permite que você adicione novos arquivos nessa lista depois sem perder os do banco
                gvArquivos.DataSource = ListaArquivosMemoria;
                gvArquivos.DataBind();

            }
            catch (Exception ex)
            {
                MensagemModalArquivos.MostraMensagem_Erro("Erro ao listar arquivos: " + ex.Message);
            }
        }
        private void ExcluirArquivoDoBanco(int idArquivo)
        {
            // Instancia o Adapter apontando para a procedure específica desta tela e a ConnectionString do Framework
            SqlDataAdapter da = new SqlDataAdapter("sp_Manipula_tbl_Flow_Colaboradores_Relatorio_Despesas", TT.FrameWork.BD.StringDeConexao);

            // Configurações do Comando
            da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
            da.SelectCommand.CommandType = CommandType.StoredProcedure;

            // Adição dos Parâmetros (Tipados explicitamente como no seu exemplo)
            da.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "DOCUMENTO_DELETAR";
            da.SelectCommand.Parameters.Add("@idArquivo", SqlDbType.Int).Value = idArquivo;

            try
            {
                DataSet ds = new DataSet();
                da.Fill(ds); // O método Fill abre a conexão, executa e fecha automaticamente
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir o arquivo (ID: {idArquivo}): {ex.Message}");
            }
        }

        // Método para controlar o estado da tela (Novo vs Edição de Arquivos)
        private void ConfigurarModalLancamento(bool modoEdicaoArquivos)
        {
            // Se for modo edição de arquivos, bloqueia os inputs
            bool habilitarCampos = !modoEdicaoArquivos;

            ddlUsuarioLancamento.Enabled = habilitarCampos;
            txtDataLancamento.Enabled = habilitarCampos;
            ddlTipoLancamento.Enabled = habilitarCampos;
            txtLocalLancamento.Enabled = habilitarCampos;
            txtValorLancamento.Enabled = habilitarCampos;

            // Controla visibilidade dos botões
            btnSalvarLancamento.Visible = habilitarCampos; // Botão "Efetivar"
            btnSalvarArquivosBanco.Visible = modoEdicaoArquivos; // Botão "Salvar Anexos"

            lblTituloModal.Text = modoEdicaoArquivos ? "Gerenciar Anexos (Lançamento Bloqueado)" : "Novo Lançamento";

            // Se estiver desbloqueado, limpa os campos para novo insert
            if (habilitarCampos)
            {
                txtDataLancamento.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtLocalLancamento.Text = "";
                txtValorLancamento.Text = "";
                ddlTipoLancamento.SelectedIndex = 0;
                try { ddlUsuarioLancamento.SelectedIndex = 0; } catch { }
            }
        }

        protected void gvArquivos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Encontra o botão de download na linha atual
                LinkButton btnDownload = (LinkButton)e.Row.FindControl("btnDownload");

                // Registra ele como um gatilho de PostBack completo
                // Isso permite o download de arquivos de dentro do UpdatePanel
                ScriptManager sm = ScriptManager.GetCurrent(this);
                if (sm != null)
                {
                    sm.RegisterPostBackControl(btnDownload);
                }
            }
        }

        private void VerificarRegrasBloqueio()
        {
            bool existemLancamentos = gvLancamentos.Rows.Cast<GridViewRow>().Any(r => r.RowType == DataControlRowType.DataRow);

            // -----------------------------------------------------------------------
            // 1. BLOQUEIO DOS CAMPOS DO PAINEL (CSS + Attributes)
            // -----------------------------------------------------------------------
            if (existemLancamentos)
            {
                // TextBoxes: Use 'readonly'. O Bootstrap entende e estiliza corretamente, 
                // e o usuário consegue copiar o texto se quiser, mas não editar.
                txtDtInicio.Attributes.Add("readonly", "readonly");
                txtsDscMotivo.Attributes.Add("readonly", "readonly");
                txtsObservacao.Attributes.Add("readonly", "readonly");

                // --- Bloqueio dos DropDowns/Listbox (Via Painel Wrapper) ---
                // Usamos 'opacity' para parecer desabilitado e 'pointer-events' para impedir clique.
                // O tabindex -1 impede chegar via teclado.
                string styleBloqueio = "pointer-events: none; opacity: 0.6;";

                pnlColaborador.Attributes.Add("style", styleBloqueio);
                pnlCentroCusto.Attributes.Add("style", styleBloqueio);

                // Bloqueio extra para impedir navegação via teclado nos painéis
                pnlColaborador.Attributes.Add("tabindex", "-1");
                pnlCentroCusto.Attributes.Add("tabindex", "-1");

                // Bloqueia botão de adicionar nova regra (opcional, mas recomendado para consistência)
                //btnAbrirModal.Visible = false;
            }
            else
            {
                // Libera tudo removendo os atributos
                txtDtInicio.Attributes.Remove("readonly");
                txtsDscMotivo.Attributes.Remove("readonly");
                txtsObservacao.Attributes.Remove("readonly");

                // Remove estilos dos painéis
                pnlColaborador.Attributes.Remove("style");
                pnlColaborador.Attributes.Remove("tabindex");

                pnlCentroCusto.Attributes.Remove("style");
                pnlCentroCusto.Attributes.Remove("tabindex");

                btnAbrirModal.Visible = true;
            }

            // O Botão 'Salvar Base' continua habilitado conforme seu pedido.
            btnSalvarBase.Enabled = true;
            btnSalvarBase.CssClass = "btn btn-lg btn-success";

            gvRegras.DataSource = ListaRegras;
            gvRegras.DataBind();
        }
        #endregion

        #region | Ações (Finalizar / Aprovar / Rejeitar)

        protected void btnAcao_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                string idStatus = btn.CommandArgument; // 2=Finalizar, 3=Aprovar, 4=Rejeitar
                string idDespesa = Request.QueryString["id"];

                if (string.IsNullOrEmpty(idDespesa) || idDespesa == "0")
                {
                    MensagemPagina_Entregas.MostraMensagem_Erro("Salve o relatório antes de alterar o status.");
                    return;
                }

                // Lógica para Aprovação (Status 3) -> Gerar Lançamentos Bancários
                // O SQL já cuida da inserção se o status for 3, conforme sua proc 'Finalizar'

                Dictionary<string, string> p = new Dictionary<string, string>();
                p.Add("@sFuncao", "Finalizar");
                p.Add("@idDespesas", idDespesa);
                p.Add("@idStatus", idStatus);
                p.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());

                // Se precisar de parametros extras para o insert bancário (como data atual), o SQL @dtAgora já resolve.
                // Se precisar enviar parametros da tela, adicione aqui.

                BD.ExecutarDataSet(sProcedure, p);

                MensagemPagina_Entregas.MostraMensagem_Sucesso($"Status alterado com sucesso! (Status ID: {idStatus})");

                // Recarrega para atualizar a tela/bloqueios
                CarregarDados(idDespesa);
            }
            catch (Exception ex)
            {
                MensagemPagina_Entregas.MostraMensagem_Erro("Erro ao alterar status: " + ex.Message);
            }
        }

        // Método auxiliar para controlar visibilidade do botão de ações
        private void ConfigurarBotoesAcao(string idDespesa)
        {
            // Só mostra o botão de ações se for uma edição (ID > 0)
            // Você pode adicionar mais regras aqui (ex: se já estiver finalizado, esconder, etc)
            bool isEdicao = !string.IsNullOrEmpty(idDespesa) && idDespesa != "0";
            divAcoes.Visible = isEdicao;
        }

        #endregion

        #region | Historico
        private void CarregarHistorico(string idDespesa)
        {
            try
            {
                Dictionary<string, string> p = new Dictionary<string, string>();
                p.Add("@sFuncao", "CONSULTAR-HISTORICO");
                p.Add("@idDespesas", idDespesa);

                DataSet ds = BD.ExecutarDataSet(sProcedure, p);

                if (ds != null && ds.Tables.Count > 0)
                {
                    gvHistorico.DataSource = ds.Tables[0];
                    gvHistorico.DataBind();
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_Entregas.MostraMensagem_Erro("Erro ao carregar histórico: " + ex.Message);
            }
        }
        #endregion

        protected void ddlUsuarioLancamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            FUNCOES.Popula_Combo(ddlidCartao, "sp_Select 'tbl_Flow_Adm_ContasBancarias_x_Cartao', @idUsuario = " + RecuperarUsuario(ddlUsuarioLancamento.SelectedValue) + "", "idCartao", "sDscCartao", false, "Sem Cartão", "0");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepModalOpen", "abrirModalLancamento();", true);
        }
    }
}