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

namespace TT_Hub.App.Paginas.Cadastros
{
    public partial class Cliente_Detalhe : System.Web.UI.Page
    {



        string sTituloPagina = "Cliente";
        string sProcedure = "sp_HUB_Manipula_tbl_Cliente";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidarPermissaoAcesso();
            if (!IsPostBack)
            {
  
                if (Request["id"] != null)
                {
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    Pesquisar("0");
                }

            }

        }

        protected void Pesquisar(string idCliente)
        {
            string sErro = "";
            try
            {
                LimpaCampos();

                if (idCliente != "0")
                {
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR");
                    vParametros.Add("@idCliente", idCliente);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidCliente.Value = RETORNO.DATASET(dsPesquisa, 0, "idCliente");
                        txtssDscCliente.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscCliente");
                        txtsChaveGUI.Text = RETORNO.DATASET(dsPesquisa, 0, "sChaveGUI");
                        div_sChaveGUI.Visible = true;
                        
                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"));
                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtssDscCliente.Text);
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

                txtssDscCliente.Focus();
                ValidarAcessoCliente(idCliente);
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }
        void LimpaCampos()
        {
            txtssDscCliente.Text = "";
            //txtsw_Account.Text = "";
            txtsChaveGUI.Text = "";
            div_sChaveGUI.Visible = false;
            hddidCliente.Value = "0";
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
        }

        void ValidarAcessoCliente(string idCliente)
        {
            
            if (IDENTITY.Variaveis.sTipo() == "C")
            {
                txtssDscCliente.ReadOnly = true;
                ComboAtivo.ReadOnly = true;
                cmdSalvar.Visible = false;

                if (idCliente != IDENTITY.Variaveis.idCliente())
                {
                    this.Form.Visible = false;
                }

            }

        }

        private bool AplicarValidacoes()
        {

            return true;
        }
        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            string sErro = "";


            if (AplicarValidacoes())
            {

                try
                {
                    string[] vidCliente = hddidCliente.Value.Split(',');
                    string idCliente = vidCliente[0].ToString();

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idCliente",idCliente);
                    vParametros.Add("@sDscCliente", txtssDscCliente.Text);
                    //vParametros.Add("@sw_Account", txtsw_Account.Text);

                    vParametros.Add("@sSituacao", ComboAtivo.Situacao_Recuperar());
                    vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idCliente = RETORNO.DATASET(dsSalvar, 0, "idCliente");
                        Pesquisar(idCliente);
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