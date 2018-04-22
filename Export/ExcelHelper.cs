// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcelHelper.cs">(c) 2018 Mike Fourie and Contributors (https://github.com/mikefourie/GitMonitor) under MIT License. See https://opensource.org/licenses/MIT</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Export
{
    using System;
    using System.IO;
    using Microsoft.Office.Interop.Excel;

    public class ExcelHelper
    {
        private static Workbook workbook;
        private static Worksheet worksheet;
        private static Application excelApp;

        public ExcelHelper(string fileName)
        {
            excelApp = new Application { DisplayAlerts = false };

            if (string.IsNullOrEmpty(fileName))
            {
                workbook = excelApp.Workbooks.Add();
                worksheet = (Worksheet)workbook.Sheets.Add();
            }
            else
            {
                var spreadsheetLocation = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                workbook = excelApp.Workbooks.Open(spreadsheetLocation);
            }
        }

        public void AddWorksheet(string name)
        {
            worksheet = (Worksheet)workbook.Sheets.Add();
            worksheet.Name = name;
        }

        public void GetWorksheet(string name)
        {
            worksheet = (Worksheet)workbook.Worksheets[name];
        }

        public void WriteHeaderRow(string headings)
        {
            int c = 1;
            foreach (string header in headings.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                worksheet.Cells[1, c] = header;
                c++;
            }
        }

        public void SaveWorkBook(string path, bool append)
        {
            if (append)
            {
                workbook.Save();
            }
            else
            {
                workbook.SaveAs(path, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
        }

        public void Close()
        {
            workbook.Close(true);
            excelApp.Quit();
            excelApp = null;
        }

        public void Write(int row, int column, dynamic content)
        {
            worksheet.Cells[row, column] = content;
        }

        public void Write(object[,] data, int rows, int columns)
        {
            var startCell = (Range)worksheet.Cells[2, 1];
            var endCell = (Range)worksheet.Cells[rows + 1, columns];
            var writeRange = worksheet.Range[startCell, endCell];
            writeRange.Value2 = data;
        }

        public void Append(object[,] data, int rows, int columns)
        {
            int lastUsedRow = worksheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell, Type.Missing).Row;
            var startCell = (Range)worksheet.Cells[lastUsedRow + 1, 1];
            var endCell = (Range)worksheet.Cells[lastUsedRow + rows, columns];
            var writeRange = worksheet.Range[startCell, endCell];
            writeRange.Value2 = data;
        }
        
        public int GetLastRow(string file)
        {
            return worksheet.Cells.Find(
                "*",
                worksheet.Cells[1, 1],
                XlFindLookIn.xlFormulas,
                XlLookAt.xlPart,
                XlSearchOrder.xlByRows,
                XlSearchDirection.xlPrevious,
                Type.Missing,
                Type.Missing,
                Type.Missing).Row + 1;
        }
    }
}
