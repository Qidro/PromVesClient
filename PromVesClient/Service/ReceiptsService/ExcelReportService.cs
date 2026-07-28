using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ExcelReportService> _logger;
        public ExcelReportService(ILogger<ExcelReportService> logger) 
        {
            _logger = logger;
        }
        public async Task<ServiceResult> CreateReport(List<ReceiptDtoExcel> cards, string operatorName)
        {
            try
            {
                string reportPath = Path.Combine(AppContext.BaseDirectory, "Report.xlsx");

                using (var workbook = new XLWorkbook("Templates\\CardTemplate.xlsx"))
                {
                    var ws = workbook.Worksheet(1);

                    int row = 3;
                    //заполнение документа данными квитанции
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
                    //дополнительная информация
                    ws.Cell(row, 1).Value = $"Сумма Нетто: {cards[cards.Count - 1].NetWeight} т.";
                    ws.Cell(row + 1, 1).Value = $"Дата: {DateTime.Today.ToString("dd.MM.yyyy")}";
                    ws.Cell(row + 2, 1).Value = $"Время: {DateTime.Now:HH:mm:ss}";
                    ws.Cell(row + 3, 1).Value = $"Оператор: {operatorName}";

                    ws.Cell(row, 1).Style.Font.FontSize = 16;
                    ws.Cell(row + 1, 1).Style.Font.FontSize = 16;
                    ws.Cell(row + 2, 1).Style.Font.FontSize = 16;
                    ws.Cell(row + 3, 1).Style.Font.FontSize = 16;
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
                return ServiceResult.Ok();
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError($"Шаблон отсутствует: {ex.Message}");
                return ServiceResult.Fail($"Шаблон отсутствует: {ex.Message}");
            }
            catch (DirectoryNotFoundException ex)
            {
                _logger.LogError($"Папки Templates нет: {ex.Message}");
                return ServiceResult.Fail($"Отсвутвует корневая папка Templates нет: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError($"Нет прав на изменение: {ex.Message}");
                return ServiceResult.Fail($"У вас нет прав на изменение файла: {ex.Message}");
            }
            catch (IOException ex)
            {
                _logger.LogError($"Файл занят другим процессом: {ex.Message}");
                return ServiceResult.Fail($"Файл занят другим процессом: {ex.Message}");
            }
            catch (FileFormatException ex)
            {
                _logger.LogError($"Файл поврежден: {ex.Message}");
                return ServiceResult.Fail($"Корневой файл поврежден: {ex.Message}");
            }
            catch (Exception ex)
            {
                 _logger.LogError($"Неизвестная ошибка: {ex.Message}");
                return ServiceResult.Fail($"Неизвестная ошибка: {ex.Message}");
            }
            
            
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

        //метод сохранения файла
        public async Task<ServiceResult> SaveReport(List<ReceiptDtoExcel> cards, string savePath)
        {
            try
            {
                string templatePath = Path.Combine(
                    AppContext.BaseDirectory,
                    "Templates",
                    "CardTemplate.xlsx");

                using var workbook = new XLWorkbook(templatePath);

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

                if (row > 2)
                {
                    // Границы для всех заполненных строк
                    var range = ws.Range(2, 1, row - 1, 12);
                    ws.Cell(row, 1).Value = $"Сумма Нетто: {cards[cards.Count - 1].NetWeight} т.";
                    ws.Cell(row + 1, 1).Value = $"Дата: {DateTime.Today.ToString("dd.MM.yyyy")}";
                    ws.Cell(row + 2, 1).Value = $"Время: {DateTime.Now:HH:mm:ss}";
                    //ws.Cell(row + 3, 1).Value = $"Оператор: {operatorName}";

                    ws.Cell(row, 1).Style.Font.FontSize = 16;
                    ws.Cell(row + 1, 1).Style.Font.FontSize = 16;
                    ws.Cell(row + 2, 1).Style.Font.FontSize = 16;
                    ws.Cell(row + 3, 1).Style.Font.FontSize = 16;
                    range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    range.Style.Alignment.ShrinkToFit = true;
                }

                workbook.SaveAs(savePath);

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не смогли сохранить квитанцию (отчет), причина: {ex.Message}");
                return ServiceResult.Fail(ex.Message);
            }
        }
    }
}
