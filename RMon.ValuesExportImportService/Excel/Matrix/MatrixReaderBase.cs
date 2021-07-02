using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using ExcelDataReader;
using Microsoft.Extensions.Logging;
using RMon.ValuesExportImportService.Common;
using RMon.ValuesExportImportService.Excel.Common;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Processing.Parse;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Excel.Matrix
{
    abstract class MatrixReaderBase : IMatrixReader
    {
        private readonly ILogger _logger;

        protected MatrixReaderBase(ILogger<MatrixReaderBase> logger)
        {
            _logger = logger;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        
        /// <inheritdoc />
        public List<MatrixResult> ReadExcelBook(byte[] fileBody, ExcelCellAddress logicDevicePropertyValueCell, ExcelCellAddress cellStart, int dateNumber, int timeNumber, ParseProcessingContext context)
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
                    var tableResult =  ParseTable(table, logicDevicePropertyValueCell, cellStart, dateNumber, timeNumber);
                    result.Add(tableResult);
                }
                catch (Exception e)
                {
                    context.LogWarning(e.ConcatExceptionMessage(TextExcel.SheetParseUnexpectedError.With(table.TableName)));
                }


            timer.Stop();
            _logger.LogInformation($"Разбор книги Excel завершен. Затрачено {timer.ElapsedMilliseconds} мс.");

            return result;
        }


        protected abstract MatrixResult ParseTable(DataTable dataTable, ExcelCellAddress logicDevicePropertyValueCell, ExcelCellAddress cellStart, int dateColumnNumber, int timeRowNumber);

        /// <summary>
        /// Парсит строку с часами
        /// </summary>
        /// <param name="hours"></param>
        /// <returns></returns>
        protected int ParseHours(string hours)
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
