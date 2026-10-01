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

namespace TT_Flow.App.Paginas.Manutencao
{
    public partial class Departamentos_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Departamento";
        string sProcedure = "sp_Manipula_tbl_Flow_Departamentos";
        public List<FrameWork.cls_Departamentos_Usuarios> bs_Departamentos_Usuarios
        {
            get
            {
                if (ViewState["bs_Departamentos_Usuarios"] == null)
                {
                    ViewState["bs_Departamentos_Usuarios"] = new List<FrameWork.cls_Departamentos_Usuarios>();
                }
                return (List<FrameWork.cls_Departamentos_Usuarios>)ViewState["bs_Departamentos_Usuarios"];
            }

            set
            {
                ViewState["bs_Departamentos_Usuarios"] = value;
            }

        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlsTpDepartamento, "sp_Select 'Flow_TipoDepartamento'", "sTipoDepartamento", "sDscTipoDepartamento", false, "Selecione o Tipo", "0");
                FUNCOES.Popula_Combo(ddlUsuarios, "sp_Select 'Usuarios'", "idUsuario", "sDscUsuario", false, "Selecione o Responsável", "0");;
                FUNCOES.Popula_Combo(ddlidDepartamentoPai, "sp_Select 'Flow_Departamentos_Pai'", "idDepartamento", "sDscDepartamento", false, "Selecione o Departamento Pai", "0");

                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Departamentos.Consultar, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Departamentos.Incluir, true);
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
                    vParametros.Add("@idDepartamento", idPesquisa);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {

                        hddidDepartamento.Value             = RETORNO.DATASET(dsPesquisa, 0, "idDepartamento");
                        txtidDepartamento.Text              = RETORNO.DATASET(dsPesquisa, 0, "idDepartamento");
                        txtsDscDepartamento.Text            = RETORNO.DATASET(dsPesquisa, 0, "sDscDepartamento");
                        txtsSiglaOS.Text                    = RETORNO.DATASET(dsPesquisa, 0, "sSiglaOS");
                        ddlsTpDepartamento.SelectedValue    = RETORNO.DATASET(dsPesquisa, 0, "sTipoDepartamento");
                        ddlidDepartamentoPai.Items.Remove(ddlidDepartamentoPai.Items.FindByValue(RETORNO.DATASET(dsPesquisa, 0, "idDepartamento")));
                        ddlidDepartamentoPai.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idDepartamentoPai");
                        ddlsFluxoEmail.SelectedValue        = RETORNO.DATASET(dsPesquisa, 0, "sFluxoEmail");

                        string sRequisicoes = RETORNO.DATASET(dsPesquisa, 0, "sExibeRequisicao");
                        string sRRHH = RETORNO.DATASET(dsPesquisa, 0, "sExibeRRHH");
                        string sComercial = RETORNO.DATASET(dsPesquisa, 0, "sExibeComercial");
                        string sComex = RETORNO.DATASET(dsPesquisa, 0, "sExibeComex");
                        string sOrdemServico = RETORNO.DATASET(dsPesquisa, 0, "sExibeOrdemServico");
                        string sPatrimonio = RETORNO.DATASET(dsPesquisa, 0, "sExibePatrimonio");
                        string sMensagens = RETORNO.DATASET(dsPesquisa, 0, "sExibeMensagens");
                        string sDepartFinanceiro = RETORNO.DATASET(dsPesquisa, 0, "sDepartamentoFinanceiro");

                        if (sRequisicoes.Equals("S")) cbRequisicoes.Checked = true;
                        if (sRRHH.Equals("S")) cbRRHH.Checked = true;
                        if (sComercial.Equals("S")) cbComercial.Checked = true;
                        if (sComex.Equals("S")) cbComex.Checked = true;
                        if (sOrdemServico.Equals("S")) cbOrdemServico.Checked = true;
                        if (sPatrimonio.Equals("S")) cbPatrimonio.Checked = true;
                        if (sMensagens.Equals("S")) cbMensagens.Checked = true;
                        if (sDepartFinanceiro.Equals("S")) cbDepartFinanceiro.Checked = true;

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"));
                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsDscDepartamento.Text);
                        Popular_dtgSelecao(dsPesquisa);
                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Departamentos.Alterar);

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
                    txtidDepartamento.Text = "Novo";
                    cmdSalvar.Text = "Incluir";
                }

                txtsDscDepartamento.Focus();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }

        void Popular_dtgSelecao(DataSet ds)
        {
            
            
            foreach (DataRow row in ds.Tables[1].Rows)
            {
                FrameWork.cls_Departamentos_Usuarios objItem = new FrameWork.cls_Departamentos_Usuarios();
                objItem.idRegistro          = Convert.ToInt32(row["idRegistro"].ToString());
                objItem.idDepartamento      = Convert.ToInt32(row["idDepartamento"].ToString());
                objItem.idUsuario           = Convert.ToInt32(row["idUsuario"].ToString());
                objItem.sDscUsuario         = row["sDscUsuario"].ToString();
                objItem.sNotificacaoEmail   = row["sNotificacaoEmail"].ToString();
                objItem.sGestorDepartamento = row["sGestorDepartamento"].ToString();
                bs_Departamentos_Usuarios.Add(objItem);
            }
            dtgSelecao_DataBind();
        }

        void dtgSelecao_DataBind()
        {
            dtgSelecao.DataSource = bs_Departamentos_Usuarios;
            dtgSelecao.DataBind();
        }



        void LimpaCampos()
        {
            txtsDscDepartamento.Text = "";
            txtsSiglaOS.Text = "";
            hddidDepartamento.Value = "0";
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
            bs_Departamentos_Usuarios.Clear();
            ddlUsuarios.SelectedValue = "0";
            txtidDepartamento.Text = "Novo";
            cbRRHH.Checked = false;
            cbComercial.Checked = false;
            cbRequisicoes.Checked = false;
            cbComex.Checked = false;
            cbOrdemServico.Checked = false;
            cbPatrimonio.Checked = false;
            cbMensagens.Checked = false;
            ddlidDepartamentoPai.SelectedValue = "0";
            cbDepartFinanceiro.Checked = false;
            ddlsFluxoEmail.SelectedValue = "";
        }

       
        private bool ValidarDados()
        {
            if (txtsDscDepartamento.Text.Length < 3)
            {
                MensagemPagina.MostraMensagem_Erro("Informe um nome válido para o Departamento!");
                return false;
            }


            if (ddlsTpDepartamento.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione o Tipo do departamento!");
                return false;
            }
   

            if (dtgSelecao.Rows.Count == 0)
            {
                MensagemPagina.MostraMensagem_Erro("Favor selecionar um responsável pelo departamento!");
                return false;
            }

            if (txtsSiglaOS.Text.Length > 10)
            {
                MensagemPagina.MostraMensagem_Erro("Informe uma sigla válida para o Departamento!");
                return false;
            }

            return true;
        }

        private bool ValidarDados_Selecao(ref string sMensagem)
        {
            if (ddlUsuarios.SelectedValue == "0")
            {
                sMensagem = "Selecione um responsável!";
                return false;
            }
            if (bs_Departamentos_Usuarios.Exists(x => x.idUsuario == Convert.ToInt32(ddlUsuarios.SelectedValue)))
            {
                sMensagem = "O Usuário " + ddlUsuarios.SelectedItem + " já pertence ao departamento!";
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
                    string[] vidDepartamento = hddidDepartamento.Value.Split(',');
                    string idDepartamento = vidDepartamento[0].ToString();

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao",                 "SALVAR");
                    vParametros.Add("@idDepartamento",          idDepartamento);
                    vParametros.Add("@sDscDepartamento",        txtsDscDepartamento.Text);
                    vParametros.Add("@sSiglaOS",                txtsSiglaOS.Text);
                    vParametros.Add("@sTipoDepartamento",       ddlsTpDepartamento.SelectedValue);
                    vParametros.Add("@sExibeComercial",         cbComercial.Checked? "S" : "N");
                    vParametros.Add("@sExibeRequisicao",        cbRequisicoes.Checked? "S" : "N");
                    vParametros.Add("@sExibeRRHH",              cbRRHH.Checked ? "S" : "N");
                    vParametros.Add("@sExibeComex",             cbComex.Checked ? "S" : "N");
                    vParametros.Add("@sExibeOrdemServico",      cbOrdemServico.Checked ? "S" : "N");
                    vParametros.Add("@sExibePatrimonio",        cbPatrimonio.Checked ? "S" : "N");
                    vParametros.Add("@sExibeMensagens",         cbMensagens.Checked ? "S" : "N");
                    vParametros.Add("@idDepartamentoPai",       ddlidDepartamentoPai.SelectedValue);
                    vParametros.Add("@sDepartamentoFinanceiro", cbDepartFinanceiro.Checked ? "S" : "N");
                    vParametros.Add("@sFluxoEmail",             ddlsFluxoEmail.SelectedValue);
                    

                    vParametros.Add("@sSituacao",               ComboAtivo.Situacao_Recuperar());
                    vParametros.Add("@idUsuarioAtualizacao",    IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idDepartamento = RETORNO.DATASET(dsSalvar, 0, "idDepartamento");
                        if (Salvar_Selecao(idDepartamento))
                        {
                            Pesquisar(idDepartamento);
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
        bool Salvar_Selecao(string idDepartamento )
        {
            bool bRetorno = false;
      
            try
            {

                //Limpa os Usuários Existentes
                DataSet dsSelecao_Excluir;
                Dictionary<String, String> vParametroSelecao_Excluir = new Dictionary<string, string>();
                vParametroSelecao_Excluir.Add("@sFuncao", "DELETE_POR_DEPARTAMENTO");
                vParametroSelecao_Excluir.Add("@idDepartamento", idDepartamento);
                dsSelecao_Excluir = BD.ExecutarDataSet(sProcedure, vParametroSelecao_Excluir);

                foreach (GridViewRow item in dtgSelecao.Rows)
                {
                    string idUsuario = item.Cells[0].Text;
                    string sNotificacaoEmail = item.Cells[3].Text.Substring(0,1);
                    string sGestorDepartamento = item.Cells[2].Text.Substring(0, 1);


                    if (idDepartamento != "0")
                    {
                        DataSet dsSelecao_Incluir;
                        Dictionary<String, String> vParametroSelecao_Incluir = new Dictionary<string, string>();

                        vParametroSelecao_Incluir.Add("@sFuncao", "INCLUIR_DEPARTAMENTOS_X_USUARIOS");
                        vParametroSelecao_Incluir.Add("@idDepartamento", idDepartamento);
                        vParametroSelecao_Incluir.Add("@idUsuario", idUsuario);
                        vParametroSelecao_Incluir.Add("@sNotificacaoEmail", sNotificacaoEmail);
                        vParametroSelecao_Incluir.Add("@sGestorDepartamento", sGestorDepartamento);
                        vParametroSelecao_Incluir.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                        dsSelecao_Incluir = BD.ExecutarDataSet(sProcedure, vParametroSelecao_Incluir);
                    }
                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

            return bRetorno;

        }

        protected void cmdIncluirSelecao_Click(object sender, EventArgs e)
        {
            string sMensagem = "";

            if (ValidarDados_Selecao(ref sMensagem))
            {

                FrameWork.cls_Departamentos_Usuarios objItem = new FrameWork.cls_Departamentos_Usuarios();


                objItem.idUsuario = Convert.ToInt32(ddlUsuarios.SelectedValue);
                objItem.sDscUsuario = ddlUsuarios.SelectedItem.ToString();
                objItem.sNotificacaoEmail = ddlsNotificacaoEmail.SelectedValue;
                objItem.sGestorDepartamento = ddlsGestorDepartamento.SelectedValue;
                bs_Departamentos_Usuarios.Add(objItem);
                dtgSelecao_DataBind();

                ddlUsuarios.SelectedValue = "0";
                ddlsNotificacaoEmail.SelectedValue = "S";
            }
            else
            {
                MensagemAcoes.MostraMensagem_Erro(sMensagem);
                //lblMensagem_Selecao.Text = sMensagem;
                // lblMensagem_Selecao.Visible = true;
            }
            //RegistraScript("");
        }

        protected void dtgSelecao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0);
        }

        protected void dtgSelecao_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            bs_Departamentos_Usuarios.RemoveAt(index);
            dtgSelecao_DataBind();
        }
    }
}