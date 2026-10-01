using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using TT.FrameWork;
using BD = TT.FrameWork.BD;
using FLOW_IDENTITY = TT_Flow.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;

namespace TT_Flow.App.Controles
{
    public partial class MensagensDropdown : UserControl
    {
        private const int LimiteMensagens = 5;
        private static readonly CultureInfo CulturaPtBr = CultureInfo.GetCultureInfo("pt-BR");

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            bool podeConsultar = FUNCOES.ValidaPermissao(Permissao.Mensagens.Consultar, false);
            phDropdown.Visible = podeConsultar;
            if (!podeConsultar)
            {
                return;
            }

            ConfigurarLinks();
            CarregarMensagens();
        }

        protected string Codificar(object valor)
        {
            return HttpUtility.HtmlEncode(Convert.ToString(valor) ?? string.Empty);
        }

        protected string CodificarAssunto(object valor)
        {
            string assunto = Convert.ToString(valor);
            return HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(assunto) ? "(Sem assunto)" : assunto.Trim());
        }

        protected string FormatarDataResumida(object valor)
        {
            if (valor == null || valor == DBNull.Value || !DateTime.TryParse(valor.ToString(), out DateTime data))
            {
                return string.Empty;
            }

            DateTime hoje = DateTime.Today;
            if (data.Date == hoje)
            {
                return data.ToString("HH:mm", CulturaPtBr);
            }

            if (data.Date == hoje.AddDays(-1))
            {
                return "Ontem";
            }

            return data.Year == hoje.Year
                ? data.ToString("dd/MM", CulturaPtBr)
                : data.ToString("dd/MM/yyyy", CulturaPtBr);
        }

        protected string ObterClasseItem(object tipo)
        {
            return EhAviso(tipo)
                ? "messages-dropdown-item-card messages-dropdown-item-alert"
                : "messages-dropdown-item-card messages-dropdown-item-message";
        }

        protected string ObterClasseLinha(int indice)
        {
            return indice < LimiteMensagens
                ? "messages-dropdown-item is-visible"
                : "messages-dropdown-item";
        }

        protected string ObterTipoFiltro(object tipo)
        {
            return EhAviso(tipo) ? "aviso" : "mensagem";
        }

        protected string ObterClasseIcone(object tipo)
        {
            return EhAviso(tipo) ? "fa fa-bullhorn" : "fa fa-envelope-o";
        }

        protected string ObterRotuloTipo(object tipo)
        {
            return EhAviso(tipo) ? "Aviso" : "Mensagem";
        }

        protected string MontarUrlDetalhe(object idMensagem)
        {
            return int.TryParse(Convert.ToString(idMensagem), out int id) && id > 0
                ? ResolveUrl($"~/App/Paginas/Mensagem/Mensagens_Detalhe.aspx?id={id}")
                : ResolveUrl("~/App/Paginas/Mensagem/Mensagens.aspx");
        }

        private void ConfigurarLinks()
        {
            lnkVerTodas.NavigateUrl = ResolveUrl("~/App/Paginas/Mensagem/Mensagens.aspx");
            lnkNovaMensagem.NavigateUrl = ResolveUrl("~/App/Paginas/Mensagem/Mensagens_Detalhe.aspx");
            lnkNovaMensagem.Visible = FUNCOES.ValidaPermissao(Permissao.Mensagens.Enviar, false);
        }

        private void CarregarMensagens()
        {
            try
            {
                DataSet ds = BD.ExecutarDataSet(
                    "sp_Flow_Mensagens_Dropdown",
                    new Dictionary<string, string>
                    {
                        { "@idUsuario", IDENTITY.Variaveis.idUsuario() },
                        { "@nLimite", LimiteMensagens.ToString(CultureInfo.InvariantCulture) }
                    },
                    false);

                if (ds == null || ds.Tables.Count < 2 || ds.Tables[0].Rows.Count == 0)
                {
                    throw new InvalidOperationException("A consulta do dropdown não retornou os result sets esperados.");
                }

                int totalMensagens = ObterInteiro(ds.Tables[0].Rows[0], "nNaoLidasMensagens");
                int totalAvisos = ObterInteiro(ds.Tables[0].Rows[0], "nNaoLidasAvisos");
                int totalNaoLidas = ObterInteiro(ds.Tables[0].Rows[0], "nNaoLidasTotal");
                AplicarContador(totalNaoLidas, totalMensagens, totalAvisos);

                DataTable mensagens = ds.Tables[1];
                bool possuiMensagens = mensagens.Rows.Count > 0;
                phFiltros.Visible = possuiMensagens;
                phLista.Visible = possuiMensagens;
                phVazio.Visible = !possuiMensagens;
                phFalha.Visible = false;

                rptMensagens.DataSource = possuiMensagens ? mensagens : null;
                rptMensagens.DataBind();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceWarning("sp_Flow_Mensagens_Dropdown: " + ex.Message);

                int totalFallback = ConverterInteiro(FLOW_IDENTITY.Variaveis.nQtdMensagens_NaoLidas());
                AplicarContador(totalFallback, null, null);
                phFiltros.Visible = false;
                phLista.Visible = false;
                phVazio.Visible = false;
                phFalha.Visible = true;
                rptMensagens.DataSource = null;
                rptMensagens.DataBind();
            }
        }

        private void AplicarContador(int totalNaoLidas, int? totalMensagens, int? totalAvisos)
        {
            totalNaoLidas = Math.Max(0, totalNaoLidas);
            lblBadge.Visible = totalNaoLidas > 0;
            lblBadge.Text = totalNaoLidas > 99 ? "99+" : totalNaoLidas.ToString(CulturaPtBr);
            lblResumo.Text = FormatarQuantidade(totalNaoLidas, "não lida", "não lidas");
            if (totalMensagens.HasValue && totalAvisos.HasValue)
            {
                lblResumo.Text += " · "
                    + FormatarQuantidade(totalMensagens.Value, "mensagem", "mensagens")
                    + " · "
                    + FormatarQuantidade(totalAvisos.Value, "aviso", "avisos");
            }

            lnkAbrirDropdown.Attributes["aria-label"] = totalNaoLidas == 1
                ? "Mensagens, 1 não lida"
                : $"Mensagens, {totalNaoLidas.ToString("N0", CulturaPtBr)} não lidas";
        }

        private static string FormatarQuantidade(int quantidade, string singular, string plural)
        {
            return quantidade.ToString("N0", CulturaPtBr) + " " + (quantidade == 1 ? singular : plural);
        }

        private static int ObterInteiro(DataRow linha, string coluna)
        {
            return linha.Table.Columns.Contains(coluna)
                ? ConverterInteiro(linha[coluna])
                : 0;
        }

        private static int ConverterInteiro(object valor)
        {
            return int.TryParse(Convert.ToString(valor), out int numero) ? numero : 0;
        }

        private static bool EhAviso(object tipo)
        {
            return string.Equals(Convert.ToString(tipo), "AVISO", StringComparison.OrdinalIgnoreCase);
        }
    }
}
