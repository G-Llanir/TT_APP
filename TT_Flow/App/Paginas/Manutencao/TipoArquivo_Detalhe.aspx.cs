using System;
using System.Collections.Generic;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using System.Web.UI.WebControls;

namespace TT_Flow.App.Paginas.Manutencao
{
    public partial class TipoArquivo_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Tipo de Arquivo";
        string sProcedure = "sp_Manipula_tbl_Flow_Arquivos";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(ddlidRecurso, "sp_Select 'Recursos'", "idRecurso", "sDscRecurso", false, "Selecione o Recurso", "0");

                if (Request["id"] != null)
                {
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    Pesquisar("0");
                }
            }

            if (FUNCOES.ValidaPermissao(Permissao.TipoArquivo.Alterar) == false)
            {
                txtidControle.ReadOnly = true;
                txtsTipoObjeto.ReadOnly = true;
                txtsDscTipoArquivo.ReadOnly = true;
                ddlsExtensoes.Attributes.Add("disabled", "disabled");
                swtsPermiteSobrescrever.BloquearEdicao(true);
                swtsValidacaoObrigatario.BloquearEdicao(true);
                ddlidRecurso.Attributes.Add("disabled", "disabled");
                cmdSalvar.Visible = false;
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
                    vParametros.Add("@sFuncao", "CONSULTA_TIPO_ARQUIVO");
                    vParametros.Add("@idControle", idPesquisa);
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidControle.Value = RETORNO.DATASET(dsPesquisa, 0, "idControle");
                        txtidControle.Text = RETORNO.DATASET(dsPesquisa, 0, "idControle");
                        txtsTipoObjeto.Text = RETORNO.DATASET(dsPesquisa, 0, "sTipoObjeto");
                        txtsDscTipoArquivo.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscTipoArquivo");
                        ddlidRecurso.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idRecurso");
                        foreach (string item in RETORNO.DATASET(dsPesquisa, 0, "sExtensoesArquivo").Split(','))
                        {
                            try
                            {
                                ddlsExtensoes.Items.FindByValue(item).Selected = true;
                            }
                            catch 
                            { 

                            }
                        }
                            //ddlsExtensoes.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sExtensoesArquivo");
                        swtsPermiteSobrescrever.Definir(RETORNO.DATASET(dsPesquisa, 0, "sPermiteSobrescrever"), "Permitir Sobrescrever ?", "");
                        swtsValidacaoObrigatario.Definir(RETORNO.DATASET(dsPesquisa, 0, "sValidacaoObrigatario"), "Validação Obrigatória ?", "");
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
                    swtsPermiteSobrescrever.Definir("N", "Permitir Sobrescrever ?", "");
                    swtsValidacaoObrigatario.Definir("N", "Validação Obrigatória ?", "");
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        void LimpaCampos()
        {
            txtidControle.Text = "";
            txtsTipoObjeto.Text = "";
            hddidControle.Value = "0";
            txtsDscTipoArquivo.Text = "";
            ddlsExtensoes.SelectedValue = "";
        }

        private bool ValidarDados()
        {
            string sMensagem = "";

            if (txtsTipoObjeto.Text == "")
            {
                sMensagem = "Descreva o Tipo de Arquivo!";
            }
            if (txtsDscTipoArquivo.Text == "")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Descreva a Descrição!";
            }
            if (ddlidRecurso.SelectedValue == "0")
            {
                sMensagem += (sMensagem != "" ? "</br>" : "") + "Selecione o Recurso!";
            }
            if (sMensagem != "")
            {
                MensagemPagina.MostraMensagem_Erro(sMensagem);
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
                    string[] vidTipoArquivo = hddidControle.Value.Split(',');
                    string idTipoArquivo = vidTipoArquivo[0].ToString();
                    string sExtensoesArquivo = "";

                    DataSet dsSalvar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR_TIPO_ARQUIVO");
                    vParametros.Add("@idControle", idTipoArquivo);
                    vParametros.Add("@sTipoObjeto", txtsTipoObjeto.Text);
                    vParametros.Add("@sDscTipoArquivo", txtsDscTipoArquivo.Text);
                    vParametros.Add("@idRecurso", ddlidRecurso.SelectedValue);
                    vParametros.Add("@sPermiteSobrescrever", swtsPermiteSobrescrever.Recuperar());
                    vParametros.Add("@sValidacaoObrigatario", swtsValidacaoObrigatario.Recuperar());

                    foreach (ListItem item in ddlsExtensoes.Items)
                    {
                        if (item.Selected)
                        {
                            sExtensoesArquivo += item.Value;
                            sExtensoesArquivo += ",";
                        }
                    }
                    vParametros.Add("@sExtensoesArquivo", sExtensoesArquivo);

                    dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    Pesquisar(RETORNO.DATASET(dsSalvar, "idControle"));
                    MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso!");
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }
            }
        }
    }
}