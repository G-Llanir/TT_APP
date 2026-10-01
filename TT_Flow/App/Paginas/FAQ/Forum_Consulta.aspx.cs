using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using TT.FrameWork;
using TT_Flow.FrameWork.Classes;
using TT_Hub.App.Paginas.RRHH;
using FUNCOES = TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas.Forum
{
    public partial class Forum_Consulta : System.Web.UI.Page
    {
        readonly string sPagina_NovoForum = "app/Paginas/FAQ/Forum_Detalhe.aspx?id=0";

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {

            FUNCOES.ValidaPermissao(Permissao.Forum.Consultar, true);
            pnResultado.Visible = false;

            if (!IsPostBack)
            {

                FUNCOES.Popula_Combo(ddlCategoria, "sp_Select 'Flow_Forum_Categoria'", "idCategoria", "sDscCategoria", false, "Todas as Categorias", "0");
                FUNCOES.Popula_Combo(ddlDepartamento, "sp_Select 'Flow_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Todos os Departamentos", "0");
                FUNCOES.Popula_Combo(ddlStatus, "sp_Select 'Flow_Forum_Status'", "idStatus", "sDscStatus", false, "Todos os Status", "0");

                Pesquisar();
                
            }

        }
        #endregion

        #region Pesquisar
        protected void Pesquisar()
        {
            try
            {
                DataSet ds = cls_Forum.Consultar_Forum(
                                        txtPesquisa.Text.Trim(), txtdtCriado.Text.Trim(), ddlCategoria.SelectedValue, ddlDepartamento.SelectedValue, ddlStatus.SelectedValue);

                if (BD.ValidarDataSet(ds))
                {
                    pnResultado.Visible = true;

                    rptForum.DataSource = ds.Tables[0];
                    rptForum.DataBind();
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro("Nenhum registro Localizado");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        #endregion

        #region Pesquisar_Click
        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }
        #endregion

        #region Novo Forum Click
        protected void cmdNovoForum_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina(sPagina_NovoForum);
        }
        #endregion

        #region Repeater Item Data Bound
        protected void rptForum_idb(object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                string idForum = row["idForum"].ToString();

                Label lblSolucionado = (Label)e.Item.FindControl("txtSolucionado");
                Label spannComentarios = (Label)e.Item.FindControl("nRespostas");
                Repeater rptForum_Tags = (Repeater)e.Item.FindControl("rptForum_Tags");

                DataSet dsComentarios = cls_Forum.Consultar_Forum_nRespostas(idForum);

                if (BD.ValidarDataSet(dsComentarios))
                {
                    spannComentarios.Text = dsComentarios.Tables[0].Rows.Count + " Respostas";
                }
                else
                {
                    spannComentarios.Text = "0 Respostas";
                }

                if (row["sSolucionado"].ToString() == "N")
                {
                    lblSolucionado.Visible = false;
                }


                DataSet dsTag = cls_Forum.Consultar_Forum_Tags(idForum);

                rptForum_Tags.DataSource = dsTag.Tables[0];
                rptForum_Tags.DataBind();
            }
        }
        #endregion

    }
}