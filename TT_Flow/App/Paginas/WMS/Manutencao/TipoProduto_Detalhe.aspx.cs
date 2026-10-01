using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using System.Data;

namespace TT_Flow.App.Paginas.Almoxarifado
{
    public partial class TipoProduto_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Tipos";
        string sProcedure = "sp_Manipula_tbl_Flow_WMS_Produtos_Tipo";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.TipoProdutos.Consultar, true);
                    Pesquisar(Request["id"].ToString());

                    if (Request["msg"] == "1")
                        MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso!");
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.TipoProdutos.Incluir, true);
                    Pesquisar("0");
                }
            }
        }

        protected void Pesquisar(string idPesquisa)
        {
            string sErro = "";
            try
            {
                PopulaCombos();
                LimpaCampos();

                if (idPesquisa != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<string, string> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idTipoProduto", idPesquisa);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidTipoProduto.Value              = RETORNO.DATASET(dsPesquisa, 0, "idTipoProduto");
                        txtidTipoProduto.Text               = RETORNO.DATASET(dsPesquisa, 0, "idTipoProduto");
                        txtsDscTipoProduto.Text             = RETORNO.DATASET(dsPesquisa, 0, "sDscTipoProduto");

                        bool bComposicao = RETORNO.DATASET(dsPesquisa, 0, "sComposicao") == "S";
                        bool bComercial = RETORNO.DATASET(dsPesquisa, 0, "sExibeComercial") == "S";
                        bool bEmbalagem = RETORNO.DATASET(dsPesquisa, 0, "sEmbalagem") == "S";
                        // Vittorio --------------
                        bool bEPI = RETORNO.DATASET(dsPesquisa, 0, "sEPI") == "S";
                        //------------------------
                        bool bProduto = RETORNO.DATASET(dsPesquisa, 0, "sProduto") == "S";
                        bool bServico = RETORNO.DATASET(dsPesquisa, 0, "sServico") == "S";
                        bool bRecurso = RETORNO.DATASET(dsPesquisa, 0, "sRecurso") == "S";
                        bool bIndustrializado = RETORNO.DATASET(dsPesquisa, 0, "sIndustrializado") == "S";
                        bool bSistema = RETORNO.DATASET(dsPesquisa, 0, "sSistema") == "S";
                        bool bSub = RETORNO.DATASET(dsPesquisa, 0, "sSubTipo") == "S";
                        bool bFabricacao = RETORNO.DATASET(dsPesquisa, 0, "sFabricacao") == "S";

                        cblPrincipal.Items.FindByValue("COMP").Selected = bComposicao;
                        cblPrincipal.Items.FindByValue("COMER").Selected = bComercial;
                        cblPrincipal.Items.FindByValue("EMB").Selected = bEmbalagem;
                        // Vittorio --------------
                        cblPrincipal.Items.FindByValue("EPI").Selected = bEPI;
                        //---------------------
                        cblPrincipal.Items.FindByValue("IND").Selected = bIndustrializado;
                        cblPrincipal.Items.FindByValue("SIS").Selected = bSistema;

                        cblTipo.Items.FindByValue("PROD").Selected = bProduto;
                        cblTipo.Items.FindByValue("SER").Selected = bServico;
                        cblTipo.Items.FindByValue("SUB_SER").Selected = bSub;
                        cblTipo.Items.FindByValue("REC").Selected = bRecurso;
                        cblTipo.Items.FindByValue("FAB").Selected = bFabricacao;

                        PainelAtualizacao.Visible           = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"));
                        ddlTipoRegra.SelectedValue = RETORNO.DATASET(dsPesquisa, "idTipoRegra");
                        
                        ValidarVisibilidadeCHK();


                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsDscTipoProduto.Text);
                      

                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.TipoProdutos.Alterar);

                    }
                    else
                    {
                        throw new Exception(sErro);
                    }
                }
                else
                    lblTituloPagina.Text = string.Format("{0}{1}", "Novo ", sTituloPagina);


            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }

        void PopulaCombos()
        {
            FUNCOES.Popula_Combo(ddlTipoRegra, "sp_Select 'Flow_Regras_Tipo', @sPesquisa='S'", "idTipo", "sDscTipo", false, "Tipo de Regra Fiscal", "0");
            PopularCheckBoxList();
        }

        void LimpaCampos()
        {
            txtidTipoProduto.Text = "Novo";
            txtsDscTipoProduto.Text = "";
            hddidTipoProduto.Value = "0";
            PainelAtualizacao.Visible = false;
            cblPrincipal.Items.FindByValue("COMP").Selected = false;
            cblPrincipal.Items.FindByValue("COMER").Selected = false;
            cblPrincipal.Items.FindByValue("EMB").Selected = false;
            // Vittorio -------------
            cblPrincipal.Items.FindByValue("EPI").Selected = false;
            //--------------
            cblPrincipal.Items.FindByValue("IND").Selected = false;
            cblPrincipal.Items.FindByValue("SIS").Selected = false;
            cblTipo.Items.FindByValue("PROD").Selected = true;
            cblTipo.Items.FindByValue("SER").Selected = false;
            cblTipo.Items.FindByValue("SUB_SER").Selected = false;
            cblTipo.Items.FindByValue("REC").Selected = false;
            cblTipo.Items.FindByValue("FAB").Selected = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
        }

        private void PopularCheckBoxList()
        {
            cblPrincipal.Items.Clear();

            cblPrincipal.Items.Add(new ListItem("Exibição") { Enabled = false });
            cblPrincipal.Items.Add(new ListItem("Exibe Composição", "COMP"));
            cblPrincipal.Items.Add(new ListItem("Exibe em Comercial", "COMER"));

            cblPrincipal.Items.Add(new ListItem("Categorias") { Enabled = false });
            cblPrincipal.Items.Add(new ListItem("Embalagem", "EMB"));
            cblPrincipal.Items.Add(new ListItem("EPI", "EPI"));
            cblPrincipal.Items.Add(new ListItem("Industrializado", "IND"));
            cblPrincipal.Items.Add(new ListItem("Sistema", "SIS"));
        }

        protected void cblTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidarVisibilidadeCHK();
        }

        public void ValidarVisibilidadeCHK()
        {
            // Repopular todos os itens
            PopularCheckBoxList();

            // Se não for PRODUTO, remover IND e SIS
            if (cblTipo.SelectedValue != "PROD")
            {
                // Remover pelos valores
                ListItem itemIND = cblPrincipal.Items.FindByValue("IND");
                ListItem itemSIS = cblPrincipal.Items.FindByValue("SIS");
                ListItem itemEPI = cblPrincipal.Items.FindByValue("EPI");
                ListItem itemEMB = cblPrincipal.Items.FindByValue("EMB");
                ListItem item = cblPrincipal.Items.FindByText("Categorias");

                if (itemIND != null) cblPrincipal.Items.Remove(itemIND);
                if (itemSIS != null) cblPrincipal.Items.Remove(itemSIS);
                if (itemEPI != null) cblPrincipal.Items.Remove(itemEPI);
                if (itemEMB != null) cblPrincipal.Items.Remove(itemEMB);
                if (item != null) cblPrincipal.Items.Remove(item);
            }
            if (cblTipo.SelectedValue == "SER")
            {
                divRegraFiscal.Visible = true;
            }
            else
            {
                divRegraFiscal.Visible = false;
            }
        }

        private bool ValidarDados()
        {
            string valida = "N";

            if (txtsDscTipoProduto.Text == "")
            {
                MensagemPagina.MostraMensagem_Erro("Informe um Descrição válida para o cadastro");
                return false;
            }

            foreach (ListItem item in cblTipo.Items)
            {
                if (item.Value == "PROD" && item.Selected)
                {
                    if (valida == "S")
                    {
                        MensagemPagina.MostraMensagem_Erro("Não é possível selecionar mais de um Tipo!");
                        return false;
                    }

                    valida = "S";
                }
                //if (item.Value != "PROD" && item.Selected)
                //{
                //    if (cblPrincipal.Items.FindByValue("IND").Selected)
                //    {
                //        MensagemPagina.MostraMensagem_Erro("Apenas Produtos podem ser Industrializados!");
                //        return false;
                //    }
                //    else if (cblPrincipal.Items.FindByValue("SIS").Selected)
                //    {
                //        MensagemPagina.MostraMensagem_Erro("Apenas Produtos podem ser Sistemas!");
                //        return false;
                //    }
                //    else if (cblPrincipal.Items.FindByValue("EPI").Selected)
                //    {
                //        MensagemPagina.MostraMensagem_Erro("Apenas Produtos podem ser EPI!");
                //        return false;
                //    }
                //}
                if (item.Value == "SER" && item.Selected)
                {
                    if (valida == "S")
                    {
                        MensagemPagina.MostraMensagem_Erro("Não é possível selecionar mais de um Tipo!");
                        return false;
                    }

                    valida = "S";
                }
                if (item.Value != "SER" && item.Selected)
                {
                    if (ddlTipoRegra.SelectedValue != "0")
                    {
                        MensagemPagina.MostraMensagem_Erro("Apenas Serviços podem possuir um Tipo de Regra Fiscal!");
                        return false;
                    }
                }
                if (item.Value == "SUB_SER" && item.Selected)
                {
                    if (valida == "S")
                    {
                        MensagemPagina.MostraMensagem_Erro("Não é possível selecionar mais de um Tipo!");
                        return false;
                    }

                    valida = "S";
                }
                if (item.Value == "REC" && item.Selected)
                {
                    if (valida == "S")
                    {
                        MensagemPagina.MostraMensagem_Erro("Não é possível selecionar mais de um Tipo!");
                        return false;
                    }

                    valida = "S";
                }
                if (item.Value == "FAB" && item.Selected)
                {
                    if (valida == "S")
                    {
                        MensagemPagina.MostraMensagem_Erro("Não é possível selecionar mais de um Tipo!");
                        return false;
                    }

                    valida = "S";
                }
            }

            if (valida == "N")
            {
                cblTipo.Items.FindByValue("PROD").Selected = true;

                if (ddlTipoRegra.SelectedValue != "0")
                {
                    MensagemPagina.MostraMensagem_Erro("Caso nenhuma opção seja selecionada, o Tipo será considerado como um Tipo de Produtos, no entanto apenas Serviços podem possuir um Tipo de Regra Fiscal!");
                    return false;
                }
            }

            return true;
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {
                    string[] vidTipoProduto = hddidTipoProduto.Value.Split(',');
                    string idTipoProduto = vidTipoProduto[0].ToString();

                    string sComposicao = "N";
                    string sComercial = "N";
                    string sEmbalagem = "N";
                    // Vittorio --------
                    string sEPI = "N";
                    //--------------
                    string sIndustrializado = "N";
                    string sSistema = "N";
                    string sProduto = "N";
                    string sServico = "N";
                    string sRecurso = "N";
                    string sFabricacao = "N";
                    string sSub = "N";

                    foreach (ListItem item in cblPrincipal.Items)
                    {
                        if (item.Selected && item.Value == "COMP")
                            sComposicao = "S";
                        if (item.Selected && item.Value == "COMER")
                            sComercial = "S";
                        if (item.Selected && item.Value == "EMB")
                            sEmbalagem = "S";
                        // Vittorio --------
                        if (item.Selected && item.Value == "EPI")
                            sEPI = "S";
                        //-------------------
                        if (item.Selected && item.Value == "IND")
                            sIndustrializado = "S";
                        if (item.Selected && item.Value == "SIS")
                            sSistema = "S";
                    }

                    foreach (ListItem item in cblTipo.Items)
                    {
                        if (item.Selected && item.Value == "PROD")
                            sProduto = "S";
                        if (item.Selected && item.Value == "SER")
                            sServico = "S";
                        if (item.Selected && item.Value == "REC")
                            sRecurso = "S";
                        if (item.Selected && item.Value == "SUB_SER")
                            sSub = "S";
                        if (item.Selected && item.Value == "FAB")
                            sFabricacao = "S";
                    }

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idTipoProduto", idTipoProduto);
                    vParametros.Add("@sDscTipoProduto", txtsDscTipoProduto.Text);
                    vParametros.Add("@sComposicao", sComposicao);
                    vParametros.Add("@sExibeComercial", sComercial);
                    vParametros.Add("@sEmbalagem", sEmbalagem);
                    // Vittorio --------
                    vParametros.Add("@sEPI", sEPI);
                    //------------------
                    vParametros.Add("@sSituacao", ComboAtivo.Situacao_Recuperar());
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    vParametros.Add("@sProduto", sProduto);
                    vParametros.Add("@sServico", sServico);
                    vParametros.Add("@sRecurso", sRecurso);
                    vParametros.Add("@sSubTipo", sSub);
                    vParametros.Add("@sFabricacao", sFabricacao);
                    vParametros.Add("@sIndustrializado", sIndustrializado);
                    vParametros.Add("@sSistema", sSistema);
                    vParametros.Add("@idTipoRegra", ddlTipoRegra.SelectedValue);

                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idTipoProduto = RETORNO.DATASET(dsSalvar, 0, "idTipoProduto");
                        FUNCOES.DirecionaPagina(string.Format("App/Paginas/WMS/Manutencao/TipoProduto_Detalhe.aspx?id={0}&msg=1", idTipoProduto));
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
    }
}