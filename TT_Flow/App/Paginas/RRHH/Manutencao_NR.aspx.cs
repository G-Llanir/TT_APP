using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Hub.App.Paginas.RRHH;
using Funcoes = TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas.RRHH
{
    public partial class Manutencao_NR : System.Web.UI.Page
    {
        string sTituloPagina = "NR - Normas Regulamentadoras";
        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Funcoes.ValidaPermissao(Permissao.RRHH.Manutencao_NR.Consultar, true);
                if (!Funcoes.ValidaPermissao(Permissao.RRHH.Manutencao_NR.Editar))
                {
                    div_salvar.Visible = false;
                }

                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;

                Pesquisar();
            }
            else
            {
                var requestTarget = Page.Request["__EVENTTARGET"];

                if (requestTarget == "funcao_SALVAR")
                    ValidarTabela();
            }

            RegistraScript();
        }

        private void Pesquisar()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR_Manutencao_NR" }
            };
            DataTable tb = BD.ExecutarDataTable(sProcedure, vParametros, false);

            lblTituloSalvar.Text = "Confirma as alterações realizadas? ";

            if (tb.Rows.Count > 0)
            {
                div_gvConsulta.Visible = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb, 1, "asc"), true);
            }
            else
            {
                div_gvConsulta.Visible = false;
            }
        }

        private string Salvar(string idTipoNR, string sValidadeAdministrativo, string sValidadeOperacional, string sPeriodicidadeAdmin, string sPeriodicidadeOper, string bValidadeAdministrativo, string bValidadeOperacional)
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "Salvar_Manutencao_NR" },
                { "@idTipoNR", idTipoNR },
                { "@nValidade_Administrativo", sValidadeAdministrativo },
                { "@nValidade_Operacional", sValidadeOperacional},
                { "@sPeriodicidadeAdministrativo", sPeriodicidadeAdmin},
                { "@sPeriodicidadeOperacional", sPeriodicidadeOper},
                { "@sValidadeAdministrativo", bValidadeAdministrativo},
                { "@sValidadeOperacional", bValidadeOperacional},
                { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() }
            };
            DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(ds, out string sErro))
            {
                return "Alterações salvas com sucesso!";
            }
            else
            {
                return sErro;
            }

        }

        private string Novo()
        {
            Dictionary<string, string> vParamentros = new Dictionary<string, string>
            {
                {"@sFuncao", "Novo_Manutencao_NR" },
                {"@sTipoNR", txtNr.Text },
                {"@sDscNR", txtDescricao.Text},
                {"@nValidade_Administrativo", txtValidadeAdm.Text},
                {"@nValidade_Operacional", txtValidadeOpe.Text },
                {"@sPeriodicidadeAdministrativo",ddlValidadeAdministrativa.SelectedValue},
                {"@sPeriodicidadeOperacional", ddlPeriocidadeOperacional.SelectedValue },
                { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() }
            };
            DataSet ds = BD.ExecutarDataSet(sProcedure, vParamentros);
            Pesquisar();
            return "NR incluído com sucesso!";
        }

        private bool ValidaDados(string sValidadeAdministrativo, string sValidadeOperacional)
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (sValidadeAdministrativo == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Validade Admistrativo não pode ser vazio, necessário inserir um valor";
            }

            if (sValidadeOperacional == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Validade Operacional não pode ser vazio, necessário inserir um valor";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        private void ValidarTabela()
        {
            try
            {
                string sMensagem = "";
                foreach (GridViewRow row in dtgvConsulta.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        int idTipoNR = Convert.ToInt32(dtgvConsulta.DataKeys[row.RowIndex].Value);

                        TextBox txtValAdmin = (TextBox)row.FindControl("txtnValidadeAdministrativo");
                        TextBox txtValOper = (TextBox)row.FindControl("txtnValidadeOperacional");
                        DropDownList ddlPeriodAdmin = (DropDownList)row.FindControl("ddlsPeriodicidadeAdministrativo");
                        DropDownList ddlPeriodOper = (DropDownList)row.FindControl("ddlsPeriodicidadeOperacional");
                        CheckBox cbValidadeAdministrativa = (CheckBox)row.FindControl("cbValidadeAdministrativa");
                        CheckBox cbValidadeOperacional = (CheckBox)row.FindControl("cbValidadeOperacional");

                        if (txtValAdmin == null || txtValOper == null || ddlPeriodAdmin == null || ddlPeriodOper == null || cbValidadeAdministrativa == null || cbValidadeOperacional == null)
                            continue;

                        if (ValidaDados(txtValAdmin.Text, txtValOper.Text))
                        {
                            sMensagem = Salvar(idTipoNR.ToString(), txtValAdmin.Text, txtValOper.Text, ddlPeriodAdmin.SelectedValue, ddlPeriodOper.SelectedValue, cbValidadeAdministrativa.Checked?"S":"N", cbValidadeOperacional.Checked?"S":"N");
                        }
                        else
                        {
                            sMensagem = "";
                            break;
                        }
                    }
                }

                if (sMensagem != "" && !sMensagem.Contains("Erro"))
                {
                    Pesquisar();
                    MensagemPagina.MostraMensagem_Sucesso(sMensagem);
                }
                else if (sMensagem.Contains("Erro"))
                    MensagemPagina.MostraMensagem_Erro(sMensagem);

            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro(ex.ToString());
            }
        }

        private void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            // Mensagens de Confirmação
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
            sb.Append("$v192('[id*=btnSalvar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$v192('#dialog-Salvar').dialog('open');");
            sb.Append("});");
            sb.Append("$('[id*=btnNovo]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$('#modalNovo').modal('show');");
            sb.Append("});");
            sb.AppendLine("});");
            sb.AppendLine("$(document).ready(function(){");
            sb.AppendLine("$('[id*=cbValidadeAdministrativa]').each(function(){");
            sb.AppendLine("var row = $(this).closest('tr');");
            sb.AppendLine("if($(this).prop('checked')){");
            sb.AppendLine("row.find('[id*=txtnValidadeAdministrativo]').show();");
            sb.AppendLine("row.find('[id*=ddlsPeriodicidadeAdministrativo]').show();");
            sb.AppendLine("}");
            sb.AppendLine("else{");
            sb.AppendLine("row.find('[id*=txtnValidadeAdministrativo]').hide();");
            sb.AppendLine("row.find('[id*=ddlsPeriodicidadeAdministrativo]').hide();");
            sb.AppendLine("}");
            sb.AppendLine("});");
            sb.AppendLine("$('[id*=cbValidadeOperacional]').each(function(){");
            sb.AppendLine("var row = $(this).closest('tr');");
            sb.AppendLine("if($(this).prop('checked')){");
            sb.AppendLine("row.find('[id*=txtnValidadeOperacional]').show();");
            sb.AppendLine("row.find('[id*=ddlsPeriodicidadeOperacional]').show();");
            sb.AppendLine("}");
            sb.AppendLine("else{");
            sb.AppendLine("row.find('[id*=txtnValidadeOperacional]').hide();");
            sb.AppendLine("row.find('[id*=ddlsPeriodicidadeOperacional]').hide();");
            sb.AppendLine("}");
            sb.AppendLine("});");
            sb.AppendLine("$('[id*=cbValidadeAdministrativa]').change(function(){");
            sb.AppendLine("var row = $(this).closest('tr');");
            sb.AppendLine("if($(this).prop('checked')){");
            sb.AppendLine("row.find('[id*=txtnValidadeAdministrativo]').show();");
            sb.AppendLine("row.find('[id*=ddlsPeriodicidadeAdministrativo]').show();");
            sb.AppendLine("}");
            sb.AppendLine("else{");
            sb.AppendLine("row.find('[id*=txtnValidadeAdministrativo]').val('0');");
            sb.AppendLine("row.find('[id*=txtnValidadeAdministrativo]').hide();");
            sb.AppendLine("row.find('[id*=ddlsPeriodicidadeAdministrativo]').hide();");
            sb.AppendLine("}");
            sb.AppendLine("});");
            sb.AppendLine("$('[id*=cbValidadeOperacional]').change(function(){");
            sb.AppendLine("var row = $(this).closest('tr');");
            sb.AppendLine("if($(this).prop('checked')){");
            sb.AppendLine("row.find('[id*=txtnValidadeOperacional]').show();");
            sb.AppendLine("row.find('[id*=ddlsPeriodicidadeOperacional]').show();");
            sb.AppendLine("}");
            sb.AppendLine("else{");
            sb.AppendLine("row.find('[id*=txtnValidadeOperacional]').val('0');");
            sb.AppendLine("row.find('[id*=txtnValidadeOperacional]').hide();");
            sb.AppendLine("row.find('[id*=ddlsPeriodicidadeOperacional]').hide();");
            sb.AppendLine("}");
            sb.AppendLine("});");
            sb.AppendLine("});"); 

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ScriptPagina", sb.ToString(), true);
        }

        protected void btnInlcuir_Click(object sender, EventArgs e)
        {
            string mensagemErro = "";


            if (string.IsNullOrEmpty(txtNr.Text))
            {
                mensagemErro += "É obrigatório preencher o campo de Tipo de Nr";
            }

            if (string.IsNullOrEmpty(txtDescricao.Text))
            {
                mensagemErro += "</br> É obrigatório preencher o campo de Descrição";
            }

            if (string.IsNullOrEmpty(txtValidadeAdm.Text))
            {
                mensagemErro += "</br> É obrigatório preencher o campo de Validade Administrativa";
            }

            if (string.IsNullOrEmpty(txtValidadeOpe.Text))
            {
                mensagemErro += "</br> É obrigatório preencher o campo de Validade Operacional";
            }

            if (string.IsNullOrEmpty(mensagemErro))
            {
                MensagemPagina.MostraMensagem_Sucesso(Novo());
            }
            else
            {
                MensagemPagina_Incluir.MostraMensagem_Erro(mensagemErro);
                Funcoes.Scripts.RemoverBackdrop_Modal(Page);
                Funcoes.Scripts.AbrirModal(Page, "modalNovo");
            }
        }
    }
}