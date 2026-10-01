using System;
using System.Text;
using System.Web.UI;

namespace TT_Flow.App.Controles
{
    public partial class Manual : UserControl
    {
        public string sNomeArquivo = string.Empty;
        public string sConfig_Introducao = string.Empty;
        public string sConfig_Guia = string.Empty;
        public string sEvento_BeforeChange = string.Empty;
        public string sEvento_AfterChange = string.Empty;

        protected void Page_Load(object sender, EventArgs e) => RegistraScript();

        #region | Configurações Padrão

        public string ConfigurarIntroducao(string passos) => ConfiguracaoGeral(passos, indicadoresPasso: false, barraProgresso: false, temModal: false);

        public string ConfigurarGuia(string passos) => ConfiguracaoGeral(passos, sairComClickFora: false, indicadoresPasso: false, barraProgresso: false, desabilitaInteracao: false, navegaComTeclado: false);

        #endregion

        #region | Utils

        /// <summary>
        /// Função utilizada para construir cada passo da Introdução.
        /// </summary>
        /// <param name="titulo">Recebe o título do passo.</param>
        /// <param name="texto">Recebe o texto do passo.</param>
        /// <param name="clientID">Recebe o ClientID do elemento na Página que será destacado.</param>
        /// <param name="posicao">Recebe a posição onde aparecerá a caixa de texto do passo.</param>
        /// <param name="numeroPasso">Recebe o número da ordem do passo.</param>
        /// <param name="desabilitaInteracao">Define se será possível interagir com o elemento destacado.</param>
        /// <param name="classeTooltip">Recebe uma classe personalizada para a caixa de texto do passo.</param>
        /// <returns>Retorna a string completa e formatada do passo.</returns>
        public string ConstruirPasso(string titulo, string texto, string clientID = "", string posicao = "bottom", int numeroPasso = 0, bool desabilitaInteracao = false, string classeTooltip = "", string classeDestaque = "")
        {
            string sNumeroPasso = numeroPasso > 0 ? $", step: {numeroPasso}" : string.Empty;
            string sDesabilitaInteracao = desabilitaInteracao ? "true" : "false";

            if (!string.IsNullOrEmpty(clientID)) clientID = $", element: '{clientID}'";

            return $@"{{title: '{titulo}', intro: `{texto}`{clientID}, position: '{posicao}'{sNumeroPasso}, disableInteraction: {sDesabilitaInteracao}, tooltipClass: '{classeTooltip}', highlightClass: '{classeDestaque}'}}";
        }

        /// <summary>
        /// Método Geral utilizado para configurar a Introdução.
        /// </summary>
        /// <param name="passos">Recebe a string com a Lista de Passos.</param>
        /// <param name="textoProximo">Recebe o texto do botão de 'Próximo Passo'.</param>
        /// <param name="textoAnterior">Recebe o texto do botão de 'Passo Anterior'.</param>
        /// <param name="textoPular">Recebe o texto do botão para 'Pular' os outros Passos.</param>
        /// <param name="textoConcluido">Recebe o texto do botão de 'Concluído', no Último Passo.</param>
        /// <param name="posicaoPadrao">Recebe a posição padrão da caixas de textos dos Passos.</param>
        /// <param name="botoes">Define se serão exibidos os botões de avançar, voltar, pular e etc, dos Passos.</param>
        /// <param name="escondeProximo">Define se o botão de 'Próximo', para avançar ao próximo Passo, será escondido no Último Passo.</param>
        /// <param name="escondeAnterior">Define se o botão de 'Anterior', para voltar ao Passo anterior, será escondido no primeiro Passo.</param>
        /// <param name="sairComClickFora">Define se será possível sair da Introdução apenas clicando fora da caixa de texto.</param>
        /// <param name="numeroPasso">Define se serão exibidos os Números dos Passos.</param>
        /// <param name="indicadoresPasso">Define se serão exibidos os indicadores dos Passos.</param>
        /// <param name="barraProgresso">Define se será exibida a Barra de Progresso dos Passos.</param>
        /// <param name="desabilitaInteracao">Define se será possível interagir com o elemento destacado, em todos os Passos.</param>
        /// <param name="classePadrao">Recebe a classe CSS para as caixas de textos dos Passos.</param>
        /// <param name="classeBotoes">Recebe a classe CSS para os Botões dos Passos.</param>
        /// <param name="classeBarraProgresso">Recebe a classe CSS adicional para a Barra de Progresso dos Passos.</param>
        /// <param name="grupo">Recebe o nome do grupo ao qual esta Introdução pertence.</param>
        public string ConfiguracaoGeral(string passos, string textoProximo = "Próximo", string textoAnterior = "Anterior", string textoPular = "", string textoConcluido = "Concluído", string posicaoPadrao = "bottom",
            bool botoes = true, bool escondeProximo = false, bool escondeAnterior = true, bool sairComClickFora = true, bool numeroPasso = true, bool indicadoresPasso = true, bool barraProgresso = true, bool desabilitaInteracao = true,
            bool navegaComTeclado = true, string classePadrao = "", string classeBotoes = "", string classeBarraProgresso = "", string grupo = "", bool temModal = true)
        {
            if (string.IsNullOrEmpty(passos)) return string.Empty;

            string sBotoes = botoes ? "true" : "false";
            string sEscondeProximo = escondeProximo ? "true" : "false";
            string sEscondeAnterior = escondeAnterior ? "true" : "false";
            string sSairComClickFora = sairComClickFora ? "true" : "false";
            string sNumeroPasso = numeroPasso ? "true" : "false";
            string sIndicadoresPasso = indicadoresPasso ? "true" : "false";
            string sBarraProgresso = barraProgresso ? "true" : "false";
            string sDesabilitaInteracao = desabilitaInteracao ? "true" : "false";
            string sNavegaComTeclado = navegaComTeclado ? "true" : "false";

            if (!string.IsNullOrEmpty(textoProximo)) textoProximo = $", nextLabel: '{textoProximo}'";
            if (!string.IsNullOrEmpty(textoAnterior)) textoAnterior = $", prevLabel: '{textoAnterior}'";
            if (!string.IsNullOrEmpty(textoPular)) textoPular = $", skipLabel: '{textoPular}'";
            if (!string.IsNullOrEmpty(textoConcluido)) textoConcluido = $", doneLabel: '{textoConcluido}'";
            if (!string.IsNullOrEmpty(posicaoPadrao)) posicaoPadrao = $", tooltipPosition: '{posicaoPadrao}'";
            if (!string.IsNullOrEmpty(classePadrao)) classePadrao = $", tooltipClass: '{classePadrao}'";
            if (!string.IsNullOrEmpty(classeBotoes)) classeBotoes = $", buttonClass: '{classeBotoes}'";

            if (!string.IsNullOrEmpty(sEvento_BeforeChange) && !sEvento_BeforeChange.Contains("onbeforechange")) sEvento_BeforeChange = $"IntroJS.onbeforechange(function (el) {{ {sEvento_BeforeChange} }});";

            if (temModal) sEvento_AfterChange = @"IntroJS.onafterchange(function (el) {
                                                        setTimeout(function() {
                                                            $('.introjs-tooltip').draggable({
                                                                handle: '.introjs-tooltip-header'
                                                            });

                                                            el = $(el);
                                                            const overlay = $('.introjs-overlay');
                                                            const helper = $('.introjs-helperLayer');

                                                            overlay.removeClass('permiteClick_Atraves');
                                                            helper.removeClass('permiteClick_Atraves');

                                                            if (el && el.attr('class') && el.attr('class').includes('modal')) {
                                                                overlay.addClass('permiteClick_Atraves');
                                                                helper.addClass('permiteClick_Atraves');
                                                            }
                                                        }, 50);
                                                    });";

            return $@"steps: [{passos}]{textoProximo}{textoAnterior}{textoPular}{textoConcluido}{posicaoPadrao}
                                , showButtons: {sBotoes}, hideNext: {sEscondeProximo}, hidePrev: {sEscondeAnterior}, exitOnOverlayClick: {sSairComClickFora}, showStepNumbers: {sNumeroPasso}, showBullets: {sIndicadoresPasso}, showProgress: {sBarraProgresso}
                                , disableInteraction: {sDesabilitaInteracao}, keyboardNavigation: {sNavegaComTeclado}, stepNumbersOfLabel: 'de'
                                {classePadrao}{classeBotoes}, progressBarAdditionalClass: '{classeBarraProgresso}', group: '{grupo}'";
        }

        #endregion

        #region | Script

        public void RegistraScript()
        {
            cmdManual.Visible = !string.IsNullOrEmpty(sNomeArquivo);
            cmdIntroducao.Visible = !string.IsNullOrEmpty(sConfig_Introducao);
            cmdGuia.Visible = !string.IsNullOrEmpty(sConfig_Guia);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("$(document).ready(function () {");
            sb.AppendLine("     window.IntroJS = introJs.tour();");
            sb.AppendLine("     var cmdManual = $('button[id*=cmdManual]');");
            sb.AppendLine("     var cmdIntroducao = $('button[id*=cmdIntroducao]');");
            sb.AppendLine("     var cmdGuia = $('button[id*=cmdGuia]');");
            sb.AppendLine("     var cmdEscondeManual = $('.escondeManual');");
            sb.AppendLine("     var iconManual = cmdEscondeManual.find('i');");
            sb.AppendLine("     cmdEscondeManual.off('click').on('click', function (e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         $('.divEsconde').toggle('fast');");
            sb.AppendLine("         iconManual.toggleClass('fa-chevron-right fa-chevron-left');");
            sb.AppendLine("         cmdEscondeManual.attr('data-original-title', iconManual.hasClass('fa-chevron-right') ? 'Recolher' : 'Expandir');");
            sb.AppendLine("     });");

            if (cmdManual.Visible)
            {
                sb.AppendLine("     cmdManual.off('click').on('click', function (e) {");
                sb.AppendLine("         e.preventDefault();");
                sb.AppendLine($"        var url = window.location.protocol + '//' + window.location.host + '/Manuais/' + '{sNomeArquivo}';");
                sb.AppendLine("         cmdEscondeManual.trigger('click');");
                sb.AppendLine("         window.open(url, '_blank');");
                sb.AppendLine("     });");
            }

            if (cmdIntroducao.Visible)
            {
                sb.AppendLine("     cmdIntroducao.off('click').on('click', function (e) {");
                sb.AppendLine("         e.preventDefault();");
                sb.AppendLine("         IntroJS = introJs.tour();");
                sb.AppendLine($"         IntroJS.setOptions({{{sConfig_Introducao}}});");
                sb.AppendLine($"         {sEvento_BeforeChange}{sEvento_AfterChange}");
                sb.AppendLine("          IntroJS.start();");
                sb.AppendLine("          cmdEscondeManual.trigger('click');");
                sb.AppendLine("     });");
            }

            if (cmdGuia.Visible)
            {
                sb.AppendLine("     cmdGuia.off('click').on('click', function (e) {");
                sb.AppendLine("         e.preventDefault();");
                sb.AppendLine("         IntroJS = introJs.tour();");
                sb.AppendLine($"         IntroJS.setOptions({{{sConfig_Guia}}});");
                sb.AppendLine($"         {sEvento_BeforeChange}{sEvento_AfterChange}");
                sb.AppendLine("          IntroJS.start();");
                sb.AppendLine("          cmdEscondeManual.trigger('click');");
                sb.AppendLine("     });");
            }

            sb.AppendLine("     if (iconManual.hasClass('fa-chevron-right')) cmdEscondeManual.trigger('click');");

            sb.AppendLine("});");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), $"js_Manual", sb.ToString(), true);
        }

        #endregion
    }
}