using AjaxControlToolkit.HtmlEditor.Sanitizer;
using iText.StyledXmlParser.Jsoup.Safety;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Flow.App.Paginas.FAQ
{
    public partial class FAQ_Detalhe : Page
    {
        string sTituloPagina = "FAQ Comentar/Editar";
        string sProcedure = "sp_Manipula_tbl_Flow_FAQ";

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FUNCOES.ValidaPermissao(Permissao.FAQ.Consultar, true);
                Popula_Combos();
                //RegistraScript("");
                BuscarComentarios();
                UpdDetalhe.Visible = false;
                divDescricao.Visible = false;
                smExemplos.Visible = false;
                divAlterarStatus.Visible = false;

                if (Request["id"] != null)
                {
                    if (!FUNCOES.ValidaPermissao(Permissao.FAQ.Alterar))
                    {
                        UpdDetalhe.Visible = false;
                        divDescricao.Visible = false;
                        cmdEditar.Visible = false;
                    }

                    if (FUNCOES.ValidaPermissao(Permissao.FAQ.Aprovar))
                    {
                        divAlterarStatus.Visible = true;
                        cmdEditar.Visible = true;
                    }

                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.DirecionaPagina("app/Paginas/FAQ/FAQ_Detalhe.aspx?id=0");
                }
                if (Request["id"] == "0")
                {
                    divAlterarStatus.Visible = false;
                    UpdGeral.Visible = false;
                    UpdDetalhe.Visible = true;
                    divDescricao.Visible = true;
                    smExemplos.Visible = true;
                    PainelAtualizacaoDetalhe.Visible = false;
                    FUNCOES.ValidaPermissao(Permissao.FAQ.Incluir, true);
                    LimpaCampos();
                }
                if (ddlPermiteComentario.SelectedIndex == 2)
                {
                    comments.Visible = false;
                }
                if (Request["ed"] == "1")
                {
                    UpdGeral.Visible = false;
                    UpdDetalhe.Visible = true;
                    divDescricao.Visible = true;
                }
                if (Request["ftit"] != null)
                {
                    txtsDscTitulo.Text = Request["ftit"];
                    txtsTag.Text = Request["ftag"];
                    ddlDepartamento.SelectedValue = Request["fd"];
                }
            }
        }

        protected void Pesquisar(string idPesquisa)
        {
            string sErro = "";
            if (idPesquisa != "0")
            {
                lblTituloPagina.Text = sTituloPagina;

                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                vParametros.Add("@idFaq", idPesquisa);
                vParametros.Add("@idUsuario", HttpContext.Current.Session["idUsuario"].ToString());
                DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsPesquisa, out sErro))
                {
                    txtidFAQ.Text = RETORNO.DATASET(dsPesquisa, 0, "idFaq");
                    ddlAlterarStatus.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idStatus");
                    txtsDscTitulo.Text = RETORNO.DATASET(dsPesquisa, 0, "sTitulo");
                    faq_title.InnerText = RETORNO.DATASET(dsPesquisa, 0, "sTitulo");
                    ddlCategoria.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idCategoria");
                    ddlDepartamento.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idDepartamento");
                    txtsTag.Text = RETORNO.DATASET(dsPesquisa, 0, "sTag").Replace("|", " ").Trim();
                    faq_tags.InnerText = "Tag's: " + txtsTag.Text;
                    txtdtCriado.Text = RETORNO.DATASET(dsPesquisa, 0, "dtCriado");
                    edsCorpo.Value = RETORNO.DATASET(dsPesquisa, 0, "sCorpo");
                    ltsCorpo.Text = edsCorpo.Value;

                    DataRow imgBd = dsPesquisa.Tables[0].Rows[0];
                    try
                    {
                        imgUsuario.ImageUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])imgBd["imgColaborador"]);
                    }
                    catch { }
                    PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario"));
                    PainelAtualizacaoDetalhe.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario"));

                    if (RETORNO.DATASET(dsPesquisa, 0, "sPermiteComentario") == "S")
                    {
                        ddlPermiteComentario.SelectedIndex = 1;
                    }
                    else
                    {
                        ddlPermiteComentario.SelectedIndex = 2;
                    }

                    if (sErro != "")
                    {
                        throw new Exception(sErro);
                    }
                }

            }
            else
            {
                lblTituloPagina.Text = "Novo FAQ";
            }
        }

        #endregion

        #region | Combos

        protected void Popula_Combos()
        {
            FUNCOES.Popula_Combo(ddlAlterarStatus, "sp_Select 'Flow_Faq_Status'", "idStatus", "sDscStatus");
            FUNCOES.Popula_Combo(ddlCategoria, "sp_Select 'Flow_Faq_Categoria'", "idCategoria", "sDscCategoria", false, "Selecione uma Categoria", "0");
            FUNCOES.Popula_Combo(ddlDepartamento, "sp_Select 'Flow_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Selecione um Departamento", "0");

            ddlPermiteComentario.Items.Add(new System.Web.UI.WebControls.ListItem("Selecione uma opção"));
            ddlPermiteComentario.Items.Add(new System.Web.UI.WebControls.ListItem("SIM"));
            ddlPermiteComentario.Items.Add(new System.Web.UI.WebControls.ListItem("NAO"));
        }

        #endregion

        #region | Utils

        protected void LimpaCampos()
        {
            txtidFAQ.Text = "Novo";
            txtsDscTitulo.Text = "";
            txtsTag.Text = "";
            ddlCategoria.SelectedValue = "0";
            ddlDepartamento.SelectedValue = "0";
            ddlPermiteComentario.SelectedIndex = 0;
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
            if (ddlPermiteComentario.SelectedIndex == 0)
            {
                MensagemPagina.MostraMensagem_Erro("Campo Permite Comentário inválido!");
                return false;
            }
            if (edsCorpo.Value.Length < 1)
            {
                MensagemPagina.MostraMensagem_Erro("Descrição inválida!");
                return false;
            }
            //if (txtSummernote.Text.Length < 1)
            //{
            //    MensagemPagina.MostraMensagem_Erro("Descrição inválida!");
            //    return false;
            //}


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
            catch
            {

            }

            return sImagem;
        }

        protected void BuscarComentarios()
        {

            string procedure = "sp_Manipula_tbl_Flow_FAQ";
            string idFaq = Request["id"];
            DataSet ds = new DataSet();

            Dictionary<string, string> vParametrosBuscarComentario = new Dictionary<string, string>();
            vParametrosBuscarComentario.Add("@sFuncao", "CONSULTAR_COMENTARIOS");
            vParametrosBuscarComentario.Add("@idFaq", idFaq);
            ds = BD.ExecutarDataSet(procedure, vParametrosBuscarComentario);

            if (BD.ValidarDataSet(ds))
            {
                rptComentarios.DataSource = ds.Tables[0];
                rptComentarios.DataBind();

            }
        }

        #endregion

        #region | Eventos
        
        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {
                    string idFaq = Request["id"];
                    string sPermiteComentario = "N";
                    string idStatus = ddlAlterarStatus.SelectedValue.ToString();
                    string idUsuario = HttpContext.Current.Session["idUsuario"].ToString();
                    string sTag = "|" + txtsTag.Text.Trim().Replace(" ", "|") + "|";

                    if (ddlPermiteComentario.SelectedValue == "SIM")
                    {
                        sPermiteComentario = "S";
                    }

                    if (idFaq == "0")
                    {
                        idStatus = "2";
                    }

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


                    DataSet dsSalvar;
                    Dictionary<String, String> vParametrosSalvar = new Dictionary<string, string>();

                    vParametrosSalvar.Add("@sFuncao", "FAQ_SALVAR");
                    vParametrosSalvar.Add("@idFaq", idFaq);
                    vParametrosSalvar.Add("@idDepartamento", ddlDepartamento.SelectedValue);
                    vParametrosSalvar.Add("@idCategoria", ddlCategoria.SelectedValue);
                    vParametrosSalvar.Add("@sDscNovaCategoria", txtNovaCategoria.Text);
                    vParametrosSalvar.Add("@idStatus", idStatus);
                    vParametrosSalvar.Add("@sTitulo", txtsDscTitulo.Text);
                    vParametrosSalvar.Add("@sPermiteComentario", sPermiteComentario);
                    vParametrosSalvar.Add("@sTag", sTag);
                    vParametrosSalvar.Add("@sCorpo", stringlimpa);
                    //vParametrosSalvar.Add("@sCorpo", txtSummernote.Text.ToString());
                    vParametrosSalvar.Add("@idUsuario", idUsuario);
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametrosSalvar);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
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
                    {
                        throw new Exception("BD: " + sErro.ToString());
                    }

                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }
            }
        }

        protected void cmdSalvarComentario_Click(object sender, EventArgs e)
        {

            string idUsuario = HttpContext.Current.Session["idUsuario"].ToString();
            string procedure = "sp_Manipula_tbl_Flow_FAQ";
            DataSet ds = new DataSet();

            Dictionary<string, string> vParametrosSalvarComentario = new Dictionary<string, string>();
            vParametrosSalvarComentario.Add("@sFuncao", "SALVAR_COMENTARIO");
            vParametrosSalvarComentario.Add("@idFaq", Request["id"]);
            vParametrosSalvarComentario.Add("@idUsuario", idUsuario);
            vParametrosSalvarComentario.Add("@sDscComentario", txtsCorpoComentario.Text);
            ds = BD.ExecutarDataSet(procedure, vParametrosSalvarComentario);

            if (BD.ValidarDataSet(ds))
            {
                MensagemPagina.MostraMensagem_Sucesso("Registro gravado com Sucesso");
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao Registrar");
            }

            txtsCorpoComentario.Text = "";
        }

        protected void cmdEditar_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina(string.Format("app/Paginas/FAQ/FAQ_Detalhe.aspx?id={0}&ed=1", Request["id"]));
        }
        
        #endregion

    }
}
