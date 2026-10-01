using AjaxControlToolkit.HtmlEditor;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT_Flow.App.Controles;
using static TT.FrameWork.Identity;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Flow.App.Paginas.RRHH.Solicitacoes
{
    public enum Status
    {
        EmAnalise = 1,
        AprovadoSupervisor = 2,
        Finalizado = 3,
        RejeitadoSupervisor = 4,
        Cancelado = 5,
        AprovadoRH = 6,
        RejeitadoRH = 7,
        PendenteDocumentacao = 8,
        AnaliseRH = 9,
        AprovadoDiretor = 10,
        RejeitadoDiretor = 11,
        AprovadoTerceiros = 12,
        RejeitadoTerceiros = 13,
        AguardandoNfe = 14,
        AnaliseTerceiros = 15
    }
    public enum Departamentos
    {
        RH = 33,
        Terceiros = 33,
    }
    public partial class Solicitacao_Detalhe : Page
    {
        string sTituloPagina = "Solicitações";
        string sProcedure = "sp_Manipula_tbl_Flow_Solicitacoes";
        string sistema = "TFLOW";

        #region | Funções Incialização do Form

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["id"] != null)
                {
                    Pesquisar(Request["id"].ToString(), false);

                    if (Request["msg"] == "1")
                        MensagemPagina.MostraMensagem_Sucesso("Status Alterado com Sucesso!");
                }
                else
                    Pesquisar("0", true);
            }
            else
            {
                var requestTarget = this.Request["__EVENTTARGET"];
                var requestArgs = this.Request["__EVENTARGUMENT"];

                if (requestTarget == "funcao_SAIR")
                    FUNCOES.DirecionaPagina("/app/dashboard.aspx");
                else if (requestTarget == "funcao_SALVAR")
                    Salvar();
            }
            HTMLEditor_RemoveBottomToolbar_Button(txtsObservacaoSolicitacao, "ctl00", false);
            HTMLEditor_RemoveBottomToolbar_Button(txtsObservacaoSolicitacao, "ctl01", false);
            RegistraScript("");
        }

        #endregion

        #region | Metodos Banco de Dados
        string PegarIdDepartamentoLogado(string idUsuario)
        {
            string sErro = "";
            DataSet dsPesquisa;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR-ID-DEPARTAMENTO");
            vParametros.Add("@idUsuarioLogado", idUsuario);
            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                return RETORNO.DATASET(dsPesquisa, 0, "idDepartamento");
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + sErro);
                return "Departamento Não Encontrado!";
            }
        }

        protected void Pesquisar(string idSolicitacao, bool bEdicao)
        {
            PopularCombos();
            Popular_Combo_id(IDENTITY.Variaveis.idUsuario());
            popularHistorico(idSolicitacao);
            ConsultarOpcoesDatas(idSolicitacao);

            CarregarChatInterno(idSolicitacao);

            string sErro = "";

            try
            {
                LimpaCampos();

                if (idSolicitacao != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR-TODOS");
                    vParametros.Add("@idSolicitacao", idSolicitacao);
                    vParametros.Add("@sSistema", sistema);
                    vParametros.Add("@idUsuarioLogado", IDENTITY.Variaveis.idUsuario());

                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        string descricao = RETORNO.DATASET(dsPesquisa, 0, "sDscSolicitacao");
                        string textoFormatado;

                        if (System.Text.RegularExpressions.Regex.IsMatch(descricao.Trim(), @"^#?\s*\d+\s*-"))
                        {
                            textoFormatado = descricao.Trim().TrimStart('#').Trim();
                        }
                        else
                        {
                            textoFormatado = $"{idSolicitacao} - {descricao.Trim()}";
                        }

                        hddidSolicitacao.Value = RETORNO.DATASET(dsPesquisa, 0, "idSolicitacao");
                        txtidSolicitacao.Text = hddidSolicitacao.Value;
                        txtsDscSolicitacao.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscSolicitacao");
                        BreadCrumb.TitulodaPagina = textoFormatado;
                        lblTituloPagina.Text = "Solicitação - " + textoFormatado;
                        ddlidStatus.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idStatus");
                        ddlidTipo.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idTipo");
                        lblsStatus.Text = RETORNO.DATASET(dsPesquisa, 0, "sStatus");

                        Popular_aba_Arquivo(idSolicitacao, true);
                        divArquivoForm.Visible = Popular_aba_Arquivo(idSolicitacao, true);
                        lblsStatus.CssClass = $"label label-{RETORNO.DATASET(dsPesquisa, 0, "sCor")}";

                        txtsObservacaoSolicitacao.Content = RETORNO.DATASET(dsPesquisa, 0, "sObservacao");

                        var idDepLogado = PegarIdDepartamentoLogado(IDENTITY.Variaveis.idUsuario());
                        string sGestor = VerificaGestor(RETORNO.DATASET(dsPesquisa, 0, "idDepartamento"));

                        // Lógica de visibilidade dos botões de ação
                        pnlAcoes.Visible = false; // Começa como invisível
                        hddsFluxoTerceiro.Value = RETORNO.DATASET(dsPesquisa, 0, "sFluxoTerceiro");

                        if (hddsFluxoTerceiro.Value == "S")
                        {
                            lblDtInicio.InnerText = "Período: ";
                            lblDtFinal.InnerText = "";
                            lblDtFinal.InnerHtml = "&nbsp;";

                            if (RETORNO.DATASET(dsPesquisa, 0, "sLiberaSupervisor") == "S" && sGestor == "S")
                            {
                                pnlAcoes.Visible = true;
                                Div_AprovacaoSupervisor.Visible = true;
                                Div_AprovacaoTerceiros.Visible = false;
                                Div_AprovacaoRH.Visible = false;
                                Div_AprovacaoDiretor.Visible = false;
                            }
                            else if (RETORNO.DATASET(dsPesquisa, 0, "bGerenciar") != "0")
                            {
                                pnlAcoes.Visible = true;
                                Div_AprovacaoSupervisor.Visible = false;
                                Div_AprovacaoRH.Visible = false;
                                Div_AprovacaoTerceiros.Visible = false;
                                Div_AprovacaoDiretor.Visible = true;
                            }
                            else if (RETORNO.DATASET(dsPesquisa, 0, "sLiberaTerceiros") == "S" && idDepLogado == ((int)Departamentos.Terceiros).ToString())
                            {
                                pnlAcoes.Visible = true;
                                Div_AprovacaoTerceiros.Visible = true;
                                Div_AprovacaoSupervisor.Visible = false;
                                Div_AprovacaoRH.Visible = false;
                                Div_AprovacaoDiretor.Visible = false;
                            }
                            else
                            {
                                pnlAcoes.Visible = false;
                                Div_AprovacaoTerceiros.Visible = false;
                                Div_AprovacaoSupervisor.Visible = false;
                                Div_AprovacaoRH.Visible = false;
                                Div_AprovacaoDiretor.Visible = false;
                            }
                        }
                        else
                        {
                            if (RETORNO.DATASET(dsPesquisa, 0, "sLiberaRH") == "S" && idDepLogado == ((int)Departamentos.RH).ToString())
                            {
                                pnlAcoes.Visible = true;
                                Div_AprovacaoRH.Visible = true;
                                Div_AprovacaoSupervisor.Visible = false;
                                Div_AprovacaoTerceiros.Visible = false;
                                Div_AprovacaoDiretor.Visible = false;


                                if (ddlidTipo.SelectedValue != "2")
                                {
                                    if (ddlidStatus.SelectedValue == "6") { div_PDF.Visible = true; }
                                    else { div_PDF.Visible = false; }
                                }

                            }
                            else if (RETORNO.DATASET(dsPesquisa, 0, "sLiberaSupervisor") == "S" && sGestor == "S")
                            {
                                pnlAcoes.Visible = true;
                                Div_AprovacaoSupervisor.Visible = true;
                                Div_AprovacaoTerceiros.Visible = false;
                                Div_AprovacaoRH.Visible = false;
                                Div_AprovacaoDiretor.Visible = false;
                            }
                            else if (RETORNO.DATASET(dsPesquisa, 0, "bGerenciar") != "0")
                            {
                                pnlAcoes.Visible = true;
                                Div_AprovacaoSupervisor.Visible = false;
                                Div_AprovacaoRH.Visible = false;
                                Div_AprovacaoTerceiros.Visible = false;
                                Div_AprovacaoDiretor.Visible = true;
                            }
                        }


                        if (RETORNO.DATASET(dsPesquisa, 0, "dtSolicitacao").ToString() == "01/01/1900 00:00:00")
                        {
                            txtdtSolicitacao.Text = null;
                        }
                        else
                        {
                            var dt = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtSolicitacao").ToString());
                            txtdtSolicitacao.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');
                        }

                        if (RETORNO.DATASET(dsPesquisa, 0, "dtInicio").ToString() == "01/01/1900 00:00:00")
                        {
                            txtsdtInicio.Text = null;
                        }
                        else
                        {
                            var dt = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtInicio").ToString());
                            txtsdtInicio.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');
                            div_dtFinal.Visible = false;

                            if (ddlidTipo.SelectedValue != "2")
                            {
                                txtsdtInicio.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');
                            }
                            else
                            {
                                lblDtInicio.InnerText = "Data Agendada";
                                txtsdtInicio.Attributes["type"] = "datetime-local";
                                txtsdtInicio.Text = dt.ToString("yyyy-MM-dd HH:mm:ss").Replace('/', '-');
                            }
                        }

                        if (RETORNO.DATASET(dsPesquisa, 0, "dtFinal").ToString() == "01/01/1900 00:00:00")
                        {
                            div_dtFinal.Visible = false;


                            div_espacoBranco.Attributes["class"] = "col-lg-6";

                            txtsdtFinal.Text = "";
                        }
                        else
                        {
                            div_dtFinal.Visible = true;
                            div_espacoBranco.Attributes["class"] = "col-lg-4";
                            var dt = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtFinal").ToString());
                            txtsdtFinal.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');
                        }

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));

                        if (ddlidTipo.SelectedValue == "1")
                        {
                            div_DataAlterar.Visible = true;
                            div_flagAlterar.Visible = true;
                            div_Datas.Visible = true;
                            SwitchAtivo.Definir("N", "Alterar Data Desejada?", "");

                            if (RETORNO.DATASET(dsPesquisa, 0, "dtInicio").ToString() == "01/01/1900 00:00:00")
                            {
                                txtdtInicialSugerida.Text = null;
                            }
                            else
                            {
                                var dt = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtInicio").ToString());
                                txtdtInicialSugerida.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');
                            }

                            if (RETORNO.DATASET(dsPesquisa, 0, "dtFinal").ToString() == "01/01/1900 00:00:00")
                            {
                                txtdtFinalSugerida.Text = null;
                            }
                            else
                            {
                                var dt = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtFinal").ToString());
                                txtdtFinalSugerida.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');
                            }
                        }

                        if (ddlidTipo.SelectedValue == "2")
                        {
                            div_infoExtra.Visible = true;
                            txtSolicitante.Text = RETORNO.DATASET(dsPesquisa, "sSolicitante");
                            txtDeptoSolicitante.Text = RETORNO.DATASET(dsPesquisa, "sDepartamento");
                            txtEmailSolicitante.Text = RETORNO.DATASET(dsPesquisa, "sEmailSolicitante");
                            lblDtInicio.InnerText = "Data - Hora Solicitada";
                            div_dtFinal.Visible = false;
                            txtsdtInicio.Text = DateTime.Parse(RETORNO.DATASET(dsPesquisa, 0, "dtInicio")).ToString("yyyy-MM-dd HH:mm");
                        }
                        else
                        {
                            txtSolicitante.Text = RETORNO.DATASET(dsPesquisa, "sSolicitante");
                            txtDeptoSolicitante.Text = RETORNO.DATASET(dsPesquisa, "sDepartamento");
                            txtEmailSolicitante.Text = RETORNO.DATASET(dsPesquisa, "sEmailSolicitante");
                            div_infoExtra.Visible = true;
                        }
                    }
                    else
                    {
                        MensagemPagina.MostraMensagem_Erro("Erro: " + sErro);
                    }
                }
                RegistraScript("");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        private string VerificaGestor(string idDepartamento)
        {
            string sErro = "";
            DataSet dsPesquisa;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR-GESTOR-DEPARTAMENTO");
            vParametros.Add("@idUsuarioLogado", IDENTITY.Variaveis.idUsuario());
            vParametros.Add("@idDepartamento", idDepartamento);
            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                return RETORNO.DATASET(dsPesquisa, 0, "sGestorDepartamento");
            }
            else
            {
                return "N";
            }
        }

        protected void popularHistorico(string idSolicitacao)
        {
            string sFuncao = "CONSULTAR_HISTORICO";
            DataTable tb;
            DataSet ds;
            string sSql = "sp_Manipula_tbl_Flow_Solicitacoes";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@idSolicitacao", idSolicitacao);

            tb = BD.ExecutarDataTable(sSql, vParametros, false);
            ds = BD.ExecutarDataSet(sSql, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                var nLinhas = tb.Rows.Count - 1;
                var usuarioRegistrado = RETORNO.DATASET(ds, nLinhas, "idUsuarioAprovacao");
                if (IDENTITY.Variaveis.idUsuario() == usuarioRegistrado)
                {
                    pnlAcoes.Visible = false;
                }

                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, tb, 1, "desc", "false", "''"), true);
                pnResultado.Visible = true;
            }
            else
            {
                MensagemPagina1.MostraMensagem_Erro("Nenhuma Solicitação para Aprovação");
            }
        }

        void Salvar()
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {
                    string[] vidSolicitacao = hddidSolicitacao.Value.Split(',');
                    string idSolicitacao = vidSolicitacao[0].ToString();

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idSolicitacao", idSolicitacao);
                    vParametros.Add("@sDscSolicitacao", txtsDscSolicitacao.Text);
                    vParametros.Add("@idTipo", ddlidTipo.SelectedValue);
                    vParametros.Add("@idStatus", ddlidStatus.SelectedValue);
                    vParametros.Add("@idUsuarioSolicitacao", IDENTITY.Variaveis.idUsuario());

                    //vParametros.Add("@sObservacaoSolicitacao",txtsObservacaoSolicitacao.Content);

                    if (txtsdtInicio.Text == "")
                    {
                        vParametros.Add("@dtInicio", txtsdtInicio.Text);
                    }
                    else
                    {
                        DateTime dt = DateTime.Parse(txtsdtInicio.Text.ToString(), CultureInfo.InvariantCulture);
                        vParametros.Add("@dtInicio", dt.ToString());
                    }

                    if (txtsdtFinal.Text == "")
                    {
                        vParametros.Add("@dtFinal", txtsdtFinal.Text);
                    }
                    else
                    {
                        DateTime dt = DateTime.Parse(txtsdtFinal.Text.ToString(), CultureInfo.InvariantCulture);
                        vParametros.Add("@dtFinal", dt.ToString());
                    }

                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idSolicitacao = RETORNO.DATASET(dsSalvar, "idSolicitacao");
                        Pesquisar(idSolicitacao, false);
                        MensagemPagina.MostraMensagem_Sucesso("Gravado com Sucesso!");
                    }
                    else
                    {
                        throw new Exception("BD: " + sErro.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }
            }
        }
        #endregion

        #region | Salvar Fluxo

        protected void cmdConfirmarAcao_Click(object sender, EventArgs e)
        {
            // Verifica o valor do HiddenField que o JavaScript atualizou
            if (hddAlterarData.Value == "S")
            {
                // Se o switch estava ligado, chama a nova função que altera status E datas
                AlterarStatusComDatas();
            }
            else
            {
                // Se estava desligado, segue o fluxo normal de apenas alterar o status
                string action = hddSelectedAction.Value;
                switch (action)
                {
                    case "AprovarSupervisor": gerenciar_status(2); break;
                    case "RejeitarSupervisor": gerenciar_status(4); break;
                    case "AprovarRH": gerenciar_status(6); break;
                    case "RejeitarRH": gerenciar_status(7); break;
                    case "Cancelado": gerenciar_status(5); break;
                    case "Finalizado": gerenciar_status(3); break;
                    case "PendenteDoc": gerenciar_status(8); break;

                    case "AprovarDiretor": gerenciar_status(10); break;
                    case "RejeitarDiretor": gerenciar_status(11); break;

                    case "AprovarTerceiros": gerenciar_status(12); break;
                    case "RejeitarTerceiros": gerenciar_status(13); break;

                }
            }
        }

        /// <summary>
        /// MÉTODO ATUALIZADO: Chama a procedure para alterar STATUS e DATAS ao mesmo tempo.
        /// </summary>
        void AlterarStatusComDatas()
        {
            // Validação 1: Garante que as novas datas foram preenchidas
            if (string.IsNullOrEmpty(txtdtInicialSugerida.Text) || string.IsNullOrEmpty(txtdtFinalSugerida.Text))
            {
                MensagemModal.MostraMensagem_Erro("Ao alterar as datas, os campos 'Nova Data de Início' e 'Nova Data Final' são obrigatórios.");
                AbrirModal();
                return;
            }

            // Validação 2: Garante que as novas datas são diferentes das originais
            if (txtsdtInicio.Text == txtdtInicialSugerida.Text && txtsdtFinal.Text == txtdtFinalSugerida.Text)
            {
                MensagemModal.MostraMensagem_Erro("Para alterar o período, as novas datas devem ser diferentes das originais.");
                AbrirModal();
                return;
            }

            string sErro = "";
            try
            {
                // Define o novo status com base na ação
                string action = hddSelectedAction.Value;
                int novoStatus = 0;

                if (action == "AprovarSupervisor") novoStatus = 2;
                if (action == "RejeitarSupervisor") novoStatus = 4;
                if (action == "AprovarRH") novoStatus = 6;
                if (action == "RejeitarRH") novoStatus = 7;
                if (action == "AprovarDiretor") novoStatus = (int)Status.AprovadoDiretor;
                if (action == "RejeitarDiretor") novoStatus = (int)Status.RejeitadoDiretor;

                DataSet ds;
                Dictionary<string, string> vParametros = new Dictionary<string, string>();

                vParametros.Add("@sFuncao", "ALTERAR_STATUS_E_DATAS");
                vParametros.Add("@idSolicitacao", txtidSolicitacao.Text);
                vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());
                vParametros.Add("@idStatus", novoStatus.ToString());

                // --- datas---
                vParametros.Add("@dtInicioNova",
     string.IsNullOrEmpty(txtdtInicialSugerida.Text)
     ? null
     : DateTime.Parse(txtdtInicialSugerida.Text, System.Globalization.CultureInfo.InvariantCulture).ToString()
 );
                vParametros.Add("@dtFinalNova",
                    string.IsNullOrEmpty(txtdtFinalSugerida.Text)
                    ? null
                    : DateTime.Parse(txtdtFinalSugerida.Text, System.Globalization.CultureInfo.InvariantCulture).ToString()
                );

                ds = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(ds, out sErro))
                {
                    FUNCOES.DirecionaPagina(
                        string.Format("App/Paginas/RRHH/Solicitacao_Detalhe.aspx?id={0}&msg=1", txtidSolicitacao.Text)
                    );
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro("BD: " + sErro);
                    FecharModal();
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
                FecharModal();
            }
        }

        /// <summary>
        /// MÉTODO EXISTENTE: Chama a procedure para alterar APENAS o STATUS.
        /// </summary>
        void gerenciar_status(int status)
        {
            // A validação de 'Motivo' só é necessária neste fluxo
            if (!ValidarAprovacao())
            {
                AbrirModal();
                return;
            }

            if (status == 6) { div_PDF.Visible = true; }
            else { div_PDF.Visible = false; }

            if (status == (int)Status.AprovadoSupervisor)
            {
                if (hddsFluxoTerceiro.Value == "S")
                {
                    status = (int)Status.AguardandoNfe;
                }
                else
                {
                    status = (int)Status.AprovadoSupervisor;
                }
            }

            string sErro = "";
            try
            {
                DataSet ds;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();

                vParametros.Add("@sFuncao", "ALTERAR_STATUS");
                vParametros.Add("@idSolicitacao", txtidSolicitacao.Text);
                vParametros.Add("@sObservacao", txtsObservacao.Text);
                vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());
                vParametros.Add("@idStatus", status.ToString());

                ds = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(ds, out sErro))
                {
                    FUNCOES.DirecionaPagina(string.Format("App/Paginas/RRHH/Solicitacao_Detalhe.aspx?id={0}&msg=1", txtidSolicitacao.Text));
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro("BD: " + sErro);
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }

        #endregion

        #region | Limpar Campos
        void LimpaCampos()
        {
            hddidSolicitacao.Value = "0";
            txtidSolicitacao.Text = "Novo";
            txtsDscSolicitacao.Text = "";
            PainelAtualizacao.Visible = false;

            txtsObservacaoSolicitacao.Content = "";
        }
        #endregion

        #region | Validação 
        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (txtsDscSolicitacao.Text.Length < 5)
            {
                sMensagemErro = "Descrição inválida!";
                bRetorno = false;
                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        private bool ValidarAprovacao()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (txtsObservacao.Text.Length < 5)
            {
                sMensagemErro = "O Motivo deve possuir ao menos 5 caracteres!";
                bRetorno = false;
                MensagemModal.MostraMensagem_Erro(sMensagemErro);
            }
            return bRetorno;
        }
        #endregion

        #region | Combos/DDL
        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlidTipo, "sp_Select 'Flow_Solicitacao_Tipo'", "idTipo", "sDscTipo", false, "Selecione um Tipo", "0");
            FUNCOES.Popula_Combo(ddlidStatus, "sp_Select 'Flow_Solicitacao_Status'", "idStatus", "sDscStatus", false, "Selecione o Status", "0");
        }

        void Popular_Combo_id(string idUsuario)
        {
            // Lógica de popular combo se necessário
        }
        #endregion

        #region | Arquivos
        bool Popular_aba_Arquivo(string idSolicitacao, bool exibicao)
        {
            DataSet ds;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "VERIFICAR-ARQUIVO");
            vParametros.Add("@idSolicitacao", txtidSolicitacao.Text);
            string srcCaminho = $"~/App/Paginas/Arquivos.aspx?idObjeto={idSolicitacao}&sTipoObjeto=Solicitação&sExibicao={exibicao}";

            ds = BD.ExecutarDataSet(sProcedure, vParametros);
            if (!string.IsNullOrEmpty(RETORNO.DATASET(ds, "idArquivo")))
            {
                frmArquivos.Attributes.Add("src", srcCaminho);
                aba_Arquivos.Visible = true;
                return true;
            }
            else
            {
                aba_Arquivos.Visible = false;
                return false;
            }
        }
        #endregion

        #region | Script 
        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("$(function() {"); // Usando $ em vez de $v192 por padrão

            sb.Append("$(\"#dialog-Salvar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_SALVAR\", \"\");");
            sb.Append("$(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$('[id*=cmdSalvar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$('#dialog-Salvar').dialog('open');");
            sb.Append("});");

            sb.Append("});");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);
        }

        void AbrirModal()
        {
            string script = @"
            <script type='text/javascript'>
                $(document).ready(function(){
                    $('#myModal').modal('show');
                    $('#" + txtsObservacao.ClientID + @"').focus();
                });
            </script>";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "AbrirModal", script, false);
            txtsObservacao.Focus();
        }

        void FecharModal()
        {
            string script = @"
    <script type='text/javascript'>
        $(document).ready(function(){
            $('#myModal').modal('hide');
            $('.modal-backdrop').remove(); // Remove o backdrop
            $('body').removeClass('modal-open'); // Libera o scroll do fundo
        });
    </script>";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "FecharModal", script, false);
        }
        #endregion

        #region | dtgvConsulta
        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.Cells[0].Controls[0] as HyperLink).Text == "Sem Aprovação")
                    (e.Row.Cells[0].Controls[0] as HyperLink).NavigateUrl = "#";

                if ((e.Row.Cells[1].Controls[0] as HyperLink).Text == "Nenhum Departamento")
                    (e.Row.Cells[1].Controls[0] as HyperLink).NavigateUrl = "#";

                if ((e.Row.Cells[3].Controls[0] as HyperLink).Text == "Nenhuma Observação")
                    (e.Row.Cells[3].Controls[0] as HyperLink).NavigateUrl = "#";
            }
        }
        #endregion

        #region | PDF
        protected void cmdGerarPDF_Click(object sender, EventArgs e)
        {
            PDF_SOLICITACAO();
        }

        void PDF_SOLICITACAO()
        {
            try
            {
                Microsoft.Reporting.WebForms.ReportViewer rv4 = new Microsoft.Reporting.WebForms.ReportViewer();

                rv4.ProcessingMode = ProcessingMode.Local;
                rv4.LocalReport.EnableExternalImages = true;

                rv4.LocalReport.ReportPath = Server.MapPath("~/App/Reports/Ferias.rdlc");

                string sFuncao = "CONSULTAR-SOLICITACAO-PDF";

                DataSet dsSolicitacao;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", sFuncao);
                vParametros.Add("@idSolicitacao", hddidSolicitacao.Value);


                //if(selectedEnvioIds != null)
                //{
                //    EXC_ENVIO(selectedEnvioIds);
                //}                      

                dsSolicitacao = BD.ExecutarDataSet(sProcedure, vParametros);

                if (dsSolicitacao.Tables.Count == 0 || dsSolicitacao.Tables[0].Rows.Count == 0)
                {
                    MensagemPagina.MostraMensagem_Erro("Erro ao Gerar Excel: Nenhum dado encontrado.");
                    return;
                }

                rv4.LocalReport.DataSources.Clear();
                rv4.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dsSolicitacao.Tables[0]));
                DataRow row = dsSolicitacao.Tables[0].AsEnumerable().FirstOrDefault();

                if (row != null)
                {
                    ReportParameter[] rp = new ReportParameter[7];

                    // Substituindo com os dados das colunas especificadas
                    rp[0] = new ReportParameter("Empresa_RazaoSocial", row["Empresa_RazaoSocial"].ToString());
                    rp[1] = new ReportParameter("Colaborador_Nome", row["Colaborador_Nome"].ToString());
                    rp[2] = new ReportParameter("Colaborador_CPF", row["Colaborador_CPF"].ToString());
                    rp[3] = new ReportParameter("Colaborador_Funcao", row["Colaborador_Funcao"].ToString());
                    rp[4] = new ReportParameter("Empresa_CNPJ", row["Empresa_CNPJ"].ToString());
                    rp[5] = new ReportParameter("dtInicial", row["dtInicial"].ToString());
                    rp[6] = new ReportParameter("dtFinal", row["dtFinal"].ToString());

                    rv4.LocalReport.SetParameters(rp);
                    rv4.LocalReport.Refresh();

                    byte[] bytes = rv4.LocalReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings);
                    string sNomeArquivo = "Solicitacao" + dsSolicitacao.Tables[0].Rows[0]["Colaborador_Nome"].ToString() + "_" + FUNCOES.CarimboDataHora() + ".pdf";

                    File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivo, bytes);
                    Pesquisar(hddidSolicitacao.Value, false);
                    FUNCOES.DownloadArquivo(Page, sNomeArquivo);
                    // Mostrar mensagem de sucesso
                    MensagemPagina.MostraMensagem_Sucesso("PDF da Solicitação Gerado Com Sucesso!");
                }
                else
                {
                    // Lidar com o caso onde não há dados
                    MensagemPagina.MostraMensagem_Erro("Erro ao Gerar PDF");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao gerar o PDF! </br>" + ex.Message);
            }
        }
        #endregion

        #region | htmlEditor
        [Obsolete]
        public void HTMLEditor_RemoveBottomToolbar_Button(Editor editor, string btnNome, bool bEdicao)
        {
            var toolbar = editor.FindControl("ctl03") as WebControl;

            if (toolbar != null)
            {
                var buttonToRemove = toolbar.FindControl(btnNome);

                if (buttonToRemove != null)
                    toolbar.Controls.Remove(buttonToRemove);
            }

            if (!bEdicao)
                editor.ActiveMode = ActiveModeType.Preview;
            else
                editor.ActiveMode = ActiveModeType.Design;
        }
        #endregion

        #region | Opções Datas

        void ConsultarOpcoesDatas(string idSolicitacao)
        {
            if (idSolicitacao == "0") return;

            try
            {
                // Reutiliza a PROC existente que criamos na etapa anterior
                Dictionary<string, string> vParamsDatas = new Dictionary<string, string>
        {
            { "@sFuncao", "CONSULTAR-OPCOES-DATAS" },
            { "@idSolicitacao", idSolicitacao }
        };

                DataSet dsDatas = BD.ExecutarDataSet(sProcedure, vParamsDatas);
                var listaBanco = new List<OpcaoDataItem>();

                if (BD.ValidarDataSet(dsDatas, out _))
                {
                    foreach (DataRow row in dsDatas.Tables[0].Rows)
                    {
                        listaBanco.Add(new OpcaoDataItem
                        {
                            DtInicio = Convert.ToDateTime(row["dtInicio"]),
                            DtFinal = Convert.ToDateTime(row["dtFinal"])
                        });
                    }
                }

                if (listaBanco.Count > 0)
                {
                    rptOpcoesDatas.DataSource = listaBanco;
                    rptOpcoesDatas.DataBind();
                    msgSemOpcoes.Visible = false;
                    // Se tem dados, habilita o switch (opcional, ou deixa sempre visivel)
                    SwitchOpcoesDatas.Visible = true;
                    divOpc.Visible = true;
                }
                else
                {
                    rptOpcoesDatas.DataSource = null;
                    rptOpcoesDatas.DataBind();
                    msgSemOpcoes.Visible = true;
                    // Se não tem dados, pode esconder o switch para não poluir
                    SwitchOpcoesDatas.Visible = false;
                    divOpc.Visible = false;
                }
            }
            catch
            {
                // Tratar erro silenciosamente ou logar
            }
        }

        #endregion

        #region | CHAT INTERNO

        void CarregarChatInterno(string idSolicitacao)
        {
            if (idSolicitacao == "0") return;

            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
        {
            { "@sFuncao", "CONSULTAR_HISTORICO_INTERNO" },
            { "@idSolicitacao", idSolicitacao },
            { "@sInterno", "S" } // Pede só as mensagens internas
        };

                DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);

                // Lista para o Repeater
                var listaMsgs = new List<object>();

                // --- NOVO: Garante que começa como "N" ---
                hddTemNotificacao.Value = "N";

                if (BD.ValidarDataSet(ds, out _))
                {
                    // --- NOVO: Verifica se a ÚLTIMA mensagem é de outra pessoa ---
                    // Pegamos a última linha da tabela (Count - 1)
                    int totalLinhas = ds.Tables[0].Rows.Count;
                    if (totalLinhas > 0)
                    {
                        DataRow ultimaLinha = ds.Tables[0].Rows[totalLinhas - 1];

                        // Se o ID de quem mandou a última msg for DIFERENTE do meu ID...
                        if (ultimaLinha["idUsuario"].ToString() != IDENTITY.Variaveis.idUsuario())
                        {
                            hddTemNotificacao.Value = "S"; // ...ativa a notificação
                        }
                    }
                    // -------------------------------------------------------------

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        // Lógica visual: Se fui eu que escrevi, classe 'msg-me', senão 'msg-other'
                        string usuarioMsg = row["sUsuario"].ToString();
                        string classeCss = "msg-other";

                        if (row["idUsuario"].ToString() == IDENTITY.Variaveis.idUsuario())
                        {
                            classeCss = "msg-me";
                        }

                        listaMsgs.Add(new
                        {
                            Motivo = row["sMotivo"],
                            Usuario = usuarioMsg,
                            DataAtualizacao = Convert.ToDateTime(row["dtAtualizacao"]).ToString("dd/MM HH:mm"),
                            ClasseCss = classeCss
                        });
                    }
                }

                if (listaMsgs.Count > 0)
                {
                    rptChatInterno.DataSource = listaMsgs;
                    rptChatInterno.DataBind();
                    divSemMsg.Visible = false;
                }
                else
                {
                    rptChatInterno.DataSource = null;
                    rptChatInterno.DataBind();
                    divSemMsg.Visible = true;
                }
            }
            catch { }
        }

        protected void btnEnviarMsgInterna_Click(object sender, EventArgs e)
        {
            string msg = txtMsgInterna.Text.Trim();
            if (string.IsNullOrEmpty(msg)) return;
            if (hddidSolicitacao.Value == "0" || string.IsNullOrEmpty(hddidSolicitacao.Value)) return;

            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
        {
            { "@sFuncao", "INSERE-MSG-INTERNA" },
            { "@idSolicitacao", hddidSolicitacao.Value },
            { "@idUsuario", IDENTITY.Variaveis.idUsuario() },
            { "@sObservacao", msg } // Reutilizando o nome do parametro da proc
        };

                BD.ExecutarDataSet(sProcedure, vParametros);

                // Limpa e recarrega
                txtMsgInterna.Text = "";
                CarregarChatInterno(hddidSolicitacao.Value);
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao enviar mensagem: " + ex.Message);
            }
        }

        [WebMethod]
        public static object EnviarMensagemChat_WebMethod(string idSolicitacao, string mensagem)
        {
            try
            {
                // 1. Validar Sessão (Segurança básica pois é estático)
                if (System.Web.HttpContext.Current.Session["idUsuario"] == null)
                {
                    return new { sucesso = false, erro = "Sessão expirada." };
                }

                string idUsuario = System.Web.HttpContext.Current.Session["idUsuario"].ToString();
                // OU se o seu framework TT.FrameWork.Identity funcionar estático:
                // string idUsuario = TT.FrameWork.Identity.Variaveis.idUsuario(); 

                // 2. Executar a Procedure (Igual você fazia, mas adaptado para static)
                Dictionary<string, string> vParametros = new Dictionary<string, string>
        {
            { "@sFuncao", "INSERE-MSG-INTERNA" },
            { "@idSolicitacao", idSolicitacao },
            { "@idUsuario", idUsuario },
            { "@sObservacao", mensagem }
        };

                // Assumindo que BD.ExecutarDataSet é estático (geralmente é em frameworks assim)
                TT.FrameWork.BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Solicitacoes", vParametros);

                // 3. Retornar os dados para montar o balãozinho na hora (sem ir no banco de novo)
                return new
                {
                    sucesso = true,
                    dataHora = DateTime.Now.ToString("dd/MM HH:mm"),
                    usuario = "Eu", // Ou pegar o nome da sessão
                    mensagem = mensagem
                };
            }
            catch (Exception ex)
            {
                return new { sucesso = false, erro = ex.Message };
            }
        }
        #endregion
    }

    [Serializable]
    public class OpcaoDataItem
    {
        public DateTime DtInicio { get; set; }
        public DateTime DtFinal { get; set; }
    }
}