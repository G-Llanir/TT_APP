using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using TT_Hub.FrameWork;
using static TT.FrameWork.Identity;
using static TT.FrameWork.BD;
using static TT.FrameWork.Grid;
using System.Net;
using System.Text;
using System.Security.Cryptography;

namespace TT_Hub.App.Paginas.Manutencao
{
    public partial class Equipamento_Detalhe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string idCliente;
            FUNCOES.ValidarPermissaoAcesso();

            RegistraScript("");
            if (!IsPostBack)
            {
                idCliente = IDENTITY.Variaveis.idCliente();
                PreencheCombos();

                if (Request["id"] != null)
                {
                    PesquisarEquipamento(Request["id"].ToString(), idCliente);
                }
                else
                {
                    PesquisarEquipamento("0", idCliente);
                }

                if (Request["pag"] != null)
                {
                    if (Request["pag"] == "db")
                    {
                        BloqueiaControles_ApenasConsulta();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Tab_Tarefas", "$('#informacoes-tab').tab('show');", true);
                    }
                }
            }
            else
            {
                var requestTarget = this.Request["__EVENTTARGET"];
                var requestArgs = this.Request["__EVENTARGUMENT"];

                if (requestTarget == "funcao_SAIR")
                {
                    FUNCOES.DirecionaPagina("/app/dashboard.aspx");
                }
                else if (requestTarget == "funcao_SALVAR")
                {
                    GravarEquipamento();
                }
            }
        }

        protected string RetornarScripts()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("$v192(function() {");

            sb.Append("$v192(\"#dialog-Salvar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_SALVAR\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('#ContentPlaceHolder1_cmdSalvar').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Salvar').dialog('open');");
            sb.Append("});");

            sb.Append("});");

            return sb.ToString();
        }

        void RegistraScript(string sScript)
        {
            sScript = RetornarScripts() + sScript;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "AddShowModalScript", sScript, true);
        }

        void PreencheCombos()
        {
            FUNCOES.Popula_Combo(ddlCliente, "sp_Select 'CLIENTE', " + IDENTITY.Variaveis.idCliente(), "idCliente", "sDscCliente", false, "Selecione o Ciente", "0");
            FUNCOES.Popula_Combo(ddlTipoEquipamento, "sp_Select 'TIPO_EQUIPAMENTO'", "idTipoEquipamento", "sDscTipoEquipamento", false, "Selecione o modelo do equipamento", "0");
            FUNCOES.Popula_Combo(ddlsUF, "sp_Select 'UF'", "sEstado", "sEstado", false, "UF", "");
            FUNCOES.Popula_Combo(ddlOperadora, "sp_Select 'OPERADORA'", "idOperadora", "sDscOperadora", false, "Selecionar uma Operadora", "0");
            FUNCOES.Popula_Combo(ddlidToken, "sp_Select 'HUB_Token_Sigma'", "idToken", "sDscToken_Completo", false);
            FUNCOES.Popula_Combo(ddlidTipoMonitoramento, "sp_Select 'HUB_TipoMonitoramento'", "idTipoMonitoramento", "sDscTipoMonitoramento", false, "Selectione o  Tipo de Monitoramento", "0");
        }

        void LimparCampos()
        {
            txtsDescricao.Text = "";
            txtsDscCliente.Text = "";
            txtsDscUnidade.Text = "";
            ddlCliente.SelectedValue = "0";
            ddlTipoEquipamento.SelectedValue = "0";
            ddlidTipoMonitoramento.SelectedValue = "0";
            txtsID.Text = "";
            txtsw_Account.Text = "";
            ddlOperadora.SelectedValue = "0";
            txtsIMEI.Text = "";
            txtsContatoTecnico.Text = "";
            txtsTelefoneTecnico.Text = "";
            txtsContatoOutros.Text = "";
            txtsTelefoneOutros.Text = "";
            txtsCEP.Text = "";
            txtsLogradouro.Text = "";
            txtsNumero.Text = "";
            txtsComplemento.Text = "";
            txtsCidade.Text = "";
            ddlsUF.SelectedValue = "";
            txtsEnderecoIP.Text = "";
            txtsPorta.Text = "";
            txtsUsuario.Text = "";
            txtsSenha.Text = "";
            ddlsMonitorar_Tensao_Bateria.Text = "Monitorar Tensão de Bateria";
            ddlsMonitorar_Tensao_Rede.Text = "Monitorar Tensão de Rede";
            ddlsMonitorar_Tensao_Bateria.Situacao_Definir("N");
            ddlsMonitorar_Tensao_Rede.Situacao_Definir("N");
            ddlidToken.SelectedValue = "0";
            txtsDscCliente.Visible = false;
            txtsDscUnidade.Visible = false;
            ddlCliente.Visible = false;
            ddlidUnidade.Visible = false;
        }

        void PesquisarEquipamento(string idEquipamento, string idCliente)
        {
            string sErro = "";
            try
            {
                LimparCampos();
                pnlCadastro.Visible = false;
                abaInformacoes.Visible = false;
                abaHistorico.Visible = false;
                abaSigma.Visible = false;
                hddIdEquipamento.Value = "0";
                cmdSalvar.Text = "Salvar";

                if (idEquipamento != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@idEquipamento", idEquipamento);
                    vParametros.Add("@sFuncao", "CONSULTA_DETALHE");
                    vParametros.Add("@idCliente", idCliente);
                    dsPesquisa = BD.ExecutarDataSet("sp_HUB_Manipula_tbl_Equipamentos", vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        txtsDescricao.Text = RETORNO.DATASET(dsPesquisa, 0, "sDescricao");
                        txtsDscCliente.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscCliente");
                        txtsDscUnidade.Text = RETORNO.DATASET(dsPesquisa, "sDscUnidade");
                        ddlCliente.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idCliente");
                        Popular_Combo_idUnidade(RETORNO.DATASET(dsPesquisa, 0, "idCliente"));
                        ddlidUnidade.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idUnidade");
                        ddlTipoEquipamento.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idTIpoEquipamento");
                        ddlidTipoMonitoramento.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idTipoMonitoramento");
                        txtsID.Text = RETORNO.DATASET(dsPesquisa, 0, "sID");
                        txtsHostName.Text = RETORNO.DATASET(dsPesquisa, 0, "sHostName");
                        txtsw_Account.Text = RETORNO.DATASET(dsPesquisa, 0, "sw_Account");
                        txtsContatoTecnico.Text = RETORNO.DATASET(dsPesquisa, 0, "sContatoTecnico"); ;
                        txtsTelefoneTecnico.Text = RETORNO.DATASET(dsPesquisa, 0, "sTelefoneTecnico"); ;
                        txtsContatoOutros.Text = RETORNO.DATASET(dsPesquisa, 0, "sContatoOutros"); ;
                        txtsTelefoneOutros.Text = RETORNO.DATASET(dsPesquisa, 0, "sTelefoneOutros"); ;
                        txtsCEP.Text = RETORNO.DATASET(dsPesquisa, 0, "sCEP");
                        txtsLogradouro.Text = RETORNO.DATASET(dsPesquisa, 0, "sLogradouro");
                        txtsNumero.Text = RETORNO.DATASET(dsPesquisa, 0, "sNumero");
                        txtsComplemento.Text = RETORNO.DATASET(dsPesquisa, 0, "sComplemento");
                        txtsBairro.Text = RETORNO.DATASET(dsPesquisa, 0, "sBairro");
                        txtsEnderecoIP.Text = RETORNO.DATASET(dsPesquisa, 0, "sEnderecoIP");
                        txtsPorta.Text = RETORNO.DATASET(dsPesquisa, 0, "sPorta");
                        txtsUsuario.Text = RETORNO.DATASET(dsPesquisa, 0, "sUsuario");
                        txtsSenha.Text = RETORNO.DATASET(dsPesquisa, 0, "sSenha");
                        ddlidToken.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idToken");
                        ddlsMonitorar_Tensao_Bateria.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sMonitorar_Tensao_Bateria"));
                        ddlsMonitorar_Tensao_Rede.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sMonitorar_Tensao_Rede"));
                        ddlsUF.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sUF");
                        txtsCidade.Text = RETORNO.DATASET(dsPesquisa, 0, "sCidade");
                        ddlOperadora.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idOperadora");
                        txtsIMEI.Text = RETORNO.DATASET(dsPesquisa, 0, "sIMEI");
                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"));
                        lblTituloPagina.Text = string.Format("Consulta Equipamento {0} - {1}", RETORNO.DATASET(dsPesquisa, 0, "sDscTipoEquipamento"), RETORNO.DATASET(dsPesquisa, 0, "sDescricao"));
                        BreadCrumb.TitulodaPagina = txtsDescricao.Text;
                        hddIdEquipamento.Value = idEquipamento;
                        cmdSalvar.Text = "Salvar";
                        lblTituloSalvar.Text = "Confirma a alteração nos dados do equipamento n.° " + RETORNO.DATASET(dsPesquisa, 0, "sDescricao") + "?";
                        txtsDscCliente.Visible = true;
                        txtsDscUnidade.Visible = true;

                        CarregarAbaInformacoes(idEquipamento);
                        CarregaAbaHistorico(idEquipamento);
                        CarregaAbaSigma(idEquipamento);
                        pnlCadastro.Visible = true;
                    }
                    else
                    {
                        throw new Exception("Erro ao consultar BD: " + sErro);
                    }
                }
                else
                {
                    BreadCrumb.TitulodaPagina = "Novo Equipamento";
                    lblTituloPagina.Text = "Novo Equipamento";
                    cmdSalvar.Text = "Incluir";
                    lblTituloSalvar.Text = "Confirma a INCLUSÃO do Equipamento?";
                    txtsDescricao.Focus();
                    hddIdEquipamento.Value = "0";
                    ddlCliente.Visible = true;
                    ddlidUnidade.Visible = true;
                    pnlCadastro.Visible = true;
                    PainelAtualizacao.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao consultar BD: " + ex.Message);
                pnlCadastro.Visible = false;
            }
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {

        }

        void CarregarAbaInformacoes(string idEquipamento)
        {
            DataTable tbInformacoes;
            Dictionary<String, String> vParametros_Informacoes = new Dictionary<string, string>();
            vParametros_Informacoes.Add("@idEquipamento", idEquipamento);
            tbInformacoes = BD.ExecutarDataTable("sp_HUB_Select_tbl_Equipamentos_Status", vParametros_Informacoes, false);
            if (tbInformacoes.Rows.Count > 0)
            {
                gvInformacoes.DataSource = tbInformacoes;
                gvInformacoes.DataBind();
                abaInformacoes.Visible = true;
            }
        }

        void CarregaAbaSigma(string idEquipamento)
        {
            DataTable tbLink;
            string sSql_Link = "sp_Select";
            Dictionary<String, String> vParametros_Link = new Dictionary<string, string>();
            vParametros_Link.Add("@sTabela", "LINK_API");
            vParametros_Link.Add("@idPesquisa", idEquipamento);
            vParametros_Link.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());
            tbLink = BD.ExecutarDataTable(sSql_Link, vParametros_Link, false);

            if (tbLink.Rows.Count > 0)
            {
                gvLinks.DataSource = tbLink;
                gvLinks.DataBind();
                rptBotoes.DataSource = tbLink;
                rptBotoes.DataBind();
                pn_Link.Visible = true;
                abaSigma.Visible = true;
            }
        }

        void CarregaAbaHistorico(string idEquipamento)
        {
            DataTable tb;
            string sSql = "sp_HUB_Select_Equipamento_Historico";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@idEquipamento", idEquipamento);

            tb = BD.ExecutarDataTable(sSql, vParametros, false);
            if (tb.Rows.Count > 0)
            {
                dtgConsultaHistorico.DataSource = tb;
                dtgConsultaHistorico.DataBind();
                abaHistorico.Visible = true;
            }
        }

        void GravarEquipamento()
        {
            string sErro = "";
            string sLatitude = "";
            string sLongitude = "";

            if (AplicarValidacoes())
            {
                try
                {
                    string[] vidEquipamento = hddIdEquipamento.Value.Split(',');
                    string idEquipamentoUtilizar = vidEquipamento[0].ToString();

                    DataSet dsGravar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "Salvar");
                    vParametros.Add("@idEquipamento", hddIdEquipamento.Value.ToString());
                    vParametros.Add("@idTIpoEquipamento", ddlTipoEquipamento.SelectedValue.ToString());
                    vParametros.Add("@idTipoMonitoramento", ddlidTipoMonitoramento.SelectedValue.ToString());
                    vParametros.Add("@idCliente", ddlCliente.SelectedValue.ToString());
                    vParametros.Add("@idUnidade", ddlidUnidade.SelectedValue);
                    vParametros.Add("@sID", txtsID.Text.ToUpper());
                    vParametros.Add("@sDescricao", txtsDescricao.Text);
                    vParametros.Add("@sHostName", txtsHostName.Text);
                    vParametros.Add("@sw_Account", txtsw_Account.Text);
                    vParametros.Add("@sContatoTecnico", txtsContatoTecnico.Text);
                    vParametros.Add("@sTelefoneTecnico", txtsTelefoneTecnico.Text);
                    vParametros.Add("@sContatoOutros", txtsContatoOutros.Text);
                    vParametros.Add("@sTelefoneOutros", txtsTelefoneOutros.Text);
                    vParametros.Add("@sLogradouro", txtsLogradouro.Text.ToUpper());
                    vParametros.Add("@sNumero", txtsNumero.Text);
                    vParametros.Add("@sComplemento", txtsComplemento.Text.ToUpper());
                    vParametros.Add("@sBairro", txtsBairro.Text.ToUpper());
                    vParametros.Add("@sCEP", txtsCEP.Text.Replace("-", ""));
                    vParametros.Add("@sCidade", txtsCidade.Text.ToUpper());
                    vParametros.Add("@sUF", ddlsUF.SelectedValue.ToString());
                    vParametros.Add("@sSituacao", ComboAtivo.Situacao_Recuperar());
                    vParametros.Add("@idOperadora", ddlOperadora.SelectedValue.ToString());
                    vParametros.Add("@sIMEI", txtsIMEI.Text.ToString());
                    vParametros.Add("@sEnderecoIP", txtsEnderecoIP.Text.ToString());
                    vParametros.Add("@sPorta", txtsPorta.Text.ToString());
                    vParametros.Add("@sUsuario", txtsUsuario.Text.ToString());
                    vParametros.Add("@sSenha", txtsSenha.Text.ToString());
                    vParametros.Add("@idToken", ddlidToken.SelectedValue);
                    vParametros.Add("@sMonitorar_Tensao_Bateria", ddlsMonitorar_Tensao_Bateria.Situacao_Recuperar());
                    vParametros.Add("@sMonitorar_Tensao_Rede", ddlsMonitorar_Tensao_Rede.Situacao_Recuperar());
                    vParametros.Add("@sLatitude", sLatitude);
                    vParametros.Add("@sLongitude", sLongitude);
                    vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());

                    dsGravar = BD.ExecutarDataSet("sp_HUB_Manipula_tbl_Equipamentos", vParametros);

                    if (BD.ValidarDataSet(dsGravar, out sErro))
                    {
                        idEquipamentoUtilizar = RETORNO.DATASET(dsGravar, 0, "idEquipamento");
                        PesquisarEquipamento(idEquipamentoUtilizar, IDENTITY.Variaveis.idCliente());
                        MensagemPagina.MostraMensagem_Sucesso("Equipamento gravado com sucesso!");
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
                RegistraScript("");
            }
        }

        protected void txtsCEP_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DataSet dsCEP;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sCEP", txtsCEP.Text.Replace("-", ""));
                dsCEP = BD.ExecutarDataSet("sp_HUB_Consulta_CEP", vParametros);

                if (BD.ValidarDataSet(dsCEP))
                {
                    txtsLogradouro.Text = RETORNO.DATASET(dsCEP, 0, "sLogradouro");
                    txtsBairro.Text = RETORNO.DATASET(dsCEP, 0, "sBairro");
                    ddlsUF.SelectedValue = RETORNO.DATASET(dsCEP, 0, "sUF");
                    txtsCidade.Text = RETORNO.DATASET(dsCEP, 0, "sCidade");
                }
                else
                {

                }
            }
            catch
            {
                txtsLogradouro.Text = "";
                txtsBairro.Text = "";
                ddlsUF.SelectedValue = "";
                txtsBairro.Text = "";
                txtsLogradouro.Focus();
            }
            RegistraScript("");
        }

        bool AplicarValidacoes()
        {
            bool retorno = true;

            if (txtsDescricao.Text.Length < 4)
            {
                MensagemPagina.MostraMensagem_Erro("Informe uma Descrição para o equipamento!");
                return false;
            }
            if (ddlCliente.Visible == true)
            {
                if (ddlCliente.SelectedValue == "0")
                {
                    MensagemPagina.MostraMensagem_Erro("Selecione um Cliente!");
                    return false;
                }
            }
            if (ddlidUnidade.Visible == true)
            {
                if (ddlidUnidade.SelectedValue == "0" && ddlidUnidade.Items.Count > 1)
                {
                    MensagemPagina.MostraMensagem_Erro("Selecione uma unidade!");
                    return false;
                }
            }
            if (ddlidTipoMonitoramento.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione um tipo de Monitoramento!");
                return false;
            }
            if (ddlTipoEquipamento.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione o modelo do Equipamento!");
                return false;
            }
            if (txtsID.Text.Length < 4)
            {
                MensagemPagina.MostraMensagem_Erro("Informe o ID do Equipamento!");
                return false;
            }
            if (ddlOperadora.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Informe a Operadora!");
                return false;
            }
            if (Convert.ToInt32(ddlOperadora.SelectedValue) > 1 && txtsIMEI.Text.Length < 4)
            {
                MensagemPagina.MostraMensagem_Erro("Informe o IMEI do CHIP");
                return false;
            }
            if (txtsCEP.Text.Length < 8)
            {
                MensagemPagina.MostraMensagem_Erro("Informe um CEP Válido!");
                return false;
            }
            if (txtsLogradouro.Text.Length < 4)
            {
                MensagemPagina.MostraMensagem_Erro("Informe um Endereço!");
                return false;
            }
            if (txtsNumero.Text.Length < 1)
            {
                MensagemPagina.MostraMensagem_Erro("Informe o número!");
                return false;
            }
            if (txtsCidade.Text.Length < 4)
            {
                MensagemPagina.MostraMensagem_Erro("Informe a Cidade!");
                return false;
            }
            if (ddlsUF.SelectedValue == "")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione o Estado!");
                return false;
            }
            return retorno;
        }

        void BloqueiaControles_ApenasConsulta()
        {
            txtsID.ReadOnly = true;
            txtsBairro.ReadOnly = true;
            txtsContatoTecnico.ReadOnly = true;
            txtsTelefoneTecnico.ReadOnly = true;
            txtsTelefoneOutros.ReadOnly = true;
            txtsContatoOutros.ReadOnly = true;
            txtsCEP.ReadOnly = true;
            txtsCidade.ReadOnly = true;
            txtsComplemento.ReadOnly = true;
            txtsDescricao.ReadOnly = true;
            txtsDscCliente.ReadOnly = true;
            txtsDscUnidade.ReadOnly = true;
            txtsHostName.ReadOnly = true;
            txtsID.ReadOnly = true;
            txtsIMEI.ReadOnly = true;
            txtsLogradouro.ReadOnly = true;
            txtsNumero.ReadOnly = true;
            txtsw_Account.ReadOnly = true;
            txtsEnderecoIP.ReadOnly = true;
            txtsPorta.ReadOnly = true;
            txtsUsuario.ReadOnly = true;
            txtsSenha.ReadOnly = true;
            ddlsMonitorar_Tensao_Bateria.ReadOnly = true;
            ddlsMonitorar_Tensao_Rede.ReadOnly = true;
            ComboAtivo.ReadOnly = true;
            cmdSalvar.Visible = false;
            ddlTipoEquipamento.Attributes.Add("disabled", "disabled");
            ddlOperadora.Attributes.Add("disabled", "disabled");
            ddlsUF.Attributes.Add("disabled", "disabled");
            ddlidToken.Attributes.Add("disabled", "disabled");
            ddlidTipoMonitoramento.Attributes.Add("disabled", "disabled");
        }

        protected void dtgConsultaHistorico_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (DataBinder.Eval(e.Row.DataItem, "sLink").ToString() == "")
                {
                    HyperLink hpl1 = (e.Row.FindControl("hplDescricao") as HyperLink);
                    hpl1.Visible = false;
                }
                else
                {
                    Label lbl1 = (e.Row.FindControl("lblDescricao") as Label);
                    lbl1.Visible = false;
                }
            }
        }

        protected void gvInformacoes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (DataBinder.Eval(e.Row.DataItem, "sOK").ToString() == "N")
                {
                    e.Row.CssClass = "danger";
                }
            }
        }

        protected void ddlCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            Popular_Combo_idUnidade(ddlCliente.SelectedValue);
        }

        void Popular_Combo_idUnidade(string idCliente)
        {
            FUNCOES.Popula_Combo(ddlidUnidade, "sp_Manipula_tbl_HUB_Cliente_Unidade 'CONSULTAR', " + idCliente, "idUnidade", "sDScUnidade", false, "Selecione a Unidade", "0");
        }

        protected void ExecutarComando(object sender, EventArgs e)
        {
            string idRegistroAcao = hddidRegistroAcao.Value;
            try
            {
                TT_Hub.API.Commbox commbox = new API.Commbox();
                commbox.EnviarAcaoEquipamento(idRegistroAcao);
                msgAcoes.MostraMensagem_Sucesso("Comando enviando com Sucesso");
            }
            catch (Exception ex)
            {
                msgAcoes.MostraMensagem_Erro(ex.Message);
            }
            finally
            {
                PesquisarEquipamento(hddIdEquipamento.Value, IDENTITY.Variaveis.idCliente());
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Tab_Tarefas", "$('#informacoes-tab').tab('show');", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Tab_Focus", "$('#ContentPlaceHolder1_gvExecutarAcao').focus();", true);
                msgAcoes.Focus();
            }
        }

        protected void timer_AtualizarInformacoes_Tick(object sender, EventArgs e)
        {
            CarregarAbaInformacoes(hddIdEquipamento.Value);
            CarregaAbaHistorico(hddIdEquipamento.Value);
            timer_AtualizarInformacoes.Enabled = true;
        }

        protected void cmdEquipamentos_Click(object sender, EventArgs e)
        {
            string sRetorno = "";
            try
            {
                string sFuncao = "REGISTRAR ACAO";
                string sObservacao = "Execução via HUB";
                string idEquipamento = hddIdEquipamento.Value;
                LinkButton linkButton = (LinkButton)sender;
                string CodigoAcao_SW = linkButton.CommandArgument;

                DataSet dsAcao;
                Dictionary<String, String> vParametrosAcao = new Dictionary<string, string>();

                vParametrosAcao.Add("@sFuncao", sFuncao);
                vParametrosAcao.Add("@sObservacao", sObservacao);
                vParametrosAcao.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());
                vParametrosAcao.Add("@idEquipamento", idEquipamento);
                vParametrosAcao.Add("@CodigoAcao_SW", CodigoAcao_SW);
                dsAcao = BD.ExecutarDataSet("sp_HUB_Manipula_tbl_Eventos_Ocorrencia_Acao", vParametrosAcao);

                if (BD.ValidarDataSet(dsAcao))
                {
                    if (BD.Retorno.DATASET(dsAcao, "sExecutaImediato") == "S")
                    {
                        hddidRegistroAcao.Value = BD.Retorno.DATASET(dsAcao, "idRegistroAcao");
                        ExecutarComando(sender, e);
                    }
                    else
                    {
                        PesquisarEquipamento(hddIdEquipamento.Value, IDENTITY.Variaveis.idCliente());
                        msgAcoes.MostraMensagem_Sucesso("Comando enviando com Sucesso");
                    }
                }
            }
            catch (Exception ex)
            {
                sRetorno = "#Erro no Envio: " + ex.Message;
            }
        }
    }
}