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

namespace TT_Flow.App.Paginas.Manutencao.Feriado
{
    public partial class Feriados_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Feriado";
        string sProcedure = "sp_Manipula_tbl_Flow_Feriados";

        #region | Funções Incialização do Form
        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();

            if (!IsPostBack)
            {
                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Manutencao.Feriados.Consultar, true);
                    Pesquisar(Request["id"].ToString(), false);
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Manutencao.Feriados.Incluir, true);
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
                    Salvar_Feriado();
                }
                else if (requestTarget == "funcao_Editar")
                {
                    Pesquisar(hddidFeriado.Value, true);
                }
            }

            RegistraScript("");

        }


        #endregion

        #region | Metodos Banco de Dados
        protected void Pesquisar(string idFeriado, bool bEdicao)
        {
            PopularCombos();

            string sErro = "";

            try
            {
                LimpaCampos();

                if (idFeriado != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idFeriado", idFeriado);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidFeriado.Value = RETORNO.DATASET(dsPesquisa, 0, "idFeriado");
                        txtidFeriado.Text = hddidFeriado.Value;
                        txtsDscFeriado.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscFeriado");

                        if (RETORNO.DATASET(dsPesquisa, 0, "dtFeriado").ToString() == "01/01/1900 00:00:00")
                        {
                            txtDtFeriado.Text = null;
                        }
                        else
                        {
                            var dt = Convert.ToDateTime(RETORNO.DATASET(dsPesquisa, 0, "dtFeriado").ToString());
                            txtDtFeriado.Text = dt.ToString(@"yyyy/MM/dd").Replace('/', '-');
                        }
                        ddlidTipo.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idTipo");
  
                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        lblTituloPagina.Text = string.Format("Feriado {0}", RETORNO.DATASET(dsPesquisa, 0, "sDscFeriado"));
                        BreadCrumb.TitulodaPagina = string.Format("Feriado {0}", RETORNO.DATASET(dsPesquisa, 0, "sDscFeriado"));
                        lblTituloSalvar.Text = "Confirma a Alteração do " + lblTituloPagina.Text + "?";
                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Manutencao.Feriados.Alterar);
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }

                }
                else
                {
                    BreadCrumb.TitulodaPagina = string.Format("Novo {0}", sTituloPagina);
                    lblTituloPagina.Text = string.Format("Novo {0}", sTituloPagina);
                    txtidFeriado.Text = "Novo";
                    cmAvancar.Visible = false;
                    cmRetornar.Visible = false;
                    lblTituloSalvar.Text = "Confirma a Inclusão do Feriado?";
                    txtidFeriado.Focus();
                }
                RegistraScript("");

            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);

                if (ex.Message == "Nenhum Registro Encontrado")
                    Response.Redirect("Feriados_Detalhe.aspx");
            }

        }

        void Salvar_Feriado()
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {
                    string[] vidFeriado = hddidFeriado.Value.Split(',');
                    string idFeriado = vidFeriado[0].ToString();

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();

                    //Cadastro
                    vParametros.Add("@sFuncao", "SALVAR");

                    vParametros.Add("@idFeriado", idFeriado);
                 
                    vParametros.Add("@sDscFeriado", txtsDscFeriado.Text);

                    if (txtDtFeriado.Text == "")
                    {
                        vParametros.Add("@dtFeriado", txtDtFeriado.Text);
                    }
                    else
                    {
                        DateTime dt = DateTime.Parse(txtDtFeriado.Text.ToString());
                        vParametros.Add("@dtFeriado", dt.ToString());
                    }

                    vParametros.Add("@idTipo", ddlidTipo.SelectedValue);

                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idFeriado = RETORNO.DATASET(dsSalvar, "idFeriado");

                        Pesquisar(idFeriado, false);
                        MensagemPagina.MostraMensagem_Sucesso(string.Format("{1} gravado com sucesso!  </br><a href='{0}?id=0'>Clique aqui para incluir um novo {1}.</a>", Request.RawUrl.ToString(), sTituloPagina));
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
            hddidFeriado.Value = "0";
            txtidFeriado.Text = "Novo";

            txtsDscFeriado.Text = "";

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

            if (string.IsNullOrEmpty(ddlidTipo.SelectedValue) || Convert.ToInt32(ddlidTipo.SelectedValue) == 0)
            {
                sMensagemErro = "Selecione um Tipo";
            }

            if (string.IsNullOrEmpty(txtsDscFeriado.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Descrição de Feriado é obrigatório!";
            }

            if (string.IsNullOrEmpty(txtDtFeriado.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "A Data é Obrigatório!";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;

                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
            }


            return bRetorno;
        }
        #endregion

        #region | Combos/DDL
        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlidTipo, "sp_Select 'Flow_TipoFeriado'", "idTipo", "sDscTipo", false, "Selecione o Grupo", "0");
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
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);

        }


        #endregion

        #region | EVENTOS

        protected void cmdAvancar_click(object sender, EventArgs e)
        {
            int id = 0;
            if (txtidFeriado.Text != "Novo")
                id = Convert.ToInt32(txtidFeriado.Text) + 1;

            Response.Redirect($"Feriados_Detalhe.aspx?id={id}");
        }

        protected void cmdRetornar_click(object sender, EventArgs e)
        {
            int id = 0;

            if (txtidFeriado.Text != "Novo")
                id = Convert.ToInt32(txtidFeriado.Text) - 1;

            Response.Redirect($"Feriados_Detalhe.aspx?id={id}");
        }
        #endregion

    }
}