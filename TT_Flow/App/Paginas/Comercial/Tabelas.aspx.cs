using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Controles;
using static TT.FrameWork.BD;
using static TT.FrameWork.BD.Retorno;
using static TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas
{
    public partial class Tabelas : Page
    {
        int nCol_Total = 17;
        int nCol_Foto = 23;

        protected void Page_Load(object sender, EventArgs e)
        {
            ExcelImportar.ID_FileUpload = ImportarArquivo.ClientID;
            manual.sNomeArquivo = "Manual-TabelaPreco.pdf";

            lblTituloPagina.Text = "Tabelas de Preço";
            BreadCrumb_Pagina.TitulodaPagina = "Tabelas de Preço";

            ValidaPermissao(Permissao.Comercial.TabelaDePreco.Consultar, true);
            cmdNovo.Visible = ValidaPermissao(Permissao.Comercial.TabelaDePreco.Master) || ValidaPermissao(Permissao.Comercial.TabelaDePreco.Editar_Fornecedor_Nacional) || ValidaPermissao(Permissao.Comercial.TabelaDePreco.Editar_Fornecedor_Internacional);

            if (!IsPostBack)
            {
                Popula_Combo(ddlidTipoTabela, $"sp_Select 'tbl_Flow_Comercial_TabelaPreco_Tipo', @sPesquisa='{RetornaTipoPermissao(false)}', @idFiltro=1", "idTipoTabela", "sDscTipoTabela", false, "Todos os Tipos ", "0");
                Popula_Combo(ddlidMoedaOrigem, $"sp_Select 'Flow_Adm_Moedas__TabelaPreco', @sPesquisa='{RetornaTipoPermissao(true)}', @idFiltro=1", "idMoeda", "sDscMoeda", false, "Moedas de Origem ", "0");
                ddlidMoedaDestino.Items.AddRange(ddlidMoedaOrigem.Items.Cast<ListItem>().ToArray());
                ddlidMoedaDestino.Items.RemoveAt(0);
                ddlidMoedaDestino.Items.Insert(0, new ListItem("Moedas de Destino", "0"));

                pnResultado.Visible = false;
            }

            RegistraScript();
        }

        protected void Pesquisar()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscTabela", txtPesquisa.Text.Trim() },
                { "@idTipoTabela", ddlidTipoTabela.SelectedValue },
                { "@sTerritorio", ddlTerritorioTabela.SelectedValue },
                { "@idMoedaOrigem", ddlidMoedaOrigem.SelectedValue },
                { "@idMoedaDestino", ddlidMoedaDestino.SelectedValue },
                { "@sValidadaPesquisa", ddlsValidada.SelectedValue },
                { "@tipoPermissao", RetornaTipoPermissao(false) }
            };
            DataTable tb = ExecutarDataTable("sp_Manipula_tbl_Flow_Comercial_TabelaPreco", vParametros, false);

            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(gvConsulta, tb, 0, new int[1] { 7 }, "desc", "false", "''"), true);
                pnResultado.Visible = true;
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");
                pnResultado.Visible = false;
            }

            if (ddlidTipoTabela.Items.Count <= 1)
            {
                div_TipoTabela.Visible = false;
                MensagemPagina.MostraMensagem_Erro("Não existem Tabelas disponíveis para seu nível de Permissões!");
                pnResultado.Visible = false;
            }
            else div_TipoTabela.Visible = true;
        }

        public string RetornaTipoPermissao(bool bMoeda)
        {
            string tipoPermissao = string.Empty;

            if (ValidaPermissao(Permissao.Comercial.TabelaDePreco.Master))
                return "0";

            if (bMoeda)
            {
                if (ValidaPermissao(Permissao.Comercial.TabelaDePreco.Visualizar_LPU) || ValidaPermissao(Permissao.Comercial.TabelaDePreco.Editar_LPU))
                    return "N|I";

                if (ValidaPermissao(Permissao.Comercial.TabelaDePreco.Visualizar_Vendas_Nacional)
                    || ValidaPermissao(Permissao.Comercial.TabelaDePreco.Editar_Tabelas_de_Controle_Nacional) || ValidaPermissao(Permissao.Comercial.TabelaDePreco.Visualizar_Tabelas_de_Controle_Nacional)
                    || ValidaPermissao(Permissao.Comercial.TabelaDePreco.Editar_Fornecedor_Nacional) || ValidaPermissao(Permissao.Comercial.TabelaDePreco.Visualizar_Fornecedor_Nacional)
                   )
                    tipoPermissao += "N|";

                if (ValidaPermissao(Permissao.Comercial.TabelaDePreco.Visualizar_Vendas_Internacional)
                    || ValidaPermissao(Permissao.Comercial.TabelaDePreco.Editar_Tabelas_de_Controle_Internacional) || ValidaPermissao(Permissao.Comercial.TabelaDePreco.Visualizar_Tabelas_de_Controle_Internacional)
                    || ValidaPermissao(Permissao.Comercial.TabelaDePreco.Editar_Fornecedor_Internacional) || ValidaPermissao(Permissao.Comercial.TabelaDePreco.Visualizar_Fornecedor_Internacional)
                   )
                    tipoPermissao += "I|";
            }
            else
            {
                if (ValidaPermissao(Permissao.Comercial.TabelaDePreco.Visualizar_Vendas_Nacional))
                    tipoPermissao += "1N|";
                if (ValidaPermissao(Permissao.Comercial.TabelaDePreco.Visualizar_Vendas_Internacional))
                    tipoPermissao += "1I|";
                if (ValidaPermissao(Permissao.Comercial.TabelaDePreco.Editar_Tabelas_de_Controle_Nacional) || ValidaPermissao(Permissao.Comercial.TabelaDePreco.Visualizar_Tabelas_de_Controle_Nacional))
                    tipoPermissao += "24N|";
                if (ValidaPermissao(Permissao.Comercial.TabelaDePreco.Editar_Tabelas_de_Controle_Internacional) || ValidaPermissao(Permissao.Comercial.TabelaDePreco.Visualizar_Tabelas_de_Controle_Internacional))
                    tipoPermissao += "24I|";
                if (ValidaPermissao(Permissao.Comercial.TabelaDePreco.Editar_Fornecedor_Nacional) || ValidaPermissao(Permissao.Comercial.TabelaDePreco.Visualizar_Fornecedor_Nacional))
                    tipoPermissao += "5N|";
                if (ValidaPermissao(Permissao.Comercial.TabelaDePreco.Editar_Fornecedor_Internacional) || ValidaPermissao(Permissao.Comercial.TabelaDePreco.Visualizar_Fornecedor_Internacional))
                    tipoPermissao += "5I|";
                if (ValidaPermissao(Permissao.Comercial.TabelaDePreco.Visualizar_LPU) || ValidaPermissao(Permissao.Comercial.TabelaDePreco.Editar_LPU))
                    tipoPermissao += "11|";
            }

            return tipoPermissao;
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void cmdImportarItens_Excel_Modal_Click(object sender, EventArgs e)
        {
            if (ImportarArquivo.HasFile)
            {
                try
                {
                    var itens = ExcelImportar.Retornar_Itens_Excel__TabelaPreco(ImportarArquivo);

                    foreach (var item in itens)
                    {
                        ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_TabelaPreco", new Dictionary<string, string>
                        {
                            { "@sFuncao", "ATUALIZAR_ITEM" },
                            { "@idRegistro", item.idRegistro.ToString() },
                            { "@nTaxaEnvio", item.NEnvio.ToString().Replace(",", ".") },
                            { "@nTaxaLocal", item.NLocal.ToString().Replace(",", ".") },
                            { "@nMargem", item.NMargem.ToString().Replace(",", ".") },
                            { "@nFator", item.NFator.ToString().Replace(",", ".") },
                            { "@nPreco", item.Preco.ToString().Replace(",", ".") },
                            { "@nTotal", item.NTotal.ToString().Replace(",", ".") },
                            { "@nPreco_Zona_SD", item.Preco_Zona_SD.ToString().Replace(",", ".") },
                            { "@nPreco_Zona_ND", item.Preco_Zona_ND.ToString().Replace(",", ".") },
                            { "@nPreco_Zona_N", item.Preco_Zona_N.ToString().Replace(",", ".") },
                            { "@nPreco_Zona_CO", item.Preco_Zona_CO.ToString().Replace(",", ".") },
                            { "@nPreco_Zona_S", item.Preco_Zona_S.ToString().Replace(",", ".") },
                            { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() }
                        });
                    }

                    Pesquisar();

                    MensagemPagina.MostraMensagem_Sucesso("Itens da Tabela Importados com sucesso!");
                }
                catch (Exception ex)
                {
                    Mensagem_Modal_ImportarProdutos.MostraMensagem_Erro("Houve um erro ao processar o arquivo. Verifique se a formatação do arquivo está correta e tente novamente.<br />Erro: " + ex.Message);
                    Scripts.AbrirModal(Page, "modalUpload_ImportarExcel");
                }
            }
            else
            {
                Mensagem_Modal_ImportarProdutos.MostraMensagem_Erro("Selecione um Arquivo para Importar!");
                Scripts.AbrirModal(Page, "modalUpload_ImportarExcel");
            }
        }

        protected void cmdExcluirVisualizacao_Click(object sender, EventArgs e)
        {
            if (hddVisualizacao.Value != "0")
            {
                DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_TabelaPreco", new Dictionary<string, string> { { "@sFuncao", "EXCLUIR_EXPORTACAO" }, { "@idExportacao", hddVisualizacao.Value } });

                if (DATASET(ds, "sExcluido") == "S")
                    MensagemPagina_ModalExportar_LPU.MostraMensagem_Sucesso("Visualização Excluída com sucesso!");
                else
                    MensagemPagina_ModalExportar_LPU.MostraMensagem_Erro("Não foi possível excluir a Visualização selecionada!");
            }
            else MensagemPagina_ModalExportar_LPU.MostraMensagem_Erro("É necessário selecionar uma Visualização para ser excluída!");

            ltrVisualizacao.Text = DATASET(ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_TabelaPreco", new Dictionary<string, string> { { "@sFuncao", "CONSULTA_EXPORTACOES_EXCEL" }, { "@idTabela", cmdExportarExcel.Attributes["data-idTabela"] } }), "sDDL");
            hddVisualizacao.Value = "0";

            Scripts.RemoverBackdrop_Modal(Page);
            Scripts.AbrirModal(Page, "modalExportar_LPU");
        }

        protected void cmdExportarExcel_Click(object sender, EventArgs e) => GerarExcel((sender as Button).Attributes["data-idTabela"], hddVisualizacao.Value, false);

        protected void gvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
                (e.Row.FindControl("lnkExibicao") as LinkButton).Visible = gvConsulta.DataKeys[e.Row.RowIndex]["idTipoTabela"].ToString().Equals("11");
        }

        protected void gvConsulta_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string sidTabela = e.CommandArgument.ToString().Trim();

            if (e.CommandName == "LPU")
            {
                ltrVisualizacao.Text = DATASET(ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_TabelaPreco", new Dictionary<string, string> { { "@sFuncao", "CONSULTA_EXPORTACOES_EXCEL" }, { "@idTabela", sidTabela } }), "sDDL");
                hddVisualizacao.Value = "0";
                cmdExportarExcel.Attributes["data-idTabela"] = sidTabela;
                Scripts.AbrirModal(Page, "modalExportar_LPU");
            }
            else if (e.CommandName == "Exportar")
            {
                string tabela = GerarExcel(sidTabela, "0", true);
                MensagemPagina.MostraMensagem_Sucesso($"Itens da tabela <b>{tabela}</b> exportados com sucesso!");
            }
            else if (e.CommandName == "Importar")
            {
                hddTabela.Value = sidTabela.Split('-')[0].Trim();
                txtTabela_ImportarProdutos.Text = sidTabela;

                Mensagem_Modal_ImportarProdutos.MostraMensagem_Aviso("É necessário que o arquivo para Importação esteja com a mesma formatação de quando foi Exportado desta página!");
                Scripts.AbrirModal(Page, "modalUpload_ImportarExcel");
            }

            Pesquisar();
        }

        protected string GerarExcel(string idTabela, string idExportacao, bool bEdita)
        {
            string arquivo = "";

            DataSet ds = ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_TabelaPreco", new Dictionary<string, string> { { "@sFuncao", "EXPORTAR_ITENS_EXCEL" }, { "@idTabela", idTabela }, { "@idExportacao", idExportacao }, { "@sProdutosClientes", cbProdutosCliente.Checked ? "S" : "N" }, { "@sFoto", !bEdita ? "S" : "N" } });
            string tabela = DATASET(ds, "sDscTabela");

            if (DATASET(ds, "idTipo") == "2")
                arquivo = "Excel_LPU/" + DATASET(ds, "sTitulo");
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

                    arquivo = $"Itens_TabelaPreco_{idTabela}-{CarimboDataHora()}.xlsx";

                    if (!bEdita)
                    {
                        ExecutarDataSet("sp_Manipula_tbl_Flow_Comercial_TabelaPreco", new Dictionary<string, string> { { "@sFuncao", "SALVAR_EXPORTACAO" }, { "@idTabela", idTabela }, { "@idTipo", "2" }, { "@sTitulo", arquivo }, { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() } });

                        arquivo = "Excel_LPU/" + arquivo;
                        Directory.CreateDirectory(Server.MapPath("~/Download/") + "Excel_LPU/");
                    }

                    excel.SaveAs(new FileInfo(Path.Combine(Server.MapPath("~/Download/"), arquivo)));
                }
            }

            string script = $@"
                const link = document.createElement('a');
                link.href = '/Download/' + '{arquivo}';
                link.target = '_blank';
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            ";
            ScriptManager.RegisterStartupScript(Page, GetType(), "js_BaixarExcel", script, true);
            Scripts.RemoverBackdrop_Modal(Page);

            return tabela;
        }

        protected List<(int, string, bool)> RetornaColunas_Valores(int idTipoTabela, bool bMoeda_2, bool bNCM)
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

        protected string Retorna_Formula(int idTipoTabela, bool bMoeda_2, bool bNCM)
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

        protected void AplicaEstilos(ExcelRange linha, Color cor, string valor, bool bMescla, bool bNegrito, int nTamanhoFonte, bool bQuebraLinha, ExcelBorderStyle? borda, ExcelHorizontalAlignment alinhamento, string formato = null, bool bBloqueado = true)
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

        protected void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("$(document).ready(function() {");
            sb.AppendLine("     $('#ddlVisualizacao').change(function() {");
            sb.AppendLine($"        $('#{hddVisualizacao.ClientID}').val($(this).val());");
            sb.AppendLine("     });");
            sb.AppendLine("});");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_RegistraScript", sb.ToString(), true);
        }
    }
}