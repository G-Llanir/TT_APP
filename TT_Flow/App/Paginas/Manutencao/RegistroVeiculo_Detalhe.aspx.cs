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
using GRID = TT.FrameWork.Grid;
using TT_Flow.App.Controles;

namespace TT_Flow.App.Paginas.Manutencao
{
    public partial class RegistroVeiculo_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Veiculo";
        string sProcedure = "sp_Manipula_tbl_Flow_Veiculos";


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
              
        

                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Veiculos.Consultar, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Veiculos.Incluir, true);
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
                    vParametros.Add("@idVeiculo", idPesquisa);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {

                        hddidVeiculo.Value         = RETORNO.DATASET(dsPesquisa, 0, "idVeiculo");
                        txtidVeiculo.Text          = RETORNO.DATASET(dsPesquisa, 0, "idVeiculo");
                        txtsMarca.Text             = RETORNO.DATASET(dsPesquisa, 0, "sMarca");
                        txtsModelo.Text            = RETORNO.DATASET(dsPesquisa, 0, "sModelo");
                        txtsPlaca.Text             = RETORNO.DATASET(dsPesquisa, 0, "sPlaca").ToUpper();
                        txtsAnoModelo.Text         = RETORNO.DATASET(dsPesquisa, 0, "sAnoModelo");
                        txtsRenavam.Text           = RETORNO.DATASET(dsPesquisa, 0, "sRenavam");
                        txtsSeguro.Text            = RETORNO.DATASET(dsPesquisa, 0, "sSeguro");
                        txtdtVencimentoSeguro.Text = RETORNO.DATASET(dsPesquisa, 0, "dtVencimentoSeguro");
                        txtsCor.Text               = RETORNO.DATASET(dsPesquisa, 0, "sCor");

                        txtsLocal.Text             = RETORNO.DATASET(dsPesquisa, 0, "sLocal");
                        ddlsRstreador.SelectedValue= RETORNO.DATASET(dsPesquisa, 0, "sRstreador");
                        txtsProprietario.Text      = RETORNO.DATASET(dsPesquisa, 0, "sProprietario");
                        txtsContratoKm.Text        = RETORNO.DATASET(dsPesquisa, 0, "sContratoKm");
                        txtsContratoDuracao.Text   = RETORNO.DATASET(dsPesquisa, 0, "sContratoDuracao");
                        txtdtContrato.Text         = RETORNO.DATASET(dsPesquisa, 0, "dtContrato");
                        ddlsRodizio.SelectedValue  = RETORNO.DATASET(dsPesquisa, 0, "sRodizio");
                        txtsTipoRegistro.Text      = RETORNO.DATASET(dsPesquisa, 0, "sTipoRegistro");
                        txtsLugaresAuto.Text       = RETORNO.DATASET(dsPesquisa, 0, "sLugaresAuto");
                        txtsDadosSeguto.Text       = RETORNO.DATASET(dsPesquisa, 0, "sDadosSeguto");
                        

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));

                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsPlaca.Text);
                        BreadCrumb.TitulodaPagina = txtsPlaca.Text;
                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Veiculos.Alterar);

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
                    txtidVeiculo.Text = "Novo";
                    cmdSalvar.Text = "Incluir";
                }

                txtsPlaca.Focus();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }

   



        void LimpaCampos()
        {
            txtidVeiculo.Text = "Novo";

            txtsMarca.Text = "";
            txtsModelo.Text = "";
            txtsPlaca.Text = "";
            txtsAnoModelo.Text = "";
            txtsRenavam.Text = "";
            txtsSeguro.Text = "";
            txtdtVencimentoSeguro.Text = "";
            hddidVeiculo.Value = "0";
            txtsCor.Text = "";
            PainelAtualizacao.Visible = false;

            txtsLocal.Text = "";
            ddlsRstreador.SelectedValue = "0";
            txtsProprietario.Text = "";
            txtsContratoKm.Text = "";
            txtsContratoDuracao.Text = "";
            txtdtContrato.Text = "";
            ddlsRodizio.SelectedValue = "0";
            txtsTipoRegistro.Text = "";
            txtsLugaresAuto.Text = "";
            txtsDadosSeguto.Text = "";
        }


        private bool ValidarDados()
        {
            if (txtsMarca.Text.Length < 1)
            {
                MensagemPagina.MostraMensagem_Erro("Informe uma marca válida!");
                return false;
            }
            if (txtsPlaca.Text.Length < 8)
            {
                MensagemPagina.MostraMensagem_Erro("Placa Inválida!");
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
                    string[] vidVeiculo = hddidVeiculo.Value.Split(',');
                    string idVeiculo = vidVeiculo[0].ToString();

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idVeiculo", idVeiculo);

                    vParametros.Add("@sMarca",              txtsMarca.Text);
                    vParametros.Add("@sModelo",             txtsModelo.Text);                   
                    vParametros.Add("@sPlaca",              txtsPlaca.Text.ToUpper());
                    vParametros.Add("@sAnoModelo",          txtsAnoModelo.Text);
                    vParametros.Add("@sRenavam",            txtsRenavam.Text);
                    vParametros.Add("@sSeguro",             txtsSeguro.Text);
                    vParametros.Add("@dtVencimentoSeguro",  txtdtVencimentoSeguro.Text);
                    vParametros.Add("@sCor",                txtsCor.Text);

                    vParametros.Add("@sLocal", txtsLocal.Text);
                    vParametros.Add("@sRstreador", ddlsRstreador.SelectedValue);
                    vParametros.Add("@sProprietario", txtsProprietario.Text);
                    vParametros.Add("@sContratoKm", txtsContratoKm.Text);
                    vParametros.Add("@sContratoDuracao", txtsContratoDuracao.Text);
                    vParametros.Add("@dtContrato", txtdtContrato.Text);
                    vParametros.Add("@sRodizio", ddlsRodizio.SelectedValue);
                    vParametros.Add("@sTipoRegistro", txtsTipoRegistro.Text);
                    vParametros.Add("@sLugaresAuto", txtsLugaresAuto.Text);
                    vParametros.Add("@sDadosSeguto", txtsDadosSeguto.Text);


                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idVeiculo = RETORNO.DATASET(dsSalvar, 0, "idVeiculo");
                        Pesquisar(idVeiculo);
                        
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