using System;
using System.Collections.Generic;
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
using System.Data;
using ListBox_Item = System.Web.UI.WebControls.ListItem;
using TT_Flow.FrameWork;
using TT_Flow.App.Controles;
using static TT.FrameWork.BD;
using static NPOI.HSSF.Util.HSSFColor;

namespace TT_Flow.App.Paginas.RRHH
{
    public partial class GHE_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Cadastro de GHE";
        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_GHE";

        #region | Contrutores


        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(lstidSetor, "sp_Manipula_tbl_Flow_Colaboradores_Setor 'SELECT_SETOR'", "idSetor", "sDscSetor", false, "Selecione o Setor", "0");
                FUNCOES.Popula_Combo(lstidEmpresa, "sp_Select 'Flow_Empresa', @idUsuario=" + IDENTITY.Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");

                // NOVO CAMPO ADICIONADO - POPULANDO O COMBO
                FUNCOES.Popula_Combo(ddlIdPlano, "sp_Manipula_tbl_Flow_Colaboradores_GHE 'SELECT-PLANOS'", "idPlano", "sDscPlano", false, "Sem Plano", "0");


                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.RRHH.GHE.Consultar, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.RRHH.GHE.Incluir, true);
                    Pesquisar("0");
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
                    Salvar();
                }

                else if (requestTarget == "funcao_Excluir")
                {
                    Excluir();
                }

            }
            RegistraScript("");
        }


        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("$v192(function() {");

            sb.Append("$v192(\"#dialog-Salvar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_SALVAR\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=GHEcmdSalvar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Salvar').dialog('open');");
            sb.Append("});");

            sb.Append("$v192(\"#dialog-Excluir\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_Excluir\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=GHEcmdExcluir]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Excluir').dialog('open');");
            sb.Append("});");

            sb.Append("});");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);

        }

        protected void Pesquisar(string idGHE)
        {
            GHEcmdExcluir.Visible = false;
            aba_historico.Visible = false;
            aba_riscos.Visible = false;
            DIV_Funcoes.Visible = false;
            string sErro = "";
            try
            {
                LimpaCampos();
                if (idGHE != "0")
                {
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idGHE", idGHE);
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidGHE.Value = RETORNO.DATASET(dsPesquisa, 0, "idGHE");
                        txtidGHE.Text = RETORNO.DATASET(dsPesquisa, 0, "idGHE");
                        hddsidSetor.Value = RETORNO.DATASET(dsPesquisa, 0, "sidSetor");
                        hddsidEmpresa.Value = RETORNO.DATASET(dsPesquisa, "sidEmpresa");
                        txtsCodigoGHE.Text = RETORNO.DATASET(dsPesquisa, "sCodigoGHE");
                        txtsDscGHE.Text = RETORNO.DATASET(dsPesquisa, "sDscGHE");
                        lblsFuncoes.Text = RETORNO.DATASET(dsPesquisa, "sFuncoes");
                        txtsDscSintese.Text = RETORNO.DATASET(dsPesquisa, "sDscSintese");
                        txtsDscAmbienteTrabalho.Text = RETORNO.DATASET(dsPesquisa, "sDscAmbienteTrabalho");
                        txtsEPC.Text = RETORNO.DATASET(dsPesquisa, "sEPC");
                        txtsMaquinas_Equipamentos.Text = RETORNO.DATASET(dsPesquisa, "sMaquinas_Equipamentos");

                        // NOVO CAMPO ADICIONADO - CARREGANDO VALOR
                        ddlIdPlano.SelectedValue = RETORNO.DATASET(dsPesquisa, "idPlano");


                        Popular_Lst(lstidSetor, RETORNO.DATASET(dsPesquisa, 0, "sidSetor"), "Setores");
                        Popular_Lst(lstidEmpresa, RETORNO.DATASET(dsPesquisa, 0, "sidEmpresa"), "Empresas");
                        sAtivo.Definir(RETORNO.DATASET(dsPesquisa, 0, "sAtivo"), "Ativo", "N");
                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario"));
                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsDscGHE.Text);

                        BreadCrumb_Pagina.TitulodaPagina = txtsDscGHE.Text;
                        lblTituloSalvar.Text = "Salvar a alteração no GHE " + txtsDscGHE.Text + "?";
                        lblTituloDuplicar.Text = "Duplicar GHE " + txtsDscGHE.Text + "?";
                        lblTitulosExcluir.Text = "Deseja excluir o GHE " + txtsDscGHE.Text + "?";

                        GHEcmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.GHE.Incluir);
                        GHEcmdExcluir.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.GHE.Incluir);
                        aba_historico.Visible = true;
                        aba_riscos.Visible = true;

                        if (!string.IsNullOrEmpty(RETORNO.DATASET(dsPesquisa, "sFuncoes")) || RETORNO.DATASET(dsPesquisa, "sFuncoes") != "")
                            DIV_Funcoes.Visible = true;

                        //if (RETORNO.DATASET(dsPesquisa, 0, "nQtdRespostas") != "0")
                        //{
                        //    BloquearEdicao(true);
                        //}

                    }
                    else
                    {
                        DIV_Body.Visible = false;
                        throw new Exception(sErro);
                    }

                }
                else
                {
                    BreadCrumb_Pagina.TitulodaPagina = "Novo";
                    lblTituloPagina.Text = string.Format("Novo {0}", sTituloPagina);
                    lblTituloSalvar.Text = "Confirma a Inclusão do GHE?";
                    GHEcmdSalvar.Text = "Incluir";
                }

                txtsDscGHE.Focus();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }

        void Popular_Lst(ListBox lst, string sValor, string sNomeObjeto)
        {
            try
            {
                foreach (ListBox_Item item in lst.Items)
                {
                    if (sValor.Split('|').Contains(item.Value))
                        item.Selected = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat("Houve um erro na tentativa de popular ", sNomeObjeto, "!<br /> ") + ex.Message);
            }


        }

        string Retornar_Lst(ListBox lst)
        {
            string sRetorno = "";
            foreach (ListBox_Item item in lst.Items)
            {
                if (item.Selected)
                    sRetorno += item.Value + "|";
            }

            return sRetorno;
        }


        void BloquearEdicao(bool bResposta)
        {
            txtsDscGHE.ReadOnly = bResposta;
            txtsDscAmbienteTrabalho.ReadOnly = bResposta;
            txtsEPC.ReadOnly = bResposta;
            txtsMaquinas_Equipamentos.ReadOnly = bResposta;
            ddlIdPlano.Enabled = !bResposta; // NOVO CAMPO ADICIONADO

            GHEcmdSalvar.Visible = !bResposta;

            if (!bResposta)
                GHEcmdExcluir.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.GHE.Incluir);
            sAtivo.BloquearEdicao(bResposta);
            lstidSetor.Enabled = !bResposta;
            if (bResposta)
                MensagemPagina.MostraMensagem_Aviso("Apenas Consulta!");
        }
        void LimpaCampos()
        {

            txtidGHE.Text = "Novo";
            txtsDscGHE.Text = "";
            txtsCodigoGHE.Text = "";
            txtsDscSintese.Text = "";
            txtsDscAmbienteTrabalho.Text = "";
            txtsEPC.Text = "";
            txtsMaquinas_Equipamentos.Text = "";
            hddidGHE.Value = "0";
            PainelAtualizacao.Visible = false;
            GHEcmdSalvar.Text = "Salvar";
            sAtivo.Definir("S", "Ativo", "N");
            lblTituloPagina.Text = sTituloPagina;
            BreadCrumb_Pagina.TitulodaPagina = "Novo";
            ddlIdPlano.ClearSelection(); // NOVO CAMPO ADICIONADO

        }

        void Salvar()
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {
                    string sMensagem = "Registro gravado com sucesso";
                    string[] vidGHE = hddidGHE.Value.Split(',');
                    string idGHE = vidGHE[0].ToString();
                    Boolean bRegistroNovo = (idGHE == "0");
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idGHE", idGHE);
                    vParametros.Add("@sDscGHE", txtsDscGHE.Text);
                    vParametros.Add("@sCodigoGHE", txtsCodigoGHE.Text);
                    vParametros.Add("@sDscSintese", txtsDscSintese.Text);
                    vParametros.Add("@sidEmpresa", Retornar_Lst(lstidEmpresa));
                    vParametros.Add("@sidSetor", Retornar_Lst(lstidSetor));
                    vParametros.Add("@sDscAmbienteTrabalho", txtsDscAmbienteTrabalho.Text);
                    vParametros.Add("@sEPC", txtsEPC.Text);
                    vParametros.Add("@sMaquinas_Equipamentos", txtsMaquinas_Equipamentos.Text);

                    // NOVO CAMPO ADICIONADO - ENVIANDO PARA A PROCEDURE
                    vParametros.Add("@idPlano", ddlIdPlano.SelectedValue);

                    vParametros.Add("@sAtivo", sAtivo.Recuperar());
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());

                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        idGHE = RETORNO.DATASET(dsSalvar, 0, "idGHE");
                        Pesquisar(idGHE);

                        if (bRegistroNovo)
                            sMensagem += "</br><a href='GHE_Detalhe.aspx?id=0'>Clique aqui para incluir uma novo GHE.</a>";

                        MensagemPagina.MostraMensagem_Sucesso(sMensagem);
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


        void Excluir()
        {
            try
            {
                string[] vidGHE = hddidGHE.Value.Split(',');
                string idGHE = vidGHE[0].ToString();

                Dictionary<String, String> vParametro = new Dictionary<string, string>();

                vParametro["@idGHE"] = hddidGHE.Value;
                vParametro["@sFuncao"] = "EXCLUIR";
                vParametro["@idUsuarioAtualizacao"] = IDENTITY.Variaveis.idUsuario();
                BD.ExecutarDataSet(sProcedure, vParametro);
                Funcoes.DirecionaPagina("App/Paginas/RRHH/GHE.aspx");
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }


        private bool ValidarDados()
        {
            string sMensagemErro = "";
            bool bRetorno = true;


            if (Validacoes.ValidarTexto(txtsCodigoGHE))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um Código válido para o GHE";
            }

            if (Validacoes.ValidarTexto(txtsDscGHE))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um texto válido para a Descrição do GHE";
            }

            // NOVO CAMPO ADICIONADO - VALIDAÇÃO
            //if (!string.IsNullOrEmpty(ddlIdPlano.SelectedValue) && ddlIdPlano.SelectedValue != "0")
            //{
            //    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Plano";
            //}

            if (Retornar_Lst(lstidEmpresa) == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma empresa!";
            }

            if (Retornar_Lst(lstidSetor) == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione quais setores fazem parte deste GHE";
            }


            if (sMensagemErro != "")
            {
                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
                bRetorno = false;
            }

            return bRetorno;
        }



    }
}
