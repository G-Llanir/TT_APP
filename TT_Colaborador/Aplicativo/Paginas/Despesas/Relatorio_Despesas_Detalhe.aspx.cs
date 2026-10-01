using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT_Colaborador.FrameWork;
using BD = TT.FrameWork.BD;
using IDENTITY = TT.FrameWork.Identity;

namespace TT_Colaborador.Aplicativo.Paginas.Despesas
{
    public partial class Relatorio_Despesas_Detalhe : Page
    {
        static string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Relatorio_Despesas";

        // Mantém o ID na ViewState para não perder entre PostBacks
        public string IdDespesaAtual
        {
            get { return ViewState["IdDespesaAtual"] as string ?? "0"; }
            set { ViewState["IdDespesaAtual"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Pega ID da URL
                if (Request.QueryString["id"] != null)
                {
                    IdDespesaAtual = Request.QueryString["id"];
                    CarregarDadosIniciais();
                }
                else
                {
                    // Se não tiver ID, volta para a listagem (opcional)
                    Response.Redirect("Relatorio_Despesas.aspx");
                }

                // Data padrão hoje
                txtData.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        private void CarregarDadosIniciais()
        {
            try
            {
                // 1. Carrega Header (Dados da Obra/Centro de Custo)
                Dictionary<string, string> p = new Dictionary<string, string>();
                p.Add("@sFuncao", "CONSULTAR_DETALHE");
                p.Add("@idDespesas", IdDespesaAtual);
                DataSet ds = BD.ExecutarDataSet(sProcedure, p);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    lblMotivo.Text = dr["sDscMotivo"].ToString();
                    lblCentroCusto.Text = "CC " + dr["idPedido"].ToString(); // Ajuste conforme seu retorno real

                    DateTime dtIni = dr["dtInicioVigencia"] != DBNull.Value ? Convert.ToDateTime(dr["dtInicioVigencia"]) : DateTime.Now;
                    DateTime dtFim = dr["dtFimVigencia"] != DBNull.Value ? Convert.ToDateTime(dr["dtFimVigencia"]) : DateTime.Now;
                    lblPeriodo.Text = $"{dtIni:dd/MM} a {dtFim:dd/MM/yyyy}";

                    // --- AQUI ESTAVA FALTANDO: SALVAR NO VIEWSTATE ---
                    ViewState["DtInicio"] = dtIni;
                    ViewState["DtFim"] = dtFim;

                    // STATUS (Novo)
                    string status = dr["idStatus"].ToString();
                    lblStatus.Text = status == "1" ? "Aberto" : status == "2" ? "Finalizado" : status == "3" ? "Aprovado" : "Outro";
                    lblStatus.CssClass = status == "1" ? "badge bg-warning text-dark" : "badge bg-success";
                }

                // 2. Popula Combo de Tipos (Apenas se estiver vazio)
                if (ddlTipo.Items.Count <= 1)
                {
                    Dictionary<string, string> pTipos = new Dictionary<string, string> { { "@sFuncao", "FLOW-TIPOS" } };
                    DataSet dsTipos = BD.ExecutarDataSet(sProcedure, pTipos);

                    ddlTipo.DataSource = dsTipos.Tables[0];
                    ddlTipo.DataTextField = "sDscGasto";
                    ddlTipo.DataValueField = "idTipoGastos";
                    ddlTipo.DataBind();

                    ddlTipo.Items.Insert(0, new ListItem("Selecione...", "0"));
                }

                // 3. Carrega Lista de Lançamentos
                CarregarListaLancamentos();
            }
            catch (Exception ex)
            {
                // Tratar erro silenciosamente ou exibir alert
            }
        }

        private void CarregarListaLancamentos()
        {
            try
            {
                Dictionary<string, string> p = new Dictionary<string, string>();
                p.Add("@sFuncao", "CONSULTAR_LANCAMENTOS-POR-ID");
                p.Add("@idDespesas", IdDespesaAtual);

                // PASSAMOS O USUÁRIO LOGADO PARA O SQL FILTRAR PELO IDCOLABORADOR
                p.Add("@idUsuario", IDENTITY.Variaveis.idUsuario().ToString());

                DataSet ds = BD.ExecutarDataSet(sProcedure, p);

                if (ds.Tables.Count > 0)
                {
                    // Calcula Total
                    decimal total = 0;
                    foreach (DataRow r in ds.Tables[0].Rows)
                        total += Convert.ToDecimal(r["nValor"]);

                    lblTotalMeu.Text = "R$ " + total.ToString("N2");

                    // Binda no Repeater
                    rptLancamentos.DataSource = ds.Tables[0];
                    rptLancamentos.DataBind();
                }
            }
            catch { }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validações Servidor
                if (ddlTipo.SelectedValue == "0") 
                {
                    MensagemPagina.MostraMensagem("Selecione o Tipo!", "ERRO", false);
                    return;
                }
                if (string.IsNullOrEmpty(txtValor.Text)) 
                { 
                    MensagemPagina.MostraMensagem("Informe o Valor!", "ERRO", false);
                    return;
                }
                if (string.IsNullOrEmpty(txtLocal.Text)) 
                { 
                    MensagemPagina.MostraMensagem("Informe o Local/Descrição!", "ERRO", false);
                    return;
                }

                if (ListaArquivosMemoria.Count == 0)
                {
                    MensagemPagina.MostraMensagem("Anexe pelo menos um comprovante!", "ERRO", false);
                    return;
                }

                // 2. Validação de Vigência (Adicione antes de salvar)
                DateTime dtLanc;
                if (DateTime.TryParse(txtData.Text, out dtLanc) && ViewState["DtInicio"] != null)
                {
                    DateTime dtIni = (DateTime)ViewState["DtInicio"];
                    DateTime dtFim = (DateTime)ViewState["DtFim"];
                    if (dtLanc.Date < dtIni.Date || dtLanc.Date > dtFim.Date)
                    {
                        MensagemPagina.MostraMensagem($"Data fora da vigência ({dtIni:dd/MM} a {dtFim:dd/MM}).", "ERRO", false);
                        return;
                    }
                }

                Dictionary<string, string> p = new Dictionary<string, string>();
                // MUDANÇA: Chama a nova função da procedure que faz o lookup do Colaborador
                p.Add("@sFuncao", "INSERIR_DESPESAS_AREACOLABORADOR");
                p.Add("@idDespesas", IdDespesaAtual);
                p.Add("@sTipoItem", "L");
                p.Add("@idTipoGastos", ddlTipo.SelectedValue);

                string valorClean = txtValor.Text.Replace("R$", "").Trim().Replace(".", "").Replace(",", ".");
                p.Add("@nValor", valorClean);

                // Data
                if (!string.IsNullOrEmpty(txtData.Text))
                    p.Add("@dtDespesa", DateTime.Parse(txtData.Text).ToString("dd/MM/yyyy HH:mm:ss"));
                else
                    p.Add("@dtDespesa", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                p.Add("@sLocal", txtLocal.Text);

                // Pega ID do usuário logado (O SQL vai gravar na coluna sidParticipantes)
                string idUser = IDENTITY.Variaveis.idUsuario().ToString();
                p.Add("@idUsuario", idUser);

                // Campos padrão
                p.Add("@sFrequencia", "");
                p.Add("@nQtdDias", "0");
                p.Add("@idFormaPagamento", "0");
                p.Add("@idCategoriaPagar", "0");
                p.Add("@sidParticipantes", ""); // Pode ir vazio, a proc calcula

                // Executa
                DataSet ds = BD.ExecutarDataSet(sProcedure, p);

                // Upload de Arquivo              
                    string idGerado = "0";
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Columns.Contains("idItens"))
                        idGerado = ds.Tables[0].Rows[0]["idItens"].ToString();

                    if (idGerado != "0")
                    {
                        foreach (var arq in ListaArquivosMemoria)
                        {
                            cls_Arquivos obj = new cls_Arquivos();
                            obj.idTipoArquivo = 8888;
                            obj.idObjeto = int.Parse(idGerado);
                            obj.sNomeArquivo = arq.Nome;
                            obj.sDscArquivo = ddlTipo.SelectedItem.Text;
                            obj.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                            obj.vbArquivo = arq.Bytes;
                            obj.EnviarArquivoDespesas(obj);
                        }
                        ListaArquivosMemoria = new List<ArquivoDTO>(); // Limpa lista
                    }
                

                // Limpa campos
                txtValor.Text = "";
                txtLocal.Text = "";
                ddlTipo.SelectedValue = "0";

                // Limpa a memória e o Repeater visualmente
                ListaArquivosMemoria = new List<ArquivoDTO>();
                rptArquivosNovos.DataSource = null;
                rptArquivosNovos.DataBind();
                lblQtdArquivos.Text = ""; // Limpa o texto de contagem

                // Reseta o input file via JS
                ScriptManager.RegisterStartupScript(this, GetType(), "ResetFile", "$('#lblFileName').text('Anexar Foto/Comprovante...');", true);

                // Recarrega lista
                CarregarListaLancamentos();

                // Feedback
                MensagemPagina.MostraMensagem("Lançamento realizado com sucesso!", "SUCESSO", false);
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem("Erro ao salvar: " + ex.Message, "ERRO",false);
            }
        }

        protected void rptLancamentos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Excluir")
            {
                try
                {
                    string idItem = e.CommandArgument.ToString();
                    Dictionary<string, string> p = new Dictionary<string, string>();
                    p.Add("@sFuncao", "EXCLUIR_DESPESAS");
                    p.Add("@idItens", idItem);
                    BD.ExecutarDataSet(sProcedure, p);

                    CarregarListaLancamentos();
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem("Erro ao salvar: " + ex.Message, "ERRO", false);
                }
            }
            if (e.CommandName == "VerArquivos")
            {
                try
                {
                    string idItem = e.CommandArgument.ToString();
                    cls_Arquivos objArq = new cls_Arquivos();

                    // 1. Busca no banco
                    DataSet ds = objArq.ConsultarArquivos(int.Parse(idItem), "Despesas");

                    // 2. Transforma em Lista de Memória (IGUAL AO TFLOW)
                    var listaParaMemoria = new List<ArquivoDTO>();

                    if (BD.ValidarDataSet(ds))
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            ArquivoDTO arq = new ArquivoDTO();
                            arq.IdBanco = Convert.ToInt32(row["idArquivo"]);
                            // TRUQUE: O IdTemporario vira o ID do Banco para o botão de download achar fácil
                            arq.IdTemporario = row["idArquivo"].ToString();
                            arq.Nome = row["sNomeArquivo"].ToString();

                            // A procedure PRECISA retornar 'vbArquivo' (bytes)
                            if (row.Table.Columns.Contains("vbArquivo") && row["vbArquivo"] != DBNull.Value)
                            {
                                arq.Bytes = (byte[])row["vbArquivo"];
                            }
                            listaParaMemoria.Add(arq);
                        }
                    }

                    // 3. Joga na ViewState
                    ListaArquivosMemoria = listaParaMemoria;

                    // 4. Binda a grid com a LISTA
                    gvArquivosHistorico.DataSource = ListaArquivosMemoria;
                    gvArquivosHistorico.DataBind();

                    // Abre Modal
                    updModalArquivos.Update();
                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenModal", "abrirModalArquivos();", true);
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem("Erro: " + ex.Message, "ERRO", false);
                }
            }
        }

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

        protected void btnAdicionarArquivo_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile)
            {
                var lista = ListaArquivosMemoria;
                lista.Add(new ArquivoDTO { Nome = fileUpload.FileName, Bytes = fileUpload.FileBytes });
                ListaArquivosMemoria = lista;

                rptArquivosNovos.DataSource = lista;
                rptArquivosNovos.DataBind();
                lblQtdArquivos.Text = $"{lista.Count} arquivo(s).";
            }
            // Script para limpar texto do input
            ScriptManager.RegisterStartupScript(this, GetType(), "Reset", "$('#lblFileName').text('Selecionar...');", true);
        }

        // Novo método gvArquivosHistorico_RowDataBound (Para registrar o download no UpdatePanel)
        protected void gvArquivosHistorico_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton btn = (LinkButton)e.Row.FindControl("btnDownload");
                ScriptManager.GetCurrent(this)?.RegisterPostBackControl(btn);
            }
        }

        // Novo método gvArquivosHistorico_RowCommand (Para baixar o arquivo do banco)
        protected void gvArquivosHistorico_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Download")
            {
                string idAlvo = e.CommandArgument.ToString();

                // Busca na memória (que foi populada no passo anterior)
                var arquivo = ListaArquivosMemoria.FirstOrDefault(x => x.IdTemporario == idAlvo);

                if (arquivo != null && arquivo.Bytes != null)
                {
                    try
                    {
                        Response.Clear();
                        Response.ClearHeaders();
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);

                        Response.ContentType = "application/octet-stream";
                        Response.AddHeader("Content-Disposition", "attachment; filename=" + arquivo.Nome);

                        Response.BinaryWrite(arquivo.Bytes);
                        Response.Flush();

                        // O SEGREDO DO TFLOW
                        Response.SuppressContent = true;
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                    catch (Exception ex)
                    {
                        // Log de erro silencioso ou ScriptManager alert
                    }
                }
            }
        }

        protected void rptArquivosNovos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Remover")
            {
                string idTemp = e.CommandArgument.ToString();
                var lista = ListaArquivosMemoria;

                // Remove o item da lista em memória pelo ID Temporário
                var item = lista.FirstOrDefault(x => x.IdTemporario == idTemp);
                if (item != null)
                {
                    lista.Remove(item);
                }

                // Atualiza a ViewState e o Repeater
                ListaArquivosMemoria = lista;
                rptArquivosNovos.DataSource = lista;
                rptArquivosNovos.DataBind();

                lblQtdArquivos.Text = lista.Count > 0 ? $"{lista.Count} arquivo(s)." : "";
            }
        }
    }
}