using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT_Flow.FrameWork;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace TT_Flow.App.Controles
{
    public partial class ExcelImportar : UserControl
    {
        /// <summary>
        /// Recebe e Armazena o ID do campo FileUpload, este deve estar definido na Página onde o Controle está inserido.
        /// Esta propriedade deve possuir o ID do FileUpload, como foi definido na Página, para que o script do Controle funcione corretamente.
        /// <br /> <br />
        /// <b>*Tanto este Controle, quanto o campo FileUpload devem estar fora de qualquer UpdatePanel.</b>
        /// </summary>
        public string ID_FileUpload { get => hddFileUpload_ID.Value; set => hddFileUpload_ID.Value = value; }

        protected void Page_Load(object sender, EventArgs e) => RegistraScript();

        public IEnumerator ImportaExcel(FileUpload fu, bool bCalcula_Formulas = false)
        {
            ValidarEnvioArquivo(fu);

            IWorkbook workbook;

            try { workbook = new XSSFWorkbook(fu.FileContent); } // XLSX
            catch { workbook = new HSSFWorkbook(fu.FileContent); } // XLS

            var rows = workbook.GetSheetAt(0).GetRowEnumerator();
            rows.MoveNext();

            if (bCalcula_Formulas)
            {
                var linhas = workbook.GetSheetAt(0).GetRowEnumerator();
                linhas.MoveNext();

                while (linhas.MoveNext())
                {
                    var linha = (IRow)linhas.Current;
                    foreach (var cell in linha.Cells)
                    {
                        if (cell.CellType == CellType.Formula) workbook.GetCreationHelper().CreateFormulaEvaluator().EvaluateInCell(cell);
                    }
                }
            }

            return rows;
        }

        public List<cls_WMS_Produtos> Retornar_Itens_Excel__Cotacao(FileUpload fu)
        {
            var rows = ImportaExcel(fu);

            List<cls_WMS_Produtos> list = new List<cls_WMS_Produtos>();

            while (rows.MoveNext())
            {
                var row = (IRow)rows.Current;

                if (row == null || row.Cells[0].CellType == CellType.Blank) break;

                var sCodigo = row.GetCell(0).ToString().Trim();
                var sQtd = row.GetCell(1).ToString().Trim();
                decimal.TryParse(sQtd, out decimal nQtd);

                if (!string.IsNullOrEmpty(sCodigo))
                {
                    cls_WMS_Produtos novo = new cls_WMS_Produtos { SCodigo = sCodigo, NQuantidade = nQtd };
                    list.Add(novo);
                }
                else throw new Exception("É necessário que todos os Itens do Arquivo possuam um Código, na segunda coluna dentro do Arquivo Excel!");
            }

            return list;
        }

        public List<cls_WMS_Produtos> Retornar_Itens_Excel__Orcamento(FileUpload fu)
        {
            var rows = ImportaExcel(fu);

            List<cls_WMS_Produtos> list = new List<cls_WMS_Produtos>();

            while (rows.MoveNext())
            {
                var row = (IRow)rows.Current;

                if (row == null || row.Cells[0].CellType == CellType.Blank)
                    break;

                var sOrdem = row.GetCell(0).ToString().Trim();
                var sCodigo = row.GetCell(1).ToString().Trim();
                var sQtd = row.GetCell(2).ToString().Trim();
                int.TryParse(sOrdem, out int nOrdem);
                decimal.TryParse(sQtd, out decimal nQtd);

                if (!string.IsNullOrEmpty(sCodigo))
                {
                    cls_WMS_Produtos novo = new cls_WMS_Produtos
                    {
                        nOrdem = nOrdem
                        ,
                        SCodigo = sCodigo
                        ,
                        NQuantidade = nQtd
                    };

                    list.Add(novo);
                }
                else
                    throw new Exception("É necessário que todos os Itens do Arquivo possuam um Código, na segunda coluna dentro do Arquivo Excel!");

            }

            return list;
        }

        public List<cls_Comercial_Tabelas> Retornar_Itens_Excel__TabelaPreco(FileUpload fu)
        {
            var rows = ImportaExcel(fu, true);

            List<cls_Comercial_Tabelas> list = new List<cls_Comercial_Tabelas>();
            List<(string, int)> colunas = new List<(string, int)>();

            while (rows.MoveNext())
            {
                var row = (IRow)rows.Current;

                string cell_0 = row.GetCell(0).ToString().Trim();
                int.TryParse(cell_0, out int id);

                if (id <= 0)
                {
                    if (cell_0.Equals("ID"))
                    {
                        foreach (var cell in row.Cells)
                        {
                            if (cell.ColumnIndex > 8) colunas.Add((cell.ToString().Trim(), cell.ColumnIndex));
                        }
                    }

                    continue;
                }

                decimal nEnvio = 0, nLocal = 0, nMargem = 0, nFator = 0, nPreco = 0, nTotal = 0, nSD = 0, nND = 0, nN = 0, nCO = 0, nS = 0;

                foreach (var val in colunas)
                {
                    string sValor = row.GetCell(val.Item2).ToString().Trim();
                    decimal.TryParse(sValor, out decimal valor);

                    switch (val.Item1)
                    {
                        case "Taxa Envio": nEnvio = valor; break;
                        case "Taxa Local": nLocal = valor; break;
                        case "Margem": nMargem = valor; break;
                        case "Fator": case "Desconto": nFator = valor; break;
                        case "Preço": nPreco = valor; break;
                        case "Total": nTotal = valor; break;
                        case "Preço - Sudeste": nSD = valor; break;
                        case "Preço - Nordeste": nND = valor; break;
                        case "Preço - Norte": nN = valor; break;
                        case "Preço - Centro-Oeste": nCO = valor; break;
                        case "Preço - Sul": nS = valor; break;
                    }
                }

                cls_Comercial_Tabelas novo = new cls_Comercial_Tabelas
                {
                    idRegistro = id,
                    NEnvio = nEnvio,
                    NLocal = nLocal,
                    NMargem = nMargem,
                    NFator = nFator,
                    Preco = nPreco,
                    NTotal = nTotal,
                    Preco_Zona_SD = nSD,
                    Preco_Zona_ND = nND,
                    Preco_Zona_N = nN,
                    Preco_Zona_CO = nCO,
                    Preco_Zona_S = nS
                };
                list.Add(novo);
            }

            return list;
        }

        public List<cls_WMS_Produtos> Retornar_Itens_Excel_Compras(FileUpload fu)
        {
            List<cls_WMS_Produtos> list = new List<cls_WMS_Produtos>();

            ValidarEnvioArquivo(fu);

            string sExtensao = Path.GetExtension(fu.FileName).ToLower();

            IWorkbook workbook;

            if (sExtensao == ".xlsx")
                workbook = new XSSFWorkbook(fu.FileContent);
            else
                workbook = new HSSFWorkbook(fu.FileContent);

            var sheet = workbook.GetSheetAt(0);
            var rows = sheet.GetRowEnumerator();
            rows.MoveNext();

            while (rows.MoveNext())
            {
                var row = (IRow)rows.Current;

                if (row == null || row.Cells[0].CellType == CellType.Blank)
                    break;

                var sOrdem = row.GetCell(0).ToString().Trim();
                var sCodigo = row.GetCell(1).ToString().Trim();
                var sQtd = row.GetCell(2).ToString().Trim();
                int.TryParse(sOrdem, out int nOrdem);
                decimal.TryParse(sQtd, out decimal nQtd);

                if (!string.IsNullOrEmpty(sCodigo))
                {
                    cls_WMS_Produtos novo = new cls_WMS_Produtos
                    {
                        nOrdem = nOrdem
                        ,
                        SCodigo = sCodigo
                        ,
                        NQuantidade = nQtd
                    };

                    list.Add(novo);
                }
                else
                    throw new Exception("É necessário que todos os Itens do Arquivo possuam seu Código, na segunda coluna dentro do Arquivo Excel!");

            }

            return list;
        }

        private void ValidarEnvioArquivo(FileUpload fu)
        {
            if (!fu.HasFile) throw new Exception("É necessário selecionar um Arquivo para Importar!");
            else if (!Path.GetExtension(fu.FileName).ToLower().Equals(".xlsx") && !Path.GetExtension(fu.FileName).ToLower().Equals(".xls"))
                throw new Exception("É necessário selecionar um Arquivo que seja no formtato de Excel, com a extemsão sendo '.xlsx' ou '.xls'!");
        }

        protected void RegistraScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("var $uploadContainer = $('[id*=uploadContainer]');");
            sb.AppendLine($"var $fileInput = $('[id*={ID_FileUpload}]');");
            sb.AppendLine("var $fileNameDisplay = $('[id*=fileName]');");
            sb.AppendLine("$uploadContainer.off('click').on('click', function () {");
            sb.AppendLine("     $fileInput.click();");
            sb.AppendLine("});");
            sb.AppendLine("$fileInput.on('click', function (e) {");
            sb.AppendLine("     e.stopPropagation();");
            sb.AppendLine("});");
            sb.AppendLine("$uploadContainer.off('dragover').on('dragover', function (e) {");
            sb.AppendLine("     e.preventDefault();");
            sb.AppendLine("     e.stopPropagation();");
            sb.AppendLine("     $uploadContainer.addClass('dragover');");
            sb.AppendLine("});");
            sb.AppendLine("$uploadContainer.off('dragleave').on('dragleave', function (e) {");
            sb.AppendLine("     e.preventDefault();");
            sb.AppendLine("     e.stopPropagation();");
            sb.AppendLine("     $uploadContainer.removeClass('dragover');");
            sb.AppendLine("});");
            sb.AppendLine("$uploadContainer.off('drop').on('drop', function (e) {");
            sb.AppendLine("     e.preventDefault();");
            sb.AppendLine("     e.stopPropagation();");
            sb.AppendLine("     $uploadContainer.removeClass('dragover');");
            sb.AppendLine("     var files = e.originalEvent.dataTransfer.files;");
            sb.AppendLine("     $fileInput[0].files = files;");
            sb.AppendLine("     displayFileName(files[0].name);");
            sb.AppendLine("});");
            sb.AppendLine("$fileInput.off('change').on('change', function () {");
            sb.AppendLine("     if (this.files.length > 0) {");
            sb.AppendLine("         displayFileName(this.files[0].name);");
            sb.AppendLine("     }");
            sb.AppendLine("});");
            sb.AppendLine("function displayFileName(name) {");
            sb.AppendLine("     $fileNameDisplay.text(name);");
            sb.AppendLine("}");

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "js_UploadArquivo_Diferenciado", sb.ToString(), true);
        }
    }
}