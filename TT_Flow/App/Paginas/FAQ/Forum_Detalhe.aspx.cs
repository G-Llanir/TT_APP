using AjaxControlToolkit.HtmlEditor.Sanitizer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Flow.App.Paginas.Forum
{
    public partial class Forum_Detalhe : Page
    {
        string sTituloPagina = "Fórum Responder/Editar";
        string sProcedure = "sp_Manipula_tbl_Flow_Forum";

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FUNCOES.ValidaPermissao(Permissao.Forum.Consultar, true);
                Popula_Combos();
                BuscarRespostas();
                UpdDetalhe.Visible = false;
                divDescricao.Visible = false;
                smExemplos.Visible = false;

                if (Request["id"] != null)
                {
                    if (!FUNCOES.ValidaPermissao(Permissao.FAQ.Incluir))
                        cmdNovoFaq.Visible = false;

                    if (!FUNCOES.ValidaPermissao(Permissao.Forum.Alterar))
                    {
                        UpdDetalhe.Visible = false;
                        divDescricao.Visible = false;
                        cmdEditar.Visible = false;
                    }

                    Pesquisar(Request["id"].ToString());
                }
                else
                    FUNCOES.DirecionaPagina("app/Paginas/Forum/Forum_Detalhe.aspx?id=0");
                
                if (Request["id"] == "0")
                {
                    divAlterarStatus.Visible = false;
                    UpdGeral.Visible = false;
                    UpdDetalhe.Visible = true;
                    divDescricao.Visible = true;
                    smExemplos.Visible = true;
                    PainelAtualizacaoDetalhe.Visible = false;
                    FUNCOES.ValidaPermissao(Permissao.Forum.Incluir, true);
                    LimpaCampos();
                }

                if (Request["ed"] == "1")
                {
                    UpdGeral.Visible = false;
                    UpdDetalhe.Visible = true;
                    divDescricao.Visible = true;
                }
            }
        }

        protected void Pesquisar(string idPesquisa)
        {
            if (idPesquisa != "0")
            {
                lblTituloPagina.Text = sTituloPagina;

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE" },
                    { "@idForum", idPesquisa },
                    { "@idUsuario", HttpContext.Current.Session["idUsuario"].ToString() }
                };
                DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                {
                    txtidForum.Text = RETORNO.DATASET(dsPesquisa, 0, "idForum");
                    ddlAlterarStatus.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idStatus");
                    txtsDscTitulo.Text = RETORNO.DATASET(dsPesquisa, 0, "sTitulo");
                    forum_title.InnerText = txtsDscTitulo.Text;
                    ddlCategoria.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idCategoria");
                    ddlDepartamento.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idDepartamento");
                    txtsTag.Text = RETORNO.DATASET(dsPesquisa, 0, "sTag").Replace("|", " ").Trim();
                    forum_tags.InnerText = "Tag's: " + txtsTag.Text;
                    txtdtCriado.Text = RETORNO.DATASET(dsPesquisa, 0, "dtCriado");
                    edsCorpo.Value = RETORNO.DATASET(dsPesquisa, 0, "sCorpo");
                    ltBody.Text = edsCorpo.Value;

                    DataRow imgBd = dsPesquisa.Tables[0].Rows[0];
                    try
                    {
                        imgUsuario.ImageUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])imgBd["imgColaborador"]);
                    }
                    catch { }

                    PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario"));
                    PainelAtualizacaoDetalhe.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario"));

                    if (sErro != "")
                        throw new Exception(sErro);
                }
            }
            else
                lblTituloPagina.Text = "Novo Fórum";
        }

        #endregion

        #region | Combos

        protected void Popula_Combos()
        {
            FUNCOES.Popula_Combo(ddlAlterarStatus, "sp_Select 'Flow_Forum_Status'", "idStatus", "sDscStatus");
            FUNCOES.Popula_Combo(ddlCategoria, "sp_Select 'Flow_Forum_Categoria'", "idCategoria", "sDscCategoria", false, "Selecione uma Categoria", "0");
            FUNCOES.Popula_Combo(ddlDepartamento, "sp_Select 'Flow_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Selecione um Departamento", "0");
        }

        #endregion

        #region | Utils

        protected void LimpaCampos()
        {
            txtidForum.Text = "Novo";
            txtsDscTitulo.Text = "";
            txtsTag.Text = "";
            ddlCategoria.SelectedValue = "0";
            ddlDepartamento.SelectedValue = "0";
            txtdtCriado.Text = "";
            edsCorpo.Value = "";
            //txtSummernote.Text = "";
            PainelAtualizacao.Visible = false;
        }

        private bool ValidarDados()
        {
            if (txtsDscTitulo.Text.Length < 1)
            {
                MensagemPagina.MostraMensagem_Erro("Título inválido!");
                return false;
            }
            if (txtsTag.Text.Length < 1)
            {
                MensagemPagina.MostraMensagem_Erro("TAG inválida!");
                return false;
            }
            if (ddlDepartamento.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Departamento inválido!");
                return false;
            }
            if (ddlCategoria.SelectedValue == "0" && txtNovaCategoria.Text.Length < 1)
            {
                MensagemPagina.MostraMensagem_Erro("Um dos campos de Categoria deve ser preenchido!");
                return false;
            }
            if (ddlCategoria.SelectedValue != "0" && txtNovaCategoria.Text.Length > 0)
            {
                MensagemPagina.MostraMensagem_Erro("Não é possível criar uma Nova Categoria e ao mesmo tempo selecionar uma Categoria existente!");
                return false;
            }
            if (edsCorpo.Value.Length < 1)
            {
                MensagemPagina.MostraMensagem_Erro("Descrição inválida!");
                return false;
            }

            return true;
        }

        protected string carregaimgColaborador(object vbImagem)
        {
            string sImagem = "http://placehold.it/50/55C1E7/fff";
            try
            {
                sImagem = Convert.ToBase64String((byte[])vbImagem);
                sImagem = "data:image/jpg;base64," + sImagem;
            }
            catch { }

            return sImagem;
        }

        protected void BuscarRespostas()
        {
            string procedure = "sp_Manipula_tbl_Flow_Forum";
            string idForum = Request["id"];

            Dictionary<string, string> vParametrosBuscarResposta = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_RESPOSTAS" },
                { "@idForum", idForum }
            };
            DataSet ds = BD.ExecutarDataSet(procedure, vParametrosBuscarResposta);

            if (BD.ValidarDataSet(ds))
            {
                rptRespostas.DataSource = ds.Tables[0];
                rptRespostas.DataBind();
            }
        }

        #endregion

        #region | Eventos

        protected void cmdEditar_Click(object sender, EventArgs e) => FUNCOES.DirecionaPagina(string.Format("app/Paginas/FAQ/Forum_Detalhe.aspx?id={0}&ed=1", Request["id"]));

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            if (ValidarDados())
            {
                try
                {
                    string idForum = Request["id"];
                    string idStatus = ddlAlterarStatus.SelectedValue.ToString();
                    string idUsuario = HttpContext.Current.Session["idUsuario"].ToString();
                    string sTag = "|" + txtsTag.Text.Trim().Replace(" ", "|") + "|";

                    if (idStatus == "0")
                        idStatus = "2";


                    var whiteList = new Dictionary<string, string[]>
                    {
                        { "p", new[] { "style" } },
                        { "br", new string[0] },

                        { "strong", new string[0] },
                        { "b", new string[0] },
                        { "em", new string[0] },
                        { "i", new string[0] },
                        { "u", new string[0] },
                        { "s", new string[0] },

                        { "span", new[] { "style" } },

                        { "h1", new[] { "style" } },
                        { "h2", new[] { "style" } },
                        { "h3", new[] { "style" } },

                        { "ul", new string[0] },
                        { "ol", new string[0] },
                        { "li", new string[0] },

                        { "blockquote", new[] { "style" } },

                        { "a", new[] { "href", "title" } }
                    };

                    var sanitizer = new DefaultHtmlSanitizer();
                    string stringlimpa = sanitizer.GetSafeHtmlFragment(edsCorpo.Value.ToString(), whiteList);


                    Dictionary<string, string> vParametrosSalvar = new Dictionary<string, string>
                    {
                        { "@sFuncao", "FORUM_SALVAR" },
                        { "@idForum", idForum },
                        { "@idDepartamento", ddlDepartamento.SelectedValue },
                        { "@idCategoria", ddlCategoria.SelectedValue },
                        { "@sDscNovaCategoria", txtNovaCategoria.Text },
                        { "@idStatus", idStatus },
                        { "@sTitulo", txtsDscTitulo.Text },
                        { "@sTag", sTag },
                        { "@sCorpo", stringlimpa },
                        { "@idUsuario", idUsuario }
                    };
                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametrosSalvar);

                    if (BD.ValidarDataSet(dsSalvar, out string sErro))
                    {
                        if (Request["id"] != "0")
                        {
                            Pesquisar(Request["id"]);
                            MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                        }
                        else
                        {
                            LimpaCampos();
                            MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                        }
                    }
                    else
                        throw new Exception("BD: " + sErro.ToString());
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }
            }
        }

        protected void cmdSalvarResposta_Click(object sender, EventArgs e)
        {
            string idUsuario = HttpContext.Current.Session["idUsuario"].ToString();
            string procedure = "sp_Manipula_tbl_Flow_Forum";

            Dictionary<string, string> vParametrosSalvarResposta = new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR_RESPOSTA" },
                { "@idForum", Request["id"] },
                { "@idUsuario", idUsuario },
                { "@sDscResposta", txtsCorpoResposta.Text }
            };
            DataSet ds = BD.ExecutarDataSet(procedure, vParametrosSalvarResposta);

            if (BD.ValidarDataSet(ds))
                MensagemPagina.MostraMensagem_Sucesso("Registro gravado com Sucesso");
            else
                MensagemPagina.MostraMensagem_Erro("Erro ao Registrar");

            txtsCorpoResposta.Text = "";
        }

        protected void cmdNovoFaq_Click(object sender, EventArgs e)
        {
            string titulo = Regex.Replace(txtsDscTitulo.Text, @"[^\w\d\s.,~^´`ªº°]", string.Empty);
            string tags = Regex.Replace(txtsTag.Text, @"[^\w\d\s.,~^´`ªº°]", string.Empty);
            FUNCOES.DirecionaPagina(string.Format("app/Paginas/FAQ/FAQ_Detalhe.aspx?id=0&ftit={0}&ftag={1}&fd={2}", titulo, tags, ddlDepartamento.SelectedValue.ToString()));
        }

        #endregion

    }
}
