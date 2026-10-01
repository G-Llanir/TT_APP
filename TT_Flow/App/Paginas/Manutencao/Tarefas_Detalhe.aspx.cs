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
    public partial class Tarefas_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Tarefa";
        string sProcedure = "sp_Manipula_tbl_Flow_Tarefas";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlDepartamento, "sp_Select 'Flow_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Selecione um Departamento", "0");

                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Tarefas.Consultar, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Tarefas.Incluir, true);
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
                    vParametros.Add("@idTarefa", idPesquisa);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidTarefa.Value = RETORNO.DATASET(dsPesquisa, 0, "idTarefa");
                        txtidTarefa.Text = RETORNO.DATASET(dsPesquisa, 0, "idTarefa");
                        txtsDscTarefa.Text = RETORNO.DATASET(dsPesquisa, 0, "sdscTarefa");
                        txtsObservacaoTarefa.Text = RETORNO.DATASET(dsPesquisa, 0, "sObservacaoTarefa");
                        ddlsObrigatorioConclusao.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sObrigatorioConclusao");
                        ddlDepartamento.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idDepartamento");
                        string sSTSO = RETORNO.DATASET(dsPesquisa, 0, "sSTSO");
                        string sART = RETORNO.DATASET(dsPesquisa, 0, "sART");
                        DIV_STSO.Visible = true;
                        if (sSTSO == "S")
                        {
                            STSO.Checked = true;
                        }
                        else
                        {
                            STSO.Checked = false;
                        }
                        if (sART == "S")
                        {
                            ART.Checked = true;
                        }
                        else
                        {
                            ART.Checked = false;
                        }

                        if (RETORNO.DATASET(dsPesquisa, 0, "nTempo") != "0")
                        {
                            txtnTempo.Text = RETORNO.DATASET(dsPesquisa, 0, "nTempo");
                        }

                        ddlTipoTempo.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sTipoTempo");
                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        ComboAtivo.Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"), "Ativo", "");
                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsDscTarefa.Text);

                        if (RETORNO.DATASET(dsPesquisa, 0, "sPermiteTrocarDepartamento") == "N")
                        {
                            ddlDepartamento.Attributes.Add("disabled", "disabled");
                            txtsDscTarefa.Focus();
                        }
                        else
                        {
                            ddlDepartamento.Focus();
                        }

                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Tarefas.Alterar);
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
                    ddlDepartamento.Focus();
                    DIV_STSO.Visible = false;
                    ComboAtivo.Definir("S", "Ativo", "");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        void LimpaCampos()
        {
            txtidTarefa.Text = "Nova";
            ddlDepartamento.Attributes.Add("enabled", "enabled");
            ddlDepartamento.SelectedValue = "0";
            txtsDscTarefa.Text = "";
            txtsObservacaoTarefa.Text = "";
            ddlsObrigatorioConclusao.SelectedValue = "S";
            hddidTarefa.Value = "0";
            txtnTempo.Text = "";
            ddlTipoTempo.SelectedValue = "";
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
        }

        private bool ValidarDados()
        {
            if (txtsDscTarefa.Text.Length < 12)
            {
                MensagemPagina.MostraMensagem_Erro("Informe um nome válido para a Tarefa!");
                txtsDscTarefa.Focus();
                return false;
            }
            if (ddlDepartamento.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("selecione um Departamento");
                ddlDepartamento.Focus();
                return false;
            }

            if (txtnTempo.Text != "")
            {
                if (!VALIDACOES.ValidarNumerico(txtnTempo))
                {
                    MensagemPagina.MostraMensagem_Erro("Informe um valor válido");
                    txtnTempo.Focus();
                    return false;
                }
                else
                {
                    if (ddlTipoTempo.SelectedValue == "")
                    {
                        MensagemPagina.MostraMensagem_Erro("Escolha um formato de tempo");
                        ddlTipoTempo.Focus();
                        return false;
                    }
                }
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
                    string[] vidTarefa = hddidTarefa.Value.Split(',');
                    string idTarefa = vidTarefa[0].ToString();

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idTarefa", idTarefa);
                    vParametros.Add("@idDepartamento", ddlDepartamento.SelectedValue);
                    vParametros.Add("@sDscTarefa", txtsDscTarefa.Text);
                    vParametros.Add("@sObservacaoTarefa", txtsObservacaoTarefa.Text);
                    vParametros.Add("@sObrigatorioConclusao", ddlsObrigatorioConclusao.SelectedValue);
                    vParametros.Add("@nTempo", txtnTempo.Text);
                    vParametros.Add("@sTipoTempo", ddlTipoTempo.SelectedValue);
                    vParametros.Add("@sSituacao", ComboAtivo.Recuperar());
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    if (STSO.Checked == true)
                        vParametros.Add("@sSTSO", "S");
                    else
                        vParametros.Add("@sSTSO", "N");

                    if (ART.Checked == true)
                        vParametros.Add("@sART", "S");
                    else
                        vParametros.Add("@sART", "N");

                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        if (idTarefa == "0")
                        {

                        }
                        idTarefa = RETORNO.DATASET(dsSalvar, 0, "idTarefa");
                        Pesquisar(idTarefa);
                        MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso!  </br><a href='Tarefas_Detalhe.aspx?id=0'>Clique aqui para incluir uma nova tarefa.</a>");
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