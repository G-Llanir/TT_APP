using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using System.Web.UI.WebControls;
using System.Web;

namespace TT_Colaborador.Aplicativo.Paginas.Conversas
{
    public partial class Conversas_Dados : Page
    {
        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Area";

        #region | Funções Incialização do Form

        protected void Page_Load(object sender, EventArgs e)
        {
            lblTituloPagina.Text = HttpUtility.HtmlDecode("Conversas");
            FUNCOES.ValidaPermissao(Permissao.Conversas.Consultar, true, true);

            bool bIncluir = FUNCOES.ValidaPermissao(Permissao.Conversas.Incluir, false, true);

            div_EditarConversa.Visible = bIncluir;
            Div_Conversas.Visible = bIncluir;
            cmdNovaConversa.Visible = bIncluir;

            if (!IsPostBack)
            {
                string idUsuarioLogado = IDENTITY.Variaveis.idUsuario();
                Pesquisar(idUsuarioLogado);
            }
            else
            {
                var requestTarget = this.Request["__EVENTTARGET"];

                if (requestTarget == "funcao_SAIR")
                    FUNCOES.DirecionaPagina("/Aplicativo/MenuColaborador.aspx");
            }
        }

        protected void Pesquisar(string idUsuario)
        {
            try
            {
                if (idUsuario != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR-USUARIO" },
                        { "@idUsuarioIntegrado", idUsuario }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    BD.ValidarDataSet(dsPesquisa, out string sErro);

                    hddidColaborador.Value = RETORNO.DATASET(dsPesquisa, 0, "idColaborador");

                    pesquisarConversas(hddidColaborador.Value, false);
                    FUNCOES.Popula_Combo(ddlConversas_idTipoEvento, "sp_Select 'tbl_Flow_Colaboradores_Conversas_TipoEvento'", Convert.ToInt32(IDENTITY.Variaveis.idUsuario()), "idTipoEvento", "sDscTipoEvento", false, "Selecione um Tipo", "0");
                }

            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        #endregion

        #region | Conversas

        void pesquisarConversas(string idColaborador, bool edicao)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR-CONVERSAS" },
                { "@idColaborador", idColaborador },
                { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() },
                { "@sApenasRh", "N" }
            };
            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_Area", vParametros);

            DIV_CONVERSA.Visible = true;

            if (edicao)
                DIV_CONVERSA.Visible = true;

            rptConversas.DataSource = ds;
            rptConversas.DataBind();
        }

        protected void rptConversas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRow row = ((DataRowView)e.Item.DataItem).Row;

                (e.Item.FindControl("lnkConversasEditar") as LinkButton).Text = "Ver Detalhes";
                (e.Item.FindControl("litDscConversa") as Literal).Text = row["sDscConversa"].ToString();
                (e.Item.FindControl("litDtEventoConversa") as Literal).Text = row["dtEventoConversa"].ToString();
                (e.Item.FindControl("litDscTipoEvento") as Literal).Text = row["sDscTipoEvento"].ToString();
                (e.Item.FindControl("litDscAcao") as Literal).Text = row["sDscAcao"].ToString();
                (e.Item.FindControl("litDscStatusConversa") as Literal).Text = row["sDscStatusConversa"].ToString();

                Literal litnQtdConversasPendentes = e.Item.FindControl("litnQtdConversasPendentes") as Literal;
                litnQtdConversasPendentes.Text = row["nQtdConversasPendentes"].ToString();
                litnQtdConversasPendentes.Visible = false;
            }
        }

        protected void lnkConversasEditar_Click(object sender, EventArgs e)
        {
            string idRegistroConvesa = (sender as LinkButton).CommandArgument;

            if (idRegistroConvesa != "0")
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONVERSA-DETALHES" },
                    { "@idRegistroConvesa", idRegistroConvesa },
                    { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                };
                DataSet dsEditar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_Area", vParametros);

                Div_Conversas.Visible = true;
                DIV_CONVERSA.Visible = true;
                Div_Inclusao.Visible = false;
                div_EditarConversa.Visible = FUNCOES.ValidaPermissao(Permissao.Conversas.Editar, false, true);

                if (BD.ValidarDataSet(dsEditar, out string sErro))
                {
                    hddConversa_idRegistroConvesa.Value = idRegistroConvesa;
                    txtConversas_sDscConversa.Text = RETORNO.DATASET(dsEditar, 0, "sDscConversa");
                    lblTituloPagina.Text = string.Format("Conversas - {0}", RETORNO.DATASET(dsEditar, 0, "sDscConversa"));

                    hddidStatus.Value = RETORNO.DATASET(dsEditar, 0, "idStatusConversa");

                    ddlConversas_idTipoEvento.SelectedValue = RETORNO.DATASET(dsEditar, 0, "idTipoEvento");

                    popularHistorico(idRegistroConvesa);
                }
            }
        }

        protected void popularHistorico(string idRegistroConvesa)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONVERSA-HISTORICO" },
                { "@idRegistroConvesa", idRegistroConvesa }
            };
            DataSet dsHistorico = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_Area", vParametros);

            rptHistorico.DataSource = dsHistorico;
            rptHistorico.DataBind();
        }

        protected void cmdConversas_Cancelar_Click(object sender, EventArgs e) => pesquisarConversas(hddidColaborador.Value, false);

        protected void cmdNovaConversa_Click(object sender, EventArgs e)
        {
            Div_Conversas.Visible = true;
            DIV_CONVERSA.Visible = true;
            Div_Inclusao.Visible = true;
            LimpaCampos_Conversas();

            rptHistorico.DataSource = null;
            rptHistorico.DataBind();
        }

        protected void cmdConversas_Salvar_Click(object sender, EventArgs e)
        {
            if (ValidarDados_Conversas())
            {
                try
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR-CONVERSA" },
                        { "@idRegistroConvesa", hddConversa_idRegistroConvesa.Value },
                        { "@idColaborador", hddidColaborador.Value },
                        { "@sDscConversa", txtConversas_sDscConversa.Text },
                        { "@sObservacaoConversa", txtConversas_sObservacaoConversa.Text },
                        { "@idMeio", "6" },
                        { "@idTipoEvento", ddlConversas_idTipoEvento.SelectedValue },
                        { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                    };

                    if (!string.IsNullOrEmpty(hddidStatus.Value))
                        vParametros.Add("@idStatusConversa", hddidStatus.Value);

                    DataSet dsSalvar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_Area", vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out string sErro))
                    {
                        pesquisarConversas(hddidColaborador.Value, true);

                        MensagemPaginaConversas.MostraMensagem_Sucesso("Mensagem Enviada!");

                        if (hddConversa_idRegistroConvesa.Value != "")
                            popularHistorico(hddConversa_idRegistroConvesa.Value);
                        else
                            pesquisarConversas(hddidColaborador.Value, false);

                        txtConversas_sObservacaoConversa.Text = "";
                        LimpaCampos_Conversas();
                    }
                    else
                    {
                        throw new Exception("BD: " + sErro.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MensagemPaginaConversas.MostraMensagem_Erro(ex.Message);
                }
            }
        }

        void LimpaCampos_Conversas()
        {
            txtConversas_sDscConversa.Text = "";
            txtConversas_sObservacaoConversa.Text = "";
            ddlConversas_idTipoEvento.SelectedValue = "0";
            hddConversa_idRegistroConvesa.Value = "";
        }

        private bool ValidarDados_Conversas()
        {
            if (string.IsNullOrEmpty(txtConversas_sDscConversa.Text))
            {
                string sMensagemErro = "Informe um Assunto!";
                MensagemPaginaConversas.MostraMensagem_Erro(sMensagemErro);

                return false;
            }
            if (string.IsNullOrEmpty(txtConversas_sObservacaoConversa.Text))
            {
                string sMensagemErro = "É Preciso Digitar uma Mensagem!";
                MensagemPaginaConversas.MostraMensagem_Erro(sMensagemErro);

                return false;
            }
            if (string.IsNullOrEmpty(ddlConversas_idTipoEvento.SelectedValue) || ddlConversas_idTipoEvento.SelectedValue == "0")
            {
                string sMensagemErro = "Campo Solicitação é obrigatório!";
                MensagemPaginaConversas.MostraMensagem_Erro(sMensagemErro);

                return false;
            }
            if (txtConversas_sDscConversa.MaxLength > 200)
            {
                string sMensagemErro = "Campo Assunto tem Limite de 200 Caracteres";
                MensagemPaginaConversas.MostraMensagem_Erro(sMensagemErro);

                return false;
            }

            return true;
        }

        protected string GetLiClass(object idUsuarioItem)
        {
            var logado = IDENTITY.Variaveis.idUsuario();
            string idUsuarioAtualizacao = idUsuarioItem.ToString();

            if (logado != idUsuarioAtualizacao)
                return "right";
            else
                return "left";
        }

        protected string GetLiImagemColaborador(object vbImagem)
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

        #endregion

    }
}