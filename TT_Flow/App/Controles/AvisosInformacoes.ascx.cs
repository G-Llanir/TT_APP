using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;

namespace TT_Flow.App.Controles
{
    public partial class AvisosInformacoes : UserControl
    {
        private const string DefaultTituloPainel = "Mensagens/Informa\u00e7\u00f5es";
        private const string TextoVazio = "Nenhum aviso n\u00e3o lido.";
        private const string TextoVerTodos = "Ver todos os avisos";
        private const int DefaultLimiteRegistros = 20;

        public int LimiteRegistros
        {
            get
            {
                object valor = ViewState["LimiteRegistros"];
                return valor == null ? DefaultLimiteRegistros : Convert.ToInt32(valor);
            }
            set => ViewState["LimiteRegistros"] = value > 0 ? value : DefaultLimiteRegistros;
        }

        public string TituloPainel
        {
            get
            {
                object valor = ViewState["TituloPainel"];
                return valor == null ? DefaultTituloPainel : valor.ToString();
            }
            set => ViewState["TituloPainel"] = string.IsNullOrWhiteSpace(value) ? DefaultTituloPainel : value.Trim();
        }

        public bool ExibirLinkVerTodos
        {
            get
            {
                object valor = ViewState["ExibirLinkVerTodos"];
                return valor == null || Convert.ToBoolean(valor);
            }
            set => ViewState["ExibirLinkVerTodos"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!FUNCOES.ValidaPermissao(Permissao.Mensagens.Consultar, false))
            {
                pnlAvisos.Visible = false;
                return;
            }

            pnlAvisos.Visible = true;
            AplicarConfiguracaoPainel();
            CarregarAvisos();
        }

        public void Recarregar()
        {
            if (!FUNCOES.ValidaPermissao(Permissao.Mensagens.Consultar, false))
            {
                pnlAvisos.Visible = false;
                return;
            }

            pnlAvisos.Visible = true;
            AplicarConfiguracaoPainel();
            CarregarAvisos();
            upAvisos.Update();
        }

        protected string FormatarData(object valorData)
        {
            if (valorData == null || valorData == DBNull.Value)
            {
                return string.Empty;
            }

            if (DateTime.TryParse(valorData.ToString(), out DateTime data))
            {
                return data.ToString("dd/MM/yyyy", CultureInfo.GetCultureInfo("pt-BR"))
                    + " \u00e0s "
                    + data.ToString("HH:mm:ss", CultureInfo.GetCultureInfo("pt-BR"));
            }

            return valorData.ToString();
        }

        protected void rptAvisos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            if (!(e.Item.FindControl("lnkAssunto") is HyperLink lnkAssunto))
            {
                return;
            }

            string idMensagem = DataBinder.Eval(e.Item.DataItem, "idMensagem")?.ToString();
            string assunto = DataBinder.Eval(e.Item.DataItem, "sAssunto")?.ToString();

            lnkAssunto.Text = string.IsNullOrWhiteSpace(assunto) ? "(Sem assunto)" : assunto;
            lnkAssunto.NavigateUrl = ResolveUrl($"~/App/Paginas/Mensagem/Mensagens_Detalhe.aspx?id={idMensagem}");
            lnkAssunto.Target = "_blank";

            if (e.Item.FindControl("rptTagsAviso") is Repeater rptTagsAviso)
            {
                string sTags = DataBinder.Eval(e.Item.DataItem, "sTags")?.ToString() ?? string.Empty;
                string[] tags = sTags.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                rptTagsAviso.DataSource = tags;
                rptTagsAviso.DataBind();

                if (e.Item.FindControl("pnTagsAviso") is Panel pnTagsAviso)
                {
                    pnTagsAviso.Visible = tags.Length > 0;
                }
            }
        }

        private void AplicarConfiguracaoPainel()
        {
            lblTituloPainel.Text = TituloPainel;
            lblVazio.Text = TextoVazio;
            lnkVerTodos.Text = TextoVerTodos;
            pnVerTodos.Visible = ExibirLinkVerTodos;
            lnkVerTodos.NavigateUrl = ResolveUrl("~/App/Paginas/Mensagem/Mensagens.aspx?sFiltro=2&sCategoria=AVISO");
        }

        private void CarregarAvisos()
        {
            DataTable tbAvisos = ConsultarAvisosNaoLidos();
            DataTable tbExibicao = LimitarRegistros(tbAvisos);

            bool possuiAvisos = tbExibicao != null && tbExibicao.Rows.Count > 0;
            lblVazio.Visible = !possuiAvisos;
            pnLista.Visible = possuiAvisos;

            rptAvisos.DataSource = possuiAvisos ? tbExibicao : null;
            rptAvisos.DataBind();
        }

        private DataTable ConsultarAvisosNaoLidos()
        {
            try
            {
                return ExecutarConsultaAvisos(incluirCategoriaMensagem: true);
            }
            catch
            {
                try
                {
                    return ExecutarConsultaAvisos(incluirCategoriaMensagem: false);
                }
                catch
                {
                    return new DataTable();
                }
            }
        }

        private DataTable ExecutarConsultaAvisos(bool incluirCategoriaMensagem)
        {
            Dictionary<string, string> vParametros = MontarParametrosConsulta(incluirCategoriaMensagem);
            return BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Mensagens", vParametros, false);
        }

        private Dictionary<string, string> MontarParametrosConsulta(bool incluirCategoriaMensagem)
        {
            string idUsuario = IDENTITY.Variaveis.idUsuario();
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_COM_FILTRO" },
                { "@idUsuarioPesquisa", idUsuario },
                { "@idDepartamentoDestino", "0" },
                { "@sDepartamentosDestino", string.Empty },
                { "@dtInicial", string.Empty },
                { "@dtFinal", string.Empty },
                { "@sAssunto", string.Empty },
                { "@idUsuarioDestino", idUsuario },
                { "@idFiltroMensagem", "2" },
                { "@sDirecaoMensagem", "Recebida" },
                { "@sIsAviso", "S" },
                { "@idTipoObjeto", "-99" }
            };

            if (incluirCategoriaMensagem)
            {
                vParametros.Add("@sCategoriaMensagem", "AVISO");
            }

            return vParametros;
        }

        private DataTable LimitarRegistros(DataTable tbAvisos)
        {
            if (tbAvisos == null)
            {
                return new DataTable();
            }

            if (tbAvisos.Rows.Count <= LimiteRegistros)
            {
                return tbAvisos;
            }

            DataTable tbLimitada = tbAvisos.Clone();
            foreach (DataRow linha in tbAvisos.AsEnumerable().Take(LimiteRegistros))
            {
                tbLimitada.ImportRow(linha);
            }

            return tbLimitada;
        }
    }
}
