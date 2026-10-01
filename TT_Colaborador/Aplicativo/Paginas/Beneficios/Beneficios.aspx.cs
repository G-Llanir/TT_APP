using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using IDENTITY = TT.FrameWork.Identity;
using FUNCOES = TT.FrameWork.Funcoes;
using RETORNO = TT.FrameWork.BD.Retorno;
using BD = TT.FrameWork.BD;
using TT_Colaboradores.Aplicativo.Controles;

namespace TT_Colaborador.Aplicativo.Paginas.Beneficios
{
    public partial class Beneficios : Page
    {
        #region | Construtores

        string sProcedureArea = "sp_Manipula_tbl_Flow_Colaboradores_Area";
        string sProcedureBeneficios = "sp_Manipula_tbl_Flow_Colaboradores_Beneficios";
        string filtro = $"{DateTime.Now.Year}-{DateTime.Now.Month:D2}-01";

        #endregion

        #region | Funções Incialização do Form

        protected void Page_Load(object sender, EventArgs e)
        {
            lblTituloPagina.Text = "Benefícios";
            FUNCOES.ValidaPermissao(Permissao.Benefícios.Consultar, true, true);

            if (!IsPostBack)
            {
                string idUsuarioLogado = IDENTITY.Variaveis.idUsuario();
                Pesquisar(idUsuarioLogado);
            }
            else
            {
                var requestTarget = this.Request["__EVENTTARGET"];

                if (requestTarget == "funcao_SAIR")
                    FUNCOES.DirecionaPagina("/Aplicativo/MenuColaborador.aspx");
                else if (requestTarget == "funcao_Editar")
                    Pesquisar(hddidColaborador.Value);
            }

            RegistraScript();
        }

        #endregion

        #region | Metodos Banco de Dados

        protected void Pesquisar(string idUsuario)
        {
            try
            {
                LimpaCampos();

                if (idUsuario != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR-USUARIO" },
                        { "@idUsuarioIntegrado", idUsuario }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedureArea, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                    {
                        hddidColaborador.Value = RETORNO.DATASET(dsPesquisa, 0, "idColaborador");

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, 0, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, 0, "sDscUsuarioAtualizacao"));

                        PesquisarBeneficios(hddidColaborador.Value, filtro);
                    }
                    else
                        MensagemPagina.MostraMensagem_Erro(sErro);
                }

                RegistraScript();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        protected void PesquisarBeneficios(string idColaborador, string filtro)
        {
            try
            {
                LimpaCampos();

                if (idColaborador != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR-DADOS" },
                        { "@idColaborador", idColaborador }
                    };
                    DataSet dsSaude = BD.ExecutarDataSet(sProcedureArea, vParametros);

                    Dictionary<string, string> vParametrosControle = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR-RELATORIO" },
                        { "@idColaborador", idColaborador },
                        { "@dtReferencia", filtro }
                    };
                    DataSet dsControle = BD.ExecutarDataSet(sProcedureBeneficios, vParametrosControle);

                    if (BD.ValidarDataSet(dsSaude, out string sErro) && BD.ValidarDataSet(dsControle, out sErro))
                    {
                        beneficiosDados.Visible = true;
                        beneficiosDadosAnterior.Visible = false;
                        Cabecalho.Text = "Referência: " + Convert.ToDateTime(filtro).ToString("MMMM yyyy");
                        lblTitulo.Text = Convert.ToDateTime(filtro).ToString("MMMM yyyy");

                        ltrnVTValor.Text = RETORNO.DATASET(dsControle, 0, "ValorVT");
                        ltrsVR.Text = RETORNO.DATASET(dsControle, 0, "ValorVR");
                        ltrDiasUteis.Text = RETORNO.DATASET(dsControle, 0, "nDiasUteis");
                        ltrDiasAtestado.Text = RETORNO.DATASET(dsControle, 0, "nDiasDescontar_Atestado");
                        ltrDiasCredito.Text = RETORNO.DATASET(dsControle, 0, "nDiasCreditar");
                        ltrDiasFeriado.Text = RETORNO.DATASET(dsControle, 0, "nDiasDescontar_Feriado");

                        ltrnValorPlano.Text = RETORNO.DATASET(dsSaude, 0, "nValorPlano");
                        ltrsPlano.Text = RETORNO.DATASET(dsSaude, 0, "sPlanoConvenio");
                        ltrsQntDependentes.Text = RETORNO.DATASET(dsSaude, 0, "sQntDependentes");


                        if (RETORNO.DATASET(dsControle, 0, "optVR").ToUpper() == "N")
                            ltrPossuiVR.Text = "N/A";
                        else
                            ltrPossuiVR.Text = "Optante VR";

                        if (Convert.ToInt32(ltrDiasCredito.Text) > 0 && RETORNO.DATASET(dsControle, 0, "sCreditaVT").ToUpper() != "N")
                        {
                            ltrnDiaVt.Visible = true;
                            ltrnDiaVt.Text = $"Valor Adicional por dia: R$ {RETORNO.DATASET(dsSaude, 0, "nVTValor")}";
                        }
                        else
                            ltrnDiaVt.Text = "Valor Adicional: N/A";
                        if (Convert.ToInt32(ltrDiasCredito.Text) > 0 && RETORNO.DATASET(dsControle, 0, "sCreditaVR").ToUpper() != "N")
                        {
                            ltrnDiaVR.Visible = true;
                            ltrnDiaVR.Text = $"Valor Adicional por dia: R$ {RETORNO.DATASET(dsSaude, 0, "nValorDiaVR")}";
                        }
                        else
                            ltrnDiaVR.Text = "Valor Adicional: N/A";

                        if (Convert.ToInt32(ltrDiasAtestado.Text) > 0 && RETORNO.DATASET(dsControle, 0, "sVT").ToUpper() != "N")
                            ltrDiasDescVt.Text = $"Valor Descontado por dia: R$ {RETORNO.DATASET(dsSaude, 0, "nVTValor")}";
                        else
                            ltrDiasDescVt.Text = "Valor Descontado: N/A";
                        if (Convert.ToInt32(ltrDiasAtestado.Text) > 0 && RETORNO.DATASET(dsControle, 0, "sVR").ToUpper() != "N")
                        {
                            ltrDiasDescVR.Visible = true;
                            ltrDiasDescVR.Text = $"Valor Descontado por dia: R$ {RETORNO.DATASET(dsSaude, 0, "nValorDiaVR")} ";
                        }
                        else
                            ltrDiasDescVR.Text = "Valor Descontado: N/A";

                        PainelAtualizacao.Visible = true;
                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsControle, 0, "dtAtualizacao"), RETORNO.DATASET(dsControle, 0, "sDscUsuarioAtualizacao"));
                    }
                    else
                    {
                        MensagemPagina.MostraMensagem_Erro("Nenhum Lançamento Localizado!");

                        if (Referencia_MesAno.RetornaValor() == null)
                        {
                            beneficiosDados.Visible = false;
                            beneficiosDadosAnterior.Visible = true;

                            Referencia_MesAno1.retornaAnosAnteriores(2023);
                        }
                    }

                }

                RegistraScript();
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }

        #region | Eventos

        protected void cmdPesquisar_Click(object sender, EventArgs e)
        {
            if (Referencia_MesAno.RetornaValor() != null)
                filtro = Referencia_MesAno.RetornaValor().ToString();

            Pesquisar(IDENTITY.Variaveis.idUsuario());
            PesquisarBeneficios(hddidColaborador.Value, filtro);
        }

        protected void cmdLimpar_Click(object sender, EventArgs e)
        {
            Pesquisar(IDENTITY.Variaveis.idUsuario());
            PesquisarBeneficios(hddidColaborador.Value, filtro);
        }

        protected void cmdPesquisar_Click1(object sender, EventArgs e)
        {
            if (Referencia_MesAno1.RetornaValor() != null)
                filtro = Referencia_MesAno1.RetornaValor().ToString();

            Pesquisar(IDENTITY.Variaveis.idUsuario());
            PesquisarBeneficios(hddidColaborador.Value, filtro);
        }

        protected void cmdLimpar_Click1(object sender, EventArgs e)
        {
            Pesquisar(IDENTITY.Variaveis.idUsuario());
            PesquisarBeneficios(hddidColaborador.Value, filtro);
        }

        #endregion

        #endregion

        #region | Limpar Campos

        void LimpaCampos()
        {
            hddidColaborador.Value = "0";
            PainelAtualizacao.Visible = false;
        }

        #endregion

        #region | Script 

        void RegistraScript()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("$(function() {");

            sb.Append("$(\"#dialog-Salvar\").dialog({");
            sb.Append("resizable: false,");
            sb.Append("height: \"auto\",");
            sb.Append("width: 400,");
            sb.Append("modal: true,");
            sb.Append("autoOpen: false,");
            sb.Append("buttons:");
            sb.Append("{");
            sb.Append("\"Sim\": function() {");
            sb.Append("__doPostBack(\"funcao_SALVAR\", \"\");");
            sb.Append("$(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("\"Não\": function() {");
            sb.Append("$(this).dialog(\"close\");");
            sb.Append("},");
            sb.Append("}");
            sb.Append("});");
            sb.Append("$('[id*=cmdSalvar]').click(function(e) {");
            sb.Append("e.preventDefault();");
            sb.Append("$('#dialog-Salvar').dialog('open');");
            sb.Append("});");

            sb.Append("});");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);

        }

        #endregion
    }
}