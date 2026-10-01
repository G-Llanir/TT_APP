using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Globalization;

namespace TT_Flow.App.Controles
{
    public enum ModoSelecao
    {
        PaiUnico,
        PaiRepetido,
        Leitura,
        PaiComValor
    }

    public partial class TagDropdownControl : UserControl
    {
        public ModoSelecao Modo { get; set; } = ModoSelecao.PaiUnico;

        public decimal ValorLimite { get; set; } = 0;

        public List<TagPrincipal> TagsDisponiveis
        {
            get => ViewState["TagsDisponiveis"] as List<TagPrincipal> ?? new List<TagPrincipal>();
            set => ViewState["TagsDisponiveis"] = value;
        }

        public List<TagSelecionada> TagsSelecionadas
        {
            get
            {
                if (!string.IsNullOrEmpty(hfSelectedTags.Value))
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<List<TagSelecionada>>(hfSelectedTags.Value);
                    }
                    catch
                    {
                        return new List<TagSelecionada>(); // Retorna lista vazia em caso de JSON inválido
                    }
                }
                return new List<TagSelecionada>();
            }
            set
            {
                hfSelectedTags.Value = JsonConvert.SerializeObject(value ?? new List<TagSelecionada>());
            }
        }

        public string PlaceholderTexto { get; set; } = "Selecione...";
        public string PlaceholderValor { get; set; } = "";

        // SUBSTITUÍDO: O Page_Load não é mais usado para registrar o script
        protected void Page_Load(object sender, EventArgs e)
        {
            // A única responsabilidade aqui é garantir que o HiddenField não seja nulo no primeiro load
            if (!IsPostBack && string.IsNullOrEmpty(hfSelectedTags.Value))
            {
                hfSelectedTags.Value = "[]";
            }
        }

        // ADICIONADO: Usamos OnPreRender para garantir que o script seja registrado em TODOS os postbacks (parciais ou completos)
        protected override void OnPreRender(EventArgs e)
        {
            RegisterClientScript();
            base.OnPreRender(e);
        }

        private void RegisterClientScript()
        {
            // Evita registrar script se não houver dados, otimizando a página
            if (TagsDisponiveis == null || !TagsDisponiveis.Any())
            {
                return;
            }

            var camelCaseSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            string tagsDisponiveisJson = JsonConvert.SerializeObject(this.TagsDisponiveis, camelCaseSettings);
            string tagsSelecionadasJson = hfSelectedTags.Value; // Já está no formato JSON
            string modoJson = JsonConvert.SerializeObject(this.Modo.ToString(), camelCaseSettings);
            string placeholderTextoJson = JsonConvert.SerializeObject(this.PlaceholderTexto);
            string placeholderValorJson = JsonConvert.SerializeObject(this.PlaceholderValor);
            // Garante que o decimal seja formatado com ponto para o JavaScript entender
            string valorLimiteJson = this.ValorLimite.ToString(CultureInfo.InvariantCulture);

            // O script agora chama a função global TDC_init, que gerencia a criação das instâncias
            string script = $@"
                TDC_init(
                    '{this.ClientID}', 
                    {tagsDisponiveisJson}, 
                    {tagsSelecionadasJson}, 
                    {modoJson},
                    {placeholderTextoJson},
                    {placeholderValorJson},
                    {valorLimiteJson}
                );";

            // CORRIGIDO: Usa ScriptManager, que é compatível com UpdatePanel
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), $"Init_{this.ClientID}", script, true);
        }
    }

    // --- Classes de Modelo (mantidas como estavam) ---
    [Serializable]
    public class TagFilho
    {
        public string Id { get; set; }
        public string Nome { get; set; }
    }

    [Serializable]
    public class TagPrincipal
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public List<TagFilho> Filhos { get; set; }
    }

    [Serializable]
    public class TagSelecionada
    {
        public string IdPai { get; set; }
        public string NomePai { get; set; }
        public string IdFilho { get; set; }
        public string NomeFilho { get; set; }
        public decimal Valor { get; set; }
    }
}