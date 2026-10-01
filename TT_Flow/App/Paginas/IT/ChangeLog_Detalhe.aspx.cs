using System;
using System.IO;
using System.Text;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using TT.FrameWork;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using AjaxControlToolkit.HtmlEditor;
using AjaxControlToolkit.HtmlEditor.Sanitizer;

namespace TT_Flow.App.Paginas.IT
{
    public partial class ChangeLog_Detalhe : Page
    {
        string sTituloPagina = "Novo Change log";
        string sProcedure = "sp_Manipula_tbl_Flow_ChangeLog";
        string sAprovacao = "N";
        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtStatus.ReadOnly = true;
                div_Acao.Visible = false;

                cmdAprovar.Visible = FUNCOES.ValidaPermissao(Permissao.TI.Change_Log.Aprovar);
                cmdRejeitar.Visible = FUNCOES.ValidaPermissao(Permissao.TI.Change_Log.Aprovar);

                if (Request["id"] != null)
                {
                    Pesquisar(Request["id"].ToString());
                    if (Request["id"] == "0")
                    {
                        FUNCOES.ValidaPermissao(Permissao.TI.Change_Log.Incluir, true);
                        LimpaCampos();

                        sAprovacao = "N";

                        divStatus.Visible = false;
                        divVersao.Visible = false;

                        aba_Arquivos.Visible = false;
                        aba_Testes.Visible = false;
                        aba_Historico.Visible = false;

                        cmdListaAprovar.Visible = false;
                        cmdAprovar.Visible = false;
                        cmdAprovarTeste.Visible = false;
                        cmdRejeitar.Visible = false;
                        cmdRejeitarTeste.Visible = false;
                    }
                    else
                    {
                        if (Request["msg"] == "1")
                            MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso!");
                        else if (Request["msg"] == "2")
                            MensagemPagina.MostraMensagem_Erro("Não é possível salvar sem um Usuário! <br />Por favor efetue o Login!");
                    }
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.TI.Change_Log.Incluir, true);
                    Pesquisar("0");
                }
            }

            var requestTarget = Request["__EVENTTARGET"];
            if (requestTarget == "ExcluirArquivo")
            {
                ExcluirArquivo(hddExcluirArquivo.Value);

                UpdPrincipal.Update();
                UpdLocalArquivo.Update();
                UpdAbas.Update();
                UpdAcao.Update();
                UpdBotoes.Update();
            }
            else if (requestTarget == "DownloadArquivo")
                DownloadArquivo(hddDownloadArquivo.Value);

            RegistraScript();
        }

        protected void Pesquisar(string idPesquisa)
        {
            try
            {
                LimpaCampos();
                FUNCOES.Popula_Combo(ddlidUsuarioTeste, "sp_Select 'Usuarios_x_Departamentos', '43'", "idUsuario", "sDscUsuario", true, "Selecione um Usuário de Teste", "0");

                cmdEmTeste.Visible = false;
                cmdPausarTeste.Visible = false;
                cmdProducao.Visible = false;
                cmdAprovarTreinamento.Visible = false;
                div_gvVideos.Visible = false;
                aba_videos.Visible = false;
                aba_Treinamento.Visible = false;

                cmdAlterna_Fix_x_SmallChange.Visible = false;

                if (idPesquisa != "0")
                {
                    aba_videos.Visible = true;

                    ddlTipoAlteracao.Attributes.Add("disabled", "disabled");
                    cmdSalvar.Visible = false;

                    Dictionary<string, string> vParametrosHistorico = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_HISTORICO" },
                        { "@idItem", idPesquisa }
                    };
                    DataTable tbHistorico = BD.ExecutarDataTable(sProcedure, vParametrosHistorico);

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idItem", idPesquisa }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        hddidItem.Value = RETORNO.DATASET(dsPesquisa, "idItem");
                        txtidItem.Text = RETORNO.DATASET(dsPesquisa, "idItem");
                        txtsDscTituloAlteracao.Text = RETORNO.DATASET(dsPesquisa, "sDscTituloAlteracao");
                        txtsCodigoVersao.Text = RETORNO.DATASET(dsPesquisa, "sVersao");
                        ddlTipoAlteracao.SelectedValue = RETORNO.DATASET(dsPesquisa, "idTipoAlteracao");
                        txtStatus.Text = RETORNO.DATASET(dsPesquisa, "sDscStatus");
                        hddidStatus.Value = RETORNO.DATASET(dsPesquisa, "idStatus");
                        hddAlteracao.Value = RETORNO.DATASET(dsPesquisa, "sDscAlteracao");
                        txtsDscAlteracao.Value = RETORNO.DATASET(dsPesquisa, "sDscAlteracao");
                        hddTeste.Value = RETORNO.DATASET(dsPesquisa, "sDscEvidenciaTeste");
                        txtsDscTeste.Value = RETORNO.DATASET(dsPesquisa, "sDscEvidenciaTeste");
                        hddTreinamento.Value = RETORNO.DATASET(dsPesquisa, "sDscEvidenciaTreinamento");
                        txtsDscTreinamento.Value = RETORNO.DATASET(dsPesquisa, "sDscEvidenciaTreinamento");

                        int.TryParse(hddidStatus.Value, out int idStatus);

                        if (idStatus != 4 && idStatus != 5 && idStatus != 7 && idStatus != 8 && (ddlTipoAlteracao.SelectedValue == "4" || ddlTipoAlteracao.SelectedValue == "1"))
                            cmdAlterna_Fix_x_SmallChange.Visible = true;

                        if (tbHistorico.Rows.Count > 0)
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", Grid.DataBindComScript(dtgvHistorico, tbHistorico, 0, "desc"), true);
                        else
                            pnResultado.Visible = false;

                        ddlidUsuarioTeste.SelectedValue = RETORNO.DATASET(dsPesquisa, "idUsuarioTeste");
                        hddUsuario.Value = RETORNO.DATASET(dsPesquisa, "idUsuarioTeste");

                        if (!FUNCOES.ValidaPermissao(Permissao.TI.Change_Log.Aprovar))
                        {
                            cmdAprovar.Visible = false;
                            cmdRejeitar.Visible = false;
                            btnDivisor1.Visible = false;
                            btnDivisor2.Visible = false;

                            if (ddlidUsuarioTeste.SelectedValue != IDENTITY.Variaveis.idUsuario())
                                cmdListaAprovar.Visible = false;
                        }

                        if (idStatus == 8 && FUNCOES.ValidaPermissao(Permissao.TI.Change_Log.Aprovar))
                        {
                            cmdAprovar.Visible = true;
                            cmdRejeitar.Visible = true;
                            btnDivisor2.Visible = false;
                            btnDivisor3.Visible = false;

                            ddlidUsuarioTeste.Attributes.Add("disabled", "disabled");
                            ddlDocumentacao.Attributes.Add("disabled", "disabled");

                            txtsDscTituloAlteracao.ReadOnly = true;
                            txtsLocalArquivo.ReadOnly = true;
                            aba_Treinamento.Visible = true;

                            txtsDscAlteracao.Attributes["class"] += " naoEdita";
                            txtsDscTeste.Attributes["class"] += " naoEdita";
                            txtsDscDocumentacao.Attributes["class"] += " naoEdita";
                            txtsDscDocumentacao.Attributes["class"] += " naoEdita";
                            //HTMLEditor_RemoveBottomToolbar_Button(txtsDscTeste, "ctl00", false);
                            //HTMLEditor_RemoveBottomToolbar_Button(txtsDscTeste, "ctl01", false);
                            ////HTMLEditor_RemoveBottomToolbar_Button(txtsDscAlteracao, "ctl00", false);
                            ////HTMLEditor_RemoveBottomToolbar_Button(txtsDscAlteracao, "ctl01", false);
                            //HTMLEditor_RemoveBottomToolbar_Button(txtsDscDocumentacao, "ctl00", false);
                            //HTMLEditor_RemoveBottomToolbar_Button(txtsDscDocumentacao, "ctl01", false);
                            //HTMLEditor_RemoveBottomToolbar_Button(txtsDscTreinamento, "ctl00", false);
                            //HTMLEditor_RemoveBottomToolbar_Button(txtsDscTreinamento, "ctl01", false);
                        }
                        else
                            cmdListaAprovar.Visible = false;

                        if (idStatus == 4)
                        {
                            txtsDscTituloAlteracao.ReadOnly = true;
                            txtsLocalArquivo.ReadOnly = true;
                            ddlidUsuarioTeste.Attributes.Add("disabled", "disabled");
                            ddlDocumentacao.Attributes.Add("disabled", "disabled");
                            field_cancel.Value = "Voltar";

                            cmdListaAprovar.Visible = false;
                            cmdAprovar.Visible = false;
                            cmdAprovarTeste.Visible = false;
                            cmdRejeitar.Visible = false;
                            cmdRejeitarTeste.Visible = false;
                            cmdSalvar.Visible = false;
                            aba_Treinamento.Visible = true;

                            txtsDscAlteracao.Attributes["class"] += " naoEdita";
                            txtsDscTeste.Attributes["class"] += " naoEdita";
                            txtsDscDocumentacao.Attributes["class"] += " naoEdita";
                            txtsDscDocumentacao.Attributes["class"] += " naoEdita";
                            //HTMLEditor_RemoveBottomToolbar_Button(txtsDscTeste, "ctl00", false);
                            //HTMLEditor_RemoveBottomToolbar_Button(txtsDscTeste, "ctl01", false);
                            ////HTMLEditor_RemoveBottomToolbar_Button(txtsDscAlteracao, "ctl00", false);
                            ////HTMLEditor_RemoveBottomToolbar_Button(txtsDscAlteracao, "ctl01", false);
                            //HTMLEditor_RemoveBottomToolbar_Button(txtsDscDocumentacao, "ctl00", false);
                            //HTMLEditor_RemoveBottomToolbar_Button(txtsDscDocumentacao, "ctl01", false);
                            //HTMLEditor_RemoveBottomToolbar_Button(txtsDscTreinamento, "ctl00", false);
                            //HTMLEditor_RemoveBottomToolbar_Button(txtsDscTreinamento, "ctl01", false);
                        }
                        else if (idStatus == 2)
                        {
                            cmdListaAprovar.Visible = true;
                            cmdAprovar.Visible = false;
                            cmdRejeitar.Visible = false;
                            cmdProducao.Visible = true;
                            ddlidUsuarioTeste.Attributes.Add("disabled", "disabled");
                            ddlDocumentacao.Attributes.Add("disabled", "disabled");
                            txtsDscTituloAlteracao.ReadOnly = true;
                            txtsLocalArquivo.ReadOnly = true;
                            txtsDscAlteracao.Attributes["class"] += " naoEdita";
                            //HTMLEditor_RemoveBottomToolbar_Button(txtsDscAlteracao, "ctl00", false);
                            //HTMLEditor_RemoveBottomToolbar_Button(txtsDscAlteracao, "ctl01", false);
                        }
                        else if (idStatus == 7)
                        {
                            cmdListaAprovar.Visible = true;
                            cmdAprovar.Visible = false;
                            cmdRejeitar.Visible = false;
                            aba_Treinamento.Visible = true;
                            cmdAprovarTreinamento.Visible = true;
                            ddlidUsuarioTeste.Attributes.Add("disabled", "disabled");
                            ddlDocumentacao.Attributes.Add("disabled", "disabled");
                            txtsDscTituloAlteracao.ReadOnly = true;
                            txtsLocalArquivo.ReadOnly = true;
                            txtsDscAlteracao.Attributes["class"] += " naoEdita";
                            //HTMLEditor_RemoveBottomToolbar_Button(txtsDscAlteracao, "ctl00", false);
                            //HTMLEditor_RemoveBottomToolbar_Button(txtsDscAlteracao, "ctl01", false);
                        }

                        if ((idStatus == 1 || idStatus == 3 || idStatus == 5) && ddlidUsuarioTeste.SelectedValue != IDENTITY.Variaveis.idUsuario())
                            cmdSalvar.Visible = true;

                        if (ddlidUsuarioTeste.SelectedValue == IDENTITY.Variaveis.idUsuario() && (idStatus == 1 || idStatus == 9))
                        {
                            cmdEmTeste.Visible = true;

                            cmdAprovarTeste.Visible = false;
                            cmdRejeitarTeste.Visible = false;

                            cmdAprovar.Visible = false;
                            cmdRejeitar.Visible = false;
                            btnDivisor1.Visible = false;
                            btnDivisor2.Visible = false;
                            btnDivisor3.Visible = false;

                            cmdListaAprovar.Visible = true;
                        }
                        else if (ddlidUsuarioTeste.SelectedValue == IDENTITY.Variaveis.idUsuario() && idStatus == 6)
                        {
                            cmdPausarTeste.Visible = true;
                            cmdAprovarTeste.Visible = true;
                            cmdRejeitarTeste.Visible = true;

                            cmdAprovar.Visible = false;
                            cmdRejeitar.Visible = false;
                            btnDivisor1.Visible = false;

                            cmdListaAprovar.Visible = true;
                        }
                        else
                        {
                            cmdAprovarTeste.Visible = false;
                            cmdRejeitarTeste.Visible = false;
                            if (idStatus != 2 && idStatus != 7)
                                btnDivisor1.Visible = true;
                            else
                                btnDivisor1.Visible = false;
                            btnDivisor2.Visible = false;
                            btnDivisor3.Visible = false;

                            //HTMLEditor_RemoveBottomToolbar_Button(txtsDscTeste, "ctl00", false);
                            //HTMLEditor_RemoveBottomToolbar_Button(txtsDscTeste, "ctl01", false);
                            txtsDscTeste.Attributes["class"] += " naoEdita";
                        }

                        lblTituloPagina.Text = txtsDscTituloAlteracao.Text;
                        BreadCrumb_Pagina.TitulodaPagina = txtsCodigoVersao.Text;
                        txtsLocalArquivo.Text = RETORNO.DATASET(dsPesquisa, "sLocalArquivo");
                        ddlDocumentacao.SelectedValue = RETORNO.DATASET(dsPesquisa, "sDocumento");
                        hddDocumentacao.Value = RETORNO.DATASET(dsPesquisa, "sDscDocumento");

                        if (ddlDocumentacao.SelectedValue == "N")
                        {
                            aba_documentacao.Visible = false;
                            aba_Arquivos.Visible = false;
                        }
                        else
                        {
                            aba_documentacao.Visible = true;
                            aba_Arquivos.Visible = true;
                        }
                        //Retirei por conta de lentidão para carregar
                       // gvVideos_ConsultaArquivos();

                        eArquivos.Attributes.Add("src", string.Format("../Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}", idPesquisa, "ChangeLog"));
                    }
                }
                else
                {
                    cmdRetornar.Visible = false;
                    cmdAvancar.Visible = false;
                }

                ddlTipoAlteracao.Focus();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        #endregion

        #region | Utils

        void LimpaCampos()
        {
            txtidItem.Text = "Novo";
            txtsCodigoVersao.Text = "";
            txtsDscTituloAlteracao.Text = "";
            ddlTipoAlteracao.SelectedValue = "0";
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
            BreadCrumb_Pagina.TitulodaPagina = "Novo";
            hddAlteracao.Value = "";
            hddDocumentacao.Value = "";
            ddlidUsuarioTeste.SelectedValue = "0";
        }

        private bool ValidarDados(string alteracao, string documentacao, string teste)
        {
            if (alteracao.Length < 5)
            {
                MensagemPagina.MostraMensagem_Erro("Insira uma descrição com pelo menos 5 caracteres!");
                return false;
            }
            else if (txtsDscTituloAlteracao.Text.Length < 5)
            {
                MensagemPagina.MostraMensagem_Erro("Insira um título de pelo menos 5 caracteres!");
                return false;
            }
            else if (Validacoes.ValidarTexto(txtsLocalArquivo))
            {
                MensagemPagina.MostraMensagem_Erro("Insira um Local/Diretório dos arquivos!");
                return false;
            }
            else if (ddlTipoAlteracao.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione o tipo da Alteração!");
                return false;
            }
            else if (ddlidUsuarioTeste.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione um Usuário de Teste!");
                return false;
            }
            else if ((hddidStatus.Value == "2" || hddidStatus.Value == "3") && teste.Length < 5)
            {
                MensagemPagina.MostraMensagem_Erro("Para aprovar ou rejeitar um Teste é necessário colocar uma Evidência de Teste com pelo menos 5 caracteres!");
                return false;
            }
            else if (ddlDocumentacao.SelectedValue == "S" && documentacao.Length < 5)
            {
                MensagemPagina.MostraMensagem_Erro("Insira uma documentação com pelo menos 5 caracteres!");
                return false;
            }

            return true;
        }

        private string sRetornaNumeroVersao()
        {
            string sRetorno = "";
            try
            {
                DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_ChangeLog", new Dictionary<string, string> { { "@sFuncao", "NUMERO_MAIOR_TABELA" } });

                if (BD.ValidarDataSet(ds))
                {
                    string nMajor = "0";
                    string nMinor = "0";
                    string nPatch = "0";

                    nMajor = RETORNO.DATASET(ds, 0, "nMajor");
                    nMinor = RETORNO.DATASET(ds, 0, "nMinor");
                    nPatch = RETORNO.DATASET(ds, 0, "nPatch");

                    if (ddlTipoAlteracao.SelectedValue == "3")
                    {
                        nMajor = (int.Parse(nMajor) + 1).ToString();
                        sRetorno = nMajor + "." + "0" + "." + "0";
                    }
                    else if (ddlTipoAlteracao.SelectedValue == "2")
                    {
                        nMinor = (int.Parse(nMinor) + 1).ToString();
                        sRetorno = nMajor + "." + nMinor + "." + "0";
                    }
                    else if (ddlTipoAlteracao.SelectedValue == "1" || ddlTipoAlteracao.SelectedValue == "4")
                    {
                        nPatch = (int.Parse(nPatch) + 1).ToString();
                        sRetorno = nMajor + "." + nMinor + "." + nPatch;
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }

            return sRetorno;
        }

        #endregion

        #region | Salvar

        protected void SalvarDados(string sAprovacao, bool sEdicao)
        {
            if (ddlidUsuarioTeste.SelectedValue == "0") ddlidUsuarioTeste.SelectedValue = hddUsuario.Value;

            UpdPrincipal.Update();
            UpdLocalArquivo.Update();
            UpdAbas.Update();
            UpdAcao.Update();
            UpdBotoes.Update();

            string alteracao = HttpUtility.UrlDecode(hddAlteracao.Value);
            string documentacao = HttpUtility.UrlDecode(hddDocumentacao.Value);
            string teste = HttpUtility.UrlDecode(hddTeste.Value);
            string treinamento = HttpUtility.UrlDecode(hddTreinamento.Value);

            var whiteList = new Dictionary<string, string[]>
            {
                { "p", new[] { "style" } },
                { "br", new string[0] },

                { "strong", new string[0] },
                { "b", new string[0] },
                { "em", new string[0] },
                { "i", new string[0] },
                { "u", new string[0] },
                { "s", new string[0] },

                { "span", new[] { "style" } },

                { "h1", new[] { "style" } },
                { "h2", new[] { "style" } },
                { "h3", new[] { "style" } },

                { "ul", new string[0] },
                { "ol", new string[0] }, 
                { "li", new string[0] },

                { "blockquote", new[] { "style" } },

                { "a", new[] { "href", "title" } }
            };


            //Limpeza dos inputs para não ter perigo de injeção XSS
            var sanitizer = new DefaultHtmlSanitizer();
            if (string.IsNullOrEmpty(alteracao)) alteracao = sanitizer.GetSafeHtmlFragment(txtsDscAlteracao.Value, whiteList);
            if (string.IsNullOrEmpty(documentacao)) documentacao = sanitizer.GetSafeHtmlFragment(txtsDscDocumentacao.Value, whiteList);
            if (string.IsNullOrEmpty(teste)) teste = sanitizer.GetSafeHtmlFragment(txtsDscTeste.Value, whiteList);
            if (string.IsNullOrEmpty(treinamento)) treinamento = sanitizer.GetSafeHtmlFragment(txtsDscTreinamento.Value, whiteList);



            if (ValidarDados(alteracao, documentacao, teste))
            {
                try
                {
                    string idItem = Request["id"].ToString();
                    string idUsuario = IDENTITY.Variaveis.idUsuario();

                    if (!(int.Parse(idUsuario) > 0)) throw new Exception("Não é possível salvar sem um Usuário! Por favor efetue o Login!");

                    if (hddidStatus.Value == "4" || hddidStatus.Value == "5" || hddidStatus.Value == "7")
                    {
                        alteracao = "NAO_SALVA";
                        documentacao = "NAO_SALVA";
                        teste = "NAO_SALVA";
                        treinamento = "NAO_SALVA";
                    }

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR_CODIGO_COMPLETO" },
                        { "@sVersao", sAprovacao == "N" ? sRetornaNumeroVersao() : "" },
                        { "@sDscTituloAlteracao", txtsDscTituloAlteracao.Text },
                        { "@sDscAlteracao", alteracao },
                        { "@sLocalArquivo", txtsLocalArquivo.Text },
                        { "@idTipoAlteracao", ddlTipoAlteracao.SelectedValue },
                        { "@idItem", idItem },
                        { "@sDocumento", ddlDocumentacao.SelectedValue },
                        { "@sDscDocumento", documentacao },
                        { "@sAprovacao", hddidStatus.Value == "4" ? "S" : "N" },
                        { "@idUsuarioAtualizacao", idUsuario },
                        { "@idStatus", sEdicao ? "1" : hddidStatus.Value },
                        { "@sDscEvidenciaTeste", sEdicao ? "" : teste },
                        { "@idUsuarioTeste", ddlidUsuarioTeste.SelectedValue },
                        { "@sDscObservacao", sEdicao ? "" : txtsDscObservacao.Text},
                        { "@sDscEvidenciaTreinamento", sEdicao ? "" : treinamento }
                    };
                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out string sErro))
                    {
                        string msg = RETORNO.DATASET(dsSalvar, "sMsg");

                        if (string.IsNullOrEmpty(msg))
                        {
                            idItem = RETORNO.DATASET(dsSalvar, 0, "idItem");
                            FUNCOES.DirecionaPagina(string.Format("App/Paginas/IT/ChangeLog_Detalhe.aspx?id={0}&msg=1", idItem));
                        }
                        else FUNCOES.DirecionaPagina(string.Format("App/Paginas/IT/ChangeLog_Detalhe.aspx?id={0}&msg=2", "0"));
                    }
                    else throw new Exception(RETORNO.DATASET(dsSalvar, 0, "msg"));
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(string.Format("Erro: {0}", ex.Message));
                }
            }
        }

        #endregion

        #region | Ação

        protected void Acao(bool Acao)
        {
            FUNCOES.Scripts.FocusScript(Page, txtsDscObservacao.ClientID);

            div_Acao.Visible = true;

            if (hddidStatus.Value != "7")
                cmdListaAprovar.Disabled = true;

            if (Acao)
                lblObservacao.InnerText = "Motivo da Rejeição";
            else
                lblObservacao.InnerHtml = "Observação <small>(Opcional)</small>";

            UpdPrincipal.Update();
            UpdLocalArquivo.Update();
            UpdAbas.Update();
            UpdAcao.Update();
            UpdBotoes.Update();
        }

        protected void cmdAcaoOk_Click(object sender, EventArgs e)
        {
            if (hddObservacao.Value == "0")
            {
                if (txtsDscObservacao.Text.Length < 5)
                    MensagemPagina.MostraMensagem_Erro("Para rejeitar um Change Log é necessário incluir um motivo com pelo menos 5 caracteres!");
                else
                {
                    if (hddidStatus.Value != "7")
                        hddidStatus.Value = "5";

                    SalvarDados(sAprovacao, false);
                    div_Acao.Visible = false;
                }
            }
            else
            {
                if (hddidStatus.Value != "7")
                    hddidStatus.Value = "4";

                SalvarDados(sAprovacao, false);
                div_Acao.Visible = false;
            }

            UpdPrincipal.Update();
            UpdLocalArquivo.Update();
            UpdAbas.Update();
            UpdAcao.Update();
            UpdBotoes.Update();
        }

        protected void cmdAcaoCancelar_Click(object sender, EventArgs e)
        {
            div_Acao.Visible = false;
            cmdListaAprovar.Disabled = false;

            UpdPrincipal.Update();
            UpdLocalArquivo.Update();
            UpdAbas.Update();
            UpdAcao.Update();
            UpdBotoes.Update();

            if (hddidStatus.Value == "7")
                Pesquisar(hddidItem.Value);
        }

        protected void cmdSalvar_Click(object sender, EventArgs e) { hddidStatus.Value = "1"; SalvarDados(sAprovacao, true); }

        protected void cmdAprovar_Click(object sender, EventArgs e) { hddidStatus.Value = "4"; Acao(false); }

        protected void cmdRejeitar_Click(object sender, EventArgs e) { hddidStatus.Value = "5"; hddObservacao.Value = "0"; Acao(true); }

        protected void cmdAprovarTeste_Click(object sender, EventArgs e) { hddidStatus.Value = "2"; SalvarDados(sAprovacao, false); }

        protected void cmdRejeitarTeste_Click(object sender, EventArgs e) { hddidStatus.Value = "3"; SalvarDados(sAprovacao, false); }

        protected void cmdEmTeste_Click(object sender, EventArgs e) { hddidStatus.Value = "6"; SalvarDados(sAprovacao, false); }

        protected void cmdProducao_Click(object sender, EventArgs e) { hddidStatus.Value = "7"; Acao(false); }

        protected void cmdAprovarTreinamento_Click(object sender, EventArgs e)
        {
            if (txtsDscTreinamento.Value.Length < 5 && hddTreinamento.Value.Length < 5)
                MensagemPagina.MostraMensagem_Erro("Para aprovar o Treinamento é necessário colocar uma evidência do Treinamento com pelo menos 5 caracteres");
            else
            {
                hddidStatus.Value = "8";
                SalvarDados(sAprovacao, false);
            }
        }

        protected void cmdPausarTeste_Click(object sender, EventArgs e) { hddidStatus.Value = "9"; SalvarDados(sAprovacao, false); }

        #endregion

        #region | HTMLEditor

        public void HTMLEditor_AlterFont(Editor editor)
        {
            var toolbar = editor.FindControl("ctl01") as WebControl;

            if (toolbar != null)
            {
                var fontButton = toolbar.FindControl("ctl22");

                if (fontButton != null)
                {
                    var nobr = fontButton.FindControl("ctl00");

                    if (nobr != null)
                    {
                        HtmlGenericControl select = nobr.FindControl("select") as HtmlGenericControl;

                        if (select != null)
                        {
                            foreach (LiteralControl literal in select.Controls)
                            {
                                if (literal.Text == "<option value='arial, helvetica, sans-serif'>Arial</option>")
                                    literal.Text = "<option value='arial, helvetica, sans-serif' selected='selected'>Arial</option>";
                            }
                        }
                    }
                }
            }
        }

        public void HTMLEditor_RemoveTopToolbar_Button(Editor editor, string btnNome)
        {
            var toolbar = editor.FindControl("ctl01") as WebControl;

            if (toolbar != null)
            {
                var buttonToRemove = toolbar.FindControl(btnNome);

                if (buttonToRemove != null)
                    toolbar.Controls.Remove(buttonToRemove);
            }
        }

        public void HTMLEditor_RemoveBottomToolbar_Button(Editor editor, string btnNome) => HTMLEditor_RemoveBottomToolbar_Button(editor, btnNome, true);

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

        #region | Eventos

        protected void ddlTipoAlteracao_SelectedIndexChanged(object sender, EventArgs e)
        {
            divVersao.Visible = true;
            txtsCodigoVersao.Text = sRetornaNumeroVersao();

            if (ddlTipoAlteracao.SelectedValue == "4" || ddlTipoAlteracao.SelectedValue == "1")
                cmdAlterna_Fix_x_SmallChange.Visible = true;
            else
                cmdAlterna_Fix_x_SmallChange.Visible = false;

            UpdPrincipal.Update();
            UpdLocalArquivo.Update();
            UpdAbas.Update();
            UpdAcao.Update();
            UpdBotoes.Update();
        }

        protected void ddlDocumentacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlDocumentacao.SelectedValue == "N")
            {
                aba_documentacao.Visible = false;
                aba_Arquivos.Visible = false;
            }
            else
            {
                aba_documentacao.Visible = true;

                if (Request["id"] != "0")
                    aba_Arquivos.Visible = true;
            }

            UpdPrincipal.Update();
            UpdLocalArquivo.Update();
            UpdAbas.Update();
            UpdAcao.Update();
            UpdBotoes.Update();
        }

        protected void dtgvHistorico_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string descricao = string.Empty;

                e.Row.Cells[3].Text = HttpUtility.HtmlDecode(e.Row.Cells[3].Text);

                foreach (string txt in Regex.Split(e.Row.Cells[3].Text, @"<img[^>]*\/?>")) descricao += txt;

                e.Row.Cells[3].Text = descricao;
            }

            e.Row.Cells[3].Text = HttpUtility.HtmlDecode(e.Row.Cells[3].Text);
        }

        protected void cmdAvancar_click(object sender, EventArgs e)
        {
            DataSet ds = BD.ExecutarDataSet(sProcedure, new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_ID_MAXIMO" } });

            int.TryParse(RETORNO.DATASET(ds, 0, "idItem"), out int idMax);

            int id = 0;
            if (txtidItem.Text != "Novo")
                id = int.Parse(txtidItem.Text) + 1;
            else
                id = int.Parse(Request["id"]) + 1;

            Response.Redirect($"ChangeLog_Detalhe.aspx?id={(id > (idMax != 0 ? idMax : id + 1) ? 0 : id)}");
        }

        protected void cmdRetornar_click(object sender, EventArgs e)
        {
            int id = 0;

            if (txtidItem.Text != "Novo")
                id = int.Parse(txtidItem.Text) - 1;
            else
                id = int.Parse(Request["id"]) - 1;

            Response.Redirect($"ChangeLog_Detalhe.aspx?id={(id >= 0 ? id : 0)}");
        }

        protected void cmdEnviarArquivos_Click(object sender, EventArgs e)
        {
            if (fu_EnviarArquivo.HasFile)
            {
                if (Path.GetExtension(fu_EnviarArquivo.FileName).ToLower() != ".mp4") MensagemPagina_ModalVideos.MostraMensagem_Erro("Formato de arquivo inválido. Por favor, envie um arquivo .mp4!");
                else
                {
                    try
                    {
                        SalvarVideo(fu_EnviarArquivo.FileName, fu_EnviarArquivo.FileBytes);
                        MensagemPagina_ModalVideos.MostraMensagem_Sucesso("Arquivo de vídeo salvo com sucesso!");
                    }
                    catch (Exception ex)
                    {
                        MensagemPagina_ModalVideos.MostraMensagem_Erro("Houve um erro no Upload do arquivo! <br /> Erro: " + ex.Message, false);
                    }
                }
            }
            else MensagemPagina_ModalVideos.MostraMensagem_Erro("É necessário Inserir ao menos um Arquivo!");

            FUNCOES.Scripts.RemoverBackdrop_Modal(Page);
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "aba_videos-tab");
            FUNCOES.Scripts.AbrirModal(Page, "modalUploadVideo");
        }

        protected void cmdAlterna_Fix_x_SmallChange_Click(object sender, EventArgs e)
        {
            ddlTipoAlteracao.SelectedValue = ddlTipoAlteracao.SelectedValue == "4" ? "1" : "4";

            try
            {
                Dictionary<string, string> vParam = new Dictionary<string, string>
                {
                    { "@sFuncao", "SALVA_FIX_X_SMALL_CHANGE" },
                    { "@idItem", hddidItem.Value },
                    { "@idTipoAlteracao", ddlTipoAlteracao.SelectedValue },
                    { "@idUsuario", IDENTITY.Variaveis.idUsuario() },
                };
                BD.ExecutarDataSet(sProcedure, vParam);
            }
            catch { }
        }

        #endregion

        #region | Aba Vídeos

        protected void gvVideos_ConsultaArquivos()
        {
            Dictionary<string, string> vParam = new Dictionary<string, string>()
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sTipoObjeto", "" },
                { "@idTipoArquivo", "611" },
                { "@idObjeto", hddidItem.Value },
                { "@idUsuario", IDENTITY.Variaveis.idUsuario() }
            };
            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Arquivos", vParam);

            if (BD.ValidarDataSet(ds))
            {
                div_gvVideos.Visible = true;
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables_Videos", Grid.DataBindComScriptData(gvVideos, ds.Tables[0], 3, new int[1] { 3 }, "desc", "false", "''"), true);
            }
            else
                div_gvVideos.Visible = false;
        }

        protected void SalvarVideo(string name, byte[] bytes)
        {
            string sNomeArquivo = name;
            name = $"{FUNCOES.NormalizarTexto(name.Replace(".mp4", ""))}-{DateTime.Now:yyyyMMdd_HHmmss}.mp4";

            string videos = Server.MapPath("~/App/Videos/");
            Directory.CreateDirectory(videos);
            File.WriteAllBytes(Path.Combine(videos, name), bytes);

            Dictionary<string, string> vParam = new Dictionary<string, string>()
            {
                { "@sFuncao", "INCLUIR" },
                { "@idTipoArquivo", "611" },
                { "@idObjeto", hddidItem.Value },
                { "@sNomeArquivo", sNomeArquivo },
                { "@sObservacao", Path.Combine("/App/Videos/", name) },
                { "@idUsuario", IDENTITY.Variaveis.idUsuario() }
            };
            BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Arquivos", vParam);

            Pesquisar(hddidItem.Value);
        }

        void ExcluirArquivo(string idArquivo)
        {
            try
            {
                Dictionary<string, string> vParam = new Dictionary<string, string>()
                {
                    { "@sFuncao", "DOCUMENTO_DELETAR" },
                    { "@idArquivo", idArquivo },
                };
                DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Arquivos", vParam);

                if (File.Exists(Server.MapPath(RETORNO.DATASET(ds, "sObservacao")))) File.Delete(Server.MapPath(RETORNO.DATASET(ds, "sObservacao")));

                MensagemPagina_Videos.MostraMensagem_Sucesso("Arquivo Excluído com sucesso!", true);
            }
            catch (Exception ex)
            {
                MensagemPagina_Videos.MostraMensagem_Erro("Houve um erro na tentativa de Excluir o Arquivo de Vídeo!<br />Erro ao Excluir: " + ex.Message, true);
            }

            Pesquisar(hddidItem.Value);

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_MantemAba_Videos", "$('#aba_videos-tab').tab('show');\r\n\r\n", true);
        }

        void DownloadArquivo(string idArquivo)
        {
            try
            {
                Dictionary<string, string> vParam = new Dictionary<string, string>()
                {
                    { "@sFuncao", "CONSULTAR_DETALHE" },
                    { "@idArquivo", idArquivo },
                };
                DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Arquivos", vParam);

                string caminho = RETORNO.DATASET(ds, "sObservacao").Replace("~", "");
                string sNomeArquivo = RETORNO.DATASET(ds, "sNomeArquivo").Replace("~", "");

                FUNCOES.DirecionaPagina_NovaAba(Page, string.Format("/App/Paginas/Download.aspx?Video={0}&Nome={1}", caminho, sNomeArquivo));

                MensagemPagina_Videos.MostraMensagem_Sucesso("Download Arquivo com sucesso!", true);
            }
            catch (Exception ex)
            {
                MensagemPagina_Videos.MostraMensagem_Erro("Houve um erro na tentativa de Efetuar o Download do Arquivo de Vídeo!<br />Erro ao Excluir: " + ex.Message, true);
            }

            Pesquisar(hddidItem.Value);

            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "aba_videos-tab");
        }

        #endregion

        #region | Script

        protected void RegistraScript()
        {
            FUNCOES.Scripts.Aplica_TooltipPersonalizado(Page, "tooltip", "right");

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("$(document).ready(function () {");
            sb.AppendLine("     var $txtsDscAlteracao, $txtsDscDocumentacao, $txtsDscTeste, $txtsDscTreinamento;");
            sb.AppendLine("");
            sb.AppendLine($"    $txtsDscAlteracao = $find('{txtsDscAlteracao.ClientID}');");
            sb.AppendLine($"    $txtsDscDocumentacao = $find('{txtsDscDocumentacao.ClientID}');");
            sb.AppendLine($"    $txtsDscTeste = $find('{txtsDscTeste.ClientID}');");
            sb.AppendLine($"    $txtsDscTreinamento = $find('{txtsDscTreinamento.ClientID}');");
            sb.AppendLine("");
            sb.AppendLine("     if ($txtsDscAlteracao) $txtsDscAlteracao.value = (decodeURIComponent($('[id*=hddAlteracao]').val()));");
            sb.AppendLine("     if ($txtsDscDocumentacao) $txtsDscDocumentacao.set_content(decodeURIComponent($('[id*=hddDocumentacao]').val()));");
            sb.AppendLine("     if ($txtsDscTeste) $txtsDscTeste.set_content(decodeURIComponent($('[id*=hddTeste]').val()));");
            sb.AppendLine("     if ($txtsDscTreinamento) $txtsDscTreinamento.set_content(decodeURIComponent($('[id*=hddTreinamento]').val()));");
            sb.AppendLine("");
            sb.AppendLine("     function atualizarCampos() {");
            sb.AppendLine("         if ($txtsDscAlteracao) $('[id*=hddAlteracao]').val(encodeURIComponent($txtsDscAlteracao.get_content()));");
            sb.AppendLine("         if ($txtsDscDocumentacao) $('[id*=hddDocumentacao]').val(encodeURIComponent($txtsDscDocumentacao.get_content()));");
            sb.AppendLine("         if ($txtsDscTeste) $('[id*=hddTeste]').val(encodeURIComponent($txtsDscTeste.get_content()));");
            sb.AppendLine("         if ($txtsDscTreinamento) $('[id*=hddTreinamento]').val(encodeURIComponent($txtsDscTreinamento.get_content()));");
            sb.AppendLine("     }");
            sb.AppendLine("");
            sb.AppendLine("     setInterval(atualizarCampos, 250);");
            sb.AppendLine("");
            sb.AppendLine("     var $uploadContainer = $('[id*=uploadContainer]');");
            sb.AppendLine("     var $fileInput = $('[id*=fu_EnviarArquivo]');");
            sb.AppendLine("     var $fileNameDisplay = $('[id*=fileName]');");
            sb.AppendLine("");
            sb.AppendLine("     $uploadContainer.on('click', function () {");
            sb.AppendLine("         $fileInput.click();");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $fileInput.on('click', function (e) {");
            sb.AppendLine("         e.stopPropagation();");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $uploadContainer.on('dragover', function (e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         e.stopPropagation();");
            sb.AppendLine("         $uploadContainer.addClass('dragover');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $uploadContainer.on('dragleave', function (e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         e.stopPropagation();");
            sb.AppendLine("         $uploadContainer.removeClass('dragover');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $uploadContainer.on('drop', function (e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         e.stopPropagation();");
            sb.AppendLine("         $uploadContainer.removeClass('dragover');");
            sb.AppendLine("         var files = e.originalEvent.dataTransfer.files;");
            sb.AppendLine("         $fileInput[0].files = files;");
            sb.AppendLine("         displayFileName(files[0].name);");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $fileInput.on('change', function () {");
            sb.AppendLine("         if (this.files.length > 0) {");
            sb.AppendLine("             displayFileName(this.files[0].name);");
            sb.AppendLine("         }");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     function displayFileName(name) {");
            sb.AppendLine("         $fileNameDisplay.text(name);");
            sb.AppendLine("     }");
            sb.AppendLine("");
            sb.AppendLine("     $v192(function() {");
            sb.AppendLine("         $v192('#dialog_Apagar').dialog({");
            sb.AppendLine("             resizable: false,");
            sb.AppendLine("             height: 'auto',");
            sb.AppendLine("             width: 400,");
            sb.AppendLine("             modal: true,");
            sb.AppendLine("             autoOpen: false,");
            sb.AppendLine("             buttons:");
            sb.AppendLine("             {");
            sb.AppendLine("                 'Sim': function() {");
            sb.AppendLine("                     __doPostBack('ExcluirArquivo', '');");
            sb.AppendLine("                     $v192(this).dialog('close');");
            sb.AppendLine("                 },");
            sb.AppendLine("                 'Não': function() {");
            sb.AppendLine("                     $v192(this).dialog('close');");
            sb.AppendLine("                 },");
            sb.AppendLine("             }");
            sb.AppendLine("         });");
            sb.AppendLine("         $v192('.ExcluirArquivo').click(function(e) {");
            sb.AppendLine("             e.preventDefault();");
            sb.AppendLine("             $v192($('[id*=hddExcluirArquivo]').val($v192(this).closest('tr').find('.idArquivo').text()));");
            sb.AppendLine("             $v192('#dialog_Apagar').dialog('open');");
            sb.AppendLine("         });");
            sb.AppendLine("");
            sb.AppendLine("         $v192('.DownloadArquivo').click(function (e) {");
            sb.AppendLine("             e.preventDefault();");
            sb.AppendLine("             $v192($('[id*=hddDownloadArquivo]').val($v192(this).closest('tr').find('.idArquivo').text()));");
            sb.AppendLine("             __doPostBack('DownloadArquivo', '');");
            sb.AppendLine("         });");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $('[id*=cmdUpload]').click(function(e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         $('#modalUploadVideo').modal('show');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $('.Download').click(function(e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         $('.videoPlayer').attr('src', $(this).data('obs'));");
            sb.AppendLine("         $('#modalVideoPlayer').modal('show');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("});");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_RegistraScript", sb.ToString(), true);
        }

        #endregion
    }
}
