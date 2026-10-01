using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;
using TT_Flow.App.Controles;

namespace TT_Flow.App.Paginas.Adm.Fiscal
{
    public partial class Regras : Page
    {
        string sTituloPagina = "Regra Fiscal";
        string sProcedure = "sp_Manipula_tbl_Flow_Fiscal_Regras";
        int nCol_SUFRAMA = 13;

        #region | Page_Load + Pesquisar

        protected void Page_Load(object sender, EventArgs e)
        {
            FUNCOES.ValidaPermissao(Permissao.Administracao.Fiscal.Regras.Consultar, true);
            cmdNovo.Visible = FUNCOES.ValidaPermissao(Permissao.Administracao.Fiscal.Regras.Incluir);
            cmdSimular.Visible = FUNCOES.ValidaPermissao(Permissao.Administracao.Fiscal.Regras.Simular);

            if (!IsPostBack)
            {
                PopularCombos();
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                lblTituloPagina.Text = sTituloPagina;

                if (Request["id"] != null) Pesquisar_Detalhe(Request["id"].ToString());
                else Pesquisar();
            }
        }

        protected void Pesquisar()
        {
            pnResultado.Visible = false;
            pnDetalhe.Visible = false;
            pnPesquisa.Visible = true;
            lblSubTituloPagina.Text = "Consulta";

            string empresa = IDENTITY.Variaveis.idEmpresa();

            Dictionary<string, string> vParametros = new Dictionary<string, string>
            {
                { "@sFuncao", "CONSULTAR" },
                { "@idEmpresa", ddlEmpresa.SelectedValue },
                { "@sUFOrigem", ddlPesquisa_sUFOrigem.SelectedValue },
                { "@sUFDestino", ddlPesquisa_sUFDestino.SelectedValue },
                { "@sTipoRegra", ddlPesquisa_sTipoRegra.SelectedValue },
                { "@sSituacao", ddlPesquisa_sSituacao.SelectedValue },
                { "@idPaisPreferincia", empresa == "EUA" ? "2" : empresa == "Brasil" ? "1" : "0" },
                { "@idUsuario", IDENTITY.Variaveis.idUsuario().Trim() }
            };
            DataTable tb = BD.ExecutarDataTable(sProcedure, vParametros);

            if (tb.Rows.Count > 0)
            {
                pnResultado.Visible = true;
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DataTables", Grid.DataBindComScript(dtgvConsulta, tb, 1, "desc"), true);
            }
            else MensagemPagina.MostraMensagem_Erro("Nenhuma Regra encontrada!");            
        }

        void Pesquisar_Detalhe(string idRegra)
        {
            LimpaCampos();

            pnPesquisa.Visible = false;
            pnResultado.Visible = false;
            pnDetalhe.Visible = true;
            lblSubTituloPagina.Text = "Nova Regra";
            txtidRegra.Text = "Novo";
            ddlidTipo_SelectedIndexChanged(null, null);
            PainelAtualizacao.Visible = false;
            RegistrarScript();
            txtsDscRegra.Focus();

            string empresa = IDENTITY.Variaveis.idEmpresa();

            try
            {
                if (idRegra != "0" && FUNCOES.ValidaPermissao(Permissao.Administracao.Fiscal.Regras.Consultar, true))
                {
                    cmdSalvar.Visible = FUNCOES.ValidaPermissao(Permissao.Administracao.Fiscal.Regras.Alterar);

                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR" },
                        { "@idRegra", idRegra },
                        { "@idPaisPreferincia", empresa == "EUA" ? "2" : empresa == "Brasil" ? "1" : "0" },
                        { "@idUsuario", IDENTITY.Variaveis.idUsuario().Trim() },
                        { "@sSituacao", "T" }
                    };
                    DataSet dsConsulta = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsConsulta, out string sMensagem))
                    {
                        txtidRegra.Text = BD.Retorno.DATASET(dsConsulta, "idRegra").PadLeft(4, '0');
                        txtsDscRegra.Text = BD.Retorno.DATASET(dsConsulta, "sDscRegra");
                        ddlidEmpresa.SelectedValue = BD.Retorno.DATASET(dsConsulta, "idEmpresa");
                        ddlidEmpresa_SelectedIndexChanged(null, null);
                        ddlidEmpresa.Attributes.Add("disabled", "disabled");
                        ddlidTipo.SelectedValue = BD.Retorno.DATASET(dsConsulta, "idTipo");
                        ddlidTipo_SelectedIndexChanged(null, null);
                        ddlidTipo.Attributes.Add("disabled", "disabled");
                        ddlsUFOrigem.SelectedValue = BD.Retorno.DATASET(dsConsulta, "sUFOrigem");
                        ddlsUFDestino.SelectedValue = BD.Retorno.DATASET(dsConsulta, "sUFDestino");
                        ddlsContribuinte.SelectedValue = BD.Retorno.DATASET(dsConsulta, "sContribuinte");
                        ddlsTemIE.SelectedValue = BD.Retorno.DATASET(dsConsulta, "sTemIE");

                        if (ddlidTipo.SelectedItem.Text.Contains("Produto"))
                        {
                            txtsCFOP.Text = BD.Retorno.DATASET(dsConsulta, "sCFOP");
                            txtsCFOP_ST.Text = BD.Retorno.DATASET(dsConsulta, "sCFOP_ST");
                            ddlsProdutoIndustrializado.SelectedValue = BD.Retorno.DATASET(dsConsulta, "sProdutoIndustrializado");
                            ddlsProdutoImportado.SelectedValue = BD.Retorno.DATASET(dsConsulta, "sProdutoImportado");
                            ddlSuframa.SelectedValue = BD.Retorno.DATASET(dsConsulta, "sSUFRAMA");
                            ddlsTributaICMS.SelectedValue = BD.Retorno.DATASET(dsConsulta, "sTributaICMS");
                            ddlsBaseCalculo.SelectedValue = BD.Retorno.DATASET(dsConsulta, "sBaseCalculo");
                            ddlsICMSST.SelectedValue = BD.Retorno.DATASET(dsConsulta, "sICMSST");
                            ddlsCalculoIPI.SelectedValue = BD.Retorno.DATASET(dsConsulta, "sCalculoIPI");
                            ddlsCalculoDIFAL.SelectedValue = BD.Retorno.DATASET(dsConsulta, "sCalculoDIFAL");
                        }
                        else
                        {
                            ddlsUFDestino_SelectedIndexChanged(null, null);
                            ddlidCidadeDestino.SelectedValue = BD.Retorno.DATASET(dsConsulta, "idCidadeDestino");
                            txtsCodigoMunicipal.Text = BD.Retorno.DATASET(dsConsulta, "sCodigoMunicipal");
                            txtsCodigoFederal.Text = BD.Retorno.DATASET(dsConsulta, "sCodigoFederal");
                            txtnISS.Text = BD.Retorno.DATASET(dsConsulta, "nISS");
                            txtnCSSL.Text = BD.Retorno.DATASET(dsConsulta, "nCSSL");
                            txtnIR.Text = BD.Retorno.DATASET(dsConsulta, "nIR");
                            txtnINSS.Text = BD.Retorno.DATASET(dsConsulta, "nINSS");
                        }

                        txtnPIS.Text = BD.Retorno.DATASET(dsConsulta, "nPIS");
                        txtnCofins.Text = BD.Retorno.DATASET(dsConsulta, "nCofins");
                        ddlnOrdem.SelectedValue = BD.Retorno.DATASET(dsConsulta, "nOrdem");
                        ddlsSituacao.Situacao_Definir(BD.Retorno.DATASET(dsConsulta, "sSituacao"));

                        PainelAtualizacao.Atualizar(BD.Retorno.DATASET(dsConsulta, "dtAtualizacao"), BD.Retorno.DATASET(dsConsulta, "sDscUsuarioAtualizacao"));
                        PainelAtualizacao.Visible = true;
                        lblSubTituloPagina.Text = string.Format("Consulta Regra {0} - {1}", txtidRegra.Text.PadLeft(4, '0'), txtsDscRegra.Text);
                    }
                    else
                        MensagemPagina_Detalhe.MostraMensagem_Erro(sMensagem);
                }
                else
                    FUNCOES.ValidaPermissao(Permissao.Administracao.Fiscal.Regras.Incluir, true);
            }
            catch (Exception ex)
            {
                MensagemPagina_Detalhe.MostraMensagem_Erro(ex.Message);
            }
        }

        #endregion

        #region | Utils

        void PopularCombos()
        {
            FUNCOES.Popula_Combo(ddlEmpresa, "sp_Select 'Flow_Empresa', @idUsuario=" + IDENTITY.Variaveis.idUsuario(), "idEmpresa", "sDscEmpresa", false, "Todas as Empresas", "0");
            FUNCOES.Popula_Combo(ddlPesquisa_sTipoRegra, "sp_Manipula_tbl_Flow_Fiscal_Regras 'CONSULTAR_TIPO_REGRA'", "sTipoRegra", "sDscTipoRegra", false, "Todos os Tipos", "");
            FUNCOES.Popula_Combo(ddlPesquisa_sUFOrigem, "sp_Select 'UF'", "sEstado", "sDscEstado", true, "Todos os Estados de Origem", "");
            ddlPesquisa_sUFOrigem.Items.Add(new ListItem("EXTERNO", "EX"));
            FUNCOES.Popula_Combo(ddlPesquisa_sUFDestino, "sp_Select 'UF'", "sEstado", "sDscEstado", true, "Todos os Estados de Destino", "");
            ddlPesquisa_sUFDestino.Items.Add(new ListItem("EXTERNO", "EX"));
            ddlPesquisa_sUFDestino.Items.Add(new ListItem("DIFERENTE DA ORIGEM", "DI"));
            ddlPesquisa_sUFDestino.Items.Add(new ListItem("IMPORTADO", "IM"));

            FUNCOES.Popula_Combo(ddlidTipo, "sp_Manipula_tbl_Flow_Fiscal_Regras 'CONSULTAR_TIPO'", "idTipo", "sDscTipoCompleto", false, "Selecione o Tipo", "0");

            FUNCOES.Clona_DropDownList(ddlEmpresa, ddlidEmpresa);
            ddlidEmpresa.Items[0].Text = "Selecione a Empresa";

            // ----------------- Higor Maestrello 11/07/2024 -----------------
            if (IDENTITY.Variaveis.idEmpresa() == "Brasil") FUNCOES.Popula_Combo(ddlidEmpresa, $"sp_Select 'Flow_Empresa', @idPesquisa={1}, @idUsuario={IDENTITY.Variaveis.idUsuario()}", "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");
            else if (IDENTITY.Variaveis.idEmpresa() == "EUA") FUNCOES.Popula_Combo(ddlidEmpresa, $"sp_Select 'Flow_Empresa', @idPesquisa={2}, @idUsuario={IDENTITY.Variaveis.idUsuario()}", "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");
            // --------------------------------------------------------------

            FUNCOES.Clona_DropDownList(ddlPesquisa_sUFOrigem, ddlsUFOrigem);
            ddlsUFOrigem.Items[0].Text = "Selecione o Estado de Origem";
            FUNCOES.Clona_DropDownList(ddlPesquisa_sUFDestino, ddlsUFDestino);
            ddlsUFDestino.Items[0].Text = "Selecione o Estado de Destino";

            ddlsContribuinte.Items.Add(new ListItem("Indiferente", "I"));
            ddlsContribuinte.Items.Add(new ListItem("Sim", "S"));
            ddlsContribuinte.Items.Add(new ListItem("Não", "N"));

            FUNCOES.Clona_DropDownList(ddlsContribuinte, ddlsTemIE);
            FUNCOES.Clona_DropDownList(ddlsContribuinte, ddlsProdutoIndustrializado);
            FUNCOES.Clona_DropDownList(ddlsContribuinte, ddlsProdutoImportado);
        }

        void LimpaCampos()
        {
            DIV_idCidadeDestino.Visible = false;
            ddlsUFOrigem.Attributes.Remove("disabled");
            ddlidEmpresa.Attributes.Remove("disabled");
            ddlidTipo.Attributes.Remove("disabled");
            txtsDscRegra.Text = "";
            ddlidEmpresa.SelectedValue = "0";
            ddlidTipo.SelectedValue = "0";
            ddlsUFOrigem.SelectedValue = "";
            ddlsUFDestino.SelectedValue = "";
            ddlsContribuinte.SelectedValue = "I";
            ddlsTemIE.SelectedValue = "I";
            ddlsProdutoImportado.SelectedValue = "I";
            ddlSuframa.SelectedValue = "N";
            ddlsProdutoIndustrializado.SelectedValue = "I";
            ddlsTributaICMS.SelectedValue = "S";
            ddlsBaseCalculo.SelectedValue = "P";
            ddlsICMSST.SelectedValue = "N";
            ddlsCalculoIPI.SelectedValue = "S";
            ddlsCalculoDIFAL.SelectedValue = "N";
            txtsCFOP.Text = "";
            txtsCFOP_ST.Text = "";
            txtnPIS.Text = "";
            txtnCofins.Text = "";
            txtsCodigoMunicipal.Text = "";
            txtsCodigoFederal.Text = "";
            txtnISS.Text = "";
            txtnINSS.Text = "";
            txtnCSSL.Text = "";
            txtnIR.Text = "";
            ddlnOrdem.SelectedValue = "0";
            ddlsSituacao.Situacao_Definir("S");

            RegistrarScript();
        }

        bool ValidarDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";

            if (Validacoes.ValidarTexto(txtsDscRegra))
                sMensagemErro = "Informe um nome válido para a Regra";

            if (ddlidTipo.SelectedValue == "0")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Tipo";

            if (ddlsUFOrigem.SelectedValue == "")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma UF de Origem";

            if (ddlsUFDestino.SelectedValue == "")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma UF de Destino";

            if (ddlidTipo.SelectedItem.Text.Contains("Produto"))
            {
                if (Validacoes.ValidarTexto(txtsCFOP) || Validacoes.ValidarTexto(txtsCFOP_ST))
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe CFOPs Válidos";
                else if (txtsCFOP.Text.Length < 4 || txtsCFOP_ST.Text.Length < 4)
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe CFOPs Válidos";
            }
            else if (ddlidTipo.SelectedItem.Text.Contains("Serviço"))
            {
                if (Validacoes.ValidarTexto(txtsCodigoMunicipal))
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um Código Múnicipal Válido";
                else if (txtsCodigoMunicipal.Text.Length < 4)
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um Código Múnicipal Válido";

                if (Validacoes.ValidarTexto(txtsCodigoFederal))
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um Código Federal Válido";
                else if (txtsCodigoFederal.Text.Length < 3)
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um Código Federal Válido";

                if (!Validacoes.ValidarMoeda(txtnISS))
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um valor válido de ISS(%)";

                if (!Validacoes.ValidarMoeda(txtnCSSL))
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um valor válido de CSSL(%)";

                if (!Validacoes.ValidarMoeda(txtnIR))
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um valor válido de IR(%)";

                if (!Validacoes.ValidarMoeda(txtnINSS))
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um valor válido de INSS(%)";
            }

            if (!Validacoes.ValidarMoeda(txtnPIS))
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um valor válido de PIS(%)";

            if (!Validacoes.ValidarMoeda(txtnCofins))
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe um valor válido de COFINS(%)";

            if (sMensagemErro != "")
            {
                MensagemPagina_Detalhe.MostraMensagem_Erro(sMensagemErro);
                bRetorno = false;
            }

            return bRetorno;
        }

        #endregion

        #region | Eventos

        protected void cmdPesquisar_Click(object sender, EventArgs e) => Pesquisar();

        protected void cmdNovo_Click(object sender, EventArgs e) => Pesquisar_Detalhe("0");

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            pnDetalhe.Visible = false;
            pnPesquisa.Visible = true;
            Pesquisar();
            txtsPesquisa.Focus();
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            if (ValidarDados())
            {
                string idRegra = "0";
                if (txtidRegra.Text != "Novo") idRegra = txtidRegra.Text;

                try
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "SALVAR" },
                        { "@idRegra", idRegra },
                        { "@sDscRegra", txtsDscRegra.Text },
                        { "@idTipo", ddlidTipo.SelectedValue },
                        { "@idEmpresa", ddlidEmpresa.SelectedValue },
                        { "@sUFOrigem", ddlsUFOrigem.SelectedValue },
                        { "@sUFDestino", ddlsUFDestino.SelectedValue },
                        { "@sContribuinte", ddlsContribuinte.SelectedValue },
                        { "@sTemIE", ddlsTemIE.SelectedValue },
                        { "@nPIS", BD.Conversoes.Numerico(txtnPIS) },
                        { "@nCOFINS", BD.Conversoes.Numerico(txtnCofins) },
                        { "@nOrdem", ddlnOrdem.SelectedValue },
                        { "@sSituacao", ddlsSituacao.Situacao_Recuperar() },
                        { "@idUsuario", IDENTITY.Variaveis.idUsuario() }
                    };

                    if (ddlidTipo.SelectedItem.Text.Contains("Produto"))
                    {
                        vParametros.Add("@sProdutoIndustrializado", ddlsProdutoIndustrializado.SelectedValue);
                        vParametros.Add("@sProdutoImportado", ddlsProdutoImportado.SelectedValue);
                        vParametros.Add("@sSUFRAMA", ddlSuframa.SelectedValue);
                        vParametros.Add("@sCFOP", txtsCFOP.Text);
                        vParametros.Add("@sCFOP_ST", txtsCFOP_ST.Text);
                        vParametros.Add("@sTributaICMS", ddlsTributaICMS.SelectedValue);
                        vParametros.Add("@sICMSST", ddlsICMSST.SelectedValue);
                        vParametros.Add("@sCalculoIPI", ddlsCalculoIPI.SelectedValue);
                        vParametros.Add("@sCalculoDIFAL", ddlsCalculoDIFAL.SelectedValue);
                        vParametros.Add("@sBaseCalculo", ddlsBaseCalculo.SelectedValue);
                    }
                    else if (ddlidTipo.SelectedItem.Text.Contains("Serviço"))
                    {
                        vParametros.Add("@idCidadeDestino", ddlidCidadeDestino.SelectedValue);
                        vParametros.Add("@sCodigoMunicipal", txtsCodigoMunicipal.Text);
                        vParametros.Add("@sCodigoFederal", txtsCodigoFederal.Text);
                        vParametros.Add("@nISS", BD.Conversoes.Numerico(txtnISS));
                        vParametros.Add("@nINSS", BD.Conversoes.Numerico(txtnINSS));
                        vParametros.Add("@nCSSL", BD.Conversoes.Numerico(txtnCSSL));
                        vParametros.Add("@nIR", BD.Conversoes.Numerico(txtnIR));
                    }

                    DataSet dsSalvar = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsSalvar, out string sErro))
                    {
                        idRegra = BD.Retorno.DATASET(dsSalvar, "idRegra");
                        txtidRegra.Text = idRegra;
                        Pesquisar_Detalhe(idRegra);
                        MensagemPagina_Detalhe.MostraMensagem_Sucesso(string.Format("{1} gravado com sucesso!  </br><a href='Regras.aspx?id=0'>Clique aqui para incluir uma nova {1}.</a>", Request.RawUrl.ToString(), sTituloPagina));
                    }
                    else throw new Exception("BD: " + sErro.ToString());
                }
                catch (Exception ex)
                {
                    MensagemPagina_Detalhe.MostraMensagem_Erro(ex.Message);
                }
            }

            RegistrarScript();
        }

        protected void ddlidEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlsUFOrigem.SelectedValue = "";
            ddlsUFOrigem.Attributes.Remove("disabled");

            try
            {
                if (ddlidEmpresa.SelectedValue != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE" },
                        { "@idEmpresa", ddlidEmpresa.SelectedValue }
                    };
                    DataSet dsEmpresa = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Empresas", vParametros);

                    ddlsUFOrigem.SelectedValue = BD.Retorno.DATASET(dsEmpresa, "sEstado");
                    ddlsUFOrigem.Attributes.Add("disabled", "disabled");
                }
            }
            catch { }

            RegistrarScript();
        }

        protected void ddlidTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            DIV_Valores.Visible = false;
            DIV_ProdutoImportado.Visible = false;
            DIV_ProdutoIndustrializado.Visible = false;
            DIV_Valores_Produtos.Visible = false;
            DIV_Valores_Produtos2.Visible = false;
            DIV_Valores_Servicos.Visible = false;
            cmdSalvar.Visible = false;

            if (ddlidTipo.SelectedItem.Text.Contains("Produto"))
            {
                DIV_Valores.Visible = true;
                DIV_ProdutoImportado.Visible = true;
                DIV_ProdutoIndustrializado.Visible = true;
                DIV_Valores_Produtos.Visible = true;
                DIV_Valores_Produtos2.Visible = true;
                cmdSalvar.Visible = true;
            }
            else if (ddlidTipo.SelectedItem.Text.Contains("Serviço"))
            {
                DIV_Valores.Visible = true;
                DIV_Valores_Servicos.Visible = true;
                cmdSalvar.Visible = true;
            }

            RegistrarScript();
        }

        protected void ddlsUFDestino_SelectedIndexChanged(object sender, EventArgs e)
        {
            DIV_idCidadeDestino.Visible = false;

            if (ddlidTipo.SelectedItem.Text.Contains("Serviço") && ddlsUFDestino.SelectedValue != "" && ddlsUFDestino.SelectedValue != "DI" && ddlsUFDestino.SelectedValue != "IM")
            {
                FUNCOES.Popula_Combo(ddlidCidadeDestino, "sp_Select 'CIDADE', @sPesquisa='" + ddlsUFDestino.SelectedValue + "'", "idCidade", "sCidade", false, "Todos os Municípios", "0");
                DIV_idCidadeDestino.Visible = true;
            }

            RegistrarScript();
        }

        #endregion

        #region | dtgvConsulta

        protected void dtgvConsulta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow) e.Row.Cells[nCol_SUFRAMA].Text = e.Row.Cells[nCol_SUFRAMA].Text.Equals("S") ? "✔" : "";
        }

        #endregion

        #region | Script

        void RegistrarScript()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("$('[id*=txtsCFOP]').mask('9.999', { reverse: true });");
            sb.Append("$('[id*=txtsCodigoMunicipal]').mask('9999', { reverse: true });");
            sb.Append("$('[id*=txtsCodigoFederal]').mask('09.99', { reverse: true });");
            sb.Append("$('[id*=txtnPIS]').mask('009,99', { reverse: true });");
            sb.Append("$('[id*=txtnCofins]').mask('009,99', { reverse: true });");
            sb.Append("$('[id*=txtnISS]').mask('009,99', { reverse: true });");
            sb.Append("$('[id*=txtnCSSL]').mask('009,99', { reverse: true });");
            sb.Append("$('[id*=txtnIR]').mask('009,99', { reverse: true });");
            sb.Append("$('[id*=txtnINSS]').mask('009,99', { reverse: true });");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_ScriptPagina", sb.ToString(), true);
        }

        #endregion
    }
}