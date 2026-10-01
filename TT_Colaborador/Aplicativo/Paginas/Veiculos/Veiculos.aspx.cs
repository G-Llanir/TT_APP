using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT_Colaborador.FrameWork;
using static TT.FrameWork.BD;
using static TT.FrameWork.Funcoes;
using static TT.FrameWork.Identity;
using static TT_Colaborador.FrameWork.cls_Veiculos;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Colaborador.Aplicativo.Paginas.Veiculos
{
    public partial class Veiculos : Page
    {
        string sProcedure = "sp_Manipula_tbl_Flow_Veiculos";

        #region | Listas

        public List<cls_Veiculos> Lista_Veiculos_Movimentacao
        {
            get
            {
                if (ViewState["Lista_Veiculos_Movimentacao"] == null) ViewState["Lista_Veiculos_Movimentacao"] = new List<cls_Veiculos>();
                return (List<cls_Veiculos>)ViewState["Lista_Veiculos_Movimentacao"];
            }
            set { ViewState["Lista_Veiculos_Movimentacao"] = value; }
        }

        public List<cls_Tipos_Movimentacao> Lista_Tipos_Movimentacao = new List<cls_Tipos_Movimentacao>
        {
            new cls_Tipos_Movimentacao { idTipo = 1, sDscTipo = "Saída TT / Locadora", bRepete = false, bChecklist_Completo = true },
            new cls_Tipos_Movimentacao { idTipo = 2, sDscTipo = "Chegada no Cliente", bRepete = true, bChecklist_Completo = false },
            new cls_Tipos_Movimentacao { idTipo = 3, sDscTipo = "Chegada em Local de Descanso", bRepete = true, bChecklist_Completo = false },
            new cls_Tipos_Movimentacao { idTipo = 4, sDscTipo = "Saída de Descanso", bRepete = true, bChecklist_Completo = false },
            new cls_Tipos_Movimentacao { idTipo = 5, sDscTipo = "Chegada TT / Locadora", bRepete = false, bChecklist_Completo = true }
        };

        #endregion

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ValidaPermissao(Permissao.Veiculos.Consultar, true, true);
                PopulaCombos();
                Pesquisar();
                Scripts.FocusScript(Page, txtPesquisa.ClientID);
            }

            if (Request["__EVENTTARGET"] == "INCLUIR_ITEM") IncluirItem();

            Upload_Arquivo_FotoPainel.ID_FileUpload = fuAdicionarArquivo_FotoPainel.ClientID;
            Upload_Arquivo_FotoPainel.FuncaoPersonalizada_Script = "AdicionaArquivo($fileInput);";

            Upload_Arquivo_FotoFrente.ID_FileUpload = fuAdicionarArquivo_FotoFrente.ClientID;
            Upload_Arquivo_FotoFrente.FuncaoPersonalizada_Script = "AdicionaArquivo($fileInput);";

            Upload_Arquivo_FotoTraseira.ID_FileUpload = fuAdicionarArquivo_FotoTraseira.ClientID;
            Upload_Arquivo_FotoTraseira.FuncaoPersonalizada_Script = "AdicionaArquivo($fileInput);";

            Upload_Arquivo_FotoLatDireita.ID_FileUpload = fuAdicionarArquivo_FotoLatDireita.ClientID;
            Upload_Arquivo_FotoLatDireita.FuncaoPersonalizada_Script = "AdicionaArquivo($fileInput);";

            Upload_Arquivo_FotoLatEsquerda.ID_FileUpload = fuAdicionarArquivo_FotoLatEsquerda.ClientID;
            Upload_Arquivo_FotoLatEsquerda.FuncaoPersonalizada_Script = "AdicionaArquivo($fileInput);";

            RegistraScript();
        }

        protected void Pesquisar()
        {
            cmdNovo.Visible = ValidaPermissao(Permissao.Veiculos.Incluir, false, true);

            div_filtros.Visible = true;
            div_consulta.Visible = true;
            div_detalhe.Visible = false;
            PainelAtualizacao.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTA_MOVIMENTACAO" },
                { "@sPesquisa", txtPesquisa.Text.Trim() },
                { "@idVeiculo", ddlVeiculo_Filtro.SelectedValue },
                { "@idUsuario", Variaveis.idUsuario() }
            };
            DataSet ds = ExecutarDataSet(sProcedure, vParametros);

            if (ValidarDataSet(ds))
            {
                rptConsulta.DataSource = ds.Tables[0];
                rptConsulta.DataBind();
            }
            else
            {
                div_consulta.Visible = false;
                MensagemPagina.MostraMensagem_Erro("Nenhum Registro Localizado!");
            }
        }

        void Pesquisar(string idMovimentacao)
        {
            try
            {
                LimpaCampos();

                div_consulta.Visible = false;
                div_filtros.Visible = false;
                div_detalhe.Visible = true;
                PainelAtualizacao.Visible = false;

                if (idMovimentacao != "0")
                {
                    DataSet ds = ExecutarDataSet(sProcedure, new Dictionary<string, string> { { "@sFuncao", "CONSULTA_MOVIMENTACAO_DETALHE" }, { "@idMovimentacao", idMovimentacao } });

                    if (ValidarDataSet(ds, out string sErro))
                    {
                        cmdSalvar.Visible = ValidaPermissao(Permissao.Veiculos.Editar, false, true);
                        div_IncluirItens.Visible = ValidaPermissao(Permissao.Veiculos.Editar, false, true);

                        hddidMovimentacao.Value = RETORNO.DATASET(ds, "idMovimentacao");
                        txtID.Text = RETORNO.DATASET(ds, "idMovimentacao");
                        ddlVeiculo.SelectedValue = RETORNO.DATASET(ds, "idVeiculo");

                        if (ddlVeiculo.SelectedValue != "-1") div_VeiculoAlugado.Visible = false;
                        else
                        {
                            div_VeiculoAlugado.Visible = true;

                            txtMarca.Text = RETORNO.DATASET(ds, "sMarca");
                            txtModelo.Text = RETORNO.DATASET(ds, "sModelo");
                            txtPlaca.Text = RETORNO.DATASET(ds, "sPlaca");
                            txtAno.Text = RETORNO.DATASET(ds, "sAnoModelo");
                            txtCor.Text = RETORNO.DATASET(ds, "sCor");
                            swSeguro.Checked = RETORNO.DATASET(ds, "sSeguro").Equals("S");
                            swRastreador.Checked = RETORNO.DATASET(ds, "sRastreador").Equals("S");
                            txtLocadora.Text = RETORNO.DATASET(ds, "sLocadora");
                            txtRodizio.Text = RETORNO.DATASET(ds, "sRodizio");

                            txtRetirada.Text = DateTime.TryParse(RETORNO.DATASET(ds, "dtRetirada"), out DateTime dtRetirada) ? dtRetirada.ToString("yyyy-MM-dd") : "";
                            txtDevolucao.Text = DateTime.TryParse(RETORNO.DATASET(ds, "dtDevolucao"), out DateTime dtDevolucao) ? dtDevolucao.ToString("yyyy-MM-dd") : "";
                        }

                        PopulaItens(ds.Tables[1]);

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(ds, "dtAtualizacao"), RETORNO.DATASET(ds, "sDscUsuario"));
                    }
                    else MensagemPagina.MostraMensagem_Erro("Erro ao consultar as informações da Movimentação!<br /> Erro: " + sErro);
                }
                else
                {
                    div_VeiculoAlugado.Visible = false;

                    cmdSalvar.Visible = ValidaPermissao(Permissao.Veiculos.Incluir, false, true);

                    rptViagens_dataBind();

                    hddidMovimentacao.Value = "0";
                    txtID.Text = "Novo";
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao Pesquisar a Movimentação!<br /> Erro: " + ex.Message);
            }

            Scripts.FocusScript(Page, ddlVeiculo.ClientID);
        }

        #endregion

        #region | rptViagens

        private void PopulaItens(DataTable dt)
        {
            Lista_Veiculos_Movimentacao.Clear();

            foreach (DataRow row in dt.Rows)
            {
                int idTipoViagem = Convert.ToInt32(row["idTipoViagem"]);

                Lista_Veiculos_Movimentacao.Add(new cls_Veiculos
                {
                    idRegistro = Convert.ToInt32(row["idRegistro"]),
                    idMovimentacao = Convert.ToInt32(row["idMovimentacao"]),
                    idTipoViagem = idTipoViagem,
                    idCentroCusto = Convert.ToInt32(row["idCentroCusto"]),
                    sDscTipoMovimentacao = Lista_Tipos_Movimentacao.First(t => t.idTipo == idTipoViagem).sDscTipo,
                    nKilometros = Convert.ToInt32(row["nKilometros"]),
                    idSituacao_Tanque = Convert.ToInt32(row["idSituacao_Tanque"]),
                    idUsuario = Convert.ToInt32(row["idUsuario"]),
                    sDscUsuario = row["sDscUsuario"].ToString(),
                    nLatitude = Convert.ToDecimal(row["nLatitude"]),
                    nLongitude = Convert.ToDecimal(row["nLongitude"]),
                    dtMovimentacao = Convert.ToDateTime(row["dtMovimentacao"]),
                    sObservacao = row["sObservacao"].ToString().Trim(),

                    sPneusDianteiros = (CheckList_Movimentacao)row["sSituacao_PneusDianteiros"].ToString()[0],
                    sPneusTraseiros = (CheckList_Movimentacao)row["sSituacao_PneusTraseiros"].ToString()[0],
                    sRodasDiateiras = (CheckList_Movimentacao)row["sSituacao_RodasDiateiras"].ToString()[0],
                    sRodasTraseiras = (CheckList_Movimentacao)row["sSituacao_RodasTraseiras"].ToString()[0],
                    sBancos = (CheckList_Movimentacao)row["sSituacao_Bancos"].ToString()[0],
                    sPainel = (CheckList_Movimentacao)row["sSituacao_Painel"].ToString()[0],
                    sConsoles = (CheckList_Movimentacao)row["sSituacao_Consoles"].ToString()[0],
                    sForro = (CheckList_Movimentacao)row["sSituacao_Forro"].ToString()[0],
                    sTapetes = (CheckList_Movimentacao)row["sSituacao_Tapetes"].ToString()[0],
                    sCalotas = (CheckList_Movimentacao)row["sSituacao_Calotas"].ToString()[0],
                    sRetrovisores = (CheckList_Movimentacao)row["sSituacao_Retrovisores"].ToString()[0],
                    sPalhetas = (CheckList_Movimentacao)row["sSituacao_Palhetas"].ToString()[0],
                    sTriangulo = (CheckList_Movimentacao)row["sSituacao_Triangulo"].ToString()[0],
                    sMacaco = (CheckList_Movimentacao)row["sSituacao_Macaco"].ToString()[0],
                    sEstepe = (CheckList_Movimentacao)row["sSituacao_Estepe"].ToString()[0],
                    sBateria = (CheckList_Movimentacao)row["sSituacao_Bateria"].ToString()[0],
                    sChaves = (CheckList_Movimentacao)row["sSituacao_Chaves"].ToString()[0],
                    sDocumentos = (CheckList_Movimentacao)row["sSituacao_Documentos"].ToString()[0],
                    sSom = (CheckList_Movimentacao)row["sSituacao_Som"].ToString()[0],
                    sCaixaSelada = (CheckList_Movimentacao)row["sSituacao_CaixaSelada"].ToString()[0],

                    vbArquivo_Painel = row["vbArquivo_Painel"].ToString(),
                    vbArquivo_Frente = row["vbArquivo_Frente"].ToString(),
                    vbArquivo_Traseira = row["vbArquivo_Traseira"].ToString(),
                    vbArquivo_Lat_Direita = row["vbArquivo_Lat_Direita"].ToString(),
                    vbArquivo_Lat_Esquerda = row["vbArquivo_Lat_Esquerda"].ToString()
                });
            }

            rptViagens_dataBind();
        }

        private void rptViagens_dataBind()
        {
            rptViagens.DataSource = Lista_Veiculos_Movimentacao;
            rptViagens.DataBind();

            div_rptViagens.Visible = Lista_Veiculos_Movimentacao.Any();

            ddlTipoMovimentacao_IncluirItem.Items.Clear();
            if (Lista_Veiculos_Movimentacao.Any(v => v.idTipoViagem == 5)) div_IncluirItens.Visible = false;
            else if (Lista_Veiculos_Movimentacao.Any())
                ddlTipoMovimentacao_IncluirItem.Items.AddRange(Lista_Tipos_Movimentacao.Where(t => t.bRepete || !Lista_Veiculos_Movimentacao.Any(i => i.idTipoViagem == t.idTipo)).Select(t => new ListItem(t.sDscTipo, t.idTipo.ToString())).ToArray());
            else ddlTipoMovimentacao_IncluirItem.Items.Add(new ListItem(Lista_Tipos_Movimentacao[0].sDscTipo, Lista_Tipos_Movimentacao[0].idTipo.ToString()));
        }

        #endregion

        #region | Salvar

        private void SalvarDados()
        {
            if (ValidarDados())
            {
                bool bAlugado = ddlVeiculo.SelectedValue == "-1";
                string idMovimentacao = hddidMovimentacao.Value;

                Dictionary<string, string> vParam = new Dictionary<string, string>
                {
                    { "@sFuncao", "SALVAR_MOVIMENTACAO" },
                    { "@idMovimentacao", idMovimentacao },
                    { "@idVeiculo", ddlVeiculo.SelectedValue },
                    { "@sMarca_Movimentacao", !bAlugado ? null : txtMarca.Text },
                    { "@sModelo_Movimentacao", !bAlugado ? null : txtModelo.Text },
                    { "@sPlaca_Movimentacao", !bAlugado ? null : txtPlaca.Text },
                    { "@sAnoModelo_Movimentacao", !bAlugado ? null : txtAno.Text },
                    { "@sCor_Movimentacao", !bAlugado ? null : txtCor.Text },
                    { "@sSeguro_Movimentacao", !bAlugado ? null : swSeguro.Checked ? "S" : "N" },
                    { "@sRastreador_Movimentacao", !bAlugado ? null : swRastreador.Checked ? "S" : "M" },
                    { "@sLocadora_Movimentacao", ! bAlugado ? null : txtLocadora.Text },
                    { "@dtRetirada_Movimentacao", !bAlugado ? null : txtRetirada.Text },
                    { "@dtDevolucao_Movimentacao", !bAlugado ? null : txtDevolucao.Text },
                    { "@sRodizio_Movimentacao", !bAlugado ? null : txtRodizio.Text },
                    { "@idUsuario", Variaveis.idUsuario() }
                };
                DataSet ds = ExecutarDataSet(sProcedure, vParam);

                if (ValidarDataSet(ds, out string sErro))
                {
                    idMovimentacao = RETORNO.DATASET(ds, "idMovimentacao");

                    SalvarItens(idMovimentacao);
                    Pesquisar(idMovimentacao);

                    MensagemPagina.MostraMensagem_Sucesso("Registro de Viagem salvo com sucesso!");
                }
                else MensagemPagina.MostraMensagem_Erro("Erro ao Salvar a Viagem!<br /> Erro: " + sErro);
            }
        }

        private void SalvarItens(string idMovimentacao)
        {
            try
            {
                foreach (var item in Lista_Veiculos_Movimentacao.Where(v => v.idRegistro < 0))
                {
                    Dictionary<string, string> vParam = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR_MOVIMENTACAO_COMPOSICAO" },
                        { "@idMovimentacao", idMovimentacao },
                        { "@idRegistro", item.idRegistro.ToString() },
                        { "@idTipoViagem", item.idTipoViagem.ToString() },
                        { "@idCentroCusto_Movimentacao", item.idCentroCusto.ToString() },
                        { "@sObservacao", item.sObservacao },
                        { "@nKilometros", item.nKilometros.ToString() },
                        { "@idSituacao_Tanque", item.idSituacao_Tanque.ToString() },
                        { "@nLatitude", item.nLatitude.ToString() },
                        { "@nLongitude", item.nLongitude.ToString() },
                        { "@idUsuario", Variaveis.idUsuario() },
                        { "@sPneusDianteiros", ((char)item.sPneusDianteiros).ToString() },
                        { "@sPneusTraseiros", ((char)item.sPneusTraseiros).ToString() },
                        { "@sRodasDiateiras", ((char)item.sRodasDiateiras).ToString() },
                        { "@sRodasTraseiras", ((char)item.sRodasTraseiras).ToString() },
                        { "@sBancos", ((char)item.sBancos).ToString() },
                        { "@sPainel", ((char)item.sPainel).ToString() },
                        { "@sConsoles", ((char)item.sConsoles).ToString() },
                        { "@sForro", ((char)item.sForro).ToString() },
                        { "@sTapetes", ((char)item.sTapetes).ToString() },
                        { "@sCalotas", ((char)item.sCalotas).ToString() },
                        { "@sRetrovisores", ((char)item.sRetrovisores).ToString() },
                        { "@sPalhetas", ((char)item.sPalhetas).ToString() },
                        { "@sTriangulo", ((char)item.sTriangulo).ToString() },
                        { "@sMacaco", ((char)item.sMacaco).ToString() },
                        { "@sEstepe", ((char)item.sEstepe).ToString() },
                        { "@sBateria", ((char)item.sBateria).ToString() },
                        { "@sChaves", ((char)item.sChaves).ToString() },
                        { "@sDocumentos", ((char)item.sDocumentos).ToString() },
                        { "@sSom", ((char)item.sSom).ToString() },
                        { "@sCaixaSelada", ((char)item.sCaixaSelada).ToString() },
                        { "@vbArquivo_Painel", Convert.ToString(item.vbArquivo_Painel) },
                        { "@vbArquivo_Frente", Convert.ToString(item.vbArquivo_Frente) },
                        { "@vbArquivo_Traseira", Convert.ToString(item.vbArquivo_Traseira) },
                        { "@vbArquivo_Lat_Direita", Convert.ToString(item.vbArquivo_Lat_Direita) },
                        { "@vbArquivo_Lat_Esquerda", Convert.ToString(item.vbArquivo_Lat_Esquerda) }
                    };
                    ExecutarDataSet(sProcedure, vParam);
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao salvar a Composição da Movimentação!<br /> Erro: " + ex.Message);
            }
        }

        #endregion

        #region | Utils

        void PopulaCombos()
        {
            Popula_Combo(ddlCentroCusto_IncluirItem, "sp_Select 'Flow_Adm_CentroDeCusto'", "idCentroDeCusto", "sDescricao", false, "Selecione o Centro de Custo", "0");

            Popula_Combo(ddlVeiculo_Filtro, "sp_Select 'Flow_Veiculos_Frota'", "idVeiculo", "sFrotaCompleto", false, "Selecione o Veículo", "0");
            ddlVeiculo_Filtro.Items.Insert(1, new ListItem { Value = "-1", Text = "Veículo Alugado" });
            ddlVeiculo.Items.AddRange(ddlVeiculo_Filtro.Items.Cast<ListItem>().ToArray());
        }

        void LimpaCampos()
        {
            Lista_Veiculos_Movimentacao.Clear();

            txtID.Text = "0";
            ddlVeiculo.SelectedValue = "0";
            ddlVeiculo_Filtro.SelectedValue = "0";
        }

        void LimpaCampos_Modal()
        {
            ddlCentroCusto_IncluirItem.SelectedIndex = 0;
            ddlTipoMovimentacao_IncluirItem.SelectedIndex = 0;
            txtKilometros_IncluirItem.Text = "";
            rnTanque_IncluirItem.Value = "0";
            txtObservacao_IncluirItem.Text = "";

            lblTitulo_Modal_IncluirItem.InnerText = "Nova Viagem";

            cmdIncluirItem.Visible = true;

            MarcaSituacao_Checklist('B', rbPneusDianteiros_Novo, rbPneusDianteiros_Bom, rbPneusDianteiros_Ruim);
            MarcaSituacao_Checklist('B', rbPneusTraseiros_Novo, rbPneusTraseiros_Bom, rbPneusTraseiros_Ruim);
            MarcaSituacao_Checklist('B', rbRodasDianteiras_Novo, rbRodasDianteiras_Bom, rbRodasDianteiras_Ruim);
            MarcaSituacao_Checklist('B', rbRodasTraseiras_Novo, rbRodasTraseiras_Bom, rbRodasTraseiras_Ruim);
            MarcaSituacao_Checklist('B', rbBancos_NaoPossui, rbBancos_Bom, rbBancos_Ruim);
            MarcaSituacao_Checklist('B', rbPainel_NaoPossui, rbPainel_Bom, rbPainel_Ruim);
            MarcaSituacao_Checklist('B', rbConsoles_NaoPossui, rbConsoles_Bom, rbConsoles_Ruim);
            MarcaSituacao_Checklist('B', rbForro_NaoPossui, rbForro_Bom, rbForro_Ruim);
            MarcaSituacao_Checklist('B', rbTapetes_NaoPossui, rbTapetes_Bom, rbTapetes_Ruim);
            MarcaSituacao_Checklist('B', rbCalotas_NaoPossui, rbCalotas_Bom, rbCalotas_Ruim);
            MarcaSituacao_Checklist('B', rbRetrovisores_NaoPossui, rbRetrovisores_Bom, rbRetrovisores_Ruim);
            MarcaSituacao_Checklist('B', rbPalhetas_NaoPossui, rbPalhetas_Bom, rbPalhetas_Ruim);
            MarcaSituacao_Checklist('B', rbTriangulo_NaoPossui, rbTriangulo_Bom, rbTriangulo_Ruim);
            MarcaSituacao_Checklist('B', rbMacaco_NaoPossui, rbMacaco_Bom, rbMacaco_Ruim);
            MarcaSituacao_Checklist('B', rbEstepe_NaoPossui, rbEstepe_Bom, rbEstepe_Ruim);
            MarcaSituacao_Checklist('B', rbBateria_NaoPossui, rbBateria_Bom, rbBateria_Ruim);
            MarcaSituacao_Checklist('B', rbChaves_NaoPossui, rbChaves_Bom, rbChaves_Ruim);
            MarcaSituacao_Checklist('B', rbDocumentos_NaoPossui, rbDocumentos_Bom, rbDocumentos_Ruim);
            MarcaSituacao_Checklist('B', rbSom_NaoPossui, rbSom_Bom, rbSom_Ruim);
            MarcaSituacao_Checklist('B', rbCaixaSelada_NaoPossui, rbCaixaSelada_Bom, rbCaixaSelada_Ruim);
        }

        bool ValidarDados()
        {
            if (ddlVeiculo.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("É necessário selecionar um Veículo!");
                Scripts.FocusScript(Page, ddlVeiculo.ClientID);
                return false;
            }
            else if (ddlVeiculo.SelectedValue == "-1")
            {
                string sErro_VeiculoAlugado = "Em caso de veículo alugado,";

                if (string.IsNullOrEmpty(txtMarca.Text))
                {
                    MensagemPagina.MostraMensagem_Erro($"{sErro_VeiculoAlugado} é necessário preencher a Marca!");
                    Scripts.FocusScript(Page, txtMarca.ClientID);
                    return false;
                }
                else if (string.IsNullOrEmpty(txtModelo.Text))
                {
                    MensagemPagina.MostraMensagem_Erro($"{sErro_VeiculoAlugado} é necessário preencher o Modelo!");
                    Scripts.FocusScript(Page, txtModelo.ClientID);
                    return false;
                }
                else if (string.IsNullOrEmpty(txtPlaca.Text))
                {
                    MensagemPagina.MostraMensagem_Erro($"{sErro_VeiculoAlugado} é necessário preencher a Placa!");
                    Scripts.FocusScript(Page, txtPlaca.ClientID);
                    return false;
                }
                else if (string.IsNullOrEmpty(txtAno.Text))
                {
                    MensagemPagina.MostraMensagem_Erro($"{sErro_VeiculoAlugado} é necessário preencher o Ano!");
                    Scripts.FocusScript(Page, txtAno.ClientID);
                    return false;
                }
                else if (string.IsNullOrEmpty(txtCor.Text))
                {
                    MensagemPagina.MostraMensagem_Erro($"{sErro_VeiculoAlugado} é necessário preencher a Cor!");
                    Scripts.FocusScript(Page, txtCor.ClientID);
                    return false;
                }
                else if (string.IsNullOrEmpty(txtLocadora.Text))
                {
                    MensagemPagina.MostraMensagem_Erro($"{sErro_VeiculoAlugado} é necessário preencher a Locadora!");
                    Scripts.FocusScript(Page, txtLocadora.ClientID);
                    return false;
                }
                else if (string.IsNullOrEmpty(txtRetirada.Text))
                {
                    MensagemPagina.MostraMensagem_Erro($"{sErro_VeiculoAlugado} é necessário preencher a Data da Retirada!");
                    Scripts.FocusScript(Page, txtRetirada.ClientID);
                    return false;
                }
                else if (string.IsNullOrEmpty(txtDevolucao.Text))
                {
                    MensagemPagina.MostraMensagem_Erro($"{sErro_VeiculoAlugado} é necessário preencher a Data da Devolução!");
                    Scripts.FocusScript(Page, txtDevolucao.ClientID);
                    return false;
                }
                else if (string.IsNullOrEmpty(txtRodizio.Text))
                {
                    MensagemPagina.MostraMensagem_Erro($"{sErro_VeiculoAlugado} é necessário preencher o Rodízio!");
                    Scripts.FocusScript(Page, txtRodizio.ClientID);
                    return false;
                }
            }

            if (!Lista_Veiculos_Movimentacao.Any())
            {
                MensagemPagina.MostraMensagem_Erro("É necessário adicionar ao menos um Registro de Viagem!");
                Scripts.FocusScript(Page, "cmdIncluirItem_modal");
                return false;
            }

            return true;
        }

        void BloqueiaCampos_Modal(bool bBloqueia)
        {
            string sBloqueia = bBloqueia ? "disabled" : "enabled";

            ddlTipoMovimentacao_IncluirItem.Attributes.Remove("disabled");

            ddlTipoMovimentacao_IncluirItem.Attributes.Add(sBloqueia, sBloqueia);
            txtKilometros_IncluirItem.ReadOnly = bBloqueia;
            rnTanque_IncluirItem.Disabled = bBloqueia;
            txtObservacao_IncluirItem.ReadOnly = bBloqueia;

            // Checklist
            {
                rbPneusDianteiros_Novo.Enabled = !bBloqueia;
                rbPneusDianteiros_Bom.Enabled = !bBloqueia;
                rbPneusDianteiros_Ruim.Enabled = !bBloqueia;

                rbPneusTraseiros_Novo.Enabled = !bBloqueia;
                rbPneusTraseiros_Bom.Enabled = !bBloqueia;
                rbPneusTraseiros_Ruim.Enabled = !bBloqueia;

                rbRodasDianteiras_Novo.Enabled = !bBloqueia;
                rbRodasDianteiras_Bom.Enabled = !bBloqueia;
                rbRodasDianteiras_Ruim.Enabled = !bBloqueia;

                rbRodasTraseiras_Novo.Enabled = !bBloqueia;
                rbRodasTraseiras_Bom.Enabled = !bBloqueia;
                rbRodasTraseiras_Ruim.Enabled = !bBloqueia;

                rbBancos_NaoPossui.Enabled = !bBloqueia;
                rbBancos_Bom.Enabled = !bBloqueia;
                rbBancos_Ruim.Enabled = !bBloqueia;

                rbPainel_NaoPossui.Enabled = !bBloqueia;
                rbPainel_Bom.Enabled = !bBloqueia;
                rbPainel_Ruim.Enabled = !bBloqueia;

                rbConsoles_NaoPossui.Enabled = !bBloqueia;
                rbConsoles_Bom.Enabled = !bBloqueia;
                rbConsoles_Ruim.Enabled = !bBloqueia;

                rbForro_NaoPossui.Enabled = !bBloqueia;
                rbForro_Bom.Enabled = !bBloqueia;
                rbForro_Ruim.Enabled = !bBloqueia;

                rbTapetes_NaoPossui.Enabled = !bBloqueia;
                rbTapetes_Bom.Enabled = !bBloqueia;
                rbTapetes_Ruim.Enabled = !bBloqueia;

                rbCalotas_NaoPossui.Enabled = !bBloqueia;
                rbCalotas_Bom.Enabled = !bBloqueia;
                rbCalotas_Ruim.Enabled = !bBloqueia;

                rbRetrovisores_NaoPossui.Enabled = !bBloqueia;
                rbRetrovisores_Bom.Enabled = !bBloqueia;
                rbRetrovisores_Ruim.Enabled = !bBloqueia;

                rbPalhetas_NaoPossui.Enabled = !bBloqueia;
                rbPalhetas_Bom.Enabled = !bBloqueia;
                rbPalhetas_Ruim.Enabled = !bBloqueia;

                rbTriangulo_NaoPossui.Enabled = !bBloqueia;
                rbTriangulo_Bom.Enabled = !bBloqueia;
                rbTriangulo_Ruim.Enabled = !bBloqueia;

                rbMacaco_NaoPossui.Enabled = !bBloqueia;
                rbMacaco_Bom.Enabled = !bBloqueia;
                rbMacaco_Ruim.Enabled = !bBloqueia;

                rbEstepe_NaoPossui.Enabled = !bBloqueia;
                rbEstepe_Bom.Enabled = !bBloqueia;
                rbEstepe_Ruim.Enabled = !bBloqueia;

                rbBateria_NaoPossui.Enabled = !bBloqueia;
                rbBateria_Bom.Enabled = !bBloqueia;
                rbBateria_Ruim.Enabled = !bBloqueia;

                rbChaves_NaoPossui.Enabled = !bBloqueia;
                rbChaves_Bom.Enabled = !bBloqueia;
                rbChaves_Ruim.Enabled = !bBloqueia;

                rbDocumentos_NaoPossui.Enabled = !bBloqueia;
                rbDocumentos_Bom.Enabled = !bBloqueia;
                rbDocumentos_Ruim.Enabled = !bBloqueia;

                rbSom_NaoPossui.Enabled = !bBloqueia;
                rbSom_Bom.Enabled = !bBloqueia;
                rbSom_Ruim.Enabled = !bBloqueia;

                rbCaixaSelada_NaoPossui.Enabled = !bBloqueia;
                rbCaixaSelada_Bom.Enabled = !bBloqueia;
                rbCaixaSelada_Ruim.Enabled = !bBloqueia;
            }
        }

        CheckList_Movimentacao RetornaSituacao_Checklist(RadioButton rb1, RadioButton rb2, RadioButton rb3)
        {
            return rb1.Checked ? (CheckList_Movimentacao)rb1.Attributes["data-situacao"][0] :
                    rb2.Checked ? (CheckList_Movimentacao)rb2.Attributes["data-situacao"][0] :
                    rb3.Checked ? (CheckList_Movimentacao)rb3.Attributes["data-situacao"][0] : CheckList_Movimentacao.NaoPossui;
        }

        void MarcaSituacao_Checklist(char situacao, RadioButton rb1, RadioButton rb2, RadioButton rb3)
        {
            rb1.Checked = situacao == rb1.Attributes["data-situacao"][0];
            rb2.Checked = situacao == rb2.Attributes["data-situacao"][0];
            rb3.Checked = situacao == rb3.Attributes["data-situacao"][0];
        }

        int RetornaNovoRegistro()
        {
            if (Lista_Veiculos_Movimentacao.Any(v => v.idRegistro < 0)) return Lista_Veiculos_Movimentacao.Min(v => v.idRegistro) - 1;
            else return -1;
        }

        protected void IncluirItem()
        {
            try
            {
                var tipo = Lista_Tipos_Movimentacao.First(t => t.idTipo.ToString() == ddlTipoMovimentacao_IncluirItem.SelectedValue);

                var item = new cls_Veiculos
                {
                    idRegistro = RetornaNovoRegistro(),
                    idMovimentacao = Convert.ToInt32(hddidMovimentacao.Value),
                    idTipoViagem = tipo.idTipo,
                    sDscTipoMovimentacao = tipo.sDscTipo,
                    nKilometros = Convert.ToInt32(txtKilometros_IncluirItem.Text.Replace(".", "")),
                    idSituacao_Tanque = Convert.ToInt32(rnTanque_IncluirItem.Value),
                    nLatitude = Convert.ToDecimal(hddLatitude.Value),
                    nLongitude = Convert.ToDecimal(hddLongitude.Value),
                    idUsuario = Convert.ToInt32(Variaveis.idUsuario()),
                    sDscUsuario = Variaveis.sUsuarioLogado(),
                    dtMovimentacao = DateTime.Now,
                    sObservacao = txtObservacao_IncluirItem.Text,

                    vbArquivo_Painel = Convert.ToBase64String(fuAdicionarArquivo_FotoPainel.FileBytes)
                };

                if (tipo.bChecklist_Completo)
                {
                    item.sPneusDianteiros = RetornaSituacao_Checklist(rbPneusDianteiros_Novo, rbPneusDianteiros_Bom, rbPneusDianteiros_Ruim);
                    item.sPneusTraseiros = RetornaSituacao_Checklist(rbPneusTraseiros_Novo, rbPneusTraseiros_Bom, rbPneusTraseiros_Ruim);
                    item.sRodasDiateiras = RetornaSituacao_Checklist(rbRodasDianteiras_Novo, rbRodasDianteiras_Bom, rbRodasDianteiras_Ruim);
                    item.sRodasTraseiras = RetornaSituacao_Checklist(rbRodasTraseiras_Novo, rbRodasTraseiras_Bom, rbRodasTraseiras_Ruim);
                    item.sBancos = RetornaSituacao_Checklist(rbBancos_NaoPossui, rbBancos_Bom, rbBancos_Ruim);
                    item.sPainel = RetornaSituacao_Checklist(rbPainel_NaoPossui, rbPainel_Bom, rbPainel_Ruim);
                    item.sConsoles = RetornaSituacao_Checklist(rbConsoles_NaoPossui, rbConsoles_Bom, rbConsoles_Ruim);
                    item.sForro = RetornaSituacao_Checklist(rbForro_NaoPossui, rbForro_Bom, rbForro_Ruim);
                    item.sTapetes = RetornaSituacao_Checklist(rbTapetes_NaoPossui, rbTapetes_Bom, rbTapetes_Ruim);
                    item.sCalotas = RetornaSituacao_Checklist(rbCalotas_NaoPossui, rbCalotas_Bom, rbCalotas_Ruim);
                    item.sRetrovisores = RetornaSituacao_Checklist(rbRetrovisores_NaoPossui, rbRetrovisores_Bom, rbRetrovisores_Ruim);
                    item.sPalhetas = RetornaSituacao_Checklist(rbPalhetas_NaoPossui, rbPalhetas_Bom, rbPalhetas_Ruim);
                    item.sTriangulo = RetornaSituacao_Checklist(rbTriangulo_NaoPossui, rbTriangulo_Bom, rbTriangulo_Ruim);
                    item.sMacaco = RetornaSituacao_Checklist(rbMacaco_NaoPossui, rbMacaco_Bom, rbMacaco_Ruim);
                    item.sEstepe = RetornaSituacao_Checklist(rbEstepe_NaoPossui, rbEstepe_Bom, rbEstepe_Ruim);
                    item.sBateria = RetornaSituacao_Checklist(rbBateria_NaoPossui, rbBateria_Bom, rbBateria_Ruim);
                    item.sChaves = RetornaSituacao_Checklist(rbChaves_NaoPossui, rbChaves_Bom, rbChaves_Ruim);
                    item.sDocumentos = RetornaSituacao_Checklist(rbDocumentos_NaoPossui, rbDocumentos_Bom, rbDocumentos_Ruim);
                    item.sSom = RetornaSituacao_Checklist(rbSom_NaoPossui, rbSom_Bom, rbSom_Ruim);
                    item.sCaixaSelada = RetornaSituacao_Checklist(rbCaixaSelada_NaoPossui, rbCaixaSelada_Bom, rbCaixaSelada_Ruim);

                    item.vbArquivo_Frente = Convert.ToBase64String(fuAdicionarArquivo_FotoFrente.FileBytes);
                    item.vbArquivo_Traseira = Convert.ToBase64String(fuAdicionarArquivo_FotoTraseira.FileBytes);
                    item.vbArquivo_Lat_Direita = Convert.ToBase64String(fuAdicionarArquivo_FotoLatDireita.FileBytes);
                    item.vbArquivo_Lat_Esquerda = Convert.ToBase64String(fuAdicionarArquivo_FotoLatEsquerda.FileBytes);
                }

                Lista_Veiculos_Movimentacao.Add(item);

                rptViagens_dataBind();

                LimpaCampos_Modal();
                BloqueiaCampos_Modal(false);

                MensagemPagina_Itens.MostraMensagem_Sucesso("Viagem incluída com sucesso!");
                Scripts.RemoverBackdrop_Modal(Page);
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
                Scripts.RemoverBackdrop_Modal(Page);
            }
        }

        public bool ValidaStatus_CheckList(string sidRegistro, char status, out string itens)
        {
            itens = "";

            int.TryParse(sidRegistro, out int idRegistro);
            var viagem = Lista_Veiculos_Movimentacao.FirstOrDefault(m => m.idRegistro == idRegistro);

            if (viagem != null)
            {
                if (((char)viagem.sPneusDianteiros).Equals(status)) itens += "<li>Pneus Dianteiros</li>";
                if (((char)viagem.sPneusTraseiros).Equals(status)) itens += "<li>Pneus Traseiros</li>";
                if (((char)viagem.sRodasDiateiras).Equals(status)) itens += "<li>Rodas Diateiras</li>";
                if (((char)viagem.sRodasTraseiras).Equals(status)) itens += "<li>Rodas Traseiras</li>";
                if (((char)viagem.sBancos).Equals(status)) itens += "<li>Bancos</li>";
                if (((char)viagem.sPainel).Equals(status)) itens += "<li>Painel</li>";
                if (((char)viagem.sConsoles).Equals(status)) itens += "<li>Consoles</li>";
                if (((char)viagem.sForro).Equals(status)) itens += "<li>Forro</li>";
                if (((char)viagem.sTapetes).Equals(status)) itens += "<li>Tapetes</li>";
                if (((char)viagem.sCalotas).Equals(status)) itens += "<li>Calotas</li>";
                if (((char)viagem.sRetrovisores).Equals(status)) itens += "<li>Retrovisores</li>";
                if (((char)viagem.sPalhetas).Equals(status)) itens += "<li>Palhetas</li>";
                if (((char)viagem.sTriangulo).Equals(status)) itens += "<li>Triangulo</li>";
                if (((char)viagem.sMacaco).Equals(status)) itens += "<li>Macaco</li>";
                if (((char)viagem.sEstepe).Equals(status)) itens += "<li>Estepe</li>";
                if (((char)viagem.sBateria).Equals(status)) itens += "<li>Bateria</li>";
                if (((char)viagem.sChaves).Equals(status)) itens += "<li>Chaves</li>";
                if (((char)viagem.sDocumentos).Equals(status)) itens += "<li>Documentos</li>";
                if (((char)viagem.sSom).Equals(status)) itens += "<li>Som</li>";
                if (((char)viagem.sCaixaSelada).Equals(status)) itens += "<li>Caixa Selada</li>";

                if (!string.IsNullOrEmpty(itens)) return true;
            }

            return false;
        }

        #endregion

        #region | Eventos

        protected void rptConsulta_ItemCommand(object source, RepeaterCommandEventArgs e) => Pesquisar(e.CommandArgument.ToString());

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void cmdVoltar_Click(object sender, EventArgs e) => Pesquisar();

        protected void cmdNovo_Click(object source, EventArgs e) => Pesquisar("0");

        protected void cmdSalvar_Click(object source, EventArgs e) => SalvarDados();

        protected void ddlVeiculo_SelectedIndexChanged(object sender, EventArgs e)
        {
            div_VeiculoAlugado.Visible = false;

            if (ddlVeiculo.SelectedValue == "0") MensagemPagina.MostraMensagem_Erro("É necessário selecionar um Veículo!");
            else if (ddlVeiculo.SelectedValue == "-1") div_VeiculoAlugado.Visible = true;
        }

        #endregion

        #region | Script

        void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("$(document).ready(function() {");
            sb.AppendLine("");
            sb.AppendLine($"    var cmdProximo = $('#{cmdProximo.ClientID}');");
            sb.AppendLine($"    var cmdAnterior = $('#{cmdAnterior.ClientID}');");
            sb.AppendLine($"    var cmdIncluirItem = $('#{cmdIncluirItem.ClientID}');");
            sb.AppendLine("");
            sb.AppendLine($"    $('#{cmdProximo.ClientID}, #{cmdAnterior.ClientID}').on('click', function(e) {{");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine($"        let etapaAtual = parseInt($('#{hddEtapaAtual.ClientID}').val());");
            sb.AppendLine("");
            sb.AppendLine("         if (!ValidaDados_Modal(etapaAtual)) return false;");
            sb.AppendLine("");
            sb.AppendLine("         if (etapaAtual == 1) cmdAnterior.show();");
            sb.AppendLine("         else if (etapaAtual == 4) { cmdProximo.show(); cmdIncluirItem.hide(); }");
            sb.AppendLine("");
            sb.AppendLine("         let lado = 'right';");
            sb.AppendLine($"        const idDestino = $('#{ddlTipoMovimentacao_IncluirItem.ClientID}').val();");
            sb.AppendLine("         if (idDestino == 1 || idDestino == 5) {");
            sb.AppendLine($"            $('#{div_FotoFrente.ClientID}').show();");
            sb.AppendLine($"            $('#{div_FotoTraseira.ClientID}').show();");
            sb.AppendLine($"            $('#{div_FotoLatDireita.ClientID}').show();");
            sb.AppendLine($"            $('#{div_FotoLatEsquerda.ClientID}').show();");
            sb.AppendLine("             if ($(this).attr('id').includes('Proximo')) etapaAtual += 1;");
            sb.AppendLine("             else { etapaAtual -= 1; lado = 'left' }");
            sb.AppendLine("         } else {");
            sb.AppendLine($"            $('#{div_FotoFrente.ClientID}').hide();");
            sb.AppendLine($"            $('#{div_FotoTraseira.ClientID}').hide();");
            sb.AppendLine($"            $('#{div_FotoLatDireita.ClientID}').hide();");
            sb.AppendLine($"            $('#{div_FotoLatEsquerda.ClientID}').hide();");
            sb.AppendLine("             if ($(this).attr('id').includes('Proximo')) etapaAtual = 4;");
            sb.AppendLine("             else { etapaAtual = 1; lado = 'left' }");
            sb.AppendLine("         }");
            sb.AppendLine("");
            sb.AppendLine("         if (etapaAtual == 1) cmdAnterior.hide();");
            sb.AppendLine("         else if (etapaAtual == 4) { cmdProximo.hide(); cmdIncluirItem.show(); }");
            sb.AppendLine("");
            sb.AppendLine("         $('[id*=divEtapa_]').hide();");
            sb.AppendLine("         $(`[id*=divEtapa_${etapaAtual}]`).toggle('slide', { direction: lado }, 500);");
            sb.AppendLine($"        $('#{hddEtapaAtual.ClientID}').val(etapaAtual);");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $('#modal_IncluirItem').on('show.bs.modal', function() {");
            sb.AppendLine("         $('#divEtapa_1').show();");
            sb.AppendLine("         cmdProximo.show();");
            sb.AppendLine("         $('[id*=divAdicionar_Foto]').show();");
            sb.AppendLine("");
            sb.AppendLine("         $('#divEtapa_2').hide();");
            sb.AppendLine("         $('#divEtapa_3').hide();");
            sb.AppendLine("         $('#divEtapa_4').hide();");
            sb.AppendLine("         cmdAnterior.hide();");
            sb.AppendLine("         cmdIncluirItem.hide();");
            sb.AppendLine("         $('[id*=divVisualizar_Foto]').hide();");
            sb.AppendLine("");
            sb.AppendLine($"        $('#{hddEtapaAtual.ClientID}').val('1');");
            sb.AppendLine("         $('.is-invalid').removeClass('is-invalid');");
            sb.AppendLine("         $('.mensagemErro').addClass('invisivel');");
            sb.AppendLine("         $('.msg').text('');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $('#modal_IncluirItem').on('hide.bs.modal', function() {");
            sb.AppendLine($"        $('#{txtKilometros_IncluirItem.ClientID}').removeAttr('required');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $('.checklist td:has(input)').on('click', function() {");
            sb.AppendLine("         $(this).find('input').prop('checked', true).trigger('change');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $('[id*=cmdAlterar_Foto]').on('click', function(e) {");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         $(this).closest('.card').find('input').click();");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $('.card-img-top').on('click', function() {");
            sb.AppendLine("         $(this).closest('.form-group').toggleClass('col-6 col-12');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine($"    $('#{cmdIncluirItem.ClientID}').on('click', function(e) {{");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         $('.mensagemErro').addClass('invisivel');");
            sb.AppendLine("         $('.msg').text('');");
            sb.AppendLine("         if (Valida_Arquivos())  __doPostBack('INCLUIR_ITEM', '_blank');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     $('.todosArquivos input').on('change', function() {");
            sb.AppendLine("         const check = $(this).prop('checked');");
            sb.AppendLine("         $('.arquivo input').prop('checked', check);");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("});");
            sb.AppendLine("");
            sb.AppendLine("function Valida_Arquivos() {");
            sb.AppendLine($"    const idDestino = $('#{ddlTipoMovimentacao_IncluirItem.ClientID}').val();");
            sb.AppendLine($"    const urlPainel = $('#{imgFotoPainel.ClientID}').attr('src');");
            sb.AppendLine("     if (idDestino == 1 || idDestino == 5) {");
            sb.AppendLine($"         if (!$('#{imgFotoFrente.ClientID}').attr('src') || !$('#{imgFotoTraseira.ClientID}').attr('src') || !$('#{imgFotoLatDireita.ClientID}').attr('src') || !$('#{imgFotoLatEsquerda.ClientID}').attr('src')) {{");
            sb.AppendLine("             $('.mensagemErro').removeClass('invisivel');");
            sb.AppendLine("             $('.msg').text('Para o Destino selecionado nesta Viagem, é necessário adicionar todas as Fotos do veículo listadas acima!');");
            sb.AppendLine("             return false;");
            sb.AppendLine("         }");
            sb.AppendLine("     }");
            sb.AppendLine("     if (!urlPainel) { $('.mensagemErro').removeClass('invisivel'); $('.msg').text('É necessário adicionar a Foto do Painel do veículo!'); return false; }");
            sb.AppendLine("     return true;");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("function AdicionaArquivo(fileInput) {");
            sb.AppendLine("     const divAdicionar = $(fileInput.closest('div'));");
            sb.AppendLine("     const divVisualizar = $(divAdicionar.siblings('div'));");
            sb.AppendLine("     divAdicionar.hide();");
            sb.AppendLine("     divVisualizar.show();");
            sb.AppendLine("     const reader = new FileReader();");
            sb.AppendLine("     reader.readAsDataURL(fileInput[0].files[0]);");
            sb.AppendLine("     reader.onload = function (e) { divVisualizar.find('img').attr('src', e.target.result); };");
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("function ValidaDados_Modal(etapaAtual) {");
            sb.AppendLine("     if (etapaAtual != 1) { $('.is-invalid').removeClass('is-invalid'); $('#" + txtKilometros_IncluirItem.ClientID + "').removeAttr('required'); return true; }");
            sb.AppendLine("     else {");
            sb.AppendLine($"        $('#{txtKilometros_IncluirItem.ClientID}').attr('required', 'true');");
            sb.AppendLine($"        $('#{ddlCentroCusto_IncluirItem.ClientID}, #{txtKilometros_IncluirItem.ClientID}').removeClass('is-invalid');");
            sb.AppendLine($"        if ($('#{ddlCentroCusto_IncluirItem.ClientID}').val() == '0') $('#{ddlCentroCusto_IncluirItem.ClientID}').addClass('is-invalid');");
            sb.AppendLine($"        if (!$('#{txtKilometros_IncluirItem.ClientID}').val()) $('#{txtKilometros_IncluirItem.ClientID}').addClass('is-invalid');");
            sb.AppendLine("         if ($('.is-invalid').length > 0) return false;");
            sb.AppendLine("     }");
            sb.AppendLine("     return true;");
            sb.AppendLine("};");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ScriptPagina", sb.ToString(), true);
        }

        #endregion
    }
}