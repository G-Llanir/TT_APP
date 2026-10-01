using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.FrameWork;
using static TT.FrameWork.BD;
using static TT.FrameWork.BD.Retorno;
using static TT.FrameWork.Funcoes;
using static TT.FrameWork.Identity;

namespace TT_Flow.App.Paginas.Comercial
{
    public partial class Tabelas_Detalhe : Page
    {
        #region | Construtores

        public static readonly string sProcedure = "sp_Manipula_tbl_Flow_Comercial_TabelaPreco";
        public static readonly string sProcedure_Produtos = "sp_Manipula_tbl_Flow_Produtos";
        public static readonly string sProcedure_Clientes = "sp_Manipula_tbl_Flow_Clientes";

        static int nCol_Total = 17;
        static int nCol_Foto = 23;

        public class Item_Excel
        {
            public string SCodigo { get; set; }
            public decimal Preco { get; set; }
        }

        #endregion

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "Manual-TabelaPreco.pdf";
            ExcelImportar.ID_FileUpload = ImportarArquivo.ID;

            ComboValidada.Text = "Tabela Validada";

            // --------------------------------------------------------------------------------
            // Mensagens via JS

            MensagemPagina.MostraMensagem_Sucesso(".", false);
            MensagemIncluirItem.MostraMensagem_Sucesso(".", false);
            MensagemItens.MostraMensagem_Sucesso(".", false);
            MensagemControleValores.MostraMensagem_Sucesso(".", false);
            MensagemHistorico.MostraMensagem_Sucesso(".", false);
            Mensagem_ModalValidarItens.MostraMensagem_Sucesso(".", false);
            Mensagem_ModalDesvincularTabelas.MostraMensagem_Sucesso(".", false);

            // Mensagens Fixas
            Mensagem_ExportacaoLPU.MostraMensagem_Aviso("Este bloco irá salvar a lista de Itens sendo exibidos na Ordem atual, para que possa ser Exportado em Excel!", false);
            Mensagem_Modal_ImportarProdutos.MostraMensagem(@"<b>Lembrete</b><br />- O arquivo Excel será lido, considerando apenas as primeiras colunas do Arquivo, dentro o seguinte esquema:<br /><br />
                                                                    <table style=""width: 100%;"">
                                                                        <thead>
                                                                            <tr>
                                                                                <th style=""padding: 5px;text-align: center;border: 1px solid black;"">Código do Produto</th>
                                                                                <th style=""padding: 5px;text-align: center;border: 1px solid black;"">Valor</th>
                                                                            </tr>
                                                                        </thead>
                                                                        <tbody>
                                                                            <tr>
                                                                                <td style=""padding: 5px;text-align: center;border: 1px solid black;"">Código_do_Produto</td>
                                                                                <td style=""padding: 5px;text-align: center;border: 1px solid black;"">10,00</td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>", "INFO", false);

            // --------------------------------------------------------------------------------

            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request["id"].ToString()))
                {
                    ValidaPermissao(Permissao.Comercial.TabelaDePreco.Consultar, true);

                    if (int.TryParse(Request["filtroItem"], out int idItem))
                        hddFiltro_ID.Value = idItem <= 0 ? "0" : idItem.ToString();
                    else
                        hddFiltro_ID.Value = "0";

                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    ValidaPermissao(Permissao.Comercial.TabelaDePreco.Master, true);
                    Pesquisar("0");
                }
            }

            var requestTarget = Request["__EVENTTARGET"];
            if (requestTarget == "funcao_CADEADO")
                BloquearEdicao_Cadeado();

            Scripts.Aplica_TooltipPersonalizado(Page, "tooltip");

            RegistraScript_Tabelas();
        }

        protected void Pesquisar(string idPesquisa)
        {
            LimparCampos();
            PopulaCombos();

            hddsPermissaoCadeado.Value = ValidaPermissao(Permissao.Comercial.TabelaDePreco.BloquearEdicao, false) ? "S" : "N";

            if (idPesquisa != "0")
            {
                eArquivos.Attributes.Add("src", string.Format("../Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}", idPesquisa, "TabelaPreco"));

                DataSet dsPesquisa = ExecutarDataSet(sProcedure, new Dictionary<string, string> { { "@sFuncao", "CONSULTAR_DETALHE" }, { "@idTabela", idPesquisa } });
                if (ValidarDataSet(dsPesquisa, out string sErro))
                {
                    DateTime.TryParse(DATASET(dsPesquisa, "dtVigencia_Inicial"), out DateTime dtInicio);
                    DateTime.TryParse(DATASET(dsPesquisa, "dtVigencia_Final"), out DateTime dtFinal);
                    int.TryParse(DATASET(dsPesquisa, "nItens"), out int nItens);

                    if (nItens > 500) cmdConsulta_Itens_Todos.Attributes["title"] = "A quantidade de Itens consultados pode afetar o Desempenho da página!";

                    hddsCadeado.Value = DATASET(dsPesquisa, "sEdicao_Cadeado");

                    txtTituloTabela.Text = DATASET(dsPesquisa, "sDscTabela");
                    txtObservacao.Text = DATASET(dsPesquisa, "sObservacao");
                    txtdtInicial.Text = dtInicio.ToString("yyyy-MM-dd");
                    txtdtFinal.Text = dtFinal.ToString("yyyy-MM-dd");

                    ddlidMoedaOrigem.SelectedValue = DATASET(dsPesquisa, "idMoedaOrigem");
                    ddlidMoedaDestino.SelectedValue = DATASET(dsPesquisa, "idMoedaDestino");

                    hddidTabela.Value = DATASET(dsPesquisa, "idTabela");
                    Popula_Combo(ddlTipoTabela, $"sp_Select 'tbl_Flow_Comercial_TabelaPreco_Tipo', {hddidTabela.Value}", "idTipoTabela", "sDscTipoTabela", false, "Selecione a Tabela", "0");
                    Popula_Combo(lstFiltros_Consulta, $"sp_Select 'Filtro_Grupos_TabelaPreco', {hddidTabela.Value}", "idGrupo", "sDscGrupo", true, "Todos os Grupos", "-1");
                    ddlTipoTabela.SelectedValue = DATASET(dsPesquisa, "idTipoTabela");

                    hddTipoTabela.Value = ddlTipoTabela.SelectedValue;
                    hddTerritorio.Value = ddlidMoedaOrigem.SelectedValue == "2" ? "N" : "I";
                    hddCalculaNCM.Value = DATASET(dsPesquisa, "sCalculaImpostos");

                    hStatus_LPU.Visible = false;
                    if (hddTipoTabela.Value.Equals("11"))
                    {
                        hStatus_LPU.Visible = true;
                        bool vigente = DateTime.Today.Between(dtInicio.Date, dtFinal.Date);
                        hStatus_LPU.InnerHtml = $"<span class='label label-{(vigente ? "success" : "danger")}'>{(vigente ? "Vigente" : "Fora de Vigência")}</span>";
                    }

                    txtidTabela.Text = hddidTabela.Value;

                    decimal.TryParse(DATASET(dsPesquisa, "nFator"), out decimal nFator);
                    decimal.TryParse(DATASET(dsPesquisa, "nDesconto"), out decimal nDesconto);
                    decimal.TryParse(DATASET(dsPesquisa, "nTaxaEnvio"), out decimal nEnvio);
                    decimal.TryParse(DATASET(dsPesquisa, "nTaxaLocal"), out decimal nLocal);
                    decimal.TryParse(DATASET(dsPesquisa, "nMargem"), out decimal nMargem);
                    txtFator.Text = Math.Round(nFator, 2).ToString();
                    txtDesconto.Text = Math.Round(nDesconto, 2).ToString();
                    txtEnvio.Text = Math.Round(nEnvio, 2).ToString();
                    txtLocal.Text = Math.Round(nLocal, 2).ToString();
                    txtMargem.Text = Math.Round(nMargem, 2).ToString();
                    ComboAtivo.Situacao_Definir(DATASET(dsPesquisa, "sSituacao").Equals("0") ? "N" : DATASET(dsPesquisa, "sSituacao"));
                    ComboValidada.Situacao_Definir(DATASET(dsPesquisa, "sValidada").Equals("0") ? "N" : DATASET(dsPesquisa, "sSituacao"));

                    PopulaCombo_Vincula_Fornecedores_Parceiros();

                    try
                    {
                        foreach (string i in DATASET(dsPesquisa, "sidFornecedores").Split('|'))
                        {
                            if (!string.IsNullOrEmpty(i) && !string.IsNullOrWhiteSpace(i) && int.Parse(i) > 0 && lstFornecedores.Items.Contains(lstFornecedores.Items.FindByValue(i)))
                                lstFornecedores.Items.FindByValue(i).Selected = true;
                        }
                    }
                    catch { }

                    try
                    {
                        foreach (string i in DATASET(dsPesquisa, "sidParceiros").Split('|'))
                        {
                            if (!string.IsNullOrEmpty(i) && !string.IsNullOrWhiteSpace(i) && int.Parse(i) > 0 && lstParceiros.Items.Contains(lstParceiros.Items.FindByValue(i)))
                                lstParceiros.Items.FindByValue(i).Selected = true;
                        }
                    }
                    catch { }

                    if (ddlTipoTabela.SelectedValue != "8")
                    {
                        hddFiltro_Vinculos.Value = string.Empty;
                        lstFiltroTipos_Vinculos.Items.Clear();
                        foreach (DataRow row in dsPesquisa.Tables[4].Rows)
                        {
                            hddFiltro_Vinculos.Value += row["sFiltroTipos"].ToString().Split('-')[0].Trim() + "|";

                            ListItem item = new ListItem
                            {
                                Value = row["sFiltroTipos"].ToString().Split('-')[0].Trim(),
                                Text = row["sFiltroTipos"].ToString().Split('-')[1].Trim(),
                                Selected = true
                            };

                            lstFiltroTipos_Vinculos.Items.Add(item);
                        }

                        Popular_RepeaterVinculos(hddidTabela.Value);
                    }

                    ddlFiltroOrigem.Items.Add(new ListItem("Todas as Origens", "-1"));

                    if (ddlTipoTabela.SelectedValue != "1")
                        ddlFiltroOrigem.Items.Add(new ListItem(string.Format("0 - {0}", txtTituloTabela.Text), "0"));

                    hddIdsTabelasImportadas.Value = "|";
                    foreach (DataRow row in dsPesquisa.Tables[2].Rows)
                    {
                        string idTabela = row[0].ToString();

                        hddIdsTabelasImportadas.Value += string.Format("{0}|", idTabela);
                        ddlFiltroOrigem.Items.Add(new ListItem(string.Format("{0} - {1}", idTabela, row[1].ToString()), idTabela));
                    }

                    PopulaCombo_ImportarTabelas();

                    try
                    {
                        hddTerritorio.Value = ddlidMoedaOrigem.SelectedValue == "2" ? "N" : "I";

                        Dictionary<string, string> vParam = new Dictionary<string, string>
                        {
                            { "@sFuncao", "CONSULTA_MOEDAS" },
                            { "@idMoedaOrigem", ddlidMoedaOrigem.SelectedValue },
                            { "@idMoedaDestino", ddlidMoedaDestino.SelectedValue }
                        };
                        DataTable dt = ExecutarDataTable(sProcedure, vParam);

                        var origem_Valor = dt.Rows[0].Field<decimal>("nValorCambio");
                        var origem_Simbolo = dt.Rows[0].Field<string>("sSimbolo");
                        var destino_Valor = dt.Rows[1].Field<decimal>("nValorCambio");
                        var destino_Simbolo = dt.Rows[1].Field<string>("sSimbolo");

                        if (ddlidMoedaOrigem.SelectedValue == "0")
                        {
                            txtCambio.Text = "";
                            MensagemPagina.MostraMensagem("Selecione uma " + lblMoedaOrigem.InnerText + "!", "info", false);

                            return;
                        }

                        if (ddlidMoedaDestino.SelectedValue == "0")
                        {
                            txtCambio.Text = "";
                            MensagemPagina.MostraMensagem("Selecione uma " + lblMoedaDestino.InnerText + "!", "info", false);

                            return;
                        }

                        if (ddlidMoedaOrigem.SelectedValue != "0")
                            hddComposicao_nMoedaOrigem.Value = origem_Valor.ToString();

                        if (ddlidMoedaDestino.SelectedValue != "0")
                            hddComposicao_nMoedaDestino.Value = destino_Valor.ToString();

                        if (hddComposicao_nMoedaDestino.Value != "" && origem_Valor > decimal.Zero)
                        {
                            decimal valorBr = origem_Valor / destino_Valor;
                            txtCambio.Text = valorBr.ToString("N4");
                        }

                        lblSimboloMoeda_Cambio.InnerText = string.Format("{0} / {1}", !string.IsNullOrEmpty(origem_Simbolo) ? origem_Simbolo : "", !string.IsNullOrEmpty(destino_Simbolo) ? destino_Simbolo : "");
                    }
                    catch { }

                    lblTituloPagina.Text = string.Format("Editar {0}", txtTituloTabela.Text);
                    PainelAtualizacao.Visible = true;
                    PainelAtualizacao.Atualizar(DATASET(dsPesquisa, "dtAtualizacao"), DATASET(dsPesquisa, "sDscUsuarioAtualizacao"));

                    BloquearEdicao_TipoTabela(DATASET(dsPesquisa, "sPodeEditar") == "S");
                }
                else if (!string.IsNullOrEmpty(sErro))
                    throw new Exception("Erro ao consultar Dados da Tabela de Preços: " + sErro);

                if (ddlTipoTabela.SelectedValue == "8")
                {
                    Popula_Combo(lstGrupos, "sp_Select 'Flow_WMS_Produtos_Grupos_PAI', @idPesquisa=2", "idGrupo", "sDscGrupo", false);
                    Popula_Combo(ddlFiltroGrupo, "sp_Select 'Flow_WMS_Produtos_Grupos_PAI', @idPesquisa=2", "idGrupo", "sDscGrupo", true, "Todos os Grupos", "-1");
                    Popula_Combo(ddlFiltroTipo, "sp_Select 'Flow_Produtos_Tipo', @idPesquisa=2", "idTipoProduto", "sDscTipoProduto", false, "Todos os Tipos", "0");
                    Popula_Combo(ddlFiltroUnidade, "sp_Select 'Flow_Produtos_Unidade', @sPesquisa='S'", "sUnidade", "sDscUnidade", true, "Todas as Unidades", "0");

                    ddlControle_Filtro_Unidade.Items.Clear();
                    ddlControle_Filtro_Tipo.Items.Clear();
                    ddlControle_Filtro_Grupo.Items.Clear();

                    ddlControle_Filtro_Unidade.Items.AddRange(ddlFiltroUnidade.Items.Cast<ListItem>().ToArray());
                    ddlControle_Filtro_Tipo.Items.AddRange(ddlFiltroTipo.Items.Cast<ListItem>().ToArray());
                    ddlControle_Filtro_Grupo.Items.AddRange(ddlFiltroGrupo.Items.Cast<ListItem>().ToArray());

                    Popula_Combo(lstFiltros_Consulta, $"sp_Select 'Filtro_Grupos_TabelaPreco', {hddidTabela.Value}", "idGrupo", "sDscGrupo", true, "Todos os Grupos", "-1");
                }

                lblTituloSalvar.Text = string.Format("Confirmar Alteraçõs de {0}?", txtTituloTabela.Text);

                if (hddCalculaNCM.Value == "N" && (ddlTipoTabela.SelectedValue == "2" || ddlTipoTabela.SelectedValue == "4" || ddlTipoTabela.SelectedValue == "5"))
                {
                    cmdCalculaNCM.Text = "Calcular com Impostos (NCM)";
                    EsconderColunas(dtgItens, "Impostos (NCM)");
                }

                spanCadeado.Attributes["class"] = hddsCadeado.Value.Equals("S") ? "shackle shackle-open" : "shackle shackle-closed";
                lbl_Lock.Attributes["class"] = hddsPermissaoCadeado.Value.Equals("S") ? "lock-label" : "lock-label lock-label-closed";
            }
            else
            {
                txtidTabela.Text = "Novo";
                lblTituloPagina.Text = "Nova Tabela de Preço";

                BloquearEdicao_TipoTabela(true);

                ComboValidada.Situacao_Definir(ValidaPermissao(Permissao.Comercial.TabelaDePreco.Master) ? "S" : "N");
                ComboValidada.Situacao_BloquearEdicao(false);

                txtTituloTabela.Focus();
                BreadCrumb.TitulodaPagina = "Incluir";
                cmdSalvar.Text = "Incluir";

                if (!ValidaPermissao(Permissao.Comercial.TabelaDePreco.Master))
                {
                    string values = "|";
                    foreach (ListItem item in ddlTipoTabela.Items)
                    {
                        values += item.Value != "5" && item.Value != "0" ? item.Value + "|" : "";
                    }

                    foreach (string value in values.Split('|'))
                    {
                        ddlTipoTabela.Items.Remove(ddlTipoTabela.Items.FindByValue(value));
                    }
                }

                if (!(ValidaPermissao(Permissao.Comercial.TabelaDePreco.Editar_Fornecedor_Nacional)
                     || ValidaPermissao(Permissao.Comercial.TabelaDePreco.Master)))
                    ddlidMoedaOrigem.Items.Remove(ddlidMoedaOrigem.Items.FindByValue("2"));

                if (!(ValidaPermissao(Permissao.Comercial.TabelaDePreco.Editar_Fornecedor_Internacional)
                     || ValidaPermissao(Permissao.Comercial.TabelaDePreco.Master)))
                {
                    string values = "|";
                    foreach (ListItem item in ddlidMoedaOrigem.Items)
                    {
                        values += item.Value != "2" && item.Value != "0" ? item.Value + "|" : "";
                    }

                    foreach (string value in values.Split('|'))
                    {
                        ddlidMoedaOrigem.Items.Remove(ddlidMoedaOrigem.Items.FindByValue(value));
                    }
                }

                txtdtInicial.Text = ((DateTime)default).ToString("yyyy-MM-dd");
                txtdtFinal.Text = ((DateTime)default).ToString("yyyy-MM-dd");
                lblTituloSalvar.Text = "Confirmar Inclusão de Nova Tabela?";
            }

            var a = new List<int> { 0 };
            dtgItens.DataSource = a;
            gvControle_Grupo.DataSource = a;
            gvControle_Familia.DataSource = a;
            gvControleValores.DataSource = a;

            dtgItens.DataBind();
            gvControle_Grupo.DataBind();
            gvControle_Familia.DataBind();
            gvControleValores.DataBind();

            gv_Historico.DataSource = new List<object> { new { idRegistroHistorico = 0, sCodigo = "", sDscAlteracao = "", nPreco = 0, nFator = 0, nTaxaEnvio = 0, nTaxaLocal = 0, nMargem = 0, nTaxaImpostos = 0, nTotal = 0, sDscUsuarioAtualizacao = "", dtAtualizacao = "" } };
            gv_Historico.DataBind();

            lblLembrete.Text = HttpUtility.HtmlDecode("<small>Lembrete: É necessário validar os valores de Itens Importados por Grupos ou Famílias, validar os Itens Importados a partir do Fornecedor e validar os Itens Importados de outras Tabelas!</small>");
        }

        #endregion

        #region | Combos

        protected void PopulaCombos()
        {
            Popula_Combo(ddlTipoTabela, "sp_Select 'tbl_Flow_Comercial_TabelaPreco_Tipo', @idFiltro=-1", "idTipoTabela", "sDscTipoTabela", false, "Selecione a Tabela", "0");

            lstGrupos.Items.Clear();
            Popula_Combo(lstGrupos, "sp_Select 'Flow_WMS_Produtos_Grupos_PAI', @idPesquisa=0", "idGrupo", "sDscGrupo", false);

            lstFamilias.Items.Clear();
            Popula_Combo(lstFamilias, "sp_Select 'Flow_WMS_Produtos_Familia'", "idFamilia", "sDscFamilia", false);

            Popula_Combo(ddlidMoedaOrigem, "sp_Select 'tbl_Flow_Tipo_Moeda_CambioCadastrado'", "idMoeda", "sDscTipoMoeda", false, "Selecione uma Moeda", "0");
            Popula_Combo(ddlidMoedaDestino, "sp_Select 'tbl_Flow_Tipo_Moeda_CambioCadastrado'", "idMoeda", "sDscTipoMoeda", false, "Selecione uma Moeda", "0");

            Popula_Combo(ddlFiltroUnidade, "sp_Select 'Flow_Produtos_Unidade', @sPesquisa='" + (ddlTipoTabela.SelectedValue == "8" ? "S" : "N") + "'", "sUnidade", "sDscUnidade", true, "Todas as Unidades", "0");

            Popula_Combo(ddlFiltroTipo, "sp_Select 'Flow_Produtos_Tipo', @sPesquisa='N'", "idTipoProduto", "sDscTipoProduto", false, "Todos os Tipos", "0");
            ddlFiltroTipo.Items.Insert(1, new ListItem("Não Classificado", "-1"));

            Popula_Combo(ddlFiltroGrupo, "sp_Select 'Flow_WMS_Produtos_Grupos_PAI'", "idGrupo", "sDscGrupo", true, "Todos os Grupos", "-1");
            Popula_Combo(ddlFiltroFamília, "sp_Select 'Flow_WMS_Produtos_Familia'", "idFamilia", "sDscFamilia", true, "Todas as Famílias", "-1");
            ddlFiltroGrupo.Items.Insert(1, new ListItem("0 - Não Classificado", "0"));
            ddlFiltroFamília.Items.Insert(1, new ListItem("0 - Não Classificado", "0"));

            ddlControle_Filtro_Unidade.Items.AddRange(ddlFiltroUnidade.Items.Cast<ListItem>().ToArray());
            ddlControle_Filtro_Tipo.Items.AddRange(ddlFiltroTipo.Items.Cast<ListItem>().ToArray());
            ddlControle_Filtro_Grupo.Items.AddRange(ddlFiltroGrupo.Items.Cast<ListItem>().ToArray());
            ddlControle_Filtro_Familia.Items.AddRange(ddlFiltroFamília.Items.Cast<ListItem>().ToArray());
        }

        protected void PopulaCombo_ImportarTabelas()
        {
            lstTabelas.Items.Clear();
            lstTabelas_Vinculadas.Items.Clear();
            lstDesvincularTabelas.Items.Clear();

            if (ddlTipoTabela.SelectedValue != "8" && ddlTipoTabela.SelectedValue != "11")
            {
                if (!string.IsNullOrEmpty(hddidTabela.Value) && hddidTabela.Value != "0")
                {
                    Popula_Combo(lstTabelas, string.Format("sp_Select 'Flow_Comercial_TabelaPreco', @idPesquisa={0}, @idFiltro=0", hddidTabela.Value), "idTabela", "sDscTabela", false);
                    Popula_Combo(lstTabelas_Vinculadas, string.Format("sp_Select 'Flow_Comercial_TabelaPreco', @idPesquisa={0}, @idFiltro=1", hddidTabela.Value), "idTabela", "sDscTabela", false);
                    Popula_Combo(lstDesvincularTabelas, string.Format("sp_Select 'Flow_Comercial_TabelaPreco', @idPesquisa={0}, @idFiltro=2", hddidTabela.Value), "idTabela", "sDscTabela", true);
                }
                else if (ddlTipoTabela.SelectedValue == "1" && ddlidMoedaOrigem.SelectedValue != "0")
                    Popula_Combo(lstTabelas, string.Format("sp_Select 'Flow_Comercial_TabelaPreco', @idPesquisa=0, @idFiltro={0}, @idPais={1}", ddlTipoTabela.SelectedValue, ddlidMoedaOrigem.SelectedValue), "idTabela", "sDscTabela", false);
            }
        }

        protected void PopulaCombo_Vincula_Fornecedores_Parceiros()
        {
            string sidTabela = hddidTabela.Value,
                    idTipoTabela = ddlTipoTabela.SelectedValue;

            if (idTipoTabela.Equals("5"))
            {
                DataSet ds = ExecutarDataSet("sp_Select", new Dictionary<string, string> { { "@sTabela", "Flow_Parceiros_Fornecedores" } });
                List<ListItem> fornecedores = new List<ListItem>();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string[] tabela = row.Field<string>("idTabelaPreco").Split('|');
                    int.TryParse(tabela[0], out int idTabela);

                    if (idTabela == 0 || (tabela[0].Equals(sidTabela) && tabela[1].Equals(idTipoTabela)))
                        fornecedores.Add(new ListItem(row.Field<string>("sCPNJ_RazaoSocial"), row.Field<int>("idCliente").ToString()));
                }

                lstFornecedores.Items.AddRange(fornecedores.ToArray());
            }
            else if (idTipoTabela.Equals("11"))
            {
                DataTable tb = ExecutarDataTable("sp_Select", new Dictionary<string, string> { { "@sTabela", "Flow_Parceiros_Fornecedores" }, { "@idFiltro", "100" } });

                foreach (DataRow row in tb.Rows)
                {
                    string tabela = row.Field<string>("idTabela").Trim();
                    if (string.IsNullOrEmpty(tabela) || tabela == "0" || sidTabela.Equals(tabela))
                        lstParceiros.Items.Add(new ListItem(row.Field<string>("sCNPJ_RazaoSocial"), row.Field<int>("idParceiro").ToString()));
                }
            }
        }

        #endregion

        #region | Aba Vínculos

        protected void Popular_RepeaterVinculos(string idTabela)
        {
            //aba_Vinculos.Visible = true;
            hddsTabelasVinculadas.Value = string.Empty;
            hddsItens_TabelasVinculadas.Value = string.Empty;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_VINCULOS_PRINCIPAL" },
                { "@idTabela", idTabela },
                { "@idTabelaOrigem", hddidTabela.Value },
                { "@sFiltrosTipos_Vinculos", hddFiltro_Vinculos.Value }
            };
            DataSet dsVinculosPricipal = ExecutarDataSet(sProcedure, vParametros);

            if (ValidarDataSet(dsVinculosPricipal, out string sErro))
            {
                rptTabelaPrincipal.DataSource = dsVinculosPricipal.Tables[0];
                rptTabelaPrincipal.DataBind();

                rptTabelasPai.DataSource = dsVinculosPricipal.Tables[1];
                rptTabelasPai.DataBind();

                try
                {
                    if (ddlTipoTabela.SelectedValue == "3")
                    {
                        var tabelasVinculadas = new HashSet<string>();
                        var itensVinculados = new HashSet<string>();

                        foreach (DataRow row in dsVinculosPricipal.Tables[2].Rows)
                        {
                            string[] partes = row.Field<string>("sValidarItens_Vinculados").Split(';');
                            tabelasVinculadas.Add(partes[0]);

                            if (partes.Length > 1)
                            {
                                string item = partes[1].Contains("|") ? partes[1] : $"{partes[1]}|";
                                itensVinculados.Add(item);
                            }
                        }

                        hddsTabelasVinculadas.Value = string.Join("|", tabelasVinculadas);
                        hddsItens_TabelasVinculadas.Value = string.Join("|", itensVinculados);
                    }
                }
                catch { }

                rptTabelasFilhas.DataSource = dsVinculosPricipal.Tables[2];
                rptTabelasFilhas.DataBind();
            }

            if (!string.IsNullOrEmpty(hddsTabelasVinculadas.Value) && !string.IsNullOrEmpty(hddsItens_TabelasVinculadas.Value))
            {
                hddsTabelasVinculadas.Value = string.Join("|", new HashSet<string>(hddsTabelasVinculadas.Value.Split('|').Select(x => x.Trim())));
                hddsItens_TabelasVinculadas.Value = string.Join("|", new HashSet<string>(hddsItens_TabelasVinculadas.Value.Split('|').Select(x => x.Trim())));

                lstValidarItens_TabelasVinculadas.Items.Clear();

                string[] idsItens = hddsItens_TabelasVinculadas.Value.Split('|');
                if (idsItens.Length > 0)
                {
                    string ids = string.Join(",", idsItens.Select(id => $"'{id}'"));
                    string consulta = $"SELECT idItem, sDscProduto FROM Produtos WHERE idItem IN ({ids})";

                    foreach (string sidItem in hddsItens_TabelasVinculadas.Value.Split('|'))
                    {
                        Dictionary<string, string> vParam = new Dictionary<string, string>
                        {
                            { "@sFuncao", "CONSULTAR_ITEM" },
                            { "@idProduto", sidItem }
                        };
                        DataTable dt = ExecutarDataTable(sProcedure, vParam);

                        if (dt.Rows.Count > 0)
                            lstValidarItens_TabelasVinculadas.Items.Add(new ListItem(dt.Rows[0].Field<string>("sDscProduto"), dt.Rows[0].Field<int>("idItem").ToString()));
                    }
                }
            }

            ulPais.Visible = rptTabelasPai.Items.Count > 0;
            ulFilhos.Visible = rptTabelasFilhas.Items.Count > 0;

            if (rptTabelasFilhas.Items.Count >= 5)
                ulFilhos.Attributes.Add("style", "justify-content: start");

            if (rptTabelasPai.Items.Count >= 5)
                ulPais.Attributes.Add("style", "justify-content: start");

            if (rptTabelasPai.Items.Count < 1)
            {
                ulPais.Visible = false;
                ulPrincipal.Attributes.Add("class", "setaInvisivel");
            }
        }

        protected void rptTabelasFilhas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            string idTabela = (e.Item.DataItem as DataRowView)["idTabela"].ToString();

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_VINCULOS_FILHOS" },
                { "@idTabela", idTabela },
                { "@idTabelaOrigem", hddidTabela.Value },
                { "@sFiltrosTipos_Vinculos", hddFiltro_Vinculos.Value }
            };
            DataSet dsVinculosFilhas = ExecutarDataSet(sProcedure, vParametros);

            Repeater rptFilhasFilhas = e.Item.FindControl("rptTabelasFilhasFilhas") as Repeater;

            if (ValidarDataSet(dsVinculosFilhas))
            {
                try
                {
                    foreach (DataRow row in dsVinculosFilhas.Tables[0].Rows)
                    {
                        string sValidarItens_Vinculados = row.Field<string>("sValidarItens_Vinculados");

                        hddsTabelasVinculadas.Value += sValidarItens_Vinculados.Split(';')[0];

                        if (sValidarItens_Vinculados.Split(';').Count() > 1)
                            hddsItens_TabelasVinculadas.Value += sValidarItens_Vinculados.Split(';')[1].Contains("|") ? sValidarItens_Vinculados.Split(';')[1] : string.Format("{0}|", sValidarItens_Vinculados.Split(';')[1]);
                    }
                }
                catch { }

                rptFilhasFilhas.DataSource = dsVinculosFilhas.Tables[0];
                rptFilhasFilhas.DataBind();
            }

            if (rptFilhasFilhas.Items.Count >= 5)
                ulFilhos.Attributes.Add("style", "justify-content: start");

            e.Item.FindControl("ulFilhosFilhos").Visible = rptFilhasFilhas.Items.Count > 0;
        }

        protected void rptTabelasFilhasFilhas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            string idTabela = (e.Item.DataItem as DataRowView)["idTabela"].ToString();

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_VINCULOS_FILHOS" },
                { "@idTabela", idTabela },
                { "@idTabelaOrigem", hddidTabela.Value },
                { "@sFiltrosTipos_Vinculos", hddFiltro_Vinculos.Value }
            };
            DataSet dsVinculosFilhasFilhas = ExecutarDataSet(sProcedure, vParametros);

            Repeater rptFilhasNetas = e.Item.FindControl("rptTabelasFilhasNetas") as Repeater;

            if (ValidarDataSet(dsVinculosFilhasFilhas))
            {
                try
                {
                    foreach (DataRow row in dsVinculosFilhasFilhas.Tables[0].Rows)
                    {
                        string sValidarItens_Vinculados = row.Field<string>("sValidarItens_Vinculados");

                        hddsTabelasVinculadas.Value += sValidarItens_Vinculados.Split(';')[0];

                        if (sValidarItens_Vinculados.Split(';').Count() > 1)
                            hddsItens_TabelasVinculadas.Value += sValidarItens_Vinculados.Split(';')[1].Contains("|") ? sValidarItens_Vinculados.Split(';')[1] : string.Format("{0}|", sValidarItens_Vinculados.Split(';')[1]);
                    }
                }
                catch { }

                rptFilhasNetas.DataSource = dsVinculosFilhasFilhas.Tables[0];
                rptFilhasNetas.DataBind();
            }

            if (rptFilhasNetas.Items.Count >= 5)
                ulFilhos.Attributes.Add("style", "justify-content: start");

            e.Item.FindControl("ulFilhosNetos").Visible = rptFilhasNetas.Items.Count > 0;
        }

        #endregion

        #region | Utils

        /// <summary>
        /// Método para Limpar os Campos da página.
        /// </summary>
        void LimparCampos()
        {
            PainelAtualizacao.Visible = false;
            txtComposicao_sDscProduto.Text = "";
            txtComposicao_sCodigoProduto.Text = "";
            txtLocal.Text = "";
            txtMargem.Text = "";
            txtEnvio.Text = "";
            txtCambio.Text = "";
            txtFator.Text = "";
        }

        /// <summary>
        /// Método para Bloquear ou desbloquear a Edição do Tipo de Tabela e Moedas.
        /// </summary>
        /// <param name="bEdita">Define se os campos de Tipo de Tabela e Moedas podem ser alterados.</param>
        protected void BloquearEdicao_TipoTabela(bool bEdita)
        {
            ddlTipoTabela.Attributes.Remove("disabled");
            ddlidMoedaOrigem.Attributes.Remove("disabled");
            ddlidMoedaDestino.Attributes.Remove("disabled");

            if (!bEdita)
            {
                ddlTipoTabela.Attributes.Add("disabled", "disabled");
                ddlidMoedaOrigem.Attributes.Add("disabled", "disabled");
                ddlidMoedaDestino.Attributes.Add("disabled", "disabled");
            }
        }

        /// <summary>
        /// Método para Bloquear ou Desbloquear a Edição da Tabela de Preços.
        /// </summary>
        protected void BloquearEdicao_Cadeado()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "ALTERAR_EDICAO" },
                { "@idTabela", hddidTabela.Value },
                { "@sCadeado", hddsCadeado.Value }
            };
            ExecutarDataSet(sProcedure, vParametros);

            Pesquisar(Request["id"]);

            MensagemPagina.MostraMensagem_Aviso(hddsCadeado.Value.Equals("S") ? "Esta Tabela de Preços foi definida para Permitir Edição." : "Esta Tabela de Preços foi definida para <b>Não</b> Permitir Edição.", true);
        }

        /// <summary>
        /// Função para obter a classe CSS para cada Tipo de Tabela de Preços.
        /// </summary>
        /// <param name="idTipo">Recebe o Tipo de Tabela.</param>
        /// <returns>Retorna uma string com a classe CSS.</returns>
        protected static string RetornaClasse_x_Tipo(int idTipo)
        {
            string classe = string.Empty;

            if (idTipo > 0)
            {
                switch (idTipo)
                {
                    case 1:
                        classe = "success";
                        break;
                    case 2:
                        classe = "info";
                        break;
                    case 3:
                        classe = "warning";
                        break;
                    case 4:
                        classe = "tipo-primary";
                        break;
                    case 5:
                        classe = "tipo-primary";
                        break;
                    case 10:
                        classe = "danger";
                        break;
                    case 11:
                        classe = "success";
                        break;
                }
            }

            return classe;
        }

        protected static string GerarExcel(string idTabela, string idExportacao, bool bProdutosCliente, bool bEdita)
        {
            DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_TabelaPreco", new Dictionary<string, string> { { "@sFuncao", "EXPORTAR_ITENS_EXCEL" }, { "@idTabela", idTabela }, { "@idExportacao", idExportacao }, { "@sProdutosClientes", bProdutosCliente ? "S" : "N" }, { "@sFoto", !bEdita ? "S" : "N" } });
            string tabela = DATASET(ds, "sDscTabela");

            if (DATASET(ds, "idTipo") == "2")
                return "Excel_LPU/" + DATASET(ds, "sTitulo");
            else
            {
                int.TryParse(DATASET(ds, "idTipoTabela"), out int idTipoTabela);
                int.TryParse(DATASET(ds, "idMoedaOrigem"), out int idMoedaOrigem);
                bool bMoeda_2 = idMoedaOrigem == 2
                , bNCM = DATASET(ds, "sCalculaImpostos").Equals("S")
                , bLPU = idTipoTabela.Equals(11);

                int nLinha_titulo = 1
                , nLinha_vazia_1 = 2
                , nLinha_legenda = 3
                , nLinha_vazia_2 = 4
                , nLinha_cabecalho = 5
                , nLinha_itens = 6;

                ExcelPackage.License.SetNonCommercialOrganization("TT_Flow");

                using (var excel = new ExcelPackage())
                {
                    var aba = excel.Workbook.Worksheets.Add("Itens da Tabela");

                    var colunas = RetornaColunas_Valores(idTipoTabela, bMoeda_2, bNCM);
                    int nCells = bEdita && bLPU ? colunas.Count - 1 : colunas.Count;

                    aba.Cells[nLinha_titulo, 1].Value = tabela;
                    aba.Cells[nLinha_vazia_1, 1].Value = "";
                    aba.Cells[nLinha_legenda, 1].Value = bEdita ? "Apenas as colunas destacadas serão consideradas para atualizar as informações dos Itens, quando este arquivo for Importado!" : "";
                    aba.Cells[nLinha_vazia_2, 1].Value = "";

                    var rangeTitulo = aba.Cells[nLinha_titulo, 1, nLinha_titulo, nCells];
                    AplicaEstilos(rangeTitulo, Color.LightGreen, null, true, true, 18, false, ExcelBorderStyle.Medium, ExcelHorizontalAlignment.Left);

                    aba.Cells[nLinha_vazia_1, 1, nLinha_vazia_1, nCells].Merge = true;

                    var rangeLegenda = aba.Cells[nLinha_legenda, 1, nLinha_legenda, nCells];
                    AplicaEstilos(rangeLegenda, bEdita ? Color.LightYellow : Color.White, null, true, true, 12, true, null, ExcelHorizontalAlignment.Left);

                    aba.Cells[nLinha_vazia_2, 1, nLinha_vazia_2, nCells].Merge = true;

                    foreach (var coluna in colunas)
                    {
                        if (bLPU && bEdita && colunas.Last() == coluna) continue;

                        var linha_cabecalho = aba.Cells[nLinha_cabecalho, colunas.IndexOf(coluna) + 1];
                        AplicaEstilos(linha_cabecalho, Color.LightGray, coluna.Item2, false, true, 12, false, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left);
                    }

                    int nLinha = nLinha_itens;
                    string formula = Retorna_Formula(idTipoTabela, bMoeda_2, bNCM).Replace("nCambio", DATASET(ds, "nCambio").Replace(".", "").Replace(",", "."));
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        foreach (var coluna in colunas)
                        {
                            int index = colunas.IndexOf(coluna) + 1;
                            var linha_item = aba.Cells[nLinha, index];

                            if (coluna.Item1 != nCol_Foto)
                            {
                                if (coluna.Item1 > 8) linha_item.Value = decimal.Parse(row[coluna.Item1].ToString());
                                else linha_item.Value = row[coluna.Item1].ToString();

                                if (coluna.Item3) AplicaEstilos(linha_item, bEdita ? Color.LightYellow : Color.White, null, false, true, 12, false, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center, "#,##0.00", !bEdita);
                                else if (coluna.Item1 > 8) AplicaEstilos(linha_item, Color.White, null, false, false, 12, false, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Center);
                                else AplicaEstilos(linha_item, Color.White, null, false, false, 12, false, ExcelBorderStyle.Thin, ExcelHorizontalAlignment.Left);

                                if (coluna.Item1.Equals(nCol_Total)) linha_item.Formula = formula.Replace("nLinha", nLinha.ToString());
                            }
                            else if (!bEdita)
                            {
                                object foto = row[nCol_Foto];
                                if (foto != null && !string.IsNullOrEmpty(foto.ToString()))
                                {
                                    using (var stream = new MemoryStream(foto as byte[]))
                                    {
                                        var pic = aba.Drawings.AddPicture($"img_{nLinha}", stream);
                                        pic.SetPosition(nLinha - 1, 0, index - 1, 0);
                                        pic.SetSize(60, 60);
                                        aba.Row(nLinha).CustomHeight = true;
                                        aba.Row(nLinha).Height = 50;
                                    }
                                }
                            }
                        }

                        nLinha++;
                    }

                    aba.Calculate();

                    aba.Cells[nLinha_cabecalho, 1, nLinha, bLPU ? colunas.Count - 1 : colunas.Count].AutoFilter = true;

                    aba.Row(nLinha_titulo).CustomHeight = true;
                    aba.Row(nLinha_legenda).CustomHeight = true;
                    aba.Row(nLinha_cabecalho).CustomHeight = true;

                    for (int i = 1; i <= colunas.Count; i++) { aba.Column(i).AutoFit(); }

                    aba.Protection.IsProtected = true;
                    aba.Protection.AllowSort = true;
                    aba.Protection.AllowAutoFilter = true;
                    aba.Protection.SetPassword("TT_Tabelas_protectedSheet");

                    string arquivo = $"Itens_TabelaPreco_{idTabela}-{CarimboDataHora()}.xlsx";

                    if (!bEdita)
                    {
                        ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_TabelaPreco", new Dictionary<string, string> { { "@sFuncao", "SALVAR_EXPORTACAO" }, { "@idTabela", idTabela }, { "@idTipo", "2" }, { "@sTitulo", arquivo }, { "@idUsuarioAtualizacao", Variaveis.idUsuario() } });

                        arquivo = "Excel_LPU/" + arquivo;
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Download/") + "Excel_LPU/");
                    }

                    excel.SaveAs(new FileInfo(Path.Combine(HttpContext.Current.Server.MapPath("~/Download/"), arquivo)));

                    return arquivo;
                }
            }
        }

        protected static List<(int, string, bool)> RetornaColunas_Valores(int idTipoTabela, bool bMoeda_2, bool bNCM)
        {
            (int, string, bool)
            val_ID = (0, "ID", false)
            , val_Codigo = (1, "Código", false)
            , val_Descricao = (2, "Descrição", false)
            , val_Tipo = (3, "Tipo", false)
            , val_Grupo = (4, "Grupo", false)
            , val_Familia = (5, "Família", false)
            , val_Unidade = (6, "Unidade", false)
            , val_Atualizacao = (7, "Última Atualização", false)
            , val_UsuarioAtualizacao = (8, "Atualizado Por", false)
            , val_TaxaEnvio = (9, "Taxa Envio", true)
            , val_TaxaLocal = (10, "Taxa Local", true)
            , val_IPI = (11, "IPI", false)
            , val_TaxaImpostos = (12, "Taxa Impostos", false)
            , val_Margem = (13, "Margem", true)
            , val_Fator = (14, "Fator", true)
            , val_Desconto = (15, "Desconto", true)
            , val_Preco = (16, "Preço", true)
            , val_Total = (nCol_Total, "Total", false)
            , val_Preco_SD = (18, "Preço - Sudeste", true)
            , val_Preco_ND = (19, "Preço - Nordeste", true)
            , val_Preco_N = (20, "Preço - Norte", true)
            , val_Preco_CO = (21, "Preço - Centro-Oeste", true)
            , val_Preco_S = (22, "Preço - Sul", true)
            , val_Foto = (nCol_Foto, "Foto", false);

            List<(int, string, bool)> colunas = new List<(int, string, bool)>
            {
                val_ID,
                val_Codigo,
                val_Descricao,
                val_Tipo,
                val_Grupo,
                val_Familia,
                val_Unidade,
                val_Atualizacao,
                val_UsuarioAtualizacao
            };

            if (idTipoTabela != 8) colunas.Add(val_Preco);

            switch (idTipoTabela)
            {
                case 1: // Vendas Customizadas
                case 11: // Vendas LPU
                    colunas.Add(val_Desconto);
                    break;

                case 2: // Custo TT
                    colunas.Add(val_Margem);

                    if (bNCM)
                    {
                        if (bMoeda_2) colunas.Add(val_IPI);
                        else colunas.Add(val_TaxaImpostos);
                    }

                    if (!bMoeda_2)
                    {
                        colunas.Add(val_TaxaEnvio);
                        colunas.Add(val_TaxaLocal);
                    }
                    break;

                case 3: // Vendas PvP
                case 10: // Custo Empreitada
                    colunas.Add(val_Fator);
                    break;

                case 4: // Industrialização TT
                    colunas.Add(val_Fator);

                    if (bNCM)
                    {
                        if (bMoeda_2) colunas.Add(val_IPI);
                        else colunas.Add(val_TaxaImpostos);
                    }

                    if (!bMoeda_2)
                    {
                        colunas.Add(val_TaxaEnvio);
                        colunas.Add(val_TaxaLocal);
                    }
                    break;

                case 5: // Custo Fornecedor
                    if (bNCM && bMoeda_2) colunas.Add(val_IPI);

                    if (!bMoeda_2)
                    {
                        colunas.Add(val_TaxaEnvio);
                        colunas.Add(val_TaxaLocal);
                    }
                    break;

                case 8: // Custo de Recursos
                    colunas.Add(val_Fator);
                    colunas.Add(val_Preco_SD);
                    colunas.Add(val_Preco_ND);
                    colunas.Add(val_Preco_N);
                    colunas.Add(val_Preco_CO);
                    colunas.Add(val_Preco_S);
                    break;
            }

            if (idTipoTabela != 8) colunas.Add(val_Total);
            if (idTipoTabela == 11) colunas.Add(val_Foto);

            return colunas;
        }

        protected static string Retorna_Formula(int idTipoTabela, bool bMoeda_2, bool bNCM)
        {
            string valor_1 = "J"
            , valor_2 = "K"
            , valor_3 = "L"
            , valor_4 = "M"
            , valor_5 = "N";
            string preco_Cambio = $"{valor_1}nLinha*nCambio";
            string formula = $"{preco_Cambio}";

            switch (idTipoTabela)
            {
                case 1: // Vendas Customizadas
                case 11: // Vendas LPU
                    formula += $"-({preco_Cambio}*({valor_2}nLinha/100))";
                    break;

                case 2: // Custo TT
                    if (bMoeda_2)
                    {
                        formula += $"*({valor_2}nLinha/100+1)";
                        if (bNCM) formula += $"*({valor_3}nLinha/100+1)";
                    }
                    else
                    {
                        if (bNCM) formula += $"*(({valor_2}nLinha+{valor_5}nLinha)/100+1)+({preco_Cambio}*({valor_4}nLinha/100+1)*({valor_3}nLinha/100))";
                        else formula += $"*(({valor_2}nLinha+{valor_3}nLinha+{valor_4}nLinha)/100+1)";
                    }
                    break;

                case 3: // Vendas PvP
                case 10: // Custo Empreitada
                    formula += $"*{valor_2}nLinha";
                    break;

                case 4: // Industrialização TT
                    formula += $"*{valor_2}nLinha";

                    if (bNCM && bMoeda_2) formula += $"*({valor_3}nLinha/100+1)";
                    else if (!bMoeda_2)
                    {
                        if (bNCM) formula += $"*({valor_5}nLinha/100+1)+({preco_Cambio}*({valor_4}nLinha/100+1)*({valor_3}nLinha/100))";
                        else formula += $"*(({valor_3}nLinha+InLinha)/100+1)";
                    }
                    break;

                case 5: // Custo Fornecedor
                    if (bNCM && bMoeda_2) formula += $"*({valor_2}nLinha/100+1)";
                    else if (!bMoeda_2) formula += $"*(({valor_2}nLinha+{valor_3}nLinha)/100+1)";
                    break;
            }

            return formula;
        }

        protected static void AplicaEstilos(ExcelRange linha, Color cor, string valor, bool bMescla, bool bNegrito, int nTamanhoFonte, bool bQuebraLinha, ExcelBorderStyle? borda, ExcelHorizontalAlignment alinhamento, string formato = null, bool bBloqueado = true)
        {
            if (valor != null) linha.Value = valor;
            if (formato != null) linha.Style.Numberformat.Format = formato;

            linha.Merge = bMescla;
            linha.Style.Fill.PatternType = ExcelFillStyle.Solid;
            linha.Style.Fill.BackgroundColor.SetColor(cor);
            linha.Style.Font.Bold = bNegrito;
            linha.Style.Font.Size = nTamanhoFonte;
            linha.Style.Font.Name = "Arial";
            linha.Style.WrapText = bQuebraLinha;
            linha.Style.Locked = bBloqueado;

            if (borda != null)
            {
                linha.Style.Border.Top.Style = borda.Value;
                linha.Style.Border.Right.Style = borda.Value;
                linha.Style.Border.Bottom.Style = borda.Value;
                linha.Style.Border.Left.Style = borda.Value;
            }

            linha.Style.HorizontalAlignment = alinhamento;
        }

        #endregion

        #region | Eventos

        protected void lnkAtualiza_Click(object sender, EventArgs e) => DirecionaPagina(Request.RawUrl.Substring(1));

        protected void cmdAplicarFiltros_Vinculos_Click(object sender, EventArgs e)
        {
            string filtro = string.Empty;

            foreach (ListItem tipo in lstFiltroTipos_Vinculos.Items)
            {
                if (tipo.Selected)
                    filtro += tipo.Value + "|";
            }

            hddFiltro_Vinculos.Value = filtro;

            Popular_RepeaterVinculos(hddidTabela.Value);

            Scripts.Mantem_AbaAtiva(Page, "aba_vinculos-tab");
        }

        #endregion

        #region | Script

        protected void RegistraScript_Tabelas()
        {
            List<string> list = new List<string>
            {
                "txtComposicao_sDscProduto",
                "txtComposicao_sCodigoProduto",
                "hddComposicao_idItem",
                "hddComposicao_sDscTipoProduto",
                "hddComposicao_sUnidade",
                "hddComposicao_idGrupo",
                "hddComposicao_idFamilia",
                "hddComposicao_sDscGrupo",
                "hddComposicao_sDscFamilia",
                "hddComposicao_nII",
                "hddComposicao_nIPI",
                "hddComposicao_nPIS",
                "hddComposicao_nCOFINS",
                "hddComposicao_nICMS",
                "hddComposicao_sIndustrializado"
            };
            ScriptsPagina("txtComposicao_sDscProduto", list, Page, ddlTipoTabela.SelectedValue == "8" ? "2" : "0", false, "txtPreco");
            list[0] = "txtComposicao_sCodigoProduto";
            list[1] = "txtComposicao_sDscProduto";
            ScriptsPagina("txtComposicao_sCodigoProduto", list, Page, ddlTipoTabela.SelectedValue == "8" ? "2" : "0", true, "txtPreco");
        }

        public static string ScriptsPagina(string dgPagina, List<string> elementos, Page pg, string tipo, bool bCodigo, string focus)
        {
            string script = "";
            string sMetodo = bCodigo ? "Produtos_Codigo" : "Produtos";
            string minimo = bCodigo ? "1" : "3";

            Dictionary<string, string> vParametrosAjax = new Dictionary<string, string>
            {
                { "sDscProduto", "JSON.stringify(request.term)" },
                { "idTipoProduto", "$('[id*=ddlTipoProduto]').val() || 0" },
                { "idFamilia", "$('[id*=ddlFamilia]').val() || 0" },
                { "idGrupo", "$('[id*=ddlGrupo]').val() || 0" },
                { "idPaisOrigem", "$('[id*=ddlPaisOrigem]').val() || 0" },
                { "idMoedaOrigem", "$('[id*=ddlidMoedaOrigem]').val() || 0" },
                { "idTipoTabela", "$('[id*=ddlTipoTabela]').val() || 0" }
            };

            if (tipo == "2")
            {
                vParametrosAjax.Add("tipo", "2");
                sMetodo = bCodigo ? "Recursos_Codigo" : "Recursos";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("$v192(function() {");
            sb.Append("$v192(\"[id*=" + dgPagina + "]\").autocomplete({");
            sb.Append("source: function(request, response) {");
            sb.Append("$v192.ajax({");
            sb.Append("url: '/app/Paginas/Comercial/Tabelas_Detalhe.aspx/Get" + sMetodo + "',");
            sb.Append("data: JSON.stringify({");

            foreach (KeyValuePair<string, string> item in vParametrosAjax)
            {
                sb.Append("'" + item.Key + "': " + item.Value + ", ");
            }

            if (vParametrosAjax.Count > 0)
            {
                sb.Length -= 2; // Remove a vírgula extra
            }

            sb.Append("}),"); // Adicione uma vírgula após a chave 'data'

            sb.Append("dataType: \"json\",");
            sb.Append("type: \"POST\",");
            sb.Append("contentType: \"application/json; charset=utf-8\",");
            sb.Append("success: function(data) {");
            sb.Append("response($v192.map(data.d, function(item) {");
            sb.Append("return {");

            sb.Append("label: item.split('|')[0],");

            int index = 0;
            foreach (string elementoID in elementos)
            {
                sb.Append(elementoID + ": item.split('|')[" + (index) + "],");
                index++;
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append("};");
            sb.Append("}));"); // Adicione parênteses de fechamento para a função 'map'
            sb.Append("},");
            sb.Append("error: function(response) {");
            sb.Append("console.log(response.responseText);");
            sb.Append("},");
            sb.Append("failure: function(response) {");
            sb.Append("console.log(response.responseText);");
            sb.Append("}");
            sb.Append("});");
            sb.Append("},");
            sb.Append("select: function(e, i) {");

            foreach (string elementoID in elementos)
            {
                sb.Append("$(\"[id$=" + elementoID + "]\").val(i.item." + elementoID + ");");
            }

            sb.Append($"$('[id$={focus}]').focus();");

            if (sMetodo == "Servicos")
                sb.Append("$('[id$=hddComposicao_sServico]').val('1');");

            sb.Append("},");
            sb.Append("minLength: " + minimo);
            sb.Append("});");
            sb.Append("});");

            ScriptManager.RegisterStartupScript(pg, pg.GetType(), "js_ScriptPaginaTabelas" + Guid.NewGuid(), sb.ToString(), true);

            return script;
        }

        #endregion

        #region | WebMethod

        /// <summary>
        /// WebMethod para Salvar o Status do Cadeado.
        /// </summary>
        /// <param name="idTabela">Recebe o ID da Tabela de Preços.</param>
        /// <param name="sCadeado">Recebe o valor que define se a Tabela está aberta para alterações ou não.</param>
        [WebMethod]
        public static void Post_Salvar_Cadeado(int idTabela, string sCadeado)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "ALTERAR_EDICAO" },
                { "@idTabela", idTabela.ToString() },
                { "@sCadeado", sCadeado }
            };
            ExecutarDataSet(sProcedure, vParametros);
        }

        /// <summary>
        /// WebMethod para Consultar as informações para Popular a Aba de Histórico.
        /// </summary>
        /// <param name="idTabela">Recebe o ID da Tabela de Preços.</param>
        [WebMethod]
        public static List<Dictionary<string, object>> Get_PopularAba_Historico(string idTabela)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_HISTORICO_ITENS" },
                { "@idTabela", idTabela }
            };
            DataTable dt = ExecutarDataTable(sProcedure, vParametros);

            var lista = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = string.IsNullOrEmpty(row[col].ToString()) ? string.Empty : row[col];
                }
                lista.Add(dict);
            }

            return lista;
        }

        /// <summary>
        /// WebMethod para Consultar as informações para Popular a Aba de Controle de Valores.
        /// </summary>
        /// <param name="itens">Recebe a Lista das Tabelas selecionadas para Desvincular.</param>
        /// <param name="sidTabela">Recebe o ID da Tabela de Preços.</param>
        [WebMethod]
        public static string Get_PopularAba_ControleValores(List<cls_Comercial_Tabelas> itens, string sidTabela)
        {
            string json = "";

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_VALORES__TABELAS_VINCULADAS" },
                { "@idTabela", sidTabela }
            };
            DataTable dt = ExecutarDataTable(sProcedure, vParametros);

            if (dt != null && dt.Columns.Count > 4) // 4 = Quantidade de colunas padrão, incluindo a coluna da Tabela em que o Usuário está presente
            {
                Dictionary<int, cls_Comercial_Tabelas> dictItens = itens.ToDictionary(x => x.IdItem);
                List<object> list_Itens_Controle_Valores = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    int.TryParse(row["idItem"].ToString(), out int id);

                    if (dictItens.TryGetValue(id, out cls_Comercial_Tabelas item))
                    {
                        dynamic controle = new ExpandoObject();
                        var dictControle = (IDictionary<string, object>)controle;

                        dictControle["IdItem"] = item.IdItem;
                        dictControle["SCodigo"] = item.SCodigo;
                        dictControle["SDscProduto"] = item.SDscProduto;
                        dictControle["SCodigoComDescricao"] = item.SCodigoComDescricao;
                        dictControle["TipoProduto"] = item.TipoProduto;
                        dictControle["SUnidade"] = item.SUnidade;
                        dictControle["IdGrupoProduto"] = item.IdGrupoProduto;
                        dictControle["IdFamiliaProduto"] = item.IdFamiliaProduto;
                        dictControle["sDscGrupoProduto"] = item.sDscGrupoProduto;
                        dictControle["sDscFamiliaProduto"] = item.sDscFamiliaProduto;

                        foreach (DataColumn col in dt.Columns)
                        {
                            if (col.ColumnName.Split('|').Length > 1)
                            {
                                int.TryParse(col.ColumnName.Split('|')[3].Split('-')[0].Trim(), out int idTabela);
                                decimal.TryParse(row[dt.Columns.IndexOf(col)].ToString().Split('|')[0].Trim(), out decimal nValor);
                                decimal.TryParse(row[dt.Columns.IndexOf(col)].ToString().Split('|')[1].Trim(), out decimal nFator);
                                dictControle[string.Format("nValor_{0}", idTabela)] = nValor;
                                dictControle[string.Format("nFator_{0}", idTabela)] = nFator;
                            }
                        }

                        list_Itens_Controle_Valores.Add(controle);
                    }
                }

                string colunas = string.Empty;
                string cabecalho_1 = string.Empty;
                string cabecalho_2 = string.Empty;

                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName.Split('|').Length > 1)
                    {
                        string[] coluna = col.ColumnName.Split('|');
                        string titulo = coluna[3];
                        string split_idTabela = titulo.Split('-')[0];

                        int.TryParse(split_idTabela.Trim().Replace("\"\"", "").Replace("\"", ""), out int idTabela);
                        int.TryParse(coluna[0], out int idTipo);
                        decimal.TryParse(coluna[2], out decimal nFator);
                        string sFator = string.Format(idTipo.Equals(3) ? "Fator Global: {0}" : "Dto. Global: {0} %", Math.Round(nFator, 2));
                        string sMoeda = coluna[1];
                        string classe = RetornaClasse_x_Tipo(idTipo);

                        if (idTipo.Equals(3) || idTipo.Equals(1) || idTipo.Equals(11))
                        {
                            colunas += $@",
                                        {{ data: 'IdItem', orderable: false, searchable: false,
                                            render: function(data, type, row) {{
                                                return `" + (idTipo.Equals(3) ? "x " : "") + $"<span class='totalControle'>${{row.nFator_{idTabela}.toFixed(2).replace('.', '')}}</span>" + (!idTipo.Equals(3) ? " %" : "") + $@"`;
                                            }},
                                            createdCell: function(td, cellData, rowData, row, col) {{
                                                $(td).addClass('{classe}');
                                                $(td).css({{'text-align': 'center', 'border': '1px solid black'}});
                                            }}
                                        }}";

                            cabecalho_1 += $"<th colspan='2' style='border: 1px solid black; border-bottom: 0;' class='{classe}'><div><span>{titulo}</span><div style='display: flex; justify-content: center;'><span>{sFator}</span></div></div></th>";

                            if (idTipo.Equals(3))
                                cabecalho_2 += $"<th style='border: 1px solid black; border-top: 0.5px solid lightgray; text-align: center; min-width: 75px; max-width: 130px;' class='{classe}'>Fator</th><th style='border: 1px solid black; border-top: 0.5px solid lightgray; text-align: center;' class='{classe}'>Total<a data-toggle='tooltip' title='Editar Tabela' href='/App/Paginas/Comercial/Tabelas_Detalhe.aspx?id={idTabela}' class='btn btn-md btn-default' target='_blank' style='float: right;'><i class='fa fa-pencil'></i></a></th>";
                            else
                                cabecalho_2 += $"<th style='border: 1px solid black; border-top: 0.5px solid lightgray; text-align: center; min-width: 100px; max-width: 150px;' class='{classe}'>Desconto</th><th style='border: 1px solid black; border-top: 0.5px solid lightgray; text-align: center;' class='{classe}'>Total<a data-toggle='tooltip' title='Editar Tabela' href='/App/Paginas/Comercial/Tabelas_Detalhe.aspx?id={idTabela}' class='btn btn-md btn-default' target='_blank' style='float: right;'><i class='fa fa-pencil'></i></a></th>";
                        }
                        else
                        {
                            cabecalho_1 += $"<th style='border: 1px solid black; border-bottom: 0;' class='{classe}'><div><span>{titulo}</span></div></th>";
                            cabecalho_2 += $"<th style='border: 1px solid black; border-top: 0.5px solid lightgray; text-align: center;' class='{classe}'>Total<a data-toggle='tooltip' title='Editar Tabela' href='/App/Paginas/Comercial/Tabelas_Detalhe.aspx?id={idTabela}' class='btn btn-md btn-default' target='_blank' style='float: right;'><i class='fa fa-pencil'></i></a></th>";
                        }

                        colunas += $@",
                                    {{ data: 'IdItem', orderable: false, searchable: false,
                                        render: function(data, type, row) {{
                                            return `<div><b>{sMoeda} </b><span class='totalControle'>${{row.nValor_{idTabela}.toFixed(2).replace('.', '')}}</span><div style='float: right'><a data-toggle='tooltip' title='Editar Item' href='/App/Paginas/Comercial/Tabelas_Detalhe.aspx?id={idTabela}&filtroItem=${{data}}' class='btn btn-md btn-default' target='_blank'><i class='fa fa-pencil'></i></a></div></div>`;
                                        }},
                                        createdCell: function(td, cellData, rowData, row, col) {{
                                            $(td).addClass('{classe}');
                                            $(td).css({{'text-align': 'center', 'border': '1px solid black', 'min-width': '150px'}});
                                        }}
                                    }}";
                    }
                }

                string sItens = JsonConvert.SerializeObject(list_Itens_Controle_Valores);

                json = JsonConvert.SerializeObject(new
                {
                    itens = list_Itens_Controle_Valores,
                    colunas = colunas.Substring(1),
                    cabecalho_1 = cabecalho_1,
                    cabecalho_2 = cabecalho_2
                });
            }

            return json;
        }

        /// <summary>
        /// WebMethod que Desvincula Tabelas.
        /// </summary>
        /// <param name="tabelas">Recebe a Lista das Tabelas selecionadas para Desvincular.</param>
        /// <param name="idTabela">Recebe o ID da Tabela de Preços.</param>
        [WebMethod]
        public static string Get_DesvincularTabelas(List<string> tabelas, int idTabela)
        {
            string sMsg = null;

            try
            {
                Dictionary<string, string> vParam = new Dictionary<string, string>
                {
                    { "@sFuncao", "DESVINCULAR_TABELA" },
                    { "@idTabela", idTabela.ToString() },
                    { "@idTabelaDesvincular", "" }
                };

                foreach (string id in tabelas)
                {
                    vParam["@idTabelaDesvincular"] = id;
                    ExecutarDataSet(sProcedure, vParam);
                }
            }
            catch (Exception ex)
            {
                sMsg = "<b>Erro: </b>Houve um erro na tentativa de Desvincular as Tabelas selecionadas!<br />Erro ao Desvincular: " + ex.Message;
            }

            return sMsg;
        }

        /// <summary>
        /// WebMethod que Consulta os Produtos de Tabelas para serem Importados.
        /// </summary>
        /// <param name="tabelas">Recebe a Lista das Tabelas selecionadas para Importar.</param>
        /// <param name="idMoedaOrigem">Recebe a Moeda de Origem da Tabela de Preços.</param>
        /// <returns>Retorna a Lista dos Itens a serem Importados para a Tabela.</returns>
        [WebMethod]
        public static List<object> Get_ImportarProdutos(List<string> tabelas, int idMoedaOrigem)
        {
            List<object> list = new List<object>();

            foreach (string item in tabelas)
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_ITENS_TABELA" },
                    { "@idTabela", item }
                };
                DataSet ds = ExecutarDataSet(sProcedure, vParametros);

                if (ValidarDataSet(ds))
                {
                    string msg = "";

                    try
                    {
                        msg = DATASET(ds, "sMsg");
                    }
                    catch { }

                    if (string.IsNullOrEmpty(msg))
                    {
                        int.TryParse(DATASET(ds, "idTabela"), out int idTabelaOrigem);

                        foreach (DataRow row in ds.Tables[1].Rows)
                        {
                            int.TryParse(row["idItem"].ToString(), out int idItem);

                            object produto = new
                            {
                                SFuncao = "INCLUIR_ITEM",
                                IdItem = idItem,
                                IdTabelaOrigem = idTabelaOrigem,
                                SCodigo = row["sCodigo"].ToString(),
                                SDscProduto = row["sDscProduto"].ToString(),
                                SCodigoComDescricao = row["sCodigoComDescricao"].ToString(),
                                TipoProduto = row["sDscTipoProduto"].ToString(),
                                IdGrupoProduto = int.Parse(row["idGrupo"].ToString()),
                                IdFamiliaProduto = int.Parse(row["idFamilia"].ToString()),
                                sDscGrupoProduto = row["sDscGrupo"].ToString(),
                                sDscFamiliaProduto = row["sDscFamilia"].ToString(),
                                SUnidade = row["sUnidade"].ToString(),
                                sIndustrializado = row["sIndustrializado"].ToString(),

                                NII = Math.Round(decimal.Parse(row[idMoedaOrigem == 2 ? "nII" : "nII_Internacional"].ToString()), 2),
                                NIPI = Math.Round(decimal.Parse(row[idMoedaOrigem == 2 ? "nIPI" : "nIPI_Internacional"].ToString()), 2),
                                NPIS = Math.Round(decimal.Parse(row[idMoedaOrigem == 2 ? "nPIS" : "nPIS_Internacional"].ToString()), 2),
                                NCOFINS = Math.Round(decimal.Parse(row[idMoedaOrigem == 2 ? "nCOFINS" : "nCOFINS_Internacional"].ToString()), 2),
                                NICMS = Math.Round(decimal.Parse(row[idMoedaOrigem == 2 ? "nICMS" : "nICMS_Internacional"].ToString()), 2),
                                NImpostos = Math.Round(decimal.Parse(row["nTaxaImpostos"].ToString()), 2),

                                Preco = Math.Round(decimal.Parse(row["nTotal"].ToString()), 2),
                                NFator = Math.Round(decimal.Parse(row["nFator"].ToString()), 2),
                                NMargem = Math.Round(decimal.Parse(row["nMargem"].ToString()), 2),
                                NEnvio = Math.Round(decimal.Parse(row["nTaxaEnvio"].ToString()), 2),
                                NLocal = Math.Round(decimal.Parse(row["nTaxaLocal"].ToString()), 2),

                                DtAtualizacao = "Não cadastrado",
                                dtInclusao = "Não cadastrado",
                                bImportado = true,
                                bLiberado = false
                            };

                            list.Add(produto);
                        }
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// WebMethod que Consulta os Produtos de Fornecedores/Parceiros para serem Importados.
        /// </summary>
        /// <param name="fornecedores">Recebe a Lista dos Fornecedores/Parceiros selecionadas para Importar.</param>
        /// <param name="idMoedaOrigem">Recebe a Moeda de Origem da Tabela de Preços.</param>
        /// <returns>Retorna a Lista dos Itens a serem Importados para a Tabela.</returns>
        [WebMethod]
        public static List<object> Get_ImportarProdutos_x_Fornecedores_Parceiros(string fornecedores_parceiros, int idMoedaOrigem, bool bFornecedor)
        {
            List<object> list = new List<object>();

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_PRODUTOS_FORNECEDOR" },
                { "@sidParceiroFornecedor", fornecedores_parceiros },
                { "@sParceiroFornecedor", bFornecedor ? "S" : "N" }
            };
            DataSet ds = ExecutarDataSet(sProcedure, vParametros);

            if (ValidarDataSet(ds))
            {
                string msg = "";

                try
                {
                    msg = DATASET(ds, "sMsg");
                }
                catch { }

                if (string.IsNullOrEmpty(msg))
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int.TryParse(row["idItem"].ToString(), out int idItem);

                        object produto = new
                        {
                            SFuncao = "INCLUIR_ITEM",
                            IdItem = idItem,
                            IdTabelaOrigem = 0,
                            SCodigo = row["sCodigo"].ToString(),
                            SDscProduto = row["sDscProduto"].ToString(),
                            SCodigoComDescricao = row["sCodigo"].ToString() + " - " + row["sDscProduto"].ToString(),
                            TipoProduto = row["sDscTipoProduto"].ToString(),
                            IdGrupoProduto = int.Parse(row["idGrupo"].ToString()),
                            IdFamiliaProduto = int.Parse(row["idFamilia"].ToString()),
                            sDscGrupoProduto = row["sDscGrupo"].ToString(),
                            sDscFamiliaProduto = row["sDscFamilia"].ToString(),
                            SUnidade = row["sUnidade"].ToString(),
                            sIndustrializado = row["sIndustrializado"].ToString(),

                            NII = Math.Round(decimal.Parse(row[idMoedaOrigem == 2 ? "nII" : "nII_Internacional"].ToString()), 2),
                            NIPI = Math.Round(decimal.Parse(row[idMoedaOrigem == 2 ? "nIPI" : "nIPI_Internacional"].ToString()), 2),
                            NPIS = Math.Round(decimal.Parse(row[idMoedaOrigem == 2 ? "nPIS" : "nPIS_Internacional"].ToString()), 2),
                            NCOFINS = Math.Round(decimal.Parse(row[idMoedaOrigem == 2 ? "nCOFINS" : "nCOFINS_Internacional"].ToString()), 2),
                            NICMS = Math.Round(decimal.Parse(row[idMoedaOrigem == 2 ? "nICMS" : "nICMS_Internacional"].ToString()), 2),
                            NImpostos = 0,

                            Preco = Math.Round(decimal.Parse(row["nValorUnitario"].ToString()), 2),
                            NFator = 0,
                            NMargem = 0,
                            NEnvio = 0,
                            NLocal = 0,

                            DtAtualizacao = "Não cadastrado",
                            dtInclusao = "Não cadastrado",
                            bImportado = true,
                            bLiberado = false
                        };

                        list.Add(produto);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// WebMethod que Consulta os Produtos para serem Importados.
        /// </summary>
        /// <param name="itens">Recebe a Lista dos Itens selecionados para Importar.</param>
        /// <param name="idTipoTabela">Recebe o Tipo de Tabela.</param>
        /// <returns>Retorna a Lista dos Itens a serem Importados para a Tabela.</returns>
        [WebMethod]
        public static List<object> Get_ImportarProdutos_x_Excel(List<Item_Excel> itens, int idTipoTabela)
        {
            List<object> list = new List<object>();

            foreach (Item_Excel item in itens)
            {
                Dictionary<string, string> vParam = new Dictionary<string, string>()
                {
                    { "@sFuncao", "CONSULTAR_ITEM_IMPORTAR_EXCEL_TABELA_PRECO" },
                    { "@sCodigo", item.SCodigo.Trim() },
                    { "@idTipo", idTipoTabela.ToString() }
                };
                DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos", vParam);

                if (ValidarDataSet(ds))
                {
                    string msg = "";

                    try
                    {
                        msg = DATASET(ds, "sMsg");
                    }
                    catch { }

                    if (string.IsNullOrEmpty(msg))
                    {
                        int.TryParse(DATASET(ds, "idItem"), out int idItem);

                        cls_Comercial_Tabelas produto = new cls_Comercial_Tabelas
                        {
                            SFuncao = "INCLUIR_ITEM",
                            IdItem = idItem,
                            SCodigo = item.SCodigo,
                            SDscProduto = DATASET(ds, "sDscProduto"),
                            SCodigoComDescricao = DATASET(ds, "sCodigoComDescricao"),
                            TipoProduto = DATASET(ds, "sTipoProduto"),
                            IdGrupoProduto = int.Parse(DATASET(ds, "idGrupo")),
                            IdFamiliaProduto = int.Parse(DATASET(ds, "idFamilia")),
                            sDscGrupoProduto = DATASET(ds, "sDscGrupo"),
                            sDscFamiliaProduto = DATASET(ds, "sDscFamilia"),
                            SUnidade = DATASET(ds, "sUnidade"),
                            sIndustrializado = DATASET(ds, "sIndustrializado") == "S" ? "Sim" : "Não",

                            NII = Math.Round(decimal.Parse(DATASET(ds, "nII")), 2),
                            NIPI = Math.Round(decimal.Parse(DATASET(ds, "nIPI")), 2),
                            NPIS = Math.Round(decimal.Parse(DATASET(ds, "nPIS")), 2),
                            NCOFINS = Math.Round(decimal.Parse(DATASET(ds, "nCOFINS")), 2),
                            NICMS = Math.Round(decimal.Parse(DATASET(ds, "nICMS")), 2),

                            Preco = item.Preco > 0 ? item.Preco : 0,

                            DtAtualizacao = "Não cadastrado",
                            dtInclusao = "Não cadastrado",
                            bImportado = true,
                            bLiberado = false
                        };

                        produto.NImpostos = produto.NII + produto.NIPI + produto.NPIS + produto.NCOFINS + produto.NICMS;

                        list.Add(produto);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// WebMethod que Consulta os Produtos para serem Importados.
        /// </summary>
        /// <param name="itens">Recebe a Lista dos Itens selecionados para Importar.</param>
        /// <param name="idTipo">Recebe o Tipo de Item para Importação, entre '1 - Grupos' e '2 - Famílias'.</param>
        /// <param name="idTipoTabela">Recebe o Tipo de Tabela.</param>
        /// <param name="idMoedaOrigem">Recebe a Moeda Origem da Tabela.</param>
        /// <returns>Retorna a Lista dos Itens a serem Importados para a Tabela.</returns>
        [WebMethod]
        public static List<object> Get_ImportarProdutos_x_Grupos_e_Familias(List<string> itens, int idTipo, int idTipoTabela, int idMoedaOrigem)
        {
            List<object> list = new List<object>();

            foreach (string id in itens)
            {
                if (id != "0")
                {
                    Dictionary<string, string> vParametrosImportar = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_PRODUTOS_POR_GRUPO__FAMILIA" }
                    };

                    if (idTipo.Equals(1)) // Grupos
                        vParametrosImportar.Add("@idGrupo", id);
                    else if (idTipo.Equals(2)) // Famílias
                        vParametrosImportar.Add("@idFamilia", id);
                    else
                        break;

                    DataSet dsImportar = ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos", vParametrosImportar);

                    if (ValidarDataSet(dsImportar))
                    {
                        foreach (DataRow row in dsImportar.Tables[0].Rows)
                        {
                            string sIndustrializado = row["sIndustrializado"].ToString();

                            if ((idTipoTabela.Equals(4) && sIndustrializado.Equals("S")) || (!idTipoTabela.Equals(4) && !sIndustrializado.Equals("S")))
                            {
                                cls_Comercial_Tabelas item = new cls_Comercial_Tabelas()
                                {
                                    SFuncao = "INCLUIR_ITEM",
                                    IdItem = int.Parse(row["idItem"].ToString()),
                                    SCodigo = row["sCodigo"].ToString(),
                                    SDscProduto = row["sDscProduto"].ToString(),
                                    SCodigoComDescricao = string.Format("{0} - {1}", row["sCodigo"].ToString(), row["sDscProduto"].ToString()),
                                    TipoProduto = row["sDscTipoProduto"].ToString(),
                                    SUnidade = row["sUnidade"].ToString(),
                                    IdGrupoProduto = int.Parse(row["idGrupo"].ToString()),
                                    sDscGrupoProduto = row["sDscGrupo"].ToString(),
                                    IdFamiliaProduto = int.Parse(row["idFamilia"].ToString()),
                                    sDscFamiliaProduto = row["sDscFamilia"].ToString(),
                                    sIndustrializado = sIndustrializado,

                                    NII = Math.Round(decimal.Parse(row[idMoedaOrigem == 2 ? "nII" : "nII_Internacional"].ToString()), 2),
                                    NIPI = Math.Round(decimal.Parse(row[idMoedaOrigem == 2 ? "nIPI" : "nIPI_Internacional"].ToString()), 2),
                                    NPIS = Math.Round(decimal.Parse(row[idMoedaOrigem == 2 ? "nPIS" : "nPIS_Internacional"].ToString()), 2),
                                    NCOFINS = Math.Round(decimal.Parse(row[idMoedaOrigem == 2 ? "nCOFINS" : "nCOFINS_Internacional"].ToString()), 2),
                                    NICMS = Math.Round(decimal.Parse(row[idMoedaOrigem == 2 ? "nICMS" : "nICMS_Internacional"].ToString()), 2),

                                    Preco = 0,

                                    DtAtualizacao = "Não cadastrado",
                                    dtInclusao = "Não cadastrado",
                                    bImportado = true,
                                    bLiberado = false
                                };

                                item.NImpostos = idMoedaOrigem == 2 ? item.NIPI : item.NII + item.NIPI + item.NPIS + item.NCOFINS + item.NICMS;

                                if (idTipoTabela.Equals(8))
                                {
                                    item.Preco_Zona_SD = item.Preco;
                                    item.Preco = 0;
                                }

                                list.Add(item);
                            }
                        }
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// WebMethod para Salvar os Controles de Fator da Tabela de Preços.
        /// </summary>
        /// <param name="list_ControlesFator">Recebe a Lista dos Itens dos Controles de Fator.</param>
        /// <param name="idTabela">Recebe o ID da Tabela.</param>
        /// <returns>Retorna o erro gerado, caso exista.</returns>
        [WebMethod]
        public static bool Post_SalvarControles(List<cls_Comercial__Controles_Fator> list_ControlesFator, int idTabela)
        {
            try
            {
                foreach (var item in list_ControlesFator)
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>()
                    {
                        { "@sFuncao", "INCLUIR_CONTROLE_FATOR" },
                        { "@idTabela", idTabela.ToString() },
                        { "@idObjeto", item.idObjeto.ToString() },
                        { "@idTipoObjeto", item.idTipoObjeto.ToString() },
                        { "@nFator", item.nValor.ToString().StringToDecimalString() },
                        { "@sBloquearEdicao", item.sBloquearEdicao }
                    };
                    ExecutarDataSet(sProcedure, vParametros);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// WebMethod para Salvar os Parceiros de Tabela de Preços do tipo LPU.
        /// </summary>
        /// <param name="list">Recebe a Lista dos Parceiros da Tabela.</param>
        /// <param name="idTabela">Recebe o ID da Tabela.</param>
        /// <returns>Retorna o erro gerado, caso exista.</returns>
        [WebMethod]
        public static bool Post_SalvarParceiros(string[] list, int idTabela)
        {
            try
            {
                foreach (string idParceiro in list)
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "VINCULAR_PARCEIROS_LPU" },
                        { "@idTabela", idTabela.ToString() },
                        { "@idParceiro", idParceiro }
                    };
                    ExecutarDataSet(sProcedure_Clientes, vParametros);
                }

                return true;
            }
            catch { }

            return false;
        }

        /// <summary>
        /// WebMethod para Salvar os Itens da Tabela de Preços.
        /// </summary>
        /// <param name="list_Itens">Recebe a Lista dos Itens da Tabela.</param>
        /// <param name="idTabela">Recebe o ID da Tabela.</param>
        /// <param name="idTipoTabela">Recebe o ID do Tipo da Tabela.</param>
        /// <returns>Retorna o erro gerado, caso exista.</returns>
        [WebMethod]
        public static bool Post_SalvarItens(List<cls_Comercial_Tabelas> list_Itens, int idTabela, int idTipoTabela)
        {
            try
            {
                foreach (cls_Comercial_Tabelas item in list_Itens)
                {
                    if (item.SFuncao == "CONSULTA_ITEM")
                        continue;

                    if (idTipoTabela != 1 && item.bImportado && !item.bLiberado)
                        continue;

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", item.SFuncao },
                        { "@idTabela", idTabela.ToString() },
                        { "@idItem", item.IdItem.ToString() },
                        { "@nPreco", item.Preco.ToString().StringToDecimalString() },
                        { "@nFator", item.NFator.ToString().StringToDecimalString() },
                        { "@nTaxaEnvio", item.NEnvio.ToString().StringToDecimalString() },
                        { "@nTaxaLocal", item.NLocal.ToString().StringToDecimalString() },
                        { "@nMargem", item.NMargem.ToString().StringToDecimalString() },
                        { "@nTaxaImpostos", item.NImpostos.ToString().StringToDecimalString() },
                        { "@nPreco_Zona_SD", item.Preco_Zona_SD.ToString().StringToDecimalString() },
                        { "@nPreco_Zona_ND", item.Preco_Zona_ND.ToString().StringToDecimalString() },
                        { "@nPreco_Zona_N", item.Preco_Zona_N.ToString().StringToDecimalString() },
                        { "@nPreco_Zona_CO", item.Preco_Zona_CO.ToString().StringToDecimalString() },
                        { "@nPreco_Zona_S", item.Preco_Zona_S.ToString().StringToDecimalString() },
                        { "@nTotal", item.NTotal.ToString().StringToDecimalString() },
                        { "@sUnidade", item.SUnidade },
                        { "@sLiberado", idTipoTabela == 1 ? item.bLiberado ? "S" : "N" : "N" },
                        { "@idUsuarioAtualizacao", Variaveis.idUsuario() },
                        { "@idTabelaOrigem", item.IdTabelaOrigem.ToString() },
                        { "@idTipoTabela", idTipoTabela.ToString() }
                    };
                    DataSet dsItens = ExecutarDataSet(sProcedure, vParametros);

                    if (!string.IsNullOrEmpty(DATASET(dsItens, 0, "sErro")))
                        throw new Exception();
                }

                return true;
            }
            catch { }

            return false;
        }

        /// <summary>
        /// WebMethod para Salvar os Dados da Tabela de Preços.
        /// </summary>
        /// <param name="idTabela">Recebe o ID da Tabela.</param>
        /// <param name="idTipoTabela">Recebe o ID do Tipo de Tabela.</param>
        /// <param name="idMoedaOrigem">Recebe o ID da Moeda de Origem.</param>
        /// <param name="idMoedaDestino">Recebe o ID da Moeda de Destino.</param>
        /// <param name="sDscTituloTabela">Recebe o Título da Tabela.</param>
        /// <param name="sDscObservacao">Recebe a Observação da Tabela.</param>
        /// <param name="dtInicial">Recebe a Data Inicial da Tabela.</param>
        /// <param name="dtFinal">Recebe a Data Final da Tabela.</param>
        /// <param name="fator">Recebe o Fator Global da Tabela.</param>
        /// <param name="desconto">Recebe o Desconto Global da Tabela.</param>
        /// <param name="envio">Recebe a Taxa de Envio Global da Tabela.</param>
        /// <param name="local">Recebe a Taxa Local Global da Tabela.</param>
        /// <param name="margem">Recebe a Margem Global da Tabela.</param>
        /// <param name="cambio">Recebe o Valor do Câmbio da Tabela.</param>
        /// <param name="sidTabelasImportadas">Recebe os IDs das Tabelas Importadas/Vinculadas.</param>
        /// <param name="sAtivo">Recebe o valor que define se a Tabela está Ativa.</param>
        /// <param name="sValidada">Recebe o valor que define se a Tabela está Validada.</param>
        /// <param name="sCalcula_NCM">Recebe o valor que define se a Tabela Calcula Impostos (NCM).</param>
        /// <param name="sidFornecedores">Recebe os IDs dos Fornecedores vinculados.</param>
        /// <returns>Retorna o erro gerado, caso exista.</returns>
        [WebMethod]
        public static object Post_SalvarDados(int idTabela, int idTipoTabela, int idMoedaOrigem, int idMoedaDestino, string sDscTituloTabela, string sDscObservacao, string dtInicial, string dtFinal, string fator, string desconto, string envio,
            string local, string margem, string cambio, string sidTabelasImportadas, string sAtivo, string sValidada, bool bCalcula_NCM, string sidFornecedores)
        {
            string sErro = "";
            string sidTabela = idTabela.ToString();
            DateTime.TryParse(dtInicial, out DateTime dt_1);
            DateTime.TryParse(dtFinal, out DateTime dt_2);
            sidTabelasImportadas = sidTabelasImportadas.Length > 1 && !sidTabelasImportadas.EndsWith("|") ? sidTabelasImportadas + "|" : sidTabelasImportadas;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR" },
                { "@idTabela", idTabela.ToString() },
                { "@idTipoTabela", idTipoTabela.ToString() },
                { "@idMoedaOrigem", idMoedaOrigem.ToString() },
                { "@idMoedaDestino", idMoedaDestino.ToString() },
                { "@sDscTabela", sDscTituloTabela },
                { "@sObservacao", sDscObservacao },
                { "@dtVigencia_Inicial", dt_1.ToString("dd/MM/yyyy") },
                { "@dtVigencia_Final", dt_2.ToString("dd/MM/yyyy") },
                { "@nFatorTabela", fator },
                { "@nDesconto", desconto },
                { "@nTaxaEnvioTabela", envio },
                { "@nTaxaLocalTabela", local },
                { "@nMargemTabela", margem },
                { "@nCambio", cambio },
                { "@idTabelasImportadas", sidTabelasImportadas },
                { "@sSituacao", sAtivo },
                { "@sValidada", sValidada },
                { "@sCalculaImpostos", bCalcula_NCM ? "S" : "N" },
                { "@sidParceiroFornecedor", sidFornecedores },
                { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
            };
            DataSet dsSalvar = ExecutarDataSet(sProcedure, vParametros);

            if (ValidarDataSet(dsSalvar, out sErro))
            {
                sidTabela = DATASET(dsSalvar, 0, "idTabela");
                sErro = DATASET(dsSalvar, 0, "sErro");
            }

            return new { idTabela = sidTabela, sErro = string.IsNullOrEmpty(sErro) ? "" : sErro };
        }

        /// <summary>
        /// WebMethod que Consulta os Valores de Câmbio das Moedas.
        /// </summary>
        /// <param name="idMoedaOrigem">Recebe o ID da Moeda de Origem.</param>
        /// <param name="idMoedaDestino">Recebe o ID da Moeda de Destino.</param>
        /// <returns>Retorna o valor do Câmbio calculado e a string com os Símbolos das Moedas.</returns>
        [WebMethod]
        public static object GetValorCambio(int idMoedaOrigem, int idMoedaDestino)
        {
            Dictionary<string, string> vParam = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA_MOEDAS" },
                { "@idMoedaOrigem", idMoedaOrigem.ToString() },
                { "@idMoedaDestino", idMoedaDestino.ToString() }
            };
            DataTable dt = ExecutarDataTable(sProcedure, vParam);

            decimal cambio = dt.Rows[0].Field<decimal>("nValorCambio") / dt.Rows[1].Field<decimal>("nValorCambio");
            string sSimbolos = string.Format("{0} / {1}", dt.Rows[0].Field<string>("sSimbolo"), dt.Rows[1].Field<string>("sSimbolo"));

            return new { Cambio = cambio, Simbolos = sSimbolos };
        }

        /// <summary>
        /// WebMethod que Popula os DDLs da Importação de Tabelas.
        /// </summary>
        /// <param name="idTabela">Recebe o ID da Tabela.</param>
        /// <param name="idTipoTabela">Recebe o ID do Tipo de Tabela.</param>
        /// <param name="idMoedaOrigem">Recebe o ID da Moeda de Origem.</param>
        /// <returns>Retorna uma Lista de Listas com os Itens dos DDLs.</returns>
        [WebMethod]
        public static List<List<ListItem>> Get_PopulaCombo_ImportarTabelas(int idTabela, int idTipoTabela, int idMoedaOrigem)
        {
            List<List<ListItem>> lista = new List<List<ListItem>>();
            ListBox tabelas = new ListBox();
            ListBox vinculadas = new ListBox();
            ListBox desvincular = new ListBox();

            if (idTabela > 0)
            {
                Popula_Combo(tabelas, string.Format("sp_Select 'Flow_Comercial_TabelaPreco', @idPesquisa={0}, @idFiltro=0", idTabela), "idTabela", "sDscTabela", false);
                Popula_Combo(vinculadas, string.Format("sp_Select 'Flow_Comercial_TabelaPreco', @idPesquisa={0}, @idFiltro=1", idTabela), "idTabela", "sDscTabela", false);
                Popula_Combo(desvincular, string.Format("sp_Select 'Flow_Comercial_TabelaPreco', @idPesquisa={0}, @idFiltro=2", idTabela), "idTabela", "sDscTabela", true);

                DataTable dt = ExecutarDataTable("sp_Select", new Dictionary<string, string> { { "@sTabela", "Flow_Parceiros_Fornecedores" } });
                List<ListItem> fornecedores = new List<ListItem>();

                foreach (DataRow fornecedor in dt.Rows)
                {
                    string tabelaPreco = fornecedor.Field<string>("idTabelaPreco");
                    string[] tabelaPrecoSplit = tabelaPreco.Split('|');

                    if (int.TryParse(tabelaPrecoSplit[0], out int idTabelaPreco) && (idTabelaPreco == 0 || idTabelaPreco.Equals(idTabela)) && tabelaPrecoSplit[1].Equals(idTipoTabela.ToString()))
                        fornecedores.Add(new ListItem(fornecedor.Field<string>("sCPNJ_RazaoSocial"), fornecedor.Field<int>("idCliente").ToString()));
                }

                lista.Add(vinculadas.Items.Cast<ListItem>().ToList());
                lista.Add(desvincular.Items.Cast<ListItem>().ToList());
                lista.Add(fornecedores);
            }
            else if (idTipoTabela == 1 && idMoedaOrigem > 0)
                Popula_Combo(tabelas, string.Format("sp_Select 'Flow_Comercial_TabelaPreco', @idPesquisa=0, @idFiltro={0}, @idPais={1}", idTipoTabela, idMoedaOrigem), "idTabela", "sDscTabela", false);

            lista.Insert(0, tabelas.Items.Cast<ListItem>().ToList());

            return lista;
        }

        /// <summary>
        /// WebMethod que Consulta os Itens da Tabela.
        /// </summary>
        /// <param name="idTabela">ID da Tabela.</param>
        /// <param name="bNacional">Define se a Tabela é Nacional.</param>
        /// <param name="sGrupos">Recebe uma string concatenada, dos IDs dos Grupos selecionados, para Consulta de Itens.</param>
        /// <returns>Retorna uma string com os dados dos Itens da Tabela.</returns>
        [WebMethod]
        public static string GetItens_Tabela(string idTabela, bool bNacional, string sGrupos)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_ITENS_TABELA" },
                { "@idTabela", idTabela },
                { "@sGrupos_Filtro", sGrupos }
            };
            DataSet ds = ExecutarDataSet(sProcedure, vParametros, out string sql);

            var itens = new List<object>();

            foreach (DataRow row in ds.Tables[1].Rows)
            {
                string dtAtualizacao = "N";
                string dtInclusao = "N";

                if (DateTime.TryParse(row["dtAtualizacao"].ToString(), out DateTime dtA)) dtAtualizacao = dtA.ToString("s");
                if (DateTime.TryParse(row["dtInclusao"].ToString(), out DateTime dtI)) dtInclusao = dtI.ToString("s");

                cls_Comercial_Tabelas item = new cls_Comercial_Tabelas
                {
                    nOrdem = int.Parse(row["nOrdem"].ToString()),
                    IdItem = int.Parse(row["idItem"].ToString()),
                    IdTabelaOrigem = int.Parse(row["idTabelaOrigem"].ToString()),
                    SCodigo = row["sCodigo"].ToString(),
                    SDscProduto = row["sDscProduto"].ToString(),
                    SCodigoComDescricao = row["sCodigoComDescricao"].ToString(),
                    TipoProduto = row["sDscTipoProduto"].ToString(),
                    SUnidade = row["sUnidade"].ToString(),
                    IdGrupoProduto = int.Parse(row["idGrupo"].ToString()),
                    IdFamiliaProduto = int.Parse(row["idFamilia"].ToString()),
                    sDscGrupoProduto = row["sDscGrupo"].ToString(),
                    sDscFamiliaProduto = row["sDscFamilia"].ToString(),
                    Preco = decimal.Parse(row["nPreco"].ToString()),
                    Preco_Zona_SD = decimal.Parse(row["nPreco_Zona_SD"].ToString()),
                    Preco_Zona_ND = decimal.Parse(row["nPreco_Zona_ND"].ToString()),
                    Preco_Zona_N = decimal.Parse(row["nPreco_Zona_N"].ToString()),
                    Preco_Zona_CO = decimal.Parse(row["nPreco_Zona_CO"].ToString()),
                    Preco_Zona_S = decimal.Parse(row["nPreco_Zona_S"].ToString()),
                    NFator = decimal.Parse(row["nFator"].ToString()),
                    NMargem = decimal.Parse(row["nMargem"].ToString()),
                    NEnvio = decimal.Parse(row["nTaxaEnvio"].ToString()),
                    NLocal = decimal.Parse(row["nTaxaLocal"].ToString()),
                    NTotal = decimal.Parse(row["nTotal"].ToString()),
                    NII = decimal.Parse(bNacional ? row["nII"].ToString() : row["nII_Internacional"].ToString()),
                    NIPI = decimal.Parse(bNacional ? row["nIPI"].ToString() : row["nIPI_Internacional"].ToString()),
                    NPIS = decimal.Parse(bNacional ? row["nPIS"].ToString() : row["nPIS_Internacional"].ToString()),
                    NCOFINS = decimal.Parse(bNacional ? row["nCOFINS"].ToString() : row["nCOFINS_Internacional"].ToString()),
                    NICMS = decimal.Parse(bNacional ? row["nICMS"].ToString() : row["nICMS_Internacional"].ToString()),
                    nVlr_II = 0,
                    nVlr_IPI = 0,
                    nVlr_PIS = 0,
                    nVlr_COFINS = 0,
                    nVlr_ICMS = 0,
                    NImpostos = 0,
                    DtAtualizacao = dtAtualizacao,
                    dtInclusao = dtInclusao,
                    SFuncao = row["sFuncao"].ToString(),
                    bLiberado = row["sLiberado"].ToString() == "S",
                    bImportado = false,
                    sIndustrializado = row["sIndustrializado"].ToString()
                };

                itens.Add(item);
            }

            return JsonConvert.SerializeObject(itens);
        }

        /// <summary>
        /// WebMethod que Consulta os Controles de Fator da Tabela de Preços.
        /// </summary>
        /// <param name="idTabela">ID da Tabela.</param>
        /// <returns>Retorna uma string com os dados dos Controles de Fator da Tabela.</returns>
        [WebMethod]
        public static string GetControlesFator_Tabela(string idTabela)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_CONTROLES_FATOR_x_TABELA" },
                { "@idTabela", idTabela }
            };
            DataTable dt = ExecutarDataTable(sProcedure, vParametros, false);

            var itens = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                var item = new
                {
                    idObjeto = row["idObjeto"],
                    idTipoObjeto = row["idTipoObjeto"],
                    sDscObjeto = row["sDscObjeto"],
                    nValor = row["nFator"],
                    sBloquearEdicao = row["sBloquearEdicao"]
                };

                itens.Add(item);
            }

            return JsonConvert.SerializeObject(itens);
        }

        /// <summary>
        /// WebMethod que Consulta as Permissões do Usuário de acordo com o Tipo de Tabela e sua Moeda de Origem.
        /// </summary>
        /// <param name="idTipo">ID do Tipo de Tabela.</param>
        /// <param name="idMoedaOrigem">ID da Moeda de Origem da Tabela.</param>
        /// <returns>Retorna um valor de 0 a 3, que representa o Nível de Permissão do Usuário.</returns>
        [WebMethod]
        public static object Get_NivelPermissao(int idTipo, int idMoedaOrigem)
        {
            int nNivel_Permissao = 0;

            if (ValidaPermissao(Permissao.Comercial.TabelaDePreco.Master))
            {
                nNivel_Permissao = 3;

                goto Retorno;
            }
            else if (idTipo.Equals(11))
            {
                if (ValidaPermissao(Permissao.Comercial.TabelaDePreco.Visualizar_LPU))
                    nNivel_Permissao = 1;
                else if (ValidaPermissao(Permissao.Comercial.TabelaDePreco.Editar_LPU))
                    nNivel_Permissao = 2;

                goto Retorno;
            }

            if (idMoedaOrigem.Equals(2))
            {
                if ((idTipo.Equals(1) || idTipo.Equals(3)) && ValidaPermissao(Permissao.Comercial.TabelaDePreco.Visualizar_Vendas_Nacional))
                    nNivel_Permissao = 1;
                else if ((idTipo.Equals(2) || idTipo.Equals(4) || idTipo.Equals(8) || idTipo.Equals(10)) && ValidaPermissao(Permissao.Comercial.TabelaDePreco.Visualizar_Tabelas_de_Controle_Nacional))
                    nNivel_Permissao = 1;
                else if (idTipo.Equals(5) && ValidaPermissao(Permissao.Comercial.TabelaDePreco.Visualizar_Fornecedor_Nacional))
                    nNivel_Permissao = 1;

                if ((idTipo.Equals(2) || idTipo.Equals(4) || idTipo.Equals(8) || idTipo.Equals(10)) && ValidaPermissao(Permissao.Comercial.TabelaDePreco.Editar_Tabelas_de_Controle_Nacional))
                    nNivel_Permissao = 2;
                else if (idTipo.Equals(5) && ValidaPermissao(Permissao.Comercial.TabelaDePreco.Editar_Fornecedor_Nacional))
                    nNivel_Permissao = 2;
            }
            else
            {
                if ((idTipo.Equals(1) || idTipo.Equals(3)) && ValidaPermissao(Permissao.Comercial.TabelaDePreco.Visualizar_Vendas_Internacional))
                    nNivel_Permissao = 1;
                else if ((idTipo.Equals(2) || idTipo.Equals(4) || idTipo.Equals(8) || idTipo.Equals(10)) && ValidaPermissao(Permissao.Comercial.TabelaDePreco.Visualizar_Tabelas_de_Controle_Internacional))
                    nNivel_Permissao = 1;
                else if (idTipo.Equals(5) && ValidaPermissao(Permissao.Comercial.TabelaDePreco.Visualizar_Fornecedor_Nacional))
                    nNivel_Permissao = 1;

                if ((idTipo.Equals(2) || idTipo.Equals(4) || idTipo.Equals(8) || idTipo.Equals(10)) && ValidaPermissao(Permissao.Comercial.TabelaDePreco.Editar_Tabelas_de_Controle_Internacional))
                    nNivel_Permissao = 2;
                else if (idTipo.Equals(5) && ValidaPermissao(Permissao.Comercial.TabelaDePreco.Editar_Fornecedor_Internacional))
                    nNivel_Permissao = 2;
            }

        Retorno:
            if (nNivel_Permissao.Equals(0))
                return $"/App/PermissaoNegada.aspx?Recurso={Permissao.Comercial.TabelaDePreco.Master.ToString().PadLeft(4, '0')}";

            return new { nNivel = nNivel_Permissao, bAlteraFator = ValidaPermissao(Permissao.Produtos.AlterarFatorGlobal) };
        }

        /// <summary>
        /// WebMethod que busca Produtos de acordo com a Descrição do Produto.
        /// </summary>
        /// <param name="sDscProduto">Código do Produto.</param>
        /// <param name="idTipoProduto">ID do Tipo do Produto.</param>
        /// <param name="idFamilia">ID da Família do Produto.</param>
        /// <param name="idGrupo">ID do Grupo do Produto.</param>
        /// <param name="idPaisOrigem">ID do País de Origem do Produto.</param>
        /// <param name="idMoedaOrigem">ID da Moeda de Origem da Tabela de Preços.</param>
        /// <returns>Retorna um array de string com os dados dos Produtos</returns>
        [WebMethod]
        public static string[] GetProdutos(string sDscProduto, string idTipoProduto, string idFamilia, string idGrupo, string idPaisOrigem, string idMoedaOrigem, string idTipoTabela)
        {
            string sDscPesquisa = sDscProduto.Trim('"').Trim();
            List<string> lstProdutos = new List<string>();

            if (sDscProduto.Length > 3)
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR" },
                    { "@sDscProduto", sDscPesquisa },
                    { "@sTipoPesquisa", sDscPesquisa },
                    { "@sCodigo", sDscPesquisa },
                    { "@idTipoProduto", idTipoProduto },
                    { "@idFamilia", idFamilia },
                    { "@idGrupo", idGrupo },
                    { "@idPais", idPaisOrigem },
                    { "@idUsuarioAtualizacao", Variaveis.idUsuario() },
                    { "@sSituacao", "S" },
                    { "@sSubTipo", "-1" }
                };
                DataTable tb = ExecutarDataTable(sProcedure_Produtos, vParametros, false);

                foreach (DataRow row in tb.Rows)
                {
                    string lista;
                    if (idMoedaOrigem == "2")
                    {
                        lista = string.Format(
                            "{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}",
                            row["sDscProduto"],
                            row["sCodigo"],
                            row["idItem"],
                            row["sDscTipoProduto"],
                            row["sUnidade"],
                            row["idGrupo"],
                            row["idFamilia"],
                            row["sDscGrupo"],
                            row["sDscFamilia"],
                            row["nII"],
                            row["nIPI"],
                            row["nPIS"],
                            row["nCofins"],
                            row["nICMS"],
                            row["sIndustrializado"]
                            );
                    }
                    else
                    {
                        lista = string.Format(
                            "{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}",
                            row["sDscProduto"],
                            row["sCodigo"],
                            row["idItem"],
                            row["sDscTipoProduto"],
                            row["sUnidade"],
                            row["idGrupo"],
                            row["idFamilia"],
                            row["sDscGrupo"],
                            row["sDscFamilia"],
                            row["nII_Internacional"],
                            row["nIPI_Internacional"],
                            row["nPIS_Internacional"],
                            row["nCOFINS_Internacional"],
                            row["nICMS_Internacional"],
                            row["sIndustrializado"]
                            );
                    }

                    if (row["sIndustrializado"].ToString() == "S" && idTipoTabela == "4")
                        lstProdutos.Add(lista);
                    else if (row["sIndustrializado"].ToString() != "S" && idTipoTabela != "4")
                        lstProdutos.Add(lista);
                }
            }

            return lstProdutos.ToArray();
        }

        /// <summary>
        /// WebMethod que busca Produtos de acordo com o Código do Produto.
        /// </summary>
        /// <param name="sDscProduto">Código do Produto.</param>
        /// <param name="idTipoProduto">ID do Tipo do Produto.</param>
        /// <param name="idFamilia">ID da Família do Produto.</param>
        /// <param name="idGrupo">ID do Grupo do Produto.</param>
        /// <param name="idPaisOrigem">ID do País de Origem do Produto.</param>
        /// <param name="idMoedaOrigem">ID da Moeda de Origem da Tabela de Preços.</param>
        /// <returns>Retorna um array de string com os dados dos Produtos</returns>
        [WebMethod]
        public static string[] GetProdutos_Codigo(string sDscProduto, string idTipoProduto, string idFamilia, string idGrupo, string idPaisOrigem, string idMoedaOrigem, string idTipoTabela)
        {
            string sDscPesquisa = sDscProduto.Trim('"');
            List<string> lstProdutos = new List<string>();

            if (sDscProduto.Length > 0)
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR" },
                    { "@sDscProduto", sDscPesquisa },
                    { "@sTipoPesquisa", sDscPesquisa },
                    { "@sCodigo", sDscPesquisa },
                    { "@idTipoProduto", idTipoProduto },
                    { "@idFamilia", idFamilia },
                    { "@idGrupo", idGrupo },
                    { "@idPais", idPaisOrigem },
                    { "@idUsuarioAtualizacao", Variaveis.idUsuario() },
                    { "@sSituacao", "S" },
                    { "@sSubTipo", "-1" }
                };
                DataTable tb = ExecutarDataTable(sProcedure_Produtos, vParametros, false);

                foreach (DataRow row in tb.Rows)
                {
                    string lista;
                    if (idMoedaOrigem == "2")
                    {
                        lista = string.Format(
                            "{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}",
                            row["sCodigo"],
                            row["sDscProduto"],
                            row["idItem"],
                            row["sDscTipoProduto"],
                            row["sUnidade"],
                            row["idGrupo"],
                            row["idFamilia"],
                            row["sDscGrupo"],
                            row["sDscFamilia"],
                            row["nII"],
                            row["nIPI"],
                            row["nPIS"],
                            row["nCofins"],
                            row["nICMS"],
                            row["sIndustrializado"]
                            );
                    }
                    else
                    {
                        lista = string.Format(
                            "{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}",
                            row["sCodigo"],
                            row["sDscProduto"],
                            row["idItem"],
                            row["sDscTipoProduto"],
                            row["sUnidade"],
                            row["idGrupo"],
                            row["idFamilia"],
                            row["sDscGrupo"],
                            row["sDscFamilia"],
                            row["nII_Internacional"],
                            row["nIPI_Internacional"],
                            row["nPIS_Internacional"],
                            row["nCOFINS_Internacional"],
                            row["nICMS_Internacional"],
                            row["sIndustrializado"]
                            );
                    }

                    if (row["sIndustrializado"].ToString() == "S" && idTipoTabela == "4")
                        lstProdutos.Add(lista);
                    else if (row["sIndustrializado"].ToString() != "S" && idTipoTabela != "4")
                        lstProdutos.Add(lista);
                }


            }

            return lstProdutos.ToArray();
        }

        /// <summary>
        /// WebMethod que busca Recursos de acordo com a descrição a partir de 3 caracteres
        /// </summary>
        /// <param name="sDscProduto">Código do Produto.</param>
        /// <param name="idTipoProduto">ID do Tipo do Produto.</param>
        /// <param name="idFamilia">ID da Família do Produto.</param>
        /// <param name="idGrupo">ID do Grupo do Produto.</param>
        /// <param name="idPaisOrigem">ID do País de Origem do Produto.</param>
        /// <param name="idMoedaOrigem">ID da Moeda de Origem da Tabela de Preços.</param>
        /// <param name="tipo">ID que define se o Item é um Produto, Serviço, Sub-Serviço ou Recurso.</param>
        /// <returns>Retorna um array de string com os dados dos Recursos</returns>
        [WebMethod]
        public static string[] GetRecursos(string sDscProduto, string idTipoProduto, string idFamilia, string idGrupo, string idPaisOrigem, string idMoedaOrigem, string idTipoTabela, string tipo)
        {
            string sDscPesquisa = sDscProduto.Trim('"').Trim();
            List<string> lstProdutos = new List<string>();

            if (sDscProduto.Trim().Length > 3)
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR" },
                    { "@sDscProduto", sDscPesquisa },
                    { "@sTipoPesquisa", sDscProduto },
                    { "@sCodigo", sDscPesquisa },
                    { "@idTipoProduto", idTipoProduto },
                    { "@idFamilia", idFamilia },
                    { "@idGrupo", idGrupo },
                    { "@idPais", idPaisOrigem },
                    { "@idUsuarioAtualizacao", Variaveis.idUsuario() },
                    { "@sSituacao", "S" },
                    { "@sTIpo", tipo }
                };
                DataTable tb = ExecutarDataTable(sProcedure_Produtos, vParametros, false);

                foreach (DataRow row in tb.Rows)
                {
                    string lista = string.Format(
                        "{0}|{1}|{2}|{3}|{4}",
                         row["sDscProduto"],
                         row["sCodigo"],
                         row["idItem"],
                         row["sDscTipoProduto"],
                         row["sUnidade"]
                         );
                    lstProdutos.Add(lista);
                }
            }

            return lstProdutos.ToArray();
        }

        /// <summary>
        /// WebMethod que busca Recursos de acordo com o código a partir de 1 caractere
        /// </summary>
        /// <param name="sDscProduto">Código do Produto.</param>
        /// <param name="idTipoProduto">ID do Tipo do Produto.</param>
        /// <param name="idFamilia">ID da Família do Produto.</param>
        /// <param name="idGrupo">ID do Grupo do Produto.</param>
        /// <param name="idPaisOrigem">ID do País de Origem do Produto.</param>
        /// <param name="idMoedaOrigem">ID da Moeda de Origem da Tabela de Preços.</param>
        /// <param name="tipo">ID que define se o Item é um Produto, Serviço, Sub-Serviço ou Recurso.</param>
        /// <returns>Retorna um array de string com os dados dos Recursos</returns>
        [WebMethod]
        public static string[] GetRecursos_Codigo(string sDscProduto, string idTipoProduto, string idFamilia, string idGrupo, string idPaisOrigem, string idMoedaOrigem, string idTipoTabela, string tipo)
        {
            string sDscPesquisa = sDscProduto.Trim('"').Trim();
            List<string> lstProdutos = new List<string>();

            if (sDscProduto.Trim().Length > 3)
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR" },
                    { "@sDscProduto", sDscPesquisa },
                    { "@sTipoPesquisa", sDscProduto },
                    { "@sCodigo", sDscPesquisa },
                    { "@idTipoProduto", idTipoProduto },
                    { "@idFamilia", idFamilia },
                    { "@idGrupo", idGrupo },
                    { "@idPais", idPaisOrigem },
                    { "@idUsuarioAtualizacao", Variaveis.idUsuario() },
                    { "@sSituacao", "S" },
                    { "@sTIpo", tipo }
                };
                DataTable tb = ExecutarDataTable(sProcedure_Produtos, vParametros, false);

                foreach (DataRow row in tb.Rows)
                {
                    string lista = string.Format(
                            "{0}|{1}|{2}|{3}|{4}",
                            row["sCodigo"],
                            row["sDscProduto"],
                            row["idItem"],
                            row["sDscTipoProduto"],
                            row["sUnidade"]
                            );
                    lstProdutos.Add(lista);
                }
            }

            return lstProdutos.ToArray();
        }

        [WebMethod]
        public static void Post_SalvarExportacaoLPU(string sTitulo, List<cls_Comercial_Tabelas> itens, int idTabela)
        {
            Dictionary<string, string> vParam = new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR_EXPORTACAO" },
                { "@idTabela", idTabela.ToString() },
                { "@idTipo", "1" },
                { "@sTitulo", sTitulo },
                { "@idUsuarioAtualizacao", Variaveis.idUsuario() }
            };
            DataSet ds = ExecutarDataSet(sProcedure, vParam);

            string idExportacao = DATASET(ds, "idExportacao");

            foreach (cls_Comercial_Tabelas item in itens.OrderBy(i => i.nOrdem))
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "SALVAR_EXPORTACAO_ITENS" },
                    { "@idExportacao", idExportacao },
                    { "@idProduto", item.IdItem.ToString() },
                    { "@nOrdem", item.nOrdem.ToString() },
                    { "@nTotal", item.NTotal.ToString().Replace(".", "").Replace(",", ".") }
                };
                ExecutarDataSet(sProcedure, vParametros);
            }
        }

        [WebMethod]
        public static string Get_VisualizacaoLPU(int idTabela) => DATASET(ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_TabelaPreco", new Dictionary<string, string> { { "@sFuncao", "CONSULTA_EXPORTACOES_EXCEL" }, { "@idTabela", idTabela.ToString() } }), "sDDL");

        [WebMethod]
        public static string Post_ExportacaoLPU(int idTabela, string idVisualizacao, bool bProdutosCliente) => GerarExcel(idTabela.ToString(), idVisualizacao, bProdutosCliente, false);

        [WebMethod]
        public static (int, string) Post_ExcluirVisualizacaoLPU(string idVisualizacao, int idTabela)
        {
            (int, string) sMsg = (0, "");

            if (idVisualizacao != "0")
            {
                DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_TabelaPreco", new Dictionary<string, string> { { "@sFuncao", "EXCLUIR_EXPORTACAO" }, { "@idExportacao", idVisualizacao } });

                if (DATASET(ds, "sExcluido") == "S")
                    sMsg = (2, "Visualização Excluída com sucesso!");
                else
                    sMsg = (3, "Não foi possível excluir a Visualização selecionada!");
            }
            else sMsg = (3, "É necessário selecionar uma Visualização para ser excluída!");

            return sMsg;
        }

        #endregion
    }
}