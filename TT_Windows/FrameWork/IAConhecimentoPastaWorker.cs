using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using TT.FrameWork;

namespace TT_Windows
{
    /*
        Job tmrConhecimentoPasta: pasta de entrada ("hot folder") da Base de Conhecimento da IA.

        Largar um arquivo na pasta equivale a subi-lo pela tela IA - Conhecimento do TT_Flow: mesma
        validacao de extensao e de conteudo real (magic number), mesmo dedup por SHA-256, e o documento
        gravado pelos mesmos passos da fila de arquivos (IAArquivoMarkdownWorker.Converter e
        PersistirConversao). Aparece na tela Conhecimento igual a um que entrou pelo upload.

        Por que mora aqui e nao no TT_Flow (decisao de 16/09/2026): a execucao de jobs e centralizada
        no TT_Windows. Havia uma versao desta varredura no TT_Flow, disparada por endpoint local e tarefa
        agendada do Windows; ela foi removida para existir uma implementacao so.

        Regra de operacao: MAO UNICA. So adiciona - nada sai da base porque sumiu da pasta. O arquivo lido
        e MOVIDO para _Processados (ou _Erros), entao a pasta de entrada mostra sempre e so o que falta
        entrar. Remover documento continua sendo pela tela.

        O registro nasce com sArquivoTemporario vazio: RESERVAR_ARQUIVO_PROCESSAMENTO exige esse campo
        preenchido, entao o job tmrProcessaArquivosIA nunca pega estes documentos (sem processamento
        duplo). A conversao e feita aqui mesmo, na hora.

        Sem OCR: o conversor do TT_Windows so le camada de texto. PDF escaneado vira ERRO_CONVERSAO com
        mensagem clara, e o arquivo vai para _Erros.
    */
    public class IAConhecimentoPastaWorker
    {
        private const string Procedure = "sp_Manipula_tbl_Flow_IA_Chat";

        public const string SubpastaProcessados = "_Processados";
        public const string SubpastaErros = "_Erros";

        private static readonly HashSet<string> ExtensoesPermitidas = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".docx", ".txt", ".md", ".csv", ".xlsx"
        };

        public class ResultadoVarredura
        {
            public bool Executou { get; set; }
            // Pasta configurada, mas inexistente ou sem permissao: e o unico caso "parado" que merece log.
            public bool FalhaAcesso { get; set; }
            public string Mensagem { get; set; }
            public int Ingeridos { get; set; }
            public int Duplicados { get; set; }
            public int Erros { get; set; }
            public int Adiados { get; set; }
            public int Restantes { get; set; }

            public bool TeveMovimento
            {
                get { return Ingeridos + Duplicados + Erros + Adiados > 0; }
            }
        }

        public ResultadoVarredura Executar(Action<string> log)
        {
            ResultadoVarredura resultado = new ResultadoVarredura { Mensagem = string.Empty };

            IAArquivoMarkdownWorker.IAConfigWorker config = IAArquivoMarkdownWorker.CarregarConfig();
            if (!config.IAHabilitada)
            {
                resultado.Mensagem = "IA desabilitada.";
                return resultado;
            }

            // Pasta vazia na configuracao = funcionalidade desligada. Nao e erro e nao loga a cada ciclo.
            string pastaRaiz = (config.PastaEntrada ?? string.Empty).Trim();
            if (pastaRaiz.Length == 0)
            {
                resultado.Mensagem = "Pasta de entrada nao configurada.";
                return resultado;
            }

            int idUsuario = config.PastaIdUsuario;

            if (!Directory.Exists(pastaRaiz))
            {
                resultado.Mensagem = "Pasta de entrada nao encontrada ou sem acesso: " + pastaRaiz;
                resultado.FalhaAcesso = true;
                AuditarErroGeral(idUsuario, pastaRaiz, "pasta inexistente ou sem permissao");
                return resultado;
            }

            string pastaProcessados;
            string pastaErros;
            List<Candidato> candidatos;
            try
            {
                pastaProcessados = GarantirSubpasta(pastaRaiz, SubpastaProcessados);
                pastaErros = GarantirSubpasta(pastaRaiz, SubpastaErros);
                candidatos = Listar(pastaRaiz);
            }
            catch (Exception ex)
            {
                resultado.Mensagem = "Sem acesso de escrita/leitura na pasta de entrada: " + ex.Message;
                resultado.FalhaAcesso = true;
                AuditarErroGeral(idUsuario, pastaRaiz, ex.Message);
                return resultado;
            }

            resultado.Executou = true;
            if (candidatos.Count == 0)
            {
                resultado.Mensagem = "Nenhum arquivo novo na pasta.";
                return resultado;
            }

            int teto = config.PastaMaxPorVarredura > 0 ? config.PastaMaxPorVarredura : 20;
            resultado.Restantes = Math.Max(0, candidatos.Count - teto);

            foreach (Candidato candidato in candidatos.Take(teto))
            {
                string situacao = Processar(candidato, config, idUsuario, pastaProcessados, pastaErros, log);
                switch (situacao)
                {
                    case "INGERIDO": resultado.Ingeridos++; break;
                    case "DUPLICADO": resultado.Duplicados++; break;
                    case "ADIADO": resultado.Adiados++; break;
                    default: resultado.Erros++; break;
                }
            }

            resultado.Mensagem = MontarResumo(resultado);

            IAArquivoMarkdownWorker.RegistrarAuditoria(0, idUsuario, "CONHECIMENTO_PASTA_VARREDURA", resultado.Erros > 0 ? "ALERTA" : "INFO", new JObject
            {
                { "pasta", pastaRaiz },
                { "ingeridos", resultado.Ingeridos },
                { "duplicados", resultado.Duplicados },
                { "erros", resultado.Erros },
                { "adiados", resultado.Adiados },
                { "restantes", resultado.Restantes },
                { "origem", "TT_WINDOWS" }
            });

            return resultado;
        }

        // Devolve INGERIDO | DUPLICADO | ADIADO | ERRO
        private static string Processar(Candidato candidato, IAArquivoMarkdownWorker.IAConfigWorker config, int idUsuario,
            string pastaProcessados, string pastaErros, Action<string> log)
        {
            int idArquivoIA = 0;
            try
            {
                string extensao = Path.GetExtension(candidato.Nome);
                if (!ExtensoesPermitidas.Contains(extensao))
                {
                    return Falhar(candidato, pastaErros, idUsuario, idArquivoIA, "Formato nao suportado. Use PDF, DOCX, TXT, MD, CSV ou XLSX.", log);
                }

                FileInfo info = new FileInfo(candidato.CaminhoCompleto);
                long maxBytes = (long)config.MaxMBArquivo * 1024L * 1024L;
                if (info.Length <= 0)
                {
                    return Falhar(candidato, pastaErros, idUsuario, idArquivoIA, "Arquivo vazio.", log);
                }

                if (info.Length > maxBytes)
                {
                    return Falhar(candidato, pastaErros, idUsuario, idArquivoIA, "Arquivo acima do limite de " + config.MaxMBArquivo + " MB.", log);
                }

                // Arquivo ainda sendo copiado para a pasta nao e erro: fica para o proximo ciclo. Sem isso
                // uma copia em andamento viraria documento truncado ou entulho em _Erros.
                byte[] conteudo;
                if (!TentarLer(candidato.CaminhoCompleto, out conteudo))
                {
                    return "ADIADO";
                }

                if (!IAArquivoMarkdownWorker.ConteudoCoerenteComExtensao(conteudo, extensao))
                {
                    return Falhar(candidato, pastaErros, idUsuario, idArquivoIA, "O conteudo do arquivo nao corresponde a extensao.", log);
                }

                string hash = CalcularHashSHA256(conteudo);
                string jaExiste = ConsultarPorHash(hash);
                if (jaExiste != null)
                {
                    Mover(candidato.CaminhoCompleto, pastaProcessados, candidato.Nome);
                    log?.Invoke("Base de conhecimento: " + candidato.Nome + " ja esta na base (" + jaExiste + ").");
                    return "DUPLICADO";
                }

                string nomeSeguro = NomeSeguro(candidato.Nome);
                idArquivoIA = SalvarArquivo(candidato, nomeSeguro, extensao, conteudo.Length, hash, idUsuario);
                if (idArquivoIA <= 0)
                {
                    return Falhar(candidato, pastaErros, idUsuario, idArquivoIA, "Nao foi possivel registrar o documento na base.", log);
                }

                IAArquivoMarkdownWorker.RegistrarAuditoria(0, idUsuario, "CONHECIMENTO_PASTA_ARQUIVO", "INFO", new JObject
                {
                    { "idArquivoIA", idArquivoIA },
                    { "nome", nomeSeguro },
                    { "categoria", candidato.Categoria },
                    { "tamanhoBytes", conteudo.Length },
                    { "hash", hash },
                    { "origem", "TT_WINDOWS" }
                });

                IAArquivoMarkdownWorker.IAArquivoConvertido convertido =
                    IAArquivoMarkdownWorker.Converter(conteudo, nomeSeguro, extensao, string.Empty, config.MaxCharsTrecho);
                convertido.IdArquivoIA = idArquivoIA;
                IAArquivoMarkdownWorker.PersistirConversao(convertido, idUsuario);

                if (convertido.Status != "PRONTO")
                {
                    // O registro FICA na base com o erro, igual ao upload pela tela: aparece em IA -
                    // Conhecimento e da para ver o motivo la. So o binario vai para _Erros.
                    return Falhar(candidato, pastaErros, idUsuario, idArquivoIA,
                        string.IsNullOrWhiteSpace(convertido.Erro) ? "Falha na conversao." : convertido.Erro, log);
                }

                Mover(candidato.CaminhoCompleto, pastaProcessados, candidato.Nome);
                log?.Invoke("Base de conhecimento: " + candidato.Nome + " ingerido (" + convertido.Trechos.Count + " trecho(s)).");
                return "INGERIDO";
            }
            catch (Exception ex)
            {
                return Falhar(candidato, pastaErros, idUsuario, idArquivoIA, ex.Message, log);
            }
        }

        private static string Falhar(Candidato candidato, string pastaErros, int idUsuario, int idArquivoIA, string motivo, Action<string> log)
        {
            IAArquivoMarkdownWorker.RegistrarAuditoria(0, idUsuario, "CONHECIMENTO_PASTA_ARQUIVO_ERRO", "ERRO", new JObject
            {
                { "nome", candidato.Nome },
                { "categoria", candidato.Categoria },
                { "idArquivoIA", idArquivoIA },
                { "erro", motivo },
                { "origem", "TT_WINDOWS" }
            });

            Mover(candidato.CaminhoCompleto, pastaErros, candidato.Nome);
            log?.Invoke("Base de conhecimento: " + candidato.Nome + " recusado - " + motivo);
            return "ERRO";
        }

        private static void AuditarErroGeral(int idUsuario, string pasta, string erro)
        {
            IAArquivoMarkdownWorker.RegistrarAuditoria(0, idUsuario, "CONHECIMENTO_PASTA_ERRO", "ERRO", new JObject
            {
                { "pasta", pasta },
                { "erro", erro },
                { "origem", "TT_WINDOWS" }
            });
        }

        // Mesmos parametros que cls_IA_Repositorio.SalvarArquivo do TT_Flow usa no upload da base.
        private static int SalvarArquivo(Candidato candidato, string nomeSeguro, string extensao, long tamanho, string hash, int idUsuario)
        {
            DataSet ds = BD.ExecutarDataSet(Procedure, new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR_ARQUIVO" },
                { "@idConversaIA", "0" },
                { "@idUsuario", idUsuario.ToString() },
                { "@sNomeOriginal", nomeSeguro },
                { "@sExtensao", extensao },
                { "@sMimeType", string.Empty },
                { "@nTamanhoBytes", tamanho.ToString() },
                { "@sHashSHA256", hash },
                { "@sStatusArquivo", "CONVERTENDO" },
                { "@sArquivoTemporario", string.Empty },
                { "@nRetencaoHoras", "24" },
                { "@sTipoArquivo", "B" },
                { "@sCategoriaConhecimento", candidato.Categoria },
                // NomeSeguro admite 240 chars, mas sTituloConhecimento e NVARCHAR(200)
                { "@sTituloConhecimento", Limitar(Path.GetFileNameWithoutExtension(nomeSeguro), 200) }
            });

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0 || !ds.Tables[0].Columns.Contains("idArquivoIA"))
            {
                return 0;
            }

            int id;
            return int.TryParse(Convert.ToString(ds.Tables[0].Rows[0]["idArquivoIA"]), out id) ? id : 0;
        }

        // Nome/titulo do documento ja existente com este hash, ou null. Ignora documentos removidos pela
        // tela: remover e depois largar o arquivo de novo na pasta reingere.
        private static string ConsultarPorHash(string hash)
        {
            DataTable tb = BD.ExecutarDataTable(Procedure, new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_CONHECIMENTO_POR_HASH" },
                { "@sHashSHA256", hash }
            }, false);

            if (tb == null || tb.Rows.Count == 0)
            {
                return null;
            }

            string titulo = tb.Columns.Contains("sTitulo") ? Convert.ToString(tb.Rows[0]["sTitulo"]).Trim() : string.Empty;
            string nome = tb.Columns.Contains("sNomeOriginal") ? Convert.ToString(tb.Rows[0]["sNomeOriginal"]).Trim() : string.Empty;
            return titulo.Length > 0 ? titulo : nome;
        }

        // Raiz + subpastas de 1o nivel. O nome da subpasta vira a categoria (a raiz fica sem categoria).
        // _Processados e _Erros sao saida, nunca entrada.
        private static List<Candidato> Listar(string pastaRaiz)
        {
            List<Candidato> candidatos = new List<Candidato>();

            foreach (string caminho in Directory.GetFiles(pastaRaiz))
            {
                candidatos.Add(new Candidato(caminho, string.Empty));
            }

            foreach (string subpasta in Directory.GetDirectories(pastaRaiz))
            {
                string nomeSubpasta = new DirectoryInfo(subpasta).Name;
                if (string.Equals(nomeSubpasta, SubpastaProcessados, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(nomeSubpasta, SubpastaErros, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                foreach (string caminho in Directory.GetFiles(subpasta))
                {
                    candidatos.Add(new Candidato(caminho, Limitar(nomeSubpasta, 100)));
                }
            }

            return candidatos.OrderBy(c => c.Nome, StringComparer.OrdinalIgnoreCase).ToList();
        }

        private static string GarantirSubpasta(string pastaRaiz, string nome)
        {
            string caminho = Path.Combine(pastaRaiz, nome);
            if (!Directory.Exists(caminho))
            {
                Directory.CreateDirectory(caminho);
            }

            return caminho;
        }

        // FileShare.None: se outro processo ainda escreve no arquivo (copia em andamento), falha aqui e o
        // chamador adia. E o unico jeito confiavel de detectar isso no Windows.
        private static bool TentarLer(string caminho, out byte[] conteudo)
        {
            conteudo = null;
            try
            {
                using (FileStream fs = new FileStream(caminho, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    conteudo = new byte[fs.Length];
                    int lidos = 0;
                    while (lidos < conteudo.Length)
                    {
                        int bloco = fs.Read(conteudo, lidos, conteudo.Length - lidos);
                        if (bloco <= 0)
                        {
                            break;
                        }

                        lidos += bloco;
                    }

                    return lidos == conteudo.Length;
                }
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        // Nunca sobrescreve no destino: colidiu, sufixa com data/hora. Falha ao mover nao derruba a
        // varredura (o documento ja entrou; o binario parado so reaparece como duplicado no proximo ciclo).
        private static void Mover(string origem, string pastaDestino, string nomeArquivo)
        {
            try
            {
                string destino = Path.Combine(pastaDestino, nomeArquivo);
                if (File.Exists(destino))
                {
                    destino = Path.Combine(pastaDestino,
                        Path.GetFileNameWithoutExtension(nomeArquivo) + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + Path.GetExtension(nomeArquivo));
                }

                File.Move(origem, destino);
            }
            catch
            {
                // Sem permissao ou arquivo travado: deixa onde esta.
            }
        }

        // Hexadecimal MINUSCULO, identico a cls_IA_ArquivoMarkdownService.CalcularHashSHA256 do TT_Flow.
        // Se o formato divergir, o mesmo arquivo subido pela tela e largado na pasta nao e reconhecido
        // como repetido.
        private static string CalcularHashSHA256(byte[] conteudo)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(conteudo ?? new byte[0]);
                StringBuilder sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        // Mesma regra de cls_IA_ArquivoMarkdownService.NomeSeguro do TT_Flow.
        private static string NomeSeguro(string nomeOriginal)
        {
            string nome = Path.GetFileName(nomeOriginal ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(nome))
            {
                return "arquivo";
            }

            nome = Regex.Replace(nome, @"[\x00-\x1F<>:""/\\|?*]", "_");
            return Limitar(nome, 240);
        }

        private static string MontarResumo(ResultadoVarredura resultado)
        {
            List<string> partes = new List<string> { resultado.Ingeridos + " ingerido(s)" };
            if (resultado.Duplicados > 0) partes.Add(resultado.Duplicados + " ja na base");
            if (resultado.Erros > 0) partes.Add(resultado.Erros + " com erro");
            if (resultado.Adiados > 0) partes.Add(resultado.Adiados + " em uso");
            if (resultado.Restantes > 0) partes.Add(resultado.Restantes + " para o proximo ciclo");
            return string.Join(", ", partes) + ".";
        }

        private static string Limitar(string valor, int max)
        {
            valor = (valor ?? string.Empty).Trim();
            return valor.Length <= max ? valor : valor.Substring(0, max);
        }

        private class Candidato
        {
            public string CaminhoCompleto { get; private set; }
            public string Nome { get; private set; }
            public string Categoria { get; private set; }

            public Candidato(string caminhoCompleto, string categoria)
            {
                CaminhoCompleto = caminhoCompleto;
                Nome = Path.GetFileName(caminhoCompleto);
                Categoria = categoria ?? string.Empty;
            }
        }
    }
}
