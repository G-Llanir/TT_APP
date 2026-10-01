using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using System.Data.SqlClient;
using TT_Flow.App.Controles;
using TT_Flow.FrameWork;
using System.Data.Common.CommandTrees.ExpressionBuilder;
using System.Text;
using TT_Flow.App.Paginas.Adm.Financeiro;
using static TT.FrameWork.BD;

namespace TT_Flow.App.Paginas.Manutencao
{
    public partial class Parceiros_Detalhe : Page
    {
        #region | Construtores

        string sTituloPagina = "Parceiro";
        string sProcedure = "sp_Manipula_tbl_Flow_Clientes";

        public enum ItensMultiview
        {
            view_AdicionarFamilia,
            view_AdicionarCondicaoPagamento,
        }

        public List<cls_Clientes_Contatos> bs_Clientes_Contatos
        {
            get
            {
                if (ViewState["bs_Clientes_Contatos"] == null)
                {
                    ViewState["bs_Clientes_Contatos"] = new List<cls_Clientes_Contatos>();
                }

                return (List<cls_Clientes_Contatos>)ViewState["bs_Clientes_Contatos"];
            }

            set { ViewState["bs_Clientes_Contatos"] = value; }

        }

        public List<cls_Clientes_Bancario> bs_Clientes_Bancario
        {
            get
            {
                if (ViewState["bs_Clientes_Bancario"] == null)
                {
                    ViewState["bs_Clientes_Bancario"] = new List<cls_Clientes_Bancario>();
                }

                return (List<cls_Clientes_Bancario>)ViewState["bs_Clientes_Bancario"];
            }

            set { ViewState["bs_Clientes_Bancario"] = value; }

        }

        public List<cls_Clientes_Enderecos> bs_Clientes_Enderecos
        {
            get
            {
                if (ViewState["bs_Clientes_Enderecos"] == null)
                {
                    ViewState["bs_Clientes_Enderecos"] = new List<cls_Clientes_Enderecos>();
                }

                return (List<cls_Clientes_Enderecos>)ViewState["bs_Clientes_Enderecos"];
            }

            set { ViewState["bs_Clientes_Enderecos"] = value; }

        }

        public List<cls_CondicoesPagamento> BS_CONDICAO_PAGAMENTO
        {
            get
            {
                if (ViewState["BS_CONDICAO_PAGAMENTO"] == null)
                {
                    ViewState["BS_CONDICAO_PAGAMENTO"] = new List<cls_CondicoesPagamento>();
                }

                return (List<cls_CondicoesPagamento>)ViewState["BS_CONDICAO_PAGAMENTO"];
            }

            set { ViewState["BS_CONDICAO_PAGAMENTO"] = value; }

        }

        public List<cls_WMS_Familias> BS_WMS_FAMILIA
        {
            get
            {
                if (ViewState["BS_WMS_FAMILIA"] == null)
                {
                    ViewState["BS_WMS_FAMILIA"] = new List<cls_WMS_Familias>();
                }

                return (List<cls_WMS_Familias>)ViewState["BS_WMS_FAMILIA"];
            }

            set { ViewState["BS_WMS_FAMILIA"] = value; }

        }

        public List<cls_CondicoesPagamentoCompra> BS_PAGAMENTOCOMPRA
        {
            get
            {
                if (ViewState["BS_CONDICAO_PAGAMENTOCOMPRA"] == null)
                {
                    ViewState["BS_CONDICAO_PAGAMENTOCOMPRA"] = new List<cls_CondicoesPagamentoCompra>();
                }

                return (List<cls_CondicoesPagamentoCompra>)ViewState["BS_CONDICAO_PAGAMENTOCOMPRA"];
            }

            set { ViewState["BS_CONDICAO_PAGAMENTOCOMPRA"] = value; }

        }

        public List<Cls_STSO> Base_STSO
        {
            get
            {
                if (ViewState["Base_STSO"] == null)
                {
                    ViewState["Base_STSO"] = new List<Cls_STSO>();
                }
                return (List<Cls_STSO>)ViewState["Base_STSO"];
            }

            set
            {
                ViewState["Base_STSO"] = value;
            }

        }

        #endregion

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            divTabelaVinculada.Visible = false;
            LinkButton3.Visible = false;

            if (txtTabelaVinculada.Text.Length > 0)
                divTabelaVinculada.Visible = true;

            div_InputContato.Visible = false;
            divEndereco_Input.Visible = false;
            div_imputBancario.Visible = false;

            if (!IsPostBack)
            {
                PopularCombos();

                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Parceiros.Consultar, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Parceiros.Incluir, true);
                    Pesquisar("0");
                }

                if (FUNCOES.ValidaPermissao(Permissao.Parceiros.BloquearEdicao))
                    hddsPermissaoCadeado.Value = "1";
            }

            SwitchAtivo_VendaIndividual.MudarTitulo("Liberação Produtos Venda Individual");
            SwitchAtivo_Drawback.MudarTitulo("Possui DrawBack/Zona Franca?");
            SwitchAtivo_Contribuinte.MudarTitulo("É Contribuinte do ICMS?");

            var requestTarget = Request["__EVENTTARGET"];

            if (requestTarget == "funcao_EXCLUIR")
                ModalExcluir(hdd_modalExcluir.Value);
            else if (requestTarget == "funcao_NaoContribuinte")
                Switch_NaoContribuinte();

            if (!FUNCOES.ValidaPermissao(Permissao.Parceiros.AlterarFormaPagamento))
            {
                div5.Visible = false;
                div7.Visible = false;
            }

            //SwitchAtivo_Contribuinte.sPostBack_Switch = "funcao_NaoContribuinte";
            RegistraScript("");
        }       

        protected void Pesquisar(string idPesquisa)
        {
            imgParceiroPrincipal.Visible = false;
            DIV_CNPJ.Visible = false;
            DIV_CPF.Visible = false;
            DIV_FEIN.Visible = false;
            DIV_VAT.Visible = false;
            DIV_RNE.Visible = false;
            DIV_IE.Visible = false;
            DIV_IM.Visible = false;
            DIV_Vendedor.Visible = false;
            DIV_Comprador.Visible = false;

            try
            {
                LimpaCampos();

                if (idPesquisa != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idCliente", idPesquisa }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        hddidParceiro.Value = RETORNO.DATASET(dsPesquisa, 0, "idCliente");
                        txtidCliente.Text = RETORNO.DATASET(dsPesquisa, 0, "idCliente");
                        txtsRazaoSocial.Text = RETORNO.DATASET(dsPesquisa, 0, "sRazaoSocial");
                        txtsNomeFantasia.Text = RETORNO.DATASET(dsPesquisa, 0, "sNomeFantasia");
                        ddlsTipo.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sTipo");
                        cblsParceiros.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sTipoCliente");

                        if (cblsParceiros.SelectedValue == "N" && ddlsTipo.SelectedValue == "J") txtsCNPJ.Text = RETORNO.DATASET(dsPesquisa, 0, "sCPF_CNPJ");
                        if (ddlsTipo.SelectedValue == "F" && cblsParceiros.SelectedValue == "N") txtsCPF.Text = RETORNO.DATASET(dsPesquisa, 0, "sCPF_CNPJ");
                        if (cblsParceiros.SelectedValue == "E" && ddlsTipo.SelectedValue == "J") txtsVAT.Text = RETORNO.DATASET(dsPesquisa, 0, "sCPF_CNPJ");
                        if (cblsParceiros.SelectedValue == "U" && ddlsTipo.SelectedValue == "J") txtsFEIN.Text = RETORNO.DATASET(dsPesquisa, 0, "sCPF_CNPJ");
                        if (ddlsTipo.SelectedValue == "F" && cblsParceiros.SelectedValue == "E") txtsRNE.Text = RETORNO.DATASET(dsPesquisa, 0, "sCPF_CNPJ");
                        if (ddlsTipo.SelectedValue == "F" && cblsParceiros.SelectedValue == "U") txtsRNE.Text = RETORNO.DATASET(dsPesquisa, 0, "sCPF_CNPJ");

                        txtsRG_IE.Text = RETORNO.DATASET(dsPesquisa, 0, "sRG_IE");
                        txtsObservacao.Text = RETORNO.DATASET(dsPesquisa, 0, "sObservacao");
                        txtsObservacaoFinanceira.Text = RETORNO.DATASET(dsPesquisa, 0, "sObservacaoFinanceira");
                        txtTabelaVinculada.Text = RETORNO.DATASET(dsPesquisa, 0, "sTabelaVinculada");
                        txtsInscricaoMunicipal.Text = RETORNO.DATASET(dsPesquisa, 0, "sInscricaoMunicipal");

                        if (txtTabelaVinculada.Text.Length > 0)
                            divTabelaVinculada.Visible = true;

                        bool bErroTabelas = false;
                        foreach (string idTabela in RETORNO.DATASET(dsPesquisa, 0, "sidTabela").Split('|'))
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(idTabela))
                                    lstidTabela.Items.FindByValue(idTabela).Selected = true;
                            }
                            catch { bErroTabelas = true; }
                        }

                        if (bErroTabelas)
                            MensagemPagina.MostraMensagem_Erro("É possível que o tempo de Vigência de uma das Tabelas de Preço selecionadas tenha expirado!", false);

                        ddlidVendedor.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idVendedor");                        
                        ddlidComprador.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idComprador");
                        SwitchAtivo_VendaIndividual.Definir(RETORNO.DATASET(dsPesquisa, 0, "sVendaIndividual"), "Liberação Produtos Venda Individual", "N");
                        SwitchAtivo_Drawback.Definir(RETORNO.DATASET(dsPesquisa, 0, "sDrawback"), "Possui DrawBack/Zona Franca?", "N");
                        SwitchAtivo_Contribuinte.Definir(RETORNO.DATASET(dsPesquisa, 0, "sContribuinte"), "É Contribuinte do ICMS?", "N");

                        hddsCadeado.Value = RETORNO.DATASET(dsPesquisa, 0, "sCadeadoFinanceiro");

                        //if (SwitchAtivo_Contribuinte.Recuperar().Equals("N"))
                        //{
                        //    txtsRG_IE.Text = string.Empty;
                        //    txtsRG_IE.ReadOnly = true;
                        //}
                        //else
                        //    txtsRG_IE.ReadOnly = false;

                        ddlidPais.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idPais");

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"));
                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsRazaoSocial.Text);

                        if (FUNCOES.ValidaPermissao(Permissao.Empresas.Aba_STSO))
                            aba_STSO.Visible = true;
                        else
                            aba_STSO.Visible = false;

                        if (FUNCOES.ValidaPermissao(Permissao.Parceiros.VisualizarAbaFinanceiro))
                            aba_Financeiro.Visible = true;
                        else
                            aba_Financeiro.Visible = false;

                        object sender = new object();
                        EventArgs e = new EventArgs();
                        ddlsTipo_SelectedIndexChanged(sender, e);
                        Popular_Aba_Arquivos(hddidParceiro.Value);
                        carregaImgPrincipal(hddidParceiro.Value);
                        Contato_Popular(dsPesquisa);
                        Endereco_Popular(dsPesquisa);
                        Usuarios_Popular(dsPesquisa);
                        PopulaFamilia(hddidParceiro.Value, dsPesquisa);
                        TipoParceiro_Popular(RETORNO.DATASET(dsPesquisa, 0, "sidTipoParceiro"));
                        Bancario_Popular(dsPesquisa);
                        PopulaCondicaoPagamento(hddidParceiro.Value, dsPesquisa);
                        Popular_STSO(dsPesquisa);
                        PopulaAbaFinanceiro();
                        PopularHistorico();

                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Parceiros.Alterar);

                        BaseMultiview_CondicaoPagamento.Visible = false;
                        BaseMultiview_CondicaoCompra.Visible = false;
                        BaseMultiview_Familia.Visible = false;

                        //atualiza os itens selecionados
                        Dictionary<string, string> vParametrosFamiliasSelecionadas = new Dictionary<string, string>
                        {
                            { "@sFuncao", "CONSULTAR_FAMILIAS" },
                            { "@idParceiro", idPesquisa }
                        };
                        BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos_Cotacao", vParametrosFamiliasSelecionadas);

                        cblsParceiros_SelectedIndexChanged(sender, e);

                        sErro = "";
                    }
                    else
                        throw new Exception(sErro);
                }
                else
                {
                    BreadCrumb.TitulodaPagina = "Incluir";
                    lblTituloPagina.Text = string.Format("Novo {0}", sTituloPagina);
                    cmdSalvar.Text = "Incluir";
                    BaseMultiview_CondicaoPagamento.Visible = false;
                    BaseMultiview_CondicaoCompra.Visible = false;
                    BaseMultiview_Familia.Visible = false;
                    aba_STSO.Visible = false;
                }

                ddlsTipo.Focus();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        #endregion

        #region | Popular

        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlidVendedor, "sp_Select 'FLOW_Vendedores'", "idVendedor", "sDscUsuario", false, "Selecione o Vendedor", "0");
            FUNCOES.Popula_Combo(ddlEndereco_sEstado, "sp_Select 'Flow_Estado'", "sEstado", "sEstado", false, "Selecione o Estado", "");
            FUNCOES.Popula_Combo(ddlEndereco_TipoEndereco, "sp_Select 'FLOW_TipoEndereco'", "idTipoEndereco", "sDscTipoEndereco", false, "Selecione o Tipo do Endereço", "0");
            FUNCOES.Popula_Combo(ddlidTipoSituacaoCliente, "sp_Select 'Flow_ClientesSituacaoAdm'", "idTipoSituacaoCliente", "sDscTipoSituacaoCliente", false);
            FUNCOES.Popula_Combo(lstidTabela, "sp_Select 'Flow_Comercial_TabelaPreco_Vendas'", "idTabela", "sDscTabela", false, "Selecione a Tabela", "0");
            FUNCOES.Popula_Combo(ddlidPais, "sp_Select 'tbl_Flow_WMS_Produtos_Origem'", "idPais", "sDscPais", false, "Selecione o País de Origem", "0");
            FUNCOES.Popula_Combo(ddlCondicaoCompra, "sp_Select 'tbl_Flow_CondicaodePagamento'", "idCondicaoPagamento", "sDscCondicaoPagamento", false, "Selecione", "0");
            FUNCOES.Popula_Combo(ddlCondicaoPagamento, "sp_Select 'tbl_Flow_CondicaodePagamento'", "idCondicaoPagamento", "sDscCondicaoPagamento", false, "Selecione", "0");
            FUNCOES.Popula_Combo(ddlFamilias, "sp_Select 'Flow_WMS_Produtos_Familia'", "idFamilia", "sDscFamilia", false, "Selecione", "0");
            FUNCOES.Popula_Combo(ddlidComprador, "sp_Select 'tbl_Compradores'", "idUsuario", "sDscUsuario", false, "Selecione o Comprador", "0");
            FUNCOES.Popula_Combo(ddlidMoeda, $"{sProcedure} 'FLOW-MOEDAS'", "idMoeda", "sDscTipoMoeda", false, "Selecione a Moeda", "0");

            SqlDataReader drTipoParceiro = BD.ExecutarDataReader("sp_Select 'Flow_Tipo_Parceiro'");

            if (drTipoParceiro != null)
            {
                while (drTipoParceiro.Read())
                {
                    bool bAtivo = true;

                    if (RETORNO.nDR(drTipoParceiro, "idRecurso").ToString() != "0")
                        bAtivo = FUNCOES.ValidaPermissao(Convert.ToInt32(RETORNO.nDR(drTipoParceiro, "idRecurso")));

                    cblidTipoParceiro.Items.Add(new ListItem(("  " + drTipoParceiro["sDscTipoParceiro"].ToString()), drTipoParceiro["idTipoParceiro"].ToString(), bAtivo));
                }
            }

            drTipoParceiro.Close();
        }

        //Consulta o banco de dados e faz o "Split" dos valores concatenados da tabela, trazendo os valores individualmente.
        void TipoParceiro_Popular(string sidTipoParceiro)
        {
            string[] vidTipoParceiro = sidTipoParceiro.Split(';');
            bool ClienteChecked = false;
            bool FornecedorChecked = false;

            for (int i = 0; i < vidTipoParceiro.Count(); i++)
            {
                if (!string.IsNullOrEmpty(vidTipoParceiro[i]))
                {
                    for (int contador = 0; contador <= cblidTipoParceiro.Items.Count - 1; contador++)
                    {
                        if (cblidTipoParceiro.Items[contador].Value == vidTipoParceiro[i].ToString())
                            cblidTipoParceiro.Items[contador].Selected = true;
                    }
                }
            }

            foreach (ListItem item in cblidTipoParceiro.Items)
            {
                if (item.Value == "0" && item.Selected)
                    ClienteChecked = true;

                if (item.Value == "1" && item.Selected)
                    FornecedorChecked = true;
            }

            if (ClienteChecked)
                DIV_Vendedor.Visible = true;
            else
                DIV_Vendedor.Visible = false;

            if (FornecedorChecked)
                DIV_Comprador.Visible = true;
            else
                DIV_Comprador.Visible = false;
        }

        void Popular_Aba_Arquivos(string idCliente)
        {
            aba_Arquivos.Visible = false;

            if (FUNCOES.ValidaPermissao(Permissao.Produtos.ConsultarAbaArquivos))
            {
                frmArquivos.Attributes.Add("src", string.Format("../Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}", idCliente, "Parceiros"));
                aba_Arquivos.Visible = true;
            }
        }

        #endregion

        #region | Salvar

        protected void SalvarParceiro()
        {
            if (ValidarDados())
            {
                try
                {
                    string[] vIdCliente = hddidParceiro.Value.Split(',');
                    string idCliente = vIdCliente[0].ToString();
                    string sCPF_CNPJ = "";
                    string sidTabelas = "";

                    foreach (ListItem item in lstidTabela.Items) if (item.Selected) sidTabelas += string.Format("{0}|", item.Value);

                    if (cblsParceiros.SelectedValue == "E" && ddlsTipo.SelectedValue == "J") sCPF_CNPJ = txtsVAT.Text;
                    if (cblsParceiros.SelectedValue == "U" && ddlsTipo.SelectedValue == "J") sCPF_CNPJ = txtsFEIN.Text.Replace("-", "").Replace(".", "").Replace("/", "");
                    if (cblsParceiros.SelectedValue == "N" && ddlsTipo.SelectedValue == "J") sCPF_CNPJ = txtsCNPJ.Text.Replace("-", "").Replace(".", "").Replace("/", "");
                    if (ddlsTipo.SelectedValue == "F" && cblsParceiros.SelectedValue == "E") sCPF_CNPJ = txtsRNE.Text.Replace("-", "").Replace(".", "").Replace("/", "");
                    if (ddlsTipo.SelectedValue == "F" && cblsParceiros.SelectedValue == "U") sCPF_CNPJ = txtsRNE.Text.Replace("-", "").Replace(".", "").Replace("/", "");
                    if (ddlsTipo.SelectedValue == "F" && cblsParceiros.SelectedValue == "N") sCPF_CNPJ = txtsCPF.Text.Replace("-", "").Replace(".", "").Replace("/", "");
                    if (ddlsTipo.SelectedValue == "F") SwitchAtivo_Contribuinte.Definir("N", "É Contribuinte do ICMS?", "N");

                    //if (SwitchAtivo_Contribuinte.Recuperar().Equals("N"))
                    //{
                    //    txtsRG_IE.Text = string.Empty;
                    //    txtsRG_IE.ReadOnly = true;
                    //}
                    //else
                    //    txtsRG_IE.ReadOnly = false;																																		   

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idCliente", idCliente },
                        { "@sNomeFantasia", txtsNomeFantasia.Text },
                        { "@sRazaoSocial", txtsRazaoSocial.Text },
                        { "@sTipo", ddlsTipo.SelectedValue },
                        { "@sCPF_CNPJ", sCPF_CNPJ},
                        { "@sRG_IE", txtsRG_IE.Text },
                        { "@idVendedor", ddlidVendedor.SelectedValue },
                        { "@sidTabela", sidTabelas },
                        { "@sObservacao", txtsObservacao.Text },
                        { "@sObservacaoFinanceira", txtsObservacaoFinanceira.Text },
                        { "@sidTipoParceiro", TipoParceiro_Concatenar()},
                        { "@sSituacao", ComboAtivo.Situacao_Recuperar() },
                        { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() },
                        { "@sTipoCliente", cblsParceiros.SelectedValue },
                        { "@sVendaIndividual", SwitchAtivo_VendaIndividual.Recuperar() },
                        { "@sDrawback", SwitchAtivo_Drawback.Recuperar() },
                        { "@sContribuinte", SwitchAtivo_Contribuinte.Recuperar() },
                        { "@idPais", ddlidPais.SelectedValue },
                        { "@idComprador", ddlidComprador.SelectedValue },
                        { "@sInscricaoMunicipal", txtsInscricaoMunicipal.Text },
                        { "@sCadeadoFinanceiro", hddsCadeado.Value },                   
                        { "@sSuframa", txtsSuframa.Text } // NOVO CAMPO
                    };
                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out string sErro))
                    {
                        idCliente = RETORNO.DATASET(dsSalvar, 0, "idCliente");

                        string idHistoricoGerado = RETORNO.DATASET(dsSalvar, 0, "idHistorico");

                        hddidParceiro.Value = idCliente;

                        if (Contato_Salvar(idCliente, idHistoricoGerado) && Endereco_Salvar(idCliente, idHistoricoGerado) && Bancario_Salvar(idCliente, idHistoricoGerado) && SalvarCondicaoPagamento(idCliente, idHistoricoGerado) && SalvarFamilia(idCliente, idHistoricoGerado) && Salvar_STSO(idCliente, idHistoricoGerado))
                        {
                            SalvaDadosFinanceiros();
                            Pesquisar(idCliente);
                            MensagemPagina.Visible = true;
                            MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                        }
                    }
                    else
                        throw new Exception("BD: " + sErro.ToString());
                }
                catch (Exception ex)
                {
                    if (ex.Message == "BD: Erro ao salvar O CPF/CNPJ já existe cadastrado.")
                    {
                        string Mensagem = "";

                        if (txtsVAT.Visible)
                            Mensagem = "BD: Erro ao salvar o VAT já existe cadastrado.";

                        if (txtsFEIN.Visible)
                            Mensagem = "BD: Erro ao salvar o FEIN já existe cadastrado.";

                        if (txtsCNPJ.Visible)
                            Mensagem = "BD: Erro ao salvar o CNPJ já existe cadastrado.";

                        if (txtsRNE.Visible)
                            Mensagem = "BD: Erro ao salvar o RNE já existe cadastrado.";

                        if (txtsCPF.Visible)
                            Mensagem = "BD: Erro ao salvar o CPF já existe cadastrado.";

                        MensagemPagina.MostraMensagem_Erro(Mensagem);
                    }
                    else
                        MensagemPagina.MostraMensagem_Erro(ex.Message);

                    Contato_EsconderCaixa();
                    Endereco_EsconderCaixa();
                }
            }
        }

        #endregion

        #region | Utils

        string TipoParceiro_Concatenar()
        {
            string sRetornoConcatenado = "";

            for (int contador = 0; contador <= cblidTipoParceiro.Items.Count - 1; contador++)
            {
                if (cblidTipoParceiro.Items[contador].Selected)
                    sRetornoConcatenado += string.Concat(cblidTipoParceiro.Items[contador].Value, ";");
            }
            return sRetornoConcatenado;
        }	  
		  
        void LimpaCampos()
        {
            lblsCPF_CNPJ.Text = "CNPJ";
            lblsNomeFantasia.Text = "Nome Fantasia";
            lblsRazaoSocial.Text = "Razão Social";
            lbl_sRG_IE.Text = "IE";
            txtidCliente.Text = "Novo";
            txtsNomeFantasia.Text = "";
            txtsRazaoSocial.Text = "";
            ddlsTipo.SelectedValue = "";
            txtsCNPJ.Text = "";
            txtsSuframa.Text = "";
            txtsObservacao.Text = "";
            txtsObservacaoFinanceira.Text = "";
            txtsRG_IE.Text = "";
            ddlidVendedor.SelectedValue = "0";
            hddidParceiro.Value = "0";
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
            bs_Clientes_Contatos.Clear();
            bs_Clientes_Enderecos.Clear();
            bs_Clientes_Bancario.Clear();
            BS_PAGAMENTOCOMPRA.Clear();
            BS_CONDICAO_PAGAMENTO.Clear();
            BS_WMS_FAMILIA.Clear();
            ddlidTipoSituacaoCliente.SelectedValue = "0";
            ddlidPais.SelectedValue = "0";

            foreach (ListItem item in lstidTabela.Items)
            {
                item.Selected = false;
            }

            Contato_EsconderCaixa();
            Endereco_EsconderCaixa();
            div_Usuarios.Visible = false;
        }

        private bool ValidarDados()
        {
            if (txtsRazaoSocial.Text.Length < 3)
            {
                MensagemPagina.MostraMensagem_Erro("Informe a Razão Social  válida para o cliente!");
                return false;
            }

            if (string.IsNullOrEmpty(ddlsTipo.SelectedValue))
            {
                MensagemPagina.MostraMensagem_Erro("Selecione o Tipo!");
                return false;
            }

            if (string.IsNullOrEmpty(cblsParceiros.SelectedValue))
            {
                MensagemPagina.MostraMensagem_Erro("Selecione uma opção entre: Nacional, Estrangeiro ou USA!");
                return false;
            }

            string sidTabela = string.Empty;
            foreach (ListItem item in lstidTabela.Items)
            {
                if (item.Selected)
                    sidTabela += string.Format("{0}|", item.Value);
            }

            if (sidTabela.Split('|').Where(t => t.Length > 0).Count() > 2)
            {
                MensagemPagina.MostraMensagem_Erro("Só é possível Vincular um Parceiro à, no máximo, <b>uma</b> Tabela de Preços do Tipo Vendas Customizadas e <b>uma</b> Tabela de Preços do Tipo LPU!");
                return false;
            }
            else if (sidTabela.Split('|').Where(t => t.Length > 0).Count() == 2)
            {
                Dictionary<string, string> vParamentros = new Dictionary<string, string>
                {
                    { "@sFuncao", "VALIDAR_TABELAS_VINCULADAS" },
                    { "@sidTabela", sidTabela },
                };
                DataTable dt = BD.ExecutarDataTable(sProcedure, vParamentros);

                if (dt.Rows[0].Field<int>("idTipoTabela") == dt.Rows[1].Field<int>("idTipoTabela"))
                {
                    MensagemPagina.MostraMensagem_Erro("Só é possível Vincular um Parceiro à, no máximo, <b>uma</b> Tabela de Preços do Tipo Vendas Customizadas e <b>uma</b> Tabela de Preços do Tipo LPU!");
                    return false;
                }
            }

            if (SwitchAtivo_Contribuinte.Recuperar().Equals("S") && string.IsNullOrEmpty(txtsRG_IE.Text))
            {
                MensagemPagina.MostraMensagem_Erro("É obrigatório que Parceiros marcados como Contribuintes do ICMS possuam Inscrição Estadual (IE)!");
                return false;
            }
			
            //if (txtnLimiteCredito.Text == "")																					
            //{
            //    MensagemPagina.MostraMensagem_Erro("Inserir um valor válido para o campo <b>Limite de Crédito</b>", true);
            //    return false;	 
            //}

            return true;
        }

        protected void carregaImgPrincipal(string idObjeto)
        {
            DataTable dsPesquisa;
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_IMAGEM" },
                { "@idTipoArquivo", "1997" },
                { "@idObjeto", idObjeto }
            };
            dsPesquisa = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametros);

            if (dsPesquisa.Rows.Count > 0)
            {
                DataRow imgBd = dsPesquisa.Rows[0];
                byte[] valorImgBd = (byte[])imgBd["vbArquivo"];
                string imgUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])imgBd["vbArquivo"]);
                imgParceiroPrincipal.ImageUrl = imgUrl;
                imgParceiroPrincipal.Visible = true;
            }
        }

        protected string removeCaracteres(string sObjeto)
        {
            string sObjetoSemCaracter = new string(sObjeto.Where(c => Char.IsDigit(c) || c == ',' || c == '.').Select(c => c == ',' ? '.' : c).ToArray());

            if (sObjetoSemCaracter == "")
            {
                sObjetoSemCaracter = "0";
            }
            return sObjetoSemCaracter;
        }

        protected void ModalExcluir(string idCmdExcluir)
        {
            if (!string.IsNullOrEmpty(idCmdExcluir) && !string.IsNullOrWhiteSpace(idCmdExcluir))
            {
                string[] id = idCmdExcluir.Replace("cphCorpo_", "").Replace("gv_", "gv").Replace("_Excluir", "").Trim().Split('_');
                if (id[0].Contains("gv"))
                {
                    if (id[0].Contains("Contato"))
                        gvContato_ExcluirItem(int.Parse(id[id.Length - 1]));
                    else if (id[0].Contains("Endereco"))
                        gvEndereco_ExcluirItem(int.Parse(id[id.Length - 1]));
                    else if (id[0].Contains("Bancario"))
                        gv_Bancario_ExcluirItem(int.Parse(id[id.Length - 1]));
                    else if (id[0].Contains("Usuario"))
                        gvUsuario_ExcluirItem(int.Parse(id[id.Length - 1]));
                }
            }
        }

        protected void Switch_NaoContribuinte()
        {
            //if (SwitchAtivo_Contribuinte.Recuperar().Equals("N"))
            //{
            //    txtsRG_IE.Text = string.Empty;
            //    txtsRG_IE.ReadOnly = true;
            //}
            //else
            //    txtsRG_IE.ReadOnly = false;
        }

        #endregion

        #region | Contato

        void Contato_Popular(DataSet dsPesquisa)
        {
            foreach (DataRow row in dsPesquisa.Tables[1].Rows)
            {
                cls_Clientes_Contatos objItem = new cls_Clientes_Contatos
                {
                    idContato = Convert.ToInt32(row["idContato"].ToString()),
                    idCliente = Convert.ToInt32(row["idCliente"].ToString()),
                    sTipoContato = row["sTipoContato"].ToString(),
                    sNome = row["sNome"].ToString(),
                    sTelefone = row["sTelefone"].ToString(),
                    sEmail = row["sEmail"].ToString(),
                    bExcluir = true
                };

                if (row["sRecebeEmail"].ToString() == "S")
                    objItem.sRecebeEmail = "Sim";
                else
                    objItem.sRecebeEmail = "Não";

                bs_Clientes_Contatos.Add(objItem);
            }

            gvContato_DataBind();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "AddHideContato", "$('#div_InputContato').hide();", true);
            Endereco_EsconderCaixa();
        }

        bool Contato_Salvar(string idCliente, string idHistorico)
        {
            bool bRetorno = false;
            try
            {
                foreach (var linha in bs_Clientes_Contatos)
                {
                    if (idCliente != "0")
                    {
                        Dictionary<string, string> vParametroContato_Incluir = new Dictionary<string, string>
                        {
                            { "@sFuncao", linha.bExcluir ? "CONTATO_INCLUIR" : "CONTATO_EXCLUIR" },
                            { "@idContato", linha.idContato.ToString() },
                            { "@idCliente", idCliente },
                            { "@sTipoContato", linha.sTipoContato },
                            { "@sNome", linha.sNome },
                            { "@sTelefone", linha.sTelefone },
                            { "@sEmail", linha.sEmail },
                            { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() },
                            { "@sRecebeEmail", linha.sRecebeEmail == "Sim" ? "S" : "N" },
                            { "@idHistorico", idHistorico }
                        };
                        DataSet dsContato_Incluir = BD.ExecutarDataSet(sProcedure, vParametroContato_Incluir);
                    }
                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro Salvar Contatos: " + ex.Message);
            }

            return bRetorno;

        }

        void gvContato_DataBind()
        {
            gvContato.DataSource = bs_Clientes_Contatos.Where(c => c.bExcluir);
            gvContato.DataBind();
        }

        protected void cmdContato_Incluir_Click(object sender, EventArgs e)
        {
            div_InputContato.Visible = true;
            //Thiago - 03/10/2024
            ddlContato_sTipoContato.Visible = true;
            txtContato_sTipoContato.Visible = false;
            Contato_LimparCampos();
            hddContato_index.Value = "-1";
            Contato_MostrarCaixa("Novo Contato");
            Endereco_EsconderCaixa();
        }

        void Contato_LimparCampos()
        {
            txtContato_sEmail.Text = "";
            txtContato_sNome.Text = "";
            txtContato_sTelefone.Text = "";
            txtContato_sTipoContato.Text = "";
            //Thiago - 03/10/2024
            ddlContato_sTipoContato.SelectedValue = "";
            EMAIL.Checked = false;
        }

        //Thiago - 03/10/2024
        protected void ddlContato_sTipoContato_SelectedIndexChanged(object sender, EventArgs e)
        {
            div_InputContato.Visible = true;

            if (ddlContato_sTipoContato.SelectedValue == "Outro")
            {
                txtContato_sTipoContato.Visible = true;
                ddlContato_sTipoContato.Visible = false;
                ddlContato_sTipoContato.SelectedIndex = -1;
                txtContato_sTipoContato.Text = string.Empty;
            }
        }

        void Contato_MostrarCaixa(string sTitulo)
        {
            //Thiago - 03/10/2024
            lblContato_Titulo.Text = sTitulo;
            div_Contato_Selecao.Visible = false;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Collapse", "$('#div_InputContato').collapse();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=ddlContato_sTipoContato]').focus();", true);
            Endereco_EsconderCaixa();
        }

        void Contato_EsconderCaixa()
        {
            div_Contato_Selecao.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "AddShowModalScript", "$('#div_InputContato').hide();", true);
        }

        protected void cmdContato_Confirmar_Click(object sender, EventArgs e)
        {
            string nIndex = hddContato_index.Value;
            if (nIndex != "-1")
            {
                int.TryParse(nIndex, out int i);

                //Thiago - 03/10/2024
                if (ddlContato_sTipoContato.SelectedValue != "")
                    txtContato_sTipoContato.Text = ddlContato_sTipoContato.SelectedValue;
                else
                    txtContato_sTipoContato.Text = txtContato_sTipoContato.Text;

                bs_Clientes_Contatos[i].sTipoContato = txtContato_sTipoContato.Text;
                bs_Clientes_Contatos[i].sNome = txtContato_sNome.Text;
                bs_Clientes_Contatos[i].sTelefone = txtContato_sTelefone.Text;
                bs_Clientes_Contatos[i].sEmail = txtContato_sEmail.Text;

                if (EMAIL.Checked)
                    bs_Clientes_Contatos[i].sRecebeEmail = "Sim";
                else
                    bs_Clientes_Contatos[i].sRecebeEmail = "Não";
            }
            else
            {
                cls_Clientes_Contatos objItem = new cls_Clientes_Contatos
                {
                    idCliente = Convert.ToInt32(hddidParceiro.Value)
                };

                //Thiago - 03/10/2024
                if (ddlContato_sTipoContato.SelectedValue != "")
                    txtContato_sTipoContato.Text = ddlContato_sTipoContato.SelectedValue;
                else
                    txtContato_sTipoContato.Text = txtContato_sTipoContato.Text;

                objItem.sTipoContato = txtContato_sTipoContato.Text;
                objItem.sNome = txtContato_sNome.Text;
                objItem.sTelefone = txtContato_sTelefone.Text;
                objItem.sEmail = txtContato_sEmail.Text;
                objItem.bExcluir = true;

                if (EMAIL.Checked)
                    objItem.sRecebeEmail = "Sim";
                else
                    objItem.sRecebeEmail = "Não";

                bs_Clientes_Contatos.Add(objItem);
            }

            gvContato_DataBind();
            Contato_EsconderCaixa();
            Endereco_EsconderCaixa();
        }

        protected void cmdContato_Cancelar_Click(object sender, EventArgs e)
        {
            Contato_EsconderCaixa();
            Endereco_EsconderCaixa();
        }

        protected void gvContato_RowDataBound(object sender, GridViewRowEventArgs e)
        => GRID.EsconderColunas(e, 0);

        protected void gvContato_ExcluirItem(int index)
        {
            bs_Clientes_Contatos.ToArray()[index].bExcluir = false;
            gvContato_DataBind();
            Contato_EsconderCaixa();
            Endereco_EsconderCaixa();
        }

        protected void gvContato_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            div_InputContato.Visible = true;
            int index = e.NewSelectedIndex;
            gvContato_DataBind();

            txtContato_sEmail.Text = bs_Clientes_Contatos[index].sEmail;
            txtContato_sNome.Text = bs_Clientes_Contatos[index].sNome;
            txtContato_sTelefone.Text = bs_Clientes_Contatos[index].sTelefone;
            txtContato_sTipoContato.Text = bs_Clientes_Contatos[index].sTipoContato;

            //Thiago - 03/10/2024
            if (ddlContato_sTipoContato.Items.FindByValue(txtContato_sTipoContato.Text) != null)
            {
                ddlContato_sTipoContato.SelectedValue = txtContato_sTipoContato.Text;
                ddlContato_sTipoContato.Visible = true;
                txtContato_sTipoContato.Visible = false;
            }
            else
            {
                txtContato_sTipoContato.Visible = true;
                ddlContato_sTipoContato.Visible = false;
            }

            hddContato_index.Value = index.ToString();

            if (bs_Clientes_Contatos[index].sRecebeEmail == "Sim")
                EMAIL.Checked = true;
            else
                EMAIL.Checked = false;

            Contato_MostrarCaixa("Editar Contato");
        }

        #endregion

        #region | Endereço

        void Endereco_Popular(DataSet dsPesquisa)
        {
            foreach (DataRow row in dsPesquisa.Tables[2].Rows)
            {
                if (cblsParceiros.SelectedValue == "N")
                {
                    this.gvEndereco.Columns[2].Visible = true;
                    this.gvEndereco.Columns[3].Visible = false;
                }
                if (cblsParceiros.SelectedValue == "E")
                {
                    this.gvEndereco.Columns[2].Visible = false;
                    this.gvEndereco.Columns[3].Visible = true;
                }
                if (cblsParceiros.SelectedValue == "U")
                {
                    this.gvEndereco.Columns[2].Visible = false;
                    this.gvEndereco.Columns[3].Visible = true;
                }

                cls_Clientes_Enderecos objItem = new cls_Clientes_Enderecos
                {
                    idEndereco = Convert.ToInt32(row["idEndereco"].ToString()),
                    idCliente = Convert.ToInt32(row["idCliente"].ToString()),
                    idTipoEndereco = Convert.ToInt32(row["idTipoEndereco"].ToString()),
                    sDscTipoEndereco = row["sDscTipoEndereco"].ToString(),
                    sCEP = row["sCEP"].ToString(),
                    sLogradouro = row["sLogradouro"].ToString(),
                    sNumero = row["sNumero"].ToString(),
                    sComplemento = row["sComplemento"].ToString(),
                    sBairro = row["sBairro"].ToString(),
                    sCidade = row["sCidade"].ToString(),
                    sEstado = string.IsNullOrEmpty(row["sEstado"].ToString()) || string.IsNullOrWhiteSpace(row["sEstado"].ToString()) ? "" : row["sEstado"].ToString(),
                    sPais = row["sPais"].ToString(),
                    sEnderecoCompleto = row["sEnderecoCompleto"].ToString(),
                    sEnderecoEstrangeiro = row["sEnderecoEstrangeiro"].ToString(),
                    bExcluir = true
                };

                bs_Clientes_Enderecos.Add(objItem);
            }

            gvEndereco_DataBind();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "AddS_hide_Endereco", "$('#divEndereco_Input').hide();", true);
            Contato_EsconderCaixa();
        }

        bool Endereco_Salvar(string idCliente, string idHistorico)
        {
            bool bRetorno = false;

            try
            {
                foreach (var linha in bs_Clientes_Enderecos)
                {
                    if (idCliente != "0")
                    {
                        Dictionary<string, string> vParametroEndereco_Incluir = new Dictionary<string, string>
                        {
                            { "@sFuncao", linha.bExcluir ? "ENDERECO_INCLUIR" : "ENDERECO_EXCLUIR" },
                            { "@idEndereco", linha.idEndereco.ToString() },
                            { "@idCliente", idCliente },
                            { "@idTipoEndereco", linha.idTipoEndereco.ToString() },
                            { "@sCEP", linha.sCEP },
                            { "@sLogradouro", linha.sLogradouro },
                            { "@sNumero", linha.sNumero },
                            { "@sComplemento", linha.sComplemento },
                            { "@sBairro", linha.sBairro },
                            { "@sCidade", linha.sCidade },
                            { "@sEstado", linha.sEstado },
                            { "@sPais", linha.sPais },
                            { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() },
                            { "@idHistorico", idHistorico }
                        };
                        DataSet dsEndereco_Incluir = BD.ExecutarDataSet(sProcedure, vParametroEndereco_Incluir);
                    }
                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro Salvar Endereço: " + ex.Message);
            }

            return bRetorno;
        }

        void gvEndereco_DataBind()
        {
            gvEndereco.DataSource = bs_Clientes_Enderecos.Where(e => e.bExcluir);
            gvEndereco.DataBind();

            if (cblsParceiros.SelectedValue == "N")
            {
                DIV_txtEstado.Visible = false;
                lbl_Cidade.Text = "Cidade";
                this.gvEndereco.Columns[2].Visible = true;
                this.gvEndereco.Columns[3].Visible = false;
            }
            if (cblsParceiros.SelectedValue == "E")
            {
                DIV_ddlEstado.Visible = false;
                DIV_txtEstado.Visible = false;
                lbl_Cidade.Text = "Cidade / Estado";
                this.gvEndereco.Columns[2].Visible = false;
                this.gvEndereco.Columns[3].Visible = true;
            }
            if (cblsParceiros.SelectedValue == "U")
            {
                DIV_ddlEstado.Visible = false;
                lbl_Cidade.Text = "Cidade";
                this.gvEndereco.Columns[2].Visible = false;
                this.gvEndereco.Columns[3].Visible = true;
            }
        }

        void Endereco_LimparCampos()
        {
            ddlEndereco_TipoEndereco.SelectedValue = "0";
            txtEndereco_sLogradouro.Text = "";
            txtEndereco_sNumero.Text = "";
            txtEndereco_sComplemento.Text = "";
            txtEndereco_sBairro.Text = "";
            txtEndereco_sCEP.Text = "";
            txtEndereco_sCidade.Text = "";
            txtEndereco_sPais.Text = "";
            ddlEndereco_sEstado.SelectedValue = "";
            txtEstrangeiro_sEstado.Text = "";
        }

        void Endereco_MostrarCaixa(string sTitulo)
        {
            lblEndereco_Titulo.Text = sTitulo;
            divEndereco_Selecao.Visible = false;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Endereco_Collapse", "$('#divEndereco_Input').collapse();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Endereco_Focus", "$('[id$=ddlEndereco_TipoEndereco]').focus();", true);

            if (cblsParceiros.SelectedValue == "N")
            {
                DIV_txtEstado.Visible = false;
                DIV_ddlEstado.Visible = true;
                lbl_Cidade.Text = "Cidade";
            }

            if (cblsParceiros.SelectedValue == "E")
            {
                DIV_ddlEstado.Visible = false;
                DIV_txtEstado.Visible = false;
                lbl_Cidade.Text = "Cidade / Estado";
            }

            if (cblsParceiros.SelectedValue == "U")
            {
                DIV_ddlEstado.Visible = false;
                DIV_txtEstado.Visible = true;
                lbl_Cidade.Text = "Cidade";
            }

            Contato_EsconderCaixa();
        }

        void Endereco_EsconderCaixa()
        {
            divEndereco_Selecao.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Endereco_Hide", "$('#divEndereco_Input').hide();", true);
        }

        protected void cmdEndereco_Incluir_Click(object sender, EventArgs e)
        {
            if (cblsParceiros.SelectedValue == "N")
            {
                DIV_txtEstado.Visible = false;
                lbl_Cidade.Text = "Cidade";
            }

            if (cblsParceiros.SelectedValue == "E")
            {
                DIV_ddlEstado.Visible = false;
                DIV_txtEstado.Visible = false;
                lbl_Cidade.Text = "Cidade / Estado";
            }

            if (cblsParceiros.SelectedValue == "U")
            {
                lbl_Cidade.Text = "Cidade";
                DIV_ddlEstado.Visible = false;
                DIV_txtEstado.Visible = true;
            }

            divEndereco_Input.Visible = true;
            Endereco_LimparCampos();
            hddEndereco_Index.Value = "-1";
            Endereco_MostrarCaixa("Novo Endereço");
        }

        protected void cmdEndereco_Confirmar_Click(object sender, EventArgs e)
        {
            string nIndex = hddEndereco_Index.Value;
            string sEnderecoCompleto = txtEndereco_sLogradouro.Text;
            string sEnderecoEstrangeiro = txtEndereco_sNumero.Text;
            string mensagem = "";

            if (!string.IsNullOrEmpty(txtEndereco_sNumero.Text))
                sEnderecoCompleto += string.Format(", {0}", txtEndereco_sNumero.Text);

            if (!string.IsNullOrEmpty(txtEndereco_sComplemento.Text))
                sEnderecoCompleto += string.Format(" {0}", txtEndereco_sComplemento.Text);

            if (!string.IsNullOrEmpty(txtEndereco_sBairro.Text))
                sEnderecoCompleto += string.Format(", {0}", txtEndereco_sBairro.Text);

            if (!string.IsNullOrEmpty(txtEndereco_sCEP.Text))
                sEnderecoCompleto += string.Format(" - {0}", txtEndereco_sCEP.Text);

            if (!string.IsNullOrEmpty(txtEndereco_sCidade.Text))
                sEnderecoCompleto += string.Format(" - {0}", txtEndereco_sCidade.Text);

            if (!string.IsNullOrEmpty(ddlEndereco_sEstado.SelectedValue))
                sEnderecoCompleto += string.Format("/{0}", ddlEndereco_sEstado.SelectedValue);

            if (!string.IsNullOrEmpty(txtEndereco_sPais.Text))
                sEnderecoCompleto += string.Format("-{0}", txtEndereco_sPais.Text);

            if (!string.IsNullOrEmpty(txtEndereco_sLogradouro.Text))
                sEnderecoEstrangeiro += string.Format(" {0}", txtEndereco_sLogradouro.Text);
            else
                mensagem = "Informe o Endereço !";

            if (!string.IsNullOrEmpty(txtEndereco_sCidade.Text))
                sEnderecoEstrangeiro += string.Format(", {0}", txtEndereco_sCidade.Text);

            if (!string.IsNullOrEmpty(txtEstrangeiro_sEstado.Text))
                sEnderecoEstrangeiro += string.Format(" {0}", txtEstrangeiro_sEstado.Text);

            if (!string.IsNullOrEmpty(txtEndereco_sCEP.Text))
                sEnderecoEstrangeiro += string.Format("  {0}", txtEndereco_sCEP.Text);

            if (!string.IsNullOrEmpty(txtEndereco_sPais.Text))
                sEnderecoEstrangeiro += string.Format(" {0}", txtEndereco_sPais.Text);

            if (ddlEndereco_TipoEndereco.SelectedValue == "0")
                mensagem = "Selecione o Tipo";

            if (mensagem == "")
            {
                if (nIndex != "-1")
                {
                    int i = Convert.ToInt32(nIndex);
                    bs_Clientes_Enderecos[i].idTipoEndereco = Convert.ToInt32(ddlEndereco_TipoEndereco.SelectedValue);
                    bs_Clientes_Enderecos[i].sDscTipoEndereco = ddlEndereco_TipoEndereco.SelectedItem.ToString();
                    bs_Clientes_Enderecos[i].sCEP = txtEndereco_sCEP.Text;
                    bs_Clientes_Enderecos[i].sLogradouro = txtEndereco_sLogradouro.Text;
                    bs_Clientes_Enderecos[i].sNumero = txtEndereco_sNumero.Text;
                    bs_Clientes_Enderecos[i].sBairro = txtEndereco_sBairro.Text;
                    bs_Clientes_Enderecos[i].sComplemento = txtEndereco_sComplemento.Text;
                    bs_Clientes_Enderecos[i].sCidade = txtEndereco_sCidade.Text;
                    bs_Clientes_Enderecos[i].sEstado = ddlEndereco_sEstado.SelectedValue;
                    bs_Clientes_Enderecos[i].sPais = txtEndereco_sPais.Text;
                    bs_Clientes_Enderecos[i].sEnderecoCompleto = sEnderecoCompleto;
                    bs_Clientes_Enderecos[i].sEnderecoEstrangeiro = sEnderecoEstrangeiro;
                    bs_Clientes_Enderecos[i].sEstado = !string.IsNullOrEmpty(txtEstrangeiro_sEstado.Text) ? txtEstrangeiro_sEstado.Text : bs_Clientes_Enderecos[i].sEstado;
                }
                else
                {
                    cls_Clientes_Enderecos objItem = new cls_Clientes_Enderecos
                    {
                        idCliente = Convert.ToInt32(hddidParceiro.Value),
                        idTipoEndereco = Convert.ToInt32(ddlEndereco_TipoEndereco.SelectedValue),
                        sDscTipoEndereco = ddlEndereco_TipoEndereco.SelectedItem.ToString(),
                        sCEP = txtEndereco_sCEP.Text,
                        sLogradouro = txtEndereco_sLogradouro.Text,
                        sNumero = txtEndereco_sNumero.Text,
                        sBairro = txtEndereco_sBairro.Text,
                        sComplemento = txtEndereco_sComplemento.Text,
                        sCidade = txtEndereco_sCidade.Text,
                        sEstado = ddlEndereco_sEstado.SelectedValue,
                        sPais = txtEndereco_sPais.Text,
                        sEnderecoCompleto = sEnderecoCompleto,
                        sEnderecoEstrangeiro = sEnderecoEstrangeiro
                    };

                    objItem.sEstado = !string.IsNullOrEmpty(txtEstrangeiro_sEstado.Text) ? txtEstrangeiro_sEstado.Text : objItem.sEstado;
                    objItem.bExcluir = true;
                    bs_Clientes_Enderecos.Add(objItem);
                }

                gvEndereco_DataBind();
                Endereco_EsconderCaixa();
                Contato_EsconderCaixa();
            }
            else
            {
                divEndereco_Input.Visible = true;
                msgEndereco.MostraMensagem_Erro(mensagem);
            }
        }

        protected void cmdEndereco_Cancelar_Click(object sender, EventArgs e)
        {
            Endereco_EsconderCaixa();
            Contato_EsconderCaixa();
        }

        protected void gvEndereco_RowDataBound(object sender, GridViewRowEventArgs e)
        => GRID.EsconderColunas(e, 0);

        protected void gvEndereco_ExcluirItem(int index)
        {
            bs_Clientes_Enderecos.ToArray()[index].bExcluir = false;
            gvEndereco_DataBind();
            Contato_EsconderCaixa();
            Endereco_EsconderCaixa();
        }

        protected void gvEndereco_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            divEndereco_Input.Visible = true;
            int i = e.NewSelectedIndex;
            gvEndereco_DataBind();

            ddlEndereco_TipoEndereco.SelectedValue = bs_Clientes_Enderecos[i].idTipoEndereco.ToString();
            txtEndereco_sCEP.Text = bs_Clientes_Enderecos[i].sCEP;
            txtEndereco_sLogradouro.Text = bs_Clientes_Enderecos[i].sLogradouro;
            txtEndereco_sNumero.Text = bs_Clientes_Enderecos[i].sNumero;
            txtEndereco_sBairro.Text = bs_Clientes_Enderecos[i].sBairro;
            txtEndereco_sComplemento.Text = bs_Clientes_Enderecos[i].sComplemento;
            txtEndereco_sCidade.Text = bs_Clientes_Enderecos[i].sCidade;

            if (cblsParceiros.SelectedValue == "N" || cblsParceiros.SelectedValue == "")
                ddlEndereco_sEstado.SelectedValue = bs_Clientes_Enderecos[i].sEstado;

            txtEndereco_sPais.Text = bs_Clientes_Enderecos[i].sPais;

            if (cblsParceiros.SelectedValue == "E")
                txtEstrangeiro_sEstado.Text = bs_Clientes_Enderecos[i].sEstado;

            if (cblsParceiros.SelectedValue == "U")
                txtEstrangeiro_sEstado.Text = bs_Clientes_Enderecos[i].sEstado;

            hddEndereco_Index.Value = i.ToString();
            Endereco_MostrarCaixa("Editar Endereço");
        }

        protected void txtEndereco_sCEP_TextChanged(object sender, EventArgs e)
        {
            divEndereco_Input.Visible = true;

            try
            {
                if (cblsParceiros.SelectedValue == "N" || cblsParceiros.SelectedValue == "")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sCEP", txtEndereco_sCEP.Text.Replace("-", "") }
                    };
                    DataSet dsCEP = BD.ExecutarDataSet("sp_Consulta_CEP", vParametros);

                    if (BD.ValidarDataSet(dsCEP))
                    {
                        txtEndereco_sLogradouro.Text = RETORNO.DATASET(dsCEP, 0, "sLogradouro");
                        txtEndereco_sBairro.Text = RETORNO.DATASET(dsCEP, 0, "sBairro");
                        ddlEndereco_sEstado.SelectedValue = RETORNO.DATASET(dsCEP, 0, "sUF");
                        txtEndereco_sCidade.Text = RETORNO.DATASET(dsCEP, 0, "sCidade");
                        txtEstrangeiro_sEstado.Text = RETORNO.DATASET(dsCEP, 0, "sUF");
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Endereco_Focus", "$('[id$=txtEndereco_sNumero]').focus();", true);
                    }
                    else
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Endereco_Focus", "$('[id$=txtEndereco_sLogradouro]').focus();", true);
                }
            }
            catch
            {
                txtEndereco_sLogradouro.Text = "";
                txtEndereco_sBairro.Text = "";
                ddlEndereco_sEstado.SelectedValue = "";
                txtEndereco_sCidade.Text = "";
                txtEstrangeiro_sEstado.Text = "";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Endereco_Focus", "$('[id$=txtEndereco_sLogradouro]').focus();", true);
            }

            Contato_EsconderCaixa();
        }

        #endregion

        #region | Usuários

        protected void gvUsuario_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0);
        }

        protected void gvUsuario_ExcluirItem(int index)
        {
            GridViewRow row = gvUsuario.Rows[index];
            string idUsuario = row.Cells[0].Text;
            FUNCOES.DirecionaPagina(string.Format("app/Paginas/Manutencao/Usuarios_Detalhe.aspx?idu={0}&sf={1}|03E", idUsuario, hddidParceiro.Value));
        }

        protected void gvUsuario_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            int i = e.NewSelectedIndex;
            GridViewRow row = gvUsuario.Rows[i];
            string idUsuario = row.Cells[0].Text;
            FUNCOES.DirecionaPagina(string.Format("app/Paginas/Manutencao/Usuarios_Detalhe.aspx?idu={0}&sf={1}|02U", idUsuario, hddidParceiro.Value));
        }

        protected void lnkUsuarios_Adicionar_Click(object sender, EventArgs e)
        {
            FUNCOES.DirecionaPagina(string.Format("app/Paginas/Manutencao/Usuarios_Detalhe.aspx?idu={0}&sf={1}|01I", "0", hddidParceiro.Value));
        }

        void Usuarios_Popular(DataSet ds)
        {
            gvUsuario.DataSource = ds.Tables[3];
            gvUsuario.DataBind();
            div_Usuarios.Visible = true;
        }

        protected void lnkPagamento_Click(object sender, EventArgs e)
        {
            BaseMultiview_CondicaoPagamento.Visible = !BaseMultiview_CondicaoPagamento.Visible;
            BaseMultiview_CondicaoPagamento.ActiveViewIndex = 0;
            updCondicaoPagamento.Update();
            Contato_EsconderCaixa();
            Endereco_EsconderCaixa();
        }

        #endregion

        #region | Script

        void RegistraScript(string script)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("$('[id*=txtEndereco_sCEP]').mask('00000-000');");
            sb.Append("$('[id*=txtsCNPJ]').mask('00.000.000/0000-00', { reverse: true });");
            sb.Append("$('[id*=txtsCPF]').mask('000.000.000-00', { reverse: true });");
            sb.Append("$('[id*=txtsFEIN]').mask('00-0000000', { reverse: true });");
            sb.Append("$('[id*=txtsRNE]').mask('00000.000000/0000-00', { reverse: true });");
			sb.Append("$('[id*=txtnLimiteCredito]').mask('0.000.000.009,99', { reverse: true });");
            sb.Append("$('[id*=txtsInscricaoMunicipal]').mask('000000000000000', { reverse: true });");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Script", sb.ToString(), true);

            sb.Clear();

            sb.Append("$v192(function() {\r\n");
            // Modal Excluir
            sb.Append("     $v192(\"#dialog-Excluir\").dialog({\r\n");
            sb.Append("         resizable: false,\r\n");
            sb.Append("         height: \"auto\",\r\n");
            sb.Append("         width: 400,\r\n");
            sb.Append("         modal: true,\r\n");
            sb.Append("         autoOpen: false,\r\n");
            sb.Append("         buttons:\r\n");
            sb.Append("         {\r\n");
            sb.Append("             \"Sim\": function() {\r\n");
            sb.Append("                 __doPostBack(\"funcao_EXCLUIR\", \"\");\r\n");
            sb.Append("                 $v192(this).dialog(\"close\");\r\n");
            sb.Append("             },\r\n");
            sb.Append("             \"Não\": function() {\r\n");
            sb.Append("                 $v192(this).dialog(\"close\");\r\n");
            sb.Append("             },\r\n");
            sb.Append("         }\r\n");
            sb.Append("     });\r\n");
            sb.Append("     $v192('[id*=_Excluir]').click(function(e) {\r\n");
            sb.Append("         e.preventDefault();\r\n");
            sb.Append("         $('[id*=hdd_modalExcluir]').val($(this).attr('id'));\r\n");
            sb.Append("         $v192('#dialog-Excluir').dialog('open');\r\n");
            sb.Append("     });\r\n\r\n");
            sb.Append("});\r\n\r\n");
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ScriptPagina_Modal", sb.ToString(), true);
        }

        #endregion

        #region | Eventos

        protected void cmdSalvar_Click(object sender, EventArgs e)
        => SalvarParceiro();

        protected void cmdAvancar_click(object sender, EventArgs e)
        {
            int id = 0;
            if (txtidCliente.Text != "Novo")
                id = Convert.ToInt32(txtidCliente.Text) + 1;
            else
                id = Convert.ToInt32(Request["id"]) + 1;

            Response.Redirect($"Parceiros_Detalhe.aspx?id={id}");
        }

        protected void cmdRetornar_click(object sender, EventArgs e)
        {
            int id = 0;
            if (txtidCliente.Text != "Novo")
                id = Convert.ToInt32(txtidCliente.Text) - 1;
            else
                id = Convert.ToInt32(Request["id"]) - 1;

            Response.Redirect($"Parceiros_Detalhe.aspx?id={id}");
        }

        protected void ddlsTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            SwitchAtivo_Contribuinte.Visible = true;
            lbl_sRG_IE.Text = "IE";
            DIV_VAT.Visible = false;
            DIV_FEIN.Visible = false;
            DIV_CNPJ.Visible = false;
            DIV_RNE.Visible = false;
            DIV_CPF.Visible = false;
            DIV_IE.Visible = false;
            DIV_IM.Visible = false;
            DIV_SUFRAMA.Visible = false;

            if (cblsParceiros.SelectedValue == "E" && ddlsTipo.SelectedValue == "J")
            {
                DIV_VAT.Visible = true;								   
                lblsNomeFantasia.Text = "Nome Fantasia";
                lblsRazaoSocial.Text = "Razão Social";
            }

            if (cblsParceiros.SelectedValue == "U" && ddlsTipo.SelectedValue == "J")
            {
                DIV_FEIN.Visible = true;										   
                lblsNomeFantasia.Text = "Nome Fantasia";
                lblsRazaoSocial.Text = "Razão Social";
            }

            if (cblsParceiros.SelectedValue == "N" && ddlsTipo.SelectedValue == "J")
            {										
                DIV_CNPJ.Visible = true;										
                DIV_IE.Visible = true;
                DIV_IM.Visible = true;
                DIV_SUFRAMA.Visible = true;
                lblsNomeFantasia.Text = "Nome Fantasia";
                lblsRazaoSocial.Text = "Razão Social";
            }

            if (ddlsTipo.SelectedValue == "F")
            {
                //SwitchAtivo_Contribuinte.Visible = false;

                if (cblsParceiros.SelectedValue == "E")
                {											 
                    DIV_RNE.Visible = true;											
                    lblsNomeFantasia.Text = "Apelido";
                    lblsRazaoSocial.Text = "Nome Completo";
                }

                if (cblsParceiros.SelectedValue == "U")
                {							
                    DIV_RNE.Visible = true;											
                    lblsNomeFantasia.Text = "Apelido";
                    lblsRazaoSocial.Text = "Nome Completo";
                }

                if (cblsParceiros.SelectedValue == "N")
                {					
                    DIV_CPF.Visible = true;
                    DIV_IE.Visible = true;

                    lblsNomeFantasia.Text = "Apelido";
                    lblsRazaoSocial.Text = "Nome Completo";
                    lbl_sRG_IE.Text = "RG";
                }
            }

            txtsCNPJ.Focus();
            Contato_EsconderCaixa();
            Endereco_EsconderCaixa();
        }

        protected void cblsParceiros_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (ListItem item in cblsParceiros.Items)
            {
                if (item.Selected)
                {
                    foreach (ListItem otherItem in cblsParceiros.Items)
                    {
                        if (otherItem != item)
                            otherItem.Selected = false;
                    }
                    break; // Sair do loop após desmarcar os outros itens
                }
            }
            ddlsTipo_SelectedIndexChanged(sender, e);
        }

        protected void cblidTipoParceiro_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool ClienteChecked = false;
            bool FornecedorChecked = false;

            foreach (ListItem item in cblidTipoParceiro.Items)
            {
                if (item.Value == "0" && item.Selected)
                    ClienteChecked = true;

                if (item.Value == "1" && item.Selected)
                    FornecedorChecked = true;
            }

            if (ClienteChecked)
                DIV_Vendedor.Visible = true;
            else
            {
                DIV_Vendedor.Visible = false;
                ddlidVendedor.SelectedValue = "0";
            }

            if (FornecedorChecked)
                DIV_Comprador.Visible = true;
            else
            {
                DIV_Comprador.Visible = false;
                ddlidComprador.SelectedValue = "0";
            }
        }

        #endregion

        #region | Bancario

        bool Bancario_Salvar(string idCliente, string idHistorico)
        {
            bool bRetorno = false;
            try
            {
                foreach (var linha in bs_Clientes_Bancario)
                {
                    DataSet dsBancario_Incluir;
                    Dictionary<string, string> vParam = new Dictionary<string, string> {
                        { "@sFuncao", linha.bExcluir ? "BANCARIO_INCLUIR" : "BANCARIO_EXCLUIR_ITEM" },
                        { "@idBancario", linha.idBancario.ToString() },
                        { "@idParceiro", idCliente },
                        { "@idHistorico", idHistorico },
                        { "@sCompany", linha.sCompany },
                        { "@sCoutry", linha.sCoutry },
                        { "@sBank", linha.sBank },
                        { "@sSwift", linha.sSwift },
                        { "@sAba", linha.sAba },
                        { "@sAccount", linha.sAccount },
                        { "@sEndereco", linha.sEndereco },
                        { "@idMoeda", linha.idMoeda.ToString() },
                        { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                    };
                    dsBancario_Incluir = BD.ExecutarDataSet(sProcedure, vParam);
                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                msgBancario.MostraMensagem_Erro("Erro Salvar Dados Bancários: " + ex.Message);
            }
            return bRetorno;
        }

        void Bancario_Popular(DataSet dsPesquisa)
        {
            foreach (DataRow row in dsPesquisa.Tables[5].Rows)
            {
                cls_Clientes_Bancario objItem = new cls_Clientes_Bancario
                {
                    idBancario = Convert.ToInt32(row["idBancario"].ToString()),
                    sCompany = row["sCompany"].ToString(),
                    sCoutry = row["sCoutry"].ToString(),
                    sBank = row["sBank"].ToString(),
                    sSwift = row["sSwift"].ToString(),
                    sAba = row["sAba"].ToString(),
                    sAccount = row["sAccount"].ToString(),
                    sEndereco = row["sEndereco"].ToString(),
                    sDscTipoMoeda = row["sDscTipoMoeda"].ToString(),
                    idMoeda = Convert.ToInt32(row["idMoeda"])
                };
                bs_Clientes_Bancario.Add(objItem);
            }
            gvBancario_DataBind();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "AddHideBancario", "$('#div_imputBancario').hide();", true);
            Bancario_EsconderCaixa();
        }

        protected void cmdBancario_Click(object sender, EventArgs e)
        {
            div_imputBancario.Visible = true;
            Bancario_LimparCampos();
            hddBancario_index.Value = "-1";
            Bancario_LimparCampos();
            Bancario_MostrarCaixa("Novo Canal Bancário");
        }

        void Bancario_LimparCampos()
        {
            txtCompany.Text = "";
            txtCoutry.Text = "";
            txtBank.Text = "";
            txtSwift.Text = "";
            txtAba.Text = "";
            txtAccount.Text = "";
            txtEnderecoBancario.Text = "";
        }

        void Bancario_MostrarCaixa(string sTitulo)
        {
            lblBancario.Text = sTitulo;
            div_cmdBancario.Visible = false;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Collapse", "$('#div_imputBancario').collapse();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtCompany]').focus();", true);
            Bancario_EsconderCaixa();
        }

        protected void cmdBancario_Cancelar_Click(object sender, EventArgs e)
        {
            Bancario_EsconderCaixa();
        }

        void Bancario_EsconderCaixa()
        {
            div_cmdBancario.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "AddShowModalScript", "$('#div_imputBancario').hide();", true);
        }

        string Get_NomeTipoMoeda(string idMoeda)
        {
            DataSet dsPesquisa;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "FLOW-POR-ID");
            vParametros.Add("@idMoeda", idMoeda);
            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa))
            {
                return RETORNO.DATASET(dsPesquisa, "sDscTipoMoeda");
            }
            else
            {
                return "sem referência";
            }
        }

        protected void cmdBancario_Confirmar_Click(object sender, EventArgs e)
        {
            string nIndex = hddBancario_index.Value;
            string sMensagem = "";

            if (ValidarDados_Bancario(ref sMensagem))
            {
                if (nIndex != "-1")
                {
                    int i = Convert.ToInt32(nIndex);
                    bs_Clientes_Bancario[i].sCompany = txtCompany.Text;
                    bs_Clientes_Bancario[i].sCoutry = txtCoutry.Text;
                    bs_Clientes_Bancario[i].sBank = txtBank.Text;
                    bs_Clientes_Bancario[i].sSwift = txtSwift.Text;
                    bs_Clientes_Bancario[i].sAba = txtAba.Text;
                    bs_Clientes_Bancario[i].sAccount = txtAccount.Text;
                    bs_Clientes_Bancario[i].sEndereco = txtEnderecoBancario.Text;
                    bs_Clientes_Bancario[i].idMoeda = Convert.ToInt32(ddlidMoeda.SelectedValue);
                    bs_Clientes_Bancario[i].sDscTipoMoeda = Get_NomeTipoMoeda(ddlidMoeda.SelectedValue);
                }
                else
                {
                    cls_Clientes_Bancario objItem = new cls_Clientes_Bancario
                    {
                        idBancario = bs_Clientes_Bancario.Count() + 1,
                        sCompany = txtCompany.Text,
                        sCoutry = txtCoutry.Text,
                        sBank = txtBank.Text,
                        sSwift = txtSwift.Text,
                        sAba = txtAba.Text,
                        sAccount = txtAccount.Text,
                        sEndereco = txtEnderecoBancario.Text,
                        idMoeda = Convert.ToInt32(ddlidMoeda.SelectedValue),
                        sDscTipoMoeda = Get_NomeTipoMoeda(ddlidMoeda.SelectedValue)
                    };
                    bs_Clientes_Bancario.Add(objItem);
                }
            }
            gvBancario_DataBind();
            Bancario_EsconderCaixa();
        }

        private bool ValidarDados_Bancario(ref string sMensagem)
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (txtCompany.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira Company!";
            }
            if (txtCoutry.Text == "")
            {
                if (sMensagemErro != "")
                {
                    sMensagemErro = sMensagemErro + "</br>";
                }
                sMensagemErro += "Insira Coutry!";
            }
            if (txtBank.Text == "")
            {
                if (sMensagemErro != "")
                {
                    sMensagemErro = sMensagemErro + "</br>";
                }
                sMensagemErro += "Insira Bank!";
            }
            if (txtSwift.Text == "")
            {
                if (sMensagemErro != "")
                {
                    sMensagemErro = sMensagemErro + "</br>";
                }
                sMensagemErro += "Insira Swift!";
            }
            if (txtAccount.Text == "")
            {
                if (sMensagemErro != "")
                {
                    sMensagemErro = sMensagemErro + "</br>";
                }
                sMensagemErro += "Insira Account!";
            }
            if (ddlidMoeda.SelectedValue == "0")
            {
                if (sMensagemErro != "")
                {
                    sMensagemErro = sMensagemErro + "</br>";
                }
                sMensagemErro += "Insira a Moeda!";
            }
            if (sMensagemErro != "")
            {
                bRetorno = false;
                msgBancario.MostraMensagem_Erro(sMensagemErro);
            }
            return bRetorno;
        }

        void gvBancario_DataBind()
        {
            gv_Bancario.DataSource = bs_Clientes_Bancario.Where(b => b.bExcluir); ;
            gv_Bancario.DataBind();
        }

        protected void gv_Bancario_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0);
        }

        protected void gv_Bancario_ExcluirItem(int index)
        {
            // 1. Pegamos a lista apenas dos itens que estão aparecendo na Grid (bExcluir == true)
            var listaVisivel = bs_Clientes_Bancario.Where(x => x.bExcluir).ToList();

            // 2. Localizamos o item exato que foi clicado usando o índice da Grid
            if (index >= 0 && index < listaVisivel.Count)
            {
                // 3. Marcamos como false. O item continua na lista bs_Clientes_Bancario,
                // mas o DataBind vai escondê-lo da tela e o Salvar vai saber que deve logar a exclusão.
                listaVisivel[index].bExcluir = false;
            }

            gvBancario_DataBind();
            Bancario_EsconderCaixa();
        }

        protected void gv_Bancario_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            div_imputBancario.Visible = true;
            int index = e.NewSelectedIndex;
            gvBancario_DataBind();
            txtCompany.Text = bs_Clientes_Bancario[index].sCompany;
            txtCoutry.Text = bs_Clientes_Bancario[index].sCoutry;
            txtBank.Text = bs_Clientes_Bancario[index].sBank;
            txtSwift.Text = bs_Clientes_Bancario[index].sSwift;
            txtAba.Text = bs_Clientes_Bancario[index].sAba;
            txtAccount.Text = bs_Clientes_Bancario[index].sAccount;
            txtEnderecoBancario.Text = bs_Clientes_Bancario[index].sEndereco;
            ddlidMoeda.SelectedValue = bs_Clientes_Bancario[index].idMoeda.ToString();
            hddBancario_index.Value = index.ToString();
            Bancario_MostrarCaixa("Editar Bancario");
        }

        #endregion

        #region | Condição Pagamento

        private bool ValidarCondicaoPagamentoCompra(ref string sMensagem, int idTipo)
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (idTipo == 1)
            {
                if (ddlCondicaoCompra.SelectedValue == "0")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione a Condição de Pagamento!";

                if (sMensagemErro != "")
                {
                    bRetorno = false;
                    MensagemCondiçãoCompra.MostraMensagem_Erro(sMensagemErro);
                }
            }
            else if (idTipo == 2)
            {
                if (ddlCondicaoPagamento.SelectedValue == "0")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione a Condição de Pagamento!";

                if (sMensagemErro != "")
                {
                    bRetorno = false;
                    MensagemPagina3.MostraMensagem_Erro(sMensagemErro);
                }
            }

            return bRetorno;
        }

        void PopulaCondicaoPagamento(string idParceiro, DataSet dsPesquisa)
        {
            try
            {
                foreach (DataRow row in dsPesquisa.Tables[4].Rows)
                {
                    if (row["idCondicaoPagamento"].ToString() != "")
                    {
                        if (row["idTipo"].ToString() == "1")
                        {
                            cls_CondicoesPagamentoCompra objItem = new cls_CondicoesPagamentoCompra
                            {
                                idRegistro = Convert.ToInt16(row["idRegistro"].ToString()),
                                idParceiro = Convert.ToInt16(row["idParceiro"].ToString()),
                                idCondicaoPagamento = Convert.ToInt16(row["idCondicaoPagamento"].ToString()),
                                idTipo = Convert.ToInt16(row["idTipo"].ToString()),
                                sFuncao = "CONSULTAR",
                                sDscCondicaoPagamento = row["sDscCondicaoPagamento"].ToString(),
                                idContador = BS_PAGAMENTOCOMPRA.Count() + 1
                            };
                            BS_PAGAMENTOCOMPRA.Add(objItem);
                        }
                        else if (row["idTipo"].ToString() == "2")
                        {
                            cls_CondicoesPagamento objItem = new cls_CondicoesPagamento
                            {
                                idRegistro = Convert.ToInt16(row["idRegistro"].ToString()),
                                idParceiro = Convert.ToInt16(row["idParceiro"].ToString()),
                                idCondicaoPagamento = Convert.ToInt16(row["idCondicaoPagamento"].ToString()),
                                idTipo = Convert.ToInt16(row["idTipo"].ToString()),
                                sFuncao = "CONSULTAR",
                                sDscCondicaoPagamento = row["sDscCondicaoPagamento"].ToString(),
                                idContador = BS_CONDICAO_PAGAMENTO.Count() + 1
                            };
                            BS_CONDICAO_PAGAMENTO.Add(objItem);
                        }
                    }
                }

                dtgCondicaoPagamento_DataBind();
                gvCondicaoPagamento_DataBind();
            }
            catch { }
        }

        bool SalvarCondicaoPagamento(string idParceiro, string idHistorico)
        {
            bool bRetorno = true;
            try
            {                
                // Criamos uma lista unificada apenas para processar o salvamento
                var listaCompleta = BS_PAGAMENTOCOMPRA.Select(x => new { x.idRegistro, x.idCondicaoPagamento, x.idTipo, x.sFuncao, x.sDscCondicaoPagamento })
                    .Concat(BS_CONDICAO_PAGAMENTO.Select(x => new { x.idRegistro, x.idCondicaoPagamento, x.idTipo, x.sFuncao, x.sDscCondicaoPagamento }));

                foreach (var linha in listaCompleta)
                {   
                    if (linha.sFuncao != "CONSULTAR")
                    {
                        Dictionary<string, string> vParametroItens = new Dictionary<string, string> {
                            { "@sFuncao", linha.sFuncao },
                            { "@idParceiro", idParceiro },
                            { "@idHistorico", idHistorico },
                            { "@idCondicaoPagamento", linha.idCondicaoPagamento.ToString() },
                            { "@idTipoCondicoesdePagamento", linha.idTipo.ToString() },
                            { "@idRegistro", linha.idRegistro.ToString() },
                            { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                        };
                        DataSet ds = BD.ExecutarDataSet(sProcedure, vParametroItens);
                    }
                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro Salvar Condição Pagamento: " + ex.Message);
            }
            return bRetorno;
        }

        void IncluirCondicaoPagamento(int idTipo)
        {
            string sMensagem = "";
            if (idTipo == 1)
            {
                if (ValidarCondicaoPagamentoCompra(ref sMensagem, idTipo))
                {
                    cls_CondicoesPagamentoCompra objItem = new cls_CondicoesPagamentoCompra
                    {
                        idParceiro = Convert.ToInt16(hddidParceiro.Value),
                        idCondicaoPagamento = Convert.ToInt16(ddlCondicaoCompra.SelectedValue),
                        sDscCondicaoPagamento = ddlCondicaoCompra.SelectedItem.ToString(),
                        sFuncao = "INCLUIR_PERMISSOES_PAGAMENTO",
                        idTipo = idTipo,
                        idContador = BS_PAGAMENTOCOMPRA.Count() + 1
                    };
                    BS_PAGAMENTOCOMPRA.Add(objItem);
                }
                dtgCondicaoPagamento_DataBind();
                Contato_EsconderCaixa();
                Endereco_EsconderCaixa();

                ddlCondicaoCompra.SelectedValue = "0";

                var idsEmpresas = BS_PAGAMENTOCOMPRA.Where(x => x.sFuncao != "EXCLUIR_Pagamento").Select(x => x.idCondicaoPagamento).ToList();
                foreach (ListItem item in ddlCondicaoCompra.Items.Cast<ListItem>().ToList())
                {
                    if (!string.IsNullOrEmpty(item.Value) && idsEmpresas.Contains(Convert.ToInt32(item.Value)))
                    {
                        ddlCondicaoCompra.Items.Remove(item);
                    }
                }
            }
            else if (idTipo == 2)
            {
                if (ValidarCondicaoPagamentoCompra(ref sMensagem, idTipo))
                {
                    cls_CondicoesPagamento objItem = new cls_CondicoesPagamento
                    {
                        idParceiro = Convert.ToInt16(hddidParceiro.Value),
                        idCondicaoPagamento = Convert.ToInt16(ddlCondicaoPagamento.SelectedValue),
                        sDscCondicaoPagamento = ddlCondicaoPagamento.SelectedItem.ToString(),
                        sFuncao = "INCLUIR_PERMISSOES_PAGAMENTO",
                        idTipo = idTipo,
                        idContador = BS_CONDICAO_PAGAMENTO.Count() + 1
                    };
                    BS_CONDICAO_PAGAMENTO.Add(objItem);
                }
                gvCondicaoPagamento_DataBind();
                Contato_EsconderCaixa();
                Endereco_EsconderCaixa();

                ddlCondicaoPagamento.SelectedValue = "0";

                var idsEmpresas = BS_CONDICAO_PAGAMENTO.Where(x => x.sFuncao != "EXCLUIR_Pagamento").Select(x => x.idCondicaoPagamento).ToList();
                foreach (ListItem item in ddlCondicaoPagamento.Items.Cast<ListItem>().ToList())
                {
                    if (!string.IsNullOrEmpty(item.Value) && idsEmpresas.Contains(Convert.ToInt32(item.Value)))
                    {
                        ddlCondicaoPagamento.Items.Remove(item);
                    }
                }
            }
        }

        #region | Compra

        protected void lnkCondicaoCompra_Click(object sender, EventArgs e)
        {
            BaseMultiview_CondicaoCompra.Visible = !BaseMultiview_CondicaoCompra.Visible;
            BaseMultiview_CondicaoCompra.ActiveViewIndex = 0;
            updCondicaoCompra.Update();
            Contato_EsconderCaixa();
            Endereco_EsconderCaixa();
        }

        protected void cmdIncluirCondicaoCompra_Click(object sender, EventArgs e)
        {
            IncluirCondicaoPagamento(1);
        }

        void dtgCondicaoPagamento_DataBind()
        {
            gvCondicaoCompras.DataSource = BS_PAGAMENTOCOMPRA.Where(c => c.sFuncao.ToString() != "EXCLUIR_Pagamento").OrderBy(x => x.idContador);
            gvCondicaoCompras.DataBind();

            var idsEmpresas = BS_PAGAMENTOCOMPRA.Where(x => x.sFuncao != "EXCLUIR_Pagamento").Select(x => x.idCondicaoPagamento).ToList();
            foreach (ListItem item in ddlCondicaoCompra.Items.Cast<ListItem>().ToList())
            {
                if (!string.IsNullOrEmpty(item.Value) && idsEmpresas.Contains(Convert.ToInt32(item.Value)))
                {
                    ddlCondicaoCompra.Items.Remove(item);
                }
            }
        }

        protected void gvCondicaoCompras_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0, 1);
        }

        protected void gvCondicaoCompras_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(gvCondicaoCompras.Rows[e.RowIndex].Cells[0].Text);
            var nParcela = BS_PAGAMENTOCOMPRA[BS_PAGAMENTOCOMPRA.FindIndex(x => x.idContador.Equals(idLinha))].idCondicaoPagamento;
            BS_PAGAMENTOCOMPRA[BS_PAGAMENTOCOMPRA.FindIndex(x => x.idContador.Equals(idLinha))].sFuncao = "EXCLUIR_Pagamento";
            dtgCondicaoPagamento_DataBind();
        }

        #endregion

        #region | Venda

        protected void gvCondicaoPagamento_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(gvCondicaoPagamento.Rows[e.RowIndex].Cells[0].Text);
            var nParcela = BS_CONDICAO_PAGAMENTO[BS_CONDICAO_PAGAMENTO.FindIndex(x => x.idContador.Equals(idLinha))].idCondicaoPagamento;
            BS_CONDICAO_PAGAMENTO[BS_CONDICAO_PAGAMENTO.FindIndex(x => x.idContador.Equals(idLinha))].sFuncao = "EXCLUIR_Pagamento";
            gvCondicaoPagamento_DataBind();
        }

        void gvCondicaoPagamento_DataBind()
        {
            gvCondicaoPagamento.DataSource = BS_CONDICAO_PAGAMENTO.Where(c => c.sFuncao.ToString() != "EXCLUIR_Pagamento").OrderBy(x => x.idContador);
            gvCondicaoPagamento.DataBind();

            var idsEmpresas = BS_CONDICAO_PAGAMENTO.Where(x => x.sFuncao != "EXCLUIR_Pagamento").Select(x => x.idCondicaoPagamento).ToList();
            foreach (ListItem item in ddlCondicaoPagamento.Items.Cast<ListItem>().ToList())
            {
                if (!string.IsNullOrEmpty(item.Value) && idsEmpresas.Contains(Convert.ToInt32(item.Value)))
                {
                    ddlCondicaoPagamento.Items.Remove(item);
                }
            }
        }

        protected void cmdIncluirCondicaoPagamento_Click(object sender, EventArgs e)
        {
            IncluirCondicaoPagamento(2);
        }

        protected void gvCondicaoPagamento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0, 1);
        }

        #endregion

        #endregion

        #region | Familia

        protected void cmdIncluirFamilia_Click(object sender, EventArgs e)
        {
            string sMensagem = "";
            if (ValidarFamilia(ref sMensagem))
            {
                cls_WMS_Familias objItem = new cls_WMS_Familias
                {
                    idFamilia = Convert.ToInt16(ddlFamilias.SelectedValue),
                    sDscFamilia = ddlFamilias.SelectedItem.ToString(),
                    idContador = BS_WMS_FAMILIA.Count() + 1,
                    sFuncao = "INCLUIR_FAMILIA",
                    idParceiro = Convert.ToInt16(hddidParceiro.Value)
                };
                BS_WMS_FAMILIA.Add(objItem);
            }
            Familia_DataBind();
            Contato_EsconderCaixa();
            Endereco_EsconderCaixa();
        }

        private bool ValidarFamilia(ref string sMensagem)
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (ddlFamilias.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione a Familia!";
            }
            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPagina4.MostraMensagem_Erro(sMensagemErro);
            }
            return bRetorno;
        }

        protected void gvFamilias_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0, 1);
        }

        protected void gvFamilias_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(gvFamilias.Rows[e.RowIndex].Cells[0].Text);
            var nParcela = BS_WMS_FAMILIA[BS_WMS_FAMILIA.FindIndex(x => x.idContador.Equals(idLinha))].idFamilia;
            BS_WMS_FAMILIA[BS_WMS_FAMILIA.FindIndex(x => x.idContador.Equals(idLinha))].sFuncao = "EXCLUIR_FAMILIA";
            Familia_DataBind();
        }

        void Familia_DataBind()
        {
            gvFamilias.DataSource = BS_WMS_FAMILIA.Where(c => c.sFuncao.ToString() != "EXCLUIR_FAMILIA").OrderBy(x => x.idContador);
            gvFamilias.DataBind();

            var idsEmpresas = BS_WMS_FAMILIA.Where(x => x.sFuncao != "EXCLUIR_FAMILIA").Select(x => x.idFamilia).ToList();
            foreach (ListItem item in ddlFamilias.Items.Cast<ListItem>().ToList())
            {
                if (!string.IsNullOrEmpty(item.Value) && idsEmpresas.Contains(Convert.ToInt32(item.Value)))
                {
                    ddlFamilias.Items.Remove(item);
                }
            }
        }

        bool SalvarFamilia(string idParceiro, string idHistorico)
        {
            bool bRetorno = false;
            try
            {
                string idHistAtual = idHistorico;

                foreach (var linha in BS_WMS_FAMILIA)
                {
                    if (linha.sFuncao != "CONSULTAR")
                    {
                        Dictionary<string, string> vParam = new Dictionary<string, string> {
                            { "@sFuncao", linha.sFuncao },
                            { "@idParceiro", idParceiro },
                            { "@idHistorico", idHistAtual },
                            { "@idFamilia", linha.idFamilia.ToString() },
                            { "@idRegistro", linha.idRegistro.ToString() },
                            { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                        };

                        DataSet dsRet = BD.ExecutarDataSet(sProcedure, vParam);
                    }
                }
                bRetorno = true;
            }
            catch (Exception ex)
            {
                msgBancario.MostraMensagem_Erro("Erro Salvar Família: " + ex.Message);
            }
            return bRetorno;
        }

        void PopulaFamilia(string idParceiro, DataSet dsPesquisa)
        {
            bool bRetorno = true;
            try
            {
                foreach (DataRow row in dsPesquisa.Tables[4].Rows)
                {
                    if (row["idFamilia"].ToString() != "")
                    {
                        cls_WMS_Familias objItem = new cls_WMS_Familias
                        {
                            idRegistro = Convert.ToInt16(row["idRegistro"].ToString()),
                            idParceiro = Convert.ToInt16(row["idParceiro"].ToString()),
                            idFamilia = Convert.ToInt16(row["idFamilia"].ToString()),
                            sDscFamilia = row["sDscFamilia"].ToString(),
                            sFuncao = "CONSULTAR",
                            idContador = BS_WMS_FAMILIA.Count() + 1
                        };
                        BS_WMS_FAMILIA.Add(objItem);
                    }
                }
                Familia_DataBind();
            }
            catch
            {

            }
        }

        protected void lnkFamilias_Click(object sender, EventArgs e)
        {
            BaseMultiview_Familia.Visible = !BaseMultiview_Familia.Visible;
            BaseMultiview_Familia.ActiveViewIndex = 0;
            updFamilia.Update();
            Contato_EsconderCaixa();
            Endereco_EsconderCaixa();
        }

        #endregion

        #region | STSO

        protected void lnkAdicionar_Click(object sender, EventArgs e)
        {
            string sMensagem = "";
            if (ValidarSTSO(ref sMensagem))
            {
                Cls_STSO objItem = new Cls_STSO
                {
                    sEndereco = txtsEndereco.Text,
                    sUsuario = txtsUsuario.Text,
                    sSenha = txtsSenha.Text,
                    sObservacao = txtsObservacaoSTSO.Text,
                    idTipo = int.Parse(ddlidTipoEnvio.SelectedValue),
                    sDscTipo = ddlidTipoEnvio.SelectedItem.ToString(),
                    sFuncao = "INSERIR_STSO",
                    idContador = Base_STSO.Count() + 1
                };
                Base_STSO.Add(objItem);
                gvSTSO_DataBind();
                LimpaCampos_STSO();
            }
        }

        void gvSTSO_DataBind()
        {
            gvSTSO.DataSource = Base_STSO.Where(c => c.sFuncao.ToString() != "EXCLUIR_STSO").OrderBy(x => x.idContador);
            gvSTSO.DataBind();
        }

        private bool ValidarSTSO(ref string sMensagem)
        {
            bool bRetorno = true;
            string sMensagemErro = "";
            if (txtsEndereco.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Endereço Incorreto!" + "</br>";
            }

            if (txtsUsuario.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Usuário Incorreto!" + "</br>";
            }

            if (txtsSenha.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Senha Incorreto!" + "</br>";
            }

            if (ddlidTipoEnvio.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Tipo de Envio!" + "</br>";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina5.MostraMensagem_Erro(sMensagemErro);
            }
            return bRetorno;
        }

        void LimpaCampos_STSO()
        {
            txtsEndereco.Text = "";
            txtsUsuario.Text = "";
            txtsSenha.Text = "";
            //txtsObservacao.Text = "";
            txtsSenha.Attributes["value"] = "";
            ddlidTipoEnvio.SelectedValue = "0";
        }

        protected void gvSTSO_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0, 1);
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                var grid = (GridView)sender;
                foreach (GridViewRow row in grid.Rows)
                {
                    if ((row.Cells[4].FindControl("lblsSenha") as Label).Text != "Senha" && (row.Cells[4].FindControl("lblsSenha") as Label).Text != "&nbsp;")
                    {
                        string senha = (row.Cells[4].FindControl("lblsSenha") as Label).Text;
                        (row.Cells[4].FindControl("lblsSenha") as Label).Text = new string('*', senha.Length);
                    }
                }
            }
        }

        void Popular_STSO(DataSet dsPesquisa)
        {
            Base_STSO.Clear();
            foreach (DataRow row in dsPesquisa.Tables[6].Rows)
            {
                Cls_STSO objItem = new Cls_STSO
                {
                    idUsuario = int.Parse(row["idUsuario"].ToString()),
                    sEndereco = (row["sEndereco"].ToString()),
                    sUsuario = (row["sUsuario"].ToString()),
                    sSenha = (row["sSenha"].ToString()),
                    sObservacao = (row["sObservacao"].ToString()),
                    idTipo = int.Parse(row["idTipo"].ToString()),
                    sDscTipo = (row["sDscTipo"].ToString()),
                    idContador = Base_STSO.Count() + 1,
                    sFuncao = "SEM ALTERAÇÃO"
                };
                Base_STSO.Add(objItem);
            }
            gvSTSO_DataBind();
            LimpaCampos_STSO();
        }

        bool Salvar_STSO(string idEmpresa, string idHistorico)
        {
            bool bRetorno = true;
            try
            {
                foreach (var Linha in Base_STSO)
                {
                    if (Linha.sFuncao != "SEM ALTERAÇÃO")
                    {
                        Dictionary<String, String> vParametro = new Dictionary<string, string>
                        {
                            ["@sFuncao"] = Linha.sFuncao,
                            ["@idParceiro"] = idEmpresa,
                            ["@idHistorico"] = idHistorico,
                            ["@idUsuarioSTSO"] = Linha.idUsuario.ToString(),
                            ["@sUsuario"] = Linha.sUsuario.ToString(),
                            ["@sSenha"] = Linha.sSenha.ToString(),
                            ["@idTipo"] = Linha.idTipo.ToString(),
                            ["@sObservacaoSTSO"] = Linha.sObservacao.ToString(),
                            ["@sEnderecoSTSO"] = Linha.sEndereco.ToString(),
                            ["@idUsuarioAtualizacao"] = IDENTITY.Variaveis.idUsuario()
                        };

                        BD.ExecutarDataSet(sProcedure, vParametro);
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
                bRetorno = false;
            }
            return bRetorno;
        }

        protected void gvSTSO_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idContador = Convert.ToInt32(gvSTSO.Rows[e.RowIndex].Cells[0].Text);
            var nParcela = Base_STSO[Base_STSO.FindIndex(x => x.idContador.Equals(idContador))].idUsuario;
            Base_STSO[Base_STSO.FindIndex(x => x.idContador.Equals(idContador))].sFuncao = "EXCLUIR_STSO";
            gvSTSO_DataBind();
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            string sSenha = txtsSenha.Text;
            txtsSenha.TextMode = TextBoxMode.SingleLine;
            LinkButton1.Visible = false;
            LinkButton3.Visible = true;
            txtsSenha.Attributes["value"] = sSenha;
        }

        protected void LinkButton2_Click(object sender, EventArgs e)
        {
            string sSenha = txtsSenha.Text;
            txtsSenha.TextMode = TextBoxMode.Password;
            LinkButton1.Visible = true;
            LinkButton3.Visible = false;
            txtsSenha.Attributes["value"] = sSenha;
        }

        protected void TodoslnkVisualizarSenha_Click(object sender, EventArgs e)
        {
            LinkButton lnkVisualizarSenha = (LinkButton)sender;
            if (lnkVisualizarSenha.CssClass == "fa fa-eye")
            {
                lnkVisualizarSenha.CssClass = "fa fa-eye-slash";
            }
            else
            {
                lnkVisualizarSenha.CssClass = "fa fa-eye";
            }

            foreach (GridViewRow row in gvSTSO.Rows)
            {
                foreach (var Linha in Base_STSO)
                {
                    if (row.Cells[0].Text == Linha.idContador.ToString())
                    {
                        string senha = (row.Cells[4].FindControl("lblsSenha") as Label).Text;
                        if ((row.Cells[4].FindControl("lblsSenha") as Label).Text == new string('*', senha.Length))
                        {
                            (row.Cells[4].FindControl("lblsSenha") as Label).Text = Linha.sSenha;
                        }
                        else
                        {
                            (row.Cells[4].FindControl("lblsSenha") as Label).Text = new string('*', senha.Length);
                        }
                    }
                }
            }
        }

        #endregion

        #region | Financeiro

        private void PopulaAbaFinanceiro()
        {

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_FINANCEIRO" },
                { "@idCliente", hddidParceiro.Value },
                { "@sStatus", ddlFinanceiroStatus.SelectedValue }
            };
            DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsPesquisa, out string sErro))
            {

                // Popula Aba Dados Financeiros
                ddlidTipoSituacaoCliente.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idTipoSituacaoCliente");               
                txtnLimiteCredito.Text = RETORNO.DATASET(dsPesquisa, 0, "nLimiteCredito") != ""? RETORNO.DATASET(dsPesquisa, 0, "nLimiteCredito") : "0,00";
                txtnCreditoDisponivel.Text = RETORNO.DATASET(dsPesquisa, 0, "nCreditoDisponivel");

                dtgvDadosFinanceiros.DataSource = dsPesquisa.Tables[1];
                dtgvDadosFinanceiros.DataBind();                

                // Popula Aba Gráficos
                litDataAtualizacao.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                rptCardsMediaCompraMensal.DataSource = dsPesquisa.Tables[3];
                rptCardsMediaCompraMensal.DataBind();

                rptCardsPagamentoTitulos.DataSource = dsPesquisa.Tables[4];
                rptCardsPagamentoTitulos.DataBind();

                rptMediaTitulosAberto.DataSource = dsPesquisa.Tables[5];
                rptMediaTitulosAberto.DataBind();


                // Popula Aba Títulos
                dtgvFinanceiroTitulo.DataSource = dsPesquisa.Tables[2];
                dtgvFinanceiroTitulo.DataBind();

                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvFinanceiroTitulo, dsPesquisa.Tables[2], new int[3] { 1, 2, 12 }, "desc", "false", "''"), true);
                GRID.SomarColunas(dtgvFinanceiroTitulo, true, GRID.Formatação.Moeda, 6, 7, 8, 9);
            }
            else
            {
                MensagemPaginaDadosFinanceiros.MostraMensagem_Aviso("Não foi encontrado títulos desse parceiro");
            }
        }

        private void SalvaDadosFinanceiros()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR_FINANCEIRO" },
                { "@idCliente", hddidParceiro.Value },
                { "@idTipoSituacaoCliente", ddlidTipoSituacaoCliente.SelectedValue },
                { "@nLimiteCredito", txtnLimiteCredito.Text != ""? txtnLimiteCredito.Text.Replace(".","").Replace(",",".") : "0" }
            };
            DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsPesquisa, out string sErro))
            {
                MensagemPaginaDadosFinanceiros.MostraMensagem_Sucesso("Registro Aba Financeiro Salvos com Sucesso!");
            }
        }

        protected string GetCardClass(string categoria)
        {
            switch (categoria)
            {
                case "mediaCompraMensal": return "cardVerde";
                case "Pontual": return "cardVerde";
                case "Atraso_1_5": return "cardAmareloClaro";
                case "Atraso_6_10": return "cardAmareloEscuro";
                case "Atraso_10_30": return "cardLaranjaClaro";
                case "Atraso_Acima_30": return "cardLaranjaEscuro";
                case "Vencidos": return "cardVermelho";
                case "Vencendo_Hoje": return "cardLaranjaEscuro";
                case "Ate_7_Dias": return "cardLaranjaClaro";
                case "Ate_15_Dias": return "cardAmareloEscuro";
                case "Ate_30_Dias": return "cardAmareloClaro";
                case "Mais_30_Dias": return "cardVerde";
                default: return "";
            }
        }

        protected string GetCardHeaderText(string categoria)
        {
            switch (categoria)
            {
                case "mediaCompraMensal": return "Títulos";
                case "Pontual": return "Pontual";
                case "Atraso_1_5": return "Atraso 1 a 5 dias";
                case "Atraso_6_10": return "Atraso 6 a 10 dias";
                case "Atraso_10_30": return "Atraso 10 a 30 dias";
                case "Atraso_Acima_30": return "Atraso acima de 30 dias";
                case "Vencidos": return "Vencidos";
                case "Vencendo_Hoje": return "Vencendo Hoje";
                case "Ate_7_Dias": return "Até 7 dias";
                case "Ate_15_Dias": return "Até 15 dias";
                case "Ate_30_Dias": return "Até 30 dias";
                case "Mais_30_Dias": return "Mais de 30 dias";
                default: return categoria;
            }
        }

        protected string GetCardUrl(string categoria)
        {
            string url = string.Format("/App/Paginas/Adm/Financeiro/ContasReceber.aspx?sFiltro={0}&idParceiro={1}", categoria, hddidParceiro.Value);
            return url;            
        }

        protected void dtgvFinanceiroTitulo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                var sStatus = DataBinder.Eval(e.Row.DataItem, "sStatus").ToString();
                if (sStatus == "Em Atraso")
                {
                    e.Row.CssClass = "danger";
                }
                else if (sStatus == "Liquidado")
                {
                    e.Row.CssClass = "success";
                }
                else
                {
                    e.Row.CssClass = "info";
                }

                int id = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "idContasReceber"));

                HyperLink linksDocumento = (HyperLink)e.Row.FindControl("hlsDocumento");
                HyperLink linksCodigo = (HyperLink)e.Row.FindControl("hlsCodigo");

                if (linksDocumento != null && linksCodigo != null)
                {
                    linksDocumento.NavigateUrl = "/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id=" + id;
                    linksCodigo.NavigateUrl = "/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id=" + id;
                }
            }
        }

        protected void btnBuscaFinanceiro_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_TITULOS_FINANCEIRO" },
                { "@idCliente", hddidParceiro.Value },
                { "@sFiltroData", ddlFinanceiroFiltroData.SelectedValue },
                { "@dtInicial", txtdtInicial.Text },
                { "@dtFinal", txtdtFinal.Text },
                { "@sStatus", ddlFinanceiroStatus.SelectedValue }
            };
            DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsPesquisa, out string sErro))
            {
                dtgvFinanceiroTitulo.DataSource = dsPesquisa.Tables[0];
                dtgvFinanceiroTitulo.DataBind();

                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvFinanceiroTitulo, dsPesquisa.Tables[0], 1, "desc", "false", "''"), true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "mostraAbaTitulo", "$('#titulos-tab').tab('show');", true);
            }
        }


        [System.Web.Services.WebMethod]
        public static void SalvaEstadoCadeado (bool isLocked, string idParceiro)
        {
            try
            {
                DataTable dt;
                Dictionary<String, String> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "Salvar_Cadeado" },
                    { "@sCadeadoFinanceiro", isLocked == true ? "N" : "S" },
                    { "@idCliente", idParceiro },
                    { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                };
                dt = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Clientes", vParametros);                

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region | Historico

        void PopularHistorico()
        {
            try
            {
                string sErro = "";
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR-HISTORICO" },
                    { "@idParceiro", hddidParceiro.Value }
                };

                DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(ds, out sErro))
                {
                    gvHistorico.DataSource = ds.Tables[0];
                    gvHistorico.DataBind();
                    updHistorico.Update();
                }
                else if (sErro != "")
                {
                    msgHistorico.MostraMensagem_Erro(sErro);
                }
            }
            catch (Exception ex)
            {
                msgHistorico.MostraMensagem_Erro("Erro ao carregar histórico: " + ex.Message);
            }
        }

        protected void gvHistorico_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string idHistorico = gvHistorico.DataKeys[e.Row.RowIndex].Value.ToString();
                GridView gvItens = (GridView)e.Row.FindControl("gvHistoricoItens");
                LinkButton btnToggle = (LinkButton)e.Row.FindControl("btnToggle");

                // Por enquanto, como estamos só no cabeçalho, a grid de itens estará vazia.
                // Mas já vamos deixar a lógica pronta:
                Dictionary<string, string> vParam = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR-DETALHES-HISTORICO" }, // Criaremos essa função na Proc logo mais
                    { "@idHistorico", idHistorico }
                };

                DataSet ds = BD.ExecutarDataSet(sProcedure, vParam);

                if (BD.ValidarDataSet(ds))
                {
                    gvItens.DataSource = ds.Tables[0];
                    gvItens.DataBind();
                    btnToggle.Visible = true;
                }
                else
                {
                    btnToggle.Visible = false; // Se não tem detalhe de item, esconde o botão de expandir
                }
            }
        }

        public string NovaLinha(object id, string gridNome)
        {
            if (id != null && !string.IsNullOrEmpty(id.ToString()))
            {
                return string.Format(@"</td></tr><tr id='tr{0}{1}' class='collapsed-row'>
                                <td></td><td colspan='100' style='padding:0px; margin:0px;'>", gridNome, id);
            }
            return string.Empty;
        }

        #endregion

    }
}
