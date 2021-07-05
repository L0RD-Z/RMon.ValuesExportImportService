using System.Collections.Generic;
using RMon.ValuesExportImportService.Excel.Common;
using RMon.ValuesExportImportService.Processing.Parse;

namespace RMon.ValuesExportImportService.Excel.Matrix
{
    internal interface IMatrixReader
    {
        /// <summary>
        /// Выполняет чтение книги Excel
        /// </summary>
        /// <param name="fileBody">Книга Excel</param>
        /// <param name="logicDevicePropertyValueCell">Адрес ячейки, содержащей значение свойства оборудования</param>
        /// <param name="cellStart">Адрес левой верхней ячейки матрицы</param>
        /// <param name="dateNumber">Номер строки (столбца) с датами</param>
        /// <param name="timeNumber">Номер столбца (строки) с часами</param>
        /// <param name="context">Контекст</param>
        /// <returns></returns>
        List<ExcelLogicDeviceValues> ReadExcelBook(byte[] fileBody, ExcelCellAddress logicDevicePropertyValueCell, ExcelCellAddress cellStart, int dateNumber, int timeNumber, ParseProcessingContext context);
    }
}