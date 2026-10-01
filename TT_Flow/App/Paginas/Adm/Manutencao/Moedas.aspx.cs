using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using TT.FrameWork;
using TT_Flow.FrameWork;

namespace TT_Flow.App.Paginas.Adm.Manutencao
{
    public partial class Moedas : System.Web.UI.Page
    {
        string sTituloPagina = "Moedas";
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_Moedas";

        #region | Classes
        public List<FrameWork.cls_Moeda> bs_Moeda
        {
            get
            {
                if (ViewState["bs_Moeda"] == null)
                {
                    ViewState["bs_Moeda"] = new List<FrameWork.cls_Moeda>();
                }
                return (List<FrameWork.cls_Moeda>)ViewState["bs_Moeda"];
            }
            set
            {
                ViewState["bs_Moeda"] = value;
            }
        }
        #endregion

        #region | Funções Incialização do Forms
        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();

            if (!IsPostBack)
            {
                FUNCOES.ValidaPermissao(Permissao.Administracao.Moedas.Consultar, true);
                Pesquisar();
            }
            else
            {
                var requestTarget = this.Request["__EVENTTARGET"];
                var requestArgs = this.Request["__EVENTARGUMENT"];

                if (requestTarget == "funcao_SAIR")
                {
                    FUNCOES.DirecionaPagina("/app/dashboard.aspx");
                }
                else if (requestTarget == "funcao_SALVAR")
                {
                    Salvar_Moeda();
                }
            }
            RegistraScript("");
        }
        #endregion

        #region |Metodos Banco de Dados
        protected void Pesquisar()
        {
            PopularCombos();
            string sErro = "";

            try
            {
                LimpaCampos();

                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR");
                DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsPesquisa, out sErro))
                {
                    foreach (DataRow row in dsPesquisa.Tables[0].Rows)
                    {
                        FrameWork.cls_Moeda objItem     = new FrameWork.cls_Moeda();
                        objItem.IdMoeda                 = Convert.ToInt32(row["idMoeda"].ToString());
                        objItem.IdTipoMoeda             = Convert.ToInt32(row["idTipoMoeda"].ToString());
                        objItem.SDscTipoMoeda           = row["sDscTipoMoeda"].ToString();
                        objItem.UltimonValorCambio      = row["UltimonValorCambio"].ToString();
                        objItem.UltimodtAtualizacao     = row["UltimodtAtualizacao"].ToString();
                        objItem.NValorCambio            = Convert.ToDouble(row["NValorCambio"].ToString());
                        objItem.SFuncao                 = "ATUAL";
                        bs_Moeda.Add(objItem);
                    }
                    dtgMoeda_DataBind();
                    LimpaCampos_Moeda();
                    PainelAtualizacao.Visible = true;
                    PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));

                    lblTituloPagina.Text = string.Format("Moedas");
                    BreadCrumb.TitulodaPagina = string.Format("Moedas");
                    lblTituloSalvar.Text = "Confirma a Alteração da " + lblTituloPagina.Text + "?";

                    //VALIDAR PERMISSÂO
                    cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Administracao.Moedas.Alterar);
                    Popular_Aba_Historico(dsPesquisa);
                }
                else
                {
                    throw new Exception(sErro);
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }
        #endregion

        #region | Limpar Campos
        void LimpaCampos()
        {
            hddidMoeda.Value = "0";
            bs_Moeda.Clear();
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
        }
        #endregion

        #region | Validação 
        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
            }
            return bRetorno;
        }
        #endregion

        #region | Script 
        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            //Mensagens de Confirmação
            sb.Append("$v192(function() {");
            sb.Append("$v192(\"#dialog-Salvar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_SALVAR\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('#cphCorpo_cmdSalvar').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Salvar').dialog('open');");
            sb.Append("});");

            sb.Append("$('[id*=txtMoeda_nValorCambio]').mask('000.000.000.000.009,99', { reverse: true });");
            sb.Append("$('[id*=txtnValorCambio]').mask('000.009,99', { reverse: true });");

            sb.Append("});");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);
        }
        #endregion

        #region | Combos/DDL
        void PopularCombos()
        {
            //FUNCOES.Popula_Combo(ddlMoeda_idPais, "sp_Select 'tbl_Flow_WMS_Produtos_Origem_Sigla'", "idPais", "sDscPais", false, "Selecione um Pais", "0");
            FUNCOES.Popula_Combo(ddlMoeda_idTipoMoeda, "sp_Select 'tbl_Flow_Tipo_Moeda'", "idTipoMoeda", "sDscTipoMoeda", false, "Selecione uma Moeda", "0");
        }
        #endregion

        #region | Moedas

        void Salvar_Moeda()
        {
            try
            {
                SalvarGrid();
                Dictionary<String, String> vParametroMoeda = new Dictionary<string, string>();
                foreach (cls_Moeda Moeda_Linha in bs_Moeda)
                {
                    vParametroMoeda["@sFuncao"]         = Moeda_Linha.SFuncao ;
                    vParametroMoeda["@idMoeda"]         = Moeda_Linha.IdMoeda.ToString();
                    vParametroMoeda["@idTipoMoeda"]     = Moeda_Linha.IdTipoMoeda.ToString();
                    //vParametroMoeda["@idPais"]          = Moeda_Linha.IdPais.ToString();
                    vParametroMoeda["@nValorCambio"]    = BD.Conversoes.Numerico(Moeda_Linha.NValorCambio);
                    vParametroMoeda["@idUsuarioAtualizacao"] = IDENTITY.Variaveis.idUsuario();
                    BD.ExecutarDataSet(sProcedure, vParametroMoeda);
                }
                MensagemPagina.MostraMensagem_Sucesso("Alteração efetuada!");
                Pesquisar();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        protected void cmdMoeda_Incluir_Click(object sender, EventArgs e)
        {
            string sMensagem = "";
            if (ValidarDados_Moeda(ref sMensagem))
            {
                FrameWork.cls_Moeda objItem = new FrameWork.cls_Moeda();
                objItem.IdMoeda = 0;
                objItem.IdTipoMoeda = Convert.ToInt32(ddlMoeda_idTipoMoeda.SelectedValue); ;
                objItem.SDscTipoMoeda = ddlMoeda_idTipoMoeda.SelectedItem.ToString();
                //objItem.IdPais = Convert.ToInt32(ddlMoeda_idPais.SelectedValue);
                //objItem.SDscPais = ddlMoeda_idPais.SelectedItem.ToString();
                objItem.NValorCambio = Convert.ToDouble(txtMoeda_nValorCambio.Text);
                objItem.SFuncao = "SALVAR";

                bs_Moeda.Add(objItem);
                dtgMoeda_DataBind();
                LimpaCampos_Moeda();
            }
            else
            {
                MensagemAcoes.MostraMensagem_Erro(sMensagem, false);
            }
            RegistraScript("$('[id$=txtMoeda_nValorMoeda]').focus();");
        }

        protected void dtgMoeda_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(dtgMoeda.Rows[e.RowIndex].Cells[1].Text);
            var nParcela = bs_Moeda[bs_Moeda.FindIndex(x => x.IdTipoMoeda.Equals(idLinha))].IdTipoMoeda;
            bs_Moeda[bs_Moeda.FindIndex(x => x.IdTipoMoeda.Equals(idLinha))].SFuncao = "EXCLUIR_MOEDA";
            dtgMoeda_DataBind();
            //int index = e.RowIndex;
            //bs_Moeda[index].SFuncao = "EXCLUIR_MOEDA";
            //dtgMoeda_DataBind();
        }

        private bool ValidarDados_Moeda(ref string sMensagemErro)
        {
            if (ddlMoeda_idTipoMoeda.SelectedValue == "0")
            {
                sMensagemErro += "Selecione uma moeda <br/>";
            }

            //if (ddlMoeda_idPais.SelectedValue == "0")
            //{
            //    sMensagemErro += "Selecione um País <br/>";
            //}

            if (!Validacoes.ValidarMoeda(txtMoeda_nValorCambio))
            {
                sMensagemErro += "Valor inválido <br/>";
            }

            //if (sMensagemErro == "")
            //{
            //    if (bs_Moeda.Any(item => item.IdTipoMoeda == Convert.ToInt32(ddlMoeda_idTipoMoeda.SelectedValue) && item.IdPais == Convert.ToInt32(ddlMoeda_idPais.SelectedValue)))
            //    {
            //        sMensagemErro = "Essa moeda já existe.";
            //    }
            //}

            return sMensagemErro != "" ? false : true;
        }

        void LimpaCampos_Moeda()
        {
            ddlMoeda_idTipoMoeda.SelectedValue = "0";
            //ddlMoeda_idPais.SelectedValue = "0";
            txtMoeda_nValorCambio.Text = "";
        }

        void Popular_dtgMoeda(DataSet dsPesquisa)
        {
            PopularCombos();
        }

        void dtgMoeda_DataBind()
        {
            dtgMoeda.DataSource = bs_Moeda.Where(c => c.SFuncao.ToString() != "EXCLUIR_MOEDA");
            dtgMoeda.DataBind();
            FUNCOES.Popula_Combo(ddlMoeda_idTipoMoeda, "sp_Select 'tbl_Flow_Tipo_Moeda'", "idTipoMoeda", "sDscTipoMoeda", false, "Selecione uma Moeda", "0");
            var idMoeda = bs_Moeda.Where(c => c.SFuncao != "EXCLUIR_MOEDA").Select(c => c.IdTipoMoeda).ToList();

            foreach (ListItem item in ddlMoeda_idTipoMoeda.Items.Cast<ListItem>().ToList())
            {
                if (!string.IsNullOrEmpty(item.Value) && idMoeda.Contains(Convert.ToInt32(item.Value)))
                {
                    ddlMoeda_idTipoMoeda.Items.Remove(item);
                }
            }
        }

        #endregion

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

        protected void txtidMoeda_nValorMoeda_TextChanged(object sender, EventArgs e)
        {
            //foreach (GridViewRow item in dtgMoeda.Rows)
            //{
            //    TextBox txtMoeda_nValorMoeda = (TextBox)item.FindControl("txtidMoeda_nValorMoeda_TextChanged");
            //    bs_Moeda[nContador].nValorMoeda = Convert.ToInt32(txtMoeda_nValorMoeda.Text);
            //    nContador++;
            //}
        }

        protected void dtgMoeda_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0,1,6);

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[4].Text != "")
                {
                    decimal valor;
                    if (Decimal.TryParse(e.Row.Cells[4].Text, out valor))
                    {
                        e.Row.Cells[4].Text = valor.ToString("N2");
                    }
                }
            }
        }

        void SalvarGrid()
        {
            int nContador = 0;
            double nValorCambio_NOVO = 0;
            foreach (GridViewRow item in dtgMoeda.Rows)
            {
                TextBox txtnValorCambio_Linha = (TextBox)item.FindControl("txtnValorCambio");
                nValorCambio_NOVO = Convert.ToDouble(txtnValorCambio_Linha.Text);

                if (nValorCambio_NOVO != bs_Moeda[nContador].NValorCambio)
                {
                    bs_Moeda[nContador].NValorCambio = nValorCambio_NOVO;
                    bs_Moeda[nContador].SFuncao = "SALVAR";
                }
                nContador++;
            }
        }

        void Popular_Aba_Historico(DataSet ds)
        {
            gv_Historico.DataSource = ds.Tables[1];
            gv_Historico.DataBind();
            aba_Historico.Visible = true;
        }
    }
}