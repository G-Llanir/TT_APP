using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using RETORNO = TT.FrameWork.BD.Retorno;
using System.Web.UI.WebControls;
using HtmlAgilityPack;

namespace TT_Colaborador.Aplicativo.Paginas.Mensagem
{
    public partial class Mensagens : Page
    {
        string sPagina_NovoRegistro = "Aplicativo/Paginas/Mensagem/Mensagens_Detalhe.aspx?id=0";
        string sAviso;

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidarPermissaoAcesso();
            FUNCOES.ValidaPermissao(Permissao.Mensagens.Consultar, true, true);

            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlidDepartamento, "sp_Select 'Flow_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Todos os Departamentos ", "0");
                FUNCOES.Popula_Combo(ddlidTipoObjeto, "sp_Select 'Flow_Mensagens_x_TipoObjeto'", "idTipoObjeto", "sDscTipoObjeto", false, "Todos os Tipos", "-99");

                if (Request["sFiltro"] != null)
                {
                    ddlidFiltroMensagem.SelectedValue = Request["sFiltro"];

                    if (Request["sFiltro"] == "2")
                    {
                        spanQtdLidos.Visible = false;
                        spanQtdNaoLidos.Visible = true;
                    }
                    else if (Request["sFiltro"] == "0")
                    {
                        spanQtdLidos.Visible = true;
                        spanQtdNaoLidos.Visible = false;
                    }
                }
                else
                    ddlidFiltroMensagem.SelectedValue = "0";

                if (Request.QueryString["sIsAviso"] == "S")
                {
                    cmdNovo.Visible = false;
                    spanQtdNaoLidos.Visible = false;
                    spanQtdLidos.Visible = false;
                    lblTituloPagina.Text = "Avisos";
                    ddlidFiltroMensagem.SelectedValue = "2";
                }
                else
                {
                    cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.Mensagens.Incluir, false, true);
                    lblTituloPagina.Text = "Mensagens";
                }
            }

            Pesquisar(true);

            txtPesquisa.Focus();
        }

        protected void Pesquisar(bool bPostBack)
        {
            if (Request.QueryString["sIsAviso"] != null)
                sAviso = Request.QueryString["sIsAviso"];
            else
                sAviso = "";

            sAviso = ddlAvisos.SelectedValue;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_MENSAGENS__AREA_COLABORADOR" },
                { "@idUsuarioPesquisa", IDENTITY.Variaveis.idUsuario() },
                { "@sDirecaoMensagem", ddlsDirecaoMensagem.SelectedValue },
                { "@sIsAviso", sAviso },
                { "@idDepartamentoDestino", ddlidDepartamento.SelectedValue.ToString() },
                { "@sAssunto", txtPesquisa.Text.Trim() },
                { "@idFiltroMensagem", ddlidFiltroMensagem.SelectedValue.ToString() },
                { "@idTipoObjeto", ddlidTipoObjeto.SelectedValue }
            };
            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Mensagens", vParametros, false);

            if (ds.Tables[1].Rows.Count > 0)
            {
                divAviso.Visible = true;

                spanQtdLidos.InnerText = string.IsNullOrEmpty(RETORNO.DATASET(ds, "nLidas")) ? "0" : RETORNO.DATASET(ds, "nLidas");
                spanQtdNaoLidos.InnerText = string.IsNullOrEmpty(RETORNO.DATASET(ds, "nNaoLidas")) ? "0" : RETORNO.DATASET(ds, "nNaoLidas");

                rptMessages.DataSource = ds.Tables[1];
                rptMessages.DataBind();
            }
            else if (!bPostBack)
            {
                divAviso.Visible = false;

                if (Request.QueryString["sIsAviso"] == "S" && ddlidFiltroMensagem.SelectedValue.ToString() == "2" && ds.Tables[1].Rows.Count <= 0)
                    MensagemPagina2.MostraMensagem_Erro("Sem Novo Aviso!");
                else
                    MensagemPagina2.MostraMensagem_Erro("Nenhuma mensagem localizada para sua pesquisa!");
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar(false);

        protected void cmdNovo_Click(object sender, EventArgs e) => FUNCOES.DirecionaPagina(sPagina_NovoRegistro);

        protected void rptMessages_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    if (e.Item.DataItem is DataRowView && (e.Item.DataItem as DataRowView).Row is DataRow item)
                    {
                        string corpo = item.Field<string>("sCorpo");
                        var doc = new HtmlDocument();

                        if (!string.IsNullOrEmpty(corpo.Trim()))
                        {
                            doc.LoadHtml(corpo);

                            var linkNodes = doc.DocumentNode.SelectNodes("//a");
                            if (linkNodes != null)
                            {
                                foreach (var link in linkNodes)
                                {
                                    var span = doc.CreateElement("span");
                                    span.SetAttributeValue("style", "text-decoration: underline;");
                                    span.InnerHtml = link.InnerHtml;

                                    link.ParentNode.ReplaceChild(span, link);
                                }
                            }

                            (e.Item.FindControl("litCorpo") as Literal).Text = doc.DocumentNode.OuterHtml;
                        }
                    }
                }
            }
            catch { }
        }
    }
}