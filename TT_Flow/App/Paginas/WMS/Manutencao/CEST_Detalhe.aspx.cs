using System;
using System.Collections.Generic;
using System.Web.UI;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using System.Data;

namespace TT_Flow.App.Paginas.WMS.Manutencao
{
    public partial class CEST_Detalhe : Page
    {
        string sTituloPagina = "CEST";
        string sProcedure = "sp_Manipula_tbl_Flow_WMS_CEST";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.CEST.Consultar, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.CEST.Incluir, true);
                    Pesquisar("0");
                }
            }
        }

        private bool ValidarDados()
        {
            if (txtsDscCEST.Text == "")
            {
                MensagemPagina.MostraMensagem_Erro("Informe um Descrição válida para cadastro");
                return false;
            }
            return true;
        }

        protected void Pesquisar(string idPesquisa)
        {
            string sErro = "";
            try
            {
                LimpaCampos();
                if (idPesquisa != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idCEST", idPesquisa }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidCEST.Value = RETORNO.DATASET(dsPesquisa, 0, "idCEST");
                        txtidCEST.Text = RETORNO.DATASET(dsPesquisa, 0, "idCEST");
                        txtsCodigoCEST.Text = RETORNO.DATASET(dsPesquisa, 0, "sCodigoCEST");
                        txtsDscCEST.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscCEST");
                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"));
                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsCodigoCEST.Text);
                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.CEST.Alterar);
                    }
                    else
                        throw new Exception(sErro);
                }
                else
                {
                    BreadCrumb_Pagina.TitulodaPagina = "Novo";
                    lblTituloPagina.Text = string.Format("Nova {0}", sTituloPagina);
                    cmdSalvar.Text = "Incluir";
                }

                txtsCodigoCEST.Focus();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

            RegistraScript();

        }

        void RegistraScript()
        {
            //MyScriptInclude
            string scriptUrl = ResolveUrl("~/App/JS/Mascaras.js");
            ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "InclusaoScriptCEST", scriptUrl);
        }

        void LimpaCampos()
        {
            txtidCEST.Text = "Novo";
            txtsDscCEST.Text = "";
            txtsCodigoCEST.Text = "";
            hddidCEST.Value = "0";
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {
                    string[] vidCEST = hddidCEST.Value.Split(',');
                    string idCEST = vidCEST[0].ToString();

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idCEST", idCEST },
                        { "@sCodigoCEST", txtsCodigoCEST.Text },
                        { "@sDscCEST", txtsDscCEST.Text },
                        { "@sSituacao", ComboAtivo.Situacao_Recuperar() },
                        { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                    };
                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idCEST = RETORNO.DATASET(dsSalvar, 0, "idCEST");
                        Pesquisar(idCEST);
                        MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                    }
                    else
                        throw new Exception("BD: " + sErro.ToString());
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }
                
            }
            RegistraScript();
        }
    }
}