using System.Collections.Generic;
using System.Linq;

namespace RMon.ValuesExportImportService.Common
{
    public static class StringMethods
    {
        /// <summary>
        /// Удаляет из строки <see cref="str"/> все вхождения строки <see cref="removedStr"/>
        /// </summary>
        /// <param name="str">Исходная строка</param>
        /// <param name="removedStr">Удаляемая подстрока</param>
        /// <returns></returns>
        public static string Remove(this string str, string removedStr) => str.Replace(removedStr, string.Empty);
        
        /// <summary>
        /// Удаляет из строки <see cref="str"/> все символы <see cref="removedSymbols"/>
        /// </summary>
        /// <param name="str">Исходная строка</param>
        /// <param name="removedSymbols">Удаляемые символы</param>
        /// <returns></returns>
        public static string Remove(this string str, IList<char> removedSymbols)
        {
            return removedSymbols.Aggregate(str, (current, removedSymbol) => current.Remove(removedSymbol.ToString()));
        }
    }
}
