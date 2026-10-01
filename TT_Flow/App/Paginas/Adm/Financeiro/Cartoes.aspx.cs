using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static TT.FrameWork.BD;
using TT.FrameWork;
using Identity = TT.FrameWork.Identity;
using System.Data;
using TT_Hub.App.Paginas.RRHH;
using Funcoes = TT.FrameWork.Funcoes;
using System.Text;
using static NPOI.HSSF.Record.UnicodeString;
using TT_Flow.App.Controles;
using static TT.FrameWork.Identity;
using TT_Hub.App.Paginas.Manutencao;
using System.Data.SqlClient;
using TT_Flow.FrameWork;
using static TT_Flow.App.Paginas.Adm.Financeiro.Cartoes;
using System.Globalization;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace TT_Flow.App.Paginas.Adm.Financeiro
{
    //http://localhost:6997/App/Paginas/Adm/Financeiro/Cartoes.aspx
    public partial class Cartoes : System.Web.UI.Page
    {
        string sTituloPagina = "Cartões";
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_ContasBancarias";
        static string nValorTotalFixo;

        public List<cls__Cartao_Lancamento_Produtos> bs_Lancamento_Produtos
        {
            get
            {
                if (ViewState["bs_Lancamento_Produtos"] == null)
                {
                    ViewState["bs_Lancamento_Produtos"] = new List<FrameWork.cls__Cartao_Lancamento_Produtos>();
                }
                return (List<FrameWork.cls__Cartao_Lancamento_Produtos>)ViewState["bs_Lancamento_Produtos"];
            }
            set
            {
                ViewState["bs_Lancamento_Produtos"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Funcoes.ValidaPermissao(Permissao.Financeiro.Cartoes.Consultar, true);
            if (!Funcoes.ValidaPermissao(Permissao.Financeiro.Cartoes.Inserir))
            {
                cmdNovo.Visible = false;
            }

            //Manual Usuario
            manual.sNomeArquivo = "Manual_Cartões.pdf";

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;


                if (!string.IsNullOrEmpty(Request.QueryString["grid"]))
                {
                    hddMudaGrid.Value = Request.QueryString["grid"];
                }

                if (!string.IsNullOrEmpty(Request.QueryString["cartao"]))
                {
                    hddidCartao.Value = Request.QueryString["cartao"];
                }

                Funcoes.Popula_Combo(ddlidCategoriaConsulta, "sp_Select 'Flow_Adm_Contas_Pagar_Categoria'", "idCategoriaPagar", "sDscCategoriaPagar", false, "Todas as Categorias", "0");

                Pesquisar();

                string id = Request.QueryString["id"];
                string sConciliacao = Request.QueryString["conciliacao"];
                if (!string.IsNullOrEmpty(id) && sConciliacao == "true")
                {
                    pnResultado.Visible = true;
                    hddMudaGrid.Value = "1";
                    div_filtro.Visible = false;
                    ConsultaFatura(id);
                }

                string sCentroCusto = Request.QueryString["cc"];

                if (!string.IsNullOrEmpty(id) && sCentroCusto == "true")
                {
                    hddidLancamento.Value = id;
                    PopulaCombo();
                    LancamentoDetalhe();
                    string scriptModal = "window.onload = function() { $('#modalLancamentoDetalhe').modal('show'); };";
                    ScriptManager.RegisterStartupScript(this, GetType(), "openModalLancamentoDetalhe", scriptModal, true);
                }
            }

            if (Session["ItemId"] != null)
            {
                hddidLancamento.Value = Session["ItemId"].ToString();

                if (Session["OpenModal"] as string == "modalLancamentoDetalhe")
                {
                    PopulaCombo();
                    LancamentoDetalhe();
                    string scriptModal = "window.onload = function() { $('#modalLancamentoDetalhe').modal('show'); };";
                    ScriptManager.RegisterStartupScript(this, GetType(), "openModalLancamentoDetalhe", scriptModal, true);
                    Session.Remove("OpenModal");
                }
                Session.Remove("ItemId");
            }

            if (Session["Excluir"] != null)
            {
                MensagemPaginaGrid.MostraMensagem_Sucesso(Session["Mensagem"].ToString());
                Session.Remove("Excluir");
            }

            RegistraScript();
        }

        #region | Consulta e Outros

        private void Pesquisar()
        {
            pnResultado.Visible = false;
            div_filtro.Visible = true;

            string idUsuario = Identity.Variaveis.idUsuario();
            if (Funcoes.ValidaPermissao(Permissao.Financeiro.Cartoes.VisualizarTudo))
            {
                idUsuario = "0";
            }

            DataSet ds;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR_LANCAMENTO_CARTAO");
            vParametros.Add("@idUsuario", idUsuario);
            vParametros.Add("@idCartao", hddidCartao.Value);
            vParametros.Add("@idCategoria", ddlidCategoriaConsulta.SelectedValue);
            vParametros.Add("@dtFinal", txtdtFinal.Text);
            vParametros.Add("@dtInicial", txtdtInicial.Text);
            vParametros.Add("@sTipoLancamento", ddlsTipoConsulta.SelectedValue);
            vParametros.Add("@sDscLancamento", txtPesquisa.Text);
            vParametros.Add("@sStatus", ddlAtivoCartao.SelectedValue);

            ds = BD.ExecutarDataSet(sProcedure, vParametros, false);


            pnResultado.Visible = true;
            if (hddMudaGrid.Value == "1")
            {
                if (ds.Tables[1].Rows.Count != 0)  // Grid Lançamentos
                {
                    div_Cartoes.Visible = false;
                    div_Lancamentos.Visible = true;
                    btnVoltarGrid.Visible = true;
                    btnVoltarGrid2.Visible = true;
                    div_ddlAtivo.Visible = false;
                    div_ddlsTipo.Visible = true;
                    div_botoesFiltro.Attributes["class"] = "col-lg-3";
                    div_sPesquisa.Attributes["class"] = "col-lg-5";

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, ds.Tables[1], 1, "desc", "false", "''"), true);
                }
                else    // Grid Lançamentos 
                {
                    div_Cartoes.Visible = false;
                    div_Lancamentos.Visible = true;
                    btnVoltarGrid.Visible = true;
                    btnVoltarGrid2.Visible = true;
                    div_ddlAtivo.Visible = false;
                    div_ddlsTipo.Visible = true;
                    div_botoesFiltro.Attributes["class"] = "col-lg-3";
                    div_sPesquisa.Attributes["class"] = "col-lg-5";

                    dtgvConsulta.DataSource = null;
                    dtgvConsulta.DataBind();
                    MensagemPaginaGrid.MostraMensagem_Erro("Nenhum Lançamento Localizado");
                }
            }
            else  //Grid Cartoes
            {
                div_Cartoes.Visible = true;
                div_Lancamentos.Visible = false;
                btnVoltarGrid2.Visible = false;
                div_ddlAtivo.Visible = true;
                div_ddlsTipo.Visible = false;
                div_botoesFiltro.Attributes["class"] = "col-lg-2";
                div_sPesquisa.Attributes["class"] = "col-lg-3";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables2", TT.FrameWork.Grid.DataBindComScriptData(gv_Cartao, ds.Tables[0], 4, "asc", "false", "''"), true);
            }


        }

        private void ConsultaFatura(string id)
        {
            DataSet ds;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR_LANCAMENTO_FATURA");
            vParametros.Add("@idConciliado", id);
            ds = BD.ExecutarDataSet(sProcedure, vParametros, false);

            div_Cartoes.Visible = false;
            div_Lancamentos.Visible = true;
            btnVoltarGrid.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, ds.Tables[0], 1, "desc", "false", "''"), true);
        }

        private void LancamentoDetalhe()
        {
            string sErro = "";
            DataSet ds;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "DETALHE_LANCAMENTO_CARTAO");
            vParametros.Add("@idLancamento", hddidLancamento.Value);
            ds = BD.ExecutarDataSet(sProcedure, vParametros, false);
            if (BD.ValidarDataSet(ds, out sErro))
            {
                txtdtLancamento.Text = Retorno.DATASET(ds, 0, "dtLancamento");
                txtsDscLancamento.Text = Retorno.DATASET(ds, 0, "sDscLancamento");
                ddlidCategoriaPagar.Text = Retorno.DATASET(ds, 0, "idCategoria");
                ddlidContabil.Text = Retorno.DATASET(ds, 0, "idCodigoContabil");
                ddlsTipo.Text = Retorno.DATASET(ds, 0, "sTipoLancamento");
                txtnValor.Text = Retorno.DATASET(ds, 0, "nVlrLancamento");
                ddlidCartao.SelectedValue = Retorno.DATASET(ds, 0, "idCartao");
                ddlidUsuario.SelectedValue = Retorno.DATASET(ds, 0, "idUsuarioDespesa");

                Controle_CategoriasCC.DefinirValoresSelecionados(Retorno.DATASET(ds, 0, "idCentroDeCusto"), Retorno.DATASET(ds, 0, "idRegistroCategoria"));
                
                string sConciliado = Retorno.DATASET(ds, 0, "sConcilado") == null ? "" : Retorno.DATASET(ds, 0, "sConcilado");
                string idVinculoParcela = Retorno.DATASET(ds, 0, "idVinculoParcela");

                ddlsTipo.Attributes.Add("disabled", "disabled");
                ddlidContabil.Attributes.Add("disabled", "disabled");
                div_parcelado.Visible = false;

                if (Retorno.DATASET(ds, 0, "sInternacional") == "S")
                {
                    swtCompraInternacional.Definir("S", "Compra Internacional?", "S");
                    txtValorCambio.Text = Retorno.DATASET(ds, 0, "nValorCambio");
                    ddlMoeda.SelectedValue = Retorno.DATASET(ds, 0, "idTipoMoeda");
                    div_parcelado.Visible = true;
                    div_swtParcelado.Visible = false;
                    div_qtdParcela.Visible = false;
                    div_compraInternacional.Visible = true;
                    txtnValor.ReadOnly = true;
                    ddlMoeda.Attributes.Add("disabled", "disabled");
                    txtValorCambio.ReadOnly = true;
                }

                if (sConciliado != "")
                {
                    div_btnExcluir.Visible = false;
                    LancamentoDetalheConsulta();
                }

                if (!Funcoes.ValidaPermissao(Permissao.Financeiro.Cartoes.EditarItem))
                    dtgv_produtosLancamento.Columns[7].Visible = false;
                

                if (!Funcoes.ValidaPermissao(Permissao.Financeiro.Cartoes.Editar))
                {
                    LancamentoDetalheConsulta();
                    ddlidCategoriaPagar.Attributes.Add("disabled", "disabled");
                    btnSalvar.Visible = false;
                }

                if (!Funcoes.ValidaPermissao(Permissao.Financeiro.Cartoes.Excluir))
                    div_btnExcluir.Visible = false;

                PopularProdutoLancamento(ds);
                Popular_Aba_Historico(ds);

            }

        }

        private void LancamentoDetalheConsulta()
        {
            lblTituloModal.Text = "Lançamento Detalhe";
            txtdtLancamento.ReadOnly = true;
            txtsDscLancamento.ReadOnly = true;
            txtnValor.ReadOnly = true;
            ddlidUsuario.Attributes.Add("disabled", "disabled");
            //ddlidCategoriaPagar.Attributes.Add("disabled", "disabled");
            //ddlidContabil.Attributes.Add("disabled", "disabled");
            //ddlsTipo.Attributes.Add("disabled", "disabled");
            ddlidCartao.Attributes.Add("disabled", "disabled");
            Controle_CategoriasCC.CentroDeCusto.Attributes.Add("disabled", "disabled");
            //btnSalvar.Visible = false;
            div_InserirProdutos.Visible = false;
        }

        private void ExcluirProdutoLancamento(int idLancamentoItem)
        {
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "EXCLUIR_PRODUTO_LANCAMENTO");
            vParametros.Add("@idLancamentoItem", idLancamentoItem.ToString());
            vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
            DataTable dt = BD.ExecutarDataTable(sProcedure, vParametros);

        }

        private void CalculaTotal()
        {
            decimal total = 0;

            if (dtgv_produtosLancamento.Rows.Count > 0)
            {
                string valorGrid;
                foreach (GridViewRow row in dtgv_produtosLancamento.Rows)
                {
                    if (swtCompraInternacional.Recuperar() == "N")
                    {
                        valorGrid = row.Cells[5].Text.Replace("R$", "").Trim();
                    }
                    else
                    {
                        valorGrid = row.Cells[5].Text.Replace("R$", "").Trim();
                        decimal nValorComCambio = Convert.ToDecimal(valorGrid) * Convert.ToDecimal(txtValorCambio.Text);
                        nValorComCambio = Math.Truncate(nValorComCambio * 100) / 100;
                        valorGrid = nValorComCambio.ToString();
                    }

                    int quantidade = Convert.ToInt32(row.Cells[4].Text.Trim());
                    if (decimal.TryParse(valorGrid, NumberStyles.Currency, CultureInfo.GetCultureInfo("pt-BR"), out decimal valor))
                    {
                        valor *= quantidade;
                        total += valor;
                    }
                }
            }
            total = Math.Truncate(total * 100) / 100;
            txtnValor.Text = total.ToString("N2");
        }

        #endregion

        #region | Validação e Limpa campos

        private void LimpaCampos()
        {
            txtdtLancamento.Text = "";
            ddlidCartao.SelectedValue = "0";
            txtsDscLancamento.Text = "";
            ddlidCategoriaPagar.SelectedValue = "0";
            ddlsTipo.SelectedValue = "0";
            txtnValor.Text = "";
        }

        private bool ValidaDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (txtdtLancamento.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma Data!";
            }

            if (ddlidCartao.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o cartão!";
            }

            if (txtsDscLancamento.Text.Length < 5)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Descrição do lançamento deve ter ao menos 5 caracteres";
            }

            if (ddlidCategoriaPagar.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma categoria do lançamento!";
            }

            if (ddlsTipo.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o tipo do lançamento!";
            }

            if (txtnValor.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira o valor do lançamento!";
            }

            //Thiago Rodrigues - 02/09/2025
            if (Controle_CategoriasCC.CentroDeCusto.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Centro de Custo!";
            }

            if (ddlidUsuario.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Usuário!";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPaginaModalDetalhe.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        private void LimpaCamposLancamentoProduto()
        {
            txtsDscProduto.Text = "";
            txtnQuantidadeProduto.Text = "";
            txtnValorProduto.Text = "";
            ddlsUnidadeProduto.SelectedValue = "0";
            txtsCodigoProduto.Text = "";
        }

        private bool ValidarDadosProdutosLancamento()
        {
            bool bRetorno = true;
            string sMensagemErro = "";           

            if (txtsDscProduto.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma Descrição!";
            }

            if (ddlsUnidadeProduto.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma Unidade!";
            }

            if (txtnQuantidadeProduto.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma Quantidade!";
            }

            if (txtnValorProduto.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira o Valor unitário";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPaginaModalDetalhe.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        #endregion

        #region | Popula

        private void PopularProdutoLancamento(DataSet ds)
        {
            bs_Lancamento_Produtos.Clear();
            if (ds.Tables[2].Rows.Count > 0)
            {
                int idLinha = 0;
                foreach (DataRow row in ds.Tables[2].Rows)
                {
                    cls__Cartao_Lancamento_Produtos objItem = new cls__Cartao_Lancamento_Produtos();
                    objItem.idLinha = idLinha + 1;
                    objItem.idLancamentoItem = Convert.ToInt32(row["idLancamentoItem"]);
                    objItem.idProduto = row["idProduto"].ToString();
                    objItem.sDscProduto = row["sDscProduto"].ToString();
                    objItem.nQuantidade = Convert.ToInt32(row["nQuantidade"]);
                    objItem.nValorUnitario = Convert.ToDecimal(row["nValorUnitario"]);
                    objItem.sCodProduto = row["sCodigo"].ToString();
                    objItem.sUnidade = row["sUnidade"].ToString();

                    decimal nValorTotal = Convert.ToInt32(row["nQuantidade"]) * Convert.ToDecimal(row["nValorUnitario"]);
                    objItem.nValorTotal = nValorTotal;

                    bs_Lancamento_Produtos.Add(objItem);
                    idLinha++;
                }

                dtgv_produtosLancamento_DataBind();
                CalculaTotal();
            }

        }

        void Popular_Aba_Historico(DataSet ds)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(gv_Historico, ds.Tables[1], 0, "desc"), true);
        }

        private void PopulaCombo()
        {
            string idUsuario = Identity.Variaveis.idUsuario();
            if (Funcoes.ValidaPermissao(Permissao.Financeiro.Cartoes.VisualizarTudo)) idUsuario = "0";

            Funcoes.Popula_Combo(ddlidCartao, "sp_Select 'tbl_Flow_Adm_ContasBancarias_x_Cartao', @idUsuario = " + idUsuario + "", "idCartao", "sDscCartao", false, "Selecione o Cartão", "0");

            Funcoes.Popula_Combo(ddlidCategoriaPagar, "sp_Select 'Flow_Adm_Contas_Pagar_Categoria'", "idCategoriaPagar", "sDscCategoriaPagar", false, "Selecione a Categoria", "0");
            Funcoes.Popula_Combo(ddlidContabil, "sp_Select 'Flow_CodigoContabil'", "idContabil", "sDscCodContabil", false, "Selecione o Código Contábil", "0");
            Funcoes.Popula_Combo(ddlidUsuario, "sp_Select 'Usuarios'", "idUsuario", "sDscUsuario", false, "Selecione o Usuário", "0");
            Funcoes.Popula_Combo(Controle_CategoriasCC.CentroDeCusto, "sp_Select 'Flow_Adm_CentroDeCusto'", "idCentroDeCusto", "sDescricao", false, "Selecione um Centro de Custo", "0");
            Funcoes.Popula_Combo(ddlsUnidadeProduto, "sp_Select 'Flow_Produtos_Unidade'", "sUnidade", "sDscUnidade", false, "Selecione uma Unidade", "0");
            Funcoes.Popula_Combo(ddlMoeda, "sp_Select 'tbl_Flow_Tipo_Moeda'", "idTipoMoeda", "sDscTipoMoeda", false, "Selecione uma Moeda", "0");

        }

        #endregion

        #region | Script

        private void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<script type='text/javascript'>");

            sb.AppendLine("$(document).ready(function () {");
            sb.AppendLine("    $('[id*=txtnValor]').mask('0.000.000.009,99', { reverse: true });");
            sb.AppendLine("    $('[id*=txtValorCambio]').mask('0.000.000.009,99', { reverse: true });");
            sb.AppendLine("    function configurarAutoComplete(campoBusca, containerSugestoes) {");
            sb.AppendLine("        $(campoBusca).on('input', function() {");
            sb.AppendLine("             var termo = $(this).val().trim();");
            sb.AppendLine("             var $suggestions = $(containerSugestoes); ");
            sb.AppendLine("             if (termo.length < 2) {");
            sb.AppendLine("                 $suggestions.hide();");
            sb.AppendLine("                 return;");
            sb.AppendLine("             }");
            sb.AppendLine("             $.ajax({");
            sb.AppendLine("                 type:\"POST\",");
            sb.AppendLine("                 url: '/app/Paginas/Adm/Financeiro/Cartoes.aspx/BuscarProdutos',");
            sb.AppendLine("                 data: JSON.stringify({ termo: termo}),");
            sb.AppendLine("                 contentType: \"application/json; charset=utf-8\",");
            sb.AppendLine("                 dataType: \"json\",");
            sb.AppendLine("                 success: function (response) {");
            sb.AppendLine("                     var produtos = response.d;");
            sb.AppendLine("                     if (produtos.length > 0) {");
            sb.AppendLine("                         var html = '';");
            sb.AppendLine("                         produtos.forEach(function(produto) {");
            sb.AppendLine("                             html += '<div class=\\\"suggestion-item\\\" ' +\r\n'data-id=\\\"' + produto.Id + '\\\" ' +\r\n'data-codigo=\\\"' + produto.Codigo + '\\\" ' +\r\n'data-descricao=\\\"' + produto.Descricao + '\\\" ' +\r\n'data-unidade=\\\"' + produto.Unidade + '\\\">' +\r\nproduto.Codigo + ' - ' + produto.Descricao + '</div>';");
            sb.AppendLine("                         });");
            sb.AppendLine("                         $suggestions.html(html).show();");
            sb.AppendLine("                     } else { ");
            sb.AppendLine("                         $suggestions.hide();");
            sb.AppendLine("                     }");
            sb.AppendLine("                 }");
            sb.AppendLine("             });");
            sb.AppendLine("         });");
            sb.AppendLine("         $(containerSugestoes).on('click', '.suggestion-item', function() {");
            sb.AppendLine("             var $item = $(this);");
            sb.AppendLine($"            $('#{txtsCodigoProduto.ClientID}').val($item.data('codigo'));");
            sb.AppendLine($"            $('#{txtsDscProduto.ClientID}').val($item.data('descricao'));");
            sb.AppendLine($"            $('#{hddidProduto.ClientID}').val($item.data('id'));");
            sb.AppendLine($"            $('#{ddlsUnidadeProduto.ClientID}').val($item.data('unidade'));");
            sb.AppendLine("             $(containerSugestoes).hide();");
            sb.AppendLine("         });");
            sb.AppendLine("     }");
            sb.AppendLine($"    configurarAutoComplete('#{txtsCodigoProduto.ClientID}', '#suggestionsContainerCodigo');");
            sb.AppendLine($"    configurarAutoComplete('#{txtsDscProduto.ClientID}', '#suggestionsContainerDescricao');");
            sb.AppendLine("     $(document).on('click', function(e) {");
            sb.AppendLine($"         if (!$(e.target).closest('#{txtsCodigoProduto.ClientID}').length)" + " {");
            sb.AppendLine("             $('#suggestionsContainerCodigo').hide();");
            sb.AppendLine("         }");
            sb.AppendLine($"         if (!$(e.target).closest('#{txtsDscProduto.ClientID}').length)" + " {");
            sb.AppendLine("             $('#suggestionsContainerDescricao').hide();");
            sb.AppendLine("         }");
            sb.AppendLine("     });");
            sb.AppendLine("});");

            sb.AppendLine("$v192('[id*=SwitchParcelado_idSwitch]').click(function(e) {");
            sb.AppendLine("     var div = '#' + $(this).attr('id').replace('SwitchParcelado_idSwitch', 'SwitchParcelado_hddSwitch');");
            sb.AppendLine("     var currentValue = $(div).val();"); 
            sb.AppendLine("     if (currentValue === 'N') {");
            sb.AppendLine("         $(div).val('S');"); 
            sb.AppendLine("         $('#" + div_qtdParcela.ClientID + "').removeClass('visible');"); 
            sb.AppendLine("     } else {");
            sb.AppendLine("         $(div).val('N');"); 
            sb.AppendLine("         $('#" + div_qtdParcela.ClientID + "').addClass('visible');"); 
            sb.AppendLine("     }");
            sb.AppendLine("});");

            sb.AppendLine("$v192('[id*=swtCompraInternacional_idSwitch]').click(function(e) {");
            sb.AppendLine("     var div = '#' + $(this).attr('id').replace('swtCompraInternacional_idSwitch', 'swtCompraInternacional_hddSwitch');");
            sb.AppendLine("     var currentValue = $(div).val();");
            sb.AppendLine("     if (currentValue === 'N') {");
            sb.AppendLine("         $(div).val('S');");
            sb.AppendLine("         $('#" + div_compraInternacional.ClientID + "').removeClass('visible');");
            sb.AppendLine("     } else {");
            sb.AppendLine("         $(div).val('N');");
            sb.AppendLine($"        document.getElementById('{ddlMoeda.ClientID}').value = '0';");
            //sb.AppendLine($"        var ddl = document.getElementById('{ddlMoeda.ClientID}');");
            //sb.AppendLine($"        ddl.value = '0'");
            //sb.AppendLine($"        ddl.dispatchEvent(new Event('change'));");
            sb.AppendLine($"        $('#{txtValorCambio.ClientID}').val('');");
            sb.AppendLine($"        $('#{txtnValor.ClientID}').val('{nValorTotalFixo}');");
            sb.AppendLine("         $('#" + div_compraInternacional.ClientID + "').addClass('visible');");
            sb.AppendLine("     }");
            sb.AppendLine("});");

            sb.AppendLine("</script>");


            ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptLancamentoCartao", sb.ToString(), false);
        }

        #endregion

        #region | Grid

        protected void ddlidCategoriaPagar_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idCategoriaTipo_Consulta = 0;
            try
            {
                DataSet dsContabil;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "Autoselecao_Contabil");
                vParametros.Add("@idCategoriaPagar", ddlidCategoriaPagar.SelectedValue);

                dsContabil = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Pagar", vParametros);

                if (BD.ValidarDataSet(dsContabil))
                {
                    ddlidContabil.SelectedValue = Retorno.DATASET(dsContabil, 0, "idContabil");
                    idCategoriaTipo_Consulta = Convert.ToInt32(Retorno.DATASET(dsContabil, 0, "idCategoriaTipo"));

                }
            }
            catch
            {
                ddlidContabil.SelectedValue = "";
            }

            hddTipoCategoria.Value = idCategoriaTipo_Consulta.ToString();
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Literal litStatus = e.Row.FindControl("litStatus") as Literal;
            if (litStatus != null)
            {
                string sConciliacao = DataBinder.Eval(e.Row.DataItem, "sConcilado").ToString();

                if ((!string.IsNullOrEmpty(sConciliacao) && sConciliacao != DBNull.Value.ToString()))
                {
                    litStatus.Text = "Fatura Gerada";
                }
                else
                {
                    litStatus.Text = "Pendente";
                }
            }
        }       

        protected void gv_Historico_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[3].Text = HttpUtility.HtmlDecode(e.Row.Cells[3].Text);
            }
        }

        private void dtgv_produtosLancamento_DataBind()
        {
            int idLinha = 0;
            foreach(var item in bs_Lancamento_Produtos)
            {
                item.idLinha = idLinha + 1;
                idLinha++;
            }

            dtgv_produtosLancamento.DataSource = bs_Lancamento_Produtos.OrderBy(x => x.idLinha).ToList();
            dtgv_produtosLancamento.DataBind();

            if (bs_Lancamento_Produtos.Count > 0)
                txtnValor.ReadOnly = true;
            else
                txtnValor.ReadOnly = false;

        }

        #endregion

        #region | Eventos

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                string sErro = "";
                DataSet ds;

                if (hddidLancamento.Value == "0")
                {
                    if (ValidaDados())
                    {
                        Dictionary<String, String> vParametros = new Dictionary<string, string>();
                        vParametros.Add("@sFuncao", "SALVAR_LANCAMENTO_CARTAO");
                        vParametros.Add("@dtLancamento", DateTime.Parse(txtdtLancamento.Text).ToString());
                        vParametros.Add("@sDscLancamento", txtsDscLancamento.Text);
                        vParametros.Add("@idCategoria", ddlidCategoriaPagar.SelectedValue);
                        vParametros.Add("@idCodigoContabil", ddlidContabil.SelectedValue);
                        vParametros.Add("@sTipoLancamento", ddlsTipo.SelectedValue);

                        if (ddlsTipo.SelectedValue == "C" && SwitchParcelado.Recuperar() == "S")
                            vParametros.Add("@nQuantidade", txtnQuantidade.Text);

                        if (swtCompraInternacional.Recuperar() == "S")
                        {
                            vParametros.Add("@idTipoMoeda", ddlMoeda.SelectedValue);
                            vParametros.Add("@nValorCambio", txtValorCambio.Text.Replace(".", "").Replace(",", "."));                            
                        }

                        vParametros.Add("@sInternacional", swtCompraInternacional.Recuperar());
                        vParametros.Add("@nVlrLancamento", txtnValor.Text.Replace(".", "").Replace(",", "."));
                        vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
                        vParametros.Add("@idUsuario", ddlidUsuario.SelectedValue);
                        vParametros.Add("@idCartao", ddlidCartao.SelectedValue);

                        //Thiago Rodrigues - 02/09/2025
                        vParametros.Add("@idCentroDeCusto", Controle_CategoriasCC.CentroDeCusto.SelectedValue);
                        vParametros.Add("@idRegistroCategoria", Controle_CategoriasCC.CategoriaCC.SelectedValue);
                        vParametros.Add("@sSituacao", "S");

                        ds = BD.ExecutarDataSet(sProcedure, vParametros, false);
                        if (BD.ValidarDataSet(ds, out sErro))
                        {
                            hddidLancamento.Value = Retorno.DATASET(ds, "idLancamento");
                            string idVinculoParcela = Retorno.DATASET(ds, "idVinculoParcela");

                            foreach (var item in bs_Lancamento_Produtos)
                            {
                                vParametros.Clear();
                                vParametros.Add("@sFuncao", "SALVAR_PRODUTOS_LANCAMENTO");
                                vParametros.Add("@idLancamento", idVinculoParcela != "0" ? idVinculoParcela : hddidLancamento.Value);
                                vParametros.Add("@nQuantidade", item.nQuantidade.ToString());
                                vParametros.Add("@idProduto", item.idProduto.ToString());
                                vParametros.Add("@nValorUnitario", item.nValorUnitario.ToString().Replace(".", "").Replace(",","."));
                                vParametros.Add("@sDscProduto", item.sDscProduto.ToString());
                                vParametros.Add("@sCodigo", item.sCodProduto.ToString());
                                vParametros.Add("@sUnidade", item.sUnidade.ToString());
                                ds = BD.ExecutarDataSet(sProcedure, vParametros);
                            }

                            //Thiago Rodrigues - 02/09/2025
                            if (Controle_CategoriasCC.AtualizarSaldoCategoria(txtnValor))
                            {
                                MensagemPaginaModalDetalhe.MostraMensagem("<b>Informação: </b> O Saldo nas Categorias do Centro de Custos Selecionado foi Atualizado.", "info", false);
                            }
                            else
                            {
                                MensagemPaginaModalDetalhe.MostraMensagem_Erro("Erro ao atualizar saldo da categoria de centro de custo.");
                            }

                            LancamentoDetalhe();
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_FechaModal", "refreshPagina();", true);
                            //MensagemPagina.MostraMensagem_Sucesso("Lançamento Salvo com sucesso!");
                        }
                    }
                }
                else
                {
                    if (ValidaDados())
                    {
                        Dictionary<String, String> vParametros = new Dictionary<string, string>();
                        vParametros.Add("@sFuncao", "SALVAR_LANCAMENTO_CARTAO");
                        vParametros.Add("@idLancamento", hddidLancamento.Value);
                        vParametros.Add("@dtLancamento", DateTime.Parse(txtdtLancamento.Text).ToString());
                        vParametros.Add("@sDscLancamento", txtsDscLancamento.Text);
                        vParametros.Add("@idCategoria", ddlidCategoriaPagar.SelectedValue);
                        vParametros.Add("@idCodigoContabil", ddlidContabil.SelectedValue);
                        vParametros.Add("@sTipoLancamento", ddlsTipo.SelectedValue);
                        vParametros.Add("@nVlrLancamento", txtnValor.Text.Replace(".", "").Replace(",", "."));
                        vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
                        vParametros.Add("@idUsuario", ddlidUsuario.SelectedValue);
                        vParametros.Add("@idCartao", ddlidCartao.SelectedValue);

                        //Thiago Rodrigues - 02/09/2025
                        vParametros.Add("@idCentroDeCusto", Controle_CategoriasCC.CentroDeCusto.SelectedValue);
                        vParametros.Add("@idRegistroCategoria", Controle_CategoriasCC.CategoriaCC.SelectedValue);


                        ds = BD.ExecutarDataSet(sProcedure, vParametros, false);
                        if (BD.ValidarDataSet(ds, out sErro))
                        {
                            hddidLancamento.Value = Retorno.DATASET(ds, "idLancamento");
                            string idVinculoParcela = Retorno.DATASET(ds, "idVinculoParcela");

                            foreach (var item in bs_Lancamento_Produtos)
                            {
                                vParametros.Clear();
                                vParametros.Add("@sFuncao", "SALVAR_PRODUTOS_LANCAMENTO");
                                vParametros.Add("@idLancamento", !string.IsNullOrEmpty(idVinculoParcela) ? idVinculoParcela : hddidLancamento.Value);
                                vParametros.Add("@idLancamentoItem", item.idLancamentoItem.ToString());
                                vParametros.Add("@nQuantidade", item.nQuantidade.ToString());
                                vParametros.Add("@idProduto", item.idProduto.ToString());
                                vParametros.Add("@nValorUnitario", item.nValorUnitario.ToString().Replace(".", "").Replace(",", "."));
                                vParametros.Add("@sDscProduto", item.sDscProduto.ToString());
                                vParametros.Add("@nVlrLancamento", txtnValor.Text.Replace(".", "").Replace(",", "."));
                                vParametros.Add("@sCodigo", item.sCodProduto.ToString());
                                vParametros.Add("@sUnidade", item.sUnidade.ToString());
                                vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
                                ds = BD.ExecutarDataSet(sProcedure, vParametros);
                            }

                            //Thiago Rodrigues - 02/09/2025
                            if (Controle_CategoriasCC.AtualizarSaldoCategoria(txtnValor))
                            {
                                MensagemPaginaModalDetalhe.MostraMensagem("<b>Informação: </b> O Saldo nas Categorias do Centro de Custos Selecionado foi Atualizado.", "info", false);
                            }
                            else
                            {
                                MensagemPaginaModalDetalhe.MostraMensagem_Erro("Erro ao atualizar saldo da categoria de centro de custo.");
                            }

                            LancamentoDetalhe();
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalDetalhe", "$('#modalLancamentoDetalhe').modal('show');", true);
                            MensagemPaginaModalDetalhe.MostraMensagem_Sucesso("Lançamento Editado com sucesso!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPaginaModalDetalhe.MostraMensagem_Erro("Erro ao salvar: " + ex.Message);
            }
        }

        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            PopulaCombo();
            LimpaCampos();

            hddidLancamento.Value = "0";

            ddlidCartao.SelectedValue = hddidCartao.Value;
            ddlidUsuario.SelectedValue = Identity.Variaveis.idUsuario();
            ddlidUsuario.Attributes.Add("disabled", "disabled");
            if (Funcoes.ValidaPermissao(Permissao.Financeiro.Cartoes.VisualizarTudo))
            {
                ddlidUsuario.Attributes.Remove("disabled");
            }

            lblTituloModal.Text = "Novo Lançamento";
            SwitchParcelado.Definir("N", "Parcelado?", "S");
            swtCompraInternacional.Definir("N", "Compra Internacional?", "N");
            div_compraInternacional.Attributes["class"] = "visible";
            txtnValor.Text = "0,00";
            nValorTotalFixo = txtnValor.Text;

            txtdtLancamento.ReadOnly = false;
            txtsDscLancamento.ReadOnly = false;
            txtnValor.ReadOnly = false;
            ddlidCategoriaPagar.Attributes.Remove("disabled");
            ddlidContabil.Attributes.Add("disabled", "disabled");
            ddlsTipo.Attributes.Remove("disabled");
            ddlidCartao.Attributes.Remove("disabled");
            btnSalvar.Visible = true;
            txtnQuantidade.ReadOnly = true;
            aba_Historico.Visible = false;
            div_btnExcluir.Visible = false;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalNovo", "$('#modalLancamentoDetalhe').modal('show');", true);
        }

        protected void cmd_LancamentosCartao(object sender, CommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();
            hddidLancamento.Value = id;
            PopulaCombo();
            LancamentoDetalhe();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalDetalhe", "$('#modalLancamentoDetalhe').modal('show');", true);
        }

        protected void cmd_Cartao(object sender, CommandEventArgs e)
        {
            hddidCartao.Value = e.CommandArgument.ToString();
            hddMudaGrid.Value = "1";
            Pesquisar();
        }

        protected void btnVoltarGrid_Click(object sender, EventArgs e)
        {
            hddidCartao.Value = "0";
            hddMudaGrid.Value = "0";
            Pesquisar();
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            div_Acao_Excluir.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao", "$('#modalAcao').modal('show');", true);
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            string sErro = "";
            DataSet ds;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "EXCLUIR_LANCAMENTO");
            vParametros.Add("@idLancamento", hddidLancamento.Value);
            vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
            ds = BD.ExecutarDataSet(sProcedure, vParametros, false);
            if (BD.ValidarDataSet(ds, out sErro))
            {
                Session["Excluir"] = "S";
                Session["Mensagem"] = "Lançamento Excluído com sucesso!";
                string url = "/App/Paginas/Adm/Financeiro/Cartoes.aspx?grid=" + hddMudaGrid.Value + "&cartao=" + hddidCartao.Value;
                Response.Redirect(url);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Session["ItemId"] = hddidLancamento.Value;
            Session["OpenModal"] = "modalLancamentoDetalhe";
            Response.Redirect(Request.RawUrl);
        }

        protected void lnkDescricao_Editar_Click(object sender, EventArgs e)
        {

            LinkButton btn = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            HiddenField hddidLinha = (HiddenField)row.FindControl("hddidLinha");
            string idLinha = hddidLinha.Value;

            var item = bs_Lancamento_Produtos.FirstOrDefault(x => x.idLinha.ToString() == idLinha);
            if (item != null)
            {
                txtsDscProduto.Text = item.sDscProduto.ToString();
                txtnQuantidadeProduto.Text = item.nQuantidade.ToString();
                txtnValorProduto.Text = item.nValorUnitario.ToString();
                txtsCodigoProduto.Text = item.sCodProduto.ToString();
                ddlsUnidadeProduto.SelectedValue = item.sUnidade.ToString().Trim() == "" ? "0" : item.sUnidade.ToString();
                hddidLancamentoItem.Value = item.idLancamentoItem.ToString();

                bs_Lancamento_Produtos.Remove(item);
            }

            dtgv_produtosLancamento_DataBind();
            CalculaTotal();
        }

        protected void lnkDescricao_Excluir_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            HiddenField hddidLinha = (HiddenField)row.FindControl("hddidLinha");
            string idLinha = hddidLinha.Value;

            var item = bs_Lancamento_Produtos.FirstOrDefault(x => x.idLinha.ToString() == idLinha);

            if (item.idLancamentoItem != 0)
            {
                ExcluirProdutoLancamento(item.idLancamentoItem);
            }

            bs_Lancamento_Produtos.Remove(item);

            dtgv_produtosLancamento_DataBind();
            CalculaTotal();
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalDetalhe", "$('#modalLancamentoDetalhe').modal('show');", true);
        }

        protected void btnIncluirProduto_Click(object sender, EventArgs e)
        {

            if (ValidarDadosProdutosLancamento())
            {

                cls__Cartao_Lancamento_Produtos objItem = new cls__Cartao_Lancamento_Produtos();
                
                objItem.idLinha = bs_Lancamento_Produtos.Count + 1;                
                objItem.idLancamentoItem = hddidLancamentoItem.Value != "" ? Convert.ToInt32(hddidLancamentoItem.Value) : 0;
                objItem.idProduto = hddidProduto.Value;
                objItem.sCodProduto = txtsCodigoProduto.Text;
                objItem.sDscProduto = txtsDscProduto.Text;
                objItem.nQuantidade = Convert.ToInt32(txtnQuantidadeProduto.Text);
                objItem.nValorUnitario = Convert.ToDecimal(txtnValorProduto.Text);
                objItem.sUnidade = ddlsUnidadeProduto.SelectedValue;
                objItem.nValorTotal = Convert.ToInt32(txtnQuantidadeProduto.Text) * Convert.ToDecimal(txtnValorProduto.Text);

                bs_Lancamento_Produtos.Add(objItem);

                LimpaCamposLancamentoProduto();
                dtgv_produtosLancamento_DataBind();
                CalculaTotal();
            }

            hddidLancamentoItem.Value = "";
            hddidProduto.Value = "0";
        }

        protected void ddlsTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlsTipo.SelectedValue == "C")
            {
                SwitchParcelado.Definir("N", "Parcelado?", "");
                txtnQuantidade.ReadOnly = false;
                txtnQuantidade.Text = "";
            }
            else
            {
                SwitchParcelado.Definir("N", "Parcelado?", "S");
                txtnQuantidade.ReadOnly = true;
            }

        }

        protected void txtValorCambio_TextChanged(object sender, EventArgs e)
        {
            if (txtValorCambio.Text != "")
            {
                decimal nValorCambio = Convert.ToDecimal(txtValorCambio.Text);
                decimal nValorTotal = txtnValor.Text != "" ? Convert.ToDecimal(nValorTotalFixo) * nValorCambio : 0;
                nValorTotal = Math.Truncate(nValorTotal * 100) / 100;
                txtnValor.Text = nValorTotal.ToString("N2");
            }
        }

        protected void ddlMoeda_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlMoeda.SelectedValue != "0")
            {
                DataSet ds;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR_MOEDA_CAMBIO");
                vParametros.Add("@idTipoMoeda", ddlMoeda.SelectedValue);
                ds = BD.ExecutarDataSet(sProcedure, vParametros, false);

                decimal nValorCambio = Convert.ToDecimal(Retorno.DATASET(ds, 0, "nValorCambio"));

                if (nValorCambio != 0) 
                {
                    txtValorCambio.Text = nValorCambio.ToString("N2");
            
                    decimal nValorTotal = Convert.ToDecimal(nValorTotalFixo) * nValorCambio;
                    nValorTotal = Math.Truncate(nValorTotal * 100) / 100;
                    txtnValor.Text = nValorTotal.ToString("N2");
                }
                else
                {
                    MensagemPaginaModalDetalhe.MostraMensagem_Aviso("Moeda selecionada não possui valor de câmbio cadastrado, favor inserir manualmente um valor no campo <b>Valor Câmbio</b>");
                }

                txtnValor.ReadOnly = true;
                div_compraInternacional.Attributes["class"] = "";
            }
            else
            {
                txtnValor.ReadOnly = false;
            }
        }

        protected void txtnValor_TextChanged(object sender, EventArgs e)
        {
            //if (txtnValor.Text != "")
            //{
            //    swtCompraInternacional.Definir("N", "Compra Internacional?", "N");
            nValorTotalFixo = txtnValor.Text;
            //}
            //else
            //{
            //    swtCompraInternacional.Definir("N", "Compra Internacional?", "S");
            //}

        }

        #endregion

        #region | WebMethod

        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public static List<Produto> BuscarProdutos(string termo)
        {
            List<Produto> produtos = new List<Produto>();

            SqlDataReader sdr = BD.ExecutarDataReader("sp_Select 'Flow_BuscaProdutos', 0, 'S', '" + termo + "'");

            while (sdr.Read())
            {
                produtos.Add(new Produto()
                {
                    Id = sdr.GetInt32(0),
                    Codigo = sdr["sCodigo"].ToString(),
                    Descricao = sdr["sDscProduto"].ToString(),
                    Unidade = sdr["sUnidade"].ToString()
                });
            }

            return produtos;
        }



        #endregion

        
    }

    public class Produto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string Unidade { get; set; }
    }
}