using HtmlAgilityPack;
using NPOI.HSSF.UserModel;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common.CommandTrees.ExpressionBuilder;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.FrameWork;
using static TT.FrameWork.BD;
using Funcoes = TT.FrameWork.Funcoes;
using Identity = TT.FrameWork.Identity;

namespace TT_Flow.App.Paginas.Adm.Financeiro
{
    public partial class Extrato_Bancario : System.Web.UI.Page
    {
        string sTituloPagina = "Extrato Bancário";
        string sProcedure = "sp_Manipula_tbl_Flow_Adm_Extrato_Bancario";
        int tudoOk = 0;

        #region| Construtor

        public List<cls_ConciliacaoComposta> bs_ConciliacaoComposta
        {
            get
            {
                if (ViewState["bs_ConciliacaoComposta"] == null)
                {
                    ViewState["bs_ConciliacaoComposta"] = new List<cls_ConciliacaoComposta>();
                }
                return (List<cls_ConciliacaoComposta>)ViewState["bs_ConciliacaoComposta"];
            }
            set
            {
                ViewState["bs_ConciliacaoComposta"] = value;
            }
        }

        public List<cls_Extrato> bs_Extrato
        {
            get
            {
                if (ViewState["bs_Extrato"] == null)
                {
                    ViewState["bs_Extrato"] = new List<cls_Extrato>();
                }
                return (List<cls_Extrato>)ViewState["bs_Extrato"];
            }
            set
            {
                ViewState["bs_Extrato"] = value;
            }
        }       

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Funcoes.ValidaPermissao(Permissao.Financeiro.ExtratoBancario.Consultar, true);
            if (!Funcoes.ValidaPermissao(Permissao.Financeiro.ExtratoBancario.Importar))
            {
                btnImportar.Visible = false;
            }

            //Manual Usuario
            manual.sNomeArquivo = "Manual_ExtratoBancario.pdf";

            if (!IsPostBack)
            {
                BreadCrumb_Pagina.TitulodaPagina = sTituloPagina;
                pnResultado.Visible = false;

                Funcoes.Popula_Combo(ddlConta, "sp_Select 'tbl_Flow_Adm_ContasBancarias'", "idConta", "sDscConta", false, "Todos as Contas", "0");
                Funcoes.Popula_Combo(ddlMesAnoExtrato, "sp_Select 'Flow_Mes_Extrato'", "MesAno", "MesAnoCompleto", false, "Todos os Mêses/Ano", "0");

                if (!string.IsNullOrEmpty(Request.QueryString["grid"]))
                {
                    hddMudaGrid.Value = Request.QueryString["grid"];
                }

                if (!string.IsNullOrEmpty(Request.QueryString["extrato"]))
                {
                    hddidExtrato.Value = Request.QueryString["extrato"];
                }
                ddlTipoConsulta.SelectedValue = "1";
                ddlTipoConsulta_SelectedIndexChanged(null, null);
                //Pesquisar();
            }
            RegistraScript();
        }

        protected override void OnPreRender(EventArgs e)
        {
            RegistrarLinksDetalheComoAssincronos();
            base.OnPreRender(e);
        }

        private void RegistrarLinksDetalheComoAssincronos()
        {
            ScriptManager scriptManager = ScriptManager.GetCurrent(Page);
            if (scriptManager == null)
            {
                return;
            }

            foreach (GridViewRow row in dtgvConsultaExtrato.Rows)
            {
                LinkButton accountLink = row.FindControl("btnsDscConta") as LinkButton;
                LinkButton entryLink = row.FindControl("btnsDscLancamento") as LinkButton;

                if (accountLink != null)
                {
                    scriptManager.RegisterAsyncPostBackControl(accountLink);
                }

                if (entryLink != null)
                {
                    scriptManager.RegisterAsyncPostBackControl(entryLink);
                }
            }
        }

        protected void ddlTipoConsulta_SelectedIndexChanged(object sender, EventArgs e)
        {
            div_Selecao_Conciliado.Visible = false;
            div_Selecao_Conta.Visible = false;
            div_Selecao_Periodo.Visible = false;
            div_Selecao_PeriodoMes.Visible = false;
            Div_Selecao_TipoLancamento.Visible = false;
            btnExportarExcel.Visible = false;
            ddlTipoLancamento.SelectedValue = "T";
            ddlConciliado.SelectedValue = "T";
            hddMudaGrid.Value = ddlTipoConsulta.SelectedValue;
            if (ddlTipoConsulta.SelectedValue == "1") //Detalhe
            {
                div_Selecao_Conta.Visible = true;
                div_Selecao_Periodo.Visible = true;
                div_Selecao_Conciliado.Visible = true;
                btnImportar.Visible = false;
                div_detalheExtrato.Visible = false;
                div_Extrato.Visible=false;
                Div_Selecao_TipoLancamento.Visible = true;
                hddidExtrato.Value = "0";


                txtdtFinal.Text = DateTime.Today.ToString("u").Substring(0, 10);
                txtdtInicial.Text = DateTime.Today.ToString("u").Substring(0, 08) + "01";
            }
            else
            {
                
                div_Selecao_PeriodoMes.Visible = true;
                div_Selecao_Conta.Visible = true;
                btnImportar.Visible = true; 
                Pesquisar();
            }
        }

        protected void Pesquisar()
        {
            try
            {
                pnResultado.Visible = false;
                string dtInicial = "";
                string dtFinal = "";
                if (Validacoes.ValidarData(txtdtInicial))
                {
                    dtInicial = DateTime.ParseExact(txtdtInicial.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                }
                if (Validacoes.ValidarData(txtdtInicial))
                {
                    dtFinal = DateTime.ParseExact(txtdtFinal.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                }

                DataSet ds;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", hddMudaGrid.Value == "1" ? "CONSULTAR_LANCAMENTOS" : "CONSULTAR_ARQUIVOS");
                //vParametros.Add("@sDscLancamento", txtsDescricao.Text);
                vParametros.Add("@idConta", ddlConta.SelectedValue);
                vParametros.Add("@dtInicial", dtInicial);
                vParametros.Add("@dtFinal", dtFinal);
                vParametros.Add("@sConciliado", ddlConciliado.SelectedValue);
                vParametros.Add("@sTipoLancamento", ddlTipoLancamento.SelectedValue);
                vParametros.Add("@idExtrato", hddidExtrato.Value);
                vParametros.Add("@sPeriodo", ddlMesAnoExtrato.SelectedValue);

                ds = BD.ExecutarDataSet(sProcedure, vParametros, false);

                if (ds.Tables.Count != 0)
                {

                    if (hddMudaGrid.Value == "1")
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            pnResultado.Visible = true;
                            btnExportarExcel.Visible = true;
                            div_Extrato.Visible = false;
                            div_detalheExtrato.Visible = true;
                            btnVoltarGrid.Visible = true;
                            div_btnVoltar.Visible = true;
                            div_Selecao_Periodo.Visible = true;
                            div_Selecao_PeriodoMes.Visible = false;
                            div_Selecao_Conciliado.Visible = true;
                            if (hddidExtrato.Value == "0")
                            {
                                div_Selecao_Conta.Visible = true;
                            }
                            else
                            {
                                div_Selecao_Conta.Visible = false;
                            }


                            ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScriptData(dtgvConsultaExtrato, ds.Tables[0], 0, "asc", "false", "''"), true);    /*Agnes Partal * 07/08/2024*/

                        }
                        else
                        {
                            pnResultado.Visible = false;
                            MensagemPaginaConsulta.MostraMensagem_Erro("Nenhum registro localizado!");
                        }
                    }
                    else
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            btnExportarExcel.Visible = false;
                            div_Extrato.Visible = true;
                            div_detalheExtrato.Visible = false;
                            div_btnVoltar.Visible = false;
                            div_Selecao_Periodo.Visible = false;
                            div_Selecao_PeriodoMes.Visible = true;
                            div_Selecao_Conciliado.Visible = false;
                            div_Selecao_Conta.Visible = true;
                            //txtsDescricao.Attributes["placeholder"] = "Extrato";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables2", TT.FrameWork.Grid.DataBindComScriptData(gv_Extrato, ds.Tables[0], 2, "desc", "false", "''"), true);    /*Agnes Partal * 07/08/2024*/
                            pnResultado.Visible = true;
                        }
                        else
                        {
                            pnResultado.Visible = false;
                            MensagemPaginaConsulta.MostraMensagem_Erro("Nenhum registro localizado!");
                        }

                    }
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                MensagemPagina.MostraMensagem_Erro("Erro ao Consultar: " + ex.Message);

            }

        }

        protected void dtgvConsultaExtrato_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int nColuna_Credito = 5;
            int nColuna_Debito = 6;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string idLancamento = DataBinder.Eval(e.Row.DataItem, "idLancamento").ToString();
                e.Row.Attributes["data-id-lancamento"] = idLancamento;

                //e.Row.Cells[nColuna_Credito].ForeColor = System.Drawing.Color.Blue;
                //e.Row.Cells[nColuna_Debito].ForeColor = System.Drawing.Color.Red;


                Literal litConciliacao = e.Row.FindControl("litConciliacao") as Literal;
                if (litConciliacao != null)
                {
                    string sConciliado = DataBinder.Eval(e.Row.DataItem, "sConciliado").ToString();


                    if (sConciliado == "S")
                    {
                        litConciliacao.Text = "<i class='fa fa-check'></i>";
                    }
                    else
                    {
                        litConciliacao.Text = string.Empty;
                    }
                }

                var creditoCell = e.Row.Cells[nColuna_Credito];
                var debitoCell = e.Row.Cells[nColuna_Debito];

                if (Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "nCredito")) == 0)
                {
                    creditoCell.Text = string.Empty;
                }

                if (Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "nDebito")) == 0)
                {
                    debitoCell.Text = string.Empty;
                }


            }
        }

        protected void btnImportar_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalNovo", "$('#modalImportarExtrato').modal('show');", true);
        }

        protected void btnPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }

        protected void LinkButton_Command(object sender, CommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();
            LancamentoDetalhe(id);
            hddidLancamento.Value = id;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalDetalhe", "$('#modalLancamentoDetalhe').modal('show');", true);
        }

        protected void cmd_Extrato(object sender, CommandEventArgs e)
        {
            hddidExtrato.Value = e.CommandArgument.ToString();
            hddMudaGrid.Value = "1";
            div_Selecao_Tipo.Visible = false;
            btnImportar.Visible = false;
            txtdtInicial.Text = "";
            txtdtFinal.Text = "";
            Pesquisar();
        }

        protected void cmdEnviarArquivos_Click(object sender, EventArgs e)
        {
            if (fu_EnviarArquivo.HasFile && ddlBanco.SelectedValue != "0")
            {
                string fileName = fu_EnviarArquivo.FileName;
                string fileExtension = Path.GetExtension(fileName).ToLower();


                if (fileExtension != ".csv" && fileExtension != ".xlsx" && fileExtension != ".xls")
                {

                    MensagemPaginaModalImportar.MostraMensagem_Erro("Formato de arquivo inválido. Por favor, envie um arquivo .csv, .xls ou .xlsx");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalInvalidFile", "$('#modalImportarExtrato').modal('show');", true);
                    return;
                }
                try
                {

                    // Ler o arquivo e converter para array de bytes
                    byte[] fileBytes = fu_EnviarArquivo.FileBytes;


                    if (fileExtension == ".csv") // BRADESCO
                    {
                        ProcessaArquivoCsv(fu_EnviarArquivo.FileContent);
                    }
                    else if (fileExtension == ".xlsx" || fileExtension == ".xls") // BANCO DO BRASIL ; ITAU ; SANTANDER
                    {
                        ProcessaArquivo(fu_EnviarArquivo.FileContent);
                    }

                    if (tudoOk == 0)
                    {
                        SalvaArquivoExtrato(fileBytes, fileName);
                        MensagemPaginaModalImportar.MostraMensagem_Sucesso("Extrato Importado com sucesso.");
                    }
                    else if (tudoOk == 2)
                    {                        
                        SalvaArquivoExtrato(fileBytes, fileName);
                    }
                }
                catch (Exception ex)
                {
                    MensagemPaginaModalImportar.MostraMensagem_Erro("Houve um erro na importação do arquivo! Não foi possível ler o arquivo", false);
                    //MensagemPaginaModalImportar.MostraMensagem_Erro("Houve um erro na importação do arquivo! <br /> Erro: " + ex.Message, false);
                }
            }
            else
            {
                MensagemPaginaModalImportar.MostraMensagem_Erro("Selecione um Banco e/ou Insira um arquivo");

            }

            hddidExtrato.Value = tudoOk == 2 ? hddidExtrato.Value : "";
            ddlBanco.SelectedValue = "0";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalPostBack", "$('#modalImportarExtrato').modal('show');", true);
        }

        private void ProcessaArquivo(Stream fileStream)
        {
            fileStream.Position = 0;

            byte[] header = new byte[8];
            fileStream.Read(header, 0, 8);

            fileStream.Position = 0;

            string headerString = BitConverter.ToString(header);

            if (headerString.StartsWith("50-4B-03-04")) // Arquivo .xlsx (zip)
            {
                ProcessaArquivoExcel(fileStream, headerString);
            }
            else if (headerString.StartsWith("D0-CF-11-E0")) // Arquivo .xls (OLE2)
            {
                ProcessaArquivoExcel(fileStream, headerString);
            }
            else if (headerString.StartsWith("3C-68-74-6D-6C")) // Arquivo HTML (começa com <html>)
            {
                ProcessaArquivoHtml(fileStream);
            }
            else if (headerString.StartsWith("3C-3F-78-6D-6C")) // Arquivo XML (começa com <?xml)
            {
                MensagemPaginaModalImportar.MostraMensagem_Erro("Tipo de arquivo não suportado!");
                //MensagemPaginaModalImportar.MostraMensagem_Erro("Tipo de arquivo não suportado:" + headerString);
                tudoOk = 1;
            }
            else
            {
                throw new InvalidDataException("Tipo de arquivo não suportado");
            }
        }

        private void ProcessaArquivoExcel(Stream fileStream, string headerString)
        {            
            IWorkbook workbook;

            try
            {
                fileStream.Position = 0;

                byte[] header = new byte[8];
                fileStream.Read(header, 0, 8);

                fileStream.Position = 0;

                if (headerString.StartsWith("50-4B-03-04"))
                {
                    workbook = new XSSFWorkbook(fileStream);
                }
                else if (headerString.StartsWith("D0-CF-11-E0"))
                {
                    POIFSFileSystem fs = new POIFSFileSystem(fileStream);
                    workbook = new HSSFWorkbook(fs);
                }
                else
                {
                    throw new InvalidDataException("Tipo de arquivo não suportado.");
                }

                if (ddlBanco.SelectedValue == "1") // BANCO DO BRASIL
                {
                    var sheet = workbook.GetSheetAt(0);
                    var rows = sheet.GetRowEnumerator();
                    rows.MoveNext();

                    string agencia = "";
                    string conta = "";

                    while (rows.MoveNext())
                    {
                        var row = (IRow)rows.Current;

                        if (row.RowNum == 1)
                        {
                            agencia = row.GetCell(1).ToString().Trim();
                            conta = row.GetCell(3).ToString().Substring(6).Trim();
                            continue;
                        }

                        if (row.GetCell(0).ToString() == "Data")
                        {
                            continue;
                        }
                        if (row.GetCell(6).ToString() != "999")
                        {
                            cls_Extrato objItem = new cls_Extrato();
                            objItem.dtLancamento = row.GetCell(0).ToString().Trim();
                            objItem.sDscLancamento = row.GetCell(7).ToString().Trim();
                            objItem.sNumeroDocumento = row.GetCell(5).ToString().Trim();
                            objItem.sSaldoAnterior = row.GetCell(6).ToString() == "000" ? row.GetCell(8).ToString().Trim() : "0";
                            objItem.sCredito = row.GetCell(9).ToString() == "C" ? row.GetCell(8).ToString().Trim() : "0";
                            objItem.sDebito = row.GetCell(9).ToString() == "D" ? row.GetCell(8).ToString().Trim() : "0";
                            objItem.sObservacao = row.GetCell(10).ToString();
                            objItem.sAgencia = agencia;
                            objItem.sNumeroConta = conta;
                            objItem.sSaldoAtual = "";
                            
                            bs_Extrato.Add(objItem);
                            
                        }
                        else
                        {
                            cls_Extrato objItem = new cls_Extrato();
                            objItem.dtLancamento = "";
                            objItem.sDscLancamento = "Saldo Atual";
                            objItem.sNumeroDocumento = "";
                            objItem.sSaldoAnterior = "";
                            objItem.sCredito = "";
                            objItem.sDebito = "";
                            objItem.sObservacao = "";
                            objItem.sAgencia = "";
                            objItem.sNumeroConta = "";
                            objItem.sSaldoAtual = row.GetCell(8).ToString().Trim();
                            
                            bs_Extrato.Add(objItem);

                        continue;
                        }
                    }

                }
                else if (ddlBanco.SelectedValue == "3") // ITAU
                {
                    var sheet = workbook.GetSheetAt(0);
                    var rows = sheet.GetRowEnumerator();
                    rows.MoveNext();

                    string agencia = "";
                    string conta = "";                   

                    while (rows.MoveNext())
                    {
                        var row = (IRow)rows.Current;

                        if (row.RowNum <= 9)
                        {
                            if (row.RowNum <= 2)
                            {
                                continue;
                            }

                            if (row.RowNum == 3)
                            {
                                agencia = row.GetCell(1).ToString().Trim();
                                continue;
                            }

                            if (row.RowNum == 4)
                            {
                                conta = row.GetCell(1).ToString().Trim();
                                continue;
                            }                            

                            continue;

                        }
                        else if (row.RowNum > 9)
                        {
                            if (row.GetCell(1).ToString().Trim() == "REND PAGO APLIC AUT MAIS" || row.GetCell(1).ToString().Trim() == "SALDO TOTAL DISPONÍVEL DIA")
                            {
                                continue;
                            }

                            if (row.GetCell(1).ToString().Trim() != "SALDO ANTERIOR")
                            {
                                cls_Extrato objItem = new cls_Extrato();
                                objItem.dtLancamento = row.GetCell(0).ToString().Trim();
                                objItem.sDscLancamento = row.GetCell(1).ToString().Trim() == "SALDO EM CONTA CORRENTE" ? "SALDO ANTERIOR" : row.GetCell(1).ToString().Trim();
                                objItem.sNumeroDocumento = "";
                                objItem.sSaldoAnterior = row.GetCell(1).ToString() == "SALDO EM CONTA CORRENTE" ? row.GetCell(5).ToString().Trim() : "0";
                                objItem.sCredito = !row.GetCell(4).ToString().StartsWith("-") ? row.GetCell(4).ToString().Trim() : "0";
                                objItem.sDebito = row.GetCell(4).ToString().StartsWith("-") ? row.GetCell(4).ToString().Substring(1).Trim() : "0";
                                objItem.sObservacao = "";
                                objItem.sAgencia = agencia;
                                objItem.sNumeroConta = conta;
                                objItem.sSaldoAtual = "";
                                
                                bs_Extrato.Add(objItem);                           

                            }
                            else
                            {
                                cls_Extrato objItem = new cls_Extrato();
                                objItem.dtLancamento = "";
                                objItem.sDscLancamento = "Saldo Atual";
                                objItem.sNumeroDocumento = "";
                                objItem.sSaldoAnterior = "";
                                objItem.sCredito = "";
                                objItem.sDebito = "";
                                objItem.sObservacao = "";
                                objItem.sAgencia = "";
                                objItem.sNumeroConta = "";
                                objItem.sSaldoAtual = row.GetCell(5).ToString().Trim();
                                
                                bs_Extrato.Add(objItem);

                                continue;
                            }
                        }
                    }
                }
                else if (ddlBanco.SelectedValue == "4") // SANTANDER
                {
                    var sheet = workbook.GetSheetAt(0);
                    var rows = sheet.GetRowEnumerator();

                    string agencia = "";
                    string conta = "";

                    while (rows.MoveNext())
                    {
                        var row = (IRow)rows.Current;

                        if (row.RowNum == 0)
                        {
                            agencia = row.GetCell(1).ToString().Trim();
                            conta = row.GetCell(3).ToString().Trim();
                            continue;
                        }

                        if (row.Cells.Count == 0)
                        {
                            continue;
                        }
                        else if (row.GetCell(0).ToString() == "")
                        {
                            continue;
                        }

                        if (row.GetCell(0).ToString() == "Data")
                        {
                            continue;
                        }

                        if(row.RowNum == sheet.LastRowNum)
                        {
                            cls_Extrato objItem = new cls_Extrato();
                            objItem.dtLancamento = row.GetCell(0).ToString().Trim();
                            objItem.sDscLancamento = row.GetCell(2).ToString().Trim();
                            objItem.sNumeroDocumento = row.GetCell(3).ToString().Trim();
                            objItem.sSaldoAnterior = row.GetCell(2).ToString() == "SALDO ANTERIOR" || bs_Extrato.Count == 0 ? row.GetCell(5).ToString().Trim() : "0";
                            objItem.sCredito = !row.GetCell(4).ToString().StartsWith("-") ? row.GetCell(4).ToString().Trim() : "0";
                            objItem.sDebito = row.GetCell(4).ToString().StartsWith("-") ? row.GetCell(4).ToString().Substring(1).Trim() : "0";
                            objItem.sObservacao = "";
                            objItem.sAgencia = agencia;
                            objItem.sNumeroConta = conta;
                            objItem.sSaldoAtual = "";
                            
                            bs_Extrato.Add(objItem);

                            cls_Extrato objItem2 = new cls_Extrato();
                            objItem2.dtLancamento = row.GetCell(0).ToString().Trim();
                            objItem2.sDscLancamento = "Saldo Atual";
                            objItem2.sNumeroDocumento = "";
                            objItem2.sSaldoAnterior = "0";
                            objItem2.sCredito = "0";
                            objItem2.sDebito = "0";
                            objItem2.sObservacao = "";
                            objItem2.sAgencia = agencia;
                            objItem2.sNumeroConta = conta;
                            objItem2.sSaldoAtual = row.GetCell(5).ToString().Trim() != "" ? row.GetCell(5).ToString().Trim() : "";

                            bs_Extrato.Add(objItem2);

                        }
                        else
                        {
                            cls_Extrato objItem = new cls_Extrato();
                            objItem.dtLancamento = row.GetCell(0).ToString().Trim();
                            objItem.sDscLancamento = row.GetCell(2).ToString().Trim();
                            objItem.sNumeroDocumento = row.GetCell(3).ToString().Trim();
                            objItem.sSaldoAnterior = row.GetCell(2).ToString() == "SALDO ANTERIOR" || bs_Extrato.Count == 0 ? row.GetCell(5).ToString().Trim() : "0";
                            objItem.sCredito = !row.GetCell(4).ToString().StartsWith("-") ? row.GetCell(4).ToString().Trim() : "0";
                            objItem.sDebito = row.GetCell(4).ToString().StartsWith("-") ? row.GetCell(4).ToString().Substring(1).Trim() : "0";
                            objItem.sObservacao = "";
                            objItem.sAgencia = agencia;
                            objItem.sNumeroConta = conta;
                            objItem.sSaldoAtual = "";
                            
                            bs_Extrato.Add(objItem);
                        }

                    }
                }
                else if (ddlBanco.SelectedValue == "5") // DAYCOVAL
                {
                    var sheet = workbook.GetSheetAt(0);
                    var rows = sheet.GetRowEnumerator();

                    string agencia = "";
                    string conta = "";

                    while (rows.MoveNext())
                    {
                        var row = (IRow)rows.Current;

                        if (row.RowNum < 2)
                        {                            
                            continue;
                        }

                        if (row.RowNum == 2)
                        {
                            string celAgencia = row.GetCell(0).ToString();
                            string[] arrayHeader = celAgencia.Split(':');
                            agencia = arrayHeader[1].Substring(0, 5).Trim();
                            conta = arrayHeader[2].Trim();
                            continue;
                        }
                        else if (row.RowNum < 6)
                        {
                            continue;
                        }

                        if (row.GetCell(0).ToString() == "Data")
                        {
                            continue;
                        }

                        if(row.RowNum == sheet.LastRowNum)
                        {
                            cls_Extrato objItem = new cls_Extrato();
                            objItem.dtLancamento = DateTime.TryParseExact(row.GetCell(0).ToString().Trim(), "d-MMM", new CultureInfo("pt-BR"), DateTimeStyles.None, out DateTime dataConvertida) ? dataConvertida.ToString() : "1900-01-01";
                            objItem.sDscLancamento = row.GetCell(2).ToString().Trim();
                            objItem.sNumeroDocumento = row.GetCell(1).ToString().Trim();
                            objItem.sSaldoAnterior = row.GetCell(2).ToString() == "SALDO ANTERIOR" || bs_Extrato.Count == 0 ? row.GetCell(5).ToString().Trim() : "0";
                            objItem.sCredito = !row.GetCell(4).ToString().StartsWith("-") && row.GetCell(4).ToString().Count() > 2 ? row.GetCell(4).ToString().Trim() : "0";
                            objItem.sDebito = row.GetCell(3).ToString().StartsWith("-") && row.GetCell(3).ToString().Count() > 2 ? row.GetCell(3).ToString().Substring(1).Trim() : "0";
                            objItem.sObservacao = "";
                            objItem.sAgencia = agencia;
                            objItem.sNumeroConta = conta;
                            objItem.sSaldoAtual = "";
                            
                            bs_Extrato.Add(objItem);

                            cls_Extrato objItem2 = new cls_Extrato();
                            objItem2.dtLancamento = DateTime.TryParseExact(row.GetCell(0).ToString().Trim(), "d-MMM", new CultureInfo("pt-BR"), DateTimeStyles.None, out DateTime dataConvertidaSaldo) ? dataConvertidaSaldo.ToString() : "1900-01-01";
                            objItem2.sDscLancamento = "Saldo Atual";
                            objItem2.sNumeroDocumento = "";
                            objItem2.sSaldoAnterior = "0";
                            objItem2.sCredito = "0";
                            objItem2.sDebito = "0";
                            objItem2.sObservacao = "";
                            objItem2.sAgencia = agencia;
                            objItem2.sNumeroConta = conta;
                            objItem2.sSaldoAtual = row.GetCell(5).ToString().Trim() != "" ? row.GetCell(5).ToString().Trim() : "";

                            bs_Extrato.Add(objItem2);

                        }
                        else
                        {                            
                            cls_Extrato objItem = new cls_Extrato();
                            objItem.dtLancamento = DateTime.TryParseExact(row.GetCell(0).ToString().Trim(), "d-MMM", new CultureInfo("pt-BR"), DateTimeStyles.None, out DateTime dataConvertida) ? dataConvertida.ToString() : "1900-01-01";
                            objItem.sDscLancamento = row.GetCell(2).ToString().Trim();
                            objItem.sNumeroDocumento = row.GetCell(1).ToString().Trim();
                            objItem.sSaldoAnterior = row.GetCell(2).ToString().ToUpper() == "SALDO ANTERIOR" || bs_Extrato.Count == 0 ? row.GetCell(5).ToString().Trim() : "0";
                            objItem.sCredito = !row.GetCell(4).ToString().StartsWith("-") && row.GetCell(4).ToString().Count() > 2 ? row.GetCell(4).ToString().Trim() : "0";
                            objItem.sDebito = row.GetCell(3).ToString().StartsWith("-") && row.GetCell(3).ToString().Count() > 2? row.GetCell(3).ToString().Substring(1).Trim() : "0";
                            objItem.sObservacao = "";
                            objItem.sAgencia = agencia;
                            objItem.sNumeroConta = conta;
                            objItem.sSaldoAtual = "";

                            bs_Extrato.Add(objItem);
                            
                        }

                    }
                }
                else
                {
                    tudoOk = 1;
                    MensagemPaginaModalImportar.MostraMensagem_Erro("Formato inválido para os extratos desse banco");
                }

                if (tudoOk == 0)
                {
                    SalvaLancamento();
                }
            }
            catch (Exception ex)
            {
                MensagemPaginaModalImportar.MostraMensagem_Erro("<b>Erro ao processar informações do arquivo! Arquivo fora do padrão</b><br />Verifique se o arquivo está visualmente igual ao com o último arquivo importado, desse mesmo banco, caso tenha diferenças verifique se não há outra opção na hora de exportar o arquivo do banco, para que o mesmo esteja igual aos arquivos já importados no sistema, caso não haja diferença visual envie um e-mail para o setor T.I com o arquivo em anexo.");
                //MensagemPaginaModalImportar.MostraMensagem_Erro("Erro ao processar o arquivo Excel: " + ex.Message);
                tudoOk = 1;
            }

        }

        private void ProcessaArquivoHtml(Stream fileStream)
        {            
            try
            {
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.Load(fileStream);

                var tableRows = htmlDoc.DocumentNode.SelectNodes("//table//tr");
                string agencia = "";
                string conta = "";
                string ano = null;
                int contador = 0;
                foreach (var row in tableRows)
                {
                    var cells = row.SelectNodes("td");
                    if (cells != null)
                    {
                        if (contador == 1 || contador == 4)
                        {
                            foreach (var cell in cells)
                            {
                                string cellText = cell.InnerText.Trim();

                                //if (cellText.Contains("Agência/Conta:"))
                                //{
                                //    string[] parts = cellText.Split(new string[] { "Agência/Conta:", "&nbsp;" }, StringSplitOptions.RemoveEmptyEntries);

                                //    if (parts.Length > 0)
                                //    {
                                //        string agenciaConta = parts[0].Trim();

                                //        string[] agenciaContaSplit = agenciaConta.Split('/');
                                //        if (agenciaContaSplit.Length == 2)
                                //        {
                                //            agencia = agenciaContaSplit[0].Trim();
                                //            conta = agenciaContaSplit[1].Trim();
                                //        }
                                //    }
                                //}
                                //else 
                                if (cellText.Contains("Extrato de"))
                                {
                                    string[] parts = cellText.Split('/');
                                    ano = parts[4].Trim();
                                }

                            }
                            contador++;
                            continue;
                        }

                        if (contador <= 6)
                        {
                            contador++;
                            continue;
                        }

                        if (!string.IsNullOrEmpty(cells[1].InnerText.Trim()) && !string.IsNullOrEmpty(cells[4].InnerText.Trim()))
                        {
                            if (tableRows.Count - 1 != contador)
                            {
                                if ((cells[4].InnerText.Trim() == "SALDO ANTERIOR" || cells[4].InnerText.Trim() == "SALDO PARCIAL" || cells[4].InnerText.Trim() == "SDO CTA/APL AUTOMATICAS" || cells[4].InnerText.Trim() == "REND PAGO APLIC AUT MAIS" || cells[4].InnerText.Trim() == "S A L D O") && contador != 7)
                                {
                                    contador++;
                                    continue;
                                }

                                cls_Extrato objItem = new cls_Extrato();

                                objItem.dtLancamento = cells[1].InnerText.Trim() + "/" + ano;
                                objItem.sDscLancamento = cells[4].InnerText.Trim() == "SALDO INICIAL" || cells[4].InnerText.Trim() == "SALDO ANTERIOR" ? "SALDO ANTERIOR" : cells[4].InnerText.Trim();
                                objItem.sNumeroDocumento = "";
                                objItem.sSaldoAnterior = cells[4].InnerText.Trim() == "SALDO INICIAL" || cells[4].InnerText.Trim() == "SALDO ANTERIOR" ? cells[7].InnerText.Trim() : "0";
                                objItem.sCredito = !cells[6].InnerText.StartsWith("-") ? cells[6].InnerText.Trim() : "0";
                                objItem.sDebito = cells[6].InnerText.StartsWith("-") ? cells[6].InnerText.Substring(1).Trim() : "0";
                                objItem.sObservacao = "";
                                objItem.sAgencia = agencia;
                                objItem.sNumeroConta = conta;
                                objItem.sSaldoAtual = "";

                                bs_Extrato.Add(objItem);
                            }
                            else
                            {
                                cls_Extrato objItem = new cls_Extrato();
                                objItem.dtLancamento = "";
                                objItem.sDscLancamento = "Saldo Atual";
                                objItem.sNumeroDocumento = "";
                                objItem.sSaldoAnterior = "";
                                objItem.sCredito = "";
                                objItem.sDebito = "";
                                objItem.sObservacao = "";
                                objItem.sAgencia = "";
                                objItem.sNumeroConta = "";
                                objItem.sSaldoAtual = cells[7].InnerText.Trim();
                                
                                bs_Extrato.Add(objItem);

                            }
                        }
                    }
                    contador++;
                }

                if (tudoOk == 0)
                {
                    SalvaLancamento();
                }
            }
            catch (Exception ex)
            {
                MensagemPaginaModalImportar.MostraMensagem_Erro("Erro ao processar informações do arquivo! Arquivo fora do padrão");
                //MensagemPaginaModalImportar.MostraMensagem_Erro("Erro ao processar o arquivo HTML: " + ex.Message);
                tudoOk = 1;
            }
        }

        private void ProcessaArquivoCsv(Stream fileStream)
        {
            if (ddlBanco.SelectedValue == "2")
            {                
                try
                {

                    using (StreamReader sr = new StreamReader(fileStream))
                    {
                        DataTable dt = new DataTable();

                        int numberOfColumns = 6;
                        for (int i = 0; i < numberOfColumns; i++)
                        {
                            dt.Columns.Add();
                        }

                        bool isFirstRow = true;

                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();

                            if (isFirstRow && string.IsNullOrWhiteSpace(line))
                            {
                                isFirstRow = false;
                                continue;
                            }

                            var values = line.Split(';');

                            DataRow row = dt.NewRow();
                            for (int i = 0; i < values.Length; i++)
                            {
                                row[i] = values[i];
                            }
                            dt.Rows.Add(row);
                        }

                        bool isHeader = true;
                        string agencia = "";
                        string conta = "";
                        //int investimento = 0;

                        foreach (DataRow row in dt.Rows)
                        {
                            if (isHeader)
                            {
                                string colunaHeader = row[1].ToString();
                                string[] arrayHeader = colunaHeader.Split(':');
                                agencia = arrayHeader[2].Substring(0, 5).Trim();
                                conta = arrayHeader[3].Substring(0, 9).Trim();

                                if (agencia != "" || conta != "")
                                {
                                    isHeader = false;
                                    continue;
                                }
                                else
                                {
                                    MensagemPaginaModalImportar.MostraMensagem_Erro("Agência e Conta não encontrada");
                                    break;
                                }

                            }

                            if (row[0].ToString() == "Data" || row[0].ToString() == "Total" || row[0].ToString() == "")
                            {
                                if (row[0].ToString() == "Total")
                                {
                                    cls_Extrato objItem = new cls_Extrato();

                                    objItem.dtLancamento = "";
                                    objItem.sDscLancamento = "Saldo Atual";
                                    objItem.sNumeroDocumento = "";
                                    objItem.sSaldoAnterior = "";
                                    objItem.sCredito = "";
                                    objItem.sDebito = "";
                                    objItem.sObservacao = "";
                                    objItem.sAgencia = "";
                                    objItem.sNumeroConta = "";
                                    objItem.sSaldoAtual = row[5].ToString().Trim();
                                    
                                    bs_Extrato.Add(objItem);                               

                                    break;
                                }
                                //if (row[0].ToString() == "Data" && row[2].ToString() == "Valor (R$)")
                                //{
                                //    investimento = 1;
                                //}
                                continue;
                            }
                            else
                            {
                                //if (investimento == 0)

                                cls_Extrato objItem = new cls_Extrato();
                                objItem.dtLancamento = row[0].ToString() != "" ? row[0].ToString().Trim() : "";
                                objItem.sDscLancamento = row[1].ToString() != "" ? row[1].ToString().Trim() : "";
                                objItem.sNumeroDocumento = row[2].ToString() != "" ? row[2].ToString().Trim() : "";
                                objItem.sCredito = row[3].ToString() != "" ? row[3].ToString().Trim() : "0";
                                objItem.sDebito = row[4].ToString() != "" ? row[4].ToString().Replace("-", "").Trim() : "0";
                                objItem.sSaldoAnterior = row[1].ToString() == "SALDO ANTERIOR" ? row[5].ToString().Trim() : "0";
                                objItem.sObservacao = "";
                                objItem.sAgencia = agencia;
                                objItem.sNumeroConta = conta;
                                objItem.sSaldoAtual = "";
                                bs_Extrato.Add(objItem);
                            }

                        }
                    }
                    if (bs_Extrato.Count() == 0)
                    {
                        tudoOk = 1;
                        MensagemPaginaModalImportar.MostraMensagem_Erro("Não foi possível ler o arquivo CSV.");
                    }
                    else if (bs_Extrato.Count() > 0)
                    {
                        SalvaLancamento();
                    }
                    else
                    {
                        tudoOk = 1;
                        MensagemPaginaModalImportar.MostraMensagem_Erro("Erro na leitura do arquivo");
                    }
                }
                catch (Exception ex)
                {
                    //MensagemPaginaModalImportar.MostraMensagem_Erro("Erro ao ler o arquivo: " + ex.Message);
                    MensagemPaginaModalImportar.MostraMensagem_Erro("Erro ao processar informações do arquivo! Arquivo fora do padrão");
                    tudoOk = 1;
                }

            }
            else
            {
                tudoOk = 1;
                MensagemPaginaModalImportar.MostraMensagem_Erro("Formato inválido para os extratos desse banco");
            }

        }

        private void SalvaArquivoExtrato(byte[] fileBytes, string fileName)
        {
            if (hddValido.Value == "0")
            {
                TT_Flow.FrameWork.cls_Arquivos Arquivo = new FrameWork.cls_Arquivos
                {
                    idTipoArquivo = 302,
                    idObjeto = Convert.ToInt32(hddidExtrato.Value),
                    sNomeArquivo = fileName,
                    sDscArquivo = "Extrato - " + ddlBanco.SelectedItem.Text,
                    sObservacao = "",
                    idUsuario = Convert.ToInt32(Identity.Variaveis.idUsuario()),
                    vbArquivo = fileBytes,
                    dtExpiracaoDoc = "",
                    dtRegistroDoc = DateTime.Now.ToString()
                };

                Arquivo.EnviarArquivo(Arquivo);
                File.WriteAllBytes(Server.MapPath("~/Download/") + fileName, fileBytes);
            }
        }

        private void SalvaLancamento()
        {
            string sErro = "";
            DataSet ds;
            Dictionary<String, String> vParametrosConsulta_idExtrato = new Dictionary<string, string>();
            vParametrosConsulta_idExtrato.Add("@sFuncao", "CONSULTA_EXTRATO");
            ds = BD.ExecutarDataSet(sProcedure, vParametrosConsulta_idExtrato);
            if (BD.ValidarDataSet(ds, out sErro))
            {
                hddidExtrato.Value = Retorno.DATASET(ds, 0, "idExtrato");
            }

            if (bs_Extrato[0].sAgencia == "" || bs_Extrato[0].sNumeroConta == "")
            {
                div_importarConta.Visible = true;
                cmdEnviarArquivos.Visible = false;
                div_BancoImportar.Visible = false;
                div_envioArquivo.Visible = false;
                hddValido.Value = "0";
                tudoOk = 2;
                Funcoes.Popula_Combo(ddlImportarConta, "sp_Select 'tbl_Flow_Adm_ContasBancarias'", "idConta", "sDscConta", false, "Selecione uma Conta", "0");
                MensagemPaginaModalImportar.MostraMensagem("Agência e Conta não foi identificada no arquivo do Extrato, selecione qual é a conta referente ao extrato importado", "info", false);
            }
            else
            {
                try
                {
                    if (hddidExtrato.Value != "")
                    {
                        int idConta = 0;
                        string msg = "";
                        int valido = 0;
                        int qtd = 0;
                        foreach (var lancamento in bs_Extrato)
                        {

                            Dictionary<String, String> vParametros = new Dictionary<string, string>();
                            vParametros.Add("@sFuncao", "SALVAR");
                            if (idConta == 0)
                            {
                                vParametros.Add("@sAgencia", lancamento.sAgencia);
                                vParametros.Add("@sNumeroConta", lancamento.sNumeroConta);
                            }
                            else
                            {
                                vParametros.Add("@idConta", idConta.ToString());
                            }
                            vParametros.Add("@idExtrato", hddidExtrato.Value);
                            vParametros.Add("@dtLancamento", lancamento.dtLancamento);
                            vParametros.Add("@sDscLancamento", lancamento.sDscLancamento);
                            vParametros.Add("@sNumeroDocumento", lancamento.sNumeroDocumento);
                            vParametros.Add("@nCredito", lancamento.sCredito != "" ? Convert.ToDecimal(lancamento.sCredito).ToString().Replace(",", ".") : "0");
                            vParametros.Add("@nDebito", lancamento.sDebito != "" ? Convert.ToDecimal(lancamento.sDebito).ToString().Replace(",", ".") : "0");
                            vParametros.Add("@nSaldoAnterior", lancamento.sSaldoAnterior != "" ? Conversoes.Numerico(lancamento.sSaldoAnterior).ToString().Replace(",", ".") : "0");
                            vParametros.Add("@sObservacao", lancamento.sObservacao.Trim());
                            vParametros.Add("@sSaldoAtual", lancamento.sSaldoAtual.Trim() != "" ? Convert.ToDecimal(lancamento.sSaldoAtual).ToString().Replace(",", ".") : "0");
                            vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
                            vParametros.Add("@nQtd", qtd.ToString());
                            ds = BD.ExecutarDataSet(sProcedure, vParametros);
                            if (BD.ValidarDataSet(ds, out sErro))
                            {
                                idConta = Convert.ToInt32(Retorno.DATASET(ds, 0, "idConta"));
                                msg += (msg != "" ? "</br>" : "") + Retorno.DATASET(ds, 0, "sMSG");
                                valido = Convert.ToInt32(Retorno.DATASET(ds, 0, "Valido"));
                                qtd = Convert.ToInt32(Retorno.DATASET(ds, 0, "Qtd"));

                            }
                            if (valido == 2)
                            {
                                tudoOk = 1;
                                break;
                            }
							else if (valido == 3)
                            {
                                div_importarConta.Visible = true;
                                cmdEnviarArquivos.Visible = false;
                                div_BancoImportar.Visible = false;
                                div_envioArquivo.Visible = false;
                                hddValido.Value = "0";
                                tudoOk = 2;
                                Funcoes.Popula_Combo(ddlImportarConta, "sp_Select 'tbl_Flow_Adm_ContasBancarias'", "idConta", "sDscConta", false, "Selecione uma Conta", "0");
                                msg = "Agência e Conta não foi identificada no arquivo do Extrato, selecione qual é a conta referente ao extrato importado";
                                break;
                            }
							
                            if (valido == 0)
                            {
                                hddValido.Value = "0";
                            }
                        }
                        if (qtd == bs_Extrato.Count())
                        {
                            hddValido.Value = "1";
                        }

                        MensagemPaginaModalImportar.MostraMensagem_Aviso(msg);
                    }
                }
                catch (Exception e)
                {
                    tudoOk = 1;
                    //MensagemPaginaModalImportar.MostraMensagem_Erro("Erro ao salvar os lançamentos do extrato");
                    MensagemPaginaModalImportar.MostraMensagem_Erro("Erro ao salvar os lançamentos do extrato\nErro: " + e + "\nBD: " + sErro);
                }
            }

        }

        protected void btnImportarConta_Click(object sender, EventArgs e)
        {
            
            string sErro = "";
            DataSet ds;                
            try
            {
                string idConta = ddlImportarConta.SelectedValue;
                string msg = "";               
                int qtd = 0;
                foreach (var lancamento in bs_Extrato)
                {
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@idConta", idConta);                    
                    vParametros.Add("@idExtrato", hddidExtrato.Value);
                    vParametros.Add("@dtLancamento", lancamento.dtLancamento);
                    vParametros.Add("@sDscLancamento", lancamento.sDscLancamento);
                    vParametros.Add("@sNumeroDocumento", lancamento.sNumeroDocumento);
                    vParametros.Add("@nCredito", lancamento.sCredito != "" ? Convert.ToDecimal(lancamento.sCredito).ToString().Replace(",", ".") : "0");
                    vParametros.Add("@nDebito", lancamento.sDebito != "" ? Convert.ToDecimal(lancamento.sDebito).ToString().Replace(",", ".") : "0");
                    vParametros.Add("@nSaldoAnterior", lancamento.sSaldoAnterior != "" ? Conversoes.Numerico(lancamento.sSaldoAnterior).ToString().Replace(",", ".") : "0");
                    vParametros.Add("@sObservacao", lancamento.sObservacao.Trim());
                    vParametros.Add("@sSaldoAtual", lancamento.sSaldoAtual.Trim() != "" ? Convert.ToDecimal(lancamento.sSaldoAtual).ToString().Replace(",", ".") : "0");
                    vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
                    vParametros.Add("@nQtd", qtd.ToString());
                    ds = BD.ExecutarDataSet(sProcedure, vParametros);
                    if (BD.ValidarDataSet(ds, out sErro))
                    {
                        msg = Retorno.DATASET(ds, 0, "sMSG");
                        if (Retorno.DATASET(ds, 0, "sMSG") != "")
                        {                            
                            break;
                        }                    
                        
                    }                    
                }
                if (msg != "")
                {
                    MensagemPaginaModalImportar.MostraMensagem_Aviso(msg);
                }
                else
                {
                    MensagemPaginaModalImportar.MostraMensagem_Sucesso("Extrato Importado com sucesso.");
                }                
                
            }
            catch (Exception ex)
            {
                //MensagemPaginaModalImportar.MostraMensagem_Erro("Erro ao salvar os lançamentos do extrato");
                MensagemPaginaModalImportar.MostraMensagem_Erro("Erro ao salvar os lançamentos do extrato\nErro: " + ex + "\nBD: " + sErro);
            }

            bs_Extrato.Clear();
            hddidExtrato.Value = "";
            div_importarConta.Visible = false;
            cmdEnviarArquivos.Visible = true;
            div_BancoImportar.Visible = true;
            div_envioArquivo.Visible = true;
            ddlBanco.SelectedValue = "0";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalPostBack", "$('#modalImportarExtrato').modal('show');", true);
        }

        protected void btnVoltarGrid_Click(object sender, EventArgs e)
        {
            hddidExtrato.Value = "0";
            hddMudaGrid.Value = "0";
            btnImportar.Visible = true;
            div_Selecao_Tipo.Visible = true;
            ddlTipoConsulta.SelectedValue = "2";
            ddlTipoConsulta_SelectedIndexChanged(null, null);
            Pesquisar();
        }

        protected void btnExtrato_Click(object sender, EventArgs e)
        {
            string sErro = "";
            byte[] bObjArquivo = null;
            string sNomeArquivo = "";
            string urlAtualPagina = Request.UrlReferrer.ToString().Replace(Request.RawUrl, "/Download/");

            DataSet ds;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTA_ARQUIVO");
            vParametros.Add("@idLancamento", hddidLancamento.Value);
            ds = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(ds, out sErro))
            {
                bObjArquivo = Encoding.UTF8.GetBytes(Retorno.DATASET(ds, 0, "vbArquivo"));
                sNomeArquivo = Retorno.DATASET(ds, 0, "sNomeArquivo");
            }

            Funcoes.DownloadArquivo(Page, sNomeArquivo);

            ////Gera o Arquivo
            //TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
            //FileStream lObjFile = objArquivo.TransformarArrayBytesEmArquivo(bObjArquivo, Server.MapPath("~/Download/" + sNomeArquivo));
            //lObjFile.Close();
            //lObjFile.Dispose();

            ////Efetua o Download
            //StringBuilder strDownload = new StringBuilder();
            //strDownload.AppendLine("var link = document.createElement('a');");
            //strDownload.AppendLine("link.download = '" + sNomeArquivo + "';");
            //strDownload.AppendLine("link.href = '" + string.Concat(urlAtualPagina, sNomeArquivo) + "';");
            //strDownload.AppendLine("link.click();");
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "js_Download_dArquivos", strDownload.ToString(), true);
        }

        protected void btnDetalheConciliar_Click(object sender, EventArgs e)
        {
            if (hddTipoTitulo.Value == "C")
            {
                if (hddidTitulo.Value.Contains(";"))
                {
                    string sErro = "";
                    DataSet ds;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "Conciliacao_Detalhe");
                    vParametros.Add("@idLancamento", hddidLancamento.Value);
                    vParametros.Add("@sConciliacaoComposta", hddidTitulo.Value);
                    vParametros.Add("@sTipoTitulo", hddTipoTitulo.Value);
                    ds = BD.ExecutarDataSet(sProcedure, vParametros);
                    if (BD.ValidarDataSet(ds, out sErro))
                    {
                        div_gvConciliar.Visible = true;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "dt_ConciliacaoDetalhe", TT.FrameWork.Grid.DataBindComScript(gv_conciliar, ds.Tables[0], 1, "asc"), true);
                        gv_conciliar.Columns[5].Visible = false;

                    }
                }
                else
                {
                    string url = "ContasReceber_Detalhe.aspx?id=" + hddidTitulo.Value;
                    string script = $"window.open('{url}', '_blank');";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "openNewTab", script, true);
                }
            }
            else if (hddTipoTitulo.Value == "D")
            {
                if (hddidTitulo.Value.Contains(";"))
                {
                    string sErro = "";
                    DataSet ds;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "Conciliacao_Detalhe");
                    vParametros.Add("@idLancamento", hddidLancamento.Value);
                    vParametros.Add("@sConciliacaoComposta", hddidTitulo.Value);
                    vParametros.Add("@sTipoTitulo", hddTipoTitulo.Value);
                    ds = BD.ExecutarDataSet(sProcedure, vParametros);
                    if (BD.ValidarDataSet(ds, out sErro))
                    {
                        div_gvConciliar.Visible = true;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "dt_ConciliacaoDetalhe", TT.FrameWork.Grid.DataBindComScript(gv_conciliar, ds.Tables[0], 1, "asc"), true);
                        gv_conciliar.Columns[5].Visible = false;
                    }
                }
                else
                {
                    string url = "ContasPagar_Detalhe.aspx?id=" + hddidTitulo.Value;
                    string script = $"window.open('{url}', '_blank');";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "openNewTab", script, true);
                }
            }
            else
            {
                MensagemPaginaModalDetalhe.MostraMensagem_Erro("Erro ao encaminhar para o título conciliado, consulte o departamento de TI!");
            }
        }

        protected void btnNovoConciliar_Click(object sender, EventArgs e)
        {
            div_btnTipoConciliacao.Visible = true;
            div_NovaConciliacao.Visible = false;
            div_gvConciliar.Visible = false;
            div_ConciliacaoCompostaNova.Visible = false;
            div_gvConciliacaoComposta.Visible = false;
            div_gvTransferencia.Visible = false;
            RegistraScript();
        }

        protected void btnConciliacaoUnica_Click(object sender, EventArgs e)
        {
            div_btnTipoConciliacao.Visible = false;

            if (ValidaConciliacao(1))
            {
                string sErro = "";
                DataSet ds;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "Vinculo_Titulo");
                vParametros.Add("@idLancamento", hddidLancamento.Value);
                vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
                ds = BD.ExecutarDataSet(sProcedure, vParametros);
                if (BD.ValidarDataSet(ds, out sErro))
                {
                    int nRet = Convert.ToInt32(Retorno.DATASET(ds, 0, "retorno"));
                    if (nRet == 0)
                    {
                        PopulaCombo();
                        div_NovaConciliacao.Visible = true;
                        hddidEmpresa.Value = Retorno.DATASET(ds, 0, "idEmpresa");

                        if (hddTipoTitulo.Value == "C")
                        {
                            div_ddlCategoriaPagar.Visible = false;
                            div_ddlidParceiro.Visible = false;
                            div_dtEmissao.Visible = true;

                            Pesquisa_Colaborador.Visible = false;
                            Pesquisa_Colaborador.LimparCamposColaborador();
                            Pesquisa_Colaborador.Enabled = false;

                            Pesquisa_Parceiros.Visible = true;
                            Pesquisa_Parceiros.Enabled = true;
                            Pesquisa_Parceiros.idParceiro = Convert.ToInt32(ViewState["idParceiro"]);

                            div_ddlEmpresa.Visible = true;
                            ddlidEmpresa.SelectedValue = hddidEmpresa.Value;
                            ddlidEmpresa.Attributes.Add("disabled", "disabled");

                            div_ddlCategoriaReceber.Visible = true;

                            div_TipoCategoriaPagar.Visible = true;
                            div_txtdtApuracao.Visible = false;
                            div_ddlidCentroCusto.Visible = true;

                            div_ddlDisabled.Visible = true;
                            ddlidMeioPagamento.SelectedValue = "1";
                            ddlidMeioPagamento.Attributes.Add("disabled", "disabled");
                            ddlidContabil.Attributes.Add("disabled", "disabled");
                            btnSalvar.Visible = true;
                            if (hddConciliacaoComposta.Value == "S")
                            {
                                btnSalvar.Visible = false;
                            }

                            lblFormaPagamento.Text = "Forma de Recebimento";
                            lblMeioPagamento.Text = "Tipo de Recebimento";
                            MensagemPaginaModalDetalhe.MostraMensagem_Aviso("Esta etapa cria um título novo em Contas a Receber, caso não deseje continuar clique novamente em \"Nova Conciliação\" e escolha a opção \"Conciliação Composta\" > \"Lançamentos Existentes\"", false);

                        }
                        else if (hddTipoTitulo.Value == "D")
                        {
                            div_ddlCategoriaReceber.Visible = false;
                            div_ddlCategoriaPagar.Visible = true;
                            div_ddlidParceiro.Visible = false;
                            div_ddlEmpresa.Visible = false;
                            div_TipoCategoriaPagar.Visible = false;
                            div_ddlDisabled.Visible = false;
                            btnSalvar.Visible = false;
                            div_dtEmissao.Visible = false;

                            Pesquisa_Colaborador.Visible = false;
                            Pesquisa_Colaborador.Enabled = false;

                            Pesquisa_Parceiros.Visible = false;
                            Pesquisa_Parceiros.Enabled = false;

                            MensagemPaginaModalDetalhe.MostraMensagem_Aviso("Esta etapa cria um título novo em Contas a Pagar, caso não deseje continuar clique novamente em \"Nova Conciliação\" e escolha a opção \"Conciliação Composta\" > \"Lançamentos Existentes\"", false);
                            MensagemPaginaModalDetalhe.MostraMensagem("Selecione uma Categoria", "info", false);
                        }
                        else
                        {
                            MensagemPaginaModalDetalhe.MostraMensagem_Erro("Erro ao Conciliar, consulte o departamento de TI!");
                        }

                    }
                    else if (nRet > 1)
                    {
                        div_gvConciliar.Visible = true;
                        MensagemPaginaModalDetalhe.MostraMensagem("Escolha qual o título será feito a Conciliação", "info", false);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables", TT.FrameWork.Grid.DataBindComScript(gv_conciliar, ds.Tables[1], 1, "asc"), true);
                    }
                    else
                    {
                        LancamentoDetalhe(hddidLancamento.Value);
                        MensagemPaginaModalDetalhe.MostraMensagem_Sucesso("Conciliação realizada com sucesso!");
                        MensagemPaginaModalDetalhe.MostraMensagem("Esse processo foi feito automaticamente, clique no botão \"Detalhe Conciliação\" para verificar o título", "info", false);
                    }
                }
            }
            else
            {
                MensagemPaginaModalDetalhe.MostraMensagem_Aviso("Conciliação já foi realizada, por favor atualize a página! Caso persista consulte o departamento de TI");
            }
            RegistraScript();
        }

        protected void btnConciliacaoCompostaNova_Click(object sender, EventArgs e)
        {
            div_ConciliacaoCompostaNova.Visible = true;
            div_btnTipoConciliacao.Visible = false;
            txtnValorTotalTitulo.ReadOnly = true;
            hddConciliacaoComposta.Value = "S";

            if (ValidaConciliacao(1))
            {
                string sErro = "";
                DataSet ds;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "Conciliacao_Composta");
                vParametros.Add("@idLancamento", hddidLancamento.Value);
                vParametros.Add("@idTipoConciliacao", "1");
                ds = BD.ExecutarDataSet(sProcedure, vParametros);
                if (BD.ValidarDataSet(ds, out sErro))
                {
                    PopulaCombo();
                    div_NovaConciliacao.Visible = true;
                    hddidEmpresa.Value = Retorno.DATASET(ds, 0, "idEmpresa");

                    if (hddTipoTitulo.Value == "C")
                    {
                        div_ddlCategoriaPagar.Visible = false;
                        div_ddlidParceiro.Visible = false;
                        div_dtEmissao.Visible = true;

                        Pesquisa_Colaborador.Visible = false;
                        Pesquisa_Colaborador.LimparCamposColaborador();
                        Pesquisa_Colaborador.Enabled = false;

                        Pesquisa_Parceiros.Visible = true;
                        Pesquisa_Parceiros.Enabled = true;
                        Pesquisa_Parceiros.idParceiro = Convert.ToInt32(ViewState["idParceiro"]);

                        div_ddlEmpresa.Visible = true;
                        ddlidEmpresa.SelectedValue = hddidEmpresa.Value;
                        ddlidEmpresa.Attributes.Add("disabled", "disabled");

                        div_ddlCategoriaReceber.Visible = true;

                        div_TipoCategoriaPagar.Visible = true;
                        div_txtdtApuracao.Visible = false;
                        div_ddlidCentroCusto.Visible = true;

                        div_ddlDisabled.Visible = true;
                        ddlidMeioPagamento.SelectedValue = "1";
                        ddlidMeioPagamento.Attributes.Add("disabled", "disabled");
                        ddlidContabil.Attributes.Add("disabled", "disabled");
                        btnSalvar.Visible = false;

                        lblFormaPagamento.Text = "Forma de Recebimento";
                        lblMeioPagamento.Text = "Tipo de Recebimento";
                        lbldtTransacao.Text = "Data Recebimento";
                        lblValorTransacao.Text = "Valor Recebimento";

                    }
                    else if (hddTipoTitulo.Value == "D")
                    {
                        div_ddlCategoriaReceber.Visible = false;
                        div_ddlCategoriaPagar.Visible = true;
                        div_ddlidParceiro.Visible = false;
                        div_ddlEmpresa.Visible = false;
                        div_TipoCategoriaPagar.Visible = false;
                        div_ddlDisabled.Visible = false;
                        btnSalvar.Visible = false;
                        div_dtEmissao.Visible = false;

                        Pesquisa_Colaborador.Visible = false;
                        Pesquisa_Colaborador.Enabled = false;

                        Pesquisa_Parceiros.Visible = false;
                        Pesquisa_Parceiros.Enabled = false;

                        MensagemPaginaModalDetalhe.MostraMensagem("Selecione uma Categoria", "info", false);
                    }
                    else
                    {
                        MensagemPaginaModalDetalhe.MostraMensagem_Erro("Erro ao Conciliar, consulte o departamento de TI!");
                    }
                }
            }
            RegistraScript();
        }

        protected void btnConciliacaoCompostaExistente_Click(object sender, EventArgs e)
        {
            div_gvConciliacaoComposta.Visible = true;
            div_btnTipoConciliacao.Visible = false;
            btnConfirmar.Attributes.Add("disabled", "disabled");
            txtnValorTotal.ReadOnly = true;

            if (ValidaConciliacao(1))
            {
                string sErro = "";
                DataSet ds;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "Conciliacao_Composta");
                vParametros.Add("@idLancamento", hddidLancamento.Value);
                vParametros.Add("@sTipoTitulo", hddTipoTitulo.Value);
                vParametros.Add("@idTipoConciliacao", "2");
                ds = BD.ExecutarDataSet(sProcedure, vParametros);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    if (BD.ValidarDataSet(ds, out sErro))
                    {
                        MensagemPaginaModalDetalhe.MostraMensagem("Selecione quais os títulos serão feitos a Conciliação", "info", false);
                        AplicarDataTable(ds);
                    }
                }
                else
                {
                    div_gvConciliacaoComposta.Visible = false;
                    MensagemPaginaModalDetalhe.MostraMensagem_Erro("Nenhum registro foi encontrado", false);
                }


            }
            RegistraScript();
        }

        protected void AplicarDataTable(DataSet ds)
        {
            StringBuilder sbRetorno = new StringBuilder();
            sbRetorno.AppendLine("$(document).ready(function() {");
            sbRetorno.AppendLine("$('#" + gv_ConciliacaoComposta.ClientID + "').DataTable({");
            sbRetorno.AppendLine("responsive: true, paging: false, scrollCollapse: true, scrollX: true ,scrollY: 300,");
            sbRetorno.AppendLine("order: [[1, 'asc']],");
            sbRetorno.AppendLine("info: false,");
            sbRetorno.AppendLine("language: { url: 'https://cdn.datatables.net/plug-ins/1.11.5/i18n/pt-BR.json' },");
            sbRetorno.AppendLine("});");
            sbRetorno.AppendLine("});");
            gv_ConciliacaoComposta.DataSource = ds.Tables[0];
            gv_ConciliacaoComposta.DataBind();
            if (gv_ConciliacaoComposta.Rows.Count > 0)
            {
                //Criando tags theader, tbody, tfoot
                gv_ConciliacaoComposta.HeaderRow.TableSection = TableRowSection.TableHeader;
                gv_ConciliacaoComposta.UseAccessibleHeader = true;
                gv_ConciliacaoComposta.FooterRow.TableSection = TableRowSection.TableFooter;
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "dt_ConciliacaoComposta", sbRetorno.ToString(), true);
        }

        protected void btnTransferencia_Click(object sender, EventArgs e)
        {
            string sErro = "";
            DataSet ds;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "Vinculo_Transferencia");
            vParametros.Add("@idLancamento_Original", hddidLancamento.Value);
            ds = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(ds, out sErro))
            {
                int nRet = Convert.ToInt32(Retorno.DATASET(ds, 0, "retorno"));
                if (nRet == 0)
                {
                    MensagemPaginaModalDetalhe.MostraMensagem_Erro("Não foi encontrado lançamento do extrato para realizar o vínculo da transferência");
                }
                else if (nRet > 1)
                {
                    div_gvTransferencia.Visible = true;
                    MensagemPaginaModalDetalhe.MostraMensagem("Escolha qual o lançamento será feito o vínculo da transferência", "info", false);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DataTables_Transferencia", TT.FrameWork.Grid.DataBindComScript(gv_transferencia, ds.Tables[1], 1, "asc"), true);
                }
                else
                {
                    string idLancamentoConciliado = hddidLancamento.Value;
                    MensagemPaginaModalDetalhe.MostraMensagem_Sucesso("Vínculo da transferência realizado com sucesso!");
                    MensagemPaginaModalDetalhe.MostraMensagem("Esse processo foi feito automaticamente", "info", false);
                    LancamentoDetalhe(idLancamentoConciliado);
                    AtualizarLinhaConciliada(idLancamentoConciliado);

                   
                }
            }
            RegistraScript();
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            if (ValidaDados())
            {
                try
                {
                    string sErro = "";
                    decimal nDebito = Convert.ToDecimal(txtnDebito.Text);
                    decimal nCredito = Convert.ToDecimal(txtnCredito.Text);
                    DateTime dtLancamento = Convert.ToDateTime(txtdtLancamento.Text);
                    string idTitulo = "";
                    DataSet ds;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@dtVencimento", dtLancamento.ToString());
                    vParametros.Add("@sQuantidadeParcela", $"Única");
                    vParametros.Add("@sCodigo", txtsCodigo.Text);
                    vParametros.Add("@dtEmissao", DateTime.Parse(txtdtEmissao.Text).ToString());
                    vParametros.Add("@sDocumento", txtsLancamento.Text);
                    vParametros.Add("@idContabil", ddlidContabil.SelectedValue);
                    vParametros.Add("@idCentroDeCusto", ddlidCentroDeCusto.SelectedValue);
                    vParametros.Add("@idEmpresa", ddlidEmpresa.SelectedValue);
                    vParametros.Add("@nParcelas", "1");
                    vParametros.Add("@idParceiro", Pesquisa_Parceiros.idParceiro.ToString() == "" ? ddlidParceiro.SelectedValue : Pesquisa_Parceiros.idParceiro.ToString());
                    vParametros.Add("@sObservacaoGeral", txtsObservacao.Text);
                    vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
                    vParametros.Add("@sTituloConciliado", "S");
                    vParametros.Add("@dtConciliado", DateTime.Now.ToString());

                    if (hddTipoTitulo.Value == "D")
                    {
                        vParametros.Add("@nSaldo", nDebito.ToString().Replace(",", "."));
                        vParametros.Add("@nValorOriginal", nDebito.ToString().Replace(",", "."));
                        vParametros.Add("@nValorBruto", nDebito.ToString().Replace(",", "."));
                        vParametros.Add("@idFormaPagamento", ddlidFormaPagamento.SelectedValue);
                        vParametros.Add("@idMeioPagamento", ddlidMeioPagamento.SelectedValue);
                        vParametros.Add("@dtApuracao", txtdtApuracao.Text);
                        vParametros.Add("@idColaborador", Pesquisa_Colaborador.idColaborador.ToString());
                        vParametros.Add("@idCategoriaPagar", ddlidCategoriaPagar.SelectedValue);
                        ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Pagar", vParametros);
                        if (BD.ValidarDataSet(ds, out sErro))
                        {
                            idTitulo = Retorno.DATASET(ds, 0, "idContasPagar");
                        }

                        int idConta = SalvarIdTitulo(idTitulo);

                        vParametros.Clear();
                        vParametros.Add("@sFuncao", "INSERIR_Pagamento");
                        vParametros.Add("@idContasPagar", idTitulo);
                        vParametros.Add("@idConta_Info_Pag", idConta.ToString());
                        vParametros.Add("@nValorPagamento_Info_Pag", nDebito.ToString().Replace(",", "."));
                        vParametros.Add("@dtPagamento_Info_Pag", dtLancamento.ToString());
                        vParametros.Add("@nNumeroParcela_Info_Pag", "1");
                        vParametros.Add("@idFormaPagamento_Info_Pag", ddlidFormaPagamento.SelectedValue);
                        vParametros.Add("@sDscFormaPagamento_Info_Pag", "");
                        vParametros.Add("@sDscConta_Info_Pag", "");
                        vParametros.Add("@nMulta", "0");
                        vParametros.Add("@nJuros", "0");
                        vParametros.Add("@nDesconto", "0");
                        vParametros.Add("@nValorTotalPag", nDebito.ToString().Replace(",", "."));
                        vParametros.Add("@idArquivo", "");
                        vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
                        vParametros.Add("@idConciliacao", hddidLancamento.Value);
                        BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Pagar", vParametros);

                    }
                    else if (hddTipoTitulo.Value == "C")
                    {
                        vParametros.Add("@nSaldo", nCredito.ToString().Replace(",", "."));
                        vParametros.Add("@nValorOriginal", nCredito.ToString().Replace(",", "."));
                        vParametros.Add("@nValorBruto", nCredito.ToString().Replace(",", "."));
                        vParametros.Add("@idFormaRecebimento", ddlidFormaPagamento.SelectedValue);
                        vParametros.Add("@idMeioRecebimento", ddlidMeioPagamento.SelectedValue);
                        vParametros.Add("@dtVencimentoOriginal", dtLancamento.ToString());
                        vParametros.Add("@idCategoriaReceber", ddlidCategoriaReceber.SelectedValue);
                        ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Receber", vParametros);
                        if (BD.ValidarDataSet(ds, out sErro))
                        {
                            idTitulo = Retorno.DATASET(ds, 0, "idContasReceber");
                        }

                        int idConta = SalvarIdTitulo(idTitulo);

                        vParametros.Clear();
                        vParametros.Add("@sFuncao", "INSERIR_Recebimento");
                        vParametros.Add("@idContasReceber", idTitulo);
                        vParametros.Add("@idConta_Info_Rec", idConta.ToString());
                        vParametros.Add("@nValorRecebimento_Info_Rec", nCredito.ToString().Replace(",", "."));
                        vParametros.Add("@dtRecebimento_Info_Rec", dtLancamento.ToString());
                        vParametros.Add("@nNumeroParcela_Info_Rec", "1");
                        vParametros.Add("@idFormaRecebimento_Info_Rec", ddlidFormaPagamento.SelectedValue);
                        vParametros.Add("@sDscFormaRecebimento_Info_Rec", "");
                        vParametros.Add("@sDscConta_Info_Rec", "");
                        vParametros.Add("@nMulta", "0");
                        vParametros.Add("@nJuros", "0");
                        vParametros.Add("@nDesconto", "0");
                        vParametros.Add("@nValorTotalRec", nCredito.ToString().Replace(",", "."));
                        vParametros.Add("@idArquivo", "");
                        vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
                        vParametros.Add("@idConciliacao", hddidLancamento.Value);
                        BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Receber", vParametros);
                    }

                    string idLancamentoConciliado = hddidLancamento.Value;
                    MensagemPaginaModalDetalhe.MostraMensagem_Sucesso("Conciliação criada com sucesso!");
                    LancamentoDetalhe(idLancamentoConciliado);
                    AtualizarLinhaConciliada(idLancamentoConciliado);
                }
                catch (Exception ex)
                {
                    MensagemPaginaModalDetalhe.MostraMensagem_Erro(ex.Message);
                }

            }

            RegistraScript();
        }

        private void AtualizarLinhaConciliada(string idLancamento)
        {
            foreach (GridViewRow row in dtgvConsultaExtrato.Rows)
            {
                if (row.Attributes["data-id-lancamento"] != idLancamento)
                {
                    continue;
                }

                Literal conciliacao = row.FindControl("litConciliacao") as Literal;
                if (conciliacao != null)
                {
                    conciliacao.Text = "<i class='fa fa-check'></i>";
                }

                break;
            }

            string safeId = HttpUtility.JavaScriptStringEncode(idLancamento);
            ScriptManager.RegisterStartupScript(
                updModalDetalhe,
                updModalDetalhe.GetType(),
                "focarLancamentoConciliado",
                "atualizarLinhaConciliada('" + safeId + "');",
                true);
        }
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidaConciliacao(3))
                {
                    DataTable dt;
                    Dictionary<String, String> vParametros = new Dictionary<string, string>();
                    string sTitulos = string.Empty;
                    foreach (GridViewRow row in gv_ConciliacaoComposta.Rows)
                    {
                        CheckBox cb = (CheckBox)row.FindControl("cbConciliacaoComposta");

                        if (cb != null && cb.Checked)
                        {
                            string idRegistro = gv_ConciliacaoComposta.DataKeys[row.RowIndex]["idRegistro"].ToString();
                            string idTitulo = gv_ConciliacaoComposta.DataKeys[row.RowIndex]["idTitulo"].ToString();
                            Label lbValor = (Label)row.FindControl("lblValor");

                            //metodo para salvar conciliacao                                              

                            vParametros.Add("@sFuncao", "Conciliacao_Composta");
                            vParametros.Add("@idLancamento", hddidLancamento.Value);
                            vParametros.Add("@idTitulo", idTitulo);
                            vParametros.Add("@idRegistro", idRegistro);
                            vParametros.Add("@nValor", lbValor.Text.Replace("R$", "").Replace(".", "").Replace(",", "."));
                            vParametros.Add("@sTipoTitulo", hddTipoTitulo.Value);
                            vParametros.Add("@idTipoConciliacao", "3");
                            vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());

                            dt = BD.ExecutarDataTable(sProcedure, vParametros);

                            if (!string.IsNullOrEmpty(sTitulos))
                                sTitulos += ";";

                            sTitulos += idTitulo;
                            vParametros.Clear();
                        }
                    }

                    vParametros.Add("@sFuncao", "Conciliacao_Composta");
                    vParametros.Add("@idLancamento", hddidLancamento.Value);
                    vParametros.Add("@sConciliacaoComposta", sTitulos);
                    vParametros.Add("@idTipoConciliacao", "3");
                    vParametros.Add("@sConciliado", "S");
                    dt = BD.ExecutarDataTable(sProcedure, vParametros);


                    string idLancamentoConciliado = hddidLancamento.Value;
                    MensagemPaginaModalDetalhe.MostraMensagem_Sucesso("Conciliação realizada com sucesso!");
                    LancamentoDetalhe(idLancamentoConciliado);
                    AtualizarLinhaConciliada(idLancamentoConciliado);

                }
            }
            catch (Exception ex)
            {
                MensagemPaginaModalDetalhe.MostraMensagem_Erro("Houve um erro na conciliação! <br /> Erro: " + ex.Message, false);

            }
        }

        private bool ValidaDados()
        {
            bool bRetorno = true;
            string sMensagemErro = "";
            if (hddTipoCategoria.Value == "1" || hddTipoCategoria.Value == "2") //Compras e Serviços
            {
                if (ddlidCategoriaPagar.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Categoria!";
                }

                if (ddlidFormaPagamento.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Forma de Pagamento!";
                }

                if (Pesquisa_Parceiros.idParceiro == 0)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira um Parceiro!";
                }

                if (ddlidEmpresa.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Empresa!";
                }

                if (ddlidCentroDeCusto.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Centro de Custo!";
                }

            }
            else if (hddTipoCategoria.Value == "3") //Impostos
            {
                if (ddlidCategoriaPagar.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Categoria!";
                }

                if (!Validacoes.ValidarData(txtdtApuracao))
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira a Data de Apuração!";
                }

                if (ddlidFormaPagamento.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Forma de Pagamento!";
                }

                if (ddlidEmpresa.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Empresa!";
                }

                if (ddlidParceiro.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Credor!";
                }

            }
            else if (hddTipoCategoria.Value == "4") //Internos
            {
                if (ddlidCategoriaPagar.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Categoria!";
                }

                if (!Validacoes.ValidarData(txtdtApuracao))
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira a Data de Apuração!";
                }

                if (ddlidFormaPagamento.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Forma de Pagamento!";
                }

                if (ddlidEmpresa.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Empresa!";
                }

                if (Pesquisa_Colaborador.idColaborador == 0)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira um Colaborador!";
                }

            }
            else if (hddTipoCategoria.Value == "5" || hddTipoCategoria.Value == "6") //Operaçoes Financeiras e Benificios
            {
                if (ddlidCategoriaPagar.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Categoria!";
                }

                if (!Validacoes.ValidarData(txtdtApuracao))
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira a Data de Apuração!";
                }

                if (ddlidFormaPagamento.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Forma de Pagamento!";
                }

                if (ddlidEmpresa.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Empresa!";
                }

                if (Pesquisa_Parceiros.idParceiro == 0)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira um Parceiro!";
                }

                if (ddlidCentroDeCusto.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Centro de Custo!";
                }

            }
            else if (hddTipoCategoria.Value == "7") //Contas a Receber
            {
                if (ddlidCategoriaReceber.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Categoria!";
                }

                if (ddlidFormaPagamento.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Forma de Recebimento!";
                }

                if (Pesquisa_Parceiros.idParceiro == 0)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira um Parceiro!";
                }

                if (ddlidEmpresa.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Empresa!";
                }

                if (ddlidCentroDeCusto.SelectedValue == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione um Centro de Custo!";
                }

            }

            if (hddConciliacaoComposta.Value == "S")
            {
                if (hddTipoCategoria.Value == "" || hddTipoCategoria.Value == "0")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Selecione uma Categoria";
                }

                if (txtdtVencimento.Text == "")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma data de vencimento";
                }

                if (txtnValorBruto.Text == "")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira o Valor Bruto";
                }

                if (txtnValorLiquido.Text == "")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira o Valor Líquido";
                }

                if (txtdtTransacao.Text == "" && hddTipoTitulo.Value == "D")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma data de Pagamento";
                }

                if (txtdtTransacao.Text == "" && hddTipoTitulo.Value == "C")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira uma data de Recebimento";
                }

                if (txtnValorTransacao.Text == "" && hddTipoTitulo.Value == "D")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira o Valor de Pagamento";
                }

                if (txtnValorTransacao.Text == "" && hddTipoTitulo.Value == "C")
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Insira o Valor de Recebimento";
                }

                DateTime dtVencimento = txtdtVencimento.Text == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(txtdtVencimento.Text);
                DateTime dtEmissao = txtdtEmissao.Text == "" ? DateTime.Parse("1900-01-01") : DateTime.Parse(txtdtEmissao.Text);
                if (dtVencimento < dtEmissao)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data de Validade não pode ser antes da Data de Emissão";
                }

                //if (DateTime.Parse(txtdtVencimento.Text) < DateTime.Parse(txtdtTransacao.Text) || DateTime.Parse(txtdtEmissão.Text) > DateTime.Parse(txtdtTransacao.Text) )
                //{
                //    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Data da transação deve ser depois da data de Emissão e antes da Data de Vencimento";
                //}

                decimal valorBruto = txtnValorBruto.Text != "" ? decimal.Parse(txtnValorBruto.Text) : 0;
                decimal valorLiquido = txtnValorLiquido.Text != "" ? decimal.Parse(txtnValorLiquido.Text) : 0;
                if (valorBruto < valorLiquido)
                {
                    sMensagemErro += (sMensagemErro != "" ? "</br>" : "") + "Valor Bruto deve ser maior ou igual ao Valor Líquido";
                }

            }

            if (sMensagemErro != "")
            {
                bRetorno = false;
                MensagemPaginaModalDetalhe.MostraMensagem_Erro(sMensagemErro);
            }

            return bRetorno;
        }

        private bool ValidaConciliacao(int tipoConciliacao)
        {
            bool bRetorno = true;
            if (tipoConciliacao == 1)
            {
                DataSet ds;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                vParametros.Add("@idLancamento", hddidLancamento.Value);
                ds = BD.ExecutarDataSet(sProcedure, vParametros);

                string idTitulo = Retorno.DATASET(ds, 0, "idTitulo") == "" ? "0" : Retorno.DATASET(ds, 0, "idTitulo");

                if (idTitulo == "0")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (tipoConciliacao == 2)
            {
                if (hddTipoTitulo.Value == "C")
                {
                    if (decimal.Parse(txtnValorTotal.Text.Replace("R$", "").Replace(".", "").Replace(",", ".").Trim()) > decimal.Parse(txtnCredito.Text.Replace("R$", "").Replace(".", "").Replace(",", ".").Trim()))
                    {
                        bRetorno = false;
                        MensagemPaginaModalDetalhe.MostraMensagem_Erro("Valor total dos recebimentos selecionados é maior que o valor do lançamento do extrato. Necessário verificar para que o valor total seja igual ao valor do lançamento");
                    }
                    else if (decimal.Parse(txtnValorTotal.Text.Replace("R$", "").Replace(".", "").Replace(",", ".").Trim()) < decimal.Parse(txtnCredito.Text.Replace("R$", "").Replace(".", "").Replace(",", ".").Trim()))
                    {
                        bRetorno = false;
                        MensagemPaginaModalDetalhe.MostraMensagem_Erro("Valor total dos recebimentos selecionados é menor que o valor do lançamento do extrato. Necessário verificar para que o valor total seja igual ao valor do lançamento");
                    }
                    return bRetorno;
                }
                else if (hddTipoTitulo.Value == "D")
                {
                    if (decimal.Parse(txtnValorTotal.Text.Replace("R$", "").Replace(".", "").Replace(",", ".").Trim()) > decimal.Parse(txtnDebito.Text.Replace("R$", "").Replace(".", "").Replace(",", ".").Trim()))
                    {
                        bRetorno = false;
                        MensagemPaginaModalDetalhe.MostraMensagem_Erro("Valor total dos pagamentos selecionados é maior que o valor do lançamento do extrato. Necessário verificar para que o valor total seja igual ao valor do lançamento");
                    }
                    else if (decimal.Parse(txtnValorTotal.Text.Replace("R$", "").Replace(".", "").Replace(",", ".").Trim()) < decimal.Parse(txtnDebito.Text.Replace("R$", "").Replace(".", "").Replace(",", ".").Trim()))
                    {
                        bRetorno = false;
                        MensagemPaginaModalDetalhe.MostraMensagem_Erro("Valor total dos pagamentos selecionados é menor que o valor do lançamento do extrato. Necessário verificar para que o valor total seja igual ao valor do lançamento");
                    }
                    return bRetorno;
                }
                else
                {
                    return bRetorno;
                }
            }
            else
            {
                return bRetorno;
            }

        }

        private int SalvarIdTitulo(string idTitulo)
        {
            string sErro = "";
            int idConta = 0;
            DataSet ds;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "SALVA_TITULO");
            vParametros.Add("@idLancamento", hddidLancamento.Value);
            vParametros.Add("@idTitulo", idTitulo);
            ds = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(ds, out sErro))
            {
                idConta = Convert.ToInt32(Retorno.DATASET(ds, 0, "idConta"));
                return idConta;
            }
            return idConta;
        }

        private void LancamentoDetalhe(string id)
        {
            string sErro = "";
            string idTitulo = "";
            string idTransferencia = "";
            string sConciliacaoComposta = "";
            DataSet ds;
            Dictionary<String, String> vParametros = new Dictionary<string, string>();
            vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
            vParametros.Add("@idLancamento", id);
            ds = BD.ExecutarDataSet(sProcedure, vParametros);
            if (BD.ValidarDataSet(ds, out sErro))
            {
                //hddidExtrato.Value = Retorno.DATASET(ds, 0, "idExtrato");
                txtdtLancamento.Text = Retorno.DATASET(ds, 0, "dtLancamento");
                txtsDscBanco.Text = Retorno.DATASET(ds, 0, "sDscConta");
                txtsNumeroDocumento.Text = Retorno.DATASET(ds, 0, "sNumeroDocumento");
                txtsLancamento.Text = Retorno.DATASET(ds, 0, "sDscLancamento");
                txtnCredito.Text = Retorno.DATASET(ds, 0, "nCredito");
                txtnDebito.Text = Retorno.DATASET(ds, 0, "nDebito");
                txtsObservacao.Text = Retorno.DATASET(ds, 0, "sObservacao");
                idTitulo = Retorno.DATASET(ds, 0, "idTitulo") == "" ? "0" : Retorno.DATASET(ds, 0, "idTitulo");
                idTransferencia = Retorno.DATASET(ds, 0, "idTransferencia") == "" ? "0" : Retorno.DATASET(ds, 0, "idTransferencia");
                sConciliacaoComposta = Retorno.DATASET(ds, 0, "sConciliacaoComposta") == "" ? "0" : Retorno.DATASET(ds, 0, "sConciliacaoComposta");
            }

            txtdtLancamento.ReadOnly = true;
            txtsLancamento.ReadOnly = true;
            txtsDscBanco.ReadOnly = true;
            txtsNumeroDocumento.ReadOnly = true;
            txtnCredito.ReadOnly = true;
            txtnDebito.ReadOnly = true;
            div_NovaConciliacao.Visible = false;
            btnSalvar.Visible = false;
            txtsObservacao.ReadOnly = true;
            div_gvConciliar.Visible = false;
            div_gvTransferencia.Visible = false;
            div_detalheTransferencia.Visible = false;
            div_ConciliacaoCompostaNova.Visible = false;
            div_gvConciliacaoComposta.Visible = false;

            RegistraScript();

            //if (txtsObservacao.Text == "")
            //{
            //    div_txtObservacao.Visible = false;
            //}


            if (!Funcoes.ValidaPermissao(Permissao.Financeiro.ExtratoBancario.DownloadExtrato))
            {
                div_btnExtrato.Visible = false;
            }

            //---------------------------------- CONCILIAR ---------------------------------------------------
            div_DetalheConciliar.Visible = false;

            if (idTitulo != "0" && sConciliacaoComposta == "0")
            {
                hddidTitulo.Value = idTitulo;
                div_DetalheConciliar.Visible = true;
            }
            else if (sConciliacaoComposta != "0")
            {
                hddidTitulo.Value = sConciliacaoComposta;
                div_DetalheConciliar.Visible = true;
            }

            div_btnNovoConciliar.Visible = !div_DetalheConciliar.Visible;
            div_transferencia.Visible = !div_DetalheConciliar.Visible;

            if (idTransferencia != "0")
            {
                div_detalheTransferencia.Visible = true;
                div_transferencia.Visible = false;
                div_btnNovoConciliar.Visible = false;

                vParametros.Clear();

                vParametros.Add("@sFuncao", "CONSULTAR_DETALHE");
                vParametros.Add("@idLancamento", idTransferencia);
                ds = BD.ExecutarDataSet(sProcedure, vParametros);
                if (BD.ValidarDataSet(ds, out sErro))
                {
                    txtTransferenciaData.Text = Retorno.DATASET(ds, 0, "dtLancamento");
                    txtTransferenciaBanco.Text = Retorno.DATASET(ds, 0, "sDscConta");
                    txtTransferenciaNumeroDocumento.Text = Retorno.DATASET(ds, 0, "sNumeroDocumento");
                    txtTransferenciaDscLancamento.Text = Retorno.DATASET(ds, 0, "sDscLancamento");
                    txtTransferenciaCredito.Text = Retorno.DATASET(ds, 0, "nCredito");
                    txtTransferenciaDebito.Text = Retorno.DATASET(ds, 0, "nDebito");
                    txtTransferenciaObservacao.Text = Retorno.DATASET(ds, 0, "sObservacao");
                }

                if (txtTransferenciaObservacao.Text == "")
                {
                    div_ObservacaoTransferencia.Visible = false;
                    txtTransferenciaObservacao.ReadOnly = true;
                }

                txtTransferenciaBanco.ReadOnly = true;
                txtTransferenciaData.ReadOnly = true;
                txtTransferenciaNumeroDocumento.ReadOnly = true;
                txtTransferenciaDscLancamento.ReadOnly = true;
                txtTransferenciaDebito.ReadOnly = true;
                txtTransferenciaCredito.ReadOnly = true;

            }

            if (Convert.ToDecimal(txtnCredito.Text) != 0)
            {
                hddTipoTitulo.Value = "C";
            }
            else if (Convert.ToDecimal(txtnDebito.Text) != 0)
            {
                hddTipoTitulo.Value = "D";
            }

        }

        private void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<script type='text/javascript'>");
            sb.AppendLine("$(document).ready(function () {");
            sb.AppendLine("     $('[id*=txtnDebito]').mask('0.000.000.009,99', { reverse: true });");
            sb.AppendLine("     $('[id*=txtnCredito]').mask('0.000.000.009,99', { reverse: true });");
            sb.AppendLine("     $('[id*=txtnValorBruto]').mask('0.000.000.009,99', { reverse: true });");
            sb.AppendLine("     $('[id*=txtnValorLiquido]').mask('0.000.000.009,99', { reverse: true });");
            sb.AppendLine("     $('[id*=txtnValorTransacao]').mask('0.000.000.009,99', { reverse: true });");
            sb.AppendLine("});");


            sb.AppendLine("</script>");


            ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptExtrato", sb.ToString(), false);
        }

        private void PopulaCombo()
        {
            Funcoes.Popula_Combo(ddlidCategoriaPagar, "sp_Select 'Flow_Adm_Contas_Pagar_Categoria'", "idCategoriaPagar", "sDscCategoriaPagar", false, "Selecione a Categoria", "0");
            Funcoes.Popula_Combo(ddlidFormaPagamento, "sp_Select 'tbl_Flow_CondicaodePagamento_Tipo'", "idTipoCondicaoPagamento", "sDscTipoCondicaoPagamento", false, "Selecione o Pagamento", "0");
            Funcoes.Popula_Combo(ddlidCentroDeCusto, "sp_Select 'Flow_Adm_CentroDeCusto'", "idCentroDeCusto", "sDescricao", false, "Selecione um Centro de Custo", "0");
            Funcoes.Popula_Combo(ddlidContabil, "sp_Select 'Flow_CodigoContabil'", "idContabil", "sDscCodContabil", false, "Selecione o Código Contábil", "0");
            Funcoes.Popula_Combo(ddlidMeioPagamento, "sp_Select 'tbl_Flow_Adm_MeioPagamento'", "idMeioPagamento", "sDscMeioPagamento", false, "Selecione um Meio", "0");
            Funcoes.Popula_Combo(ddlidEmpresa, "sp_Select 'Flow_Empresa'", "idEmpresa", "sDscEmpresa", false, "Selecione a Empresa", "0");
            Funcoes.Popula_Combo(ddlidParceiro, "sp_Select 'Flow_Parceiro_Credor_Impostos'", "idParceiro", "sRazaoSocial", false, "Selecione um credor", "0");
            Funcoes.Popula_Combo(ddlidCategoriaReceber, "sp_Select 'Flow_Adm_Contas_Receber_Categoria'", "idCategoriaReceber", "sDscCategoriaReceber", false, "Selecione a Categoria", "0");
        }

        protected void btnProximoTitulo_Click(object sender, EventArgs e)
        {
            if (ValidaDados())
            {
                decimal valor = txtnValorTotalTitulo.Text != "" ? Convert.ToDecimal(txtnValorTotalTitulo.Text) : 0;
                decimal valorLancamento = txtnDebito.Text == "0,00" ? Convert.ToDecimal(txtnCredito.Text) : Convert.ToDecimal(txtnDebito.Text);
                if (bs_ConciliacaoComposta.Count > 0)
                {
                    decimal valorNovo = Convert.ToDecimal(txtnValorTransacao.Text);
                    valor = valor + valorNovo;

                    if (valor > valorLancamento)
                    {
                        valor = valor - valorNovo;
                        txtnValorTotalTitulo.Text = valor.ToString("N2");
                        MensagemPaginaModalDetalhe.MostraMensagem_Erro("Valor total não pode ser maior que o valor do lançamento do extrato.");
                        return;
                    }

                    txtnValorTotalTitulo.Text = valor.ToString("N2");

                }
                else
                {
                    valor = Convert.ToDecimal(txtnValorTransacao.Text);
                    txtnValorTotalTitulo.Text = txtnValorTransacao.Text;
                }

                cls_ConciliacaoComposta objItem = new cls_ConciliacaoComposta();
                objItem.dtVencimento = txtdtVencimento.Text;
                objItem.sCodigo = txtsCodigo.Text;
                objItem.dtEmissao = txtdtEmissao.Text;
                objItem.sDocumento = txtsLancamento.Text;
                objItem.idContabil = ddlidContabil.SelectedValue;
                objItem.idCentroDeCusto = ddlidCentroDeCusto.SelectedValue;
                objItem.idEmpresa = ddlidEmpresa.SelectedValue;
                objItem.idParceiro = Pesquisa_Parceiros.idParceiro.ToString() == "" ? ddlidParceiro.SelectedValue : Pesquisa_Parceiros.idParceiro.ToString();
                objItem.sObservacaoGeral = txtsObservacao.Text;
                objItem.nSaldo = txtnValorTransacao.Text;
                objItem.nValorBruto = txtnValorBruto.Text;
                objItem.nValorLiquido = txtnValorLiquido.Text;
                objItem.idFormaTransacao = ddlidFormaPagamento.SelectedValue;
                objItem.idMeioTransacao = ddlidMeioPagamento.SelectedValue;
                objItem.dtTransacao = txtdtTransacao.Text;
                objItem.nValorTransacao = txtnValorTransacao.Text;
                objItem.nValorTotal = valor.ToString("N2");

                if (hddTipoTitulo.Value == "D")
                {
                    objItem.idCategoria = ddlidCategoriaPagar.SelectedValue;
                    objItem.dtApuracao = txtdtApuracao.Text;
                    objItem.idColaborador = Pesquisa_Colaborador.idColaborador.ToString();
                }
                else if (hddTipoTitulo.Value == "C")
                {
                    objItem.idCategoria = ddlidCategoriaReceber.SelectedValue;
                }

                bs_ConciliacaoComposta.Add(objItem);

                if (valor == valorLancamento)
                {
                    hddConciliacaoComposta.Value = "";
                    SalvarTitulosConciliacao();
                }
                else
                {
                    LimpaCampos();
                }
            }
            RegistraScript();
        }

        private void SalvarTitulosConciliacao()
        {
            try
            {
                string sErro = "";
                DataSet ds;
                DataTable dt;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                string sTitulos = string.Empty;
                foreach (var extrato in bs_ConciliacaoComposta)
                {

                    decimal nDebito = Convert.ToDecimal(txtnDebito.Text);
                    decimal nCredito = Convert.ToDecimal(txtnCredito.Text);
                    DateTime dtLancamento = Convert.ToDateTime(txtdtLancamento.Text);
                    string idTitulo = "";
                    vParametros.Add("@sFuncao", "SALVAR");
                    vParametros.Add("@dtVencimento", DateTime.Parse(extrato.dtVencimento).ToString());
                    vParametros.Add("@sQuantidadeParcela", $"Única");
                    vParametros.Add("@sCodigo", extrato.sCodigo);
                    vParametros.Add("@dtEmissao", DateTime.Parse(extrato.dtEmissao).ToString());
                    vParametros.Add("@sDocumento", extrato.sDocumento);
                    vParametros.Add("@idContabil", extrato.idContabil);
                    vParametros.Add("@idCentroDeCusto", extrato.idCentroDeCusto);
                    vParametros.Add("@idEmpresa", extrato.idEmpresa);
                    vParametros.Add("@nParcelas", "1");
                    vParametros.Add("@idParceiro", extrato.idParceiro);
                    vParametros.Add("@sObservacaoGeral", extrato.sObservacaoGeral);
                    vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
                    vParametros.Add("@sTituloConciliado", "S");
                    vParametros.Add("@dtConciliado", DateTime.Now.ToString());

                    if (hddTipoTitulo.Value == "D")
                    {
                        vParametros.Add("@nSaldo", extrato.nSaldo.Replace(".", "").Replace(",", "."));
                        vParametros.Add("@nValorOriginal", extrato.nValorLiquido.Replace(".", "").Replace(",", "."));
                        vParametros.Add("@nValorBruto", extrato.nValorBruto.Replace(".", "").Replace(",", "."));
                        vParametros.Add("@idFormaPagamento", extrato.idFormaTransacao);
                        vParametros.Add("@idMeioPagamento", extrato.idMeioTransacao);
                        vParametros.Add("@dtApuracao", extrato.dtApuracao == "" ? extrato.dtApuracao : DateTime.Parse(extrato.dtApuracao).ToString());
                        vParametros.Add("@idColaborador", extrato.idColaborador);
                        vParametros.Add("@idCategoriaPagar", extrato.idCategoria);
                        ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Pagar", vParametros);
                        if (BD.ValidarDataSet(ds, out sErro))
                        {
                            idTitulo = Retorno.DATASET(ds, 0, "idContasPagar");
                        }

                        int idConta = SalvarIdTitulo(idTitulo);

                        vParametros.Clear();
                        vParametros.Add("@sFuncao", "INSERIR_Pagamento");
                        vParametros.Add("@idContasPagar", idTitulo);
                        vParametros.Add("@idConta_Info_Pag", idConta.ToString());
                        vParametros.Add("@nValorPagamento_Info_Pag", extrato.nValorTransacao.Replace(".", "").Replace(",", "."));
                        vParametros.Add("@dtPagamento_Info_Pag", DateTime.Parse(extrato.dtTransacao).ToString());
                        vParametros.Add("@nNumeroParcela_Info_Pag", "1");
                        vParametros.Add("@idFormaPagamento_Info_Pag", extrato.idFormaTransacao);
                        vParametros.Add("@sDscFormaPagamento_Info_Pag", "");
                        vParametros.Add("@sDscConta_Info_Pag", "");
                        vParametros.Add("@nMulta", "0");
                        vParametros.Add("@nJuros", "0");
                        vParametros.Add("@nDesconto", "0");
                        vParametros.Add("@nValorTotalPag", extrato.nValorTransacao.Replace(".", "").Replace(",", "."));
                        vParametros.Add("@idArquivo", "");
                        vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
                        vParametros.Add("@idConciliacao", hddidLancamento.Value);
                        BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Pagar", vParametros);

                    }
                    else if (hddTipoTitulo.Value == "C")
                    {
                        vParametros.Add("@nSaldo", extrato.nSaldo.Replace(".", "").Replace(",", "."));
                        vParametros.Add("@nValorOriginal", extrato.nValorLiquido.Replace(".", "").Replace(",", "."));
                        vParametros.Add("@nValorBruto", extrato.nValorBruto.Replace(".", "").Replace(",", "."));
                        vParametros.Add("@idFormaRecebimento", extrato.idFormaTransacao);
                        vParametros.Add("@idMeioRecebimento", extrato.idMeioTransacao);
                        vParametros.Add("@dtVencimentoOriginal", DateTime.Parse(extrato.dtVencimento).ToString());
                        vParametros.Add("@idCategoriaReceber", extrato.idCategoria);
                        ds = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Receber", vParametros);
                        if (BD.ValidarDataSet(ds, out sErro))
                        {
                            idTitulo = Retorno.DATASET(ds, 0, "idContasReceber");
                        }

                        int idConta = SalvarIdTitulo(idTitulo);

                        vParametros.Clear();
                        vParametros.Add("@sFuncao", "INSERIR_Recebimento");
                        vParametros.Add("@idContasReceber", idTitulo);
                        vParametros.Add("@idConta_Info_Rec", idConta.ToString());
                        vParametros.Add("@nValorRecebimento_Info_Rec", extrato.nValorTransacao.Replace(".", "").Replace(",", "."));
                        vParametros.Add("@dtRecebimento_Info_Rec", DateTime.Parse(extrato.dtTransacao).ToString());
                        vParametros.Add("@nNumeroParcela_Info_Rec", "1");
                        vParametros.Add("@idFormaRecebimento_Info_Rec", extrato.idFormaTransacao);
                        vParametros.Add("@sDscFormaRecebimento_Info_Rec", "");
                        vParametros.Add("@sDscConta_Info_Rec", "");
                        vParametros.Add("@nMulta", "0");
                        vParametros.Add("@nJuros", "0");
                        vParametros.Add("@nDesconto", "0");
                        vParametros.Add("@nValorTotalRec", extrato.nValorTransacao.Replace(".", "").Replace(",", "."));
                        vParametros.Add("@idArquivo", "");
                        vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());
                        vParametros.Add("@idConciliacao", hddidLancamento.Value);
                        BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Receber", vParametros);
                    }

                    if (!string.IsNullOrEmpty(sTitulos))
                        sTitulos += ";";

                    sTitulos += idTitulo;

                    vParametros.Clear();
                }

                vParametros.Add("@sFuncao", "Conciliacao_Composta");
                vParametros.Add("@idLancamento", hddidLancamento.Value);
                vParametros.Add("@sConciliacaoComposta", sTitulos);
                vParametros.Add("@idTipoConciliacao", "3");
                vParametros.Add("@sConciliado", "S");
                dt = BD.ExecutarDataTable(sProcedure, vParametros);

                MensagemPaginaModalDetalhe.MostraMensagem_Sucesso("Conciliação criada com sucesso!");
                LancamentoDetalhe(hddidLancamento.Value);

            }
            catch (Exception ex)
            {
                MensagemPaginaModalDetalhe.MostraMensagem_Erro(ex.Message);
            }
        }

        private void LimpaCampos()
        {
            ddlidCategoriaPagar.SelectedValue = "0";
            ddlidCategoriaReceber.SelectedValue = "0";
            Pesquisa_Colaborador.LimparCamposColaborador();
            Pesquisa_Parceiros.LimparCamposParceiro();
            txtdtEmissao.Text = "";
            txtdtApuracao.Text = "";
            ddlidFormaPagamento.SelectedValue = "0";
            ddlidCentroDeCusto.SelectedValue = "0";
            txtsCodigo.Text = "";
            ddlidContabil.SelectedValue = "0";
            txtdtVencimento.Text = "";
            txtnValorBruto.Text = "";
            txtnValorLiquido.Text = "";
            txtdtTransacao.Text = "";
            txtnValorTransacao.Text = "";

        }

        protected void ddlidCategoriaPagar_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            int idCategoriaTipo_Consulta = 0;
            try
            {
                DataSet dsContabil;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "Autoselecao_Contabil");
                vParametros.Add("@idCategoriaPagar", ddlidCategoriaPagar.SelectedValue);

                dsContabil = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Pagar", vParametros);

                if (BD.ValidarDataSet(dsContabil))
                {
                    ddlidContabil.SelectedValue = Retorno.DATASET(dsContabil, 0, "idContabil");
                    idCategoriaTipo_Consulta = Convert.ToInt32(Retorno.DATASET(dsContabil, 0, "idCategoriaTipo"));

                }
            }
            catch
            {
                ddlidContabil.SelectedValue = "";
            }
            switch (idCategoriaTipo_Consulta)
            {

                case 1: //Compras
                case 2: //Serviços 
                    Pesquisa_Colaborador.Visible = false;
                    Pesquisa_Colaborador.LimparCamposColaborador();
                    Pesquisa_Colaborador.Enabled = false;

                    Pesquisa_Parceiros.Visible = true;
                    Pesquisa_Parceiros.Enabled = true;
                    Pesquisa_Parceiros.idParceiro = Convert.ToInt32(ViewState["idParceiro"]);

                    div_ddlEmpresa.Visible = true;
                    div_ddlidParceiro.Visible = false;
                    div_TipoCategoriaPagar.Visible = true;
                    div_txtdtApuracao.Visible = false;
                    div_ddlidCentroCusto.Visible = true;

                    lblsCodigo.Text = "Nota Fiscal";

                    break;
                case 3: //Impostos                   
                    Pesquisa_Colaborador.Visible = false;
                    Pesquisa_Colaborador.LimparCamposColaborador();
                    Pesquisa_Colaborador.Enabled = false;

                    Pesquisa_Parceiros.Visible = false;
                    Pesquisa_Parceiros.Enabled = false;

                    div_ddlidParceiro.Visible = true;
                    div_ddlEmpresa.Visible = true;

                    div_TipoCategoriaPagar.Visible = true;
                    div_txtdtApuracao.Visible = true;
                    div_ddlidCentroCusto.Visible = false;

                    lblsCodigo.Text = "Código Receita";

                    break;
                case 4: //Internos
                    Pesquisa_Colaborador.Visible = true;
                    Pesquisa_Colaborador.LimparCamposColaborador();
                    Pesquisa_Colaborador.Enabled = true;

                    Pesquisa_Parceiros.Visible = false;
                    Pesquisa_Parceiros.Enabled = false;
                    Pesquisa_Parceiros.idParceiro = Convert.ToInt32(ViewState["idParceiro"]);

                    div_ddlEmpresa.Visible = true;
                    div_ddlidParceiro.Visible = false;
                    div_TipoCategoriaPagar.Visible = true;
                    div_txtdtApuracao.Visible = true;

                    lblsCodigo.Text = "Nota Fiscal";

                    break;

                case 5: //Benefícios
                case 6: //Operações Financeiras 
                    Pesquisa_Colaborador.Visible = false;
                    Pesquisa_Colaborador.LimparCamposColaborador();
                    Pesquisa_Colaborador.Enabled = false;

                    Pesquisa_Parceiros.Visible = true;
                    Pesquisa_Parceiros.Enabled = true;
                    Pesquisa_Parceiros.idParceiro = Convert.ToInt32(ViewState["idParceiro"]);

                    div_ddlEmpresa.Visible = true;
                    div_ddlidParceiro.Visible = false;
                    div_TipoCategoriaPagar.Visible = true;
                    div_txtdtApuracao.Visible = true;

                    lblsCodigo.Text = "Nota Fiscal";

                    break;
                case 0:
                    div_ddlCategoriaReceber.Visible = false;
                    div_ddlCategoriaPagar.Visible = true;
                    div_ddlidParceiro.Visible = false;
                    div_ddlEmpresa.Visible = false;
                    div_TipoCategoriaPagar.Visible = false;
                    div_ddlDisabled.Visible = false;
                    btnSalvar.Visible = false;
                    div_dtEmissao.Visible = false;

                    Pesquisa_Colaborador.Visible = false;
                    Pesquisa_Colaborador.Enabled = false;

                    Pesquisa_Parceiros.Visible = false;
                    Pesquisa_Parceiros.Enabled = false;
                    break;

            }

            ddlidEmpresa.SelectedValue = hddidEmpresa.Value;
            ddlidEmpresa.Attributes.Add("disabled", "disabled");
            div_dtEmissao.Visible = true;
            div_ddlDisabled.Visible = true;
            ddlidMeioPagamento.SelectedValue = "1";
            ddlidMeioPagamento.Attributes.Add("disabled", "disabled");
            ddlidContabil.Attributes.Add("disabled", "disabled");
            btnSalvar.Visible = true;
            if (hddConciliacaoComposta.Value == "S")
            {
                btnSalvar.Visible = false;
            }
            hddTipoCategoria.Value = idCategoriaTipo_Consulta.ToString();

            RegistraScript();
        }

        protected void ddlidCategoriaReceber_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataSet dsContabil;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "Autoselecao_Contabil");
                vParametros.Add("@idCategoriaReceber", ddlidCategoriaReceber.SelectedValue);

                dsContabil = BD.ExecutarDataSet("sp_Manipula_tbl_Flow_Adm_Contas_Receber", vParametros);

                if (BD.ValidarDataSet(dsContabil))
                {
                    ddlidContabil.SelectedValue = Retorno.DATASET(dsContabil, 0, "idContabil");
                }
                hddTipoCategoria.Value = "7";
            }
            catch
            {
                ddlidContabil.SelectedValue = "";
            }

            RegistraScript();
        }

        protected void gv_conciliar_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int id = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "idTitulo"));

                HyperLink link = (HyperLink)e.Row.FindControl("hlTitulo");
                if (link != null)
                {
                    if (hddTipoTitulo.Value == "C")
                    {
                        link.NavigateUrl = "/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id=" + id;
                        link.Target = "_blank";
                    }
                    else if (hddTipoTitulo.Value == "D")
                    {
                        link.NavigateUrl = "/App/Paginas/Adm/Financeiro/ContasPagar_Detalhe.aspx?id=" + id;
                        link.Target = "_blank";
                    }
                }
            }
        }

        protected void gv_conciliar_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "conciliar")
            {
                string[] args = e.CommandArgument.ToString().Split(';');
                int idTitulo = Convert.ToInt32(args[0]);
                int idRegistro = Convert.ToInt32(args[1]);

                string sErro = "";
                DataSet ds;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "Conciliar_Titulo");
                vParametros.Add("@idLancamento", hddidLancamento.Value);
                vParametros.Add("@idTitulo", idTitulo.ToString());
                vParametros.Add("@idRegistro", idRegistro.ToString());
                vParametros.Add("@sTipoTitulo", hddTipoTitulo.Value);
                vParametros.Add("@nCredito", Convert.ToDecimal(txtnCredito.Text).ToString().Replace(",", "."));
                vParametros.Add("@nDebito", Convert.ToDecimal(txtnDebito.Text).ToString().Replace(",", "."));
                vParametros.Add("@idUsuarioAtualizacao", Identity.Variaveis.idUsuario());

                ds = BD.ExecutarDataSet(sProcedure, vParametros);
                if (BD.ValidarDataSet(ds, out sErro))
                {
                    string msg = Retorno.DATASET(ds, 0, "sMSG");
                    MensagemPaginaModalDetalhe.MostraMensagem_Sucesso(msg);
                    LancamentoDetalhe(hddidLancamento.Value);
                }
            }
        }

        protected void gv_transferencia_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "transferencia")
            {
                string idLancamento = e.CommandArgument.ToString();

                string sErro = "";
                DataSet ds;
                Dictionary<String, String> vParametros = new Dictionary<string, string>();
                vParametros.Add("@sFuncao", "Transferencia");
                vParametros.Add("@idLancamento", idLancamento);
                vParametros.Add("@idLancamento_Original", hddidLancamento.Value);

                ds = BD.ExecutarDataSet(sProcedure, vParametros);
                if (BD.ValidarDataSet(ds, out sErro))
                {
                    string msg = Retorno.DATASET(ds, 0, "sMSG");
                    MensagemPaginaModalDetalhe.MostraMensagem_Sucesso(msg);
                    LancamentoDetalhe(hddidLancamento.Value);
                }
            }
        }



        protected void gv_ConciliacaoComposta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int id = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "idTitulo"));

                HyperLink linkID = (HyperLink)e.Row.FindControl("hlTitulo");
                HyperLink linkParceiro = (HyperLink)e.Row.FindControl("hlParceiro");
                if (linkID != null && linkParceiro != null)
                {
                    if (hddTipoTitulo.Value == "C")
                    {
                        linkID.NavigateUrl = "/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id=" + id;
                        linkID.Target = "_blank";
                        linkParceiro.NavigateUrl = "/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id=" + id;
                        linkParceiro.Target = "_blank";
                    }
                    else if (hddTipoTitulo.Value == "D")
                    {
                        linkID.NavigateUrl = "/App/Paginas/Adm/Financeiro/ContasPagar_Detalhe.aspx?id=" + id;
                        linkID.Target = "_blank";
                        linkParceiro.NavigateUrl = "/App/Paginas/Adm/Financeiro/ContasPagar_Detalhe.aspx?id=" + id;
                        linkParceiro.Target = "_blank";
                    }
                }
            }
        }

        protected void ddl_SelectedIndexChanged(object sender, EventArgs e)
        {
            Pesquisar();
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            HttpResponse response = HttpContext.Current.Response;
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Extrato bancário");

            List<int> visibleColumns = Enumerable.Range(0, dtgvConsultaExtrato.Columns.Count)
                .Where(columnIndex => dtgvConsultaExtrato.Columns[columnIndex].Visible)
                .ToList();

            ICellStyle titleStyle = workbook.CreateCellStyle();
            titleStyle.Alignment = HorizontalAlignment.Center;
            titleStyle.VerticalAlignment = VerticalAlignment.Center;
            IFont titleFont = workbook.CreateFont();
            titleFont.IsBold = true;
            titleFont.FontHeightInPoints = 16;
            titleStyle.SetFont(titleFont);

            ICellStyle subtitleStyle = workbook.CreateCellStyle();
            subtitleStyle.Alignment = HorizontalAlignment.Center;
            subtitleStyle.VerticalAlignment = VerticalAlignment.Center;
            subtitleStyle.WrapText = true;
            IFont subtitleFont = workbook.CreateFont();
            subtitleFont.IsItalic = true;
            subtitleFont.FontHeightInPoints = 11;
            subtitleStyle.SetFont(subtitleFont);

            ICellStyle headerStyle = workbook.CreateCellStyle();
            IFont headerFont = workbook.CreateFont();
            headerFont.IsBold = true;
            headerStyle.SetFont(headerFont);

            ICellStyle currencyStyle = workbook.CreateCellStyle();
            currencyStyle.DataFormat = workbook.CreateDataFormat().GetFormat("\"R$\" #,##0.00");

            int lastColumnIndex = visibleColumns.Count - 1;

            IRow titleRow = sheet.CreateRow(0);
            titleRow.HeightInPoints = 30;
            ICell titleCell = titleRow.CreateCell(0);
            titleCell.SetCellValue("Extrato Bancário");
            titleCell.CellStyle = titleStyle;

            string subtitle = MontarSubtituloExportacao();

            IRow subtitleRow = sheet.CreateRow(1);
            subtitleRow.HeightInPoints = 32;
            ICell subtitleCell = subtitleRow.CreateCell(0);
            subtitleCell.SetCellValue(subtitle);
            subtitleCell.CellStyle = subtitleStyle;

            if (lastColumnIndex > 0)
            {
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, lastColumnIndex));
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 0, lastColumnIndex));
            }

            const int headerRowIndex = 3;
            IRow header = sheet.CreateRow(headerRowIndex);
            for (int excelColumnIndex = 0; excelColumnIndex < visibleColumns.Count; excelColumnIndex++)
            {
                int gridColumnIndex = visibleColumns[excelColumnIndex];
                ICell cell = header.CreateCell(excelColumnIndex);
                cell.SetCellValue(dtgvConsultaExtrato.Columns[gridColumnIndex].HeaderText);
                cell.CellStyle = headerStyle;
            }

            for (int rowIndex = 0; rowIndex < dtgvConsultaExtrato.Rows.Count; rowIndex++)
            {
                GridViewRow gridRow = dtgvConsultaExtrato.Rows[rowIndex];
                IRow excelRow = sheet.CreateRow(headerRowIndex + rowIndex + 1);

                for (int excelColumnIndex = 0; excelColumnIndex < visibleColumns.Count; excelColumnIndex++)
                {
                    int gridColumnIndex = visibleColumns[excelColumnIndex];
                    ICell cell = excelRow.CreateCell(excelColumnIndex);
                    string cellText = ObterTextoCelula(gridRow.Cells[gridColumnIndex]);
                    string headerText = dtgvConsultaExtrato.Columns[gridColumnIndex].HeaderText;

                    decimal currencyValue;
                    if ((headerText == "Crédito" || headerText == "Débito") &&
                        decimal.TryParse(cellText, NumberStyles.Currency,
                            CultureInfo.GetCultureInfo("pt-BR"), out currencyValue))
                    {
                        cell.SetCellValue(Convert.ToDouble(currencyValue));
                        cell.CellStyle = currencyStyle;
                    }
                    else
                    {
                        cell.SetCellValue(cellText);
                    }
                }
            }

            for (int columnIndex = 0; columnIndex < visibleColumns.Count; columnIndex++)
            {
                sheet.AutoSizeColumn(columnIndex);
            }

            sheet.CreateFreezePane(0, headerRowIndex + 1);

            string fileName = "Extrato_Bancario";
            string subtitleFileName = FormatarTextoNomeArquivo(subtitle);
            if (!string.IsNullOrEmpty(subtitleFileName))
            {
                fileName += "_" + subtitleFileName;
            }

            response.Clear();
            response.Buffer = true;
            response.AddHeader("content-disposition", "attachment; filename=\"" + fileName + ".xlsx\"");
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.Cookies.Add(new HttpCookie("DownloadExtratoConcluido", "1")
            {
                Expires = DateTime.Now.AddMinutes(1),
                Path = "/"
            });
            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            workbook.Write(response.OutputStream);

            try
            {
                response.End();
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
        }

        private string MontarSubtituloExportacao()
        {
            List<string> filters = new List<string>();

            if (div_Selecao_Periodo.Visible)
            {
                string initialDate = FormatarDataFiltro(txtdtInicial.Text);
                string finalDate = FormatarDataFiltro(txtdtFinal.Text);

                if (!string.IsNullOrEmpty(initialDate) && !string.IsNullOrEmpty(finalDate))
                {
                    filters.Add(initialDate + " a " + finalDate);
                }
            }

            filters.Add("Conta: " + ObterSelecaoFiltro(ddlConta, "Todas as Contas"));

            if (ddlTipoLancamento.SelectedValue != "T")
            {
                filters.Add(ObterSelecaoFiltro(ddlTipoLancamento, "Todos"));
            }

            if (ddlConciliado.SelectedValue != "T")
            {
                filters.Add(ObterSelecaoFiltro(ddlConciliado, "Todos"));
            }

            return string.Join(" - ", filters);
        }

        private static string FormatarTextoNomeArquivo(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            string normalizedValue = value.Normalize(NormalizationForm.FormD);
            StringBuilder fileName = new StringBuilder();

            foreach (char character in normalizedValue)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                if (char.IsLetterOrDigit(character))
                {
                    fileName.Append(character);
                }
                else if (fileName.Length > 0 && fileName[fileName.Length - 1] != '_')
                {
                    fileName.Append('_');
                }
            }

            string result = fileName.ToString().Trim('_');
            return result.Length > 140 ? result.Substring(0, 140).TrimEnd('_') : result;
        }
        private static string ObterSelecaoFiltro(DropDownList dropdown, string allItemsText)
        {
            if (dropdown.SelectedItem == null || string.IsNullOrEmpty(dropdown.SelectedValue) || dropdown.SelectedValue == "0")
            {
                return allItemsText;
            }

            return HttpUtility.HtmlDecode(dropdown.SelectedItem.Text).Trim();
        }

        private static string FormatarDataFiltro(string value)
        {
            DateTime date;
            string[] formats = { "yyyy-MM-dd", "dd/MM/yyyy", "yyyy-MM-ddTHH:mm:ss" };

            if (DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out date))
            {
                return date.ToString("dd/MM/yyyy");
            }

            return value == null ? string.Empty : value.Trim();
        }
        private static string ObterTextoCelula(TableCell cell)
        {
            if (cell.Controls.Count == 0)
            {
                return HttpUtility.HtmlDecode(cell.Text).Replace("\u00a0", string.Empty).Trim();
            }

            StringBuilder text = new StringBuilder();
            AdicionarTextoControles(cell.Controls, text);
            return HttpUtility.HtmlDecode(text.ToString()).Trim();
        }

        private static void AdicionarTextoControles(ControlCollection controls, StringBuilder text)
        {
            foreach (Control control in controls)
            {
                LinkButton linkButton = control as LinkButton;
                Literal literal = control as Literal;
                if (linkButton != null)
                {
                    text.Append(linkButton.Text);
                }
                else if (literal != null && literal.Text.Contains("fa-check"))
                {
                    text.Append("Sim");
                }
                else
                {
                    ITextControl textControl = control as ITextControl;
                    if (textControl != null)
                    {
                        text.Append(textControl.Text);
                    }
                }

                if (control.HasControls())
                {
                    AdicionarTextoControles(control.Controls, text);
                }
            }
        }
    }
}
