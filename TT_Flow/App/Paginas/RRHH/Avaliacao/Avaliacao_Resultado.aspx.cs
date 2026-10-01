using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow;
using System.Web;
using System.Timers;
using NPOI.SS.Formula.Functions;
using TT_Flow.App.Paginas.RRHH;
using System.Linq;

namespace TT_Hub.App.Paginas.RRHH
{
    public partial class Avaliacao_Resultado : Page
    {
        string sTituloPagina = "Resultado da Avaliação";
        string sCaminho = "App/Paginas/RRHH/Avaliacao/";
        string sPagina = "Avaliacao_Resultado.aspx";

        int nTabela_Dados = 0;
        int nTabela_AAP_Grupos = 1;
        int nTabela_AAP_Respostas = 2;
        int nTabela_ASU_Grupos = 3;
        int nTabela_ASU_Respostas = 4;
        int nTabela_ASD_Grupos = 5;
        int nTabela_ASD_Respostas = 6;

        public List<cls_AvaliacaoDetalhe> bs_AvaliacaoDetalhe
        {
            get
            {
                if (ViewState["bs_AvaliacaoDetalhe"] == null)
                {
                    ViewState["bs_AvaliacaoDetalhe"] = new List<cls_AvaliacaoDetalhe>();
                }
                return (List<cls_AvaliacaoDetalhe>)ViewState["bs_AvaliacaoDetalhe"];
            }
            set
            {
                ViewState["bs_AvaliacaoDetalhe"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.RRHH.Avaliacao.Resultado.Consultar, true);

            if (!IsPostBack)
            {
                cmdSelecao_Gerar.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.Avaliacao.Resultado.VisualizarResultado);
                cmdExcluir.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.Avaliacao.Resultado.VisualizarResultado);
                lblTituloPagina.Text = sTituloPagina;
                PopularCombos();
                if (FUNCOES.ValidaPermissao(Permissao.RRHH.Avaliacao.Resultado.VisualizarASU) && !FUNCOES.ValidaPermissao(Permissao.RRHH.Avaliacao.Resultado.VisualizarASD))
                {
                    if (Identity.Variaveis.idColaborador() != "0")
                    {
                        try
                        {
                            PopularCombos();
                            Ajusta_Selecao("Detalhe");
                            lstSupervisorDireto.SelectedValue = Identity.Variaveis.idColaborador();
                            lstSupervisorDireto.Attributes.Remove("disabled");
                            lstSupervisorDireto.Attributes.Add("disabled", "disabled");
                            div_Selecao_Departamento.Visible = true;
                            div_Selecao_Pesquisa.Visible = true;
                            div_Selecao_Supervisores.Visible = true;
                            hdd_sFuncao.Value = "Detalhe";
                            Pesquisar("Detalhe", "");

                        }
                        catch
                        {
                            MensagemPagina.MostraMensagem_Erro("Seu usuário não está cadastrado como Supervisor, impossivel continuar!");
                            DIV_PESQUISA.Visible = false;
                            pnMensagem.Visible = true;
                        }
                    }
                    else
                    {
                        MensagemPagina.MostraMensagem_Erro("Seu usuário não está vinculado como Colaborador, impossivel continuar!");
                        DIV_PESQUISA.Visible = false;
                        pnMensagem.Visible = true;
                    }

                }
                else
                {
                    div_Selecao_Cargo.Visible = false;
                    div_Selecao_Empresa.Visible = false;
                    div_Selecao_Status.Visible = false;
                    div_Selecao_Supervisores.Visible = false;
                    div_Selecao_TempoContrato.Visible = false;
                    div_Selecao_TipoContrato.Visible = false;
                    div_Selecao_Departamento.Visible = false;
                    div_Selecao_Pesquisa.Visible = false;
                    cmdVoltar.Visible = false;
                    hdd_sFuncao.Value = "Grupo";

                    Pesquisar("Grupo", "");
                }


                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
            }

            var requestTarget = this.Request["__EVENTTARGET"];
            var requestArgs = this.Request["__EVENTARGUMENT"];

            if (requestTarget == "funcao_GERAR")
            {
                GerarAvaliacoes(requestArgs);
            }

            if (requestTarget == "funcao_Excluir")
            {
                Salvar_Avaliacao("EXCLUIR");
            }


            RegistraScript("");
            FUNCOES.Scripts.FocusScript(Page, txtPesquisa.ClientID);
        }

        void GerarAvaliacoes(string sDscGrupo)
        {
            if (sDscGrupo == "")
            {
                MensagemPagina_Top.MostraMensagem_Erro("Necessário informar um nome para o Grupo");
            }
            else
            {
                try
                {
                    List<int> colaboradoresSelecionados = new List<int>();

                    foreach (GridViewRow row in gvResultado_Gerar.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            CheckBox chk = (CheckBox)row.FindControl("chkSelecionado");

                            if (chk != null && chk.Checked)
                            {
                                colaboradoresSelecionados.Add(Convert.ToInt32(gvResultado_Gerar.DataKeys[row.RowIndex]["idColaborador"]));
                            }
                        }
                    }

                    foreach (int id in colaboradoresSelecionados)
                    {
                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", "GERAR_AVALIACOES" },
                            { "@sReferencia", txtsReferencia.Text.Trim() },
                            { "@idColaborador", id.ToString() },
                            { "@idUsuario", Identity.Variaveis.idUsuario()}
                        };

                        vParametros.Add("@sDscAvaliacao", txtPesquisa.Text.Trim());
                        vParametros.Add("@sIdDepartamento", string.Join("|", lstDepartamento.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value)));
                        vParametros.Add("@sIdCargo", string.Join("|", lstCargo.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value)));
                        vParametros.Add("@sIdEmpresa", string.Join("|", lstEmpresa.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value)));
                        vParametros.Add("@sIdTipoContrato", string.Join("|", lstTipoContrato.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value)));
                        vParametros.Add("@nTipo_dtInicioContrato", ddlnTipo_dtInicioContrato.SelectedValue);
                        vParametros.Add("@sDscGrupo", sDscGrupo);

                        DataSet tb = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_Avaliacao", vParametros);

                    }

                    MensagemPagina_Top.MostraMensagem_Sucesso("Avaliações geradas com sucesso!");
                    hdd_sFuncao.Value = "Grupo";
                    Pesquisar("Grupo", "");

                }
                catch (Exception ex)
                {

                    pnMensagem.Visible = true;
                    MensagemPagina_Top.MostraMensagem_Erro("Erro ao Gerar Avaliações: " + ex.Message);
                }


            }
        }

        void RegistraScript(string sFuncao)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("$v192(function() {");

            sb.Append("$v192(\"#dialog-Gerar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("var sDscGrupo = $('[id*=txtsTextoGerar]').val();");
            sb.Append("__doPostBack(\"funcao_GERAR\", sDscGrupo);");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=cmdGerarAvaliacoes]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Gerar').dialog('open');");
            sb.Append("});");

            sb.Append("$v192(\"#dialog-Excluir\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("zIndex: 10000,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_Excluir\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            //sb.Append("__doPostBack(\"funcao_Reconsultar\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$v192('[id*=cmdExcluir]').click(function(e) {");
            sb.Append("e.preventDefault();");
            //sb.Append("$('#modalAvaliacaoDetalhe').modal('hide');");
            sb.Append("$v192('#dialog-Excluir').dialog('open');");
            sb.Append("});");


            sb.Append("});");
            sb.Append("$('[id*=txtsReferencia]').mask('00/0000', { reverse: false });");

            sb.Append("$('.composicaoLinha').addClass('fa fa-plus');\r\n");
            sb.Append("$('.composicaoLinha').click(function() {\r\n");
            sb.Append("     var icon = $(this);\r\n");
            sb.Append("     var divId = $(this).data('div-id');\r\n");
            sb.Append("     var current = $('#' + divId).css('display');\r\n");
            sb.Append("     if (current == 'none') {\r\n");
            sb.Append("         $('#' + divId).show('slow');\r\n");
            sb.Append("         icon.removeClass('fa fa-plus').addClass('fa fa-minus');\r\n");
            sb.Append("     } else {\r\n");
            sb.Append("         $('#' + divId).hide('slow');\r\n");
            sb.Append("         icon.removeClass('fa fa-minus').addClass('fa fa-plus');\r\n");
            sb.Append("     }\r\n");
            sb.Append("     return false;\r\n");
            sb.Append("});\r\n\r\n");

            //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Mascaras", "$('[id*=txtsReferencia]').mask('00/0000', { reverse: false });", true);

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);
        }

        protected void Pesquisar(string sFuncao, string sDscGrupo)
        {
            try
            {
                lblTituloPagina.Text = sTituloPagina;
                cmdSelecao_Gerar.Visible = false;
                if (sFuncao == "cmd")
                {
                    if (hdd_sFuncao.Value == "Grupo")
                    {
                        sFuncao = "Grupo";
                    }


                    if (hdd_sFuncao.Value == "Gerar_Consulta")
                    {
                        sFuncao = "Gerar";

                        if (txtsReferencia.Text == "")
                        {
                            throw new Exception("Por favor, informe uma referência!");
                        }
                    }


                    if (div_Selecao_Supervisores.Visible)
                    {
                        sFuncao = "Detalhe";
                    }
                    else if (lblTituloPagina.Text.Contains("Resultado da Avaliação - Grupo:"))
                    {
                        sFuncao = "Detalhe";
                        cmdVoltar.Visible = true;
                    }
                }



                Ajusta_Selecao(sFuncao);
                pnResultado_Detalhe.Visible = false;
                pnResultado_Gerar.Visible = false;
                pnResultado_Grupo.Visible = false;
                pnMensagem.Visible = false;
                cmdGerarAvaliacoes.Visible = false;
                string sFuncao_Consultar = "CONSULTAR_AVALIACAO_APLICADA";

                if (sFuncao == "Grupo")
                    sFuncao_Consultar = "CONSULTAR_AVALIACAO_APLICADA_GRUPO";


                if (sFuncao == "Gerar")
                    sFuncao_Consultar = "CONSULTAR_GERAR_AVALIACAOES";



                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", sFuncao_Consultar },
                    { "@sReferencia", txtsReferencia.Text.Trim() },
                };

                if (sFuncao == "Detalhe")
                {
                    vParametros.Add("@sDscGrupo", hdd_sDscGrupo.Value);
                    vParametros.Add("@sIdSupervisorDireto", string.Join("|", lstSupervisorDireto.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value)));
                    vParametros.Add("@sIdStatus", string.Join("|", lstStatus.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value)));
                }

                if (sFuncao == "Gerar")
                {
                    vParametros.Add("@sDscAvaliacao", txtPesquisa.Text.Trim());
                    vParametros.Add("@sIdDepartamento", string.Join("|", lstDepartamento.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value)));
                    vParametros.Add("@sIdCargo", string.Join("|", lstCargo.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value)));
                    vParametros.Add("@sIdEmpresa", string.Join("|", lstEmpresa.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value)));
                    vParametros.Add("@sIdTipoContrato", string.Join("|", lstTipoContrato.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value)));
                    vParametros.Add("@nTipo_dtInicioContrato", ddlnTipo_dtInicioContrato.SelectedValue);

                }

                //{ "@",  },
                //{ "@idSupervisorDireto", ddlidSupervisorDireto.SelectedValue },
                //{ "@idStatus", ddlidStatus.SelectedValue },
                //{ "@sidDepartamento", }


                DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Colaboradores_Avaliacao", vParametros);
                lblTituloGerar.Text = "Confirma a geração das avaliações?";

                if (tb.Rows.Count > 0)
                {

                    if (sFuncao == "Grupo")
                    {
                        List<cls_AvaliacaoDetalhe> detalheAvaliacao = GetDetalheAvaliacao(tb);
                        List<cls_Avaliacao> avaliacao = GetAvaliacao(detalheAvaliacao);

                        gvResultado_Grupo.Columns[1].Visible = true;
                        gvResultado_Grupo.DataSource = avaliacao;
                        gvResultado_Grupo.DataBind();
                        gvResultado_Grupo.Columns[1].Visible = false;
                        pnResultado_Grupo.Visible = true;
                        cmdVoltar.Visible = false;
                        cmdSelecao_Gerar.Visible = FUNCOES.ValidaPermissao(Permissao.RRHH.Avaliacao.Resultado.VisualizarResultado);
                        lblTituloPagina.Text = sTituloPagina;

                    }
                    else if (sFuncao == "Detalhe")
                    {
                        dtgvConsulta.Columns[0].Visible = true;
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb, 4, "asc", true, true, true, true), true);
                        //dtgvConsulta.DataSource = tb;
                        //dtgvConsulta.DataBind();
                        dtgvConsulta.Columns[0].Visible = false;
                        pnResultado_Detalhe.Visible = true;
                        if (hdd_sDscGrupo.Value != "")
                        {
                            lblTituloPagina.Text = sTituloPagina + " - Grupo: " + hdd_sDscGrupo.Value;
                            cmdVoltar.Visible = true;
                        }

                        if (lstSupervisorDireto.SelectedValue != "")
                        {
                            lblTituloPagina.Text = lblTituloPagina.Text + " - Supervisor: " + string.Join(",", lstSupervisorDireto.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Text));
                            if (hdd_sDscGrupo.Value == "")
                                cmdVoltar.Visible = false;
                        }
                        cmdSelecao_Gerar.Visible = false;
                    }
                    else if (sFuncao == "Gerar")
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables_ResultadoGerar", TT.FrameWork.Grid.DataBindComScript(gvResultado_Gerar, tb, 1, "asc", true, true, true, true), true);
                        //gvResultado_Gerar.DataSource = tb;
                        //gvResultado_Gerar.DataBind();
                        pnResultado_Gerar.Visible = true;
                        cmdGerarAvaliacoes.Visible = true;
                    }
                }
                else
                {
                    pnMensagem.Visible = true;
                    MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");
                }

            }
            catch (Exception ex)
            {

                pnMensagem.Visible = true;
                MensagemPagina.MostraMensagem_Erro("Erro ao Carregador Dados: " + ex.Message);
            }
        }

        private List<cls_Avaliacao> GetAvaliacao(List<cls_AvaliacaoDetalhe> detalheAvaliacao)
        {
            var detalhesReferencia = detalheAvaliacao
                .GroupBy(d => d.sIdReferencia)
                .ToDictionary(g => g.Key, g => g.ToList());

            var referencia = detalheAvaliacao
                .GroupBy(d => d.sIdReferencia)
                .Select(g => new cls_Avaliacao
                {
                    sIdReferencia = g.Key,
                    sReferencia = g.First().sReferencia,
                    sDscGrupo = g.First().sDscGrupo,
                    ls_AvaliacaoDetalhe = detalhesReferencia[g.Key]
                })
                .ToList();

            return referencia;
        }

        private List<cls_AvaliacaoDetalhe> GetDetalheAvaliacao(DataTable tb)
        {
            bs_AvaliacaoDetalhe.Clear();

            foreach (DataRow row in tb.Rows)
            {
                cls_AvaliacaoDetalhe avaliacaoFerias = new cls_AvaliacaoDetalhe
                {
                    sIdReferencia = row["sIdReferencia"].ToString(),
                    sDscGrupo = row["sDscGrupo"].ToString(),
                    sReferencia = row["sReferencia"].ToString(),
                    nQuantidade = row["nQuantidade"].ToString()
                };

                bs_AvaliacaoDetalhe.Add(avaliacaoFerias);
            }

            return bs_AvaliacaoDetalhe;
        }

        protected void dtgvConsulta_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CONSULTAR_AVALIACAO_APLICADA")
            {
                int index = int.Parse(e.CommandArgument.ToString());
                string idRegistro = dtgvConsulta.Rows[index].Cells[0].Text; //Codigo do arquivo Chamado
                Avaliacao_Consultar(idRegistro);
            }
            else if (e.CommandName == "CONSULTAR_AVALIACAO_GRUPO")
            {                
                string sDscGrupo = e.CommandArgument.ToString();
                PopularCombos();
                hdd_sDscGrupo.Value = sDscGrupo;
                Pesquisar("Detalhe", sDscGrupo);

            }
        }

        void Avaliacao_Consultar(string idRegistro)
        {
            aba_Avaliacao_ASD.Visible = false;
            aba_Avaliacao_ASU.Visible = false;
            aba_Resultado.Visible = false;

            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "DETALHE_AVALIACAO_APLICADA" },
                    { "@idRegistro", idRegistro }
                };
                DataSet dsAvaliacao = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_Avaliacao", vParametros);

                if (BD.ValidarDataSet(dsAvaliacao))
                {
                    Detalhe_lblTitulo.Text = String.Format("Consulta AAP #{0} - {1}", BD.Retorno.DATASET(dsAvaliacao, "idRegistro").PadLeft(6, '0'), BD.Retorno.DATASET(dsAvaliacao, "sDscColaborador"));
                    Detalhe_txtsDscColaborador.Text = BD.Retorno.DATASET(dsAvaliacao, "sDscColaborador");
                    Detalhe_txtidRegistro.Text = BD.Retorno.DATASET(dsAvaliacao, "idRegistro").PadLeft(6, '0');

                    Detalhe_txtsDscStatus.Text = BD.Retorno.DATASET(dsAvaliacao, "sDscStatus");
                    Detalhe_txtsDscDepartamento.Text = BD.Retorno.DATASET(dsAvaliacao, "sDscDepartamento");
                    Detalhe_txtsReferencia.Text = BD.Retorno.DATASET(dsAvaliacao, "sReferencia");
                    Detalhe_txtsDscSupervisor.Text = BD.Retorno.DATASET(dsAvaliacao, "sDscSupervisorDireto");
                    Detalhe_txtsObservacao.Text = BD.Retorno.DATASET(dsAvaliacao, "sObservacao");

                    AAP_txtsDscAvaliacao.Text = BD.Retorno.DATASET(dsAvaliacao, "sDscAvaliacao");
                    AAP_txtInicio.Text = BD.Retorno.DATASET(dsAvaliacao, "dtInicio");
                    AAP_txtFim.Text = BD.Retorno.DATASET(dsAvaliacao, "dtFim");
                    AAP_txtsDscColaborador.Text = Detalhe_txtsDscColaborador.Text;
                    AAP_txtsDscDepartamento.Text = Detalhe_txtsDscDepartamento.Text;


                    ASU_txtsDscColaborador.Text = Detalhe_txtsDscColaborador.Text;
                    ASU_txtsDscDepartamento.Text = Detalhe_txtsDscDepartamento.Text;
                    ASU_txtsDscSupervisorDireto.Text = Detalhe_txtsDscSupervisor.Text;
                    ASU_txtdtInicio.Text = BD.Retorno.DATASET(dsAvaliacao, "ASU_dtInicio");
                    ASU_txtdtFim.Text = BD.Retorno.DATASET(dsAvaliacao, "ASU_dtFim");


                    ASD_txtsDscColaborador.Text = Detalhe_txtsDscColaborador.Text;
                    ASD_txtsDscDepartamento.Text = Detalhe_txtsDscDepartamento.Text;
                    ASD_txtsDscSupervisorDireto.Text = Detalhe_txtsDscSupervisor.Text;
                    ASD_txtdtInicio.Text = BD.Retorno.DATASET(dsAvaliacao, "ASD_dtInicio");
                    ASD_txtdtFim.Text = BD.Retorno.DATASET(dsAvaliacao, "ASD_dtFim");



                    ASU_DivBotaoResponde.Visible = false;
                    ASU_DIV_RESPONDER.Visible = false;
                    ASU_DIV_DADOS.Visible = true;

                    if (ASU_txtdtInicio.Text == "Não Iniciada")
                    {
                        ASU_DivBotaoResponde.Visible = true;
                        ASU_cmdResponder.Text = "Nova ASU";
                    }
                    else if (ASU_txtdtFim.Text == "Não Finalizada")
                    {
                        ASU_DivBotaoResponde.Visible = true;
                        ASU_cmdResponder.Text = "Continuar ASU";
                    }


                    ASD_DivBotaoResponde.Visible = false;
                    ASD_DIV_RESPONDER.Visible = false;
                    ASD_DIV_DADOS.Visible = true;

                    if (ASD_txtdtInicio.Text == "Não Iniciada")
                    {
                        ASD_DivBotaoResponde.Visible = true;
                        ASD_cmdResponder.Text = "Nova ASD";
                    }
                    else if (ASD_txtdtFim.Text == "Não Finalizada")
                    {
                        ASD_DivBotaoResponde.Visible = true;
                        ASD_cmdResponder.Text = "Continuar ASD";
                    }



                    dsAvaliacao.Relations.Add("AAP_Relacionamento", dsAvaliacao.Tables[nTabela_AAP_Grupos].Columns["sGrupo"], dsAvaliacao.Tables[nTabela_AAP_Respostas].Columns["sGrupo"]);
                    AAP_rptAvaliacao_Grupo.DataSource = dsAvaliacao.Tables[nTabela_AAP_Grupos];
                    AAP_rptAvaliacao_Grupo.DataBind();

                    dsAvaliacao.Relations.Add("ASU_Relacionamento", dsAvaliacao.Tables[nTabela_ASU_Grupos].Columns["sGrupo"], dsAvaliacao.Tables[nTabela_ASU_Respostas].Columns["sGrupo"]);
                    ASU_rptAvaliacao.DataSource = dsAvaliacao.Tables[nTabela_ASU_Grupos];
                    ASU_rptAvaliacao.DataBind();

                    dsAvaliacao.Relations.Add("ASD_Relacionamento", dsAvaliacao.Tables[nTabela_ASD_Grupos].Columns["sGrupo"], dsAvaliacao.Tables[nTabela_ASD_Respostas].Columns["sGrupo"]);
                    ASD_rptAvaliacao.DataSource = dsAvaliacao.Tables[nTabela_ASD_Grupos];
                    ASD_rptAvaliacao.DataBind();


                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Mascaras", "$('[id*=lblTituloExcluir]').text('" + string.Format("Excluir Avaliação #{0} - {1}", Detalhe_txtidRegistro.Text, AAP_txtsDscColaborador.Text) + "');", true);


                }
                Configurar_Campos(true);

                //Permissões de Visualização





                if (AAP_txtsDscColaborador.Text != "Anônimo")
                {

                    if (FUNCOES.ValidaPermissao(Permissao.RRHH.Avaliacao.Resultado.VisualizarASU))
                        aba_Avaliacao_ASU.Visible = true;

                    if (FUNCOES.ValidaPermissao(Permissao.RRHH.Avaliacao.Resultado.VisualizarASD))
                        aba_Avaliacao_ASD.Visible = true;

                    if (FUNCOES.ValidaPermissao(Permissao.RRHH.Avaliacao.Resultado.VisualizarResultado))
                        aba_Resultado.Visible = true;
                }
                else
                {
                    aba_Avaliacao_ASD.Visible = false;
                    aba_Avaliacao_ASU.Visible = false;
                    aba_Resultado.Visible = false;
                }




                ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalDetalhe", "$('#modalAvaliacaoDetalhe').modal('show');", true);
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao consultar avaliação: " + ex.Message);
            }
        }

        void Configurar_Campos(bool bBloquear)
        {

            Detalhe_txtidRegistro.ReadOnly = bBloquear;


            Detalhe_txtsDscColaborador.ReadOnly = bBloquear;
            Detalhe_txtsDscStatus.ReadOnly = bBloquear;
            Detalhe_txtsDscDepartamento.ReadOnly = bBloquear;
            Detalhe_txtsReferencia.ReadOnly = bBloquear;
            Detalhe_txtsDscSupervisor.ReadOnly = bBloquear;

            AAP_txtsDscColaborador.ReadOnly = bBloquear;
            AAP_txtsDscDepartamento.ReadOnly = bBloquear;
            AAP_txtFim.ReadOnly = bBloquear;
            AAP_txtInicio.ReadOnly = bBloquear;
            AAP_txtsDscAvaliacao.ReadOnly = bBloquear;


            ASU_txtsDscColaborador.ReadOnly = bBloquear;
            ASU_txtsDscDepartamento.ReadOnly = bBloquear;
            ASU_txtsDscSupervisorDireto.ReadOnly = bBloquear;
            ASU_txtdtFim.ReadOnly = bBloquear;
            ASU_txtdtInicio.ReadOnly = bBloquear;


            ASD_txtsDscColaborador.ReadOnly = bBloquear;
            ASD_txtsDscDepartamento.ReadOnly = bBloquear;
            ASD_txtsDscSupervisorDireto.ReadOnly = bBloquear;
            ASD_txtdtFim.ReadOnly = bBloquear;
            ASD_txtdtInicio.ReadOnly = bBloquear;



        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        =>
            Pesquisar("cmd", "");

        protected void ASU_cmdResponder_Click(object sender, EventArgs e)
        {
            ASU_DIV_DADOS.Visible = false;
            ASU_frmResponder.Attributes.Add("src", string.Format("https://avaliacao.tecandtec.com.br/default.aspx?schave={0}&idr={1}&sRP={2}&idU={3}", HttpContext.Current.Session["sChaveSessao"].ToString(), Detalhe_txtidRegistro.Text, "ASU", Identity.Variaveis.idUsuario()));
            ASU_DIV_RESPONDER.Visible = true;
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Avaliacao_ASU-tab");
        }

        protected void ASU_cmdFechar_Click(object sender, EventArgs e)
        {
            Avaliacao_Consultar(Detalhe_txtidRegistro.Text);
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Avaliacao_ASU-tab");
        }

        protected void ASD_cmdResponder_Click(object sender, EventArgs e)
        {
            ASD_DIV_DADOS.Visible = false;
            ASD_frmResponder.Attributes.Add("src", string.Format("https://avaliacao.tecandtec.com.br/default.aspx?schave={0}&idr={1}&sRP={2}&idU={3}", HttpContext.Current.Session["sChaveSessao"].ToString(), Detalhe_txtidRegistro.Text, "ASD", Identity.Variaveis.idUsuario()));
            ASD_DIV_RESPONDER.Visible = true;
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Avaliacao_ASD-tab");

        }

        protected void ASD_cmdFechar_Click(object sender, EventArgs e)
        {
            Avaliacao_Consultar(Detalhe_txtidRegistro.Text);
            FUNCOES.Scripts.Mantem_AbaAtiva(Page, "Avaliacao_ASD-tab");
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            Salvar_Avaliacao("Salvar");
        }

        void Salvar_Avaliacao(string sFuncao)
        {
            try
            {
                string sSituacao = "S";
                if (sFuncao == "EXCLUIR")
                    sSituacao = "E";
                Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR_AVALIACAO_APLICADA" },
                { "@idRegistro",    Detalhe_txtidRegistro.Text },
                { "@sObservacao",   Detalhe_txtsObservacao.Text },
                { "@sSituacao",    sSituacao },
                { "@idUsuario", Identity.Variaveis.idUsuario()}
            };
                DataSet tb = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_Avaliacao", vParametros);

                if (sFuncao == "EXCLUIR")
                {
                    MensagemPagina_Top.MostraMensagem_Sucesso(string.Format("Avaliação #{0} - {1}, Excluida com sucesso!", Detalhe_txtidRegistro.Text, AAP_txtsDscColaborador.Text));
                    Pesquisar("", "");
                }
                else
                {
                    MensagemPagina_Modal.MostraMensagem_Sucesso("Salvo com sucesso");
                }

            }
            catch (Exception ex)
            {
                if (sFuncao == "Excluir")
                    MensagemPagina_Top.MostraMensagem_Erro("Erro ao Excluir  avaliação: " + ex.Message);
                else
                    MensagemPagina_Modal.MostraMensagem_Sucesso("Erro ao Salvar" + ex.Message);
            }


        }

        protected void cmdSelecao_Gerar_Click(object sender, EventArgs e)
        {

            PopularCombos();
            Ajusta_Selecao("Gerar");
            hdd_sFuncao.Value = "Gerar_Consulta";
            cmdSelecao_Gerar.Visible = false;
            pnResultado_Grupo.Visible = false;
            lblTituloPagina.Text = sTituloPagina + " - Gerar Avaliações";
            MensagemPagina.MostraMensagem_Aviso("Ajuste os filtros conforme necessário e clique em Consultar!");

        }

        protected void cmdGerarAvaliacoes_Click(object sender, EventArgs e)
        {

        }

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Pesquisar("Grupo", "");
        }

        void PopularCombos()
        {
            FUNCOES.Popula_Combo(lstDepartamento, "sp_Select 'Flow_Colaboradores_Departamentos'", "idDepartamento", "sDscDepartamento", false);
            FUNCOES.Popula_Combo(lstSupervisorDireto, "sp_Select 'RRHH_SUPERVISORES'", "idColaborador", "sDscColaborador", false);
            FUNCOES.Popula_Combo(lstStatus, "sp_Select 'tbl_flow_Colaboradores_Avaliacao_Aplicada_Status'", "idStatus", "sDscStatus", false);
            FUNCOES.Popula_Combo(lstEmpresa, "sp_Select 'Flow_Empresa'", "idEmpresa", "sDscEmpresa", false);
            FUNCOES.Popula_Combo(lstCargo, "sp_Select 'Flow_Colaboradores_Cargos'", "idCargo", "sDscCargo", false);
            FUNCOES.Popula_Combo(lstTipoContrato, "sp_Select 'tbl_Flow_Colaboradores_TipoContrato'", "idTipoContrato", "sDscTipoContrato", false);




        }

        void Ajusta_Selecao(string sFuncao)
        {
            bool bExibe = false;

            if (sFuncao == "Gerar")
                bExibe = true;

            div_Selecao_Cargo.Visible = bExibe;
            div_Selecao_Empresa.Visible = bExibe;
            div_Selecao_TempoContrato.Visible = bExibe;
            div_Selecao_TipoContrato.Visible = bExibe;
            div_Selecao_Departamento.Visible = bExibe;
            div_Selecao_Pesquisa.Visible = bExibe;

            if (sFuncao != "Detalhe")
                bExibe = false;
            else
                bExibe = true;
            div_Selecao_Supervisores.Visible = bExibe;
            div_Selecao_Status.Visible = bExibe;

        }

        public string NovaLinha(object id, string gridNome)
        {
            /* 
            * Passo a passo:
            * 1. Fecha a célula atual
            * 2. Fecha a linha Atual
            * 3. Cria uma nova linha com o ID e a classe <TR id='...' style='...'>
            * 4. Cria uma célula em branco: <TD></TD>
            * 5. Cria uma nova célula para conter o gridview
            ************************************************************/
            if (id != null && !string.IsNullOrEmpty(id.ToString()))
            {
                // Se houver um ID, retorna a nova linha com o ID e a classe
                return string.Format(@"</td></tr><tr id='tr{0}{1}' class='collapsed-row' style='display: none;'>
                               <td></td><td colspan='100' style='padding:0px; margin:0px;'>", gridNome, id);
            }
            else
            {
                // Se não houver ID, retorna uma string vazia para que nada seja renderizado e o botão de colapso desapareça
                return string.Empty;
            }
        }

        protected void gvResultado_Grupo_Detalhe_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.CssClass = "gvMainTd";
            }
            else if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "gvChildHeader2";
        }

        protected void gvResultado_Grupo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var avaliacao = (cls_Avaliacao)e.Row.DataItem;
                var gvDetalhe = (GridView)e.Row.FindControl("gvResultado_Grupo_Detalhe");

                if (gvDetalhe != null)
                {
                    gvDetalhe.DataSource = avaliacao.ls_AvaliacaoDetalhe;
                    gvDetalhe.DataBind();
                }
            }
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.CssClass = "gvMainTh";
        }

    }

    [Serializable]
    public class cls_AvaliacaoDetalhe
    {
        public string sIdReferencia { get; set; }
        public string sDscGrupo { get; set; }
        public string sReferencia { get; set; }
        public string nQuantidade { get; set; }
    }

    [Serializable]
    public class cls_Avaliacao
    {
        public string sIdReferencia { get; set; }
        public string sDscGrupo { get; set; }
        public string sReferencia { get; set; }
        public List<cls_AvaliacaoDetalhe> ls_AvaliacaoDetalhe { get; set; } = new List<cls_AvaliacaoDetalhe>();
    }

}