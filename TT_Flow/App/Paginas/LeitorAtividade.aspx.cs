using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using TT.FrameWork;

namespace TT_Flow.App.Paginas
{
    public partial class LeitorAtividade : System.Web.UI.Page
    {
        string sProcedureFabricacao = "sp_Manipula_tbl_Flow_WMS_Fabricacao";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ScriptManager.GetCurrent(this) == null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ScriptMgrError", "showFrontendError('Erro interno: ScriptManager não encontrado na página. Verifique a configuração ASP.NET.');", true);
                return;
            }

            pnlLoadingMessage.Visible = false;
            pnlErrorMessage.Visible = false;
            activityDetails.Visible = false;

            pnlProjectTitle.Visible = false;
            pnlRequestNumber.Visible = false;
            pnlParentActivityTitle.Visible = false;
            pnlActivityTitle.Visible = false;
            pnlActivityStatus.Visible = false;

            if (!IsPostBack)
            {
                string idOrUuidParam = Request.QueryString["id"];

                if (!string.IsNullOrEmpty(idOrUuidParam))
                {
                    int idAtividade;
                    Guid uuidAtividade;

                    if (int.TryParse(idOrUuidParam, out idAtividade))
                    {
                        hdnActivityId.Value = idAtividade.ToString();
                        ConsultarEExibirAtividade(idAtividade: idAtividade);
                    }
                    else if (Guid.TryParse(idOrUuidParam, out uuidAtividade))
                    {
                        hdnActivityUuid.Value = uuidAtividade.ToString();
                        ConsultarEExibirAtividade(uuidAtividade: uuidAtividade);
                    }
                    else
                    {
                        ExibirMensagemFrontend($"O valor na URL '{idOrUuidParam}' não é um ID numérico ou um GUID de atividade válido.",true);
                    }
                }
                else
                {
                    ExibirMensagemFrontend("Aguardando leitura de QR Code de usuário ou ID/UUID de atividade na URL.", true);
                    UpdateScannerButtonLabel(1);
                }
            }
        }

        private void ConsultarEExibirAtividade(int idAtividade = 0, Guid uuidAtividade = default)
        {
            try
            {
                pnlLoadingMessage.Visible = true;
                pnlErrorMessage.Visible = false;
                activityDetails.Visible = false;

                var parametros = new Dictionary<string, string>
                {
                    { "sFuncao", "CONSULTAR-ATIVIDADE-QRCODE" }
                };

                if (idAtividade > 0)
                {
                    parametros.Add("idAtividade", idAtividade.ToString());
                }
                else if (uuidAtividade != Guid.Empty)
                {
                    parametros.Add("uuidAtividade", uuidAtividade.ToString());
                }
                else
                {
                    ExibirMensagemFrontend("Nenhum ID ou UUID de atividade fornecido para consulta.", true);
                    return;
                }

                DataTable dtResult = BD.ExecutarDataTable(sProcedureFabricacao, parametros);

                if (dtResult != null && dtResult.Rows.Count > 0)
                {
                    DataRow dr = dtResult.Rows[0];

                    Func<object, string> GetCleanValue = (value) =>
                    {
                        string s = value?.ToString();
                        return string.IsNullOrWhiteSpace(s) ? string.Empty : s.Trim();
                    };

                    string projectTitle = GetCleanValue(dr["sDscTituloProjeto"]);
                    if (!string.IsNullOrEmpty(projectTitle))
                    {
                        litProjectTitle.Text = projectTitle;
                        pnlProjectTitle.Visible = true;
                    }

                    string requestNumber = GetCleanValue(dr["sNumeroRequisicao"]);
                    if (!string.IsNullOrEmpty(requestNumber))
                    {
                        litRequestNumber.Text = requestNumber;
                        pnlRequestNumber.Visible = true;
                    }

                    string parentActivityTitle = GetCleanValue(dr["sDscTituloAtividadePai"]);
                    if (!string.IsNullOrEmpty(parentActivityTitle))
                    {
                        litParentActivityTitle.Text = parentActivityTitle;
                        pnlParentActivityTitle.Visible = true;
                    }

                    string activityTitle = GetCleanValue(dr["sDscTituloAtividade"]);
                    if (!string.IsNullOrEmpty(activityTitle))
                    {
                        litActivityTitle.Text = activityTitle;
                        pnlActivityTitle.Visible = true;
                    }

                    string activityStatus = GetCleanValue(dr["StatusAtividade"]);
                    if (!string.IsNullOrEmpty(activityStatus))
                    {
                        litActivityStatus.Text = activityStatus;
                        pnlActivityStatus.Visible = true;
                    }

                    int idStatusAtividade = 0;
                    if (dr["idStatus"] != DBNull.Value)
                    {
                        idStatusAtividade = Convert.ToInt32(dr["idStatus"]);

                        if(idStatusAtividade == 2)
                        {
                            pnFinalizar.Visible = true;
                        }
                        else
                        {
                            pnFinalizar.Visible = false;
                        }
                    }

                    hdnActivityId.Value = dr["idAtividade"].ToString();
                    UpdateScannerButtonLabel(idStatusAtividade);


                    activityDetails.Visible = true;
                    pnlLoadingMessage.Visible = false;
                }
                else
                {
                    ExibirMensagemFrontend("Nenhuma atividade encontrada com o código fornecido na URL.", true);
                    UpdateScannerButtonLabel(1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no Code-Behind: {ex.ToString()}");
                ExibirMensagemFrontend($"Erro ao consultar detalhes da atividade: {ex.Message}. Por favor, verifique o log do servidor e a procedure SQL.", true);
                UpdateScannerButtonLabel(1);
            }
            finally
            {
                pnlLoadingMessage.Visible = false;
            }
        }

        private void UpdateScannerButtonLabel(int idStatus)
        {
            string buttonText = "Ler QR Code Para Iniciar";

            if (idStatus == 1)
            {
                buttonText = "Ler QR Code Para Iniciar";
            }
            else if (idStatus == 2)
            {
                buttonText = "Ler QR Code Para Apontar";
            }
            else if(idStatus == 5)
            {
                buttonText = "Ler QR Code";

                if (idStatus == 5)
                {
                    divApontador.Visible = false;
                }
                else
                {
                    divApontador.Visible = true;
                }
            }
                scannerButtonText.InnerText = buttonText;
        }

        protected void cmdApontarHora_Click(object sender, EventArgs e)
        {

            var parametros = new Dictionary<string, string>
            {
                { "sFuncao", "APONTAR-HORA" },
                { "idUsuario",  hdnQRCode.Value},
                { "idAtividade", hdnActivityId.Value},
                //{"sObservacao", txtObservacao.Text},
                //{"nHoras", txtHoras.Text}
            };

            if (!string.IsNullOrEmpty(hdnActivityUuid.Value))
            {
                if (Guid.TryParse(hdnActivityUuid.Value, out Guid parsedGuid))
                {
                    parametros.Add("uuidAtividade", hdnActivityUuid.Value);
                }
                else
                {
                    ExibirMensagemFrontend("Erro: O QR Code não contém um UUID válido.", true);
                }

            }

            if (chkFinalizar.Checked)
            {
                parametros.Add("@sFinaliza", "S");
            }
            else
            {
                parametros.Add("@sFinaliza", "N");
            }

            int numero;
            bool ehInteiro = int.TryParse(hdnQRCode.Value, out numero);

            if (ehInteiro)
            {
                try
                {
                    DataTable dtResult = BD.ExecutarDataTable(sProcedureFabricacao, parametros);

                    if (dtResult != null && dtResult.Rows.Count > 0)
                    {
                        DataRow dr = dtResult.Rows[0];
                        string mensagem = "Operação de apontamento concluída.";
                        bool isError = true;
                        if (dr.Table.Columns.Contains("MensagemSucesso"))
                        {
                            isError = false;
                            mensagem = dr["MensagemSucesso"]?.ToString();
                        }

                        if (dr.Table.Columns.Contains("MensagemErro"))
                            mensagem = dr["MensagemErro"]?.ToString();

                        int idAtividade;
                        if (int.TryParse(hdnActivityId.Value, out idAtividade) && idAtividade > 0)
                        {
                            ConsultarEExibirAtividade(idAtividade: idAtividade);
                        }
                        else if (Guid.TryParse(hdnActivityUuid.Value, out Guid uuidAtividade) && uuidAtividade != Guid.Empty)
                        {
                            ConsultarEExibirAtividade(uuidAtividade: uuidAtividade);
                        }

                        ExibirMensagemFrontend(mensagem, isError);
                    }
                    else
                    {
                        ExibirMensagemFrontend("Apontamento de horas concluído, mas sem retorno de mensagem da procedure.", false);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao executar procedure de apontamento de horas: {ex.ToString()}");
                    ExibirMensagemFrontend($"Erro ao apontar horas: {ex.Message}. Verifique o log do servidor.", true);
                }
            }
            else
            {
                ExibirMensagemFrontend("Escaneie um QR Código válido de usuário e que esteja de atribuído no projeto.", true);
            }
         
        }

        //protected void qrCodeInput_TextChanged(object sender, EventArgs e)
        //{
        //    if (!string.IsNullOrEmpty(qrCodeInput.Text))
        //    {
        //        if (hdnQRCode.Value != qrCodeInput.Text)
        //        {
        //            hdnQRCode.Value = qrCodeInput.Text;
        //        }

        //        cmdApontarHora_Click(sender, e);

        //        pnlApontamentoFields.Visible = true;
        //    }
        //    else
        //    {
        //        pnlApontamentoFields.Visible = false;
        //    }
        //}
        protected void hdnQRCode_ValueChanged(object sender, EventArgs e)
        {         
             //cmdApontarHora_Click(sender, e);
            ScriptManager.RegisterStartupScript(this, GetType(), "ToggleApontamento", "window.toggleApontamentoFieldsClient(true);", true);
        }
        private void ExibirMensagemFrontend(string message, bool isError)
        {                  
            if (!isError)
            {
                pnlErrorMessage.CssClass = "success-message";
            }
            else
            {
                pnlErrorMessage.CssClass = "error-message";
            }

            pnlLoadingMessage.Visible = false;
            pnlErrorMessage.Visible = true;
            litErrorMessage.Text = message;
        }
    }
}