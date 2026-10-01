using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using TT_Flow.FrameWork;
using TT.FrameWork;
using static TT.FrameWork.BD;
using System.Text;
using System.IO;
using System.Globalization;


namespace TT_Flow.App.Paginas.Adm.Manutencao
{
    public partial class ContasBancarias_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Conta Bancária";
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_ContasBancarias";

        #region | Classes

        public List<FrameWork.cls_Contatos> bs_Contatos
        {

            get
            {
                if (ViewState["bs_Contatos"] == null)
                {
                    ViewState["bs_Contatos"] = new List<FrameWork.cls_Contatos>();
                }
                return (List<cls_Contatos>)ViewState["bs_Contatos"];
            }

            set
            {
                ViewState["bs_Contatos"] = value;
            }

        }

        public List<FrameWork.cls_CartaoCredito> bs_CartaoCredito
        {
            get
            {
                if (ViewState["bs_CartaoCredito"] == null)
                {
                    ViewState["bs_CartaoCredito"] = new List<FrameWork.cls_CartaoCredito>();
                }
                return (List<FrameWork.cls_CartaoCredito>)ViewState["bs_CartaoCredito"];
            }

            set
            {
                ViewState["bs_CartaoCredito"] = value;
            }

        }

        public List<FrameWork.cls_ProdutosFinanceiros> bs_ProdutosFinanceiros
        {
            get
            {
                if (ViewState["bs_ProdutosFinanceiros"] == null)
                {
                    ViewState["bs_ProdutosFinanceiros"] = new List<FrameWork.cls_ProdutosFinanceiros>();
                }
                return (List<FrameWork.cls_ProdutosFinanceiros>)ViewState["bs_ProdutosFinanceiros"];
            }

            set
            {
                ViewState["bs_ProdutosFinanceiros"] = value;
            }

        }


        #endregion

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();


            if (!IsPostBack)
            {

                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Administracao.ContasBancarias.Consultar, true);
                    Pesquisar(Request["id"].ToString(), false);
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Administracao.ContasBancarias.Incluir, true);
                    Pesquisar("0", true);

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
                    Salvar_ContasBancarias();
                }
                else if (requestTarget == "funcao_Editar")
                {
                    Pesquisar(hddidConta.Value, true);
                }
            }

            Funcoes.Scripts.Aplica_TooltipPersonalizado(this.Page, "tooltip");
            RegistraScript("");

        }


        #endregion

        #region |Metodos Banco de Dados
        protected void Pesquisar(string idConta, bool bEdicao)
        {
            PopularCombos();
            aba_Historico.Visible = false;
            aba_ProdutosFinanceiros.Visible = false;

            string sErro = "";

            try
            {
                LimpaCampos();

                if (idConta != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idConta", idConta);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidConta.Value = RETORNO.DATASET(dsPesquisa, 0, "idConta");
                        txtidConta.Text = RETORNO.DATASET(dsPesquisa, 0, "idConta");
                        ddlidBanco.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idBanco");
                        txtsDsConta.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscConta");
                        ddlsTipoConta.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sTipoConta");
                        txtsAgencia.Text = RETORNO.DATASET(dsPesquisa, 0, "sAgencia");
                        txtsNumeroConta.Text = RETORNO.DATASET(dsPesquisa, 0, "sNumeroConta");
                        txtsChavePix.Text = RETORNO.DATASET(dsPesquisa, 0, "sChavePix");
                        ddlidEmpresa.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idEmpresa");
                        txtnValorIncial.Text = RETORNO.DATASET(dsPesquisa, 0, "nValorIncial", true);
                        txtnValorAtual.Text = RETORNO.DATASET(dsPesquisa, 0, "nValorAtual", true);
                        ddlidLancamentoContabol.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idLancamentoContabol");
                        txtsNomePacoteTarifa.Text = RETORNO.DATASET(dsPesquisa, 0, "sNomePacoteTarifa");
                        txtnValorMensalTarifa.Text = RETORNO.DATASET(dsPesquisa, 0, "nValorMensalTarifa", true);
                        txtsObservacao.Text = RETORNO.DATASET(dsPesquisa, 0, "sObservacao");

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));

                        lblTituloPagina.Text = string.Format("Conta Bancária {0}", RETORNO.DATASET(dsPesquisa, 0, "sDscConta"));
                        BreadCrumb.TitulodaPagina = string.Format("Conta Bancária {0}", RETORNO.DATASET(dsPesquisa, 0, "sDscConta"));

                        lblTituloSalvar.Text = "Confirma a Alteração da " + lblTituloPagina.Text + "?";

                        Popular_Aba_Historico(dsPesquisa);
                        Popular_dtgProdutosFinanceiros(dsPesquisa);
                        Popular_dtgContatos(dsPesquisa);
                        Popular_dtgCartaoCredito(dsPesquisa);

                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Administracao.ContasBancarias.Alterar);
                        aba_ProdutosFinanceiros.Visible = true;
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }

                }
                else
                {
                    BreadCrumb.TitulodaPagina = string.Format("Nova {0}", sTituloPagina);
                    lblTituloPagina.Text = string.Format("Nova {0}", sTituloPagina);
                    txtidConta.Text = "Novo";
                    lblTituloSalvar.Text = "Confirma a Inclusão da Conta Bancária?";
                    cmdSalvar.Text = "Incluir";
                    txtsDsConta.Focus();

                }
                RegistraScript("");

            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }


        void Salvar_ContasBancarias()
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {
                    string[] vidConta = hddidConta.Value.Split(',');
                    string idConta = vidConta[0].ToString();

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    //Cadastro
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idConta", idConta);
                    vParametros.Add("@idBanco", ddlidBanco.SelectedValue);
                    vParametros.Add("@sDscConta", txtsDsConta.Text);
                    vParametros.Add("@sTipoConta", ddlsTipoConta.SelectedValue);
                    vParametros.Add("@sAgencia", txtsAgencia.Text);
                    vParametros.Add("@sNumeroConta", txtsNumeroConta.Text);
                    vParametros.Add("@sChavePix", txtsChavePix.Text);
                    vParametros.Add("@idEmpresa", ddlidEmpresa.SelectedValue);
                    vParametros.Add("@nValorIncial", BD.Conversoes.Numerico(txtnValorIncial));
                    vParametros.Add("@nValorAtual", BD.Conversoes.Numerico(txtnValorAtual));
                    vParametros.Add("@idLancamentoContabol", ddlidLancamentoContabol.SelectedValue);
                    vParametros.Add("@sNomePacoteTarifa", txtsNomePacoteTarifa.Text);
                    vParametros.Add("@nValorMensalTarifa", BD.Conversoes.Numerico(txtnValorMensalTarifa));
                    vParametros.Add("@sObservacao", txtsObservacao.Text);
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());

                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);
                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idConta = RETORNO.DATASET(dsSalvar, "idConta");
                        if (Salvar_ProdutosFinanceiros(idConta) && Salvar_Contatos(idConta) && Salvar_CartaoCredito(idConta))
                        {
                            Pesquisar(idConta, false);
                            MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                        }
                        else
                        {
                            throw new Exception("Erro ao Salvar Produto Financeiro:");
                        }
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
            RegistraScript("");
        }

        #endregion



        #region | Limpar Campos
        void LimpaCampos()
        {
            hddidConta.Value = "0";
            txtidConta.Text = "Novo";
            ddlidBanco.SelectedValue = "0";
            txtsDsConta.Text = "";
            ddlsTipoConta.SelectedValue = "0";
            txtsAgencia.Text = "";
            txtsNumeroConta.Text = "";
            txtsChavePix.Text = "";
            ddlidEmpresa.SelectedValue = "0";
            txtnValorIncial.Text = "";
            txtnValorAtual.Text = "";
            ddlidLancamentoContabol.SelectedValue = "0";
            txtsNomePacoteTarifa.Text = "";
            txtnValorMensalTarifa.Text = "";
            txtsObservacao.Text = "";

            bs_ProdutosFinanceiros.Clear();
            bs_Contatos.Clear();
            bs_CartaoCredito.Clear();
            aba_ProdutosFinanceiros.Visible = false;

            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
        }

        #endregion

        #region | Validação 
        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (txtsDsConta.Text.Length < 5)
            {
                sMensagemErro = "Nome da conta inválida!";
            }


            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
            }


            return bRetorno;
        }
        #endregion

        #region | Script 
        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            //Mensagens de Confirmação
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
            sb.Append("$v192('[id*=cmdSalvar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Salvar').dialog('open');");
            sb.Append("});");

            sb.Append("$('[id*=txtProdutosFinanceiros_dtInicio]').mask('99/99/9999');");
            sb.Append("$('[id*=txtProdutosFinanceiros_dtVencimento]').mask('99/99/9999');");

            sb.Append("$('[id*=txtnValorMensalTarifa]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnValorIncial]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtProdutosFinanceiros_nLimiteConta]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtProdutosFinanceiros_nPorcentagemTaxaConta]').mask('000.000.000.000.000,00', { reverse: true });");

            sb.Append("$('[id*=txtContatos_sTelefoneContato]').mask('(99) 99999-9999');");
            sb.Append("$('[id*=txtProdutosFinanceiros_dtInicio]').datepicker({autoclose: true, format: 'dd/mm/yyyy',language: 'pt-BR'});");
            sb.Append("$('[id*=txtProdutosFinanceiros_dtVencimento]').datepicker({autoclose: true, format: 'dd/mm/yyyy', language: 'pt-BR'});");
            sb.Append("$('[id*=txtCartaoCredito_dtVenciCartao]').datepicker({autoclose: true,format: 'dd/mm/yyyy', language: 'pt-BR'});");

            sb.Append("$('[id*=txtnNumCartao]').mask('0000 0000 0000 0000');");
            sb.Append("$('[id*=txtnProvisaoGastos]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnLimiteCartao]').mask('000.000.000.000.000,00', { reverse: true });");

            sb.Append("});");

            if (sFuncao != "")
            {
                sb.Append(sFuncao);
            }

            // ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina" + Guid.NewGuid(), sb.ToString(), true);
        }
        #endregion

        #region | Combos/DDL
        void PopularCombos()
        {
            //FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_empresa'", "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");
            FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa', @idUsuario=" + IDENTITY.Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");

            FUNCOES.Popula_Combo(ddlidBanco, "sp_Select 'Flow_Bancos'", "idBanco", "sDscBanco", false, "Selecione o Banco", "0");

            FUNCOES.Popula_Combo(ddlidLancamentoContabol, "sp_Select 'Flow_CodigoContabil'", "idContabil", "sCodContabil", false, "Selecione o Lançamento", "0");

            FUNCOES.Popula_Combo(ddlProdutosFinanceiros_idTipoConta, "sp_Select 'tbl_Flow_Adm_TipoConta'", "idTipoConta", "sDscTipoConta", false, "Selecione o Tipo Conta", "0");

            FUNCOES.Popula_Combo(ddlidBandeira, "sp_Select 'tbl_Flow_Adm_BandeiraCartoes'", "idBandeira", "sDscBandeira", false, "Selecione a Bandeira", "0");

            FUNCOES.Popula_Combo(ddlidUsuario, "sp_Select 'Flow_Colaborador_Associado'", "idUsuario", "sDscColaborador", false, "Selecione a Responsável", "0");

        }
        #endregion


        void Popular_Aba_Historico(DataSet ds)
        {
            gv_Historico.DataSource = ds.Tables[1];
            gv_Historico.DataBind();
            aba_Historico.Visible = true;
        }

        //void Popular_Aba_ProdutosFinanceiros(DataSet ds)
        //{
        //    gv_ProdutosFinanceiros.DataSource = ds.Tables[1];
        //    gv_ProdutosFinanceiros.DataBind();
        //    aba_ProdutosFinanceiros.Visible = true;
        //}

        #region | ProdutosFinanceiros

        bool Salvar_ProdutosFinanceiros(string idConta)
        {

            bool bRetorno = false;

            try
            {
                if (idConta != "0")
                {
                    Dictionary<String, String> vParametroItensProdutosFinanceiros = new Dictionary<string, string>()
                    {
                        { "@sFuncao","EXCLUIR_ProdutosFinanceiros" },
                        { "@idConta",idConta },
                    };
                    BD.ExecutarDataSet(sProcedure, vParametroItensProdutosFinanceiros);

                    //Incluir
                    vParametroItensProdutosFinanceiros["@sFuncao"] = "INSERIR_ProdutosFinanceiros";
                    foreach (cls_ProdutosFinanceiros ProdutosFinanceiros_Linha in bs_ProdutosFinanceiros)
                    {
                        Double nValorProdutosFinanceiros = 0;
                        nValorProdutosFinanceiros = Convert.ToDouble(ProdutosFinanceiros_Linha.nValorProdutosFinanceiros);
                        vParametroItensProdutosFinanceiros["idTipoConta"] = ProdutosFinanceiros_Linha.idTipoConta.ToString();
                        vParametroItensProdutosFinanceiros["nLimiteConta"] = ProdutosFinanceiros_Linha.nLimiteConta.ToString().Replace(",", ".");
                        vParametroItensProdutosFinanceiros["dtInicio"] = ProdutosFinanceiros_Linha.dtInicio;
                        vParametroItensProdutosFinanceiros["dtVencimento"] = ProdutosFinanceiros_Linha.dtVencimento;
                        vParametroItensProdutosFinanceiros["sGarantias"] = ProdutosFinanceiros_Linha.sGarantias;
                        vParametroItensProdutosFinanceiros["nPorcentagemTaxaConta"] = ProdutosFinanceiros_Linha.nPorcentagemTaxaConta.ToString().Replace(",", ".");
                        vParametroItensProdutosFinanceiros["nValorProdutosFinanceiros"] = ProdutosFinanceiros_Linha.nValorProdutosFinanceiros.ToString().Replace(",", ".");


                        BD.ExecutarDataSet(sProcedure, vParametroItensProdutosFinanceiros);
                    }

                    bRetorno = true;
                }


            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }


            return bRetorno;

        }

        protected void cmdProdutosFinanceiros_Incluir_Click(object sender, EventArgs e)
        {
            string sMensagem = "";
            if (ValidarDados_ProdutosFinanceiros(ref sMensagem))
            {
                FrameWork.cls_ProdutosFinanceiros objItem = new FrameWork.cls_ProdutosFinanceiros();
                string[] vidConta = hddidConta.Value.Split(',');
                string idConta = vidConta[0].ToString();
                objItem.idConta = Convert.ToInt32(idConta);

                objItem.idTipoConta = Convert.ToInt32(ddlProdutosFinanceiros_idTipoConta.SelectedValue);
                objItem.sDscTipoConta = ddlProdutosFinanceiros_idTipoConta.SelectedItem.ToString();
                objItem.nLimiteConta = ConverterStringDecimal(txtProdutosFinanceiros_nLimiteConta.Text);
                objItem.dtInicio = txtProdutosFinanceiros_dtInicio.Text;
                objItem.dtVencimento = txtProdutosFinanceiros_dtVencimento.Text;
                objItem.sGarantias = txtProdutosFinanceiros_sGarantias.Text;
                objItem.nPorcentagemTaxaConta = ConverterStringDecimal(txtProdutosFinanceiros_nPorcentagemTaxaConta.Text);
                // objItem.nValorProdutosFinanceiros = ConverterStringDecimal(txtProdutosFinanceiros_nValorProdutosFinanceiros.Text);


                bs_ProdutosFinanceiros.Add(objItem);
                dtgProdutosFinanceiros_DataBind();
                LimpaCampos_ProdutosFinanceiros();
            }
            else
            {
                MensagemProdutosFinanceiros.MostraMensagem_Erro(sMensagem, false);
            }

            RegistraScript("$('[id$=txtProdutosFinanceiros_nLimiteConta]').focus();");

        }


        protected void dtgProdutosFinanceiros_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            bs_ProdutosFinanceiros.RemoveAt(index);
            dtgProdutosFinanceiros_DataBind();
        }


        private bool ValidarDados_ProdutosFinanceiros(ref string sMensagemErro)
        {

            if (ddlProdutosFinanceiros_idTipoConta.SelectedValue == "0")
            {
                sMensagemErro += "Selecione um tipo de Conta";
            }

            //if (!Validacoes.ValidarMoeda(txtProdutosFinanceiros_nValorProdutosFinanceiros))
            //{
            //    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o Valor";
            //}


            return sMensagemErro != "" ? false : true;
        }
        void LimpaCampos_ProdutosFinanceiros()
        {

            ddlProdutosFinanceiros_idTipoConta.SelectedValue = "0";
            txtProdutosFinanceiros_nLimiteConta.Text = "";
            txtProdutosFinanceiros_dtInicio.Text = "";
            txtProdutosFinanceiros_dtVencimento.Text = "";
            txtProdutosFinanceiros_sGarantias.Text = "";
            txtProdutosFinanceiros_nPorcentagemTaxaConta.Text = "";
            // txtProdutosFinanceiros_nValorProdutosFinanceiros.Text = "";

        }

        void Popular_dtgProdutosFinanceiros(DataSet dsPesquisa)
        {
            foreach (DataRow row in dsPesquisa.Tables[2].Rows)
            {
                FrameWork.cls_ProdutosFinanceiros objItem = new FrameWork.cls_ProdutosFinanceiros();
                objItem.idConta = Convert.ToInt32(row["idConta"].ToString());

                objItem.idProdutosFinanceiros = Convert.ToInt32(row["idProdutosFinanceiros"].ToString());
                objItem.idTipoConta = Convert.ToInt32(row["idTipoConta"].ToString());
                objItem.sDscTipoConta = row["sDscTipoConta"].ToString();
                objItem.nLimiteConta = ConverterStringDecimal(row["nLimiteConta"].ToString());
                objItem.dtInicio = row["dtInicio"].ToString();
                objItem.dtVencimento = row["dtVencimento"].ToString();
                objItem.sGarantias = row["sGarantias"].ToString();
                objItem.nPorcentagemTaxaConta = ConverterStringDecimal(row["nPorcentagemTaxaConta"].ToString());
                objItem.nValorProdutosFinanceiros = ConverterStringDecimal(row["nValorProdutosFinanceiros"].ToString());



                bs_ProdutosFinanceiros.Add(objItem);
            }
            dtgProdutosFinanceiros_DataBind();
            LimpaCampos_ProdutosFinanceiros();
        }


        void dtgProdutosFinanceiros_DataBind()
        {
            dtgProdutosFinanceiros.DataSource = bs_ProdutosFinanceiros;
            dtgProdutosFinanceiros.DataBind();

        }

        #endregion

        #region | Contatos

        bool Salvar_Contatos(string idConta)
        {

            bool bRetorno = false;

            try
            {
                if (idConta != "0")
                {
                    Dictionary<String, String> vParametroItensContatos = new Dictionary<string, string>()
                    {
                        { "@sFuncao","EXCLUIR_CONTATOS" },
                        { "@idConta",idConta },
                    };
                    BD.ExecutarDataSet(sProcedure, vParametroItensContatos);

                    //Incluir
                    vParametroItensContatos["@sFuncao"] = "INSERIR_CONTATOS";
                    foreach (cls_Contatos Contatos_Linha in bs_Contatos)
                    {

                        vParametroItensContatos["idContatos"] = Contatos_Linha.idContatos.ToString();
                        vParametroItensContatos["sDscContato"] = Contatos_Linha.sDscContato;
                        vParametroItensContatos["sTelefoneContato"] = Contatos_Linha.sTelefoneContato;
                        vParametroItensContatos["sEmailContato"] = Contatos_Linha.sEmailContato;



                        BD.ExecutarDataSet(sProcedure, vParametroItensContatos);
                    }

                    bRetorno = true;
                }


            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }


            return bRetorno;

        }

        protected void cmdContatos_Incluir_Click(object sender, EventArgs e)
        {
            string sMensagem = "";
            if (ValidarDados_Contatos(ref sMensagem))
            {
                FrameWork.cls_Contatos objItem = new FrameWork.cls_Contatos();
                string[] vidConta = hddidConta.Value.Split(',');
                string idConta = vidConta[0].ToString();
                objItem.idConta = Convert.ToInt32(idConta);

                objItem.sDscContato = txtContatos_sDscContato.Text;
                objItem.sTelefoneContato = txtContatos_sTelefoneContato.Text;
                objItem.sEmailContato = txtContatos_sEmailContato.Text;


                bs_Contatos.Add(objItem);
                dtgContatos_DataBind();
                LimpaCampos_Contatos();
            }
            else
            {
                MensagemContatos.MostraMensagem_Erro(sMensagem, false);
            }

            RegistraScript("$('[id$=txtContatos_sDscContato]').focus();");

        }


        protected void dtgContatos_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            bs_Contatos.RemoveAt(index);
            dtgContatos_DataBind();
        }


        private bool ValidarDados_Contatos(ref string sMensagemErro)
        {


            if (Validacoes.ValidarTexto(txtContatos_sDscContato))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe a descrição do Contato";
            }

            if (!TT.FrameWork.Validacoes.ValidarEmail(txtContatos_sEmailContato.Text))
            {
                if (sMensagemErro != "")
                {
                    sMensagemErro = sMensagemErro + "</br>";
                }
                sMensagemErro += "e-mail inválido!";

            }

            if (!TT.FrameWork.Validacoes.ValidarTelefone(txtContatos_sTelefoneContato.Text))
            {
                if (sMensagemErro != "")
                {
                    sMensagemErro = sMensagemErro + "</br>";
                }
                sMensagemErro += "Telefone inválido!";

            }

            return sMensagemErro != "" ? false : true;
        }

        void LimpaCampos_Contatos()
        {
            //29/05/2023 - Antonio Lemos


            txtContatos_sDscContato.Text = "";
            txtContatos_sTelefoneContato.Text = "";
            txtContatos_sEmailContato.Text = "";

        }

        void Popular_dtgContatos(DataSet dsPesquisa)
        {
            foreach (DataRow row in dsPesquisa.Tables[3].Rows)
            {
                FrameWork.cls_Contatos objItem = new FrameWork.cls_Contatos();
                objItem.idConta = Convert.ToInt32(row["idConta"].ToString());

                objItem.idContatos = Convert.ToInt32(row["idContatos"].ToString());
                objItem.sDscContato = row["sDscContato"].ToString();
                objItem.sTelefoneContato = row["sTelefoneContato"].ToString();
                objItem.sEmailContato = row["sEmailContato"].ToString();


                bs_Contatos.Add(objItem);
            }
            dtgContatos_DataBind();
            LimpaCampos_Contatos();
        }


        void dtgContatos_DataBind()
        {
            dtgContatos.DataSource = bs_Contatos;
            dtgContatos.DataBind();
        }

        #endregion


        decimal ConverterStringDecimal(string dado)
        {
            decimal converter = 0;
            if (decimal.TryParse(dado, out converter))
            {
                converter = Convert.ToDecimal(dado);
                return converter;
            }
            return decimal.Zero;
        }

        #region | Cartão



        void Popular_dtgCartaoCredito(DataSet dsPesquisa)
        {

            foreach (DataRow row in dsPesquisa.Tables[4].Rows)
            {

                FrameWork.cls_CartaoCredito objItem = new FrameWork.cls_CartaoCredito();

                objItem.idLinha = bs_CartaoCredito.Count() + 1;
                objItem.sFuncao = "SEM ALTERAÇÃO";

                objItem.idConta = Convert.ToInt32(row["idConta"].ToString());
                objItem.idCartao = Convert.ToInt32(row["idCartao"].ToString());

                objItem.dtVenciCartao = row["dtVenciCartao"].ToString();
                objItem.nNumCartao = row["nNumCartao"].ToString();
                objItem.idBandeira = Convert.ToInt32(row["idBandeira"].ToString());
                objItem.sTitularCartao = row["sTitularCartao"].ToString();
                objItem.nLimiteCartao = ConverterStringDecimal(row["nLimiteCartao"].ToString());

                objItem.dtVencimento = row["dtVencimento"].ToString();
                objItem.dtCorteFatura = row["dtCorteFatura"].ToString();
                objItem.nProvisaoGastos = ConverterStringDecimal(row["nProvisaoGastos"].ToString());
                objItem.idUsuario = Convert.ToInt32(row["idUsuario"].ToString());
                objItem.sStatus = row["sStatus"].ToString();

                objItem.sDscBandeira = row["sDscBandeira"].ToString();
                objItem.sDscUsuario = row["sDscUsuario"].ToString();
                objItem.sStatus_Completo = row["sStatus_Completo"].ToString();

                objItem.idArquivo = Convert.ToInt32(row["idArquivo"].ToString());
                objItem.sNomeArquivo = row["sNomeArquivo"].ToString();
                objItem.sLancamento = row["sLancamento"].ToString();


                //var dt = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtVenciCartao").ToString());
                //txtdtVenciCartao.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');

                //var dt2 = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtVencimento").ToString());
                //txtdtVencimento.Text = dt2.ToString(@"yyyy/MM/dd").Replace('/', '-');

                //var dt3 = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtCorteFatura").ToString());
                //txtdtCorteFatura.Text = dt3.ToString(@"yyyy/MM/dd").Replace('/', '-');

                bs_CartaoCredito.Add(objItem);
            }
            dtgCartaoCredito_DataBind();
            LimpaCampos_CartaoCredito();
        }

        protected void cmdIncluirCartoes_Click(object sender, EventArgs e)
        {
            LimpaCampos_CartaoCredito();
            hddCartaoCredito_idLinha.Value = "0";
            Cartoes_lblTitulo.Text = "Novo Cartão";
            Modal_Abrir("Cartoes");
        }

        bool Salvar_CartaoCredito(string idConta)
        {
            bool bRetorno = false;
            try
            {
                foreach (var CartaoCredito_Linha in bs_CartaoCredito)
                {
                    int idArquivo = CartaoCredito_Linha.idArquivo;
                    if (CartaoCredito_Linha.sNomeArquivo != "" && CartaoCredito_Linha.idArquivo == 0)
                    {
                        TT_Flow.FrameWork.cls_Arquivos Arquivo = new FrameWork.cls_Arquivos();

                        Arquivo.idTipoArquivo = 9999;
                        Arquivo.idObjeto = CartaoCredito_Linha.idCartao;
                        Arquivo.sNomeArquivo = CartaoCredito_Linha.sNomeArquivo;
                        Arquivo.sDscArquivo = CartaoCredito_Linha.sObservacaoArquivo;
                        Arquivo.sObservacao = "";
                        Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                        Arquivo.vbArquivo = CartaoCredito_Linha.objArquivo;
                        Arquivo.dtExpiracaoDoc = "";

                        CartaoCredito_Linha.idArquivo = Convert.ToInt32(BD.Retorno.DATASET(Arquivo.EnviarArquivo(Arquivo), "idArquivo"));

                        if (CartaoCredito_Linha.sFuncao == "SEM ALTERAÇÃO")
                        {
                            CartaoCredito_Linha.sFuncao = "SALVAR_CARTOES_CREDITO";
                        }
                    }

                    if (CartaoCredito_Linha.sFuncao != "SEM ALTERAÇÃO")
                    {
                        Dictionary<String, String> vParametroItensCartaoCredito = new Dictionary<string, string>();

                        vParametroItensCartaoCredito["@sFuncao"] = CartaoCredito_Linha.sFuncao;
                        vParametroItensCartaoCredito["@idRegistro"] = CartaoCredito_Linha.idCartao.ToString();

                        vParametroItensCartaoCredito["@idConta"] = CartaoCredito_Linha.idConta.ToString();
                        Decimal nLimiteCartao = 0;
                        nLimiteCartao = Convert.ToDecimal(CartaoCredito_Linha.nLimiteCartao);
                        vParametroItensCartaoCredito["@nNumCartao"] = CartaoCredito_Linha.nNumCartao.ToString();
                        vParametroItensCartaoCredito["@dtVenciCartao"] = CartaoCredito_Linha.dtVenciCartao.ToString();
                        vParametroItensCartaoCredito["@idBandeira"] = CartaoCredito_Linha.idBandeira.ToString();
                        vParametroItensCartaoCredito["@sTitularCartao"] = CartaoCredito_Linha.sTitularCartao;
                        vParametroItensCartaoCredito["@nLimiteCartao"] = CartaoCredito_Linha.nLimiteCartao.ToString().Replace(",", ".");
                        vParametroItensCartaoCredito["@dtVencimento"] = CartaoCredito_Linha.dtVencimento.ToString();
                        vParametroItensCartaoCredito["@dtCorteFatura"] = CartaoCredito_Linha.dtCorteFatura.ToString();
                        Decimal nProvisaoGastos = 0;
                        nProvisaoGastos = Convert.ToDecimal(CartaoCredito_Linha.nProvisaoGastos);
                        vParametroItensCartaoCredito["@nProvisaoGastos"] = CartaoCredito_Linha.nProvisaoGastos.ToString().Replace(",", ".");

                        vParametroItensCartaoCredito["@idUsuario"] = CartaoCredito_Linha.idUsuario.ToString();
                        vParametroItensCartaoCredito["@sStatus"] = CartaoCredito_Linha.sStatus.ToString();


                        vParametroItensCartaoCredito["@idArquivo"] = CartaoCredito_Linha.idArquivo.ToString();

                        BD.ExecutarDataSet(sProcedure, vParametroItensCartaoCredito);
                    }

                }

                bRetorno = true;

            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Bloco Cartões - Erro: " + ex.Message);
            }


            return bRetorno;

        }

        protected void cmdCartao_Salvar_Click(object sender, EventArgs e)
        {
            string sMensagem = "";
            int idLinha = Convert.ToInt32(hddCartaoCredito_idLinha.Value);

            if (ValidarDados_CartaoCredito(ref sMensagem))
            {

                int index = 0;
                FrameWork.cls_CartaoCredito objItem = new FrameWork.cls_CartaoCredito();

                if (idLinha != 0)
                {
                    index = bs_CartaoCredito.FindIndex(x => x.idLinha.Equals(idLinha));
                    objItem = bs_CartaoCredito[index];
                }

                if (idLinha == 0)
                {
                    objItem.sNomeArquivo = "";
                    bs_CartaoCredito.Add(objItem);
                    index = bs_CartaoCredito.Count() - 1;
                }

                objItem.sFuncao = "SALVAR_CARTOES_CREDITO";
                objItem.idLinha = bs_CartaoCredito.Count();
                objItem.idConta = Convert.ToInt32(hddidConta.Value.Split(',')[0].ToString());

                objItem.nNumCartao = txtnNumCartao.Text;

                objItem.idBandeira = Convert.ToInt32(ddlidBandeira.SelectedValue.ToString());
                objItem.sTitularCartao = txtsTitularCartao.Text;
                objItem.nLimiteCartao = ConverterStringDecimal(txtnLimiteCartao.Text);

                objItem.dtVencimento = FormatarData(txtdtVencimento.Text);
                objItem.dtCorteFatura = FormatarData(txtdtCorteFatura.Text);
                objItem.dtVenciCartao = FormatarData(txtdtVenciCartao.Text);

                objItem.nProvisaoGastos = ConverterStringDecimal(txtnProvisaoGastos.Text);

                objItem.idUsuario = Convert.ToInt32(ddlidUsuario.SelectedValue.ToString());
                objItem.sStatus = ddlsStatus.SelectedItem.ToString();


                objItem.sDscBandeira = ddlidBandeira.SelectedItem.ToString();
                objItem.sStatus_Completo = ddlsStatus.SelectedItem.ToString();
                objItem.sDscUsuario = ddlidUsuario.SelectedItem.ToString();
                objItem.sLancamento = "N";



                bs_CartaoCredito[index] = objItem;

                dtgCartaoCredito_DataBind();
                LimpaCampos_CartaoCredito();
                Modal_Fechar("Cartoes");

                Salvar_ContasBancarias();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=cmdIncluirCartoes]').focus();", true);
            }
        }

        string FormatarData(string dateInput)
        {
            if (String.IsNullOrWhiteSpace(dateInput))
            {
                return "";
            }

            string[] parts = dateInput.Split('-');
            if (parts.Length != 3)
            {
                return "Formato inválido";
            }

            return parts[2] + "/" + parts[1] + "/" + parts[0];
        }

        void LimpaCampos_CartaoCredito()
        {

            txtnNumCartao.Text = "";
            txtdtVenciCartao.Text = "";
            txtsTitularCartao.Text = "";
            txtnLimiteCartao.Text = "";
            ddlidBandeira.SelectedValue = "0";

            txtdtVencimento.Text = "";
            txtdtCorteFatura.Text = "";
            txtnProvisaoGastos.Text = "";
            ddlidUsuario.SelectedValue = "0";            
        }

        private bool ValidarDados_CartaoCredito(ref string sMensagem)
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (txtnNumCartao.Text.Length < 6)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Número Cartão Inválido";
            }

            if (txtsTitularCartao.Text.Length < 6)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Nome do Titular Inválido";
            }

            if (ddlidBandeira.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Bandeira";
            }

            if (!Validacoes.ValidarMoeda(txtnLimiteCartao))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o Limite do Cartão";
            }

            if (!Validacoes.ValidarData(txtdtVenciCartao))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe a Validade";
            }

            if (!Validacoes.ValidarData(txtdtCorteFatura))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe a Data de Corte da Fatura";
            }

            if (!Validacoes.ValidarData(txtdtVencimento))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe a Data de Vencimento";
            }

            if (!Validacoes.ValidarMoeda(txtnProvisaoGastos))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o Provisão de Gastos";
            }

            if (ddlidUsuario.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Responsável";
            }            

            if (sMensagemErro != "")
            {
                bRetorno = false;

                Cartoess_MensagemPagina.MostraMensagem_Erro(sMensagemErro);
                Modal_Abrir("Cartoes");
            }


            return bRetorno;
        }

        protected void dtgCartaoCredito_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(dtgCartaoCredito.Rows[e.RowIndex].Cells[0].Text);
            bs_CartaoCredito[bs_CartaoCredito.FindIndex(x => x.idLinha.Equals(idLinha))].sFuncao = "EXCLUIR_CARTOES_CREDITO";
            dtgCartaoCredito_DataBind();
        }

        protected void dtgCartaoCredito_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int nColunaBotoes = e.Row.Cells.Count - 1;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string sNomeArquivo = DataBinder.Eval(e.Row.DataItem, "sNomeArquivo").ToString();

                foreach (LinkButton lnk in e.Row.Cells[nColunaBotoes].Controls.OfType<LinkButton>())
                {
                    if ((lnk.ID == "lnkCartoes_Download" && sNomeArquivo == "") || (lnk.ID == "lnkCartoes_UpLoad" && sNomeArquivo != ""))
                    {
                        lnk.Visible = false;
                    }
                }

                var dataItem = e.Row.DataItem as cls_CartaoCredito;

                if (dataItem != null)
                {
                    string idLinha = dataItem.idLinha.ToString();
                    string sLancamento = dataItem.sLancamento.ToString();
                    
                    LinkButton lnkExcluir = e.Row.FindControl("lnkCartoes_Excluir") as LinkButton;                                      
                    LinkButton lnkLancamento = e.Row.FindControl("lnkLancamento") as LinkButton;                                      

                    if (lnkLancamento != null || lnkExcluir != null)
                    {
                        if (sLancamento == "S")
                        {
                            lnkExcluir.Visible = false;                            
                            lnkLancamento.CommandArgument = idLinha;
                        }                        
                    }

                }

            }

            GRID.EsconderColunas(e, 0);
        }


        protected void dtgCartaoCredito_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idLinha = int.Parse(e.CommandArgument.ToString());
            hddsBloco.Value = "Cartoes";
            hddCartaoCredito_idLinha.Value = idLinha.ToString();

            if (e.CommandName == "Upload_Arquivo")
            {
                AbrirModal_EnvioArquivo(eBloco.CartaoCredito, idLinha, bs_CartaoCredito[bs_CartaoCredito.FindIndex(x => x.idLinha.Equals(idLinha))].nNumCartao);
            }
            else if (e.CommandName == "Download_Arquivo")
            {
                Efetuar_Download_Arquivo(eBloco.CartaoCredito, idLinha);
            }
            else if (e.CommandName == "Editar")
            {
                var Cartao = bs_CartaoCredito[bs_CartaoCredito.FindIndex(x => x.idLinha.Equals(idLinha))];

                txtnNumCartao.Text = Cartao.nNumCartao;
                txtsTitularCartao.Text = Cartao.sTitularCartao;
                txtnLimiteCartao.Text = Cartao.nLimiteCartao.ToString();
                txtnProvisaoGastos.Text = Cartao.nProvisaoGastos.ToString();


                string dataEmString = Cartao.dtVenciCartao;
                DateTime dataConvertida;

                if (DateTime.TryParseExact(dataEmString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataConvertida))
                {

                    txtdtVenciCartao.Text = dataConvertida.ToString("yyyy-MM-dd");
                }
                else
                {
                    txtdtVenciCartao.Text = "";
                }

                // Repita para os outros campos
                if (DateTime.TryParseExact(Cartao.dtVencimento, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataConvertida))
                {
                    txtdtVencimento.Text = dataConvertida.ToString("yyyy-MM-dd");
                }
                else
                {
                    txtdtVencimento.Text = "";
                }

                if (DateTime.TryParseExact(Cartao.dtCorteFatura, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataConvertida))
                {
                    txtdtCorteFatura.Text = dataConvertida.ToString("yyyy-MM-dd");
                }
                else
                {
                    txtdtCorteFatura.Text = "";
                }

                ddlidUsuario.SelectedValue = Cartao.idUsuario.ToString();
                ddlsStatus.SelectedValue = Cartao.sStatus.ToString();
                ddlidBandeira.SelectedValue = Cartao.idBandeira.ToString();
                Cartoes_lblTitulo.Text = "Editar Cartões";
                Modal_Abrir("Cartoes");

            }
            else if (e.CommandName == "Lancamento")
            {
                dtgCartaoCredito_DataBind();
                var cartao = bs_CartaoCredito.FirstOrDefault(x => x.idLinha.Equals(idLinha));
                string url = "/App/Paginas/Adm/Financeiro/Cartoes.aspx?grid=1&cartao=" + cartao.idCartao;
                string script = $"window.open('{url}', '_blank');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "openNewTab", script, true);
            }
        }


        void dtgCartaoCredito_DataBind()
        {
            try
            {
                dtgCartaoCredito.DataSource = bs_CartaoCredito.Where(c => c.sFuncao.ToString() != "EXCLUIR_CARTOES_CREDITO").OrderBy(x => x.idLinha).ToList();
                dtgCartaoCredito.DataBind();

                if (!Funcoes.ValidaPermissao(Permissao.Financeiro.Cartoes.Consultar, false)) dtgCartaoCredito.Columns[10].Visible = false;

                if (dtgCartaoCredito.Rows.Count > 0)
                {

                    dtgCartaoCredito.HeaderRow.TableSection = TableRowSection.TableHeader;
                    dtgCartaoCredito.UseAccessibleHeader = true;
                    dtgCartaoCredito.FooterRow.TableSection = TableRowSection.TableFooter;
                }


                StringBuilder sbRetorno = new StringBuilder();
                sbRetorno.AppendLine("$(document).ready(function() {");
                sbRetorno.AppendLine("$('#" + dtgCartaoCredito.ClientID + "').DataTable({");
                sbRetorno.AppendLine("paging: false, pageLength: 50,");
                sbRetorno.AppendLine("info: false,");
                sbRetorno.AppendLine("language: {url: 'https://cdn.datatables.net/plug-ins/1.11.5/i18n/pt-BR.json'},");
                sbRetorno.AppendLine("order: [[2, 'asc']],");
                sbRetorno.AppendLine("});});");

                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTablesCartaoCredito", sbRetorno.ToString(), true);

                RegistraScript("");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao Carregar Grid de Cartões: " + ex.Message);
            }

        }

        void Modal_Abrir(string sModal)
        {
            switch (sModal)
            {
                case "Cartoes":
                    Modal_Fechar(sModal);
                    RegistraScript("$('#modal_Cartoes').modal('show');");

                    break;
            }
        }

        protected void Modal_Fechar(object sender, EventArgs e)
        {
            LinkButton cmdClick = sender as LinkButton;

            dtgCartaoCredito_DataBind();

            switch (cmdClick.ID.ToString())
            {
                case "cmdCartoes_Cancelar":
                    Modal_Fechar("Cartoes");
                    break;
            }
        }
        void Modal_Fechar(string sModal)
        {
            switch (sModal)
            {
                case "Cartoes":
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "js_CloseModal", "$('#modal_Cartoes').modal('hide'); $('body').removeClass('modal-open'); $('.modal-backdrop').remove(); ", true);
                    break;

            }
            RegistraScript("");

        }


        #endregion

        #region | Envio Arquivos

        void AbrirModal_EnvioArquivo(eBloco bloco, int idLinha, string sTituloModal)
        {
            lblEnviarArquivos_Titulo.Text = sTituloModal;
            txtEnviarArquivo_sDscArquivo.Text = "";
            hddIdLinha.Value = idLinha.ToString();
            hddsBloco.Value = bloco.ToString();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_AbrirModalUploadArquivos", "$('#UploadArquivos_Modal').modal('show')", true);
        }

        protected void cmdEnviarArquivos_Click(object sender, EventArgs e)
        {
            int index;
            int idLinha = Convert.ToInt32(hddIdLinha.Value);
            eBloco bloco = (eBloco)Enum.Parse(typeof(eBloco), hddsBloco.Value);
            string sJSExecutar = "";

            if (fu_EnviarArquivo.HasFile)
            {
                Byte[] lObjArquivo = null;
                try
                {

                    TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                    lObjArquivo = objArquivo.TransformaArquivoEmArrayBytes(fu_EnviarArquivo.FileName, fu_EnviarArquivo.PostedFile.InputStream);


                    switch (bloco)
                    {
                        case eBloco.CartaoCredito:
                            index = bs_CartaoCredito.FindIndex(x => x.idLinha.Equals(idLinha));
                            bs_CartaoCredito[index].idArquivo = 0;
                            bs_CartaoCredito[index].sNomeArquivo = fu_EnviarArquivo.FileName;
                            bs_CartaoCredito[index].objArquivo = lObjArquivo;
                            bs_CartaoCredito[index].sObservacaoArquivo = txtEnviarArquivo_sDscArquivo.Text;
                            dtgCartaoCredito_DataBind();
                            sJSExecutar = " $('#ProdutosFinanceiros-tab').tab('show'); window.location.hash = '#ctl00_cphCorpo_Div_Cartoes';";
                            break;
                    }
                }
                catch
                {
                    return;
                }
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ModalUpload_Fecha", "$('body').removeClass('modal-open').find('.modal-backdrop').remove();" + sJSExecutar, true);
        }

        void Efetuar_Download_Arquivo(eBloco bloco, int idLinha)
        {
            int index;
            int idArquivo = 0;
            string sNomeArquivo = "";
            byte[] bObjArquivo = null;
            string urlAtualPagina = Request.UrlReferrer.ToString().Replace(Request.RawUrl, "/Download/");


            switch (bloco)
            {
                case eBloco.CartaoCredito:
                    index = bs_CartaoCredito.FindIndex(x => x.idLinha.Equals(idLinha));
                    sNomeArquivo = bs_CartaoCredito[index].sNomeArquivo;
                    bObjArquivo = bs_CartaoCredito[index].objArquivo;
                    idArquivo = bs_CartaoCredito[index].idArquivo;
                    break;

            }

            if (bObjArquivo == null)
            {
                Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();
                DataTable dtArquivo;
                vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao",                    "CONSULTAR_DETALHE" },
                        {"@idArquivo",                  idArquivo.ToString()}
                    };

                dtArquivo = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);
                foreach (DataRow item in dtArquivo.Rows)
                {
                    sNomeArquivo = item["sNomeArquivo"].ToString();
                    bObjArquivo = (byte[])item["vbArquivo"];
                }
            }



            //Gera o Arquivo
            TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
            FileStream lObjFile = objArquivo.TransformarArrayBytesEmArquivo(bObjArquivo, Server.MapPath("~/Download/" + sNomeArquivo));
            lObjFile.Close();
            lObjFile.Dispose();

            //Efetua o Download
            StringBuilder strDownload = new StringBuilder();
            strDownload.AppendLine("var link = document.createElement('a');");
            strDownload.AppendLine("link.download = '" + sNomeArquivo + "';");
            strDownload.AppendLine("link.href = '" + string.Concat(urlAtualPagina, sNomeArquivo) + "';");
            strDownload.AppendLine("link.click();");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Download_dArquivos", strDownload.ToString(), true);

        }

        private enum eBloco
        {
            CartaoCredito = 1

        }







        #endregion
                
    }
}