using System;
using System.Data;
using RMon.ValuesExportImportService.Excel.Common;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Excel.Matrix
{
    class Matrix31X24Reader : MatrixReaderBase
    {
        protected override ExcelLogicDeviceValues ParseTable(DataTable dataTable, ExcelCellAddress logicDevicePropertyValueCell, ExcelCellAddress cellStart, int dateRowNumber, int timeColumnNumber)
        {
            var rowIndex = 0;

            /*т.к. нумерация ячеек в библиотеке ведется от нуля*/
            logicDevicePropertyValueCell = new ExcelCellAddress(logicDevicePropertyValueCell.ColNumber - 1, logicDevicePropertyValueCell.RowNumber - 1);
            cellStart = new ExcelCellAddress(cellStart.ColNumber - 1, cellStart.RowNumber - 1);
            dateRowNumber--;
            timeColumnNumber--;

            var colEnd = cellStart.ColNumber + 31 - 1;
            var rowEnd = cellStart.RowNumber + 24 - 1;

            string logicDevicePropertyValue;
            try
            {
                logicDevicePropertyValue = dataTable.Rows[logicDevicePropertyValueCell.RowNumber].ItemArray[logicDevicePropertyValueCell.ColNumber].ToString();
            }
            catch (Exception)
            {
                throw new ExcelException(TextExcel.FailedParseLogicDevicePropertyValueError);
            }

            var result = new ExcelLogicDeviceValues
            {
                SheetName = dataTable.TableName,
                LogicDevicePropertyValue = logicDevicePropertyValue
            };

            foreach (DataRow row in dataTable.Rows)
                try
                {
                    if (rowIndex >= cellStart.RowNumber && rowIndex <= rowEnd)
                    {
                        var colIndex = 0;

                        var hoursStr = row.ItemArray[timeColumnNumber].ToString();
                        var hour = ParseHours(hoursStr);
                        
                        foreach (var cell in row.ItemArray)
                            try
                            {
                                if (colIndex >= cellStart.ColNumber && colIndex <= colEnd)
                                {
                                    var dateStr = dataTable.Rows[dateRowNumber].ItemArray[colIndex].ToString();
                                    if (!string.IsNullOrEmpty(dateStr))
                                    {
                                        if (!DateTime.TryParse(dateStr, out var date))
                                            throw new ExcelException(TextExcel.IncorrectDateFormatError.With(dateStr));

                                        var valueStr = cell.ToString();
                                        if (!string.IsNullOrEmpty(valueStr))
                                        {
                                            if (!double.TryParse(valueStr, out var value))
                                                throw new ExcelException(TextExcel.IncorrectValueFormatError.With(valueStr));

                                            result.AddValue(date.AddHours(hour), value);
                                        }
                                    }
                                }

                                colIndex++;
                            }
                            catch (Exception e)
                            {
                                throw new ExcelException(TextExcel.ColParseUnexpectedError.With(colIndex + 1), e);
                            }
                        
                    }
                    rowIndex++;
                }
                catch (Exception e)
                {
                    throw new ExcelException(TextExcel.RowParseUnexpectedError.With(rowIndex + 1), e);
                }

            return result;
        }
    }
}
