using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RMon.Globalization.String;
using RMon.ValuesExportImportService.Common;
using RMon.ValuesExportImportService.Excel.Common;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Processing;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Excel
{
    class ExcelWorker : IExcelWorker
    {
        private readonly ILogger _logger;

        private readonly IList<string> _defaultSelectors = new List<string> { "Id" };
        protected const string Selector = "$";

        /// <summary>
        /// Имя сущности
        /// </summary>
        protected I18nString EntityName { get; }

        public ExcelWorker(ILogger<ExcelWorker> logger)
        {
            _logger = logger;
            EntityName = TextExcel.Values;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        /// <summary>
        /// Создаёт книгу Excel и заполняет данными <see cref="exportTable"/>
        /// </summary>
        /// <param name="processingContext"></param>
        /// <param name="exportTable">Список оборудования</param>
        public byte[] WriteWorksheet(IProcessingContext processingContext, ExportTable exportTable)
        {
            try
            {
                using var excelPackage = new ExcelPackage();
                _logger.LogInformation("Формирование книги Excel начато.");
                var timer = Stopwatch.StartNew();
                excelPackage.Workbook.Properties.Author = nameof(ValuesExportImportService);
                excelPackage.Workbook.Properties.Title = TextExcel.EntityProperties.ToString(processingContext.GlobalizationProvider);

                var excelSheet = ExcelMethods.WorksheetCreate(excelPackage.Workbook, EntityName.ToString(processingContext.GlobalizationProvider));

                var entityDescriptionDictionary = exportTable.EntityTable.EntityDescription.GetPropertyDescriptionDictionary(processingContext.GlobalizationProvider);

                var columns = entityDescriptionDictionary.Keys.ToList();

                columns = columns.OrderBy(t => exportTable.PropertyCodes.Contains(t) ? exportTable.PropertyCodes.IndexOf(t) : int.MaxValue).ToList();

                _logger.LogInformation($"Лист \"{EntityName}\": начат процесс формирования.");

                const int colStart = 1; //Первая колонка
                var colEnd = columns.Count; //Последняя колонка

                const int rowStart = 3; //Первая строка
                int rowEnd; //Последняя строка

                var colIndex = colStart;
                var rowIndex = rowStart;

                //строка, столбец
                _logger.LogInformation($"Лист \"{EntityName}\": формирование заголовка.");
                var titleCells = excelSheet.Cells[1, colStart, 1, colEnd];
                titleCells.Value = EntityName.ToString(processingContext.GlobalizationProvider);
                SetTitleStyle(titleCells);

                _logger.LogInformation($"Лист \"{EntityName}\": формирование шапки таблицы.");
                colIndex = 1;

                foreach (var column in columns)
                {
                    if (entityDescriptionDictionary.ContainsKey(column))
                    {
                        excelSheet.Cells[rowIndex, colIndex].Value = _defaultSelectors.Contains(column) ? Selector + column : column;
                        excelSheet.Cells[rowIndex + 1, colIndex].Value = entityDescriptionDictionary[column];
                    }

                    colIndex++;
                }

                rowIndex++;
                SetHeaderStyle(excelSheet.Cells[rowStart, colStart, rowIndex, colEnd]);

                rowIndex++;

                _logger.LogInformation($"Лист \"{EntityName}\": нумерация столбцов.");
                for (var i = colStart; i <= colEnd; i++)
                    excelSheet.Cells[rowIndex, i].Value = i;
                excelSheet.Cells[rowIndex, colStart, rowIndex, colEnd].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                _logger.LogInformation($"Лист \"{EntityName}\": установка фильтров.");
                excelSheet.Cells[rowIndex, colStart, rowIndex, colEnd].AutoFilter = true;
                rowIndex++;

                _logger.LogInformation($"Лист \"{EntityName}\": заполнение таблицы.");
                foreach (var entity in exportTable.EntityTable.Entities)
                {
                    try
                    {
                        colIndex = colStart;
                        var propertyValueDictionary = entity.GetPropertyValueDictionary();

                        foreach (var column in columns)
                        {
                            if (propertyValueDictionary.ContainsKey(column))
                                excelSheet.Cells[rowIndex, colIndex].Value = propertyValueDictionary[column];

                            colIndex++;
                        }
                        rowIndex++;
                    }
                    catch (Exception e)
                    {
                        throw new ExcelException(TextExcel.RowUnexpectedError.With(rowIndex), e);
                    }
                }

                rowEnd = rowIndex - 1;

                _logger.LogInformation($"Лист \"{EntityName}\": автоматическая установка ширины столбцов.");
                for (var i = colStart; i <= colEnd; i++)
                    excelSheet.Column(i).AutoFit();

                _logger.LogInformation($"Лист \"{EntityName}\": форматирование границ ячеек.");
                ExcelMethods.SetBorderStyle(excelSheet.Cells[rowStart, colStart, rowEnd, colEnd]);

                _logger.LogInformation($"Лист \"{EntityName}\": выравнивание текста.");
                excelSheet.Cells[rowStart, colStart, rowEnd, colEnd].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                excelSheet.Cells[rowStart, colStart, rowEnd, colEnd].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                excelSheet.Cells[rowStart, colStart, rowEnd, colEnd].Style.WrapText = true;

                _logger.LogInformation($"Лист \"{EntityName}\": процесс формирования завершен.");

                timer.Stop();
                _logger.LogInformation($"Формирование книги Excel завершено. Затрачено {timer.ElapsedMilliseconds} мс.");
                return excelPackage.GetAsByteArray();
            }
            catch (Exception e)
            {
                throw new ExcelException(TextExcel.SheetUnexpectedError.With(EntityName), e);
            }
        }

        /// <summary>
        /// Устанавливает стиль ячеек заголовка Лист
        /// </summary>
        /// <param name="cells"></param>
        protected virtual void SetTitleStyle(ExcelRange cells)
        {
            cells.Merge = true;
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Font.Size = 14;
            cells.Style.Font.Bold = true;
        }

        /// <summary>
        /// Устанавливает стиль шапке таблицы
        /// </summary>
        /// <param name="cells"></param>
        protected virtual void SetHeaderStyle(ExcelRange cells)
        {
            cells.Style.Font.Bold = true;
            cells.Style.Font.Size = 10;
            cells.Style.Font.Name = "Times New Roman";
        }

    }
}
