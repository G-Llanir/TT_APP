using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using TT_Flow.FrameWork;

namespace TT_Flow.App.Paginas.Manutencao
{
    public partial class CondicaodePagamento_Detalhe : Page
    {
        #region | Classes

        public List<cls_CondicoesPagamento> bs_CondicoesPagamento
        {

            get
            {
                if (ViewState["bs_CondicoesPagamento"] == null)
                {
                    ViewState["bs_CondicoesPagamento"] = new List<cls_CondicoesPagamento>();
                }
                return (List<cls_CondicoesPagamento>)ViewState["bs_CondicoesPagamento"];
            }
            set
            {
                ViewState["bs_CondicoesPagamento"] = value;
            }

        }

        #endregion

        string sTituloPagina = "Condição de Pagamento";
        string sProcedure = "sp_Manipula_tbl_Flow_CondicaodePagamento";

        #region | Funções Incialização do Form

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopularCombos();

                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.CondicaodePagamento.Consultar, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.CondicaodePagamento.Incluir, true);
                    Pesquisar("0");
                }
            }
            else
            {
                var requestTarget = this.Request["__EVENTTARGET"];

                if (requestTarget == "funcao_Parcelas")
                    txtnQtdParcelas_TextChanged();
            }
            RegistraScript("");
        }

        #endregion

        #region | Metodos Banco de Dados

        protected void Pesquisar(string idPesquisa)
        {
            try
            {
                div_idPedido.Visible = false;
                DIV_CP.Visible = false;
                LimpaCampos();

                if (idPesquisa != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idCondicaoPagamento", idPesquisa }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        hddidCondicaoPagamento.Value = RETORNO.DATASET(dsPesquisa, "idCondicaoPagamento");
                        txtidCondicaoPagamento.Text = hddidCondicaoPagamento.Value;
                        txtidPedido.Text = RETORNO.DATASET(dsPesquisa, "idPedido");
                        txtidPedido.Text = RETORNO.DATASET(dsPesquisa, "idPedido");
                        txtsDscCondicaoPagamento.Text = RETORNO.DATASET(dsPesquisa, "sDscCondicaoPagamento");
                        txtnQtdParcelas.Text = RETORNO.DATASET(dsPesquisa, "nQtdParcelas");
                        hddnQtdParcelas.Value = RETORNO.DATASET(dsPesquisa, "nQtdParcelas");

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, "sDscUsuarioAtualizacao"));
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, "sSituacao"));
                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsDscCondicaoPagamento.Text);

                        Popular_dtgCondicoesPagamento(dsPesquisa);

                        if (txtnQtdParcelas.Text != "0" && txtnQtdParcelas.Text != "")
                            txtnQtdParcelas_TextChanged();

                        if (int.TryParse(txtidPedido.Text, out int idPedido) && idPedido > 0)
                        {
                            div_idPedido.Visible = true;
                            cmd_idPedido.Attributes["href"] = RETORNO.DATASET(dsPesquisa, "sLinkPedido");

                            if (!cmd_idPedido.Attributes["href"].EndsWith("=2"))
                                cmd_idPedido.InnerText = "Orçamento";
                            else
                            {
                                txtidPedido.Text = cmd_idPedido.Attributes["href"].Replace("/App/Paginas/Pedidos_Detalhe.aspx?id=", "").Replace("&sTp=2", "");
                                cmd_idPedido.InnerText = "Pedido";
                            }
                        }

                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.CondicaodePagamento.Alterar);
                    }
                    else
                        throw new Exception(sErro);
                }
                else
                {
                    BreadCrumb.TitulodaPagina = "Incluir";
                    lblTituloPagina.Text = string.Format("Novo {0}", sTituloPagina);
                    cmdSalvar.Text = "Salvar";
                }

                txtsDscCondicaoPagamento.Focus();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        protected void cmdSalvar_Click1(object sender, EventArgs e)
        {
            string sErro = "";

            dtgCondicoesPagamento_SalvarGRID();
            if (ValidarDados())
            {
                try
                {
                    string[] vidCondicaoPagamento = hddidCondicaoPagamento.Value.Split(',');
                    string idCondicaoPagamento = vidCondicaoPagamento[0].ToString();


                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idCondicaoPagamento", idCondicaoPagamento);
                    vParametros.Add("@sDscCondicaoPagamento", txtsDscCondicaoPagamento.Text);
                    vParametros.Add("@nQtdParcelas", txtnQtdParcelas.Text);
                    vParametros.Add("@sSituacao", ComboAtivo.Situacao_Recuperar());
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idCondicaoPagamento = RETORNO.DATASET(dsSalvar, 0, "idCondicaoPagamento");
                        SalvarLista(idCondicaoPagamento);
                        Pesquisar(idCondicaoPagamento);
                        MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");

                    }
                    else
                    {
                        throw new Exception("BD: " + sErro.ToString());
                    }


                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }

            }
        }

        #endregion

        #region | Combos/DDL

        void PopularCombos()
        {
            //FUNCOES.Popula_Combo(ddlidTipoCondicaoPagamento, "sp_Select 'tbl_Flow_CondicaodePagamento_Tipo'", "idTipoCondicaoPagamento", "sDscTipoCondicaoPagamenro", false, "Selecione o Pagamento", "0");
        }

        #endregion

        #region | Limpas / Validar Campos

        void LimpaCampos()
        {
            txtidCondicaoPagamento.Text = "Novo";
            txtsDscCondicaoPagamento.Text = "";
            txtnQtdParcelas.Text = "";
            hddnQtdParcelas.Value = "";
            hddidCondicaoPagamento.Value = "0";
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
            bs_CondicoesPagamento.Clear();
        }

        private bool ValidarDados()
        {
            decimal nTotalPorcentagemValor = 0;

            if (txtsDscCondicaoPagamento.Text.Length < 3)
            {
                MensagemPagina.MostraMensagem_Erro("Informe um Descrição válida para a Condição de Pagamento!");
                return false;
            }
            if (!string.IsNullOrEmpty(txtnQtdParcelas.Text))
            {
                int resultado = 0;
                if (!int.TryParse(this.txtnQtdParcelas.Text.Trim(), out resultado))
                {
                    MensagemPagina.MostraMensagem_Erro("Número de parcelas inválido!");
                    return false;

                }
            }
            foreach (var linha in bs_CondicoesPagamento)
            {
                nTotalPorcentagemValor += linha.nPorcentagemValor;
            }
            if (nTotalPorcentagemValor != 100)
            {
                MensagemPagina.MostraMensagem_Erro("O Total (" + nTotalPorcentagemValor.ToString() + "%) não é igual a 100%");
                return false;
            }
            return true;
        }

        #endregion

        #region | Condicoes pagamento 

        void Popular_dtgCondicoesPagamento(DataSet dsPesquisa)
        {
            DIV_CP.Visible = true;
            foreach (DataRow row in dsPesquisa.Tables[1].Rows)
            {
                FrameWork.cls_CondicoesPagamento objItem = new FrameWork.cls_CondicoesPagamento();

                objItem.idLinha = bs_CondicoesPagamento.Count() + 1;
                objItem.sFuncao = "SEM ALTERAÇÃO";

                objItem.idCondicaoPagamento = Convert.ToInt32(row["idCondicaoPagamento"].ToString());
                objItem.sDscCondicaoPagamento = row["sDscCondicaoPagamento"].ToString();
                //objItem.nQtdParcelas = Convert.ToInt32(row["nQtdParcelas"].ToString());
                objItem.idTipoCondicaoPagamento = Convert.ToInt32(row["idTipoCondicaoPagamento"].ToString());
                objItem.nPorcentagemValor = ConverterStringDecimal(row["nPorcentagemValor"].ToString());
                objItem.nDDL = Convert.ToInt32(row["nDDL"].ToString());

                bs_CondicoesPagamento.Add(objItem);
            }
            dtgCondicaoPagamento_DataBind();
            LimpaCampos_CondicaoPagamento();
        }

        void dtgCondicoesPagamento_SalvarGRID()
        {
            int nContador = 0;
            foreach (GridViewRow item in dtgCondicaoPagamento.Rows)
            {
                DropDownList ddlidTipoCondicaoPagamento_Linha = (DropDownList)item.FindControl("ddlidTipoCondicaoPagamento");
                bs_CondicoesPagamento[nContador].idTipoCondicaoPagamento = Convert.ToInt32(ddlidTipoCondicaoPagamento_Linha.SelectedValue);

                TextBox txtnPorcentagemValor_Linha = (TextBox)item.FindControl("txtnPorcentagemValor");


                string porcentagemValor = txtnPorcentagemValor_Linha.Text.Replace("%", "");
                bs_CondicoesPagamento[nContador].nPorcentagemValor = Convert.ToDecimal(porcentagemValor);

                TextBox txtnDDL_Linha = (TextBox)item.FindControl("txtnDDL");
                bs_CondicoesPagamento[nContador].nDDL = Convert.ToInt32(txtnDDL_Linha.Text);

                nContador++;
            }
        }

        protected void dtgCondicaoPagamento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList _ddlidTipoCondicaoPagamento = (e.Row.FindControl("ddlidTipoCondicaoPagamento") as DropDownList);
                FUNCOES.Popula_Combo(_ddlidTipoCondicaoPagamento, "sp_Select 'tbl_Flow_CondicaodePagamento_Tipo'", "idTipoCondicaoPagamento", "sDscTipoCondicaoPagamento", false, "Selecione o Pagamento", "0");
                _ddlidTipoCondicaoPagamento.SelectedValue = bs_CondicoesPagamento[e.Row.RowIndex].idTipoCondicaoPagamento.ToString();
            }
            GRID.EsconderColunas(e, 0, 1);
        }

        bool DeletarLista(string idCondicaoPagamento)
        {
            bool bRetorno = false;
            try
            {
                DataSet dsDocumento_Excluir;
                Dictionary<String, String> vParametroArquivo_Excluir = new Dictionary<string, string>();
                vParametroArquivo_Excluir.Add("@sFuncao", "EXCLUIR_CONDICAOPAGAMENTO");
                vParametroArquivo_Excluir.Add("@idCondicaoPagamento", idCondicaoPagamento);
                dsDocumento_Excluir = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_CondicaodePagamento", vParametroArquivo_Excluir);

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
            return bRetorno;
        }

        void SalvarLista(string idCondicaoPagamento)
        {
            dtgCondicoesPagamento_SalvarGRID();

            if (ValidarDados())
            {
                DeletarLista(idCondicaoPagamento);
                //FUNCAO DELETEAR AQUI------------------------------------------------------------------------------------------

                foreach (var linha in bs_CondicoesPagamento)
                {
                    if (linha.sFuncao == "SEM ALTERAÇÃO")
                    {
                        linha.sFuncao = "SALVAR_CONDICAOPAGAMENTO";
                    }
                    if (linha.sFuncao == "SALVAR_CONDICAOPAGAMENTO")
                    {
                        decimal nPorcentagemValor = linha.nPorcentagemValor;
                        Dictionary<String, String> vParametroItens_Incluir = new Dictionary<string, string>();

                        vParametroItens_Incluir["@sFuncao"] = linha.sFuncao;
                        vParametroItens_Incluir["@idRegistro"] = linha.idRegistroCondicaoPagamento.ToString();
                        vParametroItens_Incluir["@idCondicaoPagamento"] = idCondicaoPagamento;

                        vParametroItens_Incluir["@idTipoCondicaoPagamento"] = linha.idTipoCondicaoPagamento.ToString();
                        vParametroItens_Incluir["@sDscCondicaoPagamento"] = linha.sDscCondicaoPagamento;
                        vParametroItens_Incluir["@nPorcentagemValor"] = BD.Conversoes.Numerico(nPorcentagemValor);
                        vParametroItens_Incluir["@nDDL"] = linha.nDDL.ToString();

                        BD.ExecutarDataSet(sProcedure, vParametroItens_Incluir);
                    }
                }
            }
        }

        #endregion

        #region | Parcelas

        void txtnQtdParcelas_TextChanged()
        {
            DIV_CP.Visible = true;

            int qtdParcelas;
            if (!int.TryParse(txtnQtdParcelas.Text, out qtdParcelas))
            {
                return;
            }
            if (bs_CondicoesPagamento.Count.ToString() != txtnQtdParcelas.Text)
            {
                hddnQtdParcelas.Value = txtnQtdParcelas.Text;
                bs_CondicoesPagamento.Clear();

                for (int i = bs_CondicoesPagamento.Count(); i < qtdParcelas; i++)
                {
                    FrameWork.cls_CondicoesPagamento objItem = new FrameWork.cls_CondicoesPagamento();

                    string[] vidCondicaoPagamento = hddidCondicaoPagamento.Value.Split(',');
                    string idCondicaoPagamento = vidCondicaoPagamento[0].ToString();

                    objItem.idLinha = bs_CondicoesPagamento.Count() + 1;
                    objItem.sFuncao = "SALVAR_CONDICAOPAGAMENTO";

                    objItem.idCondicaoPagamento = Convert.ToInt32(idCondicaoPagamento);
                    objItem.idTipoCondicaoPagamento = 0;
                    objItem.sDscCondicaoPagamento = "";
                    objItem.nQtdParcelas = Convert.ToInt32(txtnQtdParcelas.Text);
                    objItem.nPorcentagemValor = 0;
                    objItem.nDDL = 0;

                    bs_CondicoesPagamento.Add(objItem);
                }
                dtgCondicaoPagamento_DataBind();
                LimpaCampos_CondicaoPagamento();
            }
        }

        void dtgCondicaoPagamento_DataBind()
        {
            try
            {
                dtgCondicaoPagamento.DataSource = bs_CondicoesPagamento.Where(c => c.sFuncao.ToString() != "EXCLUIR_CONDICAOPAGAMENTO");
                dtgCondicaoPagamento.DataBind();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao Carregar Grid de Itens: " + ex.Message);
            }
        }

        protected void dtgCondicaoPagamento_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(dtgCondicaoPagamento.Rows[e.RowIndex].Cells[0].Text);
            bs_CondicoesPagamento[bs_CondicoesPagamento.FindIndex(x => x.idLinha.Equals(idLinha))].sFuncao = "EXCLUIR_CONDICAOPAGAMENTO";
            dtgCondicaoPagamento_DataBind();
        }

        protected void dtgCondicaoPagamento_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idLinha = int.Parse(e.CommandArgument.ToString());
            hddCondicaoPagamento_idLinha.Value = idLinha.ToString();

        }

        void LimpaCampos_CondicaoPagamento()
        {
            hddCondicaoPagamento_idLinha.Value = "0";
        }

        decimal ConverterStringDecimal(string dado)
        {
            decimal converter = 0;
            if (decimal.TryParse(dado, out converter))
            {
                converter = Convert.ToDecimal(dado);
                return converter;
            }
            return decimal.Zero;
        }

        protected void ddlidTipoCondicaoPagamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nContador = 0;
            foreach (GridViewRow item in dtgCondicaoPagamento.Rows)
            {
                DropDownList ddlidTipoCondicaoPagamento = (DropDownList)item.FindControl("ddlidTipoCondicaoPagamento");
                bs_CondicoesPagamento[nContador].sDscCondicaoPagamento = ddlidTipoCondicaoPagamento.SelectedItem.ToString();
                bs_CondicoesPagamento[nContador].idTipoCondicaoPagamento = Convert.ToInt32(ddlidTipoCondicaoPagamento.SelectedValue);
                nContador++;
            }
        }

        protected void txtnDDL_TextChanged(object sender, EventArgs e)
        {
            int nContador = 0;
            foreach (GridViewRow item in dtgCondicaoPagamento.Rows)
            {
                TextBox txtnDDL_Linha = (TextBox)item.FindControl("txtnDDL");
                bs_CondicoesPagamento[nContador].nDDL = Convert.ToInt32(txtnDDL_Linha.Text);

                nContador++;
            }
        }

        #endregion

        #region | Script 

        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("$v192(function() {");
            sb.Append("$('[id*=txtnPorcentagemValor]').mask('000,00%', { reverse: true });");
            sb.Append("$('[id*=txtnQtdParcelas]').mask('0#');");
            sb.Append("$('[id*=txtnDDL]').mask('0#');");

            sb.Append("$v192('[id*=txtnQtdParcelas]').on('blur', function() {");
            sb.Append("    var valor = this.value;");
            sb.Append("    if (valor && !isNaN(valor) && /^\\d*(\\.\\d{0,2})?$/.test(valor)) {");
            sb.Append("        __doPostBack(\"funcao_Parcelas\", \"\");");
            sb.Append("    }");
            sb.Append("});");

            if (sFuncao != "")
            {
                sb.Append(sFuncao);
            }

            sb.Append("});");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);
        }

        #endregion
    }
}