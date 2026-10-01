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


namespace TT_Flow.App.Paginas.OS.Manutencao
{
    public partial class Procedimento_Tipo_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Tipo Procedimento";
        string sProcedure = "sp_Manipula_tbl_Flow_Procedimentos_Tipo";

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidarPermissaoAcesso();
                                
            if (!IsPostBack)
            {
                
                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.OrdemServico.TiposDeProcedimentos.Consultar, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.OrdemServico.TiposDeProcedimentos.Incluir, true);
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
                    vParametros.Add("@idTipoProcedimento", idPesquisa);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidTipoProcedimento.Value         = RETORNO.DATASET(dsPesquisa, 0, "idTipoProcedimento");
                        txtidTipoProcedimento.Text          = RETORNO.DATASET(dsPesquisa, 0, "idTipoProcedimento");
                        txtsDscTipoProcedimento.Text        = RETORNO.DATASET(dsPesquisa, 0, "sDscTipoProcedimento");
                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"));
                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsDscTipoProcedimento.Text);
                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.OrdemServico.TiposDeProcedimentos.Alterar);


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


            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }



        void LimpaCampos()
        {
            txtidTipoProcedimento.Text = "Nova";
            txtsDscTipoProcedimento.Text = "";
            hddidTipoProcedimento.Value = "0";
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
        }


        private bool ValidarDados()
        {
            if (txtsDscTipoProcedimento.Text.Length < 5)
            {
                MensagemPagina.MostraMensagem_Erro("Informe um nome válido para o Tipo de Procedimento!");
                txtsDscTipoProcedimento.Focus();
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
                    string[] vidTipoProcedimento = hddidTipoProcedimento.Value.Split(',');
                    string idTipoProcedimento = vidTipoProcedimento[0].ToString();

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idTipoProcedimento", idTipoProcedimento);
                    vParametros.Add("@sDscTipoProcedimento", txtsDscTipoProcedimento.Text);
                    vParametros.Add("@sSituacao", ComboAtivo.Situacao_Recuperar());
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        if (idTipoProcedimento == "0")
                        {

                        }
                        idTipoProcedimento = RETORNO.DATASET(dsSalvar, 0, "idTipoProcedimento");
                        Pesquisar(idTipoProcedimento);
                        MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso!  </br><a href='Procedimento_Tipo.aspx?id=0'>Clique aqui para incluir uma novo tipo de procedimento.</a>");
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