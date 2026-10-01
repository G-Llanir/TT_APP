using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.FrameWork;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using IDENTITY = TT.FrameWork.Identity;
using TT_Flow.App.Paginas.WMS.Manutencao;
using static TT_Flow.App.Controles.MultiSelecao;
using System.Reflection;
using System.EnterpriseServices;

namespace TT_Flow.App.Controles
{
    public partial class MultiSelecao : System.Web.UI.UserControl
    {

        private List<PopularCombo> popularCombos1 = new List<PopularCombo>();

        public List<PopularCombo> PopularCombos1 { get => popularCombos1; set => popularCombos1 = value; }
        public string slstidFluxoValue { get => lstidFluxo.SelectedValue; set => lstidFluxo.SelectedValue = value; }
        public ListBox _lstidFluxo { get => lstidFluxo; }
        public int idCliente { get; set; }
        public List<string> sSelecionados { get; set; }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (PopularCombos1.Count > 0)
                {
                    PopularCombos(PopularCombos1);

                }

            }
        }
        public void PopularCombos(List<PopularCombo> popularCombos)
        {
            foreach (PopularCombo combo in popularCombos)
            {
                lstidFluxo.ClearSelection();
                FUNCOES.Popula_Combo(lstidFluxo, "sp_Select " + "'" + combo.SProcedure + "'" + combo.SIdPesquisa, combo.SCampoID, combo.SOrdenacao, false, combo.SPlaceHolder, "0");
            }
            ScriptMultiSelecao();

            //ScriptManager.RegisterStartupScript(this, this.GetType(), lstidFluxo.UniqueID + "Add", "tinyMCE.execCommand('mceAddEditor', true,'" + lstidFluxo.ClientID + "');", true);
            //ScriptManager.RegisterOnSubmitStatement(this, this.GetType(), lstidFluxo.UniqueID + "Remove", "tinyMCE.execCommand('mceRemoveEditor', true,'" + lstidFluxo.ClientID + "');");
        }
        //20.12.2023 - Alterado dropRight para dropLeft
        public void ScriptMultiSelecao()
        {
            string script = @"
                    $(function carregarMultiselect() {
                    $('[id*=lstidFluxo]').multiselect({
                    buttonWidth: '195px',
                    includeSelectAllOption: true,
                    maxHeight: 300,
                    dropRight: true,
                    nSelectedText: ' - Fluxos Selecionados!',
                    allSelectedText: 'Todos os Fluxos',
                    enableFiltering: false
                    });
                    });
                    ";
            ScriptManager.RegisterStartupScript(this, GetType(), "carregarMultiselect", script, true);
        }
        public class PopularCombo
        {
            string sProcedure = "";
            string sCampoID = "";
            string sOrdenacao = "";
            string sPlaceHolder = "";
            string idPesquisa = ",@idPesquisa=0";

            public PopularCombo(string sProcedure, string sCampoID, string sOrdenacao, string sPlaceHolder)
            {
                this.sProcedure = sProcedure;
                this.sCampoID = sCampoID;
                this.sOrdenacao = sOrdenacao;
                this.sPlaceHolder = sPlaceHolder;
            }
            public PopularCombo(string sProcedure, string sCampoID, string sOrdenacao, string sPlaceHolder, string idPesquisa)
            {
                this.sProcedure = sProcedure;
                this.sCampoID = sCampoID;
                this.sOrdenacao = sOrdenacao;
                this.sPlaceHolder = sPlaceHolder;
                this.idPesquisa = idPesquisa;
            }

            public string SProcedure { get => sProcedure; set => sProcedure = value; }
            public string SCampoID { get => sCampoID; set => sCampoID = value; }
            public string SOrdenacao { get => sOrdenacao; set => sOrdenacao = value; }
            public string SPlaceHolder { get => sPlaceHolder; set => sPlaceHolder = value; }
            public string SIdPesquisa { get => idPesquisa; set => idPesquisa = value; }

        }


        public void RemoverListaItens(List<string> lst)
        {
            foreach (string i in lst)
            {
                lstidFluxo.Items.RemoveAt(int.Parse(i));
            }

        }
        public void LimparCombo()
        {
            lstidFluxo.Items.Clear();
            lstidFluxo.Items.Add(new System.Web.UI.WebControls.ListItem("Selecione a Moeda", "0"));
        }

        public void PopularCombo_MoedaOrigem(string sDescricao, int value)
        {
            lstidFluxo.Items.Add(new System.Web.UI.WebControls.ListItem(sDescricao, value.ToString()));
        }

        protected void lstidFluxo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //se cliente for diferente de zero, então repopula a tabela de permissão de acordo com o idCliente e os itens que estão selecionados no listbox
            if (idCliente != 0)
            {

            }
        }
        public void AjustarSelecao(List<string> selecao)
        {
            foreach (ListItem item in lstidFluxo.Items)
            {
                if (selecao.Contains(item.Value))
                {
                    item.Selected = true;
                }
            }
        }
    }
}