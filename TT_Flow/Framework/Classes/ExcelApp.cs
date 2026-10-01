using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.EnterpriseServices;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.DynamicData;
using System.Web.UI;
using System.Web.UI.WebControls;
using TT.FrameWork;
using TT_Flow.App.Controles;

namespace TT_Flow.FrameWork
{
    public class ExcelApp
    {
        public ExcelApp()
        {

        }
        public static DataTable ConverterExcelDatatable(byte[] excelDocumentAsBytes, bool hasHeaderRow, string worksheetName)
        {
            DataTable dt = new DataTable();
            string errorMessages = "";
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            if (excelDocumentAsBytes == null)
            {
                return dt;
            }

            using (MemoryStream stream = new MemoryStream(excelDocumentAsBytes))
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                if (excelPackage.Workbook.Worksheets.Count < 1)
                {
                    throw new Exception("Não há tabela dentro da planilha.");
                }
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault(sheet => sheet.Name == worksheetName);
                if (excelPackage.Workbook.Worksheets.Count == 0)
                {
                    throw new Exception("There is only one worksheet in the Excel package. Index out of range exception could occur.");
                }
                if (worksheet == null)
                {
                    throw new Exception("Planilha '" + worksheetName + "' não encontrada.");
                }
                //if (worksheet.Dimension == null)
                //{
                //    return dt;
                //}
                try
                {


                    for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                    {
                        string columnName = "Column " + j;
                        var excelCell = worksheet.Cells[1, j].Value;

                        if (excelCell != null)
                        {
                            var excelCellDataType = excelCell;

                            if (hasHeaderRow == true)
                            {
                                excelCellDataType = worksheet.Cells[2, j].Value;

                                columnName = excelCell.ToString().Trim().ToUpper();

                                if (dt.Columns.Contains(columnName) == true)
                                {
                                    columnName = columnName + "_" + j;
                                }
                            }

                            if (excelCellDataType is DateTime)
                            {
                                dt.Columns.Add(columnName, typeof(DateTime));
                            }
                            else if (excelCellDataType is Boolean)
                            {
                                dt.Columns.Add(columnName, typeof(Boolean));
                            }
                            else if (excelCellDataType is Double)
                            {
                                if (excelCellDataType.ToString().Contains(".") || excelCellDataType.ToString().Contains(","))
                                {
                                    dt.Columns.Add(columnName, typeof(Decimal));
                                }
                                else
                                {
                                    dt.Columns.Add(columnName, typeof(Int64));
                                }
                            }
                            else
                            {
                                dt.Columns.Add(columnName, typeof(String));
                            }
                        }
                        else
                        {
                            dt.Columns.Add(columnName, typeof(String));
                        }
                    }
                }
                catch
                {
                    return dt;
                }

                for (int i = worksheet.Dimension.Start.Row + Convert.ToInt32(hasHeaderRow); i <= worksheet.Dimension.End.Row; i++)
                {
                    DataRow row = dt.NewRow();

                    for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                    {
                        var excelCell = worksheet.Cells[i, j].Value;


                        if (excelCell != null)
                        {
                            try
                            {
                                row[j - 1] = excelCell;
                            }
                            catch
                            {
                                errorMessages += "Row " + (i - 1) + ", Column " + j + ". Invalid " + dt.Columns[j - 1].DataType.ToString().Replace("System.", "") + " value:  " + excelCell.ToString() + "<br>";
                            }
                        }
                    }

                    dt.Rows.Add(row);
                }
            }
            return dt;
        }
        static DataTable ConverterExcelDatatable(byte[] data, HttpPostedFile postedFile)
        {
            DataTable ds = new DataTable();




            return ds;
        }
        public static DataTable ConverterGridViewWorkbook(GridView gv)
        {
            DataTable dt = new DataTable();

            //adiciona os títulos
            foreach (DataControlField campo in gv.Columns)
            {
                if (!string.IsNullOrEmpty(campo.HeaderText))
                {
                    dt.Columns.Add(campo.HeaderText);
                }
            }
            //adiciona os dados
            foreach (GridViewRow row in gv.Rows)
            {
                DataRow dataRow = dt.NewRow();

                for (int i = 0; i < gv.Columns.Count; i++)
                {
                    if (gv.Columns[i] is BoundField)
                    {
                        dataRow[i] = row.Cells[i].Text;
                    }
                    else if ((gv.Columns[i] is HyperLinkField))
                    {
                        dataRow[i] = ((HyperLink)row.Cells[i].Controls[0]).Text;
                    }
                }
                dt.Rows.Add(dataRow);
            }
            return dt;

        }
        public void CriarPlanilhaExcel(DataTable dt, string sNomePlanilha)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            ExcelPackage excelPackage = new ExcelPackage();
            var worksheet = excelPackage.Workbook.Worksheets.Add(sNomePlanilha);

            worksheet.TabColor = System.Drawing.Color.Black;
            worksheet.DefaultRowHeight = 12;

            // Escrever os títulos das colunas na primeira linha do worksheet
            int offset_X = 1;
            int offset_Y = 1;
            foreach (DataColumn column in dt.Columns)
            {
                worksheet.Cells[1, offset_X].Value = column.ColumnName;
                offset_X++;
            }

            // Escrever os dados do DataTable nas células do worksheet
            offset_X = 1; // Iniciar a partir da segunda linha (abaixo dos títulos)
            foreach (DataRow row in dt.Rows)
            {
                offset_Y = 1;
                foreach (DataColumn column in dt.Columns)
                {
                    worksheet.Cells[offset_X, offset_Y].Value = row[column.ColumnName];
                    offset_Y++;
                }
                offset_X++;
            }

            // Salvar o arquivo Excel
            string caminhoArquivo = "caminho/do/arquivo.xlsx";
            excelPackage.SaveAs(new FileInfo(caminhoArquivo));


        }
        public static byte[] ExportarArquivoExcel(DataTable ds, string nomeAba, HttpResponse Response, string endereco, Page page)
        {
            DataTable dataTable = ds;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(nomeAba);

                for (int col = 0; col < dataTable.Columns.Count; col++)
                {
                    worksheet.Cells[1, col + 1].Value = dataTable.Columns[col].ColumnName;
                }

                for (int row = 0; row < dataTable.Rows.Count; row++)
                {
                    for (int col = 0; col < dataTable.Columns.Count; col++)
                    {
                        worksheet.Cells[row + 2, col + 1].Value = dataTable.Rows[row][col];
                    }
                }
                byte[] bytes = package.GetAsByteArray();

                FileStream lObjFile;
                TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                lObjFile = objArquivo.TransformarArrayBytesEmArquivo(bytes, endereco);
                lObjFile.Close();

                Funcoes.DownloadArquivo(page, nomeAba);
                return bytes;

            }
        }
        public static byte[] ExportarArquivoExcel(GridView gv, string nomeAba, HttpResponse Response, string endereco, Page page)
        {
            DataTable dataTable = ConverterGridViewWorkbook(gv);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(nomeAba);

                for (int col = 0; col < dataTable.Columns.Count; col++)
                {
                    worksheet.Cells[1, col + 1].Value = dataTable.Columns[col].ColumnName;
                }

                for (int row = 0; row < dataTable.Rows.Count; row++)
                {
                    for (int col = 0; col < dataTable.Columns.Count; col++)
                    {
                        worksheet.Cells[row + 2, col + 1].Value = dataTable.Rows[row][col];
                    }
                }
                byte[] bytes = package.GetAsByteArray();

                FileStream lObjFile;
                TT.FrameWork.Arquivo objArquivo = new TT.FrameWork.Arquivo();
                lObjFile = objArquivo.TransformarArrayBytesEmArquivo(bytes, endereco);
                lObjFile.Close();

                Funcoes.DownloadArquivo(page, nomeAba);
                return bytes;

            }
        }
        public string ExcelLeitura(byte[] excelDocumentAsBytes, int cell1, int cell2, string sheetName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (MemoryStream stream = new MemoryStream(excelDocumentAsBytes))
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                ExcelWorksheet ws = excelPackage.Workbook.Worksheets.FirstOrDefault(sheet => sheet.Name == sheetName);

                if (ws != null)
                {
                    object dados = ws.Cells[cell1, cell2].Value;
                    if (dados != null)
                    {
                        return dados.ToString();
                    }
                    else
                    {
                        throw new Exception("A célula está vazia.");
                    }
                }
                else
                {
                    throw new Exception("A planilha especificada não foi encontrada.");
                }
            }
        }
        public static List<string> ListaAbas(byte[] excelDocumentAsBytes)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<string> NomeAbas = new List<string>();
            using (MemoryStream stream = new MemoryStream(excelDocumentAsBytes))
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                //ExcelWorksheet ws = excelPackage.Workbook.
                NomeAbas = excelPackage.Workbook.Worksheets.Select(x => x.Name).ToList();
            }
            return NomeAbas;
        }
    }
}