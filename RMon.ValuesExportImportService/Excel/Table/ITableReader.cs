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
        /// <param name="logicDevicePropertyValueRowIndex">Номер строки, содержащей значения свойства оборудования</param>
        /// <param name="cellStart">Адрес левой верхней ячейки матрицы</param>
        /// <param name="dateColumnIndex">Номер столбца с датами</param>
        /// <param name="timeColumnIndex">Номер столбца с часами</param>
        /// <param name="context">Контекст</param>
        /// <returns></returns>
        List<ExcelLogicDeviceValues> ReadExcelBook(byte[] fileBody, int logicDevicePropertyValueRowIndex, ExcelCellAddress cellStart, int dateColumnIndex, int timeColumnIndex, ParseProcessingContext context);
    }
}