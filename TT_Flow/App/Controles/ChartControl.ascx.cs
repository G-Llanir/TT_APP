using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace TT_Flow.App.Controles
{
    public partial class ChartControl : System.Web.UI.UserControl
    {
        /// <summary>
        /// (Tipo) Define o tipo do gráfico a ser renderizado (ex: Barra, Pizza, Linha).
        /// </summary>
        public ChartType Type { get; set; }

        /// <summary>
        /// (Tipo de Escala) Define o tipo de escala para o eixo de valores (padrão: Linear).
        /// </summary>
        public ChartScaleType ScaleType { get; set; } = ChartScaleType.Linear;

        /// <summary>
        /// (Dados Genéricos) Estrutura de dados complexa para o gráfico, permitindo múltiplos datasets e personalização avançada.
        /// </summary>
        public GenericChartData GenericData { get; set; }

        /// <summary>
        /// (Orientação) Define a orientação do gráfico, se aplicável (padrão: Vertical).
        /// </summary>
        public ChartOrientation Orientation { get; set; } = ChartOrientation.Vertical;

        /// <summary>
        /// (Fonte de Dados) Fonte de dados simples (ex: uma lista de objetos) para popular o gráfico. Use em conjunto com DataLabelField e DataValueField.
        /// </summary>
        public IEnumerable DataSource { get; set; }

        /// <summary>
        /// (Campo de Rótulo) Nome da propriedade do objeto no DataSource que será usada como rótulo (label) no gráfico.
        /// </summary>
        public string DataLabelField { get; set; }

        /// <summary>
        /// (Campo de Valor) Nome da propriedade do objeto no DataSource que será usada como valor (value) no gráfico.
        /// </summary>
        public string DataValueField { get; set; }

        /// <summary>
        /// (JSON de Rótulos) (Protegido) Armazena a string JSON dos rótulos (labels) para ser usada no script do gráfico.
        /// </summary>
        protected string LabelsJson { get; private set; }

        /// <summary>
        /// (JSON de Conjuntos de Dados) (Protegido) Armazena a string JSON dos conjuntos de dados (datasets) para ser usada no script do gráfico.
        /// </summary>
        protected string DatasetsJson { get; private set; }

        /// <summary>
        /// (Modo das Barras) Para gráficos de barras, define se as barras devem ser agrupadas ou empilhadas.
        /// </summary>
        public ChartBarMode BarMode { get; set; }

        /// <summary>
        /// (Valor Máximo Y) Define um valor máximo para o eixo Y. Se nulo, o valor máximo será calculado automaticamente.
        /// </summary>
        public decimal? MaxYValue { get; set; }

        /// <summary>
        /// (Largura) Define a largura do contêiner do gráfico (ex: "100%", "800px"). Padrão é "100%".
        /// </summary>
        public string Width { get; set; } = "100%";

        /// <summary>
        /// (Altura) Define a altura do contêiner do gráfico (ex: "600px"). É ignorado se AspectRatio for definido. Padrão é "600px".
        /// </summary>
        public string Height { get; set; } = "600px";

        /// <summary>
        /// (Largura Máxima) Define a largura máxima do contêiner do gráfico. Padrão é "none".
        /// </summary>
        public string MaxWidth { get; set; } = "none";

        /// <summary>
        /// (Proporção de Tela) Define a proporção do gráfico (largura/altura). Se definido, a propriedade Height é ignorada para manter a proporção.
        /// </summary>
        public decimal? AspectRatio { get; set; }

        /// <summary>
        /// (Eixo Y com Porcentagem) Se verdadeiro, formata os rótulos do eixo Y para exibir um sinal de porcentagem (%).
        /// </summary>
        public bool YAxisShowPercentage { get; set; } = false;

        // Adicione esta propriedade junto com as outras (Type, ScaleType, etc.)
        /// <summary>
        /// (Cultura da Porcentagem) Define a cultura para formatação dos números no tooltip (padrão: Brasileiro).
        /// </summary>
        public TooltipNumberCulture PercentageCulture { get; set; } = TooltipNumberCulture.Brazilian;

        /// <summary>
        /// (Estilo do Contêiner) (Protegido) Gera a string de estilo CSS inline para o contêiner do gráfico com base nas propriedades Width, Height e MaxWidth.
        /// </summary>
        protected string ContainerStyle
        {
            get
            {
                var styles = new System.Text.StringBuilder();
                if (!string.IsNullOrEmpty(Width))
                {
                    styles.Append($"width: {Width}; ");
                }
                if (!string.IsNullOrEmpty(Height) && !AspectRatio.HasValue) // Só aplica altura se AspectRatio não for usado
                {
                    styles.Append($"height: {Height}; ");
                }
                if (!string.IsNullOrEmpty(MaxWidth) && MaxWidth != "none")
                {
                    styles.Append($"max-width: {MaxWidth}; ");
                }
                return styles.ToString();
            }
        }


        /// <summary>
        /// (Vincular Dados ao Gráfico) Processa os dados (de GenericData ou DataSource), serializa-os para JSON e registra o script para renderizar o gráfico na página.
        /// </summary>
        public void DataBindChart()
        {
            var serializer = new JavaScriptSerializer();
            GenericChartData dataToRender = null;

            if (GenericData != null && GenericData.datasets.Any())
            {
                dataToRender = GenericData;
                foreach (var dataset in dataToRender.datasets)
                {
                    if (!dataset.backgroundColor.Any())
                    {
                        GenerateDatasetColors(dataset, dataToRender.labels.Count);
                    }
                }
            }

            else if (DataSource != null && !string.IsNullOrEmpty(DataLabelField) && !string.IsNullOrEmpty(DataValueField))
            {
                var processedData = new List<ChartData>();
                foreach (object item in DataSource)
                {
                    try
                    {
                        var labelValue = item.GetType().GetProperty(DataLabelField)?.GetValue(item, null);
                        var valueValue = item.GetType().GetProperty(DataValueField)?.GetValue(item, null);

                        string label = labelValue?.ToString() ?? string.Empty;

                        decimal value;

                        if (!decimal.TryParse(valueValue?.ToString(), NumberStyles.Any, new CultureInfo("pt-BR"), out value))
                        {
                            decimal.TryParse(valueValue?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out value);
                        }

                        processedData.Add(new ChartData(label, value));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Erro ao processar DataSource. Verifique se as propriedades '{DataLabelField}' e '{DataValueField}' existem. Erro: {ex.Message}");
                    }
                }

                if (processedData.Any())
                {
                    dataToRender = new GenericChartData();
                    dataToRender.labels = processedData.Select(d => d.Label).ToList();
                    var simpleDataset = new ChartDataset
                    {
                        label = "Valores",
                        data = processedData.Select(d => (object)d.Value).ToList(),
                        // --> LINHA ADICIONADA: Popula os valores originais para uso no tooltip.
                        originalValues = processedData.Select(d => d.Value).ToList()
                    };
                    GenerateDatasetColors(simpleDataset, processedData.Count);
                    dataToRender.datasets.Add(simpleDataset);
                }
            }

            if (dataToRender == null || !dataToRender.datasets.Any()) return;

            LabelsJson = serializer.Serialize(dataToRender.labels);
            DatasetsJson = serializer.Serialize(dataToRender.datasets);
            RegisterChartScript();
        }

        /// <summary>
        /// (Gerar Cores do Dataset) (Privado) Gera cores aleatórias para um conjunto de dados (dataset) caso nenhuma cor tenha sido especificada.
        /// </summary>
        /// <param name="dataset">O conjunto de dados a ser colorido.</param>
        /// <param name="count">O número de cores a serem geradas.</param>
        private void GenerateDatasetColors(ChartDataset dataset, int count)
        {
            bool useSingleColorSet = (Type == ChartType.Bar || Type == ChartType.Line);
            var random = new Random(Guid.NewGuid().GetHashCode());

            if (useSingleColorSet && dataset.data.Count > 0)
            {
                int r = random.Next(256);
                int g = random.Next(256);
                int b = random.Next(256);
                dataset.backgroundColor = new List<string> { $"rgba({r}, {g}, {b}, 0.5)" };
                dataset.borderColor = new List<string> { $"rgba({r}, {g}, {b}, 1)" };
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    int r = random.Next(256);
                    int g = random.Next(256);
                    int b = random.Next(256);
                    dataset.backgroundColor.Add($"rgba({r}, {g}, {b}, 0.5)");
                    dataset.borderColor.Add($"rgba({r}, {g}, {b}, 1)");
                }
            }
        }

        /// <summary>
        /// (Registrar Script do Gráfico) (Privado) Monta e registra o script JavaScript necessário para inicializar o gráfico na página usando a biblioteca Chart.js.
        /// </summary>
        private void RegisterChartScript()
        {
            string chartJsUrl = "https://cdn.jsdelivr.net/npm/chart.js";
            ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "ChartJsLib", chartJsUrl);

            string chartTypeString = Type.ToString().ToLower();
            string stackedJs = (Type == ChartType.Bar && BarMode == ChartBarMode.Stacked).ToString().ToLower();
            string scaleTypeJs = ScaleType.ToString().ToLower();
            string scalesConfig = "";
            string maxYOptionJs = "";

            // Define as strings de cultura para serem usadas no JavaScript
            string localeString = (PercentageCulture == TooltipNumberCulture.Brazilian) ? "pt-BR" : "en-US";
            string currencyString = (PercentageCulture == TooltipNumberCulture.Brazilian) ? "BRL" : "USD";

            string ticksCallbackJs = "";

            if (MaxYValue.HasValue)
            {
                maxYOptionJs = $"max: {MaxYValue.Value.ToString(CultureInfo.InvariantCulture)},";
            }

            // Lógica para exibir '%' no eixo Y (só para Barra/Linha)
            if (YAxisShowPercentage && (Type == ChartType.Bar || Type == ChartType.Line))
            {
                ticksCallbackJs = @"
        ticks: {
            callback: function(value, index, values) {
                return value + '%';
            }
        },";
            }

            // Configurações de escalas (só para Barra/Linha)
            if (Type == ChartType.Bar || Type == ChartType.Line)
            {
                if (Orientation == ChartOrientation.Horizontal)
                {
                    scalesConfig = $@"
            x: {{
                type: '{scaleTypeJs}',
                beginAtZero: true,
                stacked: {stackedJs},
                {ticksCallbackJs}
                {maxYOptionJs}
            }},
            y: {{
                stacked: {stackedJs}
            }}";
                }
                else
                {
                    scalesConfig = $@"
            x: {{
                stacked: {stackedJs}
            }},
            y: {{
                type: '{scaleTypeJs}',
                beginAtZero: true,
                stacked: {stackedJs},
                {ticksCallbackJs}
                {maxYOptionJs}
            }}";
                }
            }

            string indexAxisOption = (Orientation == ChartOrientation.Horizontal) ? "indexAxis: 'y'," : "";
            string maintainAspectRatioJs = AspectRatio.HasValue ? "true" : "false";
            string aspectRatioJs = AspectRatio.HasValue ? $"aspectRatio: {AspectRatio.Value.ToString(CultureInfo.InvariantCulture)}," : "";
            string initFunctionName = $"initChart_{ClientID}";

            // --- LÓGICA DE TOOLTIP UNIFICADA ---
            string tooltipCallbackJs;

            // Se for Barra ou Linha, usa o tooltip complexo com Moeda + Porcentagem
            if (Type == ChartType.Bar || Type == ChartType.Line)
            {
                tooltipCallbackJs = $@"
        label: function(context) {{
            var dataset = context.dataset;
            var index = context.dataIndex;
            var locale = '{localeString}';

            var percentValue = context.parsed.y;
            if (context.chart.options.indexAxis === 'y') {{
               percentValue = context.parsed.x;
            }}

            var formattedPercent = percentValue.toLocaleString(locale, {{ minimumFractionDigits: 2, maximumFractionDigits: 2 }});
            var originalValue = dataset.originalValues ? dataset.originalValues[index] : null;

            if (originalValue !== null && originalValue !== undefined) {{
                var valorFormatado = originalValue.toLocaleString(locale, {{ style: 'currency', currency: '{currencyString}' }});
                return dataset.label + ': ' + valorFormatado + ' (' + formattedPercent + '%)';
            }} else {{
                return dataset.label + ': ' + formattedPercent + '%';
            }}
        }}";
            }
            // Para outros tipos (Rosca/Pizza), usa o tooltip simples formatado como Moeda
            else
            {
                tooltipCallbackJs = $@"
        label: function(context) {{
            var label = context.label || '';
            if (label) {{
                label += ': ';
            }}

            var dataset = context.dataset;
            var index = context.dataIndex;
            var originalValue = dataset.originalValues ? dataset.originalValues[index] : context.raw;

            if (originalValue !== null && typeof originalValue !== 'undefined') {{
                label += new Intl.NumberFormat('{localeString}', {{ style: 'currency', currency: '{currencyString}' }}).format(originalValue);
            }}
            return label;
        }}";
            }

            // Monta o plugin do tooltip com o callback JS definido acima
            string tooltipPlugin = $@"
    plugins: {{
        tooltip: {{
            callbacks: {{
                {tooltipCallbackJs}
            }}
        }}
    }},";

            // Script final de inicialização do gráfico
            string script = $@"
    if (document.getElementById('{ClientID}_chartCanvas')) {{
        window['{initFunctionName}'] = function() {{
            var ctx = document.getElementById('{ClientID}_chartCanvas').getContext('2d');
            if (window.{ClientID}_myChart instanceof Chart) {{
                window.{ClientID}_myChart.destroy();
            }}
            
            window.{ClientID}_myChart = new Chart(ctx, {{
                type: '{chartTypeString}',
                data: {{
                    labels: {LabelsJson},
                    datasets: {DatasetsJson}
                }},
                options: {{
                    {indexAxisOption}
                    responsive: true,
                    maintainAspectRatio: {maintainAspectRatioJs},
                    {aspectRatioJs}
                    {tooltipPlugin}
                    scales: {{ {scalesConfig} }}
                }}
            }});
        }};
        window['{initFunctionName}']();
    }}";

            ScriptManager.RegisterStartupScript(this, GetType(), $"{ClientID}_ChartScript", script, true);
        }

    }

    /// <summary>
    /// (Dados do Gráfico) Representa um único ponto de dados para o gráfico, contendo um rótulo e um valor.
    /// </summary>
    [Serializable]
    public class ChartData
    {
        /// <summary>
        /// (Rótulo) O rótulo do ponto de dados (ex: nome da categoria).
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// (Valor) O valor numérico do ponto de dados.
        /// </summary>
        public decimal Value { get; set; }

        public ChartData(string label, decimal value)
        {
            Label = label;
            Value = value;
        }
    }

    /// <summary>
    /// (Conjunto de Dados do Gráfico) Representa um conjunto de dados (dataset) para o gráfico. Um gráfico pode ter vários datasets.
    /// </summary>
    [Serializable]
    public class ChartDataset
    {
        /// <summary>
        /// (rótulo) O nome do conjunto de dados, que aparece na legenda do gráfico.
        /// </summary>
        public string label { get; set; }

        /// <summary>
        /// (dados) A lista de valores numéricos que compõem este conjunto de dados.
        /// </summary>
        public List<object> data { get; set; }

        /// <summary>
        /// (corDeFundo) Lista de cores de fundo para cada ponto de dados no conjunto.
        /// </summary>
        public List<string> backgroundColor { get; set; }

        /// <summary>
        /// (corDaBorda) Lista de cores de borda para cada ponto de dados no conjunto.
        /// </summary>
        public List<string> borderColor { get; set; }

        /// <summary>
        /// (larguraDaBorda) A largura da borda dos elementos do gráfico (ex: barras, pontos de linha).
        /// </summary>
        public int borderWidth { get; set; } = 1;

        public List<decimal> originalValues { get; set; }

        public ChartDataset()
        {
            data = new List<object>();
            originalValues = new List<decimal>();
            backgroundColor = new List<string>();
            borderColor = new List<string>();
        }
    }

    /// <summary>
    /// (Dados Genéricos do Gráfico) Estrutura principal que encapsula todos os dados necessários para renderizar um gráfico.
    /// </summary>
    [Serializable]
    public class GenericChartData
    {
        /// <summary>
        /// (rótulos) Lista de rótulos (strings) que são exibidos no eixo principal (geralmente o eixo X).
        /// </summary>
        public List<string> labels { get; set; }

        /// <summary>
        /// (conjuntosDeDados) Lista de conjuntos de dados (datasets) que serão exibidos no gráfico.
        /// </summary>
        public List<ChartDataset> datasets { get; set; }

        public GenericChartData()
        {
            labels = new List<string>();
            datasets = new List<ChartDataset>();
        }
    }

    /// <summary>
    /// Define os tipos de gráfico disponíveis.
    /// </summary>
    public enum ChartType
    {
        Bar,
        Doughnut,
        Line,
        Pie
    }

    /// <summary>
    /// Define a orientação do gráfico (vertical ou horizontal).
    /// </summary>
    public enum ChartOrientation
    {
        Vertical,
        Horizontal
    }

    /// <summary>
    /// Define o tipo de escala para os eixos do gráfico.
    /// </summary>
    public enum ChartScaleType
    {
        Linear,
        Logarithmic
    }

    /// <summary>
    /// Define o modo de exibição para gráficos de barra com múltiplos datasets.
    /// </summary>
    public enum ChartBarMode
    {
        /// <summary>
        /// (Agrupado) As barras de diferentes datasets são exibidas lado a lado.
        /// </summary>
        Grouped

        /// <summary>
        /// (Empilhado) As barras de diferentes datasets são empilhadas umas sobre as outras.
        /// </summary>
      , Stacked
    }

    // Adicione este enum junto com os outros (ChartType, ChartOrientation, etc.)
    /// <summary>
    /// Define a cultura para formatação de números nos tooltips.
    /// </summary>
    public enum TooltipNumberCulture
    {
        /// <summary>
        /// Padrão brasileiro (ex: 1.234,56)
        /// </summary>
        Brazilian,

        /// <summary>
        /// Padrão americano (ex: 1,234.56)
        /// </summary>
        American
    }
}