using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using static System.Net.WebRequestMethods;

namespace TT_Flow.App.Controles
{
    public partial class DetalheModalProduto : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
            }
        }
        #region | Sugestão Funções
        #region | Função de Imagem
        protected void rptItemSugerido_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                System.Web.UI.WebControls.Image img = (System.Web.UI.WebControls.Image)e.Item.FindControl("imgProdutoPrincipal");

                TextBox txtIdProduto = e.Item.FindControl("txtIdProdutoSugestao") as TextBox;
                if (txtIdProduto != null)
                {
                    CarregaImgProduto(txtIdProduto.Text, img);
                }
            }
        }
        protected void CarregaImgProduto(string idProduto, System.Web.UI.WebControls.Image img)
        {
            DataTable dsPesquisa;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR_IMAGEM");
            vParametros.Add("@idTipoArquivo", "201");
            vParametros.Add("@idObjeto", idProduto);
            dsPesquisa = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametros);

            if (dsPesquisa.Rows.Count > 0)
            {
                DataRow imgBd = dsPesquisa.Rows[0];
                string imgUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])imgBd["vbArquivo"]);

                img.ImageUrl = imgUrl;
                img.Visible = true;
            }
            else 
            {
                img.ImageUrl = "/App/img/wms_dimensoes.svg";
                img.Visible = true;
            }
        }
        #endregion

        #region | Carregar Repeater
        public void PopularRptProduto(string idProduto)
        {
            DataSet ds;
            Dictionary<string, string> vParametros = new Dictionary<string, string>();
            string sProcedure = "sp_Manipula_tbl_Flow_Pedidos";
            vParametros.Add("@sFuncao", "CONSULTAR_PRODUTO");
            vParametros.Add("@idProduto", idProduto);

            ds = BD.ExecutarDataSet(sProcedure, vParametros);

            if (ds.Tables[0].Rows.Count > 0)
            {
                rptItensDetalhes.DataSource = ds;
                rptItensDetalhes.DataBind();

                //Abrir o modal
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalSugestao", "$('#modalProduto').modal('show');", true);
            }
        }
        #endregion

        protected void Cancelar_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_CloseModal", "$('#modalProduto').modal('hide'); $('body').removeClass('modal-open'); $('.modal-backdrop').remove(); $('[id$=Item_txtsCodigoProduto]').focus();", true);
        }

        #endregion
    }
}