using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;

namespace TT_Flow.App.Paginas.COMEX.Manutencao
{
    public partial class Ato_Concessorio_Detalhe : Page
    {
        string sTituloPagina = "Ato Concessório";
        string sProcedure = "sp_Manipula_tbl_Flow_Comex_AtoConcessorio";

        #region | Page_Load + Pesquisar
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["id"] != null)
                {
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.DirecionaPagina("app/Paginas/FAQ/FAQ_Detalhe.aspx?id=0");
                }
                if (Request["id"] == "0")
                {
                    FUNCOES.ValidaPermissao(Permissao.Comex.Manutenção_de_Dados.AtoConcessorio.Incluir, true);
                    LimpaCampos();
                    lblTituloPagina.Text = "Novo Ato Concessório";
                    PainelAtualizacaoDetalhe.Visible = false;
                    aba_Arquivos.Visible = false;
                }
            }
        }

        protected void Pesquisar(string idPesquisa)
        {
            string sErro = "";
            if (idPesquisa != "0")
            {
                lblTituloPagina.Text = sTituloPagina;
                txtdtEmissao.Attributes.Remove("disabled");
                txtdtDeferimento.Attributes.Remove("disabled");
                txtdtVencimento.Attributes.Remove("disabled");

                Dictionary<string, string> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                vParametros.Add("@idAtoConcessorio", idPesquisa);
                DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(dsPesquisa, out sErro))
                {
                    string dtEmissao = "00/00/0000";
                    string dtDeferimento = "00/00/0000";
                    string dtVencimento = "00/00/0000";

                    try
                    {
                        dtEmissao = DateTime.Parse(RETORNO.DATASET(dsPesquisa, 0, "dtEmissao")).ToString("dd/MM/yyyy");
                        dtDeferimento = DateTime.Parse(RETORNO.DATASET(dsPesquisa, 0, "dtdeferimento")).ToString("dd/MM/yyyy");
                        dtVencimento = DateTime.Parse(RETORNO.DATASET(dsPesquisa, 0, "dtvencimento")).ToString("dd/MM/yyyy");
                    }
                    catch { }

                    txtidAtoConcessorio.Text = RETORNO.DATASET(dsPesquisa, 0, "idAtoConcessorio");
                    txtsCódigo.Text = RETORNO.DATASET(dsPesquisa, 0, "sCodigoAtoConcessorio");
                    txtdtEmissao.Text = dtEmissao;
                    txtdtDeferimento.Text = dtDeferimento;
                    txtdtVencimento.Text = dtVencimento;
                    txtsCorpo.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscAtoConcessorio");
                    txtsDscObservacao.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscObservacao");
                    PainelAtualizacaoDetalhe.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario"));
                    sAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"));           //Agnes Partal - 03/09/2024

                    if (sErro != "")
                    {
                        throw new Exception(sErro);
                    }
                }

                if (!FUNCOES.ValidaPermissao(Permissao.Comex.Manutenção_de_Dados.AtoConcessorio.Alterar))
                {
                    txtsCódigo.ReadOnly = true;
                    txtdtEmissao.Attributes.Add("disabled", "disabled");
                    txtdtDeferimento.Attributes.Add("disabled", "disabled");
                    txtdtVencimento.Attributes.Add("disabled", "disabled");
                    sAtivo.Situacao_BloquearEdicao(false);
                    txtsDscObservacao.ReadOnly = true;
                    txtsCorpo.ReadOnly = true;
                    aba_Arquivos.Visible = false;
                }

                eArquivos.Attributes.Add("src", string.Format("../../Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}", idPesquisa, "AtoConcessorio"));
            }
        }

        #endregion

        #region | Utils

        protected void LimpaCampos()
        {
            txtidAtoConcessorio.Text = "Novo";
            txtsCódigo.Text = "";
            txtdtEmissao.Text = "";
            txtdtDeferimento.Text = "";
            txtdtVencimento.Text = "";
            txtsCorpo.Text = "";
            txtsDscObservacao.Text = "";
        }

        private bool ValidarDados()
        {

            if (txtsCódigo.Text.Length < 1)
            {
                MensagemPagina.MostraMensagem_Erro("Código inválido!");
                return false;
            }
            if (txtdtEmissao.Text.Length < 1)
            {
                MensagemPagina.MostraMensagem_Erro("Data de Emissão inválida!");
                return false;
            }
            if (txtdtDeferimento.Text.Length < 1)
            {
                MensagemPagina.MostraMensagem_Erro("Data de Deferimento inválida!");
                return false;
            }
            if (txtdtVencimento.Text.Length < 1)
            {
                MensagemPagina.MostraMensagem_Erro("Data de Vencimento inválida!");
                return false;
            }
            if (txtsCorpo.Text.Length < 1)
            {
                MensagemPagina.MostraMensagem_Erro("Descrição inválida!");
                return false;
            }


            return true;
        }

        #endregion

        #region | Eventos

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {
                    string idAtoConcessorio = Request["id"];
                    string idUsuario = HttpContext.Current.Session["idUsuario"].ToString();

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametrosSalvar = new Dictionary<string, string>();

                    vParametrosSalvar.Add("@sFuncao", "SALVAR_ATO_CONCESSORIO");
                    vParametrosSalvar.Add("@idAtoConcessorio", idAtoConcessorio);
                    vParametrosSalvar.Add("@sCodigoAtoConcessorio", txtsCódigo.Text);
                    vParametrosSalvar.Add("@sDscAtoConcessorio", txtsCorpo.Text.ToString());
                    vParametrosSalvar.Add("@dtEmissao", txtdtEmissao.Text);
                    vParametrosSalvar.Add("@dtDeferimento", txtdtDeferimento.Text);
                    vParametrosSalvar.Add("@dtVencimento", txtdtVencimento.Text);
                    vParametrosSalvar.Add("@sSituacao", sAtivo.Situacao_Recuperar());
                    vParametrosSalvar.Add("@idUsuarioAtualizacao", idUsuario);
                    vParametrosSalvar.Add("@sDscObservacao", txtsDscObservacao.Text);
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametrosSalvar);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        if (Request["id"] != "0")
                        {
                            Pesquisar(Request["id"]);
                            MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                        }
                        else
                        {
                            LimpaCampos();
                            MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                        }
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
    }
}