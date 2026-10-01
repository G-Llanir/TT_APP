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
using SixLabors.ImageSharp.Formats;
using Org.BouncyCastle.Asn1.X509;
using System.Security.Policy;
using MathNet.Numerics;
using System.Web.SessionState;
using static System.Net.WebRequestMethods;
using TT_Flow.App.Controles;
using static TT.FrameWork.BD;

namespace TT_Flow.App.Paginas.RRHH
{
    public partial class Avaliacao_Detalhe : System.Web.UI.Page
    {
        string sTituloPagina = "Cadastro de Avaliação";
        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_Avaliacao";

        #region | Contrutores

        public List<cls_Avaliacao_Perguntas> lstPerguntas
        {
            get
            {
                if (ViewState["lstPerguntas"] == null)
                {
                    ViewState["lstPerguntas"] = new List<cls_Avaliacao_Perguntas>();
                }
                return (List<cls_Avaliacao_Perguntas>)ViewState["lstPerguntas"];
            }
            set
            {
                ViewState["lstPerguntas"] = value;
            }
        }

        public Dictionary<int, string> lstPerguntas_Opcoes
        {
            get
            {
                if (ViewState["lstPerguntas_Opcoes"] == null)
                {
                    ViewState["lstPerguntas_Opcoes"] = new Dictionary<int, string>();
                }
                return (Dictionary<int, string>)ViewState["lstPerguntas_Opcoes"];
            }
            set
            {
                ViewState["lstPerguntas_Opcoes"] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FUNCOES.Popula_Combo(lstsidDepartamento, "sp_Select 'Flow_Colaboradores_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Todos os Departamentos", "0");

                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.RRHH.Avaliacao.Consultar, true);
                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.RRHH.Avaliacao.Incluir, true);
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
                    Salvar_Avaliacao();
                }
                else if (requestTarget == "funcao_Duplicar")
                {
                    Duplicar_Avaliacao();
                }
                else if (requestTarget == "funcao_Excluir")
                {
                    Excluir_Avaliacao();
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
            sb.Append("$v192('[id*=cmdSalvarAvaliacao]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Salvar').dialog('open');");
            sb.Append("});");

            sb.Append("$v192(\"#dialog-Duplicar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_Duplicar\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=cmdDuplicar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Duplicar').dialog('open');");
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
            sb.Append("$v192('[id*=cmdExcluir]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Excluir').dialog('open');");
            sb.Append("});");

            sb.Append("});");



            sb.Append("$('[id*=txtsReferencia]').mask('00/0000', { reverse: false });");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);

        }

        protected void Pesquisar(string idAvaliacao)
        {
            cmdDuplicar.Visible = false;
            cmdSimular.Visible = false;
            cmdExcluir.Visible = false;
            string sErro = "";
            try
            {
                LimpaCampos();
                if (idAvaliacao != "0")
                {
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                    vParametros.Add("@idAvaliacao", idAvaliacao);
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidAvaliacao.Value = RETORNO.DATASET(dsPesquisa, 0, "idAvaliacao");
                        hddsChaveGUI.Value = RETORNO.DATASET(dsPesquisa, 0, "sChaveGUI");
                        txtidAvaliacao.Text = RETORNO.DATASET(dsPesquisa, 0, "idAvaliacao");
                        hddsidDepartamento.Value = RETORNO.DATASET(dsPesquisa, 0, "sidDepartamento");
                        txtsReferencia.Text = RETORNO.DATASET(dsPesquisa, 0, "sReferencia");
                        txtsDscAvaliacao.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscAvaliacao");


                        try
                        {
                            foreach (ListBox_Item item in lstsidDepartamento.Items)
                            {
                                if (hddsidDepartamento.Value.Split('|').Contains(item.Value))
                                    item.Selected = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Houve um erro na tentativa de popular os Departamentos!<br /> " + ex.Message);
                        }

                        PopularLista_Perguntas(dsPesquisa);
                        
                        SwitchsPermiteAnonimo.Definir(RETORNO.DATASET(dsPesquisa, 0, "sPermiteAnonimo"), "Permite Anônimo", "N");
                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuario"));
                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsDscAvaliacao.Text);
                        lblTituloSalvar.Text = "Salvar a alteração na Avaliação " + txtsDscAvaliacao.Text + "?";
                        lblTituloDuplicar.Text = "Duplicar a Avaliação " + txtsDscAvaliacao.Text + "?";
                        lblTitulosExcluir.Text = "Deseja excluir a avaliação " + txtsDscAvaliacao.Text + "?";

                        cmdSalvarAvaliacao.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.Avaliacao.Incluir);
                        cmdExcluir.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.Avaliacao.Incluir);


                        if (RETORNO.DATASET(dsPesquisa, 0, "nQtdRespostas") != "0")
                        {
                            BloquearEdicao(true);
                        }
                        cmdSimular.Visible = true;
                        cmdDuplicar.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.Avaliacao.Incluir);
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
                    lblTituloPagina.Text = string.Format("Nova {0}", sTituloPagina);
                    lblTituloSalvar.Text = "Confirma a Inclusão da Avaliação?";
                    cmdSalvarAvaliacao.Text = "Incluir";
                }

                txtsReferencia.Focus();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }

        public void PopularLista_Opcoes(string sOpcoes)
        {
            int i = 0;

            if (ddlsTipo.SelectedValue != "N")
            {
                foreach (string s in sOpcoes.Split('|'))
                {
                    if (s.Length > 0)
                    {
                        i++;
                        lstPerguntas_Opcoes.Add(i, string.Format("{0}|{1}|{2}", i, s.Split(';')[0], s.Split(';')[1]));
                    }
                }
            }
            else
            {
                txtnEscala_DE.Text = sOpcoes.Split('|')[0];
                txtnEscala_Ate.Text = sOpcoes.Split('|')[1];

            }

            gvPerguntas_Opcoes_DataBind();
        }
        void BloquearEdicao(bool bResposta)
        {
            txtsReferencia.ReadOnly = bResposta;
            txtsDscAvaliacao.ReadOnly = bResposta;
            cmdIncluirPergunta.Visible = !bResposta;
            cmdSalvarAvaliacao.Visible = !bResposta;

            if (!bResposta)
            cmdExcluir.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.Avaliacao.Incluir);

            gvPergunta.Columns[3].Visible = !bResposta;
            SwitchsPermiteAnonimo.BloquearEdicao(bResposta);
            lstsidDepartamento.Enabled = !bResposta;
            if (bResposta)
            MensagemPagina.MostraMensagem_Aviso("Impossivel alterar, pois já existe respostas para esta avaliação!");
        }
        void LimpaCampos()
        {
            lstPerguntas.Clear();
            txtidAvaliacao.Text = "Novo";
            txtsDscAvaliacao.Text = "";
            txtsReferencia.Text = "";
            hddidAvaliacao.Value = "0";
            PainelAtualizacao.Visible = false;
            cmdSalvarAvaliacao.Text = "Salvar";
            SwitchsPermiteAnonimo.Definir("N", "Permite Anônimo", "N");
            lblTituloPagina.Text = sTituloPagina;
        }
        void PopularLista_Perguntas(DataSet dsAvaliacao)
        {
            foreach (DataRow row in dsAvaliacao.Tables[1].Rows)
            {
                lstPerguntas_Adicionar(
                                 Convert.ToInt32(row["idAvaliacao"])
                                 , Convert.ToInt32(row["idPergunta"])
                                 , Convert.ToInt32(row["nOrdem"])
                                 , row["sTipo"].ToString()
                                 , row["sRespondidaPor"].ToString()
                                 , row["sDScPergunta"].ToString()
                                 , row["sGrupo"].ToString()
                                 , row["sCaixadeObservacao"].ToString()
                                 , row["sOpcoes"].ToString()
                                 , "CONSULTA_DETALHE"
                    );

            }
            gvPergunta_Popular();
        }


        void lstPerguntas_Adicionar(int idAvaliacao, int idPergunta, int nOrdem, string sTipo, string sRespondidaPor, string sDscPergunta, string sGrupo, string sCaixadeObservacao, string sOpcoes, string sFuncao)
        {

            if (nOrdem == 0)
            {
                switch (sRespondidaPor)
                {
                    case "AAP":
                        nOrdem = 0;
                        break;
                    case "ASU":
                        nOrdem = 1;
                        break;
                    case "ASR":
                        nOrdem = 2;
                        break;
                    case "ASD":
                        nOrdem = 99;
                        break;
                }
            }

            cls_Avaliacao_Perguntas lstAdicionar = new cls_Avaliacao_Perguntas();
            lstAdicionar.idAvaliacao = idAvaliacao;
            lstAdicionar.idPergunta = idPergunta;
            lstAdicionar.nOrdem = nOrdem;
            lstAdicionar.sTipo = sTipo;
            lstAdicionar.sRespondidaPor = sRespondidaPor;
            lstAdicionar.sDscPergunta = sDscPergunta;
            lstAdicionar.sGrupo = sGrupo;
            lstAdicionar.sCaixadeObservacao = sCaixadeObservacao;
            lstAdicionar.sOpcoes = sOpcoes;
            lstAdicionar.sFuncao = sFuncao;
            lstAdicionar.idContador = lstPerguntas.Count() + 1;
            lstPerguntas.Add(lstAdicionar);
        }



        void Salvar_Avaliacao()
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {
                    string[] vidAvaliacao = hddidAvaliacao.Value.Split(',');
                    string idAvaliacao = vidAvaliacao[0].ToString();

                    string sidDepartamento = "";

                    foreach (ListBox_Item item in lstsidDepartamento.Items)
                    {
                        if (item.Selected)
                            sidDepartamento += item.Value + "|";
                    }


                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao",         "SALVAR");
                    vParametros.Add("@idAvaliacao",     idAvaliacao);
                    vParametros.Add("@sReferencia",     txtsReferencia.Text);
                    vParametros.Add("@sDscAvaliacao",   txtsDscAvaliacao.Text);
                    vParametros.Add("@sidDepartamento", sidDepartamento);
                    vParametros.Add("@sPermiteAnonimo", SwitchsPermiteAnonimo.Recuperar());
                    vParametros.Add("@idUsuario",       IDENTITY.Variaveis.idUsuario());

                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out sErro))
                    {
                        //Salvar as Perguntas
                        idAvaliacao = RETORNO.DATASET(dsSalvar, 0, "idAvaliacao");
                        Salvar_Perguntas(idAvaliacao);
                        
                        Pesquisar(idAvaliacao);
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

        void Duplicar_Avaliacao()
        {
            txtidAvaliacao.Text = "Novo";
            hddidAvaliacao.Value = "0";
            txtsDscAvaliacao.Text = "Duplicar - " + txtsDscAvaliacao.Text;
            cmdDuplicar.Visible = false;
            cmdSimular.Visible = false;
            lblTituloPagina.Text = txtsDscAvaliacao.Text;
            foreach (ListBox_Item item in lstsidDepartamento.Items)
            {
                item.Selected = false;
            }
            foreach (var Linha in lstPerguntas)
            {

                if (Linha.sFuncao == "CONSULTA_DETALHE")
                {
                    Linha.sFuncao = "SALVAR_PERGUNTA";
                    Linha.idPergunta = 0;
                }
            }
            PainelAtualizacao.Visible = false;
            BloquearEdicao(false);
        }

        void Excluir_Avaliacao()
        {
            try
            {
                

                Dictionary<String, String> vParametro = new Dictionary<string, string>();

                vParametro["@idAvaliacao"] = hddidAvaliacao.Value;
                vParametro["@sFuncao"] = "EXCLUIR";
                vParametro["@idUsuario"] = IDENTITY.Variaveis.idUsuario();
                BD.ExecutarDataSet(sProcedure, vParametro);
                Funcoes.DirecionaPagina("App/Paginas/RRHH/Avaliacao/Avaliacao.aspx");             
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }

        }
        void Salvar_Perguntas(string idAvaliacao)
        {
            bool bRetorno = true;
            try
            {
                foreach (var Linha in lstPerguntas)
                {
                    
                    if (Linha.sFuncao != "CONSULTA_DETALHE")
                    {
                        Dictionary<String, String> vParametro = new Dictionary<string, string>();

                        vParametro["@idAvaliacao"]          = idAvaliacao;
                        vParametro["@sFuncao"]              = Linha.sFuncao;
                        vParametro["@idPergunta"]           = Linha.idPergunta.ToString();
                        vParametro["@sDscPergunta"]         = Linha.sDscPergunta;
                        vParametro["@sGrupo"]               = Linha.sGrupo;
                        vParametro["@sRespondidaPor"]       = Linha.sRespondidaPor;
                        vParametro["@sTipo"]                = Linha.sTipo;
                        vParametro["@sCaixadeObservacao"]   = Linha.sCaixadeObservacao;
                        vParametro["@sOpcoes"]              = Linha.sOpcoes;
                        vParametro["@idUsuario"]            = IDENTITY.Variaveis.idUsuario();
                        BD.ExecutarDataSet(sProcedure, vParametro);
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
                bRetorno = false;
            }
       


        }
        protected void cmdIncluirPergunta_Click(object sender, EventArgs e)
        {
            LimpaCampos_Pergunta();
            hddidContador.Value = "0";
            lblTituloModal.Text = "Nova pergunta";
            cmdSalvarPergunta.Text = "Adicionar Pergunta";
            ddlsRespondidaPor.SelectedValue = "AAP";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus_Pergunta", "$('[id*=txtsDscPergunta]').focus();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Openmodal_Perguntas   ", "$('#modal_Perguntas').modal('show');", true);

        }

        void LimpaCampos_Pergunta()
        {
            lstPerguntas_Opcoes.Clear();
            txtsDscPergunta.Text = "";
            txtsGrupo.Text = "";
            DIV_Perguntas_Descreva.Visible = false;
            DIV_Perguntas_Opcoes.Visible = false;
            switchsCaixadeObservacao.Definir("N", "Exibir caixa para Justificativa", "N");
            ddlsRespondidaPor.SelectedValue = "0";
            ddlsTipo.SelectedValue = "0";
            cmdSalvarPergunta.Text = "Salvar";
            txtsDscPergunta.Focus();
            gvPerguntas_Opcoes_DataBind();



        }
        protected void gvPergunta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    string value = (e.Row.Cells[0].FindControl("lblidPergunta") as Label).Text;

            //    (e.Row.Cells[0].FindControl("lblidPergunta") as Label).Text = value.Split('|')[0];
            //    (e.Row.Cells[0].FindControl("lblsDscPergunta") as Label).Text = value.Split('|')[1];
            //}
        }


        protected void gvPergunta_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LimpaCampos_Pergunta();
            int idContador = 0;
            if (e.CommandName == "Alterar")
            {
                idContador = int.Parse(e.CommandArgument.ToString());
                hddidContador.Value = e.CommandArgument.ToString();
                var basePergunta = lstPerguntas[lstPerguntas.FindIndex(x => x.idContador.Equals(idContador))];

                txtsDscPergunta.Text = basePergunta.sDscPergunta.ToString();
                txtsGrupo.Text = basePergunta.sGrupo.ToString();
                ddlsRespondidaPor.SelectedValue = basePergunta.sRespondidaPor;
                ddlsTipo.SelectedValue = basePergunta.sTipo.ToString();
                ddlsTipo_SelectedIndexChanged(sender, e);
                switchsCaixadeObservacao.Definir(basePergunta.sCaixadeObservacao, "Exibir caixa de Observação", "N");
                PopularLista_Opcoes(basePergunta.sOpcoes.ToString());

                lblTituloModal.Text = "Editar Pergunta: " + basePergunta.sDscPergunta.ToString();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Openmodal_Perguntas   ", "$('#modal_Perguntas').modal('show');", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus_Pergunta", "$('[id*=txtsDscPergunta]').focus();", true);

            }


        }

        protected void gvPergunta_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
       
            int index = lstPerguntas.FindIndex(x => x.idContador.Equals(Convert.ToInt32(gvPergunta.DataKeys[e.RowIndex]["idContador"].ToString())));
            lstPerguntas[index].sFuncao = "EXCLUIR_PERGUNTA";
            
            gvPergunta_Popular();
        }

        protected void cmdSalvarPergunta_Click(object sender, EventArgs e)
        {
            gvPerguntas_Opcoes_SalvarGRID();
            if (ValidarDados_Perguntas())
            {
                string sOpcoes = "|";
                if (ddlsTipo.SelectedValue != "N")
                { 
                    lstPerguntas_Opcoes.Values.OrderBy(p => p.Split('|')[0]).ToList().ForEach(p => sOpcoes += p.Split('|')[1] + ';' + p.Split('|')[2] + "|");
                }
                else
                {
                    sOpcoes = lstPerguntas_Opcoes[0];
                }

                if (hddidContador.Value == "0")
                {
                    
                    

                    

                    lstPerguntas_Adicionar(
                                     Convert.ToInt32(hddidAvaliacao.Value)
                                     , 0
                                     , 0
                                     , ddlsTipo.SelectedValue
                                     , ddlsRespondidaPor.SelectedValue.ToString()
                                     , txtsDscPergunta.Text
                                     , txtsGrupo.Text
                                     , switchsCaixadeObservacao.Recuperar()
                                     , sOpcoes
                                     , "SALVAR_PERGUNTA"
                        );

                    txtsDscPergunta.Text = "";
                    ddlsTipo.SelectedValue = "0";
                    lstPerguntas_Opcoes.Clear();
                    gvPerguntas_Opcoes_DataBind();
                    ddlsTipo_SelectedIndexChanged(sender, e);
                    txtsDscPergunta.Focus();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Focus_Pergunta", "$('[id*=txtsDscPergunta]').focus();", true);

                }
                else
                {
                    int index = lstPerguntas.FindIndex(x => x.idContador.Equals(Convert.ToInt32(hddidContador.Value)));
                    lstPerguntas[index].sDscPergunta = txtsDscPergunta.Text;
                    lstPerguntas[index].sGrupo = txtsGrupo.Text;
                    lstPerguntas[index].sRespondidaPor = ddlsRespondidaPor.SelectedValue.ToString();
                    lstPerguntas[index].sTipo = ddlsTipo.SelectedValue;
                    lstPerguntas[index].sCaixadeObservacao = switchsCaixadeObservacao.Recuperar();
                    lstPerguntas[index].sOpcoes = sOpcoes;

                    lstPerguntas[index].sFuncao = "SALVAR_PERGUNTA";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Closemodal_Perguntas   ", "$('#modal_Perguntas').modal('hide');", true);

                }
                //AtualizarGrid
                gvPergunta_Popular();

            }

        }

        void gvPergunta_Popular()
        {
            gvPergunta.DataSource = lstPerguntas.Where(c => c.sFuncao.ToString() != "EXCLUIR_PERGUNTA").OrderBy(x => x.nOrdem).ThenBy(x => x.sGrupo).ToList();
            gvPergunta.DataBind();
        }
        protected void ddlsTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            DIV_Perguntas_Descreva.Visible = false;
            DIV_Perguntas_Opcoes.Visible = false;
            DIV_RESPOSTAS_ESCALA.Visible = false;
            DIV_RESPOSTAS_OPCOES.Visible = false;
            if (ddlsTipo.SelectedValue == "N" || ddlsTipo.SelectedValue == "S")
            {
                DIV_Perguntas_Descreva.Visible = true;
                
            }

            if (ddlsTipo.SelectedValue == "E" || ddlsTipo.SelectedValue == "M" )
            {
                DIV_Perguntas_Opcoes.Visible = true;
                DIV_RESPOSTAS_OPCOES.Visible = true; 

            }

            if (ddlsTipo.SelectedValue == "N")
            {
                DIV_Perguntas_Opcoes.Visible = true;
                DIV_RESPOSTAS_ESCALA.Visible = true;
            }


        }

        private bool ValidarDados()
        {
            string sMensagemErro = "";
            bool bRetorno = true;

            string sidDepartamento = "";

            foreach (ListBox_Item item in lstsidDepartamento.Items)
            {
                if (item.Selected)
                    sidDepartamento += item.Value + "|";
            }

            if (Validacoes.ValidarTexto(txtsReferencia))
            {
                sMensagemErro = "Informe uma referência válida (MM/AAAA)";
            }


            if (Validacoes.ValidarTexto(txtsDscAvaliacao))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um texto válido para a Descrição da Avaliação";
            }
            
            if (sidDepartamento == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione quais os departamentos fazem parte desta avaliação";
            }

            if (lstPerguntas.Count == 0)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Não é possivel salvar uma avaliação sem Perguntas!";
            }


            if (sMensagemErro != "")
            {
                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
                bRetorno = false;
            }

            return bRetorno;
        }

        private bool ValidarDados_Perguntas()
        {
            string sMensagemErro = "";
            bool bRetorno = true;
            if (Validacoes.ValidarTexto(txtsDscPergunta))
            {
                sMensagemErro = "Informe um texto válido para a pergunta!";
            }

            if (Validacoes.ValidarTexto(txtsGrupo))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um Grupo de Perguntas";
            }

            if (ddlsRespondidaPor.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe quem responde a esta Pergunta!";
            }
            
            if (ddlsTipo.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o Tipo da Pergunta!";
            }

            if (ddlsTipo.SelectedValue == "E" || ddlsTipo.SelectedValue == "N" || ddlsTipo.SelectedValue == "M")
            { 
                if (lstPerguntas_Opcoes.Count == 0)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Necessário incluir opções de resposta!";
                }
            }

            if (sMensagemErro != "")
            {
                MensagemPaginaModalDetalhe.MostraMensagem_Erro(sMensagemErro);
                bRetorno = false;
            }

            return bRetorno;



        }

        protected void cmdSimular_Click(object sender, EventArgs e)
        {
            string url = "https://avaliacao.tecandtec.com.br?sChave=" + hddsChaveGUI.Value;
            string script = $"window.open('{url}', '_blank');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "RedirectNewTab" + Guid.NewGuid(), script, true);

        }

        protected void cmdIncluirOpcao_Click(object sender, EventArgs e)
        {

            if (lstPerguntas_Opcoes.Count > 0)
            {
                gvPerguntas_Opcoes_SalvarGRID();
                lstPerguntas_Opcoes.Add(lstPerguntas_Opcoes.Last().Key + 1, string.Format("{0}|{1}|{2}", Convert.ToInt32(lstPerguntas_Opcoes.Last().Value.Split('|')[0]) + 1, "", "N"));
            }
            else
                lstPerguntas_Opcoes.Add(1, string.Format("{0}|{1}|{2}", 1, "", "N"));

            gvPerguntas_Opcoes_DataBind();
        }


        void gvPerguntas_Opcoes_SalvarGRID()
        {
            foreach (GridViewRow item in gvPerguntas_Opcoes.Rows)
            {
                if (item.RowType == DataControlRowType.DataRow)
                {
                    int I = Convert.ToInt32(gvPerguntas_Opcoes.DataKeys[item.RowIndex]["Key"].ToString());
                    lstPerguntas_Opcoes[I] = string.Format("{0}|{1}|{2}", (item.FindControl("txtnOrdem") as TextBox).Text, (item.FindControl("txtsDescricao") as TextBox).Text, (item.FindControl("ddlsJustificativa") as DropDownList).SelectedValue) ;
                }
            }

            if (ddlsTipo.SelectedValue == "N")
            {
                if (lstPerguntas_Opcoes.Count == 0)
                {
                    lstPerguntas_Opcoes.Add(0, "");
                }
                    
                lstPerguntas_Opcoes[0] = txtnEscala_DE.Text + "|" + txtnEscala_Ate.Text;
            }
        }

        void gvPerguntas_Opcoes_DataBind()
        {
            gvPerguntas_Opcoes.DataSource = lstPerguntas_Opcoes.OrderBy(o => Convert.ToInt32(o.Value.Split('|')[0]));
            gvPerguntas_Opcoes.DataBind();
        }
        protected void gvPerguntas_Opcoes_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(gvPerguntas_Opcoes.DataKeys[e.RowIndex]["Key"].ToString());
            lstPerguntas_Opcoes.Remove(id);
            gvPerguntas_Opcoes_DataBind(); 
        }

        protected void gvPerguntas_Opcoes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string value = (e.Row.Cells[0].FindControl("txtnOrdem") as TextBox).Text;

                (e.Row.Cells[0].FindControl("txtnOrdem") as TextBox).Text = value.Split('|')[0];
                (e.Row.Cells[0].FindControl("txtsDescricao") as TextBox).Text = value.Split('|')[1];
                (e.Row.Cells[0].FindControl("ddlsJustificativa") as DropDownList).SelectedValue = value.Split('|')[2];
            }
        }
    }
}