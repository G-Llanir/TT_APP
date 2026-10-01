using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using IDENTITY = TT.FrameWork.Identity;
using RETORNO = TT.FrameWork.BD.Retorno;
using System.Web.UI.WebControls;
using TT_Flow.FrameWork;
using TT.FrameWork;
using TT_Flow.App.Controles;
using static TT.FrameWork.BD;
using static NPOI.SS.Formula.Functions.Countif;
using System.Linq;
using System.Reflection.Emit;

namespace TT_Flow.App.Paginas.WMS.Manutencao
{
    public partial class Sistemas : System.Web.UI.Page
    {
        #region | Classes
        string sTituloPagina = "Sistemas";
        public List<cls_WMS_Produtos> Base_Sistemas
        {
            get
            {
                if (ViewState["Base_Sistemas"] == null)
                {
                    ViewState["Base_Sistemas"] = new List<FrameWork.cls_WMS_Produtos>();
                }
                return (List<FrameWork.cls_WMS_Produtos>)ViewState["Base_Sistemas"];
            }

            set
            {
                ViewState["Base_Sistemas"] = value;
            }
        }
        #endregion

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            manual.sNomeArquivo = "Manual-Unidades.pdf";
            string idSistema = "";
            FUNCOES.ValidaPermissao(Permissao.WMS.Sistemas.Visualizar, true);
            FiltroPesquisaProdutos.TipoFiltroPesquisa = "0";

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                if (Request["id"] != null)
                {
                    idSistema = Request["id"].ToString();
                }
                Pesquisar(idSistema);
                RegistraScriptYear();
            }
            txtPesquisa.Focus();
            RegistraScriptYear();
            var campos = new Dictionary<string, bool>
            {
                { "Quantidade", false },
                { "Valor Unitário", false },
                { "Unidade", false }
            };
            FiltroPesquisaProdutos.Controle_ExibicaoCampos(campos, "1");
            FiltroPesquisaProdutos.ModificaTamanhoCampos(4, 8, 0, 0, 0);
        }
        #endregion

        #region | Metodos Banco de Dados
        protected void Pesquisar(string idSistema)
        {
            manual.Visible = false;
            Base_Sistemas.Clear();
            string sFuncao = "CONSULTAR";
            string sDscSistema = "";
            string Ativo = "";
            sDscSistema = txtPesquisa.Text.Trim();
            Ativo = ddlsAtivo.SelectedValue;
            DataSet tb;
            string sSql = "sp_Manipula_tbl_Flow_Sistemas";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@sPesquisa", sDscSistema);
            vParametros.Add("@idSistema", idSistema);
            vParametros.Add("@sAtivo", Ativo);

            tb = BD.ExecutarDataSet(sSql, vParametros, false);

            if (tb.Tables[1].Rows.Count > 0)
            {
                if (idSistema != "")
                {
                    aba_Arquivos.Visible = true;
                    txtidSistema.Text = RETORNO.DATASET(tb, 1, 0, "idSistema");
                    txtsDscSistema.Text = RETORNO.DATASET(tb, 1, 0, "sDscSistema");
                    txtsObservacao.Text = RETORNO.DATASET(tb, 1, 0, "sObservacao");
                    sAtivo.Definir(RETORNO.DATASET(tb, 1, 0, "sAtivo"), "Ativo", "N");
                    foreach (DataRow linha in tb.Tables[2].Rows)
                    {
                        FrameWork.cls_WMS_Produtos objEnvios = new FrameWork.cls_WMS_Produtos();
                        objEnvios.nOrdem = int.Parse(linha["nOrdem"].ToString());
                        objEnvios.IdItem = int.Parse(linha["IdItem"].ToString());
                        objEnvios.SFuncao = "Incluir";
                        objEnvios.SCodigo = linha["SCodigo"].ToString();
                        objEnvios.SDscProduto = linha["SDscProduto"].ToString();
                        Base_Sistemas.Add(objEnvios);
                    }
                    DataBind_gvItens();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenDetalhe", "$('#Modal_Detalhe').modal('show');", true);

                    frmArquivos.Attributes.Add("src", string.Format("~/App/Paginas/Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}", txtidSistema.Text, "Sistemas"));
                }

                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb, 0, "desc"), true);
                pnResultado.Visible = true;
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro("Nenhum Lançamento Localizado");
            }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                string idSistema = "";
                if (Validar())
                {
                    string sErro = "";
                    DataSet tb;
                    string sSql = "sp_Manipula_tbl_Flow_Sistemas";
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@sDscSistema", txtsDscSistema.Text);
                    vParametros.Add("@sObservacao", txtsObservacao.Text);
                    vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());
                    vParametros.Add("@sAtivo", sAtivo.Recuperar());
                    vParametros.Add("@idSistema", txtidSistema.Text);
                    tb = BD.ExecutarDataSet(sSql, vParametros, false);

                    if (BD.ValidarDataSet(tb, out sErro))
                    {
                        idSistema = RETORNO.DATASET(tb, 0, "idSistema");
                    }

                    foreach (var linha in Base_Sistemas)
                    {
                        DataSet tbItens;
                        Dictionary<String, String> vParametrosItens = new Dictionary<string, string>();
                        vParametrosItens.Add("@sFuncao", linha.SFuncao);
                        vParametrosItens.Add("@idProduto", linha.IdItem.ToString());
                        vParametrosItens.Add("@idItens", linha.nOrdem.ToString());
                        vParametrosItens.Add("@idSistema", idSistema);
                        tbItens = BD.ExecutarDataSet(sSql, vParametrosItens, false);
                    }

                    Pesquisar(idSistema);
                    MensagemPagina1.MostraMensagem_Sucesso("Salvo com sucesso!");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina1.MostraMensagem_Erro(ex.Message);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "$('#Modal_Detalhe').modal('show');", true);
            }
        }
        #endregion

        #region | CMD Click
        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar("");
        }

        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            sAtivo.Definir("S", "Ativo", "N");
            txtidSistema.Text = "0";
            Label2.Text = "Novo Sistema";
            LimpaCampo();
            txtsDscSistema.Text = "";
            txtsObservacao.Text = "";
            Base_Sistemas.Clear();
            DataBind_gvItens();
            aba_Arquivos.Visible = false;
            DIV_Detalhes.Visible = true;
            aba_Arquivos.Visible = false;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenDetalhe", "$('#Modal_Detalhe').modal('show');", true);
        }
        #endregion

        #region | Script
        void RegistraScriptYear()
        {

        }
        #endregion

        #region | Eventos
        private bool Validar()
        {
            string sMensagem = "";

            if (txtsDscSistema.Text == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Descreva a descrição!";
            }

            if (Base_Sistemas.Count == 0)
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Inclua produtos!";
            }

            if (sMensagem != "")
            {
                MensagemPagina1.MostraMensagem_Erro(sMensagem);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ModalDetalhe", "$('#Modal_Detalhe').modal('show');", true);
                return false;
            }

            return true;
        }

        protected void btnIncluir_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidarItens())
                {
                    FrameWork.cls_WMS_Produtos objEnvios = new FrameWork.cls_WMS_Produtos();
                    objEnvios.nOrdem = 0;
                    objEnvios.IdItem = int.Parse(FiltroPesquisaProdutos.IdItem);
                    objEnvios.SFuncao = "SALVAR_ITENS";
                    objEnvios.SCodigo = FiltroPesquisaProdutos.SCodigo;
                    objEnvios.SDscProduto = FiltroPesquisaProdutos.SDscProduto;
                    Base_Sistemas.Add(objEnvios);

                    DataBind_gvItens();
                    LimpaCampo();
                }
            }
            catch (Exception ex)
            {
                MensagemPagina1.MostraMensagem_Erro(ex.Message);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "$('#Modal_Detalhe').modal('show');", true);
            }
        }

        void LimpaCampo()
        {
            FiltroPesquisaProdutos.SCodigo = "";
            FiltroPesquisaProdutos.SDscProduto = "";
        }

        private bool ValidarItens()
        {
            string sMensagem = "";

            if (FiltroPesquisaProdutos.SDscProduto.Length <= 0)
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "<b>Erro:</b> A Descrição do Produto deve ser preenchida!";
            }
            if (FiltroPesquisaProdutos.SCodigo.Length <= 0)
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "<b>Erro:</b> O Código do Produto deve estar preenchido!";
            }

            if (sMensagem != "")
            {
                MensagemPagina1.MostraMensagem_Erro(sMensagem);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ModalDetalhe", "$('#Modal_Detalhe').modal('show');", true);
                return false;
            }

            return true;
        }

        protected void gvItens_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Grid.EsconderColunas(e, 0, 1);
        }

        protected void LinkButton_Command(object sender, CommandEventArgs e)
        {
            //DIV_Arquivos.Visible = false;
            DIV_Detalhes.Visible = true;
            aba_Arquivos.Visible = true;
            string id = e.CommandArgument.ToString();
            Base_Sistemas.Clear();
            string sFuncao = "CONSULTAR";
            string sDscSistema = "";
            sDscSistema = txtPesquisa.Text.Trim();
            DataSet tb;
            string sSql = "sp_Manipula_tbl_Flow_Sistemas";
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", sFuncao);
            vParametros.Add("@sPesquisa", sDscSistema);
            vParametros.Add("@idSistema", id);

            tb = BD.ExecutarDataSet(sSql, vParametros, false);

            if (tb.Tables.Count > 0)
            {
                if (id != "")
                {
                    Label2.Text = "Sistema - " + RETORNO.DATASET(tb, 1, 0, "sDscSistema");
                    txtidSistema.Text = RETORNO.DATASET(tb, 1, 0, "idSistema");
                    txtsDscSistema.Text = RETORNO.DATASET(tb, 1, 0, "sDscSistema");
                    txtsObservacao.Text = RETORNO.DATASET(tb, 1, 0, "sObservacao");
                    sAtivo.Definir(RETORNO.DATASET(tb, 1, 0, "sAtivo"), "Ativo", "N");
                    foreach (DataRow linha in tb.Tables[2].Rows)
                    {
                        FrameWork.cls_WMS_Produtos objEnvios = new FrameWork.cls_WMS_Produtos();
                        objEnvios.nOrdem = int.Parse(linha["nOrdem"].ToString());
                        objEnvios.IdItem = int.Parse(linha["IdItem"].ToString());
                        objEnvios.SFuncao = "Incluir";
                        objEnvios.SCodigo = linha["SCodigo"].ToString();
                        objEnvios.SDscProduto = linha["SDscProduto"].ToString();
                        Base_Sistemas.Add(objEnvios);
                    }
                    gvItens.DataSource = Base_Sistemas;
                    gvItens.DataBind();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenDetalhe", "$('#Modal_Detalhe').modal('show');", true);
                }
                frmArquivos.Attributes.Add("src", string.Format("~/App/Paginas/Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}", txtidSistema.Text, "Sistemas"));
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb, 0, "desc"), true);
        }

        protected void gvItens_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            int IdItem = Convert.ToInt32(e.Keys[0].ToString());
            Base_Sistemas[Base_Sistemas.FindIndex(x => x.IdItem.Equals(IdItem))].SFuncao = "EXCLUIR ITEM";
            DataBind_gvItens();
        }

        void DataBind_gvItens()
        {
            gvItens.DataSource = Base_Sistemas.Where(c => c.SFuncao.ToString() != "EXCLUIR ITEM").ToList();
            gvItens.DataBind();
        }
        #endregion
    }
}