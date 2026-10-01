using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using static TT.FrameWork.BD;

namespace TT_Flow.App.Paginas.Adm.Fiscal
{
    public partial class CodigoServico : System.Web.UI.Page
    {
        string sTituloPagina = "Serviços Federal";
        string sProcedure = "sp_Manipula_tbl_Flow_Codigo_Servico";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Funcoes.ValidaPermissao(Permissao.Manutencao.Codigo_Servico.Consultar, true);
                if (!Funcoes.ValidaPermissao(Permissao.Manutencao.Codigo_Servico.Inserir))
                {
                    btnNovoCodServico.Visible = false;
                }

                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;

                pnServicoConsulta.Visible = true;
                pnServicoDetalhe.Visible = false;

                Funcoes.Popula_Combo(ddlCodigoServico, "sp_Select 'Flow_Codigo_Servico_Pai'", "idCodigoServicoPai", "sDscServicoPai", false, "Todos os Serviços Pai", "0");
                Pesquisar();
            }           

        }

        private void Pesquisar()
        {

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@sPesquisa", txtPesquisa.Text.Trim() },
                { "@idServicoPai", ddlCodigoServico.SelectedValue }
            };
            DataTable tb = BD.ExecutarDataTable(sProcedure, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                div_gvConsulta.Visible = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsulta, tb, 0, new int[1] { 10 }, "asc", "false", "''"), true);               
            }
            else
            {
                div_gvConsulta.Visible = false;
            }
        }

        private void CodigoServicoDetalhe(string id)
        {
            try
            {
                string sErro = "";
                DataSet ds;
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE" },
                    { "@idCodigoServico", id }
                };
                ds = BD.ExecutarDataSet(sProcedure, vParametros);
                if (BD.ValidarDataSet(ds, out sErro))
                {
                    txtIdCodigoServico.Text = id;
                    ddlsCodigoServicoPai.SelectedValue = Retorno.DATASET(ds, 0, "idServicoPai");
                    txtnCodigoServicoFilho.Text = Retorno.DATASET(ds, 0, "idServicoFilho");
                    sCodigoServicoPai.InnerText = ddlsCodigoServicoPai.SelectedValue + ".";
                    txtsDscServicoFilho.Text = Retorno.DATASET(ds, 0, "sDscServicoFilho");
                    txtnIRRF.Text = Retorno.DATASET(ds, 0, "nIRRF");
                    txtnINSS.Text = Retorno.DATASET(ds, 0, "nINSS");
                    txtnISS.Text = Retorno.DATASET(ds, 0, "nISS");
                    txtnPIS.Text = Retorno.DATASET(ds, 0, "nPIS");
                    txtnCOFINS.Text = Retorno.DATASET(ds, 0, "nCOFINS");
                    txtnCSSL.Text = Retorno.DATASET(ds, 0, "nCSSL");

                    ddlsCodigoServicoPai.Attributes.Add("disabled", "disabled");
                    txtnCodigoServicoFilho.ReadOnly = true;
                    txtsDscServicoFilho.ReadOnly = true;                    

                    pnServicoConsulta.Visible = false;
                    pnServicoDetalhe.Visible = true;

                    if (!Funcoes.ValidaPermissao(Permissao.Manutencao.Codigo_Servico.Editar))
                    {
                        btnSalvar.Visible = false;
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

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }

        protected void btnNovoCodServico_Click(object sender, EventArgs e)
        {
            pnServicoConsulta.Visible = false;
            pnServicoDetalhe.Visible = true;

            LimpaCampos();
            PopulaCombo();

            txtIdCodigoServico.Text = "Novo";
            txtIdCodigoServico.ReadOnly = true;
            btnSalvar.Visible = true;

            RegistraScript();
        }

        protected void lbDetalheServico_Command(object sender, CommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();
            hddidCodigoServico.Value = id;
            PopulaCombo();
            CodigoServicoDetalhe(id);
        }

        private void LimpaCampos()
        {
            ddlsCodigoServicoPai.SelectedValue = "0";
            txtnCodigoServicoFilho.Text = "";
            txtsDscServicoFilho.Text = "";
            txtnCOFINS.Text = "0,00";
            txtnCSSL.Text = "0,00";
            txtnINSS.Text = "0,00";
            txtnIRRF.Text = "0,00";
            txtnISS.Text = "0,00";
            txtnPIS.Text = "0,00";
        }

        private void PopulaCombo()
        {
            Funcoes.Popula_Combo(ddlsCodigoServicoPai, "sp_Select 'Flow_Codigo_Servico_Pai'", "idCodigoServicoPai", "sDscServicoPai", false, "Selecione o Serviço Pai", "0");
        }

        private void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("$(document).ready(function () {");
            sb.AppendLine("     $('[id*=txtnIRRF]').mask('990,00', { reverse: true });");
            sb.AppendLine("     $('[id*=txtnINSS]').mask('990,00', { reverse: true });");
            sb.AppendLine("     $('[id*=txtnISS]').mask('99990,00', { reverse: true });");
            sb.AppendLine("     $('[id*=txtnPIS]').mask('990,00', { reverse: true });");
            sb.AppendLine("     $('[id*=txtnCOFINS]').mask('990,00', { reverse: true });");
            sb.AppendLine("     $('[id*=txtnCSSL]').mask('990,00', { reverse: true });");
            sb.AppendLine("     var input = $('#" + txtnCodigoServicoFilho.ClientID + "');");
            sb.AppendLine("     validarCodigoServico(input);");
            sb.AppendLine("     input.on('input', function() {");
            sb.AppendLine("         validarCodigoServico($(this));");
            sb.AppendLine("     });");
            sb.AppendLine("     input.on('blur', function() {");
            sb.AppendLine("         var valor = $(this).val();");
            sb.AppendLine("         if (valor.length === 1) {");
            sb.AppendLine("             $(this).val('0' + valor);");
            sb.AppendLine("         }");
            sb.AppendLine("     });");
            sb.AppendLine("});");
            sb.AppendLine(" function validarCodigoServico(input) {");
            sb.AppendLine("     var valor = input.val().replace(/\\D/g, '');");
            sb.AppendLine("     if (valor.length > 2) {");
            sb.AppendLine("         valor = valor.substring(0,2);");
            sb.AppendLine("     }");
            sb.AppendLine("     var numero = parseInt(valor || 0 );");
            sb.AppendLine("     if (numero < 0 || numero > 99) {");
            sb.AppendLine("         valor = '';");
            sb.AppendLine("     }");
            sb.AppendLine("     input.val(valor);");
            sb.AppendLine("     return valor;");
            sb.AppendLine(" }");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptCodigoServicoFilho", sb.ToString(), true);
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            string sErro = "";
            if (ValidarDados())
            {
                try
                {
                    DataSet ds;
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idCodigoServico", hddidCodigoServico.Value },
                        { "@idServicoPai", ddlsCodigoServicoPai.SelectedValue },
                        { "@idServicoFilho", txtnCodigoServicoFilho.Text },
                        { "@sDscServicoFilho", txtsDscServicoFilho.Text },
                        { "@nIRRF", txtnIRRF.Text.Replace(",",".") },
                        { "@nINSS", txtnINSS.Text.Replace(",",".") },
                        { "@nISS", txtnISS.Text.Replace(",",".") },
                        { "@nPIS", txtnPIS.Text.Replace(",", ".") },
                        { "@nCOFINS", txtnCOFINS.Text.Replace(",", ".") },
                        { "@nCSSL", txtnCSSL.Text.Replace(",", ".") },
                        { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() }
                    };
                    ds = BD.ExecutarDataSet(sProcedure, vParametros);
                    if (BD.ValidarDataSet(ds, out sErro))
                    {
                        hddidCodigoServico.Value = Retorno.DATASET(ds, 0, "idCodigoServico");
                        MensagemPaginaDetalhe.MostraMensagem_Sucesso("Código Serviço gravado com sucesso");
                        CodigoServicoDetalhe(hddidCodigoServico.Value);
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
            else
            {
                RegistraScript();
            }
        }

        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (ddlsCodigoServicoPai.SelectedValue == "0")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Serviço Pai";
            }

            if (txtnCodigoServicoFilho.Text.Length != 2)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Código do Serviço Filho deve ter dois digitos! Para valores abaixo de 10 colocar o zero primeiro. Ex: 03";
            }

            if (txtsDscServicoFilho.Text.Length < 5)
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Descrição do Serviço Filho deve ter ao menos 5 caracteres";
            }           

            if (txtnIRRF.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "O campo de IRRF não pode ficar vazio! Insira um valor ou coloque 0,00";
            }

            if (txtnINSS.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "O campo de INSS não pode ficar vazio! Insira um valor ou coloque 0,00";
            }

            if (txtnISS.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "O campo de ISS não pode ficar vazio! Insira um valor ou coloque 0,00";
            }

            if (txtnPIS.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "O campo de PIS não pode ficar vazio! Insira um valor ou coloque 0,00";
            }

            if (txtnCOFINS.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "O campo de COFINS não pode ficar vazio! Insira um valor ou coloque 0,00";
            }

            if (txtnCSSL.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "O campo de CSSL não pode ficar vazio! Insira um valor ou coloque 0,00";
            }            

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPaginaDetalhe.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            pnServicoConsulta.Visible = true;
            pnServicoDetalhe.Visible = false;
            Pesquisar();
        }

        protected void ddlsCodigoServicoPai_SelectedIndexChanged(object sender, EventArgs e)
        {
            sCodigoServicoPai.InnerText = ddlsCodigoServicoPai.SelectedValue + ".";
            RegistraScript();
        }
    }
}