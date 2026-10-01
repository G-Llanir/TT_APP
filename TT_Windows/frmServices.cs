using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using TT.FrameWork;
							 
						   
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using static TT.FrameWork.BD;
using static TT.FrameWork.Arquivo;
using Newtonsoft.Json.Linq;
namespace TT_Windows
{
    public partial class frmServices : Form
    {
        public frmServices() => InitializeComponent();

        #region | Classes

        TT_Hub.API.Commbox commbox = new TT_Hub.API.Commbox();
        string ImportarXML_Path = Path.Combine(Application.StartupPath, "../../../TT_Flow/App/Importacao");
        string sPath_TFlow = CarregarParametro(Parametro.Caminho_TFlow);
        public int maxLabelLength = 30;
        public string Pagar = "N";
        public string Receber = "N";
        public string Certificado = "N";
        public static DateTime? ultimaExecucaoNR = null;
        public Dictionary<string, string> vParametros_Timers = new Dictionary<string, string>();

        #endregion

        #region | Carregamento do Formulário

        private void frmServices_Load(object sender, EventArgs e)
        {
            CarregaParametros();

            if (tmrCommboxRecepcao.Enabled) { LOG_Status("Iniciando Processos Commbox"); Task.Run(() => Processar_tmrCommboxRecepcao()); }
            if (tmrValidaEquipamentoSemEventos.Enabled) { LOG_Status("Iniciando Processo de Validação de Equipamento sem eventos"); Task.Run(() => Processar_tmrValidaEquipamentoSemEventos()); }
            if (tmrEnviaMensagensPendentes.Enabled) { LOG_Status("Iniciando Processo de Envio de Emails"); Task.Run(() => Processar_tmrEnviaMensagensPendentes()); }
            if (tmrPing.Enabled) { LOG_Status("Iniciando Processo de Monitoramento PING"); Task.Run(() => Processar_tmrPing()); }
            if (tmrEnviarPagamentos.Enabled) { LOG_Status("Iniciando Processo de Envio email Pagamentos"); Task.Run(() => Processar_tmrEnviarPagamentos()); }
            if (tmrImportarXML.Enabled) { LOG_Status("Iniciando Processo de Monitoramento de documentos XML"); Task.Run(() => Processar_tmrImportarXML()); }
            if (tmrZplPrinter.Enabled) { LOG_Status("Iniciando Processo de Impressões da Zpl Printer"); Task.Run(() => Processar_tmrZplPrinter()); }
            if (tmrAtualiza_ST_LegisWeb.Enabled) { LOG_Status("Iniciando Processo de Atualização de ST - LegisWeb"); Task.Run(() => Processar_tmrAtualiza_ST_LegisWeb()); }
            if (tmrAtualiza_Impostos_LegisWeb.Enabled) { LOG_Status("Iniciando Processo de Atualização de Impostos por NCM - LegisWeb"); Task.Run(() => Processar_tmrAtualiza_Impostos_LegisWeb()); }
            if (tmrEnviaEmails_AsSete.Enabled) { LOG_Status("Iniciando processo de Geração de Emails programados para às 7 da manhã"); Task.Run(() => Processar_tmrEnviaEmails_AsSete()); }
            if (tmrGerenciadorNFe.Enabled) { LOG_Status("Iniciando Processo de Gerenciador NF-e"); Task.Run(() => Processar_tmrGerenciadorNFe()); }
            if (tmrAtualiza_Status_ClientePontual.Enabled) { LOG_Status("Iniciando Processo de Atualizar Status Cliente"); Task.Run(() => Processar_tmrAtualiza_Status_ClientePontual()); }
            if (tmrProcessaArquivosIA.Enabled) { LOG_Status("Iniciando Processo de Arquivos IA"); Task.Run(() => Processar_tmrProcessaArquivosIA()); }
            if (tmrConhecimentoPasta.Enabled) { LOG_Status("Iniciando Processo da Pasta da Base de Conhecimento IA"); Task.Run(() => Processar_tmrConhecimentoPasta()); }

        }

        void CarregaParametros(string atualiza = "Carregando")
        {
            FUNCOES.CarregarInfo_BD(0);
            LOG_Status($"{atualiza} Parâmetros - BD: {Identity.BancoAtual.sNome}");

            using (DataSet ds = ExecutarDataSet("sp_Select", new Dictionary<string, string> { { "@sTabela", "Parametros" } }))
            {
                if (ValidarDataSet(ds))
                {
                    ImportarXML_Path = RETORNO.DATASET(ds, "sCaminho_ImportarXML");

                    if (!System.Diagnostics.Debugger.IsAttached) sPath_TFlow = RETORNO.DATASET(ds, "sCaminho_TFlow");

                    if (ds.Tables.Count > 1)
                    {
                        try
                        {
                            foreach (DataRow row in ds.Tables[1].Rows)
                            {
                                Timer timer = null;
                                bool bHabilitado = Convert.ToBoolean(row["bHabilitado"]);
                                int nIntervaloSegundos = Convert.ToInt32(row["nIntervaloSegundos"]) * 1000;

                                string[] sNomeTimer = row["sNomeTimer"].ToString().Split('|');
                                string sNome = sNomeTimer[0];
                                string sParametrosTimer = sNomeTimer.Length > 1 ? sNomeTimer[1] : string.Empty;

                                switch (sNome)
                                {
                                    case "tmrCommboxRecepcao": timer = tmrCommboxRecepcao; vParametros_Timers[sNome] = sParametrosTimer; break;
                                    case "tmrValidaEquipamentoSemEventos": timer = tmrValidaEquipamentoSemEventos; vParametros_Timers[sNome] = sParametrosTimer; break;
                                    case "tmrEnviaMensagensPendentes": timer = tmrEnviaMensagensPendentes; vParametros_Timers[sNome] = sParametrosTimer; break;
                                    case "tmrPing": timer = tmrPing; vParametros_Timers[sNome] = sParametrosTimer; break;
                                    case "tmrEnviarPagamentos": timer = tmrEnviarPagamentos; vParametros_Timers[sNome] = sParametrosTimer; break;
                                    case "tmrImportarXML": timer = tmrImportarXML; vParametros_Timers[sNome] = sParametrosTimer; break;
                                    case "tmrZplPrinter": timer = tmrZplPrinter; vParametros_Timers[sNome] = sParametrosTimer; break;
                                    case "tmrAtualiza_ST_LegisWeb": timer = tmrAtualiza_ST_LegisWeb; vParametros_Timers[sNome] = sParametrosTimer; break;
                                    case "tmrAtualiza_Impostos_LegisWeb": timer = tmrAtualiza_Impostos_LegisWeb; vParametros_Timers[sNome] = sParametrosTimer; break;
                                    case "tmrEnviaEmails_AsSete": timer = tmrEnviaEmails_AsSete; vParametros_Timers[sNome] = sParametrosTimer; break;
                                    case "tmrGerenciadorNFe": timer = tmrGerenciadorNFe; vParametros_Timers[sNome] = sParametrosTimer; break;
                                    case "tmrAtualiza_Status_ClientePontual": timer = tmrAtualiza_Status_ClientePontual; vParametros_Timers[sNome] = sParametrosTimer; break;
                                    case "tmrProcessaArquivosIA": timer = tmrProcessaArquivosIA; vParametros_Timers[sNome] = sParametrosTimer; break;
                                    case "tmrConhecimentoPasta": timer = tmrConhecimentoPasta; vParametros_Timers[sNome] = sParametrosTimer; break;
                                }

                                // Linha da tbl_Parametros_Timers sem timer correspondente neste executavel (ex.: job
                                // cadastrado para uma versao mais nova, ou de outro servico). Antes caia em
                                // NullReferenceException, o catch abaixo abortava o laco e TODOS os timers das
                                // linhas seguintes ficavam sem configuracao - sem nenhum aviso claro de qual.
                                if (timer == null)
                                {
                                    LOG_Status($"Timer desconhecido na tbl_Parametros_Timers, ignorado: {sNome}");
                                    continue;
                                }

                                if (timer.Enabled != bHabilitado) timer.Enabled = bHabilitado;
                                if (timer.Interval != nIntervaloSegundos) timer.Interval = nIntervaloSegundos;
                            }
                        }
                        catch (Exception ex)
                        {
                            LOG_Status($"Houve um erro ao Carregar os Parâmetros dos Serviços: {ex.Message}");
                        }
                    }
                }
            }
        }

        private void cmdAtualizar_Click(object sender, EventArgs e) => CarregaParametros("Atualizando");

        #endregion

        #region | Consulta Commbox

        private void tmrCommboxRecepcao_Tick(object sender, EventArgs e) => Task.Run(() => Processar_tmrCommboxRecepcao());

        private void Processar_tmrCommboxRecepcao()
        {
            BeginInvoke((Action)(() => tmrCommboxRecepcao.Stop()));

            LOG_Status("Consultando Envio de Ações");
            DateTime dtInicio = DateTime.Now;

            string txt = "";
            Invoke((Action)(() => txt = txtStatus.Text));

            commbox.EnviarAcaoEquipamento(ref txt, "");

            BeginInvoke((Action)(() => txtStatus.Text = txt));

            LOG_Status("Consultando Eventos");

            Invoke((Action)(() => txt = txtStatus.Text));

            commbox.ConsultarStatusEquipamentos(ref txt, "0");

            BeginInvoke((Action)(() => txtStatus.Text = txt));

            BeginInvoke((Action)(() => tmrPing.Start()));

            TimeSpan dtTempo = DateTime.Now - dtInicio;
            LOG_Status("Processo Finalizado - Tempo: " + dtTempo.ToString());

            BeginInvoke((Action)(() => tmrCommboxRecepcao.Start()));
        }

        #endregion

        #region | Verificar Equipamentos sem Eventos

        private void tmrValidaEquipamentoSemEventos_Tick(object sender, EventArgs e) => Task.Run(() => Processar_tmrValidaEquipamentoSemEventos());

        private void Processar_tmrValidaEquipamentoSemEventos()
        {
            BeginInvoke((Action)(() => tmrValidaEquipamentoSemEventos.Stop()));
            LOG_Outros("Validando equipamentos sem Eventos");

            using (DataSet dsPesquisa = ExecutarDataSet("sp_HUB_Job_ValidaEquipamentoSemEventos"))
            {
                LOG_Outros("Quantidade de erros: " + RETORNO.DATASET(dsPesquisa, "nQtdErros"));
            }

            LOG_Outros("Processo Finalizado");

            BeginInvoke((Action)(() => tmrValidaEquipamentoSemEventos.Start()));
        }

        #endregion

        #region | PING de Equipamentos

        private void tmrPing_Tick(object sender, EventArgs e) => Task.Run(() => Processar_tmrPing());

        private void Processar_tmrPing()
        {
            BeginInvoke((Action)(() => tmrPing.Stop()));
            DateTime dtInicio = DateTime.Now;

            try
            {
                dtInicio = DateTime.Now;
                LOG_Ping("Iniciando processo de PING");
                DataSet dsPesquisa = ExecutarDataSet("sp_HUB_Manipula_tbl_Equipamentos", new Dictionary<string, string> { { "@sFuncao", "CONSULTA_PING" } });

                if (ValidarDataSet(dsPesquisa, out string sErro))
                {
                    LOG_Ping("Abrindo Conexão com O BD");
                    SqlConnection cn = new SqlConnection(StringDeConexao);
                    SqlCommand objExecBD = new SqlCommand("sp_HUB_Manipula_tbl_Equipamentos", cn) { CommandType = CommandType.StoredProcedure };
                    cn.Open();

                    for (int i = 0; i < dsPesquisa.Tables[0].Rows.Count; i++)
                    {
                        string idEquipamento = RETORNO.DATASET(dsPesquisa, i, "idEquipamento");
                        string sEnderecoDestino = RETORNO.DATASET(dsPesquisa, i, "sEnderecoIP");
                        string nTempoConexaoPing = RETORNO.DATASET(dsPesquisa, i, "nTempoConexaoPing");

                        if (sEnderecoDestino == "") sEnderecoDestino = RETORNO.DATASET(dsPesquisa, i, "sHostName");

                        Ping ping = new Ping();
                        PingReply objRespostaPing = ping.Send(sEnderecoDestino, Convert.ToInt32(nTempoConexaoPing));

                        LOG_Ping("Ping para :  " + sEnderecoDestino + " - Status: " + objRespostaPing.Status.ToString() + " - Tempo: " + objRespostaPing.RoundtripTime.ToString());
                        objExecBD.Parameters.AddWithValue("@sFuncao", "REGISTRA_PING");
                        objExecBD.Parameters.AddWithValue("@idEquipamento", idEquipamento);
                        objExecBD.Parameters.AddWithValue("@sStatusPing", objRespostaPing.Status.ToString());
                        objExecBD.Parameters.AddWithValue("@nTempoPing", objRespostaPing.RoundtripTime.ToString());
                        objExecBD.Connection = cn;
                        objExecBD.ExecuteNonQuery();
                        objExecBD.Parameters.Clear();
                    }

                    LOG_Ping("Encerrando Conexão com o Banco de Dados");
                    objExecBD.Dispose();
                    cn.Close();
                    cn.Dispose();
                }
            }
            catch (Exception ex)
            {
                LOG_Ping("Erro:  " + ex.Message);
            }
            finally
            {
                TimeSpan dtTempo = DateTime.Now - dtInicio;
                LOG_Ping("Processo Finalizado - Tempo: " + dtTempo.ToString());
            }

            BeginInvoke((Action)(() => tmrPing.Start()));
        }

        #endregion

        #region | Envio Lista Pagamentos

        private void tmrEnviarPagamentos_Tick(object sender, EventArgs e) => Task.Run(() => Processar_tmrEnviarPagamentos());

        private void Processar_tmrEnviarPagamentos()
        {
            BeginInvoke((Action)(() => tmrEnviarPagamentos.Stop()));

            LOG_Email("Iniciando Processo de Envio de Email de Pagamentos");

            DateTime agora = DateTime.Now;
            DateTime? proximoEnvio = Consultar_Proximo_Envio();

            if (proximoEnvio.HasValue && agora >= proximoEnvio.Value)
            {
                try
                {
                    ExecutarProcedimentoGerarEmail();
                }
                catch (Exception ex)
                {
                    LOG_Email("Email Pagamentos - Erro: " + ex.Message);
                }
            }

            LOG_Email("Processo de Envio de Email de Pagamentos Finalizado");

            BeginInvoke((Action)(() => tmrEnviarPagamentos.Start()));
        }

        private DateTime? Consultar_Proximo_Envio()
        {
            try
            {
                DataSet dsSalvar = ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Pagar", new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_ULTIMO_ENVIO" } });

                if (dsSalvar != null && dsSalvar.Tables[0].Rows.Count > 0) return Convert.ToDateTime(dsSalvar.Tables[0].Rows[0]["dtProximoEnvio"]);
            }
            catch (Exception ex)
            {
                LOG_Ping("Erro: " + ex.Message);
            }

            return null;
        }

        private void ExecutarProcedimentoGerarEmail()
        {
            try
            {
                ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Pagar", new Dictionary<string, string> { { "@sFuncao", "GERAR_EMAIL_LISTAS_APROVACAO" } });
            }
            catch (Exception ex)
            {
                LOG_Ping("Erro:  " + ex.Message);
            }
        }

        #endregion

        #region | Envio de Emails

        //-----------------------------------------------------------------------------------------------
        // 1 - Robo incia o evento
        private void tmrEnviaMensagensPendentes_Tick(object sender, EventArgs e) => Task.Run(() => Processar_tmrEnviaMensagensPendentes());

        private void Processar_tmrEnviaMensagensPendentes()
        {
            BeginInvoke((Action)(() => tmrEnviaMensagensPendentes.Stop()));

            LOG_Email("Consultando Emails a serem Enviados");
            PesquiusarMensagensPendentes();
            LOG_Email("Processo de Envio de Emails Finalizado");

            BeginInvoke((Action)(() => tmrEnviaMensagensPendentes.Start()));
        }

        //-----------------------------------------------------------------------------------------------
        // 2 - Consulta o BD para verificar menssagens pendentes e cria uma lista
        public void PesquiusarMensagensPendentes()
        {
            string idMensagem = "";
            try
            {
                DataSet dsConsultar = ExecutarDataSet("sp_Manipula_tbl_Flow_Mensagens", new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_PENDENTES_ENVIO" } });

                if (ValidarDataSet(dsConsultar, out string sErro))
                {

                    for (int nLinha = 0; nLinha < dsConsultar.Tables[0].Rows.Count; nLinha++)
                    {
                        try
                        {
                            idMensagem = RETORNO.DATASET(dsConsultar, nLinha, "idMensagem");
                            string sAssunto = RETORNO.DATASET(dsConsultar, nLinha, "sAssunto");
                            string sCorpo = RETORNO.DATASET(dsConsultar, nLinha, "sCorpo");
                            string idUsuarioDestino = RETORNO.DATASET(dsConsultar, nLinha, "idUsuarioDestino");
                            string idUsuarioRemetente = RETORNO.DATASET(dsConsultar, nLinha, "idUsuarioRemetente");
                            string dtEnvio = RETORNO.DATASET(dsConsultar, nLinha, "dtEnvio");
                            string idObjeto = RETORNO.DATASET(dsConsultar, nLinha, "idObjeto").ToString().PadLeft(6, '0');
                            string idTipoObjeto = RETORNO.DATASET(dsConsultar, nLinha, "idTipoObjeto");
                            string idPedido = RETORNO.DATASET(dsConsultar, nLinha, "idPedido");

                            LOG_Email("Enviando e-mail : " + sAssunto);

                            GerarEmail(idMensagem, idTipoObjeto);
                        }
                        catch (Exception ex_Loop)
                        {
                            LOG_Email("Erro ao  Enviar email: " + ex_Loop.Message);
                        }
                    }
                }
                else throw new Exception("BD: " + sErro.ToString());
            }
            catch (Exception ex)
            {
                LOG_Email("Erro ao enviar: " + ex.Message);
            }
        }

        //-----------------------------------------------------------------------------------------------
        // 3 - Pega os emails consultados, caso tenham data de envio vazia é gerado o email.
        protected void GerarEmail(string idMensagem, string idTipoObjeto)
        {
            try
            {
                Dictionary<String, String> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE" },
                    { "@idMensagem", idMensagem }
                };
                DataSet dsEmailMensagem = ExecutarDataSet("sp_Manipula_tbl_Flow_Mensagens", vParametros);

                if (ValidarDataSet(dsEmailMensagem, out string sErro))
                {
                    string sErroEnvioEmail = "";
                    string sErroEnvioPagar = "";
                    string sErroEnvioReceber = "";
                    string sErroEnvioCerticado = "";
                    string sErroEmailArquivoSTSO = "";
                    string sErroEnvioCotacao = "";
                    string sErroNFe = "";
                    HashSet<string> emailsProcessados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    for (int i = 0; i < dsEmailMensagem.Tables[1].Rows.Count; i++)
                    {
                        string sNomeDestinatario = RETORNO.DATASET(dsEmailMensagem, 1, i, "sDscUsuario");
                        string sEmail = RETORNO.DATASET(dsEmailMensagem, 1, i, "sEmail");
                        string sEmailCC = RETORNO.DATASET(dsEmailMensagem, 1, i, "sEmailCC");
                        string sEmailCCO = RETORNO.DATASET(dsEmailMensagem, 1, i, "sEmailCCO");
                        string EmailExternoPagar = RETORNO.DATASET(dsEmailMensagem, 0, 0, "EmailExternoPagar");
                        string EmailExternoReceber = RETORNO.DATASET(dsEmailMensagem, 0, 0, "EmailExternoReceber");
                        string EmailExternoCertificado = RETORNO.DATASET(dsEmailMensagem, 0, 0, "EmailCertificado");
                        string EmailArquivoSTSO = RETORNO.DATASET(dsEmailMensagem, 0, 0, "EmailArquivoSTSO");
                        string EmailExternoCotacao = RETORNO.DATASET(dsEmailMensagem, 0, 0, "EmailExternoCotacao");
                        string EmailCopiaArquivoSTSO = RETORNO.DATASET(dsEmailMensagem, 0, 0, "EmailCopiaArquivoSTSO");
                        string sEmailCopia_STSO = RETORNO.DATASET(dsEmailMensagem, 0, 0, "sEmailCopia_STSO");
                        string sEmailNFE = RETORNO.DATASET(dsEmailMensagem, 0, 0, "sEmailNFE");
                        string sEmailCopiaNFE = RETORNO.DATASET(dsEmailMensagem, 0, 0, "sEmailCopiaNFE");
                        string sAnexoNFe = RETORNO.DATASET(dsEmailMensagem, 0, 0, "sAnexoNFe");
                        string sEmailsCopiaDepartamento = RETORNO.DATASET(dsEmailMensagem, 0, 0, "sEmailsCopiaDepartamento");
                        string idSMTP = RETORNO.DATASET(dsEmailMensagem, 0, 0, "idSMTP");
                        int Tempo = 5000;

                        if (!string.IsNullOrWhiteSpace(sEmail) && !emailsProcessados.Add(sEmail.Trim()))
                        {
                            LOG_Email("E-mail duplicado ignorado para a mensagem " + idMensagem + ": " + sEmail);
                            continue;
                        }

                        //sEmail = "hmaestrello@tecandtec.com.br"; //Força a enviar para o meu email Deve ser apagado depois

                        //-----------------Função Enviar email-----------------------
                        // 1º- Enviar email para.
                        // 2º- Enviar com copia para.
                        // 3º- Enviar com copia oculta para.
                        // 4º- Assunto do email.
                        // 5º- Corpo do email.
                        // 6º- Endereço anexo.

                        if (idTipoObjeto == "1")
                        {
                            string nControleTT = RETORNO.DATASET(dsEmailMensagem, 0, 0, "nControleTT");
                            string sAssuntoPedido = RETORNO.DATASET(dsEmailMensagem, 0, 0, "sAssunto");

                            if (RETORNO.DATASET(dsEmailMensagem, 0, 0, "sNotificacaoEmail") == "N")
                                continue;

                            sErroEnvioEmail = FUNCOES.EnviarEmail
                            (
                            sEmail,
                            "",
                            "",
                            sAssuntoPedido,
                            GerarCorpoEmailFluxo(
                            RETORNO.DATASET(dsEmailMensagem, 1, i, "sDscUsuario"),
                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "sDscUsuarioRemetente"),
                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "sCorpo"),
                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "idObjeto").ToString().PadLeft(6, '0'),
                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "idTipoObjeto"),
                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "sReferencia"),
                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "sDscDepartamento"),
                            nControleTT),
                            ""
                            );

                            if (sErroEnvioEmail != "")
                            {
                                vParametros.Clear();

                                vParametros.Add("@sFuncao", "REGISTRA_ERRO");
                                vParametros.Add("@idMensagem", idMensagem);
                                vParametros.Add("@sErro", sErroEnvioEmail);
                                vParametros.Add("@idUsuarioDestino", RETORNO.DATASET(dsEmailMensagem, 1, i, "idUsuarioDestino"));

                                BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Mensagens", vParametros);

                                sErroEnvioEmail = "";
                            }

                        }
                        else if (RETORNO.DATASET(dsEmailMensagem, "sEmailExterno") == "S")
                        {
                            DataSet dsEmailMensagemExterno;
                            Dictionary<String, String> vParametrosExterno = new Dictionary<string, string>();
                            Tempo = 100;

                            vParametrosExterno.Add("@sFuncao", "CONSULTAR_DETALHE");
                            vParametrosExterno.Add("@idMensagem", idMensagem);

                            dsEmailMensagemExterno = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Mensagens", vParametrosExterno);

                            if (RETORNO.DATASET(dsEmailMensagemExterno, "dtEnvio").Length <= 0)
                            {
                                if (Pagar == "S")
                                {
                                    if (idTipoObjeto == "12")
                                    {
                                        sErroEnvioPagar = FUNCOES.EnviarEmail
                                                                    (
                                                                    EmailExternoPagar,
                                                                    "",
                                                                    "",
                                                                    RETORNO.DATASET(dsEmailMensagem, 0, 0, "sAssunto"),
                                                                    GerarCorpoEmail(
                                                                    RETORNO.DATASET(dsEmailMensagem, 0, 0, "sDscUsuarioExternoPagar"),
                                                                    RETORNO.DATASET(dsEmailMensagem, 0, 0, "sDscUsuarioRemetente"),
                                                                    RETORNO.DATASET(dsEmailMensagem, 0, 0, "sCorpo"),
                                                                    RETORNO.DATASET(dsEmailMensagem, 0, 0, "idObjeto").ToString().PadLeft(6, '0'),
                                                                    RETORNO.DATASET(dsEmailMensagem, 0, 0, "idTipoObjeto"),
                                                                    RETORNO.DATASET(dsEmailMensagem, 0, 0, "idMensagem"),
                                                                    "670px"),
                                                                    ""
                                                                    );
                                        if (sErroEnvioPagar == "")
                                        {

                                            Dictionary<String, String> vParametros_Envio = new Dictionary<string, string>
                                            {
                                                { "@sFuncao", "REGISTRAR_ENVIO" },
                                                { "@idMensagem", idMensagem }
                                            };

                                            BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Mensagens", vParametros_Envio);
                                        }
                                    }
                                }
                                if (Receber == "S")
                                {
                                    if (idTipoObjeto == "11")
                                    {
                                        sErroEnvioReceber = FUNCOES.EnviarEmail
                                        (
                                        EmailExternoReceber,
                                        "",
                                        "",
                                        RETORNO.DATASET(dsEmailMensagem, 0, 0, "sAssunto"),
                                        GerarCorpoEmail(
                                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "sDscUsuarioExternoReceber"),
                                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "sDscUsuarioRemetente"),
                                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "sCorpo"),
                                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "idObjeto").ToString().PadLeft(6, '0'),
                                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "idTipoObjeto"),
                                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "idMensagem"),
                                            "670px"),
                                        "");
                                        if (sErroEnvioReceber == "")
                                        {
                                            Dictionary<String, String> vParametros_Envio = new Dictionary<string, string>
                                            {
                                                { "@sFuncao", "REGISTRAR_ENVIO" },
                                                { "@idMensagem", idMensagem }
                                            };

                                            BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Mensagens", vParametros_Envio);
                                        }
                                    }
                                }
                                if (Certificado == "S")
                                {
                                    if (idTipoObjeto == "9")
                                    {
                                        sErroEnvioCerticado = FUNCOES.EnviarEmail
                                                                   (
                                                                   EmailExternoCertificado,
                                                                   "",
                                                                   "",
                                                                   RETORNO.DATASET(dsEmailMensagem, 0, 0, "sAssunto"),
                                                                   GerarCorpoEmailCertificado(
                                                                   RETORNO.DATASET(dsEmailMensagem, 0, 0, "sDscEmailCertificado"),
                                                                   RETORNO.DATASET(dsEmailMensagem, 0, 0, "sDscUsuarioRemetente"),
                                                                   RETORNO.DATASET(dsEmailMensagem, 0, 0, "sCorpo"),
                                                                   RETORNO.DATASET(dsEmailMensagem, 0, 0, "idObjeto").ToString().PadLeft(6, '0'),
                                                                   RETORNO.DATASET(dsEmailMensagem, 0, 0, "idTipoObjeto"),
                                                                   RETORNO.DATASET(dsEmailMensagem, 0, 0, "idMensagem")),
                                                                   ""
                                                                   );
                                        if (sErroEnvioCerticado == "")
                                        {
                                            Dictionary<String, String> vParametros_Envio = new Dictionary<string, string>
                                            {
                                                { "@sFuncao", "REGISTRAR_ENVIO" },
                                                { "@idMensagem", idMensagem }
                                            };

                                            BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Mensagens", vParametros_Envio);
                                        }
                                    }
                                }
                                if (idTipoObjeto == "13")
                                {
                                    sErroEmailArquivoSTSO = FUNCOES.EnviarEmail
                                                                    (
                                                                    EmailArquivoSTSO,
                                                                    EmailCopiaArquivoSTSO,
                                                                    sEmailCopia_STSO,
                                                                    RETORNO.DATASET(dsEmailMensagem, 0, 0, "sAssunto"),
                                                                    GerarCorpoEmailSTSO(
                                                                    "",
                                                                    RETORNO.DATASET(dsEmailMensagem, 0, 0, "sDscUsuarioRemetente"),
                                                                    RETORNO.DATASET(dsEmailMensagem, 0, 0, "sCorpo"),
                                                                    RETORNO.DATASET(dsEmailMensagem, 0, 0, "idObjeto").ToString().PadLeft(6, '0'),
                                                                    RETORNO.DATASET(dsEmailMensagem, 0, 0, "idTipoObjeto"),
                                                                    RETORNO.DATASET(dsEmailMensagem, 0, 0, "idMensagem")),
                                                                    ""
                                                                    );
                                    if (sErroEmailArquivoSTSO != "")
                                    {
                                        FUNCOES.EnviarEmail(
                                                           sEmailCopia_STSO,
                                                           "",
                                                           "",
                                                           RETORNO.DATASET(dsEmailMensagem, 0, 0, "sAssunto"),
                                                           GerarCorpoEmailSTSO(
                                                           RETORNO.DATASET(dsEmailMensagem, 0, 0, "sDscEmailCertificado"),
                                                           RETORNO.DATASET(dsEmailMensagem, 0, 0, "sDscUsuarioRemetente"),
                                                           "Erro ao enviar email: Caixa de correio não disponível." + "<br/> <br/>" + RETORNO.DATASET(dsEmailMensagem, 0, 0, "sCorpo"),
                                                           RETORNO.DATASET(dsEmailMensagem, 0, 0, "idObjeto").ToString().PadLeft(6, '0'),
                                                           RETORNO.DATASET(dsEmailMensagem, 0, 0, "idTipoObjeto"),
                                                           RETORNO.DATASET(dsEmailMensagem, 0, 0, "idMensagem")),
                                                           ""
                                                           );
                                    }

                                    Dictionary<String, String> vParametros_Envio = new Dictionary<string, string>
                                    {
                                        { "@sFuncao", "REGISTRAR_ENVIO" },
                                        { "@idMensagem", idMensagem }
                                    };
                                    BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Mensagens", vParametros_Envio);
                                }
                                if (idTipoObjeto == "16")
                                {
                                    sErroEnvioCotacao = FUNCOES.EnviarEmail
                                                           (
                                                           EmailExternoCotacao,
                                                           "",
                                                           sEmailsCopiaDepartamento,
                                                           RETORNO.DATASET(dsEmailMensagem, 0, 0, "sAssunto"),
                                                           GerarCorpoEmailCotacao(
                                                           RETORNO.DATASET(dsEmailMensagem, 0, 0, "sDscParceiroCotacao"),
                                                           RETORNO.DATASET(dsEmailMensagem, 0, 0, "sDscUsuarioRemetente"),
                                                           RETORNO.DATASET(dsEmailMensagem, 0, 0, "sCorpo"),
                                                           RETORNO.DATASET(dsEmailMensagem, 0, 0, "idTipoObjeto"),
                                                           RETORNO.DATASET(dsEmailMensagem, 0, 0, "idObjeto").ToString().PadLeft(6, '0'),
                                                           RETORNO.DATASET(dsEmailMensagem, 0, 0, "idMensagem"),
                                                           "95%"),
                                                           ""
                                                           );
                                    if (sErroEnvioCotacao == "")
                                    {
                                        Dictionary<String, String> vParametros_Envio = new Dictionary<string, string>
                                        {
                                            { "@sFuncao", "REGISTRAR_ENVIO" },
                                            { "@idMensagem", idMensagem }
                                        };

                                        BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Mensagens", vParametros_Envio);
                                    }
                                }
                                if (idTipoObjeto == "17")
                                {
                                    sErroNFe = FUNCOES.EnviarEmail
                                                           (
                                                           sEmailNFE,
                                                           sEmailCopiaNFE,
                                                           "",
                                                           RETORNO.DATASET(dsEmailMensagem, 0, 0, "sAssunto"),
                                                           GerarCorpoEmail(
                                                           RETORNO.DATASET(dsEmailMensagem, 0, 0, "sDscParceiroNFE"),
                                                           RETORNO.DATASET(dsEmailMensagem, 0, 0, "sDscUsuarioRemetente"),
                                                           RETORNO.DATASET(dsEmailMensagem, 0, 0, "sCorpo"),
                                                           RETORNO.DATASET(dsEmailMensagem, 0, 0, "idTipoObjeto"),
                                                           RETORNO.DATASET(dsEmailMensagem, 0, 0, "idObjeto").ToString().PadLeft(6, '0'),
                                                           RETORNO.DATASET(dsEmailMensagem, 0, 0, "idMensagem"),
                                                           "700px"),
                                                           sAnexoNFe
                                                           );

                                    Dictionary<String, String> vParametros_Envio = new Dictionary<string, string>
                                    {
                                        { "@sFuncao", "REGISTRAR_ENVIO" },
                                        { "@idMensagem", idMensagem }
                                    };

                                    BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Mensagens", vParametros_Envio);
                                }

                            }
                        }
                        else if (idTipoObjeto == "5")
                        {
                            string TamanhoEmail = "";
                            if (idTipoObjeto == "15")
                                TamanhoEmail = "95%";
                            else
                                TamanhoEmail = "670px";

                            sErroEnvioEmail = FUNCOES.EnviarEmail
                            (
                            sEmail,
                            "",
                            "",
                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "sAssunto"),
                            GerarCorpoEmail(
                            RETORNO.DATASET(dsEmailMensagem, 1, i, "sDscUsuario"),
                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "sDscUsuarioRemetente"),
                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "sCorpo"),
                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "idObjeto"),
                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "idTipoObjeto"),
                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "idMensagem"),
                            TamanhoEmail,
                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "sLinkBotao")),
                            ""
                            );
                        }
                        else
                        {
                            string TamanhoEmail = "";
                            if (idTipoObjeto == "15")
                                TamanhoEmail = "95%";
                            else
                                TamanhoEmail = "670px";

                            sErroEnvioEmail = FUNCOES.EnviarEmail
                            (
                                Convert.ToInt32(idSMTP),
                                sEmail,
                            sEmailCC,
                            sEmailCCO,
                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "sAssunto"),
                            GerarCorpoEmail(
                            sNomeDestinatario,
                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "sDscUsuarioRemetente"),
                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "sCorpo"),
                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "idObjeto"),
                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "idTipoObjeto"),
                            RETORNO.DATASET(dsEmailMensagem, 0, 0, "idMensagem"),
                            TamanhoEmail),
                            ""
                            );

                            if (sErroEnvioEmail != "")
                            {
                                vParametros.Clear();

                                vParametros.Add("@sFuncao", "REGISTRA_ERRO");
                                vParametros.Add("@idMensagem", idMensagem);
                                vParametros.Add("@sErro", sErroEnvioEmail);
                                vParametros.Add("@idUsuarioDestino", RETORNO.DATASET(dsEmailMensagem, 1, i, "idUsuarioDestino"));
                                BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Mensagens", vParametros);
                                sErroEnvioEmail = "";
                            }
                        }

                        //pausa de 5 segundos a cada email
                        System.Threading.Thread.Sleep(Tempo);
                    }


                    if (sErroEnvioEmail == "")
                    {

                        Dictionary<String, String> vParametros_Envio = new Dictionary<string, string>
                        {
                            { "@sFuncao", "REGISTRAR_ENVIO" },
                            { "@idMensagem", idMensagem }
                        };

                        BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Mensagens", vParametros_Envio);
                    }
                }
                else
                {
                    //criar no BD Menssagem "Não existe usuario cadastrado com esse email"
                    throw new Exception("Erro ao consultar BD: " + sErro);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar email : " + ex.Message);
            }
        }


        // 4 - Gera o corpo do e-mail
        public string GerarCorpoEmail(string sNomeDestinatario, string sNomeRemetenteEmail, string sCorpoEmail, string idTipoObjeto, string idObjeto, string idMensagem, string TamanhoEmail)
        {
            return GerarCorpoEmail(sNomeDestinatario, sNomeRemetenteEmail, sCorpoEmail, idTipoObjeto, idObjeto, idMensagem, TamanhoEmail, "");
        }
        public string GerarCorpoEmail(string sNomeDestinatario, string sNomeRemetenteEmail, string sCorpoEmail, string idTipoObjeto, string idObjeto, string idMensagem, string TamanhoEmail, string sLink)
        {
            string sLinkBotao = "";

            System.Text.StringBuilder rs = new System.Text.StringBuilder();

            if (string.IsNullOrEmpty(sLink))
            {
                sLinkBotao = ("https://t-flow.tecandtec.com.br/app/Paginas/Mensagem/Mensagens_Detalhe.aspx?id=" + idMensagem);
            }
            else
            {
                sLinkBotao = sLink;
            }

            string sLinkImagem = ("https://login.tecandtec.com.br/app/img/logo_TT.png");



            rs.AppendLine("<table cellspacing=\"0\" border=\"0\" cellpadding=\"0\" width=\"100%\" bgcolor=\"#f2f3f8\"  style=\"@import url(https: //fonts.googleapis.com/css?family=Rubik:300,400,500,700|Open+Sans:300,400,600,700); font-family: 'Open Sans', sans-serif;\">\r\n <tr>");
            rs.AppendLine("<td>");
            rs.AppendLine("<table style=\"background-color: #f2f3f8; max-width: " + TamanhoEmail + "; margin: 0 auto;\" width=\"100%\" border=\"0\"  align=\"center\" cellpadding=\"0\" cellspacing=\"0\">");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"height: 0px;\">&nbsp;</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td>");
            rs.AppendLine("<table width=\"95%\" border=\"0\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" style=\"max-width: " + TamanhoEmail + "; background: #fff; border-radius: 3px; text-align: center; -webkit-box-shadow: 0 6px 18px 0 rgba(0,0,0,.06); -moz-box-shadow: 0 6px 18px 0 rgba(0,0,0,.06); box-shadow: 0 6px 18px 0 rgba(0,0,0,.06);\">");
            rs.AppendLine("<tr>");

            //Distancia do logo para parte de cima
            rs.AppendLine("<td style=\"height: 40px;\"></td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"padding: 0 35px;\">");

            //rs.AppendLine("<img src=\"" + sLinkImagem + "\" style=\"max-height: 200px; max-width: 200px;/>");
            rs.AppendLine("<img src=\"" + sLinkImagem + "\" alt=\"LogoTT\"/>");

            rs.AppendLine("<p>&ensp;</p>");

            rs.AppendLine("<h1 style=\"color: #1e1e2d; font-weight: 500; margin: 0; font-size: 22px; font-family: 'Rubik',sans-serif;\">");

            //Titulo corpo email
            rs.AppendLine("Olá, " + sNomeDestinatario + "!" + "</br> </br>" + " Você recebeu uma nova mensagem de " + sNomeRemetenteEmail + ":");

            rs.AppendLine("</h1>");
            rs.AppendLine("<span style=\"display: inline-block; vertical-align: middle; margin: 29px 0 26px; border-bottom: 1px solid #cecece; width: 100px;\"></span>");
            rs.AppendLine("<p style=\"color: #455056; font-size: 15px; line-height: 24px; text-align: left; margin: 0;\"> ");

            //Texto corpo email
            rs.AppendLine(sCorpoEmail.Replace("\n", "<br/>") + "</br>");

            rs.AppendLine("</p>");
            rs.AppendLine("<a href=\"");

            //Link no botão
            rs.AppendLine(sLinkBotao);

            rs.AppendLine("\" style=\"background: #20e277; text-decoration: none !important; font-weight: 500; margin-top: 35px; color: #fff; text-transform: uppercase; font-size: 14px; padding: 10px 24px; display: inline-block; border-radius: 50px;\">");

            //Texto Botão
            rs.AppendLine("Acessar Mensagem");

            rs.AppendLine("</a>");

            rs.AppendLine("</br>");
            rs.AppendLine("<p style=\"color: #455056; font-size: 15px; line-height: 24px; margin: 0;\">");

            //Link no corpo do email
            //rs.AppendLine(sLinkSite);

            rs.AppendLine("</p>");
            rs.AppendLine("</br>");
            rs.AppendLine("<p>");

            //Agradecimento corpo email
            rs.AppendLine("Obrigado,");
            rs.AppendLine("<br>");
            rs.AppendLine("Equipe Tec and Tec.");

            rs.AppendLine("</p>");

            rs.AppendLine("</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"height: 30px;\">&nbsp;</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("</table>");
            rs.AppendLine("</td>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"height: 20px;\">&nbsp;</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"text-align: center;\">");
            rs.AppendLine("<p>");

            //Texto parte baixo corpo email
            rs.AppendLine("<a href=\"");
            rs.AppendLine("https://www.tecandtec.com.br");
            rs.AppendLine("\"style=\"font-size: 14px; color: rgba(69, 80, 86, 0.7411764705882353); line-height: 18px; margin: 0 0 0;\">www.tecandtec.com.br");

            rs.AppendLine("</p>");
            rs.AppendLine("</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"height: 0px;\">&nbsp;</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("</table>");
            rs.AppendLine("</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("</table>");
            return rs.ToString();
        }

        public string GerarCorpoEmailFluxo(string sNomeDestinatario, string sNomeRemetenteEmail, string sCorpoEmail, string idTipoObjeto, string idObjeto, string sReferencia, string sDscDepartamento, string nControleTT = "")
        {
            System.Text.StringBuilder rs = new System.Text.StringBuilder();
            string sControleTT = string.IsNullOrWhiteSpace(nControleTT) ? "" : ", N° Controle TT " + nControleTT.Trim();

            string sLinkImagem = ("https://login.tecandtec.com.br/app/img/logo_TT.png");
            string sLinkBotao = ("https://t-flow.tecandtec.com.br/app/Paginas/Pedidos_Detalhe.aspx?id=" + idTipoObjeto.ToString().PadLeft(6, '0'));


            rs.AppendLine("<table cellspacing=\"0\" border=\"0\" cellpadding=\"0\" width=\"100%\" bgcolor=\"#f2f3f8\"  style=\"@import url(https: //fonts.googleapis.com/css?family=Rubik:300,400,500,700|Open+Sans:300,400,600,700); font-family: 'Open Sans', sans-serif;\">\r\n <tr>");
            rs.AppendLine("<td>");
            rs.AppendLine("<table style=\"background-color: #f2f3f8; max-width: 670px; margin: 0 auto;\" width=\"100%\" border=\"0\"  align=\"center\" cellpadding=\"0\" cellspacing=\"0\">");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"height: 0px;\">&nbsp;</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td>");
            rs.AppendLine("<table width=\"95%\" border=\"0\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" style=\"max-width: 670px; background: #fff; border-radius: 3px; text-align: center; -webkit-box-shadow: 0 6px 18px 0 rgba(0,0,0,.06); -moz-box-shadow: 0 6px 18px 0 rgba(0,0,0,.06); box-shadow: 0 6px 18px 0 rgba(0,0,0,.06);\">");
            rs.AppendLine("<tr>");

            //Distancia do logo para parte de cima
            rs.AppendLine("<td style=\"height: 40px;\"></td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"padding: 0 35px;\">");

            //rs.AppendLine("<img src=\"" + sLinkImagem + "\" style=\"max-height: 200px; max-width: 200px;/>");
            rs.AppendLine("<img src=\"" + sLinkImagem + "\" alt=\"LogoTT\"/>");

            rs.AppendLine("<p>&ensp;</p>");

            rs.AppendLine("<h1 style=\"color: #1e1e2d; font-weight: 500; margin: 0; font-size: 28px; font-family: 'Rubik',sans-serif;\">");

            //Titulo corpo email

            rs.AppendLine("Olá, " + sNomeDestinatario + "!");

            rs.AppendLine("<br>");
            rs.AppendLine("O pedido Nº " + idTipoObjeto.ToString().PadLeft(6, '0') + ", ");
            rs.AppendLine("<br>");
            rs.AppendLine(" Referência: " + sReferencia + sControleTT + " ");
            rs.AppendLine("<br>");
            rs.AppendLine(" Foi alterado por " + sNomeRemetenteEmail + " e agora esta no departamento " + sDscDepartamento);


            rs.AppendLine("</h1>");
            rs.AppendLine("<span style=\"display: inline-block; vertical-align: middle; margin: 29px 0 26px; border-bottom: 1px solid #cecece; width: 100px;\"></span>");
            rs.AppendLine("<p style=\"color: #455056; font-size: 15px; line-height: 24px; margin: 0;\"> ");

            //Texto corpo email
            rs.AppendLine("Use o botão abaixo para acessar a página de pedidos no sistema para mais detalhes<br>");

            rs.AppendLine("</p>");
            rs.AppendLine("<a href=\"");

            //Link no botão
            rs.AppendLine(sLinkBotao);

            rs.AppendLine("\" style=\"background: #20e277; text-decoration: none !important; font-weight: 500; margin-top: 35px; color: #fff; text-transform: uppercase; font-size: 14px; padding: 10px 24px; display: inline-block; border-radius: 50px;\">");

            //Texto Botão
            rs.AppendLine("Acessar Pedido");

            rs.AppendLine("</a>");

            rs.AppendLine("<br>");
            rs.AppendLine("<p style=\"color: #455056; font-size: 15px; line-height: 24px; margin: 0;\">");

            //Link no corpo do email
            //rs.AppendLine(sLinkSite);

            rs.AppendLine("</p>");
            rs.AppendLine("<br>");
            rs.AppendLine("<p>");

            //Agradecimento corpo email
            rs.AppendLine("Obrigado,");
            rs.AppendLine("<br>");
            rs.AppendLine("Equipe Tec and Tec.");

            rs.AppendLine("</p>");

            rs.AppendLine("</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"height: 30px;\">&nbsp;</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("</table>");
            rs.AppendLine("</td>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"height: 20px;\">&nbsp;</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"text-align: center;\">");
            rs.AppendLine("<p>");

            //Texto parte baixo corpo email
            rs.AppendLine("<a href=\"");
            rs.AppendLine("https://www.tecandtec.com.br");
            rs.AppendLine("\"style=\"font-size: 14px; color: rgba(69, 80, 86, 0.7411764705882353); line-height: 18px; margin: 0 0 0;\">www.tecandtec.com.br");

            rs.AppendLine("</p>");
            rs.AppendLine("</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"height: 0px;\">&nbsp;</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("</table>");
            rs.AppendLine("</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("</table>");
            return rs.ToString();
        }

        public string GerarCorpoEmailCertificado(string sNomeDestinatario, string sNomeRemetenteEmail, string sCorpoEmail, string idTipoObjeto, string idObjeto, string idMensagem)
        {
            System.Text.StringBuilder rs = new System.Text.StringBuilder();

            string sLinkBotaoCerficado = ("https://t-flow.tecandtec.com.br/app/Paginas/Qualidade/Certificados_Detalhe.aspx?id=" + idMensagem);

            string sLinkImagem = ("https://login.tecandtec.com.br/app/img/logo_TT.png");



            rs.AppendLine("<table cellspacing=\"0\" border=\"0\" cellpadding=\"0\" width=\"100%\" bgcolor=\"#f2f3f8\"  style=\"@import url(https: //fonts.googleapis.com/css?family=Rubik:300,400,500,700|Open+Sans:300,400,600,700); font-family: 'Open Sans', sans-serif;\">\r\n <tr>");
            rs.AppendLine("<td>");
            rs.AppendLine("<table style=\"background-color: #f2f3f8; max-width: 670px; margin: 0 auto;\" width=\"100%\" border=\"0\"  align=\"center\" cellpadding=\"0\" cellspacing=\"0\">");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"height: 0px;\">&nbsp;</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td>");
            rs.AppendLine("<table width=\"95%\" border=\"0\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" style=\"max-width: 670px; background: #fff; border-radius: 3px; text-align: center; -webkit-box-shadow: 0 6px 18px 0 rgba(0,0,0,.06); -moz-box-shadow: 0 6px 18px 0 rgba(0,0,0,.06); box-shadow: 0 6px 18px 0 rgba(0,0,0,.06);\">");
            rs.AppendLine("<tr>");

            //Distancia do logo para parte de cima
            rs.AppendLine("<td style=\"height: 40px;\"></td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"padding: 0 35px;\">");

            //rs.AppendLine("<img src=\"" + sLinkImagem + "\" style=\"max-height: 200px; max-width: 200px;/>");
            rs.AppendLine("<img src=\"" + sLinkImagem + "\" alt=\"LogoTT\"/>");

            rs.AppendLine("<p>&ensp;</p>");

            rs.AppendLine("<h1 style=\"color: #1e1e2d; font-weight: 500; margin: 0; font-size: 22px; font-family: 'Rubik',sans-serif;\">");

            //Titulo corpo email
            rs.AppendLine("Olá, " + sNomeDestinatario + "!" + "</br>" + "Você recebeu uma nova mensagem de " + sNomeRemetenteEmail + ":");

            rs.AppendLine("</h1>");
            rs.AppendLine("<span style=\"display: inline-block; vertical-align: middle; margin: 29px 0 26px; border-bottom: 1px solid #cecece; width: 100px;\"></span>");
            rs.AppendLine("<p style=\"color: #455056; font-size: 15px; line-height: 24px; text-align: left; margin: 0;\"> ");

            //Texto corpo email
            rs.AppendLine(sCorpoEmail.Replace("\n", "<br/>") + "</br>");

            rs.AppendLine("</p>");
            rs.AppendLine("<a href=\"");

            //Link no botão
            rs.AppendLine(sLinkBotaoCerficado);

            rs.AppendLine("\" style=\"background: #20e277; text-decoration: none !important; font-weight: 500; margin-top: 35px; color: #fff; text-transform: uppercase; font-size: 14px; padding: 10px 24px; display: inline-block; border-radius: 50px;\">");

            //Texto Botão
            rs.AppendLine("Acessar Certificado");

            rs.AppendLine("</a>");

            rs.AppendLine("</br>");
            rs.AppendLine("<p style=\"color: #455056; font-size: 15px; line-height: 24px; margin: 0;\">");

            //Link no corpo do email
            //rs.AppendLine(sLinkSite);

            rs.AppendLine("</p>");
            rs.AppendLine("</br>");
            rs.AppendLine("<p>");

            //Agradecimento corpo email
            rs.AppendLine("Obrigado,");
            rs.AppendLine("<br>");
            rs.AppendLine("Equipe Tec and Tec.");

            rs.AppendLine("</p>");

            rs.AppendLine("</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"height: 30px;\">&nbsp;</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("</table>");
            rs.AppendLine("</td>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"height: 20px;\">&nbsp;</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"text-align: center;\">");
            rs.AppendLine("<p>");

            //Texto parte baixo corpo email
            rs.AppendLine("<a href=\"");
            rs.AppendLine("https://www.tecandtec.com.br");
            rs.AppendLine("\"style=\"font-size: 14px; color: rgba(69, 80, 86, 0.7411764705882353); line-height: 18px; margin: 0 0 0;\">www.tecandtec.com.br");

            rs.AppendLine("</p>");
            rs.AppendLine("</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"height: 0px;\">&nbsp;</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("</table>");
            rs.AppendLine("</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("</table>");
            return rs.ToString();
        }

        public string GerarCorpoEmailSTSO(string sNomeDestinatario, string sNomeRemetenteEmail, string sCorpoEmail, string idTipoObjeto, string idObjeto, string idMensagem)
        {
            System.Text.StringBuilder rs = new System.Text.StringBuilder();

            string sLinkBotaoCerficado = ("https://t-flow.tecandtec.com.br/app/Paginas/Qualidade/Certificados_Detalhe.aspx?id=" + idMensagem);
            string sLinkImagem = ("https://login.tecandtec.com.br/app/img/logo_TT.png");

            rs.AppendLine("<table cellspacing=\"0\" border=\"0\" cellpadding=\"0\" width=\"100%\" bgcolor=\"#f2f3f8\"  style=\"@import url(https: //fonts.googleapis.com/css?family=Rubik:300,400,500,700|Open+Sans:300,400,600,700); font-family: 'Open Sans', sans-serif;\">\r\n <tr>");
            rs.AppendLine("<td>");
            rs.AppendLine("<table style=\"background-color: #f2f3f8; max-width: 670px; margin: 0 auto;\" width=\"100%\" border=\"0\"  align=\"center\" cellpadding=\"0\" cellspacing=\"0\">");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"height: 0px;\">&nbsp;</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td>");
            rs.AppendLine("<table width=\"95%\" border=\"0\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" style=\"max-width: 670px; background: #fff; border-radius: 3px; text-align: center; -webkit-box-shadow: 0 6px 18px 0 rgba(0,0,0,.06); -moz-box-shadow: 0 6px 18px 0 rgba(0,0,0,.06); box-shadow: 0 6px 18px 0 rgba(0,0,0,.06);\">");
            rs.AppendLine("<tr>");

            //Distancia do logo para parte de cima
            rs.AppendLine("<td style=\"height: 40px;\"></td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"padding: 0 35px;\">");

            //rs.AppendLine("<img src=\"" + sLinkImagem + "\" style=\"max-height: 200px; max-width: 200px;/>");
            rs.AppendLine("<img src=\"" + sLinkImagem + "\" alt=\"LogoTT\"/>");

            rs.AppendLine("<p>&ensp;</p>");

            rs.AppendLine("<h1 style=\"color: #1e1e2d; font-weight: 500; margin: 0; font-size: 22px; font-family: 'Rubik',sans-serif;\">");

            //Texto corpo email
            rs.AppendLine(sCorpoEmail.Replace("\n", "<br/>") + "</br>");

            //Agradecimento corpo email
            rs.AppendLine("Obrigado,");
            rs.AppendLine("<br>");
            rs.AppendLine("Equipe Tec and Tec.");

            rs.AppendLine("</p>");

            rs.AppendLine("</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"height: 30px;\">&nbsp;</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("</table>");
            rs.AppendLine("</td>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"height: 20px;\">&nbsp;</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"text-align: center;\">");
            rs.AppendLine("<p>");

            //Texto parte baixo corpo email
            rs.AppendLine("<a href=\"");
            rs.AppendLine("https://www.tecandtec.com.br");
            rs.AppendLine("\"style=\"font-size: 14px; color: rgba(69, 80, 86, 0.7411764705882353); line-height: 18px; margin: 0 0 0;\">www.tecandtec.com.br");

            rs.AppendLine("</p>");
            rs.AppendLine("</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("<tr>");
            rs.AppendLine("<td style=\"height: 0px;\">&nbsp;</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("</table>");
            rs.AppendLine("</td>");
            rs.AppendLine("</tr>");
            rs.AppendLine("</table>");
            return rs.ToString();
        }

        public string GerarCorpoEmailCotacao(string sNomeDestinatario, string sNomeRemetenteEmail, string sCorpoEmail, string idTipoObjeto, string idObjeto, string idMensagem, string TamanhoEmail)
        {
            System.Text.StringBuilder rs = new System.Text.StringBuilder();

            string sLinkBotao = ("https://t-flow.tecandtec.com.br/app/Paginas/Mensagem/Mensagens_Detalhe.aspx?id=" + idMensagem);

            string sLinkImagem = ("https://login.tecandtec.com.br/app/img/logo_TT.png");


            rs.AppendLine("<div style=\"background-color: #f2f3f8; font-family: 'Open Sans', sans-serif; padding: 20px; text-align: center;\">");
            rs.AppendLine("  <div style=\"max-width: " + TamanhoEmail + "; margin: 0 auto; background: #fff; border-radius: 3px; box-shadow: 0 6px 18px 0 rgba(0,0,0,.06); padding: 30px; text-align: center;\">");

            // Logo
            rs.AppendLine("    <img src='" + sLinkImagem + "' alt='LogoTT' style='max-height: 200px; max-width: 200px;' />");

            // Saudação
            rs.AppendLine("    <h1 style='color: #1e1e2d; font-weight: 500; font-size: 22px; font-family: Rubik, sans-serif; margin: 20px 0;'>");
            rs.AppendLine("      Olá, " + sNomeDestinatario + "!<br>Você recebeu uma nova mensagem de " + sNomeRemetenteEmail + ":");
            rs.AppendLine("    </h1>");

            // Linha divisória
            rs.AppendLine("    <hr style='border: none; border-top: 1px solid #cecece; width: 100px; margin: 20px auto;'>");

            // Corpo do e-mail vindo do banco de dados
            rs.AppendLine("    " + sCorpoEmail);

            // Botão para acessar a mensagem
            rs.AppendLine("    <a href='" + sLinkBotao + "' ");
            rs.AppendLine("       style='background: #20e277; color: #fff; font-size: 14px; font-weight: 500; text-transform: uppercase; padding: 10px 24px; display: inline-block; border-radius: 50px; text-decoration: none; margin-top: 20px;'>");
            rs.AppendLine("      Acessar Mensagem");
            rs.AppendLine("    </a>");

            // Assinatura
            rs.AppendLine("    <p style='color: #455056; font-size: 15px; line-height: 24px; margin-top: 20px;'>");
            rs.AppendLine("      Obrigado,<br>Equipe Tec and Tec.");
            rs.AppendLine("    </p>");
            rs.AppendLine("    <p style='font-size: 14px; color: rgba(69, 80, 86, 0.74); margin-top: 20px;'>");
            rs.AppendLine("      <a href='https://www.tecandtec.com.br' style='color: inherit; text-decoration: none;'>www.tecandtec.com.br</a>");
            rs.AppendLine("    </p>");

            rs.AppendLine("  </div>");
            rs.AppendLine("</div>");
            return rs.ToString();
        }

        #endregion

        #region | ZPL Printer

        private void tmrZplPrinter_Tick(object sender, EventArgs e) => Task.Run(() => Processar_tmrZplPrinter());

        void Processar_tmrZplPrinter()
        {
            BeginInvoke((Action)(() => tmrZplPrinter.Stop()));

            try
            {
                LOG_Impressora("Lendo etiquetas pendentes!");

                DataSet dsEtiquetas = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_WMS_OPI_Etiqueta", new Dictionary<string, string> { { "@sFuncao", "CONSULTAR-IMPRESSAO" } });

                string arqZplProduto = "^XA\r\n\r\n" +
                 "^CI28\r\n" +
                 "^PW800\r\n" + // Largura de 10cm (800 pontos)
                 "^LL560\r\n" + // Altura de 7cm (560 pontos)
                 "^FX Top section with logo, name, and address.\r\n" +
                 "^CF0,30\r\n" +
                 "^FO80,30^GFsLogoTT^FS\r\n" +
                 // Ajustei o logo do cliente para a direita para não sobrepor o logo principal
                 "^FO400,30^FB370,1,0,R^FDsLogoCliente^FS\r\n\r\n" +
                 "^FO80,150^GB700,3,3^FS  ^FX Linha horizontal\r\n\r\n" +
                 "^CF0,30\r\n" +
                 // Textos centralizados com uma margem de 30 pontos de cada lado (800 - 60 = 740 de área útil)
                 "^FO80,165^FB740,1,0,C^FDsCliente^FS  \r\n" +
                 "^FO80,220^FB740,1,0,C^FDsObservacao^FS\r\n" +
                 // Mudei o limite da descrição para 2 linhas (o segundo número) caso o texto seja longo
                 "^FO80,260^FB740,2,0,C^FDsDscEtiqueta^FS  \r\n" +
                 "^FO80,320^FB740,1,0,C^FDnQuantidade^FS\r\n\r\n" +
                 "^FX Barcode section\r\n" +
                 "^BY3,2,90\r\n" + // Reduzi a espessura de 4 para 3 para o código caber na etiqueta
                                   // Eixo X em 160 para centralizar visualmente o código de barras
                 "^FO180,380^BCN,90,Y,N,N^FDsCodigoBarras^FS  " +
                 "^XZ";

                string arqZplEntrada = @"^XA
                                    ^CI28
                                    ^PW800
                                    ^LL160
                                    ^FX --- BLOCO ESQUERDO ---
                                    ^FO65,5^GFA,717,2352,24,:Z64:eJzdlUFr2zAUx5UEsdJA3UN03s697BvYDnR3D6zb8lnmY8hh/Qo9Bh3yAVZovkIL9a0BQy7Fhw52Etmw+iRZsSxLlEF72d+B+D1++kt6erYR0loir77kvuzFVnBPmlBKfbwA/fXkaYD/DrzPP8QH/cuyfAjwb+G/AH3z5N9q/YtyVz5cpk52eiOUfjN2becx1frqTBJpXNSMsbWf7w0wPPj3ZrB5mg35Q83gsvhS616NCvhbE/T8aTrga8WvA3wW8A/x3Y5PlqoflsWqlrIqsdjtdo9oeG6mfy7kBFbecKS/4SN/xjZsU1j+bf9gh0+Ab+D/VPpbvPF3+W3Lnz2DLJ7I8gf8BXCnP0E23xbS5T9KvlmvfoD9ZtW9KbBVT7tCkVX/4QkYvjuxSds/Wk/H/Ih2/dM7ga3fX29g6K8XdBjyI//69QQef7UDjz8aH/uf1TYPA47rt3nonRvjf41cufU3cs/3Nd7tn1f96+f6Pf3fnZf19/N56vf/5EkHdfIv8P+gcXfbKzjpQvUiMYoK0bQhLVEKl/wpXn6H5f2k+synDZr+2f+qEgFhwqcHyI9SmhMKT/CcZmCOdSh5HsdXRXy1f+I8KaKK38ZRoXg6Q2g2mqdZhhGe01w9wJPqwOIxiif7ivPIhJLPzglGBEuegH92PiOKv20iUUQCeNjLh6rhSaP8yR3O4SMGPJHrgVDu4QWXXdoh:43FB
                                    ^FX Melhorei o tamanho da fonte e desci para desgrudar da logo
                                    ^FO65,105^A0N,20,20^FD^FS
                                    ^FO65,130^A0N,24,24^FDsCodigo^FS
                                    ^FX --- BLOCO DIREITO: 1. Titulo e Linha ---
                                    ^FO300,15^A0N,22,22^FDsDscEtiqueta^FS
                                    ^FO300,45^GB480,2,2^FS
                                    ^FX --- BLOCO DIREITO: 2. Lote/Serie e EAN ---
                                    ^FO300,55^A0N,18,18^FD sLoteSerie^FS
                                    ^FO690,55^A0N,18,18^FDsEAN^FS
                                    ^FX --- BLOCO DIREITO: 3. Codigo de Barras ---
                                    ^FO300,85^BY2,2,35^BCN,,Y,N^FD>:sCodigoBarras>70000^FS
                                    ^XZ";
                if (ValidarDataSet(dsEtiquetas))
                {
                    LOG_Impressora(dsEtiquetas.Tables[0].Rows.Count.ToString() + " Na fila de impressão");

                    foreach (DataRow row in dsEtiquetas.Tables[0].Rows)
                    {
                        string arqZplComDados = "";
                        var idTipo = row["idTipoEtiqueta"].ToString();

                        if (idTipo == "2") arqZplComDados = PreencherZplComDados(arqZplEntrada, row);
                        else arqZplComDados = PreencherZplComDados(arqZplProduto, row);

                        if (row["sAtivo"].ToString() == "S")
                            ImprimirEtiqueta(arqZplComDados, row["idEtiqueta"].ToString(), row["sIP"].ToString(), row["nPort"].ToString());
                        else
                            LOG_Impressora($"A Impressora de IP: {row["sIP"].ToString()} e Porta: {row["nPort"].ToString()} está pausada para impressão.");
                    }

                    LOG_Impressora("Impressão finalizada!");

                }
                else
                    LOG_Impressora("Nenhuma Etiqueta Encontrada!");
            }
            catch (Exception ex)
            {
                LOG_Impressora("Erro:" + ex.Message);
            }
            finally
            {
                LOG_Impressora("Finalizando processo de Impressão de Etiqueta");
            }

            BeginInvoke((Action)(() => tmrZplPrinter.Start()));
        }



        string ResumirString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        //string PreencherZplComDados(string arqZpl, DataRow row)
        //{
        //    var sLogoTT = row["sLogoTT"] != DBNull.Value ? Convert.ToBase64String((byte[])row["sLogoTT"]) : "";

        //    var sLogoCliente = row["sLogoCliente"] != DBNull.Value ? Convert.ToBase64String((byte[])row["sLogoCliente"]) : "";

        //    string nQuantidade = row["nQuantidade"] != null && row["nQuantidade"].ToString() != "0" ? "QTD: " + row["nQuantidade"].ToString() : "";
        //    string sObservacao = row["sObservacao"] != null && row["sObservacao"].ToString() != "Gerado Automático" && row["sObservacao"].ToString() != "Sem Observação" ? row["sObservacao"].ToString() : "";
        //    string sDscEtiqueta = ResumirString(row["sDscEtiqueta"]?.ToString(), maxLabelLength);
        //    string sCliente = ResumirString(row["sCliente"]?.ToString(), maxLabelLength);
        //    string sCodigo = row["sCodigo"] != null ? row["sCodigo"].ToString() : "";
        //    string sLoteSerie = row["sLoteSerie"] != null ? row["sLoteSerie"].ToString() : "";
        //    string sEAN = row["sEAN"] != null ? row["sEAN"].ToString() : "";

        //    sObservacao = ResumirString(sObservacao, maxLabelLength);

        //    string zplComDados = arqZpl.Replace("sDscEtiqueta", sDscEtiqueta?.ToString() ?? "")
        //                          .Replace("nQuantidade", nQuantidade)
        //                          .Replace("sCodigoBarras", row["sCodigoBarras"]?.ToString() ?? "")
        //                          .Replace("sObservacao", sObservacao)
        //                          .Replace("sLogoTT", sLogoTT != "" ? ConverterImagemBase64ParaZPL(sLogoTT, 275, 100) : "")
        //                          .Replace("sLogoCliente", sLogoCliente != "" ? ConverterImagemBase64ParaZPL(sLogoCliente, 275, 100) : "")
        //                          .Replace("sCliente", row["sReferencia"] + " - " + sCliente?.ToString() ?? "")
        //                          .Replace("sCodigo", sCodigo)
        //                          .Replace("sLoteSerie", sLoteSerie)
        //                          .Replace("sEAN", sEAN);

        //    return zplComDados;
        //}
        string PreencherZplComDados(string arqZpl, DataRow row)
        {
            var sLogoTT = row["sLogoTT"] != DBNull.Value ? Convert.ToBase64String((byte[])row["sLogoTT"]) : "";

            var sLogoCliente = row["sLogoCliente"] != DBNull.Value ? Convert.ToBase64String((byte[])row["sLogoCliente"]) : "";

            string nQuantidade = row["nQuantidade"] != null && row["nQuantidade"].ToString() != "0" ? "QTD: " + row["nQuantidade"].ToString() : "";
            string sObservacao = row["sObservacao"] != null && row["sObservacao"].ToString() != "Gerado Automático" && row["sObservacao"].ToString() != "Sem Observação" ? row["sObservacao"].ToString() : "";
            string sDscEtiqueta = ResumirString(row["sDscEtiqueta"]?.ToString(), maxLabelLength);
            string sCliente = ResumirString(row["sCliente"]?.ToString(), maxLabelLength);
            string sCodigo = row["sCodigo"] != null ? row["sCodigo"].ToString() : "N/A";
            string sLoteSerie = row["sLoteSerie"] != null ? row["sLoteSerie"].ToString() : "N/A";

            string valorEAN = row["sEAN"]?.ToString();
            string sEAN = !string.IsNullOrWhiteSpace(valorEAN) && valorEAN != "N/A" ? "EAN: " + valorEAN : "";

            sObservacao = ResumirString(sObservacao, maxLabelLength);

            string zplComDados = arqZpl.Replace("sDscEtiqueta", sDscEtiqueta?.ToString() ?? "")
                                  .Replace("nQuantidade", nQuantidade)
                                  .Replace("sCodigoBarras", row["sCodigoBarras"]?.ToString() ?? "")
                                  .Replace("sObservacao", sObservacao)
                                  .Replace("sLogoTT", sLogoTT != "" ? ConverterImagemBase64ParaZPL(sLogoTT, 275, 100) : "")
                                  .Replace("sLogoCliente", sLogoCliente != "" ? ConverterImagemBase64ParaZPL(sLogoCliente, 275, 100) : "")
                                  .Replace("sCliente", row["sReferencia"] + " - " + sCliente?.ToString() ?? "")
                                  .Replace("sCodigo", sCodigo)
                                  .Replace("sLoteSerie", sLoteSerie)
                                  .Replace("sEAN", sEAN);

            return zplComDados;
        }
        static string ConverterImagemBase64ParaZPL(string imagemBase64, int novaLargura, int novaAltura)
        {
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(imagemBase64)))
            {
                using (Bitmap bitmap = new Bitmap(ms))
                {
                    Bitmap bitmapMonocromatico = ConverterParaMonocromatico(RedimensionarImagem(bitmap, novaLargura, novaAltura));

                    byte[] dadosImagemZPL = ObterBytesImagem(bitmapMonocromatico);

                    StringBuilder comandosZPL = new StringBuilder();
                    comandosZPL.AppendLine($"^GFA,{dadosImagemZPL.Length},{dadosImagemZPL.Length},{bitmapMonocromatico.Width / 8},");
                    comandosZPL.Append(BitConverter.ToString(dadosImagemZPL).Replace("-", ""));

                    return comandosZPL.ToString();
                }
            }
        }

        static Bitmap RedimensionarImagem(Bitmap imagemOriginal, int largura, int altura)
        {
            Bitmap novaImagem = new Bitmap(largura, altura);
            using (Graphics g = Graphics.FromImage(novaImagem))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(imagemOriginal, 0, 0, largura, altura);
            }
            return novaImagem;
        }

        static Bitmap ConverterParaMonocromatico(Bitmap original)
        {
            int largura = original.Width;
            int altura = original.Height;

            // Criar um bitmap de 24 bits para evitar problemas de índice
            Bitmap naoIndexado = new Bitmap(largura, altura, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(naoIndexado))
            {
                g.DrawImage(original, new System.Drawing.Rectangle(0, 0, largura, altura));
            }

            // Criar um bitmap monocromático
            Bitmap monocromatico = new Bitmap(largura, altura, PixelFormat.Format1bppIndexed);

            // Bloquear os bits do bitmap
            BitmapData dados = monocromatico.LockBits(new System.Drawing.Rectangle(0, 0, largura, altura), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);

            byte[] dadosBitmap = new byte[dados.Stride * altura];
            for (int y = 0; y < altura; y++)
            {
                for (int x = 0; x < largura; x++)
                {
                    // Obter a cor do pixel
                    Color cor = naoIndexado.GetPixel(x, y);

                    // Converter para um valor de cinza
                    int intensidadeCinza = (int)(cor.R * 0.3 + cor.G * 0.59 + cor.B * 0.11);

                    // Converter para preto ou branco
                    if (intensidadeCinza < 128)
                    {
                        int bytePos = x / 8;
                        int bitPos = 7 - (x % 8);
                        dadosBitmap[y * dados.Stride + bytePos] |= (byte)(1 << bitPos);
                    }
                }
            }

            // Copiar os dados para o bitmap
            System.Runtime.InteropServices.Marshal.Copy(dadosBitmap, 0, dados.Scan0, dadosBitmap.Length);
            monocromatico.UnlockBits(dados);

            return monocromatico;
        }

        static byte[] ObterBytesImagem(Bitmap bitmap)
        {
            int largura = bitmap.Width;
            int altura = bitmap.Height;
            int stride = largura / 8;
            byte[] bytesImagem = new byte[altura * stride];

            BitmapData bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, largura, altura), ImageLockMode.ReadOnly, PixelFormat.Format1bppIndexed);
            IntPtr scan0 = bitmapData.Scan0;

            for (int y = 0; y < altura; y++)
            {
                System.Runtime.InteropServices.Marshal.Copy(scan0 + y * bitmapData.Stride, bytesImagem, y * stride, stride);
            }

            bitmap.UnlockBits(bitmapData);
            return bytesImagem;
        }

        void ImprimirEtiqueta(string zplContent, string idEtiqueta, string sIP, string nPort)
        {
            var appSettings = ConfigurationManager.AppSettings;
            string zplIP = sIP ?? "192.168.44.238";

            if (nPort == "0" || string.IsNullOrEmpty(nPort))
                nPort = "6101";

            int zplPort = Convert.ToInt32(nPort);

            //Obtém o IP e Porta do Arquivo de Configuração
            if (string.IsNullOrEmpty(zplIP) && zplPort != 0)
            {
                if (!string.IsNullOrEmpty(appSettings["zplPort"]) && (appSettings["zplPort"] != "0" || !string.IsNullOrEmpty(appSettings["zplPort"])))
                {
                    zplIP = appSettings["zplIP"];
                    zplPort = Convert.ToInt32(appSettings["zplPort"]);
                }
            }

            TcpClient tcpClient = new TcpClient();

            try
            {
                // Define um tempo limite de 5 segundos para a conexão
                var result = tcpClient.BeginConnect(zplIP, zplPort, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5));

                if (!success)
                {

                    LOG_Impressora($"IdEtiqueta: {idEtiqueta} - Falha ao conectar no servidor ZPL, Impressora de Endereço: {sIP}:{nPort} está desconectada da Rede.");
                    throw new Exception($"Falha ao conectar no servidor ZPL, Impressora de Endereço: {sIP}:{nPort} está desconectada da Rede.");
                }
                else
                {
                    // Certifica de que a conexão foi estabelecida
                    tcpClient.EndConnect(result);

                    //Console.WriteLine("Conectado ao servidor de teste!");
                    InserirDataImpresso(idEtiqueta);
                    // Envia Conteúdo do arquivo ZPL
                    StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream());
                    streamWriter.Write(zplContent);
                    streamWriter.Flush();

                    // Lê a resposta do servidor
                    StreamReader streamReader = new StreamReader(tcpClient.GetStream());

                    tcpClient.Close();
                }
            }
            catch (Exception ex)
            {
                LOG_Impressora("Erro Impress: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        void InserirDataImpresso(string idEtiqueta)
        {
            DataSet dsEtiquetas;
            Dictionary<String, String> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "INSERIR-DATA-IMPRESSAO" },
                { "@idEtiqueta", idEtiqueta }
            };
            dsEtiquetas = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_WMS_OPI_Etiqueta", vParametros);

            if (BD.ValidarDataSet(dsEtiquetas))
            {
                Console.WriteLine("Data Setada Com Sucesso!");
            }
            else
            {
                Console.WriteLine("Nenhuma Data Foi Setada!");
            }
        }

        #endregion

        #region | Importar XML NFe

        private void tmrImportaXML_Tick(object sender, EventArgs e) => Task.Run(() => Processar_tmrImportarXML());

        private void Processar_tmrImportarXML()
        {
            BeginInvoke((Action)(() => tmrImportarXML.Stop()));

            try
            {

                LOG_Status(string.Format("Verificando arquivos XML no Diretório: {0}{1}", ImportarXML_Path, "/XML_NFe"));

                string[] arquivos = Directory.GetFiles(string.Format("{0}{1}", ImportarXML_Path, "/XML_NFe"));

                if (arquivos.Length < 1) LOG_Status("Nenhum arquivo XML encontrado");

                foreach (string xml in arquivos)
                {
                    string sNomeArquivo = "";

                    Dictionary<string, string> vParametros_ImportarXML = new Dictionary<string, string>();

                    try
                    {
                        sNomeArquivo = Path.GetFileName(xml);

                        LOG_Status("Importando Arquivo XML " + sNomeArquivo);
                        XDocument NFe = XDocument.Load(xml);

                        XElement infNFe = NFe.Root;
                        var chNFe = "";
                        var nNumeroNF = "";
                        var cnpj = "";

                        if (infNFe.Name.LocalName.Equals("NFe"))
                        {
                            infNFe = NFe.Root.FirstNode as XElement;
                            chNFe = infNFe.FirstAttribute.Value.Replace("NFe", "");

                            foreach (char c in chNFe)
                            {
                                if (!char.IsDigit(c))
                                {
                                    throw new Exception();
                                }
                            }

                            nNumeroNF = ((infNFe.FirstNode as XElement).Element("nNF")).Value;
                            cnpj = ((infNFe.FirstNode.NextNode as XElement).FirstNode as XElement).Value;

                            vParametros_ImportarXML.Add("@idTipoObjeto", "-1"); // Nota Não Utilizada
                        }
                        else if (infNFe.Name.LocalName.Equals("nfeProc"))
                        {
                            chNFe = ((infNFe.FirstNode as XElement).FirstNode as XElement).LastAttribute.Value.Replace("NFe", "");

                            foreach (char c in chNFe)
                            {
                                if (!char.IsDigit(c))
                                {
                                    throw new Exception();
                                }
                            }

                            nNumeroNF = (((infNFe.FirstNode as XElement).FirstNode as XElement).FirstNode as XElement).Elements().Where(x => x.Name.LocalName == "nNF").First().Value;
                            cnpj = (((infNFe.FirstNode as XElement).FirstNode as XElement).FirstNode.NextNode as XElement).Elements().Where(x => x.Name.LocalName == "CNPJ").First().Value;

                            vParametros_ImportarXML.Add("@idTipoObjeto", "-1"); // Nota Não Utilizada
                        }
                        else
                        {
                            chNFe = ((infNFe.FirstNode as XElement).FirstNode as XElement).Elements().Where(x => x.Name.LocalName == "chNFe").First().Value;

                            foreach (char c in chNFe)
                            {
                                if (!char.IsDigit(c))
                                {
                                    throw new Exception();
                                }
                            }

                            nNumeroNF = "0";
                            cnpj = ((infNFe.FirstNode as XElement).FirstNode as XElement).Elements().Where(x => x.Name.LocalName == "CNPJ").First().Value;

                            vParametros_ImportarXML.Add("@idTipoObjeto", "99"); // Nota de Cancelamento
                        }

                        vParametros_ImportarXML.Add("@sFuncao", "IMPORTAR_XML");
                        vParametros_ImportarXML.Add("@sChaveNFe", chNFe);
                        vParametros_ImportarXML.Add("@sCNPJ_Emitente", cnpj);
                        vParametros_ImportarXML.Add("@nNumeroNF", nNumeroNF);
                        vParametros_ImportarXML.Add("@sXML", NFe.Root.ToString());

                        DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametros_ImportarXML);

                        Directory.CreateDirectory(string.Format("{0}{1}", ImportarXML_Path, "/XML_NFe/Importados/") + DateTime.Now.Year + "/" + DateTime.Now.Month + "-" + DateTime.Now.Day);
                        File.Move(xml, string.Format("{0}{1}", ImportarXML_Path, "/XML_NFe/Importados/") + DateTime.Now.Year + "/" + DateTime.Now.Month + "-" + DateTime.Now.Day + "/" + sNomeArquivo);

                        LOG_Status("Importação concluída com sucesso");
                    }
                    catch
                    {
                        Directory.CreateDirectory(string.Format("{0}{1}", ImportarXML_Path, "/XML_NFe/Erro_Importacao/"));
                        File.Move(xml, string.Format("{0}{1}", ImportarXML_Path, "/XML_NFe/Erro_Importacao/") + sNomeArquivo);
                        LOG_Status("Erro de Importação XML " + sNomeArquivo);
                    }
                }

                DataSet dsCancelamento = ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe");
                string sErro = "";

                try
                {
                    sErro = RETORNO.DATASET(dsCancelamento, "sErro");
                }
                catch { }

                if (sErro != "" && sErro != null) LOG_Status(sErro);

                LOG_Status("Processo de Importação XML Finalizado");
            }
            catch (Exception ex)
            {
                LOG_Status("Erro Geral: " + ex.Message);
                LOG_Status("Processo de Importação XML Interrompido");
            }

            BeginInvoke((Action)(() => tmrImportarXML.Start()));
        }

        #endregion

        #region | Gerar Emails - entre 7h e 8h

        private void tmrEnviaEmails_AsSete_Tick(object sender, EventArgs e) => Task.Run(() => Processar_tmrEnviaEmails_AsSete());

        private void Processar_tmrEnviaEmails_AsSete()
        {
            BeginInvoke((Action)(() => tmrEnviaEmails_AsSete.Stop()));

            if (!string.IsNullOrEmpty(vParametros_Timers["tmrEnviaEmails_AsSete"]))
            {
                LOG_Email("Iniciando Processo de geração de Emails");

                DateTime agora = DateTime.Now;
                DateTime envioAsSete = new DateTime(agora.Year, agora.Month, agora.Day, 7, 0, 0);

                if (agora >= envioAsSete && agora <= envioAsSete.AddHours(1))
                {
                    int[] envios = vParametros_Timers["tmrEnviaEmails_AsSete"].Split(',').Select(x => Convert.ToInt32(x)).ToArray();

                    // Follow Up de CRM
                    if (envios.Contains(1))
                    {
                        try
                        {
                            if (ProximoEnvio_CRM().HasValue) GerarEmail_CRM();
                            LOG_Email("Email do Follow Up de CRM enviado com sucesso!");
                        }
                        catch (Exception ex)
                        {
                            LOG_Email("Erro ao enviar o Email do Follow Up de CRM:  " + ex.Message);
                        }
                    }

                    // Certificados
                    if (envios.Contains(2))
                    {
                        try
                        {
                            if (ProximoEnvio_Certificado().HasValue) GerarEmail_Certificado();
                            LOG_Email("Email de Certificados enviado com sucesso!");
                        }
                        catch (Exception ex)
                        {
                            LOG_Email("Erro ao enviar o Email de Certificados:  " + ex.Message);
                        }
                    }

                    // Contas a Receber
                    if (envios.Contains(3))
                    {
                        try
                        {
                            DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Receber", new Dictionary<string, string> { { "@sFuncao", "CONSULTA_ENVIO" } });
                            if (ds.Tables.Count > 0) { Receber = "S"; LOG_Email("Email de Contas a Receber enviado com sucesso!"); }
                        }
                        catch (Exception ex)
                        {
                            LOG_Email("Erro ao enviar o Email de Contas a Receber:  " + ex.Message);
                        }
                    }

                    // Contas a Pagar
                    if (envios.Contains(4))
                    {
                        try
                        {
                            DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Pagar", new Dictionary<string, string> { { "@sFuncao", "CONSULTA_ENVIO" } });
                            if (ds.Tables.Count > 0) { Pagar = "S"; LOG_Email("Email de Contas a Pagar enviado com sucesso!"); }
                        }
                        catch (Exception ex)
                        {
                            LOG_Email("Erro ao enviar o Email de Contas a Pagar:  " + ex.Message);
                        }
                    }

                    // Entrega de EPI
                    if (envios.Contains(5))
                    {
                        try
                        {
                            ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_Entrega_EPI", new Dictionary<string, string> { { "@sFuncao", "ENVIAR_EMAIL" } });
                            LOG_Email("Email de Entrega de EPI enviado com sucesso!");
                        }
                        catch (Exception ex)
                        {
                            LOG_Email("Erro ao enviar o Email de Entrega de EPI:  " + ex.Message);
                        }
                    }

                    // NR e ASO
                    if (envios.Contains(6))
                    {
                        try
                        {
                            DateTime hoje = agora.Date;
                            DateTime? ultimaDataExecucao = ultimaExecucaoNR?.Date;

                            if (ultimaDataExecucao != hoje)
                            {
                                ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_Entrega_EPI", new Dictionary<string, string> { { "@sFuncao", "ENVIAR_EMAIL_NR" } });

                                ultimaExecucaoNR = agora;

                                LOG_Email("Email de NR e ASO enviado com sucesso!");
                            }
                        }
                        catch (Exception ex)
                        {
                            LOG_Email("Erro ao enviar o Email de NR e ASO:  " + ex.Message);
                        }
                    }

                    // Faturamento
                    if (envios.Contains(7))
                    {
                        try
                        {
                            ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", new Dictionary<string, string> { { "@sFuncao", "EMAIL_FATURAMENTO" } });
                            LOG_Email("Email de Faturamento enviado com sucesso!");
                        }
                        catch (Exception ex)
                        {
                            LOG_Email("Erro ao enviar o Email de Faturamento:  " + ex.Message);
                        }
                    }

                    // Medição de Pedidos
                    if (envios.Contains(8))
                    {
                        try
                        {
                            ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", new Dictionary<string, string> { { "@sFuncao", "EMAIL_MEDICAO" } });
                            LOG_Email("Email da Medição de Pedidos enviado com sucesso!");
                        }
                        catch (Exception ex)
                        {
                            LOG_Email("Erro ao enviar o Email da Medição de Pedidos:  " + ex.Message);
                        }
                    }

                    // Tabela de Preços
                    if (envios.Contains(9))
                    {
                        try
                        {
                            ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_TabelaPreco", new Dictionary<string, string> { { "@sFuncao", "ENVIAR_EMAIL_ITENS_ALTERADOS" } });
                            LOG_Email("Email de Tabelas de Preços enviado com sucesso!");
                        }
                        catch (Exception ex)
                        {
                            LOG_Email("Erro ao enviar o Email de Tabelas de Preços:  " + ex.Message);
                        }
                    }
                }

                LOG_Email("Processo de geração de Emails Finalizado!");
            }

            BeginInvoke((Action)(() => tmrEnviaEmails_AsSete.Start()));
        }

        private DateTime? ProximoEnvio_CRM()
        {
            try
            {
                DataSet dsSalvar = ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_CRM", new Dictionary<string, string> { { "@sFuncao", "CONSULTA_ENVIO" } });
                if (dsSalvar != null && dsSalvar.Tables[0].Rows.Count > 0) return Convert.ToDateTime(dsSalvar.Tables[0].Rows[0]["dtProximoContato"]);
            }
            catch (Exception ex)
            {
                LOG_Ping("Erro: " + ex.Message);
            }

            return null;
        }

        private DateTime? ProximoEnvio_Certificado()
        {
            try
            {
                DataSet dsSalvar = ExecutarDataSet("sp_Manipula_tbl_Flow_Certificados", new Dictionary<string, string> { { "@sFuncao", "CONSULTA_ENVIO" } });
                if (dsSalvar != null && dsSalvar.Tables[0].Rows.Count > 0) { Certificado = "S"; return Convert.ToDateTime(dsSalvar.Tables[0].Rows[0]["dtValidade"]); }
            }
            catch (Exception ex)
            {
                LOG_Ping("Erro: " + ex.Message);
            }

            return null;
        }

        private void GerarEmail_CRM()
        {
            try
            {
                ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_CRM", new Dictionary<string, string> { { "@sFuncao", "GERAR_EMAIL" } });
            }
            catch (Exception ex)
            {
                LOG_Ping("Erro:  " + ex.Message);
            }
        }

        private void GerarEmail_Certificado()
        {
            try
            {
                ExecutarDataSet("sp_Manipula_tbl_Flow_Certificados", new Dictionary<string, string> { { "@sFuncao", "GERAR_EMAIL" } });
            }
            catch (Exception ex)
            {
                LOG_Ping("Erro:  " + ex.Message);
            }
        }

        #endregion

        #region | Gerenciador NF-e - Unimake UniNFe

        private void tmrGerenciadorNFe_Tick(object sender, EventArgs e) => Task.Run(() => Processar_tmrGerenciadorNFe());

        private void Processar_tmrGerenciadorNFe()
        {
            BeginInvoke((Action)(() => tmrGerenciadorNFe.Stop()));
            LOG_Status("Iniciando Processo de Gerenciamento de NFe/NFSe.");

            try
            {
                VerificaArquivos(RETORNO.DATASET(ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", new Dictionary<string, string> { { "@sFuncao", "CONSULTAR" } }), "sCaminho_UniNFe"));
            }
            catch (Exception ex)
            {
                LOG_NFE($"Erro: {ex.Message}");
            }

            LOG_Status("Processo de Gerenciamento de NFe/NFSe finalizado");
            BeginInvoke((Action)(() => tmrGerenciadorNFe.Start()));
        }

        void VerificaArquivos(string sCaminho_UniNFe)
        {
            LOG_NFE("[NFe | NFSe] - Iniciando processo de verificação de Arquivos");

            string sProcedure = "sp_Manipula_tbl_Flow_XML_NFe";

            foreach (var cnpjDir in Directory.GetDirectories(sCaminho_UniNFe))
            {
                string dirName = Path.GetFileName(cnpjDir);

                if (Regex.IsMatch(dirName, @"\d{14}"))
                {
                    LOG_NFE($"Validando diretório de CNPJ: {dirName}");

                    // NFe - Retorno
                    {
                        string pasta_Retorno = Path.Combine(cnpjDir, "Retorno");

                        if (Directory.Exists(pasta_Retorno))
                        {
                            string[] arquivos = Directory.GetFiles(pasta_Retorno);
                            string nomeUltimaPasta = Path.GetFileName(pasta_Retorno);

                            if (arquivos.Length > 0)
                            {
                                LOG_NFE($"Verificando arquivos da pasta: {pasta_Retorno}!");

                                foreach (var arquivo in arquivos)
                                {
                                    string NomeArquivo = Path.GetFileName(arquivo);

                                    //LOG_NFE("[NFe] - Verificando Arquivo: " + NomeArquivo.ToString());

                                    if (NomeArquivo.Contains("-pro-rec") || NomeArquivo.Contains("-ret-env-canc") || NomeArquivo.Contains("-eve"))
                                    {
                                        try
                                        {
                                            // Carregar o XML do arquivo de motivo
                                            XmlDocument doc = new XmlDocument();

                                            doc.Load(arquivo);

                                            // Definir o namespace usado no XML
                                            XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
                                            nsManager.AddNamespace("nfe", "http://www.portalfiscal.inf.br/nfe");
                                            XmlNode xMLInfProt = doc.SelectSingleNode("//nfe:protNFe/nfe:infProt", nsManager);
                                            XmlNode snProt = doc.SelectSingleNode("//nfe:protNFe/nfe:infProt/nfe:nProt", nsManager);

                                            if (xMLInfProt == null && (NomeArquivo.Contains("-ret-env-canc") || NomeArquivo.Contains("-eve"))) xMLInfProt = doc.SelectSingleNode("//nfe:retEvento /nfe:infEvento", nsManager);

                                            if (xMLInfProt != null)
                                            {
                                                Dictionary<string, string> vParametros = new Dictionary<string, string>
                                                {
                                                    { "@sFuncao", "ALTERAR_STATUS_NFE" },
                                                    { "@sChaveNFe", xMLInfProt["chNFe"].InnerText },
                                                    { "@tpAmb",     xMLInfProt["tpAmb"].InnerText },
                                                    { "@sCStat",    xMLInfProt["cStat"].InnerText },
                                                    { "@sxMotivo",  xMLInfProt["xMotivo"].InnerText }
                                                };

                                                if (xMLInfProt.InnerText.Contains("cOrgao")) vParametros.Add("@cOrgao", xMLInfProt["cOrgao"].InnerText);
                                                if (xMLInfProt.InnerText.Contains("nSeqEvento")) vParametros.Add("@nSeqEvento", xMLInfProt["nSeqEvento"].InnerText);
                                                if (xMLInfProt.InnerText.Contains("nProt")) vParametros.Add("@snProt", xMLInfProt["nProt"].InnerText);

                                                ExecutarDataSet(sProcedure, vParametros);

                                                LOG_NFE("NFE: " + xMLInfProt["chNFe"].InnerText + " - " + xMLInfProt["cStat"].InnerText + " - " + xMLInfProt["xMotivo"].InnerText);
                                                MoveArquivo_Windows(arquivo.ToString(), pasta_Retorno + @"\T-FLOW\UTILIZADA\", NomeArquivo.Trim());
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            LOG_NFE("NFE: " + NomeArquivo.Replace("-num-lot.xml", "") + " - Erro: " + ex.Message);
                                        }
                                    }
                                    else if (NomeArquivo.Contains(".err"))
                                    {
                                        if (!NomeArquivo.Contains("110110") && !NomeArquivo.Contains("110111"))
                                        {
                                            string sMotivo = ExtrairMensagemDoArquivoErr(arquivo);

                                            Dictionary<string, string> vParametros = new Dictionary<string, string>
                                            {
                                                { "@sFuncao", "ALTERAR_STATUS_NFE" },
                                                { "@sChaveNFe", NomeArquivo.Replace("-nfe.err", "") },
                                                { "@sCStat",    "999" },
                                                { "@sxMotivo",  sMotivo }
                                            };
                                            ExecutarDataSet(sProcedure, vParametros);

                                            LOG_NFE("NFE: Erro: " + NomeArquivo.Replace("-nfe.err", ""));

                                            MoveArquivo_Windows(arquivo.ToString(), pasta_Retorno + @"\T-FLOW\ERRO\", NomeArquivo.Trim());
                                        }
                                    }
                                    else
                                    {
                                        LOG_NFE("[NFe] - Arquivo Descartado: " + NomeArquivo.ToString());
                                        MoveArquivo_Windows(arquivo.ToString(), pasta_Retorno + @"\T-FLOW\DESCARTADOS\", NomeArquivo.Trim());
                                    }
                                }
                            }
                            else LOG_NFE("Nenhum arquivo para importar!");
                        }
                        else LOG_NFE($"A pasta {pasta_Retorno} não existe!");
                    }

                    // NFe - Enviados
                    {
                        string pasta_Enviados = Path.Combine(cnpjDir, $@"Enviado\\Autorizados\{DateTime.Now.Year}{DateTime.Now.Month.ToString().PadLeft(2, '0')}");

                        if (Directory.Exists(pasta_Enviados))
                        {
                            string[] arquivos = Directory.GetFiles(pasta_Enviados);
                            string nomeUltimaPasta = Path.GetFileName(pasta_Enviados);

                            if (arquivos.Length > 0)
                            {
                                LOG_NFE($"Verificando arquivos da pasta: {pasta_Enviados}!");

                                foreach (var arquivo in arquivos)
                                {
                                    string NomeArquivo = Path.GetFileName(arquivo);

                                    if (NomeArquivo.Contains("-procNFe"))
                                    {
                                        XmlDocument doc = new XmlDocument();
                                        doc.Load(arquivo);

                                        XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
                                        nsManager.AddNamespace("nfe", "http://www.portalfiscal.inf.br/nfe");
                                        XmlNode xMLInfProt = doc.SelectSingleNode("//nfe:protNFe/nfe:infProt", nsManager);
                                        XmlNode snProt = doc.SelectSingleNode("//nfe:protNFe/nfe:infProt/nfe:nProt", nsManager);

                                        if (xMLInfProt != null)
                                        {
                                            string scStat = xMLInfProt["cStat"].InnerText;
                                            Dictionary<string, string> vParametros = new Dictionary<string, string>
                                            {
                                                { "@sFuncao",           "ALTERAR_STATUS_NFE" },
                                                { "@sXML",              doc.InnerXml },
                                                { "@sXML_Autorizado",   scStat == "100" ? "S" : "N"},
                                                { "@sChaveNFe",         xMLInfProt["chNFe"].InnerText },
                                                { "@tpAmb",             xMLInfProt["tpAmb"].InnerText },
                                                { "@sCStat",            scStat},
                                                { "@sxMotivo",          xMLInfProt["xMotivo"].InnerText },
                                                { "@snProt",            snProt != null ? xMLInfProt["nProt"].InnerText : "" }
                                            };
                                            ExecutarDataSet(sProcedure, vParametros);

                                            LOG_NFE("XML Importado: " + xMLInfProt["chNFe"].InnerText + " - " + xMLInfProt["cStat"].InnerText + " - " + xMLInfProt["xMotivo"].InnerText);

                                            if (scStat == "100")
                                            {
                                                Funcoes_NFe.DANFE.GerarDANFE(cnpjDir + @"\T-FLOW\PROCESSADO", xMLInfProt["chNFe"].InnerText, doc.InnerXml, true, "", "");
                                                //Temporário - Retirar após ajutar a GRID NFe
                                                CopiarArquivo_Windows(cnpjDir + @"\T-FLOW\PROCESSADO\" + xMLInfProt["chNFe"].InnerText + "_DANFE.pdf", cnpjDir + @"\DownloadNFe\", xMLInfProt["chNFe"].InnerText + ".pdf");
                                                LOG_NFE("DANFE Gerado: " + xMLInfProt["chNFe"].InnerText);
                                            }

                                            MoveArquivo_Windows(arquivo.ToString(), pasta_Enviados + @"\T-FLOW\UTILIZADO\", NomeArquivo.Trim());
                                        }
                                    }
                                    else if (NomeArquivo.Contains("-procEventoNFe"))
                                    {
                                        string chave = "";
                                        string caminhoXml = arquivo;

                                        XmlDocument XML_procEventoNFe = new XmlDocument();
                                        XML_procEventoNFe.Load(arquivo);
                                        XmlNamespaceManager nsManager = new XmlNamespaceManager(XML_procEventoNFe.NameTable);
                                        nsManager.AddNamespace("nfe", "http://www.portalfiscal.inf.br/nfe");
                                        XmlNode xMLInfProt = XML_procEventoNFe.SelectSingleNode("//nfe:retEvento/nfe:infEvento", nsManager);

                                        chave = xMLInfProt["chNFe"].InnerText;
                                        string stpEvento = xMLInfProt["tpEvento"].InnerText;

                                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                                        {
                                            { "@sFuncao",           "ALTERAR_STATUS_NFE" },
                                            { "@sXML",              XML_procEventoNFe.InnerXml },
                                            { "@sXML_Autorizado",   "S"},
                                            { "@sChaveNFe",         chave},
                                            { "@tpAmb",             xMLInfProt["tpAmb"].InnerText },
                                            { "@sCStat",            xMLInfProt["cStat"].InnerText },
                                            { "@sxMotivo",          xMLInfProt["xMotivo"].InnerText },
                                            { "@snProt",            xMLInfProt["nProt"].InnerText },
                                            { "@tpEvento",          stpEvento },
                                            { "@nSeqEvento",        xMLInfProt["nSeqEvento"].InnerText }
                                        };
                                        ExecutarDataSet(sProcedure, vParametros);

                                        if (stpEvento == "110110")
                                        {
                                            Funcoes_CCe.Gerar_PDF(chave, xMLInfProt["nSeqEvento"].InnerText, sPath_TFlow, true);
                                            LOG_NFE("CCe Gerado: " + chave);
                                        }
                                        else LOG_NFE("Cancelamento Efetuado: " + chave);

                                        MoveArquivo_Windows(arquivo.ToString(), pasta_Enviados + @"\T-FLOW\UTILIZADO\", NomeArquivo.Trim());
                                    }
                                    else
                                    {
                                        LOG_NFE("[NFe] - Arquivo Descartado: " + NomeArquivo.ToString());
                                        MoveArquivo_Windows(arquivo.ToString(), pasta_Enviados + @"\T-FLOW\DESCARTADOS\", NomeArquivo.Trim());
                                    }
                                }
                            }
                            else LOG_NFE("Nenhum arquivo para importar!");
                        }
                        else LOG_NFE($"A pasta {pasta_Enviados} não existe!");
                    }

                    // NFSe - Retorno
                    {
                        string pasta_NFSe = Path.Combine(cnpjDir, "nfse", "Retorno");

                        if (Directory.Exists(pasta_NFSe))
                        {
                            string[] arquivos = Directory.GetFiles(pasta_NFSe);

                            if (arquivos.Length > 0)
                            {
                                LOG_NFE($"Verificando arquivos da pasta: {pasta_NFSe}!");

                                foreach (var arquivo in arquivos)
                                {
                                    string sNomeArquivo = Path.GetFileName(arquivo);

                                    if (sNomeArquivo.Contains("-ret-loterps.xml"))
                                    {
                                        XmlDocument doc = new XmlDocument();
                                        doc.Load(arquivo);

                                        bool bSucesso = bool.TryParse(doc.SelectSingleNode("//*[local-name()='Sucesso']")?.InnerText, out var sucessoValue) && sucessoValue;

                                        if (bSucesso)
                                        {
                                            XmlNodeList chaves = doc.SelectNodes("//*[local-name()='ChaveNFeRPS']");

                                            foreach (XmlNode chaveNFeRPS in chaves)
                                            {
                                                string sNumeroNFe = chaveNFeRPS.SelectSingleNode("./*[local-name()='ChaveNFe']/*[local-name()='NumeroNFe']")?.InnerText;
                                                string sCodigoVerificacao = chaveNFeRPS.SelectSingleNode("./*[local-name()='ChaveNFe']/*[local-name()='CodigoVerificacao']")?.InnerText;
                                                string sNumeroRPS = chaveNFeRPS.SelectSingleNode("./*[local-name()='ChaveRPS']/*[local-name()='NumeroRPS']")?.InnerText;
                                                string sSerieRPS = chaveNFeRPS.SelectSingleNode("./*[local-name()='ChaveRPS']/*[local-name()='SerieRPS']")?.InnerText;
                                                string idArquivo = SalvarArquivo(sNomeArquivo, 10009, File.ReadAllBytes(arquivo));

                                                Dictionary<string, string> vParametros = new Dictionary<string, string>
                                                {
                                                    { "@sFuncao", "RETORNO_XML_NFS" },
                                                    { "@nNumeroNF", sNumeroNFe },
                                                    { "@sNumeroRPS", sNumeroRPS },
                                                    { "@sSerieRPS", sSerieRPS },
                                                    { "@sCodigoVerificacao", sCodigoVerificacao },
                                                    { "@idArquivo_Retorno", idArquivo },
                                                    { "@scStat", "100" },
                                                    { "@sXML_Autorizado", "S" },
                                                    { "@sxMotivo", "100 - Autorizado o uso da NFS-e" }
                                                };
                                                ExecutarDataSet(sProcedure, vParametros);

                                                LOG_NFE($"XML Importado: {sNomeArquivo} {{NFS: {sNumeroNFe} | Serie: {sSerieRPS} | RPS: {sNumeroRPS}}}");

                                                MoveArquivo_Windows(arquivo, pasta_NFSe + @"\T-FLOW\UTILIZADO\", sNomeArquivo.Trim());
                                            }
                                        }
                                        else
                                        {
                                            string sErro = string.Empty;
                                            string sAviso = string.Empty;
                                            string sNumeroRPS = string.Empty;

                                            XmlNodeList erros = doc.SelectNodes("//*[local-name()='Erro']");
                                            if (erros.Count > 0)
                                            {
                                                List<string> sErros = new List<string> { "<label>Erros</label><ul class='danger'>" };
                                                foreach (XmlNode erro in erros)
                                                {
                                                    string sCodigo = erro.SelectSingleNode("./*[local-name()='Codigo']")?.InnerText;
                                                    string sMensagem = erro.SelectSingleNode("./*[local-name()='Descricao']")?.InnerText;

                                                    if (!int.TryParse(sNumeroRPS, out int nNumeroRPS) || nNumeroRPS <= 0)
                                                    {
                                                        string sRPS = erro.SelectSingleNode("./*[local-name()='ChaveRPS']/*[local-name()='NumeroRPS']")?.InnerText;
                                                        sNumeroRPS = int.TryParse(sRPS, out int nRPS) ? sRPS : string.Empty;
                                                    }

                                                    sErros.Add($"<li>{sCodigo} - {sMensagem}</li>");
                                                }

                                                sErros.Add("</ul>");
                                                sErro = string.Join("", sErros);
                                            }

                                            XmlNodeList avisos = doc.SelectNodes("//*[local-name()='Alerta']");
                                            if (avisos.Count > 0)
                                            {
                                                List<string> sAvisos = new List<string> { "<label>Avisos</label><ul>" };
                                                foreach (XmlNode aviso in avisos)
                                                {
                                                    string sCodigo = aviso.SelectSingleNode("./*[local-name()='Codigo']")?.InnerText;
                                                    string sMensagem = aviso.SelectSingleNode("./*[local-name()='Descricao']")?.InnerText;
                                                    sAvisos.Add($"<li>{sCodigo} - {sMensagem}</li>");
                                                }

                                                sAvisos.Add("</ul>");
                                                sAviso = string.Join("", sAvisos);
                                            }

                                            if (!string.IsNullOrWhiteSpace(sErro))
                                            {
                                                string sCNPJ_Lote = sNomeArquivo.Split('-')[0];
                                                Dictionary<string, string> vParametros = new Dictionary<string, string>
                                                {
                                                    { "@sFuncao", "RETORNO_XML_NFS" },
                                                    { "@scStat", "999" },
                                                    { "@sCancelamento", "N" },
                                                    { "@sLoteRPS", sCNPJ_Lote.Substring(sCNPJ_Lote.IndexOf('_') + 1) },
                                                    { "@sxMotivo", $"{sErro}{sAviso}" }
                                                };
                                                ExecutarDataSet(sProcedure, vParametros);

                                                LOG_NFE($"XML com erro Importado: {sNumeroRPS} - 999");

                                                MoveArquivo_Windows(arquivo, pasta_NFSe + @"\T-FLOW\UTILIZADO\", sNomeArquivo.Trim());
                                            }
                                        }
                                    }
                                    else if (sNomeArquivo.Contains("-ret-loterps.err"))
                                    {
                                        string[] linhas = File.ReadAllLines(arquivo);

                                        string sCodigoErro = null;
                                        string sMensagem = null;
                                        string sTipo = null;

                                        foreach (string linha in linhas)
                                        {
                                            int separador = linha.IndexOf('|');
                                            if (separador <= 0)
                                                continue;

                                            string valor = linha.Substring(separador + 1);
                                            switch (linha.Substring(0, separador))
                                            {
                                                case "ErrorCode": sCodigoErro = valor; break;
                                                case "Message": sMensagem = valor; break;
                                                case "Type": sTipo = valor; break;
                                            }
                                        }

                                        string sCNPJ_Lote = sNomeArquivo.Split('-')[0];
                                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                                        {
                                            { "@sFuncao", "RETORNO_XML_NFS" },
                                            { "@sLoteRPS", sCNPJ_Lote.Substring(sCNPJ_Lote.IndexOf('_') + 1) },
                                            { "@scStat", "999" },
                                            { "@sxMotivo", $"{sCodigoErro}: {sMensagem}" }
                                        };
                                        ExecutarDataSet(sProcedure, vParametros);

                                        LOG_NFE($"XML de erro importado: {sNomeArquivo} {{{sCodigoErro} - {sTipo}}}");

                                        MoveArquivo_Windows(arquivo, pasta_NFSe + @"\T-FLOW\UTILIZADO\", sNomeArquivo.Trim());
                                    }
                                    else if (sNomeArquivo.Contains("-cannfse.xml"))
                                    {
                                        XmlDocument doc = new XmlDocument();
                                        doc.Load(arquivo);

                                        bool bSucesso = bool.TryParse(doc.SelectSingleNode("//*[local-name()='Sucesso']")?.InnerText, out var sucessoValue) && sucessoValue;
                                        if (bSucesso)
                                        {
                                            string sCNPJ_Lote = sNomeArquivo.Split('-')[0];
                                            string sLote = sCNPJ_Lote.Substring(sCNPJ_Lote.IndexOf('_') + 1);
                                            Dictionary<string, string> vParametros = new Dictionary<string, string>
                                            {
                                                { "@sFuncao", "RETORNO_XML_NFS" },
                                                { "@sLoteRPS", sLote },
                                                { "@scStat", "100" },
                                                { "@sCancelamento", "S" }
                                            };
                                            ExecutarDataSet(sProcedure, vParametros);

                                            LOG_NFE($"XML do Cancelamento Importado: {sNomeArquivo} {{Lote: {sLote}}}");

                                            MoveArquivo_Windows(arquivo, pasta_NFSe + @"\T-FLOW\UTILIZADO\", sNomeArquivo.Trim());
                                        }
                                    }
                                    else if (sNomeArquivo.Contains("-cannfse.err"))
                                    {
                                        string[] linhas = File.ReadAllLines(arquivo);

                                        string sCodigoErro = null;
                                        string sTipo = null;

                                        bool lendoMensagem = false;
                                        StringBuilder mensagem = new StringBuilder();

                                        foreach (string linha in linhas)
                                        {
                                            int separador = linha.IndexOf('|');

                                            if (separador > 0)
                                            {
                                                string campo = linha.Substring(0, separador);
                                                string valor = linha.Substring(separador + 1);

                                                switch (campo)
                                                {
                                                    case "ErrorCode":
                                                        sCodigoErro = valor;
                                                        lendoMensagem = false;
                                                        break;

                                                    case "Message":
                                                        mensagem.Clear();
                                                        mensagem.Append(valor);
                                                        lendoMensagem = true;
                                                        break;

                                                    case "Type":
                                                        sTipo = valor;
                                                        lendoMensagem = false;
                                                        break;

                                                    default:
                                                        lendoMensagem = false;
                                                        break;
                                                }
                                            }
                                            else if (lendoMensagem)
                                            {
                                                mensagem.AppendLine();
                                                mensagem.Append(linha);
                                            }
                                        }

                                        string sCNPJ_Lote = sNomeArquivo.Split('-')[0];
                                        string sLote = sCNPJ_Lote.Substring(sCNPJ_Lote.IndexOf('_') + 1);
                                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                                        {
                                            { "@sFuncao", "RETORNO_XML_NFS" },
                                            { "@scStat", "999" },
                                            { "@sCancelamento", "S" },
                                            { "@sLoteRPS", sLote },
                                            { "@sxMotivo", $"{sCodigoErro} - {mensagem}" }
                                        };
                                        ExecutarDataSet(sProcedure, vParametros);

                                        LOG_NFE($"XML de erro importado: {sNomeArquivo} {{{sCodigoErro} - {sTipo}}}");
                                        MoveArquivo_Windows(arquivo, pasta_NFSe + @"\T-FLOW\UTILIZADO\", sNomeArquivo.Trim());
                                    }
                                    else
                                    {
                                        LOG_NFE($"[NFe] - Arquivo Descartado: {sNomeArquivo}");
                                        MoveArquivo_Windows(arquivo, pasta_NFSe + @"\T-FLOW\DESCARTADOS\", sNomeArquivo.Trim());
                                    }
                                }
                            }
                            else LOG_NFE("Nenhum arquivo para importar!");
                        }
                        else LOG_NFE($"A pasta {pasta_NFSe} não existe!");
                    }

                    LOG_NFE("[NFe | NFSe] - Finalizando processo de verificação de Arquivos");
                }
            }
        }

        private static string ExtrairMensagemDoArquivoErr(string arquivoErr)
        {
            try
            {
                // Ler o arquivo .err
                using (StreamReader reader = new StreamReader(arquivoErr))
                {
                    string linha;
                    bool capturandoMensagem = false;
                    StringBuilder mensagem = new StringBuilder();

                    while ((linha = reader.ReadLine()) != null)
                    {
                        // Iniciar captura quando encontrar "Message|"
                        if (linha.StartsWith("Message|"))
                        {
                            capturandoMensagem = true;
                            // Adicionar o conteúdo da linha após "Message|"
                            mensagem.AppendLine(linha.Substring("Message|".Length).Trim());
                            continue;
                        }

                        // Parar a captura quando encontrar "StackTrace|"
                        if (linha.StartsWith("StackTrace|"))
                        {
                            break;
                        }

                        // Continuar capturando linhas após "Message|" até encontrar "StackTrace|"
                        if (capturandoMensagem)
                        {
                            mensagem.AppendLine(linha.Trim());
                        }
                    }

                    // Retornar a mensagem extraída
                    return mensagem.ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler o arquivo .err: {ex.Message}");
            }

            return "Mensagem não encontrada";
        }

	#region | Atualiza Status Cliente

        private void tmrAtualiza_Status_ClientePontual_Tick(object sender, EventArgs e) => Task.Run(() => Processar_tmrAtualiza_Status_ClientePontual());

        private void Processar_tmrAtualiza_Status_ClientePontual()
        {
            BeginInvoke((Action)(() => tmrAtualiza_Status_ClientePontual.Stop()));

            LOG_Status("Iniciando Processo de Atualizar Status Cliente");

            {
                try
                {
                    DataSet dsPesquisa = ExecutarDataSet("sp_Manipula_tbl_Flow_Clientes", new Dictionary<string, string> { { "@sFuncao", "Atualiza_ClientePontual" } });                    
                }
                catch (Exception ex)
                {
                    LOG_NFE("Erro:  " + ex.Message);
                }
            }

            LOG_Status("Processo de Atualizar Status Cliente Finalizado");
            BeginInvoke((Action)(() => tmrAtualiza_Status_ClientePontual.Start()));
        }

        #endregion
        #endregion

        #region | Atualizar Impostos - LegisWeb

        /// <summary>
        /// Função utilizada para Validar os dados da requisição para a API 'LegisWeb'.
        /// </summary>
        /// <param name="dado">Recebe o valor que será validado.</param>
        /// <param name="tipo">Recebe o tipo que o dado deveria ser.</param>
        /// <returns>Retorna o dado validado e transformado, se necessário.</returns>
        protected string Validar_Dados_LegisWeb(string dado, int tipo)
        {
            string retorno = dado;

            switch (tipo) // tipos: 0 = string, 1 = decimal, 2 = DateTime
            {
                case 0:
                    retorno = string.IsNullOrEmpty(dado) ? string.Empty : dado.Replace("\\/", "/").Trim();
                    break;

                case 1:
                    decimal.TryParse(dado.Replace(".", ",").Trim(), out decimal valor);
                    retorno = Math.Round(valor, 2).ToString().Replace(",", ".");
                    break;

                case 2:
                    if (DateTime.TryParse(dado, out DateTime data)) retorno = dado.Replace("\\/", "/").Trim();
                    else retorno = string.Empty;
                    break;
            }

            return retorno;
        }

        /// <summary>
        /// Função para verificar se existem registros dentro do conteúdo em JSON.
        /// </summary>
        /// <param name="json">Recebe o objeto JObject do próprio JSON onde será verificada a existência de registros compatíveis.</param>
        /// <param name="NCM">Recebe a string NCM que será utilizada para filtrar e encontrar os registros compatíveis.</param>
        /// <returns>Retorna o objeto JSON compatível com o NCM recebido.</returns>
        protected JObject RetornaResposta_JSON(JObject json, string NCM)
        {
            if (json == null || (json.ContainsKey("registros") && json["registros"].ToString() == "0")) return null;

            JArray registros = (JArray)json["resposta"];
            foreach (JObject registro in registros)
            {
                string NCM_LegisWeb = registro["ncm"].ToString().Replace(".", "").Trim();
                string NCM_TFlow = NCM.Replace(".", "");

                if (NCM_LegisWeb == NCM_TFlow
                    || (NCM_LegisWeb.Length >= 6 && NCM_TFlow.Length >= 6 && NCM_LegisWeb.Substring(0, 6) == NCM_TFlow.Substring(0, 6))
                    || (NCM_LegisWeb.Length >= 4 && NCM_TFlow.Length >= 4 && NCM_LegisWeb.Substring(0, 4) == NCM_TFlow.Substring(0, 4))
                    )
                    return registro;
            }

            return null;
        }

        #region | ST

        private void tmrAtualiza_ST_LegisWeb_Tick(object sender, EventArgs e) => Task.Run(() => Processar_tmrAtualiza_ST_LegisWeb());

        private void Processar_tmrAtualiza_ST_LegisWeb()
        {
            string sNCM = string.Empty, sCEST = string.Empty, sOrigem = string.Empty, sDestino = string.Empty;

            try
            {
                BeginInvoke((Action)(() => tmrAtualiza_ST_LegisWeb.Stop()));

                LOG_Status("Verificando os NCMs por Estado, para Salvar e/ou Atualizar os valores de ST");

                DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Fiscal_Regras", new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_NCM_X_ESTADOS" } });

                if (ValidarDataSet(ds))
                {
                    string msg = RETORNO.DATASET(ds, 1, 0, "sMensagem_Erro_LegisWeb");

                    if (string.IsNullOrEmpty(msg))
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            DataSet dsRetorno = new DataSet();
                            string NCM = row.Field<string>("sNCM").Trim();
                            string CEST = row.Field<string>("sCEST").Trim();
                            string origem = row.Field<string>("sEstadoOrigem").Trim().ToUpper();
                            string destino = row.Field<string>("sEstadoDestino").Trim().ToUpper();

                            sNCM = NCM;
                            sCEST = CEST;
                            sOrigem = origem;
                            sDestino = destino;

                            JObject json = JObject.Parse(API.LegisWeb.Consultar_ST_Interestadual(origem, destino, NCM.Replace(".", ""), "2"));
                            JObject resposta = null;

                            if (json == null || (json.ContainsKey("registros") && json["registros"].ToString() == "0")) goto salvaResposta;

                            JArray registros = (JArray)json["resposta"];
                            foreach (JObject registro in registros)
                            {
                                string NCM_LegisWeb = registro["ncm"].ToString().Replace(".", "").Trim();
                                string NCM_TFlow = NCM.Replace(".", "");
                                string CEST_LegisWeb = registro["cest"].ToString().Replace(".", "").Trim();
                                string CEST_TFlow = CEST.Replace(".", "");

                                if (CEST_LegisWeb == CEST_TFlow &&
                                     (
                                        NCM_LegisWeb == NCM_TFlow
                                        || (NCM_LegisWeb.Length >= 6 && NCM_TFlow.Length >= 6 && NCM_LegisWeb.Substring(0, 6) == NCM_TFlow.Substring(0, 6))
                                        || (NCM_LegisWeb.Length >= 4 && NCM_TFlow.Length >= 4 && NCM_LegisWeb.Substring(0, 4) == NCM_TFlow.Substring(0, 4))
                                     )
                                    )
                                {
                                    resposta = registro;
                                    break;
                                }
                            }

                        salvaResposta:
                            if (resposta != null)
                            {
                                string final = resposta["vigencia_final"].ToString();

                                Dictionary<string, string> vParam = new Dictionary<string, string>
                                {
                                    { "@sFuncao", "SALVAR_LEGISWEB" },
                                    { "@idRegistro", row["idRegistro"].ToString() },
                                    { "@sCodigo", Validar_Dados_LegisWeb(resposta["codigo"].ToString(), 0) },
                                    { "@sSigla_Estado_Origem", Validar_Dados_LegisWeb(resposta["sigla_estado_origem"].ToString(), 0) },
                                    { "@sSigla_Estado_Destino", Validar_Dados_LegisWeb(resposta["sigla_estado_destino"].ToString(), 0) },
                                    { "@sRegime_Origem", Validar_Dados_LegisWeb(resposta["regime_origem"].ToString(), 0) },
                                    { "@sRegime_Destino", Validar_Dados_LegisWeb(resposta["regime_destino"].ToString(), 0) },
                                    { "@sDestino_Produto", Validar_Dados_LegisWeb(resposta["destino_produto"].ToString(), 0) },
                                    { "@sNCM", Validar_Dados_LegisWeb(resposta["ncm"].ToString(), 0) },
                                    { "@sCEST", Validar_Dados_LegisWeb(resposta["cest"].ToString(), 0) },
                                    { "@sDescricao", Validar_Dados_LegisWeb(resposta["descricao"].ToString(), 0) },
                                    { "@sObservacao", Validar_Dados_LegisWeb(resposta["observacao"].ToString(), 0) },
                                    { "@sSegmento", Validar_Dados_LegisWeb(resposta["segmento"].ToString(), 0) },
                                    { "@sCodigo_Segmento", Validar_Dados_LegisWeb(resposta["codigo_segmento"].ToString(), 0) },
                                    { "@nAliquota_Interna", Validar_Dados_LegisWeb(resposta["aliquota_interna"].ToString(), 1) },
                                    { "@nAliquota_Interestadual", Validar_Dados_LegisWeb(resposta["aliquota_interestadual"].ToString(), 1) },
                                    { "@nFundoPobreza", Validar_Dados_LegisWeb(resposta["fundo_pobreza"].ToString(), 1) },
                                    { "@nMVA", Validar_Dados_LegisWeb(resposta["mva"].ToString(), 1) },
                                    { "@nMVA_Ajustado", Validar_Dados_LegisWeb(resposta["mva_ajustada"].ToString(), 1) },
                                    { "@nMVA_Ajustado_4", Validar_Dados_LegisWeb(resposta["mva_ajustada_4"].ToString(), 1) },
                                    { "@nMVA_Positiva", Validar_Dados_LegisWeb(resposta["mva_positiva"].ToString(), 1) },
                                    { "@nMVA_Negativa", Validar_Dados_LegisWeb(resposta["mva_negativa"].ToString(), 1) },
                                    { "@nMVA_Neutra", Validar_Dados_LegisWeb(resposta["mva_neutra"].ToString(), 1) },
                                    { "@dtVigencia_Inicial", Validar_Dados_LegisWeb(resposta["vigencia_inicial"].ToString(), 2) },
                                    { "@dtVigencia_Final", Validar_Dados_LegisWeb(resposta["vigencia_final"].ToString(), 2) },
                                    { "@sBase_Legal_ST", Validar_Dados_LegisWeb(resposta["base_legal_st"].ToString(), 0) },
                                    { "@dtEfeito_ST", Validar_Dados_LegisWeb(resposta["data_efeito_st"].ToString(), 2) },
                                    { "@sNorma_Obs_ST", Validar_Dados_LegisWeb(resposta["norma_observacao_st"].ToString(), 0) },
                                    { "@sNorma_Base_Calculo", Validar_Dados_LegisWeb(resposta["norma_base_calculo"].ToString(), 0) },
                                    { "@sNorma_Prazo_Recolhimento", Validar_Dados_LegisWeb(resposta["norma_prazo_recolhimento"].ToString(), 0) },
                                    { "@sBase_Legal_int", Validar_Dados_LegisWeb(resposta["base_legal_int"].ToString(), 0) },
                                    { "@sObservacao_int", Validar_Dados_LegisWeb(resposta["observacao_int"].ToString(), 0) },
                                    { "@sBase_Calculo_int", Validar_Dados_LegisWeb(resposta["base_calculo_int"].ToString(), 0) },
                                    { "@sPrazo_Recolhimento_int", Validar_Dados_LegisWeb(resposta["prazo_recolhimento_int"].ToString(), 0) },
                                    { "@sAplicabilidade", Validar_Dados_LegisWeb(resposta["aplicabilidade"].ToString(), 0) },
                                    { "@sNaoAplicabilidade", Validar_Dados_LegisWeb(resposta["nao_aplicabilidade"].ToString(), 0) },
                                    { "@sVariacao_MVA", Validar_Dados_LegisWeb(resposta["variacao_mva"].ToString(), 0) },
                                    { "@sReducao_MVA", Validar_Dados_LegisWeb(resposta["reducao_mva"].ToString(), 0) },
                                    { "@sJSON", json == null ? string.Empty : json.ToString() }
                                };
                                dsRetorno = ExecutarDataSet("sp_Manipula_tbl_Flow_Fiscal_Regras", vParam);
                            }
                            else
                            {
                                Dictionary<string, string> vParam = new Dictionary<string, string>
                                {
                                    { "@sFuncao", "SALVAR_LEGISWEB" },
                                    { "@sSigla_Estado_Origem", origem },
                                    { "@sSigla_Estado_Destino", destino },
                                    { "@sNCM", NCM },
                                    { "@sCEST", CEST },
                                    { "@sJSON", json.ToString() },
                                    { "@sCodigo", "" }, { "@sRegime_Origem", "" }, { "@sRegime_Destino", "" }, { "@sDestino_Produto", "" }, { "@sDescricao", "" }, { "@sObservacao", "" }, { "@sSegmento", "" }, { "@sCodigo_Segmento", "" },
                                    { "@nAliquota_Interna", "0" }, { "@nAliquota_Interestadual", "0" }, { "@nFundoPobreza", "0" }, { "@nMVA", "0" }, { "@nMVA_Ajustado", "0" }, { "@nMVA_Ajustado_4", "0" }, { "@nMVA_Positiva", "0" },
                                    { "@nMVA_Negativa", "0" }, { "@nMVA_Neutra", "0" }, { "@dtVigencia_Inicial", "" }, { "@dtVigencia_Final", "" }, { "@sBase_Legal_ST", "" }, { "@dtEfeito_ST", "" }, { "@sNorma_Obs_ST", "" },
                                    { "@sNorma_Base_Calculo", "" }, { "@sNorma_Prazo_Recolhimento", "" }, { "@sBase_Legal_int", "" }, { "@sObservacao_int", "" }, { "@sBase_Calculo_int", "" }, { "@sPrazo_Recolhimento_int", "" },
                                    { "@sAplicabilidade", "" }, { "@sNaoAplicabilidade", "" }, { "@sVariacao_MVA", "" }, { "@sReducao_MVA", "" }
                                };
                                dsRetorno = ExecutarDataSet("sp_Manipula_tbl_Flow_Fiscal_Regras", vParam);
                            }

                            LOG_Status($"Valores de ST de {origem} para {destino}, com o NCM {NCM} e CEST {CEST}, atualizados com sucesso!      Retorno do BD: {RETORNO.DATASET(dsRetorno, "sMsg")}");
                        }

                        LOG_Status("Processo de Atualizar ST finalizado");
                    }
                    else
                        LOG_Status($"\r\n===================================================\r\nHá um Erro salvo, na tabela de Parâmetros, que impede a atualização do ST.\r\n{msg}\r\n\r\nProcesso de Atualizar ST Interrompido!\r\n===================================================\r\n");
                }
                else
                {
                    LOG_Status("Não foram encontrados registros para serem Atualizados");
                    LOG_Status("Processo de Atualizar ST finalizado");
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The underlying connection was closed")) goto final;

                string mensagem = "";

                if (string.IsNullOrEmpty(sNCM)) sNCM = "---";
                if (string.IsNullOrEmpty(sCEST)) sCEST = "---";
                if (string.IsNullOrEmpty(sOrigem)) sOrigem = "---";
                if (string.IsNullOrEmpty(sDestino)) sDestino = "---";

                try
                {
                    mensagem = $"Favor encaminhar um email, exibindo toda esta mensagem de erro junto com o número do Orçamento, para o t-flow@tecandtec.com.br <br /><b>Obs:</b> Ainda é possível Salvar as informações do Orçamento, no entanto é provável que alguns produtos do Orçamento não possuam valor de ST.<br /><br /><b>Erro:</b> Houve um erro ao Atualizar o ICMS ST, com origem em {sOrigem} e destino para {sDestino}, do NCM {sNCM} e CEST {sCEST}: {ex.Message}";

                    Dictionary<string, string> vParam = new Dictionary<string, string> { { "@sFuncao", "SALVAR_LEGISWEB_ERRO" }, { "@sMsg_Erro", mensagem } };
                    ExecutarDataSet("sp_Manipula_tbl_Flow_Fiscal_Regras", vParam);
                }
                catch (Exception ex1)
                {
                    mensagem = $"<b>Erro:</b> Houve um erro na tentativa de Salvar o registro do erro, ocorrido ao Atualizar as informações de ICMS ST.<br />Segue mensagem de erro gerada ao Salvar o erro: {ex1.Message}<br /><br />Segue mensagem de erro gerada ao Atualizar o ST: {ex.Message}";
                }

                LOG_Status($"\r\n===================================================\r\nErro ao Atualizar o ST dos Itens: \r\n{mensagem}\r\n\r\nProcesso de Atualizar ST Interrompido!\r\n===================================================\r\n");
            }

        final:
            BeginInvoke((Action)(() => tmrAtualiza_ST_LegisWeb.Start()));
        }

        #endregion

        #region | Impostos por NCM

        private void tmrAtualiza_Impostos_LegisWeb_Tick(object sender, EventArgs e) => Task.Run(() => Processar_tmrAtualiza_Impostos_LegisWeb());

        private void Processar_tmrAtualiza_Impostos_LegisWeb()
        {
            try
            {
                BeginInvoke((Action)(() => tmrAtualiza_Impostos_LegisWeb.Stop()));

                LOG_Status("Verificando os NCMs para Salvar e/ou Atualizar seus Impostos");

                DataTable tb = ExecutarDataTable("sp_Manipula_tbl_Flow_Fiscal_Regras", new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_TODOS_NCM" } });

                if (tb.Rows.Count > 0)
                {
                    foreach (DataRow row in tb.Rows)
                    {
                        string NCM = row.Field<string>("sNCM").Trim();
                        string II = null;
                        string IPI = null;
                        string PIS = null;
                        string COFINS = null;
                        string ICMS = null;
                        string mensagem = null;

                        // Consulta de Impostos
                        {
                            // II
                            try
                            {
                                JObject json = JObject.Parse(API.LegisWeb.Consultar_II(NCM.Replace(".", "")));
                                JObject resposta = RetornaResposta_JSON(json, NCM);
                                if (resposta != null) II = Validar_Dados_LegisWeb(resposta["aliquota"].ToString(), 1);
                            }
                            catch (Exception ex)
                            {
                                mensagem += $"\r\n=====\r\nErro ao Consultar o valor de II!\r\nErro: {ex.Message}\r\n=====\r\n";
                            }

                            // IPI
                            try
                            {
                                JObject json = JObject.Parse(API.LegisWeb.Consultar_IPI(NCM.Replace(".", "")));
                                JObject resposta = RetornaResposta_JSON(json, NCM);
                                if (resposta != null) IPI = Validar_Dados_LegisWeb(resposta["aliquota"].ToString(), 1);
                            }
                            catch (Exception ex)
                            {
                                mensagem += $"\r\n=====\r\nErro ao Consultar o valor de IPI!\r\nErro: {ex.Message}\r\n=====\r\n";
                            }

                            // PIS e COFINS
                            //try
                            //{
                            //    JObject json = JObject.Parse(API.LegisWeb.Consultar_PIS_COFINS(NCM.Replace(".", "")));
                            //    JObject resposta = RetornaResposta_JSON(json, NCM);
                            //    if (resposta != null) { PIS = Validar_Dados_LegisWeb(resposta["aliquota"].ToString(), 1); COFINS = Validar_Dados_LegisWeb(resposta["aliquota"].ToString(), 1); }
                            //}
                            //catch (Exception ex)
                            //{
                            //    mensagem += $"\r\n=====\r\nErro ao Consultar o valor de PIS e COFINS!\r\nErro: {ex.Message}\r\n=====\r\n";
                            //}

                            // ICMS
                            //try
                            //{
                            //    JObject json = JObject.Parse(API.LegisWeb.Consultar_ICMS(NCM.Replace(".", "")));
                            //    JObject resposta = RetornaResposta_JSON(json, NCM);
                            //    if (resposta != null) ICMS = Validar_Dados_LegisWeb(resposta["aliquota"].ToString(), 1);
                            //}
                            //catch (Exception ex)
                            //{
                            //    mensagem += $"\r\n=====\r\nErro ao Consultar o valor de ICMS!\r\nErro: {ex.Message}\r\n=====\r\n";
                            //}
                        }

                        if (!string.IsNullOrEmpty(mensagem))
                            LOG_Status($"\r\n==================================================={mensagem}===================================================\r\n");

                        Dictionary<string, string> vParam = new Dictionary<string, string>
                        {
                            { "@sFuncao", "ATUALIZA_IMPOSTOS_X_NCM" },
                            { "@idRegistro", row["idNCM"].ToString() },
                            { "@nII", II },
                            { "@nIPI", IPI },
                            { "@nPIS", PIS },
                            { "@nCOFINS", COFINS },
                            { "@nICMS", ICMS }
                        };
                        DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Fiscal_Regras", vParam);

                        LOG_Status($"Valores dos Impostos do NCM {NCM}, atualizados com sucesso!      Retorno do BD: {RETORNO.DATASET(ds, "sMsg")}");
                    }

                    LOG_Status("Processo de Atualizar Impostos finalizado");
                }
                else
                {
                    LOG_Status("Não foram encontrados registros para serem Atualizados");
                    LOG_Status("Processo de Atualizar Impostos finalizado");
                }
            }
            catch { }

            BeginInvoke((Action)(() => tmrAtualiza_Impostos_LegisWeb.Start()));
        }

        #endregion
    #region | Arquivos IA |

        private void tmrProcessaArquivosIA_Tick(object sender, EventArgs e) => Task.Run(() => Processar_tmrProcessaArquivosIA());

        private void Processar_tmrProcessaArquivosIA()
        {
            try
            {
                BeginInvoke((Action)(() => tmrProcessaArquivosIA.Stop()));

                int limite = 5;
                if (vParametros_Timers.ContainsKey("tmrProcessaArquivosIA"))
                {
                    int limiteConfigurado;
                    if (int.TryParse((vParametros_Timers["tmrProcessaArquivosIA"] ?? string.Empty).Trim(), out limiteConfigurado) && limiteConfigurado > 0)
                    {
                        limite = Math.Min(limiteConfigurado, 20);
                    }
                }

                int total = new IAArquivoMarkdownWorker().ProcessarFila(limite, LOG_Outros);
                if (total > 0)
                {
                    LOG_Outros("Fila IA arquivos processada. Total: " + total);
                }
            }
            catch (Exception ex)
            {
                LOG_Outros("Erro no processamento de arquivos IA: " + ex.Message);
            }
            finally
            {
                BeginInvoke((Action)(() => tmrProcessaArquivosIA.Start()));
            }
        }

        #endregion

        #region | Pasta da Base de Conhecimento IA |

        // Le a pasta de entrada da Base de Conhecimento (IA.Conhecimento.PastaEntrada, tela IA - Configuracao
        // do TT_Flow) e ingere os arquivos novos. Pasta vazia na configuracao = desligado, sem log.
        private void tmrConhecimentoPasta_Tick(object sender, EventArgs e) => Task.Run(() => Processar_tmrConhecimentoPasta());

        private void Processar_tmrConhecimentoPasta()
        {
            try
            {
                BeginInvoke((Action)(() => tmrConhecimentoPasta.Stop()));

                IAConhecimentoPastaWorker.ResultadoVarredura resultado = new IAConhecimentoPastaWorker().Executar(LOG_Outros);

                // So loga quando aconteceu algo ou quando a pasta configurada nao pode ser lida: um ciclo
                // sem arquivo novo a cada poucos minutos so encheria o log.
                if (resultado.TeveMovimento || resultado.Restantes > 0 || resultado.FalhaAcesso)
                {
                    LOG_Outros("Pasta da base de conhecimento: " + resultado.Mensagem);
                }
            }
            catch (Exception ex)
            {
                LOG_Outros("Erro na pasta da base de conhecimento IA: " + ex.Message);
            }
            finally
            {
                BeginInvoke((Action)(() => tmrConhecimentoPasta.Start()));
            }
        }

        #endregion

        #endregion
        #region | Logs

        void LOG_Email(string email)
        {
            if (txtLogEmail.InvokeRequired) { txtLogEmail.Invoke((Action)(() => LOG_Email(email))); return; }

            if (txtLogEmail.Text.Length > 65000) txtLogEmail.Text = txtLogEmail.Text.Substring(0, 60000);
            txtLogEmail.Text = DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff") + " - " + email + Environment.NewLine + txtLogEmail.Text;
        }

        void LOG_Impressora(string sTextoLog)
        {
            if (txtLogImpressora.InvokeRequired) { txtLogImpressora.Invoke((Action)(() => LOG_Impressora(sTextoLog))); return; }
            if (txtLogImpressora.Text.Length > 65000) txtLogImpressora.Text = txtLogImpressora.Text.Substring(0, 60000);
            txtLogImpressora.Text = DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff") + " - " + sTextoLog + Environment.NewLine + txtLogImpressora.Text;
        }

        void LOG_Ping(string ping)
        {
            if (txtlogPing.InvokeRequired) { txtlogPing.Invoke((Action)(() => LOG_Ping(ping))); return; }

            if (txtlogPing.Text.Length > 65000) txtlogPing.Text = txtlogPing.Text.Substring(0, 60000);
            txtlogPing.Text = DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff") + " - " + ping + Environment.NewLine + txtlogPing.Text;
        }

        void LOG_Outros(string outros)
        {
            if (txtOutrosLogs.InvokeRequired) { txtOutrosLogs.Invoke((Action)(() => LOG_Outros(outros))); return; }

            if (txtOutrosLogs.Text.Length > 65000) txtOutrosLogs.Text = txtOutrosLogs.Text.Substring(0, 62000);
            txtOutrosLogs.Text = DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff") + " - " + outros + Environment.NewLine + txtOutrosLogs.Text;
        }

        void LOG_Status(string status)
        {
            if (txtStatus.InvokeRequired) { txtStatus.Invoke((Action)(() => LOG_Status(status))); return; }

            if (txtStatus.Text.Length > 99000) txtStatus.Text = txtStatus.Text.Substring(0, 96000);
            txtStatus.Text = DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff") + " - " + status + Environment.NewLine + txtStatus.Text;
        }

        void LOG_NFE(string Mensagem)
        {
            if (txtLogNFe.InvokeRequired) { txtLogNFe.Invoke((Action)(() => LOG_NFE(Mensagem))); return; }

            if (txtLogNFe.Text.Length > 99000) txtLogNFe.Text = txtLogNFe.Text.Substring(0, 96000);
            txtLogNFe.Text = DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff") + " - " + Mensagem + Environment.NewLine + txtLogNFe.Text;
        }

        #endregion
    }
}