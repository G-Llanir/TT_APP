using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using GRID = TT.FrameWork.Grid;
using TT_Flow.FrameWork;
using TT_Flow.App.Controles;

namespace TT_Flow.App.Paginas.Manutencao
{
    public partial class Fluxo_Detalhe : Page
    {
        #region | Construtores

        string sTituloPagina = "Fluxo";
        string sProcedure = "sp_Manipula_tbl_Flow_Fluxo";

        public List<cls_Fluxo_x_Recursos> bs_Fluxo_x_Recursos
        {
            get
            {
                if (ViewState["bs_Fluxo_x_Recursos"] == null)
                {
                    ViewState["bs_Fluxo_x_Recursos"] = new List<cls_Fluxo_x_Recursos>();
                }
                return (List<cls_Fluxo_x_Recursos>)ViewState["bs_Fluxo_x_Recursos"];
            }
            set
            {
                ViewState["bs_Fluxo_x_Recursos"] = value;
            }
        }

        public List<cls_Fluxo_x_Departamentos> bs_Fluxo_x_Departamentos
        {
            get
            {
                if (ViewState["bs_Fluxo_x_Departamentos"] == null)
                {
                    ViewState["bs_Fluxo_x_Departamentos"] = new List<cls_Fluxo_x_Departamentos>();
                }
                return (List<cls_Fluxo_x_Departamentos>)ViewState["bs_Fluxo_x_Departamentos"];
            }
            set
            {
                ViewState["bs_Fluxo_x_Departamentos"] = value;
            }
        }

        public List<cls_Fluxo_x_Tarefas> bs_Fluxo_x_Tarefas
        {
            get
            {
                if (ViewState["bs_Fluxo_x_Tarefas"] == null)
                {
                    ViewState["bs_Fluxo_x_Tarefas"] = new List<cls_Fluxo_x_Tarefas>();
                }
                return (List<cls_Fluxo_x_Tarefas>)ViewState["bs_Fluxo_x_Tarefas"];
            }
            set
            {
                ViewState["bs_Fluxo_x_Tarefas"] = value;
            }
        }

        public List<cls_Tarefas> bs_Tarefas
        {
            get
            {
                if (ViewState["bs_Tarefas"] == null)
                {
                    ViewState["bs_Tarefas"] = new List<cls_Tarefas>();
                }
                return (List<cls_Tarefas>)ViewState["bs_Tarefas"];
            }
            set
            {
                ViewState["bs_Tarefas"] = value;
            }
        }

        public List<cls_Recursos> bs_Recursos
        {
            get
            {
                if (ViewState["bs_Recursos"] == null)
                {
                    ViewState["bs_Recursos"] = new List<cls_Recursos>();
                }
                return (List<cls_Recursos>)ViewState["bs_Recursos"];
            }
            set
            {
                ViewState["bs_Recursos"] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            object objSender = new object();
            EventArgs objEventArgs = new EventArgs();

            manual.sNomeArquivo = "Manual_Fluxos.pdf";

            if (!IsPostBack)
            {
                CarregaCombos();
                ddlidTipo_SelectedIndexChanged(objSender, objEventArgs);                
                cbAdicionaStatus_CheckedChanged(objSender, objEventArgs);

                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.Fluxo.Consultar, true);
                    Pesquisar(Request["id"].ToString());
                    
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.Fluxo.Incluir, true);
                    Pesquisar("0");
                    cbAdicionaStatus.Visible = false;
                }
            }
        }

        protected void CarregaCombos()
        {
            FUNCOES.Popula_Combo(ddlDepartamentos_Departamento, "sp_Select 'Flow_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Selecione um Departamento", "0");
            FUNCOES.Popula_Combo(ddlTarefas_Departamento, "sp_Select 'Flow_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Selecione um Departamento", "0");
            FUNCOES.Popula_Combo(ddlRecursos_idTipoRecurso, "sp_Select 'Flow_Recursos_Tipo'", "idTipoRecurso", "sDscTipoRecurso", false, "Selecione o Tipo", "0");
            FUNCOES.Popula_Combo(ddlidTipo, "sp_Select 'tbl_Flow_Pedidos_Tipo'", "idTipo", "sDscTipo", false, "Selecione o Tipo do Fluxo", "0");

            try
            {
                SqlDataReader dr = BD.ExecutarDataReader("sp_Select 'Flow_Tarefas'");

                if (dr != null)
                {
                    bs_Tarefas.Clear();

                    while (dr.Read())
                    {
                        int idTarefa = Convert.ToInt32(dr["idTarefa"].ToString());

                        cls_Tarefas objItem = new cls_Tarefas
                        {
                            idTarefa = idTarefa,
                            sDscTarefa = dr["sDscTarefa"].ToString(),
                            idDepartamento = Convert.ToInt32(dr["idDepartamento"].ToString()),
                            sDscDepartamento = dr["sDscDepartamento"].ToString(),
                            nTempo = Convert.ToInt32(dr["nTempo"].ToString()),
                            sTipoTempo = dr["sTipoTempo"].ToString(),
                            sObrigatorioConclusao = dr["sObrigatorioConclusao"].ToString()
                        };

                        bs_Tarefas.Add(objItem);

                    }

                    dr.Close();
                }

                dr = BD.ExecutarDataReader("sp_Select 'Flow_Recursos'");

                if (dr != null)
                {
                    bs_Recursos.Clear();

                    while (dr.Read())
                    {
                        int idRecurso = Convert.ToInt32(dr["idRecurso"].ToString());

                        cls_Recursos objItem = new cls_Recursos
                        {
                            idRecurso = idRecurso,
                            sDscRecurso = dr["sDscRecurso"].ToString(),
                            idTipoRecurso = Convert.ToInt32(dr["idTipoRecurso"].ToString()),
                            sDscTipoRecurso = dr["sDscTipoRecurso"].ToString(),
                            sUnidade = dr["sUnidade"].ToString()
                        };

                        bs_Recursos.Add(objItem);
                    }

                    dr.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected void Pesquisar(string idFluxo)
        {
            try
            {
                LimpaCampos();

                if (idFluxo != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idFluxo", idFluxo }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        hddidFluxo.Value = RETORNO.DATASET(dsPesquisa, 0, "idFluxo");
                        txtidFluxo.Text = RETORNO.DATASET(dsPesquisa, 0, "idFluxo");
                        txtsDscFluxo.Text = RETORNO.DATASET(dsPesquisa, 0, "sDscFluxo");
                        ddlGeraOPI.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "sGeraOPI");
                        ddlidTipo.SelectedValue = RETORNO.DATASET(dsPesquisa, 0, "idTipo");
                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));
                        ComboAtivo.Definir(RETORNO.DATASET(dsPesquisa, 0, "sSituacao"), "Ativo", "N");
                        SwitchGastos.Definir(RETORNO.DATASET(dsPesquisa, 0, "sGastos"), "Relatório de Gastos", "N");
                        lblTituloPagina.Text = string.Format("Editar {0} {1}", sTituloPagina, txtsDscFluxo.Text);

                        gvDepartamentos_Popular(dsPesquisa);
                        gvTarefas_Popular(dsPesquisa);
                        gvRecursos_Popular(dsPesquisa);

                        cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Fluxo.Alterar);
                        cbAdicionaStatus.Visible = true;

                        if (bs_Fluxo_x_Departamentos[bs_Fluxo_x_Departamentos.Count - 1].nOrdem == 999)
                            cbAdicionaStatus.Checked = true;
                    }
                    else
                        MensagemPagina.MostraMensagem_Erro(sErro);
                }
                else
                {
                    BreadCrumb.TitulodaPagina = "Incluir";
                    lblTituloPagina.Text = string.Format("Novo {0}", sTituloPagina);
                    txtidFluxo.Text = "Novo";
                    cmdSalvar.Text = "Incluir";
                }

                txtsDscFluxo.Focus();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        void LimpaCampos()
        {
            txtsDscFluxo.Text = "";
            hddidFluxo.Value = "0";
            PainelAtualizacao.Visible = false;
            cmdSalvar.Text = "Salvar";
            lblTituloPagina.Text = sTituloPagina;
            bs_Fluxo_x_Departamentos.Clear();
            ddlDepartamentos_Departamento.SelectedValue = "0";
            txtidFluxo.Text = "Novo";
            ddlidTipo.SelectedValue = "0";
            SwitchGastos.Definir("N", "Relatório de Gastos", "");
            ComboAtivo.Definir("S", "Ativo", "");
        }

        private bool ValidarDados()
        {
            if (txtsDscFluxo.Text.Length < 3)
            {
                MensagemPagina.MostraMensagem_Erro("Informe um nome válido para o Fluxo!");
                return false;
            }

            if (ddlidTipo.SelectedValue == "0")
            {
                MensagemPagina.MostraMensagem_Erro("Selecione o Tipo do Fluxo!");
                return false;
            }

            if (gvDepartamentos.Rows.Count == 0)
            {
                MensagemPagina.MostraMensagem_Erro("Favor selecionar Departamento para este fluxo!");
                return false;
            }
            else
            {
                foreach (var linha in bs_Fluxo_x_Departamentos)
                {
                    if (linha.idStatus == 0)
                    {
                        MensagemPagina.MostraMensagem_Erro(string.Format("Escolha um Status para o Departamento {0}!", linha.sDscDepartamento));
                        return false;
                    }

                    if (linha.idStatusKanban == 0)
                    {
                        MensagemPagina.MostraMensagem_Erro(string.Format("Escolha um Status para o Kanban {0}!", linha.sDscDepartamento));
                        return false;
                    }

                    if (linha.nTempo == 0)
                    {
                        MensagemPagina.MostraMensagem_Erro(string.Format("Tempo inválido para o Departamento {0}!", linha.sDscDepartamento));
                        return false;
                    }

                    if (linha.sTipoTempo == "")
                    {
                        MensagemPagina.MostraMensagem_Erro(string.Format("Tipo do Tempo inválido para o Departamento {0}!", linha.sDscDepartamento));
                        return false;
                    }
                }
            }

            var ultimo = bs_Fluxo_x_Tarefas.OrderBy(t => t.nOrdem).LastOrDefault();
            if (bs_Fluxo_x_Tarefas.Count > 0 && (bs_Fluxo_x_Tarefas.OrderBy(t => t.nOrdem).Where(t => t.nDegrau.Equals(5)).Count() > 1 || ultimo.nDegrau != 5))
            {
                MensagemPagina.MostraMensagem_Erro("É necessário que apenas a última Tarefa do Fluxo, seguindo a Ordem, seja do Degrau 'Finalizador'!");
                return false;
            }

            return true;
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            gvTarefas_SalvarGRID();

            if (ValidarDados())
            {
                try
                {
                    string[] vidFluxo = hddidFluxo.Value.Split(',');
                    string idFluxo = vidFluxo[0].ToString();

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idFluxo", idFluxo },
                        { "@sDscFluxo", txtsDscFluxo.Text },
                        { "@idTipo", ddlidTipo.SelectedValue },
                        { "@sGeraOPI", ddlGeraOPI.SelectedValue },
                        { "@sSituacao", ComboAtivo.Recuperar() },
                        { "@sGastos", SwitchGastos.Recuperar() },
                        { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                    };
                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out string sErro))
                    {
                        idFluxo = RETORNO.DATASET(dsSalvar, 0, "idFluxo");

                        if (Departamentos_Salvar(idFluxo))
                        {
                            if (Tarefas_Salvar(idFluxo))
                            {
                                if (Recursos_Salvar(idFluxo))
                                {
                                    Pesquisar(idFluxo);
                                    MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso");
                                }
                            }
                        }
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

        protected void ddlidTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlidTipo.SelectedItem.Text == "CRM")
                div_ddlGeraOPI.Visible = false;
            else
            {
                div_ddlGeraOPI.Visible = true;
                cbAdicionaStatus.Checked = false;
                cbAdicionaStatus.Visible = false;
                bs_Fluxo_x_Departamentos.Clear();
                gvDepartamentos_DataBind();
            }

            if (bs_Fluxo_x_Departamentos.Count >= 1)
            {
                cbAdicionaStatus.Checked = false;
                cbAdicionaStatus.Visible = false;
                bs_Fluxo_x_Departamentos.Clear();
                gvDepartamentos_DataBind();
                
            }
        }

        #region | Departamentos

        private bool Departamentos_ValidarDados(ref string sMensagem)
        {
            if (ddlDepartamentos_Departamento.SelectedValue == "0")
            {
                sMensagem = "Selecione um Departamento!";
                return false;
            }
            if (ddlidTipo.SelectedValue == "0")
            {
                sMensagem = "Selecione o Tipo do Fluxo!";
                return false;
            }

            return true;
        }

        bool Departamentos_Salvar(string idFluxo)
        {
            bool bRetorno = false;

            try
            {
                Dictionary<string, string> vParametroSelecao_Excluir = new Dictionary<string, string>
                {
                    { "@sFuncao", "DELETE_DEPARTAMENTOS" },
                    { "@idFluxo", idFluxo }
                };
                DataSet dsSelecao_Excluir = BD.ExecutarDataSet(sProcedure, vParametroSelecao_Excluir);

                foreach (var linha in bs_Fluxo_x_Departamentos)
                {
                    if (idFluxo != "0")
                    {
                        Dictionary<string, string> vParametroSelecao_Incluir = new Dictionary<string, string>
                        {
                            { "@sFuncao", "SALVAR_DEPARTAMENTOS" },
                            { "@idFluxo", idFluxo },
                            { "@idDepartamento", linha.idDepartamento.ToString() },
                            { "@nOrdem", linha.nOrdem.ToString() },
                            { "@idStatus", linha.idStatus.ToString() },
                            { "@idStatusKanban", linha.idStatusKanban.ToString() },
                            { "@nTempo", linha.nTempo.ToString() },
                            { "@sTipoTempo", linha.sTipoTempo.ToString() },
                            { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                        };
                        DataSet dsSelecao_Incluir = BD.ExecutarDataSet(sProcedure, vParametroSelecao_Incluir);
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

        protected void gvDepartamentos_Popular(DataSet ds)
        {
            foreach (DataRow row in ds.Tables[1].Rows)
            {
                cls_Fluxo_x_Departamentos objItem = new cls_Fluxo_x_Departamentos
                {
                    idRegistro = Convert.ToInt32(row["idRegistro"].ToString()),
                    idFluxo = Convert.ToInt32(row["idFluxo"].ToString()),
                    idDepartamento = Convert.ToInt32(row["idDepartamento"].ToString()),
                    sDscDepartamento = row["sDscDepartamento"].ToString(),
                    nOrdem = Convert.ToInt32(row["nOrdem"].ToString()),
                    idStatus = Convert.ToInt32(row["idStatus"].ToString()),
                    idStatusKanban = Convert.ToInt32(row["idStatusKanban"].ToString()),
                    sDscStatus = row["sDscStatus"].ToString(),
                    nTempo = Convert.ToInt32(row["nTempo"].ToString()),
                    sTipoTempo = row["sTipoTempo"].ToString()
                };

                bs_Fluxo_x_Departamentos.Add(objItem);
            }

            gvDepartamentos_DataBind();
        }

        protected void gvDepartamentos_DataBind()
        {
            gvDepartamentos.DataSource = bs_Fluxo_x_Departamentos;
            gvDepartamentos.DataBind();
        }

        protected void gvDepartamentos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0, 1);

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (ddlidTipo.SelectedItem.Text == "CRM")
                {
                    //IdStatus
                    DropDownList ddlStatus = (e.Row.FindControl("ddlDepartamentos_StatusInterno") as DropDownList);
                    FUNCOES.Popula_Combo(ddlStatus, "sp_Select 'Flow_Status', @sTipo='CRM'", "idStatus", "sDscStatus", false, "Selecione um Status", "0");
                    ddlStatus.SelectedValue = bs_Fluxo_x_Departamentos[e.Row.RowIndex].idStatus.ToString();

                    //Status Kanban
                    DropDownList ddlStatusKanban = (e.Row.FindControl("ddlDepartamentos_StatusKanban") as DropDownList);
                    FUNCOES.Popula_Combo(ddlStatusKanban, "sp_Select 'Flow_Status', @sTipo='CRM'", "idStatus", "sDscStatus", false, "Selecione um Status", "0");
                    ddlStatusKanban.SelectedValue = bs_Fluxo_x_Departamentos[e.Row.RowIndex].idStatusKanban.ToString();
                }
                else
                {
                    //IdStatus
                    DropDownList ddlStatus = (e.Row.FindControl("ddlDepartamentos_StatusInterno") as DropDownList);
                    FUNCOES.Popula_Combo(ddlStatus, "sp_Select 'Flow_Status'", "idStatus", "sDscStatus", false, "Selecione um Status", "0");
                    ddlStatus.SelectedValue = bs_Fluxo_x_Departamentos[e.Row.RowIndex].idStatus.ToString();

                    //Status Kanban
                    DropDownList ddlStatusKanban = (e.Row.FindControl("ddlDepartamentos_StatusKanban") as DropDownList);
                    FUNCOES.Popula_Combo(ddlStatusKanban, "sp_Select 'Flow_Status'", "idStatus", "sDscStatus", false, "Selecione um Status", "0");
                    ddlStatusKanban.SelectedValue = bs_Fluxo_x_Departamentos[e.Row.RowIndex].idStatusKanban.ToString();
                }

                //Tipo Tempo
                DropDownList ddlTipoTempo_Linha = (e.Row.FindControl("ddlDepartamentos_TipoTempo") as DropDownList);
                ddlTipoTempo_Linha.SelectedValue = bs_Fluxo_x_Departamentos[e.Row.RowIndex].sTipoTempo.ToString();

                cls_Fluxo_x_Departamentos linha = (cls_Fluxo_x_Departamentos)e.Row.DataItem;

                if (linha.nOrdem == 998)
                {
                    TextBox nOrdem = (TextBox)e.Row.FindControl("txtDepartamentos_nOrdem");
                    DropDownList ddlStatusInterno = (DropDownList)e.Row.FindControl("ddlDepartamentos_StatusInterno");                    
                    DropDownList ddlStatusKanban = (DropDownList)e.Row.FindControl("ddlDepartamentos_StatusKanban");                    
                    LinkButton lnkExcluir = (LinkButton)e.Row.FindControl("lnkDepartamentos_Excluir");
                    ListItem itemStatusInterno = ddlStatusInterno.Items.FindByText("Finalizado");
                    ListItem itemStatusKanban = ddlStatusKanban.Items.FindByText("Finalizado");

                    ddlStatusInterno.SelectedValue = itemStatusInterno.Value;
                    ddlStatusKanban.SelectedValue = itemStatusKanban.Value;

                    nOrdem.ReadOnly = true;
                    ddlStatusInterno.Attributes.Add("disabled", "disabled");
                    ddlStatusKanban.Attributes.Add("disabled", "disabled");
                    lnkExcluir.Visible = false;
                }

                if(linha.nOrdem == 999)
                {
                    TextBox nOrdem = (TextBox)e.Row.FindControl("txtDepartamentos_nOrdem");
                    DropDownList ddlStatusInterno = (DropDownList)e.Row.FindControl("ddlDepartamentos_StatusInterno");
                    DropDownList ddlStatusKanban = (DropDownList)e.Row.FindControl("ddlDepartamentos_StatusKanban");
                    LinkButton lnkExcluir = (LinkButton)e.Row.FindControl("lnkDepartamentos_Excluir");
                    ListItem itemStatusInterno = ddlStatusInterno.Items.FindByText("Cancelado");
                    ListItem itemStatusKanban = ddlStatusKanban.Items.FindByText("Cancelado");

                    ddlStatusInterno.SelectedValue = itemStatusInterno.Value;
                    ddlStatusKanban.SelectedValue = itemStatusKanban.Value;

                    nOrdem.ReadOnly = true;
                    ddlStatusInterno.Attributes.Add("disabled", "disabled");
                    ddlStatusKanban.Attributes.Add("disabled", "disabled");
                    lnkExcluir.Visible = false;
                }
            }
        }

        protected void gvDepartamentos_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            bs_Fluxo_x_Departamentos.RemoveAt(e.RowIndex);
            gvDepartamentos_DataBind();

            if (bs_Fluxo_x_Departamentos.Count == 0)
                cbAdicionaStatus.Visible = false;

            if (bs_Fluxo_x_Departamentos.Count == 2 && cbAdicionaStatus.Checked)
            {
                cbAdicionaStatus.Checked = false;
                cbAdicionaStatus_CheckedChanged(new object(), new EventArgs());
                cbAdicionaStatus.Visible = false;
            }
        }

        protected void cmdDepartamentos_Incluir_Click(object sender, EventArgs e)
        {
            string sMensagem = "";           
           
            if (Departamentos_ValidarDados(ref sMensagem))
            {
                if (bs_Fluxo_x_Departamentos.Count == 0)
                {
                    cls_Fluxo_x_Departamentos objItem = new cls_Fluxo_x_Departamentos
                    {
                        idDepartamento = Convert.ToInt32(ddlDepartamentos_Departamento.SelectedValue),
                        sDscDepartamento = ddlDepartamentos_Departamento.SelectedItem.ToString(),
                        nOrdem = bs_Fluxo_x_Departamentos.Count + 10,
                        sTipoTempo = ""
                    };

                    bs_Fluxo_x_Departamentos.Add(objItem);
                    gvDepartamentos_DataBind();
                    ddlDepartamentos_Departamento.SelectedValue = "0";
                }
                else
                {
                    int maxOrdem = bs_Fluxo_x_Departamentos.Where(d => d.nOrdem != 998 && d.nOrdem != 999).Max(d => d.nOrdem);

                    cls_Fluxo_x_Departamentos objItem = new cls_Fluxo_x_Departamentos
                    {
                        idDepartamento = Convert.ToInt32(ddlDepartamentos_Departamento.SelectedValue),
                        sDscDepartamento = ddlDepartamentos_Departamento.SelectedItem.ToString(),
                        nOrdem = maxOrdem + 10,
                        sTipoTempo = ""
                    };

                    int indexLinha = bs_Fluxo_x_Departamentos.FindIndex(d => d.nOrdem == 998 || d.nOrdem == 999);

                    if (indexLinha == -1)
                        bs_Fluxo_x_Departamentos.Add(objItem);
                    else
                        bs_Fluxo_x_Departamentos.Insert(indexLinha, objItem);

                    gvDepartamentos_DataBind();

                    ddlDepartamentos_Departamento.SelectedValue = "0";
                }
            }
            else
                MensagemAcoes.MostraMensagem_Erro(sMensagem);

            if (bs_Fluxo_x_Departamentos.Count >= 1)
                cbAdicionaStatus.Visible = true;
        }

        protected void txtDepartamentos_nOrdem_TextChanged(object sender, EventArgs e)
        {
            int nContador = 0;

            foreach (GridViewRow item in gvDepartamentos.Rows)
            {
                TextBox txtnOrdem_Linha = (TextBox)item.FindControl("txtDepartamentos_nOrdem");
                bs_Fluxo_x_Departamentos[nContador].nOrdem = Convert.ToInt32(txtnOrdem_Linha.Text);
                nContador++;
            }
        }

        protected void txtDepartamentos_nTempo_TextChanged(object sender, EventArgs e)
        {
            int nContador = 0;

            foreach (GridViewRow item in gvDepartamentos.Rows)
            {
                TextBox txtnTempo_Linha = (TextBox)item.FindControl("txtDepartamentos_nTempo");
                bs_Fluxo_x_Departamentos[nContador].nTempo = Convert.ToInt32(txtnTempo_Linha.Text);
                nContador++;
            }
        }

        protected void ddlDepartamentos_StatusInterno_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nContador = 0;

            foreach (GridViewRow item in gvDepartamentos.Rows)
            {
                DropDownList ddlStatus_Linha = (DropDownList)item.FindControl("ddlDepartamentos_StatusInterno");
                bs_Fluxo_x_Departamentos[nContador].sDscStatus = ddlStatus_Linha.SelectedItem.ToString();
                bs_Fluxo_x_Departamentos[nContador].idStatus = Convert.ToInt32(ddlStatus_Linha.SelectedValue);
                nContador++;
            }
        }

        protected void ddlDepartamentos_StatusKanban_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nContador = 0;

            foreach (GridViewRow item in gvDepartamentos.Rows)
            {
                DropDownList ddlStatus_Linha = (DropDownList)item.FindControl("ddlDepartamentos_StatusKanban");
                bs_Fluxo_x_Departamentos[nContador].sDscStatus = ddlStatus_Linha.SelectedItem.ToString();
                bs_Fluxo_x_Departamentos[nContador].idStatusKanban = Convert.ToInt32(ddlStatus_Linha.SelectedValue);
                nContador++;
            }
        }

        protected void ddlDepartamentos_TipoTempo_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nContador = 0;

            foreach (GridViewRow item in gvDepartamentos.Rows)
            {
                DropDownList ddlTipoTempo_Linha = (DropDownList)item.FindControl("ddlDepartamentos_TipoTempo");
                bs_Fluxo_x_Departamentos[nContador].sTipoTempo = ddlTipoTempo_Linha.SelectedValue;
                nContador++;
            }
        }

        #endregion

        #region | Grid Tarefas

        bool Tarefas_Salvar(string idFluxo)
        {
            bool bRetorno = false;

            try
            {
                Dictionary<string, string> vTarefas_Parametros_Excluir = new Dictionary<string, string>
                {
                    { "@sFuncao", "DELETE_TAREFAS" },
                    { "@idFluxo", idFluxo }
                };
                DataSet dsTarefas_Excluir = BD.ExecutarDataSet(sProcedure, vTarefas_Parametros_Excluir);

                foreach (var linha in bs_Fluxo_x_Tarefas)
                {
                    if (idFluxo != "0")
                    {
                        Dictionary<string, string> vTarefas_Parametros_Incluir = new Dictionary<string, string>
                        {
                            { "@sFuncao", "SALVAR_TAREFAS" },
                            { "@idFluxo", idFluxo },
                            { "@idTarefa", linha.idTarefa.ToString() },
                            { "@nOrdem", linha.nOrdem.ToString() },
                            { "@nTempo", linha.nTempo.ToString() },
                            { "@sTipoTempo", linha.sTipoTempo.ToString() },
                            { "@nDegrau", linha.nDegrau.ToString() },
                            { "@sObrigatorioConclusao", linha.sObrigatorioConclusao.ToString() },
                            { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                        };
                        DataSet dsTarefas_Incluir = BD.ExecutarDataSet(sProcedure, vTarefas_Parametros_Incluir);
                    }
                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao Gravar Tarefas: " + ex.Message);
            }

            return bRetorno;
        }

        void gvTarefas_Popular(DataSet ds)
        {
            bs_Fluxo_x_Tarefas.Clear();

            foreach (DataRow row in ds.Tables[2].Rows)
            {
                cls_Fluxo_x_Tarefas objItem = new cls_Fluxo_x_Tarefas
                {
                    idRegistro = Convert.ToInt32(row["idRegistro"].ToString()),
                    idFluxo = Convert.ToInt32(row["idFluxo"].ToString()),
                    idDepartamento = Convert.ToInt32(row["idDepartamento"].ToString()),
                    sDscDepartamento = row["sDscDepartamento"].ToString(),
                    nOrdem = Convert.ToInt32(row["nOrdem"].ToString()),
                    idTarefa = Convert.ToInt32(row["idTarefa"].ToString()),
                    sDscTarefa = row["sDscTarefa"].ToString(),
                    nTempo = Convert.ToInt32(row["nTempo"].ToString()),
                    sTipoTempo = row["sTipoTempo"].ToString(),
                    nDegrau = Convert.ToInt32(row["nDegrau"].ToString()),
                    sObrigatorioConclusao = row["sObrigatorioConclusao"].ToString()
                };

                bs_Fluxo_x_Tarefas.Add(objItem);
            }

            gvTarefas_DataBind();
        }

        void gvTarefas_DataBind()
        {
            gvTarefas.DataSource = bs_Fluxo_x_Tarefas;
            gvTarefas.DataBind();
        }

        void gvTarefas_SalvarGRID()
        {
            int nContador = 0;

            foreach (GridViewRow item in gvTarefas.Rows)
            {
                TextBox txtnOrdem_Linha = (TextBox)item.FindControl("txtTarefas_nOrdem");
                bs_Fluxo_x_Tarefas[nContador].nOrdem = Convert.ToInt32(txtnOrdem_Linha.Text);

                TextBox txtnTempo_Linha = (TextBox)item.FindControl("txtTarefas_nTempo");
                bs_Fluxo_x_Tarefas[nContador].nTempo = Convert.ToInt32(txtnTempo_Linha.Text);

                DropDownList ddlTipoTempo_Linha = (DropDownList)item.FindControl("ddlTarefas_TipoTempo");
                bs_Fluxo_x_Tarefas[nContador].sTipoTempo = ddlTipoTempo_Linha.SelectedValue;

                DropDownList ddlTarefas_nDegrau_Linha = (DropDownList)item.FindControl("ddlTarefas_nDegrau");
                bs_Fluxo_x_Tarefas[nContador].nDegrau = Convert.ToInt32(ddlTarefas_nDegrau_Linha.SelectedValue);

                DropDownList ddlTarefas_sObrigatorioConclusao_Linha = (DropDownList)item.FindControl("ddlTarefas_sObrigatorioConclusao");
                bs_Fluxo_x_Tarefas[nContador].sObrigatorioConclusao = ddlTarefas_sObrigatorioConclusao_Linha.SelectedValue;

                nContador++;
            }
        }

        protected void gvTarefas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0, 1);

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlTarefas_TipoTempo = (e.Row.FindControl("ddlTarefas_TipoTempo") as DropDownList);
                ddlTarefas_TipoTempo.SelectedValue = bs_Fluxo_x_Tarefas[e.Row.RowIndex].sTipoTempo.ToString();

                DropDownList ddlTarefas_nDegrau = (e.Row.FindControl("ddlTarefas_nDegrau") as DropDownList);
                ddlTarefas_nDegrau.SelectedValue = bs_Fluxo_x_Tarefas[e.Row.RowIndex].nDegrau.ToString();

                DropDownList ddlTarefas_sObrigatorioConclusao = (e.Row.FindControl("ddlTarefas_sObrigatorioConclusao") as DropDownList);
                ddlTarefas_sObrigatorioConclusao.SelectedValue = bs_Fluxo_x_Tarefas[e.Row.RowIndex].sObrigatorioConclusao.ToString();
            }
        }

        protected void gvTarefas_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            gvTarefas_SalvarGRID();

            bs_Fluxo_x_Tarefas.RemoveAt(e.RowIndex);

            gvTarefas_DataBind();

            if (ddlTarefas_Departamento.SelectedValue != "0")
                ddlTarefas_Tarefa_Popular(ddlTarefas_Departamento.SelectedValue);
        }

        protected void cmdTarefas_Incluir_Click(object sender, EventArgs e)
        {
            var Localiza_Tarefas_X_Departamentos = bs_Tarefas.Where(c => c.idDepartamento.ToString().Equals(ddlTarefas_Departamento.SelectedValue)).Select(x => new { x.idTarefa, x.sDscTarefa, x.sDscDepartamento, x.idDepartamento, x.nTempo, x.sTipoTempo }).ToList();

            if (Localiza_Tarefas_X_Departamentos.Count > 0)
            {
                if (ddlTarefas_Tarefa.SelectedValue != "0")
                    Localiza_Tarefas_X_Departamentos = Localiza_Tarefas_X_Departamentos.Where(c => c.idTarefa.ToString().Equals(ddlTarefas_Tarefa.SelectedValue)).Select(x => new { x.idTarefa, x.sDscTarefa, x.sDscDepartamento, x.idDepartamento, x.nTempo, x.sTipoTempo }).ToList();

                gvTarefas_SalvarGRID();

                for (int i = 0; i < Localiza_Tarefas_X_Departamentos.Count; i++)
                {
                    var Localiza_Tarefa = bs_Fluxo_x_Tarefas.Where(c => c.idTarefa.Equals(Localiza_Tarefas_X_Departamentos[i].idTarefa)).Select(x => new { x.idTarefa, x.sDscTarefa }).ToList();

                    if (Localiza_Tarefa.Count == 0)
                    {
                        cls_Fluxo_x_Tarefas objItem = new cls_Fluxo_x_Tarefas
                        {
                            idRegistro = 0,
                            idFluxo = 0,
                            idDepartamento = Localiza_Tarefas_X_Departamentos[i].idDepartamento,
                            sDscDepartamento = Localiza_Tarefas_X_Departamentos[i].sDscDepartamento,
                            nOrdem = bs_Fluxo_x_Tarefas.Count + 1,
                            idTarefa = Localiza_Tarefas_X_Departamentos[i].idTarefa,
                            sDscTarefa = Localiza_Tarefas_X_Departamentos[i].sDscTarefa,
                            nTempo = Localiza_Tarefas_X_Departamentos[i].nTempo,
                            sTipoTempo = Localiza_Tarefas_X_Departamentos[i].sTipoTempo,
                            sObrigatorioConclusao = "S"
                        };

                        bs_Fluxo_x_Tarefas.Add(objItem);
                    }
                }

            }

            gvTarefas_DataBind();

            if (ddlTarefas_Departamento.SelectedValue != "0")
                ddlTarefas_Tarefa_Popular(ddlTarefas_Departamento.SelectedValue);
        }

        void ddlTarefas_Tarefa_Popular(string idDepartamento)
        {
            ddlTarefas_Tarefa.Items.Clear();
            ddlTarefas_Tarefa.Items.Add(new ListItem("Inserir todas as Tarefas", "0"));
            var Localiza_Tarefas_X_Departamentos = bs_Tarefas.Where(c => c.idDepartamento.ToString().Equals(idDepartamento)).Select(x => new { x.idDepartamento, x.idTarefa, x.sDscDepartamento, x.sDscTarefa }).ToList();

            if (Localiza_Tarefas_X_Departamentos.Count > 0)
            {
                for (int i = 0; i < Localiza_Tarefas_X_Departamentos.Count; i++)
                {
                    var Localiza_Tarefa = bs_Fluxo_x_Tarefas.Where(c => c.idTarefa.Equals(Localiza_Tarefas_X_Departamentos[i].idTarefa)).Select(x => new { x.idTarefa, x.sDscTarefa }).ToList();

                    if (Localiza_Tarefa.Count == 0)
                        ddlTarefas_Tarefa.Items.Add(new ListItem(Localiza_Tarefas_X_Departamentos[i].sDscTarefa, Localiza_Tarefas_X_Departamentos[i].idTarefa.ToString()));
                }
            }

            if (ddlTarefas_Tarefa.Items.Count == 1)
            {
                ddlTarefas_Tarefa.Items.Clear();
                cmdTarefas_Incluir.Enabled = false;
            }
            else
                cmdTarefas_Incluir.Enabled = true;
        }

        protected void ddlTarefas_Departamento_SelectedIndexChanged(object sender, EventArgs e)
        => ddlTarefas_Tarefa_Popular(ddlTarefas_Departamento.SelectedValue);

        #endregion

        #region | Grid Recursos

        bool Recursos_Salvar(string idFluxo)
        {
            bool bRetorno = false;

            try
            {
                gvRecursos_SalvarGRID();

                Dictionary<string, string> vRecursos_Parametros_Excluir = new Dictionary<string, string>
                {
                    { "@sFuncao", "DELETE_RECURSOS" },
                    { "@idFluxo", idFluxo }
                };
                DataSet dsRecursos_Excluir = BD.ExecutarDataSet(sProcedure, vRecursos_Parametros_Excluir);

                foreach (var linha in bs_Fluxo_x_Recursos)
                {
                    if (idFluxo != "0")
                    {
                        double.TryParse(linha.nQuantidade.ToString().Replace("R$", ""), out double nQuantidade);

                        Dictionary<string, string> vRecursos_Parametros_Incluir = new Dictionary<string, string>
                        {
                            { "@sFuncao", "SALVAR_RECURSOS" },
                            { "@idFluxo", idFluxo },
                            { "@idRecurso", linha.idRecurso.ToString() },
                            { "@sUnidade", linha.sUnidade.ToString() },
                            { "@nOrdem", linha.nOrdem.ToString() },
                            { "@nQuantidade", nQuantidade.ToString().Replace(',', '.') },
                            { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                        };
                        BD.ExecutarDataSet(sProcedure, vRecursos_Parametros_Incluir);
                    }
                }

                bRetorno = true;
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao Gravar Recursos: " + ex.Message);
            }

            return bRetorno;
        }

        void gvRecursos_Popular(DataSet ds)
        {
            bs_Fluxo_x_Recursos.Clear();

            foreach (DataRow row in ds.Tables[3].Rows)
            {
                cls_Fluxo_x_Recursos objItem = new cls_Fluxo_x_Recursos
                {
                    idRegistro = Convert.ToInt32(row["idRegistro"].ToString()),
                    idFluxo = Convert.ToInt32(row["idFluxo"].ToString()),
                    idTipoRecurso = Convert.ToInt32(row["idTipoRecurso"].ToString()),
                    sDscTipoRecurso = row["sDscTipoRecurso"].ToString(),
                    idRecurso = Convert.ToInt32(row["idRecurso"].ToString()),
                    sDscRecurso = row["sDscRecurso"].ToString(),
                    nOrdem = Convert.ToInt32(row["nOrdem"].ToString()),
                    nQuantidade = Convert.ToDouble(row["nQuantidade"].ToString()),
                    sUnidade = row["sUnidade"].ToString()
                };

                bs_Fluxo_x_Recursos.Add(objItem);
            }

            gvRecursos_DataBind();
        }

        void gvRecursos_DataBind()
        {
            gvRecursos.DataSource = bs_Fluxo_x_Recursos;
            gvRecursos.DataBind();
        }

        void gvRecursos_SalvarGRID()
        {
            int nContador = 0;

            foreach (GridViewRow item in gvRecursos.Rows)
            {

                TextBox txtnOrdem_Linha = (TextBox)item.FindControl("txtRecursos_nOrdem");
                bs_Fluxo_x_Recursos[nContador].nOrdem = Convert.ToInt32(txtnOrdem_Linha.Text);

                DropDownList ddlsUnidade_Linha = (DropDownList)item.FindControl("ddlRecursos_sUnidade");
                bs_Fluxo_x_Recursos[nContador].sUnidade = ddlsUnidade_Linha.SelectedValue;

                TextBox txtnQuantidade_Linha = (TextBox)item.FindControl("txtRecursos_nQuantidade");
                bs_Fluxo_x_Recursos[nContador].nQuantidade = Convert.ToDouble(txtnQuantidade_Linha.Text);

                nContador++;
            }
        }

        protected void gvRecursos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GRID.EsconderColunas(e, 0, 1);

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlsUnidade_Linha = (DropDownList)e.Row.FindControl("ddlRecursos_sUnidade");
                FUNCOES.Popula_Combo(ddlsUnidade_Linha, "sp_Select 'Flow_Produtos_Unidade'", "sUnidade", "sDscUnidade", false, "Selecione", "0");
                ddlsUnidade_Linha.SelectedValue = bs_Fluxo_x_Recursos[e.Row.RowIndex].sUnidade;

                TextBox txtnQuantidade_Linha = (TextBox)e.Row.FindControl("txtRecursos_nQuantidade");
                txtnQuantidade_Linha.Text = bs_Fluxo_x_Recursos[e.Row.RowIndex].nQuantidade.ToString();

                TextBox txtnOrdem_Linha = (TextBox)e.Row.FindControl("txtRecursos_nOrdem");
                txtnOrdem_Linha.Text = bs_Fluxo_x_Recursos[e.Row.RowIndex].nOrdem.ToString();
            }
        }

        protected void gvRecursos_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            gvRecursos_SalvarGRID();

            bs_Fluxo_x_Recursos.RemoveAt(e.RowIndex);

            gvRecursos_DataBind();

            if (ddlRecursos_idTipoRecurso.SelectedValue != "0")
                ddlRecursos_idRecurso_Popular(ddlRecursos_idTipoRecurso.SelectedValue);
        }

        protected void cmdRecursos_Incluir_Click(object sender, EventArgs e)
        {
            var Localiza_Recursos_X_TipoRecurso = bs_Recursos.Where(c => c.idTipoRecurso.ToString().Equals(ddlRecursos_idTipoRecurso.SelectedValue)).Select(x => new { x.idRecurso, x.sDscRecurso, x.sDscTipoRecurso, x.idTipoRecurso, x.sUnidade, x.nQuantidade, x.nOrdem }).ToList();

            if (Localiza_Recursos_X_TipoRecurso.Count > 0)
            {
                if (ddlRecursos_idRecurso.SelectedValue != "0")
                    Localiza_Recursos_X_TipoRecurso = Localiza_Recursos_X_TipoRecurso.Where(c => c.idRecurso.ToString().Equals(ddlRecursos_idRecurso.SelectedValue)).Select(x => new { x.idRecurso, x.sDscRecurso, x.sDscTipoRecurso, x.idTipoRecurso, x.sUnidade, x.nQuantidade, x.nOrdem }).ToList();

                gvRecursos_SalvarGRID();

                for (int i = 0; i < Localiza_Recursos_X_TipoRecurso.Count; i++)
                {
                    var Localiza_Recurso = bs_Fluxo_x_Recursos.Where(c => c.idRecurso.Equals(Localiza_Recursos_X_TipoRecurso[i].idRecurso)).Select(x => new { x.idRecurso, x.sDscRecurso }).ToList();

                    if (Localiza_Recurso.Count == 0)
                    {
                        cls_Fluxo_x_Recursos objItem = new cls_Fluxo_x_Recursos
                        {
                            idRegistro = 0,
                            idFluxo = 0,
                            idTipoRecurso = Localiza_Recursos_X_TipoRecurso[i].idTipoRecurso,
                            sDscTipoRecurso = Localiza_Recursos_X_TipoRecurso[i].sDscTipoRecurso,
                            idRecurso = Localiza_Recursos_X_TipoRecurso[i].idRecurso,
                            sDscRecurso = Localiza_Recursos_X_TipoRecurso[i].sDscRecurso,
                            nOrdem = bs_Fluxo_x_Recursos.Count + 1,
                            sUnidade = Localiza_Recursos_X_TipoRecurso[i].sUnidade,
                            nQuantidade = Localiza_Recursos_X_TipoRecurso[i].nQuantidade
                        };

                        bs_Fluxo_x_Recursos.Add(objItem);
                    }
                }

            }

            gvRecursos_DataBind();

            if (ddlRecursos_idTipoRecurso.SelectedValue != "0")
                ddlRecursos_idRecurso_Popular(ddlRecursos_idTipoRecurso.SelectedValue);
        }

        void ddlRecursos_idRecurso_Popular(string idTipoRecurso)
        {
            ddlRecursos_idRecurso.Items.Clear();
            ddlRecursos_idRecurso.Items.Add(new ListItem("Inserir todos os Recursos", "0"));
            var Localiza_Recursos_X_TipoRecurso = bs_Recursos.Where(c => c.idTipoRecurso.ToString().Equals(idTipoRecurso)).Select(x => new { x.idRecurso, x.sDscRecurso }).ToList();

            if (Localiza_Recursos_X_TipoRecurso.Count > 0)
            {
                for (int i = 0; i < Localiza_Recursos_X_TipoRecurso.Count; i++)
                {
                    var Localiza_Recurso = bs_Fluxo_x_Recursos.Where(c => c.idRecurso.Equals(Localiza_Recursos_X_TipoRecurso[i].idRecurso)).Select(x => new { x.idRecurso, x.sDscRecurso }).ToList();

                    if (Localiza_Recurso.Count == 0)
                        ddlRecursos_idRecurso.Items.Add(new ListItem(Localiza_Recursos_X_TipoRecurso[i].sDscRecurso, Localiza_Recursos_X_TipoRecurso[i].idRecurso.ToString()));
                }
            }

            cmdRecursos_Incluir.Enabled = true;

            if (ddlRecursos_idRecurso.Items.Count == 1)
            {
                ddlRecursos_idRecurso.Items.Clear();
                cmdRecursos_Incluir.Enabled = false;
            }
        }

        protected void ddlRecursos_idTipoRecurso_SelectedIndexChanged(object sender, EventArgs e)
        => ddlRecursos_idRecurso_Popular(ddlRecursos_idTipoRecurso.SelectedValue);

        #endregion
        
        protected void cbAdicionaStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAdicionaStatus.Checked && bs_Fluxo_x_Departamentos.Count >= 1)
            {
                //Status Finalizado
                cls_Fluxo_x_Departamentos objItem1 = new cls_Fluxo_x_Departamentos
                {
                    nOrdem = 998,
                    sTipoTempo = ""
                };

                bs_Fluxo_x_Departamentos.Add(objItem1);

                //Status Cancelado
                cls_Fluxo_x_Departamentos objItem2 = new cls_Fluxo_x_Departamentos
                {
                    nOrdem = 999,
                    sTipoTempo = ""
                };

                bs_Fluxo_x_Departamentos.Add(objItem2);

                gvDepartamentos_DataBind();
            }
            else if (!cbAdicionaStatus.Checked && bs_Fluxo_x_Departamentos.Count >= 1 && bs_Fluxo_x_Departamentos[bs_Fluxo_x_Departamentos.Count - 1].nOrdem == 999)
            {
                bs_Fluxo_x_Departamentos.Remove(bs_Fluxo_x_Departamentos[bs_Fluxo_x_Departamentos.Count - 1]);
                bs_Fluxo_x_Departamentos.Remove(bs_Fluxo_x_Departamentos[bs_Fluxo_x_Departamentos.Count - 2]);

                gvDepartamentos_DataBind();
            }
            
        }
    }
}