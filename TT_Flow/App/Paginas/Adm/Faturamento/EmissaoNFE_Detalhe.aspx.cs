using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using GRID = TT.FrameWork.Grid;
using BD = TT.FrameWork.BD;
using RETORNO = TT.FrameWork.BD.Retorno;
using TT_Flow.App.Controles;
using TT_Flow.FrameWork;
using System.Linq;
using System.Web.UI.WebControls;
using TT.FrameWork;
using System.Xml.Linq;

namespace TT_Flow.App.Paginas.Adm.Faturamento
{
    public partial class EmissaoNFE_Detalhe : Page
    {
        #region | Classes

        string sTituloPagina = "NF-e";
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_EmissaoNFE";
        int idOrigem_Emissor = 5;
        int nColuna_idXML = 0;
        int nColuna_idObjeto = 1;
        int nColuna_sTipo = 3;
        int nColuna_sChave = 6;
        int nColuna_Status = 7;
        int nColuna_nNumeroCartaCorrecao = 10;
        int nColuna_Cancelamento = 11;
        int nColuna_sChaveNFe = 12;

        int nColuna_Item_nOrdem = 0;
        int nColuna_Item_Unidade = 5;
        int nColuna_Item_Unidade_text = 6;
        int nColuna_Item_nQuantidade = 7;
        int nColuna_Item_nQuantidade_text = 8;
        int nColuna_Item_nValorUnitario = 9;
        int nColuna_Item_nValorUnitario_text = 10;
        int nColuna_Item_Total = 11;
        int nColuna_Item_Botao = 12;

        public List<cls_Pedidos_Itens> Base_Itens
        {
            get
            {
                if (ViewState["Base_Itens"] == null)
                {
                    ViewState["Base_Itens"] = new List<FrameWork.cls_Pedidos_Itens>();
                }
                return (List<FrameWork.cls_Pedidos_Itens>)ViewState["Base_Itens"];
            }

            set
            {
                ViewState["Base_Itens"] = value;
            }
        }


        public List<cls_CST> Base_CST
        {
            get
            {
                if (ViewState["Base_CST"] == null)
                {
                    ViewState["Base_CST"] = new List<FrameWork.cls_CST>();
                }
                return (List<FrameWork.cls_CST>)ViewState["Base_CST"];
            }

            set
            {
                ViewState["Base_CST"] = value;
            }
        }

        #endregion

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Administracao.EmissaoNFe.Consultar, true);

            if (!IsPostBack)
            {

                if (Request["id"] != null)
                {
                    Pesquisar(Request["id"].ToString(), "CONSULTA");
                    hddidPedido.Value = Request["id"].ToString();
                }


                else Pesquisar("0", "CONSULTA");


                if (Request["msg"] != null)

                    MensagemPagina.MostraMensagem_Sucesso("Salvo com sucesso!");

            }


            ddlidEmpresa.Titulo = "Empresa";
            ddlidEmpresa.Configurar(false, "/App/Paginas/Manutencao/Empresas_Detalhe.aspx?id=");

            //FiltroPesquisaProdutos.TipoFiltroPesquisa = "6";
            FiltroPesquisaProdutos.Controle_ExibicaoCampos(new Dictionary<string, bool> { { "Unidade", true } }, "1");
            FiltroPesquisaProdutos.ModificaTamanhoCampos(2, 5, 1, 2, 2);
            FiltroPesquisaProdutos.sRegistraUnidade = "S";
            FiltroPesquisaProdutos.desligaColapso(false);
            RegistraScript("");
        }
        #endregion

        #region | Metodos Banco de Dados

        protected void Pesquisar(string idPedido, string sFuncaoPagina)
        {
            string sErro = "";
            try
            {

                PopulaCombo();

                if (idPedido == "0")
                    sFuncaoPagina = "Novo";

                hddsFuncaoPagina.Value = sFuncaoPagina;
                aba_Configuracoes.Visible = false;
                aba_NFe.Visible = false;
                DIV_EnderecoEntrega.Visible = false;
                div_txtIE.Visible = false;
                div_txtContribuinteICMS.Visible = false;
                DIV_Transportadora.Visible = false;
                DIV_Volumes.Visible = false;
                DIV_Especie.Visible = false;
                DIV_Liquido.Visible = false;
                DIV_Bruto.Visible = false;
                cmdEditar.Visible = false;
                cmdSalvar.Visible = false;
                cmdGerarPiloto.Visible = false;
                if (idPedido != "0")
                {
                    Base_Itens.Clear();
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>
                    {
                        { "@sfuncao", "CONSULTAR_DETALHE" },
                        { "@idPedido", idPedido }
                    };
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        txtdtPedido.Text = RETORNO.DATASET(dsPesquisa, 0, 0, "dtPedido");
                        txtsReferencia.Text = RETORNO.DATASET(dsPesquisa, 0, 0, "sReferencia");
                        txtsControleTT.Text = RETORNO.DATASET(dsPesquisa, 0, 0, "nControleTT");
                        ddlCliente.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, 0, "idCliente");
                        txtIE.Text = RETORNO.DATASET(dsPesquisa, 0, 0, "sRG_IE");
                        txtContribuinteICMS.Text = RETORNO.DATASET(dsPesquisa, 0, 0, "sContribuinte") == "S" ? "Sim" : "Não";
                        ddlsDestinoVenda.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, 0, "sDestinoVenda");
                        hddsCPF_CNPJ.Value = RETORNO.DATASET(dsPesquisa, 0, 0, "sCPF_CNPJ");
                        if (RETORNO.DATASET(dsPesquisa, 0, 0, "idEnderecoEntrega") != "0")
                        {
                            FUNCOES.Popula_Combo(ddlsEnderecoEntrega, string.Format("sp_Manipula_tbl_Flow_Clientes 'CONSULTAR_ENDERECO', {0}", ddlCliente.SelectedValue), "idEndereco", "sEnderecoCompleto", false);
                            DIV_EnderecoEntrega.Visible = true;
                            ddlsEnderecoEntrega.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, 0, "idEnderecoEntrega");
                        }
                        ddlidEmpresa.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, 0, "idEmpresa");
                        decimal frete = decimal.Parse(RETORNO.DATASET(dsPesquisa, 0, 0, "nFretePrevisto"));
                        txtnFrete.Text = frete.ToString("N2");
                        decimal Produto = decimal.Parse(RETORNO.DATASET(dsPesquisa, 0, 0, "nVlrProdutos"));
                        txtnVlrProdutos.Text = Produto.ToString("N2");
                        txtnVlrTotal.Text = (Produto + frete).ToString("N2");
                        txtsObservacao.Text = RETORNO.DATASET(dsPesquisa, 0, 0, "sObservacao");
                        txtsStatus.Text = RETORNO.DATASET(dsPesquisa, 0, 0, "sStatusNota");
                        txtdtEntrada_Saida.Text = DateTime.TryParse(RETORNO.DATASET(dsPesquisa, "dtEntrada_Saida"), out DateTime dt) ? dt.ToString("yyyy-MM-dd HH:mm:ss") : "";

                        hddnNumeroPedido.Value = RETORNO.DATASET(dsPesquisa, 0, 0, "nNumeroPedido");
                        hddidEmpresa.Value = RETORNO.DATASET(dsPesquisa, 0, 0, "idEmpresa");

                        ddlsTipo.SelectedValue = RETORNO.DATASET(dsPesquisa, 2, 0, "tpNF");
                        ddlsTipo_SelectedIndexChanged(null, null);
                        ddlidCFOP.SelectedValue = RETORNO.DATASET(dsPesquisa, 1, 0, "idCFOP");
                        txtsNaturezadaOperacao.Text = RETORNO.DATASET(dsPesquisa, 2, 0, "sNaturezaOperacao");

                        string refNFe = RETORNO.DATASET(dsPesquisa, 2, 0, "refNFe");
                        ddlsChaveNFe_Referencia.SelectedValue = (refNFe == "") ? "N" : "S";
                        ddlsChaveNFe_Referencia_SelectedIndexChanged(null, null);
                        txtsChaveNFe_Referencia.Text = refNFe;


                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtUltimaAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sdscUsuarioAtualizacao"));

                        foreach (DataRow linha in dsPesquisa.Tables[1].Rows)
                        {
                            FrameWork.cls_Pedidos_Itens objItem = new FrameWork.cls_Pedidos_Itens();
                            objItem.nOrdem = int.Parse(linha["nOrdem"].ToString());
                            objItem.idItem = int.Parse(linha["idItem"].ToString());
                            objItem.idProduto = int.Parse(linha["idProduto"].ToString());
                            objItem.sFuncao = "SALVAR_ITENS";
                            objItem.sCodigoProduto = linha["sCodigo"].ToString();
                            objItem.sUnidade = linha["sUnidade"].ToString();
                            objItem.sDscProduto = linha["sDscProduto"].ToString();
                            objItem.nQuantidade = Math.Round(Convert.ToDouble(linha["nQuantidade"]), 4);
                            objItem.nValorUnitario = Math.Round(Convert.ToDouble(linha["nValorUnitario"]), 4);
                            objItem.nValorTotal = Math.Round(Convert.ToDouble(linha["nValorTotal"]), 2);
                            objItem.idCST_ICMS = int.Parse(linha["idCST_ICMS"].ToString());
                            objItem.idCST_cBenef = int.Parse(linha["idCST_cBenef"].ToString());
                            objItem.nPercReducaoBC = decimal.Parse(linha["nPercReducaoBC"].ToString());
                            objItem.sPedidoCliente = linha["sPedidoCliente"].ToString();
                            Base_Itens.Add(objItem);

                            DataBind_gvItens();
                        }

                        ddltranporte.SelectedValue = RETORNO.DATASET(dsPesquisa, 2, 0, "idModoFrete");
                        if (ddltranporte.SelectedValue != "" && ddltranporte.SelectedValue != "9")
                        {
                            DIV_Transportadora.Visible = true;
                            DIV_Volumes.Visible = true;
                            DIV_Especie.Visible = true;
                            DIV_Liquido.Visible = true;
                            DIV_Bruto.Visible = true;
                            ddlidTranportadora.SelectedValue = RETORNO.DATASET(dsPesquisa, 2, 0, "idTransportadora");
                            txtVolumes.Text = RETORNO.DATASET(dsPesquisa, 2, 0, "sVolumes");
                            txtEspecie.Text = RETORNO.DATASET(dsPesquisa, 2, 0, "sEspecie");
                            txtPesoLiquido.Text = RETORNO.DATASET(dsPesquisa, 2, 0, "sPesoLiquido");
                            txtPesoBruto.Text = RETORNO.DATASET(dsPesquisa, 2, 0, "sPesoBruto");

                        }
                        txtInfoFisco.Text = RETORNO.DATASET(dsPesquisa, 2, 0, "sInfoFisco");
                        txtInfoComplementares.Text = RETORNO.DATASET(dsPesquisa, 2, 0, "sInfoComplementares");
                        hddsEmissaoNfe.Value = RETORNO.DATASET(dsPesquisa, 2, 0, "sEmissaoNfe");
                        hddidArquivo.Value = RETORNO.DATASET(dsPesquisa, 2, 0, "idArquivo");
                        hddidXML.Value = RETORNO.DATASET(dsPesquisa, 2, 0, "idXML");
                        hddchNFe.Value = RETORNO.DATASET(dsPesquisa, 2, 0, "chNFe");
                        hddstatusNota.Value = RETORNO.DATASET(dsPesquisa, 2, 0, "statusNota");
                        hddnCartaCorrecao.Value = RETORNO.DATASET(dsPesquisa, 2, 0, "nCartaCorrecao");
                        hddsCartaCorrecao.Value = RETORNO.DATASET(dsPesquisa, 2, 0, "sCartaCorrecao");

                        hddsCaminho_UniNFe.Value = RETORNO.DATASET(dsPesquisa, 4, 0, "sCaminho_UniNFe");

                        if (sFuncaoPagina == "CONSULTA")
                        {
                            ConfiguraCampos(false);
                        }
                        else
                        {
                            ConfiguraCampos(true);
                        }
                        lblTituloPagina.Text = "Emissor NF-e - " + "N.° " + hddnNumeroPedido.Value + " - Referência: " + txtsReferencia.Text;


                        if (string.IsNullOrEmpty(RETORNO.DATASET(dsPesquisa, 4, 0, "idxml")))
                        {
                            cmdEditar.Visible = true;
                            cmdGerarPiloto.Visible = true;
                        }
                        else
                        {
                            aba_NFe.Visible = true;
                            Carregar_DocumentosFiscais(dsPesquisa.Tables[4]);
                        }

                    }
                    else
                    {
                        throw new Exception(sErro);
                    }
                }
                else
                {
                    BreadCrumb.TitulodaPagina = string.Format("Nova {0}", sTituloPagina);
                    lblTituloPagina.Text = string.Format("Emissor NF-e - Nova {0}", sTituloPagina);
                    txtdtPedido.Text = DateTime.Today.ToString("dd/MM/yyyy");
                    txtnFrete.Text = "0,00";
                    ddlsChaveNFe_Referencia_SelectedIndexChanged(null, null);
                    ConfiguraCampos(true);
                    cmdSalvar.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }

        void Carregar_DocumentosFiscais(DataTable dt)
        {
            gvDocumentosFiscais.DataSource = dt;
            gvDocumentosFiscais.DataBind();

        }
        protected void cmdSalvar_Click(object sender, EventArgs e)
        {

            if (Validar())
            {
                lblTitulos_AcoesNFe.Text = "Salvar NF-e";
                if (hddidPedido.Value == "")
                {

                    lblTituloJustificativa_AcoesNFe.Text = "Confirma a Inclusão da NFe?";
                }
                else
                {
                    lblTituloJustificativa_AcoesNFe.Text = "Confirma a Alteração da NFe?";
                }
                div_Modal.Style.Add("width", "35%");
                hddsAcaoNFe.Value = "SALVAR";
                txtsJustificativa_AcoesNFe.Visible = false;

                ScriptManager.RegisterStartupScript(this, this.GetType(), "Confirma_SEFAZ", "$('#Modal_AcoesNFe').modal('show');", true);

            }

        }

        void Salvar_Detalhe()
        {
            try
            {
                string textoCompleto = ddlidTranportadora.SelectedItem.Text;
                string cnpj = textoCompleto.Split('-')[0].Trim();
                DateTime dtEntrada_Saida = DateTime.TryParse(txtdtEntrada_Saida.Text, out DateTime dt) ? dt : default;

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sfuncao", "SALVAR" },
                    { "@idPedido", hddidPedido.Value },
                    { "@sReferencia", txtsReferencia.Text },
                    { "@nControleTT", txtsControleTT.Text },
                    { "@idTipo", "8" },
                    { "@idCliente", ddlCliente.SelectedValue },
                    { "@idEnderecoEntrega", ddlsEnderecoEntrega.SelectedValue },
                    { "@idEmpresa", ddlidEmpresa.SelectedValue },
                    { "@sEmpresaTransporte", cnpj },
                    { "@nFretePrevisto", txtnFrete.Text.Replace(".", "").Replace(",", ".") },
                    { "@nVlrProdutos", txtnVlrProdutos.Text.Replace(".", "").Replace(",", ".") },
                    { "@sObservacao", txtsObservacao.Text },
                    { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() },
                    { "@idImportarItem", hddidImportarItem.Value },
                    { "@sImportarTipo", hddImportarTipo.Value },
                    { "@sDestinoVenda", ddlsDestinoVenda.SelectedValue },
                    { "@dtEntrada_Saida", dtEntrada_Saida == default ? null : dtEntrada_Saida.ToString("yyyy-MM-ddTHH:mm:ss") }
                };
                DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsPesquisa, out _))
                {
                    hddidPedido.Value = RETORNO.DATASET(dsPesquisa, "idPedido");
                    SalvaItens(hddidPedido.Value);
                    SalvaTransporte(hddidPedido.Value);
                    FUNCOES.DirecionaPagina("App/Paginas/Adm/Faturamento/EmissaoNFE_Detalhe.aspx?id=" + hddidPedido.Value + "&msg=S");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }

        void SalvaItens(string idPedido)
        {
            try
            {
                txtnValorUnitario_TextChanged(null, null);
                foreach (var linha in Base_Itens)
                {
                    if (linha.sFuncao == "SALVAR_ITENS" || linha.sFuncao == "EXCLUIR ITEM")
                    {
                        DataSet dsPesquisa;
                        Dictionary<String, String> vParametros = new Dictionary<string, string>
                        {
                            { "@sfuncao", linha.sFuncao },
                            { "@idItem", linha.idItem.ToString() },
                            { "@idPedido", idPedido },
                            { "@idProduto", linha.idProduto.ToString() },
                            { "@sCodigo", linha.sCodigoProduto },
                            { "@sDscProduto", linha.sDscProduto },
                            { "@sUnidade", linha.sUnidade },
                            { "@nQuantidade", linha.nQuantidade.ToString().Replace(".", "").Replace(",", ".") },
                            { "@nValorUnitario", linha.nValorUnitario.ToString().Replace(".", "").Replace(",", ".") },
                            { "@nValorTotal", linha.nValorTotal.ToString().Replace(".", "").Replace(",", ".") },
                            { "@idUsuarioInclusao", IDENTITY.Variaveis.idUsuario() },
                            { "@nOrdem", linha.nOrdem.ToString() },
                            { "@idCFOP", ddlidCFOP.SelectedValue },
                            { "@idCST_ICMS", string.IsNullOrEmpty(linha.idCST_ICMS.ToString()) ? "0" : linha.idCST_ICMS.ToString() },
                            { "@idCST_cBenef", string.IsNullOrEmpty(linha.idCST_cBenef.ToString()) ? "0" : linha.idCST_cBenef.ToString() },
                            { "@nPercReducaoBC", string.IsNullOrEmpty(linha.nPercReducaoBC.ToString()) ? "0" : BD.Conversoes.Numerico(linha.nPercReducaoBC) },
                            { "@sPedidoCliente", linha.sPedidoCliente }
                        };


                        dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }

        void SalvaTransporte(string idPedido)
        {


            try
            {
                if (ddlsChaveNFe_Referencia.SelectedValue == "N")

                    txtsChaveNFe_Referencia.Text = "";

                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sfuncao", "SALVAR_TRANS" },
                    { "@idModoFrete", ddltranporte.SelectedValue },
                    { "@idTransportadora", ddlidTranportadora.SelectedValue },
                    { "@sVolumes", string.IsNullOrEmpty(txtVolumes.Text) ? "0" : txtVolumes.Text },
                    { "@sEspecie", txtEspecie.Text },
                    { "@sPesoLiquido", BD.Conversoes.Numerico(txtPesoLiquido) },
                    { "@sPesoBruto", BD.Conversoes.Numerico(txtPesoBruto) },
                    { "@sInfoFisco", txtInfoFisco.Text },
                    { "@tpnf", ddlsTipo.SelectedValue },
                    { "@sInfoComplementares", txtInfoComplementares.Text },
                    { "@sNaturezaOperacao", txtsNaturezadaOperacao.Text },
                    { "@refNFe", txtsChaveNFe_Referencia.Text },
                    { "@idPedido", idPedido }
                };
                DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region | Script 
        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("$('[id*=FT_txtnValor]').mask('0.000.000.000,9999', { reverse: true });");
            sb.Append("$('[id*=FT_txtnQuantidade]').mask('000000000000000,0000', { reverse: true });");
            sb.Append("$('[id*=txtnFrete]').mask('0.000.000.009,99', { reverse: true });");
            sb.Append("$('[id*=txtnVlrProdutos]').mask('0.000.000.009,99', { reverse: true });");
            sb.Append("$('[id*=txtnVlrTotal]').mask('0.000.000.009,99', { reverse: true });");
            sb.Append("$('[id*=txtVolumes]').mask('000000000999', { reverse: true });");
            sb.Append("$('[id*=txtPesoLiquido]').mask('0.000.000.009,999', { reverse: true });");
            sb.Append("$('[id*=txtPesoBruto]').mask('0.000.000.009,999', { reverse: true });");
            sb.Append("$('[id*=txtnValorUnitario]').mask('0.000.000.009,9999', { reverse: true });");
            sb.Append("$('[id*=txtnQuantidade]').mask('0.000.000.009,9999', { reverse: true });");
            sb.Append("$('[id*=txtsChaveNFe_Referencia]').mask('00000000000000000000000000000000000000000000', { reverse: true });");
            sb.Append("$('[id*=txtReducaoBC_nPercReducaoBC]').mask('009,99', { reverse: true });");



            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina" + Guid.NewGuid(), sb.ToString(), true);
        }
        #endregion

        #region | Evento
        void ConfiguraCampos(bool bAtivo)
        {
            string sFuncao_Disabled = "disabled";

            if (bAtivo) { sFuncao_Disabled = "enabled"; }

            try
            {
                txtsReferencia.ReadOnly = !bAtivo;
                txtsControleTT.ReadOnly = !bAtivo;
                txtnFrete.ReadOnly = !bAtivo;
                txtsNaturezadaOperacao.ReadOnly = !bAtivo;
                txtVolumes.ReadOnly = !bAtivo;
                txtEspecie.ReadOnly = !bAtivo;
                txtPesoLiquido.ReadOnly = !bAtivo;
                txtPesoBruto.ReadOnly = !bAtivo;
                txtInfoFisco.ReadOnly = !bAtivo;
                txtInfoComplementares.ReadOnly = !bAtivo;
                txtsObservacao.ReadOnly = !bAtivo;
                txtsChaveNFe_Referencia.ReadOnly = !bAtivo;
                txtdtEntrada_Saida.ReadOnly = !bAtivo;

                if (bAtivo)
                {
                    ddlsTipo.Attributes.Remove("disabled");
                    ddlidCFOP.Attributes.Remove("disabled");
                    ddlCliente.Attributes.Remove("disabled");
                    ddlsEnderecoEntrega.Attributes.Remove("disabled");
                    ddlidEmpresa.Atributos.Remove("disabled");
                    ddltranporte.Attributes.Remove("disabled");
                    ddlidTranportadora.Attributes.Remove("disabled");
                    ddlsChaveNFe_Referencia.Attributes.Remove("disabled");
                    ddlsDestinoVenda.Attributes.Remove("disabled");
                }
                else
                {
                    ddlsTipo.Attributes.Add(sFuncao_Disabled, sFuncao_Disabled);
                    ddlidCFOP.Attributes.Add(sFuncao_Disabled, sFuncao_Disabled);
                    ddlCliente.Attributes.Add(sFuncao_Disabled, sFuncao_Disabled);
                    ddlsEnderecoEntrega.Attributes.Add(sFuncao_Disabled, sFuncao_Disabled);
                    ddlidEmpresa.Atributos.Add(sFuncao_Disabled, sFuncao_Disabled);
                    ddltranporte.Attributes.Add(sFuncao_Disabled, sFuncao_Disabled);
                    ddlidTranportadora.Attributes.Add(sFuncao_Disabled, sFuncao_Disabled);
                    ddlsChaveNFe_Referencia.Attributes.Add(sFuncao_Disabled, sFuncao_Disabled);
                    ddlsDestinoVenda.Attributes.Add(sFuncao_Disabled, sFuncao_Disabled);
                }

                cmdImportarItens.Visible = bAtivo;

                div_Ordem.Visible = bAtivo;
                DIV_FiltroPesquisaProdutos.Visible = bAtivo;
                DIV_IncluirItem.Visible = bAtivo;

                PainelAtualizacao.Visible = !bAtivo;

                this.gvItens.Columns[nColuna_Item_Unidade].Visible = bAtivo;
                this.gvItens.Columns[nColuna_Item_Unidade_text].Visible = !bAtivo;
                this.gvItens.Columns[nColuna_Item_nQuantidade].Visible = bAtivo;
                this.gvItens.Columns[nColuna_Item_nQuantidade_text].Visible = !bAtivo;
                this.gvItens.Columns[nColuna_Item_nValorUnitario].Visible = bAtivo;
                this.gvItens.Columns[nColuna_Item_nValorUnitario_text].Visible = !bAtivo;
                this.gvItens.Columns[nColuna_Item_Botao].Visible = true;



            }
            catch (Exception)
            {

                throw;
            }

        }

        private bool Validar()
        {
            string sMensagem = "";


            if (ddlidEmpresa.SelectedValue == "0")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione a empresa!";
            }


            if (txtsReferencia.Text == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "A Referência deve ser preenchida!";
            }

            if (ddlsTipo.SelectedValue == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Tipo!";
            }

            if (ddlidCFOP.SelectedValue == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o CFOP!";
            }

            if (txtsNaturezadaOperacao.Text == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Informe a Natureza da Operação";
            }


            if (ddlCliente.SelectedValue == "0")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Destinatáro!";
            }

            if (ddlsEnderecoEntrega.SelectedValue == "0")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o endereço de entrega!";
            }

            if (ddltranporte.SelectedValue == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o modo de frete!";
            }

            if (ddltranporte.SelectedValue != "" && ddltranporte.SelectedValue != "9")
            {
                if (ddlidTranportadora.SelectedValue == "0")
                {
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione a transportadora!";
                }
                if (txtVolumes.Text == "")
                {
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "A quantidade de volumes deve ser preenchida!";
                }
                if (txtEspecie.Text == "")
                {
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "A espécie deve ser preenchida!";
                }
                if (txtPesoLiquido.Text == "")
                {
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "O peso líquido deve ser preenchido!";
                }
                if (txtPesoBruto.Text == "")
                {
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "O peso bruto deve ser preenchido!";
                }
            }
            if (ddlsChaveNFe_Referencia.SelectedValue == "S")
            {
                if ((Validacoes.ValidarTexto(txtsChaveNFe_Referencia)) || txtsChaveNFe_Referencia.Text.Length < 44)
                {
                    sMensagem += (sMensagem != "" ? "</br>" : "") + "O Chave NFe referenciada inválida!";
                }
            }


            if (Base_Itens.Count == 0)
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Adicione itens!";
            }
            else
            {
                int nLinha = 0;
                foreach (GridViewRow linha in gvItens.Rows)
                {
                    string sUnidade = (linha.Cells[nColuna_Item_Unidade].FindControl("ddlsUnidade") as DropDownList).SelectedValue;

                    if (decimal.Parse(linha.Cells[nColuna_Item_Total].Text) == 0)
                    {
                        gvItens.Rows[nLinha].CssClass = "warning";
                        sMensagem += (sMensagem != "" ? "</br>" : "") + string.Format("Item {0} - <b>{1}</b>, está com valor unitário R$ 0,00!", linha.Cells[0].Text, linha.Cells[4].Text);
                    }
                    else if (sUnidade == "")
                    {

                        gvItens.Rows[nLinha].CssClass = "warning";
                        sMensagem += (sMensagem != "" ? "</br>" : "") + string.Format("Item {0} - <b>{1}</b>, está sem Unidade!", linha.Cells[0].Text, linha.Cells[4].Text);
                    }

                    else
                    {
                        gvItens.Rows[nLinha].CssClass = "";
                    }
                    nLinha++;
                }

            }

            if (DateTime.TryParse(txtdtEntrada_Saida.Text, out DateTime dt) && dt < DateTime.Now)
                sMensagem += (sMensagem != "" ? "</br>" : "") + "A Data de Entrada/Saída deve ser uma data <b>futura</b> válida!";

            if (sMensagem != "")
            {
                MensagemPagina.MostraMensagem_Erro(sMensagem);
                return false;
            }
            return true;
        }

        void PopulaCombo()
        {
            FUNCOES.Popula_Combo(ddlCliente, "sp_Select 'Flow_Adm_Fiscal'", "idParceiro", "sRazaoSocial", false, "Selecione o Destinatário", "0");
            ddlidEmpresa.Popula_Combo("sp_Select 'Flow_Empresa', @idUsuario=" + IDENTITY.Variaveis.idUsuario(), "idEmpresa", "sDscCodigoEmpresa", false, "Selecione a Empresa", "0");
            FUNCOES.Popula_Combo(ddlidTranportadora, "sp_Select 'Flow_Parceiro_Transportadora'", "idParceiro", "sRazaoSocial", false, "Selecione a Transportadora", "0");
            Funcoes.Popula_Combo(BaseddlsUnidade, "sp_Select 'Flow_Produtos_Unidade', @sPesquisa = 'N'", "sUnidade", "sUnidade", false, "Selecione ", "");
        }

        protected void ddlCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            DIV_EnderecoEntrega.Visible = false;
            div_txtIE.Visible = false;
            div_txtContribuinteICMS.Visible = false;

            if (ddlCliente.SelectedValue != "0")
            {
                DIV_EnderecoEntrega.Visible = true;
                div_txtIE.Visible = true;
                div_txtContribuinteICMS.Visible = true;

                FUNCOES.Popula_Combo(ddlsEnderecoEntrega, string.Format("sp_Manipula_tbl_Flow_Clientes 'CONSULTAR_ENDERECO', {0}", ddlCliente.SelectedValue), "idEndereco", "sEnderecoCompleto", false);

                Dictionary<string, string> vParam = new Dictionary<string, string>
                {
                    { "@sfuncao", "CONSULTA_INFO_DEST" },
                    { "@idCliente", ddlCliente.SelectedValue }
                };
                DataSet ds = BD.ExecutarDataSet(sProcedure, vParam);
                txtIE.Text = RETORNO.DATASET(ds, "sRG_IE");
                txtContribuinteICMS.Text = RETORNO.DATASET(ds, "sContribuinteICMS") == "S" ? "Sim" : "Não";
            }
        }

        protected void Item_cmdIncluirItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidarItem())
                {
                    FrameWork.cls_Pedidos_Itens objEnvios = new FrameWork.cls_Pedidos_Itens();
                    objEnvios.nOrdem = Base_Itens.Count + 1;
                    objEnvios.idItem = 0;
                    objEnvios.idProduto = int.Parse(FiltroPesquisaProdutos.IdItem);
                    objEnvios.sFuncao = "SALVAR_ITENS";
                    objEnvios.sCodigoProduto = FiltroPesquisaProdutos.SCodigo;
                    objEnvios.sDscProduto = FiltroPesquisaProdutos.SDscProduto;
                    objEnvios.sUnidade = FiltroPesquisaProdutos.SUnidade;
                    objEnvios.nQuantidade = Math.Round(Convert.ToDouble(FiltroPesquisaProdutos.NQuantidade), 4);
                    objEnvios.nValorUnitario = Math.Round(Convert.ToDouble(FiltroPesquisaProdutos.NValorProduto), 4);
                    decimal Vlr = FiltroPesquisaProdutos.NValorProduto * FiltroPesquisaProdutos.NQuantidade;
                    objEnvios.nValorTotal = Math.Round(Convert.ToDouble(Vlr), 2);
                    Base_Itens.Add(objEnvios);

                    DataBind_gvItens();
                    FiltroPesquisaProdutos.LimparCampos();
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_Itens.MostraMensagem_Erro(ex.Message);
            }
        }

        private bool ValidarItem()
        {
            string sMensagem = "";

            if (string.IsNullOrEmpty(FiltroPesquisaProdutos.IdItem) || FiltroPesquisaProdutos.IdItem == "0")
                sMensagem += (sMensagem != "" ? "</br>" : "") + "<b>Erro:</b> É necessário selecionar um Produto!";
            if (FiltroPesquisaProdutos.SDscProduto.Length <= 0)

                sMensagem += (sMensagem != "" ? "</br>" : "") + "<b>Erro:</b> A Descrição do Produto deve ser preenchida!";

            if (FiltroPesquisaProdutos.SCodigo.Length <= 0)

                sMensagem += (sMensagem != "" ? "</br>" : "") + "<b>Erro:</b> O Código do Produto deve estar preenchido!";

            if (FiltroPesquisaProdutos.NQuantidade.ToString() == "0")

                sMensagem += (sMensagem != "" ? "</br>" : "") + "<b>Erro:</b> A Quantidade do Produto deve ser preenchida!";

            if (FiltroPesquisaProdutos.SUnidade.ToString() == "")

                sMensagem += (sMensagem != "" ? "</br>" : "") + "<b>Erro:</b> Selecione a Unidade!";

            if (FiltroPesquisaProdutos.NValorProduto.ToString() == "0")

                sMensagem += (sMensagem != "" ? "</br>" : "") + "<b>Erro:</b> O Valor Unitário do Produto deve estar preenchido!";

            if (sMensagem != "")
            {
                MensagemPagina_Itens.MostraMensagem_Erro(sMensagem, false);
                return false;
            }

            return true;
        }

        void DataBind_gvItens()
        {
            gvItens.DataSource = Base_Itens.Where(c => c.sFuncao.ToString() != "EXCLUIR ITEM").ToList();
            gvItens.DataBind();

            decimal Produtos = 0;
            decimal frete = 0;
            foreach (GridViewRow linha in gvItens.Rows)
            {
                Produtos += decimal.Parse(linha.Cells[nColuna_Item_Total].Text);
            }
            txtnVlrProdutos.Text = Produtos.ToString("N2");
            if (txtnFrete.Text != "")
                frete = decimal.Parse(txtnFrete.Text);
            txtnVlrTotal.Text = (Produtos + frete).ToString("N2");
        }


        protected void txtnFrete_TextChanged(object sender, EventArgs e)
        {
            decimal nFrete = decimal.Parse(txtnFrete.Text);
            decimal nVlrProdutos = 0;
            if (txtnVlrProdutos.Text != "")
                nVlrProdutos = decimal.Parse(txtnVlrProdutos.Text);

            decimal nVlrTotal = nFrete + nVlrProdutos;
            txtnVlrTotal.Text = nVlrTotal.ToString("N2");
        }


        protected void ddltranporte_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddltranporte.SelectedValue == "" || ddltranporte.SelectedValue == "9")
            {
                DIV_Transportadora.Visible = false;
                DIV_Volumes.Visible = false;
                DIV_Especie.Visible = false;
                DIV_Liquido.Visible = false;
                DIV_Bruto.Visible = false;
                ddlidTranportadora.SelectedValue = "0";
                txtVolumes.Text = "0";
                txtEspecie.Text = "";
                txtPesoLiquido.Text = "0";
                txtPesoBruto.Text = "0";
            }
            if (ddltranporte.SelectedValue != "" && ddltranporte.SelectedValue != "9")
            {
                DIV_Transportadora.Visible = true;
                DIV_Volumes.Visible = true;
                DIV_Especie.Visible = true;
                DIV_Liquido.Visible = true;
                DIV_Bruto.Visible = true;
            }
        }


        #endregion

        protected void txtnValorUnitario_TextChanged(object sender, EventArgs e)
        {
            decimal totalProdutos = 0;
            decimal nTotalFrete = 0;
            foreach (GridViewRow row in gvItens.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    int ordem = int.Parse(row.Cells[nColuna_Item_nOrdem].Text);
                    double Quantidade = double.Parse((row.Cells[nColuna_Item_nQuantidade].FindControl("txtnQuantidade") as TextBox).Text);
                    double Unitario = double.Parse((row.Cells[nColuna_Item_nValorUnitario].FindControl("txtnValorUnitario") as TextBox).Text);
                    double Total = double.Parse(row.Cells[nColuna_Item_Total].Text);
                    string sUnidade = (row.Cells[nColuna_Item_Unidade].FindControl("ddlsUnidade") as DropDownList).SelectedValue;
                    Total = Unitario * Quantidade;

                    row.Cells[nColuna_Item_Total].Text = Total.ToString("N2");

                    totalProdutos += decimal.Parse(Total.ToString("N2"));
                    txtnVlrProdutos.Text = totalProdutos.ToString("N2");
                    if (txtnFrete.Text != "")
                        nTotalFrete = decimal.Parse(txtnFrete.Text);

                    txtnVlrTotal.Text = (totalProdutos + nTotalFrete).ToString("N2");



                    foreach (var linha in Base_Itens)
                    {

                        if (ordem == linha.nOrdem)
                        {
                            linha.nValorUnitario = Math.Round(Unitario, 4);
                            linha.nQuantidade = Math.Round(Quantidade, 4);
                            linha.nValorTotal = Math.Round(Total, 2);
                            linha.sUnidade = sUnidade;
                        }
                    }
                }
            }
        }

        protected void ddlsTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            FUNCOES.Popula_Combo(ddlidCFOP, "sp_Manipula_tbl_Flow_Adm_CFOP @sFuncao='SELECT', @tpNF=" + ddlsTipo.SelectedValue, "idCFOP", "sCFOP", false, "Selecione o CFOP", "0");
        }

        protected void gvDocumentosFiscais_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                LinkButton DocumentosFiscais_cmdGerarNFe = e.Row.FindControl("DocumentosFiscais_cmdGerarNFe") as LinkButton;
                LinkButton DocumentosFiscais_cmdDownloadDANFE = e.Row.FindControl("DocumentosFiscais_cmdDownloadDANFE") as LinkButton;
                LinkButton DocumentosFiscais_cmdDownloadXML = e.Row.FindControl("DocumentosFiscais_cmdDownloadXML") as LinkButton;
                LinkButton DocumentosFiscais_cmdCartaCorrecao = e.Row.FindControl("DocumentosFiscais_cmdCartaCorrecao") as LinkButton;
                LinkButton DocumentosFiscais_cmdDownloadCCePDF = e.Row.FindControl("DocumentosFiscais_cmdDownloadCCePDF") as LinkButton;
                LinkButton DocumentosFiscais_cmdDownloadCCeXML = e.Row.FindControl("DocumentosFiscais_cmdDownloadCCeXML") as LinkButton;
                LinkButton DocumentosFiscais_cmdCancelarNFE = e.Row.FindControl("DocumentosFiscais_cmdCancelarNFE") as LinkButton;

                DocumentosFiscais_cmdGerarNFe.Visible = false;
                DocumentosFiscais_cmdDownloadDANFE.Visible = false;
                DocumentosFiscais_cmdDownloadXML.Visible = false;
                DocumentosFiscais_cmdCartaCorrecao.Visible = false;
                DocumentosFiscais_cmdDownloadCCePDF.Visible = false;
                DocumentosFiscais_cmdDownloadCCeXML.Visible = false;
                DocumentosFiscais_cmdCancelarNFE.Visible = false;


                switch (e.Row.Cells[nColuna_Status].Text)
                {
                    case "Espelho":
                        DocumentosFiscais_cmdGerarNFe.Visible = true;
                        DocumentosFiscais_cmdCancelarNFE.Visible = true;
                        DocumentosFiscais_cmdDownloadXML.Visible = true;
                        DocumentosFiscais_cmdDownloadDANFE.Visible = true;
                        DocumentosFiscais_cmdDownloadDANFE.Text = "Espelho";
                        DocumentosFiscais_cmdCancelarNFE.Text = "Excluir";
                        break;

                    case "Autorizada":

                        if (e.Row.Cells[nColuna_sTipo].Text == "NF-e")
                        {
                            DocumentosFiscais_cmdDownloadXML.Visible = true;
                            DocumentosFiscais_cmdDownloadDANFE.Visible = true;
                            if (e.Row.Cells[nColuna_Cancelamento].Text == "N")
                            {
                                //DocumentosFiscais_cmdEnviarEmail.Visible = true;
                                DocumentosFiscais_cmdCartaCorrecao.Visible = true;
                                //DocumentosFiscais_cmdDevolucao.Visible = true;
                                DocumentosFiscais_cmdCancelarNFE.Visible = true;
                            }
                        }

                        else if (e.Row.Cells[nColuna_sTipo].Text == "CC-e")
                        {
                            DocumentosFiscais_cmdDownloadCCePDF.Visible = true;
                            DocumentosFiscais_cmdDownloadCCeXML.Visible = true;
                        }
                        else if (e.Row.Cells[nColuna_sTipo].Text == "NFD-e")
                        {
                            DocumentosFiscais_cmdDownloadXML.Visible = true;
                            DocumentosFiscais_cmdDownloadDANFE.Visible = true;
                            if (e.Row.Cells[nColuna_Cancelamento].Text == "N")
                            {
                                DocumentosFiscais_cmdCancelarNFE.Visible = true;
                                DocumentosFiscais_cmdCartaCorrecao.Visible = true;

                            }

                        }
                        break;

                    case "Em processamento":
                        if (e.Row.Cells[nColuna_sTipo].Text == "NF-e")
                        {
                            DocumentosFiscais_cmdDownloadXML.Visible = true;
                        }
                        break;

                    case "Rejeitada":
                        DocumentosFiscais_cmdDownloadXML.Visible = true;
                        DocumentosFiscais_cmdCancelarNFE.Text = "Excluir";
                        DocumentosFiscais_cmdCancelarNFE.Visible = true;
                        break;

                    case "Aguardando DANFE":
                        DocumentosFiscais_cmdDownloadXML.Visible = true;
                        DocumentosFiscais_cmdCancelarNFE.Visible = true;
                        break;

                    case "Cancelada":
                        DocumentosFiscais_cmdDownloadXML.Visible = true;
                        DocumentosFiscais_cmdDownloadDANFE.Visible = true;
                        break;
                }

                //Fazer Validação de DATA para Cancelamento, carta de correção e Devolução
            }

            GRID.EsconderColunas(e, nColuna_idObjeto, nColuna_nNumeroCartaCorrecao, nColuna_Cancelamento, nColuna_sChaveNFe);


        }
        protected void DocumentosFiscais_cmdGerarNFe_Click(object sender, EventArgs e)
        {
            var row = (sender as LinkButton).NamingContainer as GridViewRow;
            hddidXML.Value = row.Cells[nColuna_idXML].Text;
            div_Modal.Style.Add("width", "35%");
            hddsAcaoNFe.Value = "ENVIAR_SEFAZ";
            txtsJustificativa_AcoesNFe.Visible = false;
            lblTitulos_AcoesNFe.Text = "Envio para SEFAZ";
            lblTituloJustificativa_AcoesNFe.Text = "Confirma o envio da NF-e para o SEFAZ?";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Confirma_SEFAZ", "$('#Modal_AcoesNFe').modal('show');", true);
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");
        }
        protected void DocumentosFiscais_cmdCancelarNFE_Click(object sender, EventArgs e)
        {
            var row = (sender as LinkButton).NamingContainer as GridViewRow;
            hddidXML.Value = row.Cells[nColuna_idXML].Text;
            txtsJustificativa_AcoesNFe.Visible = true;

            if (row.Cells[nColuna_Status].Text == "Espelho" || row.Cells[nColuna_Status].Text == "Rejeitada")
            {
                lblTitulos_AcoesNFe.Text = "Excluir NF Espelho";
                txtsJustificativa_AcoesNFe.Visible = false;
                div_Modal.Style.Add("width", "35%");
                lblTituloJustificativa_AcoesNFe.Text = "Confirma a exclusão do Espelho?";
                hddsAcaoNFe.Value = "EXCLUIR_ESPELHO";
            }
            else if (row.Cells[nColuna_Status].Text == "Autorizada")
            {
                div_Modal.Style.Add("width", "60%");
                lblTitulos_AcoesNFe.Text = "Cancelar NF-e";
                lblTituloJustificativa_AcoesNFe.Text = "Informe a justificativa para cancelamento";
                hddsAcaoNFe.Value = "CANCELAR_NFe";
                txtsJustificativa_AcoesNFe.Focus();
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModalAcoes", "$('#Modal_AcoesNFe').modal('show');", true);
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");

        }
        protected void DocumentosFiscais_cmdCartaCorrecao_Click(object sender, EventArgs e)
        {
            var row = (sender as LinkButton).NamingContainer as GridViewRow;
            hddidXML.Value = row.Cells[nColuna_idXML].Text;

            txtsJustificativa_AcoesNFe.Visible = true;
            div_Modal.Style.Add("width", "60%");
            lblTitulos_AcoesNFe.Text = "Carta de Correção NFe";
            lblTituloJustificativa_AcoesNFe.Text = "Informe o texto de correção";
            hddsAcaoNFe.Value = "CCe";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenCCe", "$('#Modal_AcoesNFe').modal('show');", true);
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");
            txtsJustificativa_AcoesNFe.Focus();
        }

        protected void cmdConfirmar_AcoesNFe_Click(object sender, EventArgs e)
        {
            string sRetorno = "";

            if (txtsJustificativa_AcoesNFe.Visible)
            {
                if (txtsJustificativa_AcoesNFe.Text == "" && txtsJustificativa_AcoesNFe.Text.Length < 15)
                    sRetorno = "Informe uma justificativa válida!</br> Texto maior que 15 caracteres.";
            }

            if (sRetorno == "")
            {
                switch (hddsAcaoNFe.Value)
                {
                    case "SALVAR":
                        Salvar_Detalhe();
                        break;

                    case "EXCLUIR_ESPELHO":
                        sRetorno = Funcoes_NFe.XML.Excluir(idOrigem_Emissor.ToString(), hddidXML.Value);
                        if (sRetorno == "")
                        {
                            Pesquisar(hddidPedido.Value, "CONSULTA");
                            MensagemPagina.MostraMensagem_Sucesso("Espelho excluido com sucesso!");
                        }
                        break;

                    case "ENVIAR_SEFAZ":
                        string nNF = "";
                        sRetorno = Funcoes_NFe.XML.EnviarXML_SEFAZ(hddidXML.Value, out nNF);
                        Pesquisar(hddidPedido.Value, "CONSULTA");
                        MensagemPagina.MostraMensagem_Sucesso(string.Format("Nota Fiscal n.º {0}, Gerada com sucesso!", nNF.PadLeft(6, '0')));
                        FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");
                        break;

                    case "CCe":
                        sRetorno = Funcoes_CCe.EnviarXML_SEFAZ(hddidXML.Value, txtsJustificativa_AcoesNFe.Text);
                        Pesquisar(hddidPedido.Value, "CONSULTA");
                        MensagemPagina.MostraMensagem_Sucesso("Nota Fiscal Gerada com sucesso!");
                        FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");
                        break;

                    case "CANCELAR_NFe":
                        sRetorno = Funcoes_NFe.XML.SEFAZ_Cancelar(hddidXML.Value, txtsJustificativa_AcoesNFe.Text);
                        if (sRetorno == "")
                        {
                            Pesquisar(hddidPedido.Value, "CONSULTA");
                            MensagemPagina.MostraMensagem_Sucesso("Cancelamento efetuado!");
                            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");
                        }
                        break;
                }

            }


            if (sRetorno != "")
            {
                MensagemPaginaAcoesNFe.MostraMensagem_Erro(sRetorno);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenCCe", "$('#Modal_AcoesNFe').modal('show');", true);
                FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");
            }


        }


        protected void DocumentosFiscais_cmdDownloadXML_Click(object sender, EventArgs e)
        {
            var row = (sender as LinkButton).NamingContainer as GridViewRow;
            Funcoes_NFe.Download.XML(Page, row.Cells[nColuna_idXML].Text);
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");
        }

        protected void DocumentosFiscais_cmdDownloadDANFE_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (sender as LinkButton).NamingContainer as GridViewRow;
                Funcoes_NFe.Download.PDF(Page, row.Cells[nColuna_idXML].Text);
                FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");
            }
            catch (Exception ex)
            {

                MensagemPagina.MostraMensagem_Erro("Erro ao Gerar PDF: " + ex.Message);
            }

        }

        protected void cmdEditar_Click(object sender, EventArgs e)
        {
            string idEmpresa = ddlidEmpresa.SelectedValue;
            Pesquisar(hddidPedido.Value, "EDITAR");
            cmdGerarPiloto.Visible = false;
            cmdEditar.Visible = false;
            cmdSalvar.Visible = true;
            aba_NFe.Visible = false;
        }

        protected void cmdGerarPiloto_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime dtEntrada_Saida = DateTime.TryParse(txtdtEntrada_Saida.Text, out DateTime dt) ? dt : default;


                string filePath = Funcoes_NFe.XML_GerarNFE.GerarXML(hddidPedido.Value, hddnNumeroPedido.Value, Page, hddidEmpresa.Value, false, "0", "", "N", false, "", "", "EMISSAO", true, dtEntrada_Saida);

                string Arquivo = Server.MapPath("~/Download/" + filePath);

                XDocument xdoc = XDocument.Load(Arquivo);
                string chave = "",
                        CNPJ = "",
                        CNPJdest = "",
                        xNome = "",
                        xNomedest = "",
                        xFant = "",
                        xLgr = "",
                        nro = "",
                        UF = "",
                        CEP = "",
                        xPais = "",
                        fone = "",
                        nNF = "",
                        sSerieNF = "",
                        Corpo = "";
                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}nfeProc").FirstOrDefault() != null)
                    Corpo = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}nfeProc").FirstOrDefault().ToString();
                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}NFe").FirstOrDefault() != null)
                    Corpo = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}NFe").FirstOrDefault().ToString();

                var infNFeElement = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}infNFe").FirstOrDefault();
                if (infNFeElement != null)

                    chave = infNFeElement.Attribute("Id")?.Value?.Substring(3);


                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}nNF").FirstOrDefault() != null)
                    nNF = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}nNF").FirstOrDefault()?.Value;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}serie").FirstOrDefault() != null)
                    sSerieNF = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}ide").Descendants("{http://www.portalfiscal.inf.br/nfe}serie").FirstOrDefault()?.Value;


                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                    CNPJ = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault() != null)
                    CNPJdest = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}CNPJ").FirstOrDefault()?.Value;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault() != null)
                    xNomedest = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}dest").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault()?.Value;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault() != null)
                    xNome = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xNome").FirstOrDefault()?.Value;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xFant").FirstOrDefault() != null)
                    xFant = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xFant").FirstOrDefault()?.Value;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault() != null)
                    xLgr = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xLgr").FirstOrDefault()?.Value;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault() != null)
                    nro = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}nro").FirstOrDefault()?.Value;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault() != null)
                    UF = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}UF").FirstOrDefault()?.Value;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault() != null)
                    CEP = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}CEP").FirstOrDefault()?.Value;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xPais").FirstOrDefault() != null)
                    xPais = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}xPais").FirstOrDefault()?.Value;

                if (xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}fone").FirstOrDefault() != null)
                    fone = xdoc.Descendants("{http://www.portalfiscal.inf.br/nfe}emit").Descendants("{http://www.portalfiscal.inf.br/nfe}fone").FirstOrDefault()?.Value;


                Dictionary<string, string> vParametrosGerenciador = new Dictionary<string, string>
                {
                    { "@sfuncao", "INCLUIR_XML" },
                    { "@idTipoObjeto", "12" },
                    { "@idObjeto", hddidPedido.Value },
                    { "@sChaveNFe", chave },
                    { "@sCNPJ_Emitente", CNPJ },
                    { "@nNumeroNF", nNF },
                    { "@nSerie", sSerieNF },
                    { "@sXML", Corpo.ToString() },
                    { "@sNome_Emitente", xNome },
                    { "@sNomeFantasia_Emitente", xFant },
                    { "@sLogradouro_Emitente", xLgr },
                    { "@sNumero_Emitente", nro },
                    { "@sUF_Emitente", UF },
                    { "@sCEP_Emitente", CEP },
                    { "@sPais_Emitente", xPais },
                    { "@sTelefone_Emitente", fone },
                    { "@sNome_Destinatario", xNomedest },
                    { "@idOrigem", "5" },
                    { "@sEspelho", "S" },
                    { "@sTipoXML", "3" }
                };
                DataSet dsPesquisaGerenciador = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_XML_NFe", vParametrosGerenciador);


                Pesquisar(hddidPedido.Value, "CONSULTA");
                MensagemPagina.MostraMensagem_Sucesso("Espelho emitido com sucesso!");
                FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");

            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }



        }



        protected void DocumentosFiscais_cmdDownloadCCeXML_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (sender as LinkButton).NamingContainer as GridViewRow;
                Funcoes_CCe.Download_XML(Page, row.Cells[nColuna_sChaveNFe].Text, row.Cells[nColuna_nNumeroCartaCorrecao].Text);
                FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }

        protected void DocumentosFiscais_cmdDownloadCCePDF_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (sender as LinkButton).NamingContainer as GridViewRow;
                Funcoes_CCe.Download_PDF(Page, row.Cells[nColuna_sChaveNFe].Text, row.Cells[nColuna_nNumeroCartaCorrecao].Text);
                FUNCOES.Scripts.Mantem_AbaAtiva(Page, "NFe-tab");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }

        protected void cmdImportarItens_Click(object sender, EventArgs e)
        {
            pnResultado.Visible = false;
            rb_Resposta.SelectedValue = null;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenImportacao", "$('#Modal_ImportarItens').modal('show');", true);
        }

        protected void cbTipoImportacao_CheckedChanged(object sender, EventArgs e)
        {

            pnResultado.Visible = false;
            string sPesquisa = rb_Resposta.SelectedValue;
            string sErro = "";
            DataSet dsPesquisa;
            Dictionary<String, String> vParametros = new Dictionary<string, string>
            {
                { "@sfuncao", "PESQUISA_ITENS" },
                { "@sPesquisa", sPesquisa }
            };
            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript_Geral(gvImportacaoItens, dsPesquisa.Tables[0], true, true, true, "false", "''", "10", true, true, true, 3, "desc", null), true);
                pnResultado.Visible = true;

            }
            else
                MensagemPagina_ImportacaoItens.MostraMensagem_Erro(sErro);
        }


        protected void gvImportacaoItens_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            string id = gvImportacaoItens.DataKeys[Convert.ToInt32(e.CommandArgument.ToString())].Values[0].ToString();
            string sTipo = gvImportacaoItens.DataKeys[Convert.ToInt32(e.CommandArgument.ToString())].Values[1].ToString();

            hddidImportarItem.Value = id;
            hddImportarTipo.Value = sTipo;

            string sErro = "";
            DataSet dsPesquisa;
            Dictionary<String, String> vParametros = new Dictionary<string, string>
            {
                { "@sfuncao", "CONSULTAR_ITENS" },
                { "@sPesquisa", sTipo },
                { "@idPedido", id }
            };
            dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out sErro))
            {
                foreach (DataRow linha in dsPesquisa.Tables[0].Rows)
                {
                    FrameWork.cls_Pedidos_Itens objEnvios = new FrameWork.cls_Pedidos_Itens();
                    objEnvios.nOrdem = Base_Itens.Count + 1;
                    objEnvios.idItem = 0;
                    objEnvios.idProduto = int.Parse(linha["idProduto"].ToString());
                    objEnvios.sFuncao = "SALVAR_ITENS";
                    objEnvios.sCodigoProduto = linha["sCodigo"].ToString();
                    objEnvios.sDscProduto = linha["sDscProduto"].ToString();
                    objEnvios.sUnidade = linha["sUnidade"].ToString();
                    objEnvios.nQuantidade = Math.Round(Convert.ToDouble(linha["nQuantidade"].ToString()), 4);
                    double quantidade = Math.Round(Convert.ToDouble(linha["nQuantidade"].ToString()), 4);
                    double unitario = Convert.ToDouble(linha["nValorUnitario"].ToString());
                    objEnvios.nValorUnitario = Convert.ToDouble(linha["nValorUnitario"].ToString());
                    objEnvios.nValorTotal = Math.Round(unitario * quantidade, 2);
                    Base_Itens.Add(objEnvios);
                    DataBind_gvItens();
                }
            }


            ScriptManager.RegisterStartupScript(this, this.GetType(), "FecharModalItens", "$('#Modal_ImportarItens').modal('hide');", true);

        }

        protected void gvImportacaoItens_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Grid.EsconderColunas(e, 0);
        }

        protected void ddlsChaveNFe_Referencia_SelectedIndexChanged(object sender, EventArgs e)
        {
            Div_sChaveNFe_Referencia.Visible = false;
            if (ddlsChaveNFe_Referencia.SelectedValue == "S")
                Div_sChaveNFe_Referencia.Visible = true;
        }
        protected void gvItens_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            int IdItem = Convert.ToInt32(e.Keys[1].ToString());
            Base_Itens[Base_Itens.FindIndex(x => x.nOrdem.Equals(IdItem))].sFuncao = "EXCLUIR ITEM";
            DataBind_gvItens();
        }

        protected void gvItens_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlsUnidade = (e.Row.FindControl("ddlsUnidade") as DropDownList);
                //FUNCOES.Popula_Combo(ddlsUnidade, "sp_Select 'Flow_Produtos_Unidade', @sPesquisa = 'N'", "sUnidade", "sUnidade", false, "Selecione ", "");
                ddlsUnidade.Items.AddRange(BaseddlsUnidade.Items.Cast<ListItem>().Select(i => new ListItem(i.Text, i.Value)).ToArray());
                ddlsUnidade.SelectedValue = Base_Itens[e.Row.RowIndex].sUnidade.ToString();

                if (hddsFuncaoPagina.Value == "CONSULTA")
                {
                    LinkButton lnkDelete = e.Row.FindControl("lnkDelete") as LinkButton;
                    lnkDelete.Visible = false;

                    LinkButton lnkReducaoBC = e.Row.FindControl("lnkReducaoBC") as LinkButton;
                    if (Base_Itens[e.Row.RowIndex].idCST_ICMS == 0)
                    {
                        lnkReducaoBC.Visible = false;
                    }


                }
            }
            GRID.EsconderColunas(e, 1, 2);
        }

        protected void gvItens_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            if (e.CommandName == "RedBC")
            {
                int nOrdem = int.Parse(e.CommandArgument.ToString());
                var baseItem = Base_Itens[Base_Itens.FindIndex(x => x.nOrdem.Equals(nOrdem))];
                if (Base_CST.Count == 0)
                    Base_CST = cls_CST.Popular();

                ddlReducaoBC_idCST_ICMS.DataSource = Base_CST.Where(c => c.sICMS.ToString().Equals("S")).Select(x => new { x.idCST, x.sDescricao }).ToList();
                ddlReducaoBC_idCST_ICMS.DataValueField = "idCST";
                ddlReducaoBC_idCST_ICMS.DataTextField = "sDescricao";
                ddlReducaoBC_idCST_ICMS.DataBind();

                ddlReducaoBC_idCST_cBenef.DataSource = Base_CST.Where(c => c.sBENEF.ToString().Equals("S")).Select(x => new { x.idCST, x.sDescricao }).ToList();
                ddlReducaoBC_idCST_cBenef.DataValueField = "idCST";
                ddlReducaoBC_idCST_cBenef.DataTextField = "sDescricao";
                ddlReducaoBC_idCST_cBenef.DataBind();

                lblReducaoBC_Titulo.Text = "Redução Base de Calculo - Item: " + baseItem.sDscProduto.ToString();
                txtReducaoBC_sCodigoProduto.Text = baseItem.sCodigoProduto.ToString();
                txtReducaoBC_sDscProduto.Text = baseItem.sDscProduto.ToString();
                hddReducaoBC_nOrdem.Value = nOrdem.ToString();

                ddlReducaoBC_idCST_ICMS.SelectedValue = baseItem.idCST_ICMS.ToString();
                Verifica_CodigoBeneficio();

                ddlReducaoBC_idCST_cBenef.SelectedValue = baseItem.idCST_cBenef.ToString();
                txtReducaoBC_nPercReducaoBC.Text = string.IsNullOrEmpty(baseItem.nPercReducaoBC.ToString()) ? "" : baseItem.nPercReducaoBC.ToString("N2");
                txtReducaoBC_sPedidoCliente.Text = string.IsNullOrEmpty(baseItem.sPedidoCliente) ? "" : baseItem.sPedidoCliente.ToString();

                if (hddsFuncaoPagina.Value == "CONSULTA")
                {
                    txtReducaoBC_nPercReducaoBC.ReadOnly = true;
                    txtReducaoBC_sPedidoCliente.ReadOnly = true;
                    cmdReducaoBC_Confirmar.Visible = false;
                    ddlReducaoBC_idCST_ICMS.SetAttribute("disabled", "disabled");
                    ddlReducaoBC_idCST_cBenef.SetAttribute("disabled", "disabled");

                }
                else
                {
                    txtReducaoBC_nPercReducaoBC.ReadOnly = false;
                    txtReducaoBC_sPedidoCliente.ReadOnly = false;
                    cmdReducaoBC_Confirmar.Visible = true;
                    ddlReducaoBC_idCST_ICMS.RemoveAttribute("disabled");
                    ddlReducaoBC_idCST_cBenef.RemoveAttribute("disabled");
                }

                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModalReducaoBC_", "$('#Modal_ReducaoBC').modal('show');", true);
            }
        }

        protected void ddlReducaoBC_idCSTICMS_SelectedIndexChanged(object sender, EventArgs e)
        {
            Verifica_CodigoBeneficio();
        }

        protected void cmdReducaoBC_Confirmar_Click(object sender, EventArgs e)
        {
            int nOrdem = Convert.ToInt32(hddReducaoBC_nOrdem.Value);
            Base_Itens[Base_Itens.FindIndex(x => x.nOrdem.Equals(nOrdem))].sFuncao = "SALVAR_ITENS";
            Base_Itens[Base_Itens.FindIndex(x => x.nOrdem.Equals(nOrdem))].idCST_ICMS = Convert.ToInt32(ddlReducaoBC_idCST_ICMS.SelectedValue);
            int idCST_cBenef = 0;
            if (div_ReducaoBC_idCST_cBenef.Visible)
                idCST_cBenef = Convert.ToInt32(ddlReducaoBC_idCST_cBenef.SelectedValue);
            Base_Itens[Base_Itens.FindIndex(x => x.nOrdem.Equals(nOrdem))].idCST_cBenef = idCST_cBenef;
            Base_Itens[Base_Itens.FindIndex(x => x.nOrdem.Equals(nOrdem))].nPercReducaoBC = BD.Conversoes.Numerico_Decimal(txtReducaoBC_nPercReducaoBC.Text);
            Base_Itens[Base_Itens.FindIndex(x => x.nOrdem.Equals(nOrdem))].sPedidoCliente = txtReducaoBC_sPedidoCliente.Text;
            DataBind_gvItens();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "FecharModalItens", "$('#Modal_ReducaoBC').modal('hide');", true);

        }
        void Verifica_CodigoBeneficio()
        {
            div_ReducaoBC_idCST_cBenef.Visible = false;
            try
            {
                var Localiza = Base_CST.Where(c => c.idCST.ToString().Equals(ddlReducaoBC_idCST_ICMS.SelectedValue)).Select(x => new { x.sExige_cBenef }).ToList();
                if (Localiza[0].sExige_cBenef.ToString() == "S")
                {
                    div_ReducaoBC_idCST_cBenef.Visible = true;
                }
            }
            catch (Exception)
            {
            }
        }


    }
}

