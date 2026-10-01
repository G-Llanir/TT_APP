using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Controles;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static TT.FrameWork.BD;

namespace TT_Flow.App.Paginas.RRHH
{
    public partial class PlanoSaude : System.Web.UI.Page
    {
        string sTituloPagina = "Plano de Saúde";
        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                Funcoes.ValidaPermissao(Permissao.RRHH.PlanoSaude.Consultar, true);
                if (!Funcoes.ValidaPermissao(Permissao.RRHH.PlanoSaude.Incluir))
                {
                    btnNovoPlano.Visible = false;
                }

                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;

                AlterarVisualizacao_Edicao(false);

                Pesquisar();
            }
        }

        private void Pesquisar()
        {

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "Consultar_PlanoSaude" },
                { "@sPesquisa", txtPesquisa.Text.Trim() },
                { "@sTipo", ddlsTipoPesquisa.SelectedValue },
            };
            DataTable tb = BD.ExecutarDataTable(sProcedure, vParametros, false);

            if (tb.Rows.Count > 0)
            {
                div_gvConsulta.Visible = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(dtgvConsulta, tb, 0, "asc"), true);
            }
            else
            {
                div_gvConsulta.Visible = false;
                MensagemPagina.MostraMensagem_Erro("Nenhum registro encontrado");
            }
        }

        private void AlterarVisualizacao_Edicao(bool bHabilitar)
        {
            pnPlanoSaudeConsulta.Visible = !bHabilitar;
            pnPlanoSaudeDetalhe.Visible = bHabilitar;
            
        }

        protected void lbPlanoSaudeDetalhe_Plano(object sender, CommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();
            hddidPlanoSaude.Value = id;
            AlterarVisualizacao_Edicao(true);
            PlanoSaudeDetalhe();
        }

        private void PlanoSaudeDetalhe()
        {
            try
            {
                DataSet ds;
                Dictionary<string, string> vParametros = new Dictionary<string, string>
                {
                    { "@sFuncao", "CONSULTAR_DETALHE_PlanoSaude" },
                    { "@idPlanoSaude", hddidPlanoSaude.Value }
                };
                ds = BD.ExecutarDataSet(sProcedure, vParametros);
                if (BD.ValidarDataSet(ds, out string sErro))
                {
                    txtIdPlanoSaude.Text = hddidPlanoSaude.Value;
                    ddlsTipoPlanoSaude.SelectedValue = Retorno.DATASET(ds, 0, "sTipo");
                    txtsDscPlanoSaude.Text = Retorno.DATASET(ds, 0, "sDscPlanoSaude");
                    txtnCarencia.Text = Retorno.DATASET(ds, 0, "nCarencia");
                    txtnValorPlano.Text = Retorno.DATASET(ds, 0, "nValor");

                    PainelAtualizacao.Visible = true;
                    PainelAtualizacao.Personalizar(string.Format("Última atualização em <b>{0}</b> por <b>{1}</b>", Retorno.DATASET(ds, 0, 0, "dtAtualizacao"), Retorno.DATASET(ds, 0, 0, "sDscUsuarioAtualizacao")));

                    if (!Funcoes.ValidaPermissao(Permissao.RRHH.PlanoSaude.Editar))
                    {
                        btnSalvar.Visible = false;
                    }
                }
                else
                {
                    throw new Exception("BD: " + sErro.ToString());
                }

            }
            catch(Exception e)
            {
                MensagemPaginaDetalhe.MostraMensagem_Erro(e.Message);
            }
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            AlterarVisualizacao_Edicao(false);
            Pesquisar();
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
                        { "@sFuncao", "Salvar_NovoPlanoDeSaude" },
                        { "@idPlanoSaude", hddidPlanoSaude.Value},
                        { "@sTipo", ddlsTipoPlanoSaude.SelectedValue },
                        { "@sDscPlanoSaude", txtsDscPlanoSaude.Text },
                        { "@nCarencia", txtnCarencia.Text },
                        { "@nValorPlano", txtnValorPlano.Text.Replace(".","").Replace(",",".") },
                        { "@idUsuarioAtualizacao", Identity.Variaveis.idUsuario() }
                    };
                    ds = BD.ExecutarDataSet(sProcedure, vParametros);
                    if (BD.ValidarDataSet(ds, out string sErro))
                    {
                        hddidPlanoSaude.Value = Retorno.DATASET(ds, 0, "idPlanoSaude");

                        MensagemPaginaDetalhe.MostraMensagem_Sucesso("Plano de Saúde gravado com sucesso");
                        PlanoSaudeDetalhe();
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
            
            RegistraScript();
            
        }

        private void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<script type='text/javascript'>");
            sb.AppendLine("$(document).ready(function () {");
            sb.AppendLine("     $('[id*=txtnValor]').mask('0.000.000.009,99', { reverse: true });");
            sb.AppendLine("});");
            sb.AppendLine("</script>");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptExtrato", sb.ToString(), false);
        }

        private bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (ddlsTipoPlanoSaude.SelectedValue == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Tipo";
            }

            if (txtsDscPlanoSaude.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite uma Descrição para o Plano";
            }

            if (txtnCarencia.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Coloque a quantidade de dias da Carência";
            }

            if (txtnValorPlano.Text == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Digite o Valor do Plano";
            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPaginaDetalhe.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        protected void btnNovoPlano_Click(object sender, EventArgs e)
        {
            hddidPlanoSaude.Value = "0";
            AlterarVisualizacao_Edicao(true);
            LimpaCampos();
            txtIdPlanoSaude.Text = "Novo";
            RegistraScript();
        }

        private void LimpaCampos()
        {
            ddlsTipoPlanoSaude.SelectedValue = "";
            txtsDscPlanoSaude.Text = "";
            txtnCarencia.Text = "";
            txtnValorPlano.Text = "";
        }

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }
    }
}