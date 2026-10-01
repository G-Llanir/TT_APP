using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;

using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using IDENTITY = TT.FrameWork.Identity;
using System.Web.UI.HtmlControls;
using System.Text;

namespace TT_Flow.App.Controles
{
    public partial class FiltroPesquisa : System.Web.UI.UserControl
    {
        static public string[] _FiltrosAtivos = { "tipoProdutos", "Familia", "Grupo", "PaisOrigem" };
        public string ddlTipoProdutoValue { get => ddlTipoProduto.SelectedValue; set => ddlTipoProduto.SelectedValue = value; }
        public string ddlFamiliaValue { get => ddlFamilia.SelectedValue; set=> ddlFamilia.SelectedValue = value; }
        public string ddlGrupoValue { get => ddlGrupo.SelectedValue; set => ddlGrupo.SelectedValue = value; }
        public string ddlPaisOrigemValue { get => ddlPaisOrigem.SelectedValue; set=> ddlTipoProduto.SelectedValue = value; }

        public bool ExibirNaAbertura = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    if (_FiltrosAtivos.Count() > 0)
                    {
                        PopularCombos(_FiltrosAtivos);
                        ControleExibicao();

                    }
                }
                catch
                {
                    return;
                }
            }


        }
        void PopularCombos(string[] FiltrosAtivos)
        {
            foreach (string str in FiltrosAtivos)
            {
                if (str == "tipoProdutos")
                {
                    FUNCOES.Popula_Combo(ddlTipoProduto, "sp_Select 'Flow_Produtos_Tipo'", "idTipoProduto", "sDscTipoProduto", false, "Todos os Tipos", "0");
                }
                else if (str == "Familia")
                {
                    FUNCOES.Popula_Combo(ddlFamilia, "sp_Select 'Flow_WMS_Produtos_Familia'", "idFamilia", "sDscFamilia", false, "Todas as Famílias", "0");
                }
                else if (str == "Grupo")
                {
                    FUNCOES.Popula_Combo(ddlGrupo, "sp_Select 'Flow_WMS_Produtos_Grupos'", "idGrupo", "sDscGrupo", false, "Todos os Grupos", "0");
                }
                else if (str == "PaisOrigem")
                {
                    FUNCOES.Popula_Combo(ddlPaisOrigem, "sp_Select 'tbl_Flow_WMS_Produtos_Origem'", "idPais", "sDscPais", false, "Todos Países de Origem", "0");
                }
            }
        }

        public void ControleExibicao()
        {

            if (ExibirNaAbertura)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("$(document).ready(function() {");
                sb.AppendLine("    $('[id*=idFiltroCollapse]').collapse({");
                sb.AppendLine("        show: true");
                sb.AppendLine("    });");
                sb.AppendLine("});");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptControleExibicao", sb.ToString(), true);

            }
        }
    }
}