using System;
using System.Data;
using RMon.ValuesExportImportService.Excel.Common;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Excel.Matrix
{
    class Matrix31X24Reader : MatrixReaderBase
    {
        private const int ColCount = 31;
        private const int RowCount = 24;

        protected override ExcelLogicDeviceValues ParseTable(DataTable dataTable, ExcelCellAddress logicDevicePropertyValueCell, ExcelCellAddress cellStart, int dateColumnIndex, int timeRowIndex)
        {
            var rowIndex = 0;
            var colEnd = cellStart.ColIndex + ColCount - 1;
            var rowEnd = cellStart.RowIndex + RowCount - 1;

            string logicDevicePropertyValue;
            try
            {
                logicDevicePropertyValue = dataTable.Rows[logicDevicePropertyValueCell.RowIndex].ItemArray[logicDevicePropertyValueCell.ColIndex].ToString();
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
                    if (rowIndex >= cellStart.RowIndex && rowIndex <= rowEnd)
                    {
                        var colIndex = 0;

                        var hoursStr = row.ItemArray[timeRowIndex].ToString();
                        var hour = ParseHours(hoursStr);
                        
                        foreach (var cell in row.ItemArray)
                            try
                            {
                                if (colIndex >= cellStart.ColIndex && colIndex <= colEnd)
                                {
                                    var dateStr = dataTable.Rows[dateColumnIndex].ItemArray[colIndex].ToString();
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
