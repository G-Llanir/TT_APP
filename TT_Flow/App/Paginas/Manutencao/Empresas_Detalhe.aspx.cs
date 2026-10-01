using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using TT.FrameWork;
using TT_Flow.FrameWork;
using static TT.FrameWork.Arquivo;
using static TT.FrameWork.BD.Retorno;
using static TT.FrameWork.Identity;
using static TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas.Manutencao.Empresas
{
    public partial class Empresas_Detalhe : Page
    {
        #region | Propriedades

        string sTituloPagina = "Empresa";
        string sProcedure = "sp_Manipula_tbl_Flow_Empresas";

        public List<Cls_Empresas> Base_Impostos
        {
            get { if (ViewState["Base_Impostos"] == null) ViewState["Base_Impostos"] = new List<Cls_Empresas>(); return (List<Cls_Empresas>)ViewState["Base_Impostos"]; }
            set => ViewState["Base_Impostos"] = value;
        }

        #endregion

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            cmdFornecedores_Modal.Visible = ValidaPermissao(Permissao.Comercial.Orcamento.Alterar_ImagemFornecedores, false);

            manual.sNomeArquivo = "Manual-Empresas.pdf";
            gv_Imposto.Columns[0].Visible = false;

            div_TaxReturn.Visible = false;
            lblCNPJ_VAT.Text = "";

            if (ddlidTributacao.SelectedValue == "4")
                div_TaxReturn.Visible = true;

            if (ddlidTipo.SelectedValue == "1")
            {
                div_UF_Mun_Nacional.Visible = true;
                div_txtEstado.Visible = false;
            }

            if (ddlidTipo.SelectedValue == "2")
            {
                div_UF_Mun_Nacional.Visible = false;
                div_txtEstado.Visible = true;
            }

            if (ValidaPermissao(Permissao.Empresas.AlterarEmpresas) == false)
            {
                txtsDscEmpresa.ReadOnly = true;
                txtsDscEmpresaReduzida.ReadOnly = true;
                ddlidTipo.Attributes.Add("disabled", "disabled");
                ddlEstado.Attributes.Add("disabled", "disabled");
                txtsEstado.ReadOnly = true;
                ddlidCliente.Attributes.Add("disabled", "disabled");
                ddlidTributacao.Attributes.Add("disabled", "disabled");
                ddlsImpostos.Attributes.Add("disabled", "disabled");
                cmdSalvar.Visible = false;
                gv_Imposto.Columns[7].Visible = false;
                Div_Imposto.Visible = false;
                ddlsEfetuaCompras.Attributes.Add("disabled", "disabled");
            }

            if (ddlsImpostos.SelectedValue == "0")
            {
                Div_COFINS.Visible = false;
                Div_PIS.Visible = false;
                Div_ICMS.Visible = false;
                Div_CSSL.Visible = false;
                Div_IRPJ.Visible = false;
                Div_Ano.Visible = false;
            }

            if (ddlsImpostos.SelectedValue == "1")
            {
                Div_COFINS.Visible = true;
                Div_PIS.Visible = true;
                Div_ICMS.Visible = false;
                Div_CSSL.Visible = false;
                Div_IRPJ.Visible = false;
                Div_Ano.Visible = true;
            }

            if (ddlsImpostos.SelectedValue == "2")
            {
                Div_ICMS.Visible = true;
                Div_COFINS.Visible = true;
                Div_PIS.Visible = true;
                Div_CSSL.Visible = false;
                Div_IRPJ.Visible = false;
                Div_Ano.Visible = true;
            }

            if (ddlsImpostos.SelectedValue == "3")
            {
                Div_ICMS.Visible = false;
                Div_COFINS.Visible = true;
                Div_PIS.Visible = true;
                Div_CSSL.Visible = true;
                Div_IRPJ.Visible = true;
                Div_Ano.Visible = true;
            }

            if (!IsPostBack)
            {
                if (Session["SalvoComSucesso"] != null && (bool)Session["SalvoComSucesso"])
                {
                    MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                    Session["SalvoComSucesso"] = false;
                }

                if (Request["id"] != null)
                    Pesquisar(Request["id"].ToString(), false);
                else
                {
                    Pesquisar("0", true);
                    aba_Arquivos.Visible = false;
                }
            }
            else
            {
                var requestTarget = Request["__EVENTTARGET"];
                if (requestTarget == "funcao_SAIR")
                    DirecionaPagina("/app/dashboard.aspx");
                else if (requestTarget == "funcao_SALVAR")
                    Salvar_Empresas();
            }

            ddlsEfetuaCompras.Text = "Efetua Compras ?";
            RegistraScript("");
        }

        protected void Pesquisar(string idEmpresa, bool bEdicao)
        {
            PopularCombos();

            try
            {
                LimpaCampos();

                if (idEmpresa != "0")
                {
                    Div_Impostos.Visible = true;
                    Div_Orcamento.Visible = true;
                    div_ddlMunicipio.Visible = false;

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idEmpresa", idEmpresa }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        hddidEmpresa.Value = DATASET(dsPesquisa, "idEmpresa");
                        txtidRegistro.Text = hddidEmpresa.Value;
                        txtsDscEmpresa.Text = DATASET(dsPesquisa, "sDscEmpresa");
                        ddlidCliente.SelectedValue = DATASET(dsPesquisa, "idParceiro");
                        ddlidTipo.SelectedValue = DATASET(dsPesquisa, "idTipoPais");
                        txtsDscEmpresaReduzida.Text = DATASET(dsPesquisa, "sDscEmpresaReduzida");
                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(DATASET(dsPesquisa, "dtAtualizacao"), DATASET(dsPesquisa, "sDscUsuarioAtualizacao"));
                        txtCNPJ.Text = DATASET(dsPesquisa, "CNPJ");
                        txtsLoteRPS.Text = DATASET(dsPesquisa, "sLoteRPS");
                        txtsSerieRPS.Text = DATASET(dsPesquisa, "sSerieRPS");
                        txtsNumeroRPS.Text = DATASET(dsPesquisa, "sNumeroRPS");
                        ddlsEfetuaCompras.Situacao_Definir(DATASET(dsPesquisa, "sEfetuaCompras"));
                        ddlidCodigoTributario.SelectedValue = DATASET(dsPesquisa, "idCodigoTributario");
                        txtsSerieNFE.Text = DATASET(dsPesquisa, "sSerieNFE");
                        txtsNumeroNFE.Text = DATASET(dsPesquisa, "sNumeroNFE");
                        txtSenha_Certificado.Text = DATASET(dsPesquisa, "sSenha_Certificado");
                        ddlContaBancaria.SelectedValue = DATASET(dsPesquisa, "idContaBancaria");

                        AtualizaImagem_Fornecedores();

                        if (ddlidTipo.SelectedValue == "1")
                        {
                            ddlEstado.SelectedValue = DATASET(dsPesquisa, "sEstado");

                            Popula_Combo(ddlMunicipio, "sp_Select 'CIDADE', @sPesquisa=" + ddlEstado.SelectedValue, "idCidade", "sCidade", false, "Selecione a Cidade", "0");
                            ddlMunicipio.SelectedValue = DATASET(dsPesquisa, "idMunicipio");

                            div_UF_Mun_Nacional.Visible = true;
                            div_ddlMunicipio.Visible = true;
                            div_txtEstado.Visible = false;
                        }
                        else
                        {
                            txtsEstado.Text = DATASET(dsPesquisa, "sEstado");
                            div_UF_Mun_Nacional.Visible = false;
                            div_txtEstado.Visible = true;
                        }

                        ddlidTributacao.SelectedValue = DATASET(dsPesquisa, "idTributacao");

                        txtsCodigoTaxReturn.Text = DATASET(dsPesquisa, "sCodigoTaxReturn");
                        bool bErroTabelas = false;
                        foreach (string idTipoFaturamento in DATASET(dsPesquisa, "sidTipoFaturamento").Split(';'))
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(idTipoFaturamento))
                                    ddlidTipoFaturamento.Items.FindByValue(idTipoFaturamento).Selected = true;
                            }
                            catch { bErroTabelas = true; }
                        }

                        if (bErroTabelas)
                            MensagemPagina.MostraMensagem_Erro("Não foi possivel selecionar o tipo de faturamento!", false);

                        lblCNPJ_VAT_FEIN();

                        aba_Arquivos.Visible = true;
                        string sTipos = "";

                        if (ValidaPermissao(Permissao.Empresas.ArquivoDocumento)) sTipos += "Documentos;";
                        if (ValidaPermissao(Permissao.Empresas.ArquivoSTSO)) sTipos += "STSO;";
                        if (ValidaPermissao(Permissao.Empresas.ArquivoSocios)) sTipos += "Sócio 01;Sócio 02;Sócio 03;Sócio 04;Sócio 05;";
                        if (ValidaPermissao(Permissao.Empresas.ArquivoCertidõesEmpresa)) sTipos += "Certidões Empresa;";
                        if (ValidaPermissao(Permissao.Empresas.ArquivoCertidõesSocios)) sTipos += "Certidões Sócios;";
                        if (ValidaPermissao(Permissao.Empresas.ArquivoBalancos)) sTipos += "Balanços;";
                        if (ValidaPermissao(Permissao.Empresas.ArquivoSeguros)) sTipos += "Seguros;";

                        frmArquivos.Attributes.Add("src", string.Format("../Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}&SubTipo=true&Senha=true", idEmpresa, $"Empresas;{sTipos}Logotipo Nota Fiscal;Certificado Digital"));

                        div_TaxReturn.Visible = ddlidTributacao.SelectedValue == "4";
                    }
                    else throw new Exception(sErro);

                    Popular_gv_Imposto(dsPesquisa);
                    TipoOrcamento_Popular(DATASET(dsPesquisa, "sTipoOrcamento"));
                }
                else
                {
                    div_UF_Mun_Nacional.Visible = false;
                    div_txtEstado.Visible = false;
                    Div_Impostos.Visible = false;
                    Div_Orcamento.Visible = false;

                    lblTituloSalvar.Text = "Confirma a Inclusão da Empresa?";
                    lblTituloPagina.Text = "Nova Empresa";
                    txtidRegistro.Text = "Novo";
                }

                RegistraScript("");
            }
            catch (Exception ex)
            {
                if (ex.Message == "Nenhum Registro Encontrado")
                    Response.Redirect("Empresas_Detalhe.aspx");
            }
        }

        #endregion

        #region | Utils

        void PopularCombos()
        {
            ddlidCliente.Popula_Combo("sp_Select 'Flow_Clientes'", "idCliente", "sRazaoSocial", false, "Selecione o Parceiro", "0");
            Popula_Combo(ddlEstado, "sp_Select 'Flow_Estado'", "sEstado", "sEstado", false, "Selecione o Estado", "");
            Popula_Combo(ddlidTipoFaturamento, "sp_Select 'Flow_TipoFaturamento'", "idTipoFaturamento", "sDscTipoFaturamento", false, "Selecione o Tipo do Faturamento", "");
            Popula_Combo(ddlContaBancaria, "sp_Select 'tbl_Flow_Adm_ContasBancarias_x_AgenciaConta'", "idConta", "sDscConta", false, "Conta Bancária", "0");

            cblsTipoOrcamento.Items.Clear();
            SqlDataReader drTipoOrcamento = BD.ExecutarDataReader("sp_Select 'Flow_Orcamento_Tipo'");

            if (drTipoOrcamento != null)
            {
                while (drTipoOrcamento.Read())
                {
                    bool bAtivo = true;
                    if (nDR(drTipoOrcamento, "idRecurso").ToString() != "0")
                        bAtivo = ValidaPermissao(Convert.ToInt32(nDR(drTipoOrcamento, "idRecurso")));
                    cblsTipoOrcamento.Items.Add(new ListItem("  " + drTipoOrcamento["sDscTipoOrcamento"].ToString(), drTipoOrcamento["idTipoOrcamento"].ToString(), bAtivo));
                }
            }

            drTipoOrcamento.Close();
        }

        void LimpaCampos()
        {
            hddidEmpresa.Value = "0";
            txtidRegistro.Text = "Novo";
            ddlidTipo.SelectedValue = "0";
            ddlidCliente.SelectedValue = "0";
            txtsDscEmpresa.Text = "";
            txtsLoteRPS.Text = "";
            txtsSerieRPS.Text = "";
            txtsNumeroRPS.Text = "";
            txtsSerieNFE.Text = "";
            txtsNumeroNFE.Text = "";
            txtSenha_Certificado.Text = "";
            txtsDscEmpresaReduzida.Text = "";
            lblTituloPagina.Text = sTituloPagina;
            PainelAtualizacao.Visible = false;

            if (ddlidTipo.SelectedValue == "1") lblCNPJ_VAT.Text = "CNPJ";
            if (ddlidTipo.SelectedValue == "2") lblCNPJ_VAT.Text = "VAT / FEIN";

            txtsCodigoTaxReturn.Text = "";
            ddlidCodigoTributario.SelectedValue = "0";
        }

        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (ddlidTipo.SelectedValue == "0")
                sMensagemErro = "Selecione o País!";

            if (txtsDscEmpresa.Text == "")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira a Razão Social!";

            if (txtsDscEmpresaReduzida.Text == "")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira o Código da Empresa!";

            if (ddlidTipo.SelectedValue == "1")
            {
                if (ddlEstado.SelectedValue == "")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Estado!";
            }

            if (ddlidTipo.SelectedValue == "2")
            {
                if (txtsEstado.Text == "")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira o Estado!";
            }

            if (ddlidCliente.SelectedValue == "0")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira o Parceiro!";

            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina_empresa.MostraMensagem_Erro(sMensagemErro);
                if (txtidRegistro.Text == "")
                    txtidRegistro.Text = "Novo";
            }

            return bRetorno;
        }

        void Salvar_Empresas()
        {
            if (ValidarDados())
            {
                string[] vidEmpresa = hddidEmpresa.Value.Split(',');
                string idEmpresa = vidEmpresa[0].ToString();

                try
                {
                    string idTipoFaturamento = "";

                    foreach (ListItem item in ddlidTipoFaturamento.Items)
                    {
                        if (item.Selected)
                            idTipoFaturamento += string.Format("{0};", item.Value);
                    }

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idEmpresa", idEmpresa },
                        { "@sDscEmpresa", txtsDscEmpresa.Text },
                        { "@idParceiro", ddlidCliente.SelectedValue },
                        { "@idTipoPais", ddlidTipo.SelectedValue },
                        { "@sDscEmpresaReduzida", txtsDscEmpresaReduzida.Text },
                        { "@sEfetuaCompras", ddlsEfetuaCompras.Situacao_Recuperar() },
                        { "@idCodigoTributario", ddlidCodigoTributario.SelectedValue },
                        { "@idUsuarioAtualizacao", Variaveis.idUsuario() },
                        { "@idTributacao", ddlidTributacao.SelectedValue },
                        { "@sTipoOrcamento", TipoOrcamento_Concatenar() },
                        { "@sCodigoTaxReturn", txtsCodigoTaxReturn.Text },
                        { "@sidTipoFaturamento", idTipoFaturamento },
                        { "@idContaBancaria", ddlContaBancaria.SelectedValue },
                        { "@sSerieRPS", txtsSerieRPS.Text },
                        { "@sNumeroRPS", txtsNumeroRPS.Text },
                        { "@sSerieNFE", txtsSerieNFE.Text },
                        { "@sNumeroNFE", txtsNumeroNFE.Text },
                        { "@sSenha_Certificado", txtSenha_Certificado.Text }
                    };

                    if (ddlidTipo.SelectedValue == "1")
                    {
                        vParametros.Add("@sEstado", ddlEstado.SelectedValue.ToString());
                        vParametros.Add("@idMunicipio", ddlMunicipio.SelectedValue.ToString());
                    }
                    else vParametros.Add("@sEstado", txtsEstado.Text);

                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out string sErro))
                    {
                        if (Salvar_Imposto(idEmpresa))
                            Session["SalvoComSucesso"] = true;

                        idEmpresa = DATASET(dsSalvar, "idEmpresa");
                        Pesquisar(idEmpresa, false);
                        MensagemPagina.MostraMensagem_Sucesso(string.Format("{1} gravado com sucesso!  </br><a href='Empresas_Detalhe.aspx'>Clique aqui para incluir um novo {1}.</a>", Request.RawUrl.ToString(), sTituloPagina));
                    }
                    else throw new Exception("BD: " + sErro.ToString());
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }
            }

            RegistraScript("");
        }

        void AtualizaImagem_Fornecedores()
        {
            div_cmdVisualizar_ImagemFornecedores.Visible = true;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_IMAGEM_FORNECEDORES" },
                { "@idEmpresa", hddidEmpresa.Value }
            };
            DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(ds, out _))
            {
                try { imgFornecedores.ImageUrl = $"data:image/png;base64,{Convert.ToBase64String((byte[])ds.Tables[0].Rows[0]["imgFornecedores"])}"; }
                catch
                {
                    imgFornecedores.ImageUrl = "";
                    div_cmdVisualizar_ImagemFornecedores.Visible = false;
                }
            }
            else div_cmdVisualizar_ImagemFornecedores.Visible = false;
        }

        #endregion

        #region | Eventos

        protected void cmdAvancar_click(object sender, EventArgs e)
        {
            int id = 0;

            if (txtidRegistro.Text != "Novo")
                id = Convert.ToInt32(txtidRegistro.Text) + 1;

            Response.Redirect($"Empresas_Detalhe.aspx?id={id}");
        }

        protected void cmdRetornar_click(object sender, EventArgs e)
        {
            int id = 0;

            if (txtidRegistro.Text != "Novo")
                id = Convert.ToInt32(txtidRegistro.Text) - 1;

            Response.Redirect($"Empresas_Detalhe.aspx?id={id}");
        }

        protected void ddlidCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idParceiro", ddlidCliente.SelectedValue }
            };
            DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                txtCNPJ.Text = DATASET(dsPesquisa, "CNPJParceiro");
            else throw new Exception(sErro);

            if (DATASET(dsPesquisa, "sTipoCliente") == "U")
            {
                div_CNPJ.Visible = true;
                lblCNPJ_VAT.Text = "FEIN";
            }
            else if (DATASET(dsPesquisa, "sTipoCliente") == "E")
            {
                div_CNPJ.Visible = true;
                lblCNPJ_VAT.Text = "VAT";
            }
            else if (DATASET(dsPesquisa, "sTipoCliente") == "N")
            {
                div_CNPJ.Visible = true;
                lblCNPJ_VAT.Text = "CNPJ";
            }
            else if (DATASET(dsPesquisa, "sTipoCliente") == "")
            {
                div_CNPJ.Visible = true;
                lblCNPJ_VAT.Text = "CNPJ";
            }
        }

        protected void ddlidTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlidTipo.SelectedValue == "0")
            {
                div_UF_Mun_Nacional.Visible = false;
                div_txtEstado.Visible = false;
            }
            if (ddlidTipo.SelectedValue == "1")
            {
                div_UF_Mun_Nacional.Visible = true;
                div_txtEstado.Visible = false;

            }
            if (ddlidTipo.SelectedValue == "2")
            {
                div_UF_Mun_Nacional.Visible = false;
                div_txtEstado.Visible = true;
            }
        }

        protected void ddlidTributacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlidTributacao.SelectedValue == "4")
            {
                div_TaxReturn.Visible = true;
            }
            else
            {
                div_TaxReturn.Visible = false;
            }
        }

        void lblCNPJ_VAT_FEIN()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_DETALHE" },
                { "@idParceiro", ddlidCliente.SelectedValue }
            };
            DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

            if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                txtCNPJ.Text = DATASET(dsPesquisa, "CNPJParceiro");
            else throw new Exception(sErro);

            if (DATASET(dsPesquisa, "sTipoCliente") == "U")
            {
                div_CNPJ.Visible = true;
                lblCNPJ_VAT.Text = "FEIN";
            }
            else if (DATASET(dsPesquisa, "sTipoCliente") == "E")
            {
                div_CNPJ.Visible = true;
                lblCNPJ_VAT.Text = "VAT";
            }
            else if (DATASET(dsPesquisa, "sTipoCliente") == "N")
            {
                div_CNPJ.Visible = true;
                lblCNPJ_VAT.Text = "CNPJ";
            }
            else if (DATASET(dsPesquisa, "sTipoCliente") == "")
            {
                div_CNPJ.Visible = true;
                lblCNPJ_VAT.Text = "CNPJ";
            }
        }

        protected void ddlEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEstado.SelectedValue != "")
            {
                div_ddlMunicipio.Visible = true;

                Popula_Combo(ddlMunicipio, "sp_Select 'CIDADE', @sPesquisa=" + ddlEstado.SelectedValue, "idCidade", "sCidade", false, "Selecione a Cidade", "0");

                Scripts.FocusScript(Page, ddlMunicipio.ClientID);
            }
            else
            {
                div_ddlMunicipio.Visible = false;

                Scripts.FocusScript(Page, ddlEstado.ClientID);
            }
        }

        protected void cmdSalva_ImagemFornecedores_Click(object sender, EventArgs e)
        {
            try
            {
                string nome = fu_ImagemFornecedores.FileName;
                if (fu_ImagemFornecedores.HasFile && (nome.Contains(".png") || nome.Contains(".jpg") || nome.Contains(".jpeg") || nome.Contains(".webp") || nome.Contains(".bmp") || nome.Contains(".ico") || nome.Contains(".gif")))
                {
                    SalvarArquivo(nome, 10012, ConverterImagem_PNG(fu_ImagemFornecedores.FileBytes), Convert.ToInt32(hddidEmpresa.Value));
                    AtualizaImagem_Fornecedores();

                    MensagemPagina_View_ImagemFornecedores.MostraMensagem_Sucesso("Imagem atualizada com sucesso!");
                    Scripts.RemoverBackdrop_Modal(Page);
                    Scripts.AbrirModal(Page, "modalUpload_ImagemFornecedores, #modalView_ImagemFornecedores");
                }
                else
                {
                    MensagemPagina_ImagemFornecedores.MostraMensagem_Erro("É necessário selecionar uma Imagem com formato válido (.png, .jpg, .jpeg, .webp, .bmp, .ico, .gif)!");
                    Scripts.RemoverBackdrop_Modal(Page);
                    Scripts.AbrirModal(Page, "modalUpload_ImagemFornecedores");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina_ImagemFornecedores.MostraMensagem_Erro($"Houve um erro ao Atualizar a Imagem de Fornecedores para o PDF de Orçamentos!<br/>Erro: {ex.Message}");
            }
        }

        #endregion

        #region | gv_Imposto

        void Popular_gv_Imposto(DataSet dsPesquisa)
        {
            Base_Impostos.Clear();
            foreach (DataRow row in dsPesquisa.Tables[1].Rows)
            {
                FrameWork.Cls_Empresas objItem = new FrameWork.Cls_Empresas();

                objItem.idRegistro = Convert.ToInt32(row["idRegistro"].ToString());
                objItem.idLinha = Base_Impostos.Count() + 1;
                objItem.sFuncao = "SEM ALTERAÇÃO";

                objItem.sImpostos = (row["sDscTipo"].ToString());
                objItem.nPis = decimal.Parse((row["nPis"].ToString()));
                objItem.nCofins = decimal.Parse((row["nCofins"].ToString()));
                objItem.nICMS = decimal.Parse((row["nICMS"].ToString()));
                objItem.nCSSL = decimal.Parse((row["nCSSL"].ToString()));
                objItem.nIRPJ = decimal.Parse((row["nIRPJ"].ToString()));
                objItem.nAno = decimal.Parse((row["nAno"].ToString()));

                Base_Impostos.Add(objItem);
            }
            gv_Imposto_DataBind();
            LimpaCampos_Imposto();
        }

        protected void cmdImposto_Click(object sender, EventArgs e)
        {
            string sMensagem = "";
            if (ValidarDados_Imposto(ref sMensagem))
            {
                FrameWork.Cls_Empresas objItem = new FrameWork.Cls_Empresas();

                objItem.idLinha = Base_Impostos.Count() + 1;
                objItem.sFuncao = "Inserir_Imposto";

                objItem.sImpostos = ddlsImpostos.SelectedItem.ToString();
                objItem.idImpostos = Convert.ToInt32(ddlsImpostos.SelectedValue);
                objItem.nAno = Convert.ToDecimal(txtnAno.Text);

                if (txtnPis.Text != "")
                {
                    objItem.nPis = Convert.ToDecimal(txtnPis.Text);
                }
                else
                {
                    objItem.nPis = 0;
                }
                if (txtnCofins.Text != "")
                {
                    objItem.nCofins = Convert.ToDecimal(txtnCofins.Text);
                }
                else
                {
                    objItem.nCofins = 0;
                }
                if (txtnICMS.Text != "")
                {
                    objItem.nICMS = Convert.ToDecimal(txtnICMS.Text);
                }
                else
                {
                    objItem.nICMS = 0;
                }
                if (txtnCSSL.Text != "")
                {
                    objItem.nCSSL = Convert.ToDecimal(txtnCSSL.Text);
                }
                else
                {
                    objItem.nCSSL = 0;
                }
                if (txtnIRPJ.Text != "")
                {
                    objItem.nIRPJ = Convert.ToDecimal(txtnIRPJ.Text);
                }
                else
                {
                    objItem.nIRPJ = 0;
                }


                Base_Impostos.Add(objItem);
            }
            gv_Imposto_DataBind();
            LimpaCampos_Imposto();
        }

        bool Salvar_Imposto(string idEmpresa)
        {
            bool bRetorno = true;
            try
            {
                foreach (var Linha in Base_Impostos)
                {
                    if (Linha.sFuncao != "SEM ALTERAÇÃO")
                    {
                        Dictionary<String, String> vParametro = new Dictionary<string, string>();

                        vParametro["@idRegistro"] = Linha.idRegistro.ToString();
                        vParametro["@sFuncao"] = Linha.sFuncao;
                        vParametro["@idEmpresa"] = idEmpresa;

                        vParametro["@idTipo"] = Linha.idImpostos.ToString();
                        vParametro["@nPis"] = Linha.nPis.ToString().Replace(",", ".");
                        vParametro["@nCofins"] = Linha.nCofins.ToString().Replace(",", ".");
                        vParametro["@nICMS"] = Linha.nICMS.ToString().Replace(",", ".");
                        vParametro["@nCSSL"] = Linha.nCSSL.ToString().Replace(",", ".");
                        vParametro["@nIRPJ"] = Linha.nIRPJ.ToString().Replace(",", ".");
                        vParametro["@nAno"] = Linha.nAno.ToString();

                        BD.ExecutarDataSet(sProcedure, vParametro);
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
                bRetorno = false;
            }
            return bRetorno;
        }

        void gv_Imposto_DataBind()
        {
            gv_Imposto.DataSource = Base_Impostos.Where(c => c.sFuncao.ToString() != "Excluir_Imposto").OrderBy(x => x.idLinha);
            gv_Imposto.DataBind();
        }

        void LimpaCampos_Imposto()
        {
            ddlsImpostos.SelectedValue = "0";
            txtnPis.Text = "";
            txtnCofins.Text = "";
            txtnICMS.Text = "";
            txtnCSSL.Text = "";
            txtnIRPJ.Text = "";
            txtnAno.Text = "";
        }

        private bool ValidarDados_Imposto(ref string sMensagem)
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (ddlsImpostos.SelectedValue == "1")
            {
                if (txtnPis.Text == "")
                {
                    if (sMensagemErro != "")
                    {
                        sMensagemErro = sMensagemErro + "</br>";
                    }
                    sMensagemErro += "PIS Incorreto";
                }

                if (txtnCofins.Text == "")
                {
                    if (sMensagemErro != "")
                    {
                        sMensagemErro = sMensagemErro + "</br>";
                    }
                    sMensagemErro += "COFINS Incorreto";
                }
            }

            if (ddlsImpostos.SelectedValue == "2")
            {
                if (txtnPis.Text == "")
                {
                    if (sMensagemErro != "")
                    {
                        sMensagemErro = sMensagemErro + "</br>";
                    }
                    sMensagemErro += "PIS Incorreto";
                }

                if (txtnCofins.Text == "")
                {
                    if (sMensagemErro != "")
                    {
                        sMensagemErro = sMensagemErro + "</br>";
                    }
                    sMensagemErro += "COFINS Incorreto";
                }

                if (txtnICMS.Text == "")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "ICMS Incorreto" + "</br>";
                }
            }
            if (ddlsImpostos.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Tipo de Imposto";
            }

            if (txtnAno.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Ano!";
            }

            foreach (var Linha in Base_Impostos)
            {
                if (Linha.sFuncao != "Excluir_Imposto")
                {
                    if (ddlsImpostos.SelectedItem.ToString() == Linha.sImpostos && txtnAno.Text == Linha.nAno.ToString())
                    {
                        sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Já existe um imposto cadastrado para o ano selecionado!" + "</br>";
                    }
                }
            }

            if (ddlsImpostos.SelectedValue == "3")
            {
                if (txtnCSSL.Text == "")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "CSSL Incorreto" + "</br>";
                }

                if (txtnIRPJ.Text == "")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "IRPJ Incorreto" + "</br>";
                }
            }


            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina4.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        protected void gv_Imposto_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idLinha = Convert.ToInt32(gv_Imposto.DataKeys[e.RowIndex]["idLinha"].ToString());

            Base_Impostos[Base_Impostos.FindIndex(x => x.idLinha.Equals(idLinha))].sFuncao = "Excluir_Imposto";
            gv_Imposto_DataBind();
        }

        #endregion

        #region | Orçamento

        void TipoOrcamento_Popular(string sTipoOrcamento)
        {
            string[] vsTipoOrcamento = sTipoOrcamento.Split(';');

            for (int i = 0; i < vsTipoOrcamento.Count(); i++)
            {
                if (!string.IsNullOrEmpty(vsTipoOrcamento[i]))
                {
                    for (int contador = 0; contador <= cblsTipoOrcamento.Items.Count - 1; contador++)
                    {
                        if (cblsTipoOrcamento.Items[contador].Value == vsTipoOrcamento[i].ToString())
                            cblsTipoOrcamento.Items[contador].Selected = true;
                    }
                }
            }
        }

        string TipoOrcamento_Concatenar()
        {
            string sRetornoConcatenado = "";

            for (int contador = 0; contador <= cblsTipoOrcamento.Items.Count - 1; contador++)
            {
                if (cblsTipoOrcamento.Items[contador].Selected)
                    sRetornoConcatenado += string.Concat(cblsTipoOrcamento.Items[contador].Value, ";");
            }

            return sRetornoConcatenado;
        }

        #endregion

        #region | Script

        void RegistraScript(string sFuncao)
        {
            StringBuilder sb = new StringBuilder();

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

            sb.Append("});");

            sb.Append("$('[id*=txtnCargaHoraria]').mask('00000000000000', { reverse: true });");
            sb.Append("$('[id*=txtnPis], [id*=txtnCofins], [id*=txtnICMS], [id*=txtnCSSL], [id*=txtnIRPJ]').mask('9.999.999,99', { reverse: true });");
            sb.Append("$('[id*=txtnAno]').mask('9999', { reverse: true });");
            sb.Append("$('[id*=txtCNPJ]').mask('00.000.000/0000-00');");

            sb.AppendLine("$(document).ready(function () {");
            sb.AppendLine("");
            sb.AppendLine($"    $('#{cmdVisualizar_ImagemFornecedores.ClientID}').off('click').on('click', function (e) {{");
            sb.AppendLine("         e.preventDefault();");
            sb.AppendLine("         $('body').removeClass('modal-open');");
            sb.AppendLine("         $('.modal-backdrop').remove();");
            sb.AppendLine("         $('#modalView_ImagemFornecedores').modal('show');");
            sb.AppendLine("     });");
            sb.AppendLine("");
            sb.AppendLine("     var $uploadContainer = $('[id*=uploadContainer]');\r\n");
            sb.AppendLine($"    var $fileInput = $('#{fu_ImagemFornecedores.ClientID}');\r\n");
            sb.AppendLine("     var $fileNameDisplay = $('[id*=fileName]');\r\n\r\n");

            sb.AppendLine("     $uploadContainer.on('click', function () {\r\n");
            sb.AppendLine("          $fileInput.click();\r\n");
            sb.AppendLine("     });\r\n\r\n");
            sb.AppendLine("     $fileInput.on('click', function (e) {\r\n");
            sb.AppendLine("          e.stopPropagation();\r\n");
            sb.AppendLine("     });\r\n\r\n");
            sb.AppendLine("     $uploadContainer.on('dragover', function (e) {\r\n");
            sb.AppendLine("          e.preventDefault();\r\n");
            sb.AppendLine("          e.stopPropagation();\r\n");
            sb.AppendLine("          $uploadContainer.addClass('dragover');\r\n");
            sb.AppendLine("     });\r\n\r\n");
            sb.AppendLine("     $uploadContainer.on('dragleave', function (e) {\r\n");
            sb.AppendLine("          e.preventDefault();\r\n");
            sb.AppendLine("          e.stopPropagation();\r\n");
            sb.AppendLine("          $uploadContainer.removeClass('dragover');\r\n");
            sb.AppendLine("     });\r\n\r\n");
            sb.AppendLine("     $uploadContainer.on('drop', function (e) {\r\n");
            sb.AppendLine("          e.preventDefault();\r\n");
            sb.AppendLine("          e.stopPropagation();\r\n");
            sb.AppendLine("          $uploadContainer.removeClass('dragover');\r\n");
            sb.AppendLine("          var files = e.originalEvent.dataTransfer.files;\r\n");
            sb.AppendLine("          $fileInput[0].files = files;\r\n");
            sb.AppendLine("          displayFileName(files[0].name);\r\n");
            sb.AppendLine("     });\r\n\r\n");
            sb.AppendLine("     $fileInput.on('change', function () {\r\n");
            sb.AppendLine("          if (this.files.length > 0) {\r\n");
            sb.AppendLine("              displayFileName(this.files[0].name);\r\n");
            sb.AppendLine("          }\r\n");
            sb.AppendLine("     });\r\n\r\n");
            sb.AppendLine("     function displayFileName(name) {\r\n");
            sb.AppendLine("          $fileNameDisplay.text(name);\r\n");
            sb.AppendLine("     }");
            sb.AppendLine("");
            sb.AppendLine("});");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_RegistraScript", sb.ToString(), true);
        }

        #endregion
    }
}