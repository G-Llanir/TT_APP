using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Hub.App.Paginas.RRHH;
using static TT_Flow.App.Paginas.PCP.Fabricacao_Detalhe;
using RETORNO = TT.FrameWork.BD.Retorno;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using System.IO;

namespace TT_Flow.App.Controles
{
    public partial class GerenciadorMembrosModal : System.Web.UI.UserControl
    {
        private string CurrentId
        {
            get { return ViewState["CurrentId"] as string ?? "0"; }
            set { ViewState["CurrentId"] = value; }
        }

        private string CurrentTipoId
        {
            get { return ViewState["CurrentTipoId"] as string ?? ""; }
            set { ViewState["CurrentTipoId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {


        }

        string sProcedureFabricacao = "sp_Manipula_tbl_Flow_WMS_Fabricacao";

        #region Classes

        public class UsuarioExecutor
        {
            public int idUsuario { get; set; }
            public string NomeUsuario { get; set; }
        }

        public class AtividadeFilha
        {
            public int idAtividade { get; set; }
            public string sDscTituloAtividade { get; set; }
            public string sStatusAtividade { get; set; }
            public List<UsuarioExecutor> MembrosDisponiveis { get; set; } = new List<UsuarioExecutor>();
            public List<int> idMembrosSelecionados { get; set; } = new List<int>();
        }

        public class AtividadePai
        {
            public int idAtividadePai { get; set; }
            public string sDscTituloAtividadePai { get; set; }
            public List<AtividadeFilha> AtividadesFilhas { get; set; } = new List<AtividadeFilha>();
        }

        public class Projeto
        {
            public int idProjeto { get; set; }
            public string sDscTituloProjeto { get; set; }
            public string sNumeroRequisicao { get; set; }
            public List<AtividadePai> AtividadesPai { get; set; } = new List<AtividadePai>();
        }

        public class RawDataRow
        {
            public int idProjeto { get; set; }
            public string sDscTituloProjeto { get; set; }
            public int? idAtividadePai { get; set; }
            public string sDscTituloAtividadePai { get; set; }
            public int? idAtividade { get; set; }
            public string sDscTituloAtividade { get; set; }
            public int? idUsuarioAtividade { get; set; }
            public string NomeUsuarioAtividade { get; set; }
            public int? idUsuarioProjeto { get; set; }
            public string NomeUsuarioProjeto { get; set; }
            public int? sNumeroRequisicao { get; set; }
            public string sStatusAtividade { get; set; }
        }

        List<Projeto> lsProjetos
        {
            get
            {
                if (Session["lsProjetos"] == null)
                    Session["lsProjetos"] = new List<Projeto>();

                return (List<Projeto>)Session["lsProjetos"];
            }
            set
            {
                Session["lsProjetos"] = value;
            }
        }

        #endregion

        #region Utils

        public void PopularModal(string id, string sTipoid)
        {
            CurrentId = id;
            CurrentTipoId = sTipoid;

            if (CurrentTipoId == "P")
            {
                hddidProjeto.Value = CurrentId;
                cmdExportar.Visible = FUNCOES.ValidaPermissao(Permissao.Fabricacao.PDF, false);
                btnSalvarMembros.Visible = FUNCOES.ValidaPermissao(Permissao.Fabricacao.Gerenciar, false);
            }
            else
            {
                cmdExportar.Visible = false;
            }

            rptProjetos.DataSource = ObterAtividadesComUsuarios(id, sTipoid);
            rptProjetos.DataBind();
        }

        public bool PopularModal(string id, string sTipoid, bool modeModal)
        {
            if (modeModal)
            {
                CurrentId = id;
                CurrentTipoId = sTipoid;

                bool openModal = false;

                if (ObterAtividadesComUsuarios(id, sTipoid).Count > 0)
                {

                    if (CurrentTipoId == "P")
                    {
                        hddidProjeto.Value = CurrentId;
                        cmdExportar.Visible = FUNCOES.ValidaPermissao(Permissao.Fabricacao.PDF, false);
                        btnSalvarMembros.Visible = FUNCOES.ValidaPermissao(Permissao.Fabricacao.Gerenciar, false);
                    }
                    else
                    {
                        cmdExportar.Visible = false;
                    }

                    rptProjetos.DataSource = ObterAtividadesComUsuarios(id, sTipoid);
                    rptProjetos.DataBind();

                    openModal = true;
                }
                else
                {
                    openModal = false;
                }

                return openModal;
            }
            else
            {
                CurrentId = id;
                CurrentTipoId = sTipoid;

                if (CurrentTipoId == "P")
                {
                    hddidProjeto.Value = CurrentId;
                    cmdExportar.Visible = FUNCOES.ValidaPermissao(Permissao.Fabricacao.PDF, false);
                    btnSalvarMembros.Visible = FUNCOES.ValidaPermissao(Permissao.Fabricacao.Gerenciar, false);
                }
                else
                {
                    cmdExportar.Visible = false;
                }

                rptProjetos.DataSource = ObterAtividadesComUsuarios(id, sTipoid);
                rptProjetos.DataBind();

                return !modeModal;
            }
        }

        public List<Projeto> ObterAtividadesComUsuarios(string id, string sTipoid)
        {
            var parametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR-GERENCIADOR" },
                { "@idProjeto", "0" },
                { "@idRequisicao", "0" }
            };

            if (sTipoid == "P")
                parametros["@idProjeto"] = id;
            else if (sTipoid == "R")
                parametros["@idRequisicao"] = id;

            var linhasRawDynamic = BD.ExecutarLista<dynamic>(sProcedureFabricacao, parametros, true);

            if (linhasRawDynamic == null || !linhasRawDynamic.Any())
            {
                return new List<Projeto>();
            }

            var linhas = linhasRawDynamic.Select(l => new RawDataRow
            {
                idProjeto = (int)(l.idProjeto ?? 0),
                sDscTituloProjeto = l.sDscTituloProjeto as string,
                idAtividadePai = l.idAtividadePai as int?,
                sDscTituloAtividadePai = l.sDscTituloAtividadePai as string,
                idAtividade = l.idAtividade as int?,
                sDscTituloAtividade = l.sDscTituloAtividade as string,
                idUsuarioAtividade = l.idUsuarioAtividade as int?,
                NomeUsuarioAtividade = l.NomeUsuarioAtividade as string,
                idUsuarioProjeto = l.idUsuarioProjeto as int?,
                NomeUsuarioProjeto = l.NomeUsuarioProjeto as string,
                sNumeroRequisicao = l.sNumeroRequisicao as int?,
                sStatusAtividade = l.sStatusAtividade as string
            }).ToList();

            var todosUsuariosDisponiveisGlobal = linhas
                .Where(u => u.idUsuarioProjeto.HasValue && !string.IsNullOrEmpty(u.NomeUsuarioProjeto))
                .Select(u => new UsuarioExecutor { idUsuario = u.idUsuarioProjeto.Value, NomeUsuario = u.NomeUsuarioProjeto })
                .GroupBy(u => u.idUsuario)
                .Select(g => g.First())
                .OrderBy(u => u.NomeUsuario)
                .ToList();


            lsProjetos = linhas
                .GroupBy(p => new { p.idProjeto, p.sDscTituloProjeto, p.sNumeroRequisicao })
                .Select(gp => new Projeto
                {
                    idProjeto = gp.Key.idProjeto,
                    sDscTituloProjeto = gp.Key.sDscTituloProjeto,
                    sNumeroRequisicao = gp.Key.sNumeroRequisicao?.ToString() ?? string.Empty,
                    AtividadesPai = gp
                        .GroupBy(ap => new { IdAtividadePai = ap.idAtividadePai ?? 0, ap.sDscTituloAtividadePai })
                        .Select(gap => new AtividadePai
                        {
                            idAtividadePai = gap.Key.IdAtividadePai,
                            sDscTituloAtividadePai = gap.Key.sDscTituloAtividadePai,
                            AtividadesFilhas = gap
                                .GroupBy(af => new { IdAtividade = af.idAtividade ?? 0, af.sDscTituloAtividade })
                                .Select(gaf =>
                                {
                                    var membrosJaSelecionadosParaEstaAtividade = gaf
                                        .Where(u => u.idUsuarioAtividade.HasValue)
                                        .Select(u => u.idUsuarioAtividade.Value)
                                        .Distinct()
                                        .ToList();

                                    return new AtividadeFilha
                                    {
                                        idAtividade = gaf.Key.IdAtividade,
                                        sDscTituloAtividade = gaf.Key.sDscTituloAtividade,
                                        sStatusAtividade = gaf.First().sStatusAtividade as string,
                                        MembrosDisponiveis = todosUsuariosDisponiveisGlobal,
                                        idMembrosSelecionados = membrosJaSelecionadosParaEstaAtividade
                                    };
                                }).ToList()
                        }).ToList()
                }).ToList();

            lsProjetos = lsProjetos
                .Where(p => p.idProjeto > 0)
                .Select(p =>
                {
                    p.AtividadesPai = p.AtividadesPai
                        .Where(ap => ap.idAtividadePai > 0 && !string.IsNullOrEmpty(ap.sDscTituloAtividadePai))
                        .Select(ap =>
                        {
                            ap.AtividadesFilhas = ap.AtividadesFilhas
                                .Where(af => af.idAtividade > 0 && !string.IsNullOrEmpty(af.sDscTituloAtividade))
                                .ToList();
                            return ap;
                        })
                        .ToList();
                    return p;
                })
                .ToList();


            return lsProjetos;
        }

        private void SalvarMembrosAtividades()
        {
            string idReferencia = CurrentId;
            string tipoReferencia = CurrentTipoId;
            string msgs = "";
            DataSet DsSalvar = new DataSet();
            try
            {
                foreach (RepeaterItem projetoItem in rptProjetos.Items)
                {
                    if (projetoItem.ItemType == ListItemType.Item || projetoItem.ItemType == ListItemType.AlternatingItem)
                    {
                        Repeater rptAtividadesPai = (Repeater)projetoItem.FindControl("rptAtividadesPai");
                        if (rptAtividadesPai != null)
                        {
                            foreach (RepeaterItem atividadePaiItem in rptAtividadesPai.Items)
                            {
                                if (atividadePaiItem.ItemType == ListItemType.Item || atividadePaiItem.ItemType == ListItemType.AlternatingItem)
                                {
                                    Repeater rptAtividadesFilhas = (Repeater)atividadePaiItem.FindControl("rptAtividadesFilhas");
                                    if (rptAtividadesFilhas != null)
                                    {
                                        foreach (RepeaterItem atividadeFilhaItem in rptAtividadesFilhas.Items)
                                        {
                                            if (atividadeFilhaItem.ItemType == ListItemType.Item || atividadeFilhaItem.ItemType == ListItemType.AlternatingItem)
                                            {
                                                HiddenField hdnIdAtividade = (HiddenField)atividadeFilhaItem.FindControl("hdnIdAtividade");
                                                HiddenField HddStatus = (HiddenField)atividadeFilhaItem.FindControl("HddStatus");
                                                int idAtividade = 0;

                                                if (hdnIdAtividade != null && int.TryParse(hdnIdAtividade.Value, out idAtividade) && HddStatus.Value != "Finalizada")
                                                {
                                                    ListBox ddlMembros = (ListBox)atividadeFilhaItem.FindControl("ddlMembrosAtividade");
                                                    if (ddlMembros != null)
                                                    {

                                                        var parametrosDelete = new Dictionary<string, string>
                                                        {
                                                            { "@sFuncao", "DELETAR-MEMBROS-ATIVIDADE" },
                                                            { "@idAtividade", idAtividade.ToString() }
                                                        };
                                                        DsSalvar = BD.ExecutarDataSet(sProcedureFabricacao, parametrosDelete, true);

                                                        foreach (ListItem membroSelecionado in ddlMembros.Items)
                                                        {
                                                            if (membroSelecionado.Selected)
                                                            {
                                                                int idUsuario = int.Parse(membroSelecionado.Value);
                                                                var parametrosInsert = new Dictionary<string, string>
                                                                {
                                                                    { "@sFuncao", "INSERIR-MEMBRO-ATIVIDADE" },
                                                                    { "@idAtividade", idAtividade.ToString() },
                                                                    { "@idUsuario", idUsuario.ToString() }
                                                                };
                                                                DsSalvar = BD.ExecutarDataSet(sProcedureFabricacao, parametrosInsert, true);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msgs = "Erro ao salvar membros: " + ex.Message;
                MensagemPaginaControle.MostraMensagem_Erro(msgs, false);
                msgs = "";
            }
            finally
            {
                if (!string.IsNullOrEmpty(idReferencia) && !string.IsNullOrEmpty(tipoReferencia))
                {
                    PopularModal(idReferencia, tipoReferencia);

                    if (BD.ValidarDataSet(DsSalvar))
                    {
                        var id = RETORNO.DATASET(DsSalvar, "idAtividade");
                        msgs = "Membros Salvos com sucesso!";
                        if (!string.IsNullOrEmpty(msgs))
                            MensagemPaginaControle.MostraMensagem_Sucesso(msgs, false);
                    }
                    else
                    {
                        msgs = "Erro ao salvar.";
                        if (!string.IsNullOrEmpty(msgs))
                            MensagemPaginaControle.MostraMensagem_Erro(msgs, false);
                    }
                    //FecharModal("modalAtividades");
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "reopenModal", "$('#modalAtividades').modal('show');", true);
                }
                else
                {
                    msgs = "Erro: Não foi possível reabrir o modal. ID ou Tipo de referência ausentes.";
                    MensagemPaginaControle.MostraMensagem_Erro(msgs, false);
                    msgs = "";
                }
            }
        }

        #endregion

        #region Eventos
        protected void FecharModal_Click(object sender, EventArgs e)
        {
            FecharModal("modalAtividades");
        }
        protected void FecharModal(string modalId)
        {
            string script = $@"
        $('#{modalId}').modal('hide');
        $('.modal-backdrop').remove();
        $('body').removeClass('modal-open');
       ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_CloseModal_" + modalId, script, true);
        }
        protected void cmdSalvarMembros_Click(object sender, EventArgs e)
        {
            Session["MensagemMembrosModal"] = "Membros salvos com sucesso!";
            SalvarMembrosAtividades();
            AbrirModal(sender, e);
        }

        protected void rptProjetos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var projeto = (Projeto)e.Item.DataItem;

                var rptAtividadesPai = (Repeater)e.Item.FindControl("rptAtividadesPai");
                if (rptAtividadesPai != null)
                {
                    rptAtividadesPai.DataSource = projeto.AtividadesPai;
                    rptAtividadesPai.DataBind();
                }
            }
        }

        protected void rptAtividadesPai_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var atividadePai = (AtividadePai)e.Item.DataItem;

                var rptAtividadesFilhas = (Repeater)e.Item.FindControl("rptAtividadesFilhas");
                if (rptAtividadesFilhas != null)
                {
                    rptAtividadesFilhas.DataSource = atividadePai.AtividadesFilhas;
                    rptAtividadesFilhas.DataBind();
                }
            }
        }

        protected void rptAtividadesFilhas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var atividadeFilha = (AtividadeFilha)e.Item.DataItem;

                var ddlMembros = (ListBox)e.Item.FindControl("ddlMembrosAtividade");
                if (ddlMembros != null)
                {
                    ddlMembros.DataSource = atividadeFilha.MembrosDisponiveis;
                    ddlMembros.DataTextField = "NomeUsuario";
                    ddlMembros.DataValueField = "idUsuario";
                    ddlMembros.DataBind();

                    foreach (ListItem item in ddlMembros.Items)
                    {
                        if (atividadeFilha.idMembrosSelecionados.Contains(int.Parse(item.Value)))
                            item.Selected = true;
                    }

                    if (atividadeFilha.sStatusAtividade != null && atividadeFilha.sStatusAtividade.Equals("Finalizada", StringComparison.OrdinalIgnoreCase))
                    {
                        ddlMembros.Attributes.Add("Disabled", "Disabled");
                        ddlMembros.Attributes.Add("data-atividade-finalizada", "true");
                    }
                    else
                    {
                        if (FUNCOES.ValidaPermissao(Permissao.Fabricacao.Gerenciar, false))
                        {
                            ddlMembros.Attributes.Remove("Disabled");
                            ddlMembros.Attributes.Remove("data-atividade-finalizada");
                        }
                        else
                        {
                            ddlMembros.Attributes.Add("Disabled", "Disabled");
                            ddlMembros.Attributes.Add("data-atividade-finalizada", "true");
                        }

                    }
                }
            }
        }

        protected void AbrirModal(object sender, EventArgs e)
        {
            FecharModal("modalAtividades");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalEnvio", "$('#modalAtividades').modal('show');", true);
        }
        #endregion

        #region | PDF
        protected void cmdExportar_Click(object sender, EventArgs e)
        {
            ExportarPDF(hddidProjeto.Value);
            FecharModal_Click(sender, e);
        }
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
        //        //CarregarKanban("0");
        //        FUNCOES.DownloadArquivo(Page, sNomeArquivoOriginal);
        //    }
        //    else
        //    {
        //        MensagemPaginaControle.MostraMensagem_Erro("<b>Erro: </b>O Arquivo não foi Gerado. Nenhum dado encontrado.");
        //    }
        //}

        void ExportarPDF(string idProjeto)
        {
            var parametros = new Dictionary<string, string>
    {
        { "sFuncao",  "CONSULTAR-GERENCIADOR-PDF" },
        { "idProjeto", idProjeto },
        { "idUsuarioImpressao", IDENTITY.Variaveis.idUsuario() }
    };

            DataSet ds = BD.ExecutarDataSet(sProcedureFabricacao, parametros, true);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                string caminhoDaLogo = Server.MapPath("~/app/img/LogoTT.png");
                string caminhoDaUSer = Server.MapPath("~/app/img/userPic.png");

                GeradorPdfAtividades gerador = new GeradorPdfAtividades();
                byte[] pdfBytes = gerador.GerarRelatorioAtividadesPdf(ds, caminhoDaLogo, caminhoDaUSer);

                string sNomeArquivoOriginal = "RelatórioAtividades_" + ds.Tables[0].Rows[0]["idProjeto"].ToString() + "_" + FUNCOES.CarimboDataHora() + ".pdf";

                File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivoOriginal, pdfBytes);
                FUNCOES.DownloadArquivo(Page, sNomeArquivoOriginal);
            }
            else
            {
                MensagemPaginaControle.MostraMensagem_Erro("<b>Erro: </b>O Arquivo não foi Gerado. Nenhum dado encontrado.");
            }
        }
        #endregion
    }
}