using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using RETORNO = TT.FrameWork.BD.Retorno;
using IDENTITY = TT.FrameWork.Identity;
using TT_Flow.App.Controles;

namespace TT_Flow.App.Paginas.Comercial.Manutencao
{
    public partial class CategoriaEscopo_Detalhe : System.Web.UI.Page
    {

        #region | Contrutores

        Dictionary<int, string> list_Perguntas
        {
            get
            {
                if (ViewState["list_Perguntas"] == null)
                {
                    ViewState["list_Perguntas"] = new Dictionary<int, string>();
                }
                return (Dictionary<int, string>)ViewState["list_Perguntas"];
            }
            set
            {
                ViewState["list_Perguntas"] = value;
            }
        }

        Dictionary<int, string> list_Opcoes
        {
            get
            {
                if (ViewState["list_Opcoes"] == null)
                {
                    ViewState["list_Opcoes"] = new Dictionary<int, string>();
                }
                return (Dictionary<int, string>)ViewState["list_Opcoes"];
            }
            set
            {
                ViewState["list_Opcoes"] = value;
            }
        }

        #endregion

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "Manual-Escopo.pdf";

            if (!IsPostBack)
            {
                if (Request["id"] == null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Comercial.Manutencao.CategoriaEscopo.Incluir, true);
                    FUNCOES.DirecionaPagina("App/Paginas/Comercial/Manutencao/CategoriaEscopo_Detalhe.aspx?id=0");
                }
                else if (Request["id"] == "0")
                {
                    FUNCOES.ValidaPermissao(Permissao.Comercial.Manutencao.CategoriaEscopo.Incluir, true);
                    Pesquisar("0");
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Comercial.Manutencao.CategoriaEscopo.Consultar, true);
                    Pesquisar(Request["id"]);
                }

                try
                {
                    if (int.Parse(Request["msg"]) > 0)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_DirecionaDetalhe", "function detalhe() { __doPostBack('" + cmdDetalhe.UniqueID + "', ''); }", true);
                        MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso!<br /><br /><a href='#' onclick='detalhe();'>Ver Detalhes..</a>");
                    }
                }
                catch { }
            }

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Mascaras", "$('[id*=txtOrdem]').mask('000000009', { reverse: true });", true);
        }

        void Pesquisar(string idCategoria)
        {
            div_OrdemPergunta.Visible = false;
            div_OrdemOpcao.Visible = false;
            div_cmdAlterarPergunta.Visible = false;
            div_cmdAlterarOpcao.Visible = false;

            string sErro = "";

            Dictionary<string, string> vParametros = new Dictionary<string, string>()
            {
                { "@sFuncao", "CONSULTAR_DETALHE_CATEGORIA" },
                { "@idCategoriaEscopo", idCategoria }
            };

            DataSet dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Escopo", vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                if (idCategoria != "0")
                {
                    hddidCategoria.Value = RETORNO.DATASET(dsPesquisa, 0, 0, "idCategoria");
                    txtidCategoria.Text = RETORNO.DATASET(dsPesquisa, 0, 0, "idCategoria");
                    txtsDscCategoria.Text = RETORNO.DATASET(dsPesquisa, 0, 0, "sDscCategoria");
                    ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, 0, "sAtivo"));

                    if (Convert.ToBoolean(Request["duplicar"]))
                    {
                        hddidCategoria.Value = "0";
                        txtidCategoria.Text = "Duplicar";
                        txtsDscCategoria.Text += " - Duplicado";
                    }

                    lblTituloPagina.Text = string.Format("Editar {0}", txtsDscCategoria.Text);

                    PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, 0, "sDscUsuarioAtualizacao"));

                    PopulaPerguntas_Opcoes(dsPesquisa.Tables[0]);

                    gvPergunta.DataSource = list_Perguntas.OrderBy(p => p.Value.Split('|')[0]);
                    gvPergunta.DataBind();

                    gvOpcao.DataSource = list_Opcoes.OrderBy(o => o.Value.Split('|')[0]);
                    gvOpcao.DataBind();

                    if (!FUNCOES.ValidaPermissao(Permissao.Comercial.Manutencao.CategoriaEscopo.Alterar))
                    {
                        txtsDscCategoria.ReadOnly = true;
                        ComboAtivo.Situacao_BloquearEdicao(false);
                        txtPergunta.Visible = false;
                        txtOpcao.Visible = false;
                        cmdIncluirPergunta.Visible = false;
                        cmdIncluirOpcao.Visible = false;

                        FUNCOES.EsconderColunas(gvPergunta, "Excluir");
                        FUNCOES.EsconderColunas(gvOpcao, "Excluir");

                        cmdSalvar.Visible = false;
                    }
                }
            }
            else
            {
                txtidCategoria.Text = "Novo";
                lblTituloPagina.Text = "Nova Categoria de Escopo";

                PainelAtualizacao.Visible = false;
            }
        }

        #endregion

        #region | Salvar

        protected void SalvarDados()
        {
            if (ValidarDados())
            {
                try
                {
                    string idCategoriaEscopo = hddidCategoria.Value;

                    if (Convert.ToBoolean(Request["duplicar"]))
                        idCategoriaEscopo = "0";

                    string sPerguntas = "|";
                    string sOpcoes = "|";

                    list_Perguntas.Values.OrderBy(p => p.Split('|')[0]).ToList().ForEach(p => sPerguntas += p.Split('|')[1] + '|');
                    list_Opcoes.Values.OrderBy(o => o.Split('|')[0]).ToList().ForEach(o => sOpcoes += o.Split('|')[1] + '|');

                    Dictionary<string, string> vParametrosSalvar = new Dictionary<string, string>()
                    {
                        { "@sFuncao", "SALVAR_CATEGORIA" },
                        { "@idCategoriaEscopo", idCategoriaEscopo },
                        { "@sDscCategoria", txtsDscCategoria.Text },
                        { "@sPerguntas", sPerguntas },
                        { "@sOpcoes", sOpcoes },
                        { "@sAtivo", ComboAtivo.Situacao_Recuperar() },
                        { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                    };

                    DataSet dsSalvar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Escopo", vParametrosSalvar);

                    if (BD.ValidarDataSet(dsSalvar))
                    {
                        idCategoriaEscopo = RETORNO.DATASET(dsSalvar, 0, 0, "idCategoriaEscopo");

                        if (Request["id"] != "0")
                            FUNCOES.DirecionaPagina(string.Format("App/Paginas/Comercial/Manutencao/CategoriaEscopo_Detalhe.aspx?id={0}&msg=1", idCategoriaEscopo));
                        else
                            FUNCOES.DirecionaPagina(string.Format("App/Paginas/Comercial/Manutencao/CategoriaEscopo_Detalhe.aspx?id=0&msg={0}", idCategoriaEscopo));
                    }
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro("Erro no salvamento de dados!\r\nErro:" + ex.Message);
                }
            }
        }

        #endregion

        #region | Utils

        public void PopulaPerguntas_Opcoes(DataTable dt)
        {
            int i = list_Perguntas.Count > 0 ? list_Perguntas.Last().Key : 0;
            foreach (string s in dt.Rows[0]["sPerguntas"].ToString().Split('|'))
            {
                if (s.Length > 0)
                {
                    i++;
                    list_Perguntas.Add(i, string.Format("{0}|{1}", i, s));
                }
            }

            i = list_Opcoes.Count > 0 ? list_Opcoes.Last().Key : 0;
            foreach (string s in dt.Rows[0]["sOpcoes"].ToString().Split('|'))
            {
                if (s.Length > 0)
                {
                    i++;
                    list_Opcoes.Add(i, string.Format("{0}|{1}", i, s));
                }
            }
        }

        protected bool ValidarDados()
        {
            if (txtsDscCategoria.Text.Length < 3)
            {
                MensagemPagina.MostraMensagem_Erro("A descrição da Categoria de Escopo deve possuir ao menos 3 caracteres!", true);
                return false;
            }
            if (gvPergunta.Rows.Count < 1)
            {
                MensagemPagina.MostraMensagem_Erro("É necessário ao menos 1 Pergunta para a Categoria de Escopo!", true);
                return false;
            }
            if (gvOpcao.Rows.Count < 1)
            {
                MensagemPagina.MostraMensagem_Erro("É necessário ao menos 1 Opção para a Categoria de Escopo!", true);
                return false;
            }

            string ordemPergunta = "|";
            string ordemOpcao = "|";
            foreach (GridViewRow row in gvPergunta.Rows)
            {
                if (ordemPergunta.Split('|').Contains((row.Cells[0].FindControl("lblidPergunta") as Label).Text))
                {
                    MensagemPagina.MostraMensagem_Erro("Não podem haver Perguntas com a mesma Ordem!", true);
                    return false;
                }

                ordemPergunta += (row.Cells[0].FindControl("lblidPergunta") as Label).Text + "|";
            }
            foreach (GridViewRow row in gvOpcao.Rows)
            {
                if (ordemOpcao.Split('|').Contains((row.Cells[0].FindControl("lblidOpcao") as Label).Text))
                {
                    MensagemPagina.MostraMensagem_Erro("Não podem haver Opções com a mesma Ordem!", true);
                    return false;
                }

                ordemOpcao += (row.Cells[0].FindControl("lblidOpcao") as Label).Text + "|";
            }

            return true;
        }

        #endregion

        #region | Eventos

        protected void cmdIncluirPergunta_Click(object sender, EventArgs e)
        {
            if (txtPergunta.Text.Length < 3)
                MensagemPagina_IncluirPerguntas.MostraMensagem_Erro("A Pergunta deve possuir ao menos 3 caracteres!", false);
            else
            {
                if (list_Perguntas.Count > 0)
                    list_Perguntas.Add(list_Perguntas.Last().Key + 1, string.Format("{0}|{1}", Convert.ToInt32(list_Perguntas.Last().Value.Split('|')[0]) + 1, txtPergunta.Text));
                else
                    list_Perguntas.Add(1, string.Format("{0}|{1}", 1, txtPergunta.Text));
            }

            gvPergunta.DataSource = list_Perguntas.OrderBy(p => p.Value.Split('|')[0]);
            gvPergunta.DataBind();

            txtPergunta.Text = "";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Focus_Pergunta", "$('[id*=txtPergunta]').focus();", true);
        }

        protected void cmdIncluirOpcao_Click(object sender, EventArgs e)
        {
            if (txtOpcao.Text.Length < 3)
                MensagemPagina_IncluirOpcoes.MostraMensagem_Erro("A Opção deve possuir ao menos 3 caracteres!", false);
            else
            {
                if (list_Opcoes.Count > 0)
                    list_Opcoes.Add(list_Opcoes.Last().Key + 1, string.Format("{0}|{1}", Convert.ToInt32(list_Opcoes.Last().Value.Split('|')[0]) + 1, txtOpcao.Text));
                else
                    list_Opcoes.Add(1, string.Format("{0}|{1}", 1, txtOpcao.Text));
            }

            gvOpcao.DataSource = list_Opcoes.OrderBy(o => o.Value.Split('|')[0]);
            gvOpcao.DataBind();

            txtOpcao.Text = "";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Focus_Opcao", "$('[id*=txtOpcao]').focus();", true);
        }

        protected void cmdAlterarPergunta_Click(object sender, EventArgs e)
        {
            if (int.Parse(txtOrdemPergunta.Text) > 0)
            {
                if (txtPergunta.Text.Length >= 3)
                {
                    var pergunta = list_Perguntas.Where(p => p.Key.ToString().Equals(hddEditarPergunta.Value)).FirstOrDefault();

                    if (!pergunta.Equals(default(KeyValuePair<int, string>)))
                    {
                        list_Perguntas[int.Parse(hddEditarPergunta.Value)] = string.Format("{0}|{1}", txtOrdemPergunta.Text, txtPergunta.Text);

                        txtOrdemPergunta.Text = "";
                        txtPergunta.Text = "";

                        div_OrdemPergunta.Visible = false;
                        div_Pergunta.Attributes["class"] = "col-lg-10";
                        cmdSalvar.Visible = true;
                        div_cmdIncluirOpcao.Visible = true;
                        div_cmdIncluirPergunta.Visible = true;
                        div_cmdAlterarPergunta.Visible = false;

                        FUNCOES.ReexibirColunas(gvPergunta, "Editar", "Excluir");
                        FUNCOES.ReexibirColunas(gvOpcao, "Editar", "Excluir");

                        gvPergunta.DataSource = list_Perguntas.OrderBy(p => p.Value.Split('|')[0]);
                        gvPergunta.DataBind();

                        gvOpcao.DataSource = list_Opcoes.OrderBy(o => o.Value.Split('|')[0]);
                        gvOpcao.DataBind();
                    }
                    else
                        MensagemPagina_IncluirPerguntas.MostraMensagem_Erro("Não foi possível encontrar a Pergunta a ser Editada!", false);
                }
                else
                    MensagemPagina_IncluirPerguntas.MostraMensagem_Erro("A Pergunta deve possuir ao menos 3 caracteres!", false);
            }
            else
                MensagemPagina_IncluirPerguntas.MostraMensagem_Erro("É necesário que a Ordem seja maior que Zero!", false);
        }

        protected void cmdAlterarOpcao_Click(object sender, EventArgs e)
        {
            if (int.Parse(txtOrdemOpcao.Text) > 0)
            {
                if (txtOpcao.Text.Length >= 3)
                {
                    var opcao = list_Opcoes.Where(p => p.Key.ToString().Equals(hddEditarOpcao.Value)).FirstOrDefault();

                    if (!opcao.Equals(default(KeyValuePair<int, string>)))
                    {
                        list_Opcoes[int.Parse(hddEditarOpcao.Value)] = string.Format("{0}|{1}", txtOrdemOpcao.Text, txtOpcao.Text);

                        txtOrdemOpcao.Text = "";
                        txtOpcao.Text = "";

                        div_OrdemOpcao.Visible = false;
                        div_Opcao.Attributes["class"] = "col-lg-10";
                        cmdSalvar.Visible = true;
                        div_cmdIncluirPergunta.Visible = true;
                        div_cmdIncluirOpcao.Visible = true;
                        div_cmdAlterarOpcao.Visible = false;

                        FUNCOES.ReexibirColunas(gvPergunta, "Editar", "Excluir");
                        FUNCOES.ReexibirColunas(gvOpcao, "Editar", "Excluir");

                        gvPergunta.DataSource = list_Perguntas.OrderBy(p => p.Value.Split('|')[0]);
                        gvPergunta.DataBind();

                        gvOpcao.DataSource = list_Opcoes.OrderBy(o => o.Value.Split('|')[0]);
                        gvOpcao.DataBind();
                    }
                    else
                        MensagemPagina_IncluirOpcoes.MostraMensagem_Erro("Não foi possível encontrar a Opção a ser Editada!", false);
                }
                else
                    MensagemPagina_IncluirOpcoes.MostraMensagem_Erro("A Opção deve possuir ao menos 3 caracteres!", false);
            }
            else
                MensagemPagina_IncluirOpcoes.MostraMensagem_Erro("É necesário que a Ordem seja maior que Zero!", false);
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            SalvarDados();
        }

        protected void cmdDetalhe_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina(string.Format("App/Paginas/Comercial/Manutencao/CategoriaEscopo_Detalhe.aspx?id={0}", Request["msg"]));
        }

        protected void gvPergunta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string value = (e.Row.Cells[0].FindControl("lblidPergunta") as Label).Text;

                (e.Row.Cells[0].FindControl("lblidPergunta") as Label).Text = value.Split('|')[0];
                (e.Row.Cells[0].FindControl("lblsDscPergunta") as Label).Text = value.Split('|')[1];
            }
        }

        protected void gvOpcao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string value = (e.Row.Cells[0].FindControl("lblidOpcao") as Label).Text;

                (e.Row.Cells[0].FindControl("lblidOpcao") as Label).Text = value.Split('|')[0];
                (e.Row.Cells[0].FindControl("lblsDscOpcao") as Label).Text = value.Split('|')[1];
            }
        }

        protected void gvPergunta_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Editar")
            {
                div_OrdemPergunta.Visible = true;
                div_Pergunta.Attributes["class"] = "col-lg-8";
                cmdSalvar.Visible = false;
                div_cmdIncluirOpcao.Visible = false;
                div_cmdIncluirPergunta.Visible = false;
                div_cmdAlterarPergunta.Visible = true;

                hddEditarPergunta.Value = e.CommandArgument.ToString();
                txtOrdemPergunta.Text = list_Perguntas.Where(o => o.Key == int.Parse(e.CommandArgument.ToString())).First().Value.Split('|')[0];
                txtPergunta.Text = list_Perguntas.Where(o => o.Key == int.Parse(e.CommandArgument.ToString())).First().Value.Split('|')[1];

                FUNCOES.EsconderColunas(gvPergunta, "Editar", "Excluir");
                FUNCOES.EsconderColunas(gvOpcao, "Editar", "Excluir");

                gvPergunta.DataSource = list_Perguntas.OrderBy(p => p.Value.Split('|')[0]);
                gvPergunta.DataBind();

                gvOpcao.DataSource = list_Opcoes.OrderBy(o => o.Value.Split('|')[0]);
                gvOpcao.DataBind();
            }
        }

        protected void gvOpcao_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Editar")
            {
                div_OrdemOpcao.Visible = true;
                div_Opcao.Attributes["class"] = "col-lg-8";
                cmdSalvar.Visible = false;
                div_cmdIncluirPergunta.Visible = false;
                div_cmdIncluirOpcao.Visible = false;
                div_cmdAlterarOpcao.Visible = true;

                hddEditarOpcao.Value = e.CommandArgument.ToString();
                txtOrdemOpcao.Text = list_Opcoes.Where(o => o.Key == int.Parse(e.CommandArgument.ToString())).First().Value.Split('|')[0];
                txtOpcao.Text = list_Opcoes.Where(o => o.Key == int.Parse(e.CommandArgument.ToString())).First().Value.Split('|')[1];

                FUNCOES.EsconderColunas(gvPergunta, "Editar", "Excluir");
                FUNCOES.EsconderColunas(gvOpcao, "Editar", "Excluir");

                gvPergunta.DataSource = list_Perguntas.OrderBy(p => p.Value.Split('|')[0]);
                gvPergunta.DataBind();

                gvOpcao.DataSource = list_Opcoes.OrderBy(o => o.Value.Split('|')[0]);
                gvOpcao.DataBind();
            }
        }

        protected void gvPergunta_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(gvPergunta.DataKeys[e.RowIndex]["Key"].ToString());
            list_Perguntas.Remove(id);

            list_Perguntas.Where(p => p.Key > id).ToList().ForEach(p => list_Perguntas[p.Key] = (Convert.ToInt32(p.Value.Split('|')[0]) - 1).ToString() + "|" + p.Value.Split('|')[1]);

            gvPergunta.DataSource = list_Perguntas.OrderBy(p => p.Value.Split('|')[0]);
            gvPergunta.DataBind();
        }

        protected void gvOpcao_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(gvOpcao.DataKeys[e.RowIndex]["Key"].ToString());
            list_Opcoes.Remove(id);

            list_Opcoes.Where(o => o.Key > id).ToList().ForEach(o => list_Opcoes[o.Key] = (Convert.ToInt32(o.Value.Split('|')[0]) - 1).ToString() + "|" + o.Value.Split('|')[1]);

            gvOpcao.DataSource = list_Opcoes.OrderBy(o => o.Value.Split('|')[0]);
            gvOpcao.DataBind();
        }

        #endregion
    }
}