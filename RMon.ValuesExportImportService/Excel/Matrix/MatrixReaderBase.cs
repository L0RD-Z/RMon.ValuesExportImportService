using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using ExcelDataReader;
using RMon.ValuesExportImportService.Common;
using RMon.ValuesExportImportService.Excel.Common;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Processing.Parse;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Excel.Matrix
{
    abstract class MatrixReaderBase : IMatrixReader
    {
        protected MatrixReaderBase()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        
        /// <inheritdoc />
        public List<ExcelLogicDeviceValues> ReadExcelBook(byte[] fileBody, ExcelCellAddress logicDevicePropertyValueCell, ExcelCellAddress cellStart, int dateNumber, int timeNumber, ParseProcessingContext context)
        {
            using var stream = new MemoryStream(fileBody);
            // Авто-определение форматов, поддерживаются:
            //  - Binary Excel files (2.0-2003 format; *.xls)
            //  - OpenXml Excel files (2007 format; *.xlsx)
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var data = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                UseColumnDataType = false
            });


            var result = new List<ExcelLogicDeviceValues>();
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

            return result;
        }


        protected abstract ExcelLogicDeviceValues ParseTable(DataTable dataTable, ExcelCellAddress logicDevicePropertyValueCell, ExcelCellAddress cellStart, int dateColumnNumber, int timeRowNumber);

        /// <summary>
        /// Парсит строку с часами
        /// </summary>
        /// <param name="hours"></param>
        /// <returns></returns>
        protected int ParseHours(string hours)
        {
            try
            {
                if (DateTime.TryParseExact(hours, "G", CultureInfo.CurrentCulture, DateTimeStyles.None, out var dateTime))
                    return dateTime.Hour;
                if (DateTime.TryParseExact(hours, "G", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                    return dateTime.Hour;

                var arr = hours.Split('-');
                switch (arr.Length)
                {
                    case 1:
                    {
                        var arr2 = arr[0].Split(':');
                        if (arr2.Length == 1 || arr2.Length == 2)
                            return int.Parse(arr2[0].Trim());
                        throw new Exception();
                    }
                    case 2: return int.Parse(arr[1].Trim());
                    default: throw new Exception();
                }
            }
            catch (Exception e)
            {
                throw new ExcelException(TextExcel.IncorrectHoursFormatError.With(hours), e);
            }
        }
    }
}
