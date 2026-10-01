using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using System.Data.SqlClient;

namespace TT_Flow.App.Paginas.Manutencao
{
    public partial class Usuarios_Perfil_Detalhe : Page
    {
        string sTituloPagina = "Perfil de Acesso";
        string sProcedure = "sp_Manipula_tbl_Usuarios_Perfil";
       
        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(59, true); // Consultar
            cmdSalvar.Visible = FUNCOES.ValidaPermissao(61, false); // Alterar

            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(cblidTipoParceiro, "sp_Select 'Flow_Tipo_Parceiro', @idFiltro = 0", "idTipoParceiro", "sDscTipoParceiro", false);
                FUNCOES.Popula_Combo(ddlDashboard, "sp_Select 'Flow_DashBoards', @idFiltro=2", "idRecurso", "sDscRecurso", false, "Selecione a Página Inicial", "0");

                if (!string.IsNullOrEmpty(Request["id"]) && Request["id"] != "0")
                    Pesquisar(Request["id"].ToString());
                else
                {
                    FUNCOES.ValidaPermissao(60, true); // Incluir
                    Pesquisar("0");
                }
            }

            FUNCOES.Scripts.Aplica_TooltipPersonalizado(Page);
        }

        protected void Pesquisar(string idPesquisa)
        {
            try
            {
                LimpaCampos();

                if (idPesquisa != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idPerfil", idPesquisa }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        hddidPerfil.Value               = RETORNO.DATASET(dsPesquisa, "idPerfil");
                        txtidPerfil.Text                = RETORNO.DATASET(dsPesquisa, "idPerfil");
                        txtsDscPerfil.Text              = RETORNO.DATASET(dsPesquisa, "sDscPerfil");
                        ddlsTipoPerfil.SelectedValue    = RETORNO.DATASET(dsPesquisa, "sTipoPerfil");
                        ddlDashboard.SelectedValue      = RETORNO.DATASET(dsPesquisa, "idPaginaInicial");

                        ddlsTipoPerfil_SelectedIndexChanged(null, null);
                        TipoParceiro_Popular(RETORNO.DATASET(dsPesquisa, "sidTipoParceiro"));

                        ctrl_Recursos.sRecursos         = RETORNO.DATASET(dsPesquisa, "sRecursos");
                        ctrl_Recursos.ConsultarPermissao_Perfil(idPesquisa);

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, "sDscUsuarioAtualizacao"));
                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsDscPerfil.Text);
                    }
                    else
                        throw new Exception(sErro);
                }
                else
                {
                    BreadCrumb.TitulodaPagina = "Incluir";
                    lblTituloPagina.Text = string.Format("Novo {0}", sTituloPagina);
                    ctrl_Recursos.sRecursos = "";
                    ctrl_Recursos.ConsultarPermissao_Perfil(idPesquisa);
                    cmdSalvar.Text = "Incluir";
                }

                FUNCOES.Scripts.FocusScript(Page, txtsDscPerfil.ClientID);
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }
        
        void LimpaCampos()
        {
            txtidPerfil.Text = "Novo";
            txtsDscPerfil.Text = "";
            ddlsTipoPerfil.SelectedValue = "I";
            ddlDashboard.SelectedIndex = 0;
            DIV_TipoParceiro.Visible = false;
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
        }

        private bool ValidarDados()
        {
            if (txtsDscPerfil.Text.Length < 8)
            {
                MensagemPagina.MostraMensagem_Erro("Informe um nome válido para o Perfil!");
                return false;
            }

            return true;
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            if (ValidarDados())
            {
                try
                {
                    string[] vidPerfil = hddidPerfil.Value.Split(','); 
                    string idPerfil = vidPerfil[0].ToString();

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idPerfil", idPerfil },
                        { "@sDscPerfil", txtsDscPerfil.Text },
                        { "@sTipoPerfil", ddlsTipoPerfil.SelectedValue },
                        { "@idDashboard", ddlDashboard.SelectedValue },
                        { "@sidTipoParceiro", TipoParceiro_Concatenar() },
                        { "@sRecursos", ctrl_Recursos.RecuperarPermissao() },
                        { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                    };
                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out string sErro))
                    {
                        idPerfil = RETORNO.DATASET(dsSalvar, "idPerfil");
                        Pesquisar(idPerfil);
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
        }

        protected void ddlsTipoPerfil_SelectedIndexChanged(object sender, EventArgs e) => DIV_TipoParceiro.Visible = ddlsTipoPerfil.SelectedValue == "P";

        void TipoParceiro_Popular(string sidTipoParceiro)
        {
            string[] vidTipoParceiro = sidTipoParceiro.Split(';');

            for (int i = 0; i < vidTipoParceiro.Count(); i++)
            {
                if (!string.IsNullOrEmpty(vidTipoParceiro[i]))
                {
                    for (int contador = 0; contador <= cblidTipoParceiro.Items.Count - 1; contador++)
                    {
                        if (cblidTipoParceiro.Items[contador].Value == vidTipoParceiro[i].ToString())
                            cblidTipoParceiro.Items[contador].Selected = true;
                    }
                }
            }
        }

        string TipoParceiro_Concatenar()
        {
            string sRetornoConcatenado = "";

            for (int contador = 0; contador <= cblidTipoParceiro.Items.Count - 1; contador++)
            {
                if (cblidTipoParceiro.Items[contador].Selected)
                    sRetornoConcatenado += string.Concat(cblidTipoParceiro.Items[contador].Value, ";");
            }

            return sRetornoConcatenado;
        }
    }
}