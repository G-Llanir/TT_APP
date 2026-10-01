using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;

namespace TT_Flow.App.Controles
{
    public partial class TextBox_Padrao : UserControl
    {
        #region | Propriedades

        #region | Atributos públicos

        public string Text
        {
            get => txtPadrao.Text;
            set
            {
                txtPadrao.Text = value;
                SincronizarValor_ModoSenha();
            }
        }
        public string Titulo { get => lblPadrao.Text; set => lblPadrao.Text = value; }
        public string Classe { get => txtPadrao.GetClass(); set => txtPadrao.AddClass(value); }
        public string Placeholder { get => txtPadrao.GetAttribute("placeholder"); set => txtPadrao.SetAttribute("placeholder", value); }
        public string Mascara { get => txtPadrao.GetAttribute("x-mask"); set => txtPadrao.SetAttribute("x-mask", value); }
        public string MascaraDinamica { get => txtPadrao.GetAttribute("x-mask:dynamic"); set => txtPadrao.SetAttribute("x-mask:dynamic", value); }

        public bool AutoPostBack { get => txtPadrao.AutoPostBack; set => txtPadrao.AutoPostBack = value; }
        public bool ReadOnly { get => txtPadrao.ReadOnly; set => txtPadrao.ReadOnly = value; }

        public int MaxLength { get => txtPadrao.MaxLength; set => txtPadrao.MaxLength = value; }
        public int Rows { get => txtPadrao.Rows; set => txtPadrao.Rows = value; }


        public event EventHandler TextChanged { add => txtPadrao.TextChanged += value; remove => txtPadrao.TextChanged -= value; }

        public AttributeCollection Atributos => txtPadrao.Attributes;
        public ClientIDMode IDMode => txtPadrao.ClientIDMode;

        #endregion

        #region | Configurações

        public bool Obrigatorio { get; set; } = false;
        public Modos Modo { get; set; } = Modos.Padrão;
        public bool Validacao { get; set; } = false;
        public string FuncaoValidacao { get; set; } = "";
        public string MsgValidacao { get; set; } = "";
        public int TamanhoTotal { get; set; } = 11;
        public int CasasDecimais { get; set; } = 2;
        public bool SeparadorMilhar { get; set; } = true;
        public bool Grupo { get; set; } = false;
        public bool Prefixo { get; set; } = false;
        public string Grupo_Simbolo { get; set; } = "";

        #endregion

        #region | Classes

        public enum Modos
        {
            Padrão = 0,
            Multilinha = 1,
            Senha = 2,
            Email = 3,
            URL = 4,
            Inteiro = 5,
            Decimal = 6,
            Telefone = 7,
            Data = 8,
            Hora = 9,
            DataHora = 10,
            Mês = 11,
            Semana = 12,
            Deslizante = 13,
            Pesquisa = 14,
            CPF_CNPJ = 15
        }

        #endregion

        private bool _configurado = false;

        #endregion

        protected override void OnPreRender(EventArgs e)
        {
            if (!_configurado)
                Configurar();

            base.OnPreRender(e);
        }
                
        /// <summary>
        /// Método para configurar o controle.
        /// </summary>
        /// <param name="bObrigatorio">Exibe o ícone de 'Campo Obrigatório'.</param>
        /// <param name="idModo">Define o modo da TextBox.</param>
        /// <param name="bValidacao">Habilita ou desabilita a validação.</param>
        /// <param name="sFuncaoValidacao">Nome da função em JS para validação personalizada.</param>
        /// <param name="sMsgValidacao">Mensagem de validação personalizada.</param>
        /// <param name="Mascara_Decimal">Define as propriedades da Máscara do modo Decimal.</param>
        /// <param name="bGrupo">Define se o grupo está habilitado.</param>
        /// <param name="bPrefixo">Define se o símbolo será exibido à esquerda da TextBox.</param>
        /// <param name="sGrupo_Simbolo">Define as propriedades do Grupo/Símbolo.</param>
        public void Configurar(bool bObrigatorio, Modos idModo, bool bValidacao = true, string sFuncaoValidacao = "", string sMsgValidacao = "", (int TamanhoTotal, int CasasDecimais, bool bSeparadorMilhar) Mascara_Decimal = default,
                                bool bGrupo = false, bool bPrefixo = false, string sGrupo_Simbolo = "")
        {
            Obrigatorio = bObrigatorio;
            Modo = idModo;
            Validacao = bValidacao;
            FuncaoValidacao = sFuncaoValidacao;
            MsgValidacao = sMsgValidacao;
            TamanhoTotal = Mascara_Decimal.TamanhoTotal;
            CasasDecimais = Mascara_Decimal.CasasDecimais;
            SeparadorMilhar = Mascara_Decimal.bSeparadorMilhar;
            Grupo = bGrupo;
            Prefixo = bPrefixo;
            Grupo_Simbolo = sGrupo_Simbolo;

            Configurar();
        }

        private void Configurar()
        {
            lblPadrao.Visible = !string.IsNullOrWhiteSpace(Titulo);
            iconPadrao.Visible = Obrigatorio;

            txtPadrao.RemoveClass(string.IsNullOrWhiteSpace(FuncaoValidacao) ? "validarPadrao_TextBox" : FuncaoValidacao);
            txtPadrao.RemoveAttribute("data-validarPadrao_Erro");

            div_InputGroupPadrao.Attributes["class"] = string.IsNullOrWhiteSpace(Grupo_Simbolo) ? "" : Grupo ? "input-group" : "input-symbol";
            ltr_InputGroupPadrao_1.Text = "";
            ltr_InputGroupPadrao_2.Text = "";
            (Prefixo ? ltr_InputGroupPadrao_1 : ltr_InputGroupPadrao_2).Text = Grupo_Simbolo.Contains("input-group-addom") || Grupo_Simbolo.Contains("symbol") ? Grupo_Simbolo : $"<span class='{(Grupo ? "input-group-addon" : "symbol")}'>{Grupo_Simbolo}</span>";

            string sMsgPadrao = Obrigatorio ? "Este campo é obrigatório." : string.Empty;
            ConfigurarModo(ref sMsgPadrao);

            if (Validacao)
            {
                txtPadrao.AddClass(string.IsNullOrWhiteSpace(FuncaoValidacao) ? "validarPadrao_TextBox" : FuncaoValidacao);

                if (!string.IsNullOrWhiteSpace(MsgValidacao) || !string.IsNullOrWhiteSpace(sMsgPadrao))
                    txtPadrao.SetAttribute("data-validarPadrao_Erro", string.IsNullOrWhiteSpace(MsgValidacao) ? sMsgPadrao : MsgValidacao);
            }

            _configurado = true;
        }

        private void ConfigurarModo(ref string sMsgPadrao)
        {
            switch (Modo)
            {
                case Modos.Multilinha:
                    txtPadrao.TextMode = TextBoxMode.MultiLine; break;
                case Modos.Senha:
                    txtPadrao.TextMode = TextBoxMode.Password;
                    SincronizarValor_ModoSenha();
                    div_InputGroupPadrao.Attributes["class"] = "input-symbol";
                    ltr_InputGroupPadrao_2.Text = "<a class='symbol cursor-pointer' onclick=\"$(this).siblings('input').attr('type', $(this).siblings('input').attr('type') == 'password' ? 'text' : 'password'); $(this).find('i').toggleClass('fa-eye fa-eye-slash');\"><i class='fa fa-eye'></i></a>";
                    break;
                case Modos.Email:
                    txtPadrao.TextMode = TextBoxMode.Email;
                    sMsgPadrao = "É obrigatório definir um Email válido. (exemplo@email.com)"; break;
                case Modos.URL:
                    txtPadrao.TextMode = TextBoxMode.Url;
                    sMsgPadrao = "É obrigatório definir uma URL válida. (http://exemplo.com ou https://exemplo.com)"; break;
                case Modos.Inteiro:
                    txtPadrao.Attributes["data-inteiroPadrao"] = txtPadrao.MaxLength > 0 ? "0".PadLeft(txtPadrao.MaxLength, '0') : "00000"; break;
                case Modos.Decimal:
                    {
                        string parteInteira = new string('9', TamanhoTotal - CasasDecimais);
                        string parteDecimal = new string('9', CasasDecimais);

                        string sMascara_Decimal = $"{parteInteira},{parteDecimal}";
                        if (SeparadorMilhar)
                            sMascara_Decimal = string.Join(".", Enumerable.Repeat("999", parteInteira.Length / 3)) + $",{parteDecimal}";

                        txtPadrao.Attributes["data-decimalPadrao"] = sMascara_Decimal;
                        break;
                    }
                case Modos.Telefone:
                    txtPadrao.TextMode = TextBoxMode.Phone;
                    sMsgPadrao = "É obrigatório definir um Telefone válido. ((00) 0000-0000 ou (00) 00000-0000)"; break;
                case Modos.Data:
                    txtPadrao.TextMode = TextBoxMode.Date; break;
                case Modos.Hora:
                    txtPadrao.TextMode = TextBoxMode.Time; break;
                case Modos.DataHora:
                    txtPadrao.TextMode = TextBoxMode.DateTimeLocal; break;
                case Modos.Mês:
                    txtPadrao.TextMode = TextBoxMode.Month; break;
                case Modos.Semana:
                    txtPadrao.TextMode = TextBoxMode.Week; break;
                case Modos.Deslizante:
                    txtPadrao.TextMode = TextBoxMode.Range; break;
                case Modos.Pesquisa:
                    txtPadrao.TextMode = TextBoxMode.Search; break;
                case Modos.CPF_CNPJ:
                    txtPadrao.Attributes["x-mask:dynamic"] = "$input.replace(/\\D/g, '').length <= 11 ? '000.000.000-00' : '00.000.000/0000-00'";
                    sMsgPadrao = "É obrigatório definir um CPF ou CNPJ válido. (XXX.XXX.XXX-XX ou XX.XXX.XXX/XXXX-XX)"; break;
            }
        }

        private void SincronizarValor_ModoSenha()
        {
            if (Modo != Modos.Senha)
                return;

            if (string.IsNullOrEmpty(txtPadrao.Text))
                txtPadrao.RemoveAttribute("value");
            else
                txtPadrao.SetAttribute("value", txtPadrao.Text);
        }
    }
}
