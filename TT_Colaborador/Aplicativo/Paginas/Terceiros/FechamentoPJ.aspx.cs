using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq; // IMPORTANTE PARA O GROUPBY
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Colaborador.FrameWork;
using static TT.FrameWork.Identity;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;

namespace TT_Colaborador.Aplicativo.Paginas.Financeiro
{
    public partial class FechamentoPJ : Page
    {
        string sProcedure = "sp_Manipula_tbl_Flow_Fechamento_PJ_Controle";
        public bool PodeEditar { get; set; } = false;

        // --- CLASSES PARA O AGRUPAMENTO (RESTAURADAS) ---
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
        // -------------------------------------------------

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Variaveis.idUsuario() == "0") FUNCOES.DirecionaPagina("~/Login.aspx");

            FUNCOES.ValidaPermissao(Permissao.TerceirosPJ.Consultar, true, true);

            if (!IsPostBack)
            {
                CarregarCombos();
                ddlMes.SelectedValue = DateTime.Now.Month.ToString();
                ddlAno.SelectedValue = DateTime.Now.Year.ToString();

                CarregarMenuHistorico();

                btnCarregar.Visible = FUNCOES.ValidaPermissao(Permissao.TerceirosPJ.GerarRelatorio, false, false);
            }

            RegistraScriptConfirmacao();
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
                { "@idUsuario", Variaveis.idUsuario().ToString() }
            };

            DataSet ds = BD.ExecutarDataSet(sProcedure, param);

            if (BD.ValidarDataSet(ds, out string erro))
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
            string s = status.ToUpper().Trim();

            bool permEditarAjuste = FUNCOES.ValidaPermissao(Permissao.TerceirosPJ.EditarAjuste, false, false);
            bool permGerarAjuste = FUNCOES.ValidaPermissao(Permissao.TerceirosPJ.GerarAjuste, false, false);
            bool permGerarFechamento = FUNCOES.ValidaPermissao(Permissao.TerceirosPJ.GerarFechamento, false, false);

            hddStatus.Value = s;
            bool isRascunho = (s == "RASCUNHO" || s == "REJEITADO" || s == "REPROVADO");

            if (isRascunho)
            {
                PodeEditar = permEditarAjuste;
                divResetar.Visible = true;
                btnResetar.OnClientClick = $"return abrirConfirmacao('Isso apagará o rascunho atual e buscará os dados novamente. Confirmar?', '{btnResetar.UniqueID}');";
                divBarraFinalizar.Visible = permGerarFechamento;
                divFab.Visible = permGerarAjuste;

                lblStatus.Text = s;
                lblStatus.CssClass = "status-pill bg-warning text-dark";
                if (s == "REJEITADO") lblStatus.CssClass = "status-pill bg-danger text-white";
            }
            else
            {
                PodeEditar = false;
                divResetar.Visible = false;
                divBarraFinalizar.Visible = false;
                divFab.Visible = false;

                lblStatus.Text = s;
                lblStatus.CssClass = "status-pill bg-success text-white";
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
                CarregarMenuHistorico();
                updGeral.Update();
            }
        }

        protected void btnCarregar_Click(object sender, EventArgs e)
        {
            if (!FUNCOES.ValidaPermissao(Permissao.TerceirosPJ.GerarRelatorio, false, true)) return;

            Dictionary<string, string> param = new Dictionary<string, string> {
                { "@sFuncao", "INICIAR_RASCUNHO" },
                { "@idUsuario", Variaveis.idUsuario().ToString() },
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
                MensagemPagina.MostraMensagem_Erro("Erro: " + erro);
                divDashboard.Visible = false;
            }
            CarregarMenuHistorico();
        }

        protected void btnResetar_Click(object sender, EventArgs e)
        {
            if (hddIdFechamento.Value == "0") return;

            Dictionary<string, string> param = new Dictionary<string, string> {
                { "@sFuncao", "LIMPAR_RASCUNHO" },
                { "@idFechamento", hddIdFechamento.Value }
            };
            BD.ExecutarDataSet(sProcedure, param);

            btnCarregar_Click(null, null);
        }

        // --- AQUI ESTAVA O PROBLEMA: LÓGICA DE AGRUPAMENTO RESTAURADA ---
        private void CarregarGridDetalhada(int idFechamento)
        {
            Dictionary<string, string> param = new Dictionary<string, string> {
                { "@sFuncao", "CONSULTAR_GRID" },
                { "@idFechamento", idFechamento.ToString() }
            };

            DataSet ds = BD.ExecutarDataSet(sProcedure, param);

            if (BD.ValidarDataSet(ds, out string erro))
            {
                string idSolicitacaoVinculada = BD.Retorno.DATASET(ds, 0, "idSolicitacao");

                if (idSolicitacaoVinculada != "0" && !string.IsNullOrEmpty(idSolicitacaoVinculada))
                {
                    divAcompanhar.Visible = true;
                    lblIdSolicitacaoCard.Text = idSolicitacaoVinculada;
                    lnkAcompanhar.NavigateUrl = ResolveUrl($"~/Aplicativo/Paginas/Solicitacoes/Solicitacoes.aspx?id={idSolicitacaoVinculada}");
                }
                else
                {
                    divAcompanhar.Visible = false;
                }

                // 1. Carrega a lista plana (Flat List)
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

                // 2. AGRUPAMENTO COM LINQ
                List<DiaFechamento> listaAgrupada = listaPlana
     .GroupBy(x => x.DataRef.Date) // <--- ALTERAÇÃO AQUI: Adicionado .Date para ignorar a hora
     .Select(g => new DiaFechamento
     {
         DataRef = g.Key,
         Dia = g.Key.Day.ToString("00"),
         DiaSemana = g.Key.ToString("ddd", new CultureInfo("pt-BR")).ToUpper(),
         TotalHorasDia = g.Sum(x => x.Horas),
         // Ordena os itens dentro do dia pelo horário original
         Itens = g.OrderBy(x => x.DataRef).ThenBy(x => x.TipoRegistro).ToList()
     })
     .OrderBy(x => x.DataRef)
     .ToList();

                // 3. Bind no Repeater Pai (rptDias)
                // OBS: O HTML Novo usa rptDias, não rptItens
                rptDias.DataSource = listaAgrupada;
                rptDias.DataBind();

                // Totais Gerais
                lblTotalSistema.Text = totalSistema.ToString("N2") + "h";
                lblTotalAjustes.Text = totalAjuste.ToString("N2") + "h";
                lblTotalHoras.Text = (totalSistema + totalAjuste).ToString("N2");

                divDashboard.Visible = true;
            }
        }

        protected void btnSalvarAjuste_Click(object sender, EventArgs e)
        {
            try
            {
                string idEdicao = hddIdItemEdicao.Value;
                bool isInclusao = (idEdicao == "0" || string.IsNullOrEmpty(idEdicao));

                if (isInclusao)
                {
                    if (!FUNCOES.ValidaPermissao(Permissao.TerceirosPJ.GerarAjuste, false, true)) return;
                }
                else
                {
                    if (!FUNCOES.ValidaPermissao(Permissao.TerceirosPJ.EditarAjuste, false, true)) return;
                }

                if (string.IsNullOrEmpty(txtDataAjuste.Text) || string.IsNullOrEmpty(txtHorasAjuste.Text) || string.IsNullOrEmpty(txtObsAjuste.Text))
                    throw new Exception("Preencha todos os campos.");

                string funcao = (idEdicao == "0" || string.IsNullOrEmpty(idEdicao)) ? "INSERIR_AJUSTE" : "ALTERAR_AJUSTE";

                Dictionary<string, string> param = new Dictionary<string, string> {
                    { "@sFuncao", funcao },
                    { "@idFechamento", hddIdFechamento.Value },
                    { "@idItemAjuste", idEdicao },
                    { "@dtReferencia", string.IsNullOrEmpty(txtDataAjuste.Text.Trim()) ? "" : DateTime.Parse(txtDataAjuste.Text.Trim()).ToString("dd/MM/yyyy")},
                    { "@nHorasAjuste", txtHorasAjuste.Text.Replace(",", ".") },
                    { "@sObservacaoAjuste", txtObsAjuste.Text }
                };

                BD.ExecutarDataSet(sProcedure, param);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "fecharModal", "fecharModalAjuste();", true);

                txtObsAjuste.Text = "";
                txtHorasAjuste.Text = "";
                hddIdItemEdicao.Value = "0";

                ConfigurarPermissoes(hddStatus.Value);
                CarregarGridDetalhada(int.Parse(hddIdFechamento.Value));
            }
            catch (Exception ex) { MensagemPagina.MostraMensagem_Erro(ex.Message); }
        }

        // Este método responde ao Command do Repeater INTERNO (rptDetalhes)
        protected void rptItens_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ExcluirAjuste")
            {
                if (!FUNCOES.ValidaPermissao(Permissao.TerceirosPJ.EditarAjuste, false, true)) return;

                Dictionary<string, string> param = new Dictionary<string, string> {
                    { "@sFuncao", "EXCLUIR_ITEM_AJUSTE" },
                    { "@idItemAjuste", e.CommandArgument.ToString() }
                };
                BD.ExecutarDataSet(sProcedure, param);

                ConfigurarPermissoes(hddStatus.Value);
                CarregarGridDetalhada(int.Parse(hddIdFechamento.Value));
            }
            else if (e.CommandName == "EditarAjuste")
            {
                if (!FUNCOES.ValidaPermissao(Permissao.TerceirosPJ.EditarAjuste, false, true)) return;

                string idItem = e.CommandArgument.ToString();

                Dictionary<string, string> param = new Dictionary<string, string> {
                    { "@sFuncao", "CONSULTAR_ITEM" },
                    { "@idItemAjuste", idItem }
                };
                DataSet ds = BD.ExecutarDataSet(sProcedure, param);

                if (BD.ValidarDataSet(ds, out string erro))
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    txtDataAjuste.Text = Convert.ToDateTime(row["dtReferencia"]).ToString("yyyy-MM-dd");
                    txtHorasAjuste.Text = row["nHoras"].ToString().Replace(",", ".");
                    txtObsAjuste.Text = row["sObservacao"].ToString();

                    hddIdItemEdicao.Value = idItem;

                    btnExcluirModal.Visible = true;
                    btnExcluirModal.CommandArgument = idItem;

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "abrirModal", "abrirModalAjuste();", true);
                }
            }
        }

        protected void btnExcluirModal_Click(object sender, EventArgs e)
        {
            if (!FUNCOES.ValidaPermissao(Permissao.TerceirosPJ.EditarAjuste, false, true)) return;

            string idItem = hddIdItemEdicao.Value;
            if (idItem != "0" && !string.IsNullOrEmpty(idItem))
            {
                Dictionary<string, string> param = new Dictionary<string, string> {
                    { "@sFuncao", "EXCLUIR_ITEM_AJUSTE" },
                    { "@idItemAjuste", idItem }
                };
                BD.ExecutarDataSet(sProcedure, param);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "fecharModal", "fecharModalAjuste();", true);
                CarregarGridDetalhada(int.Parse(hddIdFechamento.Value));
            }
        }

        protected void btnFinalizar_Click(object sender, EventArgs e)
        {
            if (!FUNCOES.ValidaPermissao(Permissao.TerceirosPJ.GerarFechamento, false, true)) return;

            try
            {
                Dictionary<string, string> param = new Dictionary<string, string> {
                    { "@sFuncao", "FINALIZAR_ENVIO" },
                    { "@idFechamento", hddIdFechamento.Value },
                    { "@idUsuario", Variaveis.idUsuario().ToString() },
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

                    string mensagem = "Fechamento enviado com sucesso!";
                    if (idSolicitacaoGerada != "0" && !string.IsNullOrEmpty(idSolicitacaoGerada))
                    {
                        string url = ResolveUrl($"~/Aplicativo/Paginas/Solicitacoes/Solicitacoes.aspx?id={idSolicitacaoGerada}");
                        mensagem += $" <div class='mt-2'><a href='{url}' class='btn btn-sm btn-light fw-bold' style='color:#198754' target='_blank'><i class='fa fa-external-link-alt me-1'></i>Ver Solicitação #{idSolicitacaoGerada}</a></div>";
                    }

                    MensagemPagina.MostraMensagem_Sucesso(mensagem);
                    divDashboard.Visible = false;
                    CarregarMenuHistorico();
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro("Erro ao finalizar: " + erro);
                }
            }
            catch (Exception ex) { MensagemPagina.MostraMensagem_Erro(ex.Message); }
        }

        private byte[] GerarRelatorioExcelBytes(int idFechamento)
        {
            Dictionary<string, string> param = new Dictionary<string, string> {
                { "@sFuncao", "CONSULTAR_GRID" },
                { "@idFechamento", idFechamento.ToString() }
            };
            DataSet ds = BD.ExecutarDataSet(sProcedure, param);

            if (!BD.ValidarDataSet(ds, out string erro))
                return null;

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
                TT_Colaborador.FrameWork.cls_Arquivos Arquivo = new TT_Colaborador.FrameWork.cls_Arquivos();
                Arquivo.idTipoArquivo = 0;
                Arquivo.idObjeto = idSolicitacao;
                Arquivo.sNomeArquivo = nomeArquivo;
                Arquivo.sDscArquivo = "Relatório de Fechamento - Automático";
                Arquivo.sObservacao = "Gerado automaticamente no fechamento.";
                Arquivo.idUsuario = Convert.ToInt32(Variaveis.idUsuario());
                Arquivo.vbArquivo = conteudoArquivo;

                DataSet dsItem = EnviarArquivo(Arquivo);
            }
            catch (Exception) { }
        }

        public DataSet EnviarArquivo(cls_Arquivos Arquivo)
        {

            SqlDataAdapter da = new SqlDataAdapter("sp_Manipula_tbl_Flow_Arquivos", TT.FrameWork.BD.StringDeConexao);

            DataSet tabela = new DataSet();
            da.SelectCommand.CommandTimeout = TimeSpan.FromMinutes(120).Seconds;
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            SqlCommand lObjCommand = new SqlCommand();

            string tipo = "7101";

            da.SelectCommand.Parameters.Add("@sFuncao", SqlDbType.VarChar).Value = "INCLUIR";
            da.SelectCommand.Parameters.Add("@idTipoArquivo", SqlDbType.Int).Value = tipo;
            da.SelectCommand.Parameters.Add("@idObjeto", SqlDbType.Int).Value = Arquivo.idObjeto;
            da.SelectCommand.Parameters.Add("@sNomeArquivo", SqlDbType.VarChar).Value = Arquivo.sNomeArquivo;
            da.SelectCommand.Parameters.Add("@sDscArquivo", SqlDbType.VarChar).Value = Arquivo.sDscArquivo;
            da.SelectCommand.Parameters.Add("@sObservacao", SqlDbType.VarChar).Value = Arquivo.sObservacao;
            da.SelectCommand.Parameters.Add("@vbArquivo", SqlDbType.VarBinary).Value = Arquivo.vbArquivo;
            da.SelectCommand.Parameters.Add("@idUsuario", SqlDbType.Int).Value = Arquivo.idUsuario;
            da.SelectCommand.Parameters.Add("@dtExpiracaoDoc", SqlDbType.VarChar).Value = Arquivo.dtExpiracaoDoc;

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

        private void RegistraScriptConfirmacao()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("$(document).ready(function() {");
            sb.Append("  $('#dialog-Confirmacao').dialog({");
            sb.Append("      resizable: false,");
            sb.Append("      height: 'auto',");
            sb.Append("      width: 400,");
            sb.Append("      modal: true,");
            sb.Append("      autoOpen: false,");
            sb.Append("      buttons: {");
            sb.Append("          'Sim': function() {");
            sb.Append("              $(this).dialog('close');");
            sb.Append("              if (typeof window.confirmTargetFunc === 'function') { window.confirmTargetFunc(); }");
            sb.Append("          },");
            sb.Append("          'Não': function() {");
            sb.Append("              $(this).dialog('close');");
            sb.Append("          }");
            sb.Append("      }");
            sb.Append("  });");
            sb.Append("});");

            sb.Append("function abrirConfirmacao(mensagem, uniqueId) {");
            sb.Append("   $('#lblMensagemConfirmacao').text(mensagem);");
            sb.Append("   window.confirmTargetFunc = function() { __doPostBack(uniqueId, ''); };");
            sb.Append("   $('#dialog-Confirmacao').dialog('open');");
            sb.Append("   return false;");
            sb.Append("}");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Confirmacao_Modal", sb.ToString(), true);
        }
    }
}