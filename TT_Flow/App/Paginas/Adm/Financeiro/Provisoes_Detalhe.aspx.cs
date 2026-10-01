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
using TT_Flow.FrameWork;
using TT.FrameWork;
using static TT.FrameWork.BD;
using System.Text;
using System.IO;
using System.Globalization;
//using MathNet.Numerics.Providers.SparseSolver;


namespace TT_Flow.App.Paginas.Adm.Financeiro
{
    public partial class Provisoes_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Provisões";
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_Provisoes";

        #region | Classes


        #endregion

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();


            if (!IsPostBack)
            {

                if (Request["id"] != null)
                {
                   FUNCOES.ValidaPermissao(Permissao.Financeiro.Provisao.Consultar, true);
                    Pesquisar(Request["id"].ToString(), false);
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Financeiro.Provisao.Incluir, true);
                    Pesquisar("0", true);

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
                    Salvar_Provisao();
                }
                else if (requestTarget == "funcao_Editar")
                {
                    Pesquisar(hddidProvisao.Value, true);
                }
            }

            RegistraScript("");

        }


        #endregion

        #region |Metodos Banco de Dados
        protected void Pesquisar(string idProvisao, bool bEdicao)
        {
            PopularCombos();
            aba_Historico.Visible = false;
       
            string sErro = "";

            try
            {
                LimpaCampos();

                if (idProvisao != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idProvisao", idProvisao);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidProvisao.Value = RETORNO.DATASET(dsPesquisa, 0, "idProvisao");
                        txtidProvisao.Text = RETORNO.DATASET(dsPesquisa, 0, "idProvisao");
                        txtdtInicioProvisao.Text = RETORNO.DATASET(dsPesquisa, 0, "dtInicioProvisao");
                        txtdtFinalProvisao.Text = RETORNO.DATASET(dsPesquisa, 0, "dtFinalProvisao");
                        txtsDscProvisao.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscProvisao");
                        txtnValorProvisao.Text = RETORNO.DATASET(dsPesquisa, 0, "nValorProvisao");
                        txtnParcelasProvisao.Text = RETORNO.DATASET(dsPesquisa, 0, "nParcelasProvisao");
                        txtnRepetocoes.Text = RETORNO.DATASET(dsPesquisa, 0, "nRepetocoes");

                        //ddlidCategoriaProvisao.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idCategoriaProvisao");
                        ddlidCategoriaPagar.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idCategoriaPagar");
                        ddlidCategoriaReceber.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idCategoriaReceber");

                        ddlidEmpresa.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idEmpresa");
                        ddlidTipoProvisao.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idTipoProvisao");
                        ddlidBanco.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idBanco");
                        ddlsAtivo.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sAtivo");
                        ddlidPeriodicidade.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idPeriodicidade");

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));

                        lblTituloPagina.Text = string.Format("Provisão {0}", RETORNO.DATASET(dsPesquisa, 0, "sDscProvisao"));
                        BreadCrumb.TitulodaPagina = string.Format("Provisão {0}", RETORNO.DATASET(dsPesquisa, 0, "sDscProvisao"));

                        lblTituloSalvar.Text = "Confirma a Alteração da " + lblTituloPagina.Text + "?";

                        Popular_Aba_Historico(dsPesquisa);

                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Financeiro.Provisao.Incluir);
                    }
                    else
                    {
                        


                        throw new Exception(sErro);
                    }

                }
                else
                {
                    BreadCrumb.TitulodaPagina = string.Format("Nova {0}", sTituloPagina);
                    lblTituloPagina.Text = string.Format("Nova {0}", sTituloPagina);
                    txtidProvisao.Text = "Novo";
                    lblTituloSalvar.Text = "Confirma a Inclusão da Provisão?";
                    cmdSalvar.Text = "Incluir";
                    txtsDscProvisao.Focus();
                    div_Parcelas.Visible = false;
                    div_Periodicidade.Visible = false;
                    div_Repeticao.Visible = false;
                }
                RegistraScript("");

            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }


        void Salvar_Provisao()
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {
                    string[] vidProvisao = hddidProvisao.Value.Split(',');
                    string idProvisao = vidProvisao[0].ToString();

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    //Cadastro
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idProvisao", idProvisao);

                    vParametros.Add("@dtInicioProvisao", txtdtInicioProvisao.Text);
                    vParametros.Add("@dtFinalProvisao", txtdtFinalProvisao.Text);
                    vParametros.Add("@sDscProvisao", txtsDscProvisao.Text);
                    vParametros.Add("@nValorProvisao", BD.Conversoes.Numerico(txtnValorProvisao));
                    vParametros.Add("@nParcelasProvisao", txtnParcelasProvisao.Text);
                    vParametros.Add("@nRepetocoes", txtnRepetocoes.Text);

                    //vParametros.Add("@idCategoriaProvisao", ddlidCategoriaProvisao.SelectedValue);
                    vParametros.Add("@idCategoriaPagar", ddlidCategoriaPagar.SelectedValue);
                    vParametros.Add("@idCategoriaReceber", ddlidCategoriaReceber.SelectedValue);

                    vParametros.Add("@idEmpresa", ddlidEmpresa.SelectedValue);
                    vParametros.Add("@idTipoProvisao", ddlidTipoProvisao.SelectedValue);
                    vParametros.Add("@idBanco", ddlidBanco.SelectedValue);
                    vParametros.Add("@sAtivo", ddlsAtivo.SelectedValue);
                    vParametros.Add("@idPeriodicidade", ddlidPeriodicidade.SelectedValue);

                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());

                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);
                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idProvisao = RETORNO.DATASET(dsSalvar, "idProvisao");
                        
                            Pesquisar(idProvisao, false);
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
            RegistraScript("");
        }

        #endregion



        #region | Limpar Campos
        void LimpaCampos()
        {
            hddidProvisao.Value = "0";
            txtidProvisao.Text = "Novo";

            txtidProvisao.Text = "";
            txtdtInicioProvisao.Text =  DateTime.Today.ToString("u").Substring(0, 10);
            txtdtFinalProvisao.Text = "";
            txtsDscProvisao.Text = "";
            txtnValorProvisao.Text = "";
            txtnParcelasProvisao.Text = "";
            txtnRepetocoes.Text = "";

            //ddlidCategoriaProvisao.SelectedValue = "0";
            ddlidCategoriaReceber.SelectedValue = "0";
            ddlidCategoriaPagar.SelectedValue = "0";

            ddlidEmpresa.SelectedValue = "0";
            ddlidTipoProvisao.SelectedValue = "0";
            ddlidBanco.SelectedValue = "0";
            ddlsAtivo.SelectedValue = "0";
            ddlidPeriodicidade.SelectedValue = "0";


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

            if (txtsDscProvisao.Text.Length < 5)
            {
                sMensagemErro = "Descrição inválida, mínimo de 5 caracteres!";
            }


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
            sb.Append("$v192('[id*=cmdSalvar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Salvar').dialog('open');");
            sb.Append("});");


            sb.Append("$('[id*=txtnValorProvisao]').mask('000.000.000.000.000,00', { reverse: true });");
            sb.Append("$('[id*=txtnParcelasProvisao]').mask('00#');");
            sb.Append("$('[id*=txtnRepetocoes]').mask('00#');");

            sb.Append("});");

            if (sFuncao != "")
            {
                sb.Append(sFuncao);
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina" + Guid.NewGuid(), sb.ToString(), true);
        }
        #endregion

        #region | Combos/DDL
        void PopularCombos()
        {
            //FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_empresa'", "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");
            FUNCOES.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa', @idUsuario=" + IDENTITY.Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");

            FUNCOES.Popula_Combo(ddlidBanco, "sp_Select 'Flow_Bancos'", "idBanco", "sDscBanco", false, "Selecione o Banco", "0");

            FUNCOES.Popula_Combo(ddlidCategoriaPagar, "sp_Select 'Flow_Adm_Contas_Pagar_Categoria'", "idCategoriaPagar", "sDscCategoriaPagar", false, "Selecione a Categoria", "0");
            FUNCOES.Popula_Combo(ddlidCategoriaReceber, "sp_Select 'Flow_Adm_Contas_Receber_Categoria'", "idCategoriaReceber", "sDscCategoriaReceber", false, "Selecione a Categoria", "0");
        }
        #endregion


        void Popular_Aba_Historico(DataSet ds)
        {
            gv_Historico.DataSource = ds.Tables[1];
            gv_Historico.DataBind();
            aba_Historico.Visible = true;
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

        protected void ddlidTipoProvisao_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (Convert.ToInt32(ddlidTipoProvisao.SelectedValue))
            {
                case 1:
                    div_Parcelas.Visible = false;
                    div_Periodicidade.Visible = false;
                    div_Repeticao.Visible = false;
                    break;

                case 2:
                    div_Parcelas.Visible = true;
                    div_Periodicidade.Visible = false;
                    div_Repeticao.Visible = false;
                    break;
                case 3:

                    div_Parcelas.Visible = false;
                    div_Periodicidade.Visible = true;
                    div_Repeticao.Visible = false;
                    break;
                
            }
        }

        protected void ddlidPeriodicidade_SelectedIndexChanged(object sender, EventArgs e)
        {
            string periodicidade = ddlidPeriodicidade.SelectedValue;
            if (periodicidade != "0")
            {
                div_Repeticao.Visible = true;
            }
            
        }
    }   
}