using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using RETORNO = TT.FrameWork.BD.Retorno;
using IDENTITY = TT.FrameWork.Identity;
using TT_Flow.FrameWork;

namespace TT_Flow.App.Paginas.Comercial.Manutencao
{
    public partial class TipoOrcamento_Detalhe : System.Web.UI.Page
    {
        const string sTituloCalcularInstalacao = "Calcular Instalação?";

        #region | Contrutores

        List<cls_Fluxo> list_Fluxos
        {
            get
            {
                if (ViewState["list_Fluxos"] == null)
                {
                    ViewState["list_Fluxos"] = new List<cls_Fluxo>();
                }
                return (List<cls_Fluxo>)ViewState["list_Fluxos"];
            }
            set
            {
                ViewState["list_Fluxos"] = value;
            }
        }

        List<cls_Servico_Recursos> list_TipoServico
        {
            get
            {
                if (ViewState["list_TipoServico"] == null)
                {
                    ViewState["list_TipoServico"] = new List<cls_Servico_Recursos>();
                }
                return (List<cls_Servico_Recursos>)ViewState["list_TipoServico"];
            }
            set
            {
                ViewState["list_TipoServico"] = value;
            }
        }

        List<cls_Escopo> list_Escopos
        {
            get
            {
                if (ViewState["list_Escopos"] == null)
                {
                    ViewState["list_Escopos"] = new List<cls_Escopo>();
                }
                return (List<cls_Escopo>)ViewState["list_Escopos"];
            }
            set
            {
                ViewState["list_Escopos"] = value;
            }
        }

        #endregion

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "Manual-TipoOrcamento.pdf";

            if (!IsPostBack)
            {
                if (Request["id"] == null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Comercial.Manutencao.TipoOrcamento.Incluir, true);
                    FUNCOES.DirecionaPagina("App/Paginas/Comercial/Manutencao/TipoOrcamento_Detalhe.aspx?id=0");
                }
                else if (Request["id"] == "0")
                {
                    FUNCOES.ValidaPermissao(Permissao.Comercial.Manutencao.TipoOrcamento.Incluir, true);
                    Pesquisar("0");
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Comercial.Manutencao.TipoOrcamento.Consultar, true);
                    Pesquisar(Request["id"]);
                }

                if (Request["msg"] == "1")
                    MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso!");
            }
        }

        void Pesquisar(string idTipoOrcamento)
        {
            string sErro = "";

            PopulaCombos();

            Dictionary<string, string> vParametros = new Dictionary<string, string>()
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idTipoOrcamento", idTipoOrcamento }
            };

            DataSet dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_Orcamento_Tipo", vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                if (idTipoOrcamento != "0")
                {
                    hddidTipoOrcamento.Value = RETORNO.DATASET(dsPesquisa, 0, 0, "idTipoOrcamento");
                    txtidTipoOrcamento.Text = RETORNO.DATASET(dsPesquisa, 0, 0, "idTipoOrcamento");
                    txtsDscTipoOrcamento.Text = RETORNO.DATASET(dsPesquisa, 0, 0, "sDscTipoOrcamento");
                    ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, 0, "sAtivo"));
                    SwitchCalcularInstalacao.Definir(RETORNO.DATASET(dsPesquisa, 0, 0, "sCalcularInstalacao"), sTituloCalcularInstalacao, "");

                    lblTituloPagina.Text = string.Format("Editar Tipo de Orçamento {0}", txtsDscTipoOrcamento.Text);

                    PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, 0, "sDscUsuarioAtualizacao"));

                    Popula_list_Fluxo(dsPesquisa.Tables[1]);
                    Popula_list_Servico_Recurso(dsPesquisa.Tables[2]);
                    Popula_list_Escopo(dsPesquisa.Tables[3]);

                    if (ddlFluxo.Items.Count <= 1)
                        div_IncluirFluxo.Visible = false;

                    if (!FUNCOES.ValidaPermissao(Permissao.Comercial.Manutencao.TipoOrcamento.Alterar))
                    {
                        txtsDscTipoOrcamento.ReadOnly = true;
                        ComboAtivo.Situacao_BloquearEdicao(false);
                        SwitchCalcularInstalacao.BloquearEdicao(true);
                        ddlFluxo.Attributes.Add("disabled", "disabled");
                        cmdIncluirFluxo.Visible = false;

                        ddlTipoServico.Attributes.Add("disabled", "disabled");
                        cmdIncluirTipoServico.Visible = false;

                        ddlEscopo.Attributes.Add("disabled", "disabled");
                        cmdIncluirEscopo.Visible = false;

                        FUNCOES.EsconderColunas(gvFluxo, "Excluir");
                        FUNCOES.EsconderColunas(gvTiposServicos, "Excluir");
                        FUNCOES.EsconderColunas(gvEscopo, "Excluir");

                        cmdSalvar.Visible = false;
                    }
                }
            }
            else
            {
                txtidTipoOrcamento.Text = "Novo";
                lblTituloPagina.Text = "Novo Tipo de Orçamento";

                // Tipo novo nasce sem cálculo de instalação, igual aos tipos já existentes.
                SwitchCalcularInstalacao.Definir("N", sTituloCalcularInstalacao, "");

                PainelAtualizacao.Visible = false;
            }
        }

        #endregion

        #region | Combos

        protected void PopulaCombos()
        {
            FUNCOES.Popula_Combo(ddlFluxo, "sp_Select 'FLOW_Fluxo', @sPesquisa='1|2|5'", "idFluxo", "sDscFluxo", false, "Selecione um Fluxo", "0");
            FUNCOES.Popula_Combo(ddlTipoServico, "sp_Select 'Flow_Produtos_Tipo', 11", "idTipoProduto", "sDscTipoProduto", false, "Selecione um Tipo de Serviço", "0");
            FUNCOES.Popula_Combo(ddlEscopo, "sp_Select 'tbl_Flow_Comercial_Escopo'", "idEscopo", "sDscEscopo", false, "Selecione um Escopo", "0");
        }

        #endregion

        #region | gvFluxo

        protected void Popula_list_Fluxo(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                cls_Fluxo fluxo = new cls_Fluxo();

                fluxo.sFuncao = "CONSULTAR_FLUXO";
                fluxo.idFluxo = Convert.ToInt32(dr["idFluxo"]);
                fluxo.sDscFluxo = dr["sDscFluxo"].ToString();

                list_Fluxos.Add(fluxo);
            }

            gvFluxo_DataBind();
        }

        protected void gvFluxo_DataBind()
        {
            gvFluxo.DataSource = list_Fluxos.Where(s => s.sFuncao != "EXCLUIR_FLUXO" && s.sFuncao != "INEXISTENTE").OrderBy(s => s.idFluxo);
            gvFluxo.DataBind();

            if (ddlFluxo.Items.Count > 1) 
                div_IncluirFluxo.Visible = true;
            else
                div_IncluirFluxo.Visible = false;
        }

        protected void gvFluxo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
                ddlFluxo.Items.Remove(ddlFluxo.Items.FindByValue(e.Row.Cells[0].Text));
        }

        protected void gvFluxo_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idFluxo = int.Parse(gvFluxo.Rows[e.RowIndex].Cells[0].Text);

            try
            {
                list_Fluxos.Where(s => s.idFluxo.Equals(idFluxo)).First().sFuncao = list_Fluxos.Where(s => s.idFluxo.Equals(idFluxo)).First().sFuncao != "INCLUIR_FLUXO" ? "EXCLUIR_FLUXO" : "INEXISTENTE";
                PopulaCombos();
                gvFluxo_DataBind();
            }
            catch (Exception ex)
            {
                MensagemPagina_gvFluxo.MostraMensagem_Erro("Erro ao excluir Fluxo!\r\nErro: " + ex.Message);
            }
        }

        #endregion

        #region | gvEscopo
        protected void Popula_list_Escopo(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                cls_Escopo escopo = new cls_Escopo();

                escopo.idEscopo = Convert.ToInt32(dr["idEscopo"]);
                escopo.sDscEscopo = dr["sDscEscopo"].ToString();

                list_Escopos.Add(escopo);
            }

            gvEscopo_DataBind();
        }

        protected void gvEscopo_DataBind()
        {
            gvEscopo.DataSource = list_Escopos.OrderBy(e => e.idEscopo);
            gvEscopo.DataBind();

            if (ddlEscopo.Items.Count > 1)
                div_IncluirEscopos.Visible = true;
            else
                div_IncluirEscopos.Visible = false;
        }

        protected void gvEscopo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
                ddlEscopo.Items.Remove(ddlEscopo.Items.FindByValue(e.Row.Cells[0].Text));
        }

        protected void gvEscopo_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idEscopo = int.Parse(gvEscopo.Rows[e.RowIndex].Cells[0].Text);

            try
            {
                list_Escopos.Remove(list_Escopos.Where(es => es.idEscopo == idEscopo).First());
                PopulaCombos();
                gvEscopo_DataBind();
            }
            catch (Exception ex)
            {
                MensagemEscopo.MostraMensagem_Erro("Erro ao excluir Escopo!\r\nErro: " + ex.Message);
            }
        }

        #endregion

        #region | gvTipoServico

        protected void Popula_list_Servico_Recurso(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                cls_Servico_Recursos tipoServico = new cls_Servico_Recursos();

                tipoServico.sFuncao = "CONSULTAR_FLUXO";
                tipoServico.idTipoServico_Recurso = Convert.ToInt32(dr["idTipoProduto"]);
                tipoServico.sDscTipoServico_Recurso = dr["sDscTipoProduto"].ToString();

                list_TipoServico.Add(tipoServico);
            }

            gvServico_Recurso_DataBind();
        }

        protected void gvServico_Recurso_DataBind()
        {
            gvTiposServicos.DataSource = list_TipoServico.Where(s => s.sFuncao != "EXCLUIR_TIPO_SERVICO" && s.sFuncao != "INEXISTENTE").OrderBy(s => s.idTipoServico_Recurso);
            gvTiposServicos.DataBind();

            if (ddlTipoServico.Items.Count > 1)
                div_IncluirServico_Recurso.Visible = true;
            else
                div_IncluirServico_Recurso.Visible = false;
        }

        protected void gvTiposServicos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
                ddlTipoServico.Items.Remove(ddlTipoServico.Items.FindByValue(e.Row.Cells[0].Text));
        }

        protected void gvTiposServicos_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idTipoServico = int.Parse(gvTiposServicos.Rows[e.RowIndex].Cells[0].Text);

            try
            {
                list_TipoServico.Where(ts => ts.idTipoServico_Recurso.Equals(idTipoServico)).First().sFuncao = list_TipoServico.Where(s => s.idTipoServico_Recurso.Equals(idTipoServico)).First().sFuncao != "INCLUIR_TIPO_SERVICO" ? "EXCLUIR_TIPO_SERVICO" : "INEXISTENTE";
                PopulaCombos();
                gvServico_Recurso_DataBind();
            }
            catch (Exception ex)
            {
                MensagemEscopo.MostraMensagem_Erro("Erro ao excluir Tipo de Serviços!\r\nErro: " + ex.Message);
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
                    string idTipoOrcamento = hddidTipoOrcamento.Value;
                    string escopos = "|";
                    list_Escopos.ForEach(e => escopos += e.idEscopo + "|");

                    Dictionary<string, string> vParametrosSalvar = new Dictionary<string, string>()
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idTipoOrcamento", idTipoOrcamento },
                        { "@sDscTipoOrcamento", txtsDscTipoOrcamento.Text },
                        { "@sAtivo", ComboAtivo.Situacao_Recuperar() },
                        { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() },
                        { "@sidEscopos", escopos },
                        { "@sCalcularInstalacao", SwitchCalcularInstalacao.Recuperar() }
                    };

                    DataSet dsSalvar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_Orcamento_Tipo", vParametrosSalvar);

                    if (BD.ValidarDataSet(dsSalvar))
                    {
                        idTipoOrcamento = RETORNO.DATASET(dsSalvar, 0, 0, "idTipoOrcamento");

                        if (SalvarComposicao(idTipoOrcamento))
                            FUNCOES.DirecionaPagina(string.Format("App/Paginas/Comercial/Manutencao/TipoOrcamento_Detalhe.aspx?id={0}&msg=1", idTipoOrcamento));
                    }
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro("Erro no salvamento de dados!\r\nErro:" + ex.Message);
                }
            }
        }

        protected bool SalvarComposicao(string idTipoOrcamento)
        {
            try
            {
                string sProcedure = "sp_Manipula_tbl_Flow_Comercial_Orcamento_Tipo";

                Dictionary<string, string> vParametrosSalvarComposicao = new Dictionary<string, string>()
                {
                    { "@sFuncao", "SALVAR_COMPOSICAO" },
                    { "@idTipoOrcamento", idTipoOrcamento },
                    { "@idTipoComposicao", "" },
                    { "@idObjeto", "" },
                    { "@sDscObjeto", "" },
                    { "@sUnidade", "" }
                };

                foreach (cls_Fluxo fluxo in list_Fluxos)
                {
                    if (fluxo.sFuncao != "INCLUIR_FLUXO" && fluxo.sFuncao != "EXCLUIR_FLUXO")
                        continue;

                    if (fluxo.sFuncao == "EXCLUIR_FLUXO")
                        vParametrosSalvarComposicao["@sFuncao"] = "EXCLUIR_COMPOSICAO";

                    vParametrosSalvarComposicao["@idTipoComposicao"] = "1"; // Fluxo = 1
                    vParametrosSalvarComposicao["@idObjeto"] = fluxo.idFluxo.ToString();
                    vParametrosSalvarComposicao["@sDscObjeto"] = fluxo.sDscFluxo;

                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametrosSalvarComposicao);
                }

                foreach (cls_Servico_Recursos tipoServico in list_TipoServico)
                {
                    if (tipoServico.sFuncao != "INCLUIR_TIPO_SERVICO" && tipoServico.sFuncao != "EXCLUIR_TIPO_SERVICO")
                        continue;

                    if (tipoServico.sFuncao == "EXCLUIR_TIPO_SERVICO")
                        vParametrosSalvarComposicao["@sFuncao"] = "EXCLUIR_COMPOSICAO";

                    vParametrosSalvarComposicao["@idTipoComposicao"] = "2"; // Tipo de Serviço = 2
                    vParametrosSalvarComposicao["@idObjeto"] = tipoServico.idTipoServico_Recurso.ToString();
                    vParametrosSalvarComposicao["@sDscObjeto"] = tipoServico.sDscTipoServico_Recurso;

                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametrosSalvarComposicao);
                }

                return true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Houve um erro no salvamento dos dados de Fluxos e Serviços!\r\n" + ex.Message);
                return false;
            }
        }

        protected void AtualizaClasses(string sFuncao)
        {
            if (sFuncao.Contains("Incluir_"))
            {
                if (sFuncao.Contains("Fluxo"))
                {
                    int idFluxo = Convert.ToInt32(ddlFluxo.SelectedValue);
                    var fluxoExistente = list_Fluxos.Where(s => s.idFluxo == idFluxo).FirstOrDefault();

                    if (fluxoExistente == null)
                    {
                        cls_Fluxo novoFluxo = new cls_Fluxo();

                        novoFluxo.sFuncao = "INCLUIR_FLUXO";
                        novoFluxo.idFluxo = idFluxo;
                        novoFluxo.sDscFluxo = ddlFluxo.SelectedItem.Text;

                        list_Fluxos.Add(novoFluxo);
                    }
                    else
                    {
                        fluxoExistente.sFuncao = fluxoExistente.sFuncao == "INCLUIR_FLUXO" || fluxoExistente.sFuncao == "INEXISTENTE" ? "INCLUIR_FLUXO" : "CONSULTAR_FLUXO";
                        fluxoExistente.idFluxo = idFluxo;
                        fluxoExistente.sDscFluxo = ddlFluxo.SelectedItem.Text;
                    }
                }
                if (sFuncao.Contains("TipoServico"))
                {
                    int idTipoServico = int.Parse(ddlTipoServico.SelectedValue);
                    var tipoServico = list_TipoServico.Where(s => s.idTipoServico_Recurso.Equals(idTipoServico)).FirstOrDefault();

                    if (tipoServico == null)
                    {
                        cls_Servico_Recursos novoTipoServico = new cls_Servico_Recursos();

                        novoTipoServico.sFuncao = "INCLUIR_TIPO_SERVICO";
                        novoTipoServico.idTipoServico_Recurso = idTipoServico;
                        novoTipoServico.sDscTipoServico_Recurso = ddlTipoServico.SelectedItem.Text;

                        list_TipoServico.Add(novoTipoServico);
                    }
                    else
                    {
                        tipoServico.sFuncao = tipoServico.sFuncao == "INCLUIR_TIPO_SERVICO" || tipoServico.sFuncao == "INEXISTENTE" ? "INCLUIR_TIPO_SERVICO" : "CONSULTAR_TIPO_SERVICO";
                        tipoServico.idTipoServico_Recurso = idTipoServico;
                        tipoServico.sDscTipoServico_Recurso = ddlTipoServico.SelectedItem.Text;
                    }
                }
            }

            gvFluxo_DataBind();
            gvServico_Recurso_DataBind();
        }

        #endregion

        #region | Utils

        protected bool ValidarDados()
        {
            if (txtsDscTipoOrcamento.Text.Length < 3)
            {
                MensagemPagina.MostraMensagem_Erro("A descrição do Tipo de Orçamento deve possuir ao menos 3 caracteres!");
                return false;
            }
            if (gvFluxo.Rows.Count < 1)
            {
                MensagemPagina.MostraMensagem_Erro("É necessário ao menos 1 Fluxo para Tipo de Orçamento!");
                return false;
            }

            return true;
        }

        #endregion

        #region | Eventos

        protected void cmdIncluirFluxo_Click(object sender, EventArgs e)
        {
            if (ddlFluxo.SelectedValue == "0")
                MensagemPagina_IncluirFluxo.MostraMensagem_Erro("Selecione um Fluxo para Incluir!", false);
            else
                AtualizaClasses("Incluir_Fluxo");
        }

        protected void cmdIncluirTipoServico_Click(object sender, EventArgs e)
        {
            if (ddlTipoServico.SelectedValue == "0")
                MensagemPagina_IncluirServico_Recurso.MostraMensagem_Erro("Selecione um Tipo de Serviço para Incluir!", false);
            else
                AtualizaClasses("Incluir_TipoServico");
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            SalvarDados();
        }

        protected void cmdIncluirEscopo_Click(object sender, EventArgs e)
        {
            if (ddlEscopo.SelectedValue == "0")
                MensagemIncluirEscopos.MostraMensagem_Erro("Selecione um Escopo para ser Incluído!", false);
            else
            {
                cls_Escopo escopo = new cls_Escopo();

                escopo.idEscopo = Convert.ToInt32(ddlEscopo.SelectedValue);
                escopo.sDscEscopo = ddlEscopo.SelectedItem.Text;

                list_Escopos.Add(escopo);
            }

            gvEscopo_DataBind();
        }

        #endregion
    }
}