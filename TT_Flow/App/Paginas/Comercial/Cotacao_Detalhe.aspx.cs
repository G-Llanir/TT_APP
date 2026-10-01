using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT_Flow.App.Controles;
using TT_Flow.FrameWork;
using BD = TT.FrameWork.BD;
using RETORNO = TT.FrameWork.BD.Retorno;
using static TT.FrameWork.Funcoes;
using static TT.FrameWork.Identity;
using Microsoft.Reporting.WebForms;

namespace TT_Flow.App.Paginas.Comercial
{
    public partial class Cotacao_Detalhe : Page
    {
        #region | Contrutores

        protected static string sTituloPagina = "Cotação";
        protected static string sProcedure = "sp_Manipula_tbl_Flow_Pedidos";

        protected bool bNovo { get { return hddidCotacao.Value == "0"; } }
        protected bool bEditar { get { return Convert.ToBoolean(hddEditar.Value); } set { hddEditar.Value = value.ToString().ToLower(); } }
        protected bool bCliente { get { return Convert.ToBoolean(hddsCliente.Value); } set { hddsCliente.Value = value.ToString().ToLower(); } }

        protected List<cls_Pedidos_Itens> Lista_Itens
        {
            get
            {
                if (ViewState["Lista_Itens"] == null) ViewState["Lista_Itens"] = new List<cls_Pedidos_Itens>();
                return (List<cls_Pedidos_Itens>)ViewState["Lista_Itens"];
            }
            set { ViewState["Lista_Itens"] = value; }
        }

        #endregion

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "Manual-Cotacoes.pdf";

            if (!IsPostBack)
            {
                bCliente = Variaveis.sTipo().Equals("P");

                if (!string.IsNullOrEmpty(Request["id"])) Pesquisar(Request["id"].ToString());
                else DirecionaPagina("App/Paginas/Comercial/Cotacao_Detalhe.aspx?id=0");

                if (Request["msg"] == "1") MensagemPagina.MostraMensagem_Sucesso("Informações registradas com sucesso!");
                if (Request["msg"] == "2") MensagemPagina.MostraMensagem_Sucesso($"Nova Cotação registrada com sucesso!<br /><a target='_blank' href='/App/Paginas/Comercial/Cotacao_Detalhe.aspx?id=0'>Criar Nova Cotação...</a>");
            }
            else
            {
                var requestTarget = Request["__EVENTTARGET"];
                if (requestTarget == "funcao_SALVAR") SalvarDados();
                else if (requestTarget == "funcao_EDITAR") { bEditar = true; Pesquisar(Request["id"]); }
            }

            ExcelImportar.ID_FileUpload = ImportarItens_Excel.ClientID;

            // --------------------------------------------------------------------------------
            // Mensagens Fixas

            Mensagem_Modal_ImportarItens_Excel.MostraMensagem_Aviso(@"O arquivo Excel será lido, considerando que as informações sigam o seguinte esquema:<br /><br />
                                                                    <table style=""width: 100%;"">
                                                                        <thead>
                                                                            <tr>
                                                                                <th style=""padding: 5px;text-align: center;border: 1px solid black;"">Código do Item</th>
                                                                                <th style=""padding: 5px;text-align: center;border: 1px solid black;"">Quantidade</th>
                                                                            </tr>
                                                                        </thead>
                                                                        <tbody>
                                                                            <tr>
                                                                                <td style=""padding: 5px;text-align: center;border: 1px solid black;"">Código_do_Item</td>
                                                                                <td style=""padding: 5px;text-align: center;border: 1px solid black;"">2,00</td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>", false);

            // --------------------------------------------------------------------------------

            RegistraScript();
        }

        protected void Pesquisar(string idCotacao)
        {
            aba_Historico.Visible = false;

            div_ID.Visible = false;
            div_cmdSelecionarParceiro.Visible = false;
            div_Referencia.Visible = false;
            div_Endereco.Visible = false;
            div_Obs.Visible = false;
            div_Total.Visible = false;
            div_Itens.Visible = false;

            PainelAtualizacao.Visible = false;

            cmdSalvar.Visible = false;
            cmdEditar.Visible = false;
            cmdGerarPDF.Visible = false;
            cmdVincular.Visible = false;
            cmdOrcamento.Visible = false;

            cmdVoltar.Text = "Cancelar";

            try
            {
                LimpaCampos();

                DataSet ds = BD.ExecutarDataSet(sProcedure, new Dictionary<string, string> { { "@sFuncao", "CONSULTA_DETALHE_COTACAO" }, { "@idPedido", idCotacao } });

                hddnNumeroPedido.Value = RETORNO.DATASET(ds, 5, 0, "nNumeroCotacao");
                txtnCotacao.Text = hddnNumeroPedido.Value;

                if (idCotacao != "0")
                {
                    ValidaPermissao(Permissao.Comercial.Cotacoes.Consultar, true);

                    cmdSalvar.Text = "Salvar";
                    cmdEditar.Visible = ValidaPermissao(Permissao.Comercial.Cotacoes.Alterar);

                    if (BD.ValidarDataSet(ds, out string sErro))
                    {
                        div_ID.Visible = true;
                        div_Referencia.Visible = true;
                        div_Endereco.Visible = true;
                        div_Obs.Visible = true;
                        div_Total.Visible = true;
                        div_Itens.Visible = true;

                        PainelAtualizacao.Visible = true;

                        txtCPF_Parceiro.ReadOnly = true;
                        txtNome_Parceiro.ReadOnly = true;

                        hdddtPedido.Value = DateTime.Parse(RETORNO.DATASET(ds, "dtPedido")).ToString("yyyy-MM-ddTHH:mm:ss");

                        hddidCotacao.Value = RETORNO.DATASET(ds, "idCotacao");
                        txtidCotacao.Text = hddidCotacao.Value;
                        hddnNumeroPedido.Value = RETORNO.DATASET(ds, "nNumeroCotacao");
                        txtnCotacao.Text = hddnNumeroPedido.Value;
                        hddidParceiro.Value = RETORNO.DATASET(ds, "idParceiro");
                        txtCPF_Parceiro.Text = Formatar_CNPJ_CPF(RETORNO.DATASET(ds, "sCPF_Parceiro"));
                        txtNome_Parceiro.Text = RETORNO.DATASET(ds, "sDscParceiro");
                        txtIE.Text = RETORNO.DATASET(ds, "sDscIE");

                        // Popula Combos - Tabela de Preços, Condição de Pagamento, Endereço de Entrega
                        {
                            Dictionary<string, string> vParam = new Dictionary<string, string>
                            {
                                { "@sFuncao", "SelecionaParceiro__Cotacoes" },
                                { "@idPesquisa", hddidParceiro.Value },
                                { "@idFiltro", Variaveis.idEmpresa() == "Brasil" ? "2" : Variaveis.idEmpresa() == "0" ? "2" : "0" },
                                { "@idFiltro_1", idCotacao }
                            };
                            List<cls_Multiplos_Combos> ddls = new List<cls_Multiplos_Combos>
                            {
                                new cls_Multiplos_Combos { ddl = ddlTabela, sCampoCodigo = "idTabela", sCampoDescricao = "sDscTabela", bConcatenarCodigo_Descricao = false, sMensagemPrimeiraLinha = "Selecione a Tabela de Preço", sValorPrimeiraLinha = "0" },
                                new cls_Multiplos_Combos { ddl = ddlCondicaoPagamento, sCampoCodigo = "idCondicaoPagamento", sCampoDescricao = "sDscCondicaoPagamento", bConcatenarCodigo_Descricao = false, sMensagemPrimeiraLinha = "Selecione uma Cond. de Pagamento", sValorPrimeiraLinha = "-1" },
                                new cls_Multiplos_Combos { ddl = ddlEnderecoEntrega, sCampoCodigo = "idEndereco", sCampoDescricao = "sEndereco", bConcatenarCodigo_Descricao = false, sMensagemPrimeiraLinha = "Selecione o Endereço de Entrega", sValorPrimeiraLinha = "0" },
                            };
                            Popula_Multiplos_Combos(ddls, vParam);
                        }

                        ddlTabela.SelectedValue = RETORNO.DATASET(ds, "idTabelaPreco");
                        txtReferencia.Text = RETORNO.DATASET(ds, "sReferencia");
                        ddlEnderecoEntrega.SelectedValue = RETORNO.DATASET(ds, "idEnderecoEntrega");
                        ddlCondicaoPagamento.SelectedValue = RETORNO.DATASET(ds, "idCondicaoPagamento");
                        txtObs.Text = RETORNO.DATASET(ds, "sObservacao");

                        hddidOrcamento_Vinculado.Value = RETORNO.DATASET(ds, "idOrcamento");

                        lblTituloPagina.Text = $"{sTituloPagina} N°{txtnCotacao.Text} - {txtReferencia.Text}";
                        BreadCrumb.TitulodaPagina = "Detalhe";

                        PainelAtualizacao.Atualizar(RETORNO.DATASET(ds, "dtUltimaAtualizacao"), RETORNO.DATASET(ds, "sDscUsuarioAtualizacao"));

                        Popula_Itens(ds.Tables[1]);
                        Popula_Aba_Historico(ds.Tables[2]);

                        if (!bEditar)
                        {
                            cmdVoltar.Text = "Voltar";

                            cmdAtualizar.Visible = false;
                            cmdSalvar.Visible = false;
                            cmdGerarPDF.Visible = true;

                            BloquearCampos(true);

                            int.TryParse(hddidOrcamento_Vinculado.Value, out int idOrcamento);

                            if (idOrcamento > 0)
                            {
                                cmdEditar.Visible = false;

                                if (!bCliente) { cmdOrcamento.Visible = true; cmdOrcamento.NavigateUrl = $"/App/Paginas/Comercial/Orcamento_Detalhe.aspx?id={hddidOrcamento_Vinculado.Value}"; }
                                else MensagemPagina.MostraMensagem($"Esta Cotação já foi convertida em um Orçamento!<br />Para mais informações, consulte seu vendedor na Tec and Tec.", "INFO", true);
                            }
                            else { cmdVincular.Visible = !bCliente; cmdVincular.NavigateUrl = $"/App/Paginas/Comercial/Orcamento_Detalhe.aspx?id=0&idCotacao={hddidCotacao.Value}"; }
                        }
                        else
                        {
                            cmdAtualizar.Visible = true;
                            cmdSalvar.Visible = true;
                            cmdEditar.Visible = false;

                            BloquearCampos(false);

                            if (bCliente) ddlTabela.Attributes.Add("disabled", "disabled");
                        }
                    }
                    else throw new Exception(sErro);
                }
                else
                {
                    ValidaPermissao(Permissao.Comercial.Cotacoes.Incluir, true);

                    bEditar = true;

                    div_cmdSelecionarParceiro.Visible = true;
                    cmdSalvar.Visible = true;

                    lblTituloPagina.Text = $"Nova {sTituloPagina}";
                    BreadCrumb.TitulodaPagina = "Incluir";

                    txtidCotacao.Text = "Novo";
                    cmdSalvar.Text = "Incluir";

                    hdddtPedido.Value = DateTime.Today.ToString("yyyy-MM-ddTHH:mm:ss");

                    if (bCliente)
                    {
                        BloquearCampos(true, true);

                        string idParceiro = Variaveis.idParceiro();

                        Dictionary<string, string> vParamestrosParceiros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "CONSULTA_PARCEIRO" },
                            { "@idParceiro", idParceiro }
                        };
                        DataSet dsParceiro = BD.ExecutarDataSet(sProcedure, vParamestrosParceiros);

                        txtCPF_Parceiro.Text = Formatar_CNPJ_CPF(RETORNO.DATASET(dsParceiro, "sCPF_CNPJ"));
                        txtNome_Parceiro.Text = RETORNO.DATASET(dsParceiro, "sDscParceiro");

                        txtCPF_Parceiro.ReadOnly = true;
                        txtNome_Parceiro.ReadOnly = true;
                        cmdSelecionarParceiro.Visible = false;

                        hddidParceiro.Value = idParceiro;
                        cmdSelecionarParceiro_Click(null, null);
                    }
                }

                // Incluir Itens / Selecionar Parceiros
                {
                    hddItens.Value = "[ ";
                    hddParceiros.Value = "[ ";

                    foreach (DataRow row in ds.Tables[3].Rows) hddItens.Value += $"{{ id: {row["idProduto"]}, sCodigo: `{row["sCodigo"]}`, sDsc: `{row["sDscProduto"]}` }}, ";
                    foreach (DataRow row in ds.Tables[4].Rows) hddParceiros.Value += $"{{ id: {row["idParceiro"]}, sCodigo: `{Formatar_CNPJ_CPF(row["sCPF_CNPJ"].ToString())}`, sDsc: `{row["sDscParceiro"]}` }}, ";

                    hddItens.Value = hddItens.Value.Trim().TrimEnd(',');
                    hddParceiros.Value = hddParceiros.Value.Trim().TrimEnd(',');
                    hddItens.Value += " ]";
                    hddParceiros.Value += " ]";
                }

                Scripts.FocusScript(Page, txtNome_Parceiro.ClientID);
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Houve um erro ao carregar as informações da Cotação!<br />Erro ao pesquisar: " + ex.Message);
            }
        }

        #endregion

        #region | dtgItens

        protected void dtgItens_DataBind()
        {
            try
            {
                dtgItens.Columns[dtgItens.Columns.Count - 1].Visible = bEditar;

                dtgItens.DataSource = Lista_Itens.Where(x => x.sFuncao.ToString() != "EXCLUIR ITEM").OrderBy(x => x.idProduto);
                dtgItens.DataBind();

                bool bItens = dtgItens.Rows.Count > 0;
                div_dtgItens.Visible = bItens;
                div_Excluir.Visible = bNovo || (bEditar && bItens);
                divIncluir_Itens.Visible = bNovo || bEditar;

                txtTotal.Text = Lista_Itens.Where(x => x.sFuncao.ToString() != "EXCLUIR ITEM").Sum(x => x.nValorTotal).ToString("N2");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao Carregar os Itens: " + ex.Message);
            }
        }

        protected void dtgItens_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    (e.Row.FindControl("txtQtd") as TextBox).ReadOnly = !bEditar;
                    (e.Row.FindControl("txtValor") as TextBox).ReadOnly = !bEditar || bCliente;
                }
            }
            catch { }
        }

        #endregion

        #region | Aba Histórico

        protected void Popula_Aba_Historico(DataTable dt)
        {
            gv_Historico.DataSource = dt;
            gv_Historico.DataBind();
            aba_Historico.Visible = gv_Historico.Rows.Count > 0;
        }

        protected void gv_Historico_RowDataBound(object sender, GridViewRowEventArgs e) => e.Row.Cells[2].Text = HttpUtility.HtmlDecode(e.Row.Cells[2].Text);

        #endregion

        #region | Salvar + Classe

        protected void SalvarDados()
        {
            AtualizaClasse_Itens();

            if (ValidarDados())
            {
                try
                {
                    string idCotacao = hddidCotacao.Value;
                    string sAlteracaoItens = Lista_Itens.Any(x => x.sFuncao == "INCLUIR ITEM") ? "S" : "N";

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", idCotacao.Trim() == "0" ? "INCLUIR PEDIDO" : "ALTERAR PEDIDO" },
                        { "@idPedido", idCotacao.ToString() },
                        { "@idTipo", "4" },
                        { "@idCliente", hddidParceiro.Value },
                        { "@idCondicaoDePagamento", ddlCondicaoPagamento.SelectedValue },
                        { "@idEnderecoEntrega", ddlEnderecoEntrega.SelectedValue },
                        { "@sReferencia", txtReferencia.Text },
                        { "@dtPedido", hdddtPedido.Value },
                        { "@sObservacao", txtObs.Text },
                        { "@idUsuarioInclusao", Variaveis.idUsuario() },
                        { "@nVlrProdutos", txtTotal.Text.Replace(".", "").Replace(',', '.').Trim() },
                        { "@idTabelaPreco", ddlTabela.SelectedValue },
                        { "@sAlteracaoItens", sAlteracaoItens }
                    };
                    DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(ds, out string sErro))
                    {
                        idCotacao = RETORNO.DATASET(ds, "idPedido");

                        if (Salvar_Itens(idCotacao)) DirecionaPagina($"App/Paginas/Comercial/Cotacao_Detalhe.aspx?id={idCotacao}&msg={(Request["id"] == "0" ? "2" : "1")}");
                        else throw new Exception("Houve um erro ao Salvar os Itens da Cotação!");
                    }
                    else throw new Exception("Erro ao Salvar a Cotação: " + sErro.ToString());
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }
            }
        }

        protected bool Salvar_Itens(string idCotacao)
        {
            try
            {
                foreach (var item in Lista_Itens)
                {
                    if (item.sFuncao == "CONSULTA ITEM") continue;

                    Dictionary<string, string> vParam = new Dictionary<string, string>
                    {
                        { "@sFuncao", item.sFuncao },
                        { "@idPedido", idCotacao },
                        { "@idProduto", item.idProduto.ToString() },
                        { "@sCodigo", item.sCodigoProduto },
                        { "@sDscProduto", item.sDscProduto },
                        { "@sUnidade", item.sUnidade },
                        { "@nQuantidade", Math.Round(item.nQuantidade, 2).ToString().Replace(",", ".") },
                        { "@nValorUnitario", Math.Round(item.nValorUnitario, 2).ToString().Replace(",", ".") },
                        { "@nValorTotal", Math.Round(item.nValorTotal, 2).ToString().Replace(",", ".") },
                        { "@idUsuarioInclusao", Variaveis.idUsuario() }
                    };
                    BD.ExecutarDataSet(sProcedure, vParam);
                }

                return true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao Salvar os Itens: " + ex.Message);
                return false;
            }
        }

        protected void AtualizaClasse_Itens()
        {
            foreach (GridViewRow row in dtgItens.Rows)
            {
                try
                {
                    var item = Lista_Itens.Where(x => x.idContador.Equals(int.Parse(row.Cells[0].Text))).First();

                    double.TryParse((row.FindControl("txtQtd") as TextBox).Text, out double qtd);
                    double.TryParse((row.FindControl("txtValor") as TextBox).Text, out double valor);
                    double total = qtd * valor;

                    if (item.nQuantidade != qtd || item.nValorUnitario != valor || item.nValorTotal != total) item.sFuncao = "INCLUIR ITEM";

                    item.nQuantidade = qtd;
                    item.nValorUnitario = valor;
                    item.nValorTotal = total;
                }
                catch { }
            }
        }

        #endregion

        #region | Utils

        protected void Popula_Itens(DataTable dt)
        {
            Lista_Itens.Clear();

            foreach (DataRow row in dt.Rows)
            {
                cls_Pedidos_Itens item = new cls_Pedidos_Itens
                {
                    sFuncao = "CONSULTA ITEM",
                    idContador = Lista_Itens.Count + 1,
                    idPedido = Convert.ToInt32(row["idPedido"].ToString()),
                    idProduto = Convert.ToInt32(row["idProduto"].ToString()),
                    sCodigoProduto = row["sCodigo"].ToString(),
                    sDscProduto = row["sDscProduto"].ToString(),
                    sUnidade = row["sUnidade"].ToString(),
                    nQuantidade = Convert.ToDouble(row["nQuantidade"].ToString()),
                    nValorUnitario = Convert.ToDouble(row["nValorUnitario"].ToString()),
                    nValorTotal = Convert.ToDouble(row["nValorTotal"].ToString())
                };
                Lista_Itens.Add(item);
            }

            dtgItens_DataBind();
            LimpaCampos_Itens();
        }

        protected void BloquearCampos(bool bBloqueia, bool bNovo = false)
        {
            if (!bNovo)
            {
                txtReferencia.ReadOnly = bBloqueia;
                txtObs.ReadOnly = bBloqueia;
            }

            ddlTabela.Attributes.Remove("disabled");
            ddlEnderecoEntrega.Attributes.Remove("disabled");
            ddlCondicaoPagamento.Attributes.Remove("disabled");

            string sBloqueia = bBloqueia ? "disabled" : "enabled";

            ddlTabela.Attributes.Add(sBloqueia, sBloqueia);

            if (!bNovo)
            {
                ddlEnderecoEntrega.Attributes.Add(sBloqueia, sBloqueia);
                ddlCondicaoPagamento.Attributes.Add(sBloqueia, sBloqueia);
            }
        }

        protected void LimpaCampos()
        {
            hddidCotacao.Value = "0";
            txtCPF_Parceiro.Text = "";
            txtNome_Parceiro.Text = "";
            txtIE.Text = "";
            txtnCotacao.Text = "";
            txtReferencia.Text = "";
            ddlEnderecoEntrega.SelectedValue = "0";
            ddlCondicaoPagamento.SelectedValue = "-1";
            txtCodigo_Item.Text = "";
            txtDescricao_Item.Text = "";
            txtQuantidade_Item.Text = "";
        }

        protected bool ValidarDados()
        {
            if (hddidParceiro.Value == "0")
            {
                MensagemPagina.MostraMensagem_Erro("É necessário selecionar um Parceiro!", false);
                return false;
            }
            else if (ddlTabela.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("É necessário selecionar a Tabela de Preços!", false);
                return false;
            }
            else if (!Lista_Itens.Where(e => e.sFuncao != "EXCLUIR ITEM").Any())
            {
                MensagemPagina.MostraMensagem_Erro("É necessário incluir ao menos um Item!", false);
                return false;
            }
            else if (Lista_Itens.Where(e => e.nValorUnitario <= 0 || e.nValorTotal <= 0).Any())
            {
                MensagemPagina.MostraMensagem_Erro("É necessário que todos os Itens possuam Valores e Totais válidos!", false);
                return false;
            }

            return true;
        }

        protected void LimpaCampos_Itens()
        {
            hddIncluirItem.Value = "0";
            txtCodigo_Item.Text = string.Empty;
            txtDescricao_Item.Text = string.Empty;
            txtQuantidade_Item.Text = "1";
        }

        protected bool ValidarDados_Itens()
        {
            if (txtCodigo_Item.Text.Length <= 0)
            {
                MensagemPagina_Itens.MostraMensagem_Erro("O Código do Item é obrigatório!", false);
                return false;
            }
            if (txtDescricao_Item.Text.Length <= 0)
            {
                MensagemPagina_Itens.MostraMensagem_Erro("A Descrição do Item é obrigatória!", false);
                return false;
            }
            if (string.IsNullOrEmpty(txtQuantidade_Item.Text) || txtQuantidade_Item.Text == "0")
            {
                MensagemPagina_Itens.MostraMensagem_Erro("A Quantidade do Item é obrigatória e deve ser maior que Zero!", false);
                return false;
            }

            return true;
        }

        #endregion

        #region | Eventos

        protected void cmdSelecionarParceiro_Click(object sender, EventArgs e)
        {
            AtualizaClasse_Itens();

            if (hddidParceiro.Value != "0")
            {
                div_ID.Visible = true;
                div_Referencia.Visible = true;
                div_Endereco.Visible = true;
                div_Obs.Visible = true;
                div_Total.Visible = true;
                div_Itens.Visible = true;

                Dictionary<string, string> vParam = new Dictionary<string, string>
                {
                    { "@sFuncao", "SelecionaParceiro__Cotacoes" },
                    { "@idPesquisa", hddidParceiro.Value },
                    { "@idFiltro", Variaveis.idEmpresa() == "Brasil" ? "2" : Variaveis.idEmpresa() == "0" ? "2" : "0" },
                    { "@idFiltro_1", Request["id"] }
                };
                List<cls_Multiplos_Combos> ddls = new List<cls_Multiplos_Combos>
                {
                    new cls_Multiplos_Combos { ddl = ddlTabela, sCampoCodigo = "idTabela", sCampoDescricao = "sDscTabela", bConcatenarCodigo_Descricao = false, sMensagemPrimeiraLinha = "Selecione a Tabela de Preço", sValorPrimeiraLinha = "0" },
                    new cls_Multiplos_Combos { ddl = ddlCondicaoPagamento, sCampoCodigo = "idCondicaoPagamento", sCampoDescricao = "sDscCondicaoPagamento", bConcatenarCodigo_Descricao = false, sMensagemPrimeiraLinha = "Selecione uma Cond. de Pagamento", sValorPrimeiraLinha = "-1" },
                    new cls_Multiplos_Combos { ddl = ddlEnderecoEntrega, sCampoCodigo = "idEndereco", sCampoDescricao = "sEndereco", bConcatenarCodigo_Descricao = false, sMensagemPrimeiraLinha = "Selecione o Endereço de Entrega", sValorPrimeiraLinha = "0" },
                };
                DataSet ds = Popula_Multiplos_Combos(ddls, vParam);
                DataTable dt = ds.Tables[3];

                string sidTabela = dt.Rows[0]["sidTabela"].ToString();

                if (!string.IsNullOrEmpty(sidTabela) && !int.TryParse(sidTabela, out int idTabela) && idTabela.Equals(0))
                {
                    foreach (string sid in sidTabela.Split('|'))
                    {
                        if (int.TryParse(sid.Trim(), out int id))
                        {
                            idTabela = id;
                            break;
                        }
                    }
                }

                txtIE.Text = dt.Rows[0]["sRG_IE"].ToString();

                ddlEnderecoEntrega.SelectedIndex = 1;
                ddlTabela.SelectedIndex = 1;

                Scripts.FocusScript(Page, ddlEnderecoEntrega.ClientID);
            }
            else
            {
                div_ID.Visible = false;
                div_Referencia.Visible = false;
                div_Endereco.Visible = false;
                div_Obs.Visible = false;
                div_Total.Visible = false;
                div_Itens.Visible = false;

                MensagemPagina.MostraMensagem_Erro("É necessário selecionar um Parceiro!", false);
                Scripts.FocusScript(Page, txtCPF_Parceiro.ClientID);
            }

            dtgItens_DataBind();
        }

        protected void cmdIncluir_Item_Click(object sender, EventArgs e)
        {
            AtualizaClasse_Itens();

            if (ValidarDados_Itens())
            {
                int id = Convert.ToInt32(hddIncluirItem.Value);
                var item = Lista_Itens.Where(x => x.idProduto.Equals(id)).FirstOrDefault();

                if (item == null)
                {
                    Dictionary<string, string> vParam = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTA_TOTAL_ITEM" },
                        { "@idProduto", hddIncluirItem.Value },
                        { "@idTabelaPreco", ddlTabela.SelectedValue },
                    };
                    DataTable dt = BD.ExecutarDataTable(sProcedure, vParam);

                    item = new cls_Pedidos_Itens
                    {
                        idContador = Lista_Itens.Count + 1,
                        sFuncao = "INCLUIR ITEM",
                        idPedido = Convert.ToInt32(hddidCotacao.Value),
                        idProduto = id,
                        sCodigoProduto = txtCodigo_Item.Text,
                        sDscProduto = txtDescricao_Item.Text,
                        sUnidade = dt.Rows[0]["sUnidade"].ToString(),
                        nQuantidade = Convert.ToDouble(txtQuantidade_Item.Text),
                        nValorUnitario = Convert.ToDouble(dt.Rows[0]["nTotal"])
                    };
                    item.nValorTotal = item.nValorUnitario * item.nQuantidade;
                    Lista_Itens.Add(item);
                }
                else
                {
                    if (item.sFuncao == "EXCLUIR ITEM") item.sFuncao = "INCLUIR ITEM";
                    item.nQuantidade += Convert.ToDouble(txtQuantidade_Item.Text);
                }

                LimpaCampos_Itens();
            }

            dtgItens_DataBind();

            Scripts.FocusScript(Page, txtCodigo_Item.ClientID);
        }

        protected void cmdExcluir_Item_Click(object sender, EventArgs e)
        {
            AtualizaClasse_Itens();
            foreach (GridViewRow row in dtgItens.Rows) { if ((row.FindControl("cbExcluir") as CheckBox).Checked) Lista_Itens.FirstOrDefault(item => item.idContador == int.Parse(row.Cells[0].Text)).sFuncao = "EXCLUIR ITEM"; }
            dtgItens_DataBind();
        }

        protected void ddlTabela_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizaClasse_Itens();

            if (ddlTabela.SelectedValue != "0")
            {
                if (Lista_Itens.Count > 0)
                {
                    foreach (var item in Lista_Itens.Where(x => x.sFuncao.ToString() != "EXCLUIR ITEM"))
                    {
                        try
                        {
                            Dictionary<string, string> vParam = new Dictionary<string, string>
                            {
                                { "@sFuncao", "CONSULTA_TOTAL_ITEM" },
                                { "@idProduto", item.idProduto.ToString() },
                                { "@idTabelaPreco", ddlTabela.SelectedValue }
                            };
                            DataTable dt = BD.ExecutarDataTable(sProcedure, vParam);

                            item.sFuncao = "INCLUIR ITEM";
                            item.nValorUnitario = Convert.ToDouble(dt.Rows[0]["nTotal"]);
                            item.nValorTotal = item.nValorUnitario * item.nQuantidade;
                        }
                        catch { }
                    }
                }

                Scripts.FocusScript(Page, txtReferencia.ClientID);
            }

            dtgItens_DataBind();
        }

        protected void cmdAtualizar_Click(object sender, EventArgs e) { AtualizaClasse_Itens(); dtgItens_DataBind(); }

        protected void cmdGerarPDF_Click(object sender, EventArgs e) => GerarPDF_Cotacao();

        protected void cmdImportarItens_Excel_Click(object sender, EventArgs e)
        {
            AtualizaClasse_Itens();

            try
            {
                var itens = ExcelImportar.Retornar_Itens_Excel__Cotacao(ImportarItens_Excel);

                if (itens != null && itens.Count > 0)
                {
                    string mensagem = "";

                    foreach (cls_WMS_Produtos i in itens)
                    {
                        try
                        {
                            Dictionary<string, string> vParam = new Dictionary<string, string>
                            {
                                { "@sFuncao", "CONSULTA_TOTAL_ITEM" },
                                { "@sCodigo", i.SCodigo },
                                { "@idTabelaPreco", ddlTabela.SelectedValue }
                            };
                            DataSet ds = BD.ExecutarDataSet(sProcedure, vParam);

                            if (BD.ValidarDataSet(ds))
                            {
                                int.TryParse(hddidCotacao.Value, out int idCotacao);
                                int.TryParse(RETORNO.DATASET(ds, "idItem"), out int idProduto);
                                double.TryParse(RETORNO.DATASET(ds, "nTotal"), out double nPreco);
                                double.TryParse(i.NQuantidade.ToString(), out double qtd);
                                qtd = qtd <= 0 ? 1 : qtd;

                                var item = Lista_Itens.FirstOrDefault(x => x.idProduto.Equals(idProduto));
                                if (item != null)
                                {
                                    item.sFuncao = "INCLUIR ITEM";
                                    item.nValorUnitario = nPreco;
                                    item.nQuantidade = qtd;
                                    item.nValorTotal = nPreco * qtd;
                                }
                                else
                                {
                                    cls_Pedidos_Itens novoItem = new cls_Pedidos_Itens
                                    {
                                        idContador = Lista_Itens.Count + 1,
                                        sFuncao = "INCLUIR ITEM",
                                        idPedido = idCotacao,
                                        idProduto = idProduto,
                                        sCodigoProduto = i.SCodigo,
                                        sDscProduto = RETORNO.DATASET(ds, "sDscProduto"),
                                        sUnidade = RETORNO.DATASET(ds, "sUnidade"),
                                        nValorUnitario = nPreco,
                                        nQuantidade = qtd,
                                        nValorTotal = nPreco * qtd
                                    };
                                    Lista_Itens.Add(novoItem);
                                }
                            }
                            else mensagem += $"<li>As informações do Item de Código <b>{i.SCodigo}</b> não foram encontradas!</li>";
                        }
                        catch (Exception ex)
                        {
                            mensagem += $"<li>Houve um erro ao Importar o Item de Código <b>{i.SCodigo}</b>! Erro: {ex.Message}</li>";
                        }
                    }

                    if (!string.IsNullOrEmpty(mensagem)) Mensagem_Modal_ImportarItens_Excel.MostraMensagem_Aviso(string.Format("<b>Aviso:</b><br />- Alguns Itens não foram Importados corretamente!<br /><ul>{0}</ul>", mensagem), false);
                    else Mensagem_Modal_ImportarItens_Excel.MostraMensagem_Sucesso("Todos os Itens foram Importados com sucesso!", false);
                }
                else Mensagem_Modal_ImportarItens_Excel.MostraMensagem_Sucesso("Não foram encontrados Itens no Arquivo selecionado!", false);
            }
            catch (Exception ex)
            {
                ex.Source = ex.Message;

                if (ex.Message.Contains("header signature"))
                    ex.Source = "<br />- Não foi possível ler o arquivo, é possível que este esteja corrompido!<br />- Por favor passe as informações dos Itens para um outro Arquivo, de preferência um Novo Arquivo Excel em branco, e tente novamente!";

                Mensagem_Modal_ImportarItens_Excel.MostraMensagem_Erro($"Houve um erro ao Importar os Itens do Arquivo!<br /><b>Erro ao Importar Itens: </b>{ex.Source}", false);
            }

            Scripts.RemoverBackdrop_Modal(Page);
            Scripts.AbrirModal(Page, "modalImportarItens_Excel");

            dtgItens_DataBind();
        }

        #endregion

        #region | PDF

        protected void GerarPDF_Cotacao()
        {
            try
            {
                AtualizaClasse_Itens();

                ReportViewer rv = new ReportViewer();

                rv.LocalReport.EnableExternalImages = true;
                rv.LocalReport.ReportPath = @"App\Reports\Cotacao.rdlc";

                double nTotal_Itens = 0, nTotal_IPI = 0, nTotal = 0;

                List<object> list = new List<object>();
                foreach (var x in Lista_Itens.Where(x => x.sFuncao != "EXCLUIR ITEM"))
                {
                    Dictionary<string, string> vParam = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTA_ITEM__COTACAO_PDF" },
                        { "@idProduto", x.idProduto.ToString() },
                        { "@nVlrProduto", x.nValorUnitario.ToString(CultureInfo.InvariantCulture) },
                        { "@idParceiro", hddidParceiro.Value },
                        { "@idEnderecoEntrega", ddlEnderecoEntrega.SelectedValue }
                    };
                    DataSet ds = BD.ExecutarDataSet(sProcedure, vParam);

                    double.TryParse(RETORNO.DATASET(ds, "nICMS"), out double nICMS);
                    double.TryParse(RETORNO.DATASET(ds, "nIPI"), out double nIPI);
                    double.TryParse(RETORNO.DATASET(ds, "nVlrICMS"), out double vICMS);
                    double.TryParse(RETORNO.DATASET(ds, "nVlrIPI"), out double vIPI);

                    nTotal_Itens += x.nValorTotal - (vIPI * x.nQuantidade);
                    nTotal_IPI += vIPI * x.nQuantidade;
                    nTotal += x.nValorTotal;

                    object item = new
                    {
                        x.idProduto,
                        sCodigo = x.sCodigoProduto,
                        x.sDscProduto,
                        sUnidade = $"{x.sUnidade}\r\n{x.nQuantidade:N2}",
                        nValor = $"R$ {x.nValorUnitario:N2}",
                        nTotal = $"R$ {x.nValorTotal:N2}",

                        nValor_semIPI = $"R$ {(x.nValorUnitario - vIPI):N2}",
                        sIPI = $"{nIPI:N2} %\r\nR$ {vIPI:N2}",
                        sICMS = $"{nICMS:N2} %\r\nR$ {vICMS:N2}"
                    };
                    list.Add(item);
                }

                rv.LocalReport.DataSources.Add(new ReportDataSource("ds_Itens", list));
                rv.LocalReport.DataSources.Add(new ReportDataSource("ds_Vazio", new List<string> { "vazio" }));

                string cnpj = Formatar_CNPJ_CPF(txtCPF_Parceiro.Text);
                string endereco = ddlEnderecoEntrega.SelectedValue != "0" ? ddlEnderecoEntrega.SelectedItem.Text : string.Empty;
                string referencia = !string.IsNullOrEmpty(txtReferencia.Text) ? txtReferencia.Text : string.Empty;
                string condPgto = ddlCondicaoPagamento.SelectedValue != "-1" ? ddlCondicaoPagamento.SelectedItem.Text : string.Empty;
                string obs = !string.IsNullOrEmpty(txtObs.Text) ? txtObs.Text : string.Empty;
                string sTotal = EscreverExtenso(decimal.Parse(nTotal.ToString()));
                DateTime.TryParse(hdddtPedido.Value, out DateTime dtCotacao);

                ReportParameter[] rp = new ReportParameter[13];

                rp[0] = new ReportParameter("sNome_Parceiro", txtNome_Parceiro.Text);
                rp[1] = new ReportParameter("sCNPJ_Parceiro", cnpj.Contains("/") ? $"<b>CNPJ:  </b>{cnpj}" : $"<b>CPF:  </b>{cnpj}");
                rp[2] = new ReportParameter("sIE_Parceiro", txtIE.Text);
                rp[3] = new ReportParameter("sEndereco", endereco);
                rp[4] = new ReportParameter("dtCotacao", dtCotacao.ToString("dd/MM/yyyy"));
                rp[5] = new ReportParameter("nNumeroCotacao", hddnNumeroPedido.Value);
                rp[6] = new ReportParameter("sReferencia", referencia);
                rp[7] = new ReportParameter("nTotal_Itens", nTotal_Itens.ToString("N2"));
                rp[8] = new ReportParameter("nTotal_IPI", nTotal_IPI.ToString("N2"));
                rp[9] = new ReportParameter("nTotal", nTotal.ToString("N2"));
                rp[10] = new ReportParameter("sTotal", sTotal);
                rp[11] = new ReportParameter("sCondPagamento", condPgto);
                rp[12] = new ReportParameter("sObs", obs);

                rv.LocalReport.SetParameters(rp);
                rv.LocalReport.Refresh();

                byte[] bytes = rv.LocalReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings);
                string sNomeArquivo = $"Cotacao_{hddidCotacao.Value.PadLeft(6, '0')}_{CarimboDataHora()}.pdf";
                File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivo, bytes);

                DownloadArquivo(Page, sNomeArquivo);

                Pesquisar(hddidCotacao.Value);
                MensagemPagina.MostraMensagem_Sucesso("Documento em PDF gerado com sucesso!", true);
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro($"Houve um erro ao gerar o Documento em PDF da Cotação!<br />{RetornaMensagem_Erro(ex, seta: "<i class='fa fa-long-arrow-right'></i>")}", true);
            }
        }

        #endregion

        #region | Script

        protected void RegistraScript()
        {
            Scripts.Aplica_TooltipPersonalizado(Page, "tooltip");

            StringBuilder sb = new StringBuilder();

            // Script Personalizado para .autocomplete
            {
                sb.AppendLine(RegistraScript_Personalizado(txtCPF_Parceiro.ClientID, true, false));
                sb.AppendLine(" ");
                sb.AppendLine(RegistraScript_Personalizado(txtNome_Parceiro.ClientID, false, false));
                sb.AppendLine(" ");
                sb.AppendLine(RegistraScript_Personalizado(txtCodigo_Item.ClientID, true, true));
                sb.AppendLine(" ");
                sb.AppendLine(RegistraScript_Personalizado(txtDescricao_Item.ClientID, false, true));
                sb.AppendLine(" ");
                sb.AppendLine(" ");
                sb.AppendLine(" ");
            }

            sb.AppendLine("$(document).ready(function() {");
            sb.AppendLine(" ");
            sb.AppendLine("     if ($('.valor').length > 0) {");
            sb.AppendLine("          $('.valor').mask('0.000.000.009,99', { reverse: true });");
            sb.AppendLine("     }");
            sb.AppendLine(" ");
            sb.AppendLine("     $('.excluirTodos input').on('change', function() {");
            sb.AppendLine("         var excluir = $(this).prop('checked');");
            sb.AppendLine("         $('.excluir input').each(function () {");
            sb.AppendLine("             $(this).prop('checked', excluir);");
            sb.AppendLine("         });");
            sb.AppendLine("     });");
            sb.AppendLine(" ");
            sb.AppendLine("     $('.qtd, .preco').on('input', function() {");
            sb.AppendLine("         var row = $(this).closest('tr');");
            sb.AppendLine("         var qtd = parseFloat($(row).find('.qtd').val().replace(/\\./g, '').replace(',', '.'));");
            sb.AppendLine("         var valor = parseFloat($(row).find('.preco').val().replace(/\\./g, '').replace(',', '.'));");
            sb.AppendLine("         var total = (qtd * valor) || 0;");
            sb.AppendLine("         $(row).find('.total').text(total.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }));");
            sb.AppendLine("     });");
            sb.AppendLine(" ");
            sb.AppendLine("});");
            sb.AppendLine(" ");
            sb.AppendLine("$v192(function() {");
            sb.AppendLine("     $v192('#dialog-Salvar').dialog({");
            sb.AppendLine("         resizable: false,");
            sb.AppendLine("         height: 'auto',");
            sb.AppendLine("         width: 400,");
            sb.AppendLine("         modal: true,");
            sb.AppendLine("         autoOpen: false,");
            sb.AppendLine("         buttons:");
            sb.AppendLine("         {");
            sb.AppendLine("             'Sim': function() {");
            sb.AppendLine("                 __doPostBack('funcao_SALVAR', '');");
            sb.AppendLine("                 $v192(this).dialog('close');");
            sb.AppendLine("             },");
            sb.AppendLine("             'Não': function() {");
            sb.AppendLine("                 $v192(this).dialog('close');");
            sb.AppendLine("             },");
            sb.AppendLine("         }");
            sb.AppendLine("     });");
            sb.AppendLine("     $v192('[id*=cmdSalvar]').click(function(e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         $v192('#dialog-Salvar').dialog('open');");
            sb.AppendLine("     });\r\n");
            sb.AppendLine("     $v192('#dialog-Editar').dialog({");
            sb.AppendLine("         resizable: false,");
            sb.AppendLine("         height: 'auto',");
            sb.AppendLine("         width: 400,");
            sb.AppendLine("         modal: true,");
            sb.AppendLine("         autoOpen: false,");
            sb.AppendLine("         buttons:");
            sb.AppendLine("             {");
            sb.AppendLine("                 'Sim': function() {");
            sb.AppendLine("                     __doPostBack('funcao_EDITAR', '');");
            sb.AppendLine("                     $v192(this).dialog('close');");
            sb.AppendLine("                 },");
            sb.AppendLine("                 'Não': function() {");
            sb.AppendLine("                     $v192(this).dialog('close');");
            sb.AppendLine("                 },");
            sb.AppendLine("             }");
            sb.AppendLine("     });");
            sb.AppendLine("     $v192('[id*=cmdEditar]').click(function(e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         $v192('#dialog-Editar').dialog('open');");
            sb.AppendLine("     });");
            sb.AppendLine(" ");
            sb.AppendLine("});");
            sb.AppendLine(" ");

            // Card de informações dos EPIs
            {
                sb.Append("     var cardTimer = { };\r\n");
                sb.Append("     function mostraCard(element, idProduto, tabela) {\r\n");
                sb.Append("         cardTimer[idProduto + '_' + tabela] = setTimeout(function() {\r\n");
                sb.Append("             $.ajax({\r\n");
                sb.Append("                 url: \"/API/Pagina_Ajax.aspx/GetProdutoDetalhes\",\r\n");
                sb.Append("                 data: JSON.stringify({ idProduto: idProduto }),\r\n");
                sb.Append("                 type: 'POST',\r\n");
                sb.Append("                 dataType: 'json',\r\n");
                sb.Append("                 contentType: 'application/json; charset=utf-8',\r\n");
                sb.Append("                 success: function(response) {\r\n");
                sb.Append("                     var produto = JSON.parse(response.d);\r\n");
                sb.Append("                     var cardProduto = `\r\n");
                sb.Append("                         <div class=\"card\">\r\n");
                sb.Append("                             <div class=\"card-body d-flex\">\r\n");
                sb.Append("                                 <div class=\"flex-shrink-0\" style=\"min-inline-size: fit-content;\">\r\n");
                sb.Append("                                     ${produto.imagem? `<img src = \"${produto.imagem}\" alt=\"Imagem do Produto\" class=\"img-fluid img-thumbnail\" style=\"width: 100px; height: auto;\" />` : ''}\r\n");
                sb.Append("                                 </div>\r\n");
                sb.Append("                                 <div class=\"flex-grow-1 d-flex flex-column ms-3\">\r\n");
                sb.Append("                                     <div class=\"d-flex\">\r\n");
                sb.Append("                                         ${produto.sCategoriaVendas? `<div class=\"card-text me-3\"> <strong>Categoria Vendas: </strong>${produto.sCategoriaVendas\r\n}</div>` : ''}\r\n");
                sb.Append("                                         ${ produto.sFabricante ? `<div class= \"card-text me-3\"> <strong > Fabricante: </strong >${ produto.sFabricante}</div>` : ''}\r\n");
                sb.Append("                                         ${ produto.sTipo ? `<div class= \"card-text me-3\"> <strong > Tipo: </strong >${ produto.sTipo}</div>` : ''}\r\n");
                sb.Append("                                         ${ produto.sGrupo ? `<div class= \"card-text me-3\"> <strong > Grupo: </strong >${ produto.sGrupo}</div>` : ''}\r\n");
                sb.Append("                                         ${ produto.sFamilia ? `<div class= \"card-text me-3\"> <strong > Família: </strong >${ produto.sFamilia}</div>` : ''}\r\n");
                sb.Append("                                         ${ produto.sPaisOrigem ? `<div class= \"card-text me-3\"> <strong > Origem: </strong >${ produto.sPaisOrigem}</div>` : ''}\r\n");
                sb.Append("                                         ${ produto.sLocalArmazenamento ? `<div class= \"card-text me-3\"> <strong > Local Armazenamento: </strong >${ produto.sLocalArmazenamento}</div>` : ''}\r\n");
                sb.Append("                                     </div>\r\n");
                sb.Append("                                 </div>\r\n");
                sb.Append("                             </div>\r\n");
                sb.Append("                         </div>\r\n");
                sb.Append("                     `;\r\n");
                sb.Append("                     var cardId = idProduto + '_' + tabela;\r\n");
                sb.Append("                     var card = document.getElementById(cardId);\r\n");
                sb.Append("                     card.innerHTML = cardProduto;\r\n");
                sb.Append("                     var rect = element.getBoundingClientRect();\r\n");
                sb.Append("                     var scrollTop = document.documentElement.scrollTop || document.body.scrollTop;\r\n");
                sb.Append("                     var scrollLeft = document.documentElement.scrollLeft || document.body.scrollLeft;\r\n");
                sb.Append("                     hideAllCards();\r\n");
                sb.Append("                     card.style.top = (rect.top + scrollTop - 10) + 'px';\r\n");
                sb.Append("                     card.style.left = (rect.right + scrollLeft + element.offsetWidth + 10) + 'px';\r\n");
                sb.Append("                     card.style.display = 'block';\r\n");
                sb.Append("                 },\r\n");
                sb.Append("                 error: function(error) {\r\n");
                sb.Append("                     console.error(\"Erro ao obter os detalhes do produto:\", error);\r\n");
                sb.Append("                 }\r\n");
                sb.Append("             });\r\n");
                sb.Append("         }, 300);\r\n");
                sb.Append("     }\r\n\r\n");
                sb.Append("     function escondeCard(idProduto, tabela) {\r\n");
                sb.Append("         var cardId = idProduto + '_' + tabela;\r\n");
                sb.Append("         var card = document.getElementById(cardId);\r\n");
                sb.Append("         clearTimeout(cardTimer[idProduto + '_' + tabela]);\r\n");
                sb.Append("         card.style.display = 'none';\r\n");
                sb.Append("     }\r\n\r\n");
                sb.Append("     function hideAllCards() {\r\n");
                sb.Append("         var cards = document.querySelectorAll('.product-card');\r\n");
                sb.Append("         cards.forEach(function(card) {\r\n");
                sb.Append("             card.style.display = 'none';\r\n");
                sb.Append("         });\r\n");
                sb.Append("     }\r\n\r\n");
                sb.Append("     function openModal(idProduto) {\r\n");
                sb.Append("         $.ajax({\r\n");
                sb.Append("             url: \"/API/Pagina_Ajax.aspx/GetProdutoDetalhes\",\r\n");
                sb.Append("             data: JSON.stringify({ idProduto: idProduto }),\r\n");
                sb.Append("             type: 'POST',\r\n");
                sb.Append("             dataType: 'json',\r\n");
                sb.Append("             contentType: 'application/json; charset=utf-8',\r\n");
                sb.Append("             success: function(response) {\r\n");
                sb.Append("                 var produto = JSON.parse(response.d);\r\n");
                sb.Append("                 var tituloProduto = `\r\n");
                sb.Append("                     <button type = \"button\" class= \"close\" data - dismiss = \"modal\" aria - label = \"Close\">\r\n");
                sb.Append("                         <span aria - hidden = \"true\" > &times;</span>\r\n");
                sb.Append("                     </button>\r\n");
                sb.Append("                     <h5 class= \"modal-title\" id = \"detailsModalLabel\" > ${ produto.sCodigo} - ${ produto.sDsc}</h5>\r\n");
                sb.Append("                 `;\r\n");
                sb.Append("                 var modalInfo = document.getElementById('modalInfo');\r\n");
                sb.Append("                 modalInfo.innerHTML = tituloProduto;\r\n");
                sb.Append("                 var imagem = '';\r\n");
                sb.Append("                 if (produto.imagem) {\r\n");
                sb.Append("                     imagem += `\r\n");
                sb.Append("                         <div style = \"text-align: center; margin-bottom: 20px;\">\r\n");
                sb.Append("                             <img src = \"${produto.imagem}\" alt = \"Imagem do Produto\" class= \"img-fluid\" style = \"width: 300px; height: auto;\" />\r\n");
                sb.Append("                         </div>\r\n");
                sb.Append("                     `;\r\n");
                sb.Append("                 }\r\n");
                sb.Append("                 var tabelaProduto = '<table class=\"table table-bordered\">';\r\n");
                sb.Append("                 if (produto.sCategoriaVendas) {\r\n");
                sb.Append("                     tabelaProduto += `\r\n");
                sb.Append("                         <tr>\r\n");
                sb.Append("                             <th> Categoria Vendas </th>\r\n");
                sb.Append("                             <td>${ produto.sCategoriaVendas}</td>\r\n");
                sb.Append("                         </tr>\r\n");
                sb.Append("                     `;\r\n");
                sb.Append("                 }\r\n");
                sb.Append("                 if (produto.sTipo) {\r\n");
                sb.Append("                     tabelaProduto += `\r\n");
                sb.Append("                         <tr>\r\n");
                sb.Append("                             <th> Tipo </th>\r\n");
                sb.Append("                             <td>${ produto.sTipo}</td>\r\n");
                sb.Append("                         </tr>\r\n");
                sb.Append("                     `;\r\n");
                sb.Append("                 }\r\n");
                sb.Append("                 if (produto.sGrupo) {\r\n");
                sb.Append("                     tabelaProduto += `\r\n");
                sb.Append("                         <tr>\r\n");
                sb.Append("                             <th> Grupo </th>\r\n");
                sb.Append("                             <td>${ produto.sGrupo}</td>\r\n");
                sb.Append("                         </tr>\r\n");
                sb.Append("                     `;\r\n");
                sb.Append("                 }\r\n");
                sb.Append("                 if (produto.sFabricante) {\r\n");
                sb.Append("                     tabelaProduto += `\r\n");
                sb.Append("                         <tr>\r\n");
                sb.Append("                             <th> Fabricante </th>\r\n");
                sb.Append("                             <td>${ produto.sFabricante}</td>\r\n");
                sb.Append("                         </tr>\r\n");
                sb.Append("                     `;\r\n");
                sb.Append("                 }\r\n");
                sb.Append("                 if (produto.sLocalArmazenamento) {\r\n");
                sb.Append("                     tabelaProduto += `\r\n");
                sb.Append("                         <tr>\r\n");
                sb.Append("                             <th> Local Armazenamento </th>\r\n");
                sb.Append("                             <td>${ produto.sLocalArmazenamento}</td>\r\n");
                sb.Append("                         </tr>\r\n");
                sb.Append("                     `;\r\n");
                sb.Append("                 }\r\n");
                sb.Append("                 if (produto.sFamilia) {\r\n");
                sb.Append("                     tabelaProduto += `\r\n");
                sb.Append("                         <tr>\r\n");
                sb.Append("                             <th> Família </th>\r\n");
                sb.Append("                             <td>${ produto.sFamilia}</td>\r\n");
                sb.Append("                         </tr>\r\n");
                sb.Append("                     `;\r\n");
                sb.Append("                 }\r\n");
                sb.Append("                 if (produto.sCodigoCEST) {\r\n");
                sb.Append("                     tabelaProduto += `\r\n");
                sb.Append("                         <tr>\r\n");
                sb.Append("                             <th> CEST </th>\r\n");
                sb.Append("                             <td>${ produto.sCodigoCEST}</td>\r\n");
                sb.Append("                         </tr>\r\n");
                sb.Append("                     `;\r\n");
                sb.Append("                 }\r\n");
                sb.Append("                 if (produto.sCodigoNCM) {\r\n");
                sb.Append("                     tabelaProduto += `\r\n");
                sb.Append("                         <tr>\r\n");
                sb.Append("                             <th> NCM </th>\r\n");
                sb.Append("                             <td>${ produto.sCodigoNCM}</td>\r\n");
                sb.Append("                         </tr>\r\n");
                sb.Append("                     `;\r\n");
                sb.Append("                 }\r\n");
                sb.Append("                 if (produto.sPaisOrigem) {\r\n");
                sb.Append("                     tabelaProduto += `\r\n");
                sb.Append("                         <tr>\r\n");
                sb.Append("                             <th> Origem </th>\r\n");
                sb.Append("                             <td>${ produto.sPaisOrigem}</td>\r\n");
                sb.Append("                         </tr>\r\n");
                sb.Append("                     `;\r\n");
                sb.Append("                 }\r\n");
                sb.Append("                 tabelaProduto += `</table >`;\r\n");
                sb.Append("                 var modalBody = document.getElementById('modalBody');\r\n");
                sb.Append("                 modalBody.innerHTML = imagem + tabelaProduto;\r\n");
                sb.Append("                 $('#produtoDetalheModal').modal('show');\r\n");
                sb.Append("             },\r\n");
                sb.Append("             error: function(error) {\r\n");
                sb.Append("                 console.error(\"Erro ao obter os detalhes do produto:\", error);\r\n");
                sb.Append("             }\r\n");
                sb.Append("         });\r\n");
                sb.Append("     }\r\n");
                sb.Append("     function openProductDetail(idProduto) {\r\n");
                sb.Append("         var url = '/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id=' + idProduto;\r\n");
                sb.Append("         window.open(url, '_blank');\r\n");
                sb.Append("         return false;\r\n");
                sb.Append("     }\r\n");
            }

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_RegistraScript", sb.ToString(), true);
        }

        protected string RegistraScript_Personalizado(string clientID, bool bCodigo, bool bItem)
        {
            //string list = bItem ? JsonConvert.SerializeObject(hddItens.Value.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries)) : JsonConvert.SerializeObject(hddParceiros.Value.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries));
            string list = bItem ? hddItens.Value : hddParceiros.Value;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("$v192(function() {");
            sb.AppendLine("     $v192('[id*=" + clientID + "]').autocomplete({");
            sb.AppendLine("         source: function(request, response) {");
            sb.AppendLine("             $v192.ajax({");
            sb.AppendLine("                 url: '/API/Pagina_Ajax.aspx/Get',");
            sb.AppendLine("                 data: JSON.stringify({");
            sb.AppendLine("                     'list': " + list + ", ");
            sb.AppendLine("                     'sDsc': JSON.stringify(request.term), ");
            sb.AppendLine("                     'sCodigo': " + bCodigo.ToString().ToLower() + "");
            sb.AppendLine("                 }),");
            sb.AppendLine("                 dataType: 'json',");
            sb.AppendLine("                 type: 'POST',");
            sb.AppendLine("                 contentType: 'application/json; charset=utf-8',");
            sb.AppendLine("                 success: function(data) {");
            sb.AppendLine("                     response($v192.map(data.d, function(item) {");
            sb.AppendLine("                         return {");
            sb.AppendLine("                             label: item." + (bCodigo ? "sCodigo" : "sDsc") + ",");
            sb.AppendLine(hddIncluirItem.ClientID + ": item.id,");
            sb.AppendLine(txtCodigo_Item.ClientID + ": item.sCodigo,");
            sb.AppendLine(txtDescricao_Item.ClientID + ": item.sDsc");
            sb.AppendLine("                         };");
            sb.AppendLine("                     }));");
            sb.AppendLine("                 },");
            sb.AppendLine("                 error: function(response) {");
            sb.AppendLine("                     console.error(response.responseText);");
            sb.AppendLine("                 },");
            sb.AppendLine("                 failure: function(response) {");
            sb.AppendLine("                     console.error(response.responseText);");
            sb.AppendLine("                 }");
            sb.AppendLine("             });");
            sb.AppendLine("         },");
            sb.AppendLine("         select: function(e, i) {");
            sb.AppendLine("             $('[id$=" + (bItem ? hddIncluirItem.ClientID : hddidParceiro.ClientID) + "]').val(i.item." + hddIncluirItem.ClientID + ");");
            sb.AppendLine("             $('[id$=" + (bItem ? txtCodigo_Item.ClientID : txtCPF_Parceiro.ClientID) + "]').val(i.item." + txtCodigo_Item.ClientID + ");");
            sb.AppendLine("             $('[id$=" + (bItem ? txtDescricao_Item.ClientID : txtNome_Parceiro.ClientID) + "]').val(i.item." + txtDescricao_Item.ClientID + ");");
            sb.AppendLine("             $('[id$=" + (bItem ? txtQuantidade_Item.ClientID : cmdSelecionarParceiro.ClientID) + "]').focus();");
            sb.AppendLine("         },");
            sb.AppendLine("         minLength: 3");
            sb.AppendLine("     });");
            sb.AppendLine("});");

            return sb.ToString();

        }

        #endregion
    }
}