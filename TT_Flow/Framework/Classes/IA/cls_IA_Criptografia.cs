using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TT_Flow.FrameWork.IA
{
    /*
        Criptografia dos segredos da IA (chaves de API dos providers, token da Nuvemshop).

        POR QUE MUDOU (01/09/2026)
        --------------------------
        Ate aqui usava DPAPI com DataProtectionScope.LocalMachine, que amarra o valor cifrado
        A MAQUINA que o cifrou: outro servidor nao decifra de jeito nenhum, e uma reconstrucao
        da maquina perde as chaves gravadas.

        O assistente publico acabou ficando no mesmo servidor do TT_Flow, entao o DPAPI ainda
        atenderia. A troca foi MANTIDA por decisao do dono do sistema (01/09/2026): tira a
        amarra de maquina antes que ela vire problema numa migracao de servidor, e o custo
        ficou em administrar um segredo. Nao ha, hoje, exigencia tecnica de portabilidade.

        O formato de escrita e AES-256-CBC + HMAC-SHA256 (encrypt-then-MAC), com a chave
        derivada de um SEGREDO MESTRE que fica fora do banco. Assim qualquer servidor que
        conheca o segredo le o valor. AES-GCM seria preferivel, mas nao existe no .NET
        Framework 4.8 — AesGcm so entrou no .NET Core 3.0.

        COMPATIBILIDADE
        ---------------
        Leitura aceita os DOIS formatos: valores novos trazem o prefixo "TTv2:", e qualquer
        outra coisa e tratada como DPAPI legado. Ou seja, o que ja esta gravado continua
        funcionando na maquina do ERP. Para migrar um valor, basta regravar a chave pela tela
        de Configuracao: a escrita sempre usa o formato novo.

        ATENCAO: um valor DPAPI legado so abre na maquina que o gravou. Enquanto tudo roda no
        mesmo servidor isso nao incomoda, mas se um dia o assistente (ou o TT_Flow) mudar de
        host, e obrigatorio migrar as chaves ANTES, com o segredo mestre ja configurado nos
        dois lados. Ver Ferramentas/Migrar-ChavesIA.ps1 no projeto TT_Assistente.

        ONDE FICA O SEGREDO MESTRE
        --------------------------
        NUNCA no banco. Se a chave morasse na mesma tabela dos valores cifrados, quem lesse
        a tabela levaria cadeado e chave juntos — backup vazado, injecao de SQL ou alguem
        abrindo a tabela no SSMS teriam a chave do provider. Seria ofuscacao, nao criptografia,
        e pior do que o DPAPI que havia antes (o DPAPI ao menos inutiliza um banco roubado).

        Procurado nesta ordem:
          1. appSettings["IA.ChaveMestra"] no Web.config da aplicacao
          2. variavel de ambiente TT_IA_CHAVE_MESTRA (de maquina)
          3. tag <CHAVE_IA> no XML de configuracao de banco (C:\TT\*TT_CONFIG*.xml)

        A opcao 3 e a recomendada na operacao: esse XML ja e como a senha do SQL chega a cada
        servidor, ja esta fora do repositorio, e quem o le ja tem o banco inteiro — guardar a
        chave ali nao abre exposicao nova e nao cria passo de deploy. Como o XML e escolhido
        pelo mesmo indice que monta a conexao, a chave acompanha o ambiente (DEV/QA/PROD tem
        cada um a sua, o que e o desejado: cada um tem seu proprio banco de config).

        Gerar um valor forte (nao inventar a mao):
          [Convert]::ToBase64String((1..48 | % { Get-Random -Max 256 }))
    */
    public static class cls_IA_Criptografia
    {
        private const string PrefixoPortavel = "TTv2:";
        private const string ChaveAppSetting = "IA.ChaveMestra";
        private const string ChaveAmbiente = "TT_IA_CHAVE_MESTRA";

        private const int TamanhoIV = 16;
        private const int TamanhoHmac = 32;
        private const int Iteracoes = 100000;

        // Entropia do formato DPAPI legado — mantida apenas para LER o que ja esta gravado.
        private static readonly byte[] EntropiaLegado = Encoding.UTF8.GetBytes("TT_FLOW_IA_SECRET_V1");

        // Sal fixo de aplicacao: o segredo mestre e unico e de alta entropia, entao sal por
        // valor nao acrescenta nada aqui e obrigaria a derivar a chave a cada chamada. Com sal
        // fixo, deriva uma vez e reusa — cls_IA_Config.Carregar decifra varios valores por
        // requisicao, e 100k iteracoes por valor deixariam o chat lento.
        private static readonly byte[] SalAplicacao = Encoding.UTF8.GetBytes("TT_FLOW_IA_KDF_V2");

        private static readonly object _trava = new object();
        private static byte[] _chaveCifra;
        private static byte[] _chaveHmac;
        private static string _segredoDerivado;

        /// <summary>True quando o segredo mestre esta configurado nesta aplicacao.</summary>
        public static bool SegredoConfigurado()
        {
            return !string.IsNullOrWhiteSpace(LerSegredo());
        }

        /// <summary>True para valores no formato novo (portavel entre servidores).</summary>
        public static bool EhFormatoPortavel(string valor)
        {
            return !string.IsNullOrWhiteSpace(valor)
                && valor.Trim().StartsWith(PrefixoPortavel, StringComparison.Ordinal);
        }

        public static string Criptografar(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return string.Empty;
            }

            string segredo = LerSegredo();
            if (string.IsNullOrWhiteSpace(segredo))
            {
                /*
                    Falha alto e cedo, de proposito. A alternativa seria cair para DPAPI em
                    silencio, e ai o valor gravado so abriria nesta maquina — o assistente
                    publico quebraria em producao, com erro que nao aponta para a causa.
                    Melhor a tela de Configuracao recusar o salvamento agora.
                */
                throw new ConfigurationErrorsException(
                    "Segredo mestre da IA nao configurado. Defina appSettings[\"" + ChaveAppSetting +
                    "\"] no Web.config ou a variavel de ambiente " + ChaveAmbiente +
                    " antes de gravar chaves de API.");
            }

            byte[] chaveCifra, chaveHmac;
            DerivarChaves(segredo, out chaveCifra, out chaveHmac);

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = chaveCifra;
                aes.GenerateIV();

                byte[] textoCifrado;
                using (ICryptoTransform cifrador = aes.CreateEncryptor())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(valor.Trim());
                    textoCifrado = cifrador.TransformFinalBlock(bytes, 0, bytes.Length);
                }

                // Encrypt-then-MAC: o HMAC cobre IV + texto cifrado, e e conferido ANTES
                // de decifrar. Sem isso, o valor gravado seria maleavel por quem tem escrita
                // no banco.
                byte[] assinatura;
                using (HMACSHA256 hmac = new HMACSHA256(chaveHmac))
                {
                    byte[] paraAssinar = Concatenar(aes.IV, textoCifrado);
                    assinatura = hmac.ComputeHash(paraAssinar);
                }

                byte[] saida = Concatenar(aes.IV, assinatura, textoCifrado);
                return PrefixoPortavel + Convert.ToBase64String(saida);
            }
        }

        public static string Descriptografar(string valorCriptografado)
        {
            if (string.IsNullOrWhiteSpace(valorCriptografado))
            {
                return string.Empty;
            }

            string valor = valorCriptografado.Trim();

            if (!EhFormatoPortavel(valor))
            {
                return DescriptografarLegadoDpapi(valor);
            }

            string segredo = LerSegredo();
            if (string.IsNullOrWhiteSpace(segredo))
            {
                throw new ConfigurationErrorsException(
                    "Segredo mestre da IA nao configurado nesta aplicacao (appSettings[\"" +
                    ChaveAppSetting + "\"] ou variavel " + ChaveAmbiente + "), e o valor gravado " +
                    "esta no formato portavel.");
            }

            byte[] pacote = Convert.FromBase64String(valor.Substring(PrefixoPortavel.Length));
            if (pacote.Length <= TamanhoIV + TamanhoHmac)
            {
                throw new CryptographicException("Valor criptografado invalido ou truncado.");
            }

            byte[] chaveCifra, chaveHmac;
            DerivarChaves(segredo, out chaveCifra, out chaveHmac);

            byte[] iv = new byte[TamanhoIV];
            byte[] assinatura = new byte[TamanhoHmac];
            byte[] textoCifrado = new byte[pacote.Length - TamanhoIV - TamanhoHmac];

            Buffer.BlockCopy(pacote, 0, iv, 0, TamanhoIV);
            Buffer.BlockCopy(pacote, TamanhoIV, assinatura, 0, TamanhoHmac);
            Buffer.BlockCopy(pacote, TamanhoIV + TamanhoHmac, textoCifrado, 0, textoCifrado.Length);

            using (HMACSHA256 hmac = new HMACSHA256(chaveHmac))
            {
                byte[] esperado = hmac.ComputeHash(Concatenar(iv, textoCifrado));
                if (!ComparacaoConstante(esperado, assinatura))
                {
                    // Segredo mestre errado, ou valor adulterado. Nao decifrar.
                    throw new CryptographicException(
                        "Assinatura do valor criptografado nao confere. Segredo mestre diferente do " +
                        "usado para gravar, ou valor alterado no banco.");
                }
            }

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = chaveCifra;
                aes.IV = iv;

                using (ICryptoTransform decifrador = aes.CreateDecryptor())
                {
                    byte[] bytes = decifrador.TransformFinalBlock(textoCifrado, 0, textoCifrado.Length);
                    return Encoding.UTF8.GetString(bytes);
                }
            }
        }

        /*
            Formato antigo. So decifra na maquina que cifrou — e essa e exatamente a limitacao
            que motivou a troca. Mantido para os valores ja gravados continuarem abrindo no
            servidor do ERP ate serem regravados pela tela.
        */
        private static string DescriptografarLegadoDpapi(string valor)
        {
            byte[] protegido = Convert.FromBase64String(valor);
            byte[] bytes = ProtectedData.Unprotect(protegido, EntropiaLegado, DataProtectionScope.LocalMachine);
            return Encoding.UTF8.GetString(bytes);
        }

        private static void DerivarChaves(string segredo, out byte[] chaveCifra, out byte[] chaveHmac)
        {
            lock (_trava)
            {
                if (_chaveCifra == null || !string.Equals(_segredoDerivado, segredo, StringComparison.Ordinal))
                {
                    using (Rfc2898DeriveBytes kdf = new Rfc2898DeriveBytes(
                        segredo, SalAplicacao, Iteracoes, HashAlgorithmName.SHA256))
                    {
                        _chaveCifra = kdf.GetBytes(32);
                        _chaveHmac = kdf.GetBytes(32);
                    }

                    _segredoDerivado = segredo;
                }

                chaveCifra = _chaveCifra;
                chaveHmac = _chaveHmac;
            }
        }

        private static string LerSegredo()
        {
            try
            {
                string doConfig = ConfigurationManager.AppSettings[ChaveAppSetting];
                if (!string.IsNullOrWhiteSpace(doConfig))
                {
                    return doConfig.Trim();
                }
            }
            catch
            {
                // Sem Web.config acessivel (ex.: rodando fora de contexto web): cai para ambiente
            }

            try
            {
                string doAmbiente = Environment.GetEnvironmentVariable(ChaveAmbiente)
                                    ?? Environment.GetEnvironmentVariable(ChaveAmbiente, EnvironmentVariableTarget.Machine);
                if (!string.IsNullOrWhiteSpace(doAmbiente))
                {
                    return doAmbiente.Trim();
                }
            }
            catch
            {
                // Sem permissao de ler ambiente
            }

            /*
                Tag <CHAVE_IA> do XML de configuracao — o caminho recomendado na operacao.
                Identity.Bancos ja costuma estar carregado quando chegamos aqui (decifrar
                acontece depois de ler a config no banco), mas forcamos a carga se ainda nao,
                para nao depender da ordem das chamadas.
            */
            try
            {
                if (TT.FrameWork.Identity.Bancos.Count <= 0)
                {
                    TT.FrameWork.Funcoes.CarregarInfo_BD(0);
                }

                string doXml = TT.FrameWork.Identity.BancoAtual.sChaveIA;
                if (!string.IsNullOrWhiteSpace(doXml))
                {
                    return doXml.Trim();
                }
            }
            catch
            {
                // XML ausente, tag ausente ou banco nao configurado: segue sem segredo
            }

            return string.Empty;
        }

        // Comparacao em tempo constante: evita vazar, pelo tempo de resposta, quantos bytes
        // do HMAC bateram.
        private static bool ComparacaoConstante(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            int diferenca = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diferenca |= a[i] ^ b[i];
            }

            return diferenca == 0;
        }

        private static byte[] Concatenar(params byte[][] partes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                foreach (byte[] parte in partes)
                {
                    ms.Write(parte, 0, parte.Length);
                }

                return ms.ToArray();
            }
        }
    }
}
