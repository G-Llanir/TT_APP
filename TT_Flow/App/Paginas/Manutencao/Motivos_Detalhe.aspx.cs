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


namespace TT_Flow.App.Paginas.Manutencao
{
    public partial class Motivos_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Motivo Detalhe";
        string sProcedure = "sp_Manipula_tbl_Flow_Motivos";
       

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Motivos.Consultar, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Motivos.Incluir, true);
                    Pesquisar("0");
                }
            }
        }

        protected void Pesquisar(string idPesquisa)
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
                    vParametros.Add("@idMotivo", idPesquisa);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidMotivo.Value               = RETORNO.DATASET(dsPesquisa, 0, "idMotivo");
                        txtidMotivo.Text                = RETORNO.DATASET(dsPesquisa, 0, "idMotivo");
                        ddlsTipoMotivo.SelectedValue    = RETORNO.DATASET(dsPesquisa, 0, "sTipoMotivo");
                        txtsDscMotivo.Text              = RETORNO.DATASET(dsPesquisa, 0, "sDscMotivo");


                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"));
                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsDscMotivo.Text);
                      
                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Motivos.Alterar);
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
                }

                //txtsCodigo.Focus();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }
        


        void LimpaCampos()
        {

            txtidMotivo.Text = "Novo";
            ddlsTipoMotivo.SelectedValue = "0";
            txtsDscMotivo.Text = "";

            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
        }

       
        private bool ValidarDados()
        {


            if (ddlsTipoMotivo.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione uma opção valida");
                return false;
            }
            if (txtsDscMotivo.Text.Length < 12)
            {
                MensagemPagina.MostraMensagem_Erro("Descrição invalida");
                return false;
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
                    string[] vidMotivo = hddidMotivo.Value.Split(','); 
                    string idMotivo = vidMotivo[0].ToString();

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR"); 
                    vParametros.Add("@idMotivo", idMotivo);
                    vParametros.Add("@sTipoMotivo", ddlsTipoMotivo.SelectedValue);
                    vParametros.Add("@sDscMotivo", txtsDscMotivo.Text);
                    vParametros.Add("@sSituacao", ComboAtivo.Situacao_Recuperar()); 
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idMotivo = RETORNO.DATASET(dsSalvar, 0, "idMotivo");
                        Pesquisar(idMotivo);
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
        }
    }
}