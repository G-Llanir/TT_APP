using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT_Flow.FrameWork;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using RETORNO = TT.FrameWork.BD.Retorno;
using IDENTITY = TT.FrameWork.Identity;
using TT_Flow.App.Controles;

namespace TT_Flow.App.Paginas.Comercial
{
    public partial class Escopo_Detalhe : System.Web.UI.Page
    {
        #region | Contrutores

        List<cls_Categoria> list_Categorias
        {
            get
            {
                if (ViewState["list_Categorias"] == null)
                {
                    ViewState["list_Categorias"] = new List<cls_Categoria>();
                }
                return (List<cls_Categoria>)ViewState["list_Categorias"];
            }
            set
            {
                ViewState["list_Categorias"] = value;
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
                    FUNCOES.ValidaPermissao(Permissao.Comercial.Escopo.Incluir, true);
                    FUNCOES.DirecionaPagina("App/Paginas/Comercial/Manutencao/Escopo_Detalhe.aspx?id=0");
                }
                else if (Request["id"] == "0")
                {
                    FUNCOES.ValidaPermissao(Permissao.Comercial.Escopo.Incluir, true);
                    Pesquisar("0");
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Comercial.Escopo.Consultar, true);
                    Pesquisar(Request["id"]);
                }

                if (Request["msg"] == "1")
                    MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso!");
            }
        }

        void Pesquisar(string idEscopo)
        {
            string sErro = "";

            PopulaCombos();

            Dictionary<string, string> vParametros = new Dictionary<string, string>()
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idEscopo", idEscopo }
            };

            DataSet dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Escopo", vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                if (idEscopo != "0")
                {
                    hddidEscopo.Value = RETORNO.DATASET(dsPesquisa, 0, 0, "idEscopo");
                    txtidEscopo.Text = RETORNO.DATASET(dsPesquisa, 0, 0, "idEscopo");
                    txtsDscEscopo.Text = RETORNO.DATASET(dsPesquisa, 0, 0, "sDscEscopo");
                    ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, 0, "sAtivo"));

                    lblTituloPagina.Text = string.Format("Editar {0}", txtsDscEscopo.Text);

                    PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, 0, "sDscUsuarioAtualizacao"));

                    Popula_list_Categorias(dsPesquisa.Tables[1]);

                    if (ddlCategoria.Items.Count <= 1)
                        div_IncluirCategoria.Visible = false;

                    if (!FUNCOES.ValidaPermissao(Permissao.Comercial.Escopo.Alterar))
                    {
                        txtsDscEscopo.ReadOnly = true;
                        ComboAtivo.Situacao_BloquearEdicao(false);
                        ddlCategoria.Attributes.Add("disabled", "disabled");
                        cmdIncluirCategoria.Visible = false;

                        FUNCOES.EsconderColunas(gvCategoria, "Excluir");

                        cmdSalvar.Visible = false;
                    }
                }
            }
            else
            {
                txtidEscopo.Text = "Novo";
                lblTituloPagina.Text = "Novo Escopo";

                PainelAtualizacao.Visible = false;
            }
        }

        #endregion

        #region | Combos

        protected void PopulaCombos()
        {
            FUNCOES.Popula_Combo(ddlCategoria, "sp_Select 'tbl_Flow_Comercial_Escopo_Categoria'", "idCategoria", "sDscCategoria", false, "Selecione um Categoria", "0");
        }

        #endregion

        #region | gvCategoria

        protected void Popula_list_Categorias(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                cls_Categoria categoria = new cls_Categoria();
                
                categoria.idCategoria = Convert.ToInt32(dr["idCategoria"]);
                categoria.sDscCategoria = dr["sDscCategoria"].ToString();

                list_Categorias.Add(categoria);
            }

            gvCategoria_DataBind();
        }

        protected void gvCategoria_DataBind()
        {
            gvCategoria.DataSource = list_Categorias.OrderBy(s => s.idCategoria);
            gvCategoria.DataBind();

            if (ddlCategoria.Items.Count > 1)
                div_IncluirCategoria.Visible = true;
            else
                div_IncluirCategoria.Visible = false;
        }

        protected void gvCategoria_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ddlCategoria.Items.Remove(ddlCategoria.Items.FindByValue(e.Row.Cells[0].Text));
            }
        }

        protected void gvCategoria_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idCategoria = int.Parse(gvCategoria.Rows[e.RowIndex].Cells[0].Text);

            try
            {
                list_Categorias.Remove(list_Categorias.Where(s => s.idCategoria.Equals(idCategoria)).First());
                PopulaCombos();
                gvCategoria_DataBind();
            }
            catch (Exception ex)
            {
                MensagemPagina_gvCategoria.MostraMensagem_Erro("Erro ao excluir Categoria!\r\nErro: " + ex.Message);
            }
        }

        #endregion

        #region | Salvar + Classe

        protected void SalvarDados()
        {
            if (ValidarDados())
            {
                try
                {
                    string idEscopo = hddidEscopo.Value;
                    string sidCategoria = "|";
                    list_Categorias.ForEach(c => sidCategoria += c.idCategoria + "|");

                    Dictionary<string, string> vParametrosSalvar = new Dictionary<string, string>()
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idEscopo", idEscopo },
                        { "@sDscEscopo", txtsDscEscopo.Text },
                        { "@sidCategoriasEscopo", sidCategoria },
                        { "@sAtivo", ComboAtivo.Situacao_Recuperar() },
                        { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                    };

                    DataSet dsSalvar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Escopo", vParametrosSalvar);

                    if (BD.ValidarDataSet(dsSalvar))
                    {
                        idEscopo = RETORNO.DATASET(dsSalvar, 0, 0, "idEscopo");
                        FUNCOES.DirecionaPagina(string.Format("App/Paginas/Comercial/Escopo_Detalhe.aspx?id={0}&msg=1", idEscopo));
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

        protected bool ValidarDados()
        {
            if (txtsDscEscopo.Text.Length < 3)
            {
                MensagemPagina.MostraMensagem_Erro("A descrição do Escopo deve possuir ao menos 3 caracteres!");
                return false;
            }
            if (gvCategoria.Rows.Count < 1)
            {
                MensagemPagina.MostraMensagem_Erro("É necessário ao menos 1 Categoria para o Escopo!");
                return false;
            }

            return true;
        }

        #endregion

        #region | Eventos

        protected void cmdIncluirCategoria_Click(object sender, EventArgs e)
        {
            if (ddlCategoria.SelectedValue == "0")
                MensagemPagina_IncluirCategoria.MostraMensagem_Erro("Selecione uma Categoria de Escopo para Incluir!", false);
            else
            {
                cls_Categoria novoCategoria = new cls_Categoria();

                novoCategoria.idCategoria = int.Parse(ddlCategoria.SelectedValue);
                novoCategoria.sDscCategoria = ddlCategoria.SelectedItem.Text;

                list_Categorias.Add(novoCategoria);
            }

            gvCategoria_DataBind();
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            SalvarDados();
        }

        #endregion
    }
}