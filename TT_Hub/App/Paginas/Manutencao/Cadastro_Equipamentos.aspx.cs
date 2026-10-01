using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using static TT.FrameWork.BD;
using TT_Hub.App.Controles;

namespace TT_Hub.App.Paginas.Manutencao
{
    public partial class Cadastro_Equipamentos : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            string idCliente;
            FUNCOES.ValidarPermissaoAcesso();

            RegistraScript("");
            if (!IsPostBack)
            {
                idCliente = IDENTITY.Variaveis.idCliente();
                //PreencheCombos();


                if (Request["id"] != null)
                {
                    PesquisarEquipamento(Request["id"].ToString());
                }
                else
                {
                    PesquisarEquipamento("0");
                }

            }
            else
            {
                var requestTarget = this.Request["__EVENTTARGET"];
                var requestArgs = this.Request["__EVENTARGUMENT"];

                if (requestTarget == "funcao_SAIR")
                {
                    FUNCOES.DirecionaPagina("/app/dashboard.aspx");
                }
                else if (requestTarget == "funcao_SALVAR")
                {
                    GravarEquipamento();
                }

            }
        }

        void PesquisarEquipamento(string idTipoEquipamento)
        {
            string sErro = "";
            try
            {
                LimparCampos();
                hddidTipoEquipamento.Value = "0";
                cmdSalvar.Text = "Salvar";

                if (idTipoEquipamento != "0")
                {

                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@idTipoEquipamento", idTipoEquipamento);
                    vParametros.Add("@sFuncao", "CONSULTA_DETALHE");
                    dsPesquisa = BD.ExecutarDataSet("sp_HUB_Manipula_tbl_Equipamentos_Tipo", vParametros);



                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidTipoEquipamento.Value = RETORNO.DATASET(dsPesquisa, 0, "idTipoEquipamento");
                        txtidTipoEquipamento.Text = RETORNO.DATASET(dsPesquisa, 0, "idTipoEquipamento");

                        txtsDscTipoEquipamento.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscTipoEquipamento");

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        ComboAtivo.Situacao_Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"));

                        hddidTipoEquipamento.Value = idTipoEquipamento;

                        cmdSalvar.Text = "Salvar";
                    }
                    else
                    {
                        throw new Exception("Erro ao consultar BD: " + sErro);

                    }
                }

                else
                {
                    BreadCrumb.TitulodaPagina = "Novo Equipamento";
                    lblTituloPagina.Text = "Novo Equipamento";
                    cmdSalvar.Text = "Incluir";

                }

            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.Message);
            }
        }



        void GravarEquipamento()
        {

            string sErro = "";
            if (AplicarValidacoes())
            {

                try
                {
                    string[] vidEquipamento = hddidTipoEquipamento.Value.Split(',');
                    string idEquipamentoUtilizar = vidEquipamento[0].ToString();



                    DataSet dsGravar;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "Salvar");
                    vParametros.Add("@idTipoEquipamento", hddidTipoEquipamento.Value.ToString());

                    vParametros.Add("@sDscTipoEquipamento", txtsDscTipoEquipamento.Text);

                    vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());
                    dsGravar = BD.ExecutarDataSet("sp_HUB_Manipula_tbl_Equipamentos_Tipo", vParametros);
                    vParametros.Add("@sSituacao", ComboAtivo.Situacao_Recuperar());

                    if (BD.ValidarDataSet(dsGravar, out sErro))
                    {
                        idEquipamentoUtilizar = RETORNO.DATASET(dsGravar, 0, "idTipoEquipamento");
                        MensagemPagina.MostraMensagem_Sucesso("Equipamento gravado com sucesso!");

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

                RegistraScript("");

            }


        }




        bool AplicarValidacoes()
        {
            bool retorno = true;

            if (txtsDscTipoEquipamento.Text.Length < 4)
            {
                MensagemPagina.MostraMensagem_Erro("Informe uma Descrição para o equipamento!");
                return false;
            }

            return retorno;

        }


        /*protected*/
        void cmdSalvar_Click(object sender, EventArgs e)
        {
            GravarEquipamento();
        }


        void LimparCampos()
        {
            txtsDscTipoEquipamento.Text = "";
        }

        /* protected*/
        string RetornarScripts()
        {

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
           sb.Append("alert('A');");

            //Mensagens de Confirmação
            sb.Append("$(function() {");


            sb.Append("$(\"#dialog-Salvar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_SALVAR\", \"\");");
            sb.Append("$(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$('#ContentPlaceHolder1_cmdSalvar').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$('#dialog-Salvar').dialog('open');");
            sb.Append("});");

            sb.Append("});");




            return sb.ToString();


        }

        void RegistraScript(string sScript)
        {
            sScript = RetornarScripts() + sScript;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "AddShowModalScript", sScript, true);

        }


    }



}

