using ClosedXML.Excel;
using PromVesClient.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
namespace PromVesClient.Service.ReceiptsService
{
    public  class ExcelReportService
    {
        public void CreateReport(List<ReceiptDtoExcel> cards, string operatorName)
        {
            string reportPath = Path.Combine(AppContext.BaseDirectory, "Report.xlsx");

            using (var workbook = new XLWorkbook("Templates\\CardTemplate.xlsx"))
            {
                var ws = workbook.Worksheet(1);

                int row = 3;

                foreach (var card in cards)
                {
                    ws.Cell(row, 1).Value = card.VagonNumber;
                    ws.Cell(row, 2).Value = card.TareWeight;
                    ws.Cell(row, 3).Value = card.GrossWeight;
                    ws.Cell(row, 4).Value = card.NetWeight;
                    ws.Cell(row, 5).Value = card.LoadCapacity;
                    ws.Cell(row, 6).Value = card.LoadDeviation;
                    ws.Cell(row, 7).Value = card.FirstCart;
                    ws.Cell(row, 8).Value = card.SecondCart;
                    ws.Cell(row, 9).Value = card.DifferenceCarts;
                    ws.Cell(row, 10).Value = card.LeftSide;
                    ws.Cell(row, 11).Value = card.RightSide;
                    ws.Cell(row, 12).Value = card.DifferenceSides;
                    row++;
                }

                // Границы для всех заполненных строк
                var range = ws.Range(2, 1, row - 1, 12);
                ws.Cell(row, 1).Value = $"Сумма Нетто: {cards[cards.Count-1].NetWeight} т.";
                ws.Cell(row+1, 1).Value = $"Дата: {DateTime.Today.ToString("dd.MM.yyyy")}";
                ws.Cell(row+2, 1).Value = $"Время: {DateTime.Now:HH:mm:ss}";
                ws.Cell(row+3, 1).Value = $"Оператор: {operatorName}";
                range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                //ws.Columns(1, 4).AdjustToContents();
                range.Style.Alignment.ShrinkToFit = true;
                workbook.SaveAs(reportPath);
            }
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = Path.Combine(AppContext.BaseDirectory, "Report.xlsx"),
                Verb = "print",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true
            };

            Process.Start(psi);
            //PrintExcel(reportPath);
        }
        private void PrintExcel(string filePath)
        {
            Excel.Application excel = null;
            Excel.Workbook workbook = null;

            try
            {
                excel = new Excel.Application
                {
                    Visible = false,
                    DisplayAlerts = false
                };

                workbook = excel.Workbooks.Open(filePath);

                // Печать на принтер по умолчанию
                workbook.PrintOut();

                workbook.Close(false);
                excel.Quit();
            }
            finally
            {
                if (workbook != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);

                if (excel != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        //public void PrintOut()
    }
}
