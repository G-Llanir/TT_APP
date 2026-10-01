using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.FrameWork;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas.RRHH
{
    public partial class FechamentoPJ : System.Web.UI.Page
    {
        string sProcedure = "sp_Manipula_tbl_Flow_Fechamento_PJ_Controle";

        public bool PodeEditar
        {
            get
            {
                if (ViewState["PodeEditar"] == null) return false;
                return (bool)ViewState["PodeEditar"];
            }
            set { ViewState["PodeEditar"] = value; }
        }

        #region Classes Auxiliares
        public class DiaFechamento
        {
            public DateTime DataRef { get; set; }
            public string Dia { get; set; }
            public string DiaSemana { get; set; }
            public decimal TotalHorasDia { get; set; }
            public List<ItemFechamento> Itens { get; set; }
        }

        public class ItemFechamento
        {
            public int IdItem { get; set; }
            public string Dia { get; set; }
            public string DiaSemana { get; set; }
            public string Titulo { get; set; }
            public string Observacao { get; set; }
            public decimal Horas { get; set; }
            public string TipoRegistro { get; set; }
            public DateTime DataRef { get; set; }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.RRHH.TerceirosPJ.Consultar, true);

            if (!IsPostBack)
            {
                CarregarCombos();
                ddlMes.SelectedValue = DateTime.Now.Month.ToString();
                ddlAno.SelectedValue = DateTime.Now.Year.ToString();

                CarregarMenuHistorico();
            }
        }

        public bool ValidarEdicao(object tipoRegistro)
        {
            string sTipo = tipoRegistro != null ? tipoRegistro.ToString() : "";
            return sTipo == "AJUSTE_MANUAL" && this.PodeEditar;
        }

        // --- MÉTODO CENTRALIZADO DE AÇÃO DA CONFIRMAÇÃO ---
        protected void btnConfirmarAcao_Click(object sender, EventArgs e)
        {
            string acao = hddAcaoConfirmacao.Value;
            string id = hddIdConfirmacao.Value;

            switch (acao)
            {
                case "SALVAR":
                    ExecutarSalvarAjuste();
                    break;
                case "EXCLUIR":
                    ExecutarExcluirAjuste(id);
                    break;
                case "RESETAR":
                    ExecutarResetar();
                    break;
                case "FINALIZAR":
                    ExecutarFinalizar();
                    break;
            }

            // Fecha todas as modais após a execução
            ScriptManager.RegisterStartupScript(this, this.GetType(), "fecharTudo", "fecharTodasModais();", true);
        }

        // --- MÉTODOS DE EXECUÇÃO DE LÓGICA ---

        private void ExecutarSalvarAjuste()
        {
            try
            {
                string idEdicao = hddIdItemEdicao.Value;
                bool isInclusao = (idEdicao == "0" || string.IsNullOrEmpty(idEdicao));

                if (isInclusao && !FUNCOES.ValidaPermissao(Permissao.RRHH.TerceirosPJ.GerarAjuste, false))
                {
                    MensagemPagina.MostraMensagem_Erro("Sem permissão para incluir."); return;
                }
                if (!isInclusao && !FUNCOES.ValidaPermissao(Permissao.RRHH.TerceirosPJ.EditarAjuste, false))
                {
                    MensagemPagina.MostraMensagem_Erro("Sem permissão para editar."); return;
                }

                if (string.IsNullOrEmpty(txtDataAjuste.Text) || string.IsNullOrEmpty(txtHorasAjuste.Text) || string.IsNullOrEmpty(txtObsAjuste.Text))
                    throw new Exception("Preencha todos os campos.");

                string funcao = isInclusao ? "INSERIR_AJUSTE" : "ALTERAR_AJUSTE";

                Dictionary<string, string> param = new Dictionary<string, string> {
                    { "@sFuncao", funcao },
                    { "@idFechamento", hddIdFechamento.Value },
                    { "@idItemAjuste", idEdicao },
                    { "@dtReferencia", string.IsNullOrEmpty(txtDataAjuste.Text.Trim()) ? "" : DateTime.Parse(txtDataAjuste.Text.Trim()).ToString("dd/MM/yyyy")},
                    { "@nHorasAjuste", txtHorasAjuste.Text.Replace(",", ".") },
                    { "@sObservacaoAjuste", txtObsAjuste.Text }
                };

                BD.ExecutarDataSet(sProcedure, param);

                txtObsAjuste.Text = "";
                txtHorasAjuste.Text = "";
                hddIdItemEdicao.Value = "0";

                ConfigurarPermissoes(hddStatus.Value);
                CarregarGridDetalhada(int.Parse(hddIdFechamento.Value));
                updGeral.Update(); // Atualiza os cards e lista
            }
            catch (Exception ex) { MensagemPagina.MostraMensagem_Erro(ex.Message); }
        }

        private void ExecutarExcluirAjuste(string idItem)
        {
            if (!FUNCOES.ValidaPermissao(Permissao.RRHH.TerceirosPJ.EditarAjuste, false)) return;

            if (idItem != "0" && !string.IsNullOrEmpty(idItem))
            {
                Dictionary<string, string> param = new Dictionary<string, string> {
                    { "@sFuncao", "EXCLUIR_ITEM_AJUSTE" },
                    { "@idItemAjuste", idItem }
                };
                BD.ExecutarDataSet(sProcedure, param);

                ConfigurarPermissoes(hddStatus.Value);
                CarregarGridDetalhada(int.Parse(hddIdFechamento.Value));
                updGeral.Update(); // Atualiza os cards e lista
            }
        }

        private void ExecutarResetar()
        {
            if (hddIdFechamento.Value == "0") return;

            Dictionary<string, string> param = new Dictionary<string, string> {
                { "@sFuncao", "LIMPAR_RASCUNHO" },
                { "@idFechamento", hddIdFechamento.Value }
            };
            BD.ExecutarDataSet(sProcedure, param);

            btnCarregar_Click(null, null); // Recarrega tudo do zero
        }

        private void ExecutarFinalizar()
        {
            if (!FUNCOES.ValidaPermissao(Permissao.RRHH.TerceirosPJ.GerarFechamento, false))
            {
                MensagemPagina.MostraMensagem_Erro("Sem permissão para finalizar.");
                updGeral.Update(); // Importante atualizar aqui também caso dê erro
                return;
            }

            try
            {
                Dictionary<string, string> param = new Dictionary<string, string> {
            { "@sFuncao", "FINALIZAR_ENVIO" },
            { "@idFechamento", hddIdFechamento.Value },
            { "@idUsuario", TT.FrameWork.Identity.Variaveis.idUsuario().ToString() },
            { "@nMes", ddlMes.SelectedValue },
            { "@nAno", ddlAno.SelectedValue }
        };

                DataSet ds = BD.ExecutarDataSet(sProcedure, param);

                if (BD.ValidarDataSet(ds, out string erro))
                {
                    string idSolicitacaoGerada = BD.Retorno.DATASET(ds, 0, "idSolicitacao");

                    if (idSolicitacaoGerada != "0" && !string.IsNullOrEmpty(idSolicitacaoGerada))
                    {
                        byte[] arquivoBytes = GerarRelatorioExcelBytes(int.Parse(hddIdFechamento.Value));
                        string nomeArquivo = "Relatorio_Fechamento_" + ddlMes.SelectedValue + "_" + ddlAno.SelectedValue + $"_ID{idSolicitacaoGerada}" + ".xls";
                        AnexarRelatorioNaSolicitacao(int.Parse(idSolicitacaoGerada), arquivoBytes, nomeArquivo);
                    }

                    MensagemPagina.MostraMensagem_Sucesso("Fechamento enviado com sucesso! Solicitação #" + idSolicitacaoGerada);
                    divDashboard.Visible = false;
                    CarregarMenuHistorico();

                    // --- CORREÇÃO AQUI ---
                    // Força a atualização do painel principal para exibir a mensagem e ocultar o dashboard
                    updGeral.Update();
                    // ---------------------
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro("Erro ao finalizar: " + erro);
                    updGeral.Update(); // Atualiza para mostrar o erro
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
                updGeral.Update(); // Atualiza para mostrar a exception
            }
        }

        // --- BOTÕES ANTIGOS (Mantidos vazios ou removidos do evento Click no ASPX, mas lógica movida) ---
        protected void btnResetar_Click(object sender, EventArgs e) { }
        protected void btnFinalizar_Click(object sender, EventArgs e) { }
        protected void btnExcluirModal_Click(object sender, EventArgs e) { }
        protected void btnSalvarAjuste_Click(object sender, EventArgs e) { }

        protected void btnCarregar_Click(object sender, EventArgs e)
        {
            if (!FUNCOES.ValidaPermissao(Permissao.RRHH.TerceirosPJ.GerarRelatorio, false))
            {
                MensagemPagina.MostraMensagem_Erro("Você não tem permissão para gerar este relatório.");
                return;
            }

            Dictionary<string, string> param = new Dictionary<string, string> {
                { "@sFuncao", "INICIAR_RASCUNHO" },
                { "@idUsuario", TT.FrameWork.Identity.Variaveis.idUsuario().ToString() },
                { "@nMes", ddlMes.SelectedValue },
                { "@nAno", ddlAno.SelectedValue }
            };

            DataSet ds = BD.ExecutarDataSet(sProcedure, param);

            if (BD.ValidarDataSet(ds, out string erro))
            {
                int idFechamento = Convert.ToInt32(ds.Tables[0].Rows[0]["idFechamento"]);
                hddIdFechamento.Value = idFechamento.ToString();

                ConfigurarPermissoes("RASCUNHO");
                CarregarGridDetalhada(idFechamento);
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao iniciar rascunho: " + erro);
                divDashboard.Visible = false;
            }
            CarregarMenuHistorico();
        }

        private void CarregarCombos()
        {
            for (int i = 1; i <= 12; i++)
            {
                string nomeMes = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i);
                ddlMes.Items.Add(new ListItem(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nomeMes), i.ToString()));
            }
            int anoAtual = DateTime.Now.Year;
            for (int i = anoAtual - 1; i <= anoAtual + 1; i++)
            {
                ddlAno.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }

        private void CarregarMenuHistorico()
        {
            Dictionary<string, string> param = new Dictionary<string, string> {
                { "@sFuncao", "LISTAR_HISTORICO" },
                { "@idUsuario", TT.FrameWork.Identity.Variaveis.idUsuario().ToString() }
            };

            DataSet ds = BD.ExecutarDataSet(sProcedure, param);

            if (BD.ValidarDataSet(ds))
            {
                rptHistorico.DataSource = ds;
                rptHistorico.DataBind();
                divSemHistorico.Visible = false;
            }
            else
            {
                rptHistorico.DataSource = null;
                rptHistorico.DataBind();
                divSemHistorico.Visible = true;
            }
            updMenu.Update();
        }

        private void ConfigurarPermissoes(string status)
        {
            // Mantido EXATAMENTE como no código original
            string s = status.ToUpper().Trim();

            bool permEditarAjuste = FUNCOES.ValidaPermissao(Permissao.RRHH.TerceirosPJ.EditarAjuste, false);
            bool permGerarAjuste = FUNCOES.ValidaPermissao(Permissao.RRHH.TerceirosPJ.GerarAjuste, false);
            bool permGerarFechamento = FUNCOES.ValidaPermissao(Permissao.RRHH.TerceirosPJ.GerarFechamento, false);

            hddStatus.Value = s;
            bool isRascunho = (s == "RASCUNHO" || s == "REJEITADO" || s == "REPROVADO");

            if (isRascunho)
            {
                this.PodeEditar = permEditarAjuste;
                divResetar.Visible = true;
                divBarraFinalizar.Visible = permGerarFechamento;
                divBtnAdicionar.Visible = permGerarAjuste;

                lblStatus.Text = s;
                lblStatus.CssClass = "label label-warning";
                if (s == "REJEITADO") lblStatus.CssClass = "label label-danger";
            }
            else
            {
                this.PodeEditar = false;
                divResetar.Visible = false;
                divBarraFinalizar.Visible = false;
                divBtnAdicionar.Visible = false;

                lblStatus.Text = s;
                lblStatus.CssClass = "label label-success";
            }
        }

        protected void rptHistorico_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "CarregarFechamento")
            {
                string[] args = e.CommandArgument.ToString().Split('|');
                string idFechamento = args[0];
                string sStatus = args[1];

                hddIdFechamento.Value = idFechamento;

                ConfigurarPermissoes(sStatus);
                CarregarGridDetalhada(int.Parse(idFechamento));

                ScriptManager.RegisterStartupScript(this, this.GetType(), "fechaModalHist", "$('#modalHistorico').modal('hide');", true);
            }
        }

        protected void rptItens_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            // APENAS EDITAR AQUI. EXCLUIR É VIA JS -> BTN_CONFIRMAR
            if (e.CommandName == "EditarAjuste")
            {
                if (!FUNCOES.ValidaPermissao(Permissao.RRHH.TerceirosPJ.EditarAjuste, false)) return;

                string idItem = e.CommandArgument.ToString();

                Dictionary<string, string> param = new Dictionary<string, string> {
                    { "@sFuncao", "CONSULTAR_ITEM" },
                    { "@idItemAjuste", idItem }
                };
                DataSet ds = BD.ExecutarDataSet(sProcedure, param);

                if (BD.ValidarDataSet(ds))
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    txtDataAjuste.Text = Convert.ToDateTime(row["dtReferencia"]).ToString("yyyy-MM-dd");
                    txtHorasAjuste.Text = row["nHoras"].ToString().Replace(",", ".");
                    txtObsAjuste.Text = row["sObservacao"].ToString();

                    hddIdItemEdicao.Value = idItem;

                    btnExcluirModal.Visible = true;
                    btnExcluirModal.CommandArgument = idItem; // Mantém referência visual

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "abrirModal", "abrirModalAjuste();", true);
                }
            }
        }

        private void CarregarGridDetalhada(int idFechamento)
        {
            Dictionary<string, string> param = new Dictionary<string, string> {
                { "@sFuncao", "CONSULTAR_GRID" },
                { "@idFechamento", idFechamento.ToString() }
            };

            DataSet ds = BD.ExecutarDataSet(sProcedure, param);

            if (BD.ValidarDataSet(ds))
            {
                string idSolicitacaoVinculada = BD.Retorno.DATASET(ds, 0, "idSolicitacao");

                if (idSolicitacaoVinculada != "0" && !string.IsNullOrEmpty(idSolicitacaoVinculada))
                {
                    divAcompanhar.Visible = true;
                    lblIdSolicitacaoCard.Text = idSolicitacaoVinculada;
                    lnkAcompanhar.NavigateUrl = $"https://ac.tecandtec.com.br/Aplicativo/Paginas/Solicitacoes/Solicitacoes.aspx?id={idSolicitacaoVinculada}";
                }
                else
                {
                    divAcompanhar.Visible = false;
                }

                List<ItemFechamento> listaPlana = new List<ItemFechamento>();
                decimal totalSistema = 0, totalAjuste = 0;

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DateTime dt = Convert.ToDateTime(row["dtReferencia"]);
                    decimal horas = Convert.ToDecimal(row["nHoras"]);
                    string tipo = row["sTipoRegistro"].ToString();

                    if (tipo == "SISTEMA") totalSistema += horas; else totalAjuste += horas;

                    listaPlana.Add(new ItemFechamento
                    {
                        IdItem = Convert.ToInt32(row["idItem"]),
                        DataRef = dt,
                        Dia = dt.Day.ToString("00"),
                        DiaSemana = dt.ToString("ddd", new CultureInfo("pt-BR")).ToUpper(),
                        TipoRegistro = tipo,
                        Titulo = tipo == "SISTEMA" ? "Horas Apontadas" : "Ajuste Manual",
                        Observacao = row["sObservacao"].ToString(),
                        Horas = horas
                    });
                }

                List<DiaFechamento> listaAgrupada = listaPlana
                     .GroupBy(x => x.DataRef.Date)
                     .Select(g => new DiaFechamento
                     {
                         DataRef = g.Key,
                         Dia = g.Key.Day.ToString("00"),
                         DiaSemana = g.Key.ToString("ddd", new CultureInfo("pt-BR")).ToUpper(),
                         TotalHorasDia = g.Sum(x => x.Horas),
                         Itens = g.OrderBy(x => x.DataRef).ThenBy(x => x.TipoRegistro).ToList()
                     })
                     .OrderBy(x => x.DataRef)
                     .ToList();

                rptDias.DataSource = listaAgrupada;
                rptDias.DataBind();

                lblTotalSistema.Text = totalSistema.ToString("N2") + "h";
                lblTotalAjustes.Text = totalAjuste.ToString("N2") + "h";
                lblTotalHoras.Text = (totalSistema + totalAjuste).ToString("N2");

                divDashboard.Visible = true;
            }
        }

        private byte[] GerarRelatorioExcelBytes(int idFechamento)
        {
            Dictionary<string, string> param = new Dictionary<string, string> {
                { "@sFuncao", "CONSULTAR_GRID" },
                { "@idFechamento", idFechamento.ToString() }
            };
            DataSet ds = BD.ExecutarDataSet(sProcedure, param);

            if (!BD.ValidarDataSet(ds, out string erro)) return null;

            StringBuilder lSbExcel = new StringBuilder();
            lSbExcel.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            lSbExcel.Append("<style>");
            lSbExcel.Append("body { font-family: Arial, sans-serif; font-size: 12px; }");
            lSbExcel.Append("table { border-collapse: collapse; width: 100%; }");
            lSbExcel.Append("th { background-color: #0d6efd; color: white; border: 1px solid #999; padding: 5px; text-align: center; }");
            lSbExcel.Append("td { border: 1px solid #999; padding: 5px; text-align: center; }");
            lSbExcel.Append(".titulo { font-size: 18px; font-weight: bold; margin-bottom: 10px; text-align: center; }");
            lSbExcel.Append(".assinatura-box { margin-top: 80px; width: 100%; text-align: center; }");
            lSbExcel.Append(".linha-assinatura { border-top: 1px solid #000; width: 40%; margin: 0 auto; display: inline-block; padding-top: 5px; }");
            lSbExcel.Append("</style>");

            lSbExcel.Append("<div class='titulo'>Relatório de Fechamento de Horas</div>");
            lSbExcel.AppendFormat("<p>Gerado em: {0}</p>", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            lSbExcel.Append("<hr />");

            lSbExcel.Append("<table><thead><tr>");
            lSbExcel.Append("<th>Data</th>");
            lSbExcel.Append("<th>Dia</th>");
            lSbExcel.Append("<th>Tipo</th>");
            lSbExcel.Append("<th>Horas</th>");
            lSbExcel.Append("<th>Observação</th>");
            lSbExcel.Append("</tr></thead><tbody>");

            decimal totalHoras = 0;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                DateTime dt = Convert.ToDateTime(row["dtReferencia"]);
                decimal horas = Convert.ToDecimal(row["nHoras"]);
                string tipo = row["sTipoRegistro"].ToString();
                string obs = row["sObservacao"].ToString();

                totalHoras += horas;

                lSbExcel.Append("<tr>");
                lSbExcel.AppendFormat("<td>{0}</td>", dt.ToString("dd/MM/yyyy"));
                lSbExcel.AppendFormat("<td>{0}</td>", dt.ToString("ddd", new CultureInfo("pt-BR")).ToUpper());
                lSbExcel.AppendFormat("<td>{0}</td>", tipo);
                lSbExcel.AppendFormat("<td>{0:N2}</td>", horas);
                lSbExcel.AppendFormat("<td style='text-align:left;'>{0}</td>", obs);
                lSbExcel.Append("</tr>");
            }

            lSbExcel.AppendFormat("<tr style='background-color: #eee; font-weight:bold;'>");
            lSbExcel.Append("<td colspan='3' style='text-align:right;'>TOTAL:</td>");
            lSbExcel.AppendFormat("<td>{0:N2}</td>", totalHoras);
            lSbExcel.Append("<td></td>");
            lSbExcel.Append("</tr>");
            lSbExcel.Append("</tbody></table>");

            lSbExcel.Append("<div class='assinatura-box'>");
            lSbExcel.Append("<br /><br /><br />");
            lSbExcel.Append("<div class='linha-assinatura'>Assinatura do Prestador / Responsável</div>");
            lSbExcel.Append("</div>");

            return Encoding.UTF8.GetBytes(lSbExcel.ToString());
        }

        private void AnexarRelatorioNaSolicitacao(int idSolicitacao, byte[] conteudoArquivo, string nomeArquivo)
        {
            if (conteudoArquivo == null || conteudoArquivo.Length == 0) return;

            try
            {
                cls_Arquivos Arquivo = new cls_Arquivos();
                Arquivo.idTipoArquivo = 0;
                Arquivo.idObjeto = idSolicitacao;
                Arquivo.sNomeArquivo = nomeArquivo;
                Arquivo.sDscArquivo = "Relatório de Fechamento - Automático";
                Arquivo.sObservacao = "Gerado automaticamente no fechamento.";
                Arquivo.idUsuario = Convert.ToInt32(TT.FrameWork.Identity.Variaveis.idUsuario());
                Arquivo.vbArquivo = conteudoArquivo;

                EnviarArquivo(Arquivo);
            }
            catch (Exception) { }
        }

        public DataSet EnviarArquivo(cls_Arquivos Arquivo)
        {
            SqlDataAdapter da = new SqlDataAdapter("sp_Manipula_tbl_Flow_Arquivos", TT.FrameWork.BD.StringDeConexao);
            DataSet tabela = new DataSet();
            da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
            da.SelectCommand.CommandType = CommandType.StoredProcedure;

            string tipo = "7101";

            da.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "INCLUIR";
            da.SelectCommand.Parameters.Add("@idTipoArquivo", SqlDbType.Int).Value = tipo;
            da.SelectCommand.Parameters.Add("@idObjeto", SqlDbType.Int).Value = Arquivo.idObjeto;
            da.SelectCommand.Parameters.Add("@sNomeArquivo", SqlDbType.VarChar).Value = Arquivo.sNomeArquivo;
            da.SelectCommand.Parameters.Add("@sDscArquivo", SqlDbType.VarChar).Value = Arquivo.sDscArquivo;
            da.SelectCommand.Parameters.Add("@sObservacao", SqlDbType.VarChar).Value = Arquivo.sObservacao;
            da.SelectCommand.Parameters.Add("@vbArquivo", SqlDbType.VarBinary).Value = Arquivo.vbArquivo;
            da.SelectCommand.Parameters.Add("@idUsuario", SqlDbType.Int).Value = Arquivo.idUsuario;

            try
            {
                da.Fill(tabela);
                return tabela;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Erro BD-DS: {0}", ex.Message));
            }
        }
    }
}