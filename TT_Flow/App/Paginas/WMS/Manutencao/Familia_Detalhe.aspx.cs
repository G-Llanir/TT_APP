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
                             
namespace TT_Flow.App.Paginas.WMS.Manutencao
{
    public partial class Familia_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Família de Produtos";
        string sProcedure = "sp_Manipula_tbl_Flow_WMS_Produtos_Familia";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlidFamiliaPai, "sp_Select 'Flow_WMS_Produtos_Familia'", "idFamilia", "sDscFamilia", false, "Selecione a Família", "0");

                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Familia.Consultar, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Familia.Incluir, true);
                    Pesquisar("0");
                }
            }

        }
        private bool ValidarDados()
        {
            if (txtsDscFamilia.Text == "")
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
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao",         "CONSULTAR_DETALHE");
                    vParametros.Add("@idFamilia", idPesquisa);
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidFamilia.Value              = RETORNO.DATASET(dsPesquisa, 0, "idFamilia");
                        txtidFamilia.Text               = RETORNO.DATASET(dsPesquisa, 0, "idFamilia");
                        txtsDscFamilia.Text             = RETORNO.DATASET(dsPesquisa, 0, "sDscFamilia");
                        ddlidFamiliaPai.Items.Remove(ddlidFamiliaPai.Items.FindByValue(RETORNO.DATASET(dsPesquisa, 0, "idFamilia")));
                        ddlidFamiliaPai.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idFamiliaPai");

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"));
                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsDscFamilia.Text);
                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Familia.Alterar);
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }

                }
                else
                {
                    BreadCrumb_Pagina.TitulodaPagina = "Novo";
                    lblTituloPagina.Text = string.Format("Nova {0}", sTituloPagina);
                    cmdSalvar.Text = "Incluir";
                }

                txtsDscFamilia.Focus();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }
        void LimpaCampos()
        {
            txtidFamilia.Text = "Novo";
            txtsDscFamilia.Text = "";
            hddidFamilia.Value = "0";
            ddlidFamiliaPai.SelectedValue = "0";
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
                    string[] vidFamilia = hddidFamilia.Value.Split(',');
                    string idFamilia = vidFamilia[0].ToString();

                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao",         "SALVAR");
                    vParametros.Add("@idFamilia",       idFamilia);
                    vParametros.Add("@sDscFamilia",     txtsDscFamilia.Text);
                    vParametros.Add("@idFamiliaPai",    ddlidFamiliaPai.SelectedValue);
                    vParametros.Add("@sSituacao",       ComboAtivo.Situacao_Recuperar());
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());

                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idFamilia = RETORNO.DATASET(dsSalvar, 0, "idFamilia");
                        Pesquisar(idFamilia);
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