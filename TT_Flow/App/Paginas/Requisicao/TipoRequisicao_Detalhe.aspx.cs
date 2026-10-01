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
using TT_Hub.App.Paginas.Requisicao;

namespace TT_Flow.App.Paginas.Requisicao
{
    public partial class TipoRequisicao_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Tipo de Requisição";
        string sProcedure = "sp_Manipula_tbl_Flow_Requisicao_Tipo_Requisicao";
        public string tipoMembro1 { get; set; } = "Representantes - Adms";
        public string tipoMembro2 { get; set; } = "Executores - Membros";

        public List<FrameWork.cls_TipoRequisicao> bs_TipoRequisicao_Usuarios
        {
            get
            {
                if (ViewState["bs_TipoRequisicao_Usuarios"] == null)
                {
                    ViewState["bs_TipoRequisicao_Usuarios"] = new List<FrameWork.cls_TipoRequisicao>();
                }
                return (List<FrameWork.cls_TipoRequisicao>)ViewState["bs_TipoRequisicao_Usuarios"];
            }

            set
            {
                ViewState["bs_TipoRequisicao_Usuarios"] = value;
            }

        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlUsuarios, $"{sProcedure} 'Flow-Usuarios'", "idUsuario", "sDscUsuario", false, "Selecione um Membro", "0");
                FUNCOES.Popula_Combo(ddlidDepartamento, "sp_Manipula_tbl_Flow_Requisicao 'Flow_Departamentos_Requisicao'", "idDepartamento", "sDscDepartamento", false, "Selecione o Departamento ", "0");

                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Requisicao.Manutencao.TipoRequisicao.Consultar, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Requisicao.Manutencao.TipoRequisicao.Incluir, true);
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
                    vParametros.Add("@idTipoRequisicao", idPesquisa);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {

                        hddidTipoRequisicao.Value = RETORNO.DATASET(dsPesquisa, 0, "idTipoRequisicao");
                        txtidTipoRequisicao.Text = RETORNO.DATASET(dsPesquisa, 0, "idTipoRequisicao");
                        txtsDscTipoRequisicao.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscTipoRequisicao");
                        txtsTipoRequisicao.Text = RETORNO.DATASET(dsPesquisa, 0, "sTipoRequisicao");
                        SwitchAtivo.Definir(RETORNO.DATASET(dsPesquisa, 0, "sFabricacao"), "É Fabricação?", "");

                        ddlidDepartamento.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idDepartamento");

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario"));
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sAtivo"));
                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsDscTipoRequisicao.Text);
                        Popular_dtgSelecao(dsPesquisa);
                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Requisicao.Manutencao.TipoRequisicao.Alterar);

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
                    txtidTipoRequisicao.Text = "Novo";
                    cmdSalvar.Text = "Incluir";
                }

                txtsDscTipoRequisicao.Focus();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }

        //void popularCombos()
        //{
        //    DropDownList1.Items.Clear();
        //    DropDownList1.Items.Add(new ListItem(tipoMembro1, "1"));
        //    DropDownList1.Items.Add(new ListItem(tipoMembro2, "2"));
        //}
        void Popular_dtgSelecao(DataSet ds)
        {


            foreach (DataRow row in ds.Tables[0].Rows)
            {
                FrameWork.cls_TipoRequisicao objItem = new FrameWork.cls_TipoRequisicao();
                objItem.idTipoRequisicao = Convert.ToInt32(row["idTipoRequisicao"].ToString());
                objItem.idUsuario = Convert.ToInt32(row["idUsuarioResponsavel"].ToString());
                objItem.sDscUsuario = row["sDscUsuarioResponsavel"].ToString();
                objItem.IdTipoMembro = Convert.ToInt32(row["idTipoMembro"]);
                bs_TipoRequisicao_Usuarios.Add(objItem);
            }
            dtgSelecao_DataBind();
        }

        void dtgSelecao_DataBind()
        {
            dtgSelecao.DataSource = bs_TipoRequisicao_Usuarios;
            dtgSelecao.DataBind();
        }



        void LimpaCampos()
        {
            txtsDscTipoRequisicao.Text = "";
            hddidTipoRequisicao.Value = "0";
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
            bs_TipoRequisicao_Usuarios.Clear();
            ddlUsuarios.SelectedValue = "0";
            ddlTipoMembros.SelectedValue = "0";
            txtidTipoRequisicao.Text = "Novo";
            SwitchAtivo.Definir("N", "É Fabricação?", "");
            ddlidDepartamento.SelectedValue = "0";
        }


        private bool ValidarDados()
        {
            if (txtsDscTipoRequisicao.Text.Length < 3)
            {
                MensagemPagina.MostraMensagem_Erro("Informe uma Descrição válida !");
                return false;
            }

            if (txtsTipoRequisicao.Text.Length < 3)
            {
                MensagemPagina.MostraMensagem_Erro("Informe um Tipo de Requisição válido !");
                return false;
            }

            if (dtgSelecao.Rows.Count == 0)
            {
                MensagemPagina.MostraMensagem_Erro("Favor selecionar um membro pelo Tipo de Requisição!");
                return false;
            }

            return true;
        }

        private bool ValidarDados_Selecao(ref string sMensagem)
        {
            if (ddlUsuarios.SelectedValue == "0")
            {
                sMensagem = "Selecione um membro!";
                return false;
            }
            if (ddlTipoMembros.SelectedValue == "0")
            {
                sMensagem = "Selecione um Tipo de membro, é obrigatório!";
                return false;
            }
            if (bs_TipoRequisicao_Usuarios.Exists(x => x.idUsuario == Convert.ToInt32(ddlUsuarios.SelectedValue)))
            {
                sMensagem = "O Usuário " + ddlUsuarios.SelectedItem + " já pertence ao Tipo de Requisição!";
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
                    string[] vidTipoRequisicao = hddidTipoRequisicao.Value.Split(',');
                    string idTipoRequisicao = vidTipoRequisicao[0].ToString();

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idTipoRequisicao", idTipoRequisicao);
                    vParametros.Add("@sDscTipoRequisicao", txtsDscTipoRequisicao.Text);
                    vParametros.Add("@sAtivo", ComboAtivo.Situacao_Recuperar());
                    vParametros.Add("@sTipoRequisicao", txtsTipoRequisicao.Text);
                    vParametros.Add("@sFabricacao", SwitchAtivo.Recuperar());                   
                    vParametros.Add("@idDepartamento", ddlidDepartamento.SelectedValue);     
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idTipoRequisicao = RETORNO.DATASET(dsSalvar, 0, "idTipoRequisicao");
                        if (Salvar_Selecao(idTipoRequisicao))
                        {
                            Pesquisar(idTipoRequisicao);
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
        bool Salvar_Selecao(string idTipoRequisicao)
        {
            bool bRetorno = false;

            try
            {

                DataSet dsSelecao_Excluir;
                Dictionary<String, String> vParametroSelecao_Excluir = new Dictionary<string, string>();
                vParametroSelecao_Excluir.Add("@sFuncao", "EXCLUIR_POR_TIPO_REQUISICAO");
                vParametroSelecao_Excluir.Add("@idTipoRequisicao", idTipoRequisicao);
                dsSelecao_Excluir = BD.ExecutarDataSet(sProcedure, vParametroSelecao_Excluir);

                foreach (GridViewRow item in dtgSelecao.Rows)
                {
                    string idUsuario = item.Cells[0].Text;
                    DropDownList ddlTipoMembro = (DropDownList)item.FindControl("ddlTipoMembros");

                    string idTipoMembro = ddlTipoMembro != null ? ddlTipoMembro.SelectedValue : "0";

                    DataSet dsSelecao_Incluir;
                    Dictionary<String, String> vParametroSelecao_Incluir = new Dictionary<string, string>();

                    vParametroSelecao_Incluir.Add("@sFuncao", "INCLUIR_TIPO_REQUSICAO_X_USUARIOS");
                    vParametroSelecao_Incluir.Add("@idTipoRequisicao", idTipoRequisicao);
                    vParametroSelecao_Incluir.Add("@idUsuarioResponsavel", idUsuario);
                    vParametroSelecao_Incluir.Add("@idTipoMembro", idTipoMembro);

                    dsSelecao_Incluir = BD.ExecutarDataSet(sProcedure, vParametroSelecao_Incluir);
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

                FrameWork.cls_TipoRequisicao objItem = new FrameWork.cls_TipoRequisicao();


                objItem.idUsuario = Convert.ToInt32(ddlUsuarios.SelectedValue);
                objItem.IdTipoMembro = Convert.ToInt32(ddlTipoMembros.SelectedValue);
                objItem.sDscUsuario = ddlUsuarios.SelectedItem.Text;
                bs_TipoRequisicao_Usuarios.Add(objItem);
                dtgSelecao_DataBind();

                ddlUsuarios.SelectedValue = "0";
                ddlTipoMembros.SelectedValue = "0";
            }
            else
            {
                MensagemAcoes.MostraMensagem_Erro(sMensagem);
            }
        }

        protected void dtgSelecao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0);
        }

        protected void dtgSelecao_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            bs_TipoRequisicao_Usuarios.RemoveAt(index);
            dtgSelecao_DataBind();
        }
    }
}
