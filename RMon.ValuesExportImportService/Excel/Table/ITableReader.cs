using System.Collections.Generic;
using RMon.ValuesExportImportService.Excel.Common;
using RMon.ValuesExportImportService.Processing.Parse;

namespace RMon.ValuesExportImportService.Excel.Table
{
    internal interface ITableReader
    {
        /// <summary>
        /// Выполняет чтение книги Excel
        /// </summary>
        /// <param name="fileBody">Книга Excel</param>
        /// <param name="logicDevicePropertyValueRowNumber">Номер строки, содержащей значениz свойства оборудования</param>
        /// <param name="cellStart">Адрес левой верхней ячейки матрицы</param>
        /// <param name="dateColumnNumber">Номер столбца с датами</param>
        /// <param name="timeColumnNumber">Номер столбца с часами</param>
        /// <param name="context">Контекст</param>
        /// <returns></returns>
        List<ExcelLogicDeviceValues> ReadExcelBook(byte[] fileBody, int logicDevicePropertyValueRowNumber, ExcelCellAddress cellStart, int dateColumnNumber, int timeColumnNumber, ParseProcessingContext context);
    }
}