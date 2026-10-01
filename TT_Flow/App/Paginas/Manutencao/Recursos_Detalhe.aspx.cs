using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using VALIDACOES = TT.FrameWork.Validacoes;


namespace TT_Flow.App.Paginas.Manutencao
{
    public partial class Recursos_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Recurso";
        string sProcedure = "sp_Manipula_tbl_Flow_Recursos";
   
        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Recursos.Consultar, true);
            

            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlidTipoRecurso, "sp_Select 'Flow_Recursos_Tipo'", "idTipoRecurso", "sDscTipoRecurso", false, "Selecione o Tipo do Recurso", "0");
                FUNCOES.Popula_Combo(ddlsUnidade, "sp_Select 'Flow_Produtos_Unidade'", "sUnidade", "sDscUnidade", false, "Selecione", "");

                if (Request["id"] != null)
                {
                    PesquisarRegistro(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Recursos.Incluir, true);
                    PesquisarRegistro("0");
                }
            }
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            SalvarRegistro();
        }

        #region |Funções Principais
        protected void PesquisarRegistro(string idPesquisa)
        {
            string sErro = "";
            try
            {
                LimpaCampos();

                if (idPesquisa != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE"); 
                    vParametros.Add("@idRecurso", idPesquisa);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidRecurso.Value                      = RETORNO.DATASET(dsPesquisa, 0, "idRecurso"); 
                        txtidRecurso.Text                       = RETORNO.DATASET(dsPesquisa, 0, "idRecurso");
                        txtsDscRecurso.Text                     = RETORNO.DATASET(dsPesquisa, 0, "sDScRecurso");
                        txtsObservacao.Text                     = RETORNO.DATASET(dsPesquisa, 0, "sObservacao");
                        ddlidTipoRecurso.SelectedValue          = RETORNO.DATASET(dsPesquisa, 0, "idTipoRecurso");
                        ddlsUnidade.SelectedValue               = RETORNO.DATASET(dsPesquisa, 0, "sUnidade");

                        PainelAtualizacao.Visible           = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));                  
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"));
                        lblTituloPagina.Text                = string.Format("Editar {0} {1}", sTituloPagina, txtsDscRecurso.Text);

                        AlterarEstadoControles(FUNCOES.ValidaPermissao(Permissao.Recursos.Alterar));
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }
                }
                else
                {
                    BreadCrumb.TitulodaPagina = "Incluir";
                    lblTituloPagina.Text = string.Format("Novo {0}", sTituloPagina);
                    cmdSalvar.Text = "Incluir";
                    AlterarEstadoControles(true);
                    ddlidTipoRecurso.Focus();
                }

                
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }

        protected void SalvarRegistro()
        {
            string sErro = "";
            if (ValidarDados())
            {

                try
                {
                    string[] vidRecurso = hddidRecurso.Value.Split(',');
                    string idRecurso = vidRecurso[0].ToString();

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idRecurso",           idRecurso);
                    vParametros.Add("@idTipoRecurso",       ddlidTipoRecurso.SelectedValue);
                    vParametros.Add("@sDscRecurso",         txtsDscRecurso.Text);
                    vParametros.Add("@sObservacao",         txtsObservacao.Text);
                    vParametros.Add("@sUnidade",            ddlsUnidade.SelectedValue);
                    vParametros.Add("@sSituacao",           ComboAtivo.Situacao_Recuperar());
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);
                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idRecurso = RETORNO.DATASET(dsSalvar, 0, "idRecurso");
                        PesquisarRegistro(idRecurso);
                        MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso!  </br><a href='Recursos_Detalhe.aspx?id=0'>Clique aqui para incluir um novo Recurso.</a>");
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

        #region | Funções complementares
        void AlterarEstadoControles(bool bPermiteEdicao)
        {
            string sEstado = "enabled";

            if (!bPermiteEdicao)
            {
                sEstado = "disabled";
            }
            
            ddlidTipoRecurso.Attributes.Remove("disabled"); ;
            ddlidTipoRecurso.Attributes.Add(sEstado, sEstado);
            ddlsUnidade.Attributes.Remove("disabled");
            ddlsUnidade.Attributes.Add(sEstado, sEstado);

            txtsDscRecurso.ReadOnly = !bPermiteEdicao;
            txtsObservacao.ReadOnly = !bPermiteEdicao;
            ComboAtivo.ReadOnly = !bPermiteEdicao;

            cmdSalvar.Visible = bPermiteEdicao;
        }

        void LimpaCampos()
        {
            txtidRecurso.Text = "Nova";
            txtsDscRecurso.Text = "";
            ddlidTipoRecurso.SelectedValue = "0";
            ddlsUnidade.SelectedValue = "";
            txtsObservacao.Text = "";
            hddidRecurso.Value = "0";
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
        }

       
        private bool ValidarDados()
        {
            if (txtsDscRecurso.Text.Length < 12)
            {
                MensagemPagina.MostraMensagem_Erro("Informe um nome válido para o Recurso (maior que 12 caracteres)!");
                txtsDscRecurso.Focus();
                return false;
            }
            if (ddlidTipoRecurso.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione um Tipo");
                ddlidTipoRecurso.Focus();
                return false;
            }
            if (ddlsUnidade.SelectedValue == "")
            {
                MensagemPagina.MostraMensagem_Erro("Selectione uma unidade");
                ddlidTipoRecurso.Focus();
                return false;
            }



            return true;
        }

        #endregion

  
    }
}