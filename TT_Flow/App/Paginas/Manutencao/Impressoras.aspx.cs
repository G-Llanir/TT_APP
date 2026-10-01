using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using FUNCOES = TT.FrameWork.Funcoes;
using BD = TT.FrameWork.BD;
using RETORNO = TT.FrameWork.BD.Retorno;
using TT.FrameWork;
using System.Web.UI.WebControls;
using static TT.FrameWork.BD;
using static Permissao;

namespace TT_Flow.App.Paginas.Manutencao
{
    public partial class Impressoras : Page
    {
        string sTituloPagina = "Impressoras";
        string sPagina_NovoRegistro = "";

        #region | Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Impressora.Consultar, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.Impressora.Incluir);

            if (!IsPostBack)
            {
                lblTituloPagina.Text = sTituloPagina;
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;
                Pesquisar("0");
            }

            txtPesquisa.Focus();
        }
        #endregion

        #region | Eventos
        protected void cmdPesquisar_Click(object sender, EventArgs e)
         => Pesquisar("0");
        protected void cmdNovo_Click(object sender, EventArgs e)
        {
            LimparCampos();
            AbrirModal_Click(sender, e);
            lbltituloModal.Text = "Novo";
            hddidImpressora.Value = "0";
            Pesquisar("0");
        }
        protected void SalvarImpressora_Click(object sender, EventArgs e)
        {
            if (ValidarDadosImpressora())
            {
                Salvar_Impressora("0", "S", "");
            }
            else
            {
                AbrirModal_Click(sender, e);
            }

        }
        protected void DesativarImpressora_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string idImpressora = btn.CommandArgument;

            if (idImpressora != "0")
                Salvar_Impressora(idImpressora, "N", "Desativar");
        }
        protected void AtivarImpressora_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string idImpressora = btn.CommandArgument;

            if (idImpressora != "0")
                Salvar_Impressora(idImpressora, "S", "Ativar");
        }
        protected void EditarImpressora_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string idImpressora = btn.CommandArgument;

            EditarImpressora(idImpressora);
            AbrirModal_Click(sender, e);
        }
        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            bool podeDesativar = FUNCOES.ValidaPermissao(Permissao.Impressora.Desativar);
            bool podeEditar = FUNCOES.ValidaPermissao(Permissao.Impressora.Editar);
            bool podeAtivar = FUNCOES.ValidaPermissao(Permissao.Impressora.Ativar);

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton cmdAtivarImpressora = (LinkButton)e.Row.FindControl("cmdAtivarImpressora");
                LinkButton btnDesativar = (LinkButton)e.Row.FindControl("cmdDesativarImpressora");


                string sAtivo = DataBinder.Eval(e.Row.DataItem, "sAtivo")?.ToString();

                if (sAtivo == "N")
                {
                    if (podeAtivar)
                    {
                        cmdAtivarImpressora.Visible = true;
                        btnDesativar.Visible = false;
                    }
                    else
                    {
                        cmdAtivarImpressora.Visible = false;
                        btnDesativar.Visible = false;
                    }

                    e.Row.CssClass += " danger";
                }
                else if (sAtivo == "S")
                {
                    if (podeDesativar)
                    {
                        btnDesativar.Visible = true;
                        cmdAtivarImpressora.Visible = false;
                    }
                    else
                    {
                        btnDesativar.Visible = false;
                        cmdAtivarImpressora.Visible = false;
                    }

                    e.Row.CssClass += " success";
                }

                LinkButton btnEditar = (LinkButton)e.Row.FindControl("cmdEditarImpressora");
                if (btnEditar != null)
                    btnEditar.Visible = podeEditar;
            }

            if (!podeDesativar && !podeEditar && !podeAtivar)
                Funcoes.EsconderColunas(dtgvConsulta, "Ação");
            else
                Funcoes.ReexibirColunas(dtgvConsulta, "Ação");
        }
        #endregion

        #region | Funções de Pesquisar
        protected void Pesquisar(string idImpressora)
        {
            pnResultado.Visible = false;

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sDscImpressora", txtPesquisa.Text.Trim() },
                { "@sAtivo", ddlStatus.SelectedValue }
            };

            if (idImpressora != "0" || string.IsNullOrEmpty(idImpressora))
            {
                vParametros.Add("@idImpressora", idImpressora);

                DataSet ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Impressoras", vParametros, false);

                hddidImpressora.Value = idImpressora;
                txtsImpressora.Text = RETORNO.DATASET(ds, "sDscImpressora");
                lbltituloModal.Text = "Edição - " + txtsImpressora.Text;
                txtsIP.Text = RETORNO.DATASET(ds, "sIP");
                txtnPorta.Text = RETORNO.DATASET(ds, "nPorta");
            }

            if (vParametros.ContainsKey("@idImpressora"))
            {
                vParametros.Remove("@idImpressora");
            }

            DataTable tb = BD.ExecutarDataTable("sp_Manipula_tbl_Flow_Impressoras", vParametros, false);
            if (tb.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, tb, 0, new int[1] { 4 }, "asc", "false", "''"), true);
                pnResultado.Visible = true;
            }
            else
                MensagemPagina.MostraMensagem_Erro("Nenhum registro localizado para sua pesquisa!");

        }
        #endregion

        #region | Funções
        void LimparCampos()
        {
            txtnPorta.Text = "";
            txtsImpressora.Text = "";
            txtsIP.Text = "";           
        }
        void Salvar_Impressora(string idImpressora, string ativo, string evento)
        {
            try
            {
                if (string.IsNullOrEmpty(hddidImpressora.Value) || hddidImpressora.Value == "0")
                {
                    if (idImpressora == "0" || string.IsNullOrEmpty(idImpressora))
                    {
                        idImpressora = "0";
                        ativo = "S";
                    }                                      
                }
                else
                {
                    if (idImpressora == "0" || string.IsNullOrEmpty(idImpressora))
                        idImpressora = hddidImpressora.Value;
                }


                Dictionary<string, string> vParametros = new Dictionary<string, string>
               {
                { "@sFuncao", "SALVAR" },
                { "@idImpressora", idImpressora },
                { "@sDscImpressora", txtsImpressora.Text },
                { "@sIP", txtsIP.Text },
                { "@nPorta", txtnPorta.Text },
                { "@sSituacao", ativo },
                { "@sEvento", evento },
                { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() }
                };
                DataSet dsSalvar;
                dsSalvar = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Impressoras", vParametros, false);

                if (BD.ValidarDataSet(dsSalvar))
                {
                    if (evento == "Ativar")
                        MensagemPagina.MostraMensagem_Sucesso("Impressora <b>ativada</b> com sucesso!");
                    else if (evento == "Desativar")
                        MensagemPagina.MostraMensagem_Sucesso("Impressora <b>desativada</b> com sucesso!");
                    else
                    {
                        MensagemPagina.MostraMensagem_Sucesso("Registro salvo com sucesso!");
                    }
                    LimparCampos();
                    Pesquisar("0");
                    FecharModal("modalForm");
                }
                else
                {
                    MensagemPagina.MostraMensagem_Erro("Erro ao salvar!");
                }
            }
            catch (Exception ex)
            {
                MensagemPaginaModal.MostraMensagem_Erro("Erro: " + ex.ToString());
            }
        }
        void EditarImpressora(string idImpressora)
        {
            Pesquisar(idImpressora);
        }
        #endregion

        #region | Modal 
        protected void AbrirModal_Click(object sender, EventArgs e)
        {
            FecharModal("modalForm");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalEnvio", "$('#modalForm').modal('show');", true);
        }
        protected void FecharModal(string modalId)
        {
            string script = $@"
        $('#{modalId}').modal('hide');
        $('.modal-backdrop').remove();
        $('body').removeClass('modal-open');
       ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_CloseModal_" + modalId, script, true);
        }

        #endregion

        #region | Validações
        bool ValidarDadosImpressora()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (string.IsNullOrWhiteSpace(txtsImpressora.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o nome da Impressora.";
            }

            if (string.IsNullOrWhiteSpace(txtsIP.Text))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o endereço IP.";
            }
            else
            {
                string patternIP = @"^(\d{1,3}\.){3}\d{1,3}$";
                if (!System.Text.RegularExpressions.Regex.IsMatch(txtsIP.Text.Trim(), patternIP))
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "O endereço IP está em um formato inválido.";
                }
            }

            if (string.IsNullOrWhiteSpace(txtnPorta.Text) || txtnPorta.Text == "0000")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o número da porta.";
            }
            else
            {
                if (txtnPorta.Text.Length != 4)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "A porta deve conter 4 dígitos.";
                }
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPaginaModal.MostraMensagem_Erro(sMensagemErro, false);
            }

            return bRetorno;
        }
        #endregion
    }
}