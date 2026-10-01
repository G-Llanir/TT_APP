using System;
using System.Collections.Generic;
using System.Web.UI;
using TT_Flow.FrameWork;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using System.Text;
using System.Data;
using System.Globalization;

namespace TT_Flow.App.Controles
{
    public partial class FiltroPesquisa : UserControl
    {
        #region | Propriedades

        static public string[] _FiltrosAtivos = { "tipoProdutos", "Familia", "Grupo", "PaisOrigem", "Unidade" };

        public string sRegistraUnidade { get; set; }
        public string ddlTipoProdutoValue { get => FT_ddlidTipoProduto.SelectedValue; set => FT_ddlidTipoProduto.SelectedValue = value; }
        public string ddlFamiliaValue { get => FT_ddlidFamilia.SelectedValue; set => FT_ddlidFamilia.SelectedValue = value; }
        public string ddlGrupoValue { get => FT_ddlidGrupo.SelectedValue; set => FT_ddlidGrupo.SelectedValue = value; }
        public string ddlPaisOrigemValue { get => FT_ddlidPaisOrigem.SelectedValue; set => FT_ddlidTipoProduto.SelectedValue = value; }
        public decimal NQuantidade { get => ObterQuantidade(); set => FT_txtnQuantidade.Text = value.ToString(); } // gambiarra para permitir usar o 'set' em Orçamentos
        public string STipoProduto { get => FT_hddComposicao_sDscTipoProduto.Value; }
        public string STipoServico_Recurso { get => FT_hddComposicao_sDscTipoServico_Recurso.Value; }
        public string IDTipoServico_Recurso { get => FT_hddComposicao_idTipoServico_Recurso.Value; }

        public string IdItem { get => FT_hddComposicao_idItem.Value; set => FT_hddComposicao_idItem.Value = value; }
        public string SCodigo { get => FT_txtComposicao_sCodigoProduto.Text; set => FT_txtComposicao_sCodigoProduto.Text = value; }
        public string SDscProduto { get => FT_txtComposicao_sDscProduto.Text; set => FT_txtComposicao_sDscProduto.Text = value; }
        public string SUnidade { get => FT_ddlComposicao_sUnidade.SelectedValue; set => FT_ddlComposicao_sUnidade.SelectedValue = value; }
        public string SUnidadeSigla { get => FT_ddlComposicao_sUnidade.SelectedItem.Text; set => FT_ddlComposicao_sUnidade.SelectedItem.Text = value; }
        public string IdServico_Recurso { get => FT_hddComposicao_idServico_Recurso.Value; set => FT_hddComposicao_idServico_Recurso.Value = value; }
        public string SCodigoServico_Recurso { get => FT_txtComposicao_sCodigoServico_Recurso.Text; set => FT_txtComposicao_sCodigoServico_Recurso.Text = value; }
        public string SDscServico_Recurso { get => FT_txtComposicao_sDscServico_Recurso.Text; set => FT_txtComposicao_sDscServico_Recurso.Text = value; }
        public string SUnidadeServico_Recurso { get => FT_hddComposicao_sUnidadeServico_Recurso.Value; set => FT_hddComposicao_sUnidadeServico_Recurso.Value = value; }
        public string idParceiro_Produto { get => FT_hddidParceiro_Produtos.Value; set => FT_hddidParceiro_Produtos.Value = value; }

        public decimal NValorProduto { get => cls_Comercial_Tabelas.ConverterStringDecimal(FT_txtnValor.Text); }
        public decimal hddnValorProduto { get => decimal.Parse(FT_hddComposicao_nPreco.Value); set => FT_hddComposicao_nPreco.Value = value.ToString(); }
        public decimal NValorServico_Recurso { get => cls_Comercial_Tabelas.ConverterStringDecimal(FT_txtnValorServico_Recurso.Text); }
        public string IDParceiro { get => FT_hddComposicao_idParceiro_Colaborador.Value; set => FT_hddComposicao_idParceiro_Colaborador.Value = value; }
        public string sCNPJ_CPF_Parceiro_Colaborador { get => FT_txtComposicao_sCNPJarceiro_sCPFColaborador.Text; set => FT_txtComposicao_sCNPJarceiro_sCPFColaborador.Text = value; }
        public string sDscParceiro_Colaborador { get => FT_txtComposicao_sDscParceiro_Colaborador.Text; set => FT_txtComposicao_sDscParceiro_Colaborador.Text = value; }
        public string SAtivaPostBack { get; set; }

        public bool ExibirNaAbertura = false;

        /// <summary>
        /// Propriedade responsável por Definir o Tipo do Filtro Pesquisa, onde: 
        /// <br /><br />
        /// Produtos = "1", <br />
        /// Serviços = "2", <br />
        /// Recursos = "3", <br />
        /// Parceiros = "4", <br />
        /// Colaboradores = "5", <br />
        /// Serviços + Sub-Serviços + Recursos = "23". <br />
        /// Orçamento = "Orcamento". <br />
        /// </summary>
        public string TipoFiltroPesquisa { get => hddTipoFiltro.Value; set => hddTipoFiltro.Value = value; }

        /// <summary>
        /// Propriedade responsável por Definir o ClientID de um campo da página, que não faz parte dos campos deste Controle (FiltroPesquisa), desta forma sendo possível aplicar um '.focus()' em um campo de fora do Controle após o Script do Controle.
        /// <br /> Para utilizar esta propriedade, basta definir aqui o ClientID do campo desejado.
        /// </summary>
        public string ClientID_FocusPersonalizado = "";

        /// <summary>
        /// Propriedade responsável por Definir o Território da Empresa, desta forma definindo a Moeda a ser buscada, onde:
        /// <br /><br />
        /// 0 = Não Faz Busca <br />
        /// 1 = Dólar Americano (US$) <br />
        /// 2 = Brasil (R$) <br />
        /// </summary>
        public int TerritorioEmpresa = -1;

        public string idTabelaParceiro = "0";

        #endregion

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    PopularCombos(new string[] { "tipoProdutos", "Familia", "Grupo", "PaisOrigem", "Unidade" }, "Produto_x_Tipo");

                    if (TipoFiltroPesquisa == "0" || TipoFiltroPesquisa == "1" || TipoFiltroPesquisa == "Orcamento")
                    {
                        div_FiltroProdutos.Visible = true;

                        updpItensProduto.Visible = true;
                        UpdServico_Recurso.Visible = false;
                        UpdParceiro_Colaborador.Visible = false;
                    }
                    else if (TipoFiltroPesquisa == "2" || TipoFiltroPesquisa == "3" || TipoFiltroPesquisa == "23")
                    {
                        div_FiltroProdutos.Visible = false;

                        updpItensProduto.Visible = false;
                        UpdServico_Recurso.Visible = true;
                        UpdParceiro_Colaborador.Visible = false;
                    }
                    if (TipoFiltroPesquisa == "4" || TipoFiltroPesquisa == "5")
                    {
                        div_FiltroProdutos.Visible = false;

                        updpItensProduto.Visible = false;
                        UpdServico_Recurso.Visible = false;
                        UpdParceiro_Colaborador.Visible = true;
                    }
                    if (TipoFiltroPesquisa == "6")
                    {
                        div_FiltroProdutos.Visible = false;

                        updpItensProduto.Visible = true;
                        UpdServico_Recurso.Visible = false;
                        UpdParceiro_Colaborador.Visible = false;
                    }
                }
                catch
                {
                    return;
                }
            }
            RegistrarScriptPesquisar();
        }

        public void PopularCombos(string[] FiltrosAtivos, string tipo)
        {
            foreach (string str in FiltrosAtivos)
            {
                if (str == "tipoProdutos") FUNCOES.Popula_Combo(FT_ddlidTipoProduto, "sp_Select 'Flow_Produtos_Tipo', @sPesquisa = 'N'", "idTipoProduto", "sDscTipoProduto", false, "Todos os Tipos", "0");
                else if (str == "Familia") FUNCOES.Popula_Combo(FT_ddlidFamilia, string.Format("sp_Select 'Flow_WMS_Produtos_Familia', @sPesquisa = {0}", string.IsNullOrEmpty(FT_hddComposicao_idParceiro_Colaborador.Value) ? FT_hddComposicao_idParceiro_Colaborador.Value = "0" : FT_hddComposicao_idParceiro_Colaborador.Value), "idFamilia", "sDscFamilia", false, "Todas as Famílias", "0");
                else if (str == "Grupo") FUNCOES.Popula_Combo(FT_ddlidGrupo, "sp_Select 'Flow_WMS_Produtos_Grupos_PAI'", "idGrupo", "sDscGrupo", false, "Todos os Grupos", "0");
                else if (str == "PaisOrigem") FUNCOES.Popula_Combo(FT_ddlidPaisOrigem, "sp_Select 'tbl_Flow_WMS_Produtos_Origem'", "idPais", "sDscPais", false, "Todos Países de Origem", "0");
                else if (str == "Unidade") FUNCOES.Popula_Combo(FT_ddlComposicao_sUnidade, "sp_Select 'Flow_Produtos_Unidade', @sPesquisa = 'N'", "sUnidade", "sDscUnidade", false, "Selecione ", "");
            }
        }

        #endregion

        #region | Utils

        public void FocusPersonalizado() => FUNCOES.Scripts.FocusScript(Page, ClientID_FocusPersonalizado);

        public void Focus_sCodigo()
        {
            FUNCOES.Scripts.FocusScript(Page, TipoFiltroPesquisa == "0" || TipoFiltroPesquisa == "1" || TipoFiltroPesquisa == "Orcamento" || TipoFiltroPesquisa == "6" ? FT_txtComposicao_sCodigoProduto.ClientID
                                                                                            : TipoFiltroPesquisa == "2" || TipoFiltroPesquisa == "3" || TipoFiltroPesquisa == "23" ? FT_txtComposicao_sCodigoServico_Recurso.ClientID
                                                                                            : TipoFiltroPesquisa == "4" || TipoFiltroPesquisa == "5" ? FT_txtComposicao_sCNPJarceiro_sCPFColaborador.ClientID
                                                                                            : FT_txtComposicao_sCodigoProduto.ClientID);
        }

        public void HabilitarItensPesquisa(bool bAtivo) => div_BuscarItens.Visible = bAtivo;

        public void HabilitarItemQtde(bool bAtivo) => div_Qtde.Visible = bAtivo;

        public decimal ObterQuantidade()
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("pt-BR");
            string dado = TipoFiltroPesquisa == "0" || TipoFiltroPesquisa == "1" || TipoFiltroPesquisa == "Orcamento" || TipoFiltroPesquisa == "6" ?
                              FT_txtnQuantidade.Text :
                          TipoFiltroPesquisa == "2" || TipoFiltroPesquisa == "3" || TipoFiltroPesquisa == "23" ?
                              FT_txtnQuantidadeServico_Recurso.Text :
                          "0";

            if (decimal.TryParse(dado, NumberStyles.Number, culture, out decimal converter)) return converter;

            return decimal.Zero;
        }

        public void LimparCampos()
        {
            if (TipoFiltroPesquisa == "0" || TipoFiltroPesquisa == "1" || TipoFiltroPesquisa == "Orcamento" || TipoFiltroPesquisa == "6")
            {
                FT_txtComposicao_sCodigoProduto.Text = "";
                FT_txtComposicao_sDscProduto.Text = "";
                FT_txtnQuantidade.Text = "";
                FT_ddlComposicao_sUnidade.SelectedValue = "";
                FT_txtnQuantidade.Text = "";
                FT_txtnValor.Text = "";
            }
            else if (TipoFiltroPesquisa == "2" || TipoFiltroPesquisa == "3" || TipoFiltroPesquisa == "23")
            {
                FT_txtComposicao_sCodigoServico_Recurso.Text = "";
                FT_txtComposicao_sDscServico_Recurso.Text = "";
                FT_txtnQuantidadeServico_Recurso.Text = "";
                FT_txtnQuantidadeServico_Recurso.Text = "";
                FT_txtnValorServico_Recurso.Text = "";
            }
            else if (TipoFiltroPesquisa == "4" || TipoFiltroPesquisa == "5")
            {
                FT_txtComposicao_sCNPJarceiro_sCPFColaborador.Text = "";
                FT_txtComposicao_sDscParceiro_Colaborador.Text = "";
            }
        }

        public void ConfigurarControles(bool bAtivo)
        {
            string sStatus = "disabled";

            if (bAtivo) sStatus = "enabled";

            if (TipoFiltroPesquisa == "0" || TipoFiltroPesquisa == "1" || TipoFiltroPesquisa == "Orcamento" || TipoFiltroPesquisa == "6")
            {
                FT_ddlComposicao_sUnidade.Attributes.Remove("disabled");
                FT_ddlComposicao_sUnidade.Attributes.Add(sStatus, sStatus);

                FT_ddlidFamilia.Attributes.Remove("disabled");
                FT_ddlidFamilia.Attributes.Add(sStatus, sStatus);

                FT_ddlidGrupo.Attributes.Remove("disabled");
                FT_ddlidGrupo.Attributes.Add(sStatus, sStatus);

                FT_ddlidPaisOrigem.Attributes.Remove("disabled");
                FT_ddlidPaisOrigem.Attributes.Add(sStatus, sStatus);

                FT_ddlidTipoProduto.Attributes.Remove("disabled");
                FT_ddlidTipoProduto.Attributes.Add(sStatus, sStatus);

                FT_txtComposicao_sCodigoProduto.ReadOnly = !bAtivo;
                FT_txtComposicao_sDscProduto.ReadOnly = !bAtivo;
                FT_txtnQuantidade.ReadOnly = !bAtivo;
                FT_txtnValor.ReadOnly = !bAtivo;
            }
            else if (TipoFiltroPesquisa == "2" || TipoFiltroPesquisa == "3" || TipoFiltroPesquisa == "23")
            {

                FT_txtComposicao_sCodigoServico_Recurso.ReadOnly = !bAtivo;
                FT_txtComposicao_sDscServico_Recurso.ReadOnly = !bAtivo;
                FT_txtnQuantidadeServico_Recurso.ReadOnly = !bAtivo;
                FT_txtnValorServico_Recurso.ReadOnly = !bAtivo;
            }
            else if (TipoFiltroPesquisa == "4" || TipoFiltroPesquisa == "5")
            {
                FT_txtComposicao_sDscParceiro_Colaborador.ReadOnly = !bAtivo;
                FT_txtComposicao_sCNPJarceiro_sCPFColaborador.ReadOnly = !bAtivo;
            }
        }

        public void Controle_ExibicaoCampos(Dictionary<string, bool> sCampos, string sTipo)
        {
            if (sTipo == "1") // Produto
            {
                foreach (KeyValuePair<string, bool> campos in sCampos)
                {
                    string chave = campos.Key;
                    bool valor = campos.Value;

                    if (chave == "Codigo de Produto") div_sCodigoProduto.Visible = valor;
                    else if (chave == "Descrição Produto") div_sDscProduto.Visible = valor;
                    else if (chave == "Quantidade") div_Qtde.Visible = valor;
                    else if (chave == "Unidade") div_sUnidade.Visible = valor;
                    else if (chave == "Valor Unitário") 
                    {
                        div_nValor.Visible = valor;
                        div_sDscProduto.Attributes["class"] = "col-lg-6";
                    }
                }
            }
            else if (sTipo == "3") // Recurso
            {
                foreach (KeyValuePair<string, bool> campos in sCampos)
                {
                    string chave = campos.Key;
                    bool valor = campos.Value;

                    if (chave == "Codigo Recurso") div_sCodigoServico_Recurso.Visible = valor;
                    else if (chave == "Descrição Recurso") div_sDscServico_Recurso.Visible = valor;
                    else if (chave == "Quantidade") div_QtdeServico_Recurso.Visible = valor;
                    else if (chave == "Valor Unitário")
                    {
                        div_nValorServico_Recurso.Visible = valor;
                        div_sCodigoServico_Recurso.Attributes["class"] = "col-lg-4";
                        div_sDscServico_Recurso.Attributes["class"] = "col-lg-6";
                    }
                }
            }
        }

        public void AlteraCampos_x_Tipo(int modoExibicao, bool bFiltro, bool bValor)
        {
            if (TipoFiltroPesquisa == "2" || TipoFiltroPesquisa == "3" || TipoFiltroPesquisa == "23")
            {
                HabilitarItemQtde(true);
                _FiltrosAtivos = new string[0];

                switch (modoExibicao)
                {
                    case 0:
                        lblCodigoServico_Recurso.InnerText = "Código" + (TipoFiltroPesquisa == "2" ? " do Serviço" : TipoFiltroPesquisa == "3" ? " do Recurso" : string.Empty);
                        lblDescServico_Recurso.InnerText = "Descrição" + (TipoFiltroPesquisa == "2" ? " do Serviço" : TipoFiltroPesquisa == "3" ? " do Recurso" : string.Empty);

                        div_sCodigoServico_Recurso.Attributes.Remove("class");
                        div_sCodigoServico_Recurso.Attributes.Add("class", "col-lg-3");

                        div_sDscServico_Recurso.Attributes.Remove("class");
                        div_sDscServico_Recurso.Attributes.Add("class", "col-lg-5");

                        div_nValorServico_Recurso.Attributes.Remove("class");
                        div_nValorServico_Recurso.Attributes.Add("class", "col-lg-2");
                        break;

                    case 1:
                        lblCodigoServico_Recurso.InnerText = "Código" + (TipoFiltroPesquisa == "3" ? " do Recurso" : string.Empty);
                        lblDescServico_Recurso.InnerText = "Descrição" + (TipoFiltroPesquisa == "3" ? " do Recurso" : string.Empty);

                        div_sCodigoServico_Recurso.Attributes.Remove("class");
                        div_sCodigoServico_Recurso.Attributes.Add("class", "col-lg-3");

                        div_sDscServico_Recurso.Attributes.Remove("class");
                        div_sDscServico_Recurso.Attributes.Add("class", "col-lg-7");
                        break;
                }
            }
            else if (TipoFiltroPesquisa == "Orcamento")
            {
                div_sUnidade.Visible = false;
                div_nValor.Visible = false;

                div_sCodigoProduto.Attributes.Remove("class");
                div_sCodigoProduto.Attributes.Add("class", "col-lg-3");

                div_sDscProduto.Attributes.Remove("class");
                div_sDscProduto.Attributes.Add("class", "col-lg-7");
            }

            switch (modoExibicao)
            {
                case 1:
                    lblCNPJ_CPFColaborador.InnerText = "CPF Colaborador";
                    lblDescParceiro_Colaborador.InnerText = "Nome Colaborador";
                    break;
            }

            div_FiltroProdutos.Visible = bFiltro;
            div_nValorServico_Recurso.Visible = bValor;
        }

        public bool ValidaParceiro()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>()
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idParceiro", IDParceiro }
            };
            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Clientes", vParametros);

            if (BD.ValidarDataSet(ds))
            {
                if (RETORNO.DATASET(ds, "sRazaoSocial").Equals(sDscParceiro_Colaborador) && RETORNO.DATASET(ds, "sCPF_CNPJ").Replace(".", "").Replace("-", "").Replace("/", "").Equals(sCNPJ_CPF_Parceiro_Colaborador.Replace(".", "").Replace("-", "").Replace("/", "")))
                    return true;
            }

            return false;
        }

        #endregion

        #region | Scripts

        /// <summary>
        /// Faz a lista abrir cheia quando o campo recebe foco ou é clicado, sem digitar nada.
        /// O jQuery UI só pesquisa a partir do que o usuário digita; forçar o search com termo
        /// vazio, junto de minLength 0, é o que traz tudo de uma vez.
        ///
        /// Precisa entrar DEPOIS do autocomplete({...}) e ainda dentro do ready, senão o
        /// plugin não existe no elemento na hora de chamar o search.
        ///
        /// off() antes do on(), com namespace próprio: os filtros vivem dentro de UpdatePanel
        /// e sem isso cada postback parcial empilharia mais um handler no mesmo campo.
        /// </summary>
        static void AbrirListaNoFoco(StringBuilder sb, string dgPagina, bool bAtivo)
        {
            if (!bAtivo)
                return;

            sb.Append("$v192(\"#" + dgPagina + "\").off('focus.listaCompleta click.listaCompleta')");
            sb.Append(".on('focus.listaCompleta click.listaCompleta', function() {");
            // Pesquisa o que já está no campo, não uma string fixa: campo vazio abre a lista
            // inteira, e clicar de novo num campo já preenchido mantém o filtro em vez de
            // jogar a lista toda por cima do que o usuário estava lendo.
            sb.Append("$v192(this).autocomplete('search', $v192(this).val() || '');");
            sb.Append("});");
        }

        public void ScriptsPagina(string dgPagina, List<string> elementos, Page pg, bool bCodigo, string proxCampo)
        {
            string sMetodoGet = "Produtos";

            Dictionary<string, string> vParametrosAjax = new Dictionary<string, string>();

            if (TipoFiltroPesquisa == "0" || TipoFiltroPesquisa == "1" || TipoFiltroPesquisa == "Orcamento" || TipoFiltroPesquisa == "6")
            {
                vParametrosAjax.Add("sDscProduto", "JSON.stringify(request.term)");
                vParametrosAjax.Add("idTipoProduto", "$('[id*=FT_ddlidTipoProduto]').val() || '0'");
                vParametrosAjax.Add("idFamilia", "$('[id*=FT_ddlidFamilia]').val() || '0'");
                vParametrosAjax.Add("idGrupo", "$('[id*=FT_ddlidGrupo]').val() || '0'");
                vParametrosAjax.Add("idPaisOrigem", "$('[id*=FT_ddlidPaisOrigem]').val() || '0'");
                vParametrosAjax.Add("sUnidade", "$('[id*=FT_ddlComposicao_sUnidade]').val() || ''");

                if (TipoFiltroPesquisa == "Orcamento")
                {
                    sMetodoGet = "Produtos_Orcamento";
                    vParametrosAjax["idPaisOrigem"] = TerritorioEmpresa.ToString();
                    vParametrosAjax.Add("idTabela", string.IsNullOrEmpty(idTabelaParceiro) ? "'0'" : idTabelaParceiro);
                    vParametrosAjax.Add("idParceiro", "$('[id*=FT_hddComposicao_idParceiro_Colaborador]').val() || '0'");
                }
                else vParametrosAjax.Add("FTidParceiro", "$('[id*=FT_hddComposicao_idParceiro_Colaborador]').val() || '0'");
            }
            else if (TipoFiltroPesquisa == "2" || TipoFiltroPesquisa == "3" || TipoFiltroPesquisa == "23")
            {
                sMetodoGet = "Servicos_Recursos";
                // "23" pede sempre a lista inteira (termo vazio): busca uma vez e filtra no
                // navegador. Com termo variável cada tecla seria uma ida ao banco.
                vParametrosAjax.Add("sDscProduto", TipoFiltroPesquisa == "23" ? "JSON.stringify('')" : "JSON.stringify(request.term)");
                // "23" agora manda -2 (Serviço + Sub-Serviço + Recurso). Mandava -1, que na
                // procedure filtra APENAS Sub-Serviços - o mesmo que o ramo 3. Nenhuma tela
                // ativa usava "23", então a correção não altera comportamento existente.
                vParametrosAjax.Add("sTipo", TipoFiltroPesquisa == "23" ? "-2" : (int.Parse(TipoFiltroPesquisa) - 1).ToString());
            }
            else if (TipoFiltroPesquisa == "4")
            {
                sMetodoGet = "Parceiros";
                if (!bCodigo) vParametrosAjax.Add("sRazaoSocial", "JSON.stringify(request.term)");
                else vParametrosAjax.Add("sCNPJ", "JSON.stringify(request.term)");
            }
            else if (TipoFiltroPesquisa == "5")
            {
                sMetodoGet = "Colaboradores";
                if (!bCodigo) vParametrosAjax.Add("sDscColaborador", "JSON.stringify(request.term)");
                else vParametrosAjax.Add("sCPF", "JSON.stringify(request.term)");
            }

            // Instalação/Obra ("23"): a lista abre inteira ao focar o campo, sem digitar nada.
            // O cadastro de Serviço/Sub-Serviço/Recurso é pequeno e o usuário quer navegar
            // nele, não adivinhar o nome. Os outros filtros continuam com os 3 caracteres:
            // buscam em Produtos, onde abrir tudo travaria a tela.
            bool bAbreListaCompleta = TipoFiltroPesquisa == "23";
            string sMinLength = bAbreListaCompleta ? "0" : "3";

            StringBuilder sb = new StringBuilder();
            sb.Append("$v192(function() {");

            if (bAbreListaCompleta)
            {
                // Uma consulta por campo, no primeiro foco; daí em diante filtra em memória.
                // A consulta com termo vazio leva ~1s (a procedure varre tbl_Flow_Produtos
                // antes de reduzir aos 3 tipos), e pagar isso a cada clique era o que
                // incomodava. O cache vive no closure do campo: recarregar a página o
                // renova, que é o suficiente para um cadastro que muda pouco.
                sb.Append("var cacheLC = null;");
                sb.Append("function filtraLC(lista, termo) {");
                sb.Append("termo = (termo || '').toLowerCase();");
                sb.Append("if (!termo) return lista;");
                sb.Append("return $v192.grep(lista, function(x) { return x._busca.indexOf(termo) >= 0; });");
                sb.Append("}");
            }

            sb.Append("$v192(\"#" + dgPagina + "\").autocomplete({");
            sb.Append("source: function(request, response) {");

            if (bAbreListaCompleta)
                sb.Append("if (cacheLC) { response(filtraLC(cacheLC, request.term)); return; }");

            sb.Append("$v192.ajax({");
            sb.Append("url:'/API/Pagina_Ajax.aspx/Get" + (!bCodigo ? sMetodoGet : sMetodoGet + "_Codigo") + "',");
            sb.Append("data: JSON.stringify({");

            foreach (KeyValuePair<string, string> item in vParametrosAjax)
            {
                sb.Append("'" + item.Key + "': " + item.Value + ", ");
            }

            if (vParametrosAjax.Count > 0) sb.Length -= 2; // Remove a vírgula extra

            sb.Append("}),"); // Adicione uma vírgula após a chave 'data'

            sb.Append("dataType: \"json\",");
            sb.Append("type: \"POST\",");
            sb.Append("contentType: \"application/json; charset=utf-8\",");
            sb.Append("success: function(data) {");

            if (bAbreListaCompleta)
                sb.Append("var listaLC = $v192.map(data.d, function(item) {");
            else
                sb.Append("response($v192.map(data.d, function(item) {");

            sb.Append("return {");

            sb.Append("label: item.split('|')[0],");

            // Campo de busca local com as duas colunas de texto (código e descrição, em
            // ordem que varia conforme o campo). É o mesmo par que a procedure procura com
            // LIKE, então filtrar aqui não perde resultado nenhum.
            if (bAbreListaCompleta)
                sb.Append("_busca: (item.split('|')[0] + ' ' + item.split('|')[1]).toLowerCase(),");

            int index = 0;
            foreach (string elementoID in elementos)
            {
                sb.Append(elementoID + ": item.split('|')[" + (index) + "],");
                index++;
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append("};");

            if (bAbreListaCompleta)
            {
                sb.Append("});");
                sb.Append("cacheLC = listaLC;");
                sb.Append("response(filtraLC(listaLC, request.term));");
            }
            else
            {
                sb.Append("}));");
            }

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

            //Thiago Rodrigues 04/12/2024
            if (sRegistraUnidade != "S")
            {
                foreach (string elementoID in elementos)
                {
                    sb.Append("$(\"#" + elementoID + "\").val(i.item." + elementoID + ");");
                }
                sb.Append("$('#" + proxCampo + "').focus();");
                sb.Append("},");
                sb.Append("minLength: " + sMinLength);
                sb.Append("});");
                AbrirListaNoFoco(sb, dgPagina, bAbreListaCompleta);
                sb.Append("});");
            }
            else
            {
                foreach (string elementoID in elementos)
                {
                    if (elementoID == "cphCorpo_FiltroPesquisa_FT_hddComposicao_sUnidade")
                    {
                        // Garantir que o valor seja inserido corretamente no DDL
                        sb.Append("var ddl = $(\"#cphCorpo_FiltroPesquisa_FT_ddlComposicao_sUnidade\");");
                        sb.Append("var valor = i.item.cphCorpo_FiltroPesquisa_FT_hddComposicao_sUnidade;");

                        // Adiciona o valor diretamente ao DDL sem verificar a existência
                        sb.Append("ddl.val(valor);");

                        // Confirmação de que o valor foi inserido
                        sb.Append("console.log('Valor inserido na DDL: ' + valor);");
                    }
                    if (elementoID == "cphCorpo_FiltroPesquisa1_FT_hddComposicao_sUnidade")
                    {
                        sb.Append("var ddl = $(\"#cphCorpo_FiltroPesquisa1_FT_ddlComposicao_sUnidade\");");
                        sb.Append("var valor = i.item.cphCorpo_FiltroPesquisa1_FT_hddComposicao_sUnidade;");

                        sb.Append("ddl.val(valor);");

                        sb.Append("console.log('Valor inserido na DDL: ' + valor);");
                    }
                    if (elementoID.EndsWith("sUnidade"))
                    {
                        sb.Append("var ddl = $(\"#cphCorpo_FiltroPesquisa_FT_ddlComposicao_sUnidade\");");
                        sb.Append("var valor = i.item." + elementoID + ";");
                        sb.Append("ddl.val(valor);");
                        sb.Append("console.log('Valor inserido na DDL: ' + valor);");

                        if (SAtivaPostBack == "S")
                        {
                            // Postback com valor de IdProduto
                            sb.Append("$('#cphCorpo_hddIdProduto').val(i.item.IdProduto);");
                            sb.Append("__doPostBack('cphCorpo_hddIdProduto', '');");
                        }
                        //sb.Append($"console.log('Valor inserido na DDL:  + {FT_hddComposicao_sDscTipoProduto.Value}');");
                    }
                    else
                    {
                        // Para os outros campos, insira o valor normalmente
                        sb.Append("$(\"#" + elementoID + "\").val(i.item." + elementoID + ");");
                    }
                }
                sb.Append("$('#" + proxCampo + "').focus();");
                sb.Append("},");
                sb.Append("minLength: " + sMinLength);
                sb.Append("});");
                AbrirListaNoFoco(sb, dgPagina, bAbreListaCompleta);
                sb.Append("});");
            }
            if (TipoFiltroPesquisa == "6")
                sb.Append("$('[id*=FT_txtnQuantidade]').mask('0.000.000.009,99', { reverse: true });");
            else
                sb.Append("$('[id*=FT_txtnQuantidade]').mask('0.000.000.009,9999', { reverse: true });");

            sb.AppendLine("");

            sb.Append("$(document).ready(function () {");
            sb.Append("    $('.collapse').each(function (index) {");
            sb.Append("        var controlId = 'idFiltroCollapse' + (index + 1);");
            sb.Append("        $(this).attr('id', controlId);");
            sb.Append("        $(this).prev('a').attr('href', '#' + controlId);");
            sb.Append("    });");
            sb.Append("});");

            sb.AppendLine("");

            if (TipoFiltroPesquisa == "Orcamento" || TipoFiltroPesquisa == "4" || TipoFiltroPesquisa == "5")
            {
                sb.Append("if ($('[id*=FT_txtComposicao_sCNPJarceiro_sCPFColaborador]').val() && $('[id*=FT_txtComposicao_sCNPJarceiro_sCPFColaborador]').val().replace('.', '').replace('-', '').replace('/', '').length == 14) {\r\n");
                sb.Append("     $('[id*=FT_txtComposicao_sCNPJarceiro_sCPFColaborador]').mask('00.000.000/0000-00', { reverse: true });\r\n");
                sb.Append("}\r\n");
            }

            ScriptManager.RegisterStartupScript(pg, pg.GetType(), "js_RegistraScripts_FiltroPesquisa_" + Guid.NewGuid(), sb.ToString(), true);
        }
        public void ScriptsPaginaMovimentacao(string dgPagina, List<string> elementos, Page pg, bool bCodigo, string proxCampo)
        {
            string sMetodoGet = "Produtos";
            Dictionary<string, string> vParametrosAjax = new Dictionary<string, string>();

            // CORREÇÃO 1: Usando .ClientID para evitar conflito com o FiltroPesquisa1
            if (TipoFiltroPesquisa == "0" || TipoFiltroPesquisa == "1" || TipoFiltroPesquisa == "Orcamento" || TipoFiltroPesquisa == "6")
            {
                // AQUI ESTÁ A MÁGICA: Alterado de "sDscProduto" para "term" para dar match com o WebMethod.
                // Removido também o JSON.stringify interno. O JS vai passar a string limpa agora!
                vParametrosAjax.Add("term", "request.term");
                vParametrosAjax.Add("idTipoProduto", "$jq('#" + FT_ddlidTipoProduto.ClientID + "').val() || '0'");
                vParametrosAjax.Add("idFamilia", "$jq('#" + FT_ddlidFamilia.ClientID + "').val() || '0'");
                vParametrosAjax.Add("idGrupo", "$jq('#" + FT_ddlidGrupo.ClientID + "').val() || '0'");
                vParametrosAjax.Add("idPaisOrigem", "$jq('#" + FT_ddlidPaisOrigem.ClientID + "').val() || '0'");

                if (TipoFiltroPesquisa == "Orcamento")
                {
                    sMetodoGet = "Produtos_Orcamento";
                    // Adicionado aspas simples para injetar a string corretamente no JS
                    vParametrosAjax["idPaisOrigem"] = "'" + TerritorioEmpresa.ToString() + "'";
                    vParametrosAjax.Add("idTabela", string.IsNullOrEmpty(idTabelaParceiro) ? "'0'" : "'" + idTabelaParceiro + "'");
                    vParametrosAjax.Add("idParceiro", "$jq('#" + FT_hddComposicao_idParceiro_Colaborador.ClientID + "').val() || '0'");
                }
                else
                {
                    vParametrosAjax.Add("FTidParceiro", "$jq('#" + FT_hddComposicao_idParceiro_Colaborador.ClientID + "').val() || '0'");
                }
            }
            else if (TipoFiltroPesquisa == "2" || TipoFiltroPesquisa == "3" || TipoFiltroPesquisa == "23")
            {
                sMetodoGet = "Servicos_Recursos";
                vParametrosAjax.Add("term", "request.term"); // Ajustado aqui também por precaução
                // Mesma correção da busca por descrição: "23" passa a mandar -2.
                vParametrosAjax.Add("sTipo", "'" + (TipoFiltroPesquisa == "23" ? "-2" : (int.Parse(TipoFiltroPesquisa) - 1).ToString()) + "'");
            }
            else if (TipoFiltroPesquisa == "4")
            {
                sMetodoGet = "Parceiros";
                if (!bCodigo) vParametrosAjax.Add("sRazaoSocial", "request.term");
                else vParametrosAjax.Add("sCNPJ", "request.term");
            }
            else if (TipoFiltroPesquisa == "5")
            {
                sMetodoGet = "Colaboradores";
                if (!bCodigo) vParametrosAjax.Add("sDscColaborador", "request.term");
                else vParametrosAjax.Add("sCPF", "request.term");
            }

            StringBuilder sb = new StringBuilder();
            string funcName = "init_Autocomp_" + dgPagina;

            sb.AppendLine("function " + funcName + "() {");
            sb.AppendLine("  var $jq = window.jQuery || window.$;");
            sb.AppendLine("  if (!$jq) return;");

            sb.AppendLine("  var $input = $jq('#" + dgPagina + "');");
            sb.AppendLine("  if ($input.length === 0 || typeof $input.autocomplete !== 'function') return;");

            // CORREÇÃO 2: Bloquear a tecla ENTER no campo para evitar o Postback que mata a requisição
            sb.AppendLine("  $input.off('keydown').on('keydown', function(e) { if(e.which === 13) { e.preventDefault(); return false; } });");

            // Destrói autocomplete fantasma antigo
            sb.AppendLine("  try { if ($input.data('ui-autocomplete')) { $input.autocomplete('destroy'); } } catch(e) {}");

            sb.Append("  $input.autocomplete({");
            sb.Append("    minLength: 3,");
            sb.Append("    appendTo: $input.parent(),"); // CORREÇÃO 3: Prende a lista HTML dentro da Div, evitando erro de z-index
            sb.Append("    source: function(request, response) {");
            sb.Append("      console.log('--> Buscando no Backend: ' + request.term);");
            sb.Append("      $jq.ajax({");
            sb.Append("        url:'/app/Paginas/WMS/Movimentacao_Detalhe.aspx/Get" + (!bCodigo ? sMetodoGet : sMetodoGet + "_Codigo") + "',");

            sb.Append("        data: JSON.stringify({");
            foreach (KeyValuePair<string, string> item in vParametrosAjax)
            {
                sb.Append("'" + item.Key + "': " + item.Value + ", ");
            }
            if (vParametrosAjax.Count > 0) sb.Length -= 2;
            sb.Append("        }),");

            sb.Append("        dataType: 'json',");
            sb.Append("        type: 'POST',");
            sb.Append("        contentType: 'application/json; charset=utf-8',");
            sb.Append("        success: function(data) {");
            sb.Append("          console.log('-> Sucesso AJAX!', data);");
            sb.Append("          if(!data || !data.d) return;");
            sb.Append("          response($jq.map(data.d, function(item) {");
            sb.Append("            var parts = item.split('|');");
            sb.Append("            return {");
            sb.Append("              label: parts[0],"); // A label visual para o usuário

            // INSERINDO OS ELEMENTOS NO OBJETO JS
            int index = 0;
            foreach (string elementoID in elementos)
            {
                sb.Append("              '" + elementoID + "': parts[" + index + "],");
                index++;
            }
            sb.Length -= 1; // Remove a última vírgula
            sb.Append("            };");
            sb.Append("          }));");
            sb.Append("        },");
            sb.Append("        error: function(xhr, status, error) { console.error('-> Erro AJAX:', status, error, xhr.responseText); }");
            sb.Append("      });");
            sb.Append("    },");

            sb.Append("    select: function(e, ui) {");
            sb.Append("      console.log('-> Item Selecionado:', ui.item);");
            if (sRegistraUnidade != "S")
            {
                foreach (string elementoID in elementos)
                {
                    sb.Append("      $jq('#" + elementoID + "').val(ui.item['" + elementoID + "']);");
                }
                sb.Append("      $jq('#" + proxCampo + "').focus();");
            }
            else
            {
                foreach (string elementoID in elementos)
                {
                    if (elementoID.EndsWith("sUnidade"))
                    {
                        string ddlID = elementoID.Replace("hddComposicao", "ddlComposicao");
                        sb.Append("      $jq('#" + ddlID + "').val(ui.item['" + elementoID + "']);");
                        if (SAtivaPostBack == "S")
                        {
                            sb.Append("      $jq('#cphCorpo_hddIdProduto').val(ui.item['" + elementos[2] + "']);");
                            sb.Append("      setTimeout(function() { __doPostBack('cphCorpo_hddIdProduto', ''); }, 150);");
                        }
                    }
                    else
                    {
                        sb.Append("      $jq('#" + elementoID + "').val(ui.item['" + elementoID + "']);");
                    }
                }
                sb.Append("      $jq('#" + proxCampo + "').focus();");
            }
            sb.Append("    }");
            sb.AppendLine("  });");

            if (TipoFiltroPesquisa == "6")
                sb.AppendLine("  if(typeof $jq.fn.mask === 'function') { $jq('[id*=FT_txtnQuantidade]').mask('0.000.000.009,99', { reverse: true }); }");
            else
                sb.AppendLine("  if(typeof $jq.fn.mask === 'function') { $jq('[id*=FT_txtnQuantidade]').mask('0.000.000.009,9999', { reverse: true }); }");

            if (TipoFiltroPesquisa == "Orcamento" || TipoFiltroPesquisa == "4" || TipoFiltroPesquisa == "5")
            {
                sb.AppendLine("  var cpfCnpj = $jq('#" + FT_txtComposicao_sCNPJarceiro_sCPFColaborador.ClientID + "');");
                sb.AppendLine("  if (cpfCnpj.length > 0 && typeof $jq.fn.mask === 'function') { cpfCnpj.mask('00.000.000/0000-00', { reverse: true }); }");
            }
            sb.AppendLine("}"); // Fim função

            // Força a reinicialização a cada UpdatePanel
            sb.AppendLine("if(typeof window.jQuery !== 'undefined') { window.jQuery(document).ready(function() { " + funcName + "(); }); }");
            sb.AppendLine("if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {");
            sb.AppendLine("  var prm = Sys.WebForms.PageRequestManager.getInstance();");
            sb.AppendLine("  prm.remove_endRequest(" + funcName + ");");
            sb.AppendLine("  prm.add_endRequest(" + funcName + ");");
            sb.AppendLine("}");

            ScriptManager.RegisterStartupScript(pg, pg.GetType(), "js_AutoComp_" + dgPagina, sb.ToString(), true);
        }
        public void RegistrarScriptPesquisar()
        {
            string campoPesquisa = "";
            string campoPesquisa1 = "";

            string sDsc;
            string sCodigo;
            string hddComposicao_ID;
            string hddnPreco;
            string hddsUnidade;
            string hddsTipo;
            string hddidTipo;
            string sUnidade;

            string proxCampo = "";

            List<string> listPesquisa = new List<string>();
            List<string> listPesquisa1 = new List<string>();

            if (TipoFiltroPesquisa == "0" || TipoFiltroPesquisa == "1" || TipoFiltroPesquisa == "6")
            {
                campoPesquisa = FT_txtComposicao_sDscProduto.ClientID;
                campoPesquisa1 = FT_txtComposicao_sCodigoProduto.ClientID;

                sDsc = FT_txtComposicao_sDscProduto.ClientID;
                sCodigo = FT_txtComposicao_sCodigoProduto.ClientID;
                sUnidade = FT_ddlComposicao_sUnidade.ClientID;
                hddComposicao_ID = FT_hddComposicao_idItem.ClientID;
                hddsUnidade = FT_hddComposicao_sUnidade.ClientID;
                hddsTipo = FT_hddComposicao_sDscTipoProduto.ClientID;
                if (TipoFiltroPesquisa == "6")
                    proxCampo = FT_txtnQuantidade.ClientID;
                else
                    proxCampo = FT_txtnValor.ClientID;

                listPesquisa.Add(sDsc);
                listPesquisa.Add(sCodigo);
                listPesquisa.Add(hddComposicao_ID);
                listPesquisa.Add(sUnidade);
                listPesquisa.Add(hddsTipo);
                listPesquisa.Add(hddsUnidade);

                listPesquisa1.Add(sCodigo);
                listPesquisa1.Add(sDsc);
                listPesquisa1.Add(hddComposicao_ID);
                listPesquisa1.Add(sUnidade);
                listPesquisa1.Add(hddsTipo);
                listPesquisa1.Add(hddsUnidade);
            }
            else if (TipoFiltroPesquisa == "Orcamento")
            {
                campoPesquisa = FT_txtComposicao_sDscProduto.ClientID;
                campoPesquisa1 = FT_txtComposicao_sCodigoProduto.ClientID;

                sDsc = FT_txtComposicao_sDscProduto.ClientID;
                sCodigo = FT_txtComposicao_sCodigoProduto.ClientID;
                hddComposicao_ID = FT_hddComposicao_idItem.ClientID;
                hddnPreco = FT_hddComposicao_nPreco.ClientID;

                proxCampo = FT_txtnQuantidade.ClientID;

                listPesquisa.Add(sDsc);
                listPesquisa.Add(sCodigo);
                listPesquisa.Add(hddComposicao_ID);
                listPesquisa.Add(hddnPreco);

                listPesquisa1.Add(sCodigo);
                listPesquisa1.Add(sDsc);
                listPesquisa1.Add(hddComposicao_ID);
                listPesquisa1.Add(hddnPreco);
            }
            else if (TipoFiltroPesquisa == "2" || TipoFiltroPesquisa == "3" || TipoFiltroPesquisa == "23")
            {
                campoPesquisa = FT_txtComposicao_sDscServico_Recurso.ClientID;
                campoPesquisa1 = FT_txtComposicao_sCodigoServico_Recurso.ClientID;

                sDsc = FT_txtComposicao_sDscServico_Recurso.ClientID;
                sCodigo = FT_txtComposicao_sCodigoServico_Recurso.ClientID;
                hddComposicao_ID = FT_hddComposicao_idServico_Recurso.ClientID;
                hddsTipo = FT_hddComposicao_sDscTipoServico_Recurso.ClientID;
                hddidTipo = FT_hddComposicao_idTipoServico_Recurso.ClientID;
                hddsUnidade = FT_hddComposicao_sUnidadeServico_Recurso.ClientID;

                proxCampo = FT_txtnQuantidadeServico_Recurso.ClientID;

                listPesquisa.Add(sDsc);
                listPesquisa.Add(sCodigo);
                listPesquisa.Add(hddComposicao_ID);
                listPesquisa.Add(hddsTipo);
                listPesquisa.Add(hddidTipo);
                listPesquisa.Add(hddsUnidade);

                listPesquisa1.Add(sCodigo);
                listPesquisa1.Add(sDsc);
                listPesquisa1.Add(hddComposicao_ID);
                listPesquisa1.Add(hddsTipo);
                listPesquisa1.Add(hddidTipo);
                listPesquisa1.Add(hddsUnidade);
            }
            else if (TipoFiltroPesquisa == "4" || TipoFiltroPesquisa == "5")
            {
                campoPesquisa = FT_txtComposicao_sDscParceiro_Colaborador.ClientID;
                campoPesquisa1 = FT_txtComposicao_sCNPJarceiro_sCPFColaborador.ClientID;

                sDsc = FT_txtComposicao_sDscParceiro_Colaborador.ClientID;
                sCodigo = FT_txtComposicao_sCNPJarceiro_sCPFColaborador.ClientID;
                hddComposicao_ID = FT_hddComposicao_idParceiro_Colaborador.ClientID;

                proxCampo = ClientID_FocusPersonalizado != "" ? ClientID_FocusPersonalizado : sDsc;

                listPesquisa.Add(sDsc);
                listPesquisa.Add(sCodigo);
                listPesquisa.Add(hddComposicao_ID);

                listPesquisa1.Add(sCodigo);
                listPesquisa1.Add(sDsc);
                listPesquisa1.Add(hddComposicao_ID);
            }

            ScriptsPagina(campoPesquisa, listPesquisa, this.Page, false, proxCampo);
            ScriptsPagina(campoPesquisa1, listPesquisa1, this.Page, true, proxCampo);
        }

        public void RegistrarScriptPesquisarMovimentacao()
        {
            string campoPesquisa = "";

            string sDsc;
            string sCodigo;
            string hddComposicao_ID;
            string hddsUnidade;
            string hddsTipo;

            string proxCampo = "";

            List<string> listPesquisa = new List<string>();
            List<string> listPesquisa1 = new List<string>();

            if (TipoFiltroPesquisa == "0" || TipoFiltroPesquisa == "1" || TipoFiltroPesquisa == "6")
            {
                campoPesquisa = FT_txtComposicao_sDscProduto.ClientID;

                sDsc = FT_txtComposicao_sDscProduto.ClientID;
                sCodigo = FT_txtComposicao_sCodigoProduto.ClientID;
                hddComposicao_ID = FT_hddComposicao_idItem.ClientID;
                hddsUnidade = FT_hddComposicao_sUnidade.ClientID;
                hddsTipo = FT_hddComposicao_sDscTipoProduto.ClientID;

                proxCampo = FT_txtnQuantidade.ClientID;

                listPesquisa.Add(sDsc);
                listPesquisa.Add(sCodigo);
                listPesquisa.Add(hddComposicao_ID);
                listPesquisa.Add(hddsUnidade);
                listPesquisa.Add(hddsTipo);

                listPesquisa1.Add(sCodigo);
                listPesquisa1.Add(sDsc);
                listPesquisa1.Add(hddComposicao_ID);
                listPesquisa1.Add(hddsUnidade);
                listPesquisa1.Add(hddsTipo);
            }

            ScriptsPaginaMovimentacao(campoPesquisa, listPesquisa, this.Page, false, proxCampo);
            //ScriptsPagina(campoPesquisa1, listPesquisa1, this.Page, true, proxCampo);
        }

        #endregion

        #region | Modificar Campos

        public void ModificaTamanhoCampos(int tamanhoCodigoProduto, int tamanhoDescProduto, int tamanhoUnidade, int tamanhoQtde, int tamanhoValor)
        {
            string AtualizaClasse(string classesAtuais, int novoTamanho)
            {
                string classesAtualizadas = System.Text.RegularExpressions.Regex.Replace(classesAtuais, @"\bcol-lg-\d+\b", "").Trim();
                return $"{classesAtualizadas} col-lg-{novoTamanho}".Trim();
            }

            div_sCodigoProduto.Attributes["class"] = AtualizaClasse(div_sCodigoProduto.Attributes["class"], tamanhoCodigoProduto);
            div_sDscProduto.Attributes["class"] = AtualizaClasse(div_sDscProduto.Attributes["class"], tamanhoDescProduto);
            div_sUnidade.Attributes["class"] = AtualizaClasse(div_sUnidade.Attributes["class"], tamanhoUnidade);
            div_Qtde.Attributes["class"] = AtualizaClasse(div_Qtde.Attributes["class"], tamanhoQtde);
            div_nValor.Attributes["class"] = AtualizaClasse(div_nValor.Attributes["class"], tamanhoValor);
        }

        public void desligaColapso(bool ativo)
        {
            div_FiltroProdutos.Visible = !ativo;
        }

        #endregion
    }
}