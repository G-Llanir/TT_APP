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

namespace TT_Flow.App.Paginas.FAQ
{
    public partial class FAQ_Consulta : System.Web.UI.Page
    {
        readonly string sPagina_NovoFaq = "app/Paginas/FAQ/FAQ_Detalhe.aspx?id=0";

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.Scripts.MascaraDatas_ComDatePicker(Page, txtdtCriado.ClientID);
            FUNCOES.ValidaPermissao(Permissao.FAQ.Consultar, true);
            pnResultado.Visible = false;

            if (!IsPostBack)
            {

                FUNCOES.Popula_Combo(ddlCategoria, "sp_Select 'Flow_Faq_Categoria'", "idCategoria", "sDscCategoria", false, "Todas as Categorias", "0");
                FUNCOES.Popula_Combo(ddlDepartamento, "sp_Select 'Flow_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Todos os Departamentos", "0");
                FUNCOES.Popula_Combo(ddlStatus, "sp_Select 'Flow_Faq_Status'", "idStatus", "sDscStatus", false, "Todos os Status", "0");

                Pesquisar();
                
            }
            if (!FUNCOES.ValidaPermissao(Permissao.FAQ.Aprovar))
            {
                ddlStatus.Visible = false;
            }

        }
        #endregion

        #region Pesquisar
        protected void Pesquisar()
        {
            string sAprova = "";
            try
            {
                if (FUNCOES.ValidaPermissao(Permissao.FAQ.Aprovar))
                {
                    sAprova = "S";
                }

                DataSet ds = cls_FAQ.Consultar_FAQ(
                                        txtPesquisa.Text.Trim(), txtdtCriado.Text.Trim(), ddlCategoria.SelectedValue, ddlDepartamento.SelectedValue, ddlStatus.SelectedValue, sAprova);

                if (BD.ValidarDataSet(ds))
                {
                    pnResultado.Visible = true;

                    rptFAQ.DataSource = ds.Tables[0];
                    rptFAQ.DataBind();
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

        #region Novo Faq Click
        protected void cmdNovoFaq_Click(object sender, EventArgs e)
        {

            FUNCOES.DirecionaPagina(sPagina_NovoFaq);
        }
        #endregion

        #region Repeater Item Data Bound
        protected void rptFaq_idb(object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                string idFaq = row["idFaq"].ToString();

                Label spannComentarios = (Label)e.Item.FindControl("nComentarios");
                Repeater rptFAQ_Tags = (Repeater)e.Item.FindControl("rptFAQ_Tags");

                DataSet dsComentarios = cls_FAQ.Consultar_FAQ_nComentarios(idFaq);

                if (BD.ValidarDataSet(dsComentarios))
                {
                    spannComentarios.Text = dsComentarios.Tables[0].Rows.Count + " Comentários";
                }
                else
                {
                    spannComentarios.Text = "0 Comentários";
                }


                DataSet dsTag = cls_FAQ.Consultar_FAQ_Tags(idFaq);

                rptFAQ_Tags.DataSource = dsTag.Tables[0];
                rptFAQ_Tags.DataBind();
            }
        }
        #endregion

    }
}