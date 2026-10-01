using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using System.Web.UI.WebControls;
using TT.FrameWork;
using System.Globalization;
using TT_Flow.FrameWork;
using System.Linq;
using Microsoft.Reporting.WebForms;
using System.IO;
using static TT.FrameWork.Identity;
using System.Data.SqlClient;
using System.Web.UI.HtmlControls;
using static TT_Flow.App.Controles.Pesquisa_Parceiros;
using TT_Flow.App.Controles;
using static TT.FrameWork.BD;
//using Microsoft.IdentityModel.Tokens;
using static Permissao;
using System.Runtime.CompilerServices;

namespace TT_Flow.App.Paginas.Compras
{
    public partial class CotacaoCompras_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Detalhes da Cotação";
        string sProcedure = "sp_Manipula_tbl_Flow_Comercial_CotacaoCompras";

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();
            Pesquisa_Parceiros.SsTipoParceiro = "1";

            Pesquisa_Parceiros.ParceiroAlterado += Pesquisa_Parceiros_ParceiroAlterado;
            if (!IsPostBack)
            {
                if (!FUNCOES.ValidaPermissao(Permissao.Compras.CotacaoCompras.Alterar))
                {
                    cmdCotar.Visible = false;
                    cmdAdcionarFornecedor.Visible = false;
                    cmdIncluirItem.Visible = false;
                    cmdRegistrarFornecedor.Visible = false;
                }
                if (Request["id"] != null)
                {
                    Pesquisar(Request["id"].ToString(), false);
                }
                else
                {
                    Pesquisar("0", true);
                    if (FUNCOES.ValidaPermissao(Permissao.Compras.CotacaoCompras.Incluir))
                    {
                        cmdCotar.Visible = true;
                        cmdAdcionarFornecedor.Visible = true;
                        cmdIncluirItem.Visible = true;
                        cmdRegistrarFornecedor.Visible = true;
                    }
                }
                if (Session["MensagemSucesso"] != null)
                {
                    string mensagem = Session["MensagemSucesso"].ToString();
                    MensagemPagina.MostraMensagem_Sucesso(mensagem);

                    Session["MensagemSucesso"] = null;
                }
                if (Session["MensagemEmail"] != null)
                {
                    string mensagem = Session["MensagemEmail"].ToString();
                    MensagemPagina.MostraMensagem("<b>Informação </b>" + mensagem, "info", false);

                    Session["MensagemEmail"] = null;
                }
                if (Session["MensagemEmailErro"] != null)
                {
                    string mensagem = Session["MensagemEmailErro"].ToString();
                    MensagemPagina.MostraMensagem("<b>Erro: </b>" + mensagem, "danger", false);

                    Session["MensagemEmailErro"] = null;
                }

                // LÓGICA MODIFICADA: Define o estado inicial das novas "abas"
                panelCardsComparativo.Visible = true;
                divComparador.Visible = false;
                cmdMostrarCards.CssClass = "btn btn-primary active";
                cmdComparar.CssClass = "btn btn-default";
            }
            else
            {
                Pesquisa_Parceiros controle = (Pesquisa_Parceiros)this.Page.FindControl("Pesquisa_Parceiros");
                if (controle != null)
                {
                    controle.MeuEvento += new MeuDelegate(VerificaValoresContato);
                }
                else
                {
                    var a = "Algo tinha que acontecer";
                }
                var requestTarget = this.Request["__EVENTTARGET"];
                var requestArgs = this.Request["__EVENTARGUMENT"];

                if (requestTarget == "funcao_SAIR")
                {
                    FUNCOES.DirecionaPagina("/app/dashboard.aspx");
                }
                else if (requestTarget == "funcao_Editar")
                {
                    Pesquisar(hddidCotacao.Value, true);
                }
                else if (Request["id"] != null)
                {
                    txtidCotacao.Text = Request["id"];
                }

                SetDateToTextBox(txtDtCotacao, DateTime.Now);
            }
            txtsPedidoCompra.ReadOnly = true;
            ExcelImportar.ID_FileUpload = ImportarArquivo.ID;

            RegistraScript("");
        }
        #endregion

        #region | Classes
        public EntidadeFuncoes<cls_CotacaoComprasItens> conversorProdutos = new EntidadeFuncoes<cls_CotacaoComprasItens>();

        public List<FrameWork.cls_CotacaoComprasItens> ls_CotacaoItens
        {
            get
            {
                if (ViewState["ls_CotacaoItens"] == null)
                {
                    ViewState["ls_CotacaoItens"] = new List<FrameWork.cls_CotacaoComprasItens>();
                }
                return (List<FrameWork.cls_CotacaoComprasItens>)ViewState["ls_CotacaoItens"];
            }
            set
            {
                ViewState["ls_CotacaoItens"] = value;
            }
        }

        public List<FrameWork.cls_CotacaoComprasItens> ls_CotacaoItensFornecedor
        {
            get
            {
                if (ViewState["ls_CotacaoItensFornecedor"] == null)
                {
                    ViewState["ls_CotacaoItensFornecedor"] = new List<FrameWork.cls_CotacaoComprasItens>();
                }
                return (List<FrameWork.cls_CotacaoComprasItens>)ViewState["ls_CotacaoItensFornecedor"];
            }
            set
            {
                ViewState["ls_CotacaoItensFornecedor"] = value;
            }
        }

        public List<TT_Flow.FrameWork.cls_Fluxo> Base_Fluxo
        {
            get
            {
                if (ViewState["Base_Fluxo"] == null)
                {
                    ViewState["Base_Fluxo"] = new List<FrameWork.cls_Fluxo>();
                }
                return (List<FrameWork.cls_Fluxo>)ViewState["Base_Fluxo"];
            }

            set
            {
                ViewState["Base_Fluxo"] = value;
            }
        }
        #endregion

        #region |Metodos Banco de Dados
        protected void Pesquisar(string idCotacao, bool bEdicao)
        {
            string sErro = "";

            try
            {
                PopularCombos();
                if (idCotacao != "0")
                {
                    div_btnFornecedor.Visible = true;

                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idCotacao", idCotacao);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidCotacao.Value = RETORNO.DATASET(dsPesquisa, 0, "idCotacao");
                        txtidCotacao.Text = hddidCotacao.Value;
                        hddidRequisicao.Value = RETORNO.DATASET(dsPesquisa, 0, "idRequisicao");
                        ddlComprador.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idUsuarioComprador");
                        ddlidEmpresa.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idEmpresa");

                        ddlConceito.SelectedValue = RETORNO.DATASET(dsPesquisa, "idConceito");

                        var idGrupoPatrimonio = RETORNO.DATASET(dsPesquisa, "idGrupoPatrimonio");

                        if (idGrupoPatrimonio != "0" && !string.IsNullOrEmpty(idGrupoPatrimonio))
                        {
                            FUNCOES.Popula_Combo(ddlGrupoPatrimonio, "sp_Select 'Flow_Patrimonio_Grupo'", "idPatrimonioGrupo", "sDscPatrimonio", false, "Selecione um Grupo de Patrimônio.", "0");
                            ddlGrupoPatrimonio.SelectedValue = idGrupoPatrimonio;
                            divGrupoPatrimonio.Visible = true;
                        }
                        else
                            divGrupoPatrimonio.Visible = false;

                        dtgItens.Columns[0].Visible = true;

                        //Endereço
                        FUNCOES.Popula_Combo(ddlsEnderecoEntrega, string.Format("sp_Manipula_tbl_Flow_Clientes 'CONSULTAR_ENDERECO_COMPRAS', {0}", ddlidEmpresa.SelectedValue), "idEndereco", "sEnderecoCompleto", false);
                        ddlsEnderecoEntrega.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idEndereco");

                        if (!string.IsNullOrEmpty(RETORNO.DATASET(dsPesquisa, 0, "sObservacao")))
                        {
                            Div_Observacao.Visible = true;
                            txtsObservacao.Text = RETORNO.DATASET(dsPesquisa, 0, "sObservacao");
                            hddsObservacao.Value = txtsObservacao.Text;
                        }
                        else
                        {
                            Div_Observacao.Visible = false;
                        }
                        if (RETORNO.DATASET(dsPesquisa, 0, "sCadastroCompleto") == "Cadastro Completo")
                        {
                            cmdAdcionarFornecedor.Visible = true;
                        }
                        else
                        {
                            cmdAdcionarFornecedor.Visible = false;
                            MensagemPagina.MostraMensagem("<b>Lembrete</b> Complete o Cadastro para continuar a cotação", "info", true);
                        }
                        Popular_dtgItens(hddidCotacao.Value);
                        PopularRepeaterCapaFornecedores(hddidCotacao.Value);
                        txtsDscCotacao.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscCotacao");

                        if (RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao").ToString() == "01/01/1900 00:00:00")
                        {
                            txtDtCotacao.Text = null;
                        }
                        else
                        {
                            var dt = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao").ToString());
                            txtDtCotacao.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');
                        }
                        if (RETORNO.DATASET(dsPesquisa, 0, "dtValidade").ToString() == "01/01/1900 00:00:00")
                        {
                            txtdtValidade.Text = null;
                        }
                        else
                        {
                            var dt = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtValidade").ToString());
                            txtdtValidade.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');
                        }
                        txtnValor.Text = RETORNO.DATASET(dsPesquisa, 0, "nValorCotacao");

                        PainelAtualizacao.Visible = true;
                        Div_ItensCotacao.Visible = true;

                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario"));

                        lblTituloPagina.Text = string.Format("Cotação {0}", RETORNO.DATASET(dsPesquisa, 0, "sDscCotação"));
                        BreadCrumb.TitulodaPagina = "Detalhes da Cotação";
                    }
                    else
                    {
                        if (sErro != "")
                        {
                            MensagemPagina.MostraMensagem_Erro(sErro);
                        }
                    }

                }
                else
                {
                    Div_Observacao.Visible = false;
                    hddidCotacao.Value = "0";
                    DivAddProdutos.Visible = true;
                    div_btnFornecedor.Visible = false;
                    div_Comparativo.Visible = false;
                    SetDateToTextBox(txtDtCotacao, DateTime.Now);
                    PainelAtualizacao.Visible = false;
                    dtgItens.Columns[0].Visible = false;
                }
                RegistraScript("");

            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }
        public void SetDateToTextBox(TextBox textBox, DateTime date)
        {
            textBox.Text = date.ToString("yyyy-MM-dd");
        }
        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlsUnidade, "sp_Select 'Flow_Produtos_Unidade'", "sUnidade", "sDscUnidade", false, "Sel. Unidade ", "0");
            FUNCOES.Popula_Combo(ddlComprador, "sp_Select 'tbl_Compradores'", "idUsuario", "sDscUsuario", false, "Selecione o Comprador", "0");
            FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa', @sTipo = S, @idUsuario=" + IDENTITY.Variaveis.idUsuario(), "idEmpresa", "sDscEmpresaReduzida", false, "Selecione a Empresa", "0");
            FUNCOES.Popula_Combo(ddlsEnderecoEntrega, string.Format("sp_Manipula_tbl_Flow_Clientes 'CONSULTAR_ENDERECO_COMPRAS', {0}", ddlidEmpresa.SelectedValue), "idEndereco", "sEnderecoCompleto", false);
            FUNCOES.Popula_Combo(ddlTipoEnvio, "sp_Select 'Flow_Pedidos_TipoEnvio'," + 7, "idTipoEnvio", "sDscTipoEnvio", false, "Selecione o Tipo de Envio ", "0");
            FUNCOES.Popula_Combo(ddlCondPagamento, "sp_Select 'FLOW_CondicaoDePagamento'", "idCondicaoPagamento", "sDscCondicaoPagamento", false, "Selecione a Condição de Pagamento", "0");
            PopularComboFluxo();

            string tipoCompra = string.IsNullOrEmpty(ddlsTipoCompra.SelectedValue) || ddlsTipoCompra.SelectedValue == "0" ? "N" : ddlsTipoCompra.SelectedValue;

            FUNCOES.Popula_Combo(ddlFornecedor, "sp_Select 'Flow_Parceiros_Fornecedores', @sTipo =" + tipoCompra, "idCliente", "Razao_CNPJ", false, "Selecione o Fornecedor", "0");

            FUNCOES.Popula_Combo(ddlCentroCusto, "sp_Manipula_tbl_Flow_Pedidos 'FLOW-CENTRO-CUSTOS'", "idCentroDeCusto", "sDescricao", false, "Selecione o Centro de Custo", "0");
            FUNCOES.Popula_Combo(ddlConceito, "sp_Manipula_tbl_Flow_Pedidos 'FLOW-CONCEITO'", "idConceito", "sDscConceito", false, "Nenhum Conceito", "0");
        }
        void PopularCondPagamento(string idParceiro)
        {
            if (int.Parse(hddidCotacao.Value) >= 94 || int.Parse(hddidCotacao.Value) == 0)
                FUNCOES.Popula_Combo(ddlCondPagamento, "sp_Select 'FLOW_CondicaoDePagamento', @idFiltro= 1, @idPesquisa=" + idParceiro, "idCondicaoPagamento", "sDscCondicaoPagamento", false, "Selecione a Condição de Pagamento", "0");
            else
            {
                FUNCOES.Popula_Combo(ddlCondPagamento, "sp_Select 'FLOW_CondicaoDePagamento', @idPesquisa=" + idParceiro, "idCondicaoPagamento", "sDscCondicaoPagamento", false, "Selecione a Condição de Pagamento", "0");
            }
            ;

            if (ddlCondPagamento.Items.Count == 0)
            {
                FUNCOES.Popula_Combo(ddlCondPagamento, "sp_Select 'FLOW_CondicaoDePagamento'", "idCondicaoPagamento", "sDscCondicaoPagamento", false, "Selecione a Condição de Pagamento", "0");
            }
            ;
        }
        void SalvarCotacao(string idCotacao)
        {
            if (ValidarDadosCotacao())
            {
                try
                {
                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    string sErro = "";

                    //Cadastro
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idCotacao", idCotacao);
                    vParametros.Add("@sDscCotacao", txtsDscCotacao.Text);
                    vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());

                    vParametros.Add("@idComprador", ddlComprador.SelectedValue);
                    vParametros.Add("@idEmpresa", ddlidEmpresa.SelectedValue);
                    vParametros.Add("@idEndereco", ddlsEnderecoEntrega.SelectedValue);
                    vParametros.Add("@idConceito", ddlConceito.SelectedValue);
                    vParametros.Add("@idGrupoPatrimonio", ddlGrupoPatrimonio.SelectedValue);

                    if (txtdtValidade.Text == "")
                    {
                        vParametros.Add("@dtValidade", txtdtValidade.Text);
                    }
                    else
                    {
                        DateTime dt = DateTime.Parse(txtdtValidade.Text.ToString(), CultureInfo.InvariantCulture);
                        vParametros.Add("@dtValidade", dt.ToString());
                    }
                    if (ls_CotacaoItens.Count() != 0)
                    {
                        dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                        if (BD.ValidarDataSet(dsSalvar, out sErro))
                        {
                            idCotacao = RETORNO.DATASET(dsSalvar, "idCotacao");
                            Salvar_Itens(idCotacao);
                            hddidCotacao.Value = idCotacao;
                            Pesquisar(idCotacao, false);
                            MensagemPagina.MostraMensagem_Sucesso("Cotação Salva com sucesso!");
                        }
                        else
                        {
                            MensagemPagina.MostraMensagem_Erro("BD: " + sErro.ToString(), false);
                        }
                    }
                    else
                    {
                        MensagemPagina.MostraMensagem_Aviso("<b>Aviso</b> É Necessário Adicionar Itens Para a Cotação.", false);
                    }
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro($"Erro: {ex}", false);
                }
            }
            RepopularCampos();
        }
        protected void ValidarCamposPreenchidos()
        {
            if (string.IsNullOrWhiteSpace(txtDtCotacao.Text))
            {
                txtDtCotacao.CssClass += " campo-vazio";
            }

            if (string.IsNullOrWhiteSpace(txtdtValidade.Text))
            {
                txtdtValidade.CssClass += " campo-vazio";
            }

            if (ddlComprador.SelectedIndex == 0)
            {
                ddlComprador.CssClass += " campo-vazio";
            }

            if (ddlidEmpresa.SelectedIndex == 0)
            {
                ddlidEmpresa.CssClass += " campo-vazio";
            }

            if (ddlsEnderecoEntrega.SelectedIndex == 0)
            {
                ddlsEnderecoEntrega.CssClass += " campo-vazio";
            }

            if (ddlConceito.SelectedValue != "0")
            {
                if (ddlGrupoPatrimonio.SelectedValue == "0" && Valida_Conceito())
                    ddlGrupoPatrimonio.CssClass += " campo-vazio";
            }

            if (string.IsNullOrWhiteSpace(txtsDscCotacao.Text))
            {
                txtsDscCotacao.CssClass += " campo-vazio";
            }

            if (string.IsNullOrWhiteSpace(txtsObservacao.Text))
            {
                txtsObservacao.CssClass += " campo-vazio";
            }
        }
        void RepopularCampos()
        {
            if (!string.IsNullOrEmpty(hddsObservacao.Value))
                txtsObservacao.Text = hddsObservacao.Value;

            if (!string.IsNullOrEmpty(hddsObservacao.Value) && hddidCotacao.Value != "0")
                txtidCotacao.Text = hddidCotacao.Value;
        }
        #endregion

        #region | Metodos de Front
        #region  | Validações Front
        bool ValidarDadosCotacao()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (string.IsNullOrEmpty(txtsDscCotacao.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe uma Descrição para a Cotação";
            }
            if (string.IsNullOrEmpty(txtdtValidade.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe uma Data de Validade da Cotação";
            }
            else
            {
                DateTime dtValidade;
                if (!DateTime.TryParse(txtdtValidade.Text, out dtValidade))
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data de Validade da Cotação é inválida";
                }
                else if (dtValidade < DateTime.Today)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "A Data de Validade da Cotação não pode ser menor que a data atual";
                }
            }
            if (ddlComprador.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um Usuário Comprador";
            }
            if (ddlsEnderecoEntrega.SelectedValue == "-1" || ddlsEnderecoEntrega.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o Endereço";
            }
            if (ddlConceito.SelectedValue != "0")
            {
                if (ddlGrupoPatrimonio.SelectedValue == "0" && Valida_Conceito())
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Grupo de Patrimônio!";
            }
            if (ddlidEmpresa.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe uma Empresa";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina.MostraMensagem_Erro(sMensagemErro, false);
            }

            return bRetorno;
        }

        bool ValidarDadosCotacao(bool isValidPedido)
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (string.IsNullOrEmpty(txtsDscCotacao.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe uma Descrição para a Cotação";
            }
            if (string.IsNullOrEmpty(txtdtValidade.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe uma Data de Validade da Cotação";
            }
            if (ddlComprador.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um Usuário Comprador";
            }
            if (ddlsEnderecoEntrega.SelectedValue == "-1" || ddlsEnderecoEntrega.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o Endereço";
            }

            if (ddlConceito.SelectedValue != "0")
            {
                if (ddlGrupoPatrimonio.SelectedValue == "0" & Valida_Conceito())
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Grupo de Patrimônio!";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPaginaComparativo.MostraMensagem_Erro(sMensagemErro, false);
            }

            return bRetorno;
        }
        bool ValidarDadosItens()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (string.IsNullOrEmpty(txtsCodigoProduto.Text.Trim()))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um Código de Produto.";
            }

            if (string.IsNullOrEmpty(txtsDscProduto.Text.Trim()))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe uma Descrição para o Produto.";
            }

            int quantidade;
            if (!int.TryParse(txtnQuantidade.Text.Trim(), out quantidade) || quantidade <= 0)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe uma Quantidade válida (número maior que 0).";
            }

            if (string.IsNullOrEmpty(ddlsUnidade.SelectedValue) || ddlsUnidade.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Unidade válida.";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPaginaItens.MostraMensagem_Erro(sMensagemErro, false);
            }

            return bRetorno;
        }
        #endregion
        protected void Salvar_Click(object sender, EventArgs e)
        {
            SalvarCotacao(hddidCotacao.Value);
        }
        protected void Voltar_Click(object sender, EventArgs e)
        {
            Response.Redirect($"/App/Paginas/Compras/CotacaoCompras.aspx");
        }
        #endregion

        #region | Arquivos
        #endregion

        #region | Script 
        void RegistraScript()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("$('[id*=txtNPesoLiquido]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtNPesoBruto]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtNComprimento]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtNLargura]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtNAltura]').mask('000.000.000.000.000,00', { reverse: true });");

            sb.Append("});");


            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);

        }

        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();


            sb.Append("$v192(function() {");
            sb.Append("$v192(\"[id$=txtsDscProduto]\").autocomplete({");
            sb.Append("source: function(request, response) {");
            sb.Append("$v192.ajax({");

            sb.Append("url: '/app/Paginas/Requisicao/Requisicao_Detalhe.aspx/GetProdutos',");
            sb.Append("data: \"{ 'sDscProduto': '\" + request.term + \"'}\",");
            sb.Append("dataType: \"json\",");

            sb.Append("type: \"POST\",");
            sb.Append("contentType: \"application/json; charset=utf-8\",");

            sb.Append("success: function(data) {");
            sb.Append("response($v192.map(data.d, function(item) {");
            sb.Append("return {");

            sb.Append("label: item.split('|')[0],");
            sb.Append("val: item.split('|')[2],");
            sb.Append("id: item.split('|')[1],");
            sb.Append("un: item.split('|')[3]");
            sb.Append("}");
            sb.Append("}))");
            sb.Append("},");
            sb.Append("error: function(response) {");
            sb.Append("alert(response.responseText);");
            sb.Append("},");

            sb.Append("failure: function(response) {");

            sb.Append("alert(response.responseText);");
            sb.Append("}");
            sb.Append("});");
            sb.Append("},");

            sb.Append("select: function(e, i) {");
            sb.Append("$(\"[id$=txtsCodigoProduto]\").val(i.item.val);");
            sb.Append("$(\"[id$=ddlsUnidade]\").val(i.item.un);");
            sb.Append("$('[id$=txtnQuantidade]').focus();");

            sb.Append("},");


            sb.Append("minLength: 3");
            sb.Append("});});");


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

            sb.Append("$v192(\"#dialog-Editar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_Editar\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=cmdEditar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Editar').dialog('open');");
            sb.Append("});");

            sb.Append("});");
            //Caixa de seleção de datas
            sb.Append("$(function() {$('[id*=txtdtRequisicao]').datepicker({");
            sb.Append("autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'});});");

            sb.Append("$(function() {$('[id*=txtdtPre]').datepicker({");
            sb.Append("autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'});});");



            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);
        }

        protected void txtsCodigoProduto_TextChanged(object sender, EventArgs e)
        {
            SqlDataReader sdr = BD.ExecutarDataReader("sp_Select 'FLOW_Produtos_Codigo', 0, 'S', '" + txtsCodigoProduto.Text + "'");
            hddidProduto.Value = "0";
            while (sdr.Read())
            {
                hddidProduto.Value = sdr["idItem"].ToString();
                txtsDscProduto.Text = sdr["sDscProduto"].ToString();
                ddlsUnidade.SelectedValue = sdr["sUnidade"].ToString();
            }

            RegistraScript("");
        }

        [System.Web.Services.WebMethod]
        public static string[] GetProdutos(string sDscProduto)
        {
            List<string> lstProdutos = new List<string>();
            if (sDscProduto.Length > 3)
            {
                SqlDataReader sdr = BD.ExecutarDataReader("sp_Select 'FLOW_Produtos', 0, 'S', '" + sDscProduto + "'");
                while (sdr.Read())
                {
                    lstProdutos.Add(string.Format("{2}|{0}|{1}|{3}", sdr["idItem"], sdr["sCodigo"], sdr["sDscProduto"], sdr["sUnidade"]));
                }
            }
            return lstProdutos.ToArray();
        }

        #endregion

        #region | Itens Cotação
        protected void dtgItens_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.BackColor = System.Drawing.Color.LightYellow;

                int idProduto = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "IdProduto"));

                LinkButton cmdVincular = (LinkButton)e.Row.FindControl("cmdVincular");
                TextBox txtCodigo = (TextBox)e.Row.FindControl("txtCodigo");
                TextBox txtsAtivo = (TextBox)e.Row.FindControl("txtsAtivo");
                TextBox txtidCotacao = (TextBox)e.Row.FindControl("txtidCotacao");

                if (idProduto != 0 || (idProduto != 0 && txtsAtivo.Text == "S"))
                {
                    cmdVincular.Enabled = false;
                    txtCodigo.Attributes.Add("disabled", "disabled");
                    cmdVincular.CssClass += " disabled";
                    cmdVincular.Attributes.Add("title", "O botão está desabilitado porque o produto já está vinculado.");
                }
                else
                {
                    cmdVincular.Enabled = true;
                    cmdVincular.CssClass = cmdVincular.CssClass.Replace(" disabled", "");
                    cmdVincular.Attributes.Remove("title");
                    txtCodigo.Attributes.Remove("disabled");
                }

                if (txtidCotacao.Text == "0" || string.IsNullOrEmpty(txtidCotacao.Text))
                {
                    cmdVincular.Enabled = false;
                    txtCodigo.Attributes.Add("disabled", "disabled");
                    cmdVincular.CssClass += " disabled";
                    e.Row.CssClass = " danger";
                    cmdVincular.Attributes.Add("title", "O Produto ainda não foi salvo. Salve-o");
                }
            }
        }
        void Popular_dtgItens(string idCotacao)
        {
            DataSet dsPesquisa;
            string sErro = "";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR_ITENS");
            vParametros.Add("@idCotacao", idCotacao);
            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);


            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                ls_CotacaoItens = conversorProdutos.ConverterDataSet(dsPesquisa, "Table");
                dtgItens_DataBind(ls_CotacaoItens);
            }

        }
        void dtgItens_DataBind(object obj)
        {
            dtgItens.DataSource = obj;
            dtgItens.DataBind();
        }
        protected void cmdIncluirItem_Click(object sender, EventArgs e)
        {
            if (ValidarDadosItens())
            {
                int idProduto = string.IsNullOrEmpty(hddidProduto.Value) ? 0 : Convert.ToInt32(hddidProduto.Value);
                string sCodigo = txtsCodigoProduto.Text.Trim();
                string sDscProduto = txtsDscProduto.Text.Trim();
                int nQuantidade = int.Parse(txtnQuantidade.Text.Trim());
                string sUnidade = ddlsUnidade.SelectedValue;

                var itemExistente = ls_CotacaoItens.FirstOrDefault(item => item.SCodigo == sCodigo);

                if (itemExistente != null)
                {
                    itemExistente.SDscProduto = sDscProduto;
                    itemExistente.NQuantidade = nQuantidade;
                    itemExistente.SUnidade = sUnidade;
                }
                else
                {
                    var novoItem = new cls_CotacaoComprasItens()
                    {
                        IdProduto = idProduto,
                        SCodigo = sCodigo,
                        SDscProduto = sDscProduto,
                        NQuantidade = nQuantidade,
                        SUnidade = sUnidade
                    };

                    ls_CotacaoItens.Add(novoItem);
                }

                dtgItens_DataBind(ls_CotacaoItens);

                LimparCamposItens();
            }
        }
        protected void cmdExcluirItem_Click(object sender, EventArgs e)
        {
            LinkButton btnExcluir = (LinkButton)sender;

            GridViewRow row = (GridViewRow)btnExcluir.NamingContainer;
            int rowIndex = row.RowIndex;
            ls_CotacaoItens.RemoveAt(rowIndex);

            dtgItens_DataBind(ls_CotacaoItens);
        }
        void LimparCamposItens()
        {
            hddidProduto.Value = string.Empty;
            txtsCodigoProduto.Text = string.Empty;
            txtsDscProduto.Text = string.Empty;
            txtnQuantidade.Text = string.Empty;
            ddlsUnidade.SelectedIndex = 0;
        }
        bool AtualizarItensGrid()
        {
            bool validado = true;
            foreach (GridViewRow row in dtgItens.Rows)
            {

                int idProduto = Convert.ToInt32(dtgItens.DataKeys[row.RowIndex].Value);
                TextBox txtQuantidade = row.FindControl("txtnQuantidade") as TextBox;

                var item = ls_CotacaoItens.FirstOrDefault(i => i.IdProduto == idProduto);
                if (txtQuantidade.Text == "0" || string.IsNullOrEmpty(txtQuantidade.Text))
                {
                    validado = false;
                    MensagemPaginaItens.MostraMensagem_Erro("A Quantidade precisa estar Preenchida!", false);
                    return validado;
                }
                else
                {
                    validado = true;
                    item.NQuantidade = decimal.Parse(txtQuantidade.Text.Trim());
                }
                if (item != null && validado != false)
                {
                    dtgItens_DataBind(ls_CotacaoItens);
                }
            }
            return validado;
        }
        bool Salvar_Itens(string idCotacao)
        {
            if (AtualizarItensGrid())
            {
                try
                {
                    DataSet dsSalvar = new DataSet();
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    Dictionary<String, String> vParametrosSalvar = new Dictionary<string, string>();
                    string sErro = "";
                    vParametros.Add("@sFuncao", "EXCLUIR-ITENS");
                    vParametros.Add("@idCotacao", idCotacao);
                    BD.ExecutarDataSet(sProcedure, vParametros);

                    vParametros.Clear();
                    vParametrosSalvar.Add("@sFuncao", "SALVAR-ITENS");
                    ls_CotacaoItens.ForEach(e =>
                    {
                        vParametrosSalvar["@idProduto"] = e.IdProduto.ToString();
                        vParametrosSalvar["@sDscProduto"] = e.SDscProduto;
                        vParametrosSalvar["@sUnidade"] = e.SUnidade.ToString();
                        vParametrosSalvar["@sCodigo"] = e.SCodigo.ToString();
                        vParametrosSalvar["@idCotacao"] = idCotacao;
                        vParametrosSalvar["@nQuantidade"] = BD.Conversoes.Numerico(e.NQuantidade);
                        vParametrosSalvar["@idUsuario"] = IDENTITY.Variaveis.idUsuario();

                        dsSalvar = BD.ExecutarDataSet(sProcedure, vParametrosSalvar);
                    });


                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        MensagemPaginaItens.MostraMensagem_Sucesso("Itens Salvos com Sucesso.");
                        dtgItens_DataBind(ls_CotacaoItens);
                    }
                    else
                    {
                        if (sErro != "")
                        {
                            MensagemPaginaItens.MostraMensagem_Erro("BD: " + sErro.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro($"Erro: {ex}");
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        protected void cmdVincularItem_Click(object sender, EventArgs e)
        {

            LinkButton cmdVincular = (LinkButton)sender;
            GridViewRow row = (GridViewRow)cmdVincular.NamingContainer;
            TextBox txtCodigo = (TextBox)row.FindControl("txtCodigo");

            string codigoDigitado = txtCodigo.Text;

            string sCodigo = !string.IsNullOrEmpty(codigoDigitado) ? codigoDigitado : cmdVincular.CommandArgument;

            try
            {
                DataSet dsSalvar = new DataSet();
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "VINCULA-PRODUTO");
                vParametros.Add("@sCodigo_Base", cmdVincular.CommandArgument.ToString());
                vParametros.Add("@sCodigo", sCodigo);
                vParametros.Add("@idCotacao", hddidCotacao.Value);
                dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsSalvar))
                {
                    if (RETORNO.DATASET(dsSalvar, "nRet") == "0")
                    {

                        Session["MensagemSucesso"] += RETORNO.DATASET(dsSalvar, "msg");

                        Response.Redirect($"/App/Paginas/Compras/CotacaoCompras_Detalhe.aspx?id={hddidCotacao.Value}");

                        MensagemPaginaItens.MostraMensagem_Sucesso(RETORNO.DATASET(dsSalvar, "msg"), false);
                    }
                }
                else
                {
                    MensagemPaginaItens.MostraMensagem_Erro(RETORNO.DATASET(dsSalvar, "msg"), false);
                }
            }
            catch (Exception ex)
            {
                MensagemPaginaItens.MostraMensagem_Erro("Erro: " + ex, false);
            }

        }
        #endregion

        #region | Itens Fornecedores
        void PopularRepeaterCapaFornecedores(string idCotacao)
        {
            var sErro = "";
            DataSet dsPesquisa;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR-FORNECEDORES");
            vParametros.Add("@idCotacao", idCotacao);
            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                //Controles dos Itens da Cotação
                MensagemPaginaItens.MostraMensagem("<b>Lembrete</b> Novos itens não poderão ser adicionados, pois já existe um fornecedor.", "info", false);
                DivAddProdutos.Visible = false;
                li_Importar.Visible = false;
                cmdImportarProdutos_Modal.Visible = false;
                dtgItens.Columns[5].Visible = false;
                BloquearCamposQuantidade(true);

                rptFornecedores.DataSource = dsPesquisa;
                rptFornecedores.DataBind();
                div_Comparativo.Visible = true;
            }
            else
            {
                div_Comparativo.Visible = false;

                //Controles dos Itens da Cotação
                dtgItens.Columns[5].Visible = true;
                DivAddProdutos.Visible = true;
                li_Importar.Visible = true;
                cmdImportarProdutos_Modal.Visible = true;
                BloquearCamposQuantidade(false);
                MensagemPaginaItens.MostraMensagem_Aviso("<b>Aviso</b> Itens novos não poderão ser adicionados a cotação após a adição do primeiro fornecedor");
            }
        }
        private void BloquearCamposQuantidade(bool bloquear)
        {
            foreach (GridViewRow row in dtgItens.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    TextBox txtQuantidade = (TextBox)row.FindControl("txtnQuantidade");
                    if (txtQuantidade != null)
                    {
                        if (txtQuantidade != null)
                        {
                            txtQuantidade.ReadOnly = bloquear;
                        }
                    }
                }
            }
        }
        void VerificaValoresContato(string mensagem)
        {
            if (!string.IsNullOrEmpty(Pesquisa_Parceiros.SEmail))
            {
                txtsEmail.Text = Pesquisa_Parceiros.SEmail;
            }
            if (!string.IsNullOrEmpty(Pesquisa_Parceiros.STelefone))
            {
                txtsTelefone.Text = Pesquisa_Parceiros.STelefone;
            }
            if (!string.IsNullOrEmpty(Pesquisa_Parceiros.SRgIE))
            {
                txtsRGIE.Text = Pesquisa_Parceiros.SRgIE;
            }
        }
        protected void AdcionarFornecedor_Click(object sender, EventArgs e)
        {
            if (hddidCotacao.Value != "0" && !string.IsNullOrEmpty(hddidCotacao.Value))
            {
                var idParceiro = "0";
                ddlCondPagamento.Items.Clear();
                Popular_dtgItensFornecedor(hddidCotacao.Value);
                Pesquisa_Parceiros.AtivaCampos();
                ddlsTipoCompra.Attributes.Remove("disabled");
                ddlFornecedor.Attributes.Remove("disabled");
                ddlCentroCusto.Attributes.Remove("disabled");
                LimparCamposModal();
                ddlEmpresa_SelectedIndexChanged(sender, e);
                hddidFornecedor.Value = "0";
                hddsEdicao.Value = "N";
                divCampoDataEntrega.Visible = true;
                Pesquisa_Parceiros.ModificaTamanhoCampos(3, 6);
                AbrirModal_Click(sender, e);
                hddidParceiro.Value = "0";

                div_chkValores.Visible = true;
                div_dgtItensFornecedores.Visible = false;
                chkValores.Checked = false;

                DataSet dsPesquisa;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "VALIDAR-FORNECEDORES");
                vParametros.Add("@idCotacao", hddidCotacao.Value);
                dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                Popular_dtgItens(hddidCotacao.Value);

                if (BD.ValidarDataSet(dsPesquisa))
                {
                    BloquearCamposQuantidade(true);
                }
                else
                {
                    BloquearCamposQuantidade(false);
                }
                li_historico.Visible = false;
            }
            else
            {
                MensagemPaginaItens.MostraMensagem_Aviso("<b>Aviso</b> É Necessário Salvar os Dados Antes de Adicionar um Fornecedor.");
            }
            RepopularCampos();
        }
        protected void SalvarFornecedor_Click(object sender, EventArgs e)
        {
            SalvarFornecedor(hddidCotacao.Value);
            AbrirModal_Click(sender, e);
        }
        protected void Pesquisa_Parceiros_ParceiroAlterado(object sender, EventArgs e)
        {
            string idParceiro = Pesquisa_Parceiros.idParceiro.ToString();
        }
        bool ValidarDadosFornecedor()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (string.IsNullOrEmpty(Pesquisa_Parceiros.SRazaoSocial.Trim()))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe a Razão Social do Fornecedor.";
            }

            if (string.IsNullOrEmpty(ddlidEmpresa.SelectedValue) || ddlidEmpresa.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe uma Empresa na Cotação e depois adicione o modal.";
            }

            if (string.IsNullOrEmpty(Pesquisa_Parceiros.SCnpj_CPF.Trim()) && ddlsTipoCompra.SelectedValue != "I")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o CNPJ ou CPF do Fornecedor.";
            }

            if (string.IsNullOrEmpty(ddlFornecedor.SelectedValue) || ddlFornecedor.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Fornecedor.";
            }

            if (hddidParceiro.Value == "0" || string.IsNullOrEmpty(hddidParceiro.Value))
            {
                if (Pesquisa_Parceiros.idParceiro.ToString() == "" || Pesquisa_Parceiros.idParceiro.ToString() == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "O Fornecedor Precisa Estar Cadastrado!";
                }
            }

            if (!string.IsNullOrEmpty(txtsEmail.Text.Trim()) && !IsValidEmail(txtsEmail.Text.Trim()))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "O Email informado é inválido.";
            }

            if (string.IsNullOrEmpty(ddlsTipoCompra.SelectedValue) || ddlsTipoCompra.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Tipo de Compra.";
            }

            if (string.IsNullOrEmpty(ddlCentroCusto.SelectedValue) || ddlCentroCusto.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Centro de Custo.";
            }

            if (string.IsNullOrEmpty(ddlFluxo.SelectedValue) || ddlFluxo.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Fluxo.";
            }

            if (string.IsNullOrEmpty(ddlCondPagamento.SelectedValue) || ddlCondPagamento.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione a Condição de Pagamento.";
            }

            if (string.IsNullOrEmpty(ddlTipoEnvio.SelectedValue) || ddlTipoEnvio.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Tipo de Envio.";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPaginaModal.MostraMensagem_Erro(sMensagemErro, false);
            }

            return bRetorno;
        }
        void LimparCamposModal()
        {
            Pesquisa_Parceiros.LimparCampos();
            txtsEmail.Text = "";
            txtsRGIE.Text = "";
            txtsTelefone.Text = "";
            txtdtPrevisaoUso.Text = "";
            txtnPrazoEntrega.Text = "";
            txtnFrete.Text = "0";

            ddlsTipoCompra.SelectedValue = "0";
            ddlCentroCusto.SelectedValue = "0";


            if (ddlidEmpresa.SelectedValue == "0" || string.IsNullOrEmpty(ddlidEmpresa.SelectedValue))
            {
                txtsPedidoCompra.Text = "";
            }

            ddlFluxo.SelectedValue = "0";
            ddlCondPagamento.SelectedValue = "0";
            ddlTipoEnvio.SelectedValue = "0";
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        void Gerar_Email(string sProcedure, Dictionary<String, String> vParametros, string idFornecedor)
        {
            string sErro;
            DataSet dsSalvar;
            vParametros["@idFornecedor"] = idFornecedor;
            vParametros["@sFuncao"] = "GERAR-EMAIL";
            dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsSalvar, out sErro) || RETORNO.DATASET(dsSalvar, "nRet").ToString() == "0")
            {
                Session["MensagemEmail"] = "Um Email Foi Enviado ao Fornecedor Solicitando os Valores.";
            }
            else
            {
                Session["MensagemEmailErro"] = "Falha ao Enviar o Email - <b>Motivo: </b>" +
                        (!string.IsNullOrEmpty(RETORNO.DATASET(dsSalvar, "msg")) ? RETORNO.DATASET(dsSalvar, "msg").ToString() : "Sem Email Cadastrado no Parceiro.");
            }
        }
        void SalvarFornecedor(string idCotacao)
        {
            if (ValidarDadosFornecedor() && AtualizarItensFornecedoresGrid())
            {
                try
                {
                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    string sErro = "";

                    //Cadastro
                    vParametros.Add("@sFuncao", "SALVAR-FORNECEDORES");
                    vParametros.Add("@idCotacao", idCotacao);
                    vParametros.Add("@idFornecedor", hddidFornecedor.Value);
                    vParametros.Add("@sFornecedor", Pesquisa_Parceiros.SRazaoSocial);
                    vParametros.Add("@sCnpj", Pesquisa_Parceiros.SCnpj_CPF);
                    if (string.IsNullOrEmpty(Pesquisa_Parceiros.idParceiro.ToString()) || Pesquisa_Parceiros.idParceiro.ToString() == "0")
                    {
                        vParametros.Add("@idParceiro", hddidParceiro.Value);
                    }
                    else
                    {
                        vParametros.Add("@idParceiro", Pesquisa_Parceiros.idParceiro.ToString());
                    }

                    vParametros.Add("@sEmail", txtsEmail.Text);
                    vParametros.Add("@sTelefone", txtsTelefone.Text);
                    vParametros.Add("@sRG_IE", txtsRGIE.Text);
                    vParametros.Add("@nPrazoEntrega", txtnPrazoEntrega.Text);
                    vParametros.Add("@sEdicao", hddsEdicao.Value);
                    vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());

                    // Adicionandos
                    vParametros.Add("@sTipoCompra", ddlsTipoCompra.SelectedValue);
                    vParametros.Add("@sPedidoCompra", txtsPedidoCompra.Text);
                    vParametros.Add("@idFluxo", ddlFluxo.SelectedValue);
                    vParametros.Add("@idCondicaoPagamento", ddlCondPagamento.SelectedValue);
                    vParametros.Add("@idTipoEnvio", ddlTipoEnvio.SelectedValue);
                    vParametros.Add("@nValorFrete", BD.Conversoes.Numerico(Convert.ToDecimal(txtnFrete.Text)));

                    vParametros.Add("@idCentroCusto", ddlCentroCusto.SelectedValue);

                    if (ls_CotacaoItensFornecedor.Count() != 0)
                    {
                        dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                        if (BD.ValidarDataSet(dsSalvar, out sErro))
                        {
                            var msg = "O Fornecedor já está cadastrado.";
                            if (RETORNO.DATASET(dsSalvar, 0, "msg").ToLower() == msg.ToLower())
                            {
                                MensagemPaginaModal.MostraMensagem_Erro("Esse Fornecedor já foi adicionado!");
                                hddidParceiro.Value = "0";
                                Pesquisa_Parceiros.LimparCamposParceiro();
                                ddlFornecedor.SelectedValue = "0";
                            }
                            else
                            {
                                var idFornecedor = RETORNO.DATASET(dsSalvar, "idFornecedor");
                                hddidHistorico.Value = RETORNO.DATASET(dsSalvar, "idHistorico");
                                Salvar_ItensForncedor(idFornecedor);

                                if (!chkValores.Checked)
                                {
                                    Gerar_Email(sProcedure, vParametros, idFornecedor);
                                }

                                Pesquisar(idCotacao, false);
                                LimparCamposModal();
                                Session["MensagemSucesso"] = "Dados Salvos Com Sucesso";


                                Response.Redirect($"/App/Paginas/Compras/CotacaoCompras_Detalhe.aspx?id={hddidCotacao.Value}");
                            }
                        }
                        else
                        {
                            if (sErro != "")
                            {
                                MensagemPaginaModal.MostraMensagem_Erro("BD: " + sErro.ToString(), false);
                            }

                        }
                    }
                    else
                    {
                        MensagemPaginaModal.MostraMensagem_Aviso("<b>Aviso</b> Os Itens Não podem estar zerados, reinicie o modal.");
                    }

                }
                catch (Exception ex)
                {
                    MensagemPaginaModal.MostraMensagem_Erro($"Erro: {ex}", false);
                }
            }
            PopularHistorico();
        }
        void ValidaDdl(DropDownList ddl)
        {
            FUNCOES.Popula_Combo(ddlFornecedor, "sp_Select 'Flow_Parceiros_Fornecedores', @sTipo =" + ddl.SelectedValue, "idCliente", "Razao_CNPJ", false, "Selecione o Fornecedor", "0");
        }
        protected void ddlEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            FUNCOES.Popula_Combo(ddlFornecedor, "sp_Select 'Flow_Parceiros_Fornecedores', @sTipo =" + ddlsTipoCompra.SelectedValue, "idCliente", "Razao_CNPJ", false, "Selecione o Fornecedor", "0");
            FUNCOES.Popula_Combo(ddlsEnderecoEntrega, string.Format("sp_Manipula_tbl_Flow_Clientes 'CONSULTAR_ENDERECO_COMPRAS', {0}", ddlidEmpresa.SelectedValue), "idEndereco", "sEnderecoCompleto", false);
            ddlsEnderecoEntrega.Visible = true;

            string Pedido = "";
            string sDscEmpresa = "";
            string nPedido = "";
            int anoAtual = DateTime.Now.Year;
            string anoFormatado = (anoAtual % 100).ToString("D2");

            if (ddlsTipoCompra.SelectedValue == "N")
            {
                Pedido = "PC";
                dtgItensFornecedor.Columns[5].Visible = true;
            }
            if (ddlsTipoCompra.SelectedValue == "I")
            {
                Pedido = "PO";
                dtgItensFornecedor.Columns[5].Visible = false;
            }

            DataSet dsPesquisa;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTA_NUMERO_PEDIDO_COMPRA");
            vParametros.Add("@idEmpresa", ddlidEmpresa.SelectedValue);

            dsPesquisa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametros);

            sDscEmpresa = RETORNO.DATASET(dsPesquisa, "sDscEmpresaReduzida");
            if (ddlsTipoCompra.SelectedValue == "N")
                nPedido = RETORNO.DATASET(dsPesquisa, "nPedidoNacional");
            if (ddlsTipoCompra.SelectedValue == "I")
                nPedido = RETORNO.DATASET(dsPesquisa, "nPedidoInternacional");

            txtsPedidoCompra.Text = sDscEmpresa + "." + Pedido + ".C" + hddidCotacao.Value + "." + anoFormatado;
        }
        void PopularComboFluxo()
        {
            SqlDataReader dr;
            dr = BD.ExecutarDataReader("sp_Select 'Flow_Fluxo', " + 7);

            if (dr != null)
            {
                Base_Fluxo.Clear();
                ddlFluxo.Items.Clear();
                ddlFluxo.Items.Add(new System.Web.UI.WebControls.ListItem("Selecione o Fluxo", "0"));
                while (dr.Read())
                {
                    FrameWork.cls_Fluxo objItem = new FrameWork.cls_Fluxo();
                    objItem.idFluxo = Convert.ToInt32(dr["idFluxo"].ToString());
                    objItem.sDscFluxo = dr["sDscFluxo"].ToString();
                    objItem.nTempoTotalHoras = Convert.ToInt32(dr["nTempoTotalHoras"].ToString());
                    objItem.nTempoDias = Convert.ToInt32(dr["nTempoDias"].ToString());
                    Base_Fluxo.Add(objItem);

                    ddlFluxo.Items.Add(new System.Web.UI.WebControls.ListItem(objItem.sDscFluxo, objItem.idFluxo.ToString()));
                }
            }
            dr.Close();
        }
        private void AjustarVisibilidadeIPI()
        {
            if (ddlsTipoCompra.SelectedValue == "N")
            {
                dtgItensFornecedor.Columns[5].Visible = true;
            }
            else if (ddlsTipoCompra.SelectedValue == "I")
            {
                dtgItensFornecedor.Columns[5].Visible = false;
            }
        }
        protected void ddlTipoEnvio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlTipoEnvio.SelectedValue == "1")
            {
                ddlsEnderecoEntrega.SelectedValue = "-1";
            }
        }

        private bool IsValidacaoValoresAtiva
        {
            get { return ViewState["chkValores"] != null && (bool)ViewState["chkValores"]; }
            set { ViewState["chkValores"] = value; }
        }

        protected void lnkCard_Click(object sender, EventArgs e)
        {
     
            LinkButton btn = (LinkButton)sender;
            string idFornecedor = btn.CommandArgument;
            try
            {
                var sErro = "";
                if (idFornecedor != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE-FORNECEDOR");
                    vParametros.Add("@idFornecedor", idFornecedor);
                    hddsEdicao.Value = "S";
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);
                    Pesquisa_Parceiros.DesativaCampos();

                    div_chkValores.Visible = false;
                    div_dgtItensFornecedores.Visible = true;
                    chkValores.Checked = true;
                    IsValidacaoValoresAtiva = true;

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        Pesquisa_Parceiros.SetCampos(RETORNO.DATASET(dsPesquisa, 0, "sCnpj"), RETORNO.DATASET(dsPesquisa, 0, "sFornecedor"));
                        txtsEmail.Text = RETORNO.DATASET(dsPesquisa, 0, "sEmail");
                        txtsRGIE.Text = RETORNO.DATASET(dsPesquisa, 0, "sRG_IE");
                        txtsTelefone.Text = RETORNO.DATASET(dsPesquisa, 0, "sTelefone");
                        txtnPrazoEntrega.Text = RETORNO.DATASET(dsPesquisa, 0, "nPrazoEntrega");
                        hddidParceiro.Value = RETORNO.DATASET(dsPesquisa, 0, "idParceiro");
                        hddidFornecedor.Value = RETORNO.DATASET(dsPesquisa, "idFornecedor");
                        ddlsTipoCompra.SelectedValue = RETORNO.DATASET(dsPesquisa, "sTipoCompra");

                        FUNCOES.Popula_Combo(ddlCentroCusto, "sp_Manipula_tbl_Flow_Pedidos 'FLOW-CENTRO-CUSTOS'", "idCentroDeCusto", "sDescricao", false, "Selecione o Centro de Custo", "0");
                        ddlCentroCusto.SelectedValue = RETORNO.DATASET(dsPesquisa, "idCentroCusto");

                        AjustarVisibilidadeIPI();

                        if (string.IsNullOrEmpty(RETORNO.DATASET(dsPesquisa, "sPedidoCompra")))
                        {
                            ddlEmpresa_SelectedIndexChanged(sender, e);
                        }
                        else
                        {
                            txtsPedidoCompra.Text = RETORNO.DATASET(dsPesquisa, "sPedidoCompra");
                        }

                        if (!string.IsNullOrEmpty(ddlsTipoCompra.SelectedValue) || ddlsTipoCompra.SelectedValue != "0")
                        {
                            ValidaDdl(ddlsTipoCompra);
                        }

                        string idParceiro = RETORNO.DATASET(dsPesquisa, "idParceiro");

                        ddlFornecedor.SelectedValue = idParceiro;

                        ddlFluxo.SelectedValue = RETORNO.DATASET(dsPesquisa, "idFluxo");

                        PopularCondPagamento(idParceiro);
                        ddlCondPagamento.SelectedValue = RETORNO.DATASET(dsPesquisa, "idCondicaoPagamento");

                        ddlTipoEnvio.SelectedValue = RETORNO.DATASET(dsPesquisa, "idTipoEnvio");
                        txtnFrete.Text = RETORNO.DATASET(dsPesquisa, "nValorFrete");
                        PopularGridItensFornecedoresDetalhe(idFornecedor);

                        AbrirModal_Click(sender, e);
                    }
                    else
                    {
                        if (sErro != "")
                        {
                            MensagemPaginaModal.MostraMensagem_Erro(sErro);
                        }
                    }

                }
                RegistraScript("");
                Popular_dtgItens(hddidCotacao.Value);
                PopularHistorico();
                BloquearCamposQuantidade(true);
                ddlFornecedor.Attributes.Add("disabled", "disabled");
                ddlsTipoCompra.Attributes.Add("disabled", "disabled");

                if (ddlCentroCusto.SelectedValue != "0")
                    ddlCentroCusto.Attributes.Add("disabled", "disabled");
                else
                    ddlCentroCusto.Attributes.Remove("disabled");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }
        protected void rptFornecedores_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;

                var statusBolinha = (Panel)e.Item.FindControl("statusBolinha");
                var contadorCamposNaoPreenchidos = (Label)e.Item.FindControl("contadorCamposNaoPreenchidos");

                int camposNaoPreenchidos = Convert.ToInt32(row["nCamposNaoPreenchidos"]);

                if (statusBolinha != null)
                {
                    if (camposNaoPreenchidos == 0)
                    {
                        statusBolinha.Attributes["style"] = "background-color: green;"; // cor de sucesso
                    }
                    else
                    {
                        statusBolinha.Attributes["style"] = "background-color: red;"; // cor de erro
                    }
                    statusBolinha.Attributes["class"] += " status-circle"; // Para garantir a classe CSS
                }
            }
        }

        #region | Itens Fornecedor
        protected void dtgItensFornecedor_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                TextBox txtnQuantidade = (TextBox)e.Row.FindControl("txtnQuantidadeFornecedor");
                TextBox txtnValorCotado = (TextBox)e.Row.FindControl("txtnValorCotadoFornecedor");
                TextBox txtdtPrazo = (TextBox)e.Row.FindControl("txtdtPrazo");

                e.Row.BackColor = System.Drawing.Color.White;

                if (txtdtPrazo.Text == "1900-01-01")
                {
                    txtdtPrazo.Text = "";
                }
            }
        }
        protected void txtdtPrevisaoUso_TextChanged(object sender, EventArgs e)
        {
            string selectedDate = txtdtPrevisaoUso.Text;

            DateTime parsedDate;
            if (DateTime.TryParse(selectedDate, out parsedDate))
            {
                string formattedDate = parsedDate.ToString("yyyy-MM-dd");

                foreach (GridViewRow row in dtgItensFornecedor.Rows)
                {
                    TextBox txtdtPrazo = (TextBox)row.FindControl("txtdtPrazo");
                    if (txtdtPrazo != null)
                    {
                        txtdtPrazo.Text = formattedDate;
                    }
                }

                MensagemPaginaModal.MostraMensagem_Aviso("<b>Aviso</b> O Prazo de Entrega foi Padronizado nos itens abaixo, é possível alterar se necessário!");
                AbrirModal_Click(sender, e);
            }
            else
            {
                AbrirModal_Click(sender, e);
            }
        }
        void Popular_dtgItensFornecedor(string idCotacao)
        {
            if (idCotacao != "0")
            {
                DataSet dsPesquisa;
                string sErro = "";
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR_ITENS");
                vParametros.Add("@idCotacao", idCotacao);
                dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);


                if (BD.ValidarDataSet(dsPesquisa, out sErro))
                {
                    ls_CotacaoItensFornecedor = conversorProdutos.ConverterDataSet(dsPesquisa, "Table");
                    dtgItensFornecedor_DataBind(ls_CotacaoItensFornecedor);
                }
            }
            else
            {
                ls_CotacaoItensFornecedor = ls_CotacaoItens;
                dtgItensFornecedor_DataBind(ls_CotacaoItensFornecedor);
            }
        }
        void dtgItensFornecedor_DataBind(object obj)
        {
            dtgItensFornecedor.DataSource = obj;
            dtgItensFornecedor.DataBind();
        }
        protected void cmdIncluirItemFornecedor_Click(object sender, EventArgs e)
        {
            var novosItens = new List<cls_CotacaoComprasItens>()
             {
                 new cls_CotacaoComprasItens()
                 {
                     IdProduto = Convert.ToInt32(hddidProduto.Value),
                     SCodigo = txtsCodigoProduto.Text.Trim(),
                     SDscProduto = txtsDscProduto.Text.Trim(),
                     NQuantidade = int.Parse(txtnQuantidade.Text.Trim()),
                     SUnidade = ddlsUnidade.SelectedValue,
                     NValorCotado = 0.0000m,
                     NIPI = 0m
                 }
             };

            ls_CotacaoItensFornecedor.AddRange(novosItens);

            dtgItensFornecedor_DataBind(ls_CotacaoItensFornecedor);

            LimparCamposItens();
            AbrirModal_Click(sender, e);
        }
        protected void cmdExcluirItemFornecedor_Click(object sender, EventArgs e)
        {
            LinkButton btnExcluir = (LinkButton)sender;
            if (ls_CotacaoItensFornecedor.Count > 1)
            {
                GridViewRow row = (GridViewRow)btnExcluir.NamingContainer;
                int rowIndex = row.RowIndex;
                ls_CotacaoItensFornecedor.RemoveAt(rowIndex);
                dtgItensFornecedor_DataBind(ls_CotacaoItensFornecedor);
            }
            else
            {
                MensagemPaginaModal.MostraMensagem_Erro("<b>Erro</b> É Necessário pelo menos 1 item na grid");
            }

            AbrirModal_Click(sender, e);
        }
        bool Salvar_ItensForncedor(string idFornecedor)
        {
            try
            {
                DataSet dsSalvar = new DataSet();
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                Dictionary<String, String> vParametrosSalvar = new Dictionary<string, string>();
                string sErro = "";

                vParametros.Add("@sFuncao", "EXCLUIR-ITENS-FORNECEDORES");
                vParametros.Add("@idFornecedor", idFornecedor);
                DataSet dsAntigos = BD.ExecutarDataSet(sProcedure, vParametros);

                vParametros.Clear();
                vParametrosSalvar.Add("@sFuncao", "SALVAR-ITENS-FORNECEDORES");

                ls_CotacaoItensFornecedor.ForEach(e =>
                {
                    vParametrosSalvar["@idProduto"] = e.IdProduto.ToString();

                    if (string.IsNullOrEmpty(Pesquisa_Parceiros.idParceiro.ToString()) || Pesquisa_Parceiros.idParceiro.ToString() == "0")
                    {
                        vParametrosSalvar["@idParceiro"] = hddidParceiro.Value;
                    }
                    else
                    {
                        vParametrosSalvar["@idParceiro"] = Pesquisa_Parceiros.idParceiro.ToString();
                    }

                    if (e.DtPrevisao.ToString() == "01/01/1900 00:00:00")
                    {
                        vParametrosSalvar["@dtPrevisao"] = "";
                    }
                    else
                    {
                        vParametrosSalvar["@dtPrevisao"] = e.DtPrevisao.ToString();
                    }


                    vParametrosSalvar["@sDscProduto"] = e.SDscProduto;
                    vParametrosSalvar["@sUnidade"] = e.SUnidade.ToString();
                    vParametrosSalvar["@nValorCotado"] = BD.Conversoes.Numerico(e.NValorCotado);
                    vParametrosSalvar["@NIPI"] = BD.Conversoes.Numerico(e.NIPI);
                    vParametrosSalvar["@sCodigo"] = e.SCodigo.ToString();
                    vParametrosSalvar["@idCotacao"] = hddidCotacao.Value;
                    vParametrosSalvar["@idFornecedor"] = idFornecedor;
                    vParametrosSalvar["@idUsuario"] = IDENTITY.Variaveis.idUsuario();
                    vParametrosSalvar["@nQuantidade"] = BD.Conversoes.Numerico(e.NQuantidade);
                    vParametrosSalvar["@idHistorico"] = hddidHistorico.Value;

                    DataRow itemAntigo = dsAntigos.Tables[0].AsEnumerable()
                                             .FirstOrDefault(row => Convert.ToInt32(row["idProduto"]) == e.IdProduto ||
                                                                       row["sCodigo"].ToString() == e.SCodigo);

                    if (itemAntigo != null)
                    {
                        vParametrosSalvar["@old_sCodigo"] = itemAntigo["sCodigo"].ToString();
                        vParametrosSalvar["@old_sDscProduto"] = itemAntigo["sDscProduto"].ToString();
                        vParametrosSalvar["@old_sUnidade"] = itemAntigo["sUnidade"].ToString();
                        vParametrosSalvar["@old_nValorCotado"] = itemAntigo["nValorCotado"].ToString().Replace(",", ".");
                        vParametrosSalvar["@old_nIPI"] = itemAntigo["nIPI"].ToString().Replace(",", ".");
                        vParametrosSalvar["@old_dtPrevisao"] = itemAntigo["dtPrevisao"].ToString();
                        vParametrosSalvar["@old_nQuantidade"] = itemAntigo["nQuantidade"].ToString();
                        vParametrosSalvar["@sGerar"] = "S";
                    }
                    else
                    {
                        vParametrosSalvar["@sGerar"] = "N";
                    }

                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametrosSalvar);
                });

                if (BD.ValidarDataSet(dsSalvar, out sErro))
                {
                    MensagemPaginaModal.MostraMensagem_Sucesso("Itens Salvos com Sucesso!");
                    dtgItens_DataBind(ls_CotacaoItens);
                }
                else
                {
                    if (!string.IsNullOrEmpty(sErro))
                    {
                        MensagemPaginaModal.MostraMensagem_Erro("BD: " + sErro);
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPaginaModal.MostraMensagem_Erro($"Erro: {ex}");
                return false;
            }

            return true;
        }
        bool AtualizarItensFornecedoresGrid()
        {
            bool validado = true;
            string sMensagemErro = "";

            foreach (GridViewRow row in dtgItensFornecedor.Rows)
            {
                int idProduto = Convert.ToInt32(dtgItens.DataKeys[row.RowIndex].Value);

                TextBox txtQuantidade = row.FindControl("txtnQuantidade") as TextBox;
                TextBox txtValorCotado = row.FindControl("txtnValorCotado") as TextBox;
                TextBox txtnIPI = row.FindControl("txtnIPI") as TextBox;
                TextBox txtdtPrevisao = row.FindControl("txtdtPrazo") as TextBox;
                Label lblErroPreco = row.FindControl("lblErroPreco") as Label;
                Label lblErroDt = row.FindControl("lblErroDt") as Label;
                Label lblErroQtd = row.FindControl("lblErroQtd") as Label;
                Label lblErroIPI = row.FindControl("lblErroIPI") as Label;
                TextBox txtCodigoProduto = row.FindControl("txtsCodigo") as TextBox;

                var msg = "Campo obrigatório";

                var item = (idProduto != 0)
                    ? ls_CotacaoItensFornecedor.FirstOrDefault(i => i.IdProduto == idProduto)
                    : ls_CotacaoItensFornecedor.FirstOrDefault(i => i.SCodigo == txtCodigoProduto.Text.Trim());

                if (item == null && idProduto == 0)
                {
                    validado = false;
                    lblErroQtd.Text = msg;
                    lblErroPreco.Text = msg;
                    lblErroDt.Text = msg;
                    lblErroIPI.Text = msg;
                    continue;
                }

                lblErroPreco.Text = "";
                lblErroDt.Text = "";
                lblErroQtd.Text = "";
                lblErroIPI.Text = "";

                if (string.IsNullOrEmpty(txtQuantidade.Text) || txtQuantidade.Text == "0")
                {
                    validado = false;
                    lblErroQtd.Text = msg;
                }
                else
                {
                    if (item != null)
                    {
                        item.NQuantidade = decimal.Parse(txtQuantidade.Text.Trim());
                    }
                    else if (idProduto == 0)
                    {
                        var itemExistente = ls_CotacaoItensFornecedor.FirstOrDefault(i => i.SCodigo == txtCodigoProduto.Text.Trim());
                        if (itemExistente == null)
                        {
                            var novoItem = new cls_CotacaoComprasItens
                            {
                                SCodigo = txtCodigoProduto.Text.Trim(),
                                NQuantidade = decimal.Parse(txtQuantidade.Text.Trim()),
                            };
                            ls_CotacaoItensFornecedor.Add(novoItem);
                        }
                        else
                        {
                            itemExistente.NQuantidade += decimal.Parse(txtQuantidade.Text.Trim());
                        }
                    }
                }
                if (chkValores.Checked)
                {
                    if (string.IsNullOrEmpty(txtValorCotado.Text) || txtValorCotado.Text == "0" || txtValorCotado.Text == "0,0000")
                    {
                        validado = false;
                        lblErroPreco.Text = msg;
                    }
                    else
                    {
                        if (item != null)
                        {
                            item.NValorCotado = decimal.Parse(txtValorCotado.Text);
                        }
                    }
                }
                //if (this.IsValidacaoValoresAtiva)
                //{
                //    if (string.IsNullOrEmpty(txtValorCotado.Text) || txtValorCotado.Text == "0" || txtValorCotado.Text == "0,0000")
                //    {
                //        validado = false;
                //        lblErroPreco.Text = msg;
                //    }
                //    else
                //    {
                //        if (item != null)
                //        {
                //            item.NValorCotado = decimal.Parse(txtValorCotado.Text);
                //        }
                //    }
                //}

                if (ddlsTipoCompra.SelectedValue == "N")
                {
                    if (chkValores.Checked)
                    {
                        if (item != null)
                        {
                            item.NIPI = decimal.Parse(txtnIPI.Text.Trim());
                        }
                    }

                }

                if (chkValores.Checked)
                {
                    if (string.IsNullOrEmpty(txtdtPrevisao.Text))
                    {
                        validado = false;
                        lblErroDt.Text = msg;
                    }
                    else
                    {
                        DateTime dtPrevisao;
                        if (DateTime.TryParse(txtdtPrevisao.Text, out dtPrevisao))
                        {
                            if (dtPrevisao < DateTime.Today)
                            {
                                validado = false;
                                lblErroDt.Text = "A data de previsão não pode ser menor que a data atual.";
                            }
                            else if (item != null)
                            {
                                item.DtPrevisao = dtPrevisao;
                            }
                        }
                        else
                        {
                            validado = false;
                            lblErroDt.Text = "Data inválida.";
                        }
                    }
                }

            }

            if (validado)
            {
                dtgItensFornecedor_DataBind(ls_CotacaoItensFornecedor);
            }

            return validado;
        }
        void PopularGridItensFornecedoresDetalhe(string idFornecedor)
        {
            var sErro = "";
            DataSet dsPesquisa;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR_ITENS-FORNECEDOR");
            vParametros.Add("@idFornecedor", idFornecedor);
            vParametros.Add("@idCotacao", hddidCotacao.Value);
            vParametros.Add("@idParceiro", hddidParceiro.Value);
            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                ls_CotacaoItensFornecedor = conversorProdutos.ConverterDataSet(dsPesquisa, "Table");
                dtgItensFornecedor_DataBind(ls_CotacaoItensFornecedor);

            }
            else
            {
                if (sErro != "")
                {
                    MensagemPaginaModal.MostraMensagem_Erro(sErro);
                }
            }
        }
        #endregion

        protected void ddlFornecedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            FUNCOES.Popula_Combo(ddlCondPagamento, "sp_Select 'FLOW_CondicaoDePagamento',@idFiltro= 1, @idPesquisa=" + ddlFornecedor.SelectedValue, "idCondicaoPagamento", "sDscCondicaoPagamento", false, "Selecione a Condição de Pagamento", "0");
            CompletarCamposParceiro(ddlFornecedor.SelectedValue);
        }

        void CompletarCamposParceiro(string idParceiro)
        {
            DataSet dsPesquisa;
            string sErro = "";
            Dictionary<string, string> vParametros = new Dictionary<string, string>
        {
            { "@sFuncao", "COMPLETAR_PARCEIROS" },
            { "@idParceiro", ddlFornecedor.SelectedValue }
        };
            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                Pesquisa_Parceiros.SCnpj_CPF = RETORNO.DATASET(dsPesquisa, "sCPF_CNPJ");
                Pesquisa_Parceiros.idParceiro = Convert.ToInt32(RETORNO.DATASET(dsPesquisa, "idCliente"));
                Pesquisa_Parceiros.SRazaoSocial = RETORNO.DATASET(dsPesquisa, "sRazaoSocial");
                txtsEmail.Text = RETORNO.DATASET(dsPesquisa, "sEmail");
                txtsTelefone.Text = RETORNO.DATASET(dsPesquisa, "sTelefone");
                if (string.IsNullOrEmpty(txtsEmail.Text))
                {
                    MensagemPaginaModal.MostraMensagem_Aviso("<b>Aviso</b> Email Não Importado pois não há Contato Cadastrado no Parceiro.");
                }
            }
            else
            { }
        }

        protected void ChkValores_CheckedChanged(object sender, EventArgs e)
        {
            div_dgtItensFornecedores.Visible = chkValores.Checked;
        }
        #endregion

        #region | Histórico
        void PopularHistorico()
        {
            var sErro = "";
            DataSet dsPesquisa;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR-HISTORICO");
            vParametros.Add("@idCotacao", hddidCotacao.Value);
            vParametros.Add("@idParceiro", hddidParceiro.Value);
            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                li_historico.Visible = true;
                dtgHistoricoDataBind(dsPesquisa);
            }
            else
            {
                li_historico.Visible = false;
                if (sErro != "")
                {
                    MensagemPaginaHistorico.MostraMensagem_Erro(sErro);
                }
            }
        }
        void dtgHistoricoDataBind(DataSet dsHistorico)
        {
            dtgHistorico.DataSource = dsHistorico;
            dtgHistorico.DataBind();
        }
        protected void dtgHistorico_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Literal literalAcao = (Literal)e.Row.FindControl("LiteralAcao");
                string sDscAcao = DataBinder.Eval(e.Row.DataItem, "sDscAcao").ToString();
                literalAcao.Text = sDscAcao;

                string idHistorio = dtgHistorico.DataKeys[e.Row.RowIndex].Value.ToString();
                var dtgHistoricoItens = (GridView)e.Row.FindControl("dtgHistoricoItens");
                var btnToggle = (LinkButton)e.Row.FindControl("btnToggle");

                var sErro = "";
                DataSet dsHistoricoItens;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR-ITENS-HISTORICO");
                vParametros.Add("@idHistorico", idHistorio);
                dsHistoricoItens = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsHistoricoItens, out sErro))
                {
                    dtgHistoricoItens.DataSource = dsHistoricoItens;
                    dtgHistoricoItens.DataBind();
                    btnToggle.Visible = true;
                }
                else
                {
                    btnToggle.Visible = false;
                }
            }
        }
        protected void dtgHistoricoItens_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Literal literalAcao = (Literal)e.Row.FindControl("LiteralAcao");
                string sDscAcao = DataBinder.Eval(e.Row.DataItem, "sDscAcao").ToString();
                literalAcao.Text = sDscAcao;
            }
        }
        #endregion

        #region | Comparativos 
        private List<string> fornecedores;
        protected void cmdAbrirComparativos_Click(object sender, EventArgs e)
        {
            panelCardsComparativo.Visible = false;
            divComparador.Visible = true;
            cmdMostrarCards.CssClass = "btn btn-default";
            cmdComparar.CssClass = "btn btn-primary active";
            PopularRepeaterComparativos();
        }

        // LÓGICA NOVA: Handler para o botão "Visão em Cartões"
        protected void cmdMostrarCards_Click(object sender, EventArgs e)
        {
            panelCardsComparativo.Visible = true;
            divComparador.Visible = false;
            cmdMostrarCards.CssClass = "btn btn-primary active";
            cmdComparar.CssClass = "btn btn-default";
        }

        void PopularRepeaterComparativos()
        {
            DataSet dsPesquisa;
            string sErro = "";
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_COMPARATIVOS" },
                { "@idCotacao", hddidCotacao.Value }
            };

            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                DataTable dt = dsPesquisa.Tables[0];

                fornecedores = dt.Columns
                    .Cast<DataColumn>()
                    .Where(col => col.ColumnName.StartsWith("ValorCotado_"))
                    .Select(col => col.ColumnName.Replace("ValorCotado_", "").Replace("_", " ").Trim())
                    .Distinct()
                    .ToList();

                ViewState["fornecedores"] = fornecedores;

                var produtos = new List<ComparativoProduto>();

                foreach (DataRow row in dt.Rows)
                {
                    var valores = fornecedores.Select(f =>
                    {
                        string fornecedorFormatado = f.Replace(" ", "_");

                        var preco = row[$"ValorCotado_{fornecedorFormatado}"].ToString();
                        var previsao = row[$"Previsao_{fornecedorFormatado}"].ToString();

                        return new ComparativoFornecedor
                        {
                            Fornecedor = f,
                            Preco = preco,
                            Previsao = previsao,
                            idParceiro = dt.Columns.Contains($"idParceiro_{fornecedorFormatado}")
                                ? row[$"idParceiro_{fornecedorFormatado}"].ToString()
                                : string.Empty
                        };
                    }).ToList();

                    produtos.Add(new ComparativoProduto
                    {
                        sDscProduto = row["sDscProduto"].ToString(),
                        idProduto = dt.Columns.Contains("idProduto") ? row["idProduto"].ToString() : string.Empty,
                        idParceiro = dt.Columns.Contains("idParceiro") ? row["idParceiro"].ToString() : string.Empty,
                        Valores = valores
                    });
                }

                rptComparativos.DataSource = produtos;
                rptComparativos.DataBind();
            }
            else
            {
                MensagemPaginaComparativo.MostraMensagem_Aviso("<b>Aviso</b> Não há itens disponíveis para comparar", false);
            }
        }
        protected void rptComparativos_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                var rptFornecedores = e.Item.FindControl("rptFornecedores") as Repeater;
                var rptHeaders = e.Item.FindControl("rptHeaders") as Repeater;

                if (rptFornecedores != null && fornecedores != null)
                {
                    rptFornecedores.DataSource = fornecedores;
                    rptFornecedores.DataBind();
                }

                if (rptHeaders != null && fornecedores != null)
                {
                    var headers = new List<string>();
                    foreach (var fornecedor in fornecedores)
                    {
                        headers.Add("Preço");
                        headers.Add("Data Previsão");
                    }
                    rptHeaders.DataSource = headers;
                    rptHeaders.DataBind();
                }
            }
        }
        protected void rptComparativos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var produto = e.Item.DataItem as ComparativoProduto;

                if (produto == null)
                {
                    return;
                }

                var rptValores = e.Item.FindControl("rptValores") as Repeater;
                if (rptValores != null && produto.Valores != null)
                {
                    rptValores.DataSource = produto.Valores;
                    rptValores.DataBind();
                }

                var hiddenIdProduto = e.Item.FindControl("hiddenIdProduto") as HiddenField;
                var hiddenIdParceiro = e.Item.FindControl("hiddenIdParceiro") as HiddenField;

                if (hiddenIdProduto != null)
                {
                    hiddenIdProduto.Value = produto.idProduto;
                }

                if (hiddenIdParceiro != null)
                {
                    hiddenIdParceiro.Value = produto.idParceiro;
                }
            }
        }
        protected void rptValores_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var chkItem = e.Item.FindControl("chkItem") as CheckBox;

                if (chkItem != null && chkItem.Checked)
                {
                    cmdGerarPedido.Visible = true;
                }
                if (chkItem != null && chkItem.Checked)
                {
                    cmdGerarPedido.Visible = false;
                }
            }
        }
        protected void chkGlobal_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkGlobal = sender as CheckBox;
            RepeaterItem headerItem = chkGlobal.NamingContainer as RepeaterItem;
            Repeater headerRepeater = headerItem.Parent as Repeater;
            int colIndex = headerItem.ItemIndex;

            foreach (RepeaterItem item in headerRepeater.Items)
            {
                if (item != headerItem)
                {
                    CheckBox otherChkGlobal = item.FindControl("chkGlobal") as CheckBox;
                    if (otherChkGlobal != null)
                    {
                        otherChkGlobal.Checked = false;
                    }
                }
            }


            foreach (RepeaterItem item in rptComparativos.Items)
            {
                HiddenField hiddenIdProduto = item.FindControl("hiddenIdProduto") as HiddenField;
                if (!string.IsNullOrEmpty(hiddenIdProduto.Value) && hiddenIdProduto.Value != "0")
                {
                    string idProduto = hiddenIdProduto.Value;

                    var rptValores = item.FindControl("rptValores") as Repeater;
                    if (rptValores != null)
                    {
                        foreach (RepeaterItem valorItem in rptValores.Items)
                        {
                            CheckBox chkItem = valorItem.FindControl("chkItem") as CheckBox;
                            if (chkItem != null)
                            {
                                if (valorItem.ItemIndex == colIndex)
                                {
                                    chkItem.Checked = chkGlobal.Checked;
                                }
                                else
                                {
                                    chkItem.Checked = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    MensagemPaginaComparativo.MostraMensagem("Há produto(os) na seleção que precisam ser cadastrados", "Info", false);
                }
            }

            if (chkGlobal.Checked)
            {
                cmdGerarPedido.Visible = true;
            }
            else
            {
                cmdGerarPedido.Visible = false;
            }
        }
        protected void chkItem_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkItem = sender as CheckBox;
            RepeaterItem valorItem = chkItem.NamingContainer as RepeaterItem;
            Repeater rptValores = valorItem.Parent as Repeater;
            int colIndex = valorItem.ItemIndex;

            RepeaterItem comparativoItem = rptValores.NamingContainer as RepeaterItem;

            HiddenField hiddenIdProduto = comparativoItem.FindControl("hiddenIdProduto") as HiddenField;

            string idProduto = hiddenIdProduto != null ? hiddenIdProduto.Value : "0";

            if (idProduto == "0" || chkItem.Checked)
            {
                if (idProduto == "0")
                {
                    MensagemPaginaComparativo.MostraMensagem("Há produto(os) na seleção que precisam ser cadastrados", "Info", false);
                }
                foreach (RepeaterItem item in rptValores.Items)
                {
                    if (item.ItemIndex != colIndex)
                    {
                        CheckBox chkOtherItem = item.FindControl("chkItem") as CheckBox;
                        if (chkOtherItem != null)
                        {
                            chkOtherItem.Checked = false;
                        }
                    }
                }

                if (idProduto == "0")
                {
                    chkItem.Checked = false;
                }
            }


            if (chkItem.Checked)
            {
                cmdGerarPedido.Visible = true;
            }
            else
            {
                bool algumMarcado = false;

                foreach (RepeaterItem itemComparativo in rptComparativos.Items)
                {
                    Repeater rptValores2 = itemComparativo.FindControl("rptValores") as Repeater;

                    if (rptValores2 != null)
                    {
                        foreach (RepeaterItem itemValor in rptValores2.Items)
                        {
                            CheckBox chkItem2 = itemValor.FindControl("chkItem") as CheckBox;
                            if (chkItem2 != null && chkItem2.Checked)
                            {
                                algumMarcado = true;
                                break;
                            }
                        }
                    }

                    if (algumMarcado)
                    {
                        break;
                    }
                }

                if (!algumMarcado)
                {

                    cmdGerarPedido.Visible = false;
                }
            }
        }
        protected void GerarPedidosDeCompra()
        {
            List<PedidoCompra> pedidos = new List<PedidoCompra>();
            string mensagensErro = "";
            bool validado = true;
            List<string> linksPedidos = new List<string>();

            foreach (RepeaterItem comparativoItem in rptComparativos.Items)
            {
                var hiddenIdProduto = comparativoItem.FindControl("hiddenIdProduto") as HiddenField;
                if (hiddenIdProduto == null) continue;

                string idProduto = hiddenIdProduto.Value;

                var rptValores = comparativoItem.FindControl("rptValores") as Repeater;
                if (rptValores != null)
                {
                    foreach (RepeaterItem valorItem in rptValores.Items)
                    {
                        var chkItem = valorItem.FindControl("chkItem") as CheckBox;
                        if (chkItem != null && chkItem.Checked)
                        {
                            string idParceiro = chkItem.CssClass;

                            var lblPreco = valorItem.FindControl("lblPreco") as Label;
                            var lblPrevisao = valorItem.FindControl("lblPrevisao") as Label;

                            string preco = lblPreco != null ? lblPreco.Text : "0";
                            string previsao = lblPrevisao != null ? lblPrevisao.Text : DBNull.Value.ToString();

                            var pedidoExistente = pedidos.FirstOrDefault(p => p.idParceiro == idParceiro);
                            if (pedidoExistente != null)
                            {
                                pedidoExistente.Produtos.Add(new ProdutoPedido
                                {
                                    idProduto = idProduto,
                                    Preco = preco,
                                    Previsao = previsao
                                });
                            }
                            else
                            {
                                pedidos.Add(new PedidoCompra
                                {
                                    idParceiro = idParceiro,
                                    Produtos = new List<ProdutoPedido>
                                {
                                    new ProdutoPedido
                                    {
                                        idProduto = idProduto,
                                        Preco = preco,
                                        Previsao = previsao
                                    }
                                }
                                });
                            }
                        }
                    }
                }
            }

            foreach (var pedido in pedidos)
            {
                try
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "GERAR-PEDIDO");
                    vParametros.Add("@idParceiro", pedido.idParceiro);
                    vParametros.Add("@idVendedor", ddlComprador.SelectedValue);
                    vParametros.Add("@idUsuarioInclusao", IDENTITY.Variaveis.idUsuario());
                    vParametros.Add("@idCotacao", hddidCotacao.Value);
                    vParametros.Add("@idEmpresa", ddlidEmpresa.SelectedValue);
                    vParametros.Add("@idEndereco", ddlsEnderecoEntrega.SelectedValue);
                    vParametros.Add("@idConceito", ddlConceito.SelectedValue);
                    vParametros.Add("@idGrupoPatrimonio", ddlGrupoPatrimonio.SelectedValue);

                    if (txtdtValidade.Text == "")
                    {
                        vParametros.Add("@dtValidade", txtdtValidade.Text);
                    }
                    else
                    {
                        DateTime dt = DateTime.Parse(txtdtValidade.Text.ToString(), CultureInfo.InvariantCulture);
                        vParametros.Add("@dtValidade", dt.ToString());
                    }

                    DataSet dsSalvar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_CotacaoCompras", vParametros);
                    string sErro = "";

                    if (RETORNO.DATASET(dsSalvar, "nRet") == "1")
                    {
                        validado = false;
                        mensagensErro += $"É Necessário resolver as pendências nesse fornecedor antes de gerar o pedido.";
                    }
                    else if (RETORNO.DATASET(dsSalvar, "nRet") == "2")
                    {
                        validado = false;
                        mensagensErro += RETORNO.DATASET(dsSalvar, "msg");
                    }
                    else
                    {
                        int idPedido = Convert.ToInt32(dsSalvar.Tables[0].Rows[0]["idPedido"]);

                        string linkPedido = $"<a href='../Pedidos_Detalhe.aspx?id={idPedido}&sTp=7' target='_blank'>Pedido {idPedido}</a>";
                        linksPedidos.Add(linkPedido);
                        if (BD.ValidarDataSet(dsSalvar, out sErro) && dsSalvar.Tables[0].Rows.Count > 0)
                        {
                            int idPedidoGerado = Convert.ToInt32(dsSalvar.Tables[0].Rows[0]["idPedido"]);

                            foreach (var produto in pedido.Produtos)
                            {
                                Dictionary<string, string> vParametrosItens = new Dictionary<string, string>();
                                vParametrosItens.Add("@sFuncao", "INCLUIR-ITENS-PEDIDO");
                                vParametrosItens.Add("@idPedido", idPedidoGerado.ToString());
                                vParametrosItens.Add("@idProduto", produto.idProduto);
                                vParametrosItens.Add("@idParceiro", pedido.idParceiro);
                                vParametrosItens.Add("@idCotacao", hddidCotacao.Value);
                                vParametrosItens.Add("@idUsuarioInclusao", IDENTITY.Variaveis.idUsuario());
                                vParametrosItens.Add("@nValorCotado", Conversoes.Numerico(Convert.ToDecimal(produto.Preco)));
                                vParametrosItens.Add("@dtPrevisao", produto.Previsao);

                                DataSet dsSalvarItens = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_CotacaoCompras", vParametrosItens);

                                if (!BD.ValidarDataSet(dsSalvarItens, out sErro))
                                {
                                    validado = false;
                                    mensagensErro += $"Erro ao salvar o item {produto.idProduto} para o parceiro {pedido.idParceiro}: {sErro}. ";
                                }
                            }
                        }
                        else
                        {
                            validado = false;
                            mensagensErro += $"Erro ao gerar o pedido para o parceiro {pedido.idParceiro}: {sErro}. ";
                        }
                    }
                }
                catch (Exception ex)
                {
                    validado = false;
                    mensagensErro += $"Erro inesperado ao gerar o pedido para o parceiro {pedido.idParceiro}: {ex.Message}. ";
                }
            }

            if (validado)
            {
                string mensagemSucesso = "Pedidos gerados com sucesso. Acesse e Valide os pedidos: " + string.Join(", ", linksPedidos);
                MensagemPaginaComparativo.MostraMensagem_Sucesso(mensagemSucesso);
            }
            else
            {
                MensagemPaginaComparativo.MostraMensagem_Erro($"Falha, Detalhes do erro: {mensagensErro}");
            }
        }
        protected void GerarPedido_Click(object sender, EventArgs e)
        {
            if (Session["GerandoPedido"] != null && (bool)Session["GerandoPedido"])
            {
                return;
            }
            try
            {
                Session["GerandoPedido"] = true;

                if (ValidarDadosCotacao(true))
                {
                    GerarPedidosDeCompra();
                }
            }
            finally
            {
                Session["GerandoPedido"] = false;
            }
        }

        protected string FormatarPrecoBrasileiro(object preco)
        {
            if (preco == null || preco == DBNull.Value)
            {
                return string.Empty;
            }

            try
            {
                decimal valor = Convert.ToDecimal(preco);
                CultureInfo culturaBrasileira = new CultureInfo("pt-BR");
                return valor.ToString("N4", culturaBrasileira);
            }
            catch
            {
                return preco.ToString();
            }
        }
        #endregion

        #region | Controle de Modal
        protected void AbrirModal_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlidEmpresa.SelectedValue) || ddlidEmpresa.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Informe uma Empresa na Cotação e depois adicione os fornecedores");
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalEnvio", "$('#modalEnvio').modal('show');", true);
            }
        }
        protected void FecharModal(string modalId)
        {
            string script = $@"
        $('#{modalId}').modal('hide');
        $('.modal-backdrop').remove();
          ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_CloseModal_" + modalId, script, true);
        }
        #endregion

        #region | Método de Gerar Linha na Grid
        public String NovaLinha(object id, string gridNome)
        {
            if (id != null && !string.IsNullOrEmpty(id.ToString()))
            {
                return string.Format(@"</td></tr><tr id='tr{0}{1}' class='collapsed-row'>
                                        <td></td><td colspan='100' style='padding:0px; margin:0px;'>", gridNome, id);
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion

        #region | Excel
        #region | Importar
        protected void cmdImportarProdutos_Modal_Click(object sender, EventArgs e)
        {
            var lembrete = "<b>Lembrete:</b> É Preciso Salvar os Dados Para Validar os Itens da Importação.";
            try
            {
                var itens = ExcelImportar.Retornar_Itens_Excel_Compras(ImportarArquivo);

                if (itens != null)
                {
                    string mensagem = "";

                    foreach (cls_WMS_Produtos item in itens)
                    {
                        Dictionary<string, string> vParam = new Dictionary<string, string>()
                                {
                                    { "@sFuncao", "CONSULTA_PRODUTO_x_CODIGO" },
                                    { "@sCodigo", item.SCodigo.Trim() },
                                    { "@idProduto", item.IdItem.ToString() }
                                };

                        DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos", vParam);

                        if (BD.ValidarDataSet(ds))
                        {
                            string msg = "";

                            try
                            {
                                msg = RETORNO.DATASET(ds, "sMsg");
                            }
                            catch { }

                            if (!string.IsNullOrEmpty(msg))
                                mensagem += string.Format("<br /><br />{0}", msg);
                            else
                            {
                                int.TryParse(RETORNO.DATASET(ds, "idItem"), out int idItem);
                                var produtoExistente = idItem == 0
                                            ? ls_CotacaoItens.FirstOrDefault(p => p.SCodigo.Equals(item.SCodigo))
                                            : ls_CotacaoItens.FirstOrDefault(p => p.IdProduto.Equals(idItem));

                                if (produtoExistente == null)
                                {
                                    var novoProduto = new cls_CotacaoComprasItens
                                    {
                                        SFuncao = "INCLUIR ITEM",
                                        IdProduto = idItem,
                                        SCodigo = item.SCodigo,
                                        SDscProduto = string.IsNullOrEmpty(RETORNO.DATASET(ds, "sDscProduto")) ? "Novo Produto" : RETORNO.DATASET(ds, "sDscProduto"),
                                        SUnidade = string.IsNullOrEmpty(RETORNO.DATASET(ds, "sUnidade")) ? "NA" : RETORNO.DATASET(ds, "sUnidade"),
                                        NQuantidade = item.NQuantidade
                                    };

                                    ls_CotacaoItens.Add(novoProduto);
                                    dtgItens_DataBind(ls_CotacaoItens);
                                }
                                else
                                {
                                    MensagemPaginaItens.MostraMensagem_Erro("Produto já adicionado a Cotação!", false);
                                }

                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(mensagem))
                    {
                        MensagemPaginaItens.MostraMensagem(lembrete, "info", false);
                        MensagemPaginaItens.MostraMensagem_Aviso(string.Format("Alguns Produtos não foram Importados corretamente!<br />{0}", mensagem), false);
                    }
                    else
                    {
                        MensagemPaginaItens.MostraMensagem(lembrete, "info", false);
                        MensagemPaginaItens.MostraMensagem_Sucesso("Todos os Produtos foram Importados com sucesso!", false);
                    }

                }
                else
                    MensagemPaginaItens.MostraMensagem_Sucesso("Não foram encontrados Itens no Arquivo selecionado!", false);
            }
            catch (Exception ex)
            {
                ex.Source = ex.Message;

                if (ex.Message.Contains("header signature:") && ex.Message.Contains("0xE011CFD0"))
                    ex.Source = "Não foi possível ler o arquivo selecionado, este pode estar corrompido, por favor passe as informações dos Itens presentes no Arquivo para um outro Arquivo Excel gerado manualmente e tente novamente!";

                MensagemPaginaItens.MostraMensagem("<b>Lembrete: </b>O arquivo Excel deve estar organizado de forma que a primeira linha será considerada como o Cabeçalho para as colunas,sendo a primeira Coluna representando a Ordem, a segunda Coluna representando o Código e a terceira Coluna representando a Quantidade dos Itens!", "INFO", false);
                MensagemPaginaItens.MostraMensagem_Erro("Houve um erro ao Importar os Produtos ao Orçamento!<br /><b>Erro: </b>" + ex.Source, false);
            }
        }
        #endregion


        #endregion

        #region | Email 
        void EnviarEmail()
        {
            return;
        }
        #endregion

        #region | Conceito e Patrimônio
        protected void ddlConceito_SelectedIndexChanged(object sender, EventArgs e)
        {
            Dictionary<String, String> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR-FLAG-CONCEITO" },
                        { "@idConceito", ddlConceito.SelectedValue }
                    };
            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametros, true);
            if (RETORNO.DATASET(ds, "sExibePatrimonio") == "S")
            {
                divGrupoPatrimonio.Visible = true;
                FUNCOES.Popula_Combo(ddlGrupoPatrimonio, "sp_Select 'Flow_Patrimonio_Grupo'", "idPatrimonioGrupo", "sDscPatrimonio", false, "Selecione um Grupo de Patrimônio.", "0");
            }
            else
            {
                divGrupoPatrimonio.Visible = false;
                ddlGrupoPatrimonio.SelectedValue = "0";
            }

        }

        bool Valida_Conceito()
        {
            Dictionary<String, String> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR-FLAG-CONCEITO" },
                        { "@idConceito", ddlConceito.SelectedValue }
                    };
            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos", vParametros, true);
            if (RETORNO.DATASET(ds, "sExibePatrimonio") == "S")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
    #region | Classes Temporárias
    public class PedidoCompra
    {
        public string idParceiro { get; set; }
        public List<ProdutoPedido> Produtos { get; set; }
    }
    public class ProdutoPedido
    {
        public string idProduto { get; set; }
        public string Preco { get; set; }
        public string Previsao { get; set; }
    }
    public class ComparativoProduto
    {
        public string sDscProduto { get; set; }
        public string idProduto { get; set; }
        public string idParceiro { get; set; }
        public List<ComparativoFornecedor> Valores { get; set; }
    }
    public class ComparativoFornecedor
    {
        public string Fornecedor { get; set; }
        public string Preco { get; set; }
        public string Previsao { get; set; }
        public string idParceiro { get; set; }
    }
    #endregion

}
