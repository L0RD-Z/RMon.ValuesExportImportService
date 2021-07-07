using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RMon.Data.Provider.Units.Backend.Common;
using RMon.Globalization;
using RMon.Globalization.String;
using RMon.ValuesExportImportService.Common;
using RMon.ValuesExportImportService.Excel.Common;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Processing.Parse;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Excel.Flexible
{
    public class ExcelWorker : IExcelWorker
    {
        private readonly ILogger _logger;

        private readonly IList<string> _defaultSelectors = new List<string> { "Id" };
        protected const string Selector = "$";

        /// <summary>
        /// Имя сущности
        /// </summary>
        protected I18nString EntityName { get; }

        /// <summary>
        /// Код сущности
        /// </summary>
        protected string EntityCode { get; }

        public ExcelWorker(ILogger<ExcelWorker> logger)
        {
            _logger = logger;
            EntityName = TextExcel.Values;
            EntityCode = TextExcel.Values.ToString();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        /// <inheritdoc/>
        public byte[] WriteFile(ExportTable exportTable, IGlobalizationProvider globalizationProvider)
        {
            try
            {
                using var excelPackage = new ExcelPackage();
                _logger.LogInformation("Формирование книги Excel начато.");
                var timer = Stopwatch.StartNew();
                excelPackage.Workbook.Properties.Author = nameof(ValuesExportImportService);
                excelPackage.Workbook.Properties.Title = TextExcel.EntityProperties.ToString(globalizationProvider);

                var excelSheet = ExcelMethods.WorksheetCreate(excelPackage.Workbook, EntityName.ToString(globalizationProvider));

                var entityDescriptionDictionary = exportTable.EntityTable.EntityDescription.GetPropertyDescriptionDictionary(globalizationProvider);

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
                titleCells.Value = EntityName.ToString(globalizationProvider);
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
        /// Выполняет парсинг файла Excel-файла <see cref="fileBody"/>
        /// </summary>
        /// <param name="fileBody">Файл excel</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<List<ReadSheet>> ReadFile(byte[] fileBody, ParseProcessingContext context)
        {
            _logger.LogInformation("Разбор книги Excel начат.");
            await using var stream = new MemoryStream(fileBody);
            using var excelPackage = new ExcelPackage(stream);

            var result = new List<ReadSheet>();
            foreach (var excelSheet in excelPackage.Workbook.Worksheets)
                try
                {
                    var table = ReadWorksheet(excelSheet);
                    result.Add(new ReadSheet(excelSheet.Name, table));
                }
                catch (Exception e)
                {
                    await context.LogWarning(e.ConcatExceptionMessage(TextExcel.SheetParseUnexpectedError.With(excelSheet.Name))).ConfigureAwait(false);
                }

            _logger.LogInformation("Разбор книги Excel завершен.");
            return result;
        }

        private ImportTable ReadWorksheet(ExcelWorksheet excelSheet)
        {
            const int rowStart = 3; //Первая строка
            const int colStart = 1;
            var rowIndex = rowStart;
            var colIndex = colStart;
            var colEnd = excelSheet.Dimension.End.Column;
            var rowEnd = excelSheet.Dimension.End.Row;

            _logger.LogInformation($"Сущность \"{EntityName}\": начат процесс парсинга.");
            if (excelSheet == null)
                throw new ArgumentNullException(nameof(excelSheet));

            _logger.LogInformation($"Сущность \"{EntityName}\": парсинг заголовков.");

            var selectorDescription = new EntityDescription(EntityCode);
            var entityDescription = new EntityDescription(EntityCode);

            var selectorDescriptionMap = new Dictionary<PropertyDescription, int>();
            var entityDescriptionMap = new Dictionary<PropertyDescription, int>();

            while (colIndex <= colEnd)
            {
                var code = excelSheet.Cells[rowIndex, colIndex].Value?.ToString() ?? "";
                var isIdentity = false;

                if (!string.IsNullOrEmpty(code))
                {
                    if (code.StartsWith(Selector))
                    {
                        code = code.Substring(1);
                        isIdentity = true;
                    }

                    if (isIdentity)
                    {
                        var propertyDescription = selectorDescription.ParsePropertyDescription(code);
                        selectorDescriptionMap.Add(propertyDescription, colIndex);
                    }
                    else
                    {
                        var propertyDescription = entityDescription.ParsePropertyDescription(code);
                        entityDescriptionMap.Add(propertyDescription, colIndex);
                    }
                }

                colIndex++;
            }
            rowIndex += 3;

            _logger.LogInformation($"Сущность \"{EntityName}\": парсинг тела таблицы.");

            var entities = new List<ImportEntity>();
            while (rowIndex <= rowEnd)
                try
                {
                    var selectorPropertyMap = new Dictionary<int, PropertyValue>();
                    var entityPropertyMap = new Dictionary<int, PropertyValue>();

                    var entitySelector = selectorDescription.CreateEntity(selectorDescriptionMap, selectorPropertyMap);
                    var entity = entityDescription.CreateEntity(entityDescriptionMap, entityPropertyMap);

                    colIndex = colStart;
                    while (colIndex <= colEnd)
                    {
                        var value = excelSheet.Cells[rowIndex, colIndex].Value?.ToString() ?? "";
                        if (selectorPropertyMap.ContainsKey(colIndex))
                            selectorPropertyMap[colIndex].Value = value;
                        if (entityPropertyMap.ContainsKey(colIndex))
                            entityPropertyMap[colIndex].Value = value;

                        colIndex++;
                    }
                    entities.Add(new ImportEntity
                    {
                        Selector = entitySelector,
                        Entity = entity
                    });

                    rowIndex++;
                }
                catch (Exception e)
                {
                    throw new ExcelException(TextExcel.RowParseUnexpectedError.With(rowIndex), e);
                }

            _logger.LogInformation($"Сущность \"{EntityName}\": процесс парсинга завершен.");

            return new ImportTable
            {
                SelectorDescription = selectorDescription,
                EntityDescription = entityDescription,
                Entities = entities
            };
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
