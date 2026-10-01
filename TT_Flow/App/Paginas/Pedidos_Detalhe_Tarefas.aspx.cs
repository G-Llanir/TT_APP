using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using System.IO;
using System.Text;

namespace TT_Flow.App.Paginas
{
    public partial class Pedidos_Detalhe_Tarefas : Page
    {
        public static string MaximoidRegistro = "";
        public static List<string> TodosidRegistro = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidarPermissaoAcesso();

            if (FUNCOES.ValidaPermissao(Permissao.Pedidos.FinalizarMultiplasTarefas) == false)
                cmdTarefas_Finalizar.Visible = false;
            else
                cmdTarefas_Finalizar.Visible = true;

            if (!IsPostBack)
            {

                FUNCOES.Popula_Combo(ddlDepartamento, "sp_Select 'Departamentos_x_Usuarios', " + IDENTITY.Variaveis.idUsuario(), "idDepartamento", "sDscDepartamento", false, "Todos Departamentos", "0");
                FUNCOES.Popula_Combo(ddlStatus, "sp_Select 'Flow_Tarefas_Status'", "idStatusTarefa", "sDscStatusTarefa", false, "Todos os Status", "0");

                string idRegistroTarefa = "0";
                string idPedido = "0";
                string sDashBoard = "";

                if (Request["idrt"] != null)
                    idRegistroTarefa = Request["idrt"].ToString();

                if (Request["id"] != null)
                    idPedido = Request["id"].ToString();

                if (Request["dashboard"] != null)
                    sDashBoard = Request["dashboard"];

                if (Request["sTp"] != null)
                    hddidTipo.Value = Request["sTp"];
                
                PesquisarTarefas(idPedido, "0", "0", idRegistroTarefa, sDashBoard);
            }
            else
            {
                var requestTarget = this.Request["__EVENTTARGET"];
                var requestArgs = this.Request["__EVENTARGUMENT"];

                if (requestTarget == "dialog_OK")
                    cmdAcao_Confirmar_Click(hddAlterarData.Value);
            }
            RegistraScript();
        }

        protected void PesquisarTarefas(string idPedido, string idDepartamento, string idStatusTarefa, string idRegistroTarefa, string sDashBoard)
        {
            if (idRegistroTarefa == "-1")
                idRegistroTarefa = "0";

            hddidPedido.Value = idPedido;
            cmdIniciar.Visible = true;
            cmdFinalizarTarefa.Visible = false;
            cmdRejeitarTarefa.Visible = true;
            cmdObservacao.Visible = true;
            cmdTarefas_Excluir.Visible = false;
            cmdExcluirTarefa.Visible = true;
            div_botoes_Alteracao.Visible = false;
            div_EnviarArquivos.Visible = true;
            cmdProximaTarefa.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_TAREFAS" },
                { "@idPedido", idPedido },
                { "@idDepartamento", idDepartamento },
                { "@idStatusTarefa", idStatusTarefa },
                { "@idRegistroTarefa", idRegistroTarefa },
                { "@idUsuario", IDENTITY.Variaveis.idUsuario() }
            };

            if (sDashBoard == "SuasTarefas")
                vParametros.Add("@idUsuarioResponsavel", IDENTITY.Variaveis.idUsuario());
            
            DataSet dsTarefas = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos_x_Tarefas", vParametros);

            Tarefas_Acao_EsconderCaixa();
            
            if (dsTarefas.Tables[0].Rows.Count > 1)
            {
                foreach (DataRow row in dsTarefas.Tables[0].Rows)
                {
                    TodosidRegistro.Add(row["idRegistroTarefa"].ToString());
                }

                MaximoidRegistro = dsTarefas.Tables[0].Rows[dsTarefas.Tables[0].Rows.Count - 1]["idRegistroTarefa"].ToString();
            }

            if (BD.ValidarDataSet(dsTarefas, out string sErro))
            {
                if (idRegistroTarefa == "0")
                {
                    gv_Tarefas.DataSource = dsTarefas.Tables[0];
                    gv_Tarefas.DataBind();

                    if (hddidTipo.Value == "7")
                    {
                        if (FUNCOES.ValidaPermissao(Permissao.Compras.Pedido_Compras.ExcluirTarefas))
                        {
                            cmdTarefas_Excluir.Enabled = true;
                            cmdTarefas_Excluir.Visible = true;
                        }
                    }
                    else
                    {
                        if (FUNCOES.ValidaPermissao(Permissao.Pedidos.ExcluirTarefas))
                        {
                            cmdTarefas_Excluir.Enabled = true;
                            cmdTarefas_Excluir.Visible = true;
                        }
                    }

                    BaseMultiView.ActiveViewIndex = 0;
                }
                else
                {
                    LimparCampos(true);
                    FUNCOES.Popula_Combo(ddlResponsavel, "sp_Select 'Usuarios_x_Departamentos', " + RETORNO.DATASET(dsTarefas, "idDepartamento") + ", @idUsuario=" + RETORNO.DATASET(dsTarefas, "idUsuarioResponsavel"), "idUsuario", "sDscUsuario", false, "Nenhum Usuário Responsável", "0");

                    txtnDegrauTarefa.Text = RETORNO.DATASET(dsTarefas, "nDegrauTarefa").ToString();
                    txtidRegistroTarefa.Text = RETORNO.DATASET(dsTarefas, "idRegistroTarefa").ToString().PadLeft(6, '0');
                    hddidRegistroTarefas.Value = RETORNO.DATASET(dsTarefas, "idRegistroTarefa").ToString();
                    txtdtInclusao.Text = RETORNO.DATASET(dsTarefas, "dtInclusao");
                    txtsDscDepartamento.Text = RETORNO.DATASET(dsTarefas, "sDscDepartamento");
                    txtsDscTarefa.Text = RETORNO.DATASET(dsTarefas, "sDscTarefa");
                    txtdtPrevisaoConclusao.Text = RETORNO.DATASET(dsTarefas, "dtPrevisaoConclusao");
                    txtsDscStatusTarefa.Text = RETORNO.DATASET(dsTarefas, "sDscStatusTarefa");
                    txtsObservacaoTarefa.Text = RETORNO.DATASET(dsTarefas, "sObservacaoTarefa");
                    ddlResponsavel.SelectedValue = RETORNO.DATASET(dsTarefas, "idUsuarioResponsavel");
                    hddidRegistroTarefa.Value = RETORNO.DATASET(dsTarefas, "idRegistroTarefa");
                    hddidPedido.Value = RETORNO.DATASET(dsTarefas, "idPedido");

                    gv_Tarefas_Historico.DataSource = dsTarefas.Tables[1];
                    gv_Tarefas_Historico.DataBind();

                    FrameWork.cls_Arquivos Arquivo = new FrameWork.cls_Arquivos();
                    gv_Arquivo.DataSource = Arquivo.ConsultarArquivos(Convert.ToInt32(RETORNO.DATASET(dsTarefas, "idRegistroTarefa")), "Tarefas");
                    gv_Arquivo.DataBind();

                    if (RETORNO.DATASET(dsTarefas, "sGestorDepartamento") == "N")
                    {
                        cmdExcluirTarefa.Visible = false;
                        ddlResponsavel.Attributes.Add("disabled", "disabled");

                    }
                    else
                    {
                        txtdtPrevisaoConclusao.ReadOnly = false;
                        cmdObservacao.Visible = true;
                        div_botoes_Alteracao.Visible = true;
                    }

                    if (RETORNO.DATASET(dsTarefas, "sTarefaIniciada") == "S")
                    {
                        cmdFinalizarTarefa.Visible = true;
                        cmdIniciar.Visible = false;
                        cmdRejeitarTarefa.Visible = false;
                        cmdExcluirTarefa.Visible = false;
                    }

                    if (RETORNO.DATASET(dsTarefas, "sConcluido") == "S" || RETORNO.DATASET(dsTarefas, "sExcluido") == "S" || RETORNO.DATASET(dsTarefas, "sRejeitado") == "S")
                    {
                        cmdIniciar.Visible = false;
                        cmdFinalizarTarefa.Visible = false;
                        cmdObservacao.Visible = false;
                        cmdRejeitarTarefa.Visible = false;
                        cmdExcluirTarefa.Visible = false;
                        ddlResponsavel.Attributes.Add("disabled", "disabled");
                        txtdtPrevisaoConclusao.ReadOnly = true;
                        div_botoes_Alteracao.Visible = false;
                        div_EnviarArquivos.Visible = true;
                        cmdProximaTarefa.Visible = true;
                    }

                    if (hddidRegistroTarefas.Value == MaximoidRegistro)
                        cmdProximaTarefa.Visible = false;

                    PesquisarPedidoFinalizado();
                    Acao_EsconderCaixa();

                    if (!txtdtPrevisaoConclusao.ReadOnly)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("$(function() {$('[id*=txtdtPrevisaoConclusao]').datepicker({");
                        sb.Append("autoclose: true,format: 'dd/mm/yyyy',language: 'pt-BR'});});");
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);
                    }

                    BaseMultiView.ActiveViewIndex = 1;
                    txtidRegistroTarefa.Focus();
                }
            }
            else
            {
                gv_Tarefas.DataSource = null;
                gv_Tarefas.DataBind();
            }
        }

        void PesquisarPedidoFinalizado()
        {
            string sFuncao = "PESQUISA_STATUS";

            DataTable tb;
            string sSql = "sp_Consulta_tbl_Flow_Pedidos";
            Dictionary<String, String> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", sFuncao },
                { "@idPedido", hddidPedido.Value }
            };


            tb = BD.ExecutarDataTable(sSql, vParametros, false);
            DataSet ds = new DataSet();
            ds.Tables.Add(tb);

            if (tb.Rows.Count > 0)
            {
                string pedidoFinalizado = RETORNO.DATASET(ds, 0, "sPedidoFinalizado");
                if (pedidoFinalizado.Contains("S"))
                {
                    div_EnviarArquivos.Visible = false;
                }
            }
        }

        void Acao_MostrarCaixa(string sTitulo)
        {
            cmdExcluirTarefa.Enabled = false;
            cmdIniciar.Attributes.CssStyle.Add("disabled", "disabled");
            lnkIniciarTarefa.Enabled = false;
            lnkIniciarTarefaSemAlterarUsuario.Enabled = false;
            cmdFinalizarTarefa.Enabled = false;
            cmdRejeitarTarefa.Enabled = false;
            cmdObservacao.Enabled = false;
            lblAcao_Titulo.Text = sTitulo;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Collapse", "$('#div_Acoes').collapse();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Focus", "$('[id$=txtAcao_Observacao]').focus();", true);
        }

        void Acao_EsconderCaixa() => ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Hide", "$('#div_Acoes').hide();", true);

        void Tarefas_Acao_EsconderCaixa() => ScriptManager.RegisterStartupScript(this, this.GetType(), "Tarfas_Acao_Hide", "$('#div_Tarefas_Acoes').hide();", true);

        void Tarefas_Acao_MostrarCaixa(string sTitulo)
        {
            cmdTarefas_Excluir.Enabled = false;
            lblTarefas_Acao_Titulo.Text = sTitulo;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Collapse", "$('#div_Tarefas_Acoes').collapse();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Acao_Focus", "$('[id$=txtTarefas_Observacao]').focus();", true);
        }

        void LimparCampos(bool bEstadoControle)
        {
            hddsAtualizaPagina.Value = "";
            txtdtInclusao.Text = "";
            txtsDscDepartamento.Text = "";
            txtsDscTarefa.Text = "";
            txtdtPrevisaoConclusao.Text = "";
            txtsDscStatusTarefa.Text = "";
            txtsObservacaoTarefa.Text = "";
            txtAcao_Observacao.Text = "";

            txtdtInclusao.ReadOnly = bEstadoControle;
            txtsDscDepartamento.ReadOnly = bEstadoControle;
            txtsDscTarefa.ReadOnly = bEstadoControle;
            txtdtPrevisaoConclusao.ReadOnly = bEstadoControle;
            txtsDscStatusTarefa.ReadOnly = bEstadoControle;
            txtsObservacaoTarefa.ReadOnly = bEstadoControle;
            txtEnviarArquivo_sDscArquivo.Text = "";

            ddlResponsavel.Attributes.Remove("disabled");

            cmdIniciar.Attributes.CssStyle.Clear();

            lnkIniciarTarefa.Enabled = true;
            lnkIniciarTarefaSemAlterarUsuario.Enabled = true;
            cmdExcluirTarefa.Enabled = true;
            cmdFinalizarTarefa.Enabled = true;
            cmdRejeitarTarefa.Enabled = true;
            cmdObservacao.Enabled = true;

        }

        protected void gv_Tarefas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0, 1);

            if (hddidTipo.Value == "7")
            {
                if (FUNCOES.ValidaPermissao(Permissao.Compras.Pedido_Compras.ExcluirTarefas))
                {
                    GRID.MostrarColunas(e, 0);
                }
            }
            else
            {
                if (FUNCOES.ValidaPermissao(Permissao.Pedidos.ExcluirTarefas))
                {
                    GRID.MostrarColunas(e, 0);
                }
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string idStatusTarefa = DataBinder.Eval(e.Row.DataItem, "idStatusTarefa").ToString();
                e.Row.CssClass = DataBinder.Eval(e.Row.DataItem, "sCor").ToString();

                if (!idStatusTarefa.Contains("1") && !idStatusTarefa.Contains("6"))
                {
                    e.Row.Cells[0].Controls.Clear();
                }

            }
        }

        protected void ddlDepartamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            PesquisarTarefas(hddidPedido.Value, ddlDepartamento.SelectedValue, ddlStatus.SelectedValue, "0", "");
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            PesquisarTarefas(hddidPedido.Value, ddlDepartamento.SelectedValue, ddlStatus.SelectedValue, "0", "");
        }

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Acao_EsconderCaixa();
            TodosidRegistro.Clear();
            if (hddsAtualizaPagina.Value == "S")
            {
                if (Request["dashboard"] != null)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "AddVoltarDashBoard", string.Format("<script>window.top.location.href = 'Tarefas.aspx?Dashboard={0}' </script>", Request["dashboard"].ToString()), false);

                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "AddVoltarPedido", string.Format("<script>window.top.location.href = 'Pedidos_Detalhe.aspx?id={0}&idrt=-1' </script>", Request["id"].ToString()), false);
                }
            }
            else
            {
                if (Request["dashboard"] != null)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "AddVoltarDashBoard", string.Format("<script>window.top.location.href = 'Tarefas.aspx?Dashboard={0}' </script>", Request["dashboard"].ToString()), false);
                }
                else
                {
                    FUNCOES.DirecionaPagina(string.Format("app/Paginas/Pedidos_Detalhe_Tarefas.aspx?id={0}", hddidPedido.Value));
                }
            }
        }

        bool ModificarStatusTarefa(string idRegistroTarefa, string sFuncao)
        {
            bool bRetorno = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", sFuncao },
                { "@idRegistroTarefa", idRegistroTarefa },
                { "@sObservacao", txtAcao_Observacao.Text },
                { "@idUsuario", IDENTITY.Variaveis.idUsuario() },
                { "@idUsuarioResponsavel", ddlResponsavel.SelectedValue }
            };
            DataSet dsTarefas = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos_x_Tarefas", vParametros);

            if (BD.ValidarDataSet(dsTarefas, out string sErro))
            {
                PesquisarTarefas("0", "0", "0", idRegistroTarefa, "");
                MensagemPagina.MostraMensagem_Sucesso(RETORNO.DATASET(dsTarefas, "msg"));
                hddsAtualizaPagina.Value = RETORNO.DATASET(dsTarefas, "sAtualizaPagina");

                bRetorno = true;
            }
            else
            {
                MensagemPagina.MostraMensagem_Erro(RETORNO.DATASET(dsTarefas, "msg"));
                Acao_EsconderCaixa();

                bRetorno =  false;
            }

            Dictionary<string, string> vParam = new Dictionary<string, string>
            {
                { "@sFuncao", "ATUALIZA_EVENTOS_x_TAREFA" },
                { "@idRegistroTarefa", idRegistroTarefa }
            };
            BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos_x_Tarefas", vParam);

            return bRetorno;
        }

        protected void cmdFinalizarTarefa_Click(object sender, EventArgs e)
        {
            if (cmdFinalizarTarefa.Text == "Finalizar")
            {
                hddacao_sFuncao.Value = "FINALIZAR_TAREFA";
                Acao_MostrarCaixa("Finalizar Tarefa");
            }
            else
            {
                ModificarStatusTarefa(hddidRegistroTarefa.Value, "INICIAR_TAREFA");
            }
        }

        protected void cmdRejeitarTarefa_Click(object sender, EventArgs e)
        {
            hddacao_sFuncao.Value = "REJEITAR_TAREFA";
            Acao_MostrarCaixa("Rejeitar Tarefa");
        }

        protected void cmdExcluirTarefa_Click(object sender, EventArgs e)
        {
            hddacao_sFuncao.Value = "EXCLUIR_TAREFA";
            Acao_MostrarCaixa("Excluir Tarefa");
        }

        protected void cmdAcao_Cancelar_Click(object sender, EventArgs e)
        {
            PesquisarTarefas("0", "0", "0", hddidRegistroTarefa.Value, "");
        }

        void cmdAcao_Confirmar_Click(string AlterarData)
        {
            if (txtAcao_Observacao.Text.Length < 6)
                MensagemPagina.MostraMensagem_Erro("Insira um Motivo/Observação válido!");
            else
            {
                if(hddAlterarData.Value == "S")
                {
                    hddDataconcluir.Value = DateTime.Now.ToString("dd/MM/yyyy");
                    DateTime dataConcluir = DateTime.Parse(hddDataconcluir.Value);
                    DateTime dataPrevisaoConclusao = DateTime.Parse(txtdtPrevisaoConclusao.Text);

                    if (dataPrevisaoConclusao < dataConcluir)
                    {
                        txtdtPrevisaoConclusao.Text = hddDataconcluir.Value;
                        cmdSalvarAlteracoes_Click(null, EventArgs.Empty);
                    }
                }

                ModificarStatusTarefa(hddidRegistroTarefa.Value, hddacao_sFuncao.Value);
            }
        }

        protected void cmdObservacao_Click(object sender, EventArgs e)
        {
            hddacao_sFuncao.Value = "INSERIR_OBSERVACAO";
            Acao_MostrarCaixa("Inserir uma Observação na Tarefa");
        }

        protected void cmdSalvarAlteracoes_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SALVAR_ALTERACOES" },
                { "@idRegistroTarefa", hddidRegistroTarefa.Value },
                { "@idUsuarioResponsavel", sender == null ? "-1" : ddlResponsavel.SelectedValue },
                { "@dtPrevisaoConclusao", txtdtPrevisaoConclusao.Text },
                { "@idUsuario", IDENTITY.Variaveis.idUsuario() }
            };
            DataSet dsTarefas = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos_x_Tarefas", vParametros);

            if (BD.ValidarDataSet(dsTarefas, out string sErro))
            {
                PesquisarTarefas("0", "0", "0", hddidRegistroTarefa.Value, "");
                MensagemPagina.MostraMensagem_Sucesso(RETORNO.DATASET(dsTarefas, "msg"));
            }
            else
            {
                PesquisarTarefas("0", "0", "0", hddidRegistroTarefa.Value, "");
                MensagemPagina.MostraMensagem_Erro(RETORNO.DATASET(dsTarefas, "msg"));
                Acao_EsconderCaixa();
            }
        }

        protected void cmdEnviarArquivos_Click(object sender, EventArgs e)
        {

            if (ValidarEnvioArquivo())
            {
                string sErro = "";
                Byte[] lObjArquivo = null;
                Stream lObjConteudoArquivo = fu_Arquivo.PostedFile.InputStream;
                string sNomeArquivo = fu_Arquivo.FileName;
                string lStrCaminhoArquivo = fu_Arquivo.PostedFile.FileName;
                string lStrNomeArquivo = Path.GetFileName(lStrCaminhoArquivo);
                string lStrExtencaoArquivo = Path.GetExtension(lStrNomeArquivo);

                try
                {
                    TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                    lObjArquivo = objArquivo.TransformaArquivoEmArrayBytes(lStrNomeArquivo, lStrCaminhoArquivo, lObjConteudoArquivo);
                }
                catch
                {
                    return;
                }

                TT_Flow.FrameWork.cls_Arquivos Arquivo = new FrameWork.cls_Arquivos();
                Arquivo.idTipoArquivo = 99;
                Arquivo.idObjeto = Convert.ToInt32(hddidRegistroTarefa.Value);
                Arquivo.sNomeArquivo = sNomeArquivo;
                Arquivo.sDscArquivo = txtEnviarArquivo_sDscArquivo.Text;
                Arquivo.sObservacao = "";
                Arquivo.idUsuario = Convert.ToInt32(IDENTITY.Variaveis.idUsuario());
                Arquivo.vbArquivo = lObjArquivo;

                DataSet dsItem = Arquivo.EnviarArquivo(Arquivo);
                if (BD.ValidarDataSet(dsItem, out sErro))
                {
                    PesquisarTarefas("0", "0", "0", hddidRegistroTarefa.Value, "");
                    MensagemArquivo.MostraMensagem_Sucesso("Arquivo enviado com sucesso!");
                }
            }
        }

        bool ValidarEnvioArquivo()
        {
            bool bRetorno = true;
            if (!fu_Arquivo.HasFile)
            {
                MensagemArquivo.MostraMensagem_Erro("Selecione um arquivo para enviar!");
                fu_Arquivo.Focus();
                return false;
            }
            if (txtEnviarArquivo_sDscArquivo.Text.Length < 10)
            {
                MensagemArquivo.MostraMensagem_Erro("Informe uma Descrição do Arquivo válida!");
                txtEnviarArquivo_sDscArquivo.Focus();
                return false;
            }

            return bRetorno;

        }

        protected void gv_Arquivo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = int.Parse(e.CommandArgument.ToString());
            var gr = gv_Arquivo.Rows[index];
            string idArquivo = gr.Cells[0].Text; //Codigo do arquivo Chamado


            switch (e.CommandName)
            {
                case "Download":

                    Dictionary<String, String> vParametrosItem = new Dictionary<string, string>();
                    DataTable dtArquivo;
                    vParametrosItem = new Dictionary<string, string>
                    {
                        {"@sFuncao",                    "CONSULTAR_DETALHE" },
                        {"@idArquivo",                  idArquivo}
                    };

                    dtArquivo = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Arquivos", vParametrosItem);

                    foreach (DataRow item in dtArquivo.Rows)
                    {

                        FileStream lObjFile;
                        string sNomeArquivo = item["sNomeArquivo"].ToString();
                        TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                        lObjFile = objArquivo.TransformarArrayBytesEmArquivo((byte[])item["vbArquivo"], Server.MapPath("~/Download/" + sNomeArquivo));
                        lObjFile.Close();

                        Response.ContentType = "application/octet-stream";
                        Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}", sNomeArquivo));
                        Response.TransmitFile(Server.MapPath("~/Download/" + sNomeArquivo));
                        Response.End();
                    }
                    break;
            }

        }

        protected void gv_Arquivo_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            GRID.EsconderColunas(e, 0);
        }

        protected void cmdTarefas_Excluir_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gv_Tarefas.Rows)
            {
                if (row.Cells[row.Cells.Count - 1].Text == "Finalizada")
                {
                    row.Cells[0].Controls.Clear();
                }
            }
            hddExcluirTarefas.Value = "S";
            Tarefas_Acao_MostrarCaixa("Excluir Tarefas");
        }

        protected void cmdTarefas_Cancelar_Click(object sender, EventArgs e)
        {
            PesquisarTarefas(hddidPedido.Value, ddlDepartamento.SelectedValue, ddlStatus.SelectedValue, "0", "");
        }

        protected void cmdTarefas_OK_Click(object sender, EventArgs e)
        {
            if (txtTarefas_Observacao.Text.Length < 6)
            {
                MensagemPagina.MostraMensagem_Erro("Insira um Motivo/Observação válido!");
            }
            else
            {
                foreach (GridViewRow item in gv_Tarefas.Rows)
                {
                    string sErro = "";
                    string idRegistroTarefa = item.Cells[1].Text;
                    CheckBox chkTarefas_Seleciona_Linha = (CheckBox)item.FindControl("chkTarefas_Seleciona");
                    bool bSelecionado = chkTarefas_Seleciona_Linha.Checked;

                    if (bSelecionado)
                    {
                        DataSet dsTarefas;
                        Dictionary<String, String> vParametros = new Dictionary<string, string>();
                        if (hddExcluirTarefas.Value == "S")
                            vParametros.Add("@sFuncao", "EXCLUIR_TAREFA");
                        if (hddFinalizarTarefas.Value == "S")
                        {
                            vParametros.Add("@sFuncao", "FINALIZAR_TAREFA");
                            vParametros.Add("@Finalizar", "S");
                        }
                        vParametros.Add("@idRegistroTarefa", idRegistroTarefa);
                        vParametros.Add("@sObservacao", txtTarefas_Observacao.Text);
                        vParametros.Add("@idUsuario", IDENTITY.Variaveis.idUsuario());
                        vParametros.Add("@Pedido", hddidPedido.Value);
                        dsTarefas = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Pedidos_x_Tarefas", vParametros);

                        if (BD.ValidarDataSet(dsTarefas, out sErro))
                        {
                            //MensagemPagina.MostraMensagem_Sucesso(RETORNO.DATASET(dsTarefas, "msg"));
                        }
                        else
                        {
                            if (hddFinalizarTarefas.Value == "S")
                            {
                                MensagemPagina.MostraMensagem_Erro("Não foi possivel iniciar a tarefa, pois existe tarefas anteriores pendentes!");
                                break;
                            }
                            else
                                MensagemPagina.MostraMensagem_Erro(RETORNO.DATASET(dsTarefas, "msg"));
                        }
                    }
                }
                hddExcluirTarefas.Value = "N";
                hddFinalizarTarefas.Value = "N";

                PesquisarTarefas(hddidPedido.Value, ddlDepartamento.SelectedValue, ddlStatus.SelectedValue, "0", "");
            }
        }

        protected void lnkIniciarTarefa_Click(object sender, EventArgs e)
        {
            ModificarStatusTarefa(hddidRegistroTarefa.Value, "INICIAR_TAREFA");
        }

        protected void lnkIniciarTarefaSemAlterarUsuario_Click(object sender, EventArgs e)
        {
            ModificarStatusTarefa(hddidRegistroTarefa.Value, "INICIAR_TAREFA_SEM_ALTERAR");
        }

        protected void chkTarefas_Seleciona_Todos_CheckedChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Tarfas_Acao_Hide", "$('#div_Tarefas_Acoes').hide();", true);
            CheckBox chkTodos = (CheckBox)gv_Tarefas.HeaderRow.FindControl("chkTarefas_Seleciona_Todos");
            foreach (GridViewRow row in gv_Tarefas.Rows)
            {
                if (row.Cells[row.Cells.Count - 1].Text != "Finalizada" && row.Cells[row.Cells.Count - 1].Text != "Iniciado")
                {
                    CheckBox chkIndividual = (CheckBox)row.FindControl("chkTarefas_Seleciona");
                    chkIndividual.Checked = chkTodos.Checked;
                }
                else
                {
                    row.Cells[0].Controls.Clear();
                }
            }
        }

        protected void cmdTarefas_Finalizar_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gv_Tarefas.Rows)
            {
                if (row.Cells[row.Cells.Count - 1].Text == "Finalizada")
                {
                    row.Cells[0].Controls.Clear();
                }
            }
            hddFinalizarTarefas.Value = "S";
            Tarefas_Acao_MostrarCaixa("Finalizar Tarefas");
        }

        protected void cmdProximaTarefa_Click(object sender, EventArgs e)
        {
            string proximoId = "";
            string idAtual = hddidRegistroTarefas.Value;
            int indiceAtual = TodosidRegistro.IndexOf(idAtual);

            if (indiceAtual >= 0 && indiceAtual < TodosidRegistro.Count - 1)
            {
                proximoId = TodosidRegistro[indiceAtual + 1];
            }

            int idUltimaTarefa = Convert.ToInt32(MaximoidRegistro);
            if (Convert.ToInt32(proximoId) <= idUltimaTarefa)
                PesquisarTarefas(hddidPedido.Value, ddlDepartamento.SelectedValue, ddlStatus.SelectedValue, proximoId.ToString(), "");
        }

        void RegistraScript()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("$v192(function() {");
            sb.Append("$v192(\"#dialog_OK\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons: {");
            sb.Append("\"Sim\": function() {");
            sb.Append("$v192('[id*=hddAlterarData]').val('S');");
            sb.Append("__doPostBack(\"dialog_OK\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$v192('[id*=hddAlterarData]').val('N');");
            sb.Append("__doPostBack(\"dialog_OK\", \"\");");
            sb.Append("$v192(this).dialog(\"close\");");
            sb.Append("}");
            sb.Append("}");
            sb.Append("});");

            sb.Append("$v192('[id*=cmdAcao_Confirmar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog_OK').dialog('open');");
            sb.Append("});");
            sb.Append("});");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);
        }
    }
}