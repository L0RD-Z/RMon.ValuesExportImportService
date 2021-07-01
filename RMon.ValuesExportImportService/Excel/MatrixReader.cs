using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.Extensions.Logging;
using RMon.ValuesExportImportService.Common;
using RMon.ValuesExportImportService.Excel.Common;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Processing.Parse;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Excel
{
    class MatrixReader
    {
        private readonly ILogger _logger;
        private readonly string _entityName = "Матрица 24x31";

        public MatrixReader(ILogger<MatrixReader> logger)
        {
            _logger = logger;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public List<MatrixResult> ReadExcelBook(byte[] fileBody, ExcelCellAddress logicDevicePropertyValueCell, ExcelCellAddress cellStart, int dateColumnNumber, int timeRowNumber, ParseProcessingContext context)
        {
            _logger.LogInformation("Разбор книги Excel начат.");
            var timer = Stopwatch.StartNew();

            using var stream = new MemoryStream(fileBody);
            // Авто-определение форматов, поддерживаются:
            //  - Binary Excel files (2.0-2003 format; *.xls)
            //  - OpenXml Excel files (2007 format; *.xlsx)
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var data = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                UseColumnDataType = false
            });


            var result = new List<MatrixResult>();
            foreach (DataTable table in data.Tables)
                try
                {
                    var r =  ParseMatrix24X31(table, logicDevicePropertyValueCell, cellStart, dateColumnNumber, timeRowNumber);
                    result.Add(r);
                }
                catch (Exception e)
                {
                    context.LogWarning(e.ConcatExceptionMessage(TextExcel.SheetParseUnexpectedError.With(table.TableName)));
                }


            timer.Stop();
            _logger.LogInformation($"Разбор книги Excel завершен. Затрачено {timer.ElapsedMilliseconds} мс.");

            return result;
        }


        private MatrixResult ParseMatrix24X31(DataTable dataTable, ExcelCellAddress logicDevicePropertyValueCell, ExcelCellAddress cellStart, int dateColumnNumber, int timeRowNumber)
        {
            _logger.LogInformation($"Лист \"{dataTable.TableName}\": начат процесс парсинга.");
            var rowIndex = 0;

            /*т.к. нумерация ячеек в библиотеке ведется от нуля*/
            logicDevicePropertyValueCell = new ExcelCellAddress(logicDevicePropertyValueCell.ColNumber - 1, logicDevicePropertyValueCell.RowNumber - 1);
            cellStart = new ExcelCellAddress(cellStart.ColNumber - 1, cellStart.RowNumber - 1);
            dateColumnNumber--;
            timeRowNumber--;

            var colEnd = cellStart.ColNumber + 24 - 1;
            var rowEnd = cellStart.RowNumber + 31 - 1;

            string logicDevicePropertyValue;
            try
            {
                logicDevicePropertyValue = dataTable.Rows[logicDevicePropertyValueCell.RowNumber].ItemArray[logicDevicePropertyValueCell.ColNumber].ToString();
            }
            catch (Exception)
            {
                throw new ExcelException(TextExcel.FailedParseLogicDevicePropertyValueError);
            }

            var result = new MatrixResult { LogicDevicePropertyValue = logicDevicePropertyValue };

            foreach (DataRow row in dataTable.Rows)
                try
                {
                    if (rowIndex >= cellStart.RowNumber && rowIndex <= rowEnd)
                    {
                        var colIndex = 0;

                        var dateStr = row.ItemArray[dateColumnNumber].ToString();
                        if (!string.IsNullOrEmpty(dateStr))
                        {
                            if (!DateTime.TryParse(dateStr, out var date))
                                throw new ExcelException(TextExcel.IncorrectDateFormatError.With(dateStr));

                            foreach (var cell in row.ItemArray)
                                try
                                {
                                    if (colIndex >= cellStart.ColNumber && colIndex <= colEnd)
                                    {
                                        var hoursStr = dataTable.Rows[timeRowNumber].ItemArray[colIndex].ToString();
                                        var hour = ParseHours(hoursStr);

                                        var valueStr = cell.ToString();
                                        if (!string.IsNullOrEmpty(valueStr))
                                        {
                                            if (!double.TryParse(valueStr, out var value))
                                                throw new ExcelException(TextExcel.IncorrectValueFormatError.With(valueStr));

                                            result.AddValue(date.AddHours(hour), value);
                                        }
                                    }

                                    colIndex++;
                                }
                                catch (Exception e)
                                {
                                    throw new ExcelException(TextExcel.ColParseUnexpectedError.With(colIndex + 1), e);
                                }
                        }
                    }
                    rowIndex++;
                }
                catch (Exception e)
                {
                    throw new ExcelException(TextExcel.RowParseUnexpectedError.With(rowIndex + 1), e);
                }

            _logger.LogInformation($"Лист \"{dataTable.TableName}\": процесс парсинга завершен.");
            return result;
        }

        /// <summary>
        /// Парсит строку с часами
        /// </summary>
        /// <param name="hours"></param>
        /// <returns></returns>
        private int ParseHours(string hours)
        {
            try
            {
                var arr = hours.Split('-');
                return int.Parse(arr[1]);
            }
            catch (Exception e)
            {
                throw new ExcelException(TextExcel.IncorrectHoursFormatError.With(hours), e);
            }
        }
    }
}
