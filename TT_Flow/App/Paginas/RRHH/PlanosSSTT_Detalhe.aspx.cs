using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using TT.FrameWork;
using TT_Flow.FrameWork;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using IDENTITY = TT.FrameWork.Identity;

namespace TT_Hub.App.Paginas.RRHH
{
    public partial class PlanosSSTT_Detalhe : Page
    {
        string sTituloPagina = "Plano SSTT";
        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores_GHE_Plano_SSTT";
        string sCaminho = "App/Paginas/RRHH/";

        private List<string> CoberturasAtuais
        {
            get => ViewState["Coberturas"] as List<string> ?? new List<string>();
            set => ViewState["Coberturas"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // VALIDAÇÃO: Executa a validação de permissões antes de qualquer outra ação
            ValidarPermissoes();

            if (!IsPostBack)
            {
                if (Request.QueryString["msg"] == "sucesso")
                {
                    MensagemPagina.MostraMensagem_Sucesso("Plano incluído com sucesso!");
                }

                if (Request["id"] != null)
                {
                    CarregarDados(Request["id"]);
                }
            }
            else
            {
                var requestTarget = Request["__EVENTTARGET"];
                if (requestTarget == "funcao_SALVAR") Salvar();
                if (requestTarget == "funcao_Excluir") Excluir();
            }

            PainelAtualizacao.Visible = (hddidPlano.Value != "0");
            RegistraScript();
        }

        #region Permissões

        private void ValidarPermissoes()
        {
            FUNCOES.ValidaPermissao(Permissao.RRHH.Planos.Consultar, true);

            bool bNovo = (Request["id"] == null || Request["id"] == "0");

            if (bNovo)
            {
                bool bPodeIncluir = FUNCOES.ValidaPermissao(Permissao.RRHH.Planos.Incluir);
                cmdSalvar.Visible = bPodeIncluir;
                cmdExcluir.Visible = false; // Botão excluir nunca aparece para um novo.
                HabilitarEdicao(bPodeIncluir);
            }
            else
            {
                bool bPodeAlterar = FUNCOES.ValidaPermissao(Permissao.RRHH.Planos.Alterar);
                bool bPodeExcluir = FUNCOES.ValidaPermissao(Permissao.RRHH.Planos.Excluir);

                cmdSalvar.Visible = bPodeAlterar;
                cmdExcluir.Visible = bPodeExcluir;

                HabilitarEdicao(bPodeAlterar);
            }
        }

        private void HabilitarEdicao(bool habilitar)
        {
            txtsDscPlano.ReadOnly = !habilitar;
            txtnValor.ReadOnly = !habilitar;
            txtsObservacao.ReadOnly = !habilitar;

            pnlAdicionarCobertura.Visible = habilitar;
            gvCoberturas.Columns[1].Visible = habilitar; 
        }

        #endregion

        #region Carregamento de Dados

        private void CarregarDados(string idPlano)
        {
            try
            {
                if (idPlano != "0")
                {
                    var vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" }, { "@idPlano", idPlano }
                    };
                    DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(ds, out string sErro))
                    {
                        hddidPlano.Value = RETORNO.DATASET(ds, "idPlano");
                        txtidPlano.Text = RETORNO.DATASET(ds, "idPlano");
                        txtsDscPlano.Text = RETORNO.DATASET(ds, "sDscPlano");
                        txtnValor.Text = Convert.ToDecimal(RETORNO.DATASET(ds, "nValor")).ToString("N2", new CultureInfo("pt-BR"));
                        txtsObservacao.Text = RETORNO.DATASET(ds, "sObservacao");

                        CarregarCoberturas(Convert.ToInt32(idPlano));
                        CarregarLogs();

                        PainelAtualizacao.Atualizar(RETORNO.DATASET(ds, "dtAtualizacao"), RETORNO.DATASET(ds, "sDscUsuario"));

                        lblTituloPagina.Text = $"Editar {sTituloPagina}: {txtsDscPlano.Text}";
                        BreadCrumb_Pagina.TitulodaPagina = txtsDscPlano.Text;
                        lblTituloSalvar.Text = $"Salvar alterações no plano {txtsDscPlano.Text}?";
                        lblTitulosExcluir.Text = $"Deseja realmente excluir o plano {txtsDscPlano.Text}?";
                    }
                    else
                    {
                        DIV_Body.Visible = false;
                        MensagemPagina.MostraMensagem_Erro(sErro);
                    }
                }
                else
                {
                    hddidPlano.Value = "0";
                    txtidPlano.Text = "Novo";
                    BreadCrumb_Pagina.TitulodaPagina = "Novo";
                    lblTituloPagina.Text = $"Novo {sTituloPagina}";
                    lblTituloSalvar.Text = "Confirma a inclusão do novo plano?";
                    cmdSalvar.Text = "Incluir";
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        private void CarregarCoberturas(int idPlano)
        {
            var coberturas = new List<string>();
            var vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "SELECIONAR_COBERTURAS" }, { "@idPlano", idPlano.ToString() }
            };
            DataTable dt = BD.ExecutarDataTable(sProcedure, vParametros);
            foreach (DataRow row in dt.Rows)
            {
                coberturas.Add(row["sDscCobertura"].ToString());
            }

            CoberturasAtuais = coberturas;
            BindGridCoberturas();
        }

        private void BindGridCoberturas()
        {
            gvCoberturas.DataSource = CoberturasAtuais.Select(c => new { Descricao = c }).ToList();
            gvCoberturas.DataBind();
        }

        #endregion

        #region Ações (Salvar, Excluir, Coberturas)

        private void Salvar()
        {
            if (hddidPlano.Value == "0")
                FUNCOES.ValidaPermissao(Permissao.RRHH.Planos.Incluir, true);
            else
                FUNCOES.ValidaPermissao(Permissao.RRHH.Planos.Alterar, true);

            if (ValidarDados())
            {
                try
                {
                    XElement xmlCoberturas = new XElement("coberturas", CoberturasAtuais.Select(c => new XElement("item", c)));

                    var vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idPlano", hddidPlano.Value },
                        { "@sDscPlano", txtsDscPlano.Text.Trim() },
                        { "@nValor", txtnValor.Text.Trim() },
                        { "@sObservacao", txtsObservacao.Text.Trim() },
                        { "@xmlCoberturas", xmlCoberturas.ToString() },
                        { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                    };

                    DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);
                    if (BD.ValidarDataSet(ds, out string sErro))
                    {
                        string idPlano = RETORNO.DATASET(ds, "idPlano");

                        if (hddidPlano.Value == "0")
                        {
                            FUNCOES.DirecionaPagina($"{sCaminho}PlanosSSTT_Detalhe.aspx?id={idPlano}&msg=sucesso");
                        }
                        else
                        {
                            CarregarDados(idPlano);
                            MensagemPagina.MostraMensagem_Sucesso("Plano salvo com sucesso!");
                        }
                    }
                    else
                    {
                        throw new Exception("BD: " + sErro);
                    }
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro(ex.Message);
                }
            }
        }

        private void Excluir()
        {
            FUNCOES.ValidaPermissao(Permissao.RRHH.Planos.Excluir, true);
            try
            {
                var vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "EXcluir" },
                    { "@idPlano", hddidPlano.Value },
                    { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario() }
                };
                
                DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);

                if (BD.ValidarDataSet(ds, out string sErro))
                {
                    FUNCOES.DirecionaPagina($"{sCaminho}PlanosSSTT.aspx");
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro("Erro ao excluir: " + BD.Retorno.DATASET(ds,"msg"));
                }

               
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao excluir: " + ex.Message);
            }
        }

        protected void btnAdicionarCobertura_Click(object sender, EventArgs e)
        {
            if (hddidPlano.Value != "0") FUNCOES.ValidaPermissao(Permissao.RRHH.Planos.Alterar, true);
            else FUNCOES.ValidaPermissao(Permissao.RRHH.Planos.Incluir, true);

            if (!string.IsNullOrWhiteSpace(txtNovaCobertura.Text))
            {
                var lista = CoberturasAtuais;
                lista.Insert(0, txtNovaCobertura.Text.Trim());
                CoberturasAtuais = lista;

                BindGridCoberturas();
                txtNovaCobertura.Text = string.Empty;
                txtNovaCobertura.Focus();
            }
        }

        protected void gvCoberturas_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (hddidPlano.Value != "0") FUNCOES.ValidaPermissao(Permissao.RRHH.Planos.Alterar, true);
            else FUNCOES.ValidaPermissao(Permissao.RRHH.Planos.Incluir, true);

            var lista = CoberturasAtuais;
            lista.RemoveAt(e.RowIndex);
            CoberturasAtuais = lista;
            BindGridCoberturas();
        }

        #endregion

        #region Validação e Scripts

        private bool ValidarDados()
        {
            if (string.IsNullOrWhiteSpace(txtsDscPlano.Text))
            {
                MensagemPagina.MostraMensagem_Erro("A descrição do plano é obrigatória.");
                return false;
            }

            var culture = new CultureInfo("pt-BR");
            if (!string.IsNullOrWhiteSpace(txtnValor.Text) && !decimal.TryParse(txtnValor.Text.Replace(".", ""), NumberStyles.Currency, culture, out _))
            {
                MensagemPagina.MostraMensagem_Erro("O valor mensal informado é inválido. Use o formato 1.234,56.");
                return false;
            }
            return true;
        }

        private void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("$('[id*=txtnValor]').mask('000.000.000.000.000,00', { reverse: true });");

            sb.Append("$v192(function() {");
            sb.Append(" $v192(\"#dialog-Salvar\").dialog({ resizable: false, height: \"auto\", width: 400, modal: true, autoOpen: false, buttons: { \"Sim\": function() { __doPostBack(\"funcao_SALVAR\", \"\"); $v192(this).dialog(\"close\"); }, \"Não\": function() { $v192(this).dialog(\"close\"); } } });");
            sb.Append(" $v192('[id*=cmdSalvar]').click(function(e) { e.preventDefault(); $v192('#dialog-Salvar').dialog('open'); });");
            sb.Append(" $v192(\"#dialog-Excluir\").dialog({ resizable: false, height: \"auto\", width: 400, modal: true, autoOpen: false, buttons: { \"Sim\": function() { __doPostBack(\"funcao_Excluir\", \"\"); $v192(this).dialog(\"close\"); }, \"Não\": function() { $v192(this).dialog(\"close\"); } } });");
            sb.Append(" $v192('[id*=cmdExcluir]').click(function(e) { e.preventDefault(); $v192('#dialog-Excluir').dialog('open'); });");
            sb.Append("});");

            ScriptManager.RegisterStartupScript(this, GetType(), "js_ScriptPagina", sb.ToString(), true);
        }

        protected void CarregarLogs()
        {
            try
            {
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                 { "@sFuncao", "CONSULTAR_LOGS" },
                 { "@idPlano", hddidPlano.Value },
                };

                DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Colaboradores_GHE_Plano_SSTT", vParametros);

                if (ds.Tables.Count > 0)
                {
                    gvLogs.DataSource = ds.Tables[0];
                }
                else
                {
                    gvLogs.DataSource = null;
                }

                gvLogs.DataBind();

                gvLogs.Visible = true;

            }
            catch (Exception ex)
            {
                MensagemPaginaLOGS.MostraMensagem_Erro("Erro ao gerar relatório: " + ex.Message);
                gvLogs.Visible = false;
            }
        }
        #endregion
    }
}

