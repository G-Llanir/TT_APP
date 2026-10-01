using System;

namespace TT_Flow.FrameWork.IA
{
    // Canal de progresso de quem executa um workflow para quem está esperando por ele (o chat em streaming).
    // O chat assina enquanto roda a ferramenta; o motor avisa cada passo que vai executar. Fica por thread porque o
    // streaming roda síncrono na thread do request: nada vaza para outras requisições. Sem assinante (WebMethod,
    // dry-run do editor) Avisar não faz nada.
    public static class cls_IA_Progresso
    {
        [ThreadStatic]
        private static Action<string, string> _ouvinte;

        // Registra o ouvinte (tipo, valor) até o Dispose; o anterior é restaurado (assinaturas aninhadas são seguras).
        public static IDisposable Assinar(Action<string, string> ouvinte)
        {
            Action<string, string> anterior = _ouvinte;
            _ouvinte = ouvinte;
            return new Assinatura(anterior);
        }

        public static void Avisar(string tipo, string valor)
        {
            Action<string, string> ouvinte = _ouvinte;
            if (ouvinte == null) return;

            try
            {
                ouvinte(tipo, valor ?? string.Empty);
            }
            catch
            {
                // Progresso é cortesia: falha ao escrever no stream (cliente desconectou) não derruba o workflow
            }
        }

        private sealed class Assinatura : IDisposable
        {
            private readonly Action<string, string> _anterior;
            private bool _liberada;

            public Assinatura(Action<string, string> anterior)
            {
                _anterior = anterior;
            }

            public void Dispose()
            {
                if (_liberada) return;
                _liberada = true;
                _ouvinte = _anterior;
            }
        }
    }
}
