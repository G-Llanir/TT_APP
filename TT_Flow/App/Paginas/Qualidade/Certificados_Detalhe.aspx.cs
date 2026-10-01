using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using System.Web.UI.WebControls;
using TT.FrameWork;
using static Permissao;
using TT_Flow.FrameWork;
using System.Data.SqlClient;
using GRID = TT.FrameWork.Grid;
using TT_Hub.App.Paginas.Requisicao;
using System.Globalization;
using System.Linq;
using TT_Flow.App.Paginas.Manutencao.Feriados;

namespace TT_Flow.App.Paginas.Manutencao.Certificados
{
    public partial class Certificados_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Certificado";
        string sProcedure = "sp_Manipula_tbl_Flow_Certificados";

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();
            manual.sNomeArquivo = "Manual-Certificados.pdf";
            
            if (FUNCOES.ValidaPermissao(Permissao.Certificados.AlterarCertificado) == false)
            {
                ddlidTipo.Attributes.Add("disabled", "disabled");
                txtsNome.ReadOnly = true;
                ddlidCliente.Attributes.Add("disabled", "disabled");
                txtsAssunto.ReadOnly = true;
                txtnCargaHoraria.ReadOnly = true;
                txtdtCertificado.ReadOnly = true;
                txtdtValidade.ReadOnly = true;
                txtnNumero.ReadOnly = true;
                txtsEmail.ReadOnly = true;
                txtsTelefone.ReadOnly = true;
                cmdSalvar.Visible = false;
            }

            if (!IsPostBack)
            {
                if (Request["id"] != null)
                {
                    Pesquisar(Request["id"].ToString(), false);
                }
                else
                {
                    Pesquisar("0", true);
                    aba_Arquivos.Visible = false;
                }
            }
            else
            {
                var requestTarget = this.Request["__EVENTTARGET"];
                var requestArgs = this.Request["__EVENTARGUMENT"];
                aba_Arquivos.Visible = false;

                if (requestTarget == "funcao_SAIR")
                {
                    FUNCOES.DirecionaPagina("/app/dashboard.aspx");
                }
                else if (requestTarget == "funcao_SALVAR")
                {
                    Salvar_Certificados();
                }
                else if (requestTarget == "funcao_Editar")
                {
                }
            }

            RegistraScript("");

        }
        #endregion

        #region | Metodos Banco de Dados
        protected void Pesquisar(string idRegistro, bool bEdicao)
        {
            PopularCombos();

            string sErro = "";

            try
            {
                LimpaCampos();

                if (idRegistro != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idRegistro", idRegistro);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidCertificados.Value = RETORNO.DATASET(dsPesquisa, 0, "idRegistro");
                        txtidRegistro.Text = hddidCertificados.Value;
                        txtsNome.Text = RETORNO.DATASET(dsPesquisa, 0, "sNome");
                        ddlidCliente.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idCliente");
                        ddlidTipo.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idTipo");
                        txtnCargaHoraria.Text = RETORNO.DATASET(dsPesquisa, 0, "nCargaHoraria");
                        txtsAssunto.Text = RETORNO.DATASET(dsPesquisa, 0, "sAssunto");
                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        txtsEmail.Text = RETORNO.DATASET(dsPesquisa, 0, "sEmail");
                        txtsTelefone.Text = RETORNO.DATASET(dsPesquisa, 0, "sTelefone");

                        var dt = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtCertificado").ToString());
                        txtdtCertificado.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');

                        var dtValidade = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtValidade").ToString());
                        txtdtValidade.Text = dtValidade.ToString(@"yyyy/MM/dd").Replace('/', '-');

                        txtnNumero.Text = RETORNO.DATASET(dsPesquisa, 0, "nNumero");

                        aba_Arquivos.Visible = true;
                        Popular_Aba_Arquivos(idRegistro);
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }

                }
                else
                {
                    lblTituloSalvar.Text = "Confirma a Inclusão do Certificado?";
                    lblTituloPagina.Text = "Novo Certificado";
                    aba_Arquivos.Visible = false;
                    txtidRegistro.Text = "Novo";
                }
                RegistraScript("");

            }
            catch (Exception ex)
            {

                if (ex.Message == "Nenhum Registro Encontrado")
                    Response.Redirect("Certificados_Detalhe.aspx");
            }

        }

        void Salvar_Certificados()
        {
            string sErro = "";
            if (ValidarDados())
            {
                string[] vidCertificado = hddidCertificados.Value.Split(',');
                string idCertificado = vidCertificado[0].ToString();
                try
                {
                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    //Cadastro


                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idRegistro", idCertificado);
                    vParametros.Add("@sNome", txtsNome.Text);
                    vParametros.Add("@idCliente", ddlidCliente.SelectedValue);
                    vParametros.Add("@idTipo", ddlidTipo.SelectedValue);
                    vParametros.Add("@sAssunto", txtsAssunto.Text);
                    vParametros.Add("@nCargaHoraria", txtnCargaHoraria.Text);
                    vParametros.Add("@dtCertificado", DateTime.Parse(txtdtCertificado.Text).ToString("dd/MM/yyyy"));
                    vParametros.Add("@dtValidade", DateTime.Parse(txtdtValidade.Text).ToString("dd/MM/yyyy"));
                    vParametros.Add("@nNumero", txtnNumero.Text);
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    vParametros.Add("@sEmail", txtsEmail.Text);
                    vParametros.Add("@sTelefone", txtsTelefone.Text);

                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idCertificado = RETORNO.DATASET(dsSalvar, "idRegistro");
                        Pesquisar(idCertificado, false);
                        MensagemPagina.MostraMensagem_Sucesso(string.Format("{1} gravado com sucesso!  </br><a href='Certificados_Detalhe.aspx'>Clique aqui para incluir um novo {1}.</a>", Request.RawUrl.ToString(), sTituloPagina));
                    }
                    else
                    {
                        throw new Exception("BD: " + sErro.ToString());
                    }

                    aba_Arquivos.Visible = true;

                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                    aba_Arquivos.Visible = false;
                }

            }
            RegistraScript("");
        }
        #endregion

        #region | Popular Aba Arquivos
        void Popular_Aba_Arquivos(string idRegistro)
        {
            frmArquivos.Attributes.Add("src", string.Format("../Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}", idRegistro, "Certificados"));
        }
        #endregion

        #region | Limpar Campos
        void LimpaCampos()
        {
            hddidCertificados.Value = "0";
            txtidRegistro.Text = "Novo";
            ddlidTipo.SelectedValue = "0";
            ddlidCliente.SelectedValue = "0";
            txtsNome.Text = "";
            txtsAssunto.Text = "";
            txtnCargaHoraria.Text = "";
            txtnNumero.Text = "";
            lblTituloPagina.Text = sTituloPagina;
            PainelAtualizacao.Visible = false;
        }
        #endregion

        #region | Validação 
        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (ddlidTipo.SelectedValue == "0")
            {
                sMensagemErro = "Selecione o Tipo!";
            }

            if (txtsNome.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira um Nome Válido!";
            }

            if (ddlidCliente.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira um Parceiro Válido!";
            }

            if (txtsAssunto.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira um Assunto Válido!";
            }

            if (txtnCargaHoraria.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma Carga Horaria Válido!";
            }

            if (txtdtCertificado.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma Data Válida!";
            }

            if (!string.IsNullOrEmpty(txtdtValidade.Text) && !string.IsNullOrEmpty(txtdtCertificado.Text))
            {
                if (DateTime.Parse(txtdtValidade.Text) < DateTime.Parse(txtdtCertificado.Text))
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "A data de validade é menor que a data do certificado!";
                }
            }

            if (txtdtValidade.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma Data de Validade!";
            }

            if (txtnNumero.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira um Número de Certificado Válido!";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina_certificado.MostraMensagem_Erro(sMensagemErro);
                if(txtidRegistro.Text == "")
                {
                    txtidRegistro.Text = "Novo";
                }
            }

            return bRetorno;
        }
        #endregion

        #region | Combos/DDL
        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlidCliente, "sp_Select 'Flow_Clientes'", "idCliente", "sRazaoSocial", false, "Selecione o Parceiro", "0");
        }
        #endregion

        #region | Script 
        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

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
            sb.Append("$('[id*=txtsTelefone]').mask('(00)00000-0000');");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);

        }
        #endregion

        #region | EVENTOS
        protected void cmdAvancar_click(object sender, EventArgs e)
        {
            int id = 0;

            if (txtidRegistro.Text != "Novo")
                id = Convert.ToInt32(txtidRegistro.Text) + 1;

            Response.Redirect($"Certificados_Detalhe.aspx?id={id}");
        }

        protected void cmdRetornar_click(object sender, EventArgs e)
        {
            int id = 0;

            if (txtidRegistro.Text != "Novo")
                id = Convert.ToInt32(txtidRegistro.Text) - 1;

            Response.Redirect($"Certificados_Detalhe.aspx?id={id}");
        }
        #endregion

    }
}