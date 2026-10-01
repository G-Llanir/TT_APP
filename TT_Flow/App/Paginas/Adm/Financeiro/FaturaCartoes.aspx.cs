using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Controles;
using TT_Flow.FrameWork;
using static Permissao.Financeiro;
using static TT.FrameWork.BD;
using static TT.FrameWork.Identity;
using Identity = TT.FrameWork.Identity;

namespace TT_Flow.App.Paginas.Adm.Financeiro
{
    public partial class FaturaCartoes : System.Web.UI.Page
    {
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_ContasBancarias";

        public List<FrameWork.cls_Detalhe_Fatura> bs_Detalhe_Fatura
        {
            get
            {
                if (ViewState["bs_Detalhe_Fatura"] == null)
                {
                    ViewState["bs_Detalhe_Fatura"] = new List<FrameWork.cls_Detalhe_Fatura>();
                }
                return (List<FrameWork.cls_Detalhe_Fatura>)ViewState["bs_Detalhe_Fatura"];
            }
            set
            {
                ViewState["bs_Detalhe_Fatura"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Funcoes.ValidaPermissao(Permissao.Financeiro.Cartoes.Fatura.Consultar, true);

            //Manual Usuario
            manual.sNomeArquivo = "Manual_FaturaCartões.pdf";

            if (!IsPostBack)
            {
                Funcoes.Popula_Combo(ddlidConta, "sp_Select 'tbl_Flow_Adm_ContasBancarias'", "idConta", "sDscConta", false, "Todos as Contas", "0");
                //Funcoes.Popula_Combo(ddlidCartao, "sp_Select 'tbl_Flow_Adm_ContasBancarias_x_Cartao', @idUsuario = \"0\"", "idCartao", "sDscCartao", false, "Todos os Cartões", "0");
                Funcoes.Popula_Combo(ddlidEmpresaFiltro, "sp_Select 'Flow_Empresa'", "idEmpresa", "sDscEmpresa", false, "Todas as Empresas", "0");

                Pesquisar();
            }

            RegistraScript();

        }

        private void Pesquisar()
        {
            string sErro = "";

            try
            {

                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR_FATURA_CARTAO");
                vParametros.Add("@sPeriodo", txtsPeriodo.Text == "" ? "0" : txtsPeriodo.Text);
                vParametros.Add("@idConta", ddlidConta.SelectedValue);
                //vParametros.Add("@idCartao", ddlidCartao.SelectedValue);
                vParametros.Add("@idEmpresa", ddlidEmpresaFiltro.SelectedValue);
                DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsPesquisa, out sErro))
                {
                    List<cls_Detalhe_Fatura> detalheFaturas = GetDetalheFaturas(dsPesquisa.Tables[0]);
                    List<cls_Fatura> faturaCartoes = GetFaturaCartoes(detalheFaturas);

                    bs_Detalhe_Fatura = detalheFaturas;

                    gv_FaturaCartao.DataSource = faturaCartoes;
                    gv_FaturaCartao.DataBind();

                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        private List<cls_Fatura> GetFaturaCartoes(List<cls_Detalhe_Fatura> detalheFaturas)
        {
            var somaTotalLancamento = detalheFaturas.GroupBy(x => new { x.idFatura, x.idCartao }).ToDictionary(g => new { g.Key.idFatura, g.Key.idCartao }, g => g.Sum(d => d.nValor));

            //var faturaCartoes = detalheFaturas
            //                    .GroupBy(mv => mv.idFatura)
            //                    .Select(g =>
            //                    {
            //                        var faturaDetalhe = g.First();

            //                        int idFatura = g.Key;
            //                        int idCartao = faturaDetalhe.idCartao;

            //                        decimal nTotalLancamento = somaTotalLancamento[new { idFatura = idFatura, idCartao = idCartao }];

            //                        return new cls_Fatura
            //                        {
            //                            //idCartao = idCartao,
            //                            idFatura = idFatura,
            //                            nTotalFatura = g.Sum(mv => mv.nValor),
            //                            lsCartoes = g.ToList(),
            //                            nTotalLancamento = nTotalLancamento,
            //                            sBanco = faturaDetalhe.sBanco,
            //                            sEmpresa = faturaDetalhe.sEmpresa,
            //                            sDscCartao = faturaDetalhe.sDscCartao,
            //                            idBanco = faturaDetalhe.idBanco,
            //                            mesAno = faturaDetalhe.mesAno
            //                        };
            //                    })
            //                    .ToList();

            var faturaCartoes = detalheFaturas
                        .GroupBy(x => x.idFatura)
                        .Select(gF =>
                        {
                            var fatura = new cls_Fatura
                            {
                                idFatura = gF.Key,
                                nTotalFatura = gF.Sum(x => x.nValor),                                
                                sBanco = gF.First().sBanco,
                                sEmpresa = gF.First().sEmpresa,                                
                                idBanco = gF.First().idBanco,
                                mesAno = gF.First().mesAno,                                
                                lsCartoes = gF                                    
                                    .GroupBy(fc => fc.idCartao)
                                    .Select(gc =>
                                    {
                                        var cartao = new cls_FaturaCartao
                                        {
                                            idFatura = gF.Key,
                                            idCartao = gc.Key,
                                            nTotalLancamento = gc.Sum(x => x.nValor),
                                            sDscCartao = gc.First().sDscCartao, 
                                            lsLancamentos = gc.ToList()
                                        };
                                        return cartao;
                                    })
                                    .ToList()
                            };

                            return fatura;
                        })
                        .ToList();


            ViewState["FaturaCartoes"] = faturaCartoes;

            return faturaCartoes;
        }

        private List<cls_Detalhe_Fatura> GetDetalheFaturas(DataTable dt)
        {
            List<cls_Detalhe_Fatura> detalheFaturas = new List<cls_Detalhe_Fatura>();

            foreach (DataRow row in dt.Rows)
            {
                cls_Detalhe_Fatura detalheFatura = new cls_Detalhe_Fatura
                {
                    idLancamento = Convert.ToInt32(row["idLancamento"]),
                    dtLancamento = row["dtLancamento"].ToString(),
                    sDscLancamento = row["sDscLancamento"].ToString(),
                    nValor = Convert.ToDecimal(row["nValor"]),
                    sDscUsuario = row["sDscUsuario"].ToString(),
                    idCartao = Convert.ToInt32(row["idCartao"]),
                    idBanco = Convert.ToInt32(row["idBanco"]),
                    sBanco = row["sBanco"].ToString(),
                    sEmpresa = row["sEmpresa"].ToString(),
                    sDscCartao = row["sDscCartao"].ToString(),
                    idFatura = Convert.ToInt32(row["idFatura"]),
                    mesAno = row["MesAno"].ToString()
                };

                detalheFaturas.Add(detalheFatura);
            }

            return detalheFaturas;
        }

        private void PopulaModalFatura(string idCartao)
        {
            string sErro = "";
            try
            {
                PopulaCombo();
                LimpaCampos();
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "POPULA_MODAL");
                vParametros.Add("@idCartao", idCartao);
                DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);
                if (BD.ValidarDataSet(dsPesquisa, out sErro))
                {
                    ddlidEmpresa.SelectedValue = Retorno.DATASET(dsPesquisa, 0, "idEmpresa");
                    ddlidMeioPagamento.SelectedValue = "1";

                    var faturaCartoes = ViewState["FaturaCartoes"] as List<cls_Fatura>;
                    var fatura = faturaCartoes?.FirstOrDefault(x => x.idFatura.ToString() == hddidFatura.Value);
                    txtnValorBruto.Text = fatura?.nTotalFatura.ToString("C");
                    txtnValorLiquido.Text = fatura?.nTotalFatura.ToString("C");

                    ddlidEmpresa.Attributes.Add("disabled", "disabled");
                    ddlidMeioPagamento.Attributes.Add("disabled", "disabled");
                    ddlidContabil.Attributes.Add("disabled", "disabled");
                    txtnValorBruto.ReadOnly = true;
                    txtnValorLiquido.ReadOnly = true;

                    //MensagemPaginaModal.MostraMensagem("", "info", false);

                }

            }
            catch { }
        }

        private void LimpaCampos()
        {
            ddlidCategoriaPagar.SelectedValue = "0";
            ddlidEmpresa.SelectedValue = "0";
            ddlidParceiro.SelectedValue = "0";
            ddlidFormaPagamento.SelectedValue = "0";
            ddlidCentroDeCusto.SelectedValue = "0";
            ddlidContabil.SelectedValue = "0";
            ddlidMeioPagamento.SelectedValue = "0";
            txtsReferencia.Text = "";
            txtdtEmissão.Text = "";
            txtsCodigo.Text = "";
            txtdtVencimento.Text = "";
            txtnValorBruto.Text = "";
            txtnValorLiquido.Text = "";
        }

        private void PopulaCombo()
        {
            Funcoes.Popula_Combo(ddlidCategoriaPagar, "sp_Select 'Flow_Adm_Contas_Pagar_Categoria'", "idCategoriaPagar", "sDscCategoriaPagar", false, "Selecione a Categoria", "0");
            Funcoes.Popula_Combo(ddlidFormaPagamento, "sp_Select 'tbl_Flow_CondicaodePagamento_Tipo'", "idTipoCondicaoPagamento", "sDscTipoCondicaoPagamento", false, "Selecione o Pagamento", "0");
            Funcoes.Popula_Combo(ddlidCentroDeCusto, "sp_Select 'Flow_Adm_CentroDeCusto'", "idCentroDeCusto", "sDescricao", false, "Selecione um Centro de Custo", "0");
            Funcoes.Popula_Combo(ddlidContabil, "sp_Select 'Flow_CodigoContabil'", "idContabil", "sDscCodContabil", false, "Selecione o Código Contábil", "0");
            Funcoes.Popula_Combo(ddlidMeioPagamento, "sp_Select 'tbl_Flow_Adm_MeioPagamento'", "idMeioPagamento", "sDscMeioPagamento", false, "Selecione um Meio", "0");
            Funcoes.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa'", "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");
            Funcoes.Popula_Combo(ddlidParceiro, "sp_Select 'Flow_Parceiro_Credor_Cliente'", "idParceiro", "sRazaoSocial", false, "Selecione um credor", "0");
        }

        private bool ValidaDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (hddTipoCategoria.Value == "4") //Internos
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "A categoria Internos não pode ser selecionada para geração de Fatura!";
            }

            if (ddlidCategoriaPagar.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Categoria!";
            }

            if (ddlidParceiro.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Credor!";
            }

            if (txtsReferencia.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma Referência!";
            }

            if (!Validacoes.ValidarData(txtdtEmissão))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data de Emissão Invalída!";
            }

            if (ddlidCentroDeCusto.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Centro de Custo!";
            }

            if (ddlidFormaPagamento.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Forma de Pagamento!";
            }

            if (ddlidEmpresa.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Empresa!";
            }

            if (txtdtVencimento.Text == "01/01/1900")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Necessário corrigir a data de corte da fatura desse cartão! Favor ir em \"Contas Bancarias > Aba Produtos Financeiros\" e edite o campo Data Corte Fatura";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPaginaModal.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        private void GerarFatura(string idTitulo)
        {
            List<FrameWork.cls_Detalhe_Fatura> detalheFaturasFiltradas = bs_Detalhe_Fatura
                                                                        .Where(d => d.idFatura == Convert.ToInt16(hddidFatura.Value))
                                                                        .ToList();
            try
            {
                DataTable dsPesquisa;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                string sConciliadoCartao = "";

                for (int i = 0; i < detalheFaturasFiltradas.Count(); i++)
                {
                    vParametros.Clear();
                    vParametros.Add("@sFuncao", "VINCULO_LANCAMENTO_CARTAO");
                    vParametros.Add("@idLancamento", detalheFaturasFiltradas[i].idLancamento.ToString());
                    vParametros.Add("@idConciliado", idTitulo);
                    dsPesquisa = BD.ExecutarDataTable(sProcedure, vParametros);

                    sConciliadoCartao += detalheFaturasFiltradas[i].idLancamento.ToString() + "|";

                }

                vParametros.Clear();
                vParametros.Add("@sFuncao", "ATUALIZA_TITULO");
                vParametros.Add("@idTitulo", idTitulo);
                vParametros.Add("@sConciliadoCartao", sConciliadoCartao);
                vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
                dsPesquisa = BD.ExecutarDataTable(sProcedure, vParametros);

                ddlidCategoriaPagar.Attributes.Add("disabled", "disabled");
                ddlidParceiro.Attributes.Add("disabled", "disabled");
                ddlidFormaPagamento.Attributes.Add("disabled", "disabled");
                ddlidCentroDeCusto.Attributes.Add("disabled", "disabled");
                ddlidMeioPagamento.Attributes.Add("disabled", "disabled");
                txtsReferencia.ReadOnly = true;
                txtdtEmissão.ReadOnly = true;
                txtsCodigo.ReadOnly = true;
                txtdtVencimento.ReadOnly = true;

                string url = string.Format("/app/Paginas/Adm/Financeiro/ContasPagar_Detalhe.aspx?id={0}", idTitulo);
                string mensagem = string.Format("Fatura gerada com sucesso! <br/>" +
                    "<a onclick=\"event.preventDefault(); window.open(this.href, '_blank');\" href='{0}'>Clique aqui para acessar o título</a>", url);

                MensagemPaginaModal.MostraMensagem_Sucesso(mensagem);

            }
            catch { }

        }

        public string NovaLinha(object id, string gridNome)
        {
            /* 
            * Passo a passo:
            * 1. Fecha a célula atual
            * 2. Fecha a linha Atual
            * 3. Cria uma nova linha com o ID e a classe <TR id='...' style='...'>
            * 4. Cria uma célula em branco: <TD></TD>
            * 5. Cria uma nova célula para conter o gridview
            ************************************************************/
            if (id != null && !string.IsNullOrEmpty(id.ToString()))
            {
                // Se houver um ID, retorna a nova linha com o ID e a classe
                return string.Format(@"</td></tr><tr id='tr{0}{1}' class='collapsed-row'>
                               <td></td><td colspan='100' style='padding:0px; margin:0px;'>", gridNome, id);
            }
            else
            {
                // Se não houver ID, retorna uma string vazia para que nada seja renderizado e o botão de colapso desapareça
                return string.Empty;
            }
        }

        private void RegistraScript()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("$('.composicaoLinha').addClass('fa fa-plus');\r\n");
            sb.Append("$('.composicaoLinha').click(function() {\r\n");
            sb.Append("     var icon = $(this);\r\n");
            sb.Append("     var divId = $(this).data('div-id');\r\n");
            sb.Append("     var current = $('#' + divId).css('display');\r\n");
            sb.Append("     if (current == 'none') {\r\n");
            sb.Append("         $('#' + divId).show('slow');\r\n");
            sb.Append("         icon.removeClass('fa fa-plus').addClass('fa fa-minus');\r\n");
            sb.Append("     } else {\r\n");
            sb.Append("         $('#' + divId).hide('slow');\r\n");
            sb.Append("         icon.removeClass('fa fa-minus').addClass('fa fa-plus');\r\n");
            sb.Append("     }\r\n");
            sb.Append("     return false;\r\n");
            sb.Append("});\r\n\r\n");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptFatura", sb.ToString(), true);
        }

        protected void cmdFatura_Click(object sender, EventArgs e)
        {
            try
            {
                int idFatura = Convert.ToInt32(Request.Form["FaturaGroup"]);
                if (idFatura != 0)
                {
                    hddidFatura.Value = idFatura.ToString();
                    var lancamento = bs_Detalhe_Fatura.FirstOrDefault(x => x.idFatura == idFatura);
                    string idCartao = lancamento.idCartao.ToString();
                    PopulaModalFatura(idCartao);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalFatura", "$('#modalGerarFatura').modal('show');", true);
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro("Selecione uma fatura!");
                }
            }
            catch (Exception ex)
            {
                //MensagemPagina.MostraMensagem_Erro("Erro ao processar a fatura");
                MensagemPagina.MostraMensagem_Erro("Erro ao processar a fatura: " + ex.Message);
            }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidaDados())
                {
                    string sErro = "";
                    string idTitulo = "";
                    DataSet ds;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@dtVencimento", DateTime.Parse(txtdtVencimento.Text).ToString());
                    vParametros.Add("@sQuantidadeParcela", $"Única");
                    vParametros.Add("@sCodigo", txtsCodigo.Text);
                    vParametros.Add("@dtEmissao", DateTime.Parse(txtdtEmissão.Text).ToString());
                    vParametros.Add("@sDocumento", txtsReferencia.Text);
                    vParametros.Add("@idContabil", ddlidContabil.SelectedValue);
                    vParametros.Add("@idCentroDeCusto", ddlidCentroDeCusto.SelectedValue);
                    vParametros.Add("@idEmpresa", ddlidEmpresa.SelectedValue);
                    vParametros.Add("@nParcelas", "1");
                    vParametros.Add("@idParceiro", ddlidParceiro.SelectedValue);
                    vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
                    vParametros.Add("@sTituloConciliado", "N");
                    vParametros.Add("@nValorOriginal", txtnValorLiquido.Text.Replace(".", "").Replace(",", ".").Replace("R$", "").Trim());
                    vParametros.Add("@nValorBruto", txtnValorBruto.Text.Replace(".", "").Replace(",", ".").Replace("R$", "").Trim());
                    vParametros.Add("@nSaldo", txtnValorBruto.Text.Replace(".", "").Replace(",", ".").Replace("R$", "").Trim());
                    vParametros.Add("@idFormaPagamento", ddlidFormaPagamento.SelectedValue);
                    vParametros.Add("@idMeioPagamento", ddlidMeioPagamento.SelectedValue);
                    vParametros.Add("@idCategoriaPagar", ddlidCategoriaPagar.SelectedValue);
                    ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Pagar", vParametros);
                    if (BD.ValidarDataSet(ds, out sErro))
                    {
                        idTitulo = Retorno.DATASET(ds, 0, "idContasPagar");
                        GerarFatura(idTitulo);
                    }

                    btnSalvar.Attributes.Add("disabled", "disabled");
                }


            }
            catch (Exception ex)
            {
                MensagemPaginaModal.MostraMensagem_Erro("Erro ao salvar: " + ex.Message);
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }

        protected void ddlidCategoriaPagar_OnSelectedIndexChanged(object sender, EventArgs e)
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
            RegistraScript();
        }

        protected void gv_FaturaCartao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                cls_Fatura fatura = (cls_Fatura)e.Row.DataItem;

                var gv = (GridView)e.Row.FindControl("gv_FaturaDetalhe");
                if (gv != null)
                {
                    //gv.DataSource = bs_Detalhe_Fatura.Where(n => n.idFatura.ToString() == gv_FaturaCartao.DataKeys[e.Row.RowIndex]["idFatura"].ToString()).OrderBy(x => x.dtLancamento);
                    //gv.DataBind();

                    gv.DataSource = fatura.lsCartoes;
                    gv.DataBind();
                }                

                RadioButton rb = (RadioButton)e.Row.FindControl("cbFatura");
                if (rb != null)
                {
                    rb.InputAttributes["name"] = "FaturaGroup";
                }
                e.Row.CssClass = "gvMainTd";
            }
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "gvMainTh";
        }

        protected void gv_FaturaDetalhe_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            cls_FaturaCartao cartao = (cls_FaturaCartao)e.Row.DataItem;

            var gv2 = (GridView)e.Row.FindControl("gv_CartaoDetalhe");
            if (gv2 != null)
            {             
                gv2.DataSource = cartao.lsLancamentos;
                gv2.DataBind();
            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = "gvMainTd";
            }
            else if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "gvChildHeader";
        }

        protected void gv_CartaoDetalhe_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = "gvMainTd";
            }
            else if(e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "gvChildHeader2";
        }
    }
}