using System;
using System.Collections.Generic;
using System.Data;
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
using System.Data.SqlClient;
using System.Web.Services.Description;

namespace TT_Flow.App.Paginas.WMS.Manutencao
{
    public partial class Grupo_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Grupo";
        string sProcedure = "sp_Manipula_tbl_Flow_WMS_Produtos_Grupo";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(94, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(95, true);
                    Pesquisar("0");
                }
            }
        }

        private bool ValidarDados()
        {
            if (txtsDscGrupo.Text == "" )
            {
                MensagemPagina.MostraMensagem_Erro("Informe um Descrição válida para o Grupo");
                return false;
            }

            string valida = "N";
            foreach (ListItem item in cblTipo.Items)
            {
                if (item.Value == "PROD" && item.Selected)
                {
                    if (valida == "S")
                    {
                        MensagemPagina.MostraMensagem_Erro("Não é possível selecionar mais de um Tipo de Grupo!");
                        return false;
                    }

                    valida = "S";
                }
                if (item.Value == "SER" && item.Selected)
                {
                    if (valida == "S")
                    {
                        MensagemPagina.MostraMensagem_Erro("Não é possível selecionar mais de um Tipo de Grupo!");
                        return false;
                    }

                    valida = "S";
                }
                if (item.Value != "SER" && item.Value != "PROJ" && item.Selected)
                {
                    if (cblTipo.Items.FindByValue("PROJ").Selected)
                    {
                        MensagemPagina.MostraMensagem_Erro("Apenas Grupos de Serviços podem ser Projetos!");
                        return false;
                    }
                }
                if (item.Value == "SUB_SER" && item.Selected)
                {
                    if (valida == "S")
                    {
                        MensagemPagina.MostraMensagem_Erro("Não é possível selecionar mais de um Tipo de Grupo!");
                        return false;
                    }

                    valida = "S";
                }
                if (item.Value == "REC" && item.Selected)
                {
                    if (valida == "S")
                    {
                        MensagemPagina.MostraMensagem_Erro("Não é possível selecionar mais de um Tipo de Grupo!");
                        return false;
                    }

                    valida = "S";
                }
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
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idGrupo", idPesquisa);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidGrupo.Value            = RETORNO.DATASET(dsPesquisa, 0, "idGrupo");
                        txtidGrupo.Text             = RETORNO.DATASET(dsPesquisa, 0, "idGrupo");
                        txtsDscGrupo.Text           = RETORNO.DATASET(dsPesquisa, 0, "sDscGrupo");

                        bool bProduto = RETORNO.DATASET(dsPesquisa, 0, "sProduto") == "S";
                        bool bServico = RETORNO.DATASET(dsPesquisa, 0, "sServico") == "S";
                        bool bRecurso = RETORNO.DATASET(dsPesquisa, 0, "sRecurso") == "S";
                        bool bSub = RETORNO.DATASET(dsPesquisa, 0, "sSubTipo") == "S";
                        bool bProj = RETORNO.DATASET(dsPesquisa, 0, "sProjeto") == "S";

                        cblTipo.Items.FindByValue("PROD").Selected = bProduto;
                        cblTipo.Items.FindByValue("SER").Selected = bServico;
                        cblTipo.Items.FindByValue("SUB_SER").Selected = bSub;
                        cblTipo.Items.FindByValue("REC").Selected = bRecurso;
                        cblTipo.Items.FindByValue("PROJ").Selected = bProj;

                        PainelAtualizacao.Visible   = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"));
                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsDscGrupo.Text);
                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(100);
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }
                }
                else
                {
                    BreadCrumb_Pagina.TitulodaPagina = "Novo";
                    lblTituloPagina.Text = string.Format("Novo {0}", sTituloPagina);
                    cmdSalvar.Text = "Incluir";
                }

                txtsDscGrupo.Focus();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }

        void LimpaCampos()
        {
            txtidGrupo.Text = "Novo";
            hddidGrupo.Value = "0";
            txtsDscGrupo.Text = "";
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
                    string[] vidCadastroFamilia = hddidGrupo.Value.Split(',');
                    string idCadastroFamilia = vidCadastroFamilia[0].ToString();

                    string sProduto = "N";
                    string sServico = "N";
                    string sRecurso = "N";
                    string sSub = "N";
                    string sProj = "N";

                    foreach (ListItem item in cblTipo.Items)
                    {
                        if (item.Selected && item.Value == "PROD")
                            sProduto = "S";
                        if (item.Selected && item.Value == "SER")
                            sServico = "S";
                        if (item.Selected && item.Value == "REC")
                            sRecurso = "S";
                        if (item.Selected && item.Value == "SUB_SER")
                            sSub = "S";
                        if (item.Selected && item.Value == "PROJ")
                            sProj = "S";
                    }

                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao",         "SALVAR");
                    vParametros.Add("@idGrupo",         idCadastroFamilia);
                    vParametros.Add("@sDscGrupo",       txtsDscGrupo.Text);
                    vParametros.Add("@sSituacao",           ComboAtivo.Situacao_Recuperar());
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    vParametros.Add("@sProduto", sProduto);
                    vParametros.Add("@sServico", sServico);
                    vParametros.Add("@sRecurso", sRecurso);
                    vParametros.Add("@sSubTipo", sSub);
                    vParametros.Add("@sProjeto", sProj);

                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idCadastroFamilia = RETORNO.DATASET(dsSalvar, 0, "idGrupo");
                        Pesquisar(idCadastroFamilia);
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