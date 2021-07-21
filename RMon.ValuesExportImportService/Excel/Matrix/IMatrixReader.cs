using System.Collections.Generic;
using RMon.ValuesExportImportService.Excel.Common;
using RMon.ValuesExportImportService.Files;
using RMon.ValuesExportImportService.Processing.Parse;

namespace RMon.ValuesExportImportService.Excel.Matrix
{
    internal interface IMatrixReader
    {
        /// <summary>
        /// Выполняет чтение книги Excel
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="logicDevicePropertyValueCell">Адрес ячейки, содержащей значение свойства оборудования</param>
        /// <param name="cellStart">Адрес левой верхней ячейки матрицы</param>
        /// <param name="dateColumnIndex">Номер строки (столбца) с датами</param>
        /// <param name="timeColumnIndex">Номер столбца (строки) с часами</param>
        /// <param name="context">Контекст</param>
        /// <returns></returns>
        List<ExcelLogicDeviceValues> ReadExcelBook(LocalFile file, ExcelCellAddress logicDevicePropertyValueCell, ExcelCellAddress cellStart, int dateColumnIndex, int timeColumnIndex, ParseProcessingContext context);
    }
}