using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using TT_Flow.FrameWork;
using TT_Flow.App.Controles;
using TT.FrameWork;
using System.Linq;
using Identity = TT.FrameWork.Identity;
using System.Text;

namespace TT_Flow.App.Paginas.WMS.Manutencao
{
    public partial class ConsultaSaldo : Page
    {
        string sTituloPagina = "Consulta de Saldo/Produtos";
        string sPagina_NovoRegistro = "app/Paginas/WMS/ConsultaSaldo_Detalhe.aspx?id=0";
        string sTipoVisualizacao = "N";

        public List<cls_WMS_Produtos> bs_Consulta_Saldo
        {
            get
            {
                if (ViewState["bs_Consulta_saldo"] == null)
                {
                    ViewState["bs_Consulta_saldo"] = new List<cls_WMS_Produtos>();
                }
                return (List<cls_WMS_Produtos>)ViewState["bs_Consulta_saldo"];
            }
            set
            {
                ViewState["bs_Consulta_saldo"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.WMS.ConsultarSaldo.Consultar, true);

            if (!IsPostBack)
            {
                //ddlSituacaoCadastral.Items.Add(new ListItem("Edição de Saldo de Produtos", "EDICAO", true));

                if (!FUNCOES.ValidaPermissao(Permissao.WMS.ConsultarSaldo.ImportarSaldos))
                    cmdNovoCadastro.Visible = false;

                pnResultado.Visible = false;
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                idCampoSalvar.Visible = false;
                cmdEditar.Visible = false;
                PopulaCombo(0);
            }

            RegistraScript();
        }

        protected void Pesquisar()
        {
            pnResultado.Visible = false;
            cmdEditar.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_SALDO" },
                { "@sDscProduto", txtPesquisa.Text.Trim() },
                { "@sCodigo", txtPesquisa.Text.Trim() },
                { "@idTipoProduto", ddlTipoProduto.SelectedValue },
                { "@idFamilia", ddlFamilia.SelectedValue },
                { "@idGrupo", ddlGrupo.SelectedValue },
                { "@idPais", ddlPaisOrigem.SelectedValue },
                { "@sIdLocalArmazenamento", string.Join("|", lstLocalArmazenamento.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value)) },
                { "@sSaldoMinimo", ckbSaldoMinimo.Checked ? "S" : "N" },
                { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() },
                { "@sSituacao", "S" }
            };

            if (sTipoVisualizacao == "EDICAO")
                vParametros.Add("@sSituacaoCadastral", "");
            else
                vParametros.Add("@sSituacaoCadastral", ddlSituacaoCadastral.SelectedValue);

            DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Produtos", vParametros, false);

            if (sTipoVisualizacao == "EDICAO")
            {
                idCampoSalvar.Visible = true;
            }

            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ddlSituacaoCadastral.SelectedValue == "")
                {
                    if (FUNCOES.ValidaPermissao(Permissao.WMS.ConsultarSaldo.Editar))
                        cmdEditar.Visible = true;
                }
                    

                new cls_WMS_Produtos().ConverterTabela(ds, bs_Consulta_Saldo, sTipoVisualizacao == "EDICAO" ? sTipoVisualizacao : ddlSituacaoCadastral.SelectedValue.ToString());
                //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", Grid.DataBindComScript(dtgvConsulta, bs_Consulta_Saldo, 1, "asc"), true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables_consulta", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, bs_Consulta_Saldo, false, 1, "asc", "true", "800"), true);
                pnResultado.Visible = true;
            }
            else
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");
        }

        void ObterDadosAtuaisGridView()
        {
            try
            {
                int nContador = 0;

                foreach (GridViewRow row in dtgvConsulta.Rows)
                {
                    TextBox nEstoqueAtual = (TextBox)row.FindControl("nEstoqueAtual");
                    string valorAtualizado = nEstoqueAtual.Text;
                    decimal valorCompatibilizado = Convert.ToDecimal(valorAtualizado);

                    if (valorCompatibilizado != bs_Consulta_Saldo[nContador].NEstoqueAtual)
                        bs_Consulta_Saldo[nContador].SFuncao = "INCLUIR_SALDO_ATUALIZADO";

                    bs_Consulta_Saldo[nContador].NEstoqueAtual = valorCompatibilizado;
                    nContador++;
                }

                if (cls_WMS_Produtos.SalvarDados(bs_Consulta_Saldo))
                    MensagemPagina.MostraMensagem_Sucesso("Dados atualizados com sucesso");
                else
                    MensagemPagina.MostraMensagem_Erro("Dados não foram atualizados, tentar novamente");

                Pesquisar();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro " + ex.Message);
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        =>
            Pesquisar();

        protected void cmdNovoCadastro_Click(object sender, EventArgs e)
        =>
            FUNCOES.DirecionaPagina(sPagina_NovoRegistro);

        protected void ddlSituacaoCadastral_SelectedIndexChanged(object sender, EventArgs e)
        {
            sTipoVisualizacao = ddlSituacaoCadastral.SelectedValue;    
            Pesquisar();
        }

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (DataBinder.Eval(e.Row.DataItem, "sCor").ToString() != "")
                    e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();
            }
            if (sTipoVisualizacao == "EDICAO")
            {
                FUNCOES.EsconderColunas(dtgvConsulta, "Estoque Atual");
                FUNCOES.EsconderColunas(dtgvConsulta, "Sugestão Compra");
                FUNCOES.ReexibirColunas(dtgvConsulta, "Atualizar Estoque");
            }
            else if (sTipoVisualizacao == "MINIMO")
            {                
                FUNCOES.ReexibirColunas(dtgvConsulta, "Sugestão Compra");
            }
            else 
            {
                FUNCOES.EsconderColunas(dtgvConsulta, "Sugestão Compra");
                FUNCOES.EsconderColunas(dtgvConsulta, "Atualizar Estoque");
                FUNCOES.ReexibirColunas(dtgvConsulta, "Estoque Atual");
            }
        }

        protected void PopulaCombo(int tipo)
        {
            FUNCOES.Popula_Combo(ddlTipoProduto, "sp_Select 'Flow_Produtos_Tipo', @idPesquisa=" + tipo, "idTipoProduto", "sDscTipoProduto", false, "Todos os Tipos", "0");
            FUNCOES.Popula_Combo(ddlFamilia, "sp_Select 'Flow_WMS_Produtos_Familia'", "idFamilia", "sDscFamilia", false, "Todas as Famílias", "0");
            FUNCOES.Popula_Combo(ddlGrupo, "sp_Select 'Flow_WMS_Produtos_Grupos_PAI'", "idGrupo", "sDscGrupo", false, "Todos os Grupos", "0");
            FUNCOES.Popula_Combo(ddlPaisOrigem, "sp_Select 'tbl_Flow_WMS_Produtos_Origem'", "idPais", "sDscPais", false, "Todos Países de Origem", "0");
            FUNCOES.Popula_Combo(lstLocalArmazenamento, "sp_Select 'Flow_WMS_Produtos_Local_Armazenamento'", "idLocalArmazenamento", "sDscLocalArmazenamento", false);

        }

        private void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<script type='text/javascript'>");
            sb.AppendLine("$(document).ready(function () {");
            sb.AppendLine("    function applyMaskToElement(element) {");
            sb.AppendLine("        var $el = $(element);");
            sb.AppendLine("        var value = $el.val();");
            sb.AppendLine("        if (!value) return;");
            sb.AppendLine("");
            sb.AppendLine("        var isNegative = value.trim().startsWith('-');");
            sb.AppendLine("        var numericValue = value.replace(/[^0-9,]/g, '');");
            sb.AppendLine("");
            sb.AppendLine("        $el.val(numericValue);");
            sb.AppendLine("        $el.mask('0.000.000.009,9999', { reverse: true });");
            sb.AppendLine("");
            sb.AppendLine("        setTimeout(function() {");
            sb.AppendLine("            var maskedValue = $el.val();");
            sb.AppendLine("            if (isNegative && !maskedValue.startsWith('-')) {");
            sb.AppendLine("                $el.val('-' + maskedValue);");
            sb.AppendLine("            }");
            sb.AppendLine("        }, 1);");
            sb.AppendLine("    }");
            sb.AppendLine("");
            sb.AppendLine("    $('[id*=nEstoqueAtual]').each(function() {");
            sb.AppendLine("        applyMaskToElement(this);");
            sb.AppendLine("    });");
            sb.AppendLine("");
            sb.AppendLine("    $(document).on('keypress', '[id*=nEstoqueAtual]', function(e) {");
            sb.AppendLine("        if (e.which === 45 || e.charCode === 45) { // Tecla '-'");
            sb.AppendLine("            var currentValue = $(this).val();");
            sb.AppendLine("            var newValue = currentValue.startsWith('-') ? currentValue.substring(1) : '-' + currentValue;");
            sb.AppendLine("            $(this).val(newValue);");
            sb.AppendLine("            setTimeout(() => { applyMaskToElement(this); }, 10);");
            sb.AppendLine("            return false;");
            sb.AppendLine("        }");
            sb.AppendLine("    });");
            sb.AppendLine("});");
            sb.AppendLine("</script>");


            ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptMascara", sb.ToString(), false);
        }

        protected void cmdAtualizarSaldo_Click(object sender, EventArgs e)
        =>
            ObterDadosAtuaisGridView();

        protected void cmdEditar_Click(object sender, EventArgs e)
        {
            sTipoVisualizacao = "EDICAO";
            Pesquisar();
        }

        protected void cmdCancelar_Click(object sender, EventArgs e)
        {
            sTipoVisualizacao = ddlSituacaoCadastral.SelectedValue;
            Pesquisar();
        }
    }
}