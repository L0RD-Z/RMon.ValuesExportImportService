using System;
using System.Collections.Generic;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Excel.Common
{
    static class ExcelCellAddressConverter
    {
        private static readonly Dictionary<char, int> Map = new()
        {
            { 'A', 1 },
            { 'B', 2 },
            { 'C', 3 },
            { 'D', 4 },
            { 'E', 5 },
            { 'F', 6 },
            { 'G', 7 },
            { 'H', 8 },
            { 'I', 9 },
            { 'J', 10 },
            { 'K', 11 },
            { 'L', 12 },
            { 'M', 13 },
            { 'N', 14 },
            { 'O', 15 },
            { 'P', 16 },
            { 'Q', 17 },
            { 'R', 18 },
            { 'S', 19 },
            { 'T', 20 },
            { 'U', 21 },
            { 'V', 22 },
            { 'W', 23 },
            { 'X', 24 },
            { 'Y', 25 },
            { 'Z', 26 }
        };

        /// <summary>
        /// Преобразует буквенное представление номера столбца таблицы Excel в индекс
        /// </summary>
        /// <param name="excelColumn"></param>
        /// <returns></returns>
        public static int ExcelColumnToIndex(string excelColumn)
        {
            var result = 0;
            foreach (var ch in excelColumn.Trim().ToUpperInvariant())
                if (Map.TryGetValue(ch, out var number))
                    result = result * Map.Count + number;
                else
                    throw new ExcelException(TextParse.InvalidCharactersError.With(excelColumn, ch));
            return result - 1;
        }

        /// <summary>
        /// Преобразует числовое представление номера столбца таблицы Excel в индекс
        /// </summary>
        /// <param name="excelRow"></param>
        /// <returns></returns>
        public static int ExcelRowToIndex(string excelRow)
        {
            return int.Parse(excelRow) - 1;
        }
        

        /// <summary>
        /// Преобразует буквенное представление адреса ячейки таблицы Excel в числовое
        /// </summary>
        /// <param name="cellAddress"></param>
        /// <returns></returns>
        public static ExcelCellAddress CellAddressConvert(string cellAddress)
        {
            try
            {
                var excelColumn = string.Empty;
                var excelRow = string.Empty;
            
                var upperStr = cellAddress.Trim().ToUpperInvariant();
                for (var i = 0; i < upperStr.Length; i++)
                {
                    var ch = upperStr.Substring(i, 1);
                    if (uint.TryParse(ch, out _))
                    {
                        excelColumn = upperStr.Substring(0, i);
                        excelRow = upperStr.Substring(i, upperStr.Length - i);
                        break;
                    }
                }

                return new ExcelCellAddress(ExcelColumnToIndex(excelColumn) , ExcelRowToIndex(excelRow));
            }
            catch (Exception e)
            {
                throw new ExcelException(TextParse.InvalidCellAddressError.With(cellAddress), e);
            }
        }
    }
}
