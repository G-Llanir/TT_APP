using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using ARQUIVO = TT.FrameWork.Arquivo;

namespace TT_Flow.app.Paginas
{
    public partial class Pedidos_Detalhe_Acoes : Page
    {
        #region | Construtores

        string sProcedure = "sp_Manipula_tbl_Flow_Pedidos";
        int nView_Selecao = 1;
        int nView_EmBranco = 0;

        int nView_Confirmacao = 2;
        int nView_CancelarPedido = 3;
        int nView_AlterarDepartamento = 4;
        int nView_Aceitar_RejeitarPedido = 5;
        int nView_Comentario = 5;
        int nView_EnviarArquivo = 6;
        int nView_AlterarDatas = 5;
        int nView_SolicitarDocumentosSTSO = 7;
        int nView_GerarCentroDeCusto = 8;
        int nView_GerarMedicao = 9;

        #endregion

        #region | Inicialização da Página

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidarPermissaoAcesso();
            string idTipo = "2";

            if (!IsPostBack)
            {
                if (Request["idTipo"] != null)
                    idTipo = Request["idTipo"].ToString();

                MontarComboAcoes(idTipo, Request["idPedidoVinculado"]);
                if (Request["id"] != null)
                {
                    FUNCOES.Popula_Combo(ddlidMotivoCancelamento, "sp_Select 'FLOW_Motivos', @sPesquisa='CancelarPedido'", "idMotivo", "sDscMotivo", false, "Selecione o Motivo", "0");

                    PesquisarPedido(Request["id"].ToString());
                    BaseMultiView.ActiveViewIndex = nView_Selecao;
                }
                else
                    PesquisarPedido("0");
            }
        }

        protected void PesquisarPedido(string idPedido)
            => hddidPedido.Value = idPedido;

        #endregion

        #region | Utils

        void MontarComboAcoes(string idTipo, string idPedido_Vinculado)
        {
            idPedido_Vinculado = string.IsNullOrEmpty(idPedido_Vinculado) ? "0" : idPedido_Vinculado;

            string sDscTipo = "Pedido";

            if (idTipo == "3")
                sDscTipo = "Importação";

            hddidTipo.Value = idTipo;
            hddsDscTipo.Value = sDscTipo;

            ddlAcao.Items.Add(new System.Web.UI.WebControls.ListItem("Escolha uma ação", ""));
            ddlAcao.Items.Add(new System.Web.UI.WebControls.ListItem("Adicionar um Comentário", "Comentario"));

            if (FUNCOES.ValidaPermissao(Permissao.Pedidos.AlterarDepartamento))
                ddlAcao.Items.Add(new System.Web.UI.WebControls.ListItem(string.Format("Enviar {0} para um departamento", sDscTipo), "AlterarDepartamento"));

            if (FUNCOES.ValidaPermissao(Permissao.Pedidos.CancelarPedido))
                ddlAcao.Items.Add(new System.Web.UI.WebControls.ListItem(string.Format("Cancelar {0}", sDscTipo), "CancelarPedido"));

            if (idTipo == "3" || idTipo == "6")
            {
                if (FUNCOES.ValidaPermissao(Permissao.Pedidos.AlterarPrevisaoEntrega))
                    ddlAcao.Items.Add(new System.Web.UI.WebControls.ListItem("Alterar Previsão Chegada Destino", "AlterarPrevisaoEntrega"));
            }
            else
            {
                if (FUNCOES.ValidaPermissao(Permissao.Pedidos.AlterarPrevisaoEntrega))
                    ddlAcao.Items.Add(new System.Web.UI.WebControls.ListItem("Alterar Previsão de Entrega", "AlterarPrevisaoEntrega"));

            }

            if (FUNCOES.ValidaPermissao(Permissao.Pedidos.AlterarEstimativaEntrega))
                ddlAcao.Items.Add(new System.Web.UI.WebControls.ListItem("Alterar Estimativa de Entrega", "AlterarEstimativaEntrega"));

            if (Request["sGerarTarefas"] != null)
            {
                if (Request["sGerarTarefas"] == "S")
                    ddlAcao.Items.Add(new System.Web.UI.WebControls.ListItem("Gerar Tarefas", "GerarTarefas"));
            }

            if (Request["sGerarRecusos"] != null)
            {
                if (Request["sGerarRecursos"] == "S")
                    ddlAcao.Items.Add(new System.Web.UI.WebControls.ListItem("Gerar Recursos", "GerarRecursos"));
            }

            if (FUNCOES.ValidaPermissao(Permissao.Pedidos.AlterarStatusPedido))
                ddlAcao.Items.Add(new System.Web.UI.WebControls.ListItem(string.Format("Alterar o Status {0}", sDscTipo), "AlterarStatus"));

            if (idPedido_Vinculado != "0")
                ddlAcao.Items.Add(new System.Web.UI.WebControls.ListItem("Reabrir o Orçamento Vinculado", "ReabrirOrcamento"));

            if (Request["STSO"] == "S")
            {
                if (FUNCOES.ValidaPermissao(Permissao.Pedidos.SolicitarDocumentosSTSO))
                    ddlAcao.Items.Add(new System.Web.UI.WebControls.ListItem("Solicitar Documentos STSO", "SolicitarDocumentosSTSO"));
            }

            if (idTipo == "2")
            {
                if (FUNCOES.ValidaPermissao(Permissao.Pedidos.GerarCentroDeCusto))
                    ddlAcao.Items.Add(new System.Web.UI.WebControls.ListItem("Gerar Centro de Custo", "GerarCentroDeCusto"));

                if (FUNCOES.ValidaPermissao(Permissao.Pedidos.GerarMedicao))
                    ddlAcao.Items.Add(new System.Web.UI.WebControls.ListItem("Gerar Medição", "GerarMedicao"));
            }
        }

        void AlterarDepartamento_Popular()
        {
            FUNCOES.Popula_Combo(ddlidAlterarDepartamento, "sp_Select 'Flow_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Selecione um Departamento", "0");

            rdlStatus_AlterarDepartamento.Items.Clear();
            rdlStatus_AlterarDepartamento.Items.Add(new System.Web.UI.WebControls.ListItem("Não alterar Status", "Não Altera"));
            rdlStatus_AlterarDepartamento.Items.Add(new System.Web.UI.WebControls.ListItem("Seguir Status do Fluxo", "Segue Fluxo"));

            if (FUNCOES.ValidaPermissao(Permissao.Pedidos.AlterarStatusPedido))
                rdlStatus_AlterarDepartamento.Items.Add(new System.Web.UI.WebControls.ListItem("Definir Status", "Escolhe Status"));

            FUNCOES.Popula_Combo(ddlidStatus_AlterarDepartamento, "sp_Select 'Flow_Status_Departamento'", "idStatus", "sDscStatus", false, "Selecione um Status", "0");
            ddlidStatus_AlterarDepartamento.Visible = false;

        }

        void Aceitar_RejeitarPedido_Popular()
        {
            string sFuncao = "";
            divAceitarRejeitar_Departamento.Visible = true;
            divAceitarRejeitar_datas.Visible = false;

            if (ddlAcao.SelectedValue == "EnviarProximoDepartamento")
            {
                lblAceitarRejeitar_Titulo.Text = "Enviar para o próximo departamento do Fluxo - Departamento Atual: ";
                lblAceitarRejeitar_Motivo.Text = "Observação";
                sFuncao = "CONSULTA_PROXIMO_DEPARTAMENTO";

            }
            else if (ddlAcao.SelectedValue == "RejeitarPedido")
            {
                lblAceitarRejeitar_Titulo.Text = "Rejeição no Departamento de ";
                lblAceitarRejeitar_Motivo.Text = "Motivo do Rejeite";
                sFuncao = "CONSULTA_REJEITAR_PEDIDO";
            }

            Dictionary<string, string> vParametrosItem = new Dictionary<string, string>
            {
                 {"@sFuncao",                    sFuncao},
                 {"@idPedido",                   hddidPedido.Value},
            };
            DataSet dsItem = BD.ExecutarDataSet(sProcedure, vParametrosItem);

            if (BD.ValidarDataSet(dsItem, out string sErro))
            {
                txtAceitarRejeitar_sDscDepartamento.Text = RETORNO.DATASET(dsItem, "sDscDepartamento_Proximo");
                lblAceitarRejeitar_Titulo.Text = lblAceitarRejeitar_Titulo.Text + " " + RETORNO.DATASET(dsItem, "sDscDepartamento_Atual");
                txtAceitarRejeitar_Motivo.Text = "";
                txtAceitarRejeitar_Motivo.Focus();
            }
        }

        void Comentario_Popular()
        {
            lblAceitarRejeitar_Titulo.Text = "Informar um Comentário ";
            lblAceitarRejeitar_Motivo.Text = "Comentário";
            divAceitarRejeitar_Departamento.Visible = false;
            divAceitarRejeitar_datas.Visible = false;
            txtAceitarRejeitar_Motivo.Text = "";
            txtAceitarRejeitar_Motivo.Focus();
        }

        void AlterarDatas_Popular(string sFuncao)
        {
            if (sFuncao == "AlterarPrevisaoEntrega")
                lblAceitarRejeitar_Titulo.Text = "Alterar Previsão de Entrega ";
            else if (sFuncao == "AlterarEstimativaEntrega")
                lblAceitarRejeitar_Titulo.Text = "Alterar Estimativa de Entrega";

            lblAceitarRejeitar_Motivo.Text = "Motivo";
            divAceitarRejeitar_Departamento.Visible = false;
            divAceitarRejeitar_datas.Visible = true;
            txtDatas.Text = "";
            txtAceitarRejeitar_Motivo.Text = "";
        }

        void GerarTarefas_Popular()
        =>
            lblMensagemConfirmacao.Text = string.Format("Gerar Tarefas para {1} n.º {0}", hddidPedido.Value.ToString().PadLeft(6, '0'), hddsDscTipo.Value);

        bool AplicarValidacoes(string sAcao)
        {
            if (sAcao == "CancelarPedido")
            {
                if (ddlidMotivoCancelamento.SelectedValue == "0")
                {
                    MensagemPagina.MostraMensagem_Erro("Selecione o motivo do Cancelamento!");
                    return false;
                }
                else if (string.IsNullOrEmpty(txtMotivoCancelamento.Text))
                {
                    MensagemPagina.MostraMensagem_Erro("Informe uma observação!");
                    txtMotivoCancelamento.Focus();
                    return false;
                }

                lblMensagemConfirmacao.Text = string.Format("Confirma o Cancelamento do {1} n.º{0}?", hddidPedido.Value, hddsDscTipo.Value);
                FrameWork.cls_Variaveis.nView_Voltar = nView_CancelarPedido;
            }
            else if (sAcao == "AlterarDepartamento")
            {
                if (ddlidAlterarDepartamento.SelectedValue == "0")
                {
                    MensagemPagina.MostraMensagem_Erro("Selecione o Departamento!");
                    return false;
                }
                else if (txtSObservacao_AlterarDepartamento.Text.Length < 8)
                {
                    MensagemPagina.MostraMensagem_Erro("Informe uma observação válida!");
                    txtSObservacao_AlterarDepartamento.Focus();
                    return false;
                }
                else if (rdlStatus_AlterarDepartamento.SelectedValue == "Escolhe Status")
                {
                    if (ddlidStatus_AlterarDepartamento.SelectedValue == "0")
                    {
                        MensagemPagina.MostraMensagem_Erro("Selecione um Status!");
                        txtSObservacao_AlterarDepartamento.Focus();
                        return false;
                    }
                }

                lblMensagemConfirmacao.Text = string.Format("Confirma a alteração para o departamento {1} do {2} n.º{0}?", hddidPedido.Value, ddlidAlterarDepartamento.SelectedItem, hddsDscTipo.Value);
                FrameWork.cls_Variaveis.nView_Voltar = nView_AlterarDepartamento;
            }
            else if (sAcao == "EnviarProximoDepartamento")
            {
                if (txtAceitarRejeitar_Motivo.Text.Length < 8)
                {
                    MensagemPagina.MostraMensagem_Erro("Informe um motivo válido para o envio para o próximo departamento!");
                    txtAceitarRejeitar_Motivo.Focus();
                    return false;
                }
                lblMensagemConfirmacao.Text = string.Format("Confirma o envio do {2} n.º {0} para o Departamento {1}?", hddidPedido.Value.PadLeft(6, '0'), txtAceitarRejeitar_sDscDepartamento.Text, hddsDscTipo.Value);
                FrameWork.cls_Variaveis.nView_Voltar = nView_Aceitar_RejeitarPedido;
            }
            else if (sAcao == "RejeitarPedido")
            {
                if (txtAceitarRejeitar_Motivo.Text.Length < 8)
                {
                    MensagemPagina.MostraMensagem_Erro("Informe um motivo válido a para rejeição!");
                    txtAceitarRejeitar_Motivo.Focus();
                    return false;
                }
                lblMensagemConfirmacao.Text = string.Format("Confirma o envio do {2} n.º {0} para o Departamento {1}?", hddidPedido.Value.PadLeft(6, '0'), txtAceitarRejeitar_sDscDepartamento.Text, hddsDscTipo.Value);
                FrameWork.cls_Variaveis.nView_Voltar = nView_Aceitar_RejeitarPedido;
            }
            else if (sAcao == "Comentario")
            {
                if (txtAceitarRejeitar_Motivo.Text.Length < 10)
                {
                    MensagemPagina.MostraMensagem_Erro("Informe um Comentário válido!");
                    txtAceitarRejeitar_Motivo.Focus();
                    return false;
                }
                lblMensagemConfirmacao.Text = string.Format("Confirma o envio do seguinte comentário: </br> {0}?", txtAceitarRejeitar_Motivo.Text);
            }
            else if (sAcao == "EnviarArquivo")
            {
                if (!fu_Arquivo.HasFile)
                {
                    MensagemPagina.MostraMensagem_Erro("Selecione um arquivo para enviar!");
                    fu_Arquivo.Focus();
                    return false;
                }
                if (txtEnviarArquivo_sDscArquivo.Text.Length < 10)
                {
                    MensagemPagina.MostraMensagem_Erro("Informe uma Descrição do Arquivo válida!");
                    txtEnviarArquivo_sDscArquivo.Focus();
                    return false;
                }

                lblMensagemConfirmacao.Text = string.Format("Confirma o envio do Arquivo {0}?", fu_Arquivo.PostedFile.FileName);
            }
            else if (sAcao == "AlterarPrevisaoEntrega")
            {
                if (txtAceitarRejeitar_Motivo.Text.Length < 10)
                {
                    MensagemPagina.MostraMensagem_Erro("Informe um Motivo válido!");
                    txtAceitarRejeitar_Motivo.Focus();
                    return false;
                }

                DateTime data = DateTime.MinValue;
                if (!string.IsNullOrEmpty(txtDatas.Text))
                {
                    if (!DateTime.TryParse(this.txtDatas.Text.Trim(), out data))
                    {
                        MensagemPagina.MostraMensagem_Erro("Data inválida!");
                        return false;
                    }
                }

                lblMensagemConfirmacao.Text = string.Format("Confirma a alteração da Previsão de entrega para {0}?", data.ToString("dd/MM/yyyy"));
            }
            else if (sAcao == "AlterarEstimativaEntrega")
            {
                if (txtAceitarRejeitar_Motivo.Text.Length < 10)
                {
                    MensagemPagina.MostraMensagem_Erro("Informe um Motivo válido!");
                    txtAceitarRejeitar_Motivo.Focus();
                    return false;
                }

                DateTime data = DateTime.MinValue;
                if (!string.IsNullOrEmpty(txtDatas.Text))
                {
                    if (!DateTime.TryParse(this.txtDatas.Text.Trim(), out data))
                    {
                        MensagemPagina.MostraMensagem_Erro("Data inválida!");
                        return false;
                    }
                }

                lblMensagemConfirmacao.Text = string.Format("Confirma a alteração da Estimativa de entrega para {0}?", data.ToString("dd/MM/yyyy"));
            }

            return true;
        }

        #endregion

        #region | Eventos

        protected void ddlAcao_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlAcao.SelectedValue == "CancelarPedido")
                BaseMultiView.ActiveViewIndex = nView_CancelarPedido;
            else if (ddlAcao.SelectedValue == "AlterarDepartamento")
            {
                AlterarDepartamento_Popular();
                BaseMultiView.ActiveViewIndex = nView_AlterarDepartamento;
            }
            else if (ddlAcao.SelectedValue == "EnviarProximoDepartamento")
            {
                Aceitar_RejeitarPedido_Popular();
                BaseMultiView.ActiveViewIndex = nView_Aceitar_RejeitarPedido;
            }
            else if (ddlAcao.SelectedValue == "RejeitarPedido")
            {
                Aceitar_RejeitarPedido_Popular();
                BaseMultiView.ActiveViewIndex = nView_Aceitar_RejeitarPedido;
            }
            else if (ddlAcao.SelectedValue == "Comentario")
            {
                Comentario_Popular();
                BaseMultiView.ActiveViewIndex = nView_Comentario;
            }
            else if (ddlAcao.SelectedValue == "EnviarArquivo")
                BaseMultiView.ActiveViewIndex = nView_EnviarArquivo;
            else if (ddlAcao.SelectedValue == "AlterarPrevisaoEntrega")
            {
                AlterarDatas_Popular("AlterarPrevisaoEntrega");
                BaseMultiView.ActiveViewIndex = nView_AlterarDatas;
            }
            else if (ddlAcao.SelectedValue == "AlterarEstimativaEntrega")
            {
                AlterarDatas_Popular("AlterarEstimativaEntrega");
                BaseMultiView.ActiveViewIndex = nView_AlterarDatas;
            }
            else if (ddlAcao.SelectedValue == "GerarTarefas")
            {
                GerarTarefas_Popular();
                BaseMultiView.ActiveViewIndex = nView_Confirmacao;
            }
            else if (ddlAcao.SelectedValue == "ReabrirOrcamento")
            {
                MensagemPagina.MostraMensagem_Aviso("<b>Aviso:</b> para Reabrir o Orçamento vinculado ao Pedido, todas as Tarefas do Pedido serão reiniciadas!");
                lblMensagemConfirmacao.Text = string.Format("Confirma a reabertura do Orçamento?");

                BaseMultiView.ActiveViewIndex = nView_Confirmacao;
            }
            else if (ddlAcao.SelectedValue == "SolicitarDocumentosSTSO")
            {
                FUNCOES.Popula_Combo(ddlidUsuario, "sp_Select 'Usuarios'", "idUsuario", "sDscUsuario", false, "Selecione o Usuário", "0");
                BaseMultiView.ActiveViewIndex = nView_SolicitarDocumentosSTSO;
            }
            else if (ddlAcao.SelectedValue == "GerarCentroDeCusto")
            {
                txtnValorTeto.Text = "0";
                string script = $@"
                                jQuery.noConflict();
                                (function($) {{
                                    $(document).ready(function() {{
                                        $('#{txtnValorTeto.ClientID}').mask('0.000.000.009,99', {{ reverse: true }});
                                    }});
                                }})(jQuery);";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "MascaraValorTeto", script, true);
                MensagemPagina.MostraMensagem("Informe um Valor Teto para o novo Centro de Custo", "info", false);
                BaseMultiView.ActiveViewIndex = nView_GerarCentroDeCusto;
            }
            else if (ddlAcao.SelectedValue == "GerarMedicao")
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("$('[id*=txtsPorcentagem]').mask('009,99', { reverse: true });");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptGerarMedicao", sb.ToString(), true);
                BaseMultiView.ActiveViewIndex = nView_GerarMedicao;

                DataSet dsPesquisaCONSULTAR;
                Dictionary<String, String> vParametrosCONSULTAR = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_MEDICAO" },
                        { "@idPedido", hddidPedido.Value}
                    };

                dsPesquisaCONSULTAR = BD.ExecutarDataSet(sProcedure, vParametrosCONSULTAR);

                if (dsPesquisaCONSULTAR.Tables[0].Rows.Count > 0)
                {
                    gvMedicao.DataSource = dsPesquisaCONSULTAR;
                    gvMedicao.DataBind();
                }
            }
            else
                BaseMultiView.ActiveViewIndex = nView_EmBranco;
        }

        protected void cmdConfirmar_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> vParametrosItem = new Dictionary<string, string>();
            DataSet dsItem;
            string sMsgConfirmacao = "";
            string sMsgErro = "";

            switch (ddlAcao.SelectedValue)
            {
                case "CancelarPedido":
                    vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao",                    "CANCELAR PEDIDO"},
                        {"@idPedido",                   hddidPedido.Value},
                        {"@idMotivoCancelamento",       ddlidMotivoCancelamento.SelectedValue},
                        {"@sDscMotivo",                 txtMotivoCancelamento.Text},
                        {"@idUsuarioAtualizacao",       IDENTITY.Variaveis.idUsuario()}
                    };
                    FrameWork.cls_Variaveis.nView_Voltar = nView_CancelarPedido;
                    sMsgConfirmacao = "Cancelamento efetuado com sucesso!";
                    break;
                case "AlterarDepartamento":
                    vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao",                    "ALTERAR DEPARTAMENTO"},
                        {"@idPedido",                   hddidPedido.Value},
                        {"@idDepartamento",             ddlidAlterarDepartamento.SelectedValue},
                        {"@sDscMotivo",                 txtSObservacao_AlterarDepartamento.Text},
                        {"@sAlteraStatus",              rdlStatus_AlterarDepartamento.SelectedValue},
                        {"@idStatus",                   ddlidStatus_AlterarDepartamento.SelectedValue},
                        {"@idUsuarioAtualizacao",       IDENTITY.Variaveis.idUsuario()}
                    };
                    FrameWork.cls_Variaveis.nView_Voltar = nView_CancelarPedido;
                    sMsgConfirmacao = "Alteração de Departamento efetuada com sucesso!";
                    break;
                case "EnviarProximoDepartamento":
                    vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao",                    "ENVIAR_PROXIMO_DEPARTAMENTO"},
                        {"@idPedido",                   hddidPedido.Value},
                        {"@sDscMotivo",                 txtAceitarRejeitar_Motivo.Text},
                        {"@idUsuarioAtualizacao",       IDENTITY.Variaveis.idUsuario()}
                    };
                    FrameWork.cls_Variaveis.nView_Voltar = nView_Aceitar_RejeitarPedido;
                    sMsgConfirmacao = "Envio efetuado com sucesso!";
                    break;
                case "RejeitarPedido":
                    vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao",                    "REJEITAR_PEDIDO"},
                        {"@idPedido",                   hddidPedido.Value},
                        {"@sDscMotivo",                 txtAceitarRejeitar_Motivo.Text},
                        {"@idUsuarioAtualizacao",       IDENTITY.Variaveis.idUsuario()}
                    };
                    FrameWork.cls_Variaveis.nView_Voltar = nView_Aceitar_RejeitarPedido;
                    sMsgConfirmacao = "Envio efetuado com sucesso!";
                    break;
                case "Comentario":
                    vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao",                    "COMENTARIO"},
                        {"@idPedido",                   hddidPedido.Value},
                        {"@sDscMotivo",                 txtAceitarRejeitar_Motivo.Text},
                        {"@idUsuarioAtualizacao",       IDENTITY.Variaveis.idUsuario()}
                    };
                    FrameWork.cls_Variaveis.nView_Voltar = nView_Aceitar_RejeitarPedido;
                    sMsgConfirmacao = "Comentário efetuado com sucesso!";
                    break;

                case "EnviarArquivo":

                    byte[] lObjArquivo = null;
                    Stream lObjConteudoArquivo = fu_Arquivo.PostedFile.InputStream;
                    string sNomeArquivo = fu_Arquivo.FileName;
                    string lStrCaminhoArquivo = fu_Arquivo.PostedFile.FileName;
                    string lStrNomeArquivo = Path.GetFileName(lStrCaminhoArquivo);

                    try
                    {
                        ARQUIVO objArquivo = new ARQUIVO();
                        lObjArquivo = objArquivo.TransformaArquivoEmArrayBytes(lStrNomeArquivo, lStrCaminhoArquivo, lObjConteudoArquivo);
                    }
                    catch
                    {
                        return;
                    }

                    vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao",                    "INCLUIR" },
                        {"@idTipoArquivo",              "1" },
                        {"@idObjeto",                   hddidPedido.Value},
                        {"@sDscArquivo",                txtEnviarArquivo_sDscArquivo.Text},
                        {"@bArquivo", lObjArquivo.ToString() },
                        {"@sNomeArquivo",               sNomeArquivo},
                        {"@idUsuario",                  IDENTITY.Variaveis.idUsuario()}
                    };
                    FrameWork.cls_Variaveis.nView_Voltar = nView_EnviarArquivo;
                    sMsgConfirmacao = "Arquivo Enviado com sucesso!";
                    sProcedure = "sp_Manipula_tbl_Flow_Arquivos";
                    break;

                case "AlterarPrevisaoEntrega":
                    vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao",                    "ALTERAR_PREVISAO_ENTREGA"},
                        {"@idPedido",                   hddidPedido.Value},
                        {"@sDscMotivo",                 txtAceitarRejeitar_Motivo.Text},
                        {"@dtPrevisaoEntrega",          DateTime.Parse(txtDatas.Text).ToString("dd/MM/yyyy")},
                        {"@idUsuarioAtualizacao",       IDENTITY.Variaveis.idUsuario()}
                    };
                    FrameWork.cls_Variaveis.nView_Voltar = nView_AlterarDatas;
                    sMsgConfirmacao = "Alteração de Previsão de entrega efetuada com sucesso!";
                    break;

                case "AlterarEstimativaEntrega":
                    vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao",                    "ALTERAR_ESTIMATIVA_ENTREGA"},
                        {"@idPedido",                   hddidPedido.Value},
                        {"@sDscMotivo",                 txtAceitarRejeitar_Motivo.Text},
                        {"@dtEstimativaEntrega",        DateTime.Parse(txtDatas.Text).ToString("dd/MM/yyyy")},
                        {"@idUsuarioAtualizacao",       IDENTITY.Variaveis.idUsuario()}
                    };
                    FrameWork.cls_Variaveis.nView_Voltar = nView_AlterarDatas;
                    sMsgConfirmacao = "Alteração de Estimativa de entrega efetuada com sucesso!";
                    break;

                case "GerarTarefas":
                    vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao",                    "GERAR TAREFAS"},
                        {"@idPedido",                   hddidPedido.Value},
                        {"@idUsuario",                  IDENTITY.Variaveis.idUsuario()}
                    };
                    FrameWork.cls_Variaveis.nView_Voltar = nView_Selecao;
                    sProcedure = "sp_Manipula_tbl_Flow_Pedidos_x_Tarefas";
                    sMsgConfirmacao = "Tarefas geradas com sucesso!";
                    break;

                case "ReabrirOrcamento":
                    vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao",                    "REABRIR_ORCAMENTO"},
                        {"@idPedido",                   hddidPedido.Value},
                        {"@idUsuario",                  IDENTITY.Variaveis.idUsuario()}
                    };
                    FrameWork.cls_Variaveis.nView_Voltar = nView_Selecao;
                    sProcedure = "sp_Manipula_tbl_Flow_Pedidos_x_Tarefas";
                    sMsgConfirmacao = "Orçamento reaberto com sucesso!";
                    break;

                case "GerarCentroDeCusto":
                    vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao", "Gerar_CentroDeCusto" },
                        {"@idPedido", hddidPedido.Value },
                        {"@nValorTeto", txtnValorTeto.Text == ""? "0" : txtnValorTeto.Text.Replace(".", "").Replace(",", ".")},
                        {"@idUsuario", IDENTITY.Variaveis.idUsuario()}
                    };
                    FrameWork.cls_Variaveis.nView_Voltar = nView_Selecao;
                    sProcedure = "sp_Manipula_tbl_Flow_Pedidos_x_Tarefas";
                    sMsgConfirmacao = "Centro de Custo gerado com Sucesso!";
                    break;
            }

            dsItem = BD.ExecutarDataSet(sProcedure, vParametrosItem);

            if (BD.ValidarDataSet(dsItem, out string sErro))
            {
                sMsgErro = string.IsNullOrEmpty(RETORNO.DATASET(dsItem, "sMsg")) ? "" : RETORNO.DATASET(dsItem, "sMsg");

                if (sMsgErro != "")
                {
                    MensagemPagina.MostraMensagem_Erro(sMsgErro);
                    ddlAcao.SelectedValue = "";
                    BaseMultiView.ActiveViewIndex = FrameWork.cls_Variaveis.nView_Voltar;
                }
                else
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "AddShowModalScript", string.Format("<script>window.top.location.href = 'Pedidos_Detalhe.aspx?id={0}&sMsg={1}&sTp={2}' </script>", hddidPedido.Value, sMsgConfirmacao, hddidTipo.Value), false);
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro(sErro);
                BaseMultiView.ActiveViewIndex = FrameWork.cls_Variaveis.nView_Voltar;
            }


        }

        protected void cmdAcoes_Executar_Click(object sender, EventArgs e)
        {
            if (AplicarValidacoes(ddlAcao.SelectedValue))
                BaseMultiView.ActiveViewIndex = nView_Confirmacao;
        }

        protected void cmdVoltar_Click(object sender, EventArgs e)
        =>
            BaseMultiView.ActiveViewIndex = FrameWork.cls_Variaveis.nView_Voltar;

        protected void cmdCancelamento_Voltar_Click(object sender, EventArgs e)
        {
            ddlAcao.SelectedValue = "";
            BaseMultiView.ActiveViewIndex = nView_Selecao;
        }

        protected void rdlStatus_AlterarDepartamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlidStatus_AlterarDepartamento.SelectedValue = "0";

            if (rdlStatus_AlterarDepartamento.SelectedValue == "Escolhe Status")
                ddlidStatus_AlterarDepartamento.Visible = true;
            else
                ddlidStatus_AlterarDepartamento.Visible = false;
        }

        protected void btnGerarCentroDeCusto_Click(object sender, EventArgs e)
        {
            lblMensagemConfirmacao.Text = string.Format("Confirma a criação do novo Centro de custo?");
            BaseMultiView.ActiveViewIndex = nView_Confirmacao;
        }

        #endregion

        #region | STSO
        protected void btnConfirmarSolicitar_Click(object sender, EventArgs e)
        {
            string Mensagem = "";
            if (ddlidUsuario.SelectedValue == "0")
            {
                Mensagem = "Selecione o Usuários Destino!";
            }
            if (txtsObservacao.Text.Length < 20)
            {
                Mensagem += (Mensagem != "" ? "</br>" : "") + "Insira uma descrição breve com pelo menos 20 caracteres!";
            }
            if (Mensagem == "")
            {
                try
                {
                    Dictionary<String, String> vParametro = new Dictionary<string, string>();
                    vParametro["@sFuncao"] = "EMAIL_ACOES";
                    vParametro["@idUsuarioInclusao"] = ddlidUsuario.SelectedValue;
                    vParametro["@sObservacao"] = txtsObservacao.Text;
                    vParametro["@idPedido"] = hddidPedido.Value;
                    BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametro);

                    MensagemPagina.MostraMensagem_Sucesso("Email Enviado com Sucesso!");

                    BaseMultiView.ActiveViewIndex = nView_Selecao;
                }
                catch
                {
                    MensagemPagina.MostraMensagem_Erro("Erro ao Enviar o Email!");
                }
            }
            else
            {
                MensagemPagina7.MostraMensagem_Erro(Mensagem);
            }
        }

        protected void bntVoltarSolicitar_Click(object sender, EventArgs e)
        {
            BaseMultiView.ActiveViewIndex = nView_Selecao;
        }

        protected void ddlidUsuario_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlidUsuario.SelectedValue != "0")
            {
                DataSet dsPesquisa;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR_EMAIL_USUARIO");
                vParametros.Add("@idUsuarioInclusao", ddlidUsuario.SelectedValue);

                dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                txtEmail.Text = RETORNO.DATASET(dsPesquisa, 0, "sEmail");
            }
            else
            {
                txtEmail.Text = "";
            }
        }
        #endregion

        protected void btnIncluirMedicao_Click(object sender, EventArgs e)
        {
            if (txtdtMedicao.Text != "" && txtsPorcentagem.Text != "")
            {
                DataSet dsPesquisa;
                Dictionary<String, String> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "INCLUIR_MEDICAO" },
                        { "@dtMedicao", Convert.ToDateTime(txtdtMedicao.Text).ToString("dd/MM/yyyy") },
                        { "@nPorcentagem",  txtsPorcentagem.Text.Replace(",", ".")},
                        { "@idPedido", hddidPedido.Value},
                        { "@sAtivo", "S" },
                        { "@idUsuarioInclusao", IDENTITY.Variaveis.idUsuario() }
                    };

                dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                txtdtMedicao.Text = "";
                txtsPorcentagem.Text = "";
            }
            else
            {
                string mensagem = "";
                if (txtdtMedicao.Text == "")
                    mensagem = "Selecione a data!";
                if (txtsPorcentagem.Text == "")
                    mensagem = "Descreva a porcentagem!";
                MensagemPagina.MostraMensagem_Erro(mensagem);
            }

            DataSet dsPesquisaCONSULTAR;
            Dictionary<String, String> vParametrosCONSULTAR = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_MEDICAO" },
                        { "@idPedido", hddidPedido.Value}
                    };

            dsPesquisaCONSULTAR = BD.ExecutarDataSet(sProcedure, vParametrosCONSULTAR);

            if (dsPesquisaCONSULTAR.Tables[0].Rows.Count > 0)
            {
                gvMedicao.DataSource = dsPesquisaCONSULTAR;
                gvMedicao.DataBind();
            }
        }

        protected void gvMedicao_RowDeleting(object sender, System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            string idMedicao = gvMedicao.DataKeys[e.RowIndex].Value.ToString();

            DataSet dsPesquisaCONSULTAR;
            Dictionary<String, String> vParametrosCONSULTAR = new Dictionary<string, string>
                    {
                        { "@sFuncao", "EXCLUIR_MEDICAO" },
                        { "@idMedicao", idMedicao},
                        { "@idPedido", hddidPedido.Value}
                    };

            dsPesquisaCONSULTAR = BD.ExecutarDataSet(sProcedure, vParametrosCONSULTAR);

            gvMedicao.DataSource = dsPesquisaCONSULTAR;
            gvMedicao.DataBind();
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            ddlAcao.SelectedValue = "";
            BaseMultiView.ActiveViewIndex = nView_Selecao;
        }
    }
}