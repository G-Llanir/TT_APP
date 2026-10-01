using MathNet.Numerics.Providers.SparseSolver;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Controles;
using TT_Flow.App.Paginas.Requisicao;
using TT_Hub.App.Paginas.RRHH;
using static Permissao.WMS;
using static TT_Flow.App.Controles.GerenciadorMembrosModal;
using static TT_Flow.App.Paginas.PCP.Fabricacao_Detalhe;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;

namespace TT_Flow.App.Paginas.PCP
{
    public partial class Fabricacao_Detalhe : System.Web.UI.Page
    {
        #region | Page Loads
        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Fabricacao.Consultar, true);
            if (!IsPostBack)
            {
                CarregarKanban("0");
            }
            else
            {
                CarregarKanban("0");
            }
        }
        #endregion

        #region | Variáveis Globais
        string sProcedureFabricacao = "sp_Manipula_tbl_Flow_WMS_Fabricacao";

        #endregion

        #region | Classes
        public List<AtividadeKanban> listaKanban
        {
            get
            {
                if (Session["listaKanban"] == null)
                    Session["listaKanban"] = new List<AtividadeKanban>();

                return (List<AtividadeKanban>)Session["listaKanban"];
            }
            set
            {
                Session["listaKanban"] = value;
            }
        }
        List<ProjetoKanbanVM> projetosKanban
        {
            get
            {
                if (Session["ProjetoKanbanVM"] == null)
                    Session["ProjetoKanbanVM"] = new List<ProjetoKanbanVM>();

                return (List<ProjetoKanbanVM>)Session["ProjetoKanbanVM"];
            }
            set
            {
                Session["ProjetoKanbanVM"] = value;
            }
        }

        List<AtividadeKanbanDTO> linhas
        {
            get
            {
                if (Session["AtividadesDto"] == null)
                    Session["AtividadesDto"] = new List<AtividadeKanbanDTO>();

                return (List<AtividadeKanbanDTO>)Session["AtividadesDto"];
            }
            set
            {
                Session["AtividadesDto"] = value;
            }
        }

        List<cls_Projeto_Fabricacao> bs_Projeto_Fabricacao
        {
            get
            {
                if (Session["bs_Projeto_Fabricacao"] == null)
                    Session["bs_Projeto_Fabricacao"] = new List<cls_Projeto_Fabricacao>();

                return (List<cls_Projeto_Fabricacao>)Session["bs_Projeto_Fabricacao"];
            }
            set
            {
                Session["bs_Projeto_Fabricacao"] = value;
            }
        }
        
        #endregion

        #region | Classes Globais
        public class AtividadeKanban
        {
            public string SdscDepartamento { get; set; }
            public string SdscAtividade { get; set; }
            public int NPorcentagem { get; set; } // entre 0 e 100
            public string SdscStatusAtividade { get; set; } = "Em Andamento";
            public string SDscLinkDetalhes { get; set; }
            public int IdProjeto { get; set; }

            public string SCor =>
                NPorcentagem >= 100 ? "text-success" :
                NPorcentagem >= 50 ? "text-info" :
                "text-danger";

            public string SIcone { get; set; }
        }
        public class AtividadeKanbanDTO
        {
            public int IdAtividadePai { get; set; }
            public string TituloPai { get; set; }

            public int IdAtividadeFilha { get; set; }
            public string TituloFilha { get; set; }
            public string DescricaoFilha { get; set; }
            public decimal? HorasPrevisaoFilha { get; set; }
            public DateTime? DtInicioFilha { get; set; }
            public DateTime? DtFinalFilha { get; set; }
            public int IdProjeto { get; set; }
            public string NomeProjeto { get; set; }
            public string Status { get; set; }
            public string CorStatus { get; set; }

            public string NomeResponsavelAtividade { get; set; }
            public string NomeApontador { get; set; }

            public decimal? HorasApontadas { get; set; }
            public string Observacao { get; set; }
            public DateTime? DataApontamento { get; set; }
            public bool Concluida { get; set; }
            public bool EmAndamento { get; set; }
        }
        public class StatusGestor
        {
            public string Tarefa { get; set; }
            public string Departamento { get; set; }
            public string Solicitante { get; set; }
            public string Status { get; set; }
        }
        public class ProcedimentoVista
        {
            public string Status { get; set; }
            public string Departamento { get; set; }
            public string Solicitante { get; set; }
            public Dictionary<string, string> Recursos { get; set; } = new Dictionary<string, string>();
        }
        #endregion

        #region | View‑models auxiliares 
        public class ProjetoKanbanVM
        {
            public int IdProjeto { get; set; }
            public string NomeProjeto { get; set; }
            public List<ColunaKanbanVM> AtividadesPai { get; set; }
        }

        public class ColunaKanbanVM
        {
            public int IdProjeto { get; set; }
            public int IdAtividadePai { get; set; }
            public string TituloPai { get; set; }
            public GestorCard CardAtual { get; set; }
            public List<GestorCard> AtividadesFilhasStatus { get; set; } = new List<GestorCard>();
        }
        public class AtividadePaiVM
        {
            public string Titulo { get; set; }
            public decimal HorasPrevistas { get; set; }
            public decimal HorasApontadas { get; set; }
            public int Percentual => HorasPrevistas == 0 ? 0 :
                                     (int)Math.Round((HorasApontadas / HorasPrevistas) * 100);
        }
        #endregion

        #region | Métodos Popular
        private int PaginaAtual
        {
            get => ViewState["PaginaAtual"] != null ? (int)ViewState["PaginaAtual"] : 0;
            set => ViewState["PaginaAtual"] = value;
        }

        private void CarregarKanban(string idProjeto)
        {
            //var dados = ObterKanbanPorProjeto(idProjeto);
            var dados = ObterProjetosFabricacao();
            if (dados.Count > 0)
            {
                rptProjetos.DataSource = dados;
                rptProjetos.DataBind();
                CarregarStatusSimplificado(); 
                CarregarHistorico();
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Nenhuma Fabricação Foi Localizada!");
            }

        }

        #endregion

        #region | Eventos
        protected void Voltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/App/Paginas/WMS/Fabricacao.aspx");
        }
        //protected void rptProjetos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        //{
        //    if (e.Item.ItemType != ListItemType.Item &&
        //        e.Item.ItemType != ListItemType.AlternatingItem) return;

        //    var projeto = (ProjetoKanbanVM)e.Item.DataItem;

        //    // títulos das colunas
        //    var rptColunas = (Repeater)e.Item.FindControl("rptColunas");
        //    rptColunas.DataSource = projeto.AtividadesPai;
        //    rptColunas.DataBind();

        //    // linha de cards (uma célula por atividade PAI)
        //    var rptCardsLinha = (Repeater)e.Item.FindControl("rptCardsLinha");
        //    rptCardsLinha.DataSource = projeto.AtividadesPai;
        //    rptCardsLinha.DataBind();
        //}
        protected void rptCardsLinha_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item &&
                e.Item.ItemType != ListItemType.AlternatingItem) return;

            var coluna = (ColunaKanbanVM)e.Item.DataItem;
            var pnlCard = (Panel)e.Item.FindControl("pnlCard");

            if (coluna.CardAtual == null)
            {
                pnlCard.Controls.Add(new Literal
                {
                    Text = @"<div class='kanban-card bg-info'>
                        <div class='apontamento'> <strong>Não Iniciado</strong></div>
                     </div>"
                });
                return;
            }

            var c = coluna.CardAtual;
            pnlCard.Controls.Add(new Literal
            {
                Text = $@"
<div class='kanban-card bg-{c.CorStatus}'>
    <div class='apontamento'>
        <strong>{c.Responsavel}</strong> - 
        <small>{c.Data:dd/MM/yyyy}</small>
    </div>
</div>"
            });
        }
        protected void cmdAbrirProjeto_Click(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "AbrirProjeto")
            {
                string idProjeto = e.CommandArgument.ToString();
                hddidProjeto.Value = idProjeto;
                AbrirModal(idProjeto, "P");
                //projetosKanban.Clear();

                //projetosKanban = ObterKanbanPorProjeto(idProjeto);

                //List<Projeto> listaProjetos = projetosKanban.Select(p => new Projeto
                //{
                //    CodigoProduto = p.IdProjeto.ToString(),
                //    NomeProduto = p.NomeProjeto,
                //    Requisicao = "",
                //    Recursos = p.AtividadesPai.Select(ap => new Recurso
                //    {
                //        Nome = ap.TituloPai,
                //        Previsao = ap.CardAtual?.Horas.ToString("0.##") ?? "0",
                //        Inicio = ap.CardAtual?.Data?.ToString("dd/MM/yyyy") ?? "-",
                //        FimPrevisto = "-",
                //        Processos = new List<Processo>
                //{
                //    new Processo
                //    {
                //        Nome = ap.CardAtual?.Titulo ?? "Sem Título",
                //        Duracao = ap.CardAtual?.Horas.ToString("0.##") ?? "0",
                //        Inicio = ap.CardAtual?.Data?.ToString("dd/MM/yyyy") ?? "-",
                //        Fim = "-"
                //    }
                //}
                //    }).ToList()
                //}).ToList();

                //rptProjeto.DataSource = listaProjetos;
                //rptProjeto.DataBind();
                //AbrirModal_Click(sender, e);
            }
        }
        protected void rptColunas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item &&
                e.Item.ItemType != ListItemType.AlternatingItem) return;

            var coluna = (TT_Flow.App.Paginas.PCP.Fabricacao_Detalhe.ColunaKanbanVM)e.Item.DataItem;
            var litStatusFilhas = (Literal)e.Item.FindControl("litStatusFilhas");

            if (litStatusFilhas == null) return;

            var statusHtml = new StringBuilder();

            if (coluna.AtividadesFilhasStatus != null && coluna.AtividadesFilhasStatus.Any())
            {
                for (int i = 0; i < coluna.AtividadesFilhasStatus.Count; i++)
                {
                    var gestorCardFilha = coluna.AtividadesFilhasStatus[i];
                    string statusText = (i + 1).ToString();

                    statusHtml.Append($"<span class='status-item {gestorCardFilha.CorStatus}'>{statusText}</span>");
                }
            }
            else
            {
                statusHtml.Append("<span class='status-item bg-light' style='flex-grow: 1;'>N/A</span>");
            }

            litStatusFilhas.Text = statusHtml.ToString();
        }
        protected void cmdFechar_Click(object sender, EventArgs e)
        {
            hddidProjeto.Value = "0";
            FecharModal("modalProjetos");
        }
        protected void btnPesquisar_Click(object sender, EventArgs e)
        {
            CarregarKanban("0");
        }

        protected void TimerKanban_Tick(object sender, EventArgs e)
        {
            ObterKanbanPorProjeto(hddidProjeto.Value);
        }
        #endregion

        #region | Métodos Utilitários
        private List<ProjetoKanbanVM> ObterKanbanPorProjeto(string idProjeto)
        {
            var parametros = new Dictionary<string, string>
            {
                { "sFuncao",  "CONSULTAR-GESTOR-GERAL" },
                { "idProjeto", idProjeto },
                { "sDscTitulo", txtNomeFabricacao.Text }
            };
            linhas.Clear();

            linhas = BD.ExecutarLista<AtividadeKanbanDTO>(sProcedureFabricacao, parametros, true) ?? new List<AtividadeKanbanDTO>();

            linhas = linhas.Where(l => l.IdAtividadePai > 0).ToList();

            var mapa = new Dictionary<int, ProjetoKanbanVM>();

            var atividadesPaiAgrupadas = linhas.GroupBy(r => r.IdAtividadePai);

            foreach (var grupoPai in atividadesPaiAgrupadas)
            {
                var primeiraAtividadeDoGrupo = grupoPai.First();
                var idProjetoAtual = primeiraAtividadeDoGrupo.IdProjeto;

                if (!mapa.TryGetValue(idProjetoAtual, out var proj))
                {
                    proj = new ProjetoKanbanVM
                    {
                        IdProjeto = idProjetoAtual,
                        NomeProjeto = string.IsNullOrWhiteSpace(primeiraAtividadeDoGrupo.NomeProjeto)
                                        ? $"Projeto {idProjetoAtual}"
                                        : primeiraAtividadeDoGrupo.NomeProjeto.Trim(),
                        AtividadesPai = new List<ColunaKanbanVM>()
                    };
                    mapa[idProjetoAtual] = proj;
                }

                var col = proj.AtividadesPai
                                    .FirstOrDefault(c => c.IdAtividadePai == grupoPai.Key);

                if (col == null)
                {
                    col = new ColunaKanbanVM
                    {
                        IdProjeto = idProjetoAtual,
                        IdAtividadePai = grupoPai.Key,
                        TituloPai = string.IsNullOrWhiteSpace(primeiraAtividadeDoGrupo.TituloPai)
                                        ? "(Sem título)"
                                        : primeiraAtividadeDoGrupo.TituloPai.Trim()
                    };
                    proj.AtividadesPai.Add(col);
                }

                // >>> IMPORTANTE: POPULANDO AtividadesFilhasStatus AQUI <<<
                foreach (var r in grupoPai.OrderBy(x => x.IdAtividadeFilha).GroupBy(x => x.IdAtividadeFilha))
                {
                    // Pega a ultima atividade filha de cada grupo (garante unicidade pelo IdAtividadeFilha)
                    var atividadeFilhaUnica = r.Last();

                    col.AtividadesFilhasStatus.Add(new GestorCard
                    {
                        Status = atividadeFilhaUnica.Status,
                        CorStatus = atividadeFilhaUnica.CorStatus ?? "text-muted",
                        Responsavel = atividadeFilhaUnica.NomeApontador
                        // Se precisar de IdAtividadeFilha, adicione aqui também
                        // Titulo = atividadeFilhaUnica.TituloFilha, // Se o "1 | 2 | 3" for o título, use este
                    });

                    if (atividadeFilhaUnica.DataApontamento.HasValue)
                    {
                        if (col.CardAtual == null || atividadeFilhaUnica.DataApontamento > col.CardAtual.Data)
                        {
                            col.CardAtual = new GestorCard
                            {
                                Responsavel = atividadeFilhaUnica.NomeApontador ?? "-",
                                Horas = atividadeFilhaUnica.HorasApontadas ?? 0,
                                Descricao = atividadeFilhaUnica.Observacao ?? "-",
                                Data = atividadeFilhaUnica.DataApontamento,
                                Status = atividadeFilhaUnica.Status,
                                CorStatus = atividadeFilhaUnica.CorStatus ?? "text-muted"
                            };
                        }
                    }
                }
            }

            foreach (var p in mapa.Values)
                p.AtividadesPai = p.AtividadesPai
                                           .OrderBy(c => c.IdAtividadePai)
                                           .ToList();

            return mapa.Values.ToList();
        }

        private List<cls_Projeto_Fabricacao> ObterProjetosFabricacao()
        {
            bs_Projeto_Fabricacao.Clear();
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "Consulta-Projetos" }                
            };

            DataSet ds = BD.ExecutarDataSet(sProcedureFabricacao, vParametros);

            DataTable dtProjetos = ds.Tables[0];
            DataTable dtProdutos = ds.Tables[1];
            DataTable dtOperacao = ds.Tables[2];

            var dicProjetos = new Dictionary<int, cls_Projeto_Fabricacao>();
            var dicProdutos = new Dictionary<(int idProjeto, int idProduto), cls_Produto>();

            foreach (DataRow rowProj in dtProjetos.Rows)
            {
                int id = Convert.ToInt32(rowProj["idProjeto"]);

                var projeto = new cls_Projeto_Fabricacao
                {
                    idProjeto = Convert.ToInt32(rowProj["idProjeto"]),
                    lst_Produtos = new List<cls_Produto>(),
                    lst_OperacoesPadrao = new List<cls_Operacao>()
                };

                dicProjetos[id] = projeto;
                bs_Projeto_Fabricacao.Add(projeto);
            }

            foreach (DataRow rowProd in dtProdutos.Rows)
            {
                int idProjeto = Convert.ToInt32(rowProd["idProjeto"]);
                int idProduto = Convert.ToInt32(rowProd["idProduto"]);

                if (!dicProjetos.TryGetValue(idProjeto, out var projetoPai))
                    continue;
                
                var produto = new cls_Produto
                {
                    idProduto = Convert.ToInt32(rowProd["idProduto"]),
                    idProjeto = Convert.ToInt32(rowProd["idProjeto"]),
                    sCodigo = rowProd["sCodigo"].ToString(),
                    sDscProduto = rowProd["sDscProduto"].ToString(),
                    Quantidade = rowProd["Quantidade"].ToString(),
                    dtAtualizacao = rowProd["dtAtualizacao"].ToString(),
                    lst_Operacoes = new List<cls_Operacao>()
                };

                dicProdutos[(idProjeto, idProduto)] = produto;
                projetoPai.lst_Produtos.Add(produto);
            }

            var operacoesJaAdicionadasPorProjeto = new Dictionary<int, HashSet<int>>();
            foreach (DataRow rowOp in dtOperacao.Rows)
            {
                int idProduto = Convert.ToInt32(rowOp["idProduto"]);
                int idProjeto = Convert.ToInt32(rowOp["idProjeto"]);
                int idAtividade = Convert.ToInt32(rowOp["idAtividade"]);

                if (!dicProdutos.TryGetValue((idProjeto, idProduto), out var produtoPai))
                    continue;

                if (!dicProjetos.TryGetValue(idProjeto, out var projetoPai))
                    continue;

                var operacao = new cls_Operacao
                {
                    idAtividade = Convert.ToInt32(rowOp["idAtividade"]),
                    idProduto = Convert.ToInt32(rowOp["idProduto"]),
                    idProjeto = Convert.ToInt32(rowOp["idProjeto"]),
                    sDscTitulo = rowOp["sDscTitulo"].ToString(),
                    dtInicio = rowOp["dtInicio"].ToString(),
                    dtFinal = rowOp["dtFinal"].ToString(),
                    sDscStatus = rowOp["sDscStatus"].ToString(),
                    sCor = rowOp["sCor"].ToString()
                };

                produtoPai.lst_Operacoes.Add(operacao);

                if (!operacoesJaAdicionadasPorProjeto.ContainsKey(idProjeto))
                {
                    operacoesJaAdicionadasPorProjeto[idProjeto] = new HashSet<int>();
                }

                if (operacoesJaAdicionadasPorProjeto[idProjeto].Add(idAtividade))
                {
                    projetoPai.lst_OperacoesPadrao.Add(operacao);
                }
            }

            foreach (var projeto in bs_Projeto_Fabricacao)
            {
                projeto.lst_OperacoesPadrao = projeto.lst_OperacoesPadrao
                    .OrderBy(op => op.idAtividade)
                    .ToList();

                foreach (var produto in projeto.lst_Produtos)
                {
                    produto.lst_Operacoes = produto.lst_Operacoes
                        .OrderBy(op => op.idAtividade)
                        .ToList();
                }
            }

            return bs_Projeto_Fabricacao;
        }

        //    private List<ProjetoKanbanVM> ObterKanbanPorProjeto(string idProjeto)
        //    {
        //        var parametros = new Dictionary<string, string>
        //{
        //    { "sFuncao",  "CONSULTAR-GESTOR-GERAL" },
        //    { "idProjeto", idProjeto }
        //};
        //        linhas.Clear();

        //        linhas = BD.ExecutarLista<AtividadeKanbanDTO>(
        //                         sProcedureFabricacao, parametros, true)
        //                     ?? new List<AtividadeKanbanDTO>();

        //        linhas = linhas.Where(l => l.IdAtividadePai > 0).ToList();

        //        var mapa = new Dictionary<int, ProjetoKanbanVM>();

        //        foreach (var r in linhas)
        //        {
        //            if (!mapa.TryGetValue(r.IdProjeto, out var proj))
        //            {
        //                proj = new ProjetoKanbanVM
        //                {
        //                    IdProjeto = r.IdProjeto,
        //                    NomeProjeto = string.IsNullOrWhiteSpace(r.NomeProjeto)
        //                                    ? $"Projeto {r.IdProjeto}"
        //                                    : r.NomeProjeto.Trim(),
        //                    AtividadesPai = new List<ColunaKanbanVM>()
        //                };
        //                mapa[r.IdProjeto] = proj;
        //            }
        //            var col = proj.AtividadesPai
        //                          .FirstOrDefault(c => c.IdAtividadePai == r.IdAtividadePai);

        //            if (col == null)
        //            {
        //                col = new ColunaKanbanVM
        //                {
        //                    IdProjeto = r.IdProjeto,
        //                    IdAtividadePai = r.IdAtividadePai,
        //                    TituloPai = string.IsNullOrWhiteSpace(r.TituloPai)
        //                                    ? "(Sem título)"
        //                                    : r.TituloPai.Trim()
        //                };
        //                proj.AtividadesPai.Add(col);
        //            }

        //            if (r.DataApontamento.HasValue)
        //            {
        //                if (col.CardAtual == null ||
        //                    r.DataApontamento > col.CardAtual.Data)
        //                {
        //                    col.CardAtual = new GestorCard
        //                    {
        //                        Responsavel = r.NomeResponsavelAtividade
        //                                   ?? r.NomeApontador
        //                                   ?? "-",
        //                        Horas = r.HorasApontadas ?? 0,
        //                        Descricao = r.Observacao ?? "-",
        //                        Data = r.DataApontamento,
        //                        Status = r.Status,
        //                        CorStatus = r.CorStatus ?? "text-muted"
        //                    };
        //                }
        //            }
        //        }

        //        foreach (var p in mapa.Values)
        //            p.AtividadesPai = p.AtividadesPai
        //                                     .OrderBy(c => c.IdAtividadePai) 
        //                                     .ToList();

        //        return mapa.Values.ToList();
        //    }

        protected void FecharModal(string modalId)
        {
            string script = $@"
        $('#{modalId}').modal('hide');
        $('.modal-backdrop').remove();
        $('body').removeClass('modal-open');
       ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_CloseModal_" + modalId, script, true);
        }
        #endregion

        #region | PDF - Código usado no controle agora
        //void ExportarPDF(string idProjeto)
        //{
        //    var parametros = new Dictionary<string, string>
        //{
        //    { "sFuncao",  "CONSULTAR-GERENCIADOR-PDF" },
        //    { "idProjeto", idProjeto },
        //    { "idUsuarioImpressao", IDENTITY.Variaveis.idUsuario() }
        //};

        //    DataSet ds = BD.ExecutarDataSet(sProcedureFabricacao, parametros, true);

        //    if (ds != null && ds.Tables.Count > 0)
        //    {
        //        GeradorPdfAtividades gerador = new GeradorPdfAtividades();
        //        byte[] pdfBytes = gerador.GerarRelatorioAtividadesPdf(ds); 

        //        string sNomeArquivoOriginal = "RelatórioAtividades_" + ds.Tables[0].Rows[0]["idProjeto"].ToString() + "_" + FUNCOES.CarimboDataHora() + ".pdf";

        //        File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivoOriginal, pdfBytes);
        //        CarregarKanban("0");
        //        FUNCOES.DownloadArquivo(Page, sNomeArquivoOriginal);
        //    }
        //    else
        //    {
        //        MensagemPagina.MostraMensagem_Erro("<b>Erro: </b>O Arquivo não foi Gerado. Nenhum dado encontrado.");
        //    }
        //}
        #endregion

        #region | Modal Detalhes
        protected void AbrirModal(string id, string sTipoId)
        {
            FecharModal("modalAtividades");
            GerenciadorMembrosModal.PopularModal(id, sTipoId);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalEnvio", "$('#modalAtividades').modal('show');", true);
        }
        #endregion

        #region | Funcionários
        private void CarregarStatusSimplificado()
        {
            var parametros = new Dictionary<string, string> { { "sFuncao", "CONSULTAR-STATUS-USUARIO-LISTA" } };
            DataSet ds = BD.ExecutarDataSet(sProcedureFabricacao, parametros, true);

            if (ds == null || ds.Tables.Count < 3) return;

            var todosUsuarios = EntidadeFuncoes<UsuarioVM>.ConvertDataTable(ds.Tables[0]);
            var atividadesAtuais = EntidadeFuncoes<AtividadeInfoVM>.ConvertDataTable(ds.Tables[1]);
            var ultimasConcluidas = EntidadeFuncoes<AtividadeInfoVM>.ConvertDataTable(ds.Tables[2]);

            if (!todosUsuarios.Any())
            {
                rptStatusSimplificado.Visible = false;
                pnlFuncionariosVazio.Visible = true;
                return;
            }

            rptStatusSimplificado.Visible = true;
            pnlFuncionariosVazio.Visible = false;

            var dadosFinais = new List<StatusUsuarioSimplificadoVM>();

            foreach (var user in todosUsuarios)
            {
                var linhaUsuario = new StatusUsuarioSimplificadoVM
                {
                    NomeUsuario = user.sDscUsuario,
                    IsIdle = true // Assume ocioso por padrão
                };

                // Procura a atividade ATUAL
                var atual = atividadesAtuais.FirstOrDefault(a => a.idUsuario == user.IdUsuario);
                if (atual != null)
                {
                    linhaUsuario.AtividadeAtual = $"{atual.NomeProjeto} - {atual.NomeAtividade}";
                    linhaUsuario.IsIdle = false; // Tem atividade, não está ocioso
                }

                // Procura a atividade ANTERIOR
                var anterior = ultimasConcluidas.FirstOrDefault(a => a.idUsuario == user.IdUsuario);
                if (anterior != null)
                {
                    linhaUsuario.AtividadeAnterior = $"{anterior.NomeProjeto} - {anterior.NomeAtividade}";
                }

                dadosFinais.Add(linhaUsuario);
            }

            rptStatusSimplificado.DataSource = dadosFinais;
            rptStatusSimplificado.DataBind();
        }

        private void CarregarHistorico()
        {
            var parametros = new Dictionary<string, string>
            {
                { "sFuncao", "CONSULTAR-HISTORICO-RECURSO" }
                // Adicione aqui parâmetros de filtro se desejar
            };

            var historico = BD.ExecutarLista<HistoricoRecursoVM>(sProcedureFabricacao, parametros, true)
                            ?? new List<HistoricoRecursoVM>();

            // Lógica de visibilidade
            if (historico.Any())
            {
                rptHistorico.Visible = true;
                pnlHistoricoVazio.Visible = false;
                rptHistorico.DataSource = historico;
                rptHistorico.DataBind();
            }
            else
            {
                rptHistorico.Visible = false;
                pnlHistoricoVazio.Visible = true;
            }
        }
        #endregion

        protected void lnkCodigoProduto_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "VerProduto")
            {
                int idProduto = Convert.ToInt32(e.CommandArgument);
                ScriptManager.RegisterStartupScript(this, GetType(), "abrirProduto", $"window.open('App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={idProduto}', '_blank');", true);
            }
        }

        protected void rptProjetos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var projeto = (cls_Projeto_Fabricacao)e.Item.DataItem;

                Repeater rptProdutos = (Repeater)e.Item.FindControl("rptProdutos");
                if (rptProdutos != null && projeto.lst_Produtos != null)
                {
                    rptProdutos.DataSource = projeto.lst_Produtos;
                    rptProdutos.DataBind();
                }               
            }
        }

        protected void rptProdutos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var produto = (cls_Produto)e.Item.DataItem;

                PlaceHolder phOperacoes = (PlaceHolder)e.Item.FindControl("phOperacoesProduto");
                if (phOperacoes != null && produto.lst_Operacoes != null)
                {
                    foreach (var operacao in produto.lst_Operacoes)
                    {
                        TableCell cellOperacao = new TableCell();
                        cellOperacao.CssClass = "cell-operacao";
                        cellOperacao.Attributes["style"] = "min-width: 250px;";

                        Panel pnlContainer = new Panel();
                        pnlContainer.CssClass = "operacao-container";

                        Literal litStatus = new Literal();                        
                        litStatus.Text = $"<span class='status-indicator {operacao.sCor}'></span>";

                        Panel pnlInfo = new Panel();
                        pnlInfo.CssClass = "operacao-info";

                        Label lblNome = new Label();
                        lblNome.CssClass = "op-numero-nome";
                        lblNome.Text = $"{operacao.idAtividade} - {operacao.sDscTitulo}";

                        Panel pnlStatus = new Panel();                       
                        pnlStatus.CssClass = "op-apontamentos";
                        pnlStatus.Controls.Add(new LiteralControl("<span class='op-label'>" + operacao.sDscStatus + "</span> "));                        

                        Panel pnlCT = new Panel();                       
                        pnlCT.CssClass = "op-ct";
                        pnlCT.Controls.Add(new LiteralControl("<span class='op-label'>Inicio: </span> "));
                        pnlCT.Controls.Add(new Label { Text = operacao.dtInicio.ToString() });
                        
                        Panel pnlProd = new Panel();                       
                        pnlProd.CssClass = "op-producao";
                        pnlProd.Controls.Add(new LiteralControl("<span class='op-label'>Final Previsto: </span> "));
                        pnlProd.Controls.Add(new Label { Text = operacao.dtFinal.ToString() });
                               
                        pnlInfo.Controls.AddAt(0, lblNome);
                        pnlInfo.Controls.Add(pnlStatus);
                        pnlInfo.Controls.Add(pnlCT);
                        pnlInfo.Controls.Add(pnlProd);

                        pnlContainer.Controls.Add(litStatus);
                        pnlContainer.Controls.Add(pnlInfo);
                        cellOperacao.Controls.Add(pnlContainer);
                        phOperacoes.Controls.Add(cellOperacao);
                    }
                }
            }
        }

        protected int GetNumeroOperacoes(object dataItem)
        {
            if (dataItem is cls_Projeto_Fabricacao projeto && projeto.lst_OperacoesPadrao != null)
            {
                return projeto.lst_OperacoesPadrao.Count;
            }
            return 1;
        }

        protected int GetTotalColunas(object dataItem)
        {
            if (dataItem is cls_Projeto_Fabricacao projeto)
            {
                int numOperacoes = projeto.lst_OperacoesPadrao?.Count ?? 0;
                return 3 + numOperacoes;
            }
            return 3; 
        }
    }

    #region | Classe que Herda
    public class DynamicTemplate : ITemplate
    {
        private readonly string _recurso;
        public DynamicTemplate(string recurso)
        {
            _recurso = recurso;
        }

        public void InstantiateIn(Control container)
        {
            var lbl = new Label();
            lbl.DataBinding += (sender, e) =>
            {
                var row = (GridViewRow)((Control)sender).NamingContainer;
                var dataItem = (DataRowView)row.DataItem;

                if (dataItem.DataView.Table.Columns.Contains(_recurso) && dataItem[_recurso] != DBNull.Value)
                {
                    ((Label)sender).Text = dataItem[_recurso].ToString();
                }
                else
                {
                    ((Label)sender).Text = "—";
                }
            };
            container.Controls.Add(lbl);
        }
    }

    public class Projeto
    {
        public string CodigoProduto { get; set; }
        public string NomeProduto { get; set; }
        public string Requisicao { get; set; }
        public List<Recurso> Recursos { get; set; }
    }

    public class Recurso
    {
        public string Nome { get; set; }
        public string Previsao { get; set; }
        public string Inicio { get; set; }
        public string FimPrevisto { get; set; }
        public List<Processo> Processos { get; set; }
    }

    public class Processo
    {
        public string Nome { get; set; }
        public string Duracao { get; set; }
        public string Inicio { get; set; }
        public string Fim { get; set; }
    }
    public class GestorColuna
    {
        public string TituloAtividadePai { get; set; }
        public List<GestorCard> AtividadesFilhas { get; set; } = new List<GestorCard>();
    }
    public class GestorCard
    {
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string PrevisaoHoras { get; set; }
        public DateTime? Data { get; set; }
        public decimal Horas { get; set; }
        public string Status { get; set; }
        public string CorStatus { get; set; }

        public string Responsavel { get; set; }

        public List<Apontamento> ListaApontamentos { get; set; }
    }

    public class Apontamento
    {
        public string Responsavel { get; set; }
        public string Descricao { get; set; }
        public decimal Horas { get; set; }
        public DateTime? Data { get; set; }
    }

    public class FuncionarioStatusVM
    {
        public int idUsuarioIntegrado { get; set; } // Adicionar este campo
        public string NomeFuncionario { get; set; }
        public string NomeProjeto { get; set; }
        public string NomeAtividade { get; set; }
    }

    public class HistoricoRecursoVM
    {
        public string NomeRecurso { get; set; }
        public string NomeProcesso { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public int DuracaoMinutos { get; set; }
        public string NomeExecutor { get; set; }
    }

    public class StatusUsuarioSimplificadoVM
    {
        public string NomeUsuario { get; set; }
        public bool IsIdle { get; set; }
        public string AtividadeAtual { get; set; }
        public string AtividadeAnterior { get; set; }
    }

    // Classe auxiliar para ler os dados do banco
    public class AtividadeInfoVM
    {
        public int idUsuario { get; set; }
        public string NomeProjeto { get; set; }
        public string NomeAtividade { get; set; }
    }

    // A classe UsuarioVM que já tinhamos pode ser mantida
    public class UsuarioVM
    {
        public int IdUsuario { get; set; }
        public string sDscUsuario { get; set; }
    }

    public class cls_Projeto_Fabricacao
    {
        public int idProjeto { get; set; }
        public List<cls_Produto> lst_Produtos { get; set; } = new List<cls_Produto>();
        public List<cls_Operacao> lst_OperacoesPadrao { get; set; } = new List<cls_Operacao>();
    }

    public class cls_Produto
    {
        public int idProjeto { get; set; }
        public int idProduto { get; set; }
        public string sCodigo { get; set; }
        public string sDscProduto { get; set; }
        public string ImagemUrl { get; set; }
        public string Quantidade { get; set; }
        public string dtAtualizacao { get; set; }
        public List<cls_Operacao> lst_Operacoes { get; set; } = new List<cls_Operacao>();
    }

    public class cls_Operacao
    {
        public int idProjeto { get; set; }
        public int idProduto { get; set; }
        public int idAtividade { get; set; }
        public string sDscTitulo { get; set; }
        public string dtInicio { get; set; }
        public string dtFinal { get; set; }
        public string sDscStatus { get; set; }
        public string sCor { get; set; }
    }
    #endregion

}