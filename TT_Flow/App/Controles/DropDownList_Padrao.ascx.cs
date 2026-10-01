using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using AttributeCollection = System.Web.UI.AttributeCollection;

namespace TT_Flow.App.Controles
{
    [ParseChildren(true, "Items")]
    [PersistChildren(false)]
    public partial class DropDownList_Padrao : UserControl
    {
        #region | Propriedades

        #region | Atributos públicos

        public string SelectedValue { get => ddlPadrao.SelectedValue; set => ddlPadrao.SelectedValue = value; }
        public string Titulo { get => lblPadrao.Text; set => lblPadrao.Text = value; }
        public string Classe { get => ddlPadrao.GetClass(); set => ddlPadrao.AddClass(value); }

        public bool AutoPostBack { get => ddlPadrao.AutoPostBack; set => ddlPadrao.AutoPostBack = value; }
        public bool ReadOnly
        {
            get => ddlPadrao.HasAttribute("readonly");
            set
            {
                if (value)
                    ddlPadrao.SetAttribute("readonly", "true");
                else
                    ddlPadrao.RemoveAttribute("readonly");
            }
        }

        public int SelectedIndex { get => ddlPadrao.SelectedIndex; set => ddlPadrao.SelectedIndex = value; }

        public event EventHandler SelectedIndexChanged { add => ddlPadrao.SelectedIndexChanged += value; remove => ddlPadrao.SelectedIndexChanged -= value; }

        public ListItem SelectedItem => ddlPadrao.SelectedItem;

        public int[] SelectedIndexes => ddlPadrao.GetSelectedIndices();
        public string[] SelectedValues => ddlPadrao.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray();
        public ListItem[] SelectedItems => ddlPadrao.Items.Cast<ListItem>().Where(i => i.Selected).ToArray();

        public AttributeCollection Atributos => ddlPadrao.Attributes;

        [PersistenceMode(PersistenceMode.InnerDefaultProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ListItemCollection Items => ddlPadrao.Items;

        #endregion

        #region | Configurações

        public bool Obrigatorio { get; set; } = false;
        public string Link { get; set; } = "";
        public string SufixoLink { get; set; } = "";
        public bool Multiplo { get; set; } = false;
        public bool Validacao { get; set; } = false;
        public string FuncaoValidacao { get; set; } = "";
        public string MsgValidacao { get; set; } = "";

        #endregion

        private bool _configurado = false;

        #endregion

        protected override void OnPreRender(EventArgs e)
        {
            if (!_configurado)
                Configurar();

            base.OnPreRender(e);
        }

        public string GetSelectedValues(string separator = ",") => string.Join(separator, ddlPadrao.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value));

        public bool IsSelected(string value)
        {
            if (value == null)
                return false;

            if (!Multiplo)
                return ddlPadrao.SelectedValue == value;

            foreach (ListItem item in ddlPadrao.Items)
            {
                if (item.Selected && item.Value == value)
                    return true;
            }

            return false;
        }
        public bool IsSelected(int index)
        {
            if (!Multiplo)
                return ddlPadrao.SelectedIndex == index;

            if (index < 0 || index >= ddlPadrao.Items.Count)
                return false;

            return ddlPadrao.Items[index].Selected;
        }

        public void Limpar() => ddlPadrao.Items.Clear();
        public void Adicionar(string valor, string texto) => ddlPadrao.Items.Add(new ListItem(texto, valor));
        public void Adicionar(ListItem item) => ddlPadrao.Items.Add(item);
        public void Adicionar(ListItem[] items) => ddlPadrao.Items.AddRange(items);

        public void AdicionarItens(string idPadrao = "", params (string Valor, string Texto)[] Valores)
        {
            string selecionado = ddlPadrao.SelectedValue;

            ddlPadrao.Items.Clear();
            foreach (var (Valor, Texto) in Valores)
                ddlPadrao.Items.Add(new ListItem(Texto, Valor));

            if (!string.IsNullOrEmpty(idPadrao) && (string.IsNullOrEmpty(selecionado) || selecionado == idPadrao))
                ddlPadrao.SelectedValue = idPadrao;
            else if (!string.IsNullOrEmpty(selecionado))
                ddlPadrao.SelectedValue = selecionado;
            else
                ddlPadrao.SelectedIndex = 0;
        }

        public void Popula_Combo(string sSql, string sCodigo, string sDescricao, bool bConcatenar, string sTexto_Padrao, string sValor_Padrao)
        {
            Funcoes.Popula_Combo(ddlPadrao, sSql, sCodigo, sDescricao, bConcatenar, sTexto_Padrao, sValor_Padrao);
            ddlPadrao.SelectedValue = sValor_Padrao;
        }

        /// <summary>
        /// Método para configurar o controle.
        /// </summary>
        /// <param name="bObrigatorio">Exibe o ícone de 'Campo Obrigatório'.</param>
        /// <param name="bMultiplo">Define se o DropDownList permite múltiplas seleções.</param>
        /// <param name="bValidacao">Habilita ou desabilita a validação.</param>
        /// <param name="sFuncaoValidacao">Nome da função em JS para validação personalizada.</param>
        /// <param name="sMsgValidacao">Mensagem de validação personalizada.</param>
        public void Configurar(bool bObrigatorio, string sLink = "", string sSufixoLink = "", bool bMultiplo = false, bool bValidacao = true, string sFuncaoValidacao = "", string sMsgValidacao = "")
        {
            Obrigatorio = bObrigatorio;
            Link = sLink;
            SufixoLink = sSufixoLink;
            Multiplo = bMultiplo;
            Validacao = bValidacao;
            FuncaoValidacao = sFuncaoValidacao;
            MsgValidacao = sMsgValidacao;

            Configurar();
        }

        private void Configurar()
        {
            lblPadrao.Visible = !string.IsNullOrWhiteSpace(Titulo);
            iconPadrao.Visible = Obrigatorio;

            ddlPadrao.RemoveClass(string.IsNullOrEmpty(FuncaoValidacao) ? "validarPadrao_DropDownList" : FuncaoValidacao);
            ddlPadrao.RemoveAttribute("data-validarPadrao_Erro");
            ddlPadrao.SelectionMode = Multiplo ? ListSelectionMode.Multiple : ListSelectionMode.Single;

            bool bLink = !string.IsNullOrEmpty(Link);
            div_InputGroupPadrao.Attributes["class"] = bLink ? "input-group" : string.Empty;
            lnkPadrao.Visible = bLink;

            if (bLink)
            {
                ddlPadrao.RemoveClass("link_DropDownList_Padrao");
                ddlPadrao.AddClass("link_DropDownList_Padrao");

                lnkPadrao.Text = "<i class='fa fa-external-link'></i>";
                lnkPadrao.NavigateUrl = Link + ddlPadrao.SelectedValue + SufixoLink;
                lnkPadrao.Attributes["data-link"] = Link;
                lnkPadrao.Attributes["data-sufixo"] = SufixoLink;

                lnkPadrao.Attributes["title"] = $"Exibir {lblPadrao.Text}";
            }

            if (Validacao)
            {
                ddlPadrao.AddClass(string.IsNullOrEmpty(FuncaoValidacao) ? "validarPadrao_DropDownList" : FuncaoValidacao);

                string sMsgPadrao = Obrigatorio ? "Este campo é obrigatório." : string.Empty;
                if (!string.IsNullOrEmpty(MsgValidacao) || !string.IsNullOrEmpty(sMsgPadrao))
                    ddlPadrao.SetAttribute("data-validarPadrao_Erro", string.IsNullOrEmpty(MsgValidacao) ? sMsgPadrao : MsgValidacao);
            }

            _configurado = true;
        }
    }
}