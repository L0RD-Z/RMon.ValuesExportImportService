using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;
using RMon.ValuesExportImportService.Common;
using RMon.ValuesExportImportService.Excel.Common;
using RMon.ValuesExportImportService.Extensions;
using RMon.ValuesExportImportService.Processing.Parse;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Excel.Table
{
    class TableReader : ITableReader
    {
        public TableReader()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }


        /// <inheritdoc />
        public List<ExcelLogicDeviceValues> ReadExcelBook(byte[] fileBody, int logicDevicePropertyValueRowIndex, ExcelCellAddress cellStart, int dateColumnIndex, int timeColumnIndex, ParseProcessingContext context)
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
                    var tableResult = ParseTable(table, logicDevicePropertyValueRowIndex, cellStart, dateColumnIndex, timeColumnIndex);
                    result.AddRange(tableResult);
                }
                catch (Exception e)
                {
                    context.LogWarning(e.ConcatExceptionMessage(TextExcel.SheetParseUnexpectedError.With(table.TableName)));
                }

            return result;
        }

        private List<ExcelLogicDeviceValues> ParseTable(DataTable dataTable, int logicDevicePropertyValueRowIndex, ExcelCellAddress cellStart, int dateColumnIndex, int timeColumnIndex)
        {

            var result = new List<ExcelLogicDeviceValues>();
            for (var colIndex = 0; colIndex < dataTable.Columns.Count; colIndex++)
                try
                {
                    var rowIndex = 0;

                    if (colIndex >= cellStart.ColIndex)
                    {
                        var logicDevicePropertyValue = dataTable.Rows[logicDevicePropertyValueRowIndex].ItemArray[colIndex].ToString();

                        if (!string.IsNullOrEmpty(logicDevicePropertyValue))
                        {
                            var matrixResult = new ExcelLogicDeviceValues
                            {
                                SheetName = dataTable.TableName,
                                LogicDevicePropertyValue = logicDevicePropertyValue
                            };

                            foreach (DataRow row in dataTable.Rows)
                                try
                                {
                                    if (rowIndex >= cellStart.RowIndex)
                                    {
                                        var dateStr = row.ItemArray[dateColumnIndex].ToString();
                                        if (!string.IsNullOrEmpty(dateStr))
                                        {
                                            if (!DateTime.TryParse(dateStr, out var date))
                                                throw new ExcelException(TextExcel.IncorrectDateFormatError.With(dateStr));

                                            var timeStr = row.ItemArray[timeColumnIndex].ToString();
                                            if (!string.IsNullOrEmpty(timeStr))
                                            {
                                                if (!DateTime.TryParse(timeStr, out var time))
                                                    throw new ExcelException(TextExcel.IncorrectTimeFormatError.With(timeStr));

                                                var valueStr = row.ItemArray[colIndex].ToString();
                                                if (!string.IsNullOrEmpty(valueStr))
                                                {
                                                    if (!double.TryParse(valueStr, out var value))
                                                        throw new ExcelException(TextExcel.IncorrectValueFormatError.With(valueStr));

                                                    matrixResult.AddValue(date.AddHours(time.Hour).AddMinutes(time.Minute).AddSeconds(time.Second), value);
                                                }
                                            }
                                        }
                                    }
                                    rowIndex++;
                                }
                                catch (Exception e)
                                {
                                    throw new ExcelException(TextExcel.RowParseUnexpectedError.With(rowIndex + 1), e);
                                }

                            result.Add(matrixResult);
                        }
                    }

                    //colIndex++;
                }
                catch (Exception e)
                {
                    throw new ExcelException(TextExcel.ColParseUnexpectedError.With(colIndex + 1), e);
                }

            return result;
        }
    }
}
