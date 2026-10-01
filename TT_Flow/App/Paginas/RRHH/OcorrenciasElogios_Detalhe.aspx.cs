using iTextSharp.tool.xml.html.head;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.FrameWork;
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;
using RETORNO = TT.FrameWork.BD.Retorno;

namespace TT_Flow.App.Paginas.RRHH
{
    public partial class OcorrenciasElogios_Detalhe : Page
    {
        #region | Contrutores
        string sProcedure = "sp_Manipula_tbl_Flow_Colaboradores";        

        public List<cls_EPI_x_Funcao> bs_EPI_x_Funcao
        {

            get
            {
                if (ViewState["bs_EPI_x_Funcao"] == null)
                {
                    ViewState["bs_EPI_x_Funcao"] = new List<cls_EPI_x_Funcao>();
                }
                return (List<cls_EPI_x_Funcao>)ViewState["bs_EPI_x_Funcao"];
            }
            set
            {
                ViewState["bs_EPI_x_Funcao"] = value;
            }

        }
        #endregion

        #region | Page_Load + Pesquisar
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {               

                if (Request["id"] != null)
                {
                    FUNCOES.ValidaPermissao(Permissao.RRHH.OcorrenciasElogios.Consultar, true);

                    if (!FUNCOES.ValidaPermissao(Permissao.RRHH.OcorrenciasElogios.Alterar))
                        BtnSalvarEvento.Visible = false;

                    Pesquisar(Request["id"].ToString());
                }
                else
                {
                    FUNCOES.ValidaPermissao(Permissao.RRHH.OcorrenciasElogios.Incluir, true);
                    Pesquisar("0");
                }

                if (Request["msg"] != null)
                {
                    MensagemPagina.MostraMensagem_Sucesso("Registro gravado com sucesso!");
                }
            }

            RegistraScript();
        }

        protected void Pesquisar(string idPesquisa)
        {
            PopulaCombos();
            try
            {
                LimpaCampos();

                if (idPesquisa != "0")
                {
                    Dictionary<string, string> vParametros = new Dictionary<string, string>()
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE_Ocorrencias_Elogios" },
                        { "@idRegistroEvento", idPesquisa },
                        { "@sAdvertencia", "" },                       
                        { "@idUsuario", IDENTITY.Variaveis.idUsuario() }
                    };
                    DataSet dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out string sErro))
                    {
                    
                        hddidRegistroEvento.Value = RETORNO.DATASET(dsPesquisa, "idRegistroEvento");
                        txtidRegistroEvento.Text = RETORNO.DATASET(dsPesquisa, "idRegistroEvento");
                        ddlTipo.SelectedValue = RETORNO.DATASET(dsPesquisa, "sAdvertencia");
                        
                        ddlTipo_SelectedIndexChanged(null, null);

                        txtsDscEvento.Text = RETORNO.DATASET(dsPesquisa, "sDscEvento");
                        lblTituloPagina.Text = "Ocorrências e Elogios - " + RETORNO.DATASET(dsPesquisa, "sDscEvento");
                        ddlidReincidencia.SelectedValue = RETORNO.DATASET(dsPesquisa, "idReincidencia");
                        ddlidGravidade.SelectedValue = RETORNO.DATASET(dsPesquisa, "idGravidade");
                        ddlidProjeto.SelectedValue = RETORNO.DATASET(dsPesquisa, "idProjeto");
                        txtsCodReferencia.Text = RETORNO.DATASET(dsPesquisa, "sCodReferencia");
                        ddlidCustoOcorrencia.SelectedValue = RETORNO.DATASET(dsPesquisa, "idCustoOcorrencia");
                        txtnValorOcorrencia.Text = RETORNO.DATASET(dsPesquisa, "nValorOcorrencia");
                        ddlidResponsavelOcorrencia.SelectedValue = RETORNO.DATASET(dsPesquisa, "idResponsavelOcorrencia");
                        ddlidAfastamentoEvento.SelectedValue = RETORNO.DATASET(dsPesquisa, "idAfastamentoEvento");
                        txtsObservacaoEvento.Text = RETORNO.DATASET(dsPesquisa, "sObservacaoEvento");
                        ddlidTipoEvento.SelectedValue = RETORNO.DATASET(dsPesquisa, "idTipoEvento");
                        ddlidColaborador.SelectedValue = RETORNO.DATASET(dsPesquisa, "idColaborador");
                        hddidColaborador.Value = RETORNO.DATASET(dsPesquisa, "idColaborador");
                        ddlMotivo.SelectedValue = RETORNO.DATASET(dsPesquisa, "idEvento");
                        if (DateTime.TryParse(RETORNO.DATASET(dsPesquisa, "dtEvento"), out DateTime dtEvento))
                        {
                            txtdtEvento.Text = dtEvento.ToString("yyyy-MM-dd");
                        }
                        if (DateTime.TryParse(RETORNO.DATASET(dsPesquisa, "dtInicioAfastamento"), out DateTime dtInicioAfastamento))
                        {
                            txtdtInicioAfastamento.Text = dtInicioAfastamento.ToString("yyyy-MM-dd");
                        }
                        if (DateTime.TryParse(RETORNO.DATASET(dsPesquisa, "dtRetornoAfastamento"), out DateTime dtRetornoAfastamento))
                        {
                            txtdtRetornoAfastamento.Text = dtRetornoAfastamento.ToString("yyyy-MM-dd");
                        }

                        if (ddlidGravidade.SelectedValue == "4")
                            cmdPDF.Visible = true;

                        if (ddlTipo.SelectedValue == "A")
                        {
                            cmdPDF.Text = "Imprimir Advertência";
                        }
                        else if (ddlTipo.SelectedValue == "O")
                        {
                            cmdPDF.Text = "Imprimir Ocorrência";
                        }    
                        else
                        {
                            cmdPDF.Visible = false;
                        }

                        if (ddlidTipoEvento.SelectedValue == "1")
                        {
                            div_projeto.Visible = true;
                        }
                        else
                        {
                            div_projeto.Visible = false;
                        }

                        PainelAtualizacao.Atualizar(RETORNO.DATASET(dsPesquisa, "dtAtualizacao"), RETORNO.DATASET(dsPesquisa, "sDscUsuarioAtualizacao"));
                        PainelAtualizacao.Visible = true;

                        ConfiguraCampos();
                        Popular_Arquivo(idPesquisa);

                        ddlTipo.Attributes.Add("disabled", "disabled");
                        ddlidColaborador.Attributes.Add("disabled", "disabled");

                    }
                    else
                        throw new Exception(sErro);
                }
                else
                {
                    BreadCrumb.TitulodaPagina = "Novo";
                    //lblTituloPagina.Text = "Ocorrências e Elogios - Nova";
                    hddidRegistroEvento.Value = "0";
                    hddidColaborador.Value = "0";
                    ConfiguraCampos_NOVO();
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }
        #endregion

        #region | Combos
        protected void PopulaCombos()
        {
            FUNCOES.Popula_Combo(ddlMotivo, "sp_Manipula_tbl_Flow_Colaboradores 'tbl_Flow_Motivo_Advertencia'", "idMotivo", "sDscMotivo", false, "Selecione o Motivo", "0");
            //FUNCOES.Popula_Combo(ddlidColaborador, "sp_Select 'Flow_Credor_Colaboradores'", "idColaborador", "sDscColaborador", false, "Selecione o Colaborador", "0");
            FUNCOES.Popula_Combo(ddlidColaborador, "sp_Select 'Flow_Colaboradores_Ocorrencia', @idUsuario=" + IDENTITY.Variaveis.idUsuario(), "idColaborador", "sDscColaborador", false, "Selecione o Colaborador", "0");
            FUNCOES.Popula_Combo(ddlidProjeto, "sp_Select 'Flow_Pedidos'","idPedido", "sReferencia", false, "Selecione o Pedido", "0");
            PopularCombo_Tipo();
            PopularCombo_Gravidade();
        }

        void PopularCombo_Gravidade()
        {
            FUNCOES.Popula_Combo(ddlidGravidade, "sp_Select 'tbl_Flow_Colaboradores_Gravidade'", "idGravidade", "sDscGravidade", false, "Selecione a Gravidade", "0");
        }
        #endregion

        #region | Utils
        void LimpaCampos()
        {
            hddidRegistroEvento.Value = "0";
            txtsDscEvento.Text = "";
            ddlidReincidencia.SelectedValue = "0";
            ddlidGravidade.SelectedValue = "0";
            ddlidTipoEvento.SelectedValue = "0";
            txtsCodReferencia.Text = "";
            txtdtEvento.Text = "";
            ddlidCustoOcorrencia.SelectedValue = "0";
            txtnValorOcorrencia.Text = "0,00";
            ddlidCustoOcorrencia.SelectedValue = "0";
            txtnValorOcorrencia.Text = "";
            ddlidResponsavelOcorrencia.SelectedValue = "0";
            ddlidAfastamentoEvento.SelectedValue = "1";
            txtdtInicioAfastamento.Text = "";
            txtdtRetornoAfastamento.Text = "";
            txtsObservacaoEvento.Text = "";
            ddlidColaborador.SelectedValue = "0";
            ddlTipo.SelectedValue = "";
            //Advertencia.Definir("N", "Advertência ?", "");
        }

        void ConfiguraCampos_NOVO()
        {
            div_id.Visible = false;
            div_Colaborador.Visible = false;
            div_sDscEvento.Visible = false;
            div_dtEvento.Visible = false;
            div_idReincidencia.Visible = false;
            div_idGravidade.Visible = false;
            div_idTipoEvento.Visible = false;
            div_sCodReferencia.Visible = false;
            div_idCustoOcorrencia.Visible = false;
            div_ValorOcorrencia.Visible = false;
            div_idResponsavelOcorrencia.Visible = false;
            div_idAfastamentoEvento.Visible = false;
            div_sObservacaoEvento.Visible = false;
            div_ValorOcorrencia.Visible = false;
            div_InicioAfastamento.Visible = false;
            div_RetornoAfastamento.Visible = false;
            div_projeto.Visible = false;
            cmdPDF.Visible = false;
            div_Motivo.Visible = false;
            aba_Arquivo.Visible = false;
            txtnValorOcorrencia.Text = "0,00";
            BtnSalvarEvento.Visible = true;
            PainelAtualizacao.Visible = false;

        }
            
        private bool ValidarDados()
        {
            string sMensagemErro = "";

            if (txtdtEvento.Text == "")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe a Data do Evento";

            if (ddlidColaborador.SelectedValue == "0" && hddidColaborador.Value == "0")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe o Colaborador";

            if (ddlTipo.SelectedValue == "")
            {
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Tipo";
            }


            if (ddlTipo.SelectedValue == "O" || ddlTipo.SelectedValue == "E")
            {
                if (ddlidGravidade.SelectedValue == "0")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione a Gravidade";

                if (txtsDscEvento.Text == "")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe a Descrição do Evento";
            }

            if (ddlTipo.SelectedValue == "O" || ddlTipo.SelectedValue == "A")
            {
                if (ddlidReincidencia.SelectedValue == "0")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione a Reincidencia";

                if (ddlidCustoOcorrencia.SelectedValue == "0")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Informe se existiu Custo";                
            }


            if (ddlidTipoEvento.SelectedValue == "0")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Local do Evento";


            if (ddlTipo.SelectedValue == "O" )
            {

            }
            else if (ddlTipo.SelectedValue == "A")
            {
                if (ddlMotivo.SelectedValue == "0")
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Motivo Advertência";
            }      

            //if (txtsCodReferencia.Text == "")
            //    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Descreva a Referência";


            if (ddlidResponsavelOcorrencia.SelectedValue == "0" && ddlidCustoOcorrencia.SelectedValue == "2")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione o Responsável";

            if (txtsObservacaoEvento.Text == "")
                sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Descreva a Observação do Evento";

            if (!string.IsNullOrEmpty(sMensagemErro))
            {
                MensagemPagina.MostraMensagem_Erro(sMensagemErro);
                return false;
            }

            return true;
        }

        void ConfiguraCampos()
        {
            if (ddlidCustoOcorrencia.SelectedValue == "2")
            {
                div_ValorOcorrencia.Visible = true;
                div_idResponsavelOcorrencia.Visible = true;
            }
            else
            {
                div_ValorOcorrencia.Visible = false;
                div_idResponsavelOcorrencia.Visible = false;
            }

            if (ddlidAfastamentoEvento.SelectedValue == "2")
            {
                div_InicioAfastamento.Visible = true;
                div_RetornoAfastamento.Visible = true;
            }
            else
            {
                div_InicioAfastamento.Visible = false;
                div_RetornoAfastamento.Visible = false;
            }

            if (ddlTipo.SelectedValue != "A")
            {
                div_Motivo.Visible = false;
                div_sDscEvento.Visible = true;
            }
            else 
            {
                div_Motivo.Visible = true;
                div_sDscEvento.Visible = false;
            }
            
        }
        #endregion

        #region | Salvar
        protected void BtnSalvarEvento_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidarDados())
                {
                    string sErro = "";
                    DataSet dsPesquisa;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sfuncao", "SALVAR_EVENTO");
                    vParametros.Add("@idRegistro", hddidRegistroEvento.Value);
                    vParametros.Add("@idGravidade", ddlidGravidade.SelectedValue);
                    vParametros.Add("@idColaborador", ddlidColaborador.SelectedValue != "0" ? ddlidColaborador.SelectedValue : hddidColaborador.Value);
                    vParametros.Add("@sDscEvento", txtsDscEvento.Text);
                    vParametros.Add("@sObservacaoEvento", txtsObservacaoEvento.Text);
                    vParametros.Add("@dtEvento", txtdtEvento.Text);
                    vParametros.Add("@nValorOcorrencia", txtnValorOcorrencia.Text.Replace(".", "").Replace(",", "."));
                    vParametros.Add("@dtInicioAfastamento", txtdtInicioAfastamento.Text);
                    vParametros.Add("@dtRetornoAfastamento", txtdtRetornoAfastamento.Text);
                    vParametros.Add("@sCodReferencia", txtsCodReferencia.Text);
                    vParametros.Add("@sObservacaoOcorrencia", ddlidCustoOcorrencia.SelectedItem.ToString());
                    vParametros.Add("@idArquivo", "0");
                    vParametros.Add("@sReincidencia", ddlidReincidencia.SelectedItem.ToString());
                    vParametros.Add("@sTipoEvento", ddlidTipoEvento.SelectedItem.ToString());
                    vParametros.Add("@idProjeto", ddlidTipoEvento.SelectedValue == "1" ? ddlidProjeto.SelectedValue : "0");
                    vParametros.Add("@sCustoOcorrencia", ddlidCustoOcorrencia.SelectedItem.ToString());
                    vParametros.Add("@sResponsavelOcorrencia", ddlidResponsavelOcorrencia.SelectedItem.ToString());
                    vParametros.Add("@sAfastamentoEvento", ddlidAfastamentoEvento.SelectedItem.ToString());
                    vParametros.Add("@idReincidencia", ddlidReincidencia.SelectedValue);
                    vParametros.Add("@idTipoEvento", ddlidTipoEvento.SelectedValue);
                    vParametros.Add("@idCustoOcorrencia", ddlidCustoOcorrencia.SelectedValue);
                    vParametros.Add("@idResponsavelOcorrencia", ddlidResponsavelOcorrencia.SelectedValue);
                    vParametros.Add("@idAfastamentoEvento", ddlidAfastamentoEvento.SelectedValue);
                    vParametros.Add("@sAdvertencia", ddlTipo.SelectedValue);
                    vParametros.Add("@idEvento", ddlMotivo.SelectedValue);
                    vParametros.Add("@sDscGravidade", ddlidGravidade.SelectedItem.ToString());
                    vParametros.Add("@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario());
                    dsPesquisa = BD.ExecutarDataSet(sProcedure, vParametros);

                    if (BD.ValidarDataSet(dsPesquisa, out sErro))
                    {
                        hddidRegistroEvento.Value = RETORNO.DATASET(dsPesquisa, 0, "idRegistroEvento");
                        //FUNCOES.DirecionaPagina("App/Paginas/RRHH/OcorrenciasElogios_Detalhe.aspx?id=" + hddidRegistroEvento.Value + "&msg=S");
                        Pesquisar(hddidRegistroEvento.Value);
                        MensagemPagina.MostraMensagem_Sucesso("Evento gravado com sucesso!");
                    }
                    else
                    {
                        throw new Exception(sErro);
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro: " + ex.Message);
            }
        }
        #endregion

        #region | Script
        protected void RegistraScript()
        {
            FUNCOES.Scripts.Aplica_TooltipPersonalizado(Page, "tooltip");

            StringBuilder sb = new StringBuilder();

            sb.Append("$('[id*=txtnValorOcorrencia]').mask('000.000.000.000.000,00', { reverse: true });");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_Cadeado", sb.ToString(), true);
        }
        #endregion

        #region | Evento
        protected void ddlidCustoOcorrencia_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlidCustoOcorrencia.SelectedValue == "2")
            {
                div_ValorOcorrencia.Visible = true;
                div_idResponsavelOcorrencia.Visible = true;
            }
            else
            {
                div_ValorOcorrencia.Visible = false;
                div_idResponsavelOcorrencia.Visible = false;
            }
        }

        protected void ddlidAfastamentoEvento_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlidAfastamentoEvento.SelectedValue == "2")
            {
                div_InicioAfastamento.Visible = true;
                div_RetornoAfastamento.Visible = true;
            }
            else
            {
                div_InicioAfastamento.Visible = false;
                div_RetornoAfastamento.Visible = false;
            }
        }

        protected void cmdPDF_Click(object sender, EventArgs e)
        {
            if (ddlTipo.SelectedValue == "A")
            {
                try
                {
                    bool bPDF = true;
                    ReportViewer rv = new ReportViewer();

                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.EnableExternalImages = true;

                    rv.LocalReport.ReportPath = "App\\Reports\\" + "Advertencia.rdlc";

                    Dictionary<string, string> vParametrosProduct = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE_AFASTAMENTO" },
                        { "@idRegistroEvento", hddidRegistroEvento.Value }
                    };
                    DataSet dtProduct1 = BD.ExecutarDataSet(sProcedure, vParametrosProduct);
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtProduct1.Tables[0]));

                    ReportParameter[] rp = new ReportParameter[2];

                    rp[0] = new ReportParameter("Data", DateTime.Now.ToString("dd/MM/yyyy"));
                    rp[1] = new ReportParameter("Nivel", "3");                    

                    rv.LocalReport.SetParameters(rp);
                    rv.LocalReport.Refresh();

                    byte[] bytesPedidoCompra = bPDF ? rv.LocalReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings) : rv.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                    string sNomeArquivoPedidoCompra = "Advertência_" + FUNCOES.CarimboDataHora() + ".pdf";
                    File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivoPedidoCompra, bytesPedidoCompra);

                    FUNCOES.DownloadArquivo(Page, sNomeArquivoPedidoCompra);
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro("Erro ao gerar a Advertência! </br>" + ex.Message + (ex.InnerException == null ? "" : "<br />" + ex.InnerException.Message));
                }
            }
            else
            {
                try
                {
                    bool bPDF = true;
                    ReportViewer rv = new ReportViewer();

                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.EnableExternalImages = true;

                    rv.LocalReport.ReportPath = "App\\Reports\\" + "Ocorrencia.rdlc";

                    Dictionary<string, string> vParametrosProduct = new Dictionary<string, string>
                    {
                        { "@sFuncao", "CONSULTAR_DETALHE_AFASTAMENTO" },
                        { "@idRegistroEvento", hddidRegistroEvento.Value }
                    };
                    DataSet dtProduct1 = BD.ExecutarDataSet(sProcedure, vParametrosProduct);
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtProduct1.Tables[0]));

                    ReportParameter[] rp = new ReportParameter[3];

                    rp[0] = new ReportParameter("Data", DateTime.Now.ToString("dd/MM/yyyy"));
                    rp[1] = new ReportParameter("sCliente", RETORNO.DATASET(dtProduct1, "sCliente"));
                    rp[2] = new ReportParameter("sReferencia", RETORNO.DATASET(dtProduct1, "sReferencia"));

                    rv.LocalReport.SetParameters(rp);
                    rv.LocalReport.Refresh();

                    byte[] bytesPedidoCompra = bPDF ? rv.LocalReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings) : rv.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                    string sNomeArquivoPedidoCompra = "Ocorrência_" + FUNCOES.CarimboDataHora() + ".pdf";
                    File.WriteAllBytes(Server.MapPath("~/Download/") + sNomeArquivoPedidoCompra, bytesPedidoCompra);

                    FUNCOES.DownloadArquivo(Page, sNomeArquivoPedidoCompra);
                }
                catch (Exception ex)
                {
                    MensagemPagina.MostraMensagem_Erro("Erro ao gerar a Ocorrência! </br>" + ex.Message + (ex.InnerException == null ? "" : "<br />" + ex.InnerException.Message));
                }
            }
        }

        protected void ddlTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            FUNCOES.Popula_Combo(ddlidGravidade, "sp_Select 'tbl_Flow_Colaboradores_Gravidade'", "idGravidade", "sDscGravidade_Completa", false, "Selecione a Gravidade", "0");
            BtnSalvarEvento.Visible = true;
            if (ddlTipo.SelectedValue == "A")
            {
                div_Motivo.Visible = true;
                div_sDscEvento.Visible = false;
                div_Colaborador.Visible = true;
                div_dtEvento.Visible = true;
                div_idReincidencia.Visible = true;
                div_idGravidade.Visible = false;
                div_idTipoEvento.Visible = true;
                div_idCustoOcorrencia.Visible = true;
                div_idResponsavelOcorrencia.Visible = false;
                div_idAfastamentoEvento.Visible = true;
                div_sObservacaoEvento.Visible = true;
                ddlidGravidade.SelectedValue = "0";
 
            }
            else if (ddlTipo.SelectedValue == "O" )
            {
                div_Motivo.Visible = false;
                div_sDscEvento.Visible = true;
                div_Colaborador.Visible = true;
                div_dtEvento.Visible = true;
                div_idReincidencia.Visible = true;
                div_idGravidade.Visible = true;
                div_idTipoEvento.Visible = true;
                div_idCustoOcorrencia.Visible = true;
                div_idResponsavelOcorrencia.Visible = false;
                div_idAfastamentoEvento.Visible = true;
                div_sObservacaoEvento.Visible = true;
                ddlidGravidade.Items.Remove(ddlidGravidade.Items.FindByValue("4"));               
                ddlidGravidade.SelectedValue = "0";
                ddlidGravidade.Attributes.Remove("disabled");
            }
            else if (ddlTipo.SelectedValue == "E")
            {
                div_Motivo.Visible = false;
                div_sDscEvento.Visible = true;
                div_Colaborador.Visible = true;
                div_dtEvento.Visible = true;
                div_idReincidencia.Visible = false;
                div_idGravidade.Visible = true;
                div_idTipoEvento.Visible = true;
                div_idCustoOcorrencia.Visible = false;
                div_idResponsavelOcorrencia.Visible = false;
                div_idAfastamentoEvento.Visible = false;
                div_sObservacaoEvento.Visible = true;
                ddlidGravidade.SelectedValue = "4";
                ddlidGravidade.Attributes.Add("disabled", "disabled");
               
            }
            else
            {
                ConfiguraCampos_NOVO();
            }
        }

        protected void ddlidTipoEvento_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlidTipoEvento.SelectedValue == "1")
            {
                div_projeto.Visible = true;
            }
            else
            {
                div_projeto.Visible = false;
            }
        }
        #endregion

        #region | Aba Arquivo 

        void Popular_Arquivo(string idPesquisa)
        {
            frmArquivos.Attributes.Add("src", string.Format("~/App/Paginas/Arquivos.aspx?idObjeto={0}&sTipoObjeto={1}", idPesquisa, "Ocorrencias"));
            frmArquivos.Visible = true;
            DIV_Arquivos.Visible = true;
            aba_Arquivo.Visible = true;
        }

        #endregion

        protected void PopularCombo_Tipo()
        {
            ddlTipo.Items.Clear();
            ddlTipo.Items.Add(new ListItem("Informe o Tipo", ""));
            ddlTipo.Items.Add(new ListItem("Elogio", "E"));
            ddlTipo.Items.Add(new ListItem("Ocorrência", "O"));
            ddlTipo.Items.Add(new ListItem("Advertência", "A"));
        }
        
    }
}