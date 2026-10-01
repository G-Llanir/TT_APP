using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace TT_Flow.App.Paginas.Requisicao
{
    public partial class Requisicao_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Requisição";
        string sProcedure = "sp_Manipula_tbl_Flow_Requisicao";
        string sProcedure_Cotacao = "sp_Manipula_tbl_Flow_Comercial_CotacaoCompras";
        string idCotacao;
        public List<FrameWork.cls_Requisicao_Itens> bs_Requisicao_Itens
        {
            get
            {
                if (ViewState["bs_Requisicao_Itens"] == null)
                {
                    ViewState["bs_Requisicao_Itens"] = new List<FrameWork.cls_Requisicao_Itens>();
                }
                return (List<FrameWork.cls_Requisicao_Itens>)ViewState["bs_Requisicao_Itens"];
            }

            set
            {
                ViewState["bs_Requisicao_Itens"] = value;
            }

        }

        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlsUnidade, "sp_Select 'Flow_Produtos_Unidade'", "sUnidade", "sDscUnidade", false, "Selecione a unidade ", "0");
            FUNCOES.Popula_Combo(ddlidDepartamento, "sp_Select 'Flow_Departamentos_Requisicao'", "idDepartamento", "sDscDepartamento", false, "Selecione o Departamento ", "0");
            FUNCOES.Popula_Combo(ddlsSolicitante, "sp_Select 'Usuarios'", "idUsuario", "sDscUsuario", false, "Selecione o Solicitante", "0");
            FUNCOES.Popula_Combo(ddlidTipoRequisicao, "sp_Select 'Flow_Requisicao_Tipo'", "idTipoRequisicao", "sDscTipoRequisicao", false, "Selecione o Tipo de Requisição ", "0");

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidarPermissaoAcesso();
            if (!IsPostBack)
            {                
                PopularCombos();

                if (Request["id"] != null)
                {
                    Pesquisar(Request["id"].ToString(), false);
                }
                else
                {
                    Pesquisar("0", true);

                }
                if (Session["MensagemSucessoCotação"] != null)
                {
                    string mensagem = Session["MensagemSucessoCotação"].ToString();

                    MensagemPagina.MostraMensagem_Sucesso(mensagem);

                    Session["MensagemSucessoCotação"] = null;
                }
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
                    Salvar_Requisicao();
                }
                else if (requestTarget == "funcao_Editar")
                {
                    Pesquisar(hddIdRequisicao.Value, true);
                }
            }


        }

        protected void Pesquisar(string idRequisicao, bool bEdicao)
        {
            PopularCombos();
            aba_cotacao.Visible = false;
            aba_Historico.Visible = false;
            aba_Arquivos.Visible = false;
            aba_producao.Visible = false;
            cmdEditar.Visible = false;
            cmdAcao.Visible = false;
            cmdAlterarStatus.Visible = false;
            lblTituloStatus.Visible = false;

            string sErro = "";
            try
            {
                LimpaCampos();

                if (idRequisicao != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idRequisicao", idRequisicao);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    FUNCOES.Popula_Combo(ddlsSolicitante, "sp_Select 'tbl_Usuarios'", "idUsuario", "sDscUsuario", false, "Selecione o Solicitante", "0");

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddIdRequisicao.Value = RETORNO.DATASET(dsPesquisa, 0, "idRequisicao");
                        txtdtRequisicao.Text = RETORNO.DATASET(dsPesquisa, 0, "dtRequisicao");
                        txtsReferencia.Text = RETORNO.DATASET(dsPesquisa, 0, "sReferencia");
                        ddlsSolicitante.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idUsuarioRequisicao");
                        // hddid
                        ddlidDepartamento.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idDepartamento");
                        txtsCentroCusto.Text = RETORNO.DATASET(dsPesquisa, 0, "sCentroCusto");
                        ddlidTipoRequisicao.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idTipoRequisicao");
                        txtsDscMotivoRequisicao.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscMotivoRequisicao");
                        txtsObservacao.Text = RETORNO.DATASET(dsPesquisa, 0, "sObservacao");
                        lblTituloStatus.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscStatus");

                        hddidDepartamento.Value = RETORNO.DATASET(dsPesquisa, 0, "idDepartamento");
                        hddidSolicitante.Value = RETORNO.DATASET(dsPesquisa, 0, "idUsuarioRequisicao");
                        hddidTipReqAux.Value = RETORNO.DATASET(dsPesquisa, 0, "idTipoRequisicao");

                        int idStatus = Convert.ToInt32(RETORNO.DATASET(dsPesquisa, 0, "idStatusAtual"));

                        //Thiago Rodrigues 25/10/2024
                        if (RETORNO.DATASET(dsPesquisa, 0, "sGeraCotacao") != "S")
                        {
                            cmdCotar.Visible = false;
                        }
                        else
                        {
                            cmdCotar.Visible = true;
                        }
                        hddsTipoRequisicao.Value = RETORNO.DATASET(dsPesquisa, 0, "sTipoRequisicao");
                        hddsFabricacao.Value = RETORNO.DATASET(dsPesquisa, 0, "sFabricacao");
                        if (hddsFabricacao.Value == "S")
                        {                          
                            CarregarGridProdutosFabricacao(idRequisicao);
                        }

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        lblTituloPagina.Text = string.Format("Requisição N.º {0} - {1}", RETORNO.DATASET(dsPesquisa, 0, "idRequisicao"), ddlidTipoRequisicao.SelectedItem);
                        BreadCrumb.TitulodaPagina = string.Format("Requisição {0}", RETORNO.DATASET(dsPesquisa, 0, "idRequisicao"));

                        lblTituloSalvar.Text = "Confirma a Alteração da " + lblTituloPagina.Text + "?";
                        lblTituloEdiar.Text = "Deseja editar a Requisição " + lblTituloPagina.Text + "?";
                        lblTituloStatus.CssClass = string.Format("label label-{0}", RETORNO.DATASET(dsPesquisa, 0, "sCor"));

                        caixaTitulo.Attributes["class"] = string.Format("well-lg label-{0}", RETORNO.DATASET(dsPesquisa, 0, "sCor"));


                        Popular_dtgItens(dsPesquisa);
                        AlterarEstadoControles(bEdicao);


                        if (!bEdicao)
                        {
                            switch (RETORNO.DATASET(dsPesquisa, "sAcao"))
                            {
                                case "Aprovar":

                                    if (idStatus == 1)
                                    {
                                        foreach (DataRow row in dsPesquisa.Tables[3].Rows)
                                        {
                                            if (row["idUsuario"].ToString() == IDENTITY.Variaveis.idUsuario())
                                                cmdAcao.Visible = true;
                                        }
                                    }
                                    else if (idStatus == 13)
                                    {
                                        foreach (DataRow row in dsPesquisa.Tables[4].Rows)
                                        {
                                            if (row["idUsuario"].ToString() == IDENTITY.Variaveis.idUsuario())
                                                cmdAcao.Visible = true;
                                        }
                                    }

                                    break;

                                case "Cotacao":
                                    aba_cotacao.Visible = true;
                                    break;

                                case "Iniciar":
                                    if (RETORNO.DATASET(dsPesquisa, 0, "sTipoRequisicao") == "Compras")
                                    {
                                        aba_cotacao.Visible = true;
                                    }
                                    else
                                    {
                                        aba_producao.Visible = true;
                                    }

                                    break;

                                case "AlterarStatus":
                                    cmdAlterarStatus.Visible = true;
                                    break;                   
                            }

                            Popular_Aba_Historico(dsPesquisa);
                            Popular_Aba_Arquivos(idRequisicao);
                            lblTituloStatus.Visible = true;

                            if (RETORNO.DATASET(dsPesquisa, 0, "sPermiteEdicao") == "S")
                            {
                                cmdEditar.Visible = true;
                            }
                        }

                    }
                    else
                    {
                        throw new Exception(sErro);
                    }

                }
                else
                {
                    cmdCotar.Visible = false;
                    BreadCrumb.TitulodaPagina = string.Format("Nova {0}", sTituloPagina);
                    lblTituloPagina.Text = string.Format("Nova {0}", sTituloPagina);
                    lblTituloSalvar.Text = "Confirma a Inclusão da Requisição?";
                    cmdSalvar.Text = "Incluir";
                    txtdtRequisicao.Text = DateTime.Today.ToString("dd/MM/yyyy");
                    ddlsSolicitante.SelectedValue = IDENTITY.Variaveis.idUsuario();

                }
                RegistraScript("");
                ddlidTipoRequisicao.Focus();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }

        void Popular_Aba_Historico(DataSet ds)
        {
            gv_Historico.DataSource = ds.Tables[2];
            gv_Historico.DataBind();
            aba_Historico.Visible = true;
        }

        void Popular_Aba_Arquivos(string idPedido)
        {
            frmArquivos.Attributes.Add("src", string.Format("../Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}", idPedido, "Requisicao"));
            aba_Arquivos.Visible = true;
        }

        void AlterarEstadoControles(bool bAtivo)
        {
            cmdSalvar.Visible = false;
            cmdEditar.Visible = false;

            ddlsSolicitante.Attributes.Remove("disabled");
            ddlidDepartamento.Attributes.Remove("disabled");
            ddlidTipoRequisicao.Attributes.Remove("disabled");

            txtdtRequisicao.ReadOnly = !bAtivo;
            txtsReferencia.ReadOnly = !bAtivo;
            txtsCentroCusto.ReadOnly = !bAtivo;
            txtsDscMotivoRequisicao.ReadOnly = !bAtivo;
            txtsObservacao.ReadOnly = !bAtivo;


            if (!bAtivo)
            {
                ddlsSolicitante.Attributes.Add("disabled", "disabled");
                ddlidDepartamento.Attributes.Add("disabled", "disabled");
                ddlidTipoRequisicao.Attributes.Add("disabled", "disabled");
                div_SelecaoItens.Visible = false;
                this.dtgItens.Columns[6].Visible = false;
            }
            else
            {
                cmdSalvar.Visible = true;
                div_SelecaoItens.Visible = true;
                this.dtgItens.Columns[6].Visible = true;
            }


        }

        void Popular_dtgItens(DataSet dsPesquisa)

        {
            foreach (DataRow row in dsPesquisa.Tables[1].Rows)
            {
                FrameWork.cls_Requisicao_Itens objItem = new FrameWork.cls_Requisicao_Itens();
                objItem.idRequisicao = Convert.ToInt32(row["idRequisicao"].ToString());
                objItem.sCodigo = HttpUtility.HtmlDecode(row["sCodigo"].ToString());
                objItem.sDscProduto = HttpUtility.HtmlDecode(row["sDscProduto"].ToString()); //Thiago Rodrigues 29/11/2024
                objItem.sUnidade = row["sUnidade"].ToString();
                objItem.nQuantidade = row["nQuantidade"].ToString();
                objItem.dtPrevisaoUso = row["dtPrevisaoUso"].ToString();
                objItem.idProduto = Convert.ToInt32(row["idProduto"].ToString());

                bs_Requisicao_Itens.Add(objItem);
            }

            if (bs_Requisicao_Itens.Count == 0)
            {
                div_GerarPDF.Visible = false;
            }
            else
            {
                div_GerarPDF.Visible = true;
            }

            dtgItens_DataBind();
        }


        void dtgItens_DataBind()
        {
            dtgItens.DataSource = bs_Requisicao_Itens;
            dtgItens.DataBind();
        }

        void LimpaCampos()
        {
            hddIdRequisicao.Value = "0";

            txtdtRequisicao.Text = "";
            txtsReferencia.Text = "";
            ddlsSolicitante.SelectedValue = "0";
            ddlidDepartamento.SelectedValue = "0";
            txtsCentroCusto.Text = "";
            ddlidTipoRequisicao.SelectedValue = "0";
            txtsDscMotivoRequisicao.Text = "";
            txtsObservacao.Text = "";

            //Grid Itens
            txtsCodigoProduto.Text = "";
            txtsDscProduto.Text = "";
            ddlsUnidade.SelectedValue = "0";
            txtnQuantidade.Text = "";
            txtdtPrevisaoUso.Text = "";

            bs_Requisicao_Itens.Clear();


            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
        }

        private bool ValidarDados()
        {
            if (ddlidTipoRequisicao.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Tipo de requisição inválida!");//---------------
                return false;

            }
            if (txtdtRequisicao.Text.Length < 6)
            {
                MensagemPagina.MostraMensagem_Erro("Data inválida!");
                return false;

            }
            if (txtsReferencia.Text.Length < 4)
            {
                MensagemPagina.MostraMensagem_Erro("Referência inválida!");
                return false;
            }
            if (ddlsSolicitante.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Solicitante inválido!");//---------------
                return false;
            }
            if (ddlidDepartamento.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Departamento inválido!"); //---------------
                return false;

            }
            //if (txtsCentroCusto.Text.Length < 6)
            //{
            //    MensagemPagina.MostraMensagem_Erro("Centro de Custo Inválido!");
            //    return false;

            //}

            if (txtsDscMotivoRequisicao.Text.Length < 8)
            {
                MensagemPagina.MostraMensagem_Erro("Motivo da solicitação inválida!");
                return false;

            }


            return true;
        }

        private bool ValidarDados_Itens(ref string sMensagem)
        {

            if (txtsCodigoProduto.Text.Length < 3)
            {
                MensagemItem.MostraMensagem_Erro("Código do Produto Inválido");
                return false;
            }
            if (txtsDscProduto.Text.Length < 6)
            {
                MensagemItem.MostraMensagem_Erro("Produto inválido!");
                return false;

            }
            if (ddlsUnidade.SelectedValue == "0")
            {
                MensagemItem.MostraMensagem_Erro("Selecione uma unidade!");
                return false;
            }
            if (txtnQuantidade.Text.Length < 1)
            {
                MensagemItem.MostraMensagem_Erro("Quantidade inválida!");
                return false;

            }
            if (txtdtPrevisaoUso.Text.Length < 5)
            {
                MensagemItem.MostraMensagem_Erro("Data inválida!");
                return false;

            }

            return true;
        }

        void Salvar_Requisicao()
        {
            string sErro = "";
            if (ValidarDados())
            {

                try
                {
                    string[] vidRequisicao = hddIdRequisicao.Value.Split(',');
                    string idRequisicao = vidRequisicao[0].ToString();

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idRequisicao", idRequisicao);
                    vParametros.Add("@dtRequisicao", txtdtRequisicao.Text);
                    vParametros.Add("@sReferencia", txtsReferencia.Text);
                    vParametros.Add("@idUsuarioRequisicao", ddlsSolicitante.SelectedValue);
                    vParametros.Add("@idDepartamento", ddlidDepartamento.SelectedValue);
                    vParametros.Add("@sCentroCusto", txtsCentroCusto.Text);

                    vParametros.Add("@idTipoRequisicao", ddlidTipoRequisicao.SelectedValue);
                    hddidTipoRequisicao.Value = ddlidTipoRequisicao.SelectedValue;

                    vParametros.Add("@sDscMotivoRequisicao", txtsDscMotivoRequisicao.Text);
                    vParametros.Add("@sObservacao", txtsObservacao.Text);
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idRequisicao = RETORNO.DATASET(dsSalvar, 0, "idRequisicao");
                        if (SSalvar_Requisicao_Itens(idRequisicao))
                        {
                            Pesquisar(idRequisicao, false);
                            MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                        }
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
            RegistraScript("");
        }

        bool SSalvar_Requisicao_Itens(string idRequisicao)
        {
            bool bRetorno = false;

            try
            {
                DataSet dsItens_Excluir;
                Dictionary<String, String> vParametroItens_Excluir = new Dictionary<string, string>();
                vParametroItens_Excluir.Add("@sFuncao", "EXCLUIR ITEM");
                vParametroItens_Excluir.Add("@idRequisicao", idRequisicao);
                dsItens_Excluir = BD.ExecutarDataSet(sProcedure, vParametroItens_Excluir);

                foreach (GridViewRow item in dtgItens.Rows)
                {
                    TextBox txtQuantidade = (TextBox)item.FindControl("txtQuantidade");
                    TextBox txtPrevisaoUso = (TextBox)item.FindControl("txtPrevisaoUso");
                    LinkButton lnkProduto = (LinkButton)item.FindControl("lnkProdutoDetalhe");

                    string sCodigo = dtgItens.DataKeys[item.RowIndex].Value.ToString();
                    string dtPrevisaoUso = "";
                    decimal nQuantidade = 0;

                    if (!string.IsNullOrWhiteSpace(txtQuantidade.Text))
                        decimal.TryParse(txtQuantidade.Text.Replace(".", ","), out nQuantidade);

                    if (!string.IsNullOrWhiteSpace(txtPrevisaoUso.Text))
                    {
                        DateTime dtTemp;
                        if (DateTime.TryParseExact(txtPrevisaoUso.Text, "dd/MM/yyyy",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None, out dtTemp))
                        {
                            dtPrevisaoUso = dtTemp.ToString("dd/MM/yyyy");
                        }
                        else
                            dtPrevisaoUso = item.Cells[5].Text;                            
                    }

                    string sDscProduto = lnkProduto != null ? lnkProduto.Text : "";
                    string sUnidade = item.Cells[3].Text;                    

                    if (idRequisicao != "0")
                    {
                        DataSet dsItens_Incluir;
                        Dictionary<String, String> vParametroItens_Incluir = new Dictionary<string, string>();

                        vParametroItens_Incluir.Add("@sFuncao", "INSERIR ITEM");
                        vParametroItens_Incluir.Add("@idRequisicao", idRequisicao);

                        vParametroItens_Incluir.Add("@sCodigo", sCodigo);
                        vParametroItens_Incluir.Add("@sDscProduto", sDscProduto);
                        vParametroItens_Incluir.Add("@sUnidade", sUnidade);
                        vParametroItens_Incluir.Add("@nQuantidade", nQuantidade.ToString());
                        vParametroItens_Incluir.Add("@dtPrevisaoUso", dtPrevisaoUso);
                        dsItens_Incluir = BD.ExecutarDataSet(sProcedure, vParametroItens_Incluir);
                    }

                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

            return bRetorno;

        }

        protected void cmdIncluirItem_Click(object sender, EventArgs e)
        {
            string sMensagem = "";

            if (ValidarDados_Itens(ref sMensagem))
            {
                string idProduto = hddidProduto.Value;

                FrameWork.cls_Requisicao_Itens objItem = new FrameWork.cls_Requisicao_Itens();

                string[] vidRequisicao = hddIdRequisicao.Value.Split(',');
                string idRequisicao = vidRequisicao[0].ToString();

                objItem.idRequisicao = Convert.ToInt32(idRequisicao);

                objItem.sCodigo = txtsCodigoProduto.Text;
                objItem.sDscProduto = txtsDscProduto.Text;
                objItem.sUnidade = ddlsUnidade.SelectedValue;
                objItem.nQuantidade = txtnQuantidade.Text;
                objItem.dtPrevisaoUso = txtdtPrevisaoUso.Text;
                objItem.idProduto = Convert.ToInt32(hddidProduto.Value);

                bs_Requisicao_Itens.Add(objItem);
                dtgItens_DataBind();


                ddlsUnidade.SelectedValue = "0";
                txtsCodigoProduto.Text = "";
                txtsDscProduto.Text = "";
                txtnQuantidade.Text = "";
                txtdtPrevisaoUso.Text = "";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus", "$('[id$=txtsCodigoProduto]').focus();", true);
            }
            else
            {
                //MensagemAcoes.MostraMensagem_Erro(sMensagem);
                //lblMensagem_Selecao.Text = sMensagem;
                //lblMensagem_Selecao.Visible = true;
            }
            RegistraScript("");
        }

        protected void dtgItens_RowDataBound1(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0);
        }

        protected void dtgItens_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0);

            //Thiago Rodrigues 29/11/2024
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //var sDscProdutoCell = e.Row.Cells[2];
                //var sCodigoCell = e.Row.Cells[1];

                //string sDscProduto = HttpUtility.HtmlDecode(sDscProdutoCell.Text);
                //string sCodigo = HttpUtility.HtmlDecode(sCodigoCell.Text);

                //sDscProdutoCell.Text = sDscProduto;
                //sCodigoCell.Text = sCodigo;
            }
        }

        protected void dtgItens_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            bs_Requisicao_Itens.RemoveAt(index);
            dtgItens_DataBind();
        }

        protected void txtsCodigoProduto_TextChanged(object sender, EventArgs e)
        {
            SqlDataReader sdr = BD.ExecutarDataReader("sp_Select 'FLOW_Produtos_Codigo', 0, 'S', '" + txtsCodigoProduto.Text + "'");

            while (sdr.Read())
            {
                txtsDscProduto.Text = sdr["sDscProduto"].ToString();
                ddlsUnidade.SelectedValue = sdr["sUnidade"].ToString();
                hddidProduto.Value = sdr["idItem"].ToString();
            }

            RegistraScript("");
        }

        //Fim--------------------------------------------------------------------------------------------------------------------------------------------

        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();


            sb.Append("$v192(function() {");
            sb.Append("$v192(\"[id$=txtsDscProduto]\").autocomplete({");
            sb.Append("source: function(request, response) {");
            sb.Append("$v192.ajax({");

            sb.Append("url: '/app/Paginas/Requisicao/Requisicao_Detalhe.aspx/GetProdutos',");
            sb.Append("data: \"{ 'sDscProduto': '\" + request.term + \"'}\",");
            sb.Append("dataType: \"json\",");

            sb.Append("type: \"POST\",");
            sb.Append("contentType: \"application/json; charset=utf-8\",");

            sb.Append("success: function(data) {");
            sb.Append("response($v192.map(data.d, function(item) {");
            sb.Append("return {");

            sb.Append("label: item.split('|')[0],");
            sb.Append("val: item.split('|')[2],");
            sb.Append("idProd: item.split('|')[1],");
            sb.Append("un: item.split('|')[3]");
            sb.Append("}");
            sb.Append("}))");
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
            sb.Append("$(\"[id$=txtsCodigoProduto]\").val(i.item.val);");
            sb.Append("$(\"[id$=ddlsUnidade]\").val(i.item.un);");
            sb.Append("$(\"[id$=hddidProduto]\").val(i.item.idProd);"); 
            sb.Append("$('[id$=txtnQuantidade]').focus();");

            sb.Append("},");

            sb.Append("minLength: 3");
            sb.Append("});});");


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
            sb.Append("$v192('[id*=cmdSalvar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Salvar').dialog('open');");
            sb.Append("});");

            sb.Append("$v192(\"#dialog-Editar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_Editar\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=cmdEditar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Editar').dialog('open');");
            sb.Append("});");

            sb.Append("});");
            //Caixa de seleção de datas
            sb.Append("$(function() {$('[id*=txtdtRequisicao]').datepicker({");
            sb.Append("autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'});});");

            sb.Append("$(function() {$('[id*=txtdtPrevisaoUso]').datepicker({");
            sb.Append("autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'});});");

            //Card Produtos
            sb.Append("     var cardTimer = { };\r\n");
            sb.Append("     function mostraCard(element, idProduto, tabela) {\r\n");
            sb.Append("         cardTimer[idProduto + '_' + tabela] = setTimeout(function() {\r\n");
            sb.Append("             $.ajax({\r\n");
            sb.Append("                 url: \"/API/Pagina_Ajax.aspx/GetProdutoDetalhes\",\r\n");
            sb.Append("                 data: JSON.stringify({ idProduto: idProduto }),\r\n");
            sb.Append("                 type: 'POST',\r\n");
            sb.Append("                 dataType: 'json',\r\n");
            sb.Append("                 contentType: 'application/json; charset=utf-8',\r\n");
            sb.Append("                 success: function(response) {\r\n");
            sb.Append("                     var produto = JSON.parse(response.d);\r\n");
            sb.Append("                     var cardProduto = `\r\n");
            sb.Append("                         <div class=\"card\">\r\n");
            sb.Append("                             <div class=\"card-body d-flex\">\r\n");
            sb.Append("                                 <div class=\"flex-shrink-0\" style=\"min-inline-size: fit-content;\">\r\n");
            sb.Append("                                     ${produto.imagem? `<img src = \"${produto.imagem}\" alt=\"Imagem do Produto\" class=\"img-fluid img-thumbnail\" style=\"width: 100px; height: auto;\" />` : ''}\r\n");
            sb.Append("                                 </div>\r\n");
            sb.Append("                                 <div class=\"flex-grow-1 d-flex flex-column ms-3\">\r\n");
            sb.Append("                                     <div class=\"d-flex\">\r\n");
            sb.Append("                                         ${produto.sCategoriaVendas? `<div class=\"card-text me-3\"> <strong>Categoria Vendas: </strong>${produto.sCategoriaVendas\r\n}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sFabricante ? `<div class= \"card-text me-3\"> <strong > Fabricante: </strong >${ produto.sFabricante}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sTipo ? `<div class= \"card-text me-3\"> <strong > Tipo: </strong >${ produto.sTipo}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sGrupo ? `<div class= \"card-text me-3\"> <strong > Grupo: </strong >${ produto.sGrupo}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sFamilia ? `<div class= \"card-text me-3\"> <strong > Família: </strong >${ produto.sFamilia}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sPaisOrigem ? `<div class= \"card-text me-3\"> <strong > Origem: </strong >${ produto.sPaisOrigem}</div>` : ''}\r\n");
            sb.Append("                                         ${ produto.sLocalArmazenamento ? `<div class= \"card-text me-3\"> <strong > Local Armazenamento: </strong >${ produto.sLocalArmazenamento}</div>` : ''}\r\n");
            sb.Append("                                     </div>\r\n");
            sb.Append("                                 </div>\r\n");
            sb.Append("                             </div>\r\n");
            sb.Append("                         </div>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                     var cardId = idProduto + '_' + tabela;\r\n");
            sb.Append("                     var card = document.getElementById(cardId);\r\n");
            sb.Append("                     card.innerHTML = cardProduto;\r\n");
            sb.Append("                     var rect = element.getBoundingClientRect();\r\n");
            sb.Append("                     var scrollTop = document.documentElement.scrollTop || document.body.scrollTop;\r\n");
            sb.Append("                     var scrollLeft = document.documentElement.scrollLeft || document.body.scrollLeft;\r\n");
            sb.Append("                     hideAllCards();\r\n");
            sb.Append("                     card.style.top = (rect.top + scrollTop - 10) + 'px';\r\n");
            sb.Append("                     card.style.left = (rect.right + scrollLeft + element.offsetWidth + 10) + 'px';\r\n");
            sb.Append("                     card.style.display = 'block';\r\n");
            sb.Append("                 },\r\n");
            sb.Append("                 error: function(error) {\r\n");
            sb.Append("                     console.error(\"Erro ao obter os detalhes do produto:\", error);\r\n");
            sb.Append("                 }\r\n");
            sb.Append("             });\r\n");
            sb.Append("         }, 300);\r\n");
            sb.Append("     }\r\n\r\n");
            sb.Append("     function escondeCard(idProduto, tabela) {\r\n");
            sb.Append("         var cardId = idProduto + '_' + tabela;\r\n");
            sb.Append("         var card = document.getElementById(cardId);\r\n");
            sb.Append("         clearTimeout(cardTimer[idProduto + '_' + tabela]);\r\n");
            sb.Append("         card.style.display = 'none';\r\n");
            sb.Append("     }\r\n\r\n");
            sb.Append("     function hideAllCards() {\r\n");
            sb.Append("         var cards = document.querySelectorAll('.product-card');\r\n");
            sb.Append("         cards.forEach(function(card) {\r\n");
            sb.Append("             card.style.display = 'none';\r\n");
            sb.Append("         });\r\n");
            sb.Append("     }\r\n\r\n");
            sb.Append("     function openModal(idProduto) {\r\n");
            sb.Append("         $.ajax({\r\n");
            sb.Append("             url: \"/API/Pagina_Ajax.aspx/GetProdutoDetalhes\",\r\n");
            sb.Append("             data: JSON.stringify({ idProduto: idProduto }),\r\n");
            sb.Append("             type: 'POST',\r\n");
            sb.Append("             dataType: 'json',\r\n");
            sb.Append("             contentType: 'application/json; charset=utf-8',\r\n");
            sb.Append("             success: function(response) {\r\n");
            sb.Append("                 var produto = JSON.parse(response.d);\r\n");
            sb.Append("                 var tituloProduto = `\r\n");
            sb.Append("                     <button type = \"button\" class= \"close\" data - dismiss = \"modal\" aria - label = \"Close\">\r\n");
            sb.Append("                         <span aria - hidden = \"true\" > &times;</span>\r\n");
            sb.Append("                     </button>\r\n");
            sb.Append("                     <h5 class= \"modal-title\" id = \"detailsModalLabel\" > ${ produto.sCodigo} - ${ produto.sDsc}</h5>\r\n");
            sb.Append("                 `;\r\n");
            sb.Append("                 var modalInfo = document.getElementById('modalInfo');\r\n");
            sb.Append("                 modalInfo.innerHTML = tituloProduto;\r\n");
            sb.Append("                 var imagem = '';\r\n");
            sb.Append("                 if (produto.imagem) {\r\n");
            sb.Append("                     imagem += `\r\n");
            sb.Append("                         <div style = \"text-align: center; margin-bottom: 20px;\">\r\n");
            sb.Append("                             <img src = \"${produto.imagem}\" alt = \"Imagem do Produto\" class= \"img-fluid\" style = \"width: 300px; height: auto;\" />\r\n");
            sb.Append("                         </div>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 var tabelaProduto = '<table class=\"table table-bordered\">';\r\n");
            sb.Append("                 if (produto.sCategoriaVendas) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Categoria Vendas </th>\r\n");
            sb.Append("                             <td>${ produto.sCategoriaVendas}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sTipo) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Tipo </th>\r\n");
            sb.Append("                             <td>${ produto.sTipo}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sGrupo) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Grupo </th>\r\n");
            sb.Append("                             <td>${ produto.sGrupo}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sFabricante) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Fabricante </th>\r\n");
            sb.Append("                             <td>${ produto.sFabricante}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sLocalArmazenamento) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Local Armazenamento </th>\r\n");
            sb.Append("                             <td>${ produto.sLocalArmazenamento}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sFamilia) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Família </th>\r\n");
            sb.Append("                             <td>${ produto.sFamilia}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sCodigoCEST) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> CEST </th>\r\n");
            sb.Append("                             <td>${ produto.sCodigoCEST}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sCodigoNCM) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> NCM </th>\r\n");
            sb.Append("                             <td>${ produto.sCodigoNCM}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 if (produto.sPaisOrigem) {\r\n");
            sb.Append("                     tabelaProduto += `\r\n");
            sb.Append("                         <tr>\r\n");
            sb.Append("                             <th> Origem </th>\r\n");
            sb.Append("                             <td>${ produto.sPaisOrigem}</td>\r\n");
            sb.Append("                         </tr>\r\n");
            sb.Append("                     `;\r\n");
            sb.Append("                 }\r\n");
            sb.Append("                 tabelaProduto += `</table >`;\r\n");
            sb.Append("                 var modalBody = document.getElementById('modalBody');\r\n");
            sb.Append("                 modalBody.innerHTML = imagem + tabelaProduto;\r\n");
            sb.Append("                 $('#produtoDetalheModal').modal('show');\r\n");
            sb.Append("             },\r\n");
            sb.Append("             error: function(error) {\r\n");
            sb.Append("                 console.error(\"Erro ao obter os detalhes do produto:\", error);\r\n");
            sb.Append("             }\r\n");
            sb.Append("         });\r\n");
            sb.Append("     }\r\n");
            sb.Append("     function openProductDetail(idItem) {\r\n");
            sb.Append("         var url = '/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id=' + idItem;\r\n");
            sb.Append("         window.open(url, '_blank');\r\n");
            sb.Append("         return false;\r\n");
            sb.Append("     }\r\n");
            
            sb.Append("function SomenteNumero(e) {");
            sb.Append("     var tecla = (window.event) ? event.keyCode : e.which;");
            sb.Append("     if ((tecla >= 48 && tecla <= 57) || tecla == 8 || tecla == 9 || tecla == 13 || tecla == 37 || tecla == 39 || tecla == 40 || tecla == 44 || tecla == 188) { ");
            sb.Append("         return true;");
            sb.Append("     }");
            sb.Append("     return false;");
            sb.Append("}");            

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);
            Requisicao_Acao_EsconderCaixa();

        }

        [System.Web.Services.WebMethod]
        public static string[] GetProdutos(string sDscProduto)
        {
            List<string> lstProdutos = new List<string>();
            if (sDscProduto.Length > 3)
            {
                SqlDataReader sdr = BD.ExecutarDataReader("sp_Select 'FLOW_Produtos', 0, 'S', '" + sDscProduto + "'");
                while (sdr.Read())
                {
                    lstProdutos.Add(string.Format("{2}|{0}|{1}|{3}", sdr["idItem"], sdr["sCodigo"], sdr["sDscProduto"], sdr["sUnidade"]));
                }
            }
            return lstProdutos.ToArray();
        }

        protected void lnkIniciarRequisicao_Click(object sender, EventArgs e)
        {
            Requisicao_Acao_MostrarCaixa("Aprovar Requisição");
        }

        protected void lnkRejeitarRequisicao_Click(object sender, EventArgs e)
        {
            Requisicao_Acao_MostrarCaixa("Rejeitar Requisição");
        }

        void Requisicao_Acao_MostrarCaixa(string sTitulo)
        {
            div_AlterarStatus.Visible = false;
            cmdEditar.Enabled = false;
            cmdAcao.Visible = false;
            lblRequisicao_Acao_Titulo.Text = sTitulo;

            if (sTitulo == "Alterar Status")
            {
                FUNCOES.Popula_Combo(ddlAlterarStatus, "sp_Select 'Flow_Requisicao_Status', @sPesquisa = 'S'", "idStatus", "sDscStatus", false);
                div_AlterarStatus.Visible = true;
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Collapse", "$('#div_Requisicao_Acoes').collapse();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Focus", "$('[id$=txtRequisicao_Observacao]').focus();", true);
        }

        void Requisicao_Acao_EsconderCaixa()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Hide", "$('#div_Requisicao_Acoes').hide();", true);
        }

        protected void cmdRequisicao_Acao_OK_Click(object sender, EventArgs e)
        {
            string idStatus = "0";
            string sErro = "";

            if (lblRequisicao_Acao_Titulo.Text == "Alterar Status")
            {
                if (ddlAlterarStatus.SelectedValue == "0")
                {
                    MensagemPagina.MostraMensagem_Erro("Selecione o Status!");
                    return;
                }
                else
                {
                    idStatus = ddlAlterarStatus.SelectedValue;
                }
            }



            if (txtRequisicao_Observacao.Text.Length < 6)
            {
                MensagemPagina.MostraMensagem_Erro("Insira um Motivo/Observação válido!");
            }
            else
            {
                DataSet dsTarefas;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", lblRequisicao_Acao_Titulo.Text);
                vParametros.Add("@idRequisicao", hddIdRequisicao.Value);
                vParametros.Add("@sObservacao", txtRequisicao_Observacao.Text);

                if (string.IsNullOrEmpty(hddidTipoRequisicao.Value))
                    vParametros.Add("@idTipoRequisicao", ddlidTipoRequisicao.SelectedValue);
                else
                    vParametros.Add("@idTipoRequisicao", hddidTipoRequisicao.Value);

                vParametros.Add("@idStatus", idStatus);
                vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());

                dsTarefas = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsTarefas, out sErro))
                {
                    MensagemPagina.MostraMensagem_Sucesso(RETORNO.DATASET(dsTarefas, "msg"));
                    if(idStatus == "7")
                    {
                        if(FUNCOES.ValidaPermissao(Permissao.Fabricacao.Gerenciar, false))
                         AbrirModal(hddIdRequisicao.Value, "R");
                    }
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro(RETORNO.DATASET(dsTarefas, "msg"));
                }
                Pesquisar(hddIdRequisicao.Value, false);
                cmdEditar.Enabled = true;
                //cmdAcao.Visible = true;
                Requisicao_Acao_EsconderCaixa();
            }
        }

        protected void cmdRequisicao_Acao_Cancelar_Click(object sender, EventArgs e)
        {
            cmdEditar.Enabled = true;
            cmdAcao.Visible = true;
            Pesquisar(Request["id"].ToString(), false);
            Requisicao_Acao_EsconderCaixa();
        }

        protected void cmdAlterarStatus_Click(object sender, EventArgs e)
        {
            ddlidTipoRequisicao.SelectedValue = hddidTipReqAux.Value;
            ddlidDepartamento.SelectedValue = hddidDepartamento.Value;
            ddlsSolicitante.SelectedValue = hddidSolicitante.Value;
            //Pesquisar(Request["id"].ToString(), false);
            Requisicao_Acao_MostrarCaixa("Alterar Status");
        }

        //Thiago Rodrigues 25/10/2024
        #region | Cotação
        bool ValidarDadosCotacao()
        {
            return true;
        }
        protected void cmdCotar_Click(object sender, EventArgs e)
        {
            if (ValidarDadosCotacao())
            {
                try
                {
                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    string sErro = "";

                    //Cadastro
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@sDscCotacao", txtsDscMotivoRequisicao.Text);
                    vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());

                    vParametros.Add("@idComprador", ddlsSolicitante.SelectedValue);


                    //if (txtdtValidade.Text == "")
                    //{
                    //    vParametros.Add("@dtValidade", txtdtValidade.Text);
                    //}
                    //else
                    //{
                    //    DateTime dt = DateTime.Parse(txtdtValidade.Text.ToString(), CultureInfo.InvariantCulture);
                    //    vParametros.Add("@dtValidade", dt.ToString());
                    //}

                    vParametros.Add("@sObservacao", txtsObservacao.Text);
                    vParametros.Add("@idRequisicao", hddIdRequisicao.Value);

                    if (bs_Requisicao_Itens.Count() != 0)
                    {
                        dsSalvar = BD.ExecutarDataSet(sProcedure_Cotacao, vParametros);

                        if (BD.ValidarDataSet(dsSalvar, out sErro))
                        {
                            idCotacao = RETORNO.DATASET(dsSalvar, "idCotacao");
                            Salvar_Itens(idCotacao);
                            hddidCotacao.Value = idCotacao;

                            string linkCotacao = $"<a href='../Compras/CotacaoCompras_Detalhe.aspx?id={idCotacao}&sTp=7' target='_blank'>Cotação: {idCotacao}</a>";

                            Session["MensagemSucessoCotação"] = "Cotação Salva com sucesso! Acesse a Cotação e conclua as pendências: " + linkCotacao;

                            Response.Redirect($"/App/Paginas/Requisicao/Requisicao_Detalhe.aspx?id={hddIdRequisicao.Value}");

                            // MensagemPagina.MostraMensagem_Sucesso("Cotação Salva com sucesso! Acesse a Cotação: " + linkCotacao);
                        }
                        else
                        {
                            MensagemPagina.MostraMensagem_Erro("BD: " + sErro.ToString(), false);
                        }
                    }
                    else
                    {
                        dsSalvar = BD.ExecutarDataSet(sProcedure_Cotacao, vParametros);

                        if (BD.ValidarDataSet(dsSalvar, out sErro))
                        {
                            idCotacao = RETORNO.DATASET(dsSalvar, "idCotacao");
                            hddidCotacao.Value = idCotacao;

                            string linkCotacao = $"<a href='../Compras/CotacaoCompras_Detalhe.aspx?id={idCotacao}&sTp=7' target='_blank'>Cotação: {idCotacao}</a>";

                            Session["MensagemSucessoCotação"] = "Cotação Salva sem itens! Acesse a Cotação e Conclua o Cadastro: " + linkCotacao;

                            Response.Redirect($"/App/Paginas/Requisicao/Requisicao_Detalhe.aspx?id={hddIdRequisicao.Value}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro($"Erro: {ex}", false);
                }
            }
            Requisicao_Acao_EsconderCaixa();
        }

        bool Salvar_Itens(string idCotacao)
        {

            try
            {
                DataSet dsSalvar = new DataSet();
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                Dictionary<String, String> vParametrosSalvar = new Dictionary<string, string>();
                string sErro = "";
                vParametros.Add("@sFuncao", "EXCLUIR-ITENS");
                vParametros.Add("@idCotacao", idCotacao);
                BD.ExecutarDataSet(sProcedure_Cotacao, vParametros);

                vParametros.Clear();
                vParametrosSalvar.Add("@sFuncao", "SALVAR-ITENS");
                bs_Requisicao_Itens.ForEach(e =>
                {
                    //vParametrosSalvar["@idProduto"] = e.idRequisicao.ToString();
                    vParametrosSalvar["@sDscProduto"] = e.sDscProduto;
                    vParametrosSalvar["@sUnidade"] = e.sUnidade.ToString();
                    vParametrosSalvar["@sCodigo"] = e.sCodigo.ToString();
                    vParametrosSalvar["@idCotacao"] = idCotacao;
                    vParametrosSalvar["@nQuantidade"] = BD.Conversoes.Numerico(Convert.ToDecimal(e.nQuantidade));
                    vParametrosSalvar["@idUsuario"] = IDENTITY.Variaveis.idUsuario();
                    vParametrosSalvar["@dtPrevisao"] = e.dtPrevisaoUso;
                    vParametrosSalvar["@idRequisicao"] = hddIdRequisicao.Value;

                    dsSalvar = BD.ExecutarDataSet(sProcedure_Cotacao, vParametrosSalvar);
                });


                if (BD.ValidarDataSet(dsSalvar, out sErro))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro($"Erro: {ex}");
                return false;
            }
        }
        #endregion

        #region | PDF
        protected void cmdGerarPDF_Click(object sender, EventArgs e)
        {
            PDF_Requisicao(true);
        }
        void PDF_Requisicao(bool bPDF)
        {
            try
            {
                Microsoft.Reporting.WebForms.ReportViewer rv4 = new Microsoft.Reporting.WebForms.ReportViewer();

                rv4.ProcessingMode = ProcessingMode.Local;
                rv4.LocalReport.EnableExternalImages = true;

                rv4.LocalReport.ReportPath = Server.MapPath("~/App/Reports/Requisicao.rdlc");

                string sFuncao = "CONSULTAR-ITENS";

                DataSet dsRequisicao;
                string sSql = "sp_Manipula_tbl_Flow_Requisicao";
                Dictionary<String, String> vParametrosItens = new Dictionary<string, string>();
                vParametrosItens.Add("@sFuncao", sFuncao);
                vParametrosItens.Add("@idRequisicao", hddIdRequisicao.Value);

                dsRequisicao = BD.ExecutarDataSet(sSql, vParametrosItens);
                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@idRequisicao", hddIdRequisicao.Value);
                vParametros.Add("@sFuncao", "CONSULTAR");

                DataSet dsItens;
                dsItens = BD.ExecutarDataSet(sSql, vParametros);

                if (dsRequisicao.Tables.Count == 0 || dsRequisicao.Tables[0].Rows.Count == 0)
                {
                    MensagemPagina.MostraMensagem_Erro("Erro ao Gerar PDF: Nenhum dado encontrado.");
                    return;
                }
                if (dsItens.Tables.Count == 0 || dsItens.Tables[0].Rows.Count == 0)
                {
                    MensagemPagina.MostraMensagem_Aviso("Sem Itens Na Requisição");
                }


                rv4.LocalReport.DataSources.Clear();
                rv4.LocalReport.DataSources.Add(new ReportDataSource("dsItens", dsItens.Tables[0]));
                rv4.LocalReport.DataSources.Add(new ReportDataSource("dsRequisicao", dsRequisicao.Tables[0]));
                DataRow row = dsItens.Tables[0].AsEnumerable().FirstOrDefault();
                DataRow row2 = dsRequisicao.Tables[0].AsEnumerable().FirstOrDefault();

                if (row != null)
                {
                    ReportParameter[] rp = new ReportParameter[5];

                    rp[0] = new ReportParameter("idRequisicao", row["idRequisicao"].ToString());
                    //rp[1] = new ReportParameter("sReferencia", row["sReferencia"].ToString());
                    //rp[2] = new ReportParameter("sCliente", row["sCliente"].ToString());
                    //rp[3] = new ReportParameter("sStatus", row["sStatus"].ToString());
                    //rp[4] = new ReportParameter("dtOPI", row["dtOPI"].ToString());

                    rv4.LocalReport.SetParameters(rp);
                    rv4.LocalReport.Refresh();

                    Microsoft.Reporting.WebForms.Warning[] warnings;
                    string[] streamIds;
                    string mimeType, encoding, extension;

                    byte[] bytes = rv4.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                    string sNomeArquivo = "REQ_" + dsRequisicao.Tables[0].Rows[0]["sReferencia"].ToString() + "_" + FUNCOES.CarimboDataHora() + ".pdf";

                    File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivo, bytes);
                    Pesquisar(hddIdRequisicao.Value, false);
                    FUNCOES.DownloadArquivo(Page, sNomeArquivo);

                    MensagemPagina.MostraMensagem_Sucesso("PDF da Requisição gerada com sucesso!");
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro("Erro ao Gerar PDF");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao gerar o PDF da Requisição! </br>" + ex.Message);
            }
        }
        #endregion

        #region | Projetos 
        private void CarregarGridProdutosFabricacao(string idRequisicao)
        {
            var parametros = new Dictionary<string, string>
              {
                { "@sFuncao", "CONSULTAR-PROJETOS" },
                { "@idRequisicao", idRequisicao.ToString() }
            };

            var lista = BD.ExecutarLista<ProdutosFabricacao>(sProcedure, parametros, false);

            if(lista.Count > 0)
            {
                div_projetos.Visible = true;
                gvProdutosFabricacao.DataSource = lista;
                gvProdutosFabricacao.DataBind();
            }         
        }
        protected void AbrirModal(string id, string sTipoId)
        {           
            //object sender, EventArgs e      
            FecharModal("modalAtividades");
           
            if(GerenciadorMembrosModal.PopularModal(id, sTipoId, true))
               ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalEnvio", "$('#modalAtividades').modal('show');", true);
        }

        protected void FecharModal(string modalId)
        {
            string script = $@"
        $('#{modalId}').modal('hide');
        $('.modal-backdrop').remove();
        $('body').removeClass('modal-open');
       ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_CloseModal_" + modalId, script, true);
        }

        protected void gvProdutosFabricacao_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Gerenciar")
            {
                string idProjeto = e.CommandArgument.ToString();
                AbrirModal(idProjeto, "P");
            }
        }

        #endregion
        
    }
    public class ProdutosFabricacao
    {
        public int idFabricacao { get; set; }
        public int idAtividade { get; set; }
        public string sDscFabricacao { get; set; }
        public string sDscProduto{ get; set; }
        public int idRequisicao { get; set; }
        public int idProduto { get; set; }
        public DateTime dtAtualizacao { get; set; }
        public int idProjeto { get; set; }
    }
}
