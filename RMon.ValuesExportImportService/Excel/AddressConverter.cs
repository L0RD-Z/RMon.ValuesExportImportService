using System;
using System.Collections.Generic;
using System.Linq;
using RMon.ValuesExportImportService.Excel.Common;
using RMon.ValuesExportImportService.Text;

namespace RMon.ValuesExportImportService.Excel
{
    static class AddressConverter
    {
        private static readonly Dictionary<char, uint> _map = new()
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
        /// Преобразует буквенное представление номера столбца таблицы Excel в числовой
        /// </summary>
        /// <param name="colNumber"></param>
        /// <returns></returns>
        public static int ColNumberConvert(string colNumber)
        {
            int result = 0;
            var digit = 0;
            foreach (var ch in colNumber.Trim().ToUpperInvariant().Reverse())
                if (_map.TryGetValue(ch, out var number))
                    result += Convert.ToInt32(number * Math.Pow(_map.Count, digit++));
                else
                    throw new ExcelException(TextParse.InvalidCharactersError.With(colNumber, ch));
            return result;
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
                var colNumber = string.Empty;
                var rowNumber = string.Empty;
            
                var upperStr = cellAddress.Trim().ToUpperInvariant();
                for (var i = 0; i < upperStr.Length; i++)
                {
                    var ch = upperStr.Substring(i, 1);
                    if (uint.TryParse(ch, out _))
                    {
                        colNumber = upperStr.Substring(0, i);
                        rowNumber = upperStr.Substring(i, upperStr.Length - i);
                        break;
                    }
                }

                return new ExcelCellAddress(ColNumberConvert(colNumber) ,int.Parse(rowNumber));
            }
            catch (Exception e)
            {
                throw new ExcelException(TextParse.InvalidCellAddressError.With(cellAddress), e);
            }
        }
    }
}
