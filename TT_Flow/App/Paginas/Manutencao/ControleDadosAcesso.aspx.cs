using MathNet.Numerics;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using NPOI.POIFS.Crypt.Dsig;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Paginas.RRHH;
using TT_Hub.App.Paginas.RRHH;
using static TT.FrameWork.BD;
using Funcoes = TT.FrameWork.Funcoes;

namespace TT_Flow.App.Paginas.Manutencao
{
    public partial class ControleAcesso : System.Web.UI.Page
    {
        string sTituloPagina = "Controle Dados de Acesso ";
        string sProcedure = "sp_Manipula_tbl_Usuarios";

        public List<cls_UsuarioLiberado> bs_UsuarioLiberado
        {
            get
            {
                if (ViewState["bs_UsuarioLiberado"] == null)
                {
                    ViewState["bs_UsuarioLiberado"] = new List<cls_UsuarioLiberado>();
                }
                return (List<cls_UsuarioLiberado>)ViewState["bs_UsuarioLiberado"];
            }
            set
            {
                ViewState["bs_UsuarioLiberado"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Funcoes.ValidaPermissao(Permissao.Manutencao.DadosControleAcesso.Consultar, true);
            if (!Funcoes.ValidaPermissao(Permissao.Manutencao.DadosControleAcesso.Incluir))
            {
                btnNovoControleAcesso.Visible = false;
            }

            BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;

            if (!IsPostBack)
            {

                if (Request.QueryString["action"] == "export")
                {
                    if (Session["RelatorioDadosAcesso_Parametros"] != null)
                    {
                        string sNomeArquivo = "Relatorio_ControleDadosAcesse";

                        Dictionary<string, string> vParametros = (Dictionary<string, string>)Session["RelatorioDadosAcesso_Parametros"];
                        DataSet ds = BD.ExecutarDataSet(sProcedure, vParametros);

                        ExportarConsultaSQLparaXLS(ds, sNomeArquivo);
                    }

                    return;
                }

                if (Request["id"] != null)
                {
                    string id = Request["id"].ToString();
                    hddidDadoAcesso.Value = id;
                    PopulaCombo();
                    DadoAcessoDetalhe(id);

                    return;
                }

                PopulaCombo();
                AlterarVisualizacao_Edicao(false);
                Pesquisar();

                if (Funcoes.ValidaPermissao(Permissao.Manutencao.DadosControleAcesso.Alterar_Cadeado))
                {
                    hddsPermissaoCadeado.Value = "1";
                }
            }

            RegistraScript();
        }

        private void Pesquisar()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "Consultar_DadosAcesso" },
                { "@sPesquisa", txtPesquisa.Text.Trim() },
                { "@sTipoDadoAcesso", ddlsTipoPesquisa.SelectedValue },
                { "@idDepartamento", ddlidDepartamentoPesquisa.SelectedValue },
                { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() },
                { "@idParceiro", ddlidParceiroPesquisa.SelectedValue }
            };
            DataTable tb = BD.ExecutarDataTable(sProcedure, vParametros, false);

            Session["RelatorioDadosAcesso_Parametros"] = SalvarParametrosParaExcel();

            if (tb.Rows.Count > 0)
            {
                div_gvConsulta.Visible = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables_Consulta", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb, 0, "asc"), true);
            }
            else
            {
                div_gvConsulta.Visible = false;
                MensagemPagina.MostraMensagem_Erro("Nenhum registro encontrado");
            }
        }

        private void AlterarVisualizacao_Edicao(bool bEdicao)
        {
            pnControleAcessoConsulta.Visible = !bEdicao;
            pnControleAcessoDetalhe.Visible = bEdicao;
        }

        private void PopulaCombo()
        {
            Funcoes.Popula_Combo(ddlidUsuario, "sp_Select 'Usuarios'", "idUsuario", "sDscUsuario", false, "Selecione um Usuário", "0");
            Funcoes.Popula_Combo(ddlidDepartamento, "sp_Select 'Flow_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Todos os Departamentos", "0");
            Funcoes.Popula_Combo(ddlidDepartamentoPesquisa, "sp_Select 'Flow_Departamentos'", "idDepartamento", "sDscDepartamento", false, "Todos os Departamentos", "0");
            Funcoes.Popula_Combo(ddlidParceiroPesquisa, "sp_Select 'Flow_Clientes'", "idCliente", "sCPNJ_RazaoSocial", false, "Todos os Cliente/Fornecedor", "0");
            Funcoes.Popula_Combo(ddlidParceiro, "sp_Select 'Flow_Clientes'", "idCliente", "sCPNJ_RazaoSocial", false, "Selecione um Cliente/Fornecedor", "0");
        }

        private Dictionary<string, string> SalvarParametrosParaExcel()
        {
            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "Consultar_Relatorio_Excel" },
                { "@sPesquisa", txtPesquisa.Text.Trim() },
                { "@sTipoDadoAcesso", ddlsTipoPesquisa.SelectedValue },
                { "@idDepartamento", ddlidDepartamentoPesquisa.SelectedValue },
                { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() },
                { "@idParceiro", ddlidParceiroPesquisa.SelectedValue }
            };

            return vParametros;
        }

        private void DadoAcessoDetalhe(string id)
        {
            bs_UsuarioLiberado.Clear();

            try
            {
                DataSet ds;
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE_DadoAcesso" },
                    { "@idDadoAcesso", id },
                    { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() }
                };
                ds = BD.ExecutarDataSet(sProcedure, vParametros);
                if (BD.ValidarDataSet(ds, out string sErro))
                {
                    txtIdControleAcesso.Text = id;
                    ddlsTipo.SelectedValue = Retorno.DATASET(ds, 0, "sTipo");
                    ddlidDepartamento.SelectedValue = Retorno.DATASET(ds, 0, "idDepartamento");
                    ddlidParceiro.SelectedValue = Retorno.DATASET(ds, 0, "idParceiro");
                    txtsEndereco.Text = Retorno.DATASET(ds, 0, "sEndereco");
                    txtsUsuario.Text = Retorno.DATASET(ds, 0, "sUsuario");
                    txtsSenha.Text = Retorno.DATASET(ds, 0, "sSenha");
                    txtsToken.Text = Retorno.DATASET(ds, 0, "sToken");
                    txtsEmailRecuperacao.Text = Retorno.DATASET(ds, 0, "sEmailRecuperacao");
                    txtsCelularRecuperacao.Text = Retorno.DATASET(ds, 0, "sCelularRecuperacao");
                    txtsOutrosDados.Text = Retorno.DATASET(ds, 0, "sOutrosDados");
                    txtsObservacao.Text = Retorno.DATASET(ds, 0, "sObservacao");
                    hddsCadeado.Value = Retorno.DATASET(ds, 0, "sCadeado");

                    AlterarVisualizacao_Edicao(true);

                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        bs_UsuarioLiberado.Clear();
                        foreach (DataRow row in ds.Tables[1].Rows)
                        {
                            cls_UsuarioLiberado objItem = new cls_UsuarioLiberado();
                            objItem.sFuncao = "Sem_Alteracao";
                            objItem.idDadoAcesso = Convert.ToInt32(row["idDadoAcesso"]);
                            objItem.idUsuario= Convert.ToInt32(row["idUsuario"]);
                            objItem.sDscUsuario = row["sDscUsuario"].ToString();
                            objItem.sPermissao = row["sPermissao"].ToString();

                            bs_UsuarioLiberado.Add(objItem);

                            if (row["idUsuario"].ToString() == Identity.Variaveis.idUsuario())
                            {
                                hddsPermissaoAcesso.Value = row["sPermissao"].ToString();
                            }
                        }

                        gvUsuarioLiberado.DataSource = bs_UsuarioLiberado;
                        gvUsuarioLiberado.DataBind();
                    }

                    if (hddsPermissaoAcesso.Value == "V")
                    {
                        ModoVisualizacao();                        
                    }

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables_Historico", TT.FrameWork.Grid.DataBindComScriptData(gv_Historico, ds.Tables[2], 0, "desc", "false", "''"), true);
                    aba_Historico.Visible = true;

                    if (!Funcoes.ValidaPermissao(Permissao.Manutencao.DadosControleAcesso.Alterar))
                    {
                        btnSalvar.Visible = false;
                    }

                    if (!Funcoes.ValidaPermissao(Permissao.Manutencao.DadosControleAcesso.Visualiza_Aba_Historico))
                    {
                        aba_Historico.Visible = false;
                    }

                    if (Funcoes.ValidaPermissao(Permissao.Manutencao.DadosControleAcesso.Alterar_Cadeado))
                    {
                        hddsPermissaoCadeado.Value = "1";
                    }
                }
                else
                {
                    throw new Exception("BD: " + sErro.ToString());
                }
            }
            catch (Exception e)
            {
                MensagemPaginaDetalhe.MostraMensagem_Erro(e.Message);
            }

            RegistraScript();
        }

        private void ModoVisualizacao()
        {
            ddlsTipo.Attributes.Add("disabled", "disabled");
            ddlidDepartamento.Attributes.Add("disabled", "disabled");
            ddlidParceiro.Attributes.Add("disabled", "disabled");
            txtsEndereco.ReadOnly = true;
            txtsUsuario.ReadOnly = true;
            txtsSenha.ReadOnly = true;
            txtsToken.ReadOnly = true;
            txtsOutrosDados.ReadOnly = true;
            txtsObservacao.ReadOnly = true;
            txtsEmailRecuperacao.ReadOnly = true;
            txtsCelularRecuperacao.ReadOnly = true;
            btnIncluirUsuario.Visible = false;
            btnSalvar.Visible = false;
            hddsCadeado.Value = "S";
            div_incluirUsuario.Visible = false;
            gvUsuarioLiberado.Columns[3].Visible = false;
        }

        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (ddlsTipo.SelectedValue == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Tipo";
            } 
            
            if (txtsEndereco.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite o Endereço";
            }

            if (txtsUsuario.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite o Usuário";
            }

            if (txtsSenha.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite a Senha";
            }

            if (txtsToken.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite o Token";
            }

            if (bs_UsuarioLiberado.All(x => x.sFuncao == "Excluir_UsuarioLiberado"))
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Incluir pelo menos um Usuário Liberado";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPaginaDetalhe.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        private void LimpaCampos()
        {
            ddlsTipo.SelectedValue = "";
            ddlidDepartamento.SelectedValue = "0";
            txtsEndereco.Text = "";
            txtsUsuario.Text = "";
            txtsSenha.Text = "";
            txtsToken.Text = "";
            txtsOutrosDados.Text = "";
            txtsObservacao.Text = "";

        }

        private bool SalvarUsuariosLiberados()
        {
            bool bExisteUsuarioEdicao = false;
            foreach (GridViewRow row in gvUsuarioLiberado.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    int idUsuario = Convert.ToInt32(gvUsuarioLiberado.DataKeys[row.RowIndex]["idUsuario"]);
                    DropDownList ddl = (DropDownList)row.FindControl("ddlPermissao");                    
                    
                    var usuario = bs_UsuarioLiberado.FirstOrDefault(x => x.idUsuario == idUsuario);
                    
                    if (usuario.sPermissao != ddl.SelectedValue)
                    {
                        usuario.sPermissao = ddl.SelectedValue;
                        usuario.sFuncao = "Salvar_UsuarioLiberado";
                    }

                    if (usuario.sPermissao == "E")
                        bExisteUsuarioEdicao = true;
                }
            }

            if (bExisteUsuarioEdicao)
            {
                foreach (var item in bs_UsuarioLiberado)
                {
                    if (item.sFuncao != "Sem_Alteracao")
                    {
                        Dictionary<string, string> vParametros = new Dictionary<string, string>
                        {
                            { "@sFuncao", item.sFuncao },
                            { "@idUsuario", item.idUsuario.ToString() },
                            { "@idDadoAcesso", hddidDadoAcesso.Value },
                            { "@sPermissao", item.sPermissao },
                            { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() }
                        };
                        DataTable dt = BD.ExecutarDataTable(sProcedure, vParametros);

                    }
                }
            }
            else
                MensagemPaginaDetalhe.MostraMensagem_Erro("Obrigatório ao menos um Usuário liberado ter a permissão para Editar");

            return bExisteUsuarioEdicao;
        }

        private void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("$('[id*=txtsCelularRecuperacao]').mask('(99) 99999-9999');");

            sb.AppendLine("document.addEventListener('DOMContentLoaded', function() {");
            sb.AppendLine("    document.body.addEventListener('change', function(event) {");
            sb.AppendLine("         if (event.target && event.target.id === 'lock') {");
            sb.AppendLine("             var isChecked = event.target.checked;");
            sb.AppendLine("             var mensagem;");
            sb.AppendLine("             if (isChecked) {");
            sb.AppendLine("                 mensagem = \"Deseja habilitar a edição?\"");
            sb.AppendLine("             } else {");
            sb.AppendLine("                 mensagem = \"Deseja desabilitar a edição?\"");
            sb.AppendLine("             }");
            sb.AppendLine("             alterarEdicao(mensagem, event);");
            sb.AppendLine("         }");
            sb.AppendLine("     });");
            sb.AppendLine("});");

            sb.AppendLine("window.addEventListener('load', function() {");
            sb.AppendLine($"     var permissao = document.getElementById('{hddsPermissaoCadeado.ClientID}').value;");
            sb.AppendLine("     var lockCheckbox = document.getElementById('lock');");
            sb.AppendLine($"     var hddidDadoAcesso = document.getElementById('{hddidDadoAcesso.ClientID}').value;");
            sb.AppendLine("     if (lockCheckbox) {");            
            sb.AppendLine($"         var hddsCadeado = document.getElementById('{hddsCadeado.ClientID}').value;");
            sb.AppendLine("         var isChecked = (hddsCadeado === \"N\");");
            sb.AppendLine($"         var btnSalvar = document.getElementById('{btnSalvar.ClientID}');");
            sb.AppendLine("         if (hddidDadoAcesso === '0') {");        
            sb.AppendLine("             isChecked = true;");
            sb.AppendLine("         }");
            sb.AppendLine("         if (isChecked) {");        
            sb.AppendLine("             lockCheckbox.checked = isChecked;");
            sb.AppendLine("             if (btnSalvar) btnSalvar.style.display = 'inline';");
            sb.AppendLine("         }");
            sb.AppendLine("         else {");            
            sb.AppendLine("             if (btnSalvar) btnSalvar.style.display = 'none';");
            sb.AppendLine("             lockCheckbox.checked = isChecked;");
            sb.AppendLine("             var shackle = document.querySelector('.shackle');");
            sb.AppendLine("             shackle.style.transform = 'rotateY(0deg)';");
            sb.AppendLine("         }");
            sb.AppendLine("     }");
            sb.AppendLine("});");

            sb.AppendLine("$v192(function() {");
            sb.AppendLine("     $v192(\"#dialog_Aceitar\").dialog({");
            sb.AppendLine("         resizable: false,");
            sb.AppendLine("         height: \"auto\",");
            sb.AppendLine("         width: 400,");
            sb.AppendLine("         modal: true,");
            sb.AppendLine("         autoOpen: false");
            sb.AppendLine("     });");
            sb.AppendLine("});");

            sb.AppendLine("function alterarEdicao(mensagem, event) {");        
            sb.AppendLine("     document.getElementById('Label3').innerText = mensagem;");
            sb.AppendLine($"     var btnSalvar = document.getElementById('{btnSalvar.ClientID}');");
            sb.AppendLine($"     var hddidDadoAcesso = document.getElementById('{hddidDadoAcesso.ClientID}');");
            sb.AppendLine("     var lockCheckbox = document.getElementById('lock');");
            sb.AppendLine("     var isChecked = event.target.checked;");
            sb.AppendLine("     $v192('#dialog_Aceitar').dialog('option', 'buttons', {");
            sb.AppendLine("         \"Sim\": function() {");
            sb.AppendLine("             var idDadoAcesso = hddidDadoAcesso.value;");
            sb.AppendLine("             $.ajax({");
            sb.AppendLine("                 url: '/app/Paginas/Manutencao/ControleDadosAcesso.aspx/SalvaEstadoCadeado',");
            sb.AppendLine("                 data: JSON.stringify({");
            sb.AppendLine("                     isLocked: isChecked,");
            sb.AppendLine("                     idDadoAcesso: idDadoAcesso");
            sb.AppendLine("                 }),");
            sb.AppendLine("                 contentType: 'application/json; charset=utf-8',");
            sb.AppendLine("                 type: 'POST',");
            sb.AppendLine("                 dataType: 'json',");
            sb.AppendLine("                 success: function(data) {");
            sb.AppendLine("                     var shackle = document.querySelector('.shackle');");
            sb.AppendLine("                     if (isChecked) {");
            sb.AppendLine("                         btnSalvar.style.display = 'inline';");
            sb.AppendLine("                         shackle.style.transform = 'rotateY(150deg) translateX(3px)';");
            sb.AppendLine("                         shackle.style.transformOrigin = 'right';");
            sb.AppendLine("                     }");
            sb.AppendLine("                     else {");            
            sb.AppendLine("                         btnSalvar.style.display = 'none';");
            sb.AppendLine("                         shackle.style.transform = 'rotateY(0deg)';");
            sb.AppendLine("                     }");
            sb.AppendLine("                     window.location.href = '/app/Paginas/Manutencao/ControleDadosAcesso.aspx?id=' + idDadoAcesso;");
            sb.AppendLine("                 },");
            sb.AppendLine("                 error: function(response) {");
            sb.AppendLine("                     alert(response.responseText);");
            sb.AppendLine("                 },");
            sb.AppendLine("                 failure: function(response) {");
            sb.AppendLine("                     alert(response.responseText);");
            sb.AppendLine("                 }");
            sb.AppendLine("             });");
            sb.AppendLine("             $v192(this).dialog(\"close\");");
            sb.AppendLine("         },");
            sb.AppendLine("         \"Não\": function() {");
            sb.AppendLine("             lockCheckbox.checked = !isChecked;");
            sb.AppendLine("             var shackle = document.querySelector('.shackle');");
            sb.AppendLine("             if (!isChecked) {");            
            sb.AppendLine("                 btnSalvar.style.display = 'inline';");
            sb.AppendLine("                 shackle.style.transform = 'rotateY(150deg) translateX(3px)';");
            sb.AppendLine("                 shackle.style.transformOrigin = 'right';");
            sb.AppendLine("             }");
            sb.AppendLine("             else {");            
            sb.AppendLine("                 btnSalvar.style.display = 'none';");
            sb.AppendLine("                 shackle.style.transform = 'rotateY(0deg)';");
            sb.AppendLine("             }");
            sb.AppendLine("             $v192(this).dialog(\"close\");");
            sb.AppendLine("         }");
            sb.AppendLine("     });");
            sb.AppendLine("     $v192('#dialog_Aceitar').dialog('open');");
            sb.AppendLine("}");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_ScriptPagina" + Guid.NewGuid(), sb.ToString(), true);
        }

        protected void lbDadoAcessoDetalhe_Command(object sender, CommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();
            hddidDadoAcesso.Value = id;
            PopulaCombo();
            DadoAcessoDetalhe(id);
        }

        protected void gvUsuarioLiberado_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int idUsuario = Convert.ToInt32(gvUsuarioLiberado.DataKeys[e.RowIndex].Value);

            var UsuarioLiberado = bs_UsuarioLiberado.FirstOrDefault(x => x.idUsuario == idUsuario);
            if (UsuarioLiberado != null)
            {
                UsuarioLiberado.sFuncao = "Excluir_UsuarioLiberado";
                gvUsuarioLiberado.DataSource = bs_UsuarioLiberado.Where(x => x.sFuncao.ToString() != "Excluir_UsuarioLiberado");
                gvUsuarioLiberado.DataBind();
            }
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }

        protected void btnNovoControleAcesso_Click(object sender, EventArgs e)
        {
            hddidDadoAcesso.Value = "0";
            hddsPermissaoCadeado.Value = "0";
            bs_UsuarioLiberado.Clear();
            aba_Historico.Visible = false;
            AlterarVisualizacao_Edicao(true);
            LimpaCampos();
            PopulaCombo();
            txtIdControleAcesso.Text = "Novo";            
            RegistraScript();
        }

        protected void btnIncluirUsuario_Click(object sender, EventArgs e)
        {
            if (ddlidUsuario.SelectedValue != "0")
            {
                if (!bs_UsuarioLiberado.Any(x => x.idUsuario.ToString() == ddlidUsuario.SelectedValue))
                {
                    cls_UsuarioLiberado objItem = new cls_UsuarioLiberado
                    {
                        sFuncao = "Salvar_UsuarioLiberado",
                        idDadoAcesso = Convert.ToInt32(hddidDadoAcesso.Value),
                        idUsuario = Convert.ToInt32(ddlidUsuario.SelectedValue),
                        sDscUsuario = ddlidUsuario.SelectedItem.ToString()                    
                    };

                    bs_UsuarioLiberado.Add(objItem);

                    gvUsuarioLiberado.DataSource = bs_UsuarioLiberado.Where(x => x.sFuncao.ToString() != "Excluir_UsuarioLiberado");
                    gvUsuarioLiberado.DataBind();

                    ddlidUsuario.SelectedValue = "0";

                }
                else
                {
                    ddlidUsuario.SelectedValue = "0";
                    MensagemPaginaDetalhe.MostraMensagem_Erro("Usuário Selecionado já consta na lista");
                }
            }
            else
            {
                MensagemPaginaDetalhe.MostraMensagem_Erro("Selecione um Usuário");
            }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            if (ValidarDados())
            {
                try
                {
                    DataSet ds;
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "Salvar_DadoAcesso" },
                        { "@idDadoAcesso", hddidDadoAcesso.Value},
                        { "@sTipoDadoAcesso", ddlsTipo.SelectedValue },
                        { "@idDepartamento", ddlidDepartamento.SelectedValue },
                        { "@idParceiro", ddlidParceiro.SelectedValue },
                        { "@sEndereco", txtsEndereco.Text },
                        { "@sUsuario", txtsUsuario.Text },
                        { "@sSenha", txtsSenha.Text },
                        { "@sToken", txtsToken.Text },
                        { "@sEmailRecuperacao", txtsEmailRecuperacao.Text },
                        { "@sCelularRecuperacao", txtsCelularRecuperacao.Text },
                        { "@sOutrosDados", txtsOutrosDados.Text },
                        { "@sObservacao", txtsObservacao.Text },
                        { "@sCadeado", hddsCadeado.Value },
                        { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() }
                    };
                    ds = BD.ExecutarDataSet(sProcedure, vParametros);
                    if (BD.ValidarDataSet(ds, out string sErro))
                    {
                        hddidDadoAcesso.Value = Retorno.DATASET(ds, 0, "idDadoAcesso");
                        bool bSalvoSucesso = false;

                        if (gvUsuarioLiberado.Rows.Count > 0)
                        {
                            bSalvoSucesso = SalvarUsuariosLiberados();
                        }

                        if (bSalvoSucesso)
                        {
                            MensagemPaginaDetalhe.MostraMensagem_Sucesso("Dados de Acesso gravado com sucesso");
                            DadoAcessoDetalhe(hddidDadoAcesso.Value);
                        }
                    }
                    else
                    {
                        throw new Exception("BD: " + sErro.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MensagemPaginaDetalhe.MostraMensagem_Erro(ex.Message);
                }
            }
        }       

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            AlterarVisualizacao_Edicao(false);
            Pesquisar();
        }

        protected void gv_Historico_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[3].Text = HttpUtility.HtmlDecode(e.Row.Cells[3].Text);
            }
        }

        protected void gvUsuarioLiberado_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlPermissao = (DropDownList)e.Row.FindControl("ddlPermissao");

                int idUsuario = Convert.ToInt32(gvUsuarioLiberado.DataKeys[e.Row.RowIndex].Value);
                var item = bs_UsuarioLiberado.FirstOrDefault(x => x.idUsuario == idUsuario);

                if (item != null)
                {                    
                    ddlPermissao.SelectedValue = item.sPermissao;                    
                }

                if (hddsPermissaoAcesso.Value == "V")
                {
                    ddlPermissao.Attributes.Add("disabled", "disabled");
                }
            }            
        }

        public static void ExportarConsultaSQLparaXLS(DataSet ds, string sNomeArquivoSemExten)
        {
            string sNomeArquivoComExtensao = sNomeArquivoSemExten + Funcoes.CarimboDataHora() + ".xls";
            var server = HttpContext.Current.Server;
            var response = HttpContext.Current.Response;

            string logoBase64 = "";
            string caminhoLogo = server.MapPath("~/img/LogoTT.png");
            if (File.Exists(caminhoLogo))
            {
                byte[] imageBytes = File.ReadAllBytes(caminhoLogo);
                string base64String = Convert.ToBase64String(imageBytes);
                logoBase64 = "data:image/png;base64," + base64String;
            }

            response.BufferOutput = true;
            response.Clear();
            response.ClearHeaders();
            response.AddHeader("content-disposition", "attachment; filename=" + sNomeArquivoComExtensao);
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.ContentType = "application/vnd.ms-excel";
            response.ContentEncoding = System.Text.Encoding.UTF8;

            StringBuilder lSbExcel = new StringBuilder();

            lSbExcel.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\r\n");
            lSbExcel.Append("<style type=\"text/css\">\r\n");
            lSbExcel.Append("body { font-family: Arial, Helvetica, sans-serif; color: #333; }\r\n");
            lSbExcel.Append(".report-table { border-collapse: collapse; width: 100%; font-size: 12px; }\r\n");

            // --- ESTILO AQUI ---
            lSbExcel.Append(".report-table th, .report-table td { font-family: Arial, Helvetica, sans-serif; border: 1px solid #999999; padding: 12px 15px; text-align: left; vertical-align: middle; }\r\n");

            lSbExcel.Append(".report-table th { background-color: #009a22; color: #ffffff; font-size: 13px; font-weight: bold; text-transform: uppercase; }\r\n");
            lSbExcel.Append(".report-table tr.alt-row td { background-color: #f2f2f2; }\r\n");
            lSbExcel.Append(".header-container { text-align: center; margin-bottom: 25px; }\r\n");
            lSbExcel.Append(".logo { max-height: 60px; margin-bottom: 15px; }\r\n");
            lSbExcel.Append(".report-title { font-family: Arial, Helvetica, sans-serif; color: #024e0a; font-size: 24px; font-weight: bold; margin: 0; }\r\n");
            lSbExcel.Append(".report-subtitle { font-family: Arial, Helvetica, sans-serif; font-size: 14px; text-align: center; color: #666; margin-top: 5px; }\r\n");
            lSbExcel.Append("hr.separator { border: 0; height: 2px; background-color: #009a22; margin-top: 25px; }\r\n");
            lSbExcel.Append("</style>\r\n\r\n");

            // --- Montagem do Cabeçalho ---
            lSbExcel.Append("<div class='header-container'>");
            if (!string.IsNullOrEmpty(logoBase64))
            {
                lSbExcel.AppendFormat("<img src='{0}' class='logo' />", logoBase64);
            }
            lSbExcel.Append("<div class='report-title'>Relatório Controle Dados de Acesso</div>");
            lSbExcel.AppendFormat("<div class='report-subtitle'>Gerado em: {0}</div>", DateTime.Now.ToString("dd/MM/yyyy 'às' HH:mm:ss"));
            lSbExcel.Append("</div>");
            lSbExcel.Append("<hr class='separator' />");

            // --- Montagem da Tabela de Dados ---
            lSbExcel.Append("<table class=\"report-table\">\r\n");
            lSbExcel.Append("<thead>\r\n");
            lSbExcel.Append("<tr>\r\n");
            foreach (DataColumn Coluna in ds.Tables[0].Columns)
            {
                lSbExcel.AppendFormat("\t<th>{0}</th>\r\n", Coluna.ColumnName);
            }
            lSbExcel.Append("</tr>\r\n");
            lSbExcel.Append("</thead>\r\n");
            lSbExcel.Append("<tbody>\r\n");

            int rowIndex = 0;
            foreach (DataRow Linha in ds.Tables[0].Rows)
            {
                string rowClass = (rowIndex % 2 != 0) ? "class='alt-row'" : "";
                lSbExcel.AppendFormat("<tr {0}>\r\n", rowClass);
                foreach (DataColumn Coluna in ds.Tables[0].Columns)
                {
                    lSbExcel.AppendFormat("\t<td>{0}</td>\r\n", HttpUtility.HtmlEncode(Linha[Coluna].ToString().Trim()));
                }
                lSbExcel.Append("</tr>\r\n");
                rowIndex++;
            }

            lSbExcel.Append("</tbody>\r\n");
            lSbExcel.Append("</table>\r\n");

            // --- Finalização da Resposta ---
            response.Write(lSbExcel.ToString());
            response.Flush();
            response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }


        [System.Web.Services.WebMethod]
        public static void SalvaEstadoCadeado(bool isLocked, string idDadoAcesso)
        {
            try
            {
                DataTable dt;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "Salvar_Cadeado");
                vParametros.Add("@sCadeado", isLocked == true ? "N" : "S");
                vParametros.Add("@idDadoAcesso", idDadoAcesso);
                vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
                dt = BD.ExecutarDataTable("sp_Manipula_tbl_Usuarios", vParametros);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [Serializable]
    public class cls_UsuarioLiberado
    {
        public string sFuncao { get; set; }
        public int idDadoAcesso { get; set; }
        public int idUsuario { get; set; }
        public string sDscUsuario { get; set; }
        public string sPermissao { get; set; }


    }
}