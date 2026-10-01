using System;
using System.IO;
using System.Web.Hosting;

namespace TT_Flow.FrameWork.IA
{
    // Limpeza/retencao de dados da IA: purga a auditoria antiga (via SP, com throttle)
    // e apaga arquivos temporarios orfaos em App_Data/IA_Temp.
    public static class cls_IA_Manutencao
    {
        // Execucao automatica chamada a cada mensagem. A SP faz o throttle (1x/~20h);
        // aqui nunca lanca excecao para nao afetar o chat.
        public static void ExecutarSeNecessario(cls_IA_Config config, cls_IA_Repositorio repositorio)
        {
            try
            {
                if (config == null || repositorio == null)
                {
                    return;
                }

                IAManutencaoResultado resultado = repositorio.ExecutarManutencao(config.RetencaoDias, false);
                if (resultado != null && resultado.Executou)
                {
                    LimparArquivosTemporariosOrfaos(config.ArquivosRetencaoOriginalHoras);
                }
            }
            catch
            {
                // manutencao e best-effort; qualquer falha e ignorada
            }
        }

        // Execucao manual (botao de admin): forca a limpeza e devolve os numeros para exibir.
        public static IAManutencaoResultado ExecutarAgora(cls_IA_Config config, cls_IA_Repositorio repositorio)
        {
            int retencaoDias = config != null ? config.RetencaoDias : 0;
            int retencaoHoras = config != null ? config.ArquivosRetencaoOriginalHoras : 24;

            IAManutencaoResultado resultado = repositorio.ExecutarManutencao(retencaoDias, true) ?? new IAManutencaoResultado();
            resultado.ArquivosTemporariosRemovidos = LimparArquivosTemporariosOrfaos(retencaoHoras);
            return resultado;
        }

        // Apaga arquivos em App_Data/IA_Temp mais antigos que a retencao configurada
        // (orfaos de conversoes que nao foram concluidas, ex.: worker parado).
        public static int LimparArquivosTemporariosOrfaos(int retencaoHoras)
        {
            int removidos = 0;

            try
            {
                string pasta = HostingEnvironment.MapPath("~/App_Data/IA_Temp");
                if (string.IsNullOrWhiteSpace(pasta))
                {
                    pasta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "IA_Temp");
                }

                if (!Directory.Exists(pasta))
                {
                    return 0;
                }

                int horas = retencaoHoras < 1 ? 1 : retencaoHoras;
                DateTime limiteUtc = DateTime.UtcNow.AddHours(-horas);

                foreach (string caminho in Directory.GetFiles(pasta))
                {
                    try
                    {
                        if (File.GetLastWriteTimeUtc(caminho) < limiteUtc)
                        {
                            File.Delete(caminho);
                            removidos++;
                        }
                    }
                    catch
                    {
                        // arquivo em uso ou ja removido; ignora
                    }
                }
            }
            catch
            {
                // acesso a pasta indisponivel; ignora
            }

            return removidos;
        }
    }
}
