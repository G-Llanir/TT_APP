using NPOI.HSSF.UserModel; // NPOI para .xls
using NPOI.SS.UserModel; // NPOI core
using NPOI.XSSF.UserModel; // NPOI para .xlsx
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq; // Para usar .Any() em DataTables
using System.Text; // Para StringBuilder do script
using System.Web.UI;
using System.Web.UI.WebControls; // Para FileUpload e GridView
using BD = TT.FrameWork.BD;
using FUNCOES = TT.FrameWork.Funcoes;
using IDENTITY = TT.FrameWork.Identity;
using System.Globalization;

namespace TT_Flow.App.Controles
{
    public partial class ProdutosImportador : UserControl
    {
        public string ModalId { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RegistraScript();
                // Limpa o grid e as mensagens ao carregar a página pela primeira vez
                gvResultadosImportacao.DataSource = null;
                gvResultadosImportacao.DataBind();
                lblMensagemSucesso.Text = string.Empty;
                lblMensagemErro.Text = string.Empty;
                MensagemPagina.MostraMensagem("Favor Colocar as Colunas com os nomes \"<b>Código do Produto</b>\" e \"<b>Código EAN</b>\"", "info",false);
            }
            else
            {
                RegistraScript();
            }
        }

        protected void btnIniciarImportacao_Click(object sender, EventArgs e)
        {
            // Limpa mensagens e o GridView antes de cada importação
            lblMensagemSucesso.Text = string.Empty;
            lblMensagemErro.Text = string.Empty;
            gvResultadosImportacao.DataSource = null;
            gvResultadosImportacao.DataBind();

            DataTable dtResults = new DataTable();
            dtResults.Columns.Add("Código do Produto", typeof(string));
            dtResults.Columns.Add("Código EAN", typeof(string));
            dtResults.Columns.Add("Status da Atualização", typeof(string));
            dtResults.Columns.Add("Mensagem", typeof(string));

            string filePath = string.Empty;
            IWorkbook workbook = null;

            try
            {
                // Agora, ValidarEnvioArquivo e ProcessarImportacaoEAN usam o fuEANExcel DO PRÓPRIO CONTROLE
                ValidarEnvioArquivo(fuEANExcel);

                // 1. Salva o arquivo enviado em um diretório temporário no servidor
                string uploadPath = Server.MapPath("~/TempUploads/");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Cria um nome de arquivo único para evitar colisões
                filePath = Path.Combine(uploadPath, Guid.NewGuid().ToString() + Path.GetExtension(fuEANExcel.FileName));
                fuEANExcel.SaveAs(filePath);

                // 2. Abre o arquivo salvo com NPOI
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    string fileExtension = Path.GetExtension(fuEANExcel.FileName).ToLower();

                    if (fileExtension == ".xlsx")
                    {
                        workbook = new XSSFWorkbook(fileStream);
                    }
                    else if (fileExtension == ".xls")
                    {
                        workbook = new HSSFWorkbook(fileStream);
                    }
                    else
                    {
                        throw new Exception("Formato de arquivo não suportado. Use .xls ou .xlsx.");
                    }
                }

                // 3. Processa a primeira planilha do workbook
                ISheet sheet = workbook.GetSheetAt(0);
                if (sheet == null)
                {
                    throw new Exception("Nenhuma planilha encontrada no arquivo Excel.");
                }

                // Acha os índices das colunas "Código do Produto" e "Código EAN" pelo cabeçalho
                IRow headerRow = sheet.GetRow(sheet.FirstRowNum);
                if (headerRow == null)
                {
                    throw new Exception("Nenhuma linha de cabeçalho encontrada na planilha.");
                }

                int colCodigoProdutoIndex = -1;
                int colCodigoEANIndex = -1;

                for (int i = headerRow.FirstCellNum; i < headerRow.LastCellNum; i++)
                {
                    ICell cell = headerRow.GetCell(i);
                    if (cell != null)
                    {
                        string header = GetCellValue(cell).Trim();
                        if (header.Equals("Código do Produto", StringComparison.OrdinalIgnoreCase) ||
                            header.Equals("Codigo do Produto", StringComparison.OrdinalIgnoreCase) ||
                            header.Equals("Código Produto", StringComparison.OrdinalIgnoreCase) ||
                            header.Equals("Produto", StringComparison.OrdinalIgnoreCase))
                        {
                            colCodigoProdutoIndex = i;
                        }
                        else if (header.Equals("Código Ean", StringComparison.OrdinalIgnoreCase) ||
                                 header.Equals("Codigo Ean", StringComparison.OrdinalIgnoreCase) ||
                                 header.Equals("EAN", StringComparison.OrdinalIgnoreCase))
                        {
                            colCodigoEANIndex = i;
                        }

                        if (colCodigoProdutoIndex != -1 && colCodigoEANIndex != -1)
                            break; // Encontrou ambas, pode sair do loop
                    }
                }

                if (colCodigoProdutoIndex == -1 || colCodigoEANIndex == -1)
                {
                    throw new Exception("As colunas 'Código do Produto' e 'Código EAN' não foram encontradas no arquivo Excel. Verifique o cabeçalho.");
                }

                // 4. Itera sobre as linhas de dados (ignorando o cabeçalho)
                for (int rowNum = sheet.FirstRowNum + 1; rowNum <= sheet.LastRowNum; rowNum++)
                {
                    IRow row = sheet.GetRow(rowNum);
                    if (row == null) continue; // Linha completamente vazia

                    bool isRowTrulyEmpty = true;
                    // Consideramos vazia se ambas as células de produto e EAN estiverem vazias.
                    ICell produtoCell = row.GetCell(colCodigoProdutoIndex);
                    ICell eanCell = row.GetCell(colCodigoEANIndex);

                    if ((produtoCell != null && !string.IsNullOrWhiteSpace(GetCellValue(produtoCell))) ||
                        (eanCell != null && !string.IsNullOrWhiteSpace(GetCellValue(eanCell))))
                    {
                        isRowTrulyEmpty = false;
                    }

                    if (isRowTrulyEmpty) continue; // Pula a linha se ambas as células chave estiverem vazias.

                    string sCodigoProduto = GetCellValue(produtoCell);
                    string sCodigoEAN = GetCellValue(eanCell);

                    DataRow newResultRow = dtResults.NewRow();
                    newResultRow["Código do Produto"] = sCodigoProduto;
                    newResultRow["Código EAN"] = sCodigoEAN;

                    if (string.IsNullOrWhiteSpace(sCodigoProduto))
                    {
                        newResultRow["Status da Atualização"] = "Erro ❌";
                        newResultRow["Mensagem"] = "Código do Produto vazio. Linha ignorada.";
                        dtResults.Rows.Add(newResultRow);
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(sCodigoEAN))
                    {
                        newResultRow["Status da Atualização"] = "Erro ❌";
                        newResultRow["Mensagem"] = "EAN vazio. Linha ignorada.";
                        dtResults.Rows.Add(newResultRow);
                        continue;
                    }

                    try
                    {
                        // Chama a sua função de banco de dados (sp_Manipula_tbl_Flow_Produtos)
                        Dictionary<string, string> parametrosSP = new Dictionary<string, string>
                        {
                            { "@sFuncao", "ATUALIZA-IMPORTADOR-EXCEL" },
                            { "@sCodigo", sCodigoProduto }, // Assumindo valor padrão
                            { "@sCodigoEAN", sCodigoEAN },
                            { "@idUsuarioAtualizacao", IDENTITY.Variaveis.idUsuario().ToString() }
                        };

                        List<object> result = BD.ExecutarLista<object>("sp_Manipula_tbl_Flow_Produtos", parametrosSP, false);

                        newResultRow["Status da Atualização"] = "Sucesso ✅";
                        newResultRow["Mensagem"] = "EAN atualizado.";
                    }
                    catch (Exception dbEx)
                    {
                        newResultRow["Status da Atualização"] = "Erro BD ❌";
                        newResultRow["Mensagem"] = $"Erro ao atualizar EAN: {dbEx.Message}";
                    }
                    dtResults.Rows.Add(newResultRow);
                }

                // Vincula os resultados ao GridView interno do controle
                gvResultadosImportacao.DataSource = dtResults;
                gvResultadosImportacao.DataBind();

                // Verifica se houve algum erro nos resultados para dar o feedback adequado
                if (dtResults.AsEnumerable().Any(row => row.Field<string>("Status da Atualização").Contains("Erro")))
                {
                    lblMensagemErro.Text = "Importação concluída, mas com alguns problemas. Verifique os detalhes na tabela abaixo. ⚠️";
                    lblMensagemSucesso.Text = "";
                }
                else
                {
                    lblMensagemSucesso.Text = "Importação de EANs finalizada com sucesso! 🎉";
                    lblMensagemErro.Text = "";
                }
            }
            catch (Exception ex)
            {
                lblMensagemErro.Text = $"Ocorreu um erro crítico durante a importação: {ex.Message} 🚨 Por favor, tente novamente ou contate o suporte.";
                lblMensagemSucesso.Text = "";

                if (gvResultadosImportacao.DataSource == null || dtResults.Rows.Count == 0)
                {
                    DataTable errorTable = new DataTable();
                    errorTable.Columns.Add("Erro Geral da Importação", typeof(string));
                    errorTable.Rows.Add(ex.Message);
                    gvResultadosImportacao.DataSource = errorTable;
                    gvResultadosImportacao.DataBind();
                }
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    try { File.Delete(filePath); } catch (Exception) { /* Ignora erros na exclusão */ }
                }

                AbrirModal(ModalId);

                // Chame a função JavaScript para limpar o FileUpload e o display do nome do arquivo
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ClearUploadAfterImport", "clearFileInput();", true);
            }

            AbrirModal(ModalId);
        }

        private void ValidarEnvioArquivo(FileUpload fu)
        {
            if (!fu.HasFile)
                throw new Exception("É necessário selecionar um arquivo para importar!");
            else if (!Path.GetExtension(fu.FileName).ToLower().Equals(".xlsx") && !Path.GetExtension(fu.FileName).ToLower().Equals(".xls"))
                throw new Exception("É necessário selecionar um arquivo no formato Excel, com a extensão '.xlsx' ou '.xls'!");
        }

        private string GetCellValue(ICell cell)
        {
            if (cell == null)
            {
                return string.Empty;
            }

            switch (cell.CellType)
            {
                case CellType.String:
                    return cell.StringCellValue.Trim();

                case CellType.Numeric:
                    return cell.NumericCellValue.ToString(CultureInfo.InvariantCulture);

                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();

                case CellType.Formula:
                    try
                    {
                        if (cell.CachedFormulaResultType == CellType.Numeric)
                        {
                            return cell.NumericCellValue.ToString(CultureInfo.InvariantCulture);
                        }
                        else if (cell.CachedFormulaResultType == CellType.String)
                        {
                            return cell.StringCellValue.Trim();
                        }
                        else if (cell.CachedFormulaResultType == CellType.Boolean)
                        {
                            return cell.BooleanCellValue.ToString();
                        }
                        return cell.ToString().Trim();
                    }
                    catch (Exception)
                    {
                        return cell.ToString().Trim();
                    }

                case CellType.Blank:
                    return string.Empty;

                case CellType.Error:
                    return "#ERROR!";

                default:
                    return cell.ToString().Trim();
            }
        }

        #region | Script
        protected void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            // Envolve todo o script em uma função para que possa ser re-chamada após UpdatePanels
            sb.Append("function initFileUploadScript() {\r\n");

            // Define os seletores jQuery
            sb.Append("var $mainDropZone = $('[id$=divPnBody]');\r\n");
            sb.Append("var $fileInput = $('[id$=fuEANExcel]');\r\n");
            sb.Append("var $fileNameDisplay = $('[id*=fileName]');\r\n");
            sb.Append("var $visualDropZone = $('[id*=uploadContainer]');\r\n\r\n");

            // Importante: Remover eventos anteriores para evitar duplicação
            // Isso é crucial se initFileUploadScript() for chamada múltiplas vezes (ex: por UpdatePanel)
            sb.Append("$visualDropZone.off('click');\r\n");
            sb.Append("$fileInput.off('click');\r\n");
            sb.Append("$mainDropZone.off('dragover drop dragleave');\r\n");
            sb.Append("$fileInput.off('change');\r\n\r\n");

            // Eventos de clique para abrir o seletor de arquivo
            sb.Append("$visualDropZone.on('click', function () {\r\n");
            sb.Append("    $fileInput.click();\r\n");
            sb.Append("});\r\n\r\n");

            sb.Append("$fileInput.on('click', function (e) {\r\n");
            sb.Append("    e.stopPropagation();\r\n"); // Impede a propagação do clique do input para evitar conflitos
            sb.Append("});\r\n\r\n");

            // Eventos de Drag & Drop na área principal
            sb.Append("$mainDropZone.on('dragover', function (e) {\r\n");
            sb.Append("    e.preventDefault();\r\n");
            sb.Append("    e.stopPropagation();\r\n");
            sb.Append("    $visualDropZone.addClass('dragover');\r\n");
            sb.Append("});\r\n\r\n");

            sb.Append("$mainDropZone.on('dragleave', function (e) {\r\n");
            sb.Append("    e.preventDefault();\r\n");
            sb.Append("    e.stopPropagation();\r\n");
            sb.Append("    // Verifica se o mouse ainda está dentro do mainDropZone ou do visualDropZone\r\n");
            sb.Append("    var rect = $mainDropZone[0].getBoundingClientRect();\r\n");
            sb.Append("    var x = e.originalEvent.clientX;\r\n");
            sb.Append("    var y = e.originalEvent.clientY;\r\n");
            sb.Append("    if (x < rect.left || x >= rect.right || y < rect.top || y >= rect.bottom) {\r\n");
            sb.Append("        $visualDropZone.removeClass('dragover');\r\n");
            sb.Append("    }\r\n");
            sb.Append("});\r\n\r\n");

            sb.Append("$mainDropZone.on('drop', function (e) {\r\n");
            sb.Append("    e.preventDefault();\r\n");
            sb.Append("    e.stopPropagation();\r\n");
            sb.Append("    $visualDropZone.removeClass('dragover');\r\n");
            sb.Append("    var files = e.originalEvent.dataTransfer.files;\r\n");
            sb.Append("    if (files.length > 0) {\r\n");
            sb.Append("        $fileInput[0].files = files;\r\n");
            sb.Append("        displayFileName(files[0].name);\r\n");
            sb.Append("    }\r\n");
            sb.Append("});\r\n\r\n");

            // Evento para exibir o nome do arquivo quando selecionado
            sb.Append("$fileInput.on('change', function () {\r\n");
            sb.Append("    if (this.files.length > 0) {\r\n");
            sb.Append("        displayFileName(this.files[0].name);\r\n");
            sb.Append("    }\r\n");
            sb.Append("});\r\n\r\n");

            // Função para exibir o nome do arquivo
            sb.Append("function displayFileName(name) {\r\n");
            sb.Append("    $fileNameDisplay.text(name);\r\n");
            sb.Append("}\r\n\r\n");

            // **CORREÇÃO CRUCIAL:** Apenas limpa o valor do input, sem recriá-lo
            sb.Append("function clearFileInput() {\r\n");
            sb.Append("    $fileInput.val(''); // Limpa o valor do input type='file'\r\n");
            sb.Append("    $fileNameDisplay.text(''); // Limpa o texto exibido\r\n");
            sb.Append("}\r\n");

            sb.Append("}\r\n"); // Fecha a função initFileUploadScript()

            // Chama a função de inicialização na primeira carga do documento
            sb.Append("$(document).ready(function() { initFileUploadScript(); });\r\n");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_UploadArquivo_Diferenciado", sb.ToString(), true);
        }


        #endregion

        public void AbrirModal(string modalId)
        {
            if (!string.IsNullOrEmpty(ModalId) && string.IsNullOrEmpty(modalId))
            {
                modalId = ModalId;
            }

             FecharModal(modalId);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_OpenModalEnvio", $"$('#{modalId}').modal('show');", true);
        }
        public void FecharModal(string modalId)
        {
            if (!string.IsNullOrEmpty(ModalId) && string.IsNullOrEmpty(modalId))
            {
                modalId = ModalId;
            }

            string script = $@"
        $('#{modalId}').modal('hide');
        $('.modal-backdrop').remove();
        $('body').removeClass('modal-open');
       ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "js_CloseModal_" + modalId, script, true);
        }
    }
}